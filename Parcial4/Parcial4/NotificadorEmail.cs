using System;

namespace Parcial4
{
    class NotificadorEmail : INotificador
    {
        public void Notificar(TrabajoImpresion t)
        {
            Program.Log($"   [Email] a {t.Usuario}: impresión #{t.Numero} finalizada");
        }
    }
}
