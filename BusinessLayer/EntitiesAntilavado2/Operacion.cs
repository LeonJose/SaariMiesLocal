using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntilavado2
{
    public class Operacion
    {
        private bool venta = false;

        public string ReferenciaInterna { get; set; }
        public DateTime Fecha { get; set; }
        public int Tipo { get; set; }
        public List<CaracteristicasInmueble> Caracteristicas { get; set; }
        public List<Liquidacion> Liquidacion { get; set; }
        public int FiguraCliente { get; set; }
        public int FiguraSO { get; set; }
        public DateTime FechaContrato { get; set; }
        public decimal ValorPactado { get; set; }

        public Operacion(bool venta)
        {
            Tipo = venta ? 501 : 1501;
            FiguraCliente = 2;
            FiguraSO = 1;
            this.venta = venta;
        }
    }
}
