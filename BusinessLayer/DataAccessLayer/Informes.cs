using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Odbc;
using GestorReportes.BusinessLayer.EntitiesAntiLavado;

namespace GestorReportes.BusinessLayer.DataAccessLayer
{
    public static class Informes
    {
        public static int getReferenciaPorRFC(string rfc, bool esVenta)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                if(!esVenta)
                    sql = "SELECT P0803_ULTIMO_RECIBO FROM T08_CONTROL_REC WHERE P0800_ID_ENTE = 'ANTILAV' AND CAMPO1 = ?";
                else
                    sql = "SELECT P0803_ULTIMO_RECIBO FROM T08_CONTROL_REC WHERE P0800_ID_ENTE = 'ANTILAVVTA' AND CAMPO1 = ?";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@rfc", OdbcType.NVarChar).Value = rfc.Trim();
                DataTable dtRef = new DataTable();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtRef.Load(reader);
                conexion.Close();
                if (dtRef.Rows.Count > 0)
                    return (int)dtRef.Rows[0][0] + 1;
                else
                {
                    if(!esVenta)
                        sql = "INSERT INTO T08_CONTROL_REC (P0800_ID_ENTE, P0803_ULTIMO_RECIBO, CAMPO1) VALUES ('ANTILAV', 1, ?)";
                    else
                    {
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
                    }
                        sql = "INSERT INTO T08_CONTROL_REC (P0800_ID_ENTE, P0803_ULTIMO_RECIBO, CAMPO1) VALUES ('ANTILAVVTA', 1, ?)";
                    comando = conexion.CreateCommand();
                    comando.CommandText = sql;
                    comando.Parameters.Add("@rfc", OdbcType.NVarChar).Value = rfc.Trim();
                    conexion.Open();
                    int afect = comando.ExecuteNonQuery();
                    conexion.Close();
                    if (afect > 0)
                        return 1;
                    else
                        return 0;
                }
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        public static int setUltimaReferenciaPorRFC(string rfc, int ultimoRegistro, bool esVenta)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);                       
            try
            {
                string sql = string.Empty;
                if(!esVenta)
                    sql = "UPDATE T08_CONTROL_REC SET P0803_ULTIMO_RECIBO = ? WHERE P0800_ID_ENTE = 'ANTILAV' AND CAMPO1 = ?";
                else
                    sql = "UPDATE T08_CONTROL_REC SET P0803_ULTIMO_RECIBO = ? WHERE P0800_ID_ENTE = 'ANTILAVVTA' AND CAMPO1 = ?";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@ultrec", OdbcType.Int).Value = ultimoRegistro;
                comando.Parameters.Add("@rfc", OdbcType.NVarChar).Value = rfc.Trim();
                conexion.Open();
                int afect = comando.ExecuteNonQuery();
                conexion.Close();
                if (afect > 0)
                    return afect;
                else
                    return 0;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        public static DataTable getTelefonoPorIdCliente(string idCliente)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0203_NOMBRE, P0601_ID_ENTE, P0604_DIRECCION, P0603_TIPO_SERV FROM T06_MAIL_WEB JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T06_MAIL_WEB.P0601_ID_ENTE WHERE P0601_ID_ENTE = ? AND P0605_ORDEN = 1 AND (P0603_TIPO_SERV = 'T' OR P0603_TIPO_SERV = 'E')";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idcliente", OdbcType.NVarChar).Value = idCliente;
                DataTable dtTelMail = new DataTable();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtTelMail.Load(reader);
                conexion.Close();
                return dtTelMail;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }

        public static DataTable getDomicilioPorIdCliente(string idCliente)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P0500_ID_ENTE, P0504_COLONIA, P0503_CALLE_NUM, CAMPO1, CAMPO2, P0505_COD_POST 
                                FROM T05_DOMICILIO WHERE P0500_ID_ENTE = ? AND P0502_TIPO_DOMICILIO = 5";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idcliente", OdbcType.NVarChar).Value = idCliente;
                conexion.Open();
                DataTable dtDomicilio = new DataTable();
                OdbcDataReader reader = comando.ExecuteReader();
                dtDomicilio.Load(reader);
                conexion.Close();
                return dtDomicilio;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }

        public static DataTable getPersonaFisicaPorIdCliente(string idCliente)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P0203_NOMBRE, P0262_NOMBRE_PF, P0263_APELLIDOP_PF, P0264_APELLIDOM_PF, P0204_RFC, CAMPO3, P0257_ID_ACTIVIDAD_ECONOMICA, P0261_ID_TIPO_IDENTIFICACION, P0254_DOC_ID_OFICIAL, P0260_DESCR_AUTORIDAD_EMITE_ID, P0255_ID_DOC_OFICIAL,
                                P0207_FECHA
                                FROM T02_ARRENDATARIO WHERE P0201_ID = ?";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idcliente", OdbcType.NVarChar).Value = idCliente;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DataTable dtPersFis = new DataTable();
                dtPersFis.Load(reader);
                conexion.Close();
                return dtPersFis;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }            
        }

        public static DataTable getPersonaMoralPorIdCliente(string idCliente)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P0203_NOMBRE, P0204_RFC, P0257_ID_ACTIVIDAD_ECONOMICA, P0258_DESCR_ACTIVIDAD_ECONOMICA, T15_PERSONA.P1503_NOMBRE, T15_PERSONA.P1509_APELLIDOP_PERSONA, T15_PERSONA.P1510_APELLIDOM_PERSONA, 
                               T15_PERSONA.CAMPO1, T15_PERSONA.CAMPO2, T15_PERSONA.P1515_ID_TIPO_IDENTIFICACION, T15_PERSONA.P1511_DOC_ID_OFICIAL, T15_PERSONA.P1514_DESCR_AUTORIDAD_EMITE_ID, T15_PERSONA.P1512_ID_DOC_OFICIAL,
                               P0207_FECHA
                               FROM T02_ARRENDATARIO 
                               LEFT JOIN T15_PERSONA ON T02_ARRENDATARIO.P0201_ID = T15_PERSONA.P1504_ID_ENTE_EXT
                               WHERE P0201_ID = ? AND T15_PERSONA.P1502_TIPO_ENTE = 5";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idcliente", OdbcType.NVarChar).Value = idCliente;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader()                                                                                                                                                                                                                                                                                                                                                                                                                                                                 ;
                DataTable dtPersMor = new DataTable();
                dtPersMor.Load(reader);
                conexion.Close();
                return dtPersMor;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }

        public static bool esClienteFisica(string idCliente)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P2703_PERSONALIDAD FROM T27_IMPUESTOS WHERE P2701_ID_ENTE = ?";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@idcliente", OdbcType.NVarChar).Value = idCliente;
                conexion.Open();
                string persona = comando.ExecuteScalar().ToString();
                conexion.Close();
                if (persona == "F")
                    return true;
                else
                    return false;
            }
            catch
            {
                conexion.Close();
                return false;                
            }
        }

        public static DataTable getFacturasTotales(string rfc, DateTime inicio, DateTime fin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                //Modify by Uz  06/09/2016
                //Para informe de Antilavado se resta el importe de otros cargos al importe de la factura
                string sql = @"SELECT T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO AS CLIENTE, 
                                SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' 
                                THEN (T24_HISTORIA_RECIBOS.P2405_IMPORTE * P2421_TC_PAGO) - (T24_HISTORIA_RECIBOS.CAMPO_NUM6 * P2421_TC_PAGO)  
                                ELSE T24_HISTORIA_RECIBOS.P2405_IMPORTE - (CASE WHEN T24_HISTORIA_RECIBOS.CAMPO_NUM6 is NULL THEN 0 ELSE T24_HISTORIA_RECIBOS.CAMPO_NUM6 END) END) AS SUMA 
                                FROM T40_CFD
                                JOIN T24_HISTORIA_RECIBOS ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC 
                                WHERE P4077_RFC_INMOBILIARIA = ? AND T40_CFD.P4014_TIPO_COMPROBANTE = 'INGRESO' AND T24_HISTORIA_RECIBOS.P2406_STATUS = 2 AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ? AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','P','U')
                                GROUP BY T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO
                                ORDER BY SUMA DESC";
                //                string sql = @"SELECT T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO AS CLIENTE, SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN T24_HISTORIA_RECIBOS.P2405_IMPORTE * P2421_TC_PAGO ELSE T24_HISTORIA_RECIBOS.P2405_IMPORTE END) AS SUMA    
