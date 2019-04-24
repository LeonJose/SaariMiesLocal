namespace GestorReportes.PresentationLayer
{
    partial class Frm_RecibosCobradoPorFolio
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_RecibosCobradoPorFolio));
            this.cmbInmobiliaria = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rBallOrder = new System.Windows.Forms.RadioButton();
            this.rBSelectConjunto = new System.Windows.Forms.RadioButton();
            this.rBallConjuntos = new System.Windows.Forms.RadioButton();
            this.conjunto = new System.Windows.Forms.Label();
            this.cmbConjunto = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rBOrderRec = new System.Windows.Forms.RadioButton();
            this.rBOrderFolio = new System.Windows.Forms.RadioButton();
            this.chkIncluirCargosPeriodicos = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dtpFechaInicial = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaFinal = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.rbEsPDF = new System.Windows.Forms.RadioButton();
            this.rbExcel = new System.Windows.Forms.RadioButton();
            this.chkAbrir = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbInmobiliaria
            // 
            this.cmbInmobiliaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInmobiliaria.FormattingEnabled = true;
            this.cmbInmobiliaria.Location = new System.Drawing.Point(66, 21);
            this.cmbInmobiliaria.Name = "cmbInmobiliaria";
            this.cmbInmobiliaria.Size = new System.Drawing.Size(299, 21);
            this.cmbInmobiliaria.TabIndex = 0;
            this.cmbInmobiliaria.SelectedIndexChanged += new System.EventHandler(this.cmbInmobiliaria_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Inmobiliaria";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rBallOrder);
            this.groupBox1.Controls.Add(this.rBSelectConjunto);
            this.groupBox1.Controls.Add(this.rBallConjuntos);
            this.groupBox1.Location = new System.Drawing.Point(41, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(324, 98);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Clasificacion de Conjuntos";
            // 
            // rBallOrder
            // 
            this.rBallOrder.AutoSize = true;
            this.rBallOrder.Location = new System.Drawing.Point(7, 68);
            this.rBallOrder.Name = "rBallOrder";
            this.rBallOrder.Size = new System.Drawing.Size(300, 17);
            this.rBallOrder.TabIndex = 2;
            this.rBallOrder.Text = "Todos los conjuntos Ordenados por conjunto-Subconjunto";
            this.rBallOrder.UseVisualStyleBackColor = true;
            // 
            // rBSelectConjunto
            // 
            this.rBSelectConjunto.AutoSize = true;
            this.rBSelectConjunto.Location = new System.Drawing.Point(7, 44);
            this.rBSelectConjunto.Name = "rBSelectConjunto";
            this.rBSelectConjunto.Size = new System.Drawing.Size(125, 17);
            this.rBSelectConjunto.TabIndex = 1;
            this.rBSelectConjunto.Text = "Seleccionar conjunto";
            this.rBSelectConjunto.UseVisualStyleBackColor = true;
            this.rBSelectConjunto.CheckedChanged += new System.EventHandler(this.rBSelectConjunto_CheckedChanged);
            // 
            // rBallConjuntos
            // 
            this.rBallConjuntos.AutoSize = true;
            this.rBallConjuntos.Checked = true;
            this.rBallConjuntos.Location = new System.Drawing.Point(7, 20);
            this.rBallConjuntos.Name = "rBallConjuntos";
            this.rBallConjuntos.Size = new System.Drawing.Size(120, 17);
            this.rBallConjuntos.TabIndex = 0;
            this.rBallConjuntos.TabStop = true;
            this.rBallConjuntos.Text = "Todos los conjuntos";
            this.rBallConjuntos.UseVisualStyleBackColor = true;
            // 
            // conjunto
            // 
            this.conjunto.AutoSize = true;
            this.conjunto.Location = new System.Drawing.Point(45, 175);
            this.conjunto.Name = "conjunto";
            this.conjunto.Size = new System.Drawing.Size(49, 13);
            this.conjunto.TabIndex = 3;
            this.conjunto.Text = "Conjunto";
            this.conjunto.Visible = false;
            // 
            // cmbConjunto
            // 
            this.cmbConjunto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConjunto.FormattingEnabled = true;
            this.cmbConjunto.Location = new System.Drawing.Point(48, 191);
            this.cmbConjunto.Name = "cmbConjunto";
            this.cmbConjunto.Size = new System.Drawing.Size(303, 21);
            this.cmbConjunto.TabIndex = 4;
            this.cmbConjunto.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rBOrderRec);
            this.groupBox2.Controls.Add(this.rBOrderFolio);
            this.groupBox2.Location = new System.Drawing.Point(36, 219);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(326, 56);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ordenar por";
            // 
            // rBOrderRec
            // 
            this.rBOrderRec.AutoSize = true;
            this.rBOrderRec.Location = new System.Drawing.Point(162, 20);
            this.rBOrderRec.Name = "rBOrderRec";
            this.rBOrderRec.Size = new System.Drawing.Size(79, 17);
            this.rBOrderRec.TabIndex = 1;
            this.rBOrderRec.Text = "Folio recibo";
            this.rBOrderRec.UseVisualStyleBackColor = true;
            // 
            // rBOrderFolio
            // 
            this.rBOrderFolio.AutoSize = true;
            this.rBOrderFolio.Checked = true;
            this.rBOrderFolio.Location = new System.Drawing.Point(12, 20);
            this.rBOrderFolio.Name = "rBOrderFolio";
            this.rBOrderFolio.Size = new System.Drawing.Size(71, 17);
            this.rBOrderFolio.TabIndex = 0;
            this.rBOrderFolio.TabStop = true;
            this.rBOrderFolio.Text = "Folio CFD";
            this.rBOrderFolio.UseVisualStyleBackColor = true;
            // 
            // chkIncluirCargosPeriodicos
            // 
            this.chkIncluirCargosPeriodicos.AutoSize = true;
            this.chkIncluirCargosPeriodicos.Location = new System.Drawing.Point(48, 296);
            this.chkIncluirCargosPeriodicos.Name = "chkIncluirCargosPeriodicos";
            this.chkIncluirCargosPeriodicos.Size = new System.Drawing.Size(182, 17);
            this.chkIncluirCargosPeriodicos.TabIndex = 6;
            this.chkIncluirCargosPeriodicos.Text = "Incluir CFDI de cargos periodicos";
            this.chkIncluirCargosPeriodicos.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dtpFechaInicial);
            this.groupBox3.Controls.Add(this.dtpFechaFinal);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(72, 319);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(249, 118);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Rango de fechas";
            // 
            // dtpFechaInicial
            // 
            this.dtpFechaInicial.Location = new System.Drawing.Point(21, 41);
            this.dtpFechaInicial.Name = "dtpFechaInicial";
            this.dtpFechaInicial.Size = new System.Drawing.Size(200, 20);
            this.dtpFechaInicial.TabIndex = 3;
            // 
            // dtpFechaFinal
            // 
            this.dtpFechaFinal.Location = new System.Drawing.Point(21, 82);
            this.dtpFechaFinal.Name = "dtpFechaFinal";
            this.dtpFechaFinal.Size = new System.Drawing.Size(200, 20);
            this.dtpFechaFinal.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Del :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Al: ";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(13, 472);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(176, 30);
            this.progressBar1.TabIndex = 31;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = ((System.Drawing.Image)(resources.GetObject("botonGenerar.Imagen")));
            this.botonGenerar.Location = new System.Drawing.Point(194, 453);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 30;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_ControlClicked);
            this.botonGenerar.Load += new System.EventHandler(this.botonGenerar_Load);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.botonCancelar.Imagen = ((System.Drawing.Image)(resources.GetObject("botonCancelar.Imagen")));
            this.botonCancelar.Location = new System.Drawing.Point(287, 453);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 29;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_ControlClicked);
            // 
            // rbEsPDF
            // 
            this.rbEsPDF.AutoSize = true;
            this.rbEsPDF.Checked = true;
            this.rbEsPDF.Location = new System.Drawing.Point(13, 450);
            this.rbEsPDF.Name = "rbEsPDF";
            this.rbEsPDF.Size = new System.Drawing.Size(46, 17);
            this.rbEsPDF.TabIndex = 32;
            this.rbEsPDF.TabStop = true;
            this.rbEsPDF.Text = "PDF";
            this.rbEsPDF.UseVisualStyleBackColor = true;
            // 
            // rbExcel
            // 
            this.rbExcel.AutoSize = true;
            this.rbExcel.Location = new System.Drawing.Point(72, 449);
            this.rbExcel.Name = "rbExcel";
            this.rbExcel.Size = new System.Drawing.Size(51, 17);
            this.rbExcel.TabIndex = 33;
            this.rbExcel.TabStop = true;
            this.rbExcel.Text = "Excel";
            this.rbExcel.UseVisualStyleBackColor = true;
            // 
            // chkAbrir
            // 
            this.chkAbrir.AutoSize = true;
            this.chkAbrir.Checked = true;
            this.chkAbrir.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAbrir.Location = new System.Drawing.Point(141, 450);
            this.chkAbrir.Name = "chkAbrir";
            this.chkAbrir.Size = new System.Drawing.Size(47, 17);
            this.chkAbrir.TabIndex = 34;
            this.chkAbrir.Text = "Abrir";
            this.chkAbrir.UseVisualStyleBackColor = true;
            // 
            // Frm_RecibosCobradoPorFolio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(383, 538);
            this.Controls.Add(this.chkAbrir);
            this.Controls.Add(this.rbExcel);
            this.Controls.Add(this.rbEsPDF);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.chkIncluirCargosPeriodicos);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cmbConjunto);
            this.Controls.Add(this.conjunto);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbInmobiliaria);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(399, 576);
            this.MaximizeBox = false;
            this.Name = "Frm_RecibosCobradoPorFolio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Recibos de renta cobrados por Folio";
            this.Load += new System.EventHandler(this.Frm_RecibosCobradoPorFolio_Load);
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

        private System.Windows.Forms.ComboBox cmbInmobiliaria;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rBallOrder;
        private System.Windows.Forms.RadioButton rBSelectConjunto;
        private System.Windows.Forms.RadioButton rBallConjuntos;
        private System.Windows.Forms.Label conjunto;
        private System.Windows.Forms.ComboBox cmbConjunto;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rBOrderRec;
        private System.Windows.Forms.RadioButton rBOrderFolio;
        private System.Windows.Forms.CheckBox chkIncluirCargosPeriodicos;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DateTimePicker dtpFechaInicial;
        private System.Windows.Forms.DateTimePicker dtpFechaFinal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private Controls.Ctrl_Opcion botonGenerar;
        private Controls.Ctrl_Opcion botonCancelar;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.RadioButton rbEsPDF;
        private System.Windows.Forms.RadioButton rbExcel;
        private System.Windows.Forms.CheckBox chkAbrir;
    }
}