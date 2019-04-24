using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    class Liquidacion
    {
        public string ReferenciaInterna { get; set; }
        public DateTime FechaDePago { get; set; }
        public int FormaDePago { get; set; }
        public int ClaveInstrumentoMonetario { get; set; }
        public DetalleInstrumento DetalleDelInstrumentoMonetario { get; set; }
        public int Moneda { get; set; }
        public decimal MontoOperacion { get; set; }
    }
}
