using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
   public  class Fechas
    {
        public DateTime fecha { get; set; }
        public int days { get; set; }
        public int months { get; set; }
        public int years { get; set; }
        public string TipoDoc { get; set; }
    }
}
