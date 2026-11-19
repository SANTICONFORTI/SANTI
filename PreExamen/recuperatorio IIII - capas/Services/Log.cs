using System;

namespace Services
{
    // Escribe en consola con la hora. Lo usan la impresora (BLL) y la UI.
    public static class Log
    {
        private static readonly object candado = new object();

        public static void Escribir(string texto)
        {
            lock (candado)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "  " + texto);
            }
        }
    }
}
