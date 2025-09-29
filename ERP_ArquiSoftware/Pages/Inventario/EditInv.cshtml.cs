using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;

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

        public Producto Productos { get; set; } = default;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                return RedirectToPage("./IndexInv");
            }

            var producto = await _context.Productos.FirstOrDefaultAsync(b => b.Id == id);
            if (producto == null)
            {
                return RedirectToPage("./IndexInv");
            }

            Productos = producto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Update(Productos);
            await _context.SaveChangesAsync();
            return RedirectToPage("./IndexInv");
        }
    }
}
