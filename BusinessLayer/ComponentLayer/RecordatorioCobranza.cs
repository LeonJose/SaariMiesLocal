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
    class RecordatorioCobranza : SaariReport, IReport, IBackgroundReport
    {
        private bool esPdf = true, enviarCorreo = false, imprimir = false, esDolar = false;
        private decimal tipoCambioDia = 16.20M;
        private DateTime fechaInicial = new DateTime(); 
        private DateTime fechaFinal = DateTime.Now.Date;
        private List<ClienteEntity> listaClientes = new List<ClienteEntity>();
        private List<ContratosEntity> listaContratos = new List<ContratosEntity>();
        private string rutaFormato = string.Empty, usuario = string.Empty, idArrendadora = string.Empty, nombreArchivo = string.Empty;
        string nombreInmobiliaria = string.Empty;
        bool esPorCliente = true;
        private PrinterSettings configuracionImpresora = null;
        string messageBody = "";

        public override string NombreArchivo
        {
            get
            {
                return nombreArchivo;
            }
        }
        
        public RecordatorioCobranza(bool esPdf, DateTime fechaFinal, List<ContratosEntity> listaContratos, string nombreInmobiliaria, string rutaFormato, string usuario, bool enviarCorreo, bool imprimir, PrinterSettings configuracionImpresora, bool esDolar, bool repPoCliente, string mnjbody)
        {
            this.esPdf = esPdf;
            this.esDolar = esDolar;
            this.fechaInicial = fechaInicial.Date;
            this.fechaFinal = fechaFinal.Date;
            this.listaContratos = listaContratos;            
            this.nombreInmobiliaria = nombreInmobiliaria;
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            this.enviarCorreo = enviarCorreo;
            this.imprimir = imprimir;
            this.configuracionImpresora = configuracionImpresora;
            this.esPorCliente= repPoCliente;
            this.messageBody = mnjbody;
        }
        public RecordatorioCobranza(bool esPdf, DateTime fechaFinal, List<ClienteEntity> listaClientes, string nombreInmobiliaria, string rutaFormato, string usuario, bool enviarCorreo, bool imprimir, PrinterSettings configuracionImpresora, bool esDolar, bool repPoCliente,string mnjbody)
        {
            this.esPdf = esPdf;
            this.esDolar = esDolar;
            this.fechaInicial = fechaInicial.Date;
            this.fechaFinal = fechaFinal.Date;
            this.listaClientes = listaClientes;
            this.nombreInmobiliaria = nombreInmobiliaria;
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            this.enviarCorreo = enviarCorreo;
            this.imprimir = imprimir;
            this.configuracionImpresora = configuracionImpresora;
            this.esPorCliente = repPoCliente;
            this.messageBody = mnjbody;
        }
        public static List<ClienteEntity> obtenerClientes()
        {
            return SaariDB.getClientes();
        }
        private string generarReporte(ContratosEntity contrato)
        {
            string nombreInmo = string.Empty;
            try
            {
                OnCambioProgreso(0);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";               
                OnCambioProgreso(10);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                ClienteEntity cte = SaariDB.getClienteByID(contrato.IDCliente);
                if (nombreInmobiliaria == "TODOS")
                {
                    InmobiliariaEntity inmobil = SaariDB.getInmobiliariaByID(contrato.IDArrendadora);
                    if (inmobil != null)
                    {
                        nombreInmo = inmobil.RazonSocial;
                        contrato.NombreInmobiliaria = nombreInmo;
                    }
                    else
                        return "No se encontró la inmboliaria ligada al cliente " + contrato.Cliente + ".";

                }
                EncabezadoEntity encabezado = new EncabezadoEntity()
                {
                    Inmobiliaria =nombreInmobiliaria=="TODOS" ?contrato.NombreInmobiliaria:nombreInmobiliaria,
                    Cliente= contrato.Cliente,
                    RFC= cte.RFC,
                    Conjunto = contrato.Conjunto+" "+ contrato.Inmueble,
                    FechaFin = fechaFinal.ToString("dd/MM/yyyy"),
                    Moneda= contrato.Moneda,
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

                OnCambioProgreso(50);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                //Moneda del contrato
                esDolar = contrato.Moneda == "D" ? true : false;
                List<ReciboEntity> listaRecibos = SaariDB.getListaRecibosPendientesDeCobro(contrato, fechaFinal, esDolar, contrato.IDArrendadora);
                listaRecibos = listaRecibos.OrderBy(r => r.FechaEmision).ToList();                    
                if (listaRecibos.Count == 0)
                {
                    return "El cliente "+contrato.Cliente +" no presenta adeudos.";
                }
                OnCambioProgreso(70);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                if (File.Exists(rutaFormato))
                {
                    string rutaGuardar = Properties.Settings.Default.RutaRepositorio;
                    if(contrato.TipoContrato=="R")
                        rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + @"CobranzaRenta\" : rutaGuardar + @"\CobranzaRenta\";
                    else
                        rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + @"CobranzaVenta\" : rutaGuardar + @"\CobranzaVenta\";
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
                    //report.RegisterData(listaDeposito, "Deposito");
                    //report.RegisterData(listaSaldos, "Saldos");
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
                        if(contrato.TipoContrato=="R")
                            filename = rutaGuardar + @"CobranzaRenta_" + contrato.Cliente.Replace(',', '_') + "_" + contrato.Inmueble.Replace('/', '_').Replace('\\', '_').Replace(',', '_') + "_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_').Replace(".", "") + ".pdf";
                        else
                            filename = rutaGuardar + @"CobranzaVenta_" + contrato.Cliente.Replace(',', '_') + "_" + contrato.Inmueble.Replace('/', '_').Replace('\\', '_').Replace(',', '_') + "_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_').Replace(".", "") + ".pdf";
                        PDFExport export = new PDFExport();
                        report.Export(export, filename);
                    }
                    else
                    {
                        if (contrato.TipoContrato == "R")
                            filename = rutaGuardar + @"CobranzaRenta_" + contrato.Cliente.Replace(',', '_') + "_" + contrato.Inmueble.Replace('/', '_').Replace('\\', '_').Replace(',', '_') + "_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_').Replace(".", "") + ".xlsx";
                        else
                            filename = rutaGuardar + @"CobranzaVenta_" + contrato.Cliente.Replace(',', '_') + "_" + contrato.Inmueble.Replace('/', '_').Replace('\\', '_').Replace(',', '_') + "_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_').Replace(".", "") + ".xlsx";
                        Excel2007Export export = new Excel2007Export();
                        report.Export(export, filename);
                    }
                    contrato.RutaArchivo = filename;
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
            catch (Exception ex)
            {
                return "Error general al generar reporte: " + Environment.NewLine + ex.Message + Environment.NewLine;
            }
        }
        private string generarReporte(ClienteEntity cliente)
        {
            string nombreInmo = string.Empty;
            try
            {
                OnCambioProgreso(0);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
               
                OnCambioProgreso(10);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";                 
                               
                EncabezadoEntity encabezado = new EncabezadoEntity()
                {
                    Inmobiliaria = nombreInmobiliaria == "TODOS" ? string.Empty : nombreInmobiliaria,
                    Cliente = cliente.Nombre,
                    RFC = cliente.RFC,
                    Conjunto =string.Empty,
                    FechaFin = fechaFinal.ToString("dd/MM/yyyy"),
                    Moneda = esDolar ? "" : "",
                    Usuario = usuario
                };
                List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                listaEncabezado.Add(encabezado);

                OnCambioProgreso(20);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                //List<ContratosEntity> listaContrato = new List<ContratosEntity>();
                //listaContrato.Add(contrato);

                DomicilioEntity domcilio = SaariDB.getDomicilioPorCliente(cliente.IDCliente);
                List<DomicilioEntity> listaDomicilio = new List<DomicilioEntity>();
                listaDomicilio.Add(domcilio);

                //OnCambioProgreso(30);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                //DomicilioEntity domicilioInmueble = SaariDB.getDomicilioPorInmueble(cliente.IDInmueble);
                //List<DomicilioEntity> listaDomicilioInmueble = new List<DomicilioEntity>();
                //listaDomicilioInmueble.Add(domicilioInmueble);

                OnCambioProgreso(30);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                OnCambioProgreso(40);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                List<ReciboEntity> listaRecibos = SaariDB.getListaRecibosPendientesDeCobro(cliente, fechaFinal, esDolar, idArrendadora);
                listaRecibos = listaRecibos.OrderBy(r => r.FechaEmision).ToList();                
                //foreach (ReciboEntity listaR in listaRecibos)
                //{
                //    if (listaR.Moneda == "D")
                //    {
                //        listaR.CantidadPorPagar *= tipoCambioDia;
                //        listaR.IVA *= tipoCambioDia;
                //        listaR.Importe *= tipoCambioDia;
                //        listaR.Total *= tipoCambioDia;
                        
                //    }
                //}
                if (listaRecibos.Count == 0)
                {
                    return "El cliente " + cliente.Nombre + " no presenta adeudos.";
                }
                OnCambioProgreso(70);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                if (File.Exists(rutaFormato))
                {
                    string rutaGuardar = Properties.Settings.Default.RutaRepositorio;
                    if (cliente.TipoFactura == "R")
                        rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + @"CobranzaRenta\" : rutaGuardar + @"\CobranzaRenta\";
                    else
                        rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + @"CobranzaVenta\" : rutaGuardar + @"\CobranzaVenta\";
                    if (!System.IO.Directory.Exists(rutaGuardar))
                        System.IO.Directory.CreateDirectory(rutaGuardar);

                    OnCambioProgreso(80);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    Report report = new Report();
                    report.Load(rutaFormato);
                    report.RegisterData(listaEncabezado, "Encabezado");
                    report.RegisterData(listaClientes, "Cliente");
                    report.RegisterData(listaDomicilio, "Domicilio");
                    //report.RegisterData(listaDomicilioInmueble, "DomicilioInmueble");
                    //report.RegisterData(listaDeposito, "Deposito");
                    //report.RegisterData(listaSaldos, "Saldos");
                    report.RegisterData(listaRecibos, "Recibo");                   
                    DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                    
                    //GroupHeaderBand bandaGrupoRecibos = new GroupHeaderBand();
                    //GroupFooterBand pieDeGrupoRecibos = new GroupFooterBand();
                    //bandaGrupoRecibos.SetName("GroupHeader1");
                    //pieDeGrupoRecibos.SetName("GroupFooter1");

                    bandaRecibos.DataSource = report.GetDataSource("Recibo");
                    report.Prepare();

                    OnCambioProgreso(90);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    string filename = string.Empty;
                    if (esPdf)
                    {
                        if (cliente.TipoFactura == "R")
                            filename = rutaGuardar + @"CobranzaRenta_" + cliente.Nombre.Replace(',', '_') + "_" + cliente.IDCliente +"_"+ DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_').Replace(".", "") + ".pdf";
                        else
                            filename = rutaGuardar + @"CobranzaVenta_" + cliente.Nombre.Replace(',', '_') + "_" + cliente.IDCliente + "_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_').Replace(".", "") + ".pdf";
                        PDFExport export = new PDFExport();
                        report.Export(export, filename);
                    }
                    else
                    {
                        if (cliente.TipoFactura == "R")
                            filename = rutaGuardar + @"CobranzaRenta_" + cliente.Nombre.Replace(',', '_') + "_" + cliente.IDCliente + "_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_').Replace(".", "") + ".xlsx";
                        else
                            filename = rutaGuardar + @"CobranzaVenta_" + cliente.Nombre.Replace(',', '_') + "_" + cliente.IDCliente + "_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_').Replace(".", "") + ".xlsx";
                        Excel2007Export export = new Excel2007Export();
                        report.Export(export, filename);
                    }
                    cliente.RutaArchivo = filename;
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
            catch (Exception ex)
            {
                return "Error general al generar reporte: " + Environment.NewLine + ex.Message + Environment.NewLine;
            }
        }

        public string generar()
        {
            string erroresAcumulados = string.Empty;
            if (esPorCliente)
            {
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
                                    System.Diagnostics.Process.Start(cliente.RutaArchivo);
                                }
                                catch (Exception ex)
                                {
                                    error = "Error al abrir el archivo " + cliente.RutaArchivo + " correspondiente a " + cliente.NombreComercial + Environment.NewLine + ex.Message + Environment.NewLine;
                                }
                            }
                        }
                        else
                        {
                            //TO DO: ENVIAR CORREO
                        }
                    }
                    if (!string.IsNullOrEmpty(error))
                        erroresAcumulados += error + Environment.NewLine;                                        
                }
            }
            else
            {
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
                            error = enviarMail(contrato, usuario, contrato.NombreInmobiliaria);
                        }
                    }
                    if (!string.IsNullOrEmpty(error))
                        erroresAcumulados += error + Environment.NewLine;
                }
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
                            mensaje.Subject = "Reporte de Cobranza de Facturas de Renta Pendientes de Pago";
                            mensaje.Body = messageBody;                            

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
                            mensaje.Subject = "Aviso de Cobranza";
                            mensaje.Body = messageBody;
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
        public static List<ClienteEntity> obtenerClientesPorInmobiliaria(string idInmob)
        {
            return SaariDB.getClientesPorInmobiliaria(idInmob);
        }
        public static List<ClienteEntity> obtenerClientesConFacturaPorInmobiliaria(string idInmo)
        {
            return SaariDB.getAllClientesPorInmobiliaria(idInmo);
        }
        /// <summary>
        /// Obtener contratos por Inmobiliaria y por tipo de contrato
        /// </summary>
        /// <param name="idInmobiliaria"></param>
        /// <param name="tipoContrato"> R=Renta - V=Venta</param>
        /// <returns></returns>
        public static List<ContratosEntity> obtenerContratos(string idInmobiliaria, string tipoContrato)
        {
            return SaariDB.getContratosPorInmobiliariaYTipo(idInmobiliaria, tipoContrato);
        }
        public static void generaLog(string errores, string tipoFactura)
        {
            try
            {
                string rutaGuardar = Properties.Settings.Default.RutaRepositorio;
                string texto = string.Empty;
                if (tipoFactura == "R")
                    rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + @"CobranzaRenta\CobranzaRentaLog\" : rutaGuardar + @"\CobranzaRenta\CobranzaRentaLog\";
                else
                    rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + @"CobranzaVenta\CobranzaVentaLog\" : rutaGuardar + @"\CobranzaVenta\CobranzaVentaLog\";
                if (!System.IO.Directory.Exists(rutaGuardar))
                    System.IO.Directory.CreateDirectory(rutaGuardar);                                
                if (!string.IsNullOrEmpty(errores))
                {
                    texto += "Clientes sin adeudo con el tipo de reporte seleccionado o con problemas para generar aviso de cobranza: " + Environment.NewLine + Environment.NewLine + errores + Environment.NewLine;
                }
                string fileName = rutaGuardar +"log_"+ DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
                TextWriter tw = new StreamWriter(fileName, true);
                tw.WriteLine(texto);
                tw.Close();
                System.Diagnostics.Process.Start(fileName);
            }
            catch
            {
            }
        } 

    }
}
