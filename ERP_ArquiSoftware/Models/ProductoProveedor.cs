using ERP_ArquiSoftware.Models.Inventario;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_ArquiSoftware.Models
{
    public class ProductoProveedor
    {
        // Clave compuesta (ver OnModelCreating)
        public int ProductoId { get; set; }
        public int ProveedorId { get; set; }

        // Datos comerciales/operativos del vínculo
        public string? SkuProveedor { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PrecioCompra { get; set; }

        // Plazo de entrega estimado en días
        public int? PlazoEntregaDias { get; set; }

        // Marca si este proveedor es el principal para este producto
        public bool EsProveedorPrincipal { get; set; } = false;

        // Vigencia (útil si llevas histórico de precios/proveedores)
        public DateTime FechaDesde { get; set; } = DateTime.UtcNow;
        public DateTime? FechaHasta { get; set; }

        // Navegación
        public Producto Producto { get; set; } = null!;
        public Proveedor Proveedor { get; set; } = null!;
    }
}

