using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_ArquiSoftware.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [DisplayName("Nombre del Producto")]
        public string NombreProducto { get; set; } = string.Empty;

        // FK
        [Required(ErrorMessage = "La categoría es obligatoria")]
        [DisplayName("Categoría")]
        public int CategoriaId { get; set; }

        // Navegación
        public Categoria? Categoria { get; set; }

        [DisplayName("Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required, DisplayName("Precio Unitario")]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Required, DisplayName("Stock Actual")]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required, DisplayName("Stock Mínimo")]
        [Range(0, int.MaxValue)]
        public int StockMin { get; set; }

        public bool Activo { get; set; } = true;

        [DisplayName("Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}


