using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class ComprobantePagoEntity
    {
        //add 
        public string Ubicacion { get; set; }
        public DateTime Fechatimbre { get; set; }
        public decimal Importe { get; set; }
        public decimal IVA { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ISR { get; set; }
        public decimal RetIVA { get; set; }
        public string FolioCRP { get; set; }
        public string SerieCRP { get; set; }
        public DateTime FechaTimbrado { get; set; }
     

        public string Serie { get; set; }
        public int Folio { get; set; }
        public string UUID { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaPago { get; set; }
        public string IDCliente { get; set; }
        public string NombreReceptor { get; set; }
        public string Descripcion { get; set; }
        public string Moneda { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal TotalPago { get; set; }
        public decimal Total { get; set; }
        public int IdPago { get; set; }
        public int IdComprobante { get; set; }
        public string MetodoPago { get; set;  }
        public List<DocumentosRelacionados> DocsRelacionados { get; set; }
        public string Periodo { get; set; }
        public int IdCFD { get; set; }
        public string IdHistRec { get; set; }
        public string IdConjunto { get; set; }
        public string Conjunto { get; set; }

    }
}
