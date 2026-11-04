using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaDespachos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Despachos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Promiscuo = table.Column<bool>(type: "bit", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ComplementoDireccion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TelefonoFijo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TelefonoCelular = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    HorarioAtencion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EspecialidadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubEspecialidadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MunicipioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JurisdiccionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DistritoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CircuitoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DistritoSubEspecialidadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CircuitoSubEspecialidadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntidadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Despachos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Despachos_MaestraCircuitos_CircuitoId",
                        column: x => x.CircuitoId,
                        principalTable: "MaestraCircuitos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Despachos_MaestraCircuitos_CircuitoSubEspecialidadId",
                        column: x => x.CircuitoSubEspecialidadId,
                        principalTable: "MaestraCircuitos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Despachos_MaestraDistritos_DistritoId",
                        column: x => x.DistritoId,
                        principalTable: "MaestraDistritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Despachos_MaestraDistritos_DistritoSubEspecialidadId",
                        column: x => x.DistritoSubEspecialidadId,
                        principalTable: "MaestraDistritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Despachos_MaestraEntidades_EntidadId",
                        column: x => x.EntidadId,
                        principalTable: "MaestraEntidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Despachos_MaestraEspecialidades_EspecialidadId",
                        column: x => x.EspecialidadId,
                        principalTable: "MaestraEspecialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Despachos_MaestraJurisdicciones_JurisdiccionId",
                        column: x => x.JurisdiccionId,
                        principalTable: "MaestraJurisdicciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Despachos_MaestraMunicipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "MaestraMunicipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Despachos_MaestraSubEspecialidades_SubEspecialidadId",
                        column: x => x.SubEspecialidadId,
                        principalTable: "MaestraSubEspecialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Despachos_CircuitoId",
                table: "Despachos",
                column: "CircuitoId");

            migrationBuilder.CreateIndex(
                name: "IX_Despachos_CircuitoSubEspecialidadId",
                table: "Despachos",
                column: "CircuitoSubEspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Despachos_Codigo",
                table: "Despachos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Despachos_DistritoId",
                table: "Despachos",
                column: "DistritoId");

            migrationBuilder.CreateIndex(
                name: "IX_Despachos_DistritoSubEspecialidadId",
                table: "Despachos",
                column: "DistritoSubEspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Despachos_EntidadId",
                table: "Despachos",
                column: "EntidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Despachos_EspecialidadId",
                table: "Despachos",
                column: "EspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Despachos_JurisdiccionId",
                table: "Despachos",
                column: "JurisdiccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Despachos_MunicipioId",
                table: "Despachos",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_Despachos_SubEspecialidadId",
                table: "Despachos",
                column: "SubEspecialidadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Despachos");
        }
    }
}
