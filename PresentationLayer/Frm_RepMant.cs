using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_RepMant : Form
    {
        string rutaformato = string.Empty;
        string usuario = string.Empty;
        private bool EsCalendario = false;
        private string formato = string.Empty, error = string.Empty;
        private Mantenimiento mantenimiento = null;
        private string user = string.Empty;
        private bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;
        
        public Frm_RepMant(string rutaformato, string usuario)
        {
            InitializeComponent();
            //System.Drawing.Icon icono = new System.Drawing.Icon("IconoSAARI.ico");
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            if (!System.IO.File.Exists(rutaformato))
            {
                MessageBox.Show("No existe el formato de plantilla en ubicación: \"" + rutaformato + ". Comuniquese con el administrador del sistema.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            if (!rutaformato.Contains(".fr3"))
            {
                MessageBox.Show("Versión de plantilla no soportada. Este gestor sólo soporta plantillas en formato \".fr3\"." + ". Comuniquese con el administrador del sistema.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            this.rutaformato = rutaformato;
            this.user = usuario;
        }

        private void cargarinmobiliaria()
        {
            comboBox1.DataSource = HelpInmobiliarias.obtenerInmobiliarias(user,permisosInmo);//Mantenimiento.obtenerInmobiliarias();
            comboBox1.DisplayMember = "RazonSocial";
            comboBox1.ValueMember = "ID";
        }

        private void Btn_Cancelar_Click(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                mantenimiento.cancelar();
            botonCancelar.Enabled = false;
        }

        /*private void Btn_Impimir_Click(object sender, EventArgs e)
        {
            EsCalendario = rdBtnCalendar.Checked;
            if (EsCalendario)
            {
                _rutaformato = _rutaformato.Replace("Mantenimiento.fr3", "Calendario.fr3");
            }
            if (!System.IO.File.Exists(_rutaformato))
            {
                MessageBox.Show("No existe el formato de plantilla en ubicación: \"" + _rutaformato + "\"", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(0);
            }
            generar_reporte(string.Empty);
            System.Environment.Exit(0);
        }      */

        /*private void Btn_Excel_Click(object sender, EventArgs e)
        {
            EsCalendario = rdBtnCalendar.Checked;
            if (EsCalendario)
            {
                _rutaformato = _rutaformato.Replace("Mantenimiento.fr3", "Calendario.fr3");
            }
            if (!System.IO.File.Exists(_rutaformato))
            {
                MessageBox.Show("No existe el formato de plantilla en ubicación: \"" + _rutaformato + "\"", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(0);
            }
            generar_reporte("xls");
            System.Environment.Exit(0);
        }*/

        /*private void generar_reporte(string formato)
        {
            try
            {
                string inmobiliaria = comboBox1.Text;
                string sql;
                string fechInicial = dateTimePicker1.Value.Month + "/" + dateTimePicker1.Value.Day + "/" + dateTimePicker1.Value.Year;//dateTimePicker1.Value.ToShortDateString();
                string fechFin = dateTimePicker2.Value.Month + "/" + dateTimePicker2.Value.Day + "/" + dateTimePicker2.Value.Year;
                //si es calendario cambiar la fecha inicial y fecha final por la del datetimepicker1 
                if (EsCalendario)
                {
                    DateTime fechaIni = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, 1);
                    DateTime fechaFin = fechaIni.AddMonths(1).AddDays(-1);
                    fechInicial = fechaIni.Month + "/" + fechaIni.Day + "/" + fechaIni.Year;
                    fechFin = fechaFin.Month + "/" + fechaFin.Day + "/" + fechaFin.Year;
                }
                switch (Cmb_Stat.Text)
                {
                    case "Vencidos":
                        if (Rb_Todos.Checked)
                        {
                            sql = "select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE where T03_CENTRO_INDUSTRIAL.CAMPO1='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA<='" + fechInicial + "' AND T37_AGENDA.P3704_STATUS='S'  Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE where T07_EDIFICIO.P0709_ARRENDADORA='" + comboBox1.SelectedValue + "'" + " AND T37_AGENDA.P3702_FECHA<='" + fechInicial + "' AND T37_AGENDA.P3704_STATUS='S'";
                        }
                        else
                        {
                            usuario = Cmb_Usuario.Text;
                            sql = "select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE where T03_CENTRO_INDUSTRIAL.CAMPO1='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA<='" + fechInicial + "'and T37_AGENDA.P3706_USUARIO='" + usuario + "' AND T37_AGENDA.P3704_STATUS='S' Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE where T07_EDIFICIO.P0709_ARRENDADORA='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA<='" + fechInicial + "'and T37_AGENDA.P3706_USUARIO='" + usuario + "' AND T37_AGENDA.P3704_STATUS='S'";
                        }
                        break;
                    case "Pendientes":
                        if (Rb_Todos.Checked)
                        {
                            sql = "select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE where T03_CENTRO_INDUSTRIAL.CAMPO1='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA>='" + fechInicial + "'AND T37_AGENDA.P3702_FECHA<='" + fechFin + "' AND T37_AGENDA.P3704_STATUS='N'  Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE where T07_EDIFICIO.P0709_ARRENDADORA='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA>='" + fechInicial + "'AND T37_AGENDA.P3702_FECHA<='" + fechFin + "' AND T37_AGENDA.P3704_STATUS='N'";
                        }
                        else
                        {
                            usuario = Cmb_Usuario.Text;
                            sql = "select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE where T03_CENTRO_INDUSTRIAL.CAMPO1='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA>='" + fechInicial + "'AND T37_AGENDA.P3702_FECHA<='" + fechFin + "'and T37_AGENDA.P3706_USUARIO='" + usuario + "' AND T37_AGENDA.P3704_STATUS='N' Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE where T07_EDIFICIO.P0709_ARRENDADORA='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA>='" + fechInicial + "'AND T37_AGENDA.P3702_FECHA<='" + fechFin + "'and T37_AGENDA.P3706_USUARIO='" + usuario + "' AND T37_AGENDA.P3704_STATUS='N'";
                        }
                        break;
                    case "Terminados":
                        if (Rb_Todos.Checked)
                        {
                            sql = "select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE where T03_CENTRO_INDUSTRIAL.CAMPO1='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA>='" + fechInicial + "'AND T37_AGENDA.P3702_FECHA<='" + fechFin + "' AND T37_AGENDA.P3704_STATUS='S' Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE where T07_EDIFICIO.P0709_ARRENDADORA='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA>='" + fechInicial + "'AND T37_AGENDA.P3702_FECHA<='" + fechFin + "' AND T37_AGENDA.P3704_STATUS='S'";
                        }
                        else
                        {
                            usuario = Cmb_Usuario.Text;
                            sql = "select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE where T03_CENTRO_INDUSTRIAL.CAMPO1='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA>='" + fechInicial + "'AND T37_AGENDA.P3702_FECHA<='" + fechFin + "'and T37_AGENDA.P3706_USUARIO='" + usuario + "' AND T37_AGENDA.P3704_STATUS='S' Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE where T07_EDIFICIO.P0709_ARRENDADORA='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA>='" + fechInicial + "'AND T37_AGENDA.P3702_FECHA<='" + fechFin + "'and T37_AGENDA.P3706_USUARIO='" + usuario + "' AND T37_AGENDA.P3704_STATUS='S'";
                        }
                        break;
                    default:
                        if (Rb_Todos.Checked)
                        {
                            sql = "select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE where T03_CENTRO_INDUSTRIAL.CAMPO1='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA>='" + fechInicial + "'AND T37_AGENDA.P3702_FECHA<='" + fechFin + "'Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE where T07_EDIFICIO.P0709_ARRENDADORA='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA>='" + fechInicial + "'AND T37_AGENDA.P3702_FECHA<='" + fechFin + "'";
                        }
                        else
                        {
                            usuario = Cmb_Usuario.Text;
                            sql = "select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE where T03_CENTRO_INDUSTRIAL.CAMPO1='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA>='" + fechInicial + "'AND T37_AGENDA.P3702_FECHA<='" + fechFin + "'and T37_AGENDA.P3706_USUARIO='" + usuario + "'Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE where T07_EDIFICIO.P0709_ARRENDADORA='" + comboBox1.SelectedValue + "' AND T37_AGENDA.P3702_FECHA>='" + fechInicial + "'AND T37_AGENDA.P3702_FECHA<='" + fechFin + "'and T37_AGENDA.P3706_USUARIO='" + usuario + "'";
                        }
                        break;
                }                
                DataTable DtDatos = qry_Mant.DTArrendadora(sql);
                DataView DvDatos = new DataView(DtDatos);
                DvDatos.Sort = "Fecha ASC";
                DtDatos = DvDatos.ToTable();
                if (DtDatos.Rows.Count > 0)
                {
                    TfrxReportClass reporte = new TfrxReportClass();
                    reporte.ClearReport();
                    reporte.ClearDatasets();
                    reporte.LoadReportFromFile(_rutaformato);
                    FrxDataTable dtInmobiliaria = new FrxDataTable("dvInmobiliaria");
                    DataColumn dcInmobiliaraNombreComercial = new DataColumn("nombreComercial", typeof(string));
                    DataColumn dcFechaIni = new DataColumn("Fecha_Inicial", typeof(string));
                    DataColumn dcFechaFin = new DataColumn("Fecha_Final", typeof(string));
                    DataColumn dcMes = new DataColumn("mes", typeof(string)); // prueba calendario                

                    dtInmobiliaria.Columns.Add(dcInmobiliaraNombreComercial);
                    dtInmobiliaria.Columns.Add(dcFechaIni);
                    dtInmobiliaria.Columns.Add(dcFechaFin);
                    if (EsCalendario)
                    {
                        dtInmobiliaria.Columns.Add(dcMes);
                    }
                    dtInmobiliaria.AcceptChanges();
                    if (EsCalendario)
                        dtInmobiliaria.Rows.Add(inmobiliaria, dateTimePicker1.Value.ToShortDateString(), dateTimePicker2.Value.ToShortDateString(), dateTimePicker1.Value.ToString("MMMM yyyy").ToUpper());
                    else
                        dtInmobiliaria.Rows.Add(inmobiliaria, dateTimePicker1.Value.ToShortDateString(), dateTimePicker2.Value.ToShortDateString());

                    FrxDataTable dtMant = new FrxDataTable("dvMant");
                    if (EsCalendario)
                    {
                        try
                        {
                            int ren = 1;
                            int col = 1;
                            DateTime primeroMes = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, 1);
                            col = Convert.ToInt32(primeroMes.DayOfWeek) + 1;                           
                            for (int i = 1; i <= DateTime.DaysInMonth(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month); i++)                            
                            {
                                int contDay = 0;
                                if (col > 7)
                                {
                                    col = 1;
                                    ren++;
                                }
                                string memoDiaStr = "memoDia" + ren + col;
                                IfrxComponent memoDia = reporte.FindObject(memoDiaStr);
                                (memoDia as IfrxCustomMemoView).Text = i.ToString();
                                DateTime fechaHoy = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, i);
                                foreach (DataRow row in DtDatos.Rows)
                                {
                                    DateTime fechaBd = (DateTime)row["Fecha"];
                                    fechaBd = new DateTime(fechaBd.Year, fechaBd.Month, fechaBd.Day);
                                    if (fechaBd == fechaHoy)
                                    {
                                        contDay++;
                                        string memoActStr = "memoAct" + ren + col;
                                        IfrxComponent memoAct = reporte.FindObject(memoActStr); 
                                        if(row["Estatus"].ToString() == "N")
                                            (memoAct as IfrxCustomMemoView).Text += "~" + row["Motivo"].ToString() + Environment.NewLine;
                                        else
                                            (memoAct as IfrxCustomMemoView).Text += "X " + row["Motivo"].ToString() + Environment.NewLine;
                                        if (contDay == 6)
                                        {
                                            (memoDia as IfrxCustomMemoView).Text = "*" + (memoDia as IfrxCustomMemoView).Text;
                                        }
                                    }
                                }                                
                                col++;                                
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Problema al asignar calendario", "Mantenimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(0);
                        }

                        DataColumn dcMantUser = new DataColumn("MantUser", typeof(string));
                        dtMant.Columns.Add(dcMantUser);
                        dtMant.AcceptChanges();
                        dtMant.Rows.Add(1);//para calendario
                    }
                    else
                    {
                        DataColumn dcMantTipo = new DataColumn("MantTipo", typeof(string));
                        DataColumn dcMantFecha = new DataColumn("MantFecha", typeof(string));// linea agregada
                        DataColumn dcMantActiv = new DataColumn("MantDescrip", typeof(string));
                        DataColumn dcMantConj = new DataColumn("MantConjunto", typeof(string));
                        DataColumn dcMantObs = new DataColumn("MantObs", typeof(string));
                        DataColumn dcMantUser = new DataColumn("MantUser", typeof(string));

                        dtMant.Columns.Add(dcMantTipo);
                        dtMant.Columns.Add(dcMantFecha); // linea agregada
                        dtMant.Columns.Add(dcMantActiv);
                        dtMant.Columns.Add(dcMantConj);
                        dtMant.Columns.Add(dcMantObs);
                        dtMant.Columns.Add(dcMantUser);
                        dtMant.AcceptChanges();
                        foreach (DataRow drDatos in DtDatos.Rows)
                        {
                            try
                            {
                                dtMant.Rows.Add(drDatos["Tipo"].ToString(), Convert.ToDateTime(drDatos["Fecha"].ToString()).ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("es-ES")), drDatos["Motivo"].ToString(), drDatos["Descripcion"].ToString(), Encoding.UTF8.GetString((byte[])drDatos["Observacion"]), drDatos["Usuario"].ToString());
                            }
                            catch
                            {
                                dtMant.Rows.Add(drDatos["Tipo"].ToString(), Convert.ToDateTime(drDatos["Fecha"].ToString()).ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("es-ES")), drDatos["Motivo"].ToString(), drDatos["Descripcion"].ToString(), string.Empty, drDatos["Usuario"].ToString());
                            }
                        }
                    }
                    FrxDataView dvInmobiliaria = new FrxDataView(dtInmobiliaria, "dvInmobiliaria");
                    FrxDataView dvMant = new FrxDataView(dtMant, "dvMant");

                    dvInmobiliaria.AssignToReport(true, reporte);
                    dvMant.AssignToReport(true, reporte);

                    dtInmobiliaria.AssignToDataBand("MasterData1", reporte); //Parche para que aparezca encabezado de reporte

                    dtMant.AssignToDataBand("MasterData2", reporte);
                    reporte.PrepareReport(true);
                    if (string.IsNullOrEmpty(formato))
                    {
                        reporte.ShowPreparedReport();
                    }
                    else if (formato == "xls")
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
                }
                else
                {
                    MessageBox.Show("No se encontraron registros para los criterios seleccionados", "Mantenimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("Error general al generar reporte", "Mantenimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/

        private void Frm_RepMant_Load_1(object sender, EventArgs e)
        {
            
            cargarinmobiliaria();
            Cargar_Usuarios();
            Cmb_Usuario.Enabled = false;
            Cmb_Stat.Enabled = false;            
        }
                
        private void Rb_Todos_CheckedChanged(object sender, EventArgs e)
        {            
            Cmb_Usuario.Enabled = !Rb_Todos.Checked;     
        }

        private void Cargar_Usuarios()
        {
           Cmb_Usuario.DataSource = Mantenimiento.obtenerUsuarios(); 
        }

        private void Rb_AllSta_CheckedChanged(object sender, EventArgs e)
        {
            Cmb_Stat.Enabled = !Rb_AllSta.Checked;
            Cmb_Stat.SelectedIndex = 0;
            //rdBtnCalendar.Enabled = true;            
        }

        private void rdBtnCalendar_CheckedChanged(object sender, EventArgs e)
        {
            label3.Visible = !rdBtnCalendar.Checked;
            dateTimePicker2.Visible = !rdBtnCalendar.Checked;
            label2.Text = rdBtnCalendar.Checked ? "Fecha de mes:" : "Fecha Inicial:";            
        }

        private void Cmb_Stat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Cmb_Stat.Text == "Vencidos")
            {
                label3.Visible = false;
                dateTimePicker2.Visible = false;
                label2.Text = "Al: ";
                if (Rb_Stat.Checked)
                {
                    rdBtnDetallado.Checked = true;
                    rdBtnCalendar.Enabled = false;
                }
                else
                {
                    rdBtnCalendar.Enabled = true;
                }
            }
            else
            {
                label3.Visible = true;
                dateTimePicker2.Visible = true;
                label2.Text = "Fecha Inicial";
                rdBtnCalendar.Enabled = true;
            }
        }

        private void workerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
            error = mantenimiento.generar();
        }

        private void workerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void workerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(error))
                MessageBox.Show("¡Reporte generado correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo generar el reporte de mantenimiento: " + error , this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void Frm_RepMant_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {            
            if (rdBtnCalendar.Checked)
                rutaformato = rutaformato.Replace("Mantenimiento.fr3", "Calendario.fr3");
            else
                rutaformato = rutaformato.Replace("Calendario.fr3", "Mantenimiento.fr3");
            if (!System.IO.File.Exists(rutaformato))
            {
                MessageBox.Show("No existe el formato de plantilla en ubicación: \"" + rutaformato + ". Comuniquese con el administrador del sistema.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //this.Close();
                return ;
            }            
            botonGenerar.Enabled = false;
            try
            {
                string status = Rb_AllSta.Checked ? string.Empty : Cmb_Stat.SelectedItem.ToString();
                string usuario = Rb_Todos.Checked ? string.Empty : Cmb_Usuario.SelectedItem.ToString();
                mantenimiento = new Mantenimiento(comboBox1.SelectedItem as InmobiliariaEntity, status, usuario, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, rdBtnCalendar.Checked, radioPDF.Checked, rutaformato);
                mantenimiento.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(mantenimiento_CambioProgreso);
                workerReporte.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un problema al generar el reporte. Error: " + ex.Message + ". Comuniquese con el administrador del sistema.", "Reporte de Mantenimiento");
            }
        }

        void mantenimiento_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {

        }
    }
}
