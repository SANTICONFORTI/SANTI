using System;
using System.Collections.Generic;
using System.Threading;
using BLL;
using DomainModel;
using Services;

namespace UI
{
    class Program
    {
        private const int Capacidad = 2;
        private const int CantidadUsuarios = 5;
        private const int TrabajosPorUsuario = 3;

        public static void Escribir(string texto)
        {
            Log.Escribir(texto);
        }

        static void Main(string[] args)
        {
            Avisos avisos = new Avisos();
            avisos.Agregar(new AvisoMail());
            avisos.Agregar(new AvisoSms());
            avisos.Agregar(new AvisoWhatsapp());

            // Singleton: siempre la misma impresora.
            Impresora impresora = Impresora.Obtener();
            impresora.Configurar(Capacidad, avisos);

            List<Thread> usuarios = new List<Thread>();
            for (int i = 1; i <= CantidadUsuarios; i++)
            {
                Usuario usuario = new Usuario("Usuario " + i, impresora, TrabajosPorUsuario, i * 5);
                usuarios.Add(new Thread(usuario.Enviar));
            }

            Thread hiloImpresora = new Thread(Trabajar);
            hiloImpresora.Start();

            foreach (Thread hilo in usuarios)
            {
                hilo.Start();
            }

            foreach (Thread hilo in usuarios)
            {
                hilo.Join();
            }

            impresora.Cerrar();
            hiloImpresora.Join();

            int total = CantidadUsuarios * TrabajosPorUsuario;

            Console.WriteLine();
            Console.WriteLine("Enviados: " + impresora.Recibidos + " | Impresos: " + impresora.Impresos);
            Console.WriteLine("Avisos: " + impresora.AvisosEnviados + " en " + avisos.Cantidad + " medios");

            Comprobaciones.Verificar("llegaron todos los trabajos", impresora.Recibidos == total);
            Comprobaciones.Verificar("se imprimieron todos", impresora.Impresos == total);
            Comprobaciones.Verificar("salieron en el orden que llegaron", impresora.EnOrden);
            Comprobaciones.Verificar("se aviso por los tres medios", impresora.AvisosEnviados == total * 3);
            Comprobaciones.Verificar("la impresora es una sola", object.ReferenceEquals(impresora, Impresora.Obtener()));

            try
            {
                impresora.Imprimir(null);
                Comprobaciones.Verificar("avisa cuando no hay trabajo", false);
            }
            catch (InvalidOperationException)
            {
                Comprobaciones.Verificar("avisa cuando no hay trabajo", true);
            }

            Comprobaciones.Mostrar();
        }

        // El hilo de la impresora: saca y imprime hasta que la cierran.
        private static void Trabajar()
        {
            Impresora impresora = Impresora.Obtener();

            while (true)
            {
                Trabajo trabajo = impresora.ObtenerSiguienteTrabajo();

                if (trabajo == null)
                {
                    break;
                }

                impresora.Imprimir(trabajo);
            }

            Escribir("La impresora se apaga");
        }
    }
}
