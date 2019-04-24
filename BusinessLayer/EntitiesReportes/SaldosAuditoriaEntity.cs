using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class SaldosAuditoriaEntity
    {
        public decimal MontoCapital { get; set; }
        public decimal MontoIntereses { get; set; }
        public decimal MontoMoratorios { get; set; }
    }
}
