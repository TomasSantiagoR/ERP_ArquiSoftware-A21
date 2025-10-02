using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Ventas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Ventas.Clientes
{
    public class DireccionesModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public DireccionesModel(AppDBContext ctx) => _ctx = ctx;

        [BindProperty(SupportsGet = true)]
        public int ClienteId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        public Cliente? Cliente { get; set; }
        public List<DireccionCliente> Items { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Cliente = await _ctx.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == ClienteId);
            if (Cliente == null) return NotFound();

            var q = _ctx.DireccionesCliente.AsNoTracking().Where(d => d.ClienteId == ClienteId).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                var f = Q.Trim();
                q = q.Where(d =>
                    d.Direccion.Contains(f) ||
                    (d.Ciudad != null && d.Ciudad.Contains(f)) ||
                    (d.Departamento != null && d.Departamento.Contains(f)) ||
                    (d.CodigoPostal != null && d.CodigoPostal.Contains(f)));
            }

            Items = await q.OrderBy(d => d.Tipo).ThenBy(d => d.Direccion).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id, int clienteId)
        {
            var dir = await _ctx.DireccionesCliente.FindAsync(id);
            if (dir == null || dir.ClienteId != clienteId) return NotFound();

            _ctx.DireccionesCliente.Remove(dir);
            await _ctx.SaveChangesAsync();
            return RedirectToPage(new { clienteId });
        }
    }
}

