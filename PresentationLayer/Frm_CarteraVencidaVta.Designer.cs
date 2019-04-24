namespace GestorReportes.PresentationLayer
{
    partial class Frm_CarteraVencidaVta
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
            this.gpBx = new System.Windows.Forms.GroupBox();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.radioPDF = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.cbBxEst = new System.Windows.Forms.ComboBox();
            this.cbBxConj = new System.Windows.Forms.ComboBox();
            this.cbBxEmp = new System.Windows.Forms.ComboBox();
            this.lblEst = new System.Windows.Forms.Label();
            this.lblConj = new System.Windows.Forms.Label();
            this.lblEmp = new System.Windows.Forms.Label();
            this.lblInst = new System.Windows.Forms.Label();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.gpBx.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpBx
            // 
            this.gpBx.Controls.Add(this.radioExcel);
            this.gpBx.Controls.Add(this.radioPDF);
            this.gpBx.Controls.Add(this.label1);
            this.gpBx.Controls.Add(this.cbBxEst);
            this.gpBx.Controls.Add(this.cbBxConj);
            this.gpBx.Controls.Add(this.cbBxEmp);
            this.gpBx.Controls.Add(this.lblEst);
            this.gpBx.Controls.Add(this.lblConj);
            this.gpBx.Controls.Add(this.lblEmp);
            this.gpBx.Location = new System.Drawing.Point(16, 42);
            this.gpBx.Name = "gpBx";
            this.gpBx.Size = new System.Drawing.Size(634, 138);
            this.gpBx.TabIndex = 0;
            this.gpBx.TabStop = false;
            this.gpBx.Text = "Datos de la empresa";
            // 
            // radioExcel
            // 
            this.radioExcel.AutoSize = true;
            this.radioExcel.Location = new System.Drawing.Point(149, 109);
            this.radioExcel.Name = "radioExcel";
            this.radioExcel.Size = new System.Drawing.Size(51, 17);
            this.radioExcel.TabIndex = 8;
            this.radioExcel.TabStop = true;
            this.radioExcel.Text = "Excel";
            this.radioExcel.UseVisualStyleBackColor = true;
            // 
            // radioPDF
            // 
            this.radioPDF.AutoSize = true;
            this.radioPDF.Checked = true;
            this.radioPDF.Location = new System.Drawing.Point(97, 108);
            this.radioPDF.Name = "radioPDF";
            this.radioPDF.Size = new System.Drawing.Size(46, 17);
            this.radioPDF.TabIndex = 7;
            this.radioPDF.TabStop = true;
            this.radioPDF.Text = "PDF";
            this.radioPDF.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Formato:";
            // 
            // cbBxEst
            // 
            this.cbBxEst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBxEst.FormattingEnabled = true;
            this.cbBxEst.Items.AddRange(new object[] {
            "*Todos"});
            this.cbBxEst.Location = new System.Drawing.Point(97, 81);
            this.cbBxEst.Name = "cbBxEst";
            this.cbBxEst.Size = new System.Drawing.Size(531, 21);
            this.cbBxEst.TabIndex = 5;
            // 
            // cbBxConj
            // 
            this.cbBxConj.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBxConj.FormattingEnabled = true;
            this.cbBxConj.Location = new System.Drawing.Point(97, 54);
            this.cbBxConj.Name = "cbBxConj";
            this.cbBxConj.Size = new System.Drawing.Size(531, 21);
            this.cbBxConj.TabIndex = 4;
            // 
            // cbBxEmp
            // 
            this.cbBxEmp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBxEmp.FormattingEnabled = true;
            this.cbBxEmp.Location = new System.Drawing.Point(97, 27);
            this.cbBxEmp.Name = "cbBxEmp";
            this.cbBxEmp.Size = new System.Drawing.Size(531, 21);
            this.cbBxEmp.TabIndex = 3;
            this.cbBxEmp.SelectedValueChanged += new System.EventHandler(this.cbBxEmp_SelectedValueChanged);
            // 
            // lblEst
            // 
            this.lblEst.AutoSize = true;
            this.lblEst.Location = new System.Drawing.Point(12, 84);
            this.lblEst.Name = "lblEst";
            this.lblEst.Size = new System.Drawing.Size(45, 13);
            this.lblEst.TabIndex = 2;
            this.lblEst.Text = "Estatus:";
            // 
            // lblConj
            // 
            this.lblConj.AutoSize = true;
            this.lblConj.Location = new System.Drawing.Point(5, 57);
            this.lblConj.Name = "lblConj";
            this.lblConj.Size = new System.Drawing.Size(52, 13);
            this.lblConj.TabIndex = 1;
            this.lblConj.Text = "Conjunto:";
            // 
            // lblEmp
            // 
            this.lblEmp.AutoSize = true;
            this.lblEmp.Location = new System.Drawing.Point(6, 30);
            this.lblEmp.Name = "lblEmp";
            this.lblEmp.Size = new System.Drawing.Size(51, 13);
            this.lblEmp.TabIndex = 0;
            this.lblEmp.Text = "Empresa:";
            // 
            // lblInst
            // 
            this.lblInst.AutoSize = true;
            this.lblInst.Location = new System.Drawing.Point(13, 13);
            this.lblInst.Name = "lblInst";
            this.lblInst.Size = new System.Drawing.Size(610, 26);
            this.lblInst.TabIndex = 1;
            this.lblInst.Text = "Seleccione los criterios deseados para la generación del reporte y a continuación" +
                " de clic en generar, se generará de acuerdo al \r\nformato solicitado.";
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(575, 186);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 2;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnCancelar_Click);
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(494, 186);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 3;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_ControlClicked);
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(16, 187);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(472, 23);
            this.progreso.TabIndex = 4;
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // Frm_CarteraVencidaVta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(669, 272);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.lblInst);
            this.Controls.Add(this.gpBx);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(675, 300);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(675, 300);
            this.Name = "Frm_CarteraVencidaVta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de Cartera Vencida";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_CarteraVencidaVta_FormClosing);
            this.Load += new System.EventHandler(this.Frm_CarteraVencidaVta_Load);
            this.gpBx.ResumeLayout(false);
            this.gpBx.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gpBx;
        private System.Windows.Forms.Label lblInst;
        private System.Windows.Forms.Label lblEst;
        private System.Windows.Forms.Label lblConj;
        private System.Windows.Forms.Label lblEmp;
        private System.Windows.Forms.ComboBox cbBxEst;
        private System.Windows.Forms.ComboBox cbBxConj;
        private System.Windows.Forms.ComboBox cbBxEmp;
        private System.Windows.Forms.RadioButton radioExcel;
        private System.Windows.Forms.RadioButton radioPDF;
        private System.Windows.Forms.Label label1;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonGenerar;
        private System.Windows.Forms.ProgressBar progreso;
        private System.ComponentModel.BackgroundWorker workerReporte;
    }
}