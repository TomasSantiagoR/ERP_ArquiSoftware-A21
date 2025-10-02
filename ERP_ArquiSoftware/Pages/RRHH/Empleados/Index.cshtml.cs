using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.RRHH;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ERP_ArquiSoftware.Pages.RRHH.Empleados
{
    public class IndexModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public IndexModel(AppDBContext ctx) => _ctx = ctx;

        public List<Empleado> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        public async Task OnGetAsync()
        {
            var query = _ctx.Empleados
                .AsNoTracking()
                .Include(e => e.Almacen)
                .Include(e => e.Departamento)
                .Include(e => e.Cargo)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                var f = Q.Trim();
                query = query.Where(e =>
                    e.Nombres.Contains(f) || e.Apellidos.Contains(f) ||
                    e.Documento.Contains(f) || e.Correo.Contains(f) ||
                    (e.Almacen != null && e.Almacen.Nombre.Contains(f)));
            }

            Items = await query
                .OrderBy(e => e.Nombres)
                .ThenBy(e => e.Apellidos)
                .ToListAsync();
        }
    }
}

