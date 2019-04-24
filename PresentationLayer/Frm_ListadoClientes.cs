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
    public partial class Frm_ListadoClientes : Form
    {
        private string rutaFormato = string.Empty;
        public Frm_ListadoClientes(string rutaFormato, string user)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaFormato = rutaFormato;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ListadoClientes listado = new ListadoClientes();
            string result = listado.generarListado(rutaFormato, radioPdf.Checked);
            if (string.IsNullOrEmpty(result))
                MessageBox.Show("¡Reporte generado correctamente!", "Listado de clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Ocurrió un error al generar el listado de clientes: " + Environment.NewLine + result, "Listado de clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
