namespace GestorReportes.PresentationLayer
{
    partial class Frm_FichaInformativa2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_FichaInformativa2));
            this.lblInstrucc = new System.Windows.Forms.Label();
            this.gpBxDatos = new System.Windows.Forms.GroupBox();
            this.chckLstBxDatos = new System.Windows.Forms.CheckedListBox();
            this.chckBxSelectAll = new System.Windows.Forms.CheckBox();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonSiguiente = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonAtras = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.gpBxDatos.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInstrucc
            // 
            this.lblInstrucc.AutoSize = true;
            this.lblInstrucc.Location = new System.Drawing.Point(12, 9);
            this.lblInstrucc.Name = "lblInstrucc";
            this.lblInstrucc.Size = new System.Drawing.Size(344, 13);
            this.lblInstrucc.TabIndex = 0;
            this.lblInstrucc.Text = "Seleccionar los datos que desea que aparezcan en la ficha informativa.";
            // 
            // gpBxDatos
            // 
            this.gpBxDatos.Controls.Add(this.chckLstBxDatos);
            this.gpBxDatos.Location = new System.Drawing.Point(12, 48);
            this.gpBxDatos.Name = "gpBxDatos";
            this.gpBxDatos.Size = new System.Drawing.Size(635, 185);
            this.gpBxDatos.TabIndex = 1;
            this.gpBxDatos.TabStop = false;
            this.gpBxDatos.Text = "Selección de datos";
            // 
            // chckLstBxDatos
            // 
            this.chckLstBxDatos.CheckOnClick = true;
            this.chckLstBxDatos.ColumnWidth = 205;
            this.chckLstBxDatos.FormattingEnabled = true;
            this.chckLstBxDatos.Items.AddRange(new object[] {
            "Propietario",
            "Identificador",
            "Nombre",
            "Dirección",
            "Número Interior",
            "Número Exterior",
            "Colonia",
            "Municipio",
            "Estado",
            "Código Postal",
            "País",
            "Área terreno M2 Inmueble",
            "Área construcción M2 Inmueble",
            "Área terreno M2 Conjunto",
            "Área construcción M2 Conjunto",
            "Valor catastral",
            "Valor comercial",
            "Uso",
            "Descripción General",
            "Arrendatario",
            "Renta mensual",
            "Fecha de última renta",
            "Incluir Conjunto"});
            this.chckLstBxDatos.Location = new System.Drawing.Point(7, 20);
            this.chckLstBxDatos.MultiColumn = true;
            this.chckLstBxDatos.Name = "chckLstBxDatos";
            this.chckLstBxDatos.Size = new System.Drawing.Size(622, 154);
            this.chckLstBxDatos.TabIndex = 0;
            this.chckLstBxDatos.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chckLstBxDatos_ItemCheck);
            this.chckLstBxDatos.SelectedIndexChanged += new System.EventHandler(this.chckLstBxDatos_SelectedIndexChanged);
            // 
            // chckBxSelectAll
            // 
            this.chckBxSelectAll.AutoSize = true;
            this.chckBxSelectAll.Location = new System.Drawing.Point(15, 25);
            this.chckBxSelectAll.Name = "chckBxSelectAll";
            this.chckBxSelectAll.Size = new System.Drawing.Size(111, 17);
            this.chckBxSelectAll.TabIndex = 9;
            this.chckBxSelectAll.Text = "Seleccionar todos";
            this.chckBxSelectAll.UseVisualStyleBackColor = true;
            this.chckBxSelectAll.CheckedChanged += new System.EventHandler(this.chckBxSelectAll_CheckedChanged);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(572, 239);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 10;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnCancelar_Click);
            // 
            // botonSiguiente
            // 
            this.botonSiguiente.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonSiguiente.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonSiguiente.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonSiguiente.Imagen = ((System.Drawing.Image)(resources.GetObject("botonSiguiente.Imagen")));
            this.botonSiguiente.Location = new System.Drawing.Point(491, 239);
            this.botonSiguiente.Name = "botonSiguiente";
            this.botonSiguiente.Size = new System.Drawing.Size(75, 75);
            this.botonSiguiente.TabIndex = 11;
            this.botonSiguiente.Texto = "Siguiente";
            this.botonSiguiente.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnNext_Click);
            // 
            // botonAtras
            // 
            this.botonAtras.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonAtras.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonAtras.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonAtras.Imagen = ((System.Drawing.Image)(resources.GetObject("botonAtras.Imagen")));
            this.botonAtras.Location = new System.Drawing.Point(410, 239);
            this.botonAtras.Name = "botonAtras";
            this.botonAtras.Size = new System.Drawing.Size(75, 75);
            this.botonAtras.TabIndex = 12;
            this.botonAtras.Texto = "Atras";
            this.botonAtras.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnBack_Click);
            // 
            // Frm_FichaInformativa2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(659, 306);
            this.Controls.Add(this.botonAtras);
            this.Controls.Add(this.botonSiguiente);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.chckBxSelectAll);
            this.Controls.Add(this.gpBxDatos);
            this.Controls.Add(this.lblInstrucc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(675, 345);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(675, 345);
            this.Name = "Frm_FichaInformativa2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ficha Informativa";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_FichaInformativa2_FormClosing);
            this.Load += new System.EventHandler(this.Frm_FichaInformativa2_Load);
            this.gpBxDatos.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInstrucc;
        private System.Windows.Forms.GroupBox gpBxDatos;
        private System.Windows.Forms.CheckedListBox chckLstBxDatos;
        private System.Windows.Forms.CheckBox chckBxSelectAll;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonSiguiente;
        private Controls.Ctrl_Opcion botonAtras;
    }
}