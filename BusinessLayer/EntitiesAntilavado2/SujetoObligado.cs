using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntilavado2
{
    public class SujetoObligado
    {
        private string actividad = string.Empty;

        public string ClaveRFC { get; set; }
        public string Actividad { get; set; }

        public SujetoObligado()
        {
            Actividad = "NON";
        }

        public SujetoObligado(bool esVenta)
        {
            if (esVenta)
                Actividad = "INM";
            else
                Actividad = "ARI";
        }
    }
}
