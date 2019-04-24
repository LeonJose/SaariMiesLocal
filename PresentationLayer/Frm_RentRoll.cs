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
    public partial class Frm_RentRoll : Form
    {
        Inmobiliaria inmobiliaria = new Inmobiliaria();
        private RentRoll reporte = null;
        private string rutaFormato = string.Empty, usuario = string.Empty, result = string.Empty;
        bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;
        private string iDinmobiliaria = string.Empty; string idConjunto = string.Empty;
        private string NombreInmo = string.Empty; string NombreConjunto = string.Empty;
        public Frm_RentRoll(string rutaFormato, string usuario)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
        }
        public void cargarInmobiliarias()
        {
            try
            {
                comboInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuario, permisosInmo);
                comboInmobiliaria.DisplayMember = "RazonSocial";
                comboInmobiliaria.ValueMember = "ID";
                comboInmobiliaria.SelectedIndex = 0;
                iDinmobiliaria = comboInmobiliaria.SelectedValue.ToString();
                NombreInmo = comboInmobiliaria.Text.ToString();
            }
            catch
            {
                result = "Error al obtener las inmobiliarias.";
            }
        }

        public void cargarConjuntos(string selectedValue)
        {
            comboConjunto.DataSource = HelpInmobiliarias.getConjuntos(iDinmobiliaria); //.ObtenerConjuntos(iDinmobiliaria);
            comboConjunto.DisplayMember = "Nombre";
            comboConjunto.ValueMember = "ID";            
            idConjunto = comboConjunto.SelectedValue.ToString();
            NombreConjunto = comboConjunto.Text.ToString();
            comboConjunto.SelectedIndex = 0;
        }


        private void Frm_RentRoll_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateTimePicker2.Value = dateTimePicker1.Value.AddMonths(1).AddDays(-1);

            cargarInmobiliarias();
            cargarConjuntos(iDinmobiliaria);// 1

            //comboInmobiliaria.DataSource = inmobiliaria.getInmobiliariasCommand("Todos");
            //comboInmobiliaria.DisplayMember = "P0103_RAZON_SOCIAL";
            //comboInmobiliaria.ValueMember = "P0101_ID_ARR";

            //comboConjunto.DataSource = inmobiliaria.getConjuntosCommand(comboInmobiliaria.SelectedValue.ToString());
            //comboConjunto.DisplayMember = "P0303_NOMBRE";
            //comboConjunto.ValueMember = "P0301_ID_CENTRO";
        }

        private void comboInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                iDinmobiliaria =comboInmobiliaria.SelectedValue.ToString().Trim();
                NombreInmo = comboInmobiliaria.Text.ToString();
               cargarConjuntos(iDinmobiliaria);
                //comboConjunto.DataSource = inmobiliaria.getConjuntosCommand(comboInmobiliaria.SelectedValue.ToString());
                //comboConjunto.DisplayMember = "P0303_NOMBRE";
                //comboConjunto.ValueMember = "P0301_ID_CENTRO";
            }
            catch
            {
                idConjunto = string.Empty;
            }
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(idConjunto))
            {
                try
                {
                    botonGenerar.Enabled = false;
                    //reporte = new RentRoll(comboInmobiliaria.SelectedValue.ToString(), (comboInmobiliaria.SelectedItem as DataRowView)["P0103_RAZON_SOCIAL"].ToString(), comboConjunto.SelectedValue.ToString(), (comboConjunto.SelectedItem as DataRowView)["P0303_NOMBRE"].ToString(), dateTimePicker1.Value, dateTimePicker2.Value, radioPDF.Checked, rutaFormato);
                    reporte = new RentRoll(iDinmobiliaria, NombreInmo, idConjunto, NombreConjunto, dateTimePicker1.Value, dateTimePicker2.Value, radioPDF.Checked, rutaFormato);
                    reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
                    workerReporte.RunWorkerAsync();
                }
                catch
                {
                }
            }
            else
            {
                MessageBox.Show("¡No se puede generar el reporte si no existe un conjunto!", "Rent roll", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                botonGenerar.Enabled = true;
                botonCancelar.Enabled = true;
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
                MessageBox.Show("¡Reporte generado correctamente!", "Rent roll", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo generar el reporte: " + Environment.NewLine + result, "Rent roll", MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;

        }

        private void comboConjunto_SelectedIndexChanged(object sender, EventArgs e)
        {
            idConjunto = comboConjunto.SelectedValue.ToString();
            NombreConjunto = comboConjunto.Text.ToString();
        }

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                reporte.cancelar();
            botonCancelar.Enabled = false;
        }

        private void Frm_RentRoll_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }
    }
}
