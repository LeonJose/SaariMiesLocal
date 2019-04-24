using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    public class EgresoEntity
    {
        public int ID { get; set; }
        public string IDCuentaBanco { get; set; }
        public string Cuenta { get; set; }
        public string NombreCuenta { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Importe { get; set; }
        /// <summary>
        /// Importe de IVA por Acreditar
        /// </summary>
        public decimal ImporteIVA { get; set; }
        public decimal Total { get; set; }
        public string Moneda { get; set; }
        public decimal TipoCambio { get; set; }
        public string Concepto { get; set; }
        public string IDArrendadora { get; set; }
        public string CuentaIVA { get; set; }
        public string NombreCuentaIVA { get; set; }
        public string CuentaProveedores { get; set; }
        public string NombreCuentaProveedores { get; set; }
        public int IDProveedor { get; set; }   
        public List<RegistroEgresoEntity> ListaRegistros { get; set; }
        /// <summary>
        /// Importe de la retención del IVA
        /// </summary>
        public decimal ImporteRetIVA { get; set; }
        /// <summary>
        /// Importe de la retención del ISR
        /// </summary>
        public decimal ImporteRetISR { get; set; }
        /// <summary>
        /// Importe de IVA Acreditado
        /// </summary>
        public decimal ImporteIVAAcred { get; set; }
        public string CuentaRetIVA { get; set; }
        public string CuentaRetISR { get; set; }
        public string CuentaIVAAcred { get; set; }
        public string DescripCuentaRetIVA { get; set; }
        public string DescripCuentaRetISR { get; set; }
        public string DescripCuentaIVAAcred { get; set; }
        public string DescripCuentaIVA { get; set; }
        //add jl 11/02/2019
        public string IdentificadorConjunto { get; set; }
    }
}
