using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class BitacoraEntity
    {
        public DateTime Fecha { get; set; }
        public string IdCliente { get; set; }
        public string cliente { get; set; }
        public string Contacto { get; set; }
        public string Asunto { get; set; }
        public byte[] Descripcion { get; set; }
        public string Usuario { get; set; }
        public string Etapa { get; set; }
        public string Estatus { get; set; }
        public string TipoEnte { get; set; }
        public DateTime ?FechaContactoInicial { get; set; }
        public DateTime ?FechaEsperadaDeCierre { get; set; }
        public Decimal ? ImporteDeOportunidad { get; set; }
        public DateTime ? FechaAgendaSiguiente { get; set; }
        public string DescripcionAgendaSiguiente { get; set; }
    }
}
