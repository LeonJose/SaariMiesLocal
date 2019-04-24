using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;

namespace GestorReportes
{
    public partial class Frm_Principal : Form
    {
        //Objeto de tipo reporte
        Reporte reporte = null;
                    
        //Objeto de tipo inmobiliria
        Inmobiliaria inmobiliaria = new Inmobiliaria();

        //Variables globales
        bool incluirConjunto = false;
        bool incluirSubconjunto = false;
        bool incluirFechaCorte = false;
        bool incluirRadio1 = false;
        bool incluirRadio2 = false;
        bool incluirRadio3 = false;
        bool incluirRadio4 = false;
        private string clave = string.Empty, rutaFormato = string.Empty, nombreReporte = string.Empty, error = string.Empty;
        
        /// <summary>
        /// Realiza la carga de controles de la forma.
        /// <param name="parametros">Parámetros recibidos desde Saari. [0]:Clave de reporte, [1]:Ruta .fr3 de reporte</param>
        /// </summary>
        public Frm_Principal(string[] parametros)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.clave = parametros[0];
            this.rutaFormato = parametros[1];
            //reporte.clave = parametros[0];
            //reporte.rutaPlantilla = parametros[1];

            if (!System.IO.File.Exists(rutaFormato))
            {
                MessageBox.Show("No existe el formato de plantilla en ubicación: \"" + rutaFormato + "\"", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            if (!rutaFormato.Contains(".fr3"))
            {
                MessageBox.Show("Versión de plantilla no soportada. Este gestor sólo soporta plantillas en formato \".fr3\".", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            //Existe un archivo .xls llamado controlReportes, el cual contiene la lista de los reportes existentes, 
            //dependiendo de la definición de cada reporte se hace la carga de los controles de la forma.
            switch (this.clave)
            {                
                case "REP1" :
                    nombreReporte = "Cartera vencida a 30,60,90 y + 90 días";
                    this.Text = nombreReporte;
                    
                    incluirConjunto = true; incluirSubconjunto = false; incluirFechaCorte = true;
                    incluirRadio1 = true;incluirRadio2=true;incluirRadio3 = true;incluirRadio4=true;
                    radioButton1.Checked = true;
                    radioButton3.Checked = true;

                    cargarControles();
                    break;
                case "REP2" :
                    nombreReporte = "Histórico de ingresos por unidad de negocio";
                    this.Text = nombreReporte;
                    incluirConjunto = false; incluirSubconjunto = false; incluirFechaCorte = true;
                    incluirRadio1 = false;incluirRadio2=false;incluirRadio3 = false;incluirRadio4=false;
                    cargarControles();
                    break;
                case "REP3" :
                    nombreReporte = "Histórico de ingresos consolidado";
                    this.Text = nombreReporte;
                    incluirConjunto = false; incluirSubconjunto = false; incluirFechaCorte = true;
                    incluirRadio1 = false;incluirRadio2=false;incluirRadio3 = false;incluirRadio4=false;
                    cargarControles();
                    break;
                case "REP4":
                    nombreReporte = "Saldo de Clientes";
                    this.Text = nombreReporte;
                    incluirConjunto = true; incluirSubconjunto = false; incluirFechaCorte = true;
                    incluirRadio1 = false;incluirRadio2= false;incluirRadio3 = false;incluirRadio4=false;
                    cargarControles();
                    break;
                default     :
                    this.Close();
                    break;
            }

            cargarDatosInmobiliaria();
            cbxInmobiliaria_SelectedIndexChanged(null, null);

            cbxConjunto_SelectedIndexChanged(null, null);    
        
            datePickerIniCorte.Value = DateTime.Now;
            if(!incluirFechaCorte) datePickerFinal.Value = DateTime.Now.AddMonths(1);
        }

        /// <summary>
        /// Carga de datos para inmobiliaria.
        /// </summary>
        private void cargarDatosInmobiliaria()
        {
            DataTable dtInmobiliarias = inmobiliaria.getDtInmobiliarias();
            if (dtInmobiliarias.Rows.Count > 0)
            {
                DataView view = new DataView(dtInmobiliarias);
                view.Sort = "P0102_N_COMERCIAL";
                cbxInmobiliaria.DataSource = view;
                cbxInmobiliaria.DisplayMember = "P0102_N_COMERCIAL";
                cbxInmobiliaria.ValueMember = "P0101_ID_ARR";
                cbxInmobiliaria.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No se encontró ninguna inmobiliaria registrada. \n - Debe tener inmobiliarias registradas. \n - Revise su conexión a la base de datos.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Oculta/edita controles de la forma principal.
        /// </summary>
        private void cargarControles()
        {            
            if (!incluirConjunto)
            {
                lblConjunto.Visible = false; 
                cbxConjunto.Visible = false;
            }
            if (!incluirSubconjunto)
            {
                lblSubConjunto.Visible = false; 
                cbxSubConjunto.Visible = false;
                
            }
            if (incluirFechaCorte)
            {
                lblFechaIniCorte.Text = "Fecha de corte";
                lblFechaFinal.Visible = false;
                datePickerFinal.Visible = false;
            }
            if (!incluirRadio1)
                radioButton1.Visible = false;
            if (!incluirRadio2)
                radioButton2.Visible = false;
            if (!incluirRadio3)
                radioButton3.Visible = false;
            if (!incluirRadio4)
                radioButton4.Visible = false;
        }

        /// <summary>Enviar reporte a imprimir
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// </summary>
        private void btnReporteImprimir_Click(object sender, EventArgs e)
        {
            progreso.Value = 0;
            botonGenerarPDF.Enabled = false;
            botonGenerarExcel.Enabled = false;
            TimeSpan ts = new TimeSpan(0, 0, 0);
            datePickerFinal.Value = datePickerFinal.Value.Date + ts;
            datePickerIniCorte.Value = datePickerIniCorte.Value.Date + ts;
            DateTime fechaFin = new DateTime();
            string idConjunto = string.Empty;
            string idSubConjunto = string.Empty;
            bool repIva = true;

            if (datePickerFinal.Visible)
                fechaFin = datePickerFinal.Value.Date;
            if (cbxConjunto.Visible && cbxConjunto.Enabled) 
                idConjunto = cbxConjunto.SelectedValue.ToString().Trim();
            if (cbxSubConjunto.Visible)
                idSubConjunto = cbxSubConjunto.SelectedValue.ToString().Trim();
            repIva = radioButton3.Checked;           
            reporte = new Reporte(this.clave, this.rutaFormato, this.nombreReporte, cbxInmobiliaria.SelectedValue.ToString(), idConjunto, idSubConjunto, repIva, datePickerIniCorte.Value.Date, fechaFin, true);
            reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
            workerReporte.RunWorkerAsync();
            string rutaSalidaReporte = reporte.generar();
           
        }

        void reporte_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        /// <summary>Enviar reporte a excel
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// </summary>
        private void btnReporteExcel_Click(object sender, EventArgs e)
        {
            progreso.Value = 0;
            botonGenerarPDF.Enabled = false;
            botonGenerarExcel.Enabled = false;
            TimeSpan ts = new TimeSpan(0, 0, 0);
            datePickerFinal.Value = datePickerFinal.Value.Date + ts;
            datePickerIniCorte.Value = datePickerIniCorte.Value.Date + ts;

            DateTime fechaFin = new DateTime();
            string idConjunto = string.Empty;
            string idSubConjunto = string.Empty;
            bool repIva = true;
            
            if (datePickerFinal.Visible)
                fechaFin = datePickerFinal.Value;
            if (cbxConjunto.Visible && cbxConjunto.Enabled)                
                idConjunto = cbxConjunto.SelectedValue.ToString().Trim();
            if (cbxSubConjunto.Visible)
                idSubConjunto = cbxSubConjunto.SelectedValue.ToString().Trim();
            repIva = radioButton3.Checked;
            reporte = new Reporte(this.clave, this.rutaFormato, this.nombreReporte, cbxInmobiliaria.SelectedValue.ToString(), idConjunto, idSubConjunto, repIva, datePickerIniCorte.Value.Date, fechaFin, false);
            reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
            workerReporte.RunWorkerAsync();   
        }

        /// <summary>Actualizar control de conjuntos y subconjuntos
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// </summary>
        private void cbxInmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (incluirConjunto)
            {
                DataTable dtConjuntos = inmobiliaria.getDtConjuntos(cbxInmobiliaria.SelectedValue.ToString().Trim());

                if (dtConjuntos.Rows.Count > 0)
                {
                    lblConjunto.Visible = true; cbxConjunto.Visible = true;                    
                    //cbxConjunto.DataSource = dtConjuntos;
                    DataView view = new DataView(dtConjuntos);
                    view.Sort = "P0303_NOMBRE";
                    cbxConjunto.DataSource = view; // 3 lineas modificadas para ordenar conjuntos
                    cbxConjunto.ValueMember = "P0301_ID_CENTRO";
                    cbxConjunto.DisplayMember = "P0303_NOMBRE"; 
                    cbxConjunto.SelectedIndex = 0;

                    if (incluirSubconjunto)
                    {
                        DataTable dtSubConjuntos = inmobiliaria.getDtSubConjuntos(cbxConjunto.SelectedValue.ToString().Trim());

                        if (dtSubConjuntos.Rows.Count > 0)
                        {
                            lblSubConjunto.Visible = true; cbxSubConjunto.Visible = true;
                            //cbxSubConjunto.DataSource = dtSubConjuntos;                           
                            DataView view2 = new DataView(dtSubConjuntos);
                            view2.Sort = "P1803_NOMBRE";
                            cbxSubConjunto.DataSource = view2; // 3 lineas modificadas para ordenar conjuntos
                            cbxSubConjunto.ValueMember = "P1801_ID_SUBCONJUNTO";
                            cbxSubConjunto.DisplayMember = "P1803_NOMBRE";
                            cbxSubConjunto.SelectedIndex = 0;
                        }
                        else
                        {
                            lblSubConjunto.Visible = false; cbxSubConjunto.Visible = false;
                        }
                    }
                    else
                    {
                        lblSubConjunto.Visible = false; cbxSubConjunto.Visible = false;
                    }
                }
                else
                {
                    lblConjunto.Visible = false; cbxConjunto.Visible = false;
                    lblSubConjunto.Visible = false; cbxSubConjunto.Visible = false;
                }
            }
        }

        /// <summary>Actualizar control de subconjuntos
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// </summary>
        private void cbxConjunto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (incluirSubconjunto)
            {
                DataTable dtSubConjuntos = inmobiliaria.getDtSubConjuntos(cbxConjunto.SelectedValue.ToString().Trim());

                if (dtSubConjuntos.Rows.Count > 0)
                {
                    lblSubConjunto.Visible = true; cbxSubConjunto.Visible = true;
                    cbxSubConjunto.DataSource = dtSubConjuntos;
                    cbxSubConjunto.ValueMember = "P1801_ID_SUBCONJUNTO";
                    cbxSubConjunto.DisplayMember = "P1803_NOMBRE";
                    cbxSubConjunto.SelectedIndex = 0;
                }
                else
                {
                    lblSubConjunto.Visible = false; cbxSubConjunto.Visible = false;
                }
            }
            else
            {
                lblSubConjunto.Visible = false; cbxSubConjunto.Visible = false;
            }
        }
             
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                cbxConjunto.Enabled = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
                cbxConjunto.Enabled = true;
        }

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonGenerarExcel.Enabled && botonGenerarPDF.Enabled)
                this.Close();
            else
                reporte.cancelar();
            botonCancelar.Enabled = false;
        }

        private void workerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
            error = reporte.generar();
        }

        private void workerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void workerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(error))
                MessageBox.Show("¡Reporte generado correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo generar el reporte: " + Environment.NewLine + error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonCancelar.Enabled = true;
            botonGenerarPDF.Enabled = true;
            botonGenerarExcel.Enabled = true;
        }

        private void Frm_Principal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerarPDF.Enabled || !botonGenerarExcel.Enabled)
                e.Cancel = true;
        }
    }
}