using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FastReport;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.PresentationLayer;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_ConsecutivosCFDIsFacturacion : Form
    {
        private bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;
        private string IdInmobiliaria = string.Empty; DateTime fechaInicio = DateTime.Now;
        private string NombreInmo = string.Empty; DateTime fechaFin = DateTime.Now;
        private string idConjunto = string.Empty;
        private string NombreConjunto = string.Empty;
        private string usuario = string.Empty;
        private string errores = string.Empty;
        private bool abrir = true;
        ConsecutivosCFDIsFact consecutivosCFDIs = null;

        public Frm_ConsecutivosCFDIsFacturacion(string usuario)
        {
            InitializeComponent();
            SaariIcon.SaariIcon.setSaariIcon(this);
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
                IdInmobiliaria = comboInmobiliaria.SelectedValue.ToString();
                NombreInmo = comboInmobiliaria.Text.ToString();
            }
            catch
            { }

        }

        public void cargarConjuntos(string selectedValue)
        {
            try
            {
                IdInmobiliaria = comboInmobiliaria.SelectedValue.ToString();
                List<ConjuntoEntity> ListCont = new List<ConjuntoEntity>();
                ConjuntoEntity conjuntoTodos = new ConjuntoEntity();
                conjuntoTodos.ID = "Todos";
                conjuntoTodos.Nombre = "*Todos";
                ListCont = HelpInmobiliarias.getConjuntos(selectedValue);
                ListCont.Insert(0, conjuntoTodos);
                comboConjunto.DataSource = ListCont;// HelpInmobiliarias.getConjuntos(selectedValue);
                comboConjunto.DisplayMember = "Nombre";
                comboConjunto.ValueMember = "ID";
                comboConjunto.SelectedIndex = 0;
                idConjunto = comboConjunto.SelectedValue.ToString();
                NombreConjunto = comboConjunto.Text.ToString();
            }
            catch
            { }
        }

        private void Frm_ConsecutivosCFDIsFacturacion_Load(object sender, EventArgs e)
        {
            cargarInmobiliarias();
            cargarConjuntos(IdInmobiliaria);
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int days = 1;
            dateTimePicker1.Value = new DateTime(year,month,days);
            days =DateTime.DaysInMonth(year, month);
            dateTimePicker2.Value = new DateTime(year, month,days);
            
        }

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                consecutivosCFDIs.cancelar();
            botonCancelar.Enabled = false;

        }

        private void WorkerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
            errores = consecutivosCFDIs.generarReporte();
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            progreso.Value = 0;
            botonGenerar.Enabled = false;
            IdInmobiliaria = comboInmobiliaria.SelectedValue.ToString();
            NombreInmo = comboInmobiliaria.Text.ToString();
            idConjunto = comboConjunto.SelectedValue.ToString();
            NombreConjunto = comboConjunto.Text.ToString();
            abrir = checkOpen.Checked;
            consecutivosCFDIs = new ConsecutivosCFDIsFact(IdInmobiliaria, idConjunto, fechaInicio = dateTimePicker1.Value, fechaFin = dateTimePicker2.Value, usuario, radioPDF.Checked,abrir,NombreInmo,NombreConjunto);
            consecutivosCFDIs.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
            WorkerReporte.RunWorkerAsync();
        }
        void reporte_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            WorkerReporte.ReportProgress(e.Progreso);
        }


        private void WorkerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void WorkerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(errores))
            {
                progreso.Value = 100;
                MessageBox.Show("¡Reporte generado correctamente!", "CFDi's consecutivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("No se pudo generar el reporte: " + Environment.NewLine + errores, "CFDi's consecutivos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
            progreso.Value=(0);
        }

        private void comboInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {           
            IdInmobiliaria = comboInmobiliaria.SelectedValue.ToString();
            NombreInmo = comboInmobiliaria.Text.ToString();
            cargarConjuntos(IdInmobiliaria);
        }

        private void botonGenerar_Load(object sender, EventArgs e)
        {

        }
    }
}
