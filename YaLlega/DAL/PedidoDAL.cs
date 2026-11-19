using System;
using System.IO;
using DomainModel;

namespace DAL
{
    // Persistencia en archivo con formato personalizado:
    //   PEDIDO|1|Juan|Clasico|Origen:Servidor|Envio:1000
    //   PRODUCTO|B1|Hamburguesa|5000
    //   TARIFA|Normal
    //   RECHAZO|Doble bacon requiere plan Plus...
    //   FIN
    public class PedidoDAL
    {
        private const string ARCHIVO = "pedidos.txt";

        public void Guardar(Pedido p)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(ARCHIVO, true))
                {
                    sw.WriteLine("PEDIDO|" + p.Numero + "|" + p.Cliente.Nombre + "|" + p.Cliente.Plan +
                                 "|Origen:" + p.OrigenMenu + "|Envio:" + p.CostoEnvio);
                    foreach (Producto prod in p.Productos)
                        sw.WriteLine("PRODUCTO|" + prod.Codigo + "|" + prod.Nombre + "|" + prod.Precio);
                    foreach (string t in p.TarifasAplicadas)
                        sw.WriteLine("TARIFA|" + t);
                    foreach (string r in p.IntentosRechazados)
                        sw.WriteLine("RECHAZO|" + r);
                    sw.WriteLine("FIN");
                }
            }
            catch (IOException ex)
            {
                throw new Exception("No se pudo guardar el pedido: " + ex.Message, ex);
            }
        }

        public string LeerTodo()
        {
            return File.Exists(ARCHIVO) ? File.ReadAllText(ARCHIVO) : "";
        }
    }
}
