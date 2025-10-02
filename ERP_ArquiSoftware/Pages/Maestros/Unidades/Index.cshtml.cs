using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Maestros.Unidades
{
    public class IndexModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public IndexModel(AppDBContext ctx) => _ctx = ctx;

        public List<UnidadMedida> Items { get; set; } = new();
        [BindProperty(SupportsGet = true)] public string? Q { get; set; }

        public async Task OnGet()
        {
            var q = _ctx.UnidadesMedida.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(Q))
                q = q.Where(x => x.Codigo.Contains(Q.Trim()) || x.Descripcion.Contains(Q.Trim()));
            Items = await q.OrderBy(x => x.Codigo).ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var e = await _ctx.UnidadesMedida.FindAsync(id);
            if (e == null) return NotFound();
            _ctx.UnidadesMedida.Remove(e);
            await _ctx.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
