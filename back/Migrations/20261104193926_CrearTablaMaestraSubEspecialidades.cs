using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaMaestraSubEspecialidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaestraSubEspecialidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EspecialidadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Promiscuo = table.Column<bool>(type: "bit", nullable: false),
                    Codigo = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaestraSubEspecialidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaestraSubEspecialidades_MaestraEspecialidades_EspecialidadId",
                        column: x => x.EspecialidadId,
                        principalTable: "MaestraEspecialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaestraSubEspecialidades_EspecialidadId",
                table: "MaestraSubEspecialidades",
                column: "EspecialidadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaestraSubEspecialidades");
        }
    }
}
