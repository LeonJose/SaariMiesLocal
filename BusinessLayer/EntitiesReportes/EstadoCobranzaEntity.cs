using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class EstadoCobranzaEntity
    {
        public string Id { get; set; }
        /// <summary>
        /// Ubicacion
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// No de FOLIO
        /// </summary>
        public string Serie { get; set; }
        /// <summary>
        /// No de FOLIO
        /// </summary>
        public int? Folio { get; set; }
        public int _rfc { get; set; }
        public string _inmueble { get; set; }
        public decimal PagParciales { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        public string NomArrendatario { get; set; }
        public string _status { get; set; } 
        public decimal _importe { get; set; } 
        public decimal _descuento { get; set; } 
        public decimal _neto { get; set; } 
        public string _moneda { get; set; } 
        public decimal _tipoCambio { get; set; } 
        /// <summary>
        /// Importe
        /// </summary>
        public decimal Importe { get; set; } 
        public decimal _impdls { get; set; } 
        public decimal _imppes { get; set; } 
        public decimal _iva { get; set; } 
        /// <summary>
        /// I.V.A.
        /// </summary>
        public decimal Iva { get; set; } 
        public decimal _ivadls { get; set; } 
        public decimal _ivapes { get; set; } 
        public decimal _isr { get; set; }
        /// <summary>
        /// Sub Total
        /// </summary>
        public decimal Subtotal { get; set; }
        /// <summary>
        /// Ret ISR
        /// </summary>
        public decimal RetISR { get; set; }
        public decimal _isrdls { get; set; }
        public decimal _isrpes { get; set; }
        public decimal _ivaret { get; set; }
        /// <summary>
        /// Ret IVA
        /// </summary>
        public decimal RetIva { get; set; }
        public decimal _ivaretdls { get; set; }
        public decimal _ivaretpes { get; set; }
        /// <summary>
        /// Total condicionado
        /// </summary>
        public decimal _total { get; set; }
        /// <summary>
        /// Total
        /// </summary>
        public decimal Total  { get; set; }
        public decimal _totaldls { get; set; }
        public decimal _totalpes { get; set; }
        public decimal  __importe { get; set; }
        public decimal __iva { get; set; }
        public decimal  __isr { get; set; }
        public decimal ivar { get; set; }
        public decimal __total { get; set; }
        /// <summary>
        /// Periodo
        /// </summary>
        public string Periodo { get; set; }
        /// <summary>
        /// Concepto
        /// </summary>
        public string Concepto { get; set; }
        public DateTime _fechaPago { get; set; }
        /// <summary>
        /// Fecha de Pago
        /// </summary>
        public DateTime? FechaPago { get; set; }
        /// <summary>
        /// Saldo
        /// </summary>
        public decimal Saldo { get; set; } 
        public  DateTime FechaEmision { get; set; }
        public DateTime FechaInicio  { get; set; }
        public DateTime FechaFin { get; set; }
        public string SubConjunto { get; set; }
        //tipoDocumento
        public string TipoDoc { get; set; }

    }
}
