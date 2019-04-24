using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntilavado2
{
    public class Telefono
    {
        public string ReferenciaInterna { get; set; }
        public string Pais { get; set; }
        public string Numero { get; set; }
        public string EMail { get; set; }

        public Telefono()
        {
            Pais = "MX";
        }
    }
}
