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
  /** Indica que el usuario entró con una contraseña temporal y debe cambiarla. */
  debeCambiarContrasena: boolean;
};

/** Cuerpo de POST /api/autenticacion/recuperar-contrasena. */
export type RecuperarContrasenaRequest = {
  correoElectronico: string;
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

/** Estado funcional de un despacho, tal como lo exige el backend. */
export type EstadoDespacho = "CREADO" | "MODIFICADO" | "TRASLADADO" | "SUPRIMIDO";

/** Estado por el que se puede filtrar el listado de despachos. */
export type EstadoFiltroDespachos = "todos" | "activos" | "inactivos";

/** Parámetros de consulta de GET /api/despachos. */
export type ListarDespachosParams = {
  pagina?: number;
  tamano?: number;
  busqueda?: string;
  estado?: EstadoFiltroDespachos;
};

/** Datos editables de un despacho, usados al crear y al actualizar. */
export type DespachoDatos = {
  codigo: string;
  nombre: string;
  estado: EstadoDespacho;
  promiscuo: boolean;
  direccion: string | null;
  complementoDireccion: string | null;
  telefonoFijo: string | null;
  telefonoCelular: string | null;
  correo: string | null;
  horarioAtencion: string | null;
  especialidadId: string | null;
  subEspecialidadId: string | null;
  municipioId: string;
  jurisdiccionId: string;
  distritoId: string | null;
  circuitoId: string | null;
  distritoSubEspecialidadId: string | null;
  circuitoSubEspecialidadId: string | null;
  entidadId: string | null;
};

/** Cuerpo de POST /api/despachos. */
export type CrearDespachoRequest = DespachoDatos;

/** Cuerpo de PUT /api/despachos/{id}. */
export type ActualizarDespachoRequest = DespachoDatos;

/** Despacho devuelto por GET /api/despachos. */
export type Despacho = DespachoDatos & {
  id: string;
  activo: boolean;
  /** Fecha de creación en formato ISO 8601 (UTC). */
  fechaCreacionUtc: string;
};

/** Página de despachos devuelta por GET /api/despachos. */
export type DespachosPaginados = {
  items: Despacho[];
  pagina: number;
  tamano: number;
  /** Despachos que cumplen la búsqueda y el filtro. */
  total: number;
  totalPaginas: number;
  /** Total de despachos registrados, sin filtros. */
  totalDespachos: number;
  /** Total de despachos activos, sin filtros. */
  totalActivos: number;
  /** Total de despachos inactivos, sin filtros. */
  totalInactivos: number;
};

/** Departamento del catálogo GET /api/maestras/departamentos. */
export type Departamento = {
  id: string;
  nombre: string;
  codigoDivpola: string;
  activo: boolean;
  fechaCreacionUtc: string;
};

/** Municipio del catálogo GET /api/maestras/municipios. */
export type Municipio = {
  id: string;
  nombre: string;
  tipo: string | null;
  pdet: boolean;
  codigoDivpola: string;
  departamentoId: string;
  consejoSeccionalId: string | null;
  direccionSeccionalId: string | null;
  activo: boolean;
  fechaCreacionUtc: string;
};

/** Jurisdicción del catálogo GET /api/maestras/jurisdicciones. */
export type Jurisdiccion = {
  id: string;
  nombre: string;
  funcion: boolean;
  activo: boolean;
  fechaCreacionUtc: string;
};

/** Distrito del catálogo GET /api/maestras/distritos. */
export type Distrito = {
  id: string;
  nombre: string;
  jurisdiccionId: string;
  subEspecialidadId: string | null;
  activo: boolean;
  fechaCreacionUtc: string;
};

/** Circuito del catálogo GET /api/maestras/circuitos. */
export type Circuito = {
  id: string;
  nombre: string;
  distritoId: string;
  subEspecialidadId: string | null;
  activo: boolean;
  fechaCreacionUtc: string;
};

/** Especialidad del catálogo GET /api/maestras/especialidades. */
export type Especialidad = {
  id: string;
  nombre: string;
  descripcion: string | null;
  activo: boolean;
  fechaCreacionUtc: string;
};

/** Sub especialidad del catálogo GET /api/maestras/sub-especialidades. */
export type SubEspecialidad = {
  id: string;
  especialidadId: string | null;
  nombre: string;
  descripcion: string | null;
  promiscuo: boolean;
  codigo: string | null;
  activo: boolean;
  fechaCreacionUtc: string;
};

/** Entidad del catálogo GET /api/maestras/entidades. */
export type Entidad = {
  id: string;
  codigo: string;
  nombre: string;
  jurisdiccionId: string;
  activo: boolean;
  fechaCreacionUtc: string;
};

/** Catálogos maestros que alimentan el formulario y el detalle de despachos. */
export type CatalogosDespachos = {
  departamentos: Departamento[];
  jurisdicciones: Jurisdiccion[];
  distritos: Distrito[];
  circuitos: Circuito[];
  especialidades: Especialidad[];
  subEspecialidades: SubEspecialidad[];
  entidades: Entidad[];
};
