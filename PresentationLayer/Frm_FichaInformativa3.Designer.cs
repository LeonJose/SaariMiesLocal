namespace GestorReportes.PresentationLayer
{
    partial class Frm_FichaInformativa3
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_FichaInformativa3));
            this.lblInstrucc = new System.Windows.Forms.Label();
            this.gpBxImgs = new System.Windows.Forms.GroupBox();
            this.lblInstruc = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chckLstBxImgs = new System.Windows.Forms.CheckedListBox();
            this.gpBxOri = new System.Windows.Forms.GroupBox();
            this.rdioBnVer = new System.Windows.Forms.RadioButton();
            this.rdioBnHor = new System.Windows.Forms.RadioButton();
            this.gpBxEspacio = new System.Windows.Forms.GroupBox();
            this.rdBnMas = new System.Windows.Forms.RadioButton();
            this.rdBn1Pag = new System.Windows.Forms.RadioButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonAtras = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonGenerarPdf = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonGenerarPowerPoint = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonGenerarWord = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonEnviarCorreo = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioFotosSeparadas = new System.Windows.Forms.RadioButton();
            this.radioFotosMismaHoja = new System.Windows.Forms.RadioButton();
            this.gpBxImgs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.SuspendLayout();
            this.gpBxOri.SuspendLayout();
            this.gpBxEspacio.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInstrucc
            // 
            this.lblInstrucc.AutoSize = true;
            this.lblInstrucc.Location = new System.Drawing.Point(12, 9);
            this.lblInstrucc.Name = "lblInstrucc";
            this.lblInstrucc.Size = new System.Drawing.Size(344, 13);
            this.lblInstrucc.TabIndex = 1;
            this.lblInstrucc.Text = "Seleccionar los datos que desea que aparezcan en la ficha informativa.";
            // 
            // gpBxImgs
            // 
            this.gpBxImgs.Controls.Add(this.lblInstruc);
            this.gpBxImgs.Controls.Add(this.splitContainer1);
            this.gpBxImgs.Location = new System.Drawing.Point(12, 53);
            this.gpBxImgs.Name = "gpBxImgs";
            this.gpBxImgs.Size = new System.Drawing.Size(602, 301);
            this.gpBxImgs.TabIndex = 2;
            this.gpBxImgs.TabStop = false;
            this.gpBxImgs.Text = "Selección de imágenes";
            // 
            // lblInstruc
            // 
            this.lblInstruc.AutoSize = true;
            this.lblInstruc.Location = new System.Drawing.Point(20, 20);
            this.lblInstruc.Name = "lblInstruc";
            this.lblInstruc.Size = new System.Drawing.Size(511, 26);
            this.lblInstruc.TabIndex = 50;
            this.lblInstruc.Text = "De clic en una imágen para cambiarla de contenedor. \r\nLas imagenes que se encuent" +
    "ran en el contenedor derecho son las que se incluirán en la ficha informativa.";
            // 
            // splitContainer1
            // 
            this.splitContainer1.AllowDrop = true;
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.splitContainer1.Location = new System.Drawing.Point(20, 59);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AllowDrop = true;
            this.splitContainer1.Panel1.AutoScroll = true;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AllowDrop = true;
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Size = new System.Drawing.Size(576, 236);
            this.splitContainer1.SplitterDistance = 283;
            this.splitContainer1.TabIndex = 49;
            // 
            // chckLstBxImgs
            // 
            this.chckLstBxImgs.CheckOnClick = true;
            this.chckLstBxImgs.ColumnWidth = 205;
            this.chckLstBxImgs.FormattingEnabled = true;
            this.chckLstBxImgs.Location = new System.Drawing.Point(392, 9);
            this.chckLstBxImgs.Name = "chckLstBxImgs";
            this.chckLstBxImgs.Size = new System.Drawing.Size(415, 34);
            this.chckLstBxImgs.TabIndex = 0;
            this.chckLstBxImgs.Visible = false;
            // 
            // gpBxOri
            // 
            this.gpBxOri.Controls.Add(this.rdioBnVer);
            this.gpBxOri.Controls.Add(this.rdioBnHor);
            this.gpBxOri.Location = new System.Drawing.Point(620, 53);
            this.gpBxOri.Name = "gpBxOri";
            this.gpBxOri.Size = new System.Drawing.Size(157, 85);
            this.gpBxOri.TabIndex = 42;
            this.gpBxOri.TabStop = false;
            this.gpBxOri.Text = "Orientación";
            // 
            // rdioBnVer
            // 
            this.rdioBnVer.AutoSize = true;
            this.rdioBnVer.Location = new System.Drawing.Point(7, 44);
            this.rdioBnVer.Name = "rdioBnVer";
            this.rdioBnVer.Size = new System.Drawing.Size(60, 17);
            this.rdioBnVer.TabIndex = 1;
            this.rdioBnVer.Text = "Vertical";
            this.rdioBnVer.UseVisualStyleBackColor = true;
            // 
            // rdioBnHor
            // 
            this.rdioBnHor.AutoSize = true;
            this.rdioBnHor.Checked = true;
            this.rdioBnHor.Location = new System.Drawing.Point(7, 20);
            this.rdioBnHor.Name = "rdioBnHor";
            this.rdioBnHor.Size = new System.Drawing.Size(72, 17);
            this.rdioBnHor.TabIndex = 0;
            this.rdioBnHor.TabStop = true;
            this.rdioBnHor.Text = "Horizontal";
            this.rdioBnHor.UseVisualStyleBackColor = true;
            // 
            // gpBxEspacio
            // 
            this.gpBxEspacio.Controls.Add(this.rdBnMas);
            this.gpBxEspacio.Controls.Add(this.rdBn1Pag);
            this.gpBxEspacio.Location = new System.Drawing.Point(620, 143);
            this.gpBxEspacio.Name = "gpBxEspacio";
            this.gpBxEspacio.Size = new System.Drawing.Size(157, 74);
            this.gpBxEspacio.TabIndex = 43;
            this.gpBxEspacio.TabStop = false;
            this.gpBxEspacio.Text = "Espacio";
            // 
            // rdBnMas
            // 
            this.rdBnMas.AutoSize = true;
            this.rdBnMas.Location = new System.Drawing.Point(7, 43);
            this.rdBnMas.Name = "rdBnMas";
            this.rdBnMas.Size = new System.Drawing.Size(116, 17);
            this.rdBnMas.TabIndex = 1;
            this.rdBnMas.Text = "Más de una página";
            this.rdBnMas.UseVisualStyleBackColor = true;
            // 
            // rdBn1Pag
            // 
            this.rdBn1Pag.AutoSize = true;
            this.rdBn1Pag.Checked = true;
            this.rdBn1Pag.Location = new System.Drawing.Point(6, 20);
            this.rdBn1Pag.Name = "rdBn1Pag";
            this.rdBn1Pag.Size = new System.Drawing.Size(67, 17);
            this.rdBn1Pag.TabIndex = 0;
            this.rdBn1Pag.TabStop = true;
            this.rdBn1Pag.Text = "1 Página";
            this.rdBn1Pag.UseVisualStyleBackColor = true;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(620, 325);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(157, 23);
            this.progreso.TabIndex = 50;
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(702, 357);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 51;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.bnCancel_Click);
            // 
            // botonAtras
            // 
            this.botonAtras.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonAtras.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonAtras.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonAtras.Imagen = ((System.Drawing.Image)(resources.GetObject("botonAtras.Imagen")));
            this.botonAtras.Location = new System.Drawing.Point(85, 357);
            this.botonAtras.Name = "botonAtras";
            this.botonAtras.Size = new System.Drawing.Size(75, 75);
            this.botonAtras.TabIndex = 52;
            this.botonAtras.Texto = "Atras";
            this.botonAtras.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.bnBack_Click);
            // 
            // botonGenerarPdf
            // 
            this.botonGenerarPdf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerarPdf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerarPdf.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerarPdf.Imagen = ((System.Drawing.Image)(resources.GetObject("botonGenerarPdf.Imagen")));
            this.botonGenerarPdf.Location = new System.Drawing.Point(583, 357);
            this.botonGenerarPdf.Name = "botonGenerarPdf";
            this.botonGenerarPdf.Size = new System.Drawing.Size(113, 75);
            this.botonGenerarPdf.TabIndex = 53;
            this.botonGenerarPdf.Texto = "Generar en PDF";
            this.botonGenerarPdf.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.bnGeneratePDF_Click);
            this.botonGenerarPdf.Load += new System.EventHandler(this.botonGenerarPdf_Load);
            // 
            // botonGenerarPowerPoint
            // 
            this.botonGenerarPowerPoint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerarPowerPoint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerarPowerPoint.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerarPowerPoint.Imagen = ((System.Drawing.Image)(resources.GetObject("botonGenerarPowerPoint.Imagen")));
            this.botonGenerarPowerPoint.Location = new System.Drawing.Point(417, 357);
            this.botonGenerarPowerPoint.Name = "botonGenerarPowerPoint";
            this.botonGenerarPowerPoint.Size = new System.Drawing.Size(160, 75);
            this.botonGenerarPowerPoint.TabIndex = 54;
            this.botonGenerarPowerPoint.Texto = "Generar en PowerPoint";
            this.botonGenerarPowerPoint.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnGenPP_Click);
            // 
            // botonGenerarWord
            // 
            this.botonGenerarWord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerarWord.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerarWord.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerarWord.Imagen = ((System.Drawing.Image)(resources.GetObject("botonGenerarWord.Imagen")));
            this.botonGenerarWord.Location = new System.Drawing.Point(292, 357);
            this.botonGenerarWord.Name = "botonGenerarWord";
            this.botonGenerarWord.Size = new System.Drawing.Size(119, 75);
            this.botonGenerarWord.TabIndex = 55;
            this.botonGenerarWord.Texto = "Generar en Word";
            this.botonGenerarWord.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.bnGenerateWord_Click);
            // 
            // botonEnviarCorreo
            // 
            this.botonEnviarCorreo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonEnviarCorreo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonEnviarCorreo.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonEnviarCorreo.Imagen = ((System.Drawing.Image)(resources.GetObject("botonEnviarCorreo.Imagen")));
            this.botonEnviarCorreo.Location = new System.Drawing.Point(166, 357);
            this.botonEnviarCorreo.Name = "botonEnviarCorreo";
            this.botonEnviarCorreo.Size = new System.Drawing.Size(120, 75);
            this.botonEnviarCorreo.TabIndex = 56;
            this.botonEnviarCorreo.Texto = "Enviar por correo";
            this.botonEnviarCorreo.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.bnSendMail_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioFotosMismaHoja);
            this.groupBox1.Controls.Add(this.radioFotosSeparadas);
            this.groupBox1.Location = new System.Drawing.Point(620, 223);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(157, 74);
            this.groupBox1.TabIndex = 57;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fotos";
            // 
            // radioFotosSeparadas
            // 
            this.radioFotosSeparadas.AutoSize = true;
            this.radioFotosSeparadas.Location = new System.Drawing.Point(7, 42);
            this.radioFotosSeparadas.Name = "radioFotosSeparadas";
            this.radioFotosSeparadas.Size = new System.Drawing.Size(122, 17);
            this.radioFotosSeparadas.TabIndex = 0;
            this.radioFotosSeparadas.Text = "Fotos en otra página";
            this.radioFotosSeparadas.UseVisualStyleBackColor = true;
            // 
            // radioFotosMismaHoja
            // 
            this.radioFotosMismaHoja.AutoSize = true;
            this.radioFotosMismaHoja.Checked = true;
            this.radioFotosMismaHoja.Location = new System.Drawing.Point(6, 19);
            this.radioFotosMismaHoja.Name = "radioFotosMismaHoja";
            this.radioFotosMismaHoja.Size = new System.Drawing.Size(144, 17);
            this.radioFotosMismaHoja.TabIndex = 1;
            this.radioFotosMismaHoja.TabStop = true;
            this.radioFotosMismaHoja.Text = "Fotos en la misma página";
            this.radioFotosMismaHoja.UseVisualStyleBackColor = true;
            // 
            // Frm_FichaInformativa3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(809, 431);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.botonEnviarCorreo);
            this.Controls.Add(this.botonGenerarWord);
            this.Controls.Add(this.botonGenerarPowerPoint);
            this.Controls.Add(this.botonGenerarPdf);
            this.Controls.Add(this.botonAtras);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.chckLstBxImgs);
            this.Controls.Add(this.gpBxEspacio);
            this.Controls.Add(this.gpBxOri);
            this.Controls.Add(this.gpBxImgs);
            this.Controls.Add(this.lblInstrucc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(825, 470);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(825, 470);
            this.Name = "Frm_FichaInformativa3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ficha Informativa";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_FichaInformativa3_FormClosing);
            this.Load += new System.EventHandler(this.Frm_FichaInformativa3_Load);
            this.gpBxImgs.ResumeLayout(false);
            this.gpBxImgs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gpBxOri.ResumeLayout(false);
            this.gpBxOri.PerformLayout();
            this.gpBxEspacio.ResumeLayout(false);
            this.gpBxEspacio.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInstrucc;
        private System.Windows.Forms.GroupBox gpBxImgs;
        private System.Windows.Forms.CheckedListBox chckLstBxImgs;
        private System.Windows.Forms.GroupBox gpBxOri;
        private System.Windows.Forms.RadioButton rdioBnVer;
        private System.Windows.Forms.RadioButton rdioBnHor;
        private System.Windows.Forms.GroupBox gpBxEspacio;
        private System.Windows.Forms.RadioButton rdBnMas;
        private System.Windows.Forms.RadioButton rdBn1Pag;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblInstruc;
        private System.Windows.Forms.ProgressBar progreso;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonAtras;
        private Controls.Ctrl_Opcion botonGenerarPdf;
        private Controls.Ctrl_Opcion botonGenerarPowerPoint;
        private Controls.Ctrl_Opcion botonGenerarWord;
        private Controls.Ctrl_Opcion botonEnviarCorreo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioFotosMismaHoja;
        private System.Windows.Forms.RadioButton radioFotosSeparadas;
    }
}