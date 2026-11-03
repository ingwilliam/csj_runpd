using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaMaestraMunicipios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaestraMunicipios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CodigoDivpola = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Tipo = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    Pdet = table.Column<bool>(type: "bit", nullable: false),
                    DepartamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsejoSeccionalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DireccionSeccionalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaestraMunicipios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaestraMunicipios_MaestraConsejosSeccionales_ConsejoSeccionalId",
                        column: x => x.ConsejoSeccionalId,
                        principalTable: "MaestraConsejosSeccionales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MaestraMunicipios_MaestraDepartamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "MaestraDepartamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaestraMunicipios_MaestraDireccionesSeccionales_DireccionSeccionalId",
                        column: x => x.DireccionSeccionalId,
                        principalTable: "MaestraDireccionesSeccionales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaestraMunicipios_ConsejoSeccionalId",
                table: "MaestraMunicipios",
                column: "ConsejoSeccionalId");

            migrationBuilder.CreateIndex(
                name: "IX_MaestraMunicipios_DepartamentoId",
                table: "MaestraMunicipios",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_MaestraMunicipios_DireccionSeccionalId",
                table: "MaestraMunicipios",
                column: "DireccionSeccionalId");

            migrationBuilder.CreateIndex(
                name: "UX_MaestraMunicipios_CodigoDivpola",
                table: "MaestraMunicipios",
                column: "CodigoDivpola",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaestraMunicipios");
        }
    }
}
