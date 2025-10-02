using System.ComponentModel.DataAnnotations;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Ventas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Ventas.Clientes
{
    public class UpsertDireccionModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public UpsertDireccionModel(AppDBContext ctx) => _ctx = ctx;

        [BindProperty(SupportsGet = true)]
        public int ClienteId { get; set; }

        public class Vm
        {
            public int Id { get; set; }
            [Required, StringLength(20)]
            public string Tipo { get; set; } = "Entrega";
            [Required, StringLength(250)]
            public string Direccion { get; set; } = "";
            [StringLength(100)] public string? Ciudad { get; set; }
            [StringLength(100)] public string? Departamento { get; set; }
            [StringLength(20)] public string? CodigoPostal { get; set; }
        }

        [BindProperty] public Vm Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int clienteId, int? id)
        {
            ClienteId = clienteId;

            var clienteExists = await _ctx.Clientes.AsNoTracking().AnyAsync(c => c.Id == ClienteId);
            if (!clienteExists) return NotFound();

            if (id.HasValue)
            {
                var d = await _ctx.DireccionesCliente.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id.Value && x.ClienteId == ClienteId);
                if (d == null) return NotFound();

                Input = new Vm
                {
                    Id = d.Id,
                    Tipo = d.Tipo,
                    Direccion = d.Direccion,
                    Ciudad = d.Ciudad,
                    Departamento = d.Departamento,
                    CodigoPostal = d.CodigoPostal
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var clienteExists = await _ctx.Clientes.AsNoTracking().AnyAsync(c => c.Id == ClienteId);
            if (!clienteExists)
            {
                ModelState.AddModelError(string.Empty, "Cliente inválido.");
            }

            if (!ModelState.IsValid) return Page();

            if (Input.Id == 0)
            {
                _ctx.DireccionesCliente.Add(new DireccionCliente
                {
                    ClienteId = ClienteId,
                    Tipo = Input.Tipo,
                    Direccion = Input.Direccion,
                    Ciudad = Input.Ciudad,
                    Departamento = Input.Departamento,
                    CodigoPostal = Input.CodigoPostal
                });
            }
            else
            {
                var d = await _ctx.DireccionesCliente.FirstOrDefaultAsync(x => x.Id == Input.Id && x.ClienteId == ClienteId);
                if (d == null) return NotFound();

                d.Tipo = Input.Tipo;
                d.Direccion = Input.Direccion;
                d.Ciudad = Input.Ciudad;
                d.Departamento = Input.Departamento;
                d.CodigoPostal = Input.CodigoPostal;
            }

            await _ctx.SaveChangesAsync();
            return RedirectToPage("Direcciones", new { clienteId = ClienteId });
        }
    }
}

