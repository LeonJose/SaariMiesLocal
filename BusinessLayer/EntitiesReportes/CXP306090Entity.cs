using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class CXP306090Entity
    {
        public string IDProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public string NombreComercial { get; set; }
        public string RFC { get; set; }
        public decimal TotalProvision { get { return Total30 + Total60 + Total90 + TotalMas90; } }
        public decimal Total30 { get; set; }
        public decimal Total60 { get; set; }
        public decimal Total90 { get; set; }
        public decimal TotalMas90 { get; set; }
        public List<ProvisionEntity> Provision30 { get; set; }
        public List<ProvisionEntity> Provision60 { get; set; }
        public List<ProvisionEntity> Provision90 { get; set; }
        public List<ProvisionEntity> ProvisionMas90 { get; set; }
        public List<ProvisionEntity> Provisiones 
        { 
            get         
            { 
                return Provision30.Union(Provision60).ToList().Union(ProvisionMas90).ToList().Union(ProvisionMas90).ToList().OrderBy(p => p.IDHistRec).ToList(); 
            } 
        }
        public string IDEmpresa { get; set; }
        public string Moneda { get; set; }
        public string NombreMoneda { get { return Moneda == "P" ? "Pesos" : "Dolares"; } }
        public decimal TotalPesos30 { get; set; }
        public decimal TotalPesos60 { get; set; }
        public decimal TotalPesos90 { get; set; }
        public decimal TotalPesosMas90 { get; set; }
        public decimal TotalPesos { get { return TotalPesos30 + TotalPesos60 + TotalPesos90 + TotalPesosMas90; } }
        public string Cuenta { get; set; }

    }
}
