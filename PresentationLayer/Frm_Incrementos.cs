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
    public partial class Frm_Incrementos : Form
    {
        private Inmobiliaria inmobiliaria = new Inmobiliaria();
        private Incrementos reporte = null;
        private string usuario = string.Empty, error = string.Empty;
        private bool permisos = Properties.Settings.Default.PermisosInmobiliaria;
        public Frm_Incrementos(string user)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            //incrementos.Usuario = user;
            this.usuario = user;
        }

        private void Frm_Incrementos_Load(object sender, EventArgs e)
        {
            txtBxTipoCambio.Text = Incrementos.getTipoDeCambio(DateTime.Now);
            cargarCombos();
            //cmbBxClient.Enabled = false;
            cmbBxConj.Enabled = false;
            cmbBxInmob.Enabled = false;
            cmbBxSubConj.Enabled = false;
        }

        private void Frm_Incrementos_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!botonCancelar.Enabled || !botonGenerar.Enabled)
                e.Cancel = true;
        }

        private void cargarCombos()
        {
            DataTable dtGpoEmp = inmobiliaria.getGposCommand();
            if (dtGpoEmp.Rows.Count > 0)
            {
                DataRow row = dtGpoEmp.NewRow();
                row["P0001_ID_GRUPO"] = "Todos";
                row["P0002_NOMBRE"] = "*Todos";
                dtGpoEmp.Rows.InsertAt(row, 0);
                DataView view = new DataView(dtGpoEmp);
                view.Sort = "P0002_NOMBRE";
                cmbBxGpoEmp.DataSource = view;
                cmbBxGpoEmp.DisplayMember = "P0002_NOMBRE";
                cmbBxGpoEmp.ValueMember = "P0001_ID_GRUPO";
                cmbBxGpoEmp.SelectedIndex = 0;
                cargarComboClientes();
            }
            else
            {
                MessageBox.Show("No se encontró ningun grupo empresarial registrado. \n - Debe tener grupos empresariales registrados. \n - Revise su conexión a la base de datos.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void cmbBxGpoEmp_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dtInmob = new DataTable();
            if (cmbBxGpoEmp.SelectedIndex <= 0)
            {
                cmbBxInmob.Enabled = false;
                cmbBxInmob.SelectedIndex = -1;
                //dtInmob = inmobiliaria.getDtInmobiliarias();
            }
            else
            {
                cmbBxInmob.Enabled = true;
                dtInmob = inmobiliaria.getInmobiliariasCommand(cmbBxGpoEmp.SelectedValue.ToString().Trim());
                if (dtInmob.Rows.Count > 0)
                {
                    DataRow row = dtInmob.NewRow();
                    row["P0101_ID_ARR"] = "Todos";
                    row["P0102_N_COMERCIAL"] = "*Todos";
                    row["P0103_RAZON_SOCIAL"] = "Todos";
                    dtInmob.Rows.InsertAt(row, 0);
                    DataView view = new DataView(dtInmob);
                    view.Sort = "P0102_N_COMERCIAL";
                    cmbBxInmob.DataSource = view; // 3 lineas agregadas para ordenar inmobiliarias 
                    cmbBxInmob.DisplayMember = "P0102_N_COMERCIAL";
                    cmbBxInmob.ValueMember = "P0101_ID_ARR";
                    cmbBxInmob.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("No se encontró ninguna inmobiliaria registrada. \n - Debe tener inmobiliarias registradas. \n - Revise su conexión a la base de datos.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            cargarComboClientes();
        }

        private void cmbBxInmob_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dtConju = new DataTable();
            if ( cmbBxInmob.SelectedIndex  <= 0)
            {
                cmbBxConj.Enabled = false;
                cmbBxConj.SelectedIndex = -1;
                //dtConju = inmobiliaria.getDtAllConjuntos();
            }
            else
            {
                dtConju = inmobiliaria.getDtConjuntos(cmbBxInmob.SelectedValue.ToString().Trim());
                if (dtConju.Rows.Count > 0)
                {
                    cmbBxConj.Enabled = true;
                    DataRow row = dtConju.NewRow();
                    row["P0301_ID_CENTRO"] = "Todos";
                    row["P0303_NOMBRE"] = "*Todos";
                    dtConju.Rows.InsertAt(row, 0);
                    DataView view = new DataView(dtConju);
                    view.Sort = "P0303_NOMBRE";
                    cmbBxConj.DataSource = view; // 3 lineas modificadas para ordenar conjuntos
                    cmbBxConj.ValueMember = "P0301_ID_CENTRO";
                    cmbBxConj.DisplayMember = "P0303_NOMBRE";
                    cmbBxConj.SelectedIndex = 0;
                }
                else
                {
                    cmbBxConj.Enabled = false;
                    cmbBxConj.SelectedIndex = -1;
                }
            }
            cargarComboClientes();
        }

        private void cmbBxConj_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dtSubConju = new DataTable();
            if (cmbBxConj.SelectedIndex <= 0)
            {
                cmbBxSubConj.Enabled = false;
                cmbBxSubConj.SelectedIndex = -1;
            }
            else
            {
                cmbBxSubConj.Enabled = true;
                dtSubConju = inmobiliaria.getDtSubConjuntos(cmbBxConj.SelectedValue.ToString().Trim());
                if (dtSubConju.Rows.Count > 0)
                {
                     DataRow row = dtSubConju.NewRow();
                     row["P1801_ID_SUBCONJUNTO"] = "Todos";
                     row["P1803_NOMBRE"] = "*Todos";
                     dtSubConju.Rows.InsertAt(row, 0);
                     DataView view = new DataView(dtSubConju);
                     view.Sort = "P1803_NOMBRE";
                     cmbBxSubConj.DataSource = view; // 3 lineas modificadas para ordenar conjuntos
                     cmbBxSubConj.ValueMember = "P1801_ID_SUBCONJUNTO";
                     cmbBxSubConj.DisplayMember = "P1803_NOMBRE";
                     cmbBxSubConj.SelectedIndex = 0;
                }
                else
                {
                    cmbBxSubConj.Enabled = false;
                    cmbBxSubConj.SelectedIndex = -1;
                }
            }
            cargarComboClientes();
        }

        private void cargarComboClientes()
        {
            try
            {
                if (cmbBxSubConj.SelectedIndex > 0)
                {
                    cmbBxClient.Enabled = false;
                    cmbBxClient.SelectedIndex = -1;
                }
                else
                {
                    cmbBxClient.Enabled = true;
                    DataTable dtClientes = new DataTable();
                    if (cmbBxGpoEmp.SelectedIndex == 0)
                    {
                        dtClientes = inmobiliaria.getDtAllClientes();
                    }
                    else if (cmbBxGpoEmp.SelectedIndex > 0 && cmbBxInmob.SelectedIndex == 0)
                    {
                        dtClientes = inmobiliaria.getDtClientesPorGpoEmp(cmbBxGpoEmp.SelectedValue.ToString());
                    }
                    else if (cmbBxGpoEmp.SelectedIndex > 0 && cmbBxInmob.SelectedIndex > 0 && cmbBxConj.SelectedIndex == 0)
                    {
                        dtClientes = inmobiliaria.getDtClientesPorInm(cmbBxInmob.SelectedValue.ToString());
                    }
                    else if (cmbBxGpoEmp.SelectedIndex > 0 && cmbBxInmob.SelectedIndex > 0 && cmbBxConj.SelectedIndex > 0) //&& cmbBxSubConj.SelectedIndex == 0)
                    {
                        dtClientes = inmobiliaria.getDtClientesPorInmYConj(cmbBxInmob.SelectedValue.ToString(), cmbBxConj.SelectedValue.ToString());
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ningun cliente registrado de acuerdo a los criterios seleccionados. \n - Favor de intentar con otros criterios.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cmbBxGpoEmp.SelectedIndex = 0;
                    }
                    if (dtClientes.Rows.Count > 0)
                    {
                        string[] campos = { "P0201_ID", "P0203_NOMBRE" };
                        DataTable dtAux = dtClientes.DefaultView.ToTable(true, campos);
                        DataRow row = dtAux.NewRow();
                        row["P0201_ID"] = "Todos";
                        row["P0203_NOMBRE"] = "*Todos";
                        dtAux.Rows.InsertAt(row, 0);
                        DataView view = new DataView(dtAux);
                        view.Sort = "P0203_NOMBRE";
                        cmbBxClient.DataSource = view;
                        cmbBxClient.ValueMember = "P0201_ID";
                        cmbBxClient.DisplayMember = "P0203_NOMBRE";
                        cmbBxClient.SelectedIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ningun cliente registrado de acuerdo a los criterios seleccionados. \n - Favor de intentar con otros criterios.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cmbBxGpoEmp.SelectedIndex = 0;
                    }
                }
            }
            catch 
            {
                MessageBox.Show("Hubo un error inesperado al momento de obtener los clientes de los contratos para las condiciones seleccionadas. \n - Favor de intentar con otros criterios.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbBxGpoEmp.SelectedIndex = 0;
            }
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            if (tipoCambioValido())
            {
                progreso.Value = 0;
                botonGenerar.Enabled = false;
                reporte = new Incrementos();
                reporte.TipoDeCambio = Convert.ToDecimal(txtBxTipoCambio.Text);
                if (cmbBxGpoEmp.SelectedIndex <= 0)
                    reporte.GrupoEmpresarial = "Todos";
                else
                    reporte.GrupoEmpresarial = cmbBxGpoEmp.SelectedValue.ToString();
                if (cmbBxInmob.SelectedIndex <= 0)
                    reporte.Inmobiliaria = "Todos";
                else
                    reporte.Inmobiliaria = cmbBxInmob.SelectedValue.ToString();
                if (cmbBxConj.SelectedIndex <= 0)
                    reporte.Conjunto = "Todos";
                else
                    reporte.Conjunto = cmbBxConj.SelectedValue.ToString();
                if (cmbBxSubConj.SelectedIndex <= 0)
                    reporte.SubConjunto = "Todos";
                else
                    reporte.SubConjunto = cmbBxSubConj.SelectedValue.ToString();
                if (cmbBxClient.SelectedIndex <= 0)
                    reporte.Cliente = "Todos";
                else
                    reporte.Cliente = cmbBxClient.SelectedValue.ToString();
                reporte.Usuario = this.usuario;
                reporte.CambioProgreso += new EventHandler<BusinessLayer.Helpers.CambioProgresoEventArgs>(reporte_CambioProgreso);
                workerReporte.RunWorkerAsync(); 
            }
            else
            {
                MessageBox.Show("El valor contenido en el tipo de cambio es incorrecto!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtBxTipoCambio.Text = Incrementos.getTipoDeCambio(DateTime.Now);
            }
        }

        void reporte_CambioProgreso(object sender, BusinessLayer.Helpers.CambioProgresoEventArgs e)
        {
            workerReporte.ReportProgress(e.Progreso);
        }

        private void cmbBxSubConj_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarComboClientes();
        }

        private bool tipoCambioValido()
        {
            if (String.IsNullOrEmpty(txtBxTipoCambio.Text))
            {
                return false;
            }
            else
            {
                try
                {
                    decimal tc = Convert.ToDecimal(txtBxTipoCambio.Text);
                    if (tc > 0)
                        return true;
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            }
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
            if (string.IsNullOrEmpty(error))
                MessageBox.Show("¡Reporte generado correctamente!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Hubo un error al generar el reporte. \n" + error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            botonGenerar.Enabled = true;
            botonCancelar.Enabled = true;
        }

        private void botonCancelar_ControlClicked(object sender, EventArgs e)
        {
            if (botonGenerar.Enabled)
                this.Close();
            else
                reporte.cancelar();
            botonCancelar.Enabled = false;
        }
    }
}
