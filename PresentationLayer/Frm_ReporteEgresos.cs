using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Helpers;


namespace GestorReportes.PresentationLayer
{
    public partial class Frm_ReporteEgresos : Form
    {
        private string rutaFormato = string.Empty, usuario = string.Empty, error = string.Empty;
        private ReporteEgreso reporte = null;
        private bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;

        public Frm_ReporteEgresos(string rutaFormato, string usuario)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
        }

        private void Frm_ReporteEgresos_Load(object sender, EventArgs e)
        {
            dateInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateFin.Value = dateInicio.Value.AddMonths(1).AddDays(-1);

            comboInmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuario,permisosInmo);//ReporteEgreso.obtenerInmobiliarias();
            comboInmobiliaria.ValueMember = "ID";
            comboInmobiliaria.DisplayMember = "RazonSocial";

            comboClasificacion.DataSource = ReporteEgreso.obtenerClasificaciones();
        }

        private void comboInmobiliaria_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                comboCuentas.DataSource = ReporteEgreso.obtenerCuentas((comboInmobiliaria.SelectedItem as InmobiliariaEntity).ID);
                comboCuentas.ValueMember = "ID";
                comboCuentas.DisplayMember = "Descripcion";
            }
            catch { }
        }

        private void comboCuentas_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                grupoMoneda.Visible = (comboCuentas.SelectedItem as CuentaBancariaEntity).ID == "*Todas";
            }
            catch { }
        }

        private void botonCancelar_Click(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                reporte.cancelar();
            botonCancelar.Enabled = false;
        }

        private void botonGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                progreso.Value = 0;
                InmobiliariaEntity inmobiliaria = comboInmobiliaria.SelectedItem as InmobiliariaEntity;
                CuentaBancariaEntity cuenta = comboCuentas.SelectedItem as CuentaBancariaEntity;
                string clasificacion = comboClasificacion.SelectedItem.ToString();
                Orden orden = Orden.Ninguno;
                if (radioFecha.Checked)
                    orden = Orden.Fecha;
                else if (radioClasificacion.Checked)
                    orden = Orden.Clasificacion;
                else if (radioEstatus.Checked)
                    orden = Orden.Estatus;
                else if (radioBeneficiario.Checked)
                    orden = Orden.Beneficiario;

                botonGenerar.Enabled = false;
                reporte = new ReporteEgreso(inmobiliaria, dateInicio.Value.Date, dateFin.Value.Date, cuenta, clasificacion,
                    radioPesos.Checked ? "P" : "D", orden, rutaFormato, usuario, radioPDF.Checked, checkCancelados.Checked);
                reporte.CambioProgreso += new EventHandler<CambioProgresoEventArgs>(reporte_CambioProgreso);
                workerReporte.RunWorkerAsync();                
            }
            catch { }
        }

        void reporte_CambioProgreso(object sender, CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        private void workerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
            error = reporte.generar();            
        }

        private void workerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void workerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(error))
                MessageBox.Show("¡Reporte generado correctamente!", "Reporte de egresos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo generar el reporte de egresos: " + Environment.NewLine + error, "Reporte de egresos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void Frm_ReporteEgresos_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }
    }
}
