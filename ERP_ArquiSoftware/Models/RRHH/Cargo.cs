using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.RRHH
{
    public class Cargo
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string Nombre { get; set; } = string.Empty;
        [StringLength(500)] public string? Descripcion { get; set; }
        [Precision(18, 2)] public decimal? SalarioBase { get; set; }  // nullable
        public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    }

}
