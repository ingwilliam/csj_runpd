using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <summary>
    /// Carga de municipios de prueba (10 registros). Los IDs son fijos porque pueden estar referenciados desde otras tablas.
    /// La carga es idempotente: solo inserta los IDs que todavía no existen, así que es segura en bases con datos previos.
    /// </summary>
    public partial class SembrarMunicipios : Migration
    {
        /// <summary>
        /// Inserta los municipios cuyo ID no exista en la tabla.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO [MaestraMunicipios] ([Id], [Tipo], [Pdet], [Nombre], [CodigoDivpola], [DepartamentoId], [ConsejoSeccionalId], [DireccionSeccionalId], [Activo], [FechaCreacionUtc])
                SELECT datos.[Id], datos.[Tipo], datos.[Pdet], datos.[Nombre], datos.[CodigoDivpola], datos.[DepartamentoId], datos.[ConsejoSeccionalId], datos.[DireccionSeccionalId], 1, '2026-11-03T00:00:00'
                FROM (VALUES
                    ('d95be761-ee7b-4603-87db-59e82bdc9c04', N'Municipio', 1, N'Anorí', '05040', '89fb1d48-13a9-4cd6-bbb4-983e3aa70510', '98ed428d-5eb0-4dea-bf9d-794a0e0b7a5d', '63a88a22-9cce-47a6-8e1b-146072836894'),
                    ('215b893a-d204-4d81-9c14-f63cc309c4b5', N'Municipio', 1, N'Medio Atrato', '27425', 'adc21e09-9eec-4628-9a77-a57d72b87b5f', '8659289b-bfed-452a-9954-01b3738945cd', '797a9f2a-f788-4255-a44c-126652969004'),
                    ('9e887b74-36c1-456e-9455-51b90f5d4315', N'Municipio', 1, N'Hacarí', '54344', '0ea6cd25-121e-4885-8cd7-62549c411776', '9d537cdd-a13d-40bc-92f7-137a5640f172', '27ce1eae-2dba-45f5-bebf-c797bb8c6630'),
                    ('28f1978a-85d4-4115-ad06-c8e06fb57c98', N'Municipio', 1, N'Puerto Leguízamo', '86573', 'b347f88a-73e1-4044-a502-633510105b2f', '0e82788d-bff0-4194-ba76-eddb9527a374', '439dba54-bf52-4a99-b72f-613037f5b59b'),
                    ('f160e0eb-2f89-415d-8150-162a17a36c38', N'Municipio', 1, N'Apartadó', '05045', '89fb1d48-13a9-4cd6-bbb4-983e3aa70510', '98ed428d-5eb0-4dea-bf9d-794a0e0b7a5d', '63a88a22-9cce-47a6-8e1b-146072836894'),
                    ('e1f7d51c-1ce7-454b-90b8-c2b0cf4f02f8', N'Municipio', 1, N'San Vicente Del Caguán', '18753', '8e16774a-f493-4dfc-9c03-abbfbfb2d35c', '2251b672-5c38-4f62-be36-f9c3a110168c', 'efda4e26-269b-4091-b56a-50873c9b4995'),
                    ('155d59c3-92b1-4dfa-8b9e-9a1c081c9b88', N'Municipio', 1, N'Buenos Aires', '19110', 'a0640e9d-d080-4281-8564-5cd28ea1eae4', '3a0c4d80-1478-4e10-9b66-6957681693a6', 'e898856a-7b2d-4037-82df-d3240c29719f'),
                    ('ab10c021-9c30-4f3c-8ec5-da3be86f5b82', N'Municipio', 1, N'Valledupar', '20001', '70e96524-ce8a-4c29-8e11-52e3b5a097fa', 'db0fe9f4-a3df-4ed3-ad7a-19036c9d80d3', 'fcbe3ea5-80f0-49b9-920c-bcb9ccd89673'),
                    ('b1b41036-3c9e-4f44-aca0-73d1a15eee52', N'Municipio', 1, N'Olaya Herrera', '52490', 'b23941d7-a617-4966-bb8d-ee587f6c738a', '0e82788d-bff0-4194-ba76-eddb9527a374', '439dba54-bf52-4a99-b72f-613037f5b59b'),
                    ('5440a1b3-10bc-46a7-a9de-23d2274f0504', N'Municipio', 1, N'Carepa', '05147', '89fb1d48-13a9-4cd6-bbb4-983e3aa70510', '98ed428d-5eb0-4dea-bf9d-794a0e0b7a5d', '63a88a22-9cce-47a6-8e1b-146072836894')
                ) AS datos ([Id], [Tipo], [Pdet], [Nombre], [CodigoDivpola], [DepartamentoId], [ConsejoSeccionalId], [DireccionSeccionalId])
                WHERE NOT EXISTS (
                    SELECT 1 FROM [MaestraMunicipios] existente WHERE existente.[Id] = datos.[Id]
                );
                """);
        }

        /// <summary>
        /// No elimina los municipios al revertir: sus IDs pueden estar referenciados desde otras tablas,
        /// y borrarlos podría romper esas relaciones o datos que ya dependen de ellos.
        /// </summary>
        /// <param name="migrationBuilder">Constructor de operaciones de la migración.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
