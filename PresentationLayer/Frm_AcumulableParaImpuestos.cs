using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_AcumulableParaImpuestos : Form
    {   //DM 27/07/2018
        private bool permisosInmo;
        private string idInmobiliaria, idConjunto,rutaFormato, user, error;
        private bool cancelUser = false;

        filtroReportes filtro = new filtroReportes();
        AcumulableParaImpuestos reporte = null;
        public Frm_AcumulableParaImpuestos()
        {
            InitializeComponent();
        }
        public Frm_AcumulableParaImpuestos(string formato, string usuario)
        {
            InitializeComponent();
            try
            {
                this.permisosInmo = GestorReportes.Properties.Settings.Default.PermisosInmobiliaria;
            }
            catch { }
            this.rutaFormato = formato;
            this.user = usuario;
            this.Text = this.Text + " - " + usuario;
            SaariIcon.SaariIcon.setSaariIcon(this);
        }

        private void comboInmo_SelectedValueChanged(object sender, EventArgs e)
        {
            cargarConjunto();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {

           error = reporte.generarReporte();
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            botonGenerar.Enabled = false;
            filtro.FechaInicio = dtInicio.Value;
            filtro.FechaFin = dtFin.Value;
            filtro.IdInmobiliaria = idInmobiliaria;
            filtro.NombreInmobiliaria = comboInmo.Text;
            filtro.IdConjunto = comboConjuntos.SelectedValue.ToString(); 
            filtro.NombreConjunto = comboConjuntos.Text;
            filtro.esPDF = radioPDF.Checked;
            filtro.Usuario = user;
            filtro.RutaFormato = rutaFormato;            
            reporte = new AcumulableParaImpuestos(filtro,worker);
            reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(Acumulable_CambioProgreso);
            worker.RunWorkerAsync();
        }
        void Acumulable_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            worker.ReportProgress(e.Progreso);
        }
        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {

            if (!botonGenerar.Enabled)
            {
                worker.CancelAsync();
                cancelUser = true;
                botonGenerar.Enabled = true;
            }
            else
            {
                this.Close();
            }
        }
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progreso.Value = 100;
            if (string.IsNullOrWhiteSpace(error))
            {
                
                MessageBox.Show("¡Reporte generado correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No se pudo generar el reporte Acumulable para impuestos: " + error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            progreso.Value = 0;
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }
        private void cargarInmobiliarias()
        {
            try
            {
                comboInmo.DataSource = HelpInmobiliarias.obtenerInmobiliarias(user, permisosInmo);
                comboInmo.DisplayMember = "RazonSocial";
                comboInmo.ValueMember = "ID";
                comboInmo.SelectedIndex = 0;
                idInmobiliaria = comboInmo.SelectedValue.ToString();
            }
            catch
            {

            }
        }
        private void cargarConjunto()
        {
            try
            {
                comboConjuntos.DataSource = SaariDB.getConjuntos(comboInmo.SelectedValue.ToString());// HelpInmobiliarias.getConjuntos(comboInmo.SelectedValue.ToString());
                comboConjuntos.DisplayMember = "Nombre";
                comboConjuntos.ValueMember = "ID";
                idConjunto = comboConjuntos.SelectedValue.ToString();
            }
            catch
            {
            }

        }
        private void Frm_AcumulableParaImpuestos_Load(object sender, EventArgs e)
        {
            cargarInmobiliarias();
            dtInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            dtFin.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month,daysInMonth);
        }
    }
}
