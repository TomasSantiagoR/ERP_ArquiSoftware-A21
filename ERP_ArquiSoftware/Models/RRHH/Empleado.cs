using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ERP_ArquiSoftware.Models.RRHH
{
    public class Empleado
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        [DisplayName("Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [DisplayName("Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required, StringLength(15)]
        [DisplayName("Documento de Identidad")]
        public string Documento { get; set; } = string.Empty;

        [Required, EmailAddress]
        [DisplayName("Correo Corporativo")]
        public string Correo { get; set; } = string.Empty;

        [Phone]
        [DisplayName("Teléfono")]
        public string? Telefono { get; set; }

        [DisplayName("Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        // 🔹 Relación con Almacén
        public int AlmacenId { get; set; }
        public Almacen? Almacen { get; set; }

        // 🔹 Relación con Departamento
        public int DepartamentoId { get; set; }
        public Departamento? Departamento { get; set; }

        // 🔹 Relación con Cargo
        public int CargoId { get; set; }
        public Cargo? Cargo { get; set; }

        // 🔹 Relación con contratos
        public ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();

        [DisplayName("Activo")]
        public bool Activo { get; set; } = true;

        [DisplayName("Fecha de Ingreso")]
        public DateTime FechaIngreso { get; set; } = DateTime.Now;
    }
}


