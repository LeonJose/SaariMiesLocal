namespace GestorReportes.PresentationLayer
{
    partial class Frm_RepMant
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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.Cmb_Usuario = new System.Windows.Forms.ComboBox();
            this.Rb_Todos = new System.Windows.Forms.RadioButton();
            this.Rb_Usuario = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Cmb_Stat = new System.Windows.Forms.ComboBox();
            this.Rb_Stat = new System.Windows.Forms.RadioButton();
            this.Rb_AllSta = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdBtnCalendar = new System.Windows.Forms.RadioButton();
            this.rdBtnDetallado = new System.Windows.Forms.RadioButton();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.radioPDF = new System.Windows.Forms.RadioButton();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(258, 240);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Fecha Final";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 240);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Fecha Inicial";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(261, 257);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker2.TabIndex = 12;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(41, 257);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Inmobiliaria:";
            // 
            // comboBox1
            // 
            this.comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(91, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(397, 21);
            this.comboBox1.TabIndex = 9;
            this.comboBox1.SelectedValueChanged += new System.EventHandler(this.comboBox1_SelectedValueChanged);
            // 
            // Cmb_Usuario
            // 
            this.Cmb_Usuario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmb_Usuario.FormattingEnabled = true;
            this.Cmb_Usuario.Location = new System.Drawing.Point(12, 42);
            this.Cmb_Usuario.Name = "Cmb_Usuario";
            this.Cmb_Usuario.Size = new System.Drawing.Size(424, 21);
            this.Cmb_Usuario.TabIndex = 18;
            // 
            // Rb_Todos
            // 
            this.Rb_Todos.AutoSize = true;
            this.Rb_Todos.Checked = true;
            this.Rb_Todos.Location = new System.Drawing.Point(13, 19);
            this.Rb_Todos.Name = "Rb_Todos";
            this.Rb_Todos.Size = new System.Drawing.Size(55, 17);
            this.Rb_Todos.TabIndex = 20;
            this.Rb_Todos.TabStop = true;
            this.Rb_Todos.Text = "Todos";
            this.Rb_Todos.UseVisualStyleBackColor = true;
            this.Rb_Todos.CheckedChanged += new System.EventHandler(this.Rb_Todos_CheckedChanged);
            // 
            // Rb_Usuario
            // 
            this.Rb_Usuario.AutoSize = true;
            this.Rb_Usuario.Location = new System.Drawing.Point(317, 19);
            this.Rb_Usuario.Name = "Rb_Usuario";
            this.Rb_Usuario.Size = new System.Drawing.Size(114, 17);
            this.Rb_Usuario.TabIndex = 21;
            this.Rb_Usuario.Text = "Selecionar Usuario";
            this.Rb_Usuario.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Cmb_Usuario);
            this.groupBox1.Controls.Add(this.Rb_Usuario);
            this.groupBox1.Controls.Add(this.Rb_Todos);
            this.groupBox1.Location = new System.Drawing.Point(29, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(459, 74);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Cmb_Stat);
            this.groupBox2.Controls.Add(this.Rb_Stat);
            this.groupBox2.Controls.Add(this.Rb_AllSta);
            this.groupBox2.Location = new System.Drawing.Point(29, 119);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(459, 74);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            // 
            // Cmb_Stat
            // 
            this.Cmb_Stat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmb_Stat.FormattingEnabled = true;
            this.Cmb_Stat.Items.AddRange(new object[] {
            "Pendientes",
            "Terminados"});
            this.Cmb_Stat.Location = new System.Drawing.Point(13, 42);
            this.Cmb_Stat.Name = "Cmb_Stat";
            this.Cmb_Stat.Size = new System.Drawing.Size(423, 21);
            this.Cmb_Stat.TabIndex = 2;
            this.Cmb_Stat.SelectedIndexChanged += new System.EventHandler(this.Cmb_Stat_SelectedIndexChanged);
            // 
            // Rb_Stat
            // 
            this.Rb_Stat.AutoSize = true;
            this.Rb_Stat.Location = new System.Drawing.Point(317, 19);
            this.Rb_Stat.Name = "Rb_Stat";
            this.Rb_Stat.Size = new System.Drawing.Size(119, 17);
            this.Rb_Stat.TabIndex = 1;
            this.Rb_Stat.Text = "Seleccionar Estatus";
            this.Rb_Stat.UseVisualStyleBackColor = true;
            // 
            // Rb_AllSta
            // 
            this.Rb_AllSta.AutoSize = true;
            this.Rb_AllSta.Checked = true;
            this.Rb_AllSta.Location = new System.Drawing.Point(12, 19);
            this.Rb_AllSta.Name = "Rb_AllSta";
            this.Rb_AllSta.Size = new System.Drawing.Size(55, 17);
            this.Rb_AllSta.TabIndex = 0;
            this.Rb_AllSta.TabStop = true;
            this.Rb_AllSta.Text = "Todos";
            this.Rb_AllSta.UseVisualStyleBackColor = true;
            this.Rb_AllSta.CheckedChanged += new System.EventHandler(this.Rb_AllSta_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdBtnCalendar);
            this.groupBox3.Controls.Add(this.rdBtnDetallado);
            this.groupBox3.Location = new System.Drawing.Point(29, 199);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(459, 38);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Estilo de reporte:";
            // 
            // rdBtnCalendar
            // 
            this.rdBtnCalendar.AutoSize = true;
            this.rdBtnCalendar.Location = new System.Drawing.Point(232, 15);
            this.rdBtnCalendar.Name = "rdBtnCalendar";
            this.rdBtnCalendar.Size = new System.Drawing.Size(75, 17);
            this.rdBtnCalendar.TabIndex = 1;
            this.rdBtnCalendar.Text = "Calendario";
            this.rdBtnCalendar.UseVisualStyleBackColor = true;
            this.rdBtnCalendar.CheckedChanged += new System.EventHandler(this.rdBtnCalendar_CheckedChanged);
            // 
            // rdBtnDetallado
            // 
            this.rdBtnDetallado.AutoSize = true;
            this.rdBtnDetallado.Checked = true;
            this.rdBtnDetallado.Location = new System.Drawing.Point(128, 15);
            this.rdBtnDetallado.Name = "rdBtnDetallado";
            this.rdBtnDetallado.Size = new System.Drawing.Size(70, 17);
            this.rdBtnDetallado.TabIndex = 0;
            this.rdBtnDetallado.TabStop = true;
            this.rdBtnDetallado.Text = "Detallado";
            this.rdBtnDetallado.UseVisualStyleBackColor = true;
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(29, 318);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(297, 23);
            this.progreso.TabIndex = 27;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 284);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Formato:";
            // 
            // radioPDF
            // 
            this.radioPDF.AutoSize = true;
            this.radioPDF.Checked = true;
            this.radioPDF.Location = new System.Drawing.Point(96, 282);
            this.radioPDF.Name = "radioPDF";
            this.radioPDF.Size = new System.Drawing.Size(46, 17);
            this.radioPDF.TabIndex = 29;
            this.radioPDF.TabStop = true;
            this.radioPDF.Text = "PDF";
            this.radioPDF.UseVisualStyleBackColor = true;
            // 
            // radioExcel
            // 
            this.radioExcel.AutoSize = true;
            this.radioExcel.Location = new System.Drawing.Point(148, 282);
            this.radioExcel.Name = "radioExcel";
            this.radioExcel.Size = new System.Drawing.Size(51, 17);
            this.radioExcel.TabIndex = 30;
            this.radioExcel.Text = "Excel";
            this.radioExcel.UseVisualStyleBackColor = true;
            this.radioExcel.Visible = false;
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(332, 283);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 26;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_ControlClicked);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(413, 283);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 25;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.Btn_Cancelar_Click);
            // 
            // Frm_RepMant
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(500, 363);
            this.Controls.Add(this.radioExcel);
            this.Controls.Add(this.radioPDF);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_RepMant";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Programa de Mantenimiento";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_RepMant_FormClosing);
            this.Load += new System.EventHandler(this.Frm_RepMant_Load_1);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox Cmb_Usuario;
        private System.Windows.Forms.RadioButton Rb_Todos;
        private System.Windows.Forms.RadioButton Rb_Usuario;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox Cmb_Stat;
        private System.Windows.Forms.RadioButton Rb_Stat;
        private System.Windows.Forms.RadioButton Rb_AllSta;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rdBtnCalendar;
        private System.Windows.Forms.RadioButton rdBtnDetallado;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonGenerar;
        private System.Windows.Forms.ProgressBar progreso;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioPDF;
        private System.Windows.Forms.RadioButton radioExcel;
        private System.ComponentModel.BackgroundWorker workerReporte;

    }
}