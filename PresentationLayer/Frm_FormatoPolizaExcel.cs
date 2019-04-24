using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesPolizas;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_FormatoPolizaExcel : Form
    {
        public List<ColumnasExcelEntity> Columnas { get; set; }
        public Frm_FormatoPolizaExcel()
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            Columnas = new List<ColumnasExcelEntity>();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (lstVwOpciones.SelectedItems.Count > 0)
            {
                ListViewItem item = lstVwOpciones.SelectedItems[0];
                Frm_CapturaTitulo captTitulo = new Frm_CapturaTitulo();
                captTitulo.ShowDialog();
                if(!string.IsNullOrEmpty(captTitulo.TituloColumna))
                {
                    lstVwOpciones.Items.Remove(item);
                    item.SubItems.Add(captTitulo.TituloColumna);
                    lstVwOpcSeleccionadas.Items.Add(item);
                }
            }
            selecionaPrimerItem();
        }

        private void selecionaPrimerItem()
        {
            if (lstVwOpcSeleccionadas.Items.Count > 0)
            {
                lstVwOpcSeleccionadas.Items[0].Selected = true;
                lstVwOpcSeleccionadas.Select();
            }
            if (lstVwOpciones.Items.Count > 0)
            {
                lstVwOpciones.Items[0].Selected = true;
                lstVwOpciones.Select();
            }
        }

        private void Frm_FormatoPolizaExcel_Load(object sender, EventArgs e)
        {
            selecionaPrimerItem();
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (lstVwOpcSeleccionadas.SelectedItems.Count > 0)
            {
                ListViewItem item = lstVwOpcSeleccionadas.SelectedItems[0];
                item.SubItems.Remove(item.SubItems[1]);
                lstVwOpcSeleccionadas.Items.Remove(item);
                lstVwOpciones.Items.Add(item);
            }
            selecionaPrimerItem();
        }

        private void lstVwOpciones_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = lstVwOpciones.GetItemAt(e.X, e.Y);
            Frm_CapturaTitulo captTitulo = new Frm_CapturaTitulo();
            captTitulo.ShowDialog();
            if (!string.IsNullOrEmpty(captTitulo.TituloColumna))
            {
                if (item != null)
                {
                    lstVwOpciones.Items.Remove(item);
                    item.SubItems.Add(captTitulo.TituloColumna);
                    lstVwOpcSeleccionadas.Items.Add(item);
                    selecionaPrimerItem();
                }
            }
        }

        private void lstVwOpcSeleccionadas_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = lstVwOpcSeleccionadas.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                item.SubItems.Remove(item.SubItems[1]);
                lstVwOpcSeleccionadas.Items.Remove(item);
                lstVwOpciones.Items.Add(item);
                selecionaPrimerItem();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Columnas = new List<ColumnasExcelEntity>();
            this.Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (lstVwOpcSeleccionadas.Items.Count <= 0)
            {
                MessageBox.Show("Debe seleccionar al menos una columna para continuar.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    foreach (ListViewItem item in lstVwOpcSeleccionadas.Items)
                    {
                        ColumnasExcelEntity columna = new ColumnasExcelEntity();
                        columna.Identificador = item.SubItems[0].Text;
                        columna.Encabezado = item.SubItems[1].Text;
                        Columnas.Add(columna);
                    }
                    this.Close();
                }
                catch
                {
                    Columnas = new List<ColumnasExcelEntity>();
                }
            }
        }
    }
}
