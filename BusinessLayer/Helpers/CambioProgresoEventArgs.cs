using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.Helpers
{
    public class CambioProgresoEventArgs : EventArgs
    {
        private int progreso = 0;

        public int Progreso { get { return progreso; } }

        public CambioProgresoEventArgs(int progreso)
        {
            this.progreso = progreso;
        }
    }
}
