import { TriangleAlert } from "lucide-react";

/**
 * Aviso que recomienda cambiar la contraseña temporal tras iniciar sesión.
 * No bloquea la navegación: el usuario puede ignorarlo y seguir usando el sistema.
 */
export function AvisoContrasenaTemporal() {
  return (
    <div
      role="status"
      className="mb-6 flex items-start gap-3 rounded-lg border border-corporativo-amarillo bg-corporativo-amarillo/20 px-4 py-3 text-sm text-slate-800"
    >
      <TriangleAlert className="mt-0.5 h-4 w-4 shrink-0 text-corporativo-azul" />
      <span>
        Tu contraseña es temporal. Te recomendamos cambiarla en la sección &quot;Cambiar contraseña&quot;.
      </span>
    </div>
  );
}
