using ERP_ArquiSoftware.dA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Maestros.Departamentos
{
    public class IndexModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public IndexModel(AppDBContext ctx) => _ctx = ctx;

        public record Row(int Id, string Nombre, int EmpleadosCount);
        public List<Row> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        public async Task OnGetAsync()
        {
            var q = _ctx.Departamentos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                var f = Q.Trim();
                q = q.Where(d => d.Nombre.Contains(f));
            }

            Items = await q
                .OrderBy(d => d.Nombre)
                .Select(d => new Row(
                    d.Id,
                    d.Nombre,
                    d.Empleados.Count))
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Evitar borrar si tiene empleados
            var tieneEmpleados = await _ctx.Empleados.AnyAsync(e => e.DepartamentoId == id);
            if (tieneEmpleados)
            {
                ModelState.AddModelError(string.Empty, "No se puede eliminar: el departamento tiene empleados asociados.");
                await OnGetAsync(); // recargar lista
                return Page();
            }

            var dep = await _ctx.Departamentos.FindAsync(id);
            if (dep == null) return NotFound();

            _ctx.Departamentos.Remove(dep);
            await _ctx.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
