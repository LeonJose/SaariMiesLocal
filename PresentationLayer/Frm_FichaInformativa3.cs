using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.ComponentLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using System.Net.Mail;

namespace GestorReportes.PresentationLayer
{
    public partial class Frm_FichaInformativa3 : Form
    {
        string asunto = string.Empty;
        string cuerpo = string.Empty;
        string[] destinatarios;
        bool terminado = false;
        bool enviado = false;
        Formatos formato = Formatos.Ninguno;
        FichaInformativa _fichaInformativa = new FichaInformativa();
        Inmobiliaria inmobiliaria = new Inmobiliaria();
        FlowLayoutPanel flowLayout = new FlowLayoutPanel();
        FlowLayoutPanel flowLayout2 = new FlowLayoutPanel();
        List<PictureBoxRT> pictureBxList = new List<PictureBoxRT>();
        List<Label> lablList = new List<Label>();
        int contadorIdsPic = 0;
        public Frm_FichaInformativa3(FichaInformativa fichaInformativa)
        {
            InitializeComponent();
            //SaariIcon.setSaariIcon(Properties.Settings.Default.SaariIcon, this);
            SaariIcon.SaariIcon.setSaariIcon(this);
            _fichaInformativa = fichaInformativa;
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void bnBack_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Frm_FichaInformativa2 frm = new Frm_FichaInformativa2(_fichaInformativa);
            frm.Show();
        }

