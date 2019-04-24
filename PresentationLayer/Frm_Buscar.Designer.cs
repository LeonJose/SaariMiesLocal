namespace GestorReportes.PresentationLayer
{
    partial class Frm_Buscar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Buscar));
            this.progBar = new System.Windows.Forms.ProgressBar();
            this.lblCargando = new System.Windows.Forms.Label();
            this.lblDescr = new System.Windows.Forms.Label();
            this.bckgWork = new System.ComponentModel.BackgroundWorker();
            this.lblCancelar = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progBar
            // 
            this.progBar.Location = new System.Drawing.Point(63, 117);
            this.progBar.Name = "progBar";
            this.progBar.Size = new System.Drawing.Size(144, 14);
            this.progBar.TabIndex = 1;
            // 
            // lblCargando
            // 
            this.lblCargando.AutoSize = true;
            this.lblCargando.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCargando.ForeColor = System.Drawing.Color.White;
            this.lblCargando.Location = new System.Drawing.Point(93, 69);
            this.lblCargando.Name = "lblCargando";
            this.lblCargando.Size = new System.Drawing.Size(93, 17);
            this.lblCargando.TabIndex = 2;
            this.lblCargando.Text = "Cargando...";
            // 
            // lblDescr
            // 
            this.lblDescr.AutoSize = true;
            this.lblDescr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescr.ForeColor = System.Drawing.Color.White;
            this.lblDescr.Location = new System.Drawing.Point(49, 189);
            this.lblDescr.Name = "lblDescr";
            this.lblDescr.Size = new System.Drawing.Size(167, 13);
            this.lblDescr.TabIndex = 3;
            this.lblDescr.Text = "Estableciendo conexión a datos...";
            // 
            // bckgWork
            // 
            this.bckgWork.WorkerReportsProgress = true;
            this.bckgWork.WorkerSupportsCancellation = true;
            // 
            // lblCancelar
            // 
            this.lblCancelar.AutoSize = true;
            this.lblCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCancelar.ForeColor = System.Drawing.Color.LightGray;
            this.lblCancelar.Location = new System.Drawing.Point(218, 225);
            this.lblCancelar.Name = "lblCancelar";
            this.lblCancelar.Size = new System.Drawing.Size(49, 13);
            this.lblCancelar.TabIndex = 4;
            this.lblCancelar.Text = "Cancelar";
            this.lblCancelar.Click += new System.EventHandler(this.lblCancelar_Click);
            this.lblCancelar.MouseLeave += new System.EventHandler(this.lblCancelar_MouseLeave);
            this.lblCancelar.MouseHover += new System.EventHandler(this.lblCancelar_MouseHover);
            // 
            // Frm_Buscar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(285, 256);
            this.Controls.Add(this.lblCancelar);
            this.Controls.Add(this.lblDescr);
            this.Controls.Add(this.lblCargando);
            this.Controls.Add(this.progBar);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_Buscar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Búsqueda Global";
            this.Load += new System.EventHandler(this.Frm_Buscar_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progBar;
        private System.Windows.Forms.Label lblCargando;
        private System.Windows.Forms.Label lblDescr;
        private System.ComponentModel.BackgroundWorker bckgWork;
        private System.Windows.Forms.Label lblCancelar;
    }
}