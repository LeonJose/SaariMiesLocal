using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    public class CondicionesEntity
    {
        /// <summary>
        /// Identificador de la inmobiliaria
        /// </summary>
        public string Inmobiliaria { get; set; }
        /// <summary>
        /// Propiedad que indica cual sera el formato del archivo. 1.- Contpaq (.txt) 2.- Excel(.xlsx) 3.- Aspel-COI (.xlsx) 4.- Contavision(.txt) 5.- Axapta (.xlsx)
        /// </summary>
        public FormatoExportacionPoliza Formato { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool IncluirCancelados { get; set; }
        /// <summary>
        /// Indica de que manera se va a presentar la póliza. 1.- Consolidado 2.-Detallado 3.-Global
        /// </summary>
        public int TipoPresentacion { get; set; }
        /// <summary>
        /// Indica que tipo de póliza se va a generar. Facturación: 1.- Diario 2.- Ingreso 3.- Recibos Cancelados 4.- Notas de crédito Cheques: 1.- Egreso 2.- Provisión
        /// Cuando la poliza es del modulo de ventas 5.- Diario 6.- Ingresos
        /// </summary>
        public int TipoPoliza { get; set; }
        public string NumeroPoliza { get; set; }
        public string ConceptoPoliza { get; set; }
        public bool IncluirUUIDEnConcepto { get; set; }
        public string Mascara { get; set; }
        public bool MultiplesEncabezados { get; set; }
        public bool IncluirSegmento { get; set; }
        /// <summary>
        /// Indica si vamos a tomar el nombre del cliente como concepto de la póliza
        /// </summary>
        public bool NombreCliente { get; set; }
        /// <summary>
        /// Afectar saldos en contpaq. 1 si desea afectar 2 sin afectaciones
        /// </summary>
        public bool AfectarSaldos { get; set; }
        /// <summary>
        /// Indica si el concepto principal va a ser el concepto en los movimientos aunque sean de cargos periodicos (subtipos)
        /// </summary>
        public bool UsarConceptoPrincipal { get; set; }
        public bool IncluirPeriodoEnReferencia { get; set; }
        public bool ClienteEnLugarDePeriodo { get; set; }
        public bool ExcluirConceptoLibre { get; set; }
        public SucursalEntity Sucursal { get; set; }
        /// <summary>
        /// Indica la cantidad de decimales al generar la poliza de ingresos tipo ContPaq XLS
        /// </summary>
        public bool CuatroDecimales { get; set; }
        public bool DiariosEspeciales { get; set; }
        public bool GuionEnCuenta { get; set; }
        public bool PolizaPorRango { get; set; }

        public bool RfcUuid { get; set; }
    }
}
