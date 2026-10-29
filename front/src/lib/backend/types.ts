/** Credenciales enviadas a POST /api/autenticacion/login (según OpenAPI del backend). */
export type LoginRequest = {
  correoElectronico: string;
  contrasena: string;
};

/** Respuesta 200 de POST /api/autenticacion/login. */
export type LoginResponse = {
  accessToken: string;
  tokenType: string;
  /** Fecha de expiración del token en formato ISO 8601 (UTC). */
  expiraUtc: string;
};

/** Rol asignado a un usuario, tal como lo devuelve GET /api/usuarios. */
export type RolAsignado = {
  id: string;
  nombre: string;
};

/** Perfil del usuario autenticado, devuelto por GET /api/autenticacion/perfil. */
export type Perfil = {
  id: string;
  primerNombre: string;
  segundoNombre: string | null;
  primerApellido: string;
  segundoApellido: string | null;
  correoElectronico: string;
  roles: string[];
};

/** Datos personales editables en PUT /api/autenticacion/perfil. */
export type ActualizarPerfilRequest = {
  primerNombre: string;
  segundoNombre: string | null;
  primerApellido: string;
  segundoApellido: string | null;
};

/** Cuerpo de PUT /api/autenticacion/perfil/contrasena. */
export type CambiarContrasenaRequest = {
  contrasenaActual: string;
  contrasenaNueva: string;
};

/** Rol del catálogo devuelto por GET /api/roles. */
export type Rol = {
  id: string;
  nombre: string;
  descripcion: string | null;
  activo: boolean;
  /** Fecha de creación en formato ISO 8601 (UTC). */
  fechaCreacionUtc: string;
};

/** Datos personales y correo editables de un usuario. */
export type UsuarioDatos = {
  primerNombre: string;
  segundoNombre: string | null;
  primerApellido: string;
  segundoApellido: string | null;
  correoElectronico: string;
};

/** Cuerpo de POST /api/usuarios. Los roles son opcionales. */
export type CrearUsuarioRequest = UsuarioDatos & {
  contrasena: string;
  roles?: string[];
};

/** Cuerpo de PUT /api/usuarios/{id}. Si se envían roles, reemplazan los actuales. */
export type ActualizarUsuarioRequest = UsuarioDatos & {
  roles?: string[];
};

/** Estado por el que se puede filtrar el listado de usuarios. */
export type EstadoFiltroUsuarios = "todos" | "activos" | "inactivos";

/** Parámetros de consulta de GET /api/usuarios. */
export type ListarUsuariosParams = {
  pagina?: number;
  tamano?: number;
  busqueda?: string;
  estado?: EstadoFiltroUsuarios;
};

/** Usuario dentro de `items` en la respuesta de GET /api/usuarios. */
export type Usuario = {
  id: string;
  primerNombre: string;
  segundoNombre: string | null;
  primerApellido: string;
  segundoApellido: string | null;
  correoElectronico: string;
  activo: boolean;
  /** Fecha de creación en formato ISO 8601 (UTC). */
  fechaCreacionUtc: string;
  roles: RolAsignado[];
};

/** Página de usuarios devuelta por GET /api/usuarios. */
export type UsuariosPaginados = {
  items: Usuario[];
  pagina: number;
  tamano: number;
  /** Usuarios que cumplen la búsqueda y el filtro. */
  total: number;
  totalPaginas: number;
  /** Total de usuarios registrados, sin filtros. */
  totalUsuarios: number;
  /** Total de usuarios activos, sin filtros. */
  totalActivos: number;
  /** Total de usuarios inactivos, sin filtros. */
  totalInactivos: number;
};
