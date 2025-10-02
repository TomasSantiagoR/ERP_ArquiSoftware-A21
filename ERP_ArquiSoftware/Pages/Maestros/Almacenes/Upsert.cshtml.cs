using System.ComponentModel.DataAnnotations;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERP_ArquiSoftware.Pages.Maestros.Almacenes
{
    public class UpsertModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public UpsertModel(AppDBContext ctx) => _ctx = ctx;

        public class AlmacenInput
        {
            public int Id { get; set; }
            [Required, StringLength(100)] public string Nombre { get; set; } = "";
            [StringLength(200)] public string? Direccion { get; set; }
            public bool Activo { get; set; } = true;
        }
        [BindProperty] public AlmacenInput Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null) return Page();
            var e = await _ctx.Almacenes.FindAsync(id.Value);
            if (e is null) return NotFound();
            Input = new AlmacenInput { Id = e.Id, Nombre = e.Nombre, Direccion = e.Direccion, Activo = e.Activo };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (Input.Id == 0)
                _ctx.Almacenes.Add(new Almacen { Nombre = Input.Nombre, Direccion = Input.Direccion, Activo = Input.Activo });
            else
            {
                var e = await _ctx.Almacenes.FindAsync(Input.Id);
                if (e is null) return NotFound();
                e.Nombre = Input.Nombre; e.Direccion = Input.Direccion; e.Activo = Input.Activo;
            }
            await _ctx.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
