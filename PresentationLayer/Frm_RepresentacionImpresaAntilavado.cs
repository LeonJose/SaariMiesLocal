using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesAntiLavado;
namespace GestorReportes.PresentationLayer
{
    public partial class Frm_RepresentacionImpresaAntilavado : Form
    {        
        private string rutaFormato = string.Empty, fileName = string.Empty, result = string.Empty;
        private RepresentacionImpresaXMLAntilavado reporte = null;
        public Frm_RepresentacionImpresaAntilavado(string rutaFormato)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaFormato = rutaFormato;
        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {            
            if (opnFlDlgAntilav.ShowDialog() == DialogResult.OK)
            {
                fileName = opnFlDlgAntilav.FileName;
                txtBxRutaArchivo.Text = fileName;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                reporte.cancelar();
            botonCancelar.Enabled = false;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(rutaFormato))
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    bool esPdf = rdBtnPdf.Checked;
                    bool esV2 = false;
                    XMLAntilavado xmlBL = new XMLAntilavado();
                    try
                    {
                        string file = System.IO.Path.GetFileName(fileName);
                        string fecha = file.Split('-')[0].Trim();
                        DateTime fechaGeneración = DateTime.ParseExact(fecha, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                        if (fechaGeneración >= new DateTime(2014, 9, 18)) //Fecha en la cual se desarrollo la funcionalidad para los cambios de esquema
                            esV2 = true;
                    }
                    catch 
                    { 
                        MessageBox.Show("El archivo debe contener la fecha de generación en su nombre", "Representación impresa de XML antilavado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!esV2)
                    {
                        Informe informe = xmlBL.leerXml(fileName);
                        if (informe != null)
                        {
                            RepresentacionImpresaXMLAntilavado representacion = new RepresentacionImpresaXMLAntilavado();
                            //string result = representacion.generarRepresentacion(informe, Application.StartupPath + @"\Resources\", esPdf);
                            string result = representacion.generarRepresentacion(informe, rutaFormato, esPdf);
                            if (string.IsNullOrEmpty(result))
                            {
                                MessageBox.Show("¡Se generó la representación impresa correctamente!", "Representación impresa de XML antilavado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show(result, "Representación impresa de XML antilavado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se pudo leer el archivo XML para generar la representación impresa. Verifique que se trate de un archivo de antilavado.", "Representación impresa de XML antilavado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        GestorReportes.BusinessLayer.EntitiesAntilavado2.Informe informe = xmlBL.leerXml(fileName, esV2);
                        progreso.Value = 0;
                        if (informe != null)
                        {
                            botonGenerar.Enabled = false;
                            reporte = new RepresentacionImpresaXMLAntilavado(informe, rutaFormato, esPdf);
                            reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
                            workerReporte.RunWorkerAsync();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo leer el archivo XML para generar la representación impresa. Verifique que se trate de un archivo de antilavado.", "Representación impresa de XML antilavado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No se encontró el formato.", "Representación impresa de XML antilavado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void reporte_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
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
                MessageBox.Show("¡Se generó la representación impresa correctamente!", "Representación impresa de XML antilavado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(result, "Representación impresa de XML antilavado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void Frm_RepresentacionImpresaAntilavado_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }
    }
}
