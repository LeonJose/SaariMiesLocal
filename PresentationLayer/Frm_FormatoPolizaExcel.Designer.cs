namespace GestorReportes.PresentationLayer
{
    partial class Frm_FormatoPolizaExcel
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Consecutivo");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Contador");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Fecha de movimiento");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Cuenta");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Descripción de cuenta");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Cargos");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Abonos");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Descripción");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("Número de recibo");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("Serie y folio de CFD");
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("Estatus");
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("Cliente Razón Social");
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem("Cliente RFC");
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem("Inmobiliaria Razón Social");
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem("Inmobiliaria RFC");
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem("Conjunto");
            System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem("Inmueble");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_FormatoPolizaExcel));
            this.lstVwOpciones = new System.Windows.Forms.ListView();
            this.columnOpc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstVwOpcSeleccionadas = new System.Windows.Forms.ListView();
            this.columnOpcSel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPersonalizado = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnQuitar = new System.Windows.Forms.Button();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstVwOpciones
            // 
            this.lstVwOpciones.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnOpc});
            this.lstVwOpciones.FullRowSelect = true;
            this.lstVwOpciones.HideSelection = false;
            this.lstVwOpciones.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12,
            listViewItem13,
            listViewItem14,
            listViewItem15,
            listViewItem16,
            listViewItem17});
            this.lstVwOpciones.Location = new System.Drawing.Point(12, 65);
            this.lstVwOpciones.MultiSelect = false;
            this.lstVwOpciones.Name = "lstVwOpciones";
            this.lstVwOpciones.Size = new System.Drawing.Size(180, 320);
            this.lstVwOpciones.TabIndex = 0;
            this.lstVwOpciones.UseCompatibleStateImageBehavior = false;
            this.lstVwOpciones.View = System.Windows.Forms.View.Details;
            this.lstVwOpciones.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstVwOpciones_MouseDoubleClick);
            // 
            // columnOpc
            // 
            this.columnOpc.Text = "Opción";
            this.columnOpc.Width = 176;
            // 
            // lstVwOpcSeleccionadas
            // 
            this.lstVwOpcSeleccionadas.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnOpcSel,
            this.columnPersonalizado});
            this.lstVwOpcSeleccionadas.FullRowSelect = true;
            this.lstVwOpcSeleccionadas.HideSelection = false;
            this.lstVwOpcSeleccionadas.Location = new System.Drawing.Point(245, 65);
            this.lstVwOpcSeleccionadas.MultiSelect = false;
            this.lstVwOpcSeleccionadas.Name = "lstVwOpcSeleccionadas";
            this.lstVwOpcSeleccionadas.Size = new System.Drawing.Size(237, 320);
            this.lstVwOpcSeleccionadas.TabIndex = 1;
            this.lstVwOpcSeleccionadas.UseCompatibleStateImageBehavior = false;
            this.lstVwOpcSeleccionadas.View = System.Windows.Forms.View.Details;
            this.lstVwOpcSeleccionadas.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstVwOpcSeleccionadas_MouseDoubleClick);
            // 
            // columnOpcSel
            // 
            this.columnOpcSel.Text = "Opciones seleccionadas";
            this.columnOpcSel.Width = 130;
            // 
            // columnPersonalizado
            // 
            this.columnPersonalizado.Text = "Personalizado";
            this.columnPersonalizado.Width = 100;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(470, 53);
            this.label1.TabIndex = 2;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(198, 116);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(41, 23);
            this.btnAgregar.TabIndex = 3;
            this.btnAgregar.Text = ">";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // btnQuitar
            // 
            this.btnQuitar.Location = new System.Drawing.Point(198, 156);
            this.btnQuitar.Name = "btnQuitar";
            this.btnQuitar.Size = new System.Drawing.Size(41, 23);
            this.btnQuitar.TabIndex = 4;
            this.btnQuitar.Text = "<";
            this.btnQuitar.UseVisualStyleBackColor = true;
            this.btnQuitar.Click += new System.EventHandler(this.btnQuitar_Click);
            // 
            // btnAceptar
            // 
            this.btnAceptar.Location = new System.Drawing.Point(302, 407);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(75, 23);
            this.btnAceptar.TabIndex = 5;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(407, 407);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 6;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // Frm_FormatoPolizaExcel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 442);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.btnQuitar);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstVwOpcSeleccionadas);
            this.Controls.Add(this.lstVwOpciones);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 470);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 470);
            this.Name = "Frm_FormatoPolizaExcel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Formato póliza para Excel";
            this.Load += new System.EventHandler(this.Frm_FormatoPolizaExcel_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lstVwOpciones;
        private System.Windows.Forms.ColumnHeader columnOpc;
        private System.Windows.Forms.ListView lstVwOpcSeleccionadas;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader columnOpcSel;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnQuitar;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ColumnHeader columnPersonalizado;
    }
}