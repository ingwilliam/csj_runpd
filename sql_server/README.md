# SQL Server 2022 para desarrollo local

Este entorno ejecuta SQL Server 2022 Developer en Docker. Está pensado para desarrollo local, no para producción.

## Requisitos

- Docker Desktop con Docker Compose.
- Una contraseña para el usuario administrador `sa` que cumpla la política de SQL Server: mínimo 8 caracteres y al menos 3 de estos grupos: mayúsculas, minúsculas, números y símbolos.

## Iniciar

Desde PowerShell, dentro de esta carpeta:

```powershell
Copy-Item .env.example .env
```

Edita `.env` y define `MSSQL_SA_PASSWORD` con una contraseña segura. El archivo `.env` está excluido de Git.

Después, inicia el servicio:

```powershell
docker compose up -d
docker compose ps
```

Para consultar los registros de inicio:

```powershell
docker compose logs -f sqlserver
```

## Conectar desde DBeaver

1. Crea una conexión nueva y selecciona el controlador **SQL Server**.
2. Completa estos datos:
   - Servidor: `localhost`
   - Puerto: `1433`
   - Autenticación: SQL Server
   - Usuario: `sa`
   - Contraseña: el valor configurado en `.env`
   - Base de datos: `master` (puedes dejar el valor predeterminado)
3. Si DBeaver informa un error de certificado TLS, habilita **Trust server certificate** para esta conexión local.

El puerto está enlazado a `127.0.0.1`, por lo que solo acepta conexiones desde el equipo local. Los datos se conservan en el volumen Docker `sqlserver_data`.

## Detener y eliminar

Detener y eliminar el contenedor sin borrar los datos:

```powershell
docker compose down
```

Eliminar también el volumen y todos los datos de SQL Server:

```powershell
docker compose down --volumes
```
