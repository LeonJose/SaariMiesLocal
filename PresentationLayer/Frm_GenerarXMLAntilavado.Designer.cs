namespace GestorReportes.PresentationLayer
{
    partial class Frm_GenerarXMLAntilavado
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_GenerarXMLAntilavado));
            this.grpBxPrincipal = new System.Windows.Forms.GroupBox();
            this.comboBoxClienteGen = new System.Windows.Forms.ComboBox();
            this.labelCteGen = new System.Windows.Forms.Label();
            this.checkBoxExcluirCteGen = new System.Windows.Forms.CheckBox();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.chckBxOctubre = new System.Windows.Forms.CheckBox();
            this.cmbBxFechas = new System.Windows.Forms.ComboBox();
            this.lblFecha = new System.Windows.Forms.Label();
            this.cmbBxInm = new System.Windows.Forms.ComboBox();
            this.lblInmobiliaria = new System.Windows.Forms.Label();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.botonAceptar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.grpBxPrincipal.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpBxPrincipal
            // 
            this.grpBxPrincipal.Controls.Add(this.comboBoxClienteGen);
            this.grpBxPrincipal.Controls.Add(this.labelCteGen);
            this.grpBxPrincipal.Controls.Add(this.checkBoxExcluirCteGen);
            this.grpBxPrincipal.Controls.Add(this.progreso);
            this.grpBxPrincipal.Controls.Add(this.botonAceptar);
            this.grpBxPrincipal.Controls.Add(this.botonCancelar);
            this.grpBxPrincipal.Controls.Add(this.chckBxOctubre);
            this.grpBxPrincipal.Controls.Add(this.cmbBxFechas);
            this.grpBxPrincipal.Controls.Add(this.lblFecha);
            this.grpBxPrincipal.Controls.Add(this.cmbBxInm);
            this.grpBxPrincipal.Controls.Add(this.lblInmobiliaria);
            this.grpBxPrincipal.Location = new System.Drawing.Point(12, 12);
            this.grpBxPrincipal.Name = "grpBxPrincipal";
            this.grpBxPrincipal.Size = new System.Drawing.Size(470, 278);
            this.grpBxPrincipal.TabIndex = 0;
            this.grpBxPrincipal.TabStop = false;
            this.grpBxPrincipal.Text = "Seleccione un contribuyente y a continuación una fecha, para generar el archivo ." +
    "xml correspondiente al mes.";
            // 
            // comboBoxClienteGen
            // 
            this.comboBoxClienteGen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxClienteGen.FormattingEnabled = true;
            this.comboBoxClienteGen.Location = new System.Drawing.Point(104, 157);
            this.comboBoxClienteGen.Name = "comboBoxClienteGen";
            this.comboBoxClienteGen.Size = new System.Drawing.Size(330, 21);
            this.comboBoxClienteGen.TabIndex = 16;
            this.comboBoxClienteGen.Visible = false;
            // 
            // labelCteGen
            // 
            this.labelCteGen.AutoSize = true;
            this.labelCteGen.Location = new System.Drawing.Point(23, 160);
            this.labelCteGen.Name = "labelCteGen";
            this.labelCteGen.Size = new System.Drawing.Size(42, 13);
            this.labelCteGen.TabIndex = 15;
            this.labelCteGen.Text = "Cliente:";
            this.labelCteGen.Visible = false;
            // 
            // checkBoxExcluirCteGen
            // 
            this.checkBoxExcluirCteGen.AutoSize = true;
            this.checkBoxExcluirCteGen.Location = new System.Drawing.Point(26, 134);
            this.checkBoxExcluirCteGen.Name = "checkBoxExcluirCteGen";
            this.checkBoxExcluirCteGen.Size = new System.Drawing.Size(255, 17);
            this.checkBoxExcluirCteGen.TabIndex = 14;
            this.checkBoxExcluirCteGen.Text = "Excluir cliente (público en general o rfc genérico)";
            this.checkBoxExcluirCteGen.UseVisualStyleBackColor = true;
            this.checkBoxExcluirCteGen.Visible = false;
            this.checkBoxExcluirCteGen.CheckedChanged += new System.EventHandler(this.checkBoxExcluirCteGen_CheckedChanged);
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(26, 240);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(260, 23);
            this.progreso.TabIndex = 13;
            // 
            // chckBxOctubre
            // 
            this.chckBxOctubre.AutoSize = true;
            this.chckBxOctubre.Location = new System.Drawing.Point(26, 104);
            this.chckBxOctubre.Name = "chckBxOctubre";
            this.chckBxOctubre.Size = new System.Drawing.Size(313, 17);
            this.chckBxOctubre.TabIndex = 10;
            this.chckBxOctubre.Text = "Revisar acumulado solo a partir del 1 de Septiembre de 2013";
            this.chckBxOctubre.UseVisualStyleBackColor = true;
            // 
            // cmbBxFechas
            // 
            this.cmbBxFechas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxFechas.FormattingEnabled = true;
            this.cmbBxFechas.Location = new System.Drawing.Point(104, 69);
            this.cmbBxFechas.Name = "cmbBxFechas";
            this.cmbBxFechas.Size = new System.Drawing.Size(227, 21);
            this.cmbBxFechas.TabIndex = 9;
            // 
            // lblFecha
            // 
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(23, 72);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(40, 13);
            this.lblFecha.TabIndex = 5;
            this.lblFecha.Text = "Fecha:";
            // 
            // cmbBxInm
            // 
            this.cmbBxInm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxInm.FormattingEnabled = true;
            this.cmbBxInm.Location = new System.Drawing.Point(104, 42);
            this.cmbBxInm.Name = "cmbBxInm";
            this.cmbBxInm.Size = new System.Drawing.Size(330, 21);
            this.cmbBxInm.TabIndex = 4;
            this.cmbBxInm.SelectedValueChanged += new System.EventHandler(this.cmbBxInm_SelectedValueChanged);
            // 
            // lblInmobiliaria
            // 
            this.lblInmobiliaria.AutoSize = true;
            this.lblInmobiliaria.Location = new System.Drawing.Point(23, 45);
            this.lblInmobiliaria.Name = "lblInmobiliaria";
            this.lblInmobiliaria.Size = new System.Drawing.Size(75, 13);
            this.lblInmobiliaria.TabIndex = 3;
            this.lblInmobiliaria.Text = "Contribuyente:";
            // 
            // worker
            // 
            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            this.worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            // 
            // botonAceptar
            // 
            this.botonAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonAceptar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonAceptar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonAceptar.Imagen = ((System.Drawing.Image)(resources.GetObject("botonAceptar.Imagen")));
            this.botonAceptar.Location = new System.Drawing.Point(308, 197);
            this.botonAceptar.Name = "botonAceptar";
            this.botonAceptar.Size = new System.Drawing.Size(75, 75);
            this.botonAceptar.TabIndex = 12;
            this.botonAceptar.Texto = "Aceptar";
            this.botonAceptar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnAceptar_Click);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(389, 197);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 11;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnCancelar_Click);
            // 
            // Frm_GenerarXMLAntilavado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(494, 302);
            this.Controls.Add(this.grpBxPrincipal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 330);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 330);
            this.Name = "Frm_GenerarXMLAntilavado";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generar XML Antilavado";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_GenerarXMLAntilavado_FormClosing);
            this.Load += new System.EventHandler(this.Frm_GenerarXMLAntilavado_Load);
            this.grpBxPrincipal.ResumeLayout(false);
            this.grpBxPrincipal.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpBxPrincipal;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.ComboBox cmbBxInm;
        private System.Windows.Forms.Label lblInmobiliaria;
        private System.Windows.Forms.ComboBox cmbBxFechas;
        private System.Windows.Forms.CheckBox chckBxOctubre;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonAceptar;
        private System.Windows.Forms.ProgressBar progreso;
        private System.ComponentModel.BackgroundWorker worker;
        private System.Windows.Forms.CheckBox checkBoxExcluirCteGen;
        private System.Windows.Forms.ComboBox comboBoxClienteGen;
        private System.Windows.Forms.Label labelCteGen;
    }
}