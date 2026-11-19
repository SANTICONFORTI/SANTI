using System.Collections.Generic;

namespace DomainModel
{
    // La bateria avisa a todos los que esten suscriptos.
    public class Bateria
    {
        private readonly List<IEscuchaBateria> escuchas = new List<IEscuchaBateria>();
        private bool conectado;
        private int carga;

        public bool Conectado
        {
            get { return conectado; }
            set
            {
                // La consigna: avisar cada vez que cambia la conexion.
                if (conectado != value)
                {
                    conectado = value;
                    Avisar();
                }
            }
        }

        public int Carga
        {
            get { return carga; }
            set
            {
                // La consigna: validar el rango.
                if (value < 0 || value > 100)
                {
                    throw new BateriaException("La carga tiene que estar entre 0 y 100.");
                }

                carga = value;
            }
        }

        public int TiempoCarga
        {
            get
            {
                if (conectado)
                {
                    return (100 - carga) * 2;
                }

                return 0;
            }
        }

        public int TiempoUso
        {
            get
            {
                if (conectado)
                {
                    return 0;
                }

                return carga * 3;
            }
        }

        public void Agregar(IEscuchaBateria escucha)
        {
            if (!escuchas.Contains(escucha))
            {
                escuchas.Add(escucha);
            }
        }

        public void Quitar(IEscuchaBateria escucha)
        {
            escuchas.Remove(escucha);
        }

        public void Avisar()
        {
            // Recorro una copia: si alguien se da de baja mientras aviso, no rompe.
            List<IEscuchaBateria> copia = new List<IEscuchaBateria>(escuchas);

            foreach (IEscuchaBateria escucha in copia)
            {
                escucha.Actualizar(this);
            }
        }
    }
}
