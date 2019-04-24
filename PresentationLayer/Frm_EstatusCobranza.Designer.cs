namespace GestorReportes.PresentationLayer
{
    partial class Frm_EstatusCobranza
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_EstatusCobranza));
            this.gbxInmobiliaria = new System.Windows.Forms.GroupBox();
            this.cbxConjunto = new System.Windows.Forms.ComboBox();
            this.lblConjunto = new System.Windows.Forms.Label();
            this.gbxConjuntos = new System.Windows.Forms.GroupBox();
            this.rbtnConjuntosxSubconjuntos = new System.Windows.Forms.RadioButton();
            this.rbtnUnConjunto = new System.Windows.Forms.RadioButton();
            this.rbtnTodosConjuntos = new System.Windows.Forms.RadioButton();
            this.cbxInmobiliaria = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnFolioRecibo = new System.Windows.Forms.RadioButton();
            this.rbtnFolioCfd = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtfin = new System.Windows.Forms.DateTimePicker();
            this.dtinicio = new System.Windows.Forms.DateTimePicker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Generar = new System.Windows.Forms.Button();
            this.Worker = new System.ComponentModel.BackgroundWorker();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioPDF = new System.Windows.Forms.RadioButton();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.gbxInmobiliaria.SuspendLayout();
            this.gbxConjuntos.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxInmobiliaria
            // 
            this.gbxInmobiliaria.Controls.Add(this.cbxConjunto);
            this.gbxInmobiliaria.Controls.Add(this.lblConjunto);
            this.gbxInmobiliaria.Controls.Add(this.gbxConjuntos);
            this.gbxInmobiliaria.Controls.Add(this.cbxInmobiliaria);
            this.gbxInmobiliaria.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxInmobiliaria.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.gbxInmobiliaria.Location = new System.Drawing.Point(12, 12);
            this.gbxInmobiliaria.Name = "gbxInmobiliaria";
            this.gbxInmobiliaria.Size = new System.Drawing.Size(407, 238);
            this.gbxInmobiliaria.TabIndex = 2;
            this.gbxInmobiliaria.TabStop = false;
            this.gbxInmobiliaria.Text = "  Inmobiliaira  ";
            // 
            // cbxConjunto
            // 
            this.cbxConjunto.FormattingEnabled = true;
            this.cbxConjunto.Location = new System.Drawing.Point(18, 201);
            this.cbxConjunto.Name = "cbxConjunto";
            this.cbxConjunto.Size = new System.Drawing.Size(369, 23);
            this.cbxConjunto.TabIndex = 3;
            // 
            // lblConjunto
            // 
            this.lblConjunto.AutoSize = true;
            this.lblConjunto.Location = new System.Drawing.Point(15, 183);
            this.lblConjunto.Name = "lblConjunto";
            this.lblConjunto.Size = new System.Drawing.Size(56, 15);
            this.lblConjunto.TabIndex = 2;
            this.lblConjunto.Text = "Conjunto";
            // 
            // gbxConjuntos
            // 
            this.gbxConjuntos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.gbxConjuntos.Controls.Add(this.rbtnConjuntosxSubconjuntos);
            this.gbxConjuntos.Controls.Add(this.rbtnUnConjunto);
            this.gbxConjuntos.Controls.Add(this.rbtnTodosConjuntos);
            this.gbxConjuntos.Location = new System.Drawing.Point(13, 60);
            this.gbxConjuntos.Name = "gbxConjuntos";
            this.gbxConjuntos.Size = new System.Drawing.Size(381, 120);
            this.gbxConjuntos.TabIndex = 1;
            this.gbxConjuntos.TabStop = false;
            this.gbxConjuntos.Text = "Clasificacion de Conjuntos";
            // 
            // rbtnConjuntosxSubconjuntos
            // 
            this.rbtnConjuntosxSubconjuntos.AutoSize = true;
            this.rbtnConjuntosxSubconjuntos.Location = new System.Drawing.Point(12, 90);
            this.rbtnConjuntosxSubconjuntos.Name = "rbtnConjuntosxSubconjuntos";
            this.rbtnConjuntosxSubconjuntos.Size = new System.Drawing.Size(358, 19);
            this.rbtnConjuntosxSubconjuntos.TabIndex = 4;
            this.rbtnConjuntosxSubconjuntos.TabStop = true;
            this.rbtnConjuntosxSubconjuntos.Text = "Todos los Conjuntos Ordenados por Conjuntos-Subconjuntos";
            this.rbtnConjuntosxSubconjuntos.UseVisualStyleBackColor = true;
            this.rbtnConjuntosxSubconjuntos.CheckedChanged += new System.EventHandler(this.rbtnConjuntosxSubconjuntos_CheckedChanged);
            // 
            // rbtnUnConjunto
            // 
            this.rbtnUnConjunto.AutoSize = true;
            this.rbtnUnConjunto.Location = new System.Drawing.Point(12, 64);
            this.rbtnUnConjunto.Name = "rbtnUnConjunto";
            this.rbtnUnConjunto.Size = new System.Drawing.Size(159, 19);
            this.rbtnUnConjunto.TabIndex = 2;
            this.rbtnUnConjunto.TabStop = true;
            this.rbtnUnConjunto.Text = "Seleccionar un Conjunto";
            this.rbtnUnConjunto.UseVisualStyleBackColor = true;
            this.rbtnUnConjunto.CheckedChanged += new System.EventHandler(this.rbtnUnConjunto_CheckedChanged);
            // 
            // rbtnTodosConjuntos
            // 
            this.rbtnTodosConjuntos.AutoSize = true;
            this.rbtnTodosConjuntos.Checked = true;
            this.rbtnTodosConjuntos.Location = new System.Drawing.Point(12, 38);
            this.rbtnTodosConjuntos.Name = "rbtnTodosConjuntos";
            this.rbtnTodosConjuntos.Size = new System.Drawing.Size(136, 19);
            this.rbtnTodosConjuntos.TabIndex = 0;
            this.rbtnTodosConjuntos.TabStop = true;
            this.rbtnTodosConjuntos.Text = "Todos los Conjuntos";
            this.rbtnTodosConjuntos.UseVisualStyleBackColor = true;
            this.rbtnTodosConjuntos.CheckedChanged += new System.EventHandler(this.rbtnTodosConjuntos_CheckedChanged);
            // 
            // cbxInmobiliaria
            // 
            this.cbxInmobiliaria.FormattingEnabled = true;
            this.cbxInmobiliaria.Location = new System.Drawing.Point(18, 29);
            this.cbxInmobiliaria.Name = "cbxInmobiliaria";
            this.cbxInmobiliaria.Size = new System.Drawing.Size(369, 23);
            this.cbxInmobiliaria.TabIndex = 0;
            this.cbxInmobiliaria.SelectedIndexChanged += new System.EventHandler(this.cbxInmobiliaria_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtnFolioRecibo);
            this.groupBox1.Controls.Add(this.rbtnFolioCfd);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 256);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 49);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "  Ordenado por  ";
            // 
            // rbtnFolioRecibo
            // 
            this.rbtnFolioRecibo.AutoSize = true;
            this.rbtnFolioRecibo.Location = new System.Drawing.Point(265, 20);
            this.rbtnFolioRecibo.Name = "rbtnFolioRecibo";
            this.rbtnFolioRecibo.Size = new System.Drawing.Size(94, 19);
            this.rbtnFolioRecibo.TabIndex = 1;
            this.rbtnFolioRecibo.TabStop = true;
            this.rbtnFolioRecibo.Text = "Folio Recibo";
            this.rbtnFolioRecibo.UseVisualStyleBackColor = true;
            // 
            // rbtnFolioCfd
            // 
            this.rbtnFolioCfd.AutoSize = true;
            this.rbtnFolioCfd.Checked = true;
            this.rbtnFolioCfd.Location = new System.Drawing.Point(59, 20);
            this.rbtnFolioCfd.Name = "rbtnFolioCfd";
            this.rbtnFolioCfd.Size = new System.Drawing.Size(79, 19);
            this.rbtnFolioCfd.TabIndex = 0;
            this.rbtnFolioCfd.TabStop = true;
            this.rbtnFolioCfd.Text = "Folio CFD";
            this.rbtnFolioCfd.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.dtfin);
            this.groupBox2.Controls.Add(this.dtinicio);
            this.groupBox2.Location = new System.Drawing.Point(12, 311);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(400, 65);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "  Rango de Fechas  ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(249, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 15);
            this.label5.TabIndex = 3;
            this.label5.Text = "al :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(14, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "Del :";
            // 
            // dtfin
            // 
            this.dtfin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtfin.Location = new System.Drawing.Point(278, 33);
            this.dtfin.Name = "dtfin";
            this.dtfin.Size = new System.Drawing.Size(108, 20);
            this.dtfin.TabIndex = 1;
            // 
            // dtinicio
            // 
            this.dtinicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtinicio.Location = new System.Drawing.Point(52, 33);
            this.dtinicio.Name = "dtinicio";
            this.dtinicio.Size = new System.Drawing.Size(120, 20);
            this.dtinicio.TabIndex = 0;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(29, 479);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(383, 24);
            this.progressBar1.TabIndex = 5;
            // 
            // Generar
            // 
            this.Generar.Image = global::GestorReportes.Properties.Resources.reportCh;
            this.Generar.Location = new System.Drawing.Point(252, 382);
            this.Generar.Name = "Generar";
            this.Generar.Size = new System.Drawing.Size(74, 76);
            this.Generar.TabIndex = 6;
            this.Generar.Text = "Generar";
            this.Generar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Generar.UseVisualStyleBackColor = true;
            this.Generar.Click += new System.EventHandler(this.Generar_Click);
            // 
            // Worker
            // 
            this.Worker.WorkerReportsProgress = true;
            this.Worker.WorkerSupportsCancellation = true;
            this.Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Worker_DoWork);
            this.Worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.Worker_ProgressChanged);
            this.Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.Worker_RunWorkerCompleted);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioPDF);
            this.groupBox3.Controls.Add(this.radioExcel);
            this.groupBox3.Location = new System.Drawing.Point(30, 403);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(143, 41);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Formato:";
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
            // btnCancelar
            // 
            this.btnCancelar.ForeColor = System.Drawing.Color.Black;
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.Location = new System.Drawing.Point(337, 382);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 76);
            this.btnCancelar.TabIndex = 23;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // Frm_EstatusCobranza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(429, 520);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.Generar);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbxInmobiliaria);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_EstatusCobranza";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Estatus de Cobranza";
            this.Load += new System.EventHandler(this.Frm_EstatusCobranza_Load);
            this.gbxInmobiliaria.ResumeLayout(false);
            this.gbxInmobiliaria.PerformLayout();
            this.gbxConjuntos.ResumeLayout(false);
            this.gbxConjuntos.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxInmobiliaria;
        private System.Windows.Forms.ComboBox cbxConjunto;
        private System.Windows.Forms.Label lblConjunto;
        private System.Windows.Forms.GroupBox gbxConjuntos;
        private System.Windows.Forms.RadioButton rbtnConjuntosxSubconjuntos;
        private System.Windows.Forms.RadioButton rbtnUnConjunto;
        private System.Windows.Forms.RadioButton rbtnTodosConjuntos;
        private System.Windows.Forms.ComboBox cbxInmobiliaria;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtnFolioRecibo;
        private System.Windows.Forms.RadioButton rbtnFolioCfd;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtfin;
        private System.Windows.Forms.DateTimePicker dtinicio;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button Generar;
        private System.ComponentModel.BackgroundWorker Worker;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioPDF;
        private System.Windows.Forms.RadioButton radioExcel;
        private System.Windows.Forms.Button btnCancelar;
    }
}