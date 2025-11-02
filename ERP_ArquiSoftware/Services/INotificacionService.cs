using ERP_ArquiSoftware.Models;

namespace ERP_ArquiSoftware.Services
{
    public interface INotificacionService
    {
        Task EvaluarBajoStockAsync(
            int productoId,
            int almacenId,
            int stockActual,
            int? puntoReorden,
            string? productoNombre,
            string? almacenNombre,
            CancellationToken ct = default);

        Task<List<Notificacion>> ListAllAsync(CancellationToken ct = default);
        Task MarkAsReadAsync(int id, CancellationToken ct = default);
        Task MarkAllAsReadAsync(CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task DeleteAllAsync(CancellationToken ct = default);
    }
}






