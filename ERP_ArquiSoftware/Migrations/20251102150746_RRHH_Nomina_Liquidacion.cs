using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_ArquiSoftware.Migrations
{
    /// <inheritdoc />
    public partial class RRHH_Nomina_Liquidacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Liquidaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiasTrabajados = table.Column<int>(type: "int", nullable: false),
                    SalarioBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cesantias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InteresesCesantias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Prima = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Vacaciones = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtrasDeducciones = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetoPagar = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Liquidaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Liquidaciones_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeriodosNomina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cerrado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodosNomina", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nominas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    PeriodoNominaId = table.Column<int>(type: "int", nullable: false),
                    SalarioBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TipoContratoNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BasicoDevengado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AuxilioTransporte = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtrosDevengos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaludEmpleado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PensionEmpleado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtrasDeducciones = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDevengado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeducciones = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetoPagar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProvCesantias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProvInteresesCesantias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProvPrima = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProvVacaciones = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nominas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nominas_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Nominas_PeriodosNomina_PeriodoNominaId",
                        column: x => x.PeriodoNominaId,
                        principalTable: "PeriodosNomina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Liquidaciones_EmpleadoId",
                table: "Liquidaciones",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Nominas_EmpleadoId",
                table: "Nominas",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Nominas_PeriodoNominaId",
                table: "Nominas",
                column: "PeriodoNominaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Liquidaciones");

            migrationBuilder.DropTable(
                name: "Nominas");

            migrationBuilder.DropTable(
                name: "PeriodosNomina");
        }
    }
}
