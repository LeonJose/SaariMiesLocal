using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_CarteraVencidaVta : Form
    {
        private Inmobiliaria inmobiliaria = new Inmobiliaria();
        private string formato = string.Empty, usuario = string.Empty, error = string.Empty;
        private CarteraVencidaVenta reporte = null;
        private bool permisosInmobiliaria = Properties.Settings.Default.PermisosInmobiliaria;
        public Frm_CarteraVencidaVta(string formato, string usuario)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);  
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.formato = formato;
            this.usuario = usuario;
        }

        private void Frm_CarteraVencidaVta_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

        private void Frm_CarteraVencidaVta_Load(object sender, EventArgs e)
        {
            var inmobiliarias = HelpInmobiliarias.obtenerInmobiliarias(usuario,permisosInmobiliaria);// CarteraVencidaVenta.obtenerInmobiliarias();
            if (inmobiliarias != null)
                inmobiliarias = inmobiliarias.OrderBy(i => i.RazonSocial).ToList();
            cbBxEmp.DataSource = inmobiliarias;
            cbBxEmp.DisplayMember = "RazonSocial";
            cbBxEmp.ValueMember = "ID";
                        
            cbBxEst.DataSource = CarteraVencidaVenta.obtenerEstatus();
            cbBxEst.DisplayMember = "Estatus";
            cbBxEst.ValueMember = "Estatus";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
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
            if (string.IsNullOrWhiteSpace(error))
                MessageBox.Show("¡Reporte generado correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo generar el reporte de cartera vencida de venta: " + Environment.NewLine + error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void cbBxEmp_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                cbBxConj.DataSource = CarteraVencidaVenta.obtenerConjuntos(cbBxEmp.SelectedValue.ToString());
                cbBxConj.DisplayMember = "Nombre";
                cbBxConj.ValueMember = "ID";
            }
            catch { }
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            reporte = new CarteraVencidaVenta();
            var inmobiliaria = cbBxEmp.SelectedItem as InmobiliariaEntity;
            if (inmobiliaria != null)
            {
                progreso.Value = 0;
                reporte.Arrendadora = inmobiliaria.ID;
                reporte.Conjunto = cbBxConj.SelectedValue.ToString();
                reporte.Estatus = cbBxEst.SelectedValue.ToString();
                reporte.Nombre_Inmobiliaria = inmobiliaria.RazonSocial;
                reporte.Formato = formato;
                reporte.Usuario = usuario;
                reporte.Excel = radioExcel.Checked;
                botonGenerar.Enabled = false;
                reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
                workerReporte.RunWorkerAsync();                
            }
        }

        void reporte_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }
    }
}
