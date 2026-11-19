using DomainModel;

namespace BLL
{
    // Arma el arbol de funcionalidades del sistema.
    // Hay compuestas dentro de compuestas: Administracion contiene Usuarios.
    public static class CatalogoFuncionalidades
    {
        public static Funcionalidad Armar()
        {
            FuncionalidadCompuesta sistema = new FuncionalidadCompuesta("Sistema");

            FuncionalidadCompuesta ventas = new FuncionalidadCompuesta("Ventas");
            ventas.Agregar(new FuncionalidadSimple("Ventas - Nuevo presupuesto"));
            ventas.Agregar(new FuncionalidadSimple("Ventas - Listado"));
            ventas.Agregar(new FuncionalidadSimple("Ventas - Anular"));

            FuncionalidadCompuesta stock = new FuncionalidadCompuesta("Stock");
            stock.Agregar(new FuncionalidadSimple("Stock - Ingreso"));
            stock.Agregar(new FuncionalidadSimple("Stock - Ajuste"));

            FuncionalidadCompuesta usuarios = new FuncionalidadCompuesta("Usuarios");
            usuarios.Agregar(new FuncionalidadSimple("Usuarios - Alta"));
            usuarios.Agregar(new FuncionalidadSimple("Usuarios - Baja"));

            FuncionalidadCompuesta administracion = new FuncionalidadCompuesta("Administracion");
            administracion.Agregar(usuarios);
            administracion.Agregar(new FuncionalidadSimple("Administracion - Copia de seguridad"));

            sistema.Agregar(ventas);
            sistema.Agregar(stock);
            sistema.Agregar(administracion);

            return sistema;
        }
    }
}
