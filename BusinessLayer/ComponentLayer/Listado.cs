using System;
using System.Collections.Generic;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class Listado : SaariReport, IReport, IBackgroundReport
    {
        public string GrupoEmpresarial { get; set; }
        public string Estatus { get; set; }
        public DateTime Vigencia { get; set; }
        public decimal TipoDeCambio { get; set; }
        public string Usuario { get; set; }

        private string error = string.Empty;
        private string nombreGpoEmp = string.Empty;

        private string[] inmobiliariasPesos;
        private string[] subconjuntosPesos;
        private string[] municipiosPesos;
        private string[] estadosPesos;
        private string[] domiciliosPesos;
        private string[] identificadoresPesos;
        private string[] expCatastralesPesos;
        private string[] conjuntos;
        private decimal[] catastralesPesos;
        private decimal[] terrPesos;
        private decimal[] constPesos;
        private string[] arrendatariosPesos;
        private decimal[] rentasMensPesos;

        private string[] inmobiliariasDls;
        private string[] subconjuntosDls;
        private string[] municipiosDls;
        private string[] estadosDls;
        private string[] domiciliosDls;
        private string[] identificadoresDls;
        private string[] expCatastralesDls;
        private string[] conjuntosDls;
        private decimal[] catastralesDls;
        private decimal[] terrDls;
        private decimal[] constDls;
        private string[] arrendatariosDls;
        private decimal[] rentasMensDls;

        private string[] inmobiliariasNoRent;
        private string[] subconjuntosNoRent;
        private string[] municipiosNoRent;
        private string[] estadosNoRent;
        private string[] domiciliosNoRent;
        private string[] identificadoresNoRent;
        private string[] expCatastralesNoRent;
        private string[] conjuntosNoRent;
        private decimal[] catastralesNoRent;
        private decimal[] terrNoRent;
        private decimal[] constNoRent;
        private string[] arrendatariosNoRent;
        private decimal[] rentasMensNoRent;

        private string filename = string.Empty;
        private bool hayRentados = false;
        private bool hayNoRentados = false;

        public Listado()
        {

        }

        public Listado(string idGrupo, string estatus, DateTime fechaVigencia, decimal tipoCambio, string usuario)
        {
            GrupoEmpresarial = idGrupo;
            Estatus = estatus;
            Vigencia = fechaVigencia;
            TipoDeCambio = tipoCambio;
            Usuario = usuario;
        }

        public string generar()
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
                    Excel.Range rango = hojaExcel.get_Range("A1:P1");
                    rango.Merge();
                    rango.Value2 = "Listados de Rentados y No Rentados del Grupo Empresarial: " + nombreGpoEmp + " al: " + Vigencia.ToString("dd/MMMM/yyyy");
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 18;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //subencabezados
                    rango = hojaExcel.get_Range("A2:P2");
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    hojaExcel.get_Range("A2").Value2 = "Propietario";
                    hojaExcel.get_Range("B2").Value2 = "Descripción";
                    hojaExcel.get_Range("C2").Value2 = "Municipio";
                    hojaExcel.get_Range("D2").Value2 = "Estado";
                    hojaExcel.get_Range("E2").Value2 = "Dirección";
                    hojaExcel.get_Range("F2").Value2 = "Reg.";
                    hojaExcel.get_Range("G2").Value2 = "Exp. Catastral";
                    hojaExcel.get_Range("H2").Value2 = "Concepto";
                    hojaExcel.get_Range("I2").Value2 = "Val. Catastral";
                    hojaExcel.get_Range("J2").Value2 = "Terreno M2";
                    hojaExcel.get_Range("K2").Value2 = "Construcción M2";
                    hojaExcel.get_Range("L2").Value2 = "Arrendatario";
                    hojaExcel.get_Range("M2").Value2 = "Renta mensual";
                    hojaExcel.get_Range("N2").Value2 = "Renta anual";
                    hojaExcel.get_Range("O2").Value2 = "Productividad";
                    hojaExcel.get_Range("P2").Value2 = "Observaciones";                    

                    //variablesDeControl
                    int posicionRango = 4;
                    int posicionTotalPesos = 2;

                    OnCambioProgreso(85);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //PESOS
                    if (inmobiliariasPesos != null)
                    {
                        //ARRENDAMIENTOENPESOS
                        rango = hojaExcel.get_Range("A3:P3");
                        rango.Merge();
                        rango.Value2 = "ARRENDAMIENTO EN PESOS";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        
                        posicionTotalPesos = posicionRango + inmobiliariasPesos.Length;

                        //ColumnaA
                        foreach (string inmob in inmobiliariasPesos)
                        {
                            if (!string.IsNullOrEmpty(inmob))
                            {
                                rango = hojaExcel.get_Range("A" + posicionRango);
                                rango.Value2 = inmob;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }

                        //COLUMNA B
                        posicionRango = 4;
                        if (subconjuntosPesos != null)
                        {
                            foreach (string sub in subconjuntosPesos)
                            {
                                if (!string.IsNullOrEmpty(sub))
                                {
                                    rango = hojaExcel.get_Range("B" + posicionRango);
                                    rango.Value2 = sub;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        //COLUMNA C
                        posicionRango = 4;
                        if (municipiosPesos != null)
                        {
                            foreach (string muni in municipiosPesos)
                            {
                                if (!string.IsNullOrEmpty(muni))
                                {
                                    rango = hojaExcel.get_Range("C" + posicionRango);
                                    rango.Value2 = muni;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        //COLUMNA D
                        posicionRango = 4;
                        if (estadosPesos != null)
                        {
                            foreach (string estad in estadosPesos)
                            {
                                if (!string.IsNullOrEmpty(estad))
                                {
                                    rango = hojaExcel.get_Range("D" + posicionRango);
                                    rango.Value2 = estad;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        //COLUMNA E
                        posicionRango = 4;
                        if (domiciliosPesos != null)
                        {
                            foreach (string domi in domiciliosPesos)
                            {
                                if (!string.IsNullOrEmpty(domi))
                                {
                                    rango = hojaExcel.get_Range("E" + posicionRango);
                                    rango.Value2 = domi;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        //COLUMNA F
                        posicionRango = 4;
                        if (identificadoresPesos != null)
                        {
                            foreach (string ident in identificadoresPesos)
                            {
                                if (!string.IsNullOrEmpty(ident))
                                {
                                    rango = hojaExcel.get_Range("F" + posicionRango);
                                    rango.Value2 = ident;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        //COLUMNA G
                        posicionRango = 4;
                        if (expCatastralesPesos != null)
                        {
                            foreach (string exp in expCatastralesPesos)
                            {
                                if (!string.IsNullOrEmpty(exp))
                                {
                                    rango = hojaExcel.get_Range("G" + posicionRango);
                                    rango.Value2 = exp;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        //COLUMNA H
                        posicionRango = 4;
                        if (conjuntos != null)
                        {
                            foreach (string cnj in conjuntos)
                            {
                                if (!string.IsNullOrEmpty(cnj))
                                {
                                    rango = hojaExcel.get_Range("H" + posicionRango);
                                    rango.Value2 = cnj;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }

                        //COLUMNA I
                        posicionRango = 4;
                        if (catastralesPesos != null)
                        {
                            foreach (decimal catP in catastralesPesos)
                            {
                                    rango = hojaExcel.get_Range("I" + posicionRango);
                                    rango.Value2 = catP;
                                    rango.NumberFormat = "$###,###,###,###,000.00";
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                            }
                        }

                        //COLUMNA J
                        posicionRango = 4;
                        if (terrPesos != null)
                        {
                            foreach (decimal terr in terrPesos)
                            {
                                rango = hojaExcel.get_Range("J" + posicionRango);
                                rango.Value2 = terr;
                                rango.NumberFormat = "###,000.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }

                        //COLUMNA K
                        posicionRango = 4;
                        if (constPesos != null)
                        {
                            foreach (decimal con in constPesos)
                            {
                                rango = hojaExcel.get_Range("K" + posicionRango);
                                rango.Value2 = con;
                                rango.NumberFormat = "###,000.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }

                        //COLUMNA L
                        posicionRango = 4;
                        if (arrendatariosPesos != null)
                        {
                            foreach (string arrendat in arrendatariosPesos)
                            {
                                rango = hojaExcel.get_Range("L" + posicionRango);
                                rango.Value2 = arrendat;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }

                        //COLUMNA M,N,O y P
                        posicionRango = 4;
                        if (rentasMensPesos != null)
                        {
                            foreach (decimal rentMen in rentasMensPesos)
                            {
                                rango = hojaExcel.get_Range("M" + posicionRango);
                                rango.Value2 = rentMen;
                                rango.NumberFormat = "$###,###,###,###,000.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("N" + posicionRango);
                                rango.Value2 = Decimal.Round(rentMen * 12, 2);
                                rango.NumberFormat = "$###,###,###,###,000.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("O" + posicionRango);
                                rango.Formula = "=(M" + posicionRango + "/I" + posicionRango + ")";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.NumberFormat = "0%";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("P" + posicionRango);
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                posicionRango++;
                            }
                        }

                        //TOTAL
                        rango = hojaExcel.get_Range("A"+posicionTotalPesos+":P"+posicionTotalPesos);
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("A" + posicionTotalPesos);
                        rango.Value2 = "TOTAL";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;

                        rango = hojaExcel.get_Range("I" + posicionTotalPesos);
                        rango.Formula = "=SUMA(I4:I" + (posicionTotalPesos - 1);

                        rango = hojaExcel.get_Range("J" + posicionTotalPesos);
                        rango.Formula = "=SUMA(J4:J" + (posicionTotalPesos - 1);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;

                        rango = hojaExcel.get_Range("K" + posicionTotalPesos);
                        rango.Formula = "=SUMA(K4:K" + (posicionTotalPesos - 1);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;

                        rango = hojaExcel.get_Range("M" + posicionTotalPesos);
                        rango.Formula = "=SUMA(M4:M" + (posicionTotalPesos - 1);

                        rango = hojaExcel.get_Range("N" + posicionTotalPesos);
                        rango.Formula = "=SUMA(N4:N" + (posicionTotalPesos - 1);

                        rango = hojaExcel.get_Range("O" + posicionTotalPesos);
                        rango.Formula = "=(M" + posicionTotalPesos + "/I" + posicionTotalPesos + ")";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.NumberFormat = "0%";

                    }

                    
                    //variablesDeControl
                     posicionRango = posicionTotalPesos + 2;
                    int posicionTotalDolares = posicionRango;

                    OnCambioProgreso(90);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //DOLARES
                    if (inmobiliariasDls != null)
                    {
                        //ARRENDAMIENTOENDOLARES
                        rango = hojaExcel.get_Range("A"+ (posicionTotalPesos + 1) +":P"+ (posicionTotalPesos + 1));
                        rango.Merge();
                        rango.Value2 = "ARRENDAMIENTO EN DOLARES";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        
                        posicionTotalDolares = posicionRango + inmobiliariasDls.Length;
                        
                        //ColumnaA
                        foreach (string inmob in inmobiliariasDls)
                        {
                            if (!string.IsNullOrEmpty(inmob))
                            {
                                rango = hojaExcel.get_Range("A" + posicionRango);
                                rango.Value2 = inmob;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        //COLUMNA B
                        posicionRango = posicionTotalPesos + 2;
                        if (subconjuntosDls != null)
                        {
                            foreach (string sub in subconjuntosDls)
                            {
                                if (!string.IsNullOrEmpty(sub))
                                {
                                    rango = hojaExcel.get_Range("B" + posicionRango);
                                    rango.Value2 = sub;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA C
                        posicionRango = posicionTotalPesos + 2;
                        if (municipiosDls != null)
                        {
                            foreach (string muni in municipiosDls)
                            {
                                if (!string.IsNullOrEmpty(muni))
                                {
                                    rango = hojaExcel.get_Range("C" + posicionRango);
                                    rango.Value2 = muni;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA D
                        posicionRango = posicionTotalPesos + 2;
                        if (estadosDls != null)
                        {
                            foreach (string estad in estadosDls)
                            {
                                if (!string.IsNullOrEmpty(estad))
                                {
                                    rango = hojaExcel.get_Range("D" + posicionRango);
                                    rango.Value2 = estad;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA E
                        posicionRango = posicionTotalPesos + 2;
                        if (domiciliosDls != null)
                        {
                            foreach (string domi in domiciliosDls)
                            {
                                if (!string.IsNullOrEmpty(domi))
                                {
                                    rango = hojaExcel.get_Range("E" + posicionRango);
                                    rango.Value2 = domi;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA F
                        posicionRango = posicionTotalPesos + 2;
                        if (identificadoresDls != null)
                        {
                            foreach (string ident in identificadoresDls)
                            {
                                if (!string.IsNullOrEmpty(ident))
                                {
                                    rango = hojaExcel.get_Range("F" + posicionRango);
                                    rango.Value2 = ident;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA G
                        posicionRango = posicionTotalPesos + 2;
                        if (expCatastralesDls != null)
                        {
                            foreach (string exp in expCatastralesDls)
                            {
                                if (!string.IsNullOrEmpty(exp))
                                {
                                    rango = hojaExcel.get_Range("G" + posicionRango);
                                    rango.Value2 = exp;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA H
                        posicionRango = posicionTotalPesos + 2;
                        if (conjuntosDls != null)
                        {
                            foreach (string cnj in conjuntosDls)
                            {
                                if (!string.IsNullOrEmpty(cnj))
                                {
                                    rango = hojaExcel.get_Range("H" + posicionRango);
                                    rango.Value2 = cnj;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA I
                        posicionRango = posicionTotalPesos + 2;
                        if (catastralesDls != null)
                        {
                            foreach (decimal catP in catastralesDls)
                            {
                                    rango = hojaExcel.get_Range("I" + posicionRango);
                                    rango.Value2 = catP;
                                    rango.NumberFormat = "$###,###,###,###,000.00";
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                            }
                        }
                        
                        //COLUMNA J
                        posicionRango = posicionTotalPesos + 2;
                        if (terrDls != null)
                        {
                            foreach (decimal terr in terrDls)
                            {
                                rango = hojaExcel.get_Range("J" + posicionRango);
                                rango.Value2 = terr;
                                rango.NumberFormat = "###,000.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        //COLUMNA K
                        posicionRango = posicionTotalPesos + 2;
                        if (constDls != null)
                        {
                            foreach (decimal con in constDls)
                            {
                                rango = hojaExcel.get_Range("K" + posicionRango);
                                rango.Value2 = con;
                                rango.NumberFormat = "###,000.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        //COLUMNA L
                        posicionRango = posicionTotalPesos + 2;
                        if (arrendatariosDls != null)
                        {
                            foreach (string arrendat in arrendatariosDls)
                            {
                                rango = hojaExcel.get_Range("L" + posicionRango);
                                rango.Value2 = arrendat;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        //COLUMNA M,N,O y P
                        posicionRango = posicionTotalPesos + 2;
                        if (rentasMensDls != null)
                        {
                            foreach (decimal rentMen in rentasMensDls)
                            {
                                rango = hojaExcel.get_Range("M" + posicionRango);
                                rango.Value2 = rentMen;
                                rango.NumberFormat = "$###,###,###,###,000.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("N" + posicionRango);
                                rango.Value2 = Decimal.Round(rentMen * 12, 2);
                                rango.NumberFormat = "$###,###,###,###,000.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("O" + posicionRango);
                                rango.Formula = "=(M" + posicionRango + "/I" + posicionRango + ")";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.NumberFormat = "0%";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("P" + posicionRango);
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                posicionRango++;
                            }
                        }

                        posicionRango = posicionTotalPesos + 2;
                        //TOTAL
                        rango = hojaExcel.get_Range("A"+posicionTotalDolares+":P"+posicionTotalDolares);
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        rango = hojaExcel.get_Range("A" + posicionTotalDolares);
                        rango.Value2 = "TOTAL";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;

                        rango = hojaExcel.get_Range("I" + posicionTotalDolares);
                        rango.Formula = "=SUMA(I"+posicionRango+":I" + (posicionTotalDolares - 1);

                        rango = hojaExcel.get_Range("J" + posicionTotalDolares);
                        rango.Formula = "=SUMA(J" + posicionRango + ":J" + (posicionTotalDolares - 1);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;

                        rango = hojaExcel.get_Range("K" + posicionTotalDolares);
                        rango.Formula = "=SUMA(K" + posicionRango + ":K" + (posicionTotalDolares - 1);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;

                        rango = hojaExcel.get_Range("M" + posicionTotalDolares);
                        rango.Formula = "=SUMA(M" + posicionRango + ":M" + (posicionTotalDolares - 1);

                        rango = hojaExcel.get_Range("N" + posicionTotalDolares);
                        rango.Formula = "=SUMA(N" + posicionRango + ":N" + (posicionTotalDolares - 1);

                        rango = hojaExcel.get_Range("O" + posicionTotalDolares);
                        rango.Formula = "=(M" + posicionTotalDolares + "/I" + posicionTotalDolares + ")";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.NumberFormat = "0%";
                    }

                    OnCambioProgreso(95);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //variablesDeControl
                     posicionRango = posicionTotalDolares + 2;
                    int posicionTotalNoRentados = posicionRango;
                    
                    //DOLARES
                    if (inmobiliariasNoRent != null)
                    {
                        //NORENTADOS
                        rango = hojaExcel.get_Range("A"+ (posicionTotalDolares + 1) +":P"+ (posicionTotalDolares + 1));
                        rango.Merge();
                        rango.Value2 = "NO RENTADOS";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        
                        posicionTotalNoRentados = posicionRango + inmobiliariasNoRent.Length;
                        
                        //ColumnaA
                        foreach (string inmob in inmobiliariasNoRent)
                        {
                            if (!string.IsNullOrEmpty(inmob))
                            {
                                rango = hojaExcel.get_Range("A" + posicionRango);
                                rango.Value2 = inmob;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        //COLUMNA B
                        posicionRango = posicionTotalDolares + 2;
                        if (subconjuntosNoRent != null)
                        {
                            foreach (string sub in subconjuntosNoRent)
                            {
                                if (!string.IsNullOrEmpty(sub))
                                {
                                    rango = hojaExcel.get_Range("B" + posicionRango);
                                    rango.Value2 = sub;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA C
                        posicionRango = posicionTotalDolares + 2;
                        if (municipiosNoRent != null)
                        {
                            foreach (string muni in municipiosNoRent)
                            {
                                if (!string.IsNullOrEmpty(muni))
                                {
                                    rango = hojaExcel.get_Range("C" + posicionRango);
                                    rango.Value2 = muni;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA D
                        posicionRango = posicionTotalDolares + 2;
                        if (estadosNoRent != null)
                        {
                            foreach (string estad in estadosNoRent)
                            {
                                if (!string.IsNullOrEmpty(estad))
                                {
                                    rango = hojaExcel.get_Range("D" + posicionRango);
                                    rango.Value2 = estad;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA E
                        posicionRango = posicionTotalDolares + 2;
                        if (domiciliosNoRent != null)
                        {
                            foreach (string domi in domiciliosNoRent)
                            {
                                if (!string.IsNullOrEmpty(domi))
                                {
                                    rango = hojaExcel.get_Range("E" + posicionRango);
                                    rango.Value2 = domi;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA F
                        posicionRango = posicionTotalDolares + 2;
                        if (identificadoresNoRent != null)
                        {
                            foreach (string ident in identificadoresNoRent)
                            {
                                if (!string.IsNullOrEmpty(ident))
                                {
                                    rango = hojaExcel.get_Range("F" + posicionRango);
                                    rango.Value2 = ident;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA G
                        posicionRango = posicionTotalDolares + 2;
                        if (expCatastralesNoRent != null)
                        {
                            foreach (string exp in expCatastralesNoRent)
                            {
                                if (!string.IsNullOrEmpty(exp))
                                {
                                    rango = hojaExcel.get_Range("G" + posicionRango);
                                    rango.Value2 = exp;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA H
                        posicionRango = posicionTotalDolares + 2;
                        if (conjuntosNoRent != null)
                        {
                            foreach (string cnj in conjuntosNoRent)
                            {
                                if (!string.IsNullOrEmpty(cnj))
                                {
                                    rango = hojaExcel.get_Range("H" + posicionRango);
                                    rango.Value2 = cnj;
                                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                                }
                            }
                        }
                        
                        //COLUMNA I
                        posicionRango = posicionTotalDolares + 2;
                        if (catastralesNoRent != null)
                        {
                            foreach (decimal catP in catastralesNoRent)
                            {
                                    rango = hojaExcel.get_Range("I" + posicionRango);
                                    rango.Value2 = catP;
                                    rango.NumberFormat = "$###,###,###,###,000.00";
                                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    posicionRango++;
                            }
                        }
                        
                        //COLUMNA J
                        posicionRango = posicionTotalDolares + 2;
                        if (terrNoRent != null)
                        {
                            foreach (decimal terr in terrNoRent)
                            {
                                rango = hojaExcel.get_Range("J" + posicionRango);
                                rango.Value2 = terr;
                                rango.NumberFormat = "###,000.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        //COLUMNA K
                        posicionRango = posicionTotalDolares + 2;
                        if (constNoRent != null)
                        {
                            foreach (decimal con in constNoRent)
                            {
                                rango = hojaExcel.get_Range("K" + posicionRango);
                                rango.Value2 = con;
                                rango.NumberFormat = "###,000.00";
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        //COLUMNA L
                        posicionRango = posicionTotalDolares + 2;
                        if (arrendatariosNoRent != null)
                        {
                            foreach (string arrendat in arrendatariosNoRent)
                            {
                                rango = hojaExcel.get_Range("L" + posicionRango);
                                rango.Value2 = arrendat;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                        
                        //COLUMNA M,N,O y P
                        posicionRango = posicionTotalDolares + 2;
                        if (rentasMensNoRent != null)
                        {
                            foreach (decimal rentMen in rentasMensNoRent)
                            {
                                rango = hojaExcel.get_Range("M" + posicionRango);
                                rango.Value2 = rentMen;
                                rango.NumberFormat = "$###,###,###,###,000.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("N" + posicionRango);
                                rango.Value2 = Decimal.Round(rentMen * 12, 2);
                                rango.NumberFormat = "$###,###,###,###,000.00";
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("O" + posicionRango);
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                rango = hojaExcel.get_Range("P" + posicionRango);
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                                posicionRango++;
                            }
                        }
                        
                        posicionRango = posicionTotalDolares + 2;
                        //TOTAL
                        rango = hojaExcel.get_Range("A"+posicionTotalNoRentados+":P"+posicionTotalNoRentados);
                        rango.Font.Bold = true;
                        rango.Font.Size = 11;
                        rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                        rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        
                        rango = hojaExcel.get_Range("A" + posicionTotalNoRentados);
                        rango.Value2 = "TOTAL";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        
                        rango = hojaExcel.get_Range("I" + posicionTotalNoRentados);
                        rango.Formula = "=SUMA(I"+posicionRango+":I" + (posicionTotalNoRentados - 1);

                        rango = hojaExcel.get_Range("J" + posicionTotalNoRentados);
                        rango.Formula = "=SUMA(J" + posicionRango + ":J" + (posicionTotalNoRentados - 1);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;

                        rango = hojaExcel.get_Range("K" + posicionTotalNoRentados);
                        rango.Formula = "=SUMA(K" + posicionRango + ":K" + (posicionTotalNoRentados - 1);
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;

                        rango = hojaExcel.get_Range("M" + posicionTotalNoRentados);
                        rango.Formula = "=SUMA(M" + posicionRango + ":M" + (posicionTotalNoRentados - 1);

                        rango = hojaExcel.get_Range("N" + posicionTotalNoRentados);
                        rango.Formula = "=SUMA(N" + posicionRango + ":N" + (posicionTotalNoRentados - 1);
                    }
                    //Tipo de cambio
                    int posicionTotal = 0;
                    if (posicionTotalNoRentados > 2)
                        posicionTotal = posicionTotalNoRentados + 1;
                    else if (posicionTotalDolares > 2)
                        posicionTotal = posicionTotalDolares + 1;
                    else if (posicionTotalPesos > 2)
                        posicionTotal = posicionTotalPesos + 1;
                    else
                        posicionTotal = 3;

                    rango = hojaExcel.get_Range("A" + posicionTotal);
                    rango.Value2 = "Tipo de cambio: " + TipoDeCambio;
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    
                    //Generado por
                    rango = hojaExcel.get_Range("A" + (posicionTotal + 1));
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
                   // hojaExcel.get_Range("G2:G1000").Columns.AutoFit();
                    hojaExcel.get_Range("H2:G1000").Columns.AutoFit();
                    hojaExcel.get_Range("I2:I1000").Columns.AutoFit();
                    hojaExcel.get_Range("J2:J1000").Columns.AutoFit();
                    hojaExcel.get_Range("K2:K1000").Columns.AutoFit();
                    hojaExcel.get_Range("L2:L1000").Columns.AutoFit();
                    hojaExcel.get_Range("M2:M1000").Columns.AutoFit();
                    hojaExcel.get_Range("N2:N1000").Columns.AutoFit();
                    hojaExcel.get_Range("O2:O1000").Columns.AutoFit();
                    hojaExcel.get_Range("Q2:Q1000").Columns.AutoFit();
                    hojaExcel.get_Range("G2:G500").Columns.ColumnWidth = 20;

                    //MostrarReporte
                    filename = GestorReportes.Properties.Settings.Default.RutaRepositorio + @"ListadoRentadosYNoRentados_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + ".xlsx";
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

        private bool getDatos()
        {
            //si regresamos false hay que llenar el error
            try
            {
                Inmobiliaria inmobiliaria = new Inmobiliaria();
                if (GrupoEmpresarial == "Todos")
                    nombreGpoEmp = "Todos";
                else
                {
                    DataTable dtGpo = inmobiliaria.getDtGrupoEmpresarial(GrupoEmpresarial);
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

                if (Estatus == "Todos" || Estatus == "Rentados")
                {
                    DataTable dtRentados = inmobiliaria.getDtSubRentados(GrupoEmpresarial, Vigencia);
                    if (dtRentados.Rows.Count > 0)
                    {
                        OnCambioProgreso(30);
                        if (CancelacionPendiente)
                        {
                            error = "Proceso cancelado por el usuario";
                            return false;
                        }
                        hayRentados = true;
                        DataRow[] rowsRentadosPesos = dtRentados.Select("P0407_MONEDA_FACT = 'P'");
                        if (rowsRentadosPesos.Length > 0)
                        {
                            inmobiliariasPesos = new string[rowsRentadosPesos.Length];
                            subconjuntosPesos = new string[rowsRentadosPesos.Length];
                            municipiosPesos = new string[rowsRentadosPesos.Length];
                            estadosPesos = new string[rowsRentadosPesos.Length];
                            domiciliosPesos = new string[rowsRentadosPesos.Length];
                            identificadoresPesos = new string[rowsRentadosPesos.Length];
                            expCatastralesPesos = new string[rowsRentadosPesos.Length];
                            conjuntos = new string[rowsRentadosPesos.Length];
                            catastralesPesos = new decimal[rowsRentadosPesos.Length];
                            terrPesos = new decimal[rowsRentadosPesos.Length];
                            constPesos = new decimal[rowsRentadosPesos.Length];
                            arrendatariosPesos = new string[rowsRentadosPesos.Length];
                            rentasMensPesos = new decimal[rowsRentadosPesos.Length];

                            int porcentaje = 30;
                            decimal factor = 10 / rowsRentadosPesos.Length;
                            factor = factor >= 1 ? factor : 1;

                            int contador = 0;
                            foreach (DataRow row in rowsRentadosPesos)
                            {
                                if (porcentaje <= 40)
                                    porcentaje += Convert.ToInt32(factor);
                                OnCambioProgreso(porcentaje <= 40 ? porcentaje : 40);
                                if (CancelacionPendiente)
                                {
                                    error = "Proceso cancelado por el usuario";
                                    return false;
                                }

                                DataTable dtInmubPorSub = inmobiliaria.getDtInmueblesPorSubConj(row["P1801_ID_SUBCONJUNTO"].ToString());

                                if (string.IsNullOrEmpty(row["P0103_RAZON_SOCIAL"].ToString()))
                                    inmobiliariasPesos[contador] = "-";
                                else
                                    inmobiliariasPesos[contador] = row["P0103_RAZON_SOCIAL"].ToString();


                                if (string.IsNullOrEmpty(row["P1803_NOMBRE"].ToString()))
                                    subconjuntosPesos[contador] = "-";
                                else
                                    subconjuntosPesos[contador] = row["P1803_NOMBRE"].ToString();


                                if (string.IsNullOrEmpty(row["P0506_CIUDAD"].ToString()))
                                {
                                    if (dtInmubPorSub.Rows.Count > 0)
                                    {
                                        if (string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0506_CIUDAD"].ToString()))
                                        {
                                            municipiosPesos[contador] = "-";
                                        }
                                        else
                                        {
                                            municipiosPesos[contador] = dtInmubPorSub.Rows[0]["P0506_CIUDAD"].ToString();
                                        }
                                    }
                                    else
                                        municipiosPesos[contador] = "-";
                                }
                                else
                                    municipiosPesos[contador] = row["P0506_CIUDAD"].ToString();


                                if (string.IsNullOrEmpty(row["P0507_ESTADO"].ToString()))
                                {
                                    if (dtInmubPorSub.Rows.Count > 0)
                                    {
                                        if (string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0507_ESTADO"].ToString()))
                                        {
                                            estadosPesos[contador] = "-";
                                        }
                                        else
                                        {
                                            estadosPesos[contador] = dtInmubPorSub.Rows[0]["P0507_ESTADO"].ToString();
                                        }
                                    }
                                    else
                                        estadosPesos[contador] = "-";
                                }
                                else
                                    estadosPesos[contador] = row["P0507_ESTADO"].ToString();


                                if (string.IsNullOrEmpty(row["P0503_CALLE_NUM"].ToString()) && string.IsNullOrEmpty(row["P0504_COLONIA"].ToString()))
                                {
                                    if (dtInmubPorSub.Rows.Count > 0)
                                    {
                                        if (string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0503_CALLE_NUM"].ToString()) && string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0504_COLONIA"].ToString()))
                                        {
                                            domiciliosPesos[contador] = "-";
                                        }
                                        else
                                        {
                                            domiciliosPesos[contador] = dtInmubPorSub.Rows[0]["P0503_CALLE_NUM"].ToString() + ", " + dtInmubPorSub.Rows[0]["P0504_COLONIA"].ToString();
                                        }
                                    }
                                    else
                                        domiciliosPesos[contador] = "-";
                                }
                                else
                                    domiciliosPesos[contador] = row["P0503_CALLE_NUM"].ToString() + ", " + row["P0504_COLONIA"].ToString();


                                if (string.IsNullOrEmpty(row["P18_CAMPO1"].ToString()))
                                    identificadoresPesos[contador] = "-";
                                else
                                    identificadoresPesos[contador] = row["P18_CAMPO1"].ToString();
                                

                                string expCat = string.Empty;
                                decimal valCat = 0;
                                decimal terrM2 = 0;
                                decimal constM2 = 0;
                                foreach (DataRow dr in dtInmubPorSub.Rows)
                                {
                                    if(!string.IsNullOrEmpty(dr["P0708_PREDIAL"].ToString()))
                                        expCat += dr["P0708_PREDIAL"].ToString() + "| ";
                                    if(!string.IsNullOrEmpty(dr["P1922_A_MIN_ING"].ToString()))
                                        valCat += Decimal.Round(Convert.ToDecimal(dr["P1922_A_MIN_ING"].ToString()), 2);
                                    if (!string.IsNullOrEmpty(dr["P1926_CIST_ING"].ToString()))
                                        terrM2 += Decimal.Round(Convert.ToDecimal(dr["P1926_CIST_ING"].ToString()), 2);
                                    if (!string.IsNullOrEmpty(dr["CAMPO_NUM1"].ToString()))
                                        constM2 += Decimal.Round(Convert.ToDecimal(dr["CAMPO_NUM1"].ToString()), 2);
                                }
                                if (string.IsNullOrEmpty(expCat))
                                    expCat = "-";
                                expCatastralesPesos[contador] = expCat;
                                catastralesPesos[contador] = valCat;
                                terrPesos[contador] = terrM2;
                                constPesos[contador] = constM2;
                                
                                if (string.IsNullOrEmpty(row["P0303_NOMBRE"].ToString()))
                                    conjuntos[contador] = "-";
                                else
                                    conjuntos[contador] = row["P0303_NOMBRE"].ToString();


                                if (string.IsNullOrEmpty(row["P0203_NOMBRE"].ToString()))
                                    arrendatariosPesos[contador] = "-";
                                else
                                    arrendatariosPesos[contador] = row["P0203_NOMBRE"].ToString();

                                
                                if (string.IsNullOrEmpty(row["P0434_IMPORTE_ACTUAL"].ToString()))
                                    rentasMensPesos[contador] = 0;
                                else
                                    rentasMensPesos[contador] = Decimal.Round(Convert.ToDecimal(row["P0434_IMPORTE_ACTUAL"].ToString()), 2);

                                contador++;
                            }
                        }
                        DataRow[] rowsRentadosDolares = dtRentados.Select("P0407_MONEDA_FACT = 'D'");
                        if (rowsRentadosDolares.Length > 0)
                        {
                            inmobiliariasDls = new string[rowsRentadosDolares.Length];
                            subconjuntosDls = new string[rowsRentadosDolares.Length];
                            municipiosDls = new string[rowsRentadosDolares.Length];
                            estadosDls = new string[rowsRentadosDolares.Length];
                            domiciliosDls = new string[rowsRentadosDolares.Length];
                            identificadoresDls = new string[rowsRentadosDolares.Length];
                            expCatastralesDls = new string[rowsRentadosDolares.Length];
                            conjuntosDls = new string[rowsRentadosDolares.Length];
                            catastralesDls = new decimal[rowsRentadosDolares.Length];
                            terrDls = new decimal[rowsRentadosDolares.Length];
                            constDls = new decimal[rowsRentadosDolares.Length];
                            arrendatariosDls = new string[rowsRentadosDolares.Length];
                            rentasMensDls = new decimal[rowsRentadosDolares.Length];

                            int porcentaje = 40;
                            decimal factor = 10 / rowsRentadosDolares.Length;
                            factor = factor >= 1 ? factor : 1;

                            int contador = 0;
                            foreach (DataRow row in rowsRentadosDolares)
                            {
                                if (porcentaje <= 50)
                                    porcentaje += Convert.ToInt32(factor);
                                OnCambioProgreso(porcentaje <= 50 ? porcentaje : 50);
                                if (CancelacionPendiente)
                                {
                                    error = "Proceso cancelado por el usuario";
                                    return false;
                                }

                                DataTable dtInmubPorSub = inmobiliaria.getDtInmueblesPorSubConj(row["P1801_ID_SUBCONJUNTO"].ToString());

                                if (string.IsNullOrEmpty(row["P0103_RAZON_SOCIAL"].ToString()))
                                    inmobiliariasDls[contador] = "-";
                                else
                                    inmobiliariasDls[contador] = row["P0103_RAZON_SOCIAL"].ToString();


                                if (string.IsNullOrEmpty(row["P1803_NOMBRE"].ToString()))
                                    subconjuntosDls[contador] = "-";
                                else
                                    subconjuntosDls[contador] = row["P1803_NOMBRE"].ToString();


                                if (string.IsNullOrEmpty(row["P0506_CIUDAD"].ToString()))
                                {
                                    if (dtInmubPorSub.Rows.Count > 0)
                                    {
                                        if (string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0506_CIUDAD"].ToString()))
                                        {
                                            municipiosDls[contador] = "-";
                                        }
                                        else
                                        {
                                            municipiosDls[contador] = dtInmubPorSub.Rows[0]["P0506_CIUDAD"].ToString();
                                        }
                                    }
                                    else
                                        municipiosDls[contador] = "-";
                                }
                                else
                                    municipiosDls[contador] = row["P0506_CIUDAD"].ToString();


                                if (string.IsNullOrEmpty(row["P0507_ESTADO"].ToString()))
                                {
                                    if (dtInmubPorSub.Rows.Count > 0)
                                    {
                                        if (string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0507_ESTADO"].ToString()))
                                        {
                                            estadosDls[contador] = "-";
                                        }
                                        else
                                        {
                                            estadosDls[contador] = dtInmubPorSub.Rows[0]["P0507_ESTADO"].ToString();
                                        }
                                    }
                                    else
                                        estadosDls[contador] = "-";
                                }
                                else
                                    estadosDls[contador] = row["P0507_ESTADO"].ToString();


                                if (string.IsNullOrEmpty(row["P0503_CALLE_NUM"].ToString()) && string.IsNullOrEmpty(row["P0504_COLONIA"].ToString()))
                                {
                                    if (dtInmubPorSub.Rows.Count > 0)
                                    {
                                        if (string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0503_CALLE_NUM"].ToString()) && string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0504_COLONIA"].ToString()))
                                        {
                                            domiciliosDls[contador] = "-";
                                        }
                                        else
                                        {
                                            domiciliosDls[contador] = dtInmubPorSub.Rows[0]["P0503_CALLE_NUM"].ToString() + ", " + dtInmubPorSub.Rows[0]["P0504_COLONIA"].ToString();
                                        }
                                    }
                                    else
                                        domiciliosDls[contador] = "-";
                                }
                                else
                                    domiciliosDls[contador] = row["P0503_CALLE_NUM"].ToString() + ", " + row["P0504_COLONIA"].ToString();


                                if (string.IsNullOrEmpty(row["P18_CAMPO1"].ToString()))
                                    identificadoresDls[contador] = "-";
                                else
                                    identificadoresDls[contador] = row["P18_CAMPO1"].ToString();


                                string expCat = string.Empty;
                                decimal valCat = 0;
                                decimal terrM2 = 0;
                                decimal constM2 = 0;
                                foreach (DataRow dr in dtInmubPorSub.Rows)
                                {
                                    if (!string.IsNullOrEmpty(dr["P0708_PREDIAL"].ToString()))
                                        expCat += dr["P0708_PREDIAL"].ToString() + "| ";
                                    if (!string.IsNullOrEmpty(dr["P1922_A_MIN_ING"].ToString()))
                                        //valCat += Decimal.Round((Convert.ToDecimal(dr["P1922_A_MIN_ING"].ToString()) * TipoDeCambio), 2);
                                        valCat += Decimal.Round(Convert.ToDecimal(dr["P1922_A_MIN_ING"].ToString()), 2);
                                    if (!string.IsNullOrEmpty(dr["P1926_CIST_ING"].ToString()))
                                        terrM2 += Decimal.Round(Convert.ToDecimal(dr["P1926_CIST_ING"].ToString()), 2);
                                    if (!string.IsNullOrEmpty(dr["CAMPO_NUM1"].ToString()))
                                        constM2 += Decimal.Round(Convert.ToDecimal(dr["CAMPO_NUM1"].ToString()), 2);
                                }
                                if (string.IsNullOrEmpty(expCat))
                                    expCat = "-";
                                expCatastralesDls[contador] = expCat;
                                catastralesDls[contador] = valCat;
                                terrDls[contador] = terrM2;
                                constDls[contador] = constM2;

                                if (string.IsNullOrEmpty(row["P0303_NOMBRE"].ToString()))
                                    conjuntosDls[contador] = "-";
                                else
                                    conjuntosDls[contador] = row["P0303_NOMBRE"].ToString();


                                if (string.IsNullOrEmpty(row["P0203_NOMBRE"].ToString()))
                                    arrendatariosDls[contador] = "-";
                                else
                                    arrendatariosDls[contador] = row["P0203_NOMBRE"].ToString();


                                if (string.IsNullOrEmpty(row["P0434_IMPORTE_ACTUAL"].ToString()))
                                    rentasMensDls[contador] = 0;
                                else
                                    rentasMensDls[contador] = Decimal.Round((Convert.ToDecimal(row["P0434_IMPORTE_ACTUAL"].ToString()) * TipoDeCambio), 2);

                                contador++;
                            }
                        }
                    }                    
                }
                if (Estatus == "Todos" || Estatus == "No Rentados")
                {
                    OnCambioProgreso(60);
                    if (CancelacionPendiente)
                    {
                        error = "Proceso cancelado por el usuario";
                        return false;
                    }

                    DataTable dtNoRentados = inmobiliaria.getDtSubConjuntosNoRentadosPorGpoEmp(GrupoEmpresarial);
                    if (dtNoRentados.Rows.Count > 0)
                    {
                        hayNoRentados = true;
                        inmobiliariasNoRent = new string[dtNoRentados.Rows.Count];
                        subconjuntosNoRent = new string[dtNoRentados.Rows.Count];
                        municipiosNoRent = new string[dtNoRentados.Rows.Count];
                        estadosNoRent = new string[dtNoRentados.Rows.Count];
                        domiciliosNoRent = new string[dtNoRentados.Rows.Count];
                        identificadoresNoRent = new string[dtNoRentados.Rows.Count];
                        expCatastralesNoRent = new string[dtNoRentados.Rows.Count];
                        conjuntosNoRent = new string[dtNoRentados.Rows.Count];
                        catastralesNoRent = new decimal[dtNoRentados.Rows.Count]; 
                        terrNoRent = new decimal[dtNoRentados.Rows.Count]; 
                        constNoRent = new decimal[dtNoRentados.Rows.Count]; 
                        arrendatariosNoRent = new string[dtNoRentados.Rows.Count];
                        rentasMensNoRent = new decimal[dtNoRentados.Rows.Count];
                        int contador = 0;

                        int porcentaje = 60;
                        decimal factor = 20 / dtNoRentados.Rows.Count;
                        factor = factor >= 1 ? factor : 1;

                        foreach (DataRow row in dtNoRentados.Rows)
                        {
                            if (porcentaje <= 80)
                                porcentaje += Convert.ToInt32(factor);
                            OnCambioProgreso(porcentaje <= 80 ? porcentaje : 80);
                            if (CancelacionPendiente)
                            {
                                error = "Proceso cancelado por el usuario";
                                return false;
                            }

                            DataTable dtInmubPorSub = inmobiliaria.getDtInmueblesPorSubConj(row["P1801_ID_SUBCONJUNTO"].ToString());

                            if (string.IsNullOrEmpty(row["P0103_RAZON_SOCIAL"].ToString()))
                                inmobiliariasNoRent[contador] = "-";
                            else
                                inmobiliariasNoRent[contador] = row["P0103_RAZON_SOCIAL"].ToString();


                            if (string.IsNullOrEmpty(row["P1803_NOMBRE"].ToString()))
                                subconjuntosNoRent[contador] = "-";
                            else
                                subconjuntosNoRent[contador] = row["P1803_NOMBRE"].ToString();


                            if (string.IsNullOrEmpty(row["P0506_CIUDAD"].ToString()))
                            {
                                if (dtInmubPorSub.Rows.Count > 0)
                                {
                                    if (string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0506_CIUDAD"].ToString()))
                                    {
                                        municipiosNoRent[contador] = "-";
                                    }
                                    else
                                    {
                                        municipiosNoRent[contador] = dtInmubPorSub.Rows[0]["P0506_CIUDAD"].ToString();
                                    }
                                }
                                else
                                    municipiosNoRent[contador] = "-";
                            }
                            else
                                municipiosNoRent[contador] = row["P0506_CIUDAD"].ToString();


                            if (string.IsNullOrEmpty(row["P0507_ESTADO"].ToString()))
                            {
                                if (dtInmubPorSub.Rows.Count > 0)
                                {
                                    if (string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0507_ESTADO"].ToString()))
                                    {
                                        estadosNoRent[contador] = "-";
                                    }
                                    else
                                    {
                                        estadosNoRent[contador] = dtInmubPorSub.Rows[0]["P0507_ESTADO"].ToString();
                                    }
                                }
                                else
                                    estadosNoRent[contador] = "-";
                            }
                            else
                                estadosNoRent[contador] = row["P0507_ESTADO"].ToString();


                            if (string.IsNullOrEmpty(row["P0503_CALLE_NUM"].ToString()) && string.IsNullOrEmpty(row["P0504_COLONIA"].ToString()))
                            {
                                if (dtInmubPorSub.Rows.Count > 0)
                                {
                                    if (string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0503_CALLE_NUM"].ToString()) && string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0504_COLONIA"].ToString()))
                                    {
                                        domiciliosNoRent[contador] = "-";
                                    }
                                    else
                                    {
                                        domiciliosNoRent[contador] = dtInmubPorSub.Rows[0]["P0503_CALLE_NUM"].ToString() + ", " + dtInmubPorSub.Rows[0]["P0504_COLONIA"].ToString();
                                    }
                                }
                                else
                                    domiciliosNoRent[contador] = "-";
                            }
                            else
                                domiciliosNoRent[contador] = row["P0503_CALLE_NUM"].ToString() + ", " + row["P0504_COLONIA"].ToString();


                            if (string.IsNullOrEmpty(row["P18_CAMPO1"].ToString()))
                                identificadoresNoRent[contador] = "-";
                            else
                                identificadoresNoRent[contador] = row["P18_CAMPO1"].ToString();


                            string expCat = string.Empty;
                            decimal valCat = 0;
                            decimal terrM2 = 0;
                            decimal constM2 = 0;
                            foreach (DataRow dr in dtInmubPorSub.Rows)
                            {
                                if (!string.IsNullOrEmpty(dr["P0708_PREDIAL"].ToString()))
                                    expCat += dr["P0708_PREDIAL"].ToString() + "| ";
                                if (!string.IsNullOrEmpty(dr["P1922_A_MIN_ING"].ToString()))
                                    //valCat += Decimal.Round((Convert.ToDecimal(dr["P1922_A_MIN_ING"].ToString()) * TipoDeCambio), 2);
                                    valCat += Decimal.Round(Convert.ToDecimal(dr["P1922_A_MIN_ING"].ToString()), 2);
                                if (!string.IsNullOrEmpty(dr["P1926_CIST_ING"].ToString()))
                                    terrM2 += Decimal.Round(Convert.ToDecimal(dr["P1926_CIST_ING"].ToString()), 2);
                                if (!string.IsNullOrEmpty(dr["CAMPO_NUM1"].ToString()))
                                    constM2 += Decimal.Round(Convert.ToDecimal(dr["CAMPO_NUM1"].ToString()), 2);
                            }
                            if (string.IsNullOrEmpty(expCat))
                                expCat = "-";
                            expCatastralesNoRent[contador] = expCat;
                            catastralesNoRent[contador] = valCat;
                            terrNoRent[contador] = terrM2;
                            constNoRent[contador] = constM2;


                            if (string.IsNullOrEmpty(row["P0303_NOMBRE"].ToString()))
                                conjuntosNoRent[contador] = "-";
                            else
                                conjuntosNoRent[contador] = row["P0303_NOMBRE"].ToString();

                                                        
                                arrendatariosNoRent[contador] = "-";


                           if (string.IsNullOrEmpty(row["P18CAMPO_NUM1"].ToString()))
                               rentasMensNoRent[contador] = 0;
                           else
                           {
                               if (!string.IsNullOrEmpty(row["P18_CAMPO2"].ToString()))
                               {
                                   if (row["P18_CAMPO2"].ToString() == "D")
                                       rentasMensNoRent[contador] = Decimal.Round((Convert.ToDecimal(row["P18CAMPO_NUM1"].ToString()) * TipoDeCambio), 2);
                                   else
                                   {
                                       rentasMensNoRent[contador] = Decimal.Round(Convert.ToDecimal(row["P18CAMPO_NUM1"].ToString()), 2);
                                   }
                               }
                               else
                                   rentasMensNoRent[contador] = Decimal.Round(Convert.ToDecimal(row["P18CAMPO_NUM1"].ToString()), 2);
                           }


                            contador++;
                        }
                    }
                }
                if(!hayRentados && !hayNoRentados)
                {
                    error = "- Error al obtener los datos del reporte, no se encontraron registros para los criterios seleccionados";
                    return false;
                }
                return true;
            }
            catch
            {
                error = "- Error al establecer los datos del reporte";
                return false;
            }
        }
    }
}
