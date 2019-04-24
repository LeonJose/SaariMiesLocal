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
    class Agenda : SaariReport, IReport, IBackgroundReport
    {
        private string usuario = string.Empty, estatus = string.Empty, rutaFormato = string.Empty;
        private DateTime fechaInicio = new DateTime(), fechaFin = DateTime.Now.Date;
        private bool esCalendario = false, esPdf = true;
        private ClienteEntity cliente = null;
        public Agenda(ClienteEntity cliente, string usuario, string estatus, DateTime fechaInicio, DateTime fechaFin, bool esCalendario, bool esPdf, string rutaFormato)
        {
            this.cliente = cliente;
            this.usuario = usuario;
            this.estatus = estatus;
            this.fechaInicio = fechaInicio.Date;
            this.fechaFin = fechaFin.Date;
            this.esCalendario = esCalendario;
            this.esPdf = esPdf;
            this.rutaFormato = rutaFormato;
        }

        public static List<ClienteEntity> obtenerClientes()
        {
            return SaariDB.getClientes();
        }

        public static List<string> obtenerUsuarios()
        {
            return SaariDB.getUsuarios();
        }



        public string generar() 
        {
            if (esCalendario)
                return generar_calAgenda();
            else
                return generar_repAgenda();
        }

        private string generar_repAgenda()
        {
            OnCambioProgreso(10);
            if (CancelacionPendiente)
                return "Proceso cancelado por el usuario";
            //DateTime fechaIni = new DateTime(fechaInicio.Year, fechaInicio.Month, 1);
            //DateTime fechaFin = fechaIni.AddMonths(1).AddDays(-1);
            DateTime fechaIni = fechaInicio;
            //DateTime fechaFin = fechaFin;
            var listaAgendas = SaariDB.getRegistrosAgenda(cliente, usuario, estatus, fechaIni, fechaFin);
            OnCambioProgreso(30);
            if (CancelacionPendiente)
                return "Proceso cancelado por el usuario";
            if (listaAgendas.Count > 0)
            {
                TfrxReportClass reporte = new TfrxReportClass();
                reporte.ClearReport();
                reporte.ClearDatasets();
                reporte.LoadReportFromFile(rutaFormato);

                FrxDataTable dtInmobiliaria = new FrxDataTable("dvInmobiliria");
                DataColumn dcInmobiliria = new DataColumn("nombrecomercial", typeof(string));
                DataColumn dcFechini = new DataColumn("Fecha_Inicial", typeof(string));
                DataColumn dcfechfin = new DataColumn("Fecha_Final", typeof(string));
                dtInmobiliaria.Columns.Add(dcInmobiliria);
                dtInmobiliaria.Columns.Add(dcFechini);
                dtInmobiliaria.Columns.Add(dcfechfin);
                dtInmobiliaria.AcceptChanges();
                try
                {
                    dtInmobiliaria.Rows.Add(cliente != null ? cliente.Nombre : "Todos", fechaInicio.ToShortDateString(), this.fechaFin.ToShortDateString());
                }
                catch (Exception ex)
                {
                    return "Ocurrió un error al leer los registros. Error: " + ex.Message;
                }
                FrxDataTable dtAgenda2 = new FrxDataTable("dvAgenda");
                DataColumn dcFecha = new DataColumn("AgendaFecha", typeof(string));
                DataColumn dcNombre = new DataColumn("AgendaNombre", typeof(string));
                DataColumn dcActividad = new DataColumn("AgendaDescrip", typeof(string));
                DataColumn dcObserv = new DataColumn("AgendaObs", typeof(string));
                DataColumn dcUser = new DataColumn("AgendaUser", typeof(string));
                dtAgenda2.Columns.Add(dcFecha);
                dtAgenda2.Columns.Add(dcNombre);
                dtAgenda2.Columns.Add(dcActividad);
                dtAgenda2.Columns.Add(dcObserv);
                dtAgenda2.Columns.Add(dcUser);
                dtAgenda2.AcceptChanges();

                OnCambioProgreso(40);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                int porcentaje = 40;
                decimal factor = 40 / listaAgendas.Count;
                factor = factor >= 1 ? factor : 1;

                foreach (var agenda in listaAgendas)
                {
                    if (porcentaje <= 80)
                        porcentaje += Convert.ToInt32(factor);
                    OnCambioProgreso(porcentaje <= 80 ? porcentaje : 80);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    try
                    {
                        dtAgenda2.Rows.Add(agenda.Fecha.ToShortDateString(), agenda.Nombre, agenda.Motivo, Encoding.UTF8.GetString(agenda.Observacion), agenda.Usuario);
                    }
                    catch
                    {

                        dtAgenda2.Rows.Add(agenda.Fecha.ToShortDateString(), agenda.Nombre, agenda.Motivo, string.Empty, agenda.Usuario);

                    }

                }

                OnCambioProgreso(90);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                FrxDataView dvinmobi = new FrxDataView(dtInmobiliaria, "dvInmobiliaria");
                FrxDataView dvdetalle = new FrxDataView(dtAgenda2, "dvAgenda");

                dvinmobi.AssignToReport(true, reporte);
                dvdetalle.AssignToReport(true, reporte);

                dtInmobiliaria.AssignToDataBand("MasterData1", reporte);
                dtAgenda2.AssignToDataBand("MasterData2", reporte);

                return exportar(reporte, esPdf, "AgendaDetallada");
            }
            else
            {
                string mensaje = "No se encontraron registros ";
                if(cliente!=null)
                    if (!string.IsNullOrEmpty(cliente.Nombre))
                        mensaje += " para el cliente " + cliente.Nombre;
                mensaje += " en el periodo del " + fechaInicio.ToShortDateString() + " al " + fechaFin.ToShortDateString();
                if (!string.IsNullOrEmpty(usuario))
                    mensaje += " para el usuario " + usuario;
                if (!string.IsNullOrEmpty(estatus))
                    mensaje += " con mantenimientos " + estatus;

                return mensaje + ". Verifique los criterios seleccionados.";     
            }
        }
        private string[] Meses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
        
        private string generar_calAgenda()
        {
            OnCambioProgreso(10);
            if (CancelacionPendiente)
                return "Proceso cancelado por el usuario";
            rutaFormato = rutaFormato.Replace("Agenda.fr3", "Calendario.fr3");            
            DateTime fechaIni = new DateTime(fechaInicio.Year, fechaInicio.Month, 1);
            DateTime fechaFin = fechaIni.AddMonths(1).AddDays(-1);
            //DateTime fechaIni = fechaInicio;
            
            var listaAgendas = SaariDB.getRegistrosAgenda(cliente, usuario, estatus, fechaIni, fechaFin);
            OnCambioProgreso(30);
            if (CancelacionPendiente)
                return "Proceso cancelado por el usuario";
            if (listaAgendas.Count > 0)
            {
                TfrxReportClass reporte = new TfrxReportClass();
                reporte.ClearReport();
                reporte.ClearDatasets();
                reporte.LoadReportFromFile(rutaFormato);

                IfrxComponent memoEncab = reporte.FindObject("memoEncabezado");
                (memoEncab as IfrxCustomMemoView).Text = "Calendario Agenda";

                FrxDataTable dtInmobiliaria = new FrxDataTable("dvInmobiliria");
                DataColumn dcInmobiliria = new DataColumn("nombreComercial", typeof(string));
                DataColumn dcFechini = new DataColumn("Fecha_Inicial", typeof(string));
                DataColumn dcfechfin = new DataColumn("Fecha_Final", typeof(string));
                DataColumn dcMes = new DataColumn("mes", typeof(string));
                dtInmobiliaria.Columns.Add(dcInmobiliria);
                dtInmobiliaria.Columns.Add(dcFechini);
                dtInmobiliaria.Columns.Add(dcfechfin);
                dtInmobiliaria.Columns.Add(dcMes);
                dtInmobiliaria.AcceptChanges();
                //dtInmobiliaria.Rows.Add(cliente.Nombre, fechaInicio.ToShortDateString(), this.fechaFin.ToShortDateString(), fechaInicio.ToString("MMMM yyyy").ToUpper());
                try
                {
                    dtInmobiliaria.Rows.Add(cliente != null ? cliente.Nombre : "Todos", this.fechaInicio.ToShortDateString(), this.fechaFin.ToShortDateString(), Meses[fechaIni.Month - 1] + " " + fechaIni.Year.ToString());
                }
                catch (Exception ex)
                {
                    return "Ocurrió un error al leer los registros. Error: " + ex.Message;
                }


                FrxDataTable dtMant = new FrxDataTable("dvMant");

                OnCambioProgreso(40);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                try
                {
                    int ren = 1;
                    int col = 1;
                    DateTime primeroMes = new DateTime(fechaInicio.Year, fechaInicio.Month, 1);
                    col = Convert.ToInt32(primeroMes.DayOfWeek) + 1;

                    int porcentaje = 40;
                    decimal factor = 40 / DateTime.DaysInMonth(fechaInicio.Year, fechaInicio.Month);
                    factor = factor >= 1 ? factor : 1;

                    for (int i = 1; i <= DateTime.DaysInMonth(fechaInicio.Year, fechaInicio.Month); i++)
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
                        DateTime fechaHoy = new DateTime(fechaInicio.Year, fechaInicio.Month, i);
                        foreach (var agenda in listaAgendas)
                        {
                            if (agenda.Fecha.Date == fechaHoy)
                            {
                                contDay++;
                                string memoActStr = "memoAct" + ren + col;
                                IfrxComponent memoAct = reporte.FindObject(memoActStr);
                                if (agenda.Estatus == "N")
                                    (memoAct as IfrxCustomMemoView).Text += "X " + agenda.Motivo + Environment.NewLine + agenda.Nombre;
                                else
                                    (memoAct as IfrxCustomMemoView).Text += "~ " + agenda.Motivo + Environment.NewLine + agenda.Nombre;
                                if (contDay == 3)
                                    (memoDia as IfrxCustomMemoView).Text = "* " + (memoDia as IfrxCustomMemoView).Text + agenda.Nombre;
                            }
                        }
                        col++;
                    }
                }
                catch
                {
                    return "Problema al asignar calendario";
                }

                try
                {
                    OnCambioProgreso(90);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    DataColumn dcMantUser = new DataColumn("MantUser", typeof(string));
                    dtMant.Columns.Add(dcMantUser);
                    dtMant.AcceptChanges();
                    dtMant.Rows.Add(1);//para calendario
                    FrxDataView dvinmobi = new FrxDataView(dtInmobiliaria, "dvInmobiliaria");
                    FrxDataView dvMant = new FrxDataView(dtMant, "dvMant");

                    dvinmobi.AssignToReport(true, reporte);
                    dvMant.AssignToReport(true, reporte);

                    dtInmobiliaria.AssignToDataBand("MasterData1", reporte);
                    dvMant.AssignToDataBand("MasterData2", reporte);

                    return exportar(reporte, esPdf, "AgendaCalendario");
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else
            {
                string mensaje = "No se encontraron registros ";
                if (cliente != null)
                    if (!string.IsNullOrEmpty(cliente.Nombre))
                        mensaje += " para el cliente " + cliente.Nombre;
                mensaje += " en el periodo del " + fechaInicio.ToShortDateString() + " al " + fechaFin.ToShortDateString();
                if (!string.IsNullOrEmpty(usuario))
                    mensaje += " para el usuario " + usuario;
                if (!string.IsNullOrEmpty(estatus))
                    mensaje += " con mantenimientos " + estatus;

                return mensaje + ". Verifique los criterios seleccionados.";                
            }
        }
    }
}
