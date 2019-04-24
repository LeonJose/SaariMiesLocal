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
using FastReport;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_RepBitacora : Form
    {
        private string formato = string.Empty, error = string.Empty;
        private Bitacora BitacoraBL = null;
        public List<ClienteEntity> listaClientes = Bitacora.obtenerClientesProspectos();
        public List<string> listaUsuarios = Bitacora.obtenerUsuarios();
        public List<string> listaEtapas = Bitacora.obtnerEtapasCRM();

        public Frm_RepBitacora()
        {
            InitializeComponent();
        }

        public Frm_RepBitacora(string rutaRepBitacora)
        {
            InitializeComponent();
            //SaariIcon.SaariIcon.setSaariIcon(this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            formato = rutaRepBitacora;
            if (!System.IO.File.Exists(rutaRepBitacora))
            {
                MessageBox.Show("No existe el formato de plantilla en ubicación: \"" + rutaRepBitacora + ". Comuniquese con el administrador del sistema.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

        }

        private void Frm_RepBitacora_Load(object sender, EventArgs e)
        {
            //radioExcel.Visible = false;
            dateTimePicker1.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            Cmb_Clientes.Visible = true;
            Cmb_Clientes.Enabled = false;
            cargarClientes();                                    
            Cmb_Usuarios.Enabled = false;
            cargarUsuarios();
           
            Cmb_Etapa.Enabled = false;
            cargarEtapas();
        }

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonCancelar.Enabled)
                this.Close();
            else
                BitacoraBL.cancelar();
        }

        public void cargarClientes()
        {
            if(listaClientes!=null)
            {
                Cmb_Clientes.DataSource = listaClientes;
                Cmb_Clientes.ValueMember = "IDCliente";
                Cmb_Clientes.DisplayMember = "Nombre";
            }
            else
            {
                MessageBox.Show("Ocurrió un problema, no se pudieron cargar los datos de los clientes/prospectos.", "Saari Reporte de Bitácora", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void cargarUsuarios()
        {
            if (listaUsuarios != null)
            {
                listaUsuarios.Add("Notificaciones");
                Cmb_Usuarios.DataSource = listaUsuarios;
            }
            else
                MessageBox.Show("Ocurrió un problema, no se pudieron cargar los datos de los usuarios.", "Saari Reporte de Bitácora", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void cargarEtapas()
        {
            if (listaEtapas != null)
                Cmb_Etapa.DataSource = listaEtapas;
            else
                MessageBox.Show("Ocurrió un problema, no se pudieron cargar los datos de las etapas de seguimiento.", "Saari Reporte de Bitácora", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Rb_Todos_CheckedChanged(object sender, EventArgs e)
        {
            Cmb_Clientes.Enabled = !Rb_Todos.Checked;
        }

        private void Rb_AllUser_CheckedChanged(object sender, EventArgs e)
        {
            Cmb_Usuarios.Enabled = Rb_Usuario.Checked;            
        }

       
        private void Rb_AllEtapa_CheckedChanged(object sender, EventArgs e)
        {
            Cmb_Etapa.Enabled = Rb_Etapa.Checked;
        }

        private void botonGenerar_ControlClicked(object sender, EventArgs e)
        {
            //if (rdBtnCalendar.Checked)
            //    formato = formato.Replace("Bitacora.frx", "CalendarioBitacora.frx");
            //else
            //    formato=formato.Replace("CalendarioBitacora.frx", "Bitacora.frx");
            //formato = "Bitacora.frx";
            if (!System.IO.File.Exists(formato))
            {
                MessageBox.Show("No existe el formato de plantilla en ubicación: \"" + formato + ". Comuniquese con el administrador del sistema.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);               
                return;
            }
            
            botonGenerar.Enabled = false;
            string clienteProspecto = string.Empty;
            if (Rb_ClasifClientes.Checked)
                clienteProspecto = "Clientes";
            else if (rb_ClasifProspectos.Checked)
                clienteProspecto = "Prospectos";

            BitacoraBL= new Bitacora(clienteProspecto, !Rb_Todos.Checked ? Cmb_Clientes.SelectedItem as ClienteEntity:null, Rb_AllUser.Checked ? string.Empty : Cmb_Usuarios.SelectedItem.ToString(),
                "", Rb_AllEtapa.Checked?string.Empty:Cmb_Etapa.SelectedItem.ToString(), dateTimePicker1.Value.Date, 
                dateTimePicker2.Value.Date, false, radioPDF.Checked, formato);

            BitacoraBL.CambioProgreso+= new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(bitacora_CambioProgreso);
            workerReporte.RunWorkerAsync();

        }

        void bitacora_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        private void workerReporte_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                error = BitacoraBL.generar();
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
            if(string.IsNullOrWhiteSpace(error))
                 MessageBox.Show("¡Reporte de bitácora de seguimiento generado correctamente!", "Bitácora de Seguimiento CRM", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo generar el reporte de bitácora de seguimiento: " + error, "Bitácora de Seguimiento CRM", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void Frm_RepBitacora_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

        private void Rb_ClasifClientes_CheckedChanged(object sender, EventArgs e)
        {
            if (Rb_ClasifClientes.Checked)
            {
                List<ClienteEntity> listaClientesFiltrada = listaClientes.Where(cte=>cte.TipoEnte==2).ToList();
                Cmb_Clientes.DataSource = null;
                Cmb_Clientes.DataSource = listaClientesFiltrada;
                Cmb_Clientes.ValueMember = "IDCliente";
                Cmb_Clientes.DisplayMember = "Nombre";
            }
        }

        private void rb_ClasifProspectos_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_ClasifProspectos.Checked)
            {
                List<ClienteEntity> listaProspectosFiltrada = listaClientes.Where(cte => cte.TipoEnte == 22).ToList();
                Cmb_Clientes.DataSource = null;
                Cmb_Clientes.DataSource = listaProspectosFiltrada;
                Cmb_Clientes.ValueMember = "IDCliente";
                Cmb_Clientes.DisplayMember = "Nombre";
            }
        }        

        private void Rb_AllClasific_CheckedChanged(object sender, EventArgs e)
        {
            cargarClientes();
        }


    }
}
