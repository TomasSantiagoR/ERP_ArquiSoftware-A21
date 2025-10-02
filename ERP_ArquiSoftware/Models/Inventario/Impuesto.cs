using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.Inventario
{
    public class Impuesto
    {
        public int Id { get; set; }
        [Required, StringLength(50)] public string Nombre { get; set; } = "IVA 19%";
        [Range(0, 100)] public decimal Porcentaje { get; set; } = 19m;
        public bool IncluidoEnPrecio { get; set; } = false; // precio con IVA incluido o no
        public bool Activo { get; set; } = true;
    }
}
