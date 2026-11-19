using System;
using DomainModel;

namespace UI
{
    // Muestra la bateria en pantalla cada vez que cambia.
    public class SuscriptorVisual : IEscuchaBateria
    {
        public void Actualizar(Bateria bateria)
        {
            string estado;

            if (bateria.Conectado)
            {
                estado = "cargando";
            }
            else
            {
                estado = "sin cargador";
            }

            Console.WriteLine("Bateria " + estado + ": " + bateria.Carga + "%");

            if (bateria.Conectado)
            {
                Console.WriteLine("   faltan " + bateria.TiempoCarga + " minutos para el 100%");
            }
            else
            {
                Console.WriteLine("   quedan " + bateria.TiempoUso + " minutos de uso");
            }
        }
    }
}
