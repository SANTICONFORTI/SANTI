// =====================================================================
//  PARCIAL 4 - Sistema de impresión concurrente con control de velocidad
//  Proyecto: Aplicación de consola (.NET Framework 4.8)
//
//  PATRONES USADOS (y la frase del enunciado que los pide):
//   - SINGLETON  -> "asegurar que solo haya una instancia de Impresora"
//   - PRODUCTOR-CONSUMIDOR con MONITOR -> usuarios (productores) envían
//                  trabajos a una cola; la impresora (consumidor) los saca.
//   - CONTROL DE VELOCIDAD -> la cola tiene CAPACIDAD máxima: si está
//                  llena, el usuario ESPERA hasta que le avisen.
//   - STRATEGY + FACTORY -> notificaciones (SMS, WhatsApp, Email):
//                  "el consumidor no conoce las clases concretas ni el
//                  algoritmo". La impresora solo conoce INotificador.
//
//  Es el MISMO esqueleto que el ejercicio de la cocina:
//     Mozo -> Usuario | Bandeja -> cola de la Impresora | Cocinero -> Impresora
// =====================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Parcial4
{
    // ---------------------------------------------------------------------
    // 7) PROGRAMA + CASOS DE PRUEBA
    // ---------------------------------------------------------------------
    class Program
    {
        private static readonly Stopwatch reloj = Stopwatch.StartNew();
        private static readonly object candadoConsola = new object();

        public static void Log(string mensaje)
        {
            lock (candadoConsola)
                Console.WriteLine($"{reloj.Elapsed.TotalSeconds,5:0.00}s  {mensaje}");
        }

        static void Main()
        {
            // PRUEBA 1: Singleton -> siempre la misma instancia
            Console.WriteLine("PRUEBA 1 - Singleton: " +
                (Impresora.Instancia == Impresora.Instancia ? "OK, misma instancia" : "ERROR"));

            // PRUEBA 2: imprimir con la cola vacía -> excepción controlada
            try
            {
                Impresora.Instancia.ObtenerSiguienteTrabajo(false);
                Console.WriteLine("PRUEBA 2 - ERROR: no lanzó excepción");
            }
            catch (ColaVaciaException ex)
            {
                Console.WriteLine("PRUEBA 2 - Cola vacía: OK -> " + ex.Message);
            }

            // PRUEBA 3: tipo de notificación inválido -> excepción controlada
            try
            {
                FabricaNotificadores.Crear("Paloma");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("PRUEBA 3 - Notificación inválida: OK -> " + ex.Message);
            }
            Console.WriteLine();

            // PRUEBA 4: concurrencia con varios usuarios y una impresora
            Usuario[] usuarios =
            {
                new Usuario("Ana  ", 1),
                new Usuario("Beto ", 2),
                new Usuario("Carla", 3),
                new Usuario("Dani ", 4)
            };

            List<Thread> hilosUsuarios = new List<Thread>();
            foreach (Usuario u in usuarios) hilosUsuarios.Add(new Thread(u.Ejecutar));
            Thread hiloImpresora = new Thread(Impresora.Instancia.Procesar);

            hiloImpresora.Start();
            hilosUsuarios.ForEach(h => h.Start());

            hilosUsuarios.ForEach(h => h.Join());   // esperamos a los usuarios...
            Impresora.Instancia.Cerrar();           // ...y cerramos la cola
            hiloImpresora.Join();

            Console.WriteLine();
            Console.WriteLine($"PRUEBA 4 - Enviados: {Usuario.TotalEnviados} | Impresos: {Impresora.Instancia.Impresos} | " +
                (Usuario.TotalEnviados == Impresora.Instancia.Impresos ? "OK" : "ERROR: se perdieron trabajos"));

            Console.ReadKey();
        }
    }
}
