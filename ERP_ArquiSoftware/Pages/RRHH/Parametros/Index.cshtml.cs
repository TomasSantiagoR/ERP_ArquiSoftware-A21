using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.RRHH;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.RRHH.Parametros
{
    public class IndexModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public IndexModel(AppDBContext ctx) => _ctx = ctx;

        [BindProperty] public ParamNomina Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Trae el activo más reciente, o crea uno
            var p = await _ctx.ParametrosNomina
                .OrderByDescending(x => x.Activo)
                .ThenByDescending(x => x.Anio)
                .FirstOrDefaultAsync();

            if (p == null)
            {
                p = new ParamNomina { Activo = true, Anio = DateTime.Now.Year };
                _ctx.ParametrosNomina.Add(p);
                await _ctx.SaveChangesAsync();
            }

            Input = p;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var p = await _ctx.ParametrosNomina.FirstOrDefaultAsync(x => x.Id == Input.Id);
            if (p == null) return NotFound();

            p.Anio = Input.Anio;
            p.SMMLV = Input.SMMLV;
            p.AuxilioTransporte = Input.AuxilioTransporte;
            p.TopeAuxTranspMultiplo = Input.TopeAuxTranspMultiplo;

            p.SaludEmpleadoPorc = Input.SaludEmpleadoPorc;
            p.PensionEmpleadoPorc = Input.PensionEmpleadoPorc;

            p.CesantiasPorc = Input.CesantiasPorc;
            p.InteresesCesantiasAnualPorc = Input.InteresesCesantiasAnualPorc;
            p.PrimaPorc = Input.PrimaPorc;
            p.VacacionesPorc = Input.VacacionesPorc;

            p.DiasMes = Input.DiasMes;
            p.Activo = Input.Activo;

            await _ctx.SaveChangesAsync();
            TempData["ok"] = "Parámetros actualizados.";
            return RedirectToPage();
        }
    }
}

