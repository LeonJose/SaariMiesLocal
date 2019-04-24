using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class ProvisionEntity
    {
        public int IDHistRec { get; set; }
        public string IDCuentaBanco { get; set; }
        public string CuentaBanco { get; set; }
        public string IDProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public DateTime FechaGasto { get; set; }
        public DateTime? FechaGeneracionCheque { get; set; }
        public DateTime? FechaImpresionCheque { get; set; }
        public int NumeroCheque { get; set; }                
        public decimal ImporteGasto { get; set; }
        public string IDImpuesto { get; set; }
        public decimal IvaGasto { get; set; }
        public decimal TotalCheque { get; set; }
        public decimal ImporteRetIVA { get; set; }
        public decimal ImporteRetISR { get; set; }
        public string Moneda { get; set; }
        public string NombreMoneda { get { return Moneda == "P" ? "Pesos" : "Dólares"; } }
        public decimal TipoCambio { get; set; }
        public string ConceptoGasto { get; set; }
        public string IDEmpresa { get; set; }
        public string NombreComercialEmpresa { get; set; }
        public string RazonSocialEmpresa { get; set; }
        public string CuentaContableIva { get; set; }
        public string EstatusCheque { get; set; }
        public string EstatusCXP { get; set; }
        //public DateTime FechaCreacion { get; set; }        
        public DateTime? FechaCancelacion { get; set; }
        public List<GastosEntity> ListaGastos { get; set; }
        public string Usuario { get; set; }

        public decimal Importe30 { get; set; }
        public decimal Importe60 { get; set; }
        public decimal Importe90 { get; set; }
        public decimal ImporteMas90 { get; set; }

    }
}
