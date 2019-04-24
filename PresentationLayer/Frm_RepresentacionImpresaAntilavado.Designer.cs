namespace GestorReportes.PresentationLayer
{
    partial class Frm_RepresentacionImpresaAntilavado
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_RepresentacionImpresaAntilavado));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.botonExaminar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.txtBxRutaArchivo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.opnFlDlgAntilav = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdBtnExcel = new System.Windows.Forms.RadioButton();
            this.rdBtnPdf = new System.Windows.Forms.RadioButton();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.groupBox1.Controls.Add(this.botonExaminar);
            this.groupBox1.Controls.Add(this.txtBxRutaArchivo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(15, 51);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(467, 99);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seleccione el archivo antilavado (.xml)";
            // 
            // botonExaminar
            // 
            this.botonExaminar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonExaminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonExaminar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonExaminar.Imagen = ((System.Drawing.Image)(resources.GetObject("botonExaminar.Imagen")));
            this.botonExaminar.Location = new System.Drawing.Point(386, 18);
            this.botonExaminar.Name = "botonExaminar";
            this.botonExaminar.Size = new System.Drawing.Size(75, 75);
            this.botonExaminar.TabIndex = 3;
            this.botonExaminar.Texto = "Examinar";
            this.botonExaminar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnExaminar_Click);
            // 
            // txtBxRutaArchivo
            // 
            this.txtBxRutaArchivo.Location = new System.Drawing.Point(58, 24);
            this.txtBxRutaArchivo.Name = "txtBxRutaArchivo";
            this.txtBxRutaArchivo.ReadOnly = true;
            this.txtBxRutaArchivo.Size = new System.Drawing.Size(322, 20);
            this.txtBxRutaArchivo.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Archivo:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(470, 44);
            this.label1.TabIndex = 2;
            this.label1.Text = "Seleccione el archivo de antilavado para el cual se desea generar una representac" +
                "ión impresa para la posterior revisión de datos contenidos en el mismo.";
            // 
            // opnFlDlgAntilav
            // 
            this.opnFlDlgAntilav.DefaultExt = "xml";
            this.opnFlDlgAntilav.Filter = "Archivos Antilavado|*.xml";
            this.opnFlDlgAntilav.Title = "Seleccione el archivo xml al que desea cambiar las alertas.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdBtnExcel);
            this.groupBox2.Controls.Add(this.rdBtnPdf);
            this.groupBox2.Location = new System.Drawing.Point(15, 156);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(289, 41);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Formato de representación impresa:";
            // 
            // rdBtnExcel
            // 
            this.rdBtnExcel.AutoSize = true;
            this.rdBtnExcel.Location = new System.Drawing.Point(128, 18);
            this.rdBtnExcel.Name = "rdBtnExcel";
            this.rdBtnExcel.Size = new System.Drawing.Size(51, 17);
            this.rdBtnExcel.TabIndex = 1;
            this.rdBtnExcel.Text = "Excel";
            this.rdBtnExcel.UseVisualStyleBackColor = true;
            // 
            // rdBtnPdf
            // 
            this.rdBtnPdf.AutoSize = true;
            this.rdBtnPdf.Checked = true;
            this.rdBtnPdf.Location = new System.Drawing.Point(9, 18);
            this.rdBtnPdf.Name = "rdBtnPdf";
            this.rdBtnPdf.Size = new System.Drawing.Size(49, 17);
            this.rdBtnPdf.TabIndex = 0;
            this.rdBtnPdf.TabStop = true;
            this.rdBtnPdf.Text = "PDF ";
            this.rdBtnPdf.UseVisualStyleBackColor = true;
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(15, 203);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(289, 23);
            this.progreso.TabIndex = 10;
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
            this.botonGenerar.Location = new System.Drawing.Point(320, 156);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 9;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnAceptar_Click);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(401, 156);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 8;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnCancelar_Click);
            // 
            // Frm_RepresentacionImpresaAntilavado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(494, 242);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 270);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 270);
            this.Name = "Frm_RepresentacionImpresaAntilavado";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Representación impresa de XML antilavado";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_RepresentacionImpresaAntilavado_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBxRutaArchivo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog opnFlDlgAntilav;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdBtnExcel;
        private System.Windows.Forms.RadioButton rdBtnPdf;
        private Controls.Ctrl_Opcion botonExaminar;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonGenerar;
        private System.Windows.Forms.ProgressBar progreso;
        private System.ComponentModel.BackgroundWorker workerReporte;
    }
}