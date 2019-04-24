using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using System.IO;
using FastReport;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class ListadoContratosRenta : SaariReport, IReport, IBackgroundReport
    {
        private string idInmobiliaria = string.Empty, idConjunto = string.Empty, usuario = string.Empty, rutaFormato = string.Empty, namecolAux = string.Empty;
        private DateTime  fechaFin;
        private bool esPdf = true, todosContratos = true, IncluirCargos = false, VigentesyHistoria = false;

        MetodosGenerales metodo = new MetodosGenerales();
        

        public ListadoContratosRenta(string idInmobiliaria, string idConjunto, DateTime fechaFin, string usuario, string rutaFormato, bool esPdf, bool todosContratos)
        {
            this.idInmobiliaria = idInmobiliaria;
            this.idConjunto = idConjunto;
            this.fechaFin = fechaFin;
            this.usuario = usuario;
            this.rutaFormato = rutaFormato;
            this.esPdf = esPdf;
            this.todosContratos = todosContratos;
        }

        public ListadoContratosRenta(string idInmobiliaria, string idConjunto, DateTime fechaFin, string usuario, string rutaFormato, bool esPdf, bool todosContratos, bool incluirCargos, bool VigentesyHistoria)
        {
            this.idInmobiliaria = idInmobiliaria;
            this.idConjunto = idConjunto;
            this.fechaFin = fechaFin;
            this.usuario = usuario;
            this.rutaFormato = rutaFormato;
            this.esPdf = esPdf;
            this.todosContratos = todosContratos;
            this.IncluirCargos = incluirCargos;
            this.VigentesyHistoria = VigentesyHistoria;
        }

        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();
        }

        public static List<ConjuntoEntity> obtenerConjuntos(string idInmobiliaria)
        {
            return SaariDB.getConjuntos(idInmobiliaria);
        }

        public string generar()
        {
            try
            {
                string resultValidar = validar(idInmobiliaria,  idConjunto, rutaFormato);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario"; 
                OnCambioProgreso(10);

                if (string.IsNullOrEmpty(resultValidar))
                {
                    if (File.Exists(rutaFormato))
                    {
                        List<ContratoEntity> listaContratos = new List<ContratoEntity>();
                        if (VigentesyHistoria)
                        {
                            if(idConjunto == "Todos")
                            {
                                listaContratos = SaariDB.getListaHistorialContratos(idInmobiliaria, todosContratos,fechaFin);
                            }
                            else
                            {
                                listaContratos = SaariDB.getListaHistorialContratos(idInmobiliaria, idConjunto, todosContratos,fechaFin);
                            }
                        }
                        else
                        {
                            if (idConjunto == "Todos")
                                listaContratos = SaariDB.getListaContratosRenta(idInmobiliaria, todosContratos);
                            else
                                listaContratos = SaariDB.getListaContratosRenta(idInmobiliaria, idConjunto, todosContratos);
                        }
                        
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        OnCambioProgreso(20);

                        if (listaContratos != null)
                        {
                            if (listaContratos.Count > 0)
                            {
                                string nombreInmobiliaria = SaariDB.getNombreInmobiliariaByID(idInmobiliaria);
                                string nombreConjunto = SaariDB.getNombreConjuntoByID(idConjunto);

                                EncabezadoEntity encabezado = new EncabezadoEntity()
                                {
                                    Inmobiliaria = nombreInmobiliaria,
                                    Conjunto = nombreConjunto,
                                    Usuario = usuario
                                };
                                List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                                listaEncabezado.Add(encabezado);

                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";
                                OnCambioProgreso(50);

                                if (!IncluirCargos)
                                {
                                    Report report = new Report();
                                    report.Load(rutaFormato);
                                    report.RegisterData(listaEncabezado, "Encabezado");
                                    report.RegisterData(listaContratos, "Contrato");

                                    DataBand bandaContratos = report.FindObject("Data1") as DataBand;
                                    bandaContratos.DataSource = report.GetDataSource("Contrato");

                                    
                                    OnCambioProgreso(80);
                                    if (CancelacionPendiente)
                                        return "Proceso cancelado por el usuario";
                                    return exportar(report, esPdf, "ListadoContratosRenta");
                                }
                                else
                                {
                                    OnCambioProgreso(80);
                                    if (CancelacionPendiente)
                                        return "Proceso cancelado por el usuario";
                                    return  generarIncluyendoCargos(listaContratos, listaEncabezado);

                                    // return AbrirReporte("ListadoContratosRenta");
                                    //objEnc.FechaFin = fechaFin.ToString("dd/MM/yyyy");
                                   
                                }

                            }
                            else
                                return "No se encontraron contratos con las condiciones dadas";
                        }
                        else
                            return "Error al obtener la relación de contratos";
                    }
                    else
                        return "No se encontró el formato " + rutaFormato + Environment.NewLine;
                }
                else
                    return resultValidar;
            }
            catch (Exception ex)
            {
                return "Error inesperado al generar el reporte:" + Environment.NewLine + ex.Message;
            }
        }

        private string generarIncluyendoCargos(List<ContratoEntity> listaContratos, List<EncabezadoEntity> listaEncabezado)
        {
            #region ENCABEZADO
            int renglon = 1, columna = 1;
            string error = string.Empty;
            Excel.Application aplicacionExcel = new Excel.Application();
            aplicacionExcel.Workbooks.Add(Type.Missing);
            Excel.Workbook libroExcel = aplicacionExcel.Workbooks.Add();
            Excel.Worksheet hojaExcel = (libroExcel.ActiveSheet as Excel.Worksheet);
            Excel.Range rango;// hojaExcel.get_Range("A1:" + GetExcelColumnName(Meses.Count + 3) + "1");

            //aplicacionExcel.Visible = true;
            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
            rango.Interior.Color = Color.FromArgb(54, 96, 146);
            rango.Font.Color = Color.FromArgb(255, 255, 255);
            rango.Font.FontStyle = "Bold";
            rango.Font.Size = 14;
            rango.RowHeight = 29;
            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            rango.Value2 = listaEncabezado[0].Inmobiliaria;
            renglon++;

            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
            rango.Interior.Color = Color.FromArgb(54, 96, 146);
            rango.Font.Color = Color.FromArgb(255, 255, 255);
            rango.Font.FontStyle = "Bold";
            rango.Font.Size = 14;
            rango.RowHeight = 20;
            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            rango.Value2 = "LISTADO DE CONTRATOS";// nameConjunto;
            renglon++;

            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
            rango.Interior.Color = Color.FromArgb(54, 96, 146);
            //rango.Interior.Color = Color.FromArgb(31, 98, 145);
            rango.Font.Color = Color.FromArgb(255, 255, 255);
            rango.Font.Size = 12;
            rango.RowHeight = 20;
            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            try
            {
                rango.Value2 = listaEncabezado[0].Conjunto;
            }
            catch { rango.Value2 = ""; }
            renglon++;

            columna = 1;
            #endregion

            List<string> Encabezado = GetEncabezadoEntity();
            foreach (string encabezado in Encabezado)
            {
                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.Interior.Color = Color.FromArgb(149, 179, 255);
                rango.Font.Color = Color.FromArgb(0, 0, 0);
                rango.Font.Size = 10;
                rango.Font.FontStyle = "Bold";
                rango.RowHeight = 29;
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                rango.Value2 = encabezado.Replace("_", " ");
                rango.WrapText = true;
                columna++;
            }

            List<SubtipoEntity> ListTipoCont = SaariDB.getSubTiposOI();
            if (ListTipoCont.Count <= 0)
                ListTipoCont = SaariDB.getSubTiposIA();


            foreach (SubtipoEntity subtipo in ListTipoCont)
            {
                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.Interior.Color = Color.FromArgb(149, 179,255);
                rango.Font.Color = Color.FromArgb(0, 0, 0);
                rango.Font.Size = 10;
                rango.Font.FontStyle = "Bold";
                rango.RowHeight = 29;
                rango.ColumnWidth = 17;
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                rango.Value2 = subtipo.Nombre;
                rango.WrapText = true;
                columna++;
            }
            renglon++;

            foreach (ContratoEntity Contrato in listaContratos)
            {
                columna = 1;
                #region DATOS DEL CONTRATO
                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rango.Value2 = Contrato.ClienteNombre;
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rango.Value2 = Contrato.NombreInmueble;
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rango.Value2 = Contrato.IdentificadorInmueble;
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rango.Value2 = Contrato.NombreConjunto;
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rango.Value2 = Contrato.Clasificacion;
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                string FechaInicio = Contrato.FechaInicio.Day.ToString("00") + "/" + Contrato.FechaInicio.Month.ToString("00") + "/" + Contrato.FechaInicio.Year.ToString("0000");
                //rango.Value2 = Contrato.FechaInicio.Day + "/" + Contrato.FechaInicio.Month + "/" + Contrato.FechaInicio.Year;
                rango.Value2 = FechaInicio;
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                string FechaVencimiento = Contrato.FechaVencimiento.Day.ToString("00") + "/" + Contrato.FechaVencimiento.Month.ToString("00") + "/" + Contrato.FechaVencimiento.Year.ToString("0000");
                //rango.Value2 = Contrato.FechaVencimiento.Day + "/" + Contrato.FechaVencimiento.Month + "/" + Contrato.FechaVencimiento.Year;
                rango.Value2 = FechaVencimiento;
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rango.Value2 = Contrato.TiempoPeriodo + "-" + Contrato.TipoPeriodo;

                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                string FechaVencimientoProrroga = Contrato.FechaVencimientoProrroga.Day.ToString("00") + "/" + Contrato.FechaVencimientoProrroga.Month.ToString("00") + "/" + Contrato.FechaVencimientoProrroga.Year.ToString("0000");
                //rango.Value2 = Contrato.FechaVencimientoProrroga.Day + "/" + Contrato.FechaVencimientoProrroga.Month + "/" + Contrato.FechaVencimientoProrroga.Year;
                rango.Value2 = FechaVencimientoProrroga;
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rango.Value2 = Contrato.PorcentajeIncremento;
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                string FechaIncremento = Contrato.FechaIncremento.Day.ToString("00") + "/" + Contrato.FechaIncremento.Month.ToString("00") + "/" + Contrato.FechaIncremento.Year.ToString("0000");
                //rango.Value2 = Contrato.FechaIncremento.Day + "/" + Contrato.FechaIncremento.Month + "/" + Contrato.FechaIncremento.Year;
                rango.Value2 = FechaIncremento;
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rango.Value2 = Contrato.Moneda;
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rango.Value2 = Contrato.ImporteOriginal;
                rango.NumberFormat = "$ #,###.00";
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rango.Value2 = Contrato.ImporteActual;
                rango.NumberFormat = "$ #,###.00";
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rango.Value2 = Contrato.Deposito;
                rango.NumberFormat = "$ #,###.00";
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rango.Value2 = Contrato.Actividad;
                columna++;

                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rango.Value2 = Contrato.EstatusProrroga;
                columna++;
                #endregion

                foreach (SubtipoEntity subtipo in ListTipoCont)
                {
                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    try
                    {
                        rango.Value2 = Contrato.ListaCargos.Find(f => f.Identificador == subtipo.Identificador).importeCargo;
                        rango.NumberFormat = "$ #,###.00";
                    }
                    catch
                    {
                        rango.Value2 = "";
                    }
                    columna++;
                }

                #region FORMATO RENGLON
                string colAct = metodo.GetExcelColumnName(columna-1);
                string rStyle = "A" + renglon;
                string rStyle2 = colAct + renglon;
                Excel.Range newRng = aplicacionExcel.get_Range(rStyle, rStyle2);
                newRng.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                newRng.Borders.Color = Color.FromArgb(211, 211, 211);
                newRng.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                #endregion
                namecolAux = colAct;
                renglon++;
            }

            for (int i = 1; i <= 3; i++)
            {
                rango = hojaExcel.get_Range("A" + i + ":" + namecolAux + i);
                try
                {
                    rango.Select();
                }
                catch { }
                rango.MergeCells = true;
            }
            hojaExcel.Application.ActiveWindow.SplitRow = 4;
            hojaExcel.Application.ActiveWindow.FreezePanes = true;

            try
            {
                string nombreReporte = "ListadoContratosRenta";
                string rutaGuardar = Properties.Settings.Default.RutaRepositorio;
                rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + nombreReporte + @"\"  : rutaGuardar + @"\" + nombreReporte + @"\";
                if (!Directory.Exists(rutaGuardar))
                    Directory.CreateDirectory(rutaGuardar);
                string filename = string.Empty;
                filename = rutaGuardar + nombreReporte + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                if (esPdf)
                {
                    int colPdf = Encabezado.Count + ListTipoCont.Count; //ListTipoContAux.Count + 24;//El 24 corresponde al numero de colunas que no corresponden a cargos.
                    hojaExcel.PageSetup.PaperSize = XlPaperSize.xlPaperLegal;
                    if (colPdf <= 35)
                        hojaExcel.PageSetup.Zoom = 32;
                    else if (colPdf <= 50)
                        hojaExcel.PageSetup.Zoom = 20;
                    else
                        hojaExcel.PageSetup.Zoom = 15;
                                  
                    try
                    {
                        hojaExcel.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
                        filename = filename.Replace(".xlsx", ".pdf");

                        libroExcel.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, filename, XlFixedFormatQuality.xlQualityStandard, true, false, Type.Missing, Type.Missing, false, Type.Missing);
                        libroExcel.CheckCompatibility = false;
                        libroExcel.Close(false);
                        aplicacionExcel.Quit();
                        metodo.releaseObject(hojaExcel);
                        metodo.releaseObject(libroExcel);
                        metodo.releaseObject(aplicacionExcel);
                    }
                    catch (Exception ex)
                    {
                        libroExcel.CheckCompatibility = false;
                        libroExcel.Close();
                        aplicacionExcel.Quit();
                        metodo.releaseObject(hojaExcel);
                        metodo.releaseObject(libroExcel);
                        metodo.releaseObject(aplicacionExcel);
                        return "No se pudo exportar a PDF" + Environment.NewLine + ex.Message;
                    }
                }
                else
                {
                   
                    libroExcel.SaveAs(filename, Excel.XlFileFormat.xlWorkbookDefault);
                    libroExcel.CheckCompatibility = false;
                    libroExcel.Close(true);
                    aplicacionExcel.Quit();
                    metodo.releaseObject(hojaExcel);
                    metodo.releaseObject(libroExcel);
                    metodo.releaseObject(aplicacionExcel);
                    
                }

                try
                {
                    if (System.IO.File.Exists(filename))
                        System.Diagnostics.Process.Start(filename);
                }
                catch
                {
                    return "No se pudo abrir el archivo" + filename;
                }
            }
            catch (Exception ex)
            {
                return "Ocurrio el siguiente error en la generacion del reporte : " + ex;
            }


            return error;
        }

        public List<string> GetEncabezadoEntity()
        {
            return Enum.GetNames(typeof(EncabezadoListadoDeContratos1)).ToList();
        }

        public string validar(string idInmobiliaria, string idConjunto, string rutaFormato)
        {
            string error = string.Empty;
            if (string.IsNullOrEmpty(idInmobiliaria))
                error += "Debe seleccionar una inmobiliaria" + Environment.NewLine;
            if (string.IsNullOrEmpty(idConjunto))
                error += "Debe seleccionar un conjunto" + Environment.NewLine;
            //if (fechaFin != null)
            //    error += "Debe seleccionar una fecha" + Environment.NewLine;
            if (!File.Exists(rutaFormato))
                error += "No se encontró el formato " + rutaFormato + Environment.NewLine;
            return error;
        }
    }
}
