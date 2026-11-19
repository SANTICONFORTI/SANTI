using DomainModel;

namespace DAL
{
    // Lo que tiene que saber hacer el que guarda usuarios.
    public interface IUsuarioRepositorio
    {
        Usuario Buscar(string nombre);
    }
}
