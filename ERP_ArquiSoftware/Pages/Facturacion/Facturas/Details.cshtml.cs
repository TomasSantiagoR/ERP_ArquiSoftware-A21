using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Facturacion;
using ERP_ArquiSoftware.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Facturacion.Facturas
{
    public class DetailsModel : PageModel
    {
        private readonly AppDBContext _ctx;
        private readonly IInventarioService _inv;

        public DetailsModel(AppDBContext ctx, IInventarioService inv)
        {
            _ctx = ctx;
            _inv = inv;
        }

        public FacturaVenta? Factura { get; set; }
        public List<FacturaVentaLinea> Lineas { get; set; } = new();
        public List<Cobro> Cobros { get; set; } = new();
        public decimal TotalCobros { get; set; }
        public decimal Saldo { get; set; }
        public string? ErrorCobro { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            await CargarAsync(id);
            if (Factura is null) return NotFound();
            return Page();
        }

        // POST: Registrar Cobro
        public async Task<IActionResult> OnPostAddCobroAsync(
            int id, DateTime? Fecha, decimal? Monto, string? MedioPago, string? Referencia)
        {
            await CargarAsync(id);
            if (Factura is null) return NotFound();

            // Validaciones de cobro
            if (Monto is null || Monto <= 0)
            {
                ErrorCobro = "El monto debe ser mayor a 0.";
                return Page();
            }
            if (!string.IsNullOrWhiteSpace(MedioPago) && MedioPago.Length > 30)
            {
                ErrorCobro = "El medio de pago no puede exceder 30 caracteres.";
                return Page();
            }
            if (!string.IsNullOrWhiteSpace(Referencia) && Referencia.Length > 50)
            {
                ErrorCobro = "La referencia no puede exceder 50 caracteres.";
                return Page();
            }
            if (Factura.Estado == "Anulada")
            {
                ErrorCobro = "No se pueden registrar cobros sobre una factura anulada.";
                return Page();
            }

            var saldo = Saldo; // calculado en CargarAsync
            if (Monto > saldo)
            {
                ErrorCobro = $"El monto no puede exceder el saldo pendiente ({saldo.ToString("C")}).";
                return Page();
            }

            _ctx.Cobros.Add(new Cobro
            {
                FacturaVentaId = Factura.Id,
                Fecha = (Fecha ?? DateTime.Now),
                MedioPago = string.IsNullOrWhiteSpace(MedioPago) ? "Efectivo" : MedioPago.Trim(),
                Referencia = string.IsNullOrWhiteSpace(Referencia) ? null : Referencia.Trim(),
                Monto = Monto.Value
            });

            await _ctx.SaveChangesAsync();

            // Recalcular saldo y marcar pagada si corresponde
            var totalCobros = await _ctx.Cobros.Where(c => c.FacturaVentaId == Factura.Id).SumAsync(c => (decimal?)c.Monto) ?? 0m;
            var saldoNuevo = Factura.Total - totalCobros;

            if (saldoNuevo <= 0 && Factura.Estado != "Anulada")
            {
                Factura.Estado = "Pagada";
                await _ctx.SaveChangesAsync();
            }

            return RedirectToPage(new { id });
        }

        // POST: Anular (revertir stock si no hay cobros)
        public async Task<IActionResult> OnPostAnularAsync(int id)
        {
            await CargarAsync(id);
            if (Factura is null) return NotFound();

            if (Factura.Estado == "Anulada")
                return RedirectToPage(new { id });

            var tieneCobros = await _ctx.Cobros.AnyAsync(c => c.FacturaVentaId == id);
            if (tieneCobros)
            {
                ModelState.AddModelError(string.Empty, "No se puede anular: la factura tiene cobros registrados.");
                return Page();
            }

            // Revertir stock con el servicio (maneja su propia transacción)
            await _inv.RevertirStockPorFacturaAsync(id);

            Factura.Estado = "Anulada";
            await _ctx.SaveChangesAsync();

            return RedirectToPage(new { id });
        }

        private async Task CargarAsync(int id)
        {
            Factura = await _ctx.FacturasVenta
                .Include(f => f.Cliente)
                .Include(f => f.Almacen)
                .Include(f => f.CondicionPago)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (Factura == null) return;

            Lineas = await _ctx.FacturaVentaLineas
                .Include(l => l.Producto)
                .Where(l => l.FacturaVentaId == id)
                .OrderBy(l => l.Id)
                .ToListAsync();

            Cobros = await _ctx.Cobros
                .Where(c => c.FacturaVentaId == id)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            TotalCobros = Cobros.Sum(c => c.Monto);
            Saldo = Factura.Total - TotalCobros;
            if (Saldo < 0) Saldo = 0;
        }
    }
}


