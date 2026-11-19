namespace BLL
{
    // Demanda alta (hora pico, lluvia): recargo del 60%
    public class TarifaAlta : ITarifaEnvio
    {
        public string Nombre { get { return "Alta (+60%)"; } }

        public decimal Calcular(decimal tarifaBase)
        {
            return tarifaBase * 1.60m;
        }
    }
}
