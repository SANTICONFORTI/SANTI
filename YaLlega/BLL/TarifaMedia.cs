namespace BLL
{
    // Demanda media: recargo del 25%
    public class TarifaMedia : ITarifaEnvio
    {
        public string Nombre { get { return "Media (+25%)"; } }

        public decimal Calcular(decimal tarifaBase)
        {
            return tarifaBase * 1.25m;
        }
    }
}
