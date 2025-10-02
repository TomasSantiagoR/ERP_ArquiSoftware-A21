using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.RRHH
{
    public class Contrato
    {
        public int Id { get; set; }

        public int EmpleadoId { get; set; }
        public Empleado? Empleado { get; set; }

        public int TipoContratoId { get; set; }
        public TipoContrato? TipoContrato { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Precision(18, 2)]
        public decimal Salario { get; set; }


        public bool Activo { get; set; } = true;
    }
}
