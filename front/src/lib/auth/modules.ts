/** Rol con acceso a la administración de usuarios; el backend exige el mismo rol en GET /api/usuarios. */
export const ROLE_ADMINISTRADOR_DEL_SISTEMA = "Administrador del sistema";

/** Módulo del panel visible para los usuarios que tengan alguno de sus roles. */
export type AppModule = {
  id: string;
  title: string;
  description: string;
  href: string;
  roles: string[];
};

/** Catálogo de módulos del panel. */
export const MODULES: AppModule[] = [
  {
    id: "usuarios",
    title: "Administración de usuarios",
    description: "Consulta los usuarios, sus roles y su estado.",
    href: "/dashboard/usuarios",
    roles: [ROLE_ADMINISTRADOR_DEL_SISTEMA],
  },
];

/**
 * Obtiene los módulos a los que tiene acceso un usuario con los roles indicados.
 * @param roles Roles del usuario según el token.
 * @returns Módulos cuyo rol coincide con alguno del usuario, en el orden del catálogo.
 */
export function getModulesForRoles(roles: string[]): AppModule[] {
  return MODULES.filter((module) => module.roles.some((role) => roles.includes(role)));
}
