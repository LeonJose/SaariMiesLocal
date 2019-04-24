using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.Abstracts;
using System.IO;
using FastReport;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class AuditoriaContratos : SaariReport, IReport, IBackgroundReport
    {

        private int numeroRegistros = 0;
        private InmobiliariaEntity inmobiliaria = null;
        private DateTime fechaCorte = DateTime.Now.Date;
        private bool esPdf = false;
        private bool esPreventa = false;
        private string rutaFormato = string.Empty, usuario = string.Empty;
        private bool AbrirReporteAutomatico = false;
        public bool EsError = false;

        /// <summary>
        /// Devuelve una colección de entidades de inmobiliarias
        /// </summary>
        /// <returns></returns>
        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();
        }

        /// <summary>
        /// Inicializa una instancia del reporte de Auditoria de contratos
        /// </summary>
        /// <param name="inmobiliaria">Entidad que contiene la información de la inmobiliaria seleccionada</param>
        /// <param name="fechaCorte">Fecha hasta la cual se buscaran los contratos</param>
        /// <param name="esExcel">Indica si se desea que el reporte generado este en excel, de lo contrario sera en pdf</param>
        /// <param name="rutaFormato">Ruta fisica del formato del reporte</param>
        /// <param name="usuario">Usuario que se encuentra firmado en la aplicación</param>
        /// <returns></returns>
        public AuditoriaContratos(InmobiliariaEntity inmobiliaria, DateTime fechaCorte, bool esExcel, string rutaFormato, string usuario, bool esPreventa, bool abrirReporteAutomatico)
        {
            this.inmobiliaria = inmobiliaria;
            this.fechaCorte = fechaCorte.Date;
            this.esPdf = !esExcel;
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            this.esPreventa = esPreventa;
            this.AbrirReporteAutomatico = abrirReporteAutomatico;
        }
        /// <summary>
        /// Genera el reporte de auditoria de contratos de venta
        /// </summary>        
        /// <returns></returns>
        public string generar()
        {
            try
            {
                SaariDB.ObjetoAgregado += new SaariDB.ObjetoAgregadoHandler(SaariDB_ObjetoAgregado);
                EncabezadoEntity encabezado = new EncabezadoEntity()
                {
                    FechaFin = fechaCorte.ToString("dd/MM/yyyy"),
                    Usuario = usuario,
                    Inmobiliaria = inmobiliaria.RazonSocial,
                    TituloTipoRecibos = esPreventa? "REPORTE DE CONTRATOS DE PREVENTA AL " : "REPORTE DE CONTRATOS DE VENTA AL "
                };
                List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                listaEncabezado.Add(encabezado);
                OnCambioProgreso(10);
                if (File.Exists(rutaFormato))
                {
                    numeroRegistros = SaariDB.getAuditoriaContratosCount(inmobiliaria.ID, fechaCorte, esPreventa);
                    List<AuditoriaContratoEntity> listaContratos = SaariDB.getAuditoriaContratos(inmobiliaria.ID, fechaCorte, esPreventa);
                    if (SaariDB.CancelacionPendiente)
                    {
                        SaariDB.CancelacionPendiente = false;
                        return "Operación cancelada por el usuario";
                    }
                    if (listaContratos != null)
                    {
                        if (listaContratos.Count > 0)
                        {
                            OnCambioProgreso(95);
                            Report report = new Report();
                            report.Load(rutaFormato);
                            report.RegisterData(listaEncabezado, "Encabezado");
                            report.RegisterData(listaContratos, "Contrato");
                            DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                            bandaRecibos.DataSource = report.GetDataSource("Contrato");

                            try
                            {
                                TextObject textTituloColumnaEngancheApartado = report.FindObject("Text99") as TextObject;
                                if (this.esPreventa)
                                    textTituloColumnaEngancheApartado.Text = "Apartado";
                                else
                                    textTituloColumnaEngancheApartado.Text = "Enganche";
                            }
                            catch (Exception ex)
                            {
                                TextObject textTituloColumnaEngancheApartado = report.FindObject("Text99") as TextObject;
                                textTituloColumnaEngancheApartado.Text = "Importe";
                            }
                            EsError = false;
                            return exportar(report, esPdf, "AuditoriaContratos", AbrirReporteAutomatico);
                        }
                        else
                        {
                            EsError = false;
                            return "No existen registros con los parámetros seleccionados para el reporte." + Environment.NewLine;
                        }
                    }
                    else
                    {
                        EsError = false;
                        return "No se obtuvieron registros a presentar en el reporte" + Environment.NewLine;
                    }
                }
                else
                {
                    EsError = true;
                    return "No se encontro el formato " + rutaFormato + Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                EsError = true;
                return "Error general al generar reporte: " + Environment.NewLine + ex.Message + Environment.NewLine;
            }
        }

        void SaariDB_ObjetoAgregado(int contador)
        {
            if (numeroRegistros > 0)
            {
                decimal prc = ((decimal)contador / (decimal)numeroRegistros * 80m) + 10;
                if (prc <= 90)
                    OnCambioProgreso(Convert.ToInt32(prc));
                else
                    OnCambioProgreso(90);
            }
        }

        public override void cancelar()
        {
            base.cancelar();
            SaariDB.CancelacionPendiente = true;
        }
    }
}
