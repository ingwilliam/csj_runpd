import type { LoginRequest } from "@/lib/backend/types";

const MAX_EMAIL_LENGTH = 254;
const MAX_PASSWORD_LENGTH = 100;
// Validación de formato básica; la validación definitiva la hace el backend.
const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

/**
 * Valida las credenciales de login con las mismas reglas de longitud del backend.
 * Se usa en el formulario (respuesta inmediata) y en el Route Handler (defensa en servidor).
 * @param credentials Correo y contraseña a validar.
 * @returns Mensaje de error en español, o null si los datos son válidos.
 */
export function getLoginValidationError(credentials: LoginRequest): string | null {
  const email = credentials.correoElectronico.trim();

  if (!email) {
    return "Ingresa tu correo electrónico.";
  }
  if (email.length > MAX_EMAIL_LENGTH || !EMAIL_PATTERN.test(email)) {
    return "Ingresa un correo electrónico válido.";
  }
  if (!credentials.contrasena) {
    return "Ingresa tu contraseña.";
  }
  if (credentials.contrasena.length > MAX_PASSWORD_LENGTH) {
    return `La contraseña no puede superar ${MAX_PASSWORD_LENGTH} caracteres.`;
  }
  return null;
}
