"use server";

import { revalidatePath } from "next/cache";
import { redirect } from "next/navigation";
import {
  actualizarPerfilInBackend,
  BackendError,
  cambiarContrasenaInBackend,
} from "@/lib/backend/client";
import { getSession } from "@/lib/auth/session-claims";
import {
  getActualizarPerfilValidationError,
  getCambiarContrasenaValidationError,
} from "@/lib/perfil/perfil-validation";

/** Resultado de una acción del perfil, que la interfaz muestra como notificación. */
export type PerfilActionState = {
  ok: boolean;
  message: string | null;
};

const DEFAULT_ERROR_MESSAGE = "No se pudo completar la operación. Intenta más tarde.";

/**
 * Actualiza nombres y apellidos del usuario de la sesión.
 * Sin sesión redirige al login; el correo y los roles no se modifican.
 */
export async function actualizarPerfilAction(
  _estadoPrevio: PerfilActionState,
  formData: FormData,
): Promise<PerfilActionState> {
  const session = await getSession();
  if (!session) {
    redirect("/login");
  }

  // Paso 1: leer y validar los datos antes de llamar al backend.
  const datos = {
    primerNombre: leerTexto(formData, "primerNombre"),
    segundoNombre: leerTextoOpcional(formData, "segundoNombre"),
    primerApellido: leerTexto(formData, "primerApellido"),
    segundoApellido: leerTextoOpcional(formData, "segundoApellido"),
  };
  const validationError = getActualizarPerfilValidationError(datos);
  if (validationError) {
    return { ok: false, message: validationError };
  }

  // Paso 2: guardar en el backend y refrescar los datos mostrados.
  try {
    await actualizarPerfilInBackend(session.token, datos);
  } catch (error) {
    return manejarErrorAccion(error);
  }

  revalidatePath("/dashboard/perfil");
  return { ok: true, message: "Tus datos se actualizaron correctamente." };
}

/**
 * Cambia la contraseña del usuario de la sesión tras verificar la actual.
 * La sesión actual sigue activa hasta que venza su token.
 */
export async function cambiarContrasenaAction(
  _estadoPrevio: PerfilActionState,
  formData: FormData,
): Promise<PerfilActionState> {
  const session = await getSession();
  if (!session) {
    redirect("/login");
  }

  // Paso 1: validar los datos antes de llamar al backend.
  const datos = {
    contrasenaActual: leerTexto(formData, "contrasenaActual", false),
    contrasenaNueva: leerTexto(formData, "contrasenaNueva", false),
    confirmacion: leerTexto(formData, "confirmacion", false),
  };
  const validationError = getCambiarContrasenaValidationError(datos);
  if (validationError) {
    return { ok: false, message: validationError };
  }

  // Paso 2: enviar el cambio al backend.
  try {
    await cambiarContrasenaInBackend(session.token, {
      contrasenaActual: datos.contrasenaActual,
      contrasenaNueva: datos.contrasenaNueva,
    });
  } catch (error) {
    return manejarErrorAccion(error);
  }

  return { ok: true, message: "Tu contraseña se cambió correctamente." };
}

/**
 * Convierte un fallo del backend en un resultado para la interfaz.
 * Un 401 indica que el token venció: se envía al login.
 */
function manejarErrorAccion(error: unknown): PerfilActionState {
  if (error instanceof BackendError) {
    if (error.status === 401) {
      redirect("/login");
    }
    return { ok: false, message: error.message };
  }

  console.error("Error al actualizar el perfil:", error);
  return { ok: false, message: DEFAULT_ERROR_MESSAGE };
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
