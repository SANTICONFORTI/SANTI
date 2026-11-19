using System;
using System.Collections.Generic;

namespace DomainModel
{
    // PATRÓN COMPOSITE - Compuesto
    // Funcionalidad compuesta: conjunto de patentes y/o de otras familias.
    public class Familia : Componente
    {
        private readonly List<Componente> hijos = new List<Componente>();

        public Familia(string nombre)
        {
            Nombre = nombre;
        }

        public override List<Componente> Hijos
        {
            get { return hijos; }
        }

        public override void Agregar(Componente c)
        {
            if (c == this)
                throw new InvalidOperationException("Una familia no puede contenerse a sí misma.");
            hijos.Add(c);
        }

        // Recursivo: pregunta a cada hijo, sea hoja o compuesto
        public override bool TienePermiso(string nombre)
        {
            if (Nombre == nombre) return true;
            foreach (Componente h in hijos)
                if (h.TienePermiso(nombre)) return true;
            return false;
        }
    }
}
