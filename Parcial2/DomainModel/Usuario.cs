using System.Collections.Generic;

namespace DomainModel
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }

        // Perfil del usuario: sus familias y/o patentes
        public List<Componente> Permisos { get; set; }

        public Usuario()
        {
            Permisos = new List<Componente>();
        }

        public bool TienePermiso(string nombre)
        {
            foreach (Componente c in Permisos)
                if (c.TienePermiso(nombre)) return true;
            return false;
        }
    }
}
