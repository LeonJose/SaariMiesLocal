using System;
using System.Collections.Generic;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesAntiLavado;
using System.Data;
using GestorReportes.BusinessLayer.DataAccessLayer;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class InformeAntilavado
    {
        public int NumReferencia { get; set; }
        private string RFCContribuyente { get; set; }
        private bool LimitarASeptiembre { get; set; }
        private DateTime FechaSolicitada { get; set; }
        private DateTime periodoInicial = DateTime.Now;
        private DateTime periodoFinal = DateTime.Now;
        private DateTime periodoInicialMenos6M = DateTime.Now;
        public string Errores { get; set; }
        public string Warnings { get; set; }

        public InformeAntilavado(string rfcContrib, DateTime fecha, bool desdeSeptiembre)
        {
            RFCContribuyente = rfcContrib;
            FechaSolicitada = fecha;
            LimitarASeptiembre = desdeSeptiembre;
        }

        private string validaDatos(bool esVenta)
        {
            string errores = string.Empty;
            int registroReferencia = 0;
            registroReferencia = Informes.getReferenciaPorRFC(RFCContribuyente, esVenta);
            if (registroReferencia > 0)
                NumReferencia = registroReferencia;
            else
                errores += "- No se pudo obtener el siguiente registro de referencia." + Environment.NewLine;
            //Comentado para pruebas
            DateTime primerFechaValida = new DateTime(2013, 09, 01);
            if (FechaSolicitada < primerFechaValida)
            {
                errores += "- La fecha solicitada corresponde a una anterior a la primer fecha de emisión válida de la ley antilavado (Septiembre 2013)" + Environment.NewLine;
            }
            else
            {
                if (FechaSolicitada.Year > DateTime.Now.Year)
                {
                    errores += "- No se puede obtener un archivo que no corresponda al año en curso" + Environment.NewLine;
                }
                else if (FechaSolicitada.Year == DateTime.Now.Year)
                {
                    //mismo año
                    if (FechaSolicitada.Month > DateTime.Now.Month)
                    {
                        errores += "- No se puede obtener un archivo futuro" + Environment.NewLine;
                    }
                }
            }
            try
            {
                periodoInicial = new DateTime(FechaSolicitada.Year, FechaSolicitada.Month, 1);
                periodoFinal = periodoInicial.AddMonths(1).AddDays(-1);                
                periodoInicialMenos6M = periodoInicial.AddMonths(-5);
                if (LimitarASeptiembre)
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
            if (string.IsNullOrEmpty(RFCContribuyente))
                errores += "- No se pudo obtener el RFC del contribuyente" + Environment.NewLine;
            return errores;
        }

        public Informe GetInformeData(bool esVenta)
        {
            string error = string.Empty;
            error = validaDatos(esVenta);
            if (!string.IsNullOrEmpty(error))
            {
                Errores = "Ocurrieron los siguientes errores de validación: " + Environment.NewLine + error;
                return null;
            }
            try
            {
                Informe informe = new Informe();
                informe.MesReportado = FechaSolicitada;
                informe.SujetoObligado = getSujetoObligadoActual();
                if (!esVenta)
                    informe.Avisos = getAvisos();
                else
                {
                    informe.SujetoObligado.Actividad = "INM";
                    informe.Avisos = getAvisos(true);//aqui va para ventas
                }
                return informe;                
            }
            catch(Exception ex)
            {
                Errores = "Ocurrió uno o más errores: " + Environment.NewLine + ex.Message;
                return null;
            }
        }

        public Informe validaEstructura(Informe informe, bool venta)
        {
            Errores = string.Empty;
            Warnings = string.Empty;
            try
            {//comentado para pruebas
                
                if (informe.MesReportado.Year < 2013)
                {
                    Errores += "- No se puede emitir un informe anterior a Octubre del 2013." + Environment.NewLine;
                }
                else
                {
                    if (informe.MesReportado.Year == 2013)
                    {
                        if (informe.MesReportado.Month < 10)
                            Errores += "- No se puede emitir un informe anterior a Octubre del 2013." + Environment.NewLine;
                    }
                    if (informe.MesReportado.Year == DateTime.Now.Year)
                    {
                        if (informe.MesReportado.Month > DateTime.Now.Month)
                            Errores += "- No se puede emitir un informe con una fecha posterior." + Environment.NewLine;
                    }
                    else
                    {
                        if (DateTime.Now.Year <= informe.MesReportado.Year /*|| informe.MesReportado.Month != 12 quitado para la 2.4.4.5*/)
                            Errores += "- No se puede emitir un informe con esa fecha." + Environment.NewLine;
                    }
                }
                if (informe.SujetoObligado.ClaveRFC.Length != 12 && informe.SujetoObligado.ClaveRFC.Length != 13)
                    Errores += "- La clave del sujeto obligado no corresponde a un RFC." + Environment.NewLine;
                if (informe.Avisos == null)
                {
                    Errores += "- No se encontraron avisos válidos." + Environment.NewLine;
                }
                else
                {
                    foreach (Aviso aviso in informe.Avisos)
                    {
                        if (aviso.Persona == null)
                            Errores += "- No se encontró una persona valida relacionada al aviso." + Environment.NewLine;
                        else
                        {
                            if (aviso.Persona.EsPersonaFisica)
                            {
                                if (aviso.Persona.Fisica == null)
                                    Errores += "- No se encontró una persona fisica valida relacionada al aviso." + Environment.NewLine;
                                else
                                {
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.Nombre))
                                        Errores += "- No se encontró un nombre para la persona fisica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.ApellidoPaterno))
                                        Errores += "- No se encontró un apellido paterno para la persona fisica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.ApellidoMaterno))
                                        Errores += "- No se encontró un apellido materno para la persona fisica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    if (aviso.Persona.Fisica.FechaNacimiento >= DateTime.Now)
                                        Errores += "- La fecha de nacimiento de la persona no es válida. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.RFC))
                                        Errores += "- No se encontró un RFC para la persona fisica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    else
                                    {
                                        if (aviso.Persona.Fisica.RFC.Length != 13)
                                            Errores += "- El dato " + aviso.Persona.Fisica.RFC + " no corresponde a un RFC válido. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    }
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.CURP))
                                        Warnings += "- No se encontró el atributo opcional CURP para la persona fisica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    else
                                    {
                                        if (aviso.Persona.Fisica.CURP.Length != 18)
                                        {
                                            Warnings += " - El dato " + aviso.Persona.Fisica.CURP + " no corresponde a una CURP válida, se omitirádado que es opcional. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                            aviso.Persona.Fisica.CURP = string.Empty;
                                        }
                                    }
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.ActividadEconomica))
                                        Errores += "- No se encontró una actividad económica para la persona fisica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    else
                                    {
                                        if (aviso.Persona.Fisica.ActividadEconomica.Length != 7)
                                            Errores += "- La actividad económica seleccionada no es válida. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                        else
                                        {
                                            try
                                            {
                                                Convert.ToInt32(aviso.Persona.Fisica.ActividadEconomica);
                                            }
                                            catch
                                            {
                                                Errores += "- La actividad económica seleccionada debe ser numérica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                            }
                                        }
                                    }
                                    if (aviso.Persona.Fisica.TipoIdentificacion < 1 || aviso.Persona.Fisica.TipoIdentificacion > 13)
                                        Errores += "- El tipo de identificación de la persona fisica no es válido. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    else
                                    {
                                        if (aviso.Persona.Fisica.TipoIdentificacion > 10)
                                        {
                                            if (string.IsNullOrEmpty(aviso.Persona.Fisica.OtraIdentificacion))
                                                Errores += "- Es obligatorio especificar que identificación presentó la persona fisica, cuando se selecciona el tipo otro. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                        }
                                    }
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.Autoridad))
                                        Errores += "- Debe existir una autoridad emisora de la identificación. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                    if (string.IsNullOrEmpty(aviso.Persona.Fisica.NumeroIdentificacion))
                                        Errores += "- Debe existir un número identificación de la persona fisica. Referencia: " + aviso.Persona.Fisica.ReferenciaInterna + Environment.NewLine;
                                }
                            }
                            else
                            {
                                if (aviso.Persona.Moral == null)
                                    Errores += "- No se encontró una persona moral válida relacionada al aviso." + Environment.NewLine;
                                else
                                {
                                    if(string.IsNullOrEmpty(aviso.Persona.Moral.RazonSocial))
                                        Errores += "- No se encontró la razón social para la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                    if (aviso.Persona.Moral.FechaDeConstitucion >= DateTime.Now)
                                        Errores += "- La fecha de constitución de la persona moral es incorrecta. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                    if (string.IsNullOrEmpty(aviso.Persona.Moral.RFC))
                                        Errores += "- No se encontró un RFC para la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                    else
                                    {
                                        if (aviso.Persona.Moral.RFC.Length != 12)
                                            Errores += "- El dato " + aviso.Persona.Moral.RFC + " no corresponde a un RFC válido. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                    }
                                    if (string.IsNullOrEmpty(aviso.Persona.Moral.Giro))
                                        Errores += "- No se encontró un giro económico para la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                    else
                                    {
                                        if (aviso.Persona.Moral.Giro.Length != 7)
                                            Errores += "- El giro económico seleccionado no es válido. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        else
                                        {
                                            try
                                            {
                                                Convert.ToInt32(aviso.Persona.Moral.Giro);
                                            }
                                            catch
                                            {
                                                Errores += "- El giro económico seleccionado debe ser numérico. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                            }
                                        }
                                    }
                                    if (aviso.Persona.Moral.Apoderado == null)
                                        Errores += "- Debe existir un apoderado legal para la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                    else
                                    {
                                        if (string.IsNullOrEmpty(aviso.Persona.Moral.Apoderado.Nombre))
                                            Errores += "- No se encontró un nombre para el representante legal de la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        if (string.IsNullOrEmpty(aviso.Persona.Moral.Apoderado.ApellidoPaterno))
                                            Errores += "- No se encontró un apellido paterno para el representante legal de la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        if (string.IsNullOrEmpty(aviso.Persona.Moral.Apoderado.ApellidoMaterno))
                                            Errores += "- No se encontró un apellido materno para el representante legal de la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        if (aviso.Persona.Moral.Apoderado.FechaNacimiento >= DateTime.Now)
                                            Errores += "- La fecha de nacimiento del representante legal no es válida. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        if (string.IsNullOrEmpty(aviso.Persona.Moral.Apoderado.RFC))
                                            Errores += "- No se encontró un RFC para el representante legal de la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        else
                                        {
                                            if (aviso.Persona.Moral.Apoderado.RFC.Length != 13)
                                                Errores += "- El dato " + aviso.Persona.Moral.Apoderado.RFC + " no corresponde a un RFC válido. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        }
                                        if (string.IsNullOrEmpty(aviso.Persona.Moral.Apoderado.CURP))
                                            Warnings += "- No se encontró el atributo opcional CURP para el apoderado de la persona moral. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        else
                                        {
                                            if (aviso.Persona.Moral.Apoderado.CURP.Length != 18)
                                            {
                                                Warnings += " - El dato " + aviso.Persona.Moral.Apoderado.CURP + " no corresponde a una CURP válida para el apoderado, se omitirá dado que es opcional. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                                aviso.Persona.Moral.Apoderado.CURP = string.Empty;
                                            }
                                        }
                                        if (aviso.Persona.Moral.Apoderado.TipoIdentificacion < 1 || aviso.Persona.Moral.Apoderado.TipoIdentificacion > 13)
                                            Errores += "- El tipo de identificación del representante legal no es válido. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        else
                                        {
                                            if (aviso.Persona.Moral.Apoderado.TipoIdentificacion > 10)
                                            {
                                                if (string.IsNullOrEmpty(aviso.Persona.Moral.Apoderado.OtraIdentificacion))
                                                    Errores += "- Es obligatorio especificar que identificación presentó el apoderado de la persona moral, cuando se selecciona el tipo otro. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                            }
                                        }
                                        if (string.IsNullOrEmpty(aviso.Persona.Moral.Apoderado.Autoridad))
                                            Errores += "- Debe existir una autoridad emisora de la identificación. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                        if (string.IsNullOrEmpty(aviso.Persona.Moral.Apoderado.NumeroIdentificacion))
                                            Errores += "- Debe existir un número identificación de la persona fisica. Referencia: " + aviso.Persona.Moral.RazonSocial + Environment.NewLine;
                                    }
                                }
                            }
                            if (aviso.Persona.DomicilioNacional == null)
                                Errores += "- Debe existir un domicilio para la persona del aviso." + Environment.NewLine;
                            else
                            {
                                if (string.IsNullOrEmpty(aviso.Persona.DomicilioNacional.Colonia))
                                    Errores += "- No se encontró la colonia para la persona del aviso. Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                if (string.IsNullOrEmpty(aviso.Persona.DomicilioNacional.Calle))
                                    Errores += "- No se encontró la calle para la persona del aviso. Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                if (string.IsNullOrEmpty(aviso.Persona.DomicilioNacional.NumeroExterior))
                                    Errores += "- No se encontró el número exterior para la persona del aviso. Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                if (string.IsNullOrEmpty(aviso.Persona.DomicilioNacional.NumeroInterior))
                                    Warnings += "- No se encontró el campo opcional numero interior para la persona del aviso. Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                if (string.IsNullOrEmpty(aviso.Persona.DomicilioNacional.CodigoPostal))
                                    Errores += "- No se encontró el código postal para la persona del aviso. Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                else
                                {
                                    if (aviso.Persona.DomicilioNacional.CodigoPostal.Length != 5)
                                        Errores += "- El código postal debe ser de 5 números.  Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                    else
                                    {
                                        try
                                        {
                                            Convert.ToInt32(aviso.Persona.DomicilioNacional.CodigoPostal);
                                        }
                                        catch
                                        {
                                            Errores += "- El código postal solo debe contener números. Referencia: " + aviso.Persona.DomicilioNacional.ReferenciaInterna + Environment.NewLine;
                                        }
                                    }
                                }
                            }
                            if (aviso.Persona.Telefono == null)
                                Errores += "- Debe existir un telefono para la persona del aviso. Referencia: " + aviso.Persona.Telefono.ReferenciaInterna + Environment.NewLine;
                            else
                            {
                                if (string.IsNullOrEmpty(aviso.Persona.Telefono.Numero))
                                    Errores += "- No se encontró un número telefónico para la persona del aviso. Referencia: " + aviso.Persona.Telefono.ReferenciaInterna + Environment.NewLine;
                                else
                                {
                                    if (aviso.Persona.Telefono.Numero.Length < 10 || aviso.Persona.Telefono.Numero.Length > 12)
                                        Errores += "- El número telefónico para la persona del aviso debe constar de entre 10 y 12 digitos. Referencia: " + aviso.Persona.Telefono.ReferenciaInterna + Environment.NewLine;
                                    else
                                    {
                                        try
                                        {
                                            Convert.ToInt64(aviso.Persona.Telefono.Numero);
                                        }
                                        catch
                                        {
                                            Errores += "- El número telefónico debe contener solo caracteres númericos. Referencia: " + aviso.Persona.Telefono.ReferenciaInterna + Environment.NewLine;
                                        }
                                    }
                                }
                                if (string.IsNullOrEmpty(aviso.Persona.Telefono.EMail))
                                    Warnings += "- No se encontró el campo e-mail para la persona del aviso. El archivo se generará sin el mismo dado que es opcional. Referencia: " + aviso.Persona.Telefono.ReferenciaInterna + Environment.NewLine;
                                else
                                {
                                    if (!aviso.Persona.Telefono.EMail.Contains("@") || !aviso.Persona.Telefono.EMail.Contains("."))
                                    {
                                        Warnings += "- El e-mail para la persona del aviso no es válido. El archivo se generará sin el mismo dado que es opcional.  Referencia: " + aviso.Persona.Telefono.ReferenciaInterna + Environment.NewLine;
                                        aviso.Persona.Telefono.EMail = string.Empty;
                                    }
                                }
                            }
                        }
                        if (!venta)
                        {
                            if (aviso.Operaciones.Count <= 0)
                                Errores += "- Debe existir al menos una operación en el aviso." + Environment.NewLine;
                            else
                            {
                                foreach (Operacion operacion in aviso.Operaciones)
                                {//comentado para pruebas
                                    if (operacion.Fecha.Year < 2013)
                                    {
                                        Errores += "- La fecha de operacion no puede ser anterior a Octubre del 2013. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                    }
                                    else
                                    {
                                        if (operacion.Fecha.Year == 2013)
                                        {
                                            if (operacion.Fecha.Month < 10)
                                                Errores += "- La fecha de operacion no puede ser anterior a Octubre del 2013. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                        if (operacion.Fecha.Year == DateTime.Now.Year)
                                        {
                                            if (operacion.Fecha.Month > DateTime.Now.Month)
                                                Errores += "- No se puede emitir una operacion con una fecha de operacion posterior. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                        else
                                        {
                                            if (DateTime.Now.Year <= operacion.Fecha.Year /*|| operacion.Fecha.Month != 12*/)
                                                Errores += "- No se puede emitir una operacion con esa fecha de operacion. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                    }
                                    if (operacion.CaracteristicasDelInmueble == null)
                                        Errores += "- No se encontraron los datos para las caracteristicas del inmueble." + Environment.NewLine;
                                    else
                                    {
                                        if (operacion.CaracteristicasDelInmueble.TipoDeInmueble.ToString().Length < 1 || operacion.CaracteristicasDelInmueble.TipoDeInmueble.ToString().Length > 3)
                                            Errores += "- El tipo de inmueble debe conformarse por un rango entre 1 y 3 caracteres. Referencia: " + operacion.CaracteristicasDelInmueble.ReferenciaInterna + Environment.NewLine;
                                        else
                                        {
                                            try
                                            {
                                                if (Convert.ToInt32(operacion.CaracteristicasDelInmueble.TipoDeInmueble) <= 0)
                                                    Errores += "- Debe existir un tipo de inmueble válido. Referencia: " + operacion.CaracteristicasDelInmueble.ReferenciaInterna + Environment.NewLine;
                                            }
                                            catch
                                            {
                                                Errores += "- El tipo de inmueble solo permite caracteres númericos. Referencia: " + operacion.CaracteristicasDelInmueble.ReferenciaInterna + Environment.NewLine;
                                            }
                                        }
                                        if (operacion.CaracteristicasDelInmueble.ValorCatastral <= 0)
                                            Errores += "- El valor catastral del inmueble debe ser mayor a 0.00. Referencia: " + operacion.CaracteristicasDelInmueble.ReferenciaInterna + Environment.NewLine;
                                        if (string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.FolioReal))
                                            Errores += "- El folio real del registro publico del inmueble es obligatorio. Referencia: " + operacion.CaracteristicasDelInmueble.ReferenciaInterna + Environment.NewLine;
                                        if (operacion.CaracteristicasDelInmueble.Domicilio == null)
                                            Errores += "- Debe existir un domicilio para el inmueble de la operación. Referencia: " + operacion.CaracteristicasDelInmueble.ReferenciaInterna + Environment.NewLine;
                                        else
                                        {
                                            if (string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.Domicilio.Colonia))
                                                Errores += "- No se encontró la colonia para el inmueble de la operación. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                            if (string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.Domicilio.Calle))
                                                Errores += "- No se encontró la calle para el inmueble de la operación. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                            if (string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.Domicilio.NumeroExterior))
                                                Errores += "- No se encontró el número exterior para el inmueble de la operación. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                            if (string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.Domicilio.NumeroInterior))
                                                Warnings += "- No se encontró el campo opcional número interior para el inmueble de la operación. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                            if (string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.Domicilio.CodigoPostal))
                                                Errores += "- No se encontró el código postal para el inmueble de la operación. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                            else
                                            {
                                                if (operacion.CaracteristicasDelInmueble.Domicilio.CodigoPostal.Length != 5)
                                                    Errores += "- El código postal del inmueble debe ser de 5 números. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                else
                                                {
                                                    try
                                                    {
                                                        Convert.ToInt32(operacion.CaracteristicasDelInmueble.Domicilio.CodigoPostal);
                                                    }
                                                    catch
                                                    {
                                                        Errores += "- El código postal del inmueble solo debe contener números. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (operacion.Liquidacion == null)
                                        Errores += "- No se encontraron los datos para la liquidación de la operación." + Environment.NewLine;
                                    else
                                    {//comentado para pruebas
                                        if (operacion.Liquidacion.FechaDePago.Year < 2013)
                                        {
                                            Errores += "- La fecha de liquidación no puede ser anterior a Octubre del 2013. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                        else
                                        {
                                            if (operacion.Liquidacion.FechaDePago.Year == 2013)
                                            {
                                                if (operacion.Liquidacion.FechaDePago.Month < 10)
                                                    Errores += "- La fecha de liquidación no puede ser anterior a Octubre del 2013. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                            }
                                            if (operacion.Liquidacion.FechaDePago.Year == DateTime.Now.Year)
                                            {
                                                if (operacion.Liquidacion.FechaDePago.Month > DateTime.Now.Month)
                                                    Errores += "- No se puede liquidar una operacion en una fecha posterior. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                            }
                                            else
                                            {
                                                if (DateTime.Now.Year <= operacion.Liquidacion.FechaDePago.Year /*|| operacion.Liquidacion.FechaDePago.Month != 12*/)
                                                    Errores += "- No se puede liquidar una operacion en esa fecha. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                            }
                                        }
                                        if (operacion.Liquidacion.ClaveInstrumentoMonetario < 1 || operacion.Liquidacion.ClaveInstrumentoMonetario > 14)
                                            Errores += "- No se reconoce el instrumento monetario de pago. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                        if (operacion.Liquidacion.DetalleDelInstrumentoMonetario == null)
                                            Errores += "- No se encontraron los detalles del instrumento monetario para la liquidación de la operación. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                        else
                                        {
                                            if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque == null && operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaDelMismoBanco == null && operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInterbancaria == null && operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional == null)
                                                Errores += "- No se encontraron los detalles del instrumento monetario para la liquidación de la operación. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                            else
                                            {
                                                if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque != null)
                                                {
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.InstitucionCredito))
                                                        Errores += "- Debe existir una institución de crédito al recibir un pago con cheque. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.NumeroDeCuentaLibrador))
                                                        Errores += "- Debe existir un número de cuenta al recibir un pago con cheque. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.NumeroCheque))
                                                        Errores += "- Debe existir un número de cheque al recibir un pago con cheque. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                }
                                                else if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInterbancaria != null)
                                                {
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInterbancaria.ClaveRastreo))
                                                        Errores += "- Debe existir una clave de rastreo al recibir un pago por transferencia interbancaria. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                }
                                                else if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaDelMismoBanco != null)
                                                {
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaDelMismoBanco.FolioInterno))
                                                        Errores += "- Debe existir un folio interno al recibir un pago por transferencia del mismo banco. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.InstitucionOrdenante))
                                                        Errores += "- Debe existir una institución ordenante al recibir un pago por transferencia internacional. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.NumeroDeCuenta))
                                                        Errores += "- Debe existir un número de cuenta al recibir un pago por transferencia internacional. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.PaisDeOrigen))
                                                        Errores += "- Debe existir un pais de origen al recibir un pago por transferencia internacional. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                }
                                            }
                                        }
                                        if (operacion.Liquidacion.MontoOperacion <= 0)
                                            Errores += "- Solo se permiten montos de operacion mayores a cero. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (aviso.OperacionesVenta.Count <= 0)
                                Errores += "- Debe existir al menos una operación en el aviso." + Environment.NewLine;
                            else
                            {
                                foreach (OperacionVenta operacion in aviso.OperacionesVenta)
                                {
                                    if (operacion.Fecha.Year < 2013)
                                    {
                                        Errores += "- La fecha de operacion no puede ser anterior a Octubre del 2013. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                    }
                                    else
                                    {
                                        if (operacion.Fecha.Year == 2013)
                                        {
                                            if (operacion.Fecha.Month < 10)
                                                Errores += "- La fecha de operacion no puede ser anterior a Octubre del 2013. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                        if (operacion.Fecha.Year == DateTime.Now.Year)
                                        {
                                            if (operacion.Fecha.Month > DateTime.Now.Month)
                                                Errores += "- No se puede emitir una operacion con una fecha de operacion posterior. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                        else
                                        {
                                            if (DateTime.Now.Year <= operacion.Fecha.Year /*|| operacion.Fecha.Month != 12*/)
                                                Errores += "- No se puede emitir una operacion con esa fecha de operacion. Referencia: " + operacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                    }
                                    if (operacion.TipoDeInmueble.ToString().Length < 1 || operacion.TipoDeInmueble.ToString().Length > 3)
                                        Errores += "- El tipo de inmueble debe conformarse por un rango entre 1 y 3 caracteres. Referencia: " + operacion.CaracteristicasDelInmueble.ReferenciaInterna + Environment.NewLine;
                                    else
                                    {
                                        try
                                        {
                                            if (Convert.ToInt32(operacion.TipoDeInmueble) <= 0)
                                                Errores += "- Debe existir un tipo de inmueble válido. Referencia: " + operacion.CaracteristicasDelInmueble.ReferenciaInterna + Environment.NewLine;
                                        }
                                        catch
                                        {
                                            Errores += "- El tipo de inmueble solo permite caracteres númericos. Referencia: " + operacion.CaracteristicasDelInmueble.ReferenciaInterna + Environment.NewLine;
                                        }
                                    }
                                    if (operacion.MontoPactado <= 0)
                                        Errores += "- El valor del monto pactado debe ser mayor a 0.00. Referencia: " + operacion.CaracteristicasDelInmueble.ReferenciaInterna + Environment.NewLine;
                                    if (operacion.CaracteristicasDelInmueble == null)
                                        Errores += "- No se encontraron los datos para las caracteristicas del inmueble." + Environment.NewLine;
                                    else
                                    {
                                        if (string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.FolioReal))
                                            Errores += "- El folio real del registro publico del inmueble es obligatorio. Referencia: " + operacion.CaracteristicasDelInmueble.ReferenciaInterna + Environment.NewLine;
                                        if (operacion.CaracteristicasDelInmueble.Domicilio == null)
                                            Errores += "- Debe existir un domicilio para el inmueble de la operación. Referencia: " + operacion.CaracteristicasDelInmueble.ReferenciaInterna + Environment.NewLine;
                                        else
                                        {
                                            if (string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.Domicilio.Colonia))
                                                Errores += "- No se encontró la colonia para el inmueble de la operación. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                            if (string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.Domicilio.Calle))
                                                Errores += "- No se encontró la calle para el inmueble de la operación. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                            if (string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.Domicilio.NumeroExterior))
                                                Errores += "- No se encontró el número exterior para el inmueble de la operación. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                            if (string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.Domicilio.NumeroInterior))
                                                Warnings += "- No se encontró el campo opcional número interior para el inmueble de la operación. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                            if (string.IsNullOrEmpty(operacion.CaracteristicasDelInmueble.Domicilio.CodigoPostal))
                                                Errores += "- No se encontró el código postal para el inmueble de la operación. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                            else
                                            {
                                                if (operacion.CaracteristicasDelInmueble.Domicilio.CodigoPostal.Length != 5)
                                                    Errores += "- El código postal del inmueble debe ser de 5 números. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                else
                                                {
                                                    try
                                                    {
                                                        Convert.ToInt32(operacion.CaracteristicasDelInmueble.Domicilio.CodigoPostal);
                                                    }
                                                    catch
                                                    {
                                                        Errores += "- El código postal del inmueble solo debe contener números. Referencia: " + operacion.CaracteristicasDelInmueble.Domicilio.ReferenciaInterna + Environment.NewLine;
                                                    }
                                                }
                                            }

                                        }
                                    }
                                    if (operacion.Liquidacion == null)
                                        Errores += "- No se encontraron los datos para la liquidación de la operación." + Environment.NewLine;
                                    else
                                    {//comentado para pruebas
                                        if (operacion.Liquidacion.FechaDePago.Year < 2013)
                                        {
                                            Errores += "- La fecha de liquidación no puede ser anterior a Octubre del 2013. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                        }
                                        else
                                        {
                                            if (operacion.Liquidacion.FechaDePago.Year == 2013)
                                            {
                                                if (operacion.Liquidacion.FechaDePago.Month < 10)
                                                    Errores += "- La fecha de liquidación no puede ser anterior a Octubre del 2013. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                            }
                                            if (operacion.Liquidacion.FechaDePago.Year == DateTime.Now.Year)
                                            {
                                                if (operacion.Liquidacion.FechaDePago.Month > DateTime.Now.Month)
                                                    Errores += "- No se puede liquidar una operacion en una fecha posterior. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                            }
                                            else
                                            {
                                                if (DateTime.Now.Year <= operacion.Liquidacion.FechaDePago.Year || operacion.Liquidacion.FechaDePago.Month != 12)
                                                    Errores += "- No se puede liquidar una operacion en esa fecha. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                            }
                                        }
                                        if (operacion.Liquidacion.FormaDePago <= 0)
                                            Errores += "- No se encontró una forma de pago correcta" + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                        if (operacion.Liquidacion.ClaveInstrumentoMonetario < 1 || operacion.Liquidacion.ClaveInstrumentoMonetario > 14)
                                            Errores += "- No se reconoce el instrumento monetario de pago. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                        if (operacion.Liquidacion.DetalleDelInstrumentoMonetario == null)
                                            Errores += "- No se encontraron los detalles del instrumento monetario para la liquidación de la operación. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                        else
                                        {
                                            if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque == null && operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaDelMismoBanco == null & operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInterbancaria == null)
                                                Errores += "- No se encontraron los detalles del instrumento monetario para la liquidación de la operación. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                            else
                                            {
                                                if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque != null)
                                                {
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.InstitucionCredito))
                                                        Errores += "- Debe existir una institución de crédito al recibir un pago con cheque. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.NumeroDeCuentaLibrador))
                                                        Errores += "- Debe existir un número de cuenta recibir un pago con cheque. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.Cheque.NumeroCheque))
                                                        Errores += "- Debe existir un número de cheque al recibir un pago con cheque. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                }
                                                else if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInterbancaria != null)
                                                {
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInterbancaria.ClaveRastreo))
                                                        Errores += "- Debe existir una clave de rastreo al recibir un pago por transferencia interbancaria. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                }
                                                else if (operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaDelMismoBanco != null)
                                                {
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaDelMismoBanco.FolioInterno))
                                                        Errores += "- Debe existir un folio interno al recibir un pago por transferencia del mismo banco. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.InstitucionOrdenante))
                                                        Errores += "- Debe existir una institución ordenante al recibir un pago por transferencia internacional. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.NumeroDeCuenta))
                                                        Errores += "- Debe existir un número de cuenta al recibir un pago por transferencia internacional. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                    if (string.IsNullOrEmpty(operacion.Liquidacion.DetalleDelInstrumentoMonetario.TransferenciaInternacional.PaisDeOrigen))
                                                        Errores += "- Debe existir un pais de origen al recibir un pago por transferencia internacional. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                                }
                                            }
                                        }
                                        if (operacion.Liquidacion.MontoOperacion <= 0)
                                            Errores += "- Solo se permiten montos de operacion mayores a cero. Referencia: " + operacion.Liquidacion.ReferenciaInterna + Environment.NewLine;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                Errores += "- Ocurrio un error inesperado al momento de validar la estructura del informe.";
            }
            return informe;
        }

        private SujetoObligado getSujetoObligadoActual()
        {
            SujetoObligado sujeto = new SujetoObligado();
            sujeto.ClaveRFC = RFCContribuyente.ToUpper();
            return sujeto;
        }

        private List<Aviso> getAvisos()
        {
            List<FacturaTotalPorCliente> listaFacs = filtrarFacturas();
            if (listaFacs.Count < 0)
                return null;
            List<Aviso> listaAvisos = new List<Aviso>();
            foreach (FacturaTotalPorCliente factura in listaFacs)
            {
                Aviso aviso = new Aviso();
                aviso.Referencia = NumReferencia; //ver donde guardar el consecutivo
                aviso.Prioridad = 1;
                aviso.Alerta = new Alerta();
                aviso.Persona = getPersonaPorCliente(factura.IDCliente);
                aviso.Operaciones = getOperacionesPorCliente(factura.IDCliente);
                listaAvisos.Add(aviso);
                NumReferencia++; 
            }
            return listaAvisos;
        }

        private List<Aviso> getAvisos(bool venta)
        {
            List<FacturaTotalPorCliente> listaFacs = filtrarFacturasVenta();
            if (listaFacs.Count < 0)
                return null;
            List<Aviso> listaAvisos = new List<Aviso>();
            foreach (FacturaTotalPorCliente factura in listaFacs)
            {
                Aviso aviso = new Aviso();
                aviso.Referencia = NumReferencia; //ver donde guardar el consecutivo
                aviso.Prioridad = 1;
                aviso.Alerta = new Alerta();
                aviso.Persona = getPersonaPorCliente(factura.IDCliente);
                aviso.OperacionesVenta = getOperacionesPorCliente(factura.IDCliente, venta);
                listaAvisos.Add(aviso);
                NumReferencia++;
            }
            return listaAvisos;
        }

        private List<FacturaTotalPorCliente> filtrarFacturas()
        {
            try
            {
                DataTable dtFacturasTotales = Informes.getFacturasTotales(RFCContribuyente, periodoInicial, periodoFinal);
                decimal factorSalarioMinimo = Informes.getSalarioMinimo();
                List<FacturaTotalPorCliente> listaClientesSeguros = new List<FacturaTotalPorCliente>();
                List<FacturaTotalPorCliente> listaClientesAInvestigar = new List<FacturaTotalPorCliente>();
                foreach (DataRow rowFactura in dtFacturasTotales.Rows)
                {
                    if (Convert.ToDecimal(rowFactura["SUMA"].ToString()) >= (factorSalarioMinimo * 3210))
                    {
                        listaClientesSeguros.Add(new FacturaTotalPorCliente(rowFactura["CLIENTE"].ToString(), Convert.ToDecimal(rowFactura["SUMA"].ToString())));
                    }
                    else if (Convert.ToDecimal(rowFactura["SUMA"].ToString()) >= (factorSalarioMinimo * 1605))
                    {
                        listaClientesAInvestigar.Add(new FacturaTotalPorCliente(rowFactura["CLIENTE"].ToString(), Convert.ToDecimal(rowFactura["SUMA"].ToString())));
                    }
                    else
                    {
                        break;
                    }
                }
                if (listaClientesSeguros.Count <= 0 && listaClientesAInvestigar.Count <= 0)
                {
                    Errores += "No se encontraron registros que tengan que presentar avisos en el mes solicitado";
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
                        Errores += "No se encontraron registros que tengan que presentar avisos en el mes solicitado";
                    }
                    else
                    {
                        if (listaClientesAInvestigar.Count > 0)
                        {
                            foreach (FacturaTotalPorCliente factura in listaClientesAInvestigar)
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
                Errores = "Ha ocurrido un error inesperado al hacer la busqueda y filtro de facturas";
                return null;
            }
        }

        private List<FacturaTotalPorCliente> filtrarFacturasVenta()
        {
            try
            {
                decimal factorSalarioMinimo = Informes.getSalarioMinimo();
                decimal formulaFiltrado = (factorSalarioMinimo * 8025);
                DataTable dtVentasTotales = Informes.getVentasTotales(RFCContribuyente, periodoInicial, periodoFinal);
                List<FacturaTotalPorCliente> listaClientesSeguros = new List<FacturaTotalPorCliente>();
                List<FacturaTotalPorCliente> listaClientesAInvestigar = new List<FacturaTotalPorCliente>();
                foreach (DataRow rowVenta in dtVentasTotales.Rows)
                {
                    if (Convert.ToDecimal(rowVenta["SUMA"].ToString()) >= formulaFiltrado)
                    {
                        listaClientesSeguros.Add(new FacturaTotalPorCliente(rowVenta["CLIENTE"].ToString(), Convert.ToDecimal(rowVenta["SUMA"].ToString())));
                    }
                    else if (Convert.ToDecimal(rowVenta["SUMA"].ToString()) >= (formulaFiltrado / 6))
                    {
                        listaClientesAInvestigar.Add(new FacturaTotalPorCliente(rowVenta["CLIENTE"].ToString(), Convert.ToDecimal(rowVenta["SUMA"].ToString())));
                    }
                    else
                    {
                        break;
                    }
                } 
                if (listaClientesSeguros.Count <= 0 && listaClientesAInvestigar.Count <= 0)
                {
                    Errores += "No se encontraron registros que tengan que presentar avisos en el mes solicitado";
                }
                else
                {                  
                    if (listaClientesAInvestigar.Count > 0)
                    {
                        listaClientesAInvestigar = investigarVentas(listaClientesAInvestigar);
                    }
                    if (listaClientesSeguros.Count <= 0 && listaClientesAInvestigar.Count <= 0)
                    {
                        Errores += "No se encontraron registros que tengan que presentar avisos en el mes solicitado";
                    }
                    else
                    {
                        if (listaClientesAInvestigar.Count > 0)
                        {
                            foreach (FacturaTotalPorCliente factura in listaClientesAInvestigar)
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
                Errores = "Ha ocurrido un error inesperado al hacer la busqueda y filtro de facturas";
                return null;
            }
        }

        private List<FacturaTotalPorCliente> facturasMenosNC(List<FacturaTotalPorCliente> listaClientesFacs)
        {
            decimal factorSalarioMinimo = Informes.getSalarioMinimo();
            List<FacturaTotalPorCliente> facturasFinales = new List<FacturaTotalPorCliente>();
            foreach (FacturaTotalPorCliente factura in listaClientesFacs)
            {
                DataTable dtNc = Informes.getTotalNCPorCliente(factura, RFCContribuyente, periodoInicial, periodoFinal);
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

        private List<FacturaTotalPorCliente> investigarFacturasMenosNC(List<FacturaTotalPorCliente> listaClientesFacs)
        {
            decimal factorSalarioMinimo = Informes.getSalarioMinimo();
            List<FacturaTotalPorCliente> facturasFinales = new List<FacturaTotalPorCliente>();
            foreach (FacturaTotalPorCliente factura in listaClientesFacs)
            {
                DataTable dtFacPorCliente = Informes.getFacturasTotalesPorCliente(factura.IDCliente, RFCContribuyente, periodoInicialMenos6M, periodoFinal);
                DataTable dtNCPorCliente = Informes.getTotalNCPorCliente(factura, RFCContribuyente, periodoInicialMenos6M, periodoFinal);
                if (dtFacPorCliente.Rows.Count > 0)
                {
                    decimal montoFacturas6M = 0, montoNotas6M = 0, total = 0;
                    montoFacturas6M = Convert.ToDecimal(dtFacPorCliente.Rows[0]["SUMA"].ToString());
                    if(dtNCPorCliente.Rows.Count > 0)
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

        private List<FacturaTotalPorCliente> investigarVentas(List<FacturaTotalPorCliente> listaClientesFacs)
        {
            decimal factorSalarioMinimo = Informes.getSalarioMinimo();
            decimal formulaFiltrado = (factorSalarioMinimo * 8025);
            List<FacturaTotalPorCliente> facturasFinales = new List<FacturaTotalPorCliente>();
            foreach (FacturaTotalPorCliente factura in listaClientesFacs)
            {
                DataTable dtVtaPorCliente = Informes.getVentasTotalesPorCliente(factura.IDCliente, RFCContribuyente, periodoInicialMenos6M, periodoFinal);
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

        private PersonaMoral getPersonaMoral(string idCliente)
        {
            DataTable dtPersonaMoral = Informes.getPersonaMoralPorIdCliente(idCliente);
            PersonaMoral persona = new PersonaMoral();
            if (dtPersonaMoral.Rows.Count > 0)
            {
                try
                {
                    persona.RazonSocial = dtPersonaMoral.Rows[0]["P0203_NOMBRE"].ToString().Trim().ToUpper();
                    persona.RFC = dtPersonaMoral.Rows[0]["P0204_RFC"].ToString().Trim().ToUpper();
                    persona.FechaDeConstitucion = getFechaConstitucionByRFC(persona.RFC);
                    persona.Giro = dtPersonaMoral.Rows[0]["P0257_ID_ACTIVIDAD_ECONOMICA"].ToString().Trim();
                    PersonaFisica apoderado = new PersonaFisica();
                    apoderado.Nombre = dtPersonaMoral.Rows[0]["P1503_NOMBRE"].ToString().Trim().ToUpper();
                    apoderado.ApellidoPaterno = dtPersonaMoral.Rows[0]["P1509_APELLIDOP_PERSONA"].ToString().Trim().ToUpper();
                    apoderado.ApellidoMaterno = dtPersonaMoral.Rows[0]["P1510_APELLIDOM_PERSONA"].ToString().Trim().ToUpper();
                    apoderado.RFC = dtPersonaMoral.Rows[0]["CAMPO1"].ToString().Trim().ToUpper();
                    if (!string.IsNullOrEmpty(apoderado.RFC))
                        apoderado.FechaNacimiento = getFechaNacimientoByRFC(apoderado.RFC);
                    apoderado.CURP = dtPersonaMoral.Rows[0]["CAMPO2"].ToString().Trim().ToUpper();
                    if (!string.IsNullOrEmpty(dtPersonaMoral.Rows[0]["P1515_ID_TIPO_IDENTIFICACION"].ToString().Trim()))
                    {
                        apoderado.TipoIdentificacion = Convert.ToInt32(dtPersonaMoral.Rows[0]["P1515_ID_TIPO_IDENTIFICACION"].ToString().Trim());
                        if (apoderado.TipoIdentificacion > 10 && apoderado.TipoIdentificacion < 14)
                            apoderado.OtraIdentificacion = dtPersonaMoral.Rows[0]["P1511_DOC_ID_OFICIAL"].ToString().Trim().ToUpper();
                    }
                    apoderado.Autoridad = dtPersonaMoral.Rows[0]["P1514_DESCR_AUTORIDAD_EMITE_ID"].ToString().Trim().ToUpper();
                    apoderado.NumeroIdentificacion = dtPersonaMoral.Rows[0]["P1512_ID_DOC_OFICIAL"].ToString().Trim().ToUpper();
                    persona.Apoderado = apoderado;
                }
                catch
                {
                    persona = new PersonaMoral();
                }
            }
            return persona;
        }

        private PersonaFisica getPersonaFisica(string idCliente)
        {
            DataTable dtPersonaFisica = Informes.getPersonaFisicaPorIdCliente(idCliente);            
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
                    persona.FechaNacimiento = getFechaNacimientoByRFC(persona.RFC);
                    persona.CURP = dtPersonaFisica.Rows[0]["CAMPO3"].ToString().Trim().ToUpper();
                    persona.ActividadEconomica = dtPersonaFisica.Rows[0]["P0257_ID_ACTIVIDAD_ECONOMICA"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtPersonaFisica.Rows[0]["P0261_ID_TIPO_IDENTIFICACION"].ToString().Trim()))
                    {
                        persona.TipoIdentificacion = Convert.ToInt32(dtPersonaFisica.Rows[0]["P0261_ID_TIPO_IDENTIFICACION"].ToString().Trim());
                        if (persona.TipoIdentificacion > 10 && persona.TipoIdentificacion < 14)
                            persona.OtraIdentificacion = dtPersonaFisica.Rows[0]["P0254_DOC_ID_OFICIAL"].ToString().Trim().ToUpper();
                    }
                    persona.Autoridad = dtPersonaFisica.Rows[0]["P0260_DESCR_AUTORIDAD_EMITE_ID"].ToString().Trim().ToUpper();
                    persona.NumeroIdentificacion = dtPersonaFisica.Rows[0]["P0255_ID_DOC_OFICIAL"].ToString().Trim().ToUpper();
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
            DateTime fechaNacimiento = new DateTime();
            try
            {
                string fechaRfc = rfc.Substring(4, 6);
                int year = Convert.ToInt32(fechaRfc.Substring(0, 2));
                if (year > 14)
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

        private DateTime getFechaConstitucionByRFC(string rfc)
        {
            DateTime fechaConstitucion = new DateTime();
            try
            {
                string fechaRfc = rfc.Substring(3, 6);
                int year = Convert.ToInt32(fechaRfc.Substring(0, 2));
                if (year > 14)
                    year += 1900;
                else
                    year += 2000;
                int month = Convert.ToInt32(fechaRfc.Substring(2, 2));
                int day = Convert.ToInt32(fechaRfc.Substring(4, 2));
                fechaConstitucion = new DateTime(year, month, day);
                return fechaConstitucion;
            }
            catch
            {
                return new DateTime();
            }
        }

        private Domicilio getDomicilioPorCliente(string idCliente)
        {
            DataTable dtDomicilio = Informes.getDomicilioPorIdCliente(idCliente);
            Domicilio domicilio = new Domicilio();
            if (dtDomicilio.Rows.Count > 0)
            {
                try
                {
                    domicilio.ReferenciaInterna = dtDomicilio.Rows[0]["P0500_ID_ENTE"].ToString();
                    domicilio.Colonia = dtDomicilio.Rows[0]["P0504_COLONIA"].ToString().ToUpper();
                    domicilio.Calle = dtDomicilio.Rows[0]["P0503_CALLE_NUM"].ToString().ToUpper().Replace("#", "").Replace("-", " ").Trim();
                    domicilio.NumeroExterior =  dtDomicilio.Rows[0]["CAMPO1"].ToString().ToUpper();
                    domicilio.NumeroInterior =  dtDomicilio.Rows[0]["CAMPO2"].ToString().ToUpper();
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
            DataTable dtTelMail = Informes.getTelefonoPorIdCliente(idCliente);
            Telefono telefono = new Telefono();
            if (dtTelMail.Rows.Count > 0)
            {
                try
                {
                    telefono.ReferenciaInterna = dtTelMail.Rows[0]["P0203_NOMBRE"].ToString().Trim();
                    foreach (DataRow row in dtTelMail.Rows)
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
            DataTable dtFacturasPorCliente = Informes.getFacturasPorCliente(idCliente, RFCContribuyente, periodoInicial, periodoFinal);
            List<Operacion> operaciones = new List<Operacion>();
            if (dtFacturasPorCliente.Rows.Count > 0)
            {
                try
                {
                    foreach (DataRow row in dtFacturasPorCliente.Rows)
                    {
                        Operacion operacion = new Operacion();
                        operacion.ReferenciaInterna = "CFD " + row["P4006_SERIE"].ToString() + " " + row["P4007_FOLIO"].ToString();
                        operacion.Fecha = (DateTime)row["P2408_FECHA_PAGADO"];
                        DateTime fechaEmision = (DateTime)row["P2409_FECHA_EMISION"];
                        operacion.FechaInicioRenta = new DateTime(fechaEmision.Year, fechaEmision.Month, 1);
                        operacion.FechaFinRenta = operacion.FechaInicioRenta.AddMonths(1).AddDays(-1);
                        CaracteristicasInmueble caracteristicas = new CaracteristicasInmueble();
                        caracteristicas.ReferenciaInterna = row["P0703_NOMBRE"].ToString().Trim();
                        caracteristicas.FolioReal = row["P0740_CAMPO15"].ToString().Trim().ToUpper();
                        if (!string.IsNullOrEmpty(row["P0770_ID_CLASIF_FISCAL"].ToString().Trim()))
                            caracteristicas.TipoDeInmueble = Convert.ToInt32(row["P0770_ID_CLASIF_FISCAL"].ToString());
                        if (!string.IsNullOrEmpty(row["P0730_SUBCONJUNTO"].ToString())) // 2.3.4.11
                        {
                            caracteristicas.SubConjunto = row["P0730_SUBCONJUNTO"].ToString();
                            bool contratoPorSubConj = Informes.existeContratoPorSubconjunto(caracteristicas.SubConjunto);
                            if (!contratoPorSubConj)
                            {                                
                                if (!string.IsNullOrEmpty(row["P1922_A_MIN_ING"].ToString().Trim()))
                                    caracteristicas.ValorCatastral = Convert.ToDecimal(row["P1922_A_MIN_ING"].ToString());
                                //caracteristicas.FolioReal = row["P0740_CAMPO15"].ToString().Trim().ToUpper();
                            }
                            else
                            {
                                caracteristicas.ValorCatastral = obtenerValorCatastralDeSubConjunto(caracteristicas.SubConjunto);
                                //caracteristicas.FolioReal = obtenerFoliosRPPPorSubconjunto(caracteristicas.SubConjunto);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(row["P1922_A_MIN_ING"].ToString().Trim()))
                                caracteristicas.ValorCatastral = Convert.ToDecimal(row["P1922_A_MIN_ING"].ToString());
                            //caracteristicas.FolioReal = row["P0740_CAMPO15"].ToString().Trim().ToUpper();
                        }                        
                        caracteristicas.Blindaje = row["P0772_INMUEBLE_BLINDADO"].ToString().Trim() == "S" ? 1 : 2;    
                        Domicilio domicilio = new Domicilio();
                        domicilio.ReferenciaInterna = row["P0703_NOMBRE"].ToString().Trim();
                        domicilio.Colonia = row["P0504_COLONIA"].ToString().Trim().ToUpper();
                        domicilio.Calle = row["P0503_CALLE_NUM"].ToString().Trim().ToUpper().Replace("#", "").Replace("-", " ").Trim();
                        domicilio.NumeroExterior = row["CAMPO1"].ToString().Trim().ToUpper();
                        domicilio.NumeroInterior = row["CAMPO2"].ToString().Trim().ToUpper().Replace("(", "").Replace(")", "");
                        domicilio.CodigoPostal = row["P0505_COD_POST"].ToString().Trim();
                        caracteristicas.Domicilio = domicilio;
                        operacion.CaracteristicasDelInmueble = caracteristicas;
                        Liquidacion liquidacion = new Liquidacion();
                        liquidacion.ReferenciaInterna = "CFD " + row["P4006_SERIE"].ToString() + " " + row["P4007_FOLIO"].ToString();
                        liquidacion.FechaDePago = (DateTime)row["P2408_FECHA_PAGADO"];
                        if (!string.IsNullOrEmpty(row["P2457_ID_INSTRUMENTO_PAGO"].ToString().Trim()))
                            liquidacion.ClaveInstrumentoMonetario = Convert.ToInt32(row["P2457_ID_INSTRUMENTO_PAGO"].ToString());
                        DetalleInstrumento detallInstrumento = new DetalleInstrumento();
                        if (liquidacion.ClaveInstrumentoMonetario == 5)
                        {
                            Cheque cheque = new Cheque();
                            cheque.InstitucionCredito = row["P2453_NOMBRE_BANCO_GIRADOR"].ToString().Trim().ToUpper();
                            cheque.NumeroDeCuentaLibrador = row["P2454_CUENTA_BCO_PAGO"].ToString().Trim();
                            cheque.NumeroCheque = row["P2455_NUM_CHEQUE_PAGO"].ToString().Trim();
                            detallInstrumento.Cheque = cheque;
                        }
                        else if (liquidacion.ClaveInstrumentoMonetario == 8)
                        {
                            TransferenciaInterbancaria interbancaria = new TransferenciaInterbancaria();
                            interbancaria.ClaveRastreo = row["P2455_NUM_CHEQUE_PAGO"].ToString().Trim();
                            detallInstrumento.TransferenciaInterbancaria = interbancaria;
                        }
                        else if (liquidacion.ClaveInstrumentoMonetario == 9)
                        {
                            TransferenciaMismoBanco mismoBanco = new TransferenciaMismoBanco();
                            mismoBanco.FolioInterno = row["P2455_NUM_CHEQUE_PAGO"].ToString().Trim();
                            detallInstrumento.TransferenciaDelMismoBanco = mismoBanco;
                        }
                        else if (liquidacion.ClaveInstrumentoMonetario == 10)
                        {
                            TransferenciaInternacional internacional = new TransferenciaInternacional();
                            internacional.InstitucionOrdenante = row["P2453_NOMBRE_BANCO_GIRADOR"].ToString().Trim().ToUpper();
                            internacional.NumeroDeCuenta = row["P2455_NUM_CHEQUE_PAGO"].ToString().Trim();
                            internacional.PaisDeOrigen = row["P2459_PAIS_ORIGEN_TRANSF"].ToString().Trim().ToUpper();
                            detallInstrumento.TransferenciaInternacional = internacional;
                        }
                        liquidacion.DetalleDelInstrumentoMonetario = detallInstrumento;
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
                        if (!string.IsNullOrEmpty(row["P2405_IMPORTE"].ToString().Trim()))
                            liquidacion.MontoOperacion = Convert.ToDecimal(row["P2405_IMPORTE"].ToString());
                        operacion.Liquidacion = liquidacion;
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

        private List<OperacionVenta> getOperacionesPorCliente(string idCliente, bool venta)
        {
            DataTable dtFacturasPorCliente = Informes.getVentasPorCliente(idCliente, RFCContribuyente, periodoInicial, periodoFinal);
            List<OperacionVenta> operaciones = new List<OperacionVenta>();
            if (dtFacturasPorCliente.Rows.Count > 0)
            {
                try
                {
                    foreach (DataRow row in dtFacturasPorCliente.Rows)
                    {
                        OperacionVenta operacion = new OperacionVenta();
                        operacion.ReferenciaInterna = " Num. Recibo " + row["P2403_NUM_RECIBO"].ToString();
                        operacion.Fecha = (DateTime)row["P2408_FECHA_PAGADO"];
                        operacion.Tipo = 501;                        
                        if (!string.IsNullOrEmpty(row["P0770_ID_CLASIF_FISCAL"].ToString().Trim()))
                            operacion.TipoDeInmueble = Convert.ToInt32(row["P0770_ID_CLASIF_FISCAL"].ToString());
                        if (!string.IsNullOrEmpty(row["MONTOPACTADO"].ToString().Trim()))
                            operacion.MontoPactado = Convert.ToDecimal(row["MONTOPACTADO"].ToString().Trim());
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
                        if(!string.IsNullOrEmpty(row["TERRENO"].ToString().Trim()))
                            caracteristicas.DimensionTerreno = Convert.ToDecimal(row["TERRENO"].ToString().Trim());
                        if (!string.IsNullOrEmpty(row["CONSTRUCCION"].ToString().Trim()))
                            caracteristicas.DimensionConstruido = Convert.ToDecimal(row["CONSTRUCCION"].ToString().Trim());
                        caracteristicas.Blindaje = row["P0772_INMUEBLE_BLINDADO"].ToString().Trim() == "S" ? 1 : 2;
                        caracteristicas.FolioReal = row["P0740_CAMPO15"].ToString().Trim().ToUpper();
                        operacion.CaracteristicasDelInmueble = caracteristicas;
                        operacion.FechaContrato = (DateTime)row["CAMPO_DATE1"];
                        Liquidacion liquidacion = new Liquidacion();
                        liquidacion.ReferenciaInterna = " Num. Recibo " + row["P2403_NUM_RECIBO"].ToString();
                        liquidacion.FechaDePago = (DateTime)row["P2408_FECHA_PAGADO"];
                        if (!string.IsNullOrEmpty(row["P0406_FORMA_FACT"].ToString().Trim()))
                            liquidacion.FormaDePago = Convert.ToInt32(row["P0406_FORMA_FACT"].ToString());
                        if (!string.IsNullOrEmpty(row["P2457_ID_INSTRUMENTO_PAGO"].ToString().Trim()))
                            liquidacion.ClaveInstrumentoMonetario = Convert.ToInt32(row["P2457_ID_INSTRUMENTO_PAGO"].ToString());
                        DetalleInstrumento detallInstrumento = new DetalleInstrumento();
                        if (liquidacion.ClaveInstrumentoMonetario == 5)
                        {
                            Cheque cheque = new Cheque();
                            cheque.InstitucionCredito = row["P2453_NOMBRE_BANCO_GIRADOR"].ToString().Trim().ToUpper();
                            cheque.NumeroDeCuentaLibrador = row["P2454_CUENTA_BCO_PAGO"].ToString().Trim();
                            cheque.NumeroCheque = row["P2455_NUM_CHEQUE_PAGO"].ToString().Trim();
                            detallInstrumento.Cheque = cheque;
                        }
                        else if (liquidacion.ClaveInstrumentoMonetario == 8)
                        {
                            TransferenciaInterbancaria interbancaria = new TransferenciaInterbancaria();
                            interbancaria.ClaveRastreo = row["P2455_NUM_CHEQUE_PAGO"].ToString().Trim();
                            detallInstrumento.TransferenciaInterbancaria = interbancaria;
                        }
                        else if (liquidacion.ClaveInstrumentoMonetario == 9)
                        {
                            TransferenciaMismoBanco mismoBanco = new TransferenciaMismoBanco();
                            mismoBanco.FolioInterno = row["P2455_NUM_CHEQUE_PAGO"].ToString().Trim();
                            detallInstrumento.TransferenciaDelMismoBanco = mismoBanco;
                        }
                        else if (liquidacion.ClaveInstrumentoMonetario == 10)
                        {
                            TransferenciaInternacional internacional = new TransferenciaInternacional();
                            internacional.InstitucionOrdenante = row["P2453_NOMBRE_BANCO_GIRADOR"].ToString().Trim().ToUpper();
                            internacional.NumeroDeCuenta = row["P2455_NUM_CHEQUE_PAGO"].ToString().Trim();
                            internacional.PaisDeOrigen = row["P2459_PAIS_ORIGEN_TRANSF"].ToString().Trim().ToUpper();
                            detallInstrumento.TransferenciaInternacional = internacional;
                        }
                        liquidacion.DetalleDelInstrumentoMonetario = detallInstrumento;
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
                        operacion.Liquidacion = liquidacion;
                        operaciones.Add(operacion);
                    }
                }
                catch
                {
                    operaciones = new List<OperacionVenta>();
                }
            }
            return operaciones;
        }

        private decimal obtenerValorCatastralDeSubConjunto(string idSubconjunto)
        {//2.3.4.11
            try
            {
                decimal result = 0;
                DataTable dtValorCatastral = Informes.getDTValorCatastralDeSubconjunto(idSubconjunto);
                if (dtValorCatastral.Rows.Count > 0)
                {
                    foreach (DataRow row in dtValorCatastral.Rows)
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

        private string obtenerFoliosRPPPorSubconjunto(string idSubconjunto)
        {
            try
            {
                string folios = string.Empty;
                DataTable dtFolios = Informes.getDTFoliosRPPDeSubconjunto(idSubconjunto);
                if (dtFolios.Rows.Count > 0)
                {
                    foreach (DataRow row in dtFolios.Rows)
                    {
                        if (!string.IsNullOrEmpty(row["P0740_CAMPO15"].ToString()))
                            folios += row["P0740_CAMPO15"].ToString() + " - ";
                    }
                }
                if(folios.Trim().EndsWith(","))
                    folios = folios.Trim().Substring(0, folios.Length - 2).Trim();
                return folios;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
