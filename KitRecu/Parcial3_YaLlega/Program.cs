// =====================================================================
//  PARCIAL 3 - YaLlega S.A. (pedidos y entregas)
//  Proyecto: Aplicación de consola (.NET Framework 4.8)
//
//  Todo está en un archivo para estudiarlo, pero separado por CAPAS
//  con namespaces. En el examen: un proyecto (Biblioteca de clases) por
//  namespace -> BE, DAL, BLL, Servicios, y la consola/WinForms como UI.
//
//  PATRONES (y la frase del enunciado que los pide):
//   - STRATEGY -> "debe poder cambiar la forma de calcular el envío
//                 mientras el pedido está abierto" (Normal/Media/Alta/Promo)
//   - PROXY    -> "catálogo en servidor remoto", "impedir productos fuera
//                 del plan" (proxy de protección), "mantener los últimos 5
//                 menús" (proxy caché), "la pantalla no debe distinguir si
//                 viene del servidor o del dispositivo" (misma interfaz)
//   - GENÉRICOS + REFLECTION -> "una única rutina de exportación que
//                 funcione con cualquier clase, incluso futuras"
//
//  DIAGRAMA DE CLASES (resumen para dibujar):
//
//   <<interface>> ITarifaEnvio <------ GestorPedido ------> <<interface>> ICatalogo
//      ^   ^   ^   ^                      |                      ^           ^
//  Normal Media Alta Promocion            |              CatalogoRemoto  CatalogoProxy
//                                         v                                  |
//                                    PedidoDAL                (tiene un) CatalogoRemoto
//
//   BE: Cliente, Producto, Menu, Pedido   |  Servicios: Exportador (genérico)
// =====================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

// =====================================================================
//  CAPA BE (Entidades): SOLO DATOS. No calculan, no guardan, no muestran.
//  Uso propiedades { get; set; } para que el Exportador las lea.
// =====================================================================
namespace BE
{
    public enum Plan { Clasico = 1, Plus = 2, Premium = 3 }

    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Plan Plan { get; set; }
    }

    public class Producto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public Plan PlanMinimo { get; set; }   // plan necesario para pedirlo
    }

    public class Menu
    {
        public string Local { get; set; }
        public List<Producto> Productos { get; set; }
        public string Origen { get; set; }     // "Servidor" o "Dispositivo"
    }

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

    // Excepción propia para el proxy de protección
    public class ProductoNoIncluidoException : Exception
    {
        public ProductoNoIncluidoException(string mensaje) : base(mensaje) { }
    }
}

// =====================================================================
//  CAPA DAL (Datos): el "servidor remoto" y el guardado en archivo
// =====================================================================
namespace DAL
{
    using BE;

    // ---- PROXY: el contrato que usan TANTO el real como el proxy ----
    public interface ICatalogo
    {
        Menu ObtenerMenu(string local);
        Producto PedirProducto(string local, string codigo, Cliente cliente);
    }

    // ---- El objeto REAL: simula el servidor remoto (lento) ----
    public class CatalogoRemoto : ICatalogo
    {
        private readonly Dictionary<string, List<Producto>> menus = new Dictionary<string, List<Producto>>();
        public int Descargas { get; private set; }   // para las pruebas

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
            // Más locales para probar el límite de 5 en caché
            foreach (string local in new[] { "Pizza", "Pastas", "Empanadas", "Helados", "Cafe" })
                menus[local] = new List<Producto>
                {
                    new Producto { Codigo = local + "1", Nombre = local + " clásico", Precio = 4000, PlanMinimo = Plan.Clasico }
                };
        }

