using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntilavado2
{
    public class PersonaFisica
    {
        public string ReferenciaInterna { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string PaisNacionalidad { get; set; }
        public string ActividadEconomica { get; set; }

        public PersonaFisica()
        {
            PaisNacionalidad = "MX";
        }
    }
}
