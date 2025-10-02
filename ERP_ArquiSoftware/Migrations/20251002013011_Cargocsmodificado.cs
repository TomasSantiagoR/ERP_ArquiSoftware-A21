using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_ArquiSoftware.Migrations
{
    /// <inheritdoc />
    public partial class Cargocsmodificado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salario",
                table: "Cargos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Salario",
                table: "Cargos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
