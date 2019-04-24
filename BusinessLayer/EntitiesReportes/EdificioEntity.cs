using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public class EdificioEntity
    {
        private decimal valorMetroCuadrado = 0;

        public string ID { get; set; }
        public string Lote { get; set; }
        public string Manzana { get; set; }
        public decimal Terreno { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorPorMetro
        {
            get
            {
                if (Valor > 0 && Terreno > 0)
                    valorMetroCuadrado = Valor / Terreno;
                return valorMetroCuadrado;
            }
        }
        public string Tipo { get; set; }
        public string Identificador { get; set; }
        public string Nombre { get; set; }

        public string NombreInmuebleCompuesto
        {
            get {
                string nombre= Identificador+"-"+ Nombre;
                
                return nombre;
            
            }
        }
    }
}
