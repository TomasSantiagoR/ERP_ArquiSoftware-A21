using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.Ventas
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required, StringLength(20)] public string TipoDocumento { get; set; } = "CC"; // CC/NIT/CE
        [Required, StringLength(30)] public string NumeroDocumento { get; set; } = "";

        [Required, StringLength(200)] public string NombreRazonSocial { get; set; } = "";
        [StringLength(200)] public string? NombreComercial { get; set; }

        [EmailAddress] public string? Correo { get; set; }
        [StringLength(30)] public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;

        // Preferencias comerciales
        public int? CondicionPagoId { get; set; }
        public CondicionPago? CondicionPago { get; set; }

        public ICollection<DireccionCliente> Direcciones { get; set; } = new List<DireccionCliente>();
        public ICollection<PedidoVenta> Pedidos { get; set; } = new List<PedidoVenta>();
        public ICollection<Facturacion.FacturaVenta> Facturas { get; set; } = new List<Facturacion.FacturaVenta>();
    }
}

