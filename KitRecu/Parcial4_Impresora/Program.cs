// =====================================================================
//  PARCIAL 4 - Sistema de impresión concurrente con control de velocidad
//  Proyecto: Aplicación de consola (.NET Framework 4.8)
//
//  PATRONES USADOS (y la frase del enunciado que los pide):
//   - SINGLETON  -> "asegurar que solo haya una instancia de Impresora"
//   - PRODUCTOR-CONSUMIDOR con MONITOR -> usuarios (productores) envían
//                  trabajos a una cola; la impresora (consumidor) los saca.
//   - CONTROL DE VELOCIDAD -> la cola tiene CAPACIDAD máxima: si está
//                  llena, el usuario ESPERA hasta que le avisen.
//   - STRATEGY + FACTORY -> notificaciones (SMS, WhatsApp, Email):
//                  "el consumidor no conoce las clases concretas ni el
//                  algoritmo". La impresora solo conoce INotificador.
//
//  Es el MISMO esqueleto que el ejercicio de la cocina:
//     Mozo -> Usuario | Bandeja -> cola de la Impresora | Cocinero -> Impresora
// =====================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

// ---------------------------------------------------------------------
// 1) ENTIDAD: el trabajo que viaja por la cola (solo datos)
// ---------------------------------------------------------------------
class TrabajoImpresion
{
    public int Numero;
    public string Usuario;
    public string Documento;
    public string TipoNotificacion;   // "SMS", "WhatsApp" o "Email"

    public override string ToString()
    {
        return $"#{Numero} '{Documento}' ({Usuario})";
    }
}

// ---------------------------------------------------------------------
// 2) EXCEPCIÓN PROPIA: "intentar imprimir cuando no hay trabajos"
// ---------------------------------------------------------------------
class ColaVaciaException : Exception
{
    public ColaVaciaException(string mensaje) : base(mensaje) { }
}

// ---------------------------------------------------------------------
// 3) STRATEGY: el contrato de notificación + una clase por medio
//    La impresora solo conoce INotificador, nunca SMS/WhatsApp/Email.
// ---------------------------------------------------------------------
interface INotificador
{
    void Notificar(TrabajoImpresion t);
}

class NotificadorSMS : INotificador
{
    public void Notificar(TrabajoImpresion t)
    {
        Program.Log($"   [SMS] a {t.Usuario}: se imprimió {t.Documento}");
    }
}

class NotificadorWhatsApp : INotificador
{
    public void Notificar(TrabajoImpresion t)
    {
        Program.Log($"   [WhatsApp] a {t.Usuario}: tu documento {t.Documento} está listo");
    }
}

class NotificadorEmail : INotificador
{
    public void Notificar(TrabajoImpresion t)
    {
        Program.Log($"   [Email] a {t.Usuario}: impresión #{t.Numero} finalizada");
    }
}

// ---------------------------------------------------------------------
// 4) FACTORY: decide QUÉ clase concreta crear a partir del tipo.
//    Si mañana aparece "Telegram", solo se toca esta clase.
// ---------------------------------------------------------------------
static class FabricaNotificadores
{
    public static INotificador Crear(string tipo)
    {
        switch (tipo)
        {
            case "SMS":      return new NotificadorSMS();
            case "WhatsApp": return new NotificadorWhatsApp();
            case "Email":    return new NotificadorEmail();
            default: throw new ArgumentException("Tipo de notificación desconocido: " + tipo);
        }
    }
}

// ---------------------------------------------------------------------
// 5) IMPRESORA: SINGLETON + recurso compartido sincronizado con MONITOR
// ---------------------------------------------------------------------
class Impresora
{
    // ---- SINGLETON ----
    private static Impresora instancia;
    private static readonly object candadoInstancia = new object();

    // Constructor PRIVADO: nadie puede hacer "new Impresora()" desde afuera
    private Impresora() { }

    public static Impresora Instancia
    {
        get
        {
            lock (candadoInstancia)          // por si dos hilos la piden a la vez
            {
                if (instancia == null)
                    instancia = new Impresora();
                return instancia;
            }
        }
    }

    // ---- COLA COMPARTIDA ----
    private const int CAPACIDAD = 3;          // control de velocidad
    private readonly Queue<TrabajoImpresion> cola = new Queue<TrabajoImpresion>();
    private readonly object candado = new object();
    private bool cerrada = false;
    public int Impresos { get; private set; }

    // PRODUCTOR: la llama cada USUARIO.
    // Si la cola está llena, el usuario espera (control de velocidad).
    public void EnviarTrabajo(TrabajoImpresion t)
    {
        bool tomado = false;
        try
        {
            Monitor.Enter(candado, ref tomado);

            // WHILE y no IF: al despertar se vuelve a verificar
            while (cola.Count == CAPACIDAD)
            {
                Program.Log($"{t.Usuario} espera: cola llena {Ocupacion()}");
                Monitor.Wait(candado);        // suelta el lock y duerme
            }

            cola.Enqueue(t);                  // Queue = FIFO -> respeta el orden
            Program.Log($"{t.Usuario} envía {t} {Ocupacion()}");
            Monitor.PulseAll(candado);        // despierta a la impresora
        }
        finally
        {
            if (tomado) Monitor.Exit(candado);
        }
    }

