using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Ventas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Ventas.Pedidos
{
    public class IndexModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public IndexModel(AppDBContext ctx) => _ctx = ctx;

        public IList<PedidoVenta> Items { get; set; } = new List<PedidoVenta>();

        // Búsqueda
        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        public async Task OnGetAsync()
        {
            var query = _ctx.PedidosVenta
                .AsNoTracking()
                .Include(p => p.Cliente)
                .Include(p => p.Almacen)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                var f = Q.Trim();

                // Intento de parse "SERIE-NUMERO" (p.ej. "PV-15")
                string? serieParsed = null;
                int? numeroParsed = null;
                if (f.Contains('-'))
                {
                    var parts = f.Split('-', 2, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 1) serieParsed = parts[0].Trim();
                    if (parts.Length == 2 && int.TryParse(parts[1], out var n)) numeroParsed = n;
                }

                query = query.Where(p =>
                    p.Cliente.NombreRazonSocial.Contains(f) ||
                    p.Estado.Contains(f) ||
                    (serieParsed != null && p.Serie.Contains(serieParsed)) ||
                    (numeroParsed != null && p.Numero == numeroParsed) ||
                    (p.Serie + "-" + p.Numero).Contains(f)
                );
            }

            Items = await query
                .OrderByDescending(p => p.Fecha)
                .ThenByDescending(p => p.Id)
                .ToListAsync();
        }
    }
}

