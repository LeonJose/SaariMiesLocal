using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntilavado2
{
    public class Informe
    {
        public DateTime MesReportado { get; set; }
        public SujetoObligado SujetoObligado { get; set; }
        public List<Aviso> Avisos { get; set; }
    }
}
