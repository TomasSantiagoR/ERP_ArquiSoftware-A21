using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.RRHH;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.RRHH.Empleados
{
    public class CreateModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public CreateModel(AppDBContext ctx) => _ctx = ctx;

        [BindProperty] public Empleado Empleado { get; set; } = new();

        public List<SelectListItem> AlmacenesSelect { get; set; } = new();
        public List<SelectListItem> DepartamentosSelect { get; set; } = new();
        public List<SelectListItem> CargosSelect { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await CargarSelectsAsync();
            // FechaIngreso default hoy si viene vacío
            if (Empleado.FechaIngreso == default) Empleado.FechaIngreso = DateTime.Today;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CargarSelectsAsync();

            // Validaciones de existencia
            if (!await _ctx.Almacenes.AnyAsync(a => a.Id == Empleado.AlmacenId && a.Activo))
                ModelState.AddModelError(nameof(Empleado.AlmacenId), "Almacén no válido o inactivo.");
            if (!await _ctx.Departamentos.AnyAsync(d => d.Id == Empleado.DepartamentoId))
                ModelState.AddModelError(nameof(Empleado.DepartamentoId), "Departamento no válido.");
            if (!await _ctx.Cargos.AnyAsync(c => c.Id == Empleado.CargoId))
                ModelState.AddModelError(nameof(Empleado.CargoId), "Cargo no válido.");

            // Documento único (opcional, pero recomendable)
            var docEnUso = await _ctx.Empleados.AnyAsync(e => e.Documento == Empleado.Documento);
            if (docEnUso)
                ModelState.AddModelError(nameof(Empleado.Documento), "Ya existe un empleado con ese documento.");

            if (!ModelState.IsValid) return Page();

            if (Empleado.FechaIngreso == default)
                Empleado.FechaIngreso = DateTime.Today;

            _ctx.Empleados.Add(Empleado);
            await _ctx.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        private async Task CargarSelectsAsync()
        {
            AlmacenesSelect = await _ctx.Almacenes
                .Where(a => a.Activo)
                .OrderBy(a => a.Nombre)
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Nombre })
                .ToListAsync();

            DepartamentosSelect = await _ctx.Departamentos
                .OrderBy(d => d.Nombre)
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nombre })
                .ToListAsync();

            CargosSelect = await _ctx.Cargos
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nombre })
                .ToListAsync();
        }
    }
}
