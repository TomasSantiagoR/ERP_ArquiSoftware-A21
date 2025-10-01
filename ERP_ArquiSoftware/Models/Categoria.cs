using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        [DisplayName("Categoría")]
        public string Nombre { get; set; } = "";

        public bool Activa { get; set; } = true;

        // Navegación inversa (opcional)
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
