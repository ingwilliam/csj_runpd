import type { EstadoFiltroDespachos } from "@/lib/backend/types";

/** Cantidad de despachos que se muestran por página. */
export const TAMANO_PAGINA = 20;

const MAX_BUSQUEDA = 100;
const MAX_PAGINA = 1_000_000;
const DESPACHOS_PATH = "/dashboard/despachos";

type SearchParams = Record<string, string | string[] | undefined>;

/** Filtros normalizados de la pantalla de despachos. */
export type FiltrosDespachos = {
  pagina: number;
  busqueda: string;
  estado: EstadoFiltroDespachos;
};

/** Toma el primer valor cuando un parámetro de URL se repite. */
function getPrimerValor(valor: string | string[] | undefined): string | undefined {
  return Array.isArray(valor) ? valor[0] : valor;
}

/**
 * Normaliza los parámetros de URL de la pantalla de despachos.
 * @param params Parámetros recibidos por la página.
 * @returns Página válida, búsqueda recortada y estado permitido.
 */
export function parseFiltrosDespachos(params: SearchParams): FiltrosDespachos {
  const pagina = Number.parseInt(getPrimerValor(params.pagina) ?? "", 10);
  const busqueda = getPrimerValor(params.busqueda)?.trim() ?? "";
  const estado = getPrimerValor(params.estado)?.toLowerCase();

  return {
    pagina: Number.isFinite(pagina) && pagina >= 1 ? Math.min(pagina, MAX_PAGINA) : 1,
    busqueda: busqueda.slice(0, MAX_BUSQUEDA),
    estado: estado === "permanentes" || estado === "descongestion" || estado === "suprimidos" ? estado : "todos",
  };
}

/**
 * Construye la URL de la pantalla de despachos a partir de los filtros.
 * Omite los valores por defecto para mantener direcciones limpias.
 * @param filtros Página, búsqueda y estado que deben conservarse.
 * @returns Ruta interna con los parámetros necesarios.
 */
export function buildDespachosHref(filtros: FiltrosDespachos): string {
  const params = new URLSearchParams();

  if (filtros.pagina > 1) {
    params.set("pagina", String(filtros.pagina));
  }

  const busqueda = filtros.busqueda.trim();
  if (busqueda) {
    params.set("busqueda", busqueda);
  }

  if (filtros.estado !== "todos") {
    params.set("estado", filtros.estado);
  }

  const query = params.toString();
  return query ? `${DESPACHOS_PATH}?${query}` : DESPACHOS_PATH;
}
