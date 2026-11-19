using System;
using DAL;
using DomainModel;
using Services;

namespace BLL
{
    public class UsuarioBLL
    {
        private readonly UsuarioDAL dal = new UsuarioDAL();

        public Usuario Login(string nombre, string password)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Complete usuario y contraseña.");

            Usuario u = dal.BuscarPorNombre(nombre);
            if (u == null || u.PasswordHash != Encriptador.Hash(password))
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");

            SesionManager.Instancia.Login(u);
            return u;
        }

        public void Logout()
        {
            SesionManager.Instancia.Logout();
        }
    }
}
