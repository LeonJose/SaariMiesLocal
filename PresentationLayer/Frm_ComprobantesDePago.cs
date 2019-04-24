using GestorReportes.BusinessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.ComponentLayer;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;


namespace GestorReportes.PresentationLayer
{
    public partial class Frm_ComprobantesDePago : Form
    {
        private string rutaFormato = string.Empty;
        string usuario = string.Empty;
        string result = string.Empty;
        string idConjunto = string.Empty;
        string NombreInmo = string.Empty;
        string NombreConjunto = string.Empty;
        private bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;
        string inmb = string.Empty;

        InmobiliariaEntity IdInmobiliaria = null;
        ComprobantePago reporte = null;
        ConjuntoEntity Conjunt;
        string error;

        public Frm_ComprobantesDePago()
        {
            InitializeComponent();

        }

        public Frm_ComprobantesDePago(string rutaFormato, string usuario)
        {
            InitializeComponent();
            this.rutaFormato = rutaFormato;
            Conjunt = new ConjuntoEntity();
            IdInmobiliaria = new InmobiliariaEntity();
            this.usuario = usuario;
        }

        private void Frm_ComprobantesDePago_Load(object sender, EventArgs e)
        {
            cargarInmobiliarias();
            // cargarConjuntos(inmb);
            DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpinicio.Value = fecha;
        }

        private void cargarInmobiliarias()
        {
            try
            {
                cbxInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuario, permisosInmo);
                inmb = cbxInmobiliaria.SelectedValue.ToString();
                cbxInmobiliaria.DisplayMember = "RazonSocial";
                cbxInmobiliaria.ValueMember = "IdArrendadora";
                cbxInmobiliaria.SelectedIndex = 0;
                inmb = cbxInmobiliaria.SelectedValue.ToString();
                NombreInmo = cbxInmobiliaria.Text.ToString();
            }
            catch
            {
                result = "Error al obtener las inmobiliarias.";
            }
        }

        private void cargarConjuntos(string selectedValue)
        {
            try
            {

                cbxConjunto.DataSource =  HelpInmobiliarias.getConjuntos(selectedValue);
                inmb = cbxInmobiliaria.SelectedValue.ToString();
                cbxConjunto.DisplayMember = "Nombre";
                cbxConjunto.ValueMember = "ID";
                cbxConjunto.SelectedIndex = 0;
                inmb = cbxConjunto.SelectedValue.ToString();
                NombreConjunto = cbxConjunto.Text.ToString();
            }
            catch
            {

            }
        }

        void ComprobantePago(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            worker.ReportProgress(e.Progreso);
        }
        private void worker_DoWork_1(object sender, DoWorkEventArgs e)
        {
            error = reporte.generarReporte();
        }

        private void worker_ProgressChanged_1(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e)
        {
            progreso.Value = 100;
            if (string.IsNullOrWhiteSpace(error))
            {
                MessageBox.Show("¡Reporte generado correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No se pudo generar el reporte: " + error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            ctrl_OpcionGenerar.Enabled = true;
            ctrl_OpcionCancelar.Enabled = true;
        }

        private void cbxInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var inmob = cbxInmobiliaria.SelectedItem as InmobiliariaEntity;

                cargarConjuntos(inmob.ID);
            }
            catch (Exception ex)
            {

            }
        }

        private void ctrl_OpcionGenerar_ControlClicked(object sender, EventArgs e)
        {
            progreso.Value = 10;
            ctrl_OpcionGenerar.Enabled = false;

            DateTime fechaInicio = dtpinicio.Value.Date;
            DateTime fechaFin = dtpfin.Value.Date.AddHours(12);



            var inmob = cbxInmobiliaria.SelectedItem as InmobiliariaEntity;

            Conjunt = cbxConjunto.SelectedItem as ConjuntoEntity;
            reporte = new ComprobantePago(inmob, Conjunt, fechaInicio, fechaFin, usuario, rutaFormato, rbPDF.Checked, worker, rbConDetalle.Checked);
            reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(ComprobantePago);
            worker.RunWorkerAsync();
        }

        private void ctrl_OpcionCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (ctrl_OpcionGenerar.Enabled)
            {
                this.Close();
            }
            else
            {
                reporte.cancelar();
            }
            ctrl_OpcionCancelar.Enabled = false;
        }

       
    }
}
