using System.ComponentModel.DataAnnotations;

namespace ERP_ArquiSoftware.Models.RRHH
{
    public class PeriodoNomina
    {
        public int Id { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; } // 1..12

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public bool Cerrado { get; set; } = false;

        public ICollection<Nomina> Nominas { get; set; } = new List<Nomina>();
    }
}

