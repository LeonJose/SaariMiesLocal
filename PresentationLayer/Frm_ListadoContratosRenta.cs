using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.Helpers;


namespace GestorReportes.PresentationLayer
{
    public partial class Frm_ListadoContratosRenta : Form
    {
        private string rutaFormato = string.Empty;
        private string usuario = string.Empty;
        private string result = string.Empty;
        ListadoContratosRenta reporte = null;
        private bool permisosInmobiliarias = Properties.Settings.Default.PermisosInmobiliaria;


        public Frm_ListadoContratosRenta(string rutaFormato, string usuario)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
        }

        private void Frm_ListadoContratos_Load(object sender, EventArgs e)
        {
            comboInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuario, permisosInmobiliarias); //ListadoContratosRenta.obtenerInmobiliarias();
            comboInmobiliaria.ValueMember = "ID";
            comboInmobiliaria.DisplayMember = "RazonSocial";

            comboConjunto.DataSource = ListadoContratosRenta.obtenerConjuntos(comboInmobiliaria.SelectedValue.ToString());
            comboConjunto.ValueMember = "ID";
            comboConjunto.DisplayMember = "Nombre";

            groupBox3.Enabled= false;


        }

        private void comboInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboConjunto.DataSource = ListadoContratosRenta.obtenerConjuntos(comboInmobiliaria.SelectedValue.ToString());
            comboConjunto.ValueMember = "ID";
            comboConjunto.DisplayMember = "Nombre";
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            try
            {
                progreso.Value = 0;
                botonGenerar.Enabled = false;
                //Fecha Corte

                //DateTime fechaFin = dtpFechaFin.Value.Date.AddHours(12);
                DateTime fechaFin = dtpFechaFin.Value.Date;
                reporte = new ListadoContratosRenta(comboInmobiliaria.SelectedValue.ToString(), comboConjunto.SelectedValue.ToString(), fechaFin, usuario, rutaFormato, radioPDF.Checked, radioTodos.Checked, checkIncluirCargos.Checked, radioVigentesyHistorial.Checked);
                reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
                workerReporte.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al ejecutar el reporte: " + Environment.NewLine + ex.Message, "Listado Contratos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void reporte_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        private void workerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
            result = reporte.generar();
        }

        private void workerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void workerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("¡Reporte generado correctamente!", "Listado de Contratos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No se pudo generar el reporte: " + Environment.NewLine + result, "Listado de Contratos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
            {
                this.Close();
            }
            else
            {
                reporte.cancelar();
            }
            botonCancelar.Enabled = false;
        }

        private void Frm_ListadoContratos_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

        private void radioVigentesyHistorial_CheckedChanged(object sender, EventArgs e)
        {
            if (radioVigentesyHistorial.Checked)
            {
                groupBox3.Enabled = true;
            }
            else
            {
                groupBox3.Enabled = false;
            }
        }
    }
}
