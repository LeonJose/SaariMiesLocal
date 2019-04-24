using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.Abstracts;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class Incrementos : SaariReport, IReport, IBackgroundReport
    {
        public string GrupoEmpresarial { get; set; }
        public string Inmobiliaria { get; set; }
        public string Conjunto { get; set; }
        public string SubConjunto { get; set; }
        public string Cliente { get; set; }
        public decimal TipoDeCambio { get; set; }
        public string Usuario { get; set; }

        private string error = string.Empty;
        private string nombreGpoEmp = string.Empty;
        private string[] inmobiliarias;
        private string[] municipios;
        private string[] estados;
        private string[] identificadores;
        private string[] nombresConjuntos;
        private string[] direcciones;
        private decimal[] terrenosM2;
        private decimal[] construccionM2;
        private string[] arrendatarios;
        private decimal[] rentasActuales;
        private DateTime[] iniciosVig;
        private DateTime[] finVig;
        private DateTime[] fechasAumentos;
        private string[] incrementos;
        private decimal[] depositosGarantia;
        private string[] notasContrato;
        private string filename = string.Empty;

        public Incrementos()
        {

        }

        public Incrementos(string idGrupo, string idInmobiliaria, string idConjunto, string idSubconjunto, string idCliente, decimal tipoCambio, string usuario)
        {
            GrupoEmpresarial = idGrupo;
            Inmobiliaria = idInmobiliaria;
            Conjunto = idConjunto;
            SubConjunto = idSubconjunto;
            Cliente = idCliente;
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
                    Excel.Range rango = hojaExcel.get_Range("A1:Q1");
                    rango.Merge();
                    rango.Value2 = "Incrementos del Grupo Empresarial: " + nombreGpoEmp + " al: " + DateTime.Now.ToString("dd/MMMM/yyyy");
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 18;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //subencabezados
                    rango = hojaExcel.get_Range("A2:Q2");
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
                    hojaExcel.get_Range("D2").Value2 = "Reg.";
                    hojaExcel.get_Range("E2").Value2 = "Concepto";
                    hojaExcel.get_Range("F2").Value2 = "Dirección";
                    hojaExcel.get_Range("G2").Value2 = "Terreno M2";
                    hojaExcel.get_Range("H2").Value2 = "Construcción M2";
                    hojaExcel.get_Range("I2").Value2 = "Arrendatario";
                    hojaExcel.get_Range("J2").Value2 = "Renta mensual";
                    hojaExcel.get_Range("K2").Value2 = "Renta anual";
                    hojaExcel.get_Range("L2").Value2 = "Inicio de arrendamiento";
                    hojaExcel.get_Range("M2").Value2 = "Termino de arrendamiento";
                    hojaExcel.get_Range("N2").Value2 = "Próximo incremento";
                    hojaExcel.get_Range("O2").Value2 = "Incremento pactado";
                    hojaExcel.get_Range("P2").Value2 = "Depósito en garantía";
                    hojaExcel.get_Range("Q2").Value2 = "Contacto con el cliente";

                    //variablesDeControl
                    int posicionRango = 3;
                    int posicionTotal = posicionRango + inmobiliarias.Length;

                    OnCambioProgreso(70);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //ColumnaA
                    if (inmobiliarias != null)
                    {
                        foreach (string inmob in inmobiliarias)
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
                    }
                    
                    //ColumnaB
                    posicionRango = 3;
                    if (municipios != null)
                    {
                        foreach (string muni in municipios)
                        {
                            if (!string.IsNullOrEmpty(muni))
                            {
                                rango = hojaExcel.get_Range("B" + posicionRango);
                                rango.Value2 = muni;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                    }

                    //ColumnaC
                    posicionRango = 3;
                    if (estados != null)
                    {
                        foreach (string estd in estados)
                        {
                            if (!string.IsNullOrEmpty(estd))
                            {
                                rango = hojaExcel.get_Range("C" + posicionRango);
                                rango.Value2 = estd;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                    }

                    //ColumnaD
                    posicionRango = 3;
                    if (identificadores != null)
                    {
                        foreach (string id in identificadores)
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                rango = hojaExcel.get_Range("D" + posicionRango);
                                rango.Value2 = id;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                    }

                    //ColumnaE
                    posicionRango = 3;
                    if (nombresConjuntos != null)
                    {
                        foreach (string nameSubConj in nombresConjuntos)
                        {
                            if (!string.IsNullOrEmpty(nameSubConj))
                            {
                                rango = hojaExcel.get_Range("E" + posicionRango);
                                rango.Value2 = nameSubConj;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                    }

                    //ColumnaF
                    posicionRango = 3;
                    if (direcciones != null)
                    {
                        foreach (string direccion in direcciones)
                        {
                            if (!string.IsNullOrEmpty(direccion))
                            {
                                rango = hojaExcel.get_Range("F" + posicionRango);
                                rango.Value2 = direccion;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                    }

                    //ColumnaG
                    posicionRango = 3;
                    if (terrenosM2 != null)
                    {
                        foreach (decimal terreno in terrenosM2)
                        {
                            rango = hojaExcel.get_Range("G" + posicionRango);
                            rango.Value2 = terreno;
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }

                    //ColumnaH
                    posicionRango = 3;
                    if (construccionM2 != null)
                    {
                        foreach (decimal con in construccionM2)
                        {
                            rango = hojaExcel.get_Range("H" + posicionRango);
                            rango.Value2 = con;
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }

                    //ColumnaI
                    posicionRango = 3;
                    if (arrendatarios != null)
                    {
                        foreach (string arrenda in arrendatarios)
                        {
                            if (!string.IsNullOrEmpty(arrenda))
                            {
                                rango = hojaExcel.get_Range("I" + posicionRango);
                                rango.Value2 = arrenda;
                                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                posicionRango++;
                            }
                        }
                    }
                    
                    //ColumnaJyK
                    posicionRango = 3;
                    if (rentasActuales != null)
                    {
                        foreach (decimal rentaAct in rentasActuales)
                        {
                            rango = hojaExcel.get_Range("J" + posicionRango);
                            rango.Value2 = rentaAct;
                            rango.NumberFormat = "$###,###,###,###,##0.00";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                            rango = hojaExcel.get_Range("K" + posicionRango);
                            rango.Value2 = rentaAct * 12;
                            rango.NumberFormat = "$###,###,###,###,##0.00";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                            posicionRango++;
                        }
                    }

                    //ColumnaL
                    posicionRango = 3;
                    if (iniciosVig != null)
                    {
                        foreach (DateTime ini in iniciosVig)
                        {
                            rango = hojaExcel.get_Range("L" + posicionRango);
                            rango.Value2 = ini.ToString("dd/MMMM/yyyy");
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }

                    //ColumnaM
                    posicionRango = 3;
                    if (finVig != null)
                    {
                        foreach (DateTime fin in finVig)
                        {
                            rango = hojaExcel.get_Range("M" + posicionRango);
                            rango.Value2 = fin.ToString("dd/MMMM/yyyy");
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }

                    //ColumnaN
                    posicionRango = 3;
                    if (fechasAumentos != null)
                    {
                        foreach (DateTime fechas in fechasAumentos)
                        {
                            rango = hojaExcel.get_Range("N" + posicionRango);
                            rango.Value2 = fechas.ToString("dd/MMMM/yyyy");
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }

                    //ColumnaO
                    posicionRango = 3;
                    if (incrementos != null)
                    {
                        foreach (string inc in incrementos)
                        {
                            rango = hojaExcel.get_Range("O" + posicionRango);
                            rango.Value2 = inc;
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }

                    //ColumnaP
                    posicionRango = 3;
                    if (depositosGarantia != null)
                    {
                        foreach (decimal deposito in depositosGarantia)
                        {
                            rango = hojaExcel.get_Range("P" + posicionRango);
                            rango.Value2 = deposito;
                            rango.NumberFormat = "$###,###,###,###,##0.00";
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }

                    //ColumnaQ
                    posicionRango = 3;
                    if (notasContrato != null)
                    {
                        foreach (string nota in notasContrato)
                        {
                            rango = hojaExcel.get_Range("Q" + posicionRango);
                            rango.Value2 = nota;
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            posicionRango++;
                        }
                    }

                    OnCambioProgreso(80);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    //TOTALES
                    rango = hojaExcel.get_Range("A" + posicionTotal + ":Q"+posicionTotal);
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    rango = hojaExcel.get_Range("A" + posicionTotal);
                    rango.Value2 = "TOTAL";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;

                    rango = hojaExcel.get_Range("G" + posicionTotal);
                    rango.Formula = "=SUMA(G3:G" + (posicionTotal - 1)+")";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;

                    rango = hojaExcel.get_Range("H" + posicionTotal);
                    rango.Formula = "=SUMA(H3:H" + (posicionTotal - 1) + ")";
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;

                    rango = hojaExcel.get_Range("J" + posicionTotal);
                    rango.Formula = "=SUMA(J3:J" + (posicionTotal - 1) + ")";
                    
                    rango = hojaExcel.get_Range("K" + posicionTotal);
                    rango.Formula = "=SUMA(K3:K" + (posicionTotal - 1) + ")";
                    
                    rango = hojaExcel.get_Range("P" + posicionTotal);
                    rango.Formula = "=SUMA(P3:P" + (posicionTotal - 1) + ")";

                    //Tipo de cambio
                    rango = hojaExcel.get_Range("A" + (posicionTotal + 1));
                    rango.Value2 = "Tipo de cambio: " + TipoDeCambio;
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    //Generado por
                    rango = hojaExcel.get_Range("A" + (posicionTotal + 2));
                    rango.Value2 = "Reporte generado por: " + Usuario;
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 11;
                    rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                    rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
                    rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    OnCambioProgreso(90);
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
                    hojaExcel.get_Range("I2:I1000").Columns.AutoFit();
                    hojaExcel.get_Range("J2:J1000").Columns.AutoFit();
                    hojaExcel.get_Range("K2:K1000").Columns.AutoFit();
                    hojaExcel.get_Range("O2:O1000").Columns.AutoFit();
                    hojaExcel.get_Range("Q2:Q1000").Columns.AutoFit();
                    
                    //MostrarReporte
                    filename = GestorReportes.Properties.Settings.Default.RutaRepositorio + @"Incrementos_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + ".xlsx";
                    libroExcel.SaveAs(filename);
                    aplicacionExcel.Visible = true;
                    OnCambioProgreso(100);                    
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
                DataTable dtDatos = inmobiliaria.getDtArrendadorasContratosClientes(GrupoEmpresarial, Inmobiliaria, Conjunto, SubConjunto, Cliente);
                if (dtDatos.Rows.Count > 0)
                {
                    inmobiliarias = new string[dtDatos.Rows.Count];
                    municipios = new string[dtDatos.Rows.Count];
                    estados = new string[dtDatos.Rows.Count];
                    identificadores = new string[dtDatos.Rows.Count];
                    nombresConjuntos = new string[dtDatos.Rows.Count];
                    direcciones = new string[dtDatos.Rows.Count];
                    terrenosM2 = new decimal[dtDatos.Rows.Count];
                    construccionM2 = new decimal[dtDatos.Rows.Count];
                    arrendatarios = new string[dtDatos.Rows.Count];
                    rentasActuales = new decimal[dtDatos.Rows.Count];
                    iniciosVig = new DateTime[dtDatos.Rows.Count];
                    finVig = new DateTime[dtDatos.Rows.Count];
                    fechasAumentos = new DateTime[dtDatos.Rows.Count];
                    incrementos = new string[dtDatos.Rows.Count];
                    depositosGarantia = new decimal[dtDatos.Rows.Count];
                    notasContrato = new string[dtDatos.Rows.Count];
                    
                    OnCambioProgreso(20);
                    if (CancelacionPendiente)
                    {
                        error = "Proceso cancelado por el usuario";
                        return false;
                    }

                    int porcentaje = 20;
                    decimal factor = 40 / dtDatos.Rows.Count;
                    factor = factor >= 1 ? factor : 1;

                    int contador = 0;
                    foreach (DataRow row in dtDatos.Rows)
                    {
                        if (porcentaje <= 60)
                            porcentaje += Convert.ToInt32(factor);
                        OnCambioProgreso(porcentaje <= 60 ? porcentaje : 60);
                        if (CancelacionPendiente)
                        {
                            error = "Proceso cancelado por el usuario";
                            return false;
                        }

                        DataTable dtInmubPorSub = inmobiliaria.getDtInmueblesPorSubConj(row["P1801_ID_SUBCONJUNTO"].ToString());
                        inmobiliarias[contador] = row["P0103_RAZON_SOCIAL"].ToString();

                        if (string.IsNullOrEmpty(row["P0506_CIUDAD"].ToString()))
                        {
                            if (dtInmubPorSub.Rows.Count > 0)
                            {
                                if (string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0506_CIUDAD"].ToString()))
                                {
                                    municipios[contador] = "-";
                                }
                                else
                                {
                                    municipios[contador] = dtInmubPorSub.Rows[0]["P0506_CIUDAD"].ToString();
                                }
                            }
                            else
                                municipios[contador] = "-";
                        }
                        else
                            municipios[contador] = row["P0506_CIUDAD"].ToString();

                        if (string.IsNullOrEmpty(row["P0507_ESTADO"].ToString()))
                        {
                            if (dtInmubPorSub.Rows.Count > 0)
                            {
                                if (string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0507_ESTADO"].ToString()))
                                {
                                    estados[contador] = "-";
                                }
                                else
                                {
                                    estados[contador] = dtInmubPorSub.Rows[0]["P0507_ESTADO"].ToString();
                                }
                            }
                            else
                                estados[contador] = "-";
                        }
                        else
                            estados[contador] = row["P0507_ESTADO"].ToString();

                        if (string.IsNullOrEmpty(row["CAMPO4"].ToString()))
                            identificadores[contador] = "-"; 
                        else
                            identificadores[contador] = row["CAMPO4"].ToString();

                        if (string.IsNullOrEmpty(row["P1803_NOMBRE"].ToString()))
                            nombresConjuntos[contador] = "-";
                        else
                            nombresConjuntos[contador] = row["P1803_NOMBRE"].ToString();

                        if (string.IsNullOrEmpty(row["P0503_CALLE_NUM"].ToString()) && string.IsNullOrEmpty(row["P0504_COLONIA"].ToString()))
                        {
                            if (dtInmubPorSub.Rows.Count > 0)
                            {
                                if (string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0503_CALLE_NUM"].ToString()) && string.IsNullOrEmpty(dtInmubPorSub.Rows[0]["P0504_COLONIA"].ToString()))
                                {
                                    direcciones[contador] = "-";
                                }
                                else
                                {
                                    direcciones[contador] = dtInmubPorSub.Rows[0]["P0503_CALLE_NUM"].ToString() + ", " + dtInmubPorSub.Rows[0]["P0504_COLONIA"].ToString();
                                }
                            }
                            else
                                direcciones[contador] = "-";
                        }
                        else
                            direcciones[contador] = row["P0503_CALLE_NUM"].ToString() + ", " + row["P0504_COLONIA"].ToString();

                        decimal terr = 0;
                        decimal constru = 0;
                        foreach (DataRow dr in dtInmubPorSub.Rows)
                        {
                            if (!string.IsNullOrEmpty(dr["P1926_CIST_ING"].ToString()))
                            {
                                try
                                {
                                    terr += Convert.ToDecimal(dr["P1926_CIST_ING"].ToString());
                                }
                                catch
                                {
                                    terr += 0;
                                }
                            }
                            if (!string.IsNullOrEmpty(dr["CAMPO_NUM1"].ToString()))
                            {
                                try
                                {
                                    constru += Convert.ToDecimal(dr["CAMPO_NUM1"].ToString());
                                }
                                catch
                                {
                                    constru += 0;
                                }
                            }
                        }
                        terrenosM2[contador] = terr;
                        construccionM2[contador] = constru;

                        if (string.IsNullOrEmpty(row["P0203_NOMBRE"].ToString()))
                            arrendatarios[contador] = "-";
                        else
                            arrendatarios[contador] = row["P0203_NOMBRE"].ToString();

                        if (string.IsNullOrEmpty(row["P0434_IMPORTE_ACTUAL"].ToString()))
                            rentasActuales[contador] = 0;
                        else
                        {
                            try
                            {
                                if (row["P0407_MONEDA_FACT"].ToString() == "D")
                                {
                                    rentasActuales[contador] = (Convert.ToDecimal(row["P0434_IMPORTE_ACTUAL"].ToString()) * TipoDeCambio);
                                }
                                else
                                    rentasActuales[contador] = Convert.ToDecimal(row["P0434_IMPORTE_ACTUAL"].ToString());
                            }
                            catch
                            {
                                rentasActuales[contador] = 0;
                            }                            
                        }
                        
                        if (string.IsNullOrEmpty(row["P0410_INICIO_VIG"].ToString()))
                            iniciosVig[contador] = DateTime.Now.AddYears(-50);
                        else
                        {
                            try
                            {
                                iniciosVig[contador] = Convert.ToDateTime(row["P0410_INICIO_VIG"].ToString());
                            }
                            catch
                            {
                                iniciosVig[contador] = DateTime.Now.AddYears(-50);
                            }
                        }

                        if (string.IsNullOrEmpty(row["P0411_FIN_VIG"].ToString()))
                            finVig[contador] = DateTime.Now.AddYears(-50);
                        else
                        {
                            try
                            {
                                finVig[contador] = Convert.ToDateTime(row["P0411_FIN_VIG"].ToString());
                            }
                            catch
                            {
                                finVig[contador] = DateTime.Now.AddYears(-50);
                            }
                        }

                        if (string.IsNullOrEmpty(row["P0432_FECHA_AUMENTO"].ToString()))
                            fechasAumentos[contador] = DateTime.Now.AddYears(-50);
                        else
                        {
                            try
                            {
                                fechasAumentos[contador] = Convert.ToDateTime(row["P0432_FECHA_AUMENTO"].ToString());
                            }
                            catch
                            {
                                fechasAumentos[contador] = DateTime.Now.AddYears(-50);
                            }
                        }

                        if (string.IsNullOrEmpty(row["P0441_BASE_PARA_AUMENTO"].ToString()))
                            incrementos[contador] = "-";
                        else
                        {
                            string baseAum = row["P0441_BASE_PARA_AUMENTO"].ToString();
                            if (baseAum == "PRC")
                            {
                                if (!string.IsNullOrEmpty(row["P0415_AUMENTO_ANUAL"].ToString()))
                                    incrementos[contador] = row["P0415_AUMENTO_ANUAL"].ToString() + "%";
                                else
                                    incrementos[contador] = "-";
                            }
                            else if (baseAum == "INPC+PRC")
                            {
                                incrementos[contador] = "INPC+"+ row["P0415_AUMENTO_ANUAL"].ToString() + "%";
                            }
                            else if (baseAum == "IMPFIJO")
                            {
                                if (!string.IsNullOrEmpty(row["P0415_AUMENTO_ANUAL"].ToString()))
                                {
                                    if (row["P0407_MONEDA_FACT"].ToString() == "D")
                                    {
                                        incrementos[contador] = "$" + Decimal.Round((Convert.ToDecimal(row["P0415_AUMENTO_ANUAL"].ToString()) * TipoDeCambio), 2).ToString();
                                    }
                                    else
                                        incrementos[contador] = row["P0415_AUMENTO_ANUAL"].ToString();
                                }
                                else
                                    incrementos[contador] = "-";
                            }
                            else if (baseAum == "INPC")
                            {
                                incrementos[contador] = baseAum;
                            }
                            else
                                incrementos[contador] = "-";
                        }

                        if (string.IsNullOrEmpty(row["P0418_IMPORTE_DEPOSITO"].ToString()))
                            depositosGarantia[contador] = 0;
                        else
                        {
                            try
                            {
                                if (row["P0407_MONEDA_FACT"].ToString() == "D")
                                {
                                    depositosGarantia[contador] = (Convert.ToDecimal(row["P0418_IMPORTE_DEPOSITO"].ToString()) * TipoDeCambio);
                                }
                                else
                                    depositosGarantia[contador] = Convert.ToDecimal(row["P0418_IMPORTE_DEPOSITO"].ToString());
                            }
                            catch
                            {
                                depositosGarantia[contador] = 0;
                            }
                        }

                        if (string.IsNullOrEmpty(row["P0448_CAMPO12"].ToString()))
                            notasContrato[contador] = "-";
                        else
                            notasContrato[contador] = row["P0448_CAMPO12"].ToString();

                        contador++;
                    }
                }
                else
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
