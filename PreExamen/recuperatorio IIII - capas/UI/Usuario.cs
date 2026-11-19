using System;
using System.Threading;
using BLL;

namespace UI
{
    // Cada usuario es un hilo que manda trabajos.
    public class Usuario
    {
        private static readonly string[] Archivos = { "Informe", "Remito", "Listado" };

        private readonly string nombre;
        private readonly Impresora impresora;
        private readonly int cantidad;
        private readonly Random azar;

        public Usuario(string nombre, Impresora impresora, int cantidad, int semilla)
        {
            this.nombre = nombre;
            this.impresora = impresora;
            this.cantidad = cantidad;
            azar = new Random(semilla);
        }

        public void Enviar()
        {
            for (int i = 0; i < cantidad; i++)
            {
                // Un rato entre trabajo y trabajo: el control de velocidad.
                Thread.Sleep(azar.Next(200, 500));

                impresora.RecibirTrabajo(nombre, Archivos[azar.Next(Archivos.Length)]);
            }

            Program.Escribir(nombre + " mando todo");
        }
    }
}
