using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_ArquiSoftware.Migrations
{
    /// <inheritdoc />
    public partial class HR_Add_ParamNomina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ParamNomina",
                table: "ParamNomina");

            migrationBuilder.DropColumn(
                name: "AuxTransporteTopeSMMLV",
                table: "ParamNomina");

            migrationBuilder.DropColumn(
                name: "CajaCompensacion",
                table: "ParamNomina");

            migrationBuilder.DropColumn(
                name: "Cesantias",
                table: "ParamNomina");

            migrationBuilder.DropColumn(
                name: "ICBF",
                table: "ParamNomina");

            migrationBuilder.DropColumn(
                name: "InteresesCesAnual",
                table: "ParamNomina");

            migrationBuilder.DropColumn(
                name: "PensionEmpleado",
                table: "ParamNomina");

            migrationBuilder.DropColumn(
                name: "PensionEmpleador",
                table: "ParamNomina");

            migrationBuilder.DropColumn(
                name: "Prima",
                table: "ParamNomina");

            migrationBuilder.DropColumn(
                name: "SENA",
                table: "ParamNomina");

            migrationBuilder.DropColumn(
                name: "SaludEmpleado",
                table: "ParamNomina");

            migrationBuilder.DropColumn(
                name: "SaludEmpleador",
                table: "ParamNomina");

            migrationBuilder.DropColumn(
                name: "Vacaciones",
                table: "ParamNomina");

            migrationBuilder.RenameTable(
                name: "ParamNomina",
                newName: "ParametrosNomina");

            migrationBuilder.RenameColumn(
                name: "AuxTransporte",
                table: "ParametrosNomina",
                newName: "AuxilioTransporte");

            migrationBuilder.AddColumn<decimal>(
                name: "CesantiasPorc",
                table: "ParametrosNomina",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DiasMes",
                table: "ParametrosNomina",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "InteresesCesantiasAnualPorc",
                table: "ParametrosNomina",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PensionEmpleadoPorc",
                table: "ParametrosNomina",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrimaPorc",
                table: "ParametrosNomina",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SaludEmpleadoPorc",
                table: "ParametrosNomina",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TopeAuxTranspMultiplo",
                table: "ParametrosNomina",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "VacacionesPorc",
                table: "ParametrosNomina",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParametrosNomina",
                table: "ParametrosNomina",
                column: "Id");

            migrationBuilder.InsertData(
                table: "ParametrosNomina",
                columns: new[] { "Id", "Activo", "Anio", "AuxilioTransporte", "CesantiasPorc", "DiasMes", "InteresesCesantiasAnualPorc", "PensionEmpleadoPorc", "PrimaPorc", "SMMLV", "SaludEmpleadoPorc", "TopeAuxTranspMultiplo", "VacacionesPorc" },
                values: new object[] { 1, true, 2025, 162000m, 0.0833m, 30, 0.12m, 0.04m, 0.0833m, 1300000m, 0.04m, 2, 0.0417m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ParametrosNomina",
                table: "ParametrosNomina");

            migrationBuilder.DeleteData(
                table: "ParametrosNomina",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "CesantiasPorc",
                table: "ParametrosNomina");

            migrationBuilder.DropColumn(
                name: "DiasMes",
                table: "ParametrosNomina");

            migrationBuilder.DropColumn(
                name: "InteresesCesantiasAnualPorc",
                table: "ParametrosNomina");

            migrationBuilder.DropColumn(
                name: "PensionEmpleadoPorc",
                table: "ParametrosNomina");

            migrationBuilder.DropColumn(
                name: "PrimaPorc",
                table: "ParametrosNomina");

            migrationBuilder.DropColumn(
                name: "SaludEmpleadoPorc",
                table: "ParametrosNomina");

            migrationBuilder.DropColumn(
                name: "TopeAuxTranspMultiplo",
                table: "ParametrosNomina");

            migrationBuilder.DropColumn(
                name: "VacacionesPorc",
                table: "ParametrosNomina");

            migrationBuilder.RenameTable(
                name: "ParametrosNomina",
                newName: "ParamNomina");

            migrationBuilder.RenameColumn(
                name: "AuxilioTransporte",
                table: "ParamNomina",
                newName: "AuxTransporte");

            migrationBuilder.AddColumn<decimal>(
                name: "AuxTransporteTopeSMMLV",
                table: "ParamNomina",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CajaCompensacion",
                table: "ParamNomina",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Cesantias",
                table: "ParamNomina",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ICBF",
                table: "ParamNomina",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InteresesCesAnual",
                table: "ParamNomina",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PensionEmpleado",
                table: "ParamNomina",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PensionEmpleador",
                table: "ParamNomina",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Prima",
                table: "ParamNomina",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SENA",
                table: "ParamNomina",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SaludEmpleado",
                table: "ParamNomina",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SaludEmpleador",
                table: "ParamNomina",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Vacaciones",
                table: "ParamNomina",
                type: "decimal(6,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParamNomina",
                table: "ParamNomina",
                column: "Id");
        }
    }
}
