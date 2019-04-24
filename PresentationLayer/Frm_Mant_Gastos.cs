using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FastReport;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.PresentationLayer;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_Mant_Gastos : Form
    {

        string rutaformato = string.Empty;
        string usuario = string.Empty;
        string idInmobiliaria = string.Empty;
        private string formato = string.Empty, error = string.Empty;
        private bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;
        private MantenimientoGastos mantgastos = null;

       Inmobiliaria inmobiliaria = new Inmobiliaria();
        public Frm_Mant_Gastos(string rutaformato, string usuario)
        {
            InitializeComponent();
            SaariIcon.SaariIcon.setSaariIcon(this);
            this.rutaformato = rutaformato;
            this.usuario = usuario;

        }
       
        public void cargarInmobiliaria()
        {
            cmbBox_Inmobiliaria.DataSource = HelpInmobiliarias.obtenerInmobiliarias(usuario, permisosInmo);//MantenimientoGastos.obtenerInmobiliarias();             
            cmbBox_Inmobiliaria.DisplayMember = "RazonSocial";
            cmbBox_Inmobiliaria.ValueMember = "ID";
            cmbBox_Inmobiliaria.SelectedIndex = 0;
             
        }
               
        public void cargarUsuairos() {
            cmbBox_Usuario.DataSource= MantenimientoGastos.obtenerUsuarios();
           
        }

        public void cargarClasificaciones() 
        {
            cmbBox_Clasificacion.DataSource = MantenimientoGastos.obtenerClasificacion();
            cmbBox_Clasificacion.DisplayMember = "Nombre";           
        }

        public void Enablefalse() {
        cmbBox_Conjunto.Enabled = false;
        cmbBox_Usuario.Enabled = false;
        cmbBox_Clasificacion.Enabled = false;
        cmb_Inmueble.Enabled = false;        
        }

       private void cargarConjuntos(string selectValue)
        {
            try
            {
                DataTable dtConjuntos = inmobiliaria.getDtConjuntos(selectValue);
                if (dtConjuntos.Rows.Count > 0)
                {
                    DataRow row = dtConjuntos.NewRow();
                    row["P0301_ID_CENTRO"] = "Todos";
                    row["P0303_NOMBRE"] = "*Todos";
                    dtConjuntos.Rows.InsertAt(row, 0);
                    DataView view = new DataView(dtConjuntos);
                    view.Sort = "P0303_NOMBRE";
                    cmbBox_Conjunto.DataSource = view;
                    cmbBox_Conjunto.ValueMember = "P0301_ID_CENTRO";
                    cmbBox_Conjunto.DisplayMember = "P0303_NOMBRE";
                    cmbBox_Conjunto.SelectedIndex = 0;
                }
            }
            catch { 
            
                
            }

         }

       private void cargarInmuebles(string selectedValue)
       {
           DataTable dtInmuebles = inmobiliaria.getInmueblesCommand(selectedValue);
           if (dtInmuebles.Rows.Count > 0)
           {
               DataRow row = dtInmuebles.NewRow();
               row["P0701_ID_EDIFICIO"] = "Todos";
               row["P0703_NOMBRE"] = "*Todos";
               dtInmuebles.Rows.InsertAt(row, 0);
               DataView view = new DataView(dtInmuebles);
               view.Sort = "P0703_NOMBRE";
               cmb_Inmueble.DataSource = view;
               cmb_Inmueble.ValueMember = "P0701_ID_EDIFICIO";
               cmb_Inmueble.DisplayMember = "P0703_NOMBRE";
               cmb_Inmueble.SelectedIndex = 0;
           }
           else if (rb_allInmu.Checked)
           {
               cmb_Inmueble.DataSource = MantenimientoGastos.obtenerInmuebles();
               cmb_Inmueble.Text = "*Todos";
               cmb_Inmueble.DisplayMember = "Nombre";
               cmb_Inmueble.ValueMember = "ID";
               cmb_Inmueble.SelectedIndex = 0;

           }
       }

       private void Frm_Mant_Gastos_Load(object sender, EventArgs e)
        {
            
            cargarInmobiliaria();
            cargarConjuntos(cmbBox_Inmobiliaria.SelectedValue.ToString().Trim());
            if(cmbBox_Conjunto.Items.Count>0)
                cargarInmuebles(cmbBox_Conjunto.SelectedValue.ToString().Trim());
            cargarUsuairos();
            cargarClasificaciones();
            Enablefalse();
            cmbBox_Conjunto.Visible = true;
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int days = 1;
            dateTimePicker1.Value = new DateTime(year, month, days);
            days = DateTime.DaysInMonth(year, month);
            dateTimePicker2.Value = new DateTime(year, month, days);
           
        }

        private void rb_clasif_CheckedChanged(object sender, EventArgs e)
        {
            try{
                 cmbBox_Clasificacion.Enabled = !rb_allclasif.Checked;
                 cmbBox_Clasificacion.SelectedIndex = 0;
             }
            catch {
                rb_allclasif.Checked = true;
                MessageBox.Show("No se encontraron registros al cargar las clasificaciones, revise su catalogo de clasificaciones.", "Error Clasificaciones", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                
            }

         }

        private void rb_User_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                cmbBox_Usuario.Enabled = !rb_allUser.Checked;
                cmbBox_Usuario.SelectedIndex = 0;
            }
            catch
            {
                MessageBox.Show("No se encontraron registros al cargar las clasificaciones, revise su catalogo de Usuarios.", "Error Clasificaciones", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                rb_allUser.Checked = true;
            }
        }

        private void cmbBox_Inmobiliaria_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarConjuntos(cmbBox_Inmobiliaria.SelectedValue.ToString().Trim());

        }

        private void rb_Conjunto_CheckedChanged(object sender, EventArgs e)
        {
           cmbBox_Conjunto.Enabled = !rb_allConjuntos.Checked;
           cargarConjuntos(cmbBox_Inmobiliaria.SelectedValue.ToString().Trim());
           checkBox.Enabled = true;
                        
        }

        private void rb_Inmueble_CheckedChanged(object sender, EventArgs e)
        {
            cmb_Inmueble.Enabled = !rb_allInmu.Checked;
            if (cmbBox_Conjunto.Items.Count > 0)
                cargarInmuebles(cmbBox_Conjunto.SelectedValue.ToString().Trim());
        }

        private void cmbBox_Conjunto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbBox_Conjunto.Items.Count>0)
                cargarInmuebles(cmbBox_Conjunto.SelectedValue.ToString().Trim());
        }

       private void workerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
           error = mantgastos.generarRep();
        }

        private void workerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

       private  void mantenimiento_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        private void workerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(error))
            {
                MessageBox.Show("¡Reporte generado correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
            else
            {
                MessageBox.Show("No se pudo generar el reporte de mantenimiento: " + error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }
     

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            botonGenerar.Enabled = false;
            
            try
            {
                string IncludInmue = checkBox.Checked ? checkBox.Text : null;
                string clasif = rb_allclasif.Checked ? clasif = rb_allclasif.Text : cmbBox_Clasificacion.SelectedValue.ToString();
                string usuario = rb_allUser.Checked ? usuario = rb_allUser.Text : cmbBox_Usuario.SelectedValue.ToString();
                string conjunto = rb_allConjuntos.Checked ? conjunto = rb_allConjuntos.Text : cmbBox_Conjunto.SelectedValue.ToString();
                string inmueble = rb_allInmu.Checked ? inmueble = rb_allInmu.Text : cmb_Inmueble.SelectedValue.ToString();
                string idInmobiliaria = cmbBox_Inmobiliaria.SelectedValue.ToString();
                bool generarEx = checkBoxSoloGen.Checked;
                bool includInmue = checkBox.Checked;
                mantgastos = new MantenimientoGastos(cmbBox_Inmobiliaria.SelectedItem as InmobiliariaEntity, idInmobiliaria, clasif, usuario, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, rb_Formato.Checked, rutaformato, inmueble, conjunto, IncludInmue,generarEx);
                mantgastos.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(mantenimiento_CambioProgreso);
                workerReporte.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un problema al generar el reporte. Error: " + ex.Message + ". Comuniquese con el administrador del sistema.", "Reporte de Mantenimiento");
            }
        }

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                mantgastos.cancelar();
            botonCancelar.Enabled = false;
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void rb_allConjuntos_CheckedChanged(object sender, EventArgs e)
        {
            checkBox.Enabled = false;
            checkBox.Checked = false;
        }

        private void rb_Excel_Click(object sender, EventArgs e)
        {
            checkBoxSoloGen.Enabled = true;
        }

        private void rb_Formato_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Formato.Checked ==true) {
                checkBoxSoloGen.Enabled = false;
            }
        }

   }
}
