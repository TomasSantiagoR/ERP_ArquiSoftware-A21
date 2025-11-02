using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_ArquiSoftware.Models.RRHH
{
    public class NovedadNomina
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }

        // Ausencias (en días)
        public int DiasIncapacidadEPS { get; set; }
        public int DiasLicenciaRemunerada { get; set; }
        public int DiasNoRemunerados { get; set; }
        public int DiasVacacionesTomadas { get; set; }

        // Horas
        [Column(TypeName = "decimal(18,2)")] public decimal HED { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal HEN { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal HEDFest { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal HENFest { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal RecargoNocturno { get; set; }

        // Variables económicas
        [Column(TypeName = "decimal(18,2)")] public decimal Comisiones { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Bonificaciones { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal AuxilioNoConstitutivo { get; set; }

        // Descuentos
        [Column(TypeName = "decimal(18,2)")] public decimal PrestamoEmpresa { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Embargo { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal OtrasDeducciones { get; set; }
    }
}
