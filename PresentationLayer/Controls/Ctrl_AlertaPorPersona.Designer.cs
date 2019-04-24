namespace GestorReportes.PresentationLayer.Controls
{
    partial class Ctrl_AlertaPorPersona
    {
        /// <summary> 
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar 
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblRFC = new System.Windows.Forms.Label();
            this.cmbBxTiposAlerta = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblRFC
            // 
            this.lblRFC.AutoSize = true;
            this.lblRFC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRFC.Location = new System.Drawing.Point(3, 11);
            this.lblRFC.Name = "lblRFC";
            this.lblRFC.Size = new System.Drawing.Size(31, 13);
            this.lblRFC.TabIndex = 0;
            this.lblRFC.Text = "RFC";
            // 
            // cmbBxTiposAlerta
            // 
            this.cmbBxTiposAlerta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxTiposAlerta.FormattingEnabled = true;
            this.cmbBxTiposAlerta.Location = new System.Drawing.Point(154, 8);
            this.cmbBxTiposAlerta.Name = "cmbBxTiposAlerta";
            this.cmbBxTiposAlerta.Size = new System.Drawing.Size(293, 21);
            this.cmbBxTiposAlerta.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(111, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Alerta:";
            // 
            // Ctrl_AlertaPorPersona
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbBxTiposAlerta);
            this.Controls.Add(this.lblRFC);
            this.Name = "Ctrl_AlertaPorPersona";
            this.Size = new System.Drawing.Size(450, 35);
            this.Load += new System.EventHandler(this.Ctrl_AlertaPorPersona_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblRFC;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.ComboBox cmbBxTiposAlerta;

    }
}
