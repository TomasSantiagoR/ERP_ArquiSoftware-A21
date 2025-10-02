using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.RRHH
{
    public class Cargo
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        // Salario base de referencia (opcional)

        [Precision(18, 2)]
        public decimal Salario { get; set; }

        public decimal? SalarioBase { get; set; }

        // Relación 1:N con empleados
        public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    }
}
