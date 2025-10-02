using ERP_ArquiSoftware.dA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Facturacion.Facturas
{
    public class IndexModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public IndexModel(AppDBContext ctx) => _ctx = ctx;

        public record Row(int Id, DateTime Fecha, string Serie, int Numero, string Cliente, string Almacen, decimal Total, string Estado);
        public List<Row> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)] public string? Q { get; set; }

        public async Task OnGetAsync()
        {
            var q = _ctx.FacturasVenta
                .AsNoTracking()
                .Include(f => f.Cliente)
                .Include(f => f.Almacen)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                var f = Q.Trim();
                q = q.Where(x =>
                    x.Cliente.NombreRazonSocial.Contains(f) ||
                    x.Serie.Contains(f) ||
                    x.Numero.ToString().Contains(f));
            }

            Items = await q
                .OrderByDescending(x => x.Fecha)
                .Select(x => new Row(
                    x.Id,
                    x.Fecha, x.Serie, x.Numero,
                    x.Cliente.NombreRazonSocial,
                    x.Almacen.Nombre,
                    x.Total,
                    x.Estado))
                .ToListAsync();
        }
    }
}

