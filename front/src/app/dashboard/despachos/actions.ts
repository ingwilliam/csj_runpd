"use server";

import { revalidatePath } from "next/cache";
import { redirect } from "next/navigation";
import {
  BackendError,
  changeDespachoActivoInBackend,
  createDespachoInBackend,
  getDespachoInBackend,
  listDepartamentosInBackend,
  listMunicipiosInBackend,
  resolverTerritorioInBackend,
  updateDespachoInBackend,
} from "@/lib/backend/client";
import type {
  CrearDespachoRequest,
  Despacho,
  EstadoDespacho,
  Municipio,
  ResolverTerritorioResponse,
} from "@/lib/backend/types";
import {
  ROLE_ADMINISTRADOR_DEL_SISTEMA,
  ROLE_ADMINISTRADOR_UDAE,
  ROLE_CONSULTA,
} from "@/lib/auth/modules";
import { getSession } from "@/lib/auth/session-claims";
import {
  esIdentificadorValido,
  getDespachoValidationError,
} from "@/lib/despachos/despacho-validation";

/** Resultado de una acción de despachos, que la interfaz muestra como notificación. */
export type DespachoActionState = {
  ok: boolean;
  message: string | null;
};

/** Resultado de consultar el detalle de un despacho. */
export type DespachoDetalleResult =
  | { ok: true; despacho: Despacho }
  | { ok: false; message: string };

/** Resultado de consultar los municipios de un departamento. */
export type MunicipiosActionResult =
  | { ok: true; municipios: Municipio[] }
  | { ok: false; message: string };

/** Resultado de resolver el departamento y los datos de un municipio. */
export type MunicipioResueltoResult =
  | { ok: true; departamentoId: string; municipio: Municipio }
  | { ok: false; message: string };

/** Resultado de resolver la clasificación territorial de un despacho. */
export type TerritorioResueltoResult =
  | { ok: true; territorio: ResolverTerritorioResponse }
  | { ok: false; message: string };

/** Parámetros para resolver la clasificación territorial de un despacho. */
export type ResolverTerritorioParams = {
  jurisdiccionId: string;
  municipioId: string;
  subEspecialidadId?: string | null;
};

const DEFAULT_ERROR_MESSAGE = "No se pudo completar la operación. Intenta más tarde.";
const DESPACHOS_PATH = "/dashboard/despachos";
const ROLES_LECTURA = [
  ROLE_ADMINISTRADOR_UDAE,
  ROLE_CONSULTA,
  ROLE_ADMINISTRADOR_DEL_SISTEMA,
];

/**
 * Crea un despacho con los datos del formulario.
 * @param _estadoPrevio Estado anterior que exige `useActionState`; no se usa.
 * @param formData Datos del despacho.
 * @returns Resultado para la interfaz.
 */
export async function crearDespachoAction(
  _estadoPrevio: DespachoActionState,
  formData: FormData,
): Promise<DespachoActionState> {
  const session = await getDespachosWriteSession();

  // Paso 1: leer y validar los datos antes de llamar al backend.
  const datos = leerDespacho(formData);
  const validationError = getDespachoValidationError(datos);
  if (validationError) {
    return { ok: false, message: validationError };
  }

  // Paso 2: crear el despacho.
  try {
    await createDespachoInBackend(session.token, datos);
  } catch (error) {
    return manejarErrorAccion(error, "No se pudo crear el despacho.");
  }

  revalidatePath(DESPACHOS_PATH);
  return { ok: true, message: "El despacho se creó correctamente." };
}

/**
 * Actualiza los datos de un despacho.
 * @param _estadoPrevio Estado anterior que exige `useActionState`; no se usa.
 * @param formData Identificador y datos del despacho.
 * @returns Resultado para la interfaz.
 */
export async function actualizarDespachoAction(
  _estadoPrevio: DespachoActionState,
  formData: FormData,
): Promise<DespachoActionState> {
  const session = await getDespachosWriteSession();
  const id = leerTexto(formData, "id");
  if (!esIdentificadorValido(id)) {
    return { ok: false, message: "El despacho solicitado no es válido." };
  }

  // Paso 1: validar los datos recibidos.
  const datos = leerDespacho(formData);
  const validationError = getDespachoValidationError(datos);
  if (validationError) {
    return { ok: false, message: validationError };
  }

  // Paso 2: guardar los cambios en el backend.
  try {
    await updateDespachoInBackend(session.token, id, datos);
  } catch (error) {
    return manejarErrorAccion(error, "No se pudo actualizar el despacho.");
  }

  revalidatePath(DESPACHOS_PATH);
  return { ok: true, message: "El despacho se actualizó correctamente." };
}

