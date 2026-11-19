using System;

namespace DomainModel
{
    // Se lanza cuando el plan del cliente no alcanza para el producto
    public class ProductoNoIncluidoException : Exception
    {
        public ProductoNoIncluidoException(string mensaje) : base(mensaje) { }
    }
}
