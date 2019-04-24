using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    class Cheque
    {
        public string InstitucionCredito { get; set; }
        public string NumeroDeCuentaLibrador { get; set; }
        public string NumeroCheque { get; set; }
    }
}
