using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace ERP_ArquiSoftware.Pages.Proveedores
{
    public class ProductosProvModel : PageModel
    {
        private readonly AppDBContext _context;

        public ProductosProvModel(AppDBContext context)
        {
            _context = context;
        }

        public List<Proveedor> Proveedores { get; set; } = new();

        // Búsqueda
        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        // Select de productos para el modal
        public List<SelectListItem> ProductosSelect { get; set; } = new();

        // Para mostrar error simple bajo el select en el modal
        public string? ErrorProducto { get; set; }

        public async Task OnGetAsync()
        {
            await CargarProductosSelectAsync();

            var query = _context.Proveedores
                .AsNoTracking()
                .Include(p => p.ProductoProveedores)
                    .ThenInclude(pp => pp.Producto)
                        .ThenInclude(prod => prod.Marca)
                .Include(p => p.ProductoProveedores)
                    .ThenInclude(pp => pp.Producto)
                        .ThenInclude(prod => prod.UnidadMedida)
                .Include(p => p.ProductoProveedores)
                    .ThenInclude(pp => pp.Producto)
                        .ThenInclude(prod => prod.Impuesto)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                var q = Q.Trim();
                query = query.Where(p =>
                    p.NombreRazonSocial.Contains(q) ||
                    p.NIT.Contains(q) ||
                    p.ProductoProveedores.Any(pp =>
                        pp.Producto.NombreProducto.Contains(q) ||
                        (pp.SkuProveedor != null && pp.SkuProveedor.Contains(q)) ||
                        (pp.Producto.Marca != null && pp.Producto.Marca.Nombre.Contains(q))
                    )
                );
            }

            Proveedores = await query
                .OrderBy(p => p.NombreRazonSocial)
                .ToListAsync();
        }

        // POST: crear vínculo ProductoProveedor con soporte de versionado
        public async Task<IActionResult> OnPostAddAsync(
            int? ProveedorId,
            int? ProductoId,
            string? SkuProveedor,
            decimal? PrecioCompra,
            int? PlazoEntregaDias,
            bool EsProveedorPrincipal,
            DateTime? FechaDesde,
            bool VersionarPrecio)
        {
            await CargarProductosSelectAsync();

            if (ProveedorId is null || ProductoId is null)
            {
                ErrorProducto = "Debes seleccionar proveedor y producto.";
                await OnGetAsync();
                return Page();
            }

            var existeProv = await _context.Proveedores.AnyAsync(p => p.Id == ProveedorId.Value);
            var existeProd = await _context.Productos.AnyAsync(p => p.Id == ProductoId.Value);

            if (!existeProv || !existeProd)
            {
                ErrorProducto = "Proveedor o producto no válido.";
                await OnGetAsync();
                return Page();
            }

            var fechaNuevaDesde = (FechaDesde?.ToUniversalTime()) ?? DateTime.UtcNow;

            // Buscar vigente actual (si lo hay)
            var vigente = await _context.ProductoProveedores
                .FirstOrDefaultAsync(pp =>
                    pp.ProveedorId == ProveedorId.Value &&
                    pp.ProductoId == ProductoId.Value &&
                    pp.FechaHasta == null);

            // Si NO versiona y YA hay vigente -> bloquear
            if (!VersionarPrecio && vigente != null)
            {
                ErrorProducto = "Ya existe una relación vigente para este proveedor y producto. Marca 'Versionar precio' o cierra la vigente antes.";
                await OnGetAsync();
                return Page();
            }

            // Validar que solo exista un principal vigente
            if (EsProveedorPrincipal)
            {
                var hayOtroPrincipal = await _context.ProductoProveedores.AnyAsync(pp =>
                    pp.ProductoId == ProductoId.Value &&
                    pp.EsProveedorPrincipal &&
                    pp.FechaHasta == null &&
                    !(vigente != null && pp.ProductoId == vigente.ProductoId &&
                      pp.ProveedorId == vigente.ProveedorId &&
                      pp.FechaDesde == vigente.FechaDesde));

                if (hayOtroPrincipal)
                {
                    ErrorProducto = "Ya existe un proveedor principal vigente para este producto.";
                    await OnGetAsync();
                    return Page();
                }
            }

            var nuevaVersion = new ProductoProveedor
            {
                ProveedorId = ProveedorId.Value,
                ProductoId = ProductoId.Value,
                SkuProveedor = string.IsNullOrWhiteSpace(SkuProveedor) ? null : SkuProveedor.Trim(),
                PrecioCompra = PrecioCompra,
                PlazoEntregaDias = PlazoEntregaDias,
                EsProveedorPrincipal = EsProveedorPrincipal,
                FechaDesde = fechaNuevaDesde
            };

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                if (VersionarPrecio && vigente != null)
                {
                    if (fechaNuevaDesde <= vigente.FechaDesde)
                    {
                        ErrorProducto = "La 'Fecha desde' de la nueva versión debe ser mayor que la 'Fecha desde' de la versión vigente.";
                        await OnGetAsync();
                        return Page();
                    }

                    // Cerrar la vigente justo antes de la nueva fecha
                    vigente.FechaHasta = fechaNuevaDesde.AddSeconds(-1);
                }

                _context.ProductoProveedores.Add(nuevaVersion);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                await tx.RollbackAsync();
                ErrorProducto = "La relación ya existe (clave duplicada). Cambia 'Fecha desde' o revisa las versiones.";
                await OnGetAsync();
                return Page();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            return RedirectToPage(new { q = Q });
        }

        private async Task CargarProductosSelectAsync()
        {
            ProductosSelect = await _context.Productos
                .AsNoTracking()
                .OrderBy(p => p.NombreProducto)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.NombreProducto
                })
                .ToListAsync();
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

