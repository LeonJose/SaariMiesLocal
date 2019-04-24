namespace GestorReportes.PresentationLayer
{
    partial class Frm_GenerarPoliza
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
            this.lblInstrucciones = new System.Windows.Forms.Label();
            this.cmbBxInmob = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblFechaInicio = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.lblFechaFin = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioContpaqXls = new System.Windows.Forms.RadioButton();
            this.radioAxapta = new System.Windows.Forms.RadioButton();
            this.rdBtnContavision = new System.Windows.Forms.RadioButton();
            this.rdBtnAspel = new System.Windows.Forms.RadioButton();
            this.rdBtnExc = new System.Windows.Forms.RadioButton();
            this.rdBtnContpaq = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdBtnNo = new System.Windows.Forms.RadioButton();
            this.rdBtnSi = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioGlobal = new System.Windows.Forms.RadioButton();
            this.rdBtnDetallado = new System.Windows.Forms.RadioButton();
            this.rdBtnConsolidado = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdBtnNotasCredito = new System.Windows.Forms.RadioButton();
            this.rdBtnCancelados = new System.Windows.Forms.RadioButton();
            this.rdBtnIngreso = new System.Windows.Forms.RadioButton();
            this.rdBtnDiario = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBxIdenPol = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBxConceptoPoliza = new System.Windows.Forms.TextBox();
            this.checkUUID = new System.Windows.Forms.CheckBox();
            this.checkMascara = new System.Windows.Forms.CheckBox();
            this.textoMascara = new System.Windows.Forms.TextBox();
            this.checkMultiples = new System.Windows.Forms.CheckBox();
            this.checkSegmento = new System.Windows.Forms.CheckBox();
            this.checkCliente = new System.Windows.Forms.CheckBox();
            this.checkAfectar = new System.Windows.Forms.CheckBox();
            this.checkConceptoPrincipal = new System.Windows.Forms.CheckBox();
            this.checkReferencia = new System.Windows.Forms.CheckBox();
            this.grupoConcepMov = new System.Windows.Forms.GroupBox();
            this.radioPeriodo = new System.Windows.Forms.RadioButton();
            this.radioCliente = new System.Windows.Forms.RadioButton();
            this.checkExcluir = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboSucursales = new System.Windows.Forms.ComboBox();
            this.progreso = new System.Windows.Forms.ProgressBar();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.groupBoxDecimales = new System.Windows.Forms.GroupBox();
            this.radioButton4Dec = new System.Windows.Forms.RadioButton();
            this.radioButton2Dec = new System.Windows.Forms.RadioButton();
            this.checkDiariosEspeciales = new System.Windows.Forms.CheckBox();
            this.checkBoxGuionEnMascara = new System.Windows.Forms.CheckBox();
            this.botonGenerar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.botonCancelar = new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion();
            this.checkRfcUuid = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.grupoConcepMov.SuspendLayout();
            this.groupBoxDecimales.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInstrucciones
            // 
            this.lblInstrucciones.Location = new System.Drawing.Point(12, 9);
            this.lblInstrucciones.Name = "lblInstrucciones";
            this.lblInstrucciones.Size = new System.Drawing.Size(570, 27);
            this.lblInstrucciones.TabIndex = 6;
            this.lblInstrucciones.Text = "Elija la inmobiliaria y las demas condiciones para las cuales desea generar la pó" +
    "liza. Para generar haga clic en Generar.";
            // 
            // cmbBxInmob
            // 
            this.cmbBxInmob.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxInmob.DropDownWidth = 364;
            this.cmbBxInmob.FormattingEnabled = true;
            this.cmbBxInmob.Location = new System.Drawing.Point(80, 29);
            this.cmbBxInmob.Name = "cmbBxInmob";
            this.cmbBxInmob.Size = new System.Drawing.Size(502, 21);
            this.cmbBxInmob.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Inmobiliaria:";
            // 
            // lblFechaInicio
            // 
            this.lblFechaInicio.AutoSize = true;
            this.lblFechaInicio.Location = new System.Drawing.Point(12, 89);
            this.lblFechaInicio.Name = "lblFechaInicio";
            this.lblFechaInicio.Size = new System.Drawing.Size(40, 13);
            this.lblFechaInicio.TabIndex = 9;
            this.lblFechaInicio.Text = "Fecha:";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(80, 83);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 10;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(382, 83);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker2.TabIndex = 12;
            this.dateTimePicker2.Visible = false;
            // 
            // lblFechaFin
            // 
            this.lblFechaFin.AutoSize = true;
            this.lblFechaFin.Location = new System.Drawing.Point(311, 89);
            this.lblFechaFin.Name = "lblFechaFin";
            this.lblFechaFin.Size = new System.Drawing.Size(62, 13);
            this.lblFechaFin.TabIndex = 11;
            this.lblFechaFin.Text = "Fecha final:";
            this.lblFechaFin.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioContpaqXls);
            this.groupBox1.Controls.Add(this.radioAxapta);
            this.groupBox1.Controls.Add(this.rdBtnContavision);
            this.groupBox1.Controls.Add(this.rdBtnAspel);
            this.groupBox1.Controls.Add(this.rdBtnExc);
            this.groupBox1.Controls.Add(this.rdBtnContpaq);
            this.groupBox1.Location = new System.Drawing.Point(15, 137);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(169, 106);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Exportar a:";
            // 
            // radioContpaqXls
            // 
            this.radioContpaqXls.AutoSize = true;
            this.radioContpaqXls.Checked = true;
            this.radioContpaqXls.Location = new System.Drawing.Point(6, 80);
            this.radioContpaqXls.Name = "radioContpaqXls";
            this.radioContpaqXls.Size = new System.Drawing.Size(83, 17);
            this.radioContpaqXls.TabIndex = 5;
            this.radioContpaqXls.TabStop = true;
            this.radioContpaqXls.Text = "Contpaq .xls";
            this.radioContpaqXls.UseVisualStyleBackColor = true;
            this.radioContpaqXls.CheckedChanged += new System.EventHandler(this.radioContpaqXls_CheckedChanged);
            // 
            // radioAxapta
            // 
            this.radioAxapta.AutoSize = true;
            this.radioAxapta.Location = new System.Drawing.Point(95, 51);
            this.radioAxapta.Name = "radioAxapta";
            this.radioAxapta.Size = new System.Drawing.Size(58, 17);
            this.radioAxapta.TabIndex = 4;
            this.radioAxapta.Text = "Axapta";
            this.radioAxapta.UseVisualStyleBackColor = true;
            this.radioAxapta.CheckedChanged += new System.EventHandler(this.radioAxapta_CheckedChanged);
            // 
            // rdBtnContavision
            // 
            this.rdBtnContavision.AutoSize = true;
            this.rdBtnContavision.Location = new System.Drawing.Point(6, 51);
            this.rdBtnContavision.Name = "rdBtnContavision";
            this.rdBtnContavision.Size = new System.Drawing.Size(80, 17);
            this.rdBtnContavision.TabIndex = 2;
            this.rdBtnContavision.Text = "Contavision";
            this.rdBtnContavision.UseVisualStyleBackColor = true;
            // 
            // rdBtnAspel
            // 
            this.rdBtnAspel.AutoSize = true;
            this.rdBtnAspel.Location = new System.Drawing.Point(94, 19);
            this.rdBtnAspel.Name = "rdBtnAspel";
            this.rdBtnAspel.Size = new System.Drawing.Size(72, 17);
            this.rdBtnAspel.TabIndex = 1;
            this.rdBtnAspel.Text = "Aspel-COI";
            this.rdBtnAspel.UseVisualStyleBackColor = true;
            this.rdBtnAspel.CheckedChanged += new System.EventHandler(this.rdBtnAspel_CheckedChanged);
            // 
            // rdBtnExc
            // 
            this.rdBtnExc.AutoSize = true;
            this.rdBtnExc.Location = new System.Drawing.Point(94, 80);
            this.rdBtnExc.Name = "rdBtnExc";
            this.rdBtnExc.Size = new System.Drawing.Size(51, 17);
            this.rdBtnExc.TabIndex = 3;
            this.rdBtnExc.Text = "Excel";
            this.rdBtnExc.UseVisualStyleBackColor = true;
            this.rdBtnExc.CheckedChanged += new System.EventHandler(this.rdBtnExc_CheckedChanged);
            // 
            // rdBtnContpaq
            // 
            this.rdBtnContpaq.AutoSize = true;
            this.rdBtnContpaq.Location = new System.Drawing.Point(6, 19);
            this.rdBtnContpaq.Name = "rdBtnContpaq";
            this.rdBtnContpaq.Size = new System.Drawing.Size(82, 17);
            this.rdBtnContpaq.TabIndex = 0;
            this.rdBtnContpaq.Text = "Contpaq .txt";
            this.rdBtnContpaq.UseVisualStyleBackColor = true;
            this.rdBtnContpaq.CheckedChanged += new System.EventHandler(this.rdBtnContpaq_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdBtnNo);
            this.groupBox2.Controls.Add(this.rdBtnSi);
            this.groupBox2.Location = new System.Drawing.Point(362, 137);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(220, 36);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "¿Incluir recibos cancelados?";
            // 
            // rdBtnNo
            // 
            this.rdBtnNo.AutoSize = true;
            this.rdBtnNo.Location = new System.Drawing.Point(75, 15);
            this.rdBtnNo.Name = "rdBtnNo";
            this.rdBtnNo.Size = new System.Drawing.Size(39, 17);
            this.rdBtnNo.TabIndex = 1;
            this.rdBtnNo.Text = "No";
            this.rdBtnNo.UseVisualStyleBackColor = true;
            // 
            // rdBtnSi
            // 
            this.rdBtnSi.AutoSize = true;
            this.rdBtnSi.Checked = true;
            this.rdBtnSi.Location = new System.Drawing.Point(6, 15);
            this.rdBtnSi.Name = "rdBtnSi";
            this.rdBtnSi.Size = new System.Drawing.Size(34, 17);
            this.rdBtnSi.TabIndex = 0;
            this.rdBtnSi.TabStop = true;
            this.rdBtnSi.Text = "Si";
            this.rdBtnSi.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioGlobal);
            this.groupBox3.Controls.Add(this.rdBtnDetallado);
            this.groupBox3.Controls.Add(this.rdBtnConsolidado);
            this.groupBox3.Location = new System.Drawing.Point(362, 179);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(220, 64);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Forma de presentación:";
            // 
            // radioGlobal
            // 
            this.radioGlobal.AutoSize = true;
            this.radioGlobal.Location = new System.Drawing.Point(6, 41);
            this.radioGlobal.Name = "radioGlobal";
            this.radioGlobal.Size = new System.Drawing.Size(55, 17);
            this.radioGlobal.TabIndex = 2;
            this.radioGlobal.Text = "Global";
            this.radioGlobal.UseVisualStyleBackColor = true;
            this.radioGlobal.CheckedChanged += new System.EventHandler(this.radioGlobal_CheckedChanged);
            // 
            // rdBtnDetallado
            // 
            this.rdBtnDetallado.AutoSize = true;
            this.rdBtnDetallado.Checked = true;
            this.rdBtnDetallado.Location = new System.Drawing.Point(6, 18);
            this.rdBtnDetallado.Name = "rdBtnDetallado";
            this.rdBtnDetallado.Size = new System.Drawing.Size(70, 17);
            this.rdBtnDetallado.TabIndex = 1;
            this.rdBtnDetallado.TabStop = true;
            this.rdBtnDetallado.Text = "Detallado";
            this.rdBtnDetallado.UseVisualStyleBackColor = true;
            // 
            // rdBtnConsolidado
            // 
            this.rdBtnConsolidado.AutoSize = true;
            this.rdBtnConsolidado.Location = new System.Drawing.Point(77, 18);
            this.rdBtnConsolidado.Name = "rdBtnConsolidado";
            this.rdBtnConsolidado.Size = new System.Drawing.Size(83, 17);
            this.rdBtnConsolidado.TabIndex = 0;
            this.rdBtnConsolidado.Text = "Consolidado";
            this.rdBtnConsolidado.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdBtnNotasCredito);
            this.groupBox4.Controls.Add(this.rdBtnCancelados);
            this.groupBox4.Controls.Add(this.rdBtnIngreso);
            this.groupBox4.Controls.Add(this.rdBtnDiario);
            this.groupBox4.Location = new System.Drawing.Point(190, 137);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(166, 106);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Tipo de póliza:";
            // 
            // rdBtnNotasCredito
            // 
            this.rdBtnNotasCredito.AutoSize = true;
            this.rdBtnNotasCredito.Location = new System.Drawing.Point(6, 80);
            this.rdBtnNotasCredito.Name = "rdBtnNotasCredito";
            this.rdBtnNotasCredito.Size = new System.Drawing.Size(103, 17);
            this.rdBtnNotasCredito.TabIndex = 3;
            this.rdBtnNotasCredito.TabStop = true;
            this.rdBtnNotasCredito.Text = "Notas de crédito";
            this.rdBtnNotasCredito.UseVisualStyleBackColor = true;
            // 
            // rdBtnCancelados
            // 
            this.rdBtnCancelados.AutoSize = true;
            this.rdBtnCancelados.Location = new System.Drawing.Point(6, 51);
            this.rdBtnCancelados.Name = "rdBtnCancelados";
            this.rdBtnCancelados.Size = new System.Drawing.Size(122, 17);
            this.rdBtnCancelados.TabIndex = 2;
            this.rdBtnCancelados.TabStop = true;
            this.rdBtnCancelados.Text = "Recibos cancelados";
            this.rdBtnCancelados.UseVisualStyleBackColor = true;
            this.rdBtnCancelados.CheckedChanged += new System.EventHandler(this.rdBtnCancelados_CheckedChanged);
            // 
            // rdBtnIngreso
            // 
            this.rdBtnIngreso.AutoSize = true;
            this.rdBtnIngreso.Location = new System.Drawing.Point(100, 19);
            this.rdBtnIngreso.Name = "rdBtnIngreso";
            this.rdBtnIngreso.Size = new System.Drawing.Size(60, 17);
            this.rdBtnIngreso.TabIndex = 1;
            this.rdBtnIngreso.TabStop = true;
            this.rdBtnIngreso.Text = "Ingreso";
            this.rdBtnIngreso.UseVisualStyleBackColor = true;
            // 
            // rdBtnDiario
            // 
            this.rdBtnDiario.AutoSize = true;
            this.rdBtnDiario.Checked = true;
            this.rdBtnDiario.Location = new System.Drawing.Point(6, 19);
            this.rdBtnDiario.Name = "rdBtnDiario";
            this.rdBtnDiario.Size = new System.Drawing.Size(52, 17);
            this.rdBtnDiario.TabIndex = 0;
            this.rdBtnDiario.TabStop = true;
            this.rdBtnDiario.Text = "Diario";
            this.rdBtnDiario.UseVisualStyleBackColor = true;
            this.rdBtnDiario.CheckedChanged += new System.EventHandler(this.rdBtnDiario_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Número de póliza:";
            // 
            // txtBxIdenPol
            // 
            this.txtBxIdenPol.Location = new System.Drawing.Point(110, 109);
            this.txtBxIdenPol.Name = "txtBxIdenPol";
            this.txtBxIdenPol.Size = new System.Drawing.Size(170, 20);
            this.txtBxIdenPol.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(311, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Concepto:";
            // 
            // txtBxConceptoPoliza
            // 
            this.txtBxConceptoPoliza.Location = new System.Drawing.Point(382, 109);
            this.txtBxConceptoPoliza.Name = "txtBxConceptoPoliza";
            this.txtBxConceptoPoliza.Size = new System.Drawing.Size(200, 20);
            this.txtBxConceptoPoliza.TabIndex = 22;
            // 
            // checkUUID
            // 
            this.checkUUID.AutoSize = true;
            this.checkUUID.Location = new System.Drawing.Point(15, 342);
            this.checkUUID.Name = "checkUUID";
            this.checkUUID.Size = new System.Drawing.Size(196, 17);
            this.checkUUID.TabIndex = 23;
            this.checkUUID.Text = "Incluir UUID de factura relacionada ";
            this.checkUUID.UseVisualStyleBackColor = true;
            // 
            // checkMascara
            // 
            this.checkMascara.AutoSize = true;
            this.checkMascara.Location = new System.Drawing.Point(15, 273);
            this.checkMascara.Name = "checkMascara";
            this.checkMascara.Size = new System.Drawing.Size(234, 17);
            this.checkMascara.TabIndex = 24;
            this.checkMascara.Text = "Enmascarar números de cuenta de la forma:";
            this.checkMascara.UseVisualStyleBackColor = true;
            this.checkMascara.CheckedChanged += new System.EventHandler(this.checkMascara_CheckedChanged);
            // 
            // textoMascara
            // 
            this.textoMascara.Enabled = false;
            this.textoMascara.Location = new System.Drawing.Point(255, 271);
            this.textoMascara.Name = "textoMascara";
            this.textoMascara.Size = new System.Drawing.Size(76, 20);
            this.textoMascara.TabIndex = 25;
            this.textoMascara.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textoMascara_KeyPress);
            // 
            // checkMultiples
            // 
            this.checkMultiples.AutoSize = true;
            this.checkMultiples.Location = new System.Drawing.Point(362, 271);
            this.checkMultiples.Name = "checkMultiples";
            this.checkMultiples.Size = new System.Drawing.Size(134, 17);
            this.checkMultiples.TabIndex = 26;
            this.checkMultiples.Text = "Multiples encabezados";
            this.checkMultiples.UseVisualStyleBackColor = true;
            this.checkMultiples.CheckedChanged += new System.EventHandler(this.checkMultiples_CheckedChanged);
            // 
            // checkSegmento
            // 
            this.checkSegmento.AutoSize = true;
            this.checkSegmento.Location = new System.Drawing.Point(362, 319);
            this.checkSegmento.Name = "checkSegmento";
            this.checkSegmento.Size = new System.Drawing.Size(159, 17);
            this.checkSegmento.TabIndex = 27;
            this.checkSegmento.Text = "Incluir segmento de negocio";
            this.checkSegmento.UseVisualStyleBackColor = true;
            // 
            // checkCliente
            // 
            this.checkCliente.AutoSize = true;
            this.checkCliente.Enabled = false;
            this.checkCliente.Location = new System.Drawing.Point(362, 295);
            this.checkCliente.Name = "checkCliente";
            this.checkCliente.Size = new System.Drawing.Size(220, 17);
            this.checkCliente.TabIndex = 28;
            this.checkCliente.Text = "Tomar nombre de cliente como concepto";
            this.checkCliente.UseVisualStyleBackColor = true;
            this.checkCliente.CheckedChanged += new System.EventHandler(this.checkCliente_CheckedChanged);
            // 
            // checkAfectar
            // 
            this.checkAfectar.AutoSize = true;
            this.checkAfectar.Location = new System.Drawing.Point(362, 342);
            this.checkAfectar.Name = "checkAfectar";
            this.checkAfectar.Size = new System.Drawing.Size(96, 17);
            this.checkAfectar.TabIndex = 29;
            this.checkAfectar.Text = "Afectar saldos ";
            this.checkAfectar.UseVisualStyleBackColor = true;
            // 
            // checkConceptoPrincipal
            // 
            this.checkConceptoPrincipal.AutoSize = true;
            this.checkConceptoPrincipal.Location = new System.Drawing.Point(15, 250);
            this.checkConceptoPrincipal.Name = "checkConceptoPrincipal";
            this.checkConceptoPrincipal.Size = new System.Drawing.Size(188, 17);
            this.checkConceptoPrincipal.TabIndex = 30;
            this.checkConceptoPrincipal.Text = "Usar siempre el concepto principal";
            this.checkConceptoPrincipal.UseVisualStyleBackColor = true;
            // 
            // checkReferencia
            // 
            this.checkReferencia.AutoSize = true;
            this.checkReferencia.Location = new System.Drawing.Point(15, 319);
            this.checkReferencia.Name = "checkReferencia";
            this.checkReferencia.Size = new System.Drawing.Size(171, 17);
            this.checkReferencia.TabIndex = 31;
            this.checkReferencia.Text = "Incluir periodo como referencia";
            this.checkReferencia.UseVisualStyleBackColor = true;
            // 
            // grupoConcepMov
            // 
            this.grupoConcepMov.Controls.Add(this.radioPeriodo);
            this.grupoConcepMov.Controls.Add(this.radioCliente);
            this.grupoConcepMov.Location = new System.Drawing.Point(15, 365);
            this.grupoConcepMov.Name = "grupoConcepMov";
            this.grupoConcepMov.Size = new System.Drawing.Size(159, 65);
            this.grupoConcepMov.TabIndex = 32;
            this.grupoConcepMov.TabStop = false;
            this.grupoConcepMov.Text = "Concepto de movimiento:";
            // 
            // radioPeriodo
            // 
            this.radioPeriodo.AutoSize = true;
            this.radioPeriodo.Checked = true;
            this.radioPeriodo.Location = new System.Drawing.Point(6, 19);
            this.radioPeriodo.Name = "radioPeriodo";
            this.radioPeriodo.Size = new System.Drawing.Size(143, 17);
            this.radioPeriodo.TabIndex = 1;
            this.radioPeriodo.TabStop = true;
            this.radioPeriodo.Text = "CFDi | Inmueble | Periodo";
            this.radioPeriodo.UseVisualStyleBackColor = true;
            // 
            // radioCliente
            // 
            this.radioCliente.AutoSize = true;
            this.radioCliente.Location = new System.Drawing.Point(6, 42);
            this.radioCliente.Name = "radioCliente";
            this.radioCliente.Size = new System.Drawing.Size(139, 17);
            this.radioCliente.TabIndex = 0;
            this.radioCliente.Text = "CFDi | Inmueble | Cliente";
            this.radioCliente.UseVisualStyleBackColor = true;
            // 
            // checkExcluir
            // 
            this.checkExcluir.AutoSize = true;
            this.checkExcluir.Location = new System.Drawing.Point(362, 250);
            this.checkExcluir.Name = "checkExcluir";
            this.checkExcluir.Size = new System.Drawing.Size(156, 17);
            this.checkExcluir.TabIndex = 33;
            this.checkExcluir.Text = "Excluir facturas sin contrato";
            this.checkExcluir.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "Sucursal:";
            // 
            // comboSucursales
            // 
            this.comboSucursales.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSucursales.DropDownWidth = 364;
            this.comboSucursales.FormattingEnabled = true;
            this.comboSucursales.Location = new System.Drawing.Point(80, 56);
            this.comboSucursales.Name = "comboSucursales";
            this.comboSucursales.Size = new System.Drawing.Size(502, 21);
            this.comboSucursales.TabIndex = 35;
            // 
            // progreso
            // 
            this.progreso.Location = new System.Drawing.Point(12, 447);
            this.progreso.Name = "progreso";
            this.progreso.Size = new System.Drawing.Size(341, 23);
            this.progreso.TabIndex = 38;
            // 
            // worker
            // 
            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            this.worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            // 
            // groupBoxDecimales
            // 
            this.groupBoxDecimales.Controls.Add(this.radioButton4Dec);
            this.groupBoxDecimales.Controls.Add(this.radioButton2Dec);
            this.groupBoxDecimales.Location = new System.Drawing.Point(191, 365);
            this.groupBoxDecimales.Name = "groupBoxDecimales";
            this.groupBoxDecimales.Size = new System.Drawing.Size(159, 65);
            this.groupBoxDecimales.TabIndex = 39;
            this.groupBoxDecimales.TabStop = false;
            this.groupBoxDecimales.Text = "Cantidad de Decimales";
            // 
            // radioButton4Dec
            // 
            this.radioButton4Dec.AutoSize = true;
            this.radioButton4Dec.Location = new System.Drawing.Point(10, 42);
            this.radioButton4Dec.Name = "radioButton4Dec";
            this.radioButton4Dec.Size = new System.Drawing.Size(83, 17);
            this.radioButton4Dec.TabIndex = 1;
            this.radioButton4Dec.Text = "4 Decimales";
            this.radioButton4Dec.UseVisualStyleBackColor = true;
            // 
            // radioButton2Dec
            // 
            this.radioButton2Dec.AutoSize = true;
            this.radioButton2Dec.Checked = true;
            this.radioButton2Dec.Location = new System.Drawing.Point(10, 19);
            this.radioButton2Dec.Name = "radioButton2Dec";
            this.radioButton2Dec.Size = new System.Drawing.Size(83, 17);
            this.radioButton2Dec.TabIndex = 0;
            this.radioButton2Dec.TabStop = true;
            this.radioButton2Dec.Text = "2 Decimales";
            this.radioButton2Dec.UseVisualStyleBackColor = true;
            // 
            // checkDiariosEspeciales
            // 
            this.checkDiariosEspeciales.AutoSize = true;
            this.checkDiariosEspeciales.Location = new System.Drawing.Point(362, 365);
            this.checkDiariosEspeciales.Name = "checkDiariosEspeciales";
            this.checkDiariosEspeciales.Size = new System.Drawing.Size(155, 17);
            this.checkDiariosEspeciales.TabIndex = 40;
            this.checkDiariosEspeciales.Text = "Diarios Especiales Contpaq";
            this.checkDiariosEspeciales.UseVisualStyleBackColor = true;
            // 
            // checkBoxGuionEnMascara
            // 
            this.checkBoxGuionEnMascara.AutoSize = true;
            this.checkBoxGuionEnMascara.Location = new System.Drawing.Point(15, 295);
            this.checkBoxGuionEnMascara.Name = "checkBoxGuionEnMascara";
            this.checkBoxGuionEnMascara.Size = new System.Drawing.Size(192, 17);
            this.checkBoxGuionEnMascara.TabIndex = 41;
            this.checkBoxGuionEnMascara.Text = "Incluir guión en máscara de cuenta";
            this.checkBoxGuionEnMascara.UseVisualStyleBackColor = true;
            // 
            // botonGenerar
            // 
            this.botonGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonGenerar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonGenerar.Imagen = global::GestorReportes.Properties.Resources.reportCh;
            this.botonGenerar.Location = new System.Drawing.Point(420, 395);
            this.botonGenerar.Name = "botonGenerar";
            this.botonGenerar.Size = new System.Drawing.Size(75, 75);
            this.botonGenerar.TabIndex = 37;
            this.botonGenerar.Texto = "Generar";
            this.botonGenerar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnGenerar_Click);
            // 
            // botonCancelar
            // 
            this.botonCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonCancelar.FondoColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.botonCancelar.Imagen = global::GestorReportes.Properties.Resources.cancelCh;
            this.botonCancelar.Location = new System.Drawing.Point(501, 395);
            this.botonCancelar.Name = "botonCancelar";
            this.botonCancelar.Size = new System.Drawing.Size(75, 75);
            this.botonCancelar.TabIndex = 36;
            this.botonCancelar.Texto = "Cancelar";
            this.botonCancelar.ControlClicked += new GestorReportes.PresentationLayer.Controls.Ctrl_Opcion.ControlClickedHandler(this.btnCancelar_Click);
            // 
            // checkRfcUuid
            // 
            this.checkRfcUuid.AutoSize = true;
            this.checkRfcUuid.Location = new System.Drawing.Point(14, 273);
            this.checkRfcUuid.Name = "checkRfcUuid";
            this.checkRfcUuid.Size = new System.Drawing.Size(79, 17);
            this.checkRfcUuid.TabIndex = 42;
            this.checkRfcUuid.Text = "RFC yUUD";
            this.checkRfcUuid.UseVisualStyleBackColor = true;
            this.checkRfcUuid.Visible = false;
            // 
            // Frm_GenerarPoliza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(584, 471);
            this.Controls.Add(this.checkRfcUuid);
            this.Controls.Add(this.checkBoxGuionEnMascara);
            this.Controls.Add(this.checkDiariosEspeciales);
            this.Controls.Add(this.groupBoxDecimales);
            this.Controls.Add(this.progreso);
            this.Controls.Add(this.botonGenerar);
            this.Controls.Add(this.botonCancelar);
            this.Controls.Add(this.comboSucursales);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkExcluir);
            this.Controls.Add(this.grupoConcepMov);
            this.Controls.Add(this.checkReferencia);
            this.Controls.Add(this.checkConceptoPrincipal);
            this.Controls.Add(this.checkAfectar);
            this.Controls.Add(this.checkCliente);
            this.Controls.Add(this.checkSegmento);
            this.Controls.Add(this.checkMultiples);
            this.Controls.Add(this.textoMascara);
            this.Controls.Add(this.checkMascara);
            this.Controls.Add(this.checkUUID);
            this.Controls.Add(this.txtBxConceptoPoliza);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtBxIdenPol);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.lblFechaFin);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.lblFechaInicio);
            this.Controls.Add(this.cmbBxInmob);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblInstrucciones);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(600, 510);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 510);
            this.Name = "Frm_GenerarPoliza";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generación de poliza";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_GenerarPoliza_FormClosing);
            this.Load += new System.EventHandler(this.Frm_GenerarPoliza_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.grupoConcepMov.ResumeLayout(false);
            this.grupoConcepMov.PerformLayout();
            this.groupBoxDecimales.ResumeLayout(false);
            this.groupBoxDecimales.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInstrucciones;
        private System.Windows.Forms.ComboBox cmbBxInmob;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblFechaInicio;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label lblFechaFin;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdBtnContpaq;
        private System.Windows.Forms.RadioButton rdBtnExc;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rdBtnNo;
        private System.Windows.Forms.RadioButton rdBtnSi;
        private System.Windows.Forms.RadioButton rdBtnDetallado;
        private System.Windows.Forms.RadioButton rdBtnConsolidado;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdBtnIngreso;
        private System.Windows.Forms.RadioButton rdBtnDiario;
        private System.Windows.Forms.RadioButton rdBtnCancelados;
        private System.Windows.Forms.RadioButton rdBtnNotasCredito;
        private System.Windows.Forms.RadioButton rdBtnContavision;
        private System.Windows.Forms.RadioButton rdBtnAspel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBxIdenPol;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBxConceptoPoliza;
        private System.Windows.Forms.RadioButton radioAxapta;
        private System.Windows.Forms.CheckBox checkUUID;
        private System.Windows.Forms.CheckBox checkMascara;
        private System.Windows.Forms.TextBox textoMascara;
        private System.Windows.Forms.CheckBox checkMultiples;
        private System.Windows.Forms.CheckBox checkSegmento;
        private System.Windows.Forms.CheckBox checkCliente;
        private System.Windows.Forms.CheckBox checkAfectar;
        private System.Windows.Forms.CheckBox checkConceptoPrincipal;
        private System.Windows.Forms.CheckBox checkReferencia;
        private System.Windows.Forms.GroupBox grupoConcepMov;
        private System.Windows.Forms.RadioButton radioPeriodo;
        private System.Windows.Forms.RadioButton radioCliente;
        private System.Windows.Forms.RadioButton radioContpaqXls;
        private System.Windows.Forms.CheckBox checkExcluir;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboSucursales;
        private System.Windows.Forms.RadioButton radioGlobal;
        private Controls.Ctrl_Opcion botonCancelar;
        private Controls.Ctrl_Opcion botonGenerar;
        private System.Windows.Forms.ProgressBar progreso;
        private System.ComponentModel.BackgroundWorker worker;
        private System.Windows.Forms.GroupBox groupBoxDecimales;
        private System.Windows.Forms.RadioButton radioButton4Dec;
        private System.Windows.Forms.RadioButton radioButton2Dec;
        private System.Windows.Forms.CheckBox checkDiariosEspeciales;
        private System.Windows.Forms.CheckBox checkBoxGuionEnMascara;
        private System.Windows.Forms.CheckBox checkRfcUuid;
    }
}