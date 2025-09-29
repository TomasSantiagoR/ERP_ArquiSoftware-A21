using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERP_ArquiSoftware.Pages.Inventario
{
    public class CreateInvModel : PageModel
    {
        private readonly AppDBContext _context;

        public CreateInvModel(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }


        [BindProperty]
        public Producto Productos { get; set; } = default;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Productos.Add(Productos);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Inventario/IndexInv");
        }
        

      
    }
}
