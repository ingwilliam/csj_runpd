import { redirect } from "next/navigation";
import { connection } from "next/server";
import { BackendError, listUsuariosInBackend } from "@/lib/backend/client";
import type { Usuario } from "@/lib/backend/types";
import { ROLE_ADMINISTRADOR_DEL_SISTEMA } from "@/lib/auth/modules";
import { getSession } from "@/lib/auth/session-claims";
import { UsersTable } from "./users-table";

/**
 * Carga los usuarios desde el backend y los muestra.
 * Sin el rol de administrador envía de vuelta al panel; sin sesión, al login.
 * Debe renderizarse dentro de un `<Suspense>` porque lee la cookie.
 */
export async function UsersPanel() {
  // Los usuarios y la sesión son por petición: se indica a Next que no se prerenderice este bloque.
  await connection();

  const session = await getSession();
  if (!session) {
    redirect("/login");
  }
  if (!session.roles.includes(ROLE_ADMINISTRADOR_DEL_SISTEMA)) {
    redirect("/dashboard");
  }

  // Paso 1: consultar el backend; el try solo cubre la llamada, no el JSX.
  let usuarios: Usuario[];
  try {
    usuarios = await listUsuariosInBackend(session.token);
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
      <p role="alert" className="rounded-2xl border border-red-200 bg-red-50 p-6 text-red-700">
        {message}
      </p>
    );
  }

  // Paso 2: mostrar la tabla con los datos cargados.
  return <UsersTable usuarios={usuarios} />;
}
