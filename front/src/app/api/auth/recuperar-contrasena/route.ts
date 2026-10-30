import { NextResponse } from "next/server";
import { BackendError, recuperarContrasenaInBackend } from "@/lib/backend/client";
import { getCorreoValidationError } from "@/lib/auth/login-validation";
import type { RecuperarContrasenaRequest } from "@/lib/backend/types";

/**
 * POST /api/auth/recuperar-contrasena
 * Valida el correo y pide al backend enviar una contraseña temporal.
 * Responde 200 con `{ message }` (mismo texto exista o no el correo), o con `{ message }` y un código 400, 502 o 503.
 */
export async function POST(request: Request) {
  // Paso 1: leer el cuerpo; un JSON inválido se trata como solicitud incorrecta.
  const body: unknown = await request.json().catch(() => null);
  if (!isRecuperarContrasenaRequest(body)) {
    return jsonError(400, "La solicitud no es válida.");
  }

  // Paso 2: validar el correo antes de llamar al backend.
  const datos: RecuperarContrasenaRequest = {
    correoElectronico: body.correoElectronico.trim(),
  };
  const validationError = getCorreoValidationError(datos.correoElectronico);
  if (validationError) {
    return jsonError(400, validationError);
  }

  // Paso 3: solicitar la contraseña temporal al backend.
  try {
    const message = await recuperarContrasenaInBackend(datos);
    return NextResponse.json({ message });
  } catch (error) {
    return handleRecuperacionError(error);
  }
}

/** Indica si el cuerpo recibido tiene la forma de RecuperarContrasenaRequest. */
function isRecuperarContrasenaRequest(value: unknown): value is RecuperarContrasenaRequest {
  if (typeof value !== "object" || value === null) {
    return false;
  }
  const candidate = value as Record<string, unknown>;
  return typeof candidate.correoElectronico === "string";
}

/**
 * Convierte un fallo del backend en la respuesta HTTP adecuada.
 * Los errores de validación se reenvían; cualquier otro fallo se registra y se oculta al usuario.
 */
function handleRecuperacionError(error: unknown) {
  if (error instanceof BackendError && error.status === 400) {
    return jsonError(400, error.message);
  }

  // Errores de red, configuración o del servidor: se registran sin exponer detalles.
  console.error("Error al solicitar la recuperación de contraseña:", error);
  return jsonError(503, "No se pudo conectar con el servidor. Intenta más tarde.");
}

/** Construye una respuesta JSON de error con el formato `{ message }`. */
function jsonError(status: number, message: string) {
  return NextResponse.json({ message }, { status });
}
