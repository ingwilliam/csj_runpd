import { redirect } from "next/navigation";
import { connection } from "next/server";
import { RetryButton } from "@/components/retry-button";
import {
  BackendError,
  listCircuitosInBackend,
  listDepartamentosInBackend,
  listDespachosInBackend,
  listDistritosInBackend,
  listEntidadesInBackend,
  listEspecialidadesInBackend,
  listJurisdiccionesInBackend,
  listSubEspecialidadesInBackend,
} from "@/lib/backend/client";
import type { CatalogosDespachos, DespachosPaginados } from "@/lib/backend/types";
import {
  ROLE_ADMINISTRADOR_DEL_SISTEMA,
  ROLE_ADMINISTRADOR_UDAE,
  ROLE_CONSULTA,
} from "@/lib/auth/modules";
import { getSession } from "@/lib/auth/session-claims";
import { DespachosManager } from "./despachos-manager";
import { parseFiltrosDespachos, TAMANO_PAGINA } from "./despachos-query";

type DespachosPanelProps = {
  searchParams: PageProps<"/dashboard/despachos">["searchParams"];
};

const ROLES_LECTURA = [
  ROLE_ADMINISTRADOR_UDAE,
  ROLE_CONSULTA,
  ROLE_ADMINISTRADOR_DEL_SISTEMA,
];

/**
 * Carga la página de despachos y los catálogos maestros desde el backend.
 * Sin un rol de lectura envía de vuelta al panel; sin sesión, al login.
 * Debe renderizarse dentro de un `<Suspense>` porque lee la cookie y los parámetros de URL.
 */
export async function DespachosPanel({ searchParams }: DespachosPanelProps) {
  // Los despachos y la sesión son por petición: se indica a Next que no se prerenderice este bloque.
  await connection();

  const filtros = parseFiltrosDespachos(await searchParams);
  const session = await getSession();
  if (!session) {
    redirect("/login");
  }
  if (!ROLES_LECTURA.some((rol) => session.roles.includes(rol))) {
    redirect("/dashboard");
  }

  // Paso 1: consultar los despachos y los catálogos en paralelo.
  let resultado: DespachosPaginados;
  let catalogos: CatalogosDespachos;
  try {
    const [
      paginaDespachos,
      departamentos,
      jurisdicciones,
      distritos,
      circuitos,
      especialidades,
      subEspecialidades,
      entidades,
    ] = await Promise.all([
      listDespachosInBackend(session.token, {
        pagina: filtros.pagina,
        tamano: TAMANO_PAGINA,
        busqueda: filtros.busqueda,
        estado: filtros.estado,
      }),
      listDepartamentosInBackend(session.token),
      listJurisdiccionesInBackend(session.token),
      listDistritosInBackend(session.token),
      listCircuitosInBackend(session.token),
      listEspecialidadesInBackend(session.token),
      listSubEspecialidadesInBackend(session.token),
      listEntidadesInBackend(session.token),
    ]);

    resultado = paginaDespachos;
    catalogos = {
      departamentos,
      jurisdicciones,
      distritos,
      circuitos,
      especialidades,
      subEspecialidades,
      entidades,
    };
  } catch (error) {
    // Un 401 significa que el token venció: se vuelve a pedir inicio de sesión.
    if (error instanceof BackendError && error.status === 401) {
      redirect("/login");
    }
    console.error("Error al consultar los despachos:", error);
    const message =
      error instanceof BackendError
        ? error.message
        : "No se pudieron cargar los despachos. Intenta más tarde.";
    return (
      <div
        role="alert"
        className="rounded-2xl border border-red-200 bg-red-50 p-6 text-red-700"
      >
        <p>{message}</p>
        <div className="mt-4">
          <RetryButton className="bg-red-600 text-white hover:bg-red-700" />
        </div>
      </div>
    );
  }

  // Paso 2: mostrar la administración; solo la UDAE puede modificar despachos.
  return (
    <DespachosManager
      resultado={resultado}
      catalogos={catalogos}
      puedeEditar={session.roles.includes(ROLE_ADMINISTRADOR_UDAE)}
      busqueda={filtros.busqueda}
      estado={filtros.estado}
    />
  );
}
