using System;
using System.Collections.Generic;
using System.Text;
using FastReport;
using System.Linq;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using System.Data;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class CarteraVencidaVenta : SaariReport, IReport, IBackgroundReport
    {
        public string Arrendadora { get; set; }
        public string Conjunto { get; set; }
        public string Estatus { get; set; }
        public string Nombre_Inmobiliaria { get; set; }
        public string Formato { get; set; }
        public string Usuario { get; set; }
        public bool Excel { get; set; }

        private int totalRegistros = 0;
        private int totalSumaPagares = 0;
        private int totalVencidos  = 0;
        private decimal totalImporteMensual = 0;
        private decimal totalImporteVencido = 0;
        private decimal totalMes = 0;
        private int totalPorLiquidar = 0;
        private decimal totalImportePorCobrar = 0;

        public string generar()
        {
            if (!System.IO.File.Exists(Formato))
                return "No se encontró el formato especificado";
            try
            {
                OnCambioProgreso(10);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                
                DataTable dtDatos = new DataTable();
                List<CarteraVencidaVentaEntity> datosCartera = SaariDB.getDatosCarteraVenta(Arrendadora, Conjunto, Estatus);
                
                OnCambioProgreso(40);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                if (datosCartera.Count <= 0)
                    return "No se encontraron datos";
                else
                    dtDatos = setComponentes(datosCartera);

                OnCambioProgreso(80);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                TfrxReportClass reporte = new TfrxReportClass();
                reporte.ClearReport();
                reporte.ClearDatasets();
                reporte.LoadReportFromFile(Formato);

                FrxDataTable dtInmobiliaria = new FrxDataTable("dvInmobiliaria");
                DataColumn dcInmobiliaraNombreComercial = new DataColumn("nombreComercial", typeof(string));
                DataColumn dcUsuario = new DataColumn("usuario", typeof(string));
                dtInmobiliaria.Columns.Add(dcInmobiliaraNombreComercial);
                dtInmobiliaria.Columns.Add(dcUsuario);
                dtInmobiliaria.AcceptChanges();
                dtInmobiliaria.Rows.Add(Nombre_Inmobiliaria, Usuario);


                FrxDataTable dtMovimientos = new FrxDataTable("dvMovimientos");
                DataColumn dcContador = new DataColumn("CONTADOR", typeof(string));
                DataColumn dcManzana = new DataColumn("MANZANA", typeof(string));
                DataColumn dcLote = new DataColumn("LOTE", typeof(string));
                DataColumn dcCliente = new DataColumn("CLIENTE", typeof(string));
                DataColumn dcUltPag = new DataColumn("ULTIMO_PAGARE", typeof(string));
                DataColumn dcTotPag = new DataColumn("TOTAL_PAGARES", typeof(string));
                DataColumn dcFechUltPag = new DataColumn("FECHA_ULTIMO_PAGARE", typeof(string));
                DataColumn dcPagvenc = new DataColumn("PAGARES_VENCIDOS", typeof(string));
                DataColumn dcImpMens = new DataColumn("IMPORTE_MENSUAL", typeof(string));
                DataColumn dcImpVenc = new DataColumn("IMPORTE_VENCIDO", typeof(string));
                DataColumn dcPagoMes = new DataColumn("PAGO_MES", typeof(string));
                DataColumn dcPagPorLiq = new DataColumn("PAGARES_POR_LIQUIDAR", typeof(string));
                DataColumn dcImpPorCob = new DataColumn("IMPORTE_POR_COBRAR", typeof(string));
                DataColumn dcObserv = new DataColumn("OBSERVACIONES", typeof(string));
                dtMovimientos.Columns.Add(dcContador);
                dtMovimientos.Columns.Add(dcManzana);
                dtMovimientos.Columns.Add(dcLote);
                dtMovimientos.Columns.Add(dcCliente);
                dtMovimientos.Columns.Add(dcUltPag);
                dtMovimientos.Columns.Add(dcTotPag);
                dtMovimientos.Columns.Add(dcFechUltPag);
                dtMovimientos.Columns.Add(dcPagvenc);
                dtMovimientos.Columns.Add(dcImpMens);
                dtMovimientos.Columns.Add(dcImpVenc);
                dtMovimientos.Columns.Add(dcPagoMes);
                dtMovimientos.Columns.Add(dcPagPorLiq);
                dtMovimientos.Columns.Add(dcImpPorCob);
                dtMovimientos.Columns.Add(dcObserv);
                dtMovimientos.AcceptChanges();

                FrxDataTable dtTotales = new FrxDataTable("dvTotales");
                DataColumn dcTContador = new DataColumn("CONTADOR", typeof(string));
                DataColumn dcTTotPag = new DataColumn("TOTAL_PAGARES", typeof(string));
                DataColumn dcTPagvenc = new DataColumn("PAGARES_VENCIDOS", typeof(string));
                DataColumn dcTImpMens = new DataColumn("IMPORTE_MENSUAL", typeof(string));
                DataColumn dcTImpVenc = new DataColumn("IMPORTE_VENCIDO", typeof(string));
                DataColumn dcTPagoMes = new DataColumn("PAGO_MES", typeof(string));
                DataColumn dcTPagPorLiq = new DataColumn("PAGARES_POR_LIQUIDAR", typeof(string));
                DataColumn dcTImpPorCob = new DataColumn("IMPORTE_POR_COBRAR", typeof(string));
                dtTotales.Columns.Add(dcTContador);
                dtTotales.Columns.Add(dcTTotPag);
                dtTotales.Columns.Add(dcTPagvenc);
                dtTotales.Columns.Add(dcTImpMens);
                dtTotales.Columns.Add(dcTImpVenc);
                dtTotales.Columns.Add(dcTPagoMes);
                dtTotales.Columns.Add(dcTPagPorLiq);
                dtTotales.Columns.Add(dcTImpPorCob);
                dtTotales.AcceptChanges();
                dtTotales.Rows.Add(totalRegistros.ToString(), totalSumaPagares.ToString(), totalVencidos.ToString(), totalImporteMensual.ToString(), totalImporteVencido.ToString(), totalMes.ToString(), totalPorLiquidar.ToString(), totalImportePorCobrar.ToString());

                foreach (DataRow drMov in dtDatos.Rows)
                {
                    dtMovimientos.Rows.Add(drMov["CONTADOR"].ToString(), drMov["MANZANA"].ToString(), drMov["LOTE"].ToString(), drMov["CLIENTE"].ToString(), drMov["ULTIMO_PAGARE"].ToString(), drMov["TOTAL_PAGARES"].ToString(), drMov["FECHA_ULTIMO_PAGARE"].ToString(), drMov["PAGARES_VENCIDOS"].ToString(), drMov["IMPORTE_MENSUAL"].ToString(), drMov["IMPORTE_VENCIDO"].ToString(), drMov["PAGO_MES"].ToString(), drMov["PAGARES_POR_LIQUIDAR"].ToString(), drMov["IMPORTE_POR_COBRAR"].ToString(), drMov["OBSERVACIONES"].ToString());
                }

                FrxDataView dvInmobiliaria = new FrxDataView(dtInmobiliaria, "dvInmobiliaria");
                dvInmobiliaria.AssignToReport(true, reporte);
                //dtInmobiliaria.AssignToDataBand("MasterData1", reporte); //Parche para que aparezca encabezado de reporte

                FrxDataView dvTotales = new FrxDataView(dtTotales, "dvTotales");
                dvTotales.AssignToReport(true, reporte);

                FrxDataView dvMovimientos = new FrxDataView(dtMovimientos, "dvMovimientos");
                dvMovimientos.AssignToReport(true, reporte);
                dtMovimientos.AssignToDataBand("MasterData1", reporte);

                return exportar(reporte, !Excel, "CarteraVencidaVenta");
                //reporte.PrepareReport(true);
                /*if (Excel)
                {
                    try
                    {
                        string directoryTmpReportesSaari = @"C:\Users\Public\Documents\SaariDB\TmpReportes";
                        if (!System.IO.Directory.Exists(directoryTmpReportesSaari))
                            System.IO.Directory.CreateDirectory(directoryTmpReportesSaari);

                        string archivoXlsTemporal = directoryTmpReportesSaari + "\\Reporte" + DateTime.Now.Ticks + ".xls";
                        reporte.ExportToXLS(archivoXlsTemporal, true, false, true, false, false, true);
                        System.Diagnostics.Process.Start(archivoXlsTemporal);
                    }
                    catch 
                    {
                        return "No se pudo exportar a Excel";
                    }
                }
                else
                {
                    reporte.ShowPreparedReport();
                }*/

                //return string.Empty;
            }
            catch
            {
                return "Error inesperado en la generación del reporte";
            }            
        }
                
        /*private DataTable setComponentes(DataTable dtDatos)
        {
            try
            {
                DataTable dtFinal = new DataTable();
                dtFinal.Columns.Add(new DataColumn("CONTADOR", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("MANZANA", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("LOTE", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("CLIENTE", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("ULTIMO_PAGARE", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("TOTAL_PAGARES", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("FECHA_ULTIMO_PAGARE", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("PAGARES_VENCIDOS", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("IMPORTE_MENSUAL", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("IMPORTE_VENCIDO", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("PAGO_MES", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("PAGARES_POR_LIQUIDAR", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("IMPORTE_POR_COBRAR", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("OBSERVACIONES", typeof(string)));
                dtFinal.AcceptChanges();
                List<string> listaContratos = new List<string>();
                int contador = 1;
                foreach (DataRow drDatos in dtDatos.Rows)
                {
                    if (!listaContratos.Contains(drDatos["ID_CONTRATO"].ToString()))
                    {
                        listaContratos.Add(drDatos["ID_CONTRATO"].ToString());
                        string manzana = drDatos["MANZANA"].ToString();
                        string lote = drDatos["LOTE"].ToString();
                        string cliente = drDatos["CLIENTE"].ToString();
                        string ultimoPagare = string.Empty;
                        string totalPagares = "1";
                        if (!string.IsNullOrEmpty(drDatos["TOTAL_PAGARES"].ToString()))
                        {
                            totalPagares = drDatos["TOTAL_PAGARES"].ToString();
                        }
                        string fechaUltimoPagare = string.Empty;
                        int vencidos = 0;
                        string importeMensual = drDatos["IMPORTE_MENSUAL"].ToString();
                        decimal importeVencido = 0;
                        decimal pagosMes = 0;
                        int pagaresPorLiquidar = 0;
                        decimal importePorCobrar = 0;
                        string observaciones = string.Empty;                        
                        DataRow[] drSelect = dtDatos.Select("ID_CONTRATO = '" + drDatos["ID_CONTRATO"].ToString() + "' AND STATUS = 2", "PERIODO DESC");
                        if (drSelect.Length > 0)
                        {
                            int mayor = 0;
                            int periodo = 0;
                            foreach (DataRow drS in drSelect)
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(drS["PERIODO"].ToString()))
                                    {
                                        periodo = 1;
                                    }
                                    else
                                    {
                                        if (drS["PERIODO"].ToString().Contains("p"))
                                            periodo = int.Parse(drS["PERIODO"].ToString().Replace("p", string.Empty));
                                        else
                                            periodo = int.Parse(drS["PERIODO"].ToString());
                                    }
                                }
                                catch { }
                                if (periodo > mayor)
                                {
                                    if (string.IsNullOrEmpty(drS["PERIODO"].ToString()))
                                        ultimoPagare = "1";
                                    else
                                        ultimoPagare = drS["PERIODO"].ToString();
                                    fechaUltimoPagare = drS["VENCIMIENTO"].ToString();
                                    fechaUltimoPagare = fechaUltimoPagare.Substring(0, 10);
                                    mayor = periodo;
                                }
                            }
                        }
                        DataRow[] drSelect2 = dtDatos.Select("ID_CONTRATO = '" + drDatos["ID_CONTRATO"].ToString() + "' AND STATUS = 1", "VENCIMIENTO ASC");
                        if (drSelect2.Length > 0)
                        {
                            pagaresPorLiquidar = drSelect2.Length;
                            foreach (DataRow drS2 in drSelect2)
                            {
                                DateTime dateVenc = Convert.ToDateTime(drS2["VENCIMIENTO"].ToString());
                                if (dateVenc < DateTime.Now)
                                {
                                    vencidos++;
                                    if(!string.IsNullOrEmpty(drS2["TOTAL"].ToString()))
                                        importeVencido += Decimal.Round(Convert.ToDecimal(drS2["TOTAL"].ToString()), 2);
                                }                                
                                if (!string.IsNullOrEmpty(drS2["TOTAL"].ToString()))
                                    importePorCobrar += Decimal.Round(Convert.ToDecimal(drS2["TOTAL"].ToString()), 2);                               
                            }
                        }
                        //importePorCobrar = Decimal.Round(importePorCobrar, 2);
                        DateTime dateFirstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        DateTime dateLastDayOfMonth = dateFirstDayOfMonth.AddMonths(1).AddDays(-1);
                        //string select = "ID_CONTRATO = '" + drDatos["ID_CONTRATO"].ToString() + "' AND FECHA_PAGO >= '01/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "' AND FECHA_PAGO <= '31/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "'";
                        string select = "ID_CONTRATO = '" + drDatos["ID_CONTRATO"].ToString() + "' AND FECHA_PAGO >= '" + dateFirstDayOfMonth.ToString("dd/MM/yyyy") + "' AND FECHA_PAGO <= '" + dateLastDayOfMonth.ToString("dd/MM/yyyy") + "'";
                        DataRow[] drSelect3 = dtDatos.Select(select, "VENCIMIENTO ASC");
                        if (drSelect3.Length > 0)
                        {
                            foreach (DataRow drS3 in drSelect3)
                            {
                                if (!string.IsNullOrEmpty(drS3["TOTAL"].ToString()))
                                    pagosMes += Decimal.Round(Convert.ToDecimal(drS3["TOTAL"].ToString()), 2);
                            }
                        }

                        DataRow[] drSelect4 = dtDatos.Select("ID_CONTRATO = '" + drDatos["ID_CONTRATO"].ToString() + "'");
                        if(drSelect4.Length > 0)
                        {
                            foreach (DataRow drS4 in drSelect4)
                            {
                                if(!string.IsNullOrEmpty(drS4["OBSERVACIONES"].ToString()))
                                    observaciones += drS4["OBSERVACIONES"].ToString() + "| ";
                            }
                        }
                        if (vencidos > 0)
                        {
                            dtFinal.Rows.Add(contador.ToString(), manzana, lote, cliente, ultimoPagare, totalPagares, fechaUltimoPagare, vencidos.ToString(), importeMensual, importeVencido.ToString(), pagosMes.ToString(), pagaresPorLiquidar.ToString(), importePorCobrar.ToString(), observaciones);
                            contador++;
                        }

                        totalRegistros = contador;
                        try
                        {
                            totalSumaPagares += int.Parse(totalPagares);   
                        }
                        catch
                        {
                            totalSumaPagares += 0;
                        }
                        try
                        {
                            totalImporteMensual += Convert.ToDecimal(importeMensual);
                        }
                        catch
                        {
                            totalImporteMensual += 0;
                        }
                        totalVencidos += vencidos;
                        totalImporteVencido += importeVencido;
                        totalMes += pagosMes;
                        totalPorLiquidar += pagaresPorLiquidar;
                        totalImportePorCobrar += importePorCobrar;
                    }
                }
                totalRegistros--;
                if (dtFinal.Rows.Count > 0)
                    return dtFinal;
                else
                    return new DataTable();
            }
            catch //(Exception ex)
            {
                return new DataTable();
            }
        }*/

        private DataTable setComponentes(List<CarteraVencidaVentaEntity> datosCartera)
        {
            try
            {
                DataTable dtFinal = new DataTable();
                dtFinal.Columns.Add(new DataColumn("CONTADOR", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("MANZANA", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("LOTE", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("CLIENTE", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("ULTIMO_PAGARE", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("TOTAL_PAGARES", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("FECHA_ULTIMO_PAGARE", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("PAGARES_VENCIDOS", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("IMPORTE_MENSUAL", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("IMPORTE_VENCIDO", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("PAGO_MES", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("PAGARES_POR_LIQUIDAR", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("IMPORTE_POR_COBRAR", typeof(string)));
                dtFinal.Columns.Add(new DataColumn("OBSERVACIONES", typeof(string)));
                dtFinal.AcceptChanges();
                List<string> listaContratos = new List<string>();
                int contador = 1;

                int porcentaje = 40;
                decimal factor = 40 / datosCartera.Count;
                factor = factor >= 1 ? factor : 1;

                foreach (var cartera in datosCartera)
                {
                    if (porcentaje <= 80)
                        porcentaje += Convert.ToInt32(factor);
                    OnCambioProgreso(porcentaje <= 80 ? porcentaje : 80);
                    if (CancelacionPendiente)
                        return new DataTable();

                    if (!listaContratos.Contains(cartera.IDContrato))
                    {
                        listaContratos.Add(cartera.IDContrato);
                        string manzana = cartera.Manzana;
                        string lote = cartera.Lote;
                        string cliente = cartera.Cliente;
                        string ultimoPagare = string.Empty;
                        string totalPagares = "1";
                        if (!string.IsNullOrEmpty(cartera.TotalPagares))
                            totalPagares = cartera.TotalPagares;
                        string fechaUltimoPagare = string.Empty;
                        int vencidos = 0;
                        string importeMensual = cartera.ImporteMensual.ToString();
                        decimal importeVencido = 0;
                        decimal pagosMes = 0;
                        int pagaresPorLiquidar = 0;
                        decimal importePorCobrar = 0;
                        string observaciones = string.Empty;

                        var listaSelect = datosCartera.Where(d => d.IDContrato == cartera.IDContrato && d.EstatusFactura == "2").OrderByDescending(d => d.Periodo);
                        if (listaSelect.Count() > 0)
                        {
                            int mayor = 0;
                            int periodo = 0;
                            foreach (var carteraSelect in listaSelect)
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(carteraSelect.Periodo))
                                        periodo = 1;
                                    else
                                    {
                                        if (carteraSelect.Periodo.Contains("p"))
                                            periodo = int.Parse(carteraSelect.Periodo.Replace("p", string.Empty));
                                        else
                                            periodo = int.Parse(carteraSelect.Periodo);
                                    }
                                }
                                catch { }
                                if (periodo > mayor)
                                {
                                    if (string.IsNullOrEmpty(carteraSelect.Periodo))
                                        ultimoPagare = "1";
                                    else
                                        ultimoPagare = carteraSelect.Periodo;
                                    fechaUltimoPagare = carteraSelect.FechaVencimiento == null ? string.Empty : carteraSelect.FechaVencimiento.ToString();
                                    fechaUltimoPagare = fechaUltimoPagare.Substring(0, 10);
                                    mayor = periodo;
                                }
                            }
                        }
                        var listaSelect2 = datosCartera.Where(d => d.IDContrato == cartera.IDContrato && d.EstatusFactura == "1").OrderBy(d => d.FechaVencimiento);
                        if (listaSelect2.Count() > 0)
                        {
                            pagaresPorLiquidar = listaSelect2.Count();
                            foreach (var carteraSelect in listaSelect2)
                            {
                                DateTime dateVenc = carteraSelect.FechaVencimiento == null ? new DateTime() : (DateTime)carteraSelect.FechaVencimiento;
                                if (dateVenc < DateTime.Now)
                                {
                                    vencidos++;
                                    importeVencido += Decimal.Round(carteraSelect.Total, 2);
                                }
                                importePorCobrar += Decimal.Round(carteraSelect.Total, 2);
                            }
                        }
                        DateTime dateFirstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        DateTime dateLastDayOfMonth = dateFirstDayOfMonth.AddMonths(1).AddDays(-1);
                        var listaSelect3 = datosCartera.Where(d => d.IDContrato == cartera.IDContrato && d.FechaPagado >= dateFirstDayOfMonth && d.FechaPagado <= dateLastDayOfMonth).OrderBy(d=>d.FechaVencimiento);
                        if (listaSelect3.Count() > 0)
                        {
                            foreach (var carteraSelect in listaSelect3)
                            {
                                pagosMes += Decimal.Round(cartera.Total, 2);
                            }
                        }

                        var listaSelect4 = datosCartera.Where(d => d.IDContrato == cartera.IDContrato);
                        if (listaSelect4.Count() > 0)
                        {
                            foreach (var carteraSelect in listaSelect4)
                            {
                                if (!string.IsNullOrEmpty(carteraSelect.Observaciones))
                                    observaciones += carteraSelect.Observaciones + "| ";
                            }
                        }
                        if (vencidos > 0)
                        {
                            dtFinal.Rows.Add(contador.ToString(), manzana, lote, cliente, ultimoPagare, totalPagares, fechaUltimoPagare, vencidos.ToString(), importeMensual, importeVencido.ToString(), pagosMes.ToString(), pagaresPorLiquidar.ToString(), importePorCobrar.ToString(), observaciones);
                            contador++;
                        }

                        totalRegistros = contador;
                        try
                        {
                            totalSumaPagares += int.Parse(totalPagares);
                        }
                        catch
                        {
                            totalSumaPagares += 0;
                        }
                        try
                        {
                            totalImporteMensual += Convert.ToDecimal(importeMensual);
                        }
                        catch
                        {
                            totalImporteMensual += 0;
                        }
                        totalVencidos += vencidos;
                        totalImporteVencido += importeVencido;
                        totalMes += pagosMes;
                        totalPorLiquidar += pagaresPorLiquidar;
                        totalImportePorCobrar += importePorCobrar;
                    }
                }
                totalRegistros--;
                if (dtFinal.Rows.Count > 0)
                    return dtFinal;
                else
                    return new DataTable();
            }
            catch 
            {
                return new DataTable();
            }
        }
        
        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();
        }

        public static List<EstatusContratoEntity> obtenerEstatus()
        {
            return SaariDB.getEstatusContrato();
        }

        public static List<ConjuntoEntity> obtenerConjuntos(string idInmobiliaria)
        {
            return SaariDB.getConjuntos(idInmobiliaria);
        }
    }
}
