using System;

namespace DomainModel
{
    // Error de login: usuario o clave que no coinciden.
    public class SeguridadException : Exception
    {
        public SeguridadException(string mensaje) : base(mensaje)
        {
        }
    }
}
