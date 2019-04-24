using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using FastReport;
using FastReport.Export.Pdf;
using FastReport.Export.OoXML;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.BusinessLayer.Abstracts
{
    public abstract class SaariReport
    {
        private string nombreArchivo = string.Empty;
        private bool cancelacionPendiente = false;

       // public  bool abrirExcel = false;
        

        public virtual string NombreArchivo { get { return nombreArchivo; } }
        public bool CancelacionPendiente { get { return cancelacionPendiente; } }
        public event EventHandler<CambioProgresoEventArgs> CambioProgreso;

        protected virtual string exportar(Report reporte, bool rbPdf, string nombreReporte)
        {
            try
            {
                string rutaGuardar = Properties.Settings.Default.RutaRepositorio;
                rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + nombreReporte + @"\" : rutaGuardar + @"\"+ nombreReporte + @"\";
                if (!Directory.Exists(rutaGuardar))
                    Directory.CreateDirectory(rutaGuardar);
                reporte.Prepare();
                string filename = string.Empty;
                if (rbPdf)
                {
                    filename = rutaGuardar + nombreReporte + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";                    
                    PDFExport export = new PDFExport();
                    reporte.Export(export, filename);
                }
                else
                {
                                       
                    filename = rutaGuardar + nombreReporte + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                    Excel2007Export export = new Excel2007Export();
                    reporte.Export(export, filename);
                }
                nombreArchivo = filename;
                reporte.Dispose();

                if(File.Exists(filename))
                    Process.Start(filename);

                OnCambioProgreso(100);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error al exportar el reporte: " + Environment.NewLine + ex.Message;
            }
        }
        protected virtual string exportar(Report reporte, bool esPdf, string nombreReporte, bool abrirAutomaticamente)
        {
            try
            {
                string rutaGuardar = Properties.Settings.Default.RutaRepositorio;
                rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + nombreReporte + @"\" : rutaGuardar + @"\" + nombreReporte + @"\";
                if (!Directory.Exists(rutaGuardar))
                    Directory.CreateDirectory(rutaGuardar);
                reporte.Prepare();
                string filename = string.Empty;
                if (esPdf)
                {
                    filename = rutaGuardar + nombreReporte + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                    PDFExport export = new PDFExport();
                    reporte.Export(export, filename);
                }
                else
                {

                    filename = rutaGuardar + nombreReporte + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                    Excel2007Export export = new Excel2007Export();
                    reporte.Export(export, filename);
                }
                nombreArchivo = filename;
                reporte.Dispose();

                if (File.Exists(filename) && abrirAutomaticamente)
                    Process.Start(filename);

                OnCambioProgreso(100);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error al exportar el reporte: " + Environment.NewLine + ex.Message;
            }
        }
        protected virtual string exportarValidaAbrir(Report reporte, bool esPdf, string nombreReporte, bool AbrirExcel)
        {
            try
            {
                string rutaGuardar = Properties.Settings.Default.RutaRepositorio;
                rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + nombreReporte + @"\" : rutaGuardar + @"\" + nombreReporte + @"\";
                if (!Directory.Exists(rutaGuardar))
                    Directory.CreateDirectory(rutaGuardar);
                reporte.Prepare();
                string filename = string.Empty;
                if (esPdf)
                {
                    filename = rutaGuardar + nombreReporte + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                    PDFExport export = new PDFExport();
                    reporte.Export(export, filename);
                }
                else
                {

                    filename = rutaGuardar + nombreReporte + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                    Excel2007Export export = new Excel2007Export();
                    reporte.Export(export, filename);
                }
                nombreArchivo = filename;
                reporte.Dispose();

                if (File.Exists(filename))
                    if (!esPdf)
                    {
                        if (AbrirExcel == true)
                        {
                            Process.Start(filename);
                        }
                    }
                Process.Start(filename);
                OnCambioProgreso(100);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error al exportar el reporte: " + Environment.NewLine + ex.Message;
            }
        }

        protected virtual string exportar(TfrxReportClass reporte, bool esPdf, string nombreReporte)
        {
            try
            {
                string rutaGuardar = Properties.Settings.Default.RutaRepositorio;
                rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + nombreReporte + @"\" : rutaGuardar + @"\" + nombreReporte + @"\";
                if (!Directory.Exists(rutaGuardar))
                    Directory.CreateDirectory(rutaGuardar);
                reporte.PrepareReport(true);
                string filename = string.Empty;
                if (esPdf)
                {
                    filename = rutaGuardar + nombreReporte + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";                    
                    reporte.ExportToPDF(filename, false, false, true, false, string.Empty, string.Empty);
                }
                else
                {
                    filename = rutaGuardar + nombreReporte + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls";
                    reporte.ExportToXLS(filename, true, true, true, true, true, true);                    
                }
                nombreArchivo = filename;                
                if (File.Exists(filename))
                    Process.Start(filename);
                reporte.ClearReport();
                OnCambioProgreso(100);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error al exportar el reporte: " + Environment.NewLine + ex.Message;
            }
        }

        protected virtual void OnCambioProgreso(int porcentaje)
        {
            try
            {
                var handler = CambioProgreso;
                if (handler != null)
                    handler(this, new CambioProgresoEventArgs(porcentaje));
            }
            catch(Exception ex)
            {
                string error = ex.Message;
            }
        }

        public virtual void cancelar()
        {
            cancelacionPendiente = true;
        }
    }
}
