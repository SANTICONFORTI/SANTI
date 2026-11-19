using System;
using DAL;
using DomainModel;

namespace BLL
{
    // Escucha la bateria y anota cada cambio en el registro.
    public class SuscriptorBitacora : IEscuchaBateria
    {
        private readonly IRegistroBateria registro;

        public SuscriptorBitacora(IRegistroBateria registro)
        {
            this.registro = registro;
        }

        public void Actualizar(Bateria bateria)
        {
            string estado;

            if (bateria.Conectado)
            {
                estado = "CONECTADO";
            }
            else
            {
                estado = "DESCONECTADO";
            }

            string linea = DateTime.Now.ToString("HH:mm:ss") +
                           " | " + estado +
                           " | carga " + bateria.Carga + "%" +
                           " | " + bateria.TiempoCarga + " min de carga" +
                           " | " + bateria.TiempoUso + " min de uso";

            registro.Anotar(linea);
        }
    }
}
