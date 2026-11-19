# MACHETE PPIII – Recuperatorio

## 1. Cómo encarar el examen (3 parciales = ~3 horas)

1. **10 min, leer y subrayar.**
   - Sustantivos → clases.
   - Datos → atributos (propiedades).
   - Verbos → métodos.
2. **Marcar las frases gatillo** (tabla de abajo) y anotar al lado qué patrón va.
3. **Abrir el archivo del kit que más se parezca** y copiar el esqueleto.
4. **PRIMERO QUE CORRA:** versión mínima en consola. Después capas, archivo, excepciones y comentarios.
5. **Casos de prueba en el Main:** un bloque por escenario del enunciado, con "esperado: …".
6. **Diagrama de clases:** copiar el que está arriba de cada archivo del kit, cambiando los nombres.

> Si el tiempo no alcanza: que **compile y corra**. Pide el 70% funcionando.

## 2. Frases gatillo → patrón

| El enunciado dice… | Patrón | Archivo del kit |
|---|---|---|
| "notificar cada vez que cambia", "suscriptores", "1 a muchos" | **Observer** | Recu_Bateria_Observer.cs |
| "una única instancia", "sesión", "un solo objeto" | **Singleton** | Parcial4 (Impresora), Parcial2 (SesionManager) |
| "simples y compuestas", "conjunto que contiene otros conjuntos", "menú", "árbol", "permisos" | **Composite** | Parcial2_Login_WinForms.cs |
| "cambiar la forma de calcular", "según la situación", "en tiempo de ejecución", "intercambiable" | **Strategy** | Parcial3 (tarifas), Parcial4 (notificadores) |
| "servidor remoto", "caché", "impedir acceso", "no debe distinguir de dónde viene" | **Proxy** | Parcial3 (CatalogoProxy) |
| "no conoce las clases concretas", "crear según el tipo" | **Factory** | Parcial4 (FabricaNotificadores) |
| "varios hilos", "cola compartida", "esperar hasta que", "Monitor" | **Productor-Consumidor** | Parcial4_Impresora.cs |
| "funcione con cualquier clase, incluso futuras" | **Genéricos `<T>` + Reflection** | Parcial3 (Exportador) |

## 3. La forma común de casi todos los patrones

```
INTERFAZ (contrato)  <---  varias clases que la cumplen
        ^
        |  (solo conoce la interfaz)
   la clase que la usa
```

| Patrón | La interfaz | Las que la cumplen | La que la usa |
|---|---|---|---|
| Observer | IObservador | Visual, Bitácora | Batería (lista de observadores) |
| Strategy | ITarifaEnvio | Normal, Media, Alta, Promo | GestorPedido (tiene UNA y la cambia) |
| Proxy | ICatalogo | CatalogoRemoto **y** CatalogoProxy | La pantalla (no sabe cuál le dieron) |
| Composite | Componente (abstracta) | Patente (hoja), Familia (tiene hijos) | Usuario / el menú |

**Singleton** es aparte: constructor `private`, un `static instancia` y una propiedad `Instancia` que la crea una sola vez.

## 4. Capas

| Capa | Qué va | Ejemplo |
|---|---|---|
| **BE** | Solo datos (propiedades) | Usuario, Producto, Pedido |
| **DAL** | Guardar y leer (lista en memoria, archivo, BD) | UsuarioDAL, PedidoDAL |
| **BLL** | Reglas: validar, calcular, decidir | UsuarioBLL, GestorPedido |
| **Servicios** | Lo que usan todos: sesión, encriptación, bitácora, exportar | SesionManager |
| **UI** | Consola o WinForms: muestra y llama a la BLL | FrmLogin, Program |

**Regla:** UI → BLL → DAL. Todas conocen BE. La UI **nunca** llama a la DAL.

### Armarlo en Visual Studio

