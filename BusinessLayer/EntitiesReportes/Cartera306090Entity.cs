using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class Cartera306090Entity
    {
        public string IDCliente { get; set; }
        public string RazonSocialCliente { get; set; }
        public string NombreComercialCliente { get; set; }
        public decimal TotalCartera { get { return Total30 + Total60 + Total90 + TotalMas90; } }
        public decimal Total30 { get; set; }
        public decimal Total60 { get; set; }
        public decimal Total90 { get; set; }
        public decimal TotalMas90 { get; set; }
        public List<ReciboEntity> Recibos30 { get; set; }
        public List<ReciboEntity> Recibos60 { get; set; }
        public List<ReciboEntity> Recibos90 { get; set; }
        public List<ReciboEntity> RecibosMas90 { get; set; }
        public List<ReciboEntity> Recibos { get { return Recibos30.Union(Recibos60).ToList().Union(Recibos90).ToList().Union(RecibosMas90).ToList().OrderBy(x => x.Folio).ToList(); } }
        public string IDContrato { get; set; }
        public string MonedaContrato { get; set; }
        public string NombreMonedaContrato { get { return MonedaContrato == "P" ? "Pesos" : "Dolares"; } }
        public decimal TotalPesos30 { get; set; }
        public decimal TotalPesos60 { get; set; }
        public decimal TotalPesos90 { get; set; }
        public decimal TotalPesosMas90 { get; set; }
        public decimal TotalPesos { get { return TotalPesos30 + TotalPesos60 + TotalPesos90 + TotalPesosMas90; } }
        public string Cuenta { get; set; }
        public string NombreContacto { get; set; }
        public string NumeroContacto { get; set; }
        public string IDRubro { get; set; }// Se utiliza para el reporte 30,60,90 x Rubro de Honduras
        public string NombreRubro { get; set; }// Se utiliza para el reporte 30,60,90 x Rubro de Honduras

    }
}
