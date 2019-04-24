using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    class Aviso
    {
        public int Referencia { get; set; }
        public int Prioridad { get; set; }
        public Alerta Alerta { get; set; }
        public Persona Persona { get; set; }
        public List<Operacion> Operaciones { get; set; }
        public List<OperacionVenta> OperacionesVenta { get; set; }

        public Aviso()
        {
            Prioridad = 1;
        }
    }
}
