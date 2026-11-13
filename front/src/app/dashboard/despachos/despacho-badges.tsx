import { formatEstadoDespacho } from "@/lib/despachos/despacho-formato";

/** Colores del indicador según el estado funcional del despacho. */
const ESTADO_CLASSES: Record<string, string> = {
  CREADO: "bg-corporativo-verde/15 text-corporativo-verde",
  MODIFICADO: "bg-corporativo-amarillo/40 text-corporativo-azul",
  TRASLADADO: "bg-corporativo-azul/10 text-corporativo-azul",
  SUPRIMIDO: "bg-red-50 text-red-600",
};

/** Indicador del estado funcional del despacho. */
export function DespachoEstadoBadge({ estado }: { estado: string }) {
  return (
    <span
      className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${
        ESTADO_CLASSES[estado] ?? "bg-slate-100 text-slate-600"
      }`}
    >
      {formatEstadoDespacho(estado)}
    </span>
  );
}

/** Indicador de estado del despacho: Registrado o Suprimido. */
export function DespachoStatusBadge({ activo }: { activo: boolean }) {
  return activo ? (
    <span className="rounded-full bg-corporativo-verde/15 px-2.5 py-0.5 text-xs font-medium text-corporativo-verde">
      Registrado
    </span>
  ) : (
    <span className="rounded-full bg-slate-200 px-2.5 py-0.5 text-xs font-medium text-slate-600">
      Suprimido
    </span>
  );
}

/** Etiqueta que destaca los despachos marcados como promiscuos. */
export function DespachoPromiscuoBadge({ promiscuo }: { promiscuo: boolean }) {
  if (!promiscuo) {
    return null;
  }

  return (
    <span className="rounded-full border border-corporativo-amarillo/60 bg-white px-2.5 py-0.5 text-xs font-medium text-corporativo-azul">
      Promiscuo
    </span>
  );
}
