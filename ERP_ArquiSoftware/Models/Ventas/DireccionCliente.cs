using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.Ventas
{
    public class DireccionCliente
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        [Required, StringLength(20)] public string Tipo { get; set; } = "Entrega"; // Entrega/Facturacion
        [Required, StringLength(250)] public string Direccion { get; set; } = "";
        [StringLength(100)] public string? Ciudad { get; set; }
        [StringLength(100)] public string? Departamento { get; set; }
        [StringLength(20)] public string? CodigoPostal { get; set; }
    }
}


