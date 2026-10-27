using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <inheritdoc />
    public partial class SepararNombresYAgregarContrasena : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Usuarios",
                newName: "PrimerNombre");

            migrationBuilder.RenameColumn(
                name: "Apellido",
                table: "Usuarios",
                newName: "PrimerApellido");

            migrationBuilder.AddColumn<string>(
                name: "Contrasena",
                table: "Usuarios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SegundoApellido",
                table: "Usuarios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SegundoNombre",
                table: "Usuarios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // Conserva los datos existentes: separa el nombre y el apellido por el primer espacio.
            migrationBuilder.Sql(
                """
                UPDATE [Usuarios]
                SET [SegundoApellido] = NULLIF(LTRIM(RTRIM(SUBSTRING([PrimerApellido], CHARINDEX(' ', [PrimerApellido]) + 1, 100))), '')
                WHERE CHARINDEX(' ', [PrimerApellido]) > 0;

                UPDATE [Usuarios]
                SET [PrimerApellido] = LTRIM(RTRIM(LEFT([PrimerApellido], CHARINDEX(' ', [PrimerApellido] + ' ') - 1)));

                UPDATE [Usuarios]
                SET [SegundoNombre] = NULLIF(LTRIM(RTRIM(SUBSTRING([PrimerNombre], CHARINDEX(' ', [PrimerNombre]) + 1, 100))), '')
                WHERE CHARINDEX(' ', [PrimerNombre]) > 0;

                UPDATE [Usuarios]
                SET [PrimerNombre] = LTRIM(RTRIM(LEFT([PrimerNombre], CHARINDEX(' ', [PrimerNombre] + ' ') - 1)));
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reconstruye el nombre y el apellido completos antes de eliminar las columnas nuevas.
            migrationBuilder.Sql(
                """
                UPDATE [Usuarios]
                SET [PrimerNombre] = LEFT(LTRIM(RTRIM([PrimerNombre] + CASE WHEN [SegundoNombre] IS NULL OR [SegundoNombre] = '' THEN '' ELSE ' ' + [SegundoNombre] END)), 100);

                UPDATE [Usuarios]
                SET [PrimerApellido] = LEFT(LTRIM(RTRIM([PrimerApellido] + CASE WHEN [SegundoApellido] IS NULL OR [SegundoApellido] = '' THEN '' ELSE ' ' + [SegundoApellido] END)), 100);
                """);

            migrationBuilder.DropColumn(
                name: "Contrasena",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "SegundoApellido",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "SegundoNombre",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "PrimerNombre",
                table: "Usuarios",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "PrimerApellido",
                table: "Usuarios",
                newName: "Apellido");
        }
    }
}
