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
    class Mantenimiento : SaariReport, IReport, IBackgroundReport
    {
        private InmobiliariaEntity inmobiliaria = null;
        private DateTime fechaInicio = new DateTime(), fechaFin = DateTime.Now.Date;
        private bool esCalendario = false, esPdf = true;
        private string estatus = string.Empty, rutaFormato = string.Empty, usuario = string.Empty;
        public Mantenimiento(InmobiliariaEntity inmobiliaria, string estatus, string usuario, DateTime fechaInicio, DateTime fechaFin, bool esCalendario, bool esPdf, string rutaFormato)
        {
            this.inmobiliaria = inmobiliaria;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
            this.esCalendario = esCalendario;
            this.esPdf = esPdf;
            this.estatus = estatus;
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
        }

        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();
        }

        public static List<string> obtenerUsuarios()
        {
            return SaariDB.getUsuarios();
        }

        public string generar()
        {
            try
            {
                OnCambioProgreso(10);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                DateTime fechaIniAux = fechaInicio;
                DateTime fechaFinAux = fechaFin;
                if (esCalendario)
                {
                    fechaInicio = new DateTime(fechaInicio.Year, fechaInicio.Month, 1);
                    fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
                }

                OnCambioProgreso(20);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                var listaMantenimiento = SaariDB.getRegistrosMantenimiento(inmobiliaria, usuario, estatus, fechaInicio, fechaFin);

                OnCambioProgreso(30);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                if (listaMantenimiento.Count > 0)
                {
                    TfrxReportClass reporte = new TfrxReportClass();
                    reporte.ClearReport();
                    reporte.ClearDatasets();
                    reporte.LoadReportFromFile(rutaFormato);
                    FrxDataTable dtInmobiliaria = new FrxDataTable("dvInmobiliaria");
                    DataColumn dcInmobiliaraNombreComercial = new DataColumn("nombreComercial", typeof(string));
                    DataColumn dcFechaIni = new DataColumn("Fecha_Inicial", typeof(string));
                    DataColumn dcFechaFin = new DataColumn("Fecha_Final", typeof(string));
                    DataColumn dcMes = new DataColumn("mes", typeof(string)); // prueba calendario                

                    dtInmobiliaria.Columns.Add(dcInmobiliaraNombreComercial);
                    dtInmobiliaria.Columns.Add(dcFechaIni);
                    dtInmobiliaria.Columns.Add(dcFechaFin);
                    if (esCalendario)
                        dtInmobiliaria.Columns.Add(dcMes);
                    dtInmobiliaria.AcceptChanges();

                    if (esCalendario)
                        dtInmobiliaria.Rows.Add(inmobiliaria.RazonSocial, fechaIniAux.ToShortDateString(), fechaFinAux.ToShortDateString(), fechaIniAux.ToString("MMMM yyyy").ToUpper());
                    else
                        dtInmobiliaria.Rows.Add(inmobiliaria.RazonSocial, fechaIniAux.ToShortDateString(), fechaFinAux.ToShortDateString());

                    OnCambioProgreso(40);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    FrxDataTable dtMant = new FrxDataTable("dvMant");
                    if (esCalendario)
                    {
                        try
                        {
                            int ren = 1;
                            int col = 1;
                            DateTime primeroMes = new DateTime(fechaIniAux.Year, fechaIniAux.Month, 1);
                            col = Convert.ToInt32(primeroMes.DayOfWeek) + 1;

                            int porcentaje = 40;
                            decimal factor = 40 / DateTime.DaysInMonth(fechaInicio.Year, fechaInicio.Month);
                            factor = factor >= 1 ? factor : 1;

                            for (int i = 1; i <= DateTime.DaysInMonth(fechaIniAux.Year, fechaIniAux.Month); i++)
                            {
                                if (porcentaje <= 80)
                                    porcentaje += Convert.ToInt32(factor);
                                OnCambioProgreso(porcentaje <= 80 ? porcentaje : 80);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                int contDay = 0;
                                if (col > 7)
                                {
                                    col = 1;
                                    ren++;
                                }
                                string memoDiaStr = "memoDia" + ren + col;
                                IfrxComponent memoDia = reporte.FindObject(memoDiaStr);
                                (memoDia as IfrxCustomMemoView).Text = i.ToString();
                                DateTime fechaHoy = new DateTime(fechaIniAux.Year, fechaIniAux.Month, i);
                                foreach (var mant in listaMantenimiento)
                                {                                    
                                    if (mant.Fecha.Date == fechaHoy)
                                    {
                                        contDay++;
                                        string memoActStr = "memoAct" + ren + col;
                                        IfrxComponent memoAct = reporte.FindObject(memoActStr);
                                        if (mant.Estatus == "N")
                                            (memoAct as IfrxCustomMemoView).Text += "X " + mant.Motivo + Environment.NewLine;
                                        else
                                            (memoAct as IfrxCustomMemoView).Text += "~ " + mant.Motivo + Environment.NewLine;
                                        if (contDay == 6)
                                            (memoDia as IfrxCustomMemoView).Text = "* " + (memoDia as IfrxCustomMemoView).Text;
                                    }
                                }
                                col++;
                            }
                        }
                        catch
                        {
                            return "Problema al asignar calendario";
                        }

                        DataColumn dcMantUser = new DataColumn("MantUser", typeof(string));
                        dtMant.Columns.Add(dcMantUser);
                        dtMant.AcceptChanges();
                        dtMant.Rows.Add(1);//para calendario
                    }
                    else
                    {
                        DataColumn dcMantTipo = new DataColumn("MantTipo", typeof(string));
                        DataColumn dcMantFecha = new DataColumn("MantFecha", typeof(string));
                        DataColumn dcMantActiv = new DataColumn("MantDescrip", typeof(string));
                        DataColumn dcMantConj = new DataColumn("MantConjunto", typeof(string));
                        DataColumn dcMantObs = new DataColumn("MantObs", typeof(string));
                        DataColumn dcMantUser = new DataColumn("MantUser", typeof(string));

                        dtMant.Columns.Add(dcMantTipo);
                        dtMant.Columns.Add(dcMantFecha); 
                        dtMant.Columns.Add(dcMantActiv);
                        dtMant.Columns.Add(dcMantConj);
                        dtMant.Columns.Add(dcMantObs);
                        dtMant.Columns.Add(dcMantUser);
                        dtMant.AcceptChanges();

                        int porcentaje = 40;
                        decimal factor = 40 / listaMantenimiento.Count;
                        factor = factor >= 1 ? factor : 1;
                        string observacion = string.Empty;
                        foreach (var mant in listaMantenimiento)
                        {

                            if (porcentaje <= 80)
                                porcentaje += Convert.ToInt32(factor);
                            OnCambioProgreso(porcentaje <= 80 ? porcentaje : 80);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";
                            if (string.IsNullOrEmpty(estatus))
                                observacion = mant.Estatus == "N" ? " (Pendiente)" : " (Terminado)";
                            else
                            {
                                switch (estatus)
                                {
                                    case "Pendientes":
                                        observacion = " (Pendiente)";
                                        break;
                                    case "Terminados":
                                        observacion = " (Terminado)";
                                        break;
                                    case "Vencidos":
                                        observacion = " (Vencido)";
                                        break;
                                    default:
                                        observacion = " (Pendiente)";
                                        break;
                                }                            
                            }
                            try
                            {                                
                                //observacion = Encoding.UTF8.GetString(mant.Observacion) + " -" + estatus;
                                observacion = Encoding.UTF8.GetString(mant.Observacion) + observacion;
                                dtMant.Rows.Add(mant.Tipo, mant.Fecha.ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("es-ES")), mant.Motivo, mant.Descripcion, observacion, mant.Usuario);
                            }
                            catch
                            {
                                dtMant.Rows.Add(mant.Tipo, mant.Fecha.ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("es-ES")), mant.Motivo, mant.Descripcion, observacion, mant.Usuario);
                            }
                        }
                    }

                    OnCambioProgreso(90);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    FrxDataView dvInmobiliaria = new FrxDataView(dtInmobiliaria, "dvInmobiliaria");
                    FrxDataView dvMant = new FrxDataView(dtMant, "dvMant");

                    dvInmobiliaria.AssignToReport(true, reporte);
                    dvMant.AssignToReport(true, reporte);

                    dtInmobiliaria.AssignToDataBand("MasterData1", reporte); //Parche para que aparezca encabezado de reporte

                    dtMant.AssignToDataBand("MasterData2", reporte);

                    return exportar(reporte, esPdf, esCalendario ? "MantenimientoCalendario" : "MantenimientoDetallado");
                    /*reporte.PrepareReport(true);
                    if (esPdf)
                        reporte.ShowPreparedReport();
                    else 
                    {
                        try
                        {
                            string directoryTmpReportesSaari = @"C:\Users\Public\Documents\SaariDB\TmpReportes";
                            if (!System.IO.Directory.Exists(directoryTmpReportesSaari))
                                System.IO.Directory.CreateDirectory(directoryTmpReportesSaari);

                            string archivoXlsTemporal = directoryTmpReportesSaari + "\\Reporte" + DateTime.Now.Ticks + ".xls";
                            reporte.ExportToXLS(archivoXlsTemporal, true, false, true, false, false, true);
                            System.Diagnostics.Process.Start(archivoXlsTemporal);
                        }
                        catch { }
                    }
                    return string.Empty;*/
                }
                else
                {
                    string mensaje = "No se encontraron registros para la inmobiliaria " + inmobiliaria.RazonSocial + " en el periodo del " + fechaInicio.ToShortDateString() + " al " +
                        fechaFin.ToShortDateString();
                    if (!string.IsNullOrEmpty(usuario))
                        mensaje += " para el usuario " + usuario;
                    if (!string.IsNullOrEmpty(estatus))
                        mensaje += " con mantenimientos " + estatus ;

                    return mensaje + ". Verifique los criterios seleccionados.";
                }
            }
            catch (Exception ex)
            {
                return "Error general al generar reporte. Error: "+ ex.Message;
            }
        }
    }
}
