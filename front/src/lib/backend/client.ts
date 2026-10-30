import type {
  ActualizarPerfilRequest,
  ActualizarUsuarioRequest,
  CambiarContrasenaRequest,
  CrearUsuarioRequest,
  ListarUsuariosParams,
  LoginRequest,
  LoginResponse,
  Perfil,
  RecuperarContrasenaRequest,
  Rol,
  Usuario,
  UsuariosPaginados,
} from "@/lib/backend/types";

/** Error devuelto por el backend con un código HTTP y un mensaje listo para mostrar al usuario. */
export class BackendError extends Error {
  /**
   * @param status Código HTTP devuelto por el backend.
   * @param message Mensaje en español apto para la interfaz.
   */
  constructor(
    readonly status: number,
    message: string,
  ) {
    super(message);
    this.name = "BackendError";
  }
}

/**
 * Solicita una contraseña temporal para el correo indicado. El backend responde igual
 * exista o no el correo, así que el resultado no revela si la cuenta está registrada.
 * @param datos Correo de la cuenta.
 * @returns Mensaje genérico enviado por el backend.
 * @throws {BackendError} Si el backend rechaza la solicitud (400) u otro error HTTP.
 * @throws {Error} Si falta la variable de entorno API_URL o no hay conexión con el backend.
 */
export async function recuperarContrasenaInBackend(
  datos: RecuperarContrasenaRequest,
): Promise<string> {
  const response = await fetch(`${getApiUrl()}/autenticacion/recuperar-contrasena`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Accept: "application/json",
    },
    body: JSON.stringify(datos),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new BackendError(
      response.status,
      await getErrorMessage(response, "No se pudo procesar la solicitud. Intenta más tarde."),
    );
  }

  const body = (await response.json()) as { mensaje?: unknown };
  return typeof body.mensaje === "string" ? body.mensaje : "Revisa tu correo electrónico.";
}

/**
 * Autentica un usuario contra el backend.
 * @param credentials Correo y contraseña ya validados.
 * @returns Token de acceso y su fecha de expiración.
 * @throws {BackendError} Si el backend responde con un error HTTP.
 * @throws {Error} Si falta la variable de entorno API_URL o no hay conexión con el backend.
 */
