using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class SubtipoEntity
    {
        public string Identificador { get; set; }
        public string Nombre { get; set; }
        public int Orden { get; set; }
        //Agregado para la sumatoria del reporte Global 
        public decimal suma { get; set; }
        public string IdContrato { get; set; }
        public decimal importeCargo { get; set; }
        public DateTime iniciovigencia { get; set; }
        public DateTime finVigencia { get; set; }
    }
}
