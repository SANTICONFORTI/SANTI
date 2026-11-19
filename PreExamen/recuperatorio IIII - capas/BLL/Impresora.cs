using System;
using System.Collections.Generic;
using System.Threading;
using DomainModel;
using Services;

namespace BLL
{
    // La impresora de la oficina: una sola para todos.
    public class Impresora
    {
        // Lazy crea la instancia la primera vez que alguien la pide.
        // El delegado deja usar el constructor privado.
        private static readonly Lazy<Impresora> unica =
            new Lazy<Impresora>(() => new Impresora());

        private readonly Queue<Trabajo> cola = new Queue<Trabajo>();
        private readonly object candado = new object();

        private Avisos avisos;
        private int capacidad;
        private bool cerrada;
        private int recibidos;
        private int impresos;
        private int anterior;
        private bool enOrden;

        private Impresora()
        {
            enOrden = true;
        }

        public static Impresora Obtener()
        {
            return unica.Value;
        }

        // Se configura una sola vez, al arrancar el programa.
        public void Configurar(int capacidad, Avisos avisos)
        {
            this.capacidad = capacidad;
            this.avisos = avisos;
        }

        public int Recibidos
        {
            get { return recibidos; }
        }

        public int Impresos
        {
            get { return impresos; }
        }

        public bool EnOrden
        {
            get { return enOrden; }
        }

        public int AvisosEnviados
        {
            get { return avisos.Enviados; }
        }

        // Lo llama un usuario. Si la cola esta llena, espera.
        public void RecibirTrabajo(string usuario, string archivo)
        {
            bool tomado = false;
            try
            {
                Monitor.Enter(candado, ref tomado);

                // while y no if: cuando despierta hay que volver a mirar.
                while (cola.Count == capacidad)
                {
                    Log.Escribir(usuario + " espera, la cola esta llena");
                    Monitor.Wait(candado);
                }

                recibidos = recibidos + 1;
                cola.Enqueue(new Trabajo(recibidos, usuario, archivo));

                Log.Escribir(usuario + " manda " + archivo);

                // Hay algo nuevo: despierto a la impresora.
                Monitor.PulseAll(candado);
            }
            finally
            {
                if (tomado)
                {
                    Monitor.Exit(candado);
                }
            }
        }

        // Lo llama el hilo de la impresora.
        public Trabajo ObtenerSiguienteTrabajo()
        {
            bool tomado = false;
            try
            {
                Monitor.Enter(candado, ref tomado);

                while (cola.Count == 0 && !cerrada)
                {
                    Monitor.Wait(candado);
                }

                if (cola.Count == 0)
                {
                    return null;
                }

                Trabajo trabajo = cola.Dequeue();

                // Se fue uno: despierto a los usuarios que esperan lugar.
                Monitor.PulseAll(candado);

                return trabajo;
            }
            finally
            {
                if (tomado)
                {
                    Monitor.Exit(candado);
                }
            }
        }

        public void Imprimir(Trabajo trabajo)
        {
            if (trabajo == null)
            {
                throw new InvalidOperationException("No hay ningun trabajo para imprimir.");
            }

            Log.Escribir("Imprimiendo " + trabajo);

            // Lo que tarda la impresion va afuera del candado.
            Thread.Sleep(600);

            if (trabajo.Numero < anterior)
            {
                enOrden = false;
            }

            anterior = trabajo.Numero;
            impresos = impresos + 1;

            Log.Escribir("Salio " + trabajo);

            // La impresora no sabe por que medios se avisa.
            avisos.EnviarATodos(trabajo.Usuario, trabajo.Archivo);
        }

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
                if (tomado)
                {
                    Monitor.Exit(candado);
                }
            }
        }
    }
}
