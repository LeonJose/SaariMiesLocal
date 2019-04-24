using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class CarteraVencidaVentaEntity
    {
        public string IDContrato { get; set; }
        public string Fraccionamiento { get; set; }
        public string Cliente { get; set; }
        public string Manzana { get; set; }
        public string Lote { get; set; }
        public string TotalPagares { get; set; }
        public decimal Total { get; set; }
        public decimal TotalPago { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal ImporteMensual { get; set; }
        public string EstatusFactura { get; set; }
        public string Periodo { get; set; }
        public DateTime? FechaPagado { get; set; }
        public string Observaciones { get; set; }
        public string Estatus { get; set; }
    }
}
