using DAL;

namespace BLL
{
    // La pantalla pide por aca y no conoce el DAL.
    public static class FabricaSeguridad
    {
        public static SeguridadServicio Crear()
        {
            IUsuarioRepositorio repositorio = new UsuarioRepositorioMemoria();
            return new SeguridadServicio(repositorio);
        }
    }
}
