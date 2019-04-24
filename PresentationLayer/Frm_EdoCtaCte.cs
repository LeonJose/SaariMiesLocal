using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_EdoCtaCte : Form
    {
        string rutaFormato = string.Empty, usuario = string.Empty, result = string.Empty;
        EstadoDeCuentaCliente reporte = null;
        PrinterSettings configImpresora = null;
        private bool PermisosInmobiliaria = Properties.Settings.Default.PermisosInmobiliaria;
        public Frm_EdoCtaCte(string rutaForm, string user)
        {
            InitializeComponent();
            SaariIcon.SaariIcon.setSaariIcon(this);
            //System.Drawing.Icon icono = new System.Drawing.Icon("IconoSAARI.ico");
            //this.Icon = icono;  
            rutaFormato = rutaForm;
            usuario = user;
        }

        private void botonCancelar_Click(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                reporte.cancelar();
            botonCancelar.Enabled = false;
        }

        private void Frm_EdoCtaCte_Load(object sender, EventArgs e)
        {
            dateInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            radioMonedaLocal.Text = EstadoDeCuentaCliente.obtenerMonedaLocal();
            cargarGrid();
        }

        private void cargarGrid()
        {
            gridClientes.AutoGenerateColumns = false;
            gridClientes.DataSource = EstadoDeCuentaCliente.obtenerClientes();
            foreach (DataGridViewRow row in gridClientes.Rows)
            {
                row.Cells["seleccionColumn"].Value = false;
            }
        }

        private void gridClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((bool)gridClientes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value)
                    gridClientes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                else
                    gridClientes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = true;
            }
            catch
            { }
        }

        private void botonBuscar_Click(object sender, EventArgs e)
        {
            cargarGrid();
            List<ClienteEntity> listaClientes = gridClientes.DataSource as List<ClienteEntity>;
            if (listaClientes != null)
            {
                gridClientes.AutoGenerateColumns = false;
                if (string.IsNullOrEmpty(textoBuscar.Text))
                {
                    listaClientes = EstadoDeCuentaCliente.obtenerClientes();
                    gridClientes.DataSource = listaClientes;
                }
                else
                {
                    listaClientes = (from l in listaClientes
                                        where l.Nombre.ToLower().Contains(textoBuscar.Text.ToLower()) || l.RFC.ToLower().Contains(textoBuscar.Text.ToLower())
                                        select l).ToList();
                    gridClientes.DataSource = listaClientes;
                                        
                }
                foreach (DataGridViewRow row in gridClientes.Rows)
                {
                    row.Cells["seleccionColumn"].Value = false;
                }
            }
        }

        private void checkEnviarImpresora_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEnviarImpresora.Checked)
            {
                if (DialogResult.OK == printDialog.ShowDialog())
                {
                    configImpresora = printDialog.PrinterSettings;
                }
                else
                    checkEnviarImpresora.Checked = false;
            }
            else
            {
                configImpresora = null;
            }
        }

        private void botonGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                progreso.Value = 0;
                string idArrendadora = string.Empty;
                if(checkEmpresa.Checked)
                    idArrendadora = comboEmpresa.SelectedValue.ToString();
                List<string> listaIDsClientes = new List<string>();
                string idCliente = string.Empty;
                foreach (DataGridViewRow row in gridClientes.Rows)
                {
                    if ((bool)row.Cells["seleccionColumn"].Value)
                    {
                        idCliente = row.Cells["idCteColumn"].Value.ToString();
                        listaIDsClientes.Add(idCliente);
                    }
                }
                if (listaIDsClientes.Count > 0)
                {
                    List<ClienteEntity> listaClientes = gridClientes.DataSource as List<ClienteEntity>;
                    List<ClienteEntity> listaClientesSeleccionados = new List<ClienteEntity>();
                    foreach (string s in listaIDsClientes)
                    {
                        ClienteEntity cliente = listaClientes.FirstOrDefault(x => x.IDCliente == s);
                        if (cliente != null)
                            listaClientesSeleccionados.Add(cliente);
                    }
                    if (listaClientesSeleccionados != null)
                    {
                        if (listaClientesSeleccionados.Count > 0)
                        {
                            botonGenerar.Enabled = false;
                            reporte = new EstadoDeCuentaCliente(radioPdf.Checked, dateInicio.Value.Date, dateFin.Value.Date, listaClientesSeleccionados, rutaFormato, usuario, checkEnviarCorreo.Checked, checkEnviarImpresora.Checked, configImpresora, radioDolar.Checked, idArrendadora);
                            reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
                            workerReporte.RunWorkerAsync();                            
                        }
                        else
                            MessageBox.Show("No se encontraron clientes seleccionados", "Estado de cuenta de cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                        MessageBox.Show("Error al obtener los datos de los clientes seleccionados", "Estado de cuenta de cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show("No se pudieron obtener los clientes seleccionados", "Estado de cuenta de cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general de generación:" + Environment.NewLine + ex.Message, "Estado de cuenta de cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void reporte_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        private void checkEmpresa_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEmpresa.Checked)
            {
                comboEmpresa.Enabled = true;
                comboEmpresa.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuario, PermisosInmobiliaria); //EstadoDeCuentaCliente.obtenerInmobiliarias();
                comboEmpresa.ValueMember = "ID";
                comboEmpresa.DisplayMember = "RazonSocial";
            }
            else
            {
                comboEmpresa.DataSource = null;
                comboEmpresa.Enabled = false;
            }
        }

        private void Frm_EdoCtaCte_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
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
                MessageBox.Show("¡Reporte(s) generado(s) correctamente!", "Estado de cuenta de cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo generar el reporte: " + Environment.NewLine + result, "Estado de cuenta de cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void checkTodosClientes_CheckedChanged(object sender, EventArgs e)
        {
            if (checkTodosClientes.Checked)
            {
                foreach (DataGridViewRow row in gridClientes.Rows)
                {
                    row.Cells["seleccionColumn"].Value = true;
                }
            }
            else
            {
                foreach (DataGridViewRow row in gridClientes.Rows)
                {
                    row.Cells["seleccionColumn"].Value = false;
                }
            }
        }
    }
}
