import { PageBreadcrumb } from "../page-breadcrumb";
import { ProfileSkeleton } from "./profile-skeleton";

/** Carga de la pantalla "Mi perfil". */
export default function Loading() {
  return (
    <div aria-busy="true">
      <PageBreadcrumb items={[{ label: "Mi perfil" }]} />
      <ProfileSkeleton />
    </div>
  );
}