1. Crear la solución con el proyecto UI (Consola o Windows Forms, .NET Framework).
2. Clic derecho en la solución → Agregar → Nuevo proyecto → **Biblioteca de clases (.NET Framework)**. Crear una por cada una: BE, DAL, BLL, Servicios.
3. Clic derecho en el proyecto → Agregar → **Referencia** → Proyectos. Marcar:
   - UI: BLL, BE, Servicios
   - BLL: DAL, BE, Servicios
   - DAL: BE, Servicios
   - Servicios: BE
4. Las clases de las bibliotecas tienen que ser **`public`**. Si no, la otra capa no las ve.
5. Arriba de cada archivo poner `using BE;`, `using BLL;`, etc.
6. Clic derecho en el proyecto UI → **Establecer como proyecto de inicio**.

> Si el tiempo aprieta: un solo proyecto con **carpetas** BE, DAL, BLL y UI (o namespaces, como en el kit) también muestra las capas.

## 5. Plantilla del Monitor (de memoria)

```csharp
bool tomado = false;
try
{
    Monitor.Enter(candado, ref tomado);
    while (!puedoSeguir)          // WHILE, nunca IF
        Monitor.Wait(candado);    // suelta el lock y duerme
    // modificar la cola (Enqueue / Dequeue)
    Monitor.PulseAll(candado);    // avisar a los que esperan
}
finally
{
    if (tomado) Monitor.Exit(candado);
}
```

- **Control de velocidad:** el productor espera `while (cola.Count == CAPACIDAD)`.
- **Consumidor:** espera `while (cola.Count == 0 && !cerrada)`.
- **Orden de llegada:** usar `Queue<T>` (FIFO).

## 6. Justificaciones en una línea (para comentar en el código)

- **Observer:** el sujeto avisa a N observadores sin conocerlos. Se agregan nuevos sin tocar el sujeto.
- **Strategy:** encapsula cada algoritmo en una clase. Se cambia en tiempo de ejecución sin `if` gigantes.
- **Proxy:** misma interfaz que el real. Agrega control de acceso o caché sin que el cliente lo note.
- **Composite:** trata igual a un elemento simple que a un grupo. Permite árboles de cualquier profundidad.
- **Singleton:** garantiza una única instancia con un punto de acceso global.
- **Factory:** centraliza la creación. El que usa no conoce las clases concretas.

## 7. Teoría que tomaron (parcial 2)

- **Proceso de software:** conjunto estructurado de actividades para desarrollar un producto de software. Las actividades son especificación, diseño e implementación, validación y evolución.
- **Patrón de diseño:** una solución probada y reutilizable a un problema de diseño que se repite en un contexto determinado. Tiene nombre, problema, solución y consecuencias.
- **Objetivo de cada familia de patrones:**
  - Creacionales: cómo se crean los objetos.
  - Estructurales: cómo se componen las clases y los objetos.
  - Comportamiento: cómo se reparten responsabilidades y se comunican los objetos.
- **Arquitectura en capas:** cada capa tiene una responsabilidad y solo usa a la de abajo.
  - El **modelo de dominio (BE)** representa los conceptos del negocio y lo usan todas las capas.
  - Si cambia el dominio, impacta en todas; por eso se diseña primero.

## 8. Checklist antes de entregar

- [ ] Compila y corre.
- [ ] Cada patrón tiene un comentario arriba que dice **cuál es y por qué**.
- [ ] Hay `try/catch` en el Main y en lo que toca archivos.
- [ ] Hay validaciones (rangos, nulos) con excepciones claras.
- [ ] Hay casos de prueba en el Main, uno por escenario.
- [ ] Hay diagrama de clases.
- [ ] `Console.ReadKey()` al final, para que no se cierre la consola.

## 9. Errores típicos

- *"No se encuentra el tipo X"*: falta la referencia, el `using` o que la clase sea `public`.
- La consola se cierra sola: falta `Console.ReadKey()`.
- El programa queda colgado con hilos: usaste `if` en vez de `while`, `Pulse` en vez de `PulseAll`, o no llamaste a `Cerrar()`.
- *"El proceso no puede obtener acceso al archivo"*: un `StreamWriter` sin `using` quedó abierto.
