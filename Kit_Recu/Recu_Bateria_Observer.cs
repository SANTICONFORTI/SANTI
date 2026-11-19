// =====================================================================
//  RECUPERATORIO (julio) - Batería de laptop con OBSERVER
//  Proyecto: Aplicación de consola (.NET Framework 4.8)
//
//  PATRÓN: OBSERVER -> "notificar cada vez que Conectado se modifique",
//          "suscriptores", "relación 1 a muchos".
//    - Sujeto (el observado):  Bateria
//    - Observadores:           SuscriptorVisual, SuscriptorBitacora
//  La batería NO sabe qué suscriptores tiene: solo tiene una lista de
//  IObservador. Agregar uno nuevo no obliga a tocar Bateria.
//
//  DIAGRAMA DE CLASES (resumen para dibujar):
//
//   <<interface>> ISujeto              <<interface>> IObservador
//     Suscribir / Desuscribir /   1   *   Actualizar(Bateria)
//     Notificar          <>------------------>    ^         ^
//         ^                                       |         |
//      Bateria                       SuscriptorVisual  SuscriptorBitacora
// =====================================================================
using System;
using System.Collections.Generic;
using System.IO;

// ---------------------------------------------------------------------
// 1) LOS CONTRATOS
// ---------------------------------------------------------------------
interface IObservador
{
    void Actualizar(Bateria bateria);
}

interface ISujeto
{
    void Suscribir(IObservador o);
    void Desuscribir(IObservador o);
    void Notificar();
}

// ---------------------------------------------------------------------
// 2) EL SUJETO: la batería
// ---------------------------------------------------------------------
class Bateria : ISujeto
{
    // Supuestos para los cálculos (los dejo como constantes y comentados)
    private const double MINUTOS_POR_PORCIENTO_CARGA = 1.5;  // 0% -> 100% en 150 min
    private const double MINUTOS_POR_PORCIENTO_USO = 3.0;    // 100% dura 300 min

    private readonly List<IObservador> observadores = new List<IObservador>();
    private bool conectado;
    private int carga;

    public Bateria(int cargaInicial)
    {
        Carga = cargaInicial;          // pasa por la validación
    }

    // Cada vez que cambia Conectado se notifica AUTOMÁTICAMENTE
    public bool Conectado
    {
        get { return conectado; }
        set
        {
            if (conectado == value) return;   // si no cambió, no aviso
            conectado = value;
            Notificar();
        }
    }

    // Validación de rango 0-100
    public int Carga
    {
        get { return carga; }
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException("Carga", "La carga debe estar entre 0 y 100.");
            carga = value;
        }
    }

    // Minutos para llegar al 100% (0 si está completa o desconectada)
    public int TiempoCarga
    {
        get
        {
            if (!conectado || carga == 100) return 0;
            return (int)Math.Round((100 - carga) * MINUTOS_POR_PORCIENTO_CARGA);
        }
    }

    // Minutos de uso restantes (solo tiene sentido desconectada)
    public int TiempoUso
    {
        get
        {
            if (conectado) return 0;
            return (int)Math.Round(carga * MINUTOS_POR_PORCIENTO_USO);
        }
    }

    public void Suscribir(IObservador o)
    {
        if (o == null) throw new ArgumentNullException("o");
        if (!observadores.Contains(o))
            observadores.Add(o);
    }

    public void Desuscribir(IObservador o)
    {
        observadores.Remove(o);
    }

    public void Notificar()
    {
        foreach (IObservador o in observadores)
        {
            try
            {
                o.Actualizar(this);
            }
            catch (Exception ex)
            {
                // Si un suscriptor falla, los demás igual se enteran
                Console.WriteLine("Error en un suscriptor: " + ex.Message);
            }
        }
    }

    // Texto común que usan los dos suscriptores
    public string Descripcion()
    {
        if (conectado)
        {
            string tiempo = carga == 100 ? "carga completa" : $"faltan {TiempoCarga} min para cargar";
            return $"CARGANDO | {carga}% | {tiempo}";
        }
        return $"DESCONECTADA | {carga}% | quedan {TiempoUso} min de uso";
    }
}

