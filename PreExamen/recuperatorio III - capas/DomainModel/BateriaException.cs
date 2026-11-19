using System;

namespace DomainModel
{
    // Error propio del dominio de la bateria.
    public class BateriaException : Exception
    {
        public BateriaException(string mensaje) : base(mensaje)
        {
        }
    }
}
