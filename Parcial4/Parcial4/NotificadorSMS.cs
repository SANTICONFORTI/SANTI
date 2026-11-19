using System;

namespace Parcial4
{
    class NotificadorSMS : INotificador
    {
        public void Notificar(TrabajoImpresion t)
        {
            Program.Log($"   [SMS] a {t.Usuario}: se imprimió {t.Documento}");
        }
    }
}
