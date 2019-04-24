namespace GestorReportes.PresentationLayer
{
    partial class Frm_ConfigurarPoliza
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Importe de cargos | ICA");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Importe de descuento | ID");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Importe de pago con saldo a favor | IPS");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Importe de saldo a favor generado | ISF");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Importe en moneda original | IMO");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("IVA | IIV");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("IVA Total Cobrado (Multiples) | ITP");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Pago Total (Multiples) | PTB");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("Porcentaje de participacion | PCP");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("Retención del ISR | RIS");
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("Retención del IVA | RIV");
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("Tipo de cambio | TC");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ConfigurarPoliza));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlContenedorConfigs = new System.Windows.Forms.Panel();
            this.flwLytPnlConfigs = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdBtnDolaresCobro = new System.Windows.Forms.RadioButton();
            this.rdBtnPesosCobro = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCierraParent = new System.Windows.Forms.Button();
            this.btnAbreParent = new System.Windows.Forms.Button();
            this.btnDivide = new System.Windows.Forms.Button();
            this.btnMultiplica = new System.Windows.Forms.Button();
            this.btnMenos = new System.Windows.Forms.Button();
            this.btnMas = new System.Windows.Forms.Button();
            this.rdBtnCobranzaVtas = new System.Windows.Forms.RadioButton();
            this.radioIngresoConsolidado = new System.Windows.Forms.RadioButton();
            this.rdBtnNotasCredito = new System.Windows.Forms.RadioButton();
            this.rdBtnCancel = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdBtnDolares = new System.Windows.Forms.RadioButton();
            this.rdBtnPesos = new System.Windows.Forms.RadioButton();
            this.lstVwDatos = new System.Windows.Forms.ListView();
            this.datosCulumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstVwTipos = new System.Windows.Forms.ListView();
            this.tipoColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rdBtnCobranza = new System.Windows.Forms.RadioButton();
            this.rdBtnGeneracion = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBxInmob = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.botonGuardar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonAgregar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.groupBox1.SuspendLayout();
            this.pnlContenedorConfigs.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pnlContenedorConfigs);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.button7);
            this.groupBox1.Controls.Add(this.button8);
            this.groupBox1.Controls.Add(this.button9);
            this.groupBox1.Controls.Add(this.button10);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.btnCierraParent);
            this.groupBox1.Controls.Add(this.btnAbreParent);
            this.groupBox1.Controls.Add(this.btnDivide);
            this.groupBox1.Controls.Add(this.btnMultiplica);
            this.groupBox1.Controls.Add(this.btnMenos);
            this.groupBox1.Controls.Add(this.btnMas);
            this.groupBox1.Controls.Add(this.rdBtnCobranzaVtas);
            this.groupBox1.Controls.Add(this.botonGuardar);
            this.groupBox1.Controls.Add(this.botonCancelar);
            this.groupBox1.Controls.Add(this.botonAgregar);
            this.groupBox1.Controls.Add(this.radioIngresoConsolidado);
            this.groupBox1.Controls.Add(this.rdBtnNotasCredito);
            this.groupBox1.Controls.Add(this.rdBtnCancel);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.lstVwDatos);
            this.groupBox1.Controls.Add(this.lstVwTipos);
            this.groupBox1.Controls.Add(this.rdBtnCobranza);
            this.groupBox1.Controls.Add(this.rdBtnGeneracion);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmbBxInmob);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(844, 672);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seleccione las opciones adecuadas para configurar como desea que sea realicen las" +
    " pólizas para la inmobiliaria seleccionada. Para agregar un dato a la fórmula dé" +
    " doble clic.";
            // 
            // pnlContenedorConfigs
            // 
            this.pnlContenedorConfigs.Controls.Add(this.flwLytPnlConfigs);
            this.pnlContenedorConfigs.Location = new System.Drawing.Point(6, 361);
            this.pnlContenedorConfigs.Name = "pnlContenedorConfigs";
            this.pnlContenedorConfigs.Size = new System.Drawing.Size(829, 298);
            this.pnlContenedorConfigs.TabIndex = 59;
            // 
            // flwLytPnlConfigs
            // 
            this.flwLytPnlConfigs.AutoScroll = true;
            this.flwLytPnlConfigs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flwLytPnlConfigs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flwLytPnlConfigs.Location = new System.Drawing.Point(0, 0);
            this.flwLytPnlConfigs.Name = "flwLytPnlConfigs";
            this.flwLytPnlConfigs.Size = new System.Drawing.Size(829, 298);
            this.flwLytPnlConfigs.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdBtnDolaresCobro);
            this.groupBox3.Controls.Add(this.rdBtnPesosCobro);
            this.groupBox3.Location = new System.Drawing.Point(170, 278);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(120, 59);
            this.groupBox3.TabIndex = 58;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Moneda de Cobro:";
            this.groupBox3.Visible = false;
            // 
            // rdBtnDolaresCobro
            // 
            this.rdBtnDolaresCobro.AutoSize = true;
            this.rdBtnDolaresCobro.Location = new System.Drawing.Point(6, 36);
            this.rdBtnDolaresCobro.Name = "rdBtnDolaresCobro";
            this.rdBtnDolaresCobro.Size = new System.Drawing.Size(61, 17);
            this.rdBtnDolaresCobro.TabIndex = 1;
            this.rdBtnDolaresCobro.Text = "Dolares";
            this.rdBtnDolaresCobro.UseVisualStyleBackColor = true;
            // 
            // rdBtnPesosCobro
            // 
            this.rdBtnPesosCobro.AutoSize = true;
            this.rdBtnPesosCobro.Checked = true;
            this.rdBtnPesosCobro.Location = new System.Drawing.Point(6, 19);
            this.rdBtnPesosCobro.Name = "rdBtnPesosCobro";
            this.rdBtnPesosCobro.Size = new System.Drawing.Size(54, 17);
            this.rdBtnPesosCobro.TabIndex = 0;
            this.rdBtnPesosCobro.TabStop = true;
            this.rdBtnPesosCobro.Text = "Pesos";
            this.rdBtnPesosCobro.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(698, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 13);
            this.label3.TabIndex = 57;
            this.label3.Text = "Operaciones Aritméticas";
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.White;
            this.button6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Location = new System.Drawing.Point(701, 201);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(23, 23);
            this.button6.TabIndex = 56;
            this.button6.Text = "9";
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Click += new System.EventHandler(this.botonOperacion);
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.White;
            this.button7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Location = new System.Drawing.Point(788, 172);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(23, 23);
            this.button7.TabIndex = 55;
            this.button7.Text = "8";
            this.button7.UseVisualStyleBackColor = false;
            this.button7.Click += new System.EventHandler(this.botonOperacion);
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.Color.White;
            this.button8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Location = new System.Drawing.Point(759, 172);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(23, 23);
            this.button8.TabIndex = 54;
            this.button8.Text = "7";
            this.button8.UseVisualStyleBackColor = false;
            this.button8.Click += new System.EventHandler(this.botonOperacion);
            // 
            // button9
            // 
            this.button9.BackColor = System.Drawing.Color.White;
            this.button9.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button9.Location = new System.Drawing.Point(730, 172);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(23, 23);
            this.button9.TabIndex = 53;
            this.button9.Text = "6";
            this.button9.UseVisualStyleBackColor = false;
            this.button9.Click += new System.EventHandler(this.botonOperacion);
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.White;
            this.button10.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.Location = new System.Drawing.Point(701, 172);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(23, 23);
            this.button10.TabIndex = 52;
            this.button10.Text = "5";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.botonOperacion);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.White;
            this.button5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Location = new System.Drawing.Point(788, 143);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(23, 23);
            this.button5.TabIndex = 51;
            this.button5.Text = "4";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.botonOperacion);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.White;
            this.button3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(759, 143);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(23, 23);
            this.button3.TabIndex = 50;
            this.button3.Text = "3";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.botonOperacion);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.White;
            this.button4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(730, 143);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(23, 23);
            this.button4.TabIndex = 49;
            this.button4.Text = "2";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.botonOperacion);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.White;
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(701, 143);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(23, 23);
            this.button2.TabIndex = 48;
            this.button2.Text = "1";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.botonOperacion);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(730, 201);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(23, 23);
            this.button1.TabIndex = 47;
            this.button1.Text = "0";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.botonOperacion);
            // 
            // btnCierraParent
            // 
            this.btnCierraParent.BackColor = System.Drawing.Color.White;
            this.btnCierraParent.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCierraParent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCierraParent.Location = new System.Drawing.Point(788, 201);
            this.btnCierraParent.Name = "btnCierraParent";
            this.btnCierraParent.Size = new System.Drawing.Size(23, 23);
            this.btnCierraParent.TabIndex = 46;
            this.btnCierraParent.Text = ")";
            this.btnCierraParent.UseVisualStyleBackColor = false;
            // 
            // btnAbreParent
            // 
            this.btnAbreParent.BackColor = System.Drawing.Color.White;
            this.btnAbreParent.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAbreParent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbreParent.Location = new System.Drawing.Point(759, 201);
            this.btnAbreParent.Name = "btnAbreParent";
            this.btnAbreParent.Size = new System.Drawing.Size(23, 23);
            this.btnAbreParent.TabIndex = 45;
            this.btnAbreParent.Text = "(";
            this.btnAbreParent.UseVisualStyleBackColor = false;
            // 
            // btnDivide
            // 
            this.btnDivide.BackColor = System.Drawing.Color.White;
            this.btnDivide.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDivide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDivide.Location = new System.Drawing.Point(788, 230);
            this.btnDivide.Name = "btnDivide";
            this.btnDivide.Size = new System.Drawing.Size(23, 23);
            this.btnDivide.TabIndex = 44;
            this.btnDivide.Text = "/";
            this.btnDivide.UseVisualStyleBackColor = false;
            // 
            // btnMultiplica
            // 
            this.btnMultiplica.BackColor = System.Drawing.Color.White;
            this.btnMultiplica.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMultiplica.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMultiplica.Location = new System.Drawing.Point(759, 230);
            this.btnMultiplica.Name = "btnMultiplica";
            this.btnMultiplica.Size = new System.Drawing.Size(23, 23);
            this.btnMultiplica.TabIndex = 43;
            this.btnMultiplica.Text = "x";
            this.btnMultiplica.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.btnMultiplica.UseVisualStyleBackColor = false;
            // 
            // btnMenos
            // 
            this.btnMenos.BackColor = System.Drawing.Color.White;
            this.btnMenos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMenos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenos.Location = new System.Drawing.Point(730, 230);
            this.btnMenos.Name = "btnMenos";
            this.btnMenos.Size = new System.Drawing.Size(23, 23);
            this.btnMenos.TabIndex = 42;
            this.btnMenos.Text = "-";
            this.btnMenos.UseVisualStyleBackColor = false;
            // 
            // btnMas
            // 
            this.btnMas.BackColor = System.Drawing.Color.White;
            this.btnMas.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMas.Location = new System.Drawing.Point(701, 230);
            this.btnMas.Name = "btnMas";
            this.btnMas.Size = new System.Drawing.Size(23, 23);
            this.btnMas.TabIndex = 41;
            this.btnMas.Text = "+";
            this.btnMas.UseVisualStyleBackColor = false;
            // 
            // rdBtnCobranzaVtas
            // 
            this.rdBtnCobranzaVtas.AutoSize = true;
            this.rdBtnCobranzaVtas.Location = new System.Drawing.Point(589, 64);
            this.rdBtnCobranzaVtas.Name = "rdBtnCobranzaVtas";
            this.rdBtnCobranzaVtas.Size = new System.Drawing.Size(106, 17);
            this.rdBtnCobranzaVtas.TabIndex = 38;
            this.rdBtnCobranzaVtas.Text = "Cobranza Ventas";
            this.rdBtnCobranzaVtas.UseVisualStyleBackColor = true;
            // 
            // radioIngresoConsolidado
            // 
            this.radioIngresoConsolidado.AutoSize = true;
            this.radioIngresoConsolidado.Location = new System.Drawing.Point(444, 64);
            this.radioIngresoConsolidado.Name = "radioIngresoConsolidado";
            this.radioIngresoConsolidado.Size = new System.Drawing.Size(120, 17);
            this.radioIngresoConsolidado.TabIndex = 24;
            this.radioIngresoConsolidado.Text = "Ingreso consolidado";
            this.radioIngresoConsolidado.UseVisualStyleBackColor = true;
            // 
            // rdBtnNotasCredito
            // 
            this.rdBtnNotasCredito.AutoSize = true;
            this.rdBtnNotasCredito.Location = new System.Drawing.Point(325, 64);
            this.rdBtnNotasCredito.Name = "rdBtnNotasCredito";
            this.rdBtnNotasCredito.Size = new System.Drawing.Size(103, 17);
            this.rdBtnNotasCredito.TabIndex = 23;
            this.rdBtnNotasCredito.TabStop = true;
            this.rdBtnNotasCredito.Text = "Notas de crédito";
            this.rdBtnNotasCredito.UseVisualStyleBackColor = true;
            // 
            // rdBtnCancel
            // 
            this.rdBtnCancel.AutoSize = true;
            this.rdBtnCancel.Location = new System.Drawing.Point(225, 64);
            this.rdBtnCancel.Name = "rdBtnCancel";
            this.rdBtnCancel.Size = new System.Drawing.Size(84, 17);
            this.rdBtnCancel.TabIndex = 22;
            this.rdBtnCancel.TabStop = true;
            this.rdBtnCancel.Text = "Cancelación";
            this.rdBtnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdBtnDolares);
            this.groupBox2.Controls.Add(this.rdBtnPesos);
            this.groupBox2.Location = new System.Drawing.Point(15, 278);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(120, 59);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Para contrato en:";
            // 
            // rdBtnDolares
            // 
            this.rdBtnDolares.AutoSize = true;
            this.rdBtnDolares.Location = new System.Drawing.Point(6, 36);
            this.rdBtnDolares.Name = "rdBtnDolares";
            this.rdBtnDolares.Size = new System.Drawing.Size(61, 17);
            this.rdBtnDolares.TabIndex = 1;
            this.rdBtnDolares.Text = "Dolares";
            this.rdBtnDolares.UseVisualStyleBackColor = true;
            // 
            // rdBtnPesos
            // 
            this.rdBtnPesos.AutoSize = true;
            this.rdBtnPesos.Checked = true;
            this.rdBtnPesos.Location = new System.Drawing.Point(6, 19);
            this.rdBtnPesos.Name = "rdBtnPesos";
            this.rdBtnPesos.Size = new System.Drawing.Size(54, 17);
            this.rdBtnPesos.TabIndex = 0;
            this.rdBtnPesos.TabStop = true;
            this.rdBtnPesos.Text = "Pesos";
            this.rdBtnPesos.UseVisualStyleBackColor = true;
            // 
            // lstVwDatos
            // 
            this.lstVwDatos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.datosCulumn});
            this.lstVwDatos.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstVwDatos.FullRowSelect = true;
            this.lstVwDatos.HideSelection = false;
            listViewItem9.ToolTipText = "Porcentaje de participación Copropietarios";
            this.lstVwDatos.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12});
            this.lstVwDatos.Location = new System.Drawing.Point(389, 112);
            this.lstVwDatos.MultiSelect = false;
            this.lstVwDatos.Name = "lstVwDatos";
            this.lstVwDatos.Size = new System.Drawing.Size(275, 150);
            this.lstVwDatos.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstVwDatos.TabIndex = 10;
            this.lstVwDatos.UseCompatibleStateImageBehavior = false;
            this.lstVwDatos.View = System.Windows.Forms.View.Details;
            this.lstVwDatos.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstVwDatos_MouseDoubleClick);
            // 
            // datosCulumn
            // 
            this.datosCulumn.Text = "Datos";
            this.datosCulumn.Width = 250;
            // 
            // lstVwTipos
            // 
            this.lstVwTipos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.tipoColumn});
            this.lstVwTipos.FullRowSelect = true;
            this.lstVwTipos.HideSelection = false;
            this.lstVwTipos.Location = new System.Drawing.Point(15, 112);
            this.lstVwTipos.MultiSelect = false;
            this.lstVwTipos.Name = "lstVwTipos";
            this.lstVwTipos.Size = new System.Drawing.Size(275, 150);
            this.lstVwTipos.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstVwTipos.TabIndex = 9;
            this.lstVwTipos.UseCompatibleStateImageBehavior = false;
            this.lstVwTipos.View = System.Windows.Forms.View.Details;
            this.lstVwTipos.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstVwTipos_MouseDoubleClick);
            // 
            // tipoColumn
            // 
            this.tipoColumn.Text = "Tipos";
            this.tipoColumn.Width = 250;
            // 
            // rdBtnCobranza
            // 
            this.rdBtnCobranza.AutoSize = true;
            this.rdBtnCobranza.Location = new System.Drawing.Point(149, 64);
            this.rdBtnCobranza.Name = "rdBtnCobranza";
            this.rdBtnCobranza.Size = new System.Drawing.Size(60, 17);
            this.rdBtnCobranza.TabIndex = 8;
            this.rdBtnCobranza.Text = "Ingreso";
            this.rdBtnCobranza.UseVisualStyleBackColor = true;
            this.rdBtnCobranza.CheckedChanged += new System.EventHandler(this.rdBtnCobranza_CheckedChanged);
            // 
            // rdBtnGeneracion
            // 
            this.rdBtnGeneracion.AutoSize = true;
            this.rdBtnGeneracion.Checked = true;
            this.rdBtnGeneracion.Location = new System.Drawing.Point(81, 64);
            this.rdBtnGeneracion.Name = "rdBtnGeneracion";
            this.rdBtnGeneracion.Size = new System.Drawing.Size(52, 17);
            this.rdBtnGeneracion.TabIndex = 7;
            this.rdBtnGeneracion.TabStop = true;
            this.rdBtnGeneracion.Text = "Diario";
            this.rdBtnGeneracion.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Tipo:";
            // 
            // cmbBxInmob
            // 
            this.cmbBxInmob.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxInmob.DropDownWidth = 364;
            this.cmbBxInmob.FormattingEnabled = true;
            this.cmbBxInmob.Location = new System.Drawing.Point(80, 33);
            this.cmbBxInmob.Name = "cmbBxInmob";
            this.cmbBxInmob.Size = new System.Drawing.Size(615, 21);
            this.cmbBxInmob.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Inmobiliaria:";
            // 
            // botonGuardar
            // 
            this.botonGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGuardar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGuardar.Imagen = ((System.Drawing.Image)(resources.GetObject("botonGuardar.Imagen")));
            this.botonGuardar.Location = new System.Drawing.Point(678, 268);
            this.botonGuardar.Name = "botonGuardar";
            this.botonGuardar.Size = new System.Drawing.Size(75, 75);
            this.botonGuardar.TabIndex = 37;
            this.botonGuardar.Texto = "Guardar";
            this.botonGuardar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnGuardar_Click);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(757, 268);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 36;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnCancelar_Click);
            // 
            // botonAgregar
            // 
            this.botonAgregar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonAgregar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonAgregar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonAgregar.Imagen = ((System.Drawing.Image)(resources.GetObject("botonAgregar.Imagen")));
            this.botonAgregar.Location = new System.Drawing.Point(309, 268);
            this.botonAgregar.Name = "botonAgregar";
            this.botonAgregar.Size = new System.Drawing.Size(75, 75);
            this.botonAgregar.TabIndex = 35;
            this.botonAgregar.Texto = "Agregar";
            this.botonAgregar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonAgregar_ControlClicked);
            this.botonAgregar.Click += new System.EventHandler(this.btnAgregarControl_Click);
            // 
            // Frm_ConfigurarPoliza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(844, 672);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(850, 700);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(850, 700);
            this.Name = "Frm_ConfigurarPoliza";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración de póliza contable";
            this.Load += new System.EventHandler(this.Frm_ConfigurarPoliza_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlContenedorConfigs.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdBtnCobranza;
        private System.Windows.Forms.RadioButton rdBtnGeneracion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbBxInmob;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lstVwTipos;
        private System.Windows.Forms.ColumnHeader tipoColumn;
        private System.Windows.Forms.ListView lstVwDatos;
        private System.Windows.Forms.ColumnHeader datosCulumn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdBtnDolares;
        private System.Windows.Forms.RadioButton rdBtnPesos;
        private System.Windows.Forms.RadioButton rdBtnCancel;
        private System.Windows.Forms.RadioButton rdBtnNotasCredito;
        private System.Windows.Forms.RadioButton radioIngresoConsolidado;
        private Controls.Ctrl_Opcion botonAgregar;
        private Controls.Ctrl_Opcion botonGuardar;
        private Controls.Ctrl_Opcion botonCancelar;
        private System.Windows.Forms.RadioButton rdBtnCobranzaVtas;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnCierraParent;
        private System.Windows.Forms.Button btnAbreParent;
        private System.Windows.Forms.Button btnDivide;
        private System.Windows.Forms.Button btnMultiplica;
        private System.Windows.Forms.Button btnMenos;
        private System.Windows.Forms.Button btnMas;
        private System.Windows.Forms.Panel pnlContenedorConfigs;
        private System.Windows.Forms.FlowLayoutPanel flwLytPnlConfigs;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rdBtnDolaresCobro;
        private System.Windows.Forms.RadioButton rdBtnPesosCobro;
    }
}