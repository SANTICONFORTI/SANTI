using DomainModel;

namespace DAL
{
    // PATRÓN PROXY - Sujeto
    // Lo implementan el catálogo real y el proxy: quien lo usa no distingue.
    public interface ICatalogo
    {
        Menu ObtenerMenu(string local);
        Producto PedirProducto(string local, string codigo, Cliente cliente);
    }
}