        public Menu ObtenerMenu(string local)
        {
            if (!menus.ContainsKey(local))
                throw new KeyNotFoundException("No existe el local " + local);

            Thread.Sleep(300);                 // simula la demora de red
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

    // ---- El PROXY: misma interfaz, se pone "delante" del real ----
    //  1) Protección: controla el plan del cliente.
    //  2) Caché: guarda los últimos 5 menús en el dispositivo.
    public class CatalogoProxy : ICatalogo
    {
        private const int MAX_CACHE = 5;
        private readonly CatalogoRemoto real;
        private readonly List<Menu> cache = new List<Menu>();   // el último usado va al final

        public CatalogoProxy(CatalogoRemoto real)
        {
            this.real = real;
        }

        public Menu ObtenerMenu(string local)
        {
            Menu enCache = cache.Find(m => m.Local == local);
            if (enCache != null)
            {
                cache.Remove(enCache);          // lo paso al final: recién usado
                cache.Add(enCache);
                return new Menu { Local = local, Productos = enCache.Productos, Origen = "Dispositivo" };
            }

            Menu descargado = real.ObtenerMenu(local);   // no estaba: voy al servidor
            cache.Add(descargado);
            if (cache.Count > MAX_CACHE)
                cache.RemoveAt(0);              // saco el más viejo
            return descargado;
        }

        public Producto PedirProducto(string local, string codigo, Cliente cliente)
        {
            Menu menu = ObtenerMenu(local);     // usa la caché si puede
            Producto p = menu.Productos.Find(x => x.Codigo == codigo);
            if (p == null)
                throw new KeyNotFoundException("No existe el producto " + codigo);

            // PROTECCIÓN: el plan del cliente tiene que alcanzar
            if (cliente.Plan < p.PlanMinimo)
                throw new ProductoNoIncluidoException(
                    $"{p.Nombre} requiere plan {p.PlanMinimo} (cliente tiene {cliente.Plan})");
            return p;
        }

        public bool EstaEnCache(string local)
        {
            return cache.Exists(m => m.Local == local);
        }
    }

    // ---- Persistencia en archivo con FORMATO PERSONALIZADO ----
    public class PedidoDAL
    {
        private readonly string archivo;

        public PedidoDAL(string archivo)
        {
            this.archivo = archivo;
        }

        // Formato: una línea por dato, con etiqueta al principio
        //   PEDIDO|1|Juan|Plus|Origen:Servidor|Envio:1250
        //   PRODUCTO|B1|Hamburguesa|5000
        //   TARIFA|Normal
        //   RECHAZO|Omakase requiere plan Premium...
        //   FIN
        public void Guardar(Pedido p)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(archivo, true))   // true = agrega al final
                {
                    sw.WriteLine($"PEDIDO|{p.Numero}|{p.Cliente.Nombre}|{p.Cliente.Plan}|Origen:{p.OrigenMenu}|Envio:{p.CostoEnvio}");
                    foreach (Producto prod in p.Productos)
                        sw.WriteLine($"PRODUCTO|{prod.Codigo}|{prod.Nombre}|{prod.Precio}");
                    foreach (string t in p.TarifasAplicadas)
                        sw.WriteLine($"TARIFA|{t}");
                    foreach (string r in p.IntentosRechazados)
                        sw.WriteLine($"RECHAZO|{r}");
                    sw.WriteLine("FIN");
                }
            }
            catch (IOException ex)
            {
                throw new Exception("No se pudo guardar el pedido: " + ex.Message, ex);
            }
        }
    }
}

// =====================================================================
//  CAPA BLL (Negocio): las reglas. Acá vive el STRATEGY.
// =====================================================================
namespace BLL
{
    using BE;
    using DAL;

    // ---- STRATEGY: el contrato + una clase por forma de calcular ----
    public interface ITarifaEnvio
    {
        string Nombre { get; }
        decimal Calcular(decimal tarifaBase);
    }

    public class TarifaNormal : ITarifaEnvio
    {
        public string Nombre { get { return "Normal"; } }
        public decimal Calcular(decimal tarifaBase) { return tarifaBase; }
    }

    public class TarifaMedia : ITarifaEnvio
    {
        public string Nombre { get { return "Media (+25%)"; } }
        public decimal Calcular(decimal tarifaBase) { return tarifaBase * 1.25m; }
    }

    public class TarifaAlta : ITarifaEnvio
    {
        public string Nombre { get { return "Alta (+60%)"; } }
        public decimal Calcular(decimal tarifaBase) { return tarifaBase * 1.60m; }
    }

    public class TarifaPromocion : ITarifaEnvio
    {
        public string Nombre { get { return "Promoción (gratis)"; } }
        public decimal Calcular(decimal tarifaBase) { return 0; }
    }

