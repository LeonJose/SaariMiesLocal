using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntilavado2
{
    public class Persona
    {
        public bool EsPersonaFisica { get; set; }
        public PersonaFisica Fisica { get; set; }
        public PersonaMoral Moral { get; set; }
        public Domicilio DomicilioNacional { get; set; }
        public Telefono Telefono { get; set; }
    }
}
