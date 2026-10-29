import { redirect } from "next/navigation";
import { connection } from "next/server";
import { UserRound } from "lucide-react";
import { RetryButton } from "@/components/retry-button";
import { BackendError, getPerfilInBackend } from "@/lib/backend/client";
import type { Perfil } from "@/lib/backend/types";
import { getSession } from "@/lib/auth/session-claims";
import { PasswordForm } from "./password-form";
import { ProfileForm } from "./profile-form";

/**
 * Perfil del usuario de la sesión: datos personales y cambio de contraseña.
 * Los datos se leen del backend en cada petición. Sin sesión o con token vencido, envía al login.
 * Debe renderizarse dentro de un `<Suspense>` porque lee la cookie.
 */
export async function ProfileDetails() {
  // Los datos del perfil son por petición: se indica a Next que no se prerenderice este bloque.
  await connection();

  const session = await getSession();
  if (!session) {
    redirect("/login");
  }

  // Paso 1: cargar el perfil desde el backend.
  let perfil: Perfil;
  try {
    perfil = await getPerfilInBackend(session.token);
  } catch (error) {
    if (error instanceof BackendError && error.status === 401) {
      redirect("/login");
    }
    console.error("Error al consultar el perfil:", error);
    const message =
      error instanceof BackendError ? error.message : "No se pudo cargar tu perfil. Intenta más tarde.";
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

  // Paso 2: mostrar la cabecera del perfil y los formularios.
  return (
    <div className="space-y-6">
      <ProfileHeader perfil={perfil} />
      <div className="grid gap-6 lg:grid-cols-2">
        <ProfileForm key={JSON.stringify(perfil)} perfil={perfil} />
        <PasswordForm />
      </div>
    </div>
  );
}

/** Cabecera con iniciales, nombre completo, correo y roles del usuario. */
function ProfileHeader({ perfil }: { perfil: Perfil }) {
  const nombreCompleto = [perfil.primerNombre, perfil.segundoNombre, perfil.primerApellido, perfil.segundoApellido]
    .filter(Boolean)
    .join(" ");
  const iniciales = `${perfil.primerNombre.charAt(0)}${perfil.primerApellido.charAt(0)}`.toUpperCase();

  return (
    <header className="relative overflow-hidden rounded-3xl bg-linear-to-br from-corporativo-azul via-corporativo-azul to-corporativo-verde p-8 text-white shadow-xl">
      <div aria-hidden="true" className="absolute -top-16 -right-10 h-56 w-56 rounded-full bg-corporativo-amarillo/20 blur-3xl" />
      <div className="relative flex flex-col gap-6 sm:flex-row sm:items-center">
        <span
          aria-hidden="true"
          className="flex h-20 w-20 shrink-0 items-center justify-center rounded-2xl bg-corporativo-amarillo text-2xl font-bold text-corporativo-azul shadow-lg"
        >
          {iniciales || <UserRound className="h-8 w-8" />}
        </span>
        <div className="min-w-0 space-y-2">
          <h1 className="truncate text-2xl font-bold tracking-tight">{nombreCompleto}</h1>
          <p className="text-white/80">{perfil.correoElectronico}</p>
          <ul className="flex flex-wrap gap-2 pt-1">
            {perfil.roles.length > 0 ? (
              perfil.roles.map((rol) => (
                <li key={rol} className="rounded-full bg-white/15 px-3 py-1 text-xs font-medium backdrop-blur-sm">
                  {rol}
                </li>
              ))
            ) : (
              <li className="text-sm text-white/70">Sin roles asignados</li>
            )}
          </ul>
        </div>
      </div>
    </header>
  );
}
