using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class SaldoEntity
    {
        public int NumeroRecibo { get; set; }
        public decimal Total { get; set; }
        public string Moneda { get; set; }
        public string TipoDocumento { get; set; }
        public decimal PagoParcial { get; set; }
        public DateTime FechaPagado { get; set; }
        public decimal TipoCambioPago { get; set; }
        public decimal TipoCambio { get; set; }
        public string MonedaPago { get; set; }
        public string Estatus { get; set; }
        public decimal Cartera { get; set; }
        public decimal S30 { get; set; }
        public decimal S60 { get; set; }
        public decimal S90 { get; set; }
        public decimal SMas90 { get; set; }
        public string IDInmobiliaria { get; set; }
    }
}
