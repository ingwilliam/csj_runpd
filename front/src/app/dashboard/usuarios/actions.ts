"use server";

import { revalidatePath } from "next/cache";
import { redirect } from "next/navigation";
import {
  BackendError,
  changeUsuarioActivoInBackend,
  createUsuarioInBackend,
  getUsuarioInBackend,
  listRolesInBackend,
  updateUsuarioInBackend,
  updateUsuarioRolesInBackend,
} from "@/lib/backend/client";
import type { ActualizarUsuarioRequest, CrearUsuarioRequest, Usuario } from "@/lib/backend/types";
import { ROLE_ADMINISTRADOR_DEL_SISTEMA } from "@/lib/auth/modules";
import { getSession } from "@/lib/auth/session-claims";
import {
  esIdentificadorValido,
  getActualizarUsuarioValidationError,
  getCrearUsuarioValidationError,
  getUsuarioRolesValidationError,
} from "@/lib/usuarios/usuario-validation";

/** Resultado de una acción de usuarios, que la interfaz muestra como notificación. */
export type UsuarioActionState = {
  ok: boolean;
  message: string | null;
};

/** Resultado de consultar el detalle de un usuario. */
export type UsuarioDetalleResult =
  | { ok: true; usuario: Usuario }
  | { ok: false; message: string };

const DEFAULT_ERROR_MESSAGE = "No se pudo completar la operación. Intenta más tarde.";
const USUARIOS_PATH = "/dashboard/usuarios";

/**
 * Crea un usuario con sus roles en una sola petición.
 * @param _estadoPrevio Estado anterior que exige `useActionState`; no se usa.
 * @param formData Datos personales, correo, contraseña y roles seleccionados.
 * @returns Resultado para la interfaz.
 */
export async function crearUsuarioAction(
  _estadoPrevio: UsuarioActionState,
  formData: FormData,
): Promise<UsuarioActionState> {
  const session = await getAdminSession();

  // Paso 1: leer y validar los datos antes de llamar al backend.
  const datos: CrearUsuarioRequest = {
    ...leerDatosPersonales(formData),
    contrasena: leerTexto(formData, "contrasena", false),
    roles: leerRoles(formData),
  };
  const validationError = getCrearUsuarioValidationError(datos);
  if (validationError) {
    return { ok: false, message: validationError };
  }

  // Paso 2: crear el usuario y sus roles en la misma petición.
  try {
    await createUsuarioInBackend(session.token, datos);
  } catch (error) {
    return manejarErrorAccion(error);
  }

  revalidatePath(USUARIOS_PATH);
  return { ok: true, message: "El usuario se creó correctamente." };
}

/**
 * Actualiza los datos de un usuario y reemplaza sus roles en la misma petición.
 * @param _estadoPrevio Estado anterior que exige `useActionState`; no se usa.
 * @param formData Identificador, datos personales y roles seleccionados.
 * @returns Resultado para la interfaz.
 */
export async function actualizarUsuarioAction(
  _estadoPrevio: UsuarioActionState,
  formData: FormData,
): Promise<UsuarioActionState> {
  const session = await getAdminSession();
  const id = leerTexto(formData, "id");
  if (!esIdentificadorValido(id)) {
    return { ok: false, message: "El usuario solicitado no es válido." };
  }

  // Paso 1: validar los datos recibidos.
  const datos: ActualizarUsuarioRequest = {
    ...leerDatosPersonales(formData),
    roles: leerRoles(formData),
  };
  const validationError = getActualizarUsuarioValidationError(datos);
  if (validationError) {
    return { ok: false, message: validationError };
  }

  // Paso 2: impedir que el administrador actual se quite sus propios permisos.
  if (id === session.usuarioId) {
    const errorRolesPropios = await getErrorRolAdministradorPropio(
      session.token,
      datos.roles ?? [],
    );
    if (errorRolesPropios) {
      return { ok: false, message: errorRolesPropios };
    }
  }

  // Paso 3: guardar los cambios en el backend.
  try {
    await updateUsuarioInBackend(session.token, id, datos);
  } catch (error) {
    return manejarErrorAccion(error);
  }

  revalidatePath(USUARIOS_PATH);
  return { ok: true, message: "El usuario se actualizó correctamente." };
}

/**
 * Reemplaza únicamente los roles de un usuario.
 * @param _estadoPrevio Estado anterior que exige `useActionState`; no se usa.
 * @param formData Identificador y roles seleccionados.
 * @returns Resultado para la interfaz.
 */
export async function actualizarRolesAction(
  _estadoPrevio: UsuarioActionState,
  formData: FormData,
): Promise<UsuarioActionState> {
  const session = await getAdminSession();
  const id = leerTexto(formData, "id");
  if (!esIdentificadorValido(id)) {
    return { ok: false, message: "El usuario solicitado no es válido." };
  }

  const roles = leerRoles(formData);
  const validationError = getUsuarioRolesValidationError(roles);
  if (validationError) {
    return { ok: false, message: validationError };
  }

  if (id === session.usuarioId) {
    const errorRolesPropios = await getErrorRolAdministradorPropio(session.token, roles);
    if (errorRolesPropios) {
      return { ok: false, message: errorRolesPropios };
    }
  }

  try {
    await updateUsuarioRolesInBackend(session.token, id, roles);
  } catch (error) {
    return manejarErrorAccion(error);
  }

  revalidatePath(USUARIOS_PATH);
  return { ok: true, message: "Los roles se actualizaron correctamente." };
}