    // CONSUMIDOR: saca el siguiente trabajo de la cola.
    //  esperar = true  -> si está vacía, duerme hasta que llegue algo.
    //  esperar = false -> si está vacía, lanza ColaVaciaException.
    // Devuelve null cuando la cola está cerrada y vacía (fin del trabajo).
    public TrabajoImpresion ObtenerSiguienteTrabajo(bool esperar = true)
    {
        bool tomado = false;
        try
        {
            Monitor.Enter(candado, ref tomado);

            if (!esperar && cola.Count == 0)
                throw new ColaVaciaException("No hay trabajos en la cola para imprimir.");

            while (cola.Count == 0 && !cerrada)
                Monitor.Wait(candado);

            if (cola.Count == 0)              // vacía y cerrada
                return null;

            TrabajoImpresion t = cola.Dequeue();
            Monitor.PulseAll(candado);        // avisa a usuarios: hay lugar
            return t;
        }
        finally
        {
            if (tomado) Monitor.Exit(candado);
        }
    }

    // Simula la impresión (FUERA del lock, para no frenar a los usuarios)
    public void Imprimir(TrabajoImpresion t)
    {
        if (t == null)
            throw new ColaVaciaException("No hay trabajo para imprimir.");

        Program.Log($"IMPRESORA imprimiendo {t}...");
        Thread.Sleep(400);
        Impresos++;                           // solo la toca el hilo de la impresora
        Program.Log($"IMPRESORA terminó {t}");

        // La impresora NO sabe qué clase concreta notifica
        INotificador notificador = FabricaNotificadores.Crear(t.TipoNotificacion);
        notificador.Notificar(t);
    }

    // Lo que ejecuta el hilo de la impresora
    public void Procesar()
    {
        TrabajoImpresion t;
        while ((t = ObtenerSiguienteTrabajo()) != null)
            Imprimir(t);
        Program.Log($"IMPRESORA apagada: imprimió {Impresos} trabajos");
    }

    // Main la llama cuando todos los usuarios terminaron
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

    // Se llama siempre con el lock tomado
    private string Ocupacion()
    {
        return "[" + new string('#', cola.Count) + new string('.', CAPACIDAD - cola.Count) + "]";
    }
}

// ---------------------------------------------------------------------
// 6) USUARIO: cada uno es un HILO que envía una cantidad aleatoria
//    de trabajos, esperando un tiempo aleatorio entre envíos.
// ---------------------------------------------------------------------
class Usuario
{
    private static readonly string[] Tipos = { "SMS", "WhatsApp", "Email" };
    private static int ultimoNumero = 0;
    public static int TotalEnviados = 0;

    private readonly string nombre;
    private readonly Random rnd;

    public Usuario(string nombre, int semilla)
    {
        this.nombre = nombre;
        rnd = new Random(semilla);            // semilla distinta por usuario
    }

    public void Ejecutar()
    {
        int cantidad = rnd.Next(2, 5);        // número aleatorio de trabajos
        for (int i = 1; i <= cantidad; i++)
        {
            Thread.Sleep(rnd.Next(100, 400)); // tiempo aleatorio entre envíos
            TrabajoImpresion t = new TrabajoImpresion
            {
                Numero = Interlocked.Increment(ref ultimoNumero),
                Usuario = nombre,
                Documento = $"Doc{i}.pdf",
                TipoNotificacion = Tipos[rnd.Next(Tipos.Length)]
            };
            Impresora.Instancia.EnviarTrabajo(t);
            Interlocked.Increment(ref TotalEnviados);
        }
        Program.Log($"{nombre} terminó de enviar ({cantidad} trabajos)");
    }
}

// ---------------------------------------------------------------------
// 7) PROGRAMA + CASOS DE PRUEBA
// ---------------------------------------------------------------------
class Program
{
    private static readonly Stopwatch reloj = Stopwatch.StartNew();
    private static readonly object candadoConsola = new object();

    public static void Log(string mensaje)
    {
        lock (candadoConsola)
            Console.WriteLine($"{reloj.Elapsed.TotalSeconds,5:0.00}s  {mensaje}");
    }

    static void Main()
    {
        // PRUEBA 1: Singleton -> siempre la misma instancia
        Console.WriteLine("PRUEBA 1 - Singleton: " +
            (Impresora.Instancia == Impresora.Instancia ? "OK, misma instancia" : "ERROR"));

        // PRUEBA 2: imprimir con la cola vacía -> excepción controlada
        try
        {
            Impresora.Instancia.ObtenerSiguienteTrabajo(false);
            Console.WriteLine("PRUEBA 2 - ERROR: no lanzó excepción");
        }
        catch (ColaVaciaException ex)
        {
            Console.WriteLine("PRUEBA 2 - Cola vacía: OK -> " + ex.Message);
        }

        // PRUEBA 3: tipo de notificación inválido -> excepción controlada
        try
        {
            FabricaNotificadores.Crear("Paloma");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("PRUEBA 3 - Notificación inválida: OK -> " + ex.Message);
        }
        Console.WriteLine();

        // PRUEBA 4: concurrencia con varios usuarios y una impresora
        Usuario[] usuarios =
        {
            new Usuario("Ana  ", 1),
            new Usuario("Beto ", 2),
            new Usuario("Carla", 3),
            new Usuario("Dani ", 4)
        };

        List<Thread> hilosUsuarios = new List<Thread>();
        foreach (Usuario u in usuarios) hilosUsuarios.Add(new Thread(u.Ejecutar));
        Thread hiloImpresora = new Thread(Impresora.Instancia.Procesar);

        hiloImpresora.Start();
        hilosUsuarios.ForEach(h => h.Start());

        hilosUsuarios.ForEach(h => h.Join());   // esperamos a los usuarios...
        Impresora.Instancia.Cerrar();           // ...y cerramos la cola
        hiloImpresora.Join();

        Console.WriteLine();
        Console.WriteLine($"PRUEBA 4 - Enviados: {Usuario.TotalEnviados} | Impresos: {Impresora.Instancia.Impresos} | " +
            (Usuario.TotalEnviados == Impresora.Instancia.Impresos ? "OK" : "ERROR: se perdieron trabajos"));

        Console.ReadKey();
    }
}
