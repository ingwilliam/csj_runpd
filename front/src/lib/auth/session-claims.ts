import { cookies } from "next/headers";
import { SESSION_COOKIE_NAME } from "@/lib/auth/session";

/** Datos de la sesión actual: el token para llamar al backend y los claims para la interfaz. */
export type SessionData = {
  token: string;
  correoElectronico: string;
  roles: string[];
};

/**
 * Lee la sesión actual desde la cookie httpOnly.
 * Solo debe llamarse en el servidor (Server Components o Route Handlers).
 * @returns Datos de la sesión o `null` si no hay cookie o el token no se puede leer.
 */
export async function getSession(): Promise<SessionData | null> {
  const cookieStore = await cookies();
  const token = cookieStore.get(SESSION_COOKIE_NAME)?.value;
  if (!token) {
    return null;
  }

  const claims = decodeSessionClaims(token);
  return claims ? { token, ...claims } : null;
}

/**
 * Decodifica el payload de un JWT sin verificar su firma.
 * Solo sirve para decidir qué mostrar: el backend valida el token en cada petición.
 * @param token Token JWT completo (`encabezado.payload.firma`).
 * @returns Claims de correo y roles, o `null` si el payload no es válido.
 */
function decodeSessionClaims(token: string): Pick<SessionData, "correoElectronico" | "roles"> | null {
  const payloadSegment = token.split(".")[1];
  if (!payloadSegment) {
    return null;
  }

  // Un token malformado no es un fallo del sistema: se trata como sesión sin datos.
  let payload: unknown;
  try {
    payload = JSON.parse(Buffer.from(payloadSegment, "base64url").toString("utf8"));
  } catch {
    return null;
  }

  if (typeof payload !== "object" || payload === null) {
    return null;
  }
  const claims = payload as Record<string, unknown>;
  return {
    correoElectronico: typeof claims.email === "string" ? claims.email : "",
    roles: toStringArray(claims.role),
  };
}

/** El backend emite un claim `role` como texto si hay un rol, o como lista si hay varios. */
function toStringArray(value: unknown): string[] {
  if (typeof value === "string") {
    return [value];
  }
  if (Array.isArray(value)) {
    return value.filter((item): item is string => typeof item === "string");
  }
  return [];
}
