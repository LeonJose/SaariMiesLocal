namespace GestorReportes.PresentationLayer
{
    partial class Frm_EditarXMLAntilavado
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_EditarXMLAntilavado));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.botonExaminar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.txtBxRutaArchivo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.opnFlDlgAntilav = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowPanelAlertasPorPersonas = new System.Windows.Forms.FlowLayoutPanel();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonAceptar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(460, 44);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.botonExaminar);
            this.groupBox1.Controls.Add(this.txtBxRutaArchivo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(15, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(457, 102);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seleccione el archivo antilavado (.xml)";
            // 
            // botonExaminar
            // 
            this.botonExaminar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonExaminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonExaminar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonExaminar.Imagen = ((System.Drawing.Image)(resources.GetObject("botonExaminar.Imagen")));
            this.botonExaminar.Location = new System.Drawing.Point(365, 19);
            this.botonExaminar.Name = "botonExaminar";
            this.botonExaminar.Size = new System.Drawing.Size(86, 75);
            this.botonExaminar.TabIndex = 2;
            this.botonExaminar.Texto = "Examinar...";
            this.botonExaminar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnExaminar_Click);
            // 
            // txtBxRutaArchivo
            // 
            this.txtBxRutaArchivo.Location = new System.Drawing.Point(58, 24);
            this.txtBxRutaArchivo.Name = "txtBxRutaArchivo";
            this.txtBxRutaArchivo.ReadOnly = true;
            this.txtBxRutaArchivo.Size = new System.Drawing.Size(301, 20);
            this.txtBxRutaArchivo.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Archivo:";
            // 
            // opnFlDlgAntilav
            // 
            this.opnFlDlgAntilav.DefaultExt = "xml";
            this.opnFlDlgAntilav.Filter = "Archivos Antilavado|*.xml";
            this.opnFlDlgAntilav.Title = "Seleccione el archivo xml al que desea cambiar las alertas.";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.flowPanelAlertasPorPersonas);
            this.panel1.Location = new System.Drawing.Point(15, 164);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(457, 191);
            this.panel1.TabIndex = 2;
            // 
            // flowPanelAlertasPorPersonas
            // 
            this.flowPanelAlertasPorPersonas.AutoScroll = true;
            this.flowPanelAlertasPorPersonas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowPanelAlertasPorPersonas.Location = new System.Drawing.Point(0, 0);
            this.flowPanelAlertasPorPersonas.Name = "flowPanelAlertasPorPersonas";
            this.flowPanelAlertasPorPersonas.Size = new System.Drawing.Size(457, 191);
            this.flowPanelAlertasPorPersonas.TabIndex = 0;
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(391, 361);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 4;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnCancelar_Click);
            // 
            // botonAceptar
            // 
            this.botonAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonAceptar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonAceptar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonAceptar.Imagen = ((System.Drawing.Image)(resources.GetObject("botonAceptar.Imagen")));
            this.botonAceptar.Location = new System.Drawing.Point(310, 361);
            this.botonAceptar.Name = "botonAceptar";
            this.botonAceptar.Size = new System.Drawing.Size(75, 75);
            this.botonAceptar.TabIndex = 5;
            this.botonAceptar.Texto = "Aceptar";
            this.botonAceptar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnAceptar_Click);
            // 
            // Frm_EditarXMLAntilavado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(494, 442);
            this.Controls.Add(this.botonAceptar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 470);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 470);
            this.Name = "Frm_EditarXMLAntilavado";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Editar XML Antilavado";
            this.Load += new System.EventHandler(this.Frm_EditarXMLAntilavado_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBxRutaArchivo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog opnFlDlgAntilav;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowPanelAlertasPorPersonas;
        private Controls.Ctrl_Opcion botonExaminar;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonAceptar;
    }
}