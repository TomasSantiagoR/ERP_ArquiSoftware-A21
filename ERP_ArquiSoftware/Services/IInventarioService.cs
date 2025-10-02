using System.Threading;

namespace ERP_ArquiSoftware.Services
{
    public interface IInventarioService
    {
        Task DescargarStockPorFacturaAsync(int facturaId, bool withTransaction = true, CancellationToken ct = default);
        Task RevertirStockPorFacturaAsync(int facturaId, bool withTransaction = true, CancellationToken ct = default);
    }
}


