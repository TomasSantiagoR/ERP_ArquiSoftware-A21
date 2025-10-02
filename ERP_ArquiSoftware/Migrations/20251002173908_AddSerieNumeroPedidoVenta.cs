using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_ArquiSoftware.Migrations
{
    /// <inheritdoc />
    public partial class AddSerieNumeroPedidoVenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Numero",
                table: "PedidosVenta",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Serie",
                table: "PedidosVenta",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosVenta_Serie_Numero",
                table: "PedidosVenta",
                columns: new[] { "Serie", "Numero" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PedidosVenta_Serie_Numero",
                table: "PedidosVenta");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "PedidosVenta");

            migrationBuilder.DropColumn(
                name: "Serie",
                table: "PedidosVenta");
        }
    }
}
