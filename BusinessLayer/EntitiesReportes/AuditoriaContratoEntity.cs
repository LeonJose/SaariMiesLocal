using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class AuditoriaContratoEntity
    {
        public string IDContrato { get; set; }
        public string IDEdificio { get; set; }
        public string IDConjunto { get; set; }
        public string IDCliente { get; set; }
        public string Desarrollo { get; set; }
        public string Seccion { get; set; }
        public DateTime FechaContrato { get; set; }
        public string NombreInmueble { get; set; }
        public string Manzana { get; set; }
        public string Lote { get; set; }
        public string Cliente { get; set; }
        public string Mail { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public decimal Superficie { get; set; }
        public decimal Precio
        {
            get
            {
                return Superficie != 0 ? PrecioTotal / Superficie : 0;
            }
        }
        public decimal PrecioTotal { get; set; }
        public decimal Enganche { get; set; }
        public decimal SaldoAFinanciar { get { return PrecioTotal - Enganche; } }
        public decimal PagoMensual { get; set; }
        public decimal MontoPagadoCapital { get; set; }
        public decimal MontoPagadoIntereses { get; set; }
        public decimal MontoPagadoMoratorios { get; set; }
        public decimal SaldoInsoluto { get; set; }
        public decimal SaldoVencidoCapital { get; set; }
        public decimal SaldoVencidoInteres { get; set; }
        public decimal SaldoVencidoMoratorios { get; set; }
        public decimal MontoParaRegularizar { get; set; }
        public int PagaresPagados { get; set; }
        public int PagaresPorPagar { get; set; }
        public decimal MetrosCuadradosTerreno { get; set; }
        public decimal MetrosCuadradosConstruccion { get; set; }
        public decimal PrecioPorMetroCuadradoTerreno { get { return MetrosCuadradosTerreno != 0 ? PrecioTotal / MetrosCuadradosTerreno : 0; } }
        public decimal PrecioPorMetroCuadradoConstruccion { get { return MetrosCuadradosConstruccion != 0 ? PrecioTotal / MetrosCuadradosConstruccion : 0; } }
    }
}
