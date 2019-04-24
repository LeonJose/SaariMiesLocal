using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesPolizas;
using GestorReportes.BusinessLayer.DataAccessLayer;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class ConfiguracionesPolizas
    {
        bool esEgreso = false;
        public ConfiguracionesPolizas()
        {
            esEgreso = false;
        }

        public ConfiguracionesPolizas(bool esEgreso)
        {
            this.esEgreso = esEgreso;
        }

        public bool existeConfig(ConfiguracionPolizaEntity configuracion)
        {
            if(!esEgreso)
                return Polizas.existeConfiguracion(configuracion);
            else
                return Polizas.existeConfiguracion(configuracion, esEgreso);
        }

        public string guardar(ConfiguracionPolizaEntity configuracion)
        {
            try
            {
                if (!esEgreso)
                {
                    string errores = string.Empty;
                    foreach (FormulaPolizaEntity formPol in configuracion.Formulas)
                    {
                        string error = validaFormula(formPol.Formula);
                        if (!string.IsNullOrEmpty(error))
                            errores += error + "Referencia: " + formPol.Tipo + " " + formPol.Moneda + Environment.NewLine;
                        /*if (string.IsNullOrEmpty(errores))
                        {
                            //formPol.Tipo = reemplazarTipo(formPol.Tipo);
                        }*/
                    }
                    if (string.IsNullOrEmpty(errores))
                    {
                        errores = Polizas.agregarConfiguracion(configuracion);
                    }
                    return errores;
                }
                else
                {
                    string errores = string.Empty;
                    foreach (FormulaPolizaEntity formPol in configuracion.Formulas)
                    {
                        string error = validaFormula(formPol.Formula);
                        if (!string.IsNullOrEmpty(error))
                            errores += error + "Referencia: " + formPol.Tipo + " " + Environment.NewLine;
                    }
                    if (string.IsNullOrEmpty(errores))
                    {
                        errores = Polizas.agregarConfiguracion(configuracion, esEgreso);
                    }
                    return errores;
                }
            }
            catch(Exception ex)
            {
                return "Error general al guardar. " + ex.Message;
            }
        }

        private string validaFormula(string formula)
        {
            string error = string.Empty;
            try
            {
                if (formula.StartsWith("+") || formula.StartsWith("-") || formula.StartsWith("x") || formula.StartsWith("/"))
                    error += "La fórmula no debe empezar con una operación" + Environment.NewLine;
                if (formula.Contains("(") && !formula.Contains(")"))
                    error += "Se esperaba )" + Environment.NewLine;
                if (formula.Trim().EndsWith("+") || formula.Trim().EndsWith("-") || formula.Trim().EndsWith("x") || formula.Trim().EndsWith("/") || formula.Trim().EndsWith("("))
                    error += "Final de fórmula incorrecto" + Environment.NewLine;
                if (formula.Contains(")") && !formula.Contains("("))
                    error += "Se esperaba (" + Environment.NewLine;
                if (formula.Contains("(") && formula.Contains(")"))
                {
                    if (formula.Split('(').Length != formula.Split(')').Length)
                        error += "No se encontraron los mismos números de incidencias para '(' y ')'" + Environment.NewLine;
                }
                if (string.IsNullOrEmpty(error))
                {
                    string[] cadenas = formula.Split(' ');
                    string anterior = string.Empty;
                    for (int i = 0; i < cadenas.Length; i++)
                    {
                        if (i > 0)
                        {
                            anterior = cadenas[i - 1];
                        }
                        if (!string.IsNullOrEmpty(anterior))
                        {
                            if (anterior.Contains("+") || anterior.Contains("-") || anterior.Contains("x") || anterior.Contains("/"))
                            {
                                if (cadenas[i].Contains("+") || cadenas[i].Contains("-") || cadenas[i].Contains("x") || cadenas[i].Contains("/"))
                                    error += "No pueden existir dos o mas simbolos de operación consecutivos" + Environment.NewLine;
                                if (cadenas[i] == ")")
                                    error += "El caracter ')' no debe preceder de un simoblo de operación" + Environment.NewLine;
                            }
                            else
                            {
                                if (!anterior.Contains("(") && !anterior.Contains(")") && !string.IsNullOrEmpty(anterior))
                                {
                                    if (!cadenas[i].Contains("(") && !cadenas[i].Contains(")") && !cadenas[i].Contains("+") && !cadenas[i].Contains("-") && !cadenas[i].Contains("x") && !cadenas[i].Contains("/") && !string.IsNullOrEmpty(cadenas[i]))
                                        error += "No pueden existir dos o mas variables consecutivas" + Environment.NewLine;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error += ex.Message;
            }
            return error;
        }
        /*
        private string reemplazarTipo(string tipo)
        {
            Hashtable tablaTipos = getTablTipos();
            string tipoCorto = tipo.Length <= 13 ? tipo : tipo.Substring(0, 13);
            string tipoNuevo = string.Empty;
            tipoNuevo = tablaTipos[tipoCorto].ToString();            
            return tipoNuevo;
        }*/

       /* private Hashtable getTablTipos()
        {
            Hashtable tablaTipos = new Hashtable();
            tablaTipos.Add("Cliente", "CC");
            tablaTipos.Add("Comp. Cliente", "CD");
            tablaTipos.Add("Comp. Banco D", "BCC");
            tablaTipos.Add("Banco", "BMN");
            tablaTipos.Add("Banco Dolares", "BDL");
            tablaTipos.Add("IVA por trasl", "IV");
            tablaTipos.Add("IVA Trasladad", "IVT");
            tablaTipos.Add("Ingreso", "IA");
            tablaTipos.Add("Descuentos", "DA");
            tablaTipos.Add("Otros Ingreso", "IO");
            tablaTipos.Add("Cuenta concen", "CCDD");
            tablaTipos.Add("Retención IVA", "RIVA");
            tablaTipos.Add("Retención ISR", "RISR");
            tablaTipos.Add("Depositos en ", "DG");
            return tablaTipos;
        }*/

        public string reemplazar(ConfiguracionPolizaEntity configuracion)
        {            
            try
            {
                if (!esEgreso)
                {
                    string error = string.Empty;
                    if (Polizas.borrarConfiguraciones(configuracion))
                    {
                        error = "No se eliminaron correctamente los registros de configuración";
                    }
                    else
                    {
                        error = guardar(configuracion);
                    }
                    return error;
                }
                else
                {
                    string error = string.Empty;
                    if (Polizas.borrarConfiguraciones(configuracion, esEgreso))
                    {
                        error = "No se eliminaron correctamente los registros de configuración";
                    }
                    else
                    {
                        error = guardar(configuracion);
                    }
                    return error;
                }
            }
            catch (Exception ex)
            {
                return "Error general al reemplazar. " + ex.Message;
            }            
        }

        public bool borrar(ConfiguracionPolizaEntity configuracion)
        {
            if(!esEgreso)
                return Polizas.borrarConfiguraciones(configuracion);
            else
                return Polizas.borrarConfiguraciones(configuracion, esEgreso);
        }
    }
}
