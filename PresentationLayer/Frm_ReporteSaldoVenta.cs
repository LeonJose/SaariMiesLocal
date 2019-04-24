using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_ReporteSaldoVenta : Form
    {
        private Inmobiliaria inmobiliaria = new Inmobiliaria();
        private string result = string.Empty;
        private SaldoVenta saldo = null;
        private string usuarios = string.Empty;
        private bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;
        public Frm_ReporteSaldoVenta(string usuario)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            this.usuarios = usuario;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(result))
                MessageBox.Show("¡Reporte generado correctamente!", "Reporte de saldos de venta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo generar el reporte: " + Environment.NewLine + result, "Reporte de saldos de venta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;

        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            result = saldo.generar();            
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        public void cargarInmobiliarias()
        {
            try
            {
                comboInmo.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuarios, permisosInmo);
                comboInmo.DisplayMember = "RazonSocial";
                comboInmo.ValueMember = "ID";
                comboInmo.SelectedIndex = 0;
               // iDinmobiliaria = comboInmobiliaria.SelectedValue.ToString();
                //NombreInmo = comboInmobiliaria.Text.ToString();
            }
            catch
            {
                MessageBox.Show("Ha ocurrido un error: " + Environment.NewLine + "No se encontraron inmobiliaras en la base de datos", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(0);
            }
        }
        private void Frm_ReporteSaldoVenta_Load(object sender, EventArgs e)
        {
        //   // DataTable dtInmobiliaria = inmobiliaria.getDataTable("SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL FROM T01_ARRENDADORA ");
        //    if (dtInmobiliaria.Rows.Count <= 0)
        //    {
        //        MessageBox.Show("Ha ocurrido un error: " + Environment.NewLine + "No se encontraron inmobiliaras en la base de datos", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        System.Environment.Exit(0);
        //    }
            cargarInmobiliarias();
            //DataView dvInmobiliaria = new DataView(dtInmobiliaria);
            //dvInmobiliaria.Sort = "P0103_RAZON_SOCIAL";
            //comboInmo.DataSource = dvInmobiliaria;
            //comboInmo.DisplayMember = "P0103_RAZON_SOCIAL";
            //comboInmo.ValueMember = "P0101_ID_ARR";
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            botonGenerar.Enabled = false;
            saldo = new SaldoVenta(comboInmo.SelectedValue.ToString(), dateTimePicker1.Value, checkIncluirPreventa.Checked, checkAbrir.Checked);
            saldo.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(saldo_CambioProgreso);
            worker.RunWorkerAsync();
        }

        void saldo_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
 	        worker.ReportProgress(e.Progreso);
        }

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                saldo.cancelar();
            botonCancelar.Enabled = false;
        }

        private void Frm_ReporteSaldoVenta_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

        private void botonGenerar_Load(object sender, EventArgs e)
        {

        }
    }
}
