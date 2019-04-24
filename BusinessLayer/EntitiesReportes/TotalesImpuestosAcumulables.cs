using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class TotalesImpuestosAcumulables
    {
        public decimal IngresosMesCorrienteImporte1 { get; set; }
        public decimal IngresosMesCorrienteIva1 { get; set; }

        public decimal IngresosMesAnteriorImporte1 { get; set; }
        public decimal IngresosMesAnteriorIva1 { get; set; }

        public decimal TotalIngresosContables { get; set; }
        public decimal TotalIngresosContablesIva { get; set; }

        public decimal TotalIngresosNoCobrados { get; set; }
        public decimal TotalIngresosNoCobradosIva { get; set; }


        public decimal TotalIngresosContabilidad { get{ return TotalIngresosContables - TotalIngresosNoCobrados; } }
        public decimal TotalIngresosContabilidadIva { get { return TotalIngresosContablesIva - TotalIngresosNoCobradosIva; } }

        public decimal TotalIngresoContabiliadCobrado { get; set; }
        public decimal TotalIngresoContabiliadCobradoIva { get; set; }

        public decimal TotalDiferenciasTipoCambio { get { return TotalIngresosContabilidad - TotalIngresoContabiliadCobrado; } }
        public decimal TotalDiferenciasTipoCambioIva { get { return TotalIngresosContabilidadIva - TotalIngresoContabiliadCobradoIva; } }
    }
}
