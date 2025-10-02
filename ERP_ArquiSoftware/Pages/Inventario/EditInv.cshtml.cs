using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Inventario
{
    public class EditInvModel : PageModel
    {
        private readonly AppDBContext _context;
        public EditInvModel(AppDBContext context) => _context = context;

        [BindProperty]
        public Producto Productos { get; set; } = new();

        public List<SelectListItem> CategoriasSelect { get; set; } = new();
        public List<SelectListItem> MarcasSelect { get; set; } = new();
        public List<SelectListItem> UnidadesSelect { get; set; } = new();
        public List<SelectListItem> ImpuestosSelect { get; set; } = new();

        // Solo lectura: suma de existencias
        public int StockActual { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Productos = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Productos == null)
                return NotFound();

            await CargarSelectsAsync();

            StockActual = await _context.Existencias
                .Where(e => e.ProductoId == id)
                .SumAsync(e => (int?)e.Stock) ?? 0;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CargarSelectsAsync();

            // Validaciones
            var categoriaOk = await _context.Categorias
                .AnyAsync(c => c.Id == Productos.CategoriaId && c.Activa);
            if (!categoriaOk)
                ModelState.AddModelError(nameof(Productos.CategoriaId), "La categoría no existe o está inactiva.");

            var skuEnUso = await _context.Productos
                .AnyAsync(p => p.Sku == Productos.Sku && p.Id != Productos.Id);
            if (skuEnUso)
                ModelState.AddModelError(nameof(Productos.Sku), "El SKU ya existe.");

            if (!ModelState.IsValid)
                return Page();

            // Cargar entidad real y mapear campos permitidos (evita overposting)
            var original = await _context.Productos.FirstOrDefaultAsync(p => p.Id == Productos.Id);
            if (original == null) return NotFound();

            original.NombreProducto = Productos.NombreProducto;
            original.Sku = Productos.Sku;
            original.Gtin = Productos.Gtin;
            original.CategoriaId = Productos.CategoriaId;
            original.MarcaId = Productos.MarcaId;
            original.UnidadMedidaId = Productos.UnidadMedidaId;
            original.ImpuestoId = Productos.ImpuestoId;

            original.Descripcion = Productos.Descripcion ?? string.Empty;
            original.PrecioUnitario = Productos.PrecioUnitario;
            original.CostoEstandar = Productos.CostoEstandar;
            original.CostoPromedio = Productos.CostoPromedio;
            original.UltimoCosto = Productos.UltimoCosto;

            // Inventario: solo parámetros de reorden; el stock real no se edita aquí
            original.StockMin = Productos.StockMin;
            original.PuntoReorden = Productos.PuntoReorden;
            original.CantidadReorden = Productos.CantidadReorden;

            // Flags y atributos físicos
            original.Activo = Productos.Activo;
            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Productos.AnyAsync(p => p.Id == Productos.Id))
                    return NotFound();
                throw;
            }

            return RedirectToPage("IndexInv");
        }

        private async Task CargarSelectsAsync()
        {
            CategoriasSelect = await _context.Categorias
                .Where(c => c.Activa)
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nombre })
                .ToListAsync();

            MarcasSelect = await _context.Marcas
                .Where(m => m.Activa)
                .OrderBy(m => m.Nombre)
                .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Nombre })
                .ToListAsync();

            UnidadesSelect = await _context.UnidadesMedida
                .Where(u => u.Activa)
                .OrderBy(u => u.Descripcion)
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = $"{u.Codigo} - {u.Descripcion}" })
                .ToListAsync();

            ImpuestosSelect = await _context.Impuestos
                .Where(i => i.Activo)
                .OrderBy(i => i.Nombre)
                .Select(i => new SelectListItem { Value = i.Id.ToString(), Text = $"{i.Nombre} ({i.Porcentaje}%)" })
                .ToListAsync();
        }
    }
}

