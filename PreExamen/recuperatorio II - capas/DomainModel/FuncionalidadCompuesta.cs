using System.Collections.Generic;

namespace DomainModel
{
    // Funcionalidad compuesta: es un conjunto de funcionalidades,
    // que pueden ser simples o compuestas.
    public class FuncionalidadCompuesta : Funcionalidad
    {
        private readonly List<Funcionalidad> hijos = new List<Funcionalidad>();

        public FuncionalidadCompuesta(string nombre)
        {
            Nombre = nombre;
        }

        public override void Agregar(Funcionalidad funcionalidad)
        {
            hijos.Add(funcionalidad);
        }

        public override void Quitar(Funcionalidad funcionalidad)
        {
            hijos.Remove(funcionalidad);
        }

        public override List<Funcionalidad> ObtenerHijos()
        {
            return hijos;
        }

        public override bool TieneHijos()
        {
            return hijos.Count > 0;
        }
    }
}
