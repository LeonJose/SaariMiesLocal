using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    public class RegistroEgresoEntity
    {
        public int IDHist { get; set; }
        public string NumCuenta { get; set; }
        public string NumCuentaIVA { get; set; }
        public string Concepto { get; set; }
        public decimal Importe { get; set; }
        public decimal ImporteIVA { get; set; }
        public decimal ImporteTotal { get; set; }
        public string Clasificacion { get; set; }
        public string NombreCuenta { get; set; }
        //add jl 11/02/2019
        public string IdentificadorConjunto { get; set; }
    }
}
