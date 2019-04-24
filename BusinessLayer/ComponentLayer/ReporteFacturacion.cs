using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using System.IO;
using FastReport;
using FastReport.Export.Pdf;
using FastReport.Export.OoXML;
using System.Diagnostics;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.Abstracts;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class ReporteFacturacion : SaariReport, IReport, IBackgroundReport
    {
        private DateTime inicio = new DateTime(), fin = DateTime.Now.Date;
        private List<string> idArrendadoras = new List<string>();
        private string usuario = string.Empty, rutaFormato = string.Empty;
        private bool esPdf = false;
        public ReporteFacturacion(DateTime inicio, DateTime fin, List<string> idArrendadoras, string usuario, string rutaFormato, bool esPdf)
        {
            this.inicio = inicio;
            this.fin = fin;
            this.idArrendadoras = idArrendadoras;
            this.usuario = usuario;
            this.rutaFormato = rutaFormato;
            this.esPdf = esPdf;
        }

        public string generar()
        {
            try
            {
                if (inicio <= fin)
                {
                    OnCambioProgreso(10);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    if (idArrendadoras.Count > 0)
                    {
                        OnCambioProgreso(20);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        EncabezadoEntity encabezado = new EncabezadoEntity()
                        {
                            FechaInicio = inicio.Date.ToString(@"dd \de MMMM \del yyyy").ToLower(),
                            FechaFin = fin.Date.ToString(@"dd \de MMMM \del yyyy").ToLower(),
                            Usuario = usuario
                        };
                        List<EncabezadoEntity> listaEncabezados = new List<EncabezadoEntity>();
                        listaEncabezados.Add(encabezado);

                        OnCambioProgreso(30);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        List<RegistroFacturacionEntity> listaRegistros = SaariDB.getRegistrosFacturacion(inicio, fin, idArrendadoras);

                        if (listaRegistros != null)
                        {
                            if (listaRegistros.Count > 0)
                            {
                                OnCambioProgreso(40);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";
                                List<RegistroFacturacionEntity> listaNC = SaariDB.getNCFacturacion(inicio, fin, idArrendadoras);
                                if (listaNC != null)
                                {
                                    OnCambioProgreso(70);
                                    if (CancelacionPendiente)
                                        return "Proceso cancelado por el usuario";
                                    foreach (RegistroFacturacionEntity r in listaNC)
                                    {
                                        RegistroFacturacionEntity reg = listaRegistros.FirstOrDefault(l => l.IDContrato == r.IDContrato && l.IDCliente == r.IDCliente);
                                        if (reg != null)
                                        {
                                            reg.Total = reg.Total - r.Total;
                                            reg.TotalReal = reg.TotalReal - r.TotalReal;
                                        }
                                        else
                                        {
                                            r.Total = 0 - r.Total;
                                            r.TotalReal = 0 - r.TotalReal;
                                            listaRegistros.Add(r);
                                        }
                                    }
                                }
                                OnCambioProgreso(80);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";
                                listaRegistros = listaRegistros.OrderBy(r => r.RazonInmobiliaria).ThenBy(r => r.RazonCliente).ToList();
                                if (File.Exists(rutaFormato))
                                {
                                    OnCambioProgreso(90);
                                    if (CancelacionPendiente)
                                        return "Proceso cancelado por el usuario";

                                    Report report = new Report();
                                    report.Load(rutaFormato);
                                    report.RegisterData(listaEncabezados, "Encabezado");
                                    report.RegisterData(listaRegistros, "Registro");

                                    DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                                    bandaRecibos.DataSource = report.GetDataSource("Registro");
                                    return exportar(report, esPdf, "ReporteFacturacion");
                                }
                                else
                                    return "No se encontro el formato " + rutaFormato + Environment.NewLine;
                            }
                            else
                                return "No se encontraron registros para las condiciones dadas";
                        }
                        else
                            return "Error al obtener los registros de la base de datos";
                    }
                    else
                        return "Debe seleccionar al menos una inmobiliaria";
                }
                else
                    return "La fecha de fin debe ser mayor o igual a la fecha de inicio";
            }
            catch (Exception ex)
            {
                return "Error inesperado al generar el reporte de facturación: " + Environment.NewLine + ex.Message;
            }
        }

        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();
        }
    }
}