        private void Frm_FichaInformativa3_Load(object sender, EventArgs e)
        {            
            splitContainer1.Panel1.Controls.Add(flowLayout);
            flowLayout.Dock = DockStyle.Fill;
            //flowLayout.BackColor = System.Drawing.Color.Aqua;
            flowLayout.AutoScroll = true;

            
            splitContainer1.Panel2.Controls.Add(flowLayout2);
            flowLayout2.Dock = DockStyle.Fill;
            //flowLayout2.BackColor = System.Drawing.Color.Coral;
            flowLayout2.AutoScroll = true;

            DataTable dtImagenes = new DataTable();
            bool incluirConjunto = false;
            try
            {
                incluirConjunto = _fichaInformativa.Datos[22].Contains("Incluir Conjunto");
            }
            catch (Exception ex)
            {
                incluirConjunto = false;
            }

            if (_fichaInformativa.Inmueble == "Todos")
                dtImagenes = inmobiliaria.getImagenesConjuntos(_fichaInformativa.GrupoEmpresarial, _fichaInformativa.Inmobiliaria, _fichaInformativa.Conjunto);
            else
            {
                if(!incluirConjunto)
                    dtImagenes = inmobiliaria.getImagenesInmuebles(_fichaInformativa.Inmueble);
                else
                    dtImagenes = inmobiliaria.getImagenesInmuebles(_fichaInformativa.Inmueble,_fichaInformativa.Conjunto);
            }
            if (dtImagenes.Rows.Count > 0)
            {
                //int index = 0;
                foreach (DataRow row in dtImagenes.Rows)
                {
                    if (row["CAMPO1"].ToString().ToLower().Contains(".png") || row["CAMPO1"].ToString().ToLower().Contains(".jpg") || row["CAMPO1"].ToString().ToLower().Contains(".bmp"))
                    {
                        if (System.IO.File.Exists(row["CAMPO1"].ToString()))
                        {
                            agregarImagen(row["CAMPO1"].ToString(), row["P0906_TEXTO_ESP"].ToString());
                            chckLstBxImgs.Items.Add(row["CAMPO1"].ToString(), false);
                        }
                    }
                }
                mostrarImagen();
            }
            else
            {
                DialogResult sinImagen = MessageBox.Show("No se encontraron imágenes para los criterios seleccionados. \n ¿Desea que se genere la ficha informativa sin imágenes?",
                    "Ficha informativa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (sinImagen == DialogResult.No)
                {
                    System.Environment.Exit(0);
                }
            }
            
        }

        private void lblEvent_Click(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            Panel panelArg = (Panel)lbl.Parent;
            if (flowLayout.Contains(panelArg))
            {
                flowLayout.Controls.Remove(panelArg);
                flowLayout2.Controls.Add(panelArg);
            }
            else if (flowLayout2.Contains(panelArg))
            {
                flowLayout.Controls.Remove(panelArg);
                flowLayout.Controls.Add(panelArg);
            }
        }

        private void pictureBx_Click(object sender, EventArgs e)
        {
            PictureBoxRT pic = (PictureBoxRT)sender;
            Panel panelArg = (Panel)pic.Parent;
            if (flowLayout.Contains(panelArg))
            {
                flowLayout.Controls.Remove(panelArg);
                flowLayout2.Controls.Add(panelArg);
            }
            else if (flowLayout2.Contains(panelArg))
            {
                flowLayout.Controls.Remove(panelArg);
                flowLayout.Controls.Add(panelArg);
            }
            /*PictureBoxRT picRtNew = (PictureBoxRT)sender;

            if (flowLayout.Contains(picRtNew))
            {
                flowLayout.Controls.Remove(picRtNew);
                flowLayout2.Controls.Add(picRtNew);
            }
            else if (flowLayout2.Contains(picRtNew))
            {
                flowLayout.Controls.Remove(picRtNew);
                flowLayout.Controls.Add(picRtNew);
            }*/
        }

        private void panel_Click(object sender, EventArgs e)
        {
            Panel panelArg = (Panel)sender;
            if (flowLayout.Contains(panelArg))
            {
                flowLayout.Controls.Remove(panelArg);
                flowLayout2.Controls.Add(panelArg);
            }
            else if (flowLayout2.Contains(panelArg))
            {
                flowLayout.Controls.Remove(panelArg);
                flowLayout.Controls.Add(panelArg);
            }
        }

        private void bnGenerateWord_Click(object sender, EventArgs e)
        {
            if (confirmarSinImagen())
            {                
                inhabilitarControles();
                obtenerDatos();
                formato = Formatos.Word;
                BackgroundWorker worker = new BackgroundWorker();
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.WorkerReportsProgress = true;
                worker.RunWorkerAsync();
                /*if (_fichaInformativa.Conjunto != "Todos")
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                    worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                    worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                    worker.WorkerReportsProgress = true;
                    worker.RunWorkerAsync(_fichaInformativa);
                    //_fichaInformativa.generarWord();                    
                }
                else
                {
                    DataTable dtIdsConj = inmobiliaria.getIdsConjuntos(_fichaInformativa.GrupoEmpresarial, _fichaInformativa.Inmobiliaria, _fichaInformativa.Conjunto);
                    if (dtIdsConj.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtIdsConj.Rows)
                        {
                            _fichaInformativa.Conjunto = row["P0301_ID_CENTRO"].ToString();
                            BackgroundWorker worker = new BackgroundWorker();
                            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                            worker.WorkerReportsProgress = true;
                            worker.RunWorkerAsync(_fichaInformativa);
                            //_fichaInformativa.generarWord();
                        }
                    }
                }*/
                terminado = true;
                //System.Environment.Exit(1);
                /*Frm_FichaInformativa ficha = new Frm_FichaInformativa(_fichaInformativa.Usuario);
                ficha.Show();
                this.Hide();*/
            }
        }

        private void agregarImagen(string path, string nombreImg)
        {
            PictureBoxRT pctBx = new PictureBoxRT();
            System.Drawing.Bitmap bit = new Bitmap(path);
            pctBx.Name = "pictureBox" + contadorIdsPic;
            pctBx.Image = bit;
            pctBx.Size = new Size(110, 110);
            pctBx.SizeMode = PictureBoxSizeMode.StretchImage;
            pctBx.ID = contadorIdsPic;
            pctBx.Ruta = path;
            pctBx.NombreImagen = nombreImg;
            pctBx.Click += new EventHandler(pictureBx_Click);

            Label lblName = new Label();
            lblName.Width = 100;
            lblName.Text = pctBx.NombreImagen;
            lblName.Location = new Point(120, pctBx.Location.Y);
            lblName.Click += new EventHandler(lblEvent_Click);
            
            
            pictureBxList.Add(pctBx);
            lablList.Add(lblName);
            contadorIdsPic++;
        }

        private void mostrarImagen()
        {
            for(int i=0; i<pictureBxList.Count; i++)
            {
                Panel panel = new Panel();
                panel.Width = splitContainer1.Panel1.Width;
                panel.Controls.Add(pictureBxList[i]);
                panel.Controls.Add(lablList[i]);
                panel.Click += new EventHandler(panel_Click);
                flowLayout.Controls.Add(panel);
            }
            /*
            if (pictureBxList.Count > 0)
            {
                foreach (var pc in pictureBxList)
                {
                    flowLayout.Controls.Add(pc);
                }

                /*foreach (var s in lablList)
                {
                    flowLayout.Controls.Add(s);
                }
            }*/
        }

        bool confirmarSinImagen()
        {/*
            if (chckLstBxImgs.CheckedItems.Count < 1)
            {
                DialogResult sinImagen = MessageBox.Show("No se seleccionaron imágenes. \n ¿Desea que se genere la ficha informativa sin imágenes?",
                        "Ficha informativa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (sinImagen == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            else
                return true;*/

            if (flowLayout2.Controls.Count < 1)
            {
                DialogResult sinImagen = MessageBox.Show("No se seleccionaron imágenes. \n ¿Desea que se genere la ficha informativa sin imágenes?",
                        "Ficha informativa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (sinImagen == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            else
                return true;

        }

        void obtenerDatos()
        {
            string[] imagenes = new string[flowLayout2.Controls.Count];
            int cont = 0;
            foreach (Panel itm in flowLayout2.Controls)
            {
                for (int i = 0; i < itm.Controls.Count; i++)
                {
                    if (itm.Controls[i] is PictureBoxRT)
                    {
                        PictureBoxRT picRT = (PictureBoxRT)itm.Controls[i];
                        imagenes[cont] = picRT.Ruta;
                    }
                }
                cont++;
            }
            _fichaInformativa.Imagenes = imagenes;
            _fichaInformativa.Horizontal = rdioBnHor.Checked;
            _fichaInformativa.UnaPagina = rdBn1Pag.Checked;
            _fichaInformativa.fotosMismaPag = radioFotosMismaHoja.Checked;
        }

        private void bnGeneratePDF_Click(object sender, EventArgs e)
        {
            if (confirmarSinImagen())
            {
                inhabilitarControles();
                obtenerDatos();
                formato = Formatos.PDF;
                BackgroundWorker worker = new BackgroundWorker();
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.WorkerReportsProgress = true;
                worker.RunWorkerAsync();
                terminado = true;
                /*if (_fichaInformativa.Conjunto != "Todos")
                {
                    _fichaInformativa.generarPDF();
                }
                else
                {
                    DataTable dtIdsConj = inmobiliaria.getIdsConjuntos(_fichaInformativa.GrupoEmpresarial, _fichaInformativa.Inmobiliaria, _fichaInformativa.Conjunto);
                    if (dtIdsConj.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtIdsConj.Rows)
                        {
                            _fichaInformativa.Conjunto = row["P0301_ID_CENTRO"].ToString();
                            _fichaInformativa.generarPDF();
                        }
                    }
                }
                //System.Environment.Exit(1);
                Frm_FichaInformativa ficha = new Frm_FichaInformativa(_fichaInformativa.Usuario);
                ficha.Show();
                this.Hide();*/
            }
        }

        private void bnSendMail_Click(object sender, EventArgs e)
        {
            if (confirmarSinImagen())
            {
                Frm_Mail paramMail = new Frm_Mail();
                DialogResult dialogParamMail = paramMail.ShowDialog();
                if (dialogParamMail == System.Windows.Forms.DialogResult.OK)
                {
                    inhabilitarControles();
                    DataTable dtUserGpo = inmobiliaria.getDtUsuario(_fichaInformativa.Usuario);
                    if (dtUserGpo.Rows.Count > 0)
                    {
                        inhabilitarControles();
                        formato = Formatos.EMail;
                        List<String> listaArchivosAAdjuntar = new List<String>();
                        obtenerDatos();
                        if (paramMail.Destinatarios.Contains(";"))
                            destinatarios = paramMail.Destinatarios.Split(';');
                        else if (paramMail.Destinatarios.Contains(","))
                            destinatarios = paramMail.Destinatarios.Split(',');
                        else
                        {
                            destinatarios = new string[1];
                            destinatarios[0] = paramMail.Destinatarios;
                        }
                        asunto = paramMail.Asunto;
                        cuerpo = paramMail.Cuerpo;
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                        worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                        worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                        worker.WorkerReportsProgress = true;
                        worker.RunWorkerAsync(dtUserGpo);
                        terminado = true;
                        /*if (_fichaInformativa.Conjunto != "Todos")
                        {
                            listaArchivosAAdjuntar.Add(_fichaInformativa.generarPDF(false));
                        }
                        else
                        {
                            DataTable dtIdsConj = inmobiliaria.getIdsConjuntos(_fichaInformativa.GrupoEmpresarial, _fichaInformativa.Inmobiliaria, _fichaInformativa.Conjunto);
                            if (dtIdsConj.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtIdsConj.Rows)
                                {
                                    _fichaInformativa.Conjunto = row["P0301_ID_CENTRO"].ToString();
                                    listaArchivosAAdjuntar.Add(_fichaInformativa.generarPDF(false));
                                }
                            }
                        }
                        string remitente = dtUserGpo.Rows[0]["CAMPO2"].ToString();
                        string userRemitente = dtUserGpo.Rows[0]["CAMPO1"].ToString();
                        string passMail = dtUserGpo.Rows[0]["CAMPO3"].ToString();
                        int puerto = (int)Convert.ToDecimal(dtUserGpo.Rows[0]["CAMPO_NUM1"].ToString());
                        string host = dtUserGpo.Rows[0]["CAMPO4"].ToString();
                        string[] destinatarios;
                        if (paramMail.Destinatarios.Contains(";"))
                            destinatarios = paramMail.Destinatarios.Split(';');
                        else if (paramMail.Destinatarios.Contains(","))
                            destinatarios = paramMail.Destinatarios.Split(',');
                        else
                        {
                            destinatarios = new string[1];
                            destinatarios[0] = paramMail.Destinatarios;
                        }
                        MailMessage email = new MailMessage();
                        foreach(string dest in destinatarios)
                            email.To.Add(dest);
                        email.From = new MailAddress(remitente, userRemitente, System.Text.Encoding.UTF8);
                        email.Body = paramMail.Cuerpo + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                        email.BodyEncoding = System.Text.Encoding.UTF8;
                        email.IsBodyHtml = false;
                        email.Subject = paramMail.Asunto;
                        email.SubjectEncoding = System.Text.Encoding.UTF8;
                        Attachment adjunto;
                        foreach (var archivo in listaArchivosAAdjuntar)
                        {
                            adjunto = new Attachment(archivo);
                            email.Attachments.Add(adjunto);
                        }
                        SmtpClient smtp = new SmtpClient();
                        smtp.Credentials = new System.Net.NetworkCredential(remitente, passMail);
                        smtp.Port = puerto;
                        smtp.Host = host;
                        try
                        {
                            smtp.Send(email);
                            MessageBox.Show("Se envío correctamente el e-mail con la(s) ficha(s) tecnica(s) adjunta(s).", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch
                        {
                            MessageBox.Show("No se pudo realizar el envío de ficha(s) técnica(s). \n - Se generaron los archivos pero no se enviaron. \n - Puede enviarlos manualmente, o bien, intentar de nuevo.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            System.Environment.Exit(0);
                        }
                        //System.Environment.Exit(1);
                        Frm_FichaInformativa ficha = new Frm_FichaInformativa(_fichaInformativa.Usuario);
                        ficha.Show();
                        this.Hide();*/
                    }
                    else
                    {
                        MessageBox.Show("No se encontró cuenta de correo asociada. \n - Debe tener una cuenta de correo configurada. \n - Revise su conexión a la base de datos. \n - Revise el usuario en el archivo de configuración. \n - Revise la configuración de correo del usuario", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        System.Environment.Exit(0);
                    }
                }
            }
        }

        private void btnGenPP_Click(object sender, EventArgs e)
        {
            if (confirmarSinImagen())
            {
                inhabilitarControles();
                obtenerDatos();
                formato = Formatos.PowerPoint;
                BackgroundWorker worker = new BackgroundWorker();
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.WorkerReportsProgress = true;
                worker.RunWorkerAsync();
                terminado = true;
                /*if (_fichaInformativa.Conjunto != "Todos")
                {
                    _fichaInformativa.generarPowerPoint();
                }
                else
                {
                    DataTable dtIdsConj = inmobiliaria.getIdsConjuntos(_fichaInformativa.GrupoEmpresarial, _fichaInformativa.Inmobiliaria, _fichaInformativa.Conjunto);
                    if (dtIdsConj.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtIdsConj.Rows)
                        {
                            _fichaInformativa.Conjunto = row["P0301_ID_CENTRO"].ToString();
                            _fichaInformativa.generarPowerPoint();
                        }
                    }
                }*/
                //System.Environment.Exit(1);
                /*Frm_FichaInformativa ficha = new Frm_FichaInformativa(_fichaInformativa.Usuario);
                ficha.Show();
                this.Hide();*/
            }
        }

        private void Frm_FichaInformativa3_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void inhabilitarControles()
        {
            gpBxEspacio.Enabled = false;
            gpBxImgs.Enabled = false;
            gpBxOri.Enabled = false;
            botonGenerarPowerPoint.Enabled = false;
            botonAtras.Enabled = false;
            botonCancelar.Enabled = false;
            botonGenerarPdf.Enabled = false;
            botonGenerarWord.Enabled = false;
            botonEnviarCorreo.Enabled = false;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (formato == Formatos.Word)
            {
                if (_fichaInformativa.Conjunto != "Todos")
                    _fichaInformativa.generarWord(sender as BackgroundWorker);
                else
                {
                    DataTable dtIdsConj = inmobiliaria.getIdsConjuntos(_fichaInformativa.GrupoEmpresarial, _fichaInformativa.Inmobiliaria, _fichaInformativa.Conjunto);
                    if (dtIdsConj.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtIdsConj.Rows)
                        {
                            _fichaInformativa.Conjunto = row["P0301_ID_CENTRO"].ToString();
                            _fichaInformativa.generarWord(sender as BackgroundWorker);
                        }
                    }
                }
            }
            else if (formato == Formatos.PDF)
            {
                if (_fichaInformativa.Conjunto != "Todos")
                    _fichaInformativa.generarPDF(sender as BackgroundWorker);
                else
                {
                    DataTable dtIdsConj = inmobiliaria.getIdsConjuntos(_fichaInformativa.GrupoEmpresarial, _fichaInformativa.Inmobiliaria, _fichaInformativa.Conjunto);
                    DataTable dtDatosConjuntos = inmobiliaria.getDatosConjuntos(_fichaInformativa.GrupoEmpresarial, _fichaInformativa.Inmobiliaria, _fichaInformativa.Conjunto);

                    if (dtIdsConj.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtIdsConj.Rows)
                        {
                            _fichaInformativa.Conjunto = row["P0301_ID_CENTRO"].ToString();
                            
                            _fichaInformativa.generarPDF(sender as BackgroundWorker);
                        }
                    }
                }
            }
            else if (formato == Formatos.PowerPoint)
            {
                if (_fichaInformativa.Conjunto != "Todos")
                    _fichaInformativa.generarPowerPoint(sender as BackgroundWorker);
                else
                {
                    DataTable dtIdsConj = inmobiliaria.getIdsConjuntos(_fichaInformativa.GrupoEmpresarial, _fichaInformativa.Inmobiliaria, _fichaInformativa.Conjunto);
                    if (dtIdsConj.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtIdsConj.Rows)
                        {
                            _fichaInformativa.Conjunto = row["P0301_ID_CENTRO"].ToString();
                            _fichaInformativa.generarPowerPoint(sender as BackgroundWorker);
                        }
                    }
                }
            }
            else if (formato == Formatos.EMail)
            {
                List<String> listaArchivosAAdjuntar = new List<String>();
                if (_fichaInformativa.Conjunto != "Todos")
                    listaArchivosAAdjuntar.Add(_fichaInformativa.generarPDF(false, sender as BackgroundWorker)); 
                else
                {
                    DataTable dtIdsConj = inmobiliaria.getIdsConjuntos(_fichaInformativa.GrupoEmpresarial, _fichaInformativa.Inmobiliaria, _fichaInformativa.Conjunto);
                    if (dtIdsConj.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtIdsConj.Rows)
                        {
                            _fichaInformativa.Conjunto = row["P0301_ID_CENTRO"].ToString();
                            listaArchivosAAdjuntar.Add(_fichaInformativa.generarPDF(false, sender as BackgroundWorker)); 
                        }
                    }
                }
                (sender as BackgroundWorker).ReportProgress(0);
                DataTable dtUserGpo = e.Argument as DataTable;
                string remitente = dtUserGpo.Rows[0]["CAMPO2"].ToString();
                string userRemitente = dtUserGpo.Rows[0]["CAMPO1"].ToString();
                string passMail = dtUserGpo.Rows[0]["CAMPO3"].ToString();
                int puerto = (int)Convert.ToDecimal(dtUserGpo.Rows[0]["CAMPO_NUM1"].ToString());
                string host = dtUserGpo.Rows[0]["CAMPO4"].ToString();
                (sender as BackgroundWorker).ReportProgress(20);
                MailMessage email = new MailMessage();
                foreach (string dest in destinatarios)
                    email.To.Add(dest);
                email.From = new MailAddress(remitente, userRemitente, System.Text.Encoding.UTF8);
                email.Body = cuerpo + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                email.BodyEncoding = System.Text.Encoding.UTF8;
                email.IsBodyHtml = false;
                email.Subject = asunto;
                email.SubjectEncoding = System.Text.Encoding.UTF8;
                (sender as BackgroundWorker).ReportProgress(40);
                Attachment adjunto;
                foreach (var archivo in listaArchivosAAdjuntar)
                {
                    adjunto = new Attachment(archivo);
                    email.Attachments.Add(adjunto);
                }
                (sender as BackgroundWorker).ReportProgress(60);
                SmtpClient smtp = new SmtpClient();
                smtp.Credentials = new System.Net.NetworkCredential(remitente, passMail);
                smtp.Port = puerto;
                smtp.Host = host;
                (sender as BackgroundWorker).ReportProgress(80);
                try
                {
                    smtp.Send(email);
                    enviado = true;
                    //MessageBox.Show("Se envío correctamente el e-mail con la(s) ficha(s) tecnica(s) adjunta(s).", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    enviado = false;
                    //MessageBox.Show("No se pudo realizar el envío de ficha(s) técnica(s). \n - Se generaron los archivos pero no se enviaron. \n - Puede enviarlos manualmente, o bien, intentar de nuevo.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //System.Environment.Exit(0);
                }
                (sender as BackgroundWorker).ReportProgress(100);
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (terminado)
            {                
                terminado = false;
                if (formato == Formatos.EMail)
                {
                    if(enviado)
                        MessageBox.Show("Se envío correctamente el e-mail con la(s) ficha(s) tecnica(s) adjunta(s).", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("No se pudo realizar el envío de ficha(s) técnica(s). \n - Se generaron los archivos pero no se enviaron. \n - Puede enviarlos manualmente, o bien, intentar de nuevo.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Frm_FichaInformativa ficha = new Frm_FichaInformativa(_fichaInformativa.Usuario);
                ficha.Show();
                this.Hide();
            }
        }

        private void botonGenerarPdf_Load(object sender, EventArgs e)
        {

        }
    }
}
