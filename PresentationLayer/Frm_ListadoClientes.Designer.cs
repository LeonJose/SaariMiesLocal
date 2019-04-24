namespace GestorReportes.PresentationLayer
{
    partial class Frm_ListadoClientes
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
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.radioPdf = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 46);
            this.label1.TabIndex = 0;
            this.label1.Text = "Seleccione el formato deseado y de clic en Generar reporte";
            // 
            // radioExcel
            // 
            this.radioExcel.AutoSize = true;
            this.radioExcel.Checked = true;
            this.radioExcel.Location = new System.Drawing.Point(15, 62);
            this.radioExcel.Name = "radioExcel";
            this.radioExcel.Size = new System.Drawing.Size(51, 17);
            this.radioExcel.TabIndex = 1;
            this.radioExcel.TabStop = true;
            this.radioExcel.Text = "Excel";
            this.radioExcel.UseVisualStyleBackColor = true;
            // 
            // radioPdf
            // 
            this.radioPdf.AutoSize = true;
            this.radioPdf.Location = new System.Drawing.Point(86, 62);
            this.radioPdf.Name = "radioPdf";
            this.radioPdf.Size = new System.Drawing.Size(46, 17);
            this.radioPdf.TabIndex = 2;
            this.radioPdf.TabStop = true;
            this.radioPdf.Text = "PDF";
            this.radioPdf.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 87);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Generar reporte";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Frm_ListadoClientes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(144, 122);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.radioPdf);
            this.Controls.Add(this.radioExcel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(150, 150);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(150, 150);
            this.Name = "Frm_ListadoClientes";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Listado de clientes";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioExcel;
        private System.Windows.Forms.RadioButton radioPdf;
        private System.Windows.Forms.Button button1;
    }
}