using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_EstatusCobranza : Form
    {

        private string rutaFormato;
        private string user;

        InmobiliariaEntity Inmob = null;
        ConjuntoEntity Conjunt;
        EstadoCobranzaEntity EstadoCob;
        EstatusCobranza cobranzas = null;

        string error;

        public Frm_EstatusCobranza()
        {
            InitializeComponent();
        }
        public Frm_EstatusCobranza(string formato, string usuario)
        {
            InitializeComponent();
            this.rutaFormato = formato;
            this.user = usuario;
            EstadoCob = new EstadoCobranzaEntity();
            Conjunt = new ConjuntoEntity();
            Inmob = new InmobiliariaEntity();
            
        }
        private void CargarInmobiliarias()
        {
            try
            {
                cbxInmobiliaria.DataSource = SaariDB.getListaInmobiliarias();
                cbxInmobiliaria.DisplayMember = "RazonSocial";
                cbxInmobiliaria.ValueMember = "ID";
                cbxInmobiliaria.SelectedIndex = 0;
            }
            catch (Exception ex)
            {

            }
        }
        private void CargarConjuntos()
        {
            try
            {
                cbxConjunto.DataSource = SaariDB.getListaConjuntos(Inmob.ID);
                cbxConjunto.DisplayMember = "Nombre";
                cbxConjunto.ValueMember = "ID";
                cbxConjunto.SelectedIndex = 0;
            }
            catch (Exception ex)
            {

            }
        }
        private void Frm_EstatusCobranza_Load(object sender, EventArgs e)
        {
            DateTime fecha = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //fechaFin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 30);
            dtinicio.Value =fecha;          
            CargarInmobiliarias();
            desactivadocbxConjunto();
        }

        private void rbtnTodosConjuntos_CheckedChanged(object sender, EventArgs e)
        {
            desactivadocbxConjunto();
          
        }
        private void rbtnUnConjunto_CheckedChanged(object sender, EventArgs e)
        {
            cargarcbxConjunto();
            if(rbtnUnConjunto.Checked )
                CargarConjuntos();
        }

        private void rbtnConjuntosxSubconjuntos_CheckedChanged(object sender, EventArgs e)
        {
            desactivadocbxConjunto();
        }
        private void cargarcbxConjunto()
        {
            lblConjunto.Visible = true;
            cbxConjunto.Visible = true;
        }
        private void desactivadocbxConjunto()
        {
            lblConjunto.Visible = false;
            cbxConjunto.Visible = false;
        }

        private void Generar_Click(object sender, EventArgs e)
        {
            
            progressBar1.Value = 10;
            DateTime fechaInicio = dtinicio.Value.Date;
            DateTime fechaFin = dtfin.Value.Date.AddHours(12);
            //Clasificacion Conjuntos
            int opcion = 0;
            if (rbtnTodosConjuntos.Checked)
                opcion = 1;
            else if (rbtnUnConjunto.Checked)
                opcion = 2;
            else
                opcion = 3;

            //Opcion orden.
            int opcionOrden = 0;

            if (rbtnFolioCfd.Checked)
                opcionOrden = 1;
            else
                opcionOrden = 2;
            
            Conjunt= cbxConjunto.SelectedItem as ConjuntoEntity; 
            cobranzas = new EstatusCobranza(Inmob, Conjunt,  user, rutaFormato, fechaInicio, fechaFin, opcion, opcionOrden, radioPDF.Checked, Worker);
            cobranzas.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(EstatusCobranz);

            Worker.RunWorkerAsync();
        }
        void EstatusCobranz(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            Worker.ReportProgress(e.Progreso);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            error = cobranzas.generarReporte();
        }
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 100;
            if (string.IsNullOrWhiteSpace(error))
            {
                MessageBox.Show("¡Reporte generado correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No se pudo generar el reporte  :" + error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Generar.Enabled = true;
            btnCancelar.Enabled = true;
        }
        private void cbxInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            Inmob = cbxInmobiliaria.SelectedItem as InmobiliariaEntity;
            //validamos si es seleccionado el rbtnConjunto
            if (rbtnUnConjunto.Checked == true)
                CargarConjuntos();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if(Generar.Enabled)
            {
                this.Close();
            }
            else
            {
                cobranzas.cancelar();
            }
            btnCancelar.Enabled = false;
        }
    }
}