export async function loginInBackend(
  credentials: LoginRequest,
): Promise<LoginResponse> {
  const response = await fetch(`${getApiUrl()}/autenticacion/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Accept: "application/json",
    },
    body: JSON.stringify(credentials),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new BackendError(response.status, getLoginErrorMessage(response.status));
  }

  return (await response.json()) as LoginResponse;
}

/**
 * Obtiene una página de usuarios. Requiere el rol "Administrador del sistema" en el token.
 * @param accessToken Token de la sesión, enviado como Bearer.
 * @param params Página, tamaño, búsqueda y estado solicitados.
 * @returns La página de usuarios y los totales del listado.
 * @throws {BackendError} Si el backend responde con un error HTTP (400, 401, 403, etc.).
 * @throws {Error} Si falta la variable de entorno API_URL o no hay conexión con el backend.
 */
export async function listUsuariosInBackend(
  accessToken: string,
  params: ListarUsuariosParams = {},
): Promise<UsuariosPaginados> {
  const query = new URLSearchParams();
  query.set("pagina", String(params.pagina ?? 1));
  if (params.tamano !== undefined) {
    query.set("tamano", String(params.tamano));
  }
  const busqueda = params.busqueda?.trim();
  if (busqueda) {
    query.set("busqueda", busqueda);
  }
  if (params.estado && params.estado !== "todos") {
    query.set("estado", params.estado);
  }

  const response = await requestUsuarios(accessToken, `?${query.toString()}`);
  return (await response.json()) as UsuariosPaginados;
}

/**
 * Obtiene un usuario por su identificador.
 * @param accessToken Token de la sesión, enviado como Bearer.
 * @param id Identificador del usuario.
 * @returns El usuario solicitado con sus roles.
 * @throws {BackendError} Si el backend responde con un error HTTP.
 */
export async function getUsuarioInBackend(accessToken: string, id: string): Promise<Usuario> {
  const response = await requestUsuarios(accessToken, `/${encodeURIComponent(id)}`);
  return (await response.json()) as Usuario;
}

/**
 * Crea un usuario y le asigna los roles indicados en la misma petición.
 * @param accessToken Token de la sesión, enviado como Bearer.
 * @param datos Datos personales, correo, contraseña y roles opcionales.
 * @returns El usuario creado.
 * @throws {BackendError} Si el correo ya existe o algún rol no es válido.
 */
export async function createUsuarioInBackend(
  accessToken: string,
  datos: CrearUsuarioRequest,
): Promise<Usuario> {
  const response = await requestUsuarios(accessToken, "", {
    method: "POST",
    body: JSON.stringify(datos),
  });
  return (await response.json()) as Usuario;
}

/**
 * Actualiza los datos de un usuario. Si se envían roles, se reemplazan en la misma petición.
 * @param accessToken Token de la sesión, enviado como Bearer.
 * @param id Identificador del usuario.
 * @param datos Datos nuevos y roles opcionales.
 * @returns El usuario actualizado.
 * @throws {BackendError} Si el usuario no existe o los datos no son válidos.
 */
export async function updateUsuarioInBackend(
  accessToken: string,
  id: string,
  datos: ActualizarUsuarioRequest,
): Promise<Usuario> {
  const response = await requestUsuarios(accessToken, `/${encodeURIComponent(id)}`, {
    method: "PUT",
    body: JSON.stringify(datos),
  });
  return (await response.json()) as Usuario;
}

/**
 * Reemplaza los roles activos asignados a un usuario.
 * @param accessToken Token de la sesión, enviado como Bearer.
 * @param id Identificador del usuario.
 * @param roles Identificadores de los roles que tendrá el usuario.
 * @returns El usuario actualizado.
 * @throws {BackendError} Si el usuario no existe o algún rol no es válido.
 */
export async function updateUsuarioRolesInBackend(
  accessToken: string,
  id: string,
  roles: string[],
): Promise<Usuario> {
  const response = await requestUsuarios(accessToken, `/${encodeURIComponent(id)}/roles`, {
    method: "PUT",
    body: JSON.stringify({ roles }),
  });
  return (await response.json()) as Usuario;
}

/**
 * Activa o desactiva un usuario sin borrar su registro.
 * @param accessToken Token de la sesión, enviado como Bearer.
 * @param id Identificador del usuario.
 * @param activo Nuevo estado activo.
 * @returns El usuario actualizado.
 * @throws {BackendError} Si el usuario no existe o dejaría el sistema sin administradores.
 */
export async function changeUsuarioActivoInBackend(
  accessToken: string,
  id: string,
  activo: boolean,
): Promise<Usuario> {
  const response = await requestUsuarios(
    accessToken,
    `/${encodeURIComponent(id)}/activo/${activo}`,
    { method: "PATCH" },
  );
  return (await response.json()) as Usuario;
}

/**
 * Obtiene el catálogo completo de roles, incluidos los desactivados.
 * @param accessToken Token de la sesión, enviado como Bearer.
 * @returns Los roles ordenados por nombre.
 * @throws {BackendError} Si el backend responde con un error HTTP.
 */
export async function listRolesInBackend(accessToken: string): Promise<Rol[]> {
  const response = await fetch(`${getApiUrl()}/roles`, {
    headers: {
      Accept: "application/json",
      Authorization: `Bearer ${accessToken}`,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new BackendError(
      response.status,
      await getErrorMessage(response, "No se pudieron cargar los roles. Intenta más tarde."),
    );
  }

  return (await response.json()) as Rol[];
}

/**
 * Envía una petición a los endpoints de usuarios y lanza `BackendError` si falla.
 * @param accessToken Token de la sesión.
 * @param subruta Sufijo de la ruta de usuarios (por ejemplo "/{id}").
 * @param init Opciones extra de la petición.
 * @returns La respuesta exitosa.
 */
async function requestUsuarios(
  accessToken: string,
  subruta: string,
  init: Pick<RequestInit, "method" | "body"> = {},
): Promise<Response> {
  const response = await fetch(`${getApiUrl()}/usuarios${subruta}`, {
    ...init,
    headers: {
      Accept: "application/json",
      ...(init.body === undefined ? {} : { "Content-Type": "application/json" }),
      Authorization: `Bearer ${accessToken}`,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new BackendError(
      response.status,
      await getErrorMessage(response, "No se pudo completar la operación. Intenta más tarde."),
    );
  }

  return response;
}

/**
 * Obtiene el perfil del usuario de la sesión.
 * @param accessToken Token de la sesión, enviado como Bearer.
 * @returns Datos personales, correo y roles del usuario.
 * @throws {BackendError} Si el backend responde con un error HTTP (401, 404, etc.).
 */
export async function getPerfilInBackend(accessToken: string): Promise<Perfil> {
  const response = await requestPerfil(accessToken, "GET");
  return (await response.json()) as Perfil;
}

/**
 * Actualiza nombres y apellidos del usuario de la sesión. El correo y los roles no cambian.
 * @param accessToken Token de la sesión, enviado como Bearer.
 * @param datos Datos personales nuevos.
 * @returns El perfil actualizado.
 * @throws {BackendError} Si el backend responde con un error HTTP.
 */
export async function actualizarPerfilInBackend(
  accessToken: string,
  datos: ActualizarPerfilRequest,
): Promise<Perfil> {
  const response = await requestPerfil(accessToken, "PUT", datos);
  return (await response.json()) as Perfil;
}

/**
 * Cambia la contraseña del usuario de la sesión. La sesión actual sigue activa.
 * @param accessToken Token de la sesión, enviado como Bearer.
 * @param datos Contraseña actual y nueva.
 * @throws {BackendError} Si la contraseña actual no coincide (400) u otro error HTTP.
 */
export async function cambiarContrasenaInBackend(
  accessToken: string,
  datos: CambiarContrasenaRequest,
): Promise<void> {
  await requestPerfil(accessToken, "PUT", datos, "/contrasena");
}

/**
 * Envía una petición a los endpoints de perfil y lanza `BackendError` si la respuesta no es exitosa.
 * @param accessToken Token de la sesión.
 * @param method Método HTTP.
 * @param body Cuerpo JSON opcional.
 * @param subruta Sufijo de la ruta de perfil (por ejemplo "/contrasena").
 * @returns La respuesta exitosa.
 */
async function requestPerfil(
  accessToken: string,
  method: "GET" | "PUT",
  body?: unknown,
  subruta = "",
): Promise<Response> {
  const response = await fetch(`${getApiUrl()}/autenticacion/perfil${subruta}`, {
    method,
    headers: {
      Accept: "application/json",
      ...(body === undefined ? {} : { "Content-Type": "application/json" }),
      Authorization: `Bearer ${accessToken}`,
    },
    body: body === undefined ? undefined : JSON.stringify(body),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new BackendError(response.status, await getPerfilErrorMessage(response));
  }
  return response;
}

/**
 * Obtiene el mensaje para el usuario de un error de perfil.
 * Si el backend envía `mensaje` (p. ej. contraseña actual incorrecta), se usa tal cual.
 */
async function getPerfilErrorMessage(response: Response): Promise<string> {
  if (response.status === 400) {
    const body = (await response.json().catch(() => null)) as { mensaje?: unknown } | null;
    if (typeof body?.mensaje === "string") {
      return body.mensaje;
    }
    return "Los datos ingresados no son válidos.";
  }
  if (response.status === 401) {
    return "Tu sesión expiró. Vuelve a iniciar sesión.";
  }
  if (response.status === 404) {
    return "No se encontró tu perfil. Contacta al administrador del sistema.";
  }
  return "No se pudo completar la operación. Intenta más tarde.";
}

/** Lee la URL base del backend desde el entorno y falla de forma explícita si no existe. */
function getApiUrl(): string {
  const apiUrl = process.env.API_URL;
  if (!apiUrl) {
    throw new Error("La variable de entorno API_URL no está configurada.");
  }
  return apiUrl;
}

/** Traduce el código HTTP de un error de login a un mensaje para el usuario. */
function getLoginErrorMessage(status: number): string {
  if (status === 400) {
    return "Los datos ingresados no son válidos.";
  }
  if (status === 401) {
    return "Correo o contraseña incorrectos.";
  }
  return "El servicio de autenticación no está disponible. Intenta más tarde.";
}

/**
 * Obtiene el mensaje para el usuario de un error del backend.
 * Prioriza el `mensaje` enviado por la API y, si no existe, el primer error de validación.
 * @param response Respuesta no exitosa del backend.
 * @param fallback Mensaje genérico para errores sin detalle.
 * @returns Mensaje en español apto para la interfaz.
 */
async function getErrorMessage(response: Response, fallback: string): Promise<string> {
  if (response.status === 401) {
    return "Tu sesión expiró. Vuelve a iniciar sesión.";
  }

  const body = (await response.json().catch(() => null)) as {
    mensaje?: unknown;
    errors?: unknown;
  } | null;

  if (typeof body?.mensaje === "string" && body.mensaje.trim()) {
    return body.mensaje;
  }

  const validationError = getFirstValidationError(body?.errors);
  if (validationError) {
    return validationError;
  }

  if (response.status === 400) {
    return "Los datos ingresados no son válidos.";
  }
  if (response.status === 403) {
    return "No tienes permiso para administrar usuarios.";
  }
  if (response.status === 404) {
    return "No se encontró el usuario solicitado.";
  }
  if (response.status === 409) {
    return "La operación no se puede completar con el estado actual del usuario.";
  }
  return fallback;
}

/** Extrae el primer mensaje de una colección de errores de validación de ASP.NET Core. */
function getFirstValidationError(errors: unknown): string | null {
  if (typeof errors !== "object" || errors === null) {
    return null;
  }

  for (const value of Object.values(errors)) {
    if (!Array.isArray(value)) {
      continue;
    }
    const message = value.find(
      (item): item is string => typeof item === "string" && item.trim().length > 0,
    );
    if (message) {
      return message;
    }
  }
  return null;
}
