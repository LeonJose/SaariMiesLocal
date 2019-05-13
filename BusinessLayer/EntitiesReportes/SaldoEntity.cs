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
        public decimal PagoCapital { get; set; }
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
        public decimal M30 { get; set; }
        public decimal M60 { get; set; }
        public decimal M90 { get; set; }
        public decimal MMas90 { get; set; }
        public string IDInmobiliaria { get; set; }
        public decimal InteresMoratorio { get; set; }

        public decimal TotalEngCapital { get { return S30 + S90 + S60; } }
        public decimal TotalMoratoriosE { get { return M30 + M60 + M90; } }
        public decimal TotalEnganche { get { return TotalEngCapital + TotalMoratoriosE; } }

        public decimal TotalMenCapital { get { return S30 + S90 + S60; } }
        public decimal TotalMoratoriosM { get { return M30 + M60 + M90; } }
        public decimal TotalMensualidad { get { return TotalEngCapital + TotalMoratoriosE; } }        

        public decimal TotalCarPerCapital { get { return S30 + S90 + S60; } }
        public decimal TotalMoratoriosCP { get { return M30 + M60 + M90; } }
        public decimal TotalCargosPeriod { get { return TotalEngCapital + TotalMoratoriosE; } }

        public decimal TotalPagoCapital { get; set; }
        public decimal TotalPagoMoratorios { get; set; }
        public decimal TotalPorCobrar { get; set; }

        ////public decimal TotalPagoCapital { get { return TotalEngCapital + TotalMenCapital + TotalCarPerCapital; } }
        ////public decimal TotalPagoMoratorios { get { return TotalMoratoriosE + TotalMoratoriosM + TotalCargosPeriod; } }
        ////public decimal TotalPorCobrar { get { return TotalPagoCapital + TotalPagoMoratorios; } }        

    }
}
