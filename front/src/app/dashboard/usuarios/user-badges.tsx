/** Lista de roles del usuario como etiquetas. */
export function UserRoleChips({ roles }: { roles: string[] }) {
  if (roles.length === 0) {
    return <span className="text-sm text-slate-400">Sin roles</span>;
  }
  return (
    <ul className="flex flex-wrap gap-2">
      {roles.map((rol) => (
        <li
          key={rol}
          className="rounded-full bg-corporativo-azul/10 px-2.5 py-0.5 text-xs font-medium text-corporativo-azul"
        >
          {rol}
        </li>
      ))}
    </ul>
  );
}

/** Indicador de estado activo o inactivo con color y texto. */
export function UserStatusBadge({ activo }: { activo: boolean }) {
  return activo ? (
    <span className="rounded-full bg-corporativo-verde/15 px-2.5 py-0.5 text-xs font-medium text-corporativo-verde">
      Activo
    </span>
  ) : (
    <span className="rounded-full bg-slate-200 px-2.5 py-0.5 text-xs font-medium text-slate-600">
      Inactivo
    </span>
  );
}
