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
    class MantenimientoGastos : SaariReport,IReport, IBackgroundReport
    {
        private InmobiliariaEntity inmobiliaria = null;
        private DateTime fechaInicio = new DateTime(), fechaFin = DateTime.Now.Date;
        private bool esPDF = true;
        private string clasif = string.Empty;
        private string rutaFormato = string.Empty;
        private string usuario = string.Empty; 
        private string inmueble = string.Empty;
        private string conjunto = string.Empty;
        private string idInmobiliaria = string.Empty;
        private string includInmue = string.Empty;
        private bool generarEx = true;
         
            public MantenimientoGastos(InmobiliariaEntity inmobiliaria,string idInmobiliaria, string clasif, string usuario, DateTime fechaInicio, DateTime fechaFin, bool esPDF, string rutaFormato, string inmueble, string conjunto, string includInmue, bool generarEx) 
            {
            this.inmobiliaria =inmobiliaria;
            this.idInmobiliaria = idInmobiliaria;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
            this.esPDF = esPDF;
            this.clasif = clasif;
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            this.conjunto = conjunto;
            this.inmueble = inmueble;
            this.includInmue = includInmue;
            this.generarEx = generarEx;
    
        }


        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();
           
        }

        public static List<string> obtenerUsuarios()
        {
            return SaariDB.getUsuarios();
        }

        public static List<string> obtenerClasificacion()
        {
            return SaariDB.getClasif();
        }

        public static List<EdificioEntity> obtenerInmuebles()
        {
            return SaariDB.getInmuebles();
        }

        
        public string generarRep() {
            return generarReporteGastosMant();             
        }

        private string generarReporteGastosMant()
        {

            OnCambioProgreso(10);
            if (CancelacionPendiente)
                return "Proceso cancelado por el usuario";
            DateTime fechaIni = fechaInicio;
            var listaMantenimientoGastos = SaariDB.getRegistroMantGastos(idInmobiliaria, conjunto, inmueble, usuario, clasif, fechaInicio, fechaFin, includInmue);
            OnCambioProgreso(30);
            if (CancelacionPendiente)
                return "Proceso cancelado por el usuario";
            if (listaMantenimientoGastos != null)
            {
                if (listaMantenimientoGastos.Count > 0)
                {
                    #region REPORTE Mantenimiento

                    #region CONFIGURAR REPORTE
                    Report reporte = new Report();
                    reporte.Clear();
                    reporte.Load(rutaFormato);
                    DataTable dtEncabezado = new DataTable("Encabezado");
                    DataColumn dcNombreComercial = new DataColumn("NombreComercial",typeof(string));
                    DataColumn dcFechaInicial = new DataColumn("FechaInicial", typeof(DateTime));
                    DataColumn dcFechaFinal = new DataColumn("FechaFinal", typeof(DateTime));
                    
                    dcFechaInicial.DataType = System.Type.GetType("System.DateTime");
                    dcFechaFinal.DataType = System.Type.GetType("System.DateTime");
                    dtEncabezado.Columns.Add(dcNombreComercial);
                    dtEncabezado.Columns.Add(dcFechaInicial);
                    dtEncabezado.Columns.Add(dcFechaFinal);
                    dtEncabezado.AcceptChanges();

                    try                        
                    {
                        string nombreInmobiliaria = inmobiliaria != null ? inmobiliaria.RazonSocial : "";
                        dtEncabezado.Rows.Add(nombreInmobiliaria, this.fechaInicio.ToShortDateString(), this.fechaFin.ToShortDateString());
                    }
                    catch (Exception ex)
                    {
                        return "Ocurrió un error al leer los registros. Error: " + ex.Message;
                    }
                    DataTable dtRepMantGasto = new DataTable();
                    DataColumn dcFecha = new DataColumn("Fecha", typeof(DateTime));
                    DataColumn dcAsunto= new DataColumn("Asunto", typeof(string));
                    DataColumn dcTipoNombre = new DataColumn("Tipo/Nombre", typeof(string));
                    DataColumn dcConjunto = new DataColumn("Conjunto", typeof(string));
                    DataColumn dcProveedor = new DataColumn("Proveedor", typeof(string));
                    DataColumn dcClasificacion = new DataColumn("Clasificacion", typeof(string));
                    DataColumn dcDescripcion = new DataColumn("Descripcion", typeof(string));
                    DataColumn dcSubtotal = new DataColumn("Subtotal", typeof(decimal));
                    DataColumn dcIVA = new DataColumn("IVA", typeof(decimal));
                    DataColumn dcTotal = new DataColumn("Total", typeof(decimal));
                    DataColumn dcTipoMoneda = new DataColumn("TipoMoneda", typeof(string));
                    DataColumn dcTC = new DataColumn("TC", typeof(string));
                    DataColumn dcUsuario = new DataColumn("Usuario", typeof(string));
                    DataColumn dcNombreInmo = new DataColumn("NombreInmo", typeof(string));
                    DataColumn dcsubtotalDolares = new DataColumn("subtotalDolares", typeof(decimal));
                    DataColumn dcIvaDolares = new DataColumn("ivaDolares", typeof(decimal));
                    DataColumn dctotalDolares = new DataColumn("totalDolares", typeof(decimal));

         
                    dtRepMantGasto.Columns.Add(dcFecha);
                    dtRepMantGasto.Columns.Add(dcAsunto);
                    dtRepMantGasto.Columns.Add(dcTipoNombre);
                    dtRepMantGasto.Columns.Add(dcConjunto);
                    dtRepMantGasto.Columns.Add(dcProveedor);
                    dtRepMantGasto.Columns.Add(dcClasificacion);
                    dtRepMantGasto.Columns.Add(dcDescripcion);
                    dtRepMantGasto.Columns.Add(dcSubtotal);
                    dtRepMantGasto.Columns.Add(dcIVA);
                    dtRepMantGasto.Columns.Add(dcTotal);
                    dtRepMantGasto.Columns.Add(dcTipoMoneda);
                    dtRepMantGasto.Columns.Add(dcTC);
                    dtRepMantGasto.Columns.Add(dcUsuario);
                    dtRepMantGasto.Columns.Add(dcsubtotalDolares);
                    dtRepMantGasto.Columns.Add(dcIvaDolares);
                    dtRepMantGasto.Columns.Add(dctotalDolares);

                  
                    

                    dtRepMantGasto.AcceptChanges();
                    OnCambioProgreso(40);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    int porcentaje = 40;
                    decimal factor = 40 / listaMantenimientoGastos.Count;
                    factor = factor >= 1 ? factor : 1;


                    

                    #endregion
                    #region AGREGAR REGISTROS AL REPORTE
                    foreach (var mantGastos in listaMantenimientoGastos)
                    {
                        if (porcentaje <= 80)
                            porcentaje += Convert.ToInt32(factor);
                        OnCambioProgreso(porcentaje < 80 ? porcentaje : 80);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";

                        if (mantGastos.TipoMoneda == "D")
                        {

                            if (mantGastos.TipoCambio == 0 || mantGastos.TipoCambio == null)
                            {
                                mantGastos.TipoCambio = 1;

                                mantGastos.Subtotal = mantGastos.Subtotal * mantGastos.TipoCambio;
                                mantGastos.Iva = mantGastos.Iva * mantGastos.TipoCambio;
                                mantGastos.Total = mantGastos.Subtotal + mantGastos.Iva;

                            }
                            else {
                                mantGastos.Subtotal = mantGastos.Subtotal * mantGastos.TipoCambio;
                                mantGastos.Iva = mantGastos.Iva * mantGastos.TipoCambio;
                                mantGastos.Total = mantGastos.Subtotal + mantGastos.Iva;
                            }
                        }
                        else if (mantGastos.TipoMoneda != "P" && mantGastos.TipoMoneda != "D")
                        {
                            mantGastos.TipoMoneda = "P";
                        }
                      
                        try
                        {                           

                                dtRepMantGasto.Rows.Add(mantGastos.Fecha, mantGastos.Asunto, mantGastos.iDinmueble, mantGastos.tipoNombre, mantGastos.Proveedor, mantGastos.clasificacion,
                                Encoding.UTF8.GetString(mantGastos.descripcion), mantGastos.Subtotal, mantGastos.Iva, mantGastos.Total, mantGastos.TipoMoneda,
                                mantGastos.TipoCambio, mantGastos.Usuarios,mantGastos.subTotalDolares, mantGastos.IvalDolares, mantGastos.TotalDolares);
                          
                        }
                        catch
                        {

                                dtRepMantGasto.Rows.Add(mantGastos.Fecha, mantGastos.Asunto, mantGastos.iDinmueble, mantGastos.tipoNombre, mantGastos.Proveedor, mantGastos.clasificacion,
                                 string.Empty, mantGastos.Subtotal, mantGastos.Iva, mantGastos.Total, mantGastos.TipoMoneda,
                                 mantGastos.TipoCambio, mantGastos.Usuarios, mantGastos.subTotalDolares, mantGastos.IvalDolares, mantGastos.TotalDolares);
                           
                        }
                    }
                    OnCambioProgreso(90);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                   #endregion
                    
                    #region GENERA REPORTE

                    reporte.RegisterData(dtEncabezado, "Encabezado");
                    reporte.RegisterData(dtRepMantGasto, "movimiento");
                    DataBand bandaMovimientos = reporte.FindObject("Data1") as DataBand;                    
                    bandaMovimientos.DataSource = reporte.GetDataSource("movimiento");
                  
                    ////reporte.Prepare();
                    //if (generarEx)
                    //{
                    //    abrirExcel = generarEx;
                    //}
                    return exportarValidaAbrir(reporte, esPDF, "ReporteEgresosInmuebles", generarEx);
                    
                    #endregion

                    #endregion
                }
                else
                {
                    #region MENSAJE SIN REGISTROS
                    string mensaje = "No se encontraron registros ";

                    if (this.inmobiliaria != null)
                        if (!string.IsNullOrEmpty(inmobiliaria.NombreComercial))
                            mensaje += " para el cliente " + inmobiliaria.NombreComercial;
                            mensaje += " en el periodo del " + fechaInicio.ToShortDateString() + " al " + fechaFin.ToShortDateString();

                    if (clasif.Count() == 0)
                        mensaje += " en el periodo del " + fechaInicio.ToShortDateString() + " al " + fechaFin.ToShortDateString();
                        mensaje += " para la clasificacion " + this.clasif;
                    return mensaje + ". Verifique los criterios seleccionados.";
                       

                    if(!string.IsNullOrEmpty(usuario))
                        mensaje += " para el usuario " + this.usuario;
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




        public string generar()
        {
            throw new NotImplementedException();
        }
    }

}