// ---------------------------------------------------------------------
// 3) OBSERVADOR 1: muestra en consola (diferencia carga vs. uso con color)
// ---------------------------------------------------------------------
class SuscriptorVisual : IObservador
{
    public void Actualizar(Bateria bateria)
    {
        Console.ForegroundColor = bateria.Conectado ? ConsoleColor.Green : ConsoleColor.Yellow;
        Console.WriteLine("+--------------------------------------------------+");
        Console.WriteLine($"| Estado : {(bateria.Conectado ? "Cargando" : "Desconectada"),-40}|");
        Console.WriteLine($"| Carga  : {bateria.Carga + "%",-40}|");
        if (bateria.Conectado)
            Console.WriteLine($"| Carga completa en: {bateria.TiempoCarga + " min",-30}|");
        else
            Console.WriteLine($"| Uso restante     : {bateria.TiempoUso + " min",-30}|");
        Console.WriteLine("+--------------------------------------------------+");
        Console.ResetColor();
    }
}

// ---------------------------------------------------------------------
// 4) OBSERVADOR 2: bitácora en archivo con fecha/hora y ROTACIÓN POR FECHA
//    Se escribe siempre en bitacora.txt. Si ese archivo es de un día
//    anterior, se renombra a bitacora_AAAAMMDD.txt y se empieza uno nuevo.
// ---------------------------------------------------------------------
class SuscriptorBitacora : IObservador
{
    private const string ARCHIVO = "bitacora.txt";

    public void Actualizar(Bateria bateria)
    {
        try
        {
            RotarSiCorresponde();
            string linea = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {bateria.Descripcion()}";
            File.AppendAllText(ARCHIVO, linea + Environment.NewLine);   // crea o agrega
        }
        catch (IOException ex)
        {
            Console.WriteLine("No se pudo escribir la bitácora: " + ex.Message);
        }
    }

    private void RotarSiCorresponde()
    {
        if (!File.Exists(ARCHIVO)) return;

        DateTime fechaArchivo = File.GetLastWriteTime(ARCHIVO).Date;
        if (fechaArchivo >= DateTime.Today) return;      // es de hoy: sigo usándolo

        string historico = $"bitacora_{fechaArchivo:yyyyMMdd}.txt";
        if (File.Exists(historico))
        {
            File.AppendAllText(historico, File.ReadAllText(ARCHIVO));
            File.Delete(ARCHIVO);
        }
        else
        {
            File.Move(ARCHIVO, historico);
        }
    }
}

// ---------------------------------------------------------------------
// 5) PROGRAMA + CASOS DE PRUEBA
// ---------------------------------------------------------------------
class Program
{
    static void Main()
    {
        try
        {
            Bateria bateria = new Bateria(40);
            SuscriptorVisual visual = new SuscriptorVisual();
            SuscriptorBitacora bitacora = new SuscriptorBitacora();

            // 1 a muchos: la misma batería, dos suscriptores
            bateria.Suscribir(visual);
            bateria.Suscribir(bitacora);

            Console.WriteLine("PRUEBA 1: conecto el cargador (40%) -> avisan los dos");
            bateria.Conectado = true;

            Console.WriteLine("\nPRUEBA 2: la carga sube a 100% y desconecto");
            bateria.Carga = 100;
            bateria.Conectado = false;

            Console.WriteLine("\nPRUEBA 3: asigno el mismo valor -> NO debe notificar");
            bateria.Conectado = false;
            Console.WriteLine("(no apareció nada: OK)");

            Console.WriteLine("\nPRUEBA 4: desuscribo la vista; solo escribe la bitácora");
            bateria.Desuscribir(visual);
            bateria.Carga = 15;
            bateria.Conectado = true;
            Console.WriteLine("(la consola no mostró el cuadro: OK)");

            Console.WriteLine("\nPRUEBA 5: carga fuera de rango -> excepción controlada");
            try
            {
                bateria.Carga = 150;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("OK -> " + ex.Message);
            }

            Console.WriteLine("\nContenido de bitacora.txt:");
            Console.WriteLine(File.ReadAllText("bitacora.txt"));
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: " + ex.Message);
        }
        Console.ReadKey();
    }
}
