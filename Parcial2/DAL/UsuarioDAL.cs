using System.Collections.Generic;
using DomainModel;
using Services;

namespace DAL
{
    // Persistencia EN MEMORIA (la permite el enunciado)
    public class UsuarioDAL
    {
        private static readonly List<Usuario> usuarios = CargarDatos();

        public Usuario BuscarPorNombre(string nombre)
        {
            return usuarios.Find(u => u.NombreUsuario == nombre);
        }

        private static List<Usuario> CargarDatos()
        {
            // Patentes (funcionalidades simples)
            Patente altaUsuario = new Patente("Alta de usuario");
            Patente bajaUsuario = new Patente("Baja de usuario");
            Patente verBitacora = new Patente("Ver bitácora");
            Patente asignarPermisos = new Patente("Asignar permisos");
            Patente facturar = new Patente("Facturar");
            Patente verClientes = new Patente("Ver clientes");
            Patente reportes = new Patente("Reportes");

            // Familia dentro de otra familia
            Familia seguridad = new Familia("Seguridad");
            seguridad.Agregar(asignarPermisos);
            seguridad.Agregar(verBitacora);

            Familia administracion = new Familia("Administración");
            administracion.Agregar(altaUsuario);
            administracion.Agregar(bajaUsuario);
            administracion.Agregar(seguridad);

            Familia ventas = new Familia("Ventas");
            ventas.Agregar(facturar);
            ventas.Agregar(verClientes);

            Usuario admin = new Usuario { Id = 1, NombreUsuario = "admin", PasswordHash = Encriptador.Hash("123") };
            admin.Permisos.Add(administracion);
            admin.Permisos.Add(ventas);
            admin.Permisos.Add(reportes);

            Usuario vendedor = new Usuario { Id = 2, NombreUsuario = "vendedor", PasswordHash = Encriptador.Hash("123") };
            vendedor.Permisos.Add(ventas);

            return new List<Usuario> { admin, vendedor };
        }
    }
}
