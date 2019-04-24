namespace GestorReportes.PresentationLayer
{
    partial class Frm_ListadoContratosRenta
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
            this.comboConjunto = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboInmobiliaria = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioPDF = new System.Windows.Forms.RadioButton();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioVigentesyHistorial = new System.Windows.Forms.RadioButton();
            this.radioVigentes = new System.Windows.Forms.RadioButton();
            this.radioTodos = new System.Windows.Forms.RadioButton();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.checkIncluirCargos = new System.Windows.Forms.CheckBox();
            this.dtpFechaFin = new System.Windows.Forms.DateTimePicker();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboConjunto
            // 
            this.comboConjunto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboConjunto.FormattingEnabled = true;
            this.comboConjunto.Location = new System.Drawing.Point(77, 55);
            this.comboConjunto.Name = "comboConjunto";
            this.comboConjunto.Size = new System.Drawing.Size(350, 21);
            this.comboConjunto.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Conjunto:";
            // 
            // comboInmobiliaria
            // 
            this.comboInmobiliaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInmobiliaria.FormattingEnabled = true;
            this.comboInmobiliaria.Location = new System.Drawing.Point(77, 28);
            this.comboInmobiliaria.Name = "comboInmobiliaria";
            this.comboInmobiliaria.Size = new System.Drawing.Size(350, 21);
            this.comboInmobiliaria.TabIndex = 8;
            this.comboInmobiliaria.SelectedIndexChanged += new System.EventHandler(this.comboInmobiliaria_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Inmobiliaria:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(419, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Seleccione los criterios para generar el listado de contratos.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioPDF);
            this.groupBox2.Controls.Add(this.radioExcel);
            this.groupBox2.Location = new System.Drawing.Point(77, 204);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(143, 41);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Formato:";
            // 
            // radioPDF
            // 
            this.radioPDF.AutoSize = true;
            this.radioPDF.Checked = true;
            this.radioPDF.Location = new System.Drawing.Point(6, 14);
            this.radioPDF.Name = "radioPDF";
            this.radioPDF.Size = new System.Drawing.Size(46, 17);
            this.radioPDF.TabIndex = 1;
            this.radioPDF.TabStop = true;
            this.radioPDF.Text = "PDF";
            this.radioPDF.UseVisualStyleBackColor = true;
            // 
            // radioExcel
            // 
            this.radioExcel.AutoSize = true;
            this.radioExcel.Location = new System.Drawing.Point(71, 14);
            this.radioExcel.Name = "radioExcel";
            this.radioExcel.Size = new System.Drawing.Size(51, 17);
            this.radioExcel.TabIndex = 0;
            this.radioExcel.Text = "Excel";
            this.radioExcel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioVigentesyHistorial);
            this.groupBox1.Controls.Add(this.radioVigentes);
            this.groupBox1.Controls.Add(this.radioTodos);
            this.groupBox1.Location = new System.Drawing.Point(77, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(269, 46);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Estatus:";
            // 
            // radioVigentesyHistorial
            // 
            this.radioVigentesyHistorial.AutoSize = true;
            this.radioVigentesyHistorial.Location = new System.Drawing.Point(149, 19);
            this.radioVigentesyHistorial.Name = "radioVigentesyHistorial";
            this.radioVigentesyHistorial.Size = new System.Drawing.Size(115, 17);
            this.radioVigentesyHistorial.TabIndex = 2;
            this.radioVigentesyHistorial.TabStop = true;
            this.radioVigentesyHistorial.Text = "Vigentes e Historial";
            this.radioVigentesyHistorial.UseVisualStyleBackColor = true;
            this.radioVigentesyHistorial.CheckedChanged += new System.EventHandler(this.radioVigentesyHistorial_CheckedChanged);
            // 
            // radioVigentes
            // 
            this.radioVigentes.AutoSize = true;
            this.radioVigentes.Location = new System.Drawing.Point(67, 19);
            this.radioVigentes.Name = "radioVigentes";
            this.radioVigentes.Size = new System.Drawing.Size(66, 17);
            this.radioVigentes.TabIndex = 1;
            this.radioVigentes.Text = "Vigentes";
            this.radioVigentes.UseVisualStyleBackColor = true;
            // 
            // radioTodos
            // 
            this.radioTodos.AutoSize = true;
            this.radioTodos.Checked = true;
            this.radioTodos.Location = new System.Drawing.Point(6, 19);
            this.radioTodos.Name = "radioTodos";
            this.radioTodos.Size = new System.Drawing.Size(55, 17);
            this.radioTodos.TabIndex = 0;
            this.radioTodos.TabStop = true;
            this.radioTodos.Text = "Todos";
            this.radioTodos.UseVisualStyleBackColor = true;
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(77, 277);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(350, 23);
            this.progreso.TabIndex = 25;
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.WorkerSupportsCancellation = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // checkIncluirCargos
            // 
            this.checkIncluirCargos.AutoSize = true;
            this.checkIncluirCargos.Location = new System.Drawing.Point(77, 251);
            this.checkIncluirCargos.Name = "checkIncluirCargos";
            this.checkIncluirCargos.Size = new System.Drawing.Size(90, 17);
            this.checkIncluirCargos.TabIndex = 26;
            this.checkIncluirCargos.Text = "Incluir Cargos";
            this.checkIncluirCargos.UseVisualStyleBackColor = true;
            // 
            // dtpFechaFin
            // 
            this.dtpFechaFin.Location = new System.Drawing.Point(17, 19);
            this.dtpFechaFin.Name = "dtpFechaFin";
            this.dtpFechaFin.Size = new System.Drawing.Size(210, 20);
            this.dtpFechaFin.TabIndex = 27;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dtpFechaFin);
            this.groupBox3.Location = new System.Drawing.Point(77, 134);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(246, 53);
            this.groupBox3.TabIndex = 29;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Contratos Vigentes Fecha Corte";
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(271, 193);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 24;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_ControlClicked);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(352, 193);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 23;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_ControlClicked);
            // 
            // Frm_ListadoContratosRenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(439, 311);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.checkIncluirCargos);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboConjunto);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboInmobiliaria);
            this.Controls.Add(this.label2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(455, 350);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(455, 350);
            this.Name = "Frm_ListadoContratosRenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Listado de contratos";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_ListadoContratos_FormClosing);
            this.Load += new System.EventHandler(this.Frm_ListadoContratos_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboConjunto;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboInmobiliaria;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioPDF;
        private System.Windows.Forms.RadioButton radioExcel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioVigentes;
        private System.Windows.Forms.RadioButton radioTodos;
        private Controls.Ctrl_Opcion botonGenerar;
        private Controls.Ctrl_Opcion botonCancelar;
        private System.Windows.Forms.ProgressBar progreso;
        private System.ComponentModel.BackgroundWorker workerReporte;
        private System.Windows.Forms.CheckBox checkIncluirCargos;
        private System.Windows.Forms.DateTimePicker dtpFechaFin;
        private System.Windows.Forms.RadioButton radioVigentesyHistorial;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}