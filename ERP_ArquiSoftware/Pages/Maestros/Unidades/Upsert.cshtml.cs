using System.ComponentModel.DataAnnotations;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERP_ArquiSoftware.Pages.Maestros.Unidades
{
    public class UpsertModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public UpsertModel(AppDBContext ctx) => _ctx = ctx;

        public class UnidadInput
        {
            public int Id { get; set; }
            [Required, StringLength(10)] public string Codigo { get; set; } = "";
            [Required, StringLength(50)] public string Descripcion { get; set; } = "";
            public bool Activa { get; set; } = true;
        }
        [BindProperty] public UnidadInput Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null) return Page();
            var e = await _ctx.UnidadesMedida.FindAsync(id.Value);
            if (e is null) return NotFound();
            Input = new UnidadInput { Id = e.Id, Codigo = e.Codigo, Descripcion = e.Descripcion, Activa = e.Activa };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (Input.Id == 0)
            {
                _ctx.UnidadesMedida.Add(new UnidadMedida { Codigo = Input.Codigo, Descripcion = Input.Descripcion, Activa = Input.Activa });
            }
            else
            {
                var e = await _ctx.UnidadesMedida.FindAsync(Input.Id);
                if (e is null) return NotFound();
                e.Codigo = Input.Codigo; e.Descripcion = Input.Descripcion; e.Activa = Input.Activa;
            }
            await _ctx.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
