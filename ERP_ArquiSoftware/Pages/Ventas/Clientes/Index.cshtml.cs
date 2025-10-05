using ERP_ArquiSoftware.dA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Ventas.Clientes
{

    [Authorize(Policy = "Clientes.Ver")]
    public class IndexModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public IndexModel(AppDBContext ctx) => _ctx = ctx;

        public record Row(
            int Id,
            string TipoDocumento,
            string NumeroDocumento,
            string NombreRazonSocial,
            string? NombreComercial,
            string? Correo,
            string? Telefono,
            string? CondicionPagoNombre,
            bool Activo
        );

        public List<Row> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        public async Task OnGetAsync()
        {
            var query = _ctx.Clientes
                .AsNoTracking()
                .Include(c => c.CondicionPago)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                var f = Q.Trim();
                query = query.Where(c =>
                    c.NumeroDocumento.Contains(f) ||
                    c.NombreRazonSocial.Contains(f) ||
                    (c.NombreComercial != null && c.NombreComercial.Contains(f)) ||
                    (c.Correo != null && c.Correo.Contains(f)) ||
                    (c.Telefono != null && c.Telefono.Contains(f))
                );
            }

            Items = await query
                .OrderBy(c => c.NombreRazonSocial)
                .Select(c => new Row(
                    c.Id,
                    c.TipoDocumento,
                    c.NumeroDocumento,
                    c.NombreRazonSocial,
                    c.NombreComercial,
                    c.Correo,
                    c.Telefono,
                    c.CondicionPago != null ? c.CondicionPago.Nombre : null,
                    c.Activo
                ))
                .ToListAsync();
        }


        [Authorize(Policy = "Clientes.Eliminar")]
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var hasDocs = await _ctx.FacturasVenta.AnyAsync(f => f.ClienteId == id)
                       || await _ctx.PedidosVenta.AnyAsync(p => p.ClienteId == id);

            if (hasDocs)
            {
                ModelState.AddModelError(string.Empty, "No se puede eliminar: el cliente ya tiene movimientos.");
                await OnGetAsync();
                return Page();
            }

            var c = await _ctx.Clientes.FindAsync(id);
            if (c == null) return NotFound();

            _ctx.Clientes.Remove(c);
            await _ctx.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}

