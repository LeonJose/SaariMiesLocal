using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    class EdificiosYContratos
    {
        public EdificioEntity Edificio { get; set; }
        public ContratoEntity Contrato { get; set; }
        public decimal Pago { get; set; }
    }
}
