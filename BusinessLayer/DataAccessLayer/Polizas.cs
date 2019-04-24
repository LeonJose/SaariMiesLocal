using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using GestorReportes.BusinessLayer.EntitiesPolizas;
using GestorReportes.BusinessLayer.Entities;
namespace GestorReportes.BusinessLayer.DataAccessLayer
{
    public static class Polizas
    {
        public static int versionODBC = SaariDB.getVersionDB();
        //Obtiene la fecha de actualización de la base de datos Saari y la toma como referencia
        //para la generación de polizas con Pago Total con saldo a favor e iva total cobrado tipo Arco
        //Actualización de cobranza y MIESLocal
        public static DateTime fechaActPol = SaariDB.getFechaAct();
        
        public static string agregarConfiguracion(ConfiguracionPolizaEntity configuracion)
        {
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);            
            try
            {
                string sql = @"INSERT INTO MIESLocal.Polizas_Estructura VALUES (@idarr, @tipopoliza, @tipocuenta, @cargoabono, @formula, @moneda, @estipo, @monedaPago)";
                foreach (FormulaPolizaEntity formPol in configuracion.Formulas)
                {
                    SqlCommand comando = conexion.CreateCommand();
                    comando.CommandText = sql;
                    comando.Parameters.Add("@idarr", SqlDbType.NVarChar).Value = configuracion.Inmobiliaria;
                    comando.Parameters.Add("@tipopoliza", SqlDbType.Int).Value = configuracion.TipoPoliza;
                    comando.Parameters.Add("@tipocuenta", SqlDbType.NVarChar).Value = formPol.TipoClave;
                    comando.Parameters.Add("@cargoabono", SqlDbType.Int).Value = formPol.CargoAbono;
                    comando.Parameters.Add("@formula", SqlDbType.NVarChar).Value = formPol.Formula;
                    comando.Parameters.Add("@moneda", SqlDbType.NVarChar).Value = formPol.Moneda;                    
                    comando.Parameters.Add("@estipo", SqlDbType.Bit).Value = formPol.EsSubtipo;
                    comando.Parameters.Add("@monedaPago", SqlDbType.NVarChar).Value = formPol.MonedaPago;
                    conexion.Open();
                    int afect = Convert.ToInt32(comando.ExecuteNonQuery());
                    conexion.Close();
                }
                return string.Empty;
            }
            catch(Exception ex)
            {
                conexion.Close();
                return "Error al guardar la configuración";
            }            
        }

        public static string agregarConfiguracion(ConfiguracionPolizaEntity configuracion, bool esEgreso)
        {
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            try
            {
                string sql = @"INSERT INTO MIESLocal.PolizasEgreso_Estructura VALUES (@idarr, @tipopoliza, @tipocuenta, @cargoabono, @formula)";
                foreach (FormulaPolizaEntity formPol in configuracion.Formulas)
                {
                    SqlCommand comando = conexion.CreateCommand();
                    comando.CommandText = sql;
                    comando.Parameters.Add("@idarr", SqlDbType.NVarChar).Value = configuracion.Inmobiliaria;
                    comando.Parameters.Add("@tipopoliza", SqlDbType.Int).Value = configuracion.TipoPoliza;
                    comando.Parameters.Add("@tipocuenta", SqlDbType.NVarChar).Value = formPol.TipoClave;
                    comando.Parameters.Add("@cargoabono", SqlDbType.Int).Value = formPol.CargoAbono;
                    comando.Parameters.Add("@formula", SqlDbType.NVarChar).Value = formPol.Formula;
                    conexion.Open();
                    int afect =Convert.ToInt32( comando.ExecuteNonQuery());
                    conexion.Close();
                }
                return string.Empty;
            }
            catch//(Exception ex)
            {
                conexion.Close();
                return "Error al guardar la configuración";
            }
        }

