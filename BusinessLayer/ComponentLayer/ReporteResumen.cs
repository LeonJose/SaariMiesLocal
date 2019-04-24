using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class ReporteResumen : SaariReport, IReport, IBackgroundReport
    {
        public string GrupoEmpresarial { get; set; }
        public DateTime Vigencia { get; set; }
        public decimal TipoDeCambio { get; set; }
        public string Usuario { get; set; }

        private int filaSubTotalRentado = 0;
        private int filaSubTotalNoRentado = 0;
        private int filaDelTotal = 0;
        private string nombreGpoEmp = string.Empty;
        private DataTable dtRentados = new DataTable();
        private DataTable dtNoRentados = new DataTable();
        private string error = string.Empty;
        private string[] inmobiliariasNombres = null;
        private string[] inmobiliariasNombresNoRent = null;
        private int[] cantidadesReport;
        private int[] cantidadesNoRent;
        private decimal[] catastralesRent;
        private decimal[] catastralesNoRent;
        private decimal[] terrenoM2Predial;
        private decimal[] terrenoM2PredialNoRent;
        private decimal[] constM2Predial;
        private decimal[] constM2PredialNoRent;
        private decimal[] importesFactRent;
        private decimal[] importesFactNoRent;
        private string filename = string.Empty;

        public ReporteResumen()
        {

        }

        public ReporteResumen(string idGrupo, DateTime vigencia, decimal tipoCambio, string usuario)
        {
            GrupoEmpresarial = idGrupo;
            Vigencia = vigencia;
            TipoDeCambio = tipoCambio;
            Usuario = usuario;
        }

        public string generar()
        {
            OnCambioProgreso(10);
            if (CancelacionPendiente)
                return "Proceso cancelado por el usuario";

            if (obtenerDatos())
            {
                try
                {
                    Excel.Application aplicacionExcel = new Excel.Application();
                    Excel.Workbook libroExcel = aplicacionExcel.Workbooks.Add();
                    Excel.Worksheet hojaExcel = (libroExcel.ActiveSheet as Excel.Worksheet);

                    //encabezado
                    Excel.Range rango = hojaExcel.get_Range("A1:H1");
                    rango.Merge();
                    rango.Value2 = "Resumen " + nombreGpoEmp + " al: " + Vigencia.ToString("dd/MMMM/yyyy");
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 18;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //subencabezados
                    rango = hojaExcel.get_Range("A2:H2");
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    hojaExcel.get_Range("A2").Value2 = "Detalles";
                    hojaExcel.get_Range("B2").Value2 = "Cantidad de subconjuntos";
                    hojaExcel.get_Range("C2").Value2 = "Porcentaje";
                    hojaExcel.get_Range("D2").Value2 = "Valor catastral";
                    hojaExcel.get_Range("E2").Value2 = "Terreno m2";
                    hojaExcel.get_Range("F2").Value2 = "Construcción m2";
                    hojaExcel.get_Range("G2").Value2 = "Ingresos por renta anual";
                    hojaExcel.get_Range("H2").Value2 = "Productividad";

                    //rentadosEncab
                    rango = hojaExcel.get_Range("A3:H3");
                    rango.Merge();
                    rango.Value2 = "RENTADOS";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                     
                    //ColumnaA
                    int posicionRango = 4;
                    if (inmobiliariasNombres != null)
                    {
                        foreach (string inmob in inmobiliariasNombres)
                        {
                            if (!string.IsNullOrEmpty(inmob))
                            {
                                rango = hojaExcel.get_Range("A" + posicionRango);
                                rango.Value2 = inmob;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        filaSubTotalRentado = posicionRango;
                        rango = hojaExcel.get_Range("A" + filaSubTotalRentado);
                        rango.Value2 = "Subtotal";
                        rango.Font.Bold = true;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        posicionRango++;                        
                    }

                    OnCambioProgreso(60);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //NoRentados
                    rango = hojaExcel.get_Range("A" + posicionRango + ":H" + posicionRango);
                    rango.Merge();
                    rango.Value2 = "NO RENTADOS";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    posicionRango++;
                    if (inmobiliariasNombresNoRent != null)
                    {
                        foreach (string inmob in inmobiliariasNombresNoRent)
                        {
                            if (!string.IsNullOrEmpty(inmob))
                            {
                                rango = hojaExcel.get_Range("A" + posicionRango);
                                rango.Value2 = inmob;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        filaSubTotalNoRentado = posicionRango;
                        rango = hojaExcel.get_Range("A" + filaSubTotalNoRentado);
                        rango.Value2 = "Subtotal";
                        rango.Font.Bold = true;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        posicionRango++;  
                    }
                    filaDelTotal = posicionRango;
                    rango = hojaExcel.get_Range("A" + filaDelTotal);
                    rango.Value2 = "TOTAL";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //ColumnaB,ColumnaC
                    posicionRango = 4;
                    if (cantidadesReport != null)
                    {
                        foreach (int c in cantidadesReport)
                        {
                            rango = hojaExcel.get_Range("B" + posicionRango);
                            rango.Value2 = c;
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            
                            rango = hojaExcel.get_Range("C" + posicionRango);
                            rango.NumberFormat = "0.00%";
                            rango.Formula = "=(B"+ posicionRango + "/B"+ filaDelTotal +")";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;

                            posicionRango++;
                        }
                    }
                    //subtotalRentados
                    if (filaSubTotalRentado != 0)
                    {
                        rango = hojaExcel.get_Range("B" + filaSubTotalRentado);
                        rango.Formula = "=SUMA(B4:B" + (filaSubTotalRentado - 1) + ")";
                        rango.Font.Bold = true;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        rango = hojaExcel.get_Range("C" + filaSubTotalRentado);
                        rango.NumberFormat = "0.00%";
                        rango.Formula = "=SUMA(C4:C" + (filaSubTotalRentado - 1) + ")";
                        rango.Font.Bold = true;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        rango = hojaExcel.get_Range("D" + filaSubTotalRentado);
                        rango.NumberFormat = "$###,###,###,###,000.00";
                        rango.Formula = "=SUMA(D4:D" + (filaSubTotalRentado - 1) + ")";
                        rango.Font.Bold = true;
                        //rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        rango = hojaExcel.get_Range("E" + filaSubTotalRentado);
                        rango.NumberFormat = "0";
                        rango.Formula = "=SUMA(E4:E" + (filaSubTotalRentado - 1) + ")";
                        rango.Font.Bold = true;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        rango = hojaExcel.get_Range("F" + filaSubTotalRentado);
                        rango.NumberFormat = "0";
                        rango.Formula = "=SUMA(F4:F" + (filaSubTotalRentado - 1) + ")";
                        rango.Font.Bold = true;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        rango = hojaExcel.get_Range("G" + filaSubTotalRentado);
                        rango.NumberFormat = "$###,###,###,###,000.00";
                        rango.Formula = "=SUMA(G4:G" + (filaSubTotalRentado - 1) + ")";
                        rango.Font.Bold = true;
                        //rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        rango = hojaExcel.get_Range("H" + filaSubTotalRentado);
                        rango.NumberFormat = "0.00%";
                        rango.Formula = "=G"+filaSubTotalRentado+"/D" + filaSubTotalRentado;
                        rango.Font.Bold = true;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        posicionRango++;
                    }

                    OnCambioProgreso(70);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    posicionRango++; //leyenda No Rentados
                    // subtotalNoRentados
                    if (cantidadesNoRent != null)
                    {
                        foreach (int c in cantidadesNoRent)
                        {
                            rango = hojaExcel.get_Range("B" + posicionRango);
                            rango.Value2 = c;
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;

                            rango = hojaExcel.get_Range("C" + posicionRango);
                            rango.NumberFormat = "0.00%";
                            rango.Formula = "=(B" + posicionRango + "/B" + filaDelTotal + ")";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;

                            posicionRango++;
                        }
                    }
                    if (filaSubTotalNoRentado != 0)
                    {
                        rango = hojaExcel.get_Range("B" + filaSubTotalNoRentado);
                        rango.Formula = "=SUMA(B"+(posicionRango-cantidadesNoRent.Length)+":B" + (filaSubTotalNoRentado - 1) + ")";
                        rango.Font.Bold = true;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        rango = hojaExcel.get_Range("C" + filaSubTotalNoRentado);
                        rango.NumberFormat = "0.00%";
                        rango.Formula = "=SUMA(C" + (posicionRango - cantidadesNoRent.Length) + ":C" + (filaSubTotalNoRentado - 1) + ")";
                        rango.Font.Bold = true;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        rango = hojaExcel.get_Range("D" + filaSubTotalNoRentado);
                        rango.NumberFormat = "$###,###,###,###,000.00";
                        rango.Formula = "=SUMA(D" + (posicionRango - cantidadesNoRent.Length) + ":D" + (filaSubTotalNoRentado - 1) + ")";
                        rango.Font.Bold = true;
                        //rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        rango = hojaExcel.get_Range("E" + filaSubTotalNoRentado);
                        rango.NumberFormat = "0";
                        rango.Formula = "=SUMA(E" + (posicionRango - cantidadesNoRent.Length) + ":E" + (filaSubTotalNoRentado - 1) + ")";
                        rango.Font.Bold = true;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        rango = hojaExcel.get_Range("F" + filaSubTotalNoRentado);
                        rango.NumberFormat = "0";
                        rango.Formula = "=SUMA(F" + (posicionRango - cantidadesNoRent.Length) + ":F" + (filaSubTotalNoRentado - 1) + ")";
                        rango.Font.Bold = true;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        rango = hojaExcel.get_Range("G" + filaSubTotalNoRentado);
                        rango.NumberFormat = "$###,###,###,###,000.00";
                        rango.Formula = "=SUMA(G" + (posicionRango - cantidadesNoRent.Length) + ":G" + (filaSubTotalNoRentado - 1) + ")";
                        rango.Font.Bold = true;
                        //rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        rango = hojaExcel.get_Range("H" + filaSubTotalNoRentado);
                        rango.NumberFormat = "0.00%";
                        rango.Formula = "=G" + filaSubTotalNoRentado + "/D" + filaSubTotalNoRentado;
                        rango.Font.Bold = true;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        posicionRango++;
                    }

                    OnCambioProgreso(80);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //Columna D                    
                    posicionRango = 4;
                    if (catastralesRent != null)
                    {
                        foreach (decimal catast in catastralesRent)
                        {
                            rango = hojaExcel.get_Range("D" + posicionRango);
                            rango.Value2 = catast;
                            rango.NumberFormat = "$###,###,###,###,000.00";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }
                    if(filaSubTotalRentado !=0)
                        posicionRango = filaSubTotalRentado+2;
                    else
                        posicionRango = 5;
                    if (catastralesNoRent != null)
                    {
                        foreach (decimal catast in catastralesNoRent)
                        {
                            rango = hojaExcel.get_Range("D" + posicionRango);
                            rango.Value2 = catast;
                            rango.NumberFormat = "$###,###,###,###,000.00";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }

                    //ColumnaE
                    posicionRango = 4;
                    if (terrenoM2Predial != null)
                    {
                        foreach (decimal terre in terrenoM2Predial)
                        {
                            rango = hojaExcel.get_Range("E" + posicionRango);
                            rango.Value2 = terre;
                            rango.NumberFormat = "0";
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }
                    if (filaSubTotalRentado != 0)
                        posicionRango = filaSubTotalRentado + 2;
                    else
                        posicionRango = 5;
                    if (terrenoM2PredialNoRent != null)
                    {
                        foreach (decimal terre in terrenoM2PredialNoRent)
                        {
                            rango = hojaExcel.get_Range("E" + posicionRango);
                            rango.Value2 = terre;
                            rango.NumberFormat = "0";
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }

                    //ColumnaF
                    posicionRango = 4;
                    if (constM2Predial != null)
                    {
                        foreach (decimal constru in constM2Predial)
                        {
                            rango = hojaExcel.get_Range("F" + posicionRango);
                            rango.Value2 = constru;
                            rango.NumberFormat = "0";
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }
                    if (filaSubTotalRentado != 0)
                        posicionRango = filaSubTotalRentado + 2;
                    else
                        posicionRango = 5;
                    if (constM2PredialNoRent != null)
                    {
                        foreach (decimal constru in constM2PredialNoRent)
                        {
                            rango = hojaExcel.get_Range("F" + posicionRango);
                            rango.Value2 = constru;
                            rango.NumberFormat = "0";
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }

                    //ColumnaGyH
                    posicionRango = 4;
                    if (importesFactRent != null)
                    {
                        foreach (decimal import in importesFactRent)
                        {
                            rango = hojaExcel.get_Range("G" + posicionRango);
                            rango.Value2 = import;
                            rango.NumberFormat = "$###,###,###,###,#00.00";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                            rango = hojaExcel.get_Range("H" + posicionRango);
                            rango.NumberFormat = "0.00%";
                            rango.Formula = "=(G" + posicionRango + "/D" + posicionRango + ")";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;

                            posicionRango++;
                        }
                    }
                    if (filaSubTotalRentado != 0)
                        posicionRango = filaSubTotalRentado + 2;
                    else
                        posicionRango = 5;
                    if (importesFactNoRent != null)
                    {
                        foreach (decimal impo in importesFactNoRent)
                        {
                            rango = hojaExcel.get_Range("G" + posicionRango);
                            rango.Value2 = impo;
                            rango.NumberFormat = "$###,###,###,###,#00.00";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                            rango = hojaExcel.get_Range("H" + posicionRango);
                            rango.NumberFormat = "0.00%";
                            rango.Formula = "=(G" + posicionRango + "/D" + posicionRango + ")";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;

                            posicionRango++;
                        }
                    }

                    OnCambioProgreso(90);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //TOTAL B
                    rango = hojaExcel.get_Range("B" + filaDelTotal);
                    if (filaSubTotalRentado != 0 && filaSubTotalNoRentado != 0)
                        rango.Formula = "=SUMA(B" + filaSubTotalRentado + "+B" + filaSubTotalNoRentado + ")";
                    else if (filaSubTotalRentado != 0)
                        rango.Formula = "=SUMA(B" + filaSubTotalRentado + "+0)";
                    else if (filaSubTotalNoRentado !=0)
                        rango.Formula = "=SUMA(B" + filaSubTotalNoRentado + "+0)";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //TOTAL C
                    rango = hojaExcel.get_Range("C" + filaDelTotal);
                    if (filaSubTotalRentado != 0 && filaSubTotalNoRentado != 0)
                        rango.Formula = "=SUMA(C" + filaSubTotalRentado + "+C" + filaSubTotalNoRentado + ")";
                    else if (filaSubTotalRentado != 0)
                        rango.Formula = "=SUMA(C" + filaSubTotalRentado + "+0)";
                    else if (filaSubTotalNoRentado != 0)
                        rango.Formula = "=SUMA(C" + filaSubTotalNoRentado + "+0)";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //Total D
                    rango = hojaExcel.get_Range("D" + filaDelTotal);
                    if (filaSubTotalRentado != 0 && filaSubTotalNoRentado != 0)
                        rango.Formula = "=SUMA(D" + filaSubTotalRentado + "+D" + filaSubTotalNoRentado + ")";
                    else if (filaSubTotalRentado != 0)
                        rango.Formula = "=SUMA(D" + filaSubTotalRentado + "+0)";
                    else if (filaSubTotalNoRentado != 0)
                        rango.Formula = "=SUMA(D" + filaSubTotalNoRentado + "+0)";
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //TotalE
                    rango = hojaExcel.get_Range("E" + filaDelTotal);
                    if (filaSubTotalRentado != 0 && filaSubTotalNoRentado != 0)
                        rango.Formula = "=SUMA(E" + filaSubTotalRentado + "+E" + filaSubTotalNoRentado + ")";
                    else if (filaSubTotalRentado != 0)
                        rango.Formula = "=SUMA(E" + filaSubTotalRentado + "+0)";
                    else if (filaSubTotalNoRentado != 0)
                        rango.Formula = "=SUMA(E" + filaSubTotalNoRentado + "+0)";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //TotalF
                    rango = hojaExcel.get_Range("F" + filaDelTotal);
                    if (filaSubTotalRentado != 0 && filaSubTotalNoRentado != 0)
                        rango.Formula = "=SUMA(F" + filaSubTotalRentado + "+F" + filaSubTotalNoRentado + ")";
                    else if (filaSubTotalRentado != 0)
                        rango.Formula = "=SUMA(F" + filaSubTotalRentado + "+0)";
                    else if (filaSubTotalNoRentado != 0)
                        rango.Formula = "=SUMA(F" + filaSubTotalNoRentado + "+0)";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //TotalG
                    rango = hojaExcel.get_Range("G" + filaDelTotal);
                    if (filaSubTotalRentado != 0 && filaSubTotalNoRentado != 0)
                        rango.Formula = "=SUMA(G" + filaSubTotalRentado + "+G" + filaSubTotalNoRentado + ")";
                    else if (filaSubTotalRentado != 0)
                        rango.Formula = "=SUMA(G" + filaSubTotalRentado + "+0)";
                    else if (filaSubTotalNoRentado != 0)
                        rango.Formula = "=SUMA(G" + filaSubTotalNoRentado + "+0)";
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //TOTAL H
                    rango = hojaExcel.get_Range("H" + filaDelTotal);
                    rango.Formula = "=G" + filaDelTotal + "/D" + filaDelTotal;
                    rango.NumberFormat = "0.00%";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //Tipo de cambio
                    rango = hojaExcel.get_Range("A" + (filaDelTotal + 1));
                    rango.Value2 = "Tipo de cambio: " + TipoDeCambio;
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //Generado por
                    rango = hojaExcel.get_Range("A" + (filaDelTotal + 2));
                    rango.Value2 = "Reporte generado por: " + Usuario;
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    hojaExcel.get_Range("A2:H2").Columns.AutoFit();
                    hojaExcel.get_Range("A2:A1000").Columns.AutoFit();
                    hojaExcel.get_Range("D2:D1000").Columns.AutoFit();

                    filename = GestorReportes.Properties.Settings.Default.RutaRepositorio + @"Resumen_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + ".xlsx";
                    libroExcel.SaveAs(filename);
                    aplicacionExcel.Visible = true;
                    
                    OnCambioProgreso(100);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    System.Threading.Thread.Sleep(1000);
                }
                catch
                {
                    error = "- Error en Interop Excel";
                }
            }
            return error;
        }

        public static string getTipoDeCambio(DateTime vigenciaSeleccionada)
        {
            Inmobiliaria inmobiliaria = new Inmobiliaria();
            int mesACalcular = vigenciaSeleccionada.Month;
            int mesSig = vigenciaSeleccionada.Month + 1;
            if (mesSig == 13)
                mesSig = 1;
            int anio = vigenciaSeleccionada.Year;
            DateTime ini = Convert.ToDateTime("01/" + mesACalcular + "/"+ anio);
            DateTime fin = Convert.ToDateTime("01/" + mesSig + "/" + anio).AddDays(-1);
            DataTable dtTiposCambio = inmobiliaria.getDtTiposCambio(ini, fin);
            decimal promedio = 0;
            if (dtTiposCambio.Rows.Count > 0)
            {
                foreach(DataRow row in dtTiposCambio.Rows)
                {
                    promedio += Convert.ToDecimal(row["P3904_VALOR_INDEX"].ToString());
                }
                if (promedio > 0)
                    promedio = promedio / dtTiposCambio.Rows.Count;
                return String.Format("{0:0.0000}", promedio);
            }
            else
            {
                return String.Format("{0:0.0000}", 0);
            }
        }

        private bool obtenerDatos()
        {
            Inmobiliaria inmobiliaria = new Inmobiliaria();
            DataTable dtNombreGpo = inmobiliaria.getDtGrupoEmpresarial(GrupoEmpresarial);
            nombreGpoEmp = dtNombreGpo.Rows[0]["P0002_NOMBRE"].ToString();
            
            dtRentados = inmobiliaria.getDtSubRentados(GrupoEmpresarial, Vigencia);
            dtNoRentados = inmobiliaria.getDtSubConjuntosNoRentadosPorGpoEmp(GrupoEmpresarial);
            if (dtRentados.Rows.Count <= 0 && dtNoRentados.Rows.Count <= 0)
            {
                error = "- No se encontraron subconjuntos de acuerdo a los criterios seleccionados";
                return false;
            }
            OnCambioProgreso(20);
            if (CancelacionPendiente)
            {
                error = "Proceso cancelado por el usuario";
                return false;
            }

            if (dtRentados.Rows.Count > 0)
            {
                string inmo = string.Empty;
                DataRow[] drsPesos = dtRentados.Select("P0407_MONEDA_FACT = 'P'");
                if (drsPesos.Length > 0)
                {
                    foreach (DataRow dr in drsPesos)
                    {
                        if (!inmo.Contains(dr["P0103_RAZON_SOCIAL"].ToString()))
                            inmo += dr["P0103_RAZON_SOCIAL"].ToString() + ": Pesos" + "|";
                    }
                }
                DataRow[] drsDlls = dtRentados.Select("P0407_MONEDA_FACT = 'D'");
                if (drsDlls.Length > 0)
                {
                    foreach (DataRow dr in drsDlls)
                    {
                        if (!inmo.Contains(dr["P0103_RAZON_SOCIAL"].ToString() + ": Dolares (convertido a pesos)"))
                        {
                            inmo += dr["P0103_RAZON_SOCIAL"].ToString() + ": Dolares (convertido a pesos)" + "|";
                        }
                    }
                }
                OnCambioProgreso(30);
                if (CancelacionPendiente)
                {
                    error = "Proceso cancelado por el usuario";
                    return false;
                }
                inmobiliariasNombres = inmo.Split('|');
                if (inmobiliariasNombres.Length > 0)
                {
                    int[] cantidades = new int[inmobiliariasNombres.Length - 1];
                    decimal[] catastrales = new decimal[inmobiliariasNombres.Length - 1];
                    decimal[] terrenoM2 = new decimal[inmobiliariasNombres.Length - 1];
                    decimal[] constM2 = new decimal[inmobiliariasNombres.Length - 1];
                    decimal[] importeFact = new decimal[inmobiliariasNombres.Length - 1];
                    int contador = 0;
                    foreach (string inmobNmb in inmobiliariasNombres)
                    {
                       // catastrales[contador] = 0;
                        if (!string.IsNullOrEmpty(inmobNmb))
                        {
                            if (inmobNmb.Contains("Pesos"))
                            {
                                string inmTrunc = inmobNmb.Replace(": Pesos", "");
                                DataRow[] drsCantPesosPorInm = dtRentados.Select("P0407_MONEDA_FACT = 'P' AND P0103_RAZON_SOCIAL = '" + inmTrunc + "'");
                                cantidades[contador] = drsCantPesosPorInm.Length;
                                foreach (DataRow dr in drsCantPesosPorInm)
                                {
                                    DataTable dtInmuebPorSubConj = inmobiliaria.getDtInmueblesPorSubConj(dr["P1801_ID_SUBCONJUNTO"].ToString());
                                    foreach (DataRow drSub in dtInmuebPorSubConj.Rows)
                                    {
                                        if (!string.IsNullOrEmpty(drSub["P1922_A_MIN_ING"].ToString()))
                                        {
                                            catastrales[contador] += Convert.ToDecimal(drSub["P1922_A_MIN_ING"].ToString());
                                        }
                                        if (!string.IsNullOrEmpty(drSub["P1926_CIST_ING"].ToString()))
                                        {
                                            terrenoM2[contador] += Convert.ToDecimal(drSub["P1926_CIST_ING"].ToString());
                                        }
                                        if (!string.IsNullOrEmpty(drSub["CAMPO_NUM1"].ToString()))
                                        {
                                            constM2[contador] += Convert.ToDecimal(drSub["CAMPO_NUM1"].ToString());
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(dr["P0434_IMPORTE_ACTUAL"].ToString()))
                                    {
                                        importeFact[contador] += (Convert.ToDecimal(dr["P0434_IMPORTE_ACTUAL"].ToString()) * 12);
                                    }                                    
                                }
                            }
                            else if (inmobNmb.Contains("Dolares"))
                            {
                                string inmTrunc = inmobNmb.Replace(": Dolares (convertido a pesos)", "");
                                DataRow[] drsCantDllsPorInm = dtRentados.Select("P0407_MONEDA_FACT = 'D' AND P0103_RAZON_SOCIAL = '" + inmTrunc + "'");
                                cantidades[contador] = drsCantDllsPorInm.Length;
                                foreach (DataRow dr in drsCantDllsPorInm)
                                {
                                    DataTable dtInmuebPorSubConj = inmobiliaria.getDtInmueblesPorSubConj(dr["P1801_ID_SUBCONJUNTO"].ToString());
                                    foreach (DataRow drSub in dtInmuebPorSubConj.Rows)
                                    {
                                        if (!string.IsNullOrEmpty(drSub["P1922_A_MIN_ING"].ToString()))
                                        {
                                            catastrales[contador] += Convert.ToDecimal(drSub["P1922_A_MIN_ING"].ToString());
                                        }
                                        if (!string.IsNullOrEmpty(drSub["P1926_CIST_ING"].ToString()))
                                        {
                                            terrenoM2[contador] += Convert.ToDecimal(drSub["P1926_CIST_ING"].ToString());
                                        }
                                        if (!string.IsNullOrEmpty(drSub["CAMPO_NUM1"].ToString()))
                                        {
                                            constM2[contador] += Convert.ToDecimal(drSub["CAMPO_NUM1"].ToString());
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(dr["P0434_IMPORTE_ACTUAL"].ToString()))
                                    {
                                        importeFact[contador] += (Convert.ToDecimal(dr["P0434_IMPORTE_ACTUAL"].ToString()) * 12) * TipoDeCambio;
                                    }
                                }
                            }
                            contador++;
                        }
                    }
                    cantidadesReport = cantidades;
                    catastralesRent = catastrales;
                    terrenoM2Predial = terrenoM2;
                    constM2Predial = constM2;
                    importesFactRent = importeFact;
                }
            }

            OnCambioProgreso(40);
            if (CancelacionPendiente)
            {
                error = "Proceso cancelado por el usuario";
                return false;
            }

            if (dtNoRentados.Rows.Count > 0)
            {
                string inmobNoRent = string.Empty;
                DataRow[] drsPesosNR = dtNoRentados.Select("P18_CAMPO2 = 'P'");
                if (drsPesosNR.Length > 0)
                {
                    foreach (DataRow dr in drsPesosNR)
                    {
                        if (dr["P18_CAMPO3"].ToString() != "N")
                        {
                            if (!inmobNoRent.Contains(dr["P0103_RAZON_SOCIAL"].ToString()))
                                inmobNoRent += dr["P0103_RAZON_SOCIAL"].ToString() + ": Pesos" + "|";
                        }
                    }
                }
                DataRow[] drsDllsNR = dtNoRentados.Select("P18_CAMPO2 = 'D'");
                if (drsDllsNR.Length > 0)
                {
                    foreach (DataRow dr in drsDllsNR)
                    {
                        if (dr["P18_CAMPO3"].ToString() != "N")
                        {
                            if (!inmobNoRent.Contains(dr["P0103_RAZON_SOCIAL"].ToString() + ": Dolares (convertido a pesos)"))
                            {
                                inmobNoRent += dr["P0103_RAZON_SOCIAL"].ToString() + ": Dolares (convertido a pesos)" + "|";
                            }
                        }
                    }
                }

                OnCambioProgreso(50);
                if (CancelacionPendiente)
                {
                    error = "Proceso cancelado por el usuario";
                    return false;
                }

                inmobiliariasNombresNoRent = inmobNoRent.Split('|');
                DataTable dtNoRentSubInm = inmobiliaria.getDtInmueblesPorSubConjuntosNoRentadosPorGpoEmp(GrupoEmpresarial);
                if (inmobiliariasNombresNoRent.Length > 0)
                {
                    int[] cantidades = new int[inmobiliariasNombresNoRent.Length - 1];
                    decimal[] catastrales = new decimal[inmobiliariasNombresNoRent.Length - 1];
                    decimal[] terrenos  = new decimal[inmobiliariasNombresNoRent.Length - 1];
                    decimal[] constM2 = new decimal[inmobiliariasNombresNoRent.Length - 1];
                    decimal[] impNoRentados = new decimal[inmobiliariasNombresNoRent.Length - 1];
                    int contador = 0;
                    foreach (string inmobNmb in inmobiliariasNombresNoRent)
                    {
                        if (!string.IsNullOrEmpty(inmobNmb))
                        {
                            if (inmobNmb.Contains("Pesos"))
                            {
                                string inmTrunc = inmobNmb.Replace(": Pesos", "");
                                dtNoRentados = inmobiliaria.getDtSubConjuntosNoRentadosPorGpoEmp(GrupoEmpresarial);
                                DataRow[] drsCantPesosPorInm = dtNoRentados.Select("P18_CAMPO2 = 'P' AND P0103_RAZON_SOCIAL = '" + inmTrunc.Trim() + "'");
                                cantidades[contador] = drsCantPesosPorInm.Length;
                                foreach (DataRow dr in drsCantPesosPorInm)
                                {
                                    if (!string.IsNullOrEmpty(dr["P18CAMPO_NUM1"].ToString()))
                                    {
                                        impNoRentados[contador] += (Convert.ToDecimal(dr["P18CAMPO_NUM1"].ToString()) * 12);
                                    }
                                }
                                DataRow[] drsCatastPesos = dtNoRentSubInm.Select("P18_CAMPO2 = 'P' AND P0103_RAZON_SOCIAL = '" + inmTrunc + "'");
                                foreach (DataRow dr in drsCatastPesos)
                                {
                                    if (!string.IsNullOrEmpty(dr["P1922_A_MIN_ING"].ToString()))
                                    {
                                        catastrales[contador] += Convert.ToDecimal(dr["P1922_A_MIN_ING"].ToString());
                                    }
                                    if (!string.IsNullOrEmpty(dr["P1926_CIST_ING"].ToString()))
                                    {
                                        terrenos[contador] += Convert.ToDecimal(dr["P1926_CIST_ING"].ToString());
                                    }
                                    if (!string.IsNullOrEmpty(dr["CAMPO_NUM1"].ToString()))
                                    {
                                        constM2[contador] += Convert.ToDecimal(dr["CAMPO_NUM1"].ToString());
                                    }
                                }
                            }
                            else if (inmobNmb.Contains("Dolares"))
                            {
                                string inmTrunc = inmobNmb.Replace(": Dolares (convertido a pesos)", "");
                                DataRow[] drsCantDllsPorInm = dtNoRentados.Select("P18_CAMPO2 = 'D' AND P0103_RAZON_SOCIAL = '" + inmTrunc + "'");
                                cantidades[contador] = drsCantDllsPorInm.Length;
                                foreach (DataRow dr in drsCantDllsPorInm)
                                {
                                    if (!string.IsNullOrEmpty(dr["P18CAMPO_NUM1"].ToString()))
                                    {
                                        impNoRentados[contador] += (Convert.ToDecimal(dr["P18CAMPO_NUM1"].ToString()) * 12) * TipoDeCambio;
                                    }
                                }
                                DataRow[] drsCatastDlls = dtNoRentSubInm.Select("P18_CAMPO2 = 'D' AND P0103_RAZON_SOCIAL = '" + inmTrunc + "'");
                                foreach (DataRow dr in drsCatastDlls)
                                {
                                    if (!string.IsNullOrEmpty(dr["P1922_A_MIN_ING"].ToString()))
                                    {
                                        catastrales[contador] += Convert.ToDecimal(dr["P1922_A_MIN_ING"].ToString()) * TipoDeCambio;
                                    }
                                    if (!string.IsNullOrEmpty(dr["P1926_CIST_ING"].ToString()))
                                    {
                                        terrenos[contador] += Convert.ToDecimal(dr["P1926_CIST_ING"].ToString());
                                    }
                                    if (!string.IsNullOrEmpty(dr["CAMPO_NUM1"].ToString()))
                                    {
                                        constM2[contador] += Convert.ToDecimal(dr["CAMPO_NUM1"].ToString());
                                    }
                                }
                            }
                            contador++;
                        }
                    }
                    cantidadesNoRent = cantidades;
                    catastralesNoRent = catastrales;
                    terrenoM2PredialNoRent = terrenos;
                    constM2PredialNoRent = constM2;
                    importesFactNoRent = impNoRentados;
                }
            }
            
            return true;
           
        }
    }
}
