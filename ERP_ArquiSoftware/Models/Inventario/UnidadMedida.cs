using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.Inventario
{
    public class UnidadMedida
    {
        public int Id { get; set; }
        [Required, StringLength(10)] public string Codigo { get; set; } = "UN"; // UN, KG, LT
        [Required, StringLength(50)] public string Descripcion { get; set; } = "Unidad";
        public bool Activa { get; set; } = true;
    }
}
