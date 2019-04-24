using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class SaldoAFavorEntity
    {

        public int IdSaldo { get; set; }
        public int IdPago { get; set; }
        public string IdCliente { get; set; }
        public decimal ImporteSaldo { get; set; }
        public decimal SumaSaldoFavor { get; set; }
        public int ReciboPagadoIDPago { get; set; }
        public int SaldoFavor { get; set; }
        public decimal totalPagado { get; set; }
        public decimal totalSaldoFavor { get; set; }
        public int IdHistRec { get; set; } 

    }
}
