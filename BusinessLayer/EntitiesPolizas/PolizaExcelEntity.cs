using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    class PolizaExcelEntity
    {
        public int TipoPoliza { get; set; }
        public List<ColumnasExcelEntity> Encabezados { get; set; }
        public List<MovimientoExcelEntity> Movimientos { get; set; }
    }
}
