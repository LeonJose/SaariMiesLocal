using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class ConfiguracionMoratoriosEntity
    {
        public string idContrato { get; set; }
        /// <summary>
        /// Puede tomar los valores Null o P=%fijo - T=TIIE TMP=TIIE+%
        /// </summary>
        public string tipoCalculoMoratorio { get; set; }
        public string idInmo {get; set;}
        /// <summary>
        /// Puede tomar los valores 1= Por dias proporcionales - 2= Por meses completos
        /// </summary>
        public int diasMesesMoratorios { get; set; }
        /// <summary>
        /// Puede tomar los valores 1=Solo al primer recibo - 2=A todos los recibos
        /// </summary>
        public int aplicarDiasGracia { get; set; }
        public int diasDeGracia { get; set; }
        public decimal tasaInteres { get; set; }

    }
}
