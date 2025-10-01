using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Inventario
{
    public class EditInvModel : PageModel
    {
        private readonly AppDBContext _context;

        public EditInvModel(AppDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Producto Productos { get; set; } = new();

        public List<SelectListItem> CategoriasSelect { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Productos = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Productos == null)
            {
                return NotFound();
            }

            await CargarCategoriasAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CargarCategoriasAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _context.Attach(Productos).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Productos.Any(p => p.Id == Productos.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToPage("IndexInv");
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
