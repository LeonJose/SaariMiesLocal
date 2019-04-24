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
using Microsoft.CSharp;
namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class ReporteGlobal
    {
        string InmobiliariaID = string.Empty;
        string ConjuntoID = string.Empty;
        string InmobiliariaNombre = string.Empty;
        string nameConjunto = string.Empty;
        DateTime FechaCorte = DateTime.Now;
        bool esPDF = false;
        DateTime fechaInicioPer = DateTime.Now;
        DateTime fechaFinPer = DateTime.Now;
        BackgroundWorker bgWorker;
        bool usarSinComplementos = false;

        public ReporteGlobal(string idInmo, string nombreInmom, string nombreConjunto, string idConjunto, DateTime fechaCorte, bool espdf, BackgroundWorker bk)
        {
            this.InmobiliariaID = idInmo;
            this.InmobiliariaNombre = nombreInmom;
            this.ConjuntoID = idConjunto;
            this.FechaCorte = fechaCorte;
            this.esPDF = espdf;
            this.bgWorker = bk;
            this.nameConjunto = nombreConjunto;
        }

        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {

            return SaariDB.getListaInmobiliarias();
        }

        public static List<ConjuntoEntity> obtenerConjunto(string IdInmobiliaria)
        {
            return SaariDB.getConjuntos(IdInmobiliaria);
        }

        public List<string> GetEncabezadoEntity(int n)
        {
            if (n == 1)
                return Enum.GetNames(typeof(EncabezadoReprotesAnalisDeudores1)).ToList();
            else
                return Enum.GetNames(typeof(EncabezadoReprotesAnalisDeudores2)).ToList();
        }

        public string generarReporte()
        {
            try
            {
                try { usarSinComplementos = Properties.Settings.Default.usarSinComplementos; }
                catch { usarSinComplementos = false; }
                #region Variables
                int renglonSumaFin = 0;
                int sumaRen1 = 1;
                int sumaRen2 = 1;
                int sumaRen3 = 1;
                int renglonSumaFin2 = 0;
                int porcentaje = 0;
                int totalContratros = 0;
                int conContrato = 1;
                int columna = 1, renglon = 1;
                int i = 0;
                int r1 = 0;
                int r2 = 0;
                int cargo = 0;
                int centrar = 0;
                int cols = 1;
                string colAct = string.Empty;
                int contCargos = 0;
                Color colorCelda;
                string idComplemento = string.Empty;
                bool factLibres = false;
                int dia = 1;
                bool hayDolares = false;
                string rStyle = string.Empty;
                string rStyle2 = string.Empty;
                decimal importeCP = 0;
                string rengloCP = string.Empty;
                decimal totalFacIva = 0;
                string renglonTf = string.Empty;
                string St = string.Empty;
                #endregion

                #region OBTENCION DATOS

                fechaInicioPer = new DateTime(FechaCorte.Year, FechaCorte.Month, dia);
                dia = DateTime.DaysInMonth(FechaCorte.Year, FechaCorte.Month);
                fechaFinPer = new DateTime(FechaCorte.Year, FechaCorte.Month, dia);
                List<SubtipoEntity> ListTipoCont = SaariDB.getSubTiposOI();
                List<SubtipoEntity> ListTipoContAux = new List<SubtipoEntity>();
                bgWorker.ReportProgress(5);
                var ListaContratos = SaariDB.getContratosPorInmobiliariayConjunto(InmobiliariaID, ConjuntoID, FechaCorte, fechaInicioPer, fechaFinPer);

                if (ListaContratos.Count > 0)
                {
                    totalContratros = ListaContratos.Count;
                    var ListaContratosDolares = ListaContratos.FindAll(c => c.MonedaContrato == "D").ToList();
                    ListaContratos = ListaContratos.FindAll(x => x.MonedaContrato == "P").ToList();
                    List<ReporteGlobalEntity> UltiimoPagoMN = new List<ReporteGlobalEntity>();
                    foreach (ReporteGlobalEntity contratoDolares in ListaContratosDolares)
                    {
                        ReporteGlobalEntity ConversionUltimoPago = new ReporteGlobalEntity();
                        ConversionUltimoPago = SaariDB.getListaSumaAndUltimoAbono(InmobiliariaID, contratoDolares.IdContrato, contratoDolares.IdCliente, fechaInicioPer, FechaCorte, true);
                        UltiimoPagoMN.Add(ConversionUltimoPago);
                    }

                    List<ReciboEntity> ListaRecibosRenta = new List<ReciboEntity>();// recibos en dolares de Renta
                    List<ReciboEntity> ConversionDolaresaMN = new List<ReciboEntity>();
                    List<ReciboEntity> ConversionDolaresaMNCargos = new List<ReciboEntity>();
                    ReciboEntity cargosRecibos = new ReciboEntity();
                    List<ReciboEntity> LisNCenDll = new List<ReciboEntity>();
                    List<ReporteGlobalEntity> ListIntesesDllaMN = new List<ReporteGlobalEntity>();//intereses
                    SaldoEntity SaldoConvercionDlls = new SaldoEntity();
                    List<SaldosEntity> listaSaldosConversion = new List<SaldosEntity>();//Lista para convercion de Saldos Apesos
                    List<ReciboEntity> ListaCargosFacturados = SaariDB.getCargosFacturadosEnPeriodo(InmobiliariaID, fechaInicioPer, fechaFinPer);
                    List<SaldoAFavorEntity> ListSumatoriaSaldo = SaariE.getSumatoriaSaldoAFavor();

                    #endregion OBTENCION DATOS
                    if (bgWorker.CancellationPending)
                        return "Proceso cancelado por el usuario";
                    #region CONFIGURACION REPORTE


                    string path = Properties.Settings.Default.RutaRepositorio.EndsWith(@"\") ? Properties.Settings.Default.RutaRepositorio + @"ReporteGlobal\" : Properties.Settings.Default.RutaRepositorio + @"\ReporteGlobal\";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string filename = path + "Analisis de Deudores_" + InmobiliariaNombre + "_" + FechaCorte.Day + "-" + FechaCorte.Month + "-" + FechaCorte.Year + "-" + DateTime.Now.Millisecond + ".xlsx";
                    Excel.Application aplicacionExcel = new Excel.Application();
                    //aplicacionExcel.Workbooks.Add(Type.Missing);
                    Excel.Workbook libroExcel = aplicacionExcel.Workbooks.Add();
                    Excel.Worksheet hojaExcel = (libroExcel.ActiveSheet as Excel.Worksheet);
                    Excel.Range rango;// hojaExcel.get_Range("A1:" + GetExcelColumnName(Meses.Count + 3) + "1");
                    //Worksheet excelSheet;

                    //aplicacionExcel.Visible = true;


                    #endregion CONFIGURACION REPORTE

                    #region ENCABEZADOS
                    List<string> E2 = new List<string>();
                    try
                    {
                        if (bgWorker.CancellationPending)
                            return "Proceso cancelado por el usuario";
                        #region ENCABEZADO TITULO
                        //Titulo Nombre del reporte
                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        //rango.Interior.Color = Color.FromArgb(31, 98, 145);
                        rango.Font.Color = Color.FromArgb(0, 0, 0);
                        rango.Font.FontStyle = "Bold";
                        rango.Font.Size = 14;
                        rango.RowHeight = 29;
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        rango.Value2 = "ANALISIS DEUDORES " + InmobiliariaNombre;
                        renglon++;

                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        rango.Font.Color = Color.FromArgb(0, 0, 0);
                        rango.Font.FontStyle = "Bold";
                        rango.Font.Size = 14;
                        rango.RowHeight = 20;
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        rango.Value2 = nameConjunto;
                        renglon++;

                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        //rango.Interior.Color = Color.FromArgb(31, 98, 145);
                        rango.Font.Color = Color.FromArgb(0, 0, 0);
                        rango.Font.Size = 12;
                        rango.RowHeight = 20;
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        rango.Value2 = "FECHA DE CORTE " + FechaCorte.Day + "/" + MonthName(FechaCorte.Month).ToUpper() + "/" + FechaCorte.Year;
                        renglon++;
                        columna = 1;


                        #endregion ENCABEZADO TITULO


                        #region ENCABEZADO CARGOS

                        #region Encabeszados con complementos
                        List<string> E1 = GetEncabezadoEntity(1);
                        foreach (string encabezado in E1)
                        {
                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Interior.Color = Color.FromArgb(54, 96, 146);
                            rango.Font.Color = Color.FromArgb(255, 255, 255);
                            rango.Font.Size = 12;
                            rango.Font.FontStyle = "Bold";
                            rango.RowHeight = 107;
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = encabezado.Replace("_", " ");
                            rango.WrapText = true;
                            columna++;
                            cols++;
                        }
                        #endregion

                        #region Encabezado complementos
                        try
                        {
                            if (ListTipoCont != null)
                            {
                                if (ListTipoCont.Count > 0)
                                {
                                    if (usarSinComplementos)
                                    {
                                        #region Encabezados Sin Complemento
                                        foreach (SubtipoEntity sub in ListTipoCont)
                                        {
                                            if (!sub.Nombre.Contains("COMPLEMENTO"))
                                            {
                                                int contHeigth = 1;
                                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                                rango.Interior.Color = Color.FromArgb(54, 96, 146);
                                                rango.Font.Color = Color.FromArgb(255, 255, 255);
                                                rango.Font.Size = 12;
                                                rango.Font.FontStyle = "Bold";
                                                rango.RowHeight = 107;
                                                if (contHeigth <= 5)
                                                {
                                                    rango.ColumnWidth = 15;
                                                    contHeigth++;
                                                }
                                                else if (contHeigth > 5)
                                                {
                                                    rango.ColumnWidth = 12;
                                                    contHeigth++;
                                                }
                                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                                rango.Value2 = sub.Nombre;
                                                rango.WrapText = true;
                                                rango.ColumnWidth = 13;
                                                columna++;
                                                ListTipoContAux.Add(sub);
                                                cols++;
                                            }// if complemento

                                        }//foreach
                                        #endregion
                                    } //usar sin complemento   
                                    else
                                    {
                                        #region Encabezados con Complemento
                                        foreach (SubtipoEntity sub in ListTipoCont)
                                        {
                                            int contHeigth = 1;
                                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                            rango.Interior.Color = Color.FromArgb(54, 96, 146);
                                            rango.Font.Color = Color.FromArgb(255, 255, 255);
                                            rango.Font.Size = 12;
                                            rango.Font.FontStyle = "Bold";
                                            rango.RowHeight = 107;
                                            if (contHeigth <= 5)
                                            {
                                                rango.ColumnWidth = 15;
                                                contHeigth++;
                                            }
                                            else if (contHeigth > 5)
                                            {
                                                rango.ColumnWidth = 12;
                                                contHeigth++;
                                            }
                                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                            rango.Value2 = sub.Nombre;
                                            rango.WrapText = true;
                                            rango.ColumnWidth = 13;
                                            columna++;
                                            cols++;
                                            ListTipoContAux.Add(sub);
                                        }
                                        #endregion
                                    }
                                }
                            }

                        }//try
                        catch
                        {
                            return "Error encabezado de conceptos de mantenimeinto";
                        }
                        #endregion

                        #region Encabezados con complementos 2


                        E2 = GetEncabezadoEntity(2);
                        foreach (string encabezado in E2)
                        {
                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Interior.Color = Color.FromArgb(54, 96, 146);
                            rango.Font.Color = Color.FromArgb(255, 255, 255);
                            rango.Font.Size = 12;
                            rango.Font.FontStyle = "Bold";
                            rango.RowHeight = 107;
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = encabezado.Replace("_", " ");
                            rango.WrapText = true;
                            columna++;
                            cols++;
                        }
                        #endregion

                        renglon++;
                        hojaExcel.get_Range("A3:A3").ColumnWidth = 20;
                        hojaExcel.get_Range("B3:C3").ColumnWidth = 12;
                        hojaExcel.get_Range("D3:F3").ColumnWidth = 30;

                        for (i = 1; i <= 3; i++)
                        {
                            rango = hojaExcel.get_Range("A" + i + ":" + GetExcelColumnName(cols) + i);
                            rango.Select();
                            rango.MergeCells = true;
                        }
                        hojaExcel.Application.ActiveWindow.SplitRow = 4;
                        hojaExcel.Application.ActiveWindow.FreezePanes = true;

                        #endregion

                    }
                    catch
                    {
                        return "Error al poner emcabezados.";
                    }
                    #endregion ENCABEZADOS
                    decimal saldoTotalDllaMN = 0;
                    bgWorker.ReportProgress(8);
                    if (bgWorker.CancellationPending)
                        return "Proceso cancelado por el usuario";
                    #region CONTRATOS
                    for (int repetir = 1; repetir <= 3; repetir++)//1-Contratos Pesos, 2-Contratos Dolares, 3-Facturas libres
                    {
                        #region Tomar lista a realizar
                        var ListaARealizar = new List<ReporteGlobalEntity>();

                        switch (repetir)
                        {
                            case 1:
                                ListaARealizar = ListaContratos;
                                break;
                            case 2:
                                ListaARealizar = ListaContratosDolares;
                                break;
                            case 3:
                                factLibres = true;
                                ListaARealizar = SaariDB.getFacturaslibres(InmobiliariaID, fechaInicioPer, FechaCorte);
                                break;
                            default:
                                return "Error tratar seleccionar Lista de contratos para generacion de reporte.";
                        }


                        if (ListaARealizar != null)
                        {
                            if (ListaARealizar.Count > 0)
                            {
                                string title = "";
                                if (repetir == 1)
                                {
                                    title = "CONTRATOS EN PESOS";
                                }
                                else if (repetir == 2)
                                {
                                    hayDolares = true;
                                    title = "CONTRATOS EN DOLARES";
                                }
                                else
                                    title = "FACTURAS LIBRES";
                                rango = (hojaExcel.Cells[renglon, 1] as Excel.Range);
                                rango.Font.Color = Color.FromArgb(0, 0, 0);
                                rango.Font.Size = 12;
                                rango.RowHeight = 39;
                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                rango.Value2 = title;
                                rango = hojaExcel.get_Range("A" + renglon + ":" + GetExcelColumnName(cols - 1) + renglon);
                                rango.Select();
                                rango.MergeCells = true;
                                renglon++;
                                columna = 1;



                                renglonSumaFin2 = renglon;
                                if (bgWorker.CancellationPending)
                                    return "Proceso cancelado por el usuario";
                                #endregion

                                var porcent = 0;
                                porcent = 100 / totalContratros;
                                foreach (var contrato in ListaARealizar)
                                {
                                    
                                       string ncol = string.Empty;

                                    if (contrato.yaVencioContrato && contrato.tineAdeudo || contrato.yaVencioContrato && contrato.RecibosPorContrato.Count > 0 || !contrato.yaVencioContrato)
                                    {
                                        decimal div = 0;
                                        if (repetir != 3)
                                        {
                                            div = (porcent * conContrato);
                                            porcentaje = Convert.ToInt32(div);
                                            if (porcentaje >= 100)
                                                porcentaje = 99;
                                            bgWorker.ReportProgress(Convert.ToInt32(porcentaje));
                                        }
                                        if (bgWorker.CancellationPending)
                                            return "Proceso cancelado por el usuario";

                                        #region DATOS CONTRATO
                                        columna = 1;
                                        if ((renglon % 2) == 0)
                                        {
                                            colorCelda = Color.FromArgb(255, 255, 255);
                                        }
                                        else
                                        {
                                            colorCelda = Color.FromArgb(225, 237, 255);
                                        }


                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                        rango.Value2 = contrato.NombreInmueble;//contrato.IdEdificio + " - " +
                                        columna++;

                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                        rango.Value2 = Math.Round(contrato.M2Construccion, 2);
                                        rango.NumberFormat = "#,###.00";
                                        columna++;

                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                        rango.Value2 = contrato.CostoM2; //"=IFERROR(G" + renglon + "/B" + renglon+",0)";//
                                        rango.NumberFormat = "#,###.00";
                                        columna++;

                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                        rango.Value2 = contrato.NombreCliente;
                                        columna++;

                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                        rango.Value2 = contrato.Actividad;
                                        columna++;

                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                        rango.Value2 = contrato.NombreComercial;
                                        columna++;
                                        #endregion

                                        #region IMPORTE RENTA SIN IVA
                                        ReciboEntity recEnt = new ReciboEntity();
                                        List<ReciboEntity> ListRecibos = new List<ReciboEntity>();

                                        if (!factLibres)
                                            ListRecibos = contrato.RecibosPorContrato;
                                        
                                        List<ReciboEntity> ListRecibosXCliente = new List<ReciboEntity>();

                                        try
                                        {
                                            if (ListRecibos != null)
                                            {
                                                if (ListRecibos.Count > 0)
                                                {
                                                    ReciboEntity converDolares = new ReciboEntity();
                                                    if (!factLibres)
                                                    {
                                                        foreach (ReciboEntity li in ListRecibos)
                                                        {
                                                            if (li.FechaEmision >= fechaInicioPer && li.FechaEmision <= FechaCorte)
                                                            {
                                                                if (usarSinComplementos)
                                                                {
                                                                    if (li.TipoDoc == "R" && string.IsNullOrEmpty(li.Campo20) || li.Campo20 == "CRTA" || li.TipoDoc == "X" && string.IsNullOrEmpty(li.Campo20))
                                                                    {
                                                                        //recEnt.SumaConMNImporte += (li.Importe - li.IVARetenido) - li.ISR - li.Descuento - li.Cargo;
                                                                        recEnt.SumaConMNImporte += li.Importe - li.Cargo;
                                                                        recEnt.SumaConMNIVA += li.IVA;
                                                                        recEnt.TotalIVA += li.Importe + li.IVA - (li.IVARetenido + li.ISR + li.Descuento);// +li.IVA;
                                                                        recEnt.Abono += li.Abono;
                                                                        recEnt.IDHistRec = li.IDHistRec;
                                                                        recEnt.Total += li.Total;
                                                                        ListRecibosXCliente.Add(li);

                                                                        if (hayDolares)
                                                                        {
                                                                            converDolares.IDContrato = contrato.IdContrato;
                                                                            converDolares.IDCliente = contrato.IdCliente;
                                                                            converDolares.IDInmobiliaria = InmobiliariaID;
                                                                            //converDolares.SumaConMNImporte = (li.Importe - li.IVARetenido) - li.ISR - li.Descuento - li.Cargo;
                                                                            converDolares.SumaConMNImporte = li.Importe - li.Cargo;
                                                                            converDolares.SumaConMNIVA = li.IVA;
                                                                            //converDolares.TotalIVA = li.Importe + li.IVA - (li.IVARetenido - li.ISR);// +li.IVA;
                                                                            converDolares.TotalIVA = li.Importe + li.IVA - (li.IVARetenido + li.ISR + li.Descuento);
                                                                            converDolares.Abono = li.Abono;
                                                                            converDolares.IDHistRec = li.IDHistRec;
                                                                            converDolares.Total = li.Total;
                                                                            converDolares.TipoCambio = li.TipoCambio;
                                                                            ListaRecibosRenta.Add(converDolares);
                                                                        }

                                                                    }
                                                                }
                                                                else
                                                                {

                                                                    if (li.TipoDoc == "R" && string.IsNullOrEmpty(li.Campo20) || li.TipoDoc == "X" && string.IsNullOrEmpty(li.Campo20))
                                                                    {
                                                                        recEnt.SumaConMNImporte += (li.Importe  - li.Descuento - li.Cargo);
                                                                        recEnt.SumaConMNIVA += li.IVA;
                                                                        recEnt.TotalIVA += ((li.Importe + li.IVA - li.IVARetenido) - li.ISR);// +li.IVA;
                                                                        recEnt.Abono += li.Abono;
                                                                        recEnt.IDHistRec = li.IDHistRec;
                                                                        recEnt.Total += li.Total;
                                                                        ListRecibosXCliente.Add(li);
                                                                        if (hayDolares)
                                                                        {
                                                                            converDolares.IDContrato = contrato.IdContrato;
                                                                            converDolares.IDCliente = contrato.IdCliente;
                                                                            converDolares.IDInmobiliaria = InmobiliariaID;
                                                                            converDolares.SumaConMNImporte = (li.Importe - li.IVARetenido) - li.ISR - li.Descuento - li.Cargo;
                                                                            converDolares.SumaConMNIVA = li.IVA;
                                                                            converDolares.TotalIVA = ((li.Importe + li.IVA - li.IVARetenido) - li.ISR);// +li.IVA;
                                                                            converDolares.Abono = li.Abono;
                                                                            converDolares.IDHistRec = li.IDHistRec;
                                                                            converDolares.Total = li.Total;
                                                                            converDolares.TipoCambio = li.TipoCambio;
                                                                            ListaRecibosRenta.Add(converDolares);
                                                                        }
                                                                    }
                                                                }


                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            return "Error al calcular lo facturado en el mes corriente";
                                        }

                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        rango.Value2 = recEnt.SumaConMNImporte;
                                        rango.NumberFormat = "#,###.00";
                                        columna++;
                                        #endregion

                                        #region CARGOS
                                        try
                                        {
                                            if (ListTipoContAux != null)
                                            {
                                                if (ListTipoContAux.Count > 0)
                                                {
                                                    #region CON SUBTIPO
                                                    //if (usarSinComplementos)
                                                    //{
                                                    #region Sin Complemento
                                                    foreach (SubtipoEntity CargosContrato in ListTipoContAux)
                                                    {
                                                        decimal totalCargos = 0;
                                                        if (!factLibres)
                                                        {

                                                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                                            try
                                                            {
                                                                idComplemento = ListTipoCont.Find(com => com.Nombre.Contains("COMPLEMENTO DE" + " " + CargosContrato.Nombre)).Identificador;

                                                            }
                                                            catch
                                                            {
                                                                idComplemento = string.Empty;
                                                            }

                                                            try
                                                            {
                                                                foreach (ReciboEntity r in ListRecibosXCliente)
                                                                {
                                                                    try
                                                                    {
                                                                        totalCargos += SaariDB.getCargosFacturadosxIdHistRec(r.IDHistRec, contrato.IdContrato, CargosContrato.Identificador).Sum(s => s.Importe); //SaariDB.getCargosFacturadosxIdHistRec(r.IDHistRec,contrato);   
                                                                        if (!string.IsNullOrEmpty(idComplemento))
                                                                            totalCargos += SaariDB.getCargosFacturadosxIdHistRec(r.IDHistRec, contrato.IdContrato, idComplemento).Sum(s => s.Importe);

                                                                        if (hayDolares)
                                                                        {
                                                                            r.MonedaContrato = "D";
                                                                            ConversionDolaresaMNCargos.Add(r);
                                                                        }
                                                                    }
                                                                    catch
                                                                    {
                                                                        totalCargos += 0;
                                                                    }
                                                                }
                                                                foreach (ReciboEntity cargoRec in contrato.RecibosPorContrato)
                                                                {
                                                                    if (cargoRec.Campo20 == CargosContrato.Identificador)
                                                                    {
                                                                        if (contrato.MonedaContrato == cargoRec.Moneda)
                                                                        {
                                                                            totalCargos += cargoRec.Importe;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (contrato.MonedaContrato == "P" && cargoRec.Moneda == "D")
                                                                            {
                                                                                totalCargos += (cargoRec.TipoCambio * cargoRec.Importe);
                                                                            }
                                                                            else if (contrato.MonedaContrato == "D" && cargoRec.Moneda == "P")
                                                                            {
                                                                                totalCargos += (cargoRec.TipoCambio / cargoRec.Importe);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                //totalCargos += contrato.RecibosPorContrato.FindAll(cr => cr.Campo20 == CargosContrato.Identificador).Sum(s => s.Importe);
                                                                //if (!string.IsNullOrEmpty(idComplemento))
                                                                //    totalCargos += contrato.RecibosPorContrato.FindAll(cr => cr.Campo20 == idComplemento).Sum(s => s.Importe);
                                                                if (CargosContrato.Identificador == "CP")
                                                                {
                                                                    importeCP = totalCargos;
                                                                    rengloCP = GetExcelColumnName(columna);
                                                                }
                                                            }
                                                            catch
                                                            {

                                                            }
                                                        }
                                                        else
                                                            rango.Value2 = totalCargos = 0;

                                                        rango.Value2 = totalCargos;
                                                        rango.NumberFormat = "#,###.00";
                                                        columna++;
                                                    }// Foreach
                                                    #endregion
                                                    //}

                                                    #endregion
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            return "Error al tratar de realizar la suma por conceptos";
                                        }
                                        #endregion

                                        #region NO IDENTIFICADOS
                                        decimal totalCargos2 = 0;
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        if (!factLibres)
                                        {
                                            try
                                            {
                                                totalCargos2 += contrato.RecibosPorContrato.FindAll(cf => cf.IDContrato == contrato.IdContrato && string.IsNullOrEmpty(cf.Campo20) && cf.TipoDoc == "Z").Sum(s => s.Importe);
                                                ConversionDolaresaMNCargos.AddRange(contrato.RecibosPorContrato.FindAll(cf => cf.IDContrato == contrato.IdContrato && string.IsNullOrEmpty(cf.Campo20) && cf.TipoDoc == "Z"));
                                            }
                                            catch
                                            {
                                                totalCargos2 = 0;
                                            }
                                        }
                                        rango.Value2 = totalCargos2;
                                        rango.NumberFormat = "#,###.00";
                                        columna++;

                                        #endregion

                                        #region TOTAL FACTURADO SIN IVA y CON IVA

                                        int col = columna;
                                        col = col - 1;
                                        colAct = GetExcelColumnName(col);
                                        decimal sumafac = 0;
                                        //FACTURADO SIN IVA
                                        // public static List<ReciboEntity> getListaRecibos(ClienteEntity cliente, DateTime fechaCorte) toma todo lo facturado en la historia del cliente
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        rango.Interior.Color = colorCelda;
                                        if (!factLibres)

                                            if (contrato.RecibosPorContrato.Exists(e => e.FechaEmision >= fechaInicioPer && e.FechaEmision <= FechaCorte))
                                            {
                                                foreach (ReciboEntity fac in contrato.RecibosPorContrato)
                                                {
                                                    if (fac.FechaEmision >= fechaInicioPer && fac.FechaEmision <= FechaCorte)
                                                        if (contrato.MonedaContrato == fac.Moneda)
                                                        {
                                                            sumafac += fac.Importe;
                                                        }
                                                        else
                                                        {
                                                            if (contrato.MonedaContrato == "P" && fac.Moneda == "D")
                                                            {
                                                                sumafac += (fac.TipoCambio * fac.Importe);
                                                            }
                                                            else if (contrato.MonedaContrato == "D" && fac.Moneda == "P")
                                                            {
                                                                sumafac += (fac.TipoCambio / fac.Importe);
                                                            }
                                                        }

                                                }
                                                rango.Value2 = Math.Round(sumafac, 4);
                                            }
                                            else
                                                rango.Value2 = 0;
                                        //rango.Formula = ListRecibos.Sum(s => s.Importe);
                                        else
                                        {
                                            if (contrato.RecibosPorContrato.Exists(e => e.FechaEmision >= fechaInicioPer && e.FechaEmision <= FechaCorte))
                                                rango.Value2 = contrato.RecibosPorContrato.FindAll(f => f.FechaEmision >= fechaInicioPer && f.FechaEmision <= FechaCorte).Sum(s => s.Importe);
                                            else
                                                rango.Value2 = 0;
                                        }

                                        rango.NumberFormat = "#,###.00";
                                        columna++;
                                        totalFacIva = 0;


                                        // FACTURADO CON IVA
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        if (!factLibres)
                                        {
                                            if (contrato.RecibosPorContrato.Exists(e => e.FechaEmision >= fechaInicioPer && e.FechaEmision <= FechaCorte))
                                            {
                                                foreach (ReciboEntity fac in contrato.RecibosPorContrato)
                                                {
                                                    if (fac.FechaEmision >= fechaInicioPer && fac.FechaEmision <= FechaCorte)
                                                        if (contrato.MonedaContrato == fac.Moneda)
                                                        {
                                                            totalFacIva += fac.Total;//(fac.Total + fac.IVA) - (fac.ISR + fac.IVARetenido);
                                                        }
                                                        else
                                                        {
                                                            if (contrato.MonedaContrato == "P" && fac.Moneda == "D")
                                                            {
                                                                // + (fac.TipoCambio * fac.IVA) + (fac.TipoCambio * fac.ISR) + (fac.TipoCambio * fac.IVARetenido));
                                                                totalFacIva += ((fac.TipoCambio * fac.Total));// + (fac.TipoCambio * fac.IVA) - ((fac.TipoCambio * fac.ISR) + (fac.TipoCambio * fac.IVARetenido)));
                                                            }
                                                            else if (contrato.MonedaContrato == "D" && fac.Moneda == "P")
                                                            {
                                                                totalFacIva += (( fac.Total / fac.TipoCambio ));//+ (fac.TipoCambio * fac.IVA) - ((fac.TipoCambio * fac.ISR) + (fac.TipoCambio * fac.IVARetenido))) / fac.TipoCambio; //((fac.TipoCambio * fac.Total) / (fac.TipoCambio));
                                                            }
                                                        }

                                                }
                                                Math.Round(totalFacIva, 4);
                                            }
                                            else
                                                rango.Value2 = 0;
                                            //totalFacIva = ListRecibos.Sum(s => s.Total);//recEnt.TotalIVA ;///
                                            rango.Value2 = totalFacIva;

                                        }
                                        else
                                        {
                                            if (contrato.RecibosPorContrato.Exists(e => e.FechaEmision >= fechaInicioPer && e.FechaEmision <= FechaCorte))
                                            {
                                                totalFacIva = contrato.RecibosPorContrato.FindAll(f => f.FechaEmision >= fechaInicioPer && f.FechaEmision <= FechaCorte).Sum(s => s.Importe + s.IVA);
                                                rango.Value2 = totalFacIva;
                                            }
                                            else
                                                rango.Value2 = 0;

                                        }
                                        renglonTf = GetExcelColumnName(columna);
                                        //rango.Value2 = rango.Value2 = contrato.RecibosPorContrato.Sum(s => s.Importe + s.IVA);
                                        rango.NumberFormat = "#,###.00";
                                        columna++;

                                        #endregion

                                        #region SALDO INMEDIATO MES ANTERIOR
                                        ContratosEntity objContratos = new ContratosEntity();
                                        objContratos.IDContrato = contrato.IdContrato;
                                        objContratos.IDCliente = contrato.IdCliente;
                                        objContratos.IDArrendadora = InmobiliariaID;
                                        objContratos.Moneda = contrato.MonedaContrato;
                                        //TO DO: SE PUEDE OPTIMIZAR REALIZANDO LA SUMATORIA DESDE EL QUERY.
                                        List<SaldoEntity> listaSaldosBD = SaariDB.getSaldos2(objContratos, fechaInicioPer, fechaFinPer);
                                        List<SaldosEntity> listaSaldos = new List<SaldosEntity>();
                                        decimal saldoTotal = 0;
                                        decimal SaldoInmediatoAnterior = 0;
                                        try
                                        {
                                            //foreach (SaldoEntity s in listaSaldosBD)
                                            //{
                                            //    saldoTotal += s.PagoParcial;
                                            //}
                                            //SaldosEntity saldos = new SaldosEntity()
                                            //{
                                            // Saldo = saldoTotal - SaariDB.getSaldoAnterior(objContratos, fechaInicioPer)
                                            //};
                                            //listaSaldos.Add(saldos);
                                            //contrato.IdContrato == "CNT150" 
                                            //decimal saldoTotal = 0;
                                            foreach (SaldoEntity s in listaSaldosBD)
                                            {
                                                saldoTotal += s.PagoParcial;
                                            }
                                            SaldosEntity saldos = new SaldosEntity()
                                            {
                                                Saldo = saldoTotal - SaariDB.getSaldoAnterior(objContratos, fechaInicioPer)
                                            };
                                            //List<SaldosEntity> listaSaldos = new List<SaldosEntity>();
                                            listaSaldos.Add(saldos);

                                            if (hayDolares)
                                            {
                                                objContratos.Moneda = "P";
                                                List<SaldoEntity> listaSaldosaMNBD = SaariDB.getSaldoInicioOperaciones(objContratos, FechaCorte, hayDolares);//SALDO Converson a pesos
                                                foreach (SaldoEntity s in listaSaldosBD)
                                                {
                                                    if (s.Estatus == "2")
                                                        saldoTotalDllaMN += (s.PagoParcial * s.TipoCambioPago);
                                                    else if (s.Estatus == "1")
                                                        saldoTotalDllaMN += (s.PagoParcial * s.TipoCambio);

                                                }
                                                SaldosEntity saldosAnterior = new SaldosEntity()
                                                {
                                                    Saldo = saldoTotalDllaMN - SaariDB.getSaldoAnterior(objContratos, fechaInicioPer)
                                                };
                                                listaSaldosConversion.Add(saldosAnterior);
                                            }

                                            SaldoInmediatoAnterior = listaSaldos.FirstOrDefault().Saldo;
                                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                            rango.Value2 = SaldoInmediatoAnterior;
                                            rango.NumberFormat = "#,###.00";
                                            columna++;
                                        }
                                        catch
                                        {
                                            return "Error al calcular saldo inmediado mes anterior";
                                        }

                                        #endregion

                                        #region Saldo Total Mes Corriente + Saldo Inmediato Mes Anterior
                                        // decimal TotalSaldo = recEnt.TotalIVA + SaldoInmediatoAnterior;
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        if (!factLibres)
                                        {
                                            rango.Formula = "=SUM(" + GetExcelColumnName(columna - 2) + renglon + "+" + GetExcelColumnName(columna - 1) + renglon + ")";//TotalSaldo;// +listaSaldos[0].Saldo;
                                        }
                                        else
                                        {
                                            rango.Value2 = contrato.RecibosPorContrato.Sum(s => s.Importe + s.IVA);
                                        }
                                        rango.NumberFormat = "#,###.00";
                                        columna++;
                                        #endregion

                                        #region TOTAL ABONOS MES CORRIENTE Y MES DE ABONO
                                        try
                                        {
                                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                            if (!factLibres)
                                            {
                                                if (FechaCorte.Month == contrato.FechaUltimoAbono.Month && FechaCorte.Year == contrato.FechaUltimoAbono.Year)
                                                    rango.Value2 = contrato.SumaAbonadoMes + contrato.SaldoAFavorPeriodo;
                                                else
                                                    rango.Value2 = 0;
                                            }
                                            else
                                            {
                                                if (!factLibres)
                                                {
                                                    decimal t = contrato.RecibosPorContrato.Sum(s => s.Abono); //ListRecibos.Where(w=> w.Estatus == "2").Sum(s => s.TotalPagado);
                                                    t += contrato.SaldoAFavorPeriodo;
                                                    rango.Value2 = t;
                                                }
                                                else
                                                {
                                                    decimal abonado = 0;
                                                    //foreach (ReciboEntity r in contrato.RecibosPorContrato)
                                                    //{
                                                        //if (r.TipoDoc == "R" && r.Estatus == "2")
                                                        //{
                                                        abonado += contrato.RecibosPorContrato.FindAll(f => f.FechaPago >= fechaInicioPer && f.FechaPago <= FechaCorte).Sum(s => s.Abono);
                                                        //}
                                                        //else
                                                        //{
                                                        //    abonado +=contrato.RecibosPorContrato.FindAll(f => f.FechaPago >= fechaInicioPer && f.FechaPago <= FechaCorte).Sum(s => s.Abono);
                                                        //}
                                                    //}
                                                    abonado += contrato.SaldoAFavorPeriodo;
                                                    rango.Value2 = abonado;//contrato.RecibosPorContrato.FindAll(f => f.FechaPago >= fechaInicioPer && f.FechaPago <= FechaCorte).Sum(s => s.Importe+s.IVA);
                                                }
                                            }
                                            columna++;
                                            rango.NumberFormat = "#,###.00";


                                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                            if (!factLibres)
                                            {
                                                DateTime fechaValidar = new DateTime(1, 1, 1);
                                                if (fechaValidar.Date != contrato.FechaUltimoAbono)
                                                {
                                                    //if (FechaCorte.Month == contrato.FechaUltimoAbono.Month && FechaCorte.Year == contrato.FechaUltimoAbono.Year)
                                                    rango.Value2 = MonthName(contrato.FechaUltimoAbono.Month).ToUpper() + " " + contrato.FechaUltimoAbono.Year;
                                                }
                                                else
                                                {
                                                    DateTime mesAbono = SaariDB.getFechaUltimoAbono(InmobiliariaID, contrato.IdContrato, fechaInicioPer, FechaCorte).FechaUltimoAbono;
                                                    if (fechaValidar.Date != mesAbono.Date)
                                                        rango.Value2 = MonthName(mesAbono.Month).ToUpper() + " " + mesAbono.Year;
                                                    else
                                                    {
                                                        rango.Value2 = "No se encontro Fecha Ultimo Abono";
                                                        ncol = GetExcelColumnName(columna) + renglon + ":" + GetExcelColumnName(columna) + renglon;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                contrato.RecibosPorContrato.OrderBy(o => o.MaxFechaPago);
                                                contrato.FechaUltimoAbono = contrato.RecibosPorContrato.LastOrDefault().MaxFechaPago;
                                                if (FechaCorte.Month == contrato.FechaUltimoAbono.Month && FechaCorte.Year == contrato.FechaUltimoAbono.Year)
                                                    rango.Value2 = MonthName(contrato.FechaUltimoAbono.Month).ToUpper() + " " + contrato.FechaUltimoAbono.Year;
                                                else
                                                    rango.Value2 = " - ";
                                            }

                                            rango.NumberFormat = "dd/mm/yyyy";
                                            columna++;
                                        }
                                        catch
                                        {
                                            return "Error al asignar ultimo abono";
                                        }

                                        #endregion

                                        #region Saldo total
                                        decimal stotal = 0;
                                        try
                                        {
                                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                            if (!factLibres)
                                            {
                                                rango.Value2 = "=" + GetExcelColumnName(columna - 3) + renglon + "-" + contrato.SumaAbonadoMes;// TotalSaldo - contrato.SumaAbonadoMes;
                                                St = GetExcelColumnName(columna);
                                            }
                                            else
                                                rango.Value2 = stotal = contrato.RecibosPorContrato.Sum(s => s.TotalxPagar);

                                            //rango.Value2 = stotal;
                                            rango.NumberFormat = "#,###.00";
                                            columna++;
                                        }
                                        catch
                                        {
                                            return "Error al tratar de tomar el saldo total";
                                        }
                                        #endregion

                                        #region Saldo total Con Intereses
                                        ReporteGlobalEntity SaldoConIntereses = SaariDB.InteresRecibosPendientesPago(InmobiliariaID, contrato.IdContrato, contrato.IdCliente, FechaCorte, contrato.MonedaContrato);
                                        if (hayDolares)
                                        {
                                            ReporteGlobalEntity InteresMN = SaariDB.InteresRecibosPendientesPago(InmobiliariaID, contrato.IdContrato, contrato.IdCliente, FechaCorte, "P");
                                            ListIntesesDllaMN.Add(InteresMN);
                                        }
                                        try
                                        {
                                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                            rango.Value2 = "=SUM(" + GetExcelColumnName(columna - 1) + renglon + "+" + SaldoConIntereses.SumaAbonadoMes;// stotal + SaldoConIntereses.SumaIntereses;//ListaSaldoConIntereses.Sum(Si => Si.Pago);
                                            rango.NumberFormat = "#,###.00";
                                            columna++;
                                        }
                                        catch
                                        {
                                            return "Error al tratar de tomar el saldo total";
                                        }
                                        #endregion

                                        #region SALDO A FAVOR
                                        decimal SaldoFavor = 0;
                                        try
                                        {
                                            //    decimal saldo = 0;
                                            //    decimal pagos = 0;
                                            //    List<SaldoAFavorEntity> salAFavor = new List<SaldoAFavorEntity>();
                                            //    salAFavor = ListSumatoriaSaldo.FindAll(c => c.IdCliente == contrato.IdCliente);
                                            //    if (salAFavor != null)
                                            //    {
                                            //        if (salAFavor.Count > 0)
                                            //        {
                                            //            foreach (SaldoAFavorEntity s in salAFavor)
                                            //            {
                                            //                saldo += s.SumaSaldoFavor;
                                            //                pagos += s.totalPagado;
                                            //            }
                                            //            SaldoFavor = saldo - pagos;
                                            //        }
                                            //        else
                                            //            SaldoFavor = 0;
                                            //    }
                                            //    else
                                            //        SaldoFavor = 0;
                                            SaldoFavor = contrato.SaldoAFavor;//ListSumatoriaSaldo.Find(c => c.IdCliente == contrato.IdCliente).SumaSaldoFavor;
                                        }
                                        catch
                                        {
                                            SaldoFavor = 0;
                                        }
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        rango.Value2 = SaldoFavor;
                                        rango.NumberFormat = "#,###.00";
                                        columna++;
                                        #endregion

                                        #region NOTA DE CREDITO
                                        try
                                        {
                                            List<ReciboEntity> ListaNC = SaariDB.getNCByContratoXPeriodo(contrato.IdContrato, fechaInicioPer, FechaCorte);
                                            if (hayDolares)
                                            {
                                                LisNCenDll = ListaNC;
                                            }
                                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                            if (ListaNC != null)
                                            {
                                                if (ListaNC.Count != 0)//this.formatRange.NumberFormat = "m/d/yyyy";
                                                {
                                                    try
                                                    {
                                                        rango.Value2 = ListaNC.Sum(s => s.Pago);//saldoTotalMesCorriente = LRS.TotalIVA + listaSaldos[0].Saldo;
                                                    }
                                                    catch
                                                    {
                                                        rango.Value2 = 0;
                                                    }

                                                }
                                                else
                                                    rango.Value2 = 0;
                                            }
                                            else
                                                rango.Value2 = 0;

                                            rango.NumberFormat = "#,###.00";
                                            columna++;
                                        }
                                        catch
                                        {
                                            return "Error al tomar la nota de credito";
                                        }


                                        #endregion

                                        #region MESES ADEUDO
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        try
                                        {
                                            rango.Value2 = string.Format("=({0}{1}-{2}{3})/{4}{5}", St, renglon, rengloCP, renglon, renglonTf, renglon);
                                            decimal cellValue = Convert.ToDecimal((hojaExcel.Cells[renglon, columna] as Excel.Range).Value2);
                                            cellValue = Math.Round(cellValue, 2);
                                            if (cellValue < 0)
                                            {
                                                rango.Value2 = 0;
                                            }
                                            else if (cellValue > 0 && cellValue < 1)
                                            {
                                                rango.Value2 = "Diferencia";
                                            }
                                            else
                                            {
                                                int n = (int)cellValue;
                                                if (n - cellValue == 0)
                                                {
                                                    rango.Value2 = n;
                                                }
                                                else
                                                {
                                                    rango.Value2 = n + " + Diferencia";

                                                }
                                            }

                                        }
                                        catch
                                        {
                                            rango.Value2 = 0;
                                        }
                                        columna++;

                                        #endregion

                                        #region DATOS CONRTATO 2

                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        rango.Value2 = contrato.InicioVigencia;
                                        rango.NumberFormat = "dd/mm/yyyy";
                                        columna++;

                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        rango.Value2 = contrato.FinVigencia;
                                        rango.NumberFormat = "dd/mm/yyyy";
                                        columna++;

                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        rango.Value2 = contrato.Deposito;
                                        rango.NumberFormat = "#,###.00";
                                        columna++;


                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        rango.Value2 = contrato.DiasGracia;
                                        columna++;

                                        //getListaRecibos
                                        List<ReciboEntity> ListInicioFact = SaariDB.getInicioFacturacionContrato(objContratos, FechaCorte);
                                        DateTime inicioFact = DateTime.Now;
                                        if (ListInicioFact != null)
                                        {
                                            if (ListInicioFact.Count > 0)
                                            {
                                                inicioFact = ListInicioFact.FirstOrDefault().FechaEmision;
                                            }

                                        }

                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        if (ListInicioFact.Count > 0)
                                            rango.Value2 = inicioFact;
                                        else
                                            rango.Value2 = "-";
                                        rango.NumberFormat = "dd/mm/yyyy";
                                        //columna++;
                                        #endregion


                                        //rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        //rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        //rango.Value2 = contrato.SaldoAFavor;
                                        //columna++;

                                        #region FORMATO RENGLON
                                        colAct = GetExcelColumnName(columna);
                                        rStyle = "A" + renglon;
                                        rStyle2 = colAct + renglon;
                                        Excel.Range newRng = aplicacionExcel.get_Range(rStyle, rStyle2);
                                        newRng.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        newRng.Borders.Color = Color.FromArgb(211, 211, 211);
                                        newRng.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                        //newRng.Font.Color = Color.Black;
                                        newRng.Interior.Color = colorCelda;

                                        if (!string.IsNullOrEmpty(ncol))
                                        {
                                            Excel.Range newRng2 = aplicacionExcel.get_Range(ncol, ncol);
                                            newRng2.Font.Size = 7;
                                            newRng2.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                            newRng2.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                            newRng2 = hojaExcel.get_Range(ncol, ncol);
                                            newRng2.Select();
                                            newRng2.MergeCells = true;
                                            newRng2.WrapText = true;
                                        }
                                        columna++;
                                        renglon++;
                                        i++;
                                        conContrato++;
                                        #endregion
                                    }
                                }

                                #region SUMATORIA CONTRATOS
                                if (repetir == 1)
                                {
                                    sumaRen1 = renglon;
                                }
                                else if (repetir == 2)
                                {
                                    sumaRen2 = renglon;
                                }
                                else
                                {
                                    sumaRen3 = renglon;
                                }

                                columna = 6;
                                renglonSumaFin = renglon - 1;
                                r1 = renglonSumaFin2;
                                r2 = renglon - 1;
                                cargo = 0;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                colAct = GetExcelColumnName(columna);
                                rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                rango.Font.FontStyle = "Bold";
                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                if (repetir == 1)
                                    rango.Value2 = "TOTAL CONTRATOS PESOS";
                                else if (repetir == 2)
                                    rango.Value2 = "TOTAL CONTRATOS DOLARES";
                                else if (repetir == 3)
                                    rango.Value2 = "TOTAL FACTURAS LIBRES";
                                columna++;
                                renglonSumaFin = renglon - 1;
                                r1 = renglonSumaFin2;
                                r2 = renglon - 1;
                                cargo = 0;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                colAct = GetExcelColumnName(columna);
                                rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                rango.Font.FontStyle = "Bold";
                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2).ToString();
                                rango.NumberFormat = "$ #,##0.00";
                                columna++;


                                if (usarSinComplementos)
                                    contCargos = ListTipoContAux.Count + 1;
                                else
                                    contCargos = ListTipoCont.Count + 1;

                                for (cargo = 1; cargo <= contCargos; cargo++)
                                {

                                    colAct = GetExcelColumnName(columna);
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2);
                                    rango.NumberFormat = "$ #,###.00";
                                    columna++;

                                }


                                cargo = 0;

                                for (cargo = contCargos + 1; cargo <= contCargos + 5; cargo++)
                                {

                                    colAct = GetExcelColumnName(columna);
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2);
                                    rango.NumberFormat = "$ #,###.00";
                                    columna++;

                                }
                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                rango.Value2 = " - ";
                                columna++;

                                contCargos = columna + 2;
                                for (cargo = columna - 1; cargo <= contCargos; cargo++)
                                {
                                    colAct = GetExcelColumnName(columna);
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2);
                                    rango.NumberFormat = "$ #,###.00";
                                    columna++;

                                }


                                renglon++;


                                #endregion

                                #region conversion dolares a pesos
                                if (hayDolares)
                                {


                                    #region Titulo Fila
                                    //TotalesRecibosExpedidosEntity TotalConversion = new TotalesRecibosExpedidosEntity();
                                    sumaRen2 = renglon;
                                    columna = 6;
                                    renglonSumaFin = renglon - 1;
                                    r1 = renglonSumaFin2;
                                    r2 = renglon - 1;
                                    cargo = 0;
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    colAct = GetExcelColumnName(columna);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = "Conversion DLL a MN ";
                                    columna++;
                                    #endregion

                                    #region importe renta
                                    renglonSumaFin = renglon - 1;
                                    r1 = renglonSumaFin2;
                                    r2 = renglon - 1;
                                    cargo = 0;
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    colAct = GetExcelColumnName(columna);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = ListaRecibosRenta.Sum(s => s.SumaConMNImporte * s.TipoCambio);
                                    rango.NumberFormat = "$ #,##0.00";
                                    columna++;

                                    #endregion

                                    #region Cargos
                                    var cargosSuma = new List<SubtipoEntity>();
                                    if (usarSinComplementos)
                                        cargosSuma = ListTipoContAux;//.Count + 1;
                                    else
                                        cargosSuma = ListTipoCont;//.Count + 1;

                                    foreach (SubtipoEntity cargos in cargosSuma) //(cargo = 1; cargo <= contCargos; cargo++)
                                    {
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        colAct = GetExcelColumnName(columna);

                                        if (usarSinComplementos)
                                        {
                                            if (ConversionDolaresaMNCargos != null)
                                            {
                                                if (ConversionDolaresaMNCargos.Count > 0)
                                                {
                                                    rango.Value2 = ConversionDolaresaMNCargos.FindAll(CargoMn => cargos.Identificador == CargoMn.Campo20).Sum(s => s.Importe * s.TipoCambio);
                                                }
                                                rango.Value2 = 0;
                                            }
                                            rango.Value2 = 0;

                                        }
                                        else
                                        {
                                            if (ConversionDolaresaMNCargos != null)
                                            {
                                                if (ConversionDolaresaMNCargos.Count > 0)
                                                {
                                                    rango.Value2 = ConversionDolaresaMNCargos.FindAll(CargoMn => cargos.Identificador == CargoMn.Campo20).Sum(s => s.Importe * s.TipoCambio);
                                                }
                                                rango.Value2 = 0;
                                            }

                                        }

                                        rango.Font.Color = Color.Black;
                                        rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                        rango.Font.FontStyle = "Bold";
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                        //rango.Formula = string.Format("=SUM({0}{1}+{2}{3})", colAct, r1, colAct, r2);
                                        rango.NumberFormat = "$ #,###.00";
                                        columna++;

                                    }

                                    decimal CargosAMN = 0;
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    try
                                    {
                                        CargosAMN += ConversionDolaresaMNCargos.FindAll(cf => cf.MonedaContrato == "D" && string.IsNullOrEmpty(cf.Campo20) && cf.TipoDoc == "Z").Sum(s => s.Importe);
                                    }
                                    catch
                                    {
                                        CargosAMN = 0;
                                    }
                                    rango.Value2 = CargosAMN;
                                    rango.NumberFormat = "#,###.00";
                                    columna++;


                                    #endregion
                                    //rango.Value2 = TotalSaldo - contrato.SumaAbonadoMes;
                                    #region SALDOS
                                    cargo = 0;
                                    decimal SaldoInme = 0;
                                    decimal UltimosAbonos = 0;
                                    decimal Saldototal = 0;

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    colAct = GetExcelColumnName(columna);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = ListaRecibosRenta.Sum(s => s.SumaConMNImporte * s.TipoCambio);//TotalConversion.ImporteDolaresAPesos;
                                    rango.NumberFormat = "$ #,###.00";
                                    columna++;

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    colAct = GetExcelColumnName(columna);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = ListaRecibosRenta.Sum(s => s.Total * s.TipoCambio);// TotalConversion.TotalDolaresAPesos;// ango.Formula = string.Format("=SUM({0}{1}+{2}{3})", colAct, r1, colAct, r2);
                                    rango.NumberFormat = "$ #,###.00";
                                    columna++;

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    colAct = GetExcelColumnName(columna);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    if (listaSaldosConversion != null)
                                    {
                                        if (listaSaldosConversion.Count > 0)
                                        {
                                            SaldoInme = listaSaldosConversion.Sum(s => s.Saldo);
                                            rango.Value2 = SaldoInme;
                                        }
                                        else
                                            rango.Value2 = 0;
                                    }
                                    else
                                        rango.Value2 = 0; rango.NumberFormat = "$ #,###.00";
                                    columna++;

                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    colAct = GetExcelColumnName(columna);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    Saldototal = ListaRecibosRenta.Sum(s => (s.TotalIVA * s.TipoCambio));
                                    Saldototal = Saldototal + SaldoInme;
                                    rango.Value2 = Saldototal; rango.NumberFormat = "$ #,###.00";
                                    columna++;


                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    colAct = GetExcelColumnName(columna);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    if (UltiimoPagoMN != null)
                                    {
                                        if (UltiimoPagoMN.Count > 0)
                                        {

                                            try
                                            {
                                                UltimosAbonos = UltiimoPagoMN.Sum(s => s.SumaAbonadoMes);
                                                rango.Value2 = UltimosAbonos;
                                            }
                                            catch
                                            {
                                                rango.Value2 = 0;
                                            }

                                        }
                                        else
                                            rango.Value2 = 0;
                                    }
                                    else
                                        rango.Value2 = 0;
                                    // rango.Value2 = Saldototal + SaldoInme; rango.NumberFormat = "$ #,###.00";

                                    rango.NumberFormat = "$ #,###.00";
                                    columna++;
                                    hayDolares = false;

                                    #endregion
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = " - ";
                                    columna++;

                                    #region Saldos2
                                    decimal DllsAMnSaldo = Saldototal - UltimosAbonos;
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    colAct = GetExcelColumnName(columna);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    contCargos = columna + 2;
                                    rango.Value2 = DllsAMnSaldo;
                                    rango.NumberFormat = "$ #,###.00";
                                    columna++;


                                    decimal SaldoIntereses = ListIntesesDllaMN.Sum(s => s.SumaIntereses);
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    colAct = GetExcelColumnName(columna);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    rango.Value2 = SaldoIntereses + DllsAMnSaldo;
                                    rango.NumberFormat = "$ #,###.00";
                                    columna++;


                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    colAct = GetExcelColumnName(columna);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    if (ListSumatoriaSaldo != null)
                                    {
                                        rango.Value2 = ListSumatoriaSaldo.Sum(s => s.ImporteSaldo);
                                    }
                                    else
                                        rango.Value2 = 0;
                                    rango.NumberFormat = "$ #,###.00";
                                    columna++;


                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    colAct = GetExcelColumnName(columna);
                                    rango.Font.Color = Color.Black;
                                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                    rango.Font.FontStyle = "Bold";
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                    if (LisNCenDll != null)
                                    {
                                        if (LisNCenDll.Count > 0)
                                            rango.Value2 = LisNCenDll.Sum(Nc => Nc.Pago * Nc.TipoCambioPago);
                                        else
                                            rango.Value2 = 0;
                                    }
                                    else
                                        rango.Value2 = 0;
                                    rango.NumberFormat = "$ #,###.00";
                                    columna++;



                                    renglon++;
                                    #endregion
                                }
                                #endregion
                            }
                        }
                    }//end FOR REPETIR 
                    #endregion

                    #region SUMA TOTAL FINAL
                    rango = (hojaExcel.Cells[renglon, 1] as Excel.Range);
                    rango.Font.Size = 12;
                    rango.RowHeight = 39;
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    rango.Font.Bold = true;
                    rango.Value2 = "TOTALES FINALES";
                    rango = hojaExcel.get_Range("A" + renglon + ":" + GetExcelColumnName(cols - 1) + renglon);
                    rango.Select();
                    rango.MergeCells = true;
                    renglon++;

                    columna = 7;
                    renglonSumaFin = renglon - 1;
                    r1 = sumaRen1;
                    r2 = sumaRen2;
                    cargo = 0;
                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    colAct = GetExcelColumnName(columna);
                    rango.Font.Color = Color.Black;
                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                    rango.Font.FontStyle = "Bold";
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    if (sumaRen3 != 0)
                        rango.Formula = string.Format("=SUM({0}{1}+{2}{3}+{4}{5})", colAct, r1, colAct, r2, colAct, sumaRen3).ToString();
                    else if (sumaRen2 != 0)
                        rango.Formula = string.Format("=SUM({0}{1}+{2}{3})", colAct, r1, colAct, r2).ToString();
                    else
                        rango.Formula = string.Format("={0}{1}", colAct, r1).ToString();
                    rango.NumberFormat = "$ #,##0.00";
                    columna++;

                    if (usarSinComplementos)
                        contCargos = ListTipoContAux.Count + 1;
                    else
                        contCargos = ListTipoCont.Count + 1;

                    for (cargo = 1; cargo <= contCargos; cargo++)
                    {
                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        colAct = GetExcelColumnName(columna);
                        rango.Font.Color = Color.Black;
                        rango.Interior.Color = Color.FromArgb(208, 223, 187);
                        rango.Font.FontStyle = "Bold";
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Borders.Color = Color.FromArgb(211, 211, 211);
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        if (sumaRen3 != 0)
                            rango.Formula = string.Format("=SUM({0}{1}+{2}{3}+{4}{5})", colAct, r1, colAct, r2, colAct, sumaRen3).ToString();
                        else if (sumaRen2 != 0)
                            rango.Formula = string.Format("=SUM({0}{1}+{2}{3})", colAct, r1, colAct, r2).ToString();
                        else
                            rango.Formula = string.Format("={0}{1}", colAct, r1, colAct, sumaRen1).ToString();
                        rango.NumberFormat = "$ #,###.00";

                        columna++;

                    }

                    cargo = 0;

                    for (cargo = contCargos + 1; cargo <= contCargos + 5; cargo++)
                    {
                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        colAct = GetExcelColumnName(columna);
                        rango.Font.Color = Color.Black;
                        rango.Interior.Color = Color.FromArgb(208, 223, 187);
                        rango.Font.FontStyle = "Bold";
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Borders.Color = Color.FromArgb(211, 211, 211);
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        if (sumaRen3 != 0)
                            rango.Formula = string.Format("=SUM({0}{1}+{2}{3}+{4}{5})", colAct, r1, colAct, r2, colAct, sumaRen3).ToString();
                        else if (sumaRen2 != 0)
                            rango.Formula = string.Format("=SUM({0}{1}+{2}{3})", colAct, r1, colAct, r2).ToString();
                        else
                            rango.Formula = string.Format("={0}{1}", colAct, r1, colAct, sumaRen1).ToString();
                        rango.NumberFormat = "$ #,###.00";
                        columna++;

                    }
                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                    rango.Value2 = " - ";
                    columna++;

                    contCargos = columna + 2;
                    for (cargo = columna - 1; cargo <= contCargos; cargo++)
                    {
                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        colAct = GetExcelColumnName(columna);
                        rango.Font.Color = Color.Black;
                        rango.Interior.Color = Color.FromArgb(208, 223, 187);
                        rango.Font.FontStyle = "Bold";
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Borders.Color = Color.FromArgb(211, 211, 211);
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        if (sumaRen3 != 0)
                            rango.Formula = string.Format("=SUM({0}{1}+{2}{3}+{4}{5})", colAct, r1, colAct, r2, colAct, sumaRen3).ToString();
                        else if (sumaRen2 != 0)
                            rango.Formula = string.Format("=SUM({0}{1}+{2}{3})", colAct, r1, colAct, r2).ToString();
                        else
                            rango.Formula = string.Format("={0}{1}", colAct, r1, colAct, sumaRen1).ToString();
                        columna++;

                    }
                    renglon++;
                    #endregion

                    bgWorker.ReportProgress(90);
                    if (bgWorker.CancellationPending)
                        return "Proceso cancelado por el usuario";

                    if (!esPDF)
                    {
                        libroExcel.SaveAs(filename, Excel.XlFileFormat.xlWorkbookDefault);
                        libroExcel.CheckCompatibility = false;
                        libroExcel.Close(true);
                        aplicacionExcel.Quit();
                        releaseObject(hojaExcel);
                        releaseObject(libroExcel);
                        releaseObject(aplicacionExcel);
                    }
                    else
                    {
                        int colPdf = ListTipoContAux.Count + 24;//El 24 corresponde al numero de colunas que no corresponden a cargos.
                        hojaExcel.PageSetup.PaperSize = XlPaperSize.xlPaperLegal;
                        if (colPdf <= 35)
                            hojaExcel.PageSetup.Zoom = 32;
                        else if (colPdf <= 50)
                            hojaExcel.PageSetup.Zoom = 20;
                        else
                            hojaExcel.PageSetup.Zoom = 15;

                        #region GUARDAR Y ABRIR ARCHIVO
                        centrar = ListTipoCont.Count + 22;
                        try
                        {
                            hojaExcel.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
                            filename = filename.Replace(".xlsx", ".pdf");

                            libroExcel.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, filename, XlFixedFormatQuality.xlQualityStandard, true, false, Type.Missing, Type.Missing, false, Type.Missing);
                            libroExcel.CheckCompatibility = false;
                            libroExcel.Close(false);
                            aplicacionExcel.Quit();
                            releaseObject(hojaExcel);
                            releaseObject(libroExcel);
                            releaseObject(aplicacionExcel);
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


                    try
                    {
                        // if (System.IO.File.Exists(filename))
                        System.Diagnostics.Process.Start(filename);
                    }
                    catch
                    {
                        return "No se pudo abrir el archivo" + filename;
                    }
                    #endregion GUARDAR Y ABRIR ARCHIVO

                    bgWorker.ReportProgress(98);
                    return string.Empty;
                }
                else
                    return "No se encontraron contratos de renta con los parametros dados.";
            }//TRy
            catch (Exception ex)
            {
                return "Error general al tratar de crear reporte, intente nuevamente si la falla persiste comuniquese con el equipo de Soporte SAARI®" + Environment.NewLine
                    + "Excepcion : " + ex;
            }//Catcho 
        }

        private ReciboEntity ConvertirDolaresAPesos(ReciboEntity Recibo, string monedaContrato)
        {
            ReciboEntity conversion = Recibo;
            conversion.Importe = Recibo.Importe * Recibo.TipoCambio;
            conversion.Total = Recibo.Total * Recibo.TipoCambio;
            conversion.IVA = Recibo.IVA * Recibo.TipoCambio;
            conversion.IVARetenido = Recibo.IVARetenido * Recibo.TipoCambio;
            conversion.ISR = Recibo.ISR * Recibo.TipoCambio;
            conversion.Descuento = Recibo.Descuento * Recibo.TipoCambio;
            conversion.Cargo = Recibo.Cargo * Recibo.TipoCambio;
            return conversion;

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
                columnNumber = columnNumber * 26 + (Convert.ToInt32(ColumnName[i]) - 64);
            }
            return columnNumber;
        }
    }
}
