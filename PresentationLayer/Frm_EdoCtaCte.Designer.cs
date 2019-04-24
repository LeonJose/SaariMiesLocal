namespace GestorReportes.PresentationLayer
{
    partial class Frm_EdoCtaCte
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dateFin = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dateInicio = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.comboEmpresa = new System.Windows.Forms.ComboBox();
            this.botonBuscar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.checkEmpresa = new System.Windows.Forms.CheckBox();
            this.textoBuscar = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.checkEnviarCorreo = new System.Windows.Forms.CheckBox();
            this.checkEnviarImpresora = new System.Windows.Forms.CheckBox();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.radioPdf = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkTodosClientes = new System.Windows.Forms.CheckBox();
            this.radioDolar = new System.Windows.Forms.RadioButton();
            this.radioMonedaLocal = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.gridClientes = new System.Windows.Forms.DataGridView();
            this.idCteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.seleccionColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clienteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rfcColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.printDialog = new System.Windows.Forms.PrintDialog();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridClientes)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.gridClientes, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(644, 522);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dateFin);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.dateInicio);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(35, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(573, 30);
            this.panel2.TabIndex = 2;
            // 
            // dateFin
            // 
            this.dateFin.Location = new System.Drawing.Point(370, 3);
            this.dateFin.Name = "dateFin";
            this.dateFin.Size = new System.Drawing.Size(200, 20);
            this.dateFin.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(303, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fecha final:";
            this.label3.Visible = false;
            // 
            // dateInicio
            // 
            this.dateInicio.Location = new System.Drawing.Point(81, 3);
            this.dateInicio.Name = "dateInicio";
            this.dateInicio.Size = new System.Drawing.Size(200, 20);
            this.dateInicio.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Fecha inicial:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.comboEmpresa);
            this.panel3.Controls.Add(this.botonBuscar);
            this.panel3.Controls.Add(this.checkEmpresa);
            this.panel3.Controls.Add(this.textoBuscar);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(35, 39);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(573, 93);
            this.panel3.TabIndex = 3;
            // 
            // comboEmpresa
            // 
            this.comboEmpresa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEmpresa.Enabled = false;
            this.comboEmpresa.FormattingEnabled = true;
            this.comboEmpresa.Location = new System.Drawing.Point(210, 50);
            this.comboEmpresa.Name = "comboEmpresa";
            this.comboEmpresa.Size = new System.Drawing.Size(279, 21);
            this.comboEmpresa.TabIndex = 1;
            // 
            // botonBuscar
            // 
            this.botonBuscar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonBuscar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonBuscar.Imagen = global::GestorReportes.Properties.Resources.buscarCh;
            this.botonBuscar.Location = new System.Drawing.Point(495, 10);
            this.botonBuscar.Name = "botonBuscar";
            this.botonBuscar.Size = new System.Drawing.Size(75, 75);
            this.botonBuscar.TabIndex = 5;
            this.botonBuscar.Texto = "Buscar";
            this.botonBuscar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonBuscar_Click);
            // 
            // checkEmpresa
            // 
            this.checkEmpresa.AutoSize = true;
            this.checkEmpresa.Location = new System.Drawing.Point(9, 52);
            this.checkEmpresa.Name = "checkEmpresa";
            this.checkEmpresa.Size = new System.Drawing.Size(195, 17);
            this.checkEmpresa.TabIndex = 0;
            this.checkEmpresa.Text = "Filtrar facturas solo para la empresa:";
            this.checkEmpresa.UseVisualStyleBackColor = true;
            this.checkEmpresa.CheckedChanged += new System.EventHandler(this.checkEmpresa_CheckedChanged);
            // 
            // textoBuscar
            // 
            this.textoBuscar.Location = new System.Drawing.Point(93, 13);
            this.textoBuscar.Name = "textoBuscar";
            this.textoBuscar.Size = new System.Drawing.Size(396, 20);
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
            this.panel4.Location = new System.Drawing.Point(35, 424);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(573, 95);
            this.panel4.TabIndex = 5;
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(9, 48);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(399, 23);
            this.progreso.TabIndex = 9;
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(414, 7);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 8;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_Click);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(495, 7);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 7;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_Click);
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
            this.panel1.Location = new System.Drawing.Point(35, 388);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(573, 30);
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
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(303, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Moneda de reporte:";
            // 
            // gridClientes
            // 
            this.gridClientes.AllowUserToAddRows = false;
            this.gridClientes.AllowUserToDeleteRows = false;
            this.gridClientes.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(180)))), ((int)(((byte)(210)))));
            this.gridClientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridClientes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idCteColumn,
            this.seleccionColumn,
            this.clienteColumn,
            this.rfcColumn});
            this.gridClientes.Location = new System.Drawing.Point(35, 138);
            this.gridClientes.MultiSelect = false;
            this.gridClientes.Name = "gridClientes";
            this.gridClientes.Size = new System.Drawing.Size(573, 244);
            this.gridClientes.TabIndex = 4;
            this.gridClientes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridClientes_CellContentClick);
            // 
            // idCteColumn
            // 
            this.idCteColumn.DataPropertyName = "IDCliente";
            this.idCteColumn.HeaderText = "ID Cliente";
            this.idCteColumn.Name = "idCteColumn";
            this.idCteColumn.ReadOnly = true;
            this.idCteColumn.Visible = false;
            // 
            // seleccionColumn
            // 
            this.seleccionColumn.HeaderText = "Seleccion";
            this.seleccionColumn.Name = "seleccionColumn";
            // 
            // clienteColumn
            // 
            this.clienteColumn.DataPropertyName = "Nombre";
            this.clienteColumn.HeaderText = "Cliente";
            this.clienteColumn.Name = "clienteColumn";
            this.clienteColumn.ReadOnly = true;
            this.clienteColumn.Width = 270;
            // 
            // rfcColumn
            // 
            this.rfcColumn.DataPropertyName = "RFC";
            this.rfcColumn.HeaderText = "RFC";
            this.rfcColumn.Name = "rfcColumn";
            this.rfcColumn.ReadOnly = true;
            this.rfcColumn.Width = 150;
            // 
            // printDialog
            // 
            this.printDialog.UseEXDialog = true;
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // Frm_EdoCtaCte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(644, 522);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(650, 550);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(650, 550);
            this.Name = "Frm_EdoCtaCte";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Estado de cuenta de cliente";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_EdoCtaCte_FormClosing);
            this.Load += new System.EventHandler(this.Frm_EdoCtaCte_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridClientes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DateTimePicker dateFin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateInicio;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox textoBuscar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView gridClientes;
        private System.Windows.Forms.DataGridViewTextBoxColumn idCteColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn seleccionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn clienteColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rfcColumn;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.CheckBox checkEnviarCorreo;
        private System.Windows.Forms.CheckBox checkEnviarImpresora;
        private System.Windows.Forms.RadioButton radioExcel;
        private System.Windows.Forms.RadioButton radioPdf;
        private System.Windows.Forms.PrintDialog printDialog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioDolar;
        private System.Windows.Forms.RadioButton radioMonedaLocal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboEmpresa;
        private System.Windows.Forms.CheckBox checkEmpresa;
        private Controls.Ctrl_Opcion botonBuscar;
        private Controls.Ctrl_Opcion botonGenerar;
        private Controls.Ctrl_Opcion botonCancelar;
        private System.Windows.Forms.ProgressBar progreso;
        private System.ComponentModel.BackgroundWorker workerReporte;
        private System.Windows.Forms.CheckBox checkTodosClientes;
    }
}