using System;

namespace Parcial4
{
    class NotificadorWhatsApp : INotificador
    {
        public void Notificar(TrabajoImpresion t)
        {
            Program.Log($"   [WhatsApp] a {t.Usuario}: tu documento {t.Documento} está listo");
        }
    }
}
