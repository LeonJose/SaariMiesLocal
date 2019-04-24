using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.Interfaces
{
    interface IReport
    {
        /// <summary>
        /// Propiedad que indicará el nombre del archivo resultado de la generación
        /// </summary>
        string NombreArchivo { get; }
        /// <summary>
        /// Función que deberá contener la lógica para generar el reporte
        /// </summary>
        /// <returns></returns>
        string generar();        
    }
}
