using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.DataAccessLayer;
using FastReport;
using System.IO;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.Abstracts;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class ImpuestosReporte : SaariReport, IReport, IBackgroundReport
    {

        private string IDInmobiliaria = string.Empty;
        private string NombreInmobiliaria = string.Empty;
        private string IDConjunto = string.Empty;
        private string NombreConjunto = string.Empty;
        private DateTime FechaInicio, FechaFin;
        private bool EsPdf = false;
        private string RutaFormato = string.Empty;
        public event EventHandler<CambioProgresoEventArgs> CambioProgreso;
        public ImpuestosReporte()
        { }

        public ImpuestosReporte(string idInmobiliaria, string idConjunto, bool espdf, string nombreinmobiliaria, string nombreconjunto, DateTime fechainicio, DateTime fechafin,
            string rutaformato)
        {
            this.IDInmobiliaria = idInmobiliaria;
            this.IDConjunto = idConjunto;
            this.EsPdf = espdf;
            this.NombreInmobiliaria = nombreinmobiliaria;
            this.NombreConjunto = nombreconjunto;
            this.FechaInicio = fechainicio;
            this.FechaFin = fechafin;
            this.RutaFormato = rutaformato;
        }

        public string generar()
        {
            string error = string.Empty;
            try
            {
                OnCambioProgreso(10);
                if (CancelacionPendiente)
                    error = "Cancelacion del proceso por el usuario";

                List<ReporteImpuestos> listaRecibos = null;
                listaRecibos = SaariDB.getRecibosImpuestos();
                List<EncabezadoEntity> listaEncabezado = null;
                if (listaRecibos != null)
                {
                    if (listaRecibos.Count > 0)
                    {
                        OnCambioProgreso(25);
                        if (CancelacionPendiente)
                            error = "Cancelacion del proceso por el usuario"; 

                        EncabezadoEntity encabezado = new EncabezadoEntity();
                        encabezado.Inmobiliaria = NombreInmobiliaria;
                        encabezado.Conjunto = NombreConjunto;
                        encabezado.FechaInicio = FechaInicio.Day + "/" + HelpInmobiliarias.MonthName(FechaInicio.Month) + "/" + FechaInicio.Year;
                        encabezado.FechaFin = FechaFin.Day + "/" + HelpInmobiliarias.MonthName(FechaFin.Month) + "/" + FechaFin.Year;

                        List<SaldoEntity> listaTotal = new List<SaldoEntity>();
                        SaldoEntity total = new SaldoEntity();
                        total.Total = listaRecibos.Sum(s => s.impuestos);
                        listaTotal.Add(total);

                        OnCambioProgreso(70);
                        listaEncabezado = new List<EncabezadoEntity>();
                        listaEncabezado.Add(encabezado);
                        if (File.Exists(RutaFormato))
                        {
                            Report report = new Report();
                            report.Load(RutaFormato);
                            report.RegisterData(listaEncabezado, "Encabezado");
                            report.RegisterData(listaRecibos, "Recibo", 3);
                            report.RegisterData(listaTotal, "Total");

                            DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                            bandaRecibos.DataSource = report.GetDataSource("Recibo");


                            //DataBand bandaFacturas = report.FindObject("Data2") as DataBand;
                            //bandaFacturas.DataSource = report.GetDataSource("Recibo.Recibos");

                            OnCambioProgreso(95);
                            if (CancelacionPendiente)
                                error = "Cancelacion del proceso por el usuario";

                            error = exportar(report, EsPdf, "ResumenImpuestos");
                        }
                        else
                            error = "No se encontro el formato " + RutaFormato + Environment.NewLine;


                        error = string.Empty;
                    }
                    else
                    {
                        error = " No se encontraton registros con los parametros dados";
                    }
                }
                else
                {
                    error = " Ocurrio un error al leer los registros de la base de datos";
                }
            }
            catch (Exception e)
            {
                error = "Error " + e.Message;
            }
            return error;
        }



    }
}
