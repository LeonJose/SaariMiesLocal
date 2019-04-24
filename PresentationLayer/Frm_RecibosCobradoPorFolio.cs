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
    public partial class Frm_RecibosCobradoPorFolio : Form
    {
        RecibosCobradosPorFolio objRecCobradoFolio = new RecibosCobradosPorFolio();
        string iDinmobiliaria = string.Empty;
        string NombreInmo = string.Empty;
        string idConjunto = string.Empty;
        DateTime fechainicial;
        DateTime fechafinal;
        string error = string.Empty;
        bool cancelUser = false;
        bool esPDF = true;
        string rutaformato = string.Empty;
        bool AbrirEx = true;
        string Usuario = string.Empty;
        bool permisosInmo = GestorReportes.Properties.Settings.Default.PermisosInmobiliaria;

        public Frm_RecibosCobradoPorFolio(string rutaformato, string usuario)
        {
            InitializeComponent();
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaformato = rutaformato;
            this.Usuario = usuario;
            

        }

        public void cargarInmobiliarias (){
            try
            {
                cmbInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(Usuario, permisosInmo);
                cmbInmobiliaria.DisplayMember = "RazonSocial";
                cmbInmobiliaria.ValueMember = "ID";
                cmbInmobiliaria.SelectedIndex = 0;
                iDinmobiliaria = cmbInmobiliaria.SelectedValue.ToString();
                NombreInmo = cmbInmobiliaria.Text.ToString();
            }
            catch
            {
                 error = "Error al obtener las inmobiliarias.";
            }
        }

        public void cargarConjuntos(string selectedValue) {
            cmbConjunto.DataSource = RecibosCobradosPorFolio.ObtenerConjuntos(iDinmobiliaria);
            cmbConjunto.DisplayMember = "Nombre";
            cmbConjunto.ValueMember = "ID";
            cmbConjunto.SelectedIndex = 0;
            idConjunto = cmbConjunto.SelectedValue.ToString();}


        private void Frm_RecibosCobradoPorFolio_Load(object sender, EventArgs e)
        {
            cargarInmobiliarias();
            DateTime fecha = DateTime.Now;
            int dia = 1;      
            fechainicial = new DateTime(fecha.Year, fecha.Month, dia);
            dia = DateTime.DaysInMonth(fecha.Year,fecha.Month);
            fechafinal = new DateTime(fecha.Year,fecha.Month,dia);
           dtpFechaInicial.Value= fechainicial;
            dtpFechaFinal.Value = fechafinal;
        }

        private void rBSelectConjunto_CheckedChanged(object sender, EventArgs e)
        {
            if (!rBSelectConjunto.Checked)
            {
                conjunto.Visible = false;
                cmbConjunto.Visible = false;
            }
            else {
                conjunto.Visible = true;
                cmbConjunto.Visible = true;
                cargarConjuntos(cmbInmobiliaria.SelectedValue.ToString().Trim());
            }
        }

        private void cmbInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            iDinmobiliaria = cmbInmobiliaria.SelectedValue.ToString().Trim();
            NombreInmo = cmbInmobiliaria.Text.ToString();
            cargarConjuntos(iDinmobiliaria);
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            if (rBSelectConjunto.Checked)
            {
                idConjunto = cmbConjunto.SelectedValue.ToString();

            }
            else 
            {
                idConjunto = null;
            }

            botonGenerar.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                bool allConjuntos = rBallConjuntos.Checked;
                bool selectConjunto = rBSelectConjunto.Checked;
                bool allOrder = rBallOrder.Checked;
                // rb_allConjuntos.Checked ? conjunto = rb_allConjuntos.Text : cmbBox_Conjunto.SelectedValue.ToString();
                bool orderby = rBOrderFolio.Checked ? orderby = true : orderby = false;
                bool incluyeDetalle = chkIncluirCargosPeriodicos.Checked;
                esPDF = rbEsPDF.Checked;
                AbrirEx = chkAbrir.Checked;
                fechainicial = dtpFechaInicial.Value;
                fechafinal = dtpFechaFinal.Value;
                error = objRecCobradoFolio.generarReporte(iDinmobiliaria, NombreInmo, idConjunto, fechainicial.Date, fechafinal.Date, allConjuntos, selectConjunto, allOrder, orderby, incluyeDetalle, esPDF, AbrirEx, rutaformato, backgroundWorker1);
            }
            catch { }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (string.IsNullOrEmpty(error))
            {
                MessageBox.Show("¡Listado generado correctamente!", "Saari Rentas Variables", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                if (cancelUser)
                {
                    MessageBox.Show("Error al generar el listado de importes:" + Environment.NewLine + error, "Saari Rentas Variables", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error al generar el listado de importes:" + Environment.NewLine + error, "Saari Rentas Variables", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            progressBar1.Value = 0;
            botonGenerar.Enabled = true;
        }

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (!botonGenerar.Enabled)
            {
                backgroundWorker1.CancelAsync();
                cancelUser = true;
            }
            else
            {
                this.Close();
            }   
        }

        private void botonGenerar_Load(object sender, EventArgs e)
        {

        }






    }
}
