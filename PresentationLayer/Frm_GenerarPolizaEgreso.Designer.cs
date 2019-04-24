namespace GestorReportes.PresentationLayer
{
    partial class Frm_GenerarPolizaEgreso
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
            this.checkAfectar = new System.Windows.Forms.CheckBox();
            this.checkMultiples = new System.Windows.Forms.CheckBox();
            this.textoMascara = new System.Windows.Forms.TextBox();
            this.checkMascara = new System.Windows.Forms.CheckBox();
            this.txtBxConceptoPoliza = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBxIdenPol = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioProvision = new System.Windows.Forms.RadioButton();
            this.radioEgreso = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioContpaqXls = new System.Windows.Forms.RadioButton();
            this.rdBtnContpaq = new System.Windows.Forms.RadioButton();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.comboInmobiliaria = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblInstrucciones = new System.Windows.Forms.Label();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.checkSegmento = new System.Windows.Forms.CheckBox();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkAfectar
            // 
            this.checkAfectar.AutoSize = true;
            this.checkAfectar.Location = new System.Drawing.Point(10, 152);
            this.checkAfectar.Name = "checkAfectar";
            this.checkAfectar.Size = new System.Drawing.Size(96, 17);
            this.checkAfectar.TabIndex = 4;
            this.checkAfectar.Text = "Afectar saldos ";
            this.checkAfectar.UseVisualStyleBackColor = true;
            // 
            // checkMultiples
            // 
            this.checkMultiples.AutoSize = true;
            this.checkMultiples.Location = new System.Drawing.Point(10, 129);
            this.checkMultiples.Name = "checkMultiples";
            this.checkMultiples.Size = new System.Drawing.Size(134, 17);
            this.checkMultiples.TabIndex = 3;
            this.checkMultiples.Text = "Multiples encabezados";
            this.checkMultiples.UseVisualStyleBackColor = true;
            // 
            // textoMascara
            // 
            this.textoMascara.Enabled = false;
            this.textoMascara.Location = new System.Drawing.Point(295, 173);
            this.textoMascara.Name = "textoMascara";
            this.textoMascara.Size = new System.Drawing.Size(76, 20);
            this.textoMascara.TabIndex = 6;
            this.textoMascara.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textoMascara_KeyPress);
            // 
            // checkMascara
            // 
            this.checkMascara.AutoSize = true;
            this.checkMascara.Location = new System.Drawing.Point(10, 175);
            this.checkMascara.Name = "checkMascara";
            this.checkMascara.Size = new System.Drawing.Size(279, 17);
            this.checkMascara.TabIndex = 5;
            this.checkMascara.Text = "Enmascarar números de cuenta de la siguiente forma:";
            this.checkMascara.UseVisualStyleBackColor = true;
            this.checkMascara.CheckedChanged += new System.EventHandler(this.checkMascara_CheckedChanged);
            // 
            // txtBxConceptoPoliza
            // 
            this.txtBxConceptoPoliza.Location = new System.Drawing.Point(105, 77);
            this.txtBxConceptoPoliza.Name = "txtBxConceptoPoliza";
            this.txtBxConceptoPoliza.Size = new System.Drawing.Size(194, 20);
            this.txtBxConceptoPoliza.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 48;
            this.label3.Text = "Concepto:";
            // 
            // txtBxIdenPol
            // 
            this.txtBxIdenPol.Location = new System.Drawing.Point(105, 103);
            this.txtBxIdenPol.Name = "txtBxIdenPol";
            this.txtBxIdenPol.Size = new System.Drawing.Size(194, 20);
            this.txtBxIdenPol.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 46;
            this.label2.Text = "Número de póliza:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioProvision);
            this.groupBox4.Controls.Add(this.radioEgreso);
            this.groupBox4.Location = new System.Drawing.Point(377, 120);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 49);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Tipo de póliza:";
            // 
            // radioProvision
            // 
            this.radioProvision.AutoSize = true;
            this.radioProvision.Location = new System.Drawing.Point(95, 19);
            this.radioProvision.Name = "radioProvision";
            this.radioProvision.Size = new System.Drawing.Size(68, 17);
            this.radioProvision.TabIndex = 1;
            this.radioProvision.TabStop = true;
            this.radioProvision.Text = "Provisión";
            this.radioProvision.UseVisualStyleBackColor = true;
            // 
            // radioEgreso
            // 
            this.radioEgreso.AutoSize = true;
            this.radioEgreso.Checked = true;
            this.radioEgreso.Location = new System.Drawing.Point(6, 19);
            this.radioEgreso.Name = "radioEgreso";
            this.radioEgreso.Size = new System.Drawing.Size(58, 17);
            this.radioEgreso.TabIndex = 0;
            this.radioEgreso.TabStop = true;
            this.radioEgreso.Text = "Egreso";
            this.radioEgreso.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioContpaqXls);
            this.groupBox1.Controls.Add(this.rdBtnContpaq);
            this.groupBox1.Location = new System.Drawing.Point(377, 177);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 42);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Exportar a:";
            // 
            // radioContpaqXls
            // 
            this.radioContpaqXls.AutoSize = true;
            this.radioContpaqXls.Location = new System.Drawing.Point(95, 19);
            this.radioContpaqXls.Name = "radioContpaqXls";
            this.radioContpaqXls.Size = new System.Drawing.Size(83, 17);
            this.radioContpaqXls.TabIndex = 1;
            this.radioContpaqXls.Text = "Contpaq .xls";
            this.radioContpaqXls.UseVisualStyleBackColor = true;
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
            // dateTimePicker2
            // 
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker2.Location = new System.Drawing.Point(106, 13);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(88, 20);
            this.dateTimePicker2.TabIndex = 7;
            // 
            // comboInmobiliaria
            // 
            this.comboInmobiliaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInmobiliaria.DropDownWidth = 364;
            this.comboInmobiliaria.FormattingEnabled = true;
            this.comboInmobiliaria.Location = new System.Drawing.Point(105, 47);
            this.comboInmobiliaria.Name = "comboInmobiliaria";
            this.comboInmobiliaria.Size = new System.Drawing.Size(472, 21);
            this.comboInmobiliaria.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "Inmobiliaria:";
            // 
            // lblInstrucciones
            // 
            this.lblInstrucciones.Location = new System.Drawing.Point(7, 9);
            this.lblInstrucciones.Name = "lblInstrucciones";
            this.lblInstrucciones.Size = new System.Drawing.Size(570, 28);
            this.lblInstrucciones.TabIndex = 33;
            this.lblInstrucciones.Text = "Elija la inmobiliaria y las demas condiciones para las cuales desea generar la pó" +
    "liza de egreso. Para generar haga clic en Generar.";
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(421, 225);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 10;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_Click);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(502, 225);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 11;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnCancelar_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dateTimePicker1);
            this.groupBox2.Controls.Add(this.dateTimePicker2);
            this.groupBox2.Location = new System.Drawing.Point(377, 74);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 40);
            this.groupBox2.TabIndex = 49;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rango Fechas";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(6, 13);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(94, 20);
            this.dateTimePicker1.TabIndex = 50;
            // 
            // checkSegmento
            // 
            this.checkSegmento.AutoSize = true;
            this.checkSegmento.Location = new System.Drawing.Point(10, 198);
            this.checkSegmento.Name = "checkSegmento";
            this.checkSegmento.Size = new System.Drawing.Size(159, 17);
            this.checkSegmento.TabIndex = 51;
            this.checkSegmento.Text = "Incluir segmento de negocio";
            this.checkSegmento.UseVisualStyleBackColor = true;
            // 
            // Frm_GenerarPolizaEgreso
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(584, 301);
            this.Controls.Add(this.checkSegmento);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.checkAfectar);
            this.Controls.Add(this.checkMultiples);
            this.Controls.Add(this.textoMascara);
            this.Controls.Add(this.checkMascara);
            this.Controls.Add(this.txtBxConceptoPoliza);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtBxIdenPol);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboInmobiliaria);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblInstrucciones);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximumSize = new System.Drawing.Size(600, 340);
            this.MinimumSize = new System.Drawing.Size(600, 340);
            this.Name = "Frm_GenerarPolizaEgreso";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generación de póliza de egreso";
            this.Load += new System.EventHandler(this.Frm_GenerarPolizaEgreso_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkAfectar;
        private System.Windows.Forms.CheckBox checkMultiples;
        private System.Windows.Forms.TextBox textoMascara;
        private System.Windows.Forms.CheckBox checkMascara;
        private System.Windows.Forms.TextBox txtBxConceptoPoliza;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBxIdenPol;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioProvision;
        private System.Windows.Forms.RadioButton radioEgreso;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioContpaqXls;
        private System.Windows.Forms.RadioButton rdBtnContpaq;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.ComboBox comboInmobiliaria;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblInstrucciones;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonGenerar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.CheckBox checkSegmento;
    }
}