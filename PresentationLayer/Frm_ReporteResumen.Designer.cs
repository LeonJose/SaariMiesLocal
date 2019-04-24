namespace GestorReportes.PresentationLayer
{
    partial class Frm_ReporteResumen
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
            this.lblDescr = new System.Windows.Forms.Label();
            this.grpBxParametros = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBxTipoCambio = new System.Windows.Forms.TextBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.cmbBxGpoEmp = new System.Windows.Forms.ComboBox();
            this.lblVigencia = new System.Windows.Forms.Label();
            this.lblGpo = new System.Windows.Forms.Label();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.grpBxParametros.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDescr
            // 
            this.lblDescr.AutoSize = true;
            this.lblDescr.Location = new System.Drawing.Point(13, 13);
            this.lblDescr.Name = "lblDescr";
            this.lblDescr.Size = new System.Drawing.Size(526, 26);
            this.lblDescr.TabIndex = 0;
            this.lblDescr.Text = "Creación de Reporte Resumen. Seleccione el grupo empresarial y la fecha de fin de" +
                " vigencia de los contratos.\r\nPosteriormente dé clic en Generar Reporte";
            // 
            // grpBxParametros
            // 
            this.grpBxParametros.Controls.Add(this.label1);
            this.grpBxParametros.Controls.Add(this.txtBxTipoCambio);
            this.grpBxParametros.Controls.Add(this.dateTimePicker1);
            this.grpBxParametros.Controls.Add(this.cmbBxGpoEmp);
            this.grpBxParametros.Controls.Add(this.lblVigencia);
            this.grpBxParametros.Controls.Add(this.lblGpo);
            this.grpBxParametros.Location = new System.Drawing.Point(16, 42);
            this.grpBxParametros.Name = "grpBxParametros";
            this.grpBxParametros.Size = new System.Drawing.Size(520, 113);
            this.grpBxParametros.TabIndex = 1;
            this.grpBxParametros.TabStop = false;
            this.grpBxParametros.Text = "Selección de criterios";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Tipo de cambio:";
            // 
            // txtBxTipoCambio
            // 
            this.txtBxTipoCambio.Location = new System.Drawing.Point(151, 72);
            this.txtBxTipoCambio.Name = "txtBxTipoCambio";
            this.txtBxTipoCambio.Size = new System.Drawing.Size(100, 20);
            this.txtBxTipoCambio.TabIndex = 4;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(151, 46);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(363, 20);
            this.dateTimePicker1.TabIndex = 3;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // cmbBxGpoEmp
            // 
            this.cmbBxGpoEmp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxGpoEmp.DropDownWidth = 364;
            this.cmbBxGpoEmp.FormattingEnabled = true;
            this.cmbBxGpoEmp.Location = new System.Drawing.Point(151, 19);
            this.cmbBxGpoEmp.Name = "cmbBxGpoEmp";
            this.cmbBxGpoEmp.Size = new System.Drawing.Size(363, 21);
            this.cmbBxGpoEmp.TabIndex = 2;
            this.cmbBxGpoEmp.SelectedIndexChanged += new System.EventHandler(this.cmbBxGpoEmp_SelectedIndexChanged);
            // 
            // lblVigencia
            // 
            this.lblVigencia.AutoSize = true;
            this.lblVigencia.Location = new System.Drawing.Point(39, 52);
            this.lblVigencia.Name = "lblVigencia";
            this.lblVigencia.Size = new System.Drawing.Size(62, 13);
            this.lblVigencia.TabIndex = 1;
            this.lblVigencia.Text = "Vigentes al:";
            // 
            // lblGpo
            // 
            this.lblGpo.AutoSize = true;
            this.lblGpo.Location = new System.Drawing.Point(6, 22);
            this.lblGpo.Name = "lblGpo";
            this.lblGpo.Size = new System.Drawing.Size(95, 13);
            this.lblGpo.TabIndex = 0;
            this.lblGpo.Text = "Grupo empresarial:";
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(461, 161);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 3;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnCancelar_Click);
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(380, 161);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 4;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnGenerarReporte_Click);
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(16, 213);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(358, 23);
            this.progreso.TabIndex = 5;
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // Frm_ReporteResumen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(589, 252);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.grpBxParametros);
            this.Controls.Add(this.lblDescr);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(595, 280);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(595, 280);
            this.Name = "Frm_ReporteResumen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte Resumen";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_ReporteResumen_FormClosing);
            this.Load += new System.EventHandler(this.Frm_ReporteResumen_Load);
            this.grpBxParametros.ResumeLayout(false);
            this.grpBxParametros.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDescr;
        private System.Windows.Forms.GroupBox grpBxParametros;
        private System.Windows.Forms.Label lblGpo;
        private System.Windows.Forms.ComboBox cmbBxGpoEmp;
        private System.Windows.Forms.Label lblVigencia;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBxTipoCambio;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonGenerar;
        private System.Windows.Forms.ProgressBar progreso;
        private System.ComponentModel.BackgroundWorker workerReporte;
    }
}