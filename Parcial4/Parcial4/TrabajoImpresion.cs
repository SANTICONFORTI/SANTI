using System;

namespace Parcial4
{
    // ---------------------------------------------------------------------
    // 1) ENTIDAD: el trabajo que viaja por la cola (solo datos)
    // ---------------------------------------------------------------------
    class TrabajoImpresion
    {
        public int Numero;
        public string Usuario;
        public string Documento;
        public string TipoNotificacion;   // "SMS", "WhatsApp" o "Email"

        public override string ToString()
        {
            return $"#{Numero} '{Documento}' ({Usuario})";
        }
    }
}
