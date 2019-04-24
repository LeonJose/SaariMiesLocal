using GestorReportes.PresentationLayer.Controls;
namespace GestorReportes.PresentationLayer
{
    partial class Frm_GenerarPolizaCopropiedad
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
            this.groupBoxCoProp = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioContpaqXls = new System.Windows.Forms.RadioButton();
            this.radioAxapta = new System.Windows.Forms.RadioButton();
            this.rdBtnContavision = new System.Windows.Forms.RadioButton();
            this.rdBtnAspel = new System.Windows.Forms.RadioButton();
            this.rdBtnExc = new System.Windows.Forms.RadioButton();
            this.rdBtnContpaq = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdBtnNotasCredito = new System.Windows.Forms.RadioButton();
            this.rdBtnCancelados = new System.Windows.Forms.RadioButton();
            this.rdBtnIngreso = new System.Windows.Forms.RadioButton();
            this.rdBtnDiario = new System.Windows.Forms.RadioButton();
            this.textBoxInmo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.lblFechaFin = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.lblFechaInicio = new System.Windows.Forms.Label();
            this.buttonGenerar = new System.Windows.Forms.Button();
            this.groupBoxCoProp.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxCoProp
            // 
            this.groupBoxCoProp.Controls.Add(this.buttonGenerar);
            this.groupBoxCoProp.Controls.Add(this.groupBox1);
            this.groupBoxCoProp.Controls.Add(this.groupBox4);
            this.groupBoxCoProp.Controls.Add(this.textBoxInmo);
            this.groupBoxCoProp.Controls.Add(this.label1);
            this.groupBoxCoProp.Controls.Add(this.dateTimePicker2);
            this.groupBoxCoProp.Controls.Add(this.lblFechaFin);
            this.groupBoxCoProp.Controls.Add(this.dateTimePicker1);
            this.groupBoxCoProp.Controls.Add(this.lblFechaInicio);
            this.groupBoxCoProp.Location = new System.Drawing.Point(12, 12);
            this.groupBoxCoProp.Name = "groupBoxCoProp";
            this.groupBoxCoProp.Size = new System.Drawing.Size(625, 295);
            this.groupBoxCoProp.TabIndex = 0;
            this.groupBoxCoProp.TabStop = false;
            this.groupBoxCoProp.Text = "Copropiedad";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioContpaqXls);
            this.groupBox1.Controls.Add(this.radioAxapta);
            this.groupBox1.Controls.Add(this.rdBtnContavision);
            this.groupBox1.Controls.Add(this.rdBtnAspel);
            this.groupBox1.Controls.Add(this.rdBtnExc);
            this.groupBox1.Controls.Add(this.rdBtnContpaq);
            this.groupBox1.Location = new System.Drawing.Point(10, 104);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(169, 106);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Exportar a:";
            // 
            // radioContpaqXls
            // 
            this.radioContpaqXls.AutoSize = true;
            this.radioContpaqXls.Location = new System.Drawing.Point(6, 80);
            this.radioContpaqXls.Name = "radioContpaqXls";
            this.radioContpaqXls.Size = new System.Drawing.Size(83, 17);
            this.radioContpaqXls.TabIndex = 5;
            this.radioContpaqXls.Text = "Contpaq .xls";
            this.radioContpaqXls.UseVisualStyleBackColor = true;
            // 
            // radioAxapta
            // 
            this.radioAxapta.AutoSize = true;
            this.radioAxapta.Location = new System.Drawing.Point(95, 51);
            this.radioAxapta.Name = "radioAxapta";
            this.radioAxapta.Size = new System.Drawing.Size(58, 17);
            this.radioAxapta.TabIndex = 4;
            this.radioAxapta.Text = "Axapta";
            this.radioAxapta.UseVisualStyleBackColor = true;
            // 
            // rdBtnContavision
            // 
            this.rdBtnContavision.AutoSize = true;
            this.rdBtnContavision.Location = new System.Drawing.Point(6, 51);
            this.rdBtnContavision.Name = "rdBtnContavision";
            this.rdBtnContavision.Size = new System.Drawing.Size(80, 17);
            this.rdBtnContavision.TabIndex = 2;
            this.rdBtnContavision.Text = "Contavision";
            this.rdBtnContavision.UseVisualStyleBackColor = true;
            // 
            // rdBtnAspel
            // 
            this.rdBtnAspel.AutoSize = true;
            this.rdBtnAspel.Location = new System.Drawing.Point(94, 19);
            this.rdBtnAspel.Name = "rdBtnAspel";
            this.rdBtnAspel.Size = new System.Drawing.Size(72, 17);
            this.rdBtnAspel.TabIndex = 1;
            this.rdBtnAspel.Text = "Aspel-COI";
            this.rdBtnAspel.UseVisualStyleBackColor = true;
            // 
            // rdBtnExc
            // 
            this.rdBtnExc.AutoSize = true;
            this.rdBtnExc.Location = new System.Drawing.Point(94, 80);
            this.rdBtnExc.Name = "rdBtnExc";
            this.rdBtnExc.Size = new System.Drawing.Size(51, 17);
            this.rdBtnExc.TabIndex = 3;
            this.rdBtnExc.Text = "Excel";
            this.rdBtnExc.UseVisualStyleBackColor = true;
            // 
            // rdBtnContpaq
            // 
            this.rdBtnContpaq.AutoSize = true;
            this.rdBtnContpaq.Checked = true;
            this.rdBtnContpaq.Location = new System.Drawing.Point(6, 19);
            this.rdBtnContpaq.Name = "rdBtnContpaq";
            this.rdBtnContpaq.Size = new System.Drawing.Size(82, 17);
            this.rdBtnContpaq.TabIndex = 0;
            this.rdBtnContpaq.TabStop = true;
            this.rdBtnContpaq.Text = "Contpaq .txt";
            this.rdBtnContpaq.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdBtnNotasCredito);
            this.groupBox4.Controls.Add(this.rdBtnCancelados);
            this.groupBox4.Controls.Add(this.rdBtnIngreso);
            this.groupBox4.Controls.Add(this.rdBtnDiario);
            this.groupBox4.Location = new System.Drawing.Point(185, 104);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(166, 106);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Tipo de póliza:";
            // 
            // rdBtnNotasCredito
            // 
            this.rdBtnNotasCredito.AutoSize = true;
            this.rdBtnNotasCredito.Location = new System.Drawing.Point(6, 80);
            this.rdBtnNotasCredito.Name = "rdBtnNotasCredito";
            this.rdBtnNotasCredito.Size = new System.Drawing.Size(103, 17);
            this.rdBtnNotasCredito.TabIndex = 3;
            this.rdBtnNotasCredito.TabStop = true;
            this.rdBtnNotasCredito.Text = "Notas de crédito";
            this.rdBtnNotasCredito.UseVisualStyleBackColor = true;
            // 
            // rdBtnCancelados
            // 
            this.rdBtnCancelados.AutoSize = true;
            this.rdBtnCancelados.Location = new System.Drawing.Point(6, 51);
            this.rdBtnCancelados.Name = "rdBtnCancelados";
            this.rdBtnCancelados.Size = new System.Drawing.Size(122, 17);
            this.rdBtnCancelados.TabIndex = 2;
            this.rdBtnCancelados.TabStop = true;
            this.rdBtnCancelados.Text = "Recibos cancelados";
            this.rdBtnCancelados.UseVisualStyleBackColor = true;
            // 
            // rdBtnIngreso
            // 
            this.rdBtnIngreso.AutoSize = true;
            this.rdBtnIngreso.Location = new System.Drawing.Point(100, 19);
            this.rdBtnIngreso.Name = "rdBtnIngreso";
            this.rdBtnIngreso.Size = new System.Drawing.Size(60, 17);
            this.rdBtnIngreso.TabIndex = 1;
            this.rdBtnIngreso.TabStop = true;
            this.rdBtnIngreso.Text = "Ingreso";
            this.rdBtnIngreso.UseVisualStyleBackColor = true;
            // 
            // rdBtnDiario
            // 
            this.rdBtnDiario.AutoSize = true;
            this.rdBtnDiario.Checked = true;
            this.rdBtnDiario.Location = new System.Drawing.Point(6, 19);
            this.rdBtnDiario.Name = "rdBtnDiario";
            this.rdBtnDiario.Size = new System.Drawing.Size(52, 17);
            this.rdBtnDiario.TabIndex = 0;
            this.rdBtnDiario.TabStop = true;
            this.rdBtnDiario.Text = "Diario";
            this.rdBtnDiario.UseVisualStyleBackColor = true;
            // 
            // textBoxInmo
            // 
            this.textBoxInmo.Location = new System.Drawing.Point(75, 29);
            this.textBoxInmo.Name = "textBoxInmo";
            this.textBoxInmo.Size = new System.Drawing.Size(200, 20);
            this.textBoxInmo.TabIndex = 18;
            this.textBoxInmo.Text = "ARR17";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Inmobiliaria:";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(377, 60);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker2.TabIndex = 16;
            // 
            // lblFechaFin
            // 
            this.lblFechaFin.AutoSize = true;
            this.lblFechaFin.Location = new System.Drawing.Point(317, 66);
            this.lblFechaFin.Name = "lblFechaFin";
            this.lblFechaFin.Size = new System.Drawing.Size(54, 13);
            this.lblFechaFin.TabIndex = 15;
            this.lblFechaFin.Text = "Fecha fin:";
            this.lblFechaFin.Visible = false;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(75, 60);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 14;
            // 
            // lblFechaInicio
            // 
            this.lblFechaInicio.AutoSize = true;
            this.lblFechaInicio.Location = new System.Drawing.Point(7, 66);
            this.lblFechaInicio.Name = "lblFechaInicio";
            this.lblFechaInicio.Size = new System.Drawing.Size(40, 13);
            this.lblFechaInicio.TabIndex = 13;
            this.lblFechaInicio.Text = "Fecha:";
            // 
            // buttonGenerar
            // 
            this.buttonGenerar.Location = new System.Drawing.Point(386, 155);
            this.buttonGenerar.Name = "buttonGenerar";
            this.buttonGenerar.Size = new System.Drawing.Size(75, 52);
            this.buttonGenerar.TabIndex = 21;
            this.buttonGenerar.Text = "Generar";
            this.buttonGenerar.UseVisualStyleBackColor = true;
            this.buttonGenerar.Click += new System.EventHandler(this.buttonGenerar_Click);
            // 
            // Frm_GenerarPolizaCopropiedad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(649, 333);
            this.Controls.Add(this.groupBoxCoProp);
            this.Name = "Frm_GenerarPolizaCopropiedad";
            this.Text = "Polizas de Copropiedad";
            this.Load += new System.EventHandler(this.Form_GenerarPolizaCopropiedad_Load);
            this.groupBoxCoProp.ResumeLayout(false);
            this.groupBoxCoProp.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxCoProp;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label lblFechaFin;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label lblFechaInicio;
        private System.Windows.Forms.TextBox textBoxInmo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdBtnNotasCredito;
        private System.Windows.Forms.RadioButton rdBtnCancelados;
        private System.Windows.Forms.RadioButton rdBtnIngreso;
        private System.Windows.Forms.RadioButton rdBtnDiario;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioContpaqXls;
        private System.Windows.Forms.RadioButton radioAxapta;
        private System.Windows.Forms.RadioButton rdBtnContavision;
        private System.Windows.Forms.RadioButton rdBtnAspel;
        private System.Windows.Forms.RadioButton rdBtnExc;
        private System.Windows.Forms.RadioButton rdBtnContpaq;
        private System.Windows.Forms.Button buttonGenerar;
    }
}