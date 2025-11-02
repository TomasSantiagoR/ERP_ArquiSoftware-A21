using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP_ArquiSoftware.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly AppDBContext _ctx;
        public NotificacionService(AppDBContext ctx) => _ctx = ctx;

        public async Task EvaluarBajoStockAsync(
            int productoId,
            int almacenId,
            int stockActual,
            int? puntoReorden,
            string? productoNombre,
            string? almacenNombre,
            CancellationToken ct = default)
        {
            if (!puntoReorden.HasValue) return;
            if (stockActual > puntoReorden.Value) return;

            var hace24h = DateTime.Now.AddHours(-24);
            var yaExiste = await _ctx.Notificaciones.AsNoTracking()
                .AnyAsync(n => n.Tipo == "LOW_STOCK"
                               && n.ProductoId == productoId
                               && n.AlmacenId == almacenId
                               && !n.Visto
                               && n.Creado >= hace24h, ct);
            if (yaExiste) return;

            var mensaje = $"El producto '{(productoNombre ?? ("#" + productoId))}' en '{(almacenNombre ?? ("#" + almacenId))}' " +
                          $"tiene stock {stockActual} ≤ punto de reorden {(puntoReorden ?? 0)}.";

            _ctx.Notificaciones.Add(new Notificacion
            {
                Tipo = "LOW_STOCK",
                Titulo = "Stock bajo",
                Mensaje = mensaje,
                Severidad = "warning",
                Visto = false,
                Creado = DateTime.Now,
                ProductoId = productoId,
                AlmacenId = almacenId,
                ProductoNombre = productoNombre,
                AlmacenNombre = almacenNombre,
                Stock = stockActual,
                PuntoReorden = puntoReorden
            });

            await _ctx.SaveChangesAsync(ct);
        }

        public Task<List<Notificacion>> ListAllAsync(CancellationToken ct = default)
        {
            return _ctx.Notificaciones
                .AsNoTracking()
                .OrderByDescending(n => n.Creado)
                .ToListAsync(ct);
        }

        public async Task MarkAsReadAsync(int id, CancellationToken ct = default)
        {
            var n = await _ctx.Notificaciones.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (n == null) return;
            if (!n.Visto)
            {
                n.Visto = true;
                await _ctx.SaveChangesAsync(ct);
            }
        }

        public async Task MarkAllAsReadAsync(CancellationToken ct = default)
        {
            var list = await _ctx.Notificaciones.Where(n => !n.Visto).ToListAsync(ct);
            if (list.Count == 0) return;
            foreach (var n in list) n.Visto = true;
            await _ctx.SaveChangesAsync(ct);
        }

        // === NUEVO: borrar una ===
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var n = await _ctx.Notificaciones.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (n == null) return;
            _ctx.Notificaciones.Remove(n);
            await _ctx.SaveChangesAsync(ct);
        }

        // === NUEVO: borrar todas ===
        public async Task DeleteAllAsync(CancellationToken ct = default)
        {
            // Si tu EF/SQL soporta TRUNCATE, puedes usar ExecuteSqlRaw; aquí usamos RemoveRange para compatibilidad.
            var all = await _ctx.Notificaciones.ToListAsync(ct);
            if (all.Count == 0) return;
            _ctx.Notificaciones.RemoveRange(all);
            await _ctx.SaveChangesAsync(ct);
        }
    }
}
