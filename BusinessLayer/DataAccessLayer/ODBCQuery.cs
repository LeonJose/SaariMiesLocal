using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace GestorReportes.BusinessLayer.DataAccessLayer
{
    class ODBCQuery
    {

        /// <summary>
        /// Ejecutar cualquier consulta a ODBC Intersolv Saari - Arrendadora
        /// </summary>
        /// <param name="sqlText">Cadena que contiene Consulta para DB Arrendarora (IB-FB)</param>
        /// <returns>Registros de acuerdo a consulta ejecutada</returns>
        public DataTable DTArrendadora(string sqlText)
        {
            DataTable dt = new DataTable();
            OdbcDataReader dr = null;
            using (OdbcConnection odbcConn = new OdbcConnection(GestorReportes.Properties.Settings.Default.SaariODBC_ConnectionString))
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
            return dt;
        }

    }
}
