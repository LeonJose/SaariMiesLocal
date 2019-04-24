using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    class MovimientoPolizaEntity
    {
        /// <summary>
        /// Número de Identificador de Póliza 
        /// </summary>
        public int IdPoliza { get; set; }

        public string Cuenta { get; set; }
        /// <summary>
        /// Tipo de movimiento. 1.- Cargo 2.- Abono
        /// </summary>
        public int TipoMovimiento { get; set; }
        public decimal ImporteNacional { get; set; }
        public decimal ImporteExtranjero { get; set; }
        /// <summary>
        /// Máximo 30 caracteres
        /// </summary>
        public string Concepto { get; set; }
        public string CuentaDescripcion { get; set; }
        /// <summary>
        /// Compuesta por SerieFolio ejemplo: A158
        /// </summary>
        public string Referencia { get; set; }
        /// <summary>
        /// Serie y folio de la factura electrónica asociada. Ejemplo: A-158
        /// </summary>
        public string SerieFolio { get; set; }
        /// <summary>
        /// Periodo de facturación
        /// </summary>
        public string Periodo { get; set; }
        /// <summary>
        /// Nombre comercial del cliente asociado
        /// </summary>
        public string ClienteNombreComercial { get; set; }
        /// <summary>
        /// Si el tipo de la póliza es diario, corresponderá a la fecha de emisión, de lo contrario a la fecha de pago
        /// </summary>
        public DateTime Fecha { get; set; }
        /// <summary>
        /// Identificador del conjunto capturado por el usuario
        /// </summary>
        public string IdentificadorConjunto { get; set; }
        /// <summary>
        /// Nota 5 dentro de las observaciones de contrato, utilizado para Axapta en su columna Reporte
        /// </summary>
        public string NotaCincoContrato { get; set; }
        /// <summary>
        /// Moneda de emisión
        /// </summary>
        public string Moneda { get; set; }
        /// <summary>
        /// Indica si se debe excluir el movimiento dado que si importe es cero
        /// </summary>
        public bool Excluir { get; set; }
        /// <summary>
        /// Indica si el movimiento es el primero para cada recibo
        /// </summary>
        public bool EsPrimero { get; set; }
        /// <summary>
        /// Contiene el segmento de negocio asociado a la factura de acuerdo al identificador del conjunto capturado por el usuario
        /// </summary>
        public string Segmento { get; set; }
        /// <summary>
        /// Razon social del cliente asociado
        /// </summary>
        public string NombreCliente { get; set; }
        /// <summary>
        /// Tipo en Saari para el movimiento. Ejemplo BMN
        /// </summary>
        public string Tipo { get; set; }
        /// <summary>
        /// Tipo de cuenta en coopropiedad. Ejemplo IA
        /// </summary>
        public string TipoCuentaCopropiedad { get; set; }
        /// <summary>
        /// Indica si corresponde a una factura por concepto libre
        /// </summary>
        public bool EsLibre { get; set; }
        /// <summary>
        /// Identificador fiscal de la factura asociada
        /// </summary>
        public string UUID { get; set; }
        /// <summary>
        /// Identificador del Pago Efectuado a un bloque de facturas al que se asocia esta factura
        /// </summary>
        //Agregado para a partir de la version 3.2.1.1
        public int IdPagoCobranza { get; set; }
        /// <summary>
        /// Importe del Pago Total Efectuado a un bloque de facturas al que se asocia esta factura
        /// </summary>
        //Agregado para a partir de la version 3.2.1.1
        public decimal PagoTotal { get; set; }
        public decimal IVATotalCobrado { get; set; }
        public string Cliente { get; set; }
        public string Contrato { get; set; }
        public string IdInmob { get; set; }
        public string MonedaPago { get; set; }
        public int NumCopropietario { get; set; }

        public string RFC { get; set; }
       
    }
}
