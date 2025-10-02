using System.ComponentModel.DataAnnotations;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Facturacion;
using ERP_ArquiSoftware.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Facturacion.Facturas
{
    public class CreateModel : PageModel
    {
        private readonly AppDBContext _ctx;
        private readonly IInventarioService _inv;
        public CreateModel(AppDBContext ctx, IInventarioService inv)
        {
            _ctx = ctx;
            _inv = inv;
        }

        // ----- VM -----
        public class LineaVm
        {
            public int? ProductoId { get; set; }
            [Range(1, int.MaxValue)] public int Cantidad { get; set; } = 1;
            [Range(0, double.MaxValue)] public decimal PrecioUnitario { get; set; } = 0m;
        }
        public class FacturaVm
        {
            [Required, StringLength(10)]
            public string Serie { get; set; } = "FV";
            [Required] public int? ClienteId { get; set; }
            [Required] public int? AlmacenId { get; set; }
            public int? CondicionPagoId { get; set; }
            public List<LineaVm> Lineas { get; set; } = new() { new LineaVm() }; // 1 fila por defecto
        }

        [BindProperty] public FacturaVm Input { get; set; } = new();

        // Selects
        public List<SelectListItem> ClientesSelect { get; set; } = new();
        public List<SelectListItem> AlmacenesSelect { get; set; } = new();
        public List<SelectListItem> ProductosSelect { get; set; } = new();
        public List<SelectListItem> CondicionesSelect { get; set; } = new();

        // Diccionario para autocompletar precios en el cliente
        public Dictionary<int, decimal> PreciosPorProducto { get; set; } = new();

        public async Task OnGetAsync()
        {
            await CargarSelectsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CargarSelectsAsync();

            // Validaciones
            if (Input.ClienteId is null) ModelState.AddModelError(nameof(Input.ClienteId), "Cliente requerido.");
            if (Input.AlmacenId is null) ModelState.AddModelError(nameof(Input.AlmacenId), "Almacén requerido.");

            var lineasValidas = (Input.Lineas ?? new())
                .Where(l => l.ProductoId.HasValue && l.Cantidad > 0)
                .ToList();

            if (lineasValidas.Count == 0)
                ModelState.AddModelError(string.Empty, "Debes agregar al menos una línea válida.");

            if (!ModelState.IsValid) return Page();

            // Productos (con impuesto) para snapshot
            var ids = lineasValidas.Select(l => l.ProductoId!.Value).Distinct().ToList();
            var productos = await _ctx.Productos
                .Include(p => p.Impuesto)
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            // Secuencia activa para la serie
            var sec = await _ctx.SecuenciasDocumento
                .FirstOrDefaultAsync(s => s.Tipo == "FV" && s.Serie == Input.Serie && s.Activo);
            if (sec == null)
            {
                ModelState.AddModelError(nameof(Input.Serie), "No hay secuencia activa para esa serie.");
                return Page();
            }

            // Construcción de la factura + líneas (snapshot de precios e impuestos)
            var factura = new FacturaVenta
            {
                Serie = Input.Serie,
                Numero = sec.SiguienteNumero,   // se usa y luego se avanza
                Fecha = DateTime.Now,
                ClienteId = Input.ClienteId!.Value,
                AlmacenId = Input.AlmacenId!.Value,
                CondicionPagoId = Input.CondicionPagoId,
                Estado = "Emitida",
                Lineas = new List<FacturaVentaLinea>()
            };

            decimal subtotal = 0, totalImp = 0;

            foreach (var l in lineasValidas)
            {
                if (!productos.TryGetValue(l.ProductoId!.Value, out var prod))
                {
                    ModelState.AddModelError(string.Empty, $"Producto {l.ProductoId} inválido.");
                    return Page();
                }

                // Si el usuario deja 0, usamos el precio del producto (UX + seguridad server-side)
                var precio = l.PrecioUnitario > 0 ? l.PrecioUnitario : prod.PrecioUnitario;

                var neto = precio * l.Cantidad;
                decimal? porc = prod.Impuesto?.Porcentaje;
                var imp = porc.HasValue ? Math.Round(neto * (porc.Value / 100m), 2) : 0m;
                var total = neto + imp;

                factura.Lineas.Add(new FacturaVentaLinea
                {
                    ProductoId = prod.Id,
                    Cantidad = l.Cantidad,
                    PrecioUnitario = precio,
                    ImpuestoId = prod.ImpuestoId,
                    ImpuestoPorcentaje = porc,
                    ImporteNeto = neto,
                    ImporteImpuesto = imp,
                    ImporteTotal = total
                });

                subtotal += neto;
                totalImp += imp;
            }

            factura.Subtotal = subtotal;
            factura.TotalImpuestos = totalImp;
            factura.Total = subtotal + totalImp;

            // ----- Sin transacción en la Page -----
            try
            {
                // 1) Guardar factura (para tener Id) y avanzar secuencia
                _ctx.FacturasVenta.Add(factura);
                sec.SiguienteNumero += 1;
                await _ctx.SaveChangesAsync(); // persiste factura y líneas

                // 2) Descontar stock (el service maneja su transacción internamente)
                await _inv.DescargarStockPorFacturaAsync(factura.Id);

                // 3) OK
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                // Compensación simple: borrar factura y revertir secuencia
                try
                {
                    var creada = await _ctx.FacturasVenta
                        .Include(f => f.Lineas)
                        .FirstOrDefaultAsync(f => f.Id == factura.Id);

                    if (creada != null)
                    {
                        _ctx.FacturaVentaLineas.RemoveRange(creada.Lineas);
                        _ctx.FacturasVenta.Remove(creada);
                    }

                    // Revertir secuencia (simplificado)
                    sec.SiguienteNumero -= 1;

                    await _ctx.SaveChangesAsync();
                }
                catch
                {
                    // no ocultar el error principal
                }

                ModelState.AddModelError(string.Empty, $"Error al generar la factura: {ex.Message}");
                return Page();
            }
        }

        private async Task CargarSelectsAsync()
        {
            ClientesSelect = await _ctx.Clientes
                .AsNoTracking()
                .Where(c => c.Activo)
                .OrderBy(c => c.NombreRazonSocial)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.NombreRazonSocial} ({c.TipoDocumento} {c.NumeroDocumento})" })
                .ToListAsync();

            AlmacenesSelect = await _ctx.Almacenes
                .AsNoTracking()
                .Where(a => a.Activo)
                .OrderBy(a => a.Nombre)
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Nombre })
                .ToListAsync();

            CondicionesSelect = await _ctx.CondicionesPago
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.Nombre} ({c.DiasPlazo} días)" })
                .ToListAsync();

            ProductosSelect = await _ctx.Productos
                .AsNoTracking()
                .OrderBy(p => p.NombreProducto)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Sku} - {p.NombreProducto}" })
                .ToListAsync();

            // Diccionario id → precio para el script
            PreciosPorProducto = await _ctx.Productos
                .AsNoTracking()
                .Select(p => new { p.Id, p.PrecioUnitario })
                .ToDictionaryAsync(x => x.Id, x => x.PrecioUnitario);

            if (Input.Lineas.Count == 0) Input.Lineas.Add(new LineaVm());
        }
    }
}


