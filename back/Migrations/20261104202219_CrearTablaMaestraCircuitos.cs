using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaMaestraCircuitos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaestraCircuitos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DistritoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubEspecialidadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaestraCircuitos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaestraCircuitos_MaestraDistritos_DistritoId",
                        column: x => x.DistritoId,
                        principalTable: "MaestraDistritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaestraCircuitos_MaestraSubEspecialidades_SubEspecialidadId",
                        column: x => x.SubEspecialidadId,
                        principalTable: "MaestraSubEspecialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaestraCircuitos_DistritoId",
                table: "MaestraCircuitos",
                column: "DistritoId");

            migrationBuilder.CreateIndex(
                name: "IX_MaestraCircuitos_SubEspecialidadId",
                table: "MaestraCircuitos",
                column: "SubEspecialidadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaestraCircuitos");
        }
    }
}