/**
 * Activa o desactiva un usuario. El administrador no puede desactivar su propio usuario.
 * @param id Identificador del usuario.
 * @param activo Nuevo estado activo.
 * @returns Resultado para la interfaz.
 */
export async function cambiarActivoUsuarioAction(
  id: string,
  activo: boolean,
): Promise<UsuarioActionState> {
  const session = await getAdminSession();
  if (!esIdentificadorValido(id)) {
    return { ok: false, message: "El usuario solicitado no es válido." };
  }
  if (id === session.usuarioId && !activo) {
    return { ok: false, message: "No puedes desactivar tu propio usuario." };
  }

  try {
    await changeUsuarioActivoInBackend(session.token, id, activo);
  } catch (error) {
    return manejarErrorAccion(error);
  }

  revalidatePath(USUARIOS_PATH);
  return {
    ok: true,
    message: activo
      ? "El usuario se activó correctamente."
      : "El usuario se desactivó correctamente.",
  };
}

/**
 * Consulta un usuario por su identificador para mostrarlo en el detalle.
 * @param id Identificador del usuario.
 * @returns El usuario o un mensaje de error.
 */
export async function obtenerUsuarioAction(id: string): Promise<UsuarioDetalleResult> {
  const session = await getAdminSession();
  if (!esIdentificadorValido(id)) {
    return { ok: false, message: "El usuario solicitado no es válido." };
  }

  try {
    return { ok: true, usuario: await getUsuarioInBackend(session.token, id) };
  } catch (error) {
    if (error instanceof BackendError) {
      if (error.status === 401) {
        redirect("/login");
      }
      return { ok: false, message: error.message };
    }

    console.error("Error al consultar el usuario:", error);
    return { ok: false, message: DEFAULT_ERROR_MESSAGE };
  }
}

/** Exige una sesión con el rol de administrador del sistema. */
async function getAdminSession() {
  const session = await getSession();
  if (!session) {
    redirect("/login");
  }
  if (!session.roles.includes(ROLE_ADMINISTRADOR_DEL_SISTEMA)) {
    redirect("/dashboard");
  }
  return session;
}

/**
 * Comprueba que el administrador actual conserve su rol al editarse a sí mismo.
 * @param accessToken Token de la sesión.
 * @param rolesSeleccionados Identificadores de roles enviados.
 * @returns Mensaje de error, o null si conserva el rol o no es necesario comprobarlo.
 */
async function getErrorRolAdministradorPropio(
  accessToken: string,
  rolesSeleccionados: string[],
): Promise<string | null> {
  try {
    const roles = await listRolesInBackend(accessToken);
    const rolAdministrador = roles.find(
      (rol) => rol.nombre === ROLE_ADMINISTRADOR_DEL_SISTEMA,
    );

    if (rolAdministrador && !rolesSeleccionados.includes(rolAdministrador.id)) {
      return "No puedes quitarte el rol de Administrador del sistema.";
    }
    return null;
  } catch (error) {
    if (error instanceof BackendError && error.status === 401) {
      redirect("/login");
    }
    console.error("Error al verificar el rol de administrador:", error);
    return "No se pudo verificar tu rol de administrador. Intenta más tarde.";
  }
}

/** Convierte un fallo del backend en un resultado para la interfaz. */
function manejarErrorAccion(error: unknown): UsuarioActionState {
  if (error instanceof BackendError) {
    if (error.status === 401) {
      redirect("/login");
    }
    return { ok: false, message: error.message };
  }

  console.error("Error al administrar usuarios:", error);
  return { ok: false, message: DEFAULT_ERROR_MESSAGE };
}

/** Lee del formulario los campos personales y el correo. */
function leerDatosPersonales(formData: FormData) {
  return {
    primerNombre: leerTexto(formData, "primerNombre"),
    segundoNombre: leerTextoOpcional(formData, "segundoNombre"),
    primerApellido: leerTexto(formData, "primerApellido"),
    segundoApellido: leerTextoOpcional(formData, "segundoApellido"),
    correoElectronico: leerTexto(formData, "correoElectronico"),
  };
}

/** Lee los roles marcados en el formulario, sin repeticiones. */
function leerRoles(formData: FormData): string[] {
  const roles = formData
    .getAll("roles")
    .filter((valor): valor is string => typeof valor === "string" && valor.length > 0);
  return [...new Set(roles)];
}

/** Lee un campo de texto del formulario; con `recortar` quita los espacios de los extremos. */
function leerTexto(formData: FormData, nombre: string, recortar = true): string {
  const valor = String(formData.get(nombre) ?? "");
  return recortar ? valor.trim() : valor;
}

/** Lee un campo opcional: devuelve null si está vacío. */
function leerTextoOpcional(formData: FormData, nombre: string): string | null {
  return leerTexto(formData, nombre) || null;
}
