using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GestorReportes.PresentationLayer.Controls
{
    public partial class Ctrl_AlertaPorPersona : UserControl
    {
        private int identificadorSeleccionado = 100;
        private bool EsVenta = false;
        public Ctrl_AlertaPorPersona(int selectedId, bool esVenta)
        {
            InitializeComponent();
            identificadorSeleccionado = selectedId;
            EsVenta = esVenta;
        }

        private void Ctrl_AlertaPorPersona_Load(object sender, EventArgs e)
        {
            if(!EsVenta)
                cmbBxTiposAlerta.DataSource = getDtAlertas();
            else
                cmbBxTiposAlerta.DataSource = getDtAlertasVentas();
            cmbBxTiposAlerta.DisplayMember = "Descripcion";
            cmbBxTiposAlerta.ValueMember = "ID_Alerta";
            cmbBxTiposAlerta.SelectedValue = identificadorSeleccionado;
            //this.comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
            //this.comboBox1.DrawItem += new DrawItemEventHandler(comboBox1_DrawItem);
            cmbBxTiposAlerta.DrawMode = DrawMode.OwnerDrawFixed;
            cmbBxTiposAlerta.DrawItem += new DrawItemEventHandler(cmbBxTiposAlerta_DrawItem);
            cmbBxTiposAlerta.SelectedValueChanged += new EventHandler(cmbBxTiposAlerta_SelectedValueChanged);
        }

        void cmbBxTiposAlerta_SelectedValueChanged(object sender, EventArgs e)
        {
            this.toolTip1.Hide(cmbBxTiposAlerta);
        }

        void cmbBxTiposAlerta_DrawItem(object sender, DrawItemEventArgs e)
        {
            string text = this.cmbBxTiposAlerta.GetItemText(cmbBxTiposAlerta.Items[e.Index]);
            e.DrawBackground();
            using (SolidBrush br = new SolidBrush(e.ForeColor))
            { e.Graphics.DrawString(text, e.Font, br, e.Bounds); }

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            { this.toolTip1.Show(text, cmbBxTiposAlerta, e.Bounds.Right, e.Bounds.Bottom); }
            else { this.toolTip1.Hide(cmbBxTiposAlerta); }
            e.DrawFocusRectangle();
        }

        private DataTable getDtAlertas()
        {
            DataTable dtAlertas = new DataTable("Alertas");
            DataColumn dcIDAlertaColumn = new DataColumn("ID_Alerta");
            DataColumn dcDescAlertaColumn = new DataColumn("Descripcion");
            dtAlertas.Columns.Add(dcIDAlertaColumn);
            dtAlertas.Columns.Add(dcDescAlertaColumn);
            dtAlertas.AcceptChanges();
            DataRow drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 100;
            drAlerta["Descripcion"] = "Sin alerta";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1501;
            drAlerta["Descripcion"] = "El cliente o usuario ofrece pagar por adelantado las rentas correspondientes a un periodo largo de tiempo sin justificación lógica para ello";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1502;
            drAlerta["Descripcion"] = "El cliente o usuario paga por adelantado las rentas correspondientes a un periodo largo de tiempo en efectivo sin justificación aparente";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1503;
            drAlerta["Descripcion"] = "El cliente o usuario paga por adelantado las rentas correspondientes a un periodo largo de tiempo en efectivo y posteriormente pide la cancelación del contrato y solicita el reembolso del monto pagado por medio de otro instrumento monetario diferente al efectivo";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1504;
            drAlerta["Descripcion"] = "El cliente o usuario se niega a dar información sobre el uso que se dará al inmueble por arrendar";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1505;
            drAlerta["Descripcion"] = "Hay indicios, o certeza, de que el inmueble arrendado no está siendo utilizado para el propósito expresado por el cliente o usuario,  sino para posibles actividades ilícitas";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1506;
            drAlerta["Descripcion"] = "El cliente o usuario se rehúsa a proporcionar documentos personales que lo identifiquen";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1507;
            drAlerta["Descripcion"] = "El  pago de las rentas del inmueble es realizado por un tercero sin relación aparente con el cliente o usuario";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1508;
            drAlerta["Descripcion"] = "El cliente o usuario o personas relacionadas con él, realizan múltiples operaciones de arrendamiento en un periodo muy corto sin razón aparente";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1509;
            drAlerta["Descripcion"] = "El cliente o usuario no muestra tener interés en las características del inmueble objeto de arrendamiento o en el monto de la renta y condiciones del contrato";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1510;
            drAlerta["Descripcion"] = "Se conoce un historial criminal del cliente o usuario o el aval, de algún familiar directo o persona relacionada con él";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1511;
            drAlerta["Descripcion"] = "El cliente o usuario no quiere ser relacionado con la operación de arrendamiento";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1512;
            drAlerta["Descripcion"] = "De acuerdo con la ocupación del cliente o usuario, la operación parece estar fuera de su alcance";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1513;
            drAlerta["Descripcion"] = "De acuerdo con los ingresos declarados por el cliente o usuario, la operación parece estar fuera de su alcance";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1514;
            drAlerta["Descripcion"] = "El cliente o usuario muestra fuerte interés en la realización de la transacción con rapidez, sin que exista causa justificada";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1515;
            drAlerta["Descripcion"] = "El cliente o usuario acepta pagar un monto de arrendamiento significativamente mayor a los valores de mercado";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1516;
            drAlerta["Descripcion"] = "Hay indicios, o certeza, que las partes no están actuando en nombre propio y están tratando de ocultar la identidad del cliente o usuario real";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1517;
            drAlerta["Descripcion"] = "El cliente o usuario realiza pagos mediante el uso de divisas en efectivo en montos elevados o de poco uso sin que la ocupación del cliente o usuario lo justifique";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1518;
            drAlerta["Descripcion"] = "El cliente o usuario realiza pagos por medio de una transferencia internacional sin que la ocupación, perfil o nacionalidad del cliente o usuario lo justifique";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1519;
            drAlerta["Descripcion"] = "El cliente o usuario realiza pagos por medio de una transferencia internacional proveniente de un país considerado como paraíso fiscal o de alto riesgo";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1520;
            drAlerta["Descripcion"] = "El cliente o usuario insiste en liquidar pagar la operación en efectivo rebasando el umbral permitido para uso de efectivo";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1521;
            drAlerta["Descripcion"] = "El cliente o usuario intenta sobornar o extorsionar al intermediario o dueño del inmueble con  el fin de realizar la operación de forma irregular";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1522;
            drAlerta["Descripcion"] = "El cliente o usuario solicita condiciones especiales poco usuales en la realización de la operación o en las condiciones del inmueble por arrendar";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1523;
            drAlerta["Descripcion"] = "El cliente o usuario proporcionó datos falsos  o documentos apócrifos al realizar la operación";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1524;
            drAlerta["Descripcion"] = "Operaciones con organizaciones sin fines de lucro, cuando las características de la transacción no coinciden con los objetivos de la entidad";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1525;
            drAlerta["Descripcion"] = "El cliente o usuario realiza los pagos con cheques de caja con la intención de ocultar el origen de los recursos";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 1526;
            drAlerta["Descripcion"] = "Se tiene sospecha de que el inmueble es utilizado como centro que brinda ayuda a la comunidad o grupos vulnerables que pudieran estar vinculados con actividades u organizaciones delictivas";
            dtAlertas.Rows.Add(drAlerta);
            /*
            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 9999;
            drAlerta["Descripcion"] = "Otra alerta";
            dtAlertas.Rows.Add(drAlerta);*/

            dtAlertas.AcceptChanges();
            return dtAlertas;
        }

        private DataTable getDtAlertasVentas()
        {
            DataTable dtAlertas = new DataTable("Alertas");
            DataColumn dcIDAlertaColumn = new DataColumn("ID_Alerta");
            DataColumn dcDescAlertaColumn = new DataColumn("Descripcion");
            dtAlertas.Columns.Add(dcIDAlertaColumn);
            dtAlertas.Columns.Add(dcDescAlertaColumn);
            dtAlertas.AcceptChanges();
            DataRow drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 100;
            drAlerta["Descripcion"] = "Sin alerta";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 501;
            drAlerta["Descripcion"] = "El cliente o usuario se rehúsa a proporcionar documentos personales que lo identifiquen";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 502;
            drAlerta["Descripcion"] = "El pago por el inmueble es realizado por un tercero sin relación aparente con el cliente o usuario";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 503;
            drAlerta["Descripcion"] = "El cliente o usuario o personas relacionadas con él realizan múltiples operaciones en un periodo muy corto sin razón aparente";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 504;
            drAlerta["Descripcion"] = "El cliente o usuario no muestra tener interés en las características de la propiedad objeto de la operación o en el precio y condiciones de la transacción";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 505;
            drAlerta["Descripcion"] = "Se conoce un historial criminal del cliente o usuario, de algún familiar directo o persona relacionada con él";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 506;
            drAlerta["Descripcion"] = "El cliente o usuario no quiere ser relacionado con la compra del inmueble";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 507;
            drAlerta["Descripcion"] = "De acuerdo con la ocupación del cliente o usuario, la operación parece estar fuera de su alcance";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 508;
            drAlerta["Descripcion"] = "De acuerdo con los ingresos declarados por el cliente o usuario, la operación parece estar fuera de su alcance";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 509;
            drAlerta["Descripcion"] = "El cliente o usuario muestra fuerte interés en la realización de la transacción con rapidez, sin que exista causa justificada";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 510;
            drAlerta["Descripcion"] = "El cliente o usuario pide que el pago sea dividido en partes con un breve intervalo de tiempo entre ellos";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 511;
            drAlerta["Descripcion"] = "El cliente o usuario solicita que se realice la operación por medio de un contrato privado, donde no hay intención de registrarlo ante notario, o cuando esta intención se expresa, no se lleva a cabo finalmente";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 512;
            drAlerta["Descripcion"] = "Transacciones sucesivas de compra y venta de la misma propiedad en un periodo corto de tiempo, con cambios injustificados del valor de la misma";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 513;
            drAlerta["Descripcion"] = "La operación se lleva acabo a un valor de venta o compra significativamente diferente (mucho mayor o mucho menor) a partir del valor real de la propiedad o a los valores de mercado";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 514;
            drAlerta["Descripcion"] = "Hay indicios, o certeza, que las partes no están actuando en nombre propio y están tratando de ocultar la identidad del cliente o usuario real";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 515;
            drAlerta["Descripcion"] = "Uso de divisas en efectivo en montos elevados o de poco uso sin que la ocupación del cliente o usuario lo justifique";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 516;
            drAlerta["Descripcion"] = "La operación se liquida por medio de una transferencia internacional sin que la ocupación, perfil o nacionalidad del cliente o usuario lo justifique";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 517;
            drAlerta["Descripcion"] = "La operación se liquida por medio de una transferencia internacional proveniente de un país considerado como paraíso fiscal o de alto riesgo";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 518;
            drAlerta["Descripcion"] = "El cliente o usuario insiste en liquidar pagar la operación en efectivo rebasando el umbral permitido para uso de efectivo";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 519;
            drAlerta["Descripcion"] = "El cliente o usuario intenta sobornar o extorsionar al vendedor con  el fin de realizar la operación de forma irregular";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 520;
            drAlerta["Descripcion"] = "El cliente o usuario solicita condiciones especiales poco usuales en la realización de la operación";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 521;
            drAlerta["Descripcion"] = "El cliente o usuario proporcionó datos falsos  o documentos apócrifos al realizar la operación";
            dtAlertas.Rows.Add(drAlerta);

            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 522;
            drAlerta["Descripcion"] = "Operaciones con organizaciones sin fines de lucro, cuando las características de la transacción no coinciden con los objetivos de la entidad";
            dtAlertas.Rows.Add(drAlerta);
            /*
            drAlerta = dtAlertas.NewRow();
            drAlerta["ID_Alerta"] = 9999;
            drAlerta["Descripcion"] = "Otra alerta";
            dtAlertas.Rows.Add(drAlerta);*/

            dtAlertas.AcceptChanges();
            return dtAlertas;
        }
    }
}
