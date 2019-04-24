using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class EncabezadoEntity
    {
        public string Inmobiliaria { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string Usuario { get; set; }
        public string Cliente { get; set; }
        public string RFC { get; set; }
        public string Moneda { get; set; }
        public string Conjunto { get; set; }
        public string SubConjunto { get; set; }
        public string Mes30 { get; set; }
        public string Mes60 { get; set; }
        public string Mes90 { get; set; }
        public string MesMas90 { get; set; }
        public string Cuenta { get; set; }
        public string Clasificacion { get; set; }
        public string TipoCambio { get; set; }
        public string IncluyeIVA { get; set; }
        public string TituloTipoRecibos { get; set; }
        public string IdentificadorCliente { set; get; }
        public string Titulo1 { get; set; }
        public string Titulo2 { get; set; }
    }
}
