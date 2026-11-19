using System.Collections.Generic;

namespace DomainModel
{
    // El perfil dice a que funcionalidades accede el usuario.
    // Guarda los nombres de las funcionalidades simples.
    public class Perfil
    {
        private readonly List<string> permitidas = new List<string>();

        public Perfil(string nombre)
        {
            Nombre = nombre;
        }

        public string Nombre { get; set; }

        public List<string> Permitidas
        {
            get { return permitidas; }
        }

        public void Permitir(string funcionalidad)
        {
            permitidas.Add(funcionalidad);
        }

        public bool Puede(string funcionalidad)
        {
            return permitidas.Contains(funcionalidad);
        }
    }
}
