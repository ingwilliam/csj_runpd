using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <summary>
    /// Carga las entidades del catálogo. Los IDs son fijos porque pueden estar referenciados desde otras tablas.
    /// La carga es idempotente: solo inserta los IDs que todavía no existen, así que es segura en bases con datos previos.
    /// </summary>
    public partial class SembrarEntidades : Migration
    {
        /// <summary>
        /// Inserta las 13 entidades cuyo ID no exista en la tabla.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO [MaestraEntidades] ([Id], [Codigo], [Nombre], [JurisdiccionId], [Activo], [FechaCreacionUtc])
                SELECT datos.[Id], datos.[Codigo], datos.[Nombre], datos.[JurisdiccionId], 1, datos.[FechaCreacionUtc]
                FROM (VALUES
                    ('c464f85f-73b0-409e-926e-accc5db616e4', '02', N'Corte Suprema de Justicia', '1c9b35fc-c46b-4ff1-bb35-f4eb23f8a5ba', '2026-09-11T15:31:07.7798290'),
                    ('fdfe1127-3073-4293-9ad0-70b9383cd124', '22', N'Tribunal Superior', '1c9b35fc-c46b-4ff1-bb35-f4eb23f8a5ba', '2026-09-11T15:31:07.8522650'),
                    ('5d4dfd61-db41-4dc9-9178-a8a21636eb82', '31', N'Juzgado de Circuito', '1c9b35fc-c46b-4ff1-bb35-f4eb23f8a5ba', '2026-09-11T15:31:07.8744820'),
                    ('60f5e136-39ba-45be-ad45-ad4a79f2c24f', '34', N'Juzgado Circuito de Ejecución', '1c9b35fc-c46b-4ff1-bb35-f4eb23f8a5ba', '2026-09-11T15:31:07.8969480'),
                    ('f5d5790e-db1d-47e8-818b-d0e6d79e9169', '40', N'Juzgado Municipal', '1c9b35fc-c46b-4ff1-bb35-f4eb23f8a5ba', '2026-09-11T15:31:07.9189100'),
                    ('08f20bfe-44cb-46e7-9c62-0ee7a18e9b09', '41', N'Juzgado de Pequeñas Causas', '1c9b35fc-c46b-4ff1-bb35-f4eb23f8a5ba', '2026-09-11T15:31:08.0072400'),
                    ('79436d7b-91d5-4c27-986a-da002278896d', '43', N'Juzgado Municipal de Ejecución', '1c9b35fc-c46b-4ff1-bb35-f4eb23f8a5ba', '2026-09-11T15:31:08.0536950'),
                    ('9753e186-0fac-4c11-b0a1-7f060cd6d4c6', '03', N'Consejo de Estado', '19e9314f-03e6-4364-9cb5-8de7a1c5c5b3', '2026-09-11T15:31:08.0536950'),
                    ('22daa351-c564-4560-93a8-05d3259e6d43', '23', N'Tribunal Administrativo', '19e9314f-03e6-4364-9cb5-8de7a1c5c5b3', '2026-09-11T15:31:08.0536950'),
                    ('3c2ce852-6eeb-4305-8372-684aee141e0e', '33', N'Juzgado Administrativo', '19e9314f-03e6-4364-9cb5-8de7a1c5c5b3', '2026-09-11T15:31:08.0536950'),
                    ('34086589-711a-4432-8193-c60e8ab20dff', '04', N'Corte Constitucional', '630f6b38-ca0d-462e-9e5f-fa1c96e0aa98', '2026-09-11T15:31:08.0536950'),
                    ('685383fd-064c-439b-b0cd-4520a3230193', '08', N'Comisión Nacional de Disciplina Judicial', 'a7feea7a-7525-4ea4-8090-01e17c689cb0', '2026-09-11T15:31:08.0536950'),
                    ('18de3958-71b6-4ae5-a27c-16e8e3c85c60', '25', N'Comisión Seccional de Disciplina Judicial', 'a7feea7a-7525-4ea4-8090-01e17c689cb0', '2026-09-11T15:31:08.0536950')
                ) AS datos ([Id], [Codigo], [Nombre], [JurisdiccionId], [FechaCreacionUtc])
                WHERE NOT EXISTS (
                    SELECT 1 FROM [MaestraEntidades] existente WHERE existente.[Id] = datos.[Id]
                );
                """);
        }

        /// <summary>
        /// No elimina las entidades al revertir: sus IDs pueden estar referenciados desde otras tablas,
        /// y borrarlas podría romper esas relaciones o datos que ya dependen de ellas.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
