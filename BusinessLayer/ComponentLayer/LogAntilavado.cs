using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public static class LogAntilavado
    {
        public static void generaLog(string errores, string warnings, string rfc)
        {
            try
            {
                string path = Properties.Settings.Default.RutaRepositorio ;
                
                if(path.EndsWith(@"\"))
                    path= path + rfc + @"\SPPLD\Log\";
                else
                    path = path +@"\"+ rfc + @"\SPPLD\Log\";
                
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string texto = string.Empty;
                if (!string.IsNullOrEmpty(warnings))
                {
                    texto += "Se encontraron las siguientes advertencias: " + Environment.NewLine + warnings + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(errores))
                {
                    texto += "Se encontraron los siguientes errores: " + Environment.NewLine + errores + Environment.NewLine;
                }
                string fileName = path + DateTime.Now.ToString("yyyyMMdd - HHmmss") + ".txt";
                //System.Windows.Forms.MessageBox.Show("Ruta Archivo: " + fileName );
                TextWriter tw = new StreamWriter(fileName, true);
                tw.WriteLine(texto);
                tw.Close();
            }
            catch
            {
            }
        }
    }
}
