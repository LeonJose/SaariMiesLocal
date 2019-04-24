using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GestorReportes.PresentationLayer;

namespace GestorReportes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// <param name="parametros">Parámetros recibidos desde Saari. [0]:Clave de reporte, [1]:Ruta .fr3 de reporte</param>
        /// </summary>
        [STAThread]
        static void Main(string[] parametros)
        {
            //foreach (string parametro in parametros)
            //{
            //    MessageBox.Show(parametro);
            //}

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //parametros[0] = "cartera306090contratos";
            try
            {
                if (parametros.Length <= 0)
                    System.Environment.Exit(0);
                if (parametros[0] == "listadoclientes")
                {
                    GestorReportes.BusinessLayer.ComponentLayer.ListadoClientes listado = new GestorReportes.BusinessLayer.ComponentLayer.ListadoClientes();
                    string result = listado.generarListado(parametros[1], false);
                    if (string.IsNullOrEmpty(result))
                        MessageBox.Show("¡Reporte generado correctamente!", "Listado de clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Ocurrió un error al generar el listado de clientes: " + Environment.NewLine + result, "Listado de clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (parametros[0] == "cartera306090contratos")
                    Application.Run(new Frm_Cartera306090(parametros[1], parametros[2], true, "Cartera vencida 30, 60, 90 y +90 por contratos"));
                else if (parametros[0] == "auditoriacobranza")
                    Application.Run(new Frm_AuditoriaCobranza(parametros[1], parametros[2]));
                else if (parametros[0] == "repegresos")
                    Application.Run(new Frm_ReporteEgresos(parametros[1], parametros[2]));
                else if (parametros[0] == "auditoriacontratos")
                    Application.Run(new Frm_Auditoria(parametros[1], parametros[2]));
                else if (parametros[0] == "auditoriapreventa")
                    Application.Run(new Frm_Auditoria(parametros[1], parametros[2], true));
                else if (parametros[0] == "configurarpolizaegreso")
                    Application.Run(new Frm_ConfigurarPolizaEgreso(parametros[1]));
                else if (parametros[0] == "polizaegreso")
                    Application.Run(new Frm_GenerarPolizaEgreso(parametros[1]));
                else if (parametros[0] == "cfdiconsecutivos")
                    Application.Run(new Frm_CFDIsConsecutivos(parametros[1], parametros[2]));
                else if (parametros[0] == "cfdiconsecutivosfact")
                    Application.Run(new Frm_ConsecutivosCFDIsFacturacion(parametros[1]));
                else if (parametros[0] == "reportefacturacion")
                    Application.Run(new Frm_ReporteFacturacion(parametros[1], parametros[2]));
                else if (parametros[0] == "cartera306090")
                    Application.Run(new Frm_Cartera306090(parametros[1], parametros[2]));
                else if (parametros[0] == "cartera306090xrubro")
                    Application.Run(new Frm_Cartera306090(parametros[1], parametros[2], "Cartera vencida 30, 60, 90 y + 90 por rubro"));
                else if (parametros[0] == "rentroll")
                    Application.Run(new Frm_RentRoll(parametros[1], parametros[2]));
                else if (parametros[0] == "estadocuentacliente")
                    Application.Run(new Frm_EdoCtaCte(parametros[1], parametros[2]));
                else if (parametros[0] == "estadocuentarenta")
                    Application.Run(new Frm_EdoCtaRenta(parametros[1], parametros[2]));
                else if (parametros[0] == "saldoventa")
                    Application.Run(new Frm_ReporteSaldoVenta(parametros[1]));
                else if (parametros[0] == "repimpanti")
                    Application.Run(new Frm_RepresentacionImpresaAntilavado(parametros[1]));
                else if (parametros[0] == "avisocobranza")
                    Application.Run(new Frm_RecordatorioCobranza(parametros[1], parametros[2], parametros[3]));
                else if (parametros[0] == "antilavadoventa")
                    Application.Run(new Frm_GenerarXMLAntilavado(true));
                else if (parametros[0] == "reportegastosmant")
                    Application.Run(new Frm_Mant_Gastos(parametros[1], parametros[2]));
                else if (parametros[0] == "analisisdeudores")
                    Application.Run(new Frm_ReporteGlobal(parametros[1]));
                else if (parametros[0] == "reciboscobradosporfolio")
                    Application.Run(new Frm_RecibosCobradoPorFolio(parametros[1], parametros[2]));
                else if (parametros[0] == "editarantilavado")
                    Application.Run(new Frm_EditarXMLAntilavado());
                else if (parametros[0] == "poliza")
                {
                    #region poliza
                    string usuario = "";

                    if (parametros.Length > 1)
                    {
                        if (parametros.Length >= 3)
                            usuario = parametros[2].Trim();

                        if (parametros[1] == "copropiedad")
                        {
                            Application.Run(new Frm_GenerarPoliza("CoPropiedad", usuario));
                        }
                        else if (parametros[1] == "venta")
                        {
                            Application.Run(new Frm_GenerarPoliza("Venta", usuario));
                        }
                        else
                        {
                            Application.Run(new Frm_GenerarPoliza("Renta", usuario));
                        }
                    }
                    else
                        Application.Run(new Frm_GenerarPoliza());
                    #endregion
                }
                else if (parametros[0] == "configurarpoliza")
                {
                    #region configurarPoliza
                    if (parametros.Length > 2)
                    {
                        if (parametros[1] == "copropiedad")
                            Application.Run(new Frm_ConfigurarPoliza(true, parametros[2]));
                        else
                            Application.Run(new Frm_ConfigurarPoliza(parametros[1]));
                    }
                    else
                        Application.Run(new Frm_ConfigurarPoliza(parametros[1]));
                    #endregion
                }
                else if (parametros[0] == "antilavado")
                    Application.Run(new Frm_GenerarXMLAntilavado(false));
                else if (parametros[0] == "consultaglobal")//Este no se usa. Se hizo un proyecto independiente para la busqueda global
                    Application.Run(new Frm_Buscar(/*parametros[1]*/));
                else if (parametros[0] == "carteraventas")
                    Application.Run(new Frm_CarteraVencidaVta(parametros[1], parametros[2]));
                else if (parametros[0] == "Agenda")
                    Application.Run(new Frm_RepAgenda(parametros[1]));
                else if (parametros[0] == "Mnt")
                    Application.Run(new Frm_RepMant(parametros[1], parametros[2]));
                else if (parametros[0] == "Bitacora")
                    Application.Run(new Frm_RepBitacora(parametros[1]));
                else if (parametros[0] == "fichainformativa")
                    Application.Run(new Frm_FichaInformativa(parametros[1]));
                else if (parametros[0] == "reporteresumen")
                    Application.Run(new Frm_ReporteResumen(parametros[1]));
                else if (parametros[0] == "incrementos")
                    Application.Run(new Frm_Incrementos(parametros[1]));
                else if (parametros[0] == "listado")
                    Application.Run(new Frm_RentadosYNoRentados(parametros[1]));
                else if (parametros[0] == "resumenflujo")
                    Application.Run(new Frm_ResumenFlujoNeto(parametros[1]));
                else if (parametros[0] == "detalleflujo")
                    Application.Run(new Frm_DetalleFlujoNeto(parametros[1]));
                else if (parametros[0] == "reciboscancelados")
                    Application.Run(new Frm_CFDIsCancelados(parametros[1], parametros[2]));
                else if (parametros[0] == "listadocontratosrenta")
                    Application.Run(new Frm_ListadoContratosRenta(parametros[1], parametros[2]));
                else if (parametros[0] == "recibosexpedidosporfolio")
                    Application.Run(new Frm_RecibosExpedidosPorFolio(parametros[1], parametros[2], false, false));
                else if (parametros[0] == "recibosexpedidosporfolioTipoI") //Recibos de Venta con Pago Completo
                    Application.Run(new Frm_RecibosExpedidosPorFolio(parametros[1], parametros[2], true, false));
                else if (parametros[0] == "recibosexpedidosporfolioTipoJ") //Recibos de Venta con Pagos en parcialidades
                    Application.Run(new Frm_RecibosExpedidosPorFolio(parametros[1], parametros[2], true, true));
                else if (parametros[0] == "cxp306090")
                    Application.Run(new Frm_CXP306090(parametros[1], parametros[2]));
                else if (parametros[0] == "amortizacionrenta")
                    Application.Run(new Frm_AmortizacionRentas(parametros[1], parametros[2]));
                else if (parametros[0] == "impuestoReportes")
                    Application.Run(new Frm_Impuestos(parametros[1], parametros[2]));
                else if (parametros[0] == "impuestosAcumulables")
                    Application.Run(new Frm_AcumulableParaImpuestos(parametros[1], parametros[2]));
                else if (parametros[0] == "estatusCobranza")
                    Application.Run(new Frm_EstatusCobranza(parametros[1], parametros[2]));
                else if (parametros[0] == "comprobantesPago")
                    Application.Run(new Frm_ComprobantesDePago(parametros[1], parametros[2]));
                else
                    Application.Run(new Frm_Principal(parametros));
            }
            catch (Exception ex)
            {
            }
        }             
    }
}