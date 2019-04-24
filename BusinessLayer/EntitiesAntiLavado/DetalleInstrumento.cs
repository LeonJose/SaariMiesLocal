using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    class DetalleInstrumento
    {
        public Cheque Cheque { get; set; }
        public TransferenciaInterbancaria TransferenciaInterbancaria { get; set; }
        public TransferenciaMismoBanco TransferenciaDelMismoBanco { get; set; }
        public TransferenciaInternacional TransferenciaInternacional { get; set; }
    }
}
