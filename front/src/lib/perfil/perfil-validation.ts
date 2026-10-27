import type { ActualizarPerfilRequest } from "@/lib/backend/types";

const MAX_NAME_LENGTH = 100;
const MIN_PASSWORD_LENGTH = 8;
const MAX_PASSWORD_LENGTH = 100;

/** Datos del formulario de cambio de contraseña, antes de enviarse. */
export type CambiarContrasenaInput = {
  contrasenaActual: string;
  contrasenaNueva: string;
  confirmacion: string;
};

/**
 * Valida los datos personales con las mismas reglas de longitud del backend.
 * Se usa en el formulario (respuesta inmediata) y en la server action (defensa en servidor).
 * @param datos Nombres y apellidos a validar.
 * @returns Mensaje de error en español, o null si los datos son válidos.
 */
export function getActualizarPerfilValidationError(datos: ActualizarPerfilRequest): string | null {
  const campos = [
    { valor: datos.primerNombre, etiqueta: "primer nombre", obligatorio: true },
    { valor: datos.segundoNombre ?? "", etiqueta: "segundo nombre", obligatorio: false },
    { valor: datos.primerApellido, etiqueta: "primer apellido", obligatorio: true },
    { valor: datos.segundoApellido ?? "", etiqueta: "segundo apellido", obligatorio: false },
  ];

  for (const campo of campos) {
    if (campo.obligatorio && !campo.valor) {
      return `Ingresa tu ${campo.etiqueta}.`;
    }
    if (campo.valor.length > MAX_NAME_LENGTH) {
      return `El ${campo.etiqueta} no puede superar ${MAX_NAME_LENGTH} caracteres.`;
    }
  }
  return null;
}

/**
 * Valida el cambio de contraseña: contraseña actual presente, nueva con longitud válida y confirmación igual.
 * @param datos Contraseña actual, nueva y su confirmación.
 * @returns Mensaje de error en español, o null si los datos son válidos.
 */
export function getCambiarContrasenaValidationError(datos: CambiarContrasenaInput): string | null {
  if (!datos.contrasenaActual) {
    return "Ingresa tu contraseña actual.";
  }
  if (datos.contrasenaNueva.length < MIN_PASSWORD_LENGTH) {
    return `La nueva contraseña debe tener al menos ${MIN_PASSWORD_LENGTH} caracteres.`;
  }
  if (datos.contrasenaNueva.length > MAX_PASSWORD_LENGTH) {
    return `La nueva contraseña no puede superar ${MAX_PASSWORD_LENGTH} caracteres.`;
  }
  if (datos.contrasenaNueva !== datos.confirmacion) {
    return "La confirmación no coincide con la nueva contraseña.";
  }
  return null;
}
