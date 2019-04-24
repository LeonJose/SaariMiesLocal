using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class EgresoEntity
    {
        public DateTime Fecha { get; set; }
        public string NumeroCheque { get; set; }
        public string Estatus { get; set; }
        public string Beneficiario { get; set; }
        public string Concepto { get; set; }
        public string Clasificacion { get; set; }
        public string Conjunto { get; set; }
        public string Inmueble { get; set; }
        public decimal Importe { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public string IDCuenta { get; set; }
        public string Moneda { get; set; }
    }
}
