using ERP_ArquiSoftware.Models.RRHH;

namespace ERP_ArquiSoftware.Services
{
    public interface INominaService
    {
        Task<Nomina> CalcularYGuardarNominaAsync(
            int empleadoId, int anio, int mes,
            decimal otrosDevengos = 0m, decimal otrasDeducciones = 0m,
            CancellationToken ct = default);

        Task<Liquidacion> GenerarYGuardarLiquidacionAsync(
            int empleadoId, DateTime fechaFinContrato,
            decimal otrasDeducciones = 0m, CancellationToken ct = default);
    }
}


