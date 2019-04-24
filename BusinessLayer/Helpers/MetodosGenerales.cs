using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.Helpers
{
    class MetodosGenerales
    {
        /// <summary>
        /// Obtiene el nombre de la columna, a traves del numero de la columna, Ej. 1 = A, 2 = B, etc...
        /// </summary>
        /// <param name="ColumnNumber"> Numero de columna   </param>
        /// <returns></returns>
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

        /// <summary>
        ///  Retorna el nombre del mes.
        /// </summary>
        /// <param name="month">valor numerico del mes</param>
        /// <returns></returns>
        public string MonthName(int month)
        {
            DateTimeFormatInfo dtInfo = new CultureInfo("es-MX", false).DateTimeFormat;
            return dtInfo.GetMonthName(month);
        }

        /// <summary>
        /// Obtiene el numero de columna, a traves del nombre de la columna, Ej. A = 1, B = 2, etc...
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public static int GetExcelColumnNumber(string ColumnName)
        {
            int columnNumber = 0;
            for (int i = 0; i < ColumnName.Length; i++)
            {
                columnNumber = columnNumber * 26 + (Convert.ToInt32(ColumnName[i]) - 64);
            }
            return columnNumber;
        }

        /// <summary>
        /// Se utiliza para librerar recursos, cuando se genera un Excel en codigo.
        /// </summary>
        /// <param name="obj"></param>
        public void releaseObject(object obj)
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
    }
}

