using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class PictureBoxRT:System.Windows.Forms.PictureBox
    {
        public int ID { get; set; }
        public string Ruta { get; set; }
        public string NombreImagen { get; set; }
    }
}
