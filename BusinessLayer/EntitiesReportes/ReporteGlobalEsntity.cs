using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class ReporteGlobalEntity
    {
        public string IdEdificio { get; set; }
        public string idInmo { get; set; }
        public string NombreInmueble { get; set; }
        public string IdCliente { get; set; }
        public string idConjunto { get; set; }
        public string idSubConjunto { get; set; }
        public decimal M2Construccion { get; set; }
        public decimal CostoM2 { get; set; }
        public string IdContrato { get; set; }
        public string NombreCliente { get; set; }
        public string Actividad { get; set; }
        public string NombreComercial { get; set; }
        public decimal RentaActual { get; set; }
        public DateTime InicioVigencia { get; set; }
        public DateTime FinVigencia { get; set; }
        public decimal Deposito { get; set; }
        public int DiasGracia { get; set; }
        public bool existeSubconjunto { get; set; }
        public bool esSubconjunto { get; set; }
        public string MonedaContrato { get; set; }
        public decimal mesesDeAdeudo { get; set; }
        public decimal SumaAbonadoMes { get; set; }
        public decimal SumaIntereses { get; set; }
        public DateTime FechaUltimoAbono { get; set; }
        public decimal SaldoAFavor { get; set; }
        public decimal SaldoAFavorPeriodo { get; set; }
        public decimal IVA { get; set; }
        public decimal ISR { get; set; }
        public decimal Importe { get; set; }
        public decimal IVARetenido { get; set; }
        public decimal total { get; set; }
        public decimal TC { get; set; }
        public decimal TCPago { get; set; }
        public decimal Pago { get; set; }
        public decimal descuento { get; set; }
        public string monedaFact { get; set; }
        public string monedaPago { get; set; }
        public decimal Abonos { get; set; }
        public List<ReciboEntity> RecibosPorContrato { get; set; }
        public bool tineAdeudo { get; set; }
        public bool yaVencioContrato { get; set; }


    }
}
