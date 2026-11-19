using System;

namespace BLL
{
    public class AvisoSms : IAviso
    {
        public void Avisar(string usuario, string archivo)
        {
            Console.WriteLine("   sms a " + usuario + ": ya salio " + archivo);
        }
    }
}
