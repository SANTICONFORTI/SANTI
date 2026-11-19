using System;

namespace Parcial4
{
    // ---------------------------------------------------------------------
    // 2) EXCEPCIÓN PROPIA: "intentar imprimir cuando no hay trabajos"
    // ---------------------------------------------------------------------
    class ColaVaciaException : Exception
    {
        public ColaVaciaException(string mensaje) : base(mensaje) { }
    }
}
