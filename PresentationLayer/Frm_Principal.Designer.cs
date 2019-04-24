namespace GestorReportes
{
    partial class Frm_Principal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Principal));
            this.gbxDatosEmpresa = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.cbxSubConjunto = new System.Windows.Forms.ComboBox();
            this.lblSubConjunto = new System.Windows.Forms.Label();
            this.cbxConjunto = new System.Windows.Forms.ComboBox();
            this.lblConjunto = new System.Windows.Forms.Label();
            this.cbxInmobiliaria = new System.Windows.Forms.ComboBox();
            this.lblInmobiliaria = new System.Windows.Forms.Label();
            this.gbxDatosOperacion = new System.Windows.Forms.GroupBox();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.datePickerFinal = new System.Windows.Forms.DateTimePicker();
            this.datePickerIniCorte = new System.Windows.Forms.DateTimePicker();
            this.lblFechaFinal = new System.Windows.Forms.Label();
            this.lblFechaIniCorte = new System.Windows.Forms.Label();
            this.lblInstrucciones = new System.Windows.Forms.Label();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonGenerarPDF = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonGenerarExcel = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.gbxDatosEmpresa.SuspendLayout();
            this.gbxDatosOperacion.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxDatosEmpresa
            // 
            this.gbxDatosEmpresa.Controls.Add(this.radioButton2);
            this.gbxDatosEmpresa.Controls.Add(this.radioButton1);
            this.gbxDatosEmpresa.Controls.Add(this.cbxSubConjunto);
            this.gbxDatosEmpresa.Controls.Add(this.lblSubConjunto);
            this.gbxDatosEmpresa.Controls.Add(this.cbxConjunto);
            this.gbxDatosEmpresa.Controls.Add(this.lblConjunto);
            this.gbxDatosEmpresa.Controls.Add(this.cbxInmobiliaria);
            this.gbxDatosEmpresa.Controls.Add(this.lblInmobiliaria);
            this.gbxDatosEmpresa.Location = new System.Drawing.Point(13, 47);
            this.gbxDatosEmpresa.Name = "gbxDatosEmpresa";
            this.gbxDatosEmpresa.Size = new System.Drawing.Size(640, 130);
            this.gbxDatosEmpresa.TabIndex = 0;
            this.gbxDatosEmpresa.TabStop = false;
            this.gbxDatosEmpresa.Text = "Datos de la empresa";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(157, 67);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(126, 17);
            this.radioButton2.TabIndex = 6;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Seleccionar Conjunto";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(19, 67);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(121, 17);
            this.radioButton1.TabIndex = 5;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Todos los Conjuntos";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // cbxSubConjunto
            // 
            this.cbxSubConjunto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSubConjunto.FormattingEnabled = true;
            this.cbxSubConjunto.Location = new System.Drawing.Point(314, 103);
            this.cbxSubConjunto.Name = "cbxSubConjunto";
            this.cbxSubConjunto.Size = new System.Drawing.Size(312, 21);
            this.cbxSubConjunto.TabIndex = 3;
            // 
            // lblSubConjunto
            // 
            this.lblSubConjunto.AutoSize = true;
            this.lblSubConjunto.Location = new System.Drawing.Point(311, 87);
            this.lblSubConjunto.Name = "lblSubConjunto";
            this.lblSubConjunto.Size = new System.Drawing.Size(71, 13);
            this.lblSubConjunto.TabIndex = 4;
            this.lblSubConjunto.Text = "Sub-Conjunto";
            // 
            // cbxConjunto
            // 
            this.cbxConjunto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxConjunto.FormattingEnabled = true;
            this.cbxConjunto.Location = new System.Drawing.Point(17, 103);
            this.cbxConjunto.Name = "cbxConjunto";
            this.cbxConjunto.Size = new System.Drawing.Size(275, 21);
            this.cbxConjunto.TabIndex = 2;
            this.cbxConjunto.SelectedIndexChanged += new System.EventHandler(this.cbxConjunto_SelectedIndexChanged);
            // 
            // lblConjunto
            // 
            this.lblConjunto.AutoSize = true;
            this.lblConjunto.Location = new System.Drawing.Point(14, 87);
            this.lblConjunto.Name = "lblConjunto";
            this.lblConjunto.Size = new System.Drawing.Size(49, 13);
            this.lblConjunto.TabIndex = 2;
            this.lblConjunto.Text = "Conjunto";
            // 
            // cbxInmobiliaria
            // 
            this.cbxInmobiliaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxInmobiliaria.FormattingEnabled = true;
            this.cbxInmobiliaria.Location = new System.Drawing.Point(18, 30);
            this.cbxInmobiliaria.Name = "cbxInmobiliaria";
            this.cbxInmobiliaria.Size = new System.Drawing.Size(608, 21);
            this.cbxInmobiliaria.TabIndex = 1;
            this.cbxInmobiliaria.SelectedIndexChanged += new System.EventHandler(this.cbxInmobiliaria_SelectedIndexChanged);
            // 
            // lblInmobiliaria
            // 
            this.lblInmobiliaria.AutoSize = true;
            this.lblInmobiliaria.Location = new System.Drawing.Point(15, 14);
            this.lblInmobiliaria.Name = "lblInmobiliaria";
            this.lblInmobiliaria.Size = new System.Drawing.Size(59, 13);
            this.lblInmobiliaria.TabIndex = 0;
            this.lblInmobiliaria.Text = "Inmobiliaria";
            // 
            // gbxDatosOperacion
            // 
            this.gbxDatosOperacion.Controls.Add(this.radioButton4);
            this.gbxDatosOperacion.Controls.Add(this.radioButton3);
            this.gbxDatosOperacion.Controls.Add(this.datePickerFinal);
            this.gbxDatosOperacion.Controls.Add(this.datePickerIniCorte);
            this.gbxDatosOperacion.Controls.Add(this.lblFechaFinal);
            this.gbxDatosOperacion.Controls.Add(this.lblFechaIniCorte);
            this.gbxDatosOperacion.Location = new System.Drawing.Point(13, 183);
            this.gbxDatosOperacion.Name = "gbxDatosOperacion";
            this.gbxDatosOperacion.Size = new System.Drawing.Size(640, 76);
            this.gbxDatosOperacion.TabIndex = 1;
            this.gbxDatosOperacion.TabStop = false;
            this.gbxDatosOperacion.Text = "Datos de operación";
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Checked = true;
            this.radioButton4.Location = new System.Drawing.Point(214, 14);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(157, 17);
            this.radioButton4.TabIndex = 10;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "Importe Sin IVA y Sin Pagos";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(17, 14);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(165, 17);
            this.radioButton3.TabIndex = 9;
            this.radioButton3.Text = "Importe Con IVA y Con Pagos";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // datePickerFinal
            // 
            this.datePickerFinal.CustomFormat = "";
            this.datePickerFinal.Location = new System.Drawing.Point(256, 50);
            this.datePickerFinal.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            this.datePickerFinal.MinDate = new System.DateTime(1995, 1, 1, 0, 0, 0, 0);
            this.datePickerFinal.Name = "datePickerFinal";
            this.datePickerFinal.Size = new System.Drawing.Size(200, 20);
            this.datePickerFinal.TabIndex = 5;
            this.datePickerFinal.Value = new System.DateTime(2013, 1, 1, 0, 0, 0, 0);
            // 
            // datePickerIniCorte
            // 
            this.datePickerIniCorte.CustomFormat = "";
            this.datePickerIniCorte.Location = new System.Drawing.Point(18, 50);
            this.datePickerIniCorte.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            this.datePickerIniCorte.MinDate = new System.DateTime(1995, 1, 1, 0, 0, 0, 0);
            this.datePickerIniCorte.Name = "datePickerIniCorte";
            this.datePickerIniCorte.Size = new System.Drawing.Size(200, 20);
            this.datePickerIniCorte.TabIndex = 4;
            this.datePickerIniCorte.Value = new System.DateTime(2012, 1, 1, 0, 0, 0, 0);
            // 
            // lblFechaFinal
            // 
            this.lblFechaFinal.AutoSize = true;
            this.lblFechaFinal.Location = new System.Drawing.Point(253, 34);
            this.lblFechaFinal.Name = "lblFechaFinal";
            this.lblFechaFinal.Size = new System.Drawing.Size(59, 13);
            this.lblFechaFinal.TabIndex = 8;
            this.lblFechaFinal.Text = "Fecha final";
            // 
            // lblFechaIniCorte
            // 
            this.lblFechaIniCorte.AutoSize = true;
            this.lblFechaIniCorte.Location = new System.Drawing.Point(14, 34);
            this.lblFechaIniCorte.Name = "lblFechaIniCorte";
            this.lblFechaIniCorte.Size = new System.Drawing.Size(66, 13);
            this.lblFechaIniCorte.TabIndex = 6;
            this.lblFechaIniCorte.Text = "Fecha inicial";
            // 
            // lblInstrucciones
            // 
            this.lblInstrucciones.Location = new System.Drawing.Point(14, 9);
            this.lblInstrucciones.Name = "lblInstrucciones";
            this.lblInstrucciones.Size = new System.Drawing.Size(641, 27);
            this.lblInstrucciones.TabIndex = 5;
            this.lblInstrucciones.Text = "Instrucciones. Elija la información de la empresa, posteriormente elija la inform" +
                "ación de operación, para finalizar haga clic en Imprimir o en Enviar a excel seg" +
                "ún lo desee.";
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(578, 270);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 9;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_ControlClicked);
            // 
            // botonGenerarPDF
            // 
            this.botonGenerarPDF.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerarPDF.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerarPDF.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerarPDF.Imagen = ((System.Drawing.Image)(resources.GetObject("botonGenerarPDF.Imagen")));
            this.botonGenerarPDF.Location = new System.Drawing.Point(455, 270);
            this.botonGenerarPDF.Name = "botonGenerarPDF";
            this.botonGenerarPDF.Size = new System.Drawing.Size(117, 75);
            this.botonGenerarPDF.TabIndex = 10;
            this.botonGenerarPDF.Texto = "Generar en PDF";
            this.botonGenerarPDF.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnReporteImprimir_Click);
            // 
            // botonGenerarExcel
            // 
            this.botonGenerarExcel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerarExcel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerarExcel.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerarExcel.Imagen = ((System.Drawing.Image)(resources.GetObject("botonGenerarExcel.Imagen")));
            this.botonGenerarExcel.Location = new System.Drawing.Point(327, 270);
            this.botonGenerarExcel.Name = "botonGenerarExcel";
            this.botonGenerarExcel.Size = new System.Drawing.Size(122, 75);
            this.botonGenerarExcel.TabIndex = 11;
            this.botonGenerarExcel.Texto = "Generar en Excel";
            this.botonGenerarExcel.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnReporteExcel_Click);
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(17, 322);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(304, 23);
            this.progreso.TabIndex = 12;
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // Frm_Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(669, 357);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.botonGenerarExcel);
            this.Controls.Add(this.botonGenerarPDF);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.lblInstrucciones);
            this.Controls.Add(this.gbxDatosOperacion);
            this.Controls.Add(this.gbxDatosEmpresa);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(675, 385);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(675, 385);
            this.Name = "Frm_Principal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generación de reportes:";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_Principal_FormClosing);
            this.gbxDatosEmpresa.ResumeLayout(false);
            this.gbxDatosEmpresa.PerformLayout();
            this.gbxDatosOperacion.ResumeLayout(false);
            this.gbxDatosOperacion.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxDatosEmpresa;
        private System.Windows.Forms.GroupBox gbxDatosOperacion;
        private System.Windows.Forms.Label lblInmobiliaria;
        private System.Windows.Forms.ComboBox cbxSubConjunto;
        private System.Windows.Forms.Label lblSubConjunto;
        private System.Windows.Forms.ComboBox cbxConjunto;
        private System.Windows.Forms.Label lblConjunto;
        private System.Windows.Forms.ComboBox cbxInmobiliaria;
        private System.Windows.Forms.Label lblFechaFinal;
        private System.Windows.Forms.Label lblFechaIniCorte;
        private System.Windows.Forms.DateTimePicker datePickerFinal;
        private System.Windows.Forms.DateTimePicker datePickerIniCorte;
        private System.Windows.Forms.Label lblInstrucciones;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private PresentationLayer.Controls.Ctrl_Opcion botonCancelar;
        private PresentationLayer.Controls.Ctrl_Opcion botonGenerarPDF;
        private PresentationLayer.Controls.Ctrl_Opcion botonGenerarExcel;
        private System.Windows.Forms.ProgressBar progreso;
        private System.ComponentModel.BackgroundWorker workerReporte;
    }
}

