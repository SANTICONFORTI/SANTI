using System.Collections.Generic;
using System.Threading;
using DomainModel;

namespace DAL
{
    // PATRÓN PROXY - Sujeto real
    // Simula el catálogo alojado en el servidor remoto.
    public class CatalogoRemoto : ICatalogo
    {
        private readonly Dictionary<string, List<Producto>> menus = new Dictionary<string, List<Producto>>();

        public int Descargas { get; private set; }

        public CatalogoRemoto()
        {
            menus["Burger"] = new List<Producto>
            {
                new Producto { Codigo = "B1", Nombre = "Hamburguesa", Precio = 5000, PlanMinimo = Plan.Clasico },
                new Producto { Codigo = "B2", Nombre = "Doble bacon", Precio = 7500, PlanMinimo = Plan.Plus }
            };
            menus["Sushi"] = new List<Producto>
            {
                new Producto { Codigo = "S1", Nombre = "Combo 15 piezas", Precio = 12000, PlanMinimo = Plan.Plus },
                new Producto { Codigo = "S2", Nombre = "Omakase", Precio = 30000, PlanMinimo = Plan.Premium }
            };
            foreach (string local in new[] { "Pizza", "Pastas", "Empanadas", "Helados", "Cafe" })
            {
                menus[local] = new List<Producto>
                {
                    new Producto { Codigo = local + "1", Nombre = local + " clásico", Precio = 4000, PlanMinimo = Plan.Clasico }
                };
            }
        }

        public Menu ObtenerMenu(string local)
        {
            if (!menus.ContainsKey(local))
                throw new KeyNotFoundException("No existe el local " + local);

            Thread.Sleep(300);   // simula la demora de la red
            Descargas++;
            return new Menu { Local = local, Productos = menus[local], Origen = "Servidor" };
        }

        public Producto PedirProducto(string local, string codigo, Cliente cliente)
        {
            Menu menu = ObtenerMenu(local);
            Producto p = menu.Productos.Find(x => x.Codigo == codigo);
            if (p == null)
                throw new KeyNotFoundException("No existe el producto " + codigo);
            return p;
        }
    }
}
