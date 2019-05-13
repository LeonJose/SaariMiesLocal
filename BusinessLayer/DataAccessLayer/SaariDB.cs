using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Odbc;
using GestorReportes.BusinessLayer.EntitiesReportes;
using System.Net.Mail;
using System.Net;
using System.Linq;
using GestorReportes.BusinessLayer.Entities;
using GestorReportes.BusinessLayer.Helpers;
using System.Data.SqlClient;

namespace GestorReportes.BusinessLayer.DataAccessLayer
{
    public static class SaariDB
    {
        #region CALCULO DE MORATORIOS
        private static string cadenaDeConexion = Properties.Settings.Default.SaariODBC_ConnectionString;
        public enum BaseMoratorios
        {
            Ninguno,
            TIIE,
            TIIEMasPrc,
            TIIEPorPrc,
            PrcFijo
        }

        /// <summary>
        /// Obtiene el tipo de la la base para calcular los moratorios del contrato especificado
        /// </summary>
        /// <param name="idContrato">Identificador del contrato del cual se calcularán los moratorios</param>
        /// <returns></returns>
        public static BaseMoratorios getTipoBaseMoratoriosPorContrato(string idContrato)
        {
            OdbcConnection conexion = new OdbcConnection(cadenaDeConexion);
            try
            {
                string sql = "SELECT CAMPO1 FROM T04_CONTRATO WHERE P0401_ID_CONTRATO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = idContrato;
                conexion.Open();
                string baseMor = comando.ExecuteScalar().ToString();
                conexion.Close();
                if (string.IsNullOrEmpty(baseMor))
                    return BaseMoratorios.Ninguno;
                else if (baseMor.Trim() == "T")
                    return BaseMoratorios.TIIE;
                else if (baseMor.Trim() == "TMP")
                    return BaseMoratorios.TIIEMasPrc;
                else if (baseMor.Trim() == "TPP")
                    return BaseMoratorios.TIIEPorPrc;
                else
                {
                    if (getBaseMoratoriosPorContrato(idContrato) > 0)
                        return BaseMoratorios.PrcFijo;
                    else
                        return BaseMoratorios.Ninguno;
                }
            }
            catch (Exception ex)
            {
                conexion.Close();
                return BaseMoratorios.Ninguno;
            }
        }

        /// <summary>
        /// Obtiene el porcentaje fijo de la base para calcular los moratorios del contrato especificado
        /// </summary>
        /// <param name="idContrato">Identificador del contrato del cual se calcularán los moratorios</param>
        /// <returns></returns>
        public static decimal getBaseMoratoriosPorContrato(string idContrato)
        {
            OdbcConnection conexion = new OdbcConnection(cadenaDeConexion);
            try
            {
                string sql = "SELECT CAMPO_NUM2 FROM T04_CONTRATO WHERE P0401_ID_CONTRATO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = idContrato;
                conexion.Open();
                decimal baseMor = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return baseMor;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return 0;
            }
        }

        /// <summary>
        /// Obtiene el porcentaje del indice especificado
        /// </summary>
        /// <param name="indice">Indice que se desea obtener. Ejemplo TIIE</param>
        /// <param name="fecha">Fecha en la cual se buscará el indice</param>
        /// <returns></returns>
        public static decimal getPrcIndice(string indice, DateTime fecha)
        {
            OdbcConnection conexion = new OdbcConnection(cadenaDeConexion);
            try
            {
                string sql = "SELECT P3904_VALOR_INDEX FROM T39_INDICES_ECONOMICOS WHERE P3901_TIPO_INDEX = ? AND P3902_MES_INDEX >= ? AND P3902_MES_INDEX <= ?";
                DateTime fechaIni = new DateTime(fecha.Year, fecha.Month, 1);
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@Indice", OdbcType.VarChar).Value = indice;
                comando.Parameters.Add("@FechaIni", OdbcType.Date).Value = fechaIni;
                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaIni.AddMonths(1).AddDays(-1);
                conexion.Open();
                decimal resultIndice = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return resultIndice / 100;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return 0;
            }
        }

        /// <summary>
        /// Obtiene el criterio para calcular moratorios de la inmobiliaria; Regresa 1 si es diario, 2 si es mensual y 0 en caso de error
        /// </summary>
        /// <param name="idArrendadora">Identificador de la inmobiliaria a consultar</param>
        /// <returns></returns>
        public static int getTipoMoratorios(string idArrendadora)
        {
            OdbcConnection conexion = new OdbcConnection(cadenaDeConexion);
            try
            {
                string sql = "SELECT CAMPO_NUM1 FROM T01_ARRENDADORA WHERE P0101_ID_ARR = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArrendadora;
                conexion.Open();
                int tipoMor = Convert.ToInt32(comando.ExecuteScalar());
                conexion.Close();
                return tipoMor;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return 0;
            }
        }


        public static BaseMoratorios getTipoBaseMoratorios(string baseMor)
        {
            if (string.IsNullOrEmpty(baseMor))
                return BaseMoratorios.Ninguno;
            else if (baseMor.Trim() == "T")
                return BaseMoratorios.TIIE;
            else if (baseMor.Trim() == "TMP")
                return BaseMoratorios.TIIEMasPrc;
            else if (baseMor.Trim() == "TPP")
                return BaseMoratorios.TIIEPorPrc;
            else
                return BaseMoratorios.PrcFijo;
        }

        /// <summary>
        /// Obtiene los intereses moratorios de un recibo
        /// </summary>
        /// <param name="recibo">Entidad con la información del recibo al que se le va a aplicar el cálculo de moratorios</param>
        /// /// <param name="fechaCorte">Fecha en la que se va a realizar el pago</param>
        /// <returns></returns>
        private static decimal obtenerMoratorios(ReciboEntity recibo, DateTime fechaCorte, ConfiguracionMoratoriosEntity configMoratorios)
        {
            try
            {
                TimeSpan tiempoAtraso = fechaCorte.Date - recibo.FechaVencimiento.Date;
                int diasAtras = tiempoAtraso.Days;
                if (configMoratorios.aplicarDiasGracia == 2)
                    diasAtras -= configMoratorios.diasDeGracia;

                //BaseMoratorios tipoBase = inmoDAL.getTipoBaseMoratorios(recibo.IDContrato);
                BaseMoratorios tipoBase = getTipoBaseMoratorios(configMoratorios.tipoCalculoMoratorio);
                decimal prcFijo = configMoratorios.tasaInteres;
                if (tipoBase != BaseMoratorios.Ninguno && tipoBase != BaseMoratorios.PrcFijo)
                {
                    DateTime fechaVencAux = recibo.FechaVencimiento.Date;
                    fechaVencAux = fechaVencAux.AddDays(1);
                    decimal moratorioAcumulado = 0;
                    for (int i = 1; i <= diasAtras; i++)
                    {
                        decimal tasaInteres = getPrcIndice("TIIE", fechaVencAux.Date);
                        tasaInteres = tasaInteres / 365;
                        if (tipoBase == BaseMoratorios.TIIEMasPrc)
                            tasaInteres += (tasaInteres + prcFijo);
                        else if (tipoBase == BaseMoratorios.TIIEPorPrc)
                            tasaInteres += (tasaInteres * prcFijo);
                        moratorioAcumulado += (tasaInteres * recibo.CantidadPorPagar);
                        fechaVencAux = fechaVencAux.AddDays(1);
                    }
                    return moratorioAcumulado;
                }
                else if (tipoBase == BaseMoratorios.PrcFijo)
                {
                    decimal tasaInteres = prcFijo;
                    int tipoMoratorio = configMoratorios.diasMesesMoratorios;
                    decimal impMoratorios = 0;
                    if (tipoMoratorio == 2)
                    {
                        int mesesTranscurridos = ((fechaCorte.Year - recibo.FechaVencimiento.Year) * 12) + fechaCorte.Month - recibo.FechaVencimiento.AddDays(-configMoratorios.diasDeGracia).Month;
                        if (mesesTranscurridos > 0)
                            impMoratorios = recibo.CantidadPorPagar * (tasaInteres / 100) * mesesTranscurridos;
                    }
                    else if (tipoMoratorio == 1)
                    {
                        if (diasAtras > 0)
                            impMoratorios = recibo.CantidadPorPagar * (tasaInteres / 100) / 30.4m * diasAtras;
                    }
                    return impMoratorios;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion

        public static List<InmobiliariaEntity> getListaInmobiliarias()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, P0102_N_COMERCIAL, P0122_CAMPO15 FROM T01_ARRENDADORA ORDER BY P0103_RAZON_SOCIAL";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                List<InmobiliariaEntity> listaInmobiliarias = new List<InmobiliariaEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    InmobiliariaEntity inmo = new InmobiliariaEntity()
                    {
                        ID = reader["P0101_ID_ARR"].ToString(),
                        RazonSocial = reader["P0103_RAZON_SOCIAL"].ToString(),
                        NombreComercial = reader["P0102_N_COMERCIAL"].ToString(),
                        IdArrendadora = reader["P0122_CAMPO15"].ToString()
                        
                    };
                    listaInmobiliarias.Add(inmo);
                }
                reader.Close();
                conexion.Close();
                return listaInmobiliarias;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        //add getListConjuntos() 16/08/2018 JL
        public static List<ConjuntoEntity> getListaConjuntos(string id)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = "SELECT P0301_ID_CENTRO, P0303_NOMBRE FROM T03_CENTRO_INDUSTRIAL WHERE CAMPO1 = ? ORDER BY P0303_NOMBRE";
            try
            {
                OdbcCommand com = new OdbcCommand(sql, conexion);
                List<ConjuntoEntity> listaConjuntos = new List<ConjuntoEntity>();
                com.Parameters.Add("@IdInmb", OdbcType.VarChar).Value = id;
                listaConjuntos.Add(new ConjuntoEntity() { ID ="Todos", Nombre = "*Todos"});
                conexion.Open();
                OdbcDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    ConjuntoEntity conj = new ConjuntoEntity();

                    conj.ID = reader["P0301_ID_CENTRO"].ToString();
                    conj.Nombre = reader["P0303_NOMBRE"].ToString();
                    listaConjuntos.Add(conj);
                }
                reader.Close();
                conexion.Close();
                return listaConjuntos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        //add getEstatusCobranza
        public static List<EstadoCobranzaEntity> GetEstatusCobranza(InmobiliariaEntity Inmo, ConjuntoEntity Conjun, DateTime fechaInicio, DateTime fechaFin, int opcion,int opcionOrden)
        {
            var list = new List<EstadoCobranzaEntity>();            
            #region sql
            string sql = @"SELECT 
P2444_ID_HIST_REC,
P0303_NOMBRE, 
P4006_SERIE, 
P4007_FOLIO, 
P2403_NUM_RECIBO,
P2413_PAGO,
P0703_NOMBRE, 
P2411_N_ARRENDATARIO, 
P2406_STATUS,
P2405_IMPORTE,
P2424_DESCUENTO,
(P2419_TOTAL - P2416_IVA + T24_HISTORIA_RECIBOS.CAMPO_NUM1 + T24_HISTORIA_RECIBOS.CAMPO_NUM2) AS neto,
P2410_MONEDA, 
P2414_TIPO_CAMBIO, 
T24_HISTORIA_RECIBOS.CAMPO4, 
CASE P2406_STATUS WHEN '3' THEN 0  
ELSE (P2419_TOTAL - P2416_IVA + T24_HISTORIA_RECIBOS.CAMPO_NUM1 + T24_HISTORIA_RECIBOS.CAMPO_NUM2)  
END as IMPORTE, 
CASE P2410_MONEDA WHEN 'D' THEN (P2419_TOTAL - P2416_IVA + T24_HISTORIA_RECIBOS.CAMPO_NUM1 + T24_HISTORIA_RECIBOS.CAMPO_NUM2)  
ELSE ''  
END as imp_dls, 
CASE when P2410_MONEDA <> 'D' THEN P2405_IMPORTE  
ELSE (P2419_TOTAL - P2416_IVA)  
END AS imp_pesos,
P2416_IVA,
CASE P2406_STATUS WHEN '3' THEN 0  
ELSE P2416_IVA  
END AS IVA,
CASE P2410_MONEDA WHEN 'D' THEN P2416_IVA  
ELSE ''  
END AS iva_dls,
CASE WHEN P2410_MONEDA <> 'D' THEN P2416_IVA  
ELSE ''  
END as iva_pesos, 
T24_HISTORIA_RECIBOS.CAMPO_NUM1,
((P2419_TOTAL - P2416_IVA) + P2416_IVA)  as  SUBTOTAL,
CASE P2406_status WHEN '3' THEN 0  
ELSE T24_HISTORIA_RECIBOS.CAMPO_NUM1  
END AS RET_ISR,  
CASE P2410_MONEDA WHEN '3' THEN T24_HISTORIA_RECIBOS.CAMPO_NUM1 
ELSE '' 
END as isr_dls,
CASE WHEN P2410_MONEDA <>'3' THEN T24_HISTORIA_RECIBOS.CAMPO_NUM1  
ELSE ''  
END as isr_pesos,
T24_HISTORIA_RECIBOS.CAMPO_NUM2, 
CASE P2406_status WHEN '3' THEN 0  
ELSE T24_HISTORIA_RECIBOS.CAMPO_NUM2  
END as RET_IVA,
CASE P2410_MONEDA when 'D' THEN T24_HISTORIA_RECIBOS.CAMPO_NUM2  
ELSE ''  
END as iva_ret_dls, 
CASE when P2410_MONEDA <>'D' THEN T24_HISTORIA_RECIBOS.CAMPO_NUM2 
ELSE ''  
END as iva_ret_pes,
P2419_TOTAL,
CASE P2406_status WHEN '3' THEN 0 
ELSE P2419_TOTAL 
END as TOTAL, 
CASE P2410_MONEDA WHEN 'D' THEN P2419_TOTAL 
ELSE '' 
END AS total_dls,
CASE WHEN P2410_MONEDA  <> 'D' THEN P2419_TOTAL 
ELSE '' 
END AS total_pesos, 
CASE P2410_MONEDA WHEN 'D' THEN  ((P2419_TOTAL - P2416_IVA) * P2414_Tipo_cambio) 
ELSE '' 
END as importe,
CASE P2410_MONEDA WHEN 'D' THEN (P2416_IVA * P2414_Tipo_cambio) 
ELSE '' 
END as iva,
CASE P2410_MONEDA WHEN 'D' THEN (T24_HISTORIA_RECIBOS.CAMPO_NUM1 * P2414_TIPO_CAMBIO) 
ELSE '' 
END as isr,
CASE P2410_MONEDA WHEN 'D' THEN (T24_HISTORIA_RECIBOS.CAMPO_NUM2 * P2414_TIPO_CAMBIO) 
ELSE '' 
END as ivar,
CASE P2410_MONEDA WHEN 'D' THEN (P2419_TOTAL * P2414_TIPO_CAMBIO) 
ELSE '' 
END as total,
P2404_PERIODO,
P2412_CONCEPTO, 
P2408_FECHA_PAGADO,  
CASE P2406_status WHEN '2' THEN  P2408_FECHA_PAGADO 
ELSE '' 
END as FECHA_DE_PAGO,
P2427_CTD_PAG,
T24_HISTORIA_RECIBOS.CAMPO17,
T18_SUBCONJUNTOS.P1803_NOMBRE,
P2426_TIPO_DOC  
FROM 
T24_HISTORIA_RECIBOS 
LEFT JOIN T03_CENTRO_INDUSTRIAL ON  T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T24_HISTORIA_RECIBOS.CAMPO4 
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC 
LEFT JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T24_HISTORIA_RECIBOS.CAMPO3 
LEFT JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T24_HISTORIA_RECIBOS.CAMPO17 
WHERE P2401_ID_ARRENDADORA = ? 
AND P2409_FECHA_EMISION >= ?  
and P2409_FECHA_EMISION <= ?   
AND P2426_TIPO_DOC IN ('R','X','Z','W','P')   
AND P2406_STATUS IN ('1','2') ";
            #endregion

            //Clasificacion Conjuntos
            if (opcion == 1)
            {
                
            }
            else if (opcion == 2)
            {
                sql += " AND T24_HISTORIA_RECIBOS.CAMPO4 = ? ";
            }
            else
            {
                
            }
            //Opcion orden.
            if (opcionOrden == 1)
            {
                sql += " ORDER BY P4007_FOLIO";
            }
            else
            {
                sql += "ORDER BY P2403_NUM_RECIBO";
            }
            
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                OdbcCommand com = new OdbcCommand(sql, conexion);
                com.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = Inmo.ID;
                com.Parameters.Add("@FecInicio", OdbcType.DateTime).Value = fechaInicio;
                com.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                
                if (opcion == 1)
                {
                    
                }
                else if (opcion == 2)
                {
                    com.Parameters.Add("@conjunt", OdbcType.VarChar).Value = Conjun.ID;
                }
                else
                {

                }
                conexion.Open();
                OdbcDataReader reader = com.ExecuteReader();
                decimal TotalPagos = 0;

                while (reader.Read())
                {
                    EstadoCobranzaEntity estCob = new EstadoCobranzaEntity();
                    estCob._moneda = reader["P2410_MONEDA"]== DBNull.Value ? string.Empty :reader["P2410_MONEDA"].ToString();
                    estCob.PagParciales = reader["P2413_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2413_PAGO"];
                    estCob.NomArrendatario = reader["P2411_N_ARRENDATARIO"].ToString();
                    estCob.Nombre = reader["P0303_NOMBRE"].ToString();
                    estCob.Serie = reader["P4006_SERIE"].ToString();
                    estCob.Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"];
                    estCob.Periodo = reader["P2404_PERIODO"].ToString();
                    estCob.Concepto = reader["P2412_CONCEPTO"].ToString();
                    estCob.FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    estCob.SubConjunto = reader["P1803_NOMBRE"] == DBNull.Value ? string.Empty : reader["P1803_NOMBRE"].ToString();
                    //tipodocumento para validar
                    estCob.TipoDoc = reader["P2426_TIPO_DOC"] == DBNull.Value ? string.Empty : reader["P2426_TIPO_DOC"].ToString();

                    if (estCob._moneda == "D")
                    {
                        estCob.Importe = reader["imp_dls"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["IMPORTE"]);
                        estCob.Iva = reader["iva_dls"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["iva_dls"]);
                        //estCob.Subtotal = reader[""] == DBNull.Value ? 0 : Convert.ToDecimal(reader[""]);
                        estCob.RetISR = reader["RET_ISR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RET_ISR"]);
                        estCob.RetIva = reader["iva_ret_dls"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["iva_ret_dls"]);
                        estCob.Total = reader["total_dls"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["total_dls"]);
                       // estCob.Saldo = reader[""] == DBNull.Value ? 0 : (decimal)reader[""];

                    }
                    else
                    {
                        estCob.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2405_IMPORTE"]);
                        estCob.Iva = reader["IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["IVA"]);
                        estCob.Subtotal = reader["SUBTOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["SUBTOTAL"]);
                        estCob.RetISR = reader["isr_pesos"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["isr_pesos"]);
                        estCob.RetIva = reader["iva_ret_pes"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["iva_ret_pes"]);
                        estCob.Total = reader["TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL"]);                        
                        estCob.Saldo = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"];
                    }
                    

                    list.Add(estCob);
                }

                reader.Close();
                conexion.Close();
                return list;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        public static string getCentroIndustrial(ConjuntoEntity idConjun)
        {
            try
            {
                OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"select * from 
T40_CFD
JOIN T24_HISTORIA_RECIBOS on T40_CFD.P4001_ID_HIST_REC = T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC
JOIN T04_CONTRATO ON T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = T04_CONTRATO.P0403_ID_ARRENDADORA
JOIN T07_EDIFICIO ON T04_CONTRATO.P0404_ID_EDIFICIO = T07_EDIFICIO.P0701_ID_EDIFICIO
where T07_EDIFICIO.P0710_ID_CENTRO = @idConjun  AND T04_CONTRATO.P0428_A_PRORROGA = 'V'
AND T04_CONTRATO.P0437_TIPO = 'R' AND T07_EDIFICIO.P0710_ID_CENTRO = @Conj";
            

            }
            catch(Exception )
            {

            }
            return null;
        }

        public static List<InmobiliariaEntity> getListaInmobiliariasWithTodos()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, P0102_N_COMERCIAL FROM T01_ARRENDADORA ORDER BY P0103_RAZON_SOCIAL";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                List<InmobiliariaEntity> listaInmobiliarias = new List<InmobiliariaEntity>();
                InmobiliariaEntity i = new InmobiliariaEntity();
                i.ID = "Todos";
                i.RazonSocial = "Todos";
                i.NombreComercial = "*Todos";
                listaInmobiliarias.Add(i);

                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    InmobiliariaEntity inmo = new InmobiliariaEntity()
                    {
                        ID = reader["P0101_ID_ARR"].ToString(),
                        RazonSocial = reader["P0103_RAZON_SOCIAL"].ToString(),
                        NombreComercial = reader["P0102_N_COMERCIAL"].ToString()
                    };
                    listaInmobiliarias.Add(inmo);

                }
                reader.Close();
                conexion.Close();
                return listaInmobiliarias;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        public static InmobiliariaEntity getInmobiliariaByID(string idInmobiliaria)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, P0102_N_COMERCIAL, P0106_RFC FROM T01_ARRENDADORA WHERE P0101_ID_ARR = ?";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                InmobiliariaEntity inmo = null;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    inmo = new InmobiliariaEntity()
                    {
                        ID = reader["P0101_ID_ARR"].ToString(),
                        RazonSocial = reader["P0103_RAZON_SOCIAL"] == DBNull.Value ? "SIN RAZON SOCIAL" : reader["P0103_RAZON_SOCIAL"].ToString(),
                        NombreComercial = reader["P0102_N_COMERCIAL"] == DBNull.Value ? "SIN NOMBRE COMERCIAL" : reader["P0102_N_COMERCIAL"].ToString(),
                        RFC = reader["P0106_RFC"] == DBNull.Value ? string.Empty : reader["P0106_RFC"].ToString()
                    };
                    break;

                }
                reader.Close();
                conexion.Close();
                return inmo;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        public static List<ConjuntoEntity> getConjuntosHelp(string idInmobiliaria)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0301_ID_CENTRO, P0303_NOMBRE FROM T03_CENTRO_INDUSTRIAL WHERE CAMPO1 = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                List<ConjuntoEntity> listaConjuntos = new List<ConjuntoEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ConjuntoEntity conjunto = new ConjuntoEntity();
                    conjunto.ID = reader["P0301_ID_CENTRO"].ToString();
                    conjunto.Nombre = reader["P0303_NOMBRE"].ToString();
                    listaConjuntos.Add(conjunto);
                }                
                try
                {
                    ConjuntoEntity conjunto = new ConjuntoEntity();
                    conjunto.ID = "CTR0";
                    conjunto.Nombre = "LIBRES";
                    listaConjuntos.Add(conjunto);
                    conjunto = new ConjuntoEntity();
                    conjunto.ID = "CTRT";
                    conjunto.Nombre = "TODOS LOS CONJUNTOS";
                    listaConjuntos.Insert(0,conjunto);
                }
                catch(Exception ex)
                {

                }
                return listaConjuntos;
            }
            catch
            {
                conexion.Close();
                return new List<ConjuntoEntity>();
            }
        }
        public static List<ConjuntoEntity> getConjuntosGrupoHelp(string idInmobiliaria, bool esGrupo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0301_ID_CENTRO, P0303_NOMBRE FROM T03_CENTRO_INDUSTRIAL WHERE CAMPO1 = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                List<ConjuntoEntity> listaConjuntos = new List<ConjuntoEntity>();
                ConjuntoEntity c = new ConjuntoEntity();
                c.ID = "Todos";
                c.Nombre = "*Todos";
                //c.NombreComercial = "*Todos";
                listaConjuntos.Add(c);
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ConjuntoEntity conjunto = new ConjuntoEntity();
                    conjunto.ID = reader["P0301_ID_CENTRO"].ToString();
                    conjunto.Nombre = reader["P0303_NOMBRE"].ToString();
                    listaConjuntos.Add(conjunto);
                }
                reader.Close();
                conexion.Close();
                return listaConjuntos;
            }
            catch
            {
                conexion.Close();
                return new List<ConjuntoEntity>();
            }
        }
        public static List<ConjuntoEntity> getConjuntos(string idInmobiliaria)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0301_ID_CENTRO, P0303_NOMBRE FROM T03_CENTRO_INDUSTRIAL WHERE CAMPO1 = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                List<ConjuntoEntity> listaConjuntos = new List<ConjuntoEntity>();
                listaConjuntos.Add(new ConjuntoEntity() { ID = "Todos", Nombre = "*Todos" });
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ConjuntoEntity conjunto = new ConjuntoEntity();
                    conjunto.ID = reader["P0301_ID_CENTRO"].ToString();
                    conjunto.Nombre = reader["P0303_NOMBRE"].ToString();
                    listaConjuntos.Add(conjunto);
                }
                reader.Close();
                conexion.Close();
                return listaConjuntos;
            }
            catch
            {
                conexion.Close();
                return new List<ConjuntoEntity>();
            }
        }
        public static List<ContratosEntity> getContratosPorInmobiliaria(string idInmobiliaria)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT P0401_ID_CONTRATO, P0402_ID_ARRENDAT, P0404_ID_EDIFICIO, P0403_ID_ARRENDADORA, T02_ARRENDATARIO.P0203_NOMBRE, 
T07_EDIFICIO.P0703_NOMBRE AS nameInmueble, T04_CONTRATO.P0428_A_PRORROGA, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.CAMPO4, 
P0448_CAMPO12, P0449_CAMPO13, P0450_CAMPO14, P0451_CAMPO15, P0452_CAMPO16   
FROM T04_CONTRATO JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T04_CONTRATO.P0402_ID_ARRENDAT 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T04_CONTRATO.P0404_ID_EDIFICIO
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T07_EDIFICIO.P0710_ID_CENTRO 
LEFT JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T04_CONTRATO.P0403_ID_ARRENDADORA ";

            if (idInmobiliaria != "TODOS")
            {
                sql += "WHERE P0403_ID_ARRENDADORA = ? AND (T04_CONTRATO.P0437_TIPO = 'R' OR T04_CONTRATO.P0437_TIPO = 'S') ";
            }
            else
            {
                sql += "WHERE (T04_CONTRATO.P0437_TIPO = 'R' OR T04_CONTRATO.P0437_TIPO = 'S')";
            }

            sql += @" UNION
SELECT P0401_ID_CONTRATO, P0402_ID_ARRENDAT, P0404_ID_EDIFICIO, P0403_ID_ARRENDADORA,T02_ARRENDATARIO.P0203_NOMBRE,
T18_SUBCONJUNTOS.P1803_NOMBRE AS nameInmueble ,T04_CONTRATO.P0428_A_PRORROGA,T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.CAMPO4, 
P0448_CAMPO12, P0449_CAMPO13, P0450_CAMPO14, P0451_CAMPO15, P0452_CAMPO16 
FROM T04_CONTRATO                    
JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID
JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO =  T04_CONTRATO.P0404_ID_EDIFICIO
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T18_SUBCONJUNTOS.P1804_ID_CONJUNTO
LEFT JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T04_CONTRATO.P0403_ID_ARRENDADORA ";
            if (idInmobiliaria != "TODOS")
            {
                sql += "WHERE P0403_ID_ARRENDADORA = ? AND (T04_CONTRATO.P0437_TIPO = 'R' OR T04_CONTRATO.P0437_TIPO = 'S')";
            }
            else
            {
                sql += "WHERE (T04_CONTRATO.P0437_TIPO = 'R' OR T04_CONTRATO.P0437_TIPO = 'S')";
            }
            sql += " Order by 5";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@IDInmo2", OdbcType.VarChar).Value = idInmobiliaria;
                List<ContratosEntity> listaContratos = new List<ContratosEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ContratosEntity contrato = new ContratosEntity()
                    {
                        IDContrato = reader["P0401_ID_CONTRATO"].ToString(),
                        IDCliente = reader["P0402_ID_ARRENDAT"].ToString(),
                        IDInmueble = reader["P0404_ID_EDIFICIO"].ToString(),
                        Cliente = reader["P0203_NOMBRE"].ToString(),
                        Inmueble = reader["nameInmueble"].ToString(),
                        Vigente = reader["P0428_A_PRORROGA"].ToString() == "V",
                        Conjunto = reader["P0303_NOMBRE"].ToString(),
                        Moneda = reader["P0407_MONEDA_FACT"].ToString(),
                        Identificador = reader["CAMPO4"].ToString(),
                        IDArrendadora = reader["P0403_ID_ARRENDADORA"].ToString(),
                        Nota1 = reader["P0448_CAMPO12"].ToString(),
                        Nota2 = reader["P0449_CAMPO13"].ToString(),
                        Nota3 = reader["P0450_CAMPO14"].ToString(),
                        Nota4 = reader["P0451_CAMPO15"].ToString(),
                        Nota5 = reader["P0452_CAMPO16"].ToString()
                    };
                    listaContratos.Add(contrato);
                }
                reader.Close();
                conexion.Close();
                return listaContratos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idInmobiliaria"></param>
        /// <param name="tipoContrato">R=Renta - V=Venta</param>
        /// <returns></returns>
        /// 

        public static List<RepoteGlobal> getContratosPorInmobiliariayConjunto(string idInmobiliaria, string idConjunto)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT DISTINCT P0401_ID_CONTRATO,P0403_ID_ARRENDADORA,P0301_ID_CENTRO,P0701_ID_EDIFICIO,P0402_ID_ARRENDAT,P0703_NOMBRE,P0707_CONTRUCCION_M2,P0203_NOMBRE,P0425_ACTIVIDAD,P0253_NOMBRE_COMERCIAL,P0434_IMPORTE_ACTUAL,P0410_INICIO_VIG,
P0411_FIN_VIG,P0418_IMPORTE_DEPOSITO,P3105_DIAS_GRACIA_RENTA
FROM T04_CONTRATO
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T04_CONTRATO.P0404_ID_EDIFICIO
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T07_EDIFICIO.P0710_ID_CENTRO
JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID= T04_CONTRATO.P0402_ID_ARRENDAT
JOIN T31_CONFIGURACION ON   T31_CONFIGURACION.P3102_VALOR = T04_CONTRATO.P0401_ID_CONTRATO ";

            if (idConjunto != "Todos")
            {
                sql += @" 
AND P0301_ID_CENTRO = ? 
UNION
SELECT P0401_ID_CONTRATO,P0403_ID_ARRENDADORA,P0301_ID_CENTRO,P0701_ID_EDIFICIO,P0402_ID_ARRENDAT,P0703_NOMBRE,P0707_CONTRUCCION_M2,P0203_NOMBRE,P0425_ACTIVIDAD,P0253_NOMBRE_COMERCIAL,P0434_IMPORTE_ACTUAL,P0410_INICIO_VIG,
P0411_FIN_VIG,P0418_IMPORTE_DEPOSITO,P3105_DIAS_GRACIA_RENTA
FROM T04_CONTRATO
JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID= T04_CONTRATO.P0402_ID_ARRENDAT
JOIN T31_CONFIGURACION ON   T31_CONFIGURACION.P3102_VALOR = T04_CONTRATO.P0401_ID_CONTRATO
JOIN T07_EDIFICIO ON    P0701_ID_EDIFICIO = P0404_ID_EDIFICIO
JOIN T03_CENTRO_INDUSTRIAL ON P0301_ID_CENTRO = P0710_ID_CENTRO  
WHERE  P0301_ID_CENTRO = ?
ORDER BY 6
";
            }
            else
            {
                sql += " WHERE T04_CONTRATO.P0403_ID_ARRENDADORA = ? ORDER BY T07_EDIFICIO.P0703_NOMBRE";
            }
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                if (idConjunto != "Todos")
                {
                    comando.Parameters.Add("@IdConjunto", OdbcType.VarChar).Value = idConjunto;
                    comando.Parameters.Add("@IdConjunto", OdbcType.VarChar).Value = idConjunto;
                }
                List<RepoteGlobal> listaContratos = new List<RepoteGlobal>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    RepoteGlobal contrato = new RepoteGlobal()
                    {
                        IdContrato = reader["P0401_ID_CONTRATO"].ToString(),
                        idInmo = reader["P0403_ID_ARRENDADORA"].ToString(),
                        IdEdificio = reader["P0701_ID_EDIFICIO"].ToString(),
                        IdCliente = reader["P0402_ID_ARRENDAT"].ToString(),
                        idConjunto = reader["P0301_ID_CENTRO"].ToString(),
                        NombreInmueble = reader["P0703_NOMBRE"].ToString(),
                        M2Construccion = reader["P0707_CONTRUCCION_M2"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P0707_CONTRUCCION_M2"]),
                        NombreCliente = reader["P0203_NOMBRE"].ToString(),
                        Actividad = reader["P0425_ACTIVIDAD"].ToString(),
                        NombreComercial = reader["P0253_NOMBRE_COMERCIAL"].ToString(),
                        RentaActual = reader["P0434_IMPORTE_ACTUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P0434_IMPORTE_ACTUAL"]),
                        InicioVigencia = (DateTime)reader["P0410_INICIO_VIG"], //== DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P0410_INICIO_VIG"]),
                        FinVigencia = (DateTime)reader["P0411_FIN_VIG"], // == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P0411_FIN_VIG"]),
                        Deposito = reader["P0418_IMPORTE_DEPOSITO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P0418_IMPORTE_DEPOSITO"]),
                        DiasGracia = reader["P3105_DIAS_GRACIA_RENTA"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P3105_DIAS_GRACIA_RENTA"])
                        // Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],

                        //CostoM2 = (contrato.RentaActual / contrato.M2)
                    };

                    try
                    {
                        contrato.CostoM2 = contrato.RentaActual / contrato.M2Construccion;
                    }
                    catch
                    {
                        contrato.CostoM2 = 0;
                    }

                    listaContratos.Add(contrato);
                }
                reader.Close();
                conexion.Close();
                return listaContratos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        public static List<SubtipoEntity> getValorcargo(string idcontrato, string concepto, DateTime inicioPer, DateTime finPer)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                /*SELECT first 1  P2401_ID_ARRENDADORA,P2418_ID_CONTRATO,P2402_ID_ARRENDATARIO,CAMPO20,P2419_TOTAL,P2409_FECHA_EMISION
                                        FROM T24_HISTORIA_RECIBOS
                                        WHERE P2418_ID_CONTRATO = ?
                                        AND CAMPO20 = ?*/
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = @"SELECT first 1   P2401_ID_ARRENDADORA,P2418_ID_CONTRATO,P2402_ID_ARRENDATARIO,CAMPO20,P2419_TOTAL,P2409_FECHA_EMISION
                                        FROM T24_HISTORIA_RECIBOS
                                        WHERE P2418_ID_CONTRATO = ?
                                        AND CAMPO20 = ?
                                        AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?
                                        order BY P2409_FECHA_EMISION desc";
                List<SubtipoEntity> subtiposSuma = new List<SubtipoEntity>();
                conexion.Open();
                comando.Parameters.Add("@idContrato", OdbcType.VarChar).Value = idcontrato;
                comando.Parameters.Add("@idCargo", OdbcType.VarChar).Value = concepto;
                comando.Parameters.Add("@IniPer", OdbcType.DateTime).Value = inicioPer;
                comando.Parameters.Add("@FinPer", OdbcType.DateTime).Value = finPer;
                OdbcDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    SubtipoEntity sub = new SubtipoEntity()
                    {
                        Identificador = reader["CAMPO20"].ToString().Trim(),
                        suma = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"])

                    };
                    subtiposSuma.Add(sub);
                }
                SubtipoEntity sum = new SubtipoEntity();
                if (subtiposSuma == null)
                {
                    sum.Identificador = concepto;
                    sum.suma = 0;
                    subtiposSuma.Add(sum);

                }
                else if (subtiposSuma.Count <= 0)
                {
                    sum.Identificador = concepto;
                    sum.suma = 0;
                    subtiposSuma.Add(sum);

                }


                reader.Close();
                conexion.Close();
                return subtiposSuma;
            }
            catch
            {
                conexion.Close();
                return null;
            }

        }
        public static List<SubtipoEntity> getSumaPorCargo(string idcontrato, string concepto)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = @"SELECT P2401_ID_ARRENDADORA,P2418_ID_CONTRATO,P2402_ID_ARRENDATARIO,CAMPO20,SUM(P2419_TOTAL) AS TOTAL
                                        FROM T24_HISTORIA_RECIBOS
                                        WHERE P2418_ID_CONTRATO = ?
                                        AND CAMPO20 = ? 
                                        GROUP BY P2401_ID_ARRENDADORA,P2418_ID_CONTRATO,P2402_ID_ARRENDATARIO,CAMPO20";
                List<SubtipoEntity> subtiposSuma = new List<SubtipoEntity>();
                conexion.Open();
                comando.Parameters.Add("@idContrato", OdbcType.VarChar).Value = idcontrato;
                comando.Parameters.Add("@idCargo", OdbcType.VarChar).Value = concepto;
                OdbcDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    SubtipoEntity sub = new SubtipoEntity()
                    {
                        Identificador = reader["CAMPO20"].ToString().Trim(),
                        suma = reader["TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL"])

                    };
                    subtiposSuma.Add(sub);
                }
                SubtipoEntity sum = new SubtipoEntity();
                if (subtiposSuma == null)
                {



                    sum.Identificador = concepto;
                    sum.suma = 0;
                    subtiposSuma.Add(sum);

                }
                else if (subtiposSuma.Count <= 0)
                {

                    sum.Identificador = concepto;
                    sum.suma = 0;
                    subtiposSuma.Add(sum);

                }


                reader.Close();
                conexion.Close();
                return subtiposSuma;
            }
            catch
            {
                conexion.Close();
                return null;
            }

        }

        public static List<SubtipoEntity> getCargosPorContrato(string idContrato)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            string sql = @"SELECT T34_OTRO_CARGOS.P3401_ID_CARGO, T34_OTRO_CARGOS.P3402_CONCEPTO,T34_OTRO_CARGOS.P3403_IMPORTE, T34_OTRO_CARGOS.CAMPO2, T34_OTRO_CARGOS.CAMPO_DATE1,T34_OTRO_CARGOS.CAMPO_DATE2 from T34_OTRO_CARGOS 
                           WHERE T34_OTRO_CARGOS.P3401_ID_CARGO =?";

            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IdContrato", OdbcType.VarChar).Value = idContrato;

                List<SubtipoEntity> ListaCargosContrato = new List<SubtipoEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    SubtipoEntity CargosContrato = new SubtipoEntity()
                    {
                        IdContrato = reader["P3401_ID_CARGO"].ToString(),
                        Nombre = reader["P3402_CONCEPTO"].ToString(),
                        Identificador = reader["CAMPO2"].ToString(),
                        importeCargo = reader["P3403_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3403_IMPORTE"])
                        //Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],

                    };

                    CargosContrato.importeCargo = CargosContrato.importeCargo * 0.16M;


                    ListaCargosContrato.Add(CargosContrato);
                }
                reader.Close();
                conexion.Close();



                return ListaCargosContrato;
            }
            catch
            {
                conexion.Close();
                return null;
            }

        }
        public static List<ReciboEntity> getCargosFacturadosxIdHistRec(int idHistRec, string cargo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            string sql = @"Select T35_HISTORIAL_CARGOS.P3401_ID_CARGO, T35_HISTORIAL_CARGOS.P3402_CONCEPTO, T35_HISTORIAL_CARGOS.P3403_IMPORTE, T35_HISTORIAL_CARGOS.P3404_IVA,
 T35_HISTORIAL_CARGOS.P3405_TOTAL, T35_HISTORIAL_CARGOS.CAMPO4, T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC
from T35_HISTORIAL_CARGOS
WHERE T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC = ? AND T35_HISTORIAL_CARGOS.CAMPO4 = ?";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idHistRec", OdbcType.Int).Value = idHistRec;
                comando.Parameters.Add("@Cargo", OdbcType.VarChar).Value = cargo;

                List<ReciboEntity> ListaCargosPer = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity CargoFac = new ReciboEntity()
                    {
                        IDContrato = reader["P3401_ID_CARGO"].ToString(),
                        //IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString(),
                        Campo20 = reader["CAMPO4"].ToString(),
                        Importe = reader["P3403_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3403_IMPORTE"]),
                        IVA = reader["P3404_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3404_IVA"]),
                        TotalIVA = reader["P3405_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3405_TOTAL"])
                        //FechaEmision = reader["CAMPO20"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"])

                        //Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],

                    };

                    ListaCargosPer.Add(CargoFac);
                }


                reader.Close();
                conexion.Close();
                return ListaCargosPer;
            }
            catch
            {
                conexion.Close();
                return null;
            }

        }

        public static List<ReciboEntity> getCargosFacturadosxIdHistRec(int idHistRec)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            string sql = @"SELECT T35_HISTORIAL_CARGOS.P3401_ID_CARGO,P2409_FECHA_EMISION,P2408_FECHA_PAGADO,T35_HISTORIAL_CARGOS.P3402_CONCEPTO, 
                            T35_HISTORIAL_CARGOS.P3403_IMPORTE, T35_HISTORIAL_CARGOS.P3404_IVA, T35_HISTORIAL_CARGOS.P3405_TOTAL, T35_HISTORIAL_CARGOS.CAMPO4,
                            T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC,P2414_TIPO_CAMBIO,P2406_STATUS,P2426_TIPO_DOC,CAMPO_DATE1,P2444_ID_HIST_REC,P2410_MONEDA,
                            P2411_N_ARRENDATARIO,P2402_ID_ARRENDATARIO
                            FROM T35_HISTORIAL_CARGOS
                            JOIN T24_HISTORIA_RECIBOS ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = P3409_ID_HIST_REC 
                            WHERE T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC = ?";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idHistRec", OdbcType.Int).Value = idHistRec;
                //   comando.Parameters.Add("@Cargo", OdbcType.VarChar).Value = cargo;

                List<ReciboEntity> ListaCargosPer = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity CargoFac = new ReciboEntity()
                    {
                        IDContrato = reader["P3401_ID_CARGO"].ToString(),
                        IDHistRec = Convert.ToInt32(reader["P2444_ID_HIST_REC"].ToString()),
                        IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString(),
                        FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                        FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                        Campo20 = reader["CAMPO4"].ToString(),
                        Importe = reader["P3403_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3403_IMPORTE"]),
                        TotalIVA = reader["P3404_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3404_IVA"]),
                        Total = reader["P3405_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3405_TOTAL"]),
                        TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 1 : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"]),
                        Estatus = reader["P2406_STATUS"].ToString(),
                        TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                        VencimientoPago = reader["CAMPO_DATE1"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CAMPO_DATE1"]),
                        Moneda = reader["P2410_MONEDA"].ToString(),
                        NombreCliente = reader["P2411_N_ARRENDATARIO"] == DBNull.Value ? string.Empty : reader["P2411_N_ARRENDATARIO"].ToString(),
                        cliente = SaariDB.getClienteByID(reader["P2402_ID_ARRENDATARIO"].ToString())

                        //FechaEmision = reader["CAMPO20"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"])
                        //Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],
                    };
                    //CargoFac.NombreCliente.Replace("CANCELADO", "")
                    CargoFac.NombreCliente.Replace("CANCELADO", "");
                    ListaCargosPer.Add(CargoFac);
                }


                reader.Close();
                conexion.Close();
                return ListaCargosPer;
            }
            catch
            {
                conexion.Close();
                return null;
            }

        }


        public static List<ContratosEntity> getContratosPorInmobiliariaYTipo(string idInmobiliaria, string tipoContrato)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT P0401_ID_CONTRATO, P0402_ID_ARRENDAT, P0404_ID_EDIFICIO, P0403_ID_ARRENDADORA, T02_ARRENDATARIO.P0203_NOMBRE, 
T07_EDIFICIO.P0703_NOMBRE, T04_CONTRATO.P0428_A_PRORROGA, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.CAMPO4, T04_CONTRATO.P0437_TIPO   
FROM T04_CONTRATO JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T04_CONTRATO.P0402_ID_ARRENDAT 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T04_CONTRATO.P0404_ID_EDIFICIO
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T07_EDIFICIO.P0710_ID_CENTRO 
LEFT JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T04_CONTRATO.P0403_ID_ARRENDADORA ";

            if (idInmobiliaria != "TODOS")
            {
                sql += "WHERE P0403_ID_ARRENDADORA = ? AND (T04_CONTRATO.P0437_TIPO = ?) ORDER BY P0203_NOMBRE";
            }
            else
            {
                sql += "WHERE (T04_CONTRATO.P0437_TIPO = ?) ORDER BY P0203_NOMBRE";
            }
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                if (idInmobiliaria != "TODOS")
                {
                    comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                }
                comando.Parameters.Add("@TipoContrato", OdbcType.VarChar).Value = tipoContrato;
                List<ContratosEntity> listaContratos = new List<ContratosEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ContratosEntity contrato = new ContratosEntity()
                    {
                        IDContrato = reader["P0401_ID_CONTRATO"].ToString(),
                        IDCliente = reader["P0402_ID_ARRENDAT"].ToString(),
                        IDInmueble = reader["P0404_ID_EDIFICIO"].ToString(),
                        Cliente = reader["P0203_NOMBRE"].ToString(),
                        Inmueble = reader["P0703_NOMBRE"].ToString(),
                        Vigente = reader["P0428_A_PRORROGA"].ToString() == "V",
                        Conjunto = reader["P0303_NOMBRE"].ToString(),
                        Moneda = reader["P0407_MONEDA_FACT"].ToString(),
                        Identificador = reader["CAMPO4"].ToString(),
                        IDArrendadora = reader["P0403_ID_ARRENDADORA"].ToString(),
                        TipoContrato = reader["P0437_TIPO"].ToString()
                    };
                    listaContratos.Add(contrato);
                }
                reader.Close();
                conexion.Close();
                return listaContratos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static DomicilioEntity getDomicilioPorCliente(string idCliente)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = "SELECT P0503_CALLE_NUM, CAMPO1, CAMPO2, P0504_COLONIA, P0505_COD_POST, P0506_CIUDAD, P0507_ESTADO, P0508_PAIS FROM T05_DOMICILIO WHERE P0500_ID_ENTE = ? AND P0502_TIPO_DOMICILIO = 5";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = idCliente;
                DomicilioEntity domicilio = new DomicilioEntity();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    domicilio.Calle = reader["P0503_CALLE_NUM"].ToString().Replace(">", "-").Replace("<", "-").Replace("&", "AND").Replace("'", " ").Replace("\"", " ");
                    domicilio.NumeroExterior = reader["CAMPO1"].ToString().Replace(">", "-").Replace("<", "-").Replace("&", "AND").Replace("'", " ").Replace("\"", " ");
                    domicilio.NumeroInterior = reader["CAMPO2"].ToString().Replace(">", "-").Replace("<", "-").Replace("&", "AND").Replace("'", " ").Replace("\"", " ");
                    domicilio.Colonia = reader["P0504_COLONIA"].ToString().Replace(">", "-").Replace("<", "-").Replace("&", "AND").Replace("'", " ").Replace("\"", " ");
                    domicilio.CodigoPostal = reader["P0505_COD_POST"].ToString().Replace(">", "-").Replace("<", "-").Replace("&", "AND").Replace("'", " ").Replace("\"", " ");
                    domicilio.Ciudad = reader["P0506_CIUDAD"].ToString().Replace(">", "-").Replace("<", "-").Replace("&", "AND").Replace("'", " ").Replace("\"", " ");
                    domicilio.Estado = reader["P0507_ESTADO"].ToString().Replace(">", "-").Replace("<", "-").Replace("&", "AND").Replace("'", " ").Replace("\"", " ");
                    domicilio.Pais = reader["P0508_PAIS"].ToString().Replace(">", "-").Replace("<", "-").Replace("&", "AND").Replace("'", " ").Replace("\"", " ");
                    break;
                }
                reader.Close();
                conexion.Close();
                return domicilio;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static DomicilioEntity getDomicilioPorInmueble(string idInmueble)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = "SELECT P0503_CALLE_NUM, CAMPO1, CAMPO2, P0504_COLONIA, P0505_COD_POST, P0506_CIUDAD, P0507_ESTADO, P0508_PAIS FROM T05_DOMICILIO WHERE P0500_ID_ENTE = ? AND P0502_TIPO_DOMICILIO = 1";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInm", OdbcType.VarChar).Value = idInmueble;
                DomicilioEntity domicilio = new DomicilioEntity();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    domicilio.Calle = reader["P0503_CALLE_NUM"].ToString();
                    domicilio.NumeroExterior = reader["CAMPO1"].ToString();
                    domicilio.NumeroInterior = reader["CAMPO2"].ToString();
                    domicilio.Colonia = reader["P0504_COLONIA"].ToString();
                    domicilio.CodigoPostal = reader["P0505_COD_POST"].ToString();
                    domicilio.Ciudad = reader["P0506_CIUDAD"].ToString();
                    domicilio.Estado = reader["P0507_ESTADO"].ToString();
                    domicilio.Pais = reader["P0508_PAIS"].ToString();
                    break;
                }
                reader.Close();
                conexion.Close();
                return domicilio;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        //(idInmobiliaria, fechaInicial, fechafinal,todosConjuntos,SelectConjunto, allConjuntosSub, idConjunto, orderBy);
        public static List<ReciboEntity> getRecibosCobradosxFolio(string IdInmobiliaria, DateTime fechainicio, DateTime fechaFin, string IdConjunto, bool incluyeDetalle, bool orderBy, bool allConjuntosSub)
        {
            decimal factor = 1;

            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);


            string sql = @"SELECT T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC,T24_HISTORIA_RECIBOS.CAMPO4,T24_HISTORIA_RECIBOS.CAMPO3,T24_HISTORIA_RECIBOS.P2426_TIPO_DOC,T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO,
T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO,
T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.P2411_N_ARRENDATARIO, T24_HISTORIA_RECIBOS.P2410_MONEDA,  T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO,T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO,
T24_HISTORIA_RECIBOS.P2421_TC_PAGO,T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2419_TOTAL ,T24_HISTORIA_RECIBOS.P2416_IVA,T24_HISTORIA_RECIBOS.CAMPO_NUM1, T24_HISTORIA_RECIBOS.CAMPO_NUM2
FROM T24_HISTORIA_RECIBOS
LEFT JOIN T40_CFD on T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ?  and T24_HISTORIA_RECIBOS.P2426_TIPO_DOC in  ('R','P') and T24_HISTORIA_RECIBOS.P2406_STATUS = '2'
AND   T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? and T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ? ";
            if (incluyeDetalle == true)
            {
                sql = @"SELECT T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC,T24_HISTORIA_RECIBOS.CAMPO4,T24_HISTORIA_RECIBOS.CAMPO3,T24_HISTORIA_RECIBOS.P2426_TIPO_DOC,T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO,
T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO,
T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.P2411_N_ARRENDATARIO, T24_HISTORIA_RECIBOS.P2410_MONEDA,  T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO,T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO,
T24_HISTORIA_RECIBOS.P2421_TC_PAGO,T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2419_TOTAL ,T24_HISTORIA_RECIBOS.P2416_IVA,T24_HISTORIA_RECIBOS.CAMPO_NUM1, T24_HISTORIA_RECIBOS.CAMPO_NUM2
FROM T24_HISTORIA_RECIBOS
LEFT JOIN T40_CFD on T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ?  and T24_HISTORIA_RECIBOS.P2426_TIPO_DOC in  ('R','P','Z','U') and T24_HISTORIA_RECIBOS.P2406_STATUS = '2'
AND   T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? and T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ? ";

            }
            if (!string.IsNullOrEmpty(IdConjunto))
                sql += " AND CAMPO4 = ?";
            if (orderBy == true)
                sql += " ORDER BY T40_CFD.P4006_SERIE,T40_CFD.P4007_FOLIO ASC";
            if (orderBy == false)
                sql += " ORDER BY  T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO ASC";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IdInmobiliaria", OdbcType.VarChar).Value = IdInmobiliaria;
                comando.Parameters.Add("@fechaInicio", OdbcType.DateTime).Value = fechainicio.Date;
                comando.Parameters.Add("@fechaFin", OdbcType.DateTime).Value = fechaFin.Date;
                if (!string.IsNullOrEmpty(IdConjunto))
                    comando.Parameters.Add("@iDConjunto", OdbcType.VarChar).Value = IdConjunto;

                List<ReciboEntity> ListaRecibosCobrados = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    {

                        recibo.IDHistRec = (int)reader["P2444_ID_HIST_REC"];

                        recibo.IDConjunto = reader["CAMPO4"].ToString();
                        recibo.IDinmueble = reader["CAMPO3"].ToString();

                        if (allConjuntosSub || !string.IsNullOrEmpty(IdConjunto))
                        {
                            recibo.NombreConjunto = getNombreConjuntoByID(recibo.IDConjunto);
                            recibo.nombreSubconjunto = getNombreSubConjuntoByID(recibo.IDinmueble);
                        }
                        else
                        {
                            recibo.NombreConjunto = "Todos";
                            recibo.nombreSubconjunto = "Todos";
                        }
                        recibo.TipoDoc = reader["P2426_TIPO_DOC"].ToString();
                        recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                        recibo.FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                        recibo.FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                        recibo.Numero = (int)reader["P2403_NUM_RECIBO"];
                        recibo.Serie = reader["P4006_SERIE"].ToString();
                        recibo.Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"];
                        recibo.Inmueble = reader["CAMPO9"].ToString();
                        recibo.NombreCliente = reader["P2411_N_ARRENDATARIO"].ToString();
                        recibo.Moneda = reader["P2410_MONEDA"] == DBNull.Value ? "P" : (string)reader["P2410_MONEDA"].ToString(); //.ToString(); // Moneda contrato
                        recibo.MonedaPago = reader["P2420_MONEDA_PAGO"] == DBNull.Value ? "P" : (string)reader["P2420_MONEDA_PAGO"].ToString();
                        recibo.TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : (decimal)reader["P2414_TIPO_CAMBIO"];
                        recibo.TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2421_TC_PAGO"];
                        recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                        recibo.IVA = reader["P2416_IVA"] == DBNull.Value ? 0 : (decimal)reader["P2416_IVA"];
                        recibo.ISR = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : (decimal)reader["CAMPO_NUM1"];
                        recibo.IVARetenido = reader["CAMPO_NUM2"] == DBNull.Value ? 0 : (decimal)reader["CAMPO_NUM2"];
                        recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                        if (recibo.Moneda == "D" && recibo.MonedaPago == "P")
                        {
                            recibo.TipoCambioPago = recibo.TipoCambioPago;
                            recibo.Importe = recibo.Importe * recibo.TipoCambioPago;
                            recibo.IVA = recibo.IVA * recibo.TipoCambioPago;
                            recibo.ISR = recibo.ISR * recibo.TipoCambioPago;
                            recibo.IVARetenido = recibo.IVARetenido * recibo.TipoCambioPago;
                            recibo.Total = 0;
                            //recibo.Total = recibo.Importe + recibo.IVA + recibo.ISR + recibo.IVARetenido;

                        }
                        if (recibo.Moneda == "P" && recibo.MonedaPago == "D")
                        {
                            //recibo.TipoCambioPago = factor / recibo.TipoCambioPago;
                            recibo.Importe = recibo.Importe / recibo.TipoCambioPago;
                            recibo.IVA = recibo.IVA / recibo.TipoCambioPago;
                            recibo.ISR = recibo.ISR / recibo.TipoCambioPago;
                            recibo.IVARetenido = recibo.IVARetenido / recibo.TipoCambioPago;
                            recibo.Total = 0;
                            recibo.Total = recibo.Importe + recibo.IVA;
                        }

                        if (recibo.TipoDoc == "X" || recibo.TipoDoc == "P")
                        {
                            List<ReciboEntity> SerieAndFolio = getSeriFolioCuandoEsPagoParcial(recibo.Numero);
                            recibo.TipoPago = "Pago parcial";
                            if (SerieAndFolio != null)
                            {
                                if (SerieAndFolio.Count > 0)
                                {
                                    recibo.Serie = SerieAndFolio[0].Serie;
                                    recibo.Folio = SerieAndFolio[0].Folio;
                                }
                            }

                        }
                        else
                        {
                            recibo.TipoPago = "Completo";
                        }

                        recibo.TotalPagado = recibo.saldoAFavor + recibo.Total;
                        recibo.saldoAFavor = recibo.saldoAFavor;



                        if (recibo.TipoDoc != "W")
                        {
                            ListaRecibosCobrados.Add(recibo);
                        }
                        else
                        {
                            if (recibo.Numero != recibo.IDHistRec)
                                ListaRecibosCobrados.Add(recibo);
                        }




                    }
                }
                reader.Close();
                conexion.Close();
                return ListaRecibosCobrados;


            }
            catch
            {
                conexion.Close();
                return null;
            }



        }

        public static List<ReciboEntity> getSeriFolioCuandoEsPagoParcial(int idHisRec)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            string sql = @"Select P4006_SERIE, T40_CFD.P4007_FOLIO,P2426_TIPO_DOC,P2403_NUM_RECIBO from T40_CFD 
                            JOIN T24_HISTORIA_RECIBOS on T40_CFD.P4001_ID_HIST_REC = T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO
                            where T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO = ?";

            try
            {

                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idHisRec", OdbcType.Int).Value = idHisRec;

                List<ReciboEntity> ListaSerieFolio = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity()
                    {
                        Serie = reader["P4006_SERIE"].ToString(),
                        Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],

                    };


                    ListaSerieFolio.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return ListaSerieFolio;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        public static ReciboEntity getSeriFolioCuandoEsPagoParcial2(int idHisRec)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            string sql = @"Select P4006_SERIE, T40_CFD.P4007_FOLIO,P2426_TIPO_DOC,P2403_NUM_RECIBO from T40_CFD 
                            JOIN T24_HISTORIA_RECIBOS on T40_CFD.P4001_ID_HIST_REC = T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO
                            where T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO = ?";

            try
            {

                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idHisRec", OdbcType.Int).Value = idHisRec;

                ReciboEntity recibo = new ReciboEntity();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {

                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"];
                }
                reader.Close();
                conexion.Close();
                return recibo;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getListaRecibosAnalisisDeudores(ContratosEntity contrato, DateTime fechaIni, DateTime fechaFin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            //Se agregó la 2da seccion del query para incluir las facturas con notas de credito parciales.
            //Modify by Uz 26/10/2015
            string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, T24_HISTORIA_RECIBOS.* FROM T24_HISTORIA_RECIBOS
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
WHERE P2406_STATUS IN ('2') AND P2401_ID_ARRENDADORA = ? 
AND p2426_tipo_doc IN ('X','W')
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2408_FECHA_PAGADO >= ?)
AND (P2402_ID_ARRENDATARIO = ?)
AND (P2418_ID_CONTRATO = ?)
UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, T24_HISTORIA_RECIBOS.* FROM T24_HISTORIA_RECIBOS
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
WHERE P2406_STATUS IN ('1') AND P2401_ID_ARRENDADORA = ? 
AND p2426_tipo_doc IN ('X','W', 'L')
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2402_ID_ARRENDATARIO = ?)
AND (P2418_ID_CONTRATO = ?)
UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, r.* FROM T24_HISTORIA_RECIBOS r 
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC= r.P2444_ID_HIST_REC
WHERE EXISTS (SELECT * FROM T24_HISTORIA_RECIBOS u WHERE r.P2444_ID_HIST_REC=u.P2425_DEB_REC)
AND r.P2426_TIPO_DOC IN ('X') AND r.P2406_STATUS IN ('1') AND P2401_ID_ARRENDADORA = ?
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2402_ID_ARRENDATARIO = ?)
AND (P2418_ID_CONTRATO = ?) 

UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, z.* 
FROM T24_HISTORIA_RECIBOS z
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = z.P2444_ID_HIST_REC
WHERE z.p2426_tipo_doc in ('P','U') AND z.P2406_STATUS = '2' 
AND z.P2401_ID_ARRENDADORA = ?
AND (z.P2409_FECHA_EMISION <= ?)
AND (z.P2408_FECHA_PAGADO >= ? 
AND z.P2408_FECHA_PAGADO <= ?)
AND (z.P2402_ID_ARRENDATARIO = ?)
AND (z.P2418_ID_CONTRATO = ?)

UNION ALL
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, y.* FROM T24_HISTORIA_RECIBOS y
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = y.P2444_ID_HIST_REC
WHERE y.P2401_ID_ARRENDADORA = ?
AND
   (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('1')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('2')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2408_Fecha_pagado >= ?
   AND y.P2408_Fecha_pagado <= ?
   )
   OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('2')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )
OR
   (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('0','3')
   AND y.P2426_TIPO_DOC IN ('T')
   AND y.CAMPO_DATE1  >= ?
   AND y.CAMPO_DATE1 <= ?
   )
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('2','5','4')
   AND y.P2426_TIPO_DOC IN ('G')
   AND y.P2408_Fecha_pagado >= ?
   AND y.P2408_Fecha_pagado <= ?
   )  
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('1')
   AND y.P2426_TIPO_DOC IN ('G')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )       
order by 11, 1, 2, 8, 28";
            //Se agrega el ultimo para recibos tipo G para depositos en garantía by UZ 15/12/2015
            //Se quitan los recibos tipo T con Status 3 (recibos temporales de convenio de pago cancelados) by Uz 27/05/2016
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaIni2", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@FechaFinX", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIniX", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@FechaFin2", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni3", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin3", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte2", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt2", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@IDArr3", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte3", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt3", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni4", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin4", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte4", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt4", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni5", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin5", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte5", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt5", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni6", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin6", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte6", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt6", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni7", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin7", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte7", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt7", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni8", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin8", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte8", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt8", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni9", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin9", OdbcType.DateTime).Value = fechaFin;

                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity()
                    {
                        IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                        Numero = (int)reader["P2403_NUM_RECIBO"],
                        FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                        FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                        TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                        Moneda = reader["P2410_MONEDA"].ToString(),
                        Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"],//Agregado para reporte Inmobic,Reporte Global
                        IVA = reader["P2416_IVA"] == DBNull.Value ? 0 : (decimal)reader["P2416_IVA"],//Agregado para reporte Inmobic,Reporte Global
                        Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"],
                        Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2413_PAGO"],
                        Estatus = reader["P2406_STATUS"].ToString(),
                        PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"],
                        TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2421_TC_PAGO"],
                        TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : (decimal)reader["P2414_TIPO_CAMBIO"],
                        Periodo = reader["P2404_PERIODO"].ToString(),
                        Serie = reader["P4006_SERIE"].ToString(),
                        Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],
                        MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                        Concepto = reader["P2412_CONCEPTO"].ToString(),
                        Inmueble = reader["CAMPO9"].ToString(),
                        IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                        RutaPdfCFDI = reader["P4023_CAMPO1"] == DBNull.Value ? "" : reader["P4023_CAMPO1"].ToString()
                    };
                    if (!string.IsNullOrEmpty(recibo.RutaPdfCFDI))
                    {
                        string ruta = "file:///" + recibo.RutaPdfCFDI.Replace(".xml", ".pdf");
                        ruta = ruta.Replace("\\", "\\\\");

                        recibo.RutaPdfCFDI = string.Empty;
                        recibo.RutaPdfCFDI = ruta;
                    }
                    //Condición agregada para depositos en garantia by Uz 15/12/2015
                    if (recibo.TipoDoc == "G")
                    {
                        recibo.Serie = "DEPGAR";
                        recibo.Folio = recibo.Numero;
                    }
                    //Condición agregada para cuando son recibos sin factura by Uz 15/12/2015
                    if (string.IsNullOrEmpty(recibo.Serie) && recibo.Folio == null)
                    {
                        recibo.Serie = "REC";
                        recibo.Folio = recibo.Numero;
                    }
                    if (recibo.FechaEmision >= fechaIni)
                    {
                        if (recibo.Moneda == "P")
                        {
                            if (recibo.TipoDoc == "R" || recibo.TipoDoc == "X" || recibo.TipoDoc == "Z" || recibo.TipoDoc == "D"
                                || recibo.TipoDoc == "T" || recibo.TipoDoc == "W" || recibo.TipoDoc == "G" || recibo.TipoDoc == "L")
                            {
                                if (contrato.Moneda == "P")
                                    recibo.Cargo = recibo.Total;
                                else
                                    recibo.Cargo = recibo.Total / recibo.TipoCambio;
                            }
                            else if (recibo.TipoDoc == "B")
                            {
                                if (contrato.Moneda == "P")
                                    recibo.Abono = recibo.Total;
                                else
                                    recibo.Abono = recibo.Total / recibo.TipoCambio;
                            }
                            if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                recibo.Abono = recibo.Cargo;
                        }
                        else
                        {
                            if (recibo.TipoDoc == "R" || recibo.TipoDoc == "X" || recibo.TipoDoc == "Z" || recibo.TipoDoc == "D"
                                || recibo.TipoDoc == "T" || recibo.TipoDoc == "W" || recibo.TipoDoc == "G" || recibo.TipoDoc == "L")
                            {
                                if (contrato.Moneda == "D")
                                    recibo.Cargo = recibo.Total;
                                else
                                    recibo.Cargo = recibo.Total * recibo.TipoCambio;
                            }
                            else if (recibo.TipoDoc == "B")
                            {
                                if (contrato.Moneda == "D")
                                    recibo.Abono = recibo.Total;
                                else
                                    recibo.Abono = recibo.Total * recibo.TipoCambio;
                            }
                            if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                recibo.Abono = recibo.Cargo;
                        }
                    }
                    if (recibo.Estatus == "2" || (recibo.TipoDoc == "G" && recibo.Estatus == "5"))
                    {
                        if (recibo.FechaPago <= fechaFin)
                        {
                            if (recibo.MonedaPago == "P")
                            {
                                if (contrato.Moneda == "P")
                                {
                                    recibo.Abono = recibo.Pago;
                                    //if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                    //    recibo.Abono = recibo.PagoParcial;
                                    //else 
                                    if (recibo.TipoDoc == "G" && recibo.Estatus == "5")//Condición agregada para depositos en garantia by Uz 15/12/2015
                                        recibo.Abono = recibo.Total;
                                }
                                else
                                {
                                    recibo.Abono = recibo.Pago / recibo.TipoCambioPago;
                                    if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                        recibo.Abono = recibo.PagoParcial / recibo.TipoCambioPago;
                                    else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                        recibo.Abono = recibo.Total / recibo.TipoCambioPago;
                                }
                            }
                            else
                            {
                                if (contrato.Moneda == "D")
                                {
                                    recibo.Abono = recibo.Pago;
                                    if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                        recibo.Abono = recibo.PagoParcial;
                                    else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                        recibo.Abono = recibo.Total;
                                }
                                else
                                {
                                    recibo.Abono = recibo.Pago * recibo.TipoCambioPago;
                                    if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                        recibo.Abono = recibo.PagoParcial * recibo.TipoCambioPago;
                                    else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                        recibo.Abono = recibo.Total * recibo.TipoCambioPago;
                                }
                            }
                        }
                        else
                            recibo.FechaPago = null;
                    }
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getListaRecibos(ContratosEntity contrato, DateTime fechaIni, DateTime fechaFin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            //Se agregó la 2da seccion del query para incluir las facturas con notas de credito parciales.
            //Modify by Uz 26/10/2015
            string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, T24_HISTORIA_RECIBOS.* FROM T24_HISTORIA_RECIBOS
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
WHERE P2406_STATUS IN ('2') AND P2401_ID_ARRENDADORA = ? 
AND p2426_tipo_doc IN ('X','W')
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2408_FECHA_PAGADO >= ?)
AND (P2402_ID_ARRENDATARIO = ?)
AND (P2418_ID_CONTRATO = ?)
UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, T24_HISTORIA_RECIBOS.* FROM T24_HISTORIA_RECIBOS
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
WHERE P2406_STATUS IN ('1') AND P2401_ID_ARRENDADORA = ? 
AND p2426_tipo_doc IN ('X','W', 'L')
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2402_ID_ARRENDATARIO = ?)
AND (P2418_ID_CONTRATO = ?)
UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, r.* FROM T24_HISTORIA_RECIBOS r 
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC= r.P2444_ID_HIST_REC
WHERE EXISTS (SELECT * FROM T24_HISTORIA_RECIBOS u WHERE r.P2444_ID_HIST_REC=u.P2425_DEB_REC)
AND r.P2426_TIPO_DOC IN ('X') AND r.P2406_STATUS IN ('1') AND P2401_ID_ARRENDADORA = ?
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2402_ID_ARRENDATARIO = ?)
AND (P2418_ID_CONTRATO = ?) 

UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, z.* 
FROM T24_HISTORIA_RECIBOS z
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = z.P2444_ID_HIST_REC
WHERE z.p2426_tipo_doc in ('P','U') AND z.P2406_STATUS = '2' 
AND z.P2401_ID_ARRENDADORA = ?
AND (z.P2409_FECHA_EMISION <= ?)
AND (z.P2408_FECHA_PAGADO >= ? 
AND z.P2408_FECHA_PAGADO <= ?)
AND (z.P2402_ID_ARRENDATARIO = ?)
AND (z.P2418_ID_CONTRATO = ?)

UNION ALL
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, y.* FROM T24_HISTORIA_RECIBOS y
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = y.P2444_ID_HIST_REC
WHERE y.P2401_ID_ARRENDADORA = ?
AND
   (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('1')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('2')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2408_Fecha_pagado >= ?
   AND y.P2408_Fecha_pagado <= ?
   )
   OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('2')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )
OR
   (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('0','3')
   AND y.P2426_TIPO_DOC IN ('T')
   AND y.CAMPO_DATE1  >= ?
   AND y.CAMPO_DATE1 <= ?
   )
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('2','5','4')
   AND y.P2426_TIPO_DOC IN ('G')
   AND y.P2408_Fecha_pagado >= ?
   AND y.P2408_Fecha_pagado <= ?
   )  
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('1')
   AND y.P2426_TIPO_DOC IN ('G')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )       
order by 11, 1, 2, 8, 28";
            //Se agrega el ultimo para recibos tipo G para depositos en garantía by UZ 15/12/2015
            //Se quitan los recibos tipo T con Status 3 (recibos temporales de convenio de pago cancelados) by Uz 27/05/2016
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaIni2", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IDContrato;

                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@FechaFinX", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIniX", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IDContrato;

                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@FechaFin2", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni3", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin3", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte2", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt2", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@IDArr3", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte3", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt3", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni4", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin4", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte4", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt4", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni5", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin5", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte5", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt5", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni6", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin6", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte6", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt6", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni7", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin7", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte7", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt7", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni8", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin8", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte8", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt8", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni9", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin9", OdbcType.DateTime).Value = fechaFin;

                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity()
                    {
                        IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                        Numero = (int)reader["P2403_NUM_RECIBO"],
                        FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                        FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                        TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                        Moneda = reader["P2410_MONEDA"].ToString(),
                        Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"],//Agregado para reporte Inmobic,Reporte Global
                        IVA = reader["P2416_IVA"] == DBNull.Value ? 0 : (decimal)reader["P2416_IVA"],//Agregado para reporte Inmobic,Reporte Global
                        Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"],
                        Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2413_PAGO"],
                        Estatus = reader["P2406_STATUS"].ToString(),
                        PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"],
                        TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2421_TC_PAGO"],
                        TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : (decimal)reader["P2414_TIPO_CAMBIO"],
                        Periodo = reader["P2404_PERIODO"].ToString(),
                        Serie = reader["P4006_SERIE"].ToString(),
                        Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],
                        MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                        Concepto = reader["P2412_CONCEPTO"].ToString(),
                        Inmueble = reader["CAMPO9"].ToString(),
                        IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                        RutaPdfCFDI = reader["P4023_CAMPO1"] == DBNull.Value ? "" : reader["P4023_CAMPO1"].ToString()
                    };
                    if (!string.IsNullOrEmpty(recibo.RutaPdfCFDI))
                    {
                        string ruta = "file:///" + recibo.RutaPdfCFDI.Replace(".xml", ".pdf");
                        ruta = ruta.Replace("\\", "\\\\");

                        recibo.RutaPdfCFDI = string.Empty;
                        recibo.RutaPdfCFDI = ruta;
                    }
                    //Condición agregada para depositos en garantia by Uz 15/12/2015
                    if (recibo.TipoDoc == "G")
                    {
                        recibo.Serie = "DEPGAR";
                        recibo.Folio = recibo.Numero;
                    }
                    //Condición agregada para cuando son recibos sin factura by Uz 15/12/2015
                    if (string.IsNullOrEmpty(recibo.Serie) && recibo.Folio == null)
                    {
                        recibo.Serie = "REC";
                        recibo.Folio = recibo.Numero;
                    }
                    if (recibo.FechaEmision >= fechaIni)
                    {
                        if (recibo.Moneda == "P")
                        {
                            if (recibo.TipoDoc == "R" || recibo.TipoDoc == "X" || recibo.TipoDoc == "Z" || recibo.TipoDoc == "D"
                                || recibo.TipoDoc == "T" || recibo.TipoDoc == "W" || recibo.TipoDoc == "G" || recibo.TipoDoc == "L")
                            {
                                if (contrato.Moneda == "P")
                                    recibo.Cargo = recibo.Total;
                                else
                                    recibo.Cargo = recibo.Total / recibo.TipoCambio;
                            }
                            else if (recibo.TipoDoc == "B")
                            {
                                if (contrato.Moneda == "P")
                                    recibo.Abono = recibo.Total;
                                else
                                    recibo.Abono = recibo.Total / recibo.TipoCambio;
                            }
                            if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                recibo.Abono = recibo.Cargo;
                        }
                        else
                        {
                            if (recibo.TipoDoc == "R" || recibo.TipoDoc == "X" || recibo.TipoDoc == "Z" || recibo.TipoDoc == "D"
                                || recibo.TipoDoc == "T" || recibo.TipoDoc == "W" || recibo.TipoDoc == "G" || recibo.TipoDoc == "L")
                            {
                                if (contrato.Moneda == "D")
                                    recibo.Cargo = recibo.Total;
                                else
                                    recibo.Cargo = recibo.Total * recibo.TipoCambio;
                            }
                            else if (recibo.TipoDoc == "B")
                            {
                                if (contrato.Moneda == "D")
                                    recibo.Abono = recibo.Total;
                                else
                                    recibo.Abono = recibo.Total * recibo.TipoCambio;
                            }
                            if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                recibo.Abono = recibo.Cargo;
                        }
                    }
                    if (recibo.Estatus == "2" || (recibo.TipoDoc == "G" && recibo.Estatus == "5"))
                    {
                        if (recibo.FechaPago <= fechaFin)
                        {
                            if (recibo.MonedaPago == "P")
                            {
                                if (contrato.Moneda == "P")
                                {
                                    recibo.Abono = recibo.Pago;
                                    //if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                    //    recibo.Abono = recibo.PagoParcial;
                                    //else 
                                    if (recibo.TipoDoc == "G" && recibo.Estatus == "5")//Condición agregada para depositos en garantia by Uz 15/12/2015
                                        recibo.Abono = recibo.Total;
                                }
                                else
                                {
                                    recibo.Abono = recibo.Pago / recibo.TipoCambioPago;
                                    if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                        recibo.Abono = recibo.PagoParcial / recibo.TipoCambioPago;
                                    else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                        recibo.Abono = recibo.Total / recibo.TipoCambioPago;
                                }
                            }
                            else
                            {
                                if (contrato.Moneda == "D")
                                {
                                    recibo.Abono = recibo.Pago;
                                    if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                        recibo.Abono = recibo.PagoParcial;
                                    else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                        recibo.Abono = recibo.Total;
                                }
                                else
                                {
                                    recibo.Abono = recibo.Pago * recibo.TipoCambioPago;
                                    if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                        recibo.Abono = recibo.PagoParcial * recibo.TipoCambioPago;
                                    else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                        recibo.Abono = recibo.Total * recibo.TipoCambioPago;
                                }
                            }
                        }
                        else
                            recibo.FechaPago = null;
                    }
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        struct datosMoratoriosInmobiliaria
        {
            public string idInmo;
            public int tipoInteresMoratorio;
            public int aplicarGraciaARecibos;
            public void sinValores(string id)
            {
                idInmo = id;
                tipoInteresMoratorio = 0;
                aplicarGraciaARecibos = 0;
            }
        }

        public static List<ReciboEntity> getListaRecibosPendientesDeCobro(ContratosEntity contCliente, DateTime fechaFin, bool esDolar, string idArr)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            ConfiguracionMoratoriosEntity configMoratorios = new ConfiguracionMoratoriosEntity();
            datosMoratoriosInmobiliaria datosInmo = new datosMoratoriosInmobiliaria();
            #region CONFIG MORATORIOS DE INMOBILIARIA
            try
            {
                string sqlMora = @"SELECT P0401_ID_CONTRATO, P0402_ID_ARRENDAT, P0403_ID_ARRENDADORA, P0437_TIPO, 
       T04_CONTRATO.CAMPO1 AS TIPO_CALC_MORATORIO, T04_CONTRATO.CAMPO_NUM2 AS TASA_INTERES_MORA, T04_CONTRATO.CAMPO_NUM3 AS DIAS_DE_GRACIA, 
       T01_ARRENDADORA.CAMPO_NUM1 AS DIAS_MESES_MORA , T01_ARRENDADORA.CAMPO_NUM2 AS APLICAR_DIAS_GRACIA           
FROM T04_CONTRATO
LEFT JOIN T01_ARRENDADORA ON T04_CONTRATO.P0403_ID_ARRENDADORA= T01_ARRENDADORA.P0101_ID_ARR
WHERE P0401_ID_CONTRATO=? AND T01_ARRENDADORA.P0101_ID_ARR=?";
                OdbcCommand comando = new OdbcCommand(sqlMora, conexion);
                comando.Parameters.Add("@IDCont", OdbcType.VarChar).Value = contCliente.IDContrato;
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idArr;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                reader.Close();
                conexion.Close();
                configMoratorios.idInmo = idArr;
                configMoratorios.idContrato = contCliente.IDContrato;
                decimal valor = string.IsNullOrWhiteSpace(dt.Rows[0]["DIAS_MESES_MORA"].ToString()) ? 0 : (decimal)(dt.Rows[0]["DIAS_MESES_MORA"]);
                configMoratorios.diasMesesMoratorios = Convert.ToInt32(valor);
                valor = string.IsNullOrWhiteSpace(dt.Rows[0]["APLICAR_DIAS_GRACIA"].ToString()) ? 2 : (decimal)(dt.Rows[0]["APLICAR_DIAS_GRACIA"]);
                configMoratorios.aplicarDiasGracia = Convert.ToInt32(valor);
                string dias = dt.Rows[0]["DIAS_DE_GRACIA"].ToString();
                configMoratorios.diasDeGracia = string.IsNullOrWhiteSpace(dt.Rows[0]["DIAS_DE_GRACIA"].ToString()) ? 0 : Convert.ToInt32(dt.Rows[0]["DIAS_DE_GRACIA"]);
                configMoratorios.tipoCalculoMoratorio = dt.Rows[0]["TIPO_CALC_MORATORIO"].ToString();
                configMoratorios.tasaInteres = (decimal)dt.Rows[0]["TASA_INTERES_MORA"];
            }
            catch (Exception ex)
            {
                configMoratorios.idInmo = idArr;
                configMoratorios.idContrato = contCliente.IDContrato;
                configMoratorios.diasMesesMoratorios = 0;
                configMoratorios.aplicarDiasGracia = 0;
                configMoratorios.tipoCalculoMoratorio = string.Empty;
                configMoratorios.tasaInteres = 0;
                configMoratorios.diasDeGracia = 0;
                conexion.Close();
            }
            #endregion
            #region CONFIG MORATORIOS DE CONTRATO
            //try
            //{
            //    string sqlMora = @"SELECT P0101_ID_ARR, CAMPO_NUM1 AS TIPO_INTERES_MORATORIO, CAMPO_NUM2 AS APLICAR_GRACIA_RECIBOS FROM T01_ARRENDADORA WHERE P0101_ID_ARR=? ";
            //    OdbcCommand comando = new OdbcCommand(sqlMora, conexion);
            //    comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idArr;
            //    conexion.Open();
            //    OdbcDataReader reader = comando.ExecuteReader();
            //    DataTable dt = new DataTable();
            //    dt.Load(reader);
            //    reader.Close();
            //    conexion.Close();
            //    datosInmo.idInmo = idArr;
            //    datosInmo.tipoInteresMoratorio = string.IsNullOrWhiteSpace(dt.Rows[0]["CAMPO_NUM1"].ToString()) ? 0 : (int)dt.Rows[0]["CAMPO_NUM1"];
            //    datosInmo.aplicarGraciaARecibos = string.IsNullOrWhiteSpace(dt.Rows[0]["CAMPO_NUM2"].ToString()) ? 0 : (int)dt.Rows[0]["CAMPO_NUM2"];
            //}
            //catch (Exception ex)
            //{
            //    datosInmo.sinValores(idArr);
            //    conexion.Close();
            //}
            #endregion

            #region COMANDO SQL
            string sql = @"SELECT P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2404_PERIODO, P2405_IMPORTE, P2426_TIPO_DOC, P2406_STATUS, 
P2409_FECHA_EMISION, P2410_MONEDA, P2411_N_ARRENDATARIO, P2412_CONCEPTO, P2418_ID_CONTRATO, P2414_TIPO_CAMBIO, P2415_NO_EDIF, P2416_IVA, 
P2419_TOTAL, P2427_CTD_PAG, P2428_T_IVA, P2429_PER_ARR, P2430_PER_CTE, P2444_ID_HIST_REC, P2403_NUM_RECIBO, CAMPO1, CAMPO2, CAMPO3, CAMPO4, 
CAMPO5, CAMPO9, CAMPO_DATE1, CAMPO_DATE2, CAMPO_NUM14, P4006_SERIE, P4007_FOLIO, P2407_COMENTARIO FROM T24_HISTORIA_RECIBOS 
LEFT JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
WHERE P2406_STATUS = '1' ";
            if (contCliente.TipoContrato == "R")
            {
                sql += @"AND P2426_TIPO_DOC IN ('R','Z','X','W','T','O','L') AND T24_HISTORIA_RECIBOS.P2427_CTD_PAG > 0  
AND P2418_ID_CONTRATO = ? AND (P2409_FECHA_EMISION <= ?) ";
            }
            else if (contCliente.TipoContrato == "V")
            {
                sql += @"AND P2426_TIPO_DOC IN ('V') AND T24_HISTORIA_RECIBOS.P2406_STATUS='1'  
AND P2418_ID_CONTRATO = ? AND (CAMPO_DATE1 <= ?) ";
            }
            if (!string.IsNullOrWhiteSpace(idArr))
            {
                sql += "AND P2401_ID_ARRENDADORA = ? ";
            }
            sql += "AND P2410_MONEDA = ? ORDER BY P4006_SERIE, P4007_FOLIO";
            #endregion

            try
            {
                string moneda = esDolar ? "D" : "P";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDContrato", OdbcType.VarChar).Value = contCliente.IDContrato;
                //comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                if (!string.IsNullOrWhiteSpace(idArr))
                {
                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArr;
                }
                comando.Parameters.Add("@Moneda", OdbcType.VarChar).Value = moneda;

                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity()
                    {
                        IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                        Numero = (int)reader["P2403_NUM_RECIBO"],
                        FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                        //FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                        TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                        Moneda = reader["P2410_MONEDA"].ToString(),
                        Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"],
                        Importe = (decimal)reader["P2405_IMPORTE"],
                        IVA = (decimal)reader["P2416_IVA"],
                        //Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2413_PAGO"],
                        Estatus = reader["P2406_STATUS"].ToString(),
                        //PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"],
                        CantidadPorPagar = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"],
                        //TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2421_TC_PAGO"],
                        TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : (decimal)reader["P2414_TIPO_CAMBIO"],
                        Periodo = reader["P2404_PERIODO"].ToString(),
                        Serie = reader["P4006_SERIE"].ToString(),
                        Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],
                        //MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                        Concepto = reader["P2412_CONCEPTO"].ToString(),
                        Inmueble = reader["CAMPO9"].ToString(),
                        IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                        IDContrato = reader["P2418_ID_CONTRATO"].ToString(),
                        Info = reader["P2407_COMENTARIO"] == DBNull.Value ? "" : reader["P2407_COMENTARIO"].ToString(),
                        //Agregado 05/11/2015 by Uz                        
                        FechaVencimiento = reader["CAMPO_DATE1"] != DBNull.Value ? Convert.ToDateTime(reader["CAMPO_DATE1"]) : new DateTime(1901, 1, 1),
                        FechaMoratorios = reader["CAMPO_DATE2"] != DBNull.Value ? Convert.ToDateTime(reader["CAMPO_DATE2"]) : new DateTime(1901, 1, 1)
                    };
                    if (string.IsNullOrEmpty(recibo.Serie) && recibo.Folio == null)
                    {
                        recibo.Serie = "REC";
                        recibo.Folio = recibo.Numero;
                    }
                    if (contCliente.TipoContrato == "V")
                    {
                        //Para recibos de venta P2427_CTD_PAG corresponde al total del recibo y P2419_TOTAL corresponde al saldo por pagar 
                        decimal total = recibo.CantidadPorPagar;
                        recibo.CantidadPorPagar = recibo.Total;
                        recibo.Total = total;
                    }
                    //recibo.InteresesMoratorios = obtenerMoratorios(recibo, DateTime.Today, configMoratorios);
                    if (recibo.CantidadPorPagar > 1)
                        listaRecibos.Add(recibo);
                }

                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }


        }

        //        public static List<ReciboEntity> getListaRecibosPendientesDeCobro(ClienteEntity cliente, DateTime fechaIni, DateTime fechaFin, bool esDolar, string idArr)
        //        {
        //            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
        ////            string sql = @"SELECT P4006_SERIE, P4007_FOLIO, CAMPO9 AS INMUEBLE, CAMPO5 AS CONUNTO, P2410_MONEDA, P2405_IMPORTE, 
        ////P2416_IVA, P2419_TOTAL, P2427_CTD_PAG, P2444_ID_HIST_REC, P2409_FECHA_EMISION, P2426_TIPO_DOC, 
        ////P2412_CONCEPTO, P2402_ID_ARRENDATARIO, P2411_N_ARRENDATARIO 
        ////FROM TT24_COBRANZA 
        ////WHERE P2402_ID_ARRENDATARIO = ? AND (P2409_FECHA_EMISION >= ? AND P2409_FECHA_EMISION <= ?) 
        ////AND P2401_ID_ARRENDADORA = ? AND P2410_MONEDA = ? ORDER BY P4006_SERIE, P4007_FOLIO";

        //            string sql = @"SELECT P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2404_PERIODO, P2405_IMPORTE, P2426_TIPO_DOC, P2406_STATUS, 
        //P2409_FECHA_EMISION, P2410_MONEDA, P2411_N_ARRENDATARIO, P2412_CONCEPTO, P2418_ID_CONTRATO, P2414_TIPO_CAMBIO, P2415_NO_EDIF, P2416_IVA, 
        //P2419_TOTAL, P2427_CTD_PAG, P2428_T_IVA, P2429_PER_ARR, P2430_PER_CTE, P2444_ID_HIST_REC, P2403_NUM_RECIBO, CAMPO1, CAMPO2, CAMPO3, CAMPO4, 
        //CAMPO5, CAMPO9, CAMPO_DATE1, CAMPO_NUM14, P4006_SERIE, P4007_FOLIO FROM T24_HISTORIA_RECIBOS 
        //LEFT JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
        //WHERE P2406_STATUS = '1' AND P2426_TIPO_DOC IN ('R','Z','X','W') 
        //AND T24_HISTORIA_RECIBOS.P2427_CTD_PAG > 0  AND P2402_ID_ARRENDATARIO = ? AND (P2409_FECHA_EMISION >= ? AND P2409_FECHA_EMISION <= ?) ";
        //            if(!string.IsNullOrWhiteSpace(idArr))
        //            {              
        //                sql+="AND P2401_ID_ARRENDADORA = ? ";
        //            }
        //            sql+= "AND P2410_MONEDA = ? ORDER BY P4006_SERIE, P4007_FOLIO";

        //            try
        //            {
        //                string moneda = esDolar ? "D" : "P";
        //                OdbcCommand comando = new OdbcCommand(sql, conexion);
        //                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = cliente.IDCliente;
        //                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaIni;
        //                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaFin;
        //                if(!string.IsNullOrWhiteSpace(idArr))            
        //                {  
        //                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArr;
        //                }
        //                comando.Parameters.Add("@Moneda", OdbcType.VarChar).Value = moneda;

        //                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
        //                conexion.Open();
        //                OdbcDataReader reader = comando.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    //
        //                    // TO DO: Terminar este método
        //                    //
        //                    ReciboEntity recibo = new ReciboEntity()
        //                    {
        //                        IDHistRec = (int)reader["P2444_ID_HIST_REC"],
        //                        Numero = (int)reader["P2403_NUM_RECIBO"],
        //                        FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
        //                        //FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
        //                        TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
        //                        Moneda = reader["P2410_MONEDA"].ToString(),
        //                        Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"],
        //                        Importe= (decimal)reader["P2405_IMPORTE"],
        //                        IVA = (decimal)reader["P2416_IVA"],
        //                        //Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2413_PAGO"],
        //                        Estatus = reader["P2406_STATUS"].ToString(),
        //                        //PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"],
        //                        CantidadPorPagar = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"],
        //                        //TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2421_TC_PAGO"],
        //                        TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : (decimal)reader["P2414_TIPO_CAMBIO"],
        //                        Periodo = reader["P2404_PERIODO"].ToString(),
        //                        Serie = reader["P4006_SERIE"].ToString(),
        //                        Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],
        //                        //MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
        //                        Concepto = reader["P2412_CONCEPTO"].ToString(),
        //                        Inmueble = reader["CAMPO9"].ToString(),
        //                        IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString()
        //                    };
        //                    listaRecibos.Add(recibo);
        //                }

        //                reader.Close();
        //                conexion.Close();
        //                return listaRecibos;
        //            }
        //            catch (Exception ex)
        //            {
        //                conexion.Close();
        //                return null;
        //            }


        //        }

        public static List<ReciboEntity> getListaRecibosPendientesDeCobro(ClienteEntity cliente, DateTime fechaFin, bool esDolar, string idArr)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            ConfiguracionMoratoriosEntity configMoratorios = new ConfiguracionMoratoriosEntity();
            datosMoratoriosInmobiliaria datosInmo = new datosMoratoriosInmobiliaria();
            #region CONFIG MORATORIOS DE INMOBILIARIA
            try
            {
                string sqlMora = @"SELECT P0401_ID_CONTRATO, P0402_ID_ARRENDAT, P0403_ID_ARRENDADORA, P0437_TIPO, 
       T04_CONTRATO.CAMPO1 AS TIPO_CALC_MORATORIO, T04_CONTRATO.CAMPO_NUM2 AS TASA_INTERES_MORA, T04_CONTRATO.CAMPO_NUM3 AS DIAS_DE_GRACIA, 
       T01_ARRENDADORA.CAMPO_NUM1 AS DIAS_MESES_MORA , T01_ARRENDADORA.CAMPO_NUM2 AS APLICAR_DIAS_GRACIA           
FROM T04_CONTRATO
LEFT JOIN T01_ARRENDADORA ON T04_CONTRATO.P0403_ID_ARRENDADORA= T01_ARRENDADORA.P0101_ID_ARR
WHERE P0401_ID_CONTRATO=? AND T01_ARRENDADORA.P0101_ID_ARR=?";
                OdbcCommand comando = new OdbcCommand(sqlMora, conexion);
                comando.Parameters.Add("@IDCont", OdbcType.VarChar).Value = cliente.IDContrato;
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idArr;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                reader.Close();
                conexion.Close();
                configMoratorios.idInmo = idArr;
                configMoratorios.idContrato = cliente.IDContrato;
                decimal valor = string.IsNullOrWhiteSpace(dt.Rows[0]["DIAS_MESES_MORA"].ToString()) ? 0 : (decimal)(dt.Rows[0]["DIAS_MESES_MORA"]);
                configMoratorios.diasMesesMoratorios = Convert.ToInt32(valor);
                valor = string.IsNullOrWhiteSpace(dt.Rows[0]["APLICAR_DIAS_GRACIA"].ToString()) ? 2 : (decimal)(dt.Rows[0]["APLICAR_DIAS_GRACIA"]);
                configMoratorios.aplicarDiasGracia = Convert.ToInt32(valor);
                string dias = dt.Rows[0]["DIAS_DE_GRACIA"].ToString();
                configMoratorios.diasDeGracia = string.IsNullOrWhiteSpace(dt.Rows[0]["DIAS_DE_GRACIA"].ToString()) ? 0 : Convert.ToInt32(dt.Rows[0]["DIAS_DE_GRACIA"]);
                configMoratorios.tipoCalculoMoratorio = dt.Rows[0]["TIPO_CALC_MORATORIO"].ToString();
                configMoratorios.tasaInteres = (decimal)dt.Rows[0]["TASA_INTERES_MORA"];
            }
            catch (Exception ex)
            {
                configMoratorios.idInmo = idArr;
                configMoratorios.idContrato = cliente.IDContrato;
                configMoratorios.diasMesesMoratorios = 0;
                configMoratorios.aplicarDiasGracia = 0;
                configMoratorios.tipoCalculoMoratorio = string.Empty;
                configMoratorios.tasaInteres = 0;
                configMoratorios.diasDeGracia = 0;
                conexion.Close();
            }
            #endregion

            #region COMANDO SQL
            string sql = @"SELECT P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2404_PERIODO, P2405_IMPORTE, P2426_TIPO_DOC, P2406_STATUS, 
P2409_FECHA_EMISION, P2410_MONEDA, P2411_N_ARRENDATARIO, P2412_CONCEPTO, P2418_ID_CONTRATO, P2414_TIPO_CAMBIO, P2415_NO_EDIF, P2416_IVA, 
P2419_TOTAL, P2427_CTD_PAG, P2428_T_IVA, P2429_PER_ARR, P2430_PER_CTE, P2444_ID_HIST_REC, P2403_NUM_RECIBO, CAMPO1, CAMPO2, CAMPO3, CAMPO4, 
CAMPO5, CAMPO9, CAMPO_DATE1, CAMPO_DATE2, CAMPO_NUM14, P4006_SERIE, P4007_FOLIO FROM T24_HISTORIA_RECIBOS 
LEFT JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
WHERE P2406_STATUS = '1' ";
            if (cliente.TipoFactura == "R")
            {
                sql += @"AND P2426_TIPO_DOC IN ('R','Z','X','W','T') AND T24_HISTORIA_RECIBOS.P2427_CTD_PAG > 0  
AND P2402_ID_ARRENDATARIO = ? AND (P2409_FECHA_EMISION <= ?) ";
            }
            else if (cliente.TipoFactura == "V")
            {
                sql += @"AND P2426_TIPO_DOC IN ('V') AND T24_HISTORIA_RECIBOS.P2406_STATUS='1'  
AND P2402_ID_ARRENDATARIO = ? AND (CAMPO_DATE1 <= ?) ";
            }
            if (!string.IsNullOrWhiteSpace(idArr))
            {
                sql += "AND P2401_ID_ARRENDADORA = ? ";
            }
            //sql += "AND P2410_MONEDA = ? ORDER BY P4006_SERIE, P4007_FOLIO";
            sql += "ORDER BY P4006_SERIE, P4007_FOLIO";
            #endregion

            try
            {
                string moneda = esDolar ? "D" : "P";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDContrato", OdbcType.VarChar).Value = cliente.IDCliente;
                //comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                if (!string.IsNullOrWhiteSpace(idArr))
                {
                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArr;
                }
                //comando.Parameters.Add("@Moneda", OdbcType.VarChar).Value = moneda;

                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity()
                    {
                        IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                        Numero = (int)reader["P2403_NUM_RECIBO"],
                        FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                        //FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                        TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                        Moneda = reader["P2410_MONEDA"].ToString(),
                        Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"],
                        Importe = (decimal)reader["P2405_IMPORTE"],
                        IVA = (decimal)reader["P2416_IVA"],
                        //Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2413_PAGO"],
                        Estatus = reader["P2406_STATUS"].ToString(),
                        //PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"],                        
                        CantidadPorPagar = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"],
                        //TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2421_TC_PAGO"],
                        TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : (decimal)reader["P2414_TIPO_CAMBIO"],
                        Periodo = reader["P2404_PERIODO"].ToString(),
                        Serie = reader["P4006_SERIE"].ToString(),
                        Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],
                        //MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                        Concepto = reader["P2412_CONCEPTO"].ToString(),
                        Inmueble = reader["CAMPO9"].ToString(),
                        IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                        IDContrato = reader["P2418_ID_CONTRATO"].ToString(),
                        //Agregado 05/11/2015 by Uz                        
                        FechaVencimiento = reader["CAMPO_DATE1"] != DBNull.Value ? Convert.ToDateTime(reader["CAMPO_DATE1"]) : new DateTime(1901, 1, 1),
                        FechaMoratorios = reader["CAMPO_DATE2"] != DBNull.Value ? Convert.ToDateTime(reader["CAMPO_DATE2"]) : new DateTime(1901, 1, 1)
                    };
                    if (string.IsNullOrEmpty(recibo.Serie) && recibo.Folio == null)
                    {
                        recibo.Serie = "REC";
                        recibo.Folio = recibo.Numero;
                    }
                    InmobiliariaEntity InmoEntity = getInmobiliariaByID(recibo.IDInmobiliaria);
                    recibo.NombreInmobiliaria = InmoEntity.RazonSocial;
                    //recibo.InteresesMoratorios = obtenerMoratorios(recibo, DateTime.Today, configMoratorios);
                    if (cliente.TipoFactura == "V")
                    {
                        //Para recibos de venta P2427_CTD_PAG corresponde al total del recibo y P2419_TOTAL corresponde al saldo por pagar 
                        decimal total = recibo.CantidadPorPagar;
                        recibo.CantidadPorPagar = recibo.Total;
                        recibo.Total = total;
                    }
                    if (recibo.CantidadPorPagar > 1)
                        listaRecibos.Add(recibo);
                }

                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }


        }

        public static List<ReciboEntity> getListaRecibosReporteGlobal(ContratosEntity contrato, DateTime fechaIni, DateTime fechaFin)
        {/*
          * UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, z.* 
FROM T24_HISTORIA_RECIBOS z
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = z.P2444_ID_HIST_REC
WHERE z.p2426_tipo_doc in ('P','U') AND z.P2406_STATUS = 2 
AND z.P2401_ID_ARRENDADORA = ?
AND (z.P2409_FECHA_EMISION <= ?)
AND (z.P2408_FECHA_PAGADO >= ? 
AND z.P2408_FECHA_PAGADO <= ?)
AND (z.P2402_ID_ARRENDATARIO = ?)
AND (z.P2418_ID_CONTRATO = ?)
          */
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            //Se agregó la 2da seccion del query para incluir las facturas con notas de credito parciales.
            //Modify by Uz 26/10/2015
            string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, T24_HISTORIA_RECIBOS.* FROM T24_HISTORIA_RECIBOS
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
WHERE P2406_STATUS IN ('2') AND P2401_ID_ARRENDADORA = ?  AND (T24_HISTORIA_RECIBOS.P2407_COMENTARIO != 'CFDi IMPORTADO DESDE XL' or T24_HISTORIA_RECIBOS.P2407_COMENTARIO is null )
AND p2426_tipo_doc IN ('X','W')
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2408_FECHA_PAGADO >= ?)
AND (P2402_ID_ARRENDATARIO = ?)
AND (P2418_ID_CONTRATO = ?)
UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, T24_HISTORIA_RECIBOS.* FROM T24_HISTORIA_RECIBOS
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
WHERE P2406_STATUS IN ('1') AND P2401_ID_ARRENDADORA = ?  AND (T24_HISTORIA_RECIBOS.P2407_COMENTARIO != 'CFDi IMPORTADO DESDE XL' or T24_HISTORIA_RECIBOS.P2407_COMENTARIO is null )
AND p2426_tipo_doc IN ('X','W', 'L')
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2402_ID_ARRENDATARIO = ?)
AND (P2418_ID_CONTRATO = ?)
UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, r.* FROM T24_HISTORIA_RECIBOS r 
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC= r.P2444_ID_HIST_REC
WHERE EXISTS (SELECT * FROM T24_HISTORIA_RECIBOS u WHERE r.P2444_ID_HIST_REC=u.P2425_DEB_REC AND  (r.P2407_COMENTARIO != 'CFDi IMPORTADO DESDE XL' or r.P2407_COMENTARIO is null))
AND r.P2426_TIPO_DOC IN ('X') AND r.P2406_STATUS IN ('1') AND P2401_ID_ARRENDADORA = ?
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2402_ID_ARRENDATARIO = ?)
AND (P2418_ID_CONTRATO = ?) " +
        //Aqui va el codigo del UNION                             
        @"UNION ALL
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, y.* FROM T24_HISTORIA_RECIBOS y
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = y.P2444_ID_HIST_REC
WHERE y.P2401_ID_ARRENDADORA = ?
AND
   (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('1')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('2')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2408_Fecha_pagado >= ?
   AND y.P2408_Fecha_pagado <= ?
   )
   OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('2')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )
OR
   (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('0','3')
   AND y.P2426_TIPO_DOC IN ('T')
   AND y.CAMPO_DATE1  >= ?
   AND y.CAMPO_DATE1 <= ?
   )
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('2', '5', '4')
   AND y.P2426_TIPO_DOC IN ('G')
   AND y.P2408_Fecha_pagado >= ?
   AND y.P2408_Fecha_pagado <= ?
   )  
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('1')
   AND y.P2426_TIPO_DOC IN ('G')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )       
order by 11, 1, 2, 8, 28";
            //Se agrega el ultimo para recibos tipo G para depositos en garantía by UZ 15/12/2015
            //Se quitan los recibos tipo T con Status 3 (recibos temporales de convenio de pago cancelados) by Uz 27/05/2016
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaIni2", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IDContrato;

                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@FechaFinX", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIniX", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IDContrato;

                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IDContrato;
                /*
                comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@FechaFin2", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni3", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin3", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte2", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt2", OdbcType.VarChar).Value = contrato.IDContrato;*/
                comando.Parameters.Add("@IDArr3", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte3", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt3", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni4", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin4", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte4", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt4", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni5", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin5", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte5", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt5", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni6", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin6", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte6", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt6", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni7", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin7", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte7", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt7", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni8", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin8", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte8", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt8", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni9", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin9", OdbcType.DateTime).Value = fechaFin;

                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity()
                    {
                        IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                        Numero = (int)reader["P2403_NUM_RECIBO"],
                        FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                        FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                        TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                        Moneda = reader["P2410_MONEDA"].ToString(),
                        Descuento = reader["P2424_DESCUENTO"] == DBNull.Value ? 0 : (decimal)reader["P2424_DESCUENTO"],
                        Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"],//Agregado para reporte Inmobic,Reporte Global                        
                        IVA = reader["P2416_IVA"] == DBNull.Value ? 0 : (decimal)reader["P2416_IVA"],//Agregado para reporte Inmobic,Reporte Global
                        Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"],
                        Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2413_PAGO"],
                        ISR = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : (decimal)reader["CAMPO_NUM1"],
                        IVARetenido = reader["CAMPO_NUM2"] == DBNull.Value ? 0 : (decimal)reader["CAMPO_NUM2"],
                        Estatus = reader["P2406_STATUS"].ToString(),
                        PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"],
                        TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2421_TC_PAGO"],
                        TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : (decimal)reader["P2414_TIPO_CAMBIO"],
                        Periodo = reader["P2404_PERIODO"].ToString(),
                        Serie = reader["P4006_SERIE"].ToString(),
                        Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],
                        MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                        Concepto = reader["P2412_CONCEPTO"].ToString(),
                        Comentario = reader["P2407_COMENTARIO"].ToString(),
                        Inmueble = reader["CAMPO9"].ToString(),
                        IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                        RutaPdfCFDI = reader["P4023_CAMPO1"] == DBNull.Value ? "" : reader["P4023_CAMPO1"].ToString(),
                        //Descuento = reader["P2424_DESCUENTO"] == DBNull.Value ? 0 : (decimal)reader["P2424_DESCUENTO"]
                    };

                    recibo.Importe = recibo.Importe - recibo.Descuento;
                    if (!string.IsNullOrEmpty(recibo.RutaPdfCFDI))
                    {
                        string ruta = "file:///" + recibo.RutaPdfCFDI.Replace(".xml", ".pdf");
                        ruta = ruta.Replace("\\", "\\\\");

                        recibo.RutaPdfCFDI = string.Empty;
                        recibo.RutaPdfCFDI = ruta;
                    }
                    //Condición agregada para depositos en garantia by Uz 15/12/2015
                    if (recibo.TipoDoc == "G")
                    {
                        recibo.Serie = "DEPGAR";
                        recibo.Folio = recibo.Numero;
                    }
                    //Condición agregada para cuando son recibos sin factura by Uz 15/12/2015
                    if (string.IsNullOrEmpty(recibo.Serie) && recibo.Folio == null)
                    {
                        recibo.Serie = "REC";
                        recibo.Folio = recibo.Numero;
                    }
                    if (recibo.FechaEmision >= fechaIni)
                    {
                        if (recibo.Moneda == "P")
                        {
                            if (recibo.TipoDoc == "R" || recibo.TipoDoc == "X" || recibo.TipoDoc == "Z" || recibo.TipoDoc == "D"
                                || recibo.TipoDoc == "T" || recibo.TipoDoc == "W" || recibo.TipoDoc == "G" || recibo.TipoDoc == "L")
                            {
                                if (contrato.Moneda == "P")
                                    recibo.Cargo = recibo.Total;
                                else
                                    recibo.Cargo = recibo.Total / recibo.TipoCambio;
                            }
                            else if (recibo.TipoDoc == "B")
                            {
                                if (contrato.Moneda == "P")
                                    recibo.Abono = recibo.Total;
                                else
                                    recibo.Abono = recibo.Total / recibo.TipoCambio;
                            }
                            if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                recibo.Abono = recibo.Cargo;
                        }
                        else
                        {
                            if (recibo.TipoDoc == "R" || recibo.TipoDoc == "X" || recibo.TipoDoc == "Z" || recibo.TipoDoc == "D"
                                || recibo.TipoDoc == "T" || recibo.TipoDoc == "W" || recibo.TipoDoc == "G" || recibo.TipoDoc == "L")
                            {
                                if (contrato.Moneda == "D")
                                    recibo.Cargo = recibo.Total;
                                else
                                    recibo.Cargo = recibo.Total * recibo.TipoCambio;
                            }
                            else if (recibo.TipoDoc == "B")
                            {
                                if (contrato.Moneda == "D")
                                    recibo.Abono = recibo.Total;
                                else
                                    recibo.Abono = recibo.Total * recibo.TipoCambio;
                            }
                            if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                recibo.Abono = recibo.Cargo;
                        }
                    }
                    if (recibo.Estatus == "2" || (recibo.TipoDoc == "G" && recibo.Estatus == "5"))
                    {
                        if (recibo.FechaPago <= fechaFin)
                        {
                            if (recibo.MonedaPago == "P")
                            {
                                if (contrato.Moneda == "P")
                                {
                                    recibo.Abono = recibo.Pago;
                                    //if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                    //    recibo.Abono = recibo.PagoParcial;
                                    //else 
                                    if (recibo.TipoDoc == "G" && recibo.Estatus == "5")//Condición agregada para depositos en garantia by Uz 15/12/2015
                                        recibo.Abono = recibo.Total;
                                }
                                else
                                {
                                    recibo.Abono = recibo.Pago / recibo.TipoCambioPago;
                                    if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                        recibo.Abono = recibo.PagoParcial / recibo.TipoCambioPago;
                                    else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                        recibo.Abono = recibo.Total / recibo.TipoCambioPago;
                                }
                            }
                            else
                            {
                                if (contrato.Moneda == "D")
                                {
                                    recibo.Abono = recibo.Pago;
                                    if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                        recibo.Abono = recibo.PagoParcial;
                                    else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                        recibo.Abono = recibo.Total;
                                }
                                else
                                {
                                    recibo.Abono = recibo.Pago * recibo.TipoCambioPago;
                                    if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                        recibo.Abono = recibo.PagoParcial * recibo.TipoCambioPago;
                                    else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                        recibo.Abono = recibo.Total * recibo.TipoCambioPago;
                                }
                            }
                        }
                        else
                            recibo.FechaPago = null;
                    }
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getListaRecibosFactMesAndUltimoAbono(string idArrendadora, DateTime fechaIni, DateTime fechaFin)
        {
            string sql = @"SELECT  P2418_ID_CONTRATO,SUM(P2413_PAGO)AS SumaTotalPagado, MAX(P2408_FECHA_PAGADO) AS FechaPago
FROM T24_HISTORIA_RECIBOS WHERE  P2401_ID_ARRENDADORA = ? AND P2408_FECHA_PAGADO>= ? and P2408_FECHA_PAGADO <= ? AND T24_HISTORIA_RECIBOS.P2406_STATUS = '2' 
GROUP BY P2418_ID_CONTRATO ";
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArrendadora;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaIni;

                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity()
                    {

                        IDContrato = reader["P2418_ID_CONTRATO"].ToString(),
                        Importe = reader["SumaTotalPagado"] == DBNull.Value ? 0 : (decimal)reader["SumaTotalPagado"],
                        FechaPago = reader["FechaPago"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaPago"]),
                    };



                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getListaRecibos(ClienteEntity cliente, DateTime fechaIni, DateTime fechaFin, bool esDolar)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            //Se agregó la 2da seccion del query para incluir las facturas con notas de credito parciales.
            //Modify by Uz 26/10/2015
            string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, T24_HISTORIA_RECIBOS.* FROM T24_HISTORIA_RECIBOS
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
WHERE P2406_STATUS IN ('1','2') 
AND p2426_tipo_doc IN ('X','W')
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2408_FECHA_PAGADO >= ?)
AND (P2402_ID_ARRENDATARIO = ?)

UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1,  r.* FROM T24_HISTORIA_RECIBOS r 
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC= r.P2444_ID_HIST_REC
WHERE EXISTS (SELECT * FROM T24_HISTORIA_RECIBOS u WHERE r.P2444_ID_HIST_REC=u.P2425_DEB_REC)
AND r.P2426_TIPO_DOC IN ('X') AND r.P2406_STATUS IN ('1')
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2402_ID_ARRENDATARIO = ?) 

UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO,T40_CFD.P4023_CAMPO1, z.* 
FROM T24_HISTORIA_RECIBOS z
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = z.P2444_ID_HIST_REC
WHERE z.p2426_tipo_doc in ('P','U') AND z.P2406_STATUS = '2' 
    AND (z.P2409_FECHA_EMISION <= ?)
    AND (z.P2408_FECHA_PAGADO >= ? 
AND z.P2408_FECHA_PAGADO <= ?)
AND (z.P2402_ID_ARRENDATARIO = ?)

UNION ALL
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, y.*  FROM T24_HISTORIA_RECIBOS y
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = y.P2444_ID_HIST_REC
WHERE 
   (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2406_STATUS IN ('1')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D', 'L')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2406_STATUS IN ('2')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2408_Fecha_pagado >= ?
   AND y.P2408_Fecha_pagado <= ?
   )
   OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2406_STATUS IN ('2')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )
OR
   (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2406_STATUS IN (0, 3)
   AND y.P2426_TIPO_DOC IN ('T')
   AND y.CAMPO_DATE1  >= ?
   AND y.CAMPO_DATE1 <= ?
   )
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2406_STATUS IN ('2', '5', '4')
   AND y.P2426_TIPO_DOC IN ('G')
   AND y.P2408_Fecha_pagado >= ?
   AND y.P2408_Fecha_pagado <= ?
   )
   OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2406_STATUS IN ('1')
   AND y.P2426_TIPO_DOC IN ('G')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )      
order by 11, 1, 2, 8, 28";
            //Se modifica query para que no se incluyan los tipo T Estatus 3 (Recibos de convenio de pago temporales cancelados)
            //By Uz 27/05/2016
            try
            {
                string moneda = esDolar ? "D" : "P";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaIni2", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@FechaFin8", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni8", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte8", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@FechaFin2", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni3", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin3", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte2", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@IDCte3", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@FechaIni4", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin4", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte4", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@FechaIni5", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin5", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte5", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@FechaIni6", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin6", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte6", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@FechaIni7", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin7", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte7", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@FechaIni8", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin8", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte8", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@FechaIni9", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin9", OdbcType.DateTime).Value = fechaFin;
                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity()
                    {
                        IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                        Numero = (int)reader["P2403_NUM_RECIBO"],
                        FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                        FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                        TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                        Moneda = reader["P2410_MONEDA"].ToString(),
                        Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"],
                        Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2413_PAGO"],
                        Estatus = reader["P2406_STATUS"].ToString(),
                        PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"],
                        TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2421_TC_PAGO"],
                        TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : (decimal)reader["P2414_TIPO_CAMBIO"],
                        Periodo = reader["P2404_PERIODO"].ToString(),
                        Serie = reader["P4006_SERIE"].ToString(),
                        Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],
                        MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                        Concepto = reader["P2412_CONCEPTO"].ToString(),
                        Inmueble = reader["CAMPO9"].ToString(),
                        IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                        RutaPdfCFDI = reader["P4023_CAMPO1"] == DBNull.Value ? "" : reader["P4023_CAMPO1"].ToString()
                    };
                    if (!string.IsNullOrEmpty(recibo.RutaPdfCFDI))
                    {
                        string ruta = "file:///" + recibo.RutaPdfCFDI.Replace(".xml", ".pdf");
                        ruta = ruta.Replace("\\", "\\\\");

                        recibo.RutaPdfCFDI = string.Empty;
                        recibo.RutaPdfCFDI = ruta;
                    }
                    //Condición agregada para depositos en garantia by Uz 15/12/2015
                    if (recibo.TipoDoc == "G")
                    {
                        recibo.Serie = "DEPGAR";
                        recibo.Folio = recibo.Numero;
                    }
                    //Condición agregada para cuando son recibos sin factura by Uz 15/12/2015
                    if (string.IsNullOrEmpty(recibo.Serie) && recibo.Folio == null)
                    {
                        recibo.Serie = "REC";
                        recibo.Folio = recibo.Numero;
                    }
                    if (recibo.FechaEmision >= fechaIni)
                    {
                        if (recibo.Moneda == "P")
                        {

                            if (recibo.TipoDoc == "R" || recibo.TipoDoc == "X" || recibo.TipoDoc == "Z" || recibo.TipoDoc == "D" || recibo.TipoDoc == "T" || recibo.TipoDoc == "W"
                                || recibo.TipoDoc == "G" || recibo.TipoDoc == "L")
                            {
                                if (moneda == "P")
                                    recibo.Cargo = recibo.Total;
                                else
                                    recibo.Cargo = recibo.Total / recibo.TipoCambio;
                            }
                        }
                        else
                        {
                            if (recibo.TipoDoc == "R" || recibo.TipoDoc == "X" || recibo.TipoDoc == "Z" || recibo.TipoDoc == "D" || recibo.TipoDoc == "T"
                                || recibo.TipoDoc == "W" || recibo.TipoDoc == "G" || recibo.TipoDoc == "L")
                                if (moneda == "D")
                                    recibo.Cargo = recibo.Total;
                                else
                                    recibo.Cargo = recibo.Total * recibo.TipoCambio;
                        }
                        if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                            recibo.Abono = recibo.Cargo;
                    }
                    if (recibo.FechaPago <= fechaFin)
                    {
                        if (recibo.MonedaPago == "P")
                        {
                            if (moneda == "P")
                            {
                                recibo.Abono = recibo.Pago;
                                if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                    recibo.Abono = recibo.PagoParcial;
                                else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")//Condición agregada para depositos en garantia by Uz 15/12/2015
                                    recibo.Abono = recibo.Total;
                            }
                            else
                            {
                                recibo.Abono = recibo.Pago / recibo.TipoCambioPago;
                                if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                    recibo.Abono = recibo.PagoParcial / recibo.TipoCambioPago;
                                else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                    recibo.Abono = recibo.Total / recibo.TipoCambioPago;
                            }
                        }
                        else
                        {
                            if (moneda == "D")
                            {
                                recibo.Abono = recibo.Pago;
                                if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                    recibo.Abono = recibo.PagoParcial;
                                else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                    recibo.Abono = recibo.Total;
                            }
                            else
                            {
                                recibo.Abono = recibo.Pago * recibo.TipoCambioPago;
                                if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                    recibo.Abono = recibo.PagoParcial * recibo.TipoCambioPago;
                                else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                    recibo.Abono = recibo.Total * recibo.TipoCambioPago;
                            }
                        }

                    }
                    else
                        recibo.FechaPago = null;
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getListaRecibos(ClienteEntity cliente, DateTime fechaCorte)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,
                                    T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2419_TOTAL, 
                                    T24_HISTORIA_RECIBOS.P2413_PAGO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T24_HISTORIA_RECIBOS.P2421_TC_PAGO, T24_HISTORIA_RECIBOS.P2404_PERIODO,
                                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA,
                                    T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO,T24_HISTORIA_RECIBOS.CAMPO_NUM5  
                                    FROM T24_HISTORIA_RECIBOS
                                    LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
                                    WHERE P2426_TIPO_DOC IN ('R','Z','X','W') AND P2409_FECHA_EMISION <= ? AND P2402_ID_ARRENDATARIO = ? 
                                    ORDER BY P2409_FECHA_EMISION, P4006_SERIE, P4007_FOLIO";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@FechaCorte", OdbcType.DateTime).Value = fechaCorte.Date;
                        comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = cliente.IDCliente;
                        List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReciboEntity recibo = new ReciboEntity()
                                {
                                    IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                                    Numero = (int)reader["P2403_NUM_RECIBO"],
                                    FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                                    FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                                    TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                                    Moneda = reader["P2410_MONEDA"].ToString(),
                                    Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]),
                                    Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]),
                                    Estatus = reader["P2406_STATUS"].ToString(),
                                    PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2427_CTD_PAG"]),
                                    TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2421_TC_PAGO"]),
                                    Periodo = reader["P2404_PERIODO"].ToString(),
                                    Serie = reader["P4006_SERIE"].ToString(),
                                    Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["P4007_FOLIO"]),
                                    MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                                    Concepto = reader["P2412_CONCEPTO"].ToString(),
                                    Inmueble = reader["CAMPO9"].ToString(),
                                    IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                                    TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"]),
                                    IDPago = reader["CAMPO_NUM5"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CAMPO_NUM5"])
                                };

                                if (recibo.Estatus != "3" || (recibo.Estatus == "3" && recibo.FechaPago > fechaCorte.Date))
                                {
                                    if (recibo.Moneda == "P")
                                        recibo.Cargo = recibo.Total;
                                    else
                                        recibo.Cargo = recibo.Total * recibo.TipoCambio;
                                    if (recibo.TipoDoc == "R" || recibo.TipoDoc == "Z")
                                    {
                                        if (recibo.Estatus == "2" && (recibo.FechaPago ?? DateTime.Now) <= fechaCorte.Date)
                                        {
                                            recibo.Abono = recibo.Cargo;
                                            recibo.Pagos = new List<ReciboEntity>();
                                        }
                                        else
                                        {
                                            recibo.Pagos = getNotasCredito(recibo.IDHistRec, fechaCorte);
                                            recibo.FechaPago = null;
                                        }
                                    }
                                    else
                                    {
                                        List<ReciboEntity> abonos = new List<ReciboEntity>();
                                        var pagos = getPagosParciales(recibo.IDHistRec, fechaCorte);
                                        if (pagos != null)
                                        {
                                            if (pagos.Count <= 0 && recibo.Numero != 0)
                                                pagos = getPagosParciales(recibo.Numero, fechaCorte);
                                            if (pagos != null)
                                                pagos.ForEach(p => abonos.Add(p));
                                        }
                                        var notas = getNotasCredito(recibo.IDHistRec, fechaCorte);
                                        if (notas != null)
                                        {
                                            if (notas.Count <= 0 && recibo.Numero != 0)
                                                notas = getNotasCredito(recibo.Numero, fechaCorte);
                                            if (notas != null)
                                                notas.ForEach(n => abonos.Add(n));
                                        }
                                        recibo.Pagos = abonos;
                                    }

                                    if (recibo.IDPago > 0)
                                        recibo.Referencia = SaariE.getReferenciaPago(recibo.IDPago);

                                    listaRecibos.Add(recibo);
                                }
                            }
                        }
                        return listaRecibos;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<ReciboEntity> getListaRecibosPendientesPago(ClienteEntity cliente, DateTime fechaCorte)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,
                                    T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2419_TOTAL, 
                                    T24_HISTORIA_RECIBOS.P2413_PAGO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T24_HISTORIA_RECIBOS.P2421_TC_PAGO, T24_HISTORIA_RECIBOS.P2404_PERIODO,
                                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA,
                                    T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO 
                                    FROM T24_HISTORIA_RECIBOS
                                    LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
                                    WHERE P2426_TIPO_DOC IN ('R','Z','X','W') AND P2409_FECHA_EMISION <= ? AND P2402_ID_ARRENDATARIO = ? AND P2406_STATUS = '1'
                                    ORDER BY P2409_FECHA_EMISION, P4006_SERIE, P4007_FOLIO";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@FechaCorte", OdbcType.DateTime).Value = fechaCorte.Date;
                        comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = cliente.IDCliente;
                        List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReciboEntity recibo = new ReciboEntity()
                                {
                                    IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                                    Numero = (int)reader["P2403_NUM_RECIBO"],
                                    FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                                    FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                                    TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                                    Moneda = reader["P2410_MONEDA"].ToString(),
                                    Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]),
                                    Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]),
                                    Estatus = reader["P2406_STATUS"].ToString(),
                                    PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2427_CTD_PAG"]),
                                    TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2421_TC_PAGO"]),
                                    Periodo = reader["P2404_PERIODO"].ToString(),
                                    Serie = reader["P4006_SERIE"].ToString(),
                                    Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["P4007_FOLIO"]),
                                    MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                                    Concepto = reader["P2412_CONCEPTO"].ToString(),
                                    Inmueble = reader["CAMPO9"].ToString(),
                                    IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                                    TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"])
                                };
                                if (recibo.Estatus != "3" || (recibo.Estatus == "3" && recibo.FechaPago > fechaCorte.Date))
                                {
                                    if (recibo.Moneda == "P")
                                        recibo.Cargo = recibo.Total;
                                    else
                                        recibo.Cargo = recibo.Total * recibo.TipoCambio;
                                    if (recibo.TipoDoc == "R" || recibo.TipoDoc == "Z")
                                    {
                                        if (recibo.Estatus == "2" && (recibo.FechaPago ?? DateTime.Now) <= fechaCorte.Date)
                                        {
                                            recibo.Abono = recibo.Cargo;
                                            recibo.Pagos = new List<ReciboEntity>();
                                        }
                                        else
                                        {
                                            recibo.Pagos = getNotasCredito(recibo.IDHistRec, fechaCorte);
                                            recibo.FechaPago = null;
                                        }
                                    }
                                    else
                                    {
                                        List<ReciboEntity> abonos = new List<ReciboEntity>();
                                        var pagos = getPagosParciales(recibo.IDHistRec, fechaCorte);
                                        if (pagos != null)
                                        {
                                            if (pagos.Count <= 0 && recibo.Numero != 0)
                                                pagos = getPagosParciales(recibo.Numero, fechaCorte);
                                            if (pagos != null)
                                                pagos.ForEach(p => abonos.Add(p));
                                        }
                                        var notas = getNotasCredito(recibo.IDHistRec, fechaCorte);
                                        if (notas != null)
                                        {
                                            if (notas.Count <= 0 && recibo.Numero != 0)
                                                notas = getNotasCredito(recibo.Numero, fechaCorte);
                                            if (notas != null)
                                                notas.ForEach(n => abonos.Add(n));
                                        }
                                        recibo.Pagos = abonos;
                                    }
                                    listaRecibos.Add(recibo);
                                }
                            }
                        }
                        return listaRecibos;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static List<ReciboEntity> getListaRecibosPendientesPagoContrato(ClienteEntity cliente, DateTime fechaCorte)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,
                                    T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2419_TOTAL, 
                                    T24_HISTORIA_RECIBOS.P2413_PAGO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T24_HISTORIA_RECIBOS.P2421_TC_PAGO, T24_HISTORIA_RECIBOS.P2404_PERIODO,
                                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA,
                                    T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO 
                                    FROM T24_HISTORIA_RECIBOS
                                    LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
                                    WHERE P2426_TIPO_DOC IN ('R','Z','X','W') AND P2409_FECHA_EMISION <= ? AND P2418_ID_CONTRATO = ? AND P2406_STATUS = '1'
                                    ORDER BY P2409_FECHA_EMISION, P4006_SERIE, P4007_FOLIO";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@FechaCorte", OdbcType.DateTime).Value = fechaCorte.Date;
                        comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = cliente.IDContrato;
                        List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReciboEntity recibo = new ReciboEntity()
                                {
                                    IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                                    Numero = (int)reader["P2403_NUM_RECIBO"],
                                    FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                                    FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                                    TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                                    Moneda = reader["P2410_MONEDA"].ToString(),
                                    Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]),
                                    Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]),
                                    Estatus = reader["P2406_STATUS"].ToString(),
                                    PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2427_CTD_PAG"]),
                                    TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2421_TC_PAGO"]),
                                    Periodo = reader["P2404_PERIODO"].ToString(),
                                    Serie = reader["P4006_SERIE"].ToString(),
                                    Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["P4007_FOLIO"]),
                                    MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                                    Concepto = reader["P2412_CONCEPTO"].ToString(),
                                    Inmueble = reader["CAMPO9"].ToString(),
                                    IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                                    TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"])
                                };
                                if (recibo.Estatus != "3" || (recibo.Estatus == "3" && recibo.FechaPago > fechaCorte.Date))
                                {
                                    if (recibo.Moneda == "P")
                                        recibo.Cargo = recibo.Total;
                                    else
                                        recibo.Cargo = recibo.Total * recibo.TipoCambio;
                                    if (recibo.TipoDoc == "R" || recibo.TipoDoc == "Z")
                                    {
                                        if (recibo.Estatus == "2" && (recibo.FechaPago ?? DateTime.Now) <= fechaCorte.Date)
                                        {
                                            recibo.Abono = recibo.Cargo;
                                            recibo.Pagos = new List<ReciboEntity>();
                                        }
                                        else
                                        {
                                            recibo.Pagos = getNotasCredito(recibo.IDHistRec, fechaCorte);
                                            recibo.FechaPago = null;
                                        }
                                    }
                                    else
                                    {
                                        List<ReciboEntity> abonos = new List<ReciboEntity>();
                                        var pagos = getPagosParciales(recibo.IDHistRec, fechaCorte);
                                        if (pagos != null)
                                        {
                                            if (pagos.Count <= 0 && recibo.Numero != 0)
                                                pagos = getPagosParciales(recibo.Numero, fechaCorte);
                                            if (pagos != null)
                                                pagos.ForEach(p => abonos.Add(p));
                                        }
                                        var notas = getNotasCredito(recibo.IDHistRec, fechaCorte);
                                        if (notas != null)
                                        {
                                            if (notas.Count <= 0 && recibo.Numero != 0)
                                                notas = getNotasCredito(recibo.Numero, fechaCorte);
                                            if (notas != null)
                                                notas.ForEach(n => abonos.Add(n));
                                        }
                                        recibo.Pagos = abonos;
                                    }
                                    listaRecibos.Add(recibo);
                                }
                            }
                        }
                        return listaRecibos;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static List<ReciboEntity> getNotasCreditoByIdCliente(string idCliente, DateTime fechaCorte)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    /*string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,
                                    T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2410_MONEDA, CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2419_TOTAL * P2414_TIPO_CAMBIO) ELSE P2419_TOTAL END AS P2419_TOTAL, 
                                    T24_HISTORIA_RECIBOS.P2413_PAGO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T24_HISTORIA_RECIBOS.P2421_TC_PAGO, T24_HISTORIA_RECIBOS.P2404_PERIODO,
                                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, P2414_TIPO_CAMBIO
                                    FROM T24_HISTORIA_RECIBOS
                                    LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
                                    WHERE P2426_TIPO_DOC IN ('B') AND T24_HISTORIA_RECIBOS.P2425_DEB_REC = ? AND P2409_FECHA_EMISION <= ?
                                    ORDER BY P2409_FECHA_EMISION, P4006_SERIE, P4007_FOLIO";
                    string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,
                                    T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2410_MONEDA, CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2419_TOTAL * P2414_TIPO_CAMBIO) ELSE P2419_TOTAL END AS P2419_TOTAL, 
                                    T24_HISTORIA_RECIBOS.P2413_PAGO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T24_HISTORIA_RECIBOS.P2421_TC_PAGO, T24_HISTORIA_RECIBOS.P2404_PERIODO,
                                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, P2414_TIPO_CAMBIO
                                    FROM T24_HISTORIA_RECIBOS
                                    LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
                                    WHERE P2426_TIPO_DOC IN ('B') AND P2406_STATUS <> 3 AND T24_HISTORIA_RECIBOS.P2425_DEB_REC = ? AND P2409_FECHA_EMISION <= ?
                                    ORDER BY P2409_FECHA_EMISION, P4006_SERIE, P4007_FOLIO";*/
                    string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,
                                    T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2410_MONEDA, CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2419_TOTAL * P2414_TIPO_CAMBIO) ELSE P2419_TOTAL END AS P2419_TOTAL, 
                                    T24_HISTORIA_RECIBOS.P2413_PAGO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T24_HISTORIA_RECIBOS.P2421_TC_PAGO, T24_HISTORIA_RECIBOS.P2404_PERIODO,
                                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, P2414_TIPO_CAMBIO
                                    FROM T24_HISTORIA_RECIBOS
                                    LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
                                    WHERE P2426_TIPO_DOC IN ('B') AND P2406_STATUS <> '3' AND T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO = ? AND P2409_FECHA_EMISION <= ?
                                    ORDER BY P2409_FECHA_EMISION, P4006_SERIE, P4007_FOLIO";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@idCliente", OdbcType.NVarChar).Value = idCliente;
                        comando.Parameters.Add("@FechaCorte", OdbcType.DateTime).Value = fechaCorte.Date;
                        List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReciboEntity recibo = new ReciboEntity()
                                {
                                    IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                                    Numero = (int)reader["P2403_NUM_RECIBO"],
                                    FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                                    FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                                    TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                                    Moneda = reader["P2410_MONEDA"].ToString(),
                                    Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]),
                                    Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]),
                                    Estatus = reader["P2406_STATUS"].ToString(),
                                    PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2427_CTD_PAG"]),
                                    TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2421_TC_PAGO"]),
                                    Periodo = reader["P2404_PERIODO"].ToString(),
                                    Serie = reader["P4006_SERIE"].ToString(),
                                    Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["P4007_FOLIO"]),
                                    MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                                    Concepto = reader["P2412_CONCEPTO"].ToString(),
                                    Inmueble = reader["CAMPO9"].ToString(),
                                    IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                                    TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"])
                                };
                                listaRecibos.Add(recibo);
                            }
                        }
                        return listaRecibos;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static List<ReciboEntity> getNotasCredito(int idHistRec, DateTime fechaCorte)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    /*string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,
                                    T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2410_MONEDA, CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2419_TOTAL * P2414_TIPO_CAMBIO) ELSE P2419_TOTAL END AS P2419_TOTAL, 
                                    T24_HISTORIA_RECIBOS.P2413_PAGO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T24_HISTORIA_RECIBOS.P2421_TC_PAGO, T24_HISTORIA_RECIBOS.P2404_PERIODO,
                                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, P2414_TIPO_CAMBIO
                                    FROM T24_HISTORIA_RECIBOS
                                    LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
                                    WHERE P2426_TIPO_DOC IN ('B') AND T24_HISTORIA_RECIBOS.P2425_DEB_REC = ? AND P2409_FECHA_EMISION <= ?
                                    ORDER BY P2409_FECHA_EMISION, P4006_SERIE, P4007_FOLIO";*/
                    string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,
                                    T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2410_MONEDA, CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2419_TOTAL * P2414_TIPO_CAMBIO) ELSE P2419_TOTAL END AS P2419_TOTAL, 
                                    T24_HISTORIA_RECIBOS.P2413_PAGO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T24_HISTORIA_RECIBOS.P2421_TC_PAGO, T24_HISTORIA_RECIBOS.P2404_PERIODO,
                                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, P2414_TIPO_CAMBIO
                                    FROM T24_HISTORIA_RECIBOS
                                    LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
                                    WHERE P2426_TIPO_DOC IN ('B') AND P2406_STATUS <> '3' AND T24_HISTORIA_RECIBOS.P2425_DEB_REC = ? AND P2409_FECHA_EMISION <= ?
                                    ORDER BY P2409_FECHA_EMISION, P4006_SERIE, P4007_FOLIO";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@IDHist", OdbcType.Int).Value = idHistRec;
                        comando.Parameters.Add("@FechaCorte", OdbcType.DateTime).Value = fechaCorte.Date;
                        List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReciboEntity recibo = new ReciboEntity()
                                {
                                    IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                                    Numero = (int)reader["P2403_NUM_RECIBO"],
                                    FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                                    FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                                    TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                                    Moneda = reader["P2410_MONEDA"].ToString(),
                                    Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]),
                                    Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]),
                                    Estatus = reader["P2406_STATUS"].ToString(),
                                    PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2427_CTD_PAG"]),
                                    TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2421_TC_PAGO"]),
                                    Periodo = reader["P2404_PERIODO"].ToString(),
                                    Serie = reader["P4006_SERIE"].ToString(),
                                    Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["P4007_FOLIO"]),
                                    MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                                    Concepto = reader["P2412_CONCEPTO"].ToString(),
                                    Inmueble = reader["CAMPO9"].ToString(),
                                    IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                                    TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"])
                                };
                                listaRecibos.Add(recibo);
                            }
                        }
                        return listaRecibos;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<ReciboEntity> getPagosParciales(int idHistRec, DateTime fechaCorte)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = @"SELECT P2444_ID_HIST_REC, P2403_NUM_RECIBO, P2409_FECHA_EMISION, P2408_FECHA_PAGADO, P2426_TIPO_DOC, P2410_MONEDA, CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2419_TOTAL * P2414_TIPO_CAMBIO) ELSE P2419_TOTAL END AS P2419_TOTAL, 
                                    P2413_PAGO, P2406_STATUS, P2427_CTD_PAG, P2421_TC_PAGO, P2404_PERIODO, P2420_MONEDA_PAGO, P2412_CONCEPTO, CAMPO9, P2401_ID_ARRENDADORA, P2414_TIPO_CAMBIO
                                    FROM T24_HISTORIA_RECIBOS
                                    WHERE P2426_TIPO_DOC IN ('P','U') AND P2403_NUM_RECIBO = ? AND P2408_FECHA_PAGADO <= ?
                                    ORDER BY P2409_FECHA_EMISION";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@IDHist", OdbcType.Int).Value = idHistRec;
                        comando.Parameters.Add("@FechaCorte", OdbcType.DateTime).Value = fechaCorte.Date;
                        List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReciboEntity recibo = new ReciboEntity();
                                recibo.IDHistRec = (int)reader["P2444_ID_HIST_REC"];
                                recibo.Numero = (int)reader["P2403_NUM_RECIBO"];
                                recibo.FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                                recibo.FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                                recibo.TipoDoc = reader["P2426_TIPO_DOC"].ToString();
                                recibo.Moneda = reader["P2410_MONEDA"].ToString();
                                recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]);
                                recibo.Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]);
                                recibo.Estatus = reader["P2406_STATUS"].ToString();
                                recibo.PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2427_CTD_PAG"]);
                                recibo.TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2421_TC_PAGO"]);
                                recibo.Periodo = reader["P2404_PERIODO"].ToString();
                                //Serie = reader["P4006_SERIE"].ToString(),
                                //Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],
                                recibo.MonedaPago = reader["P2420_MONEDA_PAGO"].ToString();
                                recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                                recibo.Inmueble = reader["CAMPO9"].ToString();
                                recibo.IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString();
                                recibo.TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : (decimal)reader["P2414_TIPO_CAMBIO"];
                                listaRecibos.Add(recibo);
                            }
                        }
                        return listaRecibos;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static List<SaldoEntity> getSaldos(ContratosEntity contrato, DateTime fechaIni, DateTime fechaFin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT P2403_NUM_RECIBO,P2444_ID_HIST_REC, P2419_TOTAL, P2410_MONEDA, P2426_TIPO_DOC, P2408_FECHA_PAGADO, P2420_MONEDA_PAGO, P2421_TC_PAGO, P2406_STATUS FROM T24_HISTORIA_RECIBOS
WHERE 
(P2401_ID_ARRENDADORA = ?
 AND P2402_ID_ARRENDATARIO = ? 
 AND P2418_ID_CONTRATO = ? 
 AND P2426_TIPO_DOC IN ('R','X','T','Z','W','O','L', 'G') 
 AND P2409_FECHA_EMISION < ? 
 AND (P2408_FECHA_PAGADO IS NULL OR P2408_FECHA_PAGADO >= ?) 
 AND P2406_STATUS IN ('1','2')
)
or
   (
   P2401_ID_ARRENDADORA = ?
   AND P2402_ID_ARRENDATARIO = ?
   AND P2418_ID_CONTRATO = ?
   AND P2426_TIPO_DOC IN ('R','X','T','Z','W','O','L','B','G') AND
   P2406_STATUS IN ('1')  AND P2409_FECHA_EMISION < ? 
   )
or
   (
   P2401_ID_ARRENDADORA = ?
   AND P2402_ID_ARRENDATARIO = ?
   AND P2418_ID_CONTRATO = ?
   AND P2426_TIPO_DOC IN ('T','L') AND
   P2406_STATUS IN ('0')  AND CAMPO_DATE1 < ?
   )
or
   (
 P2401_ID_ARRENDADORA = ?
 AND P2402_ID_ARRENDATARIO =? 
 AND P2418_ID_CONTRATO =? 
 AND P2426_TIPO_DOC IN ('B') 
 AND P2409_FECHA_EMISION <= ? 
 AND (P2408_FECHA_PAGADO IS NULL OR P2408_FECHA_PAGADO <=?) 
 AND P2406_STATUS IN ('2')
)
ORDER BY T24_HISTORIA_RECIBOS.CAMPO5,
T24_HISTORIA_RECIBOS.P2411_N_ARRENDATARIO,
T24_HISTORIA_RECIBOS.CAMPO9,
T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaIni;//OJO
                comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte2", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt2", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni2", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDArr3", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte3", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt3", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni3", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDArr4", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte4", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt4", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni4", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaIni4", OdbcType.DateTime).Value = fechaIni;
                List<SaldoEntity> listaSaldos = new List<SaldoEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    SaldoEntity saldo = new SaldoEntity();
                    decimal idHistRec = reader["P2444_ID_HIST_REC"] == DBNull.Value ? 0 : (int)reader["P2444_ID_HIST_REC"];
                    saldo.NumeroRecibo = reader["P2403_NUM_RECIBO"] == DBNull.Value ? 0 : (int)reader["P2403_NUM_RECIBO"];
                    saldo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    saldo.Moneda = reader["P2410_MONEDA"].ToString();
                    saldo.TipoDocumento = reader["P2426_TIPO_DOC"].ToString();
                    saldo.FechaPagado = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    saldo.MonedaPago = reader["P2420_MONEDA_PAGO"].ToString();
                    saldo.TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2421_TC_PAGO"];
                    saldo.Estatus = reader["P2406_STATUS"].ToString();
                    if (contrato.Moneda != saldo.MonedaPago)
                    {
                        if (saldo.Estatus == "2")
                        {
                            if (saldo.TipoCambioPago > 0)
                            {
                                if (contrato.Moneda == "P" && saldo.MonedaPago == "D")
                                    saldo.Total = (saldo.Total * saldo.TipoCambioPago);
                                else if (contrato.Moneda == "D" && saldo.MonedaPago == "P")
                                    saldo.Total = (saldo.Total / saldo.TipoCambioPago);
                            }
                        }
                    }
                    if (saldo.TipoDocumento == "B")
                        saldo.PagoParcial = 0 - saldo.Total;
                    else if (saldo.TipoDocumento == "X" || saldo.TipoDocumento == "W")
                        saldo.PagoParcial = saldo.Total - (getPagoParcial(contrato, fechaIni, saldo.NumeroRecibo));
                    else if (saldo.Estatus == "1")
                        saldo.PagoParcial = saldo.Total;
                    else if (saldo.FechaPagado >= fechaIni || saldo.FechaPagado == new DateTime(1901, 1, 1))
                        saldo.PagoParcial = saldo.Total;
                    listaSaldos.Add(saldo);
                }
                reader.Close();
                conexion.Close();
                return listaSaldos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<SaldoEntity> getSaldos2(ContratosEntity contrato, DateTime fechaIni, DateTime fechaFin)
        {
            //AND  (P2407_COMENTARIO!='CFDi IMPORTADO DESDE XL' OR P2407_COMENTARIO IS NULL) Agregar en donde se utilicen recibos R
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT P2403_NUM_RECIBO,P2444_ID_HIST_REC, P2419_TOTAL, P2410_MONEDA, P2426_TIPO_DOC, P2408_FECHA_PAGADO, P2420_MONEDA_PAGO, P2421_TC_PAGO, P2406_STATUS FROM T24_HISTORIA_RECIBOS
WHERE 
(P2401_ID_ARRENDADORA = ?
 AND P2402_ID_ARRENDATARIO = ? 
 AND P2418_ID_CONTRATO = ? 
 AND P2426_TIPO_DOC IN ('R','X','T','Z','W','O','L', 'G') 
 AND P2409_FECHA_EMISION < ? 
 AND (P2408_FECHA_PAGADO IS NULL OR P2408_FECHA_PAGADO >= ?) 
 AND P2406_STATUS IN ('1','2')

)
or
   (
   P2401_ID_ARRENDADORA = ?
   AND P2402_ID_ARRENDATARIO = ?
   AND P2418_ID_CONTRATO = ?
   AND P2426_TIPO_DOC IN ('R','X','T','Z','W','O','L','B','G') AND
   P2406_STATUS IN ('1')  AND P2409_FECHA_EMISION < ? 

   )
or
   (
   P2401_ID_ARRENDADORA = ?
   AND P2402_ID_ARRENDATARIO = ?
   AND P2418_ID_CONTRATO = ?
   AND P2426_TIPO_DOC IN ('T','L') AND
   P2406_STATUS IN ('0')  AND CAMPO_DATE1 < ?
   )
or
   (
 P2401_ID_ARRENDADORA = ?
 AND P2402_ID_ARRENDATARIO =? 
 AND P2418_ID_CONTRATO =? 
 AND P2426_TIPO_DOC IN ('B') 
 AND P2409_FECHA_EMISION <= ? 
 AND (P2408_FECHA_PAGADO IS NULL OR P2408_FECHA_PAGADO <=?) 
 AND P2406_STATUS IN ('2')
)
or
   (
 P2401_ID_ARRENDADORA = ?
 AND P2402_ID_ARRENDATARIO =? 
 AND P2418_ID_CONTRATO =? 
 AND P2426_TIPO_DOC IN ('P') 
 AND P2409_FECHA_EMISION <= ? 
 AND (P2408_FECHA_PAGADO IS NULL OR P2408_FECHA_PAGADO <=?) 
 AND P2406_STATUS IN ('2')
)
ORDER BY T24_HISTORIA_RECIBOS.CAMPO5,
T24_HISTORIA_RECIBOS.P2411_N_ARRENDATARIO,
T24_HISTORIA_RECIBOS.CAMPO9,
T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO";
            //Se agregi ka OArte del OR doc Tipo P status 2.
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaIni;//OJO
                comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte2", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt2", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni2", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDArr3", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte3", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt3", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni3", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDArr4", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte4", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt4", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni4", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaIni4", OdbcType.DateTime).Value = fechaIni;

                comando.Parameters.Add("@IDArr5", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte5", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt5", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni5", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaIni5", OdbcType.DateTime).Value = fechaIni;


                List<SaldoEntity> listaSaldos = new List<SaldoEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    SaldoEntity saldo = new SaldoEntity();
                    int idHistRec = reader["P2444_ID_HIST_REC"] == DBNull.Value ? 0 : (int)reader["P2444_ID_HIST_REC"];
                    saldo.NumeroRecibo = reader["P2403_NUM_RECIBO"] == DBNull.Value ? 0 : (int)reader["P2403_NUM_RECIBO"];
                    saldo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    saldo.Moneda = reader["P2410_MONEDA"].ToString();
                    saldo.TipoDocumento = reader["P2426_TIPO_DOC"].ToString();
                    saldo.FechaPagado = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    saldo.MonedaPago = reader["P2420_MONEDA_PAGO"].ToString();
                    saldo.TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2421_TC_PAGO"];
                    saldo.Estatus = reader["P2406_STATUS"].ToString();
                    if (contrato.Moneda != saldo.MonedaPago)
                    {
                        if (saldo.Estatus == "2")
                        {
                            if (saldo.TipoCambioPago > 0)
                            {
                                if (contrato.Moneda == "P" && saldo.MonedaPago == "D")
                                    saldo.Total = (saldo.Total * saldo.TipoCambioPago);
                                else if (contrato.Moneda == "D" && saldo.MonedaPago == "P")
                                    saldo.Total = (saldo.Total / saldo.TipoCambioPago);
                            }
                        }
                    }
                    if (saldo.TipoDocumento == "B")
                        saldo.PagoParcial = 0 - saldo.Total;
                    else if (saldo.TipoDocumento == "X" || saldo.TipoDocumento == "W")
                        saldo.PagoParcial = saldo.Total - (getPagoParcial(contrato, fechaIni, idHistRec));
                    else if (saldo.Estatus == "1")
                        saldo.PagoParcial = saldo.Total;
                    else if (saldo.FechaPagado >= fechaIni || saldo.FechaPagado == new DateTime(1901, 1, 1))
                        saldo.PagoParcial = saldo.Total;
                    listaSaldos.Add(saldo);
                }
                reader.Close();
                conexion.Close();
                return listaSaldos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<SaldoEntity> getSaldos(ClienteEntity cliente, DateTime fechaIni, DateTime fechaFin, bool esDolar)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT P2403_NUM_RECIBO, P2419_TOTAL, P2410_MONEDA, P2426_TIPO_DOC, P2408_FECHA_PAGADO, P2420_MONEDA_PAGO, P2421_TC_PAGO, P2401_ID_ARRENDADORA FROM T24_HISTORIA_RECIBOS
WHERE  
(P2402_ID_ARRENDATARIO = ? 
 AND P2426_TIPO_DOC IN ('R','X','T','Z','W','O','L','G') 
 AND P2409_FECHA_EMISION < ? 
 AND (P2408_FECHA_PAGADO IS NULL OR P2408_FECHA_PAGADO >= ?) 
 AND P2406_STATUS IN ('1','2','5')
)
or
   (
   P2402_ID_ARRENDATARIO = ?
   AND P2426_TIPO_DOC IN ('R','X','T','Z','W','O','L','B','G') AND
   P2406_STATUS IN ('1')  AND P2409_FECHA_EMISION < ? 
   )
or
   (
   P2402_ID_ARRENDATARIO = ?
   AND P2426_TIPO_DOC IN ('T','L') AND
   P2406_STATUS IN ('0')  AND CAMPO_DATE1 < ?
   )
ORDER BY T24_HISTORIA_RECIBOS.CAMPO5,
T24_HISTORIA_RECIBOS.P2411_N_ARRENDATARIO,
T24_HISTORIA_RECIBOS.CAMPO9,
T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaIni;//OJO
                comando.Parameters.Add("@IDCte2", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@FechaIni2", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte3", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@FechaIni3", OdbcType.DateTime).Value = fechaIni;
                List<SaldoEntity> listaSaldos = new List<SaldoEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    SaldoEntity saldo = new SaldoEntity();
                    saldo.NumeroRecibo = reader["P2403_NUM_RECIBO"] == DBNull.Value ? 0 : (int)reader["P2403_NUM_RECIBO"];
                    saldo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    saldo.Moneda = reader["P2410_MONEDA"].ToString();
                    saldo.TipoDocumento = reader["P2426_TIPO_DOC"].ToString();
                    saldo.FechaPagado = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    saldo.MonedaPago = reader["P2420_MONEDA_PAGO"].ToString();
                    saldo.TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2421_TC_PAGO"];
                    saldo.IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString();
                    string moneda = esDolar ? "D" : "P";
                    if (moneda != saldo.MonedaPago)
                    {
                        if (saldo.TipoCambioPago > 0)
                        {
                            if (moneda == "P" && saldo.MonedaPago == "D")
                                saldo.Total = (saldo.Total * saldo.TipoCambioPago);
                            else if (moneda == "D" && saldo.MonedaPago == "P")
                                saldo.Total = (saldo.Total / saldo.TipoCambioPago);
                        }
                    }
                    if (saldo.TipoDocumento == "B")
                        saldo.PagoParcial = 0 - saldo.Total;
                    else if (saldo.TipoDocumento == "X" || saldo.TipoDocumento == "W")
                        saldo.PagoParcial = saldo.Total - (getPagoParcial(cliente, fechaIni, saldo.NumeroRecibo, moneda));
                    else if (saldo.FechaPagado >= fechaIni || saldo.FechaPagado == new DateTime(1901, 1, 1))
                        saldo.PagoParcial = saldo.Total;
                    listaSaldos.Add(saldo);
                }
                reader.Close();
                conexion.Close();
                return listaSaldos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static decimal getPagoParcial(string idArrendadora, DateTime fechaFin, string idCliente, int numRecibo, string monedaContrato)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                if (monedaContrato == "P")
                {
                    sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2413_PAGO * P2421_TC_PAGO) ELSE P2413_PAGO END) FROM T24_HISTORIA_RECIBOS 
WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION < ?
AND P2408_FECHA_PAGADO < ? AND P2402_ID_ARRENDATARIO = ?
AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND P2406_STATUS IN ('1','2'))
      OR (P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC = 'P' AND P2406_STATUS IN ('2')))";
                }
                else
                {
                    sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2413_PAGO / P2421_TC_PAGO) ELSE P2413_PAGO END) FROM T24_HISTORIA_RECIBOS 
WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION < ?
AND P2408_FECHA_PAGADO < ? AND P2402_ID_ARRENDATARIO = ?
AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND P2406_STATUS IN ('1','2'))
      OR (P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC = 'P' AND P2406_STATUS IN ('2')))";
                }
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@Arr", OdbcType.VarChar).Value = idArrendadora;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaFin2", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@Cte", OdbcType.VarChar).Value = idCliente;
                comando.Parameters.Add("@Rec1", OdbcType.Int).Value = numRecibo;
                comando.Parameters.Add("@Rec2", OdbcType.Int).Value = numRecibo;
                conexion.Open();
                decimal pagoParcial = (decimal)comando.ExecuteScalar();
                conexion.Close();
                return pagoParcial;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getPagoParcial(ContratosEntity contrato, DateTime fechaFin, int numRecibo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                if (contrato.Moneda == "P")
                {
                    sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2413_PAGO * P2421_TC_PAGO) ELSE P2413_PAGO END) FROM T24_HISTORIA_RECIBOS 
WHERE P2401_ID_ARRENDADORA = ? AND P2402_ID_ARRENDATARIO = ? AND P2418_ID_CONTRATO = ? AND P2409_FECHA_EMISION < ?
AND P2408_FECHA_PAGADO < ? 
AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND P2406_STATUS IN ('1','2'))
      OR (P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U') AND P2406_STATUS IN ('2')))";
                }
                else
                {
                    sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2413_PAGO / P2421_TC_PAGO) ELSE P2413_PAGO END) FROM T24_HISTORIA_RECIBOS 
WHERE P2401_ID_ARRENDADORA = ? AND P2402_ID_ARRENDATARIO = ? AND P2418_ID_CONTRATO = ? AND P2409_FECHA_EMISION < ?
AND P2408_FECHA_PAGADO < ? 
AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND P2406_STATUS IN ('1','2'))
      OR (P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U') AND P2406_STATUS IN ('2')))";
                }
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@Arr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@Cte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@Cnt", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaFin2", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@Rec1", OdbcType.Int).Value = numRecibo;
                comando.Parameters.Add("@Rec2", OdbcType.Int).Value = numRecibo;
                conexion.Open();
                decimal pagoParcial = (decimal)comando.ExecuteScalar();
                conexion.Close();
                return pagoParcial;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getPagoParcial(ClienteEntity cliente, DateTime fechaFin, int numRecibo, string moneda)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                if (moneda == "P")
                {
                    sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2413_PAGO * P2421_TC_PAGO) ELSE P2413_PAGO END) FROM T24_HISTORIA_RECIBOS 
WHERE P2402_ID_ARRENDATARIO = ? AND P2409_FECHA_EMISION < ?
AND P2408_FECHA_PAGADO < ? 
AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND P2406_STATUS IN ('1','2'))
      OR (P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U') AND P2406_STATUS IN ('2')))";
                }
                else
                {
                    sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2413_PAGO / P2421_TC_PAGO) ELSE P2413_PAGO END) FROM T24_HISTORIA_RECIBOS 
WHERE P2402_ID_ARRENDATARIO = ? AND P2409_FECHA_EMISION < ?
AND P2408_FECHA_PAGADO < ? 
AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND P2406_STATUS IN ('1','2'))
      OR (P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U') AND P2406_STATUS IN ('2')))";
                }
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@Cte", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaFin2", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@Rec1", OdbcType.Int).Value = numRecibo;
                comando.Parameters.Add("@Rec2", OdbcType.Int).Value = numRecibo;
                conexion.Open();
                decimal pagoParcial = (decimal)comando.ExecuteScalar();
                conexion.Close();
                return pagoParcial;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getSaldoAnterior(ContratosEntity contrato, DateTime fechaIni)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                if (contrato.Moneda == "P")
                {
                    sql = @"SELECT CASE WHEN SUM(P2413_PAGO) <> 0 THEN SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2413_PAGO * P2421_TC_PAGO) ELSE P2413_PAGO END) ELSE 0 END FROM T24_HISTORIA_RECIBOS 
WHERE P2401_ID_ARRENDADORA = ? AND P2402_ID_ARRENDATARIO = ? AND P2418_ID_CONTRATO = ? AND P2409_FECHA_EMISION >= ?
AND P2408_FECHA_PAGADO < ? ";
                }
                else
                {
                    sql = @"SELECT CASE WHEN SUM(P2413_PAGO) <> 0 THEN SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2413_PAGO / P2421_TC_PAGO) ELSE P2413_PAGO END) ELSE 0 END FROM T24_HISTORIA_RECIBOS 
WHERE P2401_ID_ARRENDADORA = ? AND P2402_ID_ARRENDATARIO = ? AND P2418_ID_CONTRATO = ? AND P2409_FECHA_EMISION >= ?
AND P2408_FECHA_PAGADO < ? ";
                }
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@Arr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@Cte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@Cnt", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaIni2", OdbcType.DateTime).Value = fechaIni;
                conexion.Open();
                decimal pagoParcial = (decimal)comando.ExecuteScalar();
                conexion.Close();
                return pagoParcial;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getSaldoAnterior(ClienteEntity cliente, DateTime fechaIni, bool esDolar, string idInmobiliaria)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                string moneda = esDolar ? "D" : "P";
                if (string.IsNullOrEmpty(idInmobiliaria))
                {
                    if (moneda == "P")
                    {
                        sql = @"SELECT CASE WHEN SUM(P2413_PAGO) <> 0 THEN SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2413_PAGO * P2421_TC_PAGO) ELSE P2413_PAGO END) ELSE 0 END FROM T24_HISTORIA_RECIBOS 
WHERE P2402_ID_ARRENDATARIO = ? AND P2409_FECHA_EMISION >= ?
AND P2408_FECHA_PAGADO < ? ";
                    }
                    else
                    {
                        sql = @"SELECT CASE WHEN SUM(P2413_PAGO) <> 0 THEN SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2413_PAGO / P2421_TC_PAGO) ELSE P2413_PAGO END) ELSE 0 END FROM T24_HISTORIA_RECIBOS 
WHERE P2402_ID_ARRENDATARIO = ? AND P2409_FECHA_EMISION >= ?
AND P2408_FECHA_PAGADO < ? ";
                    }
                }
                else
                {
                    if (moneda == "P")
                    {
                        sql = @"SELECT CASE WHEN SUM(P2413_PAGO) <> 0 THEN SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2413_PAGO * P2421_TC_PAGO) ELSE P2413_PAGO END) ELSE 0 END FROM T24_HISTORIA_RECIBOS 
WHERE P2402_ID_ARRENDATARIO = ? AND P2409_FECHA_EMISION >= ?
AND P2408_FECHA_PAGADO < ? AND P2401_ID_ARRENDADORA = ?";
                    }
                    else
                    {
                        sql = @"SELECT CASE WHEN SUM(P2413_PAGO) <> 0 THEN SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2413_PAGO / P2421_TC_PAGO) ELSE P2413_PAGO END) ELSE 0 END FROM T24_HISTORIA_RECIBOS 
WHERE P2402_ID_ARRENDATARIO = ? AND P2409_FECHA_EMISION >= ?
AND P2408_FECHA_PAGADO < ? AND P2401_ID_ARRENDADORA = ?";
                    }
                }
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@Cte", OdbcType.VarChar).Value = cliente.IDCliente;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaIni2", OdbcType.DateTime).Value = fechaIni;
                if (!string.IsNullOrEmpty(idInmobiliaria))
                    comando.Parameters.Add("@Arr", OdbcType.VarChar).Value = idInmobiliaria;
                conexion.Open();
                decimal pagoParcial = (decimal)comando.ExecuteScalar();
                conexion.Close();
                return pagoParcial;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        public static SmtpClient getSmtpCliente(string usuario)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = "SELECT CAMPO1, CAMPO2, CAMPO3, CAMPO4, CAMPO_NUM1 FROM GRUPOS WHERE USUARIO = ?";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@User", OdbcType.VarChar).Value = usuario;
                SmtpClient cliente = new SmtpClient();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    cliente.Host = reader["CAMPO4"].ToString();
                    cliente.Port = Convert.ToInt32(reader["CAMPO_NUM1"]);
                    cliente.Credentials = new NetworkCredential(reader["CAMPO2"].ToString(), reader["CAMPO3"].ToString());
                    cliente.DeliveryMethod = SmtpDeliveryMethod.Network;
                    if (cliente.Host.ToLower().Contains("gmail") || cliente.Host.ToLower().Contains("live"))
                        cliente.EnableSsl = true;
                    break;
                }
                reader.Close();
                conexion.Close();
                return cliente;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static string getMailCliente(string idCliente)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = "SELECT P0604_DIRECCION FROM T06_MAIL_WEB WHERE P0601_ID_ENTE = ? AND P0603_TIPO_SERV = 'E' AND P0605_ORDEN = 1";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("IDCte", OdbcType.VarChar).Value = idCliente;
                conexion.Open();
                string result = comando.ExecuteScalar().ToString();
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }

        public static List<ClienteEntity> getClientes()
        {
            List<PersonaFisicaEntity> Personas = null;
            try
            {
                Personas = getContactosCliente();
            }
            catch(Exception ex)
            {
                Personas = null;
            }

            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                //                string sql = @"SELECT P0201_ID, P0203_NOMBRE,P0253_NOMBRE_COMERCIAL, P0204_RFC, P0202_TIPO_ENTE,CAMPO2 FROM T02_ARRENDATARIO
                //WHERE P0202_TIPO_ENTE=2 ORDER BY P0203_NOMBRE";
                //string sql = @"SELECT P0201_ID, P0203_NOMBRE,P0253_NOMBRE_COMERCIAL, P0204_RFC, P0202_TIPO_ENTE,CAMPO2 , P2404_PERIODO, P2412_CONCEPTO 
                //            FROM T02_ARRENDATARIO 
                //            JOIN T24_HISTORIA_RECIBOS ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
                //            WHERE P0202_TIPO_ENTE=2 ORDER BY P0203_NOMBRE ";
                //string sql = @"SELECT T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL, 
                //T02_ARRENDATARIO.P0204_RFC, T02_ARRENDATARIO.P0202_TIPO_ENTE, T02_ARRENDATARIO.CAMPO2, T15_PERSONA.P1501_ID_PERSONA, 
                //T15_PERSONA.P1503_NOMBRE AS NOMBRECONTACTO
                //FROM T02_ARRENDATARIO
                //JOIN T15_PERSONA ON T15_PERSONA.P1504_ID_ENTE_EXT = T02_ARRENDATARIO.P0201_ID 
                //WHERE T02_ARRENDATARIO.P0202_TIPO_ENTE=2 AND T15_PERSONA.P1502_TIPO_ENTE = 13 
                //ORDER BY P0203_NOMBRE";
                string sql = @"SELECT T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL, 
                T02_ARRENDATARIO.P0204_RFC, T02_ARRENDATARIO.P0202_TIPO_ENTE, T02_ARRENDATARIO.CAMPO2
                FROM T02_ARRENDATARIO
                WHERE T02_ARRENDATARIO.P0202_TIPO_ENTE=2
                ORDER BY P0203_NOMBRE";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                List<ClienteEntity> listaClientes = new List<ClienteEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    PersonaFisicaEntity persona = new PersonaFisicaEntity();
                    ClienteEntity cliente = new ClienteEntity()
                    {
                        IDCliente = reader["P0201_ID"].ToString(),
                        Nombre = reader["P0203_NOMBRE"].ToString(),
                        NombreComercial = reader["P0253_NOMBRE_COMERCIAL"].ToString(),
                        RFC = reader["P0204_RFC"].ToString(),
                        //TipoEnte = reader["P0202_TIPO_ENTE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P0202_TIPO_ENTE"]),
                        TipoEnte = Convert.ToInt32(reader["P0202_TIPO_ENTE"]),                        
                        IdentificadorCliente = reader["CAMPO2"].ToString(),
                    };
                    
                    if (Personas != null)
                    {
                        persona = Personas.Where(p => p.IdExt == cliente.IDCliente).FirstOrDefault();
                    }
                    cliente.Contacto = persona;                    

                    listaClientes.Add(cliente);
                }
                reader.Close();
                conexion.Close();
                return listaClientes;
            }
            catch(Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        public static List<ClienteEntity> GetClientesReportComprobantePago(DateTime fechaIni, DateTime fechaFin)
        {
            List<PersonaFisicaEntity> Personas = null;
            try
            {
                Personas = getContactosCliente();
            }
            catch (Exception ex)
            {
                Personas = null;
            }

            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL,T24_HISTORIA_RECIBOS.P2412_CONCEPTO, 
                T02_ARRENDATARIO.P0204_RFC, T02_ARRENDATARIO.P0202_TIPO_ENTE, T02_ARRENDATARIO.CAMPO2, T15_PERSONA.P1501_ID_PERSONA, P2404_PERIODO,  
                T15_PERSONA.P1503_NOMBRE AS NOMBRECONTACTO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC 
                FROM T02_ARRENDATARIO 
                JOIN T15_PERSONA ON T15_PERSONA.P1504_ID_ENTE_EXT = T02_ARRENDATARIO.P0201_ID  
                JOIN T24_HISTORIA_RECIBOS ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
                WHERE T02_ARRENDATARIO.P0202_TIPO_ENTE=2 AND T15_PERSONA.P1502_TIPO_ENTE = 13  
                AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= '" + fechaIni.ToString("MM/dd/yyyy") + "' AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= '" + fechaFin.ToString("MM/dd/yyyy") +
                "' ORDER BY P0203_NOMBRE";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                List<ClienteEntity> listaClientes = new List<ClienteEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    PersonaFisicaEntity persona = new PersonaFisicaEntity();
                    ClienteEntity cliente = new ClienteEntity()
                    {
                        IDCliente = reader["P0201_ID"].ToString(),
                        Nombre = reader["P0203_NOMBRE"].ToString(),
                        NombreComercial = reader["P0253_NOMBRE_COMERCIAL"].ToString(),
                        RFC = reader["P0204_RFC"].ToString(),
                        TipoEnte = Convert.ToInt32(reader["P0202_TIPO_ENTE"]),
                        IdentificadorCliente = reader["CAMPO2"].ToString(),
                        PeriodoFacturacion =  reader["P2404_PERIODO"].ToString(),
                        Concepto = reader["P2412_CONCEPTO"].ToString(),
                        IDHistRec = reader["P2444_ID_HIST_REC"].ToString()
                    };

                    if (Personas != null)
                    {
                        persona = Personas.Where(p => p.IdExt == cliente.IDCliente).FirstOrDefault();
                    }
                    cliente.Contacto = persona;

                    listaClientes.Add(cliente);
                }
                reader.Close();
                conexion.Close();
                return listaClientes;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }


        public static List<ClienteEntity> GetClientesReportComprobantePago()
        {
            List<PersonaFisicaEntity> Personas = null;
            try
            {
                Personas = getContactosCliente();
            }
            catch (Exception ex)
            {
                Personas = null;
            }

            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL,T24_HISTORIA_RECIBOS.P2412_CONCEPTO, 
                T02_ARRENDATARIO.P0204_RFC, T02_ARRENDATARIO.P0202_TIPO_ENTE, T02_ARRENDATARIO.CAMPO2, T15_PERSONA.P1501_ID_PERSONA, 
                T15_PERSONA.P1503_NOMBRE AS NOMBRECONTACTO  
                FROM T02_ARRENDATARIO 
                JOIN T15_PERSONA ON T15_PERSONA.P1504_ID_ENTE_EXT = T02_ARRENDATARIO.P0201_ID  
                JOIN T24_HISTORIA_RECIBOS ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
                WHERE T02_ARRENDATARIO.P0202_TIPO_ENTE=2 AND T15_PERSONA.P1502_TIPO_ENTE = 13  
                ORDER BY P0203_NOMBRE";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                List<ClienteEntity> listaClientes = new List<ClienteEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    PersonaFisicaEntity persona = new PersonaFisicaEntity();
                    ClienteEntity cliente = new ClienteEntity()
                    {
                        IDCliente = reader["P0201_ID"].ToString(),
                        Nombre = reader["P0203_NOMBRE"].ToString(),
                        NombreComercial = reader["P0253_NOMBRE_COMERCIAL"].ToString(),
                        RFC = reader["P0204_RFC"].ToString(),
                        TipoEnte = Convert.ToInt32(reader["P0202_TIPO_ENTE"]),
                        IdentificadorCliente = reader["CAMPO2"].ToString(),
                        PeriodoFacturacion = reader["P2404_PERIODO"].ToString(),
                        Concepto = reader["P2412_CONCEPTO"].ToString()
                    };

                    if (Personas != null)
                    {
                        persona = Personas.Where(p => p.IdExt == cliente.IDCliente).FirstOrDefault();
                    }
                    cliente.Contacto = persona;

                    listaClientes.Add(cliente);
                }
                reader.Close();
                conexion.Close();
                return listaClientes;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }

        /// <summary>
        ///     Retorna el nombr ey numero de telefono del contacto principal.
        /// </summary>
        /// <param name="idCliente"> Id del cliente del que se desea obtener el contacto principal</param>
        /// <returns></returns>

        public static List<ClienteEntity> getClientesProspectos()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P0201_ID, P0203_NOMBRE, P0204_RFC, P0202_TIPO_ENTE FROM T02_ARRENDATARIO 
                                WHERE (P0202_TIPO_ENTE=2 OR P0202_TIPO_ENTE=22) ORDER BY P0203_NOMBRE";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                List<ClienteEntity> listaClientes = new List<ClienteEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ClienteEntity cliente = new ClienteEntity()
                    {
                        IDCliente = reader["P0201_ID"].ToString(),
                        Nombre = reader["P0203_NOMBRE"].ToString(),
                        RFC = reader["P0204_RFC"].ToString(),
                        TipoEnte = reader["P0202_TIPO_ENTE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P0202_TIPO_ENTE"])
                    };
                    listaClientes.Add(cliente);
                }
                reader.Close();
                conexion.Close();
                return listaClientes;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        public static List<ClienteEntity> getClientesPorInmobiliaria(string idInmobiliaria)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            bool asignarParametro = false;
            try
            {
                string sql = @"SELECT T2.P0201_ID, T2.P0203_NOMBRE, T2.P0204_RFC, T04_CONTRATO.P0401_ID_CONTRATO,
T04_CONTRATO.P0407_MONEDA_FACT,T04_CONTRATO.P0437_TIPO, T04_CONTRATO.P0403_ID_ARRENDADORA,
T04_CONTRATO.P0437_TIPO
FROM T02_ARRENDATARIO T2
LEFT JOIN T04_CONTRATO ON T04_CONTRATO.P0402_ID_ARRENDAT = T2.P0201_ID ";

                if (idInmobiliaria != "TODOS")
                {
                    if (idInmobiliaria != "SIN CONTRATO")
                    {
                        sql += "WHERE P0403_ID_ARRENDADORA = ? ";
                        asignarParametro = true;
                    }
                    else
                    {
                        sql += "WHERE P0403_ID_ARRENDADORA is null ";
                    }
                }
                else
                {
                    sql = string.Empty;
                    sql = "SELECT T2.P0201_ID, T2.P0203_NOMBRE, T2.P0204_RFC FROM T02_ARRENDATARIO T2 ";
                    asignarParametro = false;
                }
                sql += "ORDER BY P0203_NOMBRE";


                OdbcCommand comando = new OdbcCommand(sql, conexion);
                if (asignarParametro)
                    comando.Parameters.Add("@idArr", OdbcType.VarChar).Value = idInmobiliaria;
                List<ClienteEntity> listaClientes = new List<ClienteEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ClienteEntity cliente = new ClienteEntity()
                    {
                        IDCliente = reader["P0201_ID"].ToString(),
                        Nombre = reader["P0203_NOMBRE"].ToString(),
                        RFC = reader["P0204_RFC"].ToString()
                    };
                    listaClientes.Add(cliente);
                }
                reader.Close();
                conexion.Close();
                return listaClientes;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        public static List<ClienteEntity> getAllClientesPorInmobiliaria(string idInmobiliaria)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            bool asignarParametro = false;
            try
            {
                string sql = @"SELECT P2402_ID_ARRENDATARIO, COUNT(*)  
FROM T24_HISTORIA_RECIBOS ";

                if (idInmobiliaria != "TODOS")
                {
                    if (idInmobiliaria != "SIN CONTRATO")
                    {
                        sql += "WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA=? ";
                        asignarParametro = true;
                    }
                    else
                    {
                        sql += "WHERE P0403_ID_ARRENDADORA is null ";
                    }
                }
                sql += @"GROUP BY P2402_ID_ARRENDATARIO HAVING COUNT(*)>1 ORDER BY P2402_ID_ARRENDATARIO";

                OdbcCommand comando = new OdbcCommand(sql, conexion);
                if (asignarParametro)
                    comando.Parameters.Add("@idArr", OdbcType.VarChar).Value = idInmobiliaria;
                List<ClienteEntity> listaClientes = new List<ClienteEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ClienteEntity cliente = getClienteByID(reader["P2402_ID_ARRENDATARIO"].ToString());

                    //ClienteEntity cliente = new ClienteEntity()
                    //{
                    //    IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString(),
                    //    Nombre = reader["P2411_N_ARRENDATARIO"].ToString(),
                    //    RFC = reader["P2423_RFC"].ToString()                        
                    //};
                    if (cliente != null)
                    {
                        cliente.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                        listaClientes.Add(cliente);
                    }
                }
                reader.Close();
                conexion.Close();
                return listaClientes;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        public static List<ClienteEntity> getClientesCompleto()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0201_ID, P0203_NOMBRE, P0204_RFC, P0253_NOMBRE_COMERCIAL FROM T02_ARRENDATARIO WHERE P0202_TIPO_ENTE = 2 ORDER BY P0203_NOMBRE";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                List<ClienteEntity> listaClientes = new List<ClienteEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ClienteEntity cliente = new ClienteEntity()
                    {
                        IDCliente = reader["P0201_ID"].ToString(),
                        Nombre = reader["P0203_NOMBRE"].ToString().Replace(">", "-").Replace("<", "-").Replace("&", "AND").Replace("'", " ").Replace("\"", " "),
                        RFC = reader["P0204_RFC"].ToString().Replace(">", "-").Replace("<", "-").Replace("&", "AND").Replace("'", " ").Replace("\"", " "),
                        NombreComercial = reader["P0253_NOMBRE_COMERCIAL"].ToString().Trim(),
                        DomicilioFiscal = getDomicilioPorCliente(reader["P0201_ID"].ToString()),
                        Email = getEmailCliente(reader["P0201_ID"].ToString()),
                        Telefono = getTelefonoCliente(reader["P0201_ID"].ToString()),
                        Contacto = getContactoCliente(reader["P0201_ID"].ToString()),
                        RepresentanteLegal = getRepresentanteCliente(reader["P0201_ID"].ToString()),
                        Aval = getAvalCliente(reader["P0201_ID"].ToString())
                    };
                    listaClientes.Add(cliente);
                }
                reader.Close();
                conexion.Close();
                return listaClientes;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static string getEmailCliente(string idCte)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0604_DIRECCION FROM T06_MAIL_WEB WHERE P0601_ID_ENTE = ? AND P0605_ORDEN = 1 AND P0603_TIPO_SERV = 'E'";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idCte", OdbcType.VarChar).Value = idCte;
                conexion.Open();
                string result = string.Empty;
                result = comando.ExecuteScalar().ToString();
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static string getTelefonoCliente(string idCte)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0604_DIRECCION FROM T06_MAIL_WEB WHERE P0601_ID_ENTE = ? AND P0605_ORDEN = 1 AND P0603_TIPO_SERV = 'T'";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idCte", OdbcType.VarChar).Value = idCte;
                conexion.Open();
                string result = string.Empty;
                result = comando.ExecuteScalar().ToString();
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static PersonaFisicaEntity getContactoCliente(string idCte)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT T15_PERSONA.P1503_NOMBRE, T06_MAIL_WEB.P0604_DIRECCION FROM T15_PERSONA
JOIN T06_MAIL_WEB ON T06_MAIL_WEB.P0601_ID_ENTE = T15_PERSONA.P1501_ID_PERSONA 
WHERE P1504_ID_ENTE_EXT = ? AND P1502_TIPO_ENTE = 13 AND T06_MAIL_WEB.P0603_TIPO_SERV = 'T'";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idCte", OdbcType.VarChar).Value = idCte;
                conexion.Open();
                PersonaFisicaEntity persona = new PersonaFisicaEntity();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    persona.Nombre = reader["P1503_NOMBRE"].ToString();
                    persona.Telefono = reader["P0604_DIRECCION"].ToString();
                    break;
                }
                conexion.Close();
                return persona;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<PersonaFisicaEntity> getContactosCliente()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                //                string sql = @"SELECT T15_PERSONA.P1503_NOMBRE, T06_MAIL_WEB.P0604_DIRECCION, P1504_ID_ENTE_EXT, P1501_ID_PERSONA  
                //FROM T15_PERSONA 
                //JOIN T06_MAIL_WEB ON T06_MAIL_WEB.P0601_ID_ENTE = T15_PERSONA.P1501_ID_PERSONA 
                //WHERE P1502_TIPO_ENTE = 13 AND T06_MAIL_WEB.P0603_TIPO_SERV = 'T' AND T06_MAIL_WEB.P0604_DIRECCION IS NOT NULL";
                string sql = @"SELECT T15_PERSONA.P1503_NOMBRE, T06_MAIL_WEB.P0604_DIRECCION, P1504_ID_ENTE_EXT, P1501_ID_PERSONA  
FROM T15_PERSONA 
JOIN T06_MAIL_WEB ON T06_MAIL_WEB.P0601_ID_ENTE = T15_PERSONA.P1501_ID_PERSONA";
                OdbcCommand comando = new OdbcCommand(sql, conexion);                
                conexion.Open();
                List<PersonaFisicaEntity> personas = new List<PersonaFisicaEntity>();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    PersonaFisicaEntity persona = new PersonaFisicaEntity();
                    persona.IdPersona = reader["P1501_ID_PERSONA"].ToString();
                    persona.IdExt = reader["P1504_ID_ENTE_EXT"].ToString();
                    persona.Nombre = reader["P1503_NOMBRE"].ToString();
                    persona.Telefono = reader["P0604_DIRECCION"].ToString();
                    if(persona!=null)
                    {
                        personas.Add(persona);
                    }
                }
                conexion.Close();
                return personas;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static PersonaFisicaEntity getAvalCliente(string idCte)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT T15_PERSONA.P1503_NOMBRE, T06_MAIL_WEB.P0604_DIRECCION FROM T15_PERSONA
LEFT JOIN T06_MAIL_WEB ON T06_MAIL_WEB.P0601_ID_ENTE = T15_PERSONA.P1501_ID_PERSONA
WHERE P1504_ID_ENTE_EXT = ? AND P1502_TIPO_ENTE = 10 AND T06_MAIL_WEB.P0603_TIPO_SERV = 'T'";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idCte", OdbcType.VarChar).Value = idCte;
                conexion.Open();
                PersonaFisicaEntity persona = new PersonaFisicaEntity();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    persona.Nombre = reader["P1503_NOMBRE"].ToString();
                    persona.Telefono = reader["P0604_DIRECCION"].ToString();
                    break;
                }
                conexion.Close();
                return persona;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static PersonaFisicaEntity getRepresentanteCliente(string idCte)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT T15_PERSONA.P1503_NOMBRE, T06_MAIL_WEB.P0604_DIRECCION FROM T15_PERSONA
LEFT JOIN T06_MAIL_WEB ON T06_MAIL_WEB.P0601_ID_ENTE = T15_PERSONA.P1501_ID_PERSONA
WHERE P1504_ID_ENTE_EXT = ? AND P1502_TIPO_ENTE = 5 AND T06_MAIL_WEB.P0603_TIPO_SERV = 'T' and T06_MAIL_WEB.P0605_ORDEN = 1";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idCte", OdbcType.VarChar).Value = idCte;
                conexion.Open();
                PersonaFisicaEntity persona = new PersonaFisicaEntity();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    persona.Nombre = reader["P1503_NOMBRE"].ToString();
                    persona.Telefono = reader["P0604_DIRECCION"].ToString();
                    break;
                }
                conexion.Close();
                return persona;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static string getMonedaLocal()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT DESCR_CAT FROM CATEGORIA WHERE ID_COT = 'MONEDA' AND TIPO = 'P'";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                conexion.Open();
                string moneda = comando.ExecuteScalar().ToString();
                conexion.Close();
                return moneda;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }
        //Método para obtener los datos de actualización de cobranza en base de datos en Firebird
        public static ModuloCobranzaEntity getVersionCobranza()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            ModuloCobranzaEntity cobranzaEntity = new ModuloCobranzaEntity();
            try
            {
                string sql = "SELECT CAMPO_NUM1, CAMPO_DATE1 FROM CATEGORIA WHERE ID_COT = 'VC'";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                if (reader.HasRows)
                {
                    cobranzaEntity.Clave = "VC";
                    cobranzaEntity.Version = (int)Convert.ToDecimal(reader["CAMPO_NUM1"].ToString());
                    cobranzaEntity.FechaAct = Convert.ToDateTime(reader["CAMPO_DATE1"].ToString());
                }
                else
                {
                    cobranzaEntity.Clave = "VC";
                    cobranzaEntity.Version = 0;
                    cobranzaEntity.FechaAct = new DateTime(1990, 1, 1);
                }
                conexion.Close();
                return cobranzaEntity;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        //Método para obtener la version de la base de datos en Firebird
        public static int getVersionDB()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT CAMPO_NUM1 FROM CATEGORIA WHERE ID_COT = 'VA'";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                conexion.Open();
                string ver = comando.ExecuteScalar().ToString();
                int numVersion = Convert.ToInt32(Convert.ToDecimal(ver));
                conexion.Close();
                return numVersion;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }
        //Método para obtener la fecha de actualización de la base de datos en Firebird
        public static DateTime getFechaAct()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT CAMPO_DATE2 FROM CATEGORIA WHERE ID_COT = 'VA'";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                conexion.Open();
                string fechaS = comando.ExecuteScalar().ToString();
                DateTime fechaD = Convert.ToDateTime(fechaS);
                conexion.Close();
                return fechaD;
            }
            catch
            {
                conexion.Close();
                return Convert.ToDateTime("01/01/1990");
            }
        }

        public static DepositoEntity getDeposito(string idContrato, DateTime fechaFin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT SUM(P2419_TOTAL) AS Deposito, P2410_MONEDA FROM T24_HISTORIA_RECIBOS WHERE P2426_TIPO_DOC = 'G' AND P2418_ID_CONTRATO = ? AND P2409_FECHA_EMISION <= ? GROUP BY P2410_MONEDA";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = idContrato;
                comando.Parameters.Add("@Fecha", OdbcType.DateTime).Value = fechaFin;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                DepositoEntity deposito = null;
                while (reader.Read())
                {
                    deposito = new DepositoEntity()
                    {
                        Moneda = reader["P2410_MONEDA"] == DBNull.Value ? string.Empty : reader["P2410_MONEDA"].ToString() == "D" ? "Dolar" : getMonedaLocal(),
                        Cantidad = reader["Deposito"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Deposito"])
                    };
                }
                reader.Close();
                conexion.Close();
                return deposito;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<RentRollEntity> getListaRentRoll(string idInmobiliaria, string idConjunto, DateTime inicio, DateTime fin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            decimal descuentosTotal = 0M, descuento = 0M;

            decimal MantoEntreMtrs = 0;
            try
            {
                #region QuerySql
                //string sql = @"SELECT T04_CONTRATO.P0401_ID_CONTRATO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2406_STATUS, 
                //T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T07_EDIFICIO.P0703_NOMBRE,T07_EDIFICIO.P0701_ID_EDIFICIO, T07_EDIFICIO.P0707_CONTRUCCION_M2, T07_EDIFICIO.P0713_DISPONIBLE, 
                //T07_EDIFICIO.P0738_CAMPO13, T07_EDIFICIO.P0704_TIPO_ED, T02_ARRENDATARIO.P0203_NOMBRE, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL,  
                //T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2448_BANCO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, 
                //T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.CAMPO_NUM6, T24_HISTORIA_RECIBOS.CAMPO_NUM12, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, 
                //T24_HISTORIA_RECIBOS.CAMPO_NUM13, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2410_MONEDA, P2414_TIPO_CAMBIO, P2419_TOTAL,P2416_IVA 
                //FROM T24_HISTORIA_RECIBOS 
                //JOIN T04_CONTRATO ON T04_CONTRATO.P0401_ID_CONTRATO = T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO 
                //JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T04_CONTRATO.P0404_ID_EDIFICIO 
                //JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO  
                //WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ? AND T07_EDIFICIO.P0710_ID_CENTRO = ? 
                //AND ((T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?) 
                //OR (T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ?)) 
                //AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R', 'X', 'Z', 'W', 'P', 'U') 
                //AND T24_HISTORIA_RECIBOS.P2406_STATUS <> '3'";
                //string sql = @"SELECT T04_CONTRATO.P0401_ID_CONTRATO,P2402_ID_ARRENDATARIO, T04_CONTRATO.P0404_ID_EDIFICIO, T24_HISTORIA_RECIBOS.CAMPO3, T18_SUBCONJUNTOS.P1803_NOMBRE, 
                //T07_EDIFICIO.P0703_NOMBRE,T07_EDIFICIO.P0701_ID_EDIFICIO, T07_EDIFICIO.P0707_CONTRUCCION_M2, T07_EDIFICIO.P0713_DISPONIBLE, 
                //T07_EDIFICIO.P0738_CAMPO13, T07_EDIFICIO.P0704_TIPO_ED, 
                //T24_HISTORIA_RECIBOS.CAMPO4, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2406_STATUS, 
                //T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T02_ARRENDATARIO.P0203_NOMBRE, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL,  
                //T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2448_BANCO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, 
                //T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.CAMPO_NUM6, T24_HISTORIA_RECIBOS.CAMPO_NUM12, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, 
                //T24_HISTORIA_RECIBOS.CAMPO_NUM13, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2410_MONEDA, P2414_TIPO_CAMBIO, P2419_TOTAL,P2416_IVA    
                //FROM T24_HISTORIA_RECIBOS 
                //JOIN T04_CONTRATO ON T04_CONTRATO.P0401_ID_CONTRATO = T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO   
                //JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO                          
                //LEFT JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T04_CONTRATO.P0404_ID_EDIFICIO 
                //JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T24_HISTORIA_RECIBOS.CAMPO3   
                //WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ?   AND T24_HISTORIA_RECIBOS.CAMPO4 = ?            
                //AND ((T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ?  AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ? ) 
                //OR (T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ?  AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ? )) 
                //AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R', 'X', 'Z', 'W', 'P', 'U') 
                //AND T24_HISTORIA_RECIBOS.P2406_STATUS <> '3'";
                string sql = @"SELECT T04_CONTRATO.P0401_ID_CONTRATO,P2402_ID_ARRENDATARIO, T04_CONTRATO.P0404_ID_EDIFICIO, T24_HISTORIA_RECIBOS.CAMPO3, T18_SUBCONJUNTOS.P1803_NOMBRE, 
                T07_EDIFICIO.P0703_NOMBRE,T07_EDIFICIO.P0701_ID_EDIFICIO, T07_EDIFICIO.P0707_CONTRUCCION_M2, T07_EDIFICIO.P0713_DISPONIBLE, 
                T07_EDIFICIO.P0738_CAMPO13, T07_EDIFICIO.P0704_TIPO_ED, 
                T24_HISTORIA_RECIBOS.CAMPO4, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2406_STATUS, 
                T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T02_ARRENDATARIO.P0203_NOMBRE, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL,  
                T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2448_BANCO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, 
                T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.CAMPO_NUM6, T24_HISTORIA_RECIBOS.CAMPO_NUM12, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, 
                T24_HISTORIA_RECIBOS.CAMPO_NUM13, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2410_MONEDA, P2414_TIPO_CAMBIO, P2419_TOTAL, P2416_IVA    
                FROM T24_HISTORIA_RECIBOS 
                JOIN T04_CONTRATO ON T04_CONTRATO.P0401_ID_CONTRATO = T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO   
                JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO                          
                LEFT JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T04_CONTRATO.P0404_ID_EDIFICIO 
                JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T24_HISTORIA_RECIBOS.CAMPO3 
                WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ?    
                AND T24_HISTORIA_RECIBOS.CAMPO4 = ?      
                AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION BETWEEN ?  AND ?   
                AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO BETWEEN  ?  AND  ?    
                AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R', 'X', 'Z', 'W', 'P', 'U') 
                AND T24_HISTORIA_RECIBOS.P2406_STATUS <> '3' 
";
                #endregion

                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@IDCnj", OdbcType.VarChar).Value = idConjunto;
                comando.Parameters.Add("@Inicio", OdbcType.DateTime).Value = inicio.Date;
                comando.Parameters.Add("@Fin", OdbcType.DateTime).Value = fin.Date;
                comando.Parameters.Add("@Inicio2", OdbcType.DateTime).Value = inicio.Date;
                comando.Parameters.Add("@Fin2", OdbcType.DateTime).Value = fin.Date;
                List<int> listaIdsYaAgregados = new List<int>();
                List<RentRollEntity> listaRecibos = new List<RentRollEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                
                while (reader.Read())
                {
                    RentRollEntity recibo = new RentRollEntity();
                    recibo.IdCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    recibo.IDContrato = reader["P0401_ID_CONTRATO"].ToString();
                    recibo.IDHistRec = Convert.ToInt32(reader["P2444_ID_HIST_REC"].ToString());
                    recibo.NombreInmueble = reader["P0703_NOMBRE"].ToString();
                    recibo.IdInmueble = reader["P0701_ID_EDIFICIO"].ToString();
                    recibo.IdEdificio = reader["P0404_ID_EDIFICIO"].ToString();
                    recibo.NombreSubconjunto = reader["P1803_NOMBRE"].ToString();
                    recibo.NombreCliente = reader["P0203_NOMBRE"].ToString();
                    recibo.NombreClienteComercial = reader["P0253_NOMBRE_COMERCIAL"].ToString();
                    //recibo.MetrosCuadradosConstruccion = string.IsNullOrEmpty(reader["CAMPO_NUM1"].ToString()) ? 0 : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                    recibo.MetrosCuadradosConstruccion = string.IsNullOrEmpty(reader["P0707_CONTRUCCION_M2"].ToString()) ? 0 : Convert.ToDecimal(reader["P0707_CONTRUCCION_M2"]);
                    recibo.Estatus = Convert.ToInt32(reader["P2406_STATUS"].ToString());
                    recibo.Importe = string.IsNullOrEmpty(reader["P2405_IMPORTE"].ToString()) ? 0 : Convert.ToDecimal(reader["P2405_IMPORTE"]);
                    recibo.ImpuestoIVA = string.IsNullOrEmpty(reader["P2416_IVA"].ToString()) ? 0 : Convert.ToDecimal(reader["P2416_IVA"]);
                    recibo.TipoCargo = string.IsNullOrEmpty(reader["CAMPO_NUM12"].ToString()) ? 0 : Convert.ToInt32(reader["CAMPO_NUM12"]);
                    recibo.TotalCargosPeriodicos = string.IsNullOrEmpty(reader["CAMPO_NUM6"].ToString()) ? 0 : Convert.ToDecimal(reader["CAMPO_NUM6"]);
                    recibo.CantidadPorPagar = string.IsNullOrEmpty(reader["P2427_CTD_PAG"].ToString()) ? 0 : Convert.ToDecimal(reader["P2427_CTD_PAG"]);
                    recibo.TipoRecibo = reader["P2426_TIPO_DOC"].ToString();
                    recibo.EsRentaAnticipada = string.IsNullOrEmpty(reader["CAMPO_NUM13"].ToString()) ? false : Convert.ToInt32(reader["CAMPO_NUM13"]) == 1;
                    recibo.CuentaBancaria = string.IsNullOrEmpty(reader["P2448_BANCO"].ToString()) ? "No identificado" : reader["P2448_BANCO"].ToString();
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.FechaPagado = string.IsNullOrEmpty(reader["P2408_FECHA_PAGADO"].ToString()) ? new DateTime() : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.IDHistRelacionado = string.IsNullOrEmpty(reader["P2403_NUM_RECIBO"].ToString()) ? 0 : Convert.ToInt32(reader["P2403_NUM_RECIBO"]);
                    recibo.ClasificacionUbicacion = string.IsNullOrEmpty(reader["P0704_TIPO_ED"].ToString()) ? "Clasificación no definida" : reader["P0704_TIPO_ED"].ToString();
                    recibo.Moneda = string.IsNullOrEmpty(reader["P2410_MONEDA"].ToString()) ? "P" : reader["P2410_MONEDA"].ToString();
                    recibo.MonedaID = string.IsNullOrEmpty(reader["P2410_MONEDA"].ToString()) ? "P" : reader["P2410_MONEDA"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString() == "P" ? "Contratos en Pesos" : "Contratos en Dolares";
                    recibo.TipoDeCambioEmitido = string.IsNullOrEmpty(reader["P2414_TIPO_CAMBIO"].ToString()) ? 1.0m : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"]);
                    //Agregado 22/06/2016 by Uz
                    recibo.TotalFacturado = string.IsNullOrEmpty(reader["P2419_TOTAL"].ToString()) ? 0.0m : Convert.ToDecimal(reader["P2419_TOTAL"]);
                    //Agregado 13/08/2015 by Uzcanga
                    recibo.InmuebleDisponible = reader["P0713_DISPONIBLE"].ToString();

                    #region subConjuntos
                    if (recibo.IdEdificio.Contains("SCNJ"))
                    {
                        //System.Diagnostics.Debugger.Break();
                        recibo.NombreInmueble = recibo.NombreSubconjunto;
                    }
                    #endregion

                    if (recibo.InmuebleDisponible != "N")
                        recibo.MetrosCuadradosDisponibles = string.IsNullOrEmpty(reader["P0707_CONTRUCCION_M2"].ToString()) ? 0 : Convert.ToDecimal(reader["P0707_CONTRUCCION_M2"]);
                    else
                        recibo.MetrosCuadradosDisponibles = 0;
                    //Modificado 20/08/2015 by Uzcanga
                    if (recibo.EsRentaAnticipada)
                    {
                        if (recibo.FechaEmision >= inicio && recibo.FechaEmision <= fin)
                            recibo.RentasAnticipadasEmitidas = recibo.Importe;
                        if (recibo.FechaPagado >= inicio && recibo.FechaPagado <= fin)
                            recibo.ImporteRentaCobrado = recibo.Importe;
                    }
                    else
                    {
                        if (recibo.FechaEmision >= inicio && recibo.FechaEmision <= fin)
                        {
                            if (recibo.TipoRecibo == "R" || recibo.TipoRecibo == "Z" || recibo.TipoRecibo == "X" || recibo.TipoRecibo == "W")
                            {
                                if (recibo.TipoCargo == 0)
                                {
                                    recibo.RentaVariableEmitida = getImporteCargoPorIDyTipo(recibo.IDHistRec, 2);
                                    recibo.ImporteRentaEmitido = recibo.Importe - recibo.TotalCargosPeriodicos + recibo.RentaVariableEmitida;
                                    recibo.ImporteMantenimientoEmitido = getImporteCargoPorIDyTipo(recibo.IDHistRec, 1);
                                    recibo.ImportePublicidadEmitido = getImporteCargoPorIDyTipo(recibo.IDHistRec, 4, "PUB");
                                    recibo.ImporteServiciosEmitido = getImporteCargoPorIDyTipo(recibo.IDHistRec, 4, "SER");
                                    recibo.OtrosEmitidos = recibo.TotalCargosPeriodicos - recibo.ImporteMantenimientoEmitido - recibo.RentaVariableEmitida - recibo.ImportePublicidadEmitido - recibo.ImporteServiciosEmitido;
                                    recibo.DescuentoEmitido = getImporteCargoPorIDyTipo(recibo.IDHistRec, 5);
                                    recibo.OtrosEmitidos -= recibo.DescuentoEmitido;
                                    
                                }
                                else if (recibo.TipoCargo == 1)
                                    recibo.ImporteMantenimientoEmitido = recibo.Importe;
                                else if (recibo.TipoCargo == 2)
                                    recibo.ImporteRentaEmitido = recibo.Importe;
                                else
                                    recibo.OtrosEmitidos = recibo.Importe;
                            }
                        }
                        if (recibo.Estatus == 2 /*|| recibo.TipoRecibo == "X" || recibo.TipoRecibo == "W"*/)
                        {
                            if (recibo.FechaPagado >= inicio && recibo.FechaPagado <= fin)
                            {
                                if (recibo.TipoRecibo == "R" || recibo.TipoRecibo == "Z")
                                {
                                    if (recibo.TipoCargo == 0)
                                    {
                                        recibo.ImporteRentaCobrado = recibo.Importe - recibo.TotalCargosPeriodicos;
                                        //recibo.ImporteRentaEmitido = recibo.Importe - recibo.TotalCargosPeriodicos + recibo.RentaVariableEmitida;

                                        recibo.ImporteMantenimientoCobrado = getImporteCargoPorIDyTipo(recibo.IDHistRec, 1);
                                        recibo.RentaVariableCobrado = getImporteCargoPorIDyTipo(recibo.IDHistRec, 2);
                                        recibo.ImportePublicidadCobrado = getImporteCargoPorIDyTipo(recibo.IDHistRec, 4, "PUB");
                                        recibo.ImporteServiciosCobrado = getImporteCargoPorIDyTipo(recibo.IDHistRec, 4, "SER");
                                        recibo.OtrosCobrado = recibo.TotalCargosPeriodicos - recibo.ImporteMantenimientoCobrado - recibo.RentaVariableCobrado - recibo.ImportePublicidadCobrado - recibo.ImporteServiciosCobrado;
                                    }
                                    else if (recibo.TipoCargo == 1)
                                        //recibo.ImporteMantenimientoEmitido=recibo.Importe;
                                        recibo.ImporteMantenimientoCobrado = recibo.Importe;
                                    else if (recibo.TipoCargo == 2)
                                        //recibo.RentaVariableEmitida = recibo.Importe;
                                        recibo.RentaVariableCobrado = recibo.Importe;
                                    else
                                        //recibo.OtrosEmitidos = recibo.Importe;
                                        recibo.OtrosCobrado = recibo.Importe;
                                }
                                else if (recibo.TipoRecibo == "P" || recibo.TipoRecibo == "U")
                                {
                                    if (!listaIdsYaAgregados.Contains(recibo.IDHistRelacionado))
                                    {
                                        decimal totalCobradoAnteriormente = getTotalCobradoPorIDyFechas(recibo.IDHistRelacionado, new DateTime(2000, 1, 1), inicio.AddDays(-1));
                                        decimal totalAReflejar = getTotalCobradoPorIDyFechas(recibo.IDHistRelacionado, inicio, fin);
                                        if (recibo.TipoCargo == 0)
                                        {
                                            decimal importeRentaEmitido = getImporteRentaXW(recibo.IDHistRelacionado);
                                            if (totalCobradoAnteriormente >= importeRentaEmitido)
                                            {
                                                decimal diferencia = totalCobradoAnteriormente - importeRentaEmitido;
                                                decimal importeMantenimientoEmitido = getImporteCargoPorIDyTipo(recibo.IDHistRelacionado, 1);
                                                if (diferencia > importeMantenimientoEmitido)
                                                {
                                                    diferencia -= importeMantenimientoEmitido;
                                                    decimal rentaVariableEmitida = getImporteCargoPorIDyTipo(recibo.IDHistRelacionado, 2);
                                                    if (diferencia > rentaVariableEmitida)
                                                    {
                                                        diferencia -= rentaVariableEmitida;
                                                        recibo.OtrosCobrado = totalAReflejar;
                                                    }
                                                    else
                                                    {
                                                        if ((diferencia + totalAReflejar) > rentaVariableEmitida)
                                                        {
                                                            recibo.RentaVariableCobrado = rentaVariableEmitida - diferencia;
                                                            totalAReflejar -= recibo.RentaVariableCobrado;
                                                            recibo.OtrosCobrado = totalAReflejar;
                                                        }
                                                        else
                                                            recibo.RentaVariableCobrado = totalAReflejar;
                                                    }
                                                }
                                                else
                                                {
                                                    if ((diferencia + totalAReflejar) > importeMantenimientoEmitido)
                                                    {
                                                        recibo.ImporteMantenimientoCobrado = importeMantenimientoEmitido - diferencia;
                                                        totalAReflejar -= recibo.ImporteMantenimientoCobrado;
                                                        decimal rentaVariableEmitida = getImporteCargoPorIDyTipo(recibo.IDHistRelacionado, 2);
                                                        if (totalAReflejar > rentaVariableEmitida)
                                                        {
                                                            recibo.RentaVariableCobrado = rentaVariableEmitida;
                                                            totalAReflejar -= rentaVariableEmitida;
                                                            recibo.OtrosCobrado = totalAReflejar;
                                                        }
                                                        else
                                                            recibo.RentaVariableCobrado = totalAReflejar;
                                                    }
                                                    else
                                                        recibo.ImporteMantenimientoCobrado = totalAReflejar;
                                                }
                                            }
                                            else
                                            {
                                                if ((totalCobradoAnteriormente + totalAReflejar) > importeRentaEmitido)
                                                {
                                                    recibo.ImporteRentaCobrado = importeRentaEmitido - totalCobradoAnteriormente;
                                                    totalAReflejar -= recibo.ImporteRentaCobrado;
                                                    decimal importeMantenimientoEmitido = getImporteCargoPorIDyTipo(recibo.IDHistRelacionado, 1);
                                                    if (totalAReflejar > importeMantenimientoEmitido)
                                                    {
                                                        recibo.ImporteMantenimientoCobrado = importeMantenimientoEmitido;
                                                        totalAReflejar -= importeMantenimientoEmitido;
                                                        decimal rentaVariableEmitida = getImporteCargoPorIDyTipo(recibo.IDHistRelacionado, 2);
                                                        if (totalAReflejar > rentaVariableEmitida)
                                                        {
                                                            recibo.RentaVariableCobrado = rentaVariableEmitida;
                                                            totalAReflejar -= rentaVariableEmitida;
                                                            recibo.OtrosCobrado = totalAReflejar;
                                                        }
                                                        else
                                                            recibo.RentaVariableCobrado = totalAReflejar;
                                                    }
                                                    else
                                                        recibo.ImporteMantenimientoCobrado = totalAReflejar;
                                                }
                                                else
                                                    recibo.ImporteRentaCobrado = totalAReflejar;
                                            }
                                        }
                                        else if (recibo.TipoCargo == 1)
                                            recibo.ImporteMantenimientoCobrado = totalAReflejar;
                                        else if (recibo.TipoCargo == 2)
                                            recibo.RentaVariableCobrado = totalAReflejar;
                                        else
                                            recibo.OtrosCobrado = totalAReflejar;
                                        listaIdsYaAgregados.Add(recibo.IDHistRelacionado);
                                    }
                                }
                                /*else if (recibo.TipoRecibo == "X" || recibo.TipoRecibo == "W")
                                {
                                    decimal totalCobrado = getTotalCobradoPorIDyFechas(recibo.IDHistRec, inicio.Date, fin.Date);
                                    if (totalCobrado > 0)
                                    {
                                        if (recibo.TipoCargo == 0)
                                        {
                                            if (totalCobrado >= (recibo.Importe - recibo.TotalCargosPeriodicos))
                                            {
                                                recibo.ImporteRentaCobrado = recibo.Importe - recibo.TotalCargosPeriodicos;
                                                decimal diferencia = totalCobrado - recibo.ImporteRentaCobrado;
                                                decimal importeMantenimientoEmitido = getImporteCargoPorIDyTipo(recibo.IDHistRec, 1);
                                                if (diferencia >= importeMantenimientoEmitido)
                                                {
                                                    recibo.ImporteMantenimientoCobrado = importeMantenimientoEmitido;
                                                    diferencia = diferencia - importeMantenimientoEmitido;
                                                    decimal rentaVariableEmitida = getImporteCargoPorIDyTipo(recibo.IDHistRec, 2);
                                                    if (diferencia >= rentaVariableEmitida)
                                                    {
                                                        recibo.RentaVariableCobrado = rentaVariableEmitida;
                                                        diferencia -= rentaVariableEmitida;
                                                        recibo.OtrosCobrado = diferencia;
                                                    }
                                                    else
                                                        recibo.RentaVariableCobrado = diferencia;
                                                }
                                                else
                                                    recibo.ImporteMantenimientoCobrado = diferencia;
                                            }
                                            else
                                                recibo.ImporteRentaCobrado = totalCobrado;
                                        }
                                        else if (recibo.TipoCargo == 1)
                                            recibo.ImporteMantenimientoCobrado = totalCobrado;
                                        else if (recibo.TipoCargo == 2)
                                            recibo.RentaVariableCobrado = totalCobrado;
                                        else
                                            recibo.OtrosCobrado = totalCobrado;
                                    }
                                }*/
                                /*else
                                {
                                    if (recibo.TipoCargo == 0)
                                    {
                                        if (recibo.CantidadPorPagar <= (recibo.Importe - recibo.TotalCargosPeriodicos))
                                            recibo.ImporteRentaCobrado = recibo.Importe - recibo.TotalCargosPeriodicos - recibo.CantidadPorPagar;
                                        else
                                        {
                                            recibo.ImporteRentaCobrado = recibo.Importe - recibo.TotalCargosPeriodicos;
                                            decimal restoPorAplicar = recibo.CantidadPorPagar - recibo.ImporteRentaCobrado;
                                            if (restoPorAplicar > recibo.ImporteMantenimientoEmitido)
                                            {
                                                recibo.ImporteMantenimientoCobrado = recibo.ImporteMantenimientoEmitido;
                                                restoPorAplicar = restoPorAplicar - recibo.ImporteMantenimientoCobrado;
                                            }
                                        }
                                    }
                                    else if (recibo.TipoCargo == 1)
                                        recibo.ImporteMantenimientoCobrado = recibo.Importe - recibo.CantidadPorPagar;
                                    else if (recibo.TipoCargo == 2)
                                        recibo.RentaVariableCobrado = recibo.Importe - recibo.CantidadPorPagar;
                                    else
                                        recibo.OtrosCobrado = recibo.Importe - recibo.CantidadPorPagar;
                                }*/
                            }
                        }
                    }
                    if (recibo.Estatus != 3)
                        listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch(Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        public static List<RentRollEntity> getSubConjuntos(RentRollEntity recibo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT DISTINCT P1801_ID_SUBCONJUNTO, P1803_NOMBRE, P1804_ID_CONJUNTO, P18_CAMPO4, P0203_NOMBRE,P0707_CONTRUCCION_M2
FROM T18_SUBCONJUNTOS
LEFT JOIN T24_HISTORIA_RECIBOS ON T24_HISTORIA_RECIBOS.CAMPO17 =  T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO
LEFT JOIN T04_CONTRATO ON T04_CONTRATO.P0401_ID_CONTRATO = T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO
LEFT JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T04_CONTRATO.P0404_ID_EDIFICIO 
LEFT JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO 
WHERE   P1801_ID_SUBCONJUNTO= ?  AND P1804_ID_CONJUNTO = ? ";

                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idSubC", OdbcType.VarChar).Value = recibo.IdSubconjunto;
                comando.Parameters.Add("@idCentroSubC", OdbcType.VarChar).Value = recibo.IdCentroSubConjunto;
                conexion.Open();
                List<RentRollEntity> Lista = new List<RentRollEntity>();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    RentRollEntity obj = new RentRollEntity();
                    obj.IdSubconjunto = reader["P1801_ID_SUBCONJUNTO"].ToString();
                    obj.NombreSubconjunto = reader["P1803_NOMBRE"].ToString();
                    obj.IdCentroSubConjunto = reader["P1804_ID_CONJUNTO"].ToString();
                    obj.IdEdificio = reader["P18_CAMPO4"].ToString();
                    obj.NombreCliente = reader["P0203_NOMBRE"].ToString();
                    obj.MetrosCuadradosConstruccion = string.IsNullOrEmpty(reader["P0707_CONTRUCCION_M2"].ToString()) ? 0 : Convert.ToDecimal(reader["P0707_CONTRUCCION_M2"]);
                    if (obj != null)
                    {
                        Lista.Add(obj);
                    }
                }
                conexion.Close();
                return Lista;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        public static List<RentRollEntity> getDatosSubconjuntosTContratos(RentRollEntity recibo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P0401_ID_CONTRATO,P0404_ID_EDIFICIO FROM T04_CONTRATO                                          
                WHERE T04_CONTRATO.P0402_ID_ARRENDAT= {0} AND P0404_ID_EDIFICIO LIKE'SCN%' ";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idCliente", OdbcType.VarChar).Value = recibo.IdCliente;
                conexion.Open();
                List<RentRollEntity> list = new List<RentRollEntity>();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    RentRollEntity obj = new RentRollEntity();
                    obj.IDContrato = reader["P0401_ID_CONTRATO"].ToString();
                    obj.IdSubconjunto = reader["P1801_ID_SUBCONJUNTO"].ToString();
                    list.Add(obj);
                }
                conexion.Close();
                return list;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        public static decimal getImporteRentaXW(int idHistRec)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P2405_IMPORTE, CAMPO_NUM6 FROM T24_HISTORIA_RECIBOS WHERE P2444_ID_HIST_REC = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDHist", OdbcType.Int).Value = idHistRec;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                decimal importe = 0, totalCargos = 0;
                while (reader.Read())
                {
                    importe = Convert.ToDecimal(reader["P2405_IMPORTE"]);
                    totalCargos = string.IsNullOrEmpty(reader["CAMPO_NUM6"].ToString()) ? 0 : Convert.ToDecimal(reader["CAMPO_NUM6"]);
                }
                reader.Close();
                conexion.Close();
                return importe - totalCargos;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getImporteCargoPorIDyTipo(int idHistRec, int tipo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P3403_IMPORTE FROM T35_HISTORIAL_CARGOS WHERE P3409_ID_HIST_REC = ? AND CAMPO_NUM3 = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDHistRec", OdbcType.Int).Value = idHistRec;
                comando.Parameters.Add("@Tipo", OdbcType.Numeric).Value = tipo;
                decimal totalCargo = 0;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                    totalCargo += string.IsNullOrEmpty(reader["P3403_IMPORTE"].ToString()) ? 0 : Convert.ToDecimal(reader["P3403_IMPORTE"]);
                reader.Close();
                conexion.Close();
                return totalCargo;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }
        //Agregado 20/08/2015 by Ing. Uzcanga
        public static decimal getImporteCargoPorIDyTipo(int idHistRec, int tipo, string clave)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P3403_IMPORTE FROM T35_HISTORIAL_CARGOS WHERE P3409_ID_HIST_REC = ? AND CAMPO_NUM3 = ? AND CAMPO4=?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDHistRec", OdbcType.Int).Value = idHistRec;
                comando.Parameters.Add("@Tipo", OdbcType.Numeric).Value = tipo;
                comando.Parameters.Add("@Clave", OdbcType.VarChar).Value = clave;
                decimal totalCargo = 0;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                    totalCargo += string.IsNullOrEmpty(reader["P3403_IMPORTE"].ToString()) ? 0 : Convert.ToDecimal(reader["P3403_IMPORTE"]);
                reader.Close();
                conexion.Close();
                return totalCargo;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }
        public static decimal getTotalCobradoPorIDyFechas(int idHistRec, DateTime fechaInicio, DateTime fechaFin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT CASE WHEN SUM(P2405_IMPORTE) IS NULL THEN 0 ELSE SUM(P2405_IMPORTE) END FROM T24_HISTORIA_RECIBOS WHERE P2403_NUM_RECIBO = ? AND P2408_FECHA_PAGADO >= ? AND P2408_FECHA_PAGADO <= ? AND P2426_TIPO_DOC IN ('P', 'U')";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDHist", OdbcType.Int).Value = idHistRec;
                comando.Parameters.Add("@Inicio", OdbcType.Date).Value = fechaInicio;
                comando.Parameters.Add("@Fin", OdbcType.Date).Value = fechaFin;
                conexion.Open();
                decimal result = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }


        public static List<ReciboEntity> getListaRecibos(string idInmobiliaria, DateTime fechaCorte)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                //string sql = "SELECT * FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION <= ? AND (P2406_STATUS = '1' OR (P2406_STATUS = '2' AND P2408_FECHA_PAGADO > ?) OR (P2406_STATUS = '3' AND P2408_FECHA_PAGADO > ?))";
                //string sql = "SELECT * FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION <= ? AND (P2406_STATUS = '1' OR (P2406_STATUS = '2' AND P2408_FECHA_PAGADO > ?) OR (P2406_STATUS = '3' AND P2408_FECHA_PAGADO > ?)) AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC in('R','X','T','Z','W','L') OR (P2426_TIPO_DOC = 'T' and T24_HISTORIA_RECIBOS.P2406_STATUS = '0')";
                string sql = @"SELECT * FROM  
T24_HISTORIA_RECIBOS  
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC 
LEFT JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P18_CAMPO4 =   T24_HISTORIA_RECIBOS.CAMPO3 
WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION <= ? 
AND (P2406_STATUS = '1' OR (P2406_STATUS = '2'  
AND P2408_FECHA_PAGADO >  ? ) OR (P2406_STATUS = '3'  
AND P2408_FECHA_PAGADO >  ? ))  
AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC in('R','X','T','Z','W','L') OR (P2426_TIPO_DOC = 'T'  
and T24_HISTORIA_RECIBOS.P2406_STATUS = '0') ";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@Fecha", OdbcType.Date).Value = fechaCorte.Date;
                comando.Parameters.Add("@Fecha2", OdbcType.Date).Value = fechaCorte.Date;
                comando.Parameters.Add("@Fecha3", OdbcType.Date).Value = fechaCorte.Date;
                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    recibo.IDHistRec = Convert.ToInt32(reader["P2444_ID_HIST_REC"]);
                    recibo.Numero = reader["P2403_NUM_RECIBO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P2403_NUM_RECIBO"]);
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    recibo.IDConjunto = reader["CAMPO4"].ToString();
                    recibo.IdEdificio = reader["CAMPO3"].ToString(); 
                    recibo.IdSubconjunto = reader["P1801_ID_SUBCONJUNTO"].ToString();
                    recibo.IdCentroSubConjunto = reader["P1804_ID_CONJUNTO"].ToString();
                    recibo.NombreSubconjunto = reader["P1803_NOMBRE"].ToString();
                    recibo.NombreConjunto = reader["CAMPO5"].ToString();
                    recibo.TipoDoc = reader["P2426_TIPO_DOC"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.Estatus = reader["P2406_STATUS"].ToString();
                    recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    recibo.CantidadPorPagar = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"];
                    recibo.Periodo = reader["P2404_PERIODO"].ToString();
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"];
                    recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                    recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    recibo.TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 1 : (decimal)reader["P2414_TIPO_CAMBIO"];
                    recibo.TotalIVA = reader["P2416_IVA"] == DBNull.Value ? 1 : (decimal)reader["P2416_IVA"];
                    recibo.VencimientoPago = reader["CAMPO_DATE1"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CAMPO_DATE1"]);
                    recibo.Descuento = reader["P2424_DESCUENTO"] == DBNull.Value ? 0 : (decimal)reader["P2424_DESCUENTO"];
                    recibo.Inmueble = reader["CAMPO9"].ToString();
                    recibo.IDContrato = reader["P2418_ID_CONTRATO"].ToString();
                    recibo.MonedaPago = reader["P2420_MONEDA_PAGO"].ToString();
                    //CONDICION
                    recibo.TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 1 : (decimal)reader["P2421_TC_PAGO"];
                    recibo.CtdPag = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"];
                    //if (recibo.CtdPag > 0)                 
                    #region Sub Conjunto                    

                    if (recibo.IdSubconjunto.Contains("SCNJ"))
                    {
                        recibo.Inmueble = recibo.NombreSubconjunto;
                        recibo.NombreConjunto = getNombreConjunto(recibo.IdCentroSubConjunto);
                    }
                    #endregion
                    if (string.IsNullOrEmpty(recibo.NombreConjunto) && !string.IsNullOrEmpty(recibo.IDConjunto))
                    {
                        recibo.NombreConjunto = getNombreConjunto(recibo.IDConjunto);
                        if (string.IsNullOrEmpty(recibo.NombreConjunto))
                            recibo.NombreConjunto = getNombreConjunto(recibo.IdCentroSubConjunto);
                    }
                    if(string.IsNullOrEmpty(recibo.Inmueble) && string.IsNullOrEmpty(recibo.NombreConjunto))
                    {
                        recibo.IdSubconjunto = getIdEdificio(recibo.IDContrato);
                        if (!string.IsNullOrEmpty(recibo.IdSubconjunto))
                        {
                            if (recibo.IdSubconjunto.Contains("SCNJ"))
                            {
                                recibo.Inmueble = getNombreSubConjunto(recibo.IdSubconjunto);
                                recibo.IdCentroSubConjunto = getIdCentro(recibo.IdSubconjunto);
                                recibo.NombreConjunto = getNombreConjunto(recibo.IdCentroSubConjunto);
                            }
                        }
                    }

                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch(Exception ex)
            {
                conexion.Close();
                return null;
            }
        }


        #region 30 60 90 por rubro
        public static List<ReciboEntity> getIDClientes(string idInmobiliaria, DateTime fechaCorte)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                //string sql = "SELECT * FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION <= ? AND (P2406_STATUS = '1' OR (P2406_STATUS = '2' AND P2408_FECHA_PAGADO > ?) OR (P2406_STATUS = '3' AND P2408_FECHA_PAGADO > ?))";
                string sql = "SELECT * FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION <= ? AND (P2406_STATUS = '1' OR (P2406_STATUS = '2' AND P2408_FECHA_PAGADO > ?) OR (P2406_STATUS = '3' AND P2408_FECHA_PAGADO > ?)) AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC in('R','X','T','Z','W','L') OR (P2426_TIPO_DOC = 'T' and T24_HISTORIA_RECIBOS.P2406_STATUS = '0')";

                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@Fecha", OdbcType.Date).Value = fechaCorte.Date;
                comando.Parameters.Add("@Fecha2", OdbcType.Date).Value = fechaCorte.Date;
                comando.Parameters.Add("@Fecha3", OdbcType.Date).Value = fechaCorte.Date;
                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    recibo.IDHistRec = Convert.ToInt32(reader["P2444_ID_HIST_REC"]);
                    recibo.Numero = reader["P2403_NUM_RECIBO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P2403_NUM_RECIBO"]);
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    recibo.IDConjunto = reader["CAMPO4"].ToString();
                    recibo.NombreConjunto = reader["CAMPO5"].ToString();
                    recibo.TipoDoc = reader["P2426_TIPO_DOC"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.Estatus = reader["P2406_STATUS"].ToString();
                    recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    recibo.CantidadPorPagar = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"];
                    recibo.Periodo = reader["P2404_PERIODO"].ToString();
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"];
                    recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                    recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    recibo.TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 1 : (decimal)reader["P2414_TIPO_CAMBIO"];
                    recibo.TotalIVA = reader["P2416_IVA"] == DBNull.Value ? 1 : (decimal)reader["P2416_IVA"];
                    recibo.VencimientoPago = reader["CAMPO_DATE1"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CAMPO_DATE1"]);
                    recibo.Descuento = reader["P2424_DESCUENTO"] == DBNull.Value ? 0 : (decimal)reader["P2424_DESCUENTO"];
                    recibo.Inmueble = reader["CAMPO9"].ToString();
                    recibo.IDContrato = reader["P2418_ID_CONTRATO"].ToString();
                    recibo.MonedaPago = reader["P2420_MONEDA_PAGO"].ToString();
                    recibo.Campo20 = reader["CAMPO20"] == DBNull.Value ? string.Empty : (string)reader["CAMPO20"];
                    //CONDICION
                    recibo.TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 1 : (decimal)reader["P2421_TC_PAGO"];
                    recibo.CtdPag = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"];
                    //if (recibo.CtdPag > 0)                   
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getListaRecibosxRubro(string idInmobiliaria, DateTime fechaCorte)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                //string sql = "SELECT * FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION <= ? AND (P2406_STATUS = '1' OR (P2406_STATUS = '2' AND P2408_FECHA_PAGADO > ?) OR (P2406_STATUS = '3' AND P2408_FECHA_PAGADO > ?))";
                string sql = "SELECT * FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION <= ? AND (P2406_STATUS = '1' OR (P2406_STATUS = '2' AND P2408_FECHA_PAGADO > ?) OR (P2406_STATUS = '3' AND P2408_FECHA_PAGADO > ?)) AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC in('R','X','T','Z','W','L') OR (P2426_TIPO_DOC = 'T' and T24_HISTORIA_RECIBOS.P2406_STATUS = '0')";

                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@Fecha", OdbcType.Date).Value = fechaCorte.Date;
                comando.Parameters.Add("@Fecha2", OdbcType.Date).Value = fechaCorte.Date;
                comando.Parameters.Add("@Fecha3", OdbcType.Date).Value = fechaCorte.Date;
                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    recibo.IDHistRec = Convert.ToInt32(reader["P2444_ID_HIST_REC"]);
                    recibo.Numero = reader["P2403_NUM_RECIBO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P2403_NUM_RECIBO"]);
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    recibo.IDConjunto = reader["CAMPO4"].ToString();
                    recibo.NombreConjunto = reader["CAMPO5"].ToString();
                    recibo.TipoDoc = reader["P2426_TIPO_DOC"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.Estatus = reader["P2406_STATUS"].ToString();
                    recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    recibo.CantidadPorPagar = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"];
                    recibo.Periodo = reader["P2404_PERIODO"].ToString();
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"];
                    recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                    recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    recibo.TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 1 : (decimal)reader["P2414_TIPO_CAMBIO"];
                    recibo.TotalIVA = reader["P2416_IVA"] == DBNull.Value ? 1 : (decimal)reader["P2416_IVA"];
                    recibo.VencimientoPago = reader["CAMPO_DATE1"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CAMPO_DATE1"]);
                    recibo.Descuento = reader["P2424_DESCUENTO"] == DBNull.Value ? 0 : (decimal)reader["P2424_DESCUENTO"];
                    recibo.Inmueble = reader["CAMPO9"].ToString();
                    recibo.IDContrato = reader["P2418_ID_CONTRATO"].ToString();
                    recibo.MonedaPago = reader["P2420_MONEDA_PAGO"].ToString();
                    recibo.Campo20 = reader["CAMPO20"] == DBNull.Value ? string.Empty : (string)reader["CAMPO20"];
                    //CONDICION
                    recibo.TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 1 : (decimal)reader["P2421_TC_PAGO"];
                    recibo.CtdPag = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"];

                    recibo.cliente = SaariDB.getClienteByID(recibo.IDCliente);
                    recibo.Cargos = getCargosFacturadosxIdHistRec(recibo.IDHistRec);
                    if (recibo.Cargos != null)
                    {
                        if (recibo.Cargos.Count > 0)
                        {
                            foreach (ReciboEntity cargo in recibo.Cargos)
                            {
                                cargo.IDCliente = recibo.IDCliente;
                                cargo.IDConjunto = recibo.IDConjunto;

                                recibo.Total = recibo.Total - cargo.Total;
                                recibo.Importe = recibo.Importe - cargo.Importe;
                                recibo.TotalIVA = recibo.TotalIVA - cargo.TotalIVA;

                                listaRecibos.Add(cargo);
                            }
                        }

                    }

                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getListaClientes(string idInmobiliaria, string idConjunto, DateTime fechaCorte)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                if (String.IsNullOrEmpty(idConjunto))
                    sql = "SELECT P2402_ID_ARRENDATARIO, FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION <= ? AND (P2406_STATUS = '1' OR (P2406_STATUS = '2' AND P2408_FECHA_PAGADO > ?) OR (P2406_STATUS = '3' AND P2408_FECHA_PAGADO > ?)) AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC in('R','X','T','Z','W','L') OR (P2426_TIPO_DOC = 'T' and T24_HISTORIA_RECIBOS.P2406_STATUS = '0')";
                else
                    sql = "SELECT P2402_ID_ARRENDATARIO, FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION <= ? AND CAMPO4 = ? AND (P2406_STATUS = '1' OR (P2406_STATUS = '2' AND P2408_FECHA_PAGADO > ?) OR (P2406_STATUS = '3' AND P2408_FECHA_PAGADO > ?)) AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC in('R','X','T','Z','W','L') OR (P2426_TIPO_DOC = 'T' and T24_HISTORIA_RECIBOS.P2406_STATUS = '0')";

                OdbcCommand comando = new OdbcCommand(sql, conexion);
                if (String.IsNullOrEmpty(idConjunto))
                {
                    comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                    comando.Parameters.Add("@Fecha", OdbcType.Date).Value = fechaCorte.Date;
                    comando.Parameters.Add("@Fecha2", OdbcType.Date).Value = fechaCorte.Date;
                    comando.Parameters.Add("@Fecha3", OdbcType.Date).Value = fechaCorte.Date;
                }
                else
                {
                    comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                    comando.Parameters.Add("@Fecha", OdbcType.Date).Value = fechaCorte.Date;
                    comando.Parameters.Add("@IDConj", OdbcType.VarChar).Value = idConjunto;
                    comando.Parameters.Add("@Fecha2", OdbcType.Date).Value = fechaCorte.Date;
                    comando.Parameters.Add("@Fecha3", OdbcType.Date).Value = fechaCorte.Date;
                }
                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    recibo.IDHistRec = Convert.ToInt32(reader["P2444_ID_HIST_REC"]);
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getListaRecibosxRubro(string idInmobiliaria, string idConjunto, DateTime fechaCorte)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {

                string sql = "SELECT * FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION <= ? AND CAMPO4 = ? AND (P2406_STATUS = '1' OR (P2406_STATUS = '2' AND P2408_FECHA_PAGADO > ?) OR (P2406_STATUS = '3' AND P2408_FECHA_PAGADO > ?)) AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC in('R','X','T','Z','W','L') OR (P2426_TIPO_DOC = 'T' and T24_HISTORIA_RECIBOS.P2406_STATUS = '0')";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@Fecha", OdbcType.Date).Value = fechaCorte.Date;
                comando.Parameters.Add("@IDConj", OdbcType.VarChar).Value = idConjunto;
                comando.Parameters.Add("@Fecha2", OdbcType.Date).Value = fechaCorte.Date;
                comando.Parameters.Add("@Fecha3", OdbcType.Date).Value = fechaCorte.Date;
                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    recibo.IDHistRec = Convert.ToInt32(reader["P2444_ID_HIST_REC"]);
                    recibo.Numero = reader["P2403_NUM_RECIBO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P2403_NUM_RECIBO"]);
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    recibo.IDConjunto = reader["CAMPO4"].ToString();
                    recibo.NombreConjunto = reader["CAMPO5"].ToString();
                    recibo.TipoDoc = reader["P2426_TIPO_DOC"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.Estatus = reader["P2406_STATUS"].ToString();
                    recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    recibo.CantidadPorPagar = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"];
                    recibo.Periodo = reader["P2404_PERIODO"].ToString();
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"];
                    recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                    recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    recibo.TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 1 : (decimal)reader["P2414_TIPO_CAMBIO"];
                    recibo.TotalIVA = reader["P2416_IVA"] == DBNull.Value ? 1 : (decimal)reader["P2416_IVA"];
                    recibo.VencimientoPago = reader["CAMPO_DATE1"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CAMPO_DATE1"]);
                    recibo.Descuento = reader["P2424_DESCUENTO"] == DBNull.Value ? 0 : (decimal)reader["P2424_DESCUENTO"];
                    recibo.Inmueble = reader["CAMPO9"].ToString();
                    recibo.IDContrato = reader["P2418_ID_CONTRATO"].ToString();
                    recibo.MonedaPago = reader["P2420_MONEDA_PAGO"].ToString();
                    recibo.TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 1 : (decimal)reader["P2421_TC_PAGO"];
                    recibo.Campo20 = reader["CAMPO20"] == DBNull.Value ? string.Empty : (string)reader["CAMPO20"];
                    //Agregado 22/06/2015 by Ing. Rodrigo Uzcanga        
                    recibo.CtdPag = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"];
                    recibo.NombreCliente = reader["P2411_N_ARRENDATARIO"] == DBNull.Value ? string.Empty : reader["P2411_N_ARRENDATARIO"].ToString();
                    recibo.NombreCliente.Replace("CANCELADO", "");
                    recibo.cliente = SaariDB.getClienteByID(recibo.IDCliente);


                    recibo.Cargos = getCargosFacturadosxIdHistRec(recibo.IDHistRec);
                    if (recibo.Cargos != null)
                    {
                        if (recibo.Cargos.Count > 0)
                        {
                            foreach (ReciboEntity cargo in recibo.Cargos)
                            {
                                cargo.IDCliente = recibo.IDCliente;
                                cargo.IDConjunto = recibo.IDConjunto;

                                recibo.Total = recibo.Total - cargo.Total;
                                recibo.Importe = recibo.Importe - cargo.Importe;
                                recibo.TotalIVA = recibo.TotalIVA - cargo.TotalIVA;

                                listaRecibos.Add(cargo);
                            }
                        }

                    }

                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        #endregion 30 60 90 por rubro



        public static List<ReciboEntity> getListaRecibos(string idInmobiliaria, string idConjunto, DateTime fechaCorte)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                //string sql = "SELECT * FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND P2409_FECHA_EMISION <= ? AND CAMPO4 = ? AND (P2406_STATUS = '1' OR (P2406_STATUS = '2' AND P2408_FECHA_PAGADO > ?) OR (P2406_STATUS = '3' AND P2408_FECHA_PAGADO > ?)) AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC in('R','X','T','Z','W','L') OR (P2426_TIPO_DOC = 'T' and T24_HISTORIA_RECIBOS.P2406_STATUS = '0')";
                string sql = @"SELECT * FROM  
T24_HISTORIA_RECIBOS  
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC 
LEFT JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P18_CAMPO4 =   T24_HISTORIA_RECIBOS.CAMPO3 
WHERE P2401_ID_ARRENDADORA = ? 
AND P2409_FECHA_EMISION <= ? 
AND CAMPO4 = ?
AND (P2406_STATUS = '1' OR (P2406_STATUS = '2'  
AND P2408_FECHA_PAGADO >  ? ) OR (P2406_STATUS = '3'  
AND P2408_FECHA_PAGADO >  ? ))  
AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC in('R','X','T','Z','W','L') OR (P2426_TIPO_DOC = 'T'  
and T24_HISTORIA_RECIBOS.P2406_STATUS = '0')";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@Fecha", OdbcType.Date).Value = fechaCorte.Date;
                comando.Parameters.Add("@IDConj", OdbcType.VarChar).Value = idConjunto;
                comando.Parameters.Add("@Fecha2", OdbcType.Date).Value = fechaCorte.Date;
                comando.Parameters.Add("@Fecha3", OdbcType.Date).Value = fechaCorte.Date;
                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    recibo.IDHistRec = Convert.ToInt32(reader["P2444_ID_HIST_REC"]);
                    recibo.Numero = reader["P2403_NUM_RECIBO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P2403_NUM_RECIBO"]);
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    recibo.IDConjunto = reader["CAMPO4"].ToString();
                    recibo.IdSubconjunto = reader["P1801_ID_SUBCONJUNTO"].ToString();
                    recibo.IdCentroSubConjunto = reader["P1804_ID_CONJUNTO"].ToString();
                    recibo.NombreSubconjunto = reader["P1803_NOMBRE"].ToString();
                    recibo.NombreConjunto = reader["CAMPO5"].ToString();
                    recibo.TipoDoc = reader["P2426_TIPO_DOC"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.Estatus = reader["P2406_STATUS"].ToString();
                    recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    recibo.CantidadPorPagar = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"];
                    recibo.Periodo = reader["P2404_PERIODO"].ToString();
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"];
                    recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                    recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    recibo.TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 1 : (decimal)reader["P2414_TIPO_CAMBIO"];
                    recibo.TotalIVA = reader["P2416_IVA"] == DBNull.Value ? 1 : (decimal)reader["P2416_IVA"];
                    recibo.VencimientoPago = reader["CAMPO_DATE1"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CAMPO_DATE1"]);
                    recibo.Descuento = reader["P2424_DESCUENTO"] == DBNull.Value ? 0 : (decimal)reader["P2424_DESCUENTO"];
                    recibo.Inmueble = reader["CAMPO9"].ToString();
                    recibo.IDContrato = reader["P2418_ID_CONTRATO"].ToString();
                    recibo.MonedaPago = reader["P2420_MONEDA_PAGO"].ToString();
                    recibo.TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 1 : (decimal)reader["P2421_TC_PAGO"];
                    //Agregado 22/06/2015 by Ing. Rodrigo Uzcanga
                    recibo.CtdPag = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"];
                    
                    #region Sub Conjunto                    

                    if (recibo.IdSubconjunto.Contains("SCNJ"))
                    {
                        recibo.Inmueble = recibo.NombreSubconjunto;
                        recibo.NombreConjunto = getNombreConjunto(recibo.IdCentroSubConjunto);
                    }
                    #endregion
                    if (string.IsNullOrEmpty(recibo.NombreConjunto) && !string.IsNullOrEmpty(recibo.IDConjunto))
                    {
                        recibo.NombreConjunto = getNombreConjunto(recibo.IDConjunto);
                        if (string.IsNullOrEmpty(recibo.NombreConjunto))
                            recibo.NombreConjunto = getNombreConjunto(recibo.IdCentroSubConjunto);
                    }
                    if (string.IsNullOrEmpty(recibo.Inmueble) && string.IsNullOrEmpty(recibo.NombreConjunto))
                    {
                        recibo.IdSubconjunto = getIdEdificio(recibo.IDContrato);
                        if (!string.IsNullOrEmpty(recibo.IdSubconjunto))
                        {
                            if (recibo.IdSubconjunto.Contains("SCNJ"))
                            {
                                recibo.Inmueble = getNombreSubConjunto(recibo.IdSubconjunto);
                                recibo.IdCentroSubConjunto = getIdCentro(recibo.IdSubconjunto);
                                recibo.NombreConjunto = getNombreConjunto(recibo.IdCentroSubConjunto);
                            }
                        }
                    }
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ClienteEntity> getClienteByID()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0203_NOMBRE, P0253_NOMBRE_COMERCIAL, P0204_RFC FROM T02_ARRENDATARIO";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                List<ClienteEntity> listClientes = new List<ClienteEntity>();
                ClienteEntity cliente = null;
                while (reader.Read())
                {
                    cliente = new ClienteEntity();
                    cliente.Nombre = reader["P0203_NOMBRE"].ToString();
                    cliente.NombreComercial = reader["P0253_NOMBRE_COMERCIAL"].ToString();
                    cliente.RFC = reader["P0204_RFC"].ToString();
                    listClientes.Add(cliente);
                }
                reader.Close();
                conexion.Close();
                return listClientes;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }


        public static ClienteEntity getClienteByID(string idCliente)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0201_ID, P0203_NOMBRE, P0253_NOMBRE_COMERCIAL, P0204_RFC FROM T02_ARRENDATARIO WHERE P0201_ID = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = idCliente;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                ClienteEntity cliente = null;
                while (reader.Read())
                {
                    cliente = new ClienteEntity();
                    cliente.IDCliente = reader["P0201_ID"].ToString();
                    cliente.Nombre = reader["P0203_NOMBRE"].ToString();
                    cliente.NombreComercial = reader["P0253_NOMBRE_COMERCIAL"].ToString();
                    cliente.RFC = reader["P0204_RFC"].ToString();
                    cliente.Contacto = getContactoCliente(idCliente);

                    break;
                }
                reader.Close();
                conexion.Close();
                return cliente;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static string getNombreInmobiliariaByID(string idInmo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0103_RAZON_SOCIAL FROM T01_ARRENDADORA WHERE P0101_ID_ARR = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmo;
                conexion.Open();
                string result = comando.ExecuteScalar().ToString();
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }

        public static string getNombreConjuntoByID(string idConj)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0303_NOMBRE FROM T03_CENTRO_INDUSTRIAL WHERE P0301_ID_CENTRO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDConj", OdbcType.VarChar).Value = idConj;
                conexion.Open();
                string result = comando.ExecuteScalar().ToString();
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }

        public static string getNombreSubConjuntoByID(string idinmueble)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P1803_NOMBRE FROM T18_SUBCONJUNTOS 
                               JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T18_SUBCONJUNTOS.P18_CAMPO4            
                               WHERE T18_SUBCONJUNTOS.P18_CAMPO4 = ?
";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idinmueble", OdbcType.VarChar).Value = idinmueble;
                conexion.Open();
                string result = comando.ExecuteScalar().ToString();
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }
        public static decimal getSumaDeNotaCredOPagoParcial(int idHist, DateTime fechaCorte, bool incluirIVA)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                if (incluirIVA)
                    sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2419_TOTAL * P2414_TIPO_CAMBIO) ELSE P2419_TOTAL END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                else
                    sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2405_IMPORTE * P2414_TIPO_CAMBIO) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@fecha", OdbcType.Date).Value = fechaCorte;
                comando.Parameters.Add("@idhist1", OdbcType.Int).Value = idHist;
                comando.Parameters.Add("@idhist2", OdbcType.Int).Value = idHist;
                conexion.Open();
                decimal suma = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return suma;
            }
            catch (Exception)
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getSumaDeNotaCredOPagoParcial(int idHist, DateTime fechaCorte, bool incluirIVA, string monedaContrato, string monedaRecibo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                if (monedaContrato == monedaRecibo)
                {
                    if (incluirIVA)
                        sql = @"SELECT SUM(P2419_TOTAL) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                    else
                        sql = @"SELECT SUM(P2405_IMPORTE) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                }
                else
                {
                    if (monedaContrato == "P")
                    {
                        if (incluirIVA)
                        {
                            sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN P2419_TOTAL ELSE (P2419_TOTAL * P2414_TIPO_CAMBIO) END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                        }
                        else
                        {
                            sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN P2405_IMPORTE ELSE (P2405_IMPORTE * P2414_TIPO_CAMBIO) END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                        }
                    }
                    else
                    {
                        if (incluirIVA)
                        {
                            sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2419_TOTAL / P2414_TIPO_CAMBIO) ELSE P2419_TOTAL END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                        }
                        else
                        {
                            sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2405_IMPORTE / P2414_TIPO_CAMBIO) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                        }
                    }
                }
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@fecha", OdbcType.Date).Value = fechaCorte;
                comando.Parameters.Add("@idhist1", OdbcType.Int).Value = idHist;
                comando.Parameters.Add("@idhist2", OdbcType.Int).Value = idHist;
                conexion.Open();
                decimal suma = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return suma;
            }
            catch (Exception)
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getSumaDeNotaCredOPagoParcial(int idHist, DateTime fechaCorte, string monedaContrato, bool incluirIVA)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                decimal suma = 0;
                //                string sql = @"SELECT P2419_TOTAL, P2405_IMPORTE, P2420_MONEDA_PAGO, P2414_TIPO_CAMBIO, P2420_MONEDA_PAGO, P2421_TC_PAGO,
                //                P2425_DEB_REC, P2426_TIPO_DOC, P2406_STATUS, P2403_NUM_RECIBO FROM T24_HISTORIA_RECIBOS
                //                WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
                //                OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                string sql = @"SELECT P2419_TOTAL, P2405_IMPORTE, P2420_MONEDA_PAGO, P2414_TIPO_CAMBIO, P2420_MONEDA_PAGO, P2421_TC_PAGO,
                P2425_DEB_REC, P2426_TIPO_DOC, P2406_STATUS, P2403_NUM_RECIBO FROM T24_HISTORIA_RECIBOS
                WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
                OR (P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@fecha", OdbcType.Date).Value = fechaCorte;
                comando.Parameters.Add("@idhist1", OdbcType.Int).Value = idHist;
                comando.Parameters.Add("@idhist2", OdbcType.Int).Value = idHist;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    string monedaPagoRecibo = reader["P2420_MONEDA_PAGO"].ToString();
                    if (monedaContrato == monedaPagoRecibo)
                    {
                        if (incluirIVA)
                        {
                            suma += (decimal)reader["P2419_TOTAL"];
                        }
                        else
                        {
                            suma += (decimal)reader["P2405_IMPORTE"];
                        }
                    }
                    else
                    {
                        if (monedaPagoRecibo == "P")
                        {
                            if (incluirIVA)
                            {
                                suma += (decimal)reader["P2419_TOTAL"] / (decimal)reader["P2421_TC_PAGO"];
                            }
                            else
                            {
                                suma += (decimal)reader["P2405_IMPORTE"] / (decimal)reader["P2421_TC_PAGO"];
                            }
                        }
                        else
                        {
                            if (incluirIVA)
                            {
                                suma += (decimal)reader["P2419_TOTAL"] * (decimal)reader["P2421_TC_PAGO"];
                            }
                            else
                            {
                                suma += (decimal)reader["P2405_IMPORTE"] * (decimal)reader["P2421_TC_PAGO"];
                            }
                        }
                    }
                }
                reader.Close();
                conexion.Close();
                return suma;
            }
            catch (Exception)
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getSumaDeNotaCredOPagoParcial(int numeroRecibo, DateTime fechaCorte, bool incluirIVA, string idArrendadora)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                if (incluirIVA)
                    sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2419_TOTAL * P2414_TIPO_CAMBIO) ELSE P2419_TOTAL END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                else
                    sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2405_IMPORTE * P2414_TIPO_CAMBIO) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@fecha", OdbcType.Date).Value = fechaCorte;
                comando.Parameters.Add("@idarr", OdbcType.VarChar).Value = idArrendadora;
                comando.Parameters.Add("@idhist1", OdbcType.Int).Value = numeroRecibo;
                comando.Parameters.Add("@idhist2", OdbcType.Int).Value = numeroRecibo;
                conexion.Open();
                decimal suma = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return suma;
            }
            catch (Exception)
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getSumaDeNotaCredOPagoParcial(int numeroRecibo, DateTime fechaCorte, bool incluirIVA, string idArrendadora, string monedaContrato, string monedaRecibo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                if (monedaContrato == monedaRecibo)
                {
                    if (incluirIVA)
                        sql = @"SELECT SUM(P2419_TOTAL) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                    else
                        sql = @"SELECT SUM(P2405_IMPORTE) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                }
                else
                {
                    if (monedaContrato == "P")
                    {
                        if (incluirIVA)
                            sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN P2419_TOTAL ELSE (P2419_TOTAL * P2414_TIPO_CAMBIO) END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                        else
                            sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN P2405_IMPORTE ELSE (P2405_IMPORTE * P2414_TIPO_CAMBIO) END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                    }
                    else
                    {
                        if (incluirIVA)
                            sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2419_TOTAL / P2414_TIPO_CAMBIO) ELSE P2419_TOTAL END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                        else
                            sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2405_IMPORTE / P2414_TIPO_CAMBIO) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                    }
                }
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@fecha", OdbcType.Date).Value = fechaCorte;
                comando.Parameters.Add("@idarr", OdbcType.VarChar).Value = idArrendadora;
                comando.Parameters.Add("@idhist1", OdbcType.Int).Value = numeroRecibo;
                comando.Parameters.Add("@idhist2", OdbcType.Int).Value = numeroRecibo;
                conexion.Open();
                decimal suma = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return suma;
            }
            catch (Exception)
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getSumaDeNotaCredOPagoParcial(int idHist, DateTime fechaCorte, bool incluirIVA, decimal tipoCambio)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                if (incluirIVA)
                    sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2419_TOTAL * {0}) ELSE P2419_TOTAL END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio);
                else
                    sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2405_IMPORTE * {0}) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio);
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@fecha", OdbcType.Date).Value = fechaCorte;
                comando.Parameters.Add("@idhist1", OdbcType.Int).Value = idHist;
                comando.Parameters.Add("@idhist2", OdbcType.Int).Value = idHist;
                conexion.Open();
                decimal suma = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return suma;
            }
            catch (Exception)
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getSumaDeNotaCredOPagoParcial(int idHist, DateTime fechaCorte, bool incluirIVA, decimal tipoCambio, string monedaContrato, string monedaRecibo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;

                if (monedaContrato == monedaRecibo)
                {
                    if (incluirIVA)
                    {
                        //AGREGAR CONDICION CUANDO monedaRecibo sea == "P"
                        //APLICA PARA CARTERAVENCIDDA306090 POR CONTRATO AL TC DEL CORTE 
                        sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2419_TOTAL / P2421_TC_PAGO) ELSE P2419_TOTAL END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                    }
                    else
                    {
                        //sql = @"SELECT SUM(P2405_IMPORTE) FROM T24_HISTORIA_RECIBOS
                        //WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
                        //OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                        if (monedaContrato == "D")
                            sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO='P' THEN (P2405_IMPORTE / P2421_TC_PAGO) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
                            WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
                            OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                        else
                            sql = @"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO='D' THEN (P2405_IMPORTE * P2421_TC_PAGO) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
                            WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
                            OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                    }
                }
                else
                {
                    if (monedaContrato == "P")
                    {
                        if (incluirIVA)
                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN P2419_TOTAL ELSE (P2419_TOTAL * P2421_TC_PAGO) END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ");
                        //                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN P2419_TOTAL ELSE (P2419_TOTAL * {0}) END) FROM T24_HISTORIA_RECIBOS
                        //WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
                        //OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio);
                        else
                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN P2405_IMPORTE ELSE (P2405_IMPORTE * P2421_TC_PAGO) END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ");
                        //                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN P2405_IMPORTE ELSE (P2405_IMPORTE * {0}) END) FROM T24_HISTORIA_RECIBOS
                        //WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
                        //OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio);
                    }
                    else
                    {
                        if (incluirIVA)
                            //                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2419_TOTAL / {0}) ELSE P2419_TOTAL END) FROM T24_HISTORIA_RECIBOS
                            //WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
                            //OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio == 0 ? 1 : tipoCambio);
                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2419_TOTAL / P2421_TC_PAGO) ELSE P2419_TOTAL END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ");
                        else
                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2405_IMPORTE / P2421_TC_PAGO) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ");
                        //                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2405_IMPORTE / {0}) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
                        //WHERE P2408_FECHA_PAGADO <= ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
                        //OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio == 0 ? 1 : tipoCambio);

                        /*if (incluirIVA)
                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2419_TOTAL / {0}) ELSE P2419_TOTAL END) FROM T24_HISTORIA_RECIBOS
        WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
        OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio == 0 ? 1 : tipoCambio);
                        else
                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2405_IMPORTE / {0}) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
        WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
        OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio == 0 ? 1 : tipoCambio);*/
                    }
                }
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@fecha", OdbcType.Date).Value = fechaCorte;
                comando.Parameters.Add("@idhist1", OdbcType.Int).Value = idHist;
                comando.Parameters.Add("@idhist2", OdbcType.Int).Value = idHist;

                conexion.Open();
                decimal suma = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return suma;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getSumaDeNotaCredOPagoParcial(int numeroRecibo, DateTime fechaCorte, bool incluirIVA, string idArrendadora, decimal tipoCambio)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                if (incluirIVA)
                    sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2419_TOTAL * {0}) ELSE P2419_TOTAL END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio);
                else
                    sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2405_IMPORTE * {0}) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio);
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@fecha", OdbcType.Date).Value = fechaCorte;
                comando.Parameters.Add("@idarr", OdbcType.VarChar).Value = idArrendadora;
                comando.Parameters.Add("@idhist1", OdbcType.Int).Value = numeroRecibo;
                comando.Parameters.Add("@idhist2", OdbcType.Int).Value = numeroRecibo;
                conexion.Open();
                decimal suma = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return suma;
            }
            catch (Exception)
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getSumaDeNotaCredOPagoParcial(int numeroRecibo, DateTime fechaCorte, bool incluirIVA, string idArrendadora, decimal tipoCambio, string monedaContrato, string monedaRecibo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                /*if (incluirIVA)
                    sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2419_TOTAL * {0}) ELSE P2419_TOTAL END) FROM T24_HISTORIA_RECIBOS
        WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
        OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio);
                else
                    sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2405_IMPORTE * {0}) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
        WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
        OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio);*/
                if (monedaContrato == monedaRecibo)
                {
                    if (incluirIVA)
                        sql = @"SELECT SUM(P2419_TOTAL) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                    else
                        sql = @"SELECT SUM(P2405_IMPORTE) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ";
                }
                else
                {
                    if (monedaContrato == "P")
                    {
                        if (incluirIVA)
                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN P2419_TOTAL ELSE (P2419_TOTAL * P2421_TC_PAGO) END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ");
                        //                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN P2419_TOTAL ELSE (P2419_TOTAL * {0}) END) FROM T24_HISTORIA_RECIBOS
                        //WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
                        //OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio);
                        else
                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN P2405_IMPORTE ELSE (P2405_IMPORTE * P2421_TC_PAGO) END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ");
                        //                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN P2405_IMPORTE ELSE (P2405_IMPORTE * {0}) END) FROM T24_HISTORIA_RECIBOS
                        //WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
                        //OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio);
                    }
                    else
                    {
                        if (incluirIVA)
                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2419_TOTAL / P2421_TC_PAGO) ELSE P2419_TOTAL END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ");
                        //                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2419_TOTAL / {0}) ELSE P2419_TOTAL END) FROM T24_HISTORIA_RECIBOS
                        //WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
                        //OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio == 0 ? 1 : tipoCambio);
                        else
                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2405_IMPORTE / P2421_TC_PAGO) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ");
                        //                            sql = string.Format(@"SELECT SUM(CASE WHEN P2420_MONEDA_PAGO = 'P' THEN (P2405_IMPORTE / {0}) ELSE P2405_IMPORTE END) FROM T24_HISTORIA_RECIBOS
                        //WHERE P2408_FECHA_PAGADO <= ? AND P2401_ID_ARRENDADORA = ? AND ((P2425_DEB_REC = ? AND P2426_TIPO_DOC = 'B' AND  P2406_STATUS <> '3' ) 
                        //OR ( P2403_NUM_RECIBO = ? AND P2426_TIPO_DOC IN ('P','U'))) ", tipoCambio == 0 ? 1 : tipoCambio);
                    }
                }
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@fecha", OdbcType.Date).Value = fechaCorte;
                comando.Parameters.Add("@idarr", OdbcType.VarChar).Value = idArrendadora;
                comando.Parameters.Add("@idhist1", OdbcType.Int).Value = numeroRecibo;
                comando.Parameters.Add("@idhist2", OdbcType.Int).Value = numeroRecibo;

                conexion.Open();
                decimal suma = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return suma;
            }
            catch (Exception)
            {
                conexion.Close();
                return 0;
            }
        }

        public static EdificioEntity getInmuebleByID(string idInmueble)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0703_NOMBRE, P0726_CLAVE FROM T07_EDIFICIO WHERE P0701_ID_EDIFICIO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInm", OdbcType.VarChar).Value = idInmueble;
                EdificioEntity inmueble = null;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    inmueble = new EdificioEntity()
                    {
                        ID = idInmueble,
                        Nombre = reader["P0703_NOMBRE"].ToString(),
                        Identificador = reader["P0726_CLAVE"].ToString()
                    };
                    break;
                }
                reader.Close();
                conexion.Close();
                return inmueble;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<EdificioEntity> getInmuebleByIDArrAndIdConjunto(string idArrendadora, string idConjunto)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;


                if (idConjunto == "Todos")
                {
                    sql = "SELECT P0701_ID_EDIFICIO,P0703_NOMBRE, P0726_CLAVE FROM T07_EDIFICIO WHERE P0709_ARRENDADORA = ?";

                }
                else
                {
                    sql = "SELECT P0701_ID_EDIFICIO,P0703_NOMBRE, P0726_CLAVE FROM T07_EDIFICIO WHERE P0709_ARRENDADORA = ? AND P0710_ID_CENTRO = ?";
                }
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                if (idConjunto == "Todos")
                {
                    comando.Parameters.Add("@IDInm", OdbcType.VarChar).Value = idArrendadora;
                }
                else
                {
                    comando.Parameters.Add("@IDInm", OdbcType.VarChar).Value = idArrendadora;
                    comando.Parameters.Add("@IDConju", OdbcType.VarChar).Value = idConjunto;
                }
                List<EdificioEntity> ListaEdificio = new List<EdificioEntity>();

                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    EdificioEntity inmueble = new EdificioEntity()
                    {

                        ID = reader["P0701_ID_EDIFICIO"].ToString(),
                        Nombre = reader["P0703_NOMBRE"].ToString(),
                        Identificador = reader["P0726_CLAVE"].ToString(),
                    };
                    ListaEdificio.Add(inmueble);

                }
                reader.Close();
                conexion.Close();
                return ListaEdificio;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static EdificioEntity getSubconjuntoByID(string idInmueble)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P1803_NOMBRE, P18_CAMPO1 FROM T18_SUBCONJUNTOS WHERE P1801_ID_SUBCONJUNTO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInm", OdbcType.VarChar).Value = idInmueble;
                EdificioEntity inmueble = null;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    inmueble = new EdificioEntity()
                    {
                        ID = idInmueble,
                        Nombre = reader["P1803_NOMBRE"].ToString(),
                        Identificador = reader["P18_CAMPO1"].ToString()
                    };
                    break;
                }
                reader.Close();
                conexion.Close();
                return inmueble;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static ConjuntoEntity getConjuntoByID(string idConjunto)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0303_NOMBRE FROM T03_CENTRO_INDUSTRIAL WHERE P0301_ID_CENTRO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idConjunto", OdbcType.VarChar).Value = idConjunto;
                ConjuntoEntity conj = null;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    conj = new ConjuntoEntity()
                    {
                        ID = idConjunto,
                        Nombre = reader["P0303_NOMBRE"].ToString()
                    };
                    break;
                }
                reader.Close();
                conexion.Close();
                return conj;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        public static string getNombreSubConjunto(string idSubConjunto)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P1803_NOMBRE  FROM T18_SUBCONJUNTOS WHERE P1801_ID_SUBCONJUNTO= ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idConjunto", OdbcType.VarChar).Value = idSubConjunto;
                conexion.Open();
                string nombre = comando.ExecuteScalar().ToString();
                conexion.Close();
                return nombre;
            }
            catch(Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        public static string getIdEdificio(string id)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0404_ID_EDIFICIO FROM T04_CONTRATO WHERE P0401_ID_CONTRATO= ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idEdificio", OdbcType.VarChar).Value = id;
                conexion.Open();
                string nombre = comando.ExecuteScalar().ToString();
                conexion.Close();
                return nombre;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        public static string getIdCentro(string id)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P1804_ID_CONJUNTO FROM T18_SUBCONJUNTOS WHERE P1801_ID_SUBCONJUNTO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@id", OdbcType.VarChar).Value = id;
                conexion.Open();
                string nombre = comando.ExecuteScalar().ToString();
                conexion.Close();
                return nombre;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        public static string getNombreConjunto(string id)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0303_NOMBRE FROM T03_CENTRO_INDUSTRIAL WHERE P0301_ID_CENTRO= ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@id", OdbcType.VarChar).Value = id;
                conexion.Open();
                string nombre = comando.ExecuteScalar().ToString();
                conexion.Close();
                return nombre;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        public static decimal getTipoCambio(DateTime fecha)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P3904_VALOR_INDEX FROM T39_INDICES_ECONOMICOS WHERE P3901_TIPO_INDEX = 'TIPOCAMDLS' AND P3902_MES_INDEX = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@Fecha", OdbcType.Date).Value = fecha.Date;
                conexion.Open();
                decimal tipoCambio = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return tipoCambio;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        public static string getMonedaContrato(string idContrato)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = "SELECT T04_CONTRATO.P0407_MONEDA_FACT FROM T04_CONTRATO WHERE P0401_ID_CONTRATO = ?";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        conexion.Open();
                        string result = comando.ExecuteScalar().ToString();
                        return result;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static ContratoEntity getContrato(string idContrato)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = @"SELECT T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.P0402_ID_ARRENDAT, T04_CONTRATO.P0407_MONEDA_FACT, T02_ARRENDATARIO.P0203_NOMBRE, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL 
FROM T04_CONTRATO
JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T04_CONTRATO.P0402_ID_ARRENDAT
WHERE T04_CONTRATO.P0401_ID_CONTRATO = ?";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = idContrato;
                        conexion.Open();
                        ContratoEntity contrato = null;
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                contrato = new ContratoEntity();
                                contrato.ID = reader["P0401_ID_CONTRATO"].ToString();
                                contrato.Moneda = reader["P0407_MONEDA_FACT"].ToString();
                                contrato.Cliente = new ClienteEntity()
                                {
                                    IDCliente = reader["P0402_ID_ARRENDAT"].ToString(),
                                    Nombre = reader["P0203_NOMBRE"].ToString(),
                                    NombreComercial = reader["P0253_NOMBRE_COMERCIAL"].ToString(),
                                    Contacto = getContactoCliente(reader["P0402_ID_ARRENDAT"].ToString())

                                };

                            }
                        }
                        return contrato;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static List<EstatusContratoEntity> getEstatusContrato()
        {
            List<EstatusContratoEntity> listaEstatus = new List<EstatusContratoEntity>() { new EstatusContratoEntity() { Estatus = "*Todos" } };
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = "SELECT DESCR_CAT FROM CATEGORIA WHERE ID_COT = 'STATCONTR' ORDER BY DESCR_CAT";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                                listaEstatus.Add(new EstatusContratoEntity() { Estatus = reader["DESCR_CAT"].ToString() });
                        }
                    }
                }
            }
            catch
            {

            }
            return listaEstatus;
        }

        public static List<CarteraVencidaVentaEntity> getDatosCarteraVenta(string idArrendadora, string conjunto, string estatus)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = @"SELECT T04_CONTRATO.P0401_ID_CONTRATO AS ID_CONTRATO, T24_HISTORIA_RECIBOS.CAMPO5 AS FRACCIONAMIENTO, T24_HISTORIA_RECIBOS.P2411_N_ARRENDATARIO AS CLIENTE, 
                        T07_EDIFICIO.CAMPO5 AS MANZANA, T07_EDIFICIO.CAMPO6 AS LOTE, T24_HISTORIA_RECIBOS.CAMPO6 AS TOTAL_PAGARES, T24_HISTORIA_RECIBOS.P2419_TOTAL AS TOTAL, 
                        T24_HISTORIA_RECIBOS.P2413_PAGO AS TOTAL_PAGO, T24_HISTORIA_RECIBOS.CAMPO_DATE1 AS VENCIMIENTO, T04_CONTRATO.CAMPO_NUM4 AS IMPORTE_MENSUAL,
                        T24_HISTORIA_RECIBOS.P2406_STATUS AS STATUS, T24_HISTORIA_RECIBOS.P2404_PERIODO AS PERIODO, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO AS FECHA_PAGO, 
                        T04_CONTRATO.CAMPO10 AS OBSERVACIONES, T31_CONFIGURACION.CAMPO7 AS ESTATUS FROM T24_HISTORIA_RECIBOS 
                        LEFT JOIN T04_CONTRATO ON T04_CONTRATO.P0401_ID_CONTRATO = T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO 
                        LEFT JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T24_HISTORIA_RECIBOS.CAMPO3 
                        LEFT JOIN T31_CONFIGURACION ON T31_CONFIGURACION.P3102_VALOR = T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO 
                        WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ? AND (T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'V' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'Z' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'E') AND T04_CONTRATO.P0401_ID_CONTRATO is not null ";
                    if (conjunto != "Todos")
                        sql += " AND T24_HISTORIA_RECIBOS.CAMPO4 = ? ";
                    if (estatus != "*Todos")
                        sql += " AND T31_CONFIGURACION.CAMPO7 = ? ";

                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@arrendadora", OdbcType.NVarChar).Value = idArrendadora;
                        if (conjunto != "Todos")
                            comando.Parameters.Add("@conjunto", OdbcType.NVarChar).Value = conjunto;
                        if (estatus != "*Todos")
                            comando.Parameters.Add("@estatus", OdbcType.NVarChar).Value = estatus;
                        List<CarteraVencidaVentaEntity> listaCartera = new List<CarteraVencidaVentaEntity>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CarteraVencidaVentaEntity cartera = new CarteraVencidaVentaEntity();
                                cartera.IDContrato = reader["ID_CONTRATO"].ToString();
                                cartera.Fraccionamiento = reader["FRACCIONAMIENTO"].ToString();
                                cartera.Cliente = reader["CLIENTE"].ToString();
                                cartera.Manzana = reader["MANZANA"].ToString();
                                cartera.Lote = reader["LOTE"].ToString();
                                cartera.TotalPagares = reader["TOTAL_PAGARES"].ToString();
                                cartera.Total = reader["TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL"]);
                                cartera.TotalPago = reader["TOTAL_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL_PAGO"]);
                                cartera.FechaVencimiento = reader["VENCIMIENTO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VENCIMIENTO"]);
                                cartera.ImporteMensual = reader["IMPORTE_MENSUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["IMPORTE_MENSUAL"]);
                                cartera.EstatusFactura = reader["STATUS"].ToString();
                                cartera.Periodo = reader["PERIODO"].ToString();
                                cartera.FechaPagado = reader["FECHA_PAGO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FECHA_PAGO"]);
                                cartera.Observaciones = reader["OBSERVACIONES"].ToString();
                                cartera.Estatus = reader["ESTATUS"].ToString();
                                listaCartera.Add(cartera);
                            }
                        }
                        return listaCartera;
                    }
                }
            }
            catch
            {
                return new List<CarteraVencidaVentaEntity>();
            }
        }

        public static List<string> getUsuarios()
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = "SELECT USUARIO FROM GRUPOS ORDER BY USUARIO";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        List<string> usuarios = new List<string>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                                usuarios.Add(reader["USUARIO"].ToString());
                        }
                        return usuarios;
                    }
                }
            }
            catch
            {
                return new List<string>();
            }
        }

        public static List<InmobiliariaEntity> getInmobiliariasPorUsuarios(string usuario)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, P0102_N_COMERCIAL, P4702_ID_USUARIO, T01_ARRENDADORA.P0122_CAMPO15 FROM T47_EMPRESAS_US 
JOIN T01_ARRENDADORA on T01_ARRENDADORA.P0101_ID_ARR = P4701_ID_EMPRESA
WHERE P4702_ID_USUARIO = ? ";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@usuario", OdbcType.VarChar).Value = usuario;
                List<InmobiliariaEntity> listaInmobiliarias = new List<InmobiliariaEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    InmobiliariaEntity inmo = new InmobiliariaEntity()
                    {
                        ID = reader["P0101_ID_ARR"].ToString(),
                        RazonSocial = reader["P0103_RAZON_SOCIAL"].ToString(),
                        NombreComercial = reader["P0102_N_COMERCIAL"].ToString(),
                        IdArrendadora = reader["P0122_CAMPO15"].ToString()
                    };
                    listaInmobiliarias.Add(inmo);

                }
                reader.Close();
                conexion.Close();
                return listaInmobiliarias;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<InmobiliariaEntity> getInmobiliariasPorUsuariosxContribuyente(string usuario)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT P4202_RAZON_SOCIAL, P4203_RFC FROM T42_CONTRIBUYENTES
JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0122_CAMPO15 =T42_CONTRIBUYENTES.P4201_ID_CONTRIBUYENTE 
JOIN T47_EMPRESAS_US ON  T47_EMPRESAS_US.P4701_ID_EMPRESA =T01_ARRENDADORA.P0101_ID_ARR 
WHERE P4702_ID_USUARIO = ? ";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@usuario", OdbcType.VarChar).Value = usuario;
                List<InmobiliariaEntity> listaInmobiliarias = new List<InmobiliariaEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    InmobiliariaEntity inmo = new InmobiliariaEntity()
                    {
                        //ID = reader["P0101_ID_ARR"].ToString(),
                        RazonSocial = reader["P4202_RAZON_SOCIAL"].ToString(),
                        RFC = reader["P4203_RFC"].ToString()
                        // NombreComercial = reader["P0102_N_COMERCIAL"].ToString()
                    };
                    listaInmobiliarias.Add(inmo);

                }
                reader.Close();
                conexion.Close();
                return listaInmobiliarias;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<InmobiliariaEntity> getInmobiliariasPorUsuarios(string usuario, string idGrupoEmp, bool porGrupo)
        {
            string sql = string.Empty;
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            OdbcCommand comando = new OdbcCommand(sql, conexion);
            if (idGrupoEmp != "Todos" && porGrupo == false)
            {
                sql = @"SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, P0102_N_COMERCIAL, P4702_ID_USUARIO FROM T47_EMPRESAS_US
JOIN T01_ARRENDADORA on T01_ARRENDADORA.P0101_ID_ARR = P4701_ID_EMPRESA
WHERE P4702_ID_USUARIO = ? and  P0151_ID_GRUPO_EMP =  ? ";
                comando.Parameters.Add("@usuario", OdbcType.VarChar).Value = usuario;
                comando.Parameters.Add("@GrupoEmp", OdbcType.VarChar).Value = idGrupoEmp;
            }
            else if (idGrupoEmp == "Todos" && porGrupo == false)
            {
                sql = @"SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, P0102_N_COMERCIAL, P4702_ID_USUARIO FROM T47_EMPRESAS_US
JOIN T01_ARRENDADORA on T01_ARRENDADORA.P0101_ID_ARR = P4701_ID_EMPRESA
WHERE P4702_ID_USUARIO = ?";
                comando.Parameters.Add("@usuario", OdbcType.VarChar).Value = usuario;
            }
            else if (idGrupoEmp != "Todos" && porGrupo == true)
            {
                sql = @"SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, P0102_N_COMERCIAL, P4702_ID_USUARIO FROM T47_EMPRESAS_US
JOIN T01_ARRENDADORA on T01_ARRENDADORA.P0101_ID_ARR = P4701_ID_EMPRESA
WHERE P0151_ID_GRUPO_EMP =  ? ";
                comando.Parameters.Add("@GrupoEmp", OdbcType.VarChar).Value = idGrupoEmp;
            }
            else
            {
                sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, P0102_N_COMERCIAL FROM T01_ARRENDADORA ORDER BY P0103_RAZON_SOCIAL";
            }

            try
            {

                List<InmobiliariaEntity> listaInmobiliarias = new List<InmobiliariaEntity>();
                InmobiliariaEntity inmo = new InmobiliariaEntity();
                inmo.ID = "Todos";
                inmo.RazonSocial = "Todos";
                inmo.NombreComercial = "*Todos";
                listaInmobiliarias.Add(inmo);
                conexion.Open();
                comando.CommandText = sql;
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    InmobiliariaEntity i = new InmobiliariaEntity()
                    {
                        ID = reader["P0101_ID_ARR"].ToString(),
                        RazonSocial = reader["P0103_RAZON_SOCIAL"].ToString(),
                        NombreComercial = reader["P0102_N_COMERCIAL"].ToString()
                    };
                    listaInmobiliarias.Add(i);

                }
                reader.Close();
                conexion.Close();
                return listaInmobiliarias;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }


        public static List<EdificioEntity> getInmuebles()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = "Select P0701_ID_EDIFICIO, P0703_NOMBRE from T07_EDIFICIO ORDER BY P0703_NOMBRE ";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                List<EdificioEntity> listaInmuebles = new List<EdificioEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    EdificioEntity inmu = new EdificioEntity()
                    {
                        ID = reader["P0701_ID_EDIFICIO"].ToString(),
                        Nombre = reader["P0703_NOMBRE"].ToString()
                    };
                    listaInmuebles.Add(inmu);

                }
                reader.Close();
                conexion.Close();
                return listaInmuebles;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ConjuntoInmobiliaria> GetConjuntoInmobiliarias(string idArr, string idConjunto, DateTime fechaIni, DateTime fechaFin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            //            string sql = @"SELECT P0401_ID_CONTRATO, P0402_ID_ARRENDAT, P0403_ID_ARRENDADORA, P0404_ID_EDIFICIO, T07_EDIFICIO.P0710_ID_CENTRO  
            //FROM T04_CONTRATO 
            //JOIN T07_EDIFICIO ON T04_CONTRATO.P0404_ID_EDIFICIO = T07_EDIFICIO.P0701_ID_EDIFICIO 
            //WHERE P0403_ID_ARRENDADORA = ? AND T07_EDIFICIO.P0710_ID_CENTRO = ?";

            string sql = @"SELECT T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, 
T24_HISTORIA_RECIBOS.CAMPO4 AS IDConjunto, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE,  T24_HISTORIA_RECIBOS.CAMPO_NUM5 AS IDPago    
FROM T24_HISTORIA_RECIBOS 
LEFT JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T24_HISTORIA_RECIBOS.CAMPO4 
WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ? 
AND T24_HISTORIA_RECIBOS.CAMPO4 = ?  
AND T24_HISTORIA_RECIBOS.P2406_STATUS='2' 
AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','W','X','P')  
AND T24_HISTORIA_RECIBOS.CAMPO_NUM5 > 0
AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO>= '" + fechaIni.ToString("MM/dd/yyyy") + "' AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= '"+fechaFin.ToString("MM/dd/yyyy") + "' " ;
            if(idConjunto == "CTR0")
            {
                sql = @"SELECT T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, 
T24_HISTORIA_RECIBOS.CAMPO4 AS IDConjunto, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE,  T24_HISTORIA_RECIBOS.CAMPO_NUM5 AS IDPago    
FROM T24_HISTORIA_RECIBOS 
LEFT JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T24_HISTORIA_RECIBOS.CAMPO4 
WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ? 
AND T24_HISTORIA_RECIBOS.CAMPO4 IS NULL  
AND T24_HISTORIA_RECIBOS.P2406_STATUS='2' 
AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','W','X','P')  
AND T24_HISTORIA_RECIBOS.CAMPO_NUM5 > 0 
AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= '" + fechaIni.ToString("MM/dd/yyyy") + "' AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= '" + fechaFin.ToString("MM/dd/yyyy") + "' ";
            }
            else if (idConjunto == "CTRT")
            {
                sql = @"SELECT T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, 
T24_HISTORIA_RECIBOS.CAMPO4 AS IDConjunto, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE,  T24_HISTORIA_RECIBOS.CAMPO_NUM5 AS IDPago    
FROM T24_HISTORIA_RECIBOS 
LEFT JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T24_HISTORIA_RECIBOS.CAMPO4 
WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ? 
AND T24_HISTORIA_RECIBOS.P2406_STATUS='2' 
AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','W','X','P')  
AND T24_HISTORIA_RECIBOS.CAMPO_NUM5 > 0 
AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= '" + fechaIni.ToString("MM/dd/yyyy") + "' AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= '" + fechaFin.ToString("MM/dd/yyyy") + "' ";
            }
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                List<ConjuntoInmobiliaria> lista = new List<ConjuntoInmobiliaria>();
                conexion.Open();
                comando.Parameters.Add("@idArr", OdbcType.VarChar).Value = idArr;
                if(idConjunto!="CTR0")
                    comando.Parameters.Add("@idConj", OdbcType.VarChar).Value = idConjunto;

                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ConjuntoInmobiliaria info = new ConjuntoInmobiliaria()
                    {
                        IDContrato = string.Empty,
                        IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                        IDConjunto = reader["IDConjunto"] == DBNull.Value ? "CTR0" : reader["IDConjunto"].ToString(),
                        Conjunto = reader["P0303_NOMBRE"] == DBNull.Value ? "LIBRE" : reader["P0303_NOMBRE"].ToString(),
                        IdHistRec = reader["P2444_ID_HIST_REC"].ToString(),
                        IDPago = reader["IDPago"] == DBNull.Value ? 0:  Convert.ToInt32(reader["IDPago"])                                       
                    };
                    lista.Add(info);

                }
                reader.Close();
                conexion.Close();
                return lista;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }


        public static List<AgendaEntity> getRegistrosAgenda(ClienteEntity cliente, string usuario, string estatus, DateTime fechaInicial, DateTime fechaFinal)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = @"SELECT T37_AGENDA.P3702_FECHA AS Fecha,T02_ARRENDATARIO.P0203_NOMBRE as Nombre, 
T37_AGENDA.P3705_MOTIVO AS Motivo,  T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario, 
T37_AGENDA.P3704_STATUS as Estatus from T02_ARRENDATARIO JOIN T37_AGENDA ON T02_ARRENDATARIO.P0201_ID = T37_AGENDA.P3701_ID_CTE
WHERE T37_AGENDA.P3702_FECHA >= ? AND T37_AGENDA.P3702_FECHA <= ? ";
                    if (cliente != null)
                        sql += " AND T02_ARRENDATARIO.P0201_ID = ? ";
                    if (!string.IsNullOrWhiteSpace(usuario))
                        sql += " AND T37_AGENDA.P3706_USUARIO = ?";
                    if (!string.IsNullOrWhiteSpace(estatus))
                        sql += " AND T37_AGENDA.P3704_STATUS = ?";
                    sql += " ORDER BY T37_AGENDA.P3702_FECHA";

                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@FechaIni", OdbcType.Date).Value = fechaInicial.Date;
                        comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaFinal.Date;
                        if (cliente != null)
                            comando.Parameters.Add("@IDCliente", OdbcType.VarChar).Value = cliente.IDCliente;
                        if (!string.IsNullOrWhiteSpace(usuario))
                            comando.Parameters.Add("@Usuario", OdbcType.VarChar).Value = usuario;
                        if (!string.IsNullOrWhiteSpace(estatus))
                            comando.Parameters.Add("@Estatus", OdbcType.VarChar).Value = estatus == "Terminado" ? "S" : "N";
                        List<AgendaEntity> listaAgenda = new List<AgendaEntity>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AgendaEntity agenda = new AgendaEntity();
                                agenda.Fecha = Convert.ToDateTime(reader["Fecha"]);
                                agenda.Nombre = reader["Nombre"].ToString();
                                agenda.Motivo = reader["Motivo"].ToString();
                                agenda.Observacion = reader["Observacion"] == DBNull.Value ? null : (byte[])reader["Observacion"];
                                agenda.Usuario = reader["Usuario"].ToString();
                                agenda.Estatus = reader["Estatus"].ToString();
                                listaAgenda.Add(agenda);
                            }
                        }
                        return listaAgenda;
                    }
                }
            }
            catch
            {
                return new List<AgendaEntity>();
            }
        }

        public static List<string> getClasif()
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = "SELECT DESCR_CAT FROM CATEGORIA WHERE CATEGORIA.ID_COT = 'CLASMANTEN' order by DESCR_CAT ASC";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        List<string> clasificacion = new List<string>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                                clasificacion.Add(reader["DESCR_CAT"].ToString());

                        }
                        return clasificacion;
                    }
                }

            }
            catch
            {
                return new List<string>();
            }
        }
        //Dreamsitoer
        public static List<mantGastosEntity> getRegistroMantGastos(string inmobiliaria, string conjunto, string inmueble, string usuario, string clasif, DateTime fechaInicio, DateTime fechaFin, string includInmue)
        {

            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    using (OdbcCommand comando = new OdbcCommand())
                    {
                        string sql = "";
                        DateTime fechaI = new DateTime(fechaInicio.Year, fechaInicio.Month, fechaInicio.Day, 0, 0, 0);
                        DateTime fechaEnd = new DateTime(fechaFin.Year, fechaFin.Month, fechaFin.Day, 23, 59, 59);

                        if (conjunto == "Todos" && inmueble == "Todos" && usuario == "Todos" && clasif == "Todos")
                        {   //Seleccion Todo
                            sql = @" 
SELECT P3602_FECHA, P0703_NOMBRE, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5, T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, 
T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
       FROM T36_SEGUIMIENTO 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO= T36_SEGUIMIENTO.P3601_ID_CTE 
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T07_EDIFICIO.P0710_ID_CENTRO
WHERE T36_SEGUIMIENTO.CAMPO5  not like 'EGRESOS' and  T07_EDIFICIO.P0709_ARRENDADORA=? AND  T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?   
UNION SELECT P3602_FECHA,'Conjunto'AS tipo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5,
T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
 FROM T36_SEGUIMIENTO        
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T36_SEGUIMIENTO.P3601_ID_CTE 
WHERE   T36_SEGUIMIENTO.CAMPO5 not like'EGRESOS' and CAMPO1=? AND T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=? ";

                            comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                            comando.Parameters.Add("@fechaIni", OdbcType.Date).Value = fechaI;
                            comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaEnd;
                            comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria;
                            comando.Parameters.Add("@fechaIni2", OdbcType.Date).Value = fechaInicio;
                            comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaFin;

                        }
                        // Seleccion Solo conjunto                   

                        else if (conjunto != "Todos" && includInmue == "Solo conjunto")
                        {
                            if (clasif != "Todos" && usuario != "Todos")
                            {
                                sql = @"
                         SELECT P3602_FECHA,'Conjunto'AS tipo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5,
                        T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
                         FROM T36_SEGUIMIENTO        
                        JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T36_SEGUIMIENTO.P3601_ID_CTE 
                        WHERE   T36_SEGUIMIENTO.CAMPO5 not like'EGRESOS' and CAMPO1= ? AND T36_SEGUIMIENTO.P3602_FECHA>= ? AND T36_SEGUIMIENTO.P3602_FECHA<= ? 
                        AND T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = ? AND T36_SEGUIMIENTO.P3605_REPRESENTATE = ? AND T36_SEGUIMIENTO.CAMPO5= ?";

                                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni", OdbcType.Date).Value = fechaI;
                                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaEnd;
                                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = conjunto;
                                comando.Parameters.Add("@Usuario", OdbcType.VarChar).Value = usuario;
                                comando.Parameters.Add("@Clasificacion", OdbcType.VarChar).Value = clasif;

                            }
                            else if (usuario != "Todos")
                            {
                                sql = @"
                         SELECT P3602_FECHA,'Conjunto'AS tipo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5,
                        T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
                         FROM T36_SEGUIMIENTO        
                        JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T36_SEGUIMIENTO.P3601_ID_CTE 
                        WHERE   T36_SEGUIMIENTO.CAMPO5 not like'EGRESOS' and CAMPO1= ? AND T36_SEGUIMIENTO.P3602_FECHA>= ? AND T36_SEGUIMIENTO.P3602_FECHA<= ? AND T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = ? AND T36_SEGUIMIENTO.P3605_REPRESENTATE = ?";

                                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni", OdbcType.Date).Value = fechaI;
                                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaEnd;
                                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = conjunto;
                                comando.Parameters.Add("@Usuario", OdbcType.VarChar).Value = usuario;

                            }
                            else if (clasif != "Todos")
                            {
                                sql = @"
                         SELECT P3602_FECHA,'Conjunto'AS tipo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5,
                        T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
                         FROM T36_SEGUIMIENTO        
                        JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T36_SEGUIMIENTO.P3601_ID_CTE 
                        WHERE   T36_SEGUIMIENTO.CAMPO5 not like'EGRESOS' and CAMPO1= ? AND T36_SEGUIMIENTO.P3602_FECHA>= ? AND T36_SEGUIMIENTO.P3602_FECHA<= ? 
                        AND T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = ? AND T36_SEGUIMIENTO.CAMPO5= ?";

                                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni", OdbcType.Date).Value = fechaI;
                                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaEnd;
                                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = conjunto;
                                comando.Parameters.Add("@Clasificacion", OdbcType.VarChar).Value = clasif;

                            }
                            else
                            {
                                sql = @"
                         SELECT P3602_FECHA,'Conjunto'AS tipo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5,
                        T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
                         FROM T36_SEGUIMIENTO        
                        JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T36_SEGUIMIENTO.P3601_ID_CTE 
                        WHERE   T36_SEGUIMIENTO.CAMPO5 not like'EGRESOS' and CAMPO1= ? AND T36_SEGUIMIENTO.P3602_FECHA>= ? AND T36_SEGUIMIENTO.P3602_FECHA<= ? 
                        AND T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = ?";
                                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni", OdbcType.Date).Value = fechaI;
                                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaEnd;
                                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = conjunto;
                            }
                        }

                        // Seleccion Conjunto con Inmuebles

                        if (conjunto != "Todos" && includInmue != "Solo conjunto")
                        {
                            if (usuario != "Todos" && clasif != "Todos")
                            {
                                sql = @"SELECT P3602_FECHA, P0703_NOMBRE, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5, T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, 
T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
       FROM T36_SEGUIMIENTO 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO= T36_SEGUIMIENTO.P3601_ID_CTE 
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T07_EDIFICIO.P0710_ID_CENTRO
WHERE T36_SEGUIMIENTO.CAMPO5  not like 'EGRESOS' and  T07_EDIFICIO.P0709_ARRENDADORA=? AND  T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?
AND T07_EDIFICIO.P0710_ID_CENTRO= ? AND T36_SEGUIMIENTO.P3605_REPRESENTATE = ? AND T36_SEGUIMIENTO.CAMPO5 = ? 
UNION SELECT P3602_FECHA,'Conjunto'AS tipo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5,
T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
 FROM T36_SEGUIMIENTO        
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T36_SEGUIMIENTO.P3601_ID_CTE
WHERE   T36_SEGUIMIENTO.CAMPO5 not like'EGRESOS' and CAMPO1=? AND T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?
AND T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = ? AND T36_SEGUIMIENTO.P3605_REPRESENTATE = ? AND T36_SEGUIMIENTO.CAMPO5 = ? ";
                                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni", OdbcType.Date).Value = fechaI;
                                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaEnd;
                                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = conjunto;
                                comando.Parameters.Add("@Usuario", OdbcType.VarChar).Value = usuario;
                                comando.Parameters.Add("@Clasificacion", OdbcType.VarChar).Value = clasif;
                                comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni2", OdbcType.Date).Value = fechaI;
                                comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaEnd;
                                comando.Parameters.Add("@IDConjunto2", OdbcType.VarChar).Value = conjunto;
                                comando.Parameters.Add("@Usuario2", OdbcType.VarChar).Value = usuario;
                                comando.Parameters.Add("@Clasificacion2", OdbcType.VarChar).Value = clasif;
                            }
                            else if (clasif != "Todos")
                            {
                                sql = @"SELECT P3602_FECHA, P0703_NOMBRE, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5, T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, 
T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
       FROM T36_SEGUIMIENTO 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO= T36_SEGUIMIENTO.P3601_ID_CTE 
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T07_EDIFICIO.P0710_ID_CENTRO
WHERE T36_SEGUIMIENTO.CAMPO5  not like 'EGRESOS' and  T07_EDIFICIO.P0709_ARRENDADORA=? AND  T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?
AND T07_EDIFICIO.P0710_ID_CENTRO= ? AND T36_SEGUIMIENTO.CAMPO5 = ? 
UNION SELECT P3602_FECHA,'Conjunto'AS tipo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5,
T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
 FROM T36_SEGUIMIENTO        
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T36_SEGUIMIENTO.P3601_ID_CTE
WHERE   T36_SEGUIMIENTO.CAMPO5 not like'EGRESOS' and CAMPO1=? AND T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?
AND T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = ? AND T36_SEGUIMIENTO.CAMPO5 = ? ";
                                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni", OdbcType.Date).Value = fechaI;
                                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaEnd;
                                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = conjunto;
                                comando.Parameters.Add("@Clasificacion", OdbcType.VarChar).Value = clasif;
                                comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni2", OdbcType.Date).Value = fechaI;
                                comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaEnd;
                                comando.Parameters.Add("@IDConjunto2", OdbcType.VarChar).Value = conjunto;
                                comando.Parameters.Add("@Clasificacion2", OdbcType.VarChar).Value = clasif;
                            }
                            else if (usuario != "Todos")
                            {
                                sql = @"SELECT P3602_FECHA, P0703_NOMBRE, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5, T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, 
T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
       FROM T36_SEGUIMIENTO 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO= T36_SEGUIMIENTO.P3601_ID_CTE 
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T07_EDIFICIO.P0710_ID_CENTRO
WHERE T36_SEGUIMIENTO.CAMPO5  not like 'EGRESOS' and  T07_EDIFICIO.P0709_ARRENDADORA=? AND  T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?
AND T07_EDIFICIO.P0710_ID_CENTRO= ? AND T36_SEGUIMIENTO.P3605_REPRESENTATE = ? 
UNION SELECT P3602_FECHA,'Conjunto'AS tipo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5,
T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
 FROM T36_SEGUIMIENTO        
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T36_SEGUIMIENTO.P3601_ID_CTE
WHERE   T36_SEGUIMIENTO.CAMPO5 not like'EGRESOS' and CAMPO1=? AND T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?
AND T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = ? AND T36_SEGUIMIENTO.P3605_REPRESENTATE = ?";
                                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni", OdbcType.Date).Value = fechaI;
                                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaEnd;
                                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = conjunto;
                                comando.Parameters.Add("@Usuario", OdbcType.VarChar).Value = usuario;
                                comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni2", OdbcType.Date).Value = fechaI;
                                comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaEnd;
                                comando.Parameters.Add("@IDConjunto2", OdbcType.VarChar).Value = conjunto;
                                comando.Parameters.Add("@Usuario2", OdbcType.VarChar).Value = usuario;
                            }
                            else
                            {

                                sql = @"SELECT P3602_FECHA, P0703_NOMBRE, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5, T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, 
T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
       FROM T36_SEGUIMIENTO 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO= T36_SEGUIMIENTO.P3601_ID_CTE 
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T07_EDIFICIO.P0710_ID_CENTRO
WHERE T36_SEGUIMIENTO.CAMPO5  not like 'EGRESOS' and  T07_EDIFICIO.P0709_ARRENDADORA=? AND  T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?
AND T07_EDIFICIO.P0710_ID_CENTRO= ?
UNION SELECT P3602_FECHA,'Conjunto'AS tipo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5,
T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
 FROM T36_SEGUIMIENTO        
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T36_SEGUIMIENTO.P3601_ID_CTE
WHERE   T36_SEGUIMIENTO.CAMPO5 not like'EGRESOS' and CAMPO1=? AND T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?
AND T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = ?";
                                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni", OdbcType.Date).Value = fechaI;
                                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaEnd;
                                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = conjunto;
                                comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni2", OdbcType.Date).Value = fechaI;
                                comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaEnd;
                                comando.Parameters.Add("@IDConjunto2", OdbcType.VarChar).Value = conjunto;
                            }


                        }

                        //Seleccion Inmuebles
                        else if (inmueble != "Todos")
                        {
                            if (usuario != "Todos" && clasif != "Todos")
                            {
                                sql = @"SELECT P3602_FECHA, P0703_NOMBRE, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5, T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, 
T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
       FROM T36_SEGUIMIENTO 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO= T36_SEGUIMIENTO.P3601_ID_CTE 
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T07_EDIFICIO.P0710_ID_CENTRO
WHERE T36_SEGUIMIENTO.CAMPO5  not like 'EGRESOS' and  T07_EDIFICIO.P0709_ARRENDADORA=? AND  T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?  AND T07_EDIFICIO.P0701_ID_EDIFICIO= ?  AND T36_SEGUIMIENTO.P3605_REPRESENTATE=? AND T36_SEGUIMIENTO.CAMPO5=?";
                                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni", OdbcType.DateTime).Value = fechaI;
                                comando.Parameters.Add("@fechaFin", OdbcType.DateTime).Value = fechaEnd;
                                comando.Parameters.Add("@Inmueble", OdbcType.VarChar).Value = inmueble;
                                comando.Parameters.Add("@Usuari", OdbcType.VarChar).Value = usuario;
                                comando.Parameters.Add("@clasifcacion", OdbcType.VarChar).Value = clasif;
                            }
                            else if (usuario != "Todos")
                            {
                                sql = @"SELECT P3602_FECHA, P0703_NOMBRE, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5, T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, 
T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
       FROM T36_SEGUIMIENTO 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO= T36_SEGUIMIENTO.P3601_ID_CTE 
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T07_EDIFICIO.P0710_ID_CENTRO
WHERE T36_SEGUIMIENTO.CAMPO5  not like 'EGRESOS' and  T07_EDIFICIO.P0709_ARRENDADORA=? AND  T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?  AND T36_SEGUIMIENTO.P3605_REPRESENTATE=?";
                                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni", OdbcType.Date).Value = fechaI;
                                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaEnd;
                                comando.Parameters.Add("@Usuari", OdbcType.VarChar).Value = usuario;

                            }
                            else if (clasif != "Todos")
                            {
                                sql = @"
                        SELECT P3602_FECHA, P0703_NOMBRE, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5, T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, 
T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
       FROM T36_SEGUIMIENTO 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO= T36_SEGUIMIENTO.P3601_ID_CTE 
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T07_EDIFICIO.P0710_ID_CENTRO
WHERE T36_SEGUIMIENTO.CAMPO5  not like 'EGRESOS' and  T07_EDIFICIO.P0709_ARRENDADORA=? AND  T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?  AND T36_SEGUIMIENTO.CAMPO5=? 
UNION SELECT P3602_FECHA,'Conjunto'AS tipo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5";
                                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni", OdbcType.DateTime).Value = fechaI;
                                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaEnd;
                                comando.Parameters.Add("@clasifcacion", OdbcType.VarChar).Value = clasif;

                            }
                            else
                            {
                                sql = @"SELECT P3602_FECHA, P0703_NOMBRE, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5, T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, 
T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
       FROM T36_SEGUIMIENTO 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO= T36_SEGUIMIENTO.P3601_ID_CTE 
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T07_EDIFICIO.P0710_ID_CENTRO
WHERE T36_SEGUIMIENTO.CAMPO5  not like 'EGRESOS' and  T07_EDIFICIO.P0709_ARRENDADORA=? AND  T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?  AND T07_EDIFICIO.P0701_ID_EDIFICIO= ?";
                                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                                comando.Parameters.Add("@fechaIni", OdbcType.DateTime).Value = fechaI;
                                comando.Parameters.Add("@fechaFin", OdbcType.DateTime).Value = fechaEnd;
                                comando.Parameters.Add("@Inmueble", OdbcType.VarChar).Value = inmueble;
                            }
                        }

                        else if (usuario != "Todos")
                        {
                            sql = @"SELECT P3602_FECHA, P0703_NOMBRE, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5, T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, 
T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
       FROM T36_SEGUIMIENTO 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO= T36_SEGUIMIENTO.P3601_ID_CTE 
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T07_EDIFICIO.P0710_ID_CENTRO
WHERE T36_SEGUIMIENTO.CAMPO5  not like 'EGRESOS' and  T07_EDIFICIO.P0709_ARRENDADORA=? AND  T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?  AND T36_SEGUIMIENTO.P3605_REPRESENTATE=?
UNION SELECT P3602_FECHA,'Conjunto'AS tipo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5,
T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
 FROM T36_SEGUIMIENTO        
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T36_SEGUIMIENTO.P3601_ID_CTE 
WHERE   T36_SEGUIMIENTO.CAMPO5 not like'EGRESOS' and CAMPO1=? AND T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=? AND T36_SEGUIMIENTO.P3605_REPRESENTATE=?";
                            comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                            comando.Parameters.Add("@fechaIni", OdbcType.Date).Value = fechaI;
                            comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaEnd;
                            comando.Parameters.Add("@Usuari", OdbcType.VarChar).Value = usuario;
                            comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria;
                            comando.Parameters.Add("@fechaIni2", OdbcType.Date).Value = fechaI;
                            comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaEnd;
                            comando.Parameters.Add("@Usuari", OdbcType.VarChar).Value = usuario;
                        }
                        else if (clasif != "Todos")
                        {
                            sql = @"
                        SELECT P3602_FECHA, P0703_NOMBRE, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5, T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, 
T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
       FROM T36_SEGUIMIENTO 
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO= T36_SEGUIMIENTO.P3601_ID_CTE 
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T07_EDIFICIO.P0710_ID_CENTRO
WHERE T36_SEGUIMIENTO.CAMPO5  not like 'EGRESOS' and  T07_EDIFICIO.P0709_ARRENDADORA=? AND  T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=?  AND T36_SEGUIMIENTO.CAMPO5=? 
UNION SELECT P3602_FECHA,'Conjunto'AS tipo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, P3603_ASUNTO, T36_SEGUIMIENTO.CAMPO2, T36_SEGUIMIENTO.CAMPO5,
T36_SEGUIMIENTO.P3604_TEXTO, T36_SEGUIMIENTO.CAMPO_NUM1, T36_SEGUIMIENTO.CAMPO_NUM3, T36_SEGUIMIENTO.CAMPO_NUM4, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM2,T36_SEGUIMIENTO.P3605_REPRESENTATE
 FROM T36_SEGUIMIENTO        
JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T36_SEGUIMIENTO.P3601_ID_CTE 
WHERE   T36_SEGUIMIENTO.CAMPO5 not like'EGRESOS' and CAMPO1=? AND T36_SEGUIMIENTO.P3602_FECHA>=? AND T36_SEGUIMIENTO.P3602_FECHA<=? AND T36_SEGUIMIENTO.CAMPO5=?";
                            comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria;
                            comando.Parameters.Add("@fechaIni", OdbcType.DateTime).Value = fechaI;
                            comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaEnd;
                            comando.Parameters.Add("@clasifcacion", OdbcType.VarChar).Value = clasif;
                            comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria;
                            comando.Parameters.Add("@fechaIni2", OdbcType.Date).Value = fechaI;
                            comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaEnd;
                            comando.Parameters.Add("@clasifcacion2", OdbcType.VarChar).Value = clasif;
                        }

                        List<mantGastosEntity> listaMantenimientoGastos = new List<mantGastosEntity>();
                        comando.CommandText = sql;
                        comando.Connection = conexion;
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                mantGastosEntity mantGastos = new mantGastosEntity();
                                if (conjunto != "Todos" && includInmue == "Solo conjunto")
                                {
                                    // FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                                    mantGastos.Fecha = Convert.ToDateTime(reader["P3602_FECHA"]);
                                    mantGastos.iDinmueble = reader["tipo"].ToString();
                                    mantGastos.tipoNombre = reader["P0303_NOMBRE"] == DBNull.Value ? null : reader["P0303_NOMBRE"].ToString();
                                    mantGastos.Proveedor = reader["CAMPO2"].ToString();
                                    mantGastos.Asunto = reader["P3603_ASUNTO"].ToString();
                                    mantGastos.clasificacion = reader["CAMPO5"].ToString();
                                    //mantenimento.Observacion = reader["Observacion"] == DBNull.Value ? null : (byte[])reader["Observacion"];
                                    //agenda.Observacion = reader["Observacion"] == DBNull.Value ? null : (byte[])reader["Observacion"];
                                    mantGastos.descripcion = reader["P3604_TEXTO"] == DBNull.Value ? null : (byte[])reader["P3604_TEXTO"];
                                    //mantGastos.descripcion = reader["P3604_TEXTO"] == DBNull.Value ?  : reader["P3604_TEXTO"].ToString();
                                    mantGastos.Subtotal = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                                    mantGastos.Iva = reader["CAMPO_NUM3"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM3"]);
                                    mantGastos.Total = reader["CAMPO_NUM4"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM4"]);
                                    mantGastos.TipoMoneda = reader["CAMPO3"] == null ? "P" : Convert.ToString(reader["CAMPO3"]);
                                    mantGastos.TipoCambio = reader["CAMPO_NUM2"] == DBNull.Value ? 1 : Convert.ToDecimal(reader["CAMPO_NUM2"]);
                                    mantGastos.Usuarios = reader["P3605_REPRESENTATE"].ToString();
                                    mantGastos.subTotalDolares = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                                    mantGastos.IvalDolares = reader["CAMPO_NUM3"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM3"]);
                                    mantGastos.TotalDolares = reader["CAMPO_NUM4"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM4"]);
                                }

                                else
                                {
                                    mantGastos.Fecha = Convert.ToDateTime(reader["P3602_FECHA"]);
                                    mantGastos.iDinmueble = reader["P0703_NOMBRE"].ToString();
                                    mantGastos.tipoNombre = reader["P0303_NOMBRE"] == DBNull.Value ? null : reader["P0303_NOMBRE"].ToString();
                                    mantGastos.Proveedor = reader["CAMPO2"].ToString();
                                    mantGastos.Asunto = reader["P3603_ASUNTO"].ToString();
                                    mantGastos.clasificacion = reader["CAMPO5"].ToString();
                                    //mantenimento.Observacion = reader["Observacion"] == DBNull.Value ? null : (byte[])reader["Observacion"];
                                    //agenda.Observacion = reader["Observacion"] == DBNull.Value ? null : (byte[])reader["Observacion"];
                                    mantGastos.descripcion = reader["P3604_TEXTO"] == DBNull.Value ? null : (byte[])reader["P3604_TEXTO"];
                                    //mantGastos.descripcion = reader["P3604_TEXTO"] == DBNull.Value ?  : reader["P3604_TEXTO"].ToString();
                                    mantGastos.Subtotal = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                                    mantGastos.Iva = reader["CAMPO_NUM3"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM3"]);
                                    mantGastos.Total = reader["CAMPO_NUM4"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM4"]);
                                    mantGastos.TipoMoneda = reader["CAMPO3"] == null ? "P" : Convert.ToString(reader["CAMPO3"]);
                                    mantGastos.TipoCambio = reader["CAMPO_NUM2"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM2"]);
                                    mantGastos.Usuarios = reader["P3605_REPRESENTATE"].ToString();
                                    mantGastos.subTotalDolares = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                                    mantGastos.IvalDolares = reader["CAMPO_NUM3"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM3"]);
                                    mantGastos.TotalDolares = reader["CAMPO_NUM4"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM4"]);
                                }

                                listaMantenimientoGastos.Add(mantGastos);
                            }
                        }
                        return listaMantenimientoGastos.OrderBy(m => m.Fecha).ToList();
                    }


                }
            }
            catch
            {
                return new List<mantGastosEntity>();
            }


        }
        public static string getNombreSubConjuntoByIDSub(string idSubconjunto)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P1803_NOMBRE FROM T18_SUBCONJUNTOS WHERE T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idSubconjunto", OdbcType.VarChar).Value = idSubconjunto;
                conexion.Open();
                string result = comando.ExecuteScalar().ToString();
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }

        public static List<MantenimientoEntity> getRegistrosMantenimiento(InmobiliariaEntity inmobiliaria, string usuario, string estatus, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    using (OdbcCommand comando = new OdbcCommand())
                    {
                        string sql = string.Empty;
                        switch (estatus.ToLower())
                        {
                            case "vencidos":
                                if (string.IsNullOrWhiteSpace(usuario))
                                {
                                    sql = @"select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, 
T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, 
T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL 
JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE 
where T03_CENTRO_INDUSTRIAL.CAMPO1 = ? AND T37_AGENDA.P3702_FECHA<=? AND T37_AGENDA.P3704_STATUS='N'  
Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, 
T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario 
from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE 
where T07_EDIFICIO.P0709_ARRENDADORA = ? AND T37_AGENDA.P3702_FECHA<= ? AND T37_AGENDA.P3704_STATUS='N'";
                                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni2", OdbcType.Date).Value = fechaInicio.Date;
                                }
                                else
                                {
                                    sql = @"select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, 
T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, 
T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL 
JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE 
where T03_CENTRO_INDUSTRIAL.CAMPO1 = ? AND T37_AGENDA.P3702_FECHA <= ? and T37_AGENDA.P3706_USUARIO = ? AND T37_AGENDA.P3704_STATUS='N' 
Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, 
T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario 
from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE 
where T07_EDIFICIO.P0709_ARRENDADORA = ? AND T37_AGENDA.P3702_FECHA <= ? and T37_AGENDA.P3706_USUARIO = ? AND T37_AGENDA.P3704_STATUS='N'";
                                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@User", OdbcType.VarChar).Value = usuario;
                                    comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni2", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@User2", OdbcType.VarChar).Value = usuario;
                                }
                                break;
                            case "pendientes":
                                if (string.IsNullOrWhiteSpace(usuario))
                                {
                                    sql = @"select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, 
T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, 
T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL 
JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE 
where T03_CENTRO_INDUSTRIAL.CAMPO1 = ? AND T37_AGENDA.P3702_FECHA >= ? AND T37_AGENDA.P3702_FECHA <= ? AND T37_AGENDA.P3704_STATUS='N'  
Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, 
T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario 
from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE 
where T07_EDIFICIO.P0709_ARRENDADORA = ? AND T37_AGENDA.P3702_FECHA >= ? AND T37_AGENDA.P3702_FECHA <= ? AND T37_AGENDA.P3704_STATUS='N'";
                                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaFin.Date;
                                    comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni2", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaFin.Date;
                                }
                                else
                                {
                                    sql = @"select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, 
T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, 
T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL 
JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE 
where T03_CENTRO_INDUSTRIAL.CAMPO1 = ? AND T37_AGENDA.P3702_FECHA >= ? AND T37_AGENDA.P3702_FECHA <= ? and T37_AGENDA.P3706_USUARIO = ? AND T37_AGENDA.P3704_STATUS='N' 
Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, 
T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario 
from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE 
where T07_EDIFICIO.P0709_ARRENDADORA = ? AND T37_AGENDA.P3702_FECHA >= ? AND T37_AGENDA.P3702_FECHA <= ? and T37_AGENDA.P3706_USUARIO = ? AND T37_AGENDA.P3704_STATUS='N'";
                                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaFin.Date;
                                    comando.Parameters.Add("@User", OdbcType.VarChar).Value = usuario;
                                    comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni2", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaFin.Date;
                                    comando.Parameters.Add("@User2", OdbcType.VarChar).Value = usuario;
                                }
                                break;
                            case "terminados":
                                if (string.IsNullOrWhiteSpace(usuario))
                                {
                                    sql = @"select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, 
T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, 
T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL 
JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE 
where T03_CENTRO_INDUSTRIAL.CAMPO1 = ? AND T37_AGENDA.P3702_FECHA >= ? AND T37_AGENDA.P3702_FECHA <= ? AND T37_AGENDA.P3704_STATUS='S' 
Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, 
T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario 
from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE 
where T07_EDIFICIO.P0709_ARRENDADORA = ? AND T37_AGENDA.P3702_FECHA >= ? AND T37_AGENDA.P3702_FECHA <= ? AND T37_AGENDA.P3704_STATUS='S'";
                                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaFin.Date;
                                    comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni2", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaFin.Date;
                                }
                                else
                                {
                                    sql = @"select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, 
T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, 
T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL 
JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE 
where T03_CENTRO_INDUSTRIAL.CAMPO1 = ? AND T37_AGENDA.P3702_FECHA >= ? AND T37_AGENDA.P3702_FECHA <= ? and T37_AGENDA.P3706_USUARIO = ? AND T37_AGENDA.P3704_STATUS='S' 
Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, 
T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario 
from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE 
where T07_EDIFICIO.P0709_ARRENDADORA = ? AND T37_AGENDA.P3702_FECHA >= ? AND T37_AGENDA.P3702_FECHA <= ? and T37_AGENDA.P3706_USUARIO = ? AND T37_AGENDA.P3704_STATUS='S'";
                                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaFin.Date;
                                    comando.Parameters.Add("@User", OdbcType.VarChar).Value = usuario;
                                    comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni2", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaFin.Date;
                                    comando.Parameters.Add("@User2", OdbcType.VarChar).Value = usuario;
                                }
                                break;
                            default:
                                if (string.IsNullOrWhiteSpace(usuario))
                                {
                                    sql = @"select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, 
T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, 
T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL 
JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE 
where T03_CENTRO_INDUSTRIAL.CAMPO1 = ? AND T37_AGENDA.P3702_FECHA >= ? AND T37_AGENDA.P3702_FECHA <= ?  
Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, 
T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario 
from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE 
where T07_EDIFICIO.P0709_ARRENDADORA = ? AND T37_AGENDA.P3702_FECHA >= ? AND T37_AGENDA.P3702_FECHA <= ?";
                                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaFin.Date;
                                    comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni2", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaFin.Date;
                                }
                                else
                                {
                                    sql = @"select 'Conjunto' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, 
T37_AGENDA.P3705_MOTIVO AS Motivo, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE as Descripcion, T37_AGENDA.P3703_DESCRIPCION as Observacion, 
T37_AGENDA.P3706_USUARIO as Usuario from T03_CENTRO_INDUSTRIAL 
JOIN T37_AGENDA ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T37_AGENDA.P3701_ID_CTE 
where T03_CENTRO_INDUSTRIAL.CAMPO1 = ? AND T37_AGENDA.P3702_FECHA >= ? AND T37_AGENDA.P3702_FECHA <= ? and T37_AGENDA.P3706_USUARIO = ? 
Union All select 'Inmueble' as Tipo, T37_AGENDA.P3704_STATUS AS Estatus, T37_AGENDA.P3702_FECHA AS Fecha, T37_AGENDA.P3705_MOTIVO AS Motivo, 
T07_EDIFICIO.P0703_NOMBRE as Edificio, T37_AGENDA.P3703_DESCRIPCION as Observacion, T37_AGENDA.P3706_USUARIO as Usuario 
from T07_EDIFICIO  JOIN T37_AGENDA ON T07_EDIFICIO.P0701_ID_EDIFICIO = T37_AGENDA.P3701_ID_CTE 
where T07_EDIFICIO.P0709_ARRENDADORA = ? AND T37_AGENDA.P3702_FECHA >= ? AND T37_AGENDA.P3702_FECHA <= ? and T37_AGENDA.P3706_USUARIO = ? ";
                                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaFin.Date;
                                    comando.Parameters.Add("@User", OdbcType.VarChar).Value = usuario;
                                    comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = inmobiliaria.ID;
                                    comando.Parameters.Add("@FechaIni2", OdbcType.Date).Value = fechaInicio.Date;
                                    comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaFin.Date;
                                    comando.Parameters.Add("@User2", OdbcType.VarChar).Value = usuario;
                                }
                                break;
                        }
                        List<MantenimientoEntity> listaMantenimiento = new List<MantenimientoEntity>();
                        comando.CommandText = sql;
                        comando.Connection = conexion;
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MantenimientoEntity mantenimento = new MantenimientoEntity();
                                mantenimento.Tipo = reader["Tipo"].ToString();
                                mantenimento.Estatus = reader["Estatus"].ToString();
                                mantenimento.Fecha = Convert.ToDateTime(reader["Fecha"]);
                                mantenimento.Motivo = reader["Motivo"].ToString();
                                mantenimento.Descripcion = reader["Descripcion"].ToString();
                                mantenimento.Observacion = reader["Observacion"] == DBNull.Value ? null : (byte[])reader["Observacion"];
                                mantenimento.Usuario = reader["Usuario"].ToString();
                                listaMantenimiento.Add(mantenimento);
                            }
                        }
                        return listaMantenimiento.OrderBy(m => m.Fecha).ToList();
                    }
                }
            }
            catch
            {
                return new List<MantenimientoEntity>();
            }
        }

        public static List<BitacoraEntity> getRegistrosBitacora(ClienteEntity cliente, string usuario, string estatus, string etapa, DateTime fechaInicial, DateTime fechaFinal, string clasifCteProsp)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = @"SELECT T36_SEGUIMIENTO.P3601_ID_CTE, T02_ARRENDATARIO.P0203_NOMBRE, T36_SEGUIMIENTO.P3602_FECHA, T36_SEGUIMIENTO.P3603_ASUNTO, 
                        T36_SEGUIMIENTO.P3604_TEXTO AS DESCRIPCION, T36_SEGUIMIENTO.P3605_REPRESENTATE AS USUARIO, T15_PERSONA.P1503_NOMBRE, 
                        T02_ARRENDATARIO.P0234_CAMPO12, T02_ARRENDATARIO.P0202_TIPO_ENTE, T02_ARRENDATARIO.P0219_F_ALTA, T02_ARRENDATARIO.CAMPO_DATE1, 
                        T02_ARRENDATARIO.CAMPO_NUM1 
                        FROM T36_SEGUIMIENTO
                        LEFT JOIN T02_ARRENDATARIO ON P3601_ID_CTE = P0201_ID
                        LEFT JOIN T15_PERSONA ON (P3601_ID_CTE= P1504_ID_ENTE_EXT AND T15_PERSONA.P1502_TIPO_ENTE=13) 
                        WHERE T36_SEGUIMIENTO.P3602_FECHA >= ? AND T36_SEGUIMIENTO.P3602_FECHA <= ? ";
                    if (string.IsNullOrWhiteSpace(clasifCteProsp))
                        sql += " AND (T02_ARRENDATARIO.P0202_TIPO_ENTE=2 OR T02_ARRENDATARIO.P0202_TIPO_ENTE=22) ";
                    else if (clasifCteProsp == "Clientes")
                        sql += " AND T02_ARRENDATARIO.P0202_TIPO_ENTE=2 ";
                    else if (clasifCteProsp == "Prospectos")
                        sql += " AND T02_ARRENDATARIO.P0202_TIPO_ENTE=22 ";
                    if (cliente != null)
                        sql += " AND T36_SEGUIMIENTO.P3601_ID_CTE = ? ";
                    if (!string.IsNullOrWhiteSpace(usuario))
                        sql += " AND T36_SEGUIMIENTO.P3605_REPRESENTATE = ? ";
                    if (!string.IsNullOrWhiteSpace(etapa))
                        sql += " AND T02_ARRENDATARIO.P0234_CAMPO12 = ? ";

                    sql += " ORDER BY T36_SEGUIMIENTO.P3602_FECHA ";

                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        #region parametros
                        DateTime fechaIni = new DateTime(fechaInicial.Year, fechaInicial.Month, fechaInicial.Day, 0, 0, 0);
                        DateTime fechaFin = new DateTime(fechaFinal.Year, fechaFinal.Month, fechaFinal.Day, 23, 59, 59);
                        comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                        comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                        if (cliente != null)
                            comando.Parameters.Add("@IDCliente", OdbcType.VarChar).Value = cliente.IDCliente;
                        if (!string.IsNullOrWhiteSpace(usuario))
                            comando.Parameters.Add("@Usuario", OdbcType.VarChar).Value = usuario;
                        if (!string.IsNullOrWhiteSpace(etapa))
                            comando.Parameters.Add("@Etapa", OdbcType.VarChar).Value = etapa;
                        #endregion
                        List<BitacoraEntity> listaBitacora = new List<BitacoraEntity>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BitacoraEntity bitacora = new BitacoraEntity();
                                bitacora.Fecha = Convert.ToDateTime(reader["P3602_FECHA"]);
                                bitacora.IdCliente = reader["P3601_ID_CTE"].ToString();
                                bitacora.cliente = reader["P0203_NOMBRE"].ToString();
                                bitacora.Contacto = reader["P1503_NOMBRE"].ToString();
                                bitacora.Asunto = reader["P3603_ASUNTO"].ToString();
                                bitacora.Descripcion = reader["DESCRIPCION"] == DBNull.Value ? null : (byte[])reader["DESCRIPCION"];
                                bitacora.Usuario = reader["USUARIO"].ToString();
                                bitacora.Etapa = reader["P0234_CAMPO12"].ToString();
                                DateTime? fecha = null;
                                bitacora.FechaContactoInicial = reader["P0219_F_ALTA"] == DBNull.Value ? fecha : Convert.ToDateTime(reader["P0219_F_ALTA"]);
                                bitacora.FechaEsperadaDeCierre = reader["CAMPO_DATE1"] == DBNull.Value ? fecha : Convert.ToDateTime(reader["CAMPO_DATE1"]);
                                Decimal? importe = null;
                                try
                                {
                                    bitacora.ImporteDeOportunidad = string.IsNullOrEmpty(reader["CAMPO_NUM1"].ToString()) ? importe : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                                }
                                catch (Exception ex)
                                {
                                    bitacora.ImporteDeOportunidad = 0;
                                }
                                int idEnte = reader["P0202_TIPO_ENTE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P0202_TIPO_ENTE"]);
                                if (idEnte == 2)
                                    bitacora.TipoEnte = "Cliente";
                                else if (idEnte == 22)
                                    bitacora.TipoEnte = "Prospecto";
                                else
                                    bitacora.TipoEnte = "";
                                listaBitacora.Add(bitacora);
                            }
                        }
                        foreach (BitacoraEntity regBitacora in listaBitacora)
                        {
                            DataTable AgendaSiguiente = getAgendaSiguienteDeCliente(regBitacora.IdCliente);
                            if (AgendaSiguiente != null)
                            {
                                if (AgendaSiguiente.Rows.Count > 0)
                                {
                                    DateTime? fecha = null;
                                    fecha = AgendaSiguiente.Rows[0]["P3702_FECHA"].ToString() == null ? fecha : Convert.ToDateTime(AgendaSiguiente.Rows[0]["P3702_FECHA"]);
                                    regBitacora.FechaAgendaSiguiente = fecha;
                                    regBitacora.DescripcionAgendaSiguiente = AgendaSiguiente.Rows[0]["P3705_MOTIVO"].ToString();
                                }
                                else
                                {
                                    regBitacora.FechaAgendaSiguiente = null;
                                    regBitacora.DescripcionAgendaSiguiente = string.Empty;
                                }

                            }
                            else
                            {
                                regBitacora.FechaAgendaSiguiente = null;
                                regBitacora.DescripcionAgendaSiguiente = string.Empty;
                            }
                        }
                        return listaBitacora;
                    }
                }
            }
            catch
            {
                return new List<BitacoraEntity>();
            }
        }
        public static DataTable getAgendaSiguienteDeCliente(string idCliente)
        {
            DataTable AgendaSiguiente = new DataTable();
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = @"SELECT FIRST 1 T37_AGENDA.P3701_ID_CTE, T37_AGENDA.P3702_FECHA, T37_AGENDA.P3704_STATUS, T37_AGENDA.P3705_MOTIVO 
                        FROM T37_AGENDA WHERE T37_AGENDA.P3701_ID_CTE = ? AND T37_AGENDA.P3704_STATUS = 'N'
                        ORDER BY T37_AGENDA.P3702_FECHA ASC";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@IDCliente", OdbcType.VarChar).Value = idCliente;
                        using (OdbcDataAdapter adapter = new OdbcDataAdapter(comando))
                        {
                            adapter.Fill(AgendaSiguiente);
                        }
                        return AgendaSiguiente;
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<string> getEtapasSeguimientoCRM()
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = "SELECT DESCR_CAT FROM CATEGORIA WHERE ID_COT = 'ETAPASVTA'";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        List<string> Etapas = new List<string>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                                Etapas.Add(reader["DESCR_CAT"].ToString());
                        }
                        return Etapas;
                    }
                }
            }
            catch
            {
                return new List<string>();
            }


        }
        public static bool existeSubconjuntoByID(string idInmueble)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P1803_NOMBRE, P18_CAMPO1 FROM T18_SUBCONJUNTOS WHERE P1801_ID_SUBCONJUNTO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInm", OdbcType.VarChar).Value = idInmueble;
                conexion.Open();
                bool existeSubconjunto = Convert.ToBoolean(comando.ExecuteReader().HasRows);//comando.ExecuteScalar()

                conexion.Close();
                return existeSubconjunto;
            }
            catch
            {
                conexion.Close();
                return false;
            }
        }

        public static ReporteGlobalEntity sumaM2ConstruccionSubconjunto(string idSub)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            ReporteGlobalEntity RepGlobal = new ReporteGlobalEntity();
            try
            {

                string sql = "SELECT SUM(P0707_CONTRUCCION_M2) FROM T07_EDIFICIO WHERE T07_EDIFICIO.P0730_SUBCONJUNTO = ?";//"SELECT P1803_NOMBRE, P18_CAMPO1 FROM T18_SUBCONJUNTOS WHERE P1801_ID_SUBCONJUNTO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInm", OdbcType.VarChar).Value = idSub;
                conexion.Open();
                RepGlobal.M2Construccion = Convert.ToDecimal(comando.ExecuteScalar());//comando.ExecuteScalar()
                conexion.Close();
                return RepGlobal;
            }
            catch
            {
                conexion.Close();
                return RepGlobal;
            }
        }

        public static ReporteGlobalEntity ValorRentaSubconjunto(string idSub)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            ReporteGlobalEntity RepGlobal = new ReporteGlobalEntity();
            try
            {

                string sql = "SELECT P18CAMPO_NUM1 FROM T18_SUBCONJUNTOS WHERE P1801_ID_SUBCONJUNTO = ?";//"SELECT P1803_NOMBRE, P18_CAMPO1 FROM T18_SUBCONJUNTOS WHERE P1801_ID_SUBCONJUNTO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInm", OdbcType.VarChar).Value = idSub;
                conexion.Open();
                RepGlobal.RentaActual = Convert.ToDecimal(comando.ExecuteScalar());//comando.ExecuteScalar()
                conexion.Close();

                return RepGlobal;
            }
            catch
            {
                conexion.Close();
                return RepGlobal;
            }
        }
        //add JL 24/04/2019
        //EDIT BY UZ 02/05/2019
        public static List<ReciboEntity> GetListaCtasxCobrar(string idArre, DateTime fechacorte)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"Select P2402_ID_ARRENDATARIO, P2411_N_ARRENDATARIO, T07_EDIFICIO.P0703_NOMBRE, T24_HISTORIA_RECIBOS.CAMPO_DATE1,T24_HISTORIA_RECIBOS.CAMPO_DATE2, P2408_FECHA_PAGADO, P2404_PERIODO, 
P2412_CONCEPTO, P2419_TOTAL, P2405_IMPORTE, T24_HISTORIA_RECIBOS.CAMPO5, T24_HISTORIA_RECIBOS.CAMPO_NUM4, T24_HISTORIA_RECIBOS.P2410_MONEDA,T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, 
P0712_IMPORTE_RENTA, T24_HISTORIA_RECIBOS.P2407_COMENTARIO, P2427_CTD_PAG, T07_EDIFICIO.CAMPO5 AS ID_MZNA, T07_EDIFICIO.CAMPO6 AS ID_LOTE, 
T24_HISTORIA_RECIBOS.CAMPO10 AS PCT_INTERES_MORATORIO,  T01_ARRENDADORA.CAMPO_NUM1 AS TIPO_CALCULO, T24_HISTORIA_RECIBOS.CAMPO_NUM1 , P2444_ID_HIST_REC 
from T24_HISTORIA_RECIBOS  
Inner Join T04_CONTRATO On (T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO = T04_CONTRATO.P0401_ID_CONTRATO) 
Inner Join T07_EDIFICIO On (T04_CONTRATO.P0404_ID_EDIFICIO = T07_EDIFICIO.P0701_ID_EDIFICIO) 
JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
Where T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA =  ?  
and T24_HISTORIA_RECIBOS.CAMPO_DATE1 <= ?    
and T24_HISTORIA_RECIBOS.P2426_TIPO_DOC in ('A','E','M', 'V', 'Z' ) 
And T24_HISTORIA_RECIBOS.P2406_STATUS =  '1'    
Order By T24_HISTORIA_RECIBOS.P2411_N_ARRENDATARIO, T07_EDIFICIO.P0703_NOMBRE, T24_HISTORIA_RECIBOS.CAMPO_DATE1  ";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idArr", OdbcType.VarChar).Value = idArre;
                comando.Parameters.Add("@fechacorte", OdbcType.Date).Value = fechacorte;
                List<ReciboEntity> listCntasxCobrar = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    recibo.NombreCliente = reader["P2411_N_ARRENDATARIO"].ToString();
                    recibo.NombreInmueble = reader["P0703_NOMBRE"].ToString();
                    recibo.VencimientoPago = reader["CAMPO_DATE1"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CAMPO_DATE1"]);
                    recibo.FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.Periodo = reader["P2404_PERIODO"].ToString();
                    recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                    recibo.IdManzana = reader["ID_MZNA"].ToString();
                    recibo.IdLote = reader["ID_LOTE"].ToString();
                    recibo.TipoCalculoMoratorios = reader["TIPO_CALCULO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TIPO_CALCULO"]);
                    recibo.PctInteresMoratorio = reader["PCT_INTERES_MORATORIO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PCT_INTERES_MORATORIO"]);
                    try
                    {
                        recibo.FechaVencimientoMoratorios = Convert.ToDateTime(reader["CAMPO_DATE2"]);
                    }
                    catch
                    {
                        recibo.FechaVencimientoMoratorios = fechacorte;
                    }
                    recibo.importePorPagar = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"];
                    //recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    recibo.TipoDoc = reader["P2426_TIPO_DOC"].ToString();
                    if (recibo.TipoDoc == "E")
                    {
                        recibo.PagoCapital = reader["CAMPO_NUM4"] == DBNull.Value ? 0 : (decimal)reader["CAMPO_NUM4"];
                    }
                    else if (recibo.TipoDoc == "V")
                    {
                        recibo.PagoCapital = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    }
                    else if (recibo.TipoDoc == "Z")
                    {
                        recibo.PagoCapital = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    }
                    else
                    {
                        recibo.PagoCapital = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    }

                    int Meses = 0;
                    int Dias = 0;
                    decimal Moratorios = 0;
                    DateTime FechaTemp = Convert.ToDateTime(recibo.VencimientoPago);
                    DateTime FechaPago = new DateTime();
                    string idhistrec = reader["P2444_ID_HIST_REC"].ToString(); ;

                    FechaPago = fechacorte;

                    if (recibo.TipoCalculoMoratorios == 2) //INTERESES MES COMPLETO
                    {
                        while (FechaPago > FechaTemp)
                        {
                            FechaTemp = FechaTemp.AddMonths(1);
                            //int mes = FechaTemp.Month;
                            //int year = FechaTemp.Year;
                            //DateTime fecha = new DateTime(year, mes, DateTime.DaysInMonth(year, mes));
                            //FechaTemp = fecha;
                            Meses++;
                        }
                        if (Meses > 0)
                        {
                            Moratorios = recibo.PagoCapital * (recibo.PctInteresMoratorio / 100) * Meses;
                        }

                    }
                    else
                    {
                        TimeSpan tiempo = Convert.ToDateTime(FechaPago) - Convert.ToDateTime(recibo.FechaVencimientoMoratorios);
                        Dias = tiempo.Days;
                        if (Dias > 0)
                        {
                            Moratorios = recibo.PagoCapital * (recibo.PctInteresMoratorio / 100) / 30.4m * Dias;
                        }

                    }
                    recibo.InteresesMoratorios = Moratorios;

                    recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    //recibo.PagoParcial = reader["CAMPO_NUM4"] == DBNull.Value ? 0 : (decimal)reader["CAMPO_NUM4"];
                    recibo.NombreConjunto = reader["CAMPO5"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    //recibo.TipoDoc = reader["P2426_TIPO_DOC"].ToString();                    
                    recibo.Importe = reader["P0712_IMPORTE_RENTA"] == DBNull.Value ? 0 : (decimal)reader["P0712_IMPORTE_RENTA"];
                    recibo.Comentario = reader["P2407_COMENTARIO"].ToString();
                    listCntasxCobrar.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listCntasxCobrar;
            }
            catch(Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        //EDIT BY UZ 02/05/2019
        public static List<ReciboEntity> GetListaCtasxCobrar(string idArre, string idConjunto, DateTime fechacorte)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"Select P2402_ID_ARRENDATARIO, P2411_N_ARRENDATARIO, T07_EDIFICIO.P0703_NOMBRE, T24_HISTORIA_RECIBOS.CAMPO_DATE1, T24_HISTORIA_RECIBOS.CAMPO_DATE2, P2408_FECHA_PAGADO, P2404_PERIODO, 
P2412_CONCEPTO, P2419_TOTAL, P2405_IMPORTE, T24_HISTORIA_RECIBOS.CAMPO5, T24_HISTORIA_RECIBOS.CAMPO_NUM4, T24_HISTORIA_RECIBOS.P2410_MONEDA,T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, 
P0712_IMPORTE_RENTA, T24_HISTORIA_RECIBOS.P2407_COMENTARIO, P2427_CTD_PAG, T07_EDIFICIO.CAMPO5 AS ID_MZNA, T07_EDIFICIO.CAMPO6 AS ID_LOTE,  
T24_HISTORIA_RECIBOS.CAMPO10 AS PCT_INTERES_MORATORIO,  T01_ARRENDADORA.CAMPO_NUM1 AS TIPO_CALCULO, T24_HISTORIA_RECIBOS.CAMPO_NUM1, P2444_ID_HIST_REC  
from T24_HISTORIA_RECIBOS  
Inner Join T04_CONTRATO On (T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO = T04_CONTRATO.P0401_ID_CONTRATO) 
Inner Join T07_EDIFICIO On (T04_CONTRATO.P0404_ID_EDIFICIO = T07_EDIFICIO.P0701_ID_EDIFICIO) 
JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR = T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA 
Where T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA =  ? 
AND T24_HISTORIA_RECIBOS.CAMPO4 =  ? 
and T24_HISTORIA_RECIBOS.CAMPO_DATE1 <= ?    
and T24_HISTORIA_RECIBOS.P2426_TIPO_DOC in ('A','E','M', 'V', 'Z' ) 
And T24_HISTORIA_RECIBOS.P2406_STATUS =  '1'   
Order By T24_HISTORIA_RECIBOS.P2411_N_ARRENDATARIO, T07_EDIFICIO.P0703_NOMBRE, T24_HISTORIA_RECIBOS.CAMPO_DATE1   "; 
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idArr", OdbcType.VarChar).Value = idArre;
                comando.Parameters.Add("@idConj", OdbcType.VarChar).Value = idConjunto;
                comando.Parameters.Add("@fechacorte", OdbcType.Date).Value = fechacorte;
                List<ReciboEntity> listCntasxCobrar = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    recibo.NombreCliente = reader["P2411_N_ARRENDATARIO"].ToString();
                    recibo.NombreInmueble = reader["P0703_NOMBRE"].ToString();
                    recibo.VencimientoPago = reader["CAMPO_DATE1"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CAMPO_DATE1"]);
                    recibo.FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.Periodo = reader["P2404_PERIODO"].ToString();
                    recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                    recibo.IdManzana = reader["ID_MZNA"].ToString();
                    recibo.IdLote = reader["ID_LOTE"].ToString();
                    recibo.TipoCalculoMoratorios = reader["TIPO_CALCULO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TIPO_CALCULO"]);
                    recibo.PctInteresMoratorio = reader["PCT_INTERES_MORATORIO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PCT_INTERES_MORATORIO"]);
                    try
                    {
                        recibo.FechaVencimientoMoratorios = Convert.ToDateTime(reader["CAMPO_DATE2"]);
                    }
                    catch
                    {
                        recibo.FechaVencimientoMoratorios = fechacorte;
                    }
                    //recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    recibo.importePorPagar = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"];
                    recibo.TipoDoc = reader["P2426_TIPO_DOC"].ToString();
                    if (recibo.TipoDoc == "E")
                    {
                        recibo.PagoCapital = reader["CAMPO_NUM4"] == DBNull.Value ? 0 : (decimal)reader["CAMPO_NUM4"];
                    }
                    else if (recibo.TipoDoc == "V")
                    {
                        recibo.PagoCapital = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    }
                    else if (recibo.TipoDoc == "Z")
                    {
                        recibo.PagoCapital = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    }
                    else
                    {
                        recibo.PagoCapital = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    }
                    int Meses = 0;
                    int Dias = 0;
                    decimal Moratorios = 0;
                    DateTime FechaTemp = Convert.ToDateTime(recibo.FechaVencimientoMoratorios);
                    DateTime FechaPago = new DateTime();
                    string idhistrec = reader["P2444_ID_HIST_REC"].ToString(); ;
                   
                    FechaPago = fechacorte;                   

                    if(recibo.TipoCalculoMoratorios == 2) //INTERESES MES COMPLETO
                    {
                        while(FechaPago> FechaTemp)
                        {
                            FechaTemp = FechaTemp.AddMonths(1);
                            //int mes = FechaTemp.Month;
                            //int year = FechaTemp.Year;
                            //DateTime fecha = new DateTime(year, mes, DateTime.DaysInMonth(year, mes));
                            //FechaTemp = fecha;
                            Meses++;
                        }
                        if(Meses >0)
                        {
                            Moratorios = recibo.PagoCapital * (recibo.PctInteresMoratorio / 100) * Meses;
                        }
                        
                    }
                    else
                    {                       
                        TimeSpan tiempo =  Convert.ToDateTime(FechaPago) - Convert.ToDateTime( recibo.FechaVencimientoMoratorios);
                        Dias = tiempo.Days;
                        if(Dias>0)
                        {
                            Moratorios = recibo.PagoCapital * (recibo.PctInteresMoratorio / 100) / 30.4m * Dias;
                        }
                       
                    }
                    recibo.InteresesMoratorios = Moratorios;
                    
                    //recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    
                    recibo.NombreConjunto = reader["CAMPO5"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                                        
                    recibo.Importe = reader["P0712_IMPORTE_RENTA"] == DBNull.Value ? 0 : (decimal)reader["P0712_IMPORTE_RENTA"];
                    recibo.Comentario = reader["P2407_COMENTARIO"].ToString();
                    listCntasxCobrar.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listCntasxCobrar;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }

        #region REPORTE ANALISIS DE DEUDORES (REPORTE GLOBAL)
        //1
        public static List<ReporteGlobalEntity> getContratosPorInmobiliariayConjunto(string idInmobiliaria, string idConjunto, DateTime fechaCorte, DateTime fechaIniPer, DateTime fechaFinPer)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"
SELECT DISTINCT P0401_ID_CONTRATO,P0403_ID_ARRENDADORA,P0301_ID_CENTRO,P1801_ID_SUBCONJUNTO,P0701_ID_EDIFICIO,P0402_ID_ARRENDAT,P0703_NOMBRE,P0707_CONTRUCCION_M2,P0203_NOMBRE,P0425_ACTIVIDAD,P0253_NOMBRE_COMERCIAL,P0434_IMPORTE_ACTUAL,P0410_INICIO_VIG,
P0411_FIN_VIG,T04_CONTRATO.P0407_MONEDA_FACT,P0418_IMPORTE_DEPOSITO,P3105_DIAS_GRACIA_RENTA
FROM T04_CONTRATO
LEFT JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T04_CONTRATO.P0404_ID_EDIFICIO
LEFT JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T07_EDIFICIO.P0710_ID_CENTRO
LEFT JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID= T04_CONTRATO.P0402_ID_ARRENDAT
LEFT JOIN T31_CONFIGURACION ON T31_CONFIGURACION.P3102_VALOR = T04_CONTRATO.P0401_ID_CONTRATO
LEFT JOIN T18_SUBCONJUNTOS on T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO 
WHERE T04_CONTRATO.P0403_ID_ARRENDADORA = ?";

            if (idConjunto != "Todos")
            {
                sql += @"
AND P0301_ID_CENTRO = ? AND T04_CONTRATO.P0437_TIPO = 'R'
UNION
SELECT DISTINCT P0401_ID_CONTRATO,P0403_ID_ARRENDADORA,P0301_ID_CENTRO,P1801_ID_SUBCONJUNTO,P0701_ID_EDIFICIO,P0402_ID_ARRENDAT,P0703_NOMBRE,P0707_CONTRUCCION_M2,P0203_NOMBRE,P0425_ACTIVIDAD,P0253_NOMBRE_COMERCIAL,P0434_IMPORTE_ACTUAL,P0410_INICIO_VIG,
P0411_FIN_VIG,T04_CONTRATO.P0407_MONEDA_FACT,P0418_IMPORTE_DEPOSITO,P3105_DIAS_GRACIA_RENTA
FROM T04_CONTRATO
LEFT JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T04_CONTRATO.P0404_ID_EDIFICIO
LEFT JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T07_EDIFICIO.P0710_ID_CENTRO
LEFT JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID= T04_CONTRATO.P0402_ID_ARRENDAT
LEFT JOIN T31_CONFIGURACION ON T31_CONFIGURACION.P3102_VALOR = T04_CONTRATO.P0401_ID_CONTRATO
LEFT JOIN T18_SUBCONJUNTOS on T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO 
WHERE T04_CONTRATO.P0403_ID_ARRENDADORA = ?        
AND P0301_ID_CENTRO = ? or T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = ? AND T04_CONTRATO.P0437_TIPO = 'R' ORDER BY 7
";

            }
            else
            {
                sql += " AND T04_CONTRATO.P0437_TIPO = 'R' ORDER BY T07_EDIFICIO.P0703_NOMBRE";
            }
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmo", OdbcType.VarChar).Value = idInmobiliaria;
                if (idConjunto != "Todos")
                {
                    comando.Parameters.Add("@IdConjunto", OdbcType.VarChar).Value = idConjunto;
                    comando.Parameters.Add("@IDInmo2", OdbcType.VarChar).Value = idInmobiliaria;
                    comando.Parameters.Add("@IdConjunto2", OdbcType.VarChar).Value = idConjunto;
                    comando.Parameters.Add("@IdConjunto3", OdbcType.VarChar).Value = idConjunto;
                }
                List<ReporteGlobalEntity> listaContratos = new List<ReporteGlobalEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReporteGlobalEntity contrato = new ReporteGlobalEntity()
                    {
                        IdContrato = reader["P0401_ID_CONTRATO"].ToString(),
                        idInmo = reader["P0403_ID_ARRENDADORA"].ToString(),
                        IdEdificio = reader["P0701_ID_EDIFICIO"].ToString(),
                        IdCliente = reader["P0402_ID_ARRENDAT"].ToString(),
                        idConjunto = reader["P0301_ID_CENTRO"].ToString(),
                        idSubConjunto = reader["P1801_ID_SUBCONJUNTO"].ToString(),
                        NombreInmueble = reader["P0703_NOMBRE"].ToString(),
                        M2Construccion = reader["P0707_CONTRUCCION_M2"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P0707_CONTRUCCION_M2"]),
                        NombreCliente = reader["P0203_NOMBRE"].ToString(),
                        Actividad = reader["P0425_ACTIVIDAD"].ToString(),
                        NombreComercial = reader["P0253_NOMBRE_COMERCIAL"].ToString(),
                        RentaActual = reader["P0434_IMPORTE_ACTUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P0434_IMPORTE_ACTUAL"]),
                        InicioVigencia = (DateTime)reader["P0410_INICIO_VIG"], //== DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P0410_INICIO_VIG"]),
                        FinVigencia = (DateTime)reader["P0411_FIN_VIG"], // == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P0411_FIN_VIG"]),
                        Deposito = reader["P0418_IMPORTE_DEPOSITO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P0418_IMPORTE_DEPOSITO"]),
                        DiasGracia = reader["P3105_DIAS_GRACIA_RENTA"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P3105_DIAS_GRACIA_RENTA"]),
                        MonedaContrato = reader["P0407_MONEDA_FACT"].ToString()
                    };
                    #region variable complemtnto
                    bool usarComplemto = false;
                    try
                    {
                        usarComplemto = Properties.Settings.Default.usarSinComplementos;
                    }
                    catch
                    {
                        usarComplemto = false;
                    }
                    #endregion

                    #region FACTURACION DEL MES CORRIENTE
                    try
                    {
                        contrato.RecibosPorContrato = SaariDB.getRecibosdeRentaAndCargos(idInmobiliaria, contrato.IdContrato, fechaIniPer, fechaCorte, usarComplemto);
                        foreach (ReciboEntity recibo in contrato.RecibosPorContrato)
                        {
                            //para convertir factura de acuerdo a la moneda del contrato
                            if (contrato.MonedaContrato == "P" && recibo.Moneda == "D")
                            {
                                recibo.Importe = recibo.Importe * recibo.TipoCambio;
                                recibo.IVA = recibo.IVA * recibo.TipoCambio;
                                recibo.IVARetenido = recibo.IVARetenido * recibo.TipoCambio;
                                recibo.ISR = recibo.ISR * recibo.TipoCambio;
                                recibo.Descuento = recibo.Descuento * recibo.TipoCambio;
                                recibo.Cargo = recibo.Cargo * recibo.TipoCambio;
                            }
                            else if (contrato.MonedaContrato == "D" && recibo.Moneda == "P")
                            {
                                recibo.Importe = recibo.Importe / recibo.TipoCambio;
                                recibo.IVA = recibo.IVA / recibo.TipoCambio;
                                recibo.IVARetenido = recibo.IVARetenido / recibo.TipoCambio;
                                recibo.ISR = recibo.ISR / recibo.TipoCambio;
                                recibo.Descuento = recibo.Descuento / recibo.TipoCambio;
                                recibo.Cargo = recibo.Cargo / recibo.TipoCambio;
                            }
                        }

                        contrato.SaldoAFavorPeriodo = contrato.RecibosPorContrato.FindAll(sf => sf.IDPago != sf.IDPago).Sum(s => s.saldoAFavor);
                    }
                    catch
                    { }

                    #endregion

                    #region Ultimo Abono
                    try
                    {
                        ReporteGlobalEntity ListaSumaPagadoyUltimoMesAbono = new ReporteGlobalEntity();
                        ListaSumaPagadoyUltimoMesAbono = SaariDB.getListaSumaAndUltimoAbono(idInmobiliaria, contrato.IdContrato, contrato.IdCliente, fechaIniPer.Date, fechaCorte.Date, false);
                        contrato.SumaAbonadoMes = ListaSumaPagadoyUltimoMesAbono.SumaAbonadoMes;
                        contrato.FechaUltimoAbono = ListaSumaPagadoyUltimoMesAbono.FechaUltimoAbono; //SaariDB.getFechaUltimoAbono(idInmobiliaria, contrato.IdContrato, fechaIniPer.Date, fechaCorte.Date).FechaUltimoAbono;//ListaSumaPagadoyUltimoMesAbono.FechaUltimoAbono; ;
                    }
                    catch
                    {
                    }
                    #endregion

                    #region Saldo a favor TOTAL POR CLIENTE
                    List<SaldoAFavorEntity> ListSaldo = new List<SaldoAFavorEntity>();
                    try
                    {
                        contrato.SaldoAFavor = SaariE.getSaldoAFavor(contrato.IdCliente);
                    }
                    catch
                    {
                        contrato.SaldoAFavor = 0;
                    }
                    #endregion

                    #region Sub Conjunto                    
                    if (string.IsNullOrEmpty(contrato.IdEdificio))
                    {
                        if (contrato.idSubConjunto.Contains("SCNJ"))
                        {
                            contrato.esSubconjunto = true;
                            contrato.existeSubconjunto = true;
                        }
                        //else
                        //{
                        //    contrato.IdEdificio = getIdInmueblePorContrato(contrato.IdContrato);
                        //    contrato.esSubconjunto = true;
                        //    contrato.existeSubconjunto = existeSubconjuntoByID(contrato.IdEdificio);
                        //}
                        if (contrato.existeSubconjunto)
                            contrato.NombreInmueble = getNombreSubConjuntoByIDSub(contrato.idSubConjunto);
                    }
                    #endregion

                    #region metros subbconjunto
                    try
                    {
                        if (contrato.esSubconjunto && contrato.existeSubconjunto)
                        {
                            contrato.M2Construccion = sumaM2ConstruccionSubconjunto(contrato.idSubConjunto).M2Construccion;
                            contrato.RentaActual = ValorRentaSubconjunto(contrato.idSubConjunto).RentaActual;
                        }
                        contrato.CostoM2 = contrato.RentaActual / contrato.M2Construccion;
                    }
                    catch
                    {
                        contrato.CostoM2 = 0;
                    }

                    if (contrato.esSubconjunto && contrato.existeSubconjunto || !contrato.esSubconjunto && !contrato.existeSubconjunto)
                        listaContratos.Add(contrato);
                    #endregion
                }
                reader.Close();
                conexion.Close();
                return listaContratos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static decimal SumPagosParciales(decimal IdHistRec, string Idcontrato)
        {
            decimal totalPagos = 0;

            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT SUM (P2419_TOTAL)AS Total
FROM T24_HISTORIA_RECIBOS
where P2426_TIPO_DOC = 'P' AND P2403_NUM_RECIBO =  ? AND P2418_ID_CONTRATO = ?   ";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idHistRec", OdbcType.Int).Value = IdHistRec;
                comando.Parameters.Add("@idContrato", OdbcType.NVarChar).Value = Idcontrato;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    totalPagos = reader["Total"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Total"]);

                }
                reader.Close();
                conexion.Close();
                return totalPagos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return 0;
            }
        }
        //HISTORIAL RECIBOS ANALISIS DEUDORES
        public static List<ReciboEntity> getListaRecibosPendientesPagoContrato(string IDContrato, DateTime fechaCorte)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,
                                    T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2419_TOTAL, 
                                    T24_HISTORIA_RECIBOS.P2413_PAGO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T24_HISTORIA_RECIBOS.P2421_TC_PAGO, T24_HISTORIA_RECIBOS.P2404_PERIODO,
                                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA,
                                    T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO 
                                    FROM T24_HISTORIA_RECIBOS
                                    LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
                                    WHERE P2426_TIPO_DOC IN ('R','Z','X','W','O') AND P2409_FECHA_EMISION <= ? AND P2418_ID_CONTRATO = ? AND P2406_STATUS = '1'
                                    ORDER BY P2409_FECHA_EMISION, P4006_SERIE, P4007_FOLIO";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@FechaCorte", OdbcType.DateTime).Value = fechaCorte.Date;
                        comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = IDContrato;
                        List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReciboEntity recibo = new ReciboEntity()
                                {
                                    IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                                    Numero = (int)reader["P2403_NUM_RECIBO"],
                                    FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                                    FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                                    TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                                    Moneda = reader["P2410_MONEDA"].ToString(),
                                    Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]),
                                    Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]),
                                    Estatus = reader["P2406_STATUS"].ToString(),
                                    PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2427_CTD_PAG"]),
                                    TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2421_TC_PAGO"]),
                                    Periodo = reader["P2404_PERIODO"].ToString(),
                                    Serie = reader["P4006_SERIE"].ToString(),
                                    Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["P4007_FOLIO"]),
                                    MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                                    Concepto = reader["P2412_CONCEPTO"].ToString(),
                                    Inmueble = reader["CAMPO9"].ToString(),
                                    IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                                    TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"])
                                };
                                if (recibo.Estatus != "3" || (recibo.Estatus == "3" && recibo.FechaPago > fechaCorte.Date))
                                {
                                    if (recibo.Moneda == "P")
                                        recibo.Cargo = recibo.Total;
                                    else
                                        recibo.Cargo = recibo.Total * recibo.TipoCambio;
                                    if (recibo.TipoDoc == "R" || recibo.TipoDoc == "Z")
                                    {
                                        if (recibo.Estatus == "2" && (recibo.FechaPago ?? DateTime.Now) <= fechaCorte.Date)
                                        {
                                            recibo.Abono = recibo.Cargo;
                                            recibo.Pagos = new List<ReciboEntity>();
                                        }
                                        else
                                        {
                                            recibo.Pagos = getNotasCredito(recibo.IDHistRec, fechaCorte);
                                            recibo.FechaPago = null;
                                        }
                                    }
                                    else
                                    {
                                        List<ReciboEntity> abonos = new List<ReciboEntity>();
                                        var pagos = getPagosParciales(recibo.IDHistRec, fechaCorte);
                                        if (pagos != null)
                                        {
                                            if (pagos.Count <= 0 && recibo.Numero != 0)
                                                pagos = getPagosParciales(recibo.Numero, fechaCorte);
                                            if (pagos != null)
                                                pagos.ForEach(p => abonos.Add(p));
                                        }
                                        var notas = getNotasCredito(recibo.IDHistRec, fechaCorte);
                                        if (notas != null)
                                        {
                                            if (notas.Count <= 0 && recibo.Numero != 0)
                                                notas = getNotasCredito(recibo.Numero, fechaCorte);
                                            if (notas != null)
                                                notas.ForEach(n => abonos.Add(n));
                                        }
                                        recibo.Pagos = abonos;
                                    }
                                    listaRecibos.Add(recibo);
                                }
                            }
                        }
                        return listaRecibos;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        //1
        public static List<SaldoEntity> getSaldoInicioOperaciones(ContratosEntity contrato, DateTime fechaIni, bool hayDolares)
        {
            //int mes = fechaIni.AddMonths(-1).Month;
            //int ultimoDia = DateTime.DaysInMonth(fechaIni.Year, mes);
            //fechaIni = new DateTime(fechaIni.Year, mes, ultimoDia);
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT P2403_NUM_RECIBO, P2419_TOTAL, P2410_MONEDA, P2426_TIPO_DOC, P2408_FECHA_PAGADO, P2420_MONEDA_PAGO,P2414_TIPO_CAMBIO, P2421_TC_PAGO, P2406_STATUS FROM T24_HISTORIA_RECIBOS
WHERE 
(P2401_ID_ARRENDADORA = ?
 AND P2402_ID_ARRENDATARIO = ? 
 AND P2418_ID_CONTRATO = ? 
 AND P2426_TIPO_DOC IN ('R','X','T','Z','W','O','L', 'G') 
 AND P2409_FECHA_EMISION < ? 
 AND (P2408_FECHA_PAGADO IS NULL OR P2408_FECHA_PAGADO >= ?) 
 AND P2406_STATUS IN ('1','2')
)
or
   (
   P2401_ID_ARRENDADORA = ?
   AND P2402_ID_ARRENDATARIO = ?
   AND P2418_ID_CONTRATO = ?
   AND P2426_TIPO_DOC IN ('R','X','T','Z','W','O','L','B','G') AND
   P2406_STATUS IN ('1')  AND P2409_FECHA_EMISION < ? 
   )
or
   (
   P2401_ID_ARRENDADORA = ?
   AND P2402_ID_ARRENDATARIO = ?
   AND P2418_ID_CONTRATO = ?
   AND P2426_TIPO_DOC IN ('T','L') AND
   P2406_STATUS IN ('0')  AND CAMPO_DATE1 < ?
   )
or
   (
 P2401_ID_ARRENDADORA = ?
 AND P2402_ID_ARRENDATARIO =? 
 AND P2418_ID_CONTRATO =? 
 AND P2426_TIPO_DOC IN ('B') 
 AND P2409_FECHA_EMISION <= ? 
 AND (P2408_FECHA_PAGADO IS NULL OR P2408_FECHA_PAGADO <=?) 
 AND P2406_STATUS IN ('2')
)

ORDER BY T24_HISTORIA_RECIBOS.CAMPO5,
T24_HISTORIA_RECIBOS.P2411_N_ARRENDATARIO,
T24_HISTORIA_RECIBOS.CAMPO9,
T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaIni;//OJO
                comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte2", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt2", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni2", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDArr3", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte3", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt3", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni3", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDArr4", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@IDCte4", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt4", OdbcType.VarChar).Value = contrato.IDContrato;
                comando.Parameters.Add("@FechaIni4", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaIni4", OdbcType.DateTime).Value = fechaIni;
                List<SaldoEntity> listaSaldos = new List<SaldoEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    SaldoEntity saldo = new SaldoEntity();
                    saldo.NumeroRecibo = reader["P2403_NUM_RECIBO"] == DBNull.Value ? 0 : (int)reader["P2403_NUM_RECIBO"];
                    saldo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    saldo.Moneda = reader["P2410_MONEDA"].ToString();
                    saldo.TipoDocumento = reader["P2426_TIPO_DOC"].ToString();
                    saldo.FechaPagado = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    saldo.MonedaPago = reader["P2420_MONEDA_PAGO"].ToString();
                    saldo.TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : (decimal)reader["P2414_TIPO_CAMBIO"];
                    saldo.TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2421_TC_PAGO"];
                    saldo.Estatus = reader["P2406_STATUS"].ToString();
                    if (contrato.Moneda != saldo.MonedaPago)
                    {
                        if (saldo.Estatus == "2")
                        {
                            if (saldo.TipoCambioPago > 0)
                            {
                                if (contrato.Moneda == "P" && saldo.MonedaPago == "D")
                                    saldo.Total = (saldo.Total * saldo.TipoCambioPago);
                                else if (contrato.Moneda == "D" && saldo.MonedaPago == "P")
                                    saldo.Total = (saldo.Total / saldo.TipoCambioPago);
                            }
                        }

                    }
                    if (saldo.TipoDocumento == "B")
                        saldo.PagoParcial = 0 - saldo.Total;
                    else if (saldo.TipoDocumento == "X" || saldo.TipoDocumento == "W")
                        saldo.PagoParcial = saldo.Total;//- (getPagoParcial(contrato, fechaIni, saldo.NumeroRecibo));
                    else if (saldo.Estatus == "1")
                        saldo.PagoParcial = saldo.Total;
                    else if (saldo.FechaPagado >= fechaIni || saldo.FechaPagado == new DateTime(1901, 1, 1))
                        saldo.PagoParcial = saldo.Total;

                    listaSaldos.Add(saldo);
                }
                reader.Close();
                conexion.Close();
                return listaSaldos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getCargosFacturadosEnPeriodo(string idArrendadora, DateTime inicioPer, DateTime finPer)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            //            string sql = @"SELECT  COUNT(CAMPO20),P2418_ID_CONTRATO,P2402_ID_ARRENDATARIO,CAMPO20,P2419_TOTAL,P2409_FECHA_EMISION
            //                                        FROM T24_HISTORIA_RECIBOS
            //                                        WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ?
            //                                       AND CAMPO20 != ''  and T24_HISTORIA_RECIBOS.CAMPO20 is not null
            //                                        AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >=? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <=?
            //                                        group by CAMPO20,P2418_ID_CONTRATO,P2402_ID_ARRENDATARIO,CAMPO20,P2419_TOTAL,P2409_FECHA_EMISION";

            string sql = @"SELECT P2401_ID_ARRENDADORA,P2444_ID_HIST_REC, P2409_FECHA_EMISION, T35_HISTORIAL_CARGOS.P3401_ID_CARGO as CONTRATO, T35_HISTORIAL_CARGOS.P3402_CONCEPTO as Concepto,P3403_IMPORTE AS IMPORTE, T35_HISTORIAL_CARGOS.P3404_IVA AS IVA,
 T35_HISTORIAL_CARGOS.P3405_TOTAL as TOTAL, T35_HISTORIAL_CARGOS.CAMPO4 AS Cargo
FROM T24_HISTORIA_RECIBOS
JOIN T35_HISTORIAL_CARGOS ON P3409_ID_HIST_REC = P2444_ID_HIST_REC
WHERE P2401_ID_ARRENDADORA = ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? and T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?
AND T24_HISTORIA_RECIBOS.P2406_STATUS!= '3' AND T35_HISTORIAL_CARGOS.P3401_ID_CARGO NOT LIKE '%FACT%' 
union 
SELECT P2401_ID_ARRENDADORA,P2444_ID_HIST_REC, P2409_FECHA_EMISION,P2418_ID_CONTRATO, P2412_CONCEPTO,P2405_IMPORTE AS IMPORTE, P2416_IVA,
P2419_TOTAL, CAMPO20
FROM T24_HISTORIA_RECIBOS
WHERE P2401_ID_ARRENDADORA = ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? and T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ?
AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC ='Z'   and (CAMPO20 != ''  or T24_HISTORIA_RECIBOS.CAMPO20 is null)
";


            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IdContrato", OdbcType.VarChar).Value = idArrendadora;
                comando.Parameters.Add("@IniPer", OdbcType.DateTime).Value = inicioPer;
                comando.Parameters.Add("@FinPer", OdbcType.DateTime).Value = finPer;
                comando.Parameters.Add("@IdContrato2", OdbcType.VarChar).Value = idArrendadora;
                comando.Parameters.Add("@IniPer2", OdbcType.DateTime).Value = inicioPer;
                comando.Parameters.Add("@FinPer2", OdbcType.DateTime).Value = finPer;

                List<ReciboEntity> ListaCargosPer = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity CargoFac = new ReciboEntity()
                    {
                        IDContrato = reader["CONTRATO"].ToString(),
                        Concepto = reader["Concepto"].ToString(),
                        Importe = reader["IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["IMPORTE"]),
                        IVA = reader["IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["IVA"]),
                        Total = reader["TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL"]),
                        Campo20 = reader["Cargo"].ToString()


                    };

                    ListaCargosPer.Add(CargoFac);
                }


                reader.Close();
                conexion.Close();
                return ListaCargosPer;
            }
            catch
            {
                conexion.Close();
                return null;
            }

        }

        public static List<ReciboEntity> getNCByContratoXPeriodo(string idContrato, DateTime fechaInicio, DateTime fechaCorte)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,
                                    T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2410_MONEDA, CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2419_TOTAL * P2414_TIPO_CAMBIO) ELSE P2419_TOTAL END AS P2419_TOTAL, 
                                    T24_HISTORIA_RECIBOS.P2413_PAGO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T24_HISTORIA_RECIBOS.P2421_TC_PAGO, T24_HISTORIA_RECIBOS.P2404_PERIODO,
                                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, P2414_TIPO_CAMBIO
                                    FROM T24_HISTORIA_RECIBOS
                                    LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
                                    WHERE P2426_TIPO_DOC IN ('B') AND P2406_STATUS <> '3' AND T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO = ?  AND P2409_FECHA_EMISION >= ? AND P2409_FECHA_EMISION <= ?
                                    ORDER BY P2409_FECHA_EMISION, P4006_SERIE, P4007_FOLIO";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@idContrato", OdbcType.NVarChar).Value = idContrato;
                        comando.Parameters.Add("@FechaInicio", OdbcType.DateTime).Value = fechaInicio.Date;
                        comando.Parameters.Add("@FechaCorte", OdbcType.DateTime).Value = fechaCorte.Date;
                        List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReciboEntity recibo = new ReciboEntity()
                                {
                                    IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                                    Numero = (int)reader["P2403_NUM_RECIBO"],
                                    FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                                    FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                                    TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                                    Moneda = reader["P2410_MONEDA"].ToString(),
                                    Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]),
                                    Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]),
                                    Estatus = reader["P2406_STATUS"].ToString(),
                                    PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2427_CTD_PAG"]),
                                    TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2421_TC_PAGO"]),
                                    Periodo = reader["P2404_PERIODO"].ToString(),
                                    Serie = reader["P4006_SERIE"].ToString(),
                                    Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["P4007_FOLIO"]),
                                    MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                                    Concepto = reader["P2412_CONCEPTO"].ToString(),
                                    Inmueble = reader["CAMPO9"].ToString(),
                                    IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                                    TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"])
                                };
                                listaRecibos.Add(recibo);
                            }
                        }
                        return listaRecibos;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static ReporteGlobalEntity getFacturasLibres(bool ImporteSinIVA, bool ImporteConIVA, bool Abonado, DateTime fechaIni, DateTime fechaFin, string idArrendadora, string idCliente)
        {
            string sql = string.Empty;
            if (ImporteSinIVA)
            {
                sql = @"SELECT CASE WHEN SUM(P2405_IMPORTE) <> 0 THEN SUM(CASE WHEN P2410_MONEDA = 'D' THEN (P2405_IMPORTE * P2414_TIPO_CAMBIO) ELSE P2405_IMPORTE END) ELSE 0 END AS importe FROM T24_HISTORIA_RECIBOS 
WHERE P2418_ID_CONTRATO LIKE '%FACT%'   AND P2406_STATUS in ('2','1') And P2401_ID_ARRENDADORA = ? AND P2402_ID_ARRENDATARIO = ? 
AND P2409_FECHA_EMISION >= ? AND P2409_FECHA_EMISION <= ?
";
            }
            else if (ImporteConIVA)
            {
                sql = @"SELECT CASE WHEN SUM(P2419_TOTAL) <> 0 THEN SUM(CASE WHEN P2410_MONEDA = 'D' THEN (P2419_TOTAL * P2414_TIPO_CAMBIO) ELSE P2419_TOTAL END) ELSE 0 END AS importe FROM T24_HISTORIA_RECIBOS 
WHERE P2418_ID_CONTRATO LIKE '%FACT%'   AND P2406_STATUSin ('2','1') And P2401_ID_ARRENDADORA = ? AND P2402_ID_ARRENDATARIO = ?
AND P2409_FECHA_EMISION >= ? AND P2409_FECHA_EMISION <= ?";

            }
            else
            {
                sql = @"SELECT CASE WHEN SUM(P2419_TOTAL) <> 0 THEN SUM(CASE WHEN P2410_MONEDA = 'D' THEN (P2419_TOTAL * P2414_TIPO_CAMBIO) ELSE P2419_TOTAL END) ELSE 0 END AS importe FROM T24_HISTORIA_RECIBOS 
WHERE P2418_ID_CONTRATO LIKE '%FACT%'   AND P2406_STATUS = '1' And P2401_ID_ARRENDADORA = ? AND P2402_ID_ARRENDATARIO = ?
AND P2409_FECHA_EMISION >= ? AND P2409_FECHA_EMISION <= ?";

            }
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArrendadora;
                comando.Parameters.Add("@idCliente", OdbcType.VarChar).Value = idCliente;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;

                ReporteGlobalEntity listaRecibos = new ReporteGlobalEntity();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReporteGlobalEntity recibo = new ReporteGlobalEntity()
                    {
                        SumaAbonadoMes = reader["importe"] == DBNull.Value ? 0 : (decimal)reader["importe"],
                    };

                    listaRecibos = recibo;
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        //1
        public static ReporteGlobalEntity getListaSumaAndUltimoAbono(string idArrendadora, string idContrato, string idCliente, DateTime fechaIni, DateTime fechaFin, bool ContratosDolares)
        {

            string sql = string.Empty;
            if (!ContratosDolares)
            {
                sql = @"SELECT  P2418_ID_CONTRATO,SUM(P2413_PAGO)AS SumaTotalPagado, MAX(P2408_FECHA_PAGADO) AS FechaPago
FROM T24_HISTORIA_RECIBOS WHERE  P2401_ID_ARRENDADORA = ? AND T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO NOT LIKE '%FACT%' 
AND T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO = ? AND P2408_FECHA_PAGADO>= ? and P2408_FECHA_PAGADO <= ?
AND T24_HISTORIA_RECIBOS.P2406_STATUS = '2' 
GROUP BY P2418_ID_CONTRATO ";
            }
            else
            {
                sql = @"SELECT  CASE WHEN SUM(P2413_PAGO) <> 0 THEN SUM(CASE WHEN P2420_MONEDA_PAGO = 'D' THEN (P2413_PAGO * P2421_TC_PAGO) ELSE P2413_PAGO END) ELSE 0 END  AS SumaTotalPagado ,MAX(P2408_FECHA_PAGADO) AS FechaPago,P2418_ID_CONTRATO FROM T24_HISTORIA_RECIBOS 
WHERE P2401_ID_ARRENDADORA = ? AND P2402_ID_ARRENDATARIO = ? AND P2418_ID_CONTRATO = ? And P2426_TIPO_DOC != 'B' and T24_HISTORIA_RECIBOS.P2406_STATUS != '3' AND P2408_FECHA_PAGADO >= ?
AND P2408_FECHA_PAGADO <= ? 
GROUP BY P2418_ID_CONTRATO";

            }
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArrendadora;
                if (ContratosDolares)
                    comando.Parameters.Add("@idCliente", OdbcType.VarChar).Value = idCliente;
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idContrato;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;

                ReporteGlobalEntity listaRecibos = new ReporteGlobalEntity();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReporteGlobalEntity recibo = new ReporteGlobalEntity()
                    {
                        IdContrato = reader["P2418_ID_CONTRATO"].ToString(),
                        SumaAbonadoMes = reader["SumaTotalPagado"] == DBNull.Value ? 0 : (decimal)reader["SumaTotalPagado"],
                        FechaUltimoAbono = reader["FechaPago"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["FechaPago"])
                    };

                    listaRecibos = recibo;

                    //listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static ReporteGlobalEntity getFechaUltimoAbono(string idArrendadora, string idContrato, DateTime fechaIni, DateTime fechaFin)
        {

            string sql = string.Empty;

            sql = @"SELECT  P2418_ID_CONTRATO, MAX(P2408_FECHA_PAGADO) AS FechaPago
FROM T24_HISTORIA_RECIBOS WHERE  P2401_ID_ARRENDADORA = ? AND T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO NOT LIKE '%FACT%' 
AND T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO = ? AND P2408_FECHA_PAGADO <= ?
AND T24_HISTORIA_RECIBOS.P2406_STATUS = '2' 
GROUP BY P2418_ID_CONTRATO ";

            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArrendadora;
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idContrato;
                //comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;

                ReporteGlobalEntity listaRecibos = new ReporteGlobalEntity();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReporteGlobalEntity recibo = new ReporteGlobalEntity()
                    {
                        FechaUltimoAbono = reader["FechaPago"] == DBNull.Value ? new DateTime(1999, 1, 1) : Convert.ToDateTime(reader["FechaPago"])
                    };

                    listaRecibos = recibo;
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static ReporteGlobalEntity InteresRecibosPendientesPago(string idArrendadora, string idContrato, string idCliente, DateTime fechaCorte, string monedaContrato)
        {
            string sql = string.Empty;

            if (monedaContrato == "P")
            {
                sql = @"SELECT  CASE WHEN SUM(CAMPO_NUM5) <> 0 THEN SUM(CASE WHEN P2410_MONEDA = 'D' THEN (CAMPO_NUM5 * P2414_TIPO_CAMBIO) ELSE CAMPO_NUM5 END) ELSE 0 END  AS SumaIntereses,P2418_ID_CONTRATO FROM T24_HISTORIA_RECIBOS 
WHERE P2401_ID_ARRENDADORA = ? AND P2402_ID_ARRENDATARIO = ? AND P2418_ID_CONTRATO = ? and T24_HISTORIA_RECIBOS.P2406_STATUS = '1' AND P2409_FECHA_EMISION <= ?
GROUP BY P2418_ID_CONTRATO";
            }
            else
            {
                sql = @"SELECT  CASE WHEN SUM(CAMPO_NUM5) <> 0 THEN SUM(CASE WHEN P2410_MONEDA = 'P' THEN (CAMPO_NUM5 / P2414_TIPO_CAMBIO) ELSE CAMPO_NUM5 END) ELSE 0 END  AS SumaIntereses,P2418_ID_CONTRATO FROM T24_HISTORIA_RECIBOS 
WHERE P2401_ID_ARRENDADORA = ? AND P2402_ID_ARRENDATARIO = ? AND P2418_ID_CONTRATO = ? and T24_HISTORIA_RECIBOS.P2406_STATUS = '1' AND P2409_FECHA_EMISION <= ?
GROUP BY P2418_ID_CONTRATO";

            }
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArrendadora;
                comando.Parameters.Add("@idCliente", OdbcType.VarChar).Value = idCliente;
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idContrato;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaCorte;


                ReporteGlobalEntity listaRecibos = new ReporteGlobalEntity();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReporteGlobalEntity recibo = new ReporteGlobalEntity()
                    {
                        IdContrato = reader["P2418_ID_CONTRATO"].ToString(),
                        SumaIntereses = reader["SumaIntereses"] == DBNull.Value ? 0 : (decimal)reader["SumaIntereses"]
                    };

                    listaRecibos = recibo;
                    //listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getListaRecibosReporteGlobal(ReporteGlobalEntity contrato, DateTime fechaIni, DateTime fechaFin)
        {/*
          * UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, z.* 
FROM T24_HISTORIA_RECIBOS z
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = z.P2444_ID_HIST_REC
WHERE z.p2426_tipo_doc in ('P','U') AND z.P2406_STATUS = 2 
AND z.P2401_ID_ARRENDADORA = ?
AND (z.P2409_FECHA_EMISION <= ?)
AND (z.P2408_FECHA_PAGADO >= ? 
AND z.P2408_FECHA_PAGADO <= ?)
AND (z.P2402_ID_ARRENDATARIO = ?)
AND (z.P2418_ID_CONTRATO = ?)
          */
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, T24_HISTORIA_RECIBOS.* FROM T24_HISTORIA_RECIBOS
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
WHERE P2406_STATUS IN ('2') AND P2401_ID_ARRENDADORA = ?  AND (T24_HISTORIA_RECIBOS.P2407_COMENTARIO != 'CFDi IMPORTADO DESDE XL' or T24_HISTORIA_RECIBOS.P2407_COMENTARIO is null )
AND p2426_tipo_doc IN ('X','W')
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2408_FECHA_PAGADO >= ?)
AND (P2402_ID_ARRENDATARIO = ?)
AND (P2418_ID_CONTRATO = ?)
UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, T24_HISTORIA_RECIBOS.* FROM T24_HISTORIA_RECIBOS
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
WHERE P2406_STATUS IN ('1') AND P2401_ID_ARRENDADORA = ?  AND (T24_HISTORIA_RECIBOS.P2407_COMENTARIO != 'CFDi IMPORTADO DESDE XL' or T24_HISTORIA_RECIBOS.P2407_COMENTARIO is null )
AND p2426_tipo_doc IN ('X','W', 'L')
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2402_ID_ARRENDATARIO = ?)
AND (P2418_ID_CONTRATO = ?)
UNION
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, r.* FROM T24_HISTORIA_RECIBOS r 
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC= r.P2444_ID_HIST_REC
WHERE EXISTS (SELECT * FROM T24_HISTORIA_RECIBOS u WHERE r.P2444_ID_HIST_REC=u.P2425_DEB_REC AND  (r.P2407_COMENTARIO != 'CFDi IMPORTADO DESDE XL' or r.P2407_COMENTARIO is null))
AND r.P2426_TIPO_DOC IN ('X') AND r.P2406_STATUS IN ('1') AND P2401_ID_ARRENDADORA = ?
AND (P2409_FECHA_EMISION <= ?  AND P2409_FECHA_EMISION >= ?)
AND (P2402_ID_ARRENDATARIO = ?)
AND (P2418_ID_CONTRATO = ?) " +
        //Aqui va el codigo del UNION                             
        @"UNION ALL
SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T40_CFD.P4023_CAMPO1, y.* FROM T24_HISTORIA_RECIBOS y
LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = y.P2444_ID_HIST_REC
WHERE y.P2401_ID_ARRENDADORA = ?
AND
   (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('1')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('2')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2408_Fecha_pagado >= ?
   AND y.P2408_Fecha_pagado <= ?
   )
   OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('2')
   AND y.P2426_TIPO_DOC IN ('R','B','Z','T','D')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )
OR
   (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('0','3')
   AND y.P2426_TIPO_DOC IN ('T')
   AND y.CAMPO_DATE1  >= ?
   AND y.CAMPO_DATE1 <= ?
   )
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('2', '5', '4')
   AND y.P2426_TIPO_DOC IN ('G')
   AND y.P2408_Fecha_pagado >= ?
   AND y.P2408_Fecha_pagado <= ?
   )  
OR (
   y.P2402_ID_ARRENDATARIO = ?
   AND y.P2418_ID_CONTRATO = ?
   AND y.P2406_STATUS IN ('1')
   AND y.P2426_TIPO_DOC IN ('G')
   AND y.P2409_Fecha_emision >= ?
   AND y.P2409_Fecha_emision <= ?
   )       
order by 11, 1, 2, 8, 28";
            //Se agrega el ultimo para recibos tipo G para depositos en garantía by UZ 15/12/2015
            //Se quitan los recibos tipo T con Status 3 (recibos temporales de convenio de pago cancelados) by Uz 27/05/2016
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.idInmo;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaIni2", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IdCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IdContrato;

                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.idInmo;
                comando.Parameters.Add("@FechaFinX", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIniX", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IdCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IdContrato;

                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = contrato.idInmo;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IdCliente;
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = contrato.IdContrato;
                /*
                comando.Parameters.Add("@IDArr2", OdbcType.VarChar).Value = contrato.IDArrendadora;
                comando.Parameters.Add("@FechaFin2", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@FechaIni3", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin3", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte2", OdbcType.VarChar).Value = contrato.IDCliente;
                comando.Parameters.Add("@IDCnt2", OdbcType.VarChar).Value = contrato.IDContrato;*/
                comando.Parameters.Add("@IDArr3", OdbcType.VarChar).Value = contrato.idInmo;
                comando.Parameters.Add("@IDCte3", OdbcType.VarChar).Value = contrato.IdCliente;
                comando.Parameters.Add("@IDCnt3", OdbcType.VarChar).Value = contrato.IdContrato;
                comando.Parameters.Add("@FechaIni4", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin4", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte4", OdbcType.VarChar).Value = contrato.IdCliente;
                comando.Parameters.Add("@IDCnt4", OdbcType.VarChar).Value = contrato.IdContrato;
                comando.Parameters.Add("@FechaIni5", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin5", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte5", OdbcType.VarChar).Value = contrato.IdCliente;
                comando.Parameters.Add("@IDCnt5", OdbcType.VarChar).Value = contrato.IdContrato;
                comando.Parameters.Add("@FechaIni6", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin6", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte6", OdbcType.VarChar).Value = contrato.IdCliente;
                comando.Parameters.Add("@IDCnt6", OdbcType.VarChar).Value = contrato.IdContrato;
                comando.Parameters.Add("@FechaIni7", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin7", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte7", OdbcType.VarChar).Value = contrato.IdCliente;
                comando.Parameters.Add("@IDCnt7", OdbcType.VarChar).Value = contrato.IdContrato;
                comando.Parameters.Add("@FechaIni8", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin8", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@IDCte8", OdbcType.VarChar).Value = contrato.IdCliente;
                comando.Parameters.Add("@IDCnt8", OdbcType.VarChar).Value = contrato.IdContrato;
                comando.Parameters.Add("@FechaIni9", OdbcType.DateTime).Value = fechaIni;
                comando.Parameters.Add("@FechaFin9", OdbcType.DateTime).Value = fechaFin;

                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity()
                    {
                        IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                        Numero = (int)reader["P2403_NUM_RECIBO"],
                        FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                        FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                        TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                        Moneda = reader["P2410_MONEDA"].ToString(),
                        Descuento = reader["P2424_DESCUENTO"] == DBNull.Value ? 0 : (decimal)reader["P2424_DESCUENTO"],
                        Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"],//Agregado para reporte Inmobic,Reporte Global                        
                        IVA = reader["P2416_IVA"] == DBNull.Value ? 0 : (decimal)reader["P2416_IVA"],//Agregado para reporte Inmobic,Reporte Global
                        Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"],
                        Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2413_PAGO"],
                        ISR = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : (decimal)reader["CAMPO_NUM1"],
                        IVARetenido = reader["CAMPO_NUM2"] == DBNull.Value ? 0 : (decimal)reader["CAMPO_NUM2"],
                        Estatus = reader["P2406_STATUS"].ToString(),
                        PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : (decimal)reader["P2427_CTD_PAG"],
                        TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : (decimal)reader["P2421_TC_PAGO"],
                        TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : (decimal)reader["P2414_TIPO_CAMBIO"],
                        Periodo = reader["P2404_PERIODO"].ToString(),
                        Serie = reader["P4006_SERIE"].ToString(),
                        Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],
                        MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                        Concepto = reader["P2412_CONCEPTO"].ToString(),
                        Comentario = reader["P2407_COMENTARIO"].ToString(),
                        Inmueble = reader["CAMPO9"].ToString(),
                        IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                        RutaPdfCFDI = reader["P4023_CAMPO1"] == DBNull.Value ? "" : reader["P4023_CAMPO1"].ToString(),
                        //Descuento = reader["P2424_DESCUENTO"] == DBNull.Value ? 0 : (decimal)reader["P2424_DESCUENTO"]
                    };

                    recibo.Importe = recibo.Importe - recibo.Descuento;
                    if (!string.IsNullOrEmpty(recibo.RutaPdfCFDI))
                    {
                        string ruta = "file:///" + recibo.RutaPdfCFDI.Replace(".xml", ".pdf");
                        ruta = ruta.Replace("\\", "\\\\");

                        recibo.RutaPdfCFDI = string.Empty;
                        recibo.RutaPdfCFDI = ruta;
                    }
                    //Condición agregada para depositos en garantia by Uz 15/12/2015
                    if (recibo.TipoDoc == "G")
                    {
                        recibo.Serie = "DEPGAR";
                        recibo.Folio = recibo.Numero;
                    }
                    //Condición agregada para cuando son recibos sin factura by Uz 15/12/2015
                    if (string.IsNullOrEmpty(recibo.Serie) && recibo.Folio == null)
                    {
                        recibo.Serie = "REC";
                        recibo.Folio = recibo.Numero;
                    }
                    if (recibo.FechaEmision >= fechaIni)
                    {
                        if (recibo.Moneda == "P")
                        {
                            if (recibo.TipoDoc == "R" || recibo.TipoDoc == "X" || recibo.TipoDoc == "Z" || recibo.TipoDoc == "D"
                                || recibo.TipoDoc == "T" || recibo.TipoDoc == "W" || recibo.TipoDoc == "G" || recibo.TipoDoc == "L")
                            {
                                if (contrato.MonedaContrato == "P")
                                    recibo.Cargo = recibo.Total;
                                else
                                    recibo.Cargo = recibo.Total / recibo.TipoCambio;
                            }
                            else if (recibo.TipoDoc == "B")
                            {
                                if (contrato.MonedaContrato == "P")
                                    recibo.Abono = recibo.Total;
                                else
                                    recibo.Abono = recibo.Total / recibo.TipoCambio;
                            }
                            if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                recibo.Abono = recibo.Cargo;
                        }
                        else
                        {
                            if (recibo.TipoDoc == "R" || recibo.TipoDoc == "X" || recibo.TipoDoc == "Z" || recibo.TipoDoc == "D"
                                || recibo.TipoDoc == "T" || recibo.TipoDoc == "W" || recibo.TipoDoc == "G" || recibo.TipoDoc == "L")
                            {
                                if (contrato.MonedaContrato == "D")
                                    recibo.Cargo = recibo.Total;
                                else
                                    recibo.Cargo = recibo.Total * recibo.TipoCambio;
                            }
                            else if (recibo.TipoDoc == "B")
                            {
                                if (contrato.MonedaContrato == "D")
                                    recibo.Abono = recibo.Total;
                                else
                                    recibo.Abono = recibo.Total * recibo.TipoCambio;
                            }
                            if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                recibo.Abono = recibo.Cargo;
                        }
                    }
                    if (recibo.Estatus == "2" || (recibo.TipoDoc == "G" && recibo.Estatus == "5"))
                    {
                        if (recibo.FechaPago <= fechaFin)
                        {
                            if (recibo.MonedaPago == "P")
                            {
                                if (contrato.MonedaContrato == "P")
                                {
                                    recibo.Abono = recibo.Pago;
                                    //if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                    //    recibo.Abono = recibo.PagoParcial;
                                    //else 
                                    if (recibo.TipoDoc == "G" && recibo.Estatus == "5")//Condición agregada para depositos en garantia by Uz 15/12/2015
                                        recibo.Abono = recibo.Total;
                                }
                                else
                                {
                                    recibo.Abono = recibo.Pago / recibo.TipoCambioPago;
                                    if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                        recibo.Abono = recibo.PagoParcial / recibo.TipoCambioPago;
                                    else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                        recibo.Abono = recibo.Total / recibo.TipoCambioPago;
                                }
                            }
                            else
                            {
                                if (contrato.MonedaContrato == "D")
                                {
                                    recibo.Abono = recibo.Pago;
                                    if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                        recibo.Abono = recibo.PagoParcial;
                                    else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                        recibo.Abono = recibo.Total;
                                }
                                else
                                {
                                    recibo.Abono = recibo.Pago * recibo.TipoCambioPago;
                                    if (recibo.TipoDoc == "T" && recibo.Estatus == "3")
                                        recibo.Abono = recibo.PagoParcial * recibo.TipoCambioPago;
                                    else if (recibo.TipoDoc == "G" && recibo.Estatus == "5")
                                        recibo.Abono = recibo.Total * recibo.TipoCambioPago;
                                }
                            }
                        }
                        else
                            recibo.FechaPago = null;
                    }
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getCargosFacturadosxIdHistRec(int idHistRec, string contrato, string cargo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = string.Empty;

            //                sql = @"Select T35_HISTORIAL_CARGOS.P3401_ID_CARGO, T35_HISTORIAL_CARGOS.P3402_CONCEPTO, T35_HISTORIAL_CARGOS.P3403_IMPORTE, T35_HISTORIAL_CARGOS.P3404_IVA,
            // T35_HISTORIAL_CARGOS.P3405_TOTAL, T35_HISTORIAL_CARGOS.CAMPO4, T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC
            //from T35_HISTORIAL_CARGOS
            //WHERE T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC = ? AND P3401_ID_CARGO= ? AND T35_HISTORIAL_CARGOS.CAMPO4 = ?";

            sql = @"Select T35_HISTORIAL_CARGOS.P3401_ID_CARGO, T35_HISTORIAL_CARGOS.P3402_CONCEPTO, T35_HISTORIAL_CARGOS.P3403_IMPORTE, T35_HISTORIAL_CARGOS.P3404_IVA,
 T35_HISTORIAL_CARGOS.P3405_TOTAL, T35_HISTORIAL_CARGOS.CAMPO4, T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC,P2414_TIPO_CAMBIO
from T35_HISTORIAL_CARGOS
JOIN T24_HISTORIA_RECIBOS on T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC
WHERE T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC = ? AND P3401_ID_CARGO= ? AND T35_HISTORIAL_CARGOS.CAMPO4 = ?";

            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idHistRec", OdbcType.Int).Value = idHistRec;
                comando.Parameters.Add("@Cargo", OdbcType.VarChar).Value = contrato;
                comando.Parameters.Add("@Cargo", OdbcType.VarChar).Value = cargo;

                List<ReciboEntity> ListaCargosPer = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity CargoFac = new ReciboEntity()
                    {
                        IDContrato = reader["P3401_ID_CARGO"].ToString(),
                        //IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString(),
                        Campo20 = reader["CAMPO4"].ToString(),
                        Importe = reader["P3403_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3403_IMPORTE"]),
                        IVA = reader["P3404_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3404_IVA"]),
                        TotalIVA = reader["P3405_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3405_TOTAL"]),
                        TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"]),
                        //FechaEmision = reader["CAMPO20"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"])

                        //Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],

                    };

                    ListaCargosPer.Add(CargoFac);
                }


                reader.Close();
                conexion.Close();
                return ListaCargosPer;
            }
            catch
            {
                conexion.Close();
                return null;
            }

        }

        public static List<ReciboEntity> getCargosFacturadosxIdHistRec(string idContrato, string cargo, DateTime Fechainicio, DateTime FechaFin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            string sql = @"Select T35_HISTORIAL_CARGOS.P3401_ID_CARGO, T35_HISTORIAL_CARGOS.P3402_CONCEPTO, T35_HISTORIAL_CARGOS.P3403_IMPORTE, T35_HISTORIAL_CARGOS.P3404_IVA,
 T35_HISTORIAL_CARGOS.P3405_TOTAL, T35_HISTORIAL_CARGOS.CAMPO4, T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC,P2414_TIPO_CAMBIO
from T35_HISTORIAL_CARGOS
JOIN T24_HISTORIA_RECIBOS ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC
WHERE P3401_ID_CARGO = ? AND T35_HISTORIAL_CARGOS.CAMPO4 =?
AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ? ";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idHistRec", OdbcType.VarChar).Value = idContrato;
                comando.Parameters.Add("@Cargo", OdbcType.VarChar).Value = cargo;
                comando.Parameters.Add("@fechaInicio", OdbcType.DateTime).Value = Fechainicio;
                comando.Parameters.Add("@fechaFin", OdbcType.DateTime).Value = FechaFin;


                List<ReciboEntity> ListaCargosPer = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity CargoFac = new ReciboEntity()
                    {
                        IDContrato = reader["P3401_ID_CARGO"].ToString(),
                        Campo20 = reader["CAMPO4"].ToString(),
                        Importe = reader["P3403_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3403_IMPORTE"]),
                        IVA = reader["P3404_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3404_IVA"]),
                        TotalIVA = reader["P3405_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3405_TOTAL"]),
                        TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"])

                    };

                    ListaCargosPer.Add(CargoFac);
                }


                reader.Close();
                conexion.Close();
                return ListaCargosPer;
            }
            catch
            {
                conexion.Close();
                return null;
            }

        }
        //1
        public static List<ReciboEntity> getMesesAdeudoByCliente(string idContrato, DateTime fechaCorte)
        {

            try
            {

                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {

                    string sql = @"SELECT T24_HISTORIA_RECIBOS.P2406_STATUS,P2419_TOTAL,P2427_CTD_PAG,P2444_ID_HIST_REC,P2403_NUM_RECIBO,P2426_TIPO_DOC ,CAMPO20 
FROM T24_HISTORIA_RECIBOS WHERE T24_HISTORIA_RECIBOS.P2406_STATUS = '1' AND  P2426_TIPO_DOC IN ('R','X')
AND T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO = ? AND  P2409_FECHA_EMISION <= ?
AND  (P2407_COMENTARIO!='CFDi IMPORTADO DESDE XL' OR P2407_COMENTARIO IS NULL)";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@idCliente", OdbcType.NVarChar).Value = idContrato;
                        comando.Parameters.Add("@fechaCorte", OdbcType.DateTime).Value = fechaCorte;
                        List<ReciboEntity> MesesAdeudo = new List<ReciboEntity>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReciboEntity Meses = new ReciboEntity();
                                Meses.Estatus = reader["P2406_STATUS"].ToString();
                                Meses.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]);
                                Meses.CtdPag = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2427_CTD_PAG"]);
                                Meses.IDHistRec = (int)reader["P2444_ID_HIST_REC"];
                                Meses.Numero = (int)reader["P2403_NUM_RECIBO"];
                                Meses.TipoDoc = reader["P2426_TIPO_DOC"].ToString();
                                Meses.Campo20 = reader["CAMPO20"].ToString();
                                MesesAdeudo.Add(Meses);
                            }

                        }
                        return MesesAdeudo;
                    }
                }
            }
            catch
            {
                return null;
            }



        }

        public static List<ReporteGlobalEntity> getFacturaslibres(string idInmobiliaria, DateTime fechainicio, DateTime FechaCorte)
        {
            List<ReporteGlobalEntity> listaRecibos = new List<ReporteGlobalEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT DISTINCT P2401_ID_ARRENDADORA,P2402_ID_ARRENDATARIO
FROM T24_HISTORIA_RECIBOS
WHERE P2418_ID_CONTRATO LIKE '%FACT%'   AND P2406_STATUS != '3' and P2401_ID_ARRENDADORA = ? AND P2426_TIPO_DOC in('R','X')
And P2409_FECHA_EMISION >= ? AND P2409_FECHA_EMISION <= ?
UNION
SELECT DISTINCT P2401_ID_ARRENDADORA,P2402_ID_ARRENDATARIO
FROM T24_HISTORIA_RECIBOS
WHERE P2418_ID_CONTRATO LIKE '%FACT%'   AND P2406_STATUS != '3' and P2401_ID_ARRENDADORA = ? AND P2426_TIPO_DOC in('R','X')
And P2408_FECHA_PAGADO >= ? AND P2408_FECHA_PAGADO <= ?";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.NVarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@FechaInicio", OdbcType.DateTime).Value = fechainicio.Date;
                comando.Parameters.Add("FechaCorte", OdbcType.DateTime).Value = FechaCorte.Date;
                comando.Parameters.Add("@IDInmobiliaria2", OdbcType.NVarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@FechaInicio2", OdbcType.DateTime).Value = fechainicio.Date;
                comando.Parameters.Add("FechaCorte2", OdbcType.DateTime).Value = FechaCorte.Date;
                conexion.Open();

                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReporteGlobalEntity recibo = new ReporteGlobalEntity();

                    recibo.idInmo = reader["P2401_ID_ARRENDADORA"].ToString();
                    recibo.IdCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    ClienteEntity cliente = getClienteByID(recibo.IdCliente);
                    recibo.NombreCliente = cliente == null ? "" : cliente.Nombre;
                    recibo.NombreComercial = cliente == null ? "" : cliente.NombreComercial;
                    recibo.RecibosPorContrato = SaariDB.getFacturasConceptolibre(idInmobiliaria, recibo.IdCliente, fechainicio.Date, FechaCorte.Date);// Aqui la lista corresponde a las facturas Libres por cliente.
                    listaRecibos.Add(recibo);
                }

                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getFacturasConceptolibre(string idInmobiliaria, string idCliente, DateTime fechainicio, DateTime FechaCorte)
        {
            List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT P2401_ID_ARRENDADORA,P2402_ID_ARRENDATARIO,P2426_TIPO_DOC,P2406_STATUS,P2444_ID_HIST_REC,P2405_IMPORTE AS IMPORTE ,P2416_IVA AS IVA,P2413_PAGO, P2419_TOTAL,
CAMPO_NUM1 AS RetencionISR, CAMPO_NUM2 AS RetencionIVA,P2427_CTD_PAG,P2410_MONEDA,P2420_MONEDA_PAGO,P2414_TIPO_CAMBIO,P2421_TC_PAGO,P2409_FECHA_EMISION,P2408_FECHA_PAGADO
FROM T24_HISTORIA_RECIBOS
WHERE P2418_ID_CONTRATO LIKE '%FACT%'   AND P2406_STATUS != '3' and P2401_ID_ARRENDADORA = ?  AND P2426_TIPO_DOC in('R','X')
AND P2402_ID_ARRENDATARIO = ?
And (P2409_FECHA_EMISION >= ? AND P2409_FECHA_EMISION <= ?)
UNION
SELECT P2401_ID_ARRENDADORA,P2402_ID_ARRENDATARIO,P2426_TIPO_DOC,P2406_STATUS,P2444_ID_HIST_REC,P2405_IMPORTE AS IMPORTE ,P2416_IVA AS IVA, P2413_PAGO, P2419_TOTAL,
CAMPO_NUM1 AS RetencionISR, CAMPO_NUM2 AS RetencionIVA,P2427_CTD_PAG,P2410_MONEDA,P2420_MONEDA_PAGO,P2414_TIPO_CAMBIO,P2421_TC_PAGO,P2409_FECHA_EMISION,P2408_FECHA_PAGADO
FROM T24_HISTORIA_RECIBOS
WHERE P2418_ID_CONTRATO LIKE '%FACT%'   AND P2406_STATUS != '3' and P2401_ID_ARRENDADORA = ? AND P2426_TIPO_DOC in('R','X')
AND P2402_ID_ARRENDATARIO = ?
And (P2408_FECHA_PAGADO >= ? AND P2408_FECHA_PAGADO <= ?)";
            //SELECT P2401_ID_ARRENDADORA,P2402_ID_ARRENDATARIO,P2426_TIPO_DOC,P2406_STATUS,P2444_ID_HIST_REC,P2405_IMPORTE AS IMPORTE ,P2416_IVA AS IVA ,
            //CAMPO_NUM1 AS RetencionISR, CAMPO_NUM2 AS RetencionIVA,P2427_CTD_PAG,P2410_MONEDA,P2420_MONEDA_PAGO,P2414_TIPO_CAMBIO,P2421_TC_PAGO,P2408_FECHA_PAGADO
            //FROM T24_HISTORIA_RECIBOS
            //WHERE P2418_ID_CONTRATO LIKE '%FACT%'   AND P2406_STATUS != 3 and P2401_ID_ARRENDADORA = ?  AND P2426_TIPO_DOC in('R','X')
            //AND P2402_ID_ARRENDATARIO = ?
            //And P2409_FECHA_EMISION >= ? AND P2409_FECHA_EMISION <= ?
            //";

            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.NVarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@IDCliente", OdbcType.NVarChar).Value = idCliente;
                comando.Parameters.Add("@FechaInicio", OdbcType.DateTime).Value = fechainicio.Date;
                comando.Parameters.Add("FechaCorte", OdbcType.DateTime).Value = FechaCorte.Date;
                comando.Parameters.Add("@IDInmobiliaria2", OdbcType.NVarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@IDCliente2", OdbcType.NVarChar).Value = idCliente;
                comando.Parameters.Add("@FechaInicio2", OdbcType.DateTime).Value = fechainicio.Date;
                comando.Parameters.Add("FechaCorte2", OdbcType.DateTime).Value = FechaCorte.Date;
                conexion.Open();

                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();

                    recibo.IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString();
                    recibo.NombreInmobiliaria = getInmobiliariaByID(recibo.IDInmobiliaria).NombreComercial;
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    ClienteEntity cliente = getClienteByID(recibo.IDCliente);
                    recibo.NombreCliente = cliente == null ? "" : cliente.Nombre;
                    recibo.nombreComercial = cliente == null ? "" : cliente.NombreComercial;
                    recibo.TipoDoc = reader["P2426_TIPO_DOC"].ToString();
                    recibo.Estatus = reader["P2406_STATUS"].ToString();
                    recibo.IDHistRec = (int)reader["P2444_ID_HIST_REC"];
                    recibo.Importe = reader["IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["IMPORTE"]);
                    recibo.IVA = reader["IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["IVA"]);
                    recibo.ISR = reader["RetencionISR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RetencionISR"]);
                    recibo.IVARetenido = reader["RetencionIVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RetencionIVA"]);
                    recibo.TotalxPagar = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2427_CTD_PAG"]);
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.MonedaPago = reader["P2420_MONEDA_PAGO"] == null ? "" : reader["P2420_MONEDA_PAGO"].ToString();
                    recibo.TipoCambio = (decimal)reader["P2414_TIPO_CAMBIO"];
                    recibo.TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2421_TC_PAGO"]);
                    recibo.FechaEmision = (DateTime)reader["P2409_FECHA_EMISION"];// == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.Abono = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]);


                    if (recibo.Moneda == "D" && recibo.Estatus == "1")
                    {
                        recibo.Importe = recibo.Importe * recibo.TipoCambio;
                        recibo.IVA = recibo.IVA * recibo.TipoCambio;
                        recibo.ISR = recibo.ISR * recibo.TipoCambio;
                        recibo.IVARetenido = recibo.IVARetenido * recibo.TipoCambio;
                        recibo.TotalxPagar = recibo.TotalxPagar * recibo.TipoCambio;
                    }
                    else if (recibo.MonedaPago == "D" && recibo.Estatus == "2")
                    {
                        recibo.Importe = recibo.Importe * recibo.TipoCambioPago;
                        recibo.IVA = recibo.IVA * recibo.TipoCambioPago;
                        recibo.ISR = recibo.ISR * recibo.TipoCambioPago;
                        recibo.IVARetenido = recibo.IVARetenido * recibo.TipoCambioPago;
                        recibo.TotalxPagar = recibo.TotalxPagar * recibo.TipoCambioPago;
                    }
                    DateTime maxPago = new DateTime(1900, 1, 1);
                    if (recibo.TipoDoc == "X" && recibo.Estatus == "1")
                    {
                        recibo.Abono = 0;
                        List<ReciboEntity> ListPagosParciales = getPagosParciales(recibo.IDHistRec, FechaCorte);
                        if (ListPagosParciales != null)
                        {
                            if (ListPagosParciales.Count > 0)
                            {
                                foreach (ReciboEntity PagoParcial in ListPagosParciales)
                                {
                                    if (PagoParcial.MonedaPago == "D")
                                    {
                                        PagoParcial.Total = PagoParcial.Total * PagoParcial.TipoCambioPago;
                                        PagoParcial.Pago = PagoParcial.Pago * PagoParcial.TipoCambioPago;
                                    }

                                    recibo.Abono += PagoParcial.Total;
                                }

                            }
                        }
                        ListPagosParciales.OrderBy(o => o.FechaPago);
                        maxPago = Convert.ToDateTime(ListPagosParciales.LastOrDefault().FechaPago);
                    }
                    if (recibo.FechaPago >= maxPago)
                    {
                        recibo.MaxFechaPago = Convert.ToDateTime(recibo.FechaPago);
                    }
                    else
                    {
                        recibo.MaxFechaPago = maxPago;
                    }

                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getCargosDeRecibos(string idInmobiliaria, string idContrato, string idCargo, string idComplemento, DateTime fechaInicio, DateTime fechaCorte, bool SinComplementos)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            string sql = @"SELECT P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P3401_ID_CARGO, P2426_TIPO_DOC,P2406_STATUS,P2409_FECHA_EMISION,P2408_FECHA_PAGADO,P3403_IMPORTE,P2413_PAGO,
P2415_IMP_DOLAR, P3404_IVA, P2417_IVA_DOLAR, P3405_TOTAL, P2424_DESCUENTO ,CAMPO_NUM1 AS ISR, CAMPO_NUM2 AS RETIVA,CAMPO4 AS SUBTIPO,P3403_IMPORTE AS TOTALCARGOS,
P3406_MONEDA,P2420_MONEDA_PAGO, P2444_ID_HIST_REC  
from T24_HISTORIA_RECIBOS
JOIN  T35_HISTORIAL_CARGOS ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC
WHERE P2401_ID_ARRENDADORA = ? AND T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO NOT LIKE '%FACT%'    AND P2406_STATUS != '3' AND
P3401_ID_CARGO= ? AND T35_HISTORIAL_CARGOS.CAMPO4 = ? ";
            if (SinComplementos)
            {
                sql += @"OR (P3401_ID_CARGO= ? AND T35_HISTORIAL_CARGOS.CAMPO4 = ?)";
            }

            sql += @" AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?";


            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idInmo2", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@Contrato", OdbcType.VarChar).Value = idContrato;
                comando.Parameters.Add("@Cargo", OdbcType.VarChar).Value = idCargo;
                if (SinComplementos)
                {
                    comando.Parameters.Add("@Contrato2", OdbcType.VarChar).Value = idContrato;
                    comando.Parameters.Add("@Cargo2", OdbcType.VarChar).Value = idComplemento;
                }
                comando.Parameters.Add("@fechaInicio", OdbcType.DateTime).Value = fechaInicio;
                comando.Parameters.Add("@FechaCorte", OdbcType.DateTime).Value = fechaCorte;

                List<ReciboEntity> ListRecibosRenta = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity CargoFac = new ReciboEntity()
                    {
                        IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                        IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString(),
                        IDContrato = reader["P3401_ID_CARGO"].ToString(),
                        TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                        Estatus = reader["P2406_STATUS"].ToString(),
                        FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                        FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                        Importe = reader["P3403_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3403_IMPORTE"]),
                        Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]),
                        ConDllsImporte = reader["P2415_IMP_DOLAR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2415_IMP_DOLAR"]),
                        IVA = reader["P3404_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3404_IVA"]),
                        ConDllsIVA = reader["P2417_IVA_DOLAR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2417_IVA_DOLAR"]),
                        Total = reader["P3405_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3405_TOTAL"]),
                        Descuento = reader["P2424_DESCUENTO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2424_DESCUENTO"]),
                        ISR = reader["ISR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ISR"]),
                        IVARetenido = reader["RETIVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RETIVA"]),
                        Campo20 = reader["SUBTIPO"].ToString(),
                        Cargo = reader["TOTALCARGOS"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTALCARGOS"]),
                        Moneda = reader["P3406_MONEDA"].ToString(),
                        MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                        IDHistRec = (int)reader["P2444_ID_HIST_REC"]

                    };

                    ListRecibosRenta.Add(CargoFac);
                }


                reader.Close();
                conexion.Close();
                return ListRecibosRenta;
            }
            catch
            {
                conexion.Close();
                return null;
            }

        }
        //1
        public static List<ReciboEntity> getRecibosdeRentaAndCargos(string idInmobiliaria, string idContrato, DateTime fechaInicio, DateTime fechaCorte, bool SinComplementos)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            //            string sql = @"SELECT P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2426_TIPO_DOC,P2406_STATUS,P2409_FECHA_EMISION,P2408_FECHA_PAGADO,P2405_IMPORTE,P2413_PAGO,
            //P2415_IMP_DOLAR, P2416_IVA, P2417_IVA_DOLAR, P2419_TOTAL, P2424_DESCUENTO ,CAMPO_NUM1 AS ISR, CAMPO_NUM2 AS RETIVA,CAMPO20 AS SUBTIPO,CAMPO_NUM6 AS TOTALCARGOS,
            //P2410_MONEDA,P2420_MONEDA_PAGO,P2414_TIPO_CAMBIO,P2421_TC_PAGO,P2444_ID_HIST_REC  
            //FROM T24_HISTORIA_RECIBOS
            //WHERE P2401_ID_ARRENDADORA = ? AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN('R','X') AND T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO NOT LIKE '%FACT%'
            //AND P2406_STATUS != 3 AND P2418_ID_CONTRATO = ?
            //AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?";

            string sql = @"SELECT P2401_ID_ARRENDADORA, P2402_ID_ARRENDATARIO, P2418_ID_CONTRATO, P2426_TIPO_DOC,P2406_STATUS,P2409_FECHA_EMISION,P2408_FECHA_PAGADO,P2405_IMPORTE,P2413_PAGO,
P2415_IMP_DOLAR, P2416_IVA, P2417_IVA_DOLAR, P2419_TOTAL, P2424_DESCUENTO ,CAMPO_NUM1 AS ISR, CAMPO_NUM2 AS RETIVA,CAMPO20 AS SUBTIPO,CAMPO_NUM6 AS TOTALCARGOS,
P2410_MONEDA,P2420_MONEDA_PAGO,P2414_TIPO_CAMBIO,P2421_TC_PAGO,P2444_ID_HIST_REC  
FROM T24_HISTORIA_RECIBOS
WHERE P2401_ID_ARRENDADORA = ? AND T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO NOT LIKE '%FACT%'
AND P2406_STATUS != '3' AND P2418_ID_CONTRATO = ? AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','X','W') 
AND  T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?";
            //AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ? ;

            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idInmo", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@Contrato", OdbcType.VarChar).Value = idContrato;
                comando.Parameters.Add("@fechaInicio", OdbcType.DateTime).Value = fechaInicio.Date;
                comando.Parameters.Add("@FechaCorte", OdbcType.DateTime).Value = fechaCorte.Date;
                //if(SinComplementos)
                //    comando.Parameters.Add("@Contrato2", OdbcType.VarChar).Value = idContrato;
                List<ReciboEntity> ListRecibosRenta = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity CargoFac = new ReciboEntity()
                    {
                        IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                        IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString(),
                        IDContrato = reader["P2418_ID_CONTRATO"].ToString(),
                        TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                        Estatus = reader["P2406_STATUS"].ToString(),
                        FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                        FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                        Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2405_IMPORTE"]),
                        Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]),
                        ConDllsImporte = reader["P2415_IMP_DOLAR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2415_IMP_DOLAR"]),
                        IVA = reader["P2416_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2416_IVA"]),
                        ConDllsIVA = reader["P2417_IVA_DOLAR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2417_IVA_DOLAR"]),
                        Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]),
                        Descuento = reader["P2424_DESCUENTO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2424_DESCUENTO"]),
                        ISR = reader["ISR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ISR"]),
                        IVARetenido = reader["RETIVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RETIVA"]),
                        Campo20 = reader["SUBTIPO"].ToString(),
                        Cargo = reader["TOTALCARGOS"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTALCARGOS"]),
                        Moneda = reader["P2410_MONEDA"].ToString(),
                        MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                        IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                        TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 1 : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"]),
                        TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 1 : Convert.ToDecimal(reader["P2421_TC_PAGO"]),

                    };
                    CargoFac.IDPago = SaariE.getIDPagoByHistRec(CargoFac.IDHistRec);
                    if (CargoFac.IDPago != 0)
                        CargoFac.saldoAFavor = SaariE.getSaldoAFavorbyIdPago(CargoFac.IDPago);
                    else
                        CargoFac.saldoAFavor = 0;
                    if (CargoFac.IDCliente == "JCS14")
                    { }

                    ListRecibosRenta.Add(CargoFac);
                }
                reader.Close();
                conexion.Close();
                return ListRecibosRenta;
            }
            catch
            {
                conexion.Close();
                return null;
            }


        }
        //1
        public static List<ReciboEntity> getInicioFacturacionContrato(ContratosEntity contrato, DateTime fechaCorte)
        {
            try
            {
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    string sql = @"SELECT T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,
                                    T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2419_TOTAL, 
                                    T24_HISTORIA_RECIBOS.P2413_PAGO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.P2427_CTD_PAG, T24_HISTORIA_RECIBOS.P2421_TC_PAGO, T24_HISTORIA_RECIBOS.P2404_PERIODO,
                                    T24_HISTORIA_RECIBOS.P2420_MONEDA_PAGO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.CAMPO9, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA,
                                    T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO 
                                    FROM T24_HISTORIA_RECIBOS
                                    LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC
                                    WHERE P2426_TIPO_DOC IN ('R','X') AND P2409_FECHA_EMISION <= ? AND P2418_ID_CONTRATO = ? 
                                    ORDER BY P2409_FECHA_EMISION, P4006_SERIE, P4007_FOLIO";
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@FechaCorte", OdbcType.DateTime).Value = fechaCorte.Date;
                        comando.Parameters.Add("@IDCte", OdbcType.VarChar).Value = contrato.IDContrato;
                        List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReciboEntity recibo = new ReciboEntity()
                                {
                                    IDHistRec = (int)reader["P2444_ID_HIST_REC"],
                                    Numero = (int)reader["P2403_NUM_RECIBO"],
                                    FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                                    FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),
                                    TipoDoc = reader["P2426_TIPO_DOC"].ToString(),
                                    Moneda = reader["P2410_MONEDA"].ToString(),
                                    Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]),
                                    Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]),
                                    Estatus = reader["P2406_STATUS"].ToString(),
                                    PagoParcial = reader["P2427_CTD_PAG"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2427_CTD_PAG"]),
                                    TipoCambioPago = reader["P2421_TC_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2421_TC_PAGO"]),
                                    Periodo = reader["P2404_PERIODO"].ToString(),
                                    Serie = reader["P4006_SERIE"].ToString(),
                                    Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["P4007_FOLIO"]),
                                    MonedaPago = reader["P2420_MONEDA_PAGO"].ToString(),
                                    Concepto = reader["P2412_CONCEPTO"].ToString(),
                                    Inmueble = reader["CAMPO9"].ToString(),
                                    IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString(),
                                    TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"])
                                };
                                if (recibo.Estatus != "3" || (recibo.Estatus == "3" && recibo.FechaPago > fechaCorte.Date))
                                {
                                    listaRecibos.Add(recibo);
                                }
                                //Aqui estaba listaRecibos
                                //}
                            }
                        }
                        return listaRecibos;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Reporte de facturación
        public static List<RegistroFacturacionEntity> getRegistrosFacturacion(DateTime fechaInicio, DateTime fechaFin, List<string> listaIdsInmobiliarias)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                //                string sql = @"SELECT T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T04_CONTRATO.P0404_ID_EDIFICIO, T04_CONTRATO.P0401_ID_CONTRATO, T07_EDIFICIO.P0710_ID_CENTRO, 
                //SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN (T24_HISTORIA_RECIBOS.P2405_IMPORTE * P2414_TIPO_CAMBIO) ELSE T24_HISTORIA_RECIBOS.P2405_IMPORTE END) AS TOTAL
                //FROM T24_HISTORIA_RECIBOS
                //JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                //LEFT JOIN T04_CONTRATO ON T04_CONTRATO.P0401_ID_CONTRATO = T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO
                //LEFT JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T04_CONTRATO.P0404_ID_EDIFICIO  
                //WHERE T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','X','W') 
                //AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?
                //AND (T24_HISTORIA_RECIBOS.P2406_STATUS IN (1,2) OR (T24_HISTORIA_RECIBOS.P2406_STATUS = '3' AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO > ?)) 
                //AND P2401_ID_ARRENDADORA IN (";
                //                foreach (string idInmo in listaIdsInmobiliarias)
                //                    sql += "?,";
                //                sql = sql.Substring(0, sql.Length - 1);
                //                sql += ") GROUP BY T04_CONTRATO.P0401_ID_CONTRATO, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T07_EDIFICIO.P0710_ID_CENTRO, T04_CONTRATO.P0404_ID_EDIFICIO, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA";

                //                sql += @" UNION ALL
                //SELECT T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T04_CONTRATO.P0404_ID_EDIFICIO, T04_CONTRATO.P0401_ID_CONTRATO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO AS P0710_ID_CENTRO, 
                //SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN (T24_HISTORIA_RECIBOS.P2405_IMPORTE * P2414_TIPO_CAMBIO) ELSE T24_HISTORIA_RECIBOS.P2405_IMPORTE END) AS TOTAL
                //FROM T24_HISTORIA_RECIBOS
                //JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                //JOIN T04_CONTRATO ON T04_CONTRATO.P0401_ID_CONTRATO = T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO
                //JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T04_CONTRATO.P0404_ID_EDIFICIO  
                //WHERE T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','X','W') 
                //AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?
                //AND (T24_HISTORIA_RECIBOS.P2406_STATUS IN (1,2) OR (T24_HISTORIA_RECIBOS.P2406_STATUS = '3' AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO > ?)) 
                //AND P2401_ID_ARRENDADORA IN (";
                //                foreach (string idInmo in listaIdsInmobiliarias)
                //                    sql += "?,";
                //                sql = sql.Substring(0, sql.Length - 1);
                //                sql += ") GROUP BY T04_CONTRATO.P0401_ID_CONTRATO, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO, T04_CONTRATO.P0404_ID_EDIFICIO, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA";
                string sql = @"SELECT T24_HISTORIA_RECIBOS.CAMPO_NUM1,T24_HISTORIA_RECIBOS.CAMPO_NUM2, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T04_CONTRATO.P0404_ID_EDIFICIO, T04_CONTRATO.P0401_ID_CONTRATO, T07_EDIFICIO.P0710_ID_CENTRO, 
SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN (T24_HISTORIA_RECIBOS.P2419_TOTAL * P2414_TIPO_CAMBIO) ELSE T24_HISTORIA_RECIBOS.P2419_TOTAL END) AS TOTAL,
SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN (T24_HISTORIA_RECIBOS.P2416_IVA * P2414_TIPO_CAMBIO) ELSE T24_HISTORIA_RECIBOS.P2416_IVA END) AS IVA 
FROM T24_HISTORIA_RECIBOS 
JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC 
LEFT JOIN T04_CONTRATO ON T04_CONTRATO.P0401_ID_CONTRATO = T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO 
LEFT JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T04_CONTRATO.P0404_ID_EDIFICIO   
WHERE T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','X','W')  
AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ? 
AND (T24_HISTORIA_RECIBOS.P2406_STATUS IN ('1','2') OR (T24_HISTORIA_RECIBOS.P2406_STATUS = '3' AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO > ?))  
AND P2401_ID_ARRENDADORA IN (";
                foreach (string idInmo in listaIdsInmobiliarias)
                    sql += "?,";
                sql = sql.Substring(0, sql.Length - 1);
                sql += ") GROUP BY T04_CONTRATO.P0401_ID_CONTRATO, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T07_EDIFICIO.P0710_ID_CENTRO, T04_CONTRATO.P0404_ID_EDIFICIO, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, T24_HISTORIA_RECIBOS.CAMPO_NUM1,T24_HISTORIA_RECIBOS.CAMPO_NUM2";

                sql += @" UNION ALL 
SELECT T24_HISTORIA_RECIBOS.CAMPO_NUM1,T24_HISTORIA_RECIBOS.CAMPO_NUM2, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T04_CONTRATO.P0404_ID_EDIFICIO, T04_CONTRATO.P0401_ID_CONTRATO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO AS P0710_ID_CENTRO, 
SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN (T24_HISTORIA_RECIBOS.P2419_TOTAL * P2414_TIPO_CAMBIO) ELSE T24_HISTORIA_RECIBOS.P2419_TOTAL END) AS TOTAL,
SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN (T24_HISTORIA_RECIBOS.P2416_IVA * P2414_TIPO_CAMBIO) ELSE T24_HISTORIA_RECIBOS.P2416_IVA END) AS IVA 
FROM T24_HISTORIA_RECIBOS 
JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC 
JOIN T04_CONTRATO ON T04_CONTRATO.P0401_ID_CONTRATO = T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO 
JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T04_CONTRATO.P0404_ID_EDIFICIO   
WHERE T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','X','W')  
AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ? 
AND (T24_HISTORIA_RECIBOS.P2406_STATUS IN ('1','2') OR (T24_HISTORIA_RECIBOS.P2406_STATUS = '3' AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO > ?))  
AND P2401_ID_ARRENDADORA IN (";
                foreach (string idInmo in listaIdsInmobiliarias)
                    sql += "?,";
                sql = sql.Substring(0, sql.Length - 1);
                sql += ") GROUP BY T04_CONTRATO.P0401_ID_CONTRATO, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO, T04_CONTRATO.P0404_ID_EDIFICIO, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA,  T24_HISTORIA_RECIBOS.CAMPO_NUM1,T24_HISTORIA_RECIBOS.CAMPO_NUM2";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@FechaIni", OdbcType.Date).Value = fechaInicio.Date;
                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaFin.Date;
                comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaFin.Date;
                foreach (string idInmo in listaIdsInmobiliarias)
                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idInmo.Trim();
                comando.Parameters.Add("@FechaIni2", OdbcType.Date).Value = fechaInicio.Date;
                comando.Parameters.Add("@FechaFin3", OdbcType.Date).Value = fechaFin.Date;
                comando.Parameters.Add("@FechaFin4", OdbcType.Date).Value = fechaFin.Date;
                foreach (string idInmo in listaIdsInmobiliarias)
                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idInmo.Trim();
                List<RegistroFacturacionEntity> listaRegistros = new List<RegistroFacturacionEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    RegistroFacturacionEntity registro = new RegistroFacturacionEntity();
                    registro.IDArrendadora = reader["P2401_ID_ARRENDADORA"].ToString();
                    registro.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    registro.IDInmueble = reader["P0404_ID_EDIFICIO"].ToString();
                    registro.IDContrato = reader["P0401_ID_CONTRATO"].ToString();
                    registro.IDConjunto = reader["P0710_ID_CENTRO"].ToString();
                    registro.TotalReal = reader["TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL"]);
                    registro.IVA = reader["IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["IVA"]);
                    registro.ISR = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                    registro.IVARetenido = reader["CAMPO_NUM2"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM2"]);
                    registro.Total = registro.TotalReal - registro.IVA + (registro.ISR + registro.IVARetenido);
                    //registro.TotalReal = registro.Total - registro.IVA + (registro.ISR + registro.IVARetenido);
                    /*registro.Total = registro.Total - registro.IVA;*/
                    
                    InmobiliariaEntity inmo = getInmobiliariaByID(registro.IDArrendadora);
                    registro.RazonInmobiliaria = inmo == null ? string.Empty : inmo.RazonSocial;
                    registro.NComercialInmobiliaria = inmo == null ? string.Empty : inmo.NombreComercial;
                    ClienteEntity clien = getClienteByID(registro.IDCliente);
                    registro.RazonCliente = clien == null ? string.Empty : clien.Nombre;
                    registro.NComercialCliente = clien == null ? string.Empty : clien.NombreComercial;
                    EdificioEntity inmueble = getInmuebleByID(registro.IDInmueble);
                    if (inmueble != null)
                    {
                        registro.NombreInmueble = inmueble.Nombre;
                        registro.IdentificadorInmueble = inmueble.Identificador;
                    }
                    else
                    {
                        inmueble = getSubconjuntoByID(registro.IDInmueble);
                        if (inmueble != null)
                        {
                            registro.NombreInmueble = inmueble.Nombre;
                            registro.IdentificadorInmueble = inmueble.Identificador;
                        }
                    }
                    ConjuntoEntity conjunto = getConjuntoByID(registro.IDConjunto);
                    registro.NombreConjunto = conjunto == null ? string.Empty : conjunto.Nombre;
                    listaRegistros.Add(registro);
                }
                reader.Close();
                conexion.Close();
                return listaRegistros;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }

        public static List<RegistroFacturacionEntity> getNCFacturacion(DateTime fechaInicio, DateTime fechaFin, List<string> listaIdsInmobiliarias)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T04_CONTRATO.P0404_ID_EDIFICIO, T04_CONTRATO.P0401_ID_CONTRATO, T07_EDIFICIO.P0710_ID_CENTRO, 
SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN (T24_HISTORIA_RECIBOS.P2405_IMPORTE * P2414_TIPO_CAMBIO) ELSE T24_HISTORIA_RECIBOS.P2405_IMPORTE END) AS TOTAL, 
SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN (T24_HISTORIA_RECIBOS.P2416_IVA * P2414_TIPO_CAMBIO) ELSE T24_HISTORIA_RECIBOS.P2416_IVA END) AS IVA 
FROM T24_HISTORIA_RECIBOS
JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
LEFT JOIN T04_CONTRATO ON T04_CONTRATO.P0401_ID_CONTRATO = T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO
LEFT JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T04_CONTRATO.P0404_ID_EDIFICIO  
WHERE T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('B') 
AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?
AND (T24_HISTORIA_RECIBOS.P2406_STATUS IN ('1','2') OR (T24_HISTORIA_RECIBOS.P2406_STATUS = '3' AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO > ?)) 
AND P2401_ID_ARRENDADORA IN (";
                foreach (string idInmo in listaIdsInmobiliarias)
                    sql += "?,";
                sql = sql.Substring(0, sql.Length - 1);
                sql += ") GROUP BY T04_CONTRATO.P0401_ID_CONTRATO, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T07_EDIFICIO.P0710_ID_CENTRO, T04_CONTRATO.P0404_ID_EDIFICIO, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA";

                sql += @" UNION ALL
SELECT T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T04_CONTRATO.P0404_ID_EDIFICIO, T04_CONTRATO.P0401_ID_CONTRATO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO AS P0710_ID_CENTRO, 
SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN (T24_HISTORIA_RECIBOS.P2405_IMPORTE * P2414_TIPO_CAMBIO) ELSE T24_HISTORIA_RECIBOS.P2405_IMPORTE END) AS TOTAL, 
SUM(CASE WHEN T24_HISTORIA_RECIBOS.P2410_MONEDA = 'D' THEN (T24_HISTORIA_RECIBOS.P2416_IVA * P2414_TIPO_CAMBIO) ELSE T24_HISTORIA_RECIBOS.P2416_IVA END) AS IVA 
FROM T24_HISTORIA_RECIBOS
JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
JOIN T04_CONTRATO ON T04_CONTRATO.P0401_ID_CONTRATO = T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO
JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T04_CONTRATO.P0404_ID_EDIFICIO  
WHERE T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('B') 
AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?
AND (T24_HISTORIA_RECIBOS.P2406_STATUS IN ('1','2') OR (T24_HISTORIA_RECIBOS.P2406_STATUS = '3' AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO > ?)) 
AND P2401_ID_ARRENDADORA IN (";
                foreach (string idInmo in listaIdsInmobiliarias)
                    sql += "?,";
                sql = sql.Substring(0, sql.Length - 1);
                sql += ") GROUP BY T04_CONTRATO.P0401_ID_CONTRATO, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO, T04_CONTRATO.P0404_ID_EDIFICIO, T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@FechaIni", OdbcType.Date).Value = fechaInicio.Date;
                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaFin.Date;
                comando.Parameters.Add("@FechaFin2", OdbcType.Date).Value = fechaFin.Date;
                foreach (string idInmo in listaIdsInmobiliarias)
                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idInmo.Trim();
                comando.Parameters.Add("@FechaIni2", OdbcType.Date).Value = fechaInicio.Date;
                comando.Parameters.Add("@FechaFin3", OdbcType.Date).Value = fechaFin.Date;
                comando.Parameters.Add("@FechaFin4", OdbcType.Date).Value = fechaFin.Date;
                foreach (string idInmo in listaIdsInmobiliarias)
                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idInmo.Trim();
                List<RegistroFacturacionEntity> listaRegistros = new List<RegistroFacturacionEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())

                {
                    RegistroFacturacionEntity registro = new RegistroFacturacionEntity();
                    registro.IDArrendadora = reader["P2401_ID_ARRENDADORA"].ToString();
                    registro.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    registro.IDInmueble = reader["P0404_ID_EDIFICIO"].ToString();
                    registro.IDContrato = reader["P0401_ID_CONTRATO"].ToString();
                    registro.IDConjunto = reader["P0710_ID_CENTRO"].ToString();
                    registro.IVA = reader["IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["IVA"]);
                    registro.Total = reader["TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL"]);
                    registro.TotalReal = registro.Total + registro.IVA;
                    InmobiliariaEntity inmo = getInmobiliariaByID(registro.IDArrendadora);
                    registro.RazonInmobiliaria = inmo == null ? string.Empty : inmo.RazonSocial;
                    registro.NComercialInmobiliaria = inmo == null ? string.Empty : inmo.NombreComercial;
                    ClienteEntity clien = getClienteByID(registro.IDCliente);
                    registro.RazonCliente = clien == null ? string.Empty : clien.Nombre;
                    registro.NComercialCliente = clien == null ? string.Empty : clien.NombreComercial;
                    EdificioEntity inmueble = getInmuebleByID(registro.IDInmueble);
                    if (inmueble != null)
                    {
                        registro.NombreInmueble = inmueble.Nombre;
                        registro.IdentificadorInmueble = inmueble.Identificador;
                    }
                    else
                    {
                        inmueble = getSubconjuntoByID(registro.IDInmueble);
                        if (inmueble != null)
                        {
                            registro.NombreInmueble = inmueble.Nombre;
                            registro.IdentificadorInmueble = inmueble.Identificador;
                        }
                    }
                    ConjuntoEntity conjunto = getConjuntoByID(registro.IDConjunto);
                    registro.NombreConjunto = conjunto == null ? string.Empty : conjunto.Nombre;
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

        #endregion

        #region Reporte de CFDi's consecutivos
        public static List<SubtipoEntity> getSubTiposOI()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = "SELECT DESCR_CAT, CAMPO1 FROM CATEGORIA WHERE ID_COT = 'SUBTPCB' AND TIPO = 'IO' AND CAMPO1 IS NOT NULL ORDER BY CAMPO_NUM2, DESCR_CAT"; //AND CAMPO_NUM2 > 3 AND CAMPO_NUM2 <> 5;
                List<SubtipoEntity> subtipos = new List<SubtipoEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                int orden = 1;
                while (reader.Read())
                {
                    SubtipoEntity sub = new SubtipoEntity()
                    {
                        Identificador = reader["CAMPO1"].ToString().Trim(),
                        Nombre = reader["DESCR_CAT"].ToString().Trim(),
                        Orden = orden
                    };
                    orden++;
                    subtipos.Add(sub);
                }
                /*SubtipoEntity s = new SubtipoEntity()
                {
                    Nombre = "Otros",
                    Orden = orden
                };
                subtipos.Add(s);*/
                reader.Close();
                conexion.Close();
                return subtipos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<SubtipoEntity> getSubTiposIA()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = "SELECT DESCR_CAT, CAMPO1 FROM CATEGORIA WHERE ID_COT = 'SUBTPCB' AND TIPO = 'IA' AND CAMPO1 IS NOT NULL ORDER BY CAMPO_NUM2, DESCR_CAT"; //AND CAMPO_NUM2 > 3 AND CAMPO_NUM2 <> 5;
                List<SubtipoEntity> subtipos = new List<SubtipoEntity>();
                SubtipoEntity subtipo = new SubtipoEntity()
                {
                    Identificador = "",
                    Nombre = "Ingresos por Renta",
                };
                subtipos.Add(subtipo);

                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                int orden = 1;
                while (reader.Read())
                {
                    SubtipoEntity sub = new SubtipoEntity()
                    {
                        Identificador = reader["CAMPO1"].ToString().Trim(),
                        Nombre = reader["DESCR_CAT"].ToString().Trim(),
                        Orden = orden
                    };
                    orden++;
                    subtipos.Add(sub);
                }
                /*SubtipoEntity s = new SubtipoEntity()
                {
                    Nombre = "Otros",
                    Orden = orden
                };
                subtipos.Add(s);*/
                reader.Close();
                conexion.Close();
                return subtipos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<CFDIConsecutivoEntity> getRegistrosCFDISConsec(string idArrendadora, string idConjunto, DateTime fechaInicio, DateTime fechaFin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            //string idSubconjunto = string.Empty;
            //string idCentro = string.Empty;
            int x = 0;
            try
            {/*
                string sql = @"SELECT T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.CAMPO20,
 T02_ARRENDATARIO.P0203_NOMBRE, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL, T07_EDIFICIO.P0703_NOMBRE, T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.CAMPO_NUM13,   
 T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2416_IVA, T24_HISTORIA_RECIBOS.P2419_TOTAL, T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO,
  T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO, T24_HISTORIA_RECIBOS.CAMPO_NUM12, T24_HISTORIA_RECIBOS.CAMPO_NUM6, T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC
FROM T24_HISTORIA_RECIBOS 
JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO
JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T24_HISTORIA_RECIBOS.CAMPO3
WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ? AND T24_HISTORIA_RECIBOS.CAMPO4 = ? 
AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?
AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','X','W')
ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO";*/
             //AND (T24_HISTORIA_RECIBOS.P2406_STATUS <> 3 OR (T24_HISTORIA_RECIBOS.P2406_STATUS = 3 AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO > ?))
             //Se agregaron los campos de Retenciones IVA e ISR.
                string sql = string.Empty;
                if (idConjunto != "Todos")
                {
                    sql = @"SELECT T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.CAMPO20,
 T02_ARRENDATARIO.P0203_NOMBRE, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL,T24_HISTORIA_RECIBOS.CAMPO4,T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO,T18_SUBCONJUNTOS.P1803_NOMBRE,T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO ,T07_EDIFICIO.P0703_NOMBRE, T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.CAMPO_NUM13,   
 T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2416_IVA, T24_HISTORIA_RECIBOS.P2419_TOTAL,T24_HISTORIA_RECIBOS.CAMPO_NUM1,T24_HISTORIA_RECIBOS.CAMPO_NUM2, T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO,
  T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO, T24_HISTORIA_RECIBOS.CAMPO_NUM12, T24_HISTORIA_RECIBOS.CAMPO_NUM6, T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC,
 T40_CFD.P4097_CLAVE_METODO_PAGO_CFDI, T40_CFD.P4089_METODO_PAGO
FROM T24_HISTORIA_RECIBOS 
JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO
LEFT JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T24_HISTORIA_RECIBOS.CAMPO3
LEFT JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T07_EDIFICIO.P0730_SUBCONJUNTO
WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ? AND T24_HISTORIA_RECIBOS.CAMPO4 = ?
AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?
AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','X','W')
ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO";
                }
                else
                {
                    //                    sql = @"SELECT P2418_ID_CONTRATO,T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.CAMPO20,
                    // T02_ARRENDATARIO.P0203_NOMBRE, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL,T24_HISTORIA_RECIBOS.CAMPO3,T24_HISTORIA_RECIBOS.CAMPO4,T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.CAMPO_NUM13,   
                    // T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2416_IVA, T24_HISTORIA_RECIBOS.P2419_TOTAL,T24_HISTORIA_RECIBOS.CAMPO_NUM1,T24_HISTORIA_RECIBOS.CAMPO_NUM2, T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO,
                    //  T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO, T24_HISTORIA_RECIBOS.CAMPO_NUM12, T24_HISTORIA_RECIBOS.CAMPO_NUM6, T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC,
                    // T40_CFD.P4097_CLAVE_METODO_PAGO_CFDI, T40_CFD.P4089_METODO_PAGO
                    //FROM T24_HISTORIA_RECIBOS 
                    //JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                    //JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO
                    //WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ?
                    //AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?
                    //AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','X','W')
                    //ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO";
                    sql = @"SELECT P2418_ID_CONTRATO,T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.CAMPO20,
 T02_ARRENDATARIO.P0203_NOMBRE, T02_ARRENDATARIO.P0253_NOMBRE_COMERCIAL,T24_HISTORIA_RECIBOS.CAMPO3,T24_HISTORIA_RECIBOS.CAMPO4,T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO,T18_SUBCONJUNTOS.P1803_NOMBRE,T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO ,T24_HISTORIA_RECIBOS.P2404_PERIODO, T24_HISTORIA_RECIBOS.P2406_STATUS, T24_HISTORIA_RECIBOS.CAMPO_NUM13,   
 T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2416_IVA, T24_HISTORIA_RECIBOS.P2419_TOTAL,T24_HISTORIA_RECIBOS.CAMPO_NUM1,T24_HISTORIA_RECIBOS.CAMPO_NUM2, T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO,
  T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO, T24_HISTORIA_RECIBOS.CAMPO_NUM12, T24_HISTORIA_RECIBOS.CAMPO_NUM6, T24_HISTORIA_RECIBOS.P2424_DESCUENTO, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC,
 T40_CFD.P4097_CLAVE_METODO_PAGO_CFDI, T40_CFD.P4089_METODO_PAGO
FROM T24_HISTORIA_RECIBOS 
JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO
LEFT JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T24_HISTORIA_RECIBOS.CAMPO3
LEFT JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T07_EDIFICIO.P0730_SUBCONJUNTO
WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA = ?  
AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ?  AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ? 
AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','X','W')
ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO";

                }
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArrendadora;
                if (idConjunto != "Todos")
                    comando.Parameters.Add("@IDConj", OdbcType.VarChar).Value = idConjunto;
                comando.Parameters.Add("@FechaIni", OdbcType.DateTime).Value = fechaInicio.Date;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin.Date;
                List<CFDIConsecutivoEntity> listaRegistros = new List<CFDIConsecutivoEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                
                while (reader.Read())
                {
                    CFDIConsecutivoEntity c = new CFDIConsecutivoEntity();
                    //if(x == 831)
                    //{
                    //    System.Diagnostics.Debugger.Break();
                    //}
                    c.IDHistRec = (int)reader["P2444_ID_HIST_REC"];

                    c.FechaEmision = (DateTime)reader["P2409_FECHA_EMISION"];
                    c.Serie = reader["P4006_SERIE"].ToString().Trim();
                    c.Folio = (int)reader["P4007_FOLIO"];
                    c.NombreCliente = reader["P0203_NOMBRE"].ToString().Trim();
                    c.NombreComercialCliente = reader["P0253_NOMBRE_COMERCIAL"].ToString().Trim();
                    c.NombreSubConj = reader["P1803_NOMBRE"].ToString().Trim();
                    c.IdContrato  = reader["P2418_ID_CONTRATO"].ToString().Trim(); 
                    c.IdSubConjunto = reader["P1801_ID_SUBCONJUNTO"].ToString().Trim();
                    if (idConjunto != "Todos")
                        c.NombreInmueble = reader["P0703_NOMBRE"].ToString().Trim();
                    else
                    {
                        c.IdInmueble = reader["CAMPO3"].ToString().Trim();

                        if(!string.IsNullOrEmpty(c.IdInmueble) && c.IdInmueble != "N/A")
                        {
                            c.NombreInmueble = getNombreInmuebleByID(c.IdInmueble);
                        }
                        
                        #region SubConjunto
                        if (string.IsNullOrEmpty(c.NombreInmueble))
                        {
                            //consulta por el contrato 
                            if (!c.IdContrato.Contains("FACT"))
                            {
                                c.IdSubConjunto = getIdEdificio(c.IdContrato);
                            }
                            

                            //System.Diagnostics.Debugger.Break();

                            if (c.IdSubConjunto.Contains("SCNJ"))
                            {
                                //System.Diagnostics.Debugger.Break();
                                c.NombreInmueble = getNombreSubConjunto(c.IdSubConjunto);
                                //c.NombreInmueble = c.NombreSubConj;
                            }
                        }
                        #endregion
                    }
                    if (c.Folio == 1445 || c.Folio == 1451 || c.Folio == 1456)
                    { }

                    string ConjuntoID = reader["CAMPO4"].ToString().Trim();
                    try
                    {
                        c.NombreConjunto = getConjuntoByID(ConjuntoID).Nombre;
                    }
                    catch { c.NombreConjunto = string.Empty; }

                    if (string.IsNullOrEmpty(c.NombreConjunto))
                    {
                        #region SubConjunto
                        if (c.IdSubConjunto.Contains("SCNJ"))
                        {
                            ConjuntoID = getIdCentro(c.IdSubConjunto);
                            
                            c.NombreConjunto = getNombreConjunto(ConjuntoID);
                        }
                        #endregion
                    }
                    c.Periodo = reader["P2404_PERIODO"].ToString().Trim();
                    if (reader["P2406_STATUS"].ToString() == "3" && DBNull.Value != reader["P2408_FECHA_PAGADO"] && Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]) <= fechaFin.Date)
                        c.EsCancelada = true;
                    if (!c.EsCancelada)
                    {
                        c.ImporteFac = (decimal)reader["P2405_IMPORTE"];
                        c.IVAFac = (decimal)reader["P2416_IVA"];
                        c.TipoDeCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 1 : (decimal)reader["P2414_TIPO_CAMBIO"];
                        c.TotalCargos = reader["CAMPO_NUM6"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM6"]);
                        c.DescuentoRenta = reader["P2424_DESCUENTO"] == DBNull.Value ? 0 : 0 - (decimal)reader["P2424_DESCUENTO"];
                        c.IDCargo = reader["CAMPO_NUM12"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CAMPO_NUM12"]);
                        c.RetIVA = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                        c.ISR = reader["CAMPO_NUM2"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM2"]);
                    }
                    c.Moneda = reader["P2410_MONEDA"].ToString().Trim();
                    c.TipoDocumento = reader["P2426_TIPO_DOC"].ToString().Trim();
                    c.IDSubtipo = reader["CAMPO20"].ToString().Trim();
                    c.EsRentaAnticipada = string.IsNullOrEmpty(reader["CAMPO_NUM13"].ToString()) ? false : Convert.ToInt32(reader["CAMPO_NUM13"]) == 1;
                    c.FormaPago = reader["P4097_CLAVE_METODO_PAGO_CFDI"].ToString().Trim();
                    c.MetodoPago = reader["P4089_METODO_PAGO"].ToString().Trim();
                    if (c.FormaPago == "PUE" && c.MetodoPago == "01")
                    {
                        c.MetodoPago = "EFECTIVO";
                    }
                    else if (c.FormaPago == "PUE" && c.MetodoPago == "03")
                    {
                        c.MetodoPago = "TRANSFERENCIA ELECTRONICA DE PAGO";
                    }
                    else if (c.FormaPago == "PUE" && c.MetodoPago == "02")
                    {
                        c.FormaPago = "PUE"; c.MetodoPago = "CHEQUE NOMINATIVO";
                    }
                    else if (c.FormaPago == "" && c.MetodoPago == "99")
                    {
                        c.FormaPago = "PPD";
                    }
                    
                    listaRegistros.Add(c);
                    x++;

                }
               
                reader.Close();
                conexion.Close();
                return listaRegistros;
            }
            catch (Exception e)
            {
                conexion.Close();
                return null;
            }
        }

       

        public static decimal getImporteDescuentoNegativo(int idHistRec)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT SUM(P3403_IMPORTE) FROM T35_HISTORIAL_CARGOS WHERE P3409_ID_HIST_REC = ? AND P3403_IMPORTE < 0 AND CAMPO_NUM3 = 5";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDhist", OdbcType.Int).Value = idHistRec;
                conexion.Open();
                decimal descuento = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return descuento;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        public static decimal getImporteCargo(int idHistRec/*, int idTipo*/)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT SUM(P3403_IMPORTE) FROM T35_HISTORIAL_CARGOS WHERE P3409_ID_HIST_REC = ?"; //AND CAMPO_NUM3 = ? AND P3403_IMPORTE > 0;
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDhist", OdbcType.Int).Value = idHistRec;
                //comando.Parameters.Add("@IDTipo", OdbcType.Decimal).Value = idTipo;
                conexion.Open();
                decimal importe = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return importe;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }
        public static List<ReciboEntity> getImporteCargos(int idHistRec)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);

            string sql = @"Select T35_HISTORIAL_CARGOS.P3401_ID_CARGO, T35_HISTORIAL_CARGOS.P3402_CONCEPTO, T35_HISTORIAL_CARGOS.P3403_IMPORTE, T35_HISTORIAL_CARGOS.P3404_IVA,
 T35_HISTORIAL_CARGOS.P3405_TOTAL, T35_HISTORIAL_CARGOS.CAMPO4, T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC
from T35_HISTORIAL_CARGOS
WHERE T35_HISTORIAL_CARGOS.P3409_ID_HIST_REC = ? "; //AND T35_HISTORIAL_CARGOS.CAMPO4 = ?
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idHistRec", OdbcType.Int).Value = idHistRec;
                //comando.Parameters.Add("@Cargo", OdbcType.VarChar).Value = cargo;

                List<ReciboEntity> ListaCargosPer = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity CargoFac = new ReciboEntity()
                    {
                        IDContrato = reader["P3401_ID_CARGO"].ToString(),
                        //IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString(),
                        Campo20 = reader["CAMPO4"].ToString(),
                        Importe = reader["P3403_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3403_IMPORTE"]),
                        IVA = reader["P3404_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3404_IVA"]),
                        TotalIVA = reader["P3405_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3405_TOTAL"])
                        //FechaEmision = reader["CAMPO20"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"])
                        //Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"],

                    };

                    ListaCargosPer.Add(CargoFac);
                }


                reader.Close();
                conexion.Close();
                return ListaCargosPer;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static decimal getImporteCargo(int idHistRec, /*int idTipo,*/ string idSubtipo)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = string.Empty;
                /*if(idTipo != 4)
                    sql = "SELECT SUM(P3403_IMPORTE) FROM T35_HISTORIAL_CARGOS WHERE P3409_ID_HIST_REC = ? AND CAMPO_NUM3 = ? AND CAMPO4 = ? AND P3403_IMPORTE > 0";
                else
                    sql = "SELECT SUM(P3403_IMPORTE) FROM T35_HISTORIAL_CARGOS WHERE P3409_ID_HIST_REC = ? AND (CAMPO_NUM3 = ? OR CAMPO_NUM3 = 0 OR CAMPO_NUM3 IS NULL) AND CAMPO4 = ? ";//AND P3403_IMPORTE > 0
                 */
                sql = "SELECT SUM(P3403_IMPORTE) FROM T35_HISTORIAL_CARGOS WHERE P3409_ID_HIST_REC = ? AND CAMPO4 = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDhist", OdbcType.Int).Value = idHistRec;
                comando.Parameters.Add("@IDSub", OdbcType.VarChar).Value = idSubtipo;
                conexion.Open();
                decimal importe = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return importe;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }
        #endregion

        #region Reporte de auditoria de contratos
        public delegate void ObjetoAgregadoHandler(int contador);
        public static event ObjetoAgregadoHandler ObjetoAgregado;

        public static void OnObjetoAgregado(int contador)
        {
            var handler = ObjetoAgregado;
            if (handler != null)
                ObjetoAgregado(contador);
        }

        public static bool CancelacionPendiente { get; set; }

        public static int getAuditoriaContratosCount(string idArrendadora, DateTime fechaPago, bool esPreventa)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT COUNT(T04_CONTRATO.P0401_ID_CONTRATO) FROM T04_CONTRATO
LEFT JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T04_CONTRATO.P0404_ID_EDIFICIO
LEFT JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T07_EDIFICIO.P0730_SUBCONJUNTO
INNER JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T07_EDIFICIO.P0710_ID_CENTRO
INNER JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T04_CONTRATO.P0402_ID_ARRENDAT
WHERE T04_CONTRATO.P0403_ID_ARRENDADORA = ? ";
                if (!esPreventa)
                {
                    sql += "AND T04_CONTRATO.P0437_TIPO = 'V' AND T04_CONTRATO.P0428_A_PRORROGA = 'V'";
                }
                else
                {
                    sql += "AND T04_CONTRATO.P0437_TIPO IN ('P', 'Q')";
                }
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArrendadora;
                List<AuditoriaContratoEntity> listaAudit = new List<AuditoriaContratoEntity>();
                conexion.Open();
                int count = Convert.ToInt32(comando.ExecuteScalar());
                conexion.Close();
                return count;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        public static List<AuditoriaContratoEntity> getAuditoriaContratos(string idArrendadora, DateTime fechaPago, bool esPreventa)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                /*SELECT T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.P0402_ID_ARRENDAT, T04_CONTRATO.P0404_ID_EDIFICIO, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, 
                T18_SUBCONJUNTOS.P1803_NOMBRE, T04_CONTRATO.CAMPO_DATE1, T04_CONTRATO.P0433_FECHA_ACTUAL, T07_EDIFICIO.CAMPO5, T07_EDIFICIO.CAMPO6, T02_ARRENDATARIO.P0203_NOMBRE, T07_EDIFICIO.P0706_TERRENO_M2, T07_EDIFICIO.P0712_IMPORTE_RENTA,
                T04_CONTRATO.P0418_IMPORTE_DEPOSITO, T07_EDIFICIO.P0707_CONTRUCCION_M2 
                FROM T04_CONTRATO*/
                string sql = @"SELECT T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.P0402_ID_ARRENDAT, T04_CONTRATO.P0404_ID_EDIFICIO, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, 
                T18_SUBCONJUNTOS.P1803_NOMBRE, T04_CONTRATO.CAMPO_DATE1, T04_CONTRATO.P0433_FECHA_ACTUAL, T07_EDIFICIO.CAMPO5, T07_EDIFICIO.CAMPO6, T02_ARRENDATARIO.P0203_NOMBRE,
                T07_EDIFICIO.P0703_NOMBRE, T07_EDIFICIO.P0706_TERRENO_M2, T07_EDIFICIO.P0712_IMPORTE_RENTA,T04_CONTRATO.P0418_IMPORTE_DEPOSITO, T07_EDIFICIO.P0707_CONTRUCCION_M2 
                FROM T04_CONTRATO
                LEFT JOIN T07_EDIFICIO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T04_CONTRATO.P0404_ID_EDIFICIO
                LEFT JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T07_EDIFICIO.P0730_SUBCONJUNTO
                INNER JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T07_EDIFICIO.P0710_ID_CENTRO
                INNER JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T04_CONTRATO.P0402_ID_ARRENDAT ";
                if (!esPreventa)
                {
                    sql += @"WHERE T04_CONTRATO.P0403_ID_ARRENDADORA = ? AND T04_CONTRATO.P0437_TIPO = 'V' AND T04_CONTRATO.P0428_A_PRORROGA = 'V'
                           ORDER BY T18_SUBCONJUNTOS.P1803_NOMBRE, T07_EDIFICIO.CAMPO5, T03_CENTRO_INDUSTRIAL.CAMPO6";
                }
                else
                {
                    sql += @"WHERE T04_CONTRATO.P0403_ID_ARRENDADORA = ? AND T04_CONTRATO.P0437_TIPO IN ('P', 'Q') 
                           ORDER BY T18_SUBCONJUNTOS.P1803_NOMBRE, T07_EDIFICIO.CAMPO5, T03_CENTRO_INDUSTRIAL.CAMPO6";
                }

                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArrendadora;
                List<AuditoriaContratoEntity> listaAudit = new List<AuditoriaContratoEntity>();
                int contador = 0;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    if (CancelacionPendiente)
                        break;
                    contador++;
                    AuditoriaContratoEntity audit = new AuditoriaContratoEntity();
                    audit.IDContrato = reader["P0401_ID_CONTRATO"].ToString();
                    audit.IDCliente = reader["P0402_ID_ARRENDAT"].ToString();
                    audit.IDEdificio = reader["P0404_ID_EDIFICIO"].ToString();
                    audit.IDConjunto = reader["P0301_ID_CENTRO"].ToString();
                    audit.Desarrollo = reader["P0303_NOMBRE"].ToString();
                    audit.Seccion = reader["P1803_NOMBRE"].ToString();
                    audit.FechaContrato = string.IsNullOrEmpty(reader["CAMPO_DATE1"].ToString()) ? DateTime.Now.Date : Convert.ToDateTime(reader["CAMPO_DATE1"]);
                    //audit.FechaContrato = string.IsNullOrEmpty(reader["P0433_FECHA_ACTUAL"].ToString()) ? DateTime.Now.Date : Convert.ToDateTime(reader["P0433_FECHA_ACTUAL"]);
                    audit.Manzana = reader["CAMPO5"].ToString();
                    audit.Lote = reader["CAMPO6"].ToString();
                    audit.Cliente = reader["P0203_NOMBRE"].ToString();
                    var datos = getDatosContacto(audit.IDCliente);
                    if (datos != null)
                    {
                        audit.Telefono1 = datos.Tel1;
                        audit.Telefono2 = datos.Tel2;
                        audit.Mail = datos.Email;
                    }
                    audit.NombreInmueble = reader["P0703_NOMBRE"].ToString();
                    audit.Superficie = string.IsNullOrEmpty(reader["P0706_TERRENO_M2"].ToString()) ? 0 : Convert.ToDecimal(reader["P0706_TERRENO_M2"]);
                    audit.PrecioTotal = string.IsNullOrEmpty(reader["P0712_IMPORTE_RENTA"].ToString()) ? 0 : Convert.ToDecimal(reader["P0712_IMPORTE_RENTA"]);
                    audit.Enganche = string.IsNullOrEmpty(reader["P0418_IMPORTE_DEPOSITO"].ToString()) ? 0 : Convert.ToDecimal(reader["P0418_IMPORTE_DEPOSITO"]);
                    audit.PagoMensual = getPagoMensual(audit.IDContrato);
                    var saldo = getMontosPagados(audit.IDContrato, fechaPago);
                    if (saldo != null)
                    {
                        audit.MontoPagadoCapital = saldo.MontoCapital;
                        audit.MontoPagadoIntereses = saldo.MontoIntereses;
                        audit.MontoPagadoMoratorios = saldo.MontoMoratorios;
                    }
                    audit.SaldoInsoluto = getSaldoInsoluto(audit.IDContrato, fechaPago);
                    List<PagoVencidoEntity> pagosVencidos = getPagosVencidos(audit.IDContrato, fechaPago, idArrendadora);
                    audit.SaldoVencidoCapital = pagosVencidos.Sum(p => p.Capital);
                    audit.SaldoVencidoInteres = pagosVencidos.Sum(p => p.Intereses);
                    audit.SaldoVencidoMoratorios = pagosVencidos.Sum(p => p.Moratorios);
                    audit.MontoParaRegularizar = pagosVencidos.Sum(p => p.TotalVencido);
                    audit.PagaresPagados = getPagaresPagados(audit.IDContrato);
                    audit.PagaresPorPagar = getPagaresPorPagar(audit.IDContrato);
                    audit.MetrosCuadradosTerreno = string.IsNullOrEmpty(reader["P0706_TERRENO_M2"].ToString()) ? 0 : Convert.ToDecimal(reader["P0706_TERRENO_M2"]);
                    audit.MetrosCuadradosConstruccion = string.IsNullOrEmpty(reader["P0707_CONTRUCCION_M2"].ToString()) ? 0 : Convert.ToDecimal(reader["P0707_CONTRUCCION_M2"]);
                    listaAudit.Add(audit);
                    OnObjetoAgregado(contador);
                }
                reader.Close();
                conexion.Close();
                return listaAudit;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static DatosContactoEntity getDatosContacto(string idContacto)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P0603_TIPO_SERV, P0604_DIRECCION, P0605_ORDEN FROM T06_MAIL_WEB 
WHERE P0601_ID_ENTE = ? AND ((P0603_TIPO_SERV = 'T' AND P0605_ORDEN = 1) OR (P0603_TIPO_SERV = 'T' AND P0605_ORDEN = 2) OR (P0603_TIPO_SERV = 'E' AND P0605_ORDEN = 1))";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDContact", OdbcType.VarChar).Value = idContacto;
                DatosContactoEntity datos = new DatosContactoEntity();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["P0603_TIPO_SERV"].ToString() == "T" && Convert.ToInt32(reader["P0605_ORDEN"]) == 1)
                        datos.Tel1 = reader["P0604_DIRECCION"].ToString();
                    else if (reader["P0603_TIPO_SERV"].ToString() == "T" && Convert.ToInt32(reader["P0605_ORDEN"]) == 2)
                        datos.Tel2 = reader["P0604_DIRECCION"].ToString();
                    else
                        datos.Email = reader["P0604_DIRECCION"].ToString();
                }
                reader.Close();
                conexion.Close();
                return datos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static decimal getPagoMensual(string idContrato)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT CAMPO_NUM4, CAMPO_NUM5 FROM T24_HISTORIA_RECIBOS WHERE P2412_CONCEPTO LIKE '%Pagare%' AND P2412_CONCEPTO NOT LIKE '%Enganche%' AND P2418_ID_CONTRATO = ? AND P2406_STATUS = '1'";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = idContrato;
                decimal capital = 0, intereses = 0;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    capital = string.IsNullOrEmpty(reader["CAMPO_NUM4"].ToString()) ? 0 : Convert.ToDecimal(reader["CAMPO_NUM4"]);
                    intereses = string.IsNullOrEmpty(reader["CAMPO_NUM5"].ToString()) ? 0 : Convert.ToDecimal(reader["CAMPO_NUM5"]);
                    break;
                }
                reader.Close();
                conexion.Close();
                return capital + intereses;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        public static SaldosAuditoriaEntity getMontosPagados(string idContrato, DateTime fechaPago)
        {//TODO: Redondear en excel de contpaq xls
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT SUM(CAMPO_NUM1) AS Moratorios, SUM(CAMPO_NUM4) AS Capital, SUM(CAMPO_NUM5) AS Intereses
FROM T24_HISTORIA_RECIBOS WHERE P2418_ID_CONTRATO = ? AND P2406_STATUS = '2' AND P2408_FECHA_PAGADO <= ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = idContrato;
                comando.Parameters.Add("@Fecha", OdbcType.Date).Value = fechaPago.Date;
                SaldosAuditoriaEntity saldo = null;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    saldo = new SaldosAuditoriaEntity();
                    saldo.MontoCapital = string.IsNullOrEmpty(reader["Capital"].ToString()) ? 0 : Convert.ToDecimal(reader["Capital"]);
                    saldo.MontoIntereses = string.IsNullOrEmpty(reader["Intereses"].ToString()) ? 0 : Convert.ToDecimal(reader["Intereses"]);
                    saldo.MontoMoratorios = string.IsNullOrEmpty(reader["Moratorios"].ToString()) ? 0 : Convert.ToDecimal(reader["Moratorios"]);
                }
                reader.Close();
                conexion.Close();
                return saldo;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static decimal getSaldoInsoluto(string idContrato, DateTime fechaPago)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT SUM(CAMPO_NUM4) AS SaldoInsoluto FROM T24_HISTORIA_RECIBOS 
WHERE P2418_ID_CONTRATO = ? AND (P2406_STATUS = '1' OR (P2406_STATUS <> '1' AND P2408_FECHA_PAGADO > ?))";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = idContrato;
                comando.Parameters.Add("@FechaPago", OdbcType.Date).Value = fechaPago;
                conexion.Open();
                decimal result = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        public static List<PagoVencidoEntity> getPagosVencidos(string idContrato, DateTime fechaCorte, string idArrendadora)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P2444_ID_HIST_REC, CAMPO_NUM4, CAMPO_NUM5, CAMPO_DATE1, P2428_T_IVA
FROM T24_HISTORIA_RECIBOS WHERE P2418_ID_CONTRATO = ? AND ((P2406_STATUS = '1' AND CAMPO_DATE1 < ?) 
OR (P2406_STATUS <> '1' AND CAMPO_DATE1 < ? AND P2408_FECHA_PAGADO > ?))";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = idContrato;
                comando.Parameters.Add("@FechaVenc", OdbcType.Date).Value = fechaCorte.Date;
                comando.Parameters.Add("@FechaVenc2", OdbcType.Date).Value = fechaCorte.Date;
                comando.Parameters.Add("@FechaPago", OdbcType.Date).Value = fechaCorte.Date;
                List<PagoVencidoEntity> listaPagos = new List<PagoVencidoEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    PagoVencidoEntity pago = new PagoVencidoEntity();
                    pago.IDHistRec = (int)reader["P2444_ID_HIST_REC"];
                    pago.Capital = string.IsNullOrEmpty(reader["CAMPO_NUM4"].ToString()) ? 0 : Convert.ToDecimal(reader["CAMPO_NUM4"]);
                    pago.Intereses = string.IsNullOrEmpty(reader["CAMPO_NUM5"].ToString()) ? 0 : Convert.ToDecimal(reader["CAMPO_NUM5"]);
                    pago.FechaVencimiento = string.IsNullOrEmpty(reader["CAMPO_DATE1"].ToString()) ? DateTime.Now.Date : Convert.ToDateTime(reader["CAMPO_DATE1"]);
                    pago.TasaIVA = reader["P2428_T_IVA"].ToString();
                    pago.Moratorios = getMoratorios(pago, fechaCorte.Date, idContrato, idArrendadora);
                    listaPagos.Add(pago);
                }
                reader.Close();
                conexion.Close();
                return listaPagos;
            }
            catch
            {
                conexion.Close();
                return new List<PagoVencidoEntity>();
            }
        }

        public static decimal getMoratorios(PagoVencidoEntity pago, DateTime fechaCorte, string idContrato, string idArr)
        {
            try
            {
                TimeSpan tiempoAtraso = fechaCorte.Date - pago.FechaVencimiento.Date;
                decimal prcFijo = getBaseMoratorios(idContrato);
                decimal tasaInteres = prcFijo;
                int tipoMoratorio = getTipoMoratorios(idArr);
                decimal moratorios = 0;
                if (tipoMoratorio == 2)
                {
                    int mesesTranscurridos = ((fechaCorte.Year - pago.FechaVencimiento.Year) * 12) + fechaCorte.Month - pago.FechaVencimiento.Month;
                    if (mesesTranscurridos > 0)
                        moratorios = (pago.Capital + pago.Intereses) * (tasaInteres / 100) * mesesTranscurridos;
                }
                else if (tipoMoratorio == 1)
                {
                    if (tiempoAtraso.Days > 0)
                        moratorios = (pago.Capital + pago.Intereses) * (tasaInteres / 100) / 30.4m * tiempoAtraso.Days;
                }
                //return moratorios;                
                decimal tasaIva = getPorcentajeImpuesto(pago.TasaIVA);
                decimal ivaMoratorios = moratorios * tasaIva;
                return moratorios + ivaMoratorios;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static decimal getBaseMoratorios(string idContrato)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT CAMPO_NUM2 FROM T04_CONTRATO WHERE P0401_ID_CONTRATO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDCnt", OdbcType.VarChar).Value = idContrato;
                conexion.Open();
                decimal baseMor = Convert.ToDecimal(comando.ExecuteScalar());
                conexion.Close();
                return baseMor;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        //public static int getTipoMoratorios(string idArrendadora)
        //{
        //    OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
        //    try
        //    {
        //        string sql = "SELECT CAMPO_NUM1 FROM T01_ARRENDADORA WHERE P0101_ID_ARR = ?";
        //        OdbcCommand comando = new OdbcCommand(sql, conexion);
        //        comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArrendadora;
        //        conexion.Open();
        //        int tipoMor = Convert.ToInt32(comando.ExecuteScalar());
        //        conexion.Close();
        //        return tipoMor;
        //    }
        //    catch
        //    {
        //        conexion.Close();
        //        return 0;
        //    }
        //}

        public static decimal getPorcentajeImpuesto(string tasa)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P3002_NUMERADOR, P3003_DENOMINADOR FROM T30_R_IMPUESTOS WHERE P3001_CLAVE = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@Tasa", OdbcType.VarChar).Value = tasa;
                conexion.Open();
                decimal numerador = 0, denominador = 0;
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    numerador = Convert.ToDecimal(reader["P3002_NUMERADOR"]);
                    denominador = Convert.ToDecimal(reader["P3003_DENOMINADOR"]);
                    break;
                }
                conexion.Close();
                return numerador / denominador;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        private static int getPagaresPagados(string IDContrato)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT COUNT(*) FROM T24_HISTORIA_RECIBOS WHERE P2418_ID_CONTRATO = ? AND P2406_STATUS = '2' 
                        AND P2412_CONCEPTO LIKE '%Pagare%' AND P2412_CONCEPTO NOT LIKE '%Enganche%'";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDContrato", OdbcType.VarChar).Value = IDContrato;
                conexion.Open();
                int pagaresPagados = Convert.ToInt32(comando.ExecuteScalar());
                conexion.Close();
                return pagaresPagados;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        private static int getPagaresPorPagar(string IDContrato)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT Count(*) FROM T24_HISTORIA_RECIBOS WHERE P2418_ID_CONTRATO = ? AND P2406_STATUS = '1' 
                        AND P2412_CONCEPTO LIKE '%Pagare%' AND P2412_CONCEPTO NOT LIKE '%Enganche%'";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDContrato", OdbcType.VarChar).Value = IDContrato;
                conexion.Open();
                int pagaresPorPagar = Convert.ToInt32(comando.ExecuteScalar());
                conexion.Close();
                return pagaresPorPagar;
            }
            catch
            {
                conexion.Close();
                return 0;
            }
        }

        #endregion

        #region Reporte de egresos - provision

        public static ProveedorEntity getProveedorById(string id)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P1001_ID_PROV, P1003_NOMBRE FROM T10_PROVEEDOR WHERE P1001_ID_PROV = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idProv", OdbcType.VarChar).Value = id;
                ProveedorEntity proveedor = new ProveedorEntity();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    proveedor.IDProveedor = id;
                    proveedor.Nombre = reader["P1003_NOMBRE"].ToString();
                    break;
                }
                reader.Close();
                conexion.Close();
                return proveedor;
            }
            catch (Exception ex)
            {
                conexion.Close();
                ErrorCXP = ex.Message;
                return null;
            }
        }

        public static List<CuentaBancariaEntity> getCuentasBancarias(string idArrendadora)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P5301_ID_CUENTA, P5302_ID_EMPRESA, P5303_NUMERO_CUENTA, P5304_DESCR_CUENTA, P5305_BANCO, P5308_MONEDA
                FROM T53_BANCOS WHERE P5302_ID_EMPRESA = ? ORDER BY P5304_DESCR_CUENTA";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArrendadora;
                List<CuentaBancariaEntity> listaCuentas = new List<CuentaBancariaEntity>() { new CuentaBancariaEntity() { ID = "*Todas", Descripcion = "*Todas" } };
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    CuentaBancariaEntity cuenta = new CuentaBancariaEntity();
                    cuenta.ID = reader["P5301_ID_CUENTA"].ToString();
                    cuenta.IDEmpresa = reader["P5302_ID_EMPRESA"].ToString();
                    cuenta.NumeroCuenta = reader["P5303_NUMERO_CUENTA"].ToString();
                    cuenta.Descripcion = reader["P5304_DESCR_CUENTA"].ToString();
                    cuenta.Banco = reader["P5305_BANCO"].ToString();
                    cuenta.Moneda = reader["P5308_MONEDA"].ToString();
                    listaCuentas.Add(cuenta);
                }
                reader.Close();
                conexion.Close();
                return listaCuentas;
            }
            catch
            {
                conexion.Close();
                //return new List<CuentaBancariaEntity>() { new CuentaBancariaEntity() { ID = "*Todas", Descripcion = "*Todas" } };
                return null;
            }
        }

        public static List<string> getClasificaciones()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT DESCR_CAT FROM CATEGORIA WHERE ID_COT = 'CLASGASTO' ORDER BY DESCR_CAT";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                List<string> listaClas = new List<string>() { "*Todas" };
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    listaClas.Add(reader[0].ToString());
                }
                reader.Close();
                conexion.Close();
                return listaClas;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<EgresoEntity> getRegistrosEgreso(string idArrendadora, DateTime fechaInicio, DateTime fechaFin, string moneda, bool incluirCancelados, string idCuenta = "*Todas", string clasificacion = "*Todas", Orden orden = Orden.Ninguno)
        {
            try
            {
                List<EgresoEntity> listaEgresos = new List<EgresoEntity>();
                string sql = string.Empty;
                using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    if (incluirCancelados)
                    {
                        sql = @"SELECT T54_HISTORIA_CXP.P5405_FECHA_GASTO, T54_HISTORIA_CXP.P5408_NUM_CHEQUE, T54_HISTORIA_CXP.P5418_ESTATUS_CHEQUE, 
                        T54_HISTORIA_CXP.P5404_NOMBRE_PROVEEDOR, T56_HISTORIA_DETALLE_CXP.P5610_CONCEPTO, T56_HISTORIA_DETALLE_CXP.P5603_CLASIFICACION,
                        T56_HISTORIA_DETALLE_CXP.P5605_NOMBRE_INMUEBLE, T56_HISTORIA_DETALLE_CXP.P5607_NOMBRE_CONJUNTO, T56_HISTORIA_DETALLE_CXP.P5611_IMPORTE_GASTO,
                        T56_HISTORIA_DETALLE_CXP.P5612_IVA_GASTO, T56_HISTORIA_DETALLE_CXP.P5613_TOTAL_GASTO, T54_HISTORIA_CXP.P5402_ID_CUENTA_BANCO, T54_HISTORIA_CXP.P5412_MONEDA
                        FROM T54_HISTORIA_CXP
                        JOIN T56_HISTORIA_DETALLE_CXP ON T54_HISTORIA_CXP.P5401_ID_HIST_REC = T56_HISTORIA_DETALLE_CXP.P5601_ID_HIST_REC
                        WHERE T56_HISTORIA_DETALLE_CXP.P5602_ID_EMPRESA = ? AND T54_HISTORIA_CXP.P5405_FECHA_GASTO >= ? AND T54_HISTORIA_CXP.P5405_FECHA_GASTO <= ?";
                    }
                    else
                    {
                        sql = @"SELECT T54_HISTORIA_CXP.P5405_FECHA_GASTO, T54_HISTORIA_CXP.P5408_NUM_CHEQUE, T54_HISTORIA_CXP.P5418_ESTATUS_CHEQUE, 
                        T54_HISTORIA_CXP.P5404_NOMBRE_PROVEEDOR, T56_HISTORIA_DETALLE_CXP.P5610_CONCEPTO, T56_HISTORIA_DETALLE_CXP.P5603_CLASIFICACION,
                        T56_HISTORIA_DETALLE_CXP.P5605_NOMBRE_INMUEBLE, T56_HISTORIA_DETALLE_CXP.P5607_NOMBRE_CONJUNTO, T56_HISTORIA_DETALLE_CXP.P5611_IMPORTE_GASTO,
                        T56_HISTORIA_DETALLE_CXP.P5612_IVA_GASTO, T56_HISTORIA_DETALLE_CXP.P5613_TOTAL_GASTO, T54_HISTORIA_CXP.P5402_ID_CUENTA_BANCO, T54_HISTORIA_CXP.P5412_MONEDA
                        FROM T54_HISTORIA_CXP
                        JOIN T56_HISTORIA_DETALLE_CXP ON T54_HISTORIA_CXP.P5401_ID_HIST_REC = T56_HISTORIA_DETALLE_CXP.P5601_ID_HIST_REC
                        WHERE T56_HISTORIA_DETALLE_CXP.P5602_ID_EMPRESA = ? AND T54_HISTORIA_CXP.P5405_FECHA_GASTO >= ? AND T54_HISTORIA_CXP.P5405_FECHA_GASTO <= ? AND T54_HISTORIA_CXP.P5418_ESTATUS_CHEQUE != 'CANCELADO'";
                    }
                    using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = idArrendadora;
                        comando.Parameters.Add("@FechaIni", OdbcType.Date).Value = fechaInicio.Date;
                        comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaFin.Date;
                        conexion.Open();
                        using (OdbcDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EgresoEntity egreso = new EgresoEntity();
                                egreso.Fecha = reader["P5405_FECHA_GASTO"] == DBNull.Value ? new DateTime() : Convert.ToDateTime(reader["P5405_FECHA_GASTO"]);
                                egreso.NumeroCheque = reader["P5408_NUM_CHEQUE"].ToString();
                                egreso.Estatus = reader["P5418_ESTATUS_CHEQUE"].ToString();
                                egreso.Beneficiario = reader["P5404_NOMBRE_PROVEEDOR"].ToString();
                                egreso.Concepto = reader["P5610_CONCEPTO"].ToString();
                                egreso.Clasificacion = reader["P5603_CLASIFICACION"].ToString();
                                egreso.Inmueble = reader["P5605_NOMBRE_INMUEBLE"].ToString();
                                egreso.Conjunto = reader["P5607_NOMBRE_CONJUNTO"].ToString();
                                egreso.Importe = reader["P5611_IMPORTE_GASTO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P5611_IMPORTE_GASTO"]);
                                egreso.Impuesto = reader["P5612_IVA_GASTO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P5612_IVA_GASTO"]);
                                egreso.Total = reader["P5613_TOTAL_GASTO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P5613_TOTAL_GASTO"]);
                                egreso.IDCuenta = reader["P5402_ID_CUENTA_BANCO"].ToString();
                                egreso.Moneda = reader["P5412_MONEDA"].ToString();
                                listaEgresos.Add(egreso);
                            }
                        }
                    }
                }
                if (idCuenta != "*Todas")
                    listaEgresos = listaEgresos.Where(e => e.IDCuenta == idCuenta).ToList();
                else
                    listaEgresos = listaEgresos.Where(e => e.Moneda == moneda).ToList();
                if (clasificacion != "*Todas")
                    listaEgresos = listaEgresos.Where(e => e.Clasificacion == clasificacion).ToList();
                if (orden == Orden.Fecha)
                    listaEgresos = listaEgresos.OrderBy(e => e.Fecha).ToList();
                else if (orden == Orden.Estatus)
                    listaEgresos = listaEgresos.OrderBy(e => e.Estatus).ToList();
                else if (orden == Orden.Clasificacion)
                    listaEgresos = listaEgresos.OrderBy(e => e.Clasificacion).ToList();
                else if (orden == Orden.Beneficiario)
                    listaEgresos = listaEgresos.OrderBy(e => e.Beneficiario).ToList();
                return listaEgresos;
            }
            catch
            {
                return null;
            }

        }

        public static string ErrorCXP = string.Empty;

        public static List<ProvisionEntity> getProvisionesEgreso(CXPConfiguracionReporte Config)
        {
            //El encabezado de las provisiones
            List<ProvisionEntity> listaProvisiones = new List<ProvisionEntity>();
            //Los gastos detalle de las provisiones
            List<GastosEntity> listaGastos = new List<GastosEntity>();
            //Las provisiones de cheques
            DataTable ProvisionesDT = getProvisionesPendientesDePago(Config);
            //El detalle de las provisiones de cheques
            DataTable GastosDT = getGastosPendientesDePago(Config);

            #region Obtener datos del detalle de las provisiones
            foreach (DataRow gastosDr in GastosDT.Rows)
            {
                GastosEntity gastos = new GastosEntity();
                try
                {
                    gastos.IDHistRecProvision = Convert.ToInt32(gastosDr["P5401_ID_HIST_REC"]);
                    gastos.IDEmpresa = gastosDr["P5414_ID_EMPRESA"].ToString();
                    gastos.Clasificacion = gastosDr["P5603_CLASIFICACION"] == DBNull.Value ? string.Empty : gastosDr["P5603_CLASIFICACION"].ToString();
                    gastos.IDInmueble = gastosDr["P5604_ID_INMUEBLE"] == DBNull.Value ? string.Empty : gastosDr["P5604_ID_INMUEBLE"].ToString();
                    gastos.NombreInmueble = gastosDr["P5605_NOMBRE_INMUEBLE"] == DBNull.Value ? string.Empty : gastosDr["P5605_NOMBRE_INMUEBLE"].ToString();
                    gastos.IDInmueble = gastosDr["P5604_ID_INMUEBLE"] == DBNull.Value ? string.Empty : gastosDr["P5604_ID_INMUEBLE"].ToString();
                    gastos.IDConjunto = gastosDr["P5606_ID_CONJUNTO"] == DBNull.Value ? string.Empty : gastosDr["P5606_ID_CONJUNTO"].ToString();
                    gastos.NombreConjunto = gastosDr["P5607_NOMBRE_CONJUNTO"] == DBNull.Value ? string.Empty : gastosDr["P5607_NOMBRE_CONJUNTO"].ToString();
                    gastos.ConceptoGasto = gastosDr["P5610_CONCEPTO"] == DBNull.Value ? string.Empty : gastosDr["P5610_CONCEPTO"].ToString();
                    gastos.ImporteGasto = gastosDr["P5611_IMPORTE_GASTO"] == DBNull.Value ? 0 : Convert.ToDecimal(gastosDr["P5611_IMPORTE_GASTO"]);
                    gastos.IvaGasto = gastosDr["P5612_IVA_GASTO"] == DBNull.Value ? 0 : Convert.ToDecimal(gastosDr["P5612_IVA_GASTO"]);
                    gastos.TotalGasto = gastosDr["P5613_TOTAL_GASTO"] == DBNull.Value ? 0 : Convert.ToDecimal(gastosDr["P5613_TOTAL_GASTO"]);
                    gastos.ImporteRetIVA = gastosDr["P5447_IMPORTE_RET_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(gastosDr["P5447_IMPORTE_RET_IVA"]);
                    gastos.ImporteRetISR = gastosDr["P5444_IMPORTE_RET_ISR"] == DBNull.Value ? 0 : Convert.ToDecimal(gastosDr["P5444_IMPORTE_RET_ISR"]);
                    gastos.Renglon = gastosDr["P5619_RENGLON"] == DBNull.Value ? 0 : Convert.ToInt32(gastosDr["P5619_RENGLON"]);
                    listaGastos.Add(gastos);
                }
                catch (Exception ex)
                {
                    ErrorCXP = "Error al asignar los datos del detalle de la provisión." + Environment.NewLine + ex.Message;
                    return null;
                }
            }
            #endregion
            #region Obtener Datos de las provisiones
            foreach (DataRow provisionDr in ProvisionesDT.Rows)
            {
                try
                {
                    ProvisionEntity provision = new ProvisionEntity();
                    provision.IDHistRec = Convert.ToInt32(provisionDr["P5401_ID_HIST_REC"]);
                    provision.IDCuentaBanco = provisionDr["P5402_ID_CUENTA_BANCO"].ToString();
                    provision.CuentaBanco = provisionDr["P5303_NUMERO_CUENTA"] == DBNull.Value ? "N/I" : provisionDr["P5303_NUMERO_CUENTA"].ToString();
                    provision.IDProveedor = provisionDr["P5403_ID_PROVEEDOR"].ToString();
                    provision.NombreProveedor = provisionDr["P5404_NOMBRE_PROVEEDOR"].ToString();
                    provision.FechaGasto = Convert.ToDateTime(provisionDr["P5405_FECHA_GASTO"]);
                    DateTime? fechaG = null;
                    fechaG = provisionDr["P5406_FECHA_GENERACION_CHEQUE"] != DBNull.Value ? Convert.ToDateTime(provisionDr["P5406_FECHA_GENERACION_CHEQUE"]) : fechaG;
                    provision.FechaGeneracionCheque = fechaG;
                    DateTime? fechaI = null;
                    fechaI = provisionDr["P5407_FECHA_IMPRESA_CHEQUE"] != DBNull.Value ? Convert.ToDateTime(provisionDr["P5407_FECHA_IMPRESA_CHEQUE"]) : fechaI;
                    provision.FechaImpresionCheque = fechaI;
                    provision.NumeroCheque = provisionDr["P5408_NUM_CHEQUE"] == DBNull.Value ? 0 : Convert.ToInt32(provisionDr["P5408_NUM_CHEQUE"]);
                    provision.ImporteGasto = Convert.ToDecimal(provisionDr["P5409_IMPORTE_GASTO"]);
                    provision.IvaGasto = Convert.ToDecimal(provisionDr["P5410_IVA_GASTO"]);
                    provision.TotalCheque = Convert.ToDecimal(provisionDr["P5411_TOTAL_CHEQUE"]);
                    provision.ImporteRetIVA = provisionDr["P5447_IMPORTE_RET_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(provisionDr["P5447_IMPORTE_RET_IVA"]);
                    provision.ImporteRetISR = provisionDr["P5444_IMPORTE_RET_ISR"] == DBNull.Value ? 0 : Convert.ToDecimal(provisionDr["P5444_IMPORTE_RET_ISR"]);
                    provision.Moneda = provisionDr["P5412_MONEDA"] == DBNull.Value ? "P" : provisionDr["P5412_MONEDA"].ToString();
                    provision.TipoCambio = Convert.ToDecimal(provisionDr["P5412_TIPO_CAMBIO"]);
                    provision.ConceptoGasto = provisionDr["P5413_CONCEPTO"] == DBNull.Value ? string.Empty : provisionDr["P5413_CONCEPTO"].ToString();
                    provision.IDEmpresa = provisionDr["P5414_ID_EMPRESA"].ToString();
                    //provision.NombreComercialEmpresa = Config.NombreComercialInmobiliaria;
                    //provision.RazonSocialEmpresa = Config.RazonSocialInmobiliaria;
                    provision.EstatusCheque = provisionDr["P5418_ESTATUS_CHEQUE"] == DBNull.Value ? "Pendiente" : provisionDr["P5418_ESTATUS_CHEQUE"].ToString() == "" ? "Pendiente" : provisionDr["P5418_ESTATUS_CHEQUE"].ToString();
                    provision.EstatusCXP = provisionDr["P5417_ESTATUS_CXP"] == DBNull.Value ? "" : provisionDr["P5417_ESTATUS_CXP"].ToString();
                    DateTime? fechaC = null;
                    provision.FechaCancelacion = provisionDr["P5441_FECHA_CANC"] != DBNull.Value ? Convert.ToDateTime(provisionDr["P5441_FECHA_CANC"]) : fechaC;
                    provision.Usuario = provisionDr["P5422_USUARIO_UPDT"] == DBNull.Value ? string.Empty : provisionDr["P5422_USUARIO_UPDT"].ToString();
                    provision.ListaGastos = new List<GastosEntity>();
                    provision.ListaGastos = listaGastos.Where(l => l.IDHistRecProvision == provision.IDHistRec).ToList();
                    listaProvisiones.Add(provision);
                }
                catch (Exception ex)
                {
                    ErrorCXP = "Error al asignar los datos de la provisión." + Environment.NewLine + ex.Message;
                    return null;
                }
            }
            #endregion
            return listaProvisiones;
        }

        private static DataTable getProvisionesPendientesDePago(CXPConfiguracionReporte Config)
        {
            DataTable ProvisionesDT = new DataTable();
            string sql = string.Empty;
            using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
            {
                sql = @"SELECT T54_HISTORIA_CXP.* , T53_BANCOS.P5303_NUMERO_CUENTA  FROM T54_HISTORIA_CXP
                        LEFT JOIN T53_BANCOS ON T53_BANCOS.P5301_ID_CUENTA = T54_HISTORIA_CXP.P5402_ID_CUENTA_BANCO
                        WHERE T54_HISTORIA_CXP.P5414_ID_EMPRESA = ? AND T54_HISTORIA_CXP.P5405_FECHA_GASTO <= ? 
                        AND (T54_HISTORIA_CXP.P5417_ESTATUS_CXP='xGenerarCh' OR (T54_HISTORIA_CXP.P5417_ESTATUS_CXP='ChGenerado' 
                        AND (T54_HISTORIA_CXP.P5418_ESTATUS_CHEQUE = 'Generado' OR T54_HISTORIA_CXP.P5418_ESTATUS_CHEQUE= 'En Firma'))) ";
                sql += " ORDER BY T54_HISTORIA_CXP.P5405_FECHA_GASTO";
                using (OdbcCommand comando = new OdbcCommand(sql, conexion))
                {
                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = Config.IDInmobiliaria;
                    comando.Parameters.Add("@FechaCorte", OdbcType.Date).Value = Config.FechaCorte;
                    conexion.Open();
                    try
                    {
                        OdbcDataReader reader = comando.ExecuteReader();
                        ProvisionesDT.Load(reader);
                        conexion.Close();
                    }
                    catch (Exception ex)
                    {
                        ErrorCXP = ex.Message;
                        conexion.Close();
                        return null;
                    }
                }
            }
            return ProvisionesDT;
        }



        /// <summary>
        /// Obtiene el detalle de los gastos pendientes de pagos (cheques sin entregar o sin generar)
        /// </summary>
        /// <param name="Config"></param>
        /// <returns></returns>
        private static DataTable getGastosPendientesDePago(CXPConfiguracionReporte Config)
        {
            DataTable GastosDt = new DataTable();
            string sql = string.Empty;
            string sqlGastos = string.Empty;
            using (OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
            {
                if (Config.IncluirCancelados)
                {
                    sqlGastos = @"SELECT P5401_ID_HIST_REC, P5405_FECHA_GASTO, P5413_CONCEPTO, P5610_CONCEPTO, P5417_ESTATUS_CXP, P5418_ESTATUS_CHEQUE, P5402_ID_CUENTA_BANCO, P5408_NUM_CHEQUE,   
                            P5403_ID_PROVEEDOR, P5404_NOMBRE_PROVEEDOR, P5603_CLASIFICACION, P5414_ID_EMPRESA, P5604_ID_INMUEBLE, 
                            P5605_NOMBRE_INMUEBLE, P5606_ID_CONJUNTO, P5607_NOMBRE_CONJUNTO, P5412_MONEDA, P5412_TIPO_CAMBIO, P5425_ID_IMPUESTO,
                            P5409_IMPORTE_GASTO, P5410_IVA_GASTO, P5411_TOTAL_CHEQUE, P5611_IMPORTE_GASTO, P5612_IVA_GASTO, P5613_TOTAL_GASTO, 
                            P5444_IMPORTE_RET_ISR, P5447_IMPORTE_RET_IVA, P5406_FECHA_GENERACION_CHEQUE, P5407_FECHA_IMPRESA_CHEQUE, P5441_FECHA_CANC,
                            P5619_RENGLON, P5422_USUARIO_UPDT    
                            FROM T54_HISTORIA_CXP
                            JOIN T56_HISTORIA_DETALLE_CXP ON T54_HISTORIA_CXP.P5401_ID_HIST_REC = T56_HISTORIA_DETALLE_CXP.P5601_ID_HIST_REC                            
                            WHERE T56_HISTORIA_DETALLE_CXP.P5602_ID_EMPRESA = ? AND T54_HISTORIA_CXP.P5405_FECHA_GASTO <= ? 
                            AND (T54_HISTORIA_CXP.P5417_ESTATUS_CXP='xGenerarCh' OR (T54_HISTORIA_CXP.P5417_ESTATUS_CXP='ChGenerado' 
                                AND (T54_HISTORIA_CXP.P5418_ESTATUS_CHEQUE = 'Generado' OR T54_HISTORIA_CXP.P5418_ESTATUS_CHEQUE= 'En Firma')))
                            OR T54_HISTORIA_CXP.P5418_ESTATUS_CHEQUE != 'CANCELADO'";
                    if (Config.EsPorConjunto)
                        sqlGastos += " AND T56_HISTORIA_DETALLE_CXP.P5606_ID_CONJUNTO = ? ";
                    sqlGastos += " ORDER BY T54_HISTORIA_CXP.P5405_FECHA_GASTO";
                }
                else
                {
                    sqlGastos = @"SELECT P5401_ID_HIST_REC, P5405_FECHA_GASTO, P5413_CONCEPTO, P5610_CONCEPTO, P5417_ESTATUS_CXP, P5418_ESTATUS_CHEQUE, P5402_ID_CUENTA_BANCO, P5408_NUM_CHEQUE,   
                    P5403_ID_PROVEEDOR, P5404_NOMBRE_PROVEEDOR, P5603_CLASIFICACION, P5414_ID_EMPRESA, P5604_ID_INMUEBLE, 
                    P5605_NOMBRE_INMUEBLE, P5606_ID_CONJUNTO, P5607_NOMBRE_CONJUNTO, P5412_MONEDA, P5412_TIPO_CAMBIO, P5425_ID_IMPUESTO,
                    P5409_IMPORTE_GASTO, P5410_IVA_GASTO, P5411_TOTAL_CHEQUE, P5611_IMPORTE_GASTO, P5612_IVA_GASTO, P5613_TOTAL_GASTO, 
                    P5444_IMPORTE_RET_ISR, P5447_IMPORTE_RET_IVA, P5406_FECHA_GENERACION_CHEQUE, P5407_FECHA_IMPRESA_CHEQUE, P5441_FECHA_CANC,
                    P5619_RENGLON, P5422_USUARIO_UPDT    
                    FROM T54_HISTORIA_CXP
                    JOIN T56_HISTORIA_DETALLE_CXP ON T54_HISTORIA_CXP.P5401_ID_HIST_REC = T56_HISTORIA_DETALLE_CXP.P5601_ID_HIST_REC                    
                    WHERE T56_HISTORIA_DETALLE_CXP.P5602_ID_EMPRESA = ? AND T54_HISTORIA_CXP.P5405_FECHA_GASTO <= ? 
                    AND (T54_HISTORIA_CXP.P5417_ESTATUS_CXP='xGenerarCh' OR (T54_HISTORIA_CXP.P5417_ESTATUS_CXP='ChGenerado' 
                        AND (T54_HISTORIA_CXP.P5418_ESTATUS_CHEQUE = 'Generado' OR T54_HISTORIA_CXP.P5418_ESTATUS_CHEQUE= 'En Firma')))";
                    if (Config.EsPorConjunto)
                        sqlGastos += " AND T56_HISTORIA_DETALLE_CXP.P5606_ID_CONJUNTO = ? ";
                    sqlGastos += " ORDER BY T54_HISTORIA_CXP.P5405_FECHA_GASTO";
                }
                using (OdbcCommand comando = new OdbcCommand(sqlGastos, conexion))
                {
                    comando.Parameters.Add("@IDArr", OdbcType.VarChar).Value = Config.IDInmobiliaria;
                    comando.Parameters.Add("@FechaCorte", OdbcType.Date).Value = Config.FechaCorte;
                    if (Config.EsPorConjunto)
                        comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = Config.IDConjunto;
                    conexion.Open();

                    using (OdbcDataReader reader = comando.ExecuteReader())
                    {
                        GastosDt.Load(reader);
                    }
                    conexion.Close();
                }
            }
            return GastosDt;
        }

        #endregion

        #region Reporte de recibos cancelados

        private static string getNombreInmuebleByID(string idInmueble)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT P0703_NOMBRE FROM T07_EDIFICIO WHERE P0701_ID_EDIFICIO = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmueble", OdbcType.VarChar).Value = idInmueble;
                conexion.Open();
                string result = comando.ExecuteScalar().ToString();
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }

        public static List<ReciboEntity> getListaRecibosCancelados(string idInmobiliaria, DateTime fechaInicio, DateTime fechaFin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT * FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND P2406_STATUS = '3' AND P2408_FECHA_PAGADO BETWEEN ? AND ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@FechaInicio", OdbcType.Date).Value = fechaInicio.Date;
                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaFin.Date;
                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    recibo.IDHistRec = Convert.ToInt32(reader["P2444_ID_HIST_REC"]);
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.FechaCancelado = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    //recibo.FechaCancelado = Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"];
                    recibo.UUID = reader["P4002_ID_CFD"] == DBNull.Value ? "" : SaariE.getUUIDByIDCfd(Convert.ToInt32(reader["P4002_ID_CFD"]));
                    recibo.Estatus = "CANCELADO";
                    recibo.Inmueble = reader["CAMPO3"].ToString();
                    recibo.NombreInmueble = getNombreInmuebleByID(recibo.Inmueble);
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    recibo.NombreCliente = reader["P2411_N_ARRENDATARIO"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 1 : (decimal)reader["P2414_TIPO_CAMBIO"];
                    recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    recibo.TotalIVA = reader["P2416_IVA"] == DBNull.Value ? 1 : (decimal)reader["P2416_IVA"];
                    recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getListaRecibosCancelados(string idInmobiliaria, string idConjunto, DateTime fechaInicio, DateTime fechaFin)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT * FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND CAMPO4 = ? AND P2406_STATUS = '3' AND P2408_FECHA_PAGADO BETWEEN ? AND ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = idConjunto;
                comando.Parameters.Add("@FechaInicio", OdbcType.Date).Value = fechaInicio.Date;
                comando.Parameters.Add("@FechaFin", OdbcType.Date).Value = fechaFin.Date;
                List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    recibo.IDHistRec = Convert.ToInt32(reader["P2444_ID_HIST_REC"]);
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.FechaCancelado = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    //recibo.FechaCancelado = Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"];
                    recibo.UUID = reader["P4002_ID_CFD"] == DBNull.Value ? "" : SaariE.getUUIDByIDCfd(Convert.ToInt32(reader["P4002_ID_CFD"]));
                    recibo.Estatus = "CANCELADO";
                    recibo.Inmueble = reader["CAMPO3"].ToString();
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 1 : (decimal)reader["P2414_TIPO_CAMBIO"];
                    recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    recibo.TotalIVA = reader["P2416_IVA"] == DBNull.Value ? 1 : (decimal)reader["P2416_IVA"];
                    recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static ReciboEntity getDetalleReciboPorCancelar(string idInmobiliaria, int idCfd)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT * FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND T40_CFD.P4002_ID_CFD = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@IDCfd", OdbcType.Int).Value = idCfd;
                ReciboEntity recibo = new ReciboEntity();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    recibo.IDHistRec = Convert.ToInt32(reader["P2444_ID_HIST_REC"]);
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.FechaCancelado = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    //recibo.FechaCancelado = Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"];
                    recibo.Inmueble = reader["CAMPO3"].ToString();
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 1 : (decimal)reader["P2414_TIPO_CAMBIO"];
                    recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    recibo.TotalIVA = reader["P2416_IVA"] == DBNull.Value ? 1 : (decimal)reader["P2416_IVA"];
                    recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    break;
                }
                reader.Close();
                conexion.Close();
                return recibo;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static ReciboEntity getDetalleReciboPorCancelar(string idInmobiliaria, string idConjunto, int idCfd)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = "SELECT * FROM T24_HISTORIA_RECIBOS LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = P2444_ID_HIST_REC WHERE P2401_ID_ARRENDADORA = ? AND CAMPO4 = ? AND T40_CFD.P4002_ID_CFD = ?";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = idConjunto;
                comando.Parameters.Add("@IDCfd", OdbcType.Int).Value = idCfd;
                ReciboEntity recibo = new ReciboEntity();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    recibo.IDHistRec = Convert.ToInt32(reader["P2444_ID_HIST_REC"]);
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.FechaCancelado = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    //recibo.FechaCancelado = Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = reader["P4007_FOLIO"] == DBNull.Value ? (int?)null : (int)reader["P4007_FOLIO"];
                    recibo.Inmueble = reader["CAMPO3"].ToString();
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.TipoCambio = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 1 : (decimal)reader["P2414_TIPO_CAMBIO"];
                    recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : (decimal)reader["P2405_IMPORTE"];
                    recibo.TotalIVA = reader["P2416_IVA"] == DBNull.Value ? 1 : (decimal)reader["P2416_IVA"];
                    recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : (decimal)reader["P2419_TOTAL"];
                    break;
                }
                reader.Close();
                conexion.Close();
                return recibo;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        #endregion Reporte de recibos cancelados

        #region Listado de contratos
        public static List<ContratoEntity> getListaContratosRenta(string idInmobiliaria, bool todos)
        {
            List<ContratoEntity> listaContratos = new List<ContratoEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT T04_CONTRATO.P0401_ID_CONTRATO,T04_CONTRATO.P0403_ID_ARRENDADORA, T04_CONTRATO.P0402_ID_ARRENDAT, T07_EDIFICIO.P0710_ID_CENTRO, T04_CONTRATO.P0404_ID_EDIFICIO, 
            T07_EDIFICIO.P0737_CAMPO12, T07_EDIFICIO.P0703_NOMBRE, T02_ARRENDATARIO.P0203_NOMBRE, T04_CONTRATO.P0420_TIEMPO, T04_CONTRATO.P0421_TIPO_TIEMPO, 
            T04_CONTRATO.P0405_TIPO_EDIF, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0414_PRORROGA, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, 
            T04_CONTRATO.P0432_FECHA_AUMENTO, T04_CONTRATO.P0419_MONEDA_IMPORTE, T04_CONTRATO.P0408_IMPORTE_FACT, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0418_IMPORTE_DEPOSITO, 
            T1.P0604_DIRECCION AS TELEFONO1, T2.P0604_DIRECCION AS TELEFONO2, T3.P0604_DIRECCION AS TELEFONO3, T4.P0604_DIRECCION AS TELEFONO4, 
            T5.P0604_DIRECCION AS TELEFONO5, F1.P0604_DIRECCION AS FAX1, F2.P0604_DIRECCION AS FAX2, E1.P0604_DIRECCION AS EMAIL1, E2.P0604_DIRECCION AS EMAIL2, 
            T04_CONTRATO.P0420_TIEMPO, T04_CONTRATO.P0421_TIPO_TIEMPO, P0425_ACTIVIDAD, P049F_TIPO_DE_PAGO, P0442_suspension_pagos, P0407_MONEDA_FACT, 
            P0428_A_PRORROGA,P1801_ID_SUBCONJUNTO,T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, T18_SUBCONJUNTOS.P1803_NOMBRE, T07_EDIFICIO.P0724_NO_EDIF 
            FROM T04_CONTRATO 
            LEFT JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID 
            LEFT JOIN T07_EDIFICIO ON T04_CONTRATO.P0404_ID_EDIFICIO = T07_EDIFICIO.P0701_ID_EDIFICIO 
            LEFT JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T07_EDIFICIO.P0730_SUBCONJUNTO  
            LEFT JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO =  T07_EDIFICIO.P0710_ID_CENTRO
            LEFT JOIN T06_MAIL_WEB T1 ON T02_ARRENDATARIO.P0201_ID = T1.P0601_ID_ENTE AND T1.P0603_TIPO_SERV = 'T' AND T1.P0605_ORDEN = 1  
            LEFT JOIN T06_MAIL_WEB T2 ON T02_ARRENDATARIO.P0201_ID = T2.P0601_ID_ENTE AND T2.P0603_TIPO_SERV = 'T' AND T2.P0605_ORDEN = 2  
            LEFT JOIN T06_MAIL_WEB T3 ON T02_ARRENDATARIO.P0201_ID = T3.P0601_ID_ENTE AND T3.P0603_TIPO_SERV = 'T' AND T3.P0605_ORDEN = 3  
            LEFT JOIN T06_MAIL_WEB T4 ON T02_ARRENDATARIO.P0201_ID = T4.P0601_ID_ENTE AND T4.P0603_TIPO_SERV = 'T' AND T4.P0605_ORDEN = 4  
            LEFT JOIN T06_MAIL_WEB T5 ON T02_ARRENDATARIO.P0201_ID = T5.P0601_ID_ENTE AND T5.P0603_TIPO_SERV = 'T' AND T5.P0605_ORDEN = 5  
            LEFT JOIN T06_MAIL_WEB F1 ON T02_ARRENDATARIO.P0201_ID = F1.P0601_ID_ENTE AND F1.P0603_TIPO_SERV = 'F' AND F1.P0605_ORDEN = 1  
            LEFT JOIN T06_MAIL_WEB F2 ON T02_ARRENDATARIO.P0201_ID = F2.P0601_ID_ENTE AND F2.P0603_TIPO_SERV = 'F' AND F2.P0605_ORDEN = 2  
            LEFT JOIN T06_MAIL_WEB E1 ON T02_ARRENDATARIO.P0201_ID = E1.P0601_ID_ENTE AND E1.P0603_TIPO_SERV = 'E' AND E1.P0605_ORDEN = 1  
            LEFT JOIN T06_MAIL_WEB E2 ON T02_ARRENDATARIO.P0201_ID = E2.P0601_ID_ENTE AND E2.P0603_TIPO_SERV = 'E' AND E2.P0605_ORDEN = 2   
WHERE T04_CONTRATO.P0403_ID_ARRENDADORA = ? AND (T04_CONTRATO.P0437_TIPO = 'R' OR T04_CONTRATO.P0437_TIPO = 'S')";

            if (!todos)
                sql += " AND T04_CONTRATO.P0428_A_PRORROGA = 'V'";

            sql += " order by T07_EDIFICIO.P0703_NOMBRE ASC, T18_SUBCONJUNTOS.P1803_NOMBRE ASC, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE ASC ";
            int contador = 0;
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.VarChar).Value = idInmobiliaria;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    ContratoEntity contrato = new ContratoEntity();
                    contrato.ID = reader["P0401_ID_CONTRATO"].ToString();


                    ClienteEntity cliente = getClienteByID(reader["P0402_ID_ARRENDAT"].ToString());
                    contrato.Cliente = cliente;
                    contrato.IDConjunto = reader["P0710_ID_CENTRO"].ToString();
                    ConjuntoEntity conjunto = getConjuntoByID(contrato.IDConjunto);
                    contrato.NombreConjunto = conjunto == null ? "" : conjunto.Nombre;
                    contrato.IdSubconjunto = reader["P1801_ID_SUBCONJUNTO"].ToString();
                    contrato.IDInmueble = reader["P0404_ID_EDIFICIO"].ToString();
                    contrato.NombreInmueble = reader["P0703_NOMBRE"].ToString();
                    contrato.NombreInmuebleSubconjunto = reader["P1803_NOMBRE"].ToString();
                    //if (contrato.IDInmueble.Contains("SCNJ"))
                    //    contrato.NombreInmueble = getNombreSubConjuntoByIDSub(contrato.IDInmueble);
                    if (contrato.IDInmueble.Contains("SCNJ"))
                    {
                        try
                        {
                            conjunto = getSubConjuntoByIDSub(contrato.IDInmueble);
                        }
                        catch
                        {
                        }

                        contrato.IDConjunto = conjunto.ID;
                        if (!string.IsNullOrEmpty(contrato.IDConjunto))
                            contrato.NombreInmueble = conjunto.Nombre;
                    }

                    if (contrato.IDInmueble.Contains("SCNJ"))
                        contrato.NombreConjunto = getNombreConjunto(contrato.IDConjunto);

                    if (string.IsNullOrEmpty(contrato.NombreConjunto))
                        //System.Diagnostics.Debugger.Break();

                    contrato.IdentificadorInmueble = getIdentificadorInmueble(contrato.IDInmueble);
                    contrato.Clasificacion = reader["P0737_CAMPO12"].ToString();
                    contrato.FechaInicio = Convert.ToDateTime(reader["P0410_INICIO_VIG"].ToString());
                    contrato.FechaVencimiento = Convert.ToDateTime(reader["P0411_FIN_VIG"].ToString());
                    int tiempoProrroga = reader["P0414_PRORROGA"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P0414_PRORROGA"].ToString());
                    contrato.FechaVencimientoProrroga = contrato.FechaVencimiento.AddYears(tiempoProrroga);
                    contrato.PorcentajeIncremento = reader["P0441_BASE_PARA_AUMENTO"].ToString();
                    if (reader["P0432_FECHA_AUMENTO"] != DBNull.Value)
                    {
                        contrato.FechaIncremento = DateTime.Parse(reader["P0432_FECHA_AUMENTO"].ToString());
                    }
                    contrato.Moneda = reader["P0407_MONEDA_FACT"].ToString();
                    contrato.ImporteOriginal = reader["P0408_IMPORTE_FACT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P0408_IMPORTE_FACT"].ToString());
                    contrato.ImporteActual = reader["P0434_IMPORTE_ACTUAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P0434_IMPORTE_ACTUAL"].ToString());
                    contrato.Deposito = reader["P0418_IMPORTE_DEPOSITO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P0418_IMPORTE_DEPOSITO"].ToString());
                    contrato.Telefono1 = reader["TELEFONO1"].ToString();
                    contrato.Telefono2 = reader["TELEFONO2"].ToString();
                    contrato.Telefono3 = reader["TELEFONO3"].ToString();
                    contrato.Telefono4 = reader["TELEFONO4"].ToString();
                    contrato.Telefono5 = reader["TELEFONO5"].ToString();
                    contrato.Fax1 = reader["FAX1"].ToString();
                    contrato.Fax2 = reader["FAX2"].ToString();
                    contrato.CorreoElectronico1 = reader["EMAIL1"].ToString();
                    contrato.CorreoElectronico2 = reader["EMAIL2"].ToString();
                    contrato.TiempoPeriodo = reader["P0420_TIEMPO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P0420_TIEMPO"].ToString());
                    contrato.TipoPeriodo = reader["P0421_TIPO_TIEMPO"].ToString();
                    contrato.Actividad = reader["P0425_ACTIVIDAD"].ToString();
                    contrato.EstatusContrato = getEstatusContratoRenta(contrato.ID);
                    contrato.TipoPago = reader["P049F_TIPO_DE_PAGO"].ToString();
                    string prorroga = reader["P0428_A_PRORROGA"].ToString();
                    contrato.EstatusProrroga = prorroga == "V" ? "VIGENTE" : "TERMINADO";
                    string suspensionPagos = reader["P0442_suspension_pagos"].ToString().ToLower();
                    if (!string.IsNullOrEmpty(suspensionPagos))
                        contrato.SuspensionPagos = suspensionPagos.Equals("si") ? true : false;
                    else
                        contrato.SuspensionPagos = false;
                    contrato.ListaCargos = getCargosDeContrato(contrato.ID);
                    listaContratos.Add(contrato);

                }
                reader.Close();
                conexion.Close();
                listaContratos = OrdenarLista(listaContratos);
                return listaContratos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        public static List<ContratoEntity> OrdenarLista(List<ContratoEntity> lista)
        {
            var lista2 = lista.OrderBy(x => x.NombreConjunto).ThenBy(i => i.NombreInmuebleSubconjunto).ThenBy(i => i.NombreInmueble).ToList();
            return lista2;
            //var list = lista.OrderBy(i => i.NombreInmueble).ThenBy(i => i.NombreConjunto);
            //return list.ToList();
        }
        public static List<ContratoEntity> getListaContratosRenta(string idInmobiliaria, string idConjunto, bool todos)
        {
            List<ContratoEntity> listaContratos = new List<ContratoEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.P0403_ID_ARRENDADORA, T04_CONTRATO.P0402_ID_ARRENDAT, T07_EDIFICIO.P0710_ID_CENTRO, T04_CONTRATO.P0404_ID_EDIFICIO, 
            T07_EDIFICIO.P0737_CAMPO12, T07_EDIFICIO.P0703_NOMBRE, T02_ARRENDATARIO.P0203_NOMBRE, T04_CONTRATO.P0420_TIEMPO, T04_CONTRATO.P0421_TIPO_TIEMPO, 
            T04_CONTRATO.P0405_TIPO_EDIF, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0414_PRORROGA, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, 
            T04_CONTRATO.P0432_FECHA_AUMENTO, T04_CONTRATO.P0419_MONEDA_IMPORTE, T04_CONTRATO.P0408_IMPORTE_FACT, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0418_IMPORTE_DEPOSITO, 
            T1.P0604_DIRECCION AS TELEFONO1, T2.P0604_DIRECCION AS TELEFONO2, T3.P0604_DIRECCION AS TELEFONO3, T4.P0604_DIRECCION AS TELEFONO4,
            T5.P0604_DIRECCION AS TELEFONO5, F1.P0604_DIRECCION AS FAX1, F2.P0604_DIRECCION AS FAX2, E1.P0604_DIRECCION AS EMAIL1, E2.P0604_DIRECCION AS EMAIL2,
            T04_CONTRATO.P0420_TIEMPO, T04_CONTRATO.P0421_TIPO_TIEMPO, P0425_ACTIVIDAD, P049F_TIPO_DE_PAGO, P0442_suspension_pagos, P0407_MONEDA_FACT , P0407_MONEDA_FACT, P0428_A_PRORROGA,
            P1801_ID_SUBCONJUNTO,T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, T18_SUBCONJUNTOS.P1803_NOMBRE, T07_EDIFICIO.P0724_NO_EDIF
            FROM T04_CONTRATO
            LEFT JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID
            LEFT JOIN T07_EDIFICIO ON T04_CONTRATO.P0404_ID_EDIFICIO = T07_EDIFICIO.P0701_ID_EDIFICIO
            LEFT JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T07_EDIFICIO.P0730_SUBCONJUNTO 
            LEFT JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO =  T07_EDIFICIO.P0710_ID_CENTRO
            LEFT JOIN T06_MAIL_WEB T1 ON T02_ARRENDATARIO.P0201_ID = T1.P0601_ID_ENTE AND T1.P0603_TIPO_SERV = 'T' AND T1.P0605_ORDEN = 1
            LEFT JOIN T06_MAIL_WEB T2 ON T02_ARRENDATARIO.P0201_ID = T2.P0601_ID_ENTE AND T2.P0603_TIPO_SERV = 'T' AND T2.P0605_ORDEN = 2
            LEFT JOIN T06_MAIL_WEB T3 ON T02_ARRENDATARIO.P0201_ID = T3.P0601_ID_ENTE AND T3.P0603_TIPO_SERV = 'T' AND T3.P0605_ORDEN = 3
            LEFT JOIN T06_MAIL_WEB T4 ON T02_ARRENDATARIO.P0201_ID = T4.P0601_ID_ENTE AND T4.P0603_TIPO_SERV = 'T' AND T4.P0605_ORDEN = 4
            LEFT JOIN T06_MAIL_WEB T5 ON T02_ARRENDATARIO.P0201_ID = T5.P0601_ID_ENTE AND T5.P0603_TIPO_SERV = 'T' AND T5.P0605_ORDEN = 5
            LEFT JOIN T06_MAIL_WEB F1 ON T02_ARRENDATARIO.P0201_ID = F1.P0601_ID_ENTE AND F1.P0603_TIPO_SERV = 'F' AND F1.P0605_ORDEN = 1
            LEFT JOIN T06_MAIL_WEB F2 ON T02_ARRENDATARIO.P0201_ID = F2.P0601_ID_ENTE AND F2.P0603_TIPO_SERV = 'F' AND F2.P0605_ORDEN = 2
            LEFT JOIN T06_MAIL_WEB E1 ON T02_ARRENDATARIO.P0201_ID = E1.P0601_ID_ENTE AND E1.P0603_TIPO_SERV = 'E' AND E1.P0605_ORDEN = 1
            LEFT JOIN T06_MAIL_WEB E2 ON T02_ARRENDATARIO.P0201_ID = E2.P0601_ID_ENTE AND E2.P0603_TIPO_SERV = 'E' AND E2.P0605_ORDEN = 2
            WHERE T04_CONTRATO.P0403_ID_ARRENDADORA = ? AND T07_EDIFICIO.P0710_ID_CENTRO = ? AND (T04_CONTRATO.P0437_TIPO = 'R' OR T04_CONTRATO.P0437_TIPO = 'S')";

            if (!todos)
                sql += "AND T04_CONTRATO.P0428_A_PRORROGA = 'V'";

            sql += " order by T07_EDIFICIO.P0703_NOMBRE ASC, T18_SUBCONJUNTOS.P1803_NOMBRE ASC, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE ASC ";

            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = idConjunto;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ContratoEntity contrato = new ContratoEntity();
                    contrato.ID = reader["P0401_ID_CONTRATO"].ToString();
                    ClienteEntity cliente = getClienteByID(reader["P0402_ID_ARRENDAT"].ToString());
                    contrato.Cliente = cliente;
                    contrato.IDConjunto = reader["P0710_ID_CENTRO"].ToString();
                    ConjuntoEntity conjunto = getConjuntoByID(contrato.IDConjunto);
                    contrato.NombreConjunto = conjunto == null ? "" : conjunto.Nombre;
                    contrato.IdSubconjunto = reader["P1801_ID_SUBCONJUNTO"].ToString();
                    contrato.IDInmueble = reader["P0404_ID_EDIFICIO"].ToString();
                    contrato.NombreInmueble = reader["P0703_NOMBRE"].ToString();
                    contrato.NombreInmuebleSubconjunto = reader["P1803_NOMBRE"].ToString();
                    
                    if (string.IsNullOrEmpty(contrato.NombreInmueble))
                        contrato.NombreInmueble = getNombreSubConjuntoByIDSub(contrato.ID);

                    if (contrato.IDInmueble.Contains("SCNJ"))
                    {
                        //System.Diagnostics.Debugger.Break();
                        try
                        {
                            conjunto = getSubConjuntoByIDSub(contrato.IDInmueble);
                        }
                        catch
                        {
                        }

                        contrato.IDConjunto = conjunto.ID;
                        if (!string.IsNullOrEmpty(contrato.IDConjunto))
                            contrato.NombreInmueble = conjunto.Nombre;
                    }
                    if (contrato.IDInmueble.Contains("SCNJ"))
                        contrato.NombreConjunto = getNombreSubConjunto(contrato.IDConjunto);

                    contrato.IdentificadorInmueble = getIdentificadorInmueble(contrato.IDInmueble);
                    contrato.Clasificacion = reader["P0737_CAMPO12"].ToString();
                    contrato.FechaInicio = Convert.ToDateTime(reader["P0410_INICIO_VIG"].ToString());
                    contrato.FechaVencimiento = Convert.ToDateTime(reader["P0411_FIN_VIG"].ToString());
                    int tiempoProrroga = reader["P0414_PRORROGA"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P0414_PRORROGA"].ToString());
                    contrato.FechaVencimientoProrroga = contrato.FechaVencimiento.AddYears(tiempoProrroga);
                    contrato.PorcentajeIncremento = reader["P0441_BASE_PARA_AUMENTO"].ToString();
                    if (reader["P0432_FECHA_AUMENTO"] != DBNull.Value)
                    {
                        contrato.FechaIncremento = DateTime.Parse(reader["P0432_FECHA_AUMENTO"].ToString());
                    }
                    contrato.Moneda = reader["P0407_MONEDA_FACT"].ToString();
                    contrato.ImporteOriginal = Convert.ToDecimal(reader["P0408_IMPORTE_FACT"].ToString());
                    contrato.ImporteActual = Convert.ToDecimal(reader["P0434_IMPORTE_ACTUAL"].ToString());
                    contrato.Deposito = reader["P0418_IMPORTE_DEPOSITO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P0418_IMPORTE_DEPOSITO"].ToString());
                    contrato.Telefono1 = reader["TELEFONO1"].ToString();
                    contrato.Telefono2 = reader["TELEFONO2"].ToString();
                    contrato.Telefono3 = reader["TELEFONO3"].ToString();
                    contrato.Telefono4 = reader["TELEFONO4"].ToString();
                    contrato.Telefono5 = reader["TELEFONO5"].ToString();
                    contrato.Fax1 = reader["FAX1"].ToString();
                    contrato.Fax2 = reader["FAX2"].ToString();
                    contrato.CorreoElectronico1 = reader["EMAIL1"].ToString();
                    contrato.CorreoElectronico2 = reader["EMAIL2"].ToString();
                    contrato.TiempoPeriodo = reader["P0420_TIEMPO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P0420_TIEMPO"].ToString());
                    contrato.TipoPeriodo = reader["P0421_TIPO_TIEMPO"].ToString();
                    contrato.Actividad = reader["P0425_ACTIVIDAD"].ToString();
                    contrato.EstatusContrato = getEstatusContratoRenta(contrato.ID);
                    contrato.TipoPago = reader["P049F_TIPO_DE_PAGO"].ToString();
                    string prorroga = reader["P0428_A_PRORROGA"].ToString();
                    contrato.EstatusProrroga = prorroga == "V" ? "VIGENTE" : "TERMINADO";
                    string suspensionPagos = reader["P0442_suspension_pagos"].ToString().ToLower();
                    if (!string.IsNullOrEmpty(suspensionPagos))
                        contrato.SuspensionPagos = suspensionPagos.Equals("si") ? true : false;
                    else
                        contrato.SuspensionPagos = false;
                    contrato.ListaCargos = getCargosDeContrato(contrato.ID);
                    listaContratos.Add(contrato);
                }
                reader.Close();
                conexion.Close();

                listaContratos = OrdenarLista(listaContratos);

                return listaContratos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }
        public static List<ContratoEntity> getDatosMailWeb()
        {
            List<ContratoEntity> lista = new List<ContratoEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = "SELECT P0601_ID_ENTE, P0603_TIPO_SERV, P0604_DIRECCION, P0605_ORDEN FROM T06_MAIL_WEB WHERE P0602_TIPO_ENTE='2'AND P0603_TIPO_SERV<>'W'";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ContratoEntity contrato = new ContratoEntity();
                    contrato.ID = reader["P0601_ID_ENTE"].ToString();
                    contrato.Tipo = reader["P0603_TIPO_SERV"].ToString();
                    contrato.Orden = Convert.ToInt32(reader["P0605_ORDEN"].ToString());
                    contrato.Direccion = reader["P0604_DIRECCION"].ToString();
                    lista.Add(contrato);
                }

                reader.Close();
                conexion.Close();
                return lista;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ContratoEntity> getListaHistorialContratos(string idInmobiliaria, bool todos, DateTime fechaFin)
        {
            List<ContratoEntity> listaContratos = new List<ContratoEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            #region QuerySql
            string sql = @"SELECT    
DISTINCT   
P5901_ID_CONTRATO,
MAX(P5976_FECHA_HORA_UPDT),
P5902_ID_ARRENDAT_ANT,
P0201_ID, 
P0203_NOMBRE, 
P0253_NOMBRE_COMERCIAL, 
P0204_RFC,
P5906_ID_EDIFICIO_ANT,
T07_EDIFICIO.P0710_ID_CENTRO,
T07_EDIFICIO.P0703_NOMBRE, 
T07_EDIFICIO.P0737_CAMPO12, 
P5911_INICIO_VIG_NVO,
P5913_FIN_VIG_NVO,
P5915_PRORROGA_NVO,
P5909_MONEDA_FACT_NVO, 
P5941_IMP_ACTUAL_NVO, 
P5921_IMPORTE_DEPOSITO_NVO, 
P5927_ACTIVIDAD_NVO,
P5963_BASE_PARA_AUM_NVO, 
P5933_A_PRORROGA_NVO,  
P5937_FECHA_AUMENTO_NVO,
P5923_TIEMPO_NVO, 
P5925_TIPO_TIEMPO_NVO,
P5965_SUSP_PAGOS_NVO 
FROM   
T59_HISTORIA_CONTRATOS  
LEFT JOIN T01_ARRENDADORA ON T59_HISTORIA_CONTRATOS.P5904_ID_ARRENDADORA_ANT = T01_ARRENDADORA.P0101_ID_ARR   
LEFT JOIN T02_ARRENDATARIO ON T59_HISTORIA_CONTRATOS.P5902_ID_ARRENDAT_ANT  = T02_ARRENDATARIO.P0201_ID   
LEFT JOIN T07_EDIFICIO ON T59_HISTORIA_CONTRATOS.P5906_ID_EDIFICIO_ANT =T07_EDIFICIO.P0701_ID_EDIFICIO 
LEFT JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T07_EDIFICIO.P0710_ID_CENTRO 
WHERE P5904_ID_ARRENDADORA_ANT= ? 
AND P5911_INICIO_VIG_NVO <= ?   AND P5913_FIN_VIG_NVO<= ?    
and T59_HISTORIA_CONTRATOS.P5933_A_PRORROGA_NVO='V' 
GROUP BY   
P5901_ID_CONTRATO,
P5902_ID_ARRENDAT_ANT,
P0201_ID, 
P0203_NOMBRE, 
P0253_NOMBRE_COMERCIAL, 
P0204_RFC,
P5906_ID_EDIFICIO_ANT,
T07_EDIFICIO.P0710_ID_CENTRO,
T07_EDIFICIO.P0703_NOMBRE, 
T07_EDIFICIO.P0737_CAMPO12, 
P5911_INICIO_VIG_NVO,
P5913_FIN_VIG_NVO,
P5915_PRORROGA_NVO,
P5909_MONEDA_FACT_NVO, 
P5941_IMP_ACTUAL_NVO, 
P5921_IMPORTE_DEPOSITO_NVO, 
P5927_ACTIVIDAD_NVO,
P5963_BASE_PARA_AUM_NVO, 
P5933_A_PRORROGA_NVO,  
P5937_FECHA_AUMENTO_NVO, 
P5923_TIEMPO_NVO, 
P5925_TIPO_TIEMPO_NVO,
P5965_SUSP_PAGOS_NVO
ORDER BY MAX(P5976_FECHA_HORA_UPDT) DESC";
            #endregion
        
            try
            { 
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@fechaInicio", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@fechaFin", OdbcType.DateTime).Value = fechaFin;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                int x = 0;
                while (reader.Read())
                {
                    ContratoEntity contrato = new ContratoEntity();
                    contrato.ID = reader["P5901_ID_CONTRATO"].ToString();

                    contrato.Cliente = new ClienteEntity()
                    {
                        IDCliente = reader["P0201_ID"].ToString(),
                        Nombre = reader["P0203_NOMBRE"].ToString(),
                        NombreComercial = reader["P0253_NOMBRE_COMERCIAL"].ToString(),
                        RFC = reader["P0204_RFC"].ToString()

                    };
                    contrato.IDConjunto = reader["P0710_ID_CENTRO"].ToString();
                    ConjuntoEntity conjunto = getConjuntoByID(contrato.IDConjunto);
                    contrato.NombreConjunto = conjunto == null ? "" : conjunto.Nombre;
                    contrato.NombreInmueble = reader["P0703_NOMBRE"].ToString();
                    contrato.IDInmueble = reader["P5906_ID_EDIFICIO_ANT"].ToString();
                    if (string.IsNullOrEmpty(contrato.NombreInmueble))
                        contrato.NombreInmueble = getNombreSubConjuntoByIDSub(contrato.ID);
                    contrato.IdentificadorInmueble = getIdentificadorInmueble(contrato.IDInmueble);
                    contrato.Clasificacion = reader["P0737_CAMPO12"].ToString();
                    contrato.FechaInicio = Convert.ToDateTime(reader["P5911_INICIO_VIG_NVO"].ToString());
                    contrato.FechaVencimiento = Convert.ToDateTime(reader["P5913_FIN_VIG_NVO"].ToString());
                    contrato.Moneda = reader["P5909_MONEDA_FACT_NVO"].ToString();
                    contrato.ImporteActual = reader["P5941_IMP_ACTUAL_NVO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P5941_IMP_ACTUAL_NVO"].ToString());
                    contrato.Deposito = reader["P5921_IMPORTE_DEPOSITO_NVO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P5921_IMPORTE_DEPOSITO_NVO"].ToString());
                    
                    contrato.Actividad = reader["P5927_ACTIVIDAD_NVO"].ToString(); 
                    contrato.PorcentajeIncremento = reader["P5963_BASE_PARA_AUM_NVO"] == DBNull.Value ? string.Empty : reader["P5963_BASE_PARA_AUM_NVO"].ToString(); 
                    string prorroga = reader["P5933_A_PRORROGA_NVO"].ToString();
                    contrato.EstatusProrroga = prorroga == "V" ? "VIGENTE" : "TERMINADO";
                    int tiempoProrroga = reader["P5915_PRORROGA_NVO"] == DBNull.Value ? 0: Convert.ToInt32(reader["P5915_PRORROGA_NVO"].ToString());

                    contrato.FechaVencimientoProrroga = contrato.FechaVencimiento.AddYears(tiempoProrroga);

                    if (reader["P5937_FECHA_AUMENTO_NVO"] != DBNull.Value) 
                    {
                        contrato.FechaIncremento = DateTime.Parse(reader["P5937_FECHA_AUMENTO_NVO"].ToString());
                    }
                    contrato.TiempoPeriodo = Convert.ToInt32(reader["P5923_TIEMPO_NVO"].ToString()); 
                    contrato.TipoPeriodo = reader["P5925_TIPO_TIEMPO_NVO"].ToString();
                    
                    string suspensionPagos = reader["P5965_SUSP_PAGOS_NVO"].ToString().ToLower(); 
                    if (!string.IsNullOrEmpty(suspensionPagos))
                        contrato.SuspensionPagos = suspensionPagos.Equals("si") ? true : false;
                    else
                        contrato.SuspensionPagos = false;
                    
                    ContratoEntity contratoBuscar = listaContratos.Where(c => c.ID == contrato.ID).FirstOrDefault();
                    if (contratoBuscar == null)
                    {
                        listaContratos.Add(contrato);
                    }
                }
                reader.Close();
                conexion.Close();

                List<ContratoEntity> ListaMailWeb = getDatosMailWeb();
                
                foreach (ContratoEntity contrato in listaContratos)
                {
                    List<ContratoEntity> listaDetalle = ListaMailWeb.Where(c => c.ID == contrato.Cliente.IDCliente).ToList();

                    //List<ContratoEntity> listaDetalle = ListaMailWeb;

                    foreach (ContratoEntity detalle in listaDetalle)
                    {
                        if (detalle.Tipo == "T")
                        {
                            if (detalle.Orden == 1)
                            {
                                contrato.Telefono1 = detalle.Direccion;
                            }
                            else if (detalle.Orden == 2)
                            {
                                contrato.Telefono2 = detalle.Direccion;
                            }
                            else if (detalle.Orden == 3)
                            {
                                contrato.Telefono3 = detalle.Direccion;
                            }
                            else if (detalle.Orden == 4)
                            {
                                contrato.Telefono4 = detalle.Direccion;
                            }
                            else
                            {
                                contrato.Telefono5 = detalle.Direccion;
                            }
                        }
                        else if (detalle.Tipo == "F")
                        {
                            if (detalle.Orden == 1)
                            {
                                contrato.Fax1 = detalle.Direccion;
                            }
                            else
                            {
                                contrato.Fax2 = detalle.Direccion;
                            }
                        }
                        else if (detalle.Tipo == "E")
                        {
                            if (detalle.Orden == 1)
                            {
                                contrato.CorreoElectronico1 = detalle.Direccion;
                            }
                            else
                            {
                                contrato.CorreoElectronico2 = detalle.Direccion;
                            }
                        }
                    }
                  
                }

                return listaContratos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ContratoEntity> getListaHistorialContratos(string idInmobiliaria, string idConjunto, bool todos, DateTime fechaFin)
        {
            List<ContratoEntity> listaContratos = new List<ContratoEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            #region QuerySql
            string sql = @"SELECT    
DISTINCT   
P5901_ID_CONTRATO,
MAX(P5976_FECHA_HORA_UPDT),
P5902_ID_ARRENDAT_ANT,
P0201_ID, 
P0203_NOMBRE, 
P0253_NOMBRE_COMERCIAL, 
P0204_RFC,
P5906_ID_EDIFICIO_ANT,
T07_EDIFICIO.P0710_ID_CENTRO,
T07_EDIFICIO.P0703_NOMBRE, 
T07_EDIFICIO.P0737_CAMPO12, 
P5911_INICIO_VIG_NVO,
P5913_FIN_VIG_NVO,
P5915_PRORROGA_NVO,
P5909_MONEDA_FACT_NVO, 
P5941_IMP_ACTUAL_NVO, 
P5921_IMPORTE_DEPOSITO_NVO, 
P5927_ACTIVIDAD_NVO,
P5963_BASE_PARA_AUM_NVO, 
P5933_A_PRORROGA_NVO,  
P5937_FECHA_AUMENTO_NVO,
P5923_TIEMPO_NVO, 
P5925_TIPO_TIEMPO_NVO,
P5965_SUSP_PAGOS_NVO 
FROM   
T59_HISTORIA_CONTRATOS  
LEFT JOIN T01_ARRENDADORA ON T59_HISTORIA_CONTRATOS.P5904_ID_ARRENDADORA_ANT = T01_ARRENDADORA.P0101_ID_ARR   
LEFT JOIN T02_ARRENDATARIO ON T59_HISTORIA_CONTRATOS.P5902_ID_ARRENDAT_ANT  = T02_ARRENDATARIO.P0201_ID   
LEFT JOIN T07_EDIFICIO ON T59_HISTORIA_CONTRATOS.P5906_ID_EDIFICIO_ANT =T07_EDIFICIO.P0701_ID_EDIFICIO 
LEFT JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO= T07_EDIFICIO.P0710_ID_CENTRO 
WHERE P5904_ID_ARRENDADORA_ANT= ?  
AND T07_EDIFICIO.P0710_ID_CENTRO = ?  
AND P5911_INICIO_VIG_NVO <= ?   AND P5913_FIN_VIG_NVO<= ?    
and T59_HISTORIA_CONTRATOS.P5933_A_PRORROGA_NVO='V' 
GROUP BY   
P5901_ID_CONTRATO,
P5902_ID_ARRENDAT_ANT,
P0201_ID, 
P0203_NOMBRE, 
P0253_NOMBRE_COMERCIAL, 
P0204_RFC,
P5906_ID_EDIFICIO_ANT,
T07_EDIFICIO.P0710_ID_CENTRO,
T07_EDIFICIO.P0703_NOMBRE, 
T07_EDIFICIO.P0737_CAMPO12, 
P5911_INICIO_VIG_NVO,
P5913_FIN_VIG_NVO,
P5915_PRORROGA_NVO,
P5909_MONEDA_FACT_NVO, 
P5941_IMP_ACTUAL_NVO, 
P5921_IMPORTE_DEPOSITO_NVO, 
P5927_ACTIVIDAD_NVO,
P5963_BASE_PARA_AUM_NVO, 
P5933_A_PRORROGA_NVO,  
P5937_FECHA_AUMENTO_NVO, 
P5923_TIEMPO_NVO, 
P5925_TIPO_TIEMPO_NVO,
P5965_SUSP_PAGOS_NVO
ORDER BY MAX(P5976_FECHA_HORA_UPDT) DESC";
            #endregion

            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.VarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = idConjunto;
                comando.Parameters.Add("@fechaInicio", OdbcType.DateTime).Value = fechaFin;
                comando.Parameters.Add("@fechaFin", OdbcType.DateTime).Value = fechaFin;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                int x = 0;
                while (reader.Read())
                {
                    ContratoEntity contrato = new ContratoEntity();
                    contrato.ID = reader["P5901_ID_CONTRATO"].ToString();

                    contrato.Cliente = new ClienteEntity()
                    {
                        IDCliente = reader["P0201_ID"].ToString(),
                        Nombre = reader["P0203_NOMBRE"].ToString(),
                        NombreComercial = reader["P0253_NOMBRE_COMERCIAL"].ToString(),
                        RFC = reader["P0204_RFC"].ToString()
                    };
                    contrato.IDConjunto = reader["P0710_ID_CENTRO"].ToString();
                    ConjuntoEntity conjunto = getConjuntoByID(contrato.IDConjunto);
                    contrato.NombreConjunto = conjunto == null ? "" : conjunto.Nombre;
                    contrato.NombreInmueble = reader["P0703_NOMBRE"].ToString();
                    if (string.IsNullOrEmpty(contrato.NombreInmueble))
                        contrato.NombreInmueble = getNombreSubConjuntoByIDSub(contrato.ID);
                    contrato.IdentificadorInmueble = getIdentificadorInmueble(contrato.IDInmueble);
                    contrato.Clasificacion = reader["P0737_CAMPO12"].ToString();
                    contrato.FechaInicio = Convert.ToDateTime(reader["P5911_INICIO_VIG_NVO"].ToString());
                    contrato.FechaVencimiento = Convert.ToDateTime(reader["P5913_FIN_VIG_NVO"].ToString());
                    contrato.Moneda = reader["P5909_MONEDA_FACT_NVO"].ToString();
                    contrato.ImporteActual = reader["P5941_IMP_ACTUAL_NVO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P5941_IMP_ACTUAL_NVO"].ToString());
                    contrato.Deposito = reader["P5921_IMPORTE_DEPOSITO_NVO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P5921_IMPORTE_DEPOSITO_NVO"].ToString());

                    contrato.Actividad = reader["P5927_ACTIVIDAD_NVO"].ToString();
                    contrato.PorcentajeIncremento = reader["P5963_BASE_PARA_AUM_NVO"] == DBNull.Value ? string.Empty : reader["P5963_BASE_PARA_AUM_NVO"].ToString();
                    string prorroga = reader["P5933_A_PRORROGA_NVO"].ToString();
                    contrato.EstatusProrroga = prorroga == "V" ? "VIGENTE" : "TERMINADO";
                    int tiempoProrroga = reader["P5915_PRORROGA_NVO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P5915_PRORROGA_NVO"].ToString());

                    contrato.FechaVencimientoProrroga = contrato.FechaVencimiento.AddYears(tiempoProrroga);

                    if (reader["P5937_FECHA_AUMENTO_NVO"] != DBNull.Value)
                    {
                        contrato.FechaIncremento = DateTime.Parse(reader["P5937_FECHA_AUMENTO_NVO"].ToString());
                    }
                    contrato.TiempoPeriodo = Convert.ToInt32(reader["P5923_TIEMPO_NVO"].ToString());
                    contrato.TipoPeriodo = reader["P5925_TIPO_TIEMPO_NVO"].ToString();

                    string suspensionPagos = reader["P5965_SUSP_PAGOS_NVO"].ToString().ToLower();
                    if (!string.IsNullOrEmpty(suspensionPagos))
                        contrato.SuspensionPagos = suspensionPagos.Equals("si") ? true : false;
                    else
                        contrato.SuspensionPagos = false;

                    ContratoEntity contratoBuscar = listaContratos.Where(c => c.ID == contrato.ID).FirstOrDefault();
                    if (contratoBuscar == null)
                    {
                        listaContratos.Add(contrato);
                    }

                }
                reader.Close();
                conexion.Close();

                List<ContratoEntity> ListaMailWeb = getDatosMailWeb();

                foreach (ContratoEntity contrato in listaContratos)
                {
                    List<ContratoEntity> listaDetalle = ListaMailWeb.Where(c => c.ID == contrato.Cliente.IDCliente).ToList();

                    //List<ContratoEntity> listaDetalle = ListaMailWeb;

                    foreach (ContratoEntity detalle in listaDetalle)
                    {
                        if (detalle.Tipo == "T")
                        {
                            if (detalle.Orden == 1)
                            {
                                contrato.Telefono1 = detalle.Direccion;
                            }
                            else if (detalle.Orden == 2)
                            {
                                contrato.Telefono2 = detalle.Direccion;
                            }
                            else if (detalle.Orden == 3)
                            {
                                contrato.Telefono3 = detalle.Direccion;
                            }
                            else if (detalle.Orden == 4)
                            {
                                contrato.Telefono4 = detalle.Direccion;
                            }
                            else
                            {
                                contrato.Telefono5 = detalle.Direccion;
                            }
                        }
                        else if (detalle.Tipo == "F")
                        {
                            if (detalle.Orden == 1)
                            {
                                contrato.Fax1 = detalle.Direccion;
                            }
                            else
                            {
                                contrato.Fax2 = detalle.Direccion;
                            }
                        }
                        else if (detalle.Tipo == "E")
                        {
                            if (detalle.Orden == 1)
                            {
                                contrato.CorreoElectronico1 = detalle.Direccion;
                            }
                            else
                            {
                                contrato.CorreoElectronico2 = detalle.Direccion;
                            }
                        }
                    }

                }

                return listaContratos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        
        public static List<SubtipoEntity> getCargosDeContrato(string idContrato)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                OdbcCommand comando = conexion.CreateCommand();
                comando.CommandText = @"SELECT P0401_ID_CONTRATO,P3403_IMPORTE, T34_OTRO_CARGOS.CAMPO2 AS Identificador FROM T04_CONTRATO
                                        JOIN T34_OTRO_CARGOS ON   P0401_ID_CONTRATO = P3401_ID_CARGO
                                        WHERE  P0401_ID_CONTRATO = '" + idContrato + "'";
                List<SubtipoEntity> subtipos = new List<SubtipoEntity>();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                //int orden = 1;
                while (reader.Read())
                {
                    SubtipoEntity sub = new SubtipoEntity()
                    {
                        IdContrato = reader["P0401_ID_CONTRATO"].ToString().Trim(),
                        Identificador = reader["Identificador"].ToString().Trim(),
                        importeCargo = Convert.ToDecimal(reader["P3403_IMPORTE"].ToString().Trim())
                        //Nombre = reader["DESCR_CAT"].ToString().Trim(),
                        // Orden = orden
                    };
                    //orden++;
                    subtipos.Add(sub);
                }
                /*SubtipoEntity s = new SubtipoEntity()
                {
                    Nombre = "Otros",
                    Orden = orden
                };
                subtipos.Add(s);*/
                reader.Close();
                conexion.Close();
                return subtipos;
            }
            catch
            {
                conexion.Close();
                return null;
            }


        }
        private static string getIdentificadorInmueble(string idInmueble)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT  P0726_CLAVE FROM T07_EDIFICIO WHERE P0701_ID_EDIFICIO = ?";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmueble", OdbcType.VarChar).Value = idInmueble;
                conexion.Open();
                string result = comando.ExecuteScalar().ToString();
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }
        public static ConjuntoEntity getSubConjuntoByIDSub(string idSubconjunto)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                string sql = @"SELECT P1803_NOMBRE, P1804_ID_CONJUNTO FROM T18_SUBCONJUNTOS WHERE T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = ? order by P1817_FECHA_HORA_ULT_MOV ASC ";
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@idSubconjunto", OdbcType.VarChar).Value = idSubconjunto;
                ConjuntoEntity conj = new ConjuntoEntity();
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    conj.ID = reader["P1804_ID_CONJUNTO"] == DBNull.Value ? string.Empty : reader["P1804_ID_CONJUNTO"].ToString();
                    conj.Nombre = reader["P1804_ID_CONJUNTO"] == DBNull.Value ? string.Empty : reader["P1803_NOMBRE"].ToString();

                }
                reader.Close();
                conexion.Close();
                return conj;

            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }
        private static string getEstatusContratoRenta(string idContrato)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT CAMPO7 FROM T31_CONFIGURACION WHERE P3102_VALOR = ?";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDContrato", OdbcType.VarChar).Value = idContrato;
                conexion.Open();
                string result = comando.ExecuteScalar().ToString();
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }

        private static string getIdInmueblePorContrato(string idContrato)
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT T04_CONTRATO.P0404_ID_EDIFICIO FROM T04_CONTRATO  where T04_CONTRATO.P0401_ID_CONTRATO = ?";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDContrato", OdbcType.VarChar).Value = idContrato;
                conexion.Open();
                string result = comando.ExecuteScalar().ToString();
                conexion.Close();
                return result;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }

        #endregion Listado de contratos

        #region Recibos Expedidos por Folio
        public static List<ReciboEntity> UltimoAbono(string idInmobiliaria, string idContrato)
        {
            List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"select first 1 *  from T24_HISTORIA_RECIBOS where T24_HISTORIA_RECIBOS.P2418_ID_CONTRATO = ? and P2401_ID_ARRENDADORA = ?
AND T24_HISTORIA_RECIBOS.P2406_STATUS = '2' ORDER BY T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO DESC";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDcONTRATO", OdbcType.NVarChar).Value = idContrato;
                comando.Parameters.Add("@IdArrendadora", OdbcType.NVarChar).Value = idInmobiliaria;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    recibo.IDHistRec = (int)reader["P2444_ID_HIST_REC"];
                    recibo.FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]);

                    decimal PagoEnCobranzaPago = SaariE.getTotalReciboPagado(recibo.IDHistRec);


                    if (PagoEnCobranzaPago > recibo.Pago)
                    {
                        recibo.Pago = PagoEnCobranzaPago;
                    }
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getRecibosExpedidosPorFolioReporteGlobal(string idInmobiliaria, string idContrato)
        {
            List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT T40_CFD.P4001_ID_HIST_REC, T40_CFD.P4002_ID_CFD, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION,T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO,T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.CAMPO3, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T24_HISTORIA_RECIBOS.P2423_RFC, 
                        T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2416_IVA, T24_HISTORIA_RECIBOS.CAMPO_NUM1,
                        T24_HISTORIA_RECIBOS.CAMPO_NUM2, T24_HISTORIA_RECIBOS.P2406_STATUS,T24_HISTORIA_RECIBOS.P2419_TOTAL,T24_HISTORIA_RECIBOS.P2413_PAGO, T24_HISTORIA_RECIBOS.P2428_T_IVA, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2406_STATUS  
                        FROM T24_HISTORIA_RECIBOS 
                        INNER JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                        WHERE P2401_ID_ARRENDADORA = ? AND P2418_ID_CONTRATO = ? AND T40_CFD.P4006_SERIE IS NOT NULL AND T40_CFD.P4007_FOLIO IS NOT NULL 
                        AND T24_HISTORIA_RECIBOS.P2406_STATUS ='2' AND (T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'R' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'Z' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC='W' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC='X') 
                        ORDER BY P2409_FECHA_EMISION";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.NVarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@IdArrendadora", OdbcType.NVarChar).Value = idContrato;
                //comando.Parameters.Add("@FechaInicio", OdbcType.DateTime).Value = fechaInicio;
                //comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    //FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]),
                    //   FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]),

                    ReciboEntity recibo = new ReciboEntity();
                    string estadoDoc = reader["P2406_STATUS"].ToString();
                    recibo.IDHistRec = (int)reader["P4001_ID_HIST_REC"];
                    recibo.IDCFD = (int)reader["P4002_ID_CFD"];
                    recibo.Estatus = reader["P2406_STATUS"].ToString();
                    recibo.FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1901, 1, 1) : Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);//P2408_FECHA_PAGADO
                    recibo.FechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["P2408_FECHA_PAGADO"]);
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = (int)reader["P4007_FOLIO"];
                    //recibo.UUID = SaariE.getUUIDByIDCfd(recibo.IDCFD);
                    recibo.Inmueble = reader["CAMPO3"].ToString();
                    //EdificioEntity inmueble = getInmuebleByID(recibo.Inmueble);
                    //recibo.NombreInmueble = inmueble == null ? "" : inmueble.Nombre;
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    ClienteEntity cliente = getClienteByID(recibo.IDCliente);
                    recibo.NombreCliente = cliente == null ? "" : cliente.Nombre;
                    recibo.RFCCliente = reader["P2423_RFC"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                    if (estadoDoc != "3")
                    {
                        recibo.TipoCambio = Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"]);
                        recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2405_IMPORTE"]);
                        recibo.IVA = reader["P2416_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2416_IVA"]);
                        recibo.ISR = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                        recibo.IVARetenido = reader["CAMPO_NUM2"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM2"]);
                        recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]);
                        string tasaIVA = reader["P2428_T_IVA"] == DBNull.Value ? "" : reader["P2428_T_IVA"].ToString();
                        recibo.TasaIVA = getTasaIVA(tasaIVA);
                        recibo.Pago = reader["P2413_PAGO"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2413_PAGO"]);
                    }
                    else
                    {
                        recibo.TipoCambio = 0;
                        recibo.Importe = 0;
                        recibo.IVA = 0;
                        recibo.ISR = 0;
                        recibo.IVARetenido = 0;
                        recibo.Total = 0;
                        //string tasaIVA = reader["P2428_T_IVA"] == DBNull.Value ? "" : reader["P2428_T_IVA"].ToString();
                        recibo.TasaIVA = 0;
                        recibo.Pago = 0;
                    }

                    decimal PagoEnCobranzaPago = SaariE.getTotalReciboPagado(recibo.IDHistRec);



                    if (PagoEnCobranzaPago > recibo.Pago)
                    {
                        recibo.Pago = PagoEnCobranzaPago;
                    }

                    #region incluyeDetalleConceptos

                    /*
                    if (incluyeDetalleConceptos)
                    {
                        //List<ConceptoEntity> conceptosTmp = conceptosTmp = getConceptosByHistRec(recibo.IDHistRec);                        
                        ConceptoEntity conceptoPrincipal = new ConceptoEntity();
                        recibo.Conceptos = getConceptosByHistRec(recibo.IDHistRec);
                        //if (conceptosTmp.Count > 0)
                        if (recibo.Conceptos.Count > 0)
                        {
                            //recibo.Conceptos = getConceptosByHistRec(recibo.IDHistRec);
                            conceptoPrincipal.Concepto = recibo.Concepto;
                            conceptoPrincipal.Total = recibo.Total - recibo.Conceptos.Sum(c => c.Total);
                            conceptoPrincipal.Importe = conceptoPrincipal.Total / (1 + recibo.TasaIVA);
                            conceptoPrincipal.IVA = conceptoPrincipal.Importe * recibo.TasaIVA;
                            if (!string.IsNullOrEmpty(recibo.Concepto))
                            {
                                recibo.Conceptos.Insert(0, conceptoPrincipal);
                                recibo.Concepto = string.Empty;
                            }
                        }
                        else
                        {
                            //Codigo agregado para solucionar la problematica con las facturas sin detalle
                            //para incluir facturas sin detalle y con detalle en el mismo reporte
                            //anteriormente no se incluian facturas sin detalle cuando se habilitaba 
                            //la opción de incluir el detalle de los conceptos.
                            ConceptoEntity concepSD = new ConceptoEntity();
                            concepSD.IDHistRec = recibo.IDHistRec;
                            concepSD.IDCargo = "";
                            concepSD.Concepto = recibo.Concepto;
                            concepSD.Cantidad = 1;
                            concepSD.PrecioUnitario = recibo.Total;
                            concepSD.IVA = recibo.IVA;
                            concepSD.Total = recibo.Total;
                            conceptoPrincipal.Concepto = recibo.Concepto;
                            conceptoPrincipal.Total = recibo.Total;
                            conceptoPrincipal.Importe = recibo.Importe;
                            conceptoPrincipal.IVA = conceptoPrincipal.Importe * recibo.TasaIVA;
                            recibo.Conceptos = new List<ConceptoEntity>();
                            recibo.Conceptos.Add(concepSD);
                            recibo.Concepto = string.Empty;
                        }
                    }*/
                    #endregion

                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getFacturaslibresSinPagar(string idInmobiliaria)
        {
            List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT P2401_ID_ARRENDADORA,P2402_ID_ARRENDATARIO,SUM(P2405_IMPORTE) AS IMPORTE,SUM(P2416_IVA) IVA,
SUM(P2415_IMP_DOLAR) AS IMPORTEDLLS, SUM(P2417_IVA_DOLAR) AS IVADLLS,SUM(CAMPO_NUM1) AS RetencionISR, SUM(CAMPO_NUM2),SUM(P2427_CTD_PAG) AS TOTALxPAGAR 
FROM T24_HISTORIA_RECIBOS
WHERE P2418_ID_CONTRATO LIKE '%FACT%'   AND P2406_STATUS = '1' and P2401_ID_ARRENDADORA = ?
GROUP BY P2402_ID_ARRENDATARIO,P2401_ID_ARRENDADORA";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.NVarChar).Value = idInmobiliaria;
                conexion.Open();

                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();

                    recibo.IDInmobiliaria = reader["P2401_ID_ARRENDADORA"].ToString();
                    recibo.NombreInmobiliaria = getInmobiliariaByID(recibo.IDInmobiliaria).NombreComercial;
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    ClienteEntity cliente = getClienteByID(recibo.IDCliente);
                    recibo.NombreCliente = cliente == null ? "" : cliente.Nombre;
                    recibo.nombreComercial = cliente == null ? "" : cliente.NombreComercial;
                    recibo.Importe = reader["IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["IMPORTE"]);
                    recibo.IVA = reader["IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["IVA"]);
                    recibo.ISR = reader["RetencionISR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RetencionISR"]);
                    recibo.IVARetenido = reader["RetencionISR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RetencionISR"]);
                    recibo.Total = reader["TOTALxPAGAR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTALxPAGAR"]);

                    //int PagoEnCobranzaPago = SaariE.getTotalReciboPagado(recibo.IDHistRec);

                    //if (PagoEnCobranzaPago > recibo.Pago)
                    //{
                    //    recibo.Pago = PagoEnCobranzaPago;
                    //}


                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getRecibosExpedidosPorFolioByContrato(string idInmobiliaria, string idContrato, DateTime fechaInicio, DateTime fechaFin)
        {
            List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT T40_CFD.P4001_ID_HIST_REC, T40_CFD.P4002_ID_CFD, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.CAMPO3, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T24_HISTORIA_RECIBOS.P2423_RFC, 
                            T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2416_IVA, T24_HISTORIA_RECIBOS.CAMPO_NUM1,
                            T24_HISTORIA_RECIBOS.CAMPO_NUM2, T24_HISTORIA_RECIBOS.P2406_STATUS,T24_HISTORIA_RECIBOS.P2419_TOTAL, T24_HISTORIA_RECIBOS.P2428_T_IVA, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2406_STATUS  
                            FROM T24_HISTORIA_RECIBOS 
                            INNER JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                            WHERE P2401_ID_ARRENDADORA = ? AND P2402_ID_ARRENDATARIO = ? AND T40_CFD.P4006_SERIE IS NOT NULL AND T40_CFD.P4007_FOLIO IS NOT NULL 
                            AND (T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'R' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'Z' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC='W' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC='X') 
                            AND P2409_FECHA_EMISION BETWEEN ? AND ? AND T24_HISTORIA_RECIBOS.P2407_COMENTARIO != 'CFDi IMPORTADO DESDE XL'
                            ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.NVarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@IdArrendadora", OdbcType.NVarChar).Value = idContrato;
                comando.Parameters.Add("@FechaInicio", OdbcType.DateTime).Value = fechaInicio;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    string estadoDoc = reader["P2406_STATUS"].ToString();
                    recibo.IDHistRec = (int)reader["P4001_ID_HIST_REC"];
                    recibo.IDCFD = (int)reader["P4002_ID_CFD"];
                    recibo.Estatus = reader["P2406_STATUS"].ToString();
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = (int)reader["P4007_FOLIO"];
                    recibo.UUID = SaariE.getUUIDByIDCfd(recibo.IDCFD);
                    recibo.Inmueble = reader["CAMPO3"].ToString();
                    EdificioEntity inmueble = getInmuebleByID(recibo.Inmueble);
                    recibo.NombreInmueble = inmueble == null ? "" : inmueble.Nombre;
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    ClienteEntity cliente = getClienteByID(recibo.IDCliente);
                    recibo.NombreCliente = cliente == null ? "" : cliente.Nombre;
                    recibo.RFCCliente = reader["P2423_RFC"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                    if (estadoDoc != "3")
                    {
                        recibo.TipoCambio = Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"]);
                        recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2405_IMPORTE"]);
                        recibo.IVA = reader["P2416_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2416_IVA"]);
                        recibo.ISR = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                        recibo.IVARetenido = reader["CAMPO_NUM2"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM2"]);
                        recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]);
                        string tasaIVA = reader["P2428_T_IVA"] == DBNull.Value ? "" : reader["P2428_T_IVA"].ToString();
                        recibo.TasaIVA = getTasaIVA(tasaIVA);
                    }
                    else
                    {
                        recibo.TipoCambio = 0;
                        recibo.Importe = 0;
                        recibo.IVA = 0;
                        recibo.ISR = 0;
                        recibo.IVARetenido = 0;
                        recibo.Total = 0;
                        //string tasaIVA = reader["P2428_T_IVA"] == DBNull.Value ? "" : reader["P2428_T_IVA"].ToString();
                        recibo.TasaIVA = 0;
                    }
                    /*
                    if (incluyeDetalleConceptos)
                    {
                        //List<ConceptoEntity> conceptosTmp = conceptosTmp = getConceptosByHistRec(recibo.IDHistRec);                        
                        ConceptoEntity conceptoPrincipal = new ConceptoEntity();
                        recibo.Conceptos = getConceptosByHistRec(recibo.IDHistRec);
                        //if (conceptosTmp.Count > 0)
                        if (recibo.Conceptos.Count > 0)
                        {
                            //recibo.Conceptos = getConceptosByHistRec(recibo.IDHistRec);
                            conceptoPrincipal.Concepto = recibo.Concepto;
                            conceptoPrincipal.Total = recibo.Total - recibo.Conceptos.Sum(c => c.Total);
                            conceptoPrincipal.Importe = conceptoPrincipal.Total / (1 + recibo.TasaIVA);
                            conceptoPrincipal.IVA = conceptoPrincipal.Importe * recibo.TasaIVA;
                            if (!string.IsNullOrEmpty(recibo.Concepto))
                            {
                                recibo.Conceptos.Insert(0, conceptoPrincipal);
                                recibo.Concepto = string.Empty;
                            }
                        }
                        else
                        {
                            //Codigo agregado para solucionar la problematica con las facturas sin detalle
                            //para incluir facturas sin detalle y con detalle en el mismo reporte
                            //anteriormente no se incluian facturas sin detalle cuando se habilitaba 
                            //la opción de incluir el detalle de los conceptos.
                            ConceptoEntity concepSD = new ConceptoEntity();
                            concepSD.IDHistRec = recibo.IDHistRec;
                            concepSD.IDCargo = "";
                            concepSD.Concepto = recibo.Concepto;
                            concepSD.Cantidad = 1;
                            concepSD.PrecioUnitario = recibo.Total;
                            concepSD.IVA = recibo.IVA;
                            concepSD.Total = recibo.Total;
                            conceptoPrincipal.Concepto = recibo.Concepto;
                            conceptoPrincipal.Total = recibo.Total;
                            conceptoPrincipal.Importe = recibo.Importe;
                            conceptoPrincipal.IVA = conceptoPrincipal.Importe * recibo.TasaIVA;
                            recibo.Conceptos = new List<ConceptoEntity>();
                            recibo.Conceptos.Add(concepSD);
                            recibo.Concepto = string.Empty;
                        }
                    }*/
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getRecibosExpedidosPorFolio(string idInmobiliaria, DateTime fechaInicio, DateTime fechaFin, bool incluyeDetalleConceptos)
        {
            List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            /*string sql = @"SELECT T40_CFD.P4001_ID_HIST_REC, T40_CFD.P4002_ID_CFD, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.CAMPO3, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T24_HISTORIA_RECIBOS.P2423_RFC, 
                        T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2416_IVA, T24_HISTORIA_RECIBOS.CAMPO_NUM1,
                        T24_HISTORIA_RECIBOS.CAMPO_NUM2, T24_HISTORIA_RECIBOS.P2419_TOTAL, T24_HISTORIA_RECIBOS.P2428_T_IVA, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2406_STATUS  
                        FROM T24_HISTORIA_RECIBOS 
                        INNER JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                        WHERE P2401_ID_ARRENDADORA = ? AND T40_CFD.P4006_SERIE IS NOT NULL AND T40_CFD.P4007_FOLIO IS NOT NULL 
                        AND (T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'R' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'Z' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC='W' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC='X') 
                        AND P2409_FECHA_EMISION BETWEEN ? AND ?
                        ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO";*/
            //Se excluyen los recibos importados debido a que anteriormente se incluian los recibos importados.
            string sql = @"SELECT T40_CFD.P4001_ID_HIST_REC, T40_CFD.P4002_ID_CFD, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.CAMPO3, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T24_HISTORIA_RECIBOS.P2423_RFC, 
                        T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.P2407_COMENTARIO, T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2416_IVA, T24_HISTORIA_RECIBOS.CAMPO_NUM1,
                        T24_HISTORIA_RECIBOS.CAMPO_NUM2, T24_HISTORIA_RECIBOS.P2419_TOTAL, T24_HISTORIA_RECIBOS.P2428_T_IVA, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2406_STATUS  
                        FROM T24_HISTORIA_RECIBOS 
                        INNER JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                        WHERE P2401_ID_ARRENDADORA = ? AND T40_CFD.P4006_SERIE IS NOT NULL AND T40_CFD.P4007_FOLIO IS NOT NULL 
                        AND (T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'R' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'Z' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC='W' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC='X')                         
                        AND (T24_HISTORIA_RECIBOS.P2407_COMENTARIO != 'CFDi IMPORTADO DESDE XL' or  T24_HISTORIA_RECIBOS.P2407_COMENTARIO is null )
                        AND P2409_FECHA_EMISION BETWEEN ? AND ?
                        ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO   ";

            /*AND T24_HISTORIA_RECIBOS.P2407_COMENTARIO not like 'CFDi IMPORTADO DESDE XL'*/
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.NVarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@FechaInicio", OdbcType.DateTime).Value = fechaInicio;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    string estadoDoc = reader["P2406_STATUS"].ToString();
                    recibo.IDHistRec = (int)reader["P4001_ID_HIST_REC"];
                    recibo.IDCFD = (int)reader["P4002_ID_CFD"];
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = (int)reader["P4007_FOLIO"];
                    recibo.UUID = SaariE.getUUIDByIDCfd(recibo.IDCFD);
                    recibo.Inmueble = reader["CAMPO3"].ToString();
                    EdificioEntity inmueble = getInmuebleByID(recibo.Inmueble);
                    recibo.NombreInmueble = inmueble == null ? "" : inmueble.Nombre;
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    ClienteEntity cliente = getClienteByID(recibo.IDCliente);
                    recibo.NombreCliente = cliente == null ? "" : cliente.Nombre;
                    recibo.RFCCliente = reader["P2423_RFC"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                    if (estadoDoc != "3")
                    {
                        recibo.TipoCambio = Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"]);
                        recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2405_IMPORTE"]);
                        recibo.IVA = reader["P2416_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2416_IVA"]);
                        recibo.ISR = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                        recibo.IVARetenido = reader["CAMPO_NUM2"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM2"]);
                        recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]);
                        string tasaIVA = reader["P2428_T_IVA"] == DBNull.Value ? "" : reader["P2428_T_IVA"].ToString();
                        recibo.TasaIVA = getTasaIVA(tasaIVA);
                    }
                    else
                    {
                        recibo.TipoCambio = 0;
                        recibo.Importe = 0;
                        recibo.IVA = 0;
                        recibo.ISR = 0;
                        recibo.IVARetenido = 0;
                        recibo.Total = 0;
                        //string tasaIVA = reader["P2428_T_IVA"] == DBNull.Value ? "" : reader["P2428_T_IVA"].ToString();
                        recibo.TasaIVA = 0;
                    }
                    ConceptoEntity conceptoPrincipal = new ConceptoEntity();
                    if (incluyeDetalleConceptos)
                    {
                        //List<ConceptoEntity> conceptosTmp = conceptosTmp = getConceptosByHistRec(recibo.IDHistRec);                        

                        recibo.Conceptos = getConceptosByHistRec(recibo.IDHistRec);
                        //if (conceptosTmp.Count > 0)
                        if (recibo.Conceptos.Count > 0)
                        {
                            //recibo.Conceptos = getConceptosByHistRec(recibo.IDHistRec);
                            if (recibo.Conceptos[0].IDCargo.Contains("FACT"))
                            {
                                //toma el valor para cuando es una factura libre.
                                conceptoPrincipal.Concepto = recibo.Conceptos[0].Concepto;
                                conceptoPrincipal.Total = recibo.Total; //recibo.Total - recibo.Conceptos.Sum(c => c.Total);
                                conceptoPrincipal.Importe = recibo.Importe;//conceptoPrincipal.Total / (1 + recibo.TasaIVA);
                                conceptoPrincipal.IVA = recibo.IVA;// conceptoPrincipal.Importe * recibo.TasaIVA;
                                ConceptoEntity concepSD = new ConceptoEntity();
                                //conceptoPrincipal.Concepto = recibo.Concepto;
                                concepSD.Concepto = conceptoPrincipal.Concepto;
                                concepSD.Importe = recibo.Importe;
                                concepSD.IVA = recibo.IVA;
                                concepSD.Total = recibo.Total;
                                recibo.Conceptos = new List<ConceptoEntity>();
                                recibo.Conceptos.Add(concepSD);

                            }
                            else
                            {
                                conceptoPrincipal.Concepto = recibo.Concepto;
                                conceptoPrincipal.Total = recibo.Total - recibo.Conceptos.Sum(c => c.Total);
                                conceptoPrincipal.Importe = conceptoPrincipal.Total / (1 + recibo.TasaIVA);
                                conceptoPrincipal.IVA = conceptoPrincipal.Importe * recibo.TasaIVA;
                            }
                            if (!string.IsNullOrEmpty(recibo.Concepto))
                            {
                                recibo.Conceptos.Insert(0, conceptoPrincipal);
                                recibo.Concepto = string.Empty;
                            }
                        }
                        else
                        {
                            //Codigo agregado para solucionar la problematica con las facturas sin detalle
                            //para incluir facturas sin detalle y con detalle en el mismo reporte
                            //anteriormente no se incluian facturas sin detalle cuando se habilitaba 
                            //la opción de incluir el detalle de los conceptos.
                            ConceptoEntity concepSD = new ConceptoEntity();
                            concepSD.IDHistRec = recibo.IDHistRec;
                            concepSD.IDCargo = "";
                            concepSD.Concepto = recibo.Concepto;
                            concepSD.Cantidad = 1;
                            concepSD.PrecioUnitario = recibo.Total;
                            concepSD.IVA = recibo.IVA;
                            concepSD.Total = recibo.Total;
                            conceptoPrincipal.Concepto = recibo.Concepto;
                            conceptoPrincipal.Total = recibo.Total;
                            conceptoPrincipal.Importe = recibo.Importe;
                            conceptoPrincipal.IVA = conceptoPrincipal.Importe * recibo.TasaIVA;
                            recibo.Conceptos = new List<ConceptoEntity>();
                            recibo.Conceptos.Add(concepSD);
                            recibo.Concepto = string.Empty;
                        }
                    }
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getRecibosExpedidosPorFolio(string idInmobiliaria, DateTime fechaInicio, DateTime fechaFin, bool incluyeDetalleConceptos, bool esVentas, bool esParcialidad)
        {
            List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT T40_CFD.P4001_ID_HIST_REC, T40_CFD.P4002_ID_CFD, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.CAMPO3, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T24_HISTORIA_RECIBOS.P2423_RFC, 
                        T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2416_IVA, T24_HISTORIA_RECIBOS.CAMPO_NUM1,
                        T24_HISTORIA_RECIBOS.CAMPO_NUM2, T24_HISTORIA_RECIBOS.P2419_TOTAL, T24_HISTORIA_RECIBOS.P2428_T_IVA, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2406_STATUS  
                        FROM T24_HISTORIA_RECIBOS 
                        INNER JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                        WHERE P2401_ID_ARRENDADORA = ? AND T40_CFD.P4006_SERIE IS NOT NULL AND T40_CFD.P4007_FOLIO IS NOT NULL ";
            if (!esVentas)
            {
                sql += @"AND (T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'R' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'Z' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC='W') 
                        AND P2409_FECHA_EMISION BETWEEN ? AND ?
                        ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO";
            }
            else if (esParcialidad)
            {
                sql += @"AND (T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'J') 
                        AND P2409_FECHA_EMISION BETWEEN ? AND ?
                        ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO";
            }
            else
            {
                sql += @"AND (T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'I') 
                        AND P2409_FECHA_EMISION BETWEEN ? AND ?
                        ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO";
            }
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.NVarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@FechaInicio", OdbcType.DateTime).Value = fechaInicio;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    string estadoDoc = reader["P2406_STATUS"].ToString();
                    recibo.IDHistRec = (int)reader["P4001_ID_HIST_REC"];
                    recibo.IDCFD = (int)reader["P4002_ID_CFD"];
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = (int)reader["P4007_FOLIO"];
                    recibo.UUID = SaariE.getUUIDByIDCfd(recibo.IDCFD);
                    recibo.Inmueble = reader["CAMPO3"].ToString();
                    EdificioEntity inmueble = getInmuebleByID(recibo.Inmueble);
                    recibo.NombreInmueble = inmueble == null ? "" : inmueble.Nombre;
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    ClienteEntity cliente = getClienteByID(recibo.IDCliente);
                    recibo.NombreCliente = cliente == null ? "" : cliente.Nombre;
                    recibo.RFCCliente = reader["P2423_RFC"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                    if (estadoDoc != "3")
                    {
                        recibo.TipoCambio = Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"]);
                        recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2405_IMPORTE"]);
                        recibo.IVA = reader["P2416_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2416_IVA"]);
                        recibo.ISR = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                        recibo.IVARetenido = reader["CAMPO_NUM2"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM2"]);
                        recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]);
                        string tasaIVA = reader["P2428_T_IVA"] == DBNull.Value ? "" : reader["P2428_T_IVA"].ToString();
                        recibo.TasaIVA = getTasaIVA(tasaIVA);
                    }
                    else
                    {
                        recibo.TipoCambio = 0;
                        recibo.Importe = 0;
                        recibo.IVA = 0;
                        recibo.ISR = 0;
                        recibo.IVARetenido = 0;
                        recibo.Total = 0;
                        //string tasaIVA = reader["P2428_T_IVA"] == DBNull.Value ? "" : reader["P2428_T_IVA"].ToString();
                        recibo.TasaIVA = 0;
                    }

                    if (incluyeDetalleConceptos)
                    {
                        //List<ConceptoEntity> conceptosTmp = conceptosTmp = getConceptosByHistRec(recibo.IDHistRec);                        
                        ConceptoEntity conceptoPrincipal = new ConceptoEntity();
                        recibo.Conceptos = getConceptosByHistRec(recibo.IDHistRec);
                        //if (conceptosTmp.Count > 0)
                        if (recibo.Conceptos.Count > 0)
                        {
                            //recibo.Conceptos = getConceptosByHistRec(recibo.IDHistRec);
                            conceptoPrincipal.Concepto = recibo.Concepto;
                            conceptoPrincipal.Total = recibo.Total - recibo.Conceptos.Sum(c => c.Total);
                            conceptoPrincipal.Importe = conceptoPrincipal.Total / (1 + recibo.TasaIVA);
                            conceptoPrincipal.IVA = conceptoPrincipal.Importe * recibo.TasaIVA;
                            if (!string.IsNullOrEmpty(recibo.Concepto))
                            {
                                recibo.Conceptos.Insert(0, conceptoPrincipal);
                                recibo.Concepto = string.Empty;
                            }
                        }
                        else
                        {
                            //Codigo agregado para solucionar la problematica con las facturas sin detalle
                            //para incluir facturas sin detalle y con detalle en el mismo reporte
                            //anteriormente no se incluian facturas sin detalle cuando se habilitaba 
                            //la opción de incluir el detalle de los conceptos.
                            ConceptoEntity concepSD = new ConceptoEntity();
                            concepSD.IDHistRec = recibo.IDHistRec;
                            concepSD.IDCargo = "";
                            concepSD.Concepto = recibo.Concepto;
                            concepSD.Cantidad = 1;
                            concepSD.PrecioUnitario = recibo.Total;
                            concepSD.IVA = recibo.IVA;
                            concepSD.Total = recibo.Total;
                            conceptoPrincipal.Concepto = recibo.Concepto;
                            conceptoPrincipal.Total = recibo.Total;
                            conceptoPrincipal.Importe = recibo.Importe;
                            conceptoPrincipal.IVA = conceptoPrincipal.Importe * recibo.TasaIVA;
                            recibo.Conceptos = new List<ConceptoEntity>();
                            recibo.Conceptos.Add(concepSD);
                            recibo.Concepto = string.Empty;
                        }
                    }
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getRecibosExpedidosPorFolio(string idInmobiliaria, string idConjunto, DateTime fechaInicio, DateTime fechaFin, bool incluyeDetalleConceptos)
        {
            List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT T40_CFD.P4001_ID_HIST_REC, T40_CFD.P4002_ID_CFD, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.CAMPO3, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T24_HISTORIA_RECIBOS.P2423_RFC, 
                        T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2416_IVA, T24_HISTORIA_RECIBOS.CAMPO_NUM1,
                        T24_HISTORIA_RECIBOS.CAMPO_NUM2, T24_HISTORIA_RECIBOS.P2419_TOTAL, T24_HISTORIA_RECIBOS.P2428_T_IVA, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2406_STATUS  
                        FROM T24_HISTORIA_RECIBOS 
                        INNER JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                        WHERE P2401_ID_ARRENDADORA = ? AND CAMPO4 = ? AND T40_CFD.P4006_SERIE IS NOT NULL AND T40_CFD.P4007_FOLIO IS NOT NULL 
                        AND (T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'R' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'Z') 
                        AND P2409_FECHA_EMISION BETWEEN ? AND ?
                        ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.NVarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = idConjunto;
                comando.Parameters.Add("@FechaInicio", OdbcType.DateTime).Value = fechaInicio;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    string estadoDoc = reader["P2406_STATUS"].ToString();
                    recibo.IDHistRec = (int)reader["P4001_ID_HIST_REC"];
                    recibo.IDCFD = (int)reader["P4002_ID_CFD"];
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = (int)reader["P4007_FOLIO"];
                    recibo.UUID = SaariE.getUUIDByIDCfd(recibo.IDCFD);
                    recibo.Inmueble = reader["CAMPO3"].ToString();
                    EdificioEntity inmueble = getInmuebleByID(recibo.Inmueble);
                    recibo.NombreInmueble = inmueble == null ? "" : inmueble.Nombre;
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    ClienteEntity cliente = getClienteByID(recibo.IDCliente);
                    recibo.NombreCliente = cliente == null ? "" : cliente.Nombre;
                    recibo.RFCCliente = reader["P2423_RFC"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    if (estadoDoc != "3")
                    {

                        recibo.TipoCambio = Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"]);
                        recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                        recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2405_IMPORTE"]);
                        recibo.IVA = reader["P2416_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2416_IVA"]);
                        recibo.ISR = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                        recibo.IVARetenido = reader["CAMPO_NUM2"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM2"]);
                        recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]);
                        string tasaIVA = reader["P2428_T_IVA"] == DBNull.Value ? "" : reader["P2428_T_IVA"].ToString();
                        recibo.TasaIVA = getTasaIVA(tasaIVA);
                    }
                    else
                    {
                        recibo.TipoCambio = 0;
                        recibo.Importe = 0;
                        recibo.IVA = 0;
                        recibo.ISR = 0;
                        recibo.IVARetenido = 0;
                        recibo.Total = 0;
                        //string tasaIVA = reader["P2428_T_IVA"] == DBNull.Value ? "" : reader["P2428_T_IVA"].ToString();
                        recibo.TasaIVA = 0;
                    }

                    if (incluyeDetalleConceptos)
                    {
                        //recibo.Conceptos = getConceptosByHistRec(recibo.IDHistRec);
                        //Se agrego lista de conceptos para que no se pierda la informacion del recibo en caso
                        //de que no tenga detalles
                        List<ConceptoEntity> conceptosTmp = conceptosTmp = getConceptosByHistRec(recibo.IDHistRec);

                        ConceptoEntity conceptoPrincipal = new ConceptoEntity();
                        if (conceptosTmp.Count > 0)
                        {
                            recibo.Conceptos = getConceptosByHistRec(recibo.IDHistRec);
                            conceptoPrincipal.Concepto = recibo.Concepto;
                            conceptoPrincipal.Total = recibo.Total - recibo.Conceptos.Sum(c => c.Total);
                            conceptoPrincipal.Importe = conceptoPrincipal.Total / (1 + recibo.TasaIVA);
                            conceptoPrincipal.IVA = conceptoPrincipal.Importe * recibo.TasaIVA;
                            if (!string.IsNullOrEmpty(recibo.Concepto))
                            {
                                recibo.Conceptos.Insert(0, conceptoPrincipal);
                                recibo.Concepto = string.Empty;
                            }

                        }
                        else //cuando no hay detalle en alguna factura y se selecciona la opcion de incluir detalle
                        {

                            ConceptoEntity concepSD = new ConceptoEntity();
                            concepSD.IDHistRec = recibo.IDHistRec;
                            concepSD.IDCargo = "";
                            concepSD.Concepto = recibo.Concepto;
                            concepSD.Cantidad = 1;
                            concepSD.PrecioUnitario = recibo.Total;
                            concepSD.IVA = recibo.IVA;
                            concepSD.Total = recibo.Total;
                            conceptoPrincipal.Concepto = recibo.Concepto;
                            conceptoPrincipal.Total = recibo.Total;
                            conceptoPrincipal.Importe = recibo.Importe;
                            conceptoPrincipal.IVA = conceptoPrincipal.Importe * recibo.TasaIVA;
                            recibo.Conceptos = new List<ConceptoEntity>();
                            recibo.Conceptos.Add(concepSD);
                        }
                    }
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ReciboEntity> getRecibosExpedidosPorFolio(string idInmobiliaria, string idConjunto, DateTime fechaInicio, DateTime fechaFin, bool incluyeDetalleConceptos, bool esVentas, bool esParcialidad)
        {
            List<ReciboEntity> listaRecibos = new List<ReciboEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT T40_CFD.P4001_ID_HIST_REC, T40_CFD.P4002_ID_CFD, T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION, T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO, T24_HISTORIA_RECIBOS.CAMPO3, T24_HISTORIA_RECIBOS.P2402_ID_ARRENDATARIO, T24_HISTORIA_RECIBOS.P2423_RFC, 
                        T24_HISTORIA_RECIBOS.P2410_MONEDA, T24_HISTORIA_RECIBOS.P2414_TIPO_CAMBIO, T24_HISTORIA_RECIBOS.P2412_CONCEPTO, T24_HISTORIA_RECIBOS.P2405_IMPORTE, T24_HISTORIA_RECIBOS.P2416_IVA, T24_HISTORIA_RECIBOS.CAMPO_NUM1,
                        T24_HISTORIA_RECIBOS.CAMPO_NUM2, T24_HISTORIA_RECIBOS.P2419_TOTAL, T24_HISTORIA_RECIBOS.P2428_T_IVA, T24_HISTORIA_RECIBOS.P2426_TIPO_DOC, T24_HISTORIA_RECIBOS.P2406_STATUS  
                        FROM T24_HISTORIA_RECIBOS 
                        INNER JOIN T40_CFD ON T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC = T40_CFD.P4001_ID_HIST_REC
                        WHERE P2401_ID_ARRENDADORA = ? AND CAMPO4 = ? AND T40_CFD.P4006_SERIE IS NOT NULL AND T40_CFD.P4007_FOLIO IS NOT NULL ";
            if (!esVentas)
            {
                sql += @"AND (T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'R' OR T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'Z') 
                        AND P2409_FECHA_EMISION BETWEEN ? AND ?
                        ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO";
            }
            else if (esParcialidad)
            {
                sql += @"AND (T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'J') 
                        AND P2409_FECHA_EMISION BETWEEN ? AND ?
                        ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO";
            }
            else
            {
                sql += @"AND (T24_HISTORIA_RECIBOS.P2426_TIPO_DOC = 'I') 
                        AND P2409_FECHA_EMISION BETWEEN ? AND ?
                        ORDER BY T40_CFD.P4006_SERIE, T40_CFD.P4007_FOLIO";
            }

            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDInmobiliaria", OdbcType.NVarChar).Value = idInmobiliaria;
                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = idConjunto;
                comando.Parameters.Add("@FechaInicio", OdbcType.DateTime).Value = fechaInicio;
                comando.Parameters.Add("@FechaFin", OdbcType.DateTime).Value = fechaFin;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    string estadoDoc = reader["P2406_STATUS"].ToString();
                    recibo.IDHistRec = (int)reader["P4001_ID_HIST_REC"];
                    recibo.IDCFD = (int)reader["P4002_ID_CFD"];
                    recibo.FechaEmision = Convert.ToDateTime(reader["P2409_FECHA_EMISION"]);
                    recibo.Serie = reader["P4006_SERIE"].ToString();
                    recibo.Folio = (int)reader["P4007_FOLIO"];
                    recibo.UUID = SaariE.getUUIDByIDCfd(recibo.IDCFD);
                    recibo.Inmueble = reader["CAMPO3"].ToString();
                    EdificioEntity inmueble = getInmuebleByID(recibo.Inmueble);
                    recibo.NombreInmueble = inmueble == null ? "" : inmueble.Nombre;
                    recibo.IDCliente = reader["P2402_ID_ARRENDATARIO"].ToString();
                    ClienteEntity cliente = getClienteByID(recibo.IDCliente);
                    recibo.NombreCliente = cliente == null ? "" : cliente.Nombre;
                    recibo.RFCCliente = reader["P2423_RFC"].ToString();
                    recibo.Moneda = reader["P2410_MONEDA"].ToString();
                    if (estadoDoc != "3")
                    {

                        recibo.TipoCambio = Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"]);
                        recibo.Concepto = reader["P2412_CONCEPTO"].ToString();
                        recibo.Importe = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2405_IMPORTE"]);
                        recibo.IVA = reader["P2416_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2416_IVA"]);
                        recibo.ISR = reader["CAMPO_NUM1"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM1"]);
                        recibo.IVARetenido = reader["CAMPO_NUM2"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CAMPO_NUM2"]);
                        recibo.Total = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"]);
                        string tasaIVA = reader["P2428_T_IVA"] == DBNull.Value ? "" : reader["P2428_T_IVA"].ToString();
                        recibo.TasaIVA = getTasaIVA(tasaIVA);
                    }
                    else
                    {
                        recibo.TipoCambio = 0;
                        recibo.Importe = 0;
                        recibo.IVA = 0;
                        recibo.ISR = 0;
                        recibo.IVARetenido = 0;
                        recibo.Total = 0;
                        //string tasaIVA = reader["P2428_T_IVA"] == DBNull.Value ? "" : reader["P2428_T_IVA"].ToString();
                        recibo.TasaIVA = 0;
                    }

                    if (incluyeDetalleConceptos)
                    {
                        //recibo.Conceptos = getConceptosByHistRec(recibo.IDHistRec);
                        //Se agrego lista de conceptos para que no se pierda la informacion del recibo en caso
                        //de que no tenga detalles
                        List<ConceptoEntity> conceptosTmp = conceptosTmp = getConceptosByHistRec(recibo.IDHistRec);

                        ConceptoEntity conceptoPrincipal = new ConceptoEntity();
                        if (conceptosTmp.Count > 0)
                        {
                            recibo.Conceptos = getConceptosByHistRec(recibo.IDHistRec);
                            conceptoPrincipal.Concepto = recibo.Concepto;
                            conceptoPrincipal.Total = recibo.Total - recibo.Conceptos.Sum(c => c.Total);
                            conceptoPrincipal.Importe = conceptoPrincipal.Total / (1 + recibo.TasaIVA);
                            conceptoPrincipal.IVA = conceptoPrincipal.Importe * recibo.TasaIVA;
                            if (!string.IsNullOrEmpty(recibo.Concepto))
                            {
                                recibo.Conceptos.Insert(0, conceptoPrincipal);
                                recibo.Concepto = string.Empty;
                            }

                        }
                        else //cuando no hay detalle en alguna factura y se selecciona la opcion de incluir detalle
                        {

                            ConceptoEntity concepSD = new ConceptoEntity();
                            concepSD.IDHistRec = recibo.IDHistRec;
                            concepSD.IDCargo = "";
                            concepSD.Concepto = recibo.Concepto;
                            concepSD.Cantidad = 1;
                            concepSD.PrecioUnitario = recibo.Total;
                            concepSD.IVA = recibo.IVA;
                            concepSD.Total = recibo.Total;
                            conceptoPrincipal.Concepto = recibo.Concepto;
                            conceptoPrincipal.Total = recibo.Total;
                            conceptoPrincipal.Importe = recibo.Importe;
                            conceptoPrincipal.IVA = conceptoPrincipal.Importe * recibo.TasaIVA;
                            recibo.Conceptos = new List<ConceptoEntity>();
                            recibo.Conceptos.Add(concepSD);
                        }
                    }
                    listaRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();
                return listaRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        private static List<ConceptoEntity> getConceptosByHistRec(int idHistRec)
        {
            List<ConceptoEntity> listaConceptos = new List<ConceptoEntity>();
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = @"SELECT P3409_ID_HIST_REC, P3401_ID_CARGO, P3402_CONCEPTO, P3410_CANTIDAD, P3411_CTO_UNITARI0, P3403_IMPORTE, P3404_IVA, P3405_TOTAL   
                        FROM T35_HISTORIAL_CARGOS WHERE P3409_ID_HIST_REC = ?";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDHistRec", OdbcType.Int).Value = idHistRec;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ConceptoEntity concepto = new ConceptoEntity();
                    concepto.IDHistRec = (int)reader["P3409_ID_HIST_REC"];
                    concepto.IDCargo = reader["P3401_ID_CARGO"].ToString();
                    concepto.Concepto = reader["P3402_CONCEPTO"].ToString();
                    concepto.Cantidad = reader["P3410_CANTIDAD"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3410_CANTIDAD"]);
                    concepto.PrecioUnitario = reader["P3411_CTO_UNITARI0"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3411_CTO_UNITARI0"]);
                    concepto.Importe = reader["P3403_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3403_IMPORTE"]);
                    concepto.IVA = reader["P3404_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3404_IVA"]);
                    concepto.Total = reader["P3405_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P3405_TOTAL"]);
                    listaConceptos.Add(concepto);
                }
                reader.Close();
                conexion.Close();
                return listaConceptos;
            }
            catch
            {
                conexion.Close();
                return null;
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

        #endregion

        #region Reporte de  Impuestos

        public static List<ReporteImpuestos> getRecibosImpuestos()
        {

            string sql = @"SELECT P2409_FECHA_EMISION,T24_HISTORIA_RECIBOS.P2403_NUM_RECIBO AS recibo , P4006_SERIE||'-'|| P4007_FOLIO AS factura, P2411_N_ARRENDATARIO, P2412_CONCEPTO,P2419_TOTAL,P2416_IVA,CAMPO_NUM1,CAMPO_NUM2,
                           P2410_MONEDA, P2414_TIPO_CAMBIO
                           FROM T24_HISTORIA_RECIBOS 
                           JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC";
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                List<ReporteImpuestos> ListRecibos = new List<ReporteImpuestos>();
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                // comando.Parameters.Add("@IDContrato", OdbcType.VarChar).Value = idContrato;
                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReporteImpuestos recibo = new ReporteImpuestos();
                    recibo.FechaEmision = (DateTime)reader["P2409_FECHA_EMISION"];
                    recibo.NoRecibo = Convert.ToInt32(reader["recibo"].ToString());
                    recibo.factura = reader["factura"].ToString();
                    recibo.nombreCliente = reader["P2411_N_ARRENDATARIO"].ToString();
                    recibo.concepto = reader["P2412_CONCEPTO"].ToString();
                    recibo.impuestos = Convert.ToDecimal(reader["P2416_IVA"].ToString());
                    recibo.moneda = reader["P2410_MONEDA"].ToString();
                    recibo.TipoCambioEmision = Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"].ToString());
                    if (recibo.moneda == "D")
                    {
                        recibo.impuestos = recibo.impuestos * recibo.TipoCambioEmision;
                    }

                    ListRecibos.Add(recibo);
                }
                reader.Close();
                conexion.Close();

                return ListRecibos;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        #endregion

        #region Acumulable Para Impuestos
        public static List<AcumulablesImpuestosEntity> GetAcumulablesParaImpuestos(filtroReportes filtro)
        {
            List<AcumulablesImpuestosEntity> lista = new List<AcumulablesImpuestosEntity>();
            List<AcumulablesImpuestosEntity> listaAux = new List<AcumulablesImpuestosEntity>();
            bool incluirConjunto = false;
            if (filtro.IdConjunto != "Todos")
                incluirConjunto = true;
            string sql = @"SELECT DISTINCT  P2401_ID_ARRENDADORA, P2409_FECHA_EMISION,P2408_FECHA_PAGADO,P2403_NUM_RECIBO,P2406_STATUS, P4002_ID_CFD AS P4002_ID_CFD, P4006_SERIE AS P4006_SERIE,P4007_FOLIO AS P4007_FOLIO, 
        T24_HISTORIA_RECIBOS.CAMPO9 AS NombreInmueble,P2410_MONEDA, P2405_IMPORTE,P2416_IVA,P2419_TOTAL,P2414_TIPO_CAMBIO, 
        P2421_TC_PAGO,P2413_PAGO,P2426_TIPO_DOC
        FROM T24_HISTORIA_RECIBOS
        LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC   
WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA= ?";
            if (incluirConjunto)
                sql += @" AND CAMPO4 = ? ";
            sql += @"
            AND(
    T24_HISTORIA_RECIBOS.P2406_STATUS in ('1','2')
    AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','X','W','T')
    AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ? 
    AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?
    )
OR(
    T24_HISTORIA_RECIBOS.P2406_STATUS in ('2')
    AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('P')
    AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION >= ?
    AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?
    AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ?
  )
UNION

SELECT DISTINCT  P2401_ID_ARRENDADORA, P2409_FECHA_EMISION,P2408_FECHA_PAGADO,P2403_NUM_RECIBO,P2406_STATUS, P4002_ID_CFD AS P4002_ID_CFD, P4006_SERIE AS P4006_SERIE,P4007_FOLIO AS P4007_FOLIO, 
        T24_HISTORIA_RECIBOS.CAMPO9 AS NombreInmueble,P2410_MONEDA, P2405_IMPORTE,P2416_IVA,P2419_TOTAL,P2414_TIPO_CAMBIO, 
        P2421_TC_PAGO,P2413_PAGO,P2426_TIPO_DOC
        FROM T24_HISTORIA_RECIBOS
        LEFT JOIN T40_CFD ON T40_CFD.P4001_ID_HIST_REC = T24_HISTORIA_RECIBOS.P2444_ID_HIST_REC   
WHERE T24_HISTORIA_RECIBOS.P2401_ID_ARRENDADORA= ?";
            if (incluirConjunto)
                sql += @" AND CAMPO4 = ? ";
            sql += @"
AND (
    T24_HISTORIA_RECIBOS.P2406_STATUS in ('2')
    AND T24_HISTORIA_RECIBOS.P2426_TIPO_DOC IN ('R','Z','W','T','P')
    AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO >= ? 
    AND T24_HISTORIA_RECIBOS.P2408_FECHA_PAGADO <= ?
    AND T24_HISTORIA_RECIBOS.P2409_FECHA_EMISION <= ?
    )


";

            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                comando.Parameters.Add("@IDArr", OdbcType.NVarChar).Value = filtro.IdInmobiliaria;
                if (incluirConjunto)
                    comando.Parameters.Add("@IDConjunto", OdbcType.NVarChar).Value = filtro.IdConjunto;
                comando.Parameters.Add("@fechaInicio", OdbcType.DateTime).Value = filtro.FechaInicio;
                comando.Parameters.Add("@fechaFin", OdbcType.DateTime).Value = filtro.FechaFin;
                comando.Parameters.Add("@fechaInicio2", OdbcType.DateTime).Value = filtro.FechaInicio;
                comando.Parameters.Add("@fechaFin2", OdbcType.DateTime).Value = filtro.FechaFin;
                comando.Parameters.Add("@fechaFin3", OdbcType.DateTime).Value = filtro.FechaFin;

                comando.Parameters.Add("@IDArr", OdbcType.NVarChar).Value = filtro.IdInmobiliaria;
                if (incluirConjunto)
                    comando.Parameters.Add("@IDConjunto", OdbcType.NVarChar).Value = filtro.IdConjunto;
                comando.Parameters.Add("@fechaInicio4", OdbcType.DateTime).Value = filtro.FechaInicio;
                comando.Parameters.Add("@fechaFin4", OdbcType.DateTime).Value = filtro.FechaFin;
                comando.Parameters.Add("@fechaInicio4", OdbcType.DateTime).Value = filtro.FechaFin;


                conexion.Open();
                OdbcDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    AcumulablesImpuestosEntity cfdi = new AcumulablesImpuestosEntity();


                    cfdi.FechaEmision = reader["P2409_FECHA_EMISION"] == DBNull.Value ? new DateTime(1, 1, 1999) : (DateTime)reader["P2409_FECHA_EMISION"];
                    cfdi.Referencia = reader["P2403_NUM_RECIBO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P2403_NUM_RECIBO"].ToString());
                    cfdi.ID_CFD = reader["P4002_ID_CFD"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P4002_ID_CFD"].ToString());
                    cfdi.Serie = reader["P4006_SERIE"] == DBNull.Value ? string.Empty : reader["P4006_SERIE"].ToString();
                    cfdi.Folio = reader["P4007_FOLIO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P4007_FOLIO"].ToString());
                    cfdi.UUID = SaariE.getUUIDByIDCfd(cfdi.ID_CFD);
                    cfdi.Bodega = reader["NombreInmueble"] == DBNull.Value ? string.Empty : reader["NombreInmueble"].ToString();
                    cfdi.TipoDoc = reader["P2426_TIPO_DOC"] == DBNull.Value ? string.Empty : reader["P2426_TIPO_DOC"].ToString();
                    cfdi.TipoDoc = reader["P2426_TIPO_DOC"] == DBNull.Value ? string.Empty : reader["P2426_TIPO_DOC"].ToString();
                    cfdi.TipoDoc = reader["P2426_TIPO_DOC"] == DBNull.Value ? string.Empty : reader["P2426_TIPO_DOC"].ToString();
                    cfdi.ImporteEmision = reader["P2405_IMPORTE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2405_IMPORTE"].ToString());
                    cfdi.IvaEmision = reader["P2416_IVA"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2416_IVA"].ToString());
                    cfdi.TotalEmision = reader["P2419_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["P2419_TOTAL"].ToString());
                    cfdi.TCEmision = reader["P2414_TIPO_CAMBIO"] == DBNull.Value ? 1 : Convert.ToDecimal(reader["P2414_TIPO_CAMBIO"].ToString());
                    cfdi.ImporteConversion = Math.Round(cfdi.ImporteEmision * cfdi.TCEmision, 4);
                    cfdi.IvaConversion = Math.Round(cfdi.IvaEmision * cfdi.TCEmision, 4);
                    cfdi.Estatus = reader["P2406_STATUS"] == DBNull.Value ? string.Empty : reader["P2406_STATUS"].ToString();
                    DateTime fechaPago = reader["P2408_FECHA_PAGADO"] == DBNull.Value ? new DateTime(1999, 1, 1) : (DateTime)reader["P2408_FECHA_PAGADO"];
                    if ((fechaPago.Date == new DateTime(1999, 1, 1)) || (cfdi.TipoDoc == "X" && cfdi.Estatus == "1"))
                        cfdi.FechaPago = string.Empty;
                    else
                        cfdi.FechaPago = fechaPago.Day.ToString("00") + "/" + fechaPago.Month.ToString("00") + "/" + fechaPago.Year;
                    cfdi.TCCobro = reader["P2421_TC_PAGO"] == DBNull.Value ? 1 : Convert.ToDecimal(reader["P2421_TC_PAGO"].ToString());
                    cfdi.ImporteCobro = cfdi.ImporteEmision;
                    cfdi.ImporteConversion2 = Math.Round(cfdi.ImporteCobro * cfdi.TCCobro, 4);
                    cfdi.IvaConversion2 = Math.Round(cfdi.IvaEmision * cfdi.TCCobro, 4);


                    if (cfdi.FechaEmision >= filtro.FechaInicio && cfdi.FechaEmision <= filtro.FechaFin)
                        cfdi.EsMesCorriente = true;

                    if (cfdi.TipoDoc == "P")
                    {
                        ReciboEntity r = SaariDB.getSeriFolioCuandoEsPagoParcial2(cfdi.Referencia);
                        cfdi.PagoParcial = "PP - " + r.Serie + " " + r.Folio;
                        cfdi.ImporteEmision = 0;
                        cfdi.IvaEmision = 0;
                        cfdi.TotalEmision = 0;
                        cfdi.ImporteConversion = 0;
                        cfdi.IvaConversion = 0;
                    }

                    if (cfdi.Estatus == "1")
                    {
                        cfdi.ImporteCobro = 0;
                        cfdi.ImporteConversion2 = 0;
                        cfdi.IvaConversion2 = 0;
                    }

                    if (cfdi.TipoDoc == "X" && cfdi.Estatus == "2")
                    {
                        cfdi.ImporteCobro = 0;
                        cfdi.ImporteConversion2 = 0;
                        cfdi.IvaConversion2 = 0;
                    }
                    if (cfdi.TipoDoc == "X" && cfdi.Estatus == "1")
                    { }
                    else
                        listaAux.Add(cfdi);
                }
                lista = listaAux;

                reader.Close();
                conexion.Close();
            }
            catch (Exception e)
            {
                conexion.Close();
                return null;
            }



            return lista;
        }


        #endregion


        public static DataTable executeQuery(string sqlText)
        {
            DataTable dt = new DataTable();
            try
            {
                OdbcDataReader dr = null;
                using (OdbcConnection odbcConn = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString))
                {
                    OdbcCommand cmd = new OdbcCommand();
                    cmd.CommandTimeout = 100;
                    cmd.Connection = odbcConn;
                    odbcConn.Open();
                    if (odbcConn.State == ConnectionState.Open)
                    {
                        cmd.CommandText = sqlText;
                        dr = cmd.ExecuteReader();
                        dt.Load(dr);
                    }
                    odbcConn.Close();
                    odbcConn.Dispose();
                    cmd.Dispose();
                }
                dr.Dispose();
            }
            catch (Exception ex)
            {
                dt = new DataTable();
            }
            return dt;
        }
    
    }
}
