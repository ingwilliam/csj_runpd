using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <summary>
    /// Carga los departamentos del catálogo. Los IDs son fijos porque pueden estar referenciados desde otras tablas.
    /// La carga es idempotente: solo inserta los IDs que todavía no existen, así que es segura en bases con datos previos.
    /// </summary>
    public partial class SembrarDepartamentos : Migration
    {
        /// <summary>
        /// Inserta los 33 departamentos cuyo ID no exista en la tabla.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO [MaestraDepartamentos] ([Id], [Nombre], [CodigoDivpola], [Activo], [FechaCreacionUtc])
                SELECT datos.[Id], datos.[Nombre], datos.[CodigoDivpola], 1, '2026-11-03T00:00:00'
                FROM (VALUES
                    ('a7dbce38-769a-43f5-9be7-10d1c034b6f2', N'Amazonas', N'91'),
                    ('89fb1d48-13a9-4cd6-bbb4-983e3aa70510', N'Antioquia', N'05'),
                    ('76e43b5f-e4b6-4d26-8d76-d852e72b7d0e', N'Arauca', N'81'),
                    ('a29e6f8d-818c-43e8-b892-2b73f732ce80', N'Atlántico', N'08'),
                    ('d58b31d5-7748-4c2d-b67d-bc3ba727ccd6', N'Bogotá, D.C.', N'11'),
                    ('3ac73ffe-ece4-48ff-858c-cb1c5a6dcaa6', N'Bolívar', N'13'),
                    ('2db31ce7-a5b8-4099-b290-f6a464ea15c0', N'Boyacá', N'15'),
                    ('ae00ed53-4083-4ce1-b90b-d7b26da6eed3', N'Caldas', N'17'),
                    ('8e16774a-f493-4dfc-9c03-abbfbfb2d35c', N'Caquetá', N'18'),
                    ('6df6b2d1-2dec-44cd-a3f0-4c83fa40a23c', N'Casanare', N'85'),
                    ('a0640e9d-d080-4281-8564-5cd28ea1eae4', N'Cauca', N'19'),
                    ('70e96524-ce8a-4c29-8e11-52e3b5a097fa', N'Cesar', N'20'),
                    ('adc21e09-9eec-4628-9a77-a57d72b87b5f', N'Chocó', N'27'),
                    ('ebc20cf2-193c-40cc-a5ba-97b2b48ec17e', N'Córdoba', N'23'),
                    ('9d2612be-dc38-41d9-ba5a-1d584963051e', N'Cundinamarca', N'25'),
                    ('d836b233-23e1-4231-8a8e-9c4d36d60b5b', N'Guainía', N'94'),
                    ('47fe7205-23b4-4462-acce-c64f60049eec', N'Guaviare', N'95'),
                    ('3e939035-9606-4eba-886a-e051fbb83a4a', N'Huila', N'41'),
                    ('57c1b631-4adc-46af-9a45-bd060214223b', N'La Guajira', N'44'),
                    ('6a8b0f44-a3ee-422b-ad34-e3ea6f17136b', N'Magdalena', N'47'),
                    ('005bea36-f193-4bbe-849a-f8c63299e5c0', N'Meta', N'50'),
                    ('b23941d7-a617-4966-bb8d-ee587f6c738a', N'Nariño', N'52'),
                    ('0ea6cd25-121e-4885-8cd7-62549c411776', N'Norte de Santander', N'54'),
                    ('b347f88a-73e1-4044-a502-633510105b2f', N'Putumayo', N'86'),
                    ('a173bdcb-66f8-4952-b3f4-febf4166adeb', N'Quindío', N'63'),
                    ('9666ff9d-0369-4eee-afaf-274d32109246', N'Risaralda', N'66'),
                    ('e17e9616-cef6-4280-828a-aca168488ef8', N'San Andrés y Providencia', N'88'),
                    ('657266b7-4d9e-4eab-913e-631a171d074d', N'Santander', N'68'),
                    ('70459427-b74f-461a-8c9d-b35b6775095e', N'Sucre', N'70'),
                    ('d9ed6f7e-89b3-45a3-824e-68b34e99ce8a', N'Tolima', N'73'),
                    ('319df973-ce51-42b3-aae1-895f6aa74bc1', N'Valle del Cauca', N'76'),
                    ('bef4e37f-8657-4ddc-93e8-5b549cf6ad45', N'Vaupés', N'97'),
                    ('90761230-2640-40ef-8da8-9a6533446c4c', N'Vichada', N'99')
                ) AS datos ([Id], [Nombre], [CodigoDivpola])
                WHERE NOT EXISTS (
                    SELECT 1 FROM [MaestraDepartamentos] existente WHERE existente.[Id] = datos.[Id]
                );
                """);
        }

        /// <summary>
        /// No elimina los departamentos al revertir: sus IDs pueden estar referenciados desde otras tablas,
        /// y borrarlos podría romper esas relaciones o datos que ya dependen de ellos.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
