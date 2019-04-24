using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class CXPConfiguracionReporte
    {
        public string IDInmobiliaria { get; set; }
        public string RazonSocialInmobiliaria { get; set; }
        public string NombreComercialInmobiliaria { get; set; }
        public DateTime FechaCorte { get; set; }
        public string Moneda { get; set; }
        public bool IncluirCancelados { get; set; }
        public string IDConjunto { get; set; }
        public string Clasificacion { get; set; }
        public bool EsDetallado { get; set; }
        public bool EsPdf { get; set; }
        public bool IncluirIVA { get; set; }
        public decimal TipoCambio { get; set; }
        public bool EsPorConjunto { get; set; }
        public string RutaFormato { get; set; }
        public string Usuario { get; set; }
    }
}
