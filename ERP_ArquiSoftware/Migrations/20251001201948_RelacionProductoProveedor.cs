using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_ArquiSoftware.Migrations
{
    /// <inheritdoc />
    public partial class RelacionProductoProveedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proveedores_Categorias_CategoriaId",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_CategoriaId",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Proveedores");

            migrationBuilder.AlterColumn<string>(
                name: "NombreProducto",
                table: "Productos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ProductoProveedores",
                columns: table => new
                {
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    SkuProveedor = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PrecioCompra = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlazoEntregaDias = table.Column<int>(type: "int", nullable: true),
                    EsProveedorPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    FechaDesde = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaHasta = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoProveedores", x => new { x.ProductoId, x.ProveedorId });
                    table.ForeignKey(
                        name: "FK_ProductoProveedores_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoProveedores_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_NombreProducto",
                table: "Productos",
                column: "NombreProducto");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoProveedores_ProductoId_EsProveedorPrincipal",
                table: "ProductoProveedores",
                columns: new[] { "ProductoId", "EsProveedorPrincipal" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductoProveedores_ProveedorId",
                table: "ProductoProveedores",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoProveedores_SkuProveedor",
                table: "ProductoProveedores",
                column: "SkuProveedor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductoProveedores");

            migrationBuilder.DropIndex(
                name: "IX_Productos_NombreProducto",
                table: "Productos");

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Proveedores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "NombreProducto",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_CategoriaId",
                table: "Proveedores",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proveedores_Categorias_CategoriaId",
                table: "Proveedores",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
