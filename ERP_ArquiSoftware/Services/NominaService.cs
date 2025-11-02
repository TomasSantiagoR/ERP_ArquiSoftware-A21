using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.RRHH;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Services
{
    public class NominaService : INominaService
    {
        private readonly AppDBContext _ctx;
        public NominaService(AppDBContext ctx) => _ctx = ctx;

        private async Task<ParamNomina> GetParamsAsync(int anio, CancellationToken ct)
        {
            var p = await _ctx.ParametrosNomina
                .AsNoTracking()
                .Where(x => x.Activo && x.Anio == anio)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(ct);

            if (p != null) return p;

            p = await _ctx.ParametrosNomina
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderByDescending(x => x.Anio)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(ct);

            if (p == null)
                throw new InvalidOperationException("No existen parámetros de nómina activos.");

            return p;
        }

        public async Task<Nomina> CalcularYGuardarNominaAsync(
            int empleadoId, int anio, int mes,
            decimal otrosDevengos = 0m, decimal otrasDeducciones = 0m,
            CancellationToken ct = default)
        {
            var empleado = await _ctx.Empleados
                .Include(e => e.Contratos.Where(c => c.Activo))
                .ThenInclude(c => c.TipoContrato)
                .FirstOrDefaultAsync(e => e.Id == empleadoId, ct);

            if (empleado == null) throw new InvalidOperationException("Empleado no encontrado.");
            var contrato = empleado.Contratos.FirstOrDefault(c => c.Activo);
            if (contrato == null) throw new InvalidOperationException("El empleado no tiene contrato activo.");

            var prm = await GetParamsAsync(anio, ct);

            var fechaInicio = new DateTime(anio, mes, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            var periodo = await _ctx.PeriodosNomina
                .FirstOrDefaultAsync(p => p.Anio == anio && p.Mes == mes, ct);
            if (periodo == null)
            {
                periodo = new PeriodoNomina
                {
                    Anio = anio,
                    Mes = mes,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Cerrado = false
                };
                _ctx.PeriodosNomina.Add(periodo);
                await _ctx.SaveChangesAsync(ct);
            }
            if (periodo.Cerrado) throw new InvalidOperationException("El período de nómina está cerrado.");

            var desde = contrato.FechaInicio > fechaInicio ? contrato.FechaInicio : fechaInicio;
            var hasta = (contrato.FechaFin.HasValue && contrato.FechaFin.Value < fechaFin) ? contrato.FechaFin.Value : fechaFin;
            if (hasta < desde) hasta = desde;

            var diasPeriodo = (hasta - desde).Days + 1;
            if (diasPeriodo < 0) diasPeriodo = 0;

            var basicoDevengado = Math.Round((contrato.Salario / prm.DiasMes) * diasPeriodo, 2);

            var aplicaAuxilio =
                contrato.Salario <= (prm.SMMLV * prm.TopeAuxTranspMultiplo)
                && (contrato.TipoContrato?.Nombre ?? "") != "Prestación de servicios";

            var auxTransporte = aplicaAuxilio
                ? Math.Round((prm.AuxilioTransporte / prm.DiasMes) * diasPeriodo, 2)
                : 0m;

            var saludEmp = Math.Round(contrato.Salario * prm.SaludEmpleadoPorc * diasPeriodo / prm.DiasMes, 2);
            var pensionEmp = Math.Round(contrato.Salario * prm.PensionEmpleadoPorc * diasPeriodo / prm.DiasMes, 2);

            var provCes = Math.Round(basicoDevengado * prm.CesantiasPorc, 2);
            var provIntCes = Math.Round(provCes * (prm.InteresesCesantiasAnualPorc / 12m), 2);
            var provPrima = Math.Round(basicoDevengado * prm.PrimaPorc, 2);
            var provVac = Math.Round(basicoDevengado * prm.VacacionesPorc, 2);

            var totalDev = basicoDevengado + auxTransporte + Math.Max(0m, otrosDevengos);
            var totalDed = saludEmp + pensionEmp + Math.Max(0m, otrasDeducciones);
            var neto = totalDev - totalDed;

            var nomina = new Nomina
            {
                EmpleadoId = empleado.Id,
                PeriodoNominaId = periodo.Id,
                SalarioBase = contrato.Salario,
                TipoContratoNombre = contrato.TipoContrato?.Nombre ?? "",

                BasicoDevengado = basicoDevengado,
                AuxilioTransporte = auxTransporte,
                OtrosDevengos = otrosDevengos,

                SaludEmpleado = saludEmp,
                PensionEmpleado = pensionEmp,
                OtrasDeducciones = otrasDeducciones,

                TotalDevengado = totalDev,
                TotalDeducciones = totalDed,
                NetoPagar = neto,

                ProvCesantias = provCes,
                ProvInteresesCesantias = provIntCes,
                ProvPrima = provPrima,
                ProvVacaciones = provVac
            };

            _ctx.Nominas.Add(nomina);
            await _ctx.SaveChangesAsync(ct);
            return nomina;
        }

        public async Task<Liquidacion> GenerarYGuardarLiquidacionAsync(
            int empleadoId, DateTime fechaFinContrato,
            decimal otrasDeducciones = 0m, CancellationToken ct = default)
        {
            var empleado = await _ctx.Empleados
                .Include(e => e.Contratos.Where(c => c.Activo))
                .ThenInclude(c => c.TipoContrato)
                .FirstOrDefaultAsync(e => e.Id == empleadoId, ct);

            if (empleado == null) throw new InvalidOperationException("Empleado no encontrado.");
            var contrato = empleado.Contratos.FirstOrDefault(c => c.Activo);
            if (contrato == null) throw new InvalidOperationException("No hay contrato activo para liquidar.");

            var prm = await GetParamsAsync(fechaFinContrato.Year, ct);

            var inicio = contrato.FechaInicio.Date;
            var fin = fechaFinContrato.Date;
            if (fin < inicio) fin = inicio;

            var diasTrab = (fin - inicio).Days + 1;
            if (diasTrab < 0) diasTrab = 0;

            var ces = Math.Round(contrato.Salario * diasTrab / 360m, 2);
            var intCes = Math.Round(ces * prm.InteresesCesantiasAnualPorc * (diasTrab / 360m), 2);
            var prima = Math.Round(contrato.Salario * diasTrab / 360m, 2);
            var vac = Math.Round(contrato.Salario * diasTrab / 720m, 2);

            var neto = (ces + intCes + prima + vac) - Math.Max(0m, otrasDeducciones);

            var liq = new Liquidacion
            {
                EmpleadoId = empleado.Id,
                FechaInicio = inicio,
                FechaFin = fin,
                SalarioBase = contrato.Salario,
                DiasTrabajados = diasTrab,
                Cesantias = ces,
                InteresesCesantias = intCes,
                Prima = prima,
                Vacaciones = vac,
                OtrasDeducciones = otrasDeducciones,
                NetoPagar = neto
            };

            contrato.Activo = false;
            contrato.FechaFin = fin;

            _ctx.Liquidaciones.Add(liq);
            await _ctx.SaveChangesAsync(ct);

            return liq;
        }
    }
}

