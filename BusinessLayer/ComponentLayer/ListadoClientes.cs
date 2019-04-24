using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using FastReport;
using FastReport.Export.Pdf;
using FastReport.Export.OoXML;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class ListadoClientes
    {
        public string generarListado(string rutaFormato, bool esPdf)
        {
            try
            {
                List<ClienteEntity> listaClientes = SaariDB.getClientesCompleto();
                if (listaClientes != null)
                {
                    if (listaClientes.Count > 0)
                    {
                        string rutaGuardar = Properties.Settings.Default.RutaRepositorio;
                        rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar : rutaGuardar + @"\";
                        if (!System.IO.Directory.Exists(rutaGuardar))
                            System.IO.Directory.CreateDirectory(rutaGuardar);

                        Report report = new Report();
                        report.Load(rutaFormato);
                        report.RegisterData(listaClientes, "Cliente");
                        DataBand bandaClientes = report.FindObject("Data1") as DataBand;
                        bandaClientes.DataSource = report.GetDataSource("Cliente");                        
                        report.Prepare();
                        string filename = string.Empty;
                        if (esPdf)
                        {
                            filename = rutaGuardar + @"ListadoClientes_" + DateTime.Now.Ticks + ".pdf";
                            PDFExport export = new PDFExport();
                            report.Export(export, filename);
                        }
                        else
                        {
                            filename = rutaGuardar + @"ListadoClientes_" + DateTime.Now.Ticks + ".xlsx";
                            Excel2007Export export = new Excel2007Export(); 
                            report.Export(export, filename);
                        }
                        report.Dispose();
                        try
                        {
                            System.Diagnostics.Process.Start(filename);
                        }
                        catch { }
                        return string.Empty;
                    }
                    else
                        return "No se encontraron registros de clientes";
                }
                else
                    return "Error al obtener los registros de los clientes";
            }
            catch (Exception ex)
            {
                return "Error al generar el listado: " + ex.Message;
            }
        }
    }
}
