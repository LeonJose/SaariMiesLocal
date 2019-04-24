using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using FastReport;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class AuditoriaCobranza : SaariReport, IReport, IBackgroundReport
    {
        //private string nombreArchivo = string.Empty;
        private List<ClienteEntity> clientes = null;
        private string idArrendadora = string.Empty, rutaFormato = string.Empty, usuario = string.Empty;
        private DateTime fechaCorte = DateTime.Now.Date;
        private bool esPdf = true;

        public AuditoriaCobranza(ClienteEntity cliente, string idArrendadora, DateTime fechaCorte, bool esPdf, string rutaFormato, string usuario)
        {
            this.clientes = new List<ClienteEntity>() { cliente };
            this.idArrendadora = idArrendadora;
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            this.fechaCorte = fechaCorte.Date;
            this.esPdf = esPdf;
        }

        public AuditoriaCobranza(List<ClienteEntity> clientes, string idArrendadora, DateTime fechaCorte, bool esPdf, string rutaFormato, string usuario)
        {
            this.clientes = clientes;
            this.idArrendadora = idArrendadora;
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            this.fechaCorte = fechaCorte.Date;
            this.esPdf = esPdf;
        }

        public static List<ClienteEntity> obtenerClientes()
        {
            return SaariDB.getClientes();
        }

        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();
        }

        private string generarReporte(ClienteEntity cliente)
        {
            try
            {
                OnCambioProgreso(10);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                if (File.Exists(rutaFormato))
                {
                    string nombreInmobiliaria = string.Empty;
                    if(!string.IsNullOrWhiteSpace(idArrendadora))
                        nombreInmobiliaria = SaariDB.getNombreInmobiliariaByID(idArrendadora);
                    
                    OnCambioProgreso(20);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    EncabezadoEntity encabezado = new EncabezadoEntity()
                    {
                        Inmobiliaria = nombreInmobiliaria,
                        FechaFin = fechaCorte.ToString("dd/MM/yyyy"),
                        Usuario = usuario,
                        Cliente = cliente.Nombre,
                        IdentificadorCliente =cliente.IdentificadorCliente
                        
                    };
                    List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                    listaEncabezado.Add(encabezado);
                    
                    OnCambioProgreso(30);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    List<ReciboEntity> listaRecibos = SaariDB.getListaRecibos(cliente, fechaCorte);
                    if (!string.IsNullOrEmpty(idArrendadora))
                        listaRecibos = listaRecibos.Where(r => r.IDInmobiliaria == idArrendadora).ToList();
                    
                    OnCambioProgreso(80);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    Report report = new Report();
                    report.Load(rutaFormato);
                    report.RegisterData(listaEncabezado, "Encabezado");
                    report.RegisterData(listaRecibos, "Recibo", 3);
                    
                    OnCambioProgreso(90);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                    bandaRecibos.DataSource = report.GetDataSource("Recibo");
                    DataBand bandaAbonos = report.FindObject("Data2") as DataBand;
                    bandaAbonos.DataSource = report.GetDataSource("Recibo.Pagos");
                    
                    OnCambioProgreso(100);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    return exportar(report, esPdf, "AuditoriaCobranza");
                }
                else
                    return "No se encontró el formato: " + Environment.NewLine + rutaFormato;
            }
            catch (Exception ex)
            {
                return "Error al generar el reporte:" + Environment.NewLine + ex.Message;
            }
        }

        public string generar()
        {
            if (clientes != null)
            {
                string resultado = string.Empty, resultadoAcumulado = string.Empty;
                foreach (var cliente in clientes)
                {
                    resultado = generarReporte(cliente);
                    if (!string.IsNullOrWhiteSpace(resultado))
                        resultadoAcumulado += resultado + Environment.NewLine;
                }
                return resultadoAcumulado;
            }
            else
                return "No se encontraron clientes disponibles";
        }
    }
}
