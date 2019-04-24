namespace GestorReportes.PresentationLayer.Controls
{
    partial class Frm_PruebaConsultas
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
            this.gridDatos = new System.Windows.Forms.DataGridView();
            this.IDHistRec = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConceptoGasto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FechaGasto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImporteGasto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IvaGasto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalCheque = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EstatusCXP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EstatusCheque = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumeroCheque = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FechaGeneracionCheque = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FechaImpresionCheque = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IDProveedor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NombreProveedor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Moneda = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TipoCambio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridDatos)).BeginInit();
            this.SuspendLayout();
            // 
            // gridDatos
            // 
            this.gridDatos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridDatos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridDatos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IDHistRec,
            this.ConceptoGasto,
            this.FechaGasto,
            this.ImporteGasto,
            this.IvaGasto,
            this.TotalCheque,
            this.EstatusCXP,
            this.EstatusCheque,
            this.NumeroCheque,
            this.FechaGeneracionCheque,
            this.FechaImpresionCheque,
            this.IDProveedor,
            this.NombreProveedor,
            this.Moneda,
            this.TipoCambio});
            this.gridDatos.Location = new System.Drawing.Point(12, 27);
            this.gridDatos.Name = "gridDatos";
            this.gridDatos.Size = new System.Drawing.Size(771, 325);
            this.gridDatos.TabIndex = 0;
            // 
            // IDHistRec
            // 
            this.IDHistRec.DataPropertyName = "IDHistRec";
            this.IDHistRec.HeaderText = "IDHistRec";
            this.IDHistRec.Name = "IDHistRec";
            // 
            // ConceptoGasto
            // 
            this.ConceptoGasto.DataPropertyName = "ConceptoGasto";
            this.ConceptoGasto.HeaderText = "ConceptoGasto";
            this.ConceptoGasto.Name = "ConceptoGasto";
            // 
            // FechaGasto
            // 
            this.FechaGasto.DataPropertyName = "FechaGasto";
            this.FechaGasto.HeaderText = "FechaGasto";
            this.FechaGasto.Name = "FechaGasto";
            // 
            // ImporteGasto
            // 
            this.ImporteGasto.DataPropertyName = "ImporteGasto";
            this.ImporteGasto.HeaderText = "ImporteGasto";
            this.ImporteGasto.Name = "ImporteGasto";
            // 
            // IvaGasto
            // 
            this.IvaGasto.DataPropertyName = "IvaGasto";
            this.IvaGasto.HeaderText = "IvaGasto";
            this.IvaGasto.Name = "IvaGasto";
            // 
            // TotalCheque
            // 
            this.TotalCheque.DataPropertyName = "TotalCheque";
            this.TotalCheque.HeaderText = "TotalCheque";
            this.TotalCheque.Name = "TotalCheque";
            // 
            // EstatusCXP
            // 
            this.EstatusCXP.DataPropertyName = "EstatusCXP";
            this.EstatusCXP.HeaderText = "EstatusCXP";
            this.EstatusCXP.Name = "EstatusCXP";
            // 
            // EstatusCheque
            // 
            this.EstatusCheque.DataPropertyName = "EstatusCheque";
            this.EstatusCheque.HeaderText = "EstatusCheque";
            this.EstatusCheque.Name = "EstatusCheque";
            // 
            // NumeroCheque
            // 
            this.NumeroCheque.DataPropertyName = "NumeroCheque";
            this.NumeroCheque.HeaderText = "NumeroCheque";
            this.NumeroCheque.Name = "NumeroCheque";
            // 
            // FechaGeneracionCheque
            // 
            this.FechaGeneracionCheque.DataPropertyName = "FechaGeneracionCheque";
            this.FechaGeneracionCheque.HeaderText = "FechaGeneracionCheque";
            this.FechaGeneracionCheque.Name = "FechaGeneracionCheque";
            // 
            // FechaImpresionCheque
            // 
            this.FechaImpresionCheque.DataPropertyName = "FechaImpresionCheque";
            this.FechaImpresionCheque.HeaderText = "FechaImpresionCheque";
            this.FechaImpresionCheque.Name = "FechaImpresionCheque";
            // 
            // IDProveedor
            // 
            this.IDProveedor.DataPropertyName = "IDProveedor";
            this.IDProveedor.HeaderText = "IDProveedor";
            this.IDProveedor.Name = "IDProveedor";
            // 
            // NombreProveedor
            // 
            this.NombreProveedor.DataPropertyName = "NombreProveedor";
            this.NombreProveedor.HeaderText = "NombreProveedor";
            this.NombreProveedor.Name = "NombreProveedor";
            // 
            // Moneda
            // 
            this.Moneda.DataPropertyName = "Moneda";
            this.Moneda.HeaderText = "Moneda";
            this.Moneda.Name = "Moneda";
            // 
            // TipoCambio
            // 
            this.TipoCambio.DataPropertyName = "TipoCambio";
            this.TipoCambio.HeaderText = "TipoCambio";
            this.TipoCambio.Name = "TipoCambio";
            // 
            // Frm_PruebaConsultas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 368);
            this.Controls.Add(this.gridDatos);
            this.Name = "Frm_PruebaConsultas";
            this.Text = "Frm_PruebaConsultas";
            this.Load += new System.EventHandler(this.Frm_PruebaConsultas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridDatos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridDatos;
        private System.Windows.Forms.DataGridViewTextBoxColumn IDHistRec;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConceptoGasto;
        private System.Windows.Forms.DataGridViewTextBoxColumn FechaGasto;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImporteGasto;
        private System.Windows.Forms.DataGridViewTextBoxColumn IvaGasto;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalCheque;
        private System.Windows.Forms.DataGridViewTextBoxColumn EstatusCXP;
        private System.Windows.Forms.DataGridViewTextBoxColumn EstatusCheque;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumeroCheque;
        private System.Windows.Forms.DataGridViewTextBoxColumn FechaGeneracionCheque;
        private System.Windows.Forms.DataGridViewTextBoxColumn FechaImpresionCheque;
        private System.Windows.Forms.DataGridViewTextBoxColumn IDProveedor;
        private System.Windows.Forms.DataGridViewTextBoxColumn NombreProveedor;
        private System.Windows.Forms.DataGridViewTextBoxColumn Moneda;
        private System.Windows.Forms.DataGridViewTextBoxColumn TipoCambio;
    }
}