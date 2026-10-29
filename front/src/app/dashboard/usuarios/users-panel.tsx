import { redirect } from "next/navigation";
import { connection } from "next/server";
import { RetryButton } from "@/components/retry-button";
import { BackendError, listRolesInBackend, listUsuariosInBackend } from "@/lib/backend/client";
import type { Rol, UsuariosPaginados } from "@/lib/backend/types";
import { ROLE_ADMINISTRADOR_DEL_SISTEMA } from "@/lib/auth/modules";
import { getSession } from "@/lib/auth/session-claims";
import { UsersManager } from "./users-manager";
import { parseFiltrosUsuarios, TAMANO_PAGINA } from "./users-query";

type UsersPanelProps = {
  searchParams: PageProps<"/dashboard/usuarios">["searchParams"];
};

/**
 * Carga la página de usuarios y el catálogo de roles desde el backend.
 * Sin el rol de administrador envía de vuelta al panel; sin sesión, al login.
 * Debe renderizarse dentro de un `<Suspense>` porque lee la cookie y los parámetros de URL.
 */
export async function UsersPanel({ searchParams }: UsersPanelProps) {
  // Los usuarios y la sesión son por petición: se indica a Next que no se prerenderice este bloque.
  await connection();

  const filtros = parseFiltrosUsuarios(await searchParams);
  const session = await getSession();
  if (!session) {
    redirect("/login");
  }
  if (!session.roles.includes(ROLE_ADMINISTRADOR_DEL_SISTEMA)) {
    redirect("/dashboard");
  }

  // Paso 1: consultar usuarios y roles en paralelo; el try solo cubre las llamadas, no el JSX.
  let resultado: UsuariosPaginados;
  let roles: Rol[];
  try {
    [resultado, roles] = await Promise.all([
      listUsuariosInBackend(session.token, {
        pagina: filtros.pagina,
        tamano: TAMANO_PAGINA,
        busqueda: filtros.busqueda,
        estado: filtros.estado,
      }),
      listRolesInBackend(session.token),
    ]);
  } catch (error) {
    // Un 401 significa que el token venció: se vuelve a pedir inicio de sesión.
    if (error instanceof BackendError && error.status === 401) {
      redirect("/login");
    }
    console.error("Error al consultar los usuarios:", error);
    const message =
      error instanceof BackendError
        ? error.message
        : "No se pudieron cargar los usuarios. Intenta más tarde.";
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

  // Paso 2: mostrar la administración con la página y los filtros solicitados.
  return (
    <UsersManager
      resultado={resultado}
      roles={roles.filter((rol) => rol.activo)}
      usuarioActualId={session.usuarioId}
      busqueda={filtros.busqueda}
      estado={filtros.estado}
    />
  );
}
