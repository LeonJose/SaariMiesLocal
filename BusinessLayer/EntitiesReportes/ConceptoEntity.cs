using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class ConceptoEntity
    {
        public int IDHistRec { get; set; }
        public string IDCargo { get; set; }
        public string Concepto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Importe { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string Moneda { get; set; }
        public decimal TC { get; set; }
        public decimal RetencionIVA { get; set; }
        public decimal ISR { get; set; }
    }
}
