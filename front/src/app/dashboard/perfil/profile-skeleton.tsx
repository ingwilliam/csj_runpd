/** Esqueleto de "Mi perfil" mientras se consultan los datos del usuario. */
export function ProfileSkeleton() {
  return (
    <div role="status" className="space-y-6">
      <span className="sr-only">Cargando tu perfil…</span>

      <div aria-hidden="true" className="animate-pulse space-y-6">
        <div className="h-44 rounded-3xl bg-slate-200" />

        <div className="grid gap-6 lg:grid-cols-2">
          {Array.from({ length: 2 }).map((_, index) => (
            <div key={index} className="space-y-4 rounded-2xl border border-slate-200 bg-white p-6">
              <div className="h-6 w-1/2 rounded bg-slate-200" />
              <div className="h-4 w-3/4 rounded bg-slate-200" />
              <div className="h-11 w-full rounded-lg bg-slate-200" />
              <div className="h-11 w-full rounded-lg bg-slate-200" />
              <div className="h-10 w-32 rounded-lg bg-slate-200" />
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
