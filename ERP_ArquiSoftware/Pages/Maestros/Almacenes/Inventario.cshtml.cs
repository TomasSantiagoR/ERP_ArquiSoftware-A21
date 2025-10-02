using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using ERP_ArquiSoftware.Models.Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Maestros.Almacenes
{
    public class InventarioModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public InventarioModel(AppDBContext ctx) => _ctx = ctx;

        // Ruta /Maestros/Almacenes/Inventario/{id}
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        public int AlmacenId => Id;

        public Almacen? Almacen { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        public List<FilaInventario> Items { get; set; } = new();

        // Totales
        public int TotalSkus { get; set; }
        public int TotalUnidades { get; set; }
        public decimal TotalValorCosto { get; set; }
        public decimal TotalValorPvp { get; set; }

        public class FilaInventario
        {
            public int ProductoId { get; set; }
            public string Sku { get; set; } = "";
            public string Nombre { get; set; } = "";
            public string Categoria { get; set; } = "";
            public int Stock { get; set; }
            public int StockMin { get; set; }
            public int? PuntoReorden { get; set; }
            public decimal? Costo { get; set; }
            public decimal Precio { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Cargar almacén
            Almacen = await _ctx.Almacenes.AsNoTracking().FirstOrDefaultAsync(a => a.Id == AlmacenId);
            if (Almacen is null) return NotFound();

            // Query de existencias del almacén, con datos de producto y categoría
            var q = _ctx.Existencias
                .AsNoTracking()
                .Where(e => e.AlmacenId == AlmacenId)
                .Include(e => e.Producto)
                    .ThenInclude(p => p.Categoria)
                .Select(e => new FilaInventario
                {
                    ProductoId = e.ProductoId,
                    Sku = e.Producto.Sku,
                    Nombre = e.Producto.NombreProducto,
                    Categoria = e.Producto.Categoria != null ? e.Producto.Categoria.Nombre : "",
                    Stock = e.Stock,
                    StockMin = e.Producto.StockMin,
                    PuntoReorden = e.Producto.PuntoReorden,
                    Costo = e.Producto.UltimoCosto ?? e.Producto.CostoPromedio ?? e.Producto.CostoEstandar,
                    Precio = e.Producto.PrecioUnitario
                })
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                var f = Q.Trim();
                q = q.Where(r =>
                    r.Sku.Contains(f) ||
                    r.Nombre.Contains(f) ||
                    r.Categoria.Contains(f));
            }

            Items = await q
                .OrderBy(r => r.Nombre)
                .ToListAsync();

            // Totales
            TotalSkus = Items.Count;
            TotalUnidades = Items.Sum(i => i.Stock);
            TotalValorCosto = Items.Sum(i => (i.Costo ?? 0m) * i.Stock);
            TotalValorPvp = Items.Sum(i => i.Precio * i.Stock);

            return Page();
        }
    }
}
