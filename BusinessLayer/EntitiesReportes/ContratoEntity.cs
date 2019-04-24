using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class ContratoEntity
    {
        public string ID { get; set; }
        public decimal Enganche { get; set; }
        public string NombreCliente { get; set; }
        public string Tipo { get; set; }
        public ClienteEntity Cliente { get; set; }
        public string Moneda { get; set; }
        public string ClienteNombre { get { return Cliente != null ? Cliente.Nombre : string.Empty; } }
        public string ClienteNombreComercial { get { return Cliente != null ? Cliente.NombreComercial : string.Empty; } }
        public string IDConjunto { get; set; }
        public string NombreConjunto { get; set; }
        public string Tiempo { get; set; }
        public string Clasificacion { get; set; }
        public string Periodo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaVencimientoProrroga { get; set; }
        public string PorcentajeIncremento { get; set; }
        public DateTime FechaIncremento { get; set; }
        public decimal ImporteOriginal { get; set; }
        public decimal ImporteActual { get; set; }
        public decimal Deposito { get; set; }
        public string IDInmueble { get; set; }
        public string NombreInmueble { get; set; }
        public string NombreInmuebleSubconjunto { get; set; }
        public string IdSubconjunto { get; set; }
        public string IdentificadorInmueble { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Telefono3 { get; set; }
        public string Telefono4 { get; set; }
        public string Telefono5 { get; set; }
        public string Fax1 { get; set; }
        public string Fax2 { get; set; }
        public string CorreoElectronico1 { get; set; }
        public string CorreoElectronico2 { get; set; }
        public int TiempoPeriodo { get; set; }
        public string TipoPeriodo { get; set; }
        public string Actividad { get; set; }
        public string EstatusContrato { get; set; }
        public string TipoPago { get; set; }
        public bool SuspensionPagos { get; set; }
        public string EstatusProrroga { get; set; }
        public string ActividadCliente { get; set; }
        public List<SubtipoEntity> ListaCargos { get; set; }
        //MailWeb
        public int Orden { get; set; }
        public string Direccion { get; set; }
        public List<ContratoEntity> ListaMailWeb { get; set; }
    }
}
