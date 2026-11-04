using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <summary>
    /// Carga las sub especialidades del catálogo. Los IDs son fijos porque pueden estar referenciados desde otras tablas.
    /// La carga es idempotente: solo inserta los IDs que todavía no existen, así que es segura en bases con datos previos.
    /// </summary>
    public partial class SembrarSubEspecialidades : Migration
    {
        /// <summary>
        /// Inserta las 40 sub especialidades cuyo ID no exista en la tabla.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO [MaestraSubEspecialidades] ([Id], [EspecialidadId], [Nombre], [Descripcion], [Promiscuo], [Codigo], [Activo], [FechaCreacionUtc])
                SELECT datos.[Id], datos.[EspecialidadId], datos.[Nombre], NULL, datos.[Promiscuo], datos.[Codigo], 1, datos.[FechaCreacionUtc]
                FROM (VALUES
                    ('faf7a22d-3171-4403-a087-c25130f7fba3', '24aa4818-58ce-4747-87c4-a35995386af0', N'Ejecución', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('b23ab688-009b-4e2a-bdd0-100cc4ef3bd1', '24aa4818-58ce-4747-87c4-a35995386af0', N'Familia', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('534e8b73-dc3f-4733-8b18-fd6eef870525', '24aa4818-58ce-4747-87c4-a35995386af0', N'Familia - Laboral', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('5d6d7b4a-3572-40bb-b87c-ca5b5d578ad9', '24aa4818-58ce-4747-87c4-a35995386af0', N'Laboral', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('006d5eb1-c546-4cc3-bc65-d04ce7c9cbb1', '24aa4818-58ce-4747-87c4-a35995386af0', N'Pequeñas Causas y Competencia Múltiple', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('88d470e8-cc2a-4708-928e-d31d61b220d9', '24aa4818-58ce-4747-87c4-a35995386af0', N'Restitución de Tierras Enfoque Étnico', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('c54c792d-996c-4d21-b0e5-713f747fc2fe', 'bc967ee8-2c6c-40be-a0ae-7ce883cbd417', N'Ejecución', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('0628a6ce-2ff4-4789-9380-5088234bfc04', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Adolescentes', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('de9483da-916a-4951-960e-972ba6345f73', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Ambulante', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('d4adace4-3633-4745-b9c2-2bd70f565d69', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Ejecución de Sentencias para las Salas de Justicia y Paz', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('ef3baa02-8461-4a8c-8ad4-7bf12f09f091', '24aa4818-58ce-4747-87c4-a35995386af0', N'Con competencia en asuntos Laborales', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('779c3997-7e91-42a1-86f1-19ed82ed293e', '0fc936a3-d68f-4d41-94a2-3323ff470672', N'Pequeñas Causas', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('c04a72f9-1ed5-4127-bec1-7ed65473c1ae', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Con función de control de garantías - BACRIM', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('f0260a67-a468-4033-bb24-314f548f69f3', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Especializado Líderes Sociales', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('4fcbaf54-f8f9-405a-b5ba-b37498c1fb93', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Especializado OIT', 0, NULL, '2026-11-04T19:28:00.0000000'),
                    ('e6c56454-0a00-41ba-b187-e27e2c29cb5a', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Adolescentes Conocimiento', 0, '18', '2026-11-04T19:28:00.0000000'),
                    ('e1fd99f3-a8ca-4e45-9eef-18609daeba34', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Conocimiento', 0, '09', '2026-11-04T19:28:00.0000000'),
                    ('7a3fdc10-7ef9-41b3-bcf1-a481ea8bc931', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Especializado', 0, '07', '2026-11-04T19:28:00.0000000'),
                    ('47b91f07-ce0f-4ee6-a2fe-1cc2d8c90ea6', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Control de Garantías', 0, '88', '2026-11-04T19:28:00.0000000'),
                    ('c4bc4b47-4536-49b6-aea2-982231a5daed', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Adolescentes Control de Garantías', 0, '71', '2026-11-04T19:28:00.0000000'),
                    ('a07f453c-4bc7-429a-bb09-0ab040fbf203', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Ejecución de Penas y Medidas de Seguridad', 0, '87', '2026-11-04T19:28:00.0000000'),
                    ('6187b086-eb57-4429-9344-9d7ad1e585a5', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Especial de Instrucción', 0, '47', '2026-11-04T19:28:00.0000000'),
                    ('1793987b-34c0-4b73-bfa7-39826214b935', NULL, N'Civil - Familia', 1, '13', '2026-11-04T19:28:00.0000000'),
                    ('f98bc5f5-5c03-4fc4-8e4f-ff4ac34da6b1', NULL, N'Civil - Familia Laboral', 1, '14', '2026-11-04T19:28:00.0000000'),
                    ('9f718742-5063-4c1a-93e7-807a1504d6f4', NULL, N'Promiscuo', 1, '89', '2026-11-04T19:28:00.0000000'),
                    ('820d1c82-88da-419d-bd46-9227f399b669', NULL, N'Sala única', 1, '08', '2026-11-04T19:28:00.0000000'),
                    ('0128e035-f117-4fd5-8824-71c5f1350431', NULL, N'Civil - Laboral', 1, '12', '2026-11-04T19:28:00.0000000'),
                    ('c39e600e-4a5a-459a-982d-e1206f4c39ba', '24aa4818-58ce-4747-87c4-a35995386af0', N'Civil', 0, '03', '2026-11-04T19:28:00.0000000'),
                    ('b552b478-3d11-4bdb-8890-ae67a18efc29', '24aa4818-58ce-4747-87c4-a35995386af0', N'Restitución de Tierras', 0, '21', '2026-11-04T19:28:00.0000000'),
                    ('34692fd0-68f4-4d72-b9e5-bb23eb9677a3', 'bc967ee8-2c6c-40be-a0ae-7ce883cbd417', N'Familia', 0, '10', '2026-11-04T19:28:00.0000000'),
                    ('48bb365f-c628-4335-8fac-653148859e34', 'bc967ee8-2c6c-40be-a0ae-7ce883cbd417', N'Promiscuo Familia', 0, '84', '2026-11-04T19:28:00.0000000'),
                    ('40503377-fb07-4e8a-96ee-558a4cc54dc9', '0fc936a3-d68f-4d41-94a2-3323ff470672', N'Laboral', 0, '05', '2026-11-04T19:28:00.0000000'),
                    ('9b970bb3-ec39-4f42-9266-c9dbdf5d6c5b', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Penal', 0, '04', '2026-11-04T19:28:00.0000000'),
                    ('c6b7656c-5c22-438e-967c-a770b49c7dec', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Especial de Primera Instancia', 0, '48', '2026-11-04T19:28:00.0000000'),
                    ('6f06664f-fa5b-4b1e-9e65-b50a6f98ec4f', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Extinción de Dominio', 0, '20', '2026-11-04T19:28:00.0000000'),
                    ('92208ad4-68c4-46be-a440-3336554e24e0', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Mixto', 0, '46', '2026-11-04T19:28:00.0000000'),
                    ('bfcd8fa4-c655-4cd8-85b0-91a760067e3b', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Justicia y Paz', 0, '19', '2026-11-04T19:28:00.0000000'),
                    ('a3e9cde3-b4d1-4fba-897a-ca1d2f9c8e1c', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Justicia y Paz - Conocimiento', 0, '19', '2026-11-04T19:28:00.0000000'),
                    ('c50be01a-231e-4be7-9b75-282044777423', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Justicia y Paz - Control de Garantías', 0, '19', '2026-11-04T19:28:00.0000000'),
                    ('55f7cfe5-3d67-4398-8621-c43ccf1dfddf', 'd7a26090-ae88-47de-b0b6-71d50bf8412e', N'Justicia y Paz - Ejecución de Sentencias', 0, '19', '2026-11-04T19:28:00.0000000')
                ) AS datos ([Id], [EspecialidadId], [Nombre], [Promiscuo], [Codigo], [FechaCreacionUtc])
                WHERE NOT EXISTS (
                    SELECT 1 FROM [MaestraSubEspecialidades] existente WHERE existente.[Id] = datos.[Id]
                );
                """);
        }

        /// <summary>
        /// No elimina las sub especialidades al revertir: sus IDs pueden estar referenciados desde otras tablas,
        /// y borrarlas podría romper esas relaciones o datos que ya dependen de ellas.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
