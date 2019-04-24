using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesPolizas;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_GenerarPolizaEgreso : Form
    {
        private string user = string.Empty;
        private bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;

        public Frm_GenerarPolizaEgreso(string usuario)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.user = usuario;
        }
        private void btnCancelar_Click(object sender, EventArgs e)
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
                //iDinmobiliaria = comboInmobiliaria.SelectedValue.ToString();
               // NombreInmo = comboInmobiliaria.Text.ToString();
            }
            catch
            {
                //result = "Error al obtener las inmobiliarias.";
            }
        }
        private void Frm_GenerarPolizaEgreso_Load(object sender, EventArgs e)
        {
            try
            {
                cargarInmobiliarias();
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
            }
            catch
            {
            }
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
        private void botonGenerar_Click(object sender, EventArgs e)
        {            
            CondicionesEntity condiciones = new CondicionesEntity();
            condiciones.Inmobiliaria = comboInmobiliaria.SelectedValue.ToString();
            if (rdBtnContpaq.Checked)
            {
                condiciones.Formato = FormatoExportacionPoliza.ContpaqTxt;
                if (textoMascara.Enabled)
                {
                    condiciones.Mascara = textoMascara.Text;
                    Properties.Settings.Default.ContpaqMask = textoMascara.Text;
                    Properties.Settings.Default.Save();
                }
                condiciones.MultiplesEncabezados = checkMultiples.Checked;
                condiciones.IncluirSegmento = checkSegmento.Checked;
            }
            else
            {
                condiciones.Formato = FormatoExportacionPoliza.ContpaqXls;
                if (textoMascara.Enabled)
                {
                    condiciones.Mascara = textoMascara.Text;
                    Properties.Settings.Default.ContpaqMask = textoMascara.Text;
                    Properties.Settings.Default.Save();
                }
                condiciones.MultiplesEncabezados = checkMultiples.Checked;
                condiciones.IncluirSegmento = checkSegmento.Checked;
            }
            condiciones.NumeroPoliza = txtBxIdenPol.Text.Trim();
            condiciones.ConceptoPoliza = txtBxConceptoPoliza.Text.Trim();
            condiciones.FechaInicio = dateTimePicker1.Value.Date;
            condiciones.FechaFin = dateTimePicker2.Value.Date;            
            condiciones.TipoPresentacion = 2;
            condiciones.AfectarSaldos = checkAfectar.Checked;
            condiciones.TipoPoliza = radioEgreso.Checked ? 1 : 2;

            Poliza poliza = new Poliza(condiciones);
            string resultGenerar = poliza.generar();                
            if (string.IsNullOrEmpty(resultGenerar))
            {
                MessageBox.Show("¡Póliza " + poliza.NombreArchivo + " generada correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No se pudo generar la póliza solicitada." + Environment.NewLine + resultGenerar, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //this.Close();
            
        }
    }
}
