using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesPolizas
{
    class MovimientoExcelEntity
    {
        public int Consecutivo { get; set; }
        public int Contador { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Cuenta { get; set; }
        public string DescripcionCuenta { get; set; }
        public decimal Cargo { get; set; }
        public decimal Abono { get; set; }
        public string Descripcion { get; set; }
        public int NumeroRecibo { get; set; }
        public string SerieFolio { get; set; }
        public string Estatus { get; set; }
        public string ClienteRazonSocial { get; set; }
        public string ClienteRFC { get; set; }
        public string InmobiliariaRazonSocial { get; set; }
        public string InmobiliariaRFC { get; set; }
        public string Conjunto { get; set; }
        public string Inmueble { get; set; }
        public bool Excluir { get; set; }
        public string Tipo { get; set; }
    }
}
