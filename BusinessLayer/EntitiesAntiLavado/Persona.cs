using System;
using System.Collections.Generic;
using System.Text;

namespace GestorReportes.BusinessLayer.EntitiesAntiLavado
{
    class Persona
    {
        public bool EsPersonaFisica { get; set; }
        public PersonaFisica Fisica { get; set; }
        public PersonaMoral Moral { get; set; }
        public Domicilio DomicilioNacional { get; set; }
        public Telefono Telefono { get; set; }
    }
}
