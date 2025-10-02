using System.ComponentModel.DataAnnotations;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Proveedores.Vinculos
{
    public class EditModel : PageModel
    {
        private readonly AppDBContext _context;
        public EditModel(AppDBContext context) => _context = context;

        // Para mostrar nombres en la cabecera
        public string ProveedorNombre { get; set; } = "";
        public string ProductoNombre { get; set; } = "";

        public class VinculoInput
        {
            [Required] public int ProductoId { get; set; }
            [Required] public int ProveedorId { get; set; }

            // Parte de la PK
            [Required] public DateTime FechaDesde { get; set; }

            [Display(Name = "SKU Proveedor")]
            public string? SkuProveedor { get; set; }

            [Display(Name = "Precio Compra")]
            [Range(0, double.MaxValue)]
            public decimal? PrecioCompra { get; set; }

            [Display(Name = "Plazo (días)")]
            [Range(0, int.MaxValue)]
            public int? PlazoEntregaDias { get; set; }

            [Display(Name = "Proveedor principal")]
            public bool EsProveedorPrincipal { get; set; }

            [Display(Name = "Hasta")]
            [DataType(DataType.Date)]
            public DateTime? FechaHasta { get; set; }
        }

        [BindProperty] public VinculoInput Input { get; set; } = new();

        // Helper para parsear fecha de la ruta (acepta yyyy-MM-dd o ISO)
        private static bool TryParseFecha(string s, out DateTime dt)
        {
            if (DateTime.TryParse(s, out dt)) { return true; }
            // formato yyyy-MM-dd (asumir UTC)
            if (DateTime.TryParseExact(s, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
            {
                dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                return true;
            }
            return false;
        }

        public async Task<IActionResult> OnGetAsync(int productoId, int proveedorId, string fechaDesde)
        {
            if (!TryParseFecha(fechaDesde, out var fd)) return NotFound();

            var vinc = await _context.ProductoProveedores
                .Include(x => x.Producto).ThenInclude(p => p.Marca)
                .Include(x => x.Proveedor)
                .FirstOrDefaultAsync(x =>
                    x.ProductoId == productoId &&
                    x.ProveedorId == proveedorId &&
                    x.FechaDesde == fd);

            if (vinc is null) return NotFound();

            ProveedorNombre = vinc.Proveedor.NombreRazonSocial;
            ProductoNombre = string.IsNullOrWhiteSpace(vinc.Producto.Sku)
                ? vinc.Producto.NombreProducto
                : $"{vinc.Producto.Sku} - {vinc.Producto.NombreProducto}";

            Input = new VinculoInput
            {
                ProductoId = vinc.ProductoId,
                ProveedorId = vinc.ProveedorId,
                FechaDesde = vinc.FechaDesde,
                SkuProveedor = vinc.SkuProveedor,
                PrecioCompra = vinc.PrecioCompra,
                PlazoEntregaDias = vinc.PlazoEntregaDias,
                EsProveedorPrincipal = vinc.EsProveedorPrincipal,
                FechaHasta = vinc.FechaHasta?.Date
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validación básica
            if (!ModelState.IsValid) return Page();

            var vinc = await _context.ProductoProveedores
                .FirstOrDefaultAsync(x =>
                    x.ProductoId == Input.ProductoId &&
                    x.ProveedorId == Input.ProveedorId &&
                    x.FechaDesde == Input.FechaDesde);

            if (vinc is null) return NotFound();

            // Reglas de negocio
            if (Input.FechaHasta.HasValue && Input.FechaHasta.Value.ToUniversalTime() < vinc.FechaDesde)
            {
                ModelState.AddModelError(nameof(Input.FechaHasta), "Fecha 'Hasta' no puede ser menor que 'Desde'.");
                await LoadNamesAsync(vinc);
                return Page();
            }

            // Si se marca como principal, validar que no haya otro principal vigente para el mismo producto
            if (Input.EsProveedorPrincipal && (Input.FechaHasta == null))
            {
                var hayPrincipalVigente = await _context.ProductoProveedores.AnyAsync(pp =>
                    pp.ProductoId == vinc.ProductoId &&
                    pp.EsProveedorPrincipal &&
                    pp.FechaHasta == null &&
                    !(pp.ProductoId == vinc.ProductoId && pp.ProveedorId == vinc.ProveedorId && pp.FechaDesde == vinc.FechaDesde)
                );

                if (hayPrincipalVigente)
                {
                    ModelState.AddModelError(nameof(Input.EsProveedorPrincipal), "Ya existe un proveedor principal vigente para este producto.");
                    await LoadNamesAsync(vinc);
                    return Page();
                }
            }

            // Mapear cambios
            vinc.SkuProveedor = string.IsNullOrWhiteSpace(Input.SkuProveedor) ? null : Input.SkuProveedor.Trim();
            vinc.PrecioCompra = Input.PrecioCompra;
            vinc.PlazoEntregaDias = Input.PlazoEntregaDias;
            vinc.EsProveedorPrincipal = Input.EsProveedorPrincipal;
            vinc.FechaHasta = Input.FechaHasta?.ToUniversalTime();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                ModelState.AddModelError(string.Empty, "Conflicto de clave única. Verifica que no hayas creado otra versión con los mismos datos.");
                await LoadNamesAsync(vinc);
                return Page();
            }

            return RedirectToPage("/Proveedores/ProductosProv", new { q = "" });
        }

        private async Task LoadNamesAsync(ProductoProveedor vinc)
        {
            await _context.Entry(vinc).Reference(v => v.Proveedor).LoadAsync();
            await _context.Entry(vinc).Reference(v => v.Producto).LoadAsync();
            ProveedorNombre = vinc.Proveedor?.NombreRazonSocial ?? "";
            ProductoNombre = vinc.Producto is null ? "" :
                (string.IsNullOrWhiteSpace(vinc.Producto.Sku)
                    ? vinc.Producto.NombreProducto
                    : $"{vinc.Producto.Sku} - {vinc.Producto.NombreProducto}");
        }

        private static bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
                return sqlEx.Number == 2627 || sqlEx.Number == 2601;
            return ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true
                || ex.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase);
        }
    }
}

