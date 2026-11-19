using System;
using System.Threading;

namespace Parcial4
{
    // ---------------------------------------------------------------------
    // 6) USUARIO: cada uno es un HILO que envía una cantidad aleatoria
    //    de trabajos, esperando un tiempo aleatorio entre envíos.
    // ---------------------------------------------------------------------
    class Usuario
    {
        private static readonly string[] Tipos = { "SMS", "WhatsApp", "Email" };
        private static int ultimoNumero = 0;
        public static int TotalEnviados = 0;

        private readonly string nombre;
        private readonly Random rnd;

        public Usuario(string nombre, int semilla)
        {
            this.nombre = nombre;
            rnd = new Random(semilla);            // semilla distinta por usuario
        }

        public void Ejecutar()
        {
            int cantidad = rnd.Next(2, 5);        // número aleatorio de trabajos
            for (int i = 1; i <= cantidad; i++)
            {
                Thread.Sleep(rnd.Next(100, 400)); // tiempo aleatorio entre envíos
                TrabajoImpresion t = new TrabajoImpresion
                {
                    Numero = Interlocked.Increment(ref ultimoNumero),
                    Usuario = nombre,
                    Documento = $"Doc{i}.pdf",
                    TipoNotificacion = Tipos[rnd.Next(Tipos.Length)]
                };
                Impresora.Instancia.EnviarTrabajo(t);
                Interlocked.Increment(ref TotalEnviados);
            }
            Program.Log($"{nombre} terminó de enviar ({cantidad} trabajos)");
        }
    }
}
