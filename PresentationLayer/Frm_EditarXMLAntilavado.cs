using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.PresentationLayer.Controls;
using GestorReportes.BusinessLayer.EntitiesAntiLavado;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_EditarXMLAntilavado : Form
    {        
        public Frm_EditarXMLAntilavado()
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
        }

        private void Frm_EditarXMLAntilavado_Load(object sender, EventArgs e)
        {
            try
            {
                opnFlDlgAntilav.InitialDirectory = GestorReportes.Properties.Settings.Default.RutaAntilavado;
            }
            catch { }

        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            string fileName = string.Empty;
            if (opnFlDlgAntilav.ShowDialog() == DialogResult.OK)
            {
                fileName = opnFlDlgAntilav.FileName;
                txtBxRutaArchivo.Text = fileName;
                while (flowPanelAlertasPorPersonas.Controls.Count > 0)
                {
                    flowPanelAlertasPorPersonas.Controls.Remove(flowPanelAlertasPorPersonas.Controls[0]);
                }
            }
            if (!string.IsNullOrEmpty(fileName.Trim()))
            {
                try
                {
                    XMLAntilavado xmlAnti = new XMLAntilavado(string.Empty);
                    bool esVenta = xmlAnti.esArchivoVenta(fileName);
                    string error = xmlAnti.llenaAlertas(fileName);
                    if (string.IsNullOrEmpty(error))
                    {
                        foreach (AlertaPorPersona alerta in xmlAnti.alertasPorPersonas)
                        {
                            Ctrl_AlertaPorPersona ctrl = new Ctrl_AlertaPorPersona(alerta.TipoAlerta, esVenta);
                            ctrl.lblRFC.Text = alerta.RFCCliente;
                            flowPanelAlertasPorPersonas.Controls.Add(ctrl);
                        }
                    }
                    else
                    {
                        MessageBox.Show(error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch
                {
                    MessageBox.Show("Probablemente el archivo seleccionado no corresponda a la operación de tipo", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (flowPanelAlertasPorPersonas.Controls.Count > 0)
            {
                List<AlertaPorPersona> listaAlertas = new List<AlertaPorPersona>();
                foreach (Control control in flowPanelAlertasPorPersonas.Controls)
                {
                    AlertaPorPersona alerta = new AlertaPorPersona();
                    alerta.TipoAlerta = Convert.ToInt32((control as Ctrl_AlertaPorPersona).cmbBxTiposAlerta.SelectedValue.ToString());
                    alerta.RFCCliente = (control as Ctrl_AlertaPorPersona).lblRFC.Text.Trim();
                    listaAlertas.Add(alerta);
                }
                XMLAntilavado xmlAnti = new XMLAntilavado(string.Empty);
                string error = xmlAnti.modificarAlertas(txtBxRutaArchivo.Text.Trim(), listaAlertas);
                if (string.IsNullOrEmpty(error))
                {
                    MessageBox.Show("¡Cambios guardados correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                    MessageBox.Show(error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }   
    }
}
