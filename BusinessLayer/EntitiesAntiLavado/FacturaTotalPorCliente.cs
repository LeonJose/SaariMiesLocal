using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    public class FacturaTotalPorCliente
    {
        public string IDCliente { get; set; }
        public decimal TotalFacturas { get; set; }

        public FacturaTotalPorCliente(string cliente, decimal total)
        {
            IDCliente = cliente;
            TotalFacturas = total;
        }
    }
}
