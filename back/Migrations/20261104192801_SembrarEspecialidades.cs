using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <summary>
    /// Carga las especialidades del catálogo. Los IDs son fijos porque pueden estar referenciados desde otras tablas.
    /// La carga es idempotente: solo inserta los IDs que todavía no existen, así que es segura en bases con datos previos.
    /// </summary>
    public partial class SembrarEspecialidades : Migration
    {
        /// <summary>
        /// Inserta las 4 especialidades cuyo ID no exista en la tabla.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO [MaestraEspecialidades] ([Id], [Nombre], [Descripcion], [Activo], [FechaCreacionUtc])
                SELECT datos.[Id], datos.[Nombre], NULL, 1, datos.[FechaCreacionUtc]
                FROM (VALUES
                    ('24aa4818-58ce-4747-87c4-a35995386af0', N'Civil', '2026-11-04T19:28:00.0000000'),
                    ('bc967ee8-2c6c-40be-a0ae-7ce883cbd417', N'Familia', '2026-11-04T19:28:00.0000000'),
                    ('0fc936a3-d68f-4d41-94a2-3323ff470672', N'Laboral', '2026-11-04T19:28:00.0000000'),
                    ('d7a26090-ae88-47de-b0b6-71d50bf8412e', N'Penal', '2026-11-04T19:28:00.0000000')
                ) AS datos ([Id], [Nombre], [FechaCreacionUtc])
                WHERE NOT EXISTS (
                    SELECT 1 FROM [MaestraEspecialidades] existente WHERE existente.[Id] = datos.[Id]
                );
                """);
        }

        /// <summary>
        /// No elimina las especialidades al revertir: sus IDs pueden estar referenciados desde otras tablas,
        /// y borrarlas podría romper esas relaciones o datos que ya dependen de ellas.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
