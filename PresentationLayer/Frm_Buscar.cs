using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_Buscar : Form
    {
        private string Descripcion { get; set; }
        public Frm_Buscar()
        {
            InitializeComponent();
            bckgWork.DoWork += new DoWorkEventHandler(bckgWork_RealizarBusqueda);
            bckgWork.ProgressChanged += new ProgressChangedEventHandler(bckgWork_ActualizaProgreso);
            bckgWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bckgWork_TareaCompletada);
            bckgWork.RunWorkerAsync();
        }

        void bckgWork_TareaCompletada(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                this.Visible = false;
                Frm_CarteraVencidaVta frm = new Frm_CarteraVencidaVta(string.Empty, "Administrador");
                frm.Show();
            }
            else
            {
                MessageBox.Show("Se ha cancelado la carga de datos", "Buscador Global", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(0);
            }
        }

        void bckgWork_ActualizaProgreso(object sender, ProgressChangedEventArgs e)
        {
            progBar.Value = e.ProgressPercentage;
            lblDescr.Text = Descripcion;
        }

        void bckgWork_RealizarBusqueda(object sender, DoWorkEventArgs e)
        {
            if (!bckgWork.CancellationPending)
            {
                System.Threading.Thread.Sleep(1000);
                bckgWork.ReportProgress(33);
                Descripcion = "Obteniendo datos...";
            }
            else
            {
                e.Cancel = true;
                return;
            }
            if (!bckgWork.CancellationPending)
            {
                System.Threading.Thread.Sleep(1000);
                bckgWork.ReportProgress(66);
                Descripcion = "Estableciendo datos en controles...";              
            }
            else
            {
                e.Cancel = true;
                return;
            }
            if (!bckgWork.CancellationPending)
            {
                System.Threading.Thread.Sleep(1000);
                bckgWork.ReportProgress(100);
            }
            else
            {
                e.Cancel = true;
                return;
            }
        }

        private void lblCancelar_MouseHover(object sender, EventArgs e)
        {
            Font negrita = new Font(lblCancelar.Font, FontStyle.Bold);
            lblCancelar.ForeColor = Color.White;            
            lblCancelar.Font = negrita;
        }

        private void lblCancelar_MouseLeave(object sender, EventArgs e)
        {
            Font normal = new Font(lblCancelar.Font, FontStyle.Regular);
            lblCancelar.ForeColor = Color.LightGray;
            lblCancelar.Font = normal;
        }

        private void lblCancelar_Click(object sender, EventArgs e)
        {
            bckgWork.CancelAsync();                
        }

        private void Frm_Buscar_Load(object sender, EventArgs e)
        {
           
        }
    }
}
