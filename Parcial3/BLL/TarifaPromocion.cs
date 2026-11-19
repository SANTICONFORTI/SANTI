namespace BLL
{
    // Modo promoción: envío gratis sin importar la demanda
    public class TarifaPromocion : ITarifaEnvio
    {
        public string Nombre { get { return "Promoción (gratis)"; } }

        public decimal Calcular(decimal tarifaBase)
        {
            return 0;
        }
    }
}
