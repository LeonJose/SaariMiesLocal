using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.DataAccessLayer;
using FastReport;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Globalization;
using System.ComponentModel;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class ConsecutivosCFDIsFact : SaariReport
    {
        private string idInmobiliaria = string.Empty, idConjunto = string.Empty, usuario = string.Empty;
        private DateTime fechaInicio = new DateTime(), fechaFin = DateTime.Now.Date;
        private bool esPdf = true;
        private bool Abrir = true;
        private string InmobiliariaNombre = string.Empty;
        private string nameConjuto = string.Empty;

        public ConsecutivosCFDIsFact(string idInmobiliaria, string idConjunto, DateTime fechaInicio, DateTime fechaFin, string usuario, bool esPdf, bool abrir, string nombreInmo, string nombreConj)
        {
            this.idInmobiliaria = idInmobiliaria;
            this.idConjunto = idConjunto;
            this.fechaInicio = fechaInicio.Date;
            this.fechaFin = fechaFin.Date;
            this.usuario = usuario;
            this.esPdf = esPdf;
            this.Abrir = abrir;
            this.InmobiliariaNombre = nombreInmo;
            this.nameConjuto = nombreConj;
        }

        public string validar(string idInmobiliaria, string idConjunto, DateTime fechaInicio, DateTime fechaFin)
        {
            string error = string.Empty;
            if (string.IsNullOrEmpty(idInmobiliaria))
                error += "Debe seleccionar una inmobiliaria" + Environment.NewLine;
            if (string.IsNullOrEmpty(idConjunto))
                error += "Debe seleccionar un conjunto" + Environment.NewLine;
            if (fechaInicio > fechaFin)
                error += "La fecha de fin debe ser mayor o igual a la fecha de inicio" + Environment.NewLine;

            return error;
        }

        public string generarReporte()
        {
            try
            {
                string validaciones = validar(idInmobiliaria, idConjunto, fechaInicio, fechaFin);

                int ColumnaEmpezarSumatoria = 0;
                int subtipos = 0;
                int columna = 1, renglon = 1;
                int centrar = 0;
                Color colorcelda;
                bool hayDolares = false;
                bool hayPesos = false;
                decimal importeCargoDesconocido = 0;
                if (string.IsNullOrEmpty(validaciones))
                {
                    OnCambioProgreso(3);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    List<SubtipoEntity> listaSubtipos = SaariDB.getSubTiposOI();
                    if (listaSubtipos != null)
                    {
                        subtipos = listaSubtipos.Count;
                        OnCambioProgreso(5);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";

                        List<CFDIConsecutivoEntity> listaCfdisBD = SaariDB.getRegistrosCFDISConsec(idInmobiliaria, idConjunto, fechaInicio.Date, fechaFin.Date);
                        if (listaCfdisBD != null)
                        {
                            if (listaCfdisBD.Count > 0)
                            {
                                #region CONFIGURACION REPORTE
                                string path = Properties.Settings.Default.RutaRepositorio.EndsWith(@"\") ? Properties.Settings.Default.RutaRepositorio + @"CFDIsConsecutivos\" : Properties.Settings.Default.RutaRepositorio + @"\CFDIsConsecutivos\";
                                if (!Directory.Exists(path))
                                    Directory.CreateDirectory(path);

                                string filename = path + "CFDIsConsecutivos_" + InmobiliariaNombre + "_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + DateTime.Now.Second + ".xlsx";
                                Excel.Application aplicacionExcel = new Excel.Application();
                                Excel.Workbook libroExcel = aplicacionExcel.Workbooks.Add();
                                Excel.Worksheet hojaExcel = (libroExcel.ActiveSheet as Excel.Worksheet);
                                Excel.Range rango;// hojaExcel.get_Range("A1:" + GetExcelColumnName(Meses.Count + 3) + "1");
                                                  //aplicacionExcel.Visible = true;
                                int colums = listaSubtipos.Count + 16;

                                if (colums <= 6)
                                    hojaExcel.PageSetup.Zoom = 70;
                                else if (colums <= 12)
                                    hojaExcel.PageSetup.Zoom = 60;
                                else if (colums <= 18)
                                    hojaExcel.PageSetup.Zoom = 55;
                                else if (colums <= 24)
                                    hojaExcel.PageSetup.Zoom = 42;
                                else if (colums <= 36)
                                    hojaExcel.PageSetup.Zoom = 35;
                                else
                                    hojaExcel.PageSetup.Zoom = 15;

                                //if (listaSubtipos.Count + 16 <= 35)
                                //    hojaExcel.PageSetup.Zoom = 32;
                                //else if (listaSubtipos.Count + 16 <= 50)
                                //    hojaExcel.PageSetup.Zoom = 20;
                                //else 
                                //    hojaExcel.PageSetup.Zoom = 20;

                                #endregion CONFIGURACION REPORTE                               

                                #region Encabezados

                                OnCambioProgreso(15);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";
                                List<string> Encabezado = GetEncabezadoEntity(1);
                                List<string> Encabezado2 = GetEncabezadoEntity(2);

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Interior.Color = Color.FromArgb(54, 96, 146);
                                rango.Font.Color = Color.FromArgb(255, 255, 255);
                                rango.Font.FontStyle = "Bold";
                                rango.Font.Size = 14;
                                rango.RowHeight = 25;
                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                rango.Value2 = "CFDIs Consecutivos de " + InmobiliariaNombre;
                                renglon++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Interior.Color = Color.FromArgb(54, 96, 146);
                                rango.Font.Color = Color.FromArgb(255, 255, 255);
                                rango.Font.FontStyle = "Bold";
                                rango.Font.Size = 12;
                                rango.RowHeight = 18;
                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                rango.Value2 = nameConjuto;
                                renglon++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Interior.Color = Color.FromArgb(54, 96, 146);
                                rango.Font.Color = Color.FromArgb(255, 255, 255);
                                rango.Font.Size = 10;
                                rango.RowHeight = 12;
                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                rango.Value2 = "DEL " + fechaInicio.Day + "/" + MonthName(fechaInicio.Month).ToUpper() + "/" + fechaInicio.Year + " Al " + fechaFin.Day + "/" + MonthName(fechaFin.Month).ToUpper() + "/" + fechaFin.Year;
                                renglon++;
                                columna = 1;

                                centrar = listaSubtipos.Count + Encabezado.Count + Encabezado2.Count;
                                rango = hojaExcel.get_Range("A1:" + GetExcelColumnName(centrar) + "1");
                                rango.Select();
                                rango.MergeCells = true;
                                rango = hojaExcel.get_Range("A2:" + GetExcelColumnName(centrar) + "2");
                                rango.Select();
                                rango.MergeCells = true;
                                rango = hojaExcel.get_Range("A3:" + GetExcelColumnName(centrar) + "3");
                                rango.Select();
                                rango.MergeCells = true;



                                foreach (string encabezado in Encabezado)
                                {
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Interior.Color = Color.FromArgb(149, 179, 215);
                                    rango.Font.Color = Color.FromArgb(0, 0, 0);
                                    rango.Font.Size = 11;
                                    rango.Font.FontStyle = "Bold";
                                    rango.RowHeight = 66;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = encabezado.Replace("_", " ");
                                    rango.WrapText = true;
                                    if (encabezado == "TC")
                                        ColumnaEmpezarSumatoria = columna;
                                    columna++;

                                }
                                if (listaSubtipos.Count > 0)
                                {
                                    SubtipoEntity sub = new SubtipoEntity();
                                    sub.Nombre = "No Identificados";
                                    sub.Identificador = null;
                                    //sub.Nombre = "Rentas Variables";
                                    listaSubtipos.Add(sub);
                                    foreach (SubtipoEntity cfdis in listaSubtipos)
                                    {
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.Interior.Color = Color.FromArgb(149, 179, 215);
                                        rango.Font.Color = Color.FromArgb(0, 0, 0);
                                        rango.Font.Size = 11;
                                        rango.Font.FontStyle = "Bold";
                                        rango.RowHeight = 66;
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                        rango.Value2 = cfdis.Nombre.Replace("_", " ");
                                        rango.WrapText = true;
                                        columna++;
                                    }
                                }

                                foreach (string encabezado in Encabezado2)
                                {
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Interior.Color = Color.FromArgb(149, 179, 215);
                                    rango.Font.Color = Color.FromArgb(0, 0, 0);
                                    rango.Font.Size = 11;
                                    rango.Font.FontStyle = "Bold";
                                    rango.RowHeight = 66;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = encabezado.Replace("_", " ");
                                    rango.WrapText = true;
                                    columna++;
                                }

                                if (listaSubtipos.Count > 0)
                                {
                                    rango = (hojaExcel.Cells[1, columna - 1] as Excel.Range);
                                    rango.Interior.Color = Color.FromArgb(54, 96, 146);
                                    rango = (hojaExcel.Cells[2, columna - 1] as Excel.Range);
                                    rango.Interior.Color = Color.FromArgb(54, 96, 146);
                                    rango = (hojaExcel.Cells[3, columna - 1] as Excel.Range);
                                    rango.Interior.Color = Color.FromArgb(54, 96, 146);
                                }
                                hojaExcel.Application.ActiveWindow.SplitRow = 4;
                                hojaExcel.Application.ActiveWindow.SplitColumn = ColumnaEmpezarSumatoria;
                                hojaExcel.Application.ActiveWindow.FreezePanes = true;

                                #endregion Encabezados                               

                                #region LLENADO DE REPORTE
                                renglon++;
                                int porcentaje = 30;
                                decimal factor = 50 / listaCfdisBD.Count;
                                factor = factor >= 1 ? factor : 1;

                                foreach (CFDIConsecutivoEntity cfdi in listaCfdisBD)
                                {
                                    
                                    if (CancelacionPendiente)
                                    {
                                        libroExcel.Close(false);
                                        aplicacionExcel.Quit();
                                        return "Proceso cancelado por el usuario";
                                    }

                                    if (cfdi.Moneda == "P")
                                        hayPesos = true;
                                    else
                                        hayDolares = true;

                                    columna = 1;
                                    if ((renglon % 2) == 0)
                                    {
                                        if (!cfdi.EsCancelada)
                                            colorcelda = Color.FromArgb(221, 235, 247);
                                        else
                                        {
                                            colorcelda = Color.FromArgb(242, 220, 219);
                                            cfdi.NombreCliente = "CANCELADO " + cfdi.NombreCliente;
                                            cfdi.NombreComercialCliente = "CANCELADO " + cfdi.NombreComercialCliente;
                                        }
                                    }
                                    else
                                    {
                                        if (!cfdi.EsCancelada)
                                            colorcelda = Color.FromArgb(255, 255, 255);
                                        else
                                        {
                                            colorcelda = Color.FromArgb(242, 220, 219);
                                            cfdi.NombreCliente = "CANCELADO " + cfdi.NombreCliente;
                                            cfdi.NombreComercialCliente = "CANCELADO " + cfdi.NombreComercialCliente;
                                        }
                                    }

                                    decimal retenciones = cfdi.RetIVA + cfdi.ISR;
                                    decimal total = cfdi.ImporteFac + cfdi.IVAFac;
                                    TotalesRecibosExpedidosEntity totales = new TotalesRecibosExpedidosEntity();
                                    //if (cfdi.Moneda != "P")
                                    //{
                                    //    totales.ImporteDolaresAPesos += cfdi.ImporteFac * cfdi.TipoDeCambio;
                                    //    totales.IVADolaresAPesos += cfdi.IVAFac * cfdi.TipoDeCambio;
                                    //    totales.ISRDolaresAPesos += cfdi.ISR * cfdi.TipoDeCambio;
                                    //    totales.IVARetenidoDolaresAPesos += cfdi.RetIVA * cfdi.TipoDeCambio;                                      
                                    //    //retenciones += totales.IVARetenidoDolaresAPesos + totales.ISRDolaresAPesos;
                                    //    totales.TotalDolaresAPesos += totales.ImporteDolaresAPesos + totales.IVADolaresAPesos - retenciones;
                                    //    //cfdi.DescuentoRenta = cfdi.DescuentoRenta * cfdi.TipoDeCambio;
                                    //    //cfdi.TotalCargos = cfdi.TotalCargos * cfdi.TipoDeCambio;
                                    //    //cfdi.TotalCargos = cfdi.TotalCargos * cfdi.TipoDeCambio;
                                    //}

                                    List<ReciboEntity> ListaCargos = SaariDB.getImporteCargos(cfdi.IDHistRec);

                                    if (porcentaje <= 80)
                                        porcentaje += Convert.ToInt32(factor);
                                    OnCambioProgreso(porcentaje);
                                    if (CancelacionPendiente)
                                        return "Proceso cancelado por el usuario";

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = cfdi.FechaEmision;
                                    rango.NumberFormat = "dd/mm/yyyy";
                                    columna++;

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = cfdi.Serie;
                                    columna++;

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = cfdi.Folio;
                                    columna++;

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = cfdi.NombreCliente;
                                    columna++;

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = cfdi.NombreComercialCliente;
                                    columna++;

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = cfdi.NombreConjunto; //+ "    |    " + cfdi.NombreSubConjunto;
                                    columna++;

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = cfdi.NombreInmueble; //+ " | " + cfdi.IdSubConjEdificio;
                                    columna++;

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = cfdi.Periodo;
                                    columna++;

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = cfdi.Moneda;
                                    columna++;

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = cfdi.TipoDeCambio;
                                    columna++;


                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    if (!cfdi.EsRentaAnticipada)
                                    {
                                        if (string.IsNullOrEmpty(cfdi.IDSubtipo))
                                            rango.Value2 = cfdi.ImporteFac;
                                        else
                                            rango.Value2 = 0;
                                    }
                                    else
                                        rango.Value2 = 0;

                                    rango.NumberFormat = "#,###.00";
                                    columna++;


                                    if (listaSubtipos.Count > 0)
                                    {
                                        foreach (SubtipoEntity cfdiSub in listaSubtipos)
                                        {
                                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                            rango.Interior.Color = colorcelda;
                                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                            try
                                            {
                                                if (!cfdi.EsCancelada)
                                                {
                                                    if (ListaCargos.Count > 0)
                                                        rango.Value2 = ListaCargos.Find(c => c.Campo20 == cfdiSub.Identificador).Importe;
                                                    else
                                                    {
                                                        if (cfdi.IDSubtipo == cfdiSub.Identificador)
                                                            rango.Value2 = cfdi.ImporteFac;
                                                    }
                                                }
                                                else
                                                    rango.Value2 = 0;
                                            }
                                            catch
                                            {
                                                rango.Value2 = 0;
                                            }
                                            rango.WrapText = true;
                                            rango.NumberFormat = "#,###.00";
                                            columna++;
                                        }
                                    }

                                    //Subototal
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.Value2 = cfdi.ImporteFac;
                                    rango.NumberFormat = "#,###.00";
                                    columna++;
                                    //IVA
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.Value2 = cfdi.IVAFac;
                                    rango.NumberFormat = "#,###.00";
                                    columna++;
                                    //Retencion IVA
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.Value2 = cfdi.ISR;

                                    rango.NumberFormat = "#,###.00";
                                    columna++;

                                    //Retencion ISR
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.Value2 = cfdi.RetIVA;
                                    rango.NumberFormat = "#,###.00";
                                    columna++;

                                    //Retencion TOTAL
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.Interior.Color = colorcelda;
                                    rango.Value2 = total - retenciones;
                                    rango.NumberFormat = "#,###.00";
                                    columna++;
                                    renglon++;

                                }
                                #endregion LLENADO DE REPORTE

                                #region SUMATORIAS
                                int r1 = 5;
                                int r2 = renglon - 1;
                                int renglonAux = r2;
                                int r2Aux = r2;
                                int colSum = listaSubtipos.Count + Encabezado2.Count;
                                string comillas = "\"";
                                int columnaMoneda = ColumnaEmpezarSumatoria - 1;
                                string colMoneda = GetExcelColumnName(columnaMoneda);
                                if (hayPesos)
                                {
                                    columna = ColumnaEmpezarSumatoria;
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(226, 239, 218);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = "TOTAL MN :";
                                    columna++;

                                    for (int s = 0; s <= colSum; s++)
                                    {
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        string colAct = GetExcelColumnName(columna);
                                        rango.Font.Color = Color.Black;
                                        rango.Interior.Color = Color.FromArgb(226, 239, 218);
                                        rango.Font.FontStyle = "Bold";
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                        rango.Formula = string.Format("=SUMIF(" + colMoneda + r1 + ":" + colMoneda + r2 + "," + comillas + "P" + comillas + ",{0}{1}:{2}{3})", colAct, r1, colAct, r2);
                                        rango.NumberFormat = "$ #,###.00";
                                        columna++;

                                    }
                                    renglon++;
                                }

                                if (hayDolares)
                                {
                                    columna = ColumnaEmpezarSumatoria;
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(226, 239, 218);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = "TOTAL DLLS :";
                                    columna++;

                                    for (int s = 0; s <= colSum; s++)
                                    {
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        string colAct = GetExcelColumnName(columna);
                                        rango.Font.Color = Color.Black;
                                        rango.Interior.Color = Color.FromArgb(226, 239, 218);
                                        rango.Font.FontStyle = "Bold";
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                        //rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2);
                                        rango.Formula = string.Format("=SUMIF(" + colMoneda + r1 + ":" + colMoneda + r2 + "," + comillas + "D" + comillas + ",{0}{1}:{2}{3})", colAct, r1, colAct, r2);
                                        rango.NumberFormat = "$ #,###.00";
                                        columna++;
                                    }
                                    renglon++;


                                    columna = ColumnaEmpezarSumatoria;
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(226, 239, 218);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = "Totales DLLS convertidos a MN :";
                                    columna++;


                                    //r1 = 5;
                                    //r2 = renglon - 1;
                                    //int colSum = listaSubtipos.Count + Encabezado2.Count;
                                    for (int s = 0; s <= colSum; s++)
                                    {
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        string colAct = GetExcelColumnName(columna);
                                        string colTC = GetExcelColumnName(ColumnaEmpezarSumatoria);
                                        rango.Font.Color = Color.Black;
                                        rango.Interior.Color = Color.FromArgb(226, 239, 218);
                                        rango.Font.FontStyle = "Bold";
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                        rango.Formula = string.Format("=SUMPRODUCT((" + colMoneda + r1 + ":" + colMoneda + renglonAux + "=" + comillas + "D" + comillas + ")*(" + colTC + r1 + ":" + colTC + renglonAux + ")*({0}{1}:{2}{3}))", colAct, r1, colAct, r2);
                                        rango.NumberFormat = "$ #,###.00";
                                        columna++;
                                    }
                                    renglon++;
                                }

                                if (hayPesos && hayDolares)
                                {
                                    columna = ColumnaEmpezarSumatoria;
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(208, 223, 187);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = "TOTAL DLLS CONVERTIDOS + TOTAL MN: ";
                                    columna++;


                                    r1 = renglon - 1;
                                    r2 = renglon - 3;
                                    //int colSum = listaSubtipos.Count + Encabezado2.Count;
                                    for (int s = 0; s <= colSum; s++)
                                    {
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        string colAct = GetExcelColumnName(columna);
                                        rango.Font.Color = Color.Black;
                                        rango.Interior.Color = Color.FromArgb(226, 239, 218);
                                        rango.Font.FontStyle = "Bold";
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                        rango.Formula = string.Format("=SUM({0}{1}+{2}{3})", colAct, r1, colAct, r2);
                                        rango.NumberFormat = "$ #,###.00";
                                        columna++;

                                    }
                                }
                                #endregion SUMATORIAS                                   

                                #region GUARDAR ARCHIVO
                                OnCambioProgreso(85);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                if (!esPdf)
                                {

                                    libroExcel.SaveAs(filename, Excel.XlFileFormat.xlWorkbookDefault);
                                    libroExcel.CheckCompatibility = false;
                                    libroExcel.Close();
                                    aplicacionExcel.Quit();
                                    releaseObject(hojaExcel);
                                    releaseObject(libroExcel);
                                    releaseObject(aplicacionExcel);
                                }
                                else
                                {
                                    try
                                    {

                                        hojaExcel.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
                                        filename = filename.Replace(".xlsx", ".pdf");
                                        libroExcel.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, filename, XlFixedFormatQuality.xlQualityStandard, true, false, Type.Missing, Type.Missing, false, Type.Missing);
                                        libroExcel.CheckCompatibility = false;
                                        libroExcel.Close(false);
                                        aplicacionExcel.Quit();
                                    }
                                    catch (Exception ex)
                                    {
                                        libroExcel.CheckCompatibility = false;
                                        libroExcel.Close();
                                        aplicacionExcel.Quit();
                                        releaseObject(hojaExcel);
                                        releaseObject(libroExcel);
                                        releaseObject(aplicacionExcel);
                                        return "No se pudo exportar a PDF" + Environment.NewLine + ex.Message;
                                    }
                                }

                                OnCambioProgreso(95);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";
                                try
                                {
                                    if (System.IO.File.Exists(filename))
                                    {
                                        if (Abrir)
                                            System.Diagnostics.Process.Start(filename);
                                    }
                                    else
                                        return "Error al buscar el archivo para abrirlo";
                                }
                                catch
                                {
                                    return "No se pudo abrir el archivo" + filename;
                                }
                                #endregion GUARDAR ARCHIVO
                            }
                            else
                            {
                                return "No se encontraron registros de CFDIs Con los parametros ingresados.";
                            }

                        }
                        else
                        {
                            return "Error al tratar de obtener la lista de CFDI's";
                        }
                    }
                    else
                    {
                        return "Error al tratar de obtener la lista de Subtipos registrados";
                    }

                    return string.Empty;
                }
                else
                {
                    return validaciones;
                }
            }
            catch (Exception ex)
            {
                return "Error inesperado al tratar de generar el reporte : " + ex;
            }
        }

        public List<string> GetEncabezadoEntity(int n)
        {
            if (n == 1)
                return Enum.GetNames(typeof(EncabezadoCFDIsConsecutivos1)).ToList();
            else
                return Enum.GetNames(typeof(EncabezadoCFDIsConsecutivos2)).ToList();
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
            }
            finally
            {
                obj = null;
                GC.Collect();
            }
        }

        public string GetExcelColumnName(int ColumnNumber)
        {
            int intDividend = ColumnNumber;
            string strColumnName = String.Empty;
            int intModulo;
            while (intDividend > 0)
            {
                intModulo = (intDividend - 1) % 26;
                strColumnName = Convert.ToChar(65 + intModulo).ToString() + strColumnName;
                intDividend = (int)((intDividend - intModulo) / 26);
            }
            return strColumnName;
        }

        public string MonthName(int month)
        {
            DateTimeFormatInfo dtInfo = new CultureInfo("es-MX", false).DateTimeFormat;
            return dtInfo.GetMonthName(month);
        }

        public static int GetExcelColumnNumber(string ColumnName)
        {

            int columnNumber = 0;

            for (int i = 0; i < ColumnName.Length; i++)
            {

                columnNumber =
                    columnNumber * 26 + (Convert.ToInt32(ColumnName[i]) - 64)

                    ;
            }

            return columnNumber;
        }

    }
}
