namespace GestorReportes.PresentationLayer
{
    partial class Frm_ConfigurarPolizaEgreso
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ConfigurarPolizaEgreso));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Importe de gasto | IG");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Importe Retención ISR | RISR");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Importe Retención IVA | RIV");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Importe total | IT");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Importe total de IVA | ITI");
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.botonGuardar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonAgregarControl = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.pnlContenedorConfigs = new System.Windows.Forms.Panel();
            this.flowConfiguraciones = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCierraParent = new System.Windows.Forms.Button();
            this.btnAbreParent = new System.Windows.Forms.Button();
            this.btnDivide = new System.Windows.Forms.Button();
            this.btnMultiplica = new System.Windows.Forms.Button();
            this.btnMenos = new System.Windows.Forms.Button();
            this.btnMas = new System.Windows.Forms.Button();
            this.lstVwDatos = new System.Windows.Forms.ListView();
            this.datosCulumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstVwTipos = new System.Windows.Forms.ListView();
            this.tipoColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.radioProvision = new System.Windows.Forms.RadioButton();
            this.radioEgreso = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.comboInmobiliaria = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.pnlContenedorConfigs.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.botonGuardar);
            this.groupBox1.Controls.Add(this.botonCancelar);
            this.groupBox1.Controls.Add(this.botonAgregarControl);
            this.groupBox1.Controls.Add(this.pnlContenedorConfigs);
            this.groupBox1.Controls.Add(this.btnCierraParent);
            this.groupBox1.Controls.Add(this.btnAbreParent);
            this.groupBox1.Controls.Add(this.btnDivide);
            this.groupBox1.Controls.Add(this.btnMultiplica);
            this.groupBox1.Controls.Add(this.btnMenos);
            this.groupBox1.Controls.Add(this.btnMas);
            this.groupBox1.Controls.Add(this.lstVwDatos);
            this.groupBox1.Controls.Add(this.lstVwTipos);
            this.groupBox1.Controls.Add(this.radioProvision);
            this.groupBox1.Controls.Add(this.radioEgreso);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboInmobiliaria);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(544, 622);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seleccione las opciones adecuadas para configurar como desea que sea realicen las" +
    " pólizas de egreso para la inmobiliaria seleccionada. Para agregar un dato a la " +
    "fórmula dé doble clic.";
            // 
            // botonGuardar
            // 
            this.botonGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGuardar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGuardar.Imagen = ((System.Drawing.Image)(resources.GetObject("botonGuardar.Imagen")));
            this.botonGuardar.Location = new System.Drawing.Point(376, 541);
            this.botonGuardar.Name = "botonGuardar";
            this.botonGuardar.Size = new System.Drawing.Size(75, 75);
            this.botonGuardar.TabIndex = 23;
            this.botonGuardar.Texto = "Guardar";
            this.botonGuardar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGuardar_Click);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(457, 541);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 22;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_Click);
            // 
            // botonAgregarControl
            // 
            this.botonAgregarControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonAgregarControl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonAgregarControl.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonAgregarControl.Imagen = ((System.Drawing.Image)(resources.GetObject("botonAgregarControl.Imagen")));
            this.botonAgregarControl.Location = new System.Drawing.Point(15, 244);
            this.botonAgregarControl.Name = "botonAgregarControl";
            this.botonAgregarControl.Size = new System.Drawing.Size(75, 75);
            this.botonAgregarControl.TabIndex = 21;
            this.botonAgregarControl.Texto = "Agregar";
            this.botonAgregarControl.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnAgregarControl_Click);
            // 
            // pnlContenedorConfigs
            // 
            this.pnlContenedorConfigs.Controls.Add(this.flowConfiguraciones);
            this.pnlContenedorConfigs.Location = new System.Drawing.Point(15, 325);
            this.pnlContenedorConfigs.Name = "pnlContenedorConfigs";
            this.pnlContenedorConfigs.Size = new System.Drawing.Size(517, 206);
            this.pnlContenedorConfigs.TabIndex = 18;
            // 
            // flowConfiguraciones
            // 
            this.flowConfiguraciones.AutoScroll = true;
            this.flowConfiguraciones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowConfiguraciones.Location = new System.Drawing.Point(0, 0);
            this.flowConfiguraciones.Name = "flowConfiguraciones";
            this.flowConfiguraciones.Size = new System.Drawing.Size(517, 206);
            this.flowConfiguraciones.TabIndex = 0;
            // 
            // btnCierraParent
            // 
            this.btnCierraParent.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCierraParent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCierraParent.Location = new System.Drawing.Point(477, 244);
            this.btnCierraParent.Name = "btnCierraParent";
            this.btnCierraParent.Size = new System.Drawing.Size(23, 23);
            this.btnCierraParent.TabIndex = 17;
            this.btnCierraParent.Text = ")";
            this.btnCierraParent.UseVisualStyleBackColor = true;
            this.btnCierraParent.MouseEnter += new System.EventHandler(this.mouseEntered);
            this.btnCierraParent.MouseLeave += new System.EventHandler(this.mouseLeft);
            // 
            // btnAbreParent
            // 
            this.btnAbreParent.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAbreParent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbreParent.Location = new System.Drawing.Point(448, 244);
            this.btnAbreParent.Name = "btnAbreParent";
            this.btnAbreParent.Size = new System.Drawing.Size(23, 23);
            this.btnAbreParent.TabIndex = 16;
            this.btnAbreParent.Text = "(";
            this.btnAbreParent.UseVisualStyleBackColor = true;
            this.btnAbreParent.MouseEnter += new System.EventHandler(this.mouseEntered);
            this.btnAbreParent.MouseLeave += new System.EventHandler(this.mouseLeft);
            // 
            // btnDivide
            // 
            this.btnDivide.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDivide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDivide.Location = new System.Drawing.Point(419, 244);
            this.btnDivide.Name = "btnDivide";
            this.btnDivide.Size = new System.Drawing.Size(23, 23);
            this.btnDivide.TabIndex = 15;
            this.btnDivide.Text = "/";
            this.btnDivide.UseVisualStyleBackColor = true;
            this.btnDivide.MouseEnter += new System.EventHandler(this.mouseEntered);
            this.btnDivide.MouseLeave += new System.EventHandler(this.mouseLeft);
            // 
            // btnMultiplica
            // 
            this.btnMultiplica.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMultiplica.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMultiplica.Location = new System.Drawing.Point(390, 244);
            this.btnMultiplica.Name = "btnMultiplica";
            this.btnMultiplica.Size = new System.Drawing.Size(23, 23);
            this.btnMultiplica.TabIndex = 14;
            this.btnMultiplica.Text = "x";
            this.btnMultiplica.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.btnMultiplica.UseVisualStyleBackColor = true;
            this.btnMultiplica.MouseEnter += new System.EventHandler(this.mouseEntered);
            this.btnMultiplica.MouseLeave += new System.EventHandler(this.mouseLeft);
            // 
            // btnMenos
            // 
            this.btnMenos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMenos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenos.Location = new System.Drawing.Point(361, 244);
            this.btnMenos.Name = "btnMenos";
            this.btnMenos.Size = new System.Drawing.Size(23, 23);
            this.btnMenos.TabIndex = 13;
            this.btnMenos.Text = "-";
            this.btnMenos.UseVisualStyleBackColor = true;
            this.btnMenos.MouseEnter += new System.EventHandler(this.mouseEntered);
            this.btnMenos.MouseLeave += new System.EventHandler(this.mouseLeft);
            // 
            // btnMas
            // 
            this.btnMas.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMas.Location = new System.Drawing.Point(332, 244);
            this.btnMas.Name = "btnMas";
            this.btnMas.Size = new System.Drawing.Size(23, 23);
            this.btnMas.TabIndex = 12;
            this.btnMas.Text = "+";
            this.btnMas.UseVisualStyleBackColor = true;
            this.btnMas.MouseEnter += new System.EventHandler(this.mouseEntered);
            this.btnMas.MouseLeave += new System.EventHandler(this.mouseLeft);
            // 
            // lstVwDatos
            // 
            this.lstVwDatos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.datosCulumn});
            this.lstVwDatos.FullRowSelect = true;
            this.lstVwDatos.HideSelection = false;
            this.lstVwDatos.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5});
            this.lstVwDatos.Location = new System.Drawing.Point(332, 88);
            this.lstVwDatos.MultiSelect = false;
            this.lstVwDatos.Name = "lstVwDatos";
            this.lstVwDatos.Size = new System.Drawing.Size(200, 150);
            this.lstVwDatos.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstVwDatos.TabIndex = 10;
            this.lstVwDatos.UseCompatibleStateImageBehavior = false;
            this.lstVwDatos.View = System.Windows.Forms.View.Details;
            this.lstVwDatos.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstVwDatos_MouseDoubleClick);
            // 
            // datosCulumn
            // 
            this.datosCulumn.Text = "Datos";
            this.datosCulumn.Width = 195;
            // 
            // lstVwTipos
            // 
            this.lstVwTipos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.tipoColumn});
            this.lstVwTipos.FullRowSelect = true;
            this.lstVwTipos.HideSelection = false;
            this.lstVwTipos.Location = new System.Drawing.Point(15, 88);
            this.lstVwTipos.MultiSelect = false;
            this.lstVwTipos.Name = "lstVwTipos";
            this.lstVwTipos.Size = new System.Drawing.Size(200, 150);
            this.lstVwTipos.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstVwTipos.TabIndex = 9;
            this.lstVwTipos.UseCompatibleStateImageBehavior = false;
            this.lstVwTipos.View = System.Windows.Forms.View.Details;
            this.lstVwTipos.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstVwTipos_MouseDoubleClick);
            // 
            // tipoColumn
            // 
            this.tipoColumn.Text = "Tipos";
            this.tipoColumn.Width = 195;
            // 
            // radioProvision
            // 
            this.radioProvision.AutoSize = true;
            this.radioProvision.Location = new System.Drawing.Point(177, 65);
            this.radioProvision.Name = "radioProvision";
            this.radioProvision.Size = new System.Drawing.Size(68, 17);
            this.radioProvision.TabIndex = 8;
            this.radioProvision.Text = "Provisión";
            this.radioProvision.UseVisualStyleBackColor = true;
            // 
            // radioEgreso
            // 
            this.radioEgreso.AutoSize = true;
            this.radioEgreso.Checked = true;
            this.radioEgreso.Location = new System.Drawing.Point(113, 65);
            this.radioEgreso.Name = "radioEgreso";
            this.radioEgreso.Size = new System.Drawing.Size(58, 17);
            this.radioEgreso.TabIndex = 7;
            this.radioEgreso.TabStop = true;
            this.radioEgreso.Text = "Egreso";
            this.radioEgreso.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Tipo de póliza:";
            // 
            // comboInmobiliaria
            // 
            this.comboInmobiliaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInmobiliaria.DropDownWidth = 364;
            this.comboInmobiliaria.FormattingEnabled = true;
            this.comboInmobiliaria.Location = new System.Drawing.Point(113, 38);
            this.comboInmobiliaria.Name = "comboInmobiliaria";
            this.comboInmobiliaria.Size = new System.Drawing.Size(419, 21);
            this.comboInmobiliaria.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Inmobiliaria:";
            // 
            // Frm_ConfigurarPolizaEgreso
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(544, 622);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(550, 650);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(550, 650);
            this.Name = "Frm_ConfigurarPolizaEgreso";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración de póliza contable de egreso";
            this.Load += new System.EventHandler(this.Frm_ConfigurarPolizaEgreso_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlContenedorConfigs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel pnlContenedorConfigs;
        private System.Windows.Forms.FlowLayoutPanel flowConfiguraciones;
        private System.Windows.Forms.Button btnCierraParent;
        private System.Windows.Forms.Button btnAbreParent;
        private System.Windows.Forms.Button btnDivide;
        private System.Windows.Forms.Button btnMultiplica;
        private System.Windows.Forms.Button btnMenos;
        private System.Windows.Forms.Button btnMas;
        private System.Windows.Forms.ListView lstVwDatos;
        private System.Windows.Forms.ColumnHeader datosCulumn;
        private System.Windows.Forms.ListView lstVwTipos;
        private System.Windows.Forms.ColumnHeader tipoColumn;
        private System.Windows.Forms.RadioButton radioProvision;
        private System.Windows.Forms.RadioButton radioEgreso;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboInmobiliaria;
        private System.Windows.Forms.Label label1;
        private Controls.Ctrl_Opcion botonAgregarControl;
        private Controls.Ctrl_Opcion botonGuardar;
        private Controls.Ctrl_Opcion botonCancelar;
    }
}