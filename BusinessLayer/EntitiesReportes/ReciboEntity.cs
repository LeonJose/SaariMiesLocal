using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class ReciboEntity
    {
        public int IDHistRec { get; set; }
        public int IDCFD { get; set; }
        public string UUID { get; set; }
        public int Numero { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaPago { get; set; }
        public DateTime? FechaCancelado { get; set; }
        /*public decimal CargoPesos { get; set; }
        public decimal AbonoPesos { get; set; }*/
        public decimal TipoCambioPago { get; set; }
        /*public decimal CargoDolares { get; set; }
        public decimal AbonoDolares { get; set; }*/
        public string Periodo { get; set; }
        public string TipoDoc { get; set; }
        public decimal Total { get; set; }
        public string Moneda { get; set; }
        public string NombreMoneda { get { return Moneda == "P" ? "PESOS" : "DOLARES"; } }
        public decimal Pago { get; set; }
        public string Estatus { get; set; }
        public decimal PagoParcial { get; set; }
        public string Serie { get; set; }
        public int? Folio { get; set; }
        public decimal Cargo { get; set; }
        public decimal Abono { get; set; }
        public string MonedaPago { get; set; }
        public string Concepto { get; set; }
        public string Inmueble { get; set; }
        public string NombreInmueble { get; set; }
        public string IDCliente { get; set; }
        public string NombreCliente { get; set; }
        public string IDConjunto { get; set; }
        public string NombreConjunto { get; set; }
        public decimal CantidadPorPagar { get; set; }
        public decimal Importe { get; set; }
        public DateTime? VencimientoPago { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal TotalIVA { get; set; }
        public decimal Importe30 { get; set; }
        public decimal Importe60 { get; set; }
        public decimal Importe90 { get; set; }
        public decimal ImporteMas90 { get; set; }
        public decimal Descuento { get; set; }
        public string IDInmobiliaria { get; set; }
        public string Referencia { get; set; }
        public string RFCCliente { get; set; }
        public decimal IVA { get; set; }
        public decimal ISR { get; set; }
        public decimal IVARetenido { get; set; }
        public decimal TasaIVA { get; set; }
        public List<ReciboEntity> Pagos { get; set; }
        public List<ReciboEntity> Importes {get; set;}
        public List<ConceptoEntity> Conceptos { get; set; }
        public List<ReciboEntity> Cargos { get; set; }

        public string nombreComercial { get; set; }
        public decimal TotalAbono
        {
            get
            {                
                return Pagos == null? Abono : Abono + Pagos.Sum(p=>p.Total);
            }
        }
        public decimal Saldo { get { return Cargo - TotalAbono; } }
        public string IDContrato { get; set; }
        //agregado 05/11/2015 by Ing. Rodrigo Uzcanga
        //Tomar saldo a pagar P2427_CTD_PAG
        public decimal CtdPag { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaMoratorios { get; set; }
        public decimal InteresesMoratorios { get; set; }
        public string Info { get; set; }
        public string NombreInmobiliaria { get; set; }
        public string RutaPdfCFDI { get; set; }
        public decimal saldoAFavor { get; set; }
        public decimal TotalPagado { get; set; }

        // Agregago para nuevo reporte de recibos cobrados por folio, y para realizar conversion de dolares apartir de la vesion 
        //3.3.8.1 - 08/02/2017
        public string Comentario { get; set; }
        public decimal ConDllstotal { get; set; }
        public decimal ConDllsImporte { get; set; }
        public decimal ConDllsIVA { get; set; }
        public decimal ConDllsISR { get; set; }
        public decimal ConDllsIVARetenido { get; set; }
        public decimal ConDllsTotalPagado { get; set; }
        public decimal ConDllsSaldoFavor { get; set; }
        public string nombreSubconjunto { get; set; }
        public string IDinmueble { get; set; }

        public decimal SumaConDllstotal { get; set; }
        public decimal SumaConDllsImporte { get; set; }
        public decimal SumaConDllsIVA { get; set; }
        public decimal SumaConDllsISR { get; set; }
        public decimal SumaConDllsIVARetenido { get; set; }
        public ClienteEntity cliente { get; set; }
        public decimal SumaConMNstotal { get; set; }
        public decimal SumaConMNImporte { get; set; }
        public decimal SumaConMNIVA { get; set; }
        public decimal SumaConMNISR { get; set; }
        public decimal SumaConMNIVARetenido { get; set; }
        public string TipoPago { get; set; }
        public string Campo20 { get; set; }
        public string MonedaContrato { get; set; }
        public DateTime MaxFechaPago { get; set; }
        public decimal TotalxPagar { get; set; }
        public int IDPago { get; set; }
        public string IdSubconjunto { get; set; }
        public string NombreSubconjunto { get; set; }
        public string IdEdificio { get; set; }
        public string IdCentroSubConjunto { get; set; }
        //public decimal TotalImporte
        //{
        //    get { return Importes == null ? 0 : Importes.Sum(s => s.Importe); }

        //}
    }
}
