using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.RRHH
{
    public class Departamento
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        // Relación 1:N con empleados
        public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    }
}
