using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.PresentationLayer.Controls;
using GestorReportes.BusinessLayer.EntitiesPolizas;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_ConfigurarPoliza : Form
    {
        private bool configuracionCopropiedad=false;
        private string user = string.Empty; private string result = string.Empty;
        private bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;
        public Frm_ConfigurarPoliza(string usuario)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            configuracionCopropiedad = false;
            this.user = usuario;
        }
        public Frm_ConfigurarPoliza(bool esCopropiedad, string usuario)
        {
            InitializeComponent();
            if (esCopropiedad)
            {
                configuracionCopropiedad = true;
                this.Text = "Configuración de póliza contable de Copropietarios";
            }
            else
            {
                configuracionCopropiedad = false;
                this.Text = "Configuración de póliza contable";
            }
            this.user = usuario;

        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        public void cargarInmobiliarias()
        {
            try
            {
                cmbBxInmob.DataSource = HelpInmobiliarias.obtenerInmobiliarias(user, permisosInmo);
                cmbBxInmob.DisplayMember = "Razon_Nombre";
                cmbBxInmob.ValueMember = "ID";
                cmbBxInmob.SelectedIndex = 0;
                //IdInmobiliaria = cmbBxInmob.SelectedValue.ToString();
               // NombreInmo = cmbBxInmob.Text.ToString();
            }
            catch
            {
                result = "Error al obtener las inmobiliarias.";
            }
        }
        private void Frm_ConfigurarPoliza_Load(object sender, EventArgs e)
        {
            btnMas.Click += new EventHandler(botonOperacion);
            btnMenos.Click += new EventHandler(botonOperacion);
            btnMultiplica.Click += new EventHandler(botonOperacion);
            btnDivide.Click += new EventHandler(botonOperacion);
            btnAbreParent.Click += new EventHandler(botonOperacion);
            btnCierraParent.Click += new EventHandler(botonOperacion);
            try
            {
                //Inmobiliaria inmob = new Inmobiliaria();
                //DataTable dtInmobs = new DataTable();
                //dtInmobs = inmob.getDtInmobiliarias();
                //if (dtInmobs.Rows.Count > 0)
                //{
                //    DataView dvInmobs = new DataView(dtInmobs);
                //    dvInmobs.Sort = "P0103_RAZON_SOCIAL";
                //    cmbBxInmob.DataSource = dvInmobs;
                //    cmbBxInmob.DisplayMember = "P0103_RAZON_SOCIAL";
                //    cmbBxInmob.ValueMember = "P0101_ID_ARR";
                //}
                cargarInmobiliarias();
                DataTable dtTipos = GestorReportes.BusinessLayer.DataAccessLayer.Polizas.getDtTipos();
                if (dtTipos.Rows.Count > 0)
                {
                    foreach (DataRow rowTipo in dtTipos.Rows)
                    {
                        ListViewItem item = new ListViewItem(rowTipo["DESCR_CAT"].ToString());
                        item.SubItems.Add(rowTipo["TIPO"].ToString());
                        //item.SubItems.Add("T");
                        lstVwTipos.Items.Add(item);
                    }
                }
                DataTable dtSubTipos = GestorReportes.BusinessLayer.DataAccessLayer.Polizas.getDtSubTipos("IO");
                if (dtSubTipos.Rows.Count > 0)
                {
                    foreach (DataRow r in dtSubTipos.Rows)
                    {
                        ListViewItem item = new ListViewItem("S|" + r["DESCR_CAT"].ToString());
                        item.SubItems.Add(r["CAMPO1"].ToString());
                        //item.SubItems.Add("S");
                        lstVwTipos.Items.Add(item);
                    }
                }
                dtSubTipos = null;
                if (configuracionCopropiedad)
                {
                    dtSubTipos = GestorReportes.BusinessLayer.DataAccessLayer.Polizas.getDtSubTipos("IA");
                    if (dtSubTipos.Rows.Count > 0)
                    {
                        foreach (DataRow r in dtSubTipos.Rows)
                        {
                            ListViewItem item = new ListViewItem("IA|" + r["DESCR_CAT"].ToString());
                            item.SubItems.Add(r["CAMPO1"].ToString());
                            //item.SubItems.Add("S");
                            lstVwTipos.Items.Add(item);
                        }
                    }
                }

                lstVwDatos.Items[0].Selected = true;
                lstVwTipos.Items[0].Selected = true;
            }
            catch
            {
            }
        }

        void botonOperacion(object sender, EventArgs e)
        {
            if (flwLytPnlConfigs.Controls.Count > 0)
            {
                foreach (Control ctrl in flwLytPnlConfigs.Controls)
                {
                    if ((ctrl as Ctrl_PolizaConfig).rdBtnSelect.Checked)
                    {
                        (ctrl as Ctrl_PolizaConfig).txtBxFormula.AppendText((sender as Button).Text + " ");
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe existir un tipo de cuenta agregado.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        

        void rdBtnSelect_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in flwLytPnlConfigs.Controls)
                (ctrl as Ctrl_PolizaConfig).rdBtnSelect.Checked = false;
            (sender as RadioButton).Checked = true;
        }

        void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                Ctrl_PolizaConfig controlClickeado = (Ctrl_PolizaConfig)(sender as Button).Parent.Parent.Parent;
                flwLytPnlConfigs.Controls.Remove(controlClickeado);
                //MessageBox.Show(controlClickeado.Name);
            }
            catch //(Exception ex)
            {
            }
        }

        private void lstVwDatos_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (flwLytPnlConfigs.Controls.Count > 0)
            {
                ListViewItem item = lstVwDatos.GetItemAt(e.X, e.Y);
                foreach (Control ctrl in flwLytPnlConfigs.Controls)
                {
                    if ((ctrl as Ctrl_PolizaConfig).rdBtnSelect.Checked)
                    {
                        (ctrl as Ctrl_PolizaConfig).txtBxFormula.AppendText(item.Text.Split('|')[1].Trim() + " ");
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe existir un tipo de cuenta agregado.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void lstVwTipos_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            agregarFormula();
        }
        private void botonAgregar_ControlClicked(object sender, EventArgs e)
        {
            agregarFormula();
        }
        private void btnAgregarControl_Click(object sender, EventArgs e)
        {
            try
            {
                rdBtnGeneracion.Enabled = false;
                rdBtnCobranza.Enabled = false;
                rdBtnCancel.Enabled = false;
                rdBtnNotasCredito.Enabled = false;
                radioIngresoConsolidado.Enabled = false;
                string moneda = string.Empty;
                if (rdBtnPesos.Checked)
                    moneda = rdBtnPesos.Text;
                else
                    moneda = rdBtnDolares.Text;
                string selectedText = lstVwTipos.SelectedItems[0].Text;
                if (!flwLytPnlConfigs.Controls.ContainsKey(selectedText + moneda))
                {
                    if (flwLytPnlConfigs.Controls.Count > 0)
                    {
                        foreach (Control ctrl in flwLytPnlConfigs.Controls)
                            (ctrl as Ctrl_PolizaConfig).rdBtnSelect.Checked = false;
                    }
                    Ctrl_PolizaConfig polizaConfig = new Ctrl_PolizaConfig();
                    polizaConfig.Name = selectedText + moneda;
                    polizaConfig.rdBtnSelect.Checked = true;
                    polizaConfig.lblMoneda.Text = rdBtnPesos.Checked ? rdBtnPesos.Text : rdBtnDolares.Text;
                    polizaConfig.lblClaveTipo.Text = lstVwTipos.SelectedItems[0].SubItems[1].Text;
                    polizaConfig.lblTipo.Text = selectedText.Contains("Complementaria de") ? selectedText.Replace("Complementaria de", "Comp.") : selectedText;
                    polizaConfig.lblTipo.Text = polizaConfig.lblTipo.Text.Length > 13 ? polizaConfig.lblTipo.Text.Substring(0, 13) : polizaConfig.lblTipo.Text;
                    polizaConfig.btnEliminar.Click += new EventHandler(btnEliminar_Click);
                    polizaConfig.rdBtnSelect.Click += new EventHandler(rdBtnSelect_Click);
                    flwLytPnlConfigs.Controls.Add(polizaConfig);
                }
                else
                    MessageBox.Show("Ya existe el control seleccionado para la moneda seleccionada.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
                MessageBox.Show("Ha ocurrido un error no esperado al agregar control. Inténtelo de nuevo.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            bool guardar = true;
            ConfiguracionPolizaEntity configuracion = new ConfiguracionPolizaEntity();
            configuracion.Inmobiliaria = cmbBxInmob.SelectedValue.ToString();
            if (!configuracionCopropiedad)
            {
                if (rdBtnGeneracion.Checked)
                    configuracion.TipoPoliza = 1;
                else if (rdBtnCobranza.Checked)
                    configuracion.TipoPoliza = 2;
                else if (rdBtnCancel.Checked)
                    configuracion.TipoPoliza = 3;
                else if (rdBtnNotasCredito.Checked)
                    configuracion.TipoPoliza = 4;
                else if(radioIngresoConsolidado.Checked)
                    configuracion.TipoPoliza = 5;
                else if(rdBtnCobranzaVtas.Checked)
                    configuracion.TipoPoliza = 6;
            }
            else
            {
                if (rdBtnGeneracion.Checked)
                    configuracion.TipoPoliza = 11;
                else if (rdBtnCobranza.Checked)
                    configuracion.TipoPoliza = 12;
                else if (rdBtnCancel.Checked)
                    configuracion.TipoPoliza = 13;
                else if (rdBtnNotasCredito.Checked)
                    configuracion.TipoPoliza = 14;
                else if (radioIngresoConsolidado.Checked)
                    configuracion.TipoPoliza = 15;
                else if (rdBtnCobranzaVtas.Checked)
                    configuracion.TipoPoliza = 16;
            }
            ConfiguracionesPolizas configuraciones = new ConfiguracionesPolizas();
            bool existenRegistros = configuraciones.existeConfig(configuracion);
            if (flwLytPnlConfigs.Controls.Count <= 0)
            {
                if (existenRegistros)
                {
                    if (DialogResult.Yes == MessageBox.Show("Ya existe un registro para esa inmobiliaria y ese tipo de póliza. ¿Desea eliminar la configuración?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        if (configuraciones.borrar(configuracion))
                        {
                            MessageBox.Show("Configuración eliminada con éxito.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Application.Exit();
                        }
                        else
                            MessageBox.Show("No se pudo eliminar la configuración existente.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Debe existir al menos un control agregado.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                guardar = false;
            }
            foreach (Control ctrl in flwLytPnlConfigs.Controls)
            {
                if (string.IsNullOrEmpty((ctrl as Ctrl_PolizaConfig).txtBxFormula.Text))
                {
                    MessageBox.Show("El campo formula del control " + (ctrl as Ctrl_PolizaConfig).lblTipo.Text + " en moneda " + (ctrl as Ctrl_PolizaConfig).lblMoneda.Text + " está vacío.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    guardar = false;
                }
            }
            if (guardar)
            {                
                List<FormulaPolizaEntity> formulas = new List<FormulaPolizaEntity>();
                foreach (Control ctrl in flwLytPnlConfigs.Controls)
                {
                    Ctrl_PolizaConfig ctrlConf = (ctrl as Ctrl_PolizaConfig);
                    FormulaPolizaEntity formula = new FormulaPolizaEntity();
                    formula.Tipo = ctrlConf.lblTipo.Text.Contains("|")? ctrlConf.lblTipo.Text.Split('|')[1] : ctrlConf.lblTipo.Text;
                    formula.TipoClave = ctrlConf.lblClaveTipo.Text;
                    formula.CargoAbono = ctrlConf.cmbBxCargoAbono.SelectedIndex == 0 ? 1 : 2;
                    formula.Moneda = ctrlConf.lblMoneda.Text == "Pesos" ? "P" : "D";
                    if (ctrlConf.tipoPolizaConfig == "INGRESO")
                        formula.MonedaPago = ctrlConf.lblValMonCobro.Text == "Pesos" ? "P" : "D";
                    else
                        formula.MonedaPago = formula.Moneda;
                    formula.Formula = ctrlConf.txtBxFormula.Text;
                    formula.EsSubtipo = ctrlConf.lblTipo.Text.Contains("|");
                    formulas.Add(formula);
                }
                configuracion.Formulas = formulas;
                string errores = string.Empty;                
                if (existenRegistros)
                {
                    if (DialogResult.Yes == MessageBox.Show("Ya existe un registro para esa inmobiliaria y ese tipo de póliza. ¿Desea reemplazar la configuración?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        errores = configuraciones.reemplazar(configuracion);
                        if (!string.IsNullOrEmpty(errores))
                        {
                            MessageBox.Show("Se encontraron los siguientes errores: " + Environment.NewLine + errores, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Configuración reemplazada con éxito.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Application.Exit();
                        }
                    }
                }
                else
                {
                    errores = configuraciones.guardar(configuracion);
                    if (!string.IsNullOrEmpty(errores))
                    {
                        MessageBox.Show("Se encontraron los siguientes errores: " + Environment.NewLine + errores, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Configuración guardada con éxito.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Application.Exit();
                    }
                }
            }
        }

        private void boton_MouseEnter(object sender, EventArgs e)
        {
            var boton = sender as Button;
            if (boton != null)
                boton.BackColor = Color.Orange;
        }

        private void boton_MouseLeave(object sender, EventArgs e)
        {
            var boton = sender as Button;
            if (boton != null)
                boton.BackColor = this.BackColor;
        }

        private void agregarFormula()
        {
            try
            {
                rdBtnGeneracion.Enabled = false;
                rdBtnCobranza.Enabled = false;
                rdBtnCancel.Enabled = false;
                rdBtnNotasCredito.Enabled = false;
                radioIngresoConsolidado.Enabled = false;
                rdBtnCobranzaVtas.Enabled = false;

                string moneda = string.Empty;
                string monedaCobro = string.Empty;

                if (rdBtnPesos.Checked)
                    moneda = rdBtnPesos.Text;
                else
                    moneda = rdBtnDolares.Text;

                if(rdBtnPesosCobro.Checked)
                    monedaCobro = rdBtnPesosCobro.Text;
                else
                    monedaCobro = rdBtnDolaresCobro.Text;


                string selectedText = lstVwTipos.SelectedItems[0].Text;

                if (!flwLytPnlConfigs.Controls.ContainsKey(selectedText + moneda) && !rdBtnCobranza.Checked)
                {                 
                    if (flwLytPnlConfigs.Controls.Count > 0)
                    {
                        foreach (Control ctrl in flwLytPnlConfigs.Controls)
                            (ctrl as Ctrl_PolizaConfig).rdBtnSelect.Checked = false;
                    }
                    Ctrl_PolizaConfig polizaConfig = new Ctrl_PolizaConfig();                    
                    polizaConfig.tipoPolizaConfig = "";                   
                    polizaConfig.Name = selectedText + moneda;
                    polizaConfig.rdBtnSelect.Checked = true;
                    polizaConfig.lblMoneda.Text = rdBtnPesos.Checked ? rdBtnPesos.Text : rdBtnDolares.Text;
                    polizaConfig.lblClaveTipo.Text = lstVwTipos.SelectedItems[0].SubItems[1].Text;
                    polizaConfig.lblTipo.Text = selectedText.Contains("Complementaria de") ? selectedText.Replace("Complementaria de", "Comp.") : selectedText;
                    //polizaConfig.lblTipo.Text = polizaConfig.lblTipo.Text.Length > 13 ? polizaConfig.lblTipo.Text.Substring(0, 13) : polizaConfig.lblTipo.Text;
                    polizaConfig.btnEliminar.Click += new EventHandler(btnEliminar_Click);
                    polizaConfig.rdBtnSelect.Click += new EventHandler(rdBtnSelect_Click);
                    flwLytPnlConfigs.Controls.Add(polizaConfig);
                                        
                }
                else if (rdBtnCobranza.Checked && !flwLytPnlConfigs.Controls.ContainsKey(selectedText + moneda + monedaCobro))
                {
                    if (flwLytPnlConfigs.Controls.Count > 0)
                    {
                        foreach (Control ctrl in flwLytPnlConfigs.Controls)
                            (ctrl as Ctrl_PolizaConfig).rdBtnSelect.Checked = false;
                    }
                    Ctrl_PolizaConfig polizaConfig = new Ctrl_PolizaConfig();

                    polizaConfig.tipoPolizaConfig = "INGRESO";
                    groupBox3.Visible = true;
               
                    polizaConfig.Name = selectedText + moneda + monedaCobro;
                    
                    polizaConfig.monedaCobro = monedaCobro;

                    polizaConfig.rdBtnSelect.Checked = true;
                    polizaConfig.lblMoneda.Text = rdBtnPesos.Checked ? rdBtnPesos.Text : rdBtnDolares.Text;
                    polizaConfig.lblClaveTipo.Text = lstVwTipos.SelectedItems[0].SubItems[1].Text;
                    polizaConfig.lblTipo.Text = selectedText.Contains("Complementaria de") ? selectedText.Replace("Complementaria de", "Comp.") : selectedText;

                    polizaConfig.lblValMonCobro.Text = monedaCobro;

                    //polizaConfig.lblTipo.Text = polizaConfig.lblTipo.Text.Length > 13 ? polizaConfig.lblTipo.Text.Substring(0, 13) : polizaConfig.lblTipo.Text;
                    polizaConfig.btnEliminar.Click += new EventHandler(btnEliminar_Click);
                    polizaConfig.rdBtnSelect.Click += new EventHandler(rdBtnSelect_Click);
                    flwLytPnlConfigs.Controls.Add(polizaConfig);
                }                    
                else
                    MessageBox.Show("Ya existe el control seleccionado para la moneda seleccionada.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
                MessageBox.Show("Ha ocurrido un error no esperado al agregar control. Inténtelo de nuevo.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rdBtnCobranza_CheckedChanged(object sender, EventArgs e)
        {
            groupBox3.Visible = rdBtnCobranza.Checked;
        }

        
    }
}
