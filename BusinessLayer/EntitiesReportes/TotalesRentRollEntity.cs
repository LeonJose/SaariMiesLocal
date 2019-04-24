using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class TotalesRentRollEntity
    {
        public decimal TotalFacturado { get; set; }
        public decimal TotalCobrado { get; set; }
        public List<BancosRentRoll> Bancos { get; set; }
        public decimal TotalCobrando { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal GranTotal { get; set; }
        public decimal TotalDescuentosPesos { get; set; }
        public decimal TotalDescuentosDolares { get; set; }
        //Sumas totales de movimientos en pesos y dolares(convertidos a pesos)
        public decimal TotalPrecioPromedioDP { get; set; }
        public decimal TotalContratosDP { get; set; }
        public decimal TotalRentaVariableDP { get; set; }
        public decimal TotalMantenimientoDP { get; set; }
        public decimal TotalPublicidadDP { get; set; }
        public decimal TotalServiciosDP { get; set; }
        public decimal TotalOtrosDP { get; set; }
        public decimal TotalEmitidoDP { get; set; }
        public decimal TotalDescuentosDP { get; set; }
        public decimal TotalTotalDP { get; set; }
        public decimal TotalAreaDisponible { get; set; }
        public decimal TotalAraConstruida { get; set; }
        
    }
}
