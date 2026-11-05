import { Suspense } from "react";
import type { Metadata } from "next";
import { PageBreadcrumb } from "../page-breadcrumb";
import { DespachosPanel } from "./despachos-panel";
import { DespachosPanelSkeleton } from "./despachos-panel-skeleton";

export const metadata: Metadata = {
  title: "Despachos | RUNPD",
};

/** Administración de despachos. La valida el rol de lectura en `DespachosPanel`. */
export default function DespachosPage(props: PageProps<"/dashboard/despachos">) {
  return (
    <div>
      <PageBreadcrumb items={[{ label: "Administración de despachos" }]} />

      <Suspense fallback={<DespachosPanelSkeleton />}>
        <DespachosPanel searchParams={props.searchParams} />
      </Suspense>
    </div>
  );
}
