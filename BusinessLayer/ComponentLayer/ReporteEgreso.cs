using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using FastReport;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class ReporteEgreso : SaariReport, IReport, IBackgroundReport
    {
        private string fileName = string.Empty, clasificacion = string.Empty, moneda = string.Empty, rutaFormato = string.Empty, usuario = string.Empty;
        private InmobiliariaEntity inmobiliaria = null;
        private DateTime fechaInicial = new DateTime(), fechaFinal = DateTime.Now.Date;
        private CuentaBancariaEntity cuenta = null;
        private Orden orden = Orden.Ninguno;
        private bool esPdf = true;
        private bool incluirCancelados = false;
        
        public ReporteEgreso(InmobiliariaEntity inmobiliaria, DateTime fechaInicial, DateTime fechaFinal, CuentaBancariaEntity cuenta,
            string clasificacion, string moneda, Orden orden, string rutaFormato, string usuario, bool esPdf, bool incluirCancelados)
        {
            this.inmobiliaria = inmobiliaria;
            this.fechaInicial = fechaInicial.Date;
            this.fechaFinal = fechaFinal.Date;
            this.cuenta = cuenta;
            this.clasificacion = clasificacion;
            this.moneda = moneda;
            this.orden = orden;
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            this.esPdf = esPdf;
            this.incluirCancelados = incluirCancelados;
        }

        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();
        }

        public static List<CuentaBancariaEntity> obtenerCuentas(string idEmpresa)
        {
            return SaariDB.getCuentasBancarias(idEmpresa);
        }

        public static List<string> obtenerClasificaciones()
        {
            return SaariDB.getClasificaciones();
        }

        public string generar()
        {
            try
            {
                if (fechaFinal > fechaInicial)
                {
                    OnCambioProgreso(10);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    EncabezadoEntity encabezado = new EncabezadoEntity()
                    {
                        Inmobiliaria = inmobiliaria.RazonSocial,
                        FechaInicio = fechaInicial.ToString("dd/MM/yyyy"),
                        FechaFin = fechaFinal.ToString("dd/MM/yyyy"),
                        Usuario = usuario,
                        Moneda = cuenta.ID == "*Todas" ? moneda : cuenta.Moneda,
                        Cuenta = cuenta.Descripcion,
                        Clasificacion = clasificacion
                    };
                    encabezado.Moneda = encabezado.Moneda == "P" ? "Pesos" : "Dolares";
                    List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                    listaEncabezado.Add(encabezado);

                    OnCambioProgreso(20);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    List<EgresoEntity> listaEgresos = SaariDB.getRegistrosEgreso(inmobiliaria.ID, fechaInicial, fechaFinal, moneda, incluirCancelados, cuenta.ID, clasificacion, orden);
                    if (listaEgresos != null)
                    {
                        OnCambioProgreso(80);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        if (listaEgresos.Count > 0)
                        {
                            if (File.Exists(rutaFormato))
                            {
                                Report report = new Report();
                                report.Load(rutaFormato);
                                report.RegisterData(listaEncabezado, "Encabezado");
                                report.RegisterData(listaEgresos, "Registro");
                                DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                                bandaRecibos.DataSource = report.GetDataSource("Registro");
                                
                                OnCambioProgreso(90);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                GroupHeaderBand agrupacion = report.FindObject("GroupHeader1") as GroupHeaderBand;
                                if (agrupacion != null)
                                {
                                    TextObject textoOrden = report.FindObject("textoOrden") as TextObject;
                                    if (textoOrden != null)
                                    {
                                        if (orden == Orden.Estatus)
                                        {
                                            textoOrden.Text = "[Registro.Estatus]";
                                            agrupacion.Condition = "[Registro.Estatus]";
                                        }
                                        else if (orden == Orden.Clasificacion)
                                        {
                                            textoOrden.Text = "[Registro.Clasificacion]";
                                            agrupacion.Condition = "[Registro.Clasificacion]";
                                        }
                                        else if (orden == Orden.Beneficiario)
                                        {
                                            textoOrden.Text = "[Registro.Beneficiario]";
                                            agrupacion.Condition = "[Registro.Beneficiario]";
                                        }
                                    }
                                }
                                OnCambioProgreso(95);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";
                                return exportar(report, esPdf, "ReporteEgresos");
                            }
                            else
                                return "No se encontro el formato " + rutaFormato + Environment.NewLine;
                        }
                        else
                            return "No se encontraron registros de egreso" + Environment.NewLine;
                    }
                    else
                        return "Hubo un error al obtener los datos de los egresos" + Environment.NewLine;
                }
                else
                    return "La fecha final debe ser mayor a la fecha inicial" + Environment.NewLine;
            }
            catch (Exception ex)
            {
                return "Error general al generar reporte: " + Environment.NewLine + ex.Message + Environment.NewLine;
            }
        }
    }
}
