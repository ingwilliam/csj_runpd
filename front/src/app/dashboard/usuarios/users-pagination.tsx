import type { ReactNode } from "react";
import Link from "next/link";
import { ChevronLeft, ChevronRight } from "lucide-react";
import type { EstadoFiltroUsuarios } from "@/lib/backend/types";
import { buildUsuariosHref } from "./users-query";

type UsersPaginationProps = {
  pagina: number;
  tamano: number;
  total: number;
  totalPaginas: number;
  busqueda: string;
  estado: EstadoFiltroUsuarios;
};

type PaginaVisible = number | "separador";

/**
 * Resumen y controles de paginación del listado de usuarios.
 * Conserva la búsqueda y el filtro al cambiar de página.
 */
export function UsersPagination({
  pagina,
  tamano,
  total,
  totalPaginas,
  busqueda,
  estado,
}: UsersPaginationProps) {
  if (total === 0) {
    return null;
  }

  const desde = (pagina - 1) * tamano + 1;
  const hasta = Math.min(pagina * tamano, total);
  const paginas = getPaginasVisibles(pagina, totalPaginas);

  return (
    <div className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white px-4 py-3 sm:flex-row sm:items-center sm:justify-between">
      <p className="text-sm text-slate-500">
        Mostrando{" "}
        <span className="font-medium text-slate-700">
          {desde}–{hasta}
        </span>{" "}
        de <span className="font-medium text-slate-700">{total}</span>
      </p>

      {totalPaginas > 1 && (
        <nav
          aria-label="Paginación de usuarios"
          className="flex flex-wrap items-center justify-center gap-1"
        >
          <PageLink
            href={buildUsuariosHref({ pagina: pagina - 1, busqueda, estado })}
            etiqueta="Página anterior"
            deshabilitado={pagina <= 1}
          >
            <ChevronLeft aria-hidden="true" className="h-4 w-4" />
          </PageLink>

          {paginas.map((item, index) =>
            item === "separador" ? (
              <span
                key={`separador-${index}`}
                aria-hidden="true"
                className="px-1 text-sm text-slate-400"
              >
                …
              </span>
            ) : (
              <PageLink
                key={item}
                href={buildUsuariosHref({ pagina: item, busqueda, estado })}
                etiqueta={`Página ${item}`}
                activo={item === pagina}
              >
                {item}
              </PageLink>
            ),
          )}

          <PageLink
            href={buildUsuariosHref({ pagina: pagina + 1, busqueda, estado })}
            etiqueta="Página siguiente"
            deshabilitado={pagina >= totalPaginas}
          >
            <ChevronRight aria-hidden="true" className="h-4 w-4" />
          </PageLink>
        </nav>
      )}
    </div>
  );
}

/** Enlace o indicador deshabilitado de una página. */
function PageLink({
  href,
  etiqueta,
  activo = false,
  deshabilitado = false,
  children,
}: {
  href: string;
  etiqueta: string;
  activo?: boolean;
  deshabilitado?: boolean;
  children: ReactNode;
}) {
  const base =
    "inline-flex h-9 min-w-9 items-center justify-center rounded-lg border px-2 text-sm font-medium transition-colors";

  if (deshabilitado) {
    return (
      <span
        role="link"
        aria-disabled="true"
        aria-label={etiqueta}
        className={`${base} border-slate-100 text-slate-300`}
      >
        {children}
      </span>
    );
  }

  return (
    <Link
      href={href}
      aria-label={etiqueta}
      aria-current={activo ? "page" : undefined}
      className={
        activo
          ? `${base} border-corporativo-azul bg-corporativo-azul text-white`
          : `${base} border-slate-200 bg-white text-slate-600 hover:border-corporativo-azul/30 hover:text-corporativo-azul`
      }
    >
      {children}
    </Link>
  );
}

/** Genera una lista compacta de páginas con separadores cuando hay muchas. */
function getPaginasVisibles(pagina: number, totalPaginas: number): PaginaVisible[] {
  if (totalPaginas <= 7) {
    return Array.from({ length: totalPaginas }, (_, indice) => indice + 1);
  }

  const candidatas = new Set<number>([
    1,
    totalPaginas,
    pagina - 1,
    pagina,
    pagina + 1,
  ]);
  const paginas = [...candidatas]
    .filter((valor) => valor >= 1 && valor <= totalPaginas)
    .sort((a, b) => a - b);

  const resultado: PaginaVisible[] = [];
  for (const valor of paginas) {
    const anterior = resultado[resultado.length - 1];
    if (typeof anterior === "number" && valor - anterior > 1) {
      resultado.push("separador");
    }
    resultado.push(valor);
  }

  return resultado;
}
