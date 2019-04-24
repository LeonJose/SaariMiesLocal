using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class RentRollEntity
    {
        public string IDContrato { get; set; }
        public int IDHistRec { get; set; }
        public string IdCliente { get; set; }
        public string NombreInmueble { get; set; }
        public string NombreCliente { get; set; }
        public decimal MetrosCuadradosConstruccion { get; set; }
        public decimal ImporteRentaEmitido { get; set; }        
        public decimal ImporteMantenimientoEmitido { get; set; }
        public decimal RentasAnticipadasEmitidas { get; set; }
        public decimal OtrosEmitidos { get; set; }
        public decimal DescuentoEmitido { get; set; }
        public decimal TotalEmitido { get { return ImporteRentaEmitido + ImporteMantenimientoEmitido + RentasAnticipadasEmitidas + OtrosEmitidos + ImportePublicidadEmitido + ImporteServiciosEmitido + ImpuestoIVA; } } //Add ImpuestoIVA JL | 18/10/2018
        public decimal ImporteRentaCobrado { get; set; }
        public decimal ImporteMantenimientoCobrado { get; set; }
        public decimal RentaVariableCobrado { get; set; }
        public decimal OtrosCobrado { get; set; }
        public decimal TotalCobrado { get { return ImporteRentaCobrado + ImporteMantenimientoCobrado + RentaVariableCobrado + OtrosCobrado; } }
        public string CuentaBancaria { get; set; }
        public int Estatus { get; set; }
        public decimal CantidadPorPagar { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaPagado { get; set; }
        public decimal TotalCargosPeriodicos { get; set; }
        public decimal Importe { get; set; }
        public int TipoCargo { get; set; }
        public decimal RentaVariableEmitida { get; set; }
        public string TipoRecibo { get; set; }
        public bool EsRentaAnticipada { get; set; }
        public int IDHistRelacionado { get; set; }
        public string NombreClienteComercial { get; set; }
        //Agregados 19/08/2015 by Ing. Uzcanga
        public string IdInmueble { get; set; }
        public decimal MetrosCuadradosDisponibles { get; set; }
        public decimal PrecioMetroCuadrado { get; set; }
        public string InmuebleDisponible { get; set; }
        public decimal ImportePublicidadEmitido { get; set; }
        public decimal ImportePublicidadCobrado { get; set; }
        public decimal ImporteServiciosEmitido { get; set; }
        public decimal ImporteServiciosCobrado { get; set; }
        public string ClasificacionUbicacion { get; set; }
        public string Moneda { get; set; }
        public string MonedaID { get; set; }
        public decimal TipoDeCambioEmitido { get; set; }
        public decimal TotalFacturado { get; set; }
        //Add JL | 17/10/2018
        public decimal PrecioMantoEntreMts { get; set; }
        public decimal PrecioPubEntreMts { get; set; }
        public decimal ImpuestoIVA { get; set; }
        public decimal IngresoAntImpuesto { get; set; }

        public string IdSubconjunto { get; set; }
        public string NombreSubconjunto { get; set; }
        public string IdEdificio { get; set; }
        public string IdCentroSubConjunto { get; set; }
    }
}
