using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.PresentationLayer.Controls;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_ReporteFacturacion : Form
    {
        private string rutaFormato = string.Empty, usuario = string.Empty, result = string.Empty;
        private ReporteFacturacion facturacion = null;
        private bool permisosInmo = GestorReportes.Properties.Settings.Default.PermisosInmobiliaria;

        public Frm_ReporteFacturacion(string rutaFormato, string usuario)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this); 
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
        }

        private void botonCancelar_Click(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                facturacion.cancelar();
            botonCancelar.Enabled = false;
        }

        private void Frm_ReporteFacturacion_Load(object sender, EventArgs e)
        {
            List<InmobiliariaEntity> listaInmobiliarias = HelpInmobiliarias.obtenerInmobiliarias(usuario, permisosInmo);
            if (listaInmobiliarias != null)
            {
                foreach (InmobiliariaEntity inmo in listaInmobiliarias)
                {
                    RTPanel panel = new RTPanel(inmo.ID, inmo.RazonSocial, flowTodas.Width);
                    panel.ControlClicked += new RTPanel.ControlClickedHandler(panel_ControlClicked);
                    flowTodas.Controls.Add(panel);
                }
            }
        }

        void panel_ControlClicked(object sender)
        {
            try
            {
                RTPanel p = (sender as RTPanel);
                if (p != null)
                {
                    if (!p.EstaSeleccionada)
                    {
                        p.mover();
                        flowSeleccionadas.Controls.Add(p);
                        flowTodas.Controls.Remove(p);
                    }
                    else
                    {
                        p.mover();
                        flowTodas.Controls.Add(p);
                        flowSeleccionadas.Controls.Remove(p);
                    }
                }
            }
            catch
            {
                
            }
        }

        private void botonGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                if (flowSeleccionadas.Controls.Count > 0)
                {
                    progreso.Value = 0;
                    List<string> listaInmobiliarias = new List<string>();
                    foreach (Control c in flowSeleccionadas.Controls)
                    {
                        if (c is RTPanel)
                            listaInmobiliarias.Add((c as RTPanel).Identificador.Trim());
                    }

                    botonGenerar.Enabled = false;
                    facturacion = new ReporteFacturacion(dateInicio.Value.Date, dateFin.Value.Date, listaInmobiliarias, usuario, rutaFormato, radioPDF.Checked);
                    facturacion.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(facturacion_CambioProgreso);
                    workerReporte.RunWorkerAsync();
                }
                else
                    MessageBox.Show("Debe haber al menos una inmobiliaria seleccionada", "Reporte de facturación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al ejecutar instrucciones:" + Environment.NewLine + ex.Message, "Reporte de facturación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void facturacion_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        private void workerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
            result = facturacion.generar();
        }

        private void workerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void workerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(result))
                MessageBox.Show("Reporte generado correctamente!", "Reporte de facturación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Error al generar reporte de facturación:" + Environment.NewLine + result, "Reporte de facturación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void Frm_ReporteFacturacion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }
    }
}
