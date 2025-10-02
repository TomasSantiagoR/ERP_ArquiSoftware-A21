using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Maestros.Almacenes
{
    public class IndexModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public IndexModel(AppDBContext ctx) => _ctx = ctx;

        public List<Almacen> Items { get; set; } = new();
        [BindProperty(SupportsGet = true)] public string? Q { get; set; }

        public async Task OnGet()
        {
            var q = _ctx.Almacenes.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(Q))
                q = q.Where(x => x.Nombre.Contains(Q.Trim()) || (x.Direccion ?? "").Contains(Q.Trim()));
            Items = await q.OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var e = await _ctx.Almacenes.FindAsync(id);
            if (e == null) return NotFound();
            _ctx.Almacenes.Remove(e);
            await _ctx.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
