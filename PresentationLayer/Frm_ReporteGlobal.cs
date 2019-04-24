using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;

using FastReport;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.PresentationLayer;
using GestorReportes.BusinessLayer.Helpers;
using GestorReportes.Properties;

using System.Windows.Forms;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_ReporteGlobal : Form
    {
        InmobiliariaEntity inmobiliariaObj = new InmobiliariaEntity();
        private ReporteGlobal BusinesLayerObj = null;
        private AnalisisDeudores BusinesLayerObjAnalisis = null;
        string error = string.Empty;
        DateTime FechaCorte = DateTime.Now;
        string idInmobiliaria = string.Empty;
        string idConjuntoo = string.Empty;
        string nombreInmobiliaria = string.Empty;
        bool esPDF = false;
        string user = string.Empty;
        bool cancelUser = false;
        bool permisosInmo = false;
        string nombreConjunto = string.Empty;
        Stopwatch stopWatch = new Stopwatch();

        public Frm_ReporteGlobal(string user)
        {
            this.user = user;
            this.permisosInmo = GestorReportes.Properties.Settings.Default.PermisosInmobiliaria;
            InitializeComponent();
            SaariIcon.SaariIcon.setSaariIcon(this);
        }


        private void cargarInmobiliarias()
        {
            try
            {
                comboInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(user, permisosInmo);//ReporteGlobal.obtenerInmobiliarias();
                comboInmobiliaria.DisplayMember = "RazonSocial";
                comboInmobiliaria.ValueMember = "ID";
                comboInmobiliaria.SelectedIndex = 0;
                idInmobiliaria = comboInmobiliaria.SelectedValue.ToString();
            }
            catch
            {

            }
        }

        private void cargarConjunto()
        {
            try
            {
                comboConjunto.DataSource = ReporteGlobal.obtenerConjunto(comboInmobiliaria.SelectedValue.ToString());
                comboConjunto.DisplayMember = "Nombre";
                comboConjunto.ValueMember = "ID";
                idConjuntoo = comboConjunto.SelectedValue.ToString();
            }
            catch
            {
            }

        }

        private void Frm_ReporteGlobal_Load(object sender, EventArgs e)
        {
            cargarInmobiliarias();

        }

        private void comboInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarConjunto();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            error = BusinesLayerObj.generarReporte();

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progreso.Value = 100;
            if (string.IsNullOrWhiteSpace(error))
            {
                MessageBox.Show("¡Reporte generado correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No se pudo generar el reporte global: " + error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            progreso.Value = 0;
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            botonGenerar.Enabled = false;

            try
            {

                idInmobiliaria = comboInmobiliaria.SelectedValue.ToString();
                idConjuntoo = comboConjunto.SelectedValue.ToString();
                nombreInmobiliaria = comboInmobiliaria.Text;
                FechaCorte = dateTimePicker1.Value;
                esPDF = radioPDF.Checked;
                nombreConjunto = comboConjunto.Text;
                BusinesLayerObj = new ReporteGlobal (idInmobiliaria, nombreInmobiliaria, nombreConjunto, idConjuntoo, FechaCorte, esPDF, backgroundWorker1);//new ReporteGlobal(idInmobiliaria, nombreInmobiliaria,nombreConjunto, idConjuntoo, FechaCorte, esPDF, backgroundWorker1);
                backgroundWorker1.RunWorkerAsync();

            }
            catch
            { }


        }

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {

            if (!botonGenerar.Enabled)
            {
                backgroundWorker1.CancelAsync();
                cancelUser = true;
                botonGenerar.Enabled = true;
            }
            else
            {
                this.Close();
            }
        }

        private void botonGenerar_Load(object sender, EventArgs e)
        {

        }

        private void comboInmobiliaria_SelectedValueChanged(object sender, EventArgs e)
        {

        }
    }
}
