using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.DataAccessLayer;
using FastReport;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using Microsoft.CSharp;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    class AnalisisDeudores
    {
        string InmobiliariaID = string.Empty;
        string ConjuntoID = string.Empty;
        string InmobiliariaNombre = string.Empty;
        string nameConjunto = string.Empty;
        DateTime FechaCorte = DateTime.Now;
        bool esPDF = false;
        DateTime fechaInicioPer = DateTime.Now;
        DateTime fechaFinPer = DateTime.Now;
        BackgroundWorker bgWorker;
        bool usarSinComplementos = false;


        public AnalisisDeudores(string idInmo, string nombreInmom, string nombreConjunto, string idConjunto, DateTime fechaCorte, bool espdf, BackgroundWorker bk)
        {
            this.InmobiliariaID = idInmo;
            this.InmobiliariaNombre = nombreInmom;
            this.ConjuntoID = idConjunto;
            this.FechaCorte = fechaCorte;
            this.esPDF = espdf;
            this.bgWorker = bk;
            this.nameConjunto = nombreConjunto;
        }

        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();
        }

        public static List<ConjuntoEntity> obtenerConjunto(string IdInmobiliaria)
        {
            return SaariDB.getConjuntos(IdInmobiliaria);
        }
        public string generarReporte()
        {
            string error = string.Empty;
            #region Variables
            int dia =1;
            #endregion

            #region OBTENER DATOS
            fechaInicioPer = new DateTime(FechaCorte.Year, FechaCorte.Month, dia);
            dia = DateTime.DaysInMonth(FechaCorte.Year, FechaCorte.Month);
            fechaFinPer = new DateTime(FechaCorte.Year, FechaCorte.Month, dia);
            List<SubtipoEntity> ListTipoCont = SaariDB.getSubTiposOI();
            List<SubtipoEntity> ListTipoContAux = new List<SubtipoEntity>();
            bgWorker.ReportProgress(5);
            //var ListaContratos = SaariDB.getContratosAnalisisDeudores(InmobiliariaID, ConjuntoID, FechaCorte, fechaInicioPer, fechaFinPer);

            #endregion
            return error;
        }

        private ReciboEntity ConvertirDolaresAPesos(ReciboEntity Recibo, string monedaContrato)
        {
            ReciboEntity conversion = Recibo;
            conversion.Importe = Recibo.Importe * Recibo.TipoCambio;
            conversion.Total = Recibo.Total * Recibo.TipoCambio;
            conversion.IVA = Recibo.IVA * Recibo.TipoCambio;
            conversion.IVARetenido = Recibo.IVARetenido * Recibo.TipoCambio;
            conversion.ISR = Recibo.ISR * Recibo.TipoCambio;
            conversion.Descuento = Recibo.Descuento * Recibo.TipoCambio;
            conversion.Cargo = Recibo.Cargo * Recibo.TipoCambio;
            return conversion;

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
        public string MonthName(int month)
        {
            DateTimeFormatInfo dtInfo = new CultureInfo("es-MX", false).DateTimeFormat;
            return dtInfo.GetMonthName(month);
        }

        public static int GetExcelColumnNumber(string ColumnName)
        {
            int columnNumber = 0;
            for (int i = 0; i < ColumnName.Length; i++)
            {
                columnNumber = columnNumber * 26 + (Convert.ToInt32(ColumnName[i]) - 64);
            }
            return columnNumber;
        }
    }
}
