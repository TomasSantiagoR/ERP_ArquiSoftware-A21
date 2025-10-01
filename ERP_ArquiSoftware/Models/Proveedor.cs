using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_ArquiSoftware.Models
{
    public class Proveedor
    {
        public int Id { get; set; }

        // Datos Generales
        [Required(ErrorMessage = "El nombre o razón social es obligatorio")]
        [DisplayName("Nombre o Razón Social")]
        public string NombreRazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El NIT es obligatorio")]
        [DisplayName("NIT")]
        public string NIT { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de proveedor es obligatorio")]
        [DisplayName("Tipo de Proveedor")]
        public string TipoProveedor { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [DisplayName("Correo Electrónico")]
        public string CorreoElectronico { get; set; } = string.Empty;


        // Datos Comerciales
        [Required(ErrorMessage = "La categoría es obligatoria")]
        [DisplayName("Categoría Productos/Servicios")]
        public int CategoriaId { get; set; }

        [DisplayName("Categoría")]
        public Categoria? Categoria { get; set; }   // 🔗 Navegación a la misma tabla de categorías

        [DisplayName("Condiciones de Pago")]
        public string CondicionesPago { get; set; } = string.Empty;

        [DisplayName("Cuenta Bancaria")]
        public string CuentaBancaria { get; set; } = string.Empty;

        [DisplayName("Representante Legal")]
        public string RepresentanteLegal { get; set; } = string.Empty;


        // Documentos y Estado
        [DisplayName("Documentos")]
        public string Documentos { get; set; } = string.Empty;

        [DisplayName("Activo")]
        public bool Activo { get; set; } = true;

        [DisplayName("Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
