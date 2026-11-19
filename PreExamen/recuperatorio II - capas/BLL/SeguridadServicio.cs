using DAL;
using DomainModel;

namespace BLL
{
    // Login y logout de los usuarios.
    public class SeguridadServicio
    {
        private readonly IUsuarioRepositorio repositorio;

        public SeguridadServicio(IUsuarioRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public Sesion SesionActual { get; private set; }

        public Sesion IniciarSesion(string nombre, string clave)
        {
            Usuario usuario = repositorio.Buscar(nombre);

            if (usuario == null)
            {
                throw new SeguridadException("El usuario no existe.");
            }

            if (usuario.Clave != clave)
            {
                throw new SeguridadException("La clave no es correcta.");
            }

            SesionActual = new Sesion(usuario);
            return SesionActual;
        }

        public void CerrarSesion()
        {
            if (SesionActual == null)
            {
                throw new SeguridadException("No hay ninguna sesion abierta.");
            }

            SesionActual = null;
        }
    }
}
