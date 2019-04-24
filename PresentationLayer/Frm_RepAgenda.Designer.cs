namespace GestorReportes.PresentationLayer
{
    partial class Frm_RepAgenda
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
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.Rb_Cliente = new System.Windows.Forms.RadioButton();
            this.Rb_Todos = new System.Windows.Forms.RadioButton();
            this.Cmb_Usuarios = new System.Windows.Forms.ComboBox();
            this.Rb_AllUser = new System.Windows.Forms.RadioButton();
            this.Rb_Usuario = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Cmb_Stat = new System.Windows.Forms.ComboBox();
            this.Rb_Allst = new System.Windows.Forms.RadioButton();
            this.Rb_Stat = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdBtnCalendar = new System.Windows.Forms.RadioButton();
            this.rdBtnDetallado = new System.Windows.Forms.RadioButton();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.radioPDF = new System.Windows.Forms.RadioButton();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(243, 298);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker2.TabIndex = 16;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(28, 298);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(247, 282);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Fecha Final:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 282);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Fecha Inicial:";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(13, 42);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(390, 21);
            this.comboBox1.TabIndex = 12;
            // 
            // Rb_Cliente
            // 
            this.Rb_Cliente.AutoSize = true;
            this.Rb_Cliente.Location = new System.Drawing.Point(283, 19);
            this.Rb_Cliente.Name = "Rb_Cliente";
            this.Rb_Cliente.Size = new System.Drawing.Size(122, 17);
            this.Rb_Cliente.TabIndex = 11;
            this.Rb_Cliente.Text = "Selecccionar Cliente";
            this.Rb_Cliente.UseVisualStyleBackColor = true;
            // 
            // Rb_Todos
            // 
            this.Rb_Todos.AutoSize = true;
            this.Rb_Todos.Checked = true;
            this.Rb_Todos.Location = new System.Drawing.Point(13, 19);
            this.Rb_Todos.Name = "Rb_Todos";
            this.Rb_Todos.Size = new System.Drawing.Size(55, 17);
            this.Rb_Todos.TabIndex = 10;
            this.Rb_Todos.TabStop = true;
            this.Rb_Todos.Text = "Todos";
            this.Rb_Todos.UseVisualStyleBackColor = true;
            this.Rb_Todos.CheckedChanged += new System.EventHandler(this.Rb_Todos_CheckedChanged_1);
            // 
            // Cmb_Usuarios
            // 
            this.Cmb_Usuarios.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmb_Usuarios.FormattingEnabled = true;
            this.Cmb_Usuarios.Location = new System.Drawing.Point(13, 42);
            this.Cmb_Usuarios.Name = "Cmb_Usuarios";
            this.Cmb_Usuarios.Size = new System.Drawing.Size(390, 21);
            this.Cmb_Usuarios.TabIndex = 20;
            // 
            // Rb_AllUser
            // 
            this.Rb_AllUser.AutoSize = true;
            this.Rb_AllUser.Checked = true;
            this.Rb_AllUser.Location = new System.Drawing.Point(13, 19);
            this.Rb_AllUser.Name = "Rb_AllUser";
            this.Rb_AllUser.Size = new System.Drawing.Size(55, 17);
            this.Rb_AllUser.TabIndex = 21;
            this.Rb_AllUser.TabStop = true;
            this.Rb_AllUser.Text = "Todos";
            this.Rb_AllUser.UseVisualStyleBackColor = true;
            this.Rb_AllUser.CheckedChanged += new System.EventHandler(this.Rb_AllUser_CheckedChanged);
            // 
            // Rb_Usuario
            // 
            this.Rb_Usuario.AutoSize = true;
            this.Rb_Usuario.Location = new System.Drawing.Point(283, 19);
            this.Rb_Usuario.Name = "Rb_Usuario";
            this.Rb_Usuario.Size = new System.Drawing.Size(120, 17);
            this.Rb_Usuario.TabIndex = 22;
            this.Rb_Usuario.Text = "Seleccionar Usuario";
            this.Rb_Usuario.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Rb_Todos);
            this.groupBox1.Controls.Add(this.Rb_Cliente);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Location = new System.Drawing.Point(28, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(414, 72);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Rb_AllUser);
            this.groupBox2.Controls.Add(this.Rb_Usuario);
            this.groupBox2.Controls.Add(this.Cmb_Usuarios);
            this.groupBox2.Location = new System.Drawing.Point(29, 80);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(414, 70);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Cmb_Stat);
            this.groupBox3.Controls.Add(this.Rb_Allst);
            this.groupBox3.Controls.Add(this.Rb_Stat);
            this.groupBox3.Location = new System.Drawing.Point(29, 153);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(414, 77);
            this.groupBox3.TabIndex = 25;
            this.groupBox3.TabStop = false;
            // 
            // Cmb_Stat
            // 
            this.Cmb_Stat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmb_Stat.FormattingEnabled = true;
            this.Cmb_Stat.Items.AddRange(new object[] {
            "Pendiente",
            "Terminado"});
            this.Cmb_Stat.Location = new System.Drawing.Point(16, 42);
            this.Cmb_Stat.Name = "Cmb_Stat";
            this.Cmb_Stat.Size = new System.Drawing.Size(390, 21);
            this.Cmb_Stat.TabIndex = 2;
            // 
            // Rb_Allst
            // 
            this.Rb_Allst.AutoSize = true;
            this.Rb_Allst.Checked = true;
            this.Rb_Allst.Location = new System.Drawing.Point(16, 19);
            this.Rb_Allst.Name = "Rb_Allst";
            this.Rb_Allst.Size = new System.Drawing.Size(55, 17);
            this.Rb_Allst.TabIndex = 1;
            this.Rb_Allst.TabStop = true;
            this.Rb_Allst.Text = "Todos";
            this.Rb_Allst.UseVisualStyleBackColor = true;
            this.Rb_Allst.CheckedChanged += new System.EventHandler(this.Rb_Allst_CheckedChanged);
            // 
            // Rb_Stat
            // 
            this.Rb_Stat.AutoSize = true;
            this.Rb_Stat.Location = new System.Drawing.Point(286, 19);
            this.Rb_Stat.Name = "Rb_Stat";
            this.Rb_Stat.Size = new System.Drawing.Size(122, 17);
            this.Rb_Stat.TabIndex = 0;
            this.Rb_Stat.Text = "Seleccionar  Estatus";
            this.Rb_Stat.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdBtnCalendar);
            this.groupBox4.Controls.Add(this.rdBtnDetallado);
            this.groupBox4.Location = new System.Drawing.Point(29, 238);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(414, 38);
            this.groupBox4.TabIndex = 26;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Estilo de reporte:";
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
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(29, 381);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(256, 23);
            this.progreso.TabIndex = 29;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 329);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Formato:";
            // 
            // radioPDF
            // 
            this.radioPDF.AutoSize = true;
            this.radioPDF.Checked = true;
            this.radioPDF.Location = new System.Drawing.Point(83, 327);
            this.radioPDF.Name = "radioPDF";
            this.radioPDF.Size = new System.Drawing.Size(46, 17);
            this.radioPDF.TabIndex = 31;
            this.radioPDF.TabStop = true;
            this.radioPDF.Text = "PDF";
            this.radioPDF.UseVisualStyleBackColor = true;
            // 
            // radioExcel
            // 
            this.radioExcel.AutoSize = true;
            this.radioExcel.Location = new System.Drawing.Point(134, 327);
            this.radioExcel.Name = "radioExcel";
            this.radioExcel.Size = new System.Drawing.Size(51, 17);
            this.radioExcel.TabIndex = 32;
            this.radioExcel.TabStop = true;
            this.radioExcel.Text = "Excel";
            this.radioExcel.UseVisualStyleBackColor = true;
            this.radioExcel.Visible = false;
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(286, 329);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 28;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_ControlClicked);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(367, 329);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 27;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.Btn_Cancelar_Click);
            // 
            // Frm_RepAgenda
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(469, 412);
            this.Controls.Add(this.radioExcel);
            this.Controls.Add(this.radioPDF);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(475, 440);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(475, 440);
            this.Name = "Frm_RepAgenda";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte Agenda";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_RepAgenda_FormClosing);
            this.Load += new System.EventHandler(this.Frm_RepAgenda_Load_1);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.RadioButton Rb_Cliente;
        private System.Windows.Forms.RadioButton Rb_Todos;
        private System.Windows.Forms.ComboBox Cmb_Usuarios;
        private System.Windows.Forms.RadioButton Rb_AllUser;
        private System.Windows.Forms.RadioButton Rb_Usuario;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton Rb_Allst;
        private System.Windows.Forms.RadioButton Rb_Stat;
        private System.Windows.Forms.ComboBox Cmb_Stat;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdBtnCalendar;
        private System.Windows.Forms.RadioButton rdBtnDetallado;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonGenerar;
        private System.ComponentModel.BackgroundWorker workerReporte;
        private System.Windows.Forms.ProgressBar progreso;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioPDF;
        private System.Windows.Forms.RadioButton radioExcel;
    }
}