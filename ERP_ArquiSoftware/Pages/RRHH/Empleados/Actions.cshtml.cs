using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.RRHH;
using ERP_ArquiSoftware.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.RRHH.Empleados
{
    public class ActionsModel : PageModel
    {
        private readonly AppDBContext _ctx;
        private readonly INominaService _nomina;
        public ActionsModel(AppDBContext ctx, INominaService nomina)
        {
            _ctx = ctx;
            _nomina = nomina;
        }

        public Empleado? Empleado { get; set; }

        // Inputs Nómina
        [BindProperty] public int Anio { get; set; } = DateTime.Now.Year;
        [BindProperty] public int Mes { get; set; } = DateTime.Now.Month;
        [BindProperty] public decimal OtrosDevengos { get; set; } = 0m;
        [BindProperty] public decimal OtrasDeducciones { get; set; } = 0m;

        // Inputs Liquidación
        [BindProperty] public DateTime FechaFin { get; set; } = DateTime.Today;

        // Resultados
        public Nomina? NominaGenerada { get; set; }
        public Liquidacion? LiquidacionGenerada { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Empleado = await _ctx.Empleados
                .Include(e => e.Contratos.Where(c => c.Activo))
                .ThenInclude(c => c.TipoContrato)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (Empleado == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostCalcularNominaAsync(int id)
        {
            await OnGetAsync(id);
            if (Empleado == null) return NotFound();

            try
            {
                NominaGenerada = await _nomina.CalcularYGuardarNominaAsync(
                    empleadoId: id,
                    anio: Anio,
                    mes: Mes,
                    otrosDevengos: OtrosDevengos,
                    otrasDeducciones: OtrasDeducciones);

                TempData["ok"] = $"Nómina generada (ID {NominaGenerada.Id}) por {NominaGenerada.NetoPagar:C0}.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLiquidarAsync(int id)
        {
            await OnGetAsync(id);
            if (Empleado == null) return NotFound();

            try
            {
                LiquidacionGenerada = await _nomina.GenerarYGuardarLiquidacionAsync(
                    empleadoId: id,
                    fechaFinContrato: FechaFin,
                    otrasDeducciones: OtrasDeducciones);

                TempData["ok"] = $"Liquidación generada (ID {LiquidacionGenerada.Id}) por {LiquidacionGenerada.NetoPagar:C0}.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return Page();
        }
    }
}


