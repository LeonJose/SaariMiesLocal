using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    class TransferenciaInternacional
    {
        public string InstitucionOrdenante { get; set; }
        public string NumeroDeCuenta { get; set; }
        public string PaisDeOrigen { get; set; }
    }
}
