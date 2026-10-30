import { Suspense } from "react";
import type { Metadata } from "next";
import { PageBreadcrumb } from "../page-breadcrumb";
import { AvisoContrasenaTemporal } from "./aviso-contrasena-temporal";
import { ProfileDetails } from "./profile-details";
import { ProfileSkeleton } from "./profile-skeleton";

export const metadata: Metadata = {
  title: "Mi perfil | RUNPD",
};

/**
 * Página "Mi perfil": datos de la sesión actual.
 * Muestra un aviso si llega `contrasenaTemporal=1`, que envía el login tras una contraseña temporal.
 */
export default async function ProfilePage({ searchParams }: PageProps<"/dashboard/perfil">) {
  const { contrasenaTemporal } = await searchParams;

  return (
    <div>
      <PageBreadcrumb items={[{ label: "Mi perfil" }]} />

      {contrasenaTemporal === "1" && <AvisoContrasenaTemporal />}

      <Suspense fallback={<ProfileSkeleton />}>
        <ProfileDetails />
      </Suspense>
    </div>
  );
}
