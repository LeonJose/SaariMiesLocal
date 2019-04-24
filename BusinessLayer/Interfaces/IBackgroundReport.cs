using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.BusinessLayer.Interfaces
{
    interface IBackgroundReport
    {
        /// <summary>
        /// Indica si hay una cancelación del usuario pendiente
        /// </summary>
        bool CancelacionPendiente { get; }
        /// <summary>
        /// Evento que se dispara cuando el progreso del reporte cambió
        /// </summary>
        event EventHandler<CambioProgresoEventArgs> CambioProgreso;
        /// <summary>
        /// Método que detiene la realización del reporte
        /// </summary>
        void cancelar();
    }
}
