using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Inventario
{
    public class CreateInvModel : PageModel
    {
        private readonly AppDBContext _context;
        public CreateInvModel(AppDBContext context) => _context = context;

        // Selects
        public List<SelectListItem> CategoriasSelect { get; set; } = new();
        public List<SelectListItem> MarcasSelect { get; set; } = new();
        public List<SelectListItem> UnidadesSelect { get; set; } = new();
        public List<SelectListItem> ImpuestosSelect { get; set; } = new();
        public List<SelectListItem> AlmacenesSelect { get; set; } = new();

        // Entidad que estás creando
        [BindProperty]
        public Producto Productos { get; set; } = new();

        // Campos auxiliares SOLO para la vista (no pertenecen a Producto)
        [BindProperty]
        public int AlmacenId { get; set; }

        [BindProperty]
        public int StockInicial { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await CargarSelectsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CargarSelectsAsync();

            // --- Validaciones de negocio ---
            if (Productos.StockMin > StockInicial)
                ModelState.AddModelError(nameof(Productos.StockMin), "El stock mínimo no puede ser mayor que el stock inicial.");

            var categoriaOk = await _context.Categorias.AnyAsync(c => c.Id == Productos.CategoriaId && c.Activa);
            if (!categoriaOk)
                ModelState.AddModelError(nameof(Productos.CategoriaId), "La categoría no existe o está inactiva.");

            if (AlmacenId <= 0 || !await _context.Almacenes.AnyAsync(a => a.Id == AlmacenId && a.Activo))
                ModelState.AddModelError(nameof(AlmacenId), "Debe seleccionar un almacén válido.");

            if (await _context.Productos.AnyAsync(p => p.Sku == Productos.Sku))
                ModelState.AddModelError(nameof(Productos.Sku), "El SKU ya existe.");

            if (!ModelState.IsValid)
                return Page();

            // Defaults
            if (Productos.FechaCreacion == default)
                Productos.FechaCreacion = DateTime.Now;

            // Sincroniza stock global con el inicial
            Productos.Stock = StockInicial;

            // Persistir
            _context.Productos.Add(Productos);
            // Crear existencia en el almacén seleccionado
            _context.Existencias.Add(new Existencia
            {
                Producto = Productos,
                AlmacenId = AlmacenId,
                Stock = StockInicial
            });

            await _context.SaveChangesAsync();
            return RedirectToPage("/Inventario/IndexInv");
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

            AlmacenesSelect = await _context.Almacenes
                .Where(a => a.Activo)
                .OrderBy(a => a.Nombre)
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Nombre })
                .ToListAsync();
        }
    }
}



