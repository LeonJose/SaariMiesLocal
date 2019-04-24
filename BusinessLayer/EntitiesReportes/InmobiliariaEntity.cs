using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class InmobiliariaEntity
    {
        public string ID { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public string RFC { get; set; }
        public string Razon_Nombre { get { return RazonSocial + " -  " + NombreComercial; } }
        public string IdArrendadora { get; set; }
    }
}
