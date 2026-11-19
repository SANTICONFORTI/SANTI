using System.Collections.Generic;

namespace DomainModel
{
    public class Pedido
    {
        public int Numero { get; set; }
        public Cliente Cliente { get; set; }
        public List<Producto> Productos { get; set; }
        public List<string> TarifasAplicadas { get; set; }
        public List<string> IntentosRechazados { get; set; }
        public string OrigenMenu { get; set; }
        public decimal CostoEnvio { get; set; }
        public bool Cerrado { get; set; }

        public Pedido()
        {
            Productos = new List<Producto>();
            TarifasAplicadas = new List<string>();
            IntentosRechazados = new List<string>();
        }
    }
}
