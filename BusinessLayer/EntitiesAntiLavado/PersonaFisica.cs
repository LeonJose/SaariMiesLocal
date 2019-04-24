using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    class PersonaFisica
    {
        public string ReferenciaInterna { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string PaisNacionalidad { get; set; }
        public string PaisNacimiento { get; set; }
        public string ActividadEconomica { get; set; }
        public int TipoIdentificacion { get; set; }
        public string OtraIdentificacion { get; set; }
        public string Autoridad { get; set; }
        public string NumeroIdentificacion { get; set; }

        public PersonaFisica()
        {
            PaisNacimiento = "MX";
            PaisNacionalidad = "MX";
        }
    }
}
