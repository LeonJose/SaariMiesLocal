using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_AmortizacionRentas : Form
    {
        public Frm_AmortizacionRentas(string rutaReporte, string usuario)
        {
            InitializeComponent();
            SaariIcon.SaariIcon.setSaariIcon(this);
        }

        private void Frm_AmortizacionRentas_Load(object sender, EventArgs e)
        {
            this.dateTimePickerMes.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }
    }
}
