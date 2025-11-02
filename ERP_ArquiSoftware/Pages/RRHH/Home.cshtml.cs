using System.Text.Encodings.Web;
using System.Text.Json;
using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.RRHH;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Pages.RRHH
{
    public class HomeModel : PageModel
    {
        private readonly AppDBContext _ctx;
        public HomeModel(AppDBContext ctx) => _ctx = ctx;

        // KPIs
        public int TotalEmpleados { get; set; }
        public int EmpleadosActivos { get; set; }
        public int EmpleadosInactivos { get; set; }
        public int ContratosActivos { get; set; }
        public int ContratosPorVencer { get; set; }

        // Listas para tabla de últimas nóminas
        public record NominaRow(int Id, int EmpleadoId, string Empleado, string Periodo, decimal Neto);
        public List<NominaRow> UltimasNominas { get; set; } = new();

        // Datos para charts
        public List<string> DeptLabels { get; set; } = new();
        public List<int> DeptData { get; set; } = new();

        public List<string> TipoLabels { get; set; } = new();
        public List<int> TipoData { get; set; } = new();

        // JSON para inyectar a Chart.js
        public string DeptLabelsJson { get; set; } = "[]";
        public string DeptDataJson { get; set; } = "[]";
        public string TipoLabelsJson { get; set; } = "[]";
        public string TipoDataJson { get; set; } = "[]";

        public async Task OnGetAsync()
        {
            // ---- KPIs
            TotalEmpleados = await _ctx.Empleados.CountAsync();
            EmpleadosActivos = await _ctx.Empleados.CountAsync(e => e.Activo);
            EmpleadosInactivos = TotalEmpleados - EmpleadosActivos;

            ContratosActivos = await _ctx.Contratos.CountAsync(c => c.Activo);
            var hoy = DateTime.Today;
            var limite = hoy.AddDays(30);
            ContratosPorVencer = await _ctx.Contratos
                .CountAsync(c => c.Activo && c.FechaFin.HasValue && c.FechaFin.Value >= hoy && c.FechaFin.Value <= limite);

            // ---- Últimas nóminas (5)
            UltimasNominas = await _ctx.Nominas
                .AsNoTracking()
                .Include(n => n.Empleado)
                .Include(n => n.Periodo)
                .OrderByDescending(n => n.Id)
                .Take(5)
                .Select(n => new NominaRow(
                    n.Id,
                    n.EmpleadoId,
                    n.Empleado.Nombres + " " + n.Empleado.Apellidos,
                    $"{n.Periodo.Anio}-{n.Periodo.Mes:D2}",
                    n.NetoPagar
                ))
                .ToListAsync();

            // ---- Chart: Empleados activos por departamento
            var dept = await _ctx.Empleados
                .AsNoTracking()
                .Where(e => e.Activo)
                .Include(e => e.Departamento)
                .GroupBy(e => e.Departamento != null ? e.Departamento.Nombre : "Sin departamento")
                .Select(g => new { Label = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            DeptLabels = dept.Select(x => x.Label).ToList();
            DeptData = dept.Select(x => x.Count).ToList();

            // ---- Chart: Contratos activos por tipo
            var tipos = await _ctx.Contratos
                .AsNoTracking()
                .Where(c => c.Activo)
                .Include(c => c.TipoContrato)
                .GroupBy(c => c.TipoContrato != null ? c.TipoContrato.Nombre : "Sin tipo")
                .Select(g => new { Label = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            TipoLabels = tipos.Select(x => x.Label).ToList();
            TipoData = tipos.Select(x => x.Count).ToList();

            // Serializar a JSON sin escapar excesivo (para inline JS)
            var jsonOpts = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            DeptLabelsJson = JsonSerializer.Serialize(DeptLabels, jsonOpts);
            DeptDataJson = JsonSerializer.Serialize(DeptData, jsonOpts);
            TipoLabelsJson = JsonSerializer.Serialize(TipoLabels, jsonOpts);
            TipoDataJson = JsonSerializer.Serialize(TipoData, jsonOpts);
        }
    }
}
