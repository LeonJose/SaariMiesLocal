using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class ConjuntoInmobiliaria
    {
        public string IDContrato { get; set; }
        public int IDPago { get; set; }
        public string IDInmobiliaria { get; set; }        
        public string IDConjunto { get; set; }
        public string Conjunto { get; set; }
        public string IdHistRec { get; set; }

    }
}
