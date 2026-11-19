using System;

namespace Parcial4
{
    // ---------------------------------------------------------------------
    // 4) FACTORY: decide QUÉ clase concreta crear a partir del tipo.
    //    Si mañana aparece "Telegram", solo se toca esta clase.
    // ---------------------------------------------------------------------
    static class FabricaNotificadores
    {
        public static INotificador Crear(string tipo)
        {
            switch (tipo)
            {
                case "SMS":      return new NotificadorSMS();
                case "WhatsApp": return new NotificadorWhatsApp();
                case "Email":    return new NotificadorEmail();
                default: throw new ArgumentException("Tipo de notificación desconocido: " + tipo);
            }
        }
    }
}
