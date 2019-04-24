using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.Properties;
using System.Globalization;

namespace GestorReportes.BusinessLayer.Helpers
{
    class HelpInmobiliarias
    {
        
        public static List<ClienteEntity> GetClientes()
        {
            return SaariDB.getClientes();

        }
        public static List<ClienteEntity> GetClientesReportComprobantePago()
        {
            return SaariDB.GetClientesReportComprobantePago();

        }
        public static List<ClienteEntity> GetClientesReportComprobantePago(DateTime fechaIni, DateTime fechaFin)
        {
            return SaariDB.GetClientesReportComprobantePago(fechaIni, fechaFin);

        }

        /// <summary>
        /// Este metodo obtiene las Inmobiliarias por permisos de usuario.
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="permisos"></param>
        /// <returns></returns>
        public static List<InmobiliariaEntity> obtenerInmobiliarias(string usuario,  bool permisos)
        {
            //bool p = this.permisosInmobiliaria;
           
            List<InmobiliariaEntity> ListaInmobiliarias = new List<InmobiliariaEntity>();
            try
            {
                if (permisos)
                    ListaInmobiliarias = SaariDB.getInmobiliariasPorUsuarios(usuario);
                else
                {
                    if (ListaInmobiliarias.Count <= 0)
                    {
                        ListaInmobiliarias = SaariDB.getListaInmobiliarias();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return ListaInmobiliarias;
        }
        /// <summary>
        /// Este metodo obtiene los conjuntos de una inmobiliaria.
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="permisos"></param>
        /// <param name="grupoEmp"></param>
        /// <returns></returns>
        public static List<ConjuntoEntity> getConjuntos(string idInmobiliaria)
        {
                List<ConjuntoEntity> listaConjuntos = new List<ConjuntoEntity>();
                listaConjuntos = SaariDB.getConjuntosHelp(idInmobiliaria);
            return listaConjuntos;
                    
        }
      
        public static List<ConjuntoInmobiliaria> GetConjuntoInmobiliarias(string idArr, string idConjunto, DateTime fechaIni, DateTime fechaFin)
        { 
            return SaariDB.GetConjuntoInmobiliarias(idArr, idConjunto, fechaIni, fechaFin);
        }

        /// <summary>
        /// Este metodo obtiene las Inmobiliarias por permisos de usuario y grupo Empresarial
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="permisos"></param>
        /// <param name="grupoEmp"></param>
        /// <returns></returns>
        public static List<InmobiliariaEntity> obtenerInmobiliarias(string usuario, bool permisos, string grupoEmp)
        {
            //bool p = this.permisosInmobiliaria;

            List<InmobiliariaEntity> ListaInmobiliarias = new List<InmobiliariaEntity>();
            try
            {
                if (permisos && grupoEmp != "Todos")
                    ListaInmobiliarias = SaariDB.getInmobiliariasPorUsuarios(usuario, grupoEmp, false);
                else
                    ListaInmobiliarias = SaariDB.getInmobiliariasPorUsuarios(usuario,grupoEmp, false);
                
                if (ListaInmobiliarias.Count <= 1)
                {
                    ListaInmobiliarias = SaariDB.getInmobiliariasPorUsuarios(usuario, grupoEmp, true);
                    if (ListaInmobiliarias != null)
                    {
                        if (ListaInmobiliarias.Count <= 1)
                        {
                            ListaInmobiliarias = SaariDB.getInmobiliariasPorUsuarios(usuario, grupoEmp, false);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return ListaInmobiliarias;
        }

        /// <summary>
        /// Este metodo obtiene los conjuntos de una inmobiliaria por grupo Empresarial
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="permisos"></param>
        /// <param name="grupoEmp"></param>
        /// <returns></returns>
        public static List<ConjuntoEntity> getConjuntos(string idInmobiliaria, bool esGrupo)
        {
            List<ConjuntoEntity> listaConjuntos = new List<ConjuntoEntity>();
            listaConjuntos = SaariDB.getConjuntosGrupoHelp(idInmobiliaria, esGrupo);
            return listaConjuntos;

        }


       /// <summary>
       /// Este metodo obtiene el Nombre y RFC del contribuyente por medio del Usuario
       /// </summary>
       /// <param name="usuario"></param>
       /// <param name="permisos"></param>
       /// <returns></returns>
        public static List<InmobiliariaEntity> obtenerInmobiliariasContibuyente(string usuario, bool permisos)
        {
            //bool p = this.permisosInmobiliaria;

            List<InmobiliariaEntity> ListaInmobiliarias = new List<InmobiliariaEntity>();
            try
            {
                if (permisos)
                    ListaInmobiliarias = SaariDB.getInmobiliariasPorUsuariosxContribuyente(usuario);
                if (ListaInmobiliarias.Count <= 0)
                {
                  //  ListaInmobiliarias = SaariDB.getListaInmobiliarias();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return ListaInmobiliarias;
        }

        /// <summary>
        /// Este metodo ayuda a obtener el nombre del mes dependiendo su valor numerico.
        /// </summary>
        /// <param name="month"> valor numerico del mes</param>
        /// <returns></returns>
        public static string MonthName(int month)
        {
            DateTimeFormatInfo dtInfo = new CultureInfo("es-MX", false).DateTimeFormat;
            return dtInfo.GetMonthName(month);
        }


    }
}
