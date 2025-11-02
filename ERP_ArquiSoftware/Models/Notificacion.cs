using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_ArquiSoftware.Models
{
    public class Notificacion
    {
        public int Id { get; set; }

        // Categoría simple para crecer después (LOW_STOCK, INFO, WARNING, ERROR, etc.)
        [StringLength(30)]
        public string Tipo { get; set; } = "LOW_STOCK";

        [StringLength(120)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Mensaje { get; set; } = string.Empty;

        // Severidad visual para UI: info/success/warning/danger
        [StringLength(20)]
        public string Severidad { get; set; } = "warning";

        public bool Visto { get; set; } = false;

        public DateTime Creado { get; set; } = DateTime.Now;

        // Datos opcionales para enlazar
        public int? ProductoId { get; set; }
        public int? AlmacenId { get; set; }

        // Cache de valores útiles para listar sin joins (opcional)
        [StringLength(120)] public string? ProductoNombre { get; set; }
        [StringLength(120)] public string? AlmacenNombre { get; set; }
        public int? Stock { get; set; }
        public int? PuntoReorden { get; set; }
    }
}
