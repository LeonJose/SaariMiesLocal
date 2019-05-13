using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_ReporteCuentasPorCobrar : Form
    {
        private string rutaformato = string.Empty, usuario = string.Empty, idInmobiliaria = string.Empty, idConjunto = string.Empty;
        private bool esPDF = true;
        string error;
        InmobiliariaEntity Inmob = null;
        ReporteCuentasPorCobrar reporteCuentas = null;
        DateTime fechacorte = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        public Frm_ReporteCuentasPorCobrar()
        {
            InitializeComponent();
        }

        public Frm_ReporteCuentasPorCobrar(string formato, string user)
        {
            InitializeComponent();
            rutaformato = formato;
            this.usuario = user;
            Inmob = new InmobiliariaEntity();
        }
        private void Frm_ReporteCuentasPorCobrar_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = fechacorte;
            CargarInmobiliarias();
        }
        private void CargarInmobiliarias()
        {
            try
            {
                CbxInmobiliarias.DataSource = SaariDB.getListaInmobiliarias();
                CbxInmobiliarias.ValueMember = "ID";
                CbxInmobiliarias.DisplayMember = "RazonSocial";
            }
            catch (Exception ex)
            {

            }
        }
        private void CargarConjuntos()
        {
            try
            {
                CbxConjuntos.DataSource = SaariDB.getListaConjuntos(Inmob.ID);
                CbxConjuntos.ValueMember = "ID";
                CbxConjuntos.DisplayMember = "Nombre";
                CbxConjuntos.SelectedIndex = 0;
            }
            catch (Exception ex)
            {

            }
            
        }
        void ReporteCuentasPorCobrar(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            Worker.ReportProgress(e.Progreso);
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Value = 10;
                error = string.Empty;
                idInmobiliaria = CbxInmobiliarias.SelectedValue.ToString();
                idConjunto = CbxConjuntos.SelectedValue.ToString();
                esPDF = true;
                fechacorte = dateTimePicker1.Value.Date;
                botonGenerar.Enabled = false;
                reporteCuentas = new ReporteCuentasPorCobrar(idInmobiliaria, idConjunto,usuario, rutaformato, radioPDF.Checked, fechacorte);
                reporteCuentas.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(ReporteCuentasPorCobrar);
                Worker.RunWorkerAsync();

            }
            catch (Exception ex)
            {

            }
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            error = reporteCuentas.GenerarReporte();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                reporteCuentas.cancelar();
            botonCancelar.Enabled = false;
        }

        private void botonGenerar_Load(object sender, EventArgs e)
        {

        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 100;
            if (string.IsNullOrWhiteSpace(error))
            {
                MessageBox.Show("¡Reporte generado correctamente!","Saari" , MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No se pudo generar el reporte  :" + error, "Saari", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void CbxInmobiliarias_SelectedIndexChanged(object sender, EventArgs e)
        {
            Inmob = CbxInmobiliarias.SelectedItem as InmobiliariaEntity;
            CargarConjuntos();
        }               
    }
}
