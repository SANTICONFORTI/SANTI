using System;
using DAL;
using DomainModel;

namespace BLL
{
    // PATRÓN STRATEGY - Contexto
    // Usa la tarifa sin saber cuál es, y la puede cambiar con el pedido abierto.
    public class GestorPedido
    {
        private const decimal TARIFA_BASE = 1000;
        private static int ultimoNumero = 0;

        // Un solo proxy para toda la aplicación: así la caché se comparte
        private static readonly CatalogoProxy proxy = new CatalogoProxy(new CatalogoRemoto());

        private readonly ICatalogo catalogo = proxy;   // no sabe si es el real o el proxy
        private readonly PedidoDAL dal = new PedidoDAL();
        private ITarifaEnvio tarifa;
        private bool modoForzado = false;              // el operador apretó un botón

        public Pedido Pedido { get; private set; }

        public GestorPedido(Cliente cliente)
        {
            Pedido = new Pedido { Numero = ++ultimoNumero, Cliente = cliente };
            CambiarTarifa(new TarifaNormal());
        }

        public Menu CargarMenu(string local)
        {
            Menu menu = catalogo.ObtenerMenu(local);
            Pedido.OrigenMenu = menu.Origen;
            return menu;
        }

        // true si se agregó; si el plan no alcanza, se registra el rechazo
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
            if (modoForzado) return;   // si el operador forzó un modo, manda él
            switch (nivel)
            {
                case "Baja": CambiarTarifa(new TarifaNormal()); break;
                case "Media": CambiarTarifa(new TarifaMedia()); break;
                case "Alta": CambiarTarifa(new TarifaAlta()); break;
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
            return tarifa.Calcular(TARIFA_BASE);
        }

        public void Cerrar()
        {
            VerificarAbierto();
            Pedido.CostoEnvio = CalcularEnvio();
            Pedido.Cerrado = true;
            dal.Guardar(Pedido);
        }

        public string VerPedidosGuardados()
        {
            return dal.LeerTodo();
        }

        public static bool MenuEnCache(string local)
        {
            return proxy.EstaEnCache(local);
        }

        private void VerificarAbierto()
        {
            if (Pedido != null && Pedido.Cerrado)
                throw new InvalidOperationException("El pedido ya está cerrado.");
        }
    }
}
