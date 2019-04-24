using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class TotalesRecibosExpedidosEntity
    {
        public decimal ImporteDolares { get; set; }
        public decimal IVADolares { get; set; }
        public decimal ISRDolares { get; set; }
        public decimal IVARetenidoDolares { get; set; }
        public decimal TotalDolares { get; set; }
        public decimal ImportePesos { get; set; }
        public decimal IVAPesos { get; set; }
        public decimal ISRPesos { get; set; }
        public decimal IVARetenidoPesos { get; set; }
        public decimal TotalPesos { get; set; }
        public decimal ImporteDolaresAPesos { get; set; }
        public decimal IVADolaresAPesos { get; set; }
        public decimal ISRDolaresAPesos { get; set; }
        public decimal IVARetenidoDolaresAPesos { get; set; }
        public decimal TotalDolaresAPesos { get; set; }
        public decimal TotalImporte { get { return ImporteDolaresAPesos + ImportePesos; } }
        public decimal TotalIVA { get { return IVADolaresAPesos + IVAPesos; } }
        public decimal TotalISR { get { return ISRDolaresAPesos + ISRPesos; } }
        public decimal TotalIVARetenido { get { return IVARetenidoDolaresAPesos + IVARetenidoPesos; } }
        public decimal Total { get { return TotalDolaresAPesos + TotalPesos; } }
        public decimal TotalConceptoPesos { get; set; }

    }
}
