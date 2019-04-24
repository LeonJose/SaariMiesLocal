using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    public class FormulaPolizaEntity
    {
        /// <summary>
        /// Tipo completo de la cuenta a emplear. Ejemplo: Complementaria de Bancos
        /// </summary>
        public string Tipo { get; set; }
        /// <summary>
        /// Clave del tipo de cuenta. Ejemplo: IA
        /// </summary>
        public string TipoClave { get; set; }
        /// <summary>
        /// 1.- Cargo
        /// 2.- Abono
        /// </summary>
        public int CargoAbono { get; set; }
        /// <summary>
        /// P.- Pesos
        /// D.- Dolares
        /// </summary>
        public string Moneda { get; set; }
        public string Formula { get; set; }
        /// <summary>
        /// Identifica si una cuenta es subtipo. Inicialmente solo se aceptan para otros ingresos
        /// </summary>
        public bool EsSubtipo { get; set; }
        /// <summary>
        /// Identifica el tipo de Poliza,1:DIARIO, 2:INGRESOS, etc
        /// </summary>
        public int TipoPoliza { get; set; }

        public string ClaseSubTipo { get; set; }
        /// <summary>
        /// P.- Pesos
        /// D.- Dolares
        /// </summary>
        public string MonedaPago { get; set; }
    }
}
