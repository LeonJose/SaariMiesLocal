namespace GestorReportes.PresentationLayer
{
    partial class Frm_ReporteSaldoVenta
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
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.comboInmo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.checkIncluirPreventa = new System.Windows.Forms.CheckBox();
            this.checkAbrir = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDescr
            // 
            this.lblDescr.Location = new System.Drawing.Point(12, 9);
            this.lblDescr.Name = "lblDescr";
            this.lblDescr.Size = new System.Drawing.Size(460, 42);
            this.lblDescr.TabIndex = 1;
            this.lblDescr.Text = "Creación de reporte de saldos. Seleccione la inmobiliaria y la fecha a la cual se" +
    " desea obtener el reporte. \r\nPosteriormente dé clic en Generar Reporte";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dateTimePicker1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboInmo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(15, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(457, 80);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seleccione los criterios.";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(74, 50);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(377, 20);
            this.dateTimePicker1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Al: ";
            // 
            // comboInmo
            // 
            this.comboInmo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInmo.FormattingEnabled = true;
            this.comboInmo.Location = new System.Drawing.Point(74, 19);
            this.comboInmo.Name = "comboInmo";
            this.comboInmo.Size = new System.Drawing.Size(377, 21);
            this.comboInmo.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Inmobiliaria:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 181);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(273, 23);
            this.progressBar1.TabIndex = 5;
            // 
            // worker
            // 
            this.worker.WorkerReportsProgress = true;
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(310, 134);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 7;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnGenerarReporte_Click);
            this.botonGenerar.Load += new System.EventHandler(this.botonGenerar_Load);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(391, 134);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 6;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_ControlClicked);
            // 
            // checkIncluirPreventa
            // 
            this.checkIncluirPreventa.AutoSize = true;
            this.checkIncluirPreventa.Location = new System.Drawing.Point(24, 140);
            this.checkIncluirPreventa.Name = "checkIncluirPreventa";
            this.checkIncluirPreventa.Size = new System.Drawing.Size(100, 17);
            this.checkIncluirPreventa.TabIndex = 8;
            this.checkIncluirPreventa.Text = "Incluir Preventa";
            this.checkIncluirPreventa.UseVisualStyleBackColor = true;
            // 
            // checkAbrir
            // 
            this.checkAbrir.AutoSize = true;
            this.checkAbrir.Checked = true;
            this.checkAbrir.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAbrir.Location = new System.Drawing.Point(24, 158);
            this.checkAbrir.Name = "checkAbrir";
            this.checkAbrir.Size = new System.Drawing.Size(88, 17);
            this.checkAbrir.TabIndex = 9;
            this.checkAbrir.Text = "Abrir Reporte";
            this.checkAbrir.UseVisualStyleBackColor = true;
            // 
            // Frm_ReporteSaldoVenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(484, 211);
            this.Controls.Add(this.checkAbrir);
            this.Controls.Add(this.checkIncluirPreventa);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblDescr);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 250);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 250);
            this.Name = "Frm_ReporteSaldoVenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de saldos de clientes de venta";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_ReporteSaldoVenta_FormClosing);
            this.Load += new System.EventHandler(this.Frm_ReporteSaldoVenta_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDescr;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboInmo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker worker;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonGenerar;
        private System.Windows.Forms.CheckBox checkIncluirPreventa;
        private System.Windows.Forms.CheckBox checkAbrir;
    }
}