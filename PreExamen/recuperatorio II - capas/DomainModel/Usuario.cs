namespace DomainModel
{
    // Un usuario del sistema, con su clave y su perfil.
    public class Usuario
    {
        public Usuario(string nombre, string clave, Perfil perfil)
        {
            Nombre = nombre;
            Clave = clave;
            Perfil = perfil;
        }

        public string Nombre { get; set; }
        public string Clave { get; set; }
        public Perfil Perfil { get; set; }
    }
}
