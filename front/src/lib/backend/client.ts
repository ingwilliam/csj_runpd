import type {
  ActualizarPerfilRequest,
  CambiarContrasenaRequest,
  LoginRequest,
  LoginResponse,
  Perfil,
  Usuario,
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
 * Obtiene la lista de usuarios. Requiere el rol "Administrador del sistema" en el token.
 * @param accessToken Token de la sesión, enviado como Bearer.
 * @returns Lista de usuarios, incluidos los desactivados.
 * @throws {BackendError} Si el backend responde con un error HTTP (401, 403, etc.).
 * @throws {Error} Si falta la variable de entorno API_URL o no hay conexión con el backend.
 */
export async function listUsuariosInBackend(accessToken: string): Promise<Usuario[]> {
  const response = await fetch(`${getApiUrl()}/usuarios`, {
    headers: {
      Accept: "application/json",
      Authorization: `Bearer ${accessToken}`,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new BackendError(response.status, getUsuariosErrorMessage(response.status));
  }

  return (await response.json()) as Usuario[];
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

/** Traduce el código HTTP de un error al consultar usuarios a un mensaje para el usuario. */
function getUsuariosErrorMessage(status: number): string {
  if (status === 401) {
    return "Tu sesión expiró. Vuelve a iniciar sesión.";
  }
  if (status === 403) {
    return "No tienes permiso para administrar usuarios.";
  }
  return "No se pudieron cargar los usuarios. Intenta más tarde.";
}
