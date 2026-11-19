using DAL;

namespace BLL
{
    // La pantalla pide por aca y no conoce el DAL.
    public static class FabricaBateria
    {
        public static SuscriptorBitacora CrearBitacora()
        {
            IRegistroBateria registro = new RegistroBateriaArchivo("Registros");
            return new SuscriptorBitacora(registro);
        }
    }
}
