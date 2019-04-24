using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;
using FastReport;
using System.IO;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class CFDIsCancelados : SaariReport, IReport, IBackgroundReport
    {
        private string idInmobiliaria = string.Empty, idConjunto = string.Empty, usuario = string.Empty, rutaFormato = string.Empty;
        private DateTime fechaInicio = new DateTime(), fechaFin = new DateTime();
        private bool esPdf = true, incluyeEnProcesoCancelacion = false;

        public CFDIsCancelados(string idInmobiliaria, string idConjunto, DateTime fechaInicio, DateTime fechaFin, string usuario, string rutaFormato, bool esPdf, bool incluyeEnProcesoCancelacion)
        {
            this.idInmobiliaria = idInmobiliaria;
            this.idConjunto = idConjunto;
            this.fechaInicio = fechaInicio.Date;
            this.fechaFin = fechaFin.Date;
            this.usuario = usuario;
            this.rutaFormato = rutaFormato;
            this.esPdf = esPdf;
            this.incluyeEnProcesoCancelacion = incluyeEnProcesoCancelacion;
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
                string resultValidar = validar(idInmobiliaria, idConjunto, fechaInicio, fechaFin, rutaFormato);                
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                OnCambioProgreso(5);

                if (string.IsNullOrEmpty(resultValidar))
                {
                    if (File.Exists(rutaFormato))
                    {
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        OnCambioProgreso(10);

                        List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                        if (idConjunto == "Todos")
                        {
                            listaRecibos = SaariDB.getListaRecibosCancelados(idInmobiliaria, fechaInicio, fechaFin);
                            if (incluyeEnProcesoCancelacion)
                            {
                                List<ReciboEntity> listaRecibosPendientes = SaariE.getListaRecibosEnProcesoCancelacion(idInmobiliaria, fechaInicio, fechaFin);
                                if (listaRecibosPendientes != null || listaRecibosPendientes.Count > 0)
                                    listaRecibos.AddRange(listaRecibosPendientes);
                            }
                        }
                        else
                        {
                            listaRecibos = SaariDB.getListaRecibosCancelados(idInmobiliaria, idConjunto, fechaInicio, fechaFin);
                            if (incluyeEnProcesoCancelacion)
                            {
                                List<ReciboEntity> listaRecibosPendientes = SaariE.getListaRecibosEnProcesoCancelacion(idInmobiliaria, idConjunto, fechaInicio, fechaFin);
                                if (listaRecibosPendientes != null || listaRecibosPendientes.Count > 0)
                                    listaRecibos.AddRange(listaRecibosPendientes);
                            }
                        }

                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        OnCambioProgreso(15);

                        if (listaRecibos != null)
                        {
                            if (listaRecibos.Count > 0)
                            {
                                List<CFDICanceladoEntity> listaCancelados = new List<CFDICanceladoEntity>();
                                CFDICanceladoEntity registroCancelados = new CFDICanceladoEntity();                               

                                registroCancelados.RecibosCanceladosPesos = (from r in listaRecibos
                                                                             where r.Moneda == "P"
                                                                             select r).ToList();
                                
                                registroCancelados.RecibosCanceladosDolares = (from r in listaRecibos
                                                                               where r.Moneda == "D"
                                                                               select r).ToList();

                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";
                                OnCambioProgreso(25);

                                int porcentaje = 25;
                                decimal factor = 50 / listaRecibos.Count;
                                factor = factor >= 1 ? factor : 1;

                                foreach (ReciboEntity recibo in registroCancelados.RecibosCanceladosPesos)
                                {
                                    if (porcentaje <= 25)
                                        porcentaje += Convert.ToInt32(factor);
                                    OnCambioProgreso(porcentaje);
                                    if (CancelacionPendiente)
                                        return "Proceso cancelado por el usuario";

                                    recibo.TipoCambio = 0;

                                    registroCancelados.ImportePesos += recibo.Importe;
                                    registroCancelados.IVAPesos += recibo.TotalIVA;
                                    registroCancelados.TotalPesos += recibo.Total;
                                }

                                foreach (ReciboEntity recibo in registroCancelados.RecibosCanceladosDolares)
                                {
                                    if (porcentaje <= 75)
                                        porcentaje += Convert.ToInt32(factor);
                                    OnCambioProgreso(porcentaje);
                                    if (CancelacionPendiente)
                                        return "Proceso cancelado por el usuario";

                                    registroCancelados.ImporteDolares += recibo.Importe;
                                    registroCancelados.IVADolares += recibo.TotalIVA;
                                    registroCancelados.TotalDolares += recibo.Total;

                                    registroCancelados.ImporteDolaresConvertidos += recibo.Importe * recibo.TipoCambio;
                                    registroCancelados.IVADolaresConvertidos += recibo.TotalIVA * recibo.TipoCambio;
                                    registroCancelados.TotalDolaresConvertidos += recibo.Total * recibo.TipoCambio;
                                }

                                OnCambioProgreso(80);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                string nombreInmobiliaria = SaariDB.getNombreInmobiliariaByID(idInmobiliaria);
                                string nombreConjunto = SaariDB.getNombreConjuntoByID(idConjunto);

                                EncabezadoEntity encabezado = new EncabezadoEntity()
                                {
                                    Inmobiliaria = nombreInmobiliaria,
                                    Conjunto = nombreConjunto,
                                    FechaInicio = fechaInicio.ToString("dd/MM/yyyy"),
                                    FechaFin = fechaFin.ToString("dd/MM/yyyy"),
                                    Usuario = usuario
                                };
                                List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                                listaEncabezado.Add(encabezado);
                                listaCancelados.Add(registroCancelados);

                                OnCambioProgreso(85);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                Report report = new Report();
                                report.Load(rutaFormato);
                                report.RegisterData(listaEncabezado, "Encabezado");
                                report.RegisterData(listaRecibos, "Recibo");
                                report.RegisterData(listaCancelados, "Totales");

                                DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                                bandaRecibos.DataSource = report.GetDataSource("Recibo");

                                OnCambioProgreso(85);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                GroupHeaderBand agrupacion = report.FindObject("GroupHeader1") as GroupHeaderBand;
                                if (agrupacion != null)
                                {
                                    TextObject textoOrden = report.FindObject("textoOrden") as TextObject;
                                    textoOrden.Text = "[Recibo.NombreMoneda]";
                                    agrupacion.Condition = "[Recibo.NombreMoneda]";
                                }

                                if (registroCancelados.RecibosCanceladosDolares.Count == 0)
                                {
                                    ReportSummaryBand totalesEnPesos = report.FindObject("ReportSummary1") as ReportSummaryBand;
                                    totalesEnPesos.Visible = false;
                                    totalesEnPesos.Delete();
                                }

                                OnCambioProgreso(90);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";
                                return exportar(report, esPdf, "CFDIsCancelados");
                            }
                            else
                                return "No se encontraron recibos con las condiciones dadas";
                        }
                        else
                            return "Error al obtener los recibos";
                    }
                    else
                        return "No se encontró el formato " + rutaFormato + Environment.NewLine;
                }
                else
                    return resultValidar;
            }
            catch (Exception ex)
            {
                return "Error inesperado al generar el reporte: " + Environment.NewLine + ex.Message;
            }
        }

        public string validar(string idInmobiliaria, string idConjunto, DateTime fechaInicio, DateTime fechaFin, string rutaFormato)
        { 
            string error = string.Empty;
            if (string.IsNullOrEmpty(idInmobiliaria))
                error += "Debe seleccionar una inmobiliaria" + Environment.NewLine;
            if (string.IsNullOrEmpty(idConjunto))
                error += "Debe seleccionar un conjunto" + Environment.NewLine;
            if (fechaInicio > fechaFin)
                error += "La fecha de fin debe ser mayor o igual a la fecha de inicio" + Environment.NewLine;
            if (!File.Exists(rutaFormato))
                error += "No se encontró el formato  " + rutaFormato + Environment.NewLine;
            return error;
        }
    }
}