        public static bool existeConfiguracion(ConfiguracionPolizaEntity configuracion)
        {
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);            
            try
            {
                string sql = "SELECT * FROM MIESLocal.Polizas_Estructura WHERE ID_Arrendadora = @idarr AND TipoPoliza = @tipopoliza";
                SqlCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idarr", SqlDbType.NVarChar).Value = configuracion.Inmobiliaria;
                comando.Parameters.Add("@tipopoliza", SqlDbType.Int).Value = configuracion.TipoPoliza;
                DataTable dtEstructs = new DataTable();
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                dtEstructs.Load(reader);
                conexion.Close();
                return dtEstructs.Rows.Count > 0;
            }
            catch
            {
                conexion.Close();
                return false;
            }
        }

        public static bool existeConfiguracion(ConfiguracionPolizaEntity configuracion, bool esEgreso)
        {
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            try
            {
                string sql = "SELECT * FROM MIESLocal.PolizasEgreso_Estructura WHERE ID_Arrendadora = @idarr AND TipoPoliza = @tipopoliza";
                SqlCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idarr", SqlDbType.NVarChar).Value = configuracion.Inmobiliaria;
                comando.Parameters.Add("@tipopoliza", SqlDbType.Int).Value = configuracion.TipoPoliza;
                DataTable dtEstructs = new DataTable();
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                dtEstructs.Load(reader);
                conexion.Close();
                return dtEstructs.Rows.Count > 0;
            }
            catch
            {
                conexion.Close();
                return false;
            }
        }

        public static bool borrarConfiguraciones(ConfiguracionPolizaEntity configuracion)
        {
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);            
            try
            {
                string sql = "DELETE FROM MIESLocal.Polizas_Estructura WHERE ID_Arrendadora = @idarr AND TipoPoliza = @tipopoliza";
                SqlCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idarr", SqlDbType.NVarChar).Value = configuracion.Inmobiliaria;
                comando.Parameters.Add("@tipopoliza", SqlDbType.Int).Value = configuracion.TipoPoliza;
                conexion.Open();
                comando.ExecuteNonQuery();
                conexion.Close();
                return true;
            }
            catch
            {
                conexion.Close();
                return false;
            }
        }

        public static bool borrarConfiguraciones(ConfiguracionPolizaEntity configuracion, bool esEgreso)
        {
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            try
            {
                string sql = "DELETE FROM MIESLocal.PolizasEgreso_Estructura WHERE ID_Arrendadora = @idarr AND TipoPoliza = @tipopoliza";
                SqlCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idarr", SqlDbType.NVarChar).Value = configuracion.Inmobiliaria;
                comando.Parameters.Add("@tipopoliza", SqlDbType.Int).Value = configuracion.TipoPoliza;
                conexion.Open();
                comando.ExecuteNonQuery();
                conexion.Close();
                return true;
            }
            catch
            {
                conexion.Close();
                return false;
            }
        }

        public static List<RecibosEntitys> getRecibos(CondicionesEntity condiciones)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            List<RecibosEntitys> listaRecibos = new List<RecibosEntitys>();
            ModuloCobranzaEntity cobranzaEnt = new ModuloCobranzaEntity();
            try
            {
                string sql = string.Empty;
                //fechaActPol = SaariDB.getFechaAct();
                cobranzaEnt = SaariDB.getVersionCobranza();
                if (condiciones.TipoPoliza == 2)
                {
                    //Sentencia SQL para polizas de tipo de ingresos
                    //Se modifica el campo P2414_TIPO_CAMBIO POR EL P2421_TC_PAGO
                    //T24_HISTORIA_RECIBOS.CAMPO_NUM5 HACE REFERENCIA AL IDPAGO EN SAARI.PAGO
                    //Se agrega T24_HISTORIA_RECIBOS.P2443_CLAVE_ULT_MOV para validar si es RECIBO ESPORADICO V3.3.3.1
                    sql = @"SELECT T24_HISTORIA_RECIBOS.CAMPO_NUM5, P0103_RAZON_SOCIAL, P2444_ID_HIST_REC, P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2405_IMPORTE, P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.CAMPO5,
                                P2412_CONCEPTO, P2410_MONEDA, P2416_IVA, P2421_TC_PAGO, P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2443_CLAVE_ULT_MOV, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, 
                                T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.CAMPO20, T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.CAMPO_NUM1, T24_HISTORIA_RECIBOS.CAMPO_NUM2, P2413_PAGO,
                                P2408_FECHA_PAGADO, P2403_NUM_RECIBO, P2406_STATUS, T02_ARRENDATARIO.P0203_NOMBRE, P0204_RFC, P0106_RFC, T24_HISTORIA_RECIBOS.CAMPO18, T24_HISTORIA_RECIBOS.P2428_T_IVA, 
                                T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.CAMPO_NUM6,
                                T40_CFD.P4002_ID_CFD, T03_CENTRO_INDUSTRIAL.CAMPO4, T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T40_CFD.P4003_SUCURSAL, T24_HISTORIA_RECIBOS.CAMPO_NUM14
                                FROM T24_HISTORIA_RECIBOS JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
                                LEFT JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                                JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
                                LEFT JOIN T03_CENTRO_INDUSTRIAL ON T24_HISTORIA_RECIBOS.CAMPO4 = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO 
                                WHERE P2401_ID_ARRENDADORA = ? AND P2408_FECHA_PAGADO >= ? AND P2408_FECHA_PAGADO <= ? AND P2426_TIPO_DOC IN('R','Z') ";
                    if (!condiciones.IncluirCancelados)
                    {
                        sql += "AND P2406_STATUS = '2' ";
                    }
                    else
                    {
                        sql += "AND P2406_STATUS <> '1' ";
                    }
                    sql += @"UNION ALL SELECT T24_HISTORIA_RECIBOS.CAMPO_NUM5, P0103_RAZON_SOCIAL, P2444_ID_HIST_REC, P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2405_IMPORTE, P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.CAMPO5, 
                                P2412_CONCEPTO, P2410_MONEDA, P2416_IVA, P2414_TIPO_CAMBIO, P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2443_CLAVE_ULT_MOV, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, 
                                T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.CAMPO20, T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.CAMPO_NUM1, T24_HISTORIA_RECIBOS.CAMPO_NUM2, P2413_PAGO,
                                P2408_FECHA_PAGADO, P2403_NUM_RECIBO, P2406_STATUS, T02_ARRENDATARIO.P0203_NOMBRE, P0204_RFC, P0106_RFC, T24_HISTORIA_RECIBOS.CAMPO18, T24_HISTORIA_RECIBOS.P2428_T_IVA,   
                                T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.CAMPO_NUM6, 
                                T40_CFD.P4002_ID_CFD, T03_CENTRO_INDUSTRIAL.CAMPO4, T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T40_CFD.P4003_SUCURSAL, T24_HISTORIA_RECIBOS.CAMPO_NUM14
                                FROM T24_HISTORIA_RECIBOS JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
                                LEFT JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO = T40_CFD.P4001_ID_HIST_REC
                                JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
                                LEFT JOIN T03_CENTRO_INDUSTRIAL ON T24_HISTORIA_RECIBOS.CAMPO4 = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO 
                                WHERE P2401_ID_ARRENDADORA = ? AND P2408_FECHA_PAGADO >= ? AND P2408_FECHA_PAGADO <= ? AND P2426_TIPO_DOC IN('P','U') ";
                    if (!condiciones.IncluirCancelados)
                    {
                        sql += "AND P2406_STATUS = '2' ";
                    }
                    else
                    {
                        sql += "AND P2406_STATUS <> '1' ";
                    }
                }
                else if (condiciones.TipoPoliza == 1)
                {
                    sql = @"SELECT P0103_RAZON_SOCIAL, P2444_ID_HIST_REC, P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2405_IMPORTE, T24_HISTORIA_RECIBOS.CAMPO5,  
                                P2412_CONCEPTO, P2410_MONEDA, P2416_IVA, P2414_TIPO_CAMBIO, P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, 
                                T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.CAMPO20, T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.CAMPO_NUM1, T24_HISTORIA_RECIBOS.CAMPO_NUM2, 
                                P2403_NUM_RECIBO, P2406_STATUS, T02_ARRENDATARIO.P0203_NOMBRE, P0204_RFC, P0106_RFC, T24_HISTORIA_RECIBOS.CAMPO18, T24_HISTORIA_RECIBOS.P2443_CLAVE_ULT_MOV, 
                                T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.CAMPO_NUM6, 
                                T40_CFD.P4002_ID_CFD, T03_CENTRO_INDUSTRIAL.CAMPO4, T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T40_CFD.P4003_SUCURSAL, T24_HISTORIA_RECIBOS.CAMPO_NUM14     
                                FROM T24_HISTORIA_RECIBOS JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
                                LEFT JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                                JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
                                LEFT JOIN T03_CENTRO_INDUSTRIAL ON T24_HISTORIA_RECIBOS.CAMPO4 = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO 
                                WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION >= ? AND P2409_FECHA_EMISION <= ? AND ((P2426_TIPO_DOC = 'R' OR P2426_TIPO_DOC = 'X') OR P2426_TIPO_DOC = 'Z') ";
                    if (!condiciones.IncluirCancelados)
                    {
                        sql += "AND P2406_STATUS <> '3' ";
                    }
                }
                else if (condiciones.TipoPoliza == 3)
                {
                    sql = @"SELECT P0103_RAZON_SOCIAL, P2444_ID_HIST_REC, P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2405_IMPORTE, T24_HISTORIA_RECIBOS.CAMPO5,
                                P2412_CONCEPTO, P2410_MONEDA, P2416_IVA, P2414_TIPO_CAMBIO, P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2443_CLAVE_ULT_MOV, 
                                T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.CAMPO20, T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.CAMPO_NUM1, T24_HISTORIA_RECIBOS.CAMPO_NUM2, 
                                P2408_FECHA_PAGADO, P2403_NUM_RECIBO, P2406_STATUS, T02_ARRENDATARIO.P0203_NOMBRE, P0204_RFC, P0106_RFC, T24_HISTORIA_RECIBOS.CAMPO18, 
                                T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.CAMPO_NUM6, 
                                T40_CFD.P4002_ID_CFD, T03_CENTRO_INDUSTRIAL.CAMPO4,'' , T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T40_CFD.P4003_SUCURSAL, T24_HISTORIA_RECIBOS.CAMPO_NUM14  
                                FROM T24_HISTORIA_RECIBOS JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
                                LEFT JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                                JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
                                LEFT JOIN T03_CENTRO_INDUSTRIAL ON T24_HISTORIA_RECIBOS.CAMPO4 = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO 
                                WHERE P2401_ID_ARRENDADORA = ? AND P2408_FECHA_PAGADO >= ? AND P2408_FECHA_PAGADO <= ? AND P2426_TIPO_DOC = 'R' AND P2406_STATUS = '3'";
                }
                else
                {   //se agrea P2425_DEB_REC a la consulta para obtener el id de la factura relacionada a la NC
                    sql = @"SELECT P0103_RAZON_SOCIAL, P2444_ID_HIST_REC, P2425_DEB_REC, P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2405_IMPORTE, T24_HISTORIA_RECIBOS.CAMPO5,
                                P2412_CONCEPTO, P2410_MONEDA, P2416_IVA, P2414_TIPO_CAMBIO, P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2443_CLAVE_ULT_MOV, 
                                T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.CAMPO20, T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.CAMPO_NUM1, T24_HISTORIA_RECIBOS.CAMPO_NUM2, 
                                P2408_FECHA_PAGADO, P2403_NUM_RECIBO, P2406_STATUS, T02_ARRENDATARIO.P0203_NOMBRE, P0204_RFC, P0106_RFC, T24_HISTORIA_RECIBOS.CAMPO18, 
                                T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.CAMPO_NUM6, 
                                T40_CFD.P4002_ID_CFD, T03_CENTRO_INDUSTRIAL.CAMPO4, T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T40_CFD.P4003_SUCURSAL, T24_HISTORIA_RECIBOS.CAMPO_NUM14  
                                FROM T24_HISTORIA_RECIBOS JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
                                LEFT JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                                JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
                                LEFT JOIN T03_CENTRO_INDUSTRIAL ON T24_HISTORIA_RECIBOS.CAMPO4 = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO 
                                WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION >= ? AND P2409_FECHA_EMISION <= ? AND P2426_TIPO_DOC = 'B'";
                    if (!condiciones.IncluirCancelados)
                    {
                        sql += "AND P2406_STATUS <> '3' ";
                    }
                }
                
                if(cobranzaEnt.Version>=2212 && condiciones.TipoPoliza==2 && condiciones.FechaInicio.Date> cobranzaEnt.FechaAct.Date)
                {
                    sql += "ORDER BY 1,3";
                }
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idarr", OdbcType.NVarChar).Value = condiciones.Inmobiliaria;
                comando.Parameters.Add("@fechaini", OdbcType.Date).Value = condiciones.FechaInicio.Date;
                if (condiciones.Formato == FormatoExportacionPoliza.Excel || condiciones.TipoPresentacion == 3)
                    comando.Parameters.Add("@fechafin", OdbcType.Date).Value = condiciones.FechaFin.Date;
                else
                {
                    if(condiciones.PolizaPorRango)
                        comando.Parameters.Add("@fechafin", OdbcType.Date).Value = condiciones.FechaFin.Date;
                    else
                        comando.Parameters.Add("@fechafin", OdbcType.Date).Value = condiciones.FechaInicio.Date;
                    //comando.Parameters.Add("@fechafin", OdbcType.Date).Value = condiciones.FechaInicio;
                }
                if (condiciones.TipoPoliza == 2)
                {
                    comando.Parameters.Add("@idarr2", OdbcType.NVarChar).Value = condiciones.Inmobiliaria;                    
                    comando.Parameters.Add("@fechaini2", OdbcType.Date).Value = condiciones.FechaInicio.Date;
                    if (condiciones.Formato == FormatoExportacionPoliza.Excel || condiciones.TipoPresentacion == 3)
                        comando.Parameters.Add("@fechafin", OdbcType.Date).Value = condiciones.FechaFin.Date;
                    else
                    {
                        if(condiciones.PolizaPorRango)
                            comando.Parameters.Add("@fechafin2", OdbcType.Date).Value = condiciones.FechaFin.Date;
                        else
                            comando.Parameters.Add("@fechafin2", OdbcType.Date).Value = condiciones.FechaInicio.Date;
                    }
                }
                

                DataTable dtRegistros = new DataTable();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtRegistros.Load(reader);
                conexion.Close();
                if (dtRegistros.Rows.Count > 0)
                {
                    int contador = 1, idPagoEsporadico = 1;
                    foreach (DataRow row in dtRegistros.Rows)
                    {
                        DataTable dtCobranza = null;
                        RecibosEntitys recibo = new RecibosEntitys();
                        bool esPagoMultiple = false;
                        decimal tasaIVA=0;
                        recibo.Contador = contador;
                        recibo.IdPagoCobranza = 0;
                        //Se agrega manejo de recibos esporadicos en polizas a partir de la version 3.3.3.1 byUz
                        bool esporadico = row["P2443_CLAVE_ULT_MOV"].ToString() == "RECIBOESPORADICO" ? true : false;
                        recibo.EsEsporadico = esporadico;
                        if (esporadico)
                        {
                            //Los recibos esporadicos no generan movimientos de pago en Cobranza.Pago(SQL Server)
                            recibo.IdPagoCobranza = idPagoEsporadico;
                            idPagoEsporadico++;
                        }
                            
                        if (cobranzaEnt.Version >= 2212 && condiciones.TipoPoliza == 2 && condiciones.FechaInicio >= cobranzaEnt.FechaAct)
                        { //Validando version de cobranza
                            
                            if(!esporadico)
                            {
                                decimal campo5 = string.IsNullOrEmpty(row["CAMPO_NUM5"].ToString()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM5"].ToString());
                                if (campo5 > 0)
                                {
                                    recibo.IdPagoCobranza = Convert.ToInt32(campo5);
                                    dtCobranza = getPagoCobranza(recibo.IdPagoCobranza);   
                                    if (dtCobranza != null)                                
                                    {                                    
                                        if (dtCobranza.Rows.Count > 0)
                                        {
                                            recibo.PagoTotal = Convert.ToDecimal(dtCobranza.Rows[0]["Total"].ToString());
                                            recibo.IVATotalCobrado = Convert.ToDecimal(dtCobranza.Rows[0]["IVATotalCobrado"].ToString());
                                            esPagoMultiple = true;
                                        }
                                        else
                                        {
                                            recibo.PagoTotal = 0M;
                                            recibo.IVATotalCobrado = 0M;
                                        }                                                                   
                                    }
                                }
                                else
                                {
                                    recibo.IdPagoCobranza = 0;
                                    dtCobranza = null;
                                    recibo.PagoTotal = 0M;
                                    recibo.IVATotalCobrado = 0M;
                                }                                                               
                            }
                        }
                        recibo.RazonSocial = row["P0103_RAZON_SOCIAL"].ToString().Trim();
                        recibo.RFC = row["P0106_RFC"].ToString().Trim();
                        recibo.Identificador = Convert.ToInt32(row["P2444_ID_HIST_REC"].ToString().Trim());
                        //Linea agregada para dar solución a la problemática de cargos periodicos en las Notas de Crédito
                        //by Ing. Uzcanga 28/08/2015
                        //Se modifica para validar si el valor es null by Uz 10/12/2015
                        if (condiciones.TipoPoliza == 4)
                        {
                            recibo.IdRecRelNC = string.IsNullOrEmpty(row["P2425_DEB_REC"].ToString()) ? 0 : Convert.ToInt32(row["P2425_DEB_REC"].ToString().Trim());
                        }
                        recibo.Arrendadora = row["P2401_ID_ARRENDADORA"].ToString().Trim();
                        recibo.Cliente = row["P2402_ID_ARRENDATARIO"].ToString().Trim();
                        recibo.Contrato = row["P2418_ID_CONTRATO"].ToString().Trim();
                        //recibo.Importe = Convert.ToDecimal(row["P2405_IMPORTE"].ToString().Trim());
                        recibo.ConceptoRecibo = row["P2412_CONCEPTO"].ToString().Trim();
                        
                        if(!condiciones.ClienteEnLugarDePeriodo)
                            recibo.Concepto = row["P4006_SERIE"].ToString().Trim() + "-" + row["P4007_FOLIO"].ToString().Trim() + " " + row["CAMPO9"].ToString().Trim() + " " + row["P2404_PERIODO"].ToString().Trim();
                        else
                            recibo.Concepto = row["P4006_SERIE"].ToString().Trim() + "-" + row["P4007_FOLIO"].ToString().Trim() + " " + row["CAMPO9"].ToString().Trim() + " " + row["P0203_NOMBRE"].ToString().Trim();
                        
                        recibo.SerieFolio = row["P4006_SERIE"].ToString().Trim() + "-" + row["P4007_FOLIO"].ToString().Trim();
                        recibo.Periodo = row["P2404_PERIODO"].ToString().Trim();
                        recibo.ClienteNombreComercial = row["P0253_NOMBRE_COMERCIAL"].ToString().Trim();
                        recibo.Moneda = row["P2410_MONEDA"].ToString().Trim();
                        
                        recibo.TipoDocumento = row["P2426_TIPO_DOC"].ToString().Trim();
                        recibo.IVA = Convert.ToDecimal(row["P2416_IVA"].ToString().Trim());
                        string claveTasaIVA = string.Empty;
                        if (condiciones.TipoPoliza == 2)
                        {
                           claveTasaIVA = row["P2428_T_IVA"].ToString().Trim();
                        }
                        if (condiciones.TipoPoliza == 2)
                        {
                            //Linea agregada a partir de verion 3.3.1.9
                            recibo.MonedaPago = row["P2420_MONEDA_PAGO"].ToString().Trim();
                            //Tipo de cambio de pago para polizas de ingresos
                            recibo.TipoDeCambio =string.IsNullOrEmpty(row["P2421_TC_PAGO"].ToString().Trim()) ? 1.0m : Convert.ToDecimal(row["P2421_TC_PAGO"].ToString().Trim());
                        }
                        else
                        {
                            //Tipo de cambio de emision para polizas diferentes a ingresos
                            recibo.TipoDeCambio = string.IsNullOrEmpty(row["P2414_TIPO_CAMBIO"].ToString().Trim()) ? 1.0m : Convert.ToDecimal(row["P2414_TIPO_CAMBIO"].ToString().Trim());
                            recibo.MonedaPago =  recibo.Moneda  ;
                        }
                        //pago completo
                        if (recibo.TipoDocumento == "R" && recibo.Moneda=="D" && recibo.MonedaPago=="P" && cobranzaEnt.Version >= 2212 && condiciones.TipoPoliza == 2 && condiciones.FechaInicio >= cobranzaEnt.FechaAct)
                        {
                            //recibo.IVA = Convert.ToDecimal(row["P2416_IVA"].ToString().Trim()) * recibo.TipoDeCambio;
                            recibo.Importe =decimal.Round(Convert.ToDecimal(row["P2405_IMPORTE"].ToString().Trim()) * recibo.TipoDeCambio, 4);
                            
                        }
                        else//PARCIAL O MULTIPLE
                        {
                            //recibo.IVA = Convert.ToDecimal(row["P2416_IVA"].ToString().Trim());
                            recibo.Importe = Convert.ToDecimal(row["P2405_IMPORTE"].ToString().Trim());
                            if (esPagoMultiple)
                            {
                                int numRec = Convert.ToInt32(row["P2403_NUM_RECIBO"].ToString());
                                //string sql = "SELECT TotalPagado, IVAcobrado, RetISRTotalCobrado, RetIVATotalCobrado FROM Cobranza.ReciboPagado WHERE IDPago= @IdP AND IDHistRec=@IdR";
                                DataTable dtReciboCobrando = getTotalesPagoReciboCobranza(recibo.IdPagoCobranza, numRec/*recibo.Identificador*/);
                                decimal retIsrP = Convert.ToDecimal(0);
                                decimal retIVAP = Convert.ToDecimal(0);
                                if (dtReciboCobrando != null)
                                {
                                    if (dtReciboCobrando.Rows.Count > 0)
                                    {
                                        //TotalPagado, IVAcobrado, RetISRTotalCobrado, RetIVATotalCobrado
                                        recibo.Importe = Convert.ToDecimal(dtReciboCobrando.Rows[0]["TotalPagado"].ToString());
                                        recibo.IVA = Convert.ToDecimal(dtReciboCobrando.Rows[0]["IVAcobrado"].ToString());
                                       
                                        try
                                        {
                                            retIsrP = Convert.ToDecimal(dtReciboCobrando.Rows[0]["RetISRTotalCobrado"].ToString());
                                        }
                                        catch { }
                                        try
                                        {
                                            retIVAP = Convert.ToDecimal(dtReciboCobrando.Rows[0]["RetIVATotalCobrado"].ToString());
                                        }
                                        catch { }
                                        //Se recalcula el importe para obtener el subtotal y no afectar las formulas
                                        recibo.Importe = recibo.Importe - recibo.IVA + retIsrP + retIVAP;
                                        esPagoMultiple = true;
                                    }
                                    else
                                    {
                                        recibo.Importe = 0M;
                                        recibo.IVATotalCobrado = 0M;
                                    }
                                }
                                //recibo.Importe = Polizas.getPagoReciboCobranza(recibo.IdPagoCobranza, numRec);                                

                                tasaIVA = getTasaIVA(claveTasaIVA);
                                //recibo.IVA = recibo.Importe - (recibo.Importe / (1 + tasaIVA));
                                //recibo.Importe = recibo.Importe - recibo.IVA;
                                recibo.RetencionISR = retIsrP;
                                recibo.RetencionIVA = retIVAP;
                            }
                            else
                            {
                                recibo.RetencionISR = string.IsNullOrEmpty(row["CAMPO_NUM1"].ToString().Trim()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM1"].ToString().Trim());
                                recibo.RetencionIVA = string.IsNullOrEmpty(row["CAMPO_NUM2"].ToString().Trim()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM2"].ToString().Trim());
                            }




                        }
                       if (recibo.EsEsporadico)
                        {
                            recibo.PagoTotal = recibo.Importe + recibo.IVA;
                            recibo.IVATotalCobrado = recibo.IVA;
                        }
                        recibo.FechaEmision = Convert.ToDateTime(row["P2409_FECHA_EMISION"].ToString().Trim());
                        //recibo.RetencionIVA = string.IsNullOrEmpty(row["CAMPO_NUM1"].ToString().Trim()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM1"].ToString().Trim());
                        //recibo.RetencionISR = string.IsNullOrEmpty(row["CAMPO_NUM2"].ToString().Trim()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM2"].ToString().Trim());
                       
                        
                        if (condiciones.TipoPoliza == 1)
                        {
                            recibo.FechaMovimiento = Convert.ToDateTime(row["P2409_FECHA_EMISION"].ToString().Trim());
                        }
                        else
                        {
                            recibo.FechaMovimiento = Convert.ToDateTime(row["P2408_FECHA_PAGADO"].ToString().Trim());
                        }

                        recibo.NumeroRecibo = Convert.ToInt32(row["P2403_NUM_RECIBO"].ToString().Trim());
                        recibo.SerieFolio = row["P4006_SERIE"].ToString().Trim() + "_" + row["P4007_FOLIO"].ToString().Trim();
                        
                        if (Convert.ToInt32(row["P2406_STATUS"].ToString()) == 1)
                            recibo.Estatus = "Pendiente de pago";
                        else if (Convert.ToInt32(row["P2406_STATUS"].ToString()) == 2)
                            recibo.Estatus = "Pagado";
                        else
                            recibo.Estatus = "Cancelado";
                        
                        recibo.ClienteRazonSocial = row["P0203_NOMBRE"].ToString().Trim();
                        recibo.ClienteRFC = row["P0204_RFC"].ToString().Trim();
                        recibo.Conjunto = row["CAMPO18"].ToString().Trim();
                        recibo.Inmueble = row["CAMPO9"].ToString().Trim();
                        if (recibo.EsEsporadico)
                        {
                            recibo.TotalCargos = recibo.Importe + recibo.IVA;
                        }
                        else
                            recibo.TotalCargos = string.IsNullOrEmpty(row["CAMPO_NUM6"].ToString()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM6"]);
                        
                        recibo.IdentificadorConjunto = row["CAMPO4"].ToString().Trim();
                        recibo.IDCFD = string.IsNullOrEmpty(row["P4002_ID_CFD"].ToString()) ? 0 : Convert.ToInt32(row["P4002_ID_CFD"]);
                        
                        if(recibo.IDCFD != 0)
                            recibo.UUID = getUUID(recibo.IDCFD);

                        recibo.RFC = row["P0204_RFC"] == DBNull.Value ? string.Empty : row["P0204_RFC"].ToString();

                        recibo.Descuento = string.IsNullOrEmpty(row["P2424_DESCUENTO"].ToString().Trim()) ? 0 : Convert.ToDecimal(row["P2424_DESCUENTO"]);
                        recibo.TipoContable = row["CAMPO20"].ToString().Trim();

                        recibo.Sucursal = row["P4003_SUCURSAL"].ToString();
                        recibo.SaldoAFavorGenerado = string.IsNullOrEmpty(row["CAMPO_NUM14"].ToString()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM14"]);
                        recibo.SaldoAFavorAplicado = getPagosConSaldo(recibo.NumeroRecibo, recibo.IdPagoCobranza);
                        recibo.Sucursal = row["CAMPO5"].ToString();
                        //recibo.FechaPagado = string.IsNullOrEmpty(row["P2408_FECHA_PAGADO"].ToString().Trim()) ? new DateTime() : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                        listaRecibos.Add(recibo);
                        contador++;
                    }
                }
                string esCopropiedad = getTipoInmobiliariaByID(condiciones.Inmobiliaria);
                if (esCopropiedad == "S" && listaRecibos.Count>0)
                {
                    DataTable dtEdificiosCopropiedad = getEdificiosEnCopropiedad(condiciones.Inmobiliaria);
                    List<RecibosEntitys> listaRec= new List<RecibosEntitys>();
                    bool esEdifCoprop = false;
                    if (dtEdificiosCopropiedad.Rows.Count > 0)
                    {
                        foreach (RecibosEntitys rec in listaRecibos)
                        {
                            esEdifCoprop = false;
                            foreach (DataRow rowEdif in dtEdificiosCopropiedad.Rows)
                            {
                                if (rowEdif["P4401_ID_EDIFICIO"].ToString() == rec.IdEdificio)
                                {

                                    esEdifCoprop = true;
                                    break;
                                }
                            }
                            if (!esEdifCoprop)
                                listaRec.Add(rec);
                        }
                    }
                    else
                        listaRec = listaRecibos;
                    if(condiciones.ExcluirConceptoLibre)
                        return listaRec.Where(r => !r.EsConceptoLibre).ToList();
                    else
                        return listaRec;
                }                   
                if (condiciones.ExcluirConceptoLibre)
                    return listaRecibos.Where(r => !r.EsConceptoLibre).ToList();
                else
                    return listaRecibos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return new List<RecibosEntitys>();
            }            
        }

        public static List<RecibosEntitys> getRecibosVentas(CondicionesEntity condiciones)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            List<RecibosEntitys> listaRecibos = new List<RecibosEntitys>();
            ModuloCobranzaEntity cobranzaEnt = new ModuloCobranzaEntity();
            try
            {
                string sql = string.Empty;
                //fechaActPol = SaariDB.getFechaAct();
                cobranzaEnt = SaariDB.getVersionCobranza();
                if (condiciones.TipoPoliza == 6)
                {
                    //Sentencia SQL para polizas de tipo de ingresos
                    //Se modifica el campo P2414_TIPO_CAMBIO POR EL P2421_TC_PAGO
                    //T24_HISTORIA_RECIBOS.CAMPO_NUM5 HACE REFERENCIA AL IDPAGO EN SAARI.PAGO
                    //Se agrega T24_HISTORIA_RECIBOS.P2443_CLAVE_ULT_MOV para validar si es RECIBO ESPORADICO V3.3.3.1
                    sql = @"SELECT T24_HISTORIA_RECIBOS.CAMPO_NUM5, P0103_RAZON_SOCIAL, P2444_ID_HIST_REC, P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2405_IMPORTE, P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.CAMPO5,
                                P2412_CONCEPTO, P2410_MONEDA, P2416_IVA, P2421_TC_PAGO, P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2443_CLAVE_ULT_MOV, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, 
                                T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.CAMPO20, T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.CAMPO_NUM1, T24_HISTORIA_RECIBOS.CAMPO_NUM2, P2413_PAGO,
                                P2408_FECHA_PAGADO, P2403_NUM_RECIBO, P2406_STATUS, T02_ARRENDATARIO.P0203_NOMBRE, P0204_RFC, P0106_RFC, T24_HISTORIA_RECIBOS.CAMPO18, T24_HISTORIA_RECIBOS.P2428_T_IVA, 
                                T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.CAMPO_NUM6,
                                T40_CFD.P4002_ID_CFD, T03_CENTRO_INDUSTRIAL.CAMPO4, T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T40_CFD.P4003_SUCURSAL, T24_HISTORIA_RECIBOS.CAMPO_NUM14
                                FROM T24_HISTORIA_RECIBOS JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
                                LEFT JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                                JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
                                LEFT JOIN T03_CENTRO_INDUSTRIAL ON T24_HISTORIA_RECIBOS.CAMPO4 = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO 
                                WHERE P2401_ID_ARRENDADORA = ? AND P2408_FECHA_PAGADO >= ? AND P2408_FECHA_PAGADO <= ? AND P2426_TIPO_DOC IN('V') ";
                    if (!condiciones.IncluirCancelados)
                    {
                        sql += "AND P2406_STATUS = '2' ";
                    }                  
                    //sql += @"UNION ALL SELECT T24_HISTORIA_RECIBOS.CAMPO_NUM5, P0103_RAZON_SOCIAL, P2444_ID_HIST_REC, P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2405_IMPORTE, P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.CAMPO5, 
                    //            P2412_CONCEPTO, P2410_MONEDA, P2416_IVA, P2414_TIPO_CAMBIO, P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2443_CLAVE_ULT_MOV, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, 
                    //            T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.CAMPO20, T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.CAMPO_NUM1, T24_HISTORIA_RECIBOS.CAMPO_NUM2, P2413_PAGO,
                    //            P2408_FECHA_PAGADO, P2403_NUM_RECIBO, P2406_STATUS, T02_ARRENDATARIO.P0203_NOMBRE, P0204_RFC, P0106_RFC, T24_HISTORIA_RECIBOS.CAMPO18, T24_HISTORIA_RECIBOS.P2428_T_IVA,   
                    //            T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.CAMPO_NUM6, 
                    //            T40_CFD.P4002_ID_CFD, T03_CENTRO_INDUSTRIAL.CAMPO4, T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T40_CFD.P4003_SUCURSAL, T24_HISTORIA_RECIBOS.CAMPO_NUM14
                    //            FROM T24_HISTORIA_RECIBOS JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
                    //            LEFT JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO = T40_CFD.P4001_ID_HIST_REC
                    //            JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
                    //            LEFT JOIN T03_CENTRO_INDUSTRIAL ON T24_HISTORIA_RECIBOS.CAMPO4 = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO 
                    //            WHERE P2401_ID_ARRENDADORA = ? AND P2408_FECHA_PAGADO >= ? AND P2408_FECHA_PAGADO <= ? AND P2426_TIPO_DOC IN('P','U') ";
                    //if (!condiciones.IncluirCancelados)
                    //{
                    //    sql += "AND P2406_STATUS = '2' ";
                    //}
                    //else
                    //{
                    //    sql += "AND P2406_STATUS <> '1' ";
                    //}
                }
                else if (condiciones.TipoPoliza == 5)
                {
                    sql = @"SELECT P0103_RAZON_SOCIAL, P2444_ID_HIST_REC, P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2405_IMPORTE, T24_HISTORIA_RECIBOS.CAMPO5,  
                                P2412_CONCEPTO, P2410_MONEDA, P2416_IVA, P2414_TIPO_CAMBIO, P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, 
                                T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.CAMPO20, T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.CAMPO_NUM1, T24_HISTORIA_RECIBOS.CAMPO_NUM2, 
                                P2403_NUM_RECIBO, P2406_STATUS, T02_ARRENDATARIO.P0203_NOMBRE, P0204_RFC, P0106_RFC, T24_HISTORIA_RECIBOS.CAMPO18, T24_HISTORIA_RECIBOS.P2443_CLAVE_ULT_MOV, 
                                T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.CAMPO_NUM6, 
                                T40_CFD.P4002_ID_CFD, T03_CENTRO_INDUSTRIAL.CAMPO4, T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T40_CFD.P4003_SUCURSAL, T24_HISTORIA_RECIBOS.CAMPO_NUM14     
                                FROM T24_HISTORIA_RECIBOS JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
                                LEFT JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                                JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
                                LEFT JOIN T03_CENTRO_INDUSTRIAL ON T24_HISTORIA_RECIBOS.CAMPO4 = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO 
                                WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION >= ? AND P2409_FECHA_EMISION <= ? AND ((P2426_TIPO_DOC = 'V' )) ";
                    if (!condiciones.IncluirCancelados)
                    {
                        sql += "AND P2406_STATUS <> '3' ";
                    }
                }
                else if (condiciones.TipoPoliza == 7)
                {
                    sql = @"SELECT P0103_RAZON_SOCIAL, P2444_ID_HIST_REC, P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2405_IMPORTE, T24_HISTORIA_RECIBOS.CAMPO5,
                                P2412_CONCEPTO, P2410_MONEDA, P2416_IVA, P2414_TIPO_CAMBIO, P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2443_CLAVE_ULT_MOV, 
                                T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.CAMPO20, T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.CAMPO_NUM1, T24_HISTORIA_RECIBOS.CAMPO_NUM2, 
                                P2408_FECHA_PAGADO, P2403_NUM_RECIBO, P2406_STATUS, T02_ARRENDATARIO.P0203_NOMBRE, P0204_RFC, P0106_RFC, T24_HISTORIA_RECIBOS.CAMPO18, 
                                T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.CAMPO_NUM6, 
                                T40_CFD.P4002_ID_CFD, T03_CENTRO_INDUSTRIAL.CAMPO4, , T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T40_CFD.P4003_SUCURSAL, T24_HISTORIA_RECIBOS.CAMPO_NUM14  
                                FROM T24_HISTORIA_RECIBOS JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
                                LEFT JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                                JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
                                LEFT JOIN T03_CENTRO_INDUSTRIAL ON T24_HISTORIA_RECIBOS.CAMPO4 = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO 
                                WHERE P2401_ID_ARRENDADORA = ? AND P2408_FECHA_PAGADO >= ? AND P2408_FECHA_PAGADO <= ? AND P2426_TIPO_DOC = 'V' AND P2406_STATUS = '3'";
                }
                //else
                //{   //se agrea P2425_DEB_REC a la consulta para obtener el id de la factura relacionada a la NC
                //    sql = @"SELECT P0103_RAZON_SOCIAL, P2444_ID_HIST_REC, P2425_DEB_REC, P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2405_IMPORTE, T24_HISTORIA_RECIBOS.CAMPO5,
                //                P2412_CONCEPTO, P2410_MONEDA, P2416_IVA, P2414_TIPO_CAMBIO, P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2443_CLAVE_ULT_MOV, 
                //                T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.CAMPO20, T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.CAMPO_NUM1, T24_HISTORIA_RECIBOS.CAMPO_NUM2, 
                //                P2408_FECHA_PAGADO, P2403_NUM_RECIBO, P2406_STATUS, T02_ARRENDATARIO.P0203_NOMBRE, P0204_RFC, P0106_RFC, T24_HISTORIA_RECIBOS.CAMPO18, 
                //                T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.CAMPO_NUM6, 
                //                T40_CFD.P4002_ID_CFD, T03_CENTRO_INDUSTRIAL.CAMPO4, T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T40_CFD.P4003_SUCURSAL, T24_HISTORIA_RECIBOS.CAMPO_NUM14  
                //                FROM T24_HISTORIA_RECIBOS JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
                //                LEFT JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                //                JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
                //                LEFT JOIN T03_CENTRO_INDUSTRIAL ON T24_HISTORIA_RECIBOS.CAMPO4 = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO 
                //                WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION >= ? AND P2409_FECHA_EMISION <= ? AND P2426_TIPO_DOC = 'B'";
                //    if (!condiciones.IncluirCancelados)
                //    {
                //        sql += "AND P2406_STATUS <> '3' ";
                //    }
                //}

                if (cobranzaEnt.Version >= 2212 && condiciones.TipoPoliza == 2 && condiciones.FechaInicio.Date > cobranzaEnt.FechaAct.Date)
                {
                    sql += "ORDER BY 1,3";
                }
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idarr", OdbcType.NVarChar).Value = condiciones.Inmobiliaria;
                comando.Parameters.Add("@fechaini", OdbcType.Date).Value = condiciones.FechaInicio;
                if (condiciones.Formato == FormatoExportacionPoliza.Excel || condiciones.TipoPresentacion == 3)
                    comando.Parameters.Add("@fechafin", OdbcType.Date).Value = condiciones.FechaFin;
                else
                {
                    if (condiciones.PolizaPorRango)
                        comando.Parameters.Add("@fechafin", OdbcType.Date).Value = condiciones.FechaFin;
                    else
                        comando.Parameters.Add("@fechafin", OdbcType.Date).Value = condiciones.FechaInicio;
                    //comando.Parameters.Add("@fechafin", OdbcType.Date).Value = condiciones.FechaInicio;
                }
                if (condiciones.TipoPoliza == 6)
                {
                    comando.Parameters.Add("@idarr2", OdbcType.NVarChar).Value = condiciones.Inmobiliaria;
                    comando.Parameters.Add("@fechaini2", OdbcType.Date).Value = condiciones.FechaInicio;
                    if (condiciones.Formato == FormatoExportacionPoliza.Excel || condiciones.TipoPresentacion == 3)
                        comando.Parameters.Add("@fechafin", OdbcType.Date).Value = condiciones.FechaFin;
                    else
                    {
                        if (condiciones.PolizaPorRango)
                            comando.Parameters.Add("@fechafin2", OdbcType.Date).Value = condiciones.FechaFin;
                        else
                            comando.Parameters.Add("@fechafin2", OdbcType.Date).Value = condiciones.FechaInicio;
                    }
                }


                DataTable dtRegistros = new DataTable();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtRegistros.Load(reader);
                conexion.Close();
                if (dtRegistros.Rows.Count > 0)
                {
                    int contador = 1, idPagoEsporadico = 1;
                    foreach (DataRow row in dtRegistros.Rows)
                    {
                        DataTable dtCobranza = null;
                        RecibosEntitys recibo = new RecibosEntitys();
                        bool esPagoMultiple = false;
                        decimal tasaIVA = 0;
                        recibo.Contador = contador;
                        recibo.IdPagoCobranza = 0;
                        //Se agrega manejo de recibos esporadicos en polizas a partir de la version 3.3.3.1 byUz
                        bool esporadico = row["P2443_CLAVE_ULT_MOV"].ToString() == "RECIBOESPORADICO" ? true : false;
                        recibo.EsEsporadico = esporadico;
                        if (esporadico)
                        {
                            //Los recibos esporadicos no generan movimientos de pago en Cobranza.Pago(SQL Server)
                            recibo.IdPagoCobranza = idPagoEsporadico;
                            idPagoEsporadico++;
                        }

                        if (cobranzaEnt.Version >= 2212 && condiciones.TipoPoliza == 6 && condiciones.FechaInicio >= cobranzaEnt.FechaAct)
                        { //Validando version de cobranza

                            if (!esporadico)
                            {
                                decimal campo5 = string.IsNullOrEmpty(row["CAMPO_NUM5"].ToString()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM5"].ToString());
                                if (campo5 > 0)
                                {
                                    recibo.IdPagoCobranza = Convert.ToInt32(campo5);
                                    dtCobranza = getPagoCobranza(recibo.IdPagoCobranza);
                                    if (dtCobranza != null)
                                    {
                                        if (dtCobranza.Rows.Count > 0)
                                        {
                                            recibo.PagoTotal = Convert.ToDecimal(dtCobranza.Rows[0]["Total"].ToString());
                                            recibo.IVATotalCobrado = Convert.ToDecimal(dtCobranza.Rows[0]["IVATotalCobrado"].ToString());
                                            esPagoMultiple = true;
                                        }
                                        else
                                        {
                                            recibo.PagoTotal = 0M;
                                            recibo.IVATotalCobrado = 0M;
                                        }
                                    }
                                }
                                else
                                {
                                    recibo.IdPagoCobranza = 0;
                                    dtCobranza = null;
                                    recibo.PagoTotal = 0M;
                                    recibo.IVATotalCobrado = 0M;
                                }
                            }
                        }
                        recibo.RazonSocial = row["P0103_RAZON_SOCIAL"].ToString().Trim();
                        recibo.RFC = row["P0106_RFC"].ToString().Trim();
                        recibo.Identificador = Convert.ToInt32(row["P2444_ID_HIST_REC"].ToString().Trim());
                        //Linea agregada para dar solución a la problemática de cargos periodicos en las Notas de Crédito
                        //by Ing. Uzcanga 28/08/2015
                        //Se modifica para validar si el valor es null by Uz 10/12/2015
                        if (condiciones.TipoPoliza == 8)
                        {
                            recibo.IdRecRelNC = string.IsNullOrEmpty(row["P2425_DEB_REC"].ToString()) ? 0 : Convert.ToInt32(row["P2425_DEB_REC"].ToString().Trim());
                        }
                        recibo.Arrendadora = row["P2401_ID_ARRENDADORA"].ToString().Trim();
                        recibo.Cliente = row["P2402_ID_ARRENDATARIO"].ToString().Trim();
                        recibo.Contrato = row["P2418_ID_CONTRATO"].ToString().Trim();
                        //recibo.Importe = Convert.ToDecimal(row["P2405_IMPORTE"].ToString().Trim());
                        recibo.ConceptoRecibo = row["P2412_CONCEPTO"].ToString().Trim();

                        if (!condiciones.ClienteEnLugarDePeriodo)
                            recibo.Concepto = row["P4006_SERIE"].ToString().Trim() + "-" + row["P4007_FOLIO"].ToString().Trim() + " " + row["CAMPO9"].ToString().Trim() + " " + row["P2404_PERIODO"].ToString().Trim();
                        else
                            recibo.Concepto = row["P4006_SERIE"].ToString().Trim() + "-" + row["P4007_FOLIO"].ToString().Trim() + " " + row["CAMPO9"].ToString().Trim() + " " + row["P0203_NOMBRE"].ToString().Trim();

                        recibo.SerieFolio = row["P4006_SERIE"].ToString().Trim() + "-" + row["P4007_FOLIO"].ToString().Trim();
                        recibo.Periodo = row["P2404_PERIODO"].ToString().Trim();
                        recibo.ClienteNombreComercial = row["P0253_NOMBRE_COMERCIAL"].ToString().Trim();
                        recibo.Moneda = row["P2410_MONEDA"].ToString().Trim();

                        recibo.TipoDocumento = row["P2426_TIPO_DOC"].ToString().Trim();
                        recibo.IVA = Convert.ToDecimal(row["P2416_IVA"].ToString().Trim());
                        string claveTasaIVA = string.Empty;
                        if (condiciones.TipoPoliza == 6)
                        {
                            row["P2428_T_IVA"].ToString().Trim();
                        }
                        if (condiciones.TipoPoliza == 6)
                        {
                            //Linea agregada a partir de verion 3.3.1.9
                            recibo.MonedaPago = row["P2420_MONEDA_PAGO"].ToString().Trim();
                            //Tipo de cambio de pago para polizas de ingresos
                            recibo.TipoDeCambio = string.IsNullOrEmpty(row["P2421_TC_PAGO"].ToString().Trim()) ? 1.0m : Convert.ToDecimal(row["P2421_TC_PAGO"].ToString().Trim());
                        }
                        else
                        {
                            //Tipo de cambio de emision para polizas diferentes a ingresos
                            recibo.TipoDeCambio = string.IsNullOrEmpty(row["P2414_TIPO_CAMBIO"].ToString().Trim()) ? 1.0m : Convert.ToDecimal(row["P2414_TIPO_CAMBIO"].ToString().Trim());
                        }
                        if (recibo.TipoDocumento == "R" && recibo.Moneda == "D" && recibo.MonedaPago == "P" && cobranzaEnt.Version >= 2212 && condiciones.TipoPoliza == 2 && condiciones.FechaInicio >= cobranzaEnt.FechaAct)
                        {
                            //recibo.IVA = Convert.ToDecimal(row["P2416_IVA"].ToString().Trim()) * recibo.TipoDeCambio;
                            recibo.Importe = decimal.Round(Convert.ToDecimal(row["P2405_IMPORTE"].ToString().Trim()) * recibo.TipoDeCambio, 4);

                        }
                        else
                        {
                            //recibo.IVA = Convert.ToDecimal(row["P2416_IVA"].ToString().Trim());
                            recibo.Importe = Convert.ToDecimal(row["P2405_IMPORTE"].ToString().Trim());
                            if (!esPagoMultiple)
                            {
                                recibo.Importe = Convert.ToDecimal(row["P2405_IMPORTE"].ToString().Trim());
                            }
                            else
                            {
                                int numRec = Convert.ToInt32(row["P2403_NUM_RECIBO"].ToString());
                                recibo.Importe = Polizas.getPagoReciboCobranza(recibo.IdPagoCobranza, numRec);

                                tasaIVA = getTasaIVA(claveTasaIVA);
                                recibo.IVA = recibo.Importe - (recibo.Importe / (1 + tasaIVA));
                                recibo.Importe = recibo.Importe - recibo.IVA;

                            }
                        }
                        if (recibo.EsEsporadico)
                        {
                            recibo.PagoTotal = recibo.Importe + recibo.IVA;
                            recibo.IVATotalCobrado = recibo.IVA;
                        }
                        recibo.FechaEmision = Convert.ToDateTime(row["P2409_FECHA_EMISION"].ToString().Trim());
                        //recibo.RetencionIVA = string.IsNullOrEmpty(row["CAMPO_NUM1"].ToString().Trim()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM1"].ToString().Trim());
                        //recibo.RetencionISR = string.IsNullOrEmpty(row["CAMPO_NUM2"].ToString().Trim()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM2"].ToString().Trim());
                        recibo.RetencionISR = string.IsNullOrEmpty(row["CAMPO_NUM1"].ToString().Trim()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM1"].ToString().Trim());
                        recibo.RetencionIVA = string.IsNullOrEmpty(row["CAMPO_NUM2"].ToString().Trim()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM2"].ToString().Trim());

                        if (condiciones.TipoPoliza == 5)
                        {
                            recibo.FechaMovimiento = Convert.ToDateTime(row["P2409_FECHA_EMISION"].ToString().Trim());
                        }
                        else
                        {
                            recibo.FechaMovimiento = Convert.ToDateTime(row["P2408_FECHA_PAGADO"].ToString().Trim());
                        }

                        recibo.NumeroRecibo = Convert.ToInt32(row["P2403_NUM_RECIBO"].ToString().Trim());
                        recibo.SerieFolio = row["P4006_SERIE"].ToString().Trim() + "_" + row["P4007_FOLIO"].ToString().Trim();

                        if (Convert.ToInt32(row["P2406_STATUS"].ToString()) == 5)
                            recibo.Estatus = "Pendiente de pago";
                        else if (Convert.ToInt32(row["P2406_STATUS"].ToString()) == 6)
                            recibo.Estatus = "Pagado";
                        else
                            recibo.Estatus = "Cancelado";

                        recibo.ClienteRazonSocial = row["P0203_NOMBRE"].ToString().Trim();
                        recibo.ClienteRFC = row["P0204_RFC"].ToString().Trim();
                        recibo.Conjunto = row["CAMPO18"].ToString().Trim();
                        recibo.Inmueble = row["CAMPO9"].ToString().Trim();
                        if (recibo.EsEsporadico)
                        {
                            recibo.TotalCargos = recibo.Importe + recibo.IVA;
                        }
                        else
                            recibo.TotalCargos = string.IsNullOrEmpty(row["CAMPO_NUM6"].ToString()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM6"]);

                        recibo.IdentificadorConjunto = row["CAMPO4"].ToString().Trim();
                        recibo.IDCFD = string.IsNullOrEmpty(row["P4002_ID_CFD"].ToString()) ? 0 : Convert.ToInt32(row["P4002_ID_CFD"]);

                        if (recibo.IDCFD != 0)
                            recibo.UUID = getUUID(recibo.IDCFD);
                        if (recibo.IDCFD != 0)
                            recibo.RFC = getRFC(recibo.IDCFD);

                        recibo.Descuento = string.IsNullOrEmpty(row["P2424_DESCUENTO"].ToString().Trim()) ? 0 : Convert.ToDecimal(row["P2424_DESCUENTO"]);
                        recibo.TipoContable = row["CAMPO20"].ToString().Trim();

                        recibo.Sucursal = row["P4003_SUCURSAL"].ToString();
                        recibo.SaldoAFavorGenerado = string.IsNullOrEmpty(row["CAMPO_NUM14"].ToString()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM14"]);
                        recibo.SaldoAFavorAplicado = getPagosConSaldo(recibo.NumeroRecibo, recibo.IdPagoCobranza);
                        recibo.Sucursal = row["CAMPO5"].ToString();
                        //recibo.FechaPagado = string.IsNullOrEmpty(row["P2408_FECHA_PAGADO"].ToString().Trim()) ? new DateTime() : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                        listaRecibos.Add(recibo);
                        contador++;
                    }
                }
                string esCopropiedad = getTipoInmobiliariaByID(condiciones.Inmobiliaria);
                if (esCopropiedad == "S" && listaRecibos.Count > 0)
                {
                    DataTable dtEdificiosCopropiedad = getEdificiosEnCopropiedad(condiciones.Inmobiliaria);
                    List<RecibosEntitys> listaRec = new List<RecibosEntitys>();
                    bool esEdifCoprop = false;
                    if (dtEdificiosCopropiedad.Rows.Count > 0)
                    {
                        foreach (RecibosEntitys rec in listaRecibos)
                        {
                            esEdifCoprop = false;
                            foreach (DataRow rowEdif in dtEdificiosCopropiedad.Rows)
                            {
                                if (rowEdif["P4401_ID_EDIFICIO"].ToString() == rec.IdEdificio)
                                {

                                    esEdifCoprop = true;
                                    break;
                                }
                            }
                            if (!esEdifCoprop)
                                listaRec.Add(rec);
                        }
                    }
                    else
                        listaRec = listaRecibos;
                    if (condiciones.ExcluirConceptoLibre)
                        return listaRec.Where(r => !r.EsConceptoLibre).ToList();
                    else
                        return listaRec;
                }
                if (condiciones.ExcluirConceptoLibre)
                    return listaRecibos.Where(r => !r.EsConceptoLibre).ToList();
                else
                    return listaRecibos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return new List<RecibosEntitys>();
            }
        }

        private static decimal getTasaIVA(string idTasa)
        {
            decimal tasaIVA = 0;
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT P3002_NUMERADOR, P3003_DENOMINADOR FROM T30_R_IMPUESTOS WHERE P3001_CLAVE = ?";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDTasa", OdbcType.VarChar).Value = idTasa;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    decimal numerador = reader["P3002_NUMERADOR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3002_NUMERADOR"]);
                    decimal denominador = reader["P3003_DENOMINADOR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3003_DENOMINADOR"]);
                    tasaIVA = numerador / denominador;
                }
                reader.Close();
                conexion.Close();
                return tasaIVA;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }
        public static DataTable getEdificiosEnCopropiedad(string idArrendadora)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                DataTable dtEdificios = new DataTable();
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = "SELECT DISTINCT(P4401_ID_EDIFICIO) FROM T44_COPROPIETARIOS WHERE T44_COPROPIETARIOS.P4402_ID_INMOBILIARIA=?";
                comando.Parameters.Add("@idArr", OdbcType.VarChar).Value = idArrendadora;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtEdificios.Load(reader);
                conexion.Close();
                return dtEdificios;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }
        public static string getTipoInmobiliariaByID(string idInmobiliaria)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = "SELECT P0109_STATUS FROM T01_ARRENDADORA WHERE P0101_ID_ARR = ?";
            string tipo = string.Empty;
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    tipo = reader["P0109_STATUS"].ToString();
                    break;
                }
                reader.Close();
                conexion.Close();
                return tipo;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }
        public static FormulaPolizaEntity getFormulaPorTipo(string idArrendadora, int tipoPoliza, string tipoCuenta)
        {
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            FormulaPolizaEntity formula = new FormulaPolizaEntity();
            try
            {
                string sql = "SELECT * FROM MIESLocal.Polizas_Estructura WHERE ID_Arrendadora= @idArr AND TipoPoliza = @tipoPoliza AND TipoCuenta=@tipoCta";
                SqlCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idArr", SqlDbType.NVarChar).Value=  idArrendadora;
                comando.Parameters.Add("@tipoPoliza", SqlDbType.Int).Value= tipoPoliza;
                comando.Parameters.Add("@tipoCta", SqlDbType.NVarChar).Value= tipoCuenta;
                DataTable dtFormula = new DataTable();
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                dtFormula.Load(reader);
                if (dtFormula.Rows.Count > 0)
                {
                    formula.Tipo = tipoCuenta.ToString();
                    formula.TipoClave = dtFormula.Rows[0]["TipoCuenta"].ToString();
                    formula.CargoAbono = Convert.ToInt32(dtFormula.Rows[0]["CargoAbono"].ToString().Trim());
                    formula.Formula = dtFormula.Rows[0]["Formula"].ToString().Trim();
                    formula.Moneda = dtFormula.Rows[0]["Moneda"].ToString().Trim();
                    formula.EsSubtipo = (bool)dtFormula.Rows[0]["EsSubtipo"];
                    formula.TipoPoliza = tipoPoliza;
                    
                }
                return formula;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return new FormulaPolizaEntity();
            }

        }
        public static List<FormulaPolizaEntity> getFormulas(string idArrendadora, int tipoPoliza)
        {
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);            
            List<FormulaPolizaEntity> listaFormulas = new List<FormulaPolizaEntity>();
            try
            {
                string sql = "SELECT * FROM MIESLocal.Polizas_Estructura WHERE ID_Arrendadora = @idarr AND TipoPoliza = @tipopoliza";
                SqlCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idarr", SqlDbType.NVarChar).Value = idArrendadora;
                comando.Parameters.Add("@tipopoliza", SqlDbType.Int).Value = tipoPoliza;
                DataTable dtFormulas = new DataTable();
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                dtFormulas.Load(reader);
                conexion.Close();
                if (dtFormulas.Rows.Count > 0)
                {
                    string monedaPago = string.Empty;
                    foreach (DataRow row in dtFormulas.Rows)
                    {
                        FormulaPolizaEntity formula = new FormulaPolizaEntity();
                        formula.Tipo = row["TipoCuenta"].ToString().Trim();
                        formula.TipoClave = row["TipoCuenta"].ToString().Trim();
                        formula.CargoAbono = Convert.ToInt32(row["CargoAbono"].ToString().Trim());
                        formula.Formula = row["Formula"].ToString().Trim();
                        formula.Moneda = row["Moneda"].ToString().Trim();
                        formula.EsSubtipo = (bool)row["EsSubtipo"];
                        formula.TipoPoliza = tipoPoliza;
                        try
                        {
                            monedaPago = row["MonedaPago"] != DBNull.Value ? row["MonedaPago"].ToString().Trim() : formula.Moneda;
                        }
                        catch (Exception ex)
                        {
                            monedaPago = formula.Moneda;
                        }
                        formula.MonedaPago = monedaPago;
                        listaFormulas.Add(formula);
                    }
                }
                return listaFormulas;
            }
            catch
            {
                conexion.Close();
                return new List<FormulaPolizaEntity>(); 
            }
        }

        public static List<FormulaPolizaEntity> getFormulasEgreso(string idArrendadora, int tipoPoliza)
        {
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            List<FormulaPolizaEntity> listaFormulas = new List<FormulaPolizaEntity>();
            try
            {
                string sql = "SELECT * FROM MIESLocal.PolizasEgreso_Estructura WHERE ID_Arrendadora = @idarr AND TipoPoliza = @tipopoliza";
                SqlCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idarr", SqlDbType.NVarChar).Value = idArrendadora;
                comando.Parameters.Add("@tipopoliza", SqlDbType.Int).Value = tipoPoliza;
                DataTable dtFormulas = new DataTable();
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                dtFormulas.Load(reader);
                conexion.Close();
                if (dtFormulas.Rows.Count > 0)
                {
                    foreach (DataRow row in dtFormulas.Rows)
                    {
                        FormulaPolizaEntity formula = new FormulaPolizaEntity();
                        formula.Tipo = row["TipoCuenta"].ToString().Trim();
                        formula.TipoClave = row["TipoCuenta"].ToString().Trim();
                        formula.CargoAbono = Convert.ToInt32(row["CargoAbono"].ToString().Trim());
                        formula.Formula = row["Formula"].ToString().Trim();
                        listaFormulas.Add(formula);
                    }
                }
                return listaFormulas;
            }
            catch
            {
                conexion.Close();
                return new List<FormulaPolizaEntity>();
            }
        }

        public static string getCuentaByIDEntidadAndTipo(string idEntidad, string tipo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P3202_CUENTA, P3205_DESCRIPCION FROM T32_CUENTA WHERE P3201_ID_ARR = ? AND P3203_TIPO = ?";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idarre", OdbcType.NVarChar).Value = idEntidad;
                comando.Parameters.Add("@tipocuenta", OdbcType.NVarChar).Value = tipo;
                DataTable dtCuenta = new DataTable();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtCuenta.Load(reader);                
                conexion.Close();
                if (dtCuenta.Rows.Count > 0)
                    return dtCuenta.Rows[0]["P3202_CUENTA"].ToString().Trim() + "|" + dtCuenta.Rows[0]["P3205_DESCRIPCION"].ToString().Trim();
                else
                    return string.Empty;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }

        public static string getCuentaByIDEntidadAndSubtipo(string idEntidad, string subtipo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P3202_CUENTA, P3205_DESCRIPCION FROM T32_CUENTA WHERE P3201_ID_ARR = ? AND CAMPO1 = ?";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idarre", OdbcType.NVarChar).Value = idEntidad;
                comando.Parameters.Add("@subtipocuenta", OdbcType.NVarChar).Value = subtipo;
                DataTable dtCuenta = new DataTable();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtCuenta.Load(reader);
                conexion.Close();
                if (dtCuenta.Rows.Count > 0)
                    return dtCuenta.Rows[0]["P3202_CUENTA"].ToString().Trim() + "|" + dtCuenta.Rows[0]["P3205_DESCRIPCION"].ToString().Trim();
                else
                    return string.Empty;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }

        public static DataTable getDtTipos()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = "SELECT DESCR_CAT, TIPO FROM CATEGORIA WHERE ID_COT = 'TIPOCUENTA' ORDER BY DESCR_CAT";
                DataTable dtTipos = new DataTable();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtTipos.Load(reader);
                conexion.Close();
                return dtTipos;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }

        public static DataTable getDtTiposEgreso()
        {
            //OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                /*OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = "SELECT DESCR_CAT, TIPO FROM CATEGORIA WHERE ID_COT = 'CLASGASTO' ORDER BY DESCR_CAT";*/
                DataTable dtTipos = new DataTable();
                DataColumn dcNombre = new DataColumn("DESCR_CAT");
                dtTipos.Columns.Add(dcNombre);
                DataColumn dcTipo = new DataColumn("TIPO");
                dtTipos.Columns.Add(dcTipo);
                dtTipos.AcceptChanges();
                /*conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtTipos.Load(reader);
                conexion.Close();*/

                DataRow rowBancos = dtTipos.NewRow();
                rowBancos[0] = "Bancos";
                rowBancos[1] = "BPE";
                dtTipos.Rows.Add(rowBancos);

                DataRow rowIva = dtTipos.NewRow();
                rowIva[0] = "IVA";
                rowIva[1] = "IVPE";
                dtTipos.Rows.Add(rowIva);
                DataRow rowProv = dtTipos.NewRow();
                rowProv[0] = "Proveedores";
                rowProv[1] = "PPE";
                dtTipos.Rows.Add(rowProv);
                DataRow rowGasto = dtTipos.NewRow();
                rowGasto[0] = "Gasto";
                rowGasto[1] = "GPE";
                dtTipos.Rows.Add(rowGasto);
                //Tipos Agregados by Uz 26/01/2016
                DataRow rowIVPR = dtTipos.NewRow();
                rowIVPR[0] = "IVA Provisionado";
                rowIVPR[1] = "IVPR";
                dtTipos.Rows.Add(rowIVPR);
                DataRow rowIVAC = dtTipos.NewRow();
                rowIVAC[0] = "IVA Acreditado";
                rowIVAC[1] = "IVAC";
                dtTipos.Rows.Add(rowIVAC);
                DataRow rowRIV = dtTipos.NewRow();
                rowRIV[0] = "Retención de IVA";
                rowRIV[1] = "RIV";
                dtTipos.Rows.Add(rowRIV);
                DataRow rowRISR = dtTipos.NewRow();
                rowRISR[0] = "Retención de ISR";
                rowRISR[1] = "RISR";
                dtTipos.Rows.Add(rowRISR);

                EnumerableRowCollection<DataRow> query = from t in dtTipos.AsEnumerable()
                                                         orderby t.Field<string>("DESCR_CAT")
                                                         select t;
                dtTipos = new DataTable();
                dtTipos = query.CopyToDataTable();                
                return dtTipos;
            }
            catch
            {
                //conexion.Close();
                return new DataTable();
            }
        }

        public static string getClasificacionEgreso(string tipo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = "SELECT DESCR_CAT FROM CATEGORIA WHERE ID_COT = 'CLASGASTO' AND TIPO = ? ORDER BY DESCR_CAT";
                comando.Parameters.Add("@tipo", OdbcType.VarChar).Value = tipo;
                string result = string.Empty;
                conexion.Open();
                result = Convert.ToString(comando.ExecuteScalar());                
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }

        public static DataTable getDtSubTipos(string tipo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = "SELECT DESCR_CAT, CAMPO1 FROM CATEGORIA WHERE ID_COT = 'SUBTPCB' AND TIPO = ? AND CAMPO1 IS NOT NULL ORDER BY DESCR_CAT";
                comando.Parameters.Add("@tipo", OdbcType.VarChar).Value = tipo;
                DataTable dtTipos = new DataTable();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtTipos.Load(reader);
                conexion.Close();
                return dtTipos;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }

        public static string getIdentificadorConjuntoByIDContrato(string idContrato)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT T03_CENTRO_INDUSTRIAL.CAMPO4 FROM T04_CONTRATO 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T04_CONTRATO.P0404_ID_EDIFICIO
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T07_EDIFICIO.P0710_ID_CENTRO
WHERE P0401_ID_CONTRATO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@Cont", OdbcType.VarChar).Value = idContrato;
                conexion.Open();
                string result = Convert.ToString(comando.ExecuteScalar()).Trim();
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }

        public static string getNotaCincoObservacionesByIDContrato(string idContrato)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P0452_CAMPO16 FROM T04_CONTRATO WHERE P0401_ID_CONTRATO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@Cont", OdbcType.VarChar).Value = idContrato;
                conexion.Open();
                string result = Convert.ToString(comando.ExecuteScalar()).Trim();
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }

        private static decimal getImporteRecibo(int idHist)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            decimal importeFact = 0;
            try
            {
                string sql = "SELECT P2405_IMPORTE FROM T24_HISTORIA_RECIBOS WHERE T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC=?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idHist", OdbcType.Int).Value = idHist;
                conexion.Open();
                importeFact = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return importeFact;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return 0M;
            }
        }
        /// <summary>
        /// Metodo para obtener el total prorrateado de los cargos para las pólizas de Notas de Crédito
        /// </summary>
        /// <param name="idHist">IHistRec de la factura afectada por la NC</param>
        /// <param name="idContrato">ID del contrato</param>
        /// <param name="importeRecibo">Importe de la NC </param>
        /// <returns></returns>
        public static decimal getTotalCargosProrrateado(int idHist, string idContrato, decimal importeRecibo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            decimal totalCargos = 0M;
            decimal pctCargos = 0M;
            decimal importeFact=0M;
            decimal importeCargosProrrateado=0M;
            try
            {
                //Nota: para las notas de credito el idHist tiene que ser el que proviene de P2425_DEB_REC asignado a IdRecRelNC del recibo
                //El importeRecibo corresponde al de la nota de crédito para calcular la proporción del cargo
                string sql = "SELECT SUM(P3403_IMPORTE) FROM T35_HISTORIAL_CARGOS WHERE T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC=?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idHist", OdbcType.Int).Value = idHist;
                conexion.Open();
                totalCargos = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                //Se toma el importe de la factura que es afectada por una NC
                importeFact = getImporteRecibo(idHist);               
                pctCargos = totalCargos / importeFact;
                importeCargosProrrateado=Math.Round(importeRecibo*pctCargos,4);
                return importeCargosProrrateado;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return 0M;
            }
        }
        /// <summary>
        ///  Metodo para obtener un cargo con importe prorrateado para las pólizas de Notas de Crédito
        /// </summary>
        /// <param name="idHist">IHistRec de la factura afectada por la NC</param>
        /// <param name="idContrato"></param>
        /// <param name="subtipo">Tipo de subcuenta ej: MANT, AGUA, PUB</param>
        /// <param name="importeRecibo">Importe de la Nota de Crédito</param>
        /// <returns></returns>
        public static CargoEntity getCargoProrrateado(int idHist, string idContrato, string subtipo, decimal importeRecibo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            decimal importeFact = 0;
            decimal pctCargo = 0M;

            try
            {
                CargoEntity cargoP = getCargo(idHist, idContrato, subtipo);
                importeFact = getImporteRecibo(idHist);
                pctCargo = cargoP.Importe / importeFact;
                cargoP.Importe = importeRecibo * pctCargo;
                return cargoP;
            }
            catch(Exception ex)
            {
                conexion.Close();
                return null;
            }

        }
        public static CargoEntity getCargoEsporadico(int idHist, string idContrato, string subtipo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string val = "RECIBOESPORADICO";
                string sql = @"SELECT P2412_CONCEPTO, P2405_IMPORTE, P2416_IVA, P2410_MONEDA FROM T24_HISTORIA_RECIBOS WHERE P2444_ID_HIST_REC = ? AND CAMPO20 = ? AND P2418_ID_CONTRATO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idhist", OdbcType.Int).Value = idHist;
                comando.Parameters.Add("@sub", OdbcType.VarChar).Value = subtipo;
                comando.Parameters.Add("@idCont", OdbcType.VarChar).Value = idContrato;
                //comando.Parameters.Add("@val", OdbcType.VarChar).Value = val;
                CargoEntity cargo = null;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    cargo = new CargoEntity();
                    cargo.IDHistRec = idHist;
                    cargo.Subtipo = subtipo;
                    cargo.Concepto = reader["P2412_CONCEPTO"].ToString();
                    cargo.Importe = Convert.ToDecimal(reader["P2405_IMPORTE"]);
                    cargo.IVA=Convert.ToDecimal(reader["P2416_IVA"]);
                    cargo.Moneda = reader["P2410_MONEDA"].ToString();
                    break;
                }
                reader.Close();
                conexion.Close();
                return cargo;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        public static CargoEntity getCargo(int idHist, string idContrato, string subtipo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {                
                string sql = "SELECT P3402_CONCEPTO, P3403_IMPORTE, P3406_MONEDA FROM T35_HISTORIAL_CARGOS WHERE P3409_ID_HIST_REC = ? AND CAMPO4 = ? AND P3401_ID_CARGO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idhist", OdbcType.Int).Value = idHist;
                comando.Parameters.Add("@sub", OdbcType.VarChar).Value = subtipo;
                comando.Parameters.Add("@idCont", OdbcType.VarChar).Value = idContrato;
                CargoEntity cargo = null;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    cargo = new CargoEntity();
                    cargo.IDHistRec = idHist;
                    cargo.Subtipo = subtipo;
                    cargo.Concepto = reader["P3402_CONCEPTO"].ToString();
                    cargo.Importe = Convert.ToDecimal(reader["P3403_IMPORTE"]);
                    cargo.Moneda = reader["P3406_MONEDA"].ToString();
                    break;
                }
                reader.Close();
                conexion.Close();
                return cargo;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static string getUUID(int idCFD)
        {
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            try
            {
                string sql = "SELECT UUID FROM CFD.CFD WHERE ID_CFD = @IDCFD";
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@IDCFD", SqlDbType.Int).Value = idCFD;
                conexion.Open();
                string uuid =Convert.ToString(comando.ExecuteScalar());
                conexion.Close();
                return uuid;
            }
            catch (Exception e)
            {
                conexion.Close();
                return string.Empty;
            }
        }
        public static string getRFC(int idCFD)
        {
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            try
            {
                string sql = "SELECT UUID FROM CFD.CFD WHERE ID_CFD = @IDCFD";
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@IDCFD", SqlDbType.Int).Value = idCFD;
                conexion.Open();
                string uuid = Convert.ToString(comando.ExecuteScalar());
                conexion.Close();
                return uuid;
            }
            catch (Exception e)
            {
                conexion.Close();
                return string.Empty;
            }
        }

        public static List<EgresoEntity> getEgresos(CondicionesEntity condiciones)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                if (condiciones.TipoPoliza == 2)
                {
                    sql = @"SELECT P5401_ID_HIST_REC, P5402_ID_CUENTA_BANCO, P5405_FECHA_GASTO, P5409_IMPORTE_GASTO, P5410_IVA_GASTO, 
P5411_TOTAL_CHEQUE, P5412_MONEDA, P5412_TIPO_CAMBIO, P5413_CONCEPTO, P5414_ID_EMPRESA, P5431_CAMPO_NUM1, P5428_CAMPO3, P5426_CAMPO1, P5427_CAMPO2, 
P5444_IMPORTE_RET_ISR, P5445_NUM_CUENTA_CB_RET_ISR, P5446_DESCR_CUENTA_CB_RET_ISR, P5447_IMPORTE_RET_IVA, P5448_NUM_CUENTA_CB_RET_IVA, 
P5449_DESCR_CUENTA_CB_RET_IVA, P5450_NUM_CUENTA_CB_IVA_ACRED, P5451_DESCR_CUENTA_CB_IVA_ACRED, P5416_CUENTA_CONTABLE_IVA,T03_CENTRO_INDUSTRIAL.CAMPO4 
FROM T54_HISTORIA_CXP 
LEFT JOIN T56_HISTORIA_DETALLE_CXP ON T56_HISTORIA_DETALLE_CXP.P5601_ID_HIST_REC = T54_HISTORIA_CXP.P5401_ID_HIST_REC
LEFT JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T56_HISTORIA_DETALLE_CXP.P5606_ID_CONJUNTO
WHERE P5414_ID_EMPRESA = ? AND P5405_FECHA_GASTO >= ? and P5405_FECHA_GASTO <= ? ";
                }
                else
                {
                    sql = @" SELECT P5401_ID_HIST_REC, P5402_ID_CUENTA_BANCO, P5406_FECHA_GENERACION_CHEQUE, P5409_IMPORTE_GASTO, P5410_IVA_GASTO, 
P5411_TOTAL_CHEQUE, P5412_MONEDA, P5412_TIPO_CAMBIO, P5413_CONCEPTO, P5414_ID_EMPRESA, P5431_CAMPO_NUM1, P5428_CAMPO3, P5426_CAMPO1, P5427_CAMPO2, 
P5444_IMPORTE_RET_ISR, P5445_NUM_CUENTA_CB_RET_ISR, P5446_DESCR_CUENTA_CB_RET_ISR, P5447_IMPORTE_RET_IVA, P5448_NUM_CUENTA_CB_RET_IVA, 
P5449_DESCR_CUENTA_CB_RET_IVA, P5450_NUM_CUENTA_CB_IVA_ACRED, P5451_DESCR_CUENTA_CB_IVA_ACRED, P5416_CUENTA_CONTABLE_IVA,T03_CENTRO_INDUSTRIAL.CAMPO4 
FROM  
T54_HISTORIA_CXP  
LEFT JOIN T56_HISTORIA_DETALLE_CXP ON T56_HISTORIA_DETALLE_CXP.P5601_ID_HIST_REC = T54_HISTORIA_CXP.P5401_ID_HIST_REC  
LEFT JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T56_HISTORIA_DETALLE_CXP.P5606_ID_CONJUNTO  
WHERE P5414_ID_EMPRESA = ? AND P5406_FECHA_GENERACION_CHEQUE >= ? AND P5406_FECHA_GENERACION_CHEQUE <= ? AND P5418_ESTATUS_CHEQUE in('Generado','Conciliado','En Firma','Entregado','EnTransito','ESTATUS_CXP') ";
                }
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idarr", OdbcType.VarChar).Value = condiciones.Inmobiliaria;
                comando.Parameters.Add("@fecha", OdbcType.Date).Value = condiciones.FechaInicio.Date;
                comando.Parameters.Add("@fecha", OdbcType.Date).Value = condiciones.FechaFin.Date;
                List<EgresoEntity> listaEgresos = new List<EgresoEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    EgresoEntity egreso = new EgresoEntity();
                    egreso.ID = (int)reader["P5401_ID_HIST_REC"];
                    egreso.IDCuentaBanco = reader["P5402_ID_CUENTA_BANCO"].ToString().Trim();
                    egreso.Cuenta = getCuentaBanco(egreso.IDCuentaBanco);
                    egreso.NombreCuenta = getCuentaBancoNombre(egreso.IDCuentaBanco);
                    if(condiciones.TipoPoliza == 2)
                        egreso.Fecha = string.IsNullOrEmpty(reader["P5405_FECHA_GASTO"].ToString()) ? new DateTime() : Convert.ToDateTime(reader["P5405_FECHA_GASTO"]);
                    else
                        egreso.Fecha = string.IsNullOrEmpty(reader["P5406_FECHA_GENERACION_CHEQUE"].ToString()) ? new DateTime() : Convert.ToDateTime(reader["P5406_FECHA_GENERACION_CHEQUE"]);
                    egreso.Importe = Convert.ToDecimal(reader["P5409_IMPORTE_GASTO"]);
                    egreso.ImporteIVA = Convert.ToDecimal(reader["P5410_IVA_GASTO"]);
                    egreso.Total = Convert.ToDecimal(reader["P5411_TOTAL_CHEQUE"]);
                    egreso.Moneda = reader["P5412_MONEDA"].ToString().Trim();
                    egreso.TipoCambio = string.IsNullOrEmpty(reader["P5412_TIPO_CAMBIO"].ToString()) ? 0 : Convert.ToDecimal(reader["P5412_TIPO_CAMBIO"]);
                    egreso.Concepto = reader["P5413_CONCEPTO"].ToString().Trim();
                    egreso.IDArrendadora = reader["P5414_ID_EMPRESA"].ToString().Trim();
                    egreso.CuentaIVA = reader["P5416_CUENTA_CONTABLE_IVA"].ToString().Trim();
                    egreso.NombreCuentaIVA = reader["P5428_CAMPO3"].ToString().Trim();
                    egreso.IDProveedor = string.IsNullOrEmpty(reader["P5431_CAMPO_NUM1"].ToString()) ? 0 : Convert.ToInt32(reader["P5431_CAMPO_NUM1"]);                    
                    egreso.CuentaProveedores = reader["P5426_CAMPO1"].ToString().Trim();
                    egreso.NombreCuentaProveedores = reader["P5427_CAMPO2"].ToString().Trim();
                    //Agregado 27/01/2016 by Uz
                    //Se incluyen retenciones, iva acreditado y sus cuentas contables
                    //egreso.CuentaIVA = reader["P5416_CUENTA_CONTABLE_IVA"].ToString().Trim();
                    egreso.CuentaIVA = string.IsNullOrEmpty(reader["P5416_CUENTA_CONTABLE_IVA"].ToString()) ? "" : reader["P5416_CUENTA_CONTABLE_IVA"].ToString().Trim();
                    egreso.DescripCuentaIVA = string.IsNullOrEmpty(reader["P5428_CAMPO3"].ToString()) ? "" : reader["P5428_CAMPO3"].ToString().Trim();
                    egreso.ImporteRetISR = reader["P5444_IMPORTE_RET_ISR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P5444_IMPORTE_RET_ISR"]);
                    egreso.ImporteRetIVA = reader["P5447_IMPORTE_RET_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P5447_IMPORTE_RET_IVA"]);
                    egreso.CuentaRetISR = string.IsNullOrEmpty(reader["P5445_NUM_CUENTA_CB_RET_ISR"].ToString()) ? "" : reader["P5445_NUM_CUENTA_CB_RET_ISR"].ToString().Trim();
                    egreso.DescripCuentaRetISR = string.IsNullOrEmpty(reader["P5446_DESCR_CUENTA_CB_RET_ISR"].ToString()) ? "" : reader["P5446_DESCR_CUENTA_CB_RET_ISR"].ToString().Trim();
                    egreso.CuentaRetIVA = string.IsNullOrEmpty(reader["P5448_NUM_CUENTA_CB_RET_IVA"].ToString()) ? "" : reader["P5448_NUM_CUENTA_CB_RET_IVA"].ToString().Trim();
                    egreso.DescripCuentaRetIVA = string.IsNullOrEmpty(reader["P5449_DESCR_CUENTA_CB_RET_IVA"].ToString()) ? "" : reader["P5449_DESCR_CUENTA_CB_RET_IVA"].ToString().Trim();
                    egreso.CuentaIVAAcred = string.IsNullOrEmpty(reader["P5450_NUM_CUENTA_CB_IVA_ACRED"].ToString()) ? "" : reader["P5450_NUM_CUENTA_CB_IVA_ACRED"].ToString().Trim();
                    egreso.DescripCuentaIVAAcred = string.IsNullOrEmpty(reader["P5451_DESCR_CUENTA_CB_IVA_ACRED"].ToString()) ? "" : reader["P5451_DESCR_CUENTA_CB_IVA_ACRED"].ToString().Trim();
                    //add jl IdentificadorConjunto 11/02/2019
                    egreso.IdentificadorConjunto = string.IsNullOrEmpty(reader["CAMPO4"].ToString()) ? "" : reader["CAMPO4"].ToString().Trim();
                    egreso.ListaRegistros = getMovimientosEgresos(egreso.ID);
                    listaEgresos.Add(egreso);
                }
                reader.Close();
                conexion.Close();
                return listaEgresos;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }

        private static string getCuentaBanco(string idChequera)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P5319_CAMPO1 FROM T53_BANCOS WHERE P5301_ID_CUENTA = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDChq", OdbcType.VarChar).Value = idChequera;
                conexion.Open();
                string result = Convert.ToString(comando.ExecuteScalar());
                conexion.Close();
                return result;
            }
            catch (Exception)
            {
                conexion.Close();
                return string.Empty;
            }
        }

        private static string getCuentaBancoNombre(string idChequera)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P5320_CAMPO2 FROM T53_BANCOS WHERE P5301_ID_CUENTA = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDChq", OdbcType.VarChar).Value = idChequera;
                conexion.Open();
                string result = Convert.ToString(comando.ExecuteScalar());
                conexion.Close();
                return result;
            }
            catch (Exception)
            {
                conexion.Close();
                return string.Empty;
            }
        }

        private static List<RegistroEgresoEntity> getMovimientosEgresos(int idHistRec)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                //                string sql = @"SELECT P5608_CUENTA_GASTO, P5609_CUENTA_IVA, P5610_CONCEPTO, P5611_IMPORTE_GASTO, P5612_IVA_GASTO, P5613_TOTAL_GASTO, P5603_CLASIFICACION, P5620_CAMPO2 
                //FROM T56_HISTORIA_DETALLE_CXP WHERE P5601_ID_HIST_REC = ?";
                string sql = @"SELECT P5608_CUENTA_GASTO, P5609_CUENTA_IVA, P5610_CONCEPTO, P5611_IMPORTE_GASTO, P5612_IVA_GASTO, P5613_TOTAL_GASTO, P5603_CLASIFICACION, P5620_CAMPO2,T03_CENTRO_INDUSTRIAL.CAMPO4  
