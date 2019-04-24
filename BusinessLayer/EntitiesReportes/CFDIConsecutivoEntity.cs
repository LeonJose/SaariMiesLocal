using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class CFDIConsecutivoEntity
    {
        public int IDHistRec { get; set; }
        public bool EsCancelada { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Serie { get; set; }
        public int Folio { get; set; }
        public string NombreCliente { get; set; }
        public string NombreComercialCliente { get; set; }
        public string IdInmueble { get; set; }
        public string NombreInmueble { get; set; }
        public string NombreConjunto { get; set; }
        public string Periodo { get; set; }
        public decimal ImporteFac { get; set; }
        public decimal IVAFac { get; set; }
        public decimal TotalFac { get; set; }
        public string NombreSubConj { get; set; }
        public string NombreSubConjunto { get; set; }
        public string IdContrato { get; set; }
        public string IdSubConjunto { get; set; }
        /// <summary>
        /// Considerar si es dolar o peso. Importe menos importe de otros cargos
        /// </summary>
        public decimal ImporteRenta { get; set; }
        public decimal DescuentoRenta { get; set; }
        //public decimal DescuentoProntoPago { get; set; }
        //public decimal ImporteRentaVariable { get; set; }
        //public decimal ImporteMantenimiento { get; set; }
        public decimal ImporteVariable1 { get; set; }
        public decimal ImporteVariable2 { get; set; }
        public decimal ImporteVariable3 { get; set; }
        public decimal ImporteVariable4 { get; set; }
        public decimal ImporteVariable5 { get; set; }
        public decimal ImporteVariable6 { get; set; }
        public decimal ImporteVariable7 { get; set; }
        public decimal ImporteVariable8 { get; set; }
        public decimal ImporteVariable9 { get; set; }
        public decimal ImporteVariable10 { get; set; }
        public decimal ImporteVariable11 { get; set; }
        public decimal ImporteVariable12 { get; set; }
        public decimal ImporteVariable13 { get; set; }
        public decimal ImporteVariable14 { get; set; }
        public decimal ImporteVariable15 { get; set; }
        public decimal ImporteVariable16 { get; set; }
        public decimal ImporteVariable17 { get; set; }
        public decimal ImporteVariable18 { get; set; }
        public decimal ImporteVariable19 { get; set; }
        public decimal ImporteVariable20 { get; set; }
        public decimal ImporteVariable21 { get; set; }
        public decimal ImporteVariable22 { get; set; }
        public decimal ImporteVariable23 { get; set; }
        public decimal ImporteVariable24 { get; set; }
        public decimal ImporteVariable25 { get; set; }
        public decimal ImporteRentasAnticipadas { get; set; }
        /// <summary>
        /// Debe ser igual al importe de la factura
        /// </summary>
        public decimal SubTotal
        {
            get
            {
                return ImporteRenta - DescuentoRenta /*- DescuentoProntoPago + ImporteRentaVariable + ImporteMantenimiento*/ + ImporteVariable1 + ImporteVariable2
                    + ImporteVariable3 + ImporteVariable4 + ImporteVariable5 + ImporteVariable6 + ImporteVariable7 + ImporteVariable8 + ImporteVariable9 +
                    ImporteVariable10 + ImporteVariable11 + ImporteVariable12 + ImporteVariable13 + ImporteVariable14 + ImporteVariable15 +
                    ImporteVariable16 + ImporteVariable17 + ImporteVariable18 + ImporteVariable19 + ImporteVariable20 + ImporteVariable21 +
                    ImporteVariable22 + ImporteVariable23 + ImporteVariable24 + ImporteVariable25 + ImporteRentasAnticipadas + OtrosImportes;
            }
        }
        /// <summary>
        /// Debe ser igual al IVA de la factura
        /// </summary>
        public decimal IVA { get { return SubTotal * 0.16m; } }
        public decimal ISR { get; set; }
        public decimal RetIVA { get; set;}        /// <summary>
        /// Debe ser igual al total de la factura
        /// </summary>
        public decimal Total { get { return SubTotal + IVA; } }
        public decimal TotalCargos { get; set; }
        public string Moneda { get; set; }
        public decimal TipoDeCambio { get; set; }
        public int IDCargo { get; set; }
        public string TipoDocumento { get; set; }
        public string IDSubtipo { get; set; }
        public decimal OtrosImportes { get; set; }
        public bool EsRentaAnticipada { get; set; }
        public string MetodoPago { get; set; }
        public string FormaPago { get; set; }
    }
}
