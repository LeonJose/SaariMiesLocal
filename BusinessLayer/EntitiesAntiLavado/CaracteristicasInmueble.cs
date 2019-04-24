using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    class CaracteristicasInmueble
    {
        public string ReferenciaInterna { get; set; }
        public int TipoDeInmueble { get; set; }
        public decimal ValorCatastral { get; set; }
        public Domicilio Domicilio { get; set; }
        public int Blindaje { get; set; }
        public string FolioReal { get; set; }
        public decimal DimensionTerreno { get; set; }
        public decimal DimensionConstruido { get; set; }
        public string SubConjunto { get; set; }

        public CaracteristicasInmueble()
        {
            Blindaje = 2;
        }
    }
}
