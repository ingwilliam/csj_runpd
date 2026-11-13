using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runpd.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEsDescongestionADespachos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsDescongestion",
                table: "Despachos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsDescongestion",
                table: "Despachos");
        }
    }
}
