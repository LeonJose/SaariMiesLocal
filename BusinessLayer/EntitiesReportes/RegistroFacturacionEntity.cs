using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class RegistroFacturacionEntity
    {
        public string IDArrendadora { get; set; }
        public string IDCliente { get; set; }
        public string IDContrato { get; set; }
        public string IDInmueble { get; set; }
        public string IDConjunto { get; set; }
        public decimal Total { get; set; }
        public decimal IVA { get; set; }
        public string RazonInmobiliaria { get; set; }
        public string NComercialInmobiliaria { get; set; }
        public string RazonCliente { get; set; }
        public string NComercialCliente { get; set; }
        public string NombreInmueble { get; set; }
        public string IdentificadorInmueble { get; set; }
        public string NombreConjunto { get; set; }
        public decimal Descuento { get; set; }
        public decimal ISR { get; set; }
        public decimal IVARetenido { get; set; }
        public decimal TotalReal { get; set; }
    }
}
