using ERP_ArquiSoftware.dA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Ventas.CondicionesPago
{
    public class IndexModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public IndexModel(AppDBContext ctx) => _ctx = ctx;

        public record Row(int Id, string Nombre, int DiasPlazo, int ClientesCount);
        public List<Row> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        public async Task OnGetAsync()
        {
            var q = _ctx.CondicionesPago.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                var f = Q.Trim();
                q = q.Where(c => c.Nombre.Contains(f));
            }

            Items = await q.OrderBy(c => c.DiasPlazo).ThenBy(c => c.Nombre)
                .Select(c => new Row(c.Id, c.Nombre, c.DiasPlazo, c.Clientes.Count))
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var enUso = await _ctx.Clientes.AnyAsync(c => c.CondicionPagoId == id);
            if (enUso)
            {
                ModelState.AddModelError(string.Empty, "No se puede eliminar: hay clientes usando esta condición.");
                await OnGetAsync();
                return Page();
            }

            var item = await _ctx.CondicionesPago.FindAsync(id);
            if (item == null) return NotFound();

            _ctx.CondicionesPago.Remove(item);
            await _ctx.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
