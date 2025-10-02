using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Maestros.Impuestos
{
    public class IndexModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public IndexModel(AppDBContext ctx) => _ctx = ctx;

        public List<Impuesto> Items { get; set; } = new();
        [BindProperty(SupportsGet = true)] public string? Q { get; set; }

        public async Task OnGet()
        {
            var q = _ctx.Impuestos.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(Q)) q = q.Where(x => x.Nombre.Contains(Q.Trim()));
            Items = await q.OrderBy(x => x.Porcentaje).ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var e = await _ctx.Impuestos.FindAsync(id);
            if (e == null) return NotFound();
            _ctx.Impuestos.Remove(e);
            await _ctx.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
