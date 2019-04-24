using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.PresentationLayer.Controls;
using GestorReportes.BusinessLayer.EntitiesPolizas;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_ConfigurarPolizaEgreso : Form
    {
        private bool permisosInmo = false;
        private string user = string.Empty; 
        public Frm_ConfigurarPolizaEgreso(string usuario)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            btnMas.Click += new EventHandler(botonOperacion);
            btnMenos.Click += new EventHandler(botonOperacion);
            btnMultiplica.Click += new EventHandler(botonOperacion);
            btnDivide.Click += new EventHandler(botonOperacion);
            btnAbreParent.Click += new EventHandler(botonOperacion);
            btnCierraParent.Click += new EventHandler(botonOperacion);
            this.user = usuario;
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

        private void botonCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void cargarInmobiliarias()
        {
            try
            {
                comboInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(user, permisosInmo);
                comboInmobiliaria.DisplayMember = "RazonSocial";
                comboInmobiliaria.ValueMember = "ID";
                comboInmobiliaria.SelectedIndex = 0;
            }
            catch
            {}
        }
        void botonOperacion(object sender, EventArgs e)
        {
            if (flowConfiguraciones.Controls.Count > 0)
            {
                foreach (Control ctrl in flowConfiguraciones.Controls)
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

        private void Frm_ConfigurarPolizaEgreso_Load(object sender, EventArgs e)
        {
            try
            {
                //Inmobiliaria inmob = new Inmobiliaria();
                //DataTable dtInmobs = new DataTable();
                //dtInmobs = inmob.getDtInmobiliarias();
                //if (dtInmobs.Rows.Count > 0)
                //{
                //    DataView dvInmobs = new DataView(dtInmobs);
                //    dvInmobs.Sort = "P0103_RAZON_SOCIAL";
                //    comboInmobiliaria.DataSource = dvInmobs;
                //    comboInmobiliaria.DisplayMember = "P0103_RAZON_SOCIAL";
                //    comboInmobiliaria.ValueMember = "P0101_ID_ARR";
                //}
                cargarInmobiliarias();
                
                DataTable dtTipos = GestorReportes.BusinessLayer.DataAccessLayer.Polizas.getDtTiposEgreso();
                if (dtTipos.Rows.Count > 0)
                {
                    foreach (DataRow rowTipo in dtTipos.Rows)
                    {
                        ListViewItem item = new ListViewItem(rowTipo["DESCR_CAT"].ToString());
                        item.SubItems.Add(rowTipo["TIPO"].ToString());
                        lstVwTipos.Items.Add(item);
                    }
                }
                /*DataTable dtSubTipos = GestorReportes.BusinessLayer.DataAccessLayer.Polizas.getDtSubTipos("IO");
                if (dtSubTipos.Rows.Count > 0)
                {
                    foreach (DataRow r in dtSubTipos.Rows)
                    {
                        ListViewItem item = new ListViewItem("S|" + r["DESCR_CAT"].ToString());
                        item.SubItems.Add(r["CAMPO1"].ToString());
                        //item.SubItems.Add("S");
                        lstVwTipos.Items.Add(item);
                    }
                }*/
              
                lstVwDatos.Items[0].Selected = true;
                lstVwTipos.Items[0].Selected = true;
            }
            catch
            {
            }
        }

        private void btnAgregarControl_Click(object sender, EventArgs e)
        {
            try
            {
                radioEgreso.Enabled = false;
                radioProvision.Enabled = false;
                string selectedText = lstVwTipos.SelectedItems[0].Text;
                if (!flowConfiguraciones.Controls.ContainsKey(selectedText))
                {
                    if (flowConfiguraciones.Controls.Count > 0)
                    {
                        foreach (Control ctrl in flowConfiguraciones.Controls)
                            (ctrl as Ctrl_PolizaConfig).rdBtnSelect.Checked = false;
                    }
                    Ctrl_PolizaConfig polizaConfig = new Ctrl_PolizaConfig(true);
                    polizaConfig.Name = selectedText;
                    polizaConfig.rdBtnSelect.Checked = true;
                    polizaConfig.lblMoneda.Text = string.Empty;
                    polizaConfig.lblClaveTipo.Text = lstVwTipos.SelectedItems[0].SubItems[1].Text;
                    polizaConfig.lblTipo.Text = selectedText.Contains("Complementaria de") ? selectedText.Replace("Complementaria de", "Comp.") : selectedText;
                    polizaConfig.lblTipo.Text = polizaConfig.lblTipo.Text.Length > 13 ? polizaConfig.lblTipo.Text.Substring(0, 13) : polizaConfig.lblTipo.Text;
                    polizaConfig.btnEliminar.Click += new EventHandler(btnEliminar_Click);
                    polizaConfig.rdBtnSelect.Click += new EventHandler(rdBtnSelect_Click);
                    flowConfiguraciones.Controls.Add(polizaConfig);
                }
                else
                    MessageBox.Show("Ya existe el control seleccionado", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
                MessageBox.Show("Ha ocurrido un error no esperado al agregar control. Inténtelo de nuevo.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void rdBtnSelect_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in flowConfiguraciones.Controls)
                (ctrl as Ctrl_PolizaConfig).rdBtnSelect.Checked = false;
            (sender as RadioButton).Checked = true;
        }

        void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                Ctrl_PolizaConfig controlClickeado = (Ctrl_PolizaConfig)(sender as Button).Parent.Parent;
                flowConfiguraciones.Controls.Remove(controlClickeado);
                
            }
            catch 
            {
            }
        }

        private void lstVwDatos_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (flowConfiguraciones.Controls.Count > 0)
            {
                ListViewItem item = lstVwDatos.GetItemAt(e.X, e.Y);
                foreach (Control ctrl in flowConfiguraciones.Controls)
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
            radioEgreso.Enabled = false;
            radioProvision.Enabled = false;
            ListViewItem item = lstVwTipos.GetItemAt(e.X, e.Y);            
            if (!flowConfiguraciones.Controls.ContainsKey(item.Text))
            {
                if (flowConfiguraciones.Controls.Count > 0)
                {
                    foreach (Control ctrl in flowConfiguraciones.Controls)
                        (ctrl as Ctrl_PolizaConfig).rdBtnSelect.Checked = false;
                }
                Ctrl_PolizaConfig polizaConfig = new Ctrl_PolizaConfig(true);
                polizaConfig.Name = item.Text;
                polizaConfig.rdBtnSelect.Checked = true;
                polizaConfig.lblMoneda.Text = string.Empty;
                polizaConfig.lblClaveTipo.Text = lstVwTipos.SelectedItems[0].SubItems[1].Text;
                polizaConfig.lblTipo.Text = item.Text.Contains("Complementaria de") ? item.Text.Replace("Complementaria de", "Comp.") : item.Text;
                polizaConfig.btnEliminar.Click += new EventHandler(btnEliminar_Click);
                polizaConfig.rdBtnSelect.Click += new EventHandler(rdBtnSelect_Click);
                flowConfiguraciones.Controls.Add(polizaConfig);
            }
            else
            {
                MessageBox.Show("Ya existe el control seleccionado", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void botonGuardar_Click(object sender, EventArgs e)
        {
            bool guardar = true;
            ConfiguracionPolizaEntity configuracion = new ConfiguracionPolizaEntity();
            configuracion.Inmobiliaria = comboInmobiliaria.SelectedValue.ToString();
            if (radioEgreso.Checked)
                configuracion.TipoPoliza = 1;
            else
                configuracion.TipoPoliza = 2;
            ConfiguracionesPolizas configuraciones = new ConfiguracionesPolizas(true);
            bool existenRegistros = configuraciones.existeConfig(configuracion);
            if (flowConfiguraciones.Controls.Count <= 0)
            {
                if (existenRegistros)
                {
                    if (DialogResult.Yes == MessageBox.Show("Ya existe un registro para esa inmobiliaria y ese tipo de póliza. ¿Desea eliminar la configuración existente?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        if (configuraciones.borrar(configuracion))
                        {
                            MessageBox.Show("Configuración eliminada con éxito.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
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
            foreach (Control ctrl in flowConfiguraciones.Controls)
            {
                if (string.IsNullOrEmpty((ctrl as Ctrl_PolizaConfig).txtBxFormula.Text))
                {
                    MessageBox.Show("El campo formula del control " + (ctrl as Ctrl_PolizaConfig).lblTipo.Text + " está vacío.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    guardar = false;
                }
            }
            if (guardar)
            {
                List<FormulaPolizaEntity> formulas = new List<FormulaPolizaEntity>();
                foreach (Control ctrl in flowConfiguraciones.Controls)
                {
                    Ctrl_PolizaConfig ctrlConf = (ctrl as Ctrl_PolizaConfig);
                    FormulaPolizaEntity formula = new FormulaPolizaEntity();
                    formula.Tipo = ctrlConf.lblTipo.Text.Contains("|") ? ctrlConf.lblTipo.Text.Split('|')[1] : ctrlConf.lblTipo.Text;
                    formula.TipoClave = ctrlConf.lblClaveTipo.Text;
                    formula.CargoAbono = ctrlConf.cmbBxCargoAbono.SelectedIndex == 0 ? 1 : 2;
                    formula.Formula = ctrlConf.txtBxFormula.Text;
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
                            this.Close();
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
                        this.Close();
                    }
                }
            }
        }

        private void mouseEntered(object sender, EventArgs e)
        {
            var boton = sender as Button;
            if (boton != null)
                boton.BackColor = Color.Orange;
        }

        private void mouseLeft(object sender, EventArgs e)
        {
            var boton = sender as Button;
            if (boton != null)
                boton.BackColor = this.BackColor;
        }
    }
}
