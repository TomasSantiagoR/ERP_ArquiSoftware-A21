using ERP_ArquiSoftware.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERP_ArquiSoftware.Pages.Notificaciones
{
    public class IndexModel : PageModel
    {
        private readonly INotificacionService _notis;
        public IndexModel(INotificacionService notis) => _notis = notis;

        public record Row(
            int Id,
            DateTime Fecha,
            string Titulo,
            string Mensaje,
            string Tipo,
            bool Leida
        );

        public List<Row> Items { get; set; } = new();

        public async Task OnGetAsync()
        {
            var data = await _notis.ListAllAsync();
            Items = data.Select(n => new Row(
                n.Id,
                n.Creado,
                n.Titulo,
                n.Mensaje,
                n.Tipo,
                n.Visto
            )).ToList();
        }

        public async Task<IActionResult> OnPostMarkReadAsync(int id)
        {
            await _notis.MarkAsReadAsync(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostMarkAllReadAsync()
        {
            await _notis.MarkAllAsReadAsync();
            return RedirectToPage();
        }

        // NUEVO: borrar una
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _notis.DeleteAsync(id);
            return RedirectToPage();
        }

        // NUEVO: borrar todas
        public async Task<IActionResult> OnPostDeleteAllAsync()
        {
            await _notis.DeleteAllAsync();
            return RedirectToPage();
        }

        public string BadgeFor(string tipo) => (tipo ?? "").ToLowerInvariant() switch
        {
            "low_stock" => "warning",
            "warning" => "warning",
            "info" => "info",
            "error" => "danger",
            _ => "secondary"
        };
    }
}


