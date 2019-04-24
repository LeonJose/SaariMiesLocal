using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class ProveedorEntity
    {
        public string IDProveedor { get; set; }
        public string Nombre { get; set; }
        public string RFC { get; set; }
        public int IDTipoProveedor { get; set; }
        public string TipoProveedor { get; set; }
        public string NombreExtranjero { get; set; }
        public string Nacionalidad { get; set; }
        public int IDTasaIVA { get; set; }
        public string DescripTasaIVA { get; set; }
        public decimal TasaIVA { get; set; }
        public string PaisResidencia { get; set; }

    }
}
