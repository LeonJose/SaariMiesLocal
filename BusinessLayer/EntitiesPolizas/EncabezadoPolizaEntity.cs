using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    class EncabezadoPolizaEntity
    {
        public DateTime Fecha { get; set; }
        /// <summary>
        /// Tipo de poliza que acepta Contpaq 
        /// 1.- Ingresos
        /// 2.- Egresos
        /// 3.- Diario
        /// 4.- De orden
        /// 5.- Estadísticas
        /// </summary>
        public int TipoContpaq { get; set; }
    }
}
