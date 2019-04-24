using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
//using GestorReportes.BusinessLayer.EntitiesAntiLavado;
using GestorReportes.BusinessLayer.EntitiesAntilavado2;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_GenerarXMLAntilavado : Form
    {
        bool antilavadoVenta = false;
        private InformeAntilavado2 informeAnti = null;
        private DateTime dateSeleccionada = DateTime.Now.Date;
        private string rfcContribuyente = string.Empty, error = string.Empty;
        private string user = string.Empty;
        private bool permisosInmo = false;
        private string result = string.Empty;

        public Frm_GenerarXMLAntilavado(bool venta)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            if (venta)
            {
                this.Text += " para venta de inmuebles";
                antilavadoVenta = true;
            }

            #region leer configuracion
            try
            {
                permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;
            }
            catch
            {
                permisosInmo = false;
            }
            #endregion

        }

        public void cargarInmobiliarias()
        {
            try
            {
                cmbBxInm.DataSource = HelpInmobiliarias.obtenerInmobiliariasContibuyente(user, permisosInmo);
                cmbBxInm.DisplayMember = "RazonSocial";
                cmbBxInm.ValueMember = "RFC";
                cmbBxInm.SelectedIndex = 0;
                //iDinmobiliaria = cmbBxInm.SelectedValue.ToString();
                //NombreInmo = cmbBxInm.Text.ToString();
            }
            catch
            {
                Inmobiliaria inmo = new Inmobiliaria();
                DataTable dtContribuyentes = inmo.getDtContribuyentes();
                if (dtContribuyentes.Rows.Count > 0)
                {
                    DataView dvContribuyentes = new DataView(dtContribuyentes);
                    dvContribuyentes.Sort = "P4202_RAZON_SOCIAL";
                    cmbBxInm.DataSource = dvContribuyentes;
                    cmbBxInm.DisplayMember = "P4202_RAZON_SOCIAL";
                    cmbBxInm.ValueMember = "P4203_RFC";
                }
                else
                {
                    MessageBox.Show("Ha ocurrido un problema al cargar los contribuyentes. Vuelva a intentarlo", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }

        private void Frm_GenerarXMLAntilavado_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonAceptar.Enabled)
                e.Cancel = true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (botonAceptar.Enabled)
                this.Close();
            else
                informeAnti.cancelar();
            botonCancelar.Enabled = false;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            dateSeleccionada = getDateSeleccion(cmbBxFechas.SelectedItem.ToString());
            rfcContribuyente = cmbBxInm.SelectedValue.ToString().Trim();
            //string rfcGenerico = "XAXX010101000";
            progreso.Value = 0;
            botonAceptar.Enabled = false;
            informeAnti = new InformeAntilavado2(rfcContribuyente, dateSeleccionada, chckBxOctubre.Checked);
            //    informeAnti = new InformeAntilavado2(rfcContribuyente, dateSeleccionada, chckBxOctubre.Checked);
            
            informeAnti.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(informeAnti_CambioProgreso);
            worker.RunWorkerAsync();
        }

        private void cmbBxInm_SelectedValueChanged(object sender, EventArgs e)
        {
            //if(!string.IsNullOrWhiteSpace(cmbBxInm.SelectedValue.ToString()))
            //    checkBoxExcluirCteGen.Visible = true;
            //else
            //    checkBoxExcluirCteGen.Visible = false;
        }

        private void checkBoxExcluirCteGen_CheckedChanged(object sender, EventArgs e)
        {
            //labelCteGen.Visible = checkBoxExcluirCteGen.Checked;
            //comboBoxClienteGen.Visible = checkBoxExcluirCteGen.Checked;
            //string rfcSeleccionado = string.Empty;
            //if (cmbBxInm.SelectedValue != null && checkBoxExcluirCteGen.Checked)
            //{
            //    rfcSeleccionado= cmbBxInm.SelectedValue.ToString().Trim();                
            //    Inmobiliaria inmo = new Inmobiliaria();
            //    DataTable dtClientes = inmo.getClientesByRfcArrendora(rfcSeleccionado);
            //    if (dtClientes != null)
            //    {
            //        if (dtClientes.Rows.Count > 0)
            //        {
            //            DataView dvClientes = new DataView(dtClientes);                       
            //            dvClientes.Sort = "P0203_NOMBRE";
            //            comboBoxClienteGen.DataSource = dvClientes;
            //            comboBoxClienteGen.DisplayMember = "P0203_NOMBRE";
            //            comboBoxClienteGen.ValueMember = "P0204_RFC";
            //        }
            //    }
            //}
        }

        void informeAnti_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            worker.ReportProgress(e.Progreso);
        }

        private void Frm_GenerarXMLAntilavado_Load(object sender, EventArgs e)
        {
            cmbBxFechas.DataSource = getMesesList();

            cargarInmobiliarias(); 
             //DataTable dtContribuyentes = inmo.getDtContribuyentes();
             // if (dtContribuyentes.Rows.Count > 0)
             // {
             //     DataView dvContribuyentes = new DataView(dtContribuyentes);
             //     dvContribuyentes.Sort = "P4202_RAZON_SOCIAL";
             //     cmbBxInm.DataSource = dvContribuyentes;
             //     cmbBxInm.DisplayMember = "P4202_RAZON_SOCIAL";
             //     cmbBxInm.ValueMember = "P4203_RFC";
             // }
             // else
             // {
             //     MessageBox.Show("Ha ocurrido un problema al cargar los contribuyentes. Vuelva a intentarlo", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
             //     this.Close();
             // }
           
        }

        private List<string> getMesesList()        
        {
            List<string> listaMeses = new List<string>();
            DateTime mesControl = new DateTime(2013, 9, 1);
            DateTime mesActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            while(mesControl <= mesActual)
            {
                listaMeses.Add(mesControl.ToString("MMMM yyyy"));
                mesControl = mesControl.AddMonths(1);
            }
            //listaMeses.Add("febrero 2013"); //para pruebas
            return listaMeses;
        }

        private DateTime getDateSeleccion(string dateString)
        {
            try
            {
                string date = dateString + " 01";
                DateTime dateVuelta = DateTime.ParseExact(date, "MMMM yyyy dd", System.Globalization.CultureInfo.CurrentCulture);
                return dateVuelta;
            }
            catch
            {
                return DateTime.Now;
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Informe informeEntity = informeAnti.obtenerInforme(antilavadoVenta);
            if (!string.IsNullOrEmpty(informeAnti.Errores))
            {
                LogAntilavado.generaLog(informeAnti.Errores, string.Empty, rfcContribuyente);
                if (informeAnti.Errores.Contains("No se encontraron registros que tengan que presentar avisos en el mes solicitado"))
                {
                    if (DialogResult.Yes == MessageBox.Show(informeAnti.Errores + Environment.NewLine + "¿Desea generar el informe en blanco?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        XMLAntilavado antiBlanco = new XMLAntilavado(rfcContribuyente);
                        Informe info = new Informe();
                        info.MesReportado = dateSeleccionada;
                        SujetoObligado sujeto = new SujetoObligado(antilavadoVenta);
                        sujeto.ClaveRFC = rfcContribuyente.Trim();
                        info.SujetoObligado = sujeto;
                        error = antiBlanco.generar(Application.StartupPath, info, true, antilavadoVenta);
                        if (string.IsNullOrEmpty(error))
                            MessageBox.Show("Se generó el archivo XML correctamente.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("Ha ocurrido un problema no esperado al generar el archivo en blanco: " + Environment.NewLine + error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                    MessageBox.Show(informeAnti.Errores, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (informeEntity == null)
                {
                    MessageBox.Show("Ha ocurrido un problema no esperado al cargar los datos", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    informeEntity = informeAnti.validaEstructura(informeEntity);
                    if (!string.IsNullOrEmpty(informeAnti.Errores))
                    {
                        if (!string.IsNullOrEmpty(informeAnti.Warnings))
                        {
                            LogAntilavado.generaLog(informeAnti.Errores, informeAnti.Warnings, rfcContribuyente);
                            MessageBox.Show("Existe uno o más errores y más de una advertencia. Consulte el log para mas detalle", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            LogAntilavado.generaLog(informeAnti.Errores, string.Empty, rfcContribuyente);
                            MessageBox.Show("Existe uno o más errores. Consulte el log para mas detalle", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        XMLAntilavado xmlAnti = new XMLAntilavado(rfcContribuyente);
                        if (!string.IsNullOrEmpty(informeAnti.Warnings))
                        {
                            LogAntilavado.generaLog(string.Empty, informeAnti.Warnings, rfcContribuyente);
                            MessageBox.Show("Existe una o más advertencias. Se continuará con la generación debido a que las advertencias son para campos opcionales. Consulte el log para mas detalle", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            string error = xmlAnti.generar(Application.StartupPath, informeEntity, false, antilavadoVenta);
                            if (string.IsNullOrEmpty(error))
                            {
                                MessageBox.Show("Se generó el archivo XML correctamente.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                int resultAumentRef = 0;
                                resultAumentRef = GestorReportes.BusinessLayer.DataAccessLayer.Informes.setUltimaReferenciaPorRFC(rfcContribuyente, informeAnti.NumReferencia - 1, antilavadoVenta);
                                if (resultAumentRef <= 0)
                                    MessageBox.Show("No se pudo incrementar la referencia. " + Environment.NewLine + error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show("No se pudo generar el archivo. " + Environment.NewLine + error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            string error = xmlAnti.generar(Application.StartupPath, informeEntity, false, antilavadoVenta);
                            if (string.IsNullOrEmpty(error))
                            {
                                MessageBox.Show("Se generó el archivo XML correctamente.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No se pudo generar el archivo. " + Environment.NewLine + error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            botonAceptar.Enabled = true;
            botonCancelar.Enabled = true;
        }


    }
}
