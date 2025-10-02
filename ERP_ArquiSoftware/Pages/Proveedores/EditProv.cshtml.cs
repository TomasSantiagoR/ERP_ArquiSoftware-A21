using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient; // para detectar errores de clave única

namespace ERP_ArquiSoftware.Pages.Proveedores
{
    public class EditProvModel : PageModel
    {
        private readonly AppDBContext _context;

        public EditProvModel(AppDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Proveedor Proveedor { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Proveedor = await _context.Proveedores
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Proveedor == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validación de NIT único excluyendo el propio Id
            var nitDuplicado = await _context.Proveedores
                .AnyAsync(p => p.NIT == Proveedor.NIT && p.Id != Proveedor.Id);
            if (nitDuplicado)
            {
                ModelState.AddModelError("Proveedor.NIT", "Ya existe un proveedor con este NIT.");
            }

            if (!ModelState.IsValid)
                return Page();

            try
            {
                // Adjuntar y marcar como modificado
                _context.Attach(Proveedor).State = EntityState.Modified;

                // No permitir modificar la fecha de creación
                _context.Entry(Proveedor).Property(x => x.FechaCreacion).IsModified = false;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                ModelState.AddModelError("Proveedor.NIT", "Ya existe un proveedor con este NIT.");
                return Page();
            }
            catch (DbUpdateConcurrencyException)
            {
                var existe = await _context.Proveedores.AnyAsync(p => p.Id == Proveedor.Id);
                if (!existe)
                    return NotFound();
                throw;
            }

            return RedirectToPage("/Proveedores/IndexProv");
        }

        // Detecta violación de UNIQUE/PK en distintos motores
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
