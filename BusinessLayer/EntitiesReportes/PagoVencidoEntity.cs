using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class PagoVencidoEntity
    {
        public int IDHistRec { get; set; }
        public decimal Capital { get; set; }
        public decimal Intereses { get; set; }
        public decimal Moratorios { get; set; }
        public decimal TotalVencido { get { return Capital + Intereses + Moratorios; } }
        public DateTime FechaVencimiento { get; set; }
        public string TasaIVA { get; set; }
    }
}
