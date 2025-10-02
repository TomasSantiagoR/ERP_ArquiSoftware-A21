using System.ComponentModel.DataAnnotations;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Ventas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Ventas.CondicionesPago
{
    public class UpsertModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public UpsertModel(AppDBContext ctx) => _ctx = ctx;

        public class Vm
        {
            public int Id { get; set; }
            [Required, StringLength(80)]
            public string Nombre { get; set; } = "";
            [Range(0, 365)]
            public int DiasPlazo { get; set; } = 0;
        }

        [BindProperty] public Vm Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var c = await _ctx.CondicionesPago.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value);
                if (c == null) return NotFound();

                Input = new Vm { Id = c.Id, Nombre = c.Nombre, DiasPlazo = c.DiasPlazo };
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var nombreEnUso = await _ctx.CondicionesPago
                .AnyAsync(x => x.Nombre == Input.Nombre && x.Id != Input.Id);
            if (nombreEnUso)
            {
                ModelState.AddModelError(nameof(Input.Nombre), "Ya existe una condición con ese nombre.");
                return Page();
            }

            if (Input.Id == 0)
            {
                _ctx.CondicionesPago.Add(new CondicionPago
                {
                    Nombre = Input.Nombre,
                    DiasPlazo = Input.DiasPlazo
                });
            }
            else
            {
                var c = await _ctx.CondicionesPago.FindAsync(Input.Id);
                if (c == null) return NotFound();
                c.Nombre = Input.Nombre;
                c.DiasPlazo = Input.DiasPlazo;
            }

            await _ctx.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}