//                                FROM T40_CFD
//                                JOIN T24_HISTORIA_RECIBOS ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC 
//                                WHERE P4077_RFC_INMOBILIARIA = ? AND T40_CFD.P4014_TIPO_COMPROBANTE = 'INGRESO' AND T24_HISTORIA_RECIBOS.P2406_STATUS = 2 AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ? AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','P','U')
//                                GROUP BY T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO
//                                ORDER BY SUMA DESC";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@rfc", OdbcType.NVarChar).Value = rfc;
                comando.Parameters.Add("@inicio", OdbcType.DateTime).Value = inicio;
                comando.Parameters.Add("@fin", OdbcType.DateTime).Value = fin;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DataTable dtFacturasTotales = new DataTable();
                dtFacturasTotales.Load(reader);
                conexion.Close();
                return dtFacturasTotales;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }            
        }
        public static DataTable getFacturasTotales(string rfc, DateTime inicio, DateTime fin, string rfcGenericoNac, string rfcGenericoExt)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {  
                string sql = @"SELECT T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO AS CLIENTE,  
                                SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' 
                                THEN (T24_HISTORIA_RECIBOS.P2405_IMPORTE * P2421_TC_PAGO) - (T24_HISTORIA_RECIBOS.CAMPO_NUM6 * P2421_TC_PAGO)  
                                ELSE T24_HISTORIA_RECIBOS.P2405_IMPORTE - (CASE WHEN T24_HISTORIA_RECIBOS.CAMPO_NUM6 is NULL THEN 0 ELSE T24_HISTORIA_RECIBOS.CAMPO_NUM6 END) END) AS SUMA 
                                FROM T40_CFD
                                JOIN T24_HISTORIA_RECIBOS ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC 
                                WHERE P4077_RFC_INMOBILIARIA = ? AND T40_CFD.P4014_TIPO_COMPROBANTE = 'INGRESO' AND T24_HISTORIA_RECIBOS.P2406_STATUS = 2 AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ? AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','P','U') 
                                AND P2423_RFC != ? AND P2423_RFC != ?   
                                GROUP BY T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO
                                ORDER BY SUMA DESC";

