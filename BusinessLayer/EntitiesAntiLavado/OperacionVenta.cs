using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    class OperacionVenta : Operacion
    {
        public int TipoDeInmueble { get; set; }
        public decimal MontoPactado { get; set; }
        public int FiguraDelCliente { get; set; }
        public int FiguraPersona { get; set; }
        public DateTime FechaContrato { get; set; }

        public OperacionVenta()
        {
            FiguraDelCliente = 2;
            FiguraPersona = 1;
        }
    }
}
