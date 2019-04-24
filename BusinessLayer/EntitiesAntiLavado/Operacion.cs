using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    class Operacion
    {
        public string ReferenciaInterna { get; set; }
        public DateTime Fecha { get; set; }
        public int Tipo { get; set; }
        public DateTime FechaInicioRenta { get; set; }
        public DateTime FechaFinRenta { get; set; }
        public CaracteristicasInmueble CaracteristicasDelInmueble { get; set; }
        public Liquidacion Liquidacion { get; set; }

        public Operacion()
        {
            Tipo = 1501;
        }
    }
}
