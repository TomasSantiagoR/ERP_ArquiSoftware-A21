using System.ComponentModel.DataAnnotations;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using ERP_ArquiSoftware.Models.Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERP_ArquiSoftware.Pages.Maestros.Impuestos
{
    public class UpsertModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public UpsertModel(AppDBContext ctx) => _ctx = ctx;

        public class ImpuestoInput
        {
            public int Id { get; set; }
            [Required, StringLength(50)] public string Nombre { get; set; } = "";
            [Range(0, 100)] public decimal Porcentaje { get; set; }
            public bool IncluidoEnPrecio { get; set; } = false;
            public bool Activo { get; set; } = true;
        }
        [BindProperty] public ImpuestoInput Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null) return Page();
            var e = await _ctx.Impuestos.FindAsync(id.Value);
            if (e is null) return NotFound();
            Input = new ImpuestoInput
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Porcentaje = e.Porcentaje,
                IncluidoEnPrecio = e.IncluidoEnPrecio,
                Activo = e.Activo
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (Input.Id == 0)
            {
                _ctx.Impuestos.Add(new Impuesto
                {
                    Nombre = Input.Nombre,
                    Porcentaje = Input.Porcentaje,
                    IncluidoEnPrecio = Input.IncluidoEnPrecio,
                    Activo = Input.Activo
                });
            }
            else
            {
                var e = await _ctx.Impuestos.FindAsync(Input.Id);
                if (e is null) return NotFound();
                e.Nombre = Input.Nombre;
                e.Porcentaje = Input.Porcentaje;
                e.IncluidoEnPrecio = Input.IncluidoEnPrecio;
                e.Activo = Input.Activo;
            }
            await _ctx.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
