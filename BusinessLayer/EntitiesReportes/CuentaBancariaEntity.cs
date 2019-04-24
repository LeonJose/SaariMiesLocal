using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class CuentaBancariaEntity
    {
        public string ID { get; set; }
        public string IDEmpresa { get; set; }
        public string NumeroCuenta { get; set; }
        public string Descripcion { get; set; }
        public string Banco { get; set; }
        public string Moneda { get; set; }
    }
}
