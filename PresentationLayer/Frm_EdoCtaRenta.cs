using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.ComponentLayer;
using System.Drawing.Printing;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_EdoCtaRenta : Form
    {
        string rutaFormato = string.Empty, usuario = string.Empty, result = string.Empty;
        bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;
        EstadoDeCuentaRenta reporte = null;
        PrinterSettings configImpresora = null;
        public Frm_EdoCtaRenta(string rutaForm, string user)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            rutaFormato = rutaForm;
            usuario = user;
        }

        private void Frm_EdoCtaRenta_Load(object sender, EventArgs e)
        {
            dateInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            comboInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuario,permisosInmo);// EstadoDeCuentaRenta.obtenerInmobiliarias();
            comboInmobiliaria.ValueMember = "ID";
            comboInmobiliaria.DisplayMember = "RazonSocial";

            cargarGrid();
        }

        private void cargarGrid()
        {
            gridContratos.AutoGenerateColumns = false;
            gridContratos.DataSource = EstadoDeCuentaRenta.obtenerContratos(comboInmobiliaria.SelectedValue.ToString());
            foreach (DataGridViewRow row in gridContratos.Rows)
            {
                row.Cells["columnSeleccion"].Value = false;
            }
        }

        private void comboInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarGrid();
            //radioTodos.Checked = true;
            //textoBuscar.Text = string.Empty;
        }

        private void gridContratos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            /*foreach (DataGridViewRow row in gridContratos.Rows)
            {
                row.Cells["columnSeleccion"].Value = false;
            }*/
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

        private void botonBuscar_Click(object sender, EventArgs e)
        {
            buscarContrato();
        }

        private void botonCancelar_Click(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                reporte.cancelar();
            botonCancelar.Enabled = false;
        }

        private void botonGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                progreso.Value = 0;
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
                            reporte = new EstadoDeCuentaRenta(radioPdf.Checked, dateInicio.Value.Date, dateFin.Value.Date, listaContratosSeleccionados, (comboInmobiliaria.SelectedItem as InmobiliariaEntity).RazonSocial, rutaFormato, usuario, checkEnviarCorreo.Checked, checkEnviarImpresora.Checked, configImpresora);
                            reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
                            workerReporte.RunWorkerAsync();                            
                        }
                        else
                            MessageBox.Show("No se encontraron contratos seleccionados", "Estado de cuenta de renta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                        MessageBox.Show("Error al obtener los datos de los contratos seleccionados", "Estado de cuenta de renta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show("No se pudieron obtener los contratos seleccionados", "Estado de cuenta de renta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general de generación:" + Environment.NewLine + ex.Message, "Estado de cuenta de renta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void reporte_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
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
                MessageBox.Show("¡Reporte(s) generado(s) correctamente!", "Estado de cuenta de renta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo generar el reporte: " + Environment.NewLine + result, "Estado de cuenta de renta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void Frm_EdoCtaRenta_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

        private void buscarContrato()
        {

            cargarGrid();
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
        }

        private void textoBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            buscarContrato();
        }


    }
}
