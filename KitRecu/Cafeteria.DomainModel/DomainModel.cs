using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DomainModel
{
    // COMPOSITE: base común de simples y combos
    public abstract class Producto
    {
        public string Nombre { get; set; }
        public abstract decimal Precio();
    }

    // Hoja
    public class ProductoSimple : Producto
    {
        private readonly decimal precio;

        public ProductoSimple(string nombre, decimal precio)
        {
            Nombre = nombre;
            this.precio = precio;
        }

        public override decimal Precio() { return precio; }
    }

    // Compuesto: suma a sus hijos (que pueden ser simples u otros combos)
    public class Combo : Producto
    {
        private readonly List<Producto> productos = new List<Producto>();

        public Combo(string nombre) { Nombre = nombre; }

        public void Agregar(Producto p) { productos.Add(p); }

        public override decimal Precio()
        {
            decimal total = 0;
            foreach (Producto p in productos)
                total += p.Precio();          // no importa si es simple o combo
            return total;
        }
    }

    public class Pedido
    {
        public int Numero { get; set; }
        public string Cajero { get; set; }
        public Producto Producto { get; set; }
        public decimal Total { get; set; }
    }
}
