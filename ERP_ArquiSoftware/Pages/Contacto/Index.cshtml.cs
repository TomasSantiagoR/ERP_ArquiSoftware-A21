using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERP.Pages.Contacto
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ContactInputModel Input { get; set; } = new();

        public bool Submitted { get; set; }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                return;
            }

            // Aquí podrías guardar en BD, enviar correo, etc.
            Submitted = true;
        }

        public class ContactInputModel
        {
            [Required(ErrorMessage = "El nombre es obligatorio")]
            [Display(Name = "Nombre completo")]
            [StringLength(100)]
            public string Nombre { get; set; } = string.Empty;

            [Required(ErrorMessage = "El correo es obligatorio")]
            [EmailAddress(ErrorMessage = "Ingresa un correo válido")]
            [Display(Name = "Correo electrónico")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "El mensaje es obligatorio")]
            [Display(Name = "Mensaje")]
            [StringLength(500, ErrorMessage = "El mensaje no puede exceder los 500 caracteres")]
            public string Mensaje { get; set; } = string.Empty;
        }
    }
}
