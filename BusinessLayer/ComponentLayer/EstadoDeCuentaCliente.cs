using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Printing;
using System.Net.Mail;
using System.Net;
using FastReport;
using FastReport.Export.Pdf;
using FastReport.Export.OoXML;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class EstadoDeCuentaCliente : SaariReport, IReport, IBackgroundReport
    {
        private bool esPdf = true, enviarCorreo = false, imprimir = false, esDolar = false;
        private DateTime fechaInicial = new DateTime(), fechaFinal = DateTime.Now.Date;
        private List<ClienteEntity> listaClientes = new List<ClienteEntity>();
        private string rutaFormato = string.Empty, usuario = string.Empty, idArrendadora = string.Empty, nombreArchivo = string.Empty;
        private PrinterSettings configuracionImpresora = null;

        public override string NombreArchivo
        {
            get
            {
                return nombreArchivo;
            }
        }

        public EstadoDeCuentaCliente(bool esPdf, DateTime fechaInicial, DateTime fechaFinal, List<ClienteEntity> listaClientes, string rutaFormato, string usuario, bool enviarCorreo, bool imprimir, PrinterSettings configuracionImpresora, bool esDolar, string idArrendadora)
        {
            this.esPdf = esPdf;
            this.fechaInicial = fechaInicial.Date;
            this.fechaFinal = fechaFinal.Date;
            this.listaClientes = listaClientes;
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            this.enviarCorreo = enviarCorreo;
            this.imprimir = imprimir;
            this.configuracionImpresora = configuracionImpresora;
            this.esDolar = esDolar;
            this.idArrendadora = idArrendadora;
        }        
        
        public static List<ClienteEntity> obtenerClientes()
        {
            return SaariDB.getClientes();
        }

        private string generarReporte(ClienteEntity cliente)
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

                    string nombreMoneda = esDolar ? "Dolar" : obtenerMonedaLocal();
                    string nombreInmobiliaria = string.Empty;
                    if (!string.IsNullOrEmpty(idArrendadora))
                        nombreInmobiliaria = SaariDB.getNombreInmobiliariaByID(idArrendadora);
                    OnCambioProgreso(20);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    EncabezadoEntity encabezado = new EncabezadoEntity()
                    {
                        FechaInicio = fechaInicial.ToString("dd/MM/yyyy"),
                        FechaFin = fechaFinal.ToString("dd/MM/yyyy"),
                        Usuario = usuario,
                        Cliente = cliente.Nombre,
                        RFC = cliente.RFC,
                        Moneda = nombreMoneda,
                        Inmobiliaria = nombreInmobiliaria
                    };
                    List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                    listaEncabezado.Add(encabezado);
                    OnCambioProgreso(30);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    
                    DomicilioEntity domcilio = SaariDB.getDomicilioPorCliente(cliente.IDCliente);
                    List<DomicilioEntity> listaDomicilio = new List<DomicilioEntity>();
                    listaDomicilio.Add(domcilio);
                    OnCambioProgreso(40);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    List<SaldoEntity> listaSaldosBD = SaariDB.getSaldos(cliente, fechaInicial, fechaFinal, esDolar);
                    if (!string.IsNullOrEmpty(idArrendadora))
                        listaSaldosBD = listaSaldosBD.Where(s => s.IDInmobiliaria == idArrendadora).ToList();
                    decimal saldoTotal = 0;
                    foreach (SaldoEntity s in listaSaldosBD)
                    {
                        saldoTotal += s.PagoParcial;
                    }
                    SaldosEntity saldos = new SaldosEntity()
                    {
                        Saldo = saldoTotal - SaariDB.getSaldoAnterior(cliente, fechaInicial, esDolar, idArrendadora)
                    };
                    List<SaldosEntity> listaSaldos = new List<SaldosEntity>();
                    listaSaldos.Add(saldos);
                    OnCambioProgreso(50);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    List<ReciboEntity> listaRecibos = SaariDB.getListaRecibos(cliente, fechaInicial, fechaFinal, esDolar);
                    if (!string.IsNullOrEmpty(idArrendadora))
                        listaRecibos = listaRecibos.Where(r => r.IDInmobiliaria == idArrendadora).ToList();
                    OnCambioProgreso(60);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    if (File.Exists(rutaFormato))
                    {
                        string rutaGuardar = Properties.Settings.Default.RutaRepositorio;
                        rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + @"EstadoDeCuentaCliente\" : rutaGuardar + @"\EstadoDeCuentaCliente\";
                        if (!System.IO.Directory.Exists(rutaGuardar))
                            System.IO.Directory.CreateDirectory(rutaGuardar);
                        OnCambioProgreso(70);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        Report report = new Report();
                        report.Load(rutaFormato);
                        report.RegisterData(listaEncabezado, "Encabezado");                        
                        report.RegisterData(listaDomicilio, "Domicilio");
                        report.RegisterData(listaSaldos, "Saldos");
                        report.RegisterData(listaRecibos, "Recibo");
                        DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                        bandaRecibos.DataSource = report.GetDataSource("Recibo");
                        report.Prepare();
                        OnCambioProgreso(80);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        string filename = string.Empty;
                        if (esPdf)
                        {
                            filename = rutaGuardar + @"EstadoDeCuentaCliente_" + cliente.Nombre + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                            PDFExport export = new PDFExport();
                            report.Export(export, filename);
                        }
                        else
                        {
                            filename = rutaGuardar + @"EstadoDeCuentaCliente_" + cliente.Nombre + "_" +DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                            Excel2007Export export = new Excel2007Export();
                            report.Export(export, filename);
                        }
                        OnCambioProgreso(90);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        nombreArchivo = filename;
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
            foreach (ClienteEntity cliente in listaClientes)
            {
                string error = generarReporte(cliente);
                if (string.IsNullOrEmpty(error))
                {
                    if (!enviarCorreo)
                    {
                        if (!imprimir)
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(nombreArchivo);
                            }
                            catch (Exception ex)
                            {
                                error = "Error al abrir el archivo " + nombreArchivo + " correspondiente a " + cliente.Nombre + Environment.NewLine + ex.Message + Environment.NewLine;
                            }
                        }
                    }
                    else
                    {
                        error = enviarMail(cliente, usuario);
                    }
                }
                if (!string.IsNullOrEmpty(error))
                    erroresAcumulados += error + Environment.NewLine;
            }
            return erroresAcumulados;
        }

        public string enviarMail(ClienteEntity clienteDB, string usuario)
        {
            try
            {
                SmtpClient cliente = SaariDB.getSmtpCliente(usuario);
                if (cliente != null)
                {
                    if (cliente.Host.ToLower().Contains("gmail") || cliente.Host.ToLower().Contains("live"))
                        cliente.EnableSsl = true;
                    string mailCliente = SaariDB.getMailCliente(clienteDB.IDCliente);
                    //mailCliente = "alfredo.meza@saari.com.mx";//para pruebas
                    if (!string.IsNullOrEmpty(mailCliente))
                    {
                        if (File.Exists(nombreArchivo))
                        {
                            MailMessage mensaje = new MailMessage();
                            MailAddress mailRemitente = new MailAddress((cliente.Credentials as NetworkCredential).UserName);
                            mensaje.Subject = "Reporte de estado de cuenta de cliente";
                            mensaje.Body = "Se adjunta el reporte de estado de cuenta de cliente. El reporte se genera automáticamente, cualquier duda o aclaración favor de contactarnos";
                            mensaje.From = mailRemitente;
                            string[] correoDestinatarios = mailCliente.Split(';');
                            foreach (string correoDestinatario in correoDestinatarios)
                            {
                                mensaje.To.Add(correoDestinatario);
                            }
                            Attachment adjunto = new Attachment(nombreArchivo);
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
            catch (Exception ex)
            {
                return "Error general al enviar correo electrónico: " + Environment.NewLine + ex.Message + Environment.NewLine;
            }
        }

        public static string obtenerMonedaLocal()
        {
            return SaariDB.getMonedaLocal();
        }

        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();
        }
    }
}
