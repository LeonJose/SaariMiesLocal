namespace GestorReportes.PresentationLayer
{
    partial class Frm_FichaInformativa
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_FichaInformativa));
            this.lblInstrucc = new System.Windows.Forms.Label();
            this.grpBxCriterios = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboInmueble = new System.Windows.Forms.ComboBox();
            this.cmbBxConjunto = new System.Windows.Forms.ComboBox();
            this.lblConj = new System.Windows.Forms.Label();
            this.cmbBxInm = new System.Windows.Forms.ComboBox();
            this.lblInmobiliaria = new System.Windows.Forms.Label();
            this.cmbBxGpoEmpr = new System.Windows.Forms.ComboBox();
            this.lblGpoEmpr = new System.Windows.Forms.Label();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonSiguiente = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.grpBxCriterios.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInstrucc
            // 
            this.lblInstrucc.AutoSize = true;
            this.lblInstrucc.Location = new System.Drawing.Point(12, 9);
            this.lblInstrucc.Name = "lblInstrucc";
            this.lblInstrucc.Size = new System.Drawing.Size(542, 13);
            this.lblInstrucc.TabIndex = 0;
            this.lblInstrucc.Text = "Creación de una ficha informativa. Seleccione las opciones adecuadas para generar" +
                " una(s) ficha(s) informativa(s).";
            // 
            // grpBxCriterios
            // 
            this.grpBxCriterios.Controls.Add(this.label1);
            this.grpBxCriterios.Controls.Add(this.comboInmueble);
            this.grpBxCriterios.Controls.Add(this.cmbBxConjunto);
            this.grpBxCriterios.Controls.Add(this.lblConj);
            this.grpBxCriterios.Controls.Add(this.cmbBxInm);
            this.grpBxCriterios.Controls.Add(this.lblInmobiliaria);
            this.grpBxCriterios.Controls.Add(this.cmbBxGpoEmpr);
            this.grpBxCriterios.Controls.Add(this.lblGpoEmpr);
            this.grpBxCriterios.Location = new System.Drawing.Point(15, 39);
            this.grpBxCriterios.Name = "grpBxCriterios";
            this.grpBxCriterios.Size = new System.Drawing.Size(642, 132);
            this.grpBxCriterios.TabIndex = 1;
            this.grpBxCriterios.TabStop = false;
            this.grpBxCriterios.Text = "Selección de criterios";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Inmueble:";
            // 
            // comboInmueble
            // 
            this.comboInmueble.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInmueble.FormattingEnabled = true;
            this.comboInmueble.Location = new System.Drawing.Point(107, 94);
            this.comboInmueble.Name = "comboInmueble";
            this.comboInmueble.Size = new System.Drawing.Size(529, 21);
            this.comboInmueble.TabIndex = 5;
            // 
            // cmbBxConjunto
            // 
            this.cmbBxConjunto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxConjunto.FormattingEnabled = true;
            this.cmbBxConjunto.Location = new System.Drawing.Point(107, 67);
            this.cmbBxConjunto.Name = "cmbBxConjunto";
            this.cmbBxConjunto.Size = new System.Drawing.Size(529, 21);
            this.cmbBxConjunto.TabIndex = 3;
            this.cmbBxConjunto.SelectedIndexChanged += new System.EventHandler(this.cmbBxConjunto_SelectedIndexChanged);
            // 
            // lblConj
            // 
            this.lblConj.AutoSize = true;
            this.lblConj.Location = new System.Drawing.Point(6, 70);
            this.lblConj.Name = "lblConj";
            this.lblConj.Size = new System.Drawing.Size(52, 13);
            this.lblConj.TabIndex = 4;
            this.lblConj.Text = "Conjunto:";
            // 
            // cmbBxInm
            // 
            this.cmbBxInm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxInm.FormattingEnabled = true;
            this.cmbBxInm.Location = new System.Drawing.Point(107, 40);
            this.cmbBxInm.Name = "cmbBxInm";
            this.cmbBxInm.Size = new System.Drawing.Size(529, 21);
            this.cmbBxInm.TabIndex = 2;
            this.cmbBxInm.SelectedIndexChanged += new System.EventHandler(this.cmbBxInm_SelectedIndexChanged);
            // 
            // lblInmobiliaria
            // 
            this.lblInmobiliaria.AutoSize = true;
            this.lblInmobiliaria.Location = new System.Drawing.Point(6, 43);
            this.lblInmobiliaria.Name = "lblInmobiliaria";
            this.lblInmobiliaria.Size = new System.Drawing.Size(62, 13);
            this.lblInmobiliaria.TabIndex = 2;
            this.lblInmobiliaria.Text = "Inmobiliaria:";
            // 
            // cmbBxGpoEmpr
            // 
            this.cmbBxGpoEmpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxGpoEmpr.FormattingEnabled = true;
            this.cmbBxGpoEmpr.Location = new System.Drawing.Point(107, 13);
            this.cmbBxGpoEmpr.Name = "cmbBxGpoEmpr";
            this.cmbBxGpoEmpr.Size = new System.Drawing.Size(529, 21);
            this.cmbBxGpoEmpr.TabIndex = 1;
            this.cmbBxGpoEmpr.SelectedIndexChanged += new System.EventHandler(this.cmbBxGpoEmpr_SelectedIndexChanged);
            // 
            // lblGpoEmpr
            // 
            this.lblGpoEmpr.AutoSize = true;
            this.lblGpoEmpr.Location = new System.Drawing.Point(6, 16);
            this.lblGpoEmpr.Name = "lblGpoEmpr";
            this.lblGpoEmpr.Size = new System.Drawing.Size(95, 13);
            this.lblGpoEmpr.TabIndex = 0;
            this.lblGpoEmpr.Text = "Grupo empresarial:";
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(576, 177);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 6;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnCancelar_Click);
            // 
            // botonSiguiente
            // 
            this.botonSiguiente.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonSiguiente.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonSiguiente.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonSiguiente.Imagen = ((System.Drawing.Image)(resources.GetObject("botonSiguiente.Imagen")));
            this.botonSiguiente.Location = new System.Drawing.Point(495, 177);
            this.botonSiguiente.Name = "botonSiguiente";
            this.botonSiguiente.Size = new System.Drawing.Size(75, 75);
            this.botonSiguiente.TabIndex = 7;
            this.botonSiguiente.Texto = "Siguiente";
            this.botonSiguiente.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnNext_Click);
            // 
            // Frm_FichaInformativa
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(669, 272);
            this.Controls.Add(this.botonSiguiente);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.grpBxCriterios);
            this.Controls.Add(this.lblInstrucc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(675, 300);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(675, 300);
            this.Name = "Frm_FichaInformativa";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ficha Informativa";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_FichaInformativa_FormClosing);
            this.Load += new System.EventHandler(this.Frm_FichaInformativa_Load);
            this.grpBxCriterios.ResumeLayout(false);
            this.grpBxCriterios.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInstrucc;
        private System.Windows.Forms.GroupBox grpBxCriterios;
        private System.Windows.Forms.ComboBox cmbBxConjunto;
        private System.Windows.Forms.Label lblConj;
        private System.Windows.Forms.ComboBox cmbBxInm;
        private System.Windows.Forms.Label lblInmobiliaria;
        private System.Windows.Forms.ComboBox cmbBxGpoEmpr;
        private System.Windows.Forms.Label lblGpoEmpr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboInmueble;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonSiguiente;
    }
}