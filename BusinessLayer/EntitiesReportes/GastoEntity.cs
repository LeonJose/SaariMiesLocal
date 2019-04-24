using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class GastosEntity
    {
        public int IDHistRecProvision { get; set; }
        public string IDEmpresa { get; set; }
        public string Clasificacion { get; set; }
        public string IDInmueble { get; set; }
        public string NombreInmueble { get; set; }
        public string IDConjunto { get; set; }
        public string IDNombreConjunto { get; set; }
        public string NombreConjunto { get; set; }
        public string CuentaGasto { get; set; }
        public string CuentaIVA { get; set; }
        public string ConceptoGasto { get; set; }
        public decimal ImporteGasto { get; set; }
        public decimal IvaGasto { get; set; }
        public decimal TotalGasto { get; set; }
        public decimal ImporteRetIVA { get; set; }
        public decimal ImporteRetISR { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int Renglon { get; set; }

    }
}
