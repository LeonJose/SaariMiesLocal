using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.DataAccessLayer;
using FastReport;
using System.Data;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class Bitacora: SaariReport, IReport, IBackgroundReport
    {
        private string Usuario = string.Empty;
        private string Estatus = string.Empty; 
        private string rutaFormato = string.Empty;
        private DateTime fechaInicio = new DateTime(); 
        private DateTime fechaFin = DateTime.Now.Date;
        private string Clasificacion = string.Empty; //Cliente o Prospecto o Empty para todos
        private bool esCalendario = false; 
        private bool esPdf = true;
        private bool EtapasTodas = true;
        private bool UsuariosTodos = true;
        private bool EstatusTodos=true;
        private ClienteEntity Cliente = null;
        private string Etapa = string.Empty;
        private string[] Meses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

        public Bitacora(string CteProsp, ClienteEntity cliente, string usuario, string estatus, string etapa, DateTime fechaInicio, DateTime fechaFin, bool esCalendario, bool esPdf, string rutaFormato)
        {
            this.Clasificacion = CteProsp;
            this.Cliente = cliente;
            this.Usuario = usuario;
            this.Estatus = estatus;
            this.Etapa = etapa;
            this.fechaInicio = fechaInicio.Date;
            this.fechaFin = fechaFin.Date;
            this.esCalendario = esCalendario;
            this.esPdf = esPdf;
            this.rutaFormato = rutaFormato;
            EtapasTodas = string.IsNullOrEmpty(this.Etapa) ? true : false;
            EstatusTodos = string.IsNullOrEmpty(this.Estatus) ? true : false;
            UsuariosTodos = string.IsNullOrEmpty(this.Usuario) ? true : false; 
        }

        public static List<ClienteEntity> obtenerClientesProspectos()
        {
            return SaariDB.getClientesProspectos();
        }

        public static List<string> obtenerUsuarios()
        {
            return SaariDB.getUsuarios();
        }
        public static List<string> obtnerEtapasCRM()
        {
            return SaariDB.getEtapasSeguimientoCRM();
        }
        public string generar()
        {
            //if (esCalendario)
            //    return generarCalendarioBitacora();
            //else
                return generarReporteBitacora();
        }
        private string generarReporteBitacora()
        {
            
            OnCambioProgreso(10);
            if(CancelacionPendiente)
                return "Proceso cancelado por el usuario";
            DateTime fechaIni = fechaInicio;
            var listaBitacoras = SaariDB.getRegistrosBitacora(Cliente, Usuario, Estatus, Etapa, fechaInicio, fechaFin, Clasificacion);
            OnCambioProgreso(30);
            if(CancelacionPendiente)
                return "Proceso cancelado por el usuario";
            if(listaBitacoras!=null)
            {
                if (listaBitacoras.Count > 0)
                {
                    #region REPORTE BITACORA

                    #region CONFIGURAR REPORTE
                    Report reporte = new Report();
                    reporte.Clear();                    
                    reporte.Load(rutaFormato);
                    DataTable dtEncabezado = new DataTable("Encabezado");
                    DataColumn dcNombreCte= new DataColumn("NombreCliente");
                    DataColumn dcFechaInicial = new DataColumn("FechaInicial");
                    DataColumn dcFechaFinal = new DataColumn("FechaFinal");
                    dcFechaInicial.DataType = System.Type.GetType("System.DateTime");
                    dcFechaFinal.DataType = System.Type.GetType("System.DateTime");
                    dtEncabezado.Columns.Add(dcNombreCte);
                    dtEncabezado.Columns.Add(dcFechaInicial);
                    dtEncabezado.Columns.Add(dcFechaFinal);
                    dtEncabezado.AcceptChanges();
                    try
                    {
                        string nombreCliente=Cliente != null ? Cliente.Nombre : "";
                        dtEncabezado.Rows.Add(nombreCliente, this.fechaInicio.ToShortDateString(), this.fechaFin.ToShortDateString());
                    }
                    catch (Exception ex)
                    {
                        return "Ocurrió un error al leer los registros. Error: " + ex.Message;
                    }
                    DataTable dtBitacora = new DataTable("bitacora");
                    DataColumn dcFecha = new DataColumn("Fecha", typeof(DateTime));
                    DataColumn dcCliente = new DataColumn("Cliente", typeof(string));
                    DataColumn dcTipoEnte = new DataColumn("TipoEnte", typeof(string));
                    DataColumn dcContacto = new DataColumn("Contacto", typeof(string));
                    DataColumn dcAsunto = new DataColumn("Asunto", typeof(string));
                    DataColumn dcDescripcion = new DataColumn("Descripcion", typeof(string));
                    DataColumn dcUsuario = new DataColumn("Usuario", typeof(string));
                    DataColumn dcEtapa = new DataColumn("Etapa", typeof(string));
                    DataColumn dcIdCliente = new DataColumn("IdCliente", typeof(string));
                    DataColumn dcFechaContactoIni = new DataColumn("FechaContactoInicial", typeof(DateTime));
                    DataColumn dcFechaEstimadaCierre = new DataColumn("FechaEstimadaCierre", typeof(DateTime));
                    DataColumn dcImporteDeOportunidad = new DataColumn("ImporteOportunidad", typeof(Decimal));
                    DataColumn dcFechaAgendaSiguiente = new DataColumn("FechaAgendaSiguiente", typeof(DateTime));
                    DataColumn dcDescripcionAgendaSiguiente = new DataColumn("DescripcionAgendaSiguiente", typeof(string));

                    dtBitacora.Columns.Add(dcFecha);
                    dtBitacora.Columns.Add(dcCliente);
                    dtBitacora.Columns.Add(dcTipoEnte);
                    dtBitacora.Columns.Add(dcContacto);
                    dtBitacora.Columns.Add(dcAsunto);
                    dtBitacora.Columns.Add(dcDescripcion);
                    dtBitacora.Columns.Add(dcUsuario);
                    dtBitacora.Columns.Add(dcEtapa);
                    dtBitacora.Columns.Add(dcIdCliente);
                    dtBitacora.Columns.Add(dcFechaContactoIni);
                    dtBitacora.Columns.Add(dcFechaEstimadaCierre);
                    dtBitacora.Columns.Add(dcImporteDeOportunidad);
                    dtBitacora.Columns.Add(dcFechaAgendaSiguiente);
                    dtBitacora.Columns.Add(dcDescripcionAgendaSiguiente);

                    dtBitacora.AcceptChanges();
                    OnCambioProgreso(40);                    
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    int porcentaje = 40;
                    decimal factor = 40 / listaBitacoras.Count;
                    factor = factor >= 1 ? factor : 1;
                    
                    #endregion
                    #region AGREGAR REGISTROS AL REPORTE
                    foreach (var bitacora in listaBitacoras)
                    {
                        if (porcentaje <= 80)
                            porcentaje += Convert.ToInt32(factor);
                        OnCambioProgreso(porcentaje < 80 ? porcentaje : 80);
                        if(CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        try
                        {
                            dtBitacora.Rows.Add(bitacora.Fecha, bitacora.cliente,bitacora.TipoEnte, bitacora.Contacto, bitacora.Asunto, 
                                Encoding.UTF8.GetString(bitacora.Descripcion), bitacora.Usuario, bitacora.Etapa, bitacora.IdCliente, bitacora.FechaContactoInicial,
                                bitacora.FechaEsperadaDeCierre, bitacora.ImporteDeOportunidad, bitacora.FechaAgendaSiguiente, bitacora.DescripcionAgendaSiguiente);
                        }
                        catch
                        {
                            dtBitacora.Rows.Add(bitacora.Fecha, bitacora.cliente, bitacora.TipoEnte, bitacora.Contacto, bitacora.Asunto, string.Empty,
                                bitacora.Usuario, bitacora.Etapa, bitacora.IdCliente, bitacora.FechaContactoInicial,
                                bitacora.FechaEsperadaDeCierre, bitacora.ImporteDeOportunidad, bitacora.FechaAgendaSiguiente, bitacora.DescripcionAgendaSiguiente);
                        }
                    }
                    OnCambioProgreso(90);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    #endregion

                    #region GENERA REPORTE
                    
                    reporte.RegisterData(dtEncabezado, "Encabezado");
                    reporte.RegisterData(dtBitacora, "bitacora");
                    DataBand bandaMovimientos = reporte.FindObject("Data1") as DataBand;
                    bandaMovimientos.DataSource = reporte.GetDataSource("bitacora");
                    //reporte.Prepare();
                    return exportar(reporte, esPdf, "ReporteBitacoraCRM");                    
                    #endregion

                    #endregion
                }
                else
                {
                    #region MENSAJE SIN REGISTROS
                    string mensaje = "No se encontraron registros ";
                    if (this.Cliente != null)
                        if (!string.IsNullOrEmpty(Cliente.Nombre))
                            mensaje += " para el cliente " + Cliente.Nombre;
                    mensaje += " en el periodo del " + fechaInicio.ToShortDateString() + " al " + fechaFin.ToShortDateString();
                    if (!string.IsNullOrEmpty(Usuario))
                        mensaje += " para el usuario " + this.Usuario;
                    //if (!string.IsNullOrEmpty(this.Estatus))
                    //    mensaje += " con mantenimientos " + Estatus;

                    return mensaje + ". Verifique los criterios seleccionados.";
                    #endregion
                }
            }
            else
            {
                #region MENSAJE NO SE ENCOTRARON REGISTROS
                string mensaje = "No se encontraron registros ";
                mensaje += " en el periodo del " + fechaInicio.ToShortDateString() + " al " + fechaFin.ToShortDateString();
                return mensaje + ". Verifique los criterios seleccionados.";
                #endregion
            }
            
        }

        private string generarCalendarioBitacora()
        {
            //TO DO
            return string.Empty;
        }

    }//END OF CLASS
}//END OF NAMESPACE
