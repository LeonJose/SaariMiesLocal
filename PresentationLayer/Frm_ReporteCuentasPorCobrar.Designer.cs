namespace GestorReportes.PresentationLayer
{
    partial class Frm_ReporteCuentasPorCobrar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ReporteCuentasPorCobrar));
            this.label1 = new System.Windows.Forms.Label();
            this.CbxInmobiliarias = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CbxConjuntos = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Worker = new System.ComponentModel.BackgroundWorker();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioPDF = new System.Windows.Forms.RadioButton();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Empresa";
            // 
            // CbxInmobiliarias
            // 
            this.CbxInmobiliarias.BackColor = System.Drawing.SystemColors.Control;
            this.CbxInmobiliarias.FormattingEnabled = true;
            this.CbxInmobiliarias.Location = new System.Drawing.Point(25, 54);
            this.CbxInmobiliarias.Name = "CbxInmobiliarias";
            this.CbxInmobiliarias.Size = new System.Drawing.Size(417, 21);
            this.CbxInmobiliarias.TabIndex = 2;
            this.CbxInmobiliarias.SelectedIndexChanged += new System.EventHandler(this.CbxInmobiliarias_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Conjunto";
            // 
            // CbxConjuntos
            // 
            this.CbxConjuntos.BackColor = System.Drawing.SystemColors.Control;
            this.CbxConjuntos.FormattingEnabled = true;
            this.CbxConjuntos.Location = new System.Drawing.Point(12, 106);
            this.CbxConjuntos.Name = "CbxConjuntos";
            this.CbxConjuntos.Size = new System.Drawing.Size(417, 21);
            this.CbxConjuntos.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dateTimePicker1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 149);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 71);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Periodo del Reporte";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(25, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Fecha Corte :";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarMonthBackground = System.Drawing.SystemColors.ControlDark;
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(110, 31);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(175, 20);
            this.dateTimePicker1.TabIndex = 0;
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.SystemColors.Window;
            this.progressBar1.Location = new System.Drawing.Point(12, 323);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 27);
            this.progressBar1.TabIndex = 7;
            // 
            // Worker
            // 
            this.Worker.WorkerReportsProgress = true;
            this.Worker.WorkerSupportsCancellation = true;
            this.Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Worker_DoWork);
            this.Worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.Worker_ProgressChanged);
            this.Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.Worker_RunWorkerCompleted);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioPDF);
            this.groupBox2.Controls.Add(this.radioExcel);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 238);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(222, 65);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Formato para generar reporte ";
            // 
            // radioPDF
            // 
            this.radioPDF.AutoSize = true;
            this.radioPDF.Checked = true;
            this.radioPDF.Location = new System.Drawing.Point(28, 28);
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
            this.radioExcel.Location = new System.Drawing.Point(127, 28);
            this.radioExcel.Name = "radioExcel";
            this.radioExcel.Size = new System.Drawing.Size(51, 17);
            this.radioExcel.TabIndex = 8;
            this.radioExcel.TabStop = true;
            this.radioExcel.Text = "Excel";
            this.radioExcel.UseVisualStyleBackColor = true;
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.White;
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.White;
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(354, 275);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 18;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_ControlClicked);
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.White;
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.White;
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(273, 275);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 17;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_ControlClicked);
            this.botonGenerar.Load += new System.EventHandler(this.botonGenerar_Load);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.botonCancelar);
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Controls.Add(this.botonGenerar);
            this.groupBox3.Controls.Add(this.CbxConjuntos);
            this.groupBox3.Controls.Add(this.progressBar1);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Location = new System.Drawing.Point(13, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(439, 381);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Seleccione los parámetros para la generación del reporte";
            // 
            // Frm_ReporteCuentasPorCobrar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(464, 411);
            this.Controls.Add(this.CbxInmobiliarias);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_ReporteCuentasPorCobrar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de Cuentas Por Cobrar";
            this.Load += new System.EventHandler(this.Frm_ReporteCuentasPorCobrar_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox CbxInmobiliarias;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox CbxConjuntos;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker Worker;
        private Controls.Ctrl_Opcion botonGenerar;
        private Controls.Ctrl_Opcion botonCancelar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioPDF;
        private System.Windows.Forms.RadioButton radioExcel;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}