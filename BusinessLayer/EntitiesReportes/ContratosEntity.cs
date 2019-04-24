using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class ContratosEntity
    {
        public string IDContrato { get; set; }
        public string IDInmueble { get; set; }
        public string IDCliente { get; set; }
        public string Inmueble { get; set; }
        public string Cliente { get; set; }
        public bool Vigente { get; set; }
        public string Conjunto { get; set; }
        public string Moneda { get; set; }
        public string Identificador { get; set; }
        public string MonedaCompleta { get { return Moneda == "D" ? "DOLAR" : "PESO"; } }
        public string IDArrendadora { get; set; }
        public string RutaArchivo { get; set; }
        /// <summary>
        /// Tipo de contrato R=Renta  - V=Venta
        /// </summary>
        public string TipoContrato { get; set; }
        public string NombreInmobiliaria { get; set; }
        //Se agregan estas notas con primer finalidad de incluirlas como tarjetas de acceso en el reporte de estado de cuenta por contrato
        public string Nota1 { get; set; }
        public string Nota2 { get; set; }
        public string Nota3 { get; set; }
        public string Nota4 { get; set; }
        public string Nota5 { get; set; }
        // Se agregan campos para reproteGlobal
        public string RazonSocial { get; set; }
        public string Actividad { get; set; }
        public string NombreComercial { get; set;
        
        }

    }
}
