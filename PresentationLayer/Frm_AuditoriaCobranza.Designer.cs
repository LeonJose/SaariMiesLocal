namespace GestorReportes.PresentationLayer
{
    partial class Frm_AuditoriaCobranza
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
            this.panel3 = new System.Windows.Forms.Panel();
            this.textoBuscar = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.radioPdf = new System.Windows.Forms.RadioButton();
            this.gridClientes = new System.Windows.Forms.DataGridView();
            this.idCteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.seleccionColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clienteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rfcColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel5 = new System.Windows.Forms.Panel();
            this.comboEmpresa = new System.Windows.Forms.ComboBox();
            this.checkEmpresa = new System.Windows.Forms.CheckBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.botonBuscar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridClientes)).BeginInit();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.gridClientes, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel5, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel6, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel7, 2, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 58F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(630, 507);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dateFin);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(34, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(372, 39);
            this.panel2.TabIndex = 2;
            // 
            // dateFin
            // 
            this.dateFin.Location = new System.Drawing.Point(93, 4);
            this.dateFin.Name = "dateFin";
            this.dateFin.Size = new System.Drawing.Size(200, 20);
            this.dateFin.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "A la fecha:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.textoBuscar);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(34, 48);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(372, 39);
            this.panel3.TabIndex = 3;
            // 
            // textoBuscar
            // 
            this.textoBuscar.Location = new System.Drawing.Point(93, 4);
            this.textoBuscar.Name = "textoBuscar";
            this.textoBuscar.Size = new System.Drawing.Size(284, 20);
            this.textoBuscar.TabIndex = 1;
            this.textoBuscar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textoBuscar_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Buscar cliente:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.progreso);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(34, 462);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(372, 42);
            this.panel4.TabIndex = 5;
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(13, 13);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(364, 23);
            this.progreso.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.radioExcel);
            this.panel1.Controls.Add(this.radioPdf);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(34, 417);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(372, 39);
            this.panel1.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Formato:";
            // 
            // radioExcel
            // 
            this.radioExcel.AutoSize = true;
            this.radioExcel.Location = new System.Drawing.Point(112, 6);
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
            this.radioPdf.Location = new System.Drawing.Point(60, 6);
            this.radioPdf.Name = "radioPdf";
            this.radioPdf.Size = new System.Drawing.Size(46, 17);
            this.radioPdf.TabIndex = 3;
            this.radioPdf.TabStop = true;
            this.radioPdf.Text = "PDF";
            this.radioPdf.UseVisualStyleBackColor = true;
            // 
            // gridClientes
            // 
            this.gridClientes.AllowUserToAddRows = false;
            this.gridClientes.AllowUserToDeleteRows = false;
            this.gridClientes.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
            this.gridClientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridClientes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idCteColumn,
            this.seleccionColumn,
            this.clienteColumn,
            this.rfcColumn});
            this.tableLayoutPanel1.SetColumnSpan(this.gridClientes, 2);
            this.gridClientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridClientes.Location = new System.Drawing.Point(34, 123);
            this.gridClientes.MultiSelect = false;
            this.gridClientes.Name = "gridClientes";
            this.gridClientes.Size = new System.Drawing.Size(561, 288);
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
            this.clienteColumn.Width = 275;
            // 
            // rfcColumn
            // 
            this.rfcColumn.DataPropertyName = "RFC";
            this.rfcColumn.HeaderText = "RFC";
            this.rfcColumn.Name = "rfcColumn";
            this.rfcColumn.ReadOnly = true;
            this.rfcColumn.Width = 155;
            // 
            // panel5
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel5, 2);
            this.panel5.Controls.Add(this.comboEmpresa);
            this.panel5.Controls.Add(this.checkEmpresa);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(34, 93);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(561, 24);
            this.panel5.TabIndex = 7;
            // 
            // comboEmpresa
            // 
            this.comboEmpresa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEmpresa.Enabled = false;
            this.comboEmpresa.FormattingEnabled = true;
            this.comboEmpresa.Location = new System.Drawing.Point(214, 2);
            this.comboEmpresa.Name = "comboEmpresa";
            this.comboEmpresa.Size = new System.Drawing.Size(356, 21);
            this.comboEmpresa.TabIndex = 1;
            // 
            // checkEmpresa
            // 
            this.checkEmpresa.AutoSize = true;
            this.checkEmpresa.Location = new System.Drawing.Point(13, 4);
            this.checkEmpresa.Name = "checkEmpresa";
            this.checkEmpresa.Size = new System.Drawing.Size(195, 17);
            this.checkEmpresa.TabIndex = 0;
            this.checkEmpresa.Text = "Filtrar facturas solo para la empresa:";
            this.checkEmpresa.UseVisualStyleBackColor = true;
            this.checkEmpresa.CheckedChanged += new System.EventHandler(this.checkEmpresa_CheckedChanged);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.botonBuscar);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(412, 3);
            this.panel6.Name = "panel6";
            this.tableLayoutPanel1.SetRowSpan(this.panel6, 2);
            this.panel6.Size = new System.Drawing.Size(183, 84);
            this.panel6.TabIndex = 8;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.botonGenerar);
            this.panel7.Controls.Add(this.botonCancelar);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(412, 417);
            this.panel7.Name = "panel7";
            this.tableLayoutPanel1.SetRowSpan(this.panel7, 2);
            this.panel7.Size = new System.Drawing.Size(183, 87);
            this.panel7.TabIndex = 9;
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // botonBuscar
            // 
            this.botonBuscar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonBuscar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonBuscar.Imagen = global::GestorReportes.Properties.Resources.buscarCh;
            this.botonBuscar.Location = new System.Drawing.Point(28, 8);
            this.botonBuscar.Name = "botonBuscar";
            this.botonBuscar.Size = new System.Drawing.Size(75, 75);
            this.botonBuscar.TabIndex = 0;
            this.botonBuscar.Texto = "Buscar";
            this.botonBuscar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonBuscar_ControlClicked);
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(28, 7);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 3;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_Click);
            this.botonGenerar.Load += new System.EventHandler(this.botonGenerar_Load);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(109, 7);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 2;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_Click);
            // 
            // Frm_AuditoriaCobranza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(630, 507);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(650, 550);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(650, 550);
            this.Name = "Frm_AuditoriaCobranza";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auditoria de cobranza";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_AuditoriaCobranza_FormClosing);
            this.Load += new System.EventHandler(this.Frm_AuditoriaCobranza_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridClientes)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DateTimePicker dateFin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox textoBuscar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView gridClientes;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ComboBox comboEmpresa;
        private System.Windows.Forms.CheckBox checkEmpresa;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioExcel;
        private System.Windows.Forms.RadioButton radioPdf;
        private System.Windows.Forms.Panel panel6;
        private Controls.Ctrl_Opcion botonBuscar;
        private System.Windows.Forms.DataGridViewTextBoxColumn idCteColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn seleccionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn clienteColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rfcColumn;
        private System.Windows.Forms.Panel panel7;
        private Controls.Ctrl_Opcion botonGenerar;
        private Controls.Ctrl_Opcion botonCancelar;
        private System.Windows.Forms.ProgressBar progreso;
        private System.ComponentModel.BackgroundWorker workerReporte;

    }
}