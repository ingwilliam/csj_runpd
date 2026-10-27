import { getSession } from "@/lib/auth/session-claims";
import { getModulesForRoles } from "@/lib/auth/modules";
import { ModuleCard } from "./module-card";

/**
 * Muestra una tarjeta por cada módulo permitido según los roles de la sesión.
 * Lee la cookie, por lo que debe renderizarse dentro de un `<Suspense>`.
 */
export async function DashboardModules() {
  const session = await getSession();
  const modules = getModulesForRoles(session?.roles ?? []);

  if (modules.length === 0) {
    return (
      <p className="rounded-2xl border border-dashed border-corporativo-azul/30 bg-white p-6 text-center text-slate-600">
        Tu usuario no tiene módulos asignados. Contacta al administrador del sistema.
      </p>
    );
  }

  return (
    <ul className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
      {modules.map((module, index) => (
        <li key={module.id}>
          <ModuleCard module={module} index={index} />
        </li>
      ))}
    </ul>
  );
}
