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
    class ReporteGlobal
    {
        string InmobiliariaID = string.Empty;
        string ConjuntoID = string.Empty;
        string InmobiliariaNombre = string.Empty;
        DateTime FechaCorte = DateTime.Now;
        bool esPDF = false;
        DateTime fechaInicioPer = DateTime.Now;
        DateTime fechaFinPer = DateTime.Now;
        bool esDolar = false;
        BackgroundWorker bgWorker;

        public ReporteGlobal(string idInmo, string nombreInmom, string idConjunto,DateTime fechaCorte, bool espdf, BackgroundWorker bk){
            this.InmobiliariaID = idInmo;
            this.InmobiliariaNombre = nombreInmom;
            this.ConjuntoID = idConjunto;
            this.FechaCorte = fechaCorte;
            this.esPDF = espdf;
            this.bgWorker = bk;
        }


        public static List<InmobiliariaEntity> obtenerInmobiliarias() {

            return SaariDB.getListaInmobiliarias();
        }

        public static List<ConjuntoEntity> obtenerConjunto(string IdInmobiliaria)
        {
            return SaariDB.getConjuntos(IdInmobiliaria);
        }

        public string generarReporte() {
            try
            {
                int renglonSumaFin = 0;
                int renglonSumaFin2 = 0;
                int porcentaje = 0;
                int totalContratros = 0;
                int conContrato = 1;
                #region OBTENCION DE DATOS
                int columna = 1, renglon = 1;
                int i = 0;
                int r1 = 0;
                int r2 = 0;
                int cargo = 0;
                int centrar = 0;
                string colAct = string.Empty;
                int contCargos = 0;
                Color colorCelda;
                //List<EdificioEntity> ListEdificios = SaariDB.getInmuebleByIDArrAndIdConjunto(InmobiliariaID, ConjuntoID);       
                var ListaContratos = SaariDB.getContratosPorInmobiliariayConjunto(InmobiliariaID, ConjuntoID);
                totalContratros = ListaContratos.Count;                
                List<SubtipoEntity> ListTipoCont = SaariDB.getSubTiposOI();
                #endregion OBTENCION DE DATOS

                bgWorker.ReportProgress(5);
                if (bgWorker.CancellationPending)
                    return "Proceso cancelado por el usuario";
                #region CONFIGURACION REPORTE
                int dia = 1;
                    fechaInicioPer = new DateTime(FechaCorte.Year, FechaCorte.Month, dia);
                    dia = DateTime.DaysInMonth(FechaCorte.Year, FechaCorte.Month);
                    fechaFinPer = new DateTime(FechaCorte.Year, FechaCorte.Month, dia);


                    string path = Properties.Settings.Default.RutaRepositorio.EndsWith(@"\") ? Properties.Settings.Default.RutaRepositorio + @"ReporteGlobal\" : Properties.Settings.Default.RutaRepositorio + @"\ReporteGlobal\";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string filename = path + "Analisis de Deudores_" + InmobiliariaNombre + "_" + FechaCorte.Day + FechaCorte.Month + FechaCorte.Year + FechaCorte.Second + ".xlsx";
                    Excel.Application aplicacionExcel = new Excel.Application();
                    Excel.Workbook libroExcel = aplicacionExcel.Workbooks.Add();
                    Excel.Worksheet hojaExcel = (libroExcel.ActiveSheet as Excel.Worksheet);
                    Excel.Range rango;// hojaExcel.get_Range("A1:" + GetExcelColumnName(Meses.Count + 3) + "1");
                    //aplicacionExcel.Visible = true;


                    #endregion CONFIGURACION REPORTE

                #region ENCABEZADOS
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
                    rango.RowHeight = 39;
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    rango.Value2 = "ANALISIS DEUDORES " + InmobiliariaNombre;
                    renglon++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    //rango.Interior.Color = Color.FromArgb(31, 98, 145);
                    rango.Font.Color = Color.FromArgb(0, 0, 0);
                    rango.Font.Size = 12;
                    rango.RowHeight = 39;
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    rango.Value2 = "FECHA DE CORTE " + FechaCorte.Day + "/" + MonthName(FechaCorte.Month).ToUpper() + "/" + FechaCorte.Year;
                    renglon++;
                    columna = 1;
                    #endregion ENCABEZADO TITULO

                    #region ENCABEZADO CARGOS
                    string[] encabezadoArray = { "No. Local", "M2", "Costo x M2", "Razón Social", "Giro", "Nombre Comercial", "Importe Renta S/IVA" };


                    foreach (string enc in encabezadoArray)
                    {
                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        rango.Interior.Color = Color.FromArgb(47, 117, 181);
                        rango.Font.Color = Color.FromArgb(255, 255, 255);
                        rango.Font.Size = 12;
                        rango.Font.FontStyle = "Bold";
                        rango.RowHeight = 107;
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        rango.Value2 = enc;
                        rango.WrapText = true;
                        columna++;
                    }

                    try
                    {
                        if (ListTipoCont != null)
                        {
                            if (ListTipoCont.Count > 0)
                            {
                                foreach (SubtipoEntity sub in ListTipoCont)
                                {
                                    int contHeigth = 1;
                                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                    rango.Interior.Color = Color.FromArgb(47, 117, 181);
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
                                }
                            }
                        }

                    }
                    catch
                    {
                        return "Error encabezado de conceptos de mantenimeinto";
                    }

                    string[] encabezadoArray2 = { "Total Facturado S/IVA DEL PERIODO", "Total Facturado C/IVA DEL PERIODO", "Saldo Inmediato Mes Anterior",
                                                "Saldo Total Mes Corriente + Saldo Inmediato Mes Anterior", "Cantidad Ultimo Abono", "Fecha Ultimo Abono",
                                                "Saldo Total", "Saldo a Favor", "Nota de Credito", "Mes de Adeudo", "Fecha Inicio de Contrato", "Fecha Terminación de Contrato",
                                            "Deposito en Garantía","Periodo de Gracia", "Fecha Inicio de Facturación"};
                    foreach (string enc2 in encabezadoArray2)
                    {
                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        rango.Interior.Color = Color.FromArgb(47, 117, 181);
                        rango.Font.Color = Color.FromArgb(255, 255, 255);
                        rango.Font.Size = 12;
                        rango.Font.FontStyle = "Bold";
                        rango.RowHeight = 107;
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        rango.Value2 = enc2;
                        rango.WrapText = true;
                        columna++;
                    }

                    renglon++;
                    hojaExcel.get_Range("A3:A3").ColumnWidth = 20;
                    hojaExcel.get_Range("B3:C3").ColumnWidth = 12;
                    hojaExcel.get_Range("D3:F3").ColumnWidth = 30;
                    #endregion

                    if (bgWorker.CancellationPending)
                        return "Proceso cancelado por el usuario";
                }
                catch
                {
                    return "Error al poner emcabezados.";
                }
                #endregion ENCABEZADOS

                #region LLENAR DATOS DE REPORTE
                //EMPIEZA RENGLON 3, COLUMAN EMPIZA EN 1
                //renglon = 4;
                List<SaldoAFavorEntity> ListSumatoriaSaldo = SaariE.getSumatoriaSaldoAFavor();
                ListaContratos.OrderBy(L => L.IdEdificio).ToList();
                

                bgWorker.ReportProgress(15);
                if (bgWorker.CancellationPending)
                    return "Proceso cancelado por el usuario";
                if (ListaContratos != null)
                {
                    if (ListaContratos.Count > 0)
                    {
                        foreach (var contrato in ListaContratos)
                        {
                          
                            decimal div = (Convert.ToDecimal(conContrato) / Convert.ToDecimal(totalContratros)) * Convert.ToDecimal(80);
                            porcentaje = Convert.ToInt32(div) + 15;
                            bgWorker.ReportProgress(Convert.ToInt32(div));
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
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = contrato.IdEdificio + " - " + contrato.NombreInmueble;
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;

                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = Math.Round(contrato.M2Construccion);
                            rango.NumberFormat = "#,###.##";
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;

                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = contrato.CostoM2;
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;

                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = contrato.NombreCliente;
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;

                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = contrato.Actividad;
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;

                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = contrato.NombreComercial;
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;

                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = contrato.RentaActual;
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;
                            #endregion

                            #region CARGOS
                            try
                            {
                                if (ListTipoCont != null)
                                {
                                    if (ListTipoCont.Count > 0)
                                    {
                                        foreach (SubtipoEntity CargosContrato in ListTipoCont)
                                        {
                                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                            rango.Value2 = SaariDB.getSumaPorCargo(contrato.IdContrato, CargosContrato.Identificador)[0].suma;
                                            rango.NumberFormat = "";
                                            rango.Font.Color = Color.Black;
                                            rango.Interior.Color = colorCelda;
                                            columna++;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                return "Error al tratar de realizar la suma por conceptos de mantenimiento";
                            }
                            #endregion

                            #region FACTURADO C/S IVA
                            ContratosEntity objIDConjunto = new ContratosEntity();
                            objIDConjunto.IDArrendadora = ListaContratos[i].idInmo;
                            objIDConjunto.IDContrato = ListaContratos[i].IdContrato;
                            objIDConjunto.IDCliente = ListaContratos[i].IdCliente;

                            ReciboEntity recEnt = new ReciboEntity();
                           // List<ReciboEntity> ListRecibos = SaariDB.getRecibosExpedidosPorFolioByContrato(objIDConjunto.IDArrendadora, objIDConjunto.IDContrato, fechaInicioPer, fechaFinPer);
                            List<ReciboEntity> ListRecibos = SaariDB.getListaRecibos(objIDConjunto, fechaInicioPer, fechaFinPer);
                            try
                            {
                                if (ListRecibos != null)
                                {
                                    if (ListRecibos.Count > 0)
                                    {
                                        foreach (ReciboEntity li in ListRecibos)
                                        {
                                            recEnt.SumaConMNImporte += li.Importe;
                                            recEnt.SumaConMNIVA += li.IVA;
                                            recEnt.TotalIVA += li.Importe + li.IVA;
                                            //recEnt.TotalIVA += recEnt.SumaConMNImporte + recEnt.SumaConMNIVA;

                                        }
                                    }
                                }
                                // public static List<ReciboEntity> getListaRecibos(ClienteEntity cliente, DateTime fechaCorte) toma todo lo facturado en la historia del cliente
                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                rango.Value2 = recEnt.SumaConMNImporte;
                                rango.Font.Color = Color.Black;
                                rango.Interior.Color = colorCelda;
                                columna++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                rango.Value2 = recEnt.TotalIVA;
                                rango.Font.Color = Color.Black;
                                rango.Interior.Color = colorCelda;
                                columna++;
                            }
                            catch
                            {
                                return "Error al calcular lo facturado en el mes corriente";
                            }
                            #endregion

                            #region SALDO INMEDIATO MES ANTERIOR
                            ClienteEntity objClienteEntity = new ClienteEntity();
                            objClienteEntity.IDCliente = objIDConjunto.IDCliente;
                            objClienteEntity.IDContrato = contrato.IdContrato;
                            ContratosEntity objContratos = new ContratosEntity();
                            objContratos.IDContrato = contrato.IdContrato;
                            objContratos.IDCliente = contrato.IdCliente;
                            objContratos.IDArrendadora = InmobiliariaID;
                            //TO DO: SE PUEDE OPTIMIZAR REALIZANDO LA SUMATORIA DESDE EL QUERY.
                            List<SaldoEntity> listaSaldosBD = SaariDB.getSaldos(objContratos, fechaInicioPer, fechaFinPer);//
                            List<SaldosEntity> listaSaldos = new List<SaldosEntity>();
                            decimal saldoTotal = 0;
                            try
                            {
                                
                                    foreach (SaldoEntity s in listaSaldosBD)
                                    {
                                        saldoTotal += s.PagoParcial;
                                    }
                                    SaldosEntity saldos = new SaldosEntity()
                                    {
                                        Saldo = saldoTotal - SaariDB.getSaldoAnterior(objContratos, fechaInicioPer)
                                    };
                                    
                                    listaSaldos.Add(saldos);                               
                           
                           

                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = listaSaldos[0].Saldo;
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;
                            }
                            catch
                            {
                                return "Error al calcular saldo inmediado mes anterior";
                            }

                            #endregion

                            #region Saldo Total Mes Corriente + Saldo Inmediato Mes Anterior

                            decimal saldoTotalMesCorriente = 0;

                            // List<ReciboEntity> ListRecibosStatus1 = ListRecibos.Where(r => r.Estatus == "1").ToList();
                           // List<ReciboEntity> ListInicioFac = SaariDB.getListaRecibosPendientesPago(objClienteEntity, FechaCorte);
                            List<ReciboEntity> ListaSaldo = SaariDB.getListaRecibosPendientesPagoContrato(objClienteEntity, FechaCorte);
                            ReciboEntity LRS = new ReciboEntity();
                           // ListRecibos
                            List<ReciboEntity> ListRecibosSaldoMesCorriente = ListRecibos.FindAll(x=> x.Estatus == "1" && x.FechaEmision>= fechaInicioPer && x.FechaEmision <= fechaFinPer);
                            if (ListRecibosSaldoMesCorriente != null)
                            {
                                if (ListRecibosSaldoMesCorriente.Count > 0)
                                {
                                    foreach (ReciboEntity li in ListRecibosSaldoMesCorriente)
                                    {
                                        LRS.TotalIVA += li.PagoParcial;
                                    }
                                }
                            }
                            if (ListaSaldo != null)
                            {
                                if (ListaSaldo.Count > 0)
                                {
                                    foreach (ReciboEntity li in ListaSaldo)
                                    {
                                        LRS.TotalIVA += li.Pago;
                                    }
                                }
                            }
                            //listaSaldosBD = listaSaldosBD.Where(s => s.IDInmobiliaria == idArrendadora).ToList();
                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = saldoTotalMesCorriente = LRS.TotalIVA + listaSaldos[0].Saldo;// +listaSaldos[0].Saldo;
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;

                            #endregion

                            #region ULTIMO ABONO
                            List<ReciboEntity> ListRecibosUltimoAbono = SaariDB.UltimoAbono(InmobiliariaID, contrato.IdContrato);
                            try
                            {
                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                if (ListRecibosUltimoAbono != null)
                                {
                                    if (ListRecibosUltimoAbono.Count != 0)
                                        rango.Value2 = ListRecibosUltimoAbono.LastOrDefault().Pago;//saldoTotalMesCorriente = LRS.TotalIVA + listaSaldos[0].Saldo;
                                    else
                                        rango.Value2 = 0;
                                }
                                rango.Font.Color = Color.Black;
                                rango.Interior.Color = colorCelda;
                                columna++;



                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                if (ListRecibosUltimoAbono != null)
                                {
                                    if (ListRecibosUltimoAbono.Count != 0)//this.formatRange.NumberFormat = "m/d/yyyy";
                                    {
                                        rango.Value2 = ListRecibosUltimoAbono.LastOrDefault().FechaPago;//saldoTotalMesCorriente = LRS.TotalIVA + listaSaldos[0].Saldo;
                                        rango.NumberFormat = "dd/mm/yyyy";
                                    }
                                    else
                                        rango.Value2 = "-";
                                }
                                rango.Font.Color = Color.Black;
                                rango.Interior.Color = colorCelda;
                                columna++;
                            }
                            catch
                            {
                                return "Error al asignar ultimo abono";
                            }

                            #endregion
                            if (bgWorker.CancellationPending)
                                return "Proceso cancelado por el usuario";
                            #region Saldo total
                            try
                            {
                                decimal saldoTotalt = 0;
                                if (ListaSaldo != null)
                                {
                                    if (ListaSaldo.Count > 0)
                                    {
                                        foreach (ReciboEntity LST in ListaSaldo)
                                        {
                                            saldoTotalt += LST.PagoParcial;
                                        }
                                    }
                                }
                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                rango.Value2 = LRS.TotalIVA + listaSaldos[0].Saldo;
                                rango.Font.Color = Color.Black;
                                rango.Interior.Color = colorCelda;
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
                                SaldoFavor = ListSumatoriaSaldo.Find(c => c.IdCliente == contrato.IdCliente).SumaSaldoFavor;
                            }
                            catch
                            {
                                SaldoFavor = 0;
                            }
                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = SaldoFavor;
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;

                            #endregion

                            #region NOTA DE CREDITO
                            try
                            {
                                List<ReciboEntity> ListaNC = SaariDB.getNotasCreditoByIdCliente(objIDConjunto.IDCliente, FechaCorte);
                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                if (ListaNC != null)
                                {
                                    if (ListaNC.Count != 0)//this.formatRange.NumberFormat = "m/d/yyyy";
                                    {
                                        rango.Value2 = ListaNC.LastOrDefault().Pago;//saldoTotalMesCorriente = LRS.TotalIVA + listaSaldos[0].Saldo;
                                        rango.NumberFormat = "###,##";
                                    }
                                }
                                else
                                    rango.Value2 = "-";
                                rango.Font.Color = Color.Black;
                                rango.Interior.Color = colorCelda;
                                columna++;
                            }
                            catch
                            {
                                return "Error al tomar la nota de credito";
                            }


                            #endregion

                            #region MESES ADEUDO
                            try
                            {
                                List<Fechas> ListMesesAdeudo = SaariDB.getMesesAdeudoByCliente(objContratos.IDContrato, FechaCorte);
                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                rango.Borders.Color = Color.FromArgb(211, 211, 211);
                                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                if (ListMesesAdeudo != null)
                                {
                                    if (ListMesesAdeudo.Count > 0)//this.formatRange.NumberFormat = "m/d/yyyy";
                                    {
                                        rango.Value2 = ListMesesAdeudo.Count;//saldoTotalMesCorriente = LRS.TotalIVA + listaSaldos[0].Saldo;

                                    }
                                    else
                                        rango.Value2 = "0";
                                }
                                else
                                    rango.Value2 = "0";
                                rango.Font.Color = Color.Black;
                                rango.Interior.Color = colorCelda;
                                columna++;
                            }
                            catch
                            {
                                return "Error al seleccionar los meses de adeudo";
                            }

                            #endregion

                            #region DATOS CONRTATO 2

                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = contrato.InicioVigencia;
                            rango.NumberFormat = "dd/mm/yyyy";
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;

                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = contrato.FinVigencia;
                            rango.NumberFormat = "dd/mm/yyyy";
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;

                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = contrato.Deposito;
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;


                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = contrato.DiasGracia;
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;

                            //getListaRecibos
                            List<ReciboEntity> ListInicioFact = SaariDB.getListaRecibos(objClienteEntity, FechaCorte);
                            DateTime inicioFact = DateTime.Now;
                            if (ListInicioFact != null)
                            {
                                if (ListInicioFact.Count > 0)
                                {
                                    inicioFact = ListInicioFact.FirstOrDefault().FechaEmision;
                                }
                                
                            }
                            

                            rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.Borders.Color = Color.FromArgb(211, 211, 211);
                            rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                            rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            rango.Value2 = inicioFact;
                            rango.NumberFormat = "dd/mm/yyyy";
                            rango.Font.Color = Color.Black;
                            rango.Interior.Color = colorCelda;
                            columna++;


                            #endregion

                            renglon++;
                            i++;
                            conContrato++;

                        }


                #endregion LLENAR DATOS DE REPORTE
                        if (bgWorker.CancellationPending)
                            return "Proceso cancelado por el usuario";
                        #region SUMATORIA CONTRATOS
                        // siempre empieza en 7 la sumatoria
                        columna = 7;
                        renglonSumaFin = renglon - 1;
                        r1 = 4;
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
                        rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2).ToString();
                        rango.NumberFormat = "$ #,##0.00";
                        columna++;

                        contCargos = ListTipoCont.Count;
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
                            rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2);
                            rango.NumberFormat = "$ #,###.00";

                            columna++;

                        }
                        contCargos = 7 + contCargos + 5;
                        cargo = 0;
                        //contCargos =  
                        for (cargo = ListTipoCont.Count + 7; cargo <= contCargos + 8; cargo++)
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
                            rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2);
                            rango.NumberFormat = "$ #,###.00";
                            columna++;
                            if (cargo == contCargos - 1)
                            {
                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                rango.Formula = "-";
                                columna++;
                            }
                            else if (cargo > contCargos)
                            {
                                for (int x = 1; x <= 7; x++)
                                {
                                    if (x != 5)
                                    {
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                        rango.Formula = "-";
                                        columna++;
                                    }
                                    else
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
                                        rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2);
                                        rango.NumberFormat = "$ #,###.00";
                                        columna++;
                                    }
                                }
                                break;
                            }
                        }
                        renglon++;
                        #endregion

                        bgWorker.ReportProgress(90);

                        #region FACTURAS LIBRES
                        columna = 1;
                        #region ENCABEZADO TITULO
                        centrar = ListTipoCont.Count + 22;

                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        //rango.Interior.Color = Color.FromArgb(31, 98, 145);
                        rango.Font.Color = Color.FromArgb(0, 0, 0);
                        rango.Font.FontStyle = "Bold";
                        rango.Font.Size = 14;
                        rango.RowHeight = 30;
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        rango.Value2 = "FACTURAS POR CONCEPTO LIBRE";
                        rango = hojaExcel.get_Range("A" + renglon + ":" + GetExcelColumnName(centrar) + renglon);
                        //rango = hojaExcel.get_Range("A" + renglon + ":H" + renglon);
                        rango.Select();
                        rango.MergeCells = true;
                        renglon++;
                    }
                }
                #endregion ENCABEZADO TITULO
                if (bgWorker.CancellationPending)
                    return "Proceso cancelado por el usuario";
                #region DATOS FACTURA LIBRE
                List<ReciboEntity> ListaFacturasConceptoLibre = SaariDB.getFacturaslibresSinPagar(InmobiliariaID);

                foreach (ReciboEntity LFCL in ListaFacturasConceptoLibre)
                {
                    columna = 1;
                    if ((renglon % 2) == 0)
                    {
                        colorCelda = Color.FromArgb(255, 255, 255);
                    }
                    else
                    {
                        colorCelda = Color.FromArgb(225, 237, 255);
                    }
                    int y = 0;
                    for (y = 1; y <= 3; y++)
                    {
                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Borders.Color = Color.FromArgb(211, 211, 211);
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        rango.Value2 = "-";
                        rango.Font.Color = Color.Black;
                        rango.Interior.Color = colorCelda;
                        columna++;
                    }
                    y = 0;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    rango.Value2 = LFCL.NombreCliente;
                    rango.Font.Color = Color.Black;
                    rango.Interior.Color = colorCelda;
                    columna++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    rango.Value2 = "-";
                    rango.Font.Color = Color.Black;
                    rango.Interior.Color = colorCelda;
                    columna++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    rango.Value2 = LFCL.nombreComercial;
                    rango.Font.Color = Color.Black;
                    rango.Interior.Color = colorCelda;
                    columna++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    rango.Value2 = "-";
                    rango.Font.Color = Color.Black;
                    rango.Interior.Color = colorCelda;
                    columna++;

                    contCargos = ListTipoCont.Count + 7;
                    for (cargo = columna; cargo <= contCargos; cargo++)
                    {
                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Borders.Color = Color.FromArgb(211, 211, 211);
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        rango.Value2 = "-";
                        rango.Font.Color = Color.Black;
                        rango.Interior.Color = colorCelda;
                        columna++;

                    }

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    rango.Value2 = LFCL.Importe;
                    rango.Font.Color = Color.Black;
                    rango.Interior.Color = colorCelda;
                    columna++;


                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    rango.Value2 = LFCL.IVA;
                    rango.Font.Color = Color.Black;
                    rango.Interior.Color = colorCelda;
                    columna++;

                    for (y = 1; y <= 4; y++)
                    {
                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Borders.Color = Color.FromArgb(211, 211, 211);
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        rango.Value2 = "-";
                        rango.Font.Color = Color.Black;
                        rango.Interior.Color = colorCelda;
                        columna++;
                    }
                    y = 0;


                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    rango.Value2 = LFCL.Total;
                    rango.Font.Color = Color.Black;
                    rango.Interior.Color = colorCelda;
                    columna++;
                    for (y = 1; y <= 8; y++)
                    {
                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Borders.Color = Color.FromArgb(211, 211, 211);
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        rango.Value2 = "-";
                        rango.Font.Color = Color.Black;
                        rango.Interior.Color = colorCelda;
                        columna++;
                    }
                    y = 0;

                    renglon++;
                }
                #endregion

                if (bgWorker.CancellationPending)
                    return "Proceso cancelado por el usuario";
                #region SUMA FACTURA LIBRE

                // siempre empieza en 7 la sumatoria
                columna = 7;
                renglonSumaFin2 = renglon - 1;
                r1 = renglon - 1;
                r2 = renglon - 1;
                //int cargo = 0;
                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                colAct = GetExcelColumnName(columna);
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

                contCargos = ListTipoCont.Count;
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
                    rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2);
                    rango.NumberFormat = "$ #,###.00";

                    columna++;

                }
                contCargos = 7 + contCargos + 5;
                cargo = 0;
                //contCargos =  
                for (cargo = ListTipoCont.Count + 7; cargo <= contCargos + 8; cargo++)
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
                    rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2);
                    rango.NumberFormat = "$ #,###.00";
                    columna++;
                    if (cargo == contCargos - 1)
                    {
                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        rango.Interior.Color = Color.FromArgb(208, 223, 187);
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        rango.Formula = "-";
                        columna++;
                    }
                    else if (cargo > contCargos)
                    {
                        for (int x = 1; x <= 7; x++)
                        {
                            if (x != 5)
                            {
                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Interior.Color = Color.FromArgb(208, 223, 187);
                                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                rango.Formula = "-";
                                columna++;
                            }
                            else
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
                                rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2);
                                rango.NumberFormat = "$ #,###.00";
                                columna++;
                            }
                        }
                        break;
                    }
                }
                renglon++;



                #endregion

                if (bgWorker.CancellationPending)
                    return "Proceso cancelado por el usuario";
                bgWorker.ReportProgress(95);
                renglon++;
                columna = 7;
                #endregion
                contCargos = ListTipoCont.Count +14;
                    #region ENCABEZADOS SUMA FINAL
                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                colAct = GetExcelColumnName(columna);
                rango.Font.Color = Color.White;
                rango.Interior.Color = Color.FromArgb(47, 117, 181);
                rango.Font.FontStyle = "Bold";
                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                rango.Borders.Color = Color.FromArgb(211, 211, 211);
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                rango.Value2 = "TOTALES FINALES";
                //rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2);
                rango.NumberFormat = "$ #,###.00";
                //scolumna++;
                centrar = 0;
                centrar = 7 + ListTipoCont.Count + 7;
                rango = hojaExcel.get_Range("G"+renglon+":"+GetExcelColumnName(centrar)+renglon);
                rango.Select();
                rango.MergeCells = true;


                renglon++;
                i = 0;
                contCargos = 0 ;
                
                string[] encabezadoSuma = {"Total Facturado S/IVA DEL PERIODO", "Total Facturado C/IVA DEL PERIODO",
                                          "Saldo Inmediato Mes Anterior","Saldo Total Mes Corriente + Saldo Inmediato Mes Anterior","Cantidad Ultimo Abono",
                                          "Saldo Total","Saldo a Favor"};
                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        colAct = GetExcelColumnName(columna);
                        rango.Font.Color = Color.White;
                        rango.Interior.Color = Color.FromArgb(47, 117, 181);
                        rango.Font.FontStyle = "Bold";
                        rango.Font.Size = 12;
                        rango.RowHeight = 46.50;
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Borders.Color = Color.FromArgb(211, 211, 211);
                        rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        rango.Value2 = "Importe Renta S/IVA";
                        columna++;
                string valor = string.Empty;
                foreach (SubtipoEntity t2 in ListTipoCont)
                {
                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    colAct = GetExcelColumnName(columna);
                    rango.Font.Color = Color.White;
                    rango.Interior.Color = Color.FromArgb(47, 117, 181);
                    rango.Font.FontStyle = "Bold";
                    rango.Font.Size = 12;
                    rango.RowHeight = 46.50;
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    rango.Value2 = t2.Nombre;
                  //  rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2);
                    rango.NumberFormat = "$ #,###.00";
                    columna++;

                }
                
                foreach (string t in encabezadoSuma)
                {
                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    colAct = GetExcelColumnName(columna);
                    rango.Font.Color = Color.White;
                    rango.Interior.Color = Color.FromArgb(47, 117, 181);
                    rango.Font.FontStyle = "Bold";
                    rango.Font.Size = 12;
                    rango.RowHeight = 46.50;
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    rango.Value2 = t;
                   // rango.Formula = string.Format("=SUM({0}{1}:{2}{3})", colAct, r1, colAct, r2);
                    rango.NumberFormat = "$ #,###.00";
                    columna++;
                }
                #endregion ENCABEZADO SUMA FINAL
                    renglon++;    
                    columna = 7;
                    r1=renglonSumaFin + 1;
                    r2=renglonSumaFin2 + 1;
                    #region SUMA TOTAL FINAL
                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    colAct = GetExcelColumnName(columna);
                    rango.Font.Color = Color.Black;
                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                    rango.Font.FontStyle = "Bold";

                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    //rango.Value2 = "TOTALES FINALES";
                    rango.Formula = string.Format("=SUM({0}{1}+{2}{3})", colAct, r1, colAct, r2);
                    rango.NumberFormat = "$ #,###.00";
                    columna++;
               
                foreach (SubtipoEntity sum in ListTipoCont)
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
                    rango.Formula = string.Format("=SUM({0}{1}+{2}{3})", colAct, r1, colAct, r2);
                    rango.NumberFormat = "$ #,###.00";
                    columna++;

                }

                i=1;
                foreach (string sum in encabezadoSuma)
                {
                    colAct = GetExcelColumnName(columna);
                    if (i == 6)
                        columna = columna - 1;
                    else if (i == 7)
                        columna = columna - 1;
                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);                    
                    rango.Font.Color = Color.Black;
                    rango.Interior.Color = Color.FromArgb(208, 223, 187);
                    rango.Font.FontStyle = "Bold";
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    rango.Borders.Color = Color.FromArgb(211, 211, 211);
                    rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    rango.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    rango.Formula = string.Format("=SUM({0}{1}+{2}{3})", colAct, r1, colAct, r2);
                    rango.NumberFormat = "$ #,###.00";

                    if (i == 5)
                    { columna++; columna++; }

                    else if (i == 6) { columna++; columna++; }
                    else { columna++; }
                    i++;

                }
                #endregion

                if (bgWorker.CancellationPending)
                    return "Proceso cancelado por el usuario";

                hojaExcel.PageSetup.PaperSize = XlPaperSize.xlPaperLegal;
                if (ListTipoCont.Count <= 6)
                    hojaExcel.PageSetup.Zoom = 40;
                else if (ListTipoCont.Count <= 12)
                    hojaExcel.PageSetup.Zoom = 20;
                else
                    hojaExcel.PageSetup.Zoom = 5;
                #region GUARDAR Y ABRIR ARCHIVO
                 centrar = ListTipoCont.Count + 22;

                rango = hojaExcel.get_Range("A1:"+GetExcelColumnName(centrar)+"1");
                rango.Select();
                rango.MergeCells = true;
                rango = hojaExcel.get_Range("A2:"+GetExcelColumnName(centrar)+"2");
                rango.Select();
                rango.MergeCells = true;

                hojaExcel.Application.ActiveWindow.SplitRow = 3;
                hojaExcel.Application.ActiveWindow.FreezePanes = true;
                //hojaExcel.get_Range("A3:AB10").Columns.AutoFit();
                if (!esPDF)
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
            catch 
            {
                return "No se general al tratar de crear el reporte";
            }
        
        
        
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
