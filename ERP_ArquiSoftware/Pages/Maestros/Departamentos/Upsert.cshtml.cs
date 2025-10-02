using System.ComponentModel.DataAnnotations;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.RRHH;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Maestros.Departamentos
{
    public class UpsertModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public UpsertModel(AppDBContext ctx) => _ctx = ctx;

        public class Vm
        {
            public int Id { get; set; }
            [Required, StringLength(100)]
            public string Nombre { get; set; } = string.Empty;
        }

        [BindProperty] public Vm Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var d = await _ctx.Departamentos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value);
                if (d == null) return NotFound();
                Input = new Vm { Id = d.Id, Nombre = d.Nombre };
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Unicidad por nombre
            var existe = await _ctx.Departamentos
                .AnyAsync(d => d.Nombre == Input.Nombre && d.Id != Input.Id);
            if (existe)
            {
                ModelState.AddModelError(nameof(Input.Nombre), "Ya existe un departamento con ese nombre.");
                return Page();
            }

            if (Input.Id == 0)
            {
                _ctx.Departamentos.Add(new Departamento { Nombre = Input.Nombre });
            }
            else
            {
                var d = await _ctx.Departamentos.FindAsync(Input.Id);
                if (d == null) return NotFound();
                d.Nombre = Input.Nombre;
            }

            await _ctx.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
