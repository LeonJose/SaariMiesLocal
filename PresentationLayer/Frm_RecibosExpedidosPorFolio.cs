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
    public partial class Frm_RecibosExpedidosPorFolio : Form
    {
        private string rutaFormato = string.Empty;
        private string usuario = string.Empty;
        private string result = string.Empty;
        RecibosExpedidosPorFolio reporte = null;
        private bool EsVentas = false;
        private bool EsParcialidad = false;
        private string TipoDocVentas=string.Empty;
        private bool permisosInmo = GestorReportes.Properties.Settings.Default.PermisosInmobiliaria;

        public Frm_RecibosExpedidosPorFolio(string rutaFormato, string usuario)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this); 
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
        }

        public Frm_RecibosExpedidosPorFolio(string rutaFormato, string usuario,bool esVentas, bool esParcialidad)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this); 
            SaariIcon.SaariIcon.setSaariIcon(this);
           // this.Text = "Recibos de Ventas";
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            EsVentas = esVentas;
            if (EsVentas)
                checkDetalleConceptos.Visible = false;
            EsParcialidad = esParcialidad;
            if (esVentas)
            {
                this.TipoDocVentas = EsParcialidad ? "J" : "I";
            }
            else
                TipoDocVentas = "R";
            switch (TipoDocVentas)
            {
                case "I":
                    this.Text = "Recibos de Ventas (Pago en una sola exhibición)";
                    esVentas = true;
                    break;
                case "J":
                    this.Text = "Recibos de Ventas en Parcialidades";
                    esVentas = true;
                    break;
                case "R" :
                    break;
                default:
                    MessageBox.Show("Funcionalidad no disponible en Saari.", "Saari", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    esVentas = false;
                    this.Close();
                    break;
            }
            
        }

        private void Frm_RecibosExpedidosPorFolio_Load(object sender, EventArgs e)
        {
            dateTimePickerInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateTimePickerFin.Value = dateTimePickerInicio.Value.AddMonths(1).AddDays(-1);

            comboInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuario, permisosInmo);
            comboInmobiliaria.ValueMember = "ID";
            comboInmobiliaria.DisplayMember = "RazonSocial";

            comboConjunto.DataSource = RecibosExpedidosPorFolio.obtenerConjuntos(comboInmobiliaria.SelectedValue.ToString());
            comboConjunto.ValueMember = "ID";
            comboConjunto.DisplayMember = "Nombre";
        }

        private void comboInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboConjunto.DataSource = RecibosExpedidosPorFolio.obtenerConjuntos(comboInmobiliaria.SelectedValue.ToString());
            comboConjunto.ValueMember = "ID";
            comboConjunto.DisplayMember = "Nombre";
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            try
            {
                progreso.Value = 0;
                botonGenerar.Enabled = false;
                if (EsVentas)
                {
                    reporte = new RecibosExpedidosPorFolio(comboInmobiliaria.SelectedValue.ToString(), comboConjunto.SelectedValue.ToString(), dateTimePickerInicio.Value, dateTimePickerFin.Value, usuario, rutaFormato, radioPDF.Checked, checkDetalleConceptos.Checked, EsVentas, EsParcialidad);
                }
                else
                {
                    reporte = new RecibosExpedidosPorFolio(comboInmobiliaria.SelectedValue.ToString(), comboConjunto.SelectedValue.ToString(), dateTimePickerInicio.Value, dateTimePickerFin.Value, usuario, rutaFormato, radioPDF.Checked, checkDetalleConceptos.Checked);
                }
                //reporte = new RecibosExpedidosPorFolio(comboInmobiliaria.SelectedValue.ToString(), comboConjunto.SelectedValue.ToString(), dateTimePickerInicio.Value, dateTimePickerFin.Value, usuario, rutaFormato, radioPDF.Checked, checkDetalleConceptos.Checked);
                reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
                workerReporte.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al ejecutar el reporte: " + Environment.NewLine + ex.Message, "Recibos Expedidos por Folio", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            //result = reporte.generar();
            result = reporte.generarReporte();
        }

        private void workerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void workerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("¡Reporte generado correctamente!", "Recibos Expedidos por Folio", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No se pudo generar el reporte: " + Environment.NewLine + result, "Recibos Expedidos por Folio", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void Frm_RecibosExpedidosPorFolio_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

        
    }
}
