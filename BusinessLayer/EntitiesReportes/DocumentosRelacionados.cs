using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class DocumentosRelacionados
    {
        public string Ubicacion { get; set; }
        public string Serie { get; set; }
        public int Folio { get; set; }
        public DateTime Fechatimbre { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreReceptor { get; set; }
        public int Importe { get; set; }
        public int IVA { get; set; }
        public int SubTotal { get; set; }
        public int ISR { get; set; }
        public int RetIVA { get; set; }
        public int Total { get; set; }
        public string Concepto { get; set; }
        public string FolioCRP { get; set; }
        public string SerieCRP { get; set; }
        public DateTime FechaTimbre { get; set; }
        public DateTime FechaPago { get; set; }
        public int IdComprobante { get; set; }
        public int ID_CFD { get; set; }
        public string IdHistRec { get; set; }
        public string UUID { get; set; }
        public string Moneda { get; set; }
        public int NumParcialidad { get; set; }
        public decimal ImpSaldoAnt { get; set; }
        public decimal impPagado { get; set; }
        public decimal ImpSaldoInsoluto { get; set; }
        public string Descripcion { get; set; }

        public string SerieFactura { get; set; }
        public int FolioFactura { get; set; }
        public decimal TotalFactura { get; set; }
        public decimal TotalCRP { get; set; }



    }
}
