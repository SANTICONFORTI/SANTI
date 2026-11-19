namespace DomainModel
{
    public class Producto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public Plan PlanMinimo { get; set; }   // plan necesario para pedirlo
    }
}
