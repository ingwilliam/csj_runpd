/** Esqueleto de las tarjetas de módulos mientras se resuelven los roles de la sesión. */
export function ModuleCardsSkeleton() {
  return (
    <div role="status">
      <span className="sr-only">Cargando módulos…</span>
      <div aria-hidden="true" className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
        {Array.from({ length: 3 }).map((_, index) => (
          <div
            key={index}
            className="h-52 animate-pulse rounded-2xl border border-slate-200 bg-white p-6"
          >
            <div className="h-12 w-12 rounded-xl bg-slate-200" />
            <div className="mt-5 space-y-2">
              <div className="h-5 w-2/3 rounded bg-slate-200" />
              <div className="h-4 w-full rounded bg-slate-200" />
              <div className="h-4 w-4/5 rounded bg-slate-200" />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