/**
 * Activa o desactiva un despacho.
 * @param id Identificador del despacho.
 * @param activo Nuevo estado activo.
 * @returns Resultado para la interfaz.
 */
export async function cambiarActivoDespachoAction(
  id: string,
  activo: boolean,
): Promise<DespachoActionState> {
  const session = await getDespachosWriteSession();
  if (!esIdentificadorValido(id)) {
    return { ok: false, message: "El despacho solicitado no es válido." };
  }

  try {
    await changeDespachoActivoInBackend(session.token, id, activo);
  } catch (error) {
    return manejarErrorAccion(error, "No se pudo cambiar el estado del despacho.");
  }

  revalidatePath(DESPACHOS_PATH);
  return {
    ok: true,
    message: activo
      ? "El despacho se activó correctamente."
      : "El despacho se desactivó correctamente.",
  };
}

/**
 * Consulta un despacho por su identificador para mostrarlo en el detalle.
 * @param id Identificador del despacho.
 * @returns El despacho o un mensaje de error.
 */
export async function obtenerDespachoAction(id: string): Promise<DespachoDetalleResult> {
  const session = await getDespachosReadSession();
  if (!esIdentificadorValido(id)) {
    return { ok: false, message: "El despacho solicitado no es válido." };
  }

  try {
    return { ok: true, despacho: await getDespachoInBackend(session.token, id) };
  } catch (error) {
    if (error instanceof BackendError && error.status === 401) {
      redirect("/login");
    }
    if (error instanceof BackendError) {
      return { ok: false, message: error.message };
    }

    console.error("Error al consultar el despacho:", error);
    return { ok: false, message: DEFAULT_ERROR_MESSAGE };
  }
}

/**
 * Obtiene los municipios activos de un departamento para el formulario.
 * @param departamentoId Identificador del departamento.
 * @returns Los municipios o un mensaje de error.
 */
export async function obtenerMunicipiosAction(
  departamentoId: string,
): Promise<MunicipiosActionResult> {
  const session = await getDespachosReadSession();
  if (!esIdentificadorValido(departamentoId)) {
    return { ok: false, message: "El departamento seleccionado no es válido." };
  }

  try {
    return {
      ok: true,
      municipios: await listMunicipiosInBackend(session.token, departamentoId),
    };
  } catch (error) {
    if (error instanceof BackendError && error.status === 401) {
      redirect("/login");
    }
    if (error instanceof BackendError) {
      return { ok: false, message: error.message };
    }

    console.error("Error al consultar los municipios:", error);
    return { ok: false, message: "No se pudieron cargar los municipios." };
  }
}

/**
 * Resuelve la clasificación territorial general y específica de un despacho.
 * @param params Jurisdicción y municipio obligatorios; subespecialidad opcional.
 * @returns Las clasificaciones encontradas o un mensaje de error.
 */
export async function resolverTerritorioAction(
  params: ResolverTerritorioParams,
): Promise<TerritorioResueltoResult> {
  const session = await getDespachosReadSession();
  if (!esIdentificadorValido(params.jurisdiccionId)) {
    return { ok: false, message: "La jurisdicción seleccionada no es válida." };
  }
  if (!esIdentificadorValido(params.municipioId)) {
    return { ok: false, message: "El municipio seleccionado no es válido." };
  }
  if (params.subEspecialidadId && !esIdentificadorValido(params.subEspecialidadId)) {
    return { ok: false, message: "La sub especialidad seleccionada no es válida." };
  }

  try {
    return {
      ok: true,
      territorio: await resolverTerritorioInBackend(session.token, {
        jurisdiccionId: params.jurisdiccionId,
        municipioId: params.municipioId,
        subEspecialidadId: params.subEspecialidadId ?? null,
      }),
    };
  } catch (error) {
    if (error instanceof BackendError && error.status === 401) {
      redirect("/login");
    }
    if (error instanceof BackendError) {
      return { ok: false, message: error.message };
    }

    console.error("Error al consultar la clasificación territorial:", error);
    return { ok: false, message: "No se pudo consultar la clasificación territorial." };
  }
}

