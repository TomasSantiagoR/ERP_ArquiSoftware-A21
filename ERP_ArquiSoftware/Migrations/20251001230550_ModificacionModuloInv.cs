using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_ArquiSoftware.Migrations
{
    /// <inheritdoc />
    public partial class ModificacionModuloInv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductoProveedores",
                table: "ProductoProveedores");

            migrationBuilder.AlterColumn<string>(
                name: "NombreProducto",
                table: "Productos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<decimal>(
                name: "AltoCm",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AnchoCm",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CantidadReorden",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Productos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ContenidoPorUnidad",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CostoEstandar",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CostoPromedio",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Descontinuado",
                table: "Productos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsPublicableWeb",
                table: "Productos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GestionaLotes",
                table: "Productos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GestionaSeries",
                table: "Productos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Gtin",
                table: "Productos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImpuestoId",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LargoCm",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MarcaId",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Perecedero",
                table: "Productos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PesoBrutoKg",
                table: "Productos",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PesoNetoKg",
                table: "Productos",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PuntoReorden",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RazonDescontinuacion",
                table: "Productos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "Productos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Talla",
                table: "Productos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UltimoCosto",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnidadContenidoId",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnidadMedidaId",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VidaUtilDias",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductoProveedores",
                table: "ProductoProveedores",
                columns: new[] { "ProductoId", "ProveedorId", "FechaDesde" });

            migrationBuilder.CreateTable(
                name: "Almacenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Almacenes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Impuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Porcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IncluidoEnPrecio = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Impuestos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Marcas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesMedida",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesMedida", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Existencias",
                columns: table => new
                {
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    AlmacenId = table.Column<int>(type: "int", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Existencias", x => new { x.ProductoId, x.AlmacenId });
                    table.ForeignKey(
                        name: "FK_Existencias_Almacenes_AlmacenId",
                        column: x => x.AlmacenId,
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Existencias_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Gtin",
                table: "Productos",
                column: "Gtin");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ImpuestoId",
                table: "Productos",
                column: "ImpuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_MarcaId",
                table: "Productos",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Sku",
                table: "Productos",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_UnidadContenidoId",
                table: "Productos",
                column: "UnidadContenidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_UnidadMedidaId",
                table: "Productos",
                column: "UnidadMedidaId");

            migrationBuilder.CreateIndex(
                name: "IX_Existencias_AlmacenId",
                table: "Existencias",
                column: "AlmacenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Impuestos_ImpuestoId",
                table: "Productos",
                column: "ImpuestoId",
                principalTable: "Impuestos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Marcas_MarcaId",
                table: "Productos",
                column: "MarcaId",
                principalTable: "Marcas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_UnidadesMedida_UnidadContenidoId",
                table: "Productos",
                column: "UnidadContenidoId",
                principalTable: "UnidadesMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_UnidadesMedida_UnidadMedidaId",
                table: "Productos",
                column: "UnidadMedidaId",
                principalTable: "UnidadesMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Impuestos_ImpuestoId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Marcas_MarcaId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_UnidadesMedida_UnidadContenidoId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_UnidadesMedida_UnidadMedidaId",
                table: "Productos");

            migrationBuilder.DropTable(
                name: "Existencias");

            migrationBuilder.DropTable(
                name: "Impuestos");

            migrationBuilder.DropTable(
                name: "Marcas");

            migrationBuilder.DropTable(
                name: "UnidadesMedida");

            migrationBuilder.DropTable(
                name: "Almacenes");

            migrationBuilder.DropIndex(
                name: "IX_Productos_Gtin",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_ImpuestoId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_MarcaId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_Sku",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_UnidadContenidoId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_UnidadMedidaId",
                table: "Productos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductoProveedores",
                table: "ProductoProveedores");

            migrationBuilder.DropColumn(
                name: "AltoCm",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "AnchoCm",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CantidadReorden",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ContenidoPorUnidad",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CostoEstandar",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CostoPromedio",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Descontinuado",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "EsPublicableWeb",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "GestionaLotes",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "GestionaSeries",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Gtin",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ImpuestoId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "LargoCm",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "MarcaId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Perecedero",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PesoBrutoKg",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PesoNetoKg",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PuntoReorden",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "RazonDescontinuacion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Talla",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UltimoCosto",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UnidadContenidoId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UnidadMedidaId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "VidaUtilDias",
                table: "Productos");

            migrationBuilder.AlterColumn<string>(
                name: "NombreProducto",
                table: "Productos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductoProveedores",
                table: "ProductoProveedores",
                columns: new[] { "ProductoId", "ProveedorId" });
        }
    }
}
