using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_ArquiSoftware.Migrations
{
    /// <inheritdoc />
    public partial class actualizadodbcontextconrrhh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contrato_Empleado_EmpleadoId",
                table: "Contrato");

            migrationBuilder.DropForeignKey(
                name: "FK_Contrato_TipoContrato_TipoContratoId",
                table: "Contrato");

            migrationBuilder.DropForeignKey(
                name: "FK_Empleado_Almacenes_AlmacenId",
                table: "Empleado");

            migrationBuilder.DropForeignKey(
                name: "FK_Empleado_Cargo_CargoId",
                table: "Empleado");

            migrationBuilder.DropForeignKey(
                name: "FK_Empleado_Departamento_DepartamentoId",
                table: "Empleado");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipoContrato",
                table: "TipoContrato");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Empleado",
                table: "Empleado");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departamento",
                table: "Departamento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contrato",
                table: "Contrato");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cargo",
                table: "Cargo");

            migrationBuilder.RenameTable(
                name: "TipoContrato",
                newName: "TiposContrato");

            migrationBuilder.RenameTable(
                name: "Empleado",
                newName: "Empleados");

            migrationBuilder.RenameTable(
                name: "Departamento",
                newName: "Departamentos");

            migrationBuilder.RenameTable(
                name: "Contrato",
                newName: "Contratos");

            migrationBuilder.RenameTable(
                name: "Cargo",
                newName: "Cargos");

            migrationBuilder.RenameIndex(
                name: "IX_Empleado_DepartamentoId",
                table: "Empleados",
                newName: "IX_Empleados_DepartamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Empleado_CargoId",
                table: "Empleados",
                newName: "IX_Empleados_CargoId");

            migrationBuilder.RenameIndex(
                name: "IX_Empleado_AlmacenId",
                table: "Empleados",
                newName: "IX_Empleados_AlmacenId");

            migrationBuilder.RenameIndex(
                name: "IX_Contrato_TipoContratoId",
                table: "Contratos",
                newName: "IX_Contratos_TipoContratoId");

            migrationBuilder.RenameIndex(
                name: "IX_Contrato_EmpleadoId",
                table: "Contratos",
                newName: "IX_Contratos_EmpleadoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TiposContrato",
                table: "TiposContrato",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Empleados",
                table: "Empleados",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departamentos",
                table: "Departamentos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contratos",
                table: "Contratos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cargos",
                table: "Cargos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_Documento",
                table: "Empleados",
                column: "Documento",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Empleados_EmpleadoId",
                table: "Contratos",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_TiposContrato_TipoContratoId",
                table: "Contratos",
                column: "TipoContratoId",
                principalTable: "TiposContrato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Empleados_Almacenes_AlmacenId",
                table: "Empleados",
                column: "AlmacenId",
                principalTable: "Almacenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Empleados_Cargos_CargoId",
                table: "Empleados",
                column: "CargoId",
                principalTable: "Cargos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Empleados_Departamentos_DepartamentoId",
                table: "Empleados",
                column: "DepartamentoId",
                principalTable: "Departamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Empleados_EmpleadoId",
                table: "Contratos");

            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_TiposContrato_TipoContratoId",
                table: "Contratos");

            migrationBuilder.DropForeignKey(
                name: "FK_Empleados_Almacenes_AlmacenId",
                table: "Empleados");

            migrationBuilder.DropForeignKey(
                name: "FK_Empleados_Cargos_CargoId",
                table: "Empleados");

            migrationBuilder.DropForeignKey(
                name: "FK_Empleados_Departamentos_DepartamentoId",
                table: "Empleados");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TiposContrato",
                table: "TiposContrato");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Empleados",
                table: "Empleados");

            migrationBuilder.DropIndex(
                name: "IX_Empleados_Documento",
                table: "Empleados");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departamentos",
                table: "Departamentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contratos",
                table: "Contratos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cargos",
                table: "Cargos");

            migrationBuilder.RenameTable(
                name: "TiposContrato",
                newName: "TipoContrato");

            migrationBuilder.RenameTable(
                name: "Empleados",
                newName: "Empleado");

            migrationBuilder.RenameTable(
                name: "Departamentos",
                newName: "Departamento");

            migrationBuilder.RenameTable(
                name: "Contratos",
                newName: "Contrato");

            migrationBuilder.RenameTable(
                name: "Cargos",
                newName: "Cargo");

            migrationBuilder.RenameIndex(
                name: "IX_Empleados_DepartamentoId",
                table: "Empleado",
                newName: "IX_Empleado_DepartamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Empleados_CargoId",
                table: "Empleado",
                newName: "IX_Empleado_CargoId");

            migrationBuilder.RenameIndex(
                name: "IX_Empleados_AlmacenId",
                table: "Empleado",
                newName: "IX_Empleado_AlmacenId");

            migrationBuilder.RenameIndex(
                name: "IX_Contratos_TipoContratoId",
                table: "Contrato",
                newName: "IX_Contrato_TipoContratoId");

            migrationBuilder.RenameIndex(
                name: "IX_Contratos_EmpleadoId",
                table: "Contrato",
                newName: "IX_Contrato_EmpleadoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipoContrato",
                table: "TipoContrato",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Empleado",
                table: "Empleado",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departamento",
                table: "Departamento",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contrato",
                table: "Contrato",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cargo",
                table: "Cargo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contrato_Empleado_EmpleadoId",
                table: "Contrato",
                column: "EmpleadoId",
                principalTable: "Empleado",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contrato_TipoContrato_TipoContratoId",
                table: "Contrato",
                column: "TipoContratoId",
                principalTable: "TipoContrato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Empleado_Almacenes_AlmacenId",
                table: "Empleado",
                column: "AlmacenId",
                principalTable: "Almacenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Empleado_Cargo_CargoId",
                table: "Empleado",
                column: "CargoId",
                principalTable: "Cargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Empleado_Departamento_DepartamentoId",
                table: "Empleado",
                column: "DepartamentoId",
                principalTable: "Departamento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
