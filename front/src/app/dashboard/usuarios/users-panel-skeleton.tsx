/** Esqueleto de carga mientras se consultan los usuarios y los roles. */
export function UsersPanelSkeleton() {
  return (
    <div role="status" className="space-y-6">
      <span className="sr-only">Cargando usuarios…</span>

      <div aria-hidden="true" className="animate-pulse space-y-6">
        <div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
          <div className="space-y-2">
            <div className="h-8 w-64 rounded-lg bg-slate-200" />
            <div className="h-4 w-48 rounded bg-slate-200" />
          </div>
          <div className="h-11 w-full rounded-lg bg-slate-200 sm:w-72" />
        </div>
        <div className="flex gap-2">
          <div className="h-9 w-24 rounded-full bg-slate-200" />
          <div className="h-9 w-24 rounded-full bg-slate-200" />
          <div className="h-9 w-24 rounded-full bg-slate-200" />
        </div>
        <div className="space-y-1 rounded-2xl border border-slate-200 bg-white p-4">
          {Array.from({ length: 6 }).map((_, index) => (
            <div key={index} className="flex items-center gap-4 py-4">
              <div className="h-10 w-10 rounded-full bg-slate-200" />
              <div className="flex-1 space-y-2">
                <div className="h-4 w-1/3 rounded bg-slate-200" />
                <div className="h-3 w-1/4 rounded bg-slate-200" />
              </div>
              <div className="h-6 w-20 rounded-full bg-slate-200" />
            </div>
          ))}
        </div>
        <div className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white px-4 py-3 sm:flex-row sm:items-center sm:justify-between">
          <div className="h-4 w-40 rounded bg-slate-200" />
          <div className="h-9 w-56 rounded-lg bg-slate-200" />
        </div>
      </div>
    </div>
  );
}
