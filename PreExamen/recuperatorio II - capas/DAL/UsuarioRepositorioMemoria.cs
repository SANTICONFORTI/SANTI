using System.Collections.Generic;
using DomainModel;

namespace DAL
{
    // Guarda los usuarios en memoria, con dos de ejemplo y perfiles distintos.
    public class UsuarioRepositorioMemoria : IUsuarioRepositorio
    {
        private readonly List<Usuario> usuarios = new List<Usuario>();

        public UsuarioRepositorioMemoria()
        {
            // Perfil de administracion: puede ver todo.
            Perfil administrador = new Perfil("Administrador");
            administrador.Permitir("Ventas - Nuevo presupuesto");
            administrador.Permitir("Ventas - Listado");
            administrador.Permitir("Ventas - Anular");
            administrador.Permitir("Stock - Ingreso");
            administrador.Permitir("Stock - Ajuste");
            administrador.Permitir("Usuarios - Alta");
            administrador.Permitir("Usuarios - Baja");
            administrador.Permitir("Administracion - Copia de seguridad");

            // Perfil de vendedor: solo algunas cosas de ventas y una de stock.
            Perfil vendedor = new Perfil("Vendedor");
            vendedor.Permitir("Ventas - Nuevo presupuesto");
            vendedor.Permitir("Ventas - Listado");
            vendedor.Permitir("Stock - Ingreso");

            usuarios.Add(new Usuario("dante", "1234", administrador));
            usuarios.Add(new Usuario("santino", "1234", vendedor));
        }

        public Usuario Buscar(string nombre)
        {
            foreach (Usuario usuario in usuarios)
            {
                if (usuario.Nombre == nombre)
                {
                    return usuario;
                }
            }

            return null;
        }
    }
}
