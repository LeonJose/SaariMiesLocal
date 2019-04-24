using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.Entities
{
    /// <summary>
    /// Entidad para los datos del módulo de cobranza instalado (clave, version, fecha)
    /// </summary>
    public class ModuloCobranzaEntity
    {
        /// <summary>
        /// Clave que identifica al registro del módulo de cobranza instalado
        /// </summary>
        public string Clave { get; set; }
        /// <summary>
        /// Versión del módulo de cobranza instalado
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Fecha en la que se instaló la ultima actualización del módulo de cobranza
        /// </summary>
        public DateTime FechaAct { get; set; }
    }
}
