namespace GestorReportes.PresentationLayer
{
    partial class Frm_CXP306090
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
            this.components = new System.ComponentModel.Container();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioIva = new System.Windows.Forms.RadioButton();
            this.radioSinIva = new System.Windows.Forms.RadioButton();
            this.checkDetallado = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioPDF = new System.Windows.Forms.RadioButton();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.dateFechaCorte = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.comboInmobiliaria = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxTipoCambio = new System.Windows.Forms.TextBox();
            this.errorProviderTC = new System.Windows.Forms.ErrorProvider(this.components);
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderTC)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioIva);
            this.groupBox2.Controls.Add(this.radioSinIva);
            this.groupBox2.Location = new System.Drawing.Point(14, 142);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(243, 45);
            this.groupBox2.TabIndex = 4;
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
            this.radioSinIva.Text = "Sin impuestos";
            this.radioSinIva.UseVisualStyleBackColor = true;
            // 
            // checkDetallado
            // 
            this.checkDetallado.AutoSize = true;
            this.checkDetallado.Location = new System.Drawing.Point(326, 112);
            this.checkDetallado.Name = "checkDetallado";
            this.checkDetallado.Size = new System.Drawing.Size(144, 17);
            this.checkDetallado.TabIndex = 3;
            this.checkDetallado.Text = "Incluir detalle de Egresos";
            this.checkDetallado.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioPDF);
            this.groupBox1.Controls.Add(this.radioExcel);
            this.groupBox1.Location = new System.Drawing.Point(15, 193);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(207, 45);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Formato";
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
            this.radioExcel.Location = new System.Drawing.Point(132, 18);
            this.radioExcel.Name = "radioExcel";
            this.radioExcel.Size = new System.Drawing.Size(51, 17);
            this.radioExcel.TabIndex = 8;
            this.radioExcel.TabStop = true;
            this.radioExcel.Text = "Excel";
            this.radioExcel.UseVisualStyleBackColor = true;
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(15, 257);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(284, 23);
            this.progreso.TabIndex = 8;
            // 
            // dateFechaCorte
            // 
            this.dateFechaCorte.Location = new System.Drawing.Point(101, 80);
            this.dateFechaCorte.Name = "dateFechaCorte";
            this.dateFechaCorte.Size = new System.Drawing.Size(200, 20);
            this.dateFechaCorte.TabIndex = 1;
            this.dateFechaCorte.ValueChanged += new System.EventHandler(this.dateFechaCorte_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Fecha de corte:";
            // 
            // comboInmobiliaria
            // 
            this.comboInmobiliaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInmobiliaria.FormattingEnabled = true;
            this.comboInmobiliaria.Location = new System.Drawing.Point(100, 53);
            this.comboInmobiliaria.Name = "comboInmobiliaria";
            this.comboInmobiliaria.Size = new System.Drawing.Size(370, 21);
            this.comboInmobiliaria.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Inmobiliaria:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(11, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(472, 29);
            this.label1.TabIndex = 17;
            this.label1.Text = "Seleccione la inmobilaria, la fecha de corte, capture el tipo de cambio e indique" +
    " los criterios y formato del reporte, posteriormente haga clic en generar";
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.WorkerSupportsCancellation = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(11, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(226, 26);
            this.label3.TabIndex = 31;
            this.label3.Text = "Tipo de Cambio de Fecha de Corte:";
            // 
            // textBoxTipoCambio
            // 
            this.textBoxTipoCambio.Location = new System.Drawing.Point(199, 110);
            this.textBoxTipoCambio.Name = "textBoxTipoCambio";
            this.textBoxTipoCambio.Size = new System.Drawing.Size(100, 20);
            this.textBoxTipoCambio.TabIndex = 2;
            this.textBoxTipoCambio.Text = "0.0000";
            this.textBoxTipoCambio.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxTipoCambio.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxTipoCambio_Validating);
            // 
            // errorProviderTC
            // 
            this.errorProviderTC.ContainerControl = this;
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(314, 205);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 6;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_ControlClicked);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(395, 205);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 7;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_ControlClicked);
            // 
            // Frm_CXP306090
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(494, 292);
            this.Controls.Add(this.textBoxTipoCambio);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkDetallado);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.dateFechaCorte);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboInmobiliaria);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 320);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 320);
            this.Name = "Frm_CXP306090";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de Antigüedad de CXP a 30, 60, 90, y 90+";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_Cartera306090_FormClosing);
            this.Load += new System.EventHandler(this.Frm_CXP306090_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderTC)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.Ctrl_Opcion botonGenerar;
        private Controls.Ctrl_Opcion botonCancelar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioIva;
        private System.Windows.Forms.RadioButton radioSinIva;
        private System.Windows.Forms.CheckBox checkDetallado;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioPDF;
        private System.Windows.Forms.RadioButton radioExcel;
        private System.Windows.Forms.ProgressBar progreso;
        private System.Windows.Forms.DateTimePicker dateFechaCorte;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboInmobiliaria;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker workerReporte;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxTipoCambio;
        private System.Windows.Forms.ErrorProvider errorProviderTC;
    }
}