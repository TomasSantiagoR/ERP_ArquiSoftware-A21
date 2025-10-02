using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Inventario
{
    public class IndexInvModel : PageModel
    {
        private readonly AppDBContext _context;

        public IndexInvModel(AppDBContext context)
        {
            _context = context;
        }

        public IList<ProductoListadoDTO> Productos { get; set; } = new List<ProductoListadoDTO>();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Productos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.UnidadMedida)
                .Include(p => p.Impuesto)
                .Include(p => p.Existencias) // para calcular stock
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                var q = Q.Trim();
                query = query.Where(p =>
                    p.NombreProducto.Contains(q) ||
                    p.Sku.Contains(q) ||
                    p.Descripcion.Contains(q) ||
                    (p.Categoria != null && p.Categoria.Nombre.Contains(q)) ||
                    (p.Marca != null && p.Marca.Nombre.Contains(q))
                );
            }

            Productos = await query
                .OrderBy(p => p.NombreProducto)
                .Select(p => new ProductoListadoDTO
                {
                    Id = p.Id,
                    NombreProducto = p.NombreProducto,
                    Sku = p.Sku,
                    Categoria = p.Categoria != null ? p.Categoria.Nombre : "—",
                    Marca = p.Marca != null ? p.Marca.Nombre : "—",
                    Unidad = p.UnidadMedida != null ? p.UnidadMedida.Codigo : "",
                    Precio = p.PrecioUnitario,
                    Impuesto = p.Impuesto != null ? p.Impuesto.Nombre : "",
                    StockActual = p.Existencias.Sum(e => e.Stock),
                    StockMin = p.StockMin,
                    Activo = p.Activo,
                    FechaCreacion = p.FechaCreacion
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        // DTO para la tabla
        public class ProductoListadoDTO
        {
            public int Id { get; set; }
            public string NombreProducto { get; set; } = "";
            public string Sku { get; set; } = "";
            public string Categoria { get; set; } = "";
            public string Marca { get; set; } = "";
            public string Unidad { get; set; } = "";
            public decimal Precio { get; set; }
            public string Impuesto { get; set; } = "";
            public int StockActual { get; set; }
            public int StockMin { get; set; }
            public bool Activo { get; set; }
            public DateTime FechaCreacion { get; set; }
        }
    }
}
