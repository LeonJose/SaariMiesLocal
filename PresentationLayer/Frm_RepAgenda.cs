using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using FastReport;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_RepAgenda : Form
    {
        private string formato = string.Empty, error = string.Empty;
        private Agenda agenda = null;

        public Frm_RepAgenda(string rutarepagenda)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            formato = rutarepagenda;            
            if (!System.IO.File.Exists(rutarepagenda))
            {
                MessageBox.Show("No existe el formato de plantilla en ubicación: \"" + rutarepagenda + ". Comuniquese con el administrador del sistema.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            if (!rutarepagenda.Contains(".fr3"))
            {
                MessageBox.Show("Versión de plantilla no soportada. Este gestor sólo soporta plantillas en formato \".fr\"." + ". Comuniquese con el administrador del sistema.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
        
        public void cargarclientes()
        {
            comboBox1.DataSource = Agenda.obtenerClientes();
            comboBox1.ValueMember = "IDCliente";
            comboBox1.DisplayMember = "Nombre";
        }                

        private void Btn_Cancelar_Click(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                agenda.cancelar();
            botonCancelar.Enabled = false;
        }               

        private void Frm_RepAgenda_Load_1(object sender, EventArgs e)
        {
            radioExcel.Visible = false;
            cargarclientes();
            comboBox1.Enabled = false;
            Cmb_Usuarios.Enabled = false;
            Cmb_Stat.Enabled = false;

        }

        private void Rb_Todos_CheckedChanged_1(object sender, EventArgs e)
        {
            comboBox1.Enabled = !Rb_Todos.Checked;            
        }

        private void Rb_AllUser_CheckedChanged(object sender, EventArgs e)
        {
            if (Rb_AllUser.Checked)
                Cmb_Usuarios.Enabled = false;
            else
            {
                Cmb_Usuarios.Enabled = true;
                Cargar_Usuarios();
            }
        }
        
        private void Cargar_Usuarios()
        {            
            Cmb_Usuarios.DataSource = Agenda.obtenerUsuarios();            
        }
               
        private void Rb_Allst_CheckedChanged(object sender, EventArgs e)
        {        
            Cmb_Stat.Enabled = !Rb_Allst.Checked;
            Cmb_Stat.SelectedIndex=0;
        }

        private void rdBtnCalendar_CheckedChanged(object sender, EventArgs e)
        {
            label2.Visible = !rdBtnCalendar.Checked;
            dateTimePicker2.Visible = !rdBtnCalendar.Checked;
            label1.Text = rdBtnCalendar.Checked ? "Fecha mes:" : "Fecha Inicial:";
        }

        private void workerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                error = agenda.generar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el reporte. Error: " + ex.Message, "Generar reporte");
            }
        }

        private void workerReporte_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void workerReporte_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(error))
                MessageBox.Show("¡Agenda generada correctamente!", "Agenda", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo generar la agenda: " + error, "Agenda", MessageBoxButtons.OK, MessageBoxIcon.Information);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
            //agenda = null;
            //progreso.Refresh();
            //workerReporte.Dispose();
        }

        private void Frm_RepAgenda_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            if (rdBtnCalendar.Checked)
                formato = formato.Replace("Agenda.fr3", "Calendario.fr3");
            else
                formato = formato.Replace("Calendario.fr3", "Agenda.fr3");
            if (!System.IO.File.Exists(formato))
            {
                MessageBox.Show("No existe el formato de plantilla en ubicación: \"" + formato +  ". Comuniquese con el administrador del sistema.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //this.Close();
                return;
            }
            if (!Rb_Allst.Checked && Cmb_Stat.SelectedIndex==-1)
            {
                MessageBox.Show("Debe elegir la opción de estatus, debe seleccionar un tipo de estatus del listado.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            botonGenerar.Enabled = false;
            //string status = Cmb_Stat.SelectedItem.ToString();            
            //agenda = new Agenda(comboBox1.SelectedItem as ClienteEntity, Rb_AllUser.Checked ? string.Empty : Cmb_Usuarios.SelectedText,
            //    Rb_Allst.Checked ? string.Empty : Cmb_Stat.SelectedText, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, rdBtnCalendar.Checked, radioPDF.Checked, formato);
            agenda = new Agenda(!Rb_Todos.Checked ? comboBox1.SelectedItem as ClienteEntity:null, Rb_AllUser.Checked ? string.Empty : Cmb_Usuarios.SelectedItem.ToString(),
                Rb_Allst.Checked ? string.Empty : Cmb_Stat.SelectedItem.ToString(), dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, rdBtnCalendar.Checked, radioPDF.Checked, formato);

            agenda.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(agenda_CambioProgreso);
            workerReporte.RunWorkerAsync();
        }

        void agenda_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }
    }
}
