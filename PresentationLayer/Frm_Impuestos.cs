using GestorReportes.BusinessLayer.ComponentLayer;
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
    public partial class Frm_Impuestos : Form
    {

        private string rutaFormato = string.Empty;
        private string usuario = string.Empty;
        private bool permisosInmobiliaria = false;
        private string idInmobiliaria = string.Empty;
        private string idConjunto = string.Empty;
        private bool esPDF = true;
        private string error = string.Empty;
        ImpuestosReporte reporte = null;

        public Frm_Impuestos(string rutaFormato, string usuario)
        {
            InitializeComponent();
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            this.permisosInmobiliaria = GestorReportes.Properties.Settings.Default.PermisosInmobiliaria;
        }
        
        private void Frm_Impuestos_Load(object sender, EventArgs e)
        {
            comboInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuario, permisosInmobiliaria);//CarteraVencida306090.obtenerInmobiliarias();
            comboInmobiliaria.ValueMember = "ID";
            comboInmobiliaria.DisplayMember = "RazonSocial";

            comboConjunto.DataSource = HelpInmobiliarias.getConjuntos(comboInmobiliaria.SelectedValue.ToString()); //CarteraVencida306090.obtenerConjuntos(comboInmobiliaria.SelectedValue.ToString());
            comboConjunto.ValueMember = "ID";
            comboConjunto.DisplayMember = "Nombre";
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            idInmobiliaria = comboInmobiliaria.SelectedValue.ToString();
            idConjunto = comboConjunto.SelectedValue.ToString();
            string nombreInmo = comboInmobiliaria.Text;
            string nombreConjunto = comboInmobiliaria.Text;
            reporte = new ImpuestosReporte(idInmobiliaria, idConjunto,radioPDF.Checked, nombreInmo,nombreConjunto, dtpInicio.Value, dtpFin.Value,rutaFormato);
            reporte.CambioProgreso += new EventHandler<CambioProgresoEventArgs>(reporte_CambioProgreso);
            workerReporte.RunWorkerAsync();
        }

        private void comboInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboConjunto.DataSource = HelpInmobiliarias.getConjuntos(comboInmobiliaria.SelectedValue.ToString());
            comboConjunto.ValueMember = "ID";
            comboConjunto.DisplayMember = "Nombre";
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            error = reporte.generar();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (string.IsNullOrEmpty(error))
            {
                progreso.Value = 100;

                MessageBox.Show("¡Reporte generado correctamente!", "Reporte de Impuestos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Error al generar el reporte: \n" + error, "Reporte de Impuestos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
            progreso.Value = 0;
        }
        void reporte_CambioProgreso(object sender, CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }
    }
}
