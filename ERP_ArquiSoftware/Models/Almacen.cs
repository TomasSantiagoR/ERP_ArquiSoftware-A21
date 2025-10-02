using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ERP_ArquiSoftware.Models.Inventario;

namespace ERP_ArquiSoftware.Models
{
    public class Almacen
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string Nombre { get; set; } = "";
        [StringLength(200)] public string? Direccion { get; set; }
        public bool Activo { get; set; } = true;
        public ICollection<Existencia> Existencias { get; set; } = new List<Existencia>();
    }
}
