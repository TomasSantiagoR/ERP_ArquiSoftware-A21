using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_ArquiSoftware.Models.Inventario
{
    public class Producto
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string NombreProducto { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Sku { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Gtin { get; set; }

        [Required] public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public int? MarcaId { get; set; }
        public Marca? Marca { get; set; }

        public int? UnidadMedidaId { get; set; }
        public UnidadMedida? UnidadMedida { get; set; }

        public string Descripcion { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")] public decimal? CostoEstandar { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? CostoPromedio { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? UltimoCosto { get; set; }

        public int? ImpuestoId { get; set; }
        public Impuesto? Impuesto { get; set; }

        [Range(0, int.MaxValue)] public int Stock { get; set; }
        [Range(0, int.MaxValue)] public int StockMin { get; set; }
        [Range(0, int.MaxValue)] public int? PuntoReorden { get; set; }
        [Range(0, int.MaxValue)] public int? CantidadReorden { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relaciones
        


        public ICollection<ProductoProveedor> ProductoProveedores { get; set; } = new List<ProductoProveedor>();
        public ICollection<Existencia> Existencias { get; set; } = new List<Existencia>();

    }
}


