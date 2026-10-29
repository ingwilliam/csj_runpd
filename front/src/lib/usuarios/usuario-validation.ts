import type { ActualizarUsuarioRequest, CrearUsuarioRequest } from "@/lib/backend/types";

const MAX_NAME_LENGTH = 100;
const MAX_EMAIL_LENGTH = 254;
const MIN_PASSWORD_LENGTH = 8;
const MAX_PASSWORD_LENGTH = 100;
const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const GUID_PATTERN = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

/**
 * Valida los datos de un usuario nuevo con las mismas reglas del backend.
 * @param datos Datos personales, correo, contraseña y roles opcionales.
 * @returns Mensaje de error en español, o null si los datos son válidos.
 */
export function getCrearUsuarioValidationError(datos: CrearUsuarioRequest): string | null {
  const datosError = getUsuarioDatosValidationError(datos);
  if (datosError) {
    return datosError;
  }

  if (!datos.contrasena) {
    return "La contraseña es obligatoria.";
  }
  if (datos.contrasena.length < MIN_PASSWORD_LENGTH) {
    return `La contraseña debe tener al menos ${MIN_PASSWORD_LENGTH} caracteres.`;
  }
  if (datos.contrasena.length > MAX_PASSWORD_LENGTH) {
    return `La contraseña no puede superar ${MAX_PASSWORD_LENGTH} caracteres.`;
  }

  return getUsuarioRolesValidationError(datos.roles ?? []);
}

/**
 * Valida los datos editables de un usuario. La contraseña no se modifica desde esta pantalla.
 * @param datos Datos personales, correo y roles opcionales.
 * @returns Mensaje de error en español, o null si los datos son válidos.
 */
export function getActualizarUsuarioValidationError(
  datos: ActualizarUsuarioRequest,
): string | null {
  const datosError = getUsuarioDatosValidationError(datos);
  return datosError ?? getUsuarioRolesValidationError(datos.roles ?? []);
}

/**
 * Valida nombres, apellidos y correo electrónico.
 * @param datos Datos personales a validar.
 * @returns Mensaje de error en español, o null si son válidos.
 */
function getUsuarioDatosValidationError(
  datos: Pick<
    ActualizarUsuarioRequest,
    "primerNombre" | "segundoNombre" | "primerApellido" | "segundoApellido" | "correoElectronico"
  >,
): string | null {
  const nombres = [
    { valor: datos.primerNombre, etiqueta: "primer nombre", obligatorio: true },
    { valor: datos.segundoNombre ?? "", etiqueta: "segundo nombre", obligatorio: false },
    { valor: datos.primerApellido, etiqueta: "primer apellido", obligatorio: true },
    { valor: datos.segundoApellido ?? "", etiqueta: "segundo apellido", obligatorio: false },
  ];

  for (const nombre of nombres) {
    if (nombre.obligatorio && !nombre.valor.trim()) {
      return `El ${nombre.etiqueta} es obligatorio.`;
    }
    if (nombre.valor.length > MAX_NAME_LENGTH) {
      return `El ${nombre.etiqueta} no puede superar ${MAX_NAME_LENGTH} caracteres.`;
    }
  }

  const correo = datos.correoElectronico.trim();
  if (!correo) {
    return "El correo electrónico es obligatorio.";
  }
  if (correo.length > MAX_EMAIL_LENGTH) {
    return `El correo electrónico no puede superar ${MAX_EMAIL_LENGTH} caracteres.`;
  }
  if (!EMAIL_PATTERN.test(correo)) {
    return "El correo electrónico no tiene un formato válido.";
  }

  return null;
}

/**
 * Valida que los identificadores de roles tengan forma de GUID.
 * @param roles Identificadores seleccionados.
 * @returns Mensaje de error en español, o null si son válidos.
 */
export function getUsuarioRolesValidationError(roles: string[]): string | null {
  for (const rol of roles) {
    if (!GUID_PATTERN.test(rol)) {
      return "Uno de los roles seleccionados no es válido.";
    }
  }
  return null;
}

/**
 * Indica si un texto tiene forma de GUID, como los identificadores que expone el backend.
 * @param valor Texto a comprobar.
 * @returns true si el formato es válido.
 */
export function esIdentificadorValido(valor: string): boolean {
  return GUID_PATTERN.test(valor);
}
