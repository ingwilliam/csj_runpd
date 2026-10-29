import { ModuleCardsSkeleton } from "./module-cards-skeleton";

/** Carga del panel mientras se resuelven los módulos de la sesión. */
export default function Loading() {
  return (
    <div className="space-y-6" aria-busy="true">
      <div aria-hidden="true" className="h-40 animate-pulse rounded-3xl bg-slate-200" />
      <ModuleCardsSkeleton />
    </div>
  );
}
