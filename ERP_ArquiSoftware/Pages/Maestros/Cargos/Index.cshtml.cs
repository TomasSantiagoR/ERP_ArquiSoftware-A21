using ERP_ArquiSoftware.dA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Maestros.Cargos
{
    public class IndexModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public IndexModel(AppDBContext ctx) => _ctx = ctx;

        public record Row(int Id, string Nombre, string? Descripcion, decimal? SalarioBase, int EmpleadosCount);
        public List<Row> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        public async Task OnGetAsync()
        {
            var q = _ctx.Cargos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                var f = Q.Trim();
                q = q.Where(c => c.Nombre.Contains(f) || (c.Descripcion != null && c.Descripcion.Contains(f)));
            }

            Items = await q
                .OrderBy(c => c.Nombre)
                .Select(c => new Row(
                    c.Id,
                    c.Nombre,
                    c.Descripcion,
                    c.SalarioBase,
                    c.Empleados.Count))
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var tieneEmpleados = await _ctx.Empleados.AnyAsync(e => e.CargoId == id);
            if (tieneEmpleados)
            {
                ModelState.AddModelError(string.Empty, "No se puede eliminar: el cargo tiene empleados asociados.");
                await OnGetAsync();
                return Page();
            }

            var cargo = await _ctx.Cargos.FindAsync(id);
            if (cargo == null) return NotFound();

            _ctx.Cargos.Remove(cargo);
            await _ctx.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}

