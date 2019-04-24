using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntilavado2
{
    public class PersonaMoral
    {
        public string RazonSocial { get; set; }
        public DateTime FechaDeConstitucion { get; set; }
        public string RFC { get; set; }
        public string PaisNacionalidad { get; set; }
        public string Giro { get; set; }
        public PersonaFisica Apoderado { get; set; }

        public PersonaMoral()
        {
            PaisNacionalidad = "MX";
        }     
    }
}
