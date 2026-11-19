using System.Collections.Generic;

namespace DomainModel
{
    public class Menu
    {
        public string Local { get; set; }
        public List<Producto> Productos { get; set; }
        public string Origen { get; set; }     // "Servidor" o "Dispositivo"
    }
}
