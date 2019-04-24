using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    class SujetoObligado
    {
        public string ClaveRFC { get; set; }
        public string Actividad { get; set; }

        public SujetoObligado()
        {
            Actividad = "ARI";
        }
    }
}
