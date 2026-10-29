import { PageBreadcrumb } from "../page-breadcrumb";
import { UsersPanelSkeleton } from "./users-panel-skeleton";

/** Carga de la administración de usuarios. */
export default function Loading() {
  return (
    <div aria-busy="true">
      <PageBreadcrumb items={[{ label: "Administración de usuarios" }]} />
      <UsersPanelSkeleton />
    </div>
  );
}
