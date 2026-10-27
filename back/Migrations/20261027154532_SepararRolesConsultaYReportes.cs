using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <inheritdoc />
    public partial class SepararRolesConsultaYReportes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Conserva las asignaciones existentes al convertir el rol combinado en el rol de consulta.
            migrationBuilder.Sql(
                """
                UPDATE [Roles]
                SET [Nombre] = N'Consulta',
                    [Descripcion] = N'Consulta la información del sistema.'
                WHERE [Nombre] = N'Consulta y Reportes';
                """);

            // Agrega el rol de reportes como permiso independiente.
            migrationBuilder.Sql(
                """
                INSERT INTO [Roles] ([Id], [Nombre], [Descripcion], [Activo], [FechaCreacionUtc])
                SELECT NEWID(), N'Reportes', N'Genera reportes del sistema.', 1, SYSUTCDATETIME()
                WHERE NOT EXISTS (SELECT 1 FROM [Roles] WHERE [Nombre] = N'Reportes');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Quita las asignaciones de reportes antes de eliminar el rol.
            migrationBuilder.Sql(
                """
                DELETE [UsuariosRoles]
                FROM [UsuariosRoles]
                INNER JOIN [Roles] ON [Roles].[Id] = [UsuariosRoles].[RolId]
                WHERE [Roles].[Nombre] = N'Reportes';

                DELETE FROM [Roles] WHERE [Nombre] = N'Reportes';
                """);

            // Restaura el rol combinado con su descripción anterior.
            migrationBuilder.Sql(
                """
                UPDATE [Roles]
                SET [Nombre] = N'Consulta y Reportes',
                    [Descripcion] = N'Consulta la información y genera reportes.'
                WHERE [Nombre] = N'Consulta';
                """);
        }
    }
}
