using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Inventario
{
    public class CreateInvModel : PageModel
    {
        private readonly AppDBContext _context;

        public CreateInvModel(AppDBContext context)
        {
            _context = context;
        }

        // Lista para el <select>
        public List<SelectListItem> CategoriasSelect { get; set; } = new();

        [BindProperty]
        public Producto Productos { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await CargarCategoriasAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Siempre recarga la lista por si hay que redisplayear el formulario
            await CargarCategoriasAsync();

            // Validación de reglas de negocio adicionales (opcional)
            if (Productos.StockMin > Productos.Stock)
            {
                ModelState.AddModelError("Productos.StockMin", "El stock mínimo no puede ser mayor que el stock actual.");
            }

            // Asegura que la categoría exista
            var existeCategoria = await _context.Categorias
                .AnyAsync(c => c.Id == Productos.CategoriaId && c.Activa);

            if (!existeCategoria)
            {
                ModelState.AddModelError("Productos.CategoriaId", "La categoría seleccionada no existe o está inactiva.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fecha de creación (por si en BD no hay default)
            if (Productos.FechaCreacion == default)
                Productos.FechaCreacion = DateTime.Now;

            _context.Productos.Add(Productos);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Inventario/IndexInv");
        }

        private async Task CargarCategoriasAsync()
        {
            CategoriasSelect = await _context.Categorias
                .Where(c => c.Activa)
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                })
                .ToListAsync();
        }
    }
}


