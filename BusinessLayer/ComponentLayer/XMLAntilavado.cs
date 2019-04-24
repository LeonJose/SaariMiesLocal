using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Xml;
using System.Linq;
using GestorReportes.BusinessLayer.EntitiesAntiLavado;
using GestorReportes.BusinessLayer.DataAccessLayer;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class XMLAntilavado
    {
        public List<AlertaPorPersona> alertasPorPersonas = new List<AlertaPorPersona>();
        private string RFC = string.Empty;
        private bool EsEnCeros { get; set; }
        private bool EsVenta { get; set; }
        int numOperacion = 1;
        int numAviso = 1;
        DataSet dsAntilavado = new DataSet("archivo");

        #region Constructores
        public XMLAntilavado(string rfc)
        {
            this.RFC = rfc;
        }

        public XMLAntilavado()
        {
        }
        #endregion

        #region Generación de representación impresa antilavado V2  
        public GestorReportes.BusinessLayer.EntitiesAntilavado2.Informe leerXml(string filename, bool esV2)
        {            
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filename);
                XmlNodeList archivos = xmlDoc.GetElementsByTagName("archivo");
                if (archivos.Count > 0)
                {
                    XmlNodeList informes = ((XmlElement)archivos[0]).GetElementsByTagName("informe");
                    if (informes.Count > 0)
                    {
                        GestorReportes.BusinessLayer.EntitiesAntilavado2.Informe informe = new GestorReportes.BusinessLayer.EntitiesAntilavado2.Informe();
                        XmlNode mesInforme = ((XmlElement)informes[0]).GetElementsByTagName("mes_reportado")[0];
                        if (mesInforme != null)
                        {
                            informe.MesReportado = new DateTime(Convert.ToInt32(mesInforme.InnerText.Substring(0, 4)), Convert.ToInt32(mesInforme.InnerText.Substring(4, 2)), 1);
                            XmlNodeList sujetosObligados = ((XmlElement)informes[0]).GetElementsByTagName("sujeto_obligado");
                            if (sujetosObligados.Count > 0)
                            {
                                GestorReportes.BusinessLayer.EntitiesAntilavado2.SujetoObligado sujeto = new GestorReportes.BusinessLayer.EntitiesAntilavado2.SujetoObligado();
                                sujeto.ClaveRFC = ((XmlElement)sujetosObligados[0]).GetElementsByTagName("clave_sujeto_obligado")[0].InnerText;
                                sujeto.Actividad = ((XmlElement)sujetosObligados[0]).GetElementsByTagName("clave_actividad")[0].InnerText;
                                informe.SujetoObligado = sujeto;
                                XmlNodeList avisos = ((XmlElement)informes[0]).GetElementsByTagName("aviso");
                                if (avisos.Count > 0)
                                {
                                    List<GestorReportes.BusinessLayer.EntitiesAntilavado2.Aviso> listaAvisos = new List<GestorReportes.BusinessLayer.EntitiesAntilavado2.Aviso>();
                                    foreach (XmlNode avis in avisos)
                                    {
                                        GestorReportes.BusinessLayer.EntitiesAntilavado2.Aviso aviso = leeAviso(avis, esV2);
                                        if (aviso == null)
                                            return null;
                                        else
                                            listaAvisos.Add(aviso);
                                    }
                                    informe.Avisos = listaAvisos;
                                }
                                return informe;
                            }
                            else
                                return null;                            
                        }
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        private GestorReportes.BusinessLayer.EntitiesAntilavado2.Aviso leeAviso(XmlNode nodoAviso, bool esV2)
        {
            try
            {
                GestorReportes.BusinessLayer.EntitiesAntilavado2.Aviso aviso = new GestorReportes.BusinessLayer.EntitiesAntilavado2.Aviso();
                aviso.Referencia = Convert.ToInt32(((XmlElement)nodoAviso).GetElementsByTagName("referencia_aviso")[0].InnerText);
                aviso.Prioridad = Convert.ToInt32(((XmlElement)nodoAviso).GetElementsByTagName("prioridad")[0].InnerText);
                XmlNode nodeAlerta = ((XmlElement)nodoAviso).GetElementsByTagName("alerta")[0];
                GestorReportes.BusinessLayer.EntitiesAntilavado2.Alerta alert = new GestorReportes.BusinessLayer.EntitiesAntilavado2.Alerta();
                alert.Tipo = Convert.ToInt32(((XmlElement)nodeAlerta).GetElementsByTagName("tipo_alerta")[0].InnerText);
                aviso.Alerta = alert;
                XmlNode personaAviso = ((XmlElement)nodoAviso).GetElementsByTagName("persona_aviso")[0];
                aviso.Persona = leePersona(personaAviso, esV2);
                if (aviso.Persona == null)
                    return null;
                XmlNode detalleOperaciones = ((XmlElement)nodoAviso).GetElementsByTagName("detalle_operaciones")[0];
                aviso.Operaciones = leeOperaciones(detalleOperaciones, esV2);
                if (aviso.Operaciones == null)
                    return null;
                return aviso;
            }
            catch
            {
                return null;
            }
        }

        private GestorReportes.BusinessLayer.EntitiesAntilavado2.Persona leePersona(XmlNode nodoPersonaAviso, bool esV2)
        {
            try
            {
                XmlNode tipoPersona = ((XmlElement)nodoPersonaAviso).GetElementsByTagName("tipo_persona")[0];
                GestorReportes.BusinessLayer.EntitiesAntilavado2.Persona persona = new GestorReportes.BusinessLayer.EntitiesAntilavado2.Persona();
                XmlNodeList nodeType = ((XmlElement)tipoPersona).GetElementsByTagName("persona_moral");
                if (nodeType.Count > 0)
                {
                    persona.EsPersonaFisica = false;
                    GestorReportes.BusinessLayer.EntitiesAntilavado2.PersonaMoral moral = new GestorReportes.BusinessLayer.EntitiesAntilavado2.PersonaMoral();
                    moral.RazonSocial = ((XmlElement)nodeType[0]).GetElementsByTagName("denominacion_razon")[0].InnerText;
                    string fechaConstString = ((XmlElement)nodeType[0]).GetElementsByTagName("fecha_constitucion")[0].InnerText;
                    moral.FechaDeConstitucion = new DateTime(Convert.ToInt32(fechaConstString.Substring(0, 4)), Convert.ToInt32(fechaConstString.Substring(4, 2)), Convert.ToInt32(fechaConstString.Substring(6, 2)));
                    moral.RFC = ((XmlElement)nodeType[0]).GetElementsByTagName("rfc")[0].InnerText;
                    moral.PaisNacionalidad = ((XmlElement)nodeType[0]).GetElementsByTagName("pais_nacionalidad")[0].InnerText;
                    moral.Giro = ((XmlElement)nodeType[0]).GetElementsByTagName("giro_mercantil")[0].InnerText;
                    XmlNodeList nodoRepresentante = ((XmlElement)nodeType[0]).GetElementsByTagName("representante_apoderado");
                    GestorReportes.BusinessLayer.EntitiesAntilavado2.PersonaFisica representante = new GestorReportes.BusinessLayer.EntitiesAntilavado2.PersonaFisica();
                    representante.Nombre = ((XmlElement)nodoRepresentante[0]).GetElementsByTagName("nombre")[0].InnerText;
                    representante.ApellidoPaterno = ((XmlElement)nodoRepresentante[0]).GetElementsByTagName("apellido_paterno")[0].InnerText;
                    representante.ApellidoMaterno = ((XmlElement)nodoRepresentante[0]).GetElementsByTagName("apellido_materno")[0].InnerText;
                    string fechaNac = ((XmlElement)nodoRepresentante[0]).GetElementsByTagName("fecha_nacimiento")[0].InnerText;
                    representante.FechaNacimiento = new DateTime(Convert.ToInt32(fechaNac.Substring(0,4)), Convert.ToInt32(fechaNac.Substring(4,2)), Convert.ToInt32(fechaNac.Substring(6,2)));
                    representante.RFC = ((XmlElement)nodoRepresentante[0]).GetElementsByTagName("rfc")[0].InnerText;                    
                    moral.Apoderado = representante;
                    persona.Moral = moral;                    
                }
                else
                {
                    nodeType = ((XmlElement)tipoPersona).GetElementsByTagName("persona_fisica");
                    if (nodeType.Count > 0)
                    {
                        persona.EsPersonaFisica = true;
                        GestorReportes.BusinessLayer.EntitiesAntilavado2.PersonaFisica fisica = new GestorReportes.BusinessLayer.EntitiesAntilavado2.PersonaFisica();
                        fisica.Nombre = ((XmlElement)nodeType[0]).GetElementsByTagName("nombre")[0].InnerText;
                        fisica.ApellidoPaterno = ((XmlElement)nodeType[0]).GetElementsByTagName("apellido_paterno")[0].InnerText;
                        fisica.ApellidoMaterno = ((XmlElement)nodeType[0]).GetElementsByTagName("apellido_materno")[0].InnerText;
                        string fechaNac = ((XmlElement)nodeType[0]).GetElementsByTagName("fecha_nacimiento")[0].InnerText;
                        fisica.FechaNacimiento = new DateTime(Convert.ToInt32(fechaNac.Substring(0, 4)), Convert.ToInt32(fechaNac.Substring(4, 2)), Convert.ToInt32(fechaNac.Substring(6, 2)));
                        fisica.RFC = ((XmlElement)nodeType[0]).GetElementsByTagName("rfc")[0].InnerText;
                        fisica.PaisNacionalidad = ((XmlElement)nodeType[0]).GetElementsByTagName("pais_nacionalidad")[0].InnerText;
                        fisica.ActividadEconomica = ((XmlElement)nodeType[0]).GetElementsByTagName("actividad_economica")[0].InnerText;
                        persona.Fisica = fisica;
                    }
                    else
                        return null;
                }
                XmlNode domic = ((XmlElement)nodoPersonaAviso).GetElementsByTagName("tipo_domicilio")[0];
                persona.DomicilioNacional = leeDomicilio(domic, esV2);
                if (persona.DomicilioNacional == null)
                    return null;
                XmlNode tel = ((XmlElement)nodoPersonaAviso).GetElementsByTagName("telefono")[0];
                GestorReportes.BusinessLayer.EntitiesAntilavado2.Telefono telefono = new GestorReportes.BusinessLayer.EntitiesAntilavado2.Telefono();
                telefono.Numero = ((XmlElement)tel).GetElementsByTagName("numero_telefono")[0].InnerText;
                telefono.Pais = ((XmlElement)tel).GetElementsByTagName("clave_pais")[0].InnerText;
                persona.Telefono = telefono;
                return persona;
            }
            catch
            {
                return null;
            }
        }

        private GestorReportes.BusinessLayer.EntitiesAntilavado2.Domicilio leeDomicilio(XmlNode nodoDomicilio, bool esV2)
        {
            try
            {
                GestorReportes.BusinessLayer.EntitiesAntilavado2.Domicilio domicilio = new GestorReportes.BusinessLayer.EntitiesAntilavado2.Domicilio();
                XmlNode nacional = ((XmlElement)nodoDomicilio).GetElementsByTagName("nacional")[0];
                domicilio.Colonia = ((XmlElement)nacional).GetElementsByTagName("colonia")[0].InnerText;
                domicilio.Calle = ((XmlElement)nacional).GetElementsByTagName("calle")[0].InnerText;
                domicilio.NumeroExterior = ((XmlElement)nacional).GetElementsByTagName("numero_exterior")[0].InnerText;
                domicilio.CodigoPostal = ((XmlElement)nacional).GetElementsByTagName("codigo_postal")[0].InnerText;
                return domicilio;
            }
            catch
            {
                return null;
            }
        }

        private List<GestorReportes.BusinessLayer.EntitiesAntilavado2.Operacion> leeOperaciones(XmlNode nodoDetalleOperaciones, bool esV2)
        {
            try
            {
                //XmlNode operacionesRealizadas = ((XmlElement)nodoDetalleOperaciones).GetElementsByTagName("operaciones_realizadas")[0];
                XmlNodeList datosOperacion = ((XmlElement)nodoDetalleOperaciones).GetElementsByTagName("datos_operacion");
                if (datosOperacion.Count > 0)
                {
                    List<GestorReportes.BusinessLayer.EntitiesAntilavado2.Operacion> operaciones = new List<GestorReportes.BusinessLayer.EntitiesAntilavado2.Operacion>();
                    foreach (XmlNode nodoOperacion in datosOperacion)
                    {
                        GestorReportes.BusinessLayer.EntitiesAntilavado2.Operacion operacion = new GestorReportes.BusinessLayer.EntitiesAntilavado2.Operacion(false);
                        string fechaOper = ((XmlElement)nodoOperacion).GetElementsByTagName("fecha_operacion")[0].InnerText;
                        operacion.Fecha = new DateTime(Convert.ToInt32(fechaOper.Substring(0, 4)), Convert.ToInt32(fechaOper.Substring(4, 2)), Convert.ToInt32(fechaOper.Substring(6, 2)));
                        operacion.Tipo = Convert.ToInt32(((XmlElement)nodoOperacion).GetElementsByTagName("tipo_operacion")[0].InnerText);
                        string fechaIn = ((XmlElement)nodoOperacion).GetElementsByTagName("fecha_inicio")[0].InnerText;
                        string fechaFin = ((XmlElement)nodoOperacion).GetElementsByTagName("fecha_termino")[0].InnerText;
                        XmlNode nodoCarac = ((XmlElement)nodoOperacion).GetElementsByTagName("caracteristicas")[0];
                        operacion.Caracteristicas = leeCaracteristicas(nodoCarac, esV2);
                        if (operacion.Caracteristicas == null)
                            return null;
                        XmlNode nodoLiq = ((XmlElement)nodoOperacion).GetElementsByTagName("datos_liquidacion")[0];
                        operacion.Liquidacion = leeLiquidacion(nodoLiq, esV2);
                        if (operacion.Liquidacion == null)
                            return null;
                        operaciones.Add(operacion);
                    }
                    return operaciones;
                }
                else 
                    return null;
            }
            catch
            {
                return null;
            }
        }

        private List<GestorReportes.BusinessLayer.EntitiesAntilavado2.CaracteristicasInmueble> leeCaracteristicas(XmlNode nodoCaracteristicas, bool esV2)
        {
            try
            {
                GestorReportes.BusinessLayer.EntitiesAntilavado2.CaracteristicasInmueble carac = new GestorReportes.BusinessLayer.EntitiesAntilavado2.CaracteristicasInmueble();
                carac.TipoDeInmueble = Convert.ToInt32(((XmlElement)nodoCaracteristicas).GetElementsByTagName("tipo_inmueble")[0].InnerText);
                carac.ValorReferencia = Convert.ToDecimal(((XmlElement)nodoCaracteristicas).GetElementsByTagName("valor_referencia")[0].InnerText);
                carac.FechaInicio = DateTime.ParseExact(((XmlElement)nodoCaracteristicas).GetElementsByTagName("fecha_inicio")[0].InnerText, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                carac.FechaFin = DateTime.ParseExact(((XmlElement)nodoCaracteristicas).GetElementsByTagName("fecha_termino")[0].InnerText, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                GestorReportes.BusinessLayer.EntitiesAntilavado2.Domicilio domcInm = new GestorReportes.BusinessLayer.EntitiesAntilavado2.Domicilio();
                domcInm.Colonia = ((XmlElement)nodoCaracteristicas).GetElementsByTagName("colonia")[0].InnerText;
                domcInm.Calle = ((XmlElement)nodoCaracteristicas).GetElementsByTagName("calle")[0].InnerText;
                domcInm.NumeroExterior = ((XmlElement)nodoCaracteristicas).GetElementsByTagName("numero_exterior")[0].InnerText;
                domcInm.CodigoPostal = ((XmlElement)nodoCaracteristicas).GetElementsByTagName("codigo_postal")[0].InnerText;
                carac.Domicilio = domcInm;
                carac.FolioReal = ((XmlElement)nodoCaracteristicas).GetElementsByTagName("folio_real")[0].InnerText;
                return new List<GestorReportes.BusinessLayer.EntitiesAntilavado2.CaracteristicasInmueble>() { carac };
            }
            catch
            {
                return null;
            }
        }

        private List<GestorReportes.BusinessLayer.EntitiesAntilavado2.Liquidacion> leeLiquidacion(XmlNode nodoLiquidacion, bool esV2)
        {
            try
            {
                GestorReportes.BusinessLayer.EntitiesAntilavado2.Liquidacion liquidacion = new GestorReportes.BusinessLayer.EntitiesAntilavado2.Liquidacion();
                string fechaPago = ((XmlElement)nodoLiquidacion).GetElementsByTagName("fecha_pago")[0].InnerText;
                liquidacion.FechaDePago = new DateTime(Convert.ToInt32(fechaPago.Substring(0, 4)), Convert.ToInt32(fechaPago.Substring(4, 2)), Convert.ToInt32(fechaPago.Substring(6, 2)));
                liquidacion.ClaveInstrumentoMonetario = Convert.ToInt32(((XmlElement)nodoLiquidacion).GetElementsByTagName("instrumento_monetario")[0].InnerText);                
                liquidacion.Moneda = Convert.ToInt32(((XmlElement)nodoLiquidacion).GetElementsByTagName("moneda")[0].InnerText);
                liquidacion.MontoOperacion = Convert.ToDecimal(((XmlElement)nodoLiquidacion).GetElementsByTagName("monto_operacion")[0].InnerText);
                return new List<GestorReportes.BusinessLayer.EntitiesAntilavado2.Liquidacion>() { liquidacion };
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Generación de representación impresa antilavado

        public Informe leerXml(string filename)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filename);
                XmlNodeList archivos = xmlDoc.GetElementsByTagName("archivo");
                if (archivos.Count > 0)
                {
                    XmlNodeList informes = ((XmlElement)archivos[0]).GetElementsByTagName("informe");
                    if (informes.Count > 0)
                    {
                        Informe informe = new Informe();
                        XmlNode mesInforme = ((XmlElement)informes[0]).GetElementsByTagName("mes_reportado")[0];
                        if (mesInforme != null)
                        {
                            informe.MesReportado = new DateTime(Convert.ToInt32(mesInforme.InnerText.Substring(0, 4)), Convert.ToInt32(mesInforme.InnerText.Substring(4, 2)), 1);
                            XmlNodeList sujetosObligados = ((XmlElement)informes[0]).GetElementsByTagName("sujeto_obligado");
                            if (sujetosObligados.Count > 0)
                            {
                                SujetoObligado sujeto = new SujetoObligado();
                                sujeto.ClaveRFC = ((XmlElement)sujetosObligados[0]).GetElementsByTagName("clave_sujeto_obligado")[0].InnerText;
                                sujeto.Actividad = ((XmlElement)sujetosObligados[0]).GetElementsByTagName("clave_actividad")[0].InnerText;
                                informe.SujetoObligado = sujeto;
                                XmlNodeList avisos = ((XmlElement)informes[0]).GetElementsByTagName("aviso");
                                if (avisos.Count > 0)
                                {
                                    List<Aviso> listaAvisos = new List<Aviso>();
                                    foreach (XmlNode avis in avisos)
                                    {
                                        Aviso aviso = leeAviso(avis);
                                        if (aviso == null)
                                            return null;
                                        else
                                            listaAvisos.Add(aviso);
                                    }
                                    informe.Avisos = listaAvisos;
                                }
                                return informe;
                            }
                            else
                                return null;
                        }
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        private Aviso leeAviso(XmlNode nodoAviso)
        {
            try
            {
                Aviso aviso = new Aviso();
                aviso.Referencia = Convert.ToInt32(((XmlElement)nodoAviso).GetElementsByTagName("referencia_aviso")[0].InnerText);
                aviso.Prioridad = Convert.ToInt32(((XmlElement)nodoAviso).GetElementsByTagName("prioridad")[0].InnerText);
                XmlNode nodeAlerta = ((XmlElement)nodoAviso).GetElementsByTagName("alerta")[0];
                Alerta alert = new Alerta();
                alert.Tipo = Convert.ToInt32(((XmlElement)nodeAlerta).GetElementsByTagName("tipo_alerta")[0].InnerText);
                aviso.Alerta = alert;
                XmlNode personaAviso = ((XmlElement)nodoAviso).GetElementsByTagName("persona_aviso")[0];
                aviso.Persona = leePersona(personaAviso);
                if (aviso.Persona == null)
                    return null;
                XmlNode detalleOperaciones = ((XmlElement)nodoAviso).GetElementsByTagName("detalle_operaciones")[0];
                aviso.Operaciones = leeOperaciones(detalleOperaciones);
                if (aviso.Operaciones == null)
                    return null;
                return aviso;
            }
            catch
            {
                return null;
            }
        }

        private Persona leePersona(XmlNode nodoPersonaAviso)
        {
            try
            {
                XmlNode tipoPersona = ((XmlElement)nodoPersonaAviso).GetElementsByTagName("tipo_persona")[0];
                Persona persona = new Persona();
                XmlNodeList nodeType = ((XmlElement)tipoPersona).GetElementsByTagName("persona_moral");
                if (nodeType.Count > 0)
                {
                    persona.EsPersonaFisica = false;
                    PersonaMoral moral = new PersonaMoral();
                    moral.RazonSocial = ((XmlElement)nodeType[0]).GetElementsByTagName("denominacion_razon")[0].InnerText;
                    string fechaConstString = ((XmlElement)nodeType[0]).GetElementsByTagName("fecha_constitucion")[0].InnerText;
                    moral.FechaDeConstitucion = new DateTime(Convert.ToInt32(fechaConstString.Substring(0, 4)), Convert.ToInt32(fechaConstString.Substring(4, 2)), Convert.ToInt32(fechaConstString.Substring(6, 2)));
                    moral.RFC = ((XmlElement)nodeType[0]).GetElementsByTagName("rfc")[0].InnerText;
                    moral.PaisNacionalidad = ((XmlElement)nodeType[0]).GetElementsByTagName("pais_nacionalidad")[0].InnerText;
                    moral.Giro = ((XmlElement)nodeType[0]).GetElementsByTagName("giro_mercantil")[0].InnerText;
                    XmlNodeList nodoRepresentante = ((XmlElement)nodeType[0]).GetElementsByTagName("representante_apoderado");
                    PersonaFisica representante = new PersonaFisica();
                    representante.Nombre = ((XmlElement)nodoRepresentante[0]).GetElementsByTagName("nombre")[0].InnerText;
                    representante.ApellidoPaterno = ((XmlElement)nodoRepresentante[0]).GetElementsByTagName("apellido_paterno")[0].InnerText;
                    representante.ApellidoMaterno = ((XmlElement)nodoRepresentante[0]).GetElementsByTagName("apellido_materno")[0].InnerText;
                    string fechaNac = ((XmlElement)nodoRepresentante[0]).GetElementsByTagName("fecha_nacimiento")[0].InnerText;
                    representante.FechaNacimiento = new DateTime(Convert.ToInt32(fechaNac.Substring(0, 4)), Convert.ToInt32(fechaNac.Substring(4, 2)), Convert.ToInt32(fechaNac.Substring(6, 2)));
                    representante.RFC = ((XmlElement)nodoRepresentante[0]).GetElementsByTagName("rfc")[0].InnerText;
                    representante.TipoIdentificacion = Convert.ToInt32(((XmlElement)nodoRepresentante[0]).GetElementsByTagName("tipo_identificacion")[0].InnerText);
                    representante.Autoridad = ((XmlElement)nodoRepresentante[0]).GetElementsByTagName("autoridad_identificacion")[0].InnerText;
                    representante.NumeroIdentificacion = ((XmlElement)nodoRepresentante[0]).GetElementsByTagName("numero_identificacion")[0].InnerText;
                    moral.Apoderado = representante;
                    persona.Moral = moral;
                }
                else
                {
                    nodeType = ((XmlElement)tipoPersona).GetElementsByTagName("persona_fisica");
                    if (nodeType.Count > 0)
                    {
                        persona.EsPersonaFisica = true;
                        PersonaFisica fisica = new PersonaFisica();
                        fisica.Nombre = ((XmlElement)nodeType[0]).GetElementsByTagName("nombre")[0].InnerText;
                        fisica.ApellidoPaterno = ((XmlElement)nodeType[0]).GetElementsByTagName("apellido_paterno")[0].InnerText;
                        fisica.ApellidoMaterno = ((XmlElement)nodeType[0]).GetElementsByTagName("apellido_materno")[0].InnerText;
                        string fechaNac = ((XmlElement)nodeType[0]).GetElementsByTagName("fecha_nacimiento")[0].InnerText;
                        fisica.FechaNacimiento = new DateTime(Convert.ToInt32(fechaNac.Substring(0, 4)), Convert.ToInt32(fechaNac.Substring(4, 2)), Convert.ToInt32(fechaNac.Substring(6, 2)));
                        fisica.RFC = ((XmlElement)nodeType[0]).GetElementsByTagName("rfc")[0].InnerText;
                        fisica.PaisNacionalidad = ((XmlElement)nodeType[0]).GetElementsByTagName("pais_nacionalidad")[0].InnerText;
                        fisica.PaisNacimiento = ((XmlElement)nodeType[0]).GetElementsByTagName("pais_nacimiento")[0].InnerText;
                        fisica.ActividadEconomica = ((XmlElement)nodeType[0]).GetElementsByTagName("actividad_economica")[0].InnerText;
                        fisica.TipoIdentificacion = Convert.ToInt32(((XmlElement)nodeType[0]).GetElementsByTagName("tipo_identificacion")[0].InnerText);
                        fisica.Autoridad = ((XmlElement)nodeType[0]).GetElementsByTagName("autoridad_identificacion")[0].InnerText;
                        fisica.NumeroIdentificacion = ((XmlElement)nodeType[0]).GetElementsByTagName("numero_identificacion")[0].InnerText;
                        persona.Fisica = fisica;
                    }
                    else
                        return null;
                }
                XmlNode domic = ((XmlElement)nodoPersonaAviso).GetElementsByTagName("tipo_domicilio")[0];
                persona.DomicilioNacional = leeDomicilio(domic);
                if (persona.DomicilioNacional == null)
                    return null;
                XmlNode tel = ((XmlElement)nodoPersonaAviso).GetElementsByTagName("telefono")[0];
                Telefono telefono = new Telefono();
                telefono.Numero = ((XmlElement)tel).GetElementsByTagName("numero_telefono")[0].InnerText;
                telefono.Pais = ((XmlElement)tel).GetElementsByTagName("clave_pais")[0].InnerText;
                persona.Telefono = telefono;
                return persona;
            }
            catch
            {
                return null;
            }
        }

        private Domicilio leeDomicilio(XmlNode nodoDomicilio)
        {
            try
            {
                Domicilio domicilio = new Domicilio();
                XmlNode nacional = ((XmlElement)nodoDomicilio).GetElementsByTagName("nacional")[0];
                domicilio.Colonia = ((XmlElement)nacional).GetElementsByTagName("colonia")[0].InnerText;
                domicilio.Calle = ((XmlElement)nacional).GetElementsByTagName("calle")[0].InnerText;
                domicilio.NumeroExterior = ((XmlElement)nacional).GetElementsByTagName("numero_exterior")[0].InnerText;
                domicilio.CodigoPostal = ((XmlElement)nacional).GetElementsByTagName("codigo_postal")[0].InnerText;
                return domicilio;
            }
            catch
            {
                return null;
            }
        }

        private List<Operacion> leeOperaciones(XmlNode nodoDetalleOperaciones)
        {
            try
            {
                XmlNode operacionesRealizadas = ((XmlElement)nodoDetalleOperaciones).GetElementsByTagName("operaciones_realizadas")[0];
                XmlNodeList datosOperacion = ((XmlElement)operacionesRealizadas).GetElementsByTagName("datos_operacion");
                if (datosOperacion.Count > 0)
                {
                    List<Operacion> operaciones = new List<Operacion>();
                    foreach (XmlNode nodoOperacion in datosOperacion)
                    {
                        Operacion operacion = new Operacion();
                        string fechaOper = ((XmlElement)nodoOperacion).GetElementsByTagName("fecha_operacion")[0].InnerText;
                        operacion.Fecha = new DateTime(Convert.ToInt32(fechaOper.Substring(0, 4)), Convert.ToInt32(fechaOper.Substring(4, 2)), Convert.ToInt32(fechaOper.Substring(6, 2)));
                        operacion.Tipo = Convert.ToInt32(((XmlElement)nodoOperacion).GetElementsByTagName("tipo_operacion")[0].InnerText);
                        string fechaIn = ((XmlElement)nodoOperacion).GetElementsByTagName("fecha_inicio")[0].InnerText;
                        string fechaFin = ((XmlElement)nodoOperacion).GetElementsByTagName("fecha_termino")[0].InnerText;
                        operacion.FechaInicioRenta = new DateTime(Convert.ToInt32(fechaIn.Substring(0, 4)), Convert.ToInt32(fechaIn.Substring(4, 2)), Convert.ToInt32(fechaIn.Substring(6, 2)));
                        operacion.FechaFinRenta = new DateTime(Convert.ToInt32(fechaFin.Substring(0, 4)), Convert.ToInt32(fechaFin.Substring(4, 2)), Convert.ToInt32(fechaFin.Substring(6, 2)));
                        XmlNode nodoCarac = ((XmlElement)nodoOperacion).GetElementsByTagName("caracteristicas")[0];
                        operacion.CaracteristicasDelInmueble = leeCaracteristicas(nodoCarac);
                        if (operacion.CaracteristicasDelInmueble == null)
                            return null;
                        XmlNode nodoLiq = ((XmlElement)nodoOperacion).GetElementsByTagName("datos_liquidacion")[0];
                        operacion.Liquidacion = leeLiquidacion(nodoLiq);
                        if (operacion.Liquidacion == null)
                            return null;
                        operaciones.Add(operacion);
                    }
                    return operaciones;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        private CaracteristicasInmueble leeCaracteristicas(XmlNode nodoCaracteristicas)
        {
            try
            {
                CaracteristicasInmueble carac = new CaracteristicasInmueble();
                carac.TipoDeInmueble = Convert.ToInt32(((XmlElement)nodoCaracteristicas).GetElementsByTagName("tipo_inmueble")[0].InnerText);
                carac.ValorCatastral = Convert.ToDecimal(((XmlElement)nodoCaracteristicas).GetElementsByTagName("valor_avaluo_catastral")[0].InnerText);
                Domicilio domcInm = new Domicilio();
                domcInm.Colonia = ((XmlElement)nodoCaracteristicas).GetElementsByTagName("colonia")[0].InnerText;
                domcInm.Calle = ((XmlElement)nodoCaracteristicas).GetElementsByTagName("calle")[0].InnerText;
                domcInm.NumeroExterior = ((XmlElement)nodoCaracteristicas).GetElementsByTagName("numero_exterior")[0].InnerText;
                domcInm.CodigoPostal = ((XmlElement)nodoCaracteristicas).GetElementsByTagName("codigo_postal")[0].InnerText;
                carac.Domicilio = domcInm;
                carac.Blindaje = ((XmlElement)nodoCaracteristicas).GetElementsByTagName("blindaje")[0].InnerText.ToLower() == "si" ? 1 : 2;
                carac.FolioReal = ((XmlElement)nodoCaracteristicas).GetElementsByTagName("folio_real")[0].InnerText;
                return carac;
            }
            catch
            {
                return null;
            }
        }

        private Liquidacion leeLiquidacion(XmlNode nodoLiquidacion)
        {
            try
            {
                Liquidacion liquidacion = new Liquidacion();
                string fechaPago = ((XmlElement)nodoLiquidacion).GetElementsByTagName("fecha_pago")[0].InnerText;
                liquidacion.FechaDePago = new DateTime(Convert.ToInt32(fechaPago.Substring(0, 4)), Convert.ToInt32(fechaPago.Substring(4, 2)), Convert.ToInt32(fechaPago.Substring(6, 2)));
                liquidacion.ClaveInstrumentoMonetario = Convert.ToInt32(((XmlElement)nodoLiquidacion).GetElementsByTagName("instrumento_monetario")[0].InnerText);
                XmlNode nodoDetalleInst = ((XmlElement)nodoLiquidacion).GetElementsByTagName("detalle_instrumento")[0];
                DetalleInstrumento detalleInstrumento = new DetalleInstrumento();
                if (liquidacion.ClaveInstrumentoMonetario == 5) //cheque nominativo
                {
                    Cheque cheque = new Cheque();
                    XmlNode nodoCheque = ((XmlElement)nodoDetalleInst).GetElementsByTagName("cheque")[0];
                    cheque.InstitucionCredito = ((XmlElement)nodoCheque).GetElementsByTagName("institucion_credito")[0].InnerText;
                    cheque.NumeroDeCuentaLibrador = ((XmlElement)nodoCheque).GetElementsByTagName("numero_cuenta")[0].InnerText;
                    cheque.NumeroCheque = ((XmlElement)nodoCheque).GetElementsByTagName("numero_cheque")[0].InnerText;
                    detalleInstrumento.Cheque = cheque;
                }
                else if (liquidacion.ClaveInstrumentoMonetario == 8)//transferencia interbancaria
                {
                    TransferenciaInterbancaria transfInter = new TransferenciaInterbancaria();
                    XmlNode nodoTransfInter = ((XmlElement)nodoDetalleInst).GetElementsByTagName("transferencia_interbancaria")[0];
                    transfInter.ClaveRastreo = ((XmlElement)nodoTransfInter).GetElementsByTagName("clave_rastreo")[0].InnerText;
                    detalleInstrumento.TransferenciaInterbancaria = transfInter;
                }
                else if (liquidacion.ClaveInstrumentoMonetario == 9)//transferencia mismo banco
                {
                    TransferenciaMismoBanco transfMismoBanco = new TransferenciaMismoBanco();
                    XmlNode nodoTransfMism = ((XmlElement)nodoDetalleInst).GetElementsByTagName("transferencia_mismo_banco")[0];
                    transfMismoBanco.FolioInterno = ((XmlElement)nodoTransfMism).GetElementsByTagName("folio_interno")[0].InnerText;
                    detalleInstrumento.TransferenciaDelMismoBanco = transfMismoBanco;
                }
                else
                    return null;
                liquidacion.DetalleDelInstrumentoMonetario = detalleInstrumento;
                liquidacion.Moneda = Convert.ToInt32(((XmlElement)nodoLiquidacion).GetElementsByTagName("moneda")[0].InnerText);
                liquidacion.MontoOperacion = Convert.ToDecimal(((XmlElement)nodoLiquidacion).GetElementsByTagName("monto_operacion")[0].InnerText);
                return liquidacion;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Generación de archivo antilavado V1  
        public string generar(string pathApplication, Informe informe, bool esEnCeros, bool esVenta)
        {
            EsEnCeros = esEnCeros;
            EsVenta = esVenta;
            string pathXSD = string.Empty;
            if(!esVenta)
                pathXSD = pathApplication + @"\Resources\Antilavado_Arrendamiento.xsd";
            else
                pathXSD = pathApplication + @"\Resources\Antilavado_Inmuebles.xsd";
            if (!File.Exists(pathXSD))
                return "Error: No se encontró el archivo de esquema (.xsd), de la ley antilavado en los recursos.";
            try
            {                
                dsAntilavado.ReadXmlSchema(pathXSD);
                
                string errorInforme = fillInforme(informe);
                if (!string.IsNullOrEmpty(errorInforme))
                    return errorInforme;  

                string errorGenerar = generaArchivo(pathXSD, esVenta);
                if (!string.IsNullOrEmpty(errorGenerar))
                    return errorGenerar;                           
            }
            catch
            {
                return "Error: No se pudo leer el archivo de esquema (.xsd).";
            }
            return string.Empty;
        }

        private string fillInforme(Informe informe)
        {
            try
            {
                DataRow rowInforme = dsAntilavado.Tables["informe"].NewRow();
                rowInforme["mes_reportado"] = informe.MesReportado.ToString("yyyyMM");
                rowInforme["informe_Id"] = 1;
                dsAntilavado.Tables["informe"].Rows.Add(rowInforme);
                dsAntilavado.AcceptChanges();

                string errorSujetoObligado = fillSujetoObligado(informe.SujetoObligado);
                if (!string.IsNullOrEmpty(errorSujetoObligado))
                    return errorSujetoObligado;

                if (!EsEnCeros)
                {                    
                    string errorAvisos = fillAvisos(informe.Avisos);
                    if (!string.IsNullOrEmpty(errorAvisos))
                        return errorAvisos;
                }
            }
            catch
            {
                return "Error: Ocurrio un problema al generar la sección informe.";
            }
            return string.Empty;
        }

        private string fillSujetoObligado(SujetoObligado sujeto)
        {
            try
            {
                DataRow rowSujetoObligado = dsAntilavado.Tables["sujeto_obligado"].NewRow();
                rowSujetoObligado["informe_Id"] = 1;
                rowSujetoObligado["clave_sujeto_obligado"] = sujeto.ClaveRFC;
                rowSujetoObligado["clave_actividad"] = sujeto.Actividad;
                dsAntilavado.Tables["sujeto_obligado"].Rows.Add(rowSujetoObligado);
                dsAntilavado.AcceptChanges();
                return string.Empty;
            }
            catch
            {
                return "Error: Ocurrio un problema al generar la sección sujeto obligado.";
            }            
        }

        private string fillAvisos(List<Aviso> avisos)
        {
            try
            {
                foreach (Aviso aviso in avisos)
                {
                    DataRow rowAviso = dsAntilavado.Tables["aviso"].NewRow();
                    rowAviso["informe_Id"] = 1;
                    rowAviso["aviso_Id"] = numAviso;
                    rowAviso["referencia_aviso"] = aviso.Referencia;
                    rowAviso["prioridad"] = aviso.Prioridad;
                    dsAntilavado.Tables["aviso"].Rows.Add(rowAviso);
                    dsAntilavado.AcceptChanges();
                    string errorAlerta = fillAlerta(aviso.Alerta, numAviso);
                    if (!string.IsNullOrEmpty(errorAlerta))
                        return errorAlerta;
                    string errorPersona = fillPersona(aviso.Persona, numAviso);
                    if (!string.IsNullOrEmpty(errorPersona))
                        return errorPersona;
                    if (!EsVenta)
                    {
                        string errorOperaciones = fillOperaciones(aviso.Operaciones, numAviso);
                        if (!string.IsNullOrEmpty(errorOperaciones))
                            return errorOperaciones;
                    }
                    else
                    {
                        string errorOperaciones = fillOperaciones(aviso.OperacionesVenta, numAviso);
                        if (!string.IsNullOrEmpty(errorOperaciones))
                            return errorOperaciones;
                    }
                    numAviso++;
                }
                return string.Empty;
            }
            catch
            {
                return "Error: Ocurrió un problema al generar la sección avisos.";
            }            
        }

        private string fillAlerta(Alerta alerta, int numeroDeAviso)
        {
            try
            {
                DataRow rowAlerta = dsAntilavado.Tables["alerta"].NewRow();
                rowAlerta["tipo_alerta"] = alerta.Tipo;
                rowAlerta["descripcion_alerta"] = alerta.Descripcion;
                rowAlerta["aviso_Id"] = numeroDeAviso;
                dsAntilavado.Tables["alerta"].Rows.Add(rowAlerta);
                dsAntilavado.AcceptChanges();
                return string.Empty;
            }
            catch
            {
                return "Error: Ocurrió un problema al generar la sección alerta.";
            }
        }

        private string fillPersona(Persona persona, int numeroDeAviso)
        {
            try
            {
                DataRow rowPersona = dsAntilavado.Tables["persona_aviso"].NewRow();
                rowPersona["aviso_Id"] = numeroDeAviso;
                rowPersona["persona_aviso_Id"] = numeroDeAviso;
                dsAntilavado.Tables["persona_aviso"].Rows.Add(rowPersona);
                dsAntilavado.AcceptChanges();

                DataRow rowTipoPersona = dsAntilavado.Tables["tipo_persona"].NewRow();
                rowTipoPersona["persona_aviso_Id"] = numeroDeAviso;
                rowTipoPersona["tipo_persona_Id"] = numeroDeAviso;
                dsAntilavado.Tables["tipo_persona"].Rows.Add(rowTipoPersona);
                dsAntilavado.AcceptChanges();

                if (persona.EsPersonaFisica)
                {
                    DataRow rowPersonaFisica = dsAntilavado.Tables["persona_fisica"].NewRow();
                    rowPersonaFisica["tipo_persona_Id"] = numeroDeAviso;
                    //rowPersonaFisica["persona_fisica_Id"] = numeroDeAviso;
                    rowPersonaFisica["nombre"] = persona.Fisica.Nombre;
                    rowPersonaFisica["apellido_paterno"] = persona.Fisica.ApellidoPaterno;
                    rowPersonaFisica["apellido_materno"] = persona.Fisica.ApellidoMaterno;
                    rowPersonaFisica["fecha_nacimiento"] = persona.Fisica.FechaNacimiento.ToString("yyyyMMdd");
                    rowPersonaFisica["rfc"] = persona.Fisica.RFC;
                    if (!string.IsNullOrEmpty(persona.Fisica.CURP))
                        rowPersonaFisica["curp"] = persona.Fisica.CURP;
                    rowPersonaFisica["pais_nacionalidad"] = persona.Fisica.PaisNacionalidad;
                    rowPersonaFisica["pais_nacimiento"] = persona.Fisica.PaisNacimiento;
                    rowPersonaFisica["actividad_economica"] = persona.Fisica.ActividadEconomica;
                    rowPersonaFisica["tipo_identificacion"] = persona.Fisica.TipoIdentificacion;
                    if (!string.IsNullOrEmpty(persona.Fisica.OtraIdentificacion))
                        rowPersonaFisica["identificacion_otro"] = persona.Fisica.OtraIdentificacion;
                    rowPersonaFisica["autoridad_identificacion"] = persona.Fisica.Autoridad;
                    rowPersonaFisica["numero_identificacion"] = persona.Fisica.NumeroIdentificacion;
                    dsAntilavado.Tables["persona_fisica"].Rows.Add(rowPersonaFisica);
                    dsAntilavado.AcceptChanges();
                }
                else
                {
                    DataRow rowPersonaMoral = dsAntilavado.Tables["persona_moral"].NewRow();
                    rowPersonaMoral["tipo_persona_Id"] = numeroDeAviso;
                    rowPersonaMoral["persona_moral_Id"] = numeroDeAviso;
                    rowPersonaMoral["denominacion_razon"] = persona.Moral.RazonSocial;
                    rowPersonaMoral["fecha_constitucion"] = persona.Moral.FechaDeConstitucion.ToString("yyyyMMdd");
                    rowPersonaMoral["rfc"] = persona.Moral.RFC;
                    rowPersonaMoral["pais_nacionalidad"] = persona.Moral.PaisNacionalidad;
                    rowPersonaMoral["giro_mercantil"] = persona.Moral.Giro;
                    dsAntilavado.Tables["persona_moral"].Rows.Add(rowPersonaMoral);
                    dsAntilavado.AcceptChanges();

                    DataRow rowApoderado = dsAntilavado.Tables["representante_apoderado"].NewRow();
                    rowApoderado["persona_moral_Id"] = numeroDeAviso;
                    rowApoderado["nombre"] = persona.Moral.Apoderado.Nombre;
                    rowApoderado["apellido_paterno"] = persona.Moral.Apoderado.ApellidoPaterno;
                    rowApoderado["apellido_materno"] = persona.Moral.Apoderado.ApellidoMaterno;
                    rowApoderado["fecha_nacimiento"] = persona.Moral.Apoderado.FechaNacimiento.ToString("yyyyMMdd");
                    rowApoderado["rfc"] = persona.Moral.Apoderado.RFC;
                    if (!string.IsNullOrEmpty(persona.Moral.Apoderado.CURP))
                        rowApoderado["curp"] = persona.Moral.Apoderado.CURP;
                    rowApoderado["tipo_identificacion"] = persona.Moral.Apoderado.TipoIdentificacion;
                    if (!string.IsNullOrEmpty(persona.Moral.Apoderado.OtraIdentificacion))
                        rowApoderado["identificacion_otro"] = persona.Moral.Apoderado.OtraIdentificacion;
                    rowApoderado["autoridad_identificacion"] = persona.Moral.Apoderado.Autoridad;
                    rowApoderado["numero_identificacion"] = persona.Moral.Apoderado.NumeroIdentificacion;
                    dsAntilavado.Tables["representante_apoderado"].Rows.Add(rowApoderado);
                    dsAntilavado.AcceptChanges();
                }

                DataRow rowDomicilio = dsAntilavado.Tables["tipo_domicilio"].NewRow();
                rowDomicilio["persona_aviso_Id"] = numeroDeAviso;
                rowDomicilio["tipo_domicilio_Id"] = numeroDeAviso;
                dsAntilavado.Tables["tipo_domicilio"].Rows.Add(rowDomicilio);
                dsAntilavado.AcceptChanges();

                DataRow rowDomicilioNacional = dsAntilavado.Tables["nacional"].NewRow();
                rowDomicilioNacional["tipo_domicilio_Id"] = numeroDeAviso;
                rowDomicilioNacional["colonia"] = persona.DomicilioNacional.Colonia;
                rowDomicilioNacional["calle"] = persona.DomicilioNacional.Calle;
                rowDomicilioNacional["numero_exterior"] = persona.DomicilioNacional.NumeroExterior;
                if (!string.IsNullOrEmpty(persona.DomicilioNacional.NumeroInterior))
                    rowDomicilioNacional["numero_interior"] = persona.DomicilioNacional.NumeroInterior;
                rowDomicilioNacional["codigo_postal"] = persona.DomicilioNacional.CodigoPostal;
                dsAntilavado.Tables["nacional"].Rows.Add(rowDomicilioNacional);
                dsAntilavado.AcceptChanges();

                DataRow rowTelefono = dsAntilavado.Tables["telefono"].NewRow();
                rowTelefono["persona_aviso_Id"] = numeroDeAviso;
                rowTelefono["clave_pais"] = persona.Telefono.Pais;
                rowTelefono["numero_telefono"] = persona.Telefono.Numero;
                if (!string.IsNullOrEmpty(persona.Telefono.EMail))
                    rowTelefono["correo_electronico"] = persona.Telefono.EMail;
                dsAntilavado.Tables["telefono"].Rows.Add(rowTelefono);
                dsAntilavado.AcceptChanges();
                return string.Empty;
            }
            catch
            {
                return "Error: Ocurrió un problema al generar la sección persona.";
            }
        }

        private string fillOperaciones(List<Operacion> operaciones, int numeroDeAviso)
        {
            try
            {
                DataRow rowOperacion = dsAntilavado.Tables["detalle_operaciones"].NewRow();
                rowOperacion["aviso_Id"] = numeroDeAviso;
                rowOperacion["detalle_operaciones_Id"] = numeroDeAviso;
                dsAntilavado.Tables["detalle_operaciones"].Rows.Add(rowOperacion);
                dsAntilavado.AcceptChanges();

                DataRow rowOperacionRealizada = dsAntilavado.Tables["operaciones_realizadas"].NewRow();
                rowOperacionRealizada["detalle_operaciones_Id"] = numeroDeAviso;
                rowOperacionRealizada["operaciones_realizadas_Id"] = numeroDeAviso;
                dsAntilavado.Tables["operaciones_realizadas"].Rows.Add(rowOperacionRealizada);
                dsAntilavado.AcceptChanges();
                                
                foreach (Operacion operacion in operaciones)
                {
                    DataRow rowDatosOperacion = dsAntilavado.Tables["datos_operacion"].NewRow();
                    rowDatosOperacion["operaciones_realizadas_Id"] = numeroDeAviso;
                    rowDatosOperacion["datos_operacion_Id"] = numOperacion;
                    rowDatosOperacion["fecha_operacion"] = operacion.Fecha.ToString("yyyyMMdd");
                    rowDatosOperacion["tipo_operacion"] = operacion.Tipo;
                    rowDatosOperacion["fecha_inicio"] = operacion.FechaInicioRenta.ToString("yyyyMMdd");
                    rowDatosOperacion["fecha_termino"] = operacion.FechaFinRenta.ToString("yyyyMMdd");
                    dsAntilavado.Tables["datos_operacion"].Rows.Add(rowDatosOperacion);
                    dsAntilavado.AcceptChanges();

                    DataRow rowCaracteristicas = dsAntilavado.Tables["caracteristicas"].NewRow();
                    rowCaracteristicas["datos_operacion_Id"] = numOperacion;
                    rowCaracteristicas["tipo_inmueble"] = operacion.CaracteristicasDelInmueble.TipoDeInmueble;
                    rowCaracteristicas["valor_avaluo_catastral"] = Decimal.Round(operacion.CaracteristicasDelInmueble.ValorCatastral, 2).ToString("F");
                    rowCaracteristicas["colonia"] = operacion.CaracteristicasDelInmueble.Domicilio.Colonia;
                    rowCaracteristicas["calle"] = operacion.CaracteristicasDelInmueble.Domicilio.Calle;
                    rowCaracteristicas["numero_exterior"] = operacion.CaracteristicasDelInmueble.Domicilio.NumeroExterior;
                    if(!string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.Domicilio.NumeroInterior))
                        rowCaracteristicas["numero_interior"] = operacion.CaracteristicasDelInmueble.Domicilio.NumeroInterior;
                    rowCaracteristicas["codigo_postal"] = operacion.CaracteristicasDelInmueble.Domicilio.CodigoPostal;
                    rowCaracteristicas["blindaje"] = operacion.CaracteristicasDelInmueble.Blindaje == 1 ? "SI" : "NO";
                    rowCaracteristicas["folio_real"] = operacion.CaracteristicasDelInmueble.FolioReal;
                    dsAntilavado.Tables["caracteristicas"].Rows.Add(rowCaracteristicas);
                    dsAntilavado.AcceptChanges();

                    DataRow rowLiquidacion = dsAntilavado.Tables["datos_liquidacion"].NewRow();
                    rowLiquidacion["datos_operacion_Id"] = numOperacion;
                    rowLiquidacion["datos_liquidacion_Id"] = numOperacion;
                    rowLiquidacion["fecha_pago"] = operacion.Liquidacion.FechaDePago.ToString("yyyyMMdd");
                    rowLiquidacion["instrumento_monetario"] = operacion.Liquidacion.ClaveInstrumentoMonetario;
                    rowLiquidacion["moneda"] = operacion.Liquidacion.Moneda;
                    rowLiquidacion["monto_operacion"] = Decimal.Round(operacion.Liquidacion.MontoOperacion, 2).ToString("F");
                    dsAntilavado.Tables["datos_liquidacion"].Rows.Add(rowLiquidacion);
                    dsAntilavado.AcceptChanges();

                    DataRow rowDetalleInstrumento = dsAntilavado.Tables["detalle_instrumento"].NewRow();
                    rowDetalleInstrumento["datos_liquidacion_Id"] = numOperacion;
                    rowDetalleInstrumento["detalle_instrumento_Id"] = numOperacion;
                    dsAntilavado.Tables["detalle_instrumento"].Rows.Add(rowDetalleInstrumento);
                    dsAntilavado.AcceptChanges();

                    if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque != null)
                    {
                        DataRow rowCheque = dsAntilavado.Tables["cheque"].NewRow();
                        rowCheque["detalle_instrumento_Id"] = numOperacion;
                        rowCheque["institucion_credito"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.InstitucionCredito;
                        rowCheque["numero_cuenta"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.NumeroDeCuentaLibrador;
                        rowCheque["numero_cheque"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.NumeroCheque;
                        dsAntilavado.Tables["cheque"].Rows.Add(rowCheque);
                        dsAntilavado.AcceptChanges();
                    }
                    else if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaDelMismoBanco != null)
                    {
                        DataRow rowTransMismoBanco = dsAntilavado.Tables["transferencia_mismo_banco"].NewRow();
                        rowTransMismoBanco["detalle_instrumento_Id"] = numOperacion;
                        rowTransMismoBanco["folio_interno"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaDelMismoBanco.FolioInterno;
                        dsAntilavado.Tables["transferencia_mismo_banco"].Rows.Add(rowTransMismoBanco);
                        dsAntilavado.AcceptChanges();
                    }
                    else if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInterbancaria != null)
                    {
                        DataRow rowTransInterbancaria = dsAntilavado.Tables["transferencia_interbancaria"].NewRow();
                        rowTransInterbancaria["detalle_instrumento_Id"] = numOperacion;
                        rowTransInterbancaria["clave_rastreo"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInterbancaria.ClaveRastreo;
                        dsAntilavado.Tables["transferencia_interbancaria"].Rows.Add(rowTransInterbancaria);
                        dsAntilavado.AcceptChanges();
                    }
                    else if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional != null)
                    {
                        DataRow rowTransInternacional = dsAntilavado.Tables["transferencia_internacional"].NewRow();
                        rowTransInternacional["detalle_instrumento_Id"] = numOperacion;
                        rowTransInternacional["institucion_ordenante"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.InstitucionOrdenante;
                        rowTransInternacional["numero_cuenta"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.NumeroDeCuenta;
                        rowTransInternacional["pais_origen"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.PaisDeOrigen;
                        dsAntilavado.Tables["transferencia_internacional"].Rows.Add(rowTransInternacional);
                        dsAntilavado.AcceptChanges();
                    }
                    numOperacion++;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error: Ocurrió un error mientras se generaba la sección operaciones. " + ex.Message;
            }
        }

        private string fillOperaciones(List<OperacionVenta> operaciones, int numeroDeAviso)
        {
            try
            {
                DataRow rowOperacion = dsAntilavado.Tables["detalle_operaciones"].NewRow();
                rowOperacion["aviso_Id"] = numeroDeAviso;
                rowOperacion["detalle_operaciones_Id"] = numeroDeAviso;
                dsAntilavado.Tables["detalle_operaciones"].Rows.Add(rowOperacion);
                dsAntilavado.AcceptChanges();

                DataRow rowOperacionRealizada = dsAntilavado.Tables["operaciones_realizadas"].NewRow();
                rowOperacionRealizada["detalle_operaciones_Id"] = numeroDeAviso;
                rowOperacionRealizada["operaciones_realizadas_Id"] = numeroDeAviso;
                dsAntilavado.Tables["operaciones_realizadas"].Rows.Add(rowOperacionRealizada);
                dsAntilavado.AcceptChanges();

                foreach (OperacionVenta operacion in operaciones)
                {
                    DataRow rowDatosOperacion = dsAntilavado.Tables["datos_operacion"].NewRow();
                    rowDatosOperacion["operaciones_realizadas_Id"] = numeroDeAviso;
                    rowDatosOperacion["datos_operacion_Id"] = numOperacion;
                    rowDatosOperacion["fecha_operacion"] = operacion.Fecha.ToString("yyyyMMdd");
                    rowDatosOperacion["tipo_operacion"] = operacion.Tipo;
                    rowDatosOperacion["tipo_inmueble"] = operacion.TipoDeInmueble;
                    rowDatosOperacion["valor_pactado"] = Decimal.Round(operacion.MontoPactado, 2).ToString("F");
                    rowDatosOperacion["figura_cliente"] = operacion.FiguraDelCliente;
                    rowDatosOperacion["figura_so"] = operacion.FiguraPersona;
                    dsAntilavado.Tables["datos_operacion"].Rows.Add(rowDatosOperacion);
                    dsAntilavado.AcceptChanges();

                    DataRow rowCaracteristicas = dsAntilavado.Tables["caracteristicas_inmueble"].NewRow();
                    rowCaracteristicas["datos_operacion_Id"] = numOperacion;
                    rowCaracteristicas["colonia"] = operacion.CaracteristicasDelInmueble.Domicilio.Colonia;
                    rowCaracteristicas["calle"] = operacion.CaracteristicasDelInmueble.Domicilio.Calle;
                    rowCaracteristicas["numero_exterior"] = operacion.CaracteristicasDelInmueble.Domicilio.NumeroExterior;
                    if (!string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.Domicilio.NumeroInterior))
                        rowCaracteristicas["numero_interior"] = operacion.CaracteristicasDelInmueble.Domicilio.NumeroInterior;
                    rowCaracteristicas["codigo_postal"] = operacion.CaracteristicasDelInmueble.Domicilio.CodigoPostal;
                    rowCaracteristicas["dimension_terreno"] = Decimal.Round(operacion.CaracteristicasDelInmueble.DimensionTerreno, 2).ToString("F");
                    rowCaracteristicas["dimension_construido"] = Decimal.Round(operacion.CaracteristicasDelInmueble.DimensionConstruido, 2).ToString("F");
                    rowCaracteristicas["blindaje"] = operacion.CaracteristicasDelInmueble.Blindaje;
                    rowCaracteristicas["folio_real"] = operacion.CaracteristicasDelInmueble.FolioReal;
                    dsAntilavado.Tables["caracteristicas_inmueble"].Rows.Add(rowCaracteristicas);
                    dsAntilavado.AcceptChanges();

                    DataRow rowContratoInstPublico = dsAntilavado.Tables["contrato_instrumento_publico"].NewRow();
                    rowContratoInstPublico["datos_operacion_Id"] = numOperacion;
                    rowContratoInstPublico["contrato_instrumento_publico_Id"] = numOperacion;
                    dsAntilavado.Tables["contrato_instrumento_publico"].Rows.Add(rowContratoInstPublico);
                    dsAntilavado.AcceptChanges();

                    DataRow rowDatosContrato = dsAntilavado.Tables["datos_contrato"].NewRow();
                    rowDatosContrato["contrato_instrumento_publico_Id"] = numOperacion;
                    rowDatosContrato["fecha_contrato"] = operacion.FechaContrato.ToString("yyyyMMdd");
                    dsAntilavado.Tables["datos_contrato"].Rows.Add(rowDatosContrato);
                    dsAntilavado.AcceptChanges();

                    DataRow rowLiquidacion = dsAntilavado.Tables["datos_liquidacion"].NewRow();
                    rowLiquidacion["datos_operacion_Id"] = numOperacion;
                    rowLiquidacion["datos_liquidacion_Id"] = numOperacion;
                    rowLiquidacion["fecha_pago"] = operacion.Liquidacion.FechaDePago.ToString("yyyyMMdd");
                    rowLiquidacion["forma_pago"] = operacion.Liquidacion.FormaDePago;
                    rowLiquidacion["instrumento_monetario"] = operacion.Liquidacion.ClaveInstrumentoMonetario;
                    rowLiquidacion["moneda"] = operacion.Liquidacion.Moneda;
                    rowLiquidacion["monto_operacion"] = Decimal.Round(operacion.Liquidacion.MontoOperacion, 2).ToString("F");
                    dsAntilavado.Tables["datos_liquidacion"].Rows.Add(rowLiquidacion);
                    dsAntilavado.AcceptChanges();

                    DataRow rowDetalleInstrumento = dsAntilavado.Tables["detalle_instrumento"].NewRow();
                    rowDetalleInstrumento["datos_liquidacion_Id"] = numOperacion;
                    rowDetalleInstrumento["detalle_instrumento_Id"] = numOperacion;
                    dsAntilavado.Tables["detalle_instrumento"].Rows.Add(rowDetalleInstrumento);
                    dsAntilavado.AcceptChanges();

                    if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque != null)
                    {
                        DataRow rowCheque = dsAntilavado.Tables["cheque"].NewRow();
                        rowCheque["detalle_instrumento_Id"] = numOperacion;
                        rowCheque["institucion_credito"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.InstitucionCredito;
                        rowCheque["numero_cuenta"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.NumeroDeCuentaLibrador;
                        rowCheque["numero_cheque"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.NumeroCheque;
                        dsAntilavado.Tables["cheque"].Rows.Add(rowCheque);
                        dsAntilavado.AcceptChanges();
                    }
                    else if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaDelMismoBanco != null)
                    {
                        DataRow rowTransMismoBanco = dsAntilavado.Tables["transferencia_mismo_banco"].NewRow();
                        rowTransMismoBanco["detalle_instrumento_Id"] = numOperacion;
                        rowTransMismoBanco["folio_interno"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaDelMismoBanco.FolioInterno;
                        dsAntilavado.Tables["transferencia_mismo_banco"].Rows.Add(rowTransMismoBanco);
                        dsAntilavado.AcceptChanges();
                    }
                    else if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInterbancaria != null)
                    {
                        DataRow rowTransInterbancaria = dsAntilavado.Tables["transferencia_interbancaria"].NewRow();
                        rowTransInterbancaria["detalle_instrumento_Id"] = numOperacion;
                        rowTransInterbancaria["clave_rastreo"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInterbancaria.ClaveRastreo;
                        dsAntilavado.Tables["transferencia_interbancaria"].Rows.Add(rowTransInterbancaria);
                        dsAntilavado.AcceptChanges();
                    }
                    else if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional != null)
                    {
                        DataRow rowTransInternacional = dsAntilavado.Tables["transferencia_internacional"].NewRow();
                        rowTransInternacional["detalle_instrumento_Id"] = numOperacion;
                        rowTransInternacional["institucion_ordenante"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.InstitucionOrdenante;
                        rowTransInternacional["numero_cuenta"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.NumeroDeCuenta;
                        rowTransInternacional["pais_origen"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.PaisDeOrigen;
                        dsAntilavado.Tables["transferencia_internacional"].Rows.Add(rowTransInternacional);
                        dsAntilavado.AcceptChanges();
                    }
                    numOperacion++;
                }
                return string.Empty;
            }
            catch
            {
                return "Error: Ocurrió un error mientras se generaba la sección operaciones.";
            }
        }

        private string generaArchivo(string pathEsquema, bool venta)
        {
            try
            {
                string path = Properties.Settings.Default.RutaAntilavado + RFC + @"\SPPLD\Reportes\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string fileName = path + DateTime.Now.ToString("yyyyMMdd - HHmmss") + ".xml";
                string xml = dsAntilavado.GetXml();
                xml = xml.Replace("NewDataSet", "archivo");                
                if (!venta)
                {
                    xml = xml.Replace(@"<archivo xmlns=""http://www.uif.shcp.gob.mx/recepcion/ari"">", @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                        <archivo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                                        xsi:schemaLocation=""http://www.uif.shcp.gob.mx/recepcion/ari ari.xsd""
                                        xmlns=""http://www.uif.shcp.gob.mx/recepcion/ari"">");
                }
                else
                {
                    xml = xml.Replace(@"<archivo xmlns=""http://www.uif.shcp.gob.mx/recepcion/inm"">", @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                        <archivo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                                        xsi:schemaLocation=""http://www.uif.shcp.gob.mx/recepcion/inm inm.xsd""
                                        xmlns=""http://www.uif.shcp.gob.mx/recepcion/inm"">");
                }
                FileStream fs = new FileStream(fileName, FileMode.Create);
                using (StreamWriter myxml = new StreamWriter(fs, Encoding.UTF8))
                {
                    myxml.Write(xml);
                }
                string errorValidacion = validaXML(fileName, pathEsquema, venta);
                if (!string.IsNullOrEmpty(errorValidacion))
                {
                    try
                    {
                        if (File.Exists(fileName))
                            File.Delete(fileName);
                    }
                    catch
                    {
                    }
                }
                return errorValidacion;
            }
            catch
            {
                return "Error: No se pudo crear el archivo XML.";
            }
        }

        private string validaXML(string fileName, string pathEsquema, bool venta)
        {
            //moverNodosXml(fileName);
            XmlTextReader textReader = new XmlTextReader(fileName);
            XmlValidatingReader validateReader = new XmlValidatingReader(textReader);
            try
            {
                if (!venta)
                {
                    //validateReader.Schemas.Add("http://www.uif.shcp.gob.mx/recepcion/ari","https://sppld.sat.gob.mx/pld/documentos/links/xsd/ari.xsd");
                    validateReader.Schemas.Add("http://www.uif.shcp.gob.mx/recepcion/ari", pathEsquema);
                    validateReader.ValidationType = ValidationType.Schema;
                    while (validateReader.Read()) ;
                }
                else
                {
                    validateReader.Schemas.Add("http://www.uif.shcp.gob.mx/recepcion/inm", pathEsquema);
                    validateReader.ValidationType = ValidationType.Schema;
                    while (validateReader.Read()) ;
                }
                validateReader.Close();
                textReader.Close();

                return string.Empty;
            }
            catch (Exception ex)
            {
                validateReader.Close();
                textReader.Close();
                return "Error: Validación de XML no aprobada. " + ex.Message;
            }
        }

        private void moverNodosXml(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filename);

                XmlNodeList aviso = xmlDoc.GetElementsByTagName("aviso");
                foreach (XmlElement nodo in aviso)
                {
                    XmlNodeList datosOperacion = nodo.GetElementsByTagName("datos_operacion");
                    foreach (XmlElement dato in datosOperacion)
                    {
                        XmlNodeList datosLiquidacion = dato.GetElementsByTagName("datos_liquidacion");
                        foreach (XmlElement liquid in datosLiquidacion)
                        {
                            XmlNode detallesInstrumentos = liquid.GetElementsByTagName("detalle_instrumento")[0];
                            XmlNode instrumentoMonetario = liquid.GetElementsByTagName("instrumento_monetario")[0];
                            liquid.InsertAfter(detallesInstrumentos, instrumentoMonetario);
                            break;
                        }
                    }
                }
                xmlDoc.Save(filename);
            }
            catch
            {
            }
            xmlDoc = null;
        }

        public string llenaAlertas(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filename);
                XmlNodeList informe = xmlDoc.GetElementsByTagName("informe");
                if (informe.Count <= 0)
                    return "El archivo seleccionado no corresponde a un archivo antilavado válido.";
                XmlNodeList avisos = xmlDoc.GetElementsByTagName("aviso");
                if (avisos.Count <= 0)
                    return "El archivo corresponde a un archivo antilavado en blanco, por lo cual no hay alertas que modificar.";
                List<AlertaPorPersona> alertaPorPersonaList = new List<AlertaPorPersona>();
                foreach (XmlElement aviso in avisos)
                {
                    AlertaPorPersona alertaPorPersona = new AlertaPorPersona();
                    XmlNodeList alertas = aviso.GetElementsByTagName("alerta");
                    string alerta = alertas[0].FirstChild.InnerText;
                    alertaPorPersona.TipoAlerta = Convert.ToInt32(alerta);
                    XmlNodeList personasAviso = aviso.GetElementsByTagName("persona_aviso");
                    string persona = personasAviso[0].FirstChild.FirstChild["rfc"].FirstChild.InnerText;
                    alertaPorPersona.RFCCliente = persona;
                    alertaPorPersonaList.Add(alertaPorPersona);
                }
                alertasPorPersonas = alertaPorPersonaList;
                return string.Empty;
            }
            catch(Exception ex)
            {
                return "Error general al obtener datos del .xml de antilavado." + ex.Message;
            }
        }

        public string modificarAlertas(string fileName, List<AlertaPorPersona> listaAlertas)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(fileName);
                XmlNodeList avisos = xmlDoc.GetElementsByTagName("aviso");
                foreach (AlertaPorPersona alerta in listaAlertas)
                {
                    foreach (XmlElement aviso in avisos)
                    {
                        XmlNodeList personasAviso = aviso.GetElementsByTagName("persona_aviso");
                        if (personasAviso[0].FirstChild.FirstChild["rfc"].FirstChild.InnerText == alerta.RFCCliente)
                        {
                            XmlNodeList alertas = aviso.GetElementsByTagName("alerta");
                            alertas[0].FirstChild.InnerText = alerta.TipoAlerta.ToString();
                        }
                    }
                }
                xmlDoc.Save(fileName);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error al guardar archivo .xml. " + Environment.NewLine + ex.Message;
            }
        }

        public bool esArchivoVenta(string fileName)
        {
            try
            {
                string line = string.Empty;
                using (StreamReader sr = new StreamReader(fileName))
                {
                    line = sr.ReadToEnd();
                }
                return line.Contains(@"http://www.uif.shcp.gob.mx/recepcion/inm");
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Generación de archivo antilavado V2
        public string generar(string pathApplication, GestorReportes.BusinessLayer.EntitiesAntilavado2.Informe informe, bool esEnCeros, bool esVenta)
        {
            EsEnCeros = esEnCeros;
            EsVenta = esVenta;
            string pathXSD = string.Empty;
            if (!esVenta)
                pathXSD = pathApplication + @"\Resources\Antilavado_Arrendamiento2.xsd";
            else
                pathXSD = pathApplication + @"\Resources\Antilavado_Inmuebles2.xsd";
            if (!File.Exists(pathXSD))
                return "Error: No se encontró el archivo de esquema (.xsd), de la ley antilavado (V2) en los recursos.";
            try
            {
                dsAntilavado.ReadXmlSchema(pathXSD);

                string errorInforme = fillInforme(informe);
                if (!string.IsNullOrEmpty(errorInforme))
                    return errorInforme;

                string errorGenerar = generaArchivo(pathXSD, esVenta);
                if (!string.IsNullOrEmpty(errorGenerar))
                    return errorGenerar;
            }
            catch
            {
                return "Error: No se pudo leer el archivo de esquema (.xsd).";
            }
            return string.Empty;
        }

        private string fillInforme(GestorReportes.BusinessLayer.EntitiesAntilavado2.Informe informe)
        {
            try
            {
                DataRow rowInforme = dsAntilavado.Tables["informe"].NewRow();
                rowInforme["mes_reportado"] = informe.MesReportado.ToString("yyyyMM");
                rowInforme["informe_Id"] = 1;
                dsAntilavado.Tables["informe"].Rows.Add(rowInforme);
                dsAntilavado.AcceptChanges();

                string errorSujetoObligado = fillSujetoObligado(informe.SujetoObligado);
                if (!string.IsNullOrEmpty(errorSujetoObligado))
                    return errorSujetoObligado;

                if (!EsEnCeros)
                {
                    string errorAvisos = fillAvisos(informe.Avisos);
                    if (!string.IsNullOrEmpty(errorAvisos))
                        return errorAvisos;
                }
            }
            catch
            {
                return "Error: Ocurrio un problema al generar la sección informe.";
            }
            return string.Empty;
        }

        private string fillSujetoObligado(GestorReportes.BusinessLayer.EntitiesAntilavado2.SujetoObligado sujeto)
        {
            try
            {
                DataRow rowSujetoObligado = dsAntilavado.Tables["sujeto_obligado"].NewRow();
                rowSujetoObligado["informe_Id"] = 1;
                rowSujetoObligado["clave_sujeto_obligado"] = sujeto.ClaveRFC;
                rowSujetoObligado["clave_actividad"] = sujeto.Actividad;
                dsAntilavado.Tables["sujeto_obligado"].Rows.Add(rowSujetoObligado);
                dsAntilavado.AcceptChanges();
                return string.Empty;
            }
            catch
            {
                return "Error: Ocurrio un problema al generar la sección sujeto obligado.";
            }
        }

        private string fillAvisos(List<GestorReportes.BusinessLayer.EntitiesAntilavado2.Aviso> avisos)
        {
            try
            {
                foreach (GestorReportes.BusinessLayer.EntitiesAntilavado2.Aviso aviso in avisos)
                {
                    DataRow rowAviso = dsAntilavado.Tables["aviso"].NewRow();
                    rowAviso["informe_Id"] = 1;
                    rowAviso["aviso_Id"] = numAviso;
                    rowAviso["referencia_aviso"] = aviso.Referencia;
                    rowAviso["prioridad"] = aviso.Prioridad;
                    dsAntilavado.Tables["aviso"].Rows.Add(rowAviso);
                    dsAntilavado.AcceptChanges();
                    string errorAlerta = fillAlerta(aviso.Alerta, numAviso);
                    if (!string.IsNullOrEmpty(errorAlerta))
                        return errorAlerta;
                    string errorPersona = fillPersona(aviso.Persona, numAviso);
                    if (!string.IsNullOrEmpty(errorPersona))
                        return errorPersona;
                    //if (!EsVenta)
                    //{
                        //string errorOperaciones = fillOperaciones(aviso.Operaciones, numAviso);
                        //if (!string.IsNullOrEmpty(errorOperaciones))
                            //return errorOperaciones;
                    //}
                    //else
                    //{
                        string errorOperaciones = fillOperaciones(aviso.Operaciones, numAviso);
                        if (!string.IsNullOrEmpty(errorOperaciones))
                            return errorOperaciones;
                    //}
                    numAviso++;
                }
                return string.Empty;
            }
            catch
            {
                return "Error: Ocurrió un problema al generar la sección avisos.";
            }
        }

        private string fillAlerta(GestorReportes.BusinessLayer.EntitiesAntilavado2.Alerta alerta, int numeroDeAviso)
        {
            try
            {
                DataRow rowAlerta = dsAntilavado.Tables["alerta"].NewRow();
                rowAlerta["tipo_alerta"] = alerta.Tipo;
                rowAlerta["descripcion_alerta"] = alerta.Descripcion;
                rowAlerta["aviso_Id"] = numeroDeAviso;
                dsAntilavado.Tables["alerta"].Rows.Add(rowAlerta);
                dsAntilavado.AcceptChanges();
                return string.Empty;
            }
            catch
            {
                return "Error: Ocurrió un problema al generar la sección alerta.";
            }
        }

        private string fillPersona(GestorReportes.BusinessLayer.EntitiesAntilavado2.Persona persona, int numeroDeAviso)
        {
            try
            {
                DataRow rowPersona = dsAntilavado.Tables["persona_aviso"].NewRow();
                rowPersona["aviso_Id"] = numeroDeAviso;
                rowPersona["persona_aviso_Id"] = numeroDeAviso;
                dsAntilavado.Tables["persona_aviso"].Rows.Add(rowPersona);
                dsAntilavado.AcceptChanges();

                DataRow rowTipoPersona = dsAntilavado.Tables["tipo_persona"].NewRow();
                rowTipoPersona["persona_aviso_Id"] = numeroDeAviso;
                rowTipoPersona["tipo_persona_Id"] = numeroDeAviso;
                dsAntilavado.Tables["tipo_persona"].Rows.Add(rowTipoPersona);
                dsAntilavado.AcceptChanges();

                if (persona.EsPersonaFisica)
                {
                    DataRow rowPersonaFisica = dsAntilavado.Tables["persona_fisica"].NewRow();
                    rowPersonaFisica["tipo_persona_Id"] = numeroDeAviso;
                    //rowPersonaFisica["persona_fisica_Id"] = numeroDeAviso;
                    rowPersonaFisica["nombre"] = persona.Fisica.Nombre;
                    rowPersonaFisica["apellido_paterno"] = persona.Fisica.ApellidoPaterno;
                    rowPersonaFisica["apellido_materno"] = persona.Fisica.ApellidoMaterno;
                    rowPersonaFisica["fecha_nacimiento"] = persona.Fisica.FechaNacimiento.ToString("yyyyMMdd");
                    rowPersonaFisica["rfc"] = persona.Fisica.RFC;
                    if (!string.IsNullOrEmpty(persona.Fisica.CURP))
                        rowPersonaFisica["curp"] = persona.Fisica.CURP;
                    rowPersonaFisica["pais_nacionalidad"] = persona.Fisica.PaisNacionalidad;                    
                    rowPersonaFisica["actividad_economica"] = persona.Fisica.ActividadEconomica;                    
                    dsAntilavado.Tables["persona_fisica"].Rows.Add(rowPersonaFisica);
                    dsAntilavado.AcceptChanges();
                }
                else
                {
                    DataRow rowPersonaMoral = dsAntilavado.Tables["persona_moral"].NewRow();
                    rowPersonaMoral["tipo_persona_Id"] = numeroDeAviso;
                    rowPersonaMoral["persona_moral_Id"] = numeroDeAviso;
                    rowPersonaMoral["denominacion_razon"] = persona.Moral.RazonSocial;
                    rowPersonaMoral["fecha_constitucion"] = persona.Moral.FechaDeConstitucion.ToString("yyyyMMdd");
                    rowPersonaMoral["rfc"] = persona.Moral.RFC;
                    rowPersonaMoral["pais_nacionalidad"] = persona.Moral.PaisNacionalidad;
                    rowPersonaMoral["giro_mercantil"] = persona.Moral.Giro;
                    dsAntilavado.Tables["persona_moral"].Rows.Add(rowPersonaMoral);
                    dsAntilavado.AcceptChanges();

                    DataRow rowApoderado = dsAntilavado.Tables["representante_apoderado"].NewRow();
                    rowApoderado["persona_moral_Id"] = numeroDeAviso;
                    rowApoderado["nombre"] = persona.Moral.Apoderado.Nombre;
                    rowApoderado["apellido_paterno"] = persona.Moral.Apoderado.ApellidoPaterno;
                    rowApoderado["apellido_materno"] = persona.Moral.Apoderado.ApellidoMaterno;
                    rowApoderado["fecha_nacimiento"] = persona.Moral.Apoderado.FechaNacimiento.ToString("yyyyMMdd");
                    rowApoderado["rfc"] = persona.Moral.Apoderado.RFC;
                    if (!string.IsNullOrEmpty(persona.Moral.Apoderado.CURP))
                        rowApoderado["curp"] = persona.Moral.Apoderado.CURP;                    
                    dsAntilavado.Tables["representante_apoderado"].Rows.Add(rowApoderado);
                    dsAntilavado.AcceptChanges();
                }

                DataRow rowDomicilio = dsAntilavado.Tables["tipo_domicilio"].NewRow();
                rowDomicilio["persona_aviso_Id"] = numeroDeAviso;
                rowDomicilio["tipo_domicilio_Id"] = numeroDeAviso;
                dsAntilavado.Tables["tipo_domicilio"].Rows.Add(rowDomicilio);
                dsAntilavado.AcceptChanges();

                DataRow rowDomicilioNacional = dsAntilavado.Tables["nacional"].NewRow();
                rowDomicilioNacional["tipo_domicilio_Id"] = numeroDeAviso;
                rowDomicilioNacional["colonia"] = persona.DomicilioNacional.Colonia;
                rowDomicilioNacional["calle"] = persona.DomicilioNacional.Calle;
                rowDomicilioNacional["numero_exterior"] = persona.DomicilioNacional.NumeroExterior;
                if (!string.IsNullOrEmpty(persona.DomicilioNacional.NumeroInterior))
                    rowDomicilioNacional["numero_interior"] = persona.DomicilioNacional.NumeroInterior;
                rowDomicilioNacional["codigo_postal"] = persona.DomicilioNacional.CodigoPostal;
                dsAntilavado.Tables["nacional"].Rows.Add(rowDomicilioNacional);
                dsAntilavado.AcceptChanges();

                DataRow rowTelefono = dsAntilavado.Tables["telefono"].NewRow();
                rowTelefono["persona_aviso_Id"] = numeroDeAviso;
                rowTelefono["clave_pais"] = persona.Telefono.Pais;
                rowTelefono["numero_telefono"] = persona.Telefono.Numero;
                if (!string.IsNullOrEmpty(persona.Telefono.EMail))
                    rowTelefono["correo_electronico"] = persona.Telefono.EMail;
                dsAntilavado.Tables["telefono"].Rows.Add(rowTelefono);
                dsAntilavado.AcceptChanges();
                return string.Empty;
            }
            catch
            {
                return "Error: Ocurrió un problema al generar la sección persona.";
            }
        }

        private string fillOperaciones(List<GestorReportes.BusinessLayer.EntitiesAntilavado2.Operacion> operaciones, int numeroDeAviso)
        {
            try
            {
                if (!EsVenta)
                {
                    DataRow rowOperacion = dsAntilavado.Tables["detalle_operaciones"].NewRow();
                    rowOperacion["aviso_Id"] = numeroDeAviso;
                    rowOperacion["detalle_operaciones_Id"] = numeroDeAviso;
                    dsAntilavado.Tables["detalle_operaciones"].Rows.Add(rowOperacion);
                    dsAntilavado.AcceptChanges();

                    /*DataRow rowOperacionRealizada = dsAntilavado.Tables["operaciones_realizadas"].NewRow();
                    rowOperacionRealizada["detalle_operaciones_Id"] = numeroDeAviso;
                    rowOperacionRealizada["operaciones_realizadas_Id"] = numeroDeAviso;
                    dsAntilavado.Tables["operaciones_realizadas"].Rows.Add(rowOperacionRealizada);
                    dsAntilavado.AcceptChanges();*/

                    foreach (GestorReportes.BusinessLayer.EntitiesAntilavado2.Operacion operacion in operaciones)
                    {
                        DataRow rowDatosOperacion = dsAntilavado.Tables["datos_operacion"].NewRow();
                        //rowDatosOperacion["operaciones_realizadas_Id"] = numeroDeAviso;
                        rowDatosOperacion["detalle_operaciones_Id"] = numeroDeAviso;
                        rowDatosOperacion["datos_operacion_Id"] = numOperacion;
                        rowDatosOperacion["fecha_operacion"] = operacion.Fecha.ToString("yyyyMMdd");
                        rowDatosOperacion["tipo_operacion"] = operacion.Tipo;
                        dsAntilavado.Tables["datos_operacion"].Rows.Add(rowDatosOperacion);
                        dsAntilavado.AcceptChanges();

                        DataRow rowCaracteristicas = dsAntilavado.Tables["caracteristicas"].NewRow();
                        rowCaracteristicas["datos_operacion_Id"] = numOperacion;
                        rowCaracteristicas["tipo_inmueble"] = operacion.Caracteristicas.First().TipoDeInmueble;
                        rowCaracteristicas["valor_referencia"] = Decimal.Round(operacion.Caracteristicas.First().ValorReferencia, 2).ToString("F");
                        rowCaracteristicas["colonia"] = operacion.Caracteristicas.First().Domicilio.Colonia;
                        rowCaracteristicas["calle"] = operacion.Caracteristicas.First().Domicilio.Calle;
                        rowCaracteristicas["numero_exterior"] = operacion.Caracteristicas.First().Domicilio.NumeroExterior;
                        if (!string.IsNullOrEmpty(operacion.Caracteristicas.First().Domicilio.NumeroInterior))
                            rowCaracteristicas["numero_interior"] = operacion.Caracteristicas.First().Domicilio.NumeroInterior;
                        rowCaracteristicas["codigo_postal"] = operacion.Caracteristicas.First().Domicilio.CodigoPostal;
                        rowCaracteristicas["folio_real"] = operacion.Caracteristicas.First().FolioReal;
                        rowCaracteristicas["fecha_inicio"] = operacion.Caracteristicas.First().FechaInicio.ToString("yyyyMMdd");
                        rowCaracteristicas["fecha_termino"] = operacion.Caracteristicas.First().FechaFin.ToString("yyyyMMdd");
                        dsAntilavado.Tables["caracteristicas"].Rows.Add(rowCaracteristicas);
                        dsAntilavado.AcceptChanges();

                        DataRow rowLiquidacion = dsAntilavado.Tables["datos_liquidacion"].NewRow();
                        rowLiquidacion["datos_operacion_Id"] = numOperacion;
                        //rowLiquidacion["datos_liquidacion_Id"] = numOperacion;
                        rowLiquidacion["fecha_pago"] = operacion.Liquidacion.First().FechaDePago.ToString("yyyyMMdd");
                        rowLiquidacion["forma_pago"] = operacion.Liquidacion.First().FormaDePago;
                        rowLiquidacion["instrumento_monetario"] = operacion.Liquidacion.First().ClaveInstrumentoMonetario;
                        rowLiquidacion["moneda"] = operacion.Liquidacion.First().Moneda;
                        rowLiquidacion["monto_operacion"] = Decimal.Round(operacion.Liquidacion.First().MontoOperacion, 2).ToString("F");
                        dsAntilavado.Tables["datos_liquidacion"].Rows.Add(rowLiquidacion);
                        dsAntilavado.AcceptChanges();

                        /*DataRow rowDetalleInstrumento = dsAntilavado.Tables["detalle_instrumento"].NewRow();
                        rowDetalleInstrumento["datos_liquidacion_Id"] = numOperacion;
                        rowDetalleInstrumento["detalle_instrumento_Id"] = numOperacion;
                        dsAntilavado.Tables["detalle_instrumento"].Rows.Add(rowDetalleInstrumento);
                        dsAntilavado.AcceptChanges();

                        if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque != null)
                        {
                            DataRow rowCheque = dsAntilavado.Tables["cheque"].NewRow();
                            rowCheque["detalle_instrumento_Id"] = numOperacion;
                            rowCheque["institucion_credito"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.InstitucionCredito;
                            rowCheque["numero_cuenta"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.NumeroDeCuentaLibrador;
                            rowCheque["numero_cheque"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.NumeroCheque;
                            dsAntilavado.Tables["cheque"].Rows.Add(rowCheque);
                            dsAntilavado.AcceptChanges();
                        }
                        else if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaDelMismoBanco != null)
                        {
                            DataRow rowTransMismoBanco = dsAntilavado.Tables["transferencia_mismo_banco"].NewRow();
                            rowTransMismoBanco["detalle_instrumento_Id"] = numOperacion;
                            rowTransMismoBanco["folio_interno"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaDelMismoBanco.FolioInterno;
                            dsAntilavado.Tables["transferencia_mismo_banco"].Rows.Add(rowTransMismoBanco);
                            dsAntilavado.AcceptChanges();
                        }
                        else if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInterbancaria != null)
                        {
                            DataRow rowTransInterbancaria = dsAntilavado.Tables["transferencia_interbancaria"].NewRow();
                            rowTransInterbancaria["detalle_instrumento_Id"] = numOperacion;
                            rowTransInterbancaria["clave_rastreo"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInterbancaria.ClaveRastreo;
                            dsAntilavado.Tables["transferencia_interbancaria"].Rows.Add(rowTransInterbancaria);
                            dsAntilavado.AcceptChanges();
                        }
                        else if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional != null)
                        {
                            DataRow rowTransInternacional = dsAntilavado.Tables["transferencia_internacional"].NewRow();
                            rowTransInternacional["detalle_instrumento_Id"] = numOperacion;
                            rowTransInternacional["institucion_ordenante"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.InstitucionOrdenante;
                            rowTransInternacional["numero_cuenta"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.NumeroDeCuenta;
                            rowTransInternacional["pais_origen"] = operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.PaisDeOrigen;
                            dsAntilavado.Tables["transferencia_internacional"].Rows.Add(rowTransInternacional);
                            dsAntilavado.AcceptChanges();
                        }*/
                        numOperacion++;
                    }
                }
                else
                {
                    DataRow rowOperacion = dsAntilavado.Tables["detalle_operaciones"].NewRow();
                    rowOperacion["aviso_Id"] = numeroDeAviso;
                    rowOperacion["detalle_operaciones_Id"] = numeroDeAviso;
                    dsAntilavado.Tables["detalle_operaciones"].Rows.Add(rowOperacion);
                    dsAntilavado.AcceptChanges();

                    foreach (var operacion in operaciones)
                    {
                        DataRow rowDatosOperacion = dsAntilavado.Tables["datos_operacion"].NewRow();
                        rowDatosOperacion["detalle_operaciones_Id"] = numeroDeAviso;
                        rowDatosOperacion["datos_operacion_Id"] = numOperacion;
                        rowDatosOperacion["fecha_operacion"] = operacion.Fecha.ToString("yyyyMMdd");
                        rowDatosOperacion["tipo_operacion"] = operacion.Tipo;
                        rowDatosOperacion["figura_cliente"] = operacion.FiguraCliente;
                        rowDatosOperacion["figura_so"] = operacion.FiguraSO;
                        dsAntilavado.Tables["datos_operacion"].Rows.Add(rowDatosOperacion);
                        dsAntilavado.AcceptChanges();
                                                
                        DataRow rowCaracteristicas = dsAntilavado.Tables["caracteristicas_inmueble"].NewRow();
                        rowCaracteristicas["datos_operacion_Id"] = numOperacion;
                        rowCaracteristicas["tipo_inmueble"] = operacion.Caracteristicas.First().TipoDeInmueble;
                        rowCaracteristicas["valor_pactado"] = operacion.ValorPactado.ToString("F");
                        rowCaracteristicas["colonia"] = operacion.Caracteristicas.First().Domicilio.Colonia;
                        rowCaracteristicas["calle"] = operacion.Caracteristicas.First().Domicilio.Calle;
                        rowCaracteristicas["numero_exterior"] = operacion.Caracteristicas.First().Domicilio.NumeroExterior;
                        if (!string.IsNullOrEmpty(operacion.Caracteristicas.First().Domicilio.NumeroInterior))
                            rowCaracteristicas["numero_interior"] = operacion.Caracteristicas.First().Domicilio.NumeroInterior;
                        rowCaracteristicas["codigo_postal"] = operacion.Caracteristicas.First().Domicilio.CodigoPostal;
                        rowCaracteristicas["dimension_terreno"] = Decimal.Round(operacion.Caracteristicas.First().DimensionTerreno, 2).ToString("F");
                        rowCaracteristicas["dimension_construido"] = Decimal.Round(operacion.Caracteristicas.First().DimensionConstruccion, 2).ToString("F");
                        rowCaracteristicas["folio_real"] = operacion.Caracteristicas.First().FolioReal;
                        dsAntilavado.Tables["caracteristicas_inmueble"].Rows.Add(rowCaracteristicas);
                        dsAntilavado.AcceptChanges();
                        
                        DataRow rowContratoInstPublico = dsAntilavado.Tables["contrato_instrumento_publico"].NewRow();
                        rowContratoInstPublico["datos_operacion_Id"] = numOperacion;
                        rowContratoInstPublico["contrato_instrumento_publico_Id"] = numOperacion;
                        dsAntilavado.Tables["contrato_instrumento_publico"].Rows.Add(rowContratoInstPublico);
                        dsAntilavado.AcceptChanges();
                        
                        DataRow rowDatosContrato = dsAntilavado.Tables["datos_contrato"].NewRow();
                        rowDatosContrato["contrato_instrumento_publico_Id"] = numOperacion;
                        rowDatosContrato["fecha_contrato"] = operacion.FechaContrato.ToString("yyyyMMdd");
                        dsAntilavado.Tables["datos_contrato"].Rows.Add(rowDatosContrato);
                        dsAntilavado.AcceptChanges();
                        
                        DataRow rowLiquidacion = dsAntilavado.Tables["datos_liquidacion"].NewRow();
                        rowLiquidacion["datos_operacion_Id"] = numOperacion;
                        rowLiquidacion["fecha_pago"] = operacion.Liquidacion.First().FechaDePago.ToString("yyyyMMdd");
                        rowLiquidacion["forma_pago"] = operacion.Liquidacion.First().FormaDePago;
                        rowLiquidacion["instrumento_monetario"] = operacion.Liquidacion.First().ClaveInstrumentoMonetario;
                        rowLiquidacion["moneda"] = operacion.Liquidacion.First().Moneda;
                        rowLiquidacion["monto_operacion"] = Decimal.Round(operacion.Liquidacion.First().MontoOperacion, 2).ToString("F");
                        dsAntilavado.Tables["datos_liquidacion"].Rows.Add(rowLiquidacion);
                        dsAntilavado.AcceptChanges();
                        numOperacion++;
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error: Ocurrió un error mientras se generaba la sección operaciones. " + ex.Message;
            }
        }
        #endregion
    }
}
