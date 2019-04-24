using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.Helpers
{
    public class filtroReportes
    {
        public string IdInmobiliaria { get; set; }
        public string NombreInmobiliaria { get; set; }
        public string IdConjunto { get; set; }
        public string NombreConjunto { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool esPDF { get; set; }
        public string Usuario { get; set; }
        public string RutaFormato { get; set; }

       

    }
}
