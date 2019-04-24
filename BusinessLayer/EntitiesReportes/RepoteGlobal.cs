using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class RepoteGlobal
    {
       public string IdEdificio { get; set; }
       public string idInmo { get; set; }
       public string NombreInmueble { get; set; }
       public string IdCliente { get; set; }
       public string idConjunto { get; set; }
       public decimal M2Construccion { get; set; }
       public decimal CostoM2 { get; set;}
       public string IdContrato { get; set; }
       public string NombreCliente { get; set; }
       public string Actividad { get; set; }
       public string NombreComercial { get; set; }
       public decimal RentaActual { get; set; }
       public DateTime InicioVigencia { get; set; }
       public DateTime FinVigencia { get; set; }
       public decimal Deposito { get; set; }
       public int DiasGracia { get; set; }
        
    }
}
