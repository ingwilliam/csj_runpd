using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarCatalogoRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Renombra el rol administrador existente sin perder sus asignaciones.
            migrationBuilder.Sql(
                """
                UPDATE [Roles]
                SET [Nombre] = N'Administrador del sistema'
                WHERE [Nombre] = N'Administrador';
                """);

            // Elimina el rol estándar anterior: los usuarios nuevos ya no reciben roles por defecto.
            migrationBuilder.Sql(
                """
                DELETE [UsuariosRoles]
                FROM [UsuariosRoles]
                INNER JOIN [Roles] ON [Roles].[Id] = [UsuariosRoles].[RolId]
                WHERE [Roles].[Nombre] = N'Usuario';

                DELETE FROM [Roles] WHERE [Nombre] = N'Usuario';
                """);

            // Inserta los roles del sistema que falten, con identificadores generados por SQL Server.
            migrationBuilder.Sql(
                """
                INSERT INTO [Roles] ([Id], [Nombre], [Descripcion], [Activo], [FechaCreacionUtc])
                SELECT NEWID(), [Rol].[Nombre], [Rol].[Descripcion], 1, SYSUTCDATETIME()
                FROM (VALUES
                    (N'Administrador del sistema', N'Administra usuarios, roles y la configuración general.'),
                    (N'Administrador de la UDAE', N'Administra la información de la UDAE.'),
                    (N'Administrador del Consejo Seccional', N'Administra la información del Consejo Seccional.'),
                    (N'Administrador del Despacho', N'Administra la información del Despacho.'),
                    (N'Consulta y Reportes', N'Consulta la información y genera reportes.')
                ) AS [Rol] ([Nombre], [Descripcion])
                WHERE NOT EXISTS (
                    SELECT 1 FROM [Roles] WHERE [Roles].[Nombre] = [Rol].[Nombre]
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Quita las asignaciones de los roles nuevos antes de eliminarlos.
            migrationBuilder.Sql(
                """
                DELETE [UsuariosRoles]
                FROM [UsuariosRoles]
                INNER JOIN [Roles] ON [Roles].[Id] = [UsuariosRoles].[RolId]
                WHERE [Roles].[Nombre] IN (
                    N'Administrador de la UDAE',
                    N'Administrador del Consejo Seccional',
                    N'Administrador del Despacho',
                    N'Consulta y Reportes'
                );

                DELETE FROM [Roles]
                WHERE [Nombre] IN (
                    N'Administrador de la UDAE',
                    N'Administrador del Consejo Seccional',
                    N'Administrador del Despacho',
                    N'Consulta y Reportes'
                );
                """);

            // Restaura el nombre anterior del rol administrador.
            migrationBuilder.Sql(
                """
                UPDATE [Roles]
                SET [Nombre] = N'Administrador'
                WHERE [Nombre] = N'Administrador del sistema';
                """);

            // Recrea el rol estándar; sus asignaciones anteriores no se pueden recuperar.
            migrationBuilder.Sql(
                """
                INSERT INTO [Roles] ([Id], [Nombre], [Descripcion], [Activo], [FechaCreacionUtc])
                SELECT NEWID(), N'Usuario', N'Acceso estándar al sistema.', 1, SYSUTCDATETIME()
                WHERE NOT EXISTS (SELECT 1 FROM [Roles] WHERE [Nombre] = N'Usuario');
                """);
        }
    }
}
