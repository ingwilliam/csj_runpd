using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <summary>
    /// Carga las jurisdicciones del catálogo. Los IDs son fijos porque pueden estar referenciados desde otras tablas.
    /// La carga es idempotente: solo inserta los IDs que todavía no existen, así que es segura en bases con datos previos.
    /// </summary>
    public partial class SembrarJurisdicciones : Migration
    {
        /// <summary>
        /// Inserta las 5 jurisdicciones cuyo ID no exista en la tabla.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO [MaestraJurisdicciones] ([Id], [Nombre], [Funcion], [Activo], [FechaCreacionUtc])
                SELECT datos.[Id], datos.[Nombre], datos.[Funcion], 1, datos.[FechaCreacionUtc]
                FROM (VALUES
                    ('1c9b35fc-c46b-4ff1-bb35-f4eb23f8a5ba', N'Jurisdicción Ordinaria', 0, '2026-08-13T15:01:46.8816690'),
                    ('19e9314f-03e6-4364-9cb5-8de7a1c5c5b3', N'Jurisdicción de lo Contencioso Administrativo', 0, '2026-08-13T15:01:46.8816690'),
                    ('630f6b38-ca0d-462e-9e5f-fa1c96e0aa98', N'Jurisdicción Constitucional', 0, '2026-08-13T15:01:46.8816690'),
                    ('51a7cb06-a1ee-4d69-8923-dd1d55561ddb', N'Jurisdicción Agraria y Rural', 0, '2026-08-13T15:01:46.8816690'),
                    ('a7feea7a-7525-4ea4-8090-01e17c689cb0', N'Función Jurisdicción Disciplinaria', 1, '2026-09-01T16:58:09.1075990')
                ) AS datos ([Id], [Nombre], [Funcion], [FechaCreacionUtc])
                WHERE NOT EXISTS (
                    SELECT 1 FROM [MaestraJurisdicciones] existente WHERE existente.[Id] = datos.[Id]
                );
                """);
        }

        /// <summary>
        /// No elimina las jurisdicciones al revertir: sus IDs pueden estar referenciados desde otras tablas,
        /// y borrarlas podría romper esas relaciones o datos que ya dependen de ellas.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
