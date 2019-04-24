namespace GestorReportes.PresentationLayer
{
    partial class Frm_TipoCambio
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textoTipoCambio = new System.Windows.Forms.TextBox();
            this.botonAceptar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(374, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "No se ha encontrado el tipo de cambio para la fecha de corte. Debe capturar el ti" +
                "po de cambio para el reporte";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Tipo de cambio:";
            // 
            // textoTipoCambio
            // 
            this.textoTipoCambio.Location = new System.Drawing.Point(130, 55);
            this.textoTipoCambio.Name = "textoTipoCambio";
            this.textoTipoCambio.Size = new System.Drawing.Size(100, 20);
            this.textoTipoCambio.TabIndex = 2;
            this.textoTipoCambio.Text = "1.0";
            this.textoTipoCambio.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textoTipoCambio_KeyPress);
            // 
            // botonAceptar
            // 
            this.botonAceptar.Location = new System.Drawing.Point(256, 53);
            this.botonAceptar.Name = "botonAceptar";
            this.botonAceptar.Size = new System.Drawing.Size(75, 23);
            this.botonAceptar.TabIndex = 3;
            this.botonAceptar.Text = "Aceptar";
            this.botonAceptar.UseVisualStyleBackColor = true;
            this.botonAceptar.Click += new System.EventHandler(this.botonAceptar_Click);
            // 
            // Frm_TipoCambio
            // 
            this.AcceptButton = this.botonAceptar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 97);
            this.Controls.Add(this.botonAceptar);
            this.Controls.Add(this.textoTipoCambio);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 125);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 125);
            this.Name = "Frm_TipoCambio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tipo de cambio";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_TipoCambio_FormClosing);
            this.Load += new System.EventHandler(this.Frm_TipoCambio_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textoTipoCambio;
        private System.Windows.Forms.Button botonAceptar;
    }
}