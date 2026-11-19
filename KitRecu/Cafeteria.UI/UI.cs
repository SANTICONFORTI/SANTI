// =====================================================================
//  EJERCICIO DE PRÁCTICA - Cafetería (mezcla de los parciales 2, 3 y 4)
//  Proyecto: Aplicación de consola (.NET Framework 4.8)
//
//  ENUNCIADO
//  Una cafetería vende productos simples (café, medialuna, jugo) y combos.
//  Un combo es un conjunto de productos y puede contener otros combos. El
//  precio de un combo es la suma de sus productos.
//  El precio final de un pedido depende del descuento vigente: sin
//  descuento, happy hour (-20%) o jubilado (-10%), y se debe poder cambiar
//  en cualquier momento.
//  Dos cajeros (hilos) toman pedidos y los dejan en una bandeja que
//  tiene lugar para 2 pedidos; si está llena, esperan. Un barista (hilo)
//  toma los pedidos de la bandeja en orden y los prepara. Usar Monitor.
//  Guardar cada pedido preparado en un archivo. Usar capas.
//
//  PATRONES
//   - COMPOSITE -> "un combo es un conjunto y puede contener otros combos"
//   - STRATEGY  -> "el precio depende del descuento y se puede cambiar"
//   - PRODUCTOR-CONSUMIDOR con MONITOR -> cajeros / bandeja / barista
//
//  CAPAS (un namespace = un proyecto en el examen)
//   DomainModel: Producto, ProductoSimple, Combo, Pedido
//   DAL:         PedidoDAL (archivo)
//   BLL:         descuentos (Strategy), Bandeja (Monitor)
//   UI:          Program (Cajero, Barista, Main)
// =====================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace UI
{
    using DomainModel;
    using BLL;

    class Cajero
    {
        private static int ultimoNumero = 0;
        private readonly string nombre;
        private readonly Bandeja bandeja;
        private readonly Caja caja;
        private readonly List<Producto> menu;

        public Cajero(string nombre, Bandeja bandeja, Caja caja, List<Producto> menu)
        {
            this.nombre = nombre;
            this.bandeja = bandeja;
            this.caja = caja;
            this.menu = menu;
        }

        public void Ejecutar()
        {
            for (int i = 0; i < menu.Count; i++)
            {
                Thread.Sleep(200);
                Pedido p = new Pedido
                {
                    Numero = Interlocked.Increment(ref ultimoNumero),
                    Cajero = nombre,
                    Producto = menu[i],
                    Total = caja.Cobrar(menu[i])
                };
                bandeja.Dejar(p);
            }
        }
    }

    class Barista
    {
        private readonly Bandeja bandeja;
        private readonly PedidoBLL bll = new PedidoBLL();

        public Barista(Bandeja bandeja) { this.bandeja = bandeja; }

        public void Ejecutar()
        {
            Pedido p;
            while ((p = bandeja.Tomar()) != null)
            {
                Thread.Sleep(500);            // preparar FUERA del lock
                bll.Guardar(p);
                Console.WriteLine($"   Barista preparó #{p.Numero}: {p.Producto.Nombre} ${p.Total}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            // COMPOSITE: combo dentro de combo
            ProductoSimple cafe = new ProductoSimple("Café", 1000);
            ProductoSimple medialuna = new ProductoSimple("Medialuna", 500);
            ProductoSimple jugo = new ProductoSimple("Jugo", 1500);

            Combo desayuno = new Combo("Desayuno");
            desayuno.Agregar(cafe);
            desayuno.Agregar(medialuna);

            Combo desayunoCompleto = new Combo("Desayuno completo");
            desayunoCompleto.Agregar(desayuno);    // combo dentro de combo
            desayunoCompleto.Agregar(jugo);

            Console.WriteLine($"PRUEBA 1 - Desayuno: ${desayuno.Precio()} (esperado 1500)");
            Console.WriteLine($"PRUEBA 2 - Desayuno completo: ${desayunoCompleto.Precio()} (esperado 3000)");

            // STRATEGY: el mismo producto con distintos descuentos
            Caja caja = new Caja();
            Console.WriteLine($"PRUEBA 3 - Sin descuento: ${caja.Cobrar(desayuno)} (esperado 1500)");
            caja.CambiarDescuento(new HappyHour());
            Console.WriteLine($"PRUEBA 4 - Happy hour: ${caja.Cobrar(desayuno)} (esperado 1200)");
            caja.CambiarDescuento(new Jubilado());
            Console.WriteLine($"PRUEBA 5 - Jubilado: ${caja.Cobrar(desayuno)} (esperado 1350)");
            caja.CambiarDescuento(new SinDescuento());
            Console.WriteLine();

            // MONITOR: 2 cajeros y 1 barista
            List<Producto> menu = new List<Producto> { cafe, desayuno, desayunoCompleto };
            Bandeja bandeja = new Bandeja();

            Thread c1 = new Thread(new Cajero("Cajero Ana ", bandeja, caja, menu).Ejecutar);
            Thread c2 = new Thread(new Cajero("Cajero Beto", bandeja, caja, menu).Ejecutar);
            Thread b = new Thread(new Barista(bandeja).Ejecutar);

            b.Start();
            c1.Start();
            c2.Start();

            c1.Join();
            c2.Join();
            bandeja.Cerrar();      // ya no hay cajeros: que el barista termine
            b.Join();

            Console.WriteLine("\nPRUEBA 6 - Se prepararon los 6 pedidos y están en pedidos.txt");
            Console.ReadKey();
        }
    }
}
