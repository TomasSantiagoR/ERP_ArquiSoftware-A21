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

        public IList<Producto> Productos { get; set; }

        public async Task OnGetAsync()
        {
            Productos = await _context.Productos.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if(producto == null)
            {
                return NotFound();

            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
