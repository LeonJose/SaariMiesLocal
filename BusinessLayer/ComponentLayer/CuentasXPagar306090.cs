using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastReport;
using FastReport.Export.Pdf;
using FastReport.Export.OoXML;
using System.Diagnostics;
using System.IO;
using GestorReportes.BusinessLayer.Entities;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class CuentasXPagar306090: SaariReport, IReport, IBackgroundReport 
    {
        CXPConfiguracionReporte ConfiguracionReporte = new CXPConfiguracionReporte();
        public List<ProvisionEntity> ListaDeProvisiones = new List<ProvisionEntity>();
        /// <summary>
        /// Constructor Base
        /// </summary>
        public CuentasXPagar306090()
        {
        }
        /// <summary>
        /// Constructor con parámetros de entrada, se asignan valores
        /// </summary>
        /// <param name="idInmobiliaria"></param>
        /// <param name="idConjuntoDateTime"></param>
        /// <param name="?"></param>
        /// <param name="esPdf"></param>
        /// <param name="incluirIVA"></param>
        /// <param name="esDetallado"></param>
        /// <param name="rutaFormato"></param>
        /// <param name="usuario"></param>
        /// <param name="tomarTcEmision"></param>
        /// <param name="tipoCambio"></param>
        /// <param name="esPorContrato"></param>
        public CuentasXPagar306090(string idInmobiliaria, string idConjunto, DateTime fechaCorte, bool esPdf, bool incluirIVA, bool esDetallado, string rutaFormato, string usuario, bool tomarTcEmision, decimal tipoCambio, bool esPorConjunto)
        {
            ConfiguracionReporte.IDInmobiliaria = idInmobiliaria;
            ConfiguracionReporte.IDConjunto = idConjunto;
            ConfiguracionReporte.FechaCorte = fechaCorte;
            ConfiguracionReporte.EsPdf = esPdf;
            ConfiguracionReporte.IncluirIVA = incluirIVA;
            ConfiguracionReporte.EsDetallado = esDetallado;
            ConfiguracionReporte.RutaFormato = rutaFormato;
            ConfiguracionReporte.Usuario = usuario;            
            ConfiguracionReporte.TipoCambio = tipoCambio;
            ConfiguracionReporte.EsPorConjunto= esPorConjunto;
        }
        public void asignarConfiguracion(CXPConfiguracionReporte config)
        {
            ConfiguracionReporte = config;
        }
        public InmobiliariaEntity obtenerInmobiliariaPorID(string ID)
        {
            return SaariDB.getInmobiliariaByID(ID);
        }
        public List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();
        }
        public List<ConjuntoEntity> obtenerConjuntos(string idInmobiliaria)
        {
            return SaariDB.getConjuntos(idInmobiliaria);
        }

        public string generar()
        {
            return generarReporte();
        }
       
        private string generarReporte()
        {
            try
            {
                //List<ProvisionEntity> listaProvisiones = null;
                OnCambioProgreso(10);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                //if(ConfiguracionReporte.EsPorConjunto)
                ListaDeProvisiones = SaariDB.getProvisionesEgreso(ConfiguracionReporte);
                string result = SaariDB.ErrorCXP;
                OnCambioProgreso(25);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                if (ListaDeProvisiones != null)
                {
                    if (ListaDeProvisiones.Count > 0)
                    {
                        List<string> listaIDsProveedores = ListaDeProvisiones.Select(p => p.IDProveedor).Distinct().ToList();
                        List<CXP306090Entity> listaCXP = new List<CXP306090Entity>();                      
                        OnCambioProgreso(30);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        int porcentaje = 25;
                        decimal factor = 50 / listaIDsProveedores.Count;
                        factor = factor >= 1 ? factor : 1;


                        foreach (string idProveedor in listaIDsProveedores)
                        {
                            if (porcentaje <= 75)
                                porcentaje += Convert.ToInt32(factor);
                            OnCambioProgreso(porcentaje);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";
                            
                            CXP306090Entity registroCXP = new CXP306090Entity();                           
                            registroCXP.IDProveedor = idProveedor;
                            registroCXP.Moneda = string.Empty;

                            #region Asignación de listas 30,60,90 y mas
                            List<ProvisionEntity> lista30 = (from p in ListaDeProvisiones
                                                             where (p.FechaGasto >= ConfiguracionReporte.FechaCorte.AddDays(-30) && p.FechaGasto <= ConfiguracionReporte.FechaCorte.Date &&
                                                             p.IDProveedor == idProveedor)
                                                             select p).ToList();

                            List<ProvisionEntity> lista60 = (from p in ListaDeProvisiones
                                                             where (p.FechaGasto >= ConfiguracionReporte.FechaCorte.AddDays(-60) && p.FechaGasto <= ConfiguracionReporte.FechaCorte.Date.AddDays(-31) &&
                                                             p.IDProveedor == idProveedor)
                                                             select p).ToList();

                            List<ProvisionEntity> lista90 = (from p in ListaDeProvisiones
                                                             where (p.FechaGasto >= ConfiguracionReporte.FechaCorte.AddDays(-90) && p.FechaGasto <= ConfiguracionReporte.FechaCorte.Date.AddDays(-61) &&
                                                             p.IDProveedor == idProveedor)
                                                             select p).ToList();

                            List<ProvisionEntity> listaMas90 = (from p in ListaDeProvisiones
                                                                where (p.FechaGasto <= ConfiguracionReporte.FechaCorte.AddDays(-91) &&
                                                                p.IDProveedor == idProveedor)
                                                                select p).ToList();

                            #endregion

                            #region Datos del Proveedor
                            int idProv = 0;
                            ProveedorEntity proveedor = new ProveedorEntity();
                            try
                            {
                                idProv = Convert.ToInt32(idProveedor);
                                proveedor = SaariE.getProveedorByID(idProv);
                                if (proveedor != null)
                                {
                                    registroCXP.IDProveedor = idProveedor;
                                    
                                    registroCXP.NombreProveedor = proveedor.Nombre;
                                    registroCXP.Cuenta = proveedor.RFC;
                                    registroCXP.RFC = proveedor.RFC;
                                    
                                }
                                else
                                {
                                    registroCXP.IDProveedor = idProveedor;
                                    registroCXP.NombreProveedor = "No localizado";
                                    registroCXP.Cuenta = "";
                                    registroCXP.RFC = string.Empty;
                                }
                            }
                            catch
                            {
                                proveedor=SaariDB.getProveedorById(idProveedor);
                                if (proveedor != null)
                                {
                                    registroCXP.IDProveedor = idProveedor;
                                    if (string.IsNullOrEmpty(proveedor.Nombre))
                                    {
                                        registroCXP.NombreProveedor = (from p in ListaDeProvisiones where (p.IDProveedor == idProveedor) select p.NombreProveedor).First();
                                    }
                                    else
                                        registroCXP.NombreProveedor = proveedor.Nombre;
                                    registroCXP.Cuenta = "";
                                    registroCXP.RFC = string.Empty;
                                }
                                else
                                {
                                    registroCXP.IDProveedor = idProveedor;
                                    registroCXP.NombreProveedor = "No localizado";
                                    registroCXP.Cuenta = "";
                                    registroCXP.RFC = string.Empty;
                                }
                            }
                            #endregion

                            if (ConfiguracionReporte.IncluirIVA)
                            {
                                #region Incluir Impuestos
                                //TO DO:
                                decimal total30 = 0, total60 = 0, total90 = 0, totalMas90 = 0;

                                #region a 30 dias
                                foreach (var p in lista30)
                                {
                                    decimal cantidad30 = 0;
                                    if (p.Moneda == "D")
                                    {
                                        cantidad30 = Math.Round(p.TotalCheque * ConfiguracionReporte.TipoCambio, 2);
                                        total30 += cantidad30;
                                    }
                                    else
                                    {
                                        cantidad30 = p.TotalCheque;
                                        total30 += cantidad30;
                                    }
                                    p.Importe30 = cantidad30;
                                }
                                #endregion

                                #region a 60 dias
                                foreach (var p in lista60)
                                {
                                    decimal cantidad60 = 0;
                                    if (p.Moneda == "D")
                                    {
                                        cantidad60 = Math.Round(p.TotalCheque * ConfiguracionReporte.TipoCambio, 2);
                                        total60 += cantidad60;
                                    }
                                    else
                                    {
                                        cantidad60 = p.TotalCheque;
                                        total60 += cantidad60;
                                    }
                                    p.Importe60 = cantidad60;
                                }
                                #endregion

                                #region a 90 dias
                                foreach (var p in lista90)
                                {
                                    decimal cantidad90 = 0;
                                    if (p.Moneda == "D")
                                    {
                                        cantidad90 = Math.Round(p.TotalCheque * ConfiguracionReporte.TipoCambio, 2);
                                        total90 += cantidad90;
                                    }
                                    else
                                    {
                                        cantidad90 = p.TotalCheque;
                                        total90 += cantidad90;
                                    }
                                    p.Importe90 = cantidad90;
                                }
                                #endregion

                                #region a mas de 90 dias
                                foreach (var p in listaMas90)
                                {
                                    decimal cantidadMas90 = 0;
                                    if (p.Moneda == "D")
                                    {
                                        cantidadMas90 = Math.Round(p.TotalCheque * ConfiguracionReporte.TipoCambio, 2);
                                        totalMas90 += cantidadMas90;
                                    }
                                    else
                                    {
                                        cantidadMas90 = p.TotalCheque;
                                        totalMas90 += cantidadMas90;
                                    }
                                    p.ImporteMas90 = cantidadMas90;
                                }
                                #endregion

                                #region Totales
                                registroCXP.Total30 = total30;
                                registroCXP.Total60 = total60;
                                registroCXP.Total90 = total90;
                                registroCXP.TotalMas90 = totalMas90;
                                #endregion

                                #endregion
                            }
                            else
                            {
                                #region Sin Impuestos

                                decimal total30 = 0, total60 = 0, total90 = 0, totalMas90 = 0;

                                #region a 30 dias
                                foreach (var p in lista30)
                                {
                                    decimal cantidad30 = 0;
                                    if (p.Moneda == "D")
                                    {
                                        cantidad30 = Math.Round(p.ImporteGasto * ConfiguracionReporte.TipoCambio,2);
                                        total30 += cantidad30;
                                    }
                                    else
                                    {
                                        cantidad30 = p.ImporteGasto;
                                        total30 += cantidad30;
                                    }
                                    p.Importe30 = cantidad30;
                                }
                                #endregion

                                #region a 60 dias
                                foreach (var p in lista60)
                                {
                                    decimal cantidad60 = 0;
                                    if (p.Moneda == "D")
                                    {
                                        cantidad60 = Math.Round(p.ImporteGasto * ConfiguracionReporte.TipoCambio, 2);
                                        total60 += cantidad60;
                                    }
                                    else
                                    {
                                        cantidad60 = p.ImporteGasto;
                                        total60 += cantidad60;
                                    }
                                    p.Importe60 = cantidad60;
                                }
                                #endregion

                                #region a 90 dias
                                foreach (var p in lista90)
                                {
                                    decimal cantidad90 = 0;
                                    if (p.Moneda == "D")
                                    {
                                        cantidad90 = Math.Round(p.ImporteGasto * ConfiguracionReporte.TipoCambio, 2);
                                        total90 += cantidad90;
                                    }
                                    else
                                    {
                                        cantidad90 = p.ImporteGasto;
                                        total90 += cantidad90;
                                    }
                                    p.Importe90 = cantidad90;
                                }
                                #endregion

                                #region a mas de 90 dias
                                foreach (var p in listaMas90)
                                {
                                    decimal cantidadMas90 = 0;
                                    if (p.Moneda == "D")
                                    {
                                        cantidadMas90 = Math.Round(p.ImporteGasto * ConfiguracionReporte.TipoCambio, 2);
                                        totalMas90 += cantidadMas90;
                                    }
                                    else
                                    {
                                        cantidadMas90 = p.ImporteGasto;
                                        totalMas90 += cantidadMas90;
                                    }
                                    p.ImporteMas90 = cantidadMas90;
                                }
                                #endregion

                                #region Totales
                                registroCXP.Total30 = total30;
                                registroCXP.Total60 = total60;
                                registroCXP.Total90 = total90;
                                registroCXP.TotalMas90 = totalMas90;
                                #endregion

                                #endregion
                            }//end if Impuestos
                            
                            #region Listas 
                            registroCXP.Provision30 = lista30;
                            registroCXP.Provision60 = lista60;
                            registroCXP.Provision90 = lista90;
                            registroCXP.ProvisionMas90 = listaMas90;

                            if (registroCXP.TotalProvision > 0)
                                listaCXP.Add(registroCXP);
                            #endregion

                        }//End foreach
                        OnCambioProgreso(80);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";

                        #region Encabezado
                        string nombreInmobiliaria = SaariDB.getNombreInmobiliariaByID(ConfiguracionReporte.IDInmobiliaria);
                        InmobiliariaEntity InmoEntity = SaariDB.getInmobiliariaByID(ConfiguracionReporte.IDInmobiliaria);

                        listaCXP = listaCXP.OrderBy(p => p.NombreProveedor).ToList();                        
                        EncabezadoEntity encabezado = new EncabezadoEntity()
                        {
                            Inmobiliaria = InmoEntity.NombreComercial,
                            RFC=InmoEntity.RFC,
                            FechaFin = ConfiguracionReporte.FechaCorte.ToString(@"dd \de MMMM \del yyyy").ToUpper(),
                            Usuario = ConfiguracionReporte.Usuario,
                            Conjunto = "",
                            Mes30 = getNombreMes(ConfiguracionReporte.FechaCorte.Month),
                            Mes60 = getNombreMes(ConfiguracionReporte.FechaCorte.AddMonths(-1).Month),
                            Mes90 = getNombreMes(ConfiguracionReporte.FechaCorte.AddMonths(-2).Month),
                            MesMas90 = getNombreMes(ConfiguracionReporte.FechaCorte.AddMonths(-3).Month),
                            TipoCambio= ConfiguracionReporte.TipoCambio.ToString("N4"),
                            IncluyeIVA= ConfiguracionReporte.IncluirIVA ? "Incluye IVA" : "No Incluye IVA"
                        };
                        List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                        listaEncabezado.Add(encabezado);
                        #endregion

                        #region Saldos
                        List<SaldoEntity> listaSaldos = new List<SaldoEntity>();
                        SaldoEntity saldo = new SaldoEntity()
                        {
                            Cartera = listaCXP.Sum(s => s.TotalProvision),
                            S30 = listaCXP.Sum(s => s.Total30),
                            S60 = listaCXP.Sum(s => s.Total60),
                            S90 = listaCXP.Sum(s => s.Total90),
                            SMas90 = listaCXP.Sum(s => s.TotalMas90)
                        };
                        listaSaldos.Add(saldo);

                        OnCambioProgreso(90);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        #endregion

                        #region Generar Reporte
                        if (File.Exists(ConfiguracionReporte.RutaFormato))
                        {
                            Report report = new Report();
                            report.Load(ConfiguracionReporte.RutaFormato);
                            report.RegisterData(listaEncabezado, "Encabezado");
                            report.RegisterData(listaCXP, "Provision",2);
                            report.RegisterData(listaSaldos, "Saldo");
                           
                            DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                            bandaRecibos.DataSource = report.GetDataSource("Provision");
                            
                            DataBand bandaFacturas = report.FindObject("Data2") as DataBand;
                            bandaFacturas.DataSource = report.GetDataSource("Provision.Provisiones");

                            PageFooterBand pieDePagina = report.FindObject("PageFooter1") as PageFooterBand;

                            if (!ConfiguracionReporte.EsDetallado)
                                bandaFacturas.Visible = false;

                            if (!ConfiguracionReporte.EsPdf)
                                pieDePagina.Visible = false;
                            return exportar(report, ConfiguracionReporte.EsPdf, "CXP306090");
                        }
                        else
                            return "No se encontro el formato " + ConfiguracionReporte.RutaFormato + Environment.NewLine;
                        
                        #endregion
                    
                    }//End if
                    else
                        return "No se encontraron provisiones de cheque y/o transferencias con las condiciones proporcionadas.";
                }//End if
                else
                    return "Error al obtener las provisiones.";
            }
            catch (Exception ex)
            {
                return "Error inesperado al generar el reporte: " + Environment.NewLine + ex.Message;
            }

        }//End generarReporte()

        public decimal obtenerTipoCambio(DateTime fechaCorte)
        {
            return SaariDB.getTipoCambio(fechaCorte);
        }

        private string getNombreMes(int numMes)
        {
            switch (numMes)
            {
                case 1: return "Enero";
                case 2: return "Febrero";
                case 3: return "Marzo";
                case 4: return "Abril";
                case 5: return "Mayo";
                case 6: return "Junio";
                case 7: return "Julio";
                case 8: return "Agosto";
                case 9: return "Septiembre";
                case 10: return "Octubre";
                case 11: return "Noviembre";
                case 12: return "Diciembre";
                default: return string.Empty;
            }
        }

    
    }//End Class
}//End namespace
