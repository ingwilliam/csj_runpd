import type { EstadoDespacho } from "@/lib/backend/types";

/** Estados funcionales permitidos por el backend, con su etiqueta para la interfaz. */
export const ESTADOS_DESPACHO: { value: EstadoDespacho; label: string }[] = [
  { value: "CREADO", label: "Creado" },
  { value: "MODIFICADO", label: "Modificado" },
  { value: "TRASLADADO", label: "Trasladado" },
  { value: "SUPRIMIDO", label: "Suprimido" },
];

const DATE_FORMATTER = new Intl.DateTimeFormat("es-CO", {
  dateStyle: "medium",
  // Fijar la zona evita diferencias entre el render del servidor y el del navegador.
  timeZone: "America/Bogota",
});

/** Traduce el estado del despacho a una etiqueta legible. */
export function formatEstadoDespacho(estado: string): string {
  return ESTADOS_DESPACHO.find((item) => item.value === estado)?.label ?? estado;
}

/** Convierte la fecha UTC del backend a una fecha legible en Colombia. */
export function formatFechaCreacion(fechaUtc: string): string {
  return DATE_FORMATTER.format(new Date(fechaUtc));
}
