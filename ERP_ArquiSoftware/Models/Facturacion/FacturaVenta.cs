using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP_ArquiSoftware.Models.Inventario;
using ERP_ArquiSoftware.Models.Ventas;

namespace ERP_ArquiSoftware.Models.Facturacion
{
    public class FacturaVenta
    {
        public int Id { get; set; }

        [Required, StringLength(10)] public string Serie { get; set; } = "FV";
        [Required] public int Numero { get; set; }             
        [Required] public DateTime Fecha { get; set; } = DateTime.Now;

        [Required] public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        [Required] public int AlmacenId { get; set; }
        public Almacen Almacen { get; set; } = null!;

        public int? PedidoVentaId { get; set; }
        public Ventas.PedidoVenta? Pedido { get; set; }

        public int? CondicionPagoId { get; set; }
        public CondicionPago? CondicionPago { get; set; }

        [StringLength(20)] public string Estado { get; set; } = "Emitida"; // Emitida/Anulada

        [Column(TypeName = "decimal(18,2)")] public decimal Subtotal { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal TotalImpuestos { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Total { get; set; }

        public ICollection<FacturaVentaLinea> Lineas { get; set; } = new List<FacturaVentaLinea>();
        public ICollection<Cobro> Cobros { get; set; } = new List<Cobro>();
    }

    public class FacturaVentaLinea
    {
        public int Id { get; set; }
        public int FacturaVentaId { get; set; }
        public FacturaVenta Factura { get; set; } = null!;

        [Required] public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        [Range(1, int.MaxValue)] public int Cantidad { get; set; }

        // Snapshot
        [Column(TypeName = "decimal(18,2)")] public decimal PrecioUnitario { get; set; }
        public int? ImpuestoId { get; set; }
        [Column(TypeName = "decimal(5,2)")] public decimal? ImpuestoPorcentaje { get; set; }

        [Column(TypeName = "decimal(18,2)")] public decimal ImporteNeto { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal ImporteImpuesto { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal ImporteTotal { get; set; }
    }
}
