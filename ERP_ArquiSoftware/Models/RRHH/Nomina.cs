using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_ArquiSoftware.Models.RRHH
{
    public class Nomina
    {
        public int Id { get; set; }

        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; } = null!;

        public int PeriodoNominaId { get; set; }
        public PeriodoNomina Periodo { get; set; } = null!;

        // Snapshot de base
        [Column(TypeName = "decimal(18,2)")] public decimal SalarioBase { get; set; }
        public string TipoContratoNombre { get; set; } = "";

        // Devengos
        [Column(TypeName = "decimal(18,2)")] public decimal BasicoDevengado { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal AuxilioTransporte { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal OtrosDevengos { get; set; }

        // Deducciones (empleado)
        [Column(TypeName = "decimal(18,2)")] public decimal SaludEmpleado { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal PensionEmpleado { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal OtrasDeducciones { get; set; }

        // Totales
        [Column(TypeName = "decimal(18,2)")] public decimal TotalDevengado { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal TotalDeducciones { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal NetoPagar { get; set; }

        // Provisiones (referencia empresa)
        [Column(TypeName = "decimal(18,2)")] public decimal ProvCesantias { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal ProvInteresesCesantias { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal ProvPrima { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal ProvVacaciones { get; set; }
    }
}

