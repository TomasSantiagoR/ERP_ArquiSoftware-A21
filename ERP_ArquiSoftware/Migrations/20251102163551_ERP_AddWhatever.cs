using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_ArquiSoftware.Migrations
{
    /// <inheritdoc />
    public partial class ERP_AddWhatever : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Severidad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Visto = table.Column<bool>(type: "bit", nullable: false),
                    Creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: true),
                    AlmacenId = table.Column<int>(type: "int", nullable: true),
                    ProductoNombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    AlmacenNombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Stock = table.Column<int>(type: "int", nullable: true),
                    PuntoReorden = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_Tipo_ProductoId_AlmacenId_Visto_Creado",
                table: "Notificaciones",
                columns: new[] { "Tipo", "ProductoId", "AlmacenId", "Visto", "Creado" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notificaciones");
        }
    }
}
