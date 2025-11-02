using ERP_ArquiSoftware.dA;
using ERP_ArquiSoftware.Models.Facturacion;
using ERP_ArquiSoftware.Models.Inventario;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage; // IDbContextTransaction

namespace ERP_ArquiSoftware.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly AppDBContext _ctx;
        private readonly INotificacionService _notifs;

        public InventarioService(AppDBContext ctx, INotificacionService notifs)
        {
            _ctx = ctx;
            _notifs = notifs;
        }

        public async Task DescargarStockPorFacturaAsync(int facturaId, bool withTransaction = true, CancellationToken ct = default)
        {
            // 1) Cargar factura con líneas
            var f = await _ctx.FacturasVenta
                .Include(x => x.Lineas)
                .FirstOrDefaultAsync(x => x.Id == facturaId, ct);

            if (f == null) throw new InvalidOperationException("Factura no encontrada.");

            // 2) Abrir transacción SOLO si me lo piden y no hay una activa
            IDbContextTransaction? tx = null;
            if (withTransaction && _ctx.Database.CurrentTransaction == null)
            {
                tx = await _ctx.Database.BeginTransactionAsync(ct);
            }

            try
            {
                // 3) Descontar por cada línea
                foreach (var l in f.Lineas)
                {
                    var ex = await _ctx.Existencias
                        .FirstOrDefaultAsync(e => e.ProductoId == l.ProductoId && e.AlmacenId == f.AlmacenId, ct);

                    if (ex == null)
                        throw new InvalidOperationException($"No existe stock en almacén {f.AlmacenId} para producto {l.ProductoId}.");

                    if (ex.Stock < l.Cantidad)
                        throw new InvalidOperationException($"Stock insuficiente para producto {l.ProductoId}. Disponible {ex.Stock}, requerido {l.Cantidad}.");

                    // ↓ Descontar stock
                    ex.Stock -= l.Cantidad;

                    // Registrar movimiento
                    _ctx.MovimientosInventario.Add(new MovimientoInventario
                    {
                        Fecha = DateTime.Now,
                        Tipo = "VENTA",
                        AlmacenId = f.AlmacenId,
                        ProductoId = l.ProductoId,
                        Cantidad = -l.Cantidad,
                        DocumentoTipo = "FacturaVenta",
                        DocumentoId = f.Id
                    });

                    // === Notificación de bajo stock ===
                    // Cargar datos (sin tracking para no contaminar el change tracker)
                    var prod = await _ctx.Productos
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == l.ProductoId, ct);

                    var alm = await _ctx.Almacenes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.Id == f.AlmacenId, ct);

                    // Disparar evaluación de bajo stock
                    await _notifs.EvaluarBajoStockAsync(
                        productoId: l.ProductoId,
                        almacenId: f.AlmacenId,
                        stockActual: ex.Stock,
                        puntoReorden: prod?.PuntoReorden,
                        productoNombre: prod?.NombreProducto,
                        almacenNombre: alm?.Nombre,
                        ct: ct
                    );
                }

                await _ctx.SaveChangesAsync(ct);

                if (tx != null) await tx.CommitAsync(ct);
            }
            catch
            {
                if (tx != null) await tx.RollbackAsync(ct);
                throw;
            }
        }

        public async Task RevertirStockPorFacturaAsync(int facturaId, bool withTransaction = true, CancellationToken ct = default)
        {
            var movs = await _ctx.MovimientosInventario
                .Where(m => m.DocumentoTipo == "FacturaVenta" && m.DocumentoId == facturaId)
                .ToListAsync(ct);

            if (!movs.Any()) return;

            IDbContextTransaction? tx = null;
            if (withTransaction && _ctx.Database.CurrentTransaction == null)
            {
                tx = await _ctx.Database.BeginTransactionAsync(ct);
            }

            try
            {
                foreach (var m in movs)
                {
                    var ex = await _ctx.Existencias
                        .FirstOrDefaultAsync(e => e.ProductoId == m.ProductoId && e.AlmacenId == m.AlmacenId, ct);

                    if (ex == null)
                    {
                        ex = new Existencia { ProductoId = m.ProductoId, AlmacenId = m.AlmacenId, Stock = 0 };
                        _ctx.Existencias.Add(ex);
                    }

                    // Si el movimiento original fue -N, ahora sumo N
                    ex.Stock += (-m.Cantidad);

                    _ctx.MovimientosInventario.Add(new MovimientoInventario
                    {
                        Fecha = DateTime.Now,
                        Tipo = "ANULACION_VENTA",
                        AlmacenId = m.AlmacenId,
                        ProductoId = m.ProductoId,
                        Cantidad = -m.Cantidad, // invierto el signo
                        DocumentoTipo = "FacturaVenta",
                        DocumentoId = facturaId
                    });
                }

                await _ctx.SaveChangesAsync(ct);

                if (tx != null) await tx.CommitAsync(ct);
            }
            catch
            {
                if (tx != null) await tx.RollbackAsync(ct);
                throw;
            }
        }
    }
}



