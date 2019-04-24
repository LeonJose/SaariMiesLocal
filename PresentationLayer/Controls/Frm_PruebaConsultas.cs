using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.EntitiesReportes;

namespace GestorReportes.PresentationLayer.Controls
{
    public partial class Frm_PruebaConsultas : Form
    {
        public Frm_PruebaConsultas()
        {
            InitializeComponent();
        }

        public Frm_PruebaConsultas(List<ProvisionEntity> lista):this()
        {
            asignarDatos(lista);
        }

        private void Frm_PruebaConsultas_Load(object sender, EventArgs e)
        {
            //gridDatos = new DataGridView();
        }

        public void asignarDatos(List<ProvisionEntity> lista)
        {
            try
            {
                int i = 0;
                //gridDatos = new DataGridView();
                gridDatos.AutoGenerateColumns = false;
                foreach (ProvisionEntity provision in lista)
                {                                        
                     gridDatos.Rows.Add(provision.IDHistRec, provision.ConceptoGasto, provision.FechaGasto, provision.ImporteGasto,
                        provision.IvaGasto, provision.TotalCheque, provision.EstatusCXP, provision.EstatusCheque, provision.NumeroCheque,
                        provision.FechaGeneracionCheque, provision.FechaImpresionCheque, provision.IDProveedor,
                        provision.NombreProveedor, provision.Moneda, provision.TipoCambio);                     
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
