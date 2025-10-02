using System.ComponentModel.DataAnnotations;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.RRHH;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Maestros.Cargos
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

            [StringLength(500)]
            public string? Descripcion { get; set; }

            [Range(0, double.MaxValue)]
            public decimal? SalarioBase { get; set; }
        }

        [BindProperty] public Vm Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var c = await _ctx.Cargos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value);
                if (c == null) return NotFound();

                Input = new Vm
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    SalarioBase = c.SalarioBase
                };
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Unicidad por nombre
            var existe = await _ctx.Cargos
                .AnyAsync(c => c.Nombre == Input.Nombre && c.Id != Input.Id);
            if (existe)
            {
                ModelState.AddModelError(nameof(Input.Nombre), "Ya existe un cargo con ese nombre.");
                return Page();
            }

            if (Input.Id == 0)
            {
                _ctx.Cargos.Add(new Cargo
                {
                    Nombre = Input.Nombre,
                    Descripcion = Input.Descripcion,
                    SalarioBase = Input.SalarioBase
                });
            }
            else
            {
                var c = await _ctx.Cargos.FindAsync(Input.Id);
                if (c == null) return NotFound();

                c.Nombre = Input.Nombre;
                c.Descripcion = Input.Descripcion;
                c.SalarioBase = Input.SalarioBase;
            }

            await _ctx.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}

