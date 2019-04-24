using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    class Informe
    {
        public DateTime MesReportado { get; set; }
        public SujetoObligado SujetoObligado { get; set; }
        public List<Aviso> Avisos { get; set; }
    }
}
