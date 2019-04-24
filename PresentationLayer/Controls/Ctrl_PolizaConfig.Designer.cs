namespace GestorReportes.PresentationLayer.Controls
{
    partial class Ctrl_PolizaConfig
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
            this.cmbBxCargoAbono = new System.Windows.Forms.ComboBox();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.rdBtnSelect = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblMonCob = new System.Windows.Forms.Label();
            this.txtBxFormula = new System.Windows.Forms.TextBox();
            this.lblMoneda = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTipo = new System.Windows.Forms.Label();
            this.labelCto = new System.Windows.Forms.Label();
            this.lblValMonCobro = new System.Windows.Forms.Label();
            this.grpBxCtrl = new System.Windows.Forms.GroupBox();
            this.lblClaveTipo = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.grpBxCtrl.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbBxCargoAbono
            // 
            this.cmbBxCargoAbono.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cmbBxCargoAbono.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxCargoAbono.FormattingEnabled = true;
            this.cmbBxCargoAbono.Items.AddRange(new object[] {
            "Cargo",
            "Abono"});
            this.cmbBxCargoAbono.Location = new System.Drawing.Point(23, 13);
            this.cmbBxCargoAbono.Name = "cmbBxCargoAbono";
            this.cmbBxCargoAbono.Size = new System.Drawing.Size(68, 21);
            this.cmbBxCargoAbono.TabIndex = 2;
            // 
            // btnEliminar
            // 
            this.btnEliminar.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.Location = new System.Drawing.Point(748, 13);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(57, 21);
            this.btnEliminar.TabIndex = 5;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.MouseEnter += new System.EventHandler(this.btnEliminar_MouseEnter);
            this.btnEliminar.MouseLeave += new System.EventHandler(this.btnEliminar_MouseLeave);
            // 
            // rdBtnSelect
            // 
            this.rdBtnSelect.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rdBtnSelect.AutoSize = true;
            this.rdBtnSelect.Location = new System.Drawing.Point(3, 17);
            this.rdBtnSelect.Name = "rdBtnSelect";
            this.rdBtnSelect.Size = new System.Drawing.Size(14, 13);
            this.rdBtnSelect.TabIndex = 0;
            this.rdBtnSelect.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 10;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.cmbBxCargoAbono, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.rdBtnSelect, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnEliminar, 9, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblMonCob, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtBxFormula, 8, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblMoneda, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblTipo, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelCto, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblValMonCobro, 6, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 7);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(812, 47);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // lblMonCob
            // 
            this.lblMonCob.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblMonCob.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMonCob.ForeColor = System.Drawing.Color.OrangeRed;
            this.lblMonCob.Location = new System.Drawing.Point(344, 10);
            this.lblMonCob.Margin = new System.Windows.Forms.Padding(10, 0, 3, 0);
            this.lblMonCob.Name = "lblMonCob";
            this.lblMonCob.Size = new System.Drawing.Size(54, 27);
            this.lblMonCob.TabIndex = 8;
            this.lblMonCob.Text = "Moneda Cobro";
            this.lblMonCob.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMonCob.Visible = false;
            // 
            // txtBxFormula
            // 
            this.txtBxFormula.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtBxFormula.Location = new System.Drawing.Point(512, 13);
            this.txtBxFormula.Name = "txtBxFormula";
            this.txtBxFormula.Size = new System.Drawing.Size(230, 20);
            this.txtBxFormula.TabIndex = 4;
            this.txtBxFormula.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBxFormula_KeyDown);
            this.txtBxFormula.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBxFormula_KeyPress);
            // 
            // lblMoneda
            // 
            this.lblMoneda.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblMoneda.AutoSize = true;
            this.lblMoneda.Location = new System.Drawing.Point(286, 17);
            this.lblMoneda.Margin = new System.Windows.Forms.Padding(2, 0, 5, 0);
            this.lblMoneda.Name = "lblMoneda";
            this.lblMoneda.Size = new System.Drawing.Size(36, 13);
            this.lblMoneda.TabIndex = 6;
            this.lblMoneda.Text = "Pesos";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(461, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(10, 0, 1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Formula:";
            // 
            // lblTipo
            // 
            this.lblTipo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTipo.Location = new System.Drawing.Point(97, 3);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Size = new System.Drawing.Size(120, 41);
            this.lblTipo.TabIndex = 1;
            this.lblTipo.Text = "Comp cliente";
            this.lblTipo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCto
            // 
            this.labelCto.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelCto.AutoSize = true;
            this.labelCto.Location = new System.Drawing.Point(232, 17);
            this.labelCto.Margin = new System.Windows.Forms.Padding(10, 0, 2, 0);
            this.labelCto.Name = "labelCto";
            this.labelCto.Size = new System.Drawing.Size(50, 13);
            this.labelCto.TabIndex = 10;
            this.labelCto.Text = "Contrato:";
            // 
            // lblValMonCobro
            // 
            this.lblValMonCobro.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblValMonCobro.AutoSize = true;
            this.lblValMonCobro.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValMonCobro.ForeColor = System.Drawing.Color.OrangeRed;
            this.lblValMonCobro.Location = new System.Drawing.Point(404, 17);
            this.lblValMonCobro.Name = "lblValMonCobro";
            this.lblValMonCobro.Size = new System.Drawing.Size(36, 13);
            this.lblValMonCobro.TabIndex = 11;
            this.lblValMonCobro.Text = "Pesos";
            this.lblValMonCobro.Visible = false;
            // 
            // grpBxCtrl
            // 
            this.grpBxCtrl.Controls.Add(this.tableLayoutPanel1);
            this.grpBxCtrl.Controls.Add(this.lblClaveTipo);
            this.grpBxCtrl.Location = new System.Drawing.Point(0, -6);
            this.grpBxCtrl.Name = "grpBxCtrl";
            this.grpBxCtrl.Size = new System.Drawing.Size(971, 49);
            this.grpBxCtrl.TabIndex = 1;
            this.grpBxCtrl.TabStop = false;
            // 
            // lblClaveTipo
            // 
            this.lblClaveTipo.AutoSize = true;
            this.lblClaveTipo.Location = new System.Drawing.Point(225, 19);
            this.lblClaveTipo.Name = "lblClaveTipo";
            this.lblClaveTipo.Size = new System.Drawing.Size(0, 13);
            this.lblClaveTipo.TabIndex = 7;
            this.lblClaveTipo.Visible = false;
            // 
            // Ctrl_PolizaConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.Controls.Add(this.grpBxCtrl);
            this.Name = "Ctrl_PolizaConfig";
            this.Size = new System.Drawing.Size(820, 46);
            this.Load += new System.EventHandler(this.Ctrl_PolizaConfig_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.grpBxCtrl.ResumeLayout(false);
            this.grpBxCtrl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ComboBox cmbBxCargoAbono;
        public System.Windows.Forms.Button btnEliminar;
        public System.Windows.Forms.RadioButton rdBtnSelect;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.Label lblMonCob;
        public System.Windows.Forms.TextBox txtBxFormula;
        public System.Windows.Forms.Label lblMoneda;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.Label labelCto;
        private System.Windows.Forms.GroupBox grpBxCtrl;
        public System.Windows.Forms.Label lblClaveTipo;
        public System.Windows.Forms.Label lblValMonCobro;

    }
}
