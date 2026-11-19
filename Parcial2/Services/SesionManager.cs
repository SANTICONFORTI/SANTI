using System;
using DomainModel;

namespace Services
{
    // PATRÓN SINGLETON
    // Una sola sesión en toda la aplicación.
    public class SesionManager
    {
        private static SesionManager instancia;
        private static readonly object candado = new object();

        private SesionManager() { }

        public static SesionManager Instancia
        {
            get
            {
                lock (candado)
                {
                    if (instancia == null)
                        instancia = new SesionManager();
                    return instancia;
                }
            }
        }

        public Usuario Usuario { get; private set; }

        public bool EstaLogueado
        {
            get { return Usuario != null; }
        }

        public void Login(Usuario u)
        {
            if (EstaLogueado)
                throw new InvalidOperationException("Ya hay una sesión iniciada.");
            Usuario = u;
        }

        public void Logout()
        {
            Usuario = null;
        }
    }
}
