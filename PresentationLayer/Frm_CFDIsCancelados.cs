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
    public partial class Frm_CFDIsCancelados : Form
    {
        private string rutaFormato = string.Empty;
        private string usuario = string.Empty;
        private string result = string.Empty;
        private bool permisos = GestorReportes.Properties.Settings.Default.PermisosInmobiliaria;
        CFDIsCancelados reporte = null;
        
        
        public Frm_CFDIsCancelados(string rutaFormato, string usuario)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);            
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
        }

        private void Frm_RecibosCancelados_Load(object sender, EventArgs e)
        {
            
            dateTimePickerInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateTimePickerFin.Value = dateTimePickerInicio.Value.AddMonths(1).AddDays(-1);
            try
            {
                comboInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuario, permisos);
                comboInmobiliaria.ValueMember = "ID";
                comboInmobiliaria.DisplayMember = "RazonSocial";

                comboConjunto.DataSource = CFDIsCancelados.obtenerConjuntos(comboInmobiliaria.SelectedValue.ToString());
                comboConjunto.ValueMember = "ID";
                comboConjunto.DisplayMember = "Nombre";
            }
            catch
            {
                result = "Error al obtener las inmobiliarias";
            }
        }

        private void comboInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboConjunto.DataSource = CFDIsCancelados.obtenerConjuntos(comboInmobiliaria.SelectedValue.ToString());
            comboConjunto.ValueMember = "ID";
            comboConjunto.DisplayMember = "Nombre";
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            try
            {
                progreso.Value = 0;
                botonGenerar.Enabled = false;
                reporte = new CFDIsCancelados(comboInmobiliaria.SelectedValue.ToString(), comboConjunto.SelectedValue.ToString(), dateTimePickerInicio.Value, dateTimePickerFin.Value, usuario, rutaFormato, radioPDF.Checked, checkProcesados.Checked);
                reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
                workerReporte.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al ejecutar el reporte: " + Environment.NewLine + ex.Message, "CFDis Cancelados", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void reporte_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
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
                MessageBox.Show("¡Reporte generado correctamente!", "CFDIs Cancelados", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No se pudo generar el reporte: " + Environment.NewLine + result, "CFDIs Cancelados", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void Frm_CFDIsCancelados_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }
    }
}
