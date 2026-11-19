using System;

namespace Parcial4
{
    // ---------------------------------------------------------------------
    // 3) STRATEGY: el contrato de notificación + una clase por medio
    //    La impresora solo conoce INotificador, nunca SMS/WhatsApp/Email.
    // ---------------------------------------------------------------------
    interface INotificador
    {
        void Notificar(TrabajoImpresion t);
    }
}
