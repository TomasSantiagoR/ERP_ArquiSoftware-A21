using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Proveedores
{
    public class IndexProvModel : PageModel
    {
        private readonly AppDBContext _context;
        public IndexProvModel(AppDBContext context) => _context = context;

        public List<Proveedor> Proveedores { get; set; } = new();
        public string? Q { get; set; }

        public async Task OnGetAsync(string? q)
        {
            Q = q?.Trim();

            var query = _context.Proveedores
                .Include(p => p.Categoria)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                query = query.Where(p =>
                    p.NombreRazonSocial.Contains(Q) ||
                    p.NIT.Contains(Q));
            }

            Proveedores = await query
                .OrderBy(p => p.NombreRazonSocial)
                .ToListAsync();
        }
    }
}

