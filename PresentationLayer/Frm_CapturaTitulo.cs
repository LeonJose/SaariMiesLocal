using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_CapturaTitulo : Form
    {
        public string TituloColumna { get; set; }
        public Frm_CapturaTitulo()
        {
            InitializeComponent();
            //System.Drawing.Icon icono = new System.Drawing.Icon("IconoSAARI.ico");
            SaariIcon.SaariIcon.setSaariIcon(this);
            //this.Icon = icono;  
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {            
            if (string.IsNullOrEmpty(txtBxTitulo1.Text))
                MessageBox.Show("Debe seleccionar un titulo", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                TituloColumna = txtBxTitulo1.Text;            
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            TituloColumna = string.Empty;
            this.Close();
        }

        private void Frm_CapturaTitulo_Load(object sender, EventArgs e)
        {
        }
    }
}
