using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesPolizas;
using GestorReportes.BusinessLayer.Helpers;
using GestorReportes.BusinessLayer.EntitiesReportes;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_GenerarPoliza : Form
    {
        private Poliza poliza = null;
        private PolizaCopropietarios polizaCoprop = null;

        private string resultGenerar = string.Empty;
        private bool esPolizaCopropiedad = false;
        private bool esPolizaVenta = false;
        private bool esPolizaRenta = false;
        private bool PolizasPorRango = false;
        private bool permisosInmobiliaria = false;
        private string usuario = string.Empty;
        private string iDinmobiliaria = string.Empty; string NombreInmo = string.Empty; string result = string.Empty;
        public Frm_GenerarPoliza()
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            esPolizaCopropiedad = false;
            this.usuario = "Administrador";
            #region leer configuracion
            try
            {
                permisosInmobiliaria = Properties.Settings.Default.PermisosInmobiliaria;
            }
            catch
            {
                permisosInmobiliaria = false;
            }
            #endregion

        }
        public Frm_GenerarPoliza(string tipoPoliza, string usuario)
        {
            InitializeComponent();
            SaariIcon.SaariIcon.setSaariIcon(this);
            #region leer configuracion
            try
            {
                permisosInmobiliaria = Properties.Settings.Default.PermisosInmobiliaria;
            }
            catch
            {
                permisosInmobiliaria = false;
            }
            #endregion

            if (tipoPoliza == "CoPropiedad")
            {
                esPolizaCopropiedad = true;
                esPolizaVenta = false;
                esPolizaRenta = false;
                if (string.IsNullOrEmpty(usuario))
                    this.Text = "Generación de Póliza de Copropietarios";
                else
                    this.Text = "Generación de Póliza de Copropietarios - " + usuario;
            }
            else if (tipoPoliza == "Venta")
            {
                esPolizaCopropiedad = false;
                esPolizaVenta = true;
                esPolizaRenta = false;
                if (string.IsNullOrEmpty(usuario))
                    this.Text = "Generación de Póliza de Ventas";
                else
                    this.Text = "Generación de Póliza de Ventas - " + usuario;
            }
            else
            {
                esPolizaCopropiedad = false;
                esPolizaVenta = false;
                esPolizaRenta = true;

                this.Text += " - " + usuario;
            }
            this.usuario = usuario;
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
            {
                if (esPolizaCopropiedad)
                    polizaCoprop.cancelar();
                else
                    poliza.cancelar();
            }
            botonCancelar.Enabled = false;
        }

        private void cargarInmobiliarias()
        {
            try
            {

                List<InmobiliariaEntity> listaInmobiliarias = HelpInmobiliarias.obtenerInmobiliarias(usuario, permisosInmobiliaria);
                cmbBxInmob.DataSource = listaInmobiliarias;
                if (listaInmobiliarias.Count <= 0 && permisosInmobiliaria)
                {
                    MessageBox.Show("El usuario " + usuario + " no tiene asignados permisos a ninguna inmobiliaria. Favor de verificar los permisos del usuario.", "SAARI", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.Close();
                }
                else if (listaInmobiliarias.Count <= 0 && !permisosInmobiliaria)
                {
                    MessageBox.Show("Ocurrio un problema al intentar cargar la lista de inmobiliarias. Favor de notificar al Administrador de Sistemas.", "SAARI", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.Close();
                }
                cmbBxInmob.DisplayMember = "RazonSocial";

                cmbBxInmob.ValueMember = "ID";
                cmbBxInmob.SelectedIndex = 0;
                iDinmobiliaria = cmbBxInmob.SelectedValue.ToString();
                NombreInmo = cmbBxInmob.Text.ToString();

            }
            catch
            {
                result = "Error al obtener las inmobiliarias.";
            }
        }

        //private void cargarConjuntos(string selectedValue)
        //{
        //    comboConjunto.DataSource = HelpInmobiliarias.getConjuntos(iDinmobiliaria); //.ObtenerConjuntos(iDinmobiliaria);
        //    comboConjunto.DisplayMember = "Nombre";
        //    comboConjunto.ValueMember = "ID";
        //    idConjunto = comboConjunto.SelectedValue.ToString();
        //    NombreConjunto = comboConjunto.Text.ToString();
        //    comboConjunto.SelectedIndex = 0;
        //}

        private void rdBtnContpaq_CheckedChanged(object sender, EventArgs e)
        {
            checkMascara.Visible = rdBtnContpaq.Checked;
            textoMascara.Visible = rdBtnContpaq.Checked;
            checkBoxGuionEnMascara.Visible = rdBtnContpaq.Checked;
            checkMultiples.Visible = rdBtnContpaq.Checked;
            checkSegmento.Visible = rdBtnContpaq.Checked;
            checkAfectar.Visible = rdBtnContpaq.Checked;
            checkCliente.Visible = rdBtnContpaq.Checked;
            checkReferencia.Visible = rdBtnContpaq.Checked;
            //checkUUID.Visible = rdBtnContpaq.Checked;
            grupoConcepMov.Visible = rdBtnContpaq.Checked;

        }

        private void Frm_GenerarPoliza_Load(object sender, EventArgs e)
        {
            try
            {
                Inmobiliaria inmob = new Inmobiliaria();
                DataTable dtInmobs = new DataTable();

                if (esPolizaCopropiedad)
                {
                    //TO DO: Agregar metodos para consultar
                    //dtInmobs = inmob.getDtInmobiliariasEnCopropiedad();
                    // dtInmobs = inmob.getDtInmobiliarias();--Este motodo estaba
                    cargarInmobiliarias();
                }
                else
                {
                    cargarInmobiliarias();
                    // dtInmobs = inmob.getDtInmobiliarias();
                }
                //if (dtInmobs.Rows.Count > 0)
                //{
                //    DataView dvInmobs = new DataView(dtInmobs);
                //    dvInmobs.Sort = "P0103_RAZON_SOCIAL";
                //    cmbBxInmob.DataSource = dvInmobs;
                //    cmbBxInmob.DisplayMember = "P0103_RAZON_SOCIAL";
                //    cmbBxInmob.ValueMember = "P0101_ID_ARR";
                //}

                comboSucursales.DataSource = Poliza.obtenerSucursales();
                comboSucursales.ValueMember = "Identificador";
                comboSucursales.DisplayMember = "NombreSucursal";

                if (esPolizaCopropiedad)
                {
                    lblFechaFin.Visible = true;
                    dateTimePicker2.Visible = true;
                    lblFechaInicio.Text = "Fecha inicio:";
                }
                else
                {
                    lblFechaFin.Visible = false;
                    dateTimePicker2.Visible = false;
                    lblFechaInicio.Text = "Fecha:";
                }
                try
                {
                    PolizasPorRango = Properties.Settings.Default.PolizaPorRango;
                    if (PolizasPorRango)
                    {
                        lblFechaFin.Visible = true;
                        dateTimePicker2.Visible = true;
                        lblFechaInicio.Text = "Fecha inicio:";
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al cargar las inmobiliarias, consulte con su administrador de sistemas. Error : " +
                    ex.Message, "Error con Inmobiliarias", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            bool continuar = true;
            List<ColumnasExcelEntity> columnas = new List<ColumnasExcelEntity>();
            if (rdBtnExc.Checked)
            {
                if (dateTimePicker1.Value <= dateTimePicker2.Value)
                {
                    Frm_FormatoPolizaExcel formatoExc = new Frm_FormatoPolizaExcel();
                    formatoExc.ShowDialog();
                    if (formatoExc.Columnas.Count <= 0)
                        continuar = false;
                    else
                    {
                        continuar = true;
                        columnas = formatoExc.Columnas;
                    }
                }
                else
                {
                    continuar = false;
                    MessageBox.Show("La fecha de fin debe ser mayor a la fecha de inicio.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            if (continuar)
            {
                CondicionesEntity condiciones = new CondicionesEntity();
                condiciones.Inmobiliaria = cmbBxInmob.SelectedValue.ToString();
                if (rdBtnContpaq.Checked)
                {
                    condiciones.Formato = FormatoExportacionPoliza.ContpaqTxt;
                    if (textoMascara.Enabled)
                    {
                        condiciones.Mascara = textoMascara.Text;
                        condiciones.GuionEnCuenta = checkBoxGuionEnMascara.Checked;
                        Properties.Settings.Default.ContpaqMask = textoMascara.Text;
                        Properties.Settings.Default.Save();
                    }
                    condiciones.MultiplesEncabezados = checkMultiples.Checked;
                    condiciones.IncluirSegmento = checkSegmento.Checked;
                }
                else if (rdBtnExc.Checked)
                    condiciones.Formato = FormatoExportacionPoliza.Excel;
                else if (rdBtnAspel.Checked)
                    condiciones.Formato = FormatoExportacionPoliza.Aspel;
                else if (rdBtnContavision.Checked)
                    condiciones.Formato = FormatoExportacionPoliza.Contavision;
                else if (radioAxapta.Checked)
                    condiciones.Formato = FormatoExportacionPoliza.Axapta;
                else
                {
                    condiciones.Formato = FormatoExportacionPoliza.ContpaqXls;
                    if (textoMascara.Enabled)
                    {
                        condiciones.Mascara = textoMascara.Text;
                        condiciones.GuionEnCuenta = checkBoxGuionEnMascara.Checked;
                        Properties.Settings.Default.ContpaqMask = textoMascara.Text;
                        Properties.Settings.Default.Save();
                    }
                    condiciones.MultiplesEncabezados = checkMultiples.Checked;
                    condiciones.IncluirSegmento = checkSegmento.Checked;
                }
                condiciones.NumeroPoliza = txtBxIdenPol.Text.Trim();
                condiciones.ConceptoPoliza = txtBxConceptoPoliza.Text.Trim();
                condiciones.FechaInicio = dateTimePicker1.Value;
                condiciones.FechaFin = dateTimePicker2.Value;
                condiciones.IncluirCancelados = rdBtnSi.Checked;
                condiciones.TipoPresentacion = 2;
                condiciones.IncluirUUIDEnConcepto = checkUUID.Checked;
                condiciones.NombreCliente = checkCliente.Checked;
                condiciones.AfectarSaldos = checkAfectar.Checked;
                condiciones.UsarConceptoPrincipal = checkConceptoPrincipal.Checked;
                condiciones.IncluirPeriodoEnReferencia = checkReferencia.Checked;
                condiciones.ClienteEnLugarDePeriodo = radioCliente.Checked;
                condiciones.ExcluirConceptoLibre = checkExcluir.Checked;
                condiciones.PolizaPorRango = PolizasPorRango;
                //if (groupBox3.Visible)                
                //condiciones.TipoPresentacion = rdBtnConsolidado.Checked ? 1 : 2;
                if (rdBtnDetallado.Checked)
                    condiciones.TipoPresentacion = 2;
                else if (rdBtnConsolidado.Checked)
                    condiciones.TipoPresentacion = 1;
                else
                    condiciones.TipoPresentacion = 3;
                if (esPolizaCopropiedad)
                {
                    if (rdBtnDiario.Checked)
                        condiciones.TipoPoliza = 11;
                    else if (rdBtnIngreso.Checked)
                        condiciones.TipoPoliza = 12;
                    else if (rdBtnCancelados.Checked)
                        condiciones.TipoPoliza = 13;
                    else
                        condiciones.TipoPoliza = 14;
                }
                else if (esPolizaRenta)
                {
                    if (rdBtnDiario.Checked)
                        condiciones.TipoPoliza = 1;
                    else if (rdBtnIngreso.Checked)
                        condiciones.TipoPoliza = 2;
                    else if (rdBtnCancelados.Checked)
                        condiciones.TipoPoliza = 3;
                    else
                        condiciones.TipoPoliza = 4;
                }
                else if (esPolizaVenta)
                {
                    if (rdBtnDiario.Checked)
                        condiciones.TipoPoliza = 5;
                    else if (rdBtnIngreso.Checked)
                        condiciones.TipoPoliza = 6;
                    else if (rdBtnCancelados.Checked)
                        condiciones.TipoPoliza = 7;
                    else
                        condiciones.TipoPoliza = 8;
                }

                if (checkRfcUuid.Checked)
                    condiciones.RfcUuid = true;

                condiciones.Sucursal = comboSucursales.SelectedItem as SucursalEntity;

                if (radioButton4Dec.Checked)
                    condiciones.CuatroDecimales = true;
                else
                    condiciones.CuatroDecimales = false;
                //Para asignar diarios especiales a la poliza tipo Contpaq .xls
                condiciones.DiariosEspeciales = checkDiariosEspeciales.Checked;
                progreso.Value = 0;
                botonGenerar.Enabled = false;
                if (esPolizaCopropiedad)
                {
                    polizaCoprop = new PolizaCopropietarios(condiciones, columnas);
                    polizaCoprop.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(poliza_CambioProgreso);
                }
                else
                {
                    poliza = new Poliza(condiciones, columnas);
                    poliza.esRentas = esPolizaRenta;
                    poliza.esVentas = esPolizaVenta;
                    poliza.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(poliza_CambioProgreso);
                }
                worker.RunWorkerAsync();
            }
        }

        void poliza_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            worker.ReportProgress(e.Progreso);
        }

        private void rdBtnDiario_CheckedChanged(object sender, EventArgs e)
        {
            //groupBox3.Visible = rdBtnDiario.Checked;
        }

        private void rdBtnCancelados_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Visible = !rdBtnCancelados.Checked;
        }

        private void rdBtnExc_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnExc.Checked || radioGlobal.Checked)
            {
                lblFechaFin.Visible = true;
                dateTimePicker2.Visible = true;
                lblFechaInicio.Text = "Fecha inicio:";
            }
            else
            {
                lblFechaFin.Visible = false;
                dateTimePicker2.Visible = false;
                lblFechaInicio.Text = "Fecha:";
            }
        }

        private void rdBtnAspel_CheckedChanged(object sender, EventArgs e)
        {
            checkUUID.Visible = rdBtnAspel.Checked;
            if (checkUUID.Visible)
                checkUUID.Top -= (checkUUID.Height * 3);
            else
                checkUUID.Top += (checkUUID.Height * 3);
        }

        private void textoMascara_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '-')
            {
                e.Handled = true;
                return;
            }
        }

        private void checkMascara_CheckedChanged(object sender, EventArgs e)
        {
            textoMascara.Enabled = checkMascara.Checked;
            if (!textoMascara.Enabled)
                textoMascara.Clear();
            else
                textoMascara.Text = Properties.Settings.Default.ContpaqMask;
        }

        private void checkCliente_CheckedChanged(object sender, EventArgs e)
        {
            if (checkCliente.Checked)
            {
                txtBxConceptoPoliza.Clear();
                txtBxConceptoPoliza.Enabled = false;
            }
            else
                txtBxConceptoPoliza.Enabled = true;
        }

        private void checkMultiples_CheckedChanged(object sender, EventArgs e)
        {
            checkCliente.Enabled = checkMultiples.Checked;
            if (!checkMultiples.Checked)
                checkCliente.Checked = false;
        }

        private void radioContpaqXls_CheckedChanged(object sender, EventArgs e)
        {
            checkMascara.Visible = radioContpaqXls.Checked;
            textoMascara.Visible = radioContpaqXls.Checked;
            checkMultiples.Visible = radioContpaqXls.Checked;
            checkSegmento.Visible = radioContpaqXls.Checked;
            checkAfectar.Visible = radioContpaqXls.Checked;
            checkCliente.Visible = radioContpaqXls.Checked;
            checkReferencia.Visible = radioContpaqXls.Checked;
            checkBoxGuionEnMascara.Visible = radioContpaqXls.Checked;
            checkUUID.Visible = radioContpaqXls.Checked;
            grupoConcepMov.Visible = radioContpaqXls.Checked;
        }

        private void radioGlobal_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnExc.Checked || radioGlobal.Checked)
            {
                lblFechaFin.Visible = true;
                dateTimePicker2.Visible = true;
                lblFechaInicio.Text = "Fecha inicio:";
            }
            else
            {
                if (esPolizaCopropiedad)
                {
                    lblFechaFin.Visible = true;
                    dateTimePicker2.Visible = true;
                    lblFechaInicio.Text = "Fecha inicio:";
                }
                else
                {


                    try
                    {
                        if (PolizasPorRango)
                        {
                            lblFechaFin.Visible = true;
                            dateTimePicker2.Visible = true;
                            lblFechaInicio.Text = "Fecha inicio:";
                        }
                        else
                        {
                            lblFechaFin.Visible = false;
                            dateTimePicker2.Visible = false;
                            lblFechaInicio.Text = "Fecha:";
                        }
                    }
                    catch { }


                }

            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (esPolizaCopropiedad)
                resultGenerar = polizaCoprop.generar();
            else
                resultGenerar = poliza.generar();
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(resultGenerar))
            {
                if (esPolizaCopropiedad)
                    MessageBox.Show("¡Póliza " + polizaCoprop.NombreArchivo + " generada correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("¡Póliza " + poliza.NombreArchivo + " generada correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
                MessageBox.Show("No se pudo generar la póliza solicitada." + Environment.NewLine + resultGenerar, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void Frm_GenerarPoliza_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

        private void radioAxapta_CheckedChanged(object sender, EventArgs e)
        {
            if(checkRfcUuid.Visible)
                checkRfcUuid.Visible = false;
            else
                checkRfcUuid.Visible = true;
        }

        //private void rdBtnIngreso_CheckedChanged(object sender, EventArgs e)
        //{
        //    checkDiariosEspeciales.Visible = rdBtnIngreso.Checked;
        //}
    }
}
