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

`GET /api/usuarios` acepta `pagina` (1 por defecto), `tamano` (20 por defecto, máximo 100), `busqueda` (máximo 100 caracteres) y `estado` (`todos`, `activos` o `inactivos`). Devuelve `items`, `pagina`, `tamano`, `total`, `totalPaginas`, `totalUsuarios`, `totalActivos` y `totalInactivos`. `total` corresponde al resultado filtrado; los totales globales no aplican filtros. La búsqueda recorre nombres, correo y roles sin distinguir mayúsculas ni tildes, y el listado se ordena del más reciente al más antiguo. Los parámetros inválidos responden `400`.

El resto de endpoints de `/api/usuarios` son `GET/{id}`, `POST`, `PUT/{id}`, `PUT/{id}/roles` y `PATCH /{id}/activo/{true|false}`. El `id` es un UUID (`Guid`). El JSON de un usuario usa `id`, `primerNombre`, `segundoNombre`, `primerApellido`, `segundoApellido`, `correoElectronico`, `activo`, `fechaCreacionUtc` y `roles`; `segundoNombre` y `segundoApellido` pueden ser `null`. `POST` crea al usuario activo y exige `contrasena`; la contraseña se guarda como hash en la columna `Contrasena` y nunca se devuelve. `PATCH /{id}/activo/{true|false}` es la única forma de activar o desactivar, y conserva el registro.

Un usuario puede tener varios roles. `POST` y `PUT/{id}` aceptan `roles` opcional con GUIDs de roles activos: si un rol no existe o está inactivo responden `400` y no modifican nada. En `POST`, omitir `roles` crea el usuario sin roles. En `PUT/{id}`, omitir `roles` deja los roles igual, `[]` quita todos y una lista los reemplaza. Para cambiar solo los roles sin reenviar los datos del usuario usa `PUT /api/usuarios/{id}/roles` con el cuerpo `{"roles":["<guid>","<guid>"]}`. Ninguna de estas operaciones puede dejar el sistema sin un usuario activo con el rol `Administrador del sistema`.

Los roles se administran con `GET /api/roles`, `POST /api/roles` y `PATCH /api/roles/{id}/activo/{true|false}`. El catálogo del sistema es `Administrador del sistema`, `Administrador de la UDAE`, `Administrador del Consejo Seccional`, `Administrador del Despacho`, `Consulta` y `Reportes`; estos roles no pueden desactivarse. Un rol personalizado solo se desactiva si no tiene usuarios asignados.

Todas las tablas de negocio incluyen `Activo` y `FechaCreacionUtc`; la tabla puente `UsuariosRoles` no los necesita porque asignar o quitar un rol es un `INSERT` o `DELETE`.

## Tablas maestras

Las tablas maestras son catálogos de solo consulta para usuarios autenticados. Cada una vive en su carpeta dentro de `Features/TablasMaestras/`.

`MaestraConsejosSeccionales` guarda los consejos seccionales con `Id`, `Nombre`, `Activo` y `FechaCreacionUtc`. Al crearse, el registro queda activo. La migración `SembrarConsejosSeccionales` carga los 24 consejos seccionales con IDs fijos, porque otras tablas los referencian. La carga solo inserta los IDs que no existen, así que es segura en bases con datos previos. Al revertirla no se borran datos.

`GET /api/maestras/consejos-seccionales` requiere un JWT válido (cualquier rol) y devuelve solo los consejos activos, ordenados por nombre, como lista simple: `[{ id, nombre, activo, fechaCreacionUtc }]`. Sin token responde `401`.

`MaestraDepartamentos` guarda los departamentos con `Id`, `Nombre`, `CodigoDivpola` (DIVIPOLA, único y guardado como texto para conservar los ceros iniciales), `Activo` y `FechaCreacionUtc`. La migración `SembrarDepartamentos` carga los 33 registros (32 departamentos y Bogotá, D.C.) con IDs fijos y la misma regla de inserción segura. `GET /api/maestras/departamentos` requiere JWT (cualquier rol) y devuelve solo los activos, como lista simple con `codigoDivpola`.

`MaestraMunicipios` guarda los municipios con `Id`, `Tipo`, `Pdet`, `Nombre`, `CodigoDivpola` (único), `DepartamentoId` (obligatorio), `ConsejoSeccionalId` y `DireccionSeccionalId` (opcionales), `Activo` y `FechaCreacionUtc`. La migración `SembrarMunicipios` carga los 1122 municipios con IDs fijos y la misma regla de inserción segura. `GET /api/maestras/municipios?departamentoId={guid}` requiere JWT y devuelve los municipios activos del departamento, ordenados por nombre. Sin `departamentoId` responde `400`.

