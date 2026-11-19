using System;

namespace DomainModel
{
    // Error propio del modulo de seguridad.
    public class FuncionalidadException : Exception
    {
        public FuncionalidadException(string mensaje) : base(mensaje)
        {
        }
    }
}
