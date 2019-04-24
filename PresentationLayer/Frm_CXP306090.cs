using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.PresentationLayer.Controls;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_CXP306090 : Form
    {
        CXPConfiguracionReporte configReporte = new CXPConfiguracionReporte();
        CuentasXPagar306090 reporteCuentasCXP = new CuentasXPagar306090();
        private bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;
        bool TCValido = false;
        private string error = string.Empty;

        public Frm_CXP306090()
        {
            InitializeComponent();
            SaariIcon.SaariIcon.setSaariIcon(this);
        }
        public Frm_CXP306090(string rutaFormato, string usuario):this()
        {
            configReporte.RutaFormato = rutaFormato;
            configReporte.Usuario = usuario;
        }
        private void Frm_CXP306090_Load(object sender, EventArgs e)
        {
            comboInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(configReporte.Usuario, permisosInmo); //reporteCuentasCXP.obtenerInmobiliarias();
            comboInmobiliaria.ValueMember = "ID";
            comboInmobiliaria.DisplayMember = "RazonSocial";
        }
        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            if (!TCValido)
            {
                CancelEventArgs ev = new CancelEventArgs();
                textBoxTipoCambio_Validating(sender, ev);
            }
            else
            {
                this.leerParametrosDeConfiguracion();
                botonGenerar.Enabled = false;
                
                reporteCuentasCXP.asignarConfiguracion(configReporte);
                reporteCuentasCXP.CambioProgreso += new EventHandler<CambioProgresoEventArgs>(reporte_CambioProgreso);
                workerReporte.RunWorkerAsync();
                
                //Frm_PruebaConsultas PruebasForm = new Frm_PruebaConsultas(reporteCuentasCXP.ListaDeProvisiones);
                //PruebasForm.ShowDialog();
            }
        }
        private void leerParametrosDeConfiguracion()
        {
            configReporte.IDInmobiliaria = comboInmobiliaria.SelectedValue.ToString();
            configReporte.FechaCorte = dateFechaCorte.Value;
            configReporte.TipoCambio = Convert.ToDecimal(textBoxTipoCambio.Text);
            configReporte.EsDetallado = checkDetallado.Checked;
            configReporte.IncluirIVA = radioIva.Checked;
            configReporte.IncluirCancelados = false;
            configReporte.EsPdf = radioPDF.Checked;
            configReporte.EsPorConjunto = false;
            configReporte.Clasificacion = string.Empty;
            configReporte.Moneda = "P";
            configReporte.IDConjunto = string.Empty;
            InmobiliariaEntity inmo = reporteCuentasCXP.obtenerInmobiliariaPorID(configReporte.IDInmobiliaria);            
            if (inmo != null)
            {
                configReporte.NombreComercialInmobiliaria = inmo.NombreComercial;
                configReporte.RazonSocialInmobiliaria = inmo.RazonSocial;
            }
            else
            {
                configReporte.NombreComercialInmobiliaria = string.Empty;
                configReporte.RazonSocialInmobiliaria = string.Empty;
            }
            
        }

        

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                reporteCuentasCXP.cancelar();
            botonCancelar.Enabled = false;
        }

        private void dateFechaCorte_ValueChanged(object sender, EventArgs e)
        {
            decimal tc = reporteCuentasCXP.obtenerTipoCambio(dateFechaCorte.Value);
            textBoxTipoCambio.Text = tc.ToString("N4");
            CancelEventArgs ev= new CancelEventArgs();
            textBoxTipoCambio_Validating(sender,ev);
        }

        private void textBoxTipoCambio_Validating(object sender, CancelEventArgs e)
        {
            TCValido = false;
            errorProviderTC.Clear();
            if (string.IsNullOrEmpty(textBoxTipoCambio.Text))
            {
                errorProviderTC.SetError(textBoxTipoCambio, "El tipo de cambio no se ha capturado.");
            }
            else
            {
                try
                {
                    decimal tc = Convert.ToDecimal(textBoxTipoCambio.Text);
                    if (tc == 0)
                    {
                        errorProviderTC.SetError(textBoxTipoCambio, "No existe tipo de cambio asignado para la fecha de corte indicada.");
                    }
                    else
                    {
                        TCValido = true;
                        errorProviderTC.Clear();
                    }
                }
                catch
                {
                    errorProviderTC.SetError(textBoxTipoCambio, "El tipo de cambio debe ser un dato numérico.");
                }
            }
        }

        private void workerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
            error = reporteCuentasCXP.generar();
        }

        private void workerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void workerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(error))
                MessageBox.Show("¡Reporte generado correctamente!", "Cartera vencida 30, 60, 90 y +90", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Error al generar el reporte: \n" + error, "Cartera vencida 30, 60, 90 y +90", MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }
        void reporte_CambioProgreso(object sender, CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }
        private void Frm_Cartera306090_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

    }//End Class
}//End Namespace