`MaestraJurisdicciones` guarda las jurisdicciones con `Id`, `Nombre`, `Funcion`, `Activo` y `FechaCreacionUtc`. La migración `SembrarJurisdicciones` carga las 5 jurisdicciones con IDs fijos y la misma regla de inserción segura. `GET /api/maestras/jurisdicciones` requiere JWT (cualquier rol) y devuelve solo las activas, ordenadas por nombre.

`MaestraEntidades` guarda las entidades con `Id`, `Codigo`, `Nombre`, `JurisdiccionId` (obligatorio), `Activo` y `FechaCreacionUtc`. `JurisdiccionId` referencia a `MaestraJurisdicciones` con borrado restringido. La migración `SembrarEntidades` carga las 13 entidades con IDs fijos y la misma regla de inserción segura. `GET /api/maestras/entidades` requiere JWT (cualquier rol) y devuelve solo las activas, ordenadas por nombre.

`MaestraDireccionesSeccionales` sigue el mismo patrón. La migración `SembrarDireccionesSeccionales` carga las 23 direcciones seccionales con IDs fijos, con la misma regla de inserción segura y sin borrado al revertir. `GET /api/maestras/direcciones-seccionales` requiere JWT (cualquier rol) y devuelve solo las activas, como lista simple.

`MaestraEspecialidades` guarda las especialidades con `Id`, `Nombre`, `Descripcion` (opcional), `Activo` y `FechaCreacionUtc`. La migración `SembrarEspecialidades` carga las 4 especialidades (Civil, Familia, Laboral y Penal) con IDs fijos y la misma regla de inserción segura. `GET /api/maestras/especialidades` requiere JWT (cualquier rol) y devuelve solo las activas, ordenadas por nombre.

`MaestraSubEspecialidades` guarda las sub especialidades con `Id`, `EspecialidadId` (opcional, referencia a `MaestraEspecialidades` con borrado restringido), `Nombre`, `Descripcion` (opcional), `Promiscuo`, `Codigo` (opcional y no único), `Activo` y `FechaCreacionUtc`. La migración `SembrarSubEspecialidades` carga las 40 sub especialidades con IDs fijos y la misma regla de inserción segura. `GET /api/maestras/sub-especialidades` requiere JWT (cualquier rol) y devuelve solo las activas, ordenadas por nombre.

`MaestraDistritos` guarda los distritos con `Id`, `Nombre`, `JurisdiccionId` (obligatorio, referencia a `MaestraJurisdicciones` con borrado restringido), `SubEspecialidadId` (opcional, referencia a `MaestraSubEspecialidades` con borrado que deja la relación en nulo), `Activo` y `FechaCreacionUtc`. La migración `SembrarDistritos` carga los 169 distritos con IDs fijos y la misma regla de inserción segura. `GET /api/maestras/distritos` requiere JWT (cualquier rol) y devuelve solo los activos, ordenados por nombre.

`MaestraCircuitos` guarda los circuitos con `Id`, `Nombre`, `DistritoId` (obligatorio, referencia a `MaestraDistritos` con borrado restringido), `SubEspecialidadId` (opcional, referencia a `MaestraSubEspecialidades` con borrado que deja la relación en nulo), `Activo` y `FechaCreacionUtc`. La migración `SembrarCircuitos` carga los 745 circuitos con IDs fijos y la misma regla de inserción segura. `GET /api/maestras/circuitos` requiere JWT (cualquier rol) y devuelve solo los activos, ordenados por nombre.

## Despachos

Los despachos son un módulo propio en `Features/Despachos/`, con CRUD y sin siembra de datos. La tabla `Despachos` tiene un índice único en `Codigo`, y sus relaciones apuntan a las tablas maestras: `MunicipioId` y `JurisdiccionId` son obligatorias; `EspecialidadId`, `SubEspecialidadId`, `DistritoId`, `CircuitoId`, `DistritoSubEspecialidadId`, `CircuitoSubEspecialidadId` y `EntidadId` son opcionales. `SubEspecialidadId` usa borrado que deja la relación en nulo; el resto restringe el borrado.

El estado (`CREADO`, `MODIFICADO`, `TRASLADADO` o `SUPRIMIDO`) lo envía el front y el backend solo valida que sea uno de esos valores.

Endpoints:
- `GET /api/despachos`: lista paginada (`pagina`, `tamano`, `busqueda` por código o nombre, `estado` con `todos`, `activos` o `inactivos`). Roles: `Administrador de la UDAE`, `Consulta` y `Administrador del sistema`.
- `GET /api/despachos/{id}`: mismos roles de lectura.
- `POST /api/despachos`: crea un despacho (`201`). Solo `Administrador de la UDAE`.
- `PUT /api/despachos/{id}`: actualiza los datos. Solo `Administrador de la UDAE`.
- `PATCH /api/despachos/{id}/activo/{true|false}`: cambia solo `Activo`. Solo `Administrador de la UDAE`.

Respuestas de error: `400` si una relación no existe o está inactiva, `404` si el despacho no existe y `409` si el código ya pertenece a otro despacho.
