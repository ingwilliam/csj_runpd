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

/** Usuario devuelto por GET /api/usuarios (solo lo que muestra la interfaz). */
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
