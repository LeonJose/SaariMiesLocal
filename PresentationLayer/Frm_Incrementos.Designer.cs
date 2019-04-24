namespace GestorReportes.PresentationLayer
{
    partial class Frm_Incrementos
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
            this.gpBxCriterios = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBxTipoCambio = new System.Windows.Forms.TextBox();
            this.cmbBxClient = new System.Windows.Forms.ComboBox();
            this.cmbBxSubConj = new System.Windows.Forms.ComboBox();
            this.cmbBxConj = new System.Windows.Forms.ComboBox();
            this.cmbBxInmob = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBxGpoEmp = new System.Windows.Forms.ComboBox();
            this.lblGpo = new System.Windows.Forms.Label();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.gpBxCriterios.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDescr
            // 
            this.lblDescr.AutoSize = true;
            this.lblDescr.Location = new System.Drawing.Point(12, 9);
            this.lblDescr.Name = "lblDescr";
            this.lblDescr.Size = new System.Drawing.Size(501, 26);
            this.lblDescr.TabIndex = 1;
            this.lblDescr.Text = "Creación de Reporte Incrementos. Seleccione el grupo empresarial,  inmobiliaria, " +
                "conjunto y subconjunto.\r\nPosteriormente dé clic en Generar Reporte";
            // 
            // gpBxCriterios
            // 
            this.gpBxCriterios.Controls.Add(this.label5);
            this.gpBxCriterios.Controls.Add(this.txtBxTipoCambio);
            this.gpBxCriterios.Controls.Add(this.cmbBxClient);
            this.gpBxCriterios.Controls.Add(this.cmbBxSubConj);
            this.gpBxCriterios.Controls.Add(this.cmbBxConj);
            this.gpBxCriterios.Controls.Add(this.cmbBxInmob);
            this.gpBxCriterios.Controls.Add(this.label4);
            this.gpBxCriterios.Controls.Add(this.label3);
            this.gpBxCriterios.Controls.Add(this.label2);
            this.gpBxCriterios.Controls.Add(this.label1);
            this.gpBxCriterios.Controls.Add(this.cmbBxGpoEmp);
            this.gpBxCriterios.Controls.Add(this.lblGpo);
            this.gpBxCriterios.Location = new System.Drawing.Point(15, 38);
            this.gpBxCriterios.Name = "gpBxCriterios";
            this.gpBxCriterios.Size = new System.Drawing.Size(529, 188);
            this.gpBxCriterios.TabIndex = 2;
            this.gpBxCriterios.TabStop = false;
            this.gpBxCriterios.Text = "Selección de criterios";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Tipo de cambio:";
            // 
            // txtBxTipoCambio
            // 
            this.txtBxTipoCambio.Location = new System.Drawing.Point(150, 154);
            this.txtBxTipoCambio.Name = "txtBxTipoCambio";
            this.txtBxTipoCambio.Size = new System.Drawing.Size(100, 20);
            this.txtBxTipoCambio.TabIndex = 13;
            // 
            // cmbBxClient
            // 
            this.cmbBxClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxClient.FormattingEnabled = true;
            this.cmbBxClient.Location = new System.Drawing.Point(150, 127);
            this.cmbBxClient.Name = "cmbBxClient";
            this.cmbBxClient.Size = new System.Drawing.Size(363, 21);
            this.cmbBxClient.TabIndex = 12;
            // 
            // cmbBxSubConj
            // 
            this.cmbBxSubConj.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxSubConj.FormattingEnabled = true;
            this.cmbBxSubConj.Location = new System.Drawing.Point(150, 100);
            this.cmbBxSubConj.Name = "cmbBxSubConj";
            this.cmbBxSubConj.Size = new System.Drawing.Size(363, 21);
            this.cmbBxSubConj.TabIndex = 11;
            this.cmbBxSubConj.SelectedIndexChanged += new System.EventHandler(this.cmbBxSubConj_SelectedIndexChanged);
            // 
            // cmbBxConj
            // 
            this.cmbBxConj.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxConj.FormattingEnabled = true;
            this.cmbBxConj.Location = new System.Drawing.Point(150, 73);
            this.cmbBxConj.Name = "cmbBxConj";
            this.cmbBxConj.Size = new System.Drawing.Size(363, 21);
            this.cmbBxConj.TabIndex = 10;
            this.cmbBxConj.SelectedIndexChanged += new System.EventHandler(this.cmbBxConj_SelectedIndexChanged);
            // 
            // cmbBxInmob
            // 
            this.cmbBxInmob.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxInmob.FormattingEnabled = true;
            this.cmbBxInmob.Location = new System.Drawing.Point(150, 46);
            this.cmbBxInmob.Name = "cmbBxInmob";
            this.cmbBxInmob.Size = new System.Drawing.Size(363, 21);
            this.cmbBxInmob.TabIndex = 9;
            this.cmbBxInmob.SelectedIndexChanged += new System.EventHandler(this.cmbBxInmob_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(59, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Cliente:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "SubConjunto:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Conjunto:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Inmobiliaria:";
            // 
            // cmbBxGpoEmp
            // 
            this.cmbBxGpoEmp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxGpoEmp.DropDownWidth = 364;
            this.cmbBxGpoEmp.FormattingEnabled = true;
            this.cmbBxGpoEmp.Location = new System.Drawing.Point(150, 19);
            this.cmbBxGpoEmp.Name = "cmbBxGpoEmp";
            this.cmbBxGpoEmp.Size = new System.Drawing.Size(363, 21);
            this.cmbBxGpoEmp.TabIndex = 4;
            this.cmbBxGpoEmp.SelectedIndexChanged += new System.EventHandler(this.cmbBxGpoEmp_SelectedIndexChanged);
            // 
            // lblGpo
            // 
            this.lblGpo.AutoSize = true;
            this.lblGpo.Location = new System.Drawing.Point(6, 22);
            this.lblGpo.Name = "lblGpo";
            this.lblGpo.Size = new System.Drawing.Size(95, 13);
            this.lblGpo.TabIndex = 3;
            this.lblGpo.Text = "Grupo empresarial:";
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(469, 232);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 4;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_ControlClicked);
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(388, 232);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 5;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnGenerarReporte_Click);
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(15, 284);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(367, 23);
            this.progreso.TabIndex = 6;
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // Frm_Incrementos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(589, 322);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.gpBxCriterios);
            this.Controls.Add(this.lblDescr);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(595, 350);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(595, 350);
            this.Name = "Frm_Incrementos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte Incrementos";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_Incrementos_FormClosing);
            this.Load += new System.EventHandler(this.Frm_Incrementos_Load);
            this.gpBxCriterios.ResumeLayout(false);
            this.gpBxCriterios.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDescr;
        private System.Windows.Forms.GroupBox gpBxCriterios;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbBxGpoEmp;
        private System.Windows.Forms.Label lblGpo;
        private System.Windows.Forms.ComboBox cmbBxClient;
        private System.Windows.Forms.ComboBox cmbBxSubConj;
        private System.Windows.Forms.ComboBox cmbBxConj;
        private System.Windows.Forms.ComboBox cmbBxInmob;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBxTipoCambio;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonGenerar;
        private System.Windows.Forms.ProgressBar progreso;
        private System.ComponentModel.BackgroundWorker workerReporte;
    }
}