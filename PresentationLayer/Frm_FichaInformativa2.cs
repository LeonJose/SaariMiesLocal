using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_FichaInformativa2 : Form
    {
        FichaInformativa _fichaInformativa = new FichaInformativa();
        public Frm_FichaInformativa2(FichaInformativa fichaInformativa)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, thi
            SaariIcon.SaariIcon.setSaariIcon(this);
            _fichaInformativa = fichaInformativa;
        }

        private void Frm_FichaInformativa2_Load(object sender, EventArgs e)
        {
            botonSiguiente.Enabled = false;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Frm_FichaInformativa frm = new Frm_FichaInformativa(_fichaInformativa.Usuario);
            frm.Show();
        }

        private void Frm_FichaInformativa2_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            string[] datos = new string[chckLstBxDatos.CheckedItems.Count];
            int cont = 0;
            foreach(var x in chckLstBxDatos.CheckedItems)
            {
                datos[cont] = x.ToString();
                cont++;
              
            }
            
            _fichaInformativa.Datos = datos;

            this.Visible = false;
            Frm_FichaInformativa3 frm = new Frm_FichaInformativa3(_fichaInformativa);
            frm.Show();
        }

        private void chckLstBxDatos_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            
        }

        private void chckLstBxDatos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chckLstBxDatos.CheckedItems.Count >= 1)
                botonSiguiente.Enabled = true;
            else
                botonSiguiente.Enabled = false;
        }

        private void chckBxSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chckBxSelectAll.Checked)
            {
                botonSiguiente.Enabled = true;
                for (int i = 0; i < chckLstBxDatos.Items.Count; i++)
                {
                    chckLstBxDatos.SetItemChecked(i, true);
                }
            }
            else
            {
                botonSiguiente.Enabled = false;
                for (int i = 0; i < chckLstBxDatos.Items.Count; i++)
                {
                    chckLstBxDatos.SetItemChecked(i, false);
                }
            }
        }
    }
}
