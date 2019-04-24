using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GestorReportes.BusinessLayer.EntitiesAntilavado2;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class InformeAntilavado2
    {
        private string rfcContribuyente = string.Empty, errores = string.Empty, warnings = string.Empty;
        private string rfcClienteGenericoNacional = string.Empty;
        private string rfcClienteGenericoExtranjero = string.Empty;
        private DateTime fechaSolicitada = new DateTime(), periodoInicial = DateTime.Now, periodoFinal = DateTime.Now, periodoInicialMenos6M = DateTime.Now;
        private bool desdeSeptiembre = false, esVenta = false, cancelacionPendiente = false;
        private int numReferencia = 0;

        public string Errores { get { return errores; } }
        public string Warnings { get { return warnings; } }
        public int NumReferencia { get { return numReferencia; } }

        public event EventHandler<CambioProgresoEventArgs> CambioProgreso;

        public InformeAntilavado2(string rfcContrib, DateTime fecha, bool desdeSeptiembre)
        {
            rfcContribuyente = rfcContrib;
            fechaSolicitada = fecha;
            this.desdeSeptiembre = desdeSeptiembre;
            try
            {
                rfcClienteGenericoNacional = Properties.Settings.Default.rfcGenericoNacional;
            }
            catch
            {
                rfcClienteGenericoNacional = string.Empty;
            }
            try
            {
                rfcClienteGenericoExtranjero = Properties.Settings.Default.rfcGenericoExtranjero;
            }
            catch
            {
                rfcClienteGenericoExtranjero = string.Empty;
            }
                               

        }

        public InformeAntilavado2(string rfcContrib, DateTime fecha, bool desdeSeptiembre, string rfcGenerico)
        {
            rfcContribuyente = rfcContrib;
            fechaSolicitada = fecha;
            this.desdeSeptiembre = desdeSeptiembre;
            rfcClienteGenericoNacional = rfcGenerico;
        }

        public Informe obtenerInforme(bool esVenta)
        {
            try 
	        {
                this.esVenta = esVenta;
                OnCambioProgreso(10);
                if (cancelacionPendiente)
                {
                    errores = "Proceso cancelado por el usuario";
                    return null;
                }
                string error = validaDatos();
                OnCambioProgreso(20);
                if (cancelacionPendiente)
                {
                    errores = "Proceso cancelado por el usuario";
                    return null;
                }
                if (!string.IsNullOrEmpty(error))
                {
                    errores = "Ocurrieron los siguientes errores de validación: " + Environment.NewLine + error;
                    return null;
                }

                Informe informe = new Informe();
                informe.MesReportado = fechaSolicitada;
                OnCambioProgreso(30);
                if (cancelacionPendiente)
                {
                    errores = "Proceso cancelado por el usuario";
                    return null;
                }
                informe.SujetoObligado = getSujetoObligadoActual();
                OnCambioProgreso(40);
                if (cancelacionPendiente)
                {
                    errores = "Proceso cancelado por el usuario";
                    return null;
                }
                informe.Avisos = getAvisos();
                OnCambioProgreso(100);
                return informe;    
	        }
	        catch (Exception ex)
	        {
                errores = "Ocurrió un error inesperado: " + Environment.NewLine + ex.Message;
		        return null;
	        }            
        }

        private string validaDatos()
        {
            string errores = string.Empty;
            int registroReferencia = 0;
            registroReferencia = Informes.getReferenciaPorRFC(rfcContribuyente, esVenta);
            if (registroReferencia > 0)
                numReferencia = registroReferencia;
            else
                errores += "- No se pudo obtener el siguiente registro de referencia." + Environment.NewLine;
            //Comentado para pruebas
            DateTime primerFechaValida = new DateTime(2013, 09, 01);
            if (fechaSolicitada < primerFechaValida)
            {
                errores += "- La fecha solicitada corresponde a una anterior a la primer fecha de emisión válida de la ley antilavado (Septiembre 2013)" + Environment.NewLine;
            }
            else
            {
                if (fechaSolicitada.Year > DateTime.Now.Year)
                {
                    errores += "- No se puede obtener un archivo que no corresponda al año en curso" + Environment.NewLine;
                }
                else if (fechaSolicitada.Year == DateTime.Now.Year)
                {
                    //mismo año
                    if (fechaSolicitada.Month > DateTime.Now.Month)
                    {
                        errores += "- No se puede obtener un archivo futuro" + Environment.NewLine;
                    }
                }
            }
            try
            {
                periodoInicial = new DateTime(fechaSolicitada.Year, fechaSolicitada.Month, 1);
                periodoFinal = periodoInicial.AddMonths(1).AddDays(-1);
                periodoInicialMenos6M = periodoInicial.AddMonths(-5);
                if (desdeSeptiembre)
                {
                    DateTime primeroOctubre = new DateTime(2013, 9, 1);
                    if (periodoInicialMenos6M < primeroOctubre)
                        periodoInicialMenos6M = primeroOctubre;
                }
            }
            catch
            {
                errores += "- Error al obtener el periodo de los datos" + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(rfcContribuyente))
                errores += "- No se pudo obtener el RFC del contribuyente" + Environment.NewLine;
            return errores;
        }

        private SujetoObligado getSujetoObligadoActual()
        {
            SujetoObligado sujeto = new SujetoObligado(esVenta);
            sujeto.ClaveRFC = rfcContribuyente.ToUpper();
            return sujeto;
        }

        private List<Aviso> getAvisos()
        {
            try
            {
                if (!esVenta)
                {
                    List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> listaFacs = filtrarFacturas();
                    if (listaFacs.Count <= 0)
                        return null;
                    List<Aviso> listaAvisos = new List<Aviso>();
                    OnCambioProgreso(50);
                    if (cancelacionPendiente)
                    {
                        errores = "Proceso cancelado por el usuario";
                        return null;
                    }

                    int porcentaje = 40;
                    decimal factor = 90 / listaFacs.Count;
                    factor = factor >= 1 ? factor : 1;

                    foreach (GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente factura in listaFacs)
                    {
                        if (porcentaje <= 90)
                            porcentaje += Convert.ToInt32(factor);
                        OnCambioProgreso(porcentaje);
                        if (cancelacionPendiente)
                        {
                            errores = "Proceso cancelado por el usuario";
                            return null;
                        }

                        Aviso aviso = new Aviso();
                        aviso.Referencia = numReferencia;
                        aviso.Prioridad = 1;
                        aviso.Alerta = new Alerta();
                        aviso.Persona = getPersonaPorCliente(factura.IDCliente);
                        
                        aviso.Operaciones = getOperacionesPorCliente(factura.IDCliente);
                        listaAvisos.Add(aviso);
                        numReferencia++;
                    }
                    return listaAvisos;
                }
                else
                {
                    List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> listaFacs = filtrarFacturasVenta();
                    if (listaFacs.Count <= 0)
                        return null;
                    List<Aviso> listaAvisos = new List<Aviso>();
                    OnCambioProgreso(50);
                    if (cancelacionPendiente)
                    {
                        errores = "Proceso cancelado por el usuario";
                        return null;
                    }

                    int porcentaje = 40;
                    decimal factor = 90 / listaFacs.Count;
                    factor = factor >= 1 ? factor : 1;

                    foreach (var factura in listaFacs)
                    {
                        if (porcentaje <= 90)
                            porcentaje += Convert.ToInt32(factor);
                        OnCambioProgreso(porcentaje);
                        if (cancelacionPendiente)
                        {
                            errores = "Proceso cancelado por el usuario";
                            return null;
                        }
                        Aviso aviso = new Aviso();
                        aviso.Referencia = NumReferencia; 
                        aviso.Prioridad = 1;
                        aviso.Alerta = new Alerta();
                        aviso.Persona = getPersonaPorCliente(factura.IDCliente);
                        aviso.Operaciones = getOperacionesPorCliente(factura.IDCliente, esVenta);
                        listaAvisos.Add(aviso);
                        numReferencia++;
                    }
                    return listaAvisos;       
                }
            }
            catch (Exception ex)
            {
                errores = "Error inesperado al obtener los avisos:" + Environment.NewLine + ex.Message;
                return null;
            }
        }

        private List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> filtrarFacturasVenta()
        {
            try
            {
                decimal factorSalarioMinimo = Informes.getSalarioMinimo();
                decimal formulaFiltrado = (factorSalarioMinimo * 8025);
                System.Data.DataTable dtVentasTotales = Informes.getVentasTotales(rfcContribuyente, periodoInicial, periodoFinal);
                List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> listaClientesSeguros = new List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente>();
                List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> listaClientesAInvestigar = new List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente>();
                foreach (System.Data.DataRow rowVenta in dtVentasTotales.Rows)
                {
                    if (Convert.ToDecimal(rowVenta["SUMA"].ToString()) >= formulaFiltrado)
                    {
                        listaClientesSeguros.Add(new GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente(rowVenta["CLIENTE"].ToString(), Convert.ToDecimal(rowVenta["SUMA"].ToString())));
                    }
                    else if (Convert.ToDecimal(rowVenta["SUMA"].ToString()) >= (formulaFiltrado / 6))
                    {
                        listaClientesAInvestigar.Add(new GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente(rowVenta["CLIENTE"].ToString(), Convert.ToDecimal(rowVenta["SUMA"].ToString())));
                    }
                    else
                    {
                        break;
                    }
                }
                if (listaClientesSeguros.Count <= 0 && listaClientesAInvestigar.Count <= 0)
                {
                    errores += "No se encontraron registros que tengan que presentar avisos en el mes solicitado";
                }
                else
                {
                    if (listaClientesAInvestigar.Count > 0)
                    {
                        listaClientesAInvestigar = investigarVentas(listaClientesAInvestigar);
                    }
                    if (listaClientesSeguros.Count <= 0 && listaClientesAInvestigar.Count <= 0)
                    {
                        errores += "No se encontraron registros que tengan que presentar avisos en el mes solicitado";
                    }
                    else
                    {
                        if (listaClientesAInvestigar.Count > 0)
                        {
                            foreach (GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente factura in listaClientesAInvestigar)
                            {
                                listaClientesSeguros.Add(factura);
                            }
                        }
                    }
                }
                return listaClientesSeguros;
            }
            catch
            {
                errores = "Ha ocurrido un error inesperado al hacer la busqueda y filtro de facturas";
                return null;
            }
        }

        private List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> investigarVentas(List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> listaClientesFacs)
        {
            decimal factorSalarioMinimo = Informes.getSalarioMinimo();
            decimal formulaFiltrado = (factorSalarioMinimo * 8025);
            List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> facturasFinales = new List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente>();
            foreach (GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente factura in listaClientesFacs)
            {
                System.Data.DataTable dtVtaPorCliente = Informes.getVentasTotalesPorCliente(factura.IDCliente, rfcContribuyente, periodoInicialMenos6M, periodoFinal);
                if (dtVtaPorCliente.Rows.Count > 0)
                {
                    decimal montoFacturas6M = 0;
                    montoFacturas6M = Convert.ToDecimal(dtVtaPorCliente.Rows[0]["SUMA"].ToString());
                    if (montoFacturas6M >= formulaFiltrado)
                    {
                        factura.TotalFacturas = montoFacturas6M;
                        facturasFinales.Add(factura);
                    }
                }
            }
            return facturasFinales;
        }

        private List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> filtrarFacturas()
        {
            try
            {
                System.Data.DataTable dtFacturasTotales = null;
                //dtFacturasTotales = Informes.getFacturasTotales(rfcContribuyente, periodoInicial, periodoFinal); 
                dtFacturasTotales = Informes.getFacturasTotales(rfcContribuyente, periodoInicial, periodoFinal, rfcClienteGenericoNacional, rfcClienteGenericoExtranjero);               
                decimal factorSalarioMinimo = Informes.getSalarioMinimo();
                List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> listaClientesSeguros = new List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente>();
                List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> listaClientesAInvestigar = new List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente>();
                foreach (System.Data.DataRow rowFactura in dtFacturasTotales.Rows)
                {
                    if (Convert.ToDecimal(rowFactura["SUMA"].ToString()) >= (factorSalarioMinimo * 3210))
                    {
                        listaClientesSeguros.Add(new GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente(rowFactura["CLIENTE"].ToString(), Convert.ToDecimal(rowFactura["SUMA"].ToString())));
                    }
                    else if (Convert.ToDecimal(rowFactura["SUMA"].ToString()) >= (factorSalarioMinimo * 1605))
                    {
                        listaClientesAInvestigar.Add(new GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente(rowFactura["CLIENTE"].ToString(), Convert.ToDecimal(rowFactura["SUMA"].ToString())));
                    }
                    else
                    {
                        break;
                    }
                }
                if (listaClientesSeguros.Count <= 0 && listaClientesAInvestigar.Count <= 0)
                {
                    errores += "No se encontraron registros que tengan que presentar avisos en el mes solicitado";
                }
                else
                {
                    if (listaClientesSeguros.Count > 0)
                    {
                        listaClientesSeguros = facturasMenosNC(listaClientesSeguros);
                    }
                    if (listaClientesAInvestigar.Count > 0)
                    {
                        listaClientesAInvestigar = investigarFacturasMenosNC(listaClientesAInvestigar);
                    }
                    if (listaClientesSeguros.Count <= 0 && listaClientesAInvestigar.Count <= 0)
                    {
                        errores += "No se encontraron registros que tengan que presentar avisos en el mes solicitado";
                    }
                    else
                    {
                        if (listaClientesAInvestigar.Count > 0)
                        {
                            foreach (GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente factura in listaClientesAInvestigar)
                            {
                                listaClientesSeguros.Add(factura);
                            }
                        }
                    }
                }
                return listaClientesSeguros;
            }
            catch
            {
                errores = "Ha ocurrido un error inesperado al hacer la busqueda y filtro de facturas";
                return null;
            }
        }

        private List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> facturasMenosNC(List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> listaClientesFacs)
        {
            decimal factorSalarioMinimo = Informes.getSalarioMinimo();
            List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> facturasFinales = new List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente>();
            foreach (GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente factura in listaClientesFacs)
            {
                System.Data.DataTable dtNc = Informes.getTotalNCPorCliente(factura, rfcContribuyente, periodoInicial, periodoFinal);
                if (dtNc.Rows.Count > 0)
                {
                    decimal auxiliar = 0;
                    auxiliar = factura.TotalFacturas - Convert.ToDecimal(dtNc.Rows[0]["SUMA"].ToString());
                    if (auxiliar >= (factorSalarioMinimo * 1605))
                    {
                        factura.TotalFacturas = auxiliar;
                        facturasFinales.Add(factura);
                    }
                }
                else
                {
                    facturasFinales.Add(factura);
                }
            }
            return facturasFinales;
        }

        private List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> investigarFacturasMenosNC(List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> listaClientesFacs)
        {
            decimal factorSalarioMinimo = Informes.getSalarioMinimo();
            List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente> facturasFinales = new List<GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente>();
            foreach (GestorReportes.BusinessLayer.EntitiesAntiLavado.FacturaTotalPorCliente factura in listaClientesFacs)
            {
                System.Data.DataTable dtFacPorCliente = Informes.getFacturasTotalesPorCliente(factura.IDCliente, rfcContribuyente, periodoInicialMenos6M, periodoFinal);
                System.Data.DataTable dtNCPorCliente = Informes.getTotalNCPorCliente(factura, rfcContribuyente, periodoInicialMenos6M, periodoFinal);
                if (dtFacPorCliente.Rows.Count > 0)
                {
                    decimal montoFacturas6M = 0, montoNotas6M = 0, total = 0;
                    montoFacturas6M = Convert.ToDecimal(dtFacPorCliente.Rows[0]["SUMA"].ToString());
                    if (dtNCPorCliente.Rows.Count > 0)
                    {
                        montoNotas6M = Convert.ToDecimal(dtNCPorCliente.Rows[0]["SUMA"].ToString());
                    }
                    total = montoFacturas6M - montoNotas6M;
                    if (total >= (factorSalarioMinimo * 3210))
                    {
                        factura.TotalFacturas = total;
                        facturasFinales.Add(factura);
                    }
                }

            }
            return facturasFinales;
        }

        private Persona getPersonaPorCliente(string idCliente)
        {
            Persona persona = new Persona();
            persona.EsPersonaFisica = Informes.esClienteFisica(idCliente);
            if (persona.EsPersonaFisica)
            {
                persona.Fisica = getPersonaFisica(idCliente);
            }
            else
            {
                persona.Moral = getPersonaMoral(idCliente);
            }
            persona.DomicilioNacional = getDomicilioPorCliente(idCliente);
            persona.Telefono = getTelefonoPorCliente(idCliente);
            return persona;
        }

        private PersonaFisica getPersonaFisica(string idCliente)
        {
            System.Data.DataTable dtPersonaFisica = Informes.getPersonaFisicaPorIdCliente(idCliente);
            PersonaFisica persona = new PersonaFisica();
            if (dtPersonaFisica.Rows.Count > 0)
            {
                try
                {
                    persona.ReferenciaInterna = dtPersonaFisica.Rows[0]["P0203_NOMBRE"].ToString().Trim();
                    persona.Nombre = dtPersonaFisica.Rows[0]["P0262_NOMBRE_PF"].ToString().Trim().ToUpper();
                    persona.ApellidoPaterno = dtPersonaFisica.Rows[0]["P0263_APELLIDOP_PF"].ToString().Trim().ToUpper();
                    persona.ApellidoMaterno = dtPersonaFisica.Rows[0]["P0264_APELLIDOM_PF"].ToString().Trim().ToUpper();
                    persona.RFC = dtPersonaFisica.Rows[0]["P0204_RFC"].ToString().Trim().ToUpper();
                    persona.FechaNacimiento = Convert.ToDateTime( dtPersonaFisica.Rows[0]["P0207_FECHA"].ToString());/*getFechaNacimientoByRFC(persona.RFC);*/
                    persona.CURP = dtPersonaFisica.Rows[0]["CAMPO3"].ToString().Trim().ToUpper();
                    persona.ActividadEconomica = dtPersonaFisica.Rows[0]["P0257_ID_ACTIVIDAD_ECONOMICA"].ToString().Trim();                    
                }
                catch
                {
                    persona = new PersonaFisica();
                }
            }
            return persona;
        }

        private DateTime getFechaNacimientoByRFC(string rfc)
        {
            /*SABE360423249 */
            DateTime fechaNacimiento = new DateTime();
            try
            {
                string fechaRfc = rfc.Substring(4, 6);
                int year = Convert.ToInt32(fechaRfc.Substring(0, 2));
                if (year > Convert.ToInt32(DateTime.Now.ToString("yy")))
                    year += 1900;
                else
                    year += 2000;
                int month = Convert.ToInt32(fechaRfc.Substring(2, 2));
                int day = Convert.ToInt32(fechaRfc.Substring(4, 2));
                fechaNacimiento = new DateTime(year, month, day);
                return fechaNacimiento;
            }
            catch
            {
                return new DateTime();
            }
        }

        private PersonaMoral getPersonaMoral(string idCliente)
        {
            System.Data.DataTable dtPersonaMoral = Informes.getPersonaMoralPorIdCliente(idCliente);
            PersonaMoral persona = new PersonaMoral();
            if (dtPersonaMoral.Rows.Count > 0)
            {
                try
                {
                    persona.RazonSocial = dtPersonaMoral.Rows[0]["P0203_NOMBRE"].ToString().Trim().ToUpper();
                    persona.RFC = dtPersonaMoral.Rows[0]["P0204_RFC"].ToString().Trim().ToUpper();
                    persona.FechaDeConstitucion = Convert.ToDateTime( dtPersonaMoral.Rows[0]["P0207_FECHA"]);/* getFechaConstitucionByRFC(persona.RFC);*/
                    persona.Giro = dtPersonaMoral.Rows[0]["P0257_ID_ACTIVIDAD_ECONOMICA"].ToString().Trim();
                    PersonaFisica apoderado = new PersonaFisica();
                    apoderado.Nombre = dtPersonaMoral.Rows[0]["P1503_NOMBRE"].ToString().Trim().ToUpper();
                    apoderado.ApellidoPaterno = dtPersonaMoral.Rows[0]["P1509_APELLIDOP_PERSONA"].ToString().Trim().ToUpper();
                    apoderado.ApellidoMaterno = dtPersonaMoral.Rows[0]["P1510_APELLIDOM_PERSONA"].ToString().Trim().ToUpper();
                    apoderado.RFC = dtPersonaMoral.Rows[0]["CAMPO1"].ToString().Trim().ToUpper();
                    if (!string.IsNullOrEmpty(apoderado.RFC))
                        apoderado.FechaNacimiento = getFechaNacimientoByRFC(apoderado.RFC);
                    apoderado.CURP = dtPersonaMoral.Rows[0]["CAMPO2"].ToString().Trim().ToUpper();                    
                    persona.Apoderado = apoderado;
                }
                catch(Exception ex)         
                {
                    persona = new PersonaMoral();
                }
            }
            return persona;
        }

        //private DateTime getFechaConstitucionByRFC(string rfc)
        //{
        //    DateTime fechaConstitucion = new DateTime();
        //    try
        //    {
        //        string fechaRfc = rfc.Substring(3, 6);
        //        int year = Convert.ToInt32(fechaRfc.Substring(0, 2));
        //        if (year > 14)
        //            year += 1900;
        //        else
        //            year += 2000;
        //        int month = Convert.ToInt32(fechaRfc.Substring(2, 2));
        //        int day = Convert.ToInt32(fechaRfc.Substring(4, 2));
        //        fechaConstitucion = new DateTime(year, month, day);
        //        return fechaConstitucion;
        //    }
        //    catch
        //    {
        //        return new DateTime();
        //    }
        //}

        private Domicilio getDomicilioPorCliente(string idCliente)
        {
            System.Data.DataTable dtDomicilio = Informes.getDomicilioPorIdCliente(idCliente);
            Domicilio domicilio = new Domicilio();
            if (dtDomicilio.Rows.Count > 0)
            {
                try
                {
                    domicilio.ReferenciaInterna = dtDomicilio.Rows[0]["P0500_ID_ENTE"].ToString();
                    domicilio.Colonia = dtDomicilio.Rows[0]["P0504_COLONIA"].ToString().ToUpper();
                    domicilio.Calle = dtDomicilio.Rows[0]["P0503_CALLE_NUM"].ToString().ToUpper().Replace("#", "").Replace("-", " ").Trim();
                    domicilio.NumeroExterior = dtDomicilio.Rows[0]["CAMPO1"].ToString().ToUpper();
                    domicilio.NumeroInterior = dtDomicilio.Rows[0]["CAMPO2"].ToString().ToUpper();
                    domicilio.CodigoPostal = dtDomicilio.Rows[0]["P0505_COD_POST"].ToString();
                }
                catch
                {
                    domicilio = new Domicilio();
                }
            }
            return domicilio;
        }

        private Telefono getTelefonoPorCliente(string idCliente)
        {
            System.Data.DataTable dtTelMail = Informes.getTelefonoPorIdCliente(idCliente);
            Telefono telefono = new Telefono();
            if (dtTelMail.Rows.Count > 0)
            {
                try
                {
                    telefono.ReferenciaInterna = dtTelMail.Rows[0]["P0203_NOMBRE"].ToString().Trim();
                    foreach (System.Data.DataRow row in dtTelMail.Rows)
                    {
                        if (row["P0603_TIPO_SERV"].ToString().Trim() == "T")
                            telefono.Numero = row["P0604_DIRECCION"].ToString().Trim().Replace("-", "").Replace("(", "").Replace(")", "").Replace(",", "").Replace(" ", "");
                        else if (row["P0603_TIPO_SERV"].ToString().Trim() == "E")
                        {
                            telefono.EMail = row["P0604_DIRECCION"].ToString().Trim().ToUpper();
                            if (telefono.EMail.Contains(";"))
                                telefono.EMail = telefono.EMail.Split(';')[0];
                        }
                    }
                }
                catch
                {
                    telefono = new Telefono();
                }
            }
            return telefono;
        }

        private List<Operacion> getOperacionesPorCliente(string idCliente)
        {
            System.Data.DataTable dtFacturasPorCliente = Informes.getFacturasPorCliente(idCliente, rfcContribuyente, periodoInicial, periodoFinal);
            List<Operacion> operaciones = new List<Operacion>();
            string monedaFactura = string.Empty;
            if (dtFacturasPorCliente.Rows.Count > 0)
            {
                try
                {
                    foreach (System.Data.DataRow row in dtFacturasPorCliente.Rows)
                    {
                        Operacion operacion = new Operacion(false);
                        operacion.ReferenciaInterna = "CFD " + row["P4006_SERIE"].ToString() + " " + row["P4007_FOLIO"].ToString();
                        operacion.Fecha = (DateTime)row["P2408_FECHA_PAGADO"];
                        CaracteristicasInmueble caracteristicas = new CaracteristicasInmueble();
                        caracteristicas.ReferenciaInterna = row["P0703_NOMBRE"].ToString().Trim();
                        caracteristicas.FolioReal = row["P0740_CAMPO15"].ToString().Trim().ToUpper();
                        DateTime fechaEmision = (DateTime)row["P2409_FECHA_EMISION"];
                        caracteristicas.FechaInicio = new DateTime(fechaEmision.Year, fechaEmision.Month, 1);
                        caracteristicas.FechaFin = caracteristicas.FechaInicio.AddMonths(1).AddDays(-1);
                        if (!string.IsNullOrEmpty(row["P0770_ID_CLASIF_FISCAL"].ToString().Trim()))
                            caracteristicas.TipoDeInmueble = Convert.ToInt32(row["P0770_ID_CLASIF_FISCAL"].ToString());
                        if (!string.IsNullOrEmpty(row["P0730_SUBCONJUNTO"].ToString())) 
                        {
                            caracteristicas.SubConjunto = row["P0730_SUBCONJUNTO"].ToString();
                            bool contratoPorSubConj = Informes.existeContratoPorSubconjunto(caracteristicas.SubConjunto);
                            if (!contratoPorSubConj)
                            {
                                if (!string.IsNullOrEmpty(row["P1922_A_MIN_ING"].ToString().Trim()))
                                    caracteristicas.ValorReferencia = Convert.ToDecimal(row["P1922_A_MIN_ING"].ToString());
                            }
                            else
                            {
                                caracteristicas.ValorReferencia = obtenerValorCatastralDeSubConjunto(caracteristicas.SubConjunto);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(row["P1922_A_MIN_ING"].ToString().Trim()))
                                caracteristicas.ValorReferencia = Convert.ToDecimal(row["P1922_A_MIN_ING"].ToString());
                        }

                        Domicilio domicilio = new Domicilio();
                        domicilio.ReferenciaInterna = row["P0703_NOMBRE"].ToString().Trim();
                        domicilio.Colonia = row["P0504_COLONIA"].ToString().Trim().ToUpper();
                        domicilio.Calle = row["P0503_CALLE_NUM"].ToString().Trim().ToUpper().Replace("#", "").Replace("-", " ").Trim();
                        domicilio.NumeroExterior = row["CAMPO1"].ToString().Trim().ToUpper();
                        domicilio.NumeroInterior = row["CAMPO2"].ToString().Trim().ToUpper().Replace("(", "").Replace(")", "");
                        domicilio.CodigoPostal = row["P0505_COD_POST"].ToString().Trim();
                        caracteristicas.Domicilio = domicilio;
                        operacion.Caracteristicas = new List<CaracteristicasInmueble>();
                        operacion.Caracteristicas.Add(caracteristicas);
                        Liquidacion liquidacion = new Liquidacion();
                        liquidacion.ReferenciaInterna = "CFD " + row["P4006_SERIE"].ToString() + " " + row["P4007_FOLIO"].ToString();
                        liquidacion.FechaDePago = (DateTime)row["P2408_FECHA_PAGADO"];
                        
                        if (!string.IsNullOrEmpty(row["P2457_ID_INSTRUMENTO_PAGO"].ToString().Trim()))
                            liquidacion.ClaveInstrumentoMonetario = Convert.ToInt32(row["P2457_ID_INSTRUMENTO_PAGO"].ToString());                        
                        if (!string.IsNullOrEmpty(row["P2420_MONEDA_PAGO"].ToString().Trim()))
                        {
                            if (row["P2420_MONEDA_PAGO"].ToString() == "D")
                                liquidacion.Moneda = 2;
                            else
                                liquidacion.Moneda = 1;
                        }
                        else
                        {
                            liquidacion.Moneda = 1;
                        }
                        decimal descuento = Convert.ToDecimal(row["P2424_DESCUENTO"].ToString());
                        monedaFactura = row["P2410_MONEDA"].ToString()!=null ? row["P2410_MONEDA"].ToString() : "P";
                        if (!string.IsNullOrEmpty(row["P2405_IMPORTE"].ToString().Trim()))
                        {
                            decimal otrosCargos = string.IsNullOrEmpty(row["CAMPO_NUM6"].ToString()) ? 0 : Convert.ToDecimal(row["CAMPO_NUM6"].ToString());
                            if (liquidacion.Moneda == 1 && monedaFactura == "D")
                            {
                                decimal tcPago = !string.IsNullOrEmpty(row["P2421_TC_PAGO"].ToString()) ? Convert.ToDecimal(row["P2421_TC_PAGO"].ToString()) : 1;                                
                                otrosCargos = otrosCargos * tcPago;
                                //decimal importePesos = Convert.ToDecimal(row["P2413_PAGO"].ToString());// *tcPago;  
                                decimal importePesos = Convert.ToDecimal(row["P2405_IMPORTE"].ToString()) * tcPago;                                
                                decimal monto = importePesos - (otrosCargos + descuento);
                                liquidacion.MontoOperacion = monto;
                            }
                            else if (liquidacion.Moneda == 2 && monedaFactura == "P")
                            {
                                decimal tcPago = !string.IsNullOrEmpty(row["P2421_TC_PAGO"].ToString()) ? Convert.ToDecimal(row["P2421_TC_PAGO"].ToString()) : 1;
                                decimal monto = Convert.ToDecimal(row["P2405_IMPORTE"].ToString()); //Convert.ToDecimal(row["P2413_PAGO"].ToString());                               
                                liquidacion.MontoOperacion = monto - (otrosCargos + descuento);
                            }
                            else
                            {
                                //liquidacion.MontoOperacion = Convert.ToDecimal(row["P2413_PAGO"].ToString()) - otrosCargos;
                                liquidacion.MontoOperacion = Convert.ToDecimal(row["P2405_IMPORTE"].ToString()) - (otrosCargos + descuento);
                            }
                        }
                        liquidacion.FormaDePago = 1;//Por default. Se acordó no considerar pagos parciales
                        operacion.Liquidacion = new List<Liquidacion>();
                        operacion.Liquidacion.Add(liquidacion);
                        operaciones.Add(operacion);
                    }
                }
                catch
                {
                    operaciones = new List<Operacion>();
                }
            }
            return operaciones;
        }

        private List<Operacion> getOperacionesPorCliente(string idCliente, bool esVenta)
        {
            if (esVenta)
            {
                System.Data.DataTable dtFacturasPorCliente = Informes.getVentasPorCliente(idCliente, rfcContribuyente, periodoInicial, periodoFinal);
                List<Operacion> operaciones = new List<Operacion>();
                if (dtFacturasPorCliente.Rows.Count > 0)
                {
                    try
                    {
                        foreach (System.Data.DataRow row in dtFacturasPorCliente.Rows)
                        {
                            Operacion operacion = new Operacion(esVenta);
                            operacion.ReferenciaInterna = " Num. Recibo " + row["P2403_NUM_RECIBO"].ToString();
                            operacion.Fecha = (DateTime)row["P2408_FECHA_PAGADO"];                            
                            if (!string.IsNullOrEmpty(row["MONTOPACTADO"].ToString().Trim()))
                                operacion.ValorPactado = Convert.ToDecimal(row["MONTOPACTADO"].ToString().Trim());
                            CaracteristicasInmueble caracteristicas = new CaracteristicasInmueble();
                            caracteristicas.ReferenciaInterna = row["P0703_NOMBRE"].ToString().Trim();
                            Domicilio domicilio = new Domicilio();
                            domicilio.ReferenciaInterna = row["P0703_NOMBRE"].ToString().Trim();
                            domicilio.Colonia = row["P0504_COLONIA"].ToString().Trim().ToUpper();
                            domicilio.Calle = row["P0503_CALLE_NUM"].ToString().Trim().ToUpper().Replace("#", "").Replace("-", "").Trim();
                            domicilio.NumeroExterior = row["CAMPO1"].ToString().Trim().ToUpper();
                            domicilio.NumeroInterior = row["CAMPO2"].ToString().Trim().ToUpper().Replace("(", "").Replace(")", "");
                            domicilio.CodigoPostal = row["P0505_COD_POST"].ToString().Trim();
                            caracteristicas.Domicilio = domicilio;
                            if (!string.IsNullOrEmpty(row["P0770_ID_CLASIF_FISCAL"].ToString().Trim()))
                                caracteristicas.TipoDeInmueble = Convert.ToInt32(row["P0770_ID_CLASIF_FISCAL"].ToString());                            
                            if (!string.IsNullOrEmpty(row["TERRENO"].ToString().Trim()))
                                caracteristicas.DimensionTerreno = Convert.ToDecimal(row["TERRENO"].ToString().Trim());
                            if (!string.IsNullOrEmpty(row["CONSTRUCCION"].ToString().Trim()))
                                caracteristicas.DimensionConstruccion = Convert.ToDecimal(row["CONSTRUCCION"].ToString().Trim());
                            caracteristicas.FolioReal = row["P0740_CAMPO15"].ToString().Trim().ToUpper();
                            operacion.Caracteristicas = new List<CaracteristicasInmueble>();
                            operacion.Caracteristicas.Add(caracteristicas);
                            operacion.FechaContrato = (DateTime)row["CAMPO_DATE1"];
                            Liquidacion liquidacion = new Liquidacion();
                            liquidacion.ReferenciaInterna = " Num. Recibo " + row["P2403_NUM_RECIBO"].ToString();
                            liquidacion.FechaDePago = (DateTime)row["P2408_FECHA_PAGADO"];
                            /*if (!string.IsNullOrEmpty(row["P0406_FORMA_FACT"].ToString().Trim()))
                                liquidacion.FormaDePago = Convert.ToInt32(row["P0406_FORMA_FACT"].ToString());*/ //Se acordo no incluir pagos parciales
                            liquidacion.FormaDePago = 1;
                            if (!string.IsNullOrEmpty(row["P2457_ID_INSTRUMENTO_PAGO"].ToString().Trim()))
                                liquidacion.ClaveInstrumentoMonetario = Convert.ToInt32(row["P2457_ID_INSTRUMENTO_PAGO"].ToString());                            
                            if (!string.IsNullOrEmpty(row["P0407_MONEDA_FACT"].ToString().Trim()))
                            {
                                if (row["P0407_MONEDA_FACT"].ToString() == "D")
                                    liquidacion.Moneda = 2;
                                else
                                    liquidacion.Moneda = 1;
                            }
                            else
                            {
                                liquidacion.Moneda = 1;
                            }
                            if (!string.IsNullOrEmpty(row["P2405_IMPORTE"].ToString().Trim()))
                                liquidacion.MontoOperacion = Convert.ToDecimal(row["P2405_IMPORTE"].ToString());
                            operacion.Liquidacion = new List<Liquidacion>();
                            operacion.Liquidacion.Add(liquidacion);
                            operaciones.Add(operacion);
                        }
                    }
                    catch
                    {
                        operaciones = new List<Operacion>();
                    }
                }
                return operaciones;
            }
            else
                return getOperacionesPorCliente(idCliente);
        }

        private decimal obtenerValorCatastralDeSubConjunto(string idSubconjunto)
        {
            try
            {
                decimal result = 0;
                System.Data.DataTable dtValorCatastral = Informes.getDTValorCatastralDeSubconjunto(idSubconjunto);
                if (dtValorCatastral.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow row in dtValorCatastral.Rows)
                    {
                        result += Convert.ToDecimal(row["P1922_A_MIN_ING"]);
                    }
                }
                return result;
            }
            catch
            {
                return 0;
            }
        }

        public Informe validaEstructura(Informe informe)
        {
            errores = string.Empty;
            warnings = string.Empty;
            try
            {
                if (informe.MesReportado.Year < 2013)
                {
                    errores += "- No se puede emitir un informe anterior a Octubre del 2013." + Environment.NewLine;
                }
                else
                {
                    if (informe.MesReportado.Year == 2013)
                    {
                        if (informe.MesReportado.Month < 10)
                            errores += "- No se puede emitir un informe anterior a Octubre del 2013." + Environment.NewLine;
                    }
                    if (informe.MesReportado.Year == DateTime.Now.Year)
                    {
                        if (informe.MesReportado.Month > DateTime.Now.Month)
                            errores += "- No se puede emitir un informe con una fecha posterior." + Environment.NewLine;
                    }
                    else
                    {
                        if (DateTime.Now.Year <= informe.MesReportado.Year /*|| informe.MesReportado.Month != 12 quitado para la 2.4.4.5*/)
                            errores += "- No se puede emitir un informe con esa fecha." + Environment.NewLine;
                    }
                }
                if (informe.SujetoObligado.ClaveRFC.Length != 12 && informe.SujetoObligado.ClaveRFC.Length != 13)
                    errores += "- La clave del sujeto obligado no corresponde a un RFC." + Environment.NewLine;
                if (informe.Avisos == null)
                {
                    errores += "- No se encontraron avisos válidos." + Environment.NewLine;
                }
                else
                {
                    foreach (Aviso aviso in informe.Avisos)
                    {
                        if (aviso.Persona == null)
                            errores += "- No se encontró una persona valida relacionada al aviso." + Environment.NewLine;
                        else
                        {
                            if (aviso.Persona.EsPersonaFisica)
                            {
                                if (aviso.Persona.Fisica == null)
                                    errores += "- No se encontró una persona fisica valida relacionada al aviso." + Environment.NewLine;
                                else
                                {
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.Nombre))
                                        errores += "- No se encontró un nombre para la persona fisica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.ApellidoPaterno))
                                        errores += "- No se encontró un apellido paterno para la persona fisica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.ApellidoMaterno))
                                        errores += "- No se encontró un apellido materno para la persona fisica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    if (aviso.Persona.Fisica.FechaNacimiento >= DateTime.Now)
                                        errores += "- La fecha de nacimiento de la persona no es válida. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.RFC))
                                        errores += "- No se encontró un RFC para la persona fisica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    else
                                    {
                                        if (aviso.Persona.Fisica.RFC.Length != 13)
                                            errores += "- El dato " + aviso.Persona.Fisica.RFC + " no corresponde a un RFC válido. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    }
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.CURP))
                                        warnings += "- No se encontró el atributo opcional CURP para la persona fisica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    else
                                    {
                                        if (aviso.Persona.Fisica.CURP.Length != 18)
                                        {
                                            warnings += " - El dato " + aviso.Persona.Fisica.CURP + " no corresponde a una CURP válida, se omitirádado que es opcional. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                            aviso.Persona.Fisica.CURP = string.Empty;
                                        }
                                    }
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.ActividadEconomica))
                                        errores += "- No se encontró una actividad económica para la persona fisica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    else
                                    {
                                        if (aviso.Persona.Fisica.ActividadEconomica.Length != 7)
                                            errores += "- La actividad económica seleccionada no es válida. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                        else
                                        {
                                            try
                                            {
                                                Convert.ToInt32(aviso.Persona.Fisica.ActividadEconomica);
                                            }
                                            catch
                                            {
                                                errores += "- La actividad económica seleccionada debe ser numérica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                            }
                                        }
                                    }                                    
                                }
                            }
                            else
                            {
                                if (aviso.Persona.Moral == null)
                                    errores += "- No se encontró una persona moral válida relacionada al aviso." + Environment.NewLine;
                                else
                                {
                                    if (string.IsNullOrEmpty(aviso.Persona.Moral.RazonSocial))
                                        errores += "- No se encontró la razón social para la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                    if (aviso.Persona.Moral.FechaDeConstitucion >= DateTime.Now)
                                        errores += "- La fecha de constitución de la persona moral es incorrecta. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                    if (string.IsNullOrEmpty(aviso.Persona.Moral.RFC))
                                        errores += "- No se encontró un RFC para la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                    else
                                    {
                                        if (aviso.Persona.Moral.RFC.Length != 12)
                                            errores += "- El dato " + aviso.Persona.Moral.RFC + " no corresponde a un RFC válido. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                    }
                                    if (string.IsNullOrEmpty(aviso.Persona.Moral.Giro))
                                        errores += "- No se encontró un giro económico para la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                    else
                                    {
                                        if (aviso.Persona.Moral.Giro.Length != 7)
                                            errores += "- El giro económico seleccionado no es válido. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        else
                                        {
                                            try
                                            {
                                                Convert.ToInt32(aviso.Persona.Moral.Giro);
                                            }
                                            catch
                                            {
                                                errores += "- El giro económico seleccionado debe ser numérico. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                            }
                                        }
                                    }
                                    if (aviso.Persona.Moral.Apoderado == null)
                                        errores += "- Debe existir un apoderado legal para la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                    else
                                    {
                                        if (string.IsNullOrEmpty(aviso.Persona.Moral.Apoderado.Nombre))
                                            errores += "- No se encontró un nombre para el representante legal de la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        if (string.IsNullOrEmpty(aviso.Persona.Moral.Apoderado.ApellidoPaterno))
                                            errores += "- No se encontró un apellido paterno para el representante legal de la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        if (string.IsNullOrEmpty(aviso.Persona.Moral.Apoderado.ApellidoMaterno))
                                            errores += "- No se encontró un apellido materno para el representante legal de la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        if (aviso.Persona.Moral.Apoderado.FechaNacimiento >= DateTime.Now)
                                            errores += "- La fecha de nacimiento del representante legal no es válida. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        if (string.IsNullOrEmpty(aviso.Persona.Moral.Apoderado.RFC))
                                            errores += "- No se encontró un RFC para el representante legal de la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        else
                                        {
                                            if (aviso.Persona.Moral.Apoderado.RFC.Length != 13)
                                                errores += "- El dato " + aviso.Persona.Moral.Apoderado.RFC + " no corresponde a un RFC válido. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        }
                                        if (string.IsNullOrEmpty(aviso.Persona.Moral.Apoderado.CURP))
                                            warnings += "- No se encontró el atributo opcional CURP para el apoderado de la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        else
                                        {
                                            if (aviso.Persona.Moral.Apoderado.CURP.Length != 18)
                                            {
                                                warnings += " - El dato " + aviso.Persona.Moral.Apoderado.CURP + " no corresponde a una CURP válida para el apoderado, se omitirá dado que es opcional. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                                aviso.Persona.Moral.Apoderado.CURP = string.Empty;
                                            }
                                        }                                        
                                    }
                                }
                            }
                            if (aviso.Persona.DomicilioNacional == null)
                                errores += "- Debe existir un domicilio para la persona del aviso." + Environment.NewLine;
                            else
                            {
                                if (string.IsNullOrEmpty(aviso.Persona.DomicilioNacional.Colonia))
                                    errores += "- No se encontró la colonia para la persona del aviso. Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                if (string.IsNullOrEmpty(aviso.Persona.DomicilioNacional.Calle))
                                    errores += "- No se encontró la calle para la persona del aviso. Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                if (string.IsNullOrEmpty(aviso.Persona.DomicilioNacional.NumeroExterior))
                                    errores += "- No se encontró el número exterior para la persona del aviso. Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                if (string.IsNullOrEmpty(aviso.Persona.DomicilioNacional.NumeroInterior))
                                    warnings += "- No se encontró el campo opcional numero interior para la persona del aviso. Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                if (string.IsNullOrEmpty(aviso.Persona.DomicilioNacional.CodigoPostal))
                                    errores += "- No se encontró el código postal para la persona del aviso. Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                else
                                {
                                    if (aviso.Persona.DomicilioNacional.CodigoPostal.Length != 5)
                                        errores += "- El código postal debe ser de 5 números.  Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                    else
                                    {
                                        try
                                        {
                                            Convert.ToInt32(aviso.Persona.DomicilioNacional.CodigoPostal);
                                        }
                                        catch
                                        {
                                            errores += "- El código postal solo debe contener números. Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                        }
                                    }
                                }
                            }
                            if (aviso.Persona.Telefono == null)
                                errores += "- Debe existir un telefono para la persona del aviso. Referencia: " + aviso.Persona.Telefono.ReferenciaInterna + Environment.NewLine;
                            else
                            {
                                if (string.IsNullOrEmpty(aviso.Persona.Telefono.Numero))
                                    errores += "- No se encontró un número telefónico para la persona del aviso. Referencia: " + aviso.Persona.Telefono.ReferenciaInterna + Environment.NewLine;
                                else
                                {
                                    if (aviso.Persona.Telefono.Numero.Length < 10 || aviso.Persona.Telefono.Numero.Length > 12)
                                        errores += "- El número telefónico para la persona del aviso debe constar de entre 10 y 12 digitos. Referencia: " + aviso.Persona.Telefono.ReferenciaInterna + Environment.NewLine;
                                    else
                                    {
                                        try
                                        {
                                            Convert.ToInt64(aviso.Persona.Telefono.Numero);
                                        }
                                        catch
                                        {
                                            errores += "- El número telefónico debe contener solo caracteres númericos. Referencia: " + aviso.Persona.Telefono.ReferenciaInterna + Environment.NewLine;
                                        }
                                    }
                                }
                                if (string.IsNullOrEmpty(aviso.Persona.Telefono.EMail))
                                    warnings += "- No se encontró el campo e-mail para la persona del aviso. El archivo se generará sin el mismo dado que es opcional. Referencia: " + aviso.Persona.Telefono.ReferenciaInterna + Environment.NewLine;
                                else
                                {
                                    if (!aviso.Persona.Telefono.EMail.Contains("@") || !aviso.Persona.Telefono.EMail.Contains("."))
                                    {
                                        warnings += "- El e-mail para la persona del aviso no es válido. El archivo se generará sin el mismo dado que es opcional.  Referencia: " + aviso.Persona.Telefono.ReferenciaInterna + Environment.NewLine;
                                        aviso.Persona.Telefono.EMail = string.Empty;
                                    }
                                }
                            }
                        }
                        if (!esVenta)
                        {
                            #region Arrendamiento
                            if (aviso.Operaciones.Count <= 0)
                                errores += "- Debe existir al menos una operación en el aviso." + Environment.NewLine;
                            else
                            {
                                foreach (Operacion operacion in aviso.Operaciones)
                                {
                                    if (operacion.Fecha.Year < 2013)
                                    {
                                        errores += "- La fecha de operacion no puede ser anterior a Octubre del 2013. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                    }
                                    else
                                    {
                                        if (operacion.Fecha.Year == 2013)
                                        {
                                            if (operacion.Fecha.Month < 10)
                                                errores += "- La fecha de operacion no puede ser anterior a Octubre del 2013. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                        if (operacion.Fecha.Year == DateTime.Now.Year)
                                        {
                                            if (operacion.Fecha.Month > DateTime.Now.Month)
                                                errores += "- No se puede emitir una operacion con una fecha de operacion posterior. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                        else
                                        {
                                            if (DateTime.Now.Year <= operacion.Fecha.Year /*|| operacion.Fecha.Month != 12*/)
                                                errores += "- No se puede emitir una operacion con esa fecha de operacion. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                    }
                                    if (operacion.Caracteristicas== null)
                                        errores += "- No se encontraron los datos para las caracteristicas del inmueble." + Environment.NewLine;
                                    else
                                    {
                                        if (operacion.Caracteristicas.Count == 0)
                                            errores += "- Debe existir al menos un dato para la caracteristica del arrendamiento." + Environment.NewLine;
                                        foreach (var c in operacion.Caracteristicas)
                                        {
                                            if (c.TipoDeInmueble.ToString().Length < 1 || c.TipoDeInmueble.ToString().Length > 3)
                                                errores += "- El tipo de inmueble debe conformarse por un rango entre 1 y 3 caracteres. Referencia: " + c.ReferenciaInterna + Environment.NewLine;
                                            else
                                            {
                                                try
                                                {
                                                    if (Convert.ToInt32(c.TipoDeInmueble) <= 0)
                                                        errores += "- Debe existir un tipo de inmueble válido. Referencia: " + c.ReferenciaInterna + Environment.NewLine;
                                                }
                                                catch
                                                {
                                                    errores += "- El tipo de inmueble solo permite caracteres númericos. Referencia: " + c.ReferenciaInterna + Environment.NewLine;
                                                }
                                            }
                                            if (c.ValorReferencia <= 0)
                                                errores += "- El valor de referencia del inmueble debe ser mayor a 0.00. Referencia: " + c.ReferenciaInterna + Environment.NewLine;
                                            if (string.IsNullOrEmpty(c.FolioReal))
                                                errores += "- El folio real del registro publico del inmueble es obligatorio. Referencia: " + c.ReferenciaInterna + Environment.NewLine;
                                            if (c.Domicilio == null)
                                                errores += "- Debe existir un domicilio para el inmueble de la operación. Referencia: " + c.ReferenciaInterna + Environment.NewLine;
                                            else
                                            {
                                                if (string.IsNullOrEmpty(c.Domicilio.Colonia))
                                                    errores += "- No se encontró la colonia para el inmueble de la operación. Referencia: " + c.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                if (string.IsNullOrEmpty(c.Domicilio.Calle))
                                                    errores += "- No se encontró la calle para el inmueble de la operación. Referencia: " + c.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                if (string.IsNullOrEmpty(c.Domicilio.NumeroExterior))
                                                    errores += "- No se encontró el número exterior para el inmueble de la operación. Referencia: " + c.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                if (string.IsNullOrEmpty(c.Domicilio.NumeroInterior))
                                                    warnings += "- No se encontró el campo opcional número interior para el inmueble de la operación. Referencia: " + c.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                if (string.IsNullOrEmpty(c.Domicilio.CodigoPostal))
                                                    errores += "- No se encontró el código postal para el inmueble de la operación. Referencia: " + c.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                else
                                                {
                                                    if (c.Domicilio.CodigoPostal.Length != 5)
                                                        errores += "- El código postal del inmueble debe ser de 5 números. Referencia: " + c.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                    else
                                                    {
                                                        try
                                                        {
                                                            Convert.ToInt32(c.Domicilio.CodigoPostal);
                                                        }
                                                        catch
                                                        {
                                                            errores += "- El código postal del inmueble solo debe contener números. Referencia: " + c.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (operacion.Liquidacion == null)
                                        errores += "- No se encontraron los datos para la liquidación de la operación." + Environment.NewLine;
                                    else
                                    {
                                        if (operacion.Liquidacion.Count == 0)
                                            errores += "- Debe existir al menos un dato para la liquidación de la operación." + Environment.NewLine;
                                        foreach (var l in operacion.Liquidacion)
                                        {
                                            if (l.FechaDePago.Year < 2013)
                                            {
                                                errores += "- La fecha de liquidación no puede ser anterior a Octubre del 2013. Referencia: " + l.ReferenciaInterna + Environment.NewLine;
                                            }
                                            else
                                            {
                                                if (l.FechaDePago.Year == 2013)
                                                {
                                                    if (l.FechaDePago.Month < 10)
                                                        errores += "- La fecha de liquidación no puede ser anterior a Octubre del 2013. Referencia: " + l.ReferenciaInterna + Environment.NewLine;
                                                }
                                                if (l.FechaDePago.Year == DateTime.Now.Year)
                                                {
                                                    if (l.FechaDePago.Month > DateTime.Now.Month)
                                                        errores += "- No se puede liquidar una operacion en una fecha posterior. Referencia: " + l.ReferenciaInterna + Environment.NewLine;
                                                }
                                                else
                                                {
                                                    if (DateTime.Now.Year <= l.FechaDePago.Year /*|| operacion.Liquidacion.FechaDePago.Month != 12*/)
                                                        errores += "- No se puede liquidar una operacion en esa fecha. Referencia: " + l.ReferenciaInterna + Environment.NewLine;
                                                }
                                            }
                                            if (l.ClaveInstrumentoMonetario < 1 || l.ClaveInstrumentoMonetario > 14)
                                                errores += "- No se reconoce el instrumento monetario de pago. Referencia: " + l.ReferenciaInterna + Environment.NewLine;                                            
                                            if (l.MontoOperacion <= 0)
                                                errores += "- Solo se permiten montos de operacion mayores a cero. Referencia: " + l.ReferenciaInterna + Environment.NewLine;
                                            if(l.FormaDePago <= 0 || l.FormaDePago > 4)
                                                errores += "- No se identificó la forma de pago. Referencia: " + l.ReferenciaInterna + Environment.NewLine;
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region Antilavado de Venta
                            if (aviso.Operaciones.Count <= 0)
                                errores += "- Debe existir al menos una operación en el aviso." + Environment.NewLine;
                            else
                            {
                                foreach (Operacion operacion in aviso.Operaciones)
                                {
                                    if (operacion.Fecha.Year < 2013)
                                    {
                                        errores += "- La fecha de operacion no puede ser anterior a Octubre del 2013. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                    }
                                    else
                                    {
                                        if (operacion.Fecha.Year == 2013)
                                        {
                                            if (operacion.Fecha.Month < 10)
                                                errores += "- La fecha de operacion no puede ser anterior a Octubre del 2013. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                        if (operacion.Fecha.Year == DateTime.Now.Year)
                                        {
                                            if (operacion.Fecha.Month > DateTime.Now.Month)
                                                errores += "- No se puede emitir una operacion con una fecha de operacion posterior. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                        else
                                        {
                                            if (DateTime.Now.Year <= operacion.Fecha.Year || operacion.Fecha.Month != 12)
                                                errores += "- No se puede emitir una operacion con esa fecha de operacion. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                    }
                                    if(operacion.FechaContrato > DateTime.Now.Date)
                                        errores += "- La fecha de contrato debe ser menor a la fecha actual. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                    if(operacion.ValorPactado <= 0)
                                        errores += "- El valor pactado debe ser superior a cero. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                    if(operacion.Caracteristicas == null)
                                        errores += "- No se encontraron los datos para las caracteristicas del inmueble. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                    else
                                    {
                                        if(operacion.Caracteristicas.Count <= 0)
                                            errores += "- No se encontraron caracteristicas del inmueble. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                        else
                                        {
                                            foreach(var inmueble in operacion.Caracteristicas)
                                            {
                                                if(inmueble.TipoDeInmueble > 100 || inmueble.TipoDeInmueble <= 0)
                                                    errores += "- El tipo del inmueble es inválido. Referencia: " + inmueble.ReferenciaInterna + Environment.NewLine;
                                                if (string.IsNullOrEmpty(inmueble.FolioReal))
                                                    errores += "- El folio real del registro publico del inmueble es obligatorio. Referencia: " + inmueble.ReferenciaInterna + Environment.NewLine;
                                                if(inmueble.DimensionTerreno == 0)
                                                    warnings += "- La dimensión del terreno es cero. Referencia: " + inmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                if(inmueble.DimensionConstruccion == 0)
                                                    warnings += "- La dimensión de construcción es cero. Referencia: " + inmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                if (inmueble.Domicilio == null)
                                                    errores += "- Debe existir un domicilio para el inmueble de la operación. Referencia: " + inmueble.ReferenciaInterna + Environment.NewLine;
                                                else
                                                {
                                                    if (string.IsNullOrEmpty(inmueble.Domicilio.Colonia))
                                                        errores += "- No se encontró la colonia para el inmueble de la operación. Referencia: " + inmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                    if (string.IsNullOrEmpty(inmueble.Domicilio.Calle))
                                                        errores += "- No se encontró la calle para el inmueble de la operación. Referencia: " + inmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                    if (string.IsNullOrEmpty(inmueble.Domicilio.NumeroExterior))
                                                        errores += "- No se encontró el número exterior para el inmueble de la operación. Referencia: " + inmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                    if (string.IsNullOrEmpty(inmueble.Domicilio.NumeroInterior))
                                                        warnings += "- No se encontró el campo opcional número interior para el inmueble de la operación. Referencia: " + inmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                    if (string.IsNullOrEmpty(inmueble.Domicilio.CodigoPostal))
                                                        errores += "- No se encontró el código postal para el inmueble de la operación. Referencia: " + inmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                    else
                                                    {
                                                        if (inmueble.Domicilio.CodigoPostal.Length != 5)
                                                            errores += "- El código postal del inmueble debe ser de 5 números. Referencia: " + inmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                        else
                                                        {
                                                            try
                                                            {
                                                                Convert.ToInt32(inmueble.Domicilio.CodigoPostal);
                                                            }
                                                            catch
                                                            {
                                                                errores += "- El código postal del inmueble solo debe contener números. Referencia: " + inmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (operacion.Liquidacion == null)
                                        errores += "- No se encontraron los datos para la liquidación de la operación." + Environment.NewLine;
                                    else
                                    {
                                        if (operacion.Liquidacion.Count <= 0)
                                            errores += "- No se encontraron liquidaciones de la operación." + Environment.NewLine;
                                        else
                                        {
                                            foreach (var liquidacion in operacion.Liquidacion)
                                            {
                                                if (liquidacion.FechaDePago.Year < 2013)                                        
                                                    errores += "- La fecha de liquidación no puede ser anterior a Octubre del 2013. Referencia: " + liquidacion.ReferenciaInterna + Environment.NewLine;
                                                else
                                                {
                                                    if (liquidacion.FechaDePago.Year == 2013)
                                                    {
                                                        if (liquidacion.FechaDePago.Month < 10)
                                                            errores += "- La fecha de liquidación no puede ser anterior a Octubre del 2013. Referencia: " + liquidacion.ReferenciaInterna + Environment.NewLine;
                                                    }
                                                    if (liquidacion.FechaDePago.Year == DateTime.Now.Year)
                                                    {
                                                        if (liquidacion.FechaDePago.Month > DateTime.Now.Month)
                                                            errores += "- No se puede liquidar una operacion en una fecha posterior. Referencia: " + liquidacion.ReferenciaInterna + Environment.NewLine;
                                                    }
                                                    else
                                                    {
                                                        if (DateTime.Now.Year <= liquidacion.FechaDePago.Year || liquidacion.FechaDePago.Month != 12)
                                                            errores += "- No se puede liquidar una operacion en esa fecha. Referencia: " + liquidacion.ReferenciaInterna + Environment.NewLine;
                                                    }
                                                }
                                                if (liquidacion.FormaDePago <= 0)
                                                    errores += "- No se encontró una forma de pago correcta" + liquidacion.ReferenciaInterna + Environment.NewLine;
                                                if (liquidacion.ClaveInstrumentoMonetario < 1 || liquidacion.ClaveInstrumentoMonetario > 15)
                                                    errores += "- No se reconoce el instrumento monetario de pago. Referencia: " + liquidacion.ReferenciaInterna + Environment.NewLine;
                                                if (liquidacion.MontoOperacion <= 0)
                                                    errores += "- Solo se permiten montos de operacion mayores a cero. Referencia: " + liquidacion.ReferenciaInterna + Environment.NewLine;
                                                if(liquidacion.Moneda < 1 || liquidacion.Moneda > 184)
                                                    errores += "- La moneda es incorrecta. Referencia: " + liquidacion.ReferenciaInterna + Environment.NewLine;
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            catch
            {
                errores += "- Ocurrio un error inesperado al momento de validar la estructura del informe.";
            }
            return informe;
        }

        public void cancelar()
        {
            cancelacionPendiente = true;
        }

        protected virtual void OnCambioProgreso(int progreso)
        {
            var handler = CambioProgreso;
            if (handler != null)
                handler(this, new CambioProgresoEventArgs(progreso <= 100 ? progreso : 100));
        }
    }
}
