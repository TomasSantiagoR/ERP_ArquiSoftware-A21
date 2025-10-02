using System.ComponentModel.DataAnnotations;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Ventas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Ventas.Pedidos
{
    public class CreateModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public CreateModel(AppDBContext ctx) => _ctx = ctx;

        public class LineaVm
        {
            public int? ProductoId { get; set; }
            [Range(1, int.MaxValue)] public int Cantidad { get; set; } = 1;
            [Range(0, double.MaxValue)] public decimal PrecioUnitario { get; set; } = 0m;
        }
        public class PedidoVm
        {
            [Required, StringLength(10)]
            public string Serie { get; set; } = "PV";
            [Required] public int? ClienteId { get; set; }
            [Required] public int? AlmacenId { get; set; }
            public int? CondicionPagoId { get; set; }
            public List<LineaVm> Lineas { get; set; } = new() { new LineaVm() };
        }

        [BindProperty] public PedidoVm Input { get; set; } = new();

        public List<SelectListItem> ClientesSelect { get; set; } = new();
        public List<SelectListItem> AlmacenesSelect { get; set; } = new();
        public List<SelectListItem> ProductosSelect { get; set; } = new();
        public List<SelectListItem> CondicionesSelect { get; set; } = new();

        public Dictionary<int, decimal> PreciosPorProducto { get; set; } = new();

        public async Task OnGetAsync()
        {
            await CargarSelectsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CargarSelectsAsync();

            if (Input.ClienteId is null) ModelState.AddModelError(nameof(Input.ClienteId), "Cliente requerido.");
            if (Input.AlmacenId is null) ModelState.AddModelError(nameof(Input.AlmacenId), "Almacén requerido.");

            var lineasValidas = (Input.Lineas ?? new())
                .Where(l => l.ProductoId.HasValue && l.Cantidad > 0)
                .ToList();

            if (lineasValidas.Count == 0)
                ModelState.AddModelError(string.Empty, "Debes agregar al menos una línea válida.");

            if (!ModelState.IsValid) return Page();

            // Productos para snapshot
            var ids = lineasValidas.Select(l => l.ProductoId!.Value).Distinct().ToList();
            var productos = await _ctx.Productos
                .Include(p => p.Impuesto)
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            // Secuencia PV
            var sec = await _ctx.SecuenciasDocumento
                .FirstOrDefaultAsync(s => s.Tipo == "PV" && s.Serie == Input.Serie && s.Activo);
            if (sec == null)
            {
                ModelState.AddModelError(nameof(Input.Serie), "No hay secuencia activa para esa serie de pedido.");
                return Page();
            }

            var pedido = new PedidoVenta
            {
                Serie = Input.Serie,
                Numero = sec.SiguienteNumero,
                Fecha = DateTime.Now,
                ClienteId = Input.ClienteId!.Value,
                AlmacenId = Input.AlmacenId!.Value,
                CondicionPagoId = Input.CondicionPagoId,
                Estado = "Confirmado",
                Lineas = new List<PedidoVentaLinea>()
            };

            decimal subtotal = 0, totalImp = 0;
            foreach (var l in lineasValidas)
            {
                if (!productos.TryGetValue(l.ProductoId!.Value, out var prod))
                {
                    ModelState.AddModelError(string.Empty, $"Producto {l.ProductoId} inválido.");
                    return Page();
                }

                var precio = l.PrecioUnitario > 0 ? l.PrecioUnitario : prod.PrecioUnitario;

                var neto = precio * l.Cantidad;
                var porc = prod.Impuesto?.Porcentaje;
                var imp = porc.HasValue ? Math.Round(neto * (porc.Value / 100m), 2) : 0m;

                pedido.Lineas.Add(new PedidoVentaLinea
                {
                    ProductoId = prod.Id,
                    Cantidad = l.Cantidad,
                    PrecioUnitario = precio,
                    ImpuestoId = prod.ImpuestoId,
                    ImpuestoPorcentaje = porc,
                    ImporteNeto = neto,
                    ImporteImpuesto = imp,
                    ImporteTotal = neto + imp
                });

                subtotal += neto;
                totalImp += imp;
            }

            pedido.Subtotal = subtotal;
            pedido.TotalImpuestos = totalImp;
            pedido.Total = subtotal + totalImp;

            try
            {
                _ctx.PedidosVenta.Add(pedido);
                sec.SiguienteNumero += 1;
                await _ctx.SaveChangesAsync();
                return RedirectToPage("Details", new { id = pedido.Id });
            }
            catch (Exception ex)
            {
                try { sec.SiguienteNumero -= 1; await _ctx.SaveChangesAsync(); } catch { }
                ModelState.AddModelError(string.Empty, $"Error al guardar pedido: {ex.Message}");
                return Page();
            }
        }

        private async Task CargarSelectsAsync()
        {
            ClientesSelect = await _ctx.Clientes.AsNoTracking()
                .Where(c => c.Activo)
                .OrderBy(c => c.NombreRazonSocial)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.NombreRazonSocial} ({c.TipoDocumento} {c.NumeroDocumento})" })
                .ToListAsync();

            AlmacenesSelect = await _ctx.Almacenes.AsNoTracking()
                .Where(a => a.Activo)
                .OrderBy(a => a.Nombre)
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Nombre })
                .ToListAsync();

            CondicionesSelect = await _ctx.CondicionesPago.AsNoTracking()
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"{c.Nombre} ({c.DiasPlazo} días)" })
                .ToListAsync();

            ProductosSelect = await _ctx.Productos.AsNoTracking()
                .OrderBy(p => p.NombreProducto)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Sku} - {p.NombreProducto}" })
                .ToListAsync();

            PreciosPorProducto = await _ctx.Productos.AsNoTracking()
                .Select(p => new { p.Id, p.PrecioUnitario })
                .ToDictionaryAsync(x => x.Id, x => x.PrecioUnitario);

            if (Input.Lineas.Count == 0) Input.Lineas.Add(new LineaVm());
        }
    }
}

