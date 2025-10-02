using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.RRHH
{
    public class TipoContrato
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Nombre { get; set; } = string.Empty; // Ej: Indefinido, Fijo, Prestación de servicios

        public ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
    }
}