FROM T56_HISTORIA_DETALLE_CXP 
LEFT JOIN T54_HISTORIA_CXP ON T54_HISTORIA_CXP.P5401_ID_HIST_REC= T56_HISTORIA_DETALLE_CXP.P5601_ID_HIST_REC 
LEFT JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T56_HISTORIA_DETALLE_CXP.P5606_ID_CONJUNTO 
WHERE P5601_ID_HIST_REC = ? ";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDHis", OdbcType.Int).Value = idHistRec;
                List<RegistroEgresoEntity> listaRegistros = new List<RegistroEgresoEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                 {
                    RegistroEgresoEntity registro = new RegistroEgresoEntity();
                    registro.IDHist = idHistRec;
                    registro.NumCuenta = reader["P5608_CUENTA_GASTO"].ToString().Trim();
                    registro.NumCuentaIVA = reader["P5608_CUENTA_GASTO"].ToString().Trim();
                    registro.Concepto = reader["P5610_CONCEPTO"].ToString().Trim();
                    registro.Importe = string.IsNullOrEmpty(reader["P5611_IMPORTE_GASTO"].ToString()) ? 0 : Convert.ToDecimal(reader["P5611_IMPORTE_GASTO"]);
                    registro.ImporteIVA = string.IsNullOrEmpty(reader["P5612_IVA_GASTO"].ToString()) ? 0 : Convert.ToDecimal(reader["P5612_IVA_GASTO"]);
                    registro.ImporteTotal = string.IsNullOrEmpty(reader["P5613_TOTAL_GASTO"].ToString()) ? 0 : Convert.ToDecimal(reader["P5613_TOTAL_GASTO"]);
                    registro.Clasificacion = reader["P5603_CLASIFICACION"].ToString().Trim();
                    registro.NombreCuenta = reader["P5620_CAMPO2"].ToString().Trim();
                    //add jl IdentificadorConjunto 11/02/2019
                    registro.IdentificadorConjunto = string.IsNullOrEmpty(reader["CAMPO4"].ToString()) ? "" : reader["CAMPO4"].ToString().Trim();
                    listaRegistros.Add(registro);
                }
                reader.Close();
                conexion.Close();
                return listaRegistros;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<SucursalEntity> getSucursales()
        {
            List<SucursalEntity> listaSucursales = new List<SucursalEntity>();
            listaSucursales.Add(new SucursalEntity() { Identificador = "*Todas", NombreSucursal = "*Todas" });
            try
            {                
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = "SELECT P4101_ID_SUCURSAL, P4103_NOMBRE FROM T41_SUCURSALES ORDER BY P4103_NOMBRE";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaSucursales.Add(new SucursalEntity() { Identificador = reader["P4101_ID_SUCURSAL"].ToString(), NombreSucursal = reader["P4103_NOMBRE"].ToString() });
                            }
                        }
                    }
                }
                return listaSucursales;
            }
            catch 
            {
                return listaSucursales;
            }
        }
        private static DataTable getPagoCobranza(int idPag)
        {
            try
            {
                SqlDataReader reader = null;
                DataTable dt = new DataTable();
                using (SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString))
                {
                    string sql = "SELECT Total, IVATotalCobrado FROM Cobranza.Pago WHERE IDPago= @IdP";
                    using (SqlCommand comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@IDP", idPag);
                        
                        conexion.Open();
                        reader = comando.ExecuteReader();
                        dt.Load(reader);
                        return dt;
                    }
                }
            }
            catch
            {
                return null;
            }
        }


        private static decimal getPagoReciboCobranza(int idPag, int idHistRec)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString))
                {
                    string sql = "SELECT TotalPagado FROM Cobranza.ReciboPagado WHERE IDPago= @IdP AND IDHistRec=@IdR";
                    using (SqlCommand comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdP", idPag);
                        comando.Parameters.AddWithValue("@IdR", idHistRec);
                        decimal pago;
                        conexion.Open();
                        pago = Convert.ToDecimal(comando.ExecuteScalar());
                        return pago;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        private static DataTable getTotalesPagoReciboCobranza(int idPag, int idHistRec)
        {
            try
            {
                SqlDataReader reader = null;
                DataTable dt = new DataTable();
                using (SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString))
                {
                    string sql = "SELECT TotalPagado, IVAcobrado, RetISRTotalCobrado, RetIVATotalCobrado FROM Cobranza.ReciboPagado WHERE IDPago= @IdP AND IDHistRec=@IdR";
                    using (SqlCommand comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdP", idPag);
                        comando.Parameters.AddWithValue("@IdR", idHistRec);
                        conexion.Open();
                        reader = comando.ExecuteReader();
                        dt.Load(reader);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static decimal getPagosConSaldo(int idHist, int idPago)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString))
                {
                    string sql = "SELECT SUM(Total) AS Pagos FROM Cobranza.PagoConSaldoAFavor WHERE IDHistRec = @idHist AND IDPagoCob = @idPago";
                    using (SqlCommand comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@idHist", SqlDbType.Int).Value = idHist;
                        comando.Parameters.Add("@idPago", SqlDbType.Int).Value = idPago;
                        conexion.Open();
                        decimal result = Convert.ToDecimal(comando.ExecuteScalar());
                        return result;
                    }
                }
            }
            catch 
            {
                return 0;
            }
        }
    }
}
