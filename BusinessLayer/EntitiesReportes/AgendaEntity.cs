using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class AgendaEntity
    {
        public DateTime Fecha { get; set; }
        public string Nombre { get; set; }
        public string Motivo { get; set; }
        public byte[] Observacion { get; set; }
        public string Usuario { get; set; }
        public string Estatus { get; set; }
    }
}
