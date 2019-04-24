using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    public class CargoEntity
    {
        public int IDHistRec { get; set; }
        public string Concepto { get; set; }
        public decimal Importe { get; set; }
        public string Subtipo { get; set; }
        public string Moneda { get; set; }
        public decimal IVA { get; set; }
    }
}
