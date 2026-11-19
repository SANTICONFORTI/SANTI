using System;
using System.Threading;
using BLL;
using DomainModel;

namespace UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Bateria bateria = new Bateria();
            bateria.Carga = 45;

            SuscriptorVisual visual = new SuscriptorVisual();
            SuscriptorBitacora bitacora = FabricaBateria.CrearBitacora();

            bateria.Agregar(visual);
            bateria.Agregar(bitacora);

            Console.WriteLine("Enchufo el cargador");
            bateria.Conectado = true;
            Thread.Sleep(1000);

            Console.WriteLine("Desenchufo");
            bateria.Conectado = false;
            Thread.Sleep(1000);

            // Muestro que se puede dar de baja a uno de los dos.
            bateria.Quitar(visual);
            Console.WriteLine("Saco la pantalla: el proximo aviso va solo al registro");

            bateria.Conectado = true;
            Thread.Sleep(1000);

            Console.WriteLine();
            Console.WriteLine("Pruebo la validacion de la carga");

            try
            {
                bateria.Carga = 150;
            }
            catch (BateriaException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Fin. Revisar la carpeta Registros.");
        }
    }
}
