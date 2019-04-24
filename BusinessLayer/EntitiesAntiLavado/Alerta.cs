using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    class Alerta
    {
        public int Tipo { get; set; }
        public string Descripcion { get; set; }

        public Alerta()
        {
            Tipo = 100;
        }
    }
}
