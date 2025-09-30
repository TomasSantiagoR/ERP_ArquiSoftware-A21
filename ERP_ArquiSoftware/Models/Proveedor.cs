using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_ArquiSoftware.Models
{
    public class Proveedor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [DisplayName("Nombre del Producto")]
        public string NombreProducto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [DisplayName("Categoría")]
        public string Categoria { get; set; } = string.Empty;


        [DisplayName("Descripción")]
        public string Descripcion { get; set; } = string.Empty;


        [Required(ErrorMessage = "El precio del producto es obligatorio")]
        [DisplayName("Precio Unitario")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        [Column(TypeName = "decimal(18,2)")]   // 👈 especifica precisión y escala en SQL Server
        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "El stock del producto es obligatorio")]
        [DisplayName("Stock Actual")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El stock minimo del producto es obligatorio")]
        [DisplayName("Stock Mínimo")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
        public int StockMin { get; set; }

        public bool Activo { get; set; } = true;

        [DisplayName("Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
