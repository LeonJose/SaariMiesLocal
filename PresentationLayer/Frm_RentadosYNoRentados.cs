using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_RentadosYNoRentados : Form
    {
        private Listado listado = null;
        private Inmobiliaria inmobiliaria = new Inmobiliaria();
        private string usuario = string.Empty, error = string.Empty;
        public Frm_RentadosYNoRentados(string user)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            //listado.Usuario = user;
            this.usuario = user;
        }

        private void Frm_RentadosYNoRentados_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

        private void Frm_RentadosYNoRentados_Load(object sender, EventArgs e)
        {
            cmbBxEstatus.SelectedIndex = 0;
            txtBxTipoCambio.Text = Listado.getTipoDeCambio(dateTimePicker1.Value);
            DataTable dtGpoEmp = inmobiliaria.getGposCommand();
            if (dtGpoEmp.Rows.Count > 0)
            {
                DataRow row = dtGpoEmp.NewRow();
                row["P0001_ID_GRUPO"] = "Todos";
                row["P0002_NOMBRE"] = "*Todos";
                dtGpoEmp.Rows.InsertAt(row, 0);
                DataView view = new DataView(dtGpoEmp);
                view.Sort = "P0002_NOMBRE";
                cmbBxGpoEmp.DataSource = view;
                cmbBxGpoEmp.DisplayMember = "P0002_NOMBRE";
                cmbBxGpoEmp.ValueMember = "P0001_ID_GRUPO";
                cmbBxGpoEmp.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No se encontró ningun grupo empresarial registrado. \n - Debe tener grupos empresariales registrados. \n - Revise su conexión a la base de datos.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            txtBxTipoCambio.Text = Listado.getTipoDeCambio(dateTimePicker1.Value);
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            if (tipoCambioValido())
            {
                progreso.Value = 0;
                botonGenerar.Enabled = false;
                listado = new Listado();
                listado.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(listado_CambioProgreso);
                listado.Usuario = this.usuario;
                listado.Estatus = cmbBxEstatus.SelectedItem.ToString();
                listado.TipoDeCambio = Convert.ToDecimal(txtBxTipoCambio.Text);
                listado.GrupoEmpresarial = cmbBxGpoEmp.SelectedValue.ToString().Trim();
                listado.Vigencia = dateTimePicker1.Value;
                workerReporte.RunWorkerAsync();
                
            }
            else
            {
                MessageBox.Show("El valor contenido en el tipo de cambio es incorrecto!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtBxTipoCambio.Text = Listado.getTipoDeCambio(dateTimePicker1.Value);
            }
        }

        void listado_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        private bool tipoCambioValido()
        {
            if (String.IsNullOrEmpty(txtBxTipoCambio.Text))
            {
                return false;
            }
            else
            {
                try
                {
                    decimal tc = Convert.ToDecimal(txtBxTipoCambio.Text);
                    if (tc > 0)
                        return true;
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                listado.cancelar();
            botonCancelar.Enabled = false;
        }

        private void workerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
            error = listado.generar();            
        }

        private void workerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void workerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(error))
                MessageBox.Show("¡Reporte generado correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Hubo un error al generar el reporte. \n" + error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;            
        }
    }
}
