using System;
using System.Collections.Generic;
using System.Text;
using Word = Microsoft.Office.Interop.Word;
using System.Data.Odbc;
using System.Data;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Interop.Word;
using GestorReportes.BusinessLayer.DataAccessLayer;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class FichaInformativa
    {
        #region Variables 
        public string GrupoEmpresarial { get; set; }
        public string Inmobiliaria { get; set; }
        public string Conjunto { get; set; }
        public string[] Datos { get; set; }
        public string[] Imagenes { get; set; }
        public bool Horizontal { get; set; }
        public bool UnaPagina { get; set; }
        public string Usuario { get; set; }
        public string Inmueble { get; set; }
        public bool fotosMismaPag { get; set; }
        private string LogoArrendadora = string.Empty;
        private string propietario = string.Empty;
        private string identificador = string.Empty;
        private string descripcion = string.Empty;
        private string direccion = string.Empty;
        private string colonia = string.Empty;
        private string municipio = string.Empty;
        private string estado = string.Empty;
        private string terreno = string.Empty;
        private string construccion = string.Empty;
        private string terrenoConjunto = string.Empty;
        private string construccionConjunto = string.Empty;
        private string valorCatastral = string.Empty;
        private string valorComercial = string.Empty;
        private string descripcionGeneral = string.Empty;
        private string pais = string.Empty;
        private string numInterior = string.Empty;
        private string numExterior = string.Empty;
        private int codigoPostal = 0;
        private string uso = string.Empty;

        private string[] datosContrato;

        private bool esPDF = false;
        private string fileName = string.Empty;
        //private float anchoAnt = 320;
        private float altoAnt = 55;
        #endregion

        public void generarWord(System.ComponentModel.BackgroundWorker worker)
        {
            worker.ReportProgress(0);
            setDatos();
            worker.ReportProgress(10);

            Word.Application aplicacionWord = new Word.Application();
            Word.Document documentoWord = new Word.Document();
            Object oMissing = System.Reflection.Missing.Value;
            documentoWord = aplicacionWord.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            documentoWord.Activate();
          //  aplicacionWord.Visible = true;
            //PagConfig
            if (Horizontal)
                documentoWord.PageSetup.Orientation = Word.WdOrientation.wdOrientLandscape;
            else
                documentoWord.PageSetup.Orientation = Word.WdOrientation.wdOrientPortrait;
            Word.TextColumn textCol = documentoWord.PageSetup.TextColumns.Add();
            documentoWord.Paragraphs.Format.SpaceAfter = 0;
            documentoWord.PageSetup.BottomMargin = 60;
            documentoWord.PageSetup.TopMargin = 50;

            float pageWidth = documentoWord.PageSetup.PageWidth;
            float pageHeight = documentoWord.PageSetup.PageHeight;
            worker.ReportProgress(20);
            try
            {
                if (!string.IsNullOrEmpty(LogoArrendadora))
                {
                    aplicacionWord.Selection.TypeParagraph();
                    Word.InlineShape inlSh = aplicacionWord.Selection.InlineShapes.AddPicture(LogoArrendadora);
                    while (inlSh.Height > 50 && inlSh.Width > 80)
                    {
                        inlSh.Height -= 10;
                        inlSh.Width -= 10;
                    }
                }
            }
            catch { }
            //Texto
            aplicacionWord.Selection.TypeParagraph();
            //aplicacionWord.Selection.Font.Name = "Arial";
            aplicacionWord.Selection.Font.Size = 14;
            aplicacionWord.Selection.Font.Bold = 1;
            aplicacionWord.Selection.TypeText("FICHA INFORMATIVA");
            aplicacionWord.Selection.Font.Size = 10;
            aplicacionWord.Selection.Font.Bold = 1;
            aplicacionWord.Selection.TypeParagraph();
            aplicacionWord.Selection.TypeParagraph();
            aplicacionWord.Selection.Font.Size = 12;
            aplicacionWord.Selection.Font.Bold = 1;
            foreach (string carac in Datos)
            {
                if (carac == "Propietario")
                {
                    aplicacionWord.Selection.TypeText("Propietario: \t\t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(propietario);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Identificador")
                {
                    aplicacionWord.Selection.TypeText("Identificador: \t\t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(identificador);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Nombre")
                {
                    //Se modifica de Descripción a Nombre by Uz  14/06/2016
                    aplicacionWord.Selection.TypeText("Nombre: \t\t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(descripcion);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Dirección")
                {
                    aplicacionWord.Selection.TypeText("Dirección: \t\t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(direccion);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Número Interior")
                {
                    //Agregado por Ing. Uz.  15/06/2016
                    aplicacionWord.Selection.TypeText("Núm. Interior: \t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(numInterior);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Número Exterior")
                {
                    //Agregado por Ing. Uz.  15/06/2016
                    aplicacionWord.Selection.TypeText("Núm. Exterior: \t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(numExterior);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Colonia")
                {
                    //Agregado por Ing. Uz.  15/06/2016
                    aplicacionWord.Selection.TypeText("Colonia: \t\t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(colonia);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Municipio")
                {
                    //Agregado por Ing. Uz.  14/06/2016
                    aplicacionWord.Selection.TypeText("Municipio: \t\t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(municipio);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Estado")
                {
                    //Agregado por Ing. Uz.  14/06/2016
                    aplicacionWord.Selection.TypeText("Estado: \t\t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(estado);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Código Postal")
                {
                    //Agregado por Ing. Uz.  14/06/2016
                    aplicacionWord.Selection.TypeText("Código Postal: \t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(codigoPostal.ToString());
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Área terreno M2 Inmueble")
                {
                    aplicacionWord.Selection.TypeText("Área terreno Inmueble: \t\t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(terreno);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Área construcción M2 Inmueble")
                {
                    aplicacionWord.Selection.TypeText("Área construcción Inmueble: \t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(construccion);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Área terreno M2 Conjunto")
                {
                    aplicacionWord.Selection.TypeText("Área terreno Conjunto: \t\t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(terrenoConjunto + " m2");
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Área construcción M2 Conjunto")
                {
                    aplicacionWord.Selection.TypeText("Área construcción Conjunto: \t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(construccionConjunto + " m2");
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Valor catastral")
                {
                    aplicacionWord.Selection.TypeText("Valor catastral: \t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(valorCatastral);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Valor comercial")
                {
                    aplicacionWord.Selection.TypeText("Valor comercial: \t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(valorComercial);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Uso")
                {
                    aplicacionWord.Selection.TypeText("Uso: \t\t\t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(uso);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }
                else if (carac == "Descripción General")
                {
                    //Agregado por Ing. Uz.  14/06/2016
                    aplicacionWord.Selection.TypeText("Descripción: \t");
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.TypeText(descripcionGeneral);
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 1;
                }

                else if (carac == "Arrendatario" || carac == "Renta mensual" || carac == "Fecha de última renta")
                {
                    aplicacionWord.Selection.TypeText("Arrendatario \t\t\t Renta Mensual   Última Renta");
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.Font.Bold = 0;
                    aplicacionWord.Selection.Font.Size = 10;
                    int cont = 0;
                    if (datosContrato == null)
                        break;
                    foreach (string dato in datosContrato)
                    {
                        if (cont == 12 && UnaPagina)
                        {
                            aplicacionWord.Selection.TypeText("*NOTA: Existen más arrendatarios, cambiar la configuración de la página a 'Más de una página' si desea mostrarlos");
                            aplicacionWord.Selection.TypeParagraph();
                            break;
                        }
                        else
                        {
                            string data = dato;
                            if (!string.IsNullOrEmpty(data))
                            {
                                string[] datos = data.Split('|');
                                if (datos.Length == 3)
                                {
                                    cont++;
                                    string arrendador = datos[0];
                                    string renta = datos[1];
                                    string fechaUlt = datos[2];
                                    if (!String.IsNullOrEmpty(arrendador))
                                    {
                                        if (arrendador.Length >= 35)
                                        {
                                            arrendador = arrendador.Substring(0, 35);
                                            aplicacionWord.Selection.TypeText(arrendador + "\t");
                                        }
                                        else if (arrendador.Length >= 25)
                                            aplicacionWord.Selection.TypeText(arrendador + "\t");
                                        else if (arrendador.Length >= 20)
                                            aplicacionWord.Selection.TypeText(arrendador + "\t\t");
                                        else if (arrendador.Length >= 10)
                                            aplicacionWord.Selection.TypeText(arrendador + "\t\t\t");
                                        else
                                            aplicacionWord.Selection.TypeText(arrendador + "\t\t\t\t");
                                    }
                                    if (!String.IsNullOrEmpty(renta))
                                    {
                                        aplicacionWord.Selection.TypeText(renta + "\t\t");
                                    }
                                    if (!String.IsNullOrEmpty(fechaUlt))
                                    {
                                        aplicacionWord.Selection.TypeText(fechaUlt);
                                    }
                                    aplicacionWord.Selection.TypeParagraph();
                                }
                            }
                        }
                    }
                    break;
                }

            }
            worker.ReportProgress(50);
            if (fotosMismaPag)
            {

                if (Imagenes.Length > 0)
                {
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.TypeParagraph();
                    int cont2 = 0;
                    foreach (var img in Imagenes)
                    {
                        cont2++;
                        /*if (cont2 > 3 && UnaPagina)
                        {
                            aplicacionWord.Selection.TypeText("*NOTA: Sólo se muestran 3 imágenes en la versión a una página");
                            break;
                        }
                        else
                        {*/
                        if (perteneceImagenAConjunto(img))
                        {
                            object c = 10;
                            Word.InlineShape inlSh = aplicacionWord.Selection.InlineShapes.AddPicture(img);//.AddPicture(img);
                            while (inlSh.Height > 155 && inlSh.Width > 185)
                            {
                                inlSh.Height -= 10;
                                inlSh.Width -= 10;
                            }
                          
                        }
                        else //Agregado by Uz 03/06/2016 sino pertenece a un conjunto pero puede pertenecer al inmueble
                        {
                            Word.InlineShape inlSh = aplicacionWord.Selection.InlineShapes.AddPicture(img);
                            while (inlSh.Height > 155 && inlSh.Width > 185)
                            {
                                inlSh.Height -= 10;
                                inlSh.Width -= 10;
                            }
                        }
                        //}
                        aplicacionWord.Selection.TypeParagraph();
                        aplicacionWord.Selection.TypeParagraph();
                    }
                }
            }
            else
            {

                int x = Imagenes.Length;
                if (x > 0)
                {
                    aplicacionWord.Selection.InsertNewPage();
                    aplicacionWord.Selection.TypeParagraph();
                }
                else
                    fotosMismaPag = false;
                int cont2 = 0;
                foreach (var img in Imagenes)
                {
                    cont2++;
                    if (perteneceImagenAConjunto(img))
                    {
                        Word.InlineShape inlSh = aplicacionWord.Selection.InlineShapes.AddPicture(img);
                        while (inlSh.Height > 155 && inlSh.Width > 185)
                        {
                            inlSh.Height -= 10;
                            inlSh.Width -= 10;
                        }
                    }
                    else //Agregado by Uz 03/06/2016 sino pertenece a un conjunto pero puede pertenecer al inmueble
                    {
                        Word.InlineShape inlSh = aplicacionWord.Selection.InlineShapes.AddPicture(img);
                        while (inlSh.Height > 155 && inlSh.Width > 185)
                        {
                            inlSh.Height -= 10;
                            inlSh.Width -= 10;
                        }
                       
                    }
                    aplicacionWord.Selection.TypeParagraph();
                    aplicacionWord.Selection.TypeParagraph();
                }
            }

            worker.ReportProgress(80);
            //Formas 
            System.Drawing.Color color = System.Drawing.Color.FromArgb(150, 70, 255, 5);
            if (!UnaPagina || !fotosMismaPag)
            {
                Word.Range CurrRange = aplicacionWord.Selection.Range;
                int shapeId = 1;
                var shapeIniA = aplicacionWord.Selection.Range.Document.Shapes.AddLine(50, 30, 50, 530);
               
                //var shape = CurrRange.Document.Shapes.AddShape(shapeId, 0, 0, 400, 50);
                //shape.Fill.ForeColor.RGB = color.ToArgb();
                //shape.Line.ForeColor.RGB = color.ToArgb();
                var shapeIniB = aplicacionWord.Selection.Range.Document.Shapes.AddLine(10, 50, 600, 50);
                // var shape2 = CurrRange.Document.Shapes.AddShape(shapeId, 0, 51, 50, 800);
                //shape2.Fill.ForeColor.RGB = color.ToArgb();
                //shape2.Line.ForeColor.RGB = color.ToArgb();
            }
            //var imgVilla = CurrRange.Document.Shapes.AddPicture(GestorReportes.Properties.Settings.Default.RutaImagen_FichaInformativa);
            aplicacionWord.Selection.HomeKey(Word.WdUnits.wdStory, false);
            try
            {
                string fileImg = GestorReportes.Properties.Settings.Default.RutaImagen_FichaInformativa;
                var shapeImg = aplicacionWord.Selection.Document.Shapes.AddPicture(fileImg);
                shapeImg.ConvertToInlineShape();
                //shapeImg.WrapFormat.
            }
            catch { }

            // var shapeIni = aplicacionWord.Selection.Range.Document.Shapes.AddShape(1, 0, 0, 400, 50);
            //shapeIni.Fill.ForeColor.RGB = color.ToArgb();
            //shapeIni.Line.ForeColor.RGB = color.ToArgb();
            //var shape2Ini = aplicacionWord.Selection.Range.Document.Shapes.AddShape(1, 0, 51, 50, 800);
            //shape2Ini.Fill.ForeColor.RGB = color.ToArgb();
            //shape2Ini.Line.ForeColor.RGB = color.ToArgb();
            var shapeIni = aplicacionWord.Selection.Range.Document.Shapes.AddLine(50, 30, 50, 530);
            var shapeIni2 = aplicacionWord.Selection.Range.Document.Shapes.AddLine(10, 50, 600, 50);
            //shapeIni.CanvasCropLeft(60);
            worker.ReportProgress(90);
            //aplicacionWord.Selection.
            if (!esPDF)
            {
                aplicacionWord.Visible = true;
            }
            fileName = GestorReportes.Properties.Settings.Default.RutaRepositorio + "Ficha Informativa";

            if (!System.IO.Directory.Exists(fileName))
            {
                System.IO.Directory.CreateDirectory(fileName);
            }

            fileName += @"\FichaInformativa_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + ".docx";


            documentoWord.SaveAs2(fileName);
            if (esPDF)
            {
                fileName = fileName.Replace(".docx", ".pdf");
                object fileFormat = Word.WdSaveFormat.wdFormatPDF;
                documentoWord.SaveAs2(fileName, fileFormat);
                documentoWord.Close();
                aplicacionWord.Quit();
            }
            worker.ReportProgress(100);
            System.Threading.Thread.Sleep(1000);
        }

        private void setDatos()
        {
            string sql = string.Empty;
            try
            {
                if (Inmueble == "Todos")
                {
                    #region TODOS
                    List<string> idsSubs = new List<string>();
                    sql = "SELECT P0103_RAZON_SOCIAL, T03_CENTRO_INDUSTRIAL.CAMPO4, P0303_NOMBRE, P0306_AVALUO_TOTAL, P0503_CALLE_NUM, P0504_COLONIA, P0506_CIUDAD, " +
                        "P0507_ESTADO, T03_CENTRO_INDUSTRIAL.P0306_M2_TERRENO, T03_CENTRO_INDUSTRIAL.P0306_M2_CONSTRUCCION, T03_CENTRO_INDUSTRIAL.P0306_M2_TOTAL " +
                        "FROM T03_CENTRO_INDUSTRIAL JOIN T01_ARRENDADORA" +
                        " ON T03_CENTRO_INDUSTRIAL.CAMPO1 = T01_ARRENDADORA.P0101_ID_ARR JOIN T05_DOMICILIO ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T05_DOMICILIO.P0500_ID_ENTE" +
                        " WHERE T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = '" + Conjunto + "'";
                    OdbcConnection conex = new OdbcConnection(GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString);
                    OdbcCommand comando = new OdbcCommand(sql, conex);
                    System.Data.DataTable dtConj = new System.Data.DataTable();
                    conex.Open();
                    OdbcDataReader reader = comando.ExecuteReader();
                    dtConj.Load(reader);
                    conex.Close();
                    propietario = dtConj.Rows[0]["P0103_RAZON_SOCIAL"].ToString();
                    identificador = dtConj.Rows[0]["CAMPO4"].ToString();
                    descripcion = dtConj.Rows[0]["P0303_NOMBRE"].ToString();
                    direccion = dtConj.Rows[0]["P0503_CALLE_NUM"].ToString();
                    direccion += " " + dtConj.Rows[0]["P0504_COLONIA"].ToString();
                    municipio = dtConj.Rows[0]["P0506_CIUDAD"].ToString();
                    estado = dtConj.Rows[0]["P0507_ESTADO"].ToString();
                    terrenoConjunto = dtConj.Rows[0]["P0306_M2_TERRENO"].ToString();
                    construccionConjunto = dtConj.Rows[0]["P0306_M2_CONSTRUCCION"].ToString();
                    decimal val = 0;
                    if (!string.IsNullOrEmpty(dtConj.Rows[0]["P0306_AVALUO_TOTAL"].ToString()))
                    {
                        val = Convert.ToDecimal(dtConj.Rows[0]["P0306_AVALUO_TOTAL"].ToString());
                    }
                    valorComercial = val.ToString("C");

                    sql = "SELECT P0701_ID_EDIFICIO, P0706_TERRENO_M2, P0707_CONTRUCCION_M2, P0730_SUBCONJUNTO, T19_DESC_GRAL.P1926_CIST_ING, T19_DESC_GRAL.CAMPO_NUM1, P0728_DESC_GRAL FROM T07_EDIFICIO JOIN T19_DESC_GRAL ON T07_EDIFICIO.P0701_ID_EDIFICIO = T19_DESC_GRAL.P1901_ID_ENTE WHERE P0710_ID_CENTRO = '" + Conjunto + "'";
                    comando = new OdbcCommand(sql, conex);
                    System.Data.DataTable dtInm = new System.Data.DataTable();
                    conex.Open();
                    reader = comando.ExecuteReader();
                    dtInm.Load(reader);
                    conex.Close();
                    decimal terrm2 = 0;
                    decimal constrm2 = 0;
                    string[] idsInmuebles = new string[dtInm.Rows.Count];
                    int contador = 0;
                    foreach (DataRow row in dtInm.Rows)
                    {
                        if (!idsSubs.Contains(row["P0730_SUBCONJUNTO"].ToString()))
                            idsSubs.Add(row["P0730_SUBCONJUNTO"].ToString());
                        idsInmuebles[contador] = row["P0701_ID_EDIFICIO"].ToString();
                        if (row["P1926_CIST_ING"].ToString() != string.Empty)
                            terrm2 += Decimal.Round(Convert.ToDecimal(row["P1926_CIST_ING"].ToString()), 2);
                        if (row["CAMPO_NUM1"].ToString() != string.Empty)
                            constrm2 += Decimal.Round(Convert.ToDecimal(row["CAMPO_NUM1"].ToString()), 2);
                        contador++;
                    }
                    terreno = terrm2.ToString("N") + " m2";
                    construccion = constrm2.ToString("N") + " m2";

                    sql = "SELECT P1901_ID_ENTE, P1906_A_SAN_ING, P1908_OF_BAJA_ING, P1922_A_MIN_ING FROM T19_DESC_GRAL";
                    comando = new OdbcCommand(sql, conex);
                    System.Data.DataTable dtDescGral = new System.Data.DataTable();
                    conex.Open();
                    reader = comando.ExecuteReader();
                    dtDescGral.Load(reader);
                    conex.Close();
                    decimal valorCatastralTotal = 0;
                    foreach (DataRow row in dtDescGral.Rows)
                    {
                        for (int i = 0; i < idsInmuebles.Length; i++)
                        {
                            if (row["P1901_ID_ENTE"].ToString() == idsInmuebles[i])
                            {
                                if (row["P1922_A_MIN_ING"].ToString() != string.Empty)
                                {
                                    valorCatastralTotal += Decimal.Round(Convert.ToDecimal(row["P1922_A_MIN_ING"].ToString()), 2);
                                }
                            }
                        }
                    }
                    valorCatastral = valorCatastralTotal.ToString("C");

                    sql = "SELECT P1403_TEXTO FROM T14_TEXTO_BLOB WHERE P1401_ID_ENTE = '" + Conjunto + "'";
                    comando = new OdbcCommand(sql, conex);
                    System.Data.DataTable dtTexts = new System.Data.DataTable();
                    conex.Open();
                    reader = comando.ExecuteReader();
                    dtTexts.Load(reader);
                    conex.Close();
                    if (dtTexts.Rows.Count > 0)
                        uso = dtTexts.Rows[0]["P1403_TEXTO"].ToString();


                    sql = "SELECT P0401_ID_CONTRATO, P0402_ID_ARRENDAT, P0404_ID_EDIFICIO, P0434_IMPORTE_ACTUAL, P0411_FIN_VIG, P0203_NOMBRE FROM T04_CONTRATO JOIN T02_ARRENDATARIO ON P0402_ID_ARRENDAT = P0201_ID";
                    comando = new OdbcCommand(sql, conex);
                    System.Data.DataTable dtContratos = new System.Data.DataTable();
                    conex.Open();
                    reader = comando.ExecuteReader();
                    dtContratos.Load(reader);
                    conex.Close();
                    string[] dataCenter = new string[idsSubs.Count];
                    decimal rentaMensual = 0;
                    int cont = 0;
                    List<string> contados = new List<string>();
                    DateTime fechaUltima = DateTime.Now.AddYears(-10);
                    foreach (DataRow row in dtContratos.Rows)
                    {
                        foreach (string s in idsSubs)
                        {
                            if (idsSubs.Contains(row["P0404_ID_EDIFICIO"].ToString()))
                            {
                                if (!contados.Contains(row["P0404_ID_EDIFICIO"].ToString()))
                                {
                                    contados.Add(row["P0404_ID_EDIFICIO"].ToString());
                                    if (row["P0434_IMPORTE_ACTUAL"].ToString() != string.Empty)
                                        rentaMensual = Decimal.Round(Convert.ToDecimal(row["P0434_IMPORTE_ACTUAL"].ToString()), 2);
                                    if (row["P0411_FIN_VIG"].ToString() != string.Empty)
                                        fechaUltima = Convert.ToDateTime(row["P0411_FIN_VIG"]);
                                    dataCenter[cont] = row["P0203_NOMBRE"].ToString() + "|" + rentaMensual.ToString("C") + "|" + fechaUltima.ToShortDateString();
                                    cont++;
                                }
                            }
                        }
                    }
                    datosContrato = dataCenter;
                    #endregion
                }
                else
                {
                    sql = @"SELECT P0103_RAZON_SOCIAL, P0726_CLAVE, P0703_NOMBRE, P1906_A_SAN_ING, P1908_OF_BAJA_ING, P1922_A_MIN_ING, P0503_CALLE_NUM, T05_DOMICILIO.CAMPO1, T05_DOMICILIO.CAMPO2, 
                        P0504_COLONIA, P0506_CIUDAD, P0505_COD_POST, P0507_ESTADO, P0508_PAIS, P0701_ID_EDIFICIO, P0706_TERRENO_M2, P0707_CONTRUCCION_M2, P0730_SUBCONJUNTO, 
                        T19_DESC_GRAL.P1926_CIST_ING, T19_DESC_GRAL.CAMPO_NUM1, P0728_DESC_GRAL FROM T07_EDIFICIO JOIN T01_ARRENDADORA" +
                        " ON T07_EDIFICIO.P0709_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR JOIN T05_DOMICILIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T05_DOMICILIO.P0500_ID_ENTE" +
                        " JOIN T19_DESC_GRAL ON T19_DESC_GRAL.P1901_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO" +
                        " WHERE T07_EDIFICIO.P0701_ID_EDIFICIO = '" + Inmueble + "'";
                    OdbcConnection conex = new OdbcConnection(GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString);
                    OdbcCommand comando = new OdbcCommand(sql, conex);
                    System.Data.DataTable dtInm = new System.Data.DataTable();
                    conex.Open();
                    OdbcDataReader reader = comando.ExecuteReader();
                    dtInm.Load(reader);
                    conex.Close();
                    propietario = dtInm.Rows[0]["P0103_RAZON_SOCIAL"].ToString();
                    identificador = dtInm.Rows[0]["P0726_CLAVE"].ToString();
                    descripcion = dtInm.Rows[0]["P0703_NOMBRE"].ToString();
                    direccion = dtInm.Rows[0]["P0503_CALLE_NUM"].ToString();
                    colonia = dtInm.Rows[0]["P0504_COLONIA"].ToString();
                    municipio = dtInm.Rows[0]["P0506_CIUDAD"].ToString();
                    estado = dtInm.Rows[0]["P0507_ESTADO"].ToString();
                    //codigo agregado by Ing. Uz 14/05/2016 
                    //se obtienen datos adicionales para la ficha informativa
                    descripcionGeneral = dtInm.Rows[0]["P0728_DESC_GRAL"] == DBNull.Value ? string.Empty : Encoding.UTF8.GetString((byte[])dtInm.Rows[0]["P0728_DESC_GRAL"]);
                    pais = dtInm.Rows[0]["P0508_PAIS"].ToString();
                    codigoPostal = Convert.ToInt32(dtInm.Rows[0]["P0505_COD_POST"].ToString());
                    numInterior = dtInm.Rows[0]["CAMPO2"].ToString(); //num interior
                    numExterior = dtInm.Rows[0]["CAMPO1"].ToString();//num exterior

                    //reader["DESCRIPCION"] == DBNull.Value ? null : (byte[])reader["DESCRIPCION"];

                    decimal val = 0;
                    if (!string.IsNullOrEmpty(dtInm.Rows[0]["P1908_OF_BAJA_ING"].ToString()))
                        val = Convert.ToDecimal(dtInm.Rows[0]["P1908_OF_BAJA_ING"].ToString());
                    valorComercial = val.ToString("C");

                    decimal terrm2 = 0;
                    decimal constrm2 = 0;
                    if (!string.IsNullOrEmpty(dtInm.Rows[0]["P0707_CONTRUCCION_M2"].ToString()))
                        constrm2 = Decimal.Round(Convert.ToDecimal(dtInm.Rows[0]["P0707_CONTRUCCION_M2"].ToString()), 2);
                    construccion = constrm2.ToString("N") + " m2";

                    if (!string.IsNullOrEmpty(dtInm.Rows[0]["P0706_TERRENO_M2"].ToString()))
                        terrm2 = Decimal.Round(Convert.ToDecimal(dtInm.Rows[0]["P0706_TERRENO_M2"].ToString()), 2);
                    terreno = terrm2.ToString("N") + " m2";

                    decimal valorCatastralTotal = 0;
                    if (!string.IsNullOrEmpty(dtInm.Rows[0]["P1922_A_MIN_ING"].ToString()))
                        valorCatastralTotal = Decimal.Round(Convert.ToDecimal(dtInm.Rows[0]["P1922_A_MIN_ING"].ToString()), 2);
                    valorCatastral = valorCatastralTotal.ToString("C");

                    sql = "SELECT P1403_TEXTO FROM T14_TEXTO_BLOB WHERE P1401_ID_ENTE = '" + Inmueble + "'";
                    comando = new OdbcCommand(sql, conex);
                    System.Data.DataTable dtTexts = new System.Data.DataTable();
                    conex.Open();
                    reader = comando.ExecuteReader();
                    dtTexts.Load(reader);
                    conex.Close();
                    if (dtTexts.Rows.Count > 0)
                        uso = dtTexts.Rows[0]["P1403_TEXTO"].ToString();


                    sql = @"SELECT P0401_ID_CONTRATO, P0402_ID_ARRENDAT, P0404_ID_EDIFICIO, P0434_IMPORTE_ACTUAL, P0411_FIN_VIG, P0203_NOMBRE FROM T04_CONTRATO JOIN T02_ARRENDATARIO ON P0402_ID_ARRENDAT = P0201_ID
                    WHERE P0404_ID_EDIFICIO = '" + Inmueble + "'";
                    comando = new OdbcCommand(sql, conex);
                    System.Data.DataTable dtContratos = new System.Data.DataTable();
                    conex.Open();
                    reader = comando.ExecuteReader();
                    dtContratos.Load(reader);
                    conex.Close();
                    string[] dataCenter = new string[dtContratos.Rows.Count];
                    decimal rentaMensual = 0;
                    int cont = 0;
                    DateTime fechaUltima = DateTime.Now.AddYears(-10);
                    foreach (DataRow row in dtContratos.Rows)
                    {
                        if (row["P0434_IMPORTE_ACTUAL"].ToString() != string.Empty)
                            rentaMensual = Decimal.Round(Convert.ToDecimal(row["P0434_IMPORTE_ACTUAL"].ToString()), 2);
                        if (row["P0411_FIN_VIG"].ToString() != string.Empty)
                            fechaUltima = Convert.ToDateTime(row["P0411_FIN_VIG"]);
                        dataCenter[cont] = row["P0203_NOMBRE"].ToString() + "|" + rentaMensual.ToString("C") + "|" + fechaUltima.ToShortDateString();
                        cont++;
                    }
                    datosContrato = dataCenter;
                }


               LogoArrendadora = SaariE.GetLogoArrendadora(Inmobiliaria);
                          

            }
            catch
            {

            }
        }

        public void generarPDF(System.ComponentModel.BackgroundWorker worker)
        {
            esPDF = true;
            generarWord(worker);
            System.Diagnostics.Process.Start(fileName);
        }

        public string generarPDF(bool mostrar, System.ComponentModel.BackgroundWorker worker)
        {
            esPDF = true;
            generarWord(worker);
            if (mostrar)
                System.Diagnostics.Process.Start(fileName);
            return fileName;
        }

        public void generarPowerPoint(System.ComponentModel.BackgroundWorker worker)
        {
            worker.ReportProgress(0);
            setDatos();
            worker.ReportProgress(10);
            altoAnt = 55;
            //Crear aplicacion
            PowerPoint.Application aplicacionPpt = new PowerPoint.Application();
            //aplicacionPpt.Visible = Microsoft.Office.Core.MsoTriState.msoTrue;
            //Crear objetos PPT
            PowerPoint.Slides slides;
            PowerPoint.Slide slide;
            PowerPoint.TextRange objText;

            //Presentacion PPT
            PowerPoint.Presentation presentacion = aplicacionPpt.Presentations.Add();

            //Layout
            PowerPoint.CustomLayout layout = presentacion.SlideMaster.CustomLayouts[PowerPoint.PpSlideLayout.ppLayoutTitle];

            //Slides
            slides = presentacion.Slides;
            slide = slides.AddSlide(1, layout);
            worker.ReportProgress(20);
            //Add shapes
            System.Drawing.Color color = System.Drawing.Color.FromArgb(150, 70, 255, 5);
           
            // var shape = slide.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle, 0, 0, 400, 50);
            var shape = slide.Shapes.AddLine(50, 30, 50, 530);
            //shape.Fill.ForeColor.RGB = color.ToArgb();
            //shape.Line.ForeColor.RGB = color.ToArgb();

            //shape = slide.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle, 0, 51, 50, 500);
            shape = slide.Shapes.AddLine(10, 50, 600, 50);
            //shape.Fill.ForeColor.RGB = color.ToArgb();
            //shape.Line.ForeColor.RGB = color.ToArgb();
            //Img
            try
            {
                string fileImg = string.Empty;
                try
                {
                    fileImg = GestorReportes.Properties.Settings.Default.RutaImagen_FichaInformativa;
                }
                catch
                {
                    fileImg = LogoArrendadora;
                }

                var shapeImg = slide.Shapes.AddPicture(fileImg, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 0, 0);
                shapeImg.Left = 55;
                shapeImg.Top = 55;
                while (shapeImg.Height > 60)
                {
                    shapeImg.Height -= 5;
                    shapeImg.Width -= 5;
                }
                //anchoAnt += shapeImg.Width;
                altoAnt += shapeImg.Height;
            }
            catch { }
            worker.ReportProgress(50);
            // Add title
            shape = slide.Shapes[1];
            objText = shape.TextFrame.TextRange;
            objText.Text = "FICHA INFORMATIVA";
            objText.Font.Name = "Arial";
            objText.Font.Size = 14;
            objText.Font.Bold = Microsoft.Office.Core.MsoTriState.msoTrue;
            shape.Height = (shape.Height / 2);
            shape.Width = (shape.Width / 2);
            shape.Left = 55;
            altoAnt = altoAnt - 50;
            shape.Top = altoAnt;
            altoAnt += shape.Height;
            //anchoAnt += shape.Width;
            worker.ReportProgress(60);
            //Agregar texto
            shape = slide.Shapes.AddTextbox(Microsoft.Office.Core.MsoTextOrientation.msoTextOrientationHorizontal, 55, altoAnt, 132, (480 - altoAnt));
            var shape2 = slide.Shapes.AddTextbox(Microsoft.Office.Core.MsoTextOrientation.msoTextOrientationHorizontal, 187, altoAnt, 223, (480 - altoAnt));
            altoAnt += shape2.Height;

            //objText = shape.TextFrame.TextRange;
            //objText.Text = "Texto";
            shape.TextFrame2.TextRange.Font.Name = "Arial";
            shape.TextFrame2.TextRange.Font.Size = 12;
            shape2.TextFrame2.TextRange.Font.Name = "Arial";
            shape2.TextFrame2.TextRange.Font.Size = 12;
            string primerColumn = string.Empty;
            string segundaCulomn = string.Empty;
            string tercerColumn = string.Empty;
            foreach (string carac in Datos)
            {
                if (carac == "Propietario")
                {
                    primerColumn += "Propietario:" + Environment.NewLine;
                    if (propietario.Length >= 30)
                        primerColumn += Environment.NewLine;
                    //propietario = propietario.Substring(0, 29);
                    segundaCulomn += propietario + Environment.NewLine;
                    //altoAnt += 10;

                }
                else if (carac == "Identificador")
                {
                    primerColumn += "Identificador:" + Environment.NewLine;
                    segundaCulomn += identificador + Environment.NewLine;
                    // altoAnt += 10;
                }
                else if (carac == "Nombre")
                {
                    primerColumn += "Descripción:" + Environment.NewLine;
                    segundaCulomn += descripcion + Environment.NewLine;
                    //altoAnt += 10;
                }
                else if (carac == "Dirección")
                {
                    primerColumn += "Dirección:" + Environment.NewLine;
                    if (direccion.Length >= 35)
                        primerColumn += Environment.NewLine;
                    //direccion = direccion.Substring(0, 34);
                    segundaCulomn += direccion + Environment.NewLine;
                    //altoAnt += 10;
                }
                else if (carac == "Número Interior")
                {
                    primerColumn += "Número Interior:" + Environment.NewLine;
                    if (direccion.Length >= 35)
                        primerColumn += Environment.NewLine;
                    //direccion = direccion.Substring(0, 34);
                    segundaCulomn += numInterior + Environment.NewLine;
                    //altoAnt += 10;
                }
                else if (carac == "Número Exterior")
                {
                    primerColumn += "Número Exterior:" + Environment.NewLine;
                    if (direccion.Length >= 35)
                        primerColumn += Environment.NewLine;
                    //direccion = direccion.Substring(0, 34);
                    segundaCulomn += numExterior + Environment.NewLine;
                    //altoAnt += 10;
                }
                else if (carac == "Municipio")
                {
                    primerColumn += "Municipio:" + Environment.NewLine;
                    segundaCulomn += municipio + Environment.NewLine;
                    //altoAnt += 10;
                }
                else if (carac == "Estado")
                {
                    primerColumn += "Estado:" + Environment.NewLine;
                    segundaCulomn += estado + Environment.NewLine;
                    //altoAnt += 10;
                }
                else if (carac == "País")
                {
                    primerColumn += "Pais:" + Environment.NewLine;
                    segundaCulomn += pais + Environment.NewLine;
                    //altoAnt += 10;
                }
                else if (carac == "Área terreno M2")
                {
                    primerColumn += "Área terreno:" + Environment.NewLine;
                    segundaCulomn += terreno + Environment.NewLine;
                    // altoAnt += 10;
                }
                else if (carac == "Área construcción M2")
                {
                    primerColumn += "Área construcción:" + Environment.NewLine;
                    segundaCulomn += construccion + Environment.NewLine;
                    //altoAnt += 10;
                }
                else if (carac == "Valor catastral")
                {
                    primerColumn += "Valor catastral:" + Environment.NewLine;
                    segundaCulomn += valorCatastral + Environment.NewLine;
                    //altoAnt += 10;
                }
                else if (carac == "Valor comercial")
                {
                    primerColumn += "Valor comercial:" + Environment.NewLine;
                    segundaCulomn += valorComercial + Environment.NewLine;
                    //altoAnt += 10;
                }
                else if (carac == "Descripción General:")
                {
                    primerColumn += "Descripción:" + Environment.NewLine;
                    if (direccion.Length >= 35)
                        primerColumn += Environment.NewLine;
                    //direccion = direccion.Substring(0, 34);
                    segundaCulomn += descripcionGeneral + Environment.NewLine;
                    //altoAnt += 10;
                }
                else if (carac == "Uso")
                {
                    primerColumn += "Uso:" + Environment.NewLine;
                    segundaCulomn += uso + Environment.NewLine;
                    //altoAnt += 10;
                }
                else if (carac == "Arrendatario" || carac == "Renta mensual" || carac == "Fecha de última renta")
                {
                    var shape3 = slide.Shapes.AddTextbox(Microsoft.Office.Core.MsoTextOrientation.msoTextOrientationHorizontal, 55, 300, 355, 10);
                    shape3.TextFrame2.TextRange.Text = " Arrendatario " + "\t\t\t Renta Mensual   ÚltRen";
                    shape3.TextFrame2.TextRange.Font.Name = "Arial";
                    shape3.TextFrame2.TextRange.Font.Size = 10;
                    shape3.TextFrame2.TextRange.Font.Bold = Microsoft.Office.Core.MsoTriState.msoTrue;


                    int cont = 0;
                    if (datosContrato == null)
                        break;
                    foreach (string dato in datosContrato)
                    {
                        /*if (cont == 12)
                        {
                            var shape5 = slide.Shapes.AddTextbox(Microsoft.Office.Core.MsoTextOrientation.msoTextOrientationHorizontal, 55, 480, 355, 10);
                            shape5.TextFrame2.TextRange.Font.Name = "Arial";
                            shape5.TextFrame2.TextRange.Font.Size = 10;
                            shape5.TextFrame2.TextRange.Text = "*NOTA: Existen más arrendatarios, genere otra ficha informativa que no sea en PowerPoint si desea incluirlos";
                            break;
                        }
                        else
                        {*/
                        string data = dato;
                        if (!string.IsNullOrEmpty(data))
                        {
                            string[] datos = data.Split('|');
                            if (datos.Length == 3)
                            {
                                //string final = string.Empty;
                                cont++;
                                string arrendador = datos[0];
                                string renta = datos[1];
                                string fechaUlt = datos[2];
                                if (!String.IsNullOrEmpty(arrendador))
                                {
                                    if (arrendador.Length >= 35)
                                    {
                                        //final = Environment.NewLine;
                                        // cont++;
                                        arrendador = arrendador.Substring(0, 35);
                                        tercerColumn += arrendador + "\t";
                                        //aplicacionWord.Selection.TypeText(arrendador + "\t");
                                    }
                                    else if (arrendador.Length >= 25)
                                        tercerColumn += arrendador + "\t";//aplicacionWord.Selection.TypeText(arrendador + "\t");
                                    else if (arrendador.Length >= 20)
                                        tercerColumn += arrendador + "\t\t";//aplicacionWord.Selection.TypeText(arrendador + "\t\t");
                                    else if (arrendador.Length >= 10)
                                        tercerColumn += arrendador + "\t\t\t";//aplicacionWord.Selection.TypeText(arrendador + "\t\t\t");
                                    else
                                        tercerColumn += arrendador + "\t\t\t\t";//aplicacionWord.Selection.TypeText(arrendador + "\t\t\t\t");
                                }
                                if (!String.IsNullOrEmpty(renta))
                                {
                                    tercerColumn += renta + "\t";
                                    //aplicacionWord.Selection.TypeText(renta + "\t\t");
                                }
                                if (!String.IsNullOrEmpty(fechaUlt))
                                {
                                    tercerColumn += fechaUlt;
                                    //aplicacionWord.Selection.TypeText(fechaUlt);
                                }
                                //tercerColumn += final;
                                tercerColumn += Environment.NewLine;
                            }
                        }
                        //}
                    }
                    break;
                }
            }
            worker.ReportProgress(80);
            var shape4 = slide.Shapes.AddTextbox(Microsoft.Office.Core.MsoTextOrientation.msoTextOrientationHorizontal, 55, 365, 355, 10);
            shape4.TextFrame2.TextRange.Text = tercerColumn;
            shape4.TextFrame2.TextRange.Font.Name = "Arial";
            shape4.TextFrame2.TextRange.Font.Size = 10;
            shape4.TextFrame2.TextRange.Font.Bold = Microsoft.Office.Core.MsoTriState.msoFalse;
            shape.TextFrame2.TextRange.Font.Bold = Microsoft.Office.Core.MsoTriState.msoTrue;
            shape.TextFrame2.TextRange.Text = primerColumn;
            shape2.TextFrame2.TextRange.Font.Bold = Microsoft.Office.Core.MsoTriState.msoFalse;
            shape2.TextFrame2.TextRange.Text = segundaCulomn;

            if (Imagenes.Length > 0)
            {
                int cont2 = 0;
                int altura = 60;
                int izquierda = 410;
                int contSlide = 1;
                bool addnew = false;
                foreach (var img in Imagenes)
                {
                    //if (perteneceImagenAConjunto(img))

                    if (fotosMismaPag)
                    {
                        cont2++;
                        if (cont2 == 4 || cont2 == 10 || cont2 == 16 || cont2 == 22 || cont2 == 28 || cont2 == 34)
                        {
                            contSlide++;
                            slide = slides.AddSlide(contSlide, layout);
                            for (int c = 1; c <= slide.Shapes.Count; c++)
                                slide.Shapes[c].Delete();
                            //Add shapes
                            shape = slide.Shapes.AddLine(50, 30, 50, 530); //slide.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle, 0, 0, 400, 50);
                            shape = slide.Shapes.AddLine(10, 50, 600, 50); //slide.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle, 0, 0, 400, 50);
                            //shape.Fill.ForeColor.RGB = color.ToArgb();
                            //shape.Line.ForeColor.RGB = color.ToArgb();

                            //shape = slide.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle, 0, 51, 50, 500);
                            //shape.Fill.ForeColor.RGB = color.ToArgb();
                            //shape.Line.ForeColor.RGB = color.ToArgb();
                            altura = 55;
                            izquierda = 55;
                        }
                        else if (cont2 == 7 || cont2 == 13 || cont2 == 19 || cont2 == 25 || cont2 == 31 || cont2 == 37)
                        {
                            altura = 55;
                            izquierda = 410;
                        }
                        var shapeImg2 = slide.Shapes.AddPicture(img, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 0, 0);
                        shapeImg2.Left = izquierda;
                        shapeImg2.Top = altura;
                        altura += 157;
                        while (shapeImg2.Height > 155 && shapeImg2.Width > 185)
                        {
                            shapeImg2.Height -= 10;
                            shapeImg2.Width -= 10;
                        }
                    }
                    else
                    {

                        // aplicacionPpt.ActivePresentation.Slides.AddSlide(2, layout);
                        if (!addnew)
                        {
                            slide = slides.AddSlide(2, layout);
                            shape = slide.Shapes.AddLine(50, 30, 50, 530);
                            shape = slide.Shapes.AddLine(10, 50, 600, 50);
                            addnew = true;
                        }
                        cont2++;
                        if (cont2 == 4 || cont2 == 10 || cont2 == 16 || cont2 == 22 || cont2 == 28 || cont2 == 34)
                        {
                            contSlide++;
                            slide = slides.AddSlide(contSlide, layout);
                            for (int c = 1; c <= slide.Shapes.Count; c++)
                                slide.Shapes[c].Delete();
                            //Add shapes
                            shape = slide.Shapes.AddLine(50, 30, 50, 530);
                            //shape = slide.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle, 0, 0, 400, 50);
                            //shape.Fill.ForeColor.RGB = color.ToArgb();
                            //shape.Line.ForeColor.RGB = color.ToArgb();
                            shape = slide.Shapes.AddLine(10, 50, 600, 50);
                            // shape = slide.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle, 0, 51, 50, 500);
                            //shape.Fill.ForeColor.RGB = color.ToArgb();
                            //shape.Line.ForeColor.RGB = color.ToArgb();
                            altura = 55;
                            izquierda = 60;
                        }
                        else if (cont2 == 7 || cont2 == 13 || cont2 == 19 || cont2 == 25 || cont2 == 31 || cont2 == 37)
                        {
                            altura = 55;
                            izquierda = 410;
                        }
                        var shapeImg2 = slide.Shapes.AddPicture(img, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 0, 0);
                        shapeImg2.Left = izquierda;
                        shapeImg2.Top = altura;
                        altura += 157;
                        while (shapeImg2.Height > 155 && shapeImg2.Width > 185)
                        {
                            shapeImg2.Height -= 10;
                            shapeImg2.Width -= 10;
                        }
                    }
                }
            }
            worker.ReportProgress(95);
            slide.NotesPage.Shapes[2].TextFrame.TextRange.Text = "Ficha Informativa de " + propietario;

            fileName = GestorReportes.Properties.Settings.Default.RutaRepositorio + "Ficha Informativa";
            if (!System.IO.Directory.Exists(fileName))
            {
                System.IO.Directory.CreateDirectory(fileName);
            }
            fileName += @"\FichaInformativa_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + ".pptx";
            
            

            presentacion.SaveAs(fileName);

            aplicacionPpt.Visible = Microsoft.Office.Core.MsoTriState.msoTrue;
            worker.ReportProgress(100);
            //pptPresentation.SaveAs(@"c:\temp\fppt.pptx", Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsDefault, MsoTriState.msoTrue);
            //pptPresentation.Close();
            //pptApplication.Quit();
            //
        }

        private bool perteneceImagenAConjunto(string path)
        {
            try
            {
                BusinessLayer.ComponentLayer.Inmobiliaria inmo = new Inmobiliaria();
                List<string> listaPaths = inmo.getImagenesConjunto(Conjunto);
                return listaPaths.Contains(path.Trim().ToLower());
            }
            catch
            {
                return false;
            }
        }
        private bool perteneceImagenAInmueble(string path, string idInmueble)
        {
            try
            {
                BusinessLayer.ComponentLayer.Inmobiliaria inmo = new Inmobiliaria();
                List<string> listaPaths = inmo.getImagenesInmueble(idInmueble);
                return listaPaths.Contains(path.Trim().ToLower());
            }
            catch
            {
                return false;
            }
        }

    }
}
