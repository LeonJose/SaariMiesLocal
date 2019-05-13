using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.Helpers;
using GestorReportes.BusinessLayer.Entities;
using System.IO;
using FastReport;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class ReporteCuentasPorCobrar : SaariReport
    {
        private string idInmobiliaria = string.Empty, idConjunto = string.Empty, rutaFormato = string.Empty, usuario = string.Empty;
        private DateTime fechaCorte;
        private bool esPdf;
        public ReporteCuentasPorCobrar(string idArre, string idConjun, string user, string rutaformato, bool Pdf, DateTime fecha)
        {
            idInmobiliaria = idArre;
            fechaCorte = fecha;
            idConjunto = idConjun;
            esPdf = Pdf;
            usuario = user;
            rutaFormato = rutaformato;
        }

        public string GenerarReporte()
        {
            string resp = string.Empty;
            try
            {
                List<ReciboEntity> listacuentasporCobrar = null;
                OnCambioProgreso(10);

                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                if (idConjunto == "Todos")
                    listacuentasporCobrar = SaariDB.GetListaCtasxCobrar(idInmobiliaria, fechaCorte);
                else
                    listacuentasporCobrar = SaariDB.GetListaCtasxCobrar(idInmobiliaria, idConjunto, fechaCorte);
                OnCambioProgreso(20);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                if (listacuentasporCobrar != null)
                {
                    if (listacuentasporCobrar.Count > 0)
                    {

                        OnCambioProgreso(25);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";

                        List<ClienteEntity> ListaClientes = SaariDB.getClientes();
                        List<CuentaPorCobrarEntity> listCntasxCobrar = new List<CuentaPorCobrarEntity>();
                        CuentaPorCobrarEntity cuentaPorCobrar = new CuentaPorCobrarEntity();
                        decimal total30 = 0, total60 = 0, total90 = 0, totalMas90 = 0;
                        decimal interesMoratorio30 = 0, interesMoratorio60 = 0, interesMoratorio90 = 0;
                        //foreach (ReciboEntity list in listacuentasporCobrar)
                        //{
                        List<ReciboEntity> list30 = (from r in listacuentasporCobrar
                                                     where (r.VencimientoPago >= fechaCorte.Date.AddDays(-30) && r.VencimientoPago <= fechaCorte.Date)
                                                     select r).ToList();
                        List<ReciboEntity> list60 = (from r in listacuentasporCobrar
                                                     where (r.VencimientoPago >= fechaCorte.Date.AddDays(-60) && r.VencimientoPago <= fechaCorte.Date && r.VencimientoPago <= fechaCorte.Date.AddDays(-31))
                                                     select r).ToList();
                        List<ReciboEntity> list90 = (from r in listacuentasporCobrar
                                                     where (r.VencimientoPago <= fechaCorte.Date.AddDays(-61) && r.VencimientoPago <= fechaCorte.Date && r.VencimientoPago <= fechaCorte.Date.AddDays(-61))
                                                     select r).ToList();
                        //List<ReciboEntity> list90 = (from r in listacuentasporCobrar
                        //                             where (r.VencimientoPago <= fechaCorte.Date.AddDays(-61) && r.VencimientoPago <= fechaCorte.Date && r.VencimientoPago <= fechaCorte.Date.AddDays(-61))
                        //                             select r).ToList();
                        //List<ReciboEntity> listMas90 = (from r in listacuentasporCobrar
                        //                             where (r.VencimientoPago >= fechaCorte.Date.AddDays(-91) && r.VencimientoPago <= fechaCorte.Date /*&& r.VencimientoPago <= fechaCorte.Date.AddDays(-91)*/)
                        //                             select r).ToList();
                        //}
                        OnCambioProgreso(50);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        List<ReciboEntity> listEnganches = new List<ReciboEntity>();
                        List<ReciboEntity> listMensualidad = new List<ReciboEntity>();
                        List<ReciboEntity> listcargosPeriodicos = new List<ReciboEntity>();

                        List<SaldoEntity> listSaldoEng = new List<SaldoEntity>();
                        List<SaldoEntity> listSaldoMen = new List<SaldoEntity>();
                        List<SaldoEntity> listSaldoCarg= new List<SaldoEntity>(); 
                        List<SaldoEntity> listSaldosTotales = new List<SaldoEntity>();

                        #region FILTRO LIST30

                        foreach (var e in list30)
                        {
                            decimal cantidadEng30 = 0;
                            decimal InteresEng30 = 0;
                            cantidadEng30 = e.PagoCapital;
                            InteresEng30 = e.InteresesMoratorios;

                            if (e.TipoDoc == "E")//E = Cargo por enganche normal preventa
                            {
                                total30 += cantidadEng30;
                                e.Importe30 = cantidadEng30;
                                e.InteresesMoratorios30 = InteresEng30;
                                listEnganches.Add(e);
                            }
                            else if (e.TipoDoc == "V")
                            {
                                total30 += cantidadEng30;                                
                                e.Importe30 = cantidadEng30;
                                e.InteresesMoratorios30 = InteresEng30;
                                listMensualidad.Add(e);
                            }
                            else if (e.TipoDoc == "Z")
                            {
                                total30 += cantidadEng30;
                                e.Importe30 = cantidadEng30;
                                e.InteresesMoratorios30 = InteresEng30;
                                listcargosPeriodicos.Add(e);
                            }
                        }
                        #endregion
                        #region FILTRO LIST60

                        foreach (var e in list60)
                        {
                            decimal cantidadEng60 = 0;
                            decimal InteresEng60 = 0;
                            cantidadEng60 = e.PagoCapital;
                            InteresEng60 = e.InteresesMoratorios;
                            if (e.TipoDoc == "E")//E = Cargo por enganche normal preventa
                            {
                                total60 += cantidadEng60;
                                e.Importe60 = cantidadEng60;
                                e.InteresesMoratorios60 = InteresEng60;
                                listEnganches.Add(e);
                            }
                            else if (e.TipoDoc == "V")
                            {
                                total60 += cantidadEng60;
                                e.Importe60 = cantidadEng60;
                                e.InteresesMoratorios60 = InteresEng60;
                                listMensualidad.Add(e);
                            }
                            else if (e.TipoDoc == "Z")
                            {
                                total60 += cantidadEng60;
                                e.Importe60 = cantidadEng60;
                                e.InteresesMoratorios60 = InteresEng60;
                                listcargosPeriodicos.Add(e);
                            }
                        }
                        #endregion
                        #region FILTRO LIST90

                        foreach (var e in list90)
                        {
                            decimal cantidadEng90 = 0;
                            decimal InteresEng90 = 0;
                            cantidadEng90 = e.PagoCapital;
                            InteresEng90 = e.InteresesMoratorios;
                            if (e.TipoDoc == "E")//E = Cargo por enganche normal preventa
                            {
                                total90 += cantidadEng90;
                                e.Importe90 = cantidadEng90;
                                e.InteresesMoratorios90 = InteresEng90;
                                listEnganches.Add(e);
                            }
                            else if (e.TipoDoc == "V")
                            {
                                total90 += cantidadEng90;
                                e.Importe90 = cantidadEng90;
                                e.InteresesMoratorios90 = InteresEng90;
                                listMensualidad.Add(e);
                            }
                            else if(e.TipoDoc == "Z")
                            {
                                total90 += cantidadEng90;
                                e.Importe90 = cantidadEng90;
                                e.InteresesMoratorios90 = InteresEng90;
                                listcargosPeriodicos.Add(e);
                            }
                        }
                        #endregion
                        OnCambioProgreso(70);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";

                        #region SALDOS 

                        SaldoEntity saldosEng = new SaldoEntity()
                        {
                            S30 = listEnganches.Sum(s => s.Importe30),
                            S60 = listEnganches.Sum(s => s.Importe60),
                            S90 = listEnganches.Sum(s => s.Importe90),
                            M30 = listEnganches.Sum(s => s.InteresesMoratorios30),
                            M60 = listEnganches.Sum(s => s.InteresesMoratorios60),
                            M90 = listEnganches.Sum(s => s.InteresesMoratorios90)
                        };
                        listSaldoEng.Add(saldosEng);

                        SaldoEntity saldosMen = new SaldoEntity()
                        {
                            S30 = listMensualidad.Sum(s => s.Importe30),
                            S60 = listMensualidad.Sum(s => s.Importe60),
                            S90 = listMensualidad.Sum(s => s.Importe90),
                            M30 = listMensualidad.Sum(s => s.InteresesMoratorios30),
                            M60 = listMensualidad.Sum(s => s.InteresesMoratorios60),
                            M90 = listMensualidad.Sum(s => s.InteresesMoratorios90)
                        };
                        listSaldoMen.Add(saldosMen);

                        SaldoEntity saldosCargPer = new SaldoEntity()
                        {
                            S30 = listcargosPeriodicos.Sum(s => s.Importe30),
                            S60 = listcargosPeriodicos.Sum(s => s.Importe60),
                            S90 = listcargosPeriodicos.Sum(s => s.Importe90),
                            M30 = listcargosPeriodicos.Sum(s => s.InteresesMoratorios30),
                            M60 = listcargosPeriodicos.Sum(s => s.InteresesMoratorios60),
                            M90 = listcargosPeriodicos.Sum(s => s.InteresesMoratorios90),                            
                        };
                        listSaldoCarg.Add(saldosCargPer);

                        decimal pagoTotalEng = 0 , moratoriosE = 0, totalE = 0;
                        decimal pagoTotalMen = 0, moratoriosMe = 0, totalMe = 0;
                        decimal pagoTotalMor = 0, moratoriosMo = 0, totalMo = 0;
                        SaldoEntity saldosTotales = new SaldoEntity();
                        foreach (var l in listSaldoEng)
                        {
                            pagoTotalEng = l.TotalEngCapital;
                            moratoriosE = l.TotalMoratoriosE;
                            totalE = l.TotalEnganche;

                        }
                        foreach (var l in listSaldoMen)
                        {
                            pagoTotalMen = l.TotalMenCapital;
                            moratoriosMe = l.TotalMoratoriosM;
                            totalMe = l.TotalMensualidad;
                        }
                        foreach (var l in listSaldoCarg)
                        {
                            pagoTotalMor = l.TotalCarPerCapital;
                            moratoriosMo = l.TotalMoratoriosCP;
                            totalMo = l.TotalMensualidad;
                        }
                        decimal TotalPagoCapital = pagoTotalEng + pagoTotalMen + pagoTotalMor;
                        decimal TotalPagoMoratorios = moratoriosE + moratoriosMe + moratoriosMo;
                        decimal TotalPorCobrar = TotalPagoCapital + TotalPagoMoratorios;

                        saldosTotales.TotalPagoCapital = TotalPagoCapital;
                        saldosTotales.TotalPagoMoratorios = TotalPagoMoratorios;
                        saldosTotales.TotalPorCobrar = TotalPorCobrar;
                        listSaldosTotales.Add(saldosTotales);
                        #endregion

                        OnCambioProgreso(80);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";

                        string nombreInmobiliaria = SaariDB.getNombreInmobiliariaByID(idInmobiliaria);
                        string nombreConjunto = SaariDB.getNombreConjuntoByID(idConjunto);

                        EncabezadoEntity encabezado = new EncabezadoEntity()
                        {
                            Inmobiliaria = nombreInmobiliaria,
                            FechaFin = fechaCorte.ToString(@"dd \de MMMM \del yyyy").ToLower(),
                            Conjunto = nombreConjunto,
                            Usuario = usuario
                        };
                        if (string.IsNullOrEmpty(encabezado.Conjunto))
                            encabezado.Conjunto = "Todos los Conjuntos";

                        List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                        listaEncabezado.Add(encabezado);                      

                        OnCambioProgreso(90);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        if (File.Exists(rutaFormato))
                        {
                            Report report = new Report();
                            report.Load(rutaFormato);
                            report.RegisterData(listaEncabezado, "Encabezado");
                            report.RegisterData(listEnganches, "Enganche");
                            report.RegisterData(listMensualidad, "Mensualidad");
                            report.RegisterData(listcargosPeriodicos, "CargoPeriodico");

                            report.RegisterData(listSaldoEng, "SaldoEnganche");
                            report.RegisterData(listSaldoMen, "SaldoMensualidad");
                            report.RegisterData(listSaldoCarg, "SaldoCargoPer");
                            report.RegisterData(listSaldosTotales, "SaldoTotal");

                            DataBand bandaEnganche = report.FindObject("Data1") as DataBand;
                            bandaEnganche.DataSource = report.GetDataSource("Enganche");

                            DataBand bandaMensualidad = report.FindObject("Data2") as DataBand;
                            bandaMensualidad.DataSource = report.GetDataSource("Mensualidad");

                            DataBand bandaCargoPeriodico = report.FindObject("Data3") as DataBand;
                            bandaCargoPeriodico.DataSource = report.GetDataSource("CargoPeriodico");

                            DataBand bandaSaldoTotal = report.FindObject("Data4") as DataBand;
                            bandaSaldoTotal.DataSource = report.GetDataSource("SaldoTotal");


                            return exportar(report, esPdf, "ReporteCuentasPorCobrar");
                        }
                        else
                            return "No se encontro el formato " + rutaFormato + Environment.NewLine;
                    }
                }
                else
                    return "Error al obtener lista de cuentas por cobrar.";

                return resp;
            }

            catch (Exception ex)
            {
                return "Error inesperado al generar el reporte: " + Environment.NewLine + ex.Message;
            }
        }
    }
}
