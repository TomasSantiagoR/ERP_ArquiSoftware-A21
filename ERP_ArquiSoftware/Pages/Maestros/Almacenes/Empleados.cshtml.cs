using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using ERP_ArquiSoftware.Models.Inventario;
using ERP_ArquiSoftware.Models.RRHH;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.Maestros.Almacenes
{
    public class EmpleadosModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public EmpleadosModel(AppDBContext ctx) => _ctx = ctx;

        // Ruta /Maestros/Almacenes/Empleados/{id}
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        public int AlmacenId => Id;

        public Almacen? Almacen { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        public List<Empleado> Items { get; set; } = new();

        // Totales
        public int TotalEmpleados { get; set; }
        public int Activos { get; set; }
        public int Inactivos { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Cargar almacén
            Almacen = await _ctx.Almacenes.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == AlmacenId);
            if (Almacen is null) return NotFound();

            // Empleados del almacén con dpt/cargo
            var query = _ctx.Empleados.AsNoTracking()
                .Where(e => e.AlmacenId == AlmacenId)
                .Include(e => e.Departamento)
                .Include(e => e.Cargo)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                var f = Q.Trim();
                query = query.Where(e =>
                    e.Nombres.Contains(f) ||
                    e.Apellidos.Contains(f) ||
                    e.Documento.Contains(f) ||
                    (e.Correo != null && e.Correo.Contains(f)) ||
                    (e.Departamento != null && e.Departamento.Nombre.Contains(f)) ||
                    (e.Cargo != null && e.Cargo.Nombre.Contains(f))
                );
            }

            Items = await query
                .OrderBy(e => e.Nombres).ThenBy(e => e.Apellidos)
                .ToListAsync();

            // Totales
            TotalEmpleados = Items.Count;
            Activos = Items.Count(e => e.Activo);
            Inactivos = TotalEmpleados - Activos;

            return Page();
        }
    }
}
