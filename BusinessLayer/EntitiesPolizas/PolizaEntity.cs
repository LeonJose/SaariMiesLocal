using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    class PolizaEntity
    {
        public EncabezadoPolizaEntity Encabezado { get; set; }
        public List<MovimientoPolizaEntity> Movimientos { get; set; }
        public List<ConceptoGeneralEntity> ConceptosGenerales { get; set; }
    }
}
