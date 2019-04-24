namespace GestorReportes.PresentationLayer
{
    partial class Frm_RentRoll
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioPDF = new System.Windows.Forms.RadioButton();
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.comboConjunto = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboInmobiliaria = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDescr
            // 
            this.lblDescr.Location = new System.Drawing.Point(12, 9);
            this.lblDescr.Name = "lblDescr";
            this.lblDescr.Size = new System.Drawing.Size(460, 42);
            this.lblDescr.TabIndex = 2;
            this.lblDescr.Text = "Reporte de banco rent roll. Seleccione la inmobiliaria, posteriormente seleccione" +
    " el conjunto asociado, seleccione el rango de fechas en los que desea buscar. Fi" +
    "nalmente dé clic en Generar Reporte";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.dateTimePicker2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dateTimePicker1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboConjunto);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboInmobiliaria);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(459, 128);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selección de criterios";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioPDF);
            this.groupBox2.Controls.Add(this.radioExcel);
            this.groupBox2.Location = new System.Drawing.Point(326, 71);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(127, 46);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Formato:";
            // 
            // radioPDF
            // 
            this.radioPDF.AutoSize = true;
            this.radioPDF.Location = new System.Drawing.Point(75, 19);
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
            this.radioExcel.Checked = true;
            this.radioExcel.Location = new System.Drawing.Point(15, 19);
            this.radioExcel.Name = "radioExcel";
            this.radioExcel.Size = new System.Drawing.Size(51, 17);
            this.radioExcel.TabIndex = 0;
            this.radioExcel.TabStop = true;
            this.radioExcel.Text = "Excel";
            this.radioExcel.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(90, 97);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker2.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Fin:";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(90, 71);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Inicio:";
            // 
            // comboConjunto
            // 
            this.comboConjunto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboConjunto.DropDownWidth = 364;
            this.comboConjunto.FormattingEnabled = true;
            this.comboConjunto.Location = new System.Drawing.Point(90, 44);
            this.comboConjunto.Name = "comboConjunto";
            this.comboConjunto.Size = new System.Drawing.Size(363, 21);
            this.comboConjunto.TabIndex = 5;
            this.comboConjunto.SelectedIndexChanged += new System.EventHandler(this.comboConjunto_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Conjunto:";
            // 
            // comboInmobiliaria
            // 
            this.comboInmobiliaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInmobiliaria.DropDownWidth = 364;
            this.comboInmobiliaria.FormattingEnabled = true;
            this.comboInmobiliaria.Location = new System.Drawing.Point(90, 17);
            this.comboInmobiliaria.Name = "comboInmobiliaria";
            this.comboInmobiliaria.Size = new System.Drawing.Size(363, 21);
            this.comboInmobiliaria.TabIndex = 3;
            this.comboInmobiliaria.SelectedIndexChanged += new System.EventHandler(this.comboInmobiliaria_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Inmobiliaria:";
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(397, 189);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 11;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_ControlClicked);
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(316, 189);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 12;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnGenerarReporte_Click);
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(15, 200);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(288, 23);
            this.progreso.TabIndex = 13;
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // Frm_RentRoll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(484, 261);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblDescr);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 300);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "Frm_RentRoll";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rent roll";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_RentRoll_FormClosing);
            this.Load += new System.EventHandler(this.Frm_RentRoll_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblDescr;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboConjunto;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboInmobiliaria;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioPDF;
        private System.Windows.Forms.RadioButton radioExcel;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonGenerar;
        private System.Windows.Forms.ProgressBar progreso;
        private System.ComponentModel.BackgroundWorker workerReporte;
    }
}