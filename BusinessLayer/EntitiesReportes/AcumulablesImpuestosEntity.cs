using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class AcumulablesImpuestosEntity
    {

        public DateTime FechaEmision { get; set; }
        public string FechaPago { get; set; }
        public int Referencia { get; set; }
        public int ID_CFD { get; set; }
        public string UUID { get; set; }
        public string Serie { get; set; }
        public int Folio { get; set; }
        public string CFDi { get { return Serie + " " + Folio; }}
        public string Bodega { get; set; }
        public string TipoDoc { get; set; }
        public string Moneda { get; set; }
        public string Estatus { get; set; }
        public bool EsMesCorriente { get; set; }
    public string PagoParcial { get; set; }
        public decimal ImporteEmision { get; set; }
        public decimal IvaEmision { get; set; }
        public decimal TotalEmision { get; set; }
        public decimal TCEmision { get; set; }

        public decimal ImporteConversion { get; set; }
        public decimal IvaConversion { get; set; }
        public decimal TotaConversion { get; set; }

        public decimal ImporteConversion2 { get; set; }
        public decimal IvaConversion2 { get; set; }
        public decimal TotaConversion2{ get; set; }

        public decimal ImporteCobro { get; set;}
        public decimal TCCobro { get; set; }

        public decimal TotalNocobrado { get; set; }
        public List<TotalesImpuestosAcumulables> totales { get; set; }
    }
}
