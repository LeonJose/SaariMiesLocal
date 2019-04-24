using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesPolizas;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_GenerarPolizaCopropiedad : Form
    {
        private PolizaCopropietarios poliza = null;
        private string resultGenerar = string.Empty;
        public Frm_GenerarPolizaCopropiedad()
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
        }

        private void generar()
        {
            List<ColumnasExcelEntity> columnas = new List<ColumnasExcelEntity>();
            
            CondicionesEntity condiciones = new CondicionesEntity();
            condiciones.Inmobiliaria = textBoxInmo.Text;
            condiciones.Formato = FormatoExportacionPoliza.ContpaqXls;
            //condiciones.Formato = FormatoExportacionPoliza.Axapta;
            condiciones.MultiplesEncabezados = true;
            condiciones.IncluirSegmento = false;            
            condiciones.NumeroPoliza = "10092015";
            condiciones.ConceptoPoliza = "Recibos Expedidos Septiembre 2015";
            condiciones.FechaInicio = dateTimePicker1.Value;
            condiciones.FechaFin = dateTimePicker2.Value;
            condiciones.IncluirCancelados = false;
            condiciones.TipoPresentacion = 2;
            condiciones.IncluirUUIDEnConcepto = true;
            condiciones.NombreCliente = true;
            condiciones.AfectarSaldos = true; ;
            condiciones.UsarConceptoPrincipal = true;
            condiciones.IncluirPeriodoEnReferencia = true;
            condiciones.NombreCliente = false;
            condiciones.ClienteEnLugarDePeriodo = true;
            condiciones.ExcluirConceptoLibre = true;
            condiciones.TipoPresentacion = 2;
            condiciones.TipoPoliza = 11;
            condiciones.CuatroDecimales = false;
            //condiciones.Sucursal.NombreSucursal = "Merida";           
            poliza = new PolizaCopropietarios(condiciones, columnas);
            resultGenerar = poliza.generar();
            if (resultGenerar == "")
            {
                MessageBox.Show("Poliza generada correctamente");
            }
            else
            {
                MessageBox.Show("Ocurrio un error al generar la póliza: " + resultGenerar, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form_GenerarPolizaCopropiedad_Load(object sender, EventArgs e)
        {
            
        }

        private void buttonGenerar_Click(object sender, EventArgs e)
        {
            generar();
        }
    }
}
