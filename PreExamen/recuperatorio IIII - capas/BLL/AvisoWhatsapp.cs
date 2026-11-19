using System;

namespace BLL
{
    public class AvisoWhatsapp : IAviso
    {
        public void Avisar(string usuario, string archivo)
        {
            Console.WriteLine("   whatsapp a " + usuario + ": ya salio " + archivo);
        }
    }
}
