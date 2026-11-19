using System.Collections.Generic;

namespace BLL
{
    // Guarda los medios y avisa por todos.
    // La impresora le habla a esta clase y no sabe cuales son.
    public class Avisos
    {
        private readonly List<IAviso> medios = new List<IAviso>();

        public int Enviados { get; private set; }

        public int Cantidad
        {
            get { return medios.Count; }
        }

        public void Agregar(IAviso medio)
        {
            medios.Add(medio);
        }

        public void EnviarATodos(string usuario, string archivo)
        {
            foreach (IAviso medio in medios)
            {
                medio.Avisar(usuario, archivo);
                Enviados = Enviados + 1;
            }
        }
    }
}
