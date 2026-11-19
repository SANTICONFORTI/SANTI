using System;
using System.Collections.Generic;

namespace DomainModel
{
    // PATRÓN COMPOSITE - Hoja
    // Funcionalidad simple / atómica: una opción de menú.
    public class Patente : Componente
    {
        public Patente(string nombre)
        {
            Nombre = nombre;
        }

        public override List<Componente> Hijos
        {
            get { return new List<Componente>(); }
        }

        public override void Agregar(Componente c)
        {
            throw new InvalidOperationException("Una patente no puede tener hijos.");
        }

        public override bool TienePermiso(string nombre)
        {
            return Nombre == nombre;
        }
    }
}
