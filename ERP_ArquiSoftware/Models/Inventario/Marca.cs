using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.Inventario
{
    public class Marca
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string Nombre { get; set; } = "";
        public bool Activa { get; set; } = true;
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
