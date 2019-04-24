using System;
using System.Collections.Generic;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesAntiLavado;
using FastReport;
using System.Data;
using System.Linq;
using GestorReportes.BusinessLayer.Helpers;
using System.IO;
using FastReport.Export.Pdf;
using System.Diagnostics;
using FastReport.Export.OoXML;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class RepresentacionImpresaXMLAntilavado
    {
        private bool cancelacionPendiente = false, esPdf = true;
        private GestorReportes.BusinessLayer.EntitiesAntilavado2.Informe informe = null;
        private string applicationPath = string.Empty;
        public event EventHandler<CambioProgresoEventArgs> CambioProgreso;

        public RepresentacionImpresaXMLAntilavado()
        {

        }

        public RepresentacionImpresaXMLAntilavado(GestorReportes.BusinessLayer.EntitiesAntilavado2.Informe informe, string applicationPath, bool esPdf)
        {
            this.informe = informe;
            this.applicationPath = applicationPath;
            this.esPdf = esPdf;
        }

        #region Version de esquema 1
        public string generarRepresentacion(Informe informe, string applicationPath, bool esPdf)
        {
            try
            {
                string formato = applicationPath;
                //string formato = applicationPath + "ReporteAntilavado.fr3";
                if (!System.IO.File.Exists(formato))
                    return "No se encontró el formato del reporte: " + formato;

                Inmobiliaria inmo = new Inmobiliaria();
                string nombreContrib = inmo.getRazonContribuyenteByRFC(informe.SujetoObligado.ClaveRFC);
                if (string.IsNullOrEmpty(nombreContrib))
                    return "No se encontró el contribuyente " + informe.SujetoObligado.ClaveRFC + " en la base de datos";

                TfrxReportClass reporte = new TfrxReportClass();
                reporte.ClearReport();
                reporte.ClearDatasets();
                reporte.LoadReportFromFile(formato);

                FrxDataTable dtEncabezado = new FrxDataTable("dvEncabezado");
                DataColumn dcMesReportado = new DataColumn("mesReportado", typeof(string));
                DataColumn dcSujetoObligado = new DataColumn("sujetoObligado", typeof(string));
                DataColumn dcContribuyente = new DataColumn("contribuyente", typeof(string));
                dtEncabezado.Columns.Add(dcMesReportado);
                dtEncabezado.Columns.Add(dcSujetoObligado);
                dtEncabezado.Columns.Add(dcContribuyente);
                dtEncabezado.AcceptChanges();
                dtEncabezado.Rows.Add(informe.MesReportado.ToString("MMMM/yyyy").ToUpper(), informe.SujetoObligado.ClaveRFC + " - " + informe.SujetoObligado.Actividad, nombreContrib);

                FrxDataTable dtAvisos = new FrxDataTable("dvAvisos");
                DataColumn dcNumAviso = new DataColumn("numAviso", typeof(string));
                DataColumn dcAlerta = new DataColumn("alerta", typeof(string));
                DataColumn dcTipoPersona = new DataColumn("tipoPersona", typeof(string));
                DataColumn dcColonia = new DataColumn("colonia", typeof(string));
                DataColumn dcCalle = new DataColumn("calle", typeof(string));
                DataColumn dcNumExterior = new DataColumn("numExterior", typeof(string));
                DataColumn dcCodigoPostal = new DataColumn("codigoPostal", typeof(string));
                DataColumn dcTelefono = new DataColumn("telefono", typeof(string));
                dtAvisos.Columns.Add(dcNumAviso);
                dtAvisos.Columns.Add(dcAlerta);
                dtAvisos.Columns.Add(dcTipoPersona);
                dtAvisos.Columns.Add(dcColonia);
                dtAvisos.Columns.Add(dcCalle);
                dtAvisos.Columns.Add(dcNumExterior);
                dtAvisos.Columns.Add(dcCodigoPostal);
                dtAvisos.Columns.Add(dcTelefono);
                dtAvisos.AcceptChanges();

                FrxDataTable dtMorales = new FrxDataTable("dvMorales");
                DataColumn dcNumAvisoM = new DataColumn("numAvisoM", typeof(string));
                DataColumn dcRazon = new DataColumn("razon", typeof(string));
                DataColumn dcFechaConst = new DataColumn("fechaConst", typeof(string));
                DataColumn dcRFC = new DataColumn("rfc", typeof(string));
                DataColumn dcNombreApoderado = new DataColumn("nombreApoderado", typeof(string));
                DataColumn dcRfcApod = new DataColumn("rfcApod", typeof(string));
                DataColumn dcAutoridad = new DataColumn("autoridad", typeof(string));
                DataColumn dcNumId = new DataColumn("numId", typeof(string));
                dtMorales.Columns.Add(dcNumAvisoM);
                dtMorales.Columns.Add(dcRazon);
                dtMorales.Columns.Add(dcFechaConst);
                dtMorales.Columns.Add(dcRFC);
                dtMorales.Columns.Add(dcNombreApoderado);
                dtMorales.Columns.Add(dcRfcApod);
                dtMorales.Columns.Add(dcAutoridad);
                dtMorales.Columns.Add(dcNumId);
                dtMorales.AcceptChanges();

                /*FrxDataTable dtMoralesDetail = new FrxDataTable("dvMoralesDetail");
                DataColumn dcImporte = new DataColumn("importe", typeof(string));
                DataColumn dcNumAvisoMD = new DataColumn("numAvisoM", typeof(string));
                dtMoralesDetail.Columns.Add(dcImporte);
                dtMoralesDetail.Columns.Add(dcNumAvisoMD);
                dtMoralesDetail.AcceptChanges();*/
                
                FrxDataTable dtFisicas = new FrxDataTable("dvFisicas");
                DataColumn dcNumAvisoF = new DataColumn("numAvisoF");
                DataColumn dcNombre = new DataColumn("nombre");
                DataColumn dcApellidoP = new DataColumn("apellidoP");
                DataColumn dcApellidoM = new DataColumn("apellidoM");
                DataColumn dcRfcF = new DataColumn("rfcF");
                DataColumn dcPais = new DataColumn("pais");
                DataColumn dcActividad = new DataColumn("actividad");
                DataColumn dcAutoridadF = new DataColumn("autoridadF");
                DataColumn dcNumIdF = new DataColumn("numIdF");
                dtFisicas.Columns.Add(dcNumAvisoF);
                dtFisicas.Columns.Add(dcNombre);
                dtFisicas.Columns.Add(dcApellidoP);
                dtFisicas.Columns.Add(dcApellidoM);
                dtFisicas.Columns.Add(dcRfcF);
                dtFisicas.Columns.Add(dcPais);
                dtFisicas.Columns.Add(dcActividad);
                dtFisicas.Columns.Add(dcAutoridadF);
                dtFisicas.Columns.Add(dcNumIdF);
                dtFisicas.AcceptChanges();

                FrxDataTable dtOperaciones = new FrxDataTable("dvOperaciones");
                DataColumn dcNumAvisoO = new DataColumn("numAvisoO");
                DataColumn dcFechaInicio = new DataColumn("fechaInicio");
                DataColumn dcFechaFin = new DataColumn("fechaFin");
                DataColumn dcFechapago = new DataColumn("fechaPago");
                DataColumn dcInstrumento = new DataColumn("instrumentoMon");
                DataColumn dcMoneda = new DataColumn("moneda");
                DataColumn dcMonto = new DataColumn("monto");
                DataColumn dcDireccionInmueble = new DataColumn("dirInm");
                dtOperaciones.Columns.Add(dcNumAvisoO);
                dtOperaciones.Columns.Add(dcFechaInicio);
                dtOperaciones.Columns.Add(dcFechaFin);
                dtOperaciones.Columns.Add(dcFechapago);
                dtOperaciones.Columns.Add(dcInstrumento);
                dtOperaciones.Columns.Add(dcMoneda);
                dtOperaciones.Columns.Add(dcMonto);
                dtOperaciones.Columns.Add(dcDireccionInmueble);
                dtOperaciones.AcceptChanges();

                int contAviso = 1;
                if (informe.Avisos != null)
                {
                    foreach (Aviso aviso in informe.Avisos)
                    {
                        string persona = aviso.Persona.EsPersonaFisica ? "Fisica" : "Moral";
                        dtAvisos.Rows.Add(contAviso.ToString(), aviso.Alerta.Tipo.ToString(), persona, aviso.Persona.DomicilioNacional.Colonia, aviso.Persona.DomicilioNacional.Calle, aviso.Persona.DomicilioNacional.NumeroExterior, aviso.Persona.DomicilioNacional.CodigoPostal, aviso.Persona.Telefono.Numero);
                        if (aviso.Persona.EsPersonaFisica)
                        {
                            dtFisicas.Rows.Add(contAviso.ToString(), aviso.Persona.Fisica.Nombre, aviso.Persona.Fisica.ApellidoPaterno, aviso.Persona.Fisica.ApellidoMaterno, aviso.Persona.Fisica.RFC, aviso.Persona.Fisica.PaisNacimiento, aviso.Persona.Fisica.ActividadEconomica, aviso.Persona.Fisica.Autoridad, aviso.Persona.Fisica.NumeroIdentificacion);
                        }
                        else
                        {
                            dtMorales.Rows.Add(contAviso.ToString(), aviso.Persona.Moral.RazonSocial, aviso.Persona.Moral.FechaDeConstitucion.ToString("dd/MM/yyyy"), aviso.Persona.Moral.RFC, aviso.Persona.Moral.Apoderado.Nombre + " " + aviso.Persona.Moral.Apoderado.ApellidoPaterno + " " + aviso.Persona.Moral.Apoderado.ApellidoMaterno, aviso.Persona.Moral.Apoderado.RFC, aviso.Persona.Moral.Apoderado.Autoridad, aviso.Persona.Moral.Apoderado.NumeroIdentificacion);                            
                        }
                        foreach (Operacion operacion in aviso.Operaciones)
                        {
                            string instrumento = string.Empty;
                            if (operacion.Liquidacion.ClaveInstrumentoMonetario == 5)
                                instrumento = "Cheque";
                            else if (operacion.Liquidacion.ClaveInstrumentoMonetario == 8)
                                instrumento = "Transferencia interbancaria";
                            else if (operacion.Liquidacion.ClaveInstrumentoMonetario == 9)
                                instrumento = "Transferencia del mismo banco";
                            else
                                instrumento = "No identificado";
                            string moneda = operacion.Liquidacion.Moneda == 2 ? "Dolar" : "Peso";
                            string direccion = operacion.CaracteristicasDelInmueble.Domicilio.Calle + " " + operacion.CaracteristicasDelInmueble.Domicilio.NumeroExterior + " " + operacion.CaracteristicasDelInmueble.Domicilio.Colonia;
                            dtOperaciones.Rows.Add(contAviso.ToString(), operacion.FechaInicioRenta.ToShortDateString(), operacion.FechaFinRenta.ToShortDateString(), operacion.Liquidacion.FechaDePago.ToShortDateString(), instrumento, moneda, operacion.Liquidacion.MontoOperacion.ToString("N2"), direccion);
                        }
                        contAviso++;
                    }
                }             
   
                FrxDataView dvEncabezado = new FrxDataView(dtEncabezado, "dvEncabezado");
                dvEncabezado.AssignToReport(true, reporte);
                
                FrxDataView dvAvisos = new FrxDataView(dtAvisos, "dvAvisos");
                dvAvisos.AssignToReport(true, reporte);
                dtAvisos.AssignToDataBand("MasterData1", reporte);

                FrxDataView dvMorales = new FrxDataView(dtMorales, "dvMorales");
                dvMorales.AssignToReport(true, reporte);
                dtMorales.AssignToDataBand("MasterData2", reporte);                
                /*
                FrxDataView dvMoralesDetail = new FrxDataView(dtMoralesDetail, "dvMoralesDetail");
                dvMoralesDetail.AssignToReport(true, reporte);               
                dtMoralesDetail.AssignToDataBand("DetailData1", reporte);*/
                /*
                DataSet ds = new DataSet();
                ds.Tables.Add(dtMorales);
                ds.Tables.Add(dtMoralesDetail);
                ds.AcceptChanges();
                DataRelation rel = new DataRelation("", dcNumAvisoM, dcNumAvisoMD);
                ds.Relations.Add(rel);
                ds.AcceptChanges();
                IfrxDataBand db = (IfrxDataBand)reporte.FindObject("MasterData2");
                db.DataSet = (IfrxDataSet)ds;*/                
                
                FrxDataView dvFisicas = new FrxDataView(dtFisicas, "dvFisicas");
                dvFisicas.AssignToReport(true, reporte);
                dtFisicas.AssignToDataBand("MasterData3", reporte);

                FrxDataView dvOperaciones = new FrxDataView(dtOperaciones, "dvOperaciones");
                dvOperaciones.AssignToReport(true, reporte);
                dtOperaciones.AssignToDataBand("MasterData4", reporte);

                reporte.PrepareReport(true);

                if (esPdf)
                {
                    reporte.ShowPreparedReport();
                }
                else
                {
                    try
                    {
                        string directoryTmpReportesSaari = @"C:\Users\Public\Documents\SaariDB\TmpReportes";
                        if (!System.IO.Directory.Exists(directoryTmpReportesSaari))
                            System.IO.Directory.CreateDirectory(directoryTmpReportesSaari);

                        string archivoXlsTemporal = directoryTmpReportesSaari + "\\ReporteAntilavado" + DateTime.Now.Ticks + ".xls";
                        reporte.ExportToXLS(archivoXlsTemporal, true, false, true, false, false, true);
                        System.Diagnostics.Process.Start(archivoXlsTemporal);
                    }
                    catch
                    {
                        return "No se pudo exportar a Excel";
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Ocurrió un error al intentar generar la representación impresa del XML antilavado:" + Environment.NewLine + ex.Message;
            }
        }
        #endregion

        #region Version de esquema 2
        public string generar()
        {
            try
            {
                string formato = applicationPath;
                //string formato = applicationPath + "ReporteAntilavado.fr3";
                if (!System.IO.File.Exists(formato))
                    return "No se encontró el formato del reporte: " + formato;

                OnCambioProgreso(10);
                if (cancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                Inmobiliaria inmo = new Inmobiliaria();
                string nombreContrib = inmo.getRazonContribuyenteByRFC(informe.SujetoObligado.ClaveRFC);
                if (string.IsNullOrEmpty(nombreContrib))
                    return "No se encontró el contribuyente " + informe.SujetoObligado.ClaveRFC + " en la base de datos";

                OnCambioProgreso(20);
                if (cancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                //TfrxReportClass reporte = new TfrxReportClass();
                //reporte.ClearReport();
                //reporte.ClearDatasets();
                //reporte.LoadReportFromFile(formato);

                Report Reporte = new Report();
                Reporte.Clear();
                Reporte.Load(formato);
                DataTable dtEncabezado = new DataTable("dvEncabezado");
                DataColumn dcMesReportado = new DataColumn("mesReportado", typeof(string));
                DataColumn dcSujetoObligado = new DataColumn("sujetoObligado", typeof(string));
                DataColumn dcContribuyente = new DataColumn("contribuyente", typeof(string));
                dtEncabezado.Columns.Add(dcMesReportado);
                dtEncabezado.Columns.Add(dcSujetoObligado);
                dtEncabezado.Columns.Add(dcContribuyente);
                dtEncabezado.AcceptChanges();
                dtEncabezado.Rows.Add(informe.MesReportado.ToString("MMMM/yyyy").ToUpper(), informe.SujetoObligado.ClaveRFC + " - " + informe.SujetoObligado.Actividad, nombreContrib);

                OnCambioProgreso(30);
                if (cancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                DataTable dtAvisos = new DataTable("dvAvisos");
                DataColumn dcNumAviso = new DataColumn("numAviso", typeof(string));
                DataColumn dcAlerta = new DataColumn("alerta", typeof(string));
                DataColumn dcTipoPersona = new DataColumn("tipoPersona", typeof(string));
                DataColumn dcColonia = new DataColumn("colonia", typeof(string));
                DataColumn dcCalle = new DataColumn("calle", typeof(string));
                DataColumn dcNumExterior = new DataColumn("numExterior", typeof(string));
                DataColumn dcCodigoPostal = new DataColumn("codigoPostal", typeof(string));
                DataColumn dcTelefono = new DataColumn("telefono", typeof(string));
                dtAvisos.Columns.Add(dcNumAviso);
                dtAvisos.Columns.Add(dcAlerta);
                dtAvisos.Columns.Add(dcTipoPersona);
                dtAvisos.Columns.Add(dcColonia);
                dtAvisos.Columns.Add(dcCalle);
                dtAvisos.Columns.Add(dcNumExterior);
                dtAvisos.Columns.Add(dcCodigoPostal);
                dtAvisos.Columns.Add(dcTelefono);
                dtAvisos.AcceptChanges();

                DataTable dtMorales = new DataTable("dvMorales");
                DataColumn dcNumAvisoM = new DataColumn("numAvisoM", typeof(string));
                DataColumn dcRazon = new DataColumn("razon", typeof(string));
                DataColumn dcFechaConst = new DataColumn("fechaConst", typeof(string));
                DataColumn dcRFC = new DataColumn("rfc", typeof(string));
                DataColumn dcNombreApoderado = new DataColumn("nombreApoderado", typeof(string));
                DataColumn dcRfcApod = new DataColumn("rfcApod", typeof(string));
                dtMorales.Columns.Add(dcNumAvisoM);
                dtMorales.Columns.Add(dcRazon);
                dtMorales.Columns.Add(dcFechaConst);
                dtMorales.Columns.Add(dcRFC);
                dtMorales.Columns.Add(dcNombreApoderado);
                dtMorales.Columns.Add(dcRfcApod);
                dtMorales.AcceptChanges();

                /*FrxDataTable dtMoralesDetail = new FrxDataTable("dvMoralesDetail");
                DataColumn dcImporte = new DataColumn("importe", typeof(string));
                DataColumn dcNumAvisoMD = new DataColumn("numAvisoM", typeof(string));
                dtMoralesDetail.Columns.Add(dcImporte);
                dtMoralesDetail.Columns.Add(dcNumAvisoMD);
                dtMoralesDetail.AcceptChanges();*/

                DataTable dtFisicas = new DataTable("dvFisicas");
                DataColumn dcNumAvisoF = new DataColumn("numAvisoF", typeof(string));
                DataColumn dcNombre = new DataColumn("nombre", typeof(string));
                DataColumn dcApellidoP = new DataColumn("apellidoP", typeof(string));
                DataColumn dcApellidoM = new DataColumn("apellidoM", typeof(string));
                DataColumn dcRfcF = new DataColumn("rfcF", typeof(string));
                DataColumn dcPais = new DataColumn("pais", typeof(string));
                DataColumn dcActividad = new DataColumn("actividad", typeof(string));
                dtFisicas.Columns.Add(dcNumAvisoF);
                dtFisicas.Columns.Add(dcNombre);
                dtFisicas.Columns.Add(dcApellidoP);
                dtFisicas.Columns.Add(dcApellidoM);
                dtFisicas.Columns.Add(dcRfcF);
                dtFisicas.Columns.Add(dcPais);
                dtFisicas.Columns.Add(dcActividad);
                dtFisicas.AcceptChanges();

                DataTable dtOperaciones = new DataTable("dvOperaciones");
                DataColumn dcNumAvisoO = new DataColumn("numAvisoO", typeof(string));
                DataColumn dcFechaInicio = new DataColumn("fechaInicio", typeof(string));
                DataColumn dcFechaFin = new DataColumn("fechaFin", typeof(string));
                DataColumn dcFechapago = new DataColumn("fechaPago", typeof(string));
                DataColumn dcInstrumento = new DataColumn("instrumentoMon", typeof(string));
                DataColumn dcMoneda = new DataColumn("moneda", typeof(string));
                DataColumn dcMonto = new DataColumn("monto", typeof(string));
                DataColumn dcDireccionInmueble = new DataColumn("dirInm", typeof(string));
                dtOperaciones.Columns.Add(dcNumAvisoO);
                dtOperaciones.Columns.Add(dcFechaInicio);
                dtOperaciones.Columns.Add(dcFechaFin);
                dtOperaciones.Columns.Add(dcFechapago);
                dtOperaciones.Columns.Add(dcInstrumento);
                dtOperaciones.Columns.Add(dcMoneda);
                dtOperaciones.Columns.Add(dcMonto);
                dtOperaciones.Columns.Add(dcDireccionInmueble);
                dtOperaciones.AcceptChanges();

                OnCambioProgreso(40);
                if (cancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                int contAviso = 1;
                if (informe.Avisos != null)
                {
                    int porcentaje = 40;
                    decimal factor = 40 / informe.Avisos.Count;
                    factor = factor >= 1 ? factor : 1;

                    foreach (GestorReportes.BusinessLayer.EntitiesAntilavado2.Aviso aviso in informe.Avisos)
                    {
                        if (porcentaje <= 80)
                            porcentaje += Convert.ToInt32(factor);
                        OnCambioProgreso(porcentaje);
                        if (cancelacionPendiente)
                            return "Proceso cancelado por el usuario";

                        string persona = aviso.Persona.EsPersonaFisica ? "Fisica" : "Moral";
                        dtAvisos.Rows.Add(contAviso.ToString(), aviso.Alerta.Tipo.ToString(), persona, aviso.Persona.DomicilioNacional.Colonia, aviso.Persona.DomicilioNacional.Calle, aviso.Persona.DomicilioNacional.NumeroExterior, aviso.Persona.DomicilioNacional.CodigoPostal, aviso.Persona.Telefono.Numero);
                        if (aviso.Persona.EsPersonaFisica)
                        {
                            dtFisicas.Rows.Add(contAviso.ToString(), aviso.Persona.Fisica.Nombre, aviso.Persona.Fisica.ApellidoPaterno, aviso.Persona.Fisica.ApellidoMaterno, aviso.Persona.Fisica.RFC, aviso.Persona.Fisica.PaisNacionalidad, aviso.Persona.Fisica.ActividadEconomica);
                        }
                        else
                        {
                            dtMorales.Rows.Add(contAviso.ToString(), aviso.Persona.Moral.RazonSocial, aviso.Persona.Moral.FechaDeConstitucion.ToString("dd/MM/yyyy"), aviso.Persona.Moral.RFC, aviso.Persona.Moral.Apoderado.Nombre + " " + aviso.Persona.Moral.Apoderado.ApellidoPaterno + " " + aviso.Persona.Moral.Apoderado.ApellidoMaterno, aviso.Persona.Moral.Apoderado.RFC);
                        }
                        foreach (GestorReportes.BusinessLayer.EntitiesAntilavado2.Operacion operacion in aviso.Operaciones)
                        {
                            string instrumento = string.Empty;
                            if (operacion.Liquidacion.First().ClaveInstrumentoMonetario == 5)
                                instrumento = "Cheque";
                            else if (operacion.Liquidacion.First().ClaveInstrumentoMonetario == 8)
                                instrumento = "Transferencia interbancaria";
                            else if (operacion.Liquidacion.First().ClaveInstrumentoMonetario == 9)
                                instrumento = "Transferencia del mismo banco";
                            else
                                instrumento = "No identificado";
                            string moneda = operacion.Liquidacion.First().Moneda == 2 ? "Dolar" : "Peso";
                            string direccion = operacion.Caracteristicas.First().Domicilio.Calle + " " + operacion.Caracteristicas.First().Domicilio.NumeroExterior + " " + operacion.Caracteristicas.First().Domicilio.Colonia;
                            dtOperaciones.Rows.Add(contAviso.ToString(), operacion.Caracteristicas.First().FechaInicio.ToShortDateString(), operacion.Caracteristicas.First().FechaFin.ToShortDateString(), operacion.Liquidacion.First().FechaDePago.ToShortDateString(), instrumento, moneda, operacion.Liquidacion.First().MontoOperacion.ToString("N2"), direccion);
                        }
                        contAviso++;
                    }
                }

                OnCambioProgreso(85);
                if (cancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                Reporte.RegisterData(dtEncabezado, "dvEncabezado");
                Reporte.RegisterData(dtAvisos, "dvAvisos");
                Reporte.RegisterData(dtMorales, "dvMorales");
                Reporte.RegisterData(dtFisicas, "dvFisicas");
                Reporte.RegisterData(dtOperaciones, "dvOperaciones");

                DataBand bandaDetalle = Reporte.FindObject("Data1") as DataBand;
                bandaDetalle.DataSource = Reporte.GetDataSource("dvAvisos");

                DataBand morales = Reporte.FindObject("Data2") as DataBand;
                morales.DataSource = Reporte.GetDataSource("dvMorales");

                DataBand fisicas = Reporte.FindObject("Data3") as DataBand;
                fisicas.DataSource = Reporte.GetDataSource("dvFisicas");

                DataBand operaciones = Reporte.FindObject("Data4") as DataBand;
                operaciones.DataSource = Reporte.GetDataSource("dvOperaciones");
                                
                /*
                FrxDataView dvMoralesDetail = new FrxDataView(dtMoralesDetail, "dvMoralesDetail");
                dvMoralesDetail.AssignToReport(true, reporte);               
                dtMoralesDetail.AssignToDataBand("DetailData1", reporte);*/
                /*
                DataSet ds = new DataSet();
                ds.Tables.Add(dtMorales);
                ds.Tables.Add(dtMoralesDetail);
                ds.AcceptChanges();
                DataRelation rel = new DataRelation("", dcNumAvisoM, dcNumAvisoMD);
                ds.Relations.Add(rel);
                ds.AcceptChanges();
                IfrxDataBand db = (IfrxDataBand)reporte.FindObject("MasterData2");
                db.DataSet = (IfrxDataSet)ds;*/

                           
                OnCambioProgreso(90);
                if (cancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                string nombreReporte = "ReporteAntilavado";
                string rutaGuardar = Properties.Settings.Default.RutaRepositorio;
                rutaGuardar = rutaGuardar.EndsWith(@"\") ? rutaGuardar + nombreReporte + @"\" : rutaGuardar + @"\" + nombreReporte + @"\";
                if (!Directory.Exists(rutaGuardar))
                    Directory.CreateDirectory(rutaGuardar);
                Reporte.Prepare();
                if (esPdf)
                {
                    try
                    {
                        string filename = rutaGuardar + nombreReporte + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                        PDFExport export = new PDFExport();
                        Reporte.Export(export, filename);

                        string nombreArchivo = filename;
                        Reporte.Dispose();

                        if (File.Exists(filename))
                            Process.Start(filename);
                    }
                    catch (Exception ex)
                    {
                        return "No se pudo exportar a PDF";
                    }
                }
                else
                {
                    try
                    {
                        string filename = rutaGuardar + nombreReporte + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                        Excel2007Export export = new Excel2007Export();
                        Reporte.Export(export, filename);

                        string nombreArchivo = filename;
                        Reporte.Dispose();

                        if (File.Exists(filename))
                            Process.Start(filename);
                    }
                    catch (Exception ex)
                    {
                        return "No se pudo exportar a Excel";
                    }
                }
                OnCambioProgreso(100);
                if (cancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Ocurrió un error al intentar generar la representación impresa del XML antilavado:" + Environment.NewLine + ex.Message;
            }
        }
        #endregion

        public void cancelar()
        {
            cancelacionPendiente = true;
        }

        protected virtual void OnCambioProgreso(int porcentaje)
        {
            var handler = CambioProgreso;
            if (handler != null)
                handler(this, new CambioProgresoEventArgs(porcentaje));
        }
    }
}
