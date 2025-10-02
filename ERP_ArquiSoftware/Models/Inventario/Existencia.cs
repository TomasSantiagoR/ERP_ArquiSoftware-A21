using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.Inventario
{
    public class Existencia
    {
        public int ProductoId { get; set; }
        public int AlmacenId { get; set; }
        [Range(0, int.MaxValue)] public int Stock { get; set; }
        public Producto Producto { get; set; } = null!;
        public Almacen Almacen { get; set; } = null!;
    }
}
