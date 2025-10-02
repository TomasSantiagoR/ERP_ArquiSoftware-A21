using System.ComponentModel.DataAnnotations;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERP_ArquiSoftware.Pages.Maestros.Marcas
{
    public class UpsertModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public UpsertModel(AppDBContext ctx) => _ctx = ctx;

        public class MarcaInput
        {
            public int Id { get; set; }
            [Required, StringLength(100)] public string Nombre { get; set; } = "";
            public bool Activa { get; set; } = true;
        }

        [BindProperty] public MarcaInput Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null) return Page();
            var e = await _ctx.Marcas.FindAsync(id.Value);
            if (e is null) return NotFound();
            Input = new MarcaInput { Id = e.Id, Nombre = e.Nombre, Activa = e.Activa };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (Input.Id == 0)
            {
                _ctx.Marcas.Add(new Marca { Nombre = Input.Nombre, Activa = Input.Activa });
            }
            else
            {
                var e = await _ctx.Marcas.FindAsync(Input.Id);
                if (e is null) return NotFound();
                e.Nombre = Input.Nombre;
                e.Activa = Input.Activa;
            }
            await _ctx.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