    // ---- El CONTEXTO del Strategy: usa la tarifa sin saber cuál es ----
    public class GestorPedido
    {
        private const decimal TARIFA_BASE = 1000;
        private static int ultimoNumero = 0;

        private readonly ICatalogo catalogo;   // no sabe si es el real o el proxy
        private readonly PedidoDAL dal;
        private ITarifaEnvio tarifa;
        private bool modoForzado = false;      // el operador apretó un botón

        public Pedido Pedido { get; private set; }

        public GestorPedido(Cliente cliente, ICatalogo catalogo, PedidoDAL dal)
        {
            this.catalogo = catalogo;
            this.dal = dal;
            Pedido = new Pedido { Numero = ++ultimoNumero, Cliente = cliente };
            CambiarTarifa(new TarifaNormal());
        }

        public Menu CargarMenu(string local)
        {
            Menu menu = catalogo.ObtenerMenu(local);
            Pedido.OrigenMenu = menu.Origen;
            return menu;
        }

        // Devuelve true si se agregó; si el plan no alcanza, registra el rechazo
        public bool AgregarProducto(string local, string codigo)
        {
            VerificarAbierto();
            try
            {
                Producto p = catalogo.PedirProducto(local, codigo, Pedido.Cliente);
                Pedido.Productos.Add(p);
                return true;
            }
            catch (ProductoNoIncluidoException ex)
            {
                Pedido.IntentosRechazados.Add(ex.Message);
                return false;
            }
        }

        // El sistema mide la demanda cada cierto tiempo y llama a esto
        public void ActualizarDemanda(string nivel)
        {
            if (modoForzado) return;           // si el operador forzó, manda él
            switch (nivel)
            {
                case "Baja":  CambiarTarifa(new TarifaNormal()); break;
                case "Media": CambiarTarifa(new TarifaMedia()); break;
                case "Alta":  CambiarTarifa(new TarifaAlta()); break;
                default: throw new ArgumentException("Nivel de demanda inválido: " + nivel);
            }
        }

        // Botón del operador: fuerza un modo manualmente
        public void ForzarModo(ITarifaEnvio nueva)
        {
            modoForzado = true;
            CambiarTarifa(nueva);
        }

        private void CambiarTarifa(ITarifaEnvio nueva)
        {
            VerificarAbierto();
            tarifa = nueva;
            Pedido.TarifasAplicadas.Add(nueva.Nombre);
        }

        public decimal CalcularEnvio()
        {
            return tarifa.Calcular(TARIFA_BASE);   // no sabe cuál tarifa es
        }

        public void Cerrar()
        {
            VerificarAbierto();
            Pedido.CostoEnvio = CalcularEnvio();
            Pedido.Cerrado = true;
            dal.Guardar(Pedido);
        }

        private void VerificarAbierto()
        {
            if (Pedido != null && Pedido.Cerrado)
                throw new InvalidOperationException("El pedido ya está cerrado.");
        }
    }
}

// =====================================================================
//  CAPA SERVICIOS: exportador GENÉRICO con REFLECTION
//  Funciona con cualquier clase (Producto, Cliente, Pedido o una futura)
//  porque lee las propiedades en tiempo de ejecución.
// =====================================================================
namespace Servicios
{
    public static class Exportador
    {
        public static void Exportar<T>(IEnumerable<T> lista, string archivo)
        {
            PropertyInfo[] props = typeof(T).GetProperties();

            using (StreamWriter sw = new StreamWriter(archivo, false))
            {
                // Encabezados = nombres de las propiedades
                List<string> encabezados = new List<string>();
                foreach (PropertyInfo p in props) encabezados.Add(p.Name);
                sw.WriteLine(string.Join(";", encabezados));

                // Una fila por objeto
                foreach (T item in lista)
                {
                    List<string> valores = new List<string>();
                    foreach (PropertyInfo p in props)
                        valores.Add(Formatear(p.GetValue(item)));
                    sw.WriteLine(string.Join(";", valores));
                }
            }
        }

        private static string Formatear(object valor)
        {
            if (valor == null) return "";
            if (valor is string) return (string)valor;
            if (valor is ICollection) return ((ICollection)valor).Count + " items";   // listas
            // Objetos con propiedad "Nombre" (ej: Cliente dentro de Pedido)
            PropertyInfo nombre = valor.GetType().GetProperty("Nombre");
            if (nombre != null && !valor.GetType().IsPrimitive)
                return Convert.ToString(nombre.GetValue(valor));
            return valor.ToString();
        }
    }
}

