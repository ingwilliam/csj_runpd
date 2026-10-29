import { Suspense } from "react";
import type { Metadata } from "next";
import { PageBreadcrumb } from "../page-breadcrumb";
import { UsersPanel } from "./users-panel";
import { UsersPanelSkeleton } from "./users-panel-skeleton";

export const metadata: Metadata = {
  title: "Usuarios | RUNPD",
};

/** Administración de usuarios. Solo para el rol "Administrador del sistema" (se valida en `UsersPanel`). */
export default function UsuariosPage(props: PageProps<"/dashboard/usuarios">) {
  return (
    <div>
      <PageBreadcrumb items={[{ label: "Administración de usuarios" }]} />

      <Suspense fallback={<UsersPanelSkeleton />}>
        <UsersPanel searchParams={props.searchParams} />
      </Suspense>
    </div>
  );
}
