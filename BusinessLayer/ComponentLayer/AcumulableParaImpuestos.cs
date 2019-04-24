using FastReport;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Helpers;
using GestorReportes.BusinessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.ComponentLayer
{

   public  class AcumulableParaImpuestos : SaariReport
    {
        private filtroReportes filtro = null;
        private string Error = string.Empty;
        private BackgroundWorker Wkr = null;
        public AcumulableParaImpuestos()
        {
        }

        public AcumulableParaImpuestos(filtroReportes filtroReporte, BackgroundWorker bkg)
        {
            this.filtro = filtroReporte;
            this.Wkr = bkg;
        }

        public new EventHandler<CambioProgresoEventArgs> CambioProgreso { get; internal set; }

        public string  generarReporte()
        {
            try
            {
                if (Wkr.CancellationPending)
                    return "Proceso cancelado por el Usuario";

               Wkr.ReportProgress(30);
                List<AcumulablesImpuestosEntity> ListAcumulableImpuestos = SaariDB.GetAcumulablesParaImpuestos(filtro);
                if (ListAcumulableImpuestos != null)
                {
                    if (ListAcumulableImpuestos.Count > 0)
                    {
                        Wkr.ReportProgress(55);
                        List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                        EncabezadoEntity encabezado = new EncabezadoEntity();
                        encabezado.Inmobiliaria = filtro.NombreInmobiliaria;
                        encabezado.Conjunto = filtro.NombreConjunto;
                        encabezado.FechaInicio = filtro.FechaInicio.Day + "/" + HelpInmobiliarias.MonthName(filtro.FechaInicio.Month) + "/" + filtro.FechaInicio.Year;
                        encabezado.FechaFin = filtro.FechaFin.Day + "/" + HelpInmobiliarias.MonthName(filtro.FechaFin.Month) + "/" + filtro.FechaFin.Year;
                        encabezado.Titulo1 = "Ingresos Mes Corriente";
                        encabezado.Titulo2 = "Ingresos Mes Anteriores";
                        listaEncabezado.Add(encabezado);

                        if (Wkr.CancellationPending)
                            return "Proceso cancelado por el Usuario";
                        List<TotalesImpuestosAcumulables> totales = new List<TotalesImpuestosAcumulables>();
                        TotalesImpuestosAcumulables total = new TotalesImpuestosAcumulables();

                        total.IngresosMesCorrienteImporte1 = ListAcumulableImpuestos.FindAll(f => f.EsMesCorriente == true).Sum(s => s.ImporteConversion);
                        total.IngresosMesCorrienteIva1 = ListAcumulableImpuestos.FindAll(f => f.EsMesCorriente == true).Sum(s => s.IvaConversion);

                        total.IngresosMesAnteriorImporte1 = ListAcumulableImpuestos.FindAll(f => f.EsMesCorriente == false).Sum(s => s.ImporteConversion2);
                        total.IngresosMesAnteriorIva1 = ListAcumulableImpuestos.FindAll(f => f.EsMesCorriente == false).Sum(s => s.IvaConversion2);
                        total.TotalIngresosContables = total.IngresosMesCorrienteImporte1 + total.IngresosMesAnteriorImporte1;
                        total.TotalIngresosContablesIva = total.IngresosMesCorrienteIva1 + total.IngresosMesAnteriorIva1;
                        total.TotalIngresosNoCobrados = ListAcumulableImpuestos.FindAll(f => f.EsMesCorriente == true && f.Estatus == "1").Sum(s => s.ImporteConversion);
                        total.TotalIngresosNoCobradosIva = ListAcumulableImpuestos.FindAll(f => f.EsMesCorriente == true && f.Estatus == "1").Sum(s => s.IvaConversion);

                        total.TotalIngresoContabiliadCobrado = ListAcumulableImpuestos.FindAll(f =>  f.Estatus == "2" && f.TipoDoc != "X").Sum(s => s.ImporteConversion2);
                        total.TotalIngresoContabiliadCobradoIva = ListAcumulableImpuestos.FindAll(f => f.Estatus == "2" && f.TipoDoc != "X").Sum(s => s.IvaConversion2);

                        totales.Add(total);

                        if (Wkr.CancellationPending)
                            return "Proceso cancelado por el Usuario";
                        Wkr.ReportProgress(75);


                        Report reporte = new Report();
                        reporte.Load(filtro.RutaFormato);
                        reporte.RegisterData(listaEncabezado, "Encabezado");
                        reporte.RegisterData(ListAcumulableImpuestos,"Acumulable");
                        reporte.RegisterData(totales, "Total");
                        DataBand bandaRecibos = reporte.FindObject("Data1") as DataBand;
                        bandaRecibos.DataSource = reporte.GetDataSource("Acumulable");

                        Wkr.ReportProgress(85);
                        if (Wkr.CancellationPending)
                            return "Proceso cancelado por el Usuario";
                        return exportar(reporte, filtro.esPDF, "Impuestos Acumulables");
                    }
                    else
                        Error = "No se encontraron registros con los parametros dados.";
                }
                else
                    Error = "Ocurrio un error al tratar de obtener los datos.";
            }
            catch (Exception e)
            {
                return "Error General al tratar de generar el reporte, Error: " + e.Message;
            }
            return Error;
        }

    }
}
