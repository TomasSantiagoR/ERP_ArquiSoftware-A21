using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_ArquiSoftware.Migrations
{
    /// <inheritdoc />
    public partial class HR_Payroll_Params : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NovedadesNomina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    DiasIncapacidadEPS = table.Column<int>(type: "int", nullable: false),
                    DiasLicenciaRemunerada = table.Column<int>(type: "int", nullable: false),
                    DiasNoRemunerados = table.Column<int>(type: "int", nullable: false),
                    DiasVacacionesTomadas = table.Column<int>(type: "int", nullable: false),
                    HED = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HEN = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HEDFest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HENFest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RecargoNocturno = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Comisiones = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bonificaciones = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AuxilioNoConstitutivo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrestamoEmpresa = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Embargo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtrasDeducciones = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NovedadesNomina", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParamNomina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    SMMLV = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AuxTransporte = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AuxTransporteTopeSMMLV = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SaludEmpleado = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    PensionEmpleado = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    SaludEmpleador = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    PensionEmpleador = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    CajaCompensacion = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    ICBF = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    SENA = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Cesantias = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    InteresesCesAnual = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Prima = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Vacaciones = table.Column<decimal>(type: "decimal(6,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParamNomina", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NovedadesNomina");

            migrationBuilder.DropTable(
                name: "ParamNomina");
        }
    }
}
