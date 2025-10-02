using System.ComponentModel.DataAnnotations;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Ventas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Ventas.Clientes
{
    public class UpsertModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public UpsertModel(AppDBContext ctx) => _ctx = ctx;

        public class Vm
        {
            public int Id { get; set; }

            [Required, StringLength(20)]
            [Display(Name = "Tipo Doc.")]
            public string TipoDocumento { get; set; } = "CC";

            [Required, StringLength(30)]
            [Display(Name = "Número Doc.")]
            public string NumeroDocumento { get; set; } = "";

            [Required, StringLength(200)]
            [Display(Name = "Nombre / Razón Social")]
            public string NombreRazonSocial { get; set; } = "";

            [StringLength(200)]
            [Display(Name = "Nombre Comercial")]
            public string? NombreComercial { get; set; }

            [EmailAddress]
            public string? Correo { get; set; }

            [StringLength(30)]
            public string? Telefono { get; set; }

            [Display(Name = "Condición de Pago")]
            public int? CondicionPagoId { get; set; }

            public bool Activo { get; set; } = true;
        }

        [BindProperty] public Vm Input { get; set; } = new();
        public List<SelectListItem> CondicionesSelect { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await CargarCondicionesAsync();

            if (id.HasValue)
            {
                var c = await _ctx.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value);
                if (c == null) return NotFound();

                Input = new Vm
                {
                    Id = c.Id,
                    TipoDocumento = c.TipoDocumento,
                    NumeroDocumento = c.NumeroDocumento,
                    NombreRazonSocial = c.NombreRazonSocial,
                    NombreComercial = c.NombreComercial,
                    Correo = c.Correo,
                    Telefono = c.Telefono,
                    CondicionPagoId = c.CondicionPagoId,
                    Activo = c.Activo
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CargarCondicionesAsync();

            // Unicidad por TipoDocumento + NumeroDocumento
            var existe = await _ctx.Clientes
                .AnyAsync(c => c.TipoDocumento == Input.TipoDocumento
                            && c.NumeroDocumento == Input.NumeroDocumento
                            && c.Id != Input.Id);
            if (existe)
            {
                ModelState.AddModelError(nameof(Input.NumeroDocumento), "Ya existe un cliente con este documento.");
            }

            if (!ModelState.IsValid) return Page();

            if (Input.Id == 0)
            {
                _ctx.Clientes.Add(new Cliente
                {
                    TipoDocumento = Input.TipoDocumento,
                    NumeroDocumento = Input.NumeroDocumento,
                    NombreRazonSocial = Input.NombreRazonSocial,
                    NombreComercial = Input.NombreComercial,
                    Correo = Input.Correo,
                    Telefono = Input.Telefono,
                    CondicionPagoId = Input.CondicionPagoId,
                    Activo = Input.Activo
                });
            }
            else
            {
                var c = await _ctx.Clientes.FindAsync(Input.Id);
                if (c == null) return NotFound();

                c.TipoDocumento = Input.TipoDocumento;
                c.NumeroDocumento = Input.NumeroDocumento;
                c.NombreRazonSocial = Input.NombreRazonSocial;
                c.NombreComercial = Input.NombreComercial;
                c.Correo = Input.Correo;
                c.Telefono = Input.Telefono;
                c.CondicionPagoId = Input.CondicionPagoId;
                c.Activo = Input.Activo;
            }

            await _ctx.SaveChangesAsync();
            return RedirectToPage("Index");
        }

        private async Task CargarCondicionesAsync()
        {
            CondicionesSelect = await _ctx.CondicionesPago
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.Nombre} ({c.DiasPlazo} días)" })
                .ToListAsync();
        }
    }
}
