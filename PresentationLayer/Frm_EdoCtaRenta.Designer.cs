namespace GestorReportes.PresentationLayer
{
    partial class Frm_EdoCtaRenta
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboInmobiliaria = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dateFin = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dateInicio = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.botonBuscar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.radioTodos = new System.Windows.Forms.RadioButton();
            this.radioVigente = new System.Windows.Forms.RadioButton();
            this.textoBuscar = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gridContratos = new System.Windows.Forms.DataGridView();
            this.columnSeleccion = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnCliente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnInmueble = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnConjunto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contratoColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idclienteColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel4 = new System.Windows.Forms.Panel();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.checkEnviarCorreo = new System.Windows.Forms.CheckBox();
            this.checkEnviarImpresora = new System.Windows.Forms.CheckBox();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.radioPdf = new System.Windows.Forms.RadioButton();
            this.printDialog = new System.Windows.Forms.PrintDialog();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridContratos)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.gridContratos, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(644, 472);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.comboInmobiliaria);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(35, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(573, 27);
            this.panel1.TabIndex = 0;
            // 
            // comboInmobiliaria
            // 
            this.comboInmobiliaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInmobiliaria.FormattingEnabled = true;
            this.comboInmobiliaria.Location = new System.Drawing.Point(81, 3);
            this.comboInmobiliaria.Name = "comboInmobiliaria";
            this.comboInmobiliaria.Size = new System.Drawing.Size(489, 21);
            this.comboInmobiliaria.TabIndex = 1;
            this.comboInmobiliaria.SelectedIndexChanged += new System.EventHandler(this.comboInmobiliaria_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Inmobiliaria:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dateFin);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.dateInicio);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(35, 36);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(573, 27);
            this.panel2.TabIndex = 1;
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
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.panel3.Controls.Add(this.botonBuscar);
            this.panel3.Controls.Add(this.radioTodos);
            this.panel3.Controls.Add(this.radioVigente);
            this.panel3.Controls.Add(this.textoBuscar);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(35, 69);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(573, 83);
            this.panel3.TabIndex = 2;
            // 
            // botonBuscar
            // 
            this.botonBuscar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonBuscar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonBuscar.Imagen = global::GestorReportes.Properties.Resources.buscarCh;
            this.botonBuscar.Location = new System.Drawing.Point(495, 5);
            this.botonBuscar.Name = "botonBuscar";
            this.botonBuscar.Size = new System.Drawing.Size(75, 75);
            this.botonBuscar.TabIndex = 5;
            this.botonBuscar.Texto = "Buscar";
            this.botonBuscar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonBuscar_Click);
            // 
            // radioTodos
            // 
            this.radioTodos.AutoSize = true;
            this.radioTodos.Checked = true;
            this.radioTodos.Location = new System.Drawing.Point(424, 37);
            this.radioTodos.Name = "radioTodos";
            this.radioTodos.Size = new System.Drawing.Size(55, 17);
            this.radioTodos.TabIndex = 3;
            this.radioTodos.TabStop = true;
            this.radioTodos.Text = "Todos";
            this.radioTodos.UseVisualStyleBackColor = true;
            // 
            // radioVigente
            // 
            this.radioVigente.AutoSize = true;
            this.radioVigente.Location = new System.Drawing.Point(424, 14);
            this.radioVigente.Name = "radioVigente";
            this.radioVigente.Size = new System.Drawing.Size(66, 17);
            this.radioVigente.TabIndex = 2;
            this.radioVigente.Text = "Vigentes";
            this.radioVigente.UseVisualStyleBackColor = true;
            // 
            // textoBuscar
            // 
            this.textoBuscar.Location = new System.Drawing.Point(96, 37);
            this.textoBuscar.Name = "textoBuscar";
            this.textoBuscar.Size = new System.Drawing.Size(322, 20);
            this.textoBuscar.TabIndex = 1;
            this.textoBuscar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textoBuscar_KeyPress);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 47);
            this.label4.TabIndex = 0;
            this.label4.Text = "Buscar contrato por cliente o inmueble:";
            // 
            // gridContratos
            // 
            this.gridContratos.AllowUserToAddRows = false;
            this.gridContratos.AllowUserToDeleteRows = false;
            this.gridContratos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(180)))), ((int)(((byte)(210)))));
            this.gridContratos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridContratos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnSeleccion,
            this.columnCliente,
            this.columnInmueble,
            this.columnConjunto,
            this.contratoColumn,
            this.idclienteColumn});
            this.gridContratos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridContratos.Location = new System.Drawing.Point(35, 158);
            this.gridContratos.MultiSelect = false;
            this.gridContratos.Name = "gridContratos";
            this.gridContratos.ReadOnly = true;
            this.gridContratos.Size = new System.Drawing.Size(573, 220);
            this.gridContratos.TabIndex = 3;
            this.gridContratos.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridContratos_CellContentClick);
            // 
            // columnSeleccion
            // 
            this.columnSeleccion.HeaderText = "Seleccion";
            this.columnSeleccion.Name = "columnSeleccion";
            this.columnSeleccion.ReadOnly = true;
            this.columnSeleccion.Width = 75;
            // 
            // columnCliente
            // 
            this.columnCliente.DataPropertyName = "Cliente";
            this.columnCliente.HeaderText = "Cliente";
            this.columnCliente.Name = "columnCliente";
            this.columnCliente.ReadOnly = true;
            this.columnCliente.Width = 150;
            // 
            // columnInmueble
            // 
            this.columnInmueble.DataPropertyName = "Inmueble";
            this.columnInmueble.HeaderText = "Inmueble";
            this.columnInmueble.Name = "columnInmueble";
            this.columnInmueble.ReadOnly = true;
            this.columnInmueble.Width = 150;
            // 
            // columnConjunto
            // 
            this.columnConjunto.DataPropertyName = "Conjunto";
            this.columnConjunto.HeaderText = "Conjunto";
            this.columnConjunto.Name = "columnConjunto";
            this.columnConjunto.ReadOnly = true;
            this.columnConjunto.Width = 150;
            // 
            // contratoColumn
            // 
            this.contratoColumn.DataPropertyName = "IDContrato";
            this.contratoColumn.HeaderText = "IDContrato";
            this.contratoColumn.Name = "contratoColumn";
            this.contratoColumn.ReadOnly = true;
            this.contratoColumn.Visible = false;
            // 
            // idclienteColumn
            // 
            this.idclienteColumn.DataPropertyName = "IDCliente";
            this.idclienteColumn.HeaderText = "IDCliente";
            this.idclienteColumn.Name = "idclienteColumn";
            this.idclienteColumn.ReadOnly = true;
            this.idclienteColumn.Visible = false;
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
            this.panel4.Location = new System.Drawing.Point(35, 384);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(573, 85);
            this.panel4.TabIndex = 4;
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(9, 43);
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
            this.botonGenerar.Location = new System.Drawing.Point(414, 5);
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
            this.botonCancelar.Location = new System.Drawing.Point(495, 5);
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
            // Frm_EdoCtaRenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
            this.ClientSize = new System.Drawing.Size(644, 472);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(650, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(650, 500);
            this.Name = "Frm_EdoCtaRenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Estado de cuenta de renta";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_EdoCtaRenta_FormClosing);
            this.Load += new System.EventHandler(this.Frm_EdoCtaRenta_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridContratos)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboInmobiliaria;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DateTimePicker dateFin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateInicio;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox textoBuscar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioTodos;
        private System.Windows.Forms.RadioButton radioVigente;
        private System.Windows.Forms.DataGridView gridContratos;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.RadioButton radioExcel;
        private System.Windows.Forms.RadioButton radioPdf;
        private System.Windows.Forms.CheckBox checkEnviarImpresora;
        private System.Windows.Forms.PrintDialog printDialog;
        private System.Windows.Forms.CheckBox checkEnviarCorreo;
        private Controls.Ctrl_Opcion botonBuscar;
        private System.Windows.Forms.ProgressBar progreso;
        private Controls.Ctrl_Opcion botonGenerar;
        private Controls.Ctrl_Opcion botonCancelar;
        private System.ComponentModel.BackgroundWorker workerReporte;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnSeleccion;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnCliente;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnInmueble;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnConjunto;
        private System.Windows.Forms.DataGridViewTextBoxColumn contratoColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn idclienteColumn;
    }
}