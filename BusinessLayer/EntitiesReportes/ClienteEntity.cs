using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class ClienteEntity
    {
        public string IDCliente { get; set; }
        public string Nombre { get; set; }
        public string RFC { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public DomicilioEntity DomicilioFiscal { get; set; }
        public PersonaFisicaEntity RepresentanteLegal { get; set; }
        public PersonaFisicaEntity Contacto { get; set; }
        public PersonaFisicaEntity Aval { get; set; }
        public string NombreRepresentante { get { return RepresentanteLegal.Nombre; } }
        public string TelefonoRepresentante { get { return RepresentanteLegal.Telefono; } }
        public string NombreContacto { get { return Contacto.Nombre; } }
        public string TelefonoContacto { get { return Contacto.Telefono; } }
        public string NombreAval { get { return Aval.Nombre; } }
        public string TelefonoAval { get { return Aval.Telefono; } }
        public string Domicilio { get { return string.Format("{0} {1} {2}, COLONIA {3}, {4}, {5}, {6}, C.P. {7}", DomicilioFiscal.Calle, DomicilioFiscal.NumeroExterior, DomicilioFiscal.NumeroInterior, DomicilioFiscal.Colonia, DomicilioFiscal.Ciudad, DomicilioFiscal.Estado, DomicilioFiscal.Pais, DomicilioFiscal.CodigoPostal); } }
        public string NombreComercial { get; set; }
        public string RutaArchivo { get; set; }
        public string TipoFactura { get; set; }
        public string IDContrato { get; set; }
        public int TipoEnte { get; set; }
        public string IdentificadorCliente { get; set; }
        public string PeriodoFacturacion { get; set; }
        public string Concepto { get; set; }
        public string IDHistRec { get; set; }

    }
}