// =====================================================================
//  CAPA UI (consola): CASOS DE PRUEBA de cada escenario
// =====================================================================
namespace UI
{
    using BE;
    using DAL;
    using BLL;
    using Servicios;

    class Program
    {
        static void Main()
        {
            try
            {
                CatalogoRemoto remoto = new CatalogoRemoto();
                CatalogoProxy proxy = new CatalogoProxy(remoto);
                PedidoDAL dal = new PedidoDAL("pedidos.txt");

                Cliente juan = new Cliente { Id = 1, Nombre = "Juan", Plan = Plan.Clasico };
                Cliente sofi = new Cliente { Id = 2, Nombre = "Sofi", Plan = Plan.Premium };

                // ---------------- STRATEGY ----------------
                Console.WriteLine("=== PRUEBA 1: tarifas según demanda (base $1000) ===");
                GestorPedido g1 = new GestorPedido(juan, proxy, dal);
                Console.WriteLine($"Baja:  ${g1.CalcularEnvio()}   (esperado 1000)");
                g1.ActualizarDemanda("Media");
                Console.WriteLine($"Media: ${g1.CalcularEnvio()}   (esperado 1250)");
                g1.ActualizarDemanda("Alta");
                Console.WriteLine($"Alta:  ${g1.CalcularEnvio()}   (esperado 1600)");

                Console.WriteLine("\n=== PRUEBA 2: el operador fuerza promoción ===");
                g1.ForzarModo(new TarifaPromocion());
                g1.ActualizarDemanda("Alta");      // se ignora: está forzado
                Console.WriteLine($"Promo con demanda alta: ${g1.CalcularEnvio()}   (esperado 0)");

                // ---------------- PROXY: protección ----------------
                Console.WriteLine("\n=== PRUEBA 3: plan del cliente (proxy de protección) ===");
                g1.CargarMenu("Burger");
                Console.WriteLine("Juan (Clásico) pide Hamburguesa: " + (g1.AgregarProducto("Burger", "B1") ? "OK" : "RECHAZADO"));
                Console.WriteLine("Juan (Clásico) pide Doble bacon: " + (g1.AgregarProducto("Burger", "B2") ? "OK" : "RECHAZADO (esperado)"));

                // ---------------- PROXY: caché ----------------
                Console.WriteLine("\n=== PRUEBA 4: caché de los últimos 5 menús ===");
                GestorPedido g2 = new GestorPedido(sofi, proxy, dal);
                Console.WriteLine("Sushi 1ra vez -> " + g2.CargarMenu("Sushi").Origen + "     (esperado Servidor)");
                Console.WriteLine("Sushi 2da vez -> " + g2.CargarMenu("Sushi").Origen + "  (esperado Dispositivo)");
                g2.AgregarProducto("Sushi", "S2");     // Sofi es Premium: puede
                foreach (string local in new[] { "Pizza", "Pastas", "Empanadas", "Helados", "Cafe" })
                    g2.CargarMenu(local);
                Console.WriteLine("Tras 5 menús nuevos, ¿Burger sigue en caché? " +
                    (proxy.EstaEnCache("Burger") ? "SI" : "NO (esperado: se descartó el más viejo)"));
                Console.WriteLine("Descargas reales del servidor: " + remoto.Descargas);

                // ---------------- Pedido cerrado ----------------
                Console.WriteLine("\n=== PRUEBA 5: no se puede cambiar un pedido cerrado ===");
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

                // ---------------- PERSISTENCIA ----------------
                Console.WriteLine("\n=== PRUEBA 6: pedidos guardados en pedidos.txt ===");
                Console.WriteLine(File.ReadAllText("pedidos.txt"));

                // ---------------- EXPORTADOR GENÉRICO ----------------
                Console.WriteLine("=== PRUEBA 7: exportar cualquier listado ===");
                Exportador.Exportar(new List<Cliente> { juan, sofi }, "clientes.txt");
                Exportador.Exportar(remoto.ObtenerMenu("Sushi").Productos, "productos.txt");
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
