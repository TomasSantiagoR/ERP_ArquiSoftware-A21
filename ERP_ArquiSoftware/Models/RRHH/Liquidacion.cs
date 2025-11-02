using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_ArquiSoftware.Models.RRHH
{
    public class Liquidacion
    {
        public int Id { get; set; }

        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; } = null!;

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DiasTrabajados { get; set; }

        [Column(TypeName = "decimal(18,2)")] public decimal SalarioBase { get; set; }

        [Column(TypeName = "decimal(18,2)")] public decimal Cesantias { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal InteresesCesantias { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Prima { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Vacaciones { get; set; }

        [Column(TypeName = "decimal(18,2)")] public decimal OtrasDeducciones { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal NetoPagar { get; set; }
    }
}
