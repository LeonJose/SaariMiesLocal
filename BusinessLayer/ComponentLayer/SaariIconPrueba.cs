using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public static class SaariIcon
    {
        public static bool setSaariIcon(string icon, Form formulario)
        {
            if (System.IO.File.Exists(icon))
            {
                try
                {
                    System.Drawing.Icon saariIcon = new System.Drawing.Icon(icon);
                    formulario.Icon = saariIcon;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problema con el icono de Saari, comuniquese con su administrador de Saari. " + ex.Message, "Saari", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }
    }
}
