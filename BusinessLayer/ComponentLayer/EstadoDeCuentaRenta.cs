using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using FastReport;
using FastReport.Export.Pdf;
using FastReport.Export.OoXML;
using System.Drawing.Printing;
using System.Net;
using System.Net.Mail;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.Abstracts;
using System.Data;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class EstadoDeCuentaRenta : SaariReport, IReport, IBackgroundReport
    {
        private bool esPdf = true, enviarCorreo = false, imprimir = false;
        private DateTime fechaInicial = new DateTime(), fechaFinal = DateTime.Now;
        private List<ContratosEntity> listaContratos = new List<ContratosEntity>();
        private string nombreInmobiliaria = string.Empty, rutaFormato = string.Empty, usuario = string.Empty;
        private PrinterSettings configuracionImpresora = null;
        public EstadoDeCuentaRenta(bool esPdf, DateTime fechaInicial, DateTime fechaFinal, List<ContratosEntity> listaContratos, string nombreInmobiliaria, string rutaFormato, string usuario, bool enviarCorreo, bool imprimir, PrinterSettings configuracionImpresora)
        {
            this.esPdf = esPdf;
            this.fechaInicial = fechaInicial.Date;
            this.fechaFinal = fechaFinal.Date;
            this.listaContratos = listaContratos;
            this.nombreInmobiliaria = nombreInmobiliaria;
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            this.enviarCorreo = enviarCorreo;
            this.imprimir = imprimir;
            this.configuracionImpresora = configuracionImpresora;
        }

        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();
        }

        public static List<ContratosEntity> obtenerContratos(string idInmobiliaria)
        {
            return SaariDB.getContratosPorInmobiliaria(idInmobiliaria);
        }

        private string generarReporte(ContratosEntity contrato)
        {
            try
            {
                OnCambioProgreso(0);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                if (fechaFinal > fechaInicial)
                {
                    OnCambioProgreso(10);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    EncabezadoEntity encabezado = new EncabezadoEntity()
                    {
                        Inmobiliaria = nombreInmobiliaria,
                        FechaInicio = fechaInicial.ToString("dd/MM/yyyy"),
                        FechaFin = fechaFinal.ToString("dd/MM/yyyy"),
                        Usuario = usuario
                    };
                    List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                    listaEncabezado.Add(encabezado);

                    OnCambioProgreso(20);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    List<ContratosEntity> listaContrato = new List<ContratosEntity>();
                    listaContrato.Add(contrato);
                    
                    DomicilioEntity domcilio = SaariDB.getDomicilioPorCliente(contrato.IDCliente);
                    List<DomicilioEntity> listaDomicilio = new List<DomicilioEntity>();
                    listaDomicilio.Add(domcilio);

                    OnCambioProgreso(30);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    DomicilioEntity domicilioInmueble = SaariDB.getDomicilioPorInmueble(contrato.IDInmueble);
                    List<DomicilioEntity> listaDomicilioInmueble = new List<DomicilioEntity>();
                    listaDomicilioInmueble.Add(domicilioInmueble);

                    OnCambioProgreso(40);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    DepositoEntity deposito = SaariDB.getDeposito(contrato.IDContrato, fechaFinal);
                    List<DepositoEntity> listaDeposito = new List<DepositoEntity>();
                    listaDeposito.Add(deposito);

                    OnCambioProgreso(40);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    List<SaldoEntity> listaSaldosBD = SaariDB.getSaldos(contrato, fechaInicial, fechaFinal);
                    decimal saldoTotal = 0;
                    foreach (SaldoEntity s in listaSaldosBD)
                    {
                        saldoTotal += s.PagoParcial;
                    }                    
                    SaldosEntity saldos = new SaldosEntity()
                    {
                        Saldo = saldoTotal - SaariDB.getSaldoAnterior(contrato, fechaInicial)
                    };
                    List<SaldosEntity> listaSaldos = new List<SaldosEntity>();
                    listaSaldos.Add(saldos);

                    OnCambioProgreso(50);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    List<ReciboEntity> listaRecibos = SaariDB.getListaRecibos(contrato, fechaInicial, fechaFinal);
                    listaRecibos = listaRecibos.OrderBy(r => r.FechaEmision).ToList();
                    


                    OnCambioProgreso(70);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    if (File.Exists(rutaFormato))
                    {                        
                        string rutaGuardar = Properties.Settings.Default.RutaRepositorio;
                        rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + @"EstadoDeCuentaRenta\" : rutaGuardar + @"\EstadoDeCuentaRenta\";
                        if (!System.IO.Directory.Exists(rutaGuardar))
                            System.IO.Directory.CreateDirectory(rutaGuardar);

                        OnCambioProgreso(80);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";

                        Report report = new Report();
                        report.Load(rutaFormato);
                        report.RegisterData(listaEncabezado, "Encabezado");
                        report.RegisterData(listaContrato, "Contrato");
                        report.RegisterData(listaDomicilio, "Domicilio");
                        report.RegisterData(listaDomicilioInmueble, "DomicilioInmueble");
                        report.RegisterData(listaDeposito, "Deposito");
                        report.RegisterData(listaSaldos, "Saldos");
                        report.RegisterData(listaRecibos, "Recibo");
                        DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                        bandaRecibos.DataSource = report.GetDataSource("Recibo");
                        report.Prepare();
                        OnCambioProgreso(90);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        string filename = string.Empty;
                        if (esPdf)
                        {
                            filename = rutaGuardar + @"EstadoDeCuentaRenta_" + contrato.Cliente + "_" + contrato.Inmueble + "_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + ".pdf";
                            PDFExport export = new PDFExport();
                            report.Export(export, filename);
                        }
                        else
                        {
                            filename = rutaGuardar + @"EstadoDeCuentaRenta_" + contrato.Cliente + "_" + contrato.Inmueble + "_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + ".xlsx";
                            Excel2007Export export = new Excel2007Export();
                            report.Export(export, filename);
                        }
                        contrato.RutaArchivo = filename;
                        if (imprimir)
                        {
                            if (configuracionImpresora != null)
                            {
                                report.PrintPrepared(configuracionImpresora);
                            }
                        }
                        report.Dispose();
                        OnCambioProgreso(100);                        
                        return string.Empty;
                    }
                    else
                        return "No se encontro el formato " + rutaFormato + Environment.NewLine;
                }
                else
                    return "La fecha final debe ser mayor a la fecha inicial" + Environment.NewLine;
            }
            catch (Exception ex)
            {
                return "Error general al generar reporte: " + Environment.NewLine + ex.Message + Environment.NewLine;
            }
        }

        public string generar()
        {
            string erroresAcumulados = string.Empty;
            foreach (ContratosEntity contrato in listaContratos)
            {
                string error = generarReporte(contrato);
                if (string.IsNullOrEmpty(error))
                {
                    if (!enviarCorreo)
                    {
                        if (!imprimir)
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(contrato.RutaArchivo);
                            }
                            catch (Exception ex)
                            {
                                error = "Error al abrir el archivo " + contrato.RutaArchivo + " correspondiente a " + contrato.Cliente + Environment.NewLine + ex.Message + Environment.NewLine;
                            }
                        }
                    }
                    else
                    {
                        error = enviarMail(contrato, usuario, nombreInmobiliaria);
                    }
                }
                if(!string.IsNullOrEmpty(error))
                    erroresAcumulados += error + Environment.NewLine;                
            }
            return erroresAcumulados;
        }

        public string enviarMail(ContratosEntity contrato, string usuario, string inmobiliaria)
        {
            try
            {
                SmtpClient cliente = SaariDB.getSmtpCliente(usuario);
                if (cliente != null)
                {
                    if (cliente.Host.ToLower().Contains("gmail") || cliente.Host.ToLower().Contains("live"))
                        cliente.EnableSsl = true;
                    string mailCliente = SaariDB.getMailCliente(contrato.IDCliente);
                    //mailCliente = "alfredo.meza@saari.com.mx";
                    if (!string.IsNullOrEmpty(mailCliente))
                    {
                        if (File.Exists(contrato.RutaArchivo))
                        {
                            MailMessage mensaje = new MailMessage();
                            MailAddress mailRemitente = new MailAddress((cliente.Credentials as NetworkCredential).UserName, inmobiliaria);
                            mensaje.Subject = "Reporte de estado de cuenta de renta";
                            mensaje.Body = "Se adjunta el reporte de estado de cuenta de renta. El reporte se genera automáticamente, cualquier duda o aclaración favor de contactarnos";
                            mensaje.From = mailRemitente;
                            string[] correoDestinatarios = mailCliente.Split(';');
                            foreach (string correoDestinatario in correoDestinatarios)
                            {
                                mensaje.To.Add(correoDestinatario);
                            }
                            Attachment adjunto = new Attachment(contrato.RutaArchivo);
                            mensaje.Attachments.Add(adjunto);
                            cliente.Send(mensaje);
                            return string.Empty;
                        }
                        else
                            return "No se encontró el reporte para ser adjuntado" + Environment.NewLine;
                    }
                    else
                        return "No se encontró correo electrónico del cliente" + Environment.NewLine;
                }
                else
                    return "No se pudo obtener la configuración de correo del usuario" + Environment.NewLine;
            }
            catch(Exception ex)
            {
                return "Error general al enviar correo electrónico: " + Environment.NewLine + ex.Message + Environment.NewLine;
            }
        }
    }
}
