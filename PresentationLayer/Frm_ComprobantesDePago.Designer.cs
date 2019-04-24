namespace GestorReportes.PresentationLayer
{
    partial class Frm_ComprobantesDePago
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ComprobantesDePago));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbSinDetalle = new System.Windows.Forms.RadioButton();
            this.rbConDetalle = new System.Windows.Forms.RadioButton();
            this.ctrl_OpcionCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.ctrl_OpcionGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbPDF = new System.Windows.Forms.RadioButton();
            this.rbEXCEL = new System.Windows.Forms.RadioButton();
            this.dtpfin = new System.Windows.Forms.DateTimePicker();
            this.dtpinicio = new System.Windows.Forms.DateTimePicker();
            this.cbxConjunto = new System.Windows.Forms.ComboBox();
            this.cbxInmobiliaria = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbSinDetalle);
            this.groupBox1.Controls.Add(this.rbConDetalle);
            this.groupBox1.Controls.Add(this.ctrl_OpcionCancelar);
            this.groupBox1.Controls.Add(this.ctrl_OpcionGenerar);
            this.groupBox1.Controls.Add(this.progreso);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.dtpfin);
            this.groupBox1.Controls.Add(this.dtpinicio);
            this.groupBox1.Controls.Add(this.cbxConjunto);
            this.groupBox1.Controls.Add(this.cbxInmobiliaria);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // rbSinDetalle
            // 
            resources.ApplyResources(this.rbSinDetalle, "rbSinDetalle");
            this.rbSinDetalle.Name = "rbSinDetalle";
            this.rbSinDetalle.TabStop = true;
            this.rbSinDetalle.UseVisualStyleBackColor = true;
            // 
            // rbConDetalle
            // 
            resources.ApplyResources(this.rbConDetalle, "rbConDetalle");
            this.rbConDetalle.Checked = true;
            this.rbConDetalle.Name = "rbConDetalle";
            this.rbConDetalle.TabStop = true;
            this.rbConDetalle.UseVisualStyleBackColor = true;
            // 
            // ctrl_OpcionCancelar
            // 
            this.ctrl_OpcionCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ctrl_OpcionCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ctrl_OpcionCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ctrl_OpcionCancelar.Imagen = ((System.Drawing.Image)(resources.GetObject("ctrl_OpcionCancelar.Imagen")));
            resources.ApplyResources(this.ctrl_OpcionCancelar, "ctrl_OpcionCancelar");
            this.ctrl_OpcionCancelar.Name = "ctrl_OpcionCancelar";
            this.ctrl_OpcionCancelar.Texto = "Cancelar";
            this.ctrl_OpcionCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.ctrl_OpcionCancelar_ControlClicked);
            // 
            // ctrl_OpcionGenerar
            // 
            this.ctrl_OpcionGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ctrl_OpcionGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ctrl_OpcionGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ctrl_OpcionGenerar.Imagen = ((System.Drawing.Image)(resources.GetObject("ctrl_OpcionGenerar.Imagen")));
            resources.ApplyResources(this.ctrl_OpcionGenerar, "ctrl_OpcionGenerar");
            this.ctrl_OpcionGenerar.Name = "ctrl_OpcionGenerar";
            this.ctrl_OpcionGenerar.Texto = "Generar";
            this.ctrl_OpcionGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.ctrl_OpcionGenerar_ControlClicked);
            // 
            // progreso
            // 
            resources.ApplyResources(this.progreso, "progreso");
            this.progreso.Name = "progreso";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbPDF);
            this.groupBox2.Controls.Add(this.rbEXCEL);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // rbPDF
            // 
            resources.ApplyResources(this.rbPDF, "rbPDF");
            this.rbPDF.Checked = true;
            this.rbPDF.Name = "rbPDF";
            this.rbPDF.TabStop = true;
            this.rbPDF.UseVisualStyleBackColor = true;
            // 
            // rbEXCEL
            // 
            resources.ApplyResources(this.rbEXCEL, "rbEXCEL");
            this.rbEXCEL.Name = "rbEXCEL";
            this.rbEXCEL.UseVisualStyleBackColor = true;
            // 
            // dtpfin
            // 
            resources.ApplyResources(this.dtpfin, "dtpfin");
            this.dtpfin.Name = "dtpfin";
            // 
            // dtpinicio
            // 
            resources.ApplyResources(this.dtpinicio, "dtpinicio");
            this.dtpinicio.Name = "dtpinicio";
            this.dtpinicio.Value = new System.DateTime(2018, 10, 26, 0, 0, 0, 0);
            // 
            // cbxConjunto
            // 
            this.cbxConjunto.FormattingEnabled = true;
            resources.ApplyResources(this.cbxConjunto, "cbxConjunto");
            this.cbxConjunto.Name = "cbxConjunto";
            // 
            // cbxInmobiliaria
            // 
            this.cbxInmobiliaria.FormattingEnabled = true;
            resources.ApplyResources(this.cbxInmobiliaria, "cbxInmobiliaria");
            this.cbxInmobiliaria.Name = "cbxInmobiliaria";
            this.cbxInmobiliaria.SelectedIndexChanged += new System.EventHandler(this.cbxInmobiliaria_SelectedIndexChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // worker
            // 
            this.worker.WorkerReportsProgress = true;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork_1);
            this.worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged_1);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted_1);
            // 
            // Frm_ComprobantesDePago
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "Frm_ComprobantesDePago";
            this.Load += new System.EventHandler(this.Frm_ComprobantesDePago_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbxConjunto;
        private System.Windows.Forms.ComboBox cbxInmobiliaria;
        private System.Windows.Forms.DateTimePicker dtpinicio;
        private System.Windows.Forms.DateTimePicker dtpfin;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbEXCEL;
        private System.Windows.Forms.RadioButton rbPDF;
        private System.Windows.Forms.ProgressBar progreso;
        private System.ComponentModel.BackgroundWorker worker;
        private Controls.Ctrl_Opcion ctrl_OpcionGenerar;
        private Controls.Ctrl_Opcion ctrl_OpcionCancelar;
        private System.Windows.Forms.RadioButton rbSinDetalle;
        private System.Windows.Forms.RadioButton rbConDetalle;
    }
}