using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_FichaInformativa : Form
    {
        Inmobiliaria inmobiliaria = new Inmobiliaria();
        FichaInformativa fichaInformativa = new FichaInformativa();
        private bool permisosInmo = Properties.Settings.Default.PermisosInmobiliaria;
        private string idInmobiliaria = string.Empty; private string NombreInmo = string.Empty;
        string idConjunto = string.Empty;

        public Frm_FichaInformativa(string user)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            inicializador();
            fichaInformativa.Usuario = user;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void Frm_FichaInformativa_Load(object sender, EventArgs e)
        {
            cargarGrupoEmpresarial();
           // cargarInmobiliarias("Todos");
            //cmbBxInm.DataSource = HelpInmobiliarias.obtenerInmobiliarias(fichaInformativa.Usuario,permisosInmo);
            cargarInmobiliarias("Todos");
        }

        public void cargarInmobiliarias(string idGrupo)
        {
            try
            {
                cmbBxInm.DataSource = HelpInmobiliarias.obtenerInmobiliarias(fichaInformativa.Usuario, permisosInmo,idGrupo);
                cmbBxInm.DisplayMember = "NombreComercial";
                cmbBxInm.ValueMember = "ID";
                cmbBxInm.SelectedIndex = 0;
                idInmobiliaria = cmbBxInm.SelectedValue.ToString();
                NombreInmo = cmbBxInm.Text.ToString();
            }
            catch
            {
               // result = "Error al obtener las inmobiliarias.";
            }
        }

        public void cargarConjuntos(string selectedValue)
        {
            cmbBxConjunto.DataSource = HelpInmobiliarias.getConjuntos(selectedValue,true); //.ObtenerConjuntos(iDinmobiliaria);
            cmbBxConjunto.DisplayMember = "Nombre";
            cmbBxConjunto.ValueMember = "ID";
            idConjunto = cmbBxConjunto.SelectedValue.ToString();
           // NombreConjunto = comboConjunto.Text.ToString();
            //comboConjunto.SelectedIndex = 0;
        }


        private void inicializador()
        {
            //btnNext.Focus();
            //cmbBxInm.Visible = false;
            //lblInmobiliaria.Visible = false;
            cmbBxConjunto.Visible = false;
            lblConj.Visible = false;
            label1.Visible = false;
            comboInmueble.Visible = false;
        }

        private void cargarGrupoEmpresarial()
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
                cmbBxGpoEmpr.DataSource = view;
                cmbBxGpoEmpr.DisplayMember = "P0002_NOMBRE";
                cmbBxGpoEmpr.ValueMember = "P0001_ID_GRUPO";
                cmbBxGpoEmpr.SelectedIndex = 0;
            }
            else
            {
                DataTable dtTodos = new DataTable();
                dtTodos.Columns.Add("P0001_ID_GRUPO");
                dtTodos.Columns.Add("P0002_NOMBRE");
                DataRow row = dtTodos.NewRow();
                row["P0001_ID_GRUPO"] = "Todos";
                row["P0002_NOMBRE"] = "*Todos";
                dtTodos.Rows.InsertAt(row, 0);
                DataView view = new DataView(dtTodos);
                view.Sort = "P0002_NOMBRE";
                cmbBxGpoEmpr.DataSource = view;
                cmbBxGpoEmpr.DisplayMember = "P0002_NOMBRE";
                cmbBxGpoEmpr.ValueMember = "P0001_ID_GRUPO";
                cmbBxGpoEmpr.SelectedIndex = 0;
            }
        }

        /*private void cargarInmobiliarias(string idGpo)
        {
                DataTable dtInmobiliarias = inmobiliaria.getInmobiliariasCommand(idGpo);
                if (dtInmobiliarias.Rows.Count > 0)
                {
                    DataRow row = dtInmobiliarias.NewRow();
                    row["P0101_ID_ARR"] = "Todos";
                    row["P0102_N_COMERCIAL"] = "*Todos";
                    row["P0103_RAZON_SOCIAL"] = "Todos";
                    dtInmobiliarias.Rows.InsertAt(row, 0);
                    DataView view = new DataView(dtInmobiliarias);
                    view.Sort = "P0102_N_COMERCIAL";
                    cmbBxInm.DataSource = view; // 3 lineas agregadas para ordenar inmobiliarias 
                    cmbBxInm.DisplayMember = "P0102_N_COMERCIAL";
                    cmbBxInm.ValueMember = "P0101_ID_ARR";
                    cmbBxInm.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("No se encontró ninguna inmobiliaria registrada. \n - Debe tener inmobiliarias registradas. \n - Revise su conexión a la base de datos.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Environment.Exit(0);
                }
        }*/

       /* private void cargarConjuntos(string selectedValue)
        {
            DataTable dtConjuntos = inmobiliaria.getDtConjuntos(selectedValue);
            if (dtConjuntos.Rows.Count > 0)
            {
                DataRow row = dtConjuntos.NewRow();
                row["P0301_ID_CENTRO"] = "Todos";
                row["P0303_NOMBRE"] = "*Todos";
                dtConjuntos.Rows.InsertAt(row, 0);
                DataView view = new DataView(dtConjuntos);
                view.Sort = "P0303_NOMBRE";
                cmbBxConjunto.DataSource = view; // 3 lineas modificadas para ordenar conjuntos
                cmbBxConjunto.ValueMember = "P0301_ID_CENTRO";
                cmbBxConjunto.DisplayMember = "P0303_NOMBRE";
                cmbBxConjunto.SelectedIndex = 0;
            }
            else
            {
                dtConjuntos = inmobiliaria.getConjuntosCommand(selectedValue);
                if (dtConjuntos.Rows.Count > 0)
                {
                    DataRow row = dtConjuntos.NewRow();
                    row["P0301_ID_CENTRO"] = "Todos";
                    row["P0303_NOMBRE"] = "*Todos";
                    dtConjuntos.Rows.InsertAt(row, 0);
                    DataView view = new DataView(dtConjuntos);
                    view.Sort = "P0303_NOMBRE";
                    cmbBxConjunto.DataSource = view; // 3 lineas modificadas para ordenar conjuntos
                    cmbBxConjunto.ValueMember = "P0301_ID_CENTRO";
                    cmbBxConjunto.DisplayMember = "P0303_NOMBRE";
                    cmbBxConjunto.SelectedIndex = 0;
                }
                else
                {
                    cmbBxConjunto.Visible = false;
                    lblConj.Visible = false;                    
                }
            }
        }*/

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
                comboInmueble.DataSource = view;
                comboInmueble.ValueMember = "P0701_ID_EDIFICIO";
                comboInmueble.DisplayMember = "P0703_NOMBRE";
                comboInmueble.SelectedIndex = 0;
            }
            else
            {
                comboInmueble.Visible = false;
                label1.Visible = false;
            }
        
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            fichaInformativa.GrupoEmpresarial = cmbBxGpoEmpr.SelectedValue.ToString().Trim();
            if (cmbBxInm.Visible)
                fichaInformativa.Inmobiliaria = cmbBxInm.SelectedValue.ToString().Trim();
            else
                fichaInformativa.Inmobiliaria = "Todos";
            if(cmbBxConjunto.Visible)
                fichaInformativa.Conjunto = cmbBxConjunto.SelectedValue.ToString().Trim();
            else
                fichaInformativa.Conjunto = "Todos";
            if (comboInmueble.Visible)
            {
                fichaInformativa.Inmueble = comboInmueble.SelectedValue.ToString().Trim();
                              
            }
            else
                fichaInformativa.Inmueble = "Todos";
            this.Visible = false;
            Frm_FichaInformativa2 frm = new Frm_FichaInformativa2(fichaInformativa);
            frm.Show();
        }

        private void cmbBxInm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBxInm.SelectedIndex == 0)
            {
                cmbBxConjunto.Visible = false;
                lblConj.Visible = false;
            }
            else
            {
                cmbBxConjunto.Visible = true;
                lblConj.Visible = true;
                cargarConjuntos(cmbBxInm.SelectedValue.ToString().Trim());
                //cargarConjuntos(cmbBxInm.SelectedValue.ToString().Trim());
            }
        }

        private void cmbBxConjunto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBxConjunto.SelectedIndex == 0)
            {
                comboInmueble.Visible = false;
                label1.Visible = false;
            }
            else
            {
                comboInmueble.Visible = true;
                label1.Visible = true;
                cargarInmuebles(cmbBxConjunto.SelectedValue.ToString().Trim());
            }
        }

        private void cmbBxGpoEmpr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBxGpoEmpr.SelectedIndex == 0)
            {
                //cmbBxInm.Visible = false;
                //lblInmobiliaria.Visible = false;
                cargarInmobiliarias("Todos");
                cmbBxConjunto.Visible = false;
                lblConj.Visible = false;
            }
            else
            {
                cmbBxInm.Visible = true;
                lblInmobiliaria.Visible = true;
                cargarInmobiliarias(cmbBxGpoEmpr.SelectedValue.ToString().Trim());
            }
        }

        private void Frm_FichaInformativa_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
