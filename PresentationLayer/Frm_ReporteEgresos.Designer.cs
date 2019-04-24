namespace GestorReportes.PresentationLayer
{
    partial class Frm_ReporteEgresos
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
            this.comboInmobiliaria = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dateInicio = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.dateFin = new System.Windows.Forms.DateTimePicker();
            this.comboCuentas = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboClasificacion = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioBeneficiario = new System.Windows.Forms.RadioButton();
            this.radioEstatus = new System.Windows.Forms.RadioButton();
            this.radioClasificacion = new System.Windows.Forms.RadioButton();
            this.radioFecha = new System.Windows.Forms.RadioButton();
            this.grupoMoneda = new System.Windows.Forms.GroupBox();
            this.radioDolares = new System.Windows.Forms.RadioButton();
            this.radioPesos = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.radioPDF = new System.Windows.Forms.RadioButton();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.checkCancelados = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.grupoMoneda.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(472, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "Seleccione la inmobilaria, rango de fechas, cuenta bancaria, clasificación, forma" +
                "  de presentación y moneda, en caso necesario, y haga clic en generar";
            // 
            // comboInmobiliaria
            // 
            this.comboInmobiliaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInmobiliaria.FormattingEnabled = true;
            this.comboInmobiliaria.Location = new System.Drawing.Point(118, 52);
            this.comboInmobiliaria.Name = "comboInmobiliaria";
            this.comboInmobiliaria.Size = new System.Drawing.Size(354, 21);
            this.comboInmobiliaria.TabIndex = 4;
            this.comboInmobiliaria.SelectedValueChanged += new System.EventHandler(this.comboInmobiliaria_SelectedValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Inmobiliaria:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Con fecha del:";
            // 
            // dateInicio
            // 
            this.dateInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateInicio.Location = new System.Drawing.Point(118, 80);
            this.dateInicio.Name = "dateInicio";
            this.dateInicio.Size = new System.Drawing.Size(140, 20);
            this.dateInicio.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(299, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "al:";
            // 
            // dateFin
            // 
            this.dateFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateFin.Location = new System.Drawing.Point(323, 80);
            this.dateFin.Name = "dateFin";
            this.dateFin.Size = new System.Drawing.Size(140, 20);
            this.dateFin.TabIndex = 8;
            // 
            // comboCuentas
            // 
            this.comboCuentas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCuentas.FormattingEnabled = true;
            this.comboCuentas.Location = new System.Drawing.Point(118, 106);
            this.comboCuentas.Name = "comboCuentas";
            this.comboCuentas.Size = new System.Drawing.Size(354, 21);
            this.comboCuentas.TabIndex = 10;
            this.comboCuentas.SelectedValueChanged += new System.EventHandler(this.comboCuentas_SelectedValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Cuenta bancaria:";
            // 
            // comboClasificacion
            // 
            this.comboClasificacion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboClasificacion.FormattingEnabled = true;
            this.comboClasificacion.Location = new System.Drawing.Point(118, 133);
            this.comboClasificacion.Name = "comboClasificacion";
            this.comboClasificacion.Size = new System.Drawing.Size(354, 21);
            this.comboClasificacion.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 136);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Clasificación:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioBeneficiario);
            this.groupBox1.Controls.Add(this.radioEstatus);
            this.groupBox1.Controls.Add(this.radioClasificacion);
            this.groupBox1.Controls.Add(this.radioFecha);
            this.groupBox1.Location = new System.Drawing.Point(16, 160);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(166, 72);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Orden de presentación:";
            // 
            // radioBeneficiario
            // 
            this.radioBeneficiario.AutoSize = true;
            this.radioBeneficiario.Location = new System.Drawing.Point(73, 44);
            this.radioBeneficiario.Name = "radioBeneficiario";
            this.radioBeneficiario.Size = new System.Drawing.Size(80, 17);
            this.radioBeneficiario.TabIndex = 3;
            this.radioBeneficiario.TabStop = true;
            this.radioBeneficiario.Text = "Beneficiario";
            this.radioBeneficiario.UseVisualStyleBackColor = true;
            // 
            // radioEstatus
            // 
            this.radioEstatus.AutoSize = true;
            this.radioEstatus.Location = new System.Drawing.Point(7, 44);
            this.radioEstatus.Name = "radioEstatus";
            this.radioEstatus.Size = new System.Drawing.Size(60, 17);
            this.radioEstatus.TabIndex = 2;
            this.radioEstatus.TabStop = true;
            this.radioEstatus.Text = "Estatus";
            this.radioEstatus.UseVisualStyleBackColor = true;
            // 
            // radioClasificacion
            // 
            this.radioClasificacion.AutoSize = true;
            this.radioClasificacion.Location = new System.Drawing.Point(73, 20);
            this.radioClasificacion.Name = "radioClasificacion";
            this.radioClasificacion.Size = new System.Drawing.Size(84, 17);
            this.radioClasificacion.TabIndex = 1;
            this.radioClasificacion.TabStop = true;
            this.radioClasificacion.Text = "Clasificación";
            this.radioClasificacion.UseVisualStyleBackColor = true;
            // 
            // radioFecha
            // 
            this.radioFecha.AutoSize = true;
            this.radioFecha.Checked = true;
            this.radioFecha.Location = new System.Drawing.Point(7, 20);
            this.radioFecha.Name = "radioFecha";
            this.radioFecha.Size = new System.Drawing.Size(55, 17);
            this.radioFecha.TabIndex = 0;
            this.radioFecha.TabStop = true;
            this.radioFecha.Text = "Fecha";
            this.radioFecha.UseVisualStyleBackColor = true;
            // 
            // grupoMoneda
            // 
            this.grupoMoneda.Controls.Add(this.radioDolares);
            this.grupoMoneda.Controls.Add(this.radioPesos);
            this.grupoMoneda.Location = new System.Drawing.Point(188, 205);
            this.grupoMoneda.Name = "grupoMoneda";
            this.grupoMoneda.Size = new System.Drawing.Size(129, 38);
            this.grupoMoneda.TabIndex = 14;
            this.grupoMoneda.TabStop = false;
            this.grupoMoneda.Text = "Moneda:";
            this.grupoMoneda.Visible = false;
            // 
            // radioDolares
            // 
            this.radioDolares.AutoSize = true;
            this.radioDolares.Location = new System.Drawing.Point(66, 15);
            this.radioDolares.Name = "radioDolares";
            this.radioDolares.Size = new System.Drawing.Size(61, 17);
            this.radioDolares.TabIndex = 1;
            this.radioDolares.Text = "Dolares";
            this.radioDolares.UseVisualStyleBackColor = true;
            // 
            // radioPesos
            // 
            this.radioPesos.AutoSize = true;
            this.radioPesos.Checked = true;
            this.radioPesos.Location = new System.Drawing.Point(6, 15);
            this.radioPesos.Name = "radioPesos";
            this.radioPesos.Size = new System.Drawing.Size(54, 17);
            this.radioPesos.TabIndex = 0;
            this.radioPesos.TabStop = true;
            this.radioPesos.Text = "Pesos";
            this.radioPesos.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioExcel);
            this.groupBox2.Controls.Add(this.radioPDF);
            this.groupBox2.Location = new System.Drawing.Point(188, 160);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(129, 38);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Formato:";
            // 
            // radioExcel
            // 
            this.radioExcel.AutoSize = true;
            this.radioExcel.Location = new System.Drawing.Point(66, 15);
            this.radioExcel.Name = "radioExcel";
            this.radioExcel.Size = new System.Drawing.Size(51, 17);
            this.radioExcel.TabIndex = 1;
            this.radioExcel.Text = "Excel";
            this.radioExcel.UseVisualStyleBackColor = true;
            // 
            // radioPDF
            // 
            this.radioPDF.AutoSize = true;
            this.radioPDF.Checked = true;
            this.radioPDF.Location = new System.Drawing.Point(6, 15);
            this.radioPDF.Name = "radioPDF";
            this.radioPDF.Size = new System.Drawing.Size(46, 17);
            this.radioPDF.TabIndex = 0;
            this.radioPDF.TabStop = true;
            this.radioPDF.Text = "PDF";
            this.radioPDF.UseVisualStyleBackColor = true;
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(397, 205);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 17;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_Click);
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(321, 205);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(70, 75);
            this.botonGenerar.TabIndex = 18;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_Click);
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(23, 257);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(294, 23);
            this.progreso.TabIndex = 19;
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // checkCancelados
            // 
            this.checkCancelados.AutoSize = true;
            this.checkCancelados.Location = new System.Drawing.Point(323, 175);
            this.checkCancelados.Name = "checkCancelados";
            this.checkCancelados.Size = new System.Drawing.Size(156, 17);
            this.checkCancelados.TabIndex = 20;
            this.checkCancelados.Text = "Incluir cheques cancelados";
            this.checkCancelados.UseVisualStyleBackColor = true;
            // 
            // Frm_ReporteEgresos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(494, 292);
            this.Controls.Add(this.checkCancelados);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grupoMoneda);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboClasificacion);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboCuentas);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dateFin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dateInicio);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboInmobiliaria);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 320);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 320);
            this.Name = "Frm_ReporteEgresos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de egresos";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_ReporteEgresos_FormClosing);
            this.Load += new System.EventHandler(this.Frm_ReporteEgresos_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grupoMoneda.ResumeLayout(false);
            this.grupoMoneda.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboInmobiliaria;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateInicio;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateFin;
        private System.Windows.Forms.ComboBox comboCuentas;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboClasificacion;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioFecha;
        private System.Windows.Forms.RadioButton radioBeneficiario;
        private System.Windows.Forms.RadioButton radioEstatus;
        private System.Windows.Forms.RadioButton radioClasificacion;
        private System.Windows.Forms.GroupBox grupoMoneda;
        private System.Windows.Forms.RadioButton radioDolares;
        private System.Windows.Forms.RadioButton radioPesos;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioExcel;
        private System.Windows.Forms.RadioButton radioPDF;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonGenerar;
        private System.Windows.Forms.ProgressBar progreso;
        private System.ComponentModel.BackgroundWorker workerReporte;
        private System.Windows.Forms.CheckBox checkCancelados;
    }
}