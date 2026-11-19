using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DAL
{
    using DomainModel;

    public class PedidoDAL
    {
        private static readonly object candado = new object();   // escriben varios hilos

        public void Guardar(Pedido p)
        {
            lock (candado)
                File.AppendAllText("pedidos.txt",
                    $"{p.Numero};{p.Cajero};{p.Producto.Nombre};{p.Total}{Environment.NewLine}");
        }
    }
}
