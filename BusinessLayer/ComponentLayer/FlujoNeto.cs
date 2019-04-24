using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.Abstracts;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class FlujoNeto : SaariReport, IReport, IBackgroundReport
    {
        //string prueba = string.Empty;
        public string GrupoEmpresarial { get; set; }
        public string Estatus { get; set; }
        public DateTime Vigencia { get; set; }
        public decimal TipoDeCambio { get; set; }
        public string Usuario { get; set; }

        private string error = string.Empty;
        private string filename = string.Empty;
        private bool hayRentados = false;
        private bool hayNoRentados = false;
        private string nombreGpoEmp = string.Empty;
        private bool esResumen = true;

        private string[] rentados;
        private int[] cantidadRentados;
        private decimal[] catastralesRentados;
        private decimal[] terrenosRentados;
        private decimal[] construccionRentados;
        private decimal[] ingresosPorRentaAnual;

        private string[] noRentados;
        private int[] cantidadNoRentados;
        private decimal[] catastralesNoRentados;
        private decimal[] terrenosNoRentados;
        private decimal[] construccionNoRentados;
        private decimal pagoPredial = 0;

        private string[] gastosNames;
        private decimal[] gastosAnuales;

        List<string> idsSubsRentadosP = new List<string>();
        List<string> idsSubsRentadosD = new List<string>();

        private List<string> subRentadosP = new List<string>();
        private List<string> municipioP = new List<string>();
        private List<string> estadoP = new List<string>();
        private List<string> direccionP = new List<string>();
        private List<string> identifP = new List<string>();
        private List<string> expCatastP = new List<string>();
        private List<string> conceptoP = new List<string>();
        private List<decimal> valCatastP = new List<decimal>();
        private List<decimal> terrenosP = new List<decimal>();
        private List<decimal> constP = new List<decimal>();
        private List<string> arrendatariosP = new List<string>();
        private List<decimal> rentaMensualP = new List<decimal>();
        private List<decimal> rentaAnualP = new List<decimal>();

        private List<string> subRentadosD = new List<string>();
        private List<string> municipioD = new List<string>();
        private List<string> estadoD = new List<string>();
        private List<string> direccionD = new List<string>();
        private List<string> identifD = new List<string>();
        private List<string> expCatastD = new List<string>();
        private List<string> conceptoD = new List<string>();
        private List<decimal> valCatastD = new List<decimal>();
        private List<decimal> terrenosD = new List<decimal>();
        private List<decimal> constD = new List<decimal>();
        private List<string> arrendatariosD = new List<string>();
        private List<decimal> rentaMensualD = new List<decimal>();
        private List<decimal> rentaAnualD = new List<decimal>();

        private List<string> subNoRentados = new List<string>();
        private List<string> municipioNR = new List<string>();
        private List<string> estadoNR = new List<string>();
        private List<string> direccionNR = new List<string>();
        private List<string> identifNR = new List<string>();
        private List<string> expCatastNR = new List<string>();
        private List<string> conceptoNR = new List<string>();
        private List<decimal> valCatastNR = new List<decimal>();
        private List<decimal> terrenosNR = new List<decimal>();
        private List<decimal> constNR = new List<decimal>();

        private List<string> gastosNombres = new List<string>();
        private List<decimal> predialesRentadosP = new List<decimal>();
        private List<decimal> predialesRentadosD = new List<decimal>();
        decimal[,] matrizGastosSub;
        decimal[,] matrizGastosSubD;
        private string[] inmobiliariasGP;
        private string[] municipioGP;
        private string[] estadoGP;
        private string[] direccionGP;
        private string[] identifGP;
        private string[] expCatastGP;
        private string[] conceptosGP;
        private decimal[] valCatastGP;
        private decimal[] terrenosGP;
        private decimal[] constGP;
        private decimal[] predialGP;
        private decimal[,] matrizGP;

        private string[] inmobiliariasGD;
        private string[] municipioGD;
        private string[] estadoGD;
        private string[] direccionGD;
        private string[] identifGD;
        private string[] expCatastGD;
        private string[] conceptosGD;
        private decimal[] valCatastGD;
        private decimal[] terrenosGD;
        private decimal[] constGD;
        private decimal[] predialGD;
        private decimal[,] matrizGD;

        public FlujoNeto(bool esResumen)
        {
            this.esResumen = esResumen;
        }

        public FlujoNeto(string idGrupo, string estatus, DateTime fechaVigencia, decimal tipoCambio, string usuario, bool esResumen)
            : this(esResumen)
        {
            GrupoEmpresarial = idGrupo;
            Estatus = estatus;
            Vigencia = fechaVigencia;
            TipoDeCambio = tipoCambio;
            Usuario = usuario;
        }

        public string generar()
        {
            if (esResumen)
                return generarResumen();
            else
                return generarDetalle();
        }

        private string generarResumen()
        {
            OnCambioProgreso(10);
            if (CancelacionPendiente)
                return "Proceso cancelado por el usuario";

            if (getDatos())
            {
                try
                {
                    //ObjetosExcel
                    Excel.Application aplicacionExcel = new Excel.Application();
                    Excel.Workbook libroExcel = aplicacionExcel.Workbooks.Add();
                    Excel.Worksheet hojaExcel = (libroExcel.ActiveSheet as Excel.Worksheet);;

                    //encabezado
                    Excel.Range rango = hojaExcel.get_Range("A1:H1");
                    rango.Merge();
                    rango.Value2 = "Resumen Inmuebles Flujo Neto " + nombreGpoEmp + " del: " + Vigencia.Year;
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
                    hojaExcel.get_Range("B2").Value2 = "Cantidad de inmuebles";
                    hojaExcel.get_Range("C2").Value2 = "Porcentaje";
                    hojaExcel.get_Range("D2").Value2 = "Valor catastral";
                    hojaExcel.get_Range("E2").Value2 = "Terreno M2";
                    hojaExcel.get_Range("F2").Value2 = "Construcción M2";
                    hojaExcel.get_Range("G2").Value2 = "Ingresos por renta anual";
                    hojaExcel.get_Range("H2").Value2 = "Productividad";

                    //variablesDeControl
                    int posicionRango = 4;
                    int posicionSubTotalRentados = 2;
                    int posicionTotal = 0;
                    int cantidadTotalInm = 1;
                    if (rentados != null && noRentados != null)
                    {
                        posicionTotal = posicionRango + rentados.Length + noRentados.Length + 3;
                        foreach(int cant in cantidadRentados)
                        {
                            cantidadTotalInm += cant;
                        }
                        foreach (int cant in cantidadNoRentados)
                        {
                            cantidadTotalInm += cant;
                        }
                    }
                    else if (rentados != null && noRentados == null)
                    {
                        posicionTotal = posicionRango + rentados.Length + 1;
                        foreach (int cant in cantidadRentados)
                        {
                            cantidadTotalInm += cant;
                        }
                    }
                    else if (rentados == null && noRentados != null)
                    {
                        posicionTotal = posicionRango + noRentados.Length + 1;
                        foreach (int cant in cantidadNoRentados)
                        {
                            cantidadTotalInm += cant;
                        }
                    }

                    OnCambioProgreso(80);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //RENTADOS
                    if (rentados != null)
                    {
                        //RENTADOS
                        rango = hojaExcel.get_Range("A3:H3");
                        rango.Merge();
                        rango.Value2 = "RENTADOS";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        posicionSubTotalRentados = posicionRango + rentados.Length;

                        //ColumnaA
                        foreach (string rent in rentados)
                        {
                            if (!string.IsNullOrEmpty(rent))
                            {
                                rango = hojaExcel.get_Range("A" + posicionRango);
                                rango.Value2 = rent;
                                rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }

                        //COLUMNA B y C
                        posicionRango = 4;
                        if (cantidadRentados != null)
                        {
                            foreach (int cnt in cantidadRentados)
                            {
                                    rango = hojaExcel.get_Range("B" + posicionRango);
                                    rango.Value2 = cnt;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                    rango = hojaExcel.get_Range("C" + posicionRango);
                                    rango.Formula = "=(B" + posicionRango + "/" + "B" + posicionTotal + ")";    
                                //rango.Formula = "=(B" + posicionRango + "/"+cantidadTotalInm +")";
                                   // rango.Formula = "=(B" + posicionRango + "/B4"+")";
                                    rango.NumberFormat = "0%"; ;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                    posicionRango++;
                            }
                        }

                        //COLUMNA D
                        posicionRango = 4;
                        if (catastralesRentados != null)
                        {
                            foreach (decimal cat in catastralesRentados)
                            {
                                rango = hojaExcel.get_Range("D" + posicionRango);
                                rango.Value2 = cat;
                                rango.NumberFormat = "$###,###,###,###,000.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }

                        //COLUMNA E
                        posicionRango = 4;
                        if (terrenosRentados != null)
                        {
                            foreach (decimal terr in terrenosRentados)
                            {
                                rango = hojaExcel.get_Range("E" + posicionRango);
                                rango.Value2 = terr;
                                rango.NumberFormat = "###,000.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }

                        //COLUMNA F
                        posicionRango = 4;
                        if (construccionRentados != null)
                        {
                            foreach (decimal con in construccionRentados)
                            {
                                rango = hojaExcel.get_Range("F" + posicionRango);
                                rango.Value2 = con;
                                rango.NumberFormat = "###,000.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }

                        //COLUMNA G y H
                        posicionRango = 4;
                        if (ingresosPorRentaAnual != null)
                        {
                            foreach (decimal ing in ingresosPorRentaAnual)
                            {
                                rango = hojaExcel.get_Range("G" + posicionRango);
                                rango.Value2 = ing;
                                rango.NumberFormat = "$###,###,###,###,000.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("H" + posicionRango);
                                rango.Formula = "=(G" + posicionRango +"/D" + posicionRango + ")";
                                rango.NumberFormat = "0.0%";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                posicionRango++;
                            }
                        }

                        //SubTotalPesos
                        rango = hojaExcel.get_Range("A" + posicionSubTotalRentados);
                        rango.Value2 = "Subtotal";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("B" + posicionSubTotalRentados);
                        rango.Formula = "=SUMA(B4:B" + (posicionSubTotalRentados - 1) + ")";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("C" + posicionSubTotalRentados);
                        rango.Formula = "=SUMA(C4:C" + (posicionSubTotalRentados - 1) + ")";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("D" + posicionSubTotalRentados);
                        rango.Formula = "=SUMA(D4:D" + (posicionSubTotalRentados - 1) + ")";
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("E" + posicionSubTotalRentados);
                        rango.Formula = "=SUMA(E4:E" + (posicionSubTotalRentados - 1) + ")";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("F" + posicionSubTotalRentados);
                        rango.Formula = "=SUMA(F4:F" + (posicionSubTotalRentados - 1) + ")";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("G" + posicionSubTotalRentados);
                        rango.Formula = "=SUMA(G4:G" + (posicionSubTotalRentados - 1) + ")";
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("H" + posicionSubTotalRentados);
                        rango.Formula = "=(G" + posicionSubTotalRentados + "/D" + posicionSubTotalRentados+ ")";
                        rango.NumberFormat = "0.0%";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    }

                    OnCambioProgreso(85);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //variablesDeControl
                    posicionRango = posicionSubTotalRentados + 2;
                    int posicionSubTotalNoRentados = posicionRango + 1;
                                        
                    //NORENTADOS
                    if (noRentados != null)
                    {
                        //NORENTADOS
                        rango = hojaExcel.get_Range("A" + (posicionSubTotalRentados + 1) + ":H" + (posicionSubTotalRentados + 1));
                        rango.Merge();
                        rango.Value2 = "NO RENTADOS";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        posicionSubTotalNoRentados = posicionRango + noRentados.Length;
                        
                        //ColumnaA
                        foreach (string rent in noRentados)
                        {
                            if (!string.IsNullOrEmpty(rent))
                            {
                                rango = hojaExcel.get_Range("A" + posicionRango);
                                rango.Value2 = rent;
                                rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        //COLUMNA B y C
                        posicionRango = posicionSubTotalRentados + 2;
                        if (cantidadNoRentados != null)
                        {
                            foreach (int cnt in cantidadNoRentados)
                            {
                                    rango = hojaExcel.get_Range("B" + posicionRango);
                                    rango.Value2 = cnt;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                    rango = hojaExcel.get_Range("C" + posicionRango);
                                    rango.Formula = "=(B" + posicionRango + "/" + "B" + posicionTotal + ")";     
                                //rango.Formula = "=(B" + posicionRango + "/ "+ cantidadTotalInm +")";
                                   // rango.Formula = "=(B" + posicionRango + "/B4"+")";
                                    rango.NumberFormat = "0%"; ;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                    posicionRango++;
                            }
                        }
                        
                        //COLUMNA D
                        posicionRango = posicionSubTotalRentados + 2;
                        if (catastralesNoRentados != null)
                        {
                            foreach (decimal cat in catastralesNoRentados)
                            {
                                rango = hojaExcel.get_Range("D" + posicionRango);
                                rango.Value2 = cat;
                                rango.NumberFormat = "$###,###,###,###,000.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        //COLUMNA E
                        posicionRango = posicionSubTotalRentados + 2;
                        if (terrenosNoRentados != null)
                        {
                            foreach (decimal terr in terrenosNoRentados)
                            {
                                rango = hojaExcel.get_Range("E" + posicionRango);
                                rango.Value2 = terr;
                                rango.NumberFormat = "###,000.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }

                        //COLUMNA F, G y H
                        posicionRango = posicionSubTotalRentados + 2;
                        if (construccionNoRentados != null)
                        {
                            foreach (decimal con in construccionNoRentados)
                            {
                                rango = hojaExcel.get_Range("F" + posicionRango);
                                rango.Value2 = con;
                                rango.NumberFormat = "###,000.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("G" + posicionRango);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("H" + posicionRango);
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                posicionRango++;
                            }
                        }

                        posicionRango = posicionSubTotalRentados + 2;
                        //SubTotalNoRentados
                        rango = hojaExcel.get_Range("A" + posicionSubTotalNoRentados);
                        rango.Value2 = "Subtotal";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("B" + posicionSubTotalNoRentados);
                        rango.Formula = "=SUMA(B"+posicionRango+":B" + (posicionSubTotalNoRentados - 1) + ")";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("C" + posicionSubTotalNoRentados);
                        rango.Formula = "=SUMA(C" + posicionRango + ":C" + (posicionSubTotalNoRentados - 1) + ")";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("D" + posicionSubTotalNoRentados);
                        rango.Formula = "=SUMA(D" + posicionRango + ":D" + (posicionSubTotalNoRentados - 1) + ")";
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("E" + posicionSubTotalNoRentados);
                        rango.Formula = "=SUMA(E" + posicionRango + ":E" + (posicionSubTotalNoRentados - 1) + ")";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("F" + posicionSubTotalNoRentados);
                        rango.Formula = "=SUMA(F" + posicionRango + ":F" + (posicionSubTotalNoRentados - 1) + ")";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("G" + posicionSubTotalNoRentados);
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("H" + posicionSubTotalNoRentados);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    }

                    OnCambioProgreso(90);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //TOTAL
                    if (rentados != null && noRentados != null)
                    {
                        rango = hojaExcel.get_Range("A" + posicionTotal + ":H" + posicionTotal);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        hojaExcel.get_Range("A" + posicionTotal).Value2 = "TOTAL";
                        hojaExcel.get_Range("B" + posicionTotal).Formula = "=SUMA(B" + posicionSubTotalRentados + "+B" + posicionSubTotalNoRentados + ")";
                        hojaExcel.get_Range("C" + posicionTotal).Formula = "=SUMA(C" + posicionSubTotalRentados + "+C" + posicionSubTotalNoRentados + ")";
                        hojaExcel.get_Range("D" + posicionTotal).Formula = "=SUMA(D" + posicionSubTotalRentados + "+D" + posicionSubTotalNoRentados + ")";
                        hojaExcel.get_Range("D" + posicionTotal).HorizontalAlignment = Excel.Constants.xlRight;
                        hojaExcel.get_Range("E" + posicionTotal).Formula = "=SUMA(E" + posicionSubTotalRentados + "+E" + posicionSubTotalNoRentados + ")";
                        hojaExcel.get_Range("F" + posicionTotal).Formula = "=SUMA(F" + posicionSubTotalRentados + "+F" + posicionSubTotalNoRentados + ")";
                        hojaExcel.get_Range("G" + posicionTotal).Formula = "=SUMA(G" + posicionSubTotalRentados + "+G" + posicionSubTotalNoRentados + ")";
                        hojaExcel.get_Range("G" + posicionTotal).HorizontalAlignment = Excel.Constants.xlRight;
                        hojaExcel.get_Range("H" + posicionTotal).Formula = "=(G" + posicionTotal + "/D" + posicionTotal + ")";
                        hojaExcel.get_Range("H" + posicionTotal).NumberFormat = "0.0%";
                    }
                    else if (rentados != null && noRentados == null)
                    {
                        rango = hojaExcel.get_Range("A" + posicionTotal + ":H" + posicionTotal);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        hojaExcel.get_Range("A" + posicionTotal).Value2 = "TOTAL";
                        hojaExcel.get_Range("B" + posicionTotal).Formula = "=SUMA(B" + posicionSubTotalRentados + "+0)";
                        hojaExcel.get_Range("C" + posicionTotal).Formula = "=SUMA(C" + posicionSubTotalRentados + "+0)";
                        hojaExcel.get_Range("D" + posicionTotal).Formula = "=SUMA(D" + posicionSubTotalRentados + "+0)";
                        hojaExcel.get_Range("D" + posicionTotal).HorizontalAlignment = Excel.Constants.xlRight;
                        hojaExcel.get_Range("E" + posicionTotal).Formula = "=SUMA(E" + posicionSubTotalRentados + "+0)";
                        hojaExcel.get_Range("F" + posicionTotal).Formula = "=SUMA(F" + posicionSubTotalRentados + "+0)";
                        hojaExcel.get_Range("G" + posicionTotal).Formula = "=SUMA(G" + posicionSubTotalRentados + "+0)";
                        hojaExcel.get_Range("G" + posicionTotal).HorizontalAlignment = Excel.Constants.xlRight;
                        hojaExcel.get_Range("H" + posicionTotal).Formula = "=(G" + posicionTotal + "/D" + posicionTotal + ")";
                        hojaExcel.get_Range("H" + posicionTotal).NumberFormat = "0.0%";
                    }
                    else if (rentados == null && noRentados != null)
                    {
                        rango = hojaExcel.get_Range("A" + posicionTotal + ":H" + posicionTotal);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        hojaExcel.get_Range("A" + posicionTotal).Value2 = "TOTAL";
                        hojaExcel.get_Range("B" + posicionTotal).Formula = "=SUMA(0+B" + posicionSubTotalNoRentados + ")";
                        hojaExcel.get_Range("C" + posicionTotal).Formula = "=SUMA(0+C" + posicionSubTotalNoRentados + ")";
                        hojaExcel.get_Range("D" + posicionTotal).Formula = "=SUMA(0+D" + posicionSubTotalNoRentados + ")";
                        hojaExcel.get_Range("D" + posicionTotal).HorizontalAlignment = Excel.Constants.xlRight;
                        hojaExcel.get_Range("E" + posicionTotal).Formula = "=SUMA(0+E" + posicionSubTotalNoRentados + ")";
                        hojaExcel.get_Range("F" + posicionTotal).Formula = "=SUMA(0+F" + posicionSubTotalNoRentados + ")";
                        hojaExcel.get_Range("G" + posicionTotal).Formula = "=SUMA(0+G" + posicionSubTotalNoRentados + ")";
                        hojaExcel.get_Range("G" + posicionTotal).HorizontalAlignment = Excel.Constants.xlRight;
                        hojaExcel.get_Range("H" + posicionTotal).Formula = "=(G" + posicionTotal + "/D" + posicionTotal + ")";
                        hojaExcel.get_Range("H" + posicionTotal).NumberFormat = "0.0%";
                    }

                    if (GrupoEmpresarial != "Todos" && Estatus == "Todos")
                    {
                        hojaExcel.get_Range("A4").EntireRow.Hidden = true;
                        hojaExcel.get_Range("A7").EntireRow.Hidden = true;
                    }
                    else if (GrupoEmpresarial != "Todos" && Estatus != "Todos")
                    {
                        hojaExcel.get_Range("A4").EntireRow.Hidden = true;
                    }

                    //GASTOSENCAB
                    posicionRango = posicionTotal + 3;
                    
                    rango = hojaExcel.get_Range("A" + posicionRango + ":H" + posicionRango);
                    rango.Merge();
                    rango.Value2 = "GASTOS";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    
                    posicionRango++;

                    rango = hojaExcel.get_Range("A" + posicionRango + ":H" + posicionRango);
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    hojaExcel.get_Range("A" + posicionRango).Value2 = "Concepto";
                    rango = hojaExcel.get_Range("B" + posicionRango + ":F" + posicionRango);
                    rango.Merge();
                    rango.Value2 = "Descripción";
                    hojaExcel.get_Range("G" + posicionRango).Value2 = "Anual";
                    hojaExcel.get_Range("H" + posicionRango).Value2 = "Representación";

                    posicionRango = posicionTotal + 5;
                    int posicionTotalGastos = 0;
                    int numGastos = 0;
                    if (gastosNames == null)
                    {
                        posicionTotalGastos = posicionTotal + 6;
                        numGastos = 1;
                    }
                    else
                    {
                        posicionTotalGastos = posicionTotal + 6 + gastosNames.Length;
                        numGastos = gastosNames.Length + 1;
                    }
                    //Columna A Gastos                
                    rango = hojaExcel.get_Range("A" + posicionRango);
                    rango.Value2 = "Prediales";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    rango = hojaExcel.get_Range("B" + posicionRango + ":F" + posicionRango);
                    rango.Merge();
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    rango = hojaExcel.get_Range("G" + posicionRango);
                    rango.Value2 = pagoPredial;
                    rango.NumberFormat = "$###,##0.00";
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    posicionRango++;

                    if (gastosNames != null)
                    {
                        foreach (string gto in gastosNames)
                        {
                            if (!string.IsNullOrEmpty(gto))
                            {
                                rango = hojaExcel.get_Range("A" + posicionRango);
                                rango.Value2 = gto;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("B" + posicionRango + ":F" + posicionRango);
                                rango.Merge();
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                posicionRango++;
                            }                            
                        }

                        //ColumnaG Gastos
                        posicionRango = posicionTotal + 6;
                        if (gastosAnuales != null)
                        {
                            foreach (decimal gtoAn in gastosAnuales)
                            {
                                rango = hojaExcel.get_Range("G" + posicionRango);
                                rango.Value2 = gtoAn;
                                rango.NumberFormat = "$###,##0.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                    }

                    OnCambioProgreso(95);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    rango = hojaExcel.get_Range("A" + posicionTotalGastos + ":F" + posicionTotalGastos);
                    rango.Merge();
                    rango.Value2 = "TOTAL";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    posicionRango = posicionTotal + 5;

                    rango = hojaExcel.get_Range("G" + posicionTotalGastos);
                    rango.Formula = "=SUMA(G" + posicionRango + ":G" + (posicionTotalGastos - 1) + ")";
                    rango.NumberFormat = "$###,##0.00";
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    for (int i = 1; i <= numGastos; i++)
                    {
                        posicionRango = posicionTotal + 4 + i;
                        rango = hojaExcel.get_Range("H" + posicionRango);
                        rango.Formula = "=(G" + posicionRango + "/G" + posicionTotalGastos + ")";
                        rango.NumberFormat = "0.0%";
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
 
                    }

                    posicionRango = posicionTotal + 5;

                    rango = hojaExcel.get_Range("H" + posicionTotalGastos);
                    rango.Formula = "=SUMA(H" + posicionRango + ":H" + (posicionTotalGastos - 1) + ")";
                    rango.NumberFormat = "0.00%";
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //FLUJONETO
                    posicionRango = posicionTotalGastos + 3;

                    rango = hojaExcel.get_Range("A" + posicionRango + ":H" + posicionRango);
                    rango.Merge();
                    rango.Value2 = "FLUJO NETO";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    posicionRango++;

                    hojaExcel.get_Range("A2:H2").Copy();
                    rango = hojaExcel.get_Range("A" + posicionRango + ":H" + posicionRango);
                    rango.PasteSpecial();
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    posicionRango++;

                    //COLUMNA A FlujoNeto
                    rango = hojaExcel.get_Range("A" + posicionRango);
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Value2 = "Ingresos Totales";
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    hojaExcel.get_Range("B" + posicionTotal + ":H" + posicionTotal).Copy();
                    rango = hojaExcel.get_Range("B" + posicionRango + ":H" + posicionRango);
                    rango.PasteSpecial(Excel.XlPasteType.xlPasteValuesAndNumberFormats);
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    hojaExcel.get_Range("D" + posicionRango).HorizontalAlignment = Excel.Constants.xlRight;
                    hojaExcel.get_Range("G" + posicionRango).HorizontalAlignment = Excel.Constants.xlRight;

                    posicionRango++;

                    //COLUMNA A FlujoNeto
                    rango = hojaExcel.get_Range("A" + posicionRango);
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    rango.Value2 = "Gastos Totales";
                    rango = hojaExcel.get_Range("A" + posicionRango + ":H" + posicionRango);
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    hojaExcel.get_Range("G" + posicionTotalGastos).Copy();
                    hojaExcel.get_Range("G" + posicionRango).PasteSpecial(Excel.XlPasteType.xlPasteValuesAndNumberFormats);

                    posicionRango++;

                    rango = hojaExcel.get_Range("A" + posicionRango + ":F" + posicionRango);
                    rango.Merge();
                    rango.Value2 = "Flujo Neto Anual Total";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    rango = hojaExcel.get_Range("G" + posicionRango);
                    rango.Formula = "=(G" + (posicionRango - 2) + "-G" + (posicionRango - 1) + ")";
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    rango = hojaExcel.get_Range("H" + posicionRango);
                    rango.Formula = "=(G" + posicionRango + "/D" + (posicionRango - 2) + ")";
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.NumberFormat = "0.0%";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    posicionRango++;

                    //Tipo de cambio
                    rango = hojaExcel.get_Range("A" + posicionRango);
                    rango.Value2 = "Tipo de cambio: " + TipoDeCambio;
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    posicionRango++;

                    //Generado por
                    rango = hojaExcel.get_Range("A" + posicionRango);
                    rango.Value2 = "Reporte generado por: " + Usuario;
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    OnCambioProgreso(100);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //AutoAjustar
                    hojaExcel.get_Range("A2:Q2").Columns.AutoFit();
                    hojaExcel.get_Range("A2:A1000").Columns.AutoFit();
                    hojaExcel.get_Range("B2:B1000").Columns.AutoFit();
                    hojaExcel.get_Range("C2:C1000").Columns.AutoFit();
                    hojaExcel.get_Range("D2:D1000").Columns.AutoFit();
                    hojaExcel.get_Range("E2:E1000").Columns.AutoFit();
                    hojaExcel.get_Range("F2:F1000").Columns.AutoFit();
                    hojaExcel.get_Range("G2:G1000").Columns.AutoFit();
                    hojaExcel.get_Range("H2:G1000").Columns.AutoFit();

                    //MostrarReporte
                    filename = GestorReportes.Properties.Settings.Default.RutaRepositorio + @"ResumenFlujoNeto_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + ".xlsx";
                    libroExcel.SaveAs(filename);
                    aplicacionExcel.Visible = true;

                }
                catch
                {
                    error = "- Error en Interop Excel";
                }
            }
            return error;
        }

        private bool getDatos()
        {
            try
            {
                Inmobiliaria inmobiliaria = new Inmobiliaria();
                DataTable dtGpo = inmobiliaria.getDtGrupoEmpresarial(GrupoEmpresarial);
                if (GrupoEmpresarial == "Todos")
                    nombreGpoEmp = "Todos";
                else
                {                    
                    if (dtGpo.Rows.Count > 0)
                    {
                        nombreGpoEmp = dtGpo.Rows[0]["P0002_NOMBRE"].ToString();
                    }
                    else
                    {
                        error = "- Error al obtener el grupo empresarial";
                        return false;
                    }
                }

                OnCambioProgreso(20);
                if (CancelacionPendiente)
                {
                    error = "Proceso cancelado por el usuario";
                    return false;
                }

                List<string> subConjATomarPrediales = new List<string>();
                if (Estatus == "Todos" || Estatus == "Rentados")
                {
                    DataTable dtRentados = inmobiliaria.getDtSubRentadosFlujoNeto(GrupoEmpresarial, Vigencia);
                    if (dtRentados.Rows.Count > 0)
                    {
                        hayRentados = true;

                        rentados = new string[dtGpo.Rows.Count];
                        cantidadRentados = new int[dtGpo.Rows.Count];
                        catastralesRentados = new decimal[dtGpo.Rows.Count];
                        terrenosRentados = new decimal[dtGpo.Rows.Count];
                        construccionRentados = new decimal[dtGpo.Rows.Count];
                        ingresosPorRentaAnual = new decimal[dtGpo.Rows.Count];

                        int porcentaje = 20;
                        decimal factor = 20 / dtGpo.Rows.Count;
                        factor = factor >= 1 ? factor : 1;

                        int contadorGpos = 0;
                        foreach (DataRow dr in dtGpo.Rows)
                        {
                            if (porcentaje <= 40)
                                porcentaje += Convert.ToInt32(factor);
                            OnCambioProgreso(porcentaje <= 40 ? porcentaje : 40);
                            if (CancelacionPendiente)
                            {
                                error = "Proceso cancelado por el usuario";
                                return false;
                            }

                            rentados[contadorGpos] = dr["P0002_NOMBRE"].ToString().Trim();

                            DataTable dtSubRent = inmobiliaria.getDtSubRentadosFlujoNeto(dr["P0001_ID_GRUPO"].ToString().Trim(), Vigencia);
                            int contadorCantidad = 0;                                                     
                            if (dtSubRent.Rows.Count > 0)
                            {
                                
                                decimal catast = 0;
                                decimal terrM2 = 0;
                                decimal constM2 = 0;
                                List<string> subId = new List<string>();
                                foreach (DataRow row in dtSubRent.Rows)
                                {
                                    if (!subConjATomarPrediales.Contains(row["P1801_ID_SUBCONJUNTO"].ToString()))
                                        subConjATomarPrediales.Add(row["P1801_ID_SUBCONJUNTO"].ToString());
                                    if (!subId.Contains(row["P1801_ID_SUBCONJUNTO"].ToString()))
                                    {
                                        subId.Add(row["P1801_ID_SUBCONJUNTO"].ToString());
                                        contadorCantidad++;
                                        DataTable dtInmubPorSub = inmobiliaria.getDtInmueblesPorSubConj(row["P1801_ID_SUBCONJUNTO"].ToString());
                                        foreach (DataRow rowInmb in dtInmubPorSub.Rows)
                                        {
                                            if (!string.IsNullOrEmpty(rowInmb["P1922_A_MIN_ING"].ToString()))
                                                catast += Decimal.Round(Convert.ToDecimal(rowInmb["P1922_A_MIN_ING"].ToString().Trim()), 2);
                                            if (!string.IsNullOrEmpty(rowInmb["P1926_CIST_ING"].ToString()))
                                                terrM2 += Decimal.Round(Convert.ToDecimal(rowInmb["P1926_CIST_ING"].ToString()), 2);
                                            if (!string.IsNullOrEmpty(rowInmb["CAMPO_NUM1"].ToString()))
                                                constM2 += Decimal.Round(Convert.ToDecimal(rowInmb["CAMPO_NUM1"].ToString()), 2);
                                        }
                                    }
                                }
                                catastralesRentados[contadorGpos] = catast;
                                terrenosRentados[contadorGpos] = terrM2;
                                construccionRentados[contadorGpos] = constM2;
                            }
                            else
                            {
                                catastralesRentados[contadorGpos] = 0;
                                terrenosRentados[contadorGpos] = 0;
                                construccionRentados[contadorGpos] = 0;
                            }
                            cantidadRentados[contadorGpos] = contadorCantidad;

                            DataTable dtContratosHist = inmobiliaria.getDtContratosPorGrupoFlujoNeto(dr["P0001_ID_GRUPO"].ToString().Trim(), Vigencia);
                            string idContrat = string.Empty;
                            decimal importNuevo = 0;
                            decimal importePorContrato = 0;
                            DateTime fechaIncremento = DateTime.Now.AddYears(-50);//2.3.2.30
                            string baseParaAumento = "INPC";
                            DateTime finVigNvo = DateTime.Now.AddYears(-50);
                            DateTime iniVigNvo = DateTime.Now.AddYears(-50);
                            DateTime vigIni = new DateTime(Vigencia.Year, 1, 1);
                            DateTime vigFin = new DateTime(Vigencia.Year, 12, 31);
                            List<string> contratosYaContados = new List<string>();
                            if (dtContratosHist.Rows.Count > 0)
                            {                          
                                foreach (DataRow rowContrat in dtContratosHist.Rows)
                                {
                                    if (!contratosYaContados.Contains(rowContrat["P0401_ID_CONTRATO"].ToString()))
                                    {
                                        idContrat = rowContrat["P0401_ID_CONTRATO"].ToString();
                                        DataRow[] rows = dtContratosHist.Select("P0401_ID_CONTRATO = '" + rowContrat["P0401_ID_CONTRATO"].ToString() + "'", "P5976_FECHA_HORA_UPDT ASC");
                                        importNuevo = Convert.ToDecimal(rows[rows.Length - 1]["P5941_IMP_ACTUAL_NVO"].ToString());
                                        iniVigNvo = Convert.ToDateTime(rows[rows.Length - 1]["P5911_INICIO_VIG_NVO"]);
                                        finVigNvo = Convert.ToDateTime(rows[rows.Length - 1]["P5913_FIN_VIG_NVO"]);
                                        if (!string.IsNullOrEmpty(rows[rows.Length - 1]["P0432_FECHA_AUMENTO"].ToString()))
                                            fechaIncremento = Convert.ToDateTime(rows[rows.Length - 1]["P0432_FECHA_AUMENTO"]);
                                        if (!string.IsNullOrEmpty(rows[rows.Length - 1]["P0441_BASE_PARA_AUMENTO"].ToString()))
                                                baseParaAumento = rows[rows.Length - 1]["P0441_BASE_PARA_AUMENTO"].ToString();
                                        if (fechaIncremento < vigFin && fechaIncremento >= DateTime.Now && baseParaAumento.Trim() == "PRC") //2.3.2.30
                                        {
                                            int mesesAnterior = 0;
                                            int mesesNuevo = 0;
                                            decimal importeConAumento = 0;
                                            decimal aumento = 1;
                                            try { aumento = (Convert.ToDecimal(rows[rows.Length - 1]["P0415_AUMENTO_ANUAL"].ToString()) / 100) + 1; }
                                            catch { aumento = 1; }
                                            importeConAumento = importNuevo * aumento;
                                            if (iniVigNvo <= vigIni && finVigNvo >= vigFin)
                                            {
                                                mesesNuevo = 13 - fechaIncremento.Month;
                                                mesesAnterior = 12 - mesesNuevo;
                                                if (mesesAnterior > 0)
                                                {
                                                    if (rows[rows.Length - 1]["P0407_MONEDA_FACT"].ToString() == "D")
                                                        importePorContrato += Decimal.Round((importNuevo * mesesAnterior) * TipoDeCambio, 2);
                                                    else
                                                        importePorContrato += Decimal.Round((importNuevo * mesesAnterior), 2);
                                                }
                                                if (rows[rows.Length - 1]["P0407_MONEDA_FACT"].ToString() == "D")
                                                    importePorContrato += Decimal.Round((importeConAumento * mesesNuevo) * TipoDeCambio, 2);
                                                else
                                                    importePorContrato += Decimal.Round((importeConAumento * mesesNuevo), 2);
                                            }
                                            else if (iniVigNvo <= vigIni && finVigNvo >= vigIni && finVigNvo <= vigFin)
                                            {
                                                mesesNuevo = (finVigNvo.Month + 1) - fechaIncremento.Month;
                                                mesesAnterior = finVigNvo.Month - mesesNuevo;
                                                if (mesesAnterior > 0)
                                                {
                                                    if (rows[rows.Length - 1]["P0407_MONEDA_FACT"].ToString() == "D")
                                                        importePorContrato += Decimal.Round((importNuevo * mesesAnterior) * TipoDeCambio, 2);
                                                    else
                                                        importePorContrato += Decimal.Round((importNuevo * mesesAnterior), 2);
                                                }
                                                if (rows[rows.Length - 1]["P0407_MONEDA_FACT"].ToString() == "D")
                                                    importePorContrato += Decimal.Round((importeConAumento * mesesNuevo) * TipoDeCambio, 2);
                                                else
                                                    importePorContrato += Decimal.Round((importeConAumento * mesesNuevo), 2);
                                            }
                                            else if (iniVigNvo >= vigIni && iniVigNvo <= vigFin && finVigNvo >= vigFin)
                                            {
                                                int meses = 13 - iniVigNvo.Month;
                                                mesesNuevo = 13 - fechaIncremento.Month;
                                                mesesAnterior = meses - mesesNuevo;
                                                if (mesesAnterior > 0)
                                                {
                                                    if (rows[rows.Length - 1]["P0407_MONEDA_FACT"].ToString() == "D")
                                                        importePorContrato += Decimal.Round((importNuevo * mesesAnterior) * TipoDeCambio, 2);
                                                    else
                                                        importePorContrato += Decimal.Round((importNuevo * mesesAnterior), 2);
                                                }
                                                if (rows[rows.Length - 1]["P0407_MONEDA_FACT"].ToString() == "D")
                                                    importePorContrato += Decimal.Round((importeConAumento * mesesNuevo) * TipoDeCambio, 2);
                                                else
                                                    importePorContrato += Decimal.Round((importeConAumento * mesesNuevo), 2);
                                            }
                                            else
                                                importePorContrato += 0;                                   
                                        }
                                        else
                                        {
                                            if (iniVigNvo <= vigIni && finVigNvo >= vigFin)
                                            {
                                                if (rows[rows.Length - 1]["P0407_MONEDA_FACT"].ToString() == "D")
                                                {
                                                    importePorContrato += Decimal.Round((importNuevo * 12) * TipoDeCambio, 2);
                                                }
                                                else
                                                    importePorContrato += Decimal.Round((importNuevo * 12), 2);
                                            }
                                            else if (iniVigNvo <= vigIni && finVigNvo >= vigIni && finVigNvo <= vigFin)
                                            {
                                                if (rows[rows.Length - 1]["P0407_MONEDA_FACT"].ToString() == "D")
                                                {
                                                    importePorContrato += Decimal.Round((importNuevo * finVigNvo.Month) * TipoDeCambio, 2);
                                                }
                                                else
                                                    importePorContrato += Decimal.Round((importNuevo * finVigNvo.Month), 2);
                                            }
                                            else if (iniVigNvo >= vigIni && iniVigNvo <= vigFin && finVigNvo >= vigFin)
                                            {
                                                int meses = 13 - iniVigNvo.Month;
                                                if (rows[rows.Length - 1]["P0407_MONEDA_FACT"].ToString() == "D")
                                                {
                                                    importePorContrato += Decimal.Round((importNuevo * meses) * TipoDeCambio, 2);
                                                }
                                                else
                                                    importePorContrato += Decimal.Round((importNuevo * meses), 2);
                                            }
                                            else
                                                importePorContrato += 0;
                                        }
                                        contratosYaContados.Add(idContrat);
                                    }
                                }
                            }
                            ingresosPorRentaAnual[contadorGpos] = importePorContrato;
                            contadorGpos++;
                        }
                    }
                }
                if (Estatus == "Todos" || Estatus == "No Rentados")
                {
                    DataTable dtNoRentados = inmobiliaria.getDtSubConjuntosNoRentadosPorGpoEmp(GrupoEmpresarial);
                    if (dtNoRentados.Rows.Count > 0) 
                    {
                        hayNoRentados = true;

                        noRentados = new string[dtGpo.Rows.Count];
                        cantidadNoRentados = new int[dtGpo.Rows.Count];
                        catastralesNoRentados = new decimal[dtGpo.Rows.Count];
                        terrenosNoRentados = new decimal[dtGpo.Rows.Count];
                        construccionNoRentados = new decimal[dtGpo.Rows.Count];

                        int porcentaje = 40;
                        decimal factor = 20 / dtGpo.Rows.Count;
                        factor = factor >= 1 ? factor : 1;

                        int contadorGpos = 0;
                        foreach (DataRow dr in dtGpo.Rows)
                        {
                            if (porcentaje <= 60)
                                porcentaje += Convert.ToInt32(factor);
                            OnCambioProgreso(porcentaje <= 60 ? porcentaje : 60);
                            if (CancelacionPendiente)
                            {
                                error = "Proceso cancelado por el usuario";
                                return false;
                            }

                            noRentados[contadorGpos] = dr["P0002_NOMBRE"].ToString().Trim();

                            DataTable dtSubNoRent = inmobiliaria.getDtSubConjuntosNoRentadosPorGpoEmp(dr["P0001_ID_GRUPO"].ToString().Trim());
                            cantidadNoRentados[contadorGpos] = dtSubNoRent.Rows.Count;

                            if (dtSubNoRent.Rows.Count > 0)
                            {
                                decimal catast = 0;
                                decimal terrM2 = 0;
                                decimal constM2 = 0;
                                foreach (DataRow row in dtSubNoRent.Rows)
                                {
                                    if (!subConjATomarPrediales.Contains(row["P1801_ID_SUBCONJUNTO"].ToString()))
                                        subConjATomarPrediales.Add(row["P1801_ID_SUBCONJUNTO"].ToString());
                                    DataTable dtInmubPorSub = inmobiliaria.getDtInmueblesPorSubConj(row["P1801_ID_SUBCONJUNTO"].ToString());
                                    foreach (DataRow rowInmb in dtInmubPorSub.Rows)
                                    {
                                        if (!string.IsNullOrEmpty(rowInmb["P1922_A_MIN_ING"].ToString()))
                                            catast += Decimal.Round(Convert.ToDecimal(rowInmb["P1922_A_MIN_ING"].ToString().Trim()), 2);
                                        if (!string.IsNullOrEmpty(rowInmb["P1926_CIST_ING"].ToString()))
                                            terrM2 += Decimal.Round(Convert.ToDecimal(rowInmb["P1926_CIST_ING"].ToString()), 2);
                                        if (!string.IsNullOrEmpty(rowInmb["CAMPO_NUM1"].ToString()))
                                            constM2 += Decimal.Round(Convert.ToDecimal(rowInmb["CAMPO_NUM1"].ToString()), 2);
                                    }
                                }
                                catastralesNoRentados[contadorGpos] = catast;
                                terrenosNoRentados[contadorGpos] = terrM2;
                                construccionNoRentados[contadorGpos] = constM2;
                            }
                            else
                            {
                                catastralesNoRentados[contadorGpos] = 0;
                                terrenosNoRentados[contadorGpos] = 0;
                                construccionNoRentados[contadorGpos] = 0;
                            }                            
                            contadorGpos++;
                        }                                               
                    }
                }
                //pruebapredial
                if (subConjATomarPrediales.Count > 0)
                {
                    decimal acumulPrd = 0;
                    foreach (string s in subConjATomarPrediales)
                    {
                        DataTable dtPredialPorSub = inmobiliaria.getDtPredialesPorSubConjunto(s);
                        if (dtPredialPorSub.Rows.Count > 0)
                        {
                            foreach(DataRow dr in dtPredialPorSub.Rows)
                            {
                                if(!string.IsNullOrEmpty(dr["CAMPO_NUM2"].ToString()))
                                    acumulPrd += Convert.ToDecimal(dr["CAMPO_NUM2"].ToString());
                            }
                        }
                    }
                    pagoPredial = Decimal.Round(acumulPrd, 2);
                }
                if (!hayRentados && !hayNoRentados)
                {
                    error = "- Error al obtener los datos del reporte, no se encontraron registros para los criterios seleccionados";
                    return false;
                }

                OnCambioProgreso(70);
                if (CancelacionPendiente)
                {
                    error = "Proceso cancelado por el usuario";
                    return false;
                }

                DataTable dtGastosNombres = inmobiliaria.getDtGastosNombres();
                if (dtGastosNombres.Rows.Count > 0)
                {
                    gastosNames = new string[dtGastosNombres.Rows.Count];
                    gastosAnuales = new decimal[dtGastosNombres.Rows.Count];

                    int contadorGastos = 0;
                    foreach (DataRow rowGtos in dtGastosNombres.Rows) 
                    {
                        gastosNames[contadorGastos] = rowGtos["DESCR_CAT"].ToString();
                        decimal gtos = 0;
                        DataTable dtGtosConj = inmobiliaria.getDtGastosDeConjuntosDeGpoEmp(GrupoEmpresarial, rowGtos["DESCR_CAT"].ToString());
                        if (dtGtosConj.Rows.Count > 0)
                        {
                            foreach (DataRow drGto in dtGtosConj.Rows)
                            {
                                if (!string.IsNullOrEmpty(drGto["CAMPO_NUM4"].ToString()))
                                {
                                    if (drGto["CAMPO3"].ToString().Trim() == "D")
                                        gtos += Convert.ToDecimal(drGto["CAMPO_NUM4"].ToString()) * TipoDeCambio;
                                    else
                                        gtos += Convert.ToDecimal(drGto["CAMPO_NUM4"].ToString());
                                }
                                else
                                    gtos += 0;
                            }
                        }
                        DataTable dtGtosInmbs = inmobiliaria.getDtGastosDeInmueblesDeGpoEmp(GrupoEmpresarial, rowGtos["DESCR_CAT"].ToString());
                        if (dtGtosInmbs.Rows.Count > 0)
                        {
                            foreach (DataRow drGto in dtGtosInmbs.Rows)
                            {
                                if (!string.IsNullOrEmpty(drGto["CAMPO_NUM4"].ToString()))
                                {
                                    if (drGto["CAMPO3"].ToString().Trim() == "D")
                                        gtos += Convert.ToDecimal(drGto["CAMPO_NUM4"].ToString()) * TipoDeCambio;
                                    else
                                        gtos += Convert.ToDecimal(drGto["CAMPO_NUM4"].ToString());
                                }
                                else
                                    gtos += 0;
                            }
                        }
                        gastosAnuales[contadorGastos] = Decimal.Round(gtos, 2);
                        contadorGastos++;
                    }
                }

                OnCambioProgreso(75);
                if (CancelacionPendiente)
                {
                    error = "Proceso cancelado por el usuario";
                    return false;
                }
                /*DataTable dtPrediales = inmobiliaria.getDTPredialesPorGpoEmp(GrupoEmpresarial);
                if (dtPrediales.Rows.Count > 0)
                {
                    foreach (DataRow rowPredial in dtPrediales.Rows)
                    {
                        if(!string.IsNullOrEmpty(rowPredial["CAMPO_NUM2"].ToString()))
                            pagoPredial += Decimal.Round(Convert.ToDecimal(rowPredial["CAMPO_NUM2"].ToString()), 2);
                    }
                }*/
                return true;
            }
            catch
            {
                error = "- Error al establecer los datos del reporte";
                return false;
            }
        }

        private string generarDetalle()
        {
            OnCambioProgreso(5);
            if (CancelacionPendiente)
                return "Proceso cancelado por el usuario";

            if(getDetalles())
            {
                try
                {
                    //ObjetosExcel
                    Excel.Application aplicacionExcel = new Excel.Application();
                    Excel.Workbook libroExcel = aplicacionExcel.Workbooks.Add();
                    Excel.Worksheet hojaExcel = (libroExcel.ActiveSheet as Excel.Worksheet);;

                    //encabezado
                    Excel.Range rango = hojaExcel.get_Range("A1:O1");
                    rango.Merge();
                    rango.Value2 = "Reporte Detalle de Flujo Neto " + nombreGpoEmp + " del: " + Vigencia.Year;
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 18;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //subencabezados
                    rango = hojaExcel.get_Range("A2:O2");
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    hojaExcel.get_Range("A2").Value2 = "Propietario";
                    hojaExcel.get_Range("B2").Value2 = "Municipio";
                    hojaExcel.get_Range("C2").Value2 = "Estado";
                    hojaExcel.get_Range("D2").Value2 = "Dirección";
                    hojaExcel.get_Range("E2").Value2 = "Reg.";
                    hojaExcel.get_Range("F2").Value2 = "Exp. Catastral";
                    hojaExcel.get_Range("G2").Value2 = "Concepto";
                    hojaExcel.get_Range("H2").Value2 = "Val. Catastral";
                    hojaExcel.get_Range("I2").Value2 = "Terreno M2";
                    hojaExcel.get_Range("J2").Value2 = "Construcción M2";
                    hojaExcel.get_Range("K2").Value2 = "Arrendatario";
                    hojaExcel.get_Range("L2").Value2 = "Renta Mensual";
                    hojaExcel.get_Range("M2").Value2 = "Renta Anual";
                    hojaExcel.get_Range("N2").Value2 = "Productividad";
                    hojaExcel.get_Range("O2").Value2 = "Observaciones";

                    int posicionRango = 4;
                    int posicionTotalAP = 2;
                    int posicionTotalAD = 2;
                    int posicionTotalNR = 2;

                    //ARRENDAMIENTO EN PESOS
                    if (subRentadosP.Count > 0)
                    {
                        posicionTotalAP = posicionRango + subRentadosP.Count;

                        //RENTADOS
                        rango = hojaExcel.get_Range("A3:O3");
                        rango.Merge();
                        rango.Value2 = "ARRENDAMIENTO EN PESOS";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        //ColumnaA
                        foreach (string rent in subRentadosP)
                        {
                            if (!string.IsNullOrEmpty(rent))
                            {
                                rango = hojaExcel.get_Range("A" + posicionRango);
                                rango.Value2 = rent;
                                rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }

                        posicionRango = 4;
                        //COLUMNAB
                        if (municipioP.Count > 0)
                        {
                            foreach (string muniP in municipioP)
                            {
                                if (!string.IsNullOrEmpty(muniP))
                                {
                                    rango = hojaExcel.get_Range("B" + posicionRango);
                                    rango.Value2 = muniP;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        posicionRango = 4;
                        //COLUMNAC
                        if (estadoP.Count > 0)
                        {
                            foreach (string estad in estadoP)
                            {
                                if (!string.IsNullOrEmpty(estad))
                                {
                                    rango = hojaExcel.get_Range("C" + posicionRango);
                                    rango.Value2 = estad;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        posicionRango = 4;
                        //COLUMNAD
                        if (direccionP.Count > 0)
                        {
                            foreach (string dir in direccionP)
                            {
                                if (!string.IsNullOrEmpty(dir))
                                {
                                    rango = hojaExcel.get_Range("D" + posicionRango);
                                    rango.Value2 = dir;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        posicionRango = 4;
                        //COLUMNAE
                        if (identifP.Count > 0)
                        {
                            foreach (string ids in identifP)
                            {
                                if (!string.IsNullOrEmpty(ids))
                                {
                                    rango = hojaExcel.get_Range("E" + posicionRango);
                                    rango.Value2 = ids;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        posicionRango = 4;
                        //COLUMNAF
                        if (expCatastP.Count > 0)
                        {
                            foreach (string exp in expCatastP)
                            {
                                if (!string.IsNullOrEmpty(exp))
                                {
                                    rango = hojaExcel.get_Range("F" + posicionRango);
                                    rango.Value2 = exp;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        posicionRango = 4;
                        //COLUMNAG
                        if (conceptoP.Count > 0)
                        {
                            foreach (string conc in conceptoP)
                            {
                                if (!string.IsNullOrEmpty(conc))
                                {
                                    rango = hojaExcel.get_Range("G" + posicionRango);
                                    rango.Value2 = conc;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        posicionRango = 4;
                        //COLUMNAH
                        if (valCatastP.Count > 0)
                        {
                            foreach (decimal valCat in valCatastP)
                            {
                                rango = hojaExcel.get_Range("H" + posicionRango);
                                rango.Value2 = valCat;
                                rango.NumberFormat = "$###,###,###,###,##0.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                               
                            }
                        }

                        posicionRango = 4;
                        //COLUMNAI
                        if (terrenosP.Count > 0)
                        {
                            foreach (decimal terr in terrenosP)
                            {
                                rango = hojaExcel.get_Range("I" + posicionRango);
                                rango.Value2 = terr;
                                rango.NumberFormat = "###,###,###,###,##0.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;

                            }
                        }

                        posicionRango = 4;
                        //COLUMNAJ
                        if (constP.Count > 0)
                        {
                            foreach (decimal constru in constP)
                            {
                                rango = hojaExcel.get_Range("J" + posicionRango);
                                rango.Value2 = constru;
                                rango.NumberFormat = "###,###,###,###,##0.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }

                        posicionRango = 4;
                        //COLUMNAK
                        if (arrendatariosP.Count > 0)
                        {
                            foreach (string arre in arrendatariosP)
                            {
                                if (!string.IsNullOrEmpty(arre))
                                {
                                    rango = hojaExcel.get_Range("K" + posicionRango);
                                    rango.Value2 = arre;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        posicionRango = 4;
                        //COLUMNAL
                        if (rentaMensualP.Count > 0)
                        {
                            foreach (decimal rm in rentaMensualP)
                            {
                                rango = hojaExcel.get_Range("L" + posicionRango);
                                rango.Value2 = rm;
                                rango.NumberFormat = "$###,###,###,###,##0.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }

                        posicionRango = 4;
                        //COLUMNAM
                        if (rentaAnualP.Count > 0)
                        {
                            foreach (decimal ra in rentaAnualP)
                            {
                                rango = hojaExcel.get_Range("M" + posicionRango);
                                rango.Value2 = ra;
                                rango.NumberFormat = "$###,###,###,###,##0.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("N" + posicionRango);
                                rango.Formula = "=(M" + posicionRango + "/H" + posicionRango + ")";
                                rango.NumberFormat = "0.0%";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("O" + posicionRango);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                posicionRango++;
                            }
                        }

                        //TOTALPESOS
                        rango = hojaExcel.get_Range("A" + posicionTotalAP + ":O" + posicionTotalAP);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("A" + posicionTotalAP);
                        rango.Value2 = "TOTAL";

                        rango = hojaExcel.get_Range("H" + posicionTotalAP);
                        rango.Formula = "=SUMA(H4:H" + (posicionTotalAP -1);
                        rango.HorizontalAlignment = Excel.Constants.xlRight;

                        rango = hojaExcel.get_Range("I" + posicionTotalAP);
                        rango.Formula = "=SUMA(I4:I" + (posicionTotalAP - 1);

                        rango = hojaExcel.get_Range("J" + posicionTotalAP);
                        rango.Formula = "=SUMA(J4:J" + (posicionTotalAP - 1);

                        rango = hojaExcel.get_Range("L" + posicionTotalAP);
                        rango.Formula = "=SUMA(L4:L" + (posicionTotalAP - 1);
                        rango.HorizontalAlignment = Excel.Constants.xlRight;

                        rango = hojaExcel.get_Range("M" + posicionTotalAP);
                        rango.Formula = "=SUMA(M4:M" + (posicionTotalAP - 1);
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        
                        rango = hojaExcel.get_Range("N" + posicionTotalAP);
                        rango.Formula = "=(M" + posicionTotalAP + "/H" + posicionTotalAP + ")";
                        rango.NumberFormat = "0.0%";
                    }

                    posicionRango = posicionTotalAP + 2;
                    posicionTotalAD = posicionTotalAP;

                    OnCambioProgreso(70);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //ARRENDAMIENTO EN DOLARES
                    if (subRentadosD.Count > 0)
                    {
                        posicionTotalAD = posicionRango + subRentadosD.Count;
                        
                        //RENTADOSDLS
                        rango = hojaExcel.get_Range("A" + (posicionTotalAP + 1) + ":O" + (posicionTotalAP + 1));
                        rango.Merge();
                        rango.Value2 = "ARRENDAMIENTO EN DOLARES";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        
                        //ColumnaA
                        foreach (string rent in subRentadosD)
                        {
                            if (!string.IsNullOrEmpty(rent))
                            {
                                rango = hojaExcel.get_Range("A" + posicionRango);
                                rango.Value2 = rent;
                                rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        posicionRango = posicionTotalAP + 2;
                        //COLUMNAB
                        if (municipioD.Count > 0)
                        {
                            foreach (string muniD in municipioD)
                            {
                                if (!string.IsNullOrEmpty(muniD))
                                {
                                    rango = hojaExcel.get_Range("B" + posicionRango);
                                    rango.Value2 = muniD;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        posicionRango = posicionTotalAP + 2;
                        //COLUMNAC
                        if (estadoD.Count > 0)
                        {
                            foreach (string estad in estadoD)
                            {
                                if (!string.IsNullOrEmpty(estad))
                                {
                                    rango = hojaExcel.get_Range("C" + posicionRango);
                                    rango.Value2 = estad;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        posicionRango = posicionTotalAP + 2;
                        //COLUMNAD
                        if (direccionD.Count > 0)
                        {
                            foreach (string dir in direccionD)
                            {
                                if (!string.IsNullOrEmpty(dir))
                                {
                                    rango = hojaExcel.get_Range("D" + posicionRango);
                                    rango.Value2 = dir;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        posicionRango = posicionTotalAP + 2;
                        //COLUMNAE
                        if (identifD.Count > 0)
                        {
                            foreach (string ids in identifD)
                            {
                                if (!string.IsNullOrEmpty(ids))
                                {
                                    rango = hojaExcel.get_Range("E" + posicionRango);
                                    rango.Value2 = ids;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        posicionRango = posicionTotalAP + 2;
                        //COLUMNAF
                        if (expCatastD.Count > 0)
                        {
                            foreach (string exp in expCatastD)
                            {
                                if (!string.IsNullOrEmpty(exp))
                                {
                                    rango = hojaExcel.get_Range("F" + posicionRango);
                                    rango.Value2 = exp;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        posicionRango = posicionTotalAP + 2;
                        //COLUMNAG
                        if (conceptoD.Count > 0)
                        {
                            foreach (string conc in conceptoD)
                            {
                                if (!string.IsNullOrEmpty(conc))
                                {
                                    rango = hojaExcel.get_Range("G" + posicionRango);
                                    rango.Value2 = conc;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        posicionRango = posicionTotalAP + 2;
                        //COLUMNAH
                        if (valCatastD.Count > 0)
                        {
                            foreach (decimal valCat in valCatastD)
                            {
                                rango = hojaExcel.get_Range("H" + posicionRango);
                                rango.Value2 = valCat;
                                rango.NumberFormat = "$###,###,###,###,##0.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                               
                            }
                        }
                        
                        posicionRango = posicionTotalAP + 2;
                        //COLUMNAI
                        if (terrenosD.Count > 0)
                        {
                            foreach (decimal terr in terrenosD)
                            {
                                rango = hojaExcel.get_Range("I" + posicionRango);
                                rango.Value2 = terr;
                                rango.NumberFormat = "###,###,###,###,##0.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;

                            }
                        }
                        
                        posicionRango = posicionTotalAP + 2;
                        //COLUMNAJ
                        if (constD.Count > 0)
                        {
                            foreach (decimal constru in constD)
                            {
                                rango = hojaExcel.get_Range("J" + posicionRango);
                                rango.Value2 = constru;
                                rango.NumberFormat = "###,###,###,###,##0.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        posicionRango = posicionTotalAP + 2;
                        //COLUMNAK
                        if (arrendatariosD.Count > 0)
                        {
                            foreach (string arre in arrendatariosD)
                            {
                                if (!string.IsNullOrEmpty(arre))
                                {
                                    rango = hojaExcel.get_Range("K" + posicionRango);
                                    rango.Value2 = arre;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        posicionRango = posicionTotalAP + 2;
                        //COLUMNAL
                        if (rentaMensualD.Count > 0)
                        {
                            foreach (decimal rm in rentaMensualD)
                            {
                                rango = hojaExcel.get_Range("L" + posicionRango);
                                rango.Value2 = rm;
                                rango.NumberFormat = "$###,###,###,###,##0.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        posicionRango = posicionTotalAP + 2;
                        //COLUMNAM
                        if (rentaAnualD.Count > 0)
                        {
                            foreach (decimal ra in rentaAnualD)
                            {
                                rango = hojaExcel.get_Range("M" + posicionRango);
                                rango.Value2 = ra;
                                rango.NumberFormat = "$###,###,###,###,##0.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("N" + posicionRango);
                                rango.Formula = "=(M" + posicionRango + "/H" + posicionRango + ")";
                                rango.NumberFormat = "0.0%";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("O" + posicionRango);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                posicionRango++;
                            }
                        }
                        posicionRango = posicionTotalAP + 2;
                        //TOTALDolares
                        rango = hojaExcel.get_Range("A" + posicionTotalAD + ":O" + posicionTotalAD);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("A" + posicionTotalAD);
                        rango.Value2 = "TOTAL";

                        rango = hojaExcel.get_Range("H" + posicionTotalAD);
                        rango.Formula = "=SUMA(H" + posicionRango + ":H" + (posicionTotalAD -1);
                        rango.HorizontalAlignment = Excel.Constants.xlRight;

                        rango = hojaExcel.get_Range("I" + posicionTotalAD);
                        rango.Formula = "=SUMA(I" + posicionRango + ":I" + (posicionTotalAD - 1);
                        
                        rango = hojaExcel.get_Range("J" + posicionTotalAD);
                        rango.Formula = "=SUMA(J" + posicionRango + ":J" + (posicionTotalAD - 1);
                        
                        rango = hojaExcel.get_Range("L" + posicionTotalAD);
                        rango.Formula = "=SUMA(L" + posicionRango + ":L" + (posicionTotalAD - 1);
                        rango.HorizontalAlignment = Excel.Constants.xlRight;

                        rango = hojaExcel.get_Range("M" + posicionTotalAD);
                        rango.Formula = "=SUMA(M" + posicionRango + ":M" + (posicionTotalAD - 1);
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        
                        rango = hojaExcel.get_Range("N" + posicionTotalAD);
                        rango.Formula = "=(M" + posicionTotalAD + "/H" + posicionTotalAD + ")";
                        rango.NumberFormat = "0.0%";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    }

                    OnCambioProgreso(75);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                     posicionRango = posicionTotalAD + 2;
                     posicionTotalNR = posicionRango;
                    //NORENTADOS
                    if (subNoRentados.Count > 0)
                    {
                        posicionTotalNR = posicionRango + subNoRentados.Count;
                        
                        //NORENTADOS
                        rango = hojaExcel.get_Range("A" + (posicionTotalAD + 1) + ":O" + (posicionTotalAD + 1));
                        rango.Merge();
                        rango.Value2 = "NO RENTADOS";
                        rango.HorizontalAlignment = Excel.Constants.xlLeft;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        
                        //ColumnaA
                        foreach (string rent in subNoRentados)
                        {
                            if (!string.IsNullOrEmpty(rent))
                            {
                                rango = hojaExcel.get_Range("A" + posicionRango);
                                rango.Value2 = rent;
                                rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        posicionRango = posicionTotalAD + 2;
                        //COLUMNAB
                        if (municipioNR.Count > 0)
                        {
                            foreach (string muniNR in municipioNR)
                            {
                                if (!string.IsNullOrEmpty(muniNR))
                                {
                                    rango = hojaExcel.get_Range("B" + posicionRango);
                                    rango.Value2 = muniNR;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        posicionRango = posicionTotalAD + 2;
                        //COLUMNAC
                        if (estadoNR.Count > 0)
                        {
                            foreach (string estad in estadoNR)
                            {
                                if (!string.IsNullOrEmpty(estad))
                                {
                                    rango = hojaExcel.get_Range("C" + posicionRango);
                                    rango.Value2 = estad;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        posicionRango = posicionTotalAD + 2;
                        //COLUMNAD
                        if (direccionNR.Count > 0)
                        {
                            foreach (string dir in direccionNR)
                            {
                                if (!string.IsNullOrEmpty(dir))
                                {
                                    rango = hojaExcel.get_Range("D" + posicionRango);
                                    rango.Value2 = dir;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        posicionRango = posicionTotalAD + 2;
                        //COLUMNAE
                        if (identifNR.Count > 0)
                        {
                            foreach (string ids in identifNR)
                            {
                                if (!string.IsNullOrEmpty(ids))
                                {
                                    rango = hojaExcel.get_Range("E" + posicionRango);
                                    rango.Value2 = ids;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        posicionRango = posicionTotalAD + 2;
                        //COLUMNAF
                        if (expCatastNR.Count > 0)
                        {
                            foreach (string exp in expCatastNR)
                            {
                                if (!string.IsNullOrEmpty(exp))
                                {
                                    rango = hojaExcel.get_Range("F" + posicionRango);
                                    rango.Value2 = exp;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        posicionRango = posicionTotalAD + 2;
                        //COLUMNAG
                        if (conceptoNR.Count > 0)
                        {
                            foreach (string conc in conceptoNR)
                            {
                                if (!string.IsNullOrEmpty(conc))
                                {
                                    rango = hojaExcel.get_Range("G" + posicionRango);
                                    rango.Value2 = conc;
                                    rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        posicionRango = posicionTotalAD + 2;
                        //COLUMNAH
                        if (valCatastNR.Count > 0)
                        {
                            foreach (decimal valCat in valCatastNR)
                            {
                                rango = hojaExcel.get_Range("H" + posicionRango);
                                rango.Value2 = valCat;
                                rango.NumberFormat = "$###,###,###,###,##0.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                               
                            }
                        }
                        
                        posicionRango = posicionTotalAD + 2;
                        //COLUMNAI
                        if (terrenosNR.Count > 0)
                        {
                            foreach (decimal terr in terrenosNR)
                            {
                                rango = hojaExcel.get_Range("I" + posicionRango);
                                rango.Value2 = terr;
                                rango.NumberFormat = "###,###,###,###,##0.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;

                            }
                        }
                        
                        posicionRango = posicionTotalAD + 2;
                        //COLUMNAJ
                        if (constNR.Count > 0)
                        {
                            foreach (decimal constru in constNR)
                            {
                                rango = hojaExcel.get_Range("J" + posicionRango);
                                rango.Value2 = constru;
                                rango.NumberFormat = "###,###,###,###,##0.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                
                                rango = hojaExcel.get_Range("K" + posicionRango + ":O" + posicionRango);
                                rango.HorizontalAlignment = Excel.Constants.xlRight;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                posicionRango++;
                            }
                        }
                        
                        posicionRango = posicionTotalAD + 2;
                       
                        //TOTALNORENTADOS
                        rango = hojaExcel.get_Range("A" + posicionTotalNR + ":O" + posicionTotalNR);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("A" + posicionTotalNR);
                        rango.Value2 = "TOTAL";

                        rango = hojaExcel.get_Range("H" + posicionTotalNR);
                        rango.Formula = "=SUMA(H" + posicionRango + ":H" + (posicionTotalNR - 1);
                        rango.HorizontalAlignment = Excel.Constants.xlRight;

                        rango = hojaExcel.get_Range("I" + posicionTotalNR);
                        rango.Formula = "=SUMA(I" + posicionRango + ":I" + (posicionTotalNR - 1);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;

                        rango = hojaExcel.get_Range("J" + posicionTotalNR);
                        rango.Formula = "=SUMA(J" + posicionRango + ":J" + (posicionTotalNR - 1);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    }

                    OnCambioProgreso(80);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //GASTOSENCAB
                    posicionRango = posicionTotalNR + 3;

                    string totalColumn = "L";
                    string ultimaColumnaConValor = "K";
                    hojaExcel.get_Range("A2:J2").Copy();
                    hojaExcel.get_Range("A" + posicionRango + ":J" + posicionRango).PasteSpecial();
                    
                    rango = hojaExcel.get_Range("K" + posicionRango);
                    rango.Value2 = "Predial";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        
                    if (gastosNombres.Count > 0)
                    {
                        for (int i = 0; i < gastosNombres.Count; i++)
                        {
                            int gtosDlsIni = posicionRango;
                            int gtosNrP = posicionRango;
                            int gtosNRD = posicionRango;
                            posicionRango = posicionTotalNR + 3;
                            if (i == 0)
                            {
                                ultimaColumnaConValor = "L";
                                rango = hojaExcel.get_Range("L" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("L" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("L" + posicionRango);
                                    rango.Formula = "=SUMA(L" + (posicionTotalNR + 5) + ":L" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango +=2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("L" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("L" + posicionRango);
                                    rango.Formula = "=SUMA(L" + gtosDlsIni + ":L" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("L" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("L" + posicionRango);
                                    rango.Formula = "=SUMA(L" + gtosNrP + ":L" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("L" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("L" + posicionRango);
                                    rango.Formula = "=SUMA(L" + gtosNRD + ":L" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "M";
                            }
                            else if (i == 1)
                            {
                                ultimaColumnaConValor = "M";
                                rango = hojaExcel.get_Range("M" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("M" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("M" + posicionRango);
                                    rango.Formula = "=SUMA(M" + (posicionTotalNR + 5) + ":M" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("M" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("M" + posicionRango);
                                    rango.Formula = "=SUMA(M" + gtosDlsIni + ":M" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("M" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("M" + posicionRango);
                                    rango.Formula = "=SUMA(M" + gtosNrP + ":M" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("M" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("M" + posicionRango);
                                    rango.Formula = "=SUMA(M" + gtosNRD + ":M" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "N";
                            }
                            else if (i == 2)
                            {
                                ultimaColumnaConValor = "N";
                                rango = hojaExcel.get_Range("N" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("N" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("N" + posicionRango);
                                    rango.Formula = "=SUMA(N" + (posicionTotalNR + 5) + ":N" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("N" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("N" + posicionRango);
                                    rango.Formula = "=SUMA(N" + gtosDlsIni + ":N" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("N" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("N" + posicionRango);
                                    rango.Formula = "=SUMA(N" + gtosNrP + ":N" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("N" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("N" + posicionRango);
                                    rango.Formula = "=SUMA(N" + gtosNRD + ":N" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "O";
                            }
                            else if (i == 3)
                            {
                                ultimaColumnaConValor = "O";
                                rango = hojaExcel.get_Range("O" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("O" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("O" + posicionRango);
                                    rango.Formula = "=SUMA(O" + (posicionTotalNR + 5) + ":O" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("O" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("O" + posicionRango);
                                    rango.Formula = "=SUMA(O" + gtosDlsIni + ":O" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("O" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("O" + posicionRango);
                                    rango.Formula = "=SUMA(O" + gtosNrP + ":O" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("O" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("O" + posicionRango);
                                    rango.Formula = "=SUMA(O" + gtosNRD + ":O" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "P";
                            }
                            else if (i == 4)
                            {
                                ultimaColumnaConValor = "P";
                                rango = hojaExcel.get_Range("P" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("P" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("P" + posicionRango);
                                    rango.Formula = "=SUMA(P" + (posicionTotalNR + 5) + ":P" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("P" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("P" + posicionRango);
                                    rango.Formula = "=SUMA(P" + gtosDlsIni + ":P" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("P" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("P" + posicionRango);
                                    rango.Formula = "=SUMA(P" + gtosNrP + ":P" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("P" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("P" + posicionRango);
                                    rango.Formula = "=SUMA(P" + gtosNRD + ":P" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "Q";
                            }
                            else if (i == 5)
                            {
                                ultimaColumnaConValor = "Q";
                                rango = hojaExcel.get_Range("Q" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("Q" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("Q" + posicionRango);
                                    rango.Formula = "=SUMA(Q" + (posicionTotalNR + 5) + ":Q" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("Q" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("Q" + posicionRango);
                                    rango.Formula = "=SUMA(Q" + gtosDlsIni + ":Q" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("Q" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("Q" + posicionRango);
                                    rango.Formula = "=SUMA(Q" + gtosNrP + ":Q" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("Q" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("Q" + posicionRango);
                                    rango.Formula = "=SUMA(Q" + gtosNRD + ":Q" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "R";
                            }
                            else if (i == 6)
                            {
                                ultimaColumnaConValor = "R";
                                rango = hojaExcel.get_Range("R" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("R" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("R" + posicionRango);
                                    rango.Formula = "=SUMA(R" + (posicionTotalNR + 5) + ":R" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("R" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("R" + posicionRango);
                                    rango.Formula = "=SUMA(R" + gtosDlsIni + ":R" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("R" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("R" + posicionRango);
                                    rango.Formula = "=SUMA(R" + gtosNrP + ":R" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("R" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("R" + posicionRango);
                                    rango.Formula = "=SUMA(R" + gtosNRD + ":R" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "S";
                            }
                            else if (i == 7)
                            {
                                ultimaColumnaConValor = "S";
                                rango = hojaExcel.get_Range("S" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("S" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("S" + posicionRango);
                                    rango.Formula = "=SUMA(S" + (posicionTotalNR + 5) + ":S" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("S" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("S" + posicionRango);
                                    rango.Formula = "=SUMA(S" + gtosDlsIni + ":S" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("S" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("S" + posicionRango);
                                    rango.Formula = "=SUMA(S" + gtosNrP + ":S" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("S" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("S" + posicionRango);
                                    rango.Formula = "=SUMA(S" + gtosNRD + ":S" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "T";
                            }
                            else if (i == 8)
                            {
                                ultimaColumnaConValor = "T";
                                rango = hojaExcel.get_Range("T" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("T" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("T" + posicionRango);
                                    rango.Formula = "=SUMA(T" + (posicionTotalNR + 5) + ":T" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("T" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("T" + posicionRango);
                                    rango.Formula = "=SUMA(T" + gtosDlsIni + ":T" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("T" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("T" + posicionRango);
                                    rango.Formula = "=SUMA(T" + gtosNrP + ":T" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("T" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("T" + posicionRango);
                                    rango.Formula = "=SUMA(T" + gtosNRD + ":T" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "U";
                            }
                            else if (i == 9)
                            {
                                ultimaColumnaConValor = "U";
                                rango = hojaExcel.get_Range("U" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("U" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("U" + posicionRango);
                                    rango.Formula = "=SUMA(U" + (posicionTotalNR + 5) + ":U" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("U" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("U" + posicionRango);
                                    rango.Formula = "=SUMA(U" + gtosDlsIni + ":U" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("U" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("U" + posicionRango);
                                    rango.Formula = "=SUMA(U" + gtosNrP + ":U" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("U" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("U" + posicionRango);
                                    rango.Formula = "=SUMA(U" + gtosNRD + ":U" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "V";
                            }
                            else if (i == 10)
                            {
                                ultimaColumnaConValor = "V";
                                rango = hojaExcel.get_Range("V" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("V" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("V" + posicionRango);
                                    rango.Formula = "=SUMA(V" + (posicionTotalNR + 5) + ":V" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("V" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("V" + posicionRango);
                                    rango.Formula = "=SUMA(V" + gtosDlsIni + ":V" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("V" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("V" + posicionRango);
                                    rango.Formula = "=SUMA(V" + gtosNrP + ":V" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("V" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("V" + posicionRango);
                                    rango.Formula = "=SUMA(V" + gtosNRD + ":V" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "W";
                            }
                            else if (i == 11)
                            {
                                ultimaColumnaConValor = "W";
                                rango = hojaExcel.get_Range("W" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("W" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("W" + posicionRango);
                                    rango.Formula = "=SUMA(W" + (posicionTotalNR + 5) + ":W" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("W" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("W" + posicionRango);
                                    rango.Formula = "=SUMA(W" + gtosDlsIni + ":W" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("W" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("W" + posicionRango);
                                    rango.Formula = "=SUMA(W" + gtosNrP + ":W" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("W" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("W" + posicionRango);
                                    rango.Formula = "=SUMA(W" + gtosNRD + ":W" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "X";
                            }
                            else if (i == 12)
                            {
                                ultimaColumnaConValor = "X";
                                rango = hojaExcel.get_Range("X" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("X" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("X" + posicionRango);
                                    rango.Formula = "=SUMA(X" + (posicionTotalNR + 5) + ":X" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("X" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("X" + posicionRango);
                                    rango.Formula = "=SUMA(X" + gtosDlsIni + ":X" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("X" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("X" + posicionRango);
                                    rango.Formula = "=SUMA(X" + gtosNrP + ":X" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("X" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("X" + posicionRango);
                                    rango.Formula = "=SUMA(X" + gtosNRD + ":X" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "Y";
                            }
                            else if (i == 13)
                            {
                                ultimaColumnaConValor = "Y";
                                rango = hojaExcel.get_Range("Y" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("Y" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("Y" + posicionRango);
                                    rango.Formula = "=SUMA(Y" + (posicionTotalNR + 5) + ":Y" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("Y" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("Y" + posicionRango);
                                    rango.Formula = "=SUMA(Y" + gtosDlsIni + ":Y" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("Y" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("Y" + posicionRango);
                                    rango.Formula = "=SUMA(Y" + gtosNrP + ":Y" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("Y" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("Y" + posicionRango);
                                    rango.Formula = "=SUMA(Y" + gtosNRD + ":Y" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "Z";
                            }
                            else if (i == 14)
                            {
                                ultimaColumnaConValor = "Z";
                                rango = hojaExcel.get_Range("Z" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("Z" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("Z" + posicionRango);
                                    rango.Formula = "=SUMA(Z" + (posicionTotalNR + 5) + ":Z" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("Z" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("Z" + posicionRango);
                                    rango.Formula = "=SUMA(Z" + gtosDlsIni + ":Z" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("Z" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("Z" + posicionRango);
                                    rango.Formula = "=SUMA(Z" + gtosNrP + ":Z" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("Z" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("Z" + posicionRango);
                                    rango.Formula = "=SUMA(Z" + gtosNRD + ":Z" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "AA";
                            }
                            else if (i == 15)
                            {
                                ultimaColumnaConValor = "AA";
                                rango = hojaExcel.get_Range("AA" + posicionRango);
                                rango.Value2 = gastosNombres[i];
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Font.Bold = true;
                                rango.Font.Size = 11;
                                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango += 2;
                                if (subRentadosP.Count > 0)
                                {
                                    for (int j = 0; j < subRentadosP.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("AA" + posicionRango);
                                        rango.Value2 = matrizGastosSub[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("AA" + posicionRango);
                                    rango.Formula = "=SUMA(AA" + (posicionTotalNR + 5) + ":AA" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosDlsIni = posicionRango += 2;
                                if (subRentadosD.Count > 0)
                                {
                                    posicionRango = gtosDlsIni;
                                    for (int j = 0; j < subRentadosD.Count; j++)
                                    {
                                        rango = hojaExcel.get_Range("AA" + posicionRango);
                                        rango.Value2 = matrizGastosSubD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("AA" + posicionRango);
                                    rango.Formula = "=SUMA(AA" + gtosDlsIni + ":AA" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNrP = posicionRango += 2;
                                if (inmobiliariasGP != null )
                                {
                                    posicionRango = gtosNrP;
                                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;
                                    for (int j = 0; j < inmobiliariasGP.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("AA" + posicionRango);
                                        rango.Value2 = matrizGP[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("AA" + posicionRango);
                                    rango.Formula = "=SUMA(AA" + gtosNrP + ":AA" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                gtosNRD = posicionRango += 2;
                                if (inmobiliariasGD != null)
                                {
                                    posicionRango = gtosNRD;
                                    /*if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                                        posicionRango = posicionRango - 4;*/
                                    for (int j = 0; j < inmobiliariasGD.Length; j++)
                                    {
                                        rango = hojaExcel.get_Range("AA" + posicionRango);
                                        rango.Value2 = matrizGD[j, i];
                                        rango.NumberFormat = "$###,###,###,###,##0.00";
                                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                        posicionRango++;
                                    }
                                    rango = hojaExcel.get_Range("AA" + posicionRango);
                                    rango.Formula = "=SUMA(AA" + gtosNRD + ":AA" + (posicionRango - 1) + ")";
                                    rango.Font.Bold = true;
                                    rango.Font.Size = 11;
                                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                }
                                totalColumn = "AB";
                            }
                        }
                    }

                    OnCambioProgreso(85);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    posicionRango = posicionTotalNR + 3;
                    rango = hojaExcel.get_Range(totalColumn + posicionRango);
                    rango.Value2 = "Total";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    int posicionGastosTotalP = 3;
                    posicionRango++;
                    if (subRentadosP.Count > 0)
                    {
                        //GASTOS ANUALES DE INMUEBLES RENTADOS EN PESOS
                        rango = hojaExcel.get_Range("A" + posicionRango + ":" + totalColumn + posicionRango);
                        rango.Merge();
                        rango.Value2 = "GASTOS ANUALES DE INMUEBLES RENTADOS EN PESOS";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        posicionRango++;
                        
                        hojaExcel.get_Range("A4:J" + posicionTotalAP).Copy();
                        hojaExcel.get_Range("A" + posicionRango).PasteSpecial();
                        foreach (decimal predialP in predialesRentadosP)
                        {
                            rango = hojaExcel.get_Range("K" + posicionRango);
                            rango.Value2 = predialP;
                            rango.NumberFormat = "$###,###,###,###,##0.00";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                            rango = hojaExcel.get_Range(totalColumn + posicionRango);
                            rango.Formula = "=SUMA(K" + posicionRango + ":" + ultimaColumnaConValor + posicionRango + ")";
                            rango.NumberFormat = "$###,###,###,###,##0.00";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                            posicionRango++;
                        }
                        //TotalPRedialesPesos
                        posicionGastosTotalP = posicionRango;
                        rango = hojaExcel.get_Range("K" + posicionRango);
                        rango.Formula = "=SUMA(K" + (posicionTotalNR + 5) + ":K" + (posicionGastosTotalP -1);
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        posicionGastosTotalP = posicionRango;
                        rango = hojaExcel.get_Range(totalColumn + posicionRango);
                        rango.Formula = "=SUMA("+ totalColumn + (posicionTotalNR + 5) + ":" + totalColumn + (posicionGastosTotalP - 1);
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        
                    }

                    int posicionGastosTotalD = posicionGastosTotalP;
                    posicionRango = posicionGastosTotalP + 1;
                    if (subRentadosD.Count <= 0)
                        posicionRango++;
                    if (subRentadosD.Count > 0)
                    {
                        //GASTOS ANUALES DE INMUEBLES RENTADOS EN DOLARES CONVERTIDOS A PESOS
                        rango = hojaExcel.get_Range("A" + posicionRango + ":" + totalColumn + posicionRango);
                        rango.Merge();
                        rango.Value2 = "GASTOS ANUALES DE INMUEBLES RENTADOS EN DOLARES CONVERTIDOS A PESOS";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        posicionRango++;
                        
                        hojaExcel.get_Range("A" + (posicionTotalAP + 2)+":J" + posicionTotalAD).Copy();
                        hojaExcel.get_Range("A" + posicionRango).PasteSpecial();
                        foreach (decimal predialD in predialesRentadosD)
                        {
                            rango = hojaExcel.get_Range("K" + posicionRango);
                            rango.Value2 = predialD;
                            rango.NumberFormat = "$###,###,###,###,##0.00";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                            rango = hojaExcel.get_Range(totalColumn + posicionRango);
                            rango.Formula = "=SUMA(K" + posicionRango + ":" + ultimaColumnaConValor + posicionRango + ")";
                            rango.NumberFormat = "$###,###,###,###,##0.00";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                            posicionRango++;
                        }
                        //TotalPredialesDls
                        posicionGastosTotalD = posicionRango;
                        rango = hojaExcel.get_Range("K" + posicionRango);
                        rango.Formula = "=SUMA(K" + (posicionGastosTotalP + 2) + ":K" + (posicionGastosTotalD -1);
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        
                        posicionGastosTotalD = posicionRango;
                        rango = hojaExcel.get_Range(totalColumn + posicionRango);
                        rango.Formula = "=SUMA(" + totalColumn + (posicionGastosTotalP + 2) + ":" + totalColumn + (posicionGastosTotalD - 1);
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        
                    }
                    if (subRentadosP.Count <= 0 && subRentadosD.Count <= 0)
                        posicionRango = subNoRentados.Count + 7;
                    if (inmobiliariasGP != null )
                    {
                        posicionRango++;
                        //GASTOS ANUALES DE INMUEBLES NO RENTADOS EN PESOS
                        rango = hojaExcel.get_Range("A" + posicionRango + ":" + totalColumn + posicionRango);
                        rango.Merge();
                        rango.Value2 = "GASTOS ANUALES DE INMUEBLES NO RENTADOS EN PESOS";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        posicionRango++;
                        int primerFilaNRGP = posicionRango;
                        
                        //ColumnaA
                        if (inmobiliariasGP != null)
                        {
                            foreach (string inm in inmobiliariasGP)
                            {
                                rango = hojaExcel.get_Range("A" + posicionRango);
                                rango.Value2 = inm;
                                rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("A" + posicionRango);
                            rango.Value2 = "Total";
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGP;
                        //ColumnaB
                        if (municipioGP != null)
                        {
                            foreach (string mun in municipioGP)
                            {
                                rango = hojaExcel.get_Range("B" + posicionRango);
                                rango.Value2 = mun;
                                rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("B" + posicionRango);
                            rango.HorizontalAlignment = Excel.Constants.xlLeft;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGP;
                        //ColumnaC
                        if (estadoGP != null)
                        {
                            foreach (string est in estadoGP)
                            {
                                rango = hojaExcel.get_Range("C" + posicionRango);
                                rango.Value2 = est;
                                rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("C" + posicionRango);
                            rango.HorizontalAlignment = Excel.Constants.xlLeft;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGP;
                        //ColumnaD
                        if (direccionGP != null)
                        {
                            foreach (string dir in direccionGP)
                            {
                                rango = hojaExcel.get_Range("D" + posicionRango);
                                rango.Value2 = dir;
                                rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("D" + posicionRango);
                            rango.HorizontalAlignment = Excel.Constants.xlLeft;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGP;
                        //ColumnaE
                        if (identifGP != null)
                        {
                            foreach (string ident in identifGP)
                            {
                                rango = hojaExcel.get_Range("E" + posicionRango);
                                rango.Value2 = ident;
                                rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("E" + posicionRango);
                            rango.HorizontalAlignment = Excel.Constants.xlLeft;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGP;
                        //ColumnaF
                        if (expCatastGP != null)
                        {
                            foreach (string exp in expCatastGP)
                            {
                                rango = hojaExcel.get_Range("F" + posicionRango);
                                rango.Value2 = exp;
                                rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("F" + posicionRango);
                            rango.HorizontalAlignment = Excel.Constants.xlLeft;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGP;
                        //ColumnaG
                        if (conceptosGP != null)
                        {
                            foreach (string cnp in conceptosGP)
                            {
                                rango = hojaExcel.get_Range("G" + posicionRango);
                                rango.Value2 = cnp;
                                rango.HorizontalAlignment = Excel.Constants.xlLeft;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("G" + posicionRango);
                            rango.HorizontalAlignment = Excel.Constants.xlLeft;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGP;
                        //ColumnaH
                        if (valCatastGP != null)
                        {
                            foreach (decimal val in valCatastGP)
                            {
                                rango = hojaExcel.get_Range("H" + posicionRango);
                                rango.Value2 = val;
                                rango.NumberFormat = "$###,###,###,##0.00";
                                //rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("H" + posicionRango);
                            rango.Formula = "=SUMA(H" + primerFilaNRGP + ":H" + (posicionRango - 1) + ")";
                            rango.NumberFormat = "$###,###,###,##0.00";
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGP;
                        //ColumnaI
                        if (terrenosGP != null)
                        {
                            foreach (decimal terr in terrenosGP)
                            {
                                rango = hojaExcel.get_Range("I" + posicionRango);
                                rango.Value2 = terr;
                                rango.NumberFormat = "###,###,###,##0.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("I" + posicionRango);
                            rango.Formula = "=SUMA(I" + primerFilaNRGP + ":I" + (posicionRango - 1) + ")";
                            rango.NumberFormat = "###,###,###,##0.00";
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGP;
                        //ColumnaJ
                        if (constGP != null)
                        {
                            foreach (decimal cnt in constGP)
                            {
                                rango = hojaExcel.get_Range("J" + posicionRango);
                                rango.Value2 = cnt;
                                rango.NumberFormat = "###,###,###,##0.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("J" + posicionRango);
                            rango.Formula = "=SUMA(J" + primerFilaNRGP + ":J" + (posicionRango - 1) + ")";
                            rango.NumberFormat = "###,###,###,##0.00";
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGP;
                        //ColumnaK
                        if (predialGP != null)
                        {
                            foreach (decimal prd in predialGP)
                            {
                                rango = hojaExcel.get_Range("K" + posicionRango);
                                rango.Value2 = prd;
                                rango.NumberFormat = "$###,###,###,##0.00";
                                //rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range(totalColumn + posicionRango);
                                rango.Formula = "=SUMA(K" + posicionRango + ":" + ultimaColumnaConValor + posicionRango + ")";
                                rango.NumberFormat = "$###,###,###,###,##0.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("K" + posicionRango);
                            rango.Formula = "=SUMA(K" + primerFilaNRGP + ":K" + (posicionRango - 1) + ")";
                            rango.NumberFormat = "$###,###,###,##0.00";
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }
                        rango = hojaExcel.get_Range(totalColumn + posicionRango);
                        rango.Formula = "=SUMA(" + totalColumn + primerFilaNRGP + ":" + totalColumn + (posicionRango - 1) + ")";
                        rango.NumberFormat = "$###,###,###,##0.00";
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    }

                    if (inmobiliariasGD != null )
                    {
                        posicionRango++;
                        //GASTOS ANUALES DE INMUEBLES NO RENTADOS EN DOLARES
                        rango = hojaExcel.get_Range("A" + posicionRango + ":" + totalColumn + posicionRango);
                        rango.Merge();
                        rango.Value2 = "GASTOS ANUALES DE INMUEBLES NO RENTADOS EN DOLARES CONVERTIDOS A PESOS";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        posicionRango++;
                        int primerFilaNRGD = posicionRango;
                        
                        //ColumnaA
                        if (inmobiliariasGD != null)
                        {
                            foreach (string inm in inmobiliariasGD)
                            {
                                rango = hojaExcel.get_Range("A" + posicionRango);
                                rango.Value2 = inm;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("A" + posicionRango);
                            rango.Value2 = "Total";
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }
                        
                        posicionRango = primerFilaNRGD;
                        //ColumnaB
                        if (municipioGD != null)
                        {
                            foreach (string mun in municipioGD)
                            {
                                rango = hojaExcel.get_Range("B" + posicionRango);
                                rango.Value2 = mun;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("B" + posicionRango);
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }
                        
                        posicionRango = primerFilaNRGD;
                        //ColumnaC
                        if (estadoGD != null)
                        {
                            foreach (string est in estadoGD)
                            {
                                rango = hojaExcel.get_Range("C" + posicionRango);
                                rango.Value2 = est;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("C" + posicionRango);
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }
                        
                        posicionRango = primerFilaNRGD;
                        //ColumnaD
                        if (direccionGD != null)
                        {
                            foreach (string dir in direccionGD)
                            {
                                rango = hojaExcel.get_Range("D" + posicionRango);
                                rango.Value2 = dir;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("D" + posicionRango);
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }
                        
                        posicionRango = primerFilaNRGD;
                        //ColumnaE
                        if (identifGD != null)
                        {
                            foreach (string ident in identifGD)
                            {
                                rango = hojaExcel.get_Range("E" + posicionRango);
                                rango.Value2 = ident;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("E" + posicionRango);
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGD;
                        //ColumnaF
                        if (expCatastGD != null)
                        {
                            foreach (string exp in expCatastGD)
                            {
                                rango = hojaExcel.get_Range("F" + posicionRango);
                                rango.Value2 = exp;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("F" + posicionRango);
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGD;
                        //ColumnaG
                        if (conceptosGD != null)
                        {
                            foreach (string cnp in conceptosGD)
                            {
                                rango = hojaExcel.get_Range("G" + posicionRango);
                                rango.Value2 = cnp;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("G" + posicionRango);
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGD;
                        //ColumnaH
                        if (valCatastGD != null)
                        {
                            foreach (decimal val in valCatastGD)
                            {
                                rango = hojaExcel.get_Range("H" + posicionRango);
                                rango.Value2 = val;
                                rango.NumberFormat = "$###,###,###,##0.00";
                                //rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("H" + posicionRango);
                            rango.Formula = "=SUMA(H" + primerFilaNRGD + ":H" + (posicionRango -1) + ")";
                            rango.NumberFormat = "$###,###,###,##0.00";
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGD;
                        //ColumnaI
                        if (terrenosGD != null)
                        {
                            foreach (decimal terr in terrenosGD)
                            {
                                rango = hojaExcel.get_Range("I" + posicionRango);
                                rango.Value2 = terr;
                                rango.NumberFormat = "###,###,###,##0.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("I" + posicionRango);
                            rango.Formula = "=SUMA(I" + primerFilaNRGD + ":I" + (posicionRango - 1) + ")";
                            rango.NumberFormat = "###,###,###,##0.00";
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGD;
                        //ColumnaJ
                        if (constGD != null)
                        {
                            foreach (decimal cnt in constGD)
                            {
                                rango = hojaExcel.get_Range("J" + posicionRango);
                                rango.Value2 = cnt;
                                rango.NumberFormat = "###,###,###,##0.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("J" + posicionRango);
                            rango.Formula = "=SUMA(J" + primerFilaNRGD + ":J" + (posicionRango - 1) + ")";
                            rango.NumberFormat = "###,###,###,##0.00";
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }

                        posicionRango = primerFilaNRGD;
                        //ColumnaK
                        if (predialGD != null)
                        {
                            foreach (decimal prd in predialGD)
                            {
                                rango = hojaExcel.get_Range("K" + posicionRango);
                                rango.Value2 = prd;
                                rango.NumberFormat = "$###,###,###,##0.00";
                                //rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range(totalColumn + posicionRango);
                                rango.Formula = "=SUMA(K" + posicionRango + ":" + ultimaColumnaConValor + posicionRango + ")";
                                rango.NumberFormat = "$###,###,###,###,##0.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                posicionRango++;
                            }
                            rango = hojaExcel.get_Range("K" + posicionRango);
                            rango.Formula = "=SUMA(K" + primerFilaNRGD + ":K" + (posicionRango - 1) + ")";
                            rango.NumberFormat = "$###,###,###,##0.00";
                            rango.Font.Bold = true;
                            rango.Font.Size = 11;
                            rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        }
                        rango = hojaExcel.get_Range(totalColumn + posicionRango);
                        rango.Formula = "=SUMA(" + totalColumn + primerFilaNRGD + ":" + totalColumn + (posicionRango - 1) + ")";
                        rango.NumberFormat = "$###,###,###,##0.00";
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    }

                    OnCambioProgreso(90);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    posicionRango++;

                    //Tipo de cambio
                    rango = hojaExcel.get_Range("A" + posicionRango);
                    rango.Value2 = "Tipo de cambio: " + TipoDeCambio;
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    posicionRango++;

                    //Generado por
                    rango = hojaExcel.get_Range("A" + posicionRango);
                    rango.Value2 = "Reporte generado por: " + Usuario;
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //AutoAjustar
                    hojaExcel.get_Range("A2:Q2").Columns.AutoFit();
                    hojaExcel.get_Range("A2:A1000").Columns.AutoFit();
                    hojaExcel.get_Range("B2:B1000").Columns.AutoFit();
                    hojaExcel.get_Range("C2:C1000").Columns.AutoFit();
                    hojaExcel.get_Range("D2:D1000").Columns.AutoFit();
                    hojaExcel.get_Range("E2:E1000").Columns.AutoFit();
                    //hojaExcel.get_Range("F2:F1000").Columns.AutoFit();
                    hojaExcel.get_Range("G2:G1000").Columns.AutoFit();
                    hojaExcel.get_Range("H2:G1000").Columns.AutoFit();
                    hojaExcel.get_Range("I2:E1000").Columns.AutoFit();
                    hojaExcel.get_Range("J2:F1000").Columns.AutoFit();
                    hojaExcel.get_Range("K2:G1000").Columns.AutoFit();
                    hojaExcel.get_Range("L2:G1000").Columns.AutoFit();
                    hojaExcel.get_Range("M2:E1000").Columns.AutoFit();
                    hojaExcel.get_Range("N2:F1000").Columns.AutoFit();
                    hojaExcel.get_Range("O2:G1000").Columns.AutoFit();
                    hojaExcel.get_Range("F2:F500").Columns.ColumnWidth = 20;

                    OnCambioProgreso(100);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //MostrarReporte
                    filename = GestorReportes.Properties.Settings.Default.RutaRepositorio + @"DetalleFlujoNeto_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + ".xlsx";
                    libroExcel.SaveAs(filename);
                    aplicacionExcel.Visible = true;
                }
                catch(Exception ex)
                {
                    error = "- Error en Interop Excel " + Environment.NewLine + ex.Message;
                }
            }
            return error;
        }

        private bool getDetalles()
        {
            try
            {
                Inmobiliaria inmobiliaria = new Inmobiliaria();
                DataTable dtGpo = inmobiliaria.getDtGrupoEmpresarial(GrupoEmpresarial);
                if (GrupoEmpresarial == "Todos")
                    nombreGpoEmp = "Todos";
                else
                {
                    if (dtGpo.Rows.Count > 0)
                    {
                        nombreGpoEmp = dtGpo.Rows[0]["P0002_NOMBRE"].ToString();
                    }
                    else
                    {
                        error = "- Error al obtener el grupo empresarial";
                        return false;
                    }
                }
                DataTable dtGastosNames = inmobiliaria.getDtGastosNombres();
                if (dtGastosNames.Rows.Count > 0)
                {
                    foreach (DataRow gtoN in dtGastosNames.Rows)
                    {
                        gastosNombres.Add(gtoN[0].ToString());
                    }
                }

                OnCambioProgreso(10);
                if (CancelacionPendiente)
                {
                    error = "Proceso cancelado por el usuario";
                    return false;
                }

                if (Estatus == "Todos" || Estatus == "Rentados")
                {
                    DataTable dtRentados = inmobiliaria.getDtSubRentadosFlujoNeto(GrupoEmpresarial, Vigencia);
                    if (dtRentados.Rows.Count > 0)
                    {
                        hayRentados = true;
                        DataRow[] rowsRentPesos = dtRentados.Select("P0407_MONEDA_FACT = 'P'");
                        
                        foreach (DataRow rowRentSubs in rowsRentPesos)
                        {
                            if (!string.IsNullOrEmpty(rowRentSubs["P1801_ID_SUBCONJUNTO"].ToString()))
                            {
                                if (!idsSubsRentadosP.Contains(rowRentSubs["P1801_ID_SUBCONJUNTO"].ToString()))
                                {
                                    idsSubsRentadosP.Add(rowRentSubs["P1801_ID_SUBCONJUNTO"].ToString());

                                    DataTable dtInmubPorSub = inmobiliaria.getDtInmueblesPorSubConj(rowRentSubs["P1801_ID_SUBCONJUNTO"].ToString());

                                    subRentadosP.Add(rowRentSubs["P0103_RAZON_SOCIAL"].ToString());

                                    if (!string.IsNullOrEmpty(rowRentSubs["P0506_CIUDAD"].ToString()))
                                        municipioP.Add(rowRentSubs["P0506_CIUDAD"].ToString());
                                    else
                                        municipioP.Add("-");

                                    if (!string.IsNullOrEmpty(rowRentSubs["P0507_ESTADO"].ToString()))
                                        estadoP.Add(rowRentSubs["P0507_ESTADO"].ToString());
                                    else
                                        estadoP.Add("-");

                                    if (!string.IsNullOrEmpty(rowRentSubs["P0503_CALLE_NUM"].ToString()) || !string.IsNullOrEmpty(rowRentSubs["P0504_COLONIA"].ToString()))
                                        direccionP.Add(rowRentSubs["P0503_CALLE_NUM"].ToString() + ", " + rowRentSubs["P0504_COLONIA"].ToString());
                                    else
                                        direccionP.Add("-");

                                    if (!string.IsNullOrEmpty(rowRentSubs["P18_CAMPO1"].ToString()))
                                        identifP.Add(rowRentSubs["P18_CAMPO1"].ToString());
                                    else
                                        identifP.Add("-");


                                    if (!string.IsNullOrEmpty(rowRentSubs["P0303_NOMBRE"].ToString()))
                                        conceptoP.Add(rowRentSubs["P0303_NOMBRE"].ToString());
                                    else
                                        conceptoP.Add("-");

                                    string expCat = string.Empty;
                                    decimal valCat = 0;
                                    decimal terrM2 = 0;
                                    decimal constM2 = 0;
                                    foreach (DataRow dr in dtInmubPorSub.Rows)
                                    {
                                        if (!string.IsNullOrEmpty(dr["P0708_PREDIAL"].ToString()))
                                            expCat += dr["P0708_PREDIAL"].ToString() + "| ";
                                        if (!string.IsNullOrEmpty(dr["P1922_A_MIN_ING"].ToString()))
                                            valCat += Decimal.Round(Convert.ToDecimal(dr["P1922_A_MIN_ING"].ToString()), 2);
                                        if (!string.IsNullOrEmpty(dr["P1926_CIST_ING"].ToString()))
                                            terrM2 += Decimal.Round(Convert.ToDecimal(dr["P1926_CIST_ING"].ToString()), 2);
                                        if (!string.IsNullOrEmpty(dr["CAMPO_NUM1"].ToString()))
                                            constM2 += Decimal.Round(Convert.ToDecimal(dr["CAMPO_NUM1"].ToString()), 2);
                                    }
                                    if (string.IsNullOrEmpty(expCat))
                                        expCat = "-";

                                    expCatastP.Add(expCat);
                                    valCatastP.Add(valCat);
                                    terrenosP.Add(terrM2);
                                    constP.Add(constM2);

                                    List<string> listContrat = new List<string>();
                                    decimal rentM = 0;
                                    DataTable dtContratosPSub = inmobiliaria.getDtContratosPorSubconj(rowRentSubs["P1801_ID_SUBCONJUNTO"].ToString(), Vigencia.Year, true);
                                    if (dtContratosPSub.Rows.Count > 0)
                                    {
                                        if (dtContratosPSub.Rows.Count == 1)
                                        {
                                            listContrat.Add(dtContratosPSub.Rows[0]["P0401_ID_CONTRATO"].ToString());
                                            arrendatariosP.Add(dtContratosPSub.Rows[0]["P0203_NOMBRE"].ToString());
                                            try
                                            {
                                                rentM = Decimal.Round(Convert.ToDecimal(dtContratosPSub.Rows[0]["P0408_IMPORTE_FACT"].ToString()), 2);
                                            }
                                            catch
                                            {
                                                rentM = 0;
                                            }
                                        }
                                        else
                                        {
                                            string arrenda = string.Empty;
                                            foreach (DataRow rowClientes in dtContratosPSub.Rows)
                                            {
                                                listContrat.Add(rowClientes["P0401_ID_CONTRATO"].ToString());
                                                arrenda += rowClientes["P0203_NOMBRE"].ToString() + "| ";
                                                try
                                                {
                                                    rentM += Decimal.Round(Convert.ToDecimal(dtContratosPSub.Rows[0]["P0408_IMPORTE_FACT"].ToString()), 2);
                                                }
                                                catch
                                                {
                                                    rentM += 0;
                                                }
                                                
                                            }
                                            if(string.IsNullOrEmpty(arrenda))
                                                arrendatariosP.Add("-");
                                            else
                                                arrendatariosP.Add(arrenda);
                                        }
                                    }
                                    else
                                        arrendatariosP.Add("-");

                                    rentaMensualP.Add(rentM);

                                    decimal rentA = 0;
                                    decimal importNuevo = 0;
                                    DateTime fechaIncremento = DateTime.Now.AddYears(-50);//2.3.2.30
                                    string baseParaAumento = "INPC";
                                    DateTime finVigNvo = DateTime.Now.AddYears(-50);
                                    DateTime iniVigNvo = DateTime.Now.AddYears(-50);
                                    DateTime vigIni = new DateTime(Vigencia.Year, 1, 1);
                                    DateTime vigFin = new DateTime(Vigencia.Year, 12, 31);
                                    foreach (string contrat in listContrat)
                                    {
                                        DataTable dtHistCont = inmobiliaria.getDtContratosHistPorIdContrato(contrat, Vigencia);
                                        if (dtHistCont.Rows.Count > 0)
                                        {
                                            if (!string.IsNullOrEmpty(dtHistCont.Rows[0]["P0432_FECHA_AUMENTO"].ToString()))
                                                fechaIncremento = Convert.ToDateTime(dtHistCont.Rows[0]["P0432_FECHA_AUMENTO"]);
                                            iniVigNvo = Convert.ToDateTime(dtHistCont.Rows[0]["P5911_INICIO_VIG_NVO"]);
                                            finVigNvo = Convert.ToDateTime(dtHistCont.Rows[0]["P5913_FIN_VIG_NVO"]);
                                            importNuevo = Convert.ToDecimal(dtHistCont.Rows[0]["P5941_IMP_ACTUAL_NVO"].ToString());
                                            if (!string.IsNullOrEmpty(dtHistCont.Rows[0]["P0441_BASE_PARA_AUMENTO"].ToString()))
                                                baseParaAumento = dtHistCont.Rows[0]["P0441_BASE_PARA_AUMENTO"].ToString();
                                            if (fechaIncremento < vigFin && fechaIncremento >= DateTime.Now && baseParaAumento.Trim() == "PRC") //2.3.2.30
                                            {
                                                int mesesAnterior = 0;
                                                int mesesNuevo = 0;
                                                decimal importeConAumento = 0;
                                                decimal aumento = 1;
                                                try { aumento = (Convert.ToDecimal(dtHistCont.Rows[0]["P0415_AUMENTO_ANUAL"].ToString()) / 100) + 1; }
                                                catch { aumento = 1; }
                                                importeConAumento = importNuevo * aumento;
                                                if (iniVigNvo <= vigIni && finVigNvo >= vigFin)
                                                {
                                                    mesesNuevo = 13 - fechaIncremento.Month;
                                                    mesesAnterior = 12 - mesesNuevo;
                                                    if (mesesAnterior > 0)
                                                    {
                                                        rentA += Decimal.Round((importNuevo * mesesAnterior), 2);
                                                    }
                                                    rentA += Decimal.Round((importeConAumento * mesesNuevo), 2);
                                                }
                                                else if (iniVigNvo <= vigIni && finVigNvo >= vigIni && finVigNvo <= vigFin)
                                                {
                                                    mesesNuevo = (finVigNvo.Month + 1) - fechaIncremento.Month;
                                                    mesesAnterior = finVigNvo.Month - mesesNuevo;
                                                    if (mesesAnterior > 0)
                                                    {
                                                        rentA += Decimal.Round((importNuevo * mesesAnterior), 2);
                                                    }
                                                    rentA += Decimal.Round((importeConAumento * mesesNuevo), 2);
                                                }
                                                else if (iniVigNvo >= vigIni && iniVigNvo <= vigFin && finVigNvo >= vigFin)
                                                {
                                                    int meses = 13 - iniVigNvo.Month;
                                                    mesesNuevo = 13 - fechaIncremento.Month;
                                                    mesesAnterior = meses - mesesNuevo;
                                                    if (mesesAnterior > 0)
                                                    {
                                                        rentA += Decimal.Round((importNuevo * mesesAnterior), 2);
                                                    }
                                                    rentA += Decimal.Round((importeConAumento * mesesNuevo), 2);
                                                }
                                                else
                                                    rentA += 0;
                                            }
                                            else
                                            {
                                                if (iniVigNvo <= vigIni && finVigNvo >= vigFin)
                                                {
                                                    rentA += Decimal.Round((importNuevo * 12), 2);
                                                }
                                                else if (iniVigNvo <= vigIni && finVigNvo >= vigIni && finVigNvo <= vigFin)
                                                {
                                                    rentA += Decimal.Round((importNuevo * finVigNvo.Month), 2);
                                                }
                                                else if (iniVigNvo >= vigIni && iniVigNvo <= vigFin && finVigNvo >= vigFin)
                                                {
                                                    int meses = 13 - iniVigNvo.Month;
                                                    rentA += Decimal.Round((importNuevo * meses), 2);
                                                }
                                                else
                                                    rentA += 0;
                                            }
                                        }
                                        else
                                            rentA += 0;
                                    }
                                    rentaAnualP.Add(rentA);
                                }
                            }
                        }

                        OnCambioProgreso(20);
                        if (CancelacionPendiente)
                        {
                            error = "Proceso cancelado por el usuario";
                            return false;
                        }

                        DataRow[] rowsRentDolares = dtRentados.Select("P0407_MONEDA_FACT = 'D'");
                        foreach (DataRow rowRentSubs in rowsRentDolares)
                        {
                            if (!string.IsNullOrEmpty(rowRentSubs["P1801_ID_SUBCONJUNTO"].ToString()))
                            {
                                if (!idsSubsRentadosD.Contains(rowRentSubs["P1801_ID_SUBCONJUNTO"].ToString()))
                                {
                                    idsSubsRentadosD.Add(rowRentSubs["P1801_ID_SUBCONJUNTO"].ToString());

                                    DataTable dtInmubPorSub = inmobiliaria.getDtInmueblesPorSubConj(rowRentSubs["P1801_ID_SUBCONJUNTO"].ToString());

                                    subRentadosD.Add(rowRentSubs["P0103_RAZON_SOCIAL"].ToString());
                                    
                                    if (!string.IsNullOrEmpty(rowRentSubs["P0506_CIUDAD"].ToString()))
                                        municipioD.Add(rowRentSubs["P0506_CIUDAD"].ToString());
                                    else
                                        municipioD.Add("-");

                                    if (!string.IsNullOrEmpty(rowRentSubs["P0507_ESTADO"].ToString()))
                                        estadoD.Add(rowRentSubs["P0507_ESTADO"].ToString());
                                    else
                                        estadoD.Add("-");

                                    if (!string.IsNullOrEmpty(rowRentSubs["P0503_CALLE_NUM"].ToString()) || !string.IsNullOrEmpty(rowRentSubs["P0504_COLONIA"].ToString()))
                                        direccionD.Add(rowRentSubs["P0503_CALLE_NUM"].ToString() + ", " + rowRentSubs["P0504_COLONIA"].ToString());
                                    else
                                        direccionD.Add("-");
                                    
                                    if (!string.IsNullOrEmpty(rowRentSubs["P18_CAMPO1"].ToString()))
                                        identifD.Add(rowRentSubs["P18_CAMPO1"].ToString());
                                    else
                                        identifD.Add("-");


                                    if (!string.IsNullOrEmpty(rowRentSubs["P0303_NOMBRE"].ToString()))
                                        conceptoD.Add(rowRentSubs["P0303_NOMBRE"].ToString());
                                    else
                                        conceptoD.Add("-");
                                    
                                    string expCat = string.Empty;
                                    decimal valCat = 0;
                                    decimal terrM2 = 0;
                                    decimal constM2 = 0;
                                    foreach (DataRow dr in dtInmubPorSub.Rows)
                                    {
                                        if (!string.IsNullOrEmpty(dr["P0708_PREDIAL"].ToString()))
                                            expCat += dr["P0708_PREDIAL"].ToString() + "| ";
                                        if (!string.IsNullOrEmpty(dr["P1922_A_MIN_ING"].ToString()))
                                            valCat += Decimal.Round(Convert.ToDecimal(dr["P1922_A_MIN_ING"].ToString()), 2);
                                        if (!string.IsNullOrEmpty(dr["P1926_CIST_ING"].ToString()))
                                            terrM2 += Decimal.Round(Convert.ToDecimal(dr["P1926_CIST_ING"].ToString()), 2);
                                        if (!string.IsNullOrEmpty(dr["CAMPO_NUM1"].ToString()))
                                            constM2 += Decimal.Round(Convert.ToDecimal(dr["CAMPO_NUM1"].ToString()), 2);
                                    }
                                    if (string.IsNullOrEmpty(expCat))
                                        expCat = "-";

                                    expCatastD.Add(expCat);
                                    valCatastD.Add(valCat);
                                    terrenosD.Add(terrM2);
                                    constD.Add(constM2);
                                    
                                    List<string> listContrat = new List<string>();
                                    decimal rentM = 0;
                                    DataTable dtContratosDSub = inmobiliaria.getDtContratosPorSubconj(rowRentSubs["P1801_ID_SUBCONJUNTO"].ToString(), Vigencia.Year, false);
                                    if (dtContratosDSub.Rows.Count > 0)
                                    {
                                        if (dtContratosDSub.Rows.Count == 1)
                                        {
                                            listContrat.Add(dtContratosDSub.Rows[0]["P0401_ID_CONTRATO"].ToString());
                                            arrendatariosD.Add(dtContratosDSub.Rows[0]["P0203_NOMBRE"].ToString());
                                            try
                                            {
                                                rentM = Decimal.Round(Convert.ToDecimal(dtContratosDSub.Rows[0]["P0408_IMPORTE_FACT"].ToString()) * TipoDeCambio, 2);
                                            }
                                            catch
                                            {
                                                rentM = 0;
                                            }
                                        }
                                        else
                                        {
                                            string arrenda = string.Empty;
                                            foreach (DataRow rowClientes in dtContratosDSub.Rows)
                                            {
                                                listContrat.Add(rowClientes["P0401_ID_CONTRATO"].ToString());
                                                arrenda += rowClientes["P0203_NOMBRE"].ToString() + "| ";
                                                try
                                                {
                                                    rentM += Decimal.Round(Convert.ToDecimal(dtContratosDSub.Rows[0]["P0408_IMPORTE_FACT"].ToString()) * TipoDeCambio, 2);
                                                }
                                                catch
                                                {
                                                    rentM += 0;
                                                }
                                                
                                            }
                                            if(string.IsNullOrEmpty(arrenda))
                                                arrendatariosD.Add("-");
                                            else
                                                arrendatariosD.Add(arrenda);
                                        }
                                    }
                                    else
                                        arrendatariosD.Add("-");

                                    rentaMensualD.Add(rentM);
                                    
                                    decimal rentA = 0;
                                    decimal importNuevo = 0;
                                    DateTime fechaIncremento = DateTime.Now.AddYears(-50);//2.3.2.30
                                    string baseParaAumento = "INPC";
                                    DateTime finVigNvo = DateTime.Now.AddYears(-50);
                                    DateTime iniVigNvo = DateTime.Now.AddYears(-50);
                                    DateTime vigIni = new DateTime(Vigencia.Year, 1, 1);
                                    DateTime vigFin = new DateTime(Vigencia.Year, 12, 31);
                                    foreach (string contrat in listContrat)
                                    {
                                        DataTable dtHistCont = inmobiliaria.getDtContratosHistPorIdContrato(contrat, Vigencia);
                                        if (dtHistCont.Rows.Count > 0)
                                        {
                                            if (!string.IsNullOrEmpty(dtHistCont.Rows[0]["P0432_FECHA_AUMENTO"].ToString()))
                                                fechaIncremento = Convert.ToDateTime(dtHistCont.Rows[0]["P0432_FECHA_AUMENTO"]);
                                            iniVigNvo = Convert.ToDateTime(dtHistCont.Rows[0]["P5911_INICIO_VIG_NVO"]);
                                            finVigNvo = Convert.ToDateTime(dtHistCont.Rows[0]["P5913_FIN_VIG_NVO"]);
                                            importNuevo = Convert.ToDecimal(dtHistCont.Rows[0]["P5941_IMP_ACTUAL_NVO"].ToString());
                                            if (!string.IsNullOrEmpty(dtHistCont.Rows[0]["P0441_BASE_PARA_AUMENTO"].ToString()))
                                                baseParaAumento = dtHistCont.Rows[0]["P0441_BASE_PARA_AUMENTO"].ToString();
                                            if (fechaIncremento < vigFin && fechaIncremento >= DateTime.Now && baseParaAumento.Trim() == "PRC") //2.3.2.30
                                            {
                                                int mesesAnterior = 0;
                                                int mesesNuevo = 0;
                                                decimal importeConAumento = 0;
                                                decimal aumento = 1;
                                                try { aumento = (Convert.ToDecimal(dtHistCont.Rows[0]["P0415_AUMENTO_ANUAL"].ToString()) / 100) + 1; }
                                                catch { aumento = 1; }
                                                importeConAumento = importNuevo * aumento;
                                                if (iniVigNvo <= vigIni && finVigNvo >= vigFin)
                                                {
                                                    mesesNuevo = 13 - fechaIncremento.Month;
                                                    mesesAnterior = 12 - mesesNuevo;
                                                    if (mesesAnterior > 0)
                                                    {
                                                        rentA += Decimal.Round((importNuevo * mesesAnterior) * TipoDeCambio, 2);
                                                    }
                                                    rentA += Decimal.Round((importeConAumento * mesesNuevo) * TipoDeCambio, 2);
                                                }
                                                else if (iniVigNvo <= vigIni && finVigNvo >= vigIni && finVigNvo <= vigFin)
                                                {
                                                    mesesNuevo = (finVigNvo.Month + 1) - fechaIncremento.Month;
                                                    mesesAnterior = finVigNvo.Month - mesesNuevo;
                                                    if (mesesAnterior > 0)
                                                    {
                                                        rentA += Decimal.Round((importNuevo * mesesAnterior) * TipoDeCambio, 2);
                                                    }
                                                    rentA += Decimal.Round((importeConAumento * mesesNuevo) * TipoDeCambio, 2);
                                                }
                                                else if (iniVigNvo >= vigIni && iniVigNvo <= vigFin && finVigNvo >= vigFin)
                                                {
                                                    int meses = 13 - iniVigNvo.Month;
                                                    mesesNuevo = 13 - fechaIncremento.Month;
                                                    mesesAnterior = meses - mesesNuevo; 
                                                    if (mesesAnterior > 0)
                                                    {
                                                        rentA += Decimal.Round((importNuevo * mesesAnterior) * TipoDeCambio, 2);
                                                    }
                                                    rentA += Decimal.Round((importeConAumento * mesesNuevo) * TipoDeCambio, 2);
                                                }
                                                else
                                                    rentA += 0;
                                            }
                                            else
                                            {
                                                if (iniVigNvo <= vigIni && finVigNvo >= vigFin)
                                                {
                                                    rentA += Decimal.Round((importNuevo * 12) * TipoDeCambio, 2);
                                                }
                                                else if (iniVigNvo <= vigIni && finVigNvo >= vigIni && finVigNvo <= vigFin)
                                                {
                                                    rentA += Decimal.Round((importNuevo * finVigNvo.Month) * TipoDeCambio, 2);
                                                }
                                                else if (iniVigNvo >= vigIni && iniVigNvo <= vigFin && finVigNvo >= vigFin)
                                                {
                                                    int meses = 13 - iniVigNvo.Month;
                                                    rentA += Decimal.Round((importNuevo * meses) * TipoDeCambio, 2);
                                                }
                                                else
                                                    rentA += 0;
                                            }
                                        }
                                        else
                                            rentA += 0;
                                    }
                                    rentaAnualD.Add(rentA);
                                }
                            }
                        }
                    }
                }

                OnCambioProgreso(30);
                if (CancelacionPendiente)
                {
                    error = "Proceso cancelado por el usuario";
                    return false;
                }

                if (Estatus == "Todos" || Estatus == "No Rentados")
                {
                    DataTable dtNoRentados = inmobiliaria.getDtSubConjuntosNoRentadosPorGpoEmp(GrupoEmpresarial);
                    if (dtNoRentados.Rows.Count > 0)
                    {
                        hayNoRentados = true;
                        foreach (DataRow drNR in dtNoRentados.Rows)
                        {
                            if (!string.IsNullOrEmpty(drNR["P1801_ID_SUBCONJUNTO"].ToString()))
                            {
                                subNoRentados.Add(drNR["P0103_RAZON_SOCIAL"].ToString());

                                if (!string.IsNullOrEmpty(drNR["P0506_CIUDAD"].ToString()))
                                    municipioNR.Add(drNR["P0506_CIUDAD"].ToString());
                                else
                                    municipioNR.Add("-");

                                if (!string.IsNullOrEmpty(drNR["P0507_ESTADO"].ToString()))
                                    estadoNR.Add(drNR["P0507_ESTADO"].ToString());
                                else
                                    estadoNR.Add("-");

                                if (!string.IsNullOrEmpty(drNR["P0503_CALLE_NUM"].ToString()) || !string.IsNullOrEmpty(drNR["P0504_COLONIA"].ToString()))
                                    direccionNR.Add(drNR["P0503_CALLE_NUM"].ToString() + ", " + drNR["P0504_COLONIA"].ToString());
                                else
                                    direccionNR.Add("-");

                                if (!string.IsNullOrEmpty(drNR["P18_CAMPO1"].ToString()))
                                    identifNR.Add(drNR["P18_CAMPO1"].ToString());
                                else
                                    identifNR.Add("-");


                                if (!string.IsNullOrEmpty(drNR["P0303_NOMBRE"].ToString()))
                                    conceptoNR.Add(drNR["P0303_NOMBRE"].ToString());
                                else
                                    conceptoNR.Add("-");

                                string expCat = string.Empty;
                                decimal catast = 0;
                                decimal terrM2 = 0;
                                decimal constM2 = 0;
                                DataTable dtInmubPorSub = inmobiliaria.getDtInmueblesPorSubConj(drNR["P1801_ID_SUBCONJUNTO"].ToString());
                                foreach (DataRow rowInmb in dtInmubPorSub.Rows)
                                {
                                    if (!string.IsNullOrEmpty(rowInmb["P0708_PREDIAL"].ToString()))
                                        expCat += rowInmb["P0708_PREDIAL"].ToString() + "| ";
                                    if (!string.IsNullOrEmpty(rowInmb["P1922_A_MIN_ING"].ToString()))
                                        catast += Decimal.Round(Convert.ToDecimal(rowInmb["P1922_A_MIN_ING"].ToString().Trim()), 2);
                                    if (!string.IsNullOrEmpty(rowInmb["P1926_CIST_ING"].ToString()))
                                        terrM2 += Decimal.Round(Convert.ToDecimal(rowInmb["P1926_CIST_ING"].ToString()), 2);
                                    if (!string.IsNullOrEmpty(rowInmb["CAMPO_NUM1"].ToString()))
                                        constM2 += Decimal.Round(Convert.ToDecimal(rowInmb["CAMPO_NUM1"].ToString()), 2);
                                }
                                if (string.IsNullOrEmpty(expCat))
                                    expCat = "-";

                                expCatastNR.Add(expCat);
                                valCatastNR.Add(catast);
                                terrenosNR.Add(terrM2);
                                constNR.Add(constM2);
                            }
                        }

                        OnCambioProgreso(40);
                        if (CancelacionPendiente)
                        {
                            error = "Proceso cancelado por el usuario";
                            return false;
                        }

                        //Aqui para gastos
                        DataRow[] drsNoRentadosPesos = dtNoRentados.Select("P18_CAMPO2 = 'P'");
                        if (drsNoRentadosPesos.Length > 0)
                        {
                            inmobiliariasGP = new string[drsNoRentadosPesos.Length];
                            municipioGP = new string[drsNoRentadosPesos.Length];
                            estadoGP = new string[drsNoRentadosPesos.Length];
                            direccionGP = new string[drsNoRentadosPesos.Length];
                            identifGP = new string[drsNoRentadosPesos.Length];
                            expCatastGP = new string[drsNoRentadosPesos.Length];
                            conceptosGP = new string[drsNoRentadosPesos.Length];
                            valCatastGP = new decimal[drsNoRentadosPesos.Length];
                            terrenosGP = new decimal[drsNoRentadosPesos.Length];
                            constGP = new decimal[drsNoRentadosPesos.Length];
                            predialGP = new decimal[drsNoRentadosPesos.Length];
                            matrizGP = new decimal[drsNoRentadosPesos.Length, gastosNombres.Count];

                            int contador = 0;
                            foreach (DataRow drGp in drsNoRentadosPesos)
                            {
                                if (!string.IsNullOrEmpty(drGp["P0103_RAZON_SOCIAL"].ToString()))
                                    inmobiliariasGP[contador] = drGp["P0103_RAZON_SOCIAL"].ToString();
                                else
                                    inmobiliariasGP[contador] = "-";


                                if (!string.IsNullOrEmpty(drGp["P0506_CIUDAD"].ToString()))
                                    municipioGP[contador] = drGp["P0506_CIUDAD"].ToString();
                                else
                                    municipioGP[contador] = "-";

                                if (!string.IsNullOrEmpty(drGp["P0507_ESTADO"].ToString()))
                                    estadoGP[contador] = drGp["P0507_ESTADO"].ToString();
                                else
                                    estadoGP[contador] = "-";

                                if (!string.IsNullOrEmpty(drGp["P0503_CALLE_NUM"].ToString()) || !string.IsNullOrEmpty(drGp["P0504_COLONIA"].ToString()))
                                    direccionGP[contador] = drGp["P0503_CALLE_NUM"].ToString() + ", " + drGp["P0504_COLONIA"].ToString();
                                else
                                    direccionGP[contador] = "-";

                                if (!string.IsNullOrEmpty(drGp["P18_CAMPO1"].ToString()))
                                    identifGP[contador] = drGp["P18_CAMPO1"].ToString();
                                else
                                    identifGP[contador] = "-";

                                if (!string.IsNullOrEmpty(drGp["P0303_NOMBRE"].ToString()))
                                    conceptosGP[contador] = drGp["P0303_NOMBRE"].ToString();
                                else
                                    conceptosGP[contador] = "-";

                                string expCat = string.Empty;
                                decimal catast = 0;
                                decimal terrM2 = 0;
                                decimal constM2 = 0;
                                DataTable dtInmubPorSub = inmobiliaria.getDtInmueblesPorSubConj(drGp["P1801_ID_SUBCONJUNTO"].ToString());
                                foreach (DataRow rowInmb in dtInmubPorSub.Rows)
                                {
                                    if (!string.IsNullOrEmpty(rowInmb["P0708_PREDIAL"].ToString()))
                                        expCat += rowInmb["P0708_PREDIAL"].ToString() + "| ";
                                    if (!string.IsNullOrEmpty(rowInmb["P1922_A_MIN_ING"].ToString()))
                                        catast += Decimal.Round(Convert.ToDecimal(rowInmb["P1922_A_MIN_ING"].ToString().Trim()), 2);
                                    if (!string.IsNullOrEmpty(rowInmb["P1926_CIST_ING"].ToString()))
                                        terrM2 += Decimal.Round(Convert.ToDecimal(rowInmb["P1926_CIST_ING"].ToString()), 2);
                                    if (!string.IsNullOrEmpty(rowInmb["CAMPO_NUM1"].ToString()))
                                        constM2 += Decimal.Round(Convert.ToDecimal(rowInmb["CAMPO_NUM1"].ToString()), 2);
                                }
                                if (string.IsNullOrEmpty(expCat))
                                    expCat = "-";

                                expCatastGP[contador] = expCat;
                                valCatastGP[contador] = catast;
                                terrenosGP[contador] = terrM2;
                                constGP[contador] = constM2;
                                //prueba += drGp["P1801_ID_SUBCONJUNTO"].ToString() + Environment.NewLine;
                                decimal predialGastPesos = 0;
                                DataTable dtPredialGP = inmobiliaria.getDtPredialesPorSubConjunto(drGp["P1801_ID_SUBCONJUNTO"].ToString());
                                if (dtPredialGP.Rows.Count > 0)
                                {
                                    foreach (DataRow drPred in dtPredialGP.Rows)
                                    {
                                        if (!string.IsNullOrEmpty(drPred["CAMPO_NUM2"].ToString()))
                                            predialGastPesos += Decimal.Round(Convert.ToDecimal(drPred["CAMPO_NUM2"].ToString()), 2);
                                    }
                                }
                                predialGP[contador] = predialGastPesos;

                                for (int i = 0; i < gastosNombres.Count; i++)
                                {
                                    decimal gtos = 0;
                                    DataTable dtGastosP = inmobiliaria.getDtGastosDeInmueblesPorSubConj(drGp["P1801_ID_SUBCONJUNTO"].ToString(), gastosNombres[i]);
                                    if (dtGastosP.Rows.Count > 0)
                                    {
                                        foreach (DataRow drGto in dtGastosP.Rows)
                                        {
                                            if(!string.IsNullOrEmpty(drGto["CAMPO_NUM4"].ToString()))
                                                gtos += Decimal.Round(Convert.ToDecimal(drGto["CAMPO_NUM4"].ToString()), 2);
                                        }
                                    }
                                    matrizGP[contador, i] = gtos;
                                }
                                contador++;
                            }
                        }

                        OnCambioProgreso(50);
                        if (CancelacionPendiente)
                        {
                            error = "Proceso cancelado por el usuario";
                            return false;
                        }

                        DataRow[] drsNoRentadosDolares = dtNoRentados.Select("P18_CAMPO2 = 'D'");
                        if (drsNoRentadosDolares.Length > 0)
                        {
                            inmobiliariasGD = new string[drsNoRentadosDolares.Length];
                            municipioGD = new string[drsNoRentadosDolares.Length];
                            estadoGD = new string[drsNoRentadosDolares.Length];
                            direccionGD = new string[drsNoRentadosDolares.Length];
                            identifGD = new string[drsNoRentadosDolares.Length];
                            expCatastGD = new string[drsNoRentadosDolares.Length];
                            conceptosGD = new string[drsNoRentadosDolares.Length];
                            valCatastGD = new decimal[drsNoRentadosDolares.Length];
                            terrenosGD = new decimal[drsNoRentadosDolares.Length];
                            constGD = new decimal[drsNoRentadosDolares.Length];
                            predialGD = new decimal[drsNoRentadosDolares.Length];
                            matrizGD = new decimal[drsNoRentadosDolares.Length, gastosNombres.Count];
                            
                            int contador = 0;
                            foreach (DataRow drGp in drsNoRentadosDolares)
                            {
                                if (!string.IsNullOrEmpty(drGp["P0103_RAZON_SOCIAL"].ToString()))
                                    inmobiliariasGD[contador] = drGp["P0103_RAZON_SOCIAL"].ToString();
                                else
                                    inmobiliariasGD[contador] = "-";

                                
                                if (!string.IsNullOrEmpty(drGp["P0506_CIUDAD"].ToString()))
                                    municipioGD[contador] = drGp["P0506_CIUDAD"].ToString();
                                else
                                    municipioGD[contador] = "-";
                                
                                if (!string.IsNullOrEmpty(drGp["P0507_ESTADO"].ToString()))
                                    estadoGD[contador] = drGp["P0507_ESTADO"].ToString();
                                else
                                    estadoGD[contador] = "-";
                                
                                if (!string.IsNullOrEmpty(drGp["P0503_CALLE_NUM"].ToString()) || !string.IsNullOrEmpty(drGp["P0504_COLONIA"].ToString()))
                                    direccionGD[contador] = drGp["P0503_CALLE_NUM"].ToString() + ", " + drGp["P0504_COLONIA"].ToString();
                                else
                                    direccionGD[contador] = "-";
                                
                                if (!string.IsNullOrEmpty(drGp["P18_CAMPO1"].ToString()))
                                    identifGD[contador] = drGp["P18_CAMPO1"].ToString();
                                else
                                    identifGD[contador] = "-";
                                
                                if (!string.IsNullOrEmpty(drGp["P0303_NOMBRE"].ToString()))
                                    conceptosGD[contador] = drGp["P0303_NOMBRE"].ToString();
                                else
                                    conceptosGD[contador] = "-";
                                
                                string expCat = string.Empty;
                                decimal catast = 0;
                                decimal terrM2 = 0;
                                decimal constM2 = 0;
                                DataTable dtInmubPorSub = inmobiliaria.getDtInmueblesPorSubConj(drGp["P1801_ID_SUBCONJUNTO"].ToString());
                                foreach (DataRow rowInmb in dtInmubPorSub.Rows)
                                {
                                    if (!string.IsNullOrEmpty(rowInmb["P0708_PREDIAL"].ToString()))
                                        expCat += rowInmb["P0708_PREDIAL"].ToString() + "| ";
                                    if (!string.IsNullOrEmpty(rowInmb["P1922_A_MIN_ING"].ToString()))
                                        catast += Decimal.Round(Convert.ToDecimal(rowInmb["P1922_A_MIN_ING"].ToString().Trim()), 2);
                                    if (!string.IsNullOrEmpty(rowInmb["P1926_CIST_ING"].ToString()))
                                        terrM2 += Decimal.Round(Convert.ToDecimal(rowInmb["P1926_CIST_ING"].ToString()), 2);
                                    if (!string.IsNullOrEmpty(rowInmb["CAMPO_NUM1"].ToString()))
                                        constM2 += Decimal.Round(Convert.ToDecimal(rowInmb["CAMPO_NUM1"].ToString()), 2);
                                }
                                if (string.IsNullOrEmpty(expCat))
                                    expCat = "-";
                                
                                expCatastGD[contador] = expCat;
                                valCatastGD[contador] = catast;
                                terrenosGD[contador] = terrM2;
                                constGD[contador] = constM2;
                                
                                decimal predialGastPesos = 0;
                                DataTable dtPredialGD = inmobiliaria.getDtPredialesPorSubConjunto(drGp["P1801_ID_SUBCONJUNTO"].ToString());
                               // prueba += drGp["P1801_ID_SUBCONJUNTO"].ToString() + Environment.NewLine;
                                if (dtPredialGD.Rows.Count > 0)
                                {
                                    foreach (DataRow drPred in dtPredialGD.Rows)
                                    {
                                        if (!string.IsNullOrEmpty(drPred["CAMPO_NUM2"].ToString()))
                                            predialGastPesos += Decimal.Round(Convert.ToDecimal(drPred["CAMPO_NUM2"].ToString()), 2);
                                    }
                                }
                                predialGD[contador] = predialGastPesos;
                                
                                for (int i = 0; i < gastosNombres.Count; i++)
                                {
                                    decimal gtos = 0;
                                    DataTable dtGastosD = inmobiliaria.getDtGastosDeInmueblesPorSubConj(drGp["P1801_ID_SUBCONJUNTO"].ToString(), gastosNombres[i]);
                                    if (dtGastosD.Rows.Count > 0)
                                    {
                                        foreach (DataRow drGto in dtGastosD.Rows)
                                        {
                                            if(!string.IsNullOrEmpty(drGto["CAMPO_NUM4"].ToString()))
                                                gtos += Decimal.Round(Convert.ToDecimal(drGto["CAMPO_NUM4"].ToString()) * TipoDeCambio, 2);
                                        }
                                    }
                                    matrizGD[contador, i] = gtos;
                                }

                                contador++;
                            }
                        }                    
                    }
                }

                OnCambioProgreso(60);
                if (CancelacionPendiente)
                {
                    error = "Proceso cancelado por el usuario";
                    return false;
                }

                if (!hayRentados && !hayNoRentados)
                {
                    error = "- Error al obtener los datos del reporte, no se encontraron registros para los criterios seleccionados";
                    return false;
                } 
                if (Estatus == "Todos" || Estatus == "Rentados")
                {
                    foreach (string idSubconjunto in idsSubsRentadosP)
                    {
                        //prueba += idSubconjunto + Environment.NewLine;
                        DataTable dtPredialSubCnj = inmobiliaria.getDtPredialesPorSubConjunto(idSubconjunto);
                        decimal predialAcum = 0;
                        foreach (DataRow drPredialesSub in dtPredialSubCnj.Rows)
                        {
                            if (!string.IsNullOrEmpty(drPredialesSub["CAMPO_NUM2"].ToString()))
                                predialAcum += Convert.ToDecimal(drPredialesSub["CAMPO_NUM2"].ToString());
                        }
                        predialesRentadosP.Add(Decimal.Round(predialAcum, 2));
                    }
                    foreach (string idSubconjunto in idsSubsRentadosD)
                    {
                        //prueba += idSubconjunto + Environment.NewLine;
                        DataTable dtPredialSubCnj = inmobiliaria.getDtPredialesPorSubConjunto(idSubconjunto);
                        decimal predialAcum = 0;
                        foreach (DataRow drPredialesSub in dtPredialSubCnj.Rows)
                        {
                            if (!string.IsNullOrEmpty(drPredialesSub["CAMPO_NUM2"].ToString()))
                                predialAcum += Convert.ToDecimal(drPredialesSub["CAMPO_NUM2"].ToString());
                        }
                        predialesRentadosD.Add(Decimal.Round(predialAcum, 2));
                    }

                    matrizGastosSub = new decimal[idsSubsRentadosP.Count, gastosNombres.Count];
                    matrizGastosSubD = new decimal[idsSubsRentadosD.Count, gastosNombres.Count];
                    for (int i = 0; i < idsSubsRentadosP.Count; i++)
                    {
                        for (int j = 0; j < gastosNombres.Count; j++)
                        {
                            decimal acumuladorGto = 0;
                            DataTable dtGastosPorSub = inmobiliaria.getDtGastosDeInmueblesPorSubConj(idsSubsRentadosP[i], gastosNombres[j]);
                            if (dtGastosPorSub.Rows.Count > 0)
                            {
                                foreach (DataRow drGto in dtGastosPorSub.Rows)
                                {
                                    acumuladorGto += Decimal.Round(Convert.ToDecimal(drGto["CAMPO_NUM4"].ToString()), 2);
                                }
                            }
                            matrizGastosSub[i, j] = acumuladorGto;
                        }
                    }
                    for (int i = 0; i < idsSubsRentadosD.Count; i++)
                    {
                        for (int j = 0; j < gastosNombres.Count; j++)
                        {
                            decimal acumuladorGto = 0;
                            DataTable dtGastosPorSub = inmobiliaria.getDtGastosDeInmueblesPorSubConj(idsSubsRentadosP[i], gastosNombres[j]);
                            if (dtGastosPorSub.Rows.Count > 0)
                            {
                                foreach (DataRow drGto in dtGastosPorSub.Rows)
                                {
                                    if (!string.IsNullOrEmpty(drGto["CAMPO_NUM4"].ToString()))
                                    {
                                        if(drGto["CAMPO_NUM4"].ToString() == "D")
                                            acumuladorGto += Decimal.Round(Convert.ToDecimal(drGto["CAMPO_NUM4"].ToString()) * TipoDeCambio, 2);
                                        else
                                            acumuladorGto += Decimal.Round(Convert.ToDecimal(drGto["CAMPO_NUM4"].ToString()), 2);
                                    }
                                }
                            }
                            matrizGastosSubD[i, j] = acumuladorGto;
                        }
                    }
                }
                return true;
            }
            catch//(Exception ex)
            {
                error = "- Error al establecer los datos del reporte";
                return false;
            }            
        }

        public static string getTipoDeCambio(DateTime vigenciaSeleccionada)
        {
            Inmobiliaria inmobiliaria = new Inmobiliaria();
            int mesACalcular = vigenciaSeleccionada.Month;
            int mesSig = vigenciaSeleccionada.Month + 1;
            if (mesSig == 13)
                mesSig = 1;
            int anio = vigenciaSeleccionada.Year;
            DateTime ini = Convert.ToDateTime("01/" + mesACalcular + "/" + anio);
            DateTime fin = Convert.ToDateTime("01/" + mesSig + "/" + anio).AddDays(-1);
            DataTable dtTiposCambio = inmobiliaria.getDtTiposCambio(ini, fin);
            decimal promedio = 0;
            if (dtTiposCambio.Rows.Count > 0)
            {
                foreach (DataRow row in dtTiposCambio.Rows)
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
    }
}
