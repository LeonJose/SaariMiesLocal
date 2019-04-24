using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class ContratosAnalisisEntity
    {
        public string IdContrato { get; set; }
        public string idInmo { get; set; }
        public string IdEdificio { get; set; }
        public string IdCliente { get; set; }
        public string idConjunto { get; set; }
        public string idSubConjunto { get; set; }
        public string NombreInmueble { get; set; }
        public decimal M2Construccion { get; set; }
        public string NombreCliente { get; set; }
        public string Actividad { get; set; }
        public string NombreComercial { get; set; }
        public decimal RentaActual { get; set; }
        public DateTime InicioVigencia { get; set; }
        public DateTime FinVigencia { get; set; }
        public int DiasGracia { get; set; }
        public decimal Deposito { get; set; }
        public string MonedaContrato { get; set; }
        public List<ReciboEntity> HistorialRecibos { get; set; }
    }
}
