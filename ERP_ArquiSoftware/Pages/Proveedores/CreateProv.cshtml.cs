using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient; // para detectar código de error de SQL Server
using ProveedorModel = ERP_ArquiSoftware.Models.Proveedor;

namespace ERP_ArquiSoftware.Pages.Proveedores
{
    public class CreateProvModel : PageModel
    {
        private readonly AppDBContext _context;

        public CreateProvModel(AppDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProveedorModel Proveedor { get; set; } = new();

        public void OnGet()
        {
            // Ya no hay categorías que cargar
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validación previa de NIT único (evita llegar a la excepción)
            var nitDuplicado = await _context.Proveedores
                .AnyAsync(p => p.NIT == Proveedor.NIT);

            if (nitDuplicado)
            {
                ModelState.AddModelError("Proveedor.NIT", "Ya existe un proveedor con este NIT.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Proveedor.FechaCreacion == default)
                Proveedor.FechaCreacion = DateTime.Now;

            try
            {
                _context.Proveedores.Add(Proveedor);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Proveedores/IndexProv");
            }
            // Manejo de error por índice único (carrera de concurrencia)
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                ModelState.AddModelError("Proveedor.NIT", "Ya existe un proveedor con este NIT.");
                return Page();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "No se pudo guardar el proveedor. Intenta de nuevo.");
                return Page();
            }
        }

        // Detecta violación de restricción única según el proveedor de BD
        private static bool IsUniqueViolation(DbUpdateException ex)
        {
            // SQL Server: 2627 (unique/PK), 2601 (duplicated key)
            if (ex.InnerException is SqlException sqlEx)
                return sqlEx.Number == 2627 || sqlEx.Number == 2601;

            // Fallback genérico
            return ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true
                   || ex.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase);
        }
    }
}
