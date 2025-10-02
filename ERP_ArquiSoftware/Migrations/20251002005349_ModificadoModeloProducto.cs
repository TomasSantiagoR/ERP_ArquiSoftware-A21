using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_ArquiSoftware.Migrations
{
    /// <inheritdoc />
    public partial class ModificadoModeloProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_UnidadesMedida_UnidadContenidoId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_UnidadContenidoId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "AltoCm",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "AnchoCm",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ContenidoPorUnidad",
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
                name: "LargoCm",
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
                name: "RazonDescontinuacion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Salario",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Talla",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UnidadContenidoId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "VidaUtilDias",
                table: "Productos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<decimal>(
                name: "LargoCm",
                table: "Productos",
                type: "decimal(18,2)",
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

            migrationBuilder.AddColumn<string>(
                name: "RazonDescontinuacion",
                table: "Productos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Salario",
                table: "Productos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Talla",
                table: "Productos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnidadContenidoId",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VidaUtilDias",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_UnidadContenidoId",
                table: "Productos",
                column: "UnidadContenidoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_UnidadesMedida_UnidadContenidoId",
                table: "Productos",
                column: "UnidadContenidoId",
                principalTable: "UnidadesMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
