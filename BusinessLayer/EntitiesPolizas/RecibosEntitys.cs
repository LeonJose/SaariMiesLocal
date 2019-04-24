using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    public class RecibosEntitys
    {
        /// <summary>
        /// ID Hist Rec
        /// </summary>
        public int Identificador { get; set; }
        public string RazonSocial { get; set; }
        public string RFC { get; set; }
        public string Arrendadora { get; set; }
        public string Cliente { get; set; }
        public string Contrato { get; set; }
        public decimal Importe { get; set; }
        public string Moneda { get; set; }
        public string Concepto { get; set; }
        public decimal IVA { get; set; }
        public decimal TipoDeCambio { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal RetencionIVA { get; set; }
        public decimal RetencionISR { get; set; }
        public int Contador { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public int NumeroRecibo { get; set; }
        public string SerieFolio { get; set; }
        public string Estatus { get; set; }
        public string ClienteRazonSocial { get; set; }
        public string ClienteRFC { get; set; }
        public string Conjunto { get; set; }
        public string Inmueble { get; set; }
        public string Periodo { get; set; }
        public string ClienteNombreComercial { get; set; }
        public decimal TotalCargos { get; set; }
        //public DateTime FechaPagado { get; set; }
        public int IDCFD { get; set; }
        public string UUID { get; set; }
        public string IdentificadorConjunto { get; set; }
        public decimal Descuento { get; set; }
        public bool EsConceptoLibre { get { return Contrato.Contains("FAC"); } }
        public string ConceptoRecibo { get; set; }
        public string TipoContable { get; set; }
        public string TipoDocumento { get; set; }
        public string Sucursal { get; set; }
        public decimal SaldoAFavorGenerado { get; set; }
        public decimal SaldoAFavorAplicado { get; set; }
        //Agregado para a partir de la version 3.2.1.1
        public int IdPagoCobranza{get; set;}
        public decimal PagoTotal { get; set; }
        public decimal IVATotalCobrado { get; set; }
        //Agregado para a partir de la version 3.3.1.9
        public string MonedaPago { get; set; }
        //Agregado a partir de la version 3.3.2.6 para detalle de polizas de NC
        /// <summary>
        /// Id Hist Rec de la factura a la que afecta la NC
        /// </summary>
        public int IdRecRelNC { get; set; }
        /// <summary>
        /// Si es recibo esporadico: true sino: false
        /// </summary>
        public bool EsEsporadico { get; set; }
        public string IdEdificio { get; set; }
        /// <summary>
        /// Tipo de cuenta en coopropiedad. Ejemplo IA
        /// </summary>
        public string TipoCuentaCopropiedad { get; set; }
    }
}
