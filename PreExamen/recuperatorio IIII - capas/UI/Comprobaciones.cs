using System;

namespace UI
{
    // Anota las comprobaciones del final.
    public class Comprobaciones
    {
        private static int bien;
        private static int mal;

        public static void Verificar(string detalle, bool condicion)
        {
            if (condicion)
            {
                bien = bien + 1;
                Console.WriteLine("correcto  " + detalle);
            }
            else
            {
                mal = mal + 1;
                Console.WriteLine("FALLO     " + detalle);
            }
        }

        public static void Mostrar()
        {
            Console.WriteLine();
            Console.WriteLine("Comprobaciones: " + bien + " correctas, " + mal + " con falla");
        }
    }
}
