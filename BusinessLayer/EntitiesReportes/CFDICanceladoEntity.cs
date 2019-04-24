using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class CFDICanceladoEntity
    {
        public List<ReciboEntity> RecibosCanceladosPesos { get; set; }
        public List<ReciboEntity> RecibosCanceladosDolares{ get; set; }
        public decimal ImportePesos { get; set; }
        public decimal IVAPesos { get; set; }
        public decimal TotalPesos { get; set; }
        public decimal ImporteDolares { get; set; }
        public decimal IVADolares { get; set; }
        public decimal TotalDolares { get; set; }
        public decimal ImporteDolaresConvertidos { get; set; }
        public decimal IVADolaresConvertidos { get; set; }
        public decimal TotalDolaresConvertidos { get; set; }
        public decimal TotalImporte { get { return ImportePesos + ImporteDolaresConvertidos; } }
        public decimal TotalIVA { get { return IVAPesos + IVADolaresConvertidos; } }
        public decimal Total { get { return TotalPesos + TotalDolaresConvertidos; } }        
    }
}
