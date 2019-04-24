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

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_Cartera306090                                                                                                                                                                                                                                                                              : Form
    {
        private string rutaFormato = string.Empty;
        private string usuario = string.Empty;
        private string error = string.Empty;
        private string idInmobiliaria = string.Empty;
        private string idConjunto = string.Empty;
        private bool esPDF = true;
        private bool incluirIVA = true;
        private bool esDetallado = false;
        private bool tomarFechaEmisionFactura = true;
        private bool esPorContrato = false;
        private bool esPorRubro = false;
        private bool permisosInmobiliaria = false;
        private decimal tipoCambio = 1;
        private DateTime fechaCorte = DateTime.Now.Date;
        CarteraVencida306090 reporte = null;

        public Frm_Cartera306090(string rutaFormato, string usuario)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            this.permisosInmobiliaria = GestorReportes.Properties.Settings.Default.PermisosInmobiliaria;
        }

        public Frm_Cartera306090(string rutaFormato, string usuario, bool esPorContrato, string texto)
            : this(rutaFormato, usuario)
        {
            this.esPorContrato = esPorContrato;
            this.Text = texto;
        }
        public Frm_Cartera306090(string rutaFormato, string usuario, string texto)
            : this(rutaFormato, usuario)
        {
            this.esPorRubro = true;
            this.Text = texto;
        }

        private void Frm_Cartera306090_Load(object sender, EventArgs e)
        {
            comboInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuario,permisosInmobiliaria);//CarteraVencida306090.obtenerInmobiliarias();
            comboInmobiliaria.ValueMember = "ID";
            comboInmobiliaria.DisplayMember = "RazonSocial";

            comboConjunto.DataSource = CarteraVencida306090.obtenerConjuntos(comboInmobiliaria.SelectedValue.ToString());
            comboConjunto.ValueMember = "ID";
            comboConjunto.DisplayMember = "Nombre";
        }

        private void comboInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboConjunto.DataSource = CarteraVencida306090.obtenerConjuntos(comboInmobiliaria.SelectedValue.ToString());
            comboConjunto.ValueMember = "ID";
            comboConjunto.DisplayMember = "Nombre";
        }

        private void workerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
            error = reporte.generar();
            /*if(!esPorContrato)
                error = reporte.generarReporte(idInmobiliaria, idConjunto, fechaCorte, esPDF, incluirIVA, esDetallado, workerReporte, rutaFormato, usuario, tomarFechaEmisionFactura, tipoCambio);
            else
                error = reporte.generarReporte(idInmobiliaria, idConjunto, fechaCorte, esPDF, incluirIVA, esDetallado, workerReporte, rutaFormato, usuario, tomarFechaEmisionFactura, tipoCambio, true);*/
        }

        private void workerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void workerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(string.IsNullOrEmpty(error))
                MessageBox.Show("¡Reporte generado correctamente!", "Cartera vencida 30, 60, 90 y +90", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Error al generar el reporte: \n" + error, "Cartera vencida 30, 60, 90 y +90", MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }   

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                reporte.cancelar();                
            botonCancelar.Enabled = false;
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            try
            {
                progreso.Value = 0;
                error = string.Empty;
                idInmobiliaria = comboInmobiliaria.SelectedValue.ToString();
                idConjunto = comboConjunto.SelectedValue.ToString();
                esPDF = radioPDF.Checked;
                fechaCorte = dateTimePicker1.Value.Date;
                incluirIVA = radioIva.Checked;
                esDetallado = checkDetallado.Checked;
                tomarFechaEmisionFactura = radioTCEmision.Checked;
                if (!tomarFechaEmisionFactura)
                {
                    CarteraVencida306090 consulta = new CarteraVencida306090();
                    tipoCambio = consulta.obtenerTipoCambio(fechaCorte);
                    //tipoCambio = reporte.obtenerTipoCambio(fechaCorte);
                    if (tipoCambio == 0)
                    {
                        Frm_TipoCambio tipoCambioForm = new Frm_TipoCambio();
                        tipoCambioForm.ShowDialog();
                        tipoCambio = tipoCambioForm.TipoCambio;
                    }
                }
                botonGenerar.Enabled = false;
                if(!esPorRubro)
                    reporte = new CarteraVencida306090(idInmobiliaria, idConjunto, fechaCorte, esPDF, incluirIVA, esDetallado, rutaFormato, usuario, tomarFechaEmisionFactura, tipoCambio, esPorContrato); 
                else 
                    reporte = new CarteraVencida306090(idInmobiliaria, idConjunto, fechaCorte, esPDF, incluirIVA, esDetallado, rutaFormato, usuario, tomarFechaEmisionFactura, tipoCambio, esPorContrato, esPorRubro);

                reporte.CambioProgreso += new EventHandler<CambioProgresoEventArgs>(reporte_CambioProgreso);
                workerReporte.RunWorkerAsync();
            }
            catch { }
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

    }
}