/**
 * Resuelve a qué departamento pertenece un municipio para inicializar el formulario de edición.
 * Consulta los departamentos y busca el municipio en sus listados.
 * @param municipioId Identificador del municipio.
 * @returns El departamento y el municipio, o un mensaje de error.
 */
export async function resolverMunicipioAction(
  municipioId: string,
): Promise<MunicipioResueltoResult> {
  const session = await getDespachosReadSession();
  if (!esIdentificadorValido(municipioId)) {
    return { ok: false, message: "El municipio del despacho no es válido." };
  }

  try {
    const departamentos = await listDepartamentosInBackend(session.token);
    const listados = await Promise.all(
      departamentos.map((departamento) =>
        listMunicipiosInBackend(session.token, departamento.id),
      ),
    );

    for (let indice = 0; indice < listados.length; indice += 1) {
      const municipio = listados[indice].find((item) => item.id === municipioId);
      if (municipio) {
        return { ok: true, departamentoId: departamentos[indice].id, municipio };
      }
    }

    return { ok: false, message: "No se encontró el municipio del despacho." };
  } catch (error) {
    if (error instanceof BackendError && error.status === 401) {
      redirect("/login");
    }
    if (error instanceof BackendError) {
      return { ok: false, message: error.message };
    }

    console.error("Error al resolver el municipio:", error);
    return { ok: false, message: "No se pudo resolver el municipio del despacho." };
  }
}

/** Exige una sesión con alguno de los roles de lectura de despachos. */
async function getDespachosReadSession() {
  const session = await getSession();
  if (!session) {
    redirect("/login");
  }
  if (!ROLES_LECTURA.some((rol) => session.roles.includes(rol))) {
    redirect("/dashboard");
  }
  return session;
}

/** Exige una sesión con el rol que puede escribir despachos. */
async function getDespachosWriteSession() {
  const session = await getSession();
  if (!session) {
    redirect("/login");
  }
  if (!session.roles.includes(ROLE_ADMINISTRADOR_UDAE)) {
    redirect("/dashboard");
  }
  return session;
}

/** Convierte un fallo del backend en un resultado para la interfaz. */
function manejarErrorAccion(error: unknown, fallback: string): DespachoActionState {
  if (error instanceof BackendError) {
    if (error.status === 401) {
      redirect("/login");
    }
    return { ok: false, message: error.message };
  }

  console.error("Error al administrar despachos:", error);
  return { ok: false, message: fallback || DEFAULT_ERROR_MESSAGE };
}

/** Lee del formulario todos los datos editables de un despacho. */
function leerDespacho(formData: FormData): CrearDespachoRequest {
  return {
    codigo: leerTexto(formData, "codigo"),
    nombre: leerTexto(formData, "nombre"),
    estado: leerTexto(formData, "estado") as EstadoDespacho,
    promiscuo: formData.get("promiscuo") === "on",
    esDescongestion: formData.get("esDescongestion") === "on",
    direccion: leerTextoOpcional(formData, "direccion"),
    complementoDireccion: leerTextoOpcional(formData, "complementoDireccion"),
    telefonoFijo: leerTextoOpcional(formData, "telefonoFijo"),
    telefonoCelular: leerTextoOpcional(formData, "telefonoCelular"),
    correo: leerTextoOpcional(formData, "correo"),
    horarioAtencion: leerTextoOpcional(formData, "horarioAtencion"),
    especialidadId: leerTextoOpcional(formData, "especialidadId"),
    subEspecialidadId: leerTextoOpcional(formData, "subEspecialidadId"),
    municipioId: leerTexto(formData, "municipioId"),
    jurisdiccionId: leerTexto(formData, "jurisdiccionId"),
    distritoId: leerTextoOpcional(formData, "distritoId"),
    circuitoId: leerTextoOpcional(formData, "circuitoId"),
    distritoSubEspecialidadId: leerTextoOpcional(formData, "distritoSubEspecialidadId"),
    circuitoSubEspecialidadId: leerTextoOpcional(formData, "circuitoSubEspecialidadId"),
    entidadId: leerTextoOpcional(formData, "entidadId"),
  };
}

/** Lee un campo de texto del formulario y lo recorta. */
function leerTexto(formData: FormData, nombre: string): string {
  return String(formData.get(nombre) ?? "").trim();
}

/** Lee un campo opcional: devuelve null si está vacío. */
function leerTextoOpcional(formData: FormData, nombre: string): string | null {
  return leerTexto(formData, nombre) || null;
}
