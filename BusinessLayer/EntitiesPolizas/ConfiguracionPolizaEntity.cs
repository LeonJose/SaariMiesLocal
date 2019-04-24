using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    public class ConfiguracionPolizaEntity
    {
        public string Inmobiliaria { get; set; }
        /// <summary>
        /// Facturación:
        /// 1.- Diario
        /// 2.- Ingreso
        /// 3.- Cancelados
        /// 4.- Nota de credito
        /// 5.- Ingreso consolidado
        /// Cheques:
        /// 1.- Egreso
        /// 2.- Provisión
        /// </summary>
        public int TipoPoliza { get; set; }
        public List<FormulaPolizaEntity> Formulas { get; set; }
    }
}
