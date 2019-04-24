using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.Helpers;
using System.Drawing.Printing;
using System.IO;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_RecordatorioCobranza : Form
    {
        string rutaFormato = string.Empty, usuario = string.Empty, result = string.Empty;
        RecordatorioCobranza reporte = null;
        PrinterSettings configImpresora = null;
        /// <summary>
        /// Tipo de contrato R=Renta - V=Renta
        /// </summary>
        private bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;
        string tipoContrato = string.Empty;
        string tipoFacturas = string.Empty;
        bool esRenta = false;
        string mensajebody = "Se adjunta el reporte de Cobranza de Facturas de Renta Pendientes de Pago a la fecha. \nEl reporte se genera automáticamente, cualquier duda o aclaración favor de contactarnos";
        public Frm_RecordatorioCobranza(string tipo, string rutaForm, string user)
        {
            InitializeComponent();
            SaariIcon.SaariIcon.setSaariIcon(this);            
            rutaFormato = rutaForm;
            usuario = user;
            
            if (tipo != "venta")
            {
                esRenta = true;
                this.Text += " de Renta";
                tipoContrato = "R";
                tipoFacturas = "R";
            }
            else
            {
                esRenta = false;
                this.Text += " de Venta";
                tipoContrato = "V";
                tipoFacturas = "V";
            }
        }

        private void Frm_CobranzaRenta_Load(object sender, EventArgs e)
        {
            gridContratos.Visible = false;

            rtbBodymnj.Text = mensajebody;
            rtbBodymnj.Enabled = false;
            //dateInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateFin.Value = DateTime.Today;
            radioMonedaLocal.Text = RecordatorioCobranza.obtenerMonedaLocal();
            asignarDatosAComboInmo();
            comboInmobiliaria.SelectedValue = "TODOS";
            radioButtonCliente_CheckedChanged(sender, e);
            cargarGrid("TODOS");
            textoBuscar.Focus();            
        }
        private void cargarGrid(string Inmo)
        {
                        
            //gridClientes.DataSource = CobranzaRentaVencida.obtenerClientesPorInmobiliaria(Inmo);
            if (radioButtonCliente.Checked)
            {
                gridClientes.AutoGenerateColumns = false;                                          
                
                //var listaClientes = (from clienteItem in listaC group clienteItem by key)
                List<ClienteEntity> listClients = RecordatorioCobranza.obtenerClientesConFacturaPorInmobiliaria(Inmo);
                gridClientes.DataSource = listClients.OrderBy(c=>c.Nombre).ToList();

                foreach (DataGridViewRow row in gridClientes.Rows)
                {
                    row.Cells["seleccionColumn"].Value = false;
                }
            }
            else
            {
                gridContratos.AutoGenerateColumns = false;
                gridContratos.DataSource = RecordatorioCobranza.obtenerContratos(Inmo, tipoContrato);
                foreach (DataGridViewRow row in gridContratos.Rows)
                {
                    row.Cells["columnSeleccion"].Value = false;
                }
            }
            
        }
        //private void checkEmpresa_CheckedChanged(object sender, EventArgs e)
        void asignarDatosAComboInmo()
        {
            
                comboInmobiliaria.Enabled = true;
                //comboEmpresa.DataSource = CobranzaRentaVencida.obtenerInmobiliarias();
                List<InmobiliariaEntity> inmos = HelpInmobiliarias.obtenerInmobiliarias(usuario, permisosInmo); //RecordatorioCobranza.obtenerInmobiliarias();
                InmobiliariaEntity inmo=new InmobiliariaEntity();                
                inmo.ID="TODOS";
                inmo.NombreComercial="TODOS";
                inmo.RazonSocial="TODOS";
                inmos.Add(inmo);
                //inmo = new InmobiliariaEntity();
                //inmo.ID = "SIN CONTRATO";
                //inmo.NombreComercial = "SIN CONTRATO";
                //inmo.RazonSocial = "SIN CONTRATO";
                //inmos.Add(inmo);
                comboInmobiliaria.DataSource = inmos;
 
                comboInmobiliaria.ValueMember = "ID";
                comboInmobiliaria.DisplayMember = "RazonSocial";
           
                //comboInmobiliaria.DataSource = null;
                //comboInmobiliaria.Enabled = false;
           
        }

        private void comboEmpresa_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //List<ClienteEntity> listaCtes = RecordatorioCobranza.obtenerClientes();
            if (radioButtonContrato.Checked)
            {
                gridContratos.DataSource = null;
                string idInmo = comboInmobiliaria.SelectedValue.ToString();
                cargarGrid(idInmo);
            }
            else
            {
                gridClientes.DataSource = null;
                string idInmo = comboInmobiliaria.SelectedValue.ToString();
                cargarGrid(idInmo);
            }

        }
        private void botonGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                progreso.Value = 0;

                mensajebody = rtbBodymnj.Text;
                if (radioButtonContrato.Checked)
                {
                    #region POR CONTRATO
                    List<string> listaIDsContratos = new List<string>();
                    string idContrato = string.Empty;
                    foreach (DataGridViewRow row in gridContratos.Rows)
                    {
                        if ((bool)row.Cells["columnSeleccion"].Value)
                        {
                            idContrato = row.Cells["contratoColumn"].Value.ToString();
                            listaIDsContratos.Add(idContrato);
                        }
                    }
                    if (listaIDsContratos.Count > 0)
                    {
                        List<ContratosEntity> listaContratos = gridContratos.DataSource as List<ContratosEntity>;
                        List<ContratosEntity> listaContratosSeleccionados = new List<ContratosEntity>();
                        foreach (string s in listaIDsContratos)
                        {
                            ContratosEntity contrato = listaContratos.FirstOrDefault(x => x.IDContrato == s);
                            if (contrato != null)
                                listaContratosSeleccionados.Add(contrato);
                        }
                        if (listaContratosSeleccionados != null)
                        {
                            if (listaContratosSeleccionados.Count > 0)
                            {
                                botonGenerar.Enabled = false;
                                reporte = new RecordatorioCobranza(radioPdf.Checked, dateFin.Value.Date, listaContratosSeleccionados, (comboInmobiliaria.SelectedItem as InmobiliariaEntity).RazonSocial, rutaFormato, usuario, checkEnviarCorreo.Checked, checkEnviarImpresora.Checked, configImpresora, radioDolar.Checked, radioButtonCliente.Checked, mensajebody);
                                reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
                                workerReporte.RunWorkerAsync();
                            }
                            else
                                MessageBox.Show("No se encontraron contratos seleccionados.", "Recordatorio de Pago", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                            MessageBox.Show("Error al obtener los datos de los contratos seleccionados.", "Recordatorio de Pago", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                        MessageBox.Show("No se pudieron obtener los contratos seleccionados, o no se seleccionó ningún contrato.", "Recordatorio de Pago", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    #endregion                
                }
                else if (radioButtonCliente.Checked)
                {
                    #region POR CLIENTE
                    progreso.Value = 0;
                    string idArrendadora = string.Empty;
                    if (comboInmobiliaria.SelectedValue.ToString()!="TODOS")
                        idArrendadora = comboInmobiliaria.SelectedValue.ToString();
                    List<string> listaIDsClientes = new List<string>();
                    string idCliente = string.Empty;
                    foreach (DataGridViewRow row in gridClientes.Rows)
                    {
                        if ((bool)row.Cells["seleccionColumn"].Value)
                        {
                            idCliente = row.Cells["IDClienteColumn"].Value.ToString();
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
                            cliente.TipoFactura = tipoFacturas;
                            if (cliente != null)
                                listaClientesSeleccionados.Add(cliente);
                        }
                        if (listaClientesSeleccionados != null)
                        {
                            if (listaClientesSeleccionados.Count > 0)
                            {
                                botonGenerar.Enabled = false;
                                reporte = new RecordatorioCobranza(radioPdf.Checked, dateFin.Value.Date, listaClientesSeleccionados, (comboInmobiliaria.SelectedItem as InmobiliariaEntity).RazonSocial, rutaFormato, usuario, checkEnviarCorreo.Checked, checkEnviarImpresora.Checked, configImpresora, radioDolar.Checked, radioButtonCliente.Checked,mensajebody);
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
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general de generación:" + Environment.NewLine + ex.Message, "Recordatorio de Pago", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                reporte.cancelar();
            botonCancelar.Enabled = false;
        }
        private void gridContratos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((bool)gridContratos.Rows[e.RowIndex].Cells[e.ColumnIndex].Value)
                    gridContratos.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                else
                    gridContratos.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = true;
            }
            catch
            { }
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

        void reporte_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        private void workerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void workerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
            result = reporte.generar();    
        }

        private void workerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(result))
            {
                if (checkEnviarCorreo.Checked)
                    MessageBox.Show("¡Aviso(s) enviados(s) por correo electrónico correctamente!", "Recordatorio de Pago", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("¡Reporte(s) generado(s) correctamente!", "Recordatorio de Pago", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                RecordatorioCobranza.generaLog(result, tipoFacturas);
                MessageBox.Show("Se ha creado un archivo con el detalle de errores al generar el Recordatorio de Pagos. \nPresione Aceptar para continuar.", "Recordatorio de Pago", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
            progreso.Value = 0;
        }

        private void Frm_CobranzaRenta_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

        private void checkTodosClientes_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCliente.Checked)
            {
                foreach (DataGridViewRow row in gridClientes.Rows)
                    row.Cells["seleccionColumn"].Value = checkTodosClientes.Checked;
            }
            else
            {
                foreach (DataGridViewRow row in gridContratos.Rows)
                    row.Cells["columnSeleccion"].Value = checkTodosClientes.Checked;
            }
        }        

        private void botonBuscar_ControlClicked(object sender, EventArgs e)
        {
            cargarGrid(comboInmobiliaria.SelectedValue.ToString());
            if (radioButtonContrato.Checked)
            {
                #region CONTRATOS
                List<ContratosEntity> listaContratos = gridContratos.DataSource as List<ContratosEntity>;
                if (listaContratos != null)
                {
                    gridContratos.AutoGenerateColumns = false;
                    if (string.IsNullOrEmpty(textoBuscar.Text))
                    {
                        if (radioVigente.Checked)
                        {
                            listaContratos = (from l in listaContratos
                                              where l.Vigente
                                              select l).ToList();
                            gridContratos.DataSource = listaContratos;
                        }
                    }
                    else
                    {
                        if (radioVigente.Checked)
                        {
                            listaContratos = (from l in listaContratos
                                              where l.Vigente &&
                                              (l.Cliente.ToLower().Contains(textoBuscar.Text.ToLower()) || l.Inmueble.ToLower().Contains(textoBuscar.Text.ToLower()))
                                              select l).ToList();
                            gridContratos.DataSource = listaContratos;
                        }
                        else
                        {
                            listaContratos = (from l in listaContratos
                                              where l.Cliente.ToLower().Contains(textoBuscar.Text.ToLower()) || l.Inmueble.ToLower().Contains(textoBuscar.Text.ToLower())
                                              select l).ToList();
                            gridContratos.DataSource = listaContratos;
                        }
                    }
                    foreach (DataGridViewRow row in gridContratos.Rows)
                    {
                        row.Cells["columnSeleccion"].Value = false;
                    }
                }
                #endregion
            }
            else
            {
                List<ClienteEntity> listaClientes = gridClientes.DataSource as List<ClienteEntity>;
                if (listaClientes != null)
                {
                    gridContratos.AutoGenerateColumns = false;
                    if (!string.IsNullOrEmpty(textoBuscar.Text))
                    {
                        listaClientes = (from l in listaClientes where (l.Nombre.ToLower().Contains(textoBuscar.Text.ToLower())) select l).ToList();
                        gridClientes.DataSource = listaClientes;
                    }
                    foreach (DataGridViewRow row in gridClientes.Rows)
                    {
                        row.Cells["seleccionColumn"].Value = false;
                    }
                }
            }
        }

        private void radioButtonContrato_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxContratos.Visible = radioButtonContrato.Checked;
            gridContratos.Visible = radioButtonContrato.Checked;            
            gridContratos.Width = 670;
            gridContratos.Dock = DockStyle.Fill;
            if(radioButtonContrato.Checked)
                cargarGrid(comboInmobiliaria.SelectedValue.ToString());
         
        }

        private void radioButtonCliente_CheckedChanged(object sender, EventArgs e)
        {
            gridClientes.Visible = radioButtonCliente.Checked;
            gridClientes.Width = 572;            
            gridClientes.Left = 48;
            //gridClientes.Dock = DockStyle.None;            
            //if (comboInmobiliaria.SelectedValue != null)
            //    cargarGrid(comboInmobiliaria.SelectedValue.ToString());
            //else
            //    cargarGrid("TODOS");
            if(radioButtonCliente.Checked)
                cargarGrid(comboInmobiliaria.SelectedValue.ToString());
        }       

        private void rtbBodymnj_Click(object sender, EventArgs e)
        {
            rtbBodymnj.Enabled = true;

        }

        private void rtbBodymnj_DoubleClick(object sender, EventArgs e)
        {
            rtbBodymnj.Enabled = true;
        }

        private void tableLayoutPanel1_Click(object sender, EventArgs e)
        {
            rtbBodymnj.Enabled = true;
        }

        private void panel6_Click(object sender, EventArgs e)
        {
            rtbBodymnj.Enabled = true;
        }
        

    }
}
