using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BLL
{
    using DomainModel;

    // La UI guarda a través de la BLL (nunca llama a la DAL directo)
    public class PedidoBLL
    {
        private readonly DAL.PedidoDAL dal = new DAL.PedidoDAL();

        public void Guardar(Pedido p)
        {
            if (p == null) throw new ArgumentNullException("p");
            dal.Guardar(p);
        }
    }

    // ---- STRATEGY ----
    public interface IDescuento
    {
        decimal Aplicar(decimal precio);
    }

    public class SinDescuento : IDescuento
    {
        public decimal Aplicar(decimal precio) { return precio; }
    }

    public class HappyHour : IDescuento
    {
        public decimal Aplicar(decimal precio) { return precio * 0.80m; }
    }

    public class Jubilado : IDescuento
    {
        public decimal Aplicar(decimal precio) { return precio * 0.90m; }
    }

    public class Caja
    {
        private IDescuento descuento = new SinDescuento();

        public void CambiarDescuento(IDescuento nuevo) { descuento = nuevo; }

        public decimal Cobrar(Producto p)
        {
            return descuento.Aplicar(p.Precio());
        }
    }

    // ---- PRODUCTOR-CONSUMIDOR con MONITOR (igual a la cocina) ----
    public class Bandeja
    {
        private const int CAPACIDAD = 2;
        private readonly Queue<Pedido> pedidos = new Queue<Pedido>();
        private readonly object candado = new object();
        private bool cerrada = false;

        public void Dejar(Pedido p)
        {
            bool tomado = false;
            try
            {
                Monitor.Enter(candado, ref tomado);
                while (pedidos.Count == CAPACIDAD)
                    Monitor.Wait(candado);
                pedidos.Enqueue(p);
                Console.WriteLine($"{p.Cajero} deja pedido #{p.Numero} ({p.Producto.Nombre})");
                Monitor.PulseAll(candado);
            }
            finally
            {
                if (tomado) Monitor.Exit(candado);
            }
        }

        // Devuelve null cuando está cerrada y vacía
        public Pedido Tomar()
        {
            bool tomado = false;
            try
            {
                Monitor.Enter(candado, ref tomado);
                while (pedidos.Count == 0 && !cerrada)
                    Monitor.Wait(candado);
                if (pedidos.Count == 0) return null;
                Pedido p = pedidos.Dequeue();
                Monitor.PulseAll(candado);
                return p;
            }
            finally
            {
                if (tomado) Monitor.Exit(candado);
            }
        }

        public void Cerrar()
        {
            bool tomado = false;
            try
            {
                Monitor.Enter(candado, ref tomado);
                cerrada = true;
                Monitor.PulseAll(candado);
            }
            finally
            {
                if (tomado) Monitor.Exit(candado);
            }
        }
    }
}
