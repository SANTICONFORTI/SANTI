# YaLlega S.A. – Diagrama de clases

Patrones: **Strategy** (tarifas de envío), **Proxy** (catálogo remoto: plan del cliente + caché de 5 menús), **genéricos + reflection** (exportador para cualquier clase).

```mermaid
classDiagram
    direction LR

    %% ---------- DomainModel ----------
    class Plan {
        <<enumeration>>
        Clasico
        Plus
        Premium
    }
    class Cliente {
        +Id: int
        +Nombre: string
        +Plan: Plan
    }
    class Producto {
        +Codigo: string
        +Nombre: string
        +Precio: decimal
        +PlanMinimo: Plan
    }
    class Menu {
        +Local: string
        +Productos: List~Producto~
        +Origen: string
    }
    class Pedido {
        +Numero: int
        +Cliente: Cliente
        +Productos: List~Producto~
        +TarifasAplicadas: List~string~
        +IntentosRechazados: List~string~
        +OrigenMenu: string
        +CostoEnvio: decimal
        +Cerrado: bool
    }
    Pedido --> Cliente
    Pedido --> "0..*" Producto
    Menu --> "0..*" Producto

    %% ---------- BLL: STRATEGY ----------
    class ITarifaEnvio {
        <<interface>>
        +Nombre: string
        +Calcular(tarifaBase) decimal
    }
    class TarifaNormal
    class TarifaMedia
    class TarifaAlta
    class TarifaPromocion
    ITarifaEnvio <|.. TarifaNormal
    ITarifaEnvio <|.. TarifaMedia
    ITarifaEnvio <|.. TarifaAlta
    ITarifaEnvio <|.. TarifaPromocion

    class GestorPedido {
        -tarifa: ITarifaEnvio
        -catalogo: ICatalogo
        +Pedido: Pedido
        +CargarMenu(local) Menu
        +AgregarProducto(local, codigo) bool
        +ActualizarDemanda(nivel)
        +ForzarModo(tarifa)
        +CalcularEnvio() decimal
        +Cerrar()
    }
    GestorPedido --> ITarifaEnvio : usa
    GestorPedido --> ICatalogo : usa
    GestorPedido --> Pedido
    GestorPedido --> PedidoDAL

    %% ---------- DAL: PROXY ----------
    class ICatalogo {
        <<interface>>
        +ObtenerMenu(local) Menu
        +PedirProducto(local, codigo, cliente) Producto
    }
    class CatalogoRemoto
    class CatalogoProxy {
        -cache: List~Menu~ (máx. 5)
        +EstaEnCache(local) bool
    }
    ICatalogo <|.. CatalogoRemoto
    ICatalogo <|.. CatalogoProxy
    CatalogoProxy --> CatalogoRemoto : real

    class PedidoDAL {
        +Guardar(pedido)
        +LeerTodo() string
    }

    %% ---------- Services ----------
    class Exportador {
        <<static>>
        +Exportar~T~(lista, archivo)
    }
```

**Capas y referencias:** UI → BLL → DAL; todas → DomainModel. Services (Exportador) lo usa la UI.
