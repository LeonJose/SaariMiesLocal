using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    class ConceptoGeneralEntity
    {
        public int Id { get; set; }
        /// <summary>
        /// El concepto se integra por todas las facturas pagadas con un mismo recibo,
        /// así como el cliente, numero de local
        /// </summary>
        public string Concepto { get; set; }
        /// <summary>
        /// UUIDs se integra por todas las facturas pagadas con un mismo recibo
        /// </summary>
        public string UUIDs { get; set; }

    }
}
