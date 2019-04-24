using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesReportes;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Drawing;
using System.Data.Odbc;

namespace GestorReportes.BusinessLayer.DataAccessLayer
{
    public static class SaariE
    {
        public static string Mensaje=string.Empty;

        public static string MsgError = string.Empty;

        public static string getUUIDByIDCfd(int idCfd)
        {
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            string sql = @"SELECT UUID FROM cfd.CFD WHERE ID_CFD = @IDCfd";
            try
            {
                string uuid = string.Empty;
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@IDCfd", System.Data.SqlDbType.Int).Value = idCfd;
                conexion.Open();
                uuid = comando.ExecuteScalar().ToString();
                conexion.Close();
                return uuid;
            }
            catch
            {
                conexion.Close();
                return string.Empty;
            }
        }

        public static List<ReciboEntity> getListaRecibosEnProcesoCancelacion(string idInmobiliaria, DateTime fechaInicio, DateTime fechaFin)
        {
            List<ReciboEntity> listaRecibosEnProceso = new List<ReciboEntity>();            
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            string sql = @"SELECT ID_CFD, UUID FROM cfd.CancelacionProcesada WHERE FechaAcuse BETWEEN @FechaInicio AND @FechaFin";
            try
            {   
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@FechaInicio", System.Data.SqlDbType.DateTime).Value = fechaInicio;
                comando.Parameters.Add("@FechaFin", System.Data.SqlDbType.DateTime).Value = fechaFin;
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    int idCfd = (int)reader["ID_CFD"];
                    ReciboEntity recibo = SaariDB.getDetalleReciboPorCancelar(idInmobiliaria, idCfd);                    
                    if (recibo != null)
                    {
                        recibo.UUID = reader["UUID"].ToString();
                        recibo.Estatus = "EN PROCESO";
                        listaRecibosEnProceso.Add(recibo);
                    }
                }
                reader.Close();
                conexion.Close();
                return listaRecibosEnProceso;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<DocumentosRelacionados> GetDocumentosRelacionados( bool CRPs, DateTime fechaInicio, DateTime fechaFin)
        {

            var list = new List<DocumentosRelacionados>();

            //            string sql = @"select
            //UUID,
            //Serie,
            //Folio,
            //Moneda,
            //NumParcialidad,
            //ImpSaldoAnt,
            //impPagado,
            //ImpSaldoInsoluto,
            //SAT.FormaDePago.Descripcion,
            //IdComprobante
            //from cfd.DocumentosRelacionados
            //JOIN  cfd.CFD ON cfd.ID_CFD = DocumentosRelacionados.ID_CFD
            //JOIN SAT.FormaDePago ON cfd.MetodoDePago = SAT.FormaDePago.ClaveFormaDePago
            //where DocumentosRelacionados.IdComprobante = @IdComprobante";
            string sql = string.Empty;

            if (CRPs)
            {
                #region sql_DocRelacionadosComprobantesPago

                sql = @"Select t1.ID_CFD, 
t1.NumParcialidad,
t1.ImpPagado, 
t1.ImpSaldoAnt, 
t1.ImpSaldoInsoluto,
t1.IdComprobante, 
t2.Id_Hist_Rec, 
t2.Serie, 
t2.Folio, 
t2.Total, 
t2.UUID, 
t2.Id_Hist_Rec,
t2.FechaTimbrado,
t3.IDPago, 
t3.IDHistRec,
t4.Folio,
t4.Total, 
t4.IDPago,
t4.ID_XML,
t4.Moneda,
t4.Serie,
t4.Fecha, 
t6.FechaPago,
SAT.FormaDePago.Descripcion 
from cfd.DocumentosRelacionados AS t1 
JOIN  cfd.CFD AS t2 ON t2.ID_CFD = t1.ID_CFD 
JOIN Cobranza.ReciboPagado AS t3 on t3.IDHistRec = t2.Id_Hist_Rec 
JOIN cfd.ComprobantePago AS t4 on t4.IDPago = t3.IDPago 
JOIN SAT.FormaDePago ON t4.FormaPago = SAT.FormaDePago.ClaveFormaDePago 
JOIN cfd.ComplementoPago AS t6 ON t6.ID_ComprobantePago = t4.ID_ComprobantePago 
WHERE t6.FechaPago >= @fechaIni AND t6.FechaPago <= @fechaFin  ";
                #endregion
            }
            else
            {
                #region sql_DocRelacionadosCFDis

                sql = @"SELECT t3.Serie AS Serie_CRP, t3.Folio AS Folio_CRP, t4.ID_ComprobantePago AS IdComprobante, t3.Fecha AS Fecha_Timbrado_CRP, t4.FechaPago AS Fecha_Pago_CRP, 
t3.Total AS Total_CRP, t1.Serie AS Serie_FACTURA, t1.Folio AS Folio_FACTURA, t1.Total AS Total_FACTURA, t1.Id_Hist_Rec, t1.ID_CFD 
FROM cfd.CFD AS t1 
JOIN Cobranza.ReciboPagado AS t2 ON t2.IDHistRec=t1.Id_Hist_Rec 
JOIN cfd.ComprobantePago AS t3 ON t3.IDPago=t2.IDPago 
JOIN cfd.ComplementoPago AS t4 ON t4.ID_ComprobantePago=t3.ID_ComprobantePago 
WHERE t4.FechaPago >= @fechaIni AND t4.FechaPago <= @fechaFin  
ORDER BY Serie_CRP, Folio_CRP, IdComprobante ";

                #endregion
            }

            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            try
            {
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@fechaIni", System.Data.SqlDbType.DateTime).Value = fechaInicio;
                comando.Parameters.Add("@fechaFin", System.Data.SqlDbType.DateTime).Value = fechaFin;
                //if (CRPs)
                //{
                //    comando.Parameters.Add("@IdComprobante", System.Data.SqlDbType.Int).Value = IdComp;
                //}
                //else
                //{
                //    comando.Parameters.Add("@IdComprobante", System.Data.SqlDbType.Int).Value = IdComp;
                //}

                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    DocumentosRelacionados DR = new DocumentosRelacionados();
                    if (CRPs)
                    {
                        DR.UUID = reader["UUID"].ToString();
                        DR.Serie = reader["Serie"].ToString();
                        DR.Folio = Convert.ToInt32(reader["Folio"]);
                        DR.Moneda = reader["Moneda"].ToString();
                        DR.NumParcialidad = Convert.ToInt32(reader["NumParcialidad"]);
                        DR.ImpSaldoAnt = Convert.ToDecimal(reader["ImpSaldoAnt"].ToString());
                        DR.impPagado = Convert.ToDecimal(reader["impPagado"].ToString());
                        DR.ImpSaldoInsoluto = Convert.ToDecimal(reader["ImpSaldoInsoluto"].ToString());
                        DR.Descripcion = reader["Descripcion"].ToString();
                        DR.IdComprobante = Convert.ToInt32(reader["IdComprobante"]);
                        DR.Fecha = Convert.ToDateTime(reader["Fecha"].ToString());
                        DR.FechaPago = Convert.ToDateTime(reader["FechaPago"].ToString());
                        try
                        {
                            DR.ID_CFD = Convert.ToInt32(reader["ID_CFD"]);
                        }
                        catch(Exception ex)
                        { }
                        try
                        {
                            DR.IdHistRec =reader["Id_Hist_Rec"].ToString();
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        DR.Serie = reader["Serie_CRP"].ToString();
                        DR.Folio = Convert.ToInt32(reader["Folio_CRP"]);
                        DR.FechaTimbre = Convert.ToDateTime(reader["Fecha_Timbrado_CRP"].ToString());
                        DR.FechaPago = Convert.ToDateTime(reader["Fecha_Pago_CRP"].ToString());
                        DR.TotalCRP = Convert.ToDecimal(reader["Total_CRP"]);
                        DR.SerieFactura = reader["Serie_FACTURA"].ToString();
                        DR.FolioFactura = Convert.ToInt32(reader["Folio_FACTURA"]);
                        DR.TotalFactura = Convert.ToDecimal(reader["Total_FACTURA"].ToString());
                        DR.IdComprobante = Convert.ToInt32(reader["IdComprobante"]);
                        try
                        {
                            DR.ID_CFD = Convert.ToInt32(reader["ID_CFD"]);
                        }
                        catch (Exception ex)
                        { }
                        try
                        {
                            DR.IdHistRec = reader["Id_Hist_Rec"].ToString();
                        }
                        catch (Exception ex)
                        { }
                    }
                    list.Add(DR);

                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MsgError = "Ocurrió un error al consultar los documentos relacionados. " + Environment.NewLine + ex.Message;
            }
            return list;
        }


        public static List<DocumentosRelacionados> GetDocumentosRelacionados(int IdComp, bool CRPs)
        {

            var list = new List<DocumentosRelacionados>();
            
            //            string sql = @"select
            //UUID,
            //Serie,
            //Folio,
            //Moneda,
            //NumParcialidad,
            //ImpSaldoAnt,
            //impPagado,
            //ImpSaldoInsoluto,
            //SAT.FormaDePago.Descripcion,
            //IdComprobante
            //from cfd.DocumentosRelacionados
            //JOIN  cfd.CFD ON cfd.ID_CFD = DocumentosRelacionados.ID_CFD
            //JOIN SAT.FormaDePago ON cfd.MetodoDePago = SAT.FormaDePago.ClaveFormaDePago
            //where DocumentosRelacionados.IdComprobante = @IdComprobante";
            string sql = string.Empty;
            
            if (CRPs)
            {
                #region sql_DocRelacionadosComprobantesPago
                
                sql = @"Select t1.ID_CFD, 
t1.NumParcialidad,
t1.ImpPagado, 
t1.ImpSaldoAnt, 
t1.ImpSaldoInsoluto,
t1.IdComprobante, 
t2.Serie, 
t2.Folio, 
t2.Total, 
t2.UUID, 
t2.Id_Hist_Rec,
t2.FechaTimbrado,
t3.IDPago, 
t3.IDHistRec,
t4.Folio,
t4.Total, 
t4.IDPago,
t4.ID_XML,
t4.Moneda,
t4.Serie,
t4.Fecha, 
t6.FechaPago,
SAT.FormaDePago.Descripcion
from cfd.DocumentosRelacionados AS t1
JOIN  cfd.CFD AS t2 ON t2.ID_CFD = t1.ID_CFD
JOIN Cobranza.ReciboPagado AS t3 on t3.IDHistRec = t2.Id_Hist_Rec
JOIN cfd.ComprobantePago AS t4 on t4.IDPago = t3.IDPago
JOIN SAT.FormaDePago ON t4.FormaPago = SAT.FormaDePago.ClaveFormaDePago
JOIN cfd.ComplementoPago AS t6 ON t6.ID_ComprobantePago = t4.ID_ComprobantePago
where t4.ID_ComprobantePago = @IdComprobante";
                #endregion
            }
            else
            {
                #region sql_DocRelacionadosCFDis

                sql = @"SELECT t3.Serie AS Serie_CRP, t3.Folio AS Folio_CRP, t3.Fecha AS Fecha_Timbrado_CRP, t4.FechaPago AS Fecha_Pago_CRP, 
t3.Total AS Total_CRP, t1.Serie AS Serie_FACTURA, t1.Folio AS Folio_FACTURA, t1.Total AS Total_FACTURA FROM cfd.CFD AS t1 
JOIN Cobranza.ReciboPagado AS t2 ON t2.IDHistRec=t1.Id_Hist_Rec
JOIN cfd.ComprobantePago AS t3 ON t3.IDPago=t2.IDPago 
JOIN cfd.ComplementoPago AS t4 ON t4.ID_ComprobantePago=t3.ID_ComprobantePago
where t1.ID_CFD= @IdComprobante ";

                #endregion
            }

            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            try
            {
                SqlCommand comando = new SqlCommand(sql, conexion);
                if (CRPs)
                {
                    comando.Parameters.Add("@IdComprobante", System.Data.SqlDbType.Int).Value = IdComp;
                }
                else
                {
                    comando.Parameters.Add("@IdComprobante", System.Data.SqlDbType.Int).Value = IdComp;
                }
                
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while(reader.Read())
                {
                    DocumentosRelacionados DR = new DocumentosRelacionados();
                    if (CRPs)
                    {
                        DR.UUID = reader["UUID"].ToString();
                        DR.Serie = reader["Serie"].ToString();
                        DR.Folio = Convert.ToInt32(reader["Folio"]);
                        DR.Moneda = reader["Moneda"].ToString();
                        DR.NumParcialidad = Convert.ToInt32(reader["NumParcialidad"]);
                        DR.ImpSaldoAnt = Convert.ToDecimal(reader["ImpSaldoAnt"].ToString());
                        DR.impPagado = Convert.ToDecimal(reader["impPagado"].ToString());
                        DR.ImpSaldoInsoluto = Convert.ToDecimal(reader["ImpSaldoInsoluto"].ToString());
                        DR.Descripcion = reader["Descripcion"].ToString();
                        DR.IdComprobante = Convert.ToInt32(reader["IdComprobante"]);
                        DR.Fecha = Convert.ToDateTime(reader["Fecha"].ToString());
                        DR.FechaPago = Convert.ToDateTime(reader["FechaPago"].ToString());
                    }
                    else
                    {
                        DR.Serie = reader["Serie_CRP"].ToString();
                        DR.Folio = Convert.ToInt32(reader["Folio_CRP"]);
                        DR.FechaTimbre = Convert.ToDateTime(reader["Fecha_Timbrado_CRP"].ToString()); 
                        DR.FechaPago = Convert.ToDateTime(reader["Fecha_Pago_CRP"].ToString());
                        DR.TotalCRP = Convert.ToDecimal(reader["Total_CRP"]);
                        DR.SerieFactura = reader["Serie_FACTURA"].ToString();
                        DR.FolioFactura = Convert.ToInt32(reader["Folio_FACTURA"]);
                        DR.TotalFactura = Convert.ToDecimal(reader["Total_FACTURA"].ToString());
                    }
                    list.Add(DR);

                }
                reader.Close();
            }
             catch(Exception ex)
            {
                MsgError = "Ocurrió un error al consultar los documentos relacionados. " + Environment.NewLine + ex.Message;
            }
            return list;
        }

       
        public static List<ComprobantePagoEntity> GetComprobantesPago(InmobiliariaEntity idInmobiliaria, DateTime fechaInicio, DateTime fechaFin, bool rbPDF, bool CRPs)
        {
            var list = new List<ComprobantePagoEntity>();
            string sql = string.Empty;
            
            if (CRPs)
            {
                #region sql_ComprobantesPago
                //          sql = @"Select  
                //                  Serie, Folio,
                //                  FormaPago, 
                //                  MetodoPago,
                //                  Fecha,
                //                  Moneda, 
                //                  TipoCambio,
                //                  Subtotal,
                //                  Total, 
                //                  UUID, 
                //                  cfd.ComprobantePago.ID_Contribuyente,
                //                  FechaTimbrado,
                //                  SAT.FormaDePago.Descripcion, 
                //                  ID_Cliente, 
                //                  cfd.ComprobantePago.IDPago,
                //                  cfd.ComprobantePago.ID_ComprobantePago, 
                //                  Cobranza.ReciboPagado.IDHistRec AS IdHistRec 
                //                  from cfd.ComprobantePago
                //                  JOIN Empresa.Contribuyente ON cfd.ComprobantePago.ID_Contribuyente = Empresa.Contribuyente.ID_Contribuyente
                //                  JOIN cfd.TimbreDigital ON ComprobantePago.IDTimbreDigital = cfd.TimbreDigital.ID_Timbre
                //                  JOIN SAT.FormaDePago ON cfd.ComprobantePago.FormaPago = SAT.FormaDePago.ClaveFormaDePago 
                //                  JOIN cfd.ComplementoPago ON cfd.ComplementoPago.ID_ComprobantePago = cfd.ComprobantePago.ID_ComprobantePago 
                //                  JOIN Empresa.Arrendadora_x_Contribuyente ON Empresa.Arrendadora_x_Contribuyente.ID_Arrendadora = Empresa.Contribuyente.ID_Arrendadora 
                //JOIN Cobranza.ReciboPagado ON Cobranza.ReciboPagado.IDPago = cfd.ComprobantePago.IDPago 
                //                  where Empresa.Contribuyente.ID_Arrendadora = @IDArr  AND Fecha >= @FecInicio AND Fecha <= @FechaFin ";
                sql = @"Select  
                        Serie, Folio,
                        FormaPago, 
                        MetodoPago,
                        Fecha,
                        Moneda, 
                        TipoCambio,
                        Subtotal,
                        Total, 
                        UUID, 
                        cfd.ComprobantePago.ID_Contribuyente,
                        FechaTimbrado,
                        SAT.FormaDePago.Descripcion, 
                        ID_Cliente, 
                        cfd.ComprobantePago.IDPago,
                        cfd.ComprobantePago.ID_ComprobantePago, 
                        Cobranza.ReciboPagado.IDHistRec AS IdHistRec 
                        from cfd.ComprobantePago
                        JOIN Empresa.Contribuyente ON cfd.ComprobantePago.ID_Contribuyente = Empresa.Contribuyente.ID_Contribuyente
                        JOIN cfd.TimbreDigital ON ComprobantePago.IDTimbreDigital = cfd.TimbreDigital.ID_Timbre
                        JOIN SAT.FormaDePago ON cfd.ComprobantePago.FormaPago = SAT.FormaDePago.ClaveFormaDePago 
                        JOIN cfd.ComplementoPago ON cfd.ComplementoPago.ID_ComprobantePago = cfd.ComprobantePago.ID_ComprobantePago 
                        JOIN Empresa.Arrendadora_x_Contribuyente ON Empresa.Arrendadora_x_Contribuyente.ID_Arrendadora = Empresa.Contribuyente.ID_Arrendadora 
						JOIN Cobranza.ReciboPagado ON Cobranza.ReciboPagado.IDPago = cfd.ComprobantePago.IDPago                         
                        WHERE Empresa.Arrendadora_x_Contribuyente.ID_Arr = @IDArr  AND Fecha >= @FecInicio AND Fecha <= @FechaFin 
                        ORDER BY Serie, Folio ";
                #endregion
            }
            else
            {
                #region sql_Facturas
                //sql = @"Select  
                //        cfd.CFD.ID_CFD,cfd.CFD.Serie, cfd.CFD.Folio, Fecha, cfd.ComplementoPago.FechaPago, FormaDePago, MetodoPago, TipoCambio, Traslados, cfd.CFD.Subtotal as Importe, 
                //        (cfd.CFD.Subtotal + Traslados) as Subtotal, cfd.CFD.Total, TotalPagado, UUID, cfd.ComprobantePago.ID_Contribuyente, cfd.CFD.FechaTimbrado, 
                //        SAT.FormaDePago.Descripcion, cfd.ComprobantePago.ID_Cliente, ImporteCobrado, RetISRTotalCobrado, RetIVATotalCobrado, IVAcobrado, 
                //        Cobranza.ReciboPagado.IDPago, cfd.ComprobantePago.ID_ComprobantePago,cfd.ComplementoPago.ID_ComprobantePago AS idComprobComp , cfd.CFD.Moneda, cfd.Id_Hist_Rec AS IdHistRec 
                //        from  cfd.CFD 
                //        JOIN Empresa.Contribuyente ON  Empresa.Contribuyente.ID_Contribuyente = cfd.CFD.ID_Contribuyente 
                //        JOIN SAT.FormaDePago ON cfd.CFD.MetodoDePago = SAT.FormaDePago.ClaveFormaDePago 
                //        JOIN Cobranza.ReciboPagado ON Cobranza.ReciboPagado.IDHistRec = cfd.cfd.id_Hist_Rec 
                //        JOIN cfd.ComprobantePago ON cfd.ComprobantePago.IDPago = Cobranza.ReciboPagado.IDPago  
                //        JOIN cfd.ComplementoPago ON cfd.ComplementoPago.ID_ComprobantePago = cfd.ComprobantePago.ID_ComprobantePago 
                //        WHERE  Empresa.Contribuyente.ID_Arrendadora = @IDArr AND cfd.ComplementoPago.FechaPago >= @FecInicio AND cfd.ComplementoPago.FechaPago <= @FechaFin ";
                sql = @"Select  
                        cfd.CFD.ID_CFD,cfd.CFD.Serie, cfd.CFD.Folio, Fecha, cfd.ComplementoPago.FechaPago, FormaDePago, MetodoPago, TipoCambio, Traslados, cfd.CFD.Subtotal as Importe, 
                        (cfd.CFD.Subtotal + Traslados) as Subtotal, cfd.CFD.Total, TotalPagado, UUID, cfd.ComprobantePago.ID_Contribuyente, cfd.CFD.FechaTimbrado, 
                        SAT.FormaDePago.Descripcion, cfd.ComprobantePago.ID_Cliente, ImporteCobrado, RetISRTotalCobrado, RetIVATotalCobrado, IVAcobrado, 
                        Cobranza.ReciboPagado.IDPago, cfd.ComprobantePago.ID_ComprobantePago,cfd.ComplementoPago.ID_ComprobantePago AS idComprobComp , cfd.CFD.Moneda, cfd.Id_Hist_Rec AS IdHistRec 
                        from  cfd.CFD 
                        JOIN Empresa.Contribuyente ON  Empresa.Contribuyente.ID_Contribuyente = cfd.CFD.ID_Contribuyente 
                        JOIN SAT.FormaDePago ON cfd.CFD.MetodoDePago = SAT.FormaDePago.ClaveFormaDePago 
                        JOIN Cobranza.ReciboPagado ON Cobranza.ReciboPagado.IDHistRec = cfd.cfd.id_Hist_Rec 
                        JOIN cfd.ComprobantePago ON cfd.ComprobantePago.IDPago = Cobranza.ReciboPagado.IDPago  
                        JOIN cfd.ComplementoPago ON cfd.ComplementoPago.ID_ComprobantePago = cfd.ComprobantePago.ID_ComprobantePago 
                        JOIN Empresa.Arrendadora_x_Contribuyente ON Empresa.Arrendadora_x_Contribuyente.ID_Arrendadora = Empresa.Contribuyente.ID_Arrendadora 
                        WHERE Empresa.Arrendadora_x_Contribuyente.ID_Arr = @IDArr AND cfd.ComplementoPago.FechaPago >= @FecInicio AND cfd.ComplementoPago.FechaPago <= @FechaFin 
                        ORDER BY Serie, Folio ";
                
                #endregion
            }


            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            try
            {
                SqlCommand comando = new SqlCommand(sql, conexion);
                //comando.Parameters.Add("@IDArr", SqlDbType.VarChar).Value = idInmobiliaria.IdArrendadora;
                comando.Parameters.Add("@IDArr", SqlDbType.VarChar).Value = idInmobiliaria.ID;
                comando.Parameters.Add("@FecInicio", SqlDbType.DateTime).Value = fechaInicio;
                comando.Parameters.Add("@FechaFin", SqlDbType.DateTime).Value = fechaFin;
                //comando.Parameters.Add("@Conj", SqlDbType.VarChar).Value = conjunt.ID;
                //string NombreConjunto = Conj.Nombre;
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while(reader.Read())
                {
                    ComprobantePagoEntity objComPago = new ComprobantePagoEntity();
                    if (!CRPs)
                    {
                        objComPago.IdCFD = Convert.ToInt32(reader["ID_CFD"].ToString());
                    }
                    objComPago.Serie = reader["Serie"].ToString();
                    objComPago.Folio = Convert.ToInt32(reader["Folio"]);
                    objComPago.UUID = reader["UUID"].ToString();
                    if (CRPs)
                    {
                        //objComPago.FechaPago = Convert.ToDateTime(reader["FechaPago"].ToString());
                        objComPago.FechaEmision = Convert.ToDateTime(reader["Fecha"].ToString());
                    }
                    
                    objComPago.FechaTimbrado = Convert.ToDateTime(reader["FechaTimbrado"].ToString());
                    try
                    {
                        objComPago.FechaPago = Convert.ToDateTime(reader["FechaPago"].ToString());
                    }
                    catch(Exception ex)
                    {
                        string err = ex.Message;
                    }
                    objComPago.Descripcion = reader["Descripcion"].ToString();
                    objComPago.Moneda = reader["Moneda"].ToString();
                    objComPago.TipoCambio = reader["TipoCambio"] == DBNull.Value ? 1 : Convert.ToDecimal(reader["TipoCambio"].ToString());
                    if (objComPago.Moneda == "MXN")
                        objComPago.TipoCambio = 1;
                    objComPago.TotalPago = Convert.ToDecimal(reader["Total"]);
                    //objComPago.Total = Convert.ToDecimal(reader["TotalPagado"]);
                    objComPago.IDCliente = reader["ID_Cliente"].ToString();
                    objComPago.IdPago = Convert.ToInt32(reader["IDPago"]);
                    objComPago.IdComprobante = Convert.ToInt32(reader["ID_ComprobantePago"]);
                    try
                    {
                        objComPago.IdHistRec = reader["IdHistRec"].ToString(); 
                    }
                    catch(Exception ex)
                    {
                        objComPago.IdHistRec = string.Empty;
                    }
                    if (CRPs)
                    {
                        objComPago.ISR = 0;
                        objComPago.RetIVA = 0;
                        objComPago.IVA = 0;
                        objComPago.SubTotal = 0;
                        objComPago.Importe = 0;
                    }
                    else
                    {
                        objComPago.ISR = Convert.ToDecimal(reader["RetISRTotalCobrado"]);
                        objComPago.RetIVA = Convert.ToDecimal(reader["RetIVATotalCobrado"]);
                        objComPago.IVA = Convert.ToDecimal(reader["IVAcobrado"]);
                        objComPago.SubTotal = Convert.ToDecimal(reader["Subtotal"]);
                        objComPago.Importe = Convert.ToDecimal(reader["Importe"]);
                    }
                    objComPago.MetodoPago = reader["MetodoPago"].ToString();
                    list.Add(objComPago);
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                MsgError = "Ocurrió un error al consultar los comprobantes de pago. " + Environment.NewLine + ex.Message;
            }
            return list;

        }


        public static List<ReciboEntity> getListaRecibosEnProcesoCancelacion(string idInmobiliaria, string idConjunto, DateTime fechaInicio, DateTime fechaFin)
        {
            List<ReciboEntity> listaRecibosEnProceso = new List<ReciboEntity>();
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            string sql = @"SELECT ID_CFD, UUID FROM cfd.CancelacionProcesada WHERE FechaAcuse BETWEEN @FechaInicio AND @FechaFin";
            try
            {
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@FechaInicio", System.Data.SqlDbType.DateTime).Value = fechaInicio;
                comando.Parameters.Add("@FechaFin", System.Data.SqlDbType.DateTime).Value = fechaFin;
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    int idCfd = (int)reader["ID_CFD"];
                    ReciboEntity recibo = SaariDB.getDetalleReciboPorCancelar(idInmobiliaria, idConjunto, idCfd);
                    if (recibo != null)
                    {
                        recibo.UUID = reader["UUID"].ToString();
                        recibo.Estatus = "EN PROCESO";
                        listaRecibosEnProceso.Add(recibo);
                    }
                }
                reader.Close();
                conexion.Close();
                return listaRecibosEnProceso;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static List<ProveedorEntity> getListaProveedores()
        {
            List<ProveedorEntity> listaProveedores = new List<ProveedorEntity>();
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            string sql = @"SELECT Proveedor.IDProveedor, Proveedor.Nombre, Proveedor.IDTipoProveedor, TipoProveedor.Descripcion, 
                Proveedor.RFC, Proveedor.IDPaisResidencia, Proveedor.NombreExtranjero, 
                Proveedor.IDTasaIVA, TasasIVA.Descripcion, TasasIVA.Tasa, Pais.Nombre
                FROM Finanzas.Proveedor 
                LEFT JOIN Finanzas.TipoProveedor ON TipoProveedor.IDTipoProveedor = Proveedor.IDTipoProveedor
                LEFT JOIN Finanzas.TasasIVA ON TasasIVA.IDTasas = Proveedor.IDTasaIVA
                LEFT JOIN Finanzas.Pais ON Pais.IDPais = Proveedor.IDPaisResidencia
                ORDER BY Proveedor.Nombre ASC";
            try
            {
                DataTable provDt = new DataTable();
                SqlCommand comando = new SqlCommand(sql, conexion);
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();                
                provDt.Load(reader);
                conexion.Close();
                foreach (DataRow provDr in provDt.Rows)
                {
                    ProveedorEntity proveedor = new ProveedorEntity();
                    proveedor.IDProveedor = provDr["IDProveedor"].ToString();
                    proveedor.Nombre = provDr["Nombre"].ToString();                   
                    proveedor.RFC = provDr["RFC"].ToString();
                    proveedor.NombreExtranjero = provDr["NombreExtranjero"].ToString();
                    proveedor.IDTipoProveedor = Convert.ToInt32(provDr["IDTipoProveedor"]);
                    proveedor.TipoProveedor = provDr["TipoProveedor.Descripcion"].ToString();
                    proveedor.IDTasaIVA = Convert.ToInt32(provDr["IDTasaIVA"]);    
                    proveedor.DescripTasaIVA=provDr["TasasIVA.Descripcion"].ToString();
                    proveedor.TasaIVA = Convert.ToDecimal(provDr["Tasa"]);
                    proveedor.PaisResidencia = provDr["Pais.Nombre"].ToString();
                    listaProveedores.Add(proveedor);
                }
            }
            catch(Exception ex)
            {
                Mensaje = ex.Message;
                return null;
            }
            return listaProveedores;
        }

        public static ProveedorEntity getProveedorByID(int id)
        {
            ProveedorEntity proveedor = new ProveedorEntity();
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            string sql = @"SELECT Proveedor.IDProveedor, Proveedor.Nombre, Proveedor.IDTipoProveedor, TipoProveedor.Descripcion, 
                Proveedor.RFC, Proveedor.IDPaisResidencia, Proveedor.NombreExtranjero, 
                Proveedor.IDTasaIVA, TasasIVA.Descripcion, TasasIVA.Tasa, Pais.Nombre
                FROM Finanzas.Proveedor 
                LEFT JOIN Finanzas.TipoProveedor ON TipoProveedor.IDTipoProveedor = Proveedor.IDTipoProveedor
                LEFT JOIN Finanzas.TasasIVA ON TasasIVA.IDTasas = Proveedor.IDTasaIVA
                LEFT JOIN Finanzas.Pais ON Pais.IDPais = Proveedor.IDPaisResidencia
                WHERE Proveedor.IDProveedor = ? 
                ORDER BY Proveedor.Nombre ASC";
            try
            {
                DataTable provDt = new DataTable();
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@idProv", SqlDbType.Int).Value = id;
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                provDt.Load(reader);
                conexion.Close();
                foreach (DataRow provDr in provDt.Rows)
                {                    
                    proveedor.IDProveedor = provDr["IDProveedor"].ToString();
                    proveedor.Nombre = provDr["Nombre"].ToString();
                    proveedor.RFC = provDr["RFC"].ToString();
                    proveedor.NombreExtranjero = provDr["NombreExtranjero"].ToString();
                    proveedor.IDTipoProveedor = Convert.ToInt32(provDr["IDTipoProveedor"]);
                    proveedor.TipoProveedor = provDr["TipoProveedor.Descripcion"].ToString();
                    proveedor.IDTasaIVA = Convert.ToInt32(provDr["IDTasaIVA"]);
                    proveedor.DescripTasaIVA = provDr["TasasIVA.Descripcion"].ToString();
                    proveedor.TasaIVA = Convert.ToDecimal(provDr["Tasa"]);
                    proveedor.PaisResidencia = provDr["Pais.Nombre"].ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return null;
            }
            return proveedor;
        }

        public static List<ReciboEntity> getComentario(string idCliente, int IdHistRec)
        {
            List<ReciboEntity> listaComentarios = new List<ReciboEntity>();
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            string sql = @"
SELECT Cobranza.Pago.IDPago,Cobranza.Pago.IDCliente,Cobranza.Pago.Fecha,Cobranza.ReciboPagado.IDHistRec,Cobranza.ReciboPagado.TotalPagado,ReciboPagado.Comentarios 
FROM Cobranza.Pago
JOIN  Cobranza.ReciboPagado on Cobranza.Pago.IDPago = Cobranza.ReciboPagado.IDPago
WHERE IDCliente = @idCliente and IDHistRec = @IdHistRec";
            try
            {
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@idCliente", System.Data.SqlDbType.VarChar).Value = idCliente;
                comando.Parameters.Add("@IdHistRec", System.Data.SqlDbType.Int).Value = IdHistRec;
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ReciboEntity recibo = new ReciboEntity();
                    recibo.IDCliente = reader["IDCliente"].ToString();
                    recibo.IDHistRec = (int)reader["IDHistRec"];
                    recibo.Comentario = reader["Comentarios"].ToString();
                    listaComentarios.Add(recibo);
                    
                }
                reader.Close();
                conexion.Close();
                return listaComentarios;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

        public static int getTotalReciboPagado(int IdHistRec)
        {
            
            int IdPago = 0;
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            string sql = @"SELECT IDPago FROM Cobranza.ReciboPagado WHERE IDHistRec = @IdHistRec";
            try
            {
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@IdHistRec", System.Data.SqlDbType.Int).Value = IdHistRec;
                conexion.Open();
                IdPago =Convert.ToInt32(comando.ExecuteScalar());
                
                conexion.Close();
                
                
               return IdPago;
            }
            catch
            {
                conexion.Close();
                return IdPago;
            }
        }


         public static List<SaldoAFavorEntity> getTotalReciboPagadoByIDPago(int idPago)
        {
             List<SaldoAFavorEntity> ListaRecibosPagadosByIDPago = new List<SaldoAFavorEntity>();
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            string sql = @"SELECT * FROM Cobranza.ReciboPagado where IDPago = @IdPago";
            try
            {
               
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@IdPago", System.Data.SqlDbType.Int).Value = idPago;
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    SaldoAFavorEntity infoPago = new SaldoAFavorEntity();
                    infoPago.IdPago = (int)reader["IDPago"];
                    infoPago.totalPagado = (decimal)reader["TotalPagado"];
                    infoPago.IdHistRec = (int) reader ["IDHistRec"];
                    ListaRecibosPagadosByIDPago.Add(infoPago);
 
                }
                reader.Close();
                conexion.Close();
                //if (ListaRecibosPagadosByIDPago.Count > 1)
                //{
                //    int i = 0;
                //    foreach (SaldoAFavorEntity lr  in ListaRecibosPagadosByIDPago)
                //    {
                //        lr.IdHistRec =ListaRecibosPagadosByIDPago[i].IdHistRec;


                //    }
                //}
                return ListaRecibosPagadosByIDPago;
            }
            catch
            {
                conexion.Close();
                return null;
            }
        }

         public static  decimal getSaldoAFavor(string idCliente)
         {
             SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
             try
             {
                 //string sql = "SELECT ISNULL(SUM(Importe), 0) AS Importe FROM Cobranza.SaldoAFavor WHERE IDCliente = @IDCliente";
                 string sql = @"SELECT Cobranza.SaldoAFavor.IDSaldo, Cobranza.SaldoAFavor.Importe AS Favor, ISNULL(SUM(Cobranza.PagoConSaldoAFavor.Total), 0) AS Pagado
                FROM Cobranza.SaldoAFavor 
                LEFT JOIN Cobranza.PagoConSaldoAFavor ON Cobranza.PagoConSaldoAFavor.IDSaldo = Cobranza.SaldoAFavor.IDSaldo 
                WHERE Cobranza.SaldoAFavor.IDCliente = @IDCliente
                GROUP BY Cobranza.PagoConSaldoAFavor.IDSaldo, Cobranza.SaldoAFavor.IDSaldo, Cobranza.SaldoAFavor.Importe";
                 SqlCommand comando = new SqlCommand(sql, conexion);
                 comando.Parameters.Add("@IDCliente", SqlDbType.NVarChar).Value = idCliente;
                 decimal favor = 0, pagado = 0;
                 conexion.Open();
                 SqlDataReader reader = comando.ExecuteReader();
                 while (reader.Read())
                 {
                     favor += Convert.ToDecimal(reader["Favor"]);
                     pagado += Convert.ToDecimal(reader["Pagado"]);
                 }
                 reader.Close();
                 conexion.Close();
                 decimal saldo = favor - pagado;
                 return saldo;
             }
             catch (Exception ex)
             {
                 return 0;
             }
         }
      

        public static List<SaldoAFavorEntity> getSaldoAFavorByIdCliente(string idCliente)
        {
    
            List<SaldoAFavorEntity> ListaSaldoAFavor = new List<SaldoAFavorEntity>();
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            string sql = "Select * from Cobranza.SaldoAFavor Where IDCliente =@idCliente";
            try
            {
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@idCliente", System.Data.SqlDbType.VarChar).Value = idCliente;
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    SaldoAFavorEntity Saldo = new SaldoAFavorEntity();
                    Saldo.IdSaldo =(int)reader["IDSaldo"];
                    Saldo.IdPago=(int)reader["IDPago"];
                    Saldo.IdCliente=  reader["IDCliente"].ToString();
                    Saldo.ImporteSaldo = (decimal)reader["Importe"];
                    ListaSaldoAFavor.Add(Saldo);
                }
                reader.Close();
                conexion.Close();
                return ListaSaldoAFavor;
            }
            catch
            {
                conexion.Close();
                return null;
            }


        }
        public static int getIDPagoByHistRec (int idHistRec)
        {
            int idPago = 0;
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            string sql = "select IDPago from Cobranza.ReciboPagado where IDHistRec = @idHistRec";
            try
            {
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@idHistRec", System.Data.SqlDbType.Int).Value = idHistRec;
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    idPago= (int)reader["IDPago"];
                    
                }
                reader.Close();
                conexion.Close();
                return idPago;
            }
            catch
            {
                conexion.Close();
                return 0;
            }


        }

        
        public static decimal getSaldoAFavorbyIdPago(int IdPago)
        {
            decimal saldo = 0;
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            string sql = "select * from Cobranza.SaldoAFavor where  IDPago =@IDPago";
            try
            {
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@IDPago", System.Data.SqlDbType.Int).Value = IdPago;
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    SaldoAFavorEntity Saldo = new SaldoAFavorEntity();
                    Saldo.IdPago = (int)reader["IDPago"];
                    Saldo.IdCliente = reader["IDCliente"].ToString();
                    Saldo.ImporteSaldo = (decimal)reader["Importe"];
                    saldo = Saldo.ImporteSaldo;

                }
                reader.Close();
                conexion.Close();
                return saldo;
            }
            catch
            {
                conexion.Close();
                return 0;
            }


        }
        public static List<SaldoAFavorEntity> getSaldoAFavorByIdPago(int IdPago)
        {
            List<SaldoAFavorEntity> ListaSaldoAFavor = new List<SaldoAFavorEntity>();
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            string sql = "select * from Cobranza.SaldoAFavor where  IDPago =@IDPago";
            try
            {
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@IDPago", System.Data.SqlDbType.Int).Value = IdPago;
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    SaldoAFavorEntity Saldo = new SaldoAFavorEntity();                   
                    Saldo.IdPago = (int)reader["IDPago"];
                    Saldo.IdCliente = reader["IDCliente"].ToString();
                    Saldo.ImporteSaldo = (decimal)reader["Importe"];
                    ListaSaldoAFavor.Add(Saldo);

                }
                reader.Close();
                conexion.Close();
                return ListaSaldoAFavor;
            }
            catch
            {
                conexion.Close();
                return null;
            }


        }
        public static string GetLogoArrendadora(string idARR)
        {
           
            string ruta = string.Empty;
            string sql = @"SELECT Logo FROM Empresa.Contribuyente
JOIN Empresa.Arrendadora_x_Contribuyente ON Contribuyente.ID_Arrendadora = Arrendadora_x_Contribuyente.ID_Arrendadora
 WHERE Arrendadora_x_Contribuyente.ID_Arr = @idArr";

            try
            {
                SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@idArr", SqlDbType.NVarChar).Value = idARR;
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ruta = reader["Logo"] == DBNull.Value ? string.Empty : reader["Logo"].ToString();
                    break;
                }
                reader.Close();
                conexion.Close();
                return ruta;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
        public static List<SaldoAFavorEntity> getSumatoriaSaldoAFavor()
        {

            List<SaldoAFavorEntity> ListaSaldoAFavor = new List<SaldoAFavorEntity>();
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            //string sql = "SELECT SUM(Importe)AS Importe,IDCliente  FROM Cobranza.SaldoAFavor Group by IDCliente";
            string sql = @"SELECT Cobranza.SaldoAFavor.IDSaldo,Cobranza.SaldoAFavor.IDCliente, Cobranza.SaldoAFavor.Importe AS Favor, ISNULL(SUM(Cobranza.PagoConSaldoAFavor.Total), 0) AS Pagado
FROM Cobranza.SaldoAFavor 
LEFT JOIN Cobranza.PagoConSaldoAFavor ON Cobranza.PagoConSaldoAFavor.IDSaldo = Cobranza.SaldoAFavor.IDSaldo 
GROUP BY Cobranza.PagoConSaldoAFavor.IDSaldo, Cobranza.SaldoAFavor.IDSaldo, Cobranza.SaldoAFavor.Importe,Cobranza.SaldoAFavor.IDCliente";
            try
            {
                SqlCommand comando = new SqlCommand(sql, conexion);
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    SaldoAFavorEntity Saldo = new SaldoAFavorEntity();
                   
                    Saldo.IdCliente = reader["IDCliente"].ToString();
                    Saldo.SumaSaldoFavor = Convert.ToDecimal(reader["Favor"]);
                    Saldo.totalPagado = Convert.ToDecimal(reader["Pagado"]);
                    ///Saldo.SumaSaldoFavor = (decimal)reader["Importe"];
                    ListaSaldoAFavor.Add(Saldo);

                }
                reader.Close();
                conexion.Close();
                return ListaSaldoAFavor;
            }
            catch
            {
                conexion.Close();
                return null;
            }

 
        }

        public static List<ClaveMetodoPagoEntity> GetDescripcion()
        {
            List<ClaveMetodoPagoEntity> list = new List<ClaveMetodoPagoEntity>();

            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);

            string sql = "select ClaveFormaDePago, Descripcion from [SAT].[FormaDePago]";
            try
            {
                SqlCommand comando = new SqlCommand(sql, conexion);
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    ClaveMetodoPagoEntity obj = new ClaveMetodoPagoEntity();
                    obj.Clave = reader["ClaveFormaDePago"].ToString();
                    obj.Descripcion = reader["Descripcion"].ToString();
                    list.Add(obj);
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



        #region Honduras 

        public static string getReferenciaPago(int IdPago)
        {
            string Ref = string.Empty;
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.SaariCFD_ConnectionString);
            string sql = "SELECT  ReferenciaPago FROM Cobranza.Pago WHERE IDPago =@IDPago";
            try
            {
                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.Add("@IDPago", System.Data.SqlDbType.Int).Value = IdPago;
                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    Ref = reader["ReferenciaPago"].ToString();
                }
                reader.Close();
                conexion.Close();
                return Ref;
            }
            catch
            {
                conexion.Close();
                return string.Empty; ;
            }
        }
        
        #endregion


    }//END CLASS
}//END NAMESPACE
