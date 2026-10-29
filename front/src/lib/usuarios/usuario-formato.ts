import type { Usuario } from "@/lib/backend/types";

type UsuarioNombre = Pick<
  Usuario,
  "primerNombre" | "segundoNombre" | "primerApellido" | "segundoApellido"
>;

const DATE_FORMATTER = new Intl.DateTimeFormat("es-CO", {
  dateStyle: "medium",
  // Fijar la zona evita diferencias entre el render del servidor y el del navegador.
  timeZone: "America/Bogota",
});

/** Une nombres y apellidos omitiendo los campos opcionales vacíos. */
export function buildNombreCompleto(usuario: UsuarioNombre): string {
  return [
    usuario.primerNombre,
    usuario.segundoNombre,
    usuario.primerApellido,
    usuario.segundoApellido,
  ]
    .filter((parte): parte is string => Boolean(parte))
    .join(" ");
}

/** Iniciales del primer nombre y el primer apellido, para el avatar. */
export function getIniciales(usuario: UsuarioNombre): string {
  return `${usuario.primerNombre.charAt(0)}${usuario.primerApellido.charAt(0)}`.toUpperCase();
}

/** Convierte la fecha UTC del backend a una fecha legible en Colombia. */
export function formatFechaCreacion(fechaUtc: string): string {
  return DATE_FORMATTER.format(new Date(fechaUtc));
}
