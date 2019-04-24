using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    public class CopropiedadEntity
    {
        public string IDEdificio { get; set; }
        public string IDCopropietario { get; set; }
        public string NombreCopropietario { get; set; }
        public decimal PctParticipacion { get; set; }
        public string CuentaContable { get; set; }
        public string SubTipo { get; set; }        
    }
}
