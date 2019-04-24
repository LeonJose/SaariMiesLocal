using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class ReporteImpuestos
    {
        public DateTime FechaEmision { get; set; }
        public int NoRecibo { get; set; }
        public string factura { get; set; }
        public string nombreCliente { get; set; }
        public string  concepto { get; set; }
        public decimal  impuestos { get; set; }
        public string moneda { get; set; }
        public decimal TipoCambioEmision { get; set; }
    }
}
