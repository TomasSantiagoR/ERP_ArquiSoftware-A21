using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.Facturacion
{
    public class SecuenciaDocumento
    {
        public int Id { get; set; }

        [Required, StringLength(15)]
        public string Tipo { get; set; } = "FV"; // FV = Factura Venta

        [Required, StringLength(10)]
        public string Serie { get; set; } = "FV";

        public int SiguienteNumero { get; set; } = 1;
        public bool Activo { get; set; } = true;
    }
}


