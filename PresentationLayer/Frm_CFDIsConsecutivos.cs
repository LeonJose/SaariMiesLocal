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
    public partial class Frm_CFDIsConsecutivos : Form
    {
        private string rutaFormato = string.Empty, usuario = string.Empty, result = string.Empty;
        Inmobiliaria inmobiliaria = new Inmobiliaria();
        CFDIsConsecutivos reporte = null;
        private bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;
        string IdInmobiliaria = string.Empty; string idConjunto = string.Empty;
        string NombreInmo = string.Empty; string NombreConjunto = string.Empty;

        public Frm_CFDIsConsecutivos(string rutaFormato, string usuario)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
        }


        private void Frm_CFDIsConsecutivos_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateTimePicker2.Value = dateTimePicker1.Value.AddMonths(1).AddDays(-1);
            cargarInmobiliarias();
            cargarConjuntos(IdInmobiliaria);

            //comboInmobiliaria.DataSource = inmobiliaria.getInmobiliariasCommand("Todos");
            //comboInmobiliaria.DisplayMember = "P0103_RAZON_SOCIAL";
            //comboInmobiliaria.ValueMember = "P0101_ID_ARR";
            //comboConjunto.DataSource = inmobiliaria.getConjuntosCommand(comboInmobiliaria.SelectedValue.ToString());
            //comboConjunto.DisplayMember = "P0303_NOMBRE";
            //comboConjunto.ValueMember = "P0301_ID_CENTRO";
        }
        public void cargarInmobiliarias()
        {
            try
            {
                comboInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuario, permisosInmo);
                comboInmobiliaria.DisplayMember = "RazonSocial";
                comboInmobiliaria.ValueMember = "ID";
                comboInmobiliaria.SelectedIndex = 0;
                IdInmobiliaria = comboInmobiliaria.SelectedValue.ToString();
                NombreInmo = comboInmobiliaria.Text.ToString();
            }
            catch
            {
                result = "Error al obtener las inmobiliarias.";
            }
        }

        public void cargarConjuntos(string selectedValue)
        {
            IdInmobiliaria = comboInmobiliaria.SelectedValue.ToString();
            comboConjunto.DataSource = HelpInmobiliarias.getConjuntos(selectedValue); //.ObtenerConjuntos(iDinmobiliaria);
            comboConjunto.DisplayMember = "Nombre";
            comboConjunto.ValueMember = "ID";
            idConjunto = comboConjunto.SelectedValue.ToString();
            NombreConjunto = comboConjunto.Text.ToString();
            comboConjunto.SelectedIndex = 0;
        }


        private void comboInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                IdInmobiliaria = comboInmobiliaria.SelectedValue.ToString();
                cargarConjuntos(IdInmobiliaria);
                //comboConjunto.DataSource = inmobiliaria.getConjuntosCommand(comboInmobiliaria.SelectedValue.ToString());
                //comboConjunto.DisplayMember = "P0303_NOMBRE";
                //comboConjunto.ValueMember = "P0301_ID_CENTRO";
            }
            catch
            { }
        }

        private void botonGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                progreso.Value = 0;
                botonGenerar.Enabled = false;                
                reporte = new CFDIsConsecutivos(comboInmobiliaria.SelectedValue.ToString(), comboConjunto.SelectedValue.ToString(), dateTimePicker1.Value, dateTimePicker2.Value, usuario, rutaFormato, radioPDF.Checked);
                reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
                workerReporte.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al ejecutar reporte:" + Environment.NewLine + ex.Message, "CFDi's consecutivos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                botonGenerar.Enabled = true;
                botonCancelar.Enabled = true;
            }
        }

        void reporte_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                reporte.cancelar();
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
                MessageBox.Show("¡Reporte generado correctamente!", "CFDi's consecutivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo generar el reporte: " + Environment.NewLine + result, "CFDi's consecutivos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void Frm_CFDIsConsecutivos_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

        private void comboInmobiliaria_SelectedValueChanged(object sender, EventArgs e)
        {
           // cargarConjuntos();
        }

        private void botonGenerar_Load(object sender, EventArgs e)
        {

        }
    }
}
