using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_AuditoriaCobranza : Form
    {
        private string rutaFormato = string.Empty, usuario = string.Empty, idArrendadora = string.Empty, result = string.Empty;
        private AuditoriaCobranza reporte = null;
        private List<ClienteEntity> listaClientesSeleccionados = new List<ClienteEntity>();

        public Frm_AuditoriaCobranza(string rutaFormato, string usuario)
        {
            InitializeComponent();
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
        }

        private void botonCancelar_Click(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                reporte.cancelar();                
            botonCancelar.Enabled = false;
        }

        private void Frm_AuditoriaCobranza_Load(object sender, EventArgs e)
        {
            cargarGrid();
        }

        private void cargarGrid()
        {
            gridClientes.AutoGenerateColumns = false;
            gridClientes.DataSource = AuditoriaCobranza.obtenerClientes();
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

        private void checkEmpresa_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEmpresa.Checked)
            {
                comboEmpresa.Enabled = true;
                comboEmpresa.DataSource = AuditoriaCobranza.obtenerInmobiliarias();
                comboEmpresa.ValueMember = "ID";
                comboEmpresa.DisplayMember = "RazonSocial";
            }
            else
            {
                comboEmpresa.DataSource = null;
                comboEmpresa.Enabled = false;
            }
        }

        private void botonGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                progreso.Value = 0;
                idArrendadora = string.Empty;
                if (checkEmpresa.Checked)
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
                    listaClientesSeleccionados = new List<ClienteEntity>();
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
                            reporte = new AuditoriaCobranza(listaClientesSeleccionados, idArrendadora, dateFin.Value.Date, radioPdf.Checked, rutaFormato, usuario);
                            reporte.CambioProgreso += new EventHandler<CambioProgresoEventArgs>(reporte_CambioProgreso);                            
                            workerReporte.RunWorkerAsync();                            
                        }
                        else
                            MessageBox.Show("No se encontraron clientes seleccionados", "Auditoria de cobranza", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                        MessageBox.Show("Error al obtener los datos de los clientes seleccionados", "Auditoria de cobranza", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show("No se pudieron obtener los clientes seleccionados", "Auditoria de cobranza", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general de generación:" + Environment.NewLine + ex.Message, "Auditoria de cobranza", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void reporte_CambioProgreso(object sender, CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        private void botonBuscar_ControlClicked(object sender, EventArgs e)
        {
            cargarGrid();
            List<ClienteEntity> listaClientes = gridClientes.DataSource as List<ClienteEntity>;
            if (listaClientes != null)
            {
                gridClientes.AutoGenerateColumns = false;
                if (string.IsNullOrEmpty(textoBuscar.Text))
                {
                    listaClientes = AuditoriaCobranza.obtenerClientes();
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

        private void Frm_AuditoriaCobranza_FormClosing(object sender, FormClosingEventArgs e)
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
                MessageBox.Show("¡Reporte(s) generado(s) correctamente!", "Auditoria de cobranza", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo generar el reporte: " + Environment.NewLine + result, "Auditoria de cobranza", MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }
    }
}
