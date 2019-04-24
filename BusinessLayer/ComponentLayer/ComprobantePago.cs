using GestorReportes.BusinessLayer.Abstracts;
using System;
using System.Collections.Generic;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using System.ComponentModel;
using FastReport;
using System.Linq;
using FastReport.Export.Pdf;
using FastReport.Export.OoXML;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class ComprobantePago : SaariReport
    {
        string idConjunto = string.Empty;
        string usuario = string.Empty;
        string rutaFormato = string.Empty;
        private DateTime fechaInicio = new DateTime();
        private DateTime fechaFin = DateTime.Now.Date;
        BackgroundWorker Wkr = null;
        InmobiliariaEntity Inmobiliaria;
        private ConjuntoEntity conjunt;
        private bool rbEXCEL=false;
        private bool rbPDF=true;
        private int IdComprobante;
        private bool esDetalle;
        public bool CRPs = true;

        public ComprobantePago()
        {
        }

        public ComprobantePago(InmobiliariaEntity inmobiliaria, ConjuntoEntity conjunt, DateTime fechaInicio, DateTime fechaFin, string usuario, string rutaFormato, bool rbPDF, BackgroundWorker Worker, bool esDetalle)
        {
            this.Inmobiliaria = inmobiliaria;
            this.conjunt = conjunt;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
            this.usuario = usuario;
            this.rutaFormato = rutaFormato;
            this.rbPDF = rbPDF;
            this.Wkr = Worker;
            this.esDetalle = esDetalle;

        }

       public string generarReporte()
       {
            string error = string.Empty;

            try
            {
                CRPs = Properties.Settings.Default.CRPs;
            }
            catch (Exception ex)
            {
                CRPs = true;
            }

            try
            {
                var listComprobantes = new List<ComprobantePagoEntity>();

                List<ClienteEntity> clientes = HelpInmobiliarias.GetClientesReportComprobantePago(fechaInicio, fechaFin);

                OnCambioProgreso(10);

                listComprobantes = SaariE.GetComprobantesPago(Inmobiliaria, fechaInicio, fechaFin, rbPDF, CRPs);

                OnCambioProgreso(30);

                var DocRelacionados = new List<DocumentosRelacionados>();
                if (CRPs)
                {
                    DocRelacionados = SaariE.GetDocumentosRelacionados(CRPs, fechaInicio, fechaFin);
                }
                else
                {
                    DocRelacionados = SaariE.GetDocumentosRelacionados(CRPs, fechaInicio, fechaFin);
                }
                foreach (ComprobantePagoEntity comprobante in listComprobantes)
                {
                    try
                    {

                        comprobante.NombreReceptor = clientes.Where(c => c.IDCliente == comprobante.IDCliente && c.IDHistRec == comprobante.IdHistRec).FirstOrDefault().Nombre;
                        comprobante.Periodo = clientes.Where(c => c.IDCliente == comprobante.IDCliente && c.IDHistRec == comprobante.IdHistRec).FirstOrDefault().PeriodoFacturacion;                        
                        if (!CRPs)
                        {
                            comprobante.Descripcion = clientes.Where(c => c.IDCliente == comprobante.IDCliente && c.IDHistRec == comprobante.IdHistRec).FirstOrDefault().Concepto;
                        }

                    }
                    catch(Exception ex)
                    {
                        comprobante.NombreReceptor = string.Empty;
                        comprobante.Periodo = string.Empty;
                    }
                    

                    if (CRPs)
                    {                        
                        //DocRelacionados = SaariE.GetDocumentosRelacionados(comprobante.IdComprobante, CRPs);
                        var docsRel = DocRelacionados.Where(d => d.IdComprobante == comprobante.IdComprobante).ToList();
                        comprobante.DocsRelacionados = docsRel;
                    }
                    else
                    {
                        //DocRelacionados = SaariE.GetDocumentosRelacionados(comprobante.IdCFD, CRPs);
                        var docsRel = DocRelacionados.Where(d => d.ID_CFD == comprobante.IdCFD).ToList();
                        comprobante.DocsRelacionados = docsRel;
                    }

                    //comprobante.DocsRelacionados = DocRelacionados;
                    
                }
                OnCambioProgreso(70);
                List<ConjuntoInmobiliaria> conjuntoInmobiliarias = HelpInmobiliarias.GetConjuntoInmobiliarias(Inmobiliaria.ID, conjunt.ID, fechaInicio, fechaFin);

                List<ComprobantePagoEntity> listaComprobantesFiltrados= new List<ComprobantePagoEntity>();

                //ComprobantePagoEntity registro = listComprobantes.Where(p => p.IdHistRec== "1538").FirstOrDefault();

                foreach (ComprobantePagoEntity comp in listComprobantes)//asta aqui tod bien.
                {
                    ConjuntoInmobiliaria clienteEnConjunto = conjuntoInmobiliarias.Where(c => c.IDPago == comp.IdPago && c.IdHistRec == comp.IdHistRec).FirstOrDefault();

                    if (clienteEnConjunto != null)
                    {
                        comp.Conjunto = clienteEnConjunto.Conjunto;
                        comp.IdConjunto = clienteEnConjunto.IDConjunto;
                        listaComprobantesFiltrados.Add(comp); 
                    }
                }
                List<ComprobantePagoEntity> listaComprobantesOrdenados = new List<ComprobantePagoEntity>();
                //ComprobantePagoEntity registro2 = listComprobantes.Where(p => p.IdHistRec == "1538").FirstOrDefault(); ;

                if (listaComprobantesFiltrados != null)
                {
                    if (listaComprobantesFiltrados.Count == 0)
                    {
                        error = "No existen movimientos";
                        return error;
                    }
                    else
                    {
                        listaComprobantesOrdenados = listaComprobantesFiltrados.OrderBy(p=>p.Serie).ThenBy(p=>p.Folio).ToList();
                    }
                }
                
                OnCambioProgreso(80);

                List<EncabezadoEntity> encabezado = new List<EncabezadoEntity>();
                EncabezadoEntity objEnc = new EncabezadoEntity();

                objEnc.Inmobiliaria = Inmobiliaria.NombreComercial;
                objEnc.Conjunto = conjunt.Nombre;
                objEnc.Usuario = usuario;
                objEnc.FechaInicio = fechaInicio.ToString("dd/MM/yyyy");
                objEnc.FechaFin = fechaFin.ToString("dd/MM/yyyy");
                encabezado.Add(objEnc);
                OnCambioProgreso(50);

                Report Report = new Report();
                Report.Load(rutaFormato);             
                Report.RegisterData(encabezado, "Encabezado");
                Report.RegisterData(listaComprobantesOrdenados, "Comprobante", 2);
                //Report.RegisterData(listaComprobantesFiltrados, "Comprobante", 3);

                DataBand bandDetalle = Report.FindObject("Data1") as DataBand;
                bandDetalle.DataSource = Report.GetDataSource("Comprobante");

                DataBand bandaDocs = Report.FindObject("Docs") as DataBand;
                bandaDocs.DataSource = Report.GetDataSource("Comprobante.DocsRelacionados");

                if(conjunt.ID!= "CTRT")
                {
                    try
                    {
                        TextObject textEtConjunto = Report.FindObject("Text84") as TextObject;
                        TextObject textConjunto = Report.FindObject("Text85") as TextObject;
                        textEtConjunto.Visible = false;
                        textConjunto.Visible = false;
                        TextObject textEtNombreCte = Report.FindObject("Text23") as TextObject;
                        TextObject textNombreCte = Report.FindObject("Text16") as TextObject;
                        textEtNombreCte.Left = 321.3f;
                        
                        textNombreCte.Left = 321.3f;
                        textEtNombreCte.Width = 349.65f;
                        textNombreCte.Width = 349.65f;
                    }
                    catch(Exception ex)
                    {
                    }
                }

                //Report report = new Report();
                //report.Load(rutaFormato);
                //report.RegisterData(listaEncabezado, "Encabezado");
                //report.RegisterData(listaCartera, "Recibo", 3);

                //DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                //bandaRecibos.DataSource = report.GetDataSource("Recibo");

                //DataBand bandaFacturas = Report.FindObject("Data2") as DataBand;
                //bandaFacturas.DataSource = Report.GetDataSource("Recibo.Recibos");
                //Report.RegisterData(listaComprobantesFiltrados, "Detalle.documentosrelacionados");

                OnCambioProgreso(90);

                //bandDetalle2.DataSource = Report.GetDataSource("Detalle.documentosrelacionados");

                if (Wkr.CancellationPending)
                    return "Proceso cancelado por el Usuario";


                if (!esDetalle)
                    bandaDocs.Visible = false;
                //bandDetalle2.DataSource = Report.GetDataSource("Comprobante.documentosrelacionados");


                error = exportar(Report, rbPDF, "InformeComprobantesDePago");
                
            }
            catch(Exception ex)
            {
                error = ex.Message;
            }
            return error;
        }

    }
}
