using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GestorReportes.BusinessLayer.EntitiesPolizas;
using GestorReportes.BusinessLayer.DataAccessLayer;
using Excel = Microsoft.Office.Interop.Excel;
using System.Linq;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.Helpers;
using GestorReportes.BusinessLayer.Entities;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    class Poliza : IReport, IBackgroundReport
    {
        //Leer version desde la base de datos
        public int versionOBDC = SaariDB.getVersionDB();
        public ModuloCobranzaEntity cobranzaEntity = SaariDB.getVersionCobranza();
        //Obtiene la fecha de actualización de la base de datos Saari y la toma como referencia
        //para la generación de polizas con Pago Total con saldo a favor e iva total cobrado tipo Arco
        public DateTime fechaActPol = SaariDB.getFechaAct();
        private bool cancelacionPendiente = false, esEgreso = false;
        public bool esVentas = false;
        public bool esRentas = false;
        private string fileName = string.Empty;
        private CondicionesEntity condiciones = null;
        private List<ColumnasExcelEntity> columnas = new List<ColumnasExcelEntity>();

        public string NombreArchivo { get { return fileName; } }
        public bool CancelacionPendiente { get { return cancelacionPendiente; } }
        public int ContadorMovimientos = 0;
        public event EventHandler<CambioProgresoEventArgs> CambioProgreso;

        public Poliza(CondicionesEntity condiciones, List<ColumnasExcelEntity> columnas)
        {
            this.condiciones = condiciones;
            this.columnas = columnas;
            this.esEgreso = false;
        }

        public Poliza(CondicionesEntity condiciones)
        {
            this.condiciones = condiciones;
            this.esEgreso = true;
        }

        public string generar()
        {
            if (!esEgreso)
            {
                #region POLIZAS DIARIO, INGRESOS, NC, CANCELACIONES
                List<string> cuentasIva = new List<string>();
                List<string> cuentasIngresos = new List<string>();
                OnCambioProgreso(10);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                List<RecibosEntitys> recibos = null;
                if (esRentas)
                {
                    recibos = Polizas.getRecibos(condiciones);
                }
                else if (esVentas)
                {
                    recibos = Polizas.getRecibosVentas(condiciones);
                }
                OnCambioProgreso(20);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                if (recibos.Count > 0)
                {
                    if (condiciones.Sucursal != null)
                    {
                        if (condiciones.Sucursal.NombreSucursal != "*Todas")
                            recibos = recibos.Where(r => r.Sucursal == condiciones.Sucursal.NombreSucursal).ToList();
                    }
                    OnCambioProgreso(30);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    List<FormulaPolizaEntity> listaFormulas = Polizas.getFormulas(condiciones.Inmobiliaria, condiciones.TipoPoliza);
                    OnCambioProgreso(40);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";
                    if (listaFormulas.Count > 0)
                    {
                        if (condiciones.Formato == FormatoExportacionPoliza.ContpaqTxt)
                        {
                            #region Contpaq .txt
                            try
                            {
                                string[] mascara = null;
                                if (!string.IsNullOrEmpty(condiciones.Mascara))
                                    mascara = condiciones.Mascara.Trim().Split('-');
                                PolizaEntity poliza = new PolizaEntity();
                                EncabezadoPolizaEntity encabezado = new EncabezadoPolizaEntity();
                                encabezado.Fecha = condiciones.FechaInicio;
                                if (condiciones.TipoPoliza == 1)
                                    encabezado.TipoContpaq = 3;
                                else if (condiciones.TipoPoliza == 2)
                                    encabezado.TipoContpaq = 1;
                                else
                                    encabezado.TipoContpaq = 3;
                                //encabezado.TipoContpaq = condiciones.TipoPoliza == 1 ? 3 : 1;
                                poliza.Encabezado = encabezado;
                                List<MovimientoPolizaEntity> movimientos = new List<MovimientoPolizaEntity>();
                                bool esPrimero = false;
                                OnCambioProgreso(50);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                int porcentaje = 50;
                                decimal factor = 40 / recibos.Count;
                                factor = factor >= 1 ? factor : 1;

                                foreach (RecibosEntitys recibo in recibos)
                                {
                                    if (porcentaje <= 75)
                                        porcentaje += Convert.ToInt32(factor);
                                    OnCambioProgreso(porcentaje);
                                    if (CancelacionPendiente)
                                        return "Proceso cancelado por el usuario";

                                    esPrimero = true;
                                    foreach (FormulaPolizaEntity formula in listaFormulas)
                                    {
                                        if (recibo.Moneda == formula.Moneda)
                                        {
                                            MovimientoPolizaEntity movimiento = getMovimientos(recibo, formula, condiciones.UsarConceptoPrincipal);
                                            if (movimiento == null)
                                                return "Error al obtener el movimiento del recibo " + recibo.Identificador;
                                            if (condiciones.MultiplesEncabezados)
                                            {
                                                movimiento.EsPrimero = esPrimero;
                                                esPrimero = false;
                                            }
                                            if (!string.IsNullOrEmpty(movimiento.Cuenta) && movimiento.Cuenta != "NO CUENTA")
                                            {
                                                if (mascara != null)
                                                {
                                                    string cuentaEnmascarada = string.Empty;
                                                    string[] cuenta = movimiento.Cuenta.Trim().Split('-');
                                                    if (cuenta.Length == mascara.Length)
                                                    {
                                                        bool error = false;
                                                        for (int i = 0; i < cuenta.Length; i++)
                                                        {
                                                            if (!string.IsNullOrEmpty(cuenta[i]) && !string.IsNullOrEmpty(mascara[i]))
                                                            {
                                                                int masc = Convert.ToInt32(mascara[i]);
                                                                string m = string.Empty;
                                                                for (int j = 0; j < masc; j++)
                                                                    m += "9";
                                                                int max = Convert.ToInt32(m);
                                                                if (Convert.ToInt64(cuenta[i]) <= max)
                                                                {
                                                                    cuentaEnmascarada += cuenta[i].Substring(cuenta[i].Length - masc) + "-";
                                                                }
                                                                else
                                                                {
                                                                    error = true;
                                                                    break;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                error = true;
                                                                break;
                                                            }
                                                        }
                                                        if (!error && !string.IsNullOrEmpty(cuentaEnmascarada))
                                                            movimiento.Cuenta = cuentaEnmascarada.Substring(0, cuentaEnmascarada.Length - 1);
                                                    }
                                                    else if (cuenta.Length < mascara.Length)
                                                    {
                                                        bool error = false;
                                                        int limite = cuenta.Length;
                                                        for (int i = 0; i < mascara.Length; i++)
                                                        {
                                                            if (!string.IsNullOrEmpty(mascara[i]))
                                                            {
                                                                int masc = Convert.ToInt32(mascara[i]);
                                                                string m = string.Empty;
                                                                for (int j = 0; j < masc; j++)
                                                                    m += "9";
                                                                int max = Convert.ToInt32(m);
                                                                if (i < limite)
                                                                {
                                                                    if (Convert.ToInt64(cuenta[i]) <= max)
                                                                        cuentaEnmascarada += cuenta[i].Substring(cuenta[i].Length - masc) + "-";
                                                                    else
                                                                    {
                                                                        error = true;
                                                                        break;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    for (int j = 0; j < m.Length; j++)
                                                                        cuentaEnmascarada += "0";
                                                                    cuentaEnmascarada += "-";
                                                                }
                                                            }
                                                        }
                                                        if (!error && !string.IsNullOrEmpty(cuentaEnmascarada))
                                                            movimiento.Cuenta = cuentaEnmascarada.Substring(0, cuentaEnmascarada.Length - 1);
                                                    }
                                                }
                                            }
                                            if (condiciones.TipoPresentacion == 1)
                                            {
                                                if (formula.Tipo == "IV")
                                                {
                                                    if (!cuentasIva.Contains(movimiento.Cuenta))
                                                        cuentasIva.Add(movimiento.Cuenta);
                                                }
                                                else if (formula.Tipo == "IA")
                                                {
                                                    if (!cuentasIngresos.Contains(movimiento.Cuenta))
                                                        cuentasIngresos.Add(movimiento.Cuenta);
                                                }
                                            }
                                            movimientos.Add(movimiento);
                                        }
                                    }
                                }
                                if (condiciones.TipoPresentacion == 1)
                                {
                                    if (cuentasIva.Count != 1)
                                        return "No se puede generar la póliza consolidada debido a que se encontraron cuentas diferentes para IVA";
                                    if (cuentasIngresos.Count != 1)
                                        return "No se puede generar la póliza consolidada debido a que se encontraron cuentas diferentes para Ingreso";
                                    poliza.Movimientos = consolidar(movimientos, cuentasIva[0], cuentasIngresos[0]);
                                }
                                else
                                {
                                    poliza.Movimientos = movimientos;
                                }
                                OnCambioProgreso(100);
                                return generarTxt(poliza, recibos[0].RazonSocial, condiciones, false);
                            }
                            catch //(Exception ex)
                            {
                                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                                return "Hubo un error inesperado al momento de cargar los datos de la póliza Contpaq.";
                            }
                            #endregion
                        }
                        else if (condiciones.Formato == FormatoExportacionPoliza.Excel)
                        {
                            #region Excel
                            if (columnas.Count <= 0)
                                return "No se cargó correctamente el formato de columnas de Excel.";
                            try
                            {
                                PolizaExcelEntity poliza = new PolizaExcelEntity();
                                poliza.Encabezados = columnas;
                                if (condiciones.TipoPoliza == 1)
                                    poliza.TipoPoliza = 3;
                                else if (condiciones.TipoPoliza == 2)
                                    poliza.TipoPoliza = 1;
                                else
                                    poliza.TipoPoliza = 3;

                                OnCambioProgreso(50);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                int porcentaje = 50;
                                decimal factor = 40 / recibos.Count;
                                factor = factor >= 1 ? factor : 1;

                                //poliza.TipoPoliza = condiciones.TipoPoliza == 1 ? 3 : 1;
                                List<MovimientoExcelEntity> movimientos = new List<MovimientoExcelEntity>();
                                foreach (RecibosEntitys recibo in recibos)
                                {
                                    if (porcentaje <= 75)
                                        porcentaje += Convert.ToInt32(factor);
                                    OnCambioProgreso(porcentaje);
                                    if (CancelacionPendiente)
                                        return "Proceso cancelado por el usuario";

                                    foreach (FormulaPolizaEntity formula in listaFormulas)
                                    {
                                        if (recibo.Moneda == formula.Moneda)
                                        {
                                            MovimientoExcelEntity movimiento = getExcelMovimientos(recibo, formula);
                                            if (movimiento == null)
                                                return "Error al obtener el movimiento del recibo " + recibo.Identificador;
                                            if (condiciones.TipoPresentacion == 1)
                                            {
                                                if (formula.Tipo == "IV")
                                                {
                                                    if (!cuentasIva.Contains(movimiento.Cuenta))
                                                        cuentasIva.Add(movimiento.Cuenta);
                                                }
                                                else if (formula.Tipo == "IA")
                                                {
                                                    if (!cuentasIngresos.Contains(movimiento.Cuenta))
                                                        cuentasIngresos.Add(movimiento.Cuenta);
                                                }
                                            }
                                            movimientos.Add(movimiento);
                                        }
                                    }
                                }
                                if (condiciones.TipoPresentacion == 1)
                                {
                                    if (cuentasIva.Count != 1)
                                        return "No se puede generar la póliza consolidada debido a que se encontraron cuentas diferentes para IVA";
                                    if (cuentasIngresos.Count != 1)
                                        return "No se puede generar la póliza consolidada debido a que se encontraron cuentas diferentes para Ingreso";
                                    poliza.Movimientos = consolidarExcel(movimientos, cuentasIva[0], cuentasIngresos[0]);
                                }
                                else
                                {
                                    poliza.Movimientos = movimientos;
                                }
                                OnCambioProgreso(100);
                                return generarXls(poliza, condiciones.TipoPresentacion, recibos[0].RazonSocial, condiciones.TipoPoliza);
                            }
                            catch
                            {
                                return "Hubo un error inesperado al momento de cargar los datos de la póliza Excel.";
                            }
                            #endregion
                        }
                        else if (condiciones.Formato == FormatoExportacionPoliza.Aspel)
                        {
                            #region Aspel
                            try
                            {
                                PolizaEntity poliza = new PolizaEntity();
                                EncabezadoPolizaEntity encabezado = new EncabezadoPolizaEntity();
                                encabezado.Fecha = condiciones.FechaInicio;
                                if (condiciones.TipoPoliza == 1)
                                    encabezado.TipoContpaq = 3;
                                else if (condiciones.TipoPoliza == 2)
                                    encabezado.TipoContpaq = 1;
                                else
                                    encabezado.TipoContpaq = 3;
                                poliza.Encabezado = encabezado;

                                OnCambioProgreso(50);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                int porcentaje = 50;
                                decimal factor = 40 / recibos.Count;
                                factor = factor >= 1 ? factor : 1;

                                List<MovimientoPolizaEntity> movimientos = new List<MovimientoPolizaEntity>();
                                foreach (RecibosEntitys recibo in recibos)
                                {
                                    if (porcentaje <= 75)
                                        porcentaje += Convert.ToInt32(factor);
                                    OnCambioProgreso(porcentaje);
                                    if (CancelacionPendiente)
                                        return "Proceso cancelado por el usuario";

                                    foreach (FormulaPolizaEntity formula in listaFormulas)
                                    {
                                        if (recibo.Moneda == formula.Moneda)
                                        {
                                            MovimientoPolizaEntity movimiento = getMovimientos(recibo, formula, condiciones.UsarConceptoPrincipal);
                                            if (movimiento == null)
                                                return "Error al obtener el movimiento del recibo " + recibo.Identificador;
                                            if (condiciones.IncluirUUIDEnConcepto)
                                                movimiento.Concepto = recibo.UUID + " " + movimiento.Concepto;
                                            if (condiciones.TipoPresentacion == 1)
                                            {
                                                if (formula.Tipo == "IV")
                                                {
                                                    if (!cuentasIva.Contains(movimiento.Cuenta))
                                                        cuentasIva.Add(movimiento.Cuenta);
                                                }
                                                else if (formula.Tipo == "IA")
                                                {
                                                    if (!cuentasIngresos.Contains(movimiento.Cuenta))
                                                        cuentasIngresos.Add(movimiento.Cuenta);
                                                }
                                            }
                                            movimientos.Add(movimiento);
                                        }
                                    }
                                }
                                if (condiciones.TipoPresentacion == 1)
                                {
                                    if (cuentasIva.Count != 1)
                                        return "No se puede generar la póliza consolidada debido a que se encontraron cuentas diferentes para IVA";
                                    if (cuentasIngresos.Count != 1)
                                        return "No se puede generar la póliza consolidada debido a que se encontraron cuentas diferentes para Ingreso";
                                    poliza.Movimientos = consolidar(movimientos, cuentasIva[0], cuentasIngresos[0]);
                                }
                                else
                                {
                                    poliza.Movimientos = movimientos;
                                }
                                OnCambioProgreso(100);
                                return generarAspel(poliza, condiciones.TipoPresentacion, recibos[0].RazonSocial, condiciones.TipoPoliza, condiciones.NumeroPoliza, condiciones.ConceptoPoliza);
                            }
                            catch
                            {
                                return "Hubo un error inesperado al momento de cargar los datos de la póliza Aspel-COI.";
                            }
                            #endregion
                        }
                        else if (condiciones.Formato == FormatoExportacionPoliza.Contavision)
                        {
                            #region Contavision
                            try
                            {
                                PolizaEntity poliza = new PolizaEntity();
                                EncabezadoPolizaEntity encabezado = new EncabezadoPolizaEntity();
                                encabezado.Fecha = condiciones.FechaInicio;
                                if (condiciones.TipoPoliza == 1)
                                    encabezado.TipoContpaq = 3;
                                else if (condiciones.TipoPoliza == 2)
                                    encabezado.TipoContpaq = 1;
                                else
                                    encabezado.TipoContpaq = 3;
                                poliza.Encabezado = encabezado;

                                OnCambioProgreso(50);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                int porcentaje = 50;
                                decimal factor = 40 / recibos.Count;
                                factor = factor >= 1 ? factor : 1;

                                List<MovimientoPolizaEntity> movimientos = new List<MovimientoPolizaEntity>();
                                foreach (RecibosEntitys recibo in recibos)
                                {
                                    if (porcentaje <= 75)
                                        porcentaje += Convert.ToInt32(factor);
                                    OnCambioProgreso(porcentaje);
                                    if (CancelacionPendiente)
                                        return "Proceso cancelado por el usuario";

                                    foreach (FormulaPolizaEntity formula in listaFormulas)
                                    {
                                        if (recibo.Moneda == formula.Moneda)
                                        {
                                            MovimientoPolizaEntity movimiento = getMovimientos(recibo, formula, condiciones.UsarConceptoPrincipal);
                                            if (movimiento == null)
                                                return "Error al obtener el movimiento del recibo " + recibo.Identificador;
                                            if (condiciones.TipoPresentacion == 1)
                                            {
                                                if (formula.Tipo == "IV")
                                                {
                                                    if (!cuentasIva.Contains(movimiento.Cuenta))
                                                        cuentasIva.Add(movimiento.Cuenta);
                                                }
                                                else if (formula.Tipo == "IA")
                                                {
                                                    if (!cuentasIngresos.Contains(movimiento.Cuenta))
                                                        cuentasIngresos.Add(movimiento.Cuenta);
                                                }
                                            }
                                            movimientos.Add(movimiento);
                                        }
                                    }
                                }
                                if (condiciones.TipoPresentacion == 1)
                                {
                                    if (cuentasIva.Count != 1)
                                        return "No se puede generar la póliza consolidada debido a que se encontraron cuentas diferentes para IVA";
                                    if (cuentasIngresos.Count != 1)
                                        return "No se puede generar la póliza consolidada debido a que se encontraron cuentas diferentes para Ingreso";
                                    poliza.Movimientos = consolidar(movimientos, cuentasIva[0], cuentasIngresos[0]);
                                }
                                else
                                {
                                    poliza.Movimientos = movimientos;
                                }
                                OnCambioProgreso(100);
                                return generarContavision(poliza, condiciones.TipoPresentacion, recibos[0].RazonSocial, condiciones.TipoPoliza, condiciones.NumeroPoliza, condiciones.ConceptoPoliza);
                            }
                            catch
                            {
                                return "Hubo un error inesperado al momento de cargar los datos de la póliza Contavision.";
                            }
                            #endregion
                        }
                        else if (condiciones.Formato == FormatoExportacionPoliza.Axapta)
                        {
                            #region Axapta
                            try
                            {
                                PolizaEntity poliza = new PolizaEntity();
                                List<MovimientoPolizaEntity> movimientos = new List<MovimientoPolizaEntity>();
                                OnCambioProgreso(50);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                int porcentaje = 50;
                                decimal factor = 40 / recibos.Count;
                                factor = factor >= 1 ? factor : 1;

                                foreach (RecibosEntitys recibo in recibos)
                                {
                                    if (porcentaje <= 75)
                                        porcentaje += Convert.ToInt32(factor);
                                    OnCambioProgreso(porcentaje);
                                    if (CancelacionPendiente)
                                        return "Proceso cancelado por el usuario";

                                    foreach (FormulaPolizaEntity formula in listaFormulas)
                                    {
                                        if (recibo.Moneda == formula.Moneda)
                                        {
                                            MovimientoPolizaEntity movimiento = getMovimientos(recibo, formula, condiciones.UsarConceptoPrincipal);
                                            movimiento.RFC = recibo.ClienteRFC;
                                            if (movimiento == null)
                                                return "Error al obtener el movimiento del recibo " + recibo.Identificador;
                                            if (condiciones.TipoPresentacion == 1)
                                            {
                                                if (formula.Tipo == "IV")
                                                {
                                                    if (!cuentasIva.Contains(movimiento.Cuenta))
                                                        cuentasIva.Add(movimiento.Cuenta);
                                                }
                                                else if (formula.Tipo == "IA")
                                                {
                                                    if (!cuentasIngresos.Contains(movimiento.Cuenta))
                                                        cuentasIngresos.Add(movimiento.Cuenta);
                                                }
                                            }
                                            movimientos.Add(movimiento);
                                        }
                                    }
                                }
                                if (condiciones.TipoPresentacion == 1)
                                {
                                    if (cuentasIva.Count > 1)
                                        return "No se puede generar la póliza consolidada debido a que se encontraron cuentas diferentes para IVA";
                                    if (cuentasIngresos.Count > 1)
                                        return "No se puede generar la póliza consolidada debido a que se encontraron cuentas diferentes para Ingreso";
                                    poliza.Movimientos = consolidar(movimientos, cuentasIva[0], cuentasIngresos[0]);
                                }
                                else
                                {
                                    poliza.Movimientos = movimientos;
                                }
                                OnCambioProgreso(100);
                                return generarAxapta(poliza, condiciones.TipoPresentacion, recibos[0].RazonSocial, condiciones, false);
                            }
                            catch
                            {
                                return "Hubo un error inesperado al momento de cargar los datos de la póliza Axapta.";
                            }
                            #endregion
                        }
                        else if (condiciones.Formato == FormatoExportacionPoliza.ContpaqXls)
                        {
                            #region Contpaqt .xls
                            try
                            {
                                string[] mascara = null;
                                if (!string.IsNullOrEmpty(condiciones.Mascara))
                                    mascara = condiciones.Mascara.Trim().Split('-');
                                PolizaEntity poliza = new PolizaEntity();
                                List<MovimientoPolizaEntity> ListadoMovimientos = new List<MovimientoPolizaEntity>();
                                EncabezadoPolizaEntity encabezado = new EncabezadoPolizaEntity();
                                encabezado.Fecha = condiciones.TipoPresentacion != 3 ? condiciones.FechaInicio : condiciones.FechaFin;
                                if (condiciones.TipoPoliza == 1)
                                    encabezado.TipoContpaq = 3;
                                else if (condiciones.TipoPoliza == 2)
                                    encabezado.TipoContpaq = 1;
                                else
                                    encabezado.TipoContpaq = 3;
                                poliza.Encabezado = encabezado;

                                OnCambioProgreso(50);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                int porcentaje = 50;
                                decimal factor = 40 / recibos.Count;
                                factor = factor >= 1 ? factor : 1;

                                List<MovimientoPolizaEntity> movimientos = new List<MovimientoPolizaEntity>();
                                bool esPrimero = false;
                                bool IVA_Asignado = false;
                                bool IVAT_Asignado = false;
                                bool ComplemBancosAsignada = false;
                                bool BancoAsignado = false;
                                string UUIDsPagadas = "";
                                //bool cambioRefPag = true;
                                int idPagoCob = -1;
                                int idPoliza = 0;
                                List<ConceptoGeneralEntity> listaConceptos = new List<ConceptoGeneralEntity>();
                                bool primerConcepto = true;
                                //ConceptoGeneralEntity conceptoGeneral = new ConceptoGeneralEntity();
                                string conceptoGen = "";
                                if (!condiciones.MultiplesEncabezados)
                                    conceptoGen = condiciones.ConceptoPoliza;
                                //ModuloCobranzaEntity cobranzaEnt= SaariDB.getVersionCobranza();
                                //bool primerMovEvaluado = false;
                                foreach (RecibosEntitys recibo in recibos)
                                {
                                    BancoAsignado = false;
                                    if (porcentaje <= 75)
                                        porcentaje += Convert.ToInt32(factor);
                                    OnCambioProgreso(porcentaje);
                                    if (CancelacionPendiente)
                                        return "Proceso cancelado por el usuario";

                                    //if (condiciones.TipoPoliza == 2 && condiciones.FechaInicio.Date > fechaActPol.Date && versionOBDC >= 8645)
                                    //Validando la version de cobranza instalada
                                    if (cobranzaEntity.Version >= 2212 && condiciones.TipoPoliza == 2 && condiciones.FechaInicio.Date > cobranzaEntity.FechaAct.Date && recibo.IdPagoCobranza > 0)
                                        esPrimero = false;
                                    else
                                    {
                                        esPrimero = true;
                                       // if (condiciones.MultiplesEncabezados)
                                            idPoliza++;
                                    }
                                    foreach (FormulaPolizaEntity formula in listaFormulas)
                                    {
                                        // 3.3.8.1 Se agrega nuevo campo para identificar la moneda de paga en la estructura de la configuración de la póliza
                                        // 23/05/2017 by UZ
                                        if ((recibo.Moneda == formula.Moneda && recibo.MonedaPago == formula.MonedaPago && condiciones.TipoPoliza == 2) || (recibo.Moneda == formula.Moneda && condiciones.TipoPoliza != 2))
                                        {
                                            #region MOVIMIENTOS EN MISMA MONEDA
                                            //inlcuir validacion para version COBRANZA 
                                            //cuando la version sea mayor o igual que 2.2.1.2
                                            //a partir de la version 2.2.1.2 se utiliza el campo CAMPO_NUM15 de la tabla T24(firebird)
                                            //que hace referencia al IdPago de la tabla cobranza.Pago(SQL Server)
                                            MovimientoPolizaEntity movimiento = getMovimientos(recibo, formula, condiciones.UsarConceptoPrincipal);
                                            if (movimiento == null)
                                                return "Error al obtener el movimiento del recibo " + recibo.Identificador;
                                            //if (versionOBDC >= 8645 && condiciones.FechaInicio>fechaActPol)
                                            //if (movimiento != null)
                                            //    System.Windows.Forms.MessageBox.Show("Cuenta: " + movimiento.Cuenta + " Descripción : " + movimiento.CuentaDescripcion);
                                            if (cobranzaEntity.Version >= 2212 && condiciones.FechaInicio.Date > cobranzaEntity.FechaAct.Date && recibo.IdPagoCobranza > 0)
                                            {
                                                #region POLIZAS DE INGRESOS
                                                //Para polizas de ingresos

                                                if (condiciones.MultiplesEncabezados && condiciones.TipoPoliza == 2)
                                                {
                                                    if (idPagoCob != recibo.IdPagoCobranza)
                                                    {

                                                        //movimiento.EsPrimero = true;
                                                        IVA_Asignado = false;
                                                        IVAT_Asignado = false;
                                                        ComplemBancosAsignada = false;
                                                        if ((movimiento.Tipo == "BMN" || movimiento.Tipo == "BDL" && !BancoAsignado) && movimiento.ImporteNacional > 0)
                                                        {
                                                            //movimiento.Concepto = "BANCOS";
                                                            movimiento.UUID = "";
                                                            movimiento.Excluir = false;
                                                            BancoAsignado = true;
                                                            if (idPagoCob > 0)
                                                                idPoliza++;
                                                        }
                                                        if (!primerConcepto)
                                                        {
                                                            ConceptoGeneralEntity Con = new ConceptoGeneralEntity();
                                                            Con.Id = idPagoCob;
                                                            Con.Concepto = conceptoGen;
                                                            Con.UUIDs = UUIDsPagadas;
                                                            listaConceptos.Add(Con);
                                                            conceptoGen = "";
                                                            UUIDsPagadas = "";
                                                        }

                                                    }
                                                    if (idPagoCob == recibo.IdPagoCobranza)
                                                    {
                                                        movimiento.EsPrimero = false;
                                                        if (movimiento.Tipo == "BMN" || movimiento.Tipo == "BDL" && BancoAsignado)
                                                            movimiento.Excluir = true;
                                                        else if (movimiento.Tipo == "BMN" || movimiento.Tipo == "BDL" && !BancoAsignado)
                                                        {
                                                            BancoAsignado = true;
                                                            movimiento.UUID = "";
                                                            movimiento.Excluir = false;
                                                            if (idPagoCob > 0)
                                                                idPoliza++;
                                                            if (!primerConcepto)
                                                            {
                                                                ConceptoGeneralEntity Con = new ConceptoGeneralEntity();
                                                                Con.Id = idPagoCob;
                                                                Con.Concepto = conceptoGen;
                                                                Con.UUIDs = UUIDsPagadas;
                                                                listaConceptos.Add(Con);
                                                                conceptoGen = "";
                                                                UUIDsPagadas = "";
                                                            }
                                                            IVA_Asignado = false;
                                                            IVAT_Asignado = false;
                                                            ComplemBancosAsignada = false;
                                                        }
                                                    }
                                                    if (!IVA_Asignado && movimiento.Tipo == "IV" && movimiento.ImporteNacional > 0)
                                                    {
                                                        IVA_Asignado = true;
                                                        movimiento.Excluir = false;
                                                        //movimiento.Concepto = "IVA POR TRASLADAR";
                                                        movimiento.UUID = "";

                                                    }
                                                    else if (movimiento.Tipo == "IV")
                                                    {
                                                        movimiento.Excluir = true;
                                                    }
                                                    if (!IVAT_Asignado && movimiento.Tipo == "IVT" && movimiento.ImporteNacional > 0)
                                                    {
                                                        IVAT_Asignado = true;
                                                        movimiento.Excluir = false;
                                                        //movimiento.Concepto = "IVA TRASLADADO";
                                                        movimiento.UUID = "";

                                                    }
                                                    else if (movimiento.Tipo == "IVT")
                                                    {
                                                        movimiento.Excluir = true;
                                                    }
                                                    if (!ComplemBancosAsignada && movimiento.Tipo == "BCC" && movimiento.ImporteNacional > 0)
                                                    {
                                                        movimiento.Excluir = false;
                                                        ComplemBancosAsignada = true;
                                                    }
                                                    else if (movimiento.Tipo == "BCC")
                                                    {
                                                        movimiento.Excluir = true;
                                                    }
                                                    if (condiciones.MultiplesEncabezados && recibo.IdPagoCobranza > 0)
                                                    {
                                                        if (!esPrimero && movimiento.ImporteNacional > 0)
                                                        {
                                                            movimiento.EsPrimero = true;
                                                            esPrimero = true;
                                                        }
                                                        else
                                                        {
                                                            movimiento.EsPrimero = false;
                                                        }
                                                        if (movimiento.Tipo == "CC")
                                                        {
                                                            if (condiciones.ClienteEnLugarDePeriodo)
                                                                conceptoGen += recibo.SerieFolio + recibo.Inmueble + recibo.ClienteRazonSocial + "| ";
                                                            else
                                                                conceptoGen += recibo.SerieFolio + recibo.Inmueble + recibo.Periodo + "| ";
                                                            primerConcepto = false;
                                                            UUIDsPagadas += recibo.UUID + "|";
                                                        }
                                                    }
                                                    else if (idPagoCob == 0)
                                                    {
                                                        if (!esPrimero && movimiento.ImporteNacional > 0)
                                                        {
                                                            movimiento.EsPrimero = true;
                                                            esPrimero = true;
                                                        }
                                                        else
                                                        {
                                                            movimiento.EsPrimero = false;
                                                        }
                                                        if (movimiento.Tipo == "CC")
                                                        {
                                                            if (condiciones.ClienteEnLugarDePeriodo)
                                                                conceptoGen += recibo.SerieFolio + recibo.Inmueble + recibo.ClienteRazonSocial + "| ";
                                                            else
                                                                conceptoGen += recibo.SerieFolio + recibo.Inmueble + recibo.Periodo + "| ";
                                                            primerConcepto = false;
                                                            UUIDsPagadas += recibo.UUID + "|";
                                                        }
                                                    }
                                                    idPagoCob = recibo.IdPagoCobranza;
                                                    movimiento.IdPoliza = idPoliza;
                                                }
                                                #endregion

                                                #region POLIZAS DIFERENTES DE INGRESOS
                                                if (condiciones.MultiplesEncabezados && condiciones.TipoPoliza != 2)
                                                {
                                                    movimiento.EsPrimero = esPrimero;
                                                    //idPoliza++;
                                                    esPrimero = false;
                                                    //AGREGAR CODIGO PARA ASIGNAR ID DE MOVIMIENTOS POLIZA
                                                    //PARA IDENTIFICAR LOS MOVIMIENTOS QUE PERTENECEN A UNA MISMA POLIZA 
                                                    movimiento.IdPoliza = idPoliza;

                                                }
                                                #endregion
                                                movimiento.IdPoliza = idPoliza;

                                            }
                                            else if (condiciones.MultiplesEncabezados)
                                            {
                                                movimiento.EsPrimero = esPrimero;
                                                esPrimero = false;
                                                //AGREGAR CODIGO PARA ASIGNAR ID DE MOVIMIENTOS POLIZA
                                                //PARA IDENTIFICAR LOS MOVIMIENTOS QUE PERTENECEN A UNA MISMA POLIZA 
                                                movimiento.IdPoliza = idPoliza;
                                                //idPoliza++;
                                            }
                                            if (!string.IsNullOrEmpty(movimiento.Cuenta) && movimiento.Cuenta != "NO CUENTA")
                                            {
                                                movimiento.IdPoliza = idPoliza;
                                                //idPoliza++;
                                                #region MASCARA
                                                if (mascara != null)
                                                {
                                                    string cuentaEnmascarada = string.Empty;
                                                    string[] cuenta = movimiento.Cuenta.Trim().Split('-');
                                                    if (cuenta.Length == mascara.Length)
                                                    {
                                                        #region CON MASCARA PREDEFINIDA EN CUENTA
                                                        bool error = false;
                                                        for (int i = 0; i < cuenta.Length; i++)
                                                        {
                                                            if (!string.IsNullOrEmpty(cuenta[i]) && !string.IsNullOrEmpty(mascara[i]))
                                                            {
                                                                int masc = Convert.ToInt32(mascara[i]);
                                                                string m = string.Empty;
                                                                for (int j = 0; j < masc; j++)
                                                                    m += "9";
                                                                int max = Convert.ToInt32(m);
                                                                if (Convert.ToInt64(cuenta[i]) <= max)
                                                                {
                                                                    if (cuenta[i].Length >= masc)
                                                                    {
                                                                        cuentaEnmascarada += cuenta[i].Substring(cuenta[i].Length - masc) + "-";
                                                                    }
                                                                    else
                                                                    {
                                                                        cuentaEnmascarada += cuenta[i].PadLeft(masc, '0') + "-";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    error = true;
                                                                    break;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                error = true;
                                                                break;
                                                            }
                                                        }
                                                        if (!error && !string.IsNullOrEmpty(cuentaEnmascarada))
                                                            movimiento.Cuenta = cuentaEnmascarada.Substring(0, cuentaEnmascarada.Length - 1);
                                                        #endregion
                                                    }
                                                    else if (cuenta.Length == 1 && mascara.Length > 1)
                                                    {
                                                        #region SIN MASCARA PREDEFINIDA EN CUENTA
                                                        bool error = false;
                                                        int pos = 0;
                                                        foreach (string mask in mascara)
                                                        {
                                                            int masc = Convert.ToInt32(mask);
                                                            string segmento = string.Empty;
                                                            try
                                                            {
                                                                segmento = cuenta[0].Substring(pos, masc);
                                                            }
                                                            catch
                                                            {
                                                                try
                                                                {
                                                                    segmento = cuenta[0].Substring(pos);
                                                                    segmento = segmento.PadLeft(masc, '0');
                                                                }
                                                                catch
                                                                {
                                                                    segmento = segmento.PadLeft(masc, '0');
                                                                }
                                                            }
                                                            pos += masc;
                                                            cuentaEnmascarada += segmento + "-";
                                                        }
                                                        if (!error && !string.IsNullOrEmpty(cuentaEnmascarada))
                                                            movimiento.Cuenta = cuentaEnmascarada.Substring(0, cuentaEnmascarada.Length - 1);
                                                        #endregion
                                                    }
                                                    else if (cuenta.Length < mascara.Length)
                                                    {
                                                        #region MASCARA PREDEFINIDA MENOR A MASCARA
                                                        bool error = false;
                                                        int limite = cuenta.Length;
                                                        for (int i = 0; i < mascara.Length; i++)
                                                        {
                                                            if (!string.IsNullOrEmpty(mascara[i]))
                                                            {
                                                                int masc = Convert.ToInt32(mascara[i]);
                                                                string m = string.Empty;
                                                                for (int j = 0; j < masc; j++)
                                                                    m += "9";
                                                                int max = Convert.ToInt32(m);
                                                                if (i < limite)
                                                                {
                                                                    if (Convert.ToInt64(cuenta[i]) <= max)
                                                                        cuentaEnmascarada += cuenta[i].Substring(cuenta[i].Length - masc) + "-";
                                                                    else
                                                                    {
                                                                        error = true;
                                                                        break;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    for (int j = 0; j < m.Length; j++)
                                                                        cuentaEnmascarada += "0";
                                                                    cuentaEnmascarada += "-";
                                                                }
                                                            }
                                                        }
                                                        if (!error && !string.IsNullOrEmpty(cuentaEnmascarada))
                                                            movimiento.Cuenta = cuentaEnmascarada.Substring(0, cuentaEnmascarada.Length - 1);
                                                        #endregion
                                                    }
                                                }
                                                #endregion
                                            }
                                            if (condiciones.TipoPresentacion == 1)
                                            {
                                                movimiento.IdPoliza = idPoliza;
                                                //idPoliza++;
                                                if (formula.Tipo == "IV")
                                                {
                                                    if (!cuentasIva.Contains(movimiento.Cuenta))
                                                        cuentasIva.Add(movimiento.Cuenta);
                                                }
                                                else if (formula.Tipo == "IA")
                                                {
                                                    if (!cuentasIngresos.Contains(movimiento.Cuenta))
                                                        cuentasIngresos.Add(movimiento.Cuenta);
                                                }
                                            }
                                            
                                            movimiento.IdPoliza = idPoliza;

                                            movimientos.Add(movimiento);
                                            #endregion
                                        }

                                        else if (recibo.Moneda == "D")
                                        {
                                            #region PAGOS EN PESOS A CONTRATOS EN DOLARES
                                            //TO DO: Implementar esta forma de póliza
                                           
                                            #endregion
                                        }
                                        else if (recibo.Moneda == "P" && (recibo.MonedaPago == "D"))
                                        {
                                            #region MOVIMIENTOS EN FACTURA EN PESOS PAGO EN DOLARES
                                            //inlcuir validacion para version COBRANZA 
                                            //cuando la version sea mayor o igual que 2.2.1.2
                                            //a partir de la version 2.2.1.2 se utiliza el campo CAMPO_NUM5 de la tabla T24(firebird)
                                            //que hace referencia al IdPago de la tabla cobranza.Pago(SQL Server)

                                            #endregion
                                        }
                                    }

                                }
                                //
                                ConceptoGeneralEntity Conc = new ConceptoGeneralEntity();
                                if (idPagoCob > 0)
                                    Conc.Id = idPagoCob;
                                else
                                    Conc.Id = idPoliza;
                                Conc.Concepto = conceptoGen;
                                Conc.UUIDs = UUIDsPagadas;
                                listaConceptos.Add(Conc);

                                if (condiciones.TipoPresentacion == 1)
                                {
                                    if (cuentasIva.Count != 1)
                                        return "No se puede generar la póliza consolidada debido a que se encontraron cuentas diferentes para IVA";
                                    if (cuentasIngresos.Count != 1)
                                        return "No se puede generar la póliza consolidada debido a que se encontraron cuentas diferentes para Ingreso";
                                    poliza.Movimientos = consolidar(movimientos, cuentasIva[0], cuentasIngresos[0]);
                                }
                                else if (condiciones.TipoPresentacion == 3)
                                {
                                    //poliza.Movimientos = globalizar(movimientos);
                                    ListadoMovimientos = globalizar(movimientos);
                                }
                                else
                                {
                                    #region MOVIMIENTOS AUXILIARES
                                    movimientos.Add(new MovimientoPolizaEntity() { Excluir = true, EsPrimero = true });
                                    List<MovimientoPolizaEntity> movimientosAux = new List<MovimientoPolizaEntity>();
                                    foreach (var mov in movimientos)
                                    {
                                        if (mov.EsPrimero)
                                        {
                                            if (movimientosAux.Count > 0)
                                            {
                                                if (movimientosAux.Count(m => m.Tipo == "CD") > 0)
                                                {
                                                    decimal cargos = movimientosAux.Where(m => m.TipoMovimiento == 1).Sum(m => m.ImporteNacional);
                                                    decimal abonos = movimientosAux.Where(m => m.TipoMovimiento == 2).Sum(m => m.ImporteNacional);
                                                    decimal diferencia = Math.Abs(cargos - abonos);
                                                    if (diferencia > 0 && diferencia <= 0.10m)
                                                    {
                                                        var movimiento = movimientosAux.FirstOrDefault(m => m.Tipo == "CD");
                                                        if (movimiento != null)
                                                        {
                                                            if (mov.Referencia == movimiento.Referencia)
                                                            {
                                                                if (encabezado.TipoContpaq == 3)
                                                                {
                                                                    if (cargos > abonos)
                                                                        movimiento.ImporteNacional -= diferencia;
                                                                    else
                                                                        movimiento.ImporteNacional += diferencia;
                                                                }
                                                                else if (encabezado.TipoContpaq == 1)
                                                                {
                                                                    if (cargos > abonos)
                                                                        movimiento.ImporteNacional += diferencia;
                                                                    else
                                                                        movimiento.ImporteNacional -= diferencia;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            movimientosAux.Clear();
                                        }
                                        movimientosAux.Add(mov);
                                    }
                                    #endregion

                                    //Modify by Ing. Rodrigo Uzcanga 10/08/2015
                                    //poliza.Movimientos = movimientos;
                                    ListadoMovimientos = movimientos;
                                }
                                OnCambioProgreso(100);
                                poliza.ConceptosGenerales = listaConceptos;

                                #region SOLUCION A DIFERENCIA POR CENTAVOS
                                int id = 1;
                                decimal sumaCargos = 0, sumaAbonos = 0, difCarAb = 0;
                                List<MovimientoPolizaEntity> movimientosFinales = new List<MovimientoPolizaEntity>();
                                MovimientoPolizaEntity movPol = new MovimientoPolizaEntity();
                                List<MovimientoPolizaEntity> movimientosDiferencias = new List<MovimientoPolizaEntity>();

                                int cantDec = condiciones.CuatroDecimales ? 4 : 2;
                                int tipoMov = 0;
                                bool buscaDiferencias = false;
                                //FormulaPolizaEntity formulaDif = Polizas.getFormulaPorTipo(condiciones.Inmobiliaria, condiciones.TipoPoliza, "DIF");
                                //if (formulaDif.TipoClave=="DIF" && cobranzaEntity.Version >= 2212 && (condiciones.TipoPoliza == 2 || condiciones.TipoPoliza==1) && condiciones.FechaInicio.Date > cobranzaEntity.FechaAct.Date )
                                //if (formulaDif.TipoClave == "DIF" && (( condiciones.TipoPoliza == 1 || condiciones.TipoPoliza==4) || (condiciones.TipoPoliza == 2 && condiciones.FechaInicio.Date > cobranzaEntity.FechaAct.Date)))
                                if ((condiciones.TipoPoliza == 1 || condiciones.TipoPoliza == 4) || (condiciones.TipoPoliza == 2 && condiciones.FechaInicio.Date > cobranzaEntity.FechaAct.Date && cobranzaEntity.Version >= 2212))
                                {
                                    buscaDiferencias = true;
                                    foreach (MovimientoPolizaEntity pol in ListadoMovimientos)
                                    {
                                        //if (pol.Cliente == "CTE786")
                                        //{ }
                                        if (!pol.Excluir)
                                        {
                                            if (condiciones.TipoPoliza == 1 || condiciones.TipoPoliza == 4)
                                            {
                                                #region POLIZAS DE DIARIO Y DE NOTAS DE CREDITO

                                                if (id == pol.IdPoliza)
                                                {
                                                    #region AL ESTAR LEYENDO LA MISMA POLIZA
                                                    //ACUMULA CARGOS Y ABONOS PARA DESPUES DETERMINAR DIFERENCIAS EN EL CAMBIO DE POLIZA
                                                    if (pol.TipoMovimiento == 1)
                                                        sumaCargos += decimal.Round(pol.ImporteNacional, cantDec);
                                                    else
                                                        sumaAbonos += decimal.Round(pol.ImporteNacional, cantDec);
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region AL DETECTAR QUE SE LEE LA SIGUIENTE POLIZA
                                                    //CALCULA DIFERENCIA Y SI HAY DIFERENCIA AGREGA UN NUEVO MOVIMIENTO PARA LAS DIFERENCIAS
                                                    if (sumaCargos > sumaAbonos)
                                                    {
                                                        difCarAb = sumaCargos - sumaAbonos;
                                                        //if (condiciones.TipoPoliza == 1)
                                                        {
                                                            tipoMov = 2;
                                                            movPol.TipoMovimiento = 2;
                                                        }
                                                    }
                                                    else if (sumaAbonos > sumaCargos)
                                                    {
                                                        difCarAb = sumaAbonos - sumaCargos;

                                                        //if (condiciones.TipoPoliza == 1)
                                                        {
                                                            tipoMov = 1;
                                                            movPol.TipoMovimiento = 1;
                                                        }
                                                    }
                                                    if (difCarAb > 0)
                                                    {
                                                        movPol.TipoMovimiento = tipoMov;
                                                        movPol.ImporteNacional = difCarAb;
                                                        //CUENTA Cuenta = buscarCuenta(movPol.IdInmob, movPol.Cliente, "DIF");
                                                        //if (Cuenta.NumCuenta == "NO CUENTA")
                                                        //    Cuenta.NumCuenta = "9999";
                                                        movimientosDiferencias.Add(movPol);
                                                        difCarAb = 0;
                                                    }
                                                    sumaCargos = 0;
                                                    sumaAbonos = 0;
                                                    if (pol.TipoMovimiento == 1)
                                                        sumaCargos += decimal.Round(pol.ImporteNacional, cantDec);
                                                    else
                                                        sumaAbonos += decimal.Round(pol.ImporteNacional, cantDec);

                                                    #endregion
                                                }//fin else cambio de poliza
                                                //Se resguarda el id del movimiento antes de pasar al siguiente mov
                                                id = pol.IdPoliza;
                                                movPol = new MovimientoPolizaEntity();
                                                //if (condiciones.TipoPoliza == 1 || condiciones.TipoPoliza == 4)
                                                {
                                                    #region ASIGNAR DATOS AL MOVIMIENTO TEMPORAL
                                                    {
                                                        movPol.IdPoliza = pol.IdPoliza;
                                                        movPol.IdPagoCobranza = pol.IdPagoCobranza;
                                                        movPol.ClienteNombreComercial = pol.ClienteNombreComercial;
                                                        movPol.Concepto = pol.Concepto;
                                                        movPol.EsLibre = pol.EsLibre;
                                                        movPol.EsPrimero = false;
                                                        movPol.Fecha = pol.Fecha;
                                                        movPol.IdentificadorConjunto = pol.IdentificadorConjunto;
                                                        movPol.Moneda = pol.Moneda;
                                                        movPol.Periodo = pol.Periodo;
                                                        movPol.Referencia = pol.Referencia;
                                                        movPol.Segmento = pol.Segmento;
                                                        movPol.SerieFolio = pol.SerieFolio;
                                                        movPol.UUID = pol.UUID;
                                                        movPol.Cuenta = "9999999";
                                                        movPol.CuentaDescripcion = "DIFERENCIA";
                                                        movPol.Concepto = pol.Concepto;
                                                        movPol.Excluir = false;
                                                        movPol.Tipo = "DIF";
                                                        //movPol.TipoMovimiento = 9;
                                                        movPol.Cliente = pol.Cliente;
                                                        movPol.Contrato = pol.Contrato;
                                                    }
                                                    #endregion
                                                    //Se agrega el movimiento a la lista final filtrada
                                                    movimientosFinales.Add(pol);
                                                    // movimientosDiferencias.Add(pol);
                                                }

                                                #endregion
                                            }
                                            else if (condiciones.TipoPoliza == 2 && cobranzaEntity.Version >= 2212 && condiciones.FechaInicio.Date > cobranzaEntity.FechaAct.Date)
                                            {
                                                #region POLIZAS DE INGRESOS

                                                if (id == pol.IdPoliza)
                                                {
                                                    #region AL ESTAR LEYENDO LA MISMA POLIZA
                                                    //ACUMULA CARGOS Y ABONOS PARA DESPUES DETERMINAR DIFERENCIAS EN EL CAMBIO DE POLIZA
                                                    if (pol.TipoMovimiento == 1)
                                                        sumaCargos += decimal.Round(pol.ImporteNacional, cantDec);
                                                    else
                                                        sumaAbonos += decimal.Round(pol.ImporteNacional, cantDec);
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region AL DETECTAR QUE SE LEE LA SIGUIENTE POLIZA
                                                    //CALCULA DIFERENCIA Y SI HAY DIFERENCIA AGREGA UN NUEVO MOVIMIENTO PARA LAS DIFERENCIAS
                                                    if (sumaCargos > sumaAbonos)
                                                    {
                                                        difCarAb = sumaCargos - sumaAbonos;
                                                        tipoMov = 2;
                                                        movPol.TipoMovimiento = 2;
                                                    }
                                                    else if (sumaAbonos > sumaCargos)
                                                    {
                                                        difCarAb = sumaAbonos - sumaCargos;
                                                        tipoMov = 1;
                                                        movPol.TipoMovimiento = 1;
                                                    }
                                                    if (difCarAb > 0)
                                                    {
                                                        movPol.TipoMovimiento = tipoMov;
                                                        movPol.ImporteNacional = difCarAb;
                                                        movimientosDiferencias.Add(movPol);
                                                        difCarAb = 0;
                                                    }
                                                    sumaCargos = 0;
                                                    sumaAbonos = 0;
                                                    if (pol.TipoMovimiento == 1)
                                                        sumaCargos += decimal.Round(pol.ImporteNacional, cantDec);
                                                    else
                                                        sumaAbonos += decimal.Round(pol.ImporteNacional, cantDec);

                                                    #endregion
                                                }//fin else cambio de poliza
                                                //Se resguarda el id del movimiento antes de pasar al siguiente mov
                                                id = pol.IdPoliza;
                                                movPol = new MovimientoPolizaEntity();
                                                #region ASIGNAR DATOS AL MOVIMIENTO TEMPORAL
                                                {
                                                    movPol.IdPoliza = pol.IdPoliza;
                                                    movPol.IdPagoCobranza = pol.IdPagoCobranza;
                                                    movPol.ClienteNombreComercial = pol.ClienteNombreComercial;
                                                    movPol.Concepto = pol.Concepto;
                                                    movPol.EsLibre = pol.EsLibre;
                                                    movPol.EsPrimero = false;
                                                    movPol.Fecha = pol.Fecha;
                                                    movPol.IdentificadorConjunto = pol.IdentificadorConjunto;
                                                    movPol.Moneda = pol.Moneda;
                                                    movPol.Periodo = pol.Periodo;
                                                    movPol.Referencia = pol.Referencia;
                                                    movPol.Segmento = pol.Segmento;
                                                    movPol.SerieFolio = pol.SerieFolio;
                                                    movPol.UUID = pol.UUID;
                                                    movPol.Cuenta = "9999999";
                                                    movPol.CuentaDescripcion = "DIFERENCIA";
                                                    movPol.Concepto = pol.Concepto;
                                                    movPol.Excluir = false;
                                                    movPol.Tipo = "DIF";
                                                    //movPol.TipoMovimiento = 9;
                                                    movPol.Cliente = pol.Cliente;
                                                    movPol.Contrato = pol.Contrato;
                                                }
                                                #endregion
                                                //Se agrega el movimiento a la lista final filtrada
                                                movimientosFinales.Add(pol);
                                                #endregion
                                            }


                                        }// fin if (!pol.Excluir)
                                    }//fin foreach
                                    //Se evalua el ultimo movimiento para determinar si hay diferencias
                                    #region ULTIMO MOVIMIENTO DE LA POLIZA
                                    if (sumaCargos > sumaAbonos)
                                    {
                                        difCarAb = sumaCargos - sumaAbonos;
                                        {
                                            tipoMov = 2;
                                            movPol.TipoMovimiento = 2;
                                        }
                                    }
                                    else if (sumaAbonos > sumaCargos)
                                    {
                                        difCarAb = sumaAbonos - sumaCargos;
                                        {
                                            tipoMov = 1;
                                            movPol.TipoMovimiento = 1;
                                        }
                                    }
                                    if (difCarAb > 0)
                                    {
                                        movPol.TipoMovimiento = tipoMov;
                                        movPol.ImporteNacional = difCarAb;
                                        movimientosDiferencias.Add(movPol);
                                    }
                                    #endregion
                                    poliza.Movimientos = movimientosFinales;
                                }
                                #endregion
                                #region ASIGNAR DIFERENCIAS A LAS CUENTAS COMPLEMENTARIAS
                                if (buscaDiferencias)
                                {
                                    if (condiciones.TipoPoliza == 1)
                                    {
                                        #region ASIGNAR DIFERENCIAS POLIZAS DIARIO Y NOTAS DE CREDITO
                                        foreach (MovimientoPolizaEntity MovDif in movimientosDiferencias)
                                        {
                                            foreach (MovimientoPolizaEntity movFin in movimientosFinales)
                                            {
                                                if (MovDif.IdPoliza == movFin.IdPoliza && movFin.Tipo == "CDC")
                                                {
                                                    if (MovDif.TipoMovimiento == 1)
                                                    {
                                                       if(movFin.Referencia == MovDif.Referencia)
                                                            movFin.ImporteNacional += MovDif.ImporteNacional;
                                                    }
                                                    else
                                                    {
                                                        if (movFin.Referencia == MovDif.Referencia)
                                                            movFin.ImporteNacional -= MovDif.ImporteNacional;
                                                    }
                                                        break;
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (condiciones.TipoPoliza == 2)
                                    {
                                        #region ASIGNAR DIFERENCIAS POLIZAS DE INGRESOS
                                        foreach (MovimientoPolizaEntity MovDif in movimientosDiferencias)
                                        {
                                            foreach (MovimientoPolizaEntity movFin in movimientosFinales)
                                            {
                                                //EN COMPLEMENTARIA DE BANCOS CUANDO EL PAGO ES EN DOLARES
                                                if (MovDif.IdPoliza == movFin.IdPoliza && movFin.Tipo == "BCC")
                                                {
                                                    if (MovDif.TipoMovimiento == 1)
                                                    {
                                                        if (movFin.Referencia == MovDif.Referencia)
                                                            movFin.ImporteNacional += MovDif.ImporteNacional;
                                                    }
                                                    else
                                                    {
                                                        if (movFin.Referencia == MovDif.Referencia)
                                                            movFin.ImporteNacional -= MovDif.ImporteNacional;
                                                    }
                                                    break;
                                                }
                                                else if (MovDif.IdPoliza == movFin.IdPoliza && movFin.Tipo == "CDC")
                                                {
                                                    //EN COMPLEMENTARIA DE CLIENTES CUANDO EL PAGO ES EN PESOS, NO LLEVA COMP DE BANCOS
                                                    if (MovDif.TipoMovimiento == 1)
                                                    {
                                                        if (movFin.Referencia == MovDif.Referencia)
                                                            movFin.ImporteNacional -= MovDif.ImporteNacional;
                                                    }
                                                    else
                                                    {
                                                        if (movFin.Referencia == MovDif.Referencia)
                                                            movFin.ImporteNacional += MovDif.ImporteNacional;
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (condiciones.TipoPoliza == 4)
                                    {
                                        #region ASIGNAR DIFERENCIAS POLIZAS DIARIO Y NOTAS DE CREDITO
                                        foreach (MovimientoPolizaEntity MovDif in movimientosDiferencias)
                                        {
                                            foreach (MovimientoPolizaEntity movFin in movimientosFinales)
                                            {
                                                if (MovDif.IdPoliza == movFin.IdPoliza && movFin.Tipo == "CDC")
                                                {
                                                    if (MovDif.TipoMovimiento == 1)
                                                        movFin.ImporteNacional -= MovDif.ImporteNacional;
                                                    else
                                                        movFin.ImporteNacional += MovDif.ImporteNacional;
                                                    break;
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                else
                                {
                                    poliza.Movimientos = ListadoMovimientos;
                                }
                                #endregion
                                return generarContpaqXls(poliza, recibos[0].RazonSocial, condiciones, false);
                            }
                            catch (Exception ex)
                            {
                                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                                return "Hubo un error inesperado al momento de cargar los datos de la póliza Contpaq en Excel." + Environment.NewLine + ex.Message;
                            }
                            #endregion
                        }
                        else
                            return "Formato no implementado";
                    }
                    else
                    {
                        return "No se encontró configuración para la inmobiliaria y el tipo de poliza en cuestion.";
                    }
                }
                else
                {
                    return "No se encontraron registros para las condiciones seleccionadas y/o hubo errores al obtener registros.";
                }
                #endregion //POLIZAS DEIFERENTES DE EGRESOS
            }
            else
                return generarEgreso();
        }

        public struct CUENTA
        {
            public string NumCuenta;
            public string DescCuenta;
        }
        public CUENTA buscarCuenta(string Contrato, string Cliente, string Tipo)
        {
            string cuentaCompleta;
            CUENTA Cuenta;
            cuentaCompleta = Polizas.getCuentaByIDEntidadAndTipo(Contrato, Tipo);
            if (string.IsNullOrEmpty(cuentaCompleta))
            {
                cuentaCompleta = Polizas.getCuentaByIDEntidadAndTipo(Cliente, Tipo);
            }
            if (string.IsNullOrEmpty(cuentaCompleta))
            {
                Cuenta.NumCuenta = "NO CUENTA";
                Cuenta.DescCuenta = string.Empty;
            }
            else
            {
                Cuenta.NumCuenta = cuentaCompleta.Split('|')[0];
                Cuenta.DescCuenta = cuentaCompleta.Split('|')[1];
            }
            return Cuenta;
        }

        private MovimientoPolizaEntity getMovimientos(RecibosEntitys recibo, FormulaPolizaEntity formula, bool tomarConceptoPrincipal)
        {
            try
            {
                MovimientoPolizaEntity movimiento = new MovimientoPolizaEntity();
                movimiento.Segmento = recibo.IdentificadorConjunto;
                if (!formula.EsSubtipo)
                {
                    #region CUENTAS PRINCIPALES
                    string cuenta = string.Empty;
                    string descCuenta = string.Empty;
                    string cuentaCompleta = string.Empty;
                    #region BUSQUEDA DE NUMERO DE CUENTAS DE MOVIMIENTO
                    if (formula.Tipo == "IA" && recibo.TipoDocumento == "Z" && !string.IsNullOrEmpty(recibo.TipoContable))
                    {
                        #region TIPO IA
                        cuentaCompleta = Polizas.getCuentaByIDEntidadAndSubtipo(recibo.Contrato, recibo.TipoContable);
                        if (string.IsNullOrEmpty(cuentaCompleta))
                        {
                            cuentaCompleta = Polizas.getCuentaByIDEntidadAndSubtipo(recibo.Cliente, recibo.TipoContable);
                            if (string.IsNullOrEmpty(cuentaCompleta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                            else
                            {
                                cuenta = cuentaCompleta.Split('|')[0];
                                descCuenta = cuentaCompleta.Split('|')[1];
                                if (string.IsNullOrEmpty(cuenta))
                                {
                                    cuenta = "NO CUENTA";
                                    descCuenta = string.Empty;
                                }
                            }
                        }
                        else
                        {
                            cuenta = cuentaCompleta.Split('|')[0];
                            descCuenta = cuentaCompleta.Split('|')[1];
                            if (string.IsNullOrEmpty(cuenta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region CUENTAS VARIAS
                        cuentaCompleta = Polizas.getCuentaByIDEntidadAndTipo(recibo.Contrato, formula.Tipo);
                        if (string.IsNullOrEmpty(cuentaCompleta))
                        {
                            cuentaCompleta = Polizas.getCuentaByIDEntidadAndTipo(recibo.Cliente, formula.Tipo);
                            if (string.IsNullOrEmpty(cuentaCompleta))
                            {
                                cuentaCompleta = Polizas.getCuentaByIDEntidadAndTipo(recibo.Arrendadora, formula.Tipo);
                                if (string.IsNullOrEmpty(cuentaCompleta))
                                {
                                    cuenta = "NO CUENTA";
                                    descCuenta = string.Empty;
                                }
                                else
                                {
                                    cuenta = cuentaCompleta.Split('|')[0];
                                    descCuenta = cuentaCompleta.Split('|')[1];
                                    if (string.IsNullOrEmpty(cuenta))
                                    {
                                        cuenta = "NO CUENTA";
                                        descCuenta = string.Empty;
                                    }
                                }
                            }
                            else
                            {
                                cuenta = cuentaCompleta.Split('|')[0];
                                descCuenta = cuentaCompleta.Split('|')[1];
                                if (string.IsNullOrEmpty(cuenta))
                                {
                                    cuenta = "NO CUENTA";
                                    descCuenta = string.Empty;
                                }
                            }
                        }
                        else
                        {
                            cuenta = cuentaCompleta.Split('|')[0];
                            descCuenta = cuentaCompleta.Split('|')[1];
                            if (string.IsNullOrEmpty(cuenta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                        }
                        #endregion
                    }
                    #endregion
                    movimiento.SerieFolio = recibo.SerieFolio;
                    if (!recibo.EsConceptoLibre)
                        movimiento.Periodo = recibo.Periodo;
                    else
                        movimiento.Periodo = recibo.ConceptoRecibo;



                    movimiento.IdPagoCobranza = recibo.IdPagoCobranza;
                    movimiento.PagoTotal = recibo.PagoTotal;
                    movimiento.ClienteNombreComercial = recibo.ClienteNombreComercial;
                    movimiento.Cuenta = cuenta;
                    movimiento.CuentaDescripcion = descCuenta;
                    movimiento.ImporteExtranjero = 0;
                    movimiento.TipoMovimiento = formula.CargoAbono;
                    movimiento.Concepto = recibo.Concepto;
                    movimiento.Cliente = recibo.Cliente;
                    movimiento.Contrato = recibo.Contrato;
                    //System.Windows.Forms.MessageBox.Show("Cuenta: " + movimiento.Cuenta + " Descripción : " + movimiento.CuentaDescripcion + " Contrato: "+ movimiento.Contrato);
                    //movimiento.Concepto = recibo.SerieFolio + " " + recibo.Concepto;
                    movimiento.Referencia = recibo.SerieFolio.Replace("_", "").Replace("-", "");
                    string cadenaImporte = "";

                    //Aplica para cuando se tiene instalada la cobranza SaariPmtLocal 2.2.1.2 y polizas de ingresos (tipoPoliza==2)
                    if (cobranzaEntity.Version >= 2212 && condiciones.TipoPoliza == 2 && condiciones.FechaInicio.Date > cobranzaEntity.FechaAct.Date && recibo.IdPagoCobranza > 0)
                    {
                        decimal tipoCambio = 1.0M;
                        decimal importe = recibo.Importe;
                        decimal ivaR = recibo.IVA;
                        tipoCambio = recibo.TipoDeCambio;

                        #region SALDO A FAVOR DEL CLIENTE (GENERADO Y PAGOS)
                        if (formula.Tipo == "SFP" || formula.Tipo == "SFD")
                        {
                            if (recibo.SaldoAFavorGenerado < recibo.SaldoAFavorAplicado)
                            {
                                recibo.SaldoAFavorGenerado = 0;
                                movimiento.TipoMovimiento = 1;
                            }
                            else
                            {
                                recibo.SaldoAFavorAplicado = 0;
                                movimiento.TipoMovimiento = 2;
                            }
                        }
                        #endregion
                        bool entrar = true;
                        if (entrar)
                        {
                            // 3.3.8.1 Se agrega nuevo campo para identificar la moneda de paga en la estructura de la configuración de la póliza
                            // 23/05/2017 by UZ
                            if (condiciones.TipoPoliza == 2 && recibo.Moneda == formula.Moneda && recibo.MonedaPago == formula.MonedaPago)
                            {
                                decimal ica = recibo.TotalCargos;
                                cadenaImporte = formula.Formula.Replace("IMO", recibo.Importe.ToString()).Replace("IIV", recibo.IVA.ToString()).
                                   Replace("TC", recibo.TipoDeCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).
                                   Replace("x", "*").Replace("ICA", ica.ToString()).Replace("ID", recibo.Descuento.ToString()).
                                   Replace("ISF", recibo.SaldoAFavorGenerado.ToString()).Replace("IPS", recibo.SaldoAFavorAplicado.ToString()).
                                   Replace("PTB", "(" + recibo.Importe.ToString() + "+" + recibo.IVA.ToString() + "-" + recibo.Descuento.ToString() + "+" + recibo.SaldoAFavorGenerado.ToString() + ")").Replace("ITP", recibo.IVA.ToString());
                            }
                        }
                        else
                        {
                            // 3.3.8.1 Se agrega nuevo campo para identificar la moneda de paga en la estructura de la configuración de la póliza
                            // 23/05/2017 by UZ
                            // El código prácticamente queda obsoleto
                            #region POR PROGRAMACION CUANDO HABIA PAGOS EN DIFERENTE MONEDA
                            //PARA PAGOS EN PESOS A FACTURAS EN DOLARES POLIZAS DE INGRESOS
                            if (recibo.MonedaPago == "P" && recibo.Moneda == "D" && formula.Moneda == "D")
                            {
                                #region PAGOS EN PESOS A FACTURAS EN DOLARES POLIZA INGRESOS

                                #region IVA
                                //Para movimientos de iva
                                if (formula.Tipo == "IV" || formula.Tipo == "IVT")
                                {
                                    tipoCambio = 1.0M;
                                    //tipoCambio = recibo.TipoDeCambio;
                                    //if (recibo.TipoDocumento == "R")
                                    //{
                                    //    ivaR = recibo.IVA * recibo.TipoDeCambio;

                                    //}

                                }
                                else
                                {
                                    //tipoCambio = recibo.TipoDeCambio;
                                }
                                #endregion
                                #region CLIENTES
                                //Para movimientos de clientes y complementarias en dolares
                                if (formula.Tipo == "CDC" || formula.Tipo == "CD")
                                {
                                    importe = decimal.Round(recibo.Importe / recibo.TipoDeCambio, 4);
                                    if (recibo.TipoDocumento == "R")
                                    {

                                        //ivaR =decimal.Round(recibo.IVA * recibo.TipoDeCambio, 4);
                                        ivaR = recibo.IVA;
                                    }
                                    else
                                    {

                                        ivaR = decimal.Round(recibo.IVA / recibo.TipoDeCambio, 4);
                                    }
                                }
                                #endregion

                                //Bancos
                                #region MANEJO DE CUENTAS DE BANCOS PAGO EN PESOS A FACTURAS EN DOLARES POLIZAS INGRESOS
                                if (formula.Tipo == "BDL")
                                {

                                    cuentaCompleta = Polizas.getCuentaByIDEntidadAndTipo(recibo.Contrato, "BMN");
                                    if (string.IsNullOrEmpty(cuentaCompleta))
                                    {
                                        cuentaCompleta = Polizas.getCuentaByIDEntidadAndTipo(recibo.Cliente, "BMN");
                                    }
                                    if (string.IsNullOrEmpty(cuentaCompleta))
                                    {
                                        cuenta = "NO CUENTA";
                                        descCuenta = string.Empty;
                                    }
                                    else
                                    {
                                        cuenta = cuentaCompleta.Split('|')[0];
                                        descCuenta = cuentaCompleta.Split('|')[1];
                                    }
                                    movimiento.Cuenta = cuenta;
                                    movimiento.CuentaDescripcion = descCuenta;

                                }
                                if (formula.Tipo == "BCC")
                                {
                                    //En Pagos en pesos efectuados a facturas en dolares no se debe aplicar complementaria a bancos
                                    cadenaImporte = "0";
                                }
                                #endregion

                                cadenaImporte = formula.Formula.Replace("IMO", importe.ToString()).Replace("IIV", ivaR.ToString()).
                                  Replace("TC", tipoCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).
                                  Replace("x", "*").Replace("ICA", recibo.TotalCargos.ToString()).Replace("ID", recibo.Descuento.ToString()).
                                  Replace("ISF", recibo.SaldoAFavorGenerado.ToString()).Replace("IPS", recibo.SaldoAFavorAplicado.ToString()).
                                  Replace("PTB", recibo.PagoTotal.ToString()).Replace("ITP", recibo.IVATotalCobrado.ToString());

                                #endregion
                            }
                            else if (recibo.Moneda == recibo.MonedaPago && formula.Moneda == recibo.Moneda)
                            {
                                cadenaImporte = formula.Formula.Replace("IMO", recibo.Importe.ToString()).Replace("IIV", recibo.IVA.ToString()).
                                   Replace("TC", tipoCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).
                                   Replace("x", "*").Replace("ICA", recibo.TotalCargos.ToString()).Replace("ID", recibo.Descuento.ToString()).
                                   Replace("ISF", recibo.SaldoAFavorGenerado.ToString()).Replace("IPS", recibo.SaldoAFavorAplicado.ToString()).
                                   Replace("PTB", recibo.PagoTotal.ToString()).Replace("ITP", recibo.IVATotalCobrado.ToString());
                            }
                            else if (recibo.MonedaPago == "D" && recibo.Moneda == "P" && formula.Moneda == "P" && formula.MonedaPago == "D")
                            {
                                decimal ica = recibo.TotalCargos;

                                #region CLIENTES
                                //Para movimientos de clientes y complementarias en dolares
                                //if (formula.Tipo == "CDC" || formula.Tipo == "CD")
                                if (formula.Tipo == "CC")
                                {
                                    //importe = decimal.Round(recibo.Importe / recibo.TipoDeCambio, 4);
                                    importe = recibo.Importe;
                                    //if (recibo.TipoDocumento == "R")
                                    //{

                                    //    //ivaR =decimal.Round(recibo.IVA * recibo.TipoDeCambio, 4);
                                    //    ivaR = recibo.IVA;
                                    //}
                                    //else
                                    //{

                                    //    ivaR = decimal.Round(recibo.IVA / recibo.TipoDeCambio, 4);
                                    //}
                                }

                                #endregion
                                #region BANCOS
                                if (formula.Tipo == "BDL" || formula.Tipo == "BCC")
                                {
                                    #region CUENTA BANCO DLLS

                                    string tipo = "BDL";
                                    importe = recibo.Importe;
                                    //importe = decimal.Round(recibo.Importe / recibo.TipoDeCambio, 4);

                                    //if (recibo.TipoDocumento == "R")
                                    //{
                                    //    //ivaR =decimal.Round(recibo.IVA * recibo.TipoDeCambio, 4);
                                    //    ivaR = decimal.Round(recibo.IVA / recibo.TipoDeCambio, 4); ;
                                    //}
                                    //else
                                    //{
                                    //    ivaR = recibo.IVA ;
                                    //}

                                    #endregion
                                }

                                #endregion

                                cadenaImporte = formula.Formula.Replace("IMO", importe.ToString()).Replace("IIV", ivaR.ToString()).
                                Replace("TC", recibo.TipoDeCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).
                                Replace("x", "*").Replace("ICA", ica.ToString()).Replace("ID", recibo.Descuento.ToString()).
                                Replace("ISF", recibo.SaldoAFavorGenerado.ToString()).Replace("IPS", recibo.SaldoAFavorAplicado.ToString()).
                                Replace("PTB", "(" + recibo.Importe.ToString() + "+" + recibo.IVA.ToString() + "-" + recibo.Descuento.ToString() + "+" + recibo.SaldoAFavorGenerado.ToString() + ")").Replace("ITP", recibo.IVA.ToString());
                            }
                            #endregion
                        }

                    }
                    else
                    {
                        decimal ica = recibo.TotalCargos;

                        if (formula.TipoPoliza == 4 && formula.TipoClave == "IA")
                        {
                            ica = Polizas.getTotalCargosProrrateado(recibo.IdRecRelNC, recibo.Contrato, recibo.Importe);
                        }
                        if (formula.TipoClave == "IA")
                        {
                            //if (ica < 0)
                            //{
                            //    ica = recibo.TotalCargos * (-1);
                            //}
                        }

                        cadenaImporte = formula.Formula.Replace("IMO", recibo.Importe.ToString()).Replace("IIV", recibo.IVA.ToString()).
                            Replace("TC", recibo.TipoDeCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).
                            Replace("x", "*").Replace("ICA", ica.ToString()).Replace("ID", recibo.Descuento.ToString()).
                            Replace("ISF", recibo.SaldoAFavorGenerado.ToString()).Replace("IPS", recibo.SaldoAFavorAplicado.ToString()).
                            Replace("PTB", "(" + recibo.Importe.ToString() + "+" + recibo.IVA.ToString() + "-" + recibo.Descuento.ToString() + "+" + recibo.SaldoAFavorGenerado.ToString() + ")").Replace("ITP", recibo.IVA.ToString());

                        if (condiciones.TipoPoliza == 2 && recibo.MonedaPago != recibo.Moneda)
                            cadenaImporte = "0";
                    }


                    if (formula.TipoClave == "DIF")
                        cadenaImporte = "0";

                    movimiento.ImporteNacional = Decimal.Round(Convert.ToDecimal(evaluate(cadenaImporte)), 4);
                    //movimiento.Fecha = esDiario ? recibo.FechaEmision : recibo.FechaPagado;
                    movimiento.Fecha = recibo.FechaMovimiento;
                    movimiento.IdentificadorConjunto = Polizas.getIdentificadorConjuntoByIDContrato(recibo.Contrato);
                    movimiento.NotaCincoContrato = Polizas.getNotaCincoObservacionesByIDContrato(recibo.Contrato);
                    movimiento.Moneda = recibo.Moneda == "D" ? "DLS" : "MXN";
                    movimiento.MonedaPago = recibo.MonedaPago;
                    movimiento.NombreCliente = recibo.ClienteRazonSocial;
                    movimiento.Tipo = formula.Tipo;
                    movimiento.EsLibre = recibo.EsConceptoLibre;
                    movimiento.Periodo = recibo.Periodo;//Faltaba asignar para el reporte Global
                    movimiento.UUID = recibo.UUID;

                    if (movimiento.ImporteNacional == 0)
                        movimiento.Excluir = true;

                    #endregion

                }
                else
                {
                    #region CUENTAS DE DETALLE O SUBTIPO
                    string cuenta = string.Empty;
                    string descCuenta = string.Empty;
                    string cuentaCompleta = string.Empty;
                    cuentaCompleta = Polizas.getCuentaByIDEntidadAndSubtipo(recibo.Contrato, formula.Tipo);
                    #region OBTENER CUENTA
                    if (string.IsNullOrEmpty(cuentaCompleta))
                    {
                        cuentaCompleta = Polizas.getCuentaByIDEntidadAndSubtipo(recibo.Cliente, formula.Tipo);
                        if (string.IsNullOrEmpty(cuentaCompleta))
                        {
                            cuentaCompleta = Polizas.getCuentaByIDEntidadAndSubtipo(recibo.Arrendadora, formula.Tipo);
                            if (string.IsNullOrEmpty(cuentaCompleta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                            else
                            {
                                cuenta = cuentaCompleta.Split('|')[0];
                                descCuenta = cuentaCompleta.Split('|')[1];
                                if (string.IsNullOrEmpty(cuenta))
                                {
                                    cuenta = "NO CUENTA";
                                    descCuenta = string.Empty;
                                }
                            }
                        }
                        else
                        {
                            cuenta = cuentaCompleta.Split('|')[0];
                            descCuenta = cuentaCompleta.Split('|')[1];
                            if (string.IsNullOrEmpty(cuenta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        cuenta = cuentaCompleta.Split('|')[0];
                        descCuenta = cuentaCompleta.Split('|')[1];
                        if (string.IsNullOrEmpty(cuenta))
                        {
                            cuenta = "NO CUENTA";
                            descCuenta = string.Empty;
                        }
                    }
                    #endregion
                    CargoEntity cargo = null;
                    if (condiciones.TipoPoliza != 4)
                    {
                        if (recibo.EsEsporadico)
                        {
                            cargo = Polizas.getCargoEsporadico(recibo.Identificador, recibo.Contrato, formula.Tipo);
                            if (condiciones.TipoPoliza == 2 && cargo != null)
                            {
                                cargo.Importe += cargo.IVA;
                            }
                        }
                        else
                        {
                            cargo = Polizas.getCargo(recibo.Identificador, recibo.Contrato, formula.Tipo);
                        }
                    }
                    else
                    {
                        cargo = Polizas.getCargoProrrateado(recibo.IdRecRelNC, recibo.Contrato, formula.Tipo, recibo.Importe);
                    }
                    if (cargo != null)
                    {
                        movimiento.SerieFolio = recibo.SerieFolio;
                        if (!recibo.EsConceptoLibre)
                            movimiento.Periodo = recibo.Periodo;
                        else
                            movimiento.Periodo = recibo.ConceptoRecibo;
                        //if (recibo.EsEsporadico && condiciones.TipoPoliza != 1)
                        if (recibo.EsEsporadico)
                        {
                            movimiento.IdPagoCobranza = recibo.IdPagoCobranza;
                        }
                        else
                            movimiento.IdPagoCobranza = recibo.IdPagoCobranza;
                        movimiento.ClienteNombreComercial = recibo.ClienteNombreComercial;
                        movimiento.Cuenta = cuenta;
                        movimiento.CuentaDescripcion = descCuenta;
                        movimiento.ImporteExtranjero = 0;
                        movimiento.TipoMovimiento = formula.CargoAbono;
                        if (tomarConceptoPrincipal)
                            movimiento.Concepto = recibo.Concepto;
                        else
                            movimiento.Concepto = recibo.SerieFolio + " " + cargo.Concepto;
                        movimiento.Referencia = recibo.SerieFolio.Replace("_", "").Replace("-", "");
                        string cadenaImporte = "";
                        //if(recibo.EsEsporadico)
                        //    cadenaImporte = formula.Formula.Replace("IMO", cargo.Importe.ToString()).Replace("IIV", recibo.IVA.ToString()).Replace("TC", recibo.TipoDeCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).Replace("x", "*").Replace("ICA", recibo.TotalCargos.ToString());
                        //else
                        cadenaImporte = formula.Formula.Replace("IMO", cargo.Importe.ToString()).Replace("IIV", recibo.IVA.ToString()).Replace("TC", recibo.TipoDeCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).Replace("x", "*").Replace("ICA", recibo.TotalCargos.ToString());
                        movimiento.ImporteNacional = Decimal.Round(Convert.ToDecimal(evaluate(cadenaImporte)), 4);
                        //movimiento.ImporteNacional = Convert.ToDecimal(evaluate(cadenaImporte));
                        movimiento.Fecha = recibo.FechaMovimiento;
                        movimiento.IdentificadorConjunto = Polizas.getIdentificadorConjuntoByIDContrato(recibo.Contrato);
                        movimiento.NotaCincoContrato = Polizas.getNotaCincoObservacionesByIDContrato(recibo.Contrato);
                        movimiento.Moneda = cargo.Moneda == "D" ? "DLS" : "MXN";
                        movimiento.NombreCliente = recibo.ClienteRazonSocial;
                        movimiento.Tipo = formula.Tipo;
                        movimiento.EsLibre = recibo.EsConceptoLibre;
                        movimiento.UUID = recibo.UUID;
                    }
                    else
                    {
                        movimiento.Excluir = true;
                    }
                    #endregion
                }
                return movimiento;
            }
            catch //(Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                return null;
            }
        }

        private MovimientoPolizaEntity getMovimientos(RecibosEntitys recibo, FormulaPolizaEntity formula, bool tomarConceptoPrincipal, string monedaDeRecibo, string monedaDePago)
        {
            try
            {
                MovimientoPolizaEntity movimiento = new MovimientoPolizaEntity();
                movimiento.Segmento = recibo.IdentificadorConjunto;
                if (!formula.EsSubtipo)
                {
                    #region CUENTAS PRINCIPALES
                    string cuenta = string.Empty;
                    string descCuenta = string.Empty;
                    string cuentaCompleta = string.Empty;
                    #region BUSQUEDA DE NUMERO DE CUENTAS DE MOVIMIENTO
                    if (formula.Tipo == "IA" && recibo.TipoDocumento == "Z" && !string.IsNullOrEmpty(recibo.TipoContable))
                    {
                        #region TIPO IA
                        cuentaCompleta = Polizas.getCuentaByIDEntidadAndSubtipo(recibo.Contrato, recibo.TipoContable);
                        if (string.IsNullOrEmpty(cuentaCompleta))
                        {
                            cuentaCompleta = Polizas.getCuentaByIDEntidadAndSubtipo(recibo.Cliente, recibo.TipoContable);
                            if (string.IsNullOrEmpty(cuentaCompleta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                            else
                            {
                                cuenta = cuentaCompleta.Split('|')[0];
                                descCuenta = cuentaCompleta.Split('|')[1];
                                if (string.IsNullOrEmpty(cuenta))
                                {
                                    cuenta = "NO CUENTA";
                                    descCuenta = string.Empty;
                                }
                            }
                        }
                        else
                        {
                            cuenta = cuentaCompleta.Split('|')[0];
                            descCuenta = cuentaCompleta.Split('|')[1];
                            if (string.IsNullOrEmpty(cuenta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region CUENTAS VARIAS
                        cuentaCompleta = Polizas.getCuentaByIDEntidadAndTipo(recibo.Contrato, formula.Tipo);
                        if (string.IsNullOrEmpty(cuentaCompleta))
                        {
                            cuentaCompleta = Polizas.getCuentaByIDEntidadAndTipo(recibo.Cliente, formula.Tipo);
                            if (string.IsNullOrEmpty(cuentaCompleta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                            else
                            {
                                cuenta = cuentaCompleta.Split('|')[0];
                                descCuenta = cuentaCompleta.Split('|')[1];
                                if (string.IsNullOrEmpty(cuenta))
                                {
                                    cuenta = "NO CUENTA";
                                    descCuenta = string.Empty;
                                }
                            }
                        }
                        else
                        {
                            cuenta = cuentaCompleta.Split('|')[0];
                            descCuenta = cuentaCompleta.Split('|')[1];
                            if (string.IsNullOrEmpty(cuenta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                        }
                        #endregion
                    }
                    #endregion
                    movimiento.SerieFolio = recibo.SerieFolio;
                    if (!recibo.EsConceptoLibre)
                        movimiento.Periodo = recibo.Periodo;
                    else
                        movimiento.Periodo = recibo.ConceptoRecibo;


                    movimiento.IdPagoCobranza = recibo.IdPagoCobranza;
                    movimiento.PagoTotal = recibo.PagoTotal;
                    movimiento.ClienteNombreComercial = recibo.ClienteNombreComercial;
                    movimiento.Cuenta = cuenta;
                    movimiento.CuentaDescripcion = descCuenta;
                    movimiento.ImporteExtranjero = 0;
                    movimiento.TipoMovimiento = formula.CargoAbono;
                    movimiento.Concepto = recibo.Concepto;
                    movimiento.Cliente = recibo.Cliente;
                    movimiento.Contrato = recibo.Contrato;
                    //System.Windows.Forms.MessageBox.Show("Cuenta: " + movimiento.Cuenta + " Descripción : " + movimiento.CuentaDescripcion + " Contrato: "+ movimiento.Contrato);
                    //movimiento.Concepto = recibo.SerieFolio + " " + recibo.Concepto;
                    movimiento.Referencia = recibo.SerieFolio.Replace("_", "").Replace("-", "");
                    string cadenaImporte = "";

                    //Aplica para cuando se tiene instalada la cobranza SaariPmtLocal 2.2.1.2 y polizas de ingresos (tipoPoliza==2)
                    if (cobranzaEntity.Version >= 2212 && condiciones.TipoPoliza == 2 && condiciones.FechaInicio.Date > cobranzaEntity.FechaAct.Date && recibo.IdPagoCobranza > 0)
                    {
                        decimal tipoCambio = 1.0M;
                        decimal importe = recibo.Importe;
                        decimal ivaR = recibo.IVA;
                        tipoCambio = recibo.TipoDeCambio;

                        #region SALDO A FAVOR DEL CLIENTE (GENERADO Y PAGOS)
                        if (formula.Tipo == "SFP" || formula.Tipo == "SFD")
                        {
                            if (recibo.SaldoAFavorGenerado < recibo.SaldoAFavorAplicado)
                            {
                                recibo.SaldoAFavorGenerado = 0;
                                movimiento.TipoMovimiento = 1;
                            }
                            else
                            {
                                recibo.SaldoAFavorAplicado = 0;
                                movimiento.TipoMovimiento = 2;
                            }
                        }
                        #endregion
                        //PARA PAGOS EN PESOS A FACTURAS EN DOLARES POLIZAS DE INGRESOS
                        if (recibo.MonedaPago == "P" && recibo.Moneda == "D")
                        {
                            #region PAGOS EN PESOS A FACTURAS EN DOLARES POLIZA INGRESOS

                            #region IVA
                            //Para movimientos de iva
                            if (formula.Tipo == "IV" || formula.Tipo == "IVT")
                            {
                                tipoCambio = 1.0M;
                                //if (recibo.TipoDocumento == "R")
                                //{
                                //    ivaR = recibo.IVA * recibo.TipoDeCambio;

                                //}

                            }
                            else
                            {
                                tipoCambio = recibo.TipoDeCambio;
                            }
                            #endregion
                            #region CLIENTES
                            //Para movimientos de clientes y complementarias en dolares
                            if (formula.Tipo == "CDC" || formula.Tipo == "CD")
                            {
                                importe = decimal.Round(recibo.Importe / recibo.TipoDeCambio, 4);
                                if (recibo.TipoDocumento == "R")
                                {

                                    //ivaR =decimal.Round(recibo.IVA * recibo.TipoDeCambio, 4);
                                    ivaR = recibo.IVA;
                                }
                                else
                                {

                                    ivaR = decimal.Round(recibo.IVA / recibo.TipoDeCambio, 4);
                                }
                            }
                            #endregion

                            cadenaImporte = formula.Formula.Replace("IMO", importe.ToString()).Replace("IIV", ivaR.ToString()).
                               Replace("TC", tipoCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).
                               Replace("x", "*").Replace("ICA", recibo.TotalCargos.ToString()).Replace("ID", recibo.Descuento.ToString()).
                               Replace("ISF", recibo.SaldoAFavorGenerado.ToString()).Replace("IPS", recibo.SaldoAFavorAplicado.ToString()).
                               Replace("PTB", recibo.PagoTotal.ToString()).Replace("ITP", recibo.IVATotalCobrado.ToString());

                            //Bancos
                            #region MANEJO DE CUENTAS DE BANCOS PAGO EN PESOS A FACTURAS EN DOLARES POLIZAS INGRESOS
                            if (formula.Tipo == "BDL")
                            {

                                cuentaCompleta = Polizas.getCuentaByIDEntidadAndTipo(recibo.Contrato, "BMN");
                                if (string.IsNullOrEmpty(cuentaCompleta))
                                {
                                    cuentaCompleta = Polizas.getCuentaByIDEntidadAndTipo(recibo.Cliente, "BMN");
                                }
                                if (string.IsNullOrEmpty(cuentaCompleta))
                                {
                                    cuenta = "NO CUENTA";
                                    descCuenta = string.Empty;
                                }
                                else
                                {
                                    cuenta = cuentaCompleta.Split('|')[0];
                                    descCuenta = cuentaCompleta.Split('|')[1];
                                }
                                movimiento.Cuenta = cuenta;
                                movimiento.CuentaDescripcion = descCuenta;

                            }
                            if (formula.Tipo == "BCC")
                            {
                                //En Pagos en pesos efectuados a facturas en dolares no se debe aplicar complementaria a bancos
                                cadenaImporte = "0";
                            }
                            #endregion

                            #endregion
                        }
                        else if (recibo.Moneda == recibo.MonedaPago)
                        {
                            cadenaImporte = formula.Formula.Replace("IMO", recibo.Importe.ToString()).Replace("IIV", recibo.IVA.ToString()).
                               Replace("TC", tipoCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).
                               Replace("x", "*").Replace("ICA", recibo.TotalCargos.ToString()).Replace("ID", recibo.Descuento.ToString()).
                               Replace("ISF", recibo.SaldoAFavorGenerado.ToString()).Replace("IPS", recibo.SaldoAFavorAplicado.ToString()).
                               Replace("PTB", recibo.PagoTotal.ToString()).Replace("ITP", recibo.IVATotalCobrado.ToString());
                        }
                        //else if (recibo.MonedaPago == "D" && recibo.Moneda == "P")
                        //{
                        //    decimal ica = recibo.TotalCargos;

                        //    #region CLIENTES
                        //    //Para movimientos de clientes y complementarias en dolares
                        //    if (formula.Tipo == "CDC" || formula.Tipo == "CD")
                        //    {
                        //        importe = decimal.Round(recibo.Importe / recibo.TipoDeCambio, 4);
                        //        if (recibo.TipoDocumento == "R")
                        //        {

                        //            //ivaR =decimal.Round(recibo.IVA * recibo.TipoDeCambio, 4);
                        //            ivaR = recibo.IVA;
                        //        }
                        //        else
                        //        {

                        //            ivaR = decimal.Round(recibo.IVA / recibo.TipoDeCambio, 4);
                        //        }
                        //    }

                        //    #endregion


                        //    cadenaImporte = formula.Formula.Replace("IMO", recibo.Importe.ToString()).Replace("IIV", recibo.IVA.ToString()).
                        //    Replace("TC", recibo.TipoDeCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).
                        //    Replace("x", "*").Replace("ICA", ica.ToString()).Replace("ID", recibo.Descuento.ToString()).
                        //    Replace("ISF", recibo.SaldoAFavorGenerado.ToString()).Replace("IPS", recibo.SaldoAFavorAplicado.ToString()).
                        //    Replace("PTB", "(" + recibo.Importe.ToString() + "+" + recibo.IVA.ToString() + "-" + recibo.Descuento.ToString() + "+" + recibo.SaldoAFavorGenerado.ToString() + ")").Replace("ITP", recibo.IVA.ToString());



                        //}

                    }
                    else
                    {

                        decimal ica = recibo.TotalCargos;

                        if (formula.TipoPoliza == 4 && formula.TipoClave == "IA")
                        {
                            ica = Polizas.getTotalCargosProrrateado(recibo.IdRecRelNC, recibo.Contrato, recibo.Importe);
                        }
                        if (formula.TipoClave == "IA")
                        {
                            //if (ica < 0)
                            //{
                            //    ica = recibo.TotalCargos * (-1);
                            //}
                        }

                        cadenaImporte = formula.Formula.Replace("IMO", recibo.Importe.ToString()).Replace("IIV", recibo.IVA.ToString()).
                            Replace("TC", recibo.TipoDeCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).
                            Replace("x", "*").Replace("ICA", ica.ToString()).Replace("ID", recibo.Descuento.ToString()).
                            Replace("ISF", recibo.SaldoAFavorGenerado.ToString()).Replace("IPS", recibo.SaldoAFavorAplicado.ToString()).
                            Replace("PTB", "(" + recibo.Importe.ToString() + "+" + recibo.IVA.ToString() + "-" + recibo.Descuento.ToString() + "+" + recibo.SaldoAFavorGenerado.ToString() + ")").Replace("ITP", recibo.IVA.ToString());

                        if (condiciones.TipoPoliza == 2 && recibo.MonedaPago != recibo.Moneda)
                            cadenaImporte = "0";
                    }

                    ////Cambio a partir de la version 3.2.1.1
                    //if(versionODBC>=8645 && formula.Tipo=="BMN")
                    //    //cadenaImporte = formula.Formula.Replace("IMO", recibo.PagoTotal.ToString()).Replace("IIV", recibo.IVA.ToString()).Replace("TC", recibo.TipoDeCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).Replace("x", "*").Replace("ICA", recibo.TotalCargos.ToString()).Replace("ID", recibo.Descuento.ToString()).Replace("ISF", recibo.SaldoAFavorGenerado.ToString()).Replace("IPS", recibo.SaldoAFavorAplicado.ToString());
                    //    cadenaImporte=recibo.PagoTotal.ToString();
                    //else
                    //    cadenaImporte = formula.Formula.Replace("IMO", recibo.Importe.ToString()).Replace("IIV", recibo.IVA.ToString()).Replace("TC", recibo.TipoDeCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).Replace("x", "*").Replace("ICA", recibo.TotalCargos.ToString()).Replace("ID", recibo.Descuento.ToString()).Replace("ISF", recibo.SaldoAFavorGenerado.ToString()).Replace("IPS", recibo.SaldoAFavorAplicado.ToString());
                    //if (versionODBC >= 8645 && formula.Tipo == "IIV")
                    //    cadenaImporte = recibo.IVATotalCobrado.ToString();
                    //movimiento.ImporteNacional = Decimal.Round(Convert.ToDecimal(evaluate(cadenaImporte)), 4, MidpointRounding.AwayFromZero);
                    if (formula.TipoClave == "DIF")
                        cadenaImporte = "0";

                    movimiento.ImporteNacional = Decimal.Round(Convert.ToDecimal(evaluate(cadenaImporte)), 4);
                    //movimiento.Fecha = esDiario ? recibo.FechaEmision : recibo.FechaPagado;
                    movimiento.Fecha = recibo.FechaMovimiento;
                    movimiento.IdentificadorConjunto = Polizas.getIdentificadorConjuntoByIDContrato(recibo.Contrato);
                    movimiento.NotaCincoContrato = Polizas.getNotaCincoObservacionesByIDContrato(recibo.Contrato);
                    movimiento.Moneda = recibo.Moneda == "D" ? "DLS" : "MXN";
                    movimiento.MonedaPago = recibo.MonedaPago;
                    movimiento.NombreCliente = recibo.ClienteRazonSocial;
                    movimiento.Tipo = formula.Tipo;
                    movimiento.EsLibre = recibo.EsConceptoLibre;
                    movimiento.Periodo = recibo.Periodo;//Faltaba asignar para el reporte Global
                    movimiento.UUID = recibo.UUID;

                    //if (versionOBDC >= 8645 && condiciones.TipoPoliza == 2 && condiciones.FechaInicio >= fechaActPol)//POLIZA DE INGRESO
                    //{
                    //    if (formula.Tipo != "CC" && formula.Tipo != "CDC")
                    //        movimiento.UUID = ""; 
                    //}
                    if (movimiento.ImporteNacional == 0)
                        movimiento.Excluir = true;
                    #endregion
                }
                else
                {
                    #region CUENTAS DE DETALLE O SUBTIPO
                    string cuenta = string.Empty;
                    string descCuenta = string.Empty;
                    string cuentaCompleta = string.Empty;
                    cuentaCompleta = Polizas.getCuentaByIDEntidadAndSubtipo(recibo.Contrato, formula.Tipo);
                    #region OBTENER CUENTA
                    if (string.IsNullOrEmpty(cuentaCompleta))
                    {
                        cuentaCompleta = Polizas.getCuentaByIDEntidadAndSubtipo(recibo.Cliente, formula.Tipo);
                        if (string.IsNullOrEmpty(cuentaCompleta))
                        {
                            cuentaCompleta = Polizas.getCuentaByIDEntidadAndSubtipo(recibo.Arrendadora, formula.Tipo);
                            if (string.IsNullOrEmpty(cuentaCompleta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                            else
                            {
                                cuenta = cuentaCompleta.Split('|')[0];
                                descCuenta = cuentaCompleta.Split('|')[1];
                                if (string.IsNullOrEmpty(cuenta))
                                {
                                    cuenta = "NO CUENTA";
                                    descCuenta = string.Empty;
                                }
                            }
                        }
                        else
                        {
                            cuenta = cuentaCompleta.Split('|')[0];
                            descCuenta = cuentaCompleta.Split('|')[1];
                            if (string.IsNullOrEmpty(cuenta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        cuenta = cuentaCompleta.Split('|')[0];
                        descCuenta = cuentaCompleta.Split('|')[1];
                        if (string.IsNullOrEmpty(cuenta))
                        {
                            cuenta = "NO CUENTA";
                            descCuenta = string.Empty;
                        }
                    }
                    #endregion
                    CargoEntity cargo = null;
                    if (condiciones.TipoPoliza != 4)
                    {
                        if (recibo.EsEsporadico)
                        {
                            cargo = Polizas.getCargoEsporadico(recibo.Identificador, recibo.Contrato, formula.Tipo);
                            if (condiciones.TipoPoliza == 2 && cargo != null)
                            {
                                cargo.Importe += cargo.IVA;
                            }
                        }
                        else
                        {
                            cargo = Polizas.getCargo(recibo.Identificador, recibo.Contrato, formula.Tipo);
                        }
                    }
                    else
                    {
                        cargo = Polizas.getCargoProrrateado(recibo.IdRecRelNC, recibo.Contrato, formula.Tipo, recibo.Importe);
                    }
                    if (cargo != null)
                    {
                        movimiento.SerieFolio = recibo.SerieFolio;
                        if (!recibo.EsConceptoLibre)
                            movimiento.Periodo = recibo.Periodo;
                        else
                            movimiento.Periodo = recibo.ConceptoRecibo;
                        //if (recibo.EsEsporadico && condiciones.TipoPoliza != 1)
                        if (recibo.EsEsporadico)
                        {
                            movimiento.IdPagoCobranza = recibo.IdPagoCobranza;
                        }
                        else
                            movimiento.IdPagoCobranza = recibo.IdPagoCobranza;
                        movimiento.ClienteNombreComercial = recibo.ClienteNombreComercial;
                        movimiento.Cuenta = cuenta;
                        movimiento.CuentaDescripcion = descCuenta;
                        movimiento.ImporteExtranjero = 0;
                        movimiento.TipoMovimiento = formula.CargoAbono;
                        if (tomarConceptoPrincipal)
                            movimiento.Concepto = recibo.Concepto;
                        else
                            movimiento.Concepto = recibo.SerieFolio + " " + cargo.Concepto;
                        movimiento.Referencia = recibo.SerieFolio.Replace("_", "").Replace("-", "");
                        string cadenaImporte = "";
                        //if(recibo.EsEsporadico)
                        //    cadenaImporte = formula.Formula.Replace("IMO", cargo.Importe.ToString()).Replace("IIV", recibo.IVA.ToString()).Replace("TC", recibo.TipoDeCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).Replace("x", "*").Replace("ICA", recibo.TotalCargos.ToString());
                        //else
                        cadenaImporte = formula.Formula.Replace("IMO", cargo.Importe.ToString()).Replace("IIV", recibo.IVA.ToString()).Replace("TC", recibo.TipoDeCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).Replace("x", "*").Replace("ICA", recibo.TotalCargos.ToString());
                        movimiento.ImporteNacional = Decimal.Round(Convert.ToDecimal(evaluate(cadenaImporte)), 4);
                        //movimiento.ImporteNacional = Convert.ToDecimal(evaluate(cadenaImporte));
                        movimiento.Fecha = recibo.FechaMovimiento;
                        movimiento.IdentificadorConjunto = Polizas.getIdentificadorConjuntoByIDContrato(recibo.Contrato);
                        movimiento.NotaCincoContrato = Polizas.getNotaCincoObservacionesByIDContrato(recibo.Contrato);
                        movimiento.Moneda = cargo.Moneda == "D" ? "DLS" : "MXN";
                        movimiento.NombreCliente = recibo.ClienteRazonSocial;
                        movimiento.Tipo = formula.Tipo;
                        movimiento.EsLibre = recibo.EsConceptoLibre;
                        movimiento.UUID = recibo.UUID;
                    }
                    else
                    {
                        movimiento.Excluir = true;
                    }
                    #endregion
                }
                return movimiento;
            }
            catch //(Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                return null;
            }
        }

        private MovimientoPolizaEntity getMovimientos(EgresoEntity egreso, FormulaPolizaEntity formula)
        {
            try
            {
                MovimientoPolizaEntity movimiento = new MovimientoPolizaEntity();
                string cuenta = string.Empty;
                string descCuenta = string.Empty;
                if (formula.TipoClave == "BPE")
                {
                    if (string.IsNullOrEmpty(egreso.Cuenta))
                        cuenta = "NO CUENTA";
                    else
                        cuenta = egreso.Cuenta;
                    if (string.IsNullOrEmpty(egreso.NombreCuenta))
                        descCuenta = "NO DESCRIPCION";
                    else
                        descCuenta = egreso.NombreCuenta;
                }
                else if (formula.TipoClave == "IVPE")
                {
                    if (string.IsNullOrEmpty(egreso.CuentaIVA))
                        cuenta = "NO CUENTA";
                    else
                        cuenta = egreso.CuentaIVA;
                    if (string.IsNullOrEmpty(egreso.NombreCuentaIVA))
                        descCuenta = "NO DESCRIPCION";
                    else
                        descCuenta = egreso.NombreCuentaIVA;
                }
                else if (formula.TipoClave == "PPE")
                {
                    if (string.IsNullOrEmpty(egreso.CuentaProveedores))
                        cuenta = "NO CUENTA";
                    else
                        cuenta = egreso.CuentaProveedores;
                    if (string.IsNullOrEmpty(egreso.NombreCuentaProveedores))
                        descCuenta = "NO DESCRIPCION";
                    else
                        descCuenta = egreso.NombreCuentaProveedores;
                }
                else if (formula.TipoClave == "IVPR") //IVA PROVISIONADO
                {
                    if (string.IsNullOrEmpty(egreso.CuentaIVA))
                        cuenta = "NO CUENTA";
                    else
                        cuenta = egreso.CuentaIVA;
                    if (string.IsNullOrEmpty(egreso.DescripCuentaIVA))
                        descCuenta = "NO DESCRIPCION";
                    else
                        descCuenta = egreso.DescripCuentaIVA;
                }
                else if (formula.TipoClave == "IVAC") //IVA ACREDITADO
                {
                    if (string.IsNullOrEmpty(egreso.CuentaIVAAcred))
                        cuenta = "NO CUENTA";
                    else
                        cuenta = egreso.CuentaIVAAcred;
                }
                else if (formula.TipoClave == "RIV") //RETENCION DE IVA
                {
                    if (string.IsNullOrEmpty(egreso.CuentaRetIVA))
                        cuenta = "NO CUENTA";
                    else
                        cuenta = egreso.CuentaRetIVA;
                    if (string.IsNullOrEmpty(egreso.DescripCuentaRetIVA))
                        descCuenta = "NO DESCRIPCION";
                    else
                        descCuenta = egreso.DescripCuentaRetIVA;
                }
                else if (formula.TipoClave == "RISR") //RETENCION DE ISR
                {
                    if (string.IsNullOrEmpty(egreso.CuentaRetISR))
                        cuenta = "NO CUENTA";
                    else
                        cuenta = egreso.CuentaRetISR;
                    if (string.IsNullOrEmpty(egreso.DescripCuentaRetISR))
                        descCuenta = "NO DESCRIPCION";
                    else
                        descCuenta = egreso.DescripCuentaRetISR;
                }
                movimiento.Cuenta = cuenta;
                movimiento.CuentaDescripcion = descCuenta;
                movimiento.ImporteExtranjero = 0;
                movimiento.TipoMovimiento = formula.CargoAbono;
                movimiento.Concepto = egreso.Concepto;
                movimiento.Segmento = egreso.IdentificadorConjunto;
                string cadenaImporte = string.Empty;
                cadenaImporte = formula.Formula.Replace("ITI", egreso.ImporteIVA.ToString()).Replace("IT", egreso.Importe.ToString()).
                    Replace("IVAC", egreso.ImporteIVAAcred.ToString()).Replace("RIV", egreso.ImporteRetIVA.ToString()).
                    Replace("RISR", egreso.ImporteRetISR.ToString());
                movimiento.ImporteNacional = Convert.ToDecimal(evaluate(cadenaImporte));
                movimiento.Fecha = egreso.Fecha;
                if (movimiento.ImporteNacional <= 0)
                    movimiento.Excluir = true;
                return movimiento;
            }
            catch
            {
                return null;
            }
        }

        private MovimientoPolizaEntity getMovimientos(RegistroEgresoEntity registro, FormulaPolizaEntity formula)
        {
            try
            {
                MovimientoPolizaEntity movimiento = new MovimientoPolizaEntity();
                movimiento.Cuenta = registro.NumCuenta;
                movimiento.CuentaDescripcion = registro.NombreCuenta;
                movimiento.ImporteExtranjero = 0;
                movimiento.TipoMovimiento = formula.CargoAbono;
                movimiento.Concepto = registro.Concepto;
                movimiento.ImporteNacional = registro.Importe;
                movimiento.Segmento = registro.IdentificadorConjunto;
                if (movimiento.ImporteNacional <= 0)
                    movimiento.Excluir = true;
                return movimiento;
            }
            catch
            {
                return null;
            }
        }

        private MovimientoExcelEntity getExcelMovimientos(RecibosEntitys recibo, FormulaPolizaEntity formula)
        {
            try
            {
                MovimientoExcelEntity movimiento = new MovimientoExcelEntity();
                if (!formula.EsSubtipo)
                {
                    ContadorMovimientos++;
                    string cuenta = string.Empty;
                    string descCuenta = string.Empty;
                    string cuentaCompleta = string.Empty;
                    cuentaCompleta = Polizas.getCuentaByIDEntidadAndTipo(recibo.Cliente, formula.Tipo);
                    if (string.IsNullOrEmpty(cuentaCompleta))
                    {
                        cuentaCompleta = Polizas.getCuentaByIDEntidadAndTipo(recibo.Contrato, formula.Tipo);
                        if (string.IsNullOrEmpty(cuentaCompleta))
                        {
                            cuenta = "NO CUENTA";
                            descCuenta = string.Empty;
                        }
                        else
                        {
                            cuenta = cuentaCompleta.Split('|')[0];
                            descCuenta = cuentaCompleta.Split('|')[1];
                            if (string.IsNullOrEmpty(cuenta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        cuenta = cuentaCompleta.Split('|')[0];
                        descCuenta = cuentaCompleta.Split('|')[1];
                        if (string.IsNullOrEmpty(cuenta))
                        {
                            cuenta = "NO CUENTA";
                            descCuenta = string.Empty;
                        }
                    }
                    movimiento.Consecutivo = ContadorMovimientos;
                    movimiento.Contador = recibo.Contador;
                    movimiento.FechaMovimiento = recibo.FechaMovimiento;
                    movimiento.Cuenta = cuenta;
                    movimiento.DescripcionCuenta = descCuenta;
                    string cadenaImporte = formula.Formula.Replace("IMO", recibo.Importe.ToString()).Replace("IIV", recibo.IVA.ToString()).Replace("TC", recibo.TipoDeCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).Replace("x", "*").Replace("ICA", recibo.TotalCargos.ToString());
                    if (formula.CargoAbono == 1)
                    {
                        movimiento.Cargo = Convert.ToDecimal(evaluate(cadenaImporte));
                        movimiento.Abono = 0;
                    }
                    else
                    {
                        movimiento.Abono = Convert.ToDecimal(evaluate(cadenaImporte));
                        movimiento.Cargo = 0;
                    }
                    movimiento.Descripcion = recibo.Concepto;
                    movimiento.NumeroRecibo = recibo.NumeroRecibo;
                    movimiento.SerieFolio = recibo.SerieFolio;
                    movimiento.Estatus = recibo.Estatus;
                    movimiento.ClienteRazonSocial = recibo.ClienteRazonSocial;
                    movimiento.ClienteRFC = recibo.ClienteRFC;
                    movimiento.InmobiliariaRazonSocial = recibo.RazonSocial;
                    movimiento.InmobiliariaRFC = recibo.RFC;
                    movimiento.Conjunto = recibo.Conjunto;
                    movimiento.Inmueble = recibo.Inmueble;
                    movimiento.Tipo = formula.Tipo;
                }
                else
                {
                    CargoEntity cargo = Polizas.getCargo(recibo.Identificador, recibo.Contrato, formula.Tipo);
                    if (cargo != null)
                    {
                        ContadorMovimientos++;
                        string cuenta = string.Empty;
                        string descCuenta = string.Empty;
                        string cuentaCompleta = string.Empty;
                        cuentaCompleta = Polizas.getCuentaByIDEntidadAndSubtipo(recibo.Cliente, formula.Tipo);
                        if (string.IsNullOrEmpty(cuentaCompleta))
                        {
                            cuentaCompleta = Polizas.getCuentaByIDEntidadAndSubtipo(recibo.Contrato, formula.Tipo);
                            if (string.IsNullOrEmpty(cuentaCompleta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                            else
                            {
                                cuenta = cuentaCompleta.Split('|')[0];
                                descCuenta = cuentaCompleta.Split('|')[1];
                                if (string.IsNullOrEmpty(cuenta))
                                {
                                    cuenta = "NO CUENTA";
                                    descCuenta = string.Empty;
                                }
                            }
                        }
                        else
                        {
                            cuenta = cuentaCompleta.Split('|')[0];
                            descCuenta = cuentaCompleta.Split('|')[1];
                            if (string.IsNullOrEmpty(cuenta))
                            {
                                cuenta = "NO CUENTA";
                                descCuenta = string.Empty;
                            }
                        }
                        movimiento.Consecutivo = ContadorMovimientos;
                        movimiento.Contador = recibo.Contador;
                        movimiento.FechaMovimiento = recibo.FechaMovimiento;
                        movimiento.Cuenta = cuenta;
                        movimiento.DescripcionCuenta = descCuenta;
                        string cadenaImporte = formula.Formula.Replace("IMO", cargo.Importe.ToString()).Replace("IIV", recibo.IVA.ToString()).Replace("TC", recibo.TipoDeCambio.ToString()).Replace("RIS", recibo.RetencionISR.ToString()).Replace("RIV", recibo.RetencionIVA.ToString()).Replace("x", "*").Replace("ICA", recibo.TotalCargos.ToString());
                        if (formula.CargoAbono == 1)
                        {
                            movimiento.Cargo = Convert.ToDecimal(evaluate(cadenaImporte));
                            movimiento.Abono = 0;
                        }
                        else
                        {
                            movimiento.Abono = Convert.ToDecimal(evaluate(cadenaImporte));
                            movimiento.Cargo = 0;
                        }
                        movimiento.Descripcion = recibo.SerieFolio + " " + cargo.Concepto;
                        movimiento.NumeroRecibo = recibo.NumeroRecibo;
                        movimiento.SerieFolio = recibo.SerieFolio;
                        movimiento.Estatus = recibo.Estatus;
                        movimiento.ClienteRazonSocial = recibo.ClienteRazonSocial;
                        movimiento.ClienteRFC = recibo.ClienteRFC;
                        movimiento.InmobiliariaRazonSocial = recibo.RazonSocial;
                        movimiento.InmobiliariaRFC = recibo.RFC;
                        movimiento.Conjunto = recibo.Conjunto;
                        movimiento.Inmueble = recibo.Inmueble;
                        movimiento.Tipo = formula.Tipo;
                    }
                    else
                    {
                        movimiento.Excluir = true;
                    }
                }
                return movimiento;
            }
            catch
            {
                return null;
            }
        }

        private double evaluate(string expression)
        {
            var loDataTable = new System.Data.DataTable();
            var loDataColumn = new System.Data.DataColumn("Eval", typeof(double), expression);
            loDataTable.Columns.Add(loDataColumn);
            loDataTable.Rows.Add(0);
            return (double)(loDataTable.Rows[0]["Eval"]);
        }

        private string generarTxt(PolizaEntity poliza, string razonSocial, CondicionesEntity condiciones, bool esEgreso)
        {
            try
            {
                bool cancelados = condiciones.TipoPoliza == 3;
                bool notasCredito = condiciones.TipoPoliza == 4;
                string path = Properties.Settings.Default.RutaRepositorio.EndsWith(@"\") ? Properties.Settings.Default.RutaRepositorio + @"Polizas\Contpaq\" : Properties.Settings.Default.RutaRepositorio + @"\Polizas\Contpaq\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string tipoPoliza = string.Empty;
                if (poliza.Encabezado.TipoContpaq == 3)
                {
                    if (cancelados)
                        tipoPoliza = "Cancelados_";
                    else if (notasCredito)
                        tipoPoliza = "NotasDeCredito_";
                    else if (!esEgreso)
                        tipoPoliza = "Diario_";
                    else
                        tipoPoliza = "Provision_";
                }
                else if (poliza.Encabezado.TipoContpaq == 1)
                    tipoPoliza = "Ingreso_";
                else
                    tipoPoliza = "Egreso_";
                string fileName = path + tipoPoliza + poliza.Encabezado.Fecha.ToString("yyyy-MM-dd") + " (" + razonSocial + ").txt";
                this.fileName = fileName;
                long num = 0;
                long numInicial = 0;
                string encabezado = string.Empty, encabezadoAfterNumPol = string.Empty;
                if (string.IsNullOrEmpty(condiciones.NumeroPoliza))
                {
                    if (!condiciones.AfectarSaldos)
                        encabezado = "P " + poliza.Encabezado.Fecha.ToString("yyyyMMdd") + " " + poliza.Encabezado.TipoContpaq + "          2 ";
                    else
                        encabezado = "P " + poliza.Encabezado.Fecha.ToString("yyyyMMdd") + " " + poliza.Encabezado.TipoContpaq + "          1 ";
                }
                else
                {
                    string numPol = condiciones.NumeroPoliza.Length > 8 ? condiciones.NumeroPoliza.Substring(0, 8) : condiciones.NumeroPoliza;
                    while (numPol.Length < 8)
                    {
                        numPol = (char)32 + numPol;
                    }
                    if (!condiciones.AfectarSaldos)
                        encabezado = "P " + poliza.Encabezado.Fecha.ToString("yyyyMMdd") + " " + poliza.Encabezado.TipoContpaq + " " + numPol + "] 2 ";
                    else
                        encabezado = "P " + poliza.Encabezado.Fecha.ToString("yyyyMMdd") + " " + poliza.Encabezado.TipoContpaq + " " + numPol + "] 1 ";
                    num = Convert.ToInt64(numPol);
                    numInicial = num;
                }
                encabezadoAfterNumPol = encabezado;
                if (string.IsNullOrEmpty(condiciones.ConceptoPoliza))
                {
                    for (int i = 1; i <= 106; i++)
                    {
                        encabezado += (char)32;
                    }
                }
                else
                {
                    string concepto = condiciones.ConceptoPoliza.Length > 100 ? condiciones.ConceptoPoliza.Substring(0, 100) : condiciones.ConceptoPoliza;
                    while (concepto.Length <= 100)
                    {
                        concepto += (char)32;
                    }
                    encabezado += "    " + concepto + " ";
                }
                encabezado += "1 2 ";
                string movimientos = string.Empty;
                foreach (MovimientoPolizaEntity movimiento in poliza.Movimientos)
                {
                    if (!movimiento.Excluir)
                    {
                        if (condiciones.MultiplesEncabezados)
                        {
                            if (movimiento.EsPrimero)
                            {
                                string enc = encabezado;
                                if (condiciones.NombreCliente)
                                {
                                    enc = string.Empty;
                                    enc = encabezadoAfterNumPol;
                                    string concepTemp = movimiento.NombreCliente;
                                    if (string.IsNullOrEmpty(concepTemp))
                                    {
                                        for (int i = 1; i <= 106; i++)
                                            enc += (char)32;
                                    }
                                    else
                                    {
                                        string conc = concepTemp.Length > 100 ? concepTemp.Substring(0, 100) : concepTemp;
                                        while (conc.Length <= 100)
                                            conc += (char)32;
                                        enc += "    " + conc + " ";
                                    }
                                    enc += "1 2 ";
                                }
                                if (num != 0)
                                {
                                    if (!condiciones.NombreCliente)
                                        enc = encabezado.Replace(numInicial + "]", num.ToString());
                                    else
                                        enc = enc.Replace(numInicial + "]", num.ToString());
                                    num++;
                                }
                                movimientos += enc + Environment.NewLine;
                            }
                        }

                        if (condiciones.IncluirSegmento)
                        {
                            if (!string.IsNullOrEmpty(movimiento.Segmento.Trim()))
                            {
                                string seg = movimiento.Segmento.Trim();
                                if (seg.Length > 3)
                                    seg = seg.Substring(seg.Length - 3);
                                else if (seg.Length < 3)
                                {
                                    while (seg.Length < 3)
                                        seg = (char)32 + seg;
                                }
                                movimientos += "N " + seg + Environment.NewLine;
                            }
                        }

                        string cuenta = movimiento.Cuenta.Trim().Replace("-", string.Empty);
                        if (!condiciones.IncluirPeriodoEnReferencia)
                        {
                            int faltan = 32 - cuenta.Length;
                            for (int i = 1; i <= faltan; i++)
                            {
                                cuenta += (char)32;
                            }
                        }
                        else
                        {
                            if (cuenta.Length > 20)
                                cuenta = cuenta.Substring(0, 20);
                            int f = 20 - cuenta.Length;
                            for (int i = 1; i <= f; i++)
                                cuenta += (char)32;

                            cuenta += (char)32;

                            string referencia = movimiento.Periodo.Length > 10 ? movimiento.Periodo.Substring(0, 10) : movimiento.Periodo;
                            int relleno = 10 - referencia.Length;
                            for (int r = 1; r <= relleno; r++)
                                referencia += (char)32;

                            referencia += (char)32;
                            cuenta += referencia;
                        }
                        string importe = Decimal.Round(movimiento.ImporteNacional, 2).ToString("F");
                        int falta = 16 - importe.Length;
                        for (int i = 1; i <= falta; i++)
                        {
                            importe = (char)32 + importe;
                        }
                        for (int i = 1; i <= 16; i++)
                        {
                            importe += (char)32;
                        }
                        string concepto = movimiento.Concepto.Length > 30 ? movimiento.Concepto.Substring(0, 30) : movimiento.Concepto;
                        while (concepto.Length < 30)
                        {
                            concepto += (char)32;
                        }
                        importe += " 0.00 " + concepto + " ";
                        movimientos += "M " + cuenta + movimiento.TipoMovimiento + " " + importe + Environment.NewLine;
                    }
                }
                if (!condiciones.MultiplesEncabezados)
                {
                    if (num != 0)
                        encabezado = encabezado.Replace(numInicial + "]", num.ToString());
                    movimientos = encabezado + Environment.NewLine + movimientos;
                }
                string polizaText = /*encabezado + Environment.NewLine +*/ movimientos;
                FileStream fs = new FileStream(fileName, FileMode.Create);
                using (StreamWriter myTxt = new StreamWriter(fs, Encoding.ASCII))
                {
                    myTxt.Write(polizaText);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error al generar el archivo .txt." + Environment.NewLine + ex.Message;
            }
        }

        private string generarAspel(PolizaEntity poliza, int tipoPresentacion, string razonSocial, int tipoPolSaari, string numeroPoliza, string conceptoPoliza)
        {
            try
            {
                string numPol = numeroPoliza.Length > 5 ? numeroPoliza.Substring(0, 5) : numeroPoliza;
                string concepto = conceptoPoliza.Length > 120 ? conceptoPoliza.Substring(0, 120) : conceptoPoliza;
                bool cancelados = tipoPolSaari == 3;
                bool notasCredito = tipoPolSaari == 4;
                string path = Properties.Settings.Default.RutaRepositorio.EndsWith(@"\") ? Properties.Settings.Default.RutaRepositorio + @"Polizas\AspelCOI\" : Properties.Settings.Default.RutaRepositorio + @"\Polizas\Contpaq\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string tipoPoliza = string.Empty;
                string tipoAspel = string.Empty;
                if (poliza.Encabezado.TipoContpaq == 3)
                {
                    tipoAspel = "Dr";
                    if (cancelados)
                        tipoPoliza = "Cancelados_";
                    else if (notasCredito)
                        tipoPoliza = "NotasDeCredito_";
                    else
                        tipoPoliza = "Diario_";
                }
                else
                {
                    tipoAspel = "Ig";
                    tipoPoliza = "Ingreso_";
                }
                string fileName = path + tipoPoliza + poliza.Encabezado.Fecha.ToString("yyyy-MM-dd") + " Aspel-COI (" + razonSocial + ").xlsx";
                this.fileName = fileName;
                //Objetos Excel
                Excel.Application aplicacionExcel = new Excel.Application();
                Excel.Workbook libroExcel = aplicacionExcel.Workbooks.Add();
                Excel.Worksheet hojaExcel = (libroExcel.ActiveSheet as Excel.Worksheet); ;
                Excel.Range rango;
                int c = 1;
                int r = 3;
                rango = (hojaExcel.Cells[r, c] as Excel.Range);
                rango.Value2 = tipoAspel;
                c++;
                if (!string.IsNullOrEmpty(numPol))
                {
                    rango = (hojaExcel.Cells[r, c] as Excel.Range);
                    rango.Value2 = numPol.Trim();
                }
                if (!string.IsNullOrEmpty(concepto))
                {
                    rango = (hojaExcel.Cells[r, 3] as Excel.Range);
                    rango.Value2 = concepto.Trim();
                }
                rango = (hojaExcel.Cells[r, 4] as Excel.Range);
                rango.Value2 = poliza.Encabezado.Fecha.Day;
                r = 4;
                c = 2;
                foreach (MovimientoPolizaEntity movimiento in poliza.Movimientos)
                {
                    if (!movimiento.Excluir)
                    {
                        c = 2;
                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = "'" + movimiento.Cuenta;
                        c++;
                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = 0;
                        c++;
                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = movimiento.Concepto.Length <= 120 ? movimiento.Concepto : movimiento.Concepto.Substring(0, 120);
                        c++;
                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = 1;
                        rango.NumberFormat = "0.00";
                        c++;
                        if (movimiento.TipoMovimiento == 2)
                            c++;
                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = movimiento.ImporteNacional;
                        rango.NumberFormat = "0.00";
                        r++;
                    }
                }
                rango = (hojaExcel.Cells[4 + poliza.Movimientos.Count, 2] as Excel.Range);
                rango.Value2 = "FIN_PARTIDAS";
                hojaExcel.get_Range("A1:G50").Columns.AutoFit();
                libroExcel.SaveAs(fileName);
                aplicacionExcel.Visible = false;
                libroExcel.Close();
                aplicacionExcel.Quit();
                releaseObject(hojaExcel);
                releaseObject(libroExcel);
                releaseObject(aplicacionExcel);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error al generar el archivo .xlsx para Aspel-COI." + Environment.NewLine + ex.Message;
            }
        }

        private string generarContavision(PolizaEntity poliza, int tipoPresentacion, string razonSocial, int tipoPolSaari, string numeroPoliza, string conceptoPoliza)
        {
            try
            {
                bool cancelados = tipoPolSaari == 3;
                bool notasCredito = tipoPolSaari == 4;
                string path = Properties.Settings.Default.RutaRepositorio.EndsWith(@"\") ? Properties.Settings.Default.RutaRepositorio + @"Polizas\Contavision\" : Properties.Settings.Default.RutaRepositorio + @"\Polizas\Contpaq\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string numPol = numeroPoliza.Length > 10 ? numeroPoliza.Substring(0, 10) : numeroPoliza;
                while (numPol.Length < 10)
                {
                    numPol += (char)32;
                }
                string concepto = conceptoPoliza.Length > 100 ? conceptoPoliza.Substring(0, 100) : conceptoPoliza;
                string concepto2 = string.Empty;
                concepto = concepto.Length > 50 ? concepto.Substring(0, 50) : concepto;
                concepto2 = concepto.Length > 50 ? concepto.Substring(50, 100) : string.Empty;
                while (concepto.Length < 50)
                {
                    concepto += (char)32;
                }
                while (concepto2.Length < 50)
                {
                    concepto2 += (char)32;
                }
                string tipoPolizaEnc = poliza.Encabezado.TipoContpaq == 1 ? "I" : "D";
                string tipoPoliza = string.Empty;
                if (poliza.Encabezado.TipoContpaq == 3)
                {
                    if (cancelados)
                        tipoPoliza = "Cancelados_";
                    else if (notasCredito)
                        tipoPoliza = "NotasDeCredito_";
                    else
                        tipoPoliza = "Diario_";
                }
                else
                    tipoPoliza = "Ingreso_";
                string fileName = path + tipoPoliza + poliza.Encabezado.Fecha.ToString("yyyy-MM-dd") + " Contavision (" + razonSocial + ").txt";
                this.fileName = fileName;
                string encabezado = string.Empty;
                string mes = poliza.Encabezado.Fecha.Month.ToString().Length == 1 ? " " + poliza.Encabezado.Fecha.Month.ToString() : poliza.Encabezado.Fecha.Month.ToString();
                encabezado = "1" + "|" + numPol + "|" + poliza.Encabezado.Fecha.Year + "|" + mes + "|" + poliza.Encabezado.Fecha.ToString("yyyyMMdd") + "|" + tipoPolizaEnc + "|               |          | 0|          |" + concepto + "|" + concepto2 + "| 0|               |        0.000000|               |               |               | | | |        0.000000|                                                  " + Environment.NewLine;
                string movimientos = string.Empty;
                int renglon = 1;
                foreach (MovimientoPolizaEntity movimiento in poliza.Movimientos)
                {
                    if (!movimiento.Excluir)
                    {
                        string renglonSt = renglon.ToString();
                        while (renglonSt.Length < 4)
                        {
                            renglonSt = (char)32 + renglonSt;
                        }
                        string referencia = string.Empty;
                        referencia = movimiento.Referencia;
                        while (referencia.Length < 10)
                        {
                            referencia += (char)32;
                        }
                        string cuenta = movimiento.Cuenta;
                        while (cuenta.Length < 14)
                        {
                            cuenta += (char)32;
                        }
                        string conceptoMov = string.Empty;
                        conceptoMov = movimiento.Concepto.Length > 60 ? movimiento.Concepto.Substring(0, 60) : movimiento.Concepto;
                        while (conceptoMov.Length < 60)
                        {
                            conceptoMov += (char)32;
                        }
                        string importe = Decimal.Round(movimiento.ImporteNacional, 6).ToString("0.000000");
                        while (importe.Length < 16)
                        {
                            importe = (char)32 + importe;
                        }
                        string tipoMov = movimiento.TipoMovimiento == 1 ? "C" : "A";
                        string mov = string.Empty;
                        mov = "2|" + numPol + "|" + poliza.Encabezado.Fecha.Year + "|" + mes + "|" + poliza.Encabezado.Fecha.ToString("yyyyMMdd") + "|" + renglonSt + "|" + tipoPolizaEnc + "|" + referencia + "|" + cuenta + "|" + conceptoMov + "|" + importe + "|" + tipoMov + "| 0| 0" + Environment.NewLine;
                        movimientos += mov;
                        renglon++;
                    }
                }
                string polizaText = encabezado + movimientos;
                FileStream fs = new FileStream(fileName, FileMode.Create);
                using (StreamWriter myTxt = new StreamWriter(fs, Encoding.ASCII))
                {
                    myTxt.Write(polizaText);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error al generar el archivo .txt para Contavision." + Environment.NewLine + ex.Message;
            }
        }

        private string generarAxapta(PolizaEntity poliza, int tipoPresentacion, string razonSocial, CondicionesEntity condiciones, bool esEgreso)
        {
            try
            {
                string path = Properties.Settings.Default.RutaRepositorio.EndsWith(@"\") ? Properties.Settings.Default.RutaRepositorio + @"Polizas\Axapta\" : Properties.Settings.Default.RutaRepositorio + @"\Polizas\Axapta\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string tipoPoliza = string.Empty;

                switch (condiciones.TipoPoliza)
                {
                    case 1:
                        tipoPoliza = "Diario_";
                        break;
                    case 2:
                        tipoPoliza = "Ingresos_";
                        break;
                    case 3:
                        tipoPoliza = "Cancelados_";
                        break;
                    case 4:
                        tipoPoliza = "NotasDeCredito_";
                        break;
                    default:
                        tipoPoliza = "Poliza_";
                        break;
                }


                string fileName = path + tipoPoliza + DateTime.Now.ToString("yyyy-MM-dd_hhmmss") + " (" + razonSocial + ").xlsx";
                this.fileName = fileName;

                //ObjetosExcel
                Excel.Application aplicacionExcel = new Excel.Application();
                Excel.Workbook libroExcel = aplicacionExcel.Workbooks.Add();
                Excel.Worksheet hojaExcel = (libroExcel.ActiveSheet as Excel.Worksheet); ;
                Excel.Range rango;
                //aplicacionExcel.Visible = true;
                int r = 1;

                #region Encabezado
                for (int col = 1; col <= 42; col++)
                {
                    rango = (hojaExcel.Cells[r, col] as Excel.Range);
                    if (col == 1)
                        rango.Value2 = "Nombre";
                    else if (col == 2)
                        rango.Value2 = "Descripción";
                    else if (col == 3)
                        rango.Value2 = "Fecha";
                    else if (col == 4)
                        rango.Value2 = "Descripción";
                    else if (col == 5)
                        rango.Value2 = "Tipo de cuenta";
                    else if (col == 6)
                        rango.Value2 = "Cuenta";
                    else if (col == 7)
                        rango.Value2 = "Centro de Costo";
                    else if (col == 8)
                        rango.Value2 = "Departamento";
                    else if (col == 9)
                        rango.Value2 = "Tipo Gasto";
                    else if (col == 10)
                        rango.Value2 = "Reporte";
                    else if (col == 11)
                        rango.Value2 = "Financiera";
                    else if (col == 12)
                        rango.Value2 = "Propósito";
                    else if (col == 13)
                        rango.Value2 = "Dimensión";
                    else if (col == 14)
                        rango.Value2 = "Tipo de cuenta contrapartida";
                    else if (col == 15)
                        rango.Value2 = "Cuenta de contrapartida";
                    else if (col == 16)
                        rango.Value2 = "Centro de Costo";
                    else if (col == 17)
                        rango.Value2 = "Departamento";
                    else if (col == 18)
                        rango.Value2 = "Tipo Gasto";
                    else if (col == 19)
                        rango.Value2 = "Reporte";
                    else if (col == 20)
                        rango.Value2 = "Financiera";
                    else if (col == 21)
                        rango.Value2 = "Propósito";
                    else if (col == 22)
                        rango.Value2 = "Dimensión";
                    else if (col == 23)
                        rango.Value2 = "Grupo de impuestos sobre las ventas";
                    else if (col == 24)
                        rango.Value2 = "Grupo de impuestos por venta de artículos";
                    else if (col == 25)
                        rango.Value2 = "Debito";
                    else if (col == 26)
                        rango.Value2 = "Crédito";
                    else if (col == 27)
                        rango.Value2 = "Moneda";
                    else if (col == 28)
                        rango.Value2 = "Tipo de cambio";
                    else if (col == 29)
                        rango.Value2 = "";
                    else if (col == 30)
                        rango.Value2 = "";
                    else if (col == 31)
                        rango.Value2 = "Perfil de asiento contable";
                    else if (col == 32)
                        rango.Value2 = "Factura";
                    else if (col == 33)
                        rango.Value2 = "Fecha de vencimiento";
                    else if (col == 34)
                        rango.Value2 = "CURP / RFC";
                    else if (col == 35)
                        rango.Value2 = "Fecha de documento";
                    else if (col == 36)
                        rango.Value2 = "Documento";
                    else if (col == 37)
                        rango.Value2 = "Forma de Pago";
                    else if (col == 38)
                        rango.Value2 = "Destinatario";
                    else if (col == 39)
                        rango.Value2 = "Leyenda";
                    else if (col == 40)
                        rango.Value2 = "Tipo de transaccion bancaria";
                    else if (col == 41)
                        rango.Value2 = "Referencia de pago";
                    else if (col == 42)
                        rango.Value2 = "Aprobado por";
                }
                #endregion

                r++;
                foreach (MovimientoPolizaEntity mov in poliza.Movimientos)
                {
                    if (!mov.Excluir)
                    {
                        int c = 1;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = Properties.Settings.Default.AxaptaTipoPoliza;
                        c++;

                        if (condiciones.IncluirUUIDEnConcepto)
                        {
                            rango = (hojaExcel.Cells[r, c] as Excel.Range);
                            rango.Value2 = string.Format("{0}, {1}, {2}", mov.SerieFolio, mov.UUID, mov.ClienteNombreComercial);
                        }
                        else if (condiciones.RfcUuid)
                        {
                            rango = (hojaExcel.Cells[r, c] as Excel.Range);
                            rango.Value2 = string.Format("{0} {1}", mov.UUID, mov.RFC).ToUpper();
                        }
                        else
                        {
                            rango = (hojaExcel.Cells[r, c] as Excel.Range);
                            rango.Value2 = string.Format("{0}, {1}, {2}", mov.SerieFolio, mov.Periodo, mov.ClienteNombreComercial);
                        }
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = mov.Fecha.Day + "/" + mov.Fecha.Month + "/" + mov.Fecha.Year;//.ToString("dd/MM/yyyy");
                        c++;

                        if (condiciones.IncluirUUIDEnConcepto)
                        {
                            rango = (hojaExcel.Cells[r, c] as Excel.Range);
                            rango.Value2 = string.Format("{0}, {1}, {2}", mov.SerieFolio, mov.UUID, mov.ClienteNombreComercial);
                        }
                        else if (condiciones.RfcUuid)
                        {
                            rango = (hojaExcel.Cells[r, c] as Excel.Range);
                            rango.Value2 = string.Format("{0} {1}", mov.UUID, mov.RFC).ToUpper();
                        }
                        else
                        {
                            rango = (hojaExcel.Cells[r, c] as Excel.Range);
                            rango.Value2 = string.Format("{0}, {1}, {2}", mov.SerieFolio, mov.Periodo, mov.ClienteNombreComercial);
                        }
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        if (mov.Tipo == "IA")
                            rango.Value2 = "CLIENTE";
                        else
                            rango.Value2 = Properties.Settings.Default.AxaptaTipoCuenta;
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        //string cta = string.Empty;
                        //if (mov.Cuenta.ToString() != "NO CUENTA")
                        //{
                        //    if (mov.Cuenta.Length < 7)
                        //    {
                        //        int numCeros = 7 - mov.Cuenta.Length;
                        //        char pad = '0';
                        //        cta = mov.Cuenta.PadLeft(numCeros,pad);
                        //    }
                        //}
                        //else
                        //    cta = 

                        rango.Value2 = mov.Cuenta.ToString();
                        rango.NumberFormat = "0000000";
                        //cta;
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = Properties.Settings.Default.AxaptaCentroCosto;
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = mov.IdentificadorConjunto;
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = "VENTA";
                        //c++;
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = mov.NotaCincoContrato;
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = Properties.Settings.Default.AxaptaFinanciera;
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = Properties.Settings.Default.AxaptaProposito;
                        c++;
                        c++;
                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = "CONTABILIDAD";
                        c++;

                        c += 5;
                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = Properties.Settings.Default.AxaptaFinanciera;
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = Properties.Settings.Default.AxaptaProposito;
                        c++;
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = Properties.Settings.Default.AxaptaGISobreVentas;
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = Properties.Settings.Default.AxaptaGIPorVentaArticulos;
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        if (mov.TipoMovimiento == 1)
                            rango.Value2 = mov.ImporteNacional.ToString("N2");
                        else
                            rango.Value2 = 0M.ToString("N2");
                        rango.NumberFormat = "#,0.00";
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        if (mov.TipoMovimiento == 2)
                            rango.Value2 = mov.ImporteNacional.ToString("N2");
                        else
                            rango.Value2 = 0M.ToString("N2");
                        rango.NumberFormat = "#,0.00";
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = mov.Moneda;
                        c++;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = 1M.ToString("N2");
                        rango.NumberFormat = "#,0.00";
                        c++;

                        c += 2;
                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = "GRAL";
                        c += 4;


                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = mov.Fecha.Day + "/" + mov.Fecha.Month + "/" + mov.Fecha.Year;//mov.Fecha.ToString("dd/MM/yyyy");
                        c++;

                        c += 6;

                        rango = (hojaExcel.Cells[r, c] as Excel.Range);
                        rango.Value2 = "'" + Properties.Settings.Default.AxaptaUsuario;
                        r++;
                    }
                }

                hojaExcel.get_Range("A1:AP50").Columns.AutoFit();
                libroExcel.SaveAs(fileName);
                aplicacionExcel.Visible = false;
                libroExcel.Close();
                aplicacionExcel.Quit();
                releaseObject(hojaExcel);
                releaseObject(libroExcel);
                releaseObject(aplicacionExcel);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error al generar el archivo .xlsx, para Axapta." + Environment.NewLine + ex.Message;
            }
        }

        private string generarXls(PolizaExcelEntity poliza, int tipoPresentacion, string razonSocial, int tipoPolSaari)
        {
            try
            {
                bool cancelados = tipoPolSaari == 3;
                bool notasCredito = tipoPolSaari == 4;
                string path = Properties.Settings.Default.RutaRepositorio.EndsWith(@"\") ? Properties.Settings.Default.RutaRepositorio + @"Polizas\Excel\" : Properties.Settings.Default.RutaRepositorio + @"\Polizas\Excel\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string tipoPoliza = string.Empty;
                if (poliza.TipoPoliza == 3)
                {
                    if (cancelados)
                        tipoPoliza = "Cancelados_";
                    else if (notasCredito)
                        tipoPoliza = "NotasDeCredito_";
                    else
                        tipoPoliza = "Diario_";
                }
                else
                    tipoPoliza = "Ingreso_";
                string fileName = path + tipoPoliza + DateTime.Now.ToString("yyyy-MM-dd") + " (" + razonSocial + ").xlsx";
                this.fileName = fileName;
                int columnasTotales = poliza.Encabezados.Count;
                //ObjetosExcel
                Excel.Application aplicacionExcel = new Excel.Application();
                Excel.Workbook libroExcel = aplicacionExcel.Workbooks.Add();
                Excel.Worksheet hojaExcel = (libroExcel.ActiveSheet as Excel.Worksheet); ;
                Excel.Range rango;
                int c = 1;
                int r = 1;
                foreach (ColumnasExcelEntity columna in poliza.Encabezados)
                {
                    r = 1;
                    rango = (hojaExcel.Cells[r, c] as Excel.Range);
                    rango.Value2 = columna.Encabezado;
                    foreach (MovimientoExcelEntity movimiento in poliza.Movimientos)
                    {
                        if (!movimiento.Excluir)
                        {
                            r++;
                            rango = (hojaExcel.Cells[r, c] as Excel.Range);
                            if (columna.Identificador == "Consecutivo")
                            {
                                rango.Value2 = movimiento.Consecutivo;
                                rango.NumberFormat = "###0";
                            }
                            else if (columna.Identificador == "Contador")
                            {
                                rango.Value2 = movimiento.Contador;
                                rango.NumberFormat = "###0";
                            }
                            else if (columna.Identificador == "Fecha de movimiento")
                            {
                                rango.Value2 = movimiento.FechaMovimiento.ToString("dd/MM/yyyy");
                            }
                            else if (columna.Identificador == "Cuenta")
                            {
                                rango.Value2 = "'" + movimiento.Cuenta;
                            }
                            else if (columna.Identificador == "Descripción de cuenta")
                            {
                                rango.Value2 = movimiento.DescripcionCuenta;
                            }
                            else if (columna.Identificador == "Cargos")
                            {
                                rango.Value2 = movimiento.Cargo;
                                rango.NumberFormat = "$###,###,###,###,##0.00";
                            }
                            else if (columna.Identificador == "Abonos")
                            {
                                rango.Value2 = movimiento.Abono;
                                rango.NumberFormat = "$###,###,###,###,##0.00";
                            }
                            else if (columna.Identificador == "Descripción")
                            {
                                rango.Value2 = movimiento.Descripcion;
                            }
                            else if (columna.Identificador == "Número de recibo")
                            {
                                rango.Value2 = "'" + movimiento.NumeroRecibo;
                            }
                            else if (columna.Identificador == "Serie y folio de CFD")
                            {
                                rango.Value2 = movimiento.SerieFolio;
                            }
                            else if (columna.Identificador == "Estatus")
                            {
                                rango.Value2 = movimiento.Estatus;
                            }
                            else if (columna.Identificador == "Cliente Razón Social")
                            {
                                rango.Value2 = movimiento.ClienteRazonSocial;
                            }
                            else if (columna.Identificador == "Cliente RFC")
                            {
                                rango.Value2 = movimiento.ClienteRFC;
                            }
                            else if (columna.Identificador == "Inmobiliaria Razón Social")
                            {
                                rango.Value2 = movimiento.InmobiliariaRazonSocial;
                            }
                            else if (columna.Identificador == "Inmobiliaria RFC")
                            {
                                rango.Value2 = movimiento.InmobiliariaRFC;
                            }
                            else if (columna.Identificador == "Conjunto")
                            {
                                rango.Value2 = movimiento.Conjunto;
                            }
                            else if (columna.Identificador == "Inmueble")
                            {
                                rango.Value2 = movimiento.Inmueble;
                            }
                            else
                            {
                                rango.Value2 = "¡No conocido!";
                            }
                        }
                    }
                    c++;
                }
                hojaExcel.get_Range("A1:Q50").Columns.AutoFit();
                libroExcel.SaveAs(fileName);
                aplicacionExcel.Visible = false;
                libroExcel.Close();
                aplicacionExcel.Quit();
                releaseObject(hojaExcel);
                releaseObject(libroExcel);
                releaseObject(aplicacionExcel);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error al generar el archivo .xlsx." + Environment.NewLine + ex.Message;
            }
        }

        private string generarContpaqXls(PolizaEntity poliza, string razonSocial, CondicionesEntity condiciones, bool esEgreso)
        {
            try
            {
                bool cancelados = condiciones.TipoPresentacion == 3;
                bool notasCredito = condiciones.TipoPresentacion == 4;
                string path = Properties.Settings.Default.RutaRepositorio.EndsWith(@"\") ? Properties.Settings.Default.RutaRepositorio + @"Polizas\Contpaq\" : Properties.Settings.Default.RutaRepositorio + @"\Polizas\Contpaq\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string tipoPoliza = string.Empty;
                if (poliza.Encabezado.TipoContpaq == 3)
                {
                    switch (condiciones.TipoPoliza)
                    {
                        case 1:
                            tipoPoliza = "Diario_";
                            break;
                        case 2:
                            tipoPoliza = "Provision_";
                            break;
                        case 3:
                            tipoPoliza = "Cancelados_";
                            break;
                        case 4:
                            tipoPoliza = "NotasDeCredito_";
                            break;
                        default:
                            tipoPoliza = "Diario_";
                            break;
                    }
                }
                else if (poliza.Encabezado.TipoContpaq == 1)
                    tipoPoliza = "Ingresos_";
                else
                    tipoPoliza = "Egresos_";
                //if (poliza.Encabezado.TipoContpaq == 3)
                //{
                //   /* if (cancelados)
                //        tipoPoliza = "Cancelados_";
                //    else*/ if (notasCredito)
                //        tipoPoliza = "NotasDeCredito_";
                //    else if (!esEgreso)
                //        tipoPoliza = "Diario_";
                //    else
                //        tipoPoliza = "Provision_";
                //}
                //else if (poliza.Encabezado.TipoContpaq == 1)
                //    tipoPoliza = "Ingreso_";
                //else
                //    tipoPoliza = "Egreso_";
                string razon = razonSocial.Replace('/', '_');
                string fileName = path + tipoPoliza + poliza.Encabezado.Fecha.ToString("yyyy-MM-dd_hhmmss") + " (" + razon + ").xls";
                this.fileName = fileName;

                Excel.Application aplicacionExcel = new Excel.Application();
                Excel.Workbook libroExcel = aplicacionExcel.Workbooks.Add();
                Excel.Worksheet hojaExcel = (libroExcel.ActiveSheet as Excel.Worksheet); ;
                Excel.Range rango;
                int columna = 1, renglon = 1, numPoliza = 0;

                if (!string.IsNullOrEmpty(condiciones.NumeroPoliza))
                    numPoliza = Convert.ToInt32(condiciones.NumeroPoliza);

                if (!condiciones.MultiplesEncabezados)
                {
                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Value2 = "P";
                    columna++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Value2 = poliza.Encabezado.Fecha;//.ToString("yyyyMMdd");
                    rango.NumberFormat = "yyyymmdd";
                    columna++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Value2 = poliza.Encabezado.TipoContpaq;
                    columna++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Value2 = numPoliza;
                    columna++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Value2 = condiciones.AfectarSaldos ? 1 : 2;
                    columna++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Value2 = "'0"; //poliza.Encabezado.TipoContpaq == 3 ? "0" : "01";
                    columna++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Value2 = string.IsNullOrEmpty(condiciones.ConceptoPoliza) ? string.Empty :
                        condiciones.ConceptoPoliza.Length > 100 ? condiciones.ConceptoPoliza.Substring(0, 100) : condiciones.ConceptoPoliza;
                    columna++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Value2 = 11;
                    columna++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Value2 = 0;
                    columna++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Value2 = 0;
                    columna++;

                    columna = 1;
                    renglon++;
                }
                
                string uuid = string.Empty;
                string movimientos = string.Empty;
                foreach (MovimientoPolizaEntity movimiento in poliza.Movimientos)
                {
                    if (!movimiento.Excluir && movimiento.IdPoliza >= 0)
                    {
                        if (condiciones.MultiplesEncabezados)
                        {
                            #region Primer Movimiento
                            if (movimiento.EsPrimero)
                            {
                                uuid = movimiento.UUID;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Value2 = "P";
                                columna++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                //rango.Value2 = poliza.Encabezado.Fecha;//.ToString("yyyyMMdd");
                                rango.Value2 = movimiento.Fecha;
                                rango.NumberFormat = "yyyymmdd";
                                //rango.NumberFormatLocal = "yyyymmdd";
                                columna++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Value2 = poliza.Encabezado.TipoContpaq;
                                columna++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Value2 = numPoliza;
                                columna++;
                                numPoliza++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Value2 = condiciones.AfectarSaldos ? 1 : 2;
                                columna++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Value2 = "'0";//poliza.Encabezado.TipoContpaq == 3 ? "'0" : movimiento.Tipo == "BMN" ? "'01" : "'0";
                                columna++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Value2 = condiciones.NombreCliente ? movimiento.NombreCliente :
                                    string.IsNullOrEmpty(condiciones.ConceptoPoliza) ? string.Empty : condiciones.ConceptoPoliza.Length > 100 ?
                                    condiciones.ConceptoPoliza.Substring(0, 100) : condiciones.ConceptoPoliza;
                                columna++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Value2 = 11;
                                columna++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Value2 = 0;
                                columna++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Value2 = 0;
                                columna++;

                                columna = 1;
                                renglon++;
                            }
                            #endregion
                        }


                        if (condiciones.IncluirSegmento)//ADD jl 11/02/2019
                        {
                            if (!string.IsNullOrEmpty(movimiento.Segmento.Trim()))
                            {
                                string seg = movimiento.Segmento.Trim();
                                if (seg.Length > 3)
                                    seg = seg.Substring(seg.Length - 3);
                                else if (seg.Length < 3)
                                {
                                    while (seg.Length < 3)
                                        seg = (char)32 + seg;
                                }
                                movimientos += "N " + seg + Environment.NewLine;
                            } 
                        }

                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        rango.Value2 = "M1";
                        columna++;

                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        if (string.IsNullOrEmpty(condiciones.Mascara) || !condiciones.GuionEnCuenta)
                        {
                            rango.Value2 = "'" + movimiento.Cuenta.Trim().Replace("-", string.Empty);
                        }
                        else
                        {
                            rango.Value2 = "'" + movimiento.Cuenta.Trim();
                        }
                        columna++;

                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        if (movimiento.Periodo != null)
                        {
                            rango.Value2 = condiciones.IncluirPeriodoEnReferencia ? movimiento.Periodo.Length > 10 ? movimiento.Periodo.Substring(0, 10) :
                            movimiento.Periodo : string.Empty;
                        }
                        else
                            rango.Value2 = string.Empty;
                        columna++;
                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        rango.Value2 = movimiento.TipoMovimiento == 1 ? 0 : 1;
                        columna++;

                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        //decimal num = Decimal.Round(movimiento.ImporteNacional, 4,MidpointRounding.AwayFromZero);                      
                        //decimal step = (decimal)Math.Pow(10, 2);
                        //int tmp = (int)Math.Truncate(step * num);
                        //decimal num2= tmp/step;
                        int cantDec = condiciones.CuatroDecimales ? 4 : 2;
                        rango.Value2 = Decimal.Round(movimiento.ImporteNacional, cantDec);
                        if (condiciones.CuatroDecimales)
                            rango.NumberFormat = "#0.0000";
                        else
                            rango.NumberFormat = "#0.00";

                        columna++;
                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        rango.Value2 = poliza.Encabezado.TipoContpaq == 3 ? "'0" : ((movimiento.Tipo == "BMN" || movimiento.Tipo == "BDL" || movimiento.Tipo == "BCC") && condiciones.DiariosEspeciales) ? "'01" : "'0";
                        columna++;

                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        rango.Value2 = Decimal.Round(movimiento.ImporteExtranjero, cantDec);
                        if (condiciones.CuatroDecimales)
                            rango.NumberFormat = "#0.0000";
                        else
                            rango.NumberFormat = "#0.00";
                        columna++;

                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        ConceptoGeneralEntity con = new ConceptoGeneralEntity();
                        if (poliza.ConceptosGenerales != null)
                        {
                            if (movimiento.IdPagoCobranza > 0)
                                con = poliza.ConceptosGenerales.Find(c => c.Id == movimiento.IdPagoCobranza);
                            else
                                con = poliza.ConceptosGenerales.Find(c => c.Id == movimiento.IdPoliza);
                        }
                        //if (movimiento.Tipo != "CC" && versionOBDC >= 8645 && condiciones.FechaInicio.Date >= fechaActPol.Date)
                        if (condiciones.TipoPoliza == 2 && movimiento.Tipo != "CC" && cobranzaEntity.Version >= 2212 && condiciones.FechaInicio.Date > cobranzaEntity.FechaAct.Date && condiciones.MultiplesEncabezados && movimiento.IdPagoCobranza > 0)
                        {
                            if (con != null)
                                rango.Value2 = con.Concepto.Length > 100 ? con.Concepto.Substring(0, 100) : con.Concepto;
                            else
                                rango.Value2 = "";
                        }
                        else
                        {
                            rango.Value2 = movimiento.Concepto.Length > 100 ? movimiento.Concepto.Substring(0, 100) : movimiento.Concepto;
                        }
                        columna++;

                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                        rango.Value2 = condiciones.IncluirSegmento ? "'" + movimiento.Segmento : string.Empty;
                        columna++;

                        if (condiciones.IncluirUUIDEnConcepto)
                        {
                            //if (versionOBDC >= 8645 && condiciones.FechaInicio.Date > fechaActPol.Date && (movimiento.Tipo == "BMN" || movimiento.Tipo == "BDL" || movimiento.Tipo == "IV" || movimiento.Tipo == "IVT") && condiciones.TipoPoliza == 2)
                            if (cobranzaEntity.Version >= 2212 && condiciones.FechaInicio.Date > cobranzaEntity.FechaAct.Date && (movimiento.Tipo == "BMN" || movimiento.Tipo == "BDL" || movimiento.Tipo == "IV" || movimiento.Tipo == "IVT") && condiciones.TipoPoliza == 2 && condiciones.MultiplesEncabezados && movimiento.IdPagoCobranza > 0)
                            {
                                ConceptoGeneralEntity uuids = new ConceptoGeneralEntity();
                                if (movimiento.IdPagoCobranza > 0)
                                    uuids = poliza.ConceptosGenerales.Find(u => u.Id == movimiento.IdPagoCobranza);
                                else
                                    uuids = poliza.ConceptosGenerales.Find(u => u.Id == movimiento.IdPoliza);
                                //Para los movimientos de tipo BMN Y BDL se asignan todas uuids relacionadas cone el idPago de Cobranza
                                //cuando son pagos a multiples cfdi's
                                if (uuids == null)
                                {
                                    uuids = new ConceptoGeneralEntity();
                                    uuids.Id = 0;
                                    uuids.UUIDs = "";
                                    uuids.Concepto = "";
                                }
                                string[] uuidRef = uuids.UUIDs.Split('|');
                                bool unCampoUIDVacio = true;
                                foreach (string uid in uuidRef)
                                {

                                    if (uid != "" || unCampoUIDVacio)
                                    {
                                        columna = 1;
                                        renglon++;
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.Value2 = "AM";
                                        columna++;
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.Value2 = uid;
                                        columna = 1;
                                        renglon++;
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.Value2 = "AD";
                                        columna++;
                                        rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                        rango.Value2 = uid;
                                        unCampoUIDVacio = false;
                                    }
                                }
                            }
                            else
                            {

                                columna = 1;
                                renglon++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Value2 = "AM";
                                columna++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Value2 = movimiento.UUID;
                                columna++;

                                columna = 1;
                                renglon++;

                                //TODO: Confirmar que se va a incluir AD siempre, no solamente en múltiples encabezados.
                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Value2 = "AD";
                                columna++;

                                rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                                rango.Value2 = movimiento.UUID;
                                columna++;
                            }
                        }

                        columna = 1;
                        renglon++;
                    }
                }
                //TODO: Si se confirma el código anterior ¿ya no será necesario este código?
                if (condiciones.IncluirUUIDEnConcepto && !string.IsNullOrEmpty(uuid))
                {
                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Value2 = "AD";
                    columna++;

                    rango = (hojaExcel.Cells[renglon, columna] as Excel.Range);
                    rango.Value2 = uuid;
                    columna++;

                    columna = 1;
                    renglon++;
                }
                hojaExcel.get_Range("A1:K150").Columns.AutoFit();
                libroExcel.SaveAs(fileName, Excel.XlFileFormat.xlWorkbookNormal);
                aplicacionExcel.Visible = false;
                libroExcel.Close();
                aplicacionExcel.Quit();
                releaseObject(hojaExcel);
                releaseObject(libroExcel);
                releaseObject(aplicacionExcel);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error al generar el archivo de Excel para Contpaq:" + Environment.NewLine + ex.Message;
            }
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
        //AQUIIIIIII
        private List<MovimientoPolizaEntity> sumarCuentasIguales(List<MovimientoPolizaEntity> movimientos)
        {
            List<int> idPagos = movimientos.Select(mov => mov.IdPagoCobranza).Distinct().ToList();
            List<string> cuentas = movimientos.Select(m => m.Cuenta).Distinct().ToList();
            List<MovimientoPolizaEntity> listaMovimientos = new List<MovimientoPolizaEntity>();
            foreach (int idPago in idPagos)
            {
                foreach (string cuenta in cuentas)
                {
                    List<MovimientoPolizaEntity> movimientosAuxiliar = movimientos.Where(m => m.Cuenta == cuenta && !m.Excluir).ToList();
                    decimal suma = movimientosAuxiliar.Where(m => m.Cuenta == cuenta).Sum(m => m.ImporteNacional);
                    MovimientoPolizaEntity mov = new MovimientoPolizaEntity();


                    //TO DO

                    listaMovimientos.Add(mov);
                }
            }
            return listaMovimientos;
        }

        private List<MovimientoPolizaEntity> globalizar(List<MovimientoPolizaEntity> movimientos)
        {
            List<string> cuentas = movimientos.Select(m => m.Cuenta).Distinct().ToList();
            List<MovimientoPolizaEntity> listaMovimientos = new List<MovimientoPolizaEntity>();
            foreach (string cuenta in cuentas)
            {
                List<MovimientoPolizaEntity> movimientosAuxiliar = movimientos.Where(m => m.Cuenta == cuenta && !m.Excluir).ToList();
                decimal cargo = movimientosAuxiliar.Where(m => m.TipoMovimiento == 1).Sum(m => m.ImporteNacional);
                decimal abono = movimientosAuxiliar.Where(m => m.TipoMovimiento == 2).Sum(m => m.ImporteNacional);
                //bool esCargo = cargo >= abono;
                //decimal diferencia = esCargo ? cargo - abono : abono - cargo;
                if (cargo != 0)
                {
                    MovimientoPolizaEntity mov = new MovimientoPolizaEntity()
                    {
                        Cuenta = cuenta,
                        TipoMovimiento = 1,
                        ImporteNacional = cargo,
                        Concepto = string.Empty
                    };
                    listaMovimientos.Add(mov);
                }
                if (abono != 0)
                {
                    MovimientoPolizaEntity mov = new MovimientoPolizaEntity()
                    {
                        Cuenta = cuenta,
                        TipoMovimiento = 2,
                        ImporteNacional = abono,
                        Concepto = string.Empty
                    };
                    listaMovimientos.Add(mov);
                }

            }
            return listaMovimientos;
        }

        private List<MovimientoPolizaEntity> consolidar(List<MovimientoPolizaEntity> movimientos, string cuentaIva, string cuentaIngreso)
        {
            List<MovimientoPolizaEntity> movimientosAux = new List<MovimientoPolizaEntity>();
            decimal iva = 0;
            decimal ingresos = 0;
            try
            {
                foreach (MovimientoPolizaEntity movimiento in movimientos)
                {
                    if (movimiento.Cuenta == cuentaIva)
                    {
                        iva += movimiento.ImporteNacional;
                    }
                    else if (movimiento.Cuenta == cuentaIngreso)
                    {
                        ingresos += movimiento.ImporteNacional;
                    }
                    else
                    {
                        movimientosAux.Add(movimiento);
                    }
                }
            }
            catch /*(Exception ex)*/ { }
            MovimientoPolizaEntity movIngreso = new MovimientoPolizaEntity();
            movIngreso.Concepto = "INGRESOS";
            movIngreso.Cuenta = cuentaIngreso;
            movIngreso.ImporteExtranjero = 0;
            movIngreso.ImporteNacional = Decimal.Round(ingresos, 2);
            movIngreso.TipoMovimiento = 2;
            movIngreso.Referencia = string.Empty;
            movimientosAux.Add(movIngreso);
            MovimientoPolizaEntity movIva = new MovimientoPolizaEntity();
            movIva.Concepto = "IVA";
            movIva.Cuenta = cuentaIva;
            movIva.ImporteExtranjero = 0;
            movIva.ImporteNacional = Decimal.Round(iva, 2);
            movIva.TipoMovimiento = 2;
            movIva.Referencia = string.Empty;
            movimientosAux.Add(movIva);
            return movimientosAux;
        }

        private List<MovimientoExcelEntity> consolidarExcel(List<MovimientoExcelEntity> movimientos, string cuentaIva, string cuentaIngreso)
        {
            int contadorConsolidado = 0;
            List<MovimientoExcelEntity> movimientosAux = new List<MovimientoExcelEntity>();
            decimal iva = 0;
            decimal ingresos = 0;
            try
            {
                foreach (MovimientoExcelEntity movimiento in movimientos)
                {
                    if (movimiento.Cuenta == cuentaIva)
                    {
                        iva += movimiento.Abono;
                    }
                    else if (movimiento.Cuenta == cuentaIngreso)
                    {
                        ingresos += movimiento.Abono;
                    }
                    else
                    {
                        contadorConsolidado++;
                        movimiento.Contador = 1;
                        movimiento.Consecutivo = contadorConsolidado;
                        movimientosAux.Add(movimiento);
                    }
                }
            }
            catch /*(Exception ex)*/ { }
            int ultMovCargo = movimientosAux.Count;
            MovimientoExcelEntity movIngreso = new MovimientoExcelEntity();
            contadorConsolidado++;
            movIngreso.Consecutivo = contadorConsolidado;
            movIngreso.Contador = 1;
            movIngreso.FechaMovimiento = movimientos[movimientos.Count - 1].FechaMovimiento;
            movIngreso.Cuenta = cuentaIngreso;
            movIngreso.DescripcionCuenta = "INGRESOS";
            movIngreso.Cargo = 0;
            movIngreso.Abono = Decimal.Round(ingresos, 2);
            movIngreso.Descripcion = "INGRESOS";
            movIngreso.NumeroRecibo = 0;
            movIngreso.SerieFolio = "CONSOLIDADO";
            movIngreso.Estatus = "CONSOLIDADO";
            movIngreso.ClienteRazonSocial = "CONSOLIDADO";
            movIngreso.ClienteRFC = "CONSOLIDADO";
            movIngreso.InmobiliariaRazonSocial = movimientos[movimientos.Count - 1].InmobiliariaRazonSocial;
            movIngreso.InmobiliariaRFC = movimientos[movimientos.Count - 1].InmobiliariaRFC;
            movIngreso.Conjunto = "CONSOLIDADO";
            movIngreso.Inmueble = "CONSOLIDADO";
            movimientosAux.Add(movIngreso);
            MovimientoExcelEntity movIva = new MovimientoExcelEntity();
            contadorConsolidado++;
            movIva.Consecutivo = contadorConsolidado;
            movIva.Contador = 1;
            movIva.FechaMovimiento = movimientos[movimientos.Count - 1].FechaMovimiento;
            movIva.Cuenta = cuentaIva;
            movIva.DescripcionCuenta = "IVA";
            movIva.Cargo = 0;
            movIva.Abono = Decimal.Round(iva, 2);
            movIva.Descripcion = "IVA";
            movIva.NumeroRecibo = 0;
            movIva.SerieFolio = "CONSOLIDADO";
            movIva.Estatus = "CONSOLIDADO";
            movIva.ClienteRazonSocial = "CONSOLIDADO";
            movIva.ClienteRFC = "CONSOLIDADO";
            movIva.InmobiliariaRazonSocial = movimientos[movimientos.Count - 1].InmobiliariaRazonSocial;
            movIva.InmobiliariaRFC = movimientos[movimientos.Count - 1].InmobiliariaRFC;
            movIva.Conjunto = "CONSOLIDADO";
            movIva.Inmueble = "CONSOLIDADO";
            movimientosAux.Add(movIva);
            return movimientosAux;
        }

        private string generarEgreso()
        {
            List<EgresoEntity> egresos = Polizas.getEgresos(condiciones);
            if (egresos != null)
            {
                if (egresos.Count > 0)
                {
                    string razonSocial = SaariDB.getNombreInmobiliariaByID(condiciones.Inmobiliaria);
                    List<FormulaPolizaEntity> listaFormulas = Polizas.getFormulasEgreso(condiciones.Inmobiliaria, condiciones.TipoPoliza);
                    if (listaFormulas.Count > 0)
                    {
                        if (condiciones.Formato == FormatoExportacionPoliza.ContpaqTxt)
                        {
                            #region Contpaq .txt
                            try
                            {
                                string[] mascara = null;
                                if (!string.IsNullOrEmpty(condiciones.Mascara))
                                    mascara = condiciones.Mascara.Trim().Split('-');
                                PolizaEntity poliza = new PolizaEntity();
                                EncabezadoPolizaEntity encabezado = new EncabezadoPolizaEntity();
                                encabezado.Fecha = condiciones.FechaInicio;
                                if (condiciones.TipoPoliza == 1)
                                    encabezado.TipoContpaq = 2;
                                else
                                    encabezado.TipoContpaq = 3;//Diario para provisiones
                                poliza.Encabezado = encabezado;
                                List<MovimientoPolizaEntity> movimientos = new List<MovimientoPolizaEntity>();
                                bool esPrimero = false;
                                foreach (EgresoEntity egreso in egresos)
                                {
                                    esPrimero = true;
                                    foreach (FormulaPolizaEntity formula in listaFormulas)
                                    {
                                        if (formula.TipoClave != "GPE")
                                        {
                                            MovimientoPolizaEntity movimiento = getMovimientos(egreso, formula);
                                            if (movimiento == null)
                                                return "Error al obtener el movimiento del recibo " + egreso.ID;
                                            if (condiciones.MultiplesEncabezados)
                                            {
                                                movimiento.EsPrimero = esPrimero;
                                                esPrimero = false;
                                            }
                                            if (!string.IsNullOrEmpty(movimiento.Cuenta) && movimiento.Cuenta != "NO CUENTA")
                                            {
                                                if (mascara != null)
                                                {
                                                    string cuentaEnmascarada = string.Empty;
                                                    string[] cuenta = movimiento.Cuenta.Trim().Split('-');
                                                    if (cuenta.Length == mascara.Length)
                                                    {
                                                        #region MASCARA MISMA LONGITUD QUE LA CUENTA
                                                        bool error = false;
                                                        for (int i = 0; i < cuenta.Length; i++)
                                                        {
                                                            if (!string.IsNullOrEmpty(cuenta[i]) && !string.IsNullOrEmpty(mascara[i]))
                                                            {
                                                                int masc = Convert.ToInt32(mascara[i]);
                                                                string m = string.Empty;
                                                                for (int j = 0; j < masc; j++)
                                                                    m += "9";
                                                                int max = Convert.ToInt32(m);
                                                                if (!string.IsNullOrEmpty(cuenta[i]))
                                                                {
                                                                    if (Convert.ToInt64(cuenta[i]) <= max)
                                                                    {
                                                                        cuentaEnmascarada += cuenta[i].Substring(cuenta[i].Length - masc) + "-";
                                                                    }
                                                                    else
                                                                    {
                                                                        error = true;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                error = true;
                                                                break;
                                                            }
                                                        }
                                                        if (!error && !string.IsNullOrEmpty(cuentaEnmascarada))
                                                            movimiento.Cuenta = cuentaEnmascarada.Substring(0, cuentaEnmascarada.Length - 1);
                                                        #endregion
                                                    }
                                                    else if (cuenta.Length < mascara.Length)
                                                    {
                                                        #region MASCARA CON LONGITUD MAYOR A LA DE LA CUENTA
                                                        bool error = false;
                                                        int limite = cuenta.Length;
                                                        for (int i = 0; i < mascara.Length; i++)
                                                        {
                                                            if (!string.IsNullOrEmpty(mascara[i]))
                                                            {
                                                                int masc = Convert.ToInt32(mascara[i]);
                                                                string m = string.Empty;
                                                                for (int j = 0; j < masc; j++)
                                                                    m += "9";
                                                                int max = Convert.ToInt32(m);
                                                                if (i < limite)
                                                                {
                                                                    if (!string.IsNullOrEmpty(cuenta[i]))
                                                                    {
                                                                        if (Convert.ToInt64(cuenta[i]) <= max)
                                                                            cuentaEnmascarada += cuenta[i].Substring(cuenta[i].Length - masc) + "-";
                                                                        else
                                                                        {
                                                                            error = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    for (int j = 0; j < m.Length; j++)
                                                                        cuentaEnmascarada += "0";
                                                                    cuentaEnmascarada += "-";
                                                                }
                                                            }
                                                        }
                                                        if (!error && !string.IsNullOrEmpty(cuentaEnmascarada))
                                                            movimiento.Cuenta = cuentaEnmascarada.Substring(0, cuentaEnmascarada.Length - 1);
                                                        #endregion
                                                    }
                                                }
                                            }
                                            movimientos.Add(movimiento);
                                        }
                                        else
                                        {
                                            foreach (var registro in egreso.ListaRegistros)
                                            {
                                                MovimientoPolizaEntity movimiento = getMovimientos(registro, formula);
                                                if (movimiento == null)
                                                    return "Error al obtener el movimiento del recibo " + egreso.ID;
                                                if (condiciones.MultiplesEncabezados)
                                                {
                                                    movimiento.EsPrimero = esPrimero;
                                                    esPrimero = false;
                                                }
                                                if (!string.IsNullOrEmpty(movimiento.Cuenta) && movimiento.Cuenta != "NO CUENTA")
                                                {
                                                    if (mascara != null)
                                                    {
                                                        string cuentaEnmascarada = string.Empty;
                                                        string[] cuenta = movimiento.Cuenta.Trim().Split('-');
                                                        if (cuenta.Length == mascara.Length)
                                                        {
                                                            #region MASCARA CON IGUAL LONGITUD QUE LA CUENTA 2
                                                            bool error = false;
                                                            for (int i = 0; i < cuenta.Length; i++)
                                                            {
                                                                if (!string.IsNullOrEmpty(cuenta[i]) && !string.IsNullOrEmpty(mascara[i]))
                                                                {
                                                                    int masc = Convert.ToInt32(mascara[i]);
                                                                    string m = string.Empty;
                                                                    for (int j = 0; j < masc; j++)
                                                                        m += "9";
                                                                    int max = Convert.ToInt32(m);
                                                                    if (!string.IsNullOrEmpty(cuenta[i]))
                                                                    {
                                                                        if (Convert.ToInt64(cuenta[i]) <= max)
                                                                        {
                                                                            cuentaEnmascarada += cuenta[i].Substring(cuenta[i].Length - masc) + "-";
                                                                        }
                                                                        else
                                                                        {
                                                                            error = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    error = true;
                                                                    break;
                                                                }
                                                            }
                                                            if (!error && !string.IsNullOrEmpty(cuentaEnmascarada))
                                                                movimiento.Cuenta = cuentaEnmascarada.Substring(0, cuentaEnmascarada.Length - 1);
                                                            #endregion
                                                        }
                                                        else if (cuenta.Length < mascara.Length)
                                                        {
                                                            #region MASCARA CON LONGITUD MAYOR A LA DE LA CUENTA 2
                                                            bool error = false;
                                                            int limite = cuenta.Length;
                                                            for (int i = 0; i < mascara.Length; i++)
                                                            {
                                                                if (!string.IsNullOrEmpty(mascara[i]))
                                                                {
                                                                    int masc = Convert.ToInt32(mascara[i]);
                                                                    string m = string.Empty;
                                                                    for (int j = 0; j < masc; j++)
                                                                        m += "9";
                                                                    int max = Convert.ToInt32(m);
                                                                    if (i < limite)
                                                                    {
                                                                        if (!string.IsNullOrEmpty(cuenta[i]))
                                                                        {
                                                                            if (Convert.ToInt64(cuenta[i]) <= max)
                                                                                cuentaEnmascarada += cuenta[i].Substring(cuenta[i].Length - masc) + "-";
                                                                            else
                                                                            {
                                                                                error = true;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        for (int j = 0; j < m.Length; j++)
                                                                            cuentaEnmascarada += "0";
                                                                        cuentaEnmascarada += "-";
                                                                    }
                                                                }
                                                            }
                                                            if (!error && !string.IsNullOrEmpty(cuentaEnmascarada))
                                                                movimiento.Cuenta = cuentaEnmascarada.Substring(0, cuentaEnmascarada.Length - 1);
                                                            #endregion
                                                        }
                                                    }
                                                }
                                                movimientos.Add(movimiento);
                                            }
                                        }
                                    }
                                }
                                poliza.Movimientos = movimientos;
                                return generarTxt(poliza, razonSocial, condiciones, true);
                            }
                            catch (Exception ex)
                            {
                                return "Hubo un error inesperado al momento de cargar los datos de la póliza Contpaq en TXT." + Environment.NewLine + ex.Message;
                            }
                            #endregion
                        }
                        else if (condiciones.Formato == FormatoExportacionPoliza.ContpaqXls)
                        {
                            #region Contpaqt .xls
                            try
                            {
                                string[] mascara = null;
                                if (!string.IsNullOrEmpty(condiciones.Mascara))
                                    mascara = condiciones.Mascara.Trim().Split('-');
                                PolizaEntity poliza = new PolizaEntity();
                                EncabezadoPolizaEntity encabezado = new EncabezadoPolizaEntity();
                                encabezado.Fecha = condiciones.FechaInicio;
                                if (condiciones.TipoPoliza == 1)
                                    encabezado.TipoContpaq = 2;
                                else
                                    encabezado.TipoContpaq = 3;//Diario para provisiones
                                poliza.Encabezado = encabezado;
                                List<MovimientoPolizaEntity> movimientos = new List<MovimientoPolizaEntity>();
                                bool esPrimero = false;
                                foreach (EgresoEntity egreso in egresos)
                                {
                                    esPrimero = true;
                                    foreach (FormulaPolizaEntity formula in listaFormulas)
                                    {
                                        if (formula.TipoClave != "GPE")
                                        {
                                            MovimientoPolizaEntity movimiento = getMovimientos(egreso, formula);
                                            if (movimiento == null)
                                                return "Error al obtener el movimiento del recibo " + egreso.ID;
                                            if (condiciones.MultiplesEncabezados)
                                            {
                                                movimiento.EsPrimero = esPrimero;
                                                esPrimero = false;
                                            }
                                            if (!string.IsNullOrEmpty(movimiento.Cuenta) && movimiento.Cuenta != "NO CUENTA")
                                            {
                                                if (mascara != null)
                                                {
                                                    string cuentaEnmascarada = string.Empty;
                                                    string[] cuenta = movimiento.Cuenta.Trim().Split('-');
                                                    if (cuenta.Length == mascara.Length)
                                                    {
                                                        #region MASCARA IGUAL LONGITUD QUE CUENTA XLS
                                                        bool error = false;
                                                        for (int i = 0; i < cuenta.Length; i++)
                                                        {
                                                            if (!string.IsNullOrEmpty(cuenta[i]) && !string.IsNullOrEmpty(mascara[i]))
                                                            {
                                                                int masc = Convert.ToInt32(mascara[i]);
                                                                string m = string.Empty;
                                                                for (int j = 0; j < masc; j++)
                                                                    m += "9";
                                                                int max = Convert.ToInt32(m);
                                                                if (!string.IsNullOrEmpty(cuenta[i]))
                                                                {
                                                                    if (Convert.ToInt64(cuenta[i]) <= max)
                                                                    {
                                                                        cuentaEnmascarada += cuenta[i].Substring(cuenta[i].Length - masc) + "-";
                                                                    }
                                                                    else
                                                                    {
                                                                        error = true;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                error = true;
                                                                break;
                                                            }
                                                        }
                                                        if (!error && !string.IsNullOrEmpty(cuentaEnmascarada))
                                                            movimiento.Cuenta = cuentaEnmascarada.Substring(0, cuentaEnmascarada.Length - 1);
                                                        #endregion
                                                    }
                                                    else if (cuenta.Length < mascara.Length)
                                                    {
                                                        #region MASCARA LONGITUD MAYOR A LA CUENTA XLS
                                                        bool error = false;
                                                        int limite = cuenta.Length;
                                                        for (int i = 0; i < mascara.Length; i++)
                                                        {
                                                            if (!string.IsNullOrEmpty(mascara[i]))
                                                            {
                                                                int masc = Convert.ToInt32(mascara[i]);
                                                                string m = string.Empty;
                                                                for (int j = 0; j < masc; j++)
                                                                    m += "9";
                                                                int max = Convert.ToInt32(m);
                                                                if (i < limite)
                                                                {
                                                                    if (!string.IsNullOrEmpty(cuenta[i]))
                                                                    {
                                                                        if (Convert.ToInt64(cuenta[i]) <= max)
                                                                            cuentaEnmascarada += cuenta[i].Substring(cuenta[i].Length - masc) + "-";
                                                                        else
                                                                        {
                                                                            error = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    for (int j = 0; j < m.Length; j++)
                                                                        cuentaEnmascarada += "0";
                                                                    cuentaEnmascarada += "-";
                                                                }
                                                            }
                                                        }
                                                        if (!error && !string.IsNullOrEmpty(cuentaEnmascarada))
                                                            movimiento.Cuenta = cuentaEnmascarada.Substring(0, cuentaEnmascarada.Length - 1);
                                                        #endregion
                                                    }
                                                }
                                            }
                                            movimientos.Add(movimiento);
                                        }
                                        else
                                        {
                                            //GPE
                                            foreach (var registro in egreso.ListaRegistros)
                                            {
                                                MovimientoPolizaEntity movimiento = getMovimientos(registro, formula);
                                                if (movimiento == null)
                                                    return "Error al obtener el movimiento del recibo " + egreso.ID;
                                                if (condiciones.MultiplesEncabezados)
                                                {
                                                    movimiento.EsPrimero = esPrimero;
                                                    esPrimero = false;
                                                }
                                                if (!string.IsNullOrEmpty(movimiento.Cuenta) && movimiento.Cuenta != "NO CUENTA")
                                                {
                                                    if (mascara != null)
                                                    {
                                                        string cuentaEnmascarada = string.Empty;
                                                        string[] cuenta = movimiento.Cuenta.Trim().Split('-');
                                                        if (cuenta.Length == mascara.Length)
                                                        {
                                                            #region MASCARA IGUAL LONGITUD QUE CUENTA XLS 2
                                                            bool error = false;
                                                            for (int i = 0; i < cuenta.Length; i++)
                                                            {
                                                                if (!string.IsNullOrEmpty(cuenta[i]) && !string.IsNullOrEmpty(mascara[i]))
                                                                {
                                                                    int masc = Convert.ToInt32(mascara[i]);
                                                                    string m = string.Empty;
                                                                    for (int j = 0; j < masc; j++)
                                                                        m += "9";
                                                                    int max = Convert.ToInt32(m);
                                                                    if (!string.IsNullOrEmpty(cuenta[i]))
                                                                    {
                                                                        if (Convert.ToInt64(cuenta[i]) <= max)
                                                                        {
                                                                            cuentaEnmascarada += cuenta[i].Substring(cuenta[i].Length - masc) + "-";
                                                                        }
                                                                        else
                                                                        {
                                                                            error = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    error = true;
                                                                    break;
                                                                }
                                                            }
                                                            if (!error && !string.IsNullOrEmpty(cuentaEnmascarada))
                                                                movimiento.Cuenta = cuentaEnmascarada.Substring(0, cuentaEnmascarada.Length - 1);
                                                            #endregion
                                                        }
                                                        else if (cuenta.Length < mascara.Length)
                                                        {
                                                            #region MASCARA LONGITUD MAYOR QUE CUENTA XLS 2
                                                            bool error = false;
                                                            int limite = cuenta.Length;
                                                            for (int i = 0; i < mascara.Length; i++)
                                                            {
                                                                if (!string.IsNullOrEmpty(mascara[i]))
                                                                {
                                                                    int masc = Convert.ToInt32(mascara[i]);
                                                                    string m = string.Empty;
                                                                    for (int j = 0; j < masc; j++)
                                                                        m += "9";
                                                                    int max = Convert.ToInt32(m);
                                                                    if (i < limite)
                                                                    {
                                                                        if (!string.IsNullOrEmpty(cuenta[i]))
                                                                        {
                                                                            if (Convert.ToInt64(cuenta[i]) <= max)
                                                                                cuentaEnmascarada += cuenta[i].Substring(cuenta[i].Length - masc) + "-";
                                                                            else
                                                                            {
                                                                                error = true;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        for (int j = 0; j < m.Length; j++)
                                                                            cuentaEnmascarada += "0";
                                                                        cuentaEnmascarada += "-";
                                                                    }
                                                                }
                                                            }
                                                            if (!error && !string.IsNullOrEmpty(cuentaEnmascarada))
                                                                movimiento.Cuenta = cuentaEnmascarada.Substring(0, cuentaEnmascarada.Length - 1);
                                                            #endregion
                                                        }
                                                    }
                                                }
                                                movimientos.Add(movimiento);
                                            }
                                        }
                                    }
                                }
                                poliza.Movimientos = movimientos;
                                return generarContpaqXls(poliza, razonSocial, condiciones, true);
                            }
                            catch (Exception ex)
                            {
                                return "Hubo un error inesperado al momento de cargar los datos de la póliza Contpaq en Excel." + Environment.NewLine + ex.Message;
                            }
                            #endregion
                        }
                        else
                            return "Formato no implementado";
                    }
                    else
                        return "No se encontró configuración para la inmobiliaria y el tipo de poliza en cuestion.";
                }
                else
                    return "No se encontraron registros para las condiciones seleccionadas y/o hubo errores al obtener registros.";
            }
            else
                return "Hubo un error al obtener los registros de egresos";
        }

        public static List<SucursalEntity> obtenerSucursales()
        {
            return Polizas.getSucursales();
        }

        public void cancelar()
        {
            cancelacionPendiente = true;
        }

        protected virtual void OnCambioProgreso(int progreso)
        {
            var handler = CambioProgreso;
            if (handler != null)
                handler(this, new CambioProgresoEventArgs(progreso > 100 ? 100 : progreso));
        }
    }
}
