using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaMaestraDistritos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaestraDistritos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    JurisdiccionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubEspecialidadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaestraDistritos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaestraDistritos_MaestraJurisdicciones_JurisdiccionId",
                        column: x => x.JurisdiccionId,
                        principalTable: "MaestraJurisdicciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaestraDistritos_MaestraSubEspecialidades_SubEspecialidadId",
                        column: x => x.SubEspecialidadId,
                        principalTable: "MaestraSubEspecialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaestraDistritos_JurisdiccionId",
                table: "MaestraDistritos",
                column: "JurisdiccionId");

            migrationBuilder.CreateIndex(
                name: "IX_MaestraDistritos_SubEspecialidadId",
                table: "MaestraDistritos",
                column: "SubEspecialidadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaestraDistritos");
        }
    }
}
