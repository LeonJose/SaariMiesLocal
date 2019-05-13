using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class CuentaPorCobrarEntity
    {
        public string IDCliente { get; set; }
        public string RazonSocialCliente { get; set; }
        public string NombreComercialCliente { get; set; }
        public decimal TotalCartera { get { return Total30 + Total60 + Total90 + TotalMas90; } }
        public decimal Total30 { get; set; }
        public decimal Total60 { get; set; }
        public decimal Total90 { get; set; }
        public decimal TotalMas90 { get; set; }
        public List<ReciboEntity> Recibos30 { get; set; }
        public List<ReciboEntity> Recibos60 { get; set; }
        public List<ReciboEntity> Recibos90 { get; set; }
        public List<ReciboEntity> RecibosMas90 { get; set; }
        public List<ReciboEntity> Recibos { get { return Recibos30.Union(Recibos60).ToList().Union(Recibos90).ToList().Union(RecibosMas90).ToList().OrderBy(x => x.Folio).ToList(); } }
    }
}
