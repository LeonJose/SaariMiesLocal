namespace GestorReportes.PresentationLayer
{
    partial class Frm_Cartera306090
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
            this.label2 = new System.Windows.Forms.Label();
            this.comboInmobiliaria = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboConjunto = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.radioPDF = new System.Windows.Forms.RadioButton();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkDetallado = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioIva = new System.Windows.Forms.RadioButton();
            this.radioSinIva = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioTCEmision = new System.Windows.Forms.RadioButton();
            this.radioTCDia = new System.Windows.Forms.RadioButton();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(472, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Seleccione la inmobilaria, el conjunto si lo desea, la fecha de corte y el format" +
    "o del reporte y haga clic en generar";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Inmobiliaria:";
            // 
            // comboInmobiliaria
            // 
            this.comboInmobiliaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInmobiliaria.FormattingEnabled = true;
            this.comboInmobiliaria.Location = new System.Drawing.Point(95, 45);
            this.comboInmobiliaria.Name = "comboInmobiliaria";
            this.comboInmobiliaria.Size = new System.Drawing.Size(377, 21);
            this.comboInmobiliaria.TabIndex = 2;
            this.comboInmobiliaria.SelectedIndexChanged += new System.EventHandler(this.comboInmobiliaria_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Conjunto:";
            // 
            // comboConjunto
            // 
            this.comboConjunto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboConjunto.FormattingEnabled = true;
            this.comboConjunto.Location = new System.Drawing.Point(95, 72);
            this.comboConjunto.Name = "comboConjunto";
            this.comboConjunto.Size = new System.Drawing.Size(377, 21);
            this.comboConjunto.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Fecha de corte:";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(95, 101);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 6;
            // 
            // radioPDF
            // 
            this.radioPDF.AutoSize = true;
            this.radioPDF.Checked = true;
            this.radioPDF.Location = new System.Drawing.Point(33, 18);
            this.radioPDF.Name = "radioPDF";
            this.radioPDF.Size = new System.Drawing.Size(46, 17);
            this.radioPDF.TabIndex = 7;
            this.radioPDF.TabStop = true;
            this.radioPDF.Text = "PDF";
            this.radioPDF.UseVisualStyleBackColor = true;
            // 
            // radioExcel
            // 
            this.radioExcel.AutoSize = true;
            this.radioExcel.Location = new System.Drawing.Point(113, 18);
            this.radioExcel.Name = "radioExcel";
            this.radioExcel.Size = new System.Drawing.Size(51, 17);
            this.radioExcel.TabIndex = 8;
            this.radioExcel.TabStop = true;
            this.radioExcel.Text = "Excel";
            this.radioExcel.UseVisualStyleBackColor = true;
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.WorkerSupportsCancellation = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(16, 229);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(284, 23);
            this.progreso.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioPDF);
            this.groupBox1.Controls.Add(this.radioExcel);
            this.groupBox1.Location = new System.Drawing.Point(16, 127);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(166, 45);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Formato";
            // 
            // checkDetallado
            // 
            this.checkDetallado.AutoSize = true;
            this.checkDetallado.Location = new System.Drawing.Point(302, 103);
            this.checkDetallado.Name = "checkDetallado";
            this.checkDetallado.Size = new System.Drawing.Size(144, 17);
            this.checkDetallado.TabIndex = 14;
            this.checkDetallado.Text = "Incluir detalle de facturas";
            this.checkDetallado.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioIva);
            this.groupBox2.Controls.Add(this.radioSinIva);
            this.groupBox2.Location = new System.Drawing.Point(229, 126);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(243, 45);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Criterio de importes";
            // 
            // radioIva
            // 
            this.radioIva.AutoSize = true;
            this.radioIva.Checked = true;
            this.radioIva.Location = new System.Drawing.Point(21, 19);
            this.radioIva.Name = "radioIva";
            this.radioIva.Size = new System.Drawing.Size(94, 17);
            this.radioIva.TabIndex = 7;
            this.radioIva.TabStop = true;
            this.radioIva.Text = "Con impuestos";
            this.radioIva.UseVisualStyleBackColor = true;
            // 
            // radioSinIva
            // 
            this.radioSinIva.AutoSize = true;
            this.radioSinIva.Location = new System.Drawing.Point(147, 19);
            this.radioSinIva.Name = "radioSinIva";
            this.radioSinIva.Size = new System.Drawing.Size(90, 17);
            this.radioSinIva.TabIndex = 8;
            this.radioSinIva.TabStop = true;
            this.radioSinIva.Text = "Sin impuestos";
            this.radioSinIva.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioTCEmision);
            this.groupBox3.Controls.Add(this.radioTCDia);
            this.groupBox3.Location = new System.Drawing.Point(16, 178);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(279, 45);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Criterio de tipo de cambio";
            // 
            // radioTCEmision
            // 
            this.radioTCEmision.AutoSize = true;
            this.radioTCEmision.Location = new System.Drawing.Point(145, 19);
            this.radioTCEmision.Name = "radioTCEmision";
            this.radioTCEmision.Size = new System.Drawing.Size(128, 17);
            this.radioTCEmision.TabIndex = 7;
            this.radioTCEmision.Text = "De emisión de factura";
            this.radioTCEmision.UseVisualStyleBackColor = true;
            // 
            // radioTCDia
            // 
            this.radioTCDia.AutoSize = true;
            this.radioTCDia.Checked = true;
            this.radioTCDia.Location = new System.Drawing.Point(19, 19);
            this.radioTCDia.Name = "radioTCDia";
            this.radioTCDia.Size = new System.Drawing.Size(122, 17);
            this.radioTCDia.TabIndex = 8;
            this.radioTCDia.TabStop = true;
            this.radioTCDia.Text = "De la fecha de corte";
            this.radioTCDia.UseVisualStyleBackColor = true;
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(316, 174);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 16;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_ControlClicked);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(397, 174);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 15;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_ControlClicked);
            // 
            // Frm_Cartera306090
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(484, 261);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkDetallado);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboConjunto);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboInmobiliaria);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 300);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "Frm_Cartera306090";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cartera vencida 30, 60, 90 y +90";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_Cartera306090_FormClosing);
            this.Load += new System.EventHandler(this.Frm_Cartera306090_Load);
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

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboInmobiliaria;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboConjunto;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.RadioButton radioPDF;
        private System.Windows.Forms.RadioButton radioExcel;
        private System.ComponentModel.BackgroundWorker workerReporte;
        private System.Windows.Forms.ProgressBar progreso;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkDetallado;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioIva;
        private System.Windows.Forms.RadioButton radioSinIva;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioTCEmision;
        private System.Windows.Forms.RadioButton radioTCDia;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonGenerar;
    }
}