using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Inventario
{
    public class IndexInvModel : PageModel
    {
        private readonly AppDBContext _context;

        public IndexInvModel(AppDBContext context)
        {
            _context = context;
        }

        public IList<Producto> Productos { get; set; } = new List<Producto>();

        public async Task OnGetAsync()
        {
            // Incluye la navegación para poder mostrar Categoria.Nombre
            Productos = await _context.Productos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .OrderBy(p => p.NombreProducto)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            // Redirige al GET para refrescar la lista
            return RedirectToPage();
        }
    }
}

