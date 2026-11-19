using System;
using System.IO;

namespace DAL
{
    // Guarda el registro en un archivo de texto, uno por dia.
    public class RegistroBateriaArchivo : IRegistroBateria
    {
        private readonly string carpeta;

        public RegistroBateriaArchivo(string carpeta)
        {
            this.carpeta = carpeta;
        }

        public void Anotar(string linea)
        {
            try
            {
                Directory.CreateDirectory(carpeta);

                // La fecha va en el nombre: la rotacion sale sola.
                string archivo = "bitacora_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                string ruta = Path.Combine(carpeta, archivo);

                File.AppendAllText(ruta, linea + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo escribir el registro: " + ex.Message);
            }
        }
    }
}
