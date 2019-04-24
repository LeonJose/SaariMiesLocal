using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GestorReportes.BusinessLayer.DataAccessLayer.ArrendadoraTableAdapters;
using GestorReportes.BusinessLayer.EntitiesReportes;
using System.Data.Odbc;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class Inmobiliaria
    {
        #region No clasificados
        //Se cambio los que estaban como T18_SUBCONJUNTOS.P18_CAMPO4 por T18_SUBCONJUNTOS.P1804_ID_CONJUNTO en los JOIN de 2 metodos
        //Se supone que el CAMPO 4 va a almacenar el valor del inmueble principal
        public DataTable getDtInmobiliarias()
        {
            try
            {
                T01_ARRENDADORATableAdapter taArrendadora = new T01_ARRENDADORATableAdapter();
                taArrendadora.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;
                DataTable dtArrendadora = taArrendadora.GetArrendadora();
                if (dtArrendadora.Rows.Count <= 0)
                    return new DataTable();
                else
                    return dtArrendadora;
            }
            catch
            {
                return new DataTable();
            }
        }
        public DataTable getDtInmobiliariasUnPropietario()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL,P0109_STATUS FROM T01_ARRENDADORA  WHERE P0109_STATUS = 'S' ORDER BY P0103_RAZON_SOCIAL";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                DataTable dt = new DataTable();
                //List<InmobiliariaEntity> listaInmobiliarias = new List<InmobiliariaEntity>();
                //conexion.Open();                
                OdbcDataAdapter adapter = new OdbcDataAdapter(comando);
                //conexion.Close();
                adapter.Fill(dt);
                return dt;
            }
            catch
            {
                // conexion.Close();
                return null;
            }
        }
        public DataTable getDtInmobiliariasEnCopropiedad()
        {
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            string sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL,P0109_STATUS FROM T01_ARRENDADORA  WHERE P0109_STATUS = 'S' ORDER BY P0103_RAZON_SOCIAL";
            try
            {
                OdbcCommand comando = new OdbcCommand(sql, conexion);
                DataTable dt = new DataTable();
                //List<InmobiliariaEntity> listaInmobiliarias = new List<InmobiliariaEntity>();
                //conexion.Open();                
                OdbcDataAdapter adapter = new OdbcDataAdapter(comando);
                //conexion.Close();
                adapter.Fill(dt);
                return dt;
            }
            catch
            {
                // conexion.Close();
                return null;
            }
        }
        public DataTable getDtConjuntos(string idInmobiliaria)
        {
            try
            {
                T03_CENTRO_INDUSTRIALTableAdapter taConjuntos = new T03_CENTRO_INDUSTRIALTableAdapter();
                taConjuntos.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;
                DataTable dtConjuntos = taConjuntos.GetConjuntosByIdArr(idInmobiliaria);
                if (dtConjuntos.Rows.Count <= 0)
                    return new DataTable();
                else
                    return dtConjuntos;
            }
            catch
            {
                return new DataTable();
            }
        }

        public DataTable getDtSubConjuntos(string idConjunto)
        {
            try
            {
                T18_SUBCONJUNTOSTableAdapter taSubConjuntos = new T18_SUBCONJUNTOSTableAdapter();
                taSubConjuntos.Connection.ConnectionString = GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString;
                DataTable dtSubConjuntos = taSubConjuntos.GetSubconjuntosByIdConjunto(idConjunto);
                if (dtSubConjuntos.Rows.Count <= 0)
                    return new DataTable();
                else
                    return dtSubConjuntos;
            }
            catch
            {
                return new DataTable();
            }
        }

        public DataTable getGposCommand()
        {
            try
            {
                string sql = "SELECT P0001_ID_GRUPO, P0002_NOMBRE FROM T00_GRUPOS_EMP";
                OdbcConnection conex = new OdbcConnection(GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString);
                OdbcCommand comando = new OdbcCommand(sql, conex);
                DataTable dtGpos = new DataTable();
                conex.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtGpos.Load(reader);
                conex.Close();
                conex.Dispose();
                if (dtGpos.Rows.Count > 0)
                    return dtGpos;
                else
                    return new DataTable();
            }
            catch
            {
                return new DataTable();
            }
        }

        public DataTable getDtContribuyentes()
        {
            string sql = "SELECT P4202_RAZON_SOCIAL, P4203_RFC FROM T42_CONTRIBUYENTES";
            return getDataTable(sql);
        }

        //Add By Uz 
        //05/09/2016
        //Para utilizar en Frm_generarXMLAntilavado
        //Mostrar listado de clientes por rfc de la arrendadora (para selecionar cliente general o genérico para excluir de reporte SPPLD)
        public DataTable getClientesByRfcArrendora(string rfc)
        {
            string sql =  @"SELECT T04_CONTRATO.P0402_ID_ARRENDAT, T02_ARRENDATARIO.P0203_NOMBRE, T02_ARRENDATARIO.P0204_RFC FROM T04_CONTRATO
                            JOIN T01_ARRENDADORA ON T01_ARRENDADORA.P0101_ID_ARR= T04_CONTRATO.P0403_ID_ARRENDADORA
                            JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID= T04_CONTRATO.P0402_ID_ARRENDAT
                            WHERE T01_ARRENDADORA.P0106_RFC = '" + rfc + "'";
            DataTable dtClientes = getDataTable(sql);
            if (dtClientes.Rows.Count > 0)
                return dtClientes;
            else
                return null;  
        }
        public string getRazonContribuyenteByRFC(string rfc)
        {
            string sql = "SELECT P4202_RAZON_SOCIAL FROM T42_CONTRIBUYENTES WHERE P4203_RFC = '" + rfc + "'";
            DataTable dtContrib = getDataTable(sql);
            if (dtContrib.Rows.Count > 0)
                return dtContrib.Rows[0][0].ToString();
            else
                return string.Empty;            
        }

        public string getRfcByInmobiliaria(string inmobiliaria)
        {
            string sql = "SELECT P0106_RFC FROM T01_ARRENDADORA WHERE P0101_ID_ARR = '" + inmobiliaria.Trim() + "'";
            return getDataTable(sql).Rows[0][0].ToString();
        }

        public DataTable getDtTiposCambio(DateTime inicio, DateTime fin)
        {
            string fechaInicio = inicio.Month + "/" + inicio.Day + "/" + inicio.Year;
            string fechaFin = fin.Month + "/" + fin.Day + "/" + fin.Year;
            string sql = "SELECT P3904_VALOR_INDEX FROM T39_INDICES_ECONOMICOS WHERE P3901_TIPO_INDEX = 'TIPOCAMDLS' AND P3902_MES_INDEX >= '" +
                fechaInicio + "' AND P3902_MES_INDEX <= '" + fechaFin + "'";
            return getDataTable(sql);
        }

        public DataTable getDtAllConjuntos()
        {
            string sql = "SELECT P0301_ID_CENTRO, P0303_NOMBRE FROM T03_CENTRO_INDUSTRIAL";
            return getDataTable(sql);
        }

        public DataTable getDtAllSubConjuntos()
        {
            string sql = "SELECT P1801_ID_SUBCONJUNTO, P1803_NOMBRE FROM T18_SUBCONJUNTOS";
            return getDataTable(sql);
        }

        public DataTable getDtAllClientes()
        {
            string sql = "SELECT T04_CONTRATO.P0401_ID_CONTRATO, T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE FROM T04_CONTRATO " +
                "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID";
            return getDataTable(sql);
        }

        public DataTable getDtClientesPorGpoEmp(string idGpoEmp)
        {
            string sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, T04_CONTRATO.P0401_ID_CONTRATO, T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE FROM T01_ARRENDADORA " +
                "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID " +
                "WHERE P0151_ID_GRUPO_EMP = '"+idGpoEmp+"'";
            return getDataTable(sql);
        }

        public DataTable getDtClientesPorInm(string idInmob)
        {
            string sql = "SELECT P0101_ID_ARR, T04_CONTRATO.P0401_ID_CONTRATO, T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE FROM T01_ARRENDADORA " +
                "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID " +
                "WHERE T01_ARRENDADORA.P0101_ID_ARR = '" + idInmob + "'";
            return getDataTable(sql);
        }

        public DataTable getDtClientesPorInmYConj(string idInmob, string idConj)
        {
            string sql = "SELECT P0101_ID_ARR, T04_CONTRATO.P0401_ID_CONTRATO, T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO FROM T01_ARRENDADORA " +
                "JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.CAMPO1 = T01_ARRENDADORA.P0101_ID_ARR " +
                "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID " +
                "WHERE T01_ARRENDADORA.P0101_ID_ARR = '" + idInmob + "' AND T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = '" + idConj + "'";
            return getDataTable(sql);
        }

        public DataTable getDtSubRentados(string grupo, DateTime vigencia)
        {
            string sql = string.Empty;
            if (grupo == "Todos")
            {
                sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA, T18_SUBCONJUNTOS.P18_CAMPO1, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, T02_ARRENDATARIO.P0203_NOMBRE, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE FROM T01_ARRENDADORA " +
                    "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                    "JOIN T18_SUBCONJUNTOS ON T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO " +
                    "JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T18_SUBCONJUNTOS.P1804_ID_CONJUNTO " +
                    "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID  " +
                    "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                    "WHERE T04_CONTRATO.P0410_INICIO_VIG <= '" + vigencia.ToString("MM/dd/yyyy") + "' AND T04_CONTRATO.P0411_FIN_VIG >= '" + vigencia.ToString("MM/dd/yyyy") + "'";
            }
            else
            {
                sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T00_GRUPOS_EMP.P0002_NOMBRE, P0101_ID_ARR, P0103_RAZON_SOCIAL, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA, T18_SUBCONJUNTOS.P18_CAMPO1, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, T02_ARRENDATARIO.P0203_NOMBRE, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE FROM T01_ARRENDADORA " +
                    "JOIN T00_GRUPOS_EMP ON T00_GRUPOS_EMP.P0001_ID_GRUPO = T01_ARRENDADORA.P0151_ID_GRUPO_EMP " +
                    "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                    "JOIN T18_SUBCONJUNTOS ON T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO " +
                    "JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T18_SUBCONJUNTOS.P1804_ID_CONJUNTO " +
                    "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID  " +
                    "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                    "WHERE T04_CONTRATO.P0410_INICIO_VIG <= '" + vigencia.ToString("MM/dd/yyyy") + "' AND T04_CONTRATO.P0411_FIN_VIG >= '" + vigencia.ToString("MM/dd/yyyy") + "' AND T01_ARRENDADORA.P0151_ID_GRUPO_EMP = '" + grupo + "'";
            }
            return getDataTable(sql);
        }

        public DataTable getDtSubRentadosFlujoNeto(string grupo, DateTime vigencia)
        {
            string sql = string.Empty;
            if (grupo == "Todos")
            {
                sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T00_GRUPOS_EMP.P0002_NOMBRE, P0101_ID_ARR, P0103_RAZON_SOCIAL, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T04_CONTRATO.P0401_ID_CONTRATO, T18_SUBCONJUNTOS.P1803_NOMBRE, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA, T18_SUBCONJUNTOS.P18_CAMPO1, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, T02_ARRENDATARIO.P0203_NOMBRE, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE FROM T01_ARRENDADORA " +
                    "JOIN T00_GRUPOS_EMP ON T00_GRUPOS_EMP.P0001_ID_GRUPO = T01_ARRENDADORA.P0151_ID_GRUPO_EMP " +
                    "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                    "JOIN T18_SUBCONJUNTOS ON T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO " +
                    "JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T18_SUBCONJUNTOS.P1804_ID_CONJUNTO " +
                    "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID  " +
                    "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                    "WHERE T04_CONTRATO.P0410_INICIO_VIG <= '12/31/" + vigencia.Year + "' AND T04_CONTRATO.P0411_FIN_VIG >= '01/01/" + vigencia.Year + "'";
            }
            else
            {
                sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T00_GRUPOS_EMP.P0002_NOMBRE, P0101_ID_ARR, P0103_RAZON_SOCIAL, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T04_CONTRATO.P0401_ID_CONTRATO, T18_SUBCONJUNTOS.P1803_NOMBRE, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA, T18_SUBCONJUNTOS.P18_CAMPO1, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, T02_ARRENDATARIO.P0203_NOMBRE, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE FROM T01_ARRENDADORA " +
                    "JOIN T00_GRUPOS_EMP ON T00_GRUPOS_EMP.P0001_ID_GRUPO = T01_ARRENDADORA.P0151_ID_GRUPO_EMP " +
                    "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                    "JOIN T18_SUBCONJUNTOS ON T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO " +
                    "JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T18_SUBCONJUNTOS.P1804_ID_CONJUNTO " +
                    "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID  " +
                    "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                    "WHERE T04_CONTRATO.P0410_INICIO_VIG <= '12/31/" + vigencia.Year + "' AND T04_CONTRATO.P0411_FIN_VIG >= '01/01/" + vigencia.Year + "' AND T01_ARRENDADORA.P0151_ID_GRUPO_EMP = '" + grupo + "'";
            }
            return getDataTable(sql);
        }

        public DataTable getDtContratosPorGrupoFlujoNeto(string grupo, DateTime vigencia)
        {
            string sql = string.Empty;
            if (grupo == "Todos")
            {
                sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, T04_CONTRATO.P0401_ID_CONTRATO, T59_HISTORIA_CONTRATOS.P5941_IMP_ACTUAL_NVO, T59_HISTORIA_CONTRATOS.P5976_FECHA_HORA_UPDT, T59_HISTORIA_CONTRATOS.P5911_INICIO_VIG_NVO, T59_HISTORIA_CONTRATOS.P5913_FIN_VIG_NVO, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, T04_CONTRATO.P0415_AUMENTO_ANUAL, T04_CONTRATO.P0432_FECHA_AUMENTO FROM T01_ARRENDADORA " +
                    "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                    "JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T04_CONTRATO.P0404_ID_EDIFICIO  " +
                    "JOIN T59_HISTORIA_CONTRATOS ON T04_CONTRATO.P0401_ID_CONTRATO = T59_HISTORIA_CONTRATOS.P5901_ID_CONTRATO " +
                    "WHERE T04_CONTRATO.P0410_INICIO_VIG <= '12/31/" + vigencia.Year + "' AND T04_CONTRATO.P0411_FIN_VIG >= '01/01/" + vigencia.Year + "' ORDER BY 'P0401_ID_CONTRATO' ASC";
            }
            else
            {
                sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T00_GRUPOS_EMP.P0002_NOMBRE, P0101_ID_ARR, P0103_RAZON_SOCIAL, T04_CONTRATO.P0401_ID_CONTRATO, T59_HISTORIA_CONTRATOS.P5941_IMP_ACTUAL_NVO, T59_HISTORIA_CONTRATOS.P5976_FECHA_HORA_UPDT, T59_HISTORIA_CONTRATOS.P5911_INICIO_VIG_NVO, T59_HISTORIA_CONTRATOS.P5913_FIN_VIG_NVO, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, T04_CONTRATO.P0415_AUMENTO_ANUAL, T04_CONTRATO.P0432_FECHA_AUMENTO FROM T01_ARRENDADORA " +
                    "JOIN T00_GRUPOS_EMP ON T00_GRUPOS_EMP.P0001_ID_GRUPO = T01_ARRENDADORA.P0151_ID_GRUPO_EMP " +
                    "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                    "JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T04_CONTRATO.P0404_ID_EDIFICIO  " +
                    "JOIN T59_HISTORIA_CONTRATOS ON T04_CONTRATO.P0401_ID_CONTRATO = T59_HISTORIA_CONTRATOS.P5901_ID_CONTRATO " +
                    "WHERE T04_CONTRATO.P0410_INICIO_VIG <= '12/31/" + vigencia.Year + "' AND T04_CONTRATO.P0411_FIN_VIG >= '01/01/" + vigencia.Year + "' AND T01_ARRENDADORA.P0151_ID_GRUPO_EMP = '" + grupo + "' ORDER BY 'P0401_ID_CONTRATO' ASC";
            }
            return getDataTable(sql);
        }

        public DataTable getDtArrendadorasContratosClientes(string grupo, string inmobiliaria, string conjunto, string subConjunto, string cliente)
        {
            string sql = string.Empty;
            if (grupo == "Todos")
            {
                if (cliente == "Todos")
                {
                    sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.CAMPO4, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0432_FECHA_AUMENTO, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, T04_CONTRATO.P0415_AUMENTO_ANUAL, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0418_IMPORTE_DEPOSITO, T04_CONTRATO.P0448_CAMPO12, T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA FROM T01_ARRENDADORA " +
                                "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                                "JOIN T18_SUBCONJUNTOS ON T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO  " +
                                "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID " +
                                "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE ";
                }
                else
                {
                    sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.CAMPO4, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0432_FECHA_AUMENTO, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, T04_CONTRATO.P0415_AUMENTO_ANUAL, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0418_IMPORTE_DEPOSITO, T04_CONTRATO.P0448_CAMPO12, T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA FROM T01_ARRENDADORA " +
                                 "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                                 "JOIN T18_SUBCONJUNTOS ON T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO  " +
                                 "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID " +
                                 "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                                 "WHERE P0201_ID = '" + cliente + "'";
                }
            }
            else
            {
                if (inmobiliaria == "Todos")
                {
                    if (cliente == "Todos")
                    {
                        sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.CAMPO4, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0432_FECHA_AUMENTO, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, T04_CONTRATO.P0415_AUMENTO_ANUAL, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0418_IMPORTE_DEPOSITO, T04_CONTRATO.P0448_CAMPO12, T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA FROM T01_ARRENDADORA " +
                                 "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                                 "JOIN T18_SUBCONJUNTOS ON T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO  " +
                                 "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID " +
                                 "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                                "WHERE P0151_ID_GRUPO_EMP = '" + grupo + "'";
                    }
                    else
                    {
                        sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.CAMPO4, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0432_FECHA_AUMENTO, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, T04_CONTRATO.P0415_AUMENTO_ANUAL, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0418_IMPORTE_DEPOSITO, T04_CONTRATO.P0448_CAMPO12, T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA FROM T01_ARRENDADORA " +
                                 "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                                 "JOIN T18_SUBCONJUNTOS ON T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO  " +
                                 "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID " +
                                 "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                                "WHERE P0151_ID_GRUPO_EMP = '" + grupo + "' AND P0201_ID = '" + cliente + "'";
                    }
                }
                else
                {
                    if (conjunto == "Todos")
                    {
                        if (cliente == "Todos")
                        {
                            sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.CAMPO4, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0432_FECHA_AUMENTO, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, T04_CONTRATO.P0415_AUMENTO_ANUAL, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0418_IMPORTE_DEPOSITO, T04_CONTRATO.P0448_CAMPO12, T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA FROM T01_ARRENDADORA " +
                                  "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                                  "JOIN T18_SUBCONJUNTOS ON T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO  " +
                                  "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID " +
                                  "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                                "WHERE P0151_ID_GRUPO_EMP = '" + grupo + "' AND T01_ARRENDADORA.P0101_ID_ARR = '" + inmobiliaria + "'";
                        }
                        else
                        {
                            sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.CAMPO4, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0432_FECHA_AUMENTO, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, T04_CONTRATO.P0415_AUMENTO_ANUAL, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0418_IMPORTE_DEPOSITO, T04_CONTRATO.P0448_CAMPO12, T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA FROM T01_ARRENDADORA " +
                                 "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                                 "JOIN T18_SUBCONJUNTOS ON T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO  " +
                                 "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID " +
                                 "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                                "WHERE P0151_ID_GRUPO_EMP = '" + grupo + "' AND T01_ARRENDADORA.P0101_ID_ARR = '" + inmobiliaria + "' AND P0201_ID = '" + cliente + "'";
                        }
                    }
                    else
                    {
                        if (subConjunto == "Todos")
                        {
                            if (cliente == "Todos")
                            {
                                sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.CAMPO4, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0432_FECHA_AUMENTO, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, T04_CONTRATO.P0415_AUMENTO_ANUAL, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0418_IMPORTE_DEPOSITO, T04_CONTRATO.P0448_CAMPO12, T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA FROM T01_ARRENDADORA " +
                                 "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                                 "JOIN T18_SUBCONJUNTOS ON T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO  " +
                                 "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID " +
                                 "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                                "WHERE P0151_ID_GRUPO_EMP = '" + grupo + "' AND T01_ARRENDADORA.P0101_ID_ARR = '" + inmobiliaria + "' AND T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = '" + conjunto + "'";
                            }
                            else
                            {
                                sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.CAMPO4, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0432_FECHA_AUMENTO, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, T04_CONTRATO.P0415_AUMENTO_ANUAL, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0418_IMPORTE_DEPOSITO, T04_CONTRATO.P0448_CAMPO12, T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA FROM T01_ARRENDADORA " +
                                 "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                                 "JOIN T18_SUBCONJUNTOS ON T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO  " +
                                 "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID " +
                                 "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                                "WHERE P0151_ID_GRUPO_EMP = '" + grupo + "' AND T01_ARRENDADORA.P0101_ID_ARR = '" + inmobiliaria + "' AND T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = '" + conjunto + "' AND P0201_ID = '" + cliente + "'";
                            }
                        }
                        else
                        {
                            sql = "SELECT P0101_ID_ARR, P0103_RAZON_SOCIAL, T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.CAMPO4, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0432_FECHA_AUMENTO, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, T04_CONTRATO.P0415_AUMENTO_ANUAL, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0418_IMPORTE_DEPOSITO, T04_CONTRATO.P0448_CAMPO12, T02_ARRENDATARIO.P0201_ID, T02_ARRENDATARIO.P0203_NOMBRE, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1804_ID_CONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA FROM T01_ARRENDADORA " +
                                 "JOIN T04_CONTRATO ON T04_CONTRATO.P0403_ID_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                                 "JOIN T18_SUBCONJUNTOS ON T04_CONTRATO.P0404_ID_EDIFICIO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO  " +
                                 "JOIN T02_ARRENDATARIO ON T04_CONTRATO.P0402_ID_ARRENDAT = T02_ARRENDATARIO.P0201_ID " +
                                 "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                                "WHERE P0151_ID_GRUPO_EMP = '" + grupo + "' AND T01_ARRENDADORA.P0101_ID_ARR = '" + inmobiliaria + "' AND T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = '" + conjunto + "' AND T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = '" + subConjunto + "'";
                        }
                    }
                }
            }

            return getDataTable(sql);
        }

        public DataTable getInmobiliariasCommand(string idGpoEmp)
        {
            try
            {
                string sql = string.Empty;
                if(idGpoEmp == "Todos")
                    sql = "SELECT P0101_ID_ARR, P0102_N_COMERCIAL, P0103_RAZON_SOCIAL FROM T01_ARRENDADORA";
                else
                    sql = "SELECT P0101_ID_ARR, P0102_N_COMERCIAL, P0103_RAZON_SOCIAL FROM T01_ARRENDADORA WHERE P0151_ID_GRUPO_EMP = '" + idGpoEmp + "'";
                OdbcConnection conex = new OdbcConnection(GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString);
                OdbcCommand comando = new OdbcCommand(sql, conex);
                DataTable dtInmob = new DataTable();
                conex.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtInmob.Load(reader);
                conex.Close();
                conex.Dispose();
                if (dtInmob.Rows.Count > 0)
                {
                    return dtInmob;
                }
                else
                {
                    return new DataTable();
                }
            }
            catch
            {
                return new DataTable();
            }
        }
        
        public DataTable getConjuntosCommand(string idInmob)
        {
            try
            {
                string sql = "SELECT P0301_ID_CENTRO, P0303_NOMBRE FROM T03_CENTRO_INDUSTRIAL WHERE CAMPO1 = ?";
                OdbcConnection conex = new OdbcConnection(GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString);
                OdbcCommand comando = new OdbcCommand(sql, conex);
                comando.Parameters.Add("@idInmob", OdbcType.VarChar).Value = idInmob;
                DataTable dtConjuntos = new DataTable();
                conex.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtConjuntos.Load(reader);
                conex.Close();
                conex.Dispose();
                return dtConjuntos;
                /*if (dtConjuntos.Rows.Count > 0)
                    return dtConjuntos;
                else
                    return new DataTable();*/
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                return new DataTable();
            }
        }
        
        public DataTable getInmueblesCommand(string idConjunto)
        {
            try
            {
                string sql = "SELECT P0701_ID_EDIFICIO, P0703_NOMBRE, P0728_DESC_GRAL FROM T07_EDIFICIO WHERE P0710_ID_CENTRO = ?";
                OdbcConnection conex = new OdbcConnection(GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString);
                OdbcCommand comando = new OdbcCommand(sql, conex);
                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = idConjunto;
                DataTable dtInmuebles = new DataTable();
                conex.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtInmuebles.Load(reader);
                reader.Close();
                conex.Close();
                if (dtInmuebles.Rows.Count > 0)
                    return dtInmuebles;
                else
                    return new DataTable();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                return new DataTable();
            }
        }
        public DataTable getDatosInmueblePorID(string idInmueble)
        {
            try
            {
                string sql = "SELECT P0728_DESC_GRAL FROM T07_EDIFICIO WHERE P0701_ID_EDIFICIO = ?";
                OdbcConnection conex = new OdbcConnection(GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString);
                OdbcCommand comando = new OdbcCommand(sql, conex);
                comando.Parameters.Add("@IDConjunto", OdbcType.VarChar).Value = idInmueble;
                DataTable dtInmuebles = new DataTable();
                conex.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtInmuebles.Load(reader);
                reader.Close();
                conex.Close();
                if (dtInmuebles.Rows.Count > 0)
                    return dtInmuebles;
                else
                    return new DataTable();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                return new DataTable();
            }
        }

        public DataTable getImagenesConjuntos(string idGpoEmpr, string idInmobi, string idConjunto)
        {
            string sql = string.Empty;
            if (idGpoEmpr == "Todos")
            {
                if (idInmobi == "Todos")
                {
                    sql = "SELECT T01_ARRENDADORA.P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, T09_IMAGENES.P0906_TEXTO_ESP, T09_IMAGENES.CAMPO1 FROM T01_ARRENDADORA " +
                    "JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1 " +
                    " JOIN T09_IMAGENES ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T09_IMAGENES.P0901_ID_ENTE WHERE P0902_TIPO_ENTE = 4";
                }
                else
                {
                    if (idConjunto == "Todos")
                    {
                        sql = "SELECT T01_ARRENDADORA.P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, T09_IMAGENES.P0906_TEXTO_ESP, T09_IMAGENES.CAMPO1 FROM T01_ARRENDADORA " +
                        "JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1 " +
                        " JOIN T09_IMAGENES ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T09_IMAGENES.P0901_ID_ENTE WHERE P0902_TIPO_ENTE = 4 AND T01_ARRENDADORA.P0101_ID_ARR = '" + idInmobi + "'";
                    }
                    else
                    {
                        sql = "SELECT P0906_TEXTO_ESP, CAMPO1 FROM T09_IMAGENES WHERE P0902_TIPO_ENTE = 4 AND P0901_ID_ENTE = '" + idConjunto + "'";
                    }
                }
            }
            else
            {
                if (idInmobi == "Todos")
                {
                    sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T01_ARRENDADORA.P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, " +
                    "T09_IMAGENES.P0906_TEXTO_ESP, T09_IMAGENES.CAMPO1 FROM T00_GRUPOS_EMP JOIN T01_ARRENDADORA ON T00_GRUPOS_EMP.P0001_ID_GRUPO" +
                    " = T01_ARRENDADORA.P0151_ID_GRUPO_EMP JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1" +
                    " JOIN T09_IMAGENES ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T09_IMAGENES.P0901_ID_ENTE WHERE P0902_TIPO_ENTE = 4 AND " +
                    "T00_GRUPOS_EMP.P0001_ID_GRUPO = '" + idGpoEmpr + "'";
                }
                else
                {
                    if (idConjunto == "Todos")
                    {
                        sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T01_ARRENDADORA.P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, " +
                        "T09_IMAGENES.P0906_TEXTO_ESP, T09_IMAGENES.CAMPO1 FROM T00_GRUPOS_EMP JOIN T01_ARRENDADORA ON T00_GRUPOS_EMP.P0001_ID_GRUPO" +
                        " = T01_ARRENDADORA.P0151_ID_GRUPO_EMP JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1" +
                        " JOIN T09_IMAGENES ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T09_IMAGENES.P0901_ID_ENTE WHERE P0902_TIPO_ENTE = 4 AND " +
                        "T00_GRUPOS_EMP.P0001_ID_GRUPO = '" + idGpoEmpr + "' AND T01_ARRENDADORA.P0101_ID_ARR = '" + idInmobi + "'";
                    }
                    else
                    {
                        sql = "SELECT P0906_TEXTO_ESP, CAMPO1 FROM T09_IMAGENES WHERE P0902_TIPO_ENTE = 4 AND P0901_ID_ENTE = '" + idConjunto + "'";
                    }
                }
            }
            return getDataTable(sql);
        }

        public DataTable getImagenesInmuebles(string idInmueble)
        {
            string sql = string.Empty;
            sql = "SELECT P0906_TEXTO_ESP, CAMPO1 FROM T09_IMAGENES WHERE P0902_TIPO_ENTE = 7 AND P0901_ID_ENTE = '" + idInmueble + "'";
            return getDataTable(sql);
        }
        public DataTable getImagenesInmuebles(string idInmueble, string idConjunto)
        {
            string sql = string.Empty;
            sql = "SELECT P0906_TEXTO_ESP, CAMPO1 FROM T09_IMAGENES WHERE P0902_TIPO_ENTE in(7,4) AND P0901_ID_ENTE = '" + idInmueble + "' OR P0901_ID_ENTE = '"+idConjunto+"'";
            return getDataTable(sql);
        }

        public List<string> getImagenesConjunto(string idConjunto)
        {
            string sql = "SELECT CAMPO1 FROM T09_IMAGENES WHERE P0902_TIPO_ENTE = 4 AND P0901_ID_ENTE = '" + idConjunto + "'";
            DataTable dtImgs = getDataTable(sql);
            List<string> listaImgs = new List<string>();
            foreach (DataRow row in dtImgs.Rows)
                listaImgs.Add(row[0].ToString().Trim().ToLower());
            return listaImgs;
        }
        public List<string> getImagenesInmueble(string idInmueble)
        {
            string sql = "SELECT CAMPO1 FROM T09_IMAGENES WHERE P0902_TIPO_ENTE = 7 AND P0901_ID_ENTE = '" + idInmueble + "'";
            DataTable dtImgs = getDataTable(sql);
            List<string> listaImgs = new List<string>();
            foreach (DataRow row in dtImgs.Rows)
                listaImgs.Add(row[0].ToString().Trim().ToLower());
            return listaImgs;
        }
        public DataTable getDatosConjuntos(string idGpoEmpr, string idInmobi, string idConjunto)
        {
            string sql = string.Empty;
            if (idGpoEmpr == "Todos")
            {
                if (idInmobi == "Todos")
                {
                    sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T01_ARRENDADORA.P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, " +
                        "T03_CENTRO_INDUSTRIAL.P0306_M2_TERRENO, T03_CENTRO_INDUSTRIAL.P0306_M2_CONSTRUCCION, T03_CENTRO_INDUSTRIAL.P0306_M2_TOTAL " +
                        " FROM T01_ARRENDADORA LEFT JOIN T00_GRUPOS_EMP ON T00_GRUPOS_EMP.P0001_ID_GRUPO" +
                        " = T01_ARRENDADORA.P0151_ID_GRUPO_EMP JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1";
                }
                else
                {
                    sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T01_ARRENDADORA.P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, " +
                        "T03_CENTRO_INDUSTRIAL.P0306_M2_TERRENO, T03_CENTRO_INDUSTRIAL.P0306_M2_CONSTRUCCION, T03_CENTRO_INDUSTRIAL.P0306_M2_TOTAL " +
                        "FROM T01_ARRENDADORA LEFT JOIN T00_GRUPOS_EMP ON T00_GRUPOS_EMP.P0001_ID_GRUPO" +
                        " = T01_ARRENDADORA.P0151_ID_GRUPO_EMP JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1" +
                        " WHERE " + " T01_ARRENDADORA.P0101_ID_ARR = '" + idInmobi + "'";
                }
            }
            else
            {
                if (idInmobi == "Todos")
                {
                    sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T01_ARRENDADORA.P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, " +
                        "T03_CENTRO_INDUSTRIAL.P0306_M2_TERRENO, T03_CENTRO_INDUSTRIAL.P0306_M2_CONSTRUCCION, T03_CENTRO_INDUSTRIAL.P0306_M2_TOTAL " +
                        " FROM T00_GRUPOS_EMP JOIN T01_ARRENDADORA ON T00_GRUPOS_EMP.P0001_ID_GRUPO" +
                        " = T01_ARRENDADORA.P0151_ID_GRUPO_EMP JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1" +
                        " WHERE " + "T00_GRUPOS_EMP.P0001_ID_GRUPO = '" + idGpoEmpr + "'";
                }
                else
                {
                    if (idConjunto == "Todos")
                    {
                        sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T01_ARRENDADORA.P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, " +
                            "T03_CENTRO_INDUSTRIAL.P0306_M2_TERRENO, T03_CENTRO_INDUSTRIAL.P0306_M2_CONSTRUCCION, T03_CENTRO_INDUSTRIAL.P0306_M2_TOTAL " +
                            "FROM T00_GRUPOS_EMP JOIN T01_ARRENDADORA ON T00_GRUPOS_EMP.P0001_ID_GRUPO" +
                            " = T01_ARRENDADORA.P0151_ID_GRUPO_EMP JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1" +
                            " WHERE " + "T00_GRUPOS_EMP.P0001_ID_GRUPO = '" + idGpoEmpr + "' AND T01_ARRENDADORA.P0101_ID_ARR = '" + idInmobi + "'";
                    }
                    /*else
                    {
                        sql = "SELECT P0906_TEXTO_ESP, CAMPO1 FROM T09_IMAGENES WHERE P0902_TIPO_ENTE = 4 AND P0901_ID_ENTE = '" + idConjunto + "'";
                    }*/
                }
            }
            return getDataTable(sql);
        }
        public DataTable getIdsConjuntos(string idGpoEmpr, string idInmobi, string idConjunto)
        {
            string sql = string.Empty;
            if (idGpoEmpr == "Todos")
            {
                if (idInmobi == "Todos")
                {
                    sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T01_ARRENDADORA.P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO " +
                    " FROM T01_ARRENDADORA LEFT JOIN T00_GRUPOS_EMP ON T00_GRUPOS_EMP.P0001_ID_GRUPO" +
                    " = T01_ARRENDADORA.P0151_ID_GRUPO_EMP JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1";
                }
                else
                {
                    sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T01_ARRENDADORA.P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO " +
                        "FROM T01_ARRENDADORA LEFT JOIN T00_GRUPOS_EMP ON T00_GRUPOS_EMP.P0001_ID_GRUPO" +
                        " = T01_ARRENDADORA.P0151_ID_GRUPO_EMP JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1" +
                        " WHERE " + " T01_ARRENDADORA.P0101_ID_ARR = '" + idInmobi + "'";
                }
            }
            else
            {
                if (idInmobi == "Todos")
                {
                    sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T01_ARRENDADORA.P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO " +
                    " FROM T00_GRUPOS_EMP JOIN T01_ARRENDADORA ON T00_GRUPOS_EMP.P0001_ID_GRUPO" +
                    " = T01_ARRENDADORA.P0151_ID_GRUPO_EMP JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1" +
                    " WHERE " + "T00_GRUPOS_EMP.P0001_ID_GRUPO = '" + idGpoEmpr + "'";
                }
                else
                {
                    if (idConjunto == "Todos")
                    {
                        sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T01_ARRENDADORA.P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO " +
                        "FROM T00_GRUPOS_EMP JOIN T01_ARRENDADORA ON T00_GRUPOS_EMP.P0001_ID_GRUPO" +
                        " = T01_ARRENDADORA.P0151_ID_GRUPO_EMP JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1" +
                        " WHERE " + "T00_GRUPOS_EMP.P0001_ID_GRUPO = '" + idGpoEmpr + "' AND T01_ARRENDADORA.P0101_ID_ARR = '" + idInmobi + "'";
                    }
                    /*else
                    {
                        sql = "SELECT P0906_TEXTO_ESP, CAMPO1 FROM T09_IMAGENES WHERE P0902_TIPO_ENTE = 4 AND P0901_ID_ENTE = '" + idConjunto + "'";
                    }*/
                }
            }
            return getDataTable(sql);
        }

        public DataTable getDtUsuario(string user)
        {
            string sql = string.Empty;
            sql = "SELECT USUARIO, CAMPO1, CAMPO2, CAMPO3, CAMPO4, CAMPO_NUM1 FROM GRUPOS WHERE USUARIO = '" + user + "'";
            return getDataTable(sql);
        }

        public DataTable getDtGrupoEmpresarial(string idGpoEmp)
        {
            string sql = string.Empty;
            if (idGpoEmp == "Todos")
            {
                sql = "SELECT P0001_ID_GRUPO, P0002_NOMBRE FROM T00_GRUPOS_EMP";
            }
            else
            {
                sql = "SELECT P0001_ID_GRUPO, P0002_NOMBRE FROM T00_GRUPOS_EMP WHERE P0001_ID_GRUPO = '" + idGpoEmp + "'";
            }
            return getDataTable(sql);
        }

        public DataTable getDtContratosPorGpoEmp(string idGpoEmp, DateTime vigencia)
        {
            string fecha = vigencia.Month + "/" + vigencia.Day + "/" + vigencia.Year;
            string sql = string.Empty;
            sql = "SELECT T00_GRUPOS_EMP.P0001_ID_GRUPO, T01_ARRENDADORA.P0101_ID_ARR, T01_ARRENDADORA.P0103_RAZON_SOCIAL, T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.P0407_MONEDA_FACT, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0404_ID_EDIFICIO, T04_CONTRATO.P0434_IMPORTE_ACTUAL, T19_DESC_GRAL.P1922_A_MIN_ING, T19_DESC_GRAL.P1926_CIST_ING, T19_DESC_GRAL.CAMPO_NUM1 " +
                " FROM T00_GRUPOS_EMP JOIN T01_ARRENDADORA ON T00_GRUPOS_EMP.P0001_ID_GRUPO = T01_ARRENDADORA.P0151_ID_GRUPO_EMP" +
                " JOIN T04_CONTRATO ON T01_ARRENDADORA.P0101_ID_ARR = T04_CONTRATO.P0403_ID_ARRENDADORA " +
                " JOIN T07_EDIFICIO ON T07_EDIFICIO.P0730_SUBCONJUNTO = T04_CONTRATO.P0404_ID_EDIFICIO" +
                " JOIN T19_DESC_GRAL ON T07_EDIFICIO.P0701_ID_EDIFICIO = T19_DESC_GRAL.P1901_ID_ENTE " + 
                " WHERE T00_GRUPOS_EMP.P0001_ID_GRUPO = '" + idGpoEmp + "' AND P0411_FIN_VIG >= '" + fecha + "' AND P0410_INICIO_VIG <= '" + fecha + "'";
            return getDataTable(sql);
        }

        public DataTable getDtSubConjuntosRentadosPorGpoEmp(string idGpoEmp)
        {
            string sql = string.Empty;
            sql = "SELECT P0001_ID_GRUPO, T01_ARRENDADORA.P0101_ID_ARR, T01_ARRENDADORA.P0103_RAZON_SOCIAL, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T18_SUBCONJUNTOS.P18_CAMPO3, T18_SUBCONJUNTOS.P18_CAMPO2 " +
                "FROM T00_GRUPOS_EMP JOIN T01_ARRENDADORA ON T00_GRUPOS_EMP.P0001_ID_GRUPO = T01_ARRENDADORA.P0151_ID_GRUPO_EMP " +
                "JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.CAMPO1 = T01_ARRENDADORA.P0101_ID_ARR " +
                "JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO " +
                "WHERE T00_GRUPOS_EMP.P0001_ID_GRUPO = '" + idGpoEmp + "' AND T18_SUBCONJUNTOS.P18_CAMPO3 = 'N'";
            return getDataTable(sql);
        }

        public DataTable getDtSubConjuntosNoRentadosPorGpoEmp(string idGpoEmp)
        {
            string sql = string.Empty;
            if (idGpoEmp != "Todos")
            {
                sql = "SELECT T01_ARRENDADORA.P0101_ID_ARR, T01_ARRENDADORA.P0103_RAZON_SOCIAL, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T18_SUBCONJUNTOS.P18_CAMPO1, T18_SUBCONJUNTOS.P18_CAMPO3, T18_SUBCONJUNTOS.P18_CAMPO2, T18_SUBCONJUNTOS.P18CAMPO_NUM1, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA " +
                    "FROM T01_ARRENDADORA " +
                    "JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.CAMPO1 = T01_ARRENDADORA.P0101_ID_ARR " +
                    "JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO " +
                    "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                    "WHERE T01_ARRENDADORA.P0151_ID_GRUPO_EMP = '" + idGpoEmp + "' AND (T18_SUBCONJUNTOS.P18_CAMPO3 = 'S' OR T18_SUBCONJUNTOS.P18_CAMPO3 IS NULL)";
            }
            else
            {
                sql = "SELECT T01_ARRENDADORA.P0101_ID_ARR, T01_ARRENDADORA.P0103_RAZON_SOCIAL, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, T03_CENTRO_INDUSTRIAL.P0303_NOMBRE, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T18_SUBCONJUNTOS.P18_CAMPO1, T18_SUBCONJUNTOS.P18_CAMPO3, T18_SUBCONJUNTOS.P18_CAMPO2, T18_SUBCONJUNTOS.P18CAMPO_NUM1, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA " +
                    "FROM T01_ARRENDADORA " +
                    "JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.CAMPO1 = T01_ARRENDADORA.P0101_ID_ARR " +
                    "JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO " +
                    "JOIN T05_DOMICILIO ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T05_DOMICILIO.P0500_ID_ENTE " +
                    "WHERE (T18_SUBCONJUNTOS.P18_CAMPO3 = 'S' OR T18_SUBCONJUNTOS.P18_CAMPO3 IS NULL)";
            }
            return getDataTable(sql);
        }

        public DataTable getDtInmueblesPorSubConjuntosNoRentadosPorGpoEmp(string idGpoEmp)
        {
            string sql = string.Empty;
            sql = "SELECT P0001_ID_GRUPO, T01_ARRENDADORA.P0101_ID_ARR, T01_ARRENDADORA.P0103_RAZON_SOCIAL, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T18_SUBCONJUNTOS.P1803_NOMBRE, T18_SUBCONJUNTOS.P18_CAMPO3, T18_SUBCONJUNTOS.P18_CAMPO2, T07_EDIFICIO.P0701_ID_EDIFICIO, T19_DESC_GRAL.P1901_ID_ENTE, T19_DESC_GRAL.P1922_A_MIN_ING, T19_DESC_GRAL.P1926_CIST_ING, T19_DESC_GRAL.CAMPO_NUM1, T18_SUBCONJUNTOS.P18CAMPO_NUM1 " +
                "FROM T00_GRUPOS_EMP JOIN T01_ARRENDADORA ON T00_GRUPOS_EMP.P0001_ID_GRUPO = T01_ARRENDADORA.P0151_ID_GRUPO_EMP " +
                "JOIN T03_CENTRO_INDUSTRIAL ON T03_CENTRO_INDUSTRIAL.CAMPO1 = T01_ARRENDADORA.P0101_ID_ARR " +
                "JOIN T18_SUBCONJUNTOS ON T18_SUBCONJUNTOS.P1804_ID_CONJUNTO = T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO " +
                "JOIN T07_EDIFICIO ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T07_EDIFICIO.P0730_SUBCONJUNTO " +
                "JOIN T19_DESC_GRAL ON T07_EDIFICIO.P0701_ID_EDIFICIO = T19_DESC_GRAL.P1901_ID_ENTE " +
                "WHERE T00_GRUPOS_EMP.P0001_ID_GRUPO = '" + idGpoEmp + "' AND (T18_SUBCONJUNTOS.P18_CAMPO3 = 'S' OR T18_SUBCONJUNTOS.P18_CAMPO3 IS NULL)";
            return getDataTable(sql);
        }

        public DataTable getDtInmueblesPorSubConj(string idSubConj)
        {
            string sql = "SELECT P0701_ID_EDIFICIO, P0706_TERRENO_M2, P0707_CONTRUCCION_M2, P0708_PREDIAL, T05_DOMICILIO.P0503_CALLE_NUM, T05_DOMICILIO.P0504_COLONIA, T05_DOMICILIO.P0506_CIUDAD, T05_DOMICILIO.P0507_ESTADO, T19_DESC_GRAL.P1922_A_MIN_ING, T19_DESC_GRAL.P1926_CIST_ING, T19_DESC_GRAL.P1916_A_TOTAL_ING, T19_DESC_GRAL.CAMPO_NUM1 FROM T07_EDIFICIO " +
                "JOIN T05_DOMICILIO ON T05_DOMICILIO.P0500_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO " +
                "JOIN T19_DESC_GRAL ON T19_DESC_GRAL.P1901_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO " +
                "WHERE P0730_SUBCONJUNTO = '" + idSubConj + "' AND T05_DOMICILIO.P0502_TIPO_DOMICILIO = 1";
            return getDataTable(sql);
        }

        public DataTable getDtGastosNombres()
        {
            string sql = "SELECT DESCR_CAT FROM CATEGORIA WHERE ID_COT = 'CLASMANTEN' AND DESCR_CAT IS NOT NULL";
            return getDataTable(sql);
        }

        public DataTable getDtGastosDeConjuntosDeGpoEmp(string grupo, string gasto)
        {
            string sql = string.Empty;
            if (grupo != "Todos")
            {
                sql = "SELECT P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, T36_SEGUIMIENTO.P3601_ID_CTE, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM4 FROM T01_ARRENDADORA " +
                    "JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1 " +
                    "JOIN T36_SEGUIMIENTO ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T36_SEGUIMIENTO.P3601_ID_CTE " +
                    "WHERE P0151_ID_GRUPO_EMP = '"+ grupo +"' AND T36_SEGUIMIENTO.CAMPO5 = '" + gasto + "'";
            }
            else
            {
                sql = "SELECT P0101_ID_ARR, T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO, T36_SEGUIMIENTO.P3601_ID_CTE, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM4 FROM T01_ARRENDADORA " +
                    "JOIN T03_CENTRO_INDUSTRIAL ON T01_ARRENDADORA.P0101_ID_ARR = T03_CENTRO_INDUSTRIAL.CAMPO1 " +
                    "JOIN T36_SEGUIMIENTO ON T03_CENTRO_INDUSTRIAL.P0301_ID_CENTRO = T36_SEGUIMIENTO.P3601_ID_CTE " +
                    "WHERE T36_SEGUIMIENTO.CAMPO5 = '"+ gasto +"'";
            }
            return getDataTable(sql);
        }

        public DataTable getDtGastosDeInmueblesDeGpoEmp(string grupo, string gasto)
        {
            string sql = string.Empty;
            if (grupo != "Todos")
            {
                sql = "SELECT P0101_ID_ARR, T07_EDIFICIO.P0701_ID_EDIFICIO, T36_SEGUIMIENTO.P3601_ID_CTE, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM4 FROM T01_ARRENDADORA " +
                    "JOIN T07_EDIFICIO ON T01_ARRENDADORA.P0101_ID_ARR = T07_EDIFICIO.P0709_ARRENDADORA " +
                    "JOIN T36_SEGUIMIENTO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T36_SEGUIMIENTO.P3601_ID_CTE " +
                    "WHERE P0151_ID_GRUPO_EMP = '" + grupo + "' AND T36_SEGUIMIENTO.CAMPO5 = '" + gasto + "'";
            }
            else
            {
                sql = "SELECT P0101_ID_ARR, T07_EDIFICIO.P0701_ID_EDIFICIO, T36_SEGUIMIENTO.P3601_ID_CTE, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM4 FROM T01_ARRENDADORA " +
                    "JOIN T07_EDIFICIO ON T01_ARRENDADORA.P0101_ID_ARR = T07_EDIFICIO.P0709_ARRENDADORA " +
                    "JOIN T36_SEGUIMIENTO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T36_SEGUIMIENTO.P3601_ID_CTE " +
                    "WHERE T36_SEGUIMIENTO.CAMPO5 = '" + gasto + "'";
            }
            return getDataTable(sql);
        }

        public DataTable getDtGastosDeInmueblesPorSubConj(string subconjunto, string gasto)
        {
            string sql = "SELECT T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO, T07_EDIFICIO.P0701_ID_EDIFICIO, T36_SEGUIMIENTO.P3601_ID_CTE, T36_SEGUIMIENTO.CAMPO3, T36_SEGUIMIENTO.CAMPO_NUM4 FROM T18_SUBCONJUNTOS " +
                "JOIN T07_EDIFICIO ON T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = T07_EDIFICIO.P0730_SUBCONJUNTO " +
                "JOIN T36_SEGUIMIENTO ON T07_EDIFICIO.P0701_ID_EDIFICIO = T36_SEGUIMIENTO.P3601_ID_CTE " +
                "WHERE T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = '" + subconjunto + "' AND T36_SEGUIMIENTO.CAMPO5 = '" + gasto + "'";
            return getDataTable(sql);
        }

        public DataTable getDTPredialesPorGpoEmp(string grupo)
        {
            string sql = string.Empty;
            if (grupo == "Todos")
            {
                sql = "SELECT P0101_ID_ARR, T07_EDIFICIO.P0701_ID_EDIFICIO, T19_DESC_GRAL.P1901_ID_ENTE, T19_DESC_GRAL.CAMPO_NUM2 FROM T01_ARRENDADORA " +
                    "JOIN T07_EDIFICIO ON T07_EDIFICIO.P0709_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                    "JOIN T19_DESC_GRAL ON T19_DESC_GRAL.P1901_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO ";
            }
            else
            {
                sql = "SELECT P0101_ID_ARR, T07_EDIFICIO.P0701_ID_EDIFICIO, T19_DESC_GRAL.P1901_ID_ENTE, T19_DESC_GRAL.CAMPO_NUM2 FROM T01_ARRENDADORA " +
                    "JOIN T07_EDIFICIO ON T07_EDIFICIO.P0709_ARRENDADORA = T01_ARRENDADORA.P0101_ID_ARR " +
                    "JOIN T19_DESC_GRAL ON T19_DESC_GRAL.P1901_ID_ENTE = T07_EDIFICIO.P0701_ID_EDIFICIO " +
                    "WHERE T01_ARRENDADORA.P0151_ID_GRUPO_EMP = '" + grupo + "'";
            }
            return getDataTable(sql);
        }

        public DataTable getDtPredialesPorSubConjunto(string subconj)
        {
            string sql = "SELECT P1801_ID_SUBCONJUNTO, T07_EDIFICIO.P0701_ID_EDIFICIO, T19_DESC_GRAL.P1901_ID_ENTE, T19_DESC_GRAL.CAMPO_NUM2 FROM T18_SUBCONJUNTOS " +
                "JOIN T07_EDIFICIO ON T07_EDIFICIO.P0730_SUBCONJUNTO = T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO " +
                "JOIN T19_DESC_GRAL ON T07_EDIFICIO.P0701_ID_EDIFICIO = T19_DESC_GRAL.P1901_ID_ENTE " +
                "WHERE T18_SUBCONJUNTOS.P1801_ID_SUBCONJUNTO = '" + subconj + "'";
            return getDataTable(sql);
        }

        public DataTable getDtContratosPorSubconj(string idSub, int anioVig, bool esPesos)
        {
            string sql = string.Empty;
            if (esPesos)
            {
                sql = "SELECT T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.P0404_ID_EDIFICIO, T02_ARRENDATARIO.P0203_NOMBRE, P0407_MONEDA_FACT, T04_CONTRATO.P0408_IMPORTE_FACT, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG FROM T04_CONTRATO " +
                    "JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T04_CONTRATO.P0402_ID_ARRENDAT " +
                    "WHERE P0404_ID_EDIFICIO = '" + idSub + "' AND T04_CONTRATO.P0410_INICIO_VIG <= '12/31/" + anioVig + "' AND T04_CONTRATO.P0411_FIN_VIG >= '01/01/" + anioVig + "' AND P0407_MONEDA_FACT = 'P' ";
            }
            else
            {
                sql = "SELECT T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.P0404_ID_EDIFICIO, T02_ARRENDATARIO.P0203_NOMBRE, P0407_MONEDA_FACT, T04_CONTRATO.P0408_IMPORTE_FACT, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG FROM T04_CONTRATO " +
                    "JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T04_CONTRATO.P0402_ID_ARRENDAT " +
                    "WHERE P0404_ID_EDIFICIO = '" + idSub + "' AND T04_CONTRATO.P0410_INICIO_VIG <= '12/31/" + anioVig + "' AND T04_CONTRATO.P0411_FIN_VIG >= '01/01/" + anioVig + "' AND P0407_MONEDA_FACT = 'D' ";
            }
            return getDataTable(sql);
        }

        public DataTable getDtContratosHistPorIdContrato(string idContrato, DateTime vigencia)
        {
            string sql = string.Empty;
            sql = "SELECT T04_CONTRATO.P0401_ID_CONTRATO, T59_HISTORIA_CONTRATOS.P5941_IMP_ACTUAL_NVO, T59_HISTORIA_CONTRATOS.P5976_FECHA_HORA_UPDT, T59_HISTORIA_CONTRATOS.P5911_INICIO_VIG_NVO, T59_HISTORIA_CONTRATOS.P5913_FIN_VIG_NVO, T04_CONTRATO.P0407_MONEDA_FACT, " +
                "T04_CONTRATO.P0434_IMPORTE_ACTUAL, T04_CONTRATO.P0410_INICIO_VIG, T04_CONTRATO.P0411_FIN_VIG, T04_CONTRATO.P0441_BASE_PARA_AUMENTO, T04_CONTRATO.P0415_AUMENTO_ANUAL, T04_CONTRATO.P0432_FECHA_AUMENTO FROM T04_CONTRATO " +
                  "JOIN T59_HISTORIA_CONTRATOS ON T04_CONTRATO.P0401_ID_CONTRATO = T59_HISTORIA_CONTRATOS.P5901_ID_CONTRATO " +
                  "WHERE T04_CONTRATO.P0410_INICIO_VIG <= '12/31/" + vigencia.Year + "' AND T04_CONTRATO.P0411_FIN_VIG >= '01/01/" + vigencia.Year + "' AND T04_CONTRATO.P0401_ID_CONTRATO = '" + idContrato + "' " +
                  "ORDER BY T59_HISTORIA_CONTRATOS.P5976_FECHA_HORA_UPDT DESC";
           
            return getDataTable(sql);
        }

        public DataTable getDataTable(string sql)
        {
            try
            {
                OdbcConnection conex = new OdbcConnection(GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString);
                OdbcCommand comando = new OdbcCommand(sql, conex);
                comando.CommandTimeout = int.MaxValue;
                DataTable dtImgs = new DataTable();
                conex.Open();
                OdbcDataReader reader = comando.ExecuteReader();
                dtImgs.Load(reader);
                conex.Close();
                reader.Close();
                if (dtImgs.Rows.Count > 0)
                    return dtImgs;
                else
                    return new DataTable();
            }
            catch //(Exception ex)
            {
                return new DataTable();
            }
        }

        #endregion

        #region Saldo venta
        public string getNombreArrendadoraPorID(string idArr)
        {
            DataTable dtResult = getDataTable("SELECT P0102_N_COMERCIAL FROM T01_ARRENDADORA WHERE P0101_ID_ARR = '" + idArr + "'");
            if (dtResult.Rows.Count > 0)
                return dtResult.Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public List<EdificiosYContratos> getEdificiosYContratos(string idArr, bool incluirPreventa)
        {
            string sql = @"SELECT T07_EDIFICIO.P0701_ID_EDIFICIO, T07_EDIFICIO.CAMPO5, T07_EDIFICIO.CAMPO6, T07_EDIFICIO.P0706_TERRENO_M2, T07_EDIFICIO.P0712_IMPORTE_RENTA, T07_EDIFICIO.P0704_TIPO_ED, T07_EDIFICIO.P0726_CLAVE, T04_CONTRATO.P0401_ID_CONTRATO, T04_CONTRATO.P0418_IMPORTE_DEPOSITO, T04_CONTRATO.P0437_TIPO, T02_ARRENDATARIO.P0203_NOMBRE,T04_CONTRATO.P0428_A_PRORROGA
                            FROM T07_EDIFICIO
                            LEFT JOIN T04_CONTRATO ON T04_CONTRATO.P0404_ID_EDIFICIO = T07_EDIFICIO.P0701_ID_EDIFICIO
                            LEFT JOIN T02_ARRENDATARIO ON T02_ARRENDATARIO.P0201_ID = T04_CONTRATO.P0402_ID_ARRENDAT
                            WHERE P0709_ARRENDADORA = ? AND ";//  (P0437_TIPO IN ('V','W') OR P0437_TIPO IS NULL) ";
            if (incluirPreventa)
                sql += " ((P0437_TIPO IN ('V','W') OR P0437_TIPO IS NULL) OR (P0437_TIPO IN('P', 'Q')))";
            else
                sql += " (P0437_TIPO IN ('V','W') OR P0437_TIPO IS NULL) ";
            
            OdbcConnection conexion = new OdbcConnection(GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString);
            OdbcCommand comando = new OdbcCommand(sql, conexion);
            comando.Parameters.AddWithValue("@IDArrendadora", idArr);
            List<EdificiosYContratos> lista = new List<EdificiosYContratos>();
            OdbcDataReader reader;
            try
            {
                conexion.Open();
                reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    EdificiosYContratos registro = new EdificiosYContratos();
                    EdificioEntity edificio = new EdificioEntity();
                    edificio.ID = reader["P0701_ID_EDIFICIO"] == DBNull.Value ? string.Empty : (string)reader["P0701_ID_EDIFICIO"];
                    edificio.Lote = reader["CAMPO6"] == DBNull.Value ? string.Empty : (string)reader["CAMPO6"];
                    edificio.Manzana = reader["CAMPO5"] == DBNull.Value ? string.Empty : (string)reader["CAMPO5"];
                    edificio.Terreno = reader["P0706_TERRENO_M2"] == DBNull.Value ? 0 : (decimal)reader["P0706_TERRENO_M2"];
                    edificio.Valor = reader["P0712_IMPORTE_RENTA"] == DBNull.Value ? 0 : (decimal)reader["P0712_IMPORTE_RENTA"];
                    edificio.Tipo = reader["P0704_TIPO_ED"] == DBNull.Value ? string.Empty : (string)reader["P0704_TIPO_ED"];
                    edificio.Identificador = reader["P0726_CLAVE"] == DBNull.Value ? string.Empty : (string)reader["P0726_CLAVE"];
                    registro.Edificio = edificio;
                    ContratoEntity contrato = new ContratoEntity();
                    contrato.ID = reader["P0401_ID_CONTRATO"] == DBNull.Value ? string.Empty : (string)reader["P0401_ID_CONTRATO"];
                    contrato.Enganche = reader["P0418_IMPORTE_DEPOSITO"] == DBNull.Value ? 0 : (decimal)reader["P0418_IMPORTE_DEPOSITO"];
                    contrato.NombreCliente = reader["P0203_NOMBRE"] == DBNull.Value ? string.Empty : (string)reader["P0203_NOMBRE"];
                    contrato.Tipo = reader["P0437_TIPO"] == DBNull.Value ? string.Empty : (string)reader["P0437_TIPO"];
                    contrato.EstatusContrato = reader["P0428_A_PRORROGA"] == DBNull.Value ? string.Empty : (string)reader["P0428_A_PRORROGA"];
                    if (contrato.Tipo == "V")
                        contrato.EstatusContrato = "Contrato de Venta";
                    else if (contrato.Tipo == "W")
                        contrato.EstatusContrato = "Vendido";                   
                    else if (contrato.Tipo == "P")
                        contrato.EstatusContrato = "Contrato de Preventa";
                    else if (contrato.Tipo == "Q")
                        contrato.EstatusContrato = "Preventa Terminado";
                    else if(string.IsNullOrEmpty(contrato.Tipo))
                        contrato.EstatusContrato = "Disponible";


                    registro.Contrato = contrato;
                    lista.Add(registro);
                }
                reader.Close();
                conexion.Close();
                return lista;
            }
            catch
            {
                conexion.Close();
                return null;                
            }
        }

        public decimal getPagosPorIDContratoYFecha(string idCnt, DateTime fecha)
        {
            string sql = @"SELECT SUM(P2405_IMPORTE) AS IMPORTE 
                            FROM T24_HISTORIA_RECIBOS
                            WHERE P2406_STATUS = 2 AND P2418_ID_CONTRATO = ? AND P2408_FECHA_PAGADO <= ? AND (CAMPO11 is null or CAMPO11 <> 'E')
                            GROUP BY P2418_ID_CONTRATO";
            OdbcConnection conexion = new OdbcConnection(Properties.Settings.Default.SaariODBC_ConnectionString);
            OdbcCommand comando = new OdbcCommand(sql, conexion);
            comando.Parameters.AddWithValue("@IDCnt", idCnt);
            comando.Parameters.AddWithValue("@Fecha", fecha);
            try
            {
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
        #endregion
    }
}
