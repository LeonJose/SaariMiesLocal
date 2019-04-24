using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.DataAccessLayer;
using FastReport;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.Data;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class RecibosCobradosPorFolio : SaariReport, IBackgroundReport
    {
        private string fileName = string.Empty;
        private string rutaFormato = string.Empty;
        //private bool generarEx = true;
        //private bool esPDF = true;
        public RecibosCobradosPorFolio(){
        
        }
        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();

        }

        public static List<ConjuntoEntity> ObtenerConjuntos(string idInmobiliaria){
            
            return SaariDB.getConjuntos(idInmobiliaria);
        }

        public string generarReporte(string idInmobiliaria, string NombreInmo, string idConjunto, DateTime fechaInicial, DateTime fechafinal, bool todosConjuntos, bool SelectConjunto, bool allConjuntosSub, bool orderBy, bool incluyeDetalle, bool esPDF, bool AbrirEx, string rutaformato, System.ComponentModel.BackgroundWorker backgroundWorker1)
        {
            string error = string.Empty;
           backgroundWorker1.ReportProgress(10);
            List<ReciboEntity> listaRecibosExpedidosxFolio = SaariDB.getRecibosCobradosxFolio(idInmobiliaria, fechaInicial, fechafinal, idConjunto, incluyeDetalle, orderBy,allConjuntosSub);
            if (fechafinal >= fechaInicial)
            {
                if (listaRecibosExpedidosxFolio != null)
                {
                    if (listaRecibosExpedidosxFolio.Count > 0)
                    {
                        backgroundWorker1.ReportProgress(15);
                        if (backgroundWorker1.CancellationPending)
                            return "Proceso cancelado por el usuario";
                        try
                        {
                            this.rutaFormato = rutaformato;
                            Report reporte = new Report();
                            reporte.Clear();
                            reporte.Load(rutaFormato);
                            DataTable dtEncabezado = new DataTable("Encabezado");
                            DataColumn dcNombreComercial = new DataColumn("NombreComercial", typeof(string));
                            DataColumn dcFechaInicial = new DataColumn("FechaInicial", typeof(DateTime));
                            DataColumn dcFechaFinal = new DataColumn("FechaFinal", typeof(DateTime));

                            dcFechaInicial.DataType = System.Type.GetType("System.DateTime");
                            dcFechaFinal.DataType = System.Type.GetType("System.DateTime");
                            dtEncabezado.Columns.Add(dcNombreComercial);
                            dtEncabezado.Columns.Add(dcFechaInicial);
                            dtEncabezado.Columns.Add(dcFechaFinal);
                            dtEncabezado.AcceptChanges();
                            if (backgroundWorker1.CancellationPending)
                                return "Proceso cancelado por el usuario";
                            backgroundWorker1.ReportProgress(25);
                            try
                            {
                                string nombreInmobiliaria = NombreInmo != null ? NombreInmo : "";
                                dtEncabezado.Rows.Add(nombreInmobiliaria, fechaInicial, fechafinal.Date);
                            }
                            catch (Exception ex)
                            {
                                return "Ocurrió un error al leer los registros. Error: " + ex.Message;
                            }
                            backgroundWorker1.ReportProgress(30);
                            DataTable dtRepRecCobradosFolio = new DataTable();
                            DataColumn dcFecha = new DataColumn("emision", typeof(DateTime));
                            DataColumn dcpago = new DataColumn("pago", typeof(DateTime));
                            DataColumn dcserie = new DataColumn("serie", typeof(string));
                            DataColumn dcFolio = new DataColumn("folio", typeof(int));
                            DataColumn dcRef = new DataColumn("ref", typeof(int));
                            DataColumn dcInmueble = new DataColumn("inmueble", typeof(string));
                            DataColumn dcCliente = new DataColumn("cliente", typeof(string));
                            DataColumn dcComent = new DataColumn("comentario", typeof(string));
                            DataColumn dcMoneda = new DataColumn("moneda", typeof(string));
                            DataColumn dcTc = new DataColumn("tc", typeof(decimal));
                            DataColumn dcImporte = new DataColumn("importe", typeof(decimal));
                            DataColumn dcIva = new DataColumn("iva", typeof(decimal));
                            DataColumn dcIsr = new DataColumn("isr", typeof(decimal));
                            DataColumn dcRetIva = new DataColumn("retIva", typeof(decimal));
                            DataColumn dctotal = new DataColumn("total", typeof(decimal));
                            DataColumn dcCImporte = new DataColumn("Cimporte", typeof(decimal));
                            DataColumn dcCIva = new DataColumn("Civa", typeof(decimal));
                            DataColumn dcCIsr = new DataColumn("Cisr", typeof(decimal));
                            DataColumn dcCRetIva = new DataColumn("CretIva", typeof(decimal));
                            DataColumn dcCtotal = new DataColumn("Ctotal", typeof(decimal));
                            DataColumn dcIdConjunto = new DataColumn("idConjunto", typeof(string));
                            DataColumn dcNameConjunto = new DataColumn("nameConjunto", typeof(string));
                            DataColumn dcNameSubConjunto = new DataColumn("nameSubConjunto", typeof(string));
                            DataColumn dcNamEsSubConjunto = new DataColumn("nameEsSubConjunto", typeof(string));
                            DataColumn dcMonedaPag = new DataColumn("monedapag", typeof(string));
                            DataColumn dcTcPag = new DataColumn("Tcpag", typeof(decimal));
                            DataColumn dcTipoPago = new DataColumn("TipoPago", typeof(string));
                            DataColumn dcTotalPagado = new DataColumn("TotalPagado", typeof(decimal));
                            DataColumn dcSaldoFavor = new DataColumn("SaldoFavor", typeof(decimal));

                            dtRepRecCobradosFolio.Columns.Add(dcFecha);
                            dtRepRecCobradosFolio.Columns.Add(dcpago);
                            dtRepRecCobradosFolio.Columns.Add(dcserie);
                            dtRepRecCobradosFolio.Columns.Add(dcFolio);
                            dtRepRecCobradosFolio.Columns.Add(dcRef);
                            dtRepRecCobradosFolio.Columns.Add(dcInmueble);
                            dtRepRecCobradosFolio.Columns.Add(dcCliente);
                            dtRepRecCobradosFolio.Columns.Add(dcComent);
                            dtRepRecCobradosFolio.Columns.Add(dcMoneda);
                            dtRepRecCobradosFolio.Columns.Add(dcTc);
                            dtRepRecCobradosFolio.Columns.Add(dcImporte);
                            dtRepRecCobradosFolio.Columns.Add(dcIva);
                            dtRepRecCobradosFolio.Columns.Add(dcIsr);
                            dtRepRecCobradosFolio.Columns.Add(dcRetIva);
                            dtRepRecCobradosFolio.Columns.Add(dctotal);
                            dtRepRecCobradosFolio.Columns.Add(dcCImporte);
                            dtRepRecCobradosFolio.Columns.Add(dcCIva);
                            dtRepRecCobradosFolio.Columns.Add(dcCIsr);
                            dtRepRecCobradosFolio.Columns.Add(dcCRetIva);
                            dtRepRecCobradosFolio.Columns.Add(dcCtotal);
                            dtRepRecCobradosFolio.Columns.Add(dcIdConjunto);
                            dtRepRecCobradosFolio.Columns.Add(dcNameConjunto);
                            dtRepRecCobradosFolio.Columns.Add(dcNameSubConjunto);
                            dtRepRecCobradosFolio.Columns.Add(dcNamEsSubConjunto);
                            dtRepRecCobradosFolio.Columns.Add(dcMonedaPag);
                            dtRepRecCobradosFolio.Columns.Add(dcTcPag);
                            dtRepRecCobradosFolio.Columns.Add(dcTipoPago);
                            dtRepRecCobradosFolio.Columns.Add(dcTotalPagado);
                            dtRepRecCobradosFolio.Columns.Add(dcSaldoFavor);
                           

                            backgroundWorker1.ReportProgress(50);
                            if (backgroundWorker1.CancellationPending)
                                return "Proceso cancelado por el usuario";
                          
                            #region reporte normal
                            foreach (var recibo in listaRecibosExpedidosxFolio)
                                {
                                    if (backgroundWorker1.CancellationPending)
                                        return "Proceso cancelado por el usuario";

                                    string idC = recibo.IDCliente;
                                    int idHRec = recibo.IDHistRec;
                                    List<ReciboEntity> listaComentarioSQL = SaariE.getComentario(idC, idHRec);
                                    try
                                    {
                                        if (listaComentarioSQL == null)
                                        {
                                            recibo.Comentario = "";
                                        }
                                        else
                                        {
                                            ReciboEntity ComentarioEnLista = new ReciboEntity();
                                            if (listaComentarioSQL.Count > 0)
                                            {
                                                ComentarioEnLista = listaComentarioSQL.Find(x => x.IDCliente == idC && x.IDHistRec == idHRec);
                                                if (string.IsNullOrEmpty(ComentarioEnLista.Comentario))
                                                {
                                                    ComentarioEnLista.Comentario = "";
                                                }

                                            }
                                            else
                                            {
                                                ComentarioEnLista.Comentario = "";
                                            }
                                            recibo.Comentario = ComentarioEnLista.Comentario;
                                        }
                                    }
                                    catch
                                    {
                                        return "Error al asignar comentario a recibo :"+ recibo.IDHistRec;
                                    }


                                    if (recibo.Moneda != "P" && recibo.Moneda != "D" || recibo.MonedaPago != "P" && recibo.MonedaPago != "D")
                                    {
                                        recibo.Moneda = "P";
                                        recibo.MonedaPago = "P";
                                    }

                                    #region
                                    if (recibo.MonedaPago == "D")
                                    {
                                        if (recibo.TipoCambio == 0)
                                        {
                                            recibo.TipoCambio = 1;
                                        }
                                        recibo.ConDllsImporte = recibo.Importe * recibo.TipoCambio;
                                        recibo.ConDllsIVA = recibo.IVA * recibo.TipoCambio;
                                        recibo.ConDllsISR = recibo.ISR * recibo.TipoCambio;
                                        recibo.ConDllsIVARetenido = recibo.IVARetenido * recibo.TipoCambio;
                                        recibo.ConDllstotal = recibo.ConDllsImporte + recibo.ConDllsIVA;
                                    }
                                    else
                                    {
                                        recibo.ConDllsImporte = recibo.Importe;
                                        recibo.ConDllsIVA = recibo.IVA;
                                        recibo.ConDllsISR = recibo.ISR;
                                        recibo.ConDllsIVARetenido = recibo.IVARetenido;
                                        recibo.ConDllstotal = recibo.ConDllsImporte + recibo.ConDllsIVA;

                                    }
                                    #endregion

                                    

                                        int IdPago = SaariE.getTotalReciboPagado(recibo.IDHistRec);
                                        List<SaldoAFavorEntity> ListRecibosPagados = new List<SaldoAFavorEntity>();
                                        decimal SaldoFavor = 0;
                                        if (IdPago != 0)
                                        {

                                            try
                                            {
                                                ListRecibosPagados = SaariE.getTotalReciboPagadoByIDPago(IdPago);
                                            }
                                            catch
                                            {
                                                return "Error al tomar el recibo pago en SQL Server" + recibo.IDHistRec;
                                            }
                                        }
                                    

                                    List<SaldoAFavorEntity> ListSaldoAFavor = new List<SaldoAFavorEntity>();
                                    List<ReciboEntity> li = new List<ReciboEntity>();
                                    if (ListRecibosPagados != null)
                                    {
                                        if (ListRecibosPagados.Count >= 2)
                                        {
                                            foreach (SaldoAFavorEntity c in ListRecibosPagados)
                                            {
                                                ReciboEntity RE = new ReciboEntity();
                                                RE.IDHistRec = c.IdHistRec;
                                                RE.IDCliente = c.IdCliente;
                                                RE.saldoAFavor = c.SaldoFavor;
                                                li.Add(RE);
                                            }
                                            li.OrderBy(x => x.IDHistRec);
                                            int idREc = li.FirstOrDefault().IDHistRec;

                                            if (recibo.IDHistRec == idREc)
                                            {
                                                try
                                                {
                                                    ListSaldoAFavor = SaariE.getSaldoAFavorByIdPago(ListRecibosPagados[0].IdPago);
                                                    if (ListSaldoAFavor != null)
                                                    {
                                                        if (ListSaldoAFavor.Count > 0)
                                                        {
                                                            SaldoFavor = ListSaldoAFavor[0].ImporteSaldo;
                                                            recibo.saldoAFavor = SaldoFavor;
                                                            recibo.TotalPagado = recibo.Importe + recibo.IVA +SaldoFavor;
                                                        }
                                                        else
                                                        {
                                                            SaldoFavor = 0;
                                                            recibo.saldoAFavor = SaldoFavor;
                                                            recibo.TotalPagado = recibo.Importe + recibo.IVA + SaldoFavor;
                                                        
                                                        }
                                                    }
                                                }
                                                catch(Exception ex)
                                                {
                                                    return "Error al tomar saldo a favor de: "+recibo.IDHistRec +" Excepcion :"+ex;
                                                }
                                            }

                                        }
                                        else if (ListRecibosPagados.Count > 0 && ListRecibosPagados.Count < 2)
                                        {
                                            ListSaldoAFavor = SaariE.getSaldoAFavorByIdPago(ListRecibosPagados[0].IdPago);
                                            try
                                            {
                                                if (ListSaldoAFavor != null)
                                                {
                                                    if (ListSaldoAFavor.Count > 0)
                                                    {
                                                        SaldoFavor = ListSaldoAFavor[0].ImporteSaldo;
                                                        recibo.saldoAFavor = SaldoFavor;
                                                        recibo.TotalPagado = recibo.Importe + recibo.IVA + SaldoFavor;
                                                    }

                                                }
                                                else
                                                {
                                                    SaldoFavor = 0;
                                                    recibo.saldoAFavor = SaldoFavor;
                                                    recibo.TotalPagado = recibo.Importe + recibo.IVA +  SaldoFavor;

                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                return "Error al asignar saldo a favor del recibo "+ recibo.IDHistRec + " Excepcion :"+ex;
                                            }
                                        }
                                    }


                                    //recibo.saldoAFavor = SaldoFavor; 
                                    //recibo.TotalPagado = recibo.Importe + recibo.IVA + recibo.ISR + recibo.IVARetenido + SaldoFavor;

                                    if (SelectConjunto)
                                    {
                                        recibo.Info = "conjunto";
                                    }
                                    else if (allConjuntosSub)
                                    {
                                        if (recibo.NombreConjunto != "Todos" && !string.IsNullOrEmpty(recibo.nombreSubconjunto))
                                            recibo.Info = "SubConjunto";
                                        else
                                            recibo.Info = "conjunto";
                                    }
                                    else
                                    {
                                        recibo.Info = "conjunto";
                                    }

                                    try
                                    {
                                        //listaRecibosExpedidosxFolio.OrderBy(c => c.Serie).ToList();
                                        dtRepRecCobradosFolio.Rows.Add(recibo.FechaEmision, recibo.FechaPago, recibo.Serie, recibo.Folio, recibo.Numero, recibo.Inmueble,
                                                recibo.NombreCliente, recibo.Comentario, recibo.Moneda, recibo.TipoCambio, recibo.Importe, recibo.IVA,
                                                recibo.ISR, recibo.IVARetenido, recibo.Total, recibo.ConDllsImporte, recibo.ConDllsIVA,
                                                recibo.ConDllsISR, recibo.ConDllsIVARetenido, recibo.ConDllstotal, recibo.IDConjunto, recibo.NombreConjunto,
                                                recibo.nombreSubconjunto, recibo.Info, recibo.MonedaPago, recibo.TipoCambioPago, recibo.TipoPago, recibo.TotalPagado,
                                                recibo.saldoAFavor);
                                    }
                                    catch (Exception ex)
                                    {
                                        return "Error al asignar datos al reporte." + ex;
                                    }

                                }
                            
                                #endregion reporte normal
                            
                            backgroundWorker1.ReportProgress(80);
                            if (backgroundWorker1.CancellationPending)
                                return "Proceso cancelado por el usuario";


                            reporte.RegisterData(dtEncabezado, "Encabezado");
                            reporte.RegisterData(dtRepRecCobradosFolio, "recibo");

                            DataBand bandaMovimientos = reporte.FindObject("Data6") as DataBand;
                            bandaMovimientos.DataSource = reporte.GetDataSource("recibo");
                           
                            
                            backgroundWorker1.ReportProgress(100);
                            return exportarValidaAbrir(reporte, esPDF, "RecibosCobradosPorFolio", AbrirEx);

                        }
                        catch
                        {
                            return error = "Error general al crear el Archivo de Excel.";
                        }


                    }


                }
                #region MENSAJE NO SE ENCOTRARON REGISTROS
                string mensaje = "No se encontraron registros ";
                mensaje += " en el periodo del " + fechaInicial.ToShortDateString() + " al " + fechafinal.ToShortDateString();
                return mensaje + ". Verifique los criterios seleccionados.";
                #endregion
            }
             return "No se encontraron registros "+
              "La Fecha " + fechafinal.ToShortDateString() + " no puede ser menor a la  Fecha inicial" + fechaInicial.ToShortDateString() 
              +  ". Verifique los criterios seleccionados.";
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

        public string MonthName(int month)
        {
            DateTimeFormatInfo dtInfo = new CultureInfo("es-MX", false).DateTimeFormat;
            return dtInfo.GetMonthName(month);
        }


    }
}
