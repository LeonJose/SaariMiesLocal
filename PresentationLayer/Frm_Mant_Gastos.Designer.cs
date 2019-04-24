namespace GestorReportes.PresentationLayer
{
    partial class Frm_Mant_Gastos
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBox_Inmobiliaria = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbBox_Conjunto = new System.Windows.Forms.ComboBox();
            this.rb_Conjunto = new System.Windows.Forms.RadioButton();
            this.rb_allConjuntos = new System.Windows.Forms.RadioButton();
            this.checkBox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbBox_Usuario = new System.Windows.Forms.ComboBox();
            this.rb_User = new System.Windows.Forms.RadioButton();
            this.rb_allUser = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rb_clasif = new System.Windows.Forms.RadioButton();
            this.cmbBox_Clasificacion = new System.Windows.Forms.ComboBox();
            this.rb_allclasif = new System.Windows.Forms.RadioButton();
            this.rb_Formato = new System.Windows.Forms.RadioButton();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cmb_Inmueble = new System.Windows.Forms.ComboBox();
            this.rb_Inmueble = new System.Windows.Forms.RadioButton();
            this.rb_allInmu = new System.Windows.Forms.RadioButton();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.rb_Excel = new System.Windows.Forms.RadioButton();
            this.checkBoxSoloGen = new System.Windows.Forms.CheckBox();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Inmobiliaria:";
            // 
            // cmbBox_Inmobiliaria
            // 
            this.cmbBox_Inmobiliaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBox_Inmobiliaria.FormattingEnabled = true;
            this.cmbBox_Inmobiliaria.Location = new System.Drawing.Point(99, 12);
            this.cmbBox_Inmobiliaria.Name = "cmbBox_Inmobiliaria";
            this.cmbBox_Inmobiliaria.Size = new System.Drawing.Size(331, 21);
            this.cmbBox_Inmobiliaria.TabIndex = 1;
            this.cmbBox_Inmobiliaria.SelectedIndexChanged += new System.EventHandler(this.cmbBox_Inmobiliaria_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbBox_Conjunto);
            this.groupBox1.Controls.Add(this.rb_Conjunto);
            this.groupBox1.Controls.Add(this.rb_allConjuntos);
            this.groupBox1.Location = new System.Drawing.Point(34, 73);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(412, 80);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // cmbBox_Conjunto
            // 
            this.cmbBox_Conjunto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBox_Conjunto.Enabled = false;
            this.cmbBox_Conjunto.FormattingEnabled = true;
            this.cmbBox_Conjunto.Location = new System.Drawing.Point(7, 44);
            this.cmbBox_Conjunto.Name = "cmbBox_Conjunto";
            this.cmbBox_Conjunto.Size = new System.Drawing.Size(399, 21);
            this.cmbBox_Conjunto.TabIndex = 2;
            this.cmbBox_Conjunto.SelectedIndexChanged += new System.EventHandler(this.cmbBox_Conjunto_SelectedIndexChanged);
            // 
            // rb_Conjunto
            // 
            this.rb_Conjunto.AutoSize = true;
            this.rb_Conjunto.Location = new System.Drawing.Point(263, 20);
            this.rb_Conjunto.Name = "rb_Conjunto";
            this.rb_Conjunto.Size = new System.Drawing.Size(126, 17);
            this.rb_Conjunto.TabIndex = 1;
            this.rb_Conjunto.TabStop = true;
            this.rb_Conjunto.Text = "Seleccionar Conjunto";
            this.rb_Conjunto.UseVisualStyleBackColor = true;
            this.rb_Conjunto.CheckedChanged += new System.EventHandler(this.rb_Conjunto_CheckedChanged);
            // 
            // rb_allConjuntos
            // 
            this.rb_allConjuntos.AutoSize = true;
            this.rb_allConjuntos.Checked = true;
            this.rb_allConjuntos.Location = new System.Drawing.Point(7, 20);
            this.rb_allConjuntos.Name = "rb_allConjuntos";
            this.rb_allConjuntos.Size = new System.Drawing.Size(55, 17);
            this.rb_allConjuntos.TabIndex = 0;
            this.rb_allConjuntos.TabStop = true;
            this.rb_allConjuntos.Text = "Todos";
            this.rb_allConjuntos.UseVisualStyleBackColor = true;
            this.rb_allConjuntos.CheckedChanged += new System.EventHandler(this.rb_allConjuntos_CheckedChanged);
            // 
            // checkBox
            // 
            this.checkBox.AutoSize = true;
            this.checkBox.Enabled = false;
            this.checkBox.Location = new System.Drawing.Point(41, 50);
            this.checkBox.Name = "checkBox";
            this.checkBox.Size = new System.Drawing.Size(91, 17);
            this.checkBox.TabIndex = 31;
            this.checkBox.Text = "Solo conjunto";
            this.checkBox.UseVisualStyleBackColor = true;
            this.checkBox.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbBox_Usuario);
            this.groupBox2.Controls.Add(this.rb_User);
            this.groupBox2.Controls.Add(this.rb_allUser);
            this.groupBox2.Location = new System.Drawing.Point(34, 245);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(412, 80);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // cmbBox_Usuario
            // 
            this.cmbBox_Usuario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBox_Usuario.FormattingEnabled = true;
            this.cmbBox_Usuario.Location = new System.Drawing.Point(7, 44);
            this.cmbBox_Usuario.Name = "cmbBox_Usuario";
            this.cmbBox_Usuario.Size = new System.Drawing.Size(399, 21);
            this.cmbBox_Usuario.TabIndex = 2;
            // 
            // rb_User
            // 
            this.rb_User.AutoSize = true;
            this.rb_User.Location = new System.Drawing.Point(263, 20);
            this.rb_User.Name = "rb_User";
            this.rb_User.Size = new System.Drawing.Size(120, 17);
            this.rb_User.TabIndex = 1;
            this.rb_User.TabStop = true;
            this.rb_User.Text = "Seleccionar Usuario";
            this.rb_User.UseVisualStyleBackColor = true;
            this.rb_User.CheckedChanged += new System.EventHandler(this.rb_User_CheckedChanged);
            // 
            // rb_allUser
            // 
            this.rb_allUser.AutoSize = true;
            this.rb_allUser.Checked = true;
            this.rb_allUser.Location = new System.Drawing.Point(7, 20);
            this.rb_allUser.Name = "rb_allUser";
            this.rb_allUser.Size = new System.Drawing.Size(55, 17);
            this.rb_allUser.TabIndex = 0;
            this.rb_allUser.TabStop = true;
            this.rb_allUser.Text = "Todos";
            this.rb_allUser.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rb_clasif);
            this.groupBox3.Controls.Add(this.cmbBox_Clasificacion);
            this.groupBox3.Controls.Add(this.rb_allclasif);
            this.groupBox3.Location = new System.Drawing.Point(34, 331);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(412, 80);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            // 
            // rb_clasif
            // 
            this.rb_clasif.AutoSize = true;
            this.rb_clasif.Location = new System.Drawing.Point(263, 20);
            this.rb_clasif.Name = "rb_clasif";
            this.rb_clasif.Size = new System.Drawing.Size(143, 17);
            this.rb_clasif.TabIndex = 2;
            this.rb_clasif.TabStop = true;
            this.rb_clasif.Text = "Seleccionar Clasificacion";
            this.rb_clasif.UseVisualStyleBackColor = true;
            this.rb_clasif.CheckedChanged += new System.EventHandler(this.rb_clasif_CheckedChanged);
            // 
            // cmbBox_Clasificacion
            // 
            this.cmbBox_Clasificacion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBox_Clasificacion.FormattingEnabled = true;
            this.cmbBox_Clasificacion.Location = new System.Drawing.Point(7, 44);
            this.cmbBox_Clasificacion.Name = "cmbBox_Clasificacion";
            this.cmbBox_Clasificacion.Size = new System.Drawing.Size(399, 21);
            this.cmbBox_Clasificacion.TabIndex = 1;
            // 
            // rb_allclasif
            // 
            this.rb_allclasif.AutoSize = true;
            this.rb_allclasif.Checked = true;
            this.rb_allclasif.Location = new System.Drawing.Point(7, 20);
            this.rb_allclasif.Name = "rb_allclasif";
            this.rb_allclasif.Size = new System.Drawing.Size(55, 17);
            this.rb_allclasif.TabIndex = 0;
            this.rb_allclasif.TabStop = true;
            this.rb_allclasif.Text = "Todos";
            this.rb_allclasif.UseVisualStyleBackColor = true;
            // 
            // rb_Formato
            // 
            this.rb_Formato.AutoSize = true;
            this.rb_Formato.Checked = true;
            this.rb_Formato.Location = new System.Drawing.Point(93, 468);
            this.rb_Formato.Name = "rb_Formato";
            this.rb_Formato.Size = new System.Drawing.Size(46, 17);
            this.rb_Formato.TabIndex = 4;
            this.rb_Formato.TabStop = true;
            this.rb_Formato.Text = "PDF";
            this.rb_Formato.UseVisualStyleBackColor = true;
            this.rb_Formato.CheckedChanged += new System.EventHandler(this.rb_Formato_CheckedChanged);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(40, 442);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 5;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker2.Location = new System.Drawing.Point(246, 442);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker2.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 426);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Fecha Inicial";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(243, 426);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Fecha Final";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 469);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Formato :";
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(40, 501);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(221, 23);
            this.progreso.TabIndex = 10;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cmb_Inmueble);
            this.groupBox4.Controls.Add(this.rb_Inmueble);
            this.groupBox4.Controls.Add(this.rb_allInmu);
            this.groupBox4.Location = new System.Drawing.Point(34, 159);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(412, 80);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            // 
            // cmb_Inmueble
            // 
            this.cmb_Inmueble.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Inmueble.FormattingEnabled = true;
            this.cmb_Inmueble.Location = new System.Drawing.Point(10, 43);
            this.cmb_Inmueble.Name = "cmb_Inmueble";
            this.cmb_Inmueble.Size = new System.Drawing.Size(396, 21);
            this.cmb_Inmueble.TabIndex = 2;
            // 
            // rb_Inmueble
            // 
            this.rb_Inmueble.AutoSize = true;
            this.rb_Inmueble.Location = new System.Drawing.Point(263, 20);
            this.rb_Inmueble.Name = "rb_Inmueble";
            this.rb_Inmueble.Size = new System.Drawing.Size(127, 17);
            this.rb_Inmueble.TabIndex = 1;
            this.rb_Inmueble.Text = "Seleccionar Inmueble";
            this.rb_Inmueble.UseVisualStyleBackColor = true;
            this.rb_Inmueble.CheckedChanged += new System.EventHandler(this.rb_Inmueble_CheckedChanged);
            // 
            // rb_allInmu
            // 
            this.rb_allInmu.AutoSize = true;
            this.rb_allInmu.Checked = true;
            this.rb_allInmu.Location = new System.Drawing.Point(10, 20);
            this.rb_allInmu.Name = "rb_allInmu";
            this.rb_allInmu.Size = new System.Drawing.Size(55, 17);
            this.rb_allInmu.TabIndex = 0;
            this.rb_allInmu.TabStop = true;
            this.rb_allInmu.Text = "Todos";
            this.rb_allInmu.UseVisualStyleBackColor = true;
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // rb_Excel
            // 
            this.rb_Excel.AutoSize = true;
            this.rb_Excel.Location = new System.Drawing.Point(145, 469);
            this.rb_Excel.Name = "rb_Excel";
            this.rb_Excel.Size = new System.Drawing.Size(51, 17);
            this.rb_Excel.TabIndex = 29;
            this.rb_Excel.TabStop = true;
            this.rb_Excel.Text = "Excel";
            this.rb_Excel.UseVisualStyleBackColor = true;
            this.rb_Excel.Click += new System.EventHandler(this.rb_Excel_Click);
            // 
            // checkBoxSoloGen
            // 
            this.checkBoxSoloGen.AutoSize = true;
            this.checkBoxSoloGen.Enabled = false;
            this.checkBoxSoloGen.Location = new System.Drawing.Point(202, 470);
            this.checkBoxSoloGen.Name = "checkBoxSoloGen";
            this.checkBoxSoloGen.Size = new System.Drawing.Size(76, 17);
            this.checkBoxSoloGen.TabIndex = 32;
            this.checkBoxSoloGen.Text = "Abrir Excel";
            this.checkBoxSoloGen.UseVisualStyleBackColor = true;
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(297, 472);
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
            this.botonCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(390, 472);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 27;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_ControlClicked);
            // 
            // Frm_Mant_Gastos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(473, 579);
            this.Controls.Add(this.checkBoxSoloGen);
            this.Controls.Add(this.rb_Excel);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.checkBox);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.rb_Formato);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmbBox_Inmobiliaria);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 493);
            this.Name = "Frm_Mant_Gastos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de Gastos de Mantenimiento";
            this.Load += new System.EventHandler(this.Frm_Mant_Gastos_Load);
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

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbBox_Inmobiliaria;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cmbBox_Conjunto;
        private System.Windows.Forms.RadioButton rb_Conjunto;
        private System.Windows.Forms.RadioButton rb_allConjuntos;
        private System.Windows.Forms.ComboBox cmbBox_Usuario;
        private System.Windows.Forms.RadioButton rb_User;
        private System.Windows.Forms.RadioButton rb_allUser;
        private System.Windows.Forms.RadioButton rb_clasif;
        private System.Windows.Forms.ComboBox cmbBox_Clasificacion;
        private System.Windows.Forms.RadioButton rb_allclasif;
        private System.Windows.Forms.RadioButton rb_Formato;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar progreso;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cmb_Inmueble;
        private System.Windows.Forms.RadioButton rb_Inmueble;
        private System.Windows.Forms.RadioButton rb_allInmu;
        private System.ComponentModel.BackgroundWorker workerReporte;
        private Controls.Ctrl_Opcion botonGenerar;
        private Controls.Ctrl_Opcion botonCancelar;
        private System.Windows.Forms.RadioButton rb_Excel;
        private System.Windows.Forms.CheckBox checkBox;
        private System.Windows.Forms.CheckBox checkBoxSoloGen;
    }
}