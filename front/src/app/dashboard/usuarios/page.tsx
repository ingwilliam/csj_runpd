import { Suspense } from "react";
import { DashboardHeader } from "../dashboard-header";
import { UsersPanel } from "./users-panel";

/** Administración de usuarios. Solo para el rol "Administrador del sistema" (se valida en `UsersPanel`). */
export default function UsuariosPage() {
  return (
    <main className="min-h-screen bg-slate-100">
      <DashboardHeader />
      <section className="p-6">
        <Suspense fallback={null}>
          <UsersPanel />
        </Suspense>
      </section>
    </main>
  );
}
