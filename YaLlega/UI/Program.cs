using System;
using System.Collections.Generic;
using System.IO;
using BLL;
using DomainModel;
using Services;

namespace UI
{
    // CASOS DE PRUEBA de cada escenario del enunciado
    class Program
    {
        static void Main()
        {
            try
            {
                Cliente juan = new Cliente { Id = 1, Nombre = "Juan", Plan = Plan.Clasico };
                Cliente sofi = new Cliente { Id = 2, Nombre = "Sofi", Plan = Plan.Premium };

                // ---------- STRATEGY ----------
                Console.WriteLine("=== PRUEBA 1: tarifas según demanda (base $1000) ===");
                GestorPedido g1 = new GestorPedido(juan);
                Console.WriteLine("Baja:  $" + g1.CalcularEnvio() + "   (esperado 1000)");
                g1.ActualizarDemanda("Media");
                Console.WriteLine("Media: $" + g1.CalcularEnvio() + "   (esperado 1250)");
                g1.ActualizarDemanda("Alta");
                Console.WriteLine("Alta:  $" + g1.CalcularEnvio() + "   (esperado 1600)");

                Console.WriteLine();
                Console.WriteLine("=== PRUEBA 2: el operador fuerza promoción ===");
                g1.ForzarModo(new TarifaPromocion());
                g1.ActualizarDemanda("Alta");   // se ignora: el modo está forzado
                Console.WriteLine("Promo con demanda alta: $" + g1.CalcularEnvio() + "   (esperado 0)");

                // ---------- PROXY: protección ----------
                Console.WriteLine();
                Console.WriteLine("=== PRUEBA 3: plan del cliente ===");
                g1.CargarMenu("Burger");
                Console.WriteLine("Juan (Clásico) pide Hamburguesa: " +
                    (g1.AgregarProducto("Burger", "B1") ? "OK" : "RECHAZADO"));
                Console.WriteLine("Juan (Clásico) pide Doble bacon: " +
                    (g1.AgregarProducto("Burger", "B2") ? "OK" : "RECHAZADO (esperado)"));

                // ---------- PROXY: caché ----------
                Console.WriteLine();
                Console.WriteLine("=== PRUEBA 4: caché de los últimos 5 menús ===");
                GestorPedido g2 = new GestorPedido(sofi);
                Console.WriteLine("Sushi 1ra vez -> " + g2.CargarMenu("Sushi").Origen + "     (esperado Servidor)");
                Console.WriteLine("Sushi 2da vez -> " + g2.CargarMenu("Sushi").Origen + "  (esperado Dispositivo)");
                g2.AgregarProducto("Sushi", "S2");   // Sofi es Premium: puede
                foreach (string local in new[] { "Pizza", "Pastas", "Empanadas", "Helados", "Cafe" })
                    g2.CargarMenu(local);
                Console.WriteLine("Tras 5 menús nuevos, ¿Burger sigue en caché? " +
                    (GestorPedido.MenuEnCache("Burger") ? "SI" : "NO (esperado)"));

                // ---------- Pedido cerrado ----------
                Console.WriteLine();
                Console.WriteLine("=== PRUEBA 5: no se puede modificar un pedido cerrado ===");
                g1.Cerrar();
                g2.Cerrar();
                try
                {
                    g1.AgregarProducto("Burger", "B1");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine("OK -> " + ex.Message);
                }

                // ---------- PERSISTENCIA ----------
                Console.WriteLine();
                Console.WriteLine("=== PRUEBA 6: pedidos guardados en pedidos.txt ===");
                Console.WriteLine(g1.VerPedidosGuardados());

                // ---------- EXPORTADOR GENÉRICO ----------
                Console.WriteLine("=== PRUEBA 7: exportar cualquier listado ===");
                Exportador.Exportar(new List<Cliente> { juan, sofi }, "clientes.txt");
                Exportador.Exportar(g2.Pedido.Productos, "productos.txt");
                Exportador.Exportar(new List<Pedido> { g1.Pedido, g2.Pedido }, "pedidos_export.txt");
                Console.WriteLine(File.ReadAllText("clientes.txt"));
                Console.WriteLine(File.ReadAllText("productos.txt"));
                Console.WriteLine(File.ReadAllText("pedidos_export.txt"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
            Console.ReadKey();
        }
    }
}
