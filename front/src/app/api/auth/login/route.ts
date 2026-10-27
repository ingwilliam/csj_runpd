import { NextResponse } from "next/server";
import { BackendError, loginInBackend } from "@/lib/backend/client";
import { getLoginValidationError } from "@/lib/auth/login-validation";
import { SESSION_COOKIE_NAME } from "@/lib/auth/session";
import type { LoginRequest } from "@/lib/backend/types";

/**
 * POST /api/auth/login
 * Valida las credenciales, las envía al backend y guarda el token en una cookie httpOnly.
 * Responde 200 con `{ ok: true }`, o con `{ message }` y un código 400, 401, 502 o 503.
 */
export async function POST(request: Request) {
  // Paso 1: leer el cuerpo; un JSON inválido se trata como solicitud incorrecta.
  const body: unknown = await request.json().catch(() => null);
  if (!isLoginRequest(body)) {
    return jsonError(400, "La solicitud no es válida.");
  }

  // Paso 2: validar los datos antes de llamar al backend.
  const credentials: LoginRequest = {
    correoElectronico: body.correoElectronico.trim(),
    contrasena: body.contrasena,
  };
  const validationError = getLoginValidationError(credentials);
  if (validationError) {
    return jsonError(400, validationError);
  }

  // Paso 3: autenticar y crear la sesión en una cookie que el navegador no puede leer.
  try {
    const session = await loginInBackend(credentials);
    const response = NextResponse.json({ ok: true });
    response.cookies.set({
      name: SESSION_COOKIE_NAME,
      value: session.accessToken,
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/",
      expires: new Date(session.expiraUtc),
    });
    return response;
  } catch (error) {
    return handleLoginError(error);
  }
}

/** Indica si el cuerpo recibido tiene la forma de LoginRequest. */
function isLoginRequest(value: unknown): value is LoginRequest {
  if (typeof value !== "object" || value === null) {
    return false;
  }
  const candidate = value as Record<string, unknown>;
  return (
    typeof candidate.correoElectronico === "string" &&
    typeof candidate.contrasena === "string"
  );
}

/**
 * Convierte un fallo al autenticar en la respuesta HTTP adecuada.
 * Los errores de credenciales se reenvían tal cual; cualquier otro fallo se registra y se oculta al usuario.
 */
function handleLoginError(error: unknown) {
  if (error instanceof BackendError) {
    const status = error.status === 400 || error.status === 401 ? error.status : 502;
    return jsonError(status, error.message);
  }

  // Errores de red o configuración: se registran en el servidor sin exponer detalles.
  console.error("Error al conectar con el servicio de autenticación:", error);
  return jsonError(503, "No se pudo conectar con el servidor. Intenta más tarde.");
}

/** Construye una respuesta JSON de error con el formato `{ message }`. */
function jsonError(status: number, message: string) {
  return NextResponse.json({ message }, { status });
}
