using ERP_ArquiSoftware.dA;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Inventario
{
    public class HomeInvModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public HomeInvModel(AppDBContext ctx) => _ctx = ctx;

        public ResumenVm Resumen { get; set; } = new();

        public class ResumenVm
        {
            public int ProductosActivos { get; set; }
            public int ProductosInactivos { get; set; }
            public int ConStockBajo { get; set; }
            public int TotalAlmacenes { get; set; }
            public int TotalCategorias { get; set; }
            public decimal ValorInventario { get; set; }

            public List<LowStockRow> LowStock { get; set; } = new();
            public List<CatRow> PorCategoria { get; set; } = new();
        }

        public record LowStockRow(int Id, string Nombre, string? Categoria, int Stock, int? PuntoReorden);
        public record CatRow(string Categoria, int Cantidad);

        public async Task OnGetAsync()
        {
            // Productos base
            var productos = await _ctx.Productos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .ToListAsync();

            // Contadores simples
            Resumen.ProductosActivos = productos.Count(p => p.Activo);
            Resumen.ProductosInactivos = productos.Count(p => !p.Activo);
            Resumen.TotalCategorias = await _ctx.Categorias.AsNoTracking().CountAsync();
            Resumen.TotalAlmacenes = await _ctx.Almacenes.AsNoTracking().CountAsync();

            // -------- Stock efectivo por producto (Existencias si hay, si no Producto.Stock)
            Dictionary<int, int> stockPorProducto;
            var hayExistencias = await _ctx.Existencias.AsNoTracking().AnyAsync();

            if (hayExistencias)
            {
                stockPorProducto = await _ctx.Existencias
                    .AsNoTracking()
                    .GroupBy(e => e.ProductoId)
                    .Select(g => new { g.Key, Stock = g.Sum(x => x.Stock) })
                    .ToDictionaryAsync(x => x.Key, x => x.Stock);
            }
            else
            {
                // Fallback a campo Stock del producto
                stockPorProducto = productos.ToDictionary(p => p.Id, p => p.Stock);
            }

            int StockDe(int productoId, int fallback) =>
                stockPorProducto.TryGetValue(productoId, out var s) ? s : fallback;

            // -------- Valor inventario = Σ (precio * stock efectivo)
            decimal totalInv = 0m;
            foreach (var p in productos)
                totalInv += p.PrecioUnitario * StockDe(p.Id, p.Stock);
            Resumen.ValorInventario = totalInv;

            // -------- Top “stock bajo” usando stock efectivo
            var low = productos
                .Where(p => p.Activo && p.PuntoReorden.HasValue)
                .Select(p => new
                {
                    P = p,
                    StockEf = StockDe(p.Id, p.Stock),
                    Reorden = p.PuntoReorden!.Value
                })
                .Where(x => x.StockEf <= x.Reorden)
                .OrderBy(x => x.StockEf - x.Reorden) // más crítico primero
                .ThenBy(x => x.StockEf)
                .Take(8)
                .Select(x => new LowStockRow(
                    x.P.Id,
                    x.P.NombreProducto,
                    x.P.Categoria?.Nombre,
                    x.StockEf,
                    x.Reorden
                ))
                .ToList();

            Resumen.LowStock = low;
            Resumen.ConStockBajo = low.Count;

            // -------- Distribución por categoría (LEFT JOIN traducible)
            Resumen.PorCategoria = await
                (from p in _ctx.Productos.AsNoTracking()
                 where p.Activo
                 join c in _ctx.Categorias.AsNoTracking()
                     on p.CategoriaId equals c.Id into gj
                 from c in gj.DefaultIfEmpty()
                 group p by (c != null ? c.Nombre : "Sin categoría") into g
                 orderby g.Count() descending
                 select new CatRow(g.Key, g.Count()))
                .Take(10)
                .ToListAsync();
        }
    }
}
