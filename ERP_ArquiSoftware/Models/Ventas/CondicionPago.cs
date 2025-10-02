using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.Ventas
{
    public class CondicionPago
    {
        public int Id { get; set; }
        [Required, StringLength(80)] public string Nombre { get; set; } = "";
        // Ej: Contado = 0, 15 días, 30 días...
        [Range(0, 365)] public int DiasPlazo { get; set; } = 0;

        public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
        public ICollection<PedidoVenta> Pedidos { get; set; } = new List<PedidoVenta>();
        public ICollection<Facturacion.FacturaVenta> Facturas { get; set; } = new List<Facturacion.FacturaVenta>();
    }
}


