using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntilavado2
{
    public class Liquidacion
    {
        public string ReferenciaInterna { get; set; }
        public DateTime FechaDePago { get; set; }
        /// <summary>
        /// 1 Contado 2 Parcialidades 3 Dación en pago 4 Préstamo o crédito
        /// </summary>
        public int FormaDePago { get; set; }
        public int ClaveInstrumentoMonetario { get; set; }        
        public int Moneda { get; set; }
        public decimal MontoOperacion { get; set; }
    }
}
