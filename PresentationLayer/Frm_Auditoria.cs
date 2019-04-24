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
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_Auditoria : Form
    {
        private string rutaFormato = string.Empty;
        private string usuario = string.Empty;
        private AuditoriaContratos reporte = null;
        private string error = string.Empty;
        //private string idInmobiliaria = string.Empty;
        private DateTime fechaCorte = new DateTime();
        private bool esExcel = true;
        private bool EsPreventa = false;
        private bool permisos = GestorReportes.Properties.Settings.Default.PermisosInmobiliaria;

        /// <summary>
        /// Constructor que inicializa la ruta del formato y el usuario que esta generando el reporte
        /// </summary>
        /// <param name="rutaFormato">Ruta del formato del reporte de auditoria de contratos de venta (.frx)</param>
        /// <param name="usuario">Usuario que se encuentra firmado en la aplicación</param>
        public Frm_Auditoria(string rutaFormato, string usuario)
        {
            InitializeComponent();
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            EsPreventa = false;            
        }
        /// <summary>
        /// Constructor que inicializa la ruta del formato, el usuario y si es reporte de preventa
        /// </summary>
        /// <param name="rutaFormato">Ruta del formato del reporte de auditoria de contratos de venta (.frx)</param>
        /// <param name="usuario">Usuario que se encuentra firmado en la aplicación</param>
        /// <param name="esPreventa">True=Preventa  False=Venta</param>
        public Frm_Auditoria(string rutaFormato, string usuario, bool esPreventa)
        {
            InitializeComponent();
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            EsPreventa = esPreventa;
        }

        private void Frm_Auditoria_Load(object sender, EventArgs e)
        {
            this.Text += EsPreventa ? " Preventa" : " Venta";
            comboInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuario, permisos);// AuditoriaContratos.obtenerInmobiliarias();
            comboInmobiliaria.ValueMember = "ID";
            comboInmobiliaria.DisplayMember = "RazonSocial";            
        }                

        private void botonGenerar_Click(object sender, EventArgs e)
        {
            try
            {                
                progreso.Value = 0;
                error = string.Empty;
                esExcel = radioExcel.Checked;
                fechaCorte = dateTimePicker1.Value.Date;                
                botonGenerar.Enabled = false;
                //reporte = new AuditoriaContratos(comboInmobiliaria.SelectedItem as InmobiliariaEntity, fechaCorte.Date, esExcel, rutaFormato, usuario);
                reporte = new AuditoriaContratos(comboInmobiliaria.SelectedItem as InmobiliariaEntity, fechaCorte.Date, esExcel, rutaFormato, usuario, EsPreventa, checkBoxAbrirReporteAutomatico.Checked);
                reporte.CambioProgreso += new EventHandler<CambioProgresoEventArgs>(reporte_CambioProgreso);
                workerReporte.RunWorkerAsync();
            }
            catch { }
        }

        void reporte_CambioProgreso(object sender, CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        private void botonCancelar_Click(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                reporte.cancelar();
            botonCancelar.Enabled = false;
        }

        private void workerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
            error = reporte.generar();
        }

        private void workerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void workerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string tipoReporte = EsPreventa ? "preventa" : "venta";
            if (string.IsNullOrEmpty(error))
                MessageBox.Show("Reporte generado correctamente!", "Auditoria de contratos de "+ tipoReporte, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if(reporte.EsError)
                    MessageBox.Show("Error al generar el reporte: " + Environment.NewLine + error, "Auditoria de contratos de "+tipoReporte, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Reporte no generado: " + Environment.NewLine + error, "Auditoria de contratos de " + tipoReporte, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void Frm_Auditoria_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }
    }
}
