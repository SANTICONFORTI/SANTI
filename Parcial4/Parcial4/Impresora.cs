using System;
using System.Collections.Generic;
using System.Threading;

namespace Parcial4
{
    // ---------------------------------------------------------------------
    // 5) IMPRESORA: SINGLETON + recurso compartido sincronizado con MONITOR
    // ---------------------------------------------------------------------
    class Impresora
    {
        // ---- SINGLETON ----
        private static Impresora instancia;
        private static readonly object candadoInstancia = new object();

        // Constructor PRIVADO: nadie puede hacer "new Impresora()" desde afuera
        private Impresora() { }

        public static Impresora Instancia
        {
            get
            {
                lock (candadoInstancia)          // por si dos hilos la piden a la vez
                {
                    if (instancia == null)
                        instancia = new Impresora();
                    return instancia;
                }
            }
        }

        // ---- COLA COMPARTIDA ----
        private const int CAPACIDAD = 3;          // control de velocidad
        private readonly Queue<TrabajoImpresion> cola = new Queue<TrabajoImpresion>();
        private readonly object candado = new object();
        private bool cerrada = false;
        public int Impresos { get; private set; }

        // PRODUCTOR: la llama cada USUARIO.
        // Si la cola está llena, el usuario espera (control de velocidad).
        public void EnviarTrabajo(TrabajoImpresion t)
        {
            bool tomado = false;
            try
            {
                Monitor.Enter(candado, ref tomado);

                // WHILE y no IF: al despertar se vuelve a verificar
                while (cola.Count == CAPACIDAD)
                {
                    Program.Log($"{t.Usuario} espera: cola llena {Ocupacion()}");
                    Monitor.Wait(candado);        // suelta el lock y duerme
                }

                cola.Enqueue(t);                  // Queue = FIFO -> respeta el orden
                Program.Log($"{t.Usuario} envía {t} {Ocupacion()}");
                Monitor.PulseAll(candado);        // despierta a la impresora
            }
            finally
            {
                if (tomado) Monitor.Exit(candado);
            }
        }

        // CONSUMIDOR: saca el siguiente trabajo de la cola.
        //  esperar = true  -> si está vacía, duerme hasta que llegue algo.
        //  esperar = false -> si está vacía, lanza ColaVaciaException.
        // Devuelve null cuando la cola está cerrada y vacía (fin del trabajo).
        public TrabajoImpresion ObtenerSiguienteTrabajo(bool esperar = true)
        {
            bool tomado = false;
            try
            {
                Monitor.Enter(candado, ref tomado);

                if (!esperar && cola.Count == 0)
                    throw new ColaVaciaException("No hay trabajos en la cola para imprimir.");

                while (cola.Count == 0 && !cerrada)
                    Monitor.Wait(candado);

                if (cola.Count == 0)              // vacía y cerrada
                    return null;

                TrabajoImpresion t = cola.Dequeue();
                Monitor.PulseAll(candado);        // avisa a usuarios: hay lugar
                return t;
            }
            finally
            {
                if (tomado) Monitor.Exit(candado);
            }
        }

        // Simula la impresión (FUERA del lock, para no frenar a los usuarios)
        public void Imprimir(TrabajoImpresion t)
        {
            if (t == null)
                throw new ColaVaciaException("No hay trabajo para imprimir.");

            Program.Log($"IMPRESORA imprimiendo {t}...");
            Thread.Sleep(400);
            Impresos++;                           // solo la toca el hilo de la impresora
            Program.Log($"IMPRESORA terminó {t}");

            // La impresora NO sabe qué clase concreta notifica
            INotificador notificador = FabricaNotificadores.Crear(t.TipoNotificacion);
            notificador.Notificar(t);
        }

        // Lo que ejecuta el hilo de la impresora
        public void Procesar()
        {
            TrabajoImpresion t;
            while ((t = ObtenerSiguienteTrabajo()) != null)
                Imprimir(t);
            Program.Log($"IMPRESORA apagada: imprimió {Impresos} trabajos");
        }

        // Main la llama cuando todos los usuarios terminaron
        public void Cerrar()
        {
            bool tomado = false;
            try
            {
                Monitor.Enter(candado, ref tomado);
                cerrada = true;
                Monitor.PulseAll(candado);
            }
            finally
            {
                if (tomado) Monitor.Exit(candado);
            }
        }

        // Se llama siempre con el lock tomado
        private string Ocupacion()
        {
            return "[" + new string('#', cola.Count) + new string('.', CAPACIDAD - cola.Count) + "]";
        }
    }
}
