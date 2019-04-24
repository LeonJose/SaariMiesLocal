namespace GestorReportes.PresentationLayer
{
    partial class Frm_RepBitacora
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
            this.radioExcel = new System.Windows.Forms.RadioButton();
            this.radioPDF = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.groupBoxUsuarios = new System.Windows.Forms.GroupBox();
            this.Rb_AllUser = new System.Windows.Forms.RadioButton();
            this.Rb_Usuario = new System.Windows.Forms.RadioButton();
            this.Cmb_Usuarios = new System.Windows.Forms.ComboBox();
            this.groupBoxClientes = new System.Windows.Forms.GroupBox();
            this.Rb_Todos = new System.Windows.Forms.RadioButton();
            this.Rb_Cliente = new System.Windows.Forms.RadioButton();
            this.Cmb_Clientes = new System.Windows.Forms.ComboBox();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxEtapa = new System.Windows.Forms.GroupBox();
            this.Cmb_Etapa = new System.Windows.Forms.ComboBox();
            this.Rb_AllEtapa = new System.Windows.Forms.RadioButton();
            this.Rb_Etapa = new System.Windows.Forms.RadioButton();
            this.workerReporte = new System.ComponentModel.BackgroundWorker();
            this.groupBoxClasificacion = new System.Windows.Forms.GroupBox();
            this.rb_ClasifProspectos = new System.Windows.Forms.RadioButton();
            this.Rb_AllClasific = new System.Windows.Forms.RadioButton();
            this.Rb_ClasifClientes = new System.Windows.Forms.RadioButton();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.groupBoxUsuarios.SuspendLayout();
            this.groupBoxClientes.SuspendLayout();
            this.groupBoxEtapa.SuspendLayout();
            this.groupBoxClasificacion.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioExcel
            // 
            this.radioExcel.AutoSize = true;
            this.radioExcel.Location = new System.Drawing.Point(140, 389);
            this.radioExcel.Name = "radioExcel";
            this.radioExcel.Size = new System.Drawing.Size(51, 17);
            this.radioExcel.TabIndex = 45;
            this.radioExcel.TabStop = true;
            this.radioExcel.Text = "Excel";
            this.radioExcel.UseVisualStyleBackColor = true;
            // 
            // radioPDF
            // 
            this.radioPDF.AutoSize = true;
            this.radioPDF.Checked = true;
            this.radioPDF.Location = new System.Drawing.Point(89, 389);
            this.radioPDF.Name = "radioPDF";
            this.radioPDF.Size = new System.Drawing.Size(46, 17);
            this.radioPDF.TabIndex = 44;
            this.radioPDF.TabStop = true;
            this.radioPDF.Text = "PDF";
            this.radioPDF.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 391);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 44;
            this.label3.Text = "Formato:";
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(38, 434);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(250, 23);
            this.progreso.TabIndex = 43;
            // 
            // groupBoxUsuarios
            // 
            this.groupBoxUsuarios.Controls.Add(this.Rb_AllUser);
            this.groupBoxUsuarios.Controls.Add(this.Rb_Usuario);
            this.groupBoxUsuarios.Controls.Add(this.Cmb_Usuarios);
            this.groupBoxUsuarios.Location = new System.Drawing.Point(29, 146);
            this.groupBoxUsuarios.Name = "groupBoxUsuarios";
            this.groupBoxUsuarios.Size = new System.Drawing.Size(414, 70);
            this.groupBoxUsuarios.TabIndex = 38;
            this.groupBoxUsuarios.TabStop = false;
            this.groupBoxUsuarios.Text = " Usuarios ";
            // 
            // Rb_AllUser
            // 
            this.Rb_AllUser.AutoSize = true;
            this.Rb_AllUser.Checked = true;
            this.Rb_AllUser.Location = new System.Drawing.Point(13, 19);
            this.Rb_AllUser.Name = "Rb_AllUser";
            this.Rb_AllUser.Size = new System.Drawing.Size(55, 17);
            this.Rb_AllUser.TabIndex = 0;
            this.Rb_AllUser.TabStop = true;
            this.Rb_AllUser.Text = "Todos";
            this.Rb_AllUser.UseVisualStyleBackColor = true;
            this.Rb_AllUser.CheckedChanged += new System.EventHandler(this.Rb_AllUser_CheckedChanged);
            // 
            // Rb_Usuario
            // 
            this.Rb_Usuario.AutoSize = true;
            this.Rb_Usuario.Location = new System.Drawing.Point(283, 19);
            this.Rb_Usuario.Name = "Rb_Usuario";
            this.Rb_Usuario.Size = new System.Drawing.Size(120, 17);
            this.Rb_Usuario.TabIndex = 1;
            this.Rb_Usuario.Text = "Seleccionar Usuario";
            this.Rb_Usuario.UseVisualStyleBackColor = true;
            // 
            // Cmb_Usuarios
            // 
            this.Cmb_Usuarios.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmb_Usuarios.FormattingEnabled = true;
            this.Cmb_Usuarios.Location = new System.Drawing.Point(13, 42);
            this.Cmb_Usuarios.Name = "Cmb_Usuarios";
            this.Cmb_Usuarios.Size = new System.Drawing.Size(390, 21);
            this.Cmb_Usuarios.TabIndex = 2;
            // 
            // groupBoxClientes
            // 
            this.groupBoxClientes.Controls.Add(this.Rb_Todos);
            this.groupBoxClientes.Controls.Add(this.Rb_Cliente);
            this.groupBoxClientes.Controls.Add(this.Cmb_Clientes);
            this.groupBoxClientes.Location = new System.Drawing.Point(29, 68);
            this.groupBoxClientes.Name = "groupBoxClientes";
            this.groupBoxClientes.Size = new System.Drawing.Size(414, 72);
            this.groupBoxClientes.TabIndex = 37;
            this.groupBoxClientes.TabStop = false;
            this.groupBoxClientes.Text = " Clientes ";
            // 
            // Rb_Todos
            // 
            this.Rb_Todos.AutoSize = true;
            this.Rb_Todos.Checked = true;
            this.Rb_Todos.Location = new System.Drawing.Point(13, 19);
            this.Rb_Todos.Name = "Rb_Todos";
            this.Rb_Todos.Size = new System.Drawing.Size(55, 17);
            this.Rb_Todos.TabIndex = 0;
            this.Rb_Todos.TabStop = true;
            this.Rb_Todos.Text = "Todos";
            this.Rb_Todos.UseVisualStyleBackColor = true;
            this.Rb_Todos.CheckedChanged += new System.EventHandler(this.Rb_Todos_CheckedChanged);
            // 
            // Rb_Cliente
            // 
            this.Rb_Cliente.AutoSize = true;
            this.Rb_Cliente.Location = new System.Drawing.Point(283, 19);
            this.Rb_Cliente.Name = "Rb_Cliente";
            this.Rb_Cliente.Size = new System.Drawing.Size(122, 17);
            this.Rb_Cliente.TabIndex = 1;
            this.Rb_Cliente.Text = "Selecccionar Cliente";
            this.Rb_Cliente.UseVisualStyleBackColor = true;
            // 
            // Cmb_Clientes
            // 
            this.Cmb_Clientes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmb_Clientes.FormattingEnabled = true;
            this.Cmb_Clientes.Location = new System.Drawing.Point(13, 42);
            this.Cmb_Clientes.Name = "Cmb_Clientes";
            this.Cmb_Clientes.Size = new System.Drawing.Size(390, 21);
            this.Cmb_Clientes.TabIndex = 2;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(245, 333);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker2.TabIndex = 43;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(31, 333);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 42;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(247, 317);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Fecha Final:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 317);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Fecha Inicial:";
            // 
            // groupBoxEtapa
            // 
            this.groupBoxEtapa.Controls.Add(this.Cmb_Etapa);
            this.groupBoxEtapa.Controls.Add(this.Rb_AllEtapa);
            this.groupBoxEtapa.Controls.Add(this.Rb_Etapa);
            this.groupBoxEtapa.Location = new System.Drawing.Point(31, 222);
            this.groupBoxEtapa.Name = "groupBoxEtapa";
            this.groupBoxEtapa.Size = new System.Drawing.Size(414, 77);
            this.groupBoxEtapa.TabIndex = 40;
            this.groupBoxEtapa.TabStop = false;
            this.groupBoxEtapa.Text = " Etapa de Seguimiento ";
            // 
            // Cmb_Etapa
            // 
            this.Cmb_Etapa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmb_Etapa.FormattingEnabled = true;
            this.Cmb_Etapa.Items.AddRange(new object[] {
            "Pendiente",
            "Terminado"});
            this.Cmb_Etapa.Location = new System.Drawing.Point(16, 42);
            this.Cmb_Etapa.Name = "Cmb_Etapa";
            this.Cmb_Etapa.Size = new System.Drawing.Size(390, 21);
            this.Cmb_Etapa.TabIndex = 2;
            // 
            // Rb_AllEtapa
            // 
            this.Rb_AllEtapa.AutoSize = true;
            this.Rb_AllEtapa.Checked = true;
            this.Rb_AllEtapa.Location = new System.Drawing.Point(16, 19);
            this.Rb_AllEtapa.Name = "Rb_AllEtapa";
            this.Rb_AllEtapa.Size = new System.Drawing.Size(55, 17);
            this.Rb_AllEtapa.TabIndex = 0;
            this.Rb_AllEtapa.TabStop = true;
            this.Rb_AllEtapa.Text = "Todos";
            this.Rb_AllEtapa.UseVisualStyleBackColor = true;
            this.Rb_AllEtapa.CheckedChanged += new System.EventHandler(this.Rb_AllEtapa_CheckedChanged);
            // 
            // Rb_Etapa
            // 
            this.Rb_Etapa.AutoSize = true;
            this.Rb_Etapa.Location = new System.Drawing.Point(280, 19);
            this.Rb_Etapa.Name = "Rb_Etapa";
            this.Rb_Etapa.Size = new System.Drawing.Size(115, 17);
            this.Rb_Etapa.TabIndex = 1;
            this.Rb_Etapa.Text = "Seleccionar  Etapa";
            this.Rb_Etapa.UseVisualStyleBackColor = true;
            // 
            // workerReporte
            // 
            this.workerReporte.WorkerReportsProgress = true;
            this.workerReporte.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerReporte_DoWork);
            this.workerReporte.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerReporte_ProgressChanged);
            this.workerReporte.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerReporte_RunWorkerCompleted);
            // 
            // groupBoxClasificacion
            // 
            this.groupBoxClasificacion.Controls.Add(this.rb_ClasifProspectos);
            this.groupBoxClasificacion.Controls.Add(this.Rb_AllClasific);
            this.groupBoxClasificacion.Controls.Add(this.Rb_ClasifClientes);
            this.groupBoxClasificacion.Location = new System.Drawing.Point(29, 12);
            this.groupBoxClasificacion.Name = "groupBoxClasificacion";
            this.groupBoxClasificacion.Size = new System.Drawing.Size(414, 50);
            this.groupBoxClasificacion.TabIndex = 48;
            this.groupBoxClasificacion.TabStop = false;
            this.groupBoxClasificacion.Text = "Clasificación Clientes/Prospectos";
            // 
            // rb_ClasifProspectos
            // 
            this.rb_ClasifProspectos.AutoSize = true;
            this.rb_ClasifProspectos.Location = new System.Drawing.Point(282, 19);
            this.rb_ClasifProspectos.Name = "rb_ClasifProspectos";
            this.rb_ClasifProspectos.Size = new System.Drawing.Size(78, 17);
            this.rb_ClasifProspectos.TabIndex = 3;
            this.rb_ClasifProspectos.Text = "Prospectos";
            this.rb_ClasifProspectos.UseVisualStyleBackColor = true;
            this.rb_ClasifProspectos.CheckedChanged += new System.EventHandler(this.rb_ClasifProspectos_CheckedChanged);
            // 
            // Rb_AllClasific
            // 
            this.Rb_AllClasific.AutoSize = true;
            this.Rb_AllClasific.Checked = true;
            this.Rb_AllClasific.Location = new System.Drawing.Point(16, 19);
            this.Rb_AllClasific.Name = "Rb_AllClasific";
            this.Rb_AllClasific.Size = new System.Drawing.Size(55, 17);
            this.Rb_AllClasific.TabIndex = 0;
            this.Rb_AllClasific.TabStop = true;
            this.Rb_AllClasific.Text = "Todos";
            this.Rb_AllClasific.UseVisualStyleBackColor = true;
            this.Rb_AllClasific.CheckedChanged += new System.EventHandler(this.Rb_AllClasific_CheckedChanged);
            // 
            // Rb_ClasifClientes
            // 
            this.Rb_ClasifClientes.AutoSize = true;
            this.Rb_ClasifClientes.Location = new System.Drawing.Point(140, 19);
            this.Rb_ClasifClientes.Name = "Rb_ClasifClientes";
            this.Rb_ClasifClientes.Size = new System.Drawing.Size(62, 17);
            this.Rb_ClasifClientes.TabIndex = 1;
            this.Rb_ClasifClientes.Text = "Clientes";
            this.Rb_ClasifClientes.UseVisualStyleBackColor = true;
            this.Rb_ClasifClientes.CheckedChanged += new System.EventHandler(this.Rb_ClasifClientes_CheckedChanged);
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(296, 386);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 46;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonGenerar_ControlClicked);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(376, 386);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 47;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.botonCancelar_ControlClicked);
            // 
            // Frm_RepBitacora
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(469, 482);
            this.Controls.Add(this.groupBoxClasificacion);
            this.Controls.Add(this.groupBoxEtapa);
            this.Controls.Add(this.radioExcel);
            this.Controls.Add(this.radioPDF);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.groupBoxUsuarios);
            this.Controls.Add(this.groupBoxClientes);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_RepBitacora";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de Bitácora de Seguimiento CRM";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_RepBitacora_FormClosing);
            this.Load += new System.EventHandler(this.Frm_RepBitacora_Load);
            this.groupBoxUsuarios.ResumeLayout(false);
            this.groupBoxUsuarios.PerformLayout();
            this.groupBoxClientes.ResumeLayout(false);
            this.groupBoxClientes.PerformLayout();
            this.groupBoxEtapa.ResumeLayout(false);
            this.groupBoxEtapa.PerformLayout();
            this.groupBoxClasificacion.ResumeLayout(false);
            this.groupBoxClasificacion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioExcel;
        private System.Windows.Forms.RadioButton radioPDF;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progreso;
        private Controls.Ctrl_Opcion botonGenerar;
        private Controls.Ctrl_Opcion botonCancelar;
        private System.Windows.Forms.GroupBox groupBoxUsuarios;
        private System.Windows.Forms.RadioButton Rb_AllUser;
        private System.Windows.Forms.RadioButton Rb_Usuario;
        private System.Windows.Forms.ComboBox Cmb_Usuarios;
        private System.Windows.Forms.GroupBox groupBoxClientes;
        private System.Windows.Forms.RadioButton Rb_Todos;
        private System.Windows.Forms.RadioButton Rb_Cliente;
        private System.Windows.Forms.ComboBox Cmb_Clientes;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxEtapa;
        private System.Windows.Forms.ComboBox Cmb_Etapa;
        private System.Windows.Forms.RadioButton Rb_AllEtapa;
        private System.Windows.Forms.RadioButton Rb_Etapa;
        private System.ComponentModel.BackgroundWorker workerReporte;
        private System.Windows.Forms.GroupBox groupBoxClasificacion;
        private System.Windows.Forms.RadioButton rb_ClasifProspectos;
        private System.Windows.Forms.RadioButton Rb_AllClasific;
        private System.Windows.Forms.RadioButton Rb_ClasifClientes;
    }
}