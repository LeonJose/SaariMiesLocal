using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntilavado2
{
    public class Alerta
    {
        public int Tipo { get; set; }
        public string Descripcion { get; set; }

        public Alerta()
        {
            Tipo = 100;
        }
    }
}
