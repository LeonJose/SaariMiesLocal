using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesReportes;
using Excel = Microsoft.Office.Interop.Excel;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.Abstracts;
using System.Globalization;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class SaldoVenta : SaariReport, IReport, IBackgroundReport
    {
        private Inmobiliaria inmo = new Inmobiliaria();
        private string nombreArchivo = string.Empty;

        public override string NombreArchivo
        {
            get
            {
                return nombreArchivo;
            }
        }
        public string IDArrendadora { get; set; }
        public DateTime Fecha { get; set; }
        public bool IncluirPreventa { get; set; }
        public bool AbrirReporte { get; set; }

        public SaldoVenta(string arr, DateTime fecha)
        {
            IDArrendadora = arr;
            Fecha = fecha;
        }
        public SaldoVenta(string arr, DateTime fecha, bool incluirPreventa, bool Abrir)
        {
            IDArrendadora = arr;
            Fecha = fecha;
            IncluirPreventa = incluirPreventa;
            AbrirReporte = Abrir;
        }

        /// <summary>
        /// Genera el reporte de saldo para venta de manera asíncrona.
        /// </summary>
        /// <param name="worker">Componente para trabajar en segundo plano</param>
        /// <returns>Cadena vacía en caso de éxito; en caso de error regresa la descripción del mismo</returns>
        public string generar()
        {
            try
            {
                /*OnCambioProgreso(10);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";*/
                string nombreEmpresa = inmo.getNombreArrendadoraPorID(IDArrendadora);
                List<EdificiosYContratos> listaEdificiosAux = inmo.getEdificiosYContratos(IDArrendadora, IncluirPreventa);
                if (listaEdificiosAux != null)
                {
                    /*OnCambioProgreso(20);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";*/
                    if (listaEdificiosAux.Count > 0)
                    {
                        decimal prc = 0;
                        int contador = 1;
                        List<EdificiosYContratos> listaEdificios = new List<EdificiosYContratos>();
                        if (IncluirPreventa)
                            listaEdificios = filtrarContratosPreventayVenta(listaEdificiosAux);
                        else
                            listaEdificios = listaEdificiosAux;

                        foreach (EdificiosYContratos ediYcont in listaEdificios)
                        {
                            if (!string.IsNullOrEmpty(ediYcont.Contrato.ID))
                            {
                                ediYcont.Pago = inmo.getPagosPorIDContratoYFecha(ediYcont.Contrato.ID, Fecha);
                            }
                            prc = (decimal)contador / (decimal)listaEdificios.Count;
                            prc = (prc * 50);
                            OnCambioProgreso((int)prc);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";
                            contador++;
                        }
                        return generarEnExcel(listaEdificios, nombreEmpresa);
                    }
                    else
                        return "No se encontraron edificos y/o contratos para la inmobiliaria";
                }
                else
                    return "Hubo un error obtener la lista de edificios y contratos";
            }
            catch (Exception ex)
            {
                return "Se produjo un error inesperado: " + Environment.NewLine + ex.Message;
            }
        }

        /// <summary>
        /// Genera el reporte de saldo venta en el proceso actual
        /// </summary>
        /// <returns>Cadena vacía en caso de éxito; en caso de error regresa la descripción del mismo</returns>
        /*public string generarReporte()
        {
            try
            {
                string nombreEmpresa = inmo.getNombreArrendadoraPorID(IDArrendadora);
                List<EdificiosYContratos> listaEdificios = inmo.getEdificiosYContratos(IDArrendadora);
                if (listaEdificios != null)
                {
                    if (listaEdificios.Count > 0)
                    {
                        foreach (EdificiosYContratos ediYcont in listaEdificios)
                        {
                            if (!string.IsNullOrEmpty(ediYcont.Contrato.ID))
                                ediYcont.Pago = inmo.getPagosPorIDContratoYFecha(ediYcont.Contrato.ID, Fecha);
                        }
                        return generarEnExcel(listaEdificios, nombreEmpresa);
                    }
                    else
                        return "No se encontraron edificos y/o contratos para la inmobiliaria";
                }
                else
                    return "Hubo un error obtener la lista de edificios y contratos";
            }
            catch (Exception ex)
            {
                return "Se produjo un error inesperado: " + Environment.NewLine + ex.Message;
            }
        }

        private string generarEnExcel(List<EdificiosYContratos> lista, string nombreEmpresa)
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                //ObjetosExcel
                Excel.Application aplicacionExcel = new Excel.Application();
                Excel.Workbook libroExcel = aplicacionExcel.Workbooks.Add();
                Excel.Worksheet hojaExcel = (libroExcel.ActiveSheet as Excel.Worksheet);;

                //Encabezado
                Excel.Range rango = hojaExcel.get_Range("A1:R1");                
                rango.Merge();
                rango.Value2 = nombreEmpresa.ToUpper();
                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                rango.Font.Bold = true;
                rango.Font.Size = 18;
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(54, 96, 146));

                //Subencabezado
                rango = hojaExcel.get_Range("A2:R2");
                rango.Merge();
                rango.Value2 = "Reporte de saldos de venta al " + Fecha.ToShortDateString();
                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                rango.Font.Bold = true;
                rango.Font.Size = 16;
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(54, 96, 146));

                //Titulos
                const int totalColumnas = 17;
                for (int col = 1; col <= totalColumnas; col++)
                {
                    string valor = string.Empty;
                    switch (col)
                    {
                        case 1: valor = "NUM.";
                                break;
                        case 2: valor = "LOTE";
                                break;
                        case 3: valor = "TIPO";
                                break;
                        case 4: valor = "CLIENTE";
                                break;
                        case 5: valor = "SUPERFICIE";
                                break;
                        case 6: valor = "PRECIO M2";
                                break;
                        case 7: valor = "TOTAL";
                                break;
                        case 8: valor = "ESTATUS";
                                break;
                        case 9: valor = "LOTES VENDIDOS";
                                break;
                        case 10: valor = "ENGANCHE";
                                break;
                        case 11: valor = "ENGANCHE %";
                                break;
                        case 12: valor = "TOTAL";
                                break;
                        case 13: valor = "TOTAL A PAGAR";
                                break;
                        case 14: valor = "TOTAL ABONADO";
                                break;
                        case 15: valor = "IMPORTE POR PAGAR";
                                break;
                        case 16: valor = "PRC. (%)";
                                break;
                        case 17: valor = "SUPERFICIE POR VENDER";
                                break;
                        default: valor = string.Empty;
                            break;
                    }
                    
                    rango = hojaExcel.Cells[3, col] as Excel.Range;
                    rango.Value2 = valor;
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 12;
                }

                int renglon = 4;
                foreach (EdificiosYContratos elemento in lista)
                {
                    if (string.IsNullOrEmpty(elemento.Contrato.Tipo) || elemento.Contrato.Tipo == "V" || elemento.Contrato.Tipo == "W")
                    {
                        rango = hojaExcel.Cells[renglon, 1] as Excel.Range;
                        rango.Value2 = renglon - 3;
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 2] as Excel.Range;
                        rango.Value2 = elemento.Edificio.Manzana + " " + elemento.Edificio.Lote;
                        rango.HorizontalAlignment = Excel.Constants.xlLeft;
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 3] as Excel.Range;
                        rango.Value2 = elemento.Edificio.Tipo;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 4] as Excel.Range;
                        rango.Value2 = elemento.Contrato.NombreCliente;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 5] as Excel.Range;                        
                        rango.Value2 = elemento.Edificio.Terreno.ToString("N2");
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.NumberFormat = "#,0.00";
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 6] as Excel.Range;
                        rango.Value2 = elemento.Edificio.ValorPorMetro.ToString("N2");
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 7] as Excel.Range;
                        rango.Value2 = elemento.Edificio.Valor.ToString("N2");
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 8] as Excel.Range;
                        rango.Value2 = string.IsNullOrEmpty(elemento.Contrato.ID) ? "En venta" : "Vendido";
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 9] as Excel.Range;
                        rango.Value2 = string.IsNullOrEmpty(elemento.Contrato.ID) ? "0" : "1";
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 10] as Excel.Range;
                        rango.Value2 = elemento.Contrato.Enganche.ToString("N2");
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 11] as Excel.Range;
                        rango.Formula = "=J" + renglon + "/G" + renglon;
                        rango.NumberFormat = "0%";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 12] as Excel.Range;
                        rango.Formula = "=SUM(G" + renglon + "-J" + renglon + ")";
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        //rango.NumberFormat = "N2";
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 13] as Excel.Range;
                        rango.Formula = "=G" + renglon;
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        //rango.NumberFormat = "N2";
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 14] as Excel.Range;
                        rango.Value2 = (elemento.Pago + elemento.Contrato.Enganche).ToString("N2");
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 15] as Excel.Range;
                        rango.Formula = "=SUM(M" + renglon + "-N" + renglon + ")";
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        //rango.NumberFormat = "N2";
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 16] as Excel.Range;
                        rango.Formula = "=N" + renglon + "/M" + renglon;
                        rango.NumberFormat = "0%";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Size = 10;

                        rango = hojaExcel.Cells[renglon, 17] as Excel.Range;
                        rango.Value2 = string.IsNullOrEmpty(elemento.Contrato.ID) ? elemento.Edificio.Terreno.ToString("N2") : "0";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Size = 10;

                        renglon++;
                    }
                }

                //Sumas
                rango = hojaExcel.Cells[renglon, 5] as Excel.Range;
                rango.Formula = "=SUM(F4:F" + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;

                rango = hojaExcel.Cells[renglon, 7] as Excel.Range;
                rango.Formula = "=SUM(G4:G" + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;

                rango = hojaExcel.Cells[renglon, 10] as Excel.Range;
                rango.Formula = "=SUM(J4:J" + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;

                rango = hojaExcel.Cells[renglon, 12] as Excel.Range;
                rango.Formula = "=SUM(L4:L" + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;
                rango.NumberFormat = "#,##0.00";

                rango = hojaExcel.Cells[renglon, 13] as Excel.Range;
                rango.Formula = "=SUM(M4:M" + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;
                rango.NumberFormat = "#,##0.00";

                rango = hojaExcel.Cells[renglon, 14] as Excel.Range;
                rango.Formula = "=SUM(N4:N" + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;

                rango = hojaExcel.Cells[renglon, 15] as Excel.Range;
                rango.Formula = "=SUM(O4:O" + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;

                rango = hojaExcel.Cells[renglon, 17] as Excel.Range;
                rango.Formula = "=SUM(Q4:Q" + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;

                //Bordes
                rango = hojaExcel.get_Range("A3:R" + renglon);
                rango.BorderAround2();
                rango.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle = Excel.XlLineStyle.xlContinuous;
                rango.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                                
                //Ajustar columnas
                hojaExcel.Columns.AutoFit();

                //Ocultar ceros
                hojaExcel.Application.ActiveWindow.DisplayZeros = false;

                //Inmovilizar encabezados
                hojaExcel.Application.ActiveWindow.SplitRow = 3;
                /*rango = hojaExcel.get_Range("A1:R3");
                rango.Select();
                hojaExcel.Application.ActiveWindow.FreezePanes = true;

                //MostrarReporte
                string filename = GestorReportes.Properties.Settings.Default.RutaRepositorio + @"ReporteSaldoVenta_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + ".xlsx";                 
                libroExcel.SaveAs(filename);
                aplicacionExcel.Visible = true;
                liberaProceso();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error en Excel: " + Environment.NewLine + ex.Message;
            }
        }*/

        private List<EdificiosYContratos> filtrarContratosPreventayVenta(List<EdificiosYContratos> ListFiltrarContratos)
        {
            List<EdificiosYContratos> ListaContratosPre = new List<EdificiosYContratos>();
            List<EdificiosYContratos> ListaContratosVnt = new List<EdificiosYContratos>();
            List<EdificiosYContratos> ListaFiltrada = new List<EdificiosYContratos>();

            foreach (EdificiosYContratos contrato in ListFiltrarContratos)
            {
                if (contrato.Contrato.Tipo == "P" || contrato.Contrato.Tipo == "Q")
                {
                    ListaContratosPre.Add(contrato);
                }
                else
                {
                    ListaContratosVnt.Add(contrato);
                }
            }
            ListaFiltrada = ListaContratosVnt;

            foreach (EdificiosYContratos ContratoPre in ListaContratosPre)
            {
                if (!ListaFiltrada.Exists(e => (e.Edificio.ID == ContratoPre.Edificio.ID) && (e.Contrato.Tipo =="V" || e.Contrato.Tipo == "W" )))
                {
                    ListaFiltrada.Add(ContratoPre);
                }
                
            }

            return ListaFiltrada;
        }

        private string generarEnExcel(List<EdificiosYContratos> lista, string nombreEmpresa)
        {
            try
            {
                decimal prc = 0;
                int contador = 1;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                //ObjetosExcel
                Excel.Application aplicacionExcel = new Excel.Application();
                Excel.Workbook libroExcel = aplicacionExcel.Workbooks.Add();
                Excel.Worksheet hojaExcel = (libroExcel.ActiveSheet as Excel.Worksheet);;
                //aplicacionExcel.Visible = true;
                //Encabezado
                Excel.Range rango = hojaExcel.get_Range("A1:R1");
                rango.Merge();
                rango.Value2 = nombreEmpresa.ToUpper();
                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                rango.Font.Bold = true;
                rango.Font.Size = 18;
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(54, 96, 146));

                //Subencabezado
                rango = hojaExcel.get_Range("A2:R2");
                rango.Merge();
                rango.Value2 = "Reporte de saldos de venta al " +Fecha.Day+"/"+MonthName(Fecha.Month)+"/"+Fecha.Year;
                rango.HorizontalAlignment = Excel.Constants.xlCenter;
                rango.Font.Bold = true;
                rango.Font.Size = 16;
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(54, 96, 146));

                //Titulos
                List<string> ListaEncabezados = Enum.GetNames(typeof(EncabezadoSaldoVentas)).ToList();
                int totalColumnas = ListaEncabezados.Count;
                int col = 1;
                foreach(string Titulo in ListaEncabezados)
                {
                    string valor = Titulo;
                    {
                        valor = valor.Replace("_"," ").Trim();
                        if (valor == "ENGANCHE")
                            valor += " %";
                        else if (valor == "PRC")
                            valor += ". (%)";
                        else if (valor == "ENGANCHE 1")
                            valor = valor.Replace("1", "");
                    }

                    rango = hojaExcel.Cells[3, col] as Excel.Range;
                    rango.Value2 = valor;
                    rango.HorizontalAlignment = Excel.Constants.xlCenter;
                    rango.Font.Bold = true;
                    rango.Font.Size = 12;
                    col ++;
                }
                int renglon = 4;
                foreach (EdificiosYContratos elemento in lista)
                {
                    prc = (decimal)contador / (decimal)lista.Count;
                    prc = (prc * 50) + 50;
                    if (prc > 100)
                        prc = 99;
                    OnCambioProgreso((int)prc);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    contador++;
                    //if (string.IsNullOrEmpty(elemento.Contrato.Tipo) || elemento.Contrato.Tipo == "V" || elemento.Contrato.Tipo == "W")
                    {
                        if (elemento.Contrato.ID == "CNT1640")
                        { }
                        //NUM COL A
                        col = 1;
                        bool enVenta = string.IsNullOrEmpty(elemento.Contrato.ID);
                        rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                        rango.Value2 = renglon - 3;
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.Font.Size = 10;
                        col++;
                        //MANZANA COL B
                        rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                        rango.Value2 = elemento.Edificio.Manzana;
                        rango.HorizontalAlignment = Excel.Constants.xlLeft;
                        rango.Font.Size = 10;
                        col++;
                        //LOTE COL C
                        rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                        rango.Value2 = elemento.Edificio.Lote;
                        rango.HorizontalAlignment = Excel.Constants.xlLeft;
                        rango.Font.Size = 10;
                        col++;
                        //TIPO EDIFICIO COL D
                        rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                        rango.Value2 = elemento.Edificio.Tipo;
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Size = 10;
                        col++;
                        //CLIENTE COL E
                        rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                        rango.Value2 = elemento.Contrato.NombreCliente;
                        rango.HorizontalAlignment = Excel.Constants.xlLeft;
                        rango.Font.Size = 10;
                        col++;
                        //ESTATUS C ONTRATO
                        //rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                        //rango.Value2 = elemento.Contrato.EstatusContrato;
                        //rango.HorizontalAlignment = Excel.Constants.xlLeft;
                        //rango.Font.Size = 10;
                        //col++;
                        //SUPOERFICIE TERRENO COL F
                        rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                        rango.Value2 = elemento.Edificio.Terreno.ToString("N2");
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.NumberFormat = "#,0.00";
                        rango.Font.Size = 10;
                        col++;
                        //PRECIO M2 COL G
                        rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                        rango.Value2 = elemento.Edificio.ValorPorMetro.ToString("N2");
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.Font.Size = 10;
                        col++;
                        string colValorVenta = GetExcelColumnName(col);
                        if (!enVenta)
                        {
                            //VALOR DE VENTA (TOTAL) COL H
                            rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                            rango.Value2 = elemento.Edificio.Valor.ToString("N2");
                            rango.HorizontalAlignment = Excel.Constants.xlRight;
                            rango.Font.Size = 10;
                            col++;
                        }
                        else
                            col++;

                        //ESTATUS COL I
                        rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                        rango.Value2 = elemento.Contrato.EstatusContrato; //rango.Value2 = enVenta ? "En venta" : "Vendido";
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.Font.Size = 10;
                        col++;
                        //LOTES VENDIDOS COL J
                        rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                        rango.Value2 = enVenta ? "0" : "1";
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.Font.Size = 10;
                        col++;
                        //ENGANCHE COL K
                        string colEnganche = GetExcelColumnName(col);
                        rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                        rango.Value2 = elemento.Contrato.Enganche.ToString("N2");
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.Font.Size = 10;
                        col++;
                        string colEnganchePorc = GetExcelColumnName(col);
                        string colTotal = "";
                        string colTotalAPagar = "";
                        if (!enVenta)
                        {
                            //Enganche % COL L
                            colEnganchePorc = GetExcelColumnName(col);
                            rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                            rango.Formula = "="+colEnganche + renglon + "/"+ colValorVenta + renglon;
                            rango.NumberFormat = "0%";
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Size = 10;
                            col++;
                            //TOTAL COL M
                            colTotal = GetExcelColumnName(col);
                            rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                            rango.Formula = "=SUM("+colValorVenta + renglon + "-" +colEnganche + renglon + ")";
                            rango.HorizontalAlignment = Excel.Constants.xlRight;
                            //rango.NumberFormat = "N2";
                            rango.Font.Size = 10;
                            col++;
                            //TOTAL A PAGAR COL N
                            colTotalAPagar = GetExcelColumnName(col);
                            rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                            rango.Formula = "="+colValorVenta + renglon;
                            rango.HorizontalAlignment = Excel.Constants.xlRight;
                            //rango.NumberFormat = "N2";
                            rango.Font.Size = 10;
                            col++;
                        }
                        else
                            col = col + 3;

                        //TOTAL ABONADO COL O
                        string colAbonado = GetExcelColumnName(col);
                        rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                        rango.Value2 = (elemento.Pago + elemento.Contrato.Enganche).ToString("N2");
                        rango.HorizontalAlignment = Excel.Constants.xlRight;
                        rango.Font.Size = 10;
                        col++;
                        
                        if (!enVenta)
                        {
                            //IMPORTE POR PAGAR COL P
                            rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                            rango.Formula = "=SUM("+ colTotalAPagar + renglon + "-"+ colAbonado + renglon + ")";
                            rango.HorizontalAlignment = Excel.Constants.xlRight;
                            //rango.NumberFormat = "N2";
                            rango.Font.Size = 10;
                            col++;
                        }
                        else
                            col++;

                        if (!enVenta)
                        {
                            //PRC % COL Q
                            rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                            rango.Formula = "="+ colAbonado + renglon + "/" + colTotalAPagar + renglon;
                            rango.NumberFormat = "0%";
                            rango.HorizontalAlignment = Excel.Constants.xlCenter;
                            rango.Font.Size = 10;
                            col++;
                        }
                        else
                            col++;
                        //SUPERFICIE POR VENDER COL R
                        rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                        rango.Value2 = string.IsNullOrEmpty(elemento.Contrato.ID) ? elemento.Edificio.Terreno.ToString("N2") : "0";
                        rango.HorizontalAlignment = Excel.Constants.xlCenter;
                        rango.Font.Size = 10;
                        col++;

                        renglon++;
                    }
                }

                col = 6;
                string colName = GetExcelColumnName(col);
                //SUMA SUPERFICIE
                rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                rango.Formula = "=SUM("+ colName+"4:"+ colName + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;
                col++;
                //SUMA PRECIO M2
                colName = GetExcelColumnName(col);
                rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                rango.Formula = "=SUM(" + colName + "4:" + colName + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;
                col++;

                //SUMA TOTAL
                colName = GetExcelColumnName(col);
                rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                rango.Formula = "=SUM(" + colName + "4:" + colName + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;
                rango.NumberFormat = "#,##0.00";

                col = 11;
                //SUMA ENGANCHE
                colName = GetExcelColumnName(col);
                rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                rango.Formula = "=SUM(" + colName + "4:" + colName + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;
                col=13;


                //TOTAL  
                colName = GetExcelColumnName(col);
                rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                rango.Formula = "=SUM(" + colName + "4:" + colName + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;
                rango.NumberFormat = "#,##0.00";
                col++;

                //TOTAL A PAGAR 
                colName = GetExcelColumnName(col);
                rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                rango.Formula = "=SUM(" + colName + "4:" + colName + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;
                rango.NumberFormat = "#,##0.00";
                col++;
                //TOTAL ABONADO
                colName = GetExcelColumnName(col);
                rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                rango.Formula = "=SUM(" + colName + "4:" + colName + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;
                col++;
                //IMPORTE POR PAGAR
                colName = GetExcelColumnName(col);
                rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                rango.Formula = "=SUM(" + colName + "4:" + colName + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;
                col=19;
                //SUPERFICIE POR PAGAR
                colName = GetExcelColumnName(col);
                rango = hojaExcel.Cells[renglon, col] as Excel.Range;
                rango.Formula = "=SUM(" + colName + "4:" + colName + (renglon - 1) + ")";
                rango.HorizontalAlignment = Excel.Constants.xlRight;
                rango.Font.Size = 10;
                rango.Font.Bold = true;

                OnCambioProgreso(90);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                //Bordes
                rango = hojaExcel.get_Range("A3:R" + renglon);
                rango.BorderAround2();
                rango.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle = Excel.XlLineStyle.xlContinuous;
                rango.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;

                //Ajustar columnas
                hojaExcel.Columns.AutoFit();

                //Ocultar ceros
                hojaExcel.Application.ActiveWindow.DisplayZeros = false;

                //Inmovilizar encabezados
                hojaExcel.Application.ActiveWindow.SplitRow = 3;
                hojaExcel.Application.ActiveWindow.FreezePanes = true;

                //MostrarReporte
                string filename = GestorReportes.Properties.Settings.Default.RutaRepositorio + @"ReporteSaldoVenta_" + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + ".xlsx";
                libroExcel.SaveAs(filename);
               // aplicacionExcel.Visible = true;
              //  liberaProceso();
                nombreArchivo = filename;
                try
                {

                    libroExcel.CheckCompatibility = false;
                    libroExcel.Close();
                    aplicacionExcel.Quit();
                    releaseObject(hojaExcel);
                    releaseObject(libroExcel);
                    releaseObject(aplicacionExcel);
                }
                catch(Exception e)
                { }

                if (AbrirReporte)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(filename);
                    }
                    catch
                    {
                        return "No se pudo abrir el archivo" + filename;
                    }
                }

                OnCambioProgreso(100);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error en Excel: " + Environment.NewLine + ex.Message;
            }
        }

        #region Libera proceso Excel
        private int iGetIDProcces(string nameProcces)
        {
            try
            {
                System.Diagnostics.Process[] asProccess = System.Diagnostics.Process.GetProcessesByName(nameProcces);
                foreach (System.Diagnostics.Process pProccess in asProccess)
                {
                    if (pProccess.MainWindowTitle == "")
                    {
                        return pProccess.Id;
                    }
                }
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        public string GetExcelColumnName(int ColumnNumber)
        {
            int intDividend = ColumnNumber;
            string strColumnName = String.Empty;
            int intModulo;
            while (intDividend > 0)
            {
                intModulo = (intDividend - 1) % 26;
                strColumnName = Convert.ToChar(65 + intModulo).ToString() + strColumnName;
                intDividend = (int)((intDividend - intModulo) / 26);
            }
            return strColumnName;
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
            }
            finally
            {
                obj = null;
                GC.Collect();
            }
        }
        public string MonthName(int month)
        {
            DateTimeFormatInfo dtInfo = new CultureInfo("es-MX", false).DateTimeFormat;
            return dtInfo.GetMonthName(month);
        }

        private void liberaProceso()
        {
            int idproc = iGetIDProcces("EXCEL");
            if (idproc != -1)
            {
                System.Diagnostics.Process.GetProcessById(idproc).Kill();
            }
        }
        #endregion
    }
}
