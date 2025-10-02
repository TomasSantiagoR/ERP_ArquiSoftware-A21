using ERP_ArquiSoftware.Models.Inventario;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_ArquiSoftware.Models.Inventario
{
    public class MovimientoInventario
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
        [StringLength(20)] public string Tipo { get; set; } = "VENTA"; // VENTA/COMPRA/AJUSTE/TRANSFER

        public int AlmacenId { get; set; }
        public Almacen Almacen { get; set; } = null!;

        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        // Cantidad negativa para salidas (venta)
        public int Cantidad { get; set; }

        [StringLength(30)] public string? DocumentoTipo { get; set; } // "FacturaVenta"
        public int? DocumentoId { get; set; }                         // Id de la factura
    }
}

