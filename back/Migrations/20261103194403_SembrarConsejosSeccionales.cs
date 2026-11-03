using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <summary>
    /// Carga los consejos seccionales del catálogo. Los IDs son fijos porque ya están referenciados desde otras tablas.
    /// La carga es idempotente: solo inserta los IDs que todavía no existen, así que es segura en bases con datos previos.
    /// </summary>
    public partial class SembrarConsejosSeccionales : Migration
    {
        /// <summary>
        /// Inserta los 24 consejos seccionales cuyo ID no exista en la tabla.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO [MaestraConsejosSeccionales] ([Id], [Nombre], [Activo], [FechaCreacionUtc])
                SELECT datos.[Id], datos.[Nombre], 1, '2026-11-03T00:00:00'
                FROM (VALUES
                    ('98ed428d-5eb0-4dea-bf9d-794a0e0b7a5d', N'Antioquia'),
                    ('b7b13f3a-8a57-402d-9387-9ab297803064', N'Atlántico'),
                    ('1e3dcd6c-8fb5-455b-9ca0-980e9c36d329', N'Bogotá'),
                    ('c6fbdd06-346b-48ac-8b71-1e1c91ccb8f3', N'Bolívar'),
                    ('172cf9d8-1fa1-4364-9fb5-b92a2feeadc0', N'Boyacá - Casanare'),
                    ('bcac84e7-59b6-4bad-846b-0372e5f6047a', N'Caldas'),
                    ('2251b672-5c38-4f62-be36-f9c3a110168c', N'Caquetá'),
                    ('3a0c4d80-1478-4e10-9b66-6957681693a6', N'Cauca'),
                    ('db0fe9f4-a3df-4ed3-ad7a-19036c9d80d3', N'Cesar'),
                    ('8659289b-bfed-452a-9954-01b3738945cd', N'Chocó'),
                    ('1197d499-9681-413d-acb1-d7ab218c75f9', N'Córdoba'),
                    ('45ccd664-1fbf-406e-8372-56cd7c0f53d6', N'Cundinamarca'),
                    ('dd6f883e-d05c-4b61-b364-ed19fd4d2ca2', N'Huila'),
                    ('6413fdf3-8a1b-499d-80f5-a53d5f81f42d', N'La Guajira'),
                    ('cbced463-d9a8-4f22-96e9-36b587b7c2f0', N'Magdalena'),
                    ('720113d2-689f-42ce-9529-e76022484a9f', N'Meta'),
                    ('0e82788d-bff0-4194-ba76-eddb9527a374', N'Nariño'),
                    ('9d537cdd-a13d-40bc-92f7-137a5640f172', N'Norte de Santander'),
                    ('f6d73ee6-46ae-4f35-b52d-a9561b15ef3c', N'Quindío'),
                    ('65014653-a5cb-4081-9de1-0b98f881e8ba', N'Risaralda'),
                    ('4afce343-d71d-48af-89b7-25088450778f', N'Santander'),
                    ('28023a96-33af-4950-b81e-ba93031fa5fb', N'Sucre'),
                    ('2c613e87-0dbb-44b2-bc31-15c5d95202f3', N'Tolima'),
                    ('6ff77fa8-2f94-4137-89d1-2c7f360c3f74', N'Valle del Cauca')
                ) AS datos ([Id], [Nombre])
                WHERE NOT EXISTS (
                    SELECT 1 FROM [MaestraConsejosSeccionales] existente WHERE existente.[Id] = datos.[Id]
                );
                """);
        }

        /// <summary>
        /// No elimina los consejos al revertir: sus IDs pueden estar referenciados desde otras tablas,
        /// y borrarlos podría romper esas relaciones o datos que ya dependen de ellos.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
