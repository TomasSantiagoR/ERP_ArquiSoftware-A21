using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_ArquiSoftware.Migrations
{
    /// <inheritdoc />
    public partial class VentaLinea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacturaVentaLineas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacturaVentaId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImpuestoId = table.Column<int>(type: "int", nullable: true),
                    ImpuestoPorcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ImporteNeto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteImpuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaVentaLineas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacturaVentaLineas_FacturasVenta_FacturaVentaId",
                        column: x => x.FacturaVentaId,
                        principalTable: "FacturasVenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacturaVentaLineas_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoVentaLineas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoVentaId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImpuestoId = table.Column<int>(type: "int", nullable: true),
                    ImpuestoPorcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ImporteNeto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteImpuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoVentaLineas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoVentaLineas_PedidosVenta_PedidoVentaId",
                        column: x => x.PedidoVentaId,
                        principalTable: "PedidosVenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoVentaLineas_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacturaVentaLineas_FacturaVentaId",
                table: "FacturaVentaLineas",
                column: "FacturaVentaId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaVentaLineas_ProductoId",
                table: "FacturaVentaLineas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoVentaLineas_PedidoVentaId",
                table: "PedidoVentaLineas",
                column: "PedidoVentaId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoVentaLineas_ProductoId",
                table: "PedidoVentaLineas",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacturaVentaLineas");

            migrationBuilder.DropTable(
                name: "PedidoVentaLineas");
        }
    }
}
