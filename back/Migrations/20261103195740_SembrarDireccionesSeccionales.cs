using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <summary>
    /// Carga las direcciones seccionales del catálogo. Los IDs son fijos porque pueden estar referenciados desde otras tablas.
    /// La carga es idempotente: solo inserta los IDs que todavía no existen, así que es segura en bases con datos previos.
    /// </summary>
    public partial class SembrarDireccionesSeccionales : Migration
    {
        /// <summary>
        /// Inserta las 23 direcciones seccionales cuyo ID no exista en la tabla.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO [MaestraDireccionesSeccionales] ([Id], [Nombre], [Activo], [FechaCreacionUtc])
                SELECT datos.[Id], datos.[Nombre], 1, '2026-11-03T00:00:00'
                FROM (VALUES
                    ('2e51f99e-2102-4542-9153-b4a3c3228af1', N'Armenia'),
                    ('4a947961-5e75-435e-8803-69d2770b3113', N'Barranquilla'),
                    ('88ed6276-561d-4d4b-84c6-20dae9ef6aab', N'Bogotá'),
                    ('e4764a34-2fc4-488d-b436-930585a84c38', N'Bucaramanga'),
                    ('392e08f9-571b-4cd2-bd33-4316f1f135e1', N'Cali'),
                    ('8737ffbf-996c-47c8-b8bd-4313e9f3f124', N'Cartagena'),
                    ('27ce1eae-2dba-45f5-bebf-c797bb8c6630', N'Cúcuta'),
                    ('8c6da09f-4f3e-47e0-a9cf-21669eb928b5', N'Ibagué'),
                    ('3571ee32-81b9-4b4f-acf1-344cf0b193db', N'Manizales'),
                    ('63a88a22-9cce-47a6-8e1b-146072836894', N'Medellín'),
                    ('4915a06c-6660-44e1-bfbb-953f927fc5e8', N'Montería'),
                    ('efda4e26-269b-4091-b56a-50873c9b4995', N'Neiva'),
                    ('439dba54-bf52-4a99-b72f-613037f5b59b', N'Pasto'),
                    ('0c1c152e-24df-4701-a2a9-bec99679e9c8', N'Pereira'),
                    ('e898856a-7b2d-4037-82df-d3240c29719f', N'Popayán'),
                    ('797a9f2a-f788-4255-a44c-126652969004', N'Quibdó'),
                    ('94f48519-a50d-477a-b49a-32ae2e028aac', N'Riohacha'),
                    ('9e23aa7c-c11b-4934-98c7-441ccfbbb2f7', N'Santa Marta'),
                    ('4add7a84-e79c-4e6f-98f0-4eddb72f51cb', N'Sincelejo'),
                    ('82b7b3a2-6b71-473f-aff7-5069ceb0b14d', N'Tunja'),
                    ('fcbe3ea5-80f0-49b9-920c-bcb9ccd89673', N'Valledupar'),
                    ('db5904e4-8741-4402-ab9b-69fb57e6b16a', N'Villavicencio'),
                    ('930da74f-4c0f-41e1-bbb5-fffa1df76d03', N'Cundinamarca')
                ) AS datos ([Id], [Nombre])
                WHERE NOT EXISTS (
                    SELECT 1 FROM [MaestraDireccionesSeccionales] existente WHERE existente.[Id] = datos.[Id]
                );
                """);
        }

        /// <summary>
        /// No elimina las direcciones al revertir: sus IDs pueden estar referenciados desde otras tablas,
        /// y borrarlas podría romper esas relaciones o datos que ya dependen de ellas.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
