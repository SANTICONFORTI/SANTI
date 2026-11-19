using System.Collections.Generic;
using DomainModel;

namespace DAL
{
    // PATRÓN PROXY - Proxy
    // Misma interfaz que el real. Agrega:
    //  1) Protección: no deja pedir productos fuera del plan del cliente.
    //  2) Caché: guarda en el dispositivo los últimos 5 menús consultados.
    public class CatalogoProxy : ICatalogo
    {
        private const int MAX_CACHE = 5;
        private readonly CatalogoRemoto real;
        private readonly List<Menu> cache = new List<Menu>();   // el más reciente queda al final

        public CatalogoProxy(CatalogoRemoto real)
        {
            this.real = real;
        }

        public Menu ObtenerMenu(string local)
        {
            Menu enCache = cache.Find(m => m.Local == local);
            if (enCache != null)
            {
                cache.Remove(enCache);   // lo paso al final: recién usado
                cache.Add(enCache);
                return new Menu { Local = local, Productos = enCache.Productos, Origen = "Dispositivo" };
            }

            Menu descargado = real.ObtenerMenu(local);
            cache.Add(descargado);
            if (cache.Count > MAX_CACHE)
                cache.RemoveAt(0);       // saco el más viejo
            return descargado;
        }

        public Producto PedirProducto(string local, string codigo, Cliente cliente)
        {
            Menu menu = ObtenerMenu(local);
            Producto p = menu.Productos.Find(x => x.Codigo == codigo);
            if (p == null)
                throw new KeyNotFoundException("No existe el producto " + codigo);

            if (cliente.Plan < p.PlanMinimo)
                throw new ProductoNoIncluidoException(
                    p.Nombre + " requiere plan " + p.PlanMinimo + " (cliente tiene " + cliente.Plan + ")");
            return p;
        }

        public bool EstaEnCache(string local)
        {
            return cache.Exists(m => m.Local == local);
        }
    }
}
