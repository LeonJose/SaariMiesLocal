using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class mantGastosEntity
    {
        public DateTime Fecha { get; set; }
        public string Asunto { get; set; }
        public string NombreInmo { get; set; }
        public string Proveedor { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string TipoMoneda { get; set; }
        public decimal TipoCambio { get; set; }
        public string iDinmueble { get; set; }
        public string tipoNombre { get; set; }
        public byte[] descripcion { get; set; }
        public string clasificacion { get; set; }
        public string Usuarios { get; set; }
        public decimal subTotalDolares { get; set; }
        public decimal IvalDolares { get; set; }
        public decimal TotalDolares { get; set; }
        public string incluirInmuebles { get; set; }


    }
}
