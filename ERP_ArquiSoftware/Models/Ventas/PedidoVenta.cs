using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP_ArquiSoftware.Models.Inventario;

namespace ERP_ArquiSoftware.Models.Ventas
{
    public class PedidoVenta
    {
        public int Id { get; set; }

        // <-- NUEVO: numeración del pedido
        [Required, StringLength(10)]
        public string Serie { get; set; } = "PV";
        public int Numero { get; set; }

        [Required] public DateTime Fecha { get; set; } = DateTime.Now;

        [Required] public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        [Required] public int AlmacenId { get; set; }
        public Almacen Almacen { get; set; } = null!;

        public int? EmpleadoId { get; set; }

        public int? CondicionPagoId { get; set; }
        public CondicionPago? CondicionPago { get; set; }

        [StringLength(20)] public string Estado { get; set; } = "Borrador"; // Borrador/Confirmado/Facturado/Anulado

        [Column(TypeName = "decimal(18,2)")] public decimal Subtotal { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal TotalImpuestos { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Total { get; set; }

        public ICollection<PedidoVentaLinea> Lineas { get; set; } = new List<PedidoVentaLinea>();
    }

    public class PedidoVentaLinea
    {
        public int Id { get; set; }
        public int PedidoVentaId { get; set; }
        public PedidoVenta Pedido { get; set; } = null!;

        [Required] public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        [Range(1, int.MaxValue)] public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")] public decimal PrecioUnitario { get; set; }

        public int? ImpuestoId { get; set; }
        [Column(TypeName = "decimal(5,2)")] public decimal? ImpuestoPorcentaje { get; set; }

        [Column(TypeName = "decimal(18,2)")] public decimal ImporteNeto { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal ImporteImpuesto { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal ImporteTotal { get; set; }
    }
}



