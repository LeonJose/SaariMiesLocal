using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.DataAccessLayer.ArrendadoraTableAdapters;
using FastReport;
using System.Windows.Forms;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class Reporte : SaariReport, IReport, IBackgroundReport
    {
        private string clave = string.Empty, rutaPlantilla = string.Empty, nombre = string.Empty, idInmobiliaria = string.Empty, idConjunto = string.Empty, idSubConjunto = string.Empty;
        private bool repIva = true, esPdf = true;
        private DateTime fechaIniCorte = new DateTime(1990,1,1);
        private DateTime fechaFin = new DateTime(1990, 1, 1);

        public Reporte(string clave, string rutaFormato, string nombreReporte, string idInmobiliaria, string idConjunto, string idSubconjunto, bool repIva, DateTime fechaInicio, DateTime fechaFin, bool esPdf)
        {
            this.clave = clave;
            this.rutaPlantilla = rutaFormato;
            this.nombre = nombreReporte;
            this.idInmobiliaria = idInmobiliaria;
            this.idConjunto = idConjunto;
            this.idSubConjunto = idSubconjunto;
            this.repIva = repIva;
            this.fechaIniCorte = fechaInicio;
            this.fechaFin = fechaFin;
            this.esPdf = esPdf;
        }

        /// <summary>
        /// Genera un reporte a partir de los atributos indicados para el objeto.
        /// <param name="formato">Formato de salida para reporte. "xls" para excel, empty o "" para formato de impresión.</param>
        /// <returns>Ruta física del archivo de reporte.</returns>
        /// </summary>
        public string generar()
        {
            string error = string.Empty;
            switch (this.clave)
            {
                case "REP1":
                    {   
                        error = generarReporte306090Mas90();
                    }
                    break;
                case "REP2":
                    {
                        error = generarReporteHistoricoUnidadNegocio();
                    }
                    break;
                case "REP3":
                    {
                        error = generarReporteHistoricoIngresosConsolidado();
                    }
                    break;
                case "REP4":
                   {
                     error = generarReporteSaldosClientes();
                   }
                   break;
                default:
                    return string.Empty;
            }            
            return error;
        }
        
        private string generarReporteHistoricoUnidadNegocio()
        {
            try
            {

                OnCambioProgreso(10);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                TfrxReportClass reporte = new TfrxReportClass();
                reporte.ClearReport();
                reporte.ClearDatasets();
                reporte.LoadReportFromFile(rutaPlantilla);

                FrxDataTable dtInmobiliaria = new FrxDataTable("dvInmobiliaria");
                DataColumn dcInmobiliaraRazonSocial = new DataColumn("razonSocial", typeof(string));
                DataColumn dcInmobiliaraNombreComercial = new DataColumn("nombreComercial", typeof(string));
                dtInmobiliaria.Columns.Add(dcInmobiliaraRazonSocial);
                dtInmobiliaria.Columns.Add(dcInmobiliaraNombreComercial);
                dtInmobiliaria.AcceptChanges();

                FrxDataTable dtConjunto = new FrxDataTable("dvConjunto");
                DataColumn dcNombreConjunto1 = new DataColumn("nombreCnj1", typeof(string));
                DataColumn dcNombreConjunto2 = new DataColumn("nombreCnj2", typeof(string));
                DataColumn dcNombreConjunto3 = new DataColumn("nombreCnj3", typeof(string));
                DataColumn dcNombreConjunto4 = new DataColumn("nombreCnj4", typeof(string));
                DataColumn dcNombreConjunto5 = new DataColumn("nombreCnj5", typeof(string));
                dtConjunto.Columns.Add(dcNombreConjunto1);
                dtConjunto.Columns.Add(dcNombreConjunto2);
                dtConjunto.Columns.Add(dcNombreConjunto3);
                dtConjunto.Columns.Add(dcNombreConjunto4);
                dtConjunto.Columns.Add(dcNombreConjunto5);
                dtConjunto.AcceptChanges();

                FrxDataTable dtIngreso = new FrxDataTable("dvIngreso");
                DataColumn dcIngresoNombreMesAnt = new DataColumn("nombreMesAnt", typeof(string));
                DataColumn dcIngresoMesAnteriorTotal = new DataColumn("totalIngresoMesAnt", typeof(string)); // linea agregada
                DataColumn dcIngresoMesAntConjunto1 = new DataColumn("ingresoMesAntCnj1", typeof(string));
                DataColumn dcIngresoMesAntConjunto2 = new DataColumn("ingresoMesAntCnj2", typeof(string));
                DataColumn dcIngresoMesAntConjunto3 = new DataColumn("ingresoMesAntCnj3", typeof(string));
                DataColumn dcIngresoMesAntConjunto4 = new DataColumn("ingresoMesAntCnj4", typeof(string));
                DataColumn dcIngresoMesAntConjunto5 = new DataColumn("ingresoMesAntCnj5", typeof(string));

                DataColumn dcIngresoNombreMesActual = new DataColumn("nombreMesActual", typeof(string));
                DataColumn dcIngresoMesActualTotal = new DataColumn("totalIngresoMes", typeof(string)); //nueva linea prueba
                DataColumn dcIngresoMesActualConjunto1 = new DataColumn("ingresoMesActualCnj1", typeof(string));
                DataColumn dcIngresoMesActualConjunto2 = new DataColumn("ingresoMesActualCnj2", typeof(string));
                DataColumn dcIngresoMesActualConjunto3 = new DataColumn("ingresoMesActualCnj3", typeof(string));
                DataColumn dcIngresoMesActualConjunto4 = new DataColumn("ingresoMesActualCnj4", typeof(string));
                DataColumn dcIngresoMesActualConjunto5 = new DataColumn("ingresoMesActualCnj5", typeof(string));

                dtIngreso.Columns.Add(dcIngresoNombreMesAnt);
                dtIngreso.Columns.Add(dcIngresoMesAnteriorTotal); // linea agregada
                dtIngreso.Columns.Add(dcIngresoMesAntConjunto1);
                dtIngreso.Columns.Add(dcIngresoMesAntConjunto2);
                dtIngreso.Columns.Add(dcIngresoMesAntConjunto3);
                dtIngreso.Columns.Add(dcIngresoMesAntConjunto4);
                dtIngreso.Columns.Add(dcIngresoMesAntConjunto5);

                dtIngreso.Columns.Add(dcIngresoNombreMesActual);
                dtIngreso.Columns.Add(dcIngresoMesActualTotal); // linea agregada
                dtIngreso.Columns.Add(dcIngresoMesActualConjunto1);
                dtIngreso.Columns.Add(dcIngresoMesActualConjunto2);
                dtIngreso.Columns.Add(dcIngresoMesActualConjunto3);
                dtIngreso.Columns.Add(dcIngresoMesActualConjunto4);
                dtIngreso.Columns.Add(dcIngresoMesActualConjunto5);
                dtIngreso.AcceptChanges();


                FrxDataTable dtTotal = new FrxDataTable("dvTotal");
                DataColumn dcTotalTotalAnt = new DataColumn("totalTotalAnt", typeof(string));// linea agregada
                DataColumn dcTotalAntConjunto1 = new DataColumn("totalAntCnj1", typeof(string));
                DataColumn dcTotalAntConjunto2 = new DataColumn("totalAntCnj2", typeof(string));
                DataColumn dcTotalAntConjunto3 = new DataColumn("totalAntCnj3", typeof(string));
                DataColumn dcTotalAntConjunto4 = new DataColumn("totalAntCnj4", typeof(string));
                DataColumn dcTotalAntConjunto5 = new DataColumn("totalAntCnj5", typeof(string));

                DataColumn dcTotalTotalActual = new DataColumn("totalTotalActual", typeof(string)); // linea agregada
                DataColumn dcTotalActualConjunto1 = new DataColumn("totalActualCnj1", typeof(string));
                DataColumn dcTotalActualConjunto2 = new DataColumn("totalActualCnj2", typeof(string));
                DataColumn dcTotalActualConjunto3 = new DataColumn("totalActualCnj3", typeof(string));
                DataColumn dcTotalActualConjunto4 = new DataColumn("totalActualCnj4", typeof(string));
                DataColumn dcTotalActualConjunto5 = new DataColumn("totalActualCnj5", typeof(string));

                dtTotal.Columns.Add(dcTotalTotalAnt); // linea agregada
                dtTotal.Columns.Add(dcTotalAntConjunto1);
                dtTotal.Columns.Add(dcTotalAntConjunto2);
                dtTotal.Columns.Add(dcTotalAntConjunto3);
                dtTotal.Columns.Add(dcTotalAntConjunto4);
                dtTotal.Columns.Add(dcTotalAntConjunto5);

                dtTotal.Columns.Add(dcTotalTotalActual); // linea agregada
                dtTotal.Columns.Add(dcTotalActualConjunto1);
                dtTotal.Columns.Add(dcTotalActualConjunto2);
                dtTotal.Columns.Add(dcTotalActualConjunto3);
                dtTotal.Columns.Add(dcTotalActualConjunto4);
                dtTotal.Columns.Add(dcTotalActualConjunto5);
                dtTotal.AcceptChanges();

                OnCambioProgreso(20);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                T01_ARRENDADORATableAdapter taArrendadora = new T01_ARRENDADORATableAdapter();
                taArrendadora.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;
                DataRow drInmobiliaria = taArrendadora.GetArrendadoraByIdArr(this.idInmobiliaria).Rows[0];
                dtInmobiliaria.Rows.Add(drInmobiliaria["P0103_RAZON_SOCIAL"].ToString().Trim(), drInmobiliaria["P0102_N_COMERCIAL"].ToString().Trim());

                IfrxComponent dvFecha = reporte.FindObject("dvFecha");
                (dvFecha as IfrxCustomMemoView).Text = "Al " + this.fechaIniCorte.Day.ToString("00") + " de " + getNombreMes(this.fechaIniCorte.Month) + " del " + this.fechaIniCorte.Year;

                IfrxComponent memoAnoAnt = reporte.FindObject("memoAnoAnterior");
                (memoAnoAnt as IfrxCustomMemoView).Text = "Acumulado " + this.fechaIniCorte.AddYears(-1).Year;

                IfrxComponent memoAnoActual = reporte.FindObject("memoAnoActual");
                (memoAnoActual as IfrxCustomMemoView).Text = "Acumulado " + this.fechaIniCorte.Year;

                T03_CENTRO_INDUSTRIALTableAdapter taConjunto = new T03_CENTRO_INDUSTRIALTableAdapter();
                taConjunto.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;
                DataTable dtConjuntos = taConjunto.GetConjuntosByIdArr(this.idInmobiliaria);

                int cantidadConjuntos = dtConjuntos.Rows.Count > 5 ? 5 : dtConjuntos.Rows.Count;

                for (int i = cantidadConjuntos; i < 5; i++)
                {
                    dtConjuntos.Rows.Add("IdTMP" + i, "");
                }
                
                dtConjunto.Rows.Add(dtConjuntos.Rows[0]["P0303_NOMBRE"].ToString().Trim(), dtConjuntos.Rows[1]["P0303_NOMBRE"].ToString().Trim(),
                    dtConjuntos.Rows[2]["P0303_NOMBRE"].ToString().Trim(), dtConjuntos.Rows[3]["P0303_NOMBRE"].ToString().Trim(), dtConjuntos.Rows[4]["P0303_NOMBRE"].ToString().Trim());

                int anoAnterior = this.fechaIniCorte.AddYears(-1).Year;
                int anoActual = this.fechaIniCorte.Year;

                OnCambioProgreso(30);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                double[,] matrizIngresosAnteriores = new double[12, 5];
                double[] totalActualMes = new double[12]; // 2 lineas agregadas
                double[] totalAnteriorMes = new double[12];

                for (int i = 0; i < 12; i++)
                {
                    for (int j = 0; j < 5; j++)
                        matrizIngresosAnteriores[i, j] = 0;
                }

                double[,] matrizIngresosActuales = new double[12, 5];

                for (int i = 0; i < 12; i++)
                {
                    for (int j = 0; j < 5; j++)
                        matrizIngresosActuales[i, j] = 0;
                    totalActualMes[i] = 0; // 2 lineas agregadas
                    totalAnteriorMes[i] = 0;
                }

                T24_HISTORIA_RECIBOSTableAdapter taRecibos = new T24_HISTORIA_RECIBOSTableAdapter();
                taRecibos.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;
                DataTable dtRecibos = taRecibos.GetAllRecibosByIdArr(this.idInmobiliaria);

                /*Calculo de ingresos de año anterior*/
                for (int i = 0; i < cantidadConjuntos; i++)
                {
                    double[] ingresosAnoAnterior = new double[12];
                    for (int j = 0; j < 12; j++)
                    {
                        int mes = j + 1;
                        DateTime primerDiaMes = new DateTime(anoAnterior, mes, 1);
                        DateTime ultimoDiaMes = new DateTime(anoAnterior, mes, DateTime.DaysInMonth(anoAnterior, mes));
                        string fechaPrimerDiaMesIB = primerDiaMes.Day.ToString("00") + "." + primerDiaMes.Month.ToString("00") + "." + primerDiaMes.Year.ToString("0000");
                        string fechaUltimoDiaMesIB = ultimoDiaMes.Day.ToString("00") + "." + ultimoDiaMes.Month.ToString("00") + "." + ultimoDiaMes.Year.ToString("0000");

                        double ingresoMes = 0;
                        double egresoMes = 0;

                        try
                        {
                            DataRow[] drRecibosIngreso = dtRecibos.Select(" ( P2426_TIPO_DOC = 'R' OR P2426_TIPO_DOC = 'P' )AND  CAMPO4='" + dtConjuntos.Rows[i]["P0301_ID_CENTRO"].ToString().Trim() + "' AND (P2406_STATUS = 2) AND  P2409_FECHA_EMISION >='" + fechaPrimerDiaMesIB + "' AND P2409_FECHA_EMISION <='" + fechaUltimoDiaMesIB + "'");

                            foreach (DataRow drIngreso in drRecibosIngreso)
                            {
                                if (drIngreso["P2405_IMPORTE"] != null && drIngreso["P2405_IMPORTE"].ToString().Trim().Length > 0)
                                {
                                    if (drIngreso["P2410_MONEDA"].ToString().Trim() == "D")
                                        ingresoMes = ingresoMes + (Convert.ToDouble(drIngreso["P2405_IMPORTE"]) * Convert.ToDouble(drIngreso["P2414_TIPO_CAMBIO"]));
                                    else
                                        ingresoMes = ingresoMes + Convert.ToDouble(drIngreso["P2405_IMPORTE"]);

                                }
                            }
                        }
                        catch
                        {
                            ingresoMes = 0;
                        }

                        try
                        {
                            DataRow[] drRecibosEgreso = dtRecibos.Select("P2406_STATUS <> 3 AND  CAMPO4='" + dtConjuntos.Rows[i]["P0301_ID_CENTRO"].ToString().Trim() + "' AND  P2409_FECHA_EMISION >='" + fechaPrimerDiaMesIB + "'  AND P2409_FECHA_EMISION <='" + fechaUltimoDiaMesIB + "' AND P2426_TIPO_DOC = 'B'");

                            foreach (DataRow drEgreso in drRecibosEgreso)
                            {
                                if (drEgreso["P2405_IMPORTE"] != null && drEgreso["P2405_IMPORTE"].ToString().Trim().Length > 0)
                                {
                                    if (drEgreso["P2410_MONEDA"].ToString().Trim() == "D")
                                        egresoMes = egresoMes + (Convert.ToDouble(drEgreso["P2405_IMPORTE"]) * Convert.ToDouble(drEgreso["P2414_TIPO_CAMBIO"]));
                                    else
                                        egresoMes = egresoMes + Convert.ToDouble(drEgreso["P2405_IMPORTE"]);


                                }
                            }
                        }

                        catch
                        {
                            egresoMes = 0;
                        }

                        matrizIngresosAnteriores[j, i] = ingresoMes - egresoMes;
                        totalAnteriorMes[j] += (ingresoMes - egresoMes); //linea agregada
                    }
                }

                OnCambioProgreso(40);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                /*Calculo de ingresos de año actual*/
                for (int i = 0; i < cantidadConjuntos; i++)
                {
                    for (int j = 0; j < this.fechaIniCorte.Month; j++)
                    {
                        int mes = j + 1;
                        DateTime primerDiaMes = new DateTime(anoActual, mes, 1);
                        DateTime ultimoDiaMes = new DateTime(anoActual, mes, DateTime.DaysInMonth(anoActual, mes));
                        string fechaPrimerDiaMesIB = primerDiaMes.Day.ToString("00") + "." + primerDiaMes.Month.ToString("00") + "." + primerDiaMes.Year.ToString("0000");
                        string fechaUltimoDiaMesIB = ultimoDiaMes.Day.ToString("00") + "." + ultimoDiaMes.Month.ToString("00") + "." + ultimoDiaMes.Year.ToString("0000");

                        double ingresoMes = 0;
                        double egresoMes = 0;

                        try
                        {
                            //P2426_TIPO_DOC = 'R' OR P2426_TIPO_DOC = 'P' ) AND (P2406_STATUS = 2) AND  P2409_FECHA_EMISION >='" + fechaPrimerDiaMesIB + "' AND P2409_FECHA_EMISION <='" + fechaUltimoDiaMesIB + "'"
                            DataRow[] drRecibosIngreso = dtRecibos.Select(" ( P2426_TIPO_DOC = 'R' OR P2426_TIPO_DOC = 'P' )AND  CAMPO4='" + dtConjuntos.Rows[i]["P0301_ID_CENTRO"].ToString().Trim() + "' AND (P2406_STATUS = 2) AND  P2409_FECHA_EMISION >='" + fechaPrimerDiaMesIB + "' AND P2409_FECHA_EMISION <='" + fechaUltimoDiaMesIB + "'");

                            foreach (DataRow drIngreso in drRecibosIngreso)
                            {
                                if (drIngreso["P2405_IMPORTE"] != null && drIngreso["P2405_IMPORTE"].ToString().Trim().Length > 0)
                                {
                                    if (drIngreso["P2410_MONEDA"].ToString().Trim() == "D")
                                        ingresoMes = ingresoMes + (Convert.ToDouble(drIngreso["P2405_IMPORTE"]) * Convert.ToDouble(drIngreso["P2414_TIPO_CAMBIO"]));
                                    else
                                        ingresoMes = ingresoMes + Convert.ToDouble(drIngreso["P2405_IMPORTE"]);

                                }
                            }
                        }
                        catch
                        {
                            ingresoMes = 0;
                        }

                        try
                        {
                            DataRow[] drRecibosEgreso = dtRecibos.Select("P2406_STATUS <> 3 AND  CAMPO4='" + dtConjuntos.Rows[i]["P0301_ID_CENTRO"].ToString().Trim() + "' AND  P2409_FECHA_EMISION >='" + fechaPrimerDiaMesIB + "'  AND P2409_FECHA_EMISION <='" + fechaUltimoDiaMesIB + "' AND P2426_TIPO_DOC = 'B'");

                            foreach (DataRow drEgreso in drRecibosEgreso)
                            {
                                if (drEgreso["P2405_IMPORTE"] != null && drEgreso["P2405_IMPORTE"].ToString().Trim().Length > 0)
                                {
                                    if (drEgreso["P2410_MONEDA"].ToString().Trim() == "D")
                                        egresoMes = egresoMes + (Convert.ToDouble(drEgreso["P2405_IMPORTE"]) * Convert.ToDouble(drEgreso["P2414_TIPO_CAMBIO"]));
                                    else
                                        egresoMes = egresoMes + Convert.ToDouble(drEgreso["P2405_IMPORTE"]);


                                }
                            }
                        }
                        catch
                        {
                            egresoMes = 0;
                        }
                        matrizIngresosActuales[j, i] = ingresoMes - egresoMes;
                        totalActualMes[j] += (ingresoMes - egresoMes); //linea agregada

                    }
                }

                OnCambioProgreso(50);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                dtIngreso.Rows.Add(
                    "Enero", totalAnteriorMes[0],
                    matrizIngresosAnteriores[0, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[0, 0]) : "0", matrizIngresosAnteriores[0, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[0, 1]) : "0",
                    matrizIngresosAnteriores[0, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[0, 2]) : "0", matrizIngresosAnteriores[0, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[0, 3]) : "0",
                    matrizIngresosAnteriores[0, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[0, 4]) : "0",
                    "Enero", totalActualMes[0],
                    matrizIngresosActuales[0, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[0, 0]) : "0", matrizIngresosActuales[0, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[0, 1]) : "0",
                    matrizIngresosActuales[0, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[0, 2]) : "0", matrizIngresosActuales[0, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[0, 3]) : "0",
                    matrizIngresosActuales[0, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[0, 4]) : "0");

                dtIngreso.Rows.Add(
                    "Febrero", totalAnteriorMes[1],
                    matrizIngresosAnteriores[1, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[1, 0]) : "0", matrizIngresosAnteriores[1, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[1, 1]) : "0",
                    matrizIngresosAnteriores[1, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[1, 2]) : "0", matrizIngresosAnteriores[1, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[1, 3]) : "0",
                    matrizIngresosAnteriores[1, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[1, 4]) : "0",
                    "Febrero", totalActualMes[1],
                    matrizIngresosActuales[1, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[1, 0]) : "0", matrizIngresosActuales[1, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[1, 1]) : "0",
                    matrizIngresosActuales[1, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[1, 2]) : "0", matrizIngresosActuales[1, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[1, 3]) : "0",
                    matrizIngresosActuales[1, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[1, 4]) : "0");

                dtIngreso.Rows.Add(
                    "Marzo", totalAnteriorMes[2],
                    matrizIngresosAnteriores[2, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[2, 0]) : "0", matrizIngresosAnteriores[2, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[2, 1]) : "0",
                    matrizIngresosAnteriores[2, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[2, 2]) : "0", matrizIngresosAnteriores[2, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[2, 3]) : "0",
                    matrizIngresosAnteriores[2, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[2, 4]) : "0",
                    "Marzo", totalActualMes[2],
                    matrizIngresosActuales[2, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[2, 0]) : "0", matrizIngresosActuales[2, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[2, 1]) : "0",
                    matrizIngresosActuales[2, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[2, 2]) : "0", matrizIngresosActuales[2, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[2, 3]) : "0",
                    matrizIngresosActuales[2, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[2, 4]) : "0");

                dtIngreso.Rows.Add(
                    "Abril", totalAnteriorMes[3],
                    matrizIngresosAnteriores[3, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[3, 0]) : "0", matrizIngresosAnteriores[3, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[3, 1]) : "0",
                    matrizIngresosAnteriores[3, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[3, 2]) : "0", matrizIngresosAnteriores[3, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[3, 3]) : "0",
                    matrizIngresosAnteriores[3, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[3, 4]) : "0",
                    "Abril", totalActualMes[3],
                    matrizIngresosActuales[3, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[3, 0]) : "0", matrizIngresosActuales[3, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[3, 1]) : "0",
                    matrizIngresosActuales[3, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[3, 2]) : "0", matrizIngresosActuales[3, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[3, 3]) : "0",
                    matrizIngresosActuales[3, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[3, 4]) : "0");

                dtIngreso.Rows.Add(
                    "Mayo", totalAnteriorMes[4],
                    matrizIngresosAnteriores[4, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[4, 0]) : "0", matrizIngresosAnteriores[4, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[4, 1]) : "0",
                    matrizIngresosAnteriores[4, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[4, 2]) : "0", matrizIngresosAnteriores[4, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[4, 3]) : "0",
                    matrizIngresosAnteriores[4, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[4, 4]) : "0",
                    "Mayo", totalActualMes[4],
                    matrizIngresosActuales[4, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[4, 0]) : "0", matrizIngresosActuales[4, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[4, 1]) : "0",
                    matrizIngresosActuales[4, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[4, 2]) : "0", matrizIngresosActuales[4, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[4, 3]) : "0",
                    matrizIngresosActuales[4, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[4, 4]) : "0");

                dtIngreso.Rows.Add(
                    "Junio", totalAnteriorMes[5],
                    matrizIngresosAnteriores[5, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[5, 0]) : "0", matrizIngresosAnteriores[5, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[5, 1]) : "0",
                    matrizIngresosAnteriores[5, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[5, 2]) : "0", matrizIngresosAnteriores[5, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[5, 3]) : "0",
                    matrizIngresosAnteriores[5, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[5, 4]) : "0",
                    "Junio", totalActualMes[5],
                    matrizIngresosActuales[5, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[5, 0]) : "0", matrizIngresosActuales[5, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[5, 1]) : "0",
                    matrizIngresosActuales[5, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[5, 2]) : "0", matrizIngresosActuales[5, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[5, 3]) : "0",
                    matrizIngresosActuales[5, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[5, 4]) : "0");


                dtIngreso.Rows.Add(
                    "Julio", totalAnteriorMes[6],
                    matrizIngresosAnteriores[6, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[6, 0]) : "0", matrizIngresosAnteriores[6, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[6, 1]) : "0",
                    matrizIngresosAnteriores[6, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[6, 2]) : "0", matrizIngresosAnteriores[6, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[6, 3]) : "0",
                    matrizIngresosAnteriores[6, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[6, 4]) : "0",
                    "Julio", totalActualMes[6],
                    matrizIngresosActuales[6, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[6, 0]) : "0", matrizIngresosActuales[6, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[6, 1]) : "0",
                    matrizIngresosActuales[6, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[6, 2]) : "0", matrizIngresosActuales[6, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[6, 3]) : "0",
                    matrizIngresosActuales[6, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[6, 4]) : "0");

                dtIngreso.Rows.Add(
                    "Agosto", totalAnteriorMes[7],
                    matrizIngresosAnteriores[7, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[7, 0]) : "0", matrizIngresosAnteriores[7, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[7, 1]) : "0",
                    matrizIngresosAnteriores[7, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[7, 2]) : "0", matrizIngresosAnteriores[7, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[7, 3]) : "0",
                    matrizIngresosAnteriores[7, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[7, 4]) : "0",
                    "Agosto", totalActualMes[7],
                    matrizIngresosActuales[7, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[7, 0]) : "0", matrizIngresosActuales[7, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[7, 1]) : "0",
                    matrizIngresosActuales[7, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[7, 2]) : "0", matrizIngresosActuales[7, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[7, 3]) : "0",
                    matrizIngresosActuales[7, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[7, 4]) : "0");

                dtIngreso.Rows.Add(
                    "Septiembre", totalAnteriorMes[8],
                    matrizIngresosAnteriores[8, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[8, 0]) : "0", matrizIngresosAnteriores[8, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[8, 1]) : "0",
                    matrizIngresosAnteriores[8, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[8, 2]) : "0", matrizIngresosAnteriores[8, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[8, 3]) : "0",
                    matrizIngresosAnteriores[8, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[8, 4]) : "0",
                    "Septiembre", totalActualMes[8],
                    matrizIngresosActuales[8, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[8, 0]) : "0", matrizIngresosActuales[8, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[8, 1]) : "0",
                    matrizIngresosActuales[8, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[8, 2]) : "0", matrizIngresosActuales[8, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[8, 3]) : "0",
                    matrizIngresosActuales[8, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[8, 4]) : "0");

                dtIngreso.Rows.Add(
                    "Octubre", totalAnteriorMes[9],
                    matrizIngresosAnteriores[9, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[9, 0]) : "0", matrizIngresosAnteriores[9, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[9, 1]) : "0",
                    matrizIngresosAnteriores[9, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[9, 2]) : "0", matrizIngresosAnteriores[9, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[9, 3]) : "0",
                    matrizIngresosAnteriores[9, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[9, 4]) : "0",
                    "Octubre", totalActualMes[9],
                    matrizIngresosActuales[9, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[9, 0]) : "0", matrizIngresosActuales[9, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[9, 1]) : "0",
                    matrizIngresosActuales[9, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[9, 2]) : "0", matrizIngresosActuales[9, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[9, 3]) : "0",
                    matrizIngresosActuales[9, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[9, 4]) : "0");

                dtIngreso.Rows.Add(
                    "Noviembre", totalAnteriorMes[10],
                    matrizIngresosAnteriores[10, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[10, 0]) : "0", matrizIngresosAnteriores[10, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[10, 1]) : "0",
                    matrizIngresosAnteriores[10, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[10, 2]) : "0", matrizIngresosAnteriores[10, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[10, 3]) : "0",
                    matrizIngresosAnteriores[10, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[10, 4]) : "0",
                    "Noviembre", totalActualMes[10],
                    matrizIngresosActuales[10, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[10, 0]) : "0", matrizIngresosActuales[10, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[10, 1]) : "0",
                    matrizIngresosActuales[10, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[10, 2]) : "0", matrizIngresosActuales[10, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[10, 3]) : "0",
                    matrizIngresosActuales[10, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[10, 4]) : "0");

                dtIngreso.Rows.Add(
                    "Diciembre", totalAnteriorMes[11],
                    matrizIngresosAnteriores[11, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[11, 0]) : "0", matrizIngresosAnteriores[11, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[11, 1]) : "0",
                    matrizIngresosAnteriores[11, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[11, 2]) : "0", matrizIngresosAnteriores[11, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[11, 3]) : "0",
                    matrizIngresosAnteriores[11, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosAnteriores[11, 4]) : "0",
                    "Diciembre", totalActualMes[11],
                    matrizIngresosActuales[11, 0] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[11, 0]) : "0", matrizIngresosActuales[11, 1] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[11, 1]) : "0",
                    matrizIngresosActuales[11, 2] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[11, 2]) : "0", matrizIngresosActuales[11, 3] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[11, 3]) : "0",
                    matrizIngresosActuales[11, 4] != 0 ? String.Format("{0:#,0.00}", matrizIngresosActuales[11, 4]) : "0");


                OnCambioProgreso(60);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                double[] totalIngresosAnt = new double[5];
                double totalTotal = new double(); // 4 lineas agregadas
                double totalTotalAnterior = new double();
                totalTotal = 0;
                totalTotalAnterior = 0;
                for (int x = 0; x < 12; x++) // ciclo agregado
                {
                    totalTotal += totalActualMes[x];
                    totalTotalAnterior += totalAnteriorMes[x];
                }
                for (int i = 0; i < 5; i++)
                    totalIngresosAnt[i] = 0;

                for (int i = 0; i < cantidadConjuntos; i++)
                {
                    for (int j = 0; j < 12; j++)
                        totalIngresosAnt[i] = totalIngresosAnt[i] + matrizIngresosAnteriores[j, i];
                }

                double[] totalIngresosActuales = new double[5];
                for (int i = 0; i < 5; i++)
                    totalIngresosActuales[i] = 0;

                for (int i = 0; i < cantidadConjuntos; i++)
                {
                    for (int j = 0; j < 12; j++)
                        totalIngresosActuales[i] = totalIngresosActuales[i] + matrizIngresosActuales[j, i];
                }

                OnCambioProgreso(70);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                dtTotal.Rows.Add(
                    totalTotalAnterior != 0 ? String.Format("{0:#,0.00}", totalTotalAnterior) : "0", // linea agregada
                    totalIngresosAnt[0] != 0 ? String.Format("{0:#,0.00}", totalIngresosAnt[0]) : "0",
                    totalIngresosAnt[1] != 0 ? String.Format("{0:#,0.00}", totalIngresosAnt[1]) : "0",
                    totalIngresosAnt[2] != 0 ? String.Format("{0:#,0.00}", totalIngresosAnt[2]) : "0",
                    totalIngresosAnt[3] != 0 ? String.Format("{0:#,0.00}", totalIngresosAnt[3]) : "0",
                    totalIngresosAnt[4] != 0 ? String.Format("{0:#,0.00}", totalIngresosAnt[4]) : "0",
                    totalTotal != 0 ? String.Format("{0:#,0.00}", totalTotal) : "0", // linea agregada
                    totalIngresosActuales[0] != 0 ? String.Format("{0:#,0.00}", totalIngresosActuales[0]) : "0",
                    totalIngresosActuales[1] != 0 ? String.Format("{0:#,0.00}", totalIngresosActuales[1]) : "0",
                    totalIngresosActuales[2] != 0 ? String.Format("{0:#,0.00}", totalIngresosActuales[2]) : "0",
                    totalIngresosActuales[3] != 0 ? String.Format("{0:#,0.00}", totalIngresosActuales[3]) : "0",
                    totalIngresosActuales[4] != 0 ? String.Format("{0:#,0.00}", totalIngresosActuales[4]) : "0");


                FrxDataView dvInmobiliaria = new FrxDataView(dtInmobiliaria, "dvInmobiliaria");
                FrxDataView dvConjunto = new FrxDataView(dtConjunto, "dvConjunto");
                FrxDataView dvIngreso = new FrxDataView(dtIngreso, "dvIngreso");
                FrxDataView dvTotal = new FrxDataView(dtIngreso, "dvTotal");

                dvInmobiliaria.AssignToReport(true, reporte);
                dvConjunto.AssignToReport(true, reporte);
                dvIngreso.AssignToReport(true, reporte);
                dvTotal.AssignToReport(true, reporte);

                OnCambioProgreso(80);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                dtInmobiliaria.AssignToDataBand("MasterData1", reporte); //Parche para que aparezca encabezado de reporte
                dtIngreso.AssignToDataBand("MasterData1", reporte);

                OnCambioProgreso(90);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                return exportar(reporte, esPdf, "HistoricoIngresos");

                /*reporte.PrepareReport(true);

                if (esPdf)
                {
                    reporte.ShowPreparedReport();
                }
                else
                {
                    try
                    {
                        string directoryTmpReportesSaari = @"C:\Users\Public\Documents\SaariDB\TmpReportes";
                        if (!Directory.Exists(directoryTmpReportesSaari))
                            Directory.CreateDirectory(directoryTmpReportesSaari);

                        string archivoXlsTemporal = directoryTmpReportesSaari + "\\Reporte" + DateTime.Now.Ticks + ".xls";
                        reporte.ExportToXLS(archivoXlsTemporal, true, false, true, false, false, true);
                        System.Diagnostics.Process.Start(archivoXlsTemporal);
                    }
                    catch { }
                }*/
            }
            catch (Exception ex)
            {
                return "Error general al generar reporte: " + Environment.NewLine + ex.Message; 
            }
        }

        private string generarReporteHistoricoIngresosConsolidado()
        {

            try
            {
                OnCambioProgreso(10);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                TfrxReportClass reporte = new TfrxReportClass();
                reporte.ClearReport();
                reporte.ClearDatasets();
                reporte.LoadReportFromFile(rutaPlantilla);

                FrxDataTable dtInmobiliaria = new FrxDataTable("dvInmobiliaria");
                DataColumn dcInmobiliaraRazonSocial = new DataColumn("razonSocial", typeof(string));
                DataColumn dcInmobiliaraNombreComercial = new DataColumn("nombreComercial", typeof(string));
                dtInmobiliaria.Columns.Add(dcInmobiliaraRazonSocial);
                dtInmobiliaria.Columns.Add(dcInmobiliaraNombreComercial);
                dtInmobiliaria.AcceptChanges();

                FrxDataTable dtIngreso = new FrxDataTable("dvIngreso");
                DataColumn dcIngresoMesAnt = new DataColumn("ingresoMesAnt", typeof(string));
                DataColumn dcIngresoNombreMesAnt = new DataColumn("nombreMesAnt", typeof(string));
                DataColumn dcIngresoMesActual = new DataColumn("ingresoMesActual", typeof(string));
                DataColumn dcIngresoNombreMesActual = new DataColumn("nombreMesActual", typeof(string));
                dtIngreso.Columns.Add(dcIngresoMesAnt);
                dtIngreso.Columns.Add(dcIngresoNombreMesAnt);
                dtIngreso.Columns.Add(dcIngresoMesActual);
                dtIngreso.Columns.Add(dcIngresoNombreMesActual);
                dtIngreso.AcceptChanges();

                OnCambioProgreso(20);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                T01_ARRENDADORATableAdapter taArrendadora = new T01_ARRENDADORATableAdapter();
                taArrendadora.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;
                DataRow drInmobiliaria = taArrendadora.GetArrendadoraByIdArr(this.idInmobiliaria).Rows[0];
                dtInmobiliaria.Rows.Add(drInmobiliaria["P0103_RAZON_SOCIAL"].ToString().Trim(), drInmobiliaria["P0102_N_COMERCIAL"].ToString().Trim());

                IfrxComponent dvFecha = reporte.FindObject("dvFecha");
                (dvFecha as IfrxCustomMemoView).Text = "Al " + this.fechaIniCorte.Day.ToString("00") + " de " + getNombreMes(this.fechaIniCorte.Month) + " del " + this.fechaIniCorte.Year;

                IfrxComponent memoAnoAnt = reporte.FindObject("memoAnoAnterior");
                (memoAnoAnt as IfrxCustomMemoView).Text = this.fechaIniCorte.AddYears(-1).Year.ToString();

                IfrxComponent memoAnoActual = reporte.FindObject("memoAnoActual");
                (memoAnoActual as IfrxCustomMemoView).Text = this.fechaIniCorte.Year.ToString();

                OnCambioProgreso(30);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                T24_HISTORIA_RECIBOSTableAdapter taRecibos = new T24_HISTORIA_RECIBOSTableAdapter();
                taRecibos.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;
                DataTable dtRecibos = taRecibos.GetAllRecibosByIdArr(this.idInmobiliaria);

                //Ingresos del año anterior
                int anoAnterior = this.fechaIniCorte.AddYears(-1).Year;

                OnCambioProgreso(40);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                double[] ingresosAnoAnterior = new double[12];
                for (int j = 0; j < 12; j++)
                {
                    int mes = j + 1;
                    DateTime primerDiaMes = new DateTime(anoAnterior, mes, 1);
                    DateTime ultimoDiaMes = new DateTime(anoAnterior, mes, DateTime.DaysInMonth(anoAnterior, mes));
                    string fechaPrimerDiaMesIB = primerDiaMes.Day.ToString("00") + "." + primerDiaMes.Month.ToString("00") + "." + primerDiaMes.Year.ToString("0000");
                    string fechaUltimoDiaMesIB = ultimoDiaMes.Day.ToString("00") + "." + ultimoDiaMes.Month.ToString("00") + "." + ultimoDiaMes.Year.ToString("0000");

                    double ingresoMes = 0;
                    double egresoMes = 0;

                    try
                    {
                        DataRow[] drRecibosIngreso = dtRecibos.Select(" ( P2426_TIPO_DOC = 'R' OR P2426_TIPO_DOC = 'P' ) AND (P2406_STATUS = 2) AND  P2409_FECHA_EMISION >='" + fechaPrimerDiaMesIB + "' AND P2409_FECHA_EMISION <='" + fechaUltimoDiaMesIB + "'");

                        foreach (DataRow drIngreso in drRecibosIngreso)
                        {
                            if (drIngreso["P2405_IMPORTE"] != null && drIngreso["P2405_IMPORTE"].ToString().Trim().Length > 0)
                            {
                                if (drIngreso["P2410_MONEDA"].ToString().Trim() == "D")
                                    ingresoMes = ingresoMes + (Convert.ToDouble(drIngreso["P2405_IMPORTE"]) * Convert.ToDouble(drIngreso["P2414_TIPO_CAMBIO"]));
                                else
                                    ingresoMes = ingresoMes + Convert.ToDouble(drIngreso["P2405_IMPORTE"]);

                            }
                        }
                    }
                    catch
                    {
                        ingresoMes = 0;
                    }

                    try
                    {
                        DataRow[] drRecibosEgreso = dtRecibos.Select("P2406_STATUS <> 3 AND P2409_FECHA_EMISION >='" + fechaPrimerDiaMesIB + "'  AND P2409_FECHA_EMISION <='" + fechaUltimoDiaMesIB + "' AND P2426_TIPO_DOC = 'B'");

                        foreach (DataRow drEgreso in drRecibosEgreso)
                        {
                            if (drEgreso["P2405_IMPORTE"] != null && drEgreso["P2405_IMPORTE"].ToString().Trim().Length > 0)
                            {
                                if (drEgreso["P2410_MONEDA"].ToString().Trim() == "D")
                                    egresoMes = egresoMes + (Convert.ToDouble(drEgreso["P2405_IMPORTE"]) * Convert.ToDouble(drEgreso["P2414_TIPO_CAMBIO"]));
                                else
                                    egresoMes = egresoMes + Convert.ToDouble(drEgreso["P2405_IMPORTE"]);


                            }
                        }
                    }
                    catch
                    {
                        egresoMes = 0;
                    }

                    ingresosAnoAnterior[j] = ingresoMes - egresoMes;

                }

                OnCambioProgreso(50);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                //Ingresos del año actual
                int anoActual = this.fechaIniCorte.Year;

                double[] ingresosAnoActual = new double[12];

                for (int i = 0; i < 12; i++)
                    ingresosAnoActual[i] = 0;

                for (int j = 0; j < this.fechaIniCorte.Month; j++)
                {
                    int mes = j + 1;
                    DateTime primerDiaMes = new DateTime(anoActual, mes, 1);
                    DateTime ultimoDiaMes = new DateTime(anoActual, mes, DateTime.DaysInMonth(anoActual, mes));
                    string fechaPrimerDiaMesIB = primerDiaMes.Day.ToString("00") + "." + primerDiaMes.Month.ToString("00") + "." + primerDiaMes.Year.ToString("0000");
                    string fechaUltimoDiaMesIB = ultimoDiaMes.Day.ToString("00") + "." + ultimoDiaMes.Month.ToString("00") + "." + ultimoDiaMes.Year.ToString("0000");

                    double ingresoMes = 0;
                    double egresoMes = 0;

                    try
                    {
                        DataRow[] drRecibosIngreso = dtRecibos.Select(" (P2426_TIPO_DOC = 'R' OR P2426_TIPO_DOC = 'P') AND (P2406_STATUS = 2) AND  P2409_FECHA_EMISION >='" + fechaPrimerDiaMesIB + "' AND P2409_FECHA_EMISION <='" + fechaUltimoDiaMesIB + "' ");

                        foreach (DataRow drIngreso in drRecibosIngreso)
                        {
                            if (drIngreso["P2405_IMPORTE"] != null && drIngreso["P2405_IMPORTE"].ToString().Trim().Length > 0)
                            {
                                if (drIngreso["P2410_MONEDA"].ToString().Trim() == "D")
                                    ingresoMes = ingresoMes + (Convert.ToDouble(drIngreso["P2405_IMPORTE"]) * Convert.ToDouble(drIngreso["P2414_TIPO_CAMBIO"]));
                                else
                                    ingresoMes = ingresoMes + Convert.ToDouble(drIngreso["P2405_IMPORTE"]);

                            }
                        }
                    }
                    catch
                    {
                        ingresoMes = 0;
                    }

                    try
                    {
                        DataRow[] drRecibosEgreso = dtRecibos.Select("P2406_STATUS <> 3 AND P2409_FECHA_EMISION >='" + fechaPrimerDiaMesIB + "'  AND P2409_FECHA_EMISION <='" + fechaUltimoDiaMesIB + "' AND P2426_TIPO_DOC = 'B'");


                        foreach (DataRow drEgreso in drRecibosEgreso)
                        {
                            if (drEgreso["P2405_IMPORTE"] != null && drEgreso["P2405_IMPORTE"].ToString().Trim().Length > 0)
                            {
                                if (drEgreso["P2410_MONEDA"].ToString().Trim() == "D")
                                    egresoMes = egresoMes + (Convert.ToDouble(drEgreso["P2405_IMPORTE"]) * Convert.ToDouble(drEgreso["P2414_TIPO_CAMBIO"]));
                                else
                                    egresoMes = egresoMes + Convert.ToDouble(drEgreso["P2405_IMPORTE"]);


                            }
                        }
                    }
                    catch
                    {
                        egresoMes = 0;
                    }

                    ingresosAnoActual[j] = ingresoMes - egresoMes;
                }

                OnCambioProgreso(60);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                dtIngreso.Rows.Add(ingresosAnoAnterior[0] != 0 ? String.Format("{0:#,0.00}", ingresosAnoAnterior[0]) : "0", "Enero", ingresosAnoActual[0] != 0 ? String.Format("{0:#,0.00}", ingresosAnoActual[0]) : "0", "Enero");
                dtIngreso.Rows.Add(ingresosAnoAnterior[1] != 0 ? String.Format("{0:#,0.00}", ingresosAnoAnterior[1]) : "0", "Febrero", ingresosAnoActual[1] != 0 ? String.Format("{0:#,0.00}", ingresosAnoActual[1]) : "0", "Febrero");
                dtIngreso.Rows.Add(ingresosAnoAnterior[2] != 0 ? String.Format("{0:#,0.00}", ingresosAnoAnterior[2]) : "0", "Marzo", ingresosAnoActual[2] != 0 ? String.Format("{0:#,0.00}", ingresosAnoActual[2]) : "0", "Marzo");
                dtIngreso.Rows.Add(ingresosAnoAnterior[3] != 0 ? String.Format("{0:#,0.00}", ingresosAnoAnterior[3]) : "0", "Abril", ingresosAnoActual[3] != 0 ? String.Format("{0:#,0.00}", ingresosAnoActual[3]) : "0", "Abril");
                dtIngreso.Rows.Add(ingresosAnoAnterior[4] != 0 ? String.Format("{0:#,0.00}", ingresosAnoAnterior[4]) : "0", "Mayo", ingresosAnoActual[4] != 0 ? String.Format("{0:#,0.00}", ingresosAnoActual[4]) : "0", "Mayo");
                dtIngreso.Rows.Add(ingresosAnoAnterior[5] != 0 ? String.Format("{0:#,0.00}", ingresosAnoAnterior[5]) : "0", "Junio", ingresosAnoActual[5] != 0 ? String.Format("{0:#,0.00}", ingresosAnoActual[5]) : "0", "Junio");
                dtIngreso.Rows.Add(ingresosAnoAnterior[6] != 0 ? String.Format("{0:#,0.00}", ingresosAnoAnterior[6]) : "0", "Julio", ingresosAnoActual[6] != 0 ? String.Format("{0:#,0.00}", ingresosAnoActual[6]) : "0", "Julio");
                dtIngreso.Rows.Add(ingresosAnoAnterior[7] != 0 ? String.Format("{0:#,0.00}", ingresosAnoAnterior[7]) : "0", "Agosto", ingresosAnoActual[7] != 0 ? String.Format("{0:#,0.00}", ingresosAnoActual[7]) : "0", "Agosto");
                dtIngreso.Rows.Add(ingresosAnoAnterior[8] != 0 ? String.Format("{0:#,0.00}", ingresosAnoAnterior[8]) : "0", "Septiembre", ingresosAnoActual[8] != 0 ? String.Format("{0:#,0.00}", ingresosAnoActual[8]) : "0", "Septiembre");
                dtIngreso.Rows.Add(ingresosAnoAnterior[9] != 0 ? String.Format("{0:#,0.00}", ingresosAnoAnterior[9]) : "0", "Octubre", ingresosAnoActual[9] != 0 ? String.Format("{0:#,0.00}", ingresosAnoActual[9]) : "0", "Octubre");
                dtIngreso.Rows.Add(ingresosAnoAnterior[10] != 0 ? String.Format("{0:#,0.00}", ingresosAnoAnterior[10]) : "0", "Noviembre", ingresosAnoActual[10] != 0 ? String.Format("{0:#,0.00}", ingresosAnoActual[10]) : "0", "Noviembre");
                dtIngreso.Rows.Add(ingresosAnoAnterior[11] != 0 ? String.Format("{0:#,0.00}", ingresosAnoAnterior[11]) : "0", "Diciembre", ingresosAnoActual[11] != 0 ? String.Format("{0:#,0.00}", ingresosAnoActual[11]) : "0", "Diciembre");

                OnCambioProgreso(70);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                double totalAnt = 0;
                for (int i = 0; i < 12; i++)
                    totalAnt = totalAnt + ingresosAnoAnterior[i];

                IfrxComponent memoTotalAnt = reporte.FindObject("memoTotalAnt");
                (memoTotalAnt as IfrxCustomMemoView).Text = totalAnt != 0 ? String.Format("{0:#,0.00}", totalAnt) : "0";

                double totalActual = 0;
                for (int i = 0; i < 12; i++)
                    totalActual = totalActual + ingresosAnoActual[i];

                OnCambioProgreso(80);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                IfrxComponent memoTotal = reporte.FindObject("memoTotal");
                (memoTotal as IfrxCustomMemoView).Text = totalActual != 0 ? String.Format("{0:#,0.00}", totalActual) : "0";

                FrxDataView dvInmobiliaria = new FrxDataView(dtInmobiliaria, "dvInmobiliaria");
                FrxDataView dvIngreso = new FrxDataView(dtIngreso, "dvIngreso");

                dvInmobiliaria.AssignToReport(true, reporte);
                dvIngreso.AssignToReport(true, reporte);

                dtInmobiliaria.AssignToDataBand("MasterData1", reporte); //Parche para que aparezca encabezado de reporte
                dtIngreso.AssignToDataBand("MasterData1", reporte);

                OnCambioProgreso(90);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                return exportar(reporte, esPdf, "HistoricoIngresosConsolidado");
                /*reporte.PrepareReport(true);

                if (esPdf)
                {
                    reporte.ShowPreparedReport();
                }
                else
                {
                    try
                    {
                        string directoryTmpReportesSaari = @"C:\Users\Public\Documents\SaariDB\TmpReportes";
                        if (!Directory.Exists(directoryTmpReportesSaari))
                            Directory.CreateDirectory(directoryTmpReportesSaari);

                        string archivoXlsTemporal = directoryTmpReportesSaari + "\\Reporte" + DateTime.Now.Ticks + ".xls";
                        reporte.ExportToXLS(archivoXlsTemporal, true, false, true, false, false, true);
                        System.Diagnostics.Process.Start(archivoXlsTemporal);
                    }
                    catch { }
                }*/
            }
            catch (Exception ex)
            {
                return "Error general al generar reporte: " + Environment.NewLine + ex.Message; 
            }
        }

        private string generarReporte306090Mas90()
        {
            try
            {
                OnCambioProgreso(10);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                TfrxReportClass reporte = new TfrxReportClass();
                reporte.ClearReport();
                reporte.ClearDatasets();
                reporte.LoadReportFromFile(rutaPlantilla);


                /*Tablas de datos conforme a plantilla de reporte*/

                FrxDataTable dtInmobiliaria = new FrxDataTable("dvInmobiliaria");
                DataColumn dcInmobiliaraRazonSocial = new DataColumn("razonSocial", typeof(string));
                DataColumn dcInmobiliaraNombreComercial = new DataColumn("nombreComercial", typeof(string));
                dtInmobiliaria.Columns.Add(dcInmobiliaraRazonSocial);
                dtInmobiliaria.Columns.Add(dcInmobiliaraNombreComercial);
                dtInmobiliaria.AcceptChanges();

                FrxDataTable dtConjunto = new FrxDataTable("dvConjunto");
                DataColumn dcConjuntoNombre = new DataColumn("nombreConjunto", typeof(string));
                dtConjunto.Columns.Add(dcConjuntoNombre);
                dtConjunto.AcceptChanges();

                FrxDataTable dtDetalleCartera = new FrxDataTable("dvDetalle");
                DataColumn dcDetalleCarteraRazonSocialCliente = new DataColumn("razonSocialCliente", typeof(string));
                DataColumn dcDetalleCarteraNombreComercialCliente = new DataColumn("nombreComercialCliente", typeof(string));
                DataColumn dcDetalleCarteraTotal = new DataColumn("carteraTotal", typeof(string));
                DataColumn dcDetalleCartera30 = new DataColumn("cartera30", typeof(string));
                DataColumn dcDetalleCartera60 = new DataColumn("cartera60", typeof(string));
                DataColumn dcDetalleCartera90 = new DataColumn("cartera90", typeof(string));
                DataColumn dcDetalleCarteraMas90 = new DataColumn("carteraMas90", typeof(string));
                dtDetalleCartera.Columns.Add(dcDetalleCarteraRazonSocialCliente);
                dtDetalleCartera.Columns.Add(dcDetalleCarteraNombreComercialCliente);
                dtDetalleCartera.Columns.Add(dcDetalleCarteraTotal);
                dtDetalleCartera.Columns.Add(dcDetalleCartera30);
                dtDetalleCartera.Columns.Add(dcDetalleCartera60);
                dtDetalleCartera.Columns.Add(dcDetalleCartera90);
                dtDetalleCartera.Columns.Add(dcDetalleCarteraMas90);
                dtDetalleCartera.AcceptChanges();

                FrxDataTable dtSaldos = new FrxDataTable("dvSaldos");
                DataColumn dcSaldosCarteraTotal = new DataColumn("carteraTotal", typeof(string));
                DataColumn dcSaldosCartera30 = new DataColumn("cartera30", typeof(string));
                DataColumn dcSaldosCartera60 = new DataColumn("cartera60", typeof(string));
                DataColumn dcSaldosCartera90 = new DataColumn("cartera90", typeof(string));
                DataColumn dcSaldosCarteraMas90 = new DataColumn("carteraMas90", typeof(string));
                dtSaldos.Columns.Add(dcSaldosCarteraTotal);
                dtSaldos.Columns.Add(dcSaldosCartera30);
                dtSaldos.Columns.Add(dcSaldosCartera60);
                dtSaldos.Columns.Add(dcSaldosCartera90);
                dtSaldos.Columns.Add(dcSaldosCarteraMas90);
                dtSaldos.AcceptChanges();

                OnCambioProgreso(20);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                T01_ARRENDADORATableAdapter taArrendadora = new T01_ARRENDADORATableAdapter();
                taArrendadora.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;
                DataRow drInmobiliaria = taArrendadora.GetArrendadoraByIdArr(this.idInmobiliaria).Rows[0];
                dtInmobiliaria.Rows.Add(drInmobiliaria["P0103_RAZON_SOCIAL"].ToString().Trim(), drInmobiliaria["P0102_N_COMERCIAL"].ToString().Trim());

                if (!string.IsNullOrEmpty(idConjunto))
                {
                    T03_CENTRO_INDUSTRIALTableAdapter taConjunto = new T03_CENTRO_INDUSTRIALTableAdapter();
                    taConjunto.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;
                    DataRow drConjunto = taConjunto.GetConjuntoByIdConjunto(this.idConjunto).Rows[0];
                    dtConjunto.Rows.Add(drConjunto["P0303_NOMBRE"].ToString().Trim());
                }
                else
                {
                    dtConjunto.Rows.Add("TODOS");
                }


                IfrxComponent memoFechaCorteCartera = reporte.FindObject("memoFechaCorteCartera");
                (memoFechaCorteCartera as IfrxCustomMemoView).Text = "Cartera vencida al " + this.fechaIniCorte.Day.ToString("00") + " de " + getNombreMes(this.fechaIniCorte.Month) + " del " + this.fechaIniCorte.Year;

                DateTime dateIni30 = this.fechaIniCorte.AddDays(-30); DateTime dateFin30 = this.fechaIniCorte;
                DateTime dateIni60 = this.fechaIniCorte.AddDays(-60); DateTime dateFin60 = this.fechaIniCorte.AddDays(-31);
                DateTime dateIni90 = this.fechaIniCorte.AddDays(-90); DateTime dateFin90 = this.fechaIniCorte.AddDays(-61);
                DateTime dateIniMas90 = this.fechaIniCorte.AddYears(-10); DateTime dateFinMas90 = this.fechaIniCorte.AddDays(-91);

                IfrxComponent memoMes30 = reporte.FindObject("memoMes30");
                (memoMes30 as IfrxCustomMemoView).Text = getNombreMes(dateFin30.Month);

                IfrxComponent memoMes60 = reporte.FindObject("memoMes60");
                (memoMes60 as IfrxCustomMemoView).Text = getNombreMes(dateFin60.Month);

                IfrxComponent memoMes90 = reporte.FindObject("memoMes90");
                (memoMes90 as IfrxCustomMemoView).Text = getNombreMes(dateFin90.Month);

                IfrxComponent memoMesMas90 = reporte.FindObject("memoMesMas90");
                (memoMesMas90 as IfrxCustomMemoView).Text = getNombreMes(dateFinMas90.Month);

                IfrxComponent dvFecha = reporte.FindObject("dvFecha");
                (dvFecha as IfrxCustomMemoView).Text = "Cartera vencida al " + this.fechaIniCorte.Day.ToString("00") + " de " + getNombreMes(this.fechaIniCorte.Month) + " del " + this.fechaIniCorte.Year;

                ODBCQuery query = new ODBCQuery();
                DataTable dtCliente = query.DTArrendadora("SELECT * FROM T24_HISTORIA_RECIBOS WHERE P2401_ID_ARRENDADORA='" + this.idInmobiliaria + "'");
                List<String> listaclientes = new List<string>();
                foreach (DataRow drCliente in dtCliente.Rows)
                {
                    if (!listaclientes.Contains(drCliente["P2402_ID_ARRENDATARIO"].ToString().Trim()))
                        listaclientes.Add(drCliente["P2402_ID_ARRENDATARIO"].ToString().Trim());

                }

                OnCambioProgreso(30);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                T24_HISTORIA_RECIBOSTableAdapter taRecibosByArrendadora = new T24_HISTORIA_RECIBOSTableAdapter();
                taRecibosByArrendadora.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;

                DataTable dtAllRecibos = new DataTable();

                if (!string.IsNullOrEmpty(idConjunto))
                {
                    dtAllRecibos = taRecibosByArrendadora.GetAllRecibosByIdArrAndIdConjunto(this.idInmobiliaria, this.idConjunto);
                }
                else
                {
                    dtAllRecibos = taRecibosByArrendadora.GetAllRecibosByIdArr(this.idInmobiliaria);
                }

                double saldo30 = 0;
                double saldo60 = 0;
                double saldo90 = 0;
                double saldoMas90 = 0;
                double saldoTotal = 0;

                OnCambioProgreso(40);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                int porcentaje = 40;
                decimal factor = 30 / listaclientes.Count;
                factor = factor >= 1 ? factor : 1;

                for (int k = 0; k < listaclientes.Count; k++)
                {
                    if (porcentaje <= 70)
                        porcentaje += Convert.ToInt32(factor);
                    OnCambioProgreso(porcentaje <= 70 ? porcentaje : 70);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    string idCnt = listaclientes[k];

                    string fechaIniIB30 = dateIni30.Day.ToString("00") + "." + dateIni30.Month.ToString("00") + "." + dateIni30.Year.ToString("0000");
                    string fechaFinIB30 = dateFin30.Day.ToString("00") + "." + dateFin30.Month.ToString("00") + "." + dateFin30.Year.ToString("0000");
                    string fechaIniIB60 = dateIni60.Day.ToString("00") + "." + dateIni60.Month.ToString("00") + "." + dateIni60.Year.ToString("0000");
                    string fechaFinIB60 = dateFin60.Day.ToString("00") + "." + dateFin60.Month.ToString("00") + "." + dateFin60.Year.ToString("0000");
                    string fechaIniIB90 = dateIni90.Day.ToString("00") + "." + dateIni90.Month.ToString("00") + "." + dateIni90.Year.ToString("0000");
                    string fechaFinIB90 = dateFin90.Day.ToString("00") + "." + dateFin90.Month.ToString("00") + "." + dateFin90.Year.ToString("0000");
                    string fechaIniIBMas90 = dateIniMas90.Day.ToString("00") + "." + dateIniMas90.Month.ToString("00") + "." + dateIniMas90.Year.ToString("0000");
                    string fechaFinIBMas90 = dateFinMas90.Day.ToString("00") + "." + dateFinMas90.Month.ToString("00") + "." + dateFinMas90.Year.ToString("0000");


                    double total30 = 0;
                    double total60 = 0;
                    double total90 = 0;
                    double totalMas90 = 0;
                    // se quito status 2 falta los vacios
                    DataRow[] drRecibos30 = dtAllRecibos.Select("((P2426_TIPO_DOC = 'R' "
                          + " OR P2426_TIPO_DOC = 'X' OR P2426_TIPO_DOC = 'T' OR P2426_TIPO_DOC = 'Z') "
                          + " AND P2409_FECHA_EMISION >= '" + fechaIniIB30 + "' AND P2409_FECHA_EMISION <= '" + fechaFinIB30 + "' "
                          + " AND (P2408_FECHA_PAGADO IS NULL OR P2408_FECHA_PAGADO > '" + fechaFinIB30 + "') "
                          + " AND P2406_STATUS  = 1"
                          + " AND P2402_ID_ARRENDATARIO = '" + idCnt + "')"
                          + " or "
                          + " ((P2426_TIPO_DOC = 'R' OR P2426_TIPO_DOC = 'X' OR P2426_TIPO_DOC = 'T' OR P2426_TIPO_DOC = 'Z')"
                          + " AND P2406_STATUS = 1 "
                          + " AND P2409_FECHA_EMISION >= '" + fechaIniIB30 + "' AND P2409_FECHA_EMISION <= '" + fechaFinIB30 + "' "
                          + " AND P2402_ID_ARRENDATARIO = '" + idCnt + "')"
                          + " or "
                          + " ( P2426_TIPO_DOC = 'T' AND P2406_STATUS = 0 AND CAMPO_DATE1 <= '" + fechaFinIB30 + "' "
                          + " AND P2402_ID_ARRENDATARIO = '" + idCnt + "')");

                    DataRow[] drRecibos60 = dtAllRecibos.Select("((P2426_TIPO_DOC = 'R' "
                          + " OR P2426_TIPO_DOC = 'X' OR P2426_TIPO_DOC = 'T' OR P2426_TIPO_DOC = 'Z') "
                          + " AND P2409_FECHA_EMISION >= '" + fechaIniIB60 + "' AND P2409_FECHA_EMISION <= '" + fechaFinIB60 + "' "
                          + " AND (P2408_FECHA_PAGADO IS NULL OR P2408_FECHA_PAGADO > '" + fechaFinIB60 + "') "
                          + " AND P2406_STATUS  = 1"
                          + " AND P2402_ID_ARRENDATARIO = '" + idCnt + "')"
                          + " or "
                          + " ((P2426_TIPO_DOC = 'R' OR P2426_TIPO_DOC = 'X' OR P2426_TIPO_DOC = 'T' OR P2426_TIPO_DOC = 'Z')"
                          + " AND P2406_STATUS = 1"
                          + " AND P2409_FECHA_EMISION >= '" + fechaIniIB60 + "' AND P2409_FECHA_EMISION <= '" + fechaFinIB60 + "' "
                          + " AND P2402_ID_ARRENDATARIO = '" + idCnt + "')"
                          + " or "
                          + " ( P2426_TIPO_DOC = 'T' AND P2406_STATUS = 0 AND CAMPO_DATE1 <= '" + fechaFinIB60 + "' "
                          + " AND P2402_ID_ARRENDATARIO = '" + idCnt + "')");

                    DataRow[] drRecibos90 = dtAllRecibos.Select("((P2426_TIPO_DOC = 'R' "
                          + " OR P2426_TIPO_DOC = 'X' OR P2426_TIPO_DOC = 'T' OR P2426_TIPO_DOC = 'Z') "
                          + " AND P2409_FECHA_EMISION >= '" + fechaIniIB90 + "' AND P2409_FECHA_EMISION <= '" + fechaFinIB90 + "' "
                          + " AND (P2408_FECHA_PAGADO IS NULL OR P2408_FECHA_PAGADO > '" + fechaFinIB90 + "') "
                          + " AND P2406_STATUS  = 1"
                          + " AND P2402_ID_ARRENDATARIO = '" + idCnt + "')"
                          + " or "
                          + " ((P2426_TIPO_DOC = 'R' OR P2426_TIPO_DOC = 'X' OR P2426_TIPO_DOC = 'T' OR P2426_TIPO_DOC = 'Z')"
                          + " AND P2406_STATUS = 1  "
                          + " AND P2409_FECHA_EMISION >= '" + fechaIniIB90 + "' AND P2409_FECHA_EMISION <= '" + fechaFinIB90 + "' "
                          + " AND P2402_ID_ARRENDATARIO = '" + idCnt + "')"
                          + " or "
                          + " ( P2426_TIPO_DOC = 'T' AND P2406_STATUS = 0 AND CAMPO_DATE1 <= '" + fechaFinIB90 + "' "
                          + " AND P2402_ID_ARRENDATARIO = '" + idCnt + "')");

                    DataRow[] drRecibosMas90 = dtAllRecibos.Select("((P2426_TIPO_DOC = 'R' "
                          + " OR P2426_TIPO_DOC = 'X' OR P2426_TIPO_DOC = 'T' OR P2426_TIPO_DOC = 'Z') "
                          + " AND P2409_FECHA_EMISION >= '" + fechaIniIBMas90 + "' AND P2409_FECHA_EMISION <= '" + fechaFinIBMas90 + "' "
                          + " AND (P2408_FECHA_PAGADO IS NULL OR P2408_FECHA_PAGADO > '" + fechaFinIBMas90 + "') "
                          + " AND P2406_STATUS  = 1"
                          + " AND P2402_ID_ARRENDATARIO = '" + idCnt + "')"
                          + " or "
                          + " ((P2426_TIPO_DOC = 'R' OR P2426_TIPO_DOC = 'X' OR P2426_TIPO_DOC = 'T' OR P2426_TIPO_DOC = 'Z')"
                          + " AND P2406_STATUS = 1  "
                          + " AND P2409_FECHA_EMISION >= '" + fechaIniIBMas90 + "' AND P2409_FECHA_EMISION <= '" + fechaFinIBMas90 + "' "
                          + " AND P2402_ID_ARRENDATARIO = '" + idCnt + "')"
                          + " or "
                          + " ( P2426_TIPO_DOC = 'T' AND P2406_STATUS = 0 AND CAMPO_DATE1 <= '" + fechaFinIBMas90 + "' "
                          + " AND P2402_ID_ARRENDATARIO = '" + idCnt + "')");


                    foreach (DataRow drRecibo in drRecibos30)
                    {
                        double saldoRecibo = 0;
                        if (!repIva)
                        {
                            saldoRecibo = Convert.ToDouble(drRecibo["P2405_IMPORTE"]);
                        }
                        else
                        {
                            saldoRecibo = Convert.ToDouble(drRecibo["P2427_CTD_PAG"]);
                        }

                        try
                        {
                            if (drRecibo["P2410_MONEDA"].ToString().Trim() == "D")
                                saldoRecibo = saldoRecibo * Convert.ToDouble(drRecibo["P2414_TIPO_CAMBIO"]);
                        }
                        catch { }
                        total30 = total30 + saldoRecibo;
                    }


                    foreach (DataRow drRecibo in drRecibos60)
                    {
                        double saldoRecibo = 0;
                        if (!repIva)
                        {
                            saldoRecibo = Convert.ToDouble(drRecibo["P2405_IMPORTE"]);
                        }
                        else
                        {
                            saldoRecibo = Convert.ToDouble(drRecibo["P2427_CTD_PAG"]);
                        }



                        try
                        {
                            if (drRecibo["P2410_MONEDA"].ToString().Trim() == "D")
                                saldoRecibo = saldoRecibo * Convert.ToDouble(drRecibo["P2414_TIPO_CAMBIO"]);
                        }
                        catch { }

                        total60 = total60 + saldoRecibo;
                    }

                    foreach (DataRow drRecibo in drRecibos90)
                    {
                        double saldoRecibo = 0;
                        if (!repIva)
                        {
                            saldoRecibo = Convert.ToDouble(drRecibo["P2405_IMPORTE"]);
                        }
                        else
                        {
                            saldoRecibo = Convert.ToDouble(drRecibo["P2427_CTD_PAG"]);
                        }


                        try
                        {
                            if (drRecibo["P2410_MONEDA"].ToString().Trim() == "D")
                                saldoRecibo = saldoRecibo * Convert.ToDouble(drRecibo["P2414_TIPO_CAMBIO"]);
                        }
                        catch { }
                        total90 = total90 + saldoRecibo;
                    }

                    foreach (DataRow drRecibo in drRecibosMas90)
                    {
                        double saldoRecibo = 0;
                        if (!repIva)
                        {
                            saldoRecibo = Convert.ToDouble(drRecibo["P2405_IMPORTE"]);
                        }
                        else
                        {
                            saldoRecibo = Convert.ToDouble(drRecibo["P2427_CTD_PAG"]);
                        }


                        try
                        {
                            if (drRecibo["P2410_MONEDA"].ToString().Trim() == "D")
                                saldoRecibo = saldoRecibo * Convert.ToDouble(drRecibo["P2414_TIPO_CAMBIO"]);
                        }
                        catch { }
                        totalMas90 = totalMas90 + saldoRecibo;
                    }


                    if (total30 > 0 || total60 > 0 || total90 > 0 || totalMas90 > 0)
                    {
                        /*T04_CONTRATOTableAdapter taContratoActual = new T04_CONTRATOTableAdapter();
                        taContratoActual.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;
                        string idCte = taContratoActual.GetContratoById(idCnt).Rows[0]["P0402_ID_ARRENDAT"].ToString().Trim();*/
                        string idCte = listaclientes[k];
                        T02_ARRENDATARIOTableAdapter taCliente = new T02_ARRENDATARIOTableAdapter();
                        taCliente.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;
                        DataRow drCliente = taCliente.GetArrendatarioById(idCte).Rows[0];
                        string razonSocialCliente = "";
                        string nombreComercialCliente = "";

                        if (drCliente["P0203_NOMBRE"] != null && drCliente["P0203_NOMBRE"].ToString().Trim().Length > 0)
                            razonSocialCliente = drCliente["P0203_NOMBRE"].ToString().Trim();

                        if (drCliente["P0253_NOMBRE_COMERCIAL"] != null && drCliente["P0253_NOMBRE_COMERCIAL"].ToString().Trim().Length > 0)
                            nombreComercialCliente = drCliente["P0253_NOMBRE_COMERCIAL"].ToString().Trim();

                        double totalCartera = total30 + total60 + total90 + totalMas90;

                        DataRow drDetalleCartera = dtDetalleCartera.NewRow();
                        drDetalleCartera["razonSocialCliente"] = razonSocialCliente;
                        drDetalleCartera["nombreComercialCliente"] = nombreComercialCliente;
                        /*drDetalleCartera["carteraTotal"] = totalCartera != 0 ? String.Format("{0:#,0.00}", totalCartera) : "-";
                        drDetalleCartera["cartera30"] = total30 != 0 ? String.Format("{0:#,0.00}", total30) : "-";
                        drDetalleCartera["cartera60"] = total60 != 0 ? String.Format("{0:#,0.00}", total60) : "-";
                        drDetalleCartera["cartera90"] = total90 != 0 ? String.Format("{0:#,0.00}", total90) : "-";
                        drDetalleCartera["carteraMas90"] = totalMas90 != 0 ? String.Format("{0:#,0.00}", totalMas90) : "-";*/
                        drDetalleCartera["carteraTotal"] = String.Format("{0:#,0.00}", totalCartera);
                        drDetalleCartera["cartera30"] = String.Format("{0:#,0.00}", total30);
                        drDetalleCartera["cartera60"] = String.Format("{0:#,0.00}", total60);
                        drDetalleCartera["cartera90"] = String.Format("{0:#,0.00}", total90);
                        drDetalleCartera["carteraMas90"] = String.Format("{0:#,0.00}", totalMas90);
                        dtDetalleCartera.Rows.Add(drDetalleCartera);

                        saldo30 = saldo30 + total30;
                        saldo60 = saldo60 + total60;
                        saldo90 = saldo90 + total90;
                        saldoMas90 = saldoMas90 + totalMas90;
                    }
                }

                OnCambioProgreso(80);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                /* Ordenado por nombre de cliente */

                DataRow[] drDetalleOrdenado = dtDetalleCartera.Select(null, "razonSocialCliente ASC");

                /* Tabla para ordenar detalles por nombre de cliente */

                FrxDataTable dtDetalle = new FrxDataTable("dvDetalleCartera");
                DataColumn dcDetalleRazonSocialCliente = new DataColumn("razonSocialCliente", typeof(string));
                DataColumn dcDetalleNombreComercialCliente = new DataColumn("nombreComercialCliente", typeof(string));
                DataColumn dcDetalleTotal = new DataColumn("carteraTotal", typeof(string));
                DataColumn dcDetalle30 = new DataColumn("cartera30", typeof(string));
                DataColumn dcDetalle60 = new DataColumn("cartera60", typeof(string));
                DataColumn dcDetalle90 = new DataColumn("cartera90", typeof(string));
                DataColumn dcDetalleMas90 = new DataColumn("carteraMas90", typeof(string));
                dtDetalle.Columns.Add(dcDetalleRazonSocialCliente);
                dtDetalle.Columns.Add(dcDetalleNombreComercialCliente);
                dtDetalle.Columns.Add(dcDetalleTotal);
                dtDetalle.Columns.Add(dcDetalle30);
                dtDetalle.Columns.Add(dcDetalle60);
                dtDetalle.Columns.Add(dcDetalle90);
                dtDetalle.Columns.Add(dcDetalleMas90);
                dtDetalle.AcceptChanges();

                /* Fill (llenado) de tabla ordenada */

                for (int i = 0; i < drDetalleOrdenado.Length; i++)
                {
                    dtDetalle.Rows.Add(drDetalleOrdenado[i]["razonSocialCliente"].ToString(), drDetalleOrdenado[i]["nombreComercialCliente"].ToString(), drDetalleOrdenado[i]["carteraTotal"].ToString(),
                              drDetalleOrdenado[i]["cartera30"].ToString(), drDetalleOrdenado[i]["cartera60"].ToString(), drDetalleOrdenado[i]["cartera90"].ToString(),
                              drDetalleOrdenado[i]["carteraMas90"].ToString());
                }

                saldoTotal = saldo30 + saldo60 + saldo90 + saldoMas90;

                DataRow drSaldos = dtSaldos.NewRow();
                drSaldos["carteraTotal"] = String.Format("{0:#,0.00}", saldoTotal);
                drSaldos["cartera30"] = String.Format("{0:#,0.00}", saldo30);
                drSaldos["cartera60"] = String.Format("{0:#,0.00}", saldo60);
                drSaldos["cartera90"] = String.Format("{0:#,0.00}", saldo90);
                drSaldos["carteraMas90"] = String.Format("{0:#,0.00}", saldoMas90);
                dtSaldos.Rows.Add(drSaldos);

                OnCambioProgreso(90);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                FrxDataView dvInmobiliaria = new FrxDataView(dtInmobiliaria, "dvInmobiliaria");
                FrxDataView dvConjunto = new FrxDataView(dtConjunto, "dvConjunto");
                FrxDataView dvDetalleCartera = new FrxDataView(dtDetalle, "dvDetalleCartera");
                FrxDataView dvSaldos = new FrxDataView(dtSaldos, "dvSaldos");

                dvInmobiliaria.AssignToReport(true, reporte);
                dvConjunto.AssignToReport(true, reporte);
                dvDetalleCartera.AssignToReport(true, reporte);
                dvSaldos.AssignToReport(true, reporte);

                dtInmobiliaria.AssignToDataBand("MasterData1", reporte); //Parche para que aparezca encabezado de reporte
                dtDetalle.AssignToDataBand("MasterData4", reporte);
                dtSaldos.AssignToDataBand("MasterData2", reporte);
                dtSaldos.AssignToDataBand("MasterData3", reporte);

                return exportar(reporte, esPdf, "CarteraVencida306090");
                /*reporte.PrepareReport(true);

                if (esPdf)
                {
                    reporte.ShowPreparedReport();
                }
                else
                {
                    try
                    {
                        string directoryTmpReportesSaari = @"C:\Users\Public\Documents\SaariDB\TmpReportes";
                        if (!Directory.Exists(directoryTmpReportesSaari))
                            Directory.CreateDirectory(directoryTmpReportesSaari);

                        string archivoXlsTemporal = directoryTmpReportesSaari + "\\Reporte" + DateTime.Now.Ticks + ".xls";
                        reporte.ExportToXLS(archivoXlsTemporal, true, false, true, false, false, true);
                        System.Diagnostics.Process.Start(archivoXlsTemporal);
                    }
                    catch { }
                }
                idConjunto = string.Empty;*/
            }
            catch (Exception ex)
            {
                return "Error general al generar reporte: " + Environment.NewLine + ex.Message; 
            }

        }

        private string generarReporteSaldosClientes()
        {
            try
            {
                //Al parecer quedo incompleto
                TfrxReportClass Reporte = new TfrxReportClass();
                Reporte.ClearReport();
                Reporte.ClearDatasets();
                Reporte.LoadReportFromFile(rutaPlantilla);

                FrxDataTable dtCliente = new FrxDataTable("TabCliente");
                DataColumn dcId = new DataColumn("IdCliente", typeof(string));
                DataColumn dcNombre = new DataColumn("Nombre", typeof(string));
                dtCliente.Columns.Add(dcId);
                dtCliente.Columns.Add(dcNombre);

                T02_ARRENDATARIOTableAdapter taCliente = new T02_ARRENDATARIOTableAdapter();
                taCliente.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;

                DataTable Tbl_Cliente = new DataTable();
                Tbl_Cliente = taCliente.GetDataEjemplo();

                foreach (DataRow Fila in Tbl_Cliente.Rows)
                {
                    dtCliente.Rows.Add(Fila["P0201_ID"].ToString(), Fila["P0203_NOMBRE"].ToString());
                }

                FrxDataView dvCliente = new FrxDataView(dtCliente, "TabCliente");
                dvCliente.AssignToReport(true, Reporte);


                dtCliente.AssignToDataBand("MasterData1", Reporte);
                return exportar(Reporte, esPdf, "SaldosCliente");
            }
            catch (Exception ex)
            {
                return "Error general al generar reporte: " + Environment.NewLine + ex.Message;
            }
        }

        private string getNombreMes(int numMes)
        {
            switch (numMes)
            {
                case  1:  return "Enero"; 
                case  2:  return "Febrero"; 
                case  3:  return "Marzo"; 
                case  4:  return "Abril"; 
                case  5:  return "Mayo"; 
                case  6:  return "Junio"; 
                case  7:  return "Julio"; 
                case  8:  return "Agosto"; 
                case  9:  return "Septiembre"; 
                case 10:  return "Octubre";
                case 11: return "Noviembre";
                case 12: return "Diciembre"; 
                default: return string.Empty;
            }
        }
    }
}
