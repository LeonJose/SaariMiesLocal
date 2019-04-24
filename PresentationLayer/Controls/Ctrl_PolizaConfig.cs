using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GestorReportes.PresentationLayer.Controls
{
    public partial class Ctrl_PolizaConfig : UserControl
    {
        public string tipoPolizaConfig = string.Empty;
        public string monedaCobro = string.Empty;

        public Ctrl_PolizaConfig()
        {
            InitializeComponent();
        }

        public Ctrl_PolizaConfig(bool esEgreso)
            : this()
        {
            cmbBxCargoAbono.Left += 50;
        }

        private void Ctrl_PolizaConfig_Load(object sender, EventArgs e)
        {
            cmbBxCargoAbono.SelectedIndex = 0;
            if (tipoPolizaConfig == "INGRESO")
            {
                lblMonCob.Visible = true;
                lblValMonCobro.Visible = true;
            }
            else
            {
                lblMonCob.Visible = false;
                lblValMonCobro.Visible = false;
            }            
        }

        private void btnEliminar_MouseEnter(object sender, EventArgs e)
        {
            btnEliminar.BackColor = Color.Orange;
        }

        private void btnEliminar_MouseLeave(object sender, EventArgs e)
        {
            btnEliminar.BackColor = this.BackColor;
        }
        private bool backPressed = false;

        private void txtBxFormula_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
                backPressed = true;
            else
                backPressed = false;
        }

        private void txtBxFormula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!backPressed)
                e.Handled = true;
        }
    }
}
