using System.ComponentModel.DataAnnotations;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.RRHH;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.RRHH.Contratos
{
    public class CreateModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public CreateModel(AppDBContext ctx) => _ctx = ctx;

        public class Vm
        {
            [Required] public int? EmpleadoId { get; set; }
            [Required] public int? TipoContratoId { get; set; }

            [Required] public DateTime FechaInicio { get; set; } = DateTime.Today;
            public DateTime? FechaFin { get; set; }

            [Range(0, double.MaxValue)] public decimal Salario { get; set; }
            public bool Activo { get; set; } = true;
        }

        [BindProperty] public Vm Input { get; set; } = new();

        public List<SelectListItem> EmpleadosSelect { get; set; } = new();
        public List<SelectListItem> TiposSelect { get; set; } = new();

        public async Task OnGetAsync()
        {
            await CargarSelects();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CargarSelects();
            if (!ModelState.IsValid) return Page();

            // Desactivar contratos activos previos del empleado (si se marca Activo)
            if (Input.Activo && Input.EmpleadoId.HasValue)
            {
                var prev = await _ctx.Contratos
                    .Where(c => c.EmpleadoId == Input.EmpleadoId.Value && c.Activo)
                    .ToListAsync();
                foreach (var c in prev) c.Activo = false;
            }

            _ctx.Contratos.Add(new Contrato
            {
                EmpleadoId = Input.EmpleadoId!.Value,
                TipoContratoId = Input.TipoContratoId!.Value,
                FechaInicio = Input.FechaInicio,
                FechaFin = Input.FechaFin,
                Salario = Input.Salario,
                Activo = Input.Activo
            });

            await _ctx.SaveChangesAsync();
            return RedirectToPage("/RRHH/Empleados/Index");
        }

        private async Task CargarSelects()
        {
            EmpleadosSelect = await _ctx.Empleados
                .OrderBy(e => e.Nombres)
                .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = $"{e.Nombres} {e.Apellidos} ({e.Documento})" })
                .ToListAsync();

            TiposSelect = await _ctx.TiposContrato
                .OrderBy(t => t.Nombre)
                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Nombre })
                .ToListAsync();
        }
    }
}

