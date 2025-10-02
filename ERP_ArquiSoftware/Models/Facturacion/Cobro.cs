using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_ArquiSoftware.Models.Facturacion
{
    public class Cobro
    {
        public int Id { get; set; }
        public int FacturaVentaId { get; set; }
        public FacturaVenta Factura { get; set; } = null!;

        public DateTime Fecha { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [StringLength(30)] public string MedioPago { get; set; } = "Efectivo"; // Efectivo/TC/TD/Transferencia
        [StringLength(50)] public string? Referencia { get; set; }
    }
}


