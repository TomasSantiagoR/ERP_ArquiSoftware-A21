using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Facturacion;
using ERP_ArquiSoftware.Models.Ventas;
using ERP_ArquiSoftware.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Ventas.Pedidos
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

        public PedidoVenta? Pedido { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Pedido = await _ctx.PedidosVenta
                .AsNoTracking()
                .Include(p => p.Cliente)
                .Include(p => p.Almacen)
                .Include(p => p.CondicionPago)
                .Include(p => p.Lineas).ThenInclude(l => l.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Pedido == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostFacturarAsync(int id)
        {
            // Cargar pedido con líneas (seguros para facturar)
            var p = await _ctx.PedidosVenta
                .Include(x => x.Lineas)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null) return NotFound();
            if (p.Estado == "Cancelado") return BadRequest("El pedido está cancelado.");
            if (p.Estado == "Facturado") return BadRequest("El pedido ya fue facturado.");
            if (!p.Lineas.Any()) return BadRequest("El pedido no tiene líneas.");

            // Secuencia FV activa
            var sec = await _ctx.SecuenciasDocumento
                .FirstOrDefaultAsync(s => s.Tipo == "FV" && s.Activo && s.Serie == "FV");
            if (sec == null) return BadRequest("No hay secuencia de factura activa (FV).");

            // Productos con impuesto para snapshot
            var ids = p.Lineas.Select(l => l.ProductoId).Distinct().ToList();
            var productos = await _ctx.Productos
                .Include(pr => pr.Impuesto)
                .Where(pr => ids.Contains(pr.Id))
                .ToDictionaryAsync(pr => pr.Id);

            // Armar factura
            var f = new FacturaVenta
            {
                Serie = sec.Serie,
                Numero = sec.SiguienteNumero,
                Fecha = DateTime.Now,
                ClienteId = p.ClienteId,
                AlmacenId = p.AlmacenId,
                CondicionPagoId = p.CondicionPagoId,
                Estado = "Emitida",
                Lineas = new List<FacturaVentaLinea>()
            };

            decimal subtotal = 0, impTotal = 0;
            foreach (var l in p.Lineas)
            {
                var prod = productos[l.ProductoId];
                var precio = l.PrecioUnitario > 0 ? l.PrecioUnitario : prod.PrecioUnitario;

                var neto = precio * l.Cantidad;
                var porc = prod.Impuesto?.Porcentaje;
                var imp = porc.HasValue ? Math.Round(neto * (porc.Value / 100m), 2) : 0m;

                f.Lineas.Add(new FacturaVentaLinea
                {
                    ProductoId = l.ProductoId,
                    Cantidad = l.Cantidad,
                    PrecioUnitario = precio,
                    ImpuestoId = prod.ImpuestoId,
                    ImpuestoPorcentaje = porc,
                    ImporteNeto = neto,
                    ImporteImpuesto = imp,
                    ImporteTotal = neto + imp
                });

                subtotal += neto;
                impTotal += imp;
            }
            f.Subtotal = subtotal;
            f.TotalImpuestos = impTotal;
            f.Total = subtotal + impTotal;

            try
            {
                // Guardar factura y avanzar secuencia
                _ctx.FacturasVenta.Add(f);
                sec.SiguienteNumero += 1;
                await _ctx.SaveChangesAsync();

                // Descontar stock (service con su transacción)
                await _inv.DescargarStockPorFacturaAsync(f.Id);

                // Marcar pedido como facturado
                p.Estado = "Facturado";
                await _ctx.SaveChangesAsync();

                return RedirectToPage("/Facturacion/Facturas/Details", new { id = f.Id });
            }
            catch (Exception ex)
            {
                try { sec.SiguienteNumero -= 1; await _ctx.SaveChangesAsync(); } catch { }
                ModelState.AddModelError(string.Empty, $"Error al generar factura: {ex.Message}");
                return await OnGetAsync(id);
            }
        }
    }
}

