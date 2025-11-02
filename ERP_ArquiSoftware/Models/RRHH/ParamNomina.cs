using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_ArquiSoftware.Models.RRHH
{
    public class ParamNomina
    {
        public int Id { get; set; }

        // Versión/año de vigencia (puedes usar solo uno “Actual” si prefieres)
        [Range(2000, 2100)]
        public int Anio { get; set; } = DateTime.Now.Year;

        // Valores base
        [Column(TypeName = "decimal(18,2)")]
        public decimal SMMLV { get; set; } = 1300000m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal AuxilioTransporte { get; set; } = 162000m;

        // Topes/reglas
        // Si salario <= (SMMLV * TopeAuxTranspMultiplo) aplica aux transporte
        [Range(1, 10)]
        public int TopeAuxTranspMultiplo { get; set; } = 2;

        // Porcentajes EMPLEADO (descuentos)
        [Column(TypeName = "decimal(5,4)")]
        public decimal SaludEmpleadoPorc { get; set; } = 0.04m; // 4%

        [Column(TypeName = "decimal(5,4)")]
        public decimal PensionEmpleadoPorc { get; set; } = 0.04m; // 4%

        // Porcentajes EMPRESA (provisiones/referencia)
        [Column(TypeName = "decimal(5,4)")]
        public decimal CesantiasPorc { get; set; } = 0.0833m; // ~8.33%

        [Column(TypeName = "decimal(5,4)")]
        public decimal InteresesCesantiasAnualPorc { get; set; } = 0.12m; // 12% anual

        [Column(TypeName = "decimal(5,4)")]
        public decimal PrimaPorc { get; set; } = 0.0833m; // ~8.33%

        [Column(TypeName = "decimal(5,4)")]
        public decimal VacacionesPorc { get; set; } = 0.0417m; // ~4.17%

        // Parámetros de cálculo
        public int DiasMes { get; set; } = 30;

        // Bandera por si quieres “actual” (si manejas varias filas por año)
        public bool Activo { get; set; } = true;
    }
}
