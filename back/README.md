# API Runpd

API de administración de usuarios y roles con ASP.NET Core, Entity Framework Core, SQL Server y autenticación JWT.

## Estructura

- `Features/Usuarios`: endpoints, lógica y contratos de usuarios.
- `Features/Roles`: endpoints, lógica y contratos de roles.
- `Infrastructure/Persistence`: contexto y mapeo a SQL Server.
- `Migrations`: migraciones de la base de datos.

## Preparar el entorno local

El archivo `.env` local no se versiona. Créalo desde la plantilla y completa `DB_PASSWORD` con la contraseña local de SQL Server:

```powershell
Copy-Item .env.example .env
```

Si una clave contiene `$` o `#`, escríbela entre comillas simples en `.env`. Configura `JWT_SECRET` con una clave aleatoria de al menos 32 bytes; usa un valor diferente por entorno. La plantilla `.env.example` no contiene secretos.

En desarrollo, la API carga `.env` automáticamente al ejecutar `dotnet run`. En esa ejecución `DB_HOST=127.0.0.1`; en el contenedor, Compose lo cambia a `sqlserver`.

## Migraciones de base de datos

En desarrollo, `dotnet run` aplica automáticamente las migraciones pendientes al iniciar. Para aplicarlas manualmente, con SQL Server en ejecución, usa:

```powershell
dotnet tool restore
dotnet ef database update
```

Las migraciones crean las tablas `Usuarios`, `Roles` y `UsuariosRoles`. Para cambios futuros, genera una nueva migración con `dotnet ef migrations add NombreDescriptivo`. En producción, aplica las migraciones como un paso controlado del despliegue; el contenedor no las ejecuta automáticamente.

## Roles y administrador inicial

Al iniciar, la API verifica que existan los seis roles del sistema (`Administrador del sistema`, `Administrador de la UDAE`, `Administrador del Consejo Seccional`, `Administrador del Despacho`, `Consulta` y `Reportes`) y reactiva los que hayan sido desactivados manualmente.

Si no existe ningún usuario activo con el rol `Administrador del sistema`, la API usa `ADMIN_EMAIL`. Si el usuario ya existe, le garantiza el rol, lo activa si hace falta y le asigna `ADMIN_PASSWORD` (mínimo 8 caracteres) solo si no tiene contraseña; nunca la sobrescribe. Si el usuario no existe, lo crea con `ADMIN_PASSWORD` y los nombres `ADMIN_PRIMER_NOMBRE`, `ADMIN_SEGUNDO_NOMBRE`, `ADMIN_PRIMER_APELLIDO` y `ADMIN_SEGUNDO_APELLIDO` (los dos últimos opcionales). Si `ADMIN_EMAIL` no está configurado y no hay administrador activo, deja una advertencia en el log.

En producción, configura estas variables desde el gestor de secretos. No guardes credenciales en archivos versionados.

## Ejecutar directamente en el equipo

```powershell
dotnet run --project .\Runpd.Api.csproj
```

La API queda disponible en `http://localhost:5159`.

## Ejecutar en Docker

El contenedor de SQL Server debe estar activo en la red Docker `sql_server_default`. Desde esta carpeta, ejecuta:

```powershell
docker compose up --build -d
```

Compose toma las credenciales de `.env` y configura el host como `sqlserver`. El `.env` no se copia a la imagen ni debe desplegarse en producción.

En producción, configura las variables `DB_*` desde el gestor de secretos del entorno y usa un usuario SQL dedicado con permisos limitados, no `sa`.

`POST /api/autenticacion/login` recibe `correoElectronico` y `contrasena`. Si las credenciales pertenecen a un usuario activo, devuelve `accessToken`, `tokenType` (`Bearer`) y `expiraUtc`. Las credenciales inválidas o los usuarios inactivos reciben `401`.

Todos los endpoints de `/api/usuarios` y `/api/roles` requieren un JWT válido cuyo usuario tenga el rol `Administrador del sistema`. Envía el token en el encabezado `Authorization: Bearer <accessToken>`. Las rutas devuelven `401` si falta o no es válido el token y `403` si el usuario autenticado carece del rol requerido. Los tokens duran 60 minutos.

La API expone `GET`, `GET/{id}`, `POST`, `PUT/{id}` y `PATCH /{id}/activo/{true|false}` en `/api/usuarios`. El `id` es un UUID (`Guid`). El JSON de respuesta usa `id`, `primerNombre`, `segundoNombre`, `primerApellido`, `segundoApellido`, `correoElectronico`, `activo`, `fechaCreacionUtc` y `roles`; `segundoNombre` y `segundoApellido` pueden ser `null`. `POST` crea al usuario activo y exige `contrasena`; la contraseña se guarda como hash en la columna `Contrasena` y nunca se devuelve. `PATCH /{id}/activo/{true|false}` es la única forma de activar o desactivar, y conserva el registro.

Un usuario puede tener varios roles. `POST` y `PUT/{id}` aceptan `roles` opcional con GUIDs de roles activos: si un rol no existe o está inactivo responden `400` y no modifican nada. En `POST`, omitir `roles` crea el usuario sin roles. En `PUT/{id}`, omitir `roles` deja los roles igual, `[]` quita todos y una lista los reemplaza. Para cambiar solo los roles sin reenviar los datos del usuario usa `PUT /api/usuarios/{id}/roles` con el cuerpo `{"roles":["<guid>","<guid>"]}`. Ninguna de estas operaciones puede dejar el sistema sin un usuario activo con el rol `Administrador del sistema`.

Los roles se administran con `GET /api/roles`, `POST /api/roles` y `PATCH /api/roles/{id}/activo/{true|false}`. El catálogo del sistema es `Administrador del sistema`, `Administrador de la UDAE`, `Administrador del Consejo Seccional`, `Administrador del Despacho`, `Consulta` y `Reportes`; estos roles no pueden desactivarse. Un rol personalizado solo se desactiva si no tiene usuarios asignados.

Todas las tablas de negocio incluyen `Activo` y `FechaCreacionUtc`; la tabla puente `UsuariosRoles` no los necesita porque asignar o quitar un rol es un `INSERT` o `DELETE`.
