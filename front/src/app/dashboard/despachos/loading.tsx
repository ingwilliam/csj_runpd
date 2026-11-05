import { PageBreadcrumb } from "../page-breadcrumb";
import { DespachosPanelSkeleton } from "./despachos-panel-skeleton";

/** Carga de la administración de despachos. */
export default function Loading() {
  return (
    <div aria-busy="true">
      <PageBreadcrumb items={[{ label: "Administración de despachos" }]} />
      <DespachosPanelSkeleton />
    </div>
  );
}
