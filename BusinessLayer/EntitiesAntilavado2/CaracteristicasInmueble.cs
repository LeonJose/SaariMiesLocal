using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntilavado2
{
    public class CaracteristicasInmueble
    {
        public string ReferenciaInterna { get; set; }
        /// <summary>
        /// Fecha de inicio de la renta
        /// </summary>
        public DateTime FechaInicio { get; set; }
        /// <summary>
        /// Fecha de fin de la renta
        /// </summary>
        public DateTime FechaFin { get; set; }
        public int TipoDeInmueble { get; set; }
        /// <summary>
        /// Valor catastral
        /// </summary>
        public decimal ValorReferencia { get; set; }
        public Domicilio Domicilio { get; set; }
        public string FolioReal { get; set; }
        public string SubConjunto { get; set; }
        /// <summary>
        /// En metros cuadrados
        /// </summary>
        public decimal DimensionTerreno { get; set; }
        /// <summary>
        /// En metros cuadrados
        /// </summary>
        public decimal DimensionConstruccion { get; set; }
    }
}
