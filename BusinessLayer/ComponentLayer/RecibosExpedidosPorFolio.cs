using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.Helpers;
using System.IO;
using FastReport;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class RecibosExpedidosPorFolio : SaariReport, IReport, IBackgroundReport
    {
        private string idInmobiliaria = string.Empty, idConjunto = string.Empty, usuario = string.Empty, rutaFormato = string.Empty;
        private DateTime fechaInicio = new DateTime(), fechaFin = new DateTime();
        private bool esPdf = true, incluyeDetalleConceptos = false;
        private bool esVentas = false;
        private bool esParcialidad=true;

        public RecibosExpedidosPorFolio(string idInmobiliaria, string idConjunto, DateTime fechaInicio, DateTime fechaFin, string usuario, string rutaFormato, bool esPdf, bool incluyeDetalleConceptos)
        {
            this.idInmobiliaria = idInmobiliaria;
            this.idConjunto = idConjunto;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
            this.usuario = usuario;
            this.rutaFormato = rutaFormato;
            this.esPdf = esPdf;
            this.incluyeDetalleConceptos = incluyeDetalleConceptos;
        }

        public RecibosExpedidosPorFolio(string idInmobiliaria, string idConjunto, DateTime fechaInicio, DateTime fechaFin, string usuario, string rutaFormato, bool esPdf, bool incluyeDetalleConceptos, bool esVentas, bool esParcialidad)
        {
            this.idInmobiliaria = idInmobiliaria;
            this.idConjunto = idConjunto;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
            this.usuario = usuario;
            this.rutaFormato = rutaFormato;
            this.esPdf = esPdf;
            this.incluyeDetalleConceptos = incluyeDetalleConceptos;
            this.esVentas = esVentas;
            this.esParcialidad = esParcialidad;
        }

        //public static List<InmobiliariaEntity> obtenerInmobiliarias(string usuario)
        //{
        //    List<InmobiliariaEntity> ListaInmobiliarias = ObtenerInmobiliarias.obtenerInmobiliarias(usuario);

        //   return ListaInmobiliarias;
        //}

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
                        List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                        List<ConceptoEntity> listaConceptos = new List<ConceptoEntity>();
                        List<ConceptoEntity> listaTotalesConceptos = new List<ConceptoEntity>();
                        if (idConjunto == "Todos")
                            listaRecibos = SaariDB.getRecibosExpedidosPorFolio(idInmobiliaria, fechaInicio, fechaFin, incluyeDetalleConceptos);
                        else
                            listaRecibos = SaariDB.getRecibosExpedidosPorFolio(idInmobiliaria, idConjunto, fechaInicio, fechaFin, incluyeDetalleConceptos);

                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        OnCambioProgreso(10);

                        if (listaRecibos != null)
                        {
                            if (listaRecibos.Count > 0)
                            {
                                if (incluyeDetalleConceptos )
                                {
                                    decimal factor = 30 / listaRecibos.Count;
                                    factor = factor >= 1 ? factor : 1;
                                    int progreso = 10;
                                    foreach (ReciboEntity recibo in listaRecibos)
                                    {
                                        if (recibo.Conceptos.Count > 0)
                                        {
                                            foreach (ConceptoEntity concepto in recibo.Conceptos)
                                            {
                                                concepto.Importe = concepto.Total - concepto.IVA;
                                                listaConceptos.Add(concepto);
                                            }
                                        }
                                        if (progreso < 40)
                                            progreso += (int)factor;
                                        if (CancelacionPendiente)
                                            return "Proceso cancelado por el usuario";
                                        OnCambioProgreso(progreso);
                                    }
                                    if (listaConceptos.Count > 0)
                                    {
                                        decimal factorConceptos = 30 / listaConceptos.Count;
                                        factorConceptos = factorConceptos >= 1 ? factorConceptos : 1;
                                        int progresoConceptos = 40;
                                        var catalogoConceptos = listaConceptos.GroupBy(c => c.Concepto).Select(c => c.ToList()).ToList();
                                        foreach (var concepto in catalogoConceptos)
                                        {
                                            string descripcionConcepto = concepto.Select(c => c.Concepto).FirstOrDefault();
                                            decimal importe = listaConceptos.Where(c => c.Concepto.Equals(descripcionConcepto)).Sum(c => c.Importe);
                                            decimal iva = listaConceptos.Where(c => c.Concepto.Equals(descripcionConcepto)).Sum(c => c.IVA);
                                            decimal total = listaConceptos.Where(c => c.Concepto.Equals(descripcionConcepto)).Sum(c => c.Total);
                                            ConceptoEntity totalConcepto = new ConceptoEntity();
                                            totalConcepto.Concepto = descripcionConcepto.ToUpper();
                                            totalConcepto.Importe = importe;
                                            totalConcepto.IVA = iva;
                                            totalConcepto.Total = total;
                                            listaTotalesConceptos.Add(totalConcepto);

                                            if (progresoConceptos < 80)
                                                progresoConceptos += (int)factorConceptos;
                                            if (CancelacionPendiente)
                                                return "Proceso cancelado por el usuario";
                                            OnCambioProgreso(progresoConceptos);
                                        }
                                        listaTotalesConceptos = listaTotalesConceptos.OrderBy(c => c.Concepto).ToList();
                                    }
                                }
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";
                                OnCambioProgreso(85);

                                TotalesRecibosExpedidosEntity totales = new TotalesRecibosExpedidosEntity();
                                foreach (ReciboEntity recibo in listaRecibos)
                                {
                                    if (recibo.Moneda.Equals("D"))
                                    {
                                        totales.ImporteDolares += recibo.Importe;
                                        totales.IVADolares += recibo.IVA;
                                        totales.ISRDolares += recibo.ISR;
                                        totales.IVARetenidoDolares += recibo.IVARetenido;
                                        totales.TotalDolares += recibo.Total;

                                        totales.ImporteDolaresAPesos += recibo.Importe * recibo.TipoCambio;
                                        totales.IVADolaresAPesos += recibo.IVA * recibo.TipoCambio;
                                        totales.ISRDolaresAPesos += recibo.ISR * recibo.TipoCambio;
                                        totales.IVARetenidoDolaresAPesos += recibo.IVARetenido * recibo.TipoCambio;
                                        totales.TotalDolaresAPesos += recibo.Total * recibo.TipoCambio;
                                    }
                                    else
                                    {
                                        totales.ImportePesos += recibo.Importe;
                                        totales.IVAPesos += recibo.IVA;
                                        totales.ISRPesos += recibo.ISR;
                                        totales.IVARetenidoPesos += recibo.IVARetenido;
                                        totales.TotalPesos += recibo.Total;
                                    }
                                }
                                List<TotalesRecibosExpedidosEntity> listaTotales = new List<TotalesRecibosExpedidosEntity>();
                                listaTotales.Add(totales);

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

                                Report report = new Report();
                                report.Load(rutaFormato);
                                report.RegisterData(listaEncabezado, "Encabezado");
                                report.RegisterData(listaRecibos, "Recibo", 3);
                                report.RegisterData(listaTotales, "Total");
                                report.RegisterData(listaTotalesConceptos, "TotalConcepto");

                                DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                                bandaRecibos.DataSource = report.GetDataSource("Recibo");

                                DataBand bandaConceptos = report.FindObject("Data2") as DataBand;
                                bandaConceptos.DataSource = report.GetDataSource("Recibo.Conceptos");

                                DataBand bandaTotalesConceptos = report.FindObject("Data3") as DataBand;
                                bandaTotalesConceptos.DataSource = report.GetDataSource("TotalConcepto");

                                ReportSummaryBand bandaTotales = report.FindObject("ReportSummary1") as ReportSummaryBand;

                                if (!incluyeDetalleConceptos)
                                {
                                    bandaConceptos.Visible = false;
                                    bandaConceptos.Delete();
                                }

                                OnCambioProgreso(95);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";
                                return exportar(report, esPdf, "RecibosExpedidosPorFolio");
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

        public static string ReducirEspaciado(string Cadena)
        {
            if (Cadena != null)
            {
                while (Cadena.Contains("  ") || Cadena.Contains("   "))
                {
                    if (Cadena.Contains("  "))
                    {
                        Cadena = Cadena.Replace("  ", " ");
                    }
                    else if (Cadena.Contains("   ")) 
                    {
                        Cadena = Cadena.Replace("   ", " ");
                    }
                }

                return Cadena;
            }
            else
                return "";
        }

        public string generarReporte()
        {
            string TipoRecibos = string.Empty;
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
                        List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                        List<ConceptoEntity> listaConceptos = new List<ConceptoEntity>();
                        List<ConceptoEntity> listaTotalesConceptos = new List<ConceptoEntity>();
                        if (idConjunto == "Todos")
                        {
                            if (!esVentas)
                            {
                                listaRecibos = SaariDB.getRecibosExpedidosPorFolio(idInmobiliaria, fechaInicio, fechaFin, incluyeDetalleConceptos);
                                TipoRecibos = "";
                            }
                            else
                            {
                                listaRecibos = SaariDB.getRecibosExpedidosPorFolio(idInmobiliaria, fechaInicio, fechaFin, false, esVentas, esParcialidad);
                                if (esParcialidad)
                                    TipoRecibos = "de Ventas en Parcialidad";
                                else
                                    TipoRecibos = "de Ventas en una sola exhibición";
                            }
                        }
                        else                            
                        {
                            if (!esVentas)
                            {
                                listaRecibos = SaariDB.getRecibosExpedidosPorFolio(idInmobiliaria, idConjunto, fechaInicio, fechaFin, incluyeDetalleConceptos);
                                TipoRecibos = "";
                            }
                            else
                            {
                                listaRecibos = SaariDB.getRecibosExpedidosPorFolio(idInmobiliaria, idConjunto, fechaInicio, fechaFin, false, esVentas, esParcialidad);
                                if (esParcialidad)
                                    TipoRecibos = "de Ventas en Parcialidad";
                                else
                                    TipoRecibos = "de Ventas en una sola exhibición";
                            }                        
                        }
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        OnCambioProgreso(10);

                        if (listaRecibos != null)
                        {
                            if (listaRecibos.Count > 0)
                            {
                                if (incluyeDetalleConceptos)
                                {
                                    decimal factor = 30 / listaRecibos.Count;
                                    factor = factor >= 1 ? factor : 1;
                                    int progreso = 10;
                                    foreach (ReciboEntity recibo in listaRecibos)
                                    {
                                        if (recibo.Conceptos.Count > 0)
                                        {
                                            foreach (ConceptoEntity concepto in recibo.Conceptos)
                                            {
                                                concepto.Importe = concepto.Total - concepto.IVA;
                                                concepto.Moneda = recibo.Moneda;
                                                concepto.TC = recibo.TipoCambio;
                                                listaConceptos.Add(concepto);
                                            }
                                        }
                                        if (progreso < 40)
                                            progreso += (int)factor;
                                        if (CancelacionPendiente)
                                            return "Proceso cancelado por el usuario";
                                        OnCambioProgreso(progreso);
                                    }
                                    if (listaConceptos.Count > 0)
                                    {
                                        int co = 0;//Se agrega este este foreach para reducir posibles espacios 
                                        //dobles o triples en el concepto para evitar duplicidad de los mismos.
                                        foreach (ConceptoEntity l in listaConceptos)
                                        {
                                            listaConceptos[co].Concepto = ReducirEspaciado(l.Concepto);
                                            co++;
                                        }
                                        decimal factorConceptos = 30 / listaConceptos.Count;
                                        factorConceptos = factorConceptos >= 1 ? factorConceptos : 1;
                                        int progresoConceptos = 40;
                                        var catalogoConceptos = listaConceptos.GroupBy(c => c.Concepto.Trim()).Select(c => c.ToList()).ToList();
                                        foreach (var concepto in catalogoConceptos)
                                        {
                                         
                                             string descripcionConcepto = concepto.Select(c => c.Concepto).FirstOrDefault().Trim();
                                             List<ConceptoEntity> importeList  = listaConceptos.Where(c => c.Concepto.Trim().Equals(descripcionConcepto.Trim())).ToList();//;.Sum(c => c.Importe);
                                             List<ConceptoEntity> ivaList = listaConceptos.Where(c => c.Concepto.Trim().Equals(descripcionConcepto.Trim())).ToList();//.Sum(c => c.IVA);
                                             List<ConceptoEntity> totalList = listaConceptos.Where(c => c.Concepto.Trim().Equals(descripcionConcepto.Trim())).ToList();//.Sum(c => c.Total);
                                             decimal importe = 0;
                                            decimal iva = 0;
                                            decimal total =0;
                                            foreach (ConceptoEntity i in importeList)
                                            {
                                                //if (i.Moneda == "D")
                                                //{
                                                //    i.Importe = i.Importe * i.TC;
                                                //    i.IVA = i.IVA * i.TC;
                                                //    i.Total = i.Total * i.TC;
                                                //}
                                                // Revisar Esta parte para la conversion a dolares.
                                                // Se realizo esto porque en la sumatoria de detalle x conceptos se
                                                /*esta sumando dolares y pesos sin realizar conversion.
                                                 * Al hacer esto se hace la convercion pero en el detalle
                                                 * se muestra la convercion de la factura a pesos...
                                                 */
                                                importe += i.Importe;
                                                iva += i.IVA;
                                                total += i.Total;                                                
                                            }
                                           
                                           
                                            ConceptoEntity totalConcepto = new ConceptoEntity();
                                            totalConcepto.Concepto = descripcionConcepto.ToUpper();
                                            totalConcepto.Importe = importe;
                                            totalConcepto.IVA = iva;
                                            totalConcepto.Total = total;

                                            listaTotalesConceptos.Add(totalConcepto);

                                            if (progresoConceptos < 80)
                                                progresoConceptos += (int)factorConceptos;
                                            if (CancelacionPendiente)
                                                return "Proceso cancelado por el usuario";
                                            OnCambioProgreso(progresoConceptos);
                                        }
                                        listaTotalesConceptos = listaTotalesConceptos.OrderBy(c => c.Concepto).ToList();
                                    }
                                }
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";
                                OnCambioProgreso(85);

                                TotalesRecibosExpedidosEntity totales = new TotalesRecibosExpedidosEntity();
                                foreach (ReciboEntity recibo in listaRecibos)
                                {
                                    if (recibo.Moneda.Equals("D"))
                                    {
                                        totales.ImporteDolares += recibo.Importe;
                                        totales.IVADolares += recibo.IVA;
                                        totales.ISRDolares += recibo.ISR;
                                        totales.IVARetenidoDolares += recibo.IVARetenido;
                                        totales.TotalDolares += recibo.Total;

                                        totales.ImporteDolaresAPesos += recibo.Importe * recibo.TipoCambio;
                                        totales.IVADolaresAPesos += recibo.IVA * recibo.TipoCambio;
                                        totales.ISRDolaresAPesos += recibo.ISR * recibo.TipoCambio;
                                        totales.IVARetenidoDolaresAPesos += recibo.IVARetenido * recibo.TipoCambio;
                                        totales.TotalDolaresAPesos += recibo.Total * recibo.TipoCambio;
                                    }
                                    else
                                    {
                                        totales.ImportePesos += recibo.Importe;
                                        totales.IVAPesos += recibo.IVA;
                                        totales.ISRPesos += recibo.ISR;
                                        totales.IVARetenidoPesos += recibo.IVARetenido;
                                        totales.TotalPesos += recibo.Total;
                                    }
                                }
                                List<TotalesRecibosExpedidosEntity> listaTotales = new List<TotalesRecibosExpedidosEntity>();
                                listaTotales.Add(totales);

                                string nombreInmobiliaria = SaariDB.getNombreInmobiliariaByID(idInmobiliaria);
                                string nombreConjunto = SaariDB.getNombreConjuntoByID(idConjunto);
                                if (string.IsNullOrEmpty(nombreConjunto))
                                    nombreConjunto = "Todos los Conjuntos";
                                EncabezadoEntity encabezado = new EncabezadoEntity()
                                {
                                    Inmobiliaria = nombreInmobiliaria,
                                    Conjunto = nombreConjunto,
                                    FechaInicio = fechaInicio.ToString("dd/MM/yyyy"),
                                    FechaFin = fechaFin.ToString("dd/MM/yyyy"),
                                    Usuario = usuario,
                                    TituloTipoRecibos= TipoRecibos
                                };
                                List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                                listaEncabezado.Add(encabezado);

                                Report report = new Report();
                                report.Load(rutaFormato);
                                report.RegisterData(listaEncabezado, "Encabezado");
                                report.RegisterData(listaRecibos, "Recibo", 3);
                                report.RegisterData(listaTotales, "Total");
                                report.RegisterData(listaTotalesConceptos, "TotalConcepto");

                                DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                                bandaRecibos.DataSource = report.GetDataSource("Recibo");

                                DataBand bandaConceptos = report.FindObject("Data2") as DataBand;
                                bandaConceptos.DataSource = report.GetDataSource("Recibo.Conceptos");

                                DataBand bandaTotalesConceptos = report.FindObject("Data3") as DataBand;
                                bandaTotalesConceptos.DataSource = report.GetDataSource("TotalConcepto");

                                ReportSummaryBand bandaTotales = report.FindObject("ReportSummary1") as ReportSummaryBand;

                                if (!incluyeDetalleConceptos)
                                {
                                    bandaConceptos.Visible = false;
                                    bandaConceptos.Delete();
                                }

                                OnCambioProgreso(95);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";
                                if(esVentas)
                                    return exportar(report, esPdf, "RecibosVentasExpedidosPorFolio");
                                else
                                    return exportar(report, esPdf, "RecibosExpedidosPorFolio");
                                
                                    
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

    }
}
