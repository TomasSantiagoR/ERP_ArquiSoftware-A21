using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_ArquiSoftware.Models.Inventario
{
    public class Producto
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        [DisplayName("Nombre del Producto")]
        public string NombreProducto { get; set; } = string.Empty;

        // Identificación
        [Required, StringLength(50)] public string Sku { get; set; } = "";   // único interno
        [StringLength(20)] public string? Gtin { get; set; }                 // EAN/UPC opcional

        // Relaciones básicas
        [Required] public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public int? MarcaId { get; set; }
        public Marca? Marca { get; set; }

        public int? UnidadMedidaId { get; set; }
        public UnidadMedida? UnidadMedida { get; set; }

        // Descripción y catálogo
        public string Descripcion { get; set; } = string.Empty;
        [StringLength(30)] public string? Color { get; set; }
        [StringLength(30)] public string? Talla { get; set; }

        // Precios y costos
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")] public decimal? CostoEstandar { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? CostoPromedio { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? UltimoCosto { get; set; }

        public int? ImpuestoId { get; set; }
        public Impuesto? Impuesto { get; set; }

        // Inventario (global; detalle por almacén en Existencia)
        [Range(0, int.MaxValue)] public int Stock { get; set; }
        [Range(0, int.MaxValue)] public int StockMin { get; set; }
        [Range(0, int.MaxValue)] public int? PuntoReorden { get; set; }
        [Range(0, int.MaxValue)] public int? CantidadReorden { get; set; }

        // Perecederos / trazabilidad
        public bool Perecedero { get; set; } = false;
        public bool GestionaLotes { get; set; } = false;
        public bool GestionaSeries { get; set; } = false;
        public int? VidaUtilDias { get; set; }

        // Medidas y empaque
        [Column(TypeName = "decimal(18,3)")] public decimal? PesoNetoKg { get; set; }
        [Column(TypeName = "decimal(18,3)")] public decimal? PesoBrutoKg { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? AltoCm { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? AnchoCm { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? LargoCm { get; set; }

        public decimal? ContenidoPorUnidad { get; set; } // 500, 1, etc.
        public int? UnidadContenidoId { get; set; }      // ml, kg…
        public UnidadMedida? UnidadContenido { get; set; }

        // Catálogo / ciclo de vida
        public bool Activo { get; set; } = true;
        public bool Descontinuado { get; set; } = false;
        [StringLength(200)] public string? RazonDescontinuacion { get; set; }
        public bool EsPublicableWeb { get; set; } = false;

        [DisplayName("Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relaciones
        public ICollection<ProductoProveedor> ProductoProveedores { get; set; } = new List<ProductoProveedor>();
        public ICollection<Existencia> Existencias { get; set; } = new List<Existencia>();

    }
}


