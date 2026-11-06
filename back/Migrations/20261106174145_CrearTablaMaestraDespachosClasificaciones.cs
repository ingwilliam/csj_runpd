using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaMaestraDespachosClasificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaestraDespachosClasificaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JurisdiccionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MunicipioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubEspecialidadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DistritoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CircuitoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaestraDespachosClasificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaestraDespachosClasificaciones_MaestraCircuitos_CircuitoId",
                        column: x => x.CircuitoId,
                        principalTable: "MaestraCircuitos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MaestraDespachosClasificaciones_MaestraDistritos_DistritoId",
                        column: x => x.DistritoId,
                        principalTable: "MaestraDistritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaestraDespachosClasificaciones_MaestraJurisdicciones_JurisdiccionId",
                        column: x => x.JurisdiccionId,
                        principalTable: "MaestraJurisdicciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaestraDespachosClasificaciones_MaestraMunicipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "MaestraMunicipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaestraDespachosClasificaciones_MaestraSubEspecialidades_SubEspecialidadId",
                        column: x => x.SubEspecialidadId,
                        principalTable: "MaestraSubEspecialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaestraDespachosClasificaciones_CircuitoId",
                table: "MaestraDespachosClasificaciones",
                column: "CircuitoId");

            migrationBuilder.CreateIndex(
                name: "IX_MaestraDespachosClasificaciones_DistritoId",
                table: "MaestraDespachosClasificaciones",
                column: "DistritoId");

            migrationBuilder.CreateIndex(
                name: "IX_MaestraDespachosClasificaciones_JurisdiccionId_MunicipioId_SubEspecialidadId",
                table: "MaestraDespachosClasificaciones",
                columns: new[] { "JurisdiccionId", "MunicipioId", "SubEspecialidadId" },
                unique: true,
                filter: "[SubEspecialidadId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MaestraDespachosClasificaciones_MunicipioId",
                table: "MaestraDespachosClasificaciones",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_MaestraDespachosClasificaciones_SubEspecialidadId",
                table: "MaestraDespachosClasificaciones",
                column: "SubEspecialidadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaestraDespachosClasificaciones");
        }
    }
}
