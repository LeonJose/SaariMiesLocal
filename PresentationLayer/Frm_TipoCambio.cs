using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_TipoCambio : Form
    {
        private bool impedirCierre = true;
        private decimal tipoCambio = 0;

        public decimal TipoCambio { get { return tipoCambio; } }

        public Frm_TipoCambio()
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
        }

        private void textoTipoCambio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '.' && !Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void botonAceptar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textoTipoCambio.Text))
            {
                try
                {
                    tipoCambio = Convert.ToDecimal(textoTipoCambio.Text.Trim());
                    if (tipoCambio > 0)
                    {
                        impedirCierre = false;
                        this.Close();
                    }
                }
                catch 
                {  }
            }
        }

        private void Frm_TipoCambio_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (impedirCierre)
                e.Cancel = true;
        }

        private void Frm_TipoCambio_Load(object sender, EventArgs e)
        {
            textoTipoCambio.Focus();
        }
    }
}
