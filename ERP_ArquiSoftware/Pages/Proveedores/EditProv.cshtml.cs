using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient; // ← para detectar errores de clave única

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

        public List<SelectListItem> CategoriasSelect { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Proveedor = await _context.Proveedores
                .Include(p => p.Categoria)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Proveedor == null)
                return NotFound();

            await CargarCategoriasAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CargarCategoriasAsync();

            if (!ModelState.IsValid)
                return Page();

            // (Opcional) Validar que la categoría exista y esté activa
            var categoriaOk = await _context.Categorias
                .AnyAsync(c => c.Id == Proveedor.CategoriaId && c.Activa);
            if (!categoriaOk)
            {
                ModelState.AddModelError("Proveedor.CategoriaId", "La categoría seleccionada no existe o está inactiva.");
                return Page();
            }

            // ✅ Validar NIT único excluyendo el propio Id
            var nitDuplicado = await _context.Proveedores
                .AnyAsync(p => p.NIT == Proveedor.NIT && p.Id != Proveedor.Id);
            if (nitDuplicado)
            {
                ModelState.AddModelError("Proveedor.NIT", "Ya existe un proveedor con este NIT.");
                return Page();
            }

            try
            {
                // Adjuntar y marcar como modificado
                _context.Attach(Proveedor).State = EntityState.Modified;

                // (Recomendado) No permitir que se modifique la fecha de creación
                _context.Entry(Proveedor).Property(x => x.FechaCreacion).IsModified = false;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                // Colisión por índice único (carrera)
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

