namespace GestorReportes.PresentationLayer
{
    partial class Frm_RecordatorioCobranza
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
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.printDialog = new System.Windows.Forms.PrintDialog();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.rtbBodymnj = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dateFin = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBoxContratos = new System.Windows.Forms.GroupBox();
            this.radioVigente = new System.Windows.Forms.RadioButton();
            this.radioTodos = new System.Windows.Forms.RadioButton();
            this.groupBoxTipo = new System.Windows.Forms.GroupBox();
            this.radioButtonCliente = new System.Windows.Forms.RadioButton();
            this.radioButtonContrato = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.comboInmobiliaria = new System.Windows.Forms.ComboBox();
            this.textoBuscar = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.gridClientes = new System.Windows.Forms.DataGridView();
            this.IDClienteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.seleccionColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rfcColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridContratos = new System.Windows.Forms.DataGridView();
            this.idCteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnSeleccion = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clienteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inmuebleColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Conjunto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contratoColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IDCliente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Moneda = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel4 = new System.Windows.Forms.Panel();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.checkEnviarCorreo = new System.Windows.Forms.CheckBox();
            this.checkEnviarImpresora = new System.Windows.Forms.CheckBox();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.radioPdf = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkTodosClientes = new System.Windows.Forms.CheckBox();
            this.radioDolar = new System.Windows.Forms.RadioButton();
            this.radioMonedaLocal = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.botonBuscar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBoxContratos.SuspendLayout();
            this.groupBoxTipo.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridClientes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridContratos)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // printDialog
            // 
            this.printDialog.UseEXDialog = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.631579F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 94.73685F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.631579F));
            this.tableLayoutPanel1.Controls.Add(this.panel6, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel5, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.994162F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.2335F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35.12423F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.57456F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.075807F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.99776F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(714, 472);
            this.tableLayoutPanel1.TabIndex = 1;
            this.tableLayoutPanel1.Click += new System.EventHandler(this.tableLayoutPanel1_Click);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label2);
            this.panel6.Controls.Add(this.rtbBodymnj);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(21, 301);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(670, 53);
            this.panel6.TabIndex = 10;
            this.panel6.Click += new System.EventHandler(this.panel6_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(323, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Dar click en el cuadro para editar el mensaje del correo Electronico";
            // 
            // rtbBodymnj
            // 
            this.rtbBodymnj.Location = new System.Drawing.Point(3, 18);
            this.rtbBodymnj.Name = "rtbBodymnj";
            this.rtbBodymnj.Size = new System.Drawing.Size(503, 32);
            this.rtbBodymnj.TabIndex = 8;
            this.rtbBodymnj.Text = "";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dateFin);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(21, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(670, 41);
            this.panel2.TabIndex = 2;
            // 
            // dateFin
            // 
            this.dateFin.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.dateFin.Location = new System.Drawing.Point(127, 11);
            this.dateFin.Name = "dateFin";
            this.dateFin.Size = new System.Drawing.Size(213, 20);
            this.dateFin.TabIndex = 3;
            this.dateFin.TabStop = false;
            this.dateFin.Value = new System.DateTime(2015, 11, 4, 9, 3, 14, 0);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Reporte a la fecha del:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBoxContratos);
            this.panel3.Controls.Add(this.groupBoxTipo);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.comboInmobiliaria);
            this.panel3.Controls.Add(this.botonBuscar);
            this.panel3.Controls.Add(this.textoBuscar);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(21, 50);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(670, 80);
            this.panel3.TabIndex = 3;
            // 
            // groupBoxContratos
            // 
            this.groupBoxContratos.Controls.Add(this.radioVigente);
            this.groupBoxContratos.Controls.Add(this.radioTodos);
            this.groupBoxContratos.Location = new System.Drawing.Point(501, 6);
            this.groupBoxContratos.Name = "groupBoxContratos";
            this.groupBoxContratos.Size = new System.Drawing.Size(90, 67);
            this.groupBoxContratos.TabIndex = 16;
            this.groupBoxContratos.TabStop = false;
            this.groupBoxContratos.Text = "Contratos: ";
            this.groupBoxContratos.Visible = false;
            // 
            // radioVigente
            // 
            this.radioVigente.AutoSize = true;
            this.radioVigente.Location = new System.Drawing.Point(14, 42);
            this.radioVigente.Name = "radioVigente";
            this.radioVigente.Size = new System.Drawing.Size(66, 17);
            this.radioVigente.TabIndex = 7;
            this.radioVigente.Text = "Vigentes";
            this.radioVigente.UseVisualStyleBackColor = true;
            // 
            // radioTodos
            // 
            this.radioTodos.AutoSize = true;
            this.radioTodos.Checked = true;
            this.radioTodos.Location = new System.Drawing.Point(14, 19);
            this.radioTodos.Name = "radioTodos";
            this.radioTodos.Size = new System.Drawing.Size(55, 17);
            this.radioTodos.TabIndex = 8;
            this.radioTodos.TabStop = true;
            this.radioTodos.Text = "Todos";
            this.radioTodos.UseVisualStyleBackColor = true;
            // 
            // groupBoxTipo
            // 
            this.groupBoxTipo.Controls.Add(this.radioButtonCliente);
            this.groupBoxTipo.Controls.Add(this.radioButtonContrato);
            this.groupBoxTipo.Location = new System.Drawing.Point(396, 6);
            this.groupBoxTipo.Name = "groupBoxTipo";
            this.groupBoxTipo.Size = new System.Drawing.Size(100, 67);
            this.groupBoxTipo.TabIndex = 15;
            this.groupBoxTipo.TabStop = false;
            this.groupBoxTipo.Text = "Tipo: ";
            // 
            // radioButtonCliente
            // 
            this.radioButtonCliente.AutoSize = true;
            this.radioButtonCliente.Checked = true;
            this.radioButtonCliente.Location = new System.Drawing.Point(11, 19);
            this.radioButtonCliente.Name = "radioButtonCliente";
            this.radioButtonCliente.Size = new System.Drawing.Size(76, 17);
            this.radioButtonCliente.TabIndex = 10;
            this.radioButtonCliente.TabStop = true;
            this.radioButtonCliente.Text = "Por Cliente";
            this.radioButtonCliente.UseVisualStyleBackColor = true;
            this.radioButtonCliente.CheckedChanged += new System.EventHandler(this.radioButtonCliente_CheckedChanged);
            // 
            // radioButtonContrato
            // 
            this.radioButtonContrato.AutoSize = true;
            this.radioButtonContrato.Location = new System.Drawing.Point(11, 42);
            this.radioButtonContrato.Name = "radioButtonContrato";
            this.radioButtonContrato.Size = new System.Drawing.Size(84, 17);
            this.radioButtonContrato.TabIndex = 11;
            this.radioButtonContrato.Text = "Por Contrato";
            this.radioButtonContrato.UseVisualStyleBackColor = true;
            this.radioButtonContrato.CheckedChanged += new System.EventHandler(this.radioButtonContrato_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(121, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Seleccionar Inmobiliaria:";
            // 
            // comboInmobiliaria
            // 
            this.comboInmobiliaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInmobiliaria.Enabled = false;
            this.comboInmobiliaria.FormattingEnabled = true;
            this.comboInmobiliaria.Location = new System.Drawing.Point(128, 50);
            this.comboInmobiliaria.Name = "comboInmobiliaria";
            this.comboInmobiliaria.Size = new System.Drawing.Size(260, 21);
            this.comboInmobiliaria.TabIndex = 1;
            this.comboInmobiliaria.SelectionChangeCommitted += new System.EventHandler(this.comboEmpresa_SelectionChangeCommitted);
            // 
            // textoBuscar
            // 
            this.textoBuscar.Location = new System.Drawing.Point(93, 13);
            this.textoBuscar.Name = "textoBuscar";
            this.textoBuscar.Size = new System.Drawing.Size(295, 20);
            this.textoBuscar.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Buscar cliente:";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.gridClientes);
            this.panel5.Controls.Add(this.gridContratos);
            this.panel5.Location = new System.Drawing.Point(21, 136);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(670, 158);
            this.panel5.TabIndex = 7;
            // 
            // gridClientes
            // 
            this.gridClientes.AllowUserToAddRows = false;
            this.gridClientes.AllowUserToDeleteRows = false;
            this.gridClientes.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(180)))), ((int)(((byte)(210)))));
            this.gridClientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridClientes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IDClienteColumn,
            this.seleccionColumn,
            this.dataGridViewTextBoxColumn2,
            this.rfcColumn});
            this.gridClientes.Location = new System.Drawing.Point(168, 0);
            this.gridClientes.MultiSelect = false;
            this.gridClientes.Name = "gridClientes";
            this.gridClientes.Size = new System.Drawing.Size(335, 220);
            this.gridClientes.TabIndex = 5;
            this.gridClientes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridClientes_CellContentClick);
            // 
            // IDClienteColumn
            // 
            this.IDClienteColumn.DataPropertyName = "IDCliente";
            this.IDClienteColumn.HeaderText = "ID Cliente";
            this.IDClienteColumn.Name = "IDClienteColumn";
            this.IDClienteColumn.ReadOnly = true;
            this.IDClienteColumn.Visible = false;
            // 
            // seleccionColumn
            // 
            this.seleccionColumn.HeaderText = "Seleccion";
            this.seleccionColumn.Name = "seleccionColumn";
            this.seleccionColumn.ReadOnly = true;
            this.seleccionColumn.Width = 77;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Nombre";
            this.dataGridViewTextBoxColumn2.HeaderText = "Cliente";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 280;
            // 
            // rfcColumn
            // 
            this.rfcColumn.DataPropertyName = "RFC";
            this.rfcColumn.HeaderText = "RFC";
            this.rfcColumn.Name = "rfcColumn";
            this.rfcColumn.ReadOnly = true;
            this.rfcColumn.Width = 155;
            // 
            // gridContratos
            // 
            this.gridContratos.AllowUserToAddRows = false;
            this.gridContratos.AllowUserToDeleteRows = false;
            this.gridContratos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(180)))), ((int)(((byte)(210)))));
            this.gridContratos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridContratos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idCteColumn,
            this.columnSeleccion,
            this.clienteColumn,
            this.inmuebleColumn,
            this.Conjunto,
            this.contratoColumn,
            this.IDCliente,
            this.Moneda});
            this.gridContratos.Dock = System.Windows.Forms.DockStyle.Left;
            this.gridContratos.Location = new System.Drawing.Point(0, 0);
            this.gridContratos.MultiSelect = false;
            this.gridContratos.Name = "gridContratos";
            this.gridContratos.ReadOnly = true;
            this.gridContratos.RowHeadersWidth = 34;
            this.gridContratos.Size = new System.Drawing.Size(335, 158);
            this.gridContratos.TabIndex = 4;
            this.gridContratos.Visible = false;
            this.gridContratos.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridContratos_CellContentClick);
            // 
            // idCteColumn
            // 
            this.idCteColumn.DataPropertyName = "IDCliente";
            this.idCteColumn.HeaderText = "ID Cliente";
            this.idCteColumn.Name = "idCteColumn";
            this.idCteColumn.ReadOnly = true;
            this.idCteColumn.Visible = false;
            // 
            // columnSeleccion
            // 
            this.columnSeleccion.HeaderText = "Seleccion";
            this.columnSeleccion.Name = "columnSeleccion";
            this.columnSeleccion.ReadOnly = true;
            this.columnSeleccion.Width = 65;
            // 
            // clienteColumn
            // 
            this.clienteColumn.DataPropertyName = "Cliente";
            this.clienteColumn.HeaderText = "Cliente";
            this.clienteColumn.Name = "clienteColumn";
            this.clienteColumn.ReadOnly = true;
            this.clienteColumn.Width = 192;
            // 
            // inmuebleColumn
            // 
            this.inmuebleColumn.DataPropertyName = "Inmueble";
            this.inmuebleColumn.HeaderText = "Inmueble";
            this.inmuebleColumn.Name = "inmuebleColumn";
            this.inmuebleColumn.ReadOnly = true;
            this.inmuebleColumn.Width = 150;
            // 
            // Conjunto
            // 
            this.Conjunto.DataPropertyName = "Conjunto";
            this.Conjunto.HeaderText = "Conjunto";
            this.Conjunto.Name = "Conjunto";
            this.Conjunto.ReadOnly = true;
            this.Conjunto.Width = 150;
            // 
            // contratoColumn
            // 
            this.contratoColumn.DataPropertyName = "IDContrato";
            this.contratoColumn.HeaderText = "IDContrato";
            this.contratoColumn.Name = "contratoColumn";
            this.contratoColumn.ReadOnly = true;
            this.contratoColumn.Visible = false;
            // 
            // IDCliente
            // 
            this.IDCliente.HeaderText = "IDCliente";
            this.IDCliente.Name = "IDCliente";
            this.IDCliente.ReadOnly = true;
            this.IDCliente.Visible = false;
            // 
            // Moneda
            // 
            this.Moneda.DataPropertyName = "Moneda";
            this.Moneda.FillWeight = 80F;
            this.Moneda.HeaderText = "Moneda";
            this.Moneda.Name = "Moneda";
            this.Moneda.ReadOnly = true;
            this.Moneda.Width = 60;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.progreso);
            this.panel4.Controls.Add(this.botonGenerar);
            this.panel4.Controls.Add(this.botonCancelar);
            this.panel4.Controls.Add(this.checkEnviarCorreo);
            this.panel4.Controls.Add(this.checkEnviarImpresora);
            this.panel4.Controls.Add(this.radioExcel);
            this.panel4.Controls.Add(this.radioPdf);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(21, 393);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(670, 76);
            this.panel4.TabIndex = 5;
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(9, 29);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(399, 23);
            this.progreso.TabIndex = 9;
            // 
            // checkEnviarCorreo
            // 
            this.checkEnviarCorreo.AutoSize = true;
            this.checkEnviarCorreo.Location = new System.Drawing.Point(128, 6);
            this.checkEnviarCorreo.Name = "checkEnviarCorreo";
            this.checkEnviarCorreo.Size = new System.Drawing.Size(162, 17);
            this.checkEnviarCorreo.TabIndex = 6;
            this.checkEnviarCorreo.Text = "Enviar por correo electrónico";
            this.checkEnviarCorreo.UseVisualStyleBackColor = true;
            // 
            // checkEnviarImpresora
            // 
            this.checkEnviarImpresora.AutoSize = true;
            this.checkEnviarImpresora.Location = new System.Drawing.Point(9, 6);
            this.checkEnviarImpresora.Name = "checkEnviarImpresora";
            this.checkEnviarImpresora.Size = new System.Drawing.Size(113, 17);
            this.checkEnviarImpresora.TabIndex = 5;
            this.checkEnviarImpresora.Text = "Enviar a impresora";
            this.checkEnviarImpresora.UseVisualStyleBackColor = true;
            this.checkEnviarImpresora.CheckedChanged += new System.EventHandler(this.checkEnviarImpresora_CheckedChanged);
            // 
            // radioExcel
            // 
            this.radioExcel.AutoSize = true;
            this.radioExcel.Location = new System.Drawing.Point(357, 5);
            this.radioExcel.Name = "radioExcel";
            this.radioExcel.Size = new System.Drawing.Size(51, 17);
            this.radioExcel.TabIndex = 4;
            this.radioExcel.Text = "Excel";
            this.radioExcel.UseVisualStyleBackColor = true;
            // 
            // radioPdf
            // 
            this.radioPdf.AutoSize = true;
            this.radioPdf.Checked = true;
            this.radioPdf.Location = new System.Drawing.Point(305, 5);
            this.radioPdf.Name = "radioPdf";
            this.radioPdf.Size = new System.Drawing.Size(46, 17);
            this.radioPdf.TabIndex = 3;
            this.radioPdf.TabStop = true;
            this.radioPdf.Text = "PDF";
            this.radioPdf.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkTodosClientes);
            this.panel1.Controls.Add(this.radioDolar);
            this.panel1.Controls.Add(this.radioMonedaLocal);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(21, 360);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(670, 27);
            this.panel1.TabIndex = 6;
            // 
            // checkTodosClientes
            // 
            this.checkTodosClientes.AutoSize = true;
            this.checkTodosClientes.Location = new System.Drawing.Point(9, 5);
            this.checkTodosClientes.Name = "checkTodosClientes";
            this.checkTodosClientes.Size = new System.Drawing.Size(166, 17);
            this.checkTodosClientes.TabIndex = 3;
            this.checkTodosClientes.Text = "Seleccionar todos los clientes";
            this.checkTodosClientes.UseVisualStyleBackColor = true;
            this.checkTodosClientes.CheckedChanged += new System.EventHandler(this.checkTodosClientes_CheckedChanged);
            // 
            // radioDolar
            // 
            this.radioDolar.AutoSize = true;
            this.radioDolar.Location = new System.Drawing.Point(501, 5);
            this.radioDolar.Name = "radioDolar";
            this.radioDolar.Size = new System.Drawing.Size(61, 17);
            this.radioDolar.TabIndex = 2;
            this.radioDolar.TabStop = true;
            this.radioDolar.Text = "Dolares";
            this.radioDolar.UseVisualStyleBackColor = true;
            this.radioDolar.Visible = false;
            // 
            // radioMonedaLocal
            // 
            this.radioMonedaLocal.AutoSize = true;
            this.radioMonedaLocal.Checked = true;
            this.radioMonedaLocal.Location = new System.Drawing.Point(407, 5);
            this.radioMonedaLocal.Name = "radioMonedaLocal";
            this.radioMonedaLocal.Size = new System.Drawing.Size(89, 17);
            this.radioMonedaLocal.TabIndex = 1;
            this.radioMonedaLocal.TabStop = true;
            this.radioMonedaLocal.Text = "Moneda local";
            this.radioMonedaLocal.UseVisualStyleBackColor = true;
            this.radioMonedaLocal.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(303, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Moneda de reporte:";
            this.label1.Visible = false;
            // 
            // botonBuscar
            // 
            this.botonBuscar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
            this.botonBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonBuscar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
            this.botonBuscar.Imagen = global::GestorReportes.Properties.Resources.buscarCh;
            this.botonBuscar.Location = new System.Drawing.Point(592, 5);
            this.botonBuscar.Name = "botonBuscar";
            this.botonBuscar.Size = new System.Drawing.Size(75, 75);
            this.botonBuscar.TabIndex = 5;
            this.botonBuscar.Texto = "Buscar";
            this.botonBuscar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonBuscar_ControlClicked);
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(416, -1);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 8;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_Click);
            this.botonGenerar.Click += new System.EventHandler(this.botonGenerar_Click);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(497, -1);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 7;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_ControlClicked);
            // 
            // Frm_RecordatorioCobranza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
            this.ClientSize = new System.Drawing.Size(714, 472);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_RecordatorioCobranza";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Recordatorio de Pago";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_CobranzaRenta_FormClosing);
            this.Load += new System.EventHandler(this.Frm_CobranzaRenta_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.groupBoxContratos.ResumeLayout(false);
            this.groupBoxContratos.PerformLayout();
            this.groupBoxTipo.ResumeLayout(false);
            this.groupBoxTipo.PerformLayout();
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridClientes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridContratos)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker workerReporte;
        private System.Windows.Forms.PrintDialog printDialog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DateTimePicker dateFin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox comboInmobiliaria;
        private Controls.Ctrl_Opcion botonBuscar;
        private System.Windows.Forms.TextBox textoBuscar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkTodosClientes;
        private System.Windows.Forms.RadioButton radioDolar;
        private System.Windows.Forms.RadioButton radioMonedaLocal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView gridContratos;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioTodos;
        private System.Windows.Forms.RadioButton radioVigente;
        private System.Windows.Forms.RadioButton radioButtonCliente;
        private System.Windows.Forms.RadioButton radioButtonContrato;
        private System.Windows.Forms.GroupBox groupBoxTipo;
        private System.Windows.Forms.GroupBox groupBoxContratos;
        private System.Windows.Forms.DataGridViewTextBoxColumn idCteColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnSeleccion;
        private System.Windows.Forms.DataGridViewTextBoxColumn clienteColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn inmuebleColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Conjunto;
        private System.Windows.Forms.DataGridViewTextBoxColumn contratoColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn IDCliente;
        private System.Windows.Forms.DataGridViewTextBoxColumn Moneda;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.DataGridView gridClientes;
        private System.Windows.Forms.DataGridViewTextBoxColumn IDClienteColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn seleccionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn rfcColumn;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ProgressBar progreso;
        private Controls.Ctrl_Opcion botonGenerar;
        private Controls.Ctrl_Opcion botonCancelar;
        private System.Windows.Forms.CheckBox checkEnviarCorreo;
        private System.Windows.Forms.CheckBox checkEnviarImpresora;
        private System.Windows.Forms.RadioButton radioExcel;
        private System.Windows.Forms.RadioButton radioPdf;
        private System.Windows.Forms.RichTextBox rtbBodymnj;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label2;
    }
}