//                string sql = @"SELECT T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO AS CLIENTE, SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN T24_HISTORIA_RECIBOS.P2405_IMPORTE * P2421_TC_PAGO ELSE T24_HISTORIA_RECIBOS.P2405_IMPORTE END) AS SUMA    
//                                FROM T40_CFD
//                                JOIN T24_HISTORIA_RECIBOS ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC 
//                                WHERE P4077_RFC_INMOBILIARIA = ? AND T40_CFD.P4014_TIPO_COMPROBANTE = 'INGRESO' AND T24_HISTORIA_RECIBOS.P2406_STATUS = 2 AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ? AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','P','U') 
//                                AND P2423_RFC != ? 
//                                GROUP BY T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO
//                                ORDER BY SUMA DESC";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@rfc", OdbcType.NVarChar).Value = rfc;
                comando.Parameters.Add("@inicio", OdbcType.DateTime).Value = inicio;
                comando.Parameters.Add("@fin", OdbcType.DateTime).Value = fin;
                comando.Parameters.Add("@rfcGenericoN", OdbcType.NVarChar).Value = rfcGenericoNac;
                comando.Parameters.Add("@rfcGenericoE", OdbcType.NVarChar).Value = rfcGenericoExt;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DataTable dtFacturasTotales = new DataTable();
                dtFacturasTotales.Load(reader);
                conexion.Close();
                return dtFacturasTotales;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }
        public static DataTable getFacturasTotalesPorCliente(string cliente, string rfc, DateTime inicio, DateTime fin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO AS CLIENTE, SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN T24_HISTORIA_RECIBOS.P2405_IMPORTE * P2421_TC_PAGO ELSE T24_HISTORIA_RECIBOS.P2405_IMPORTE END) AS SUMA    
                                FROM T40_CFD
                                JOIN T24_HISTORIA_RECIBOS ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC 
                                WHERE P4077_RFC_INMOBILIARIA = ? AND T40_CFD.P4014_TIPO_COMPROBANTE = 'INGRESO' AND T24_HISTORIA_RECIBOS.P2406_STATUS = 2 AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ? AND T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO = ? AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','P','U')
                                GROUP BY T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO
                                ORDER BY SUMA DESC";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@rfc", OdbcType.NVarChar).Value = rfc;
                comando.Parameters.Add("@inicio", OdbcType.DateTime).Value = inicio;
                comando.Parameters.Add("@fin", OdbcType.DateTime).Value = fin;
                comando.Parameters.Add("@cliente", OdbcType.NVarChar).Value = cliente;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DataTable dtFacturasTotales = new DataTable();
                dtFacturasTotales.Load(reader);
                conexion.Close();
                return dtFacturasTotales;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }

        public static DataTable getVentasTotalesPorCliente(string cliente, string rfc, DateTime inicio, DateTime fin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO AS CLIENTE, SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN T24_HISTORIA_RECIBOS.P2405_IMPORTE * P2421_TC_PAGO ELSE T24_HISTORIA_RECIBOS.P2405_IMPORTE END) AS SUMA    
                                FROM T24_HISTORIA_RECIBOS
                                JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
                                WHERE T01_ARRENDADORA.P0106_RFC = ? AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'V'  AND T24_HISTORIA_RECIBOS.P2406_STATUS = 2 AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ? AND T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO = ?
                                GROUP BY T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO
                                ORDER BY SUMA DESC";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@rfc", OdbcType.NVarChar).Value = rfc;
                comando.Parameters.Add("@inicio", OdbcType.DateTime).Value = inicio;
                comando.Parameters.Add("@fin", OdbcType.DateTime).Value = fin;
                comando.Parameters.Add("@cliente", OdbcType.NVarChar).Value = cliente;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DataTable dtFacturasTotales = new DataTable();
                dtFacturasTotales.Load(reader);
                conexion.Close();
                return dtFacturasTotales;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }

        public static DataTable getTotalNCPorCliente(FacturaTotalPorCliente factura, string rfc, DateTime inicio, DateTime fin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO AS CLIENTE, SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN T24_HISTORIA_RECIBOS.P2405_IMPORTE * P2421_TC_PAGO ELSE T24_HISTORIA_RECIBOS.P2405_IMPORTE END) AS SUMA   
                                FROM T40_CFD
                                JOIN T24_HISTORIA_RECIBOS ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC 
                                WHERE P4077_RFC_INMOBILIARIA = ? AND T40_CFD.P4014_TIPO_COMPROBANTE = 'EGRESO' AND T24_HISTORIA_RECIBOS.P2406_STATUS <> 3 AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ? AND T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO = ? AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'B'
                                GROUP BY T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO
                                ORDER BY SUMA DESC";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@rfc", OdbcType.NVarChar).Value = rfc;
                comando.Parameters.Add("@inicio", OdbcType.DateTime).Value = inicio;
                comando.Parameters.Add("@fin", OdbcType.DateTime).Value = fin;
                comando.Parameters.Add("@cliente", OdbcType.NVarChar).Value = factura.IDCliente;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DataTable dtNCTotal = new DataTable();
                dtNCTotal.Load(reader);
                conexion.Close();
                return dtNCTotal;
            }
            catch
            {
                return new DataTable();
            }            
        }

        public static DataTable getFacturasPorCliente(string idCliente, string rfc, DateTime inicio, DateTime fin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {/*//En esta consulta se cambia Join con la tabla T40 JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO = T40_CFD.P4001_ID_HIST_REC para los pagos parciales
              * //Cuando se hace un pago parcial hace falta que se le pase el instrumento de pago.
              * * string sql = @"SELECT Distinct T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC,T24_HISTORIA_RECIBOS.P2426_TIPO_DOC,T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO,T24_HISTORIA_RECIBOS.P2415_IMP_DOLAR, T24_HISTORIA_RECIBOS.P2413_PAGO,
                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2410_MONEDA , T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,
                     T07_EDIFICIO.P0770_ID_CLASIF_FISCAL, 
                    T19_DESC_GRAL.P1922_A_MIN_ING, T05_DOMICILIO.P0504_COLONIA, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.CAMPO1, T05_DOMICILIO.CAMPO2, 
                    T05_DOMICILIO.P0505_COD_POST, T24_HISTORIA_RECIBOS.P2457_ID_INSTRUMENTO_PAGO, T24_HISTORIA_RECIBOS.P2453_NOMBRE_BANCO_GIRADOR, 
                    T24_HISTORIA_RECIBOS.P2454_CUENTA_BCO_PAGO, T24_HISTORIA_RECIBOS.P2455_NUM_CHEQUE_PAGO, T24_HISTORIA_RECIBOS.P2456_CLAVE_RESTREO_PAGO, 
                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2405_IMPORTE, P2421_TC_PAGO, T07_EDIFICIO.P0740_CAMPO15, T07_EDIFICIO.P0701_ID_EDIFICIO, 
                    T05_DOMICILIO.P0500_ID_ENTE, T07_EDIFICIO.P0703_NOMBRE, T07_EDIFICIO.P0772_INMUEBLE_BLINDADO, T07_EDIFICIO.P0730_SUBCONJUNTO, T24_HISTORIA_RECIBOS.CAMPO_NUM6 
FROM T24_HISTORIA_RECIBOS
                   JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC 
                    JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T24_HISTORIA_RECIBOS.CAMPO3
                    JOIN T19_DESC_GRAL ON T19_DESC_GRAL.P1901_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO
                    JOIN T05_DOMICILIO ON T05_DOMICILIO.P0500_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO
                    WHERE P4077_RFC_INMOBILIARIA = ? AND T40_CFD.P4014_TIPO_COMPROBANTE = 'INGRESO'
                    AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ?
                     AND T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO = ? AND T24_HISTORIA_RECIBOS.P2406_STATUS = 2 
                    AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','W')
UNION                                
SELECT Distinct T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC,T24_HISTORIA_RECIBOS.P2426_TIPO_DOC,T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO,T24_HISTORIA_RECIBOS.P2415_IMP_DOLAR, T24_HISTORIA_RECIBOS.P2413_PAGO,
                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2410_MONEDA , T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,
                     T07_EDIFICIO.P0770_ID_CLASIF_FISCAL, 
                    T19_DESC_GRAL.P1922_A_MIN_ING, T05_DOMICILIO.P0504_COLONIA, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.CAMPO1, T05_DOMICILIO.CAMPO2, 
                    T05_DOMICILIO.P0505_COD_POST, T24_HISTORIA_RECIBOS.P2457_ID_INSTRUMENTO_PAGO, T24_HISTORIA_RECIBOS.P2453_NOMBRE_BANCO_GIRADOR, 
                    T24_HISTORIA_RECIBOS.P2454_CUENTA_BCO_PAGO, T24_HISTORIA_RECIBOS.P2455_NUM_CHEQUE_PAGO, T24_HISTORIA_RECIBOS.P2456_CLAVE_RESTREO_PAGO, 
                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2405_IMPORTE, P2421_TC_PAGO, T07_EDIFICIO.P0740_CAMPO15, T07_EDIFICIO.P0701_ID_EDIFICIO, 
                    T05_DOMICILIO.P0500_ID_ENTE, T07_EDIFICIO.P0703_NOMBRE, T07_EDIFICIO.P0772_INMUEBLE_BLINDADO, T07_EDIFICIO.P0730_SUBCONJUNTO, T24_HISTORIA_RECIBOS.CAMPO_NUM6 
FROM T24_HISTORIA_RECIBOS
                    JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO = T40_CFD.P4001_ID_HIST_REC
                    JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T24_HISTORIA_RECIBOS.CAMPO3
                    JOIN T19_DESC_GRAL ON T19_DESC_GRAL.P1901_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO
                    JOIN T05_DOMICILIO ON T05_DOMICILIO.P0500_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO
                    WHERE P4077_RFC_INMOBILIARIA = ? AND T40_CFD.P4014_TIPO_COMPROBANTE = 'INGRESO'
                    AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ?
                    AND T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO = ? AND T24_HISTORIA_RECIBOS.P2406_STATUS = 2 
                    AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('X','P')";*/
                //Se agrego el campo descuento
                string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO,T24_HISTORIA_RECIBOS.P2415_IMP_DOLAR, T24_HISTORIA_RECIBOS.P2413_PAGO, 
                    T24_HISTORIA_RECIBOS.P2410_MONEDA , T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO,T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, T07_EDIFICIO.P0770_ID_CLASIF_FISCAL, 
                    T19_DESC_GRAL.P1922_A_MIN_ING, T05_DOMICILIO.P0504_COLONIA, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.CAMPO1, T05_DOMICILIO.CAMPO2, 
                    T05_DOMICILIO.P0505_COD_POST, T24_HISTORIA_RECIBOS.P2457_ID_INSTRUMENTO_PAGO, T24_HISTORIA_RECIBOS.P2453_NOMBRE_BANCO_GIRADOR, 
                    T24_HISTORIA_RECIBOS.P2454_CUENTA_BCO_PAGO, T24_HISTORIA_RECIBOS.P2455_NUM_CHEQUE_PAGO, T24_HISTORIA_RECIBOS.P2456_CLAVE_RESTREO_PAGO, 
                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2405_IMPORTE, P2421_TC_PAGO, T07_EDIFICIO.P0740_CAMPO15, T07_EDIFICIO.P0701_ID_EDIFICIO, 
                    T05_DOMICILIO.P0500_ID_ENTE, T07_EDIFICIO.P0703_NOMBRE, T07_EDIFICIO.P0772_INMUEBLE_BLINDADO, T07_EDIFICIO.P0730_SUBCONJUNTO, T24_HISTORIA_RECIBOS.CAMPO_NUM6 
                    FROM T40_CFD
                    JOIN T24_HISTORIA_RECIBOS ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC 
                    JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T24_HISTORIA_RECIBOS.CAMPO3
                    JOIN T19_DESC_GRAL ON T19_DESC_GRAL.P1901_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO
                    JOIN T05_DOMICILIO ON T05_DOMICILIO.P0500_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO
                    WHERE P4077_RFC_INMOBILIARIA = ? AND T40_CFD.P4014_TIPO_COMPROBANTE = 'INGRESO' 
                    AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ? 
                    AND T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO = ? AND T24_HISTORIA_RECIBOS.P2406_STATUS = 2 
                    AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','X','W','P')";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@rfc", OdbcType.NVarChar).Value = rfc;
                comando.Parameters.Add("@inicio", OdbcType.DateTime).Value = inicio;
                comando.Parameters.Add("@fin", OdbcType.DateTime).Value = fin;
                comando.Parameters.Add("@idcliente", OdbcType.NVarChar).Value = idCliente;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DataTable dtFacturasCliente = new DataTable();
                dtFacturasCliente.Load(reader);
                conexion.Close();
                return dtFacturasCliente;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }

        public static DataTable getVentasPorCliente(string idCliente, string rfc, DateTime inicio, DateTime fin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T07_EDIFICIO.P0703_NOMBRE, T07_EDIFICIO.P0770_ID_CLASIF_FISCAL, (CASE WHEN T04_CONTRATO.P0407_MONEDA_FACT = 'D' THEN T04_CONTRATO.P0408_IMPORTE_FACT * T04_CONTRATO.P0458_CAMPO_NUM7 ELSE T04_CONTRATO.P0408_IMPORTE_FACT END) AS MONTOPACTADO, T05_DOMICILIO.P0504_COLONIA, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.CAMPO1, T05_DOMICILIO.CAMPO2, T05_DOMICILIO.P0505_COD_POST, T19_DESC_GRAL.P1926_CIST_ING AS TERRENO, T19_DESC_GRAL.CAMPO_NUM1 AS CONSTRUCCION, P0772_INMUEBLE_BLINDADO, T07_EDIFICIO.P0740_CAMPO15, T04_CONTRATO.CAMPO_DATE1, T04_CONTRATO.P0406_FORMA_FACT, T04_CONTRATO.P0407_MONEDA_FACT, T24_HISTORIA_RECIBOS.P2457_ID_INSTRUMENTO_PAGO, T24_HISTORIA_RECIBOS.P2453_NOMBRE_BANCO_GIRADOR, T24_HISTORIA_RECIBOS.P2454_CUENTA_BCO_PAGO, T24_HISTORIA_RECIBOS.P2455_NUM_CHEQUE_PAGO, T24_HISTORIA_RECIBOS.P2456_CLAVE_RESTREO_PAGO, T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2405_IMPORTE 
                            FROM T24_HISTORIA_RECIBOS 
                            JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
                            JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T24_HISTORIA_RECIBOS.CAMPO3
                            JOIN T04_CONTRATO ON T04_CONTRATO.P0401_ID_CONTRATO = T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO
                            JOIN T19_DESC_GRAL ON T19_DESC_GRAL.P1901_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO 
                            JOIN T05_DOMICILIO ON T05_DOMICILIO.P0500_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO
                            WHERE T01_ARRENDADORA.P0106_RFC = ? AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'V'  AND T24_HISTORIA_RECIBOS.P2406_STATUS = 2 AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ? AND T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO = ?";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@rfc", OdbcType.NVarChar).Value = rfc;
                comando.Parameters.Add("@inicio", OdbcType.DateTime).Value = inicio;
                comando.Parameters.Add("@fin", OdbcType.DateTime).Value = fin;
                comando.Parameters.Add("@idcliente", OdbcType.NVarChar).Value = idCliente;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DataTable dtFacturasCliente = new DataTable();
                dtFacturasCliente.Load(reader);
                conexion.Close();
                return dtFacturasCliente;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }

        public static decimal getSalarioMinimo()
        {
            string sql = "SELECT P3904_VALOR_INDEX FROM T39_INDICES_ECONOMICOS WHERE P3901_TIPO_INDEX = 'SALARIOMIN' ORDER BY P3909_FECHA_HORA_ULT_MOV DESC";
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                conexion.Open();
                decimal salarioMin = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return salarioMin;
            }
            catch
            {
                conexion.Close();
                return 0;
            }            
        }

        public static DataTable getVentasTotales(string rfc, DateTime inicio, DateTime fin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO AS CLIENTE, SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN T24_HISTORIA_RECIBOS.P2405_IMPORTE * P2421_TC_PAGO ELSE T24_HISTORIA_RECIBOS.P2405_IMPORTE END) AS SUMA    
                                FROM T24_HISTORIA_RECIBOS
                                JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
                                WHERE T01_ARRENDADORA.P0106_RFC = ? AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'V'  AND T24_HISTORIA_RECIBOS.P2406_STATUS = 2 AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ?
                                GROUP BY T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO
                                ORDER BY SUMA DESC";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@rfc", OdbcType.NVarChar).Value = rfc;
                comando.Parameters.Add("@inicio", OdbcType.DateTime).Value = inicio;
                comando.Parameters.Add("@fin", OdbcType.DateTime).Value = fin;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DataTable dtFacturasTotales = new DataTable();
                dtFacturasTotales.Load(reader);
                conexion.Close();
                return dtFacturasTotales;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }

        public static bool existeContratoPorSubconjunto(string idSubconjunto)
        {
            string sql = "SELECT COUNT(*) FROM T04_CONTRATO WHERE P0404_ID_EDIFICIO = ?";
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                OdbcCommand comando = conexion.CreateCommand();
                comando.Parameters.Add(@"idedificio", OdbcType.VarChar).Value = idSubconjunto;
                comando.CommandText = sql;
                conexion.Open();
                int result = Convert.ToInt32(comando.ExecuteScalar());
                conexion.Close();
                return result > 0;
            }
            catch
            {
                conexion.Close();
                return false;
            }
        }

        public static DataTable getDTValorCatastralDeSubconjunto(string idSubconjunto)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P0701_ID_EDIFICIO, P1922_A_MIN_ING     
                                FROM T07_EDIFICIO 
                                JOIN T19_DESC_GRAL ON T19_DESC_GRAL.P1901_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO 
                                WHERE P0730_SUBCONJUNTO = ?";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@subconjunto", OdbcType.NVarChar).Value = idSubconjunto;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DataTable dtValoresCatastrales = new DataTable();
                dtValoresCatastrales.Load(reader);
                conexion.Close();
                return dtValoresCatastrales;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }

        public static DataTable getDTFoliosRPPDeSubconjunto(string idSubconjunto)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P0740_CAMPO15     
                                FROM T07_EDIFICIO                                  
                                WHERE P0730_SUBCONJUNTO = ?";
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = sql;
                comando.Parameters.Add("@subconjunto", OdbcType.NVarChar).Value = idSubconjunto;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DataTable dtFolios = new DataTable();
                dtFolios.Load(reader);
                conexion.Close();
                return dtFolios;
            }
            catch
            {
                conexion.Close();
                return new DataTable();
            }
        }
    }
}
