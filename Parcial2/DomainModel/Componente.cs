using System.Collections.Generic;

namespace DomainModel
{
    // PATRÓN COMPOSITE - Componente
    // Clase base común: una Patente (hoja) y una Familia (compuesto)
    // se tratan de la misma forma.
    public abstract class Componente
    {
        public string Nombre { get; set; }

        public abstract List<Componente> Hijos { get; }

        public abstract void Agregar(Componente c);

        // ¿Este componente (o alguno de adentro) es el permiso buscado?
        public abstract bool TienePermiso(string nombre);
    }
}
