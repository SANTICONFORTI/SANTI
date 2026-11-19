namespace BLL
{
    // Demanda baja: tarifa normal
    public class TarifaNormal : ITarifaEnvio
    {
        public string Nombre { get { return "Normal"; } }

        public decimal Calcular(decimal tarifaBase)
        {
            return tarifaBase;
        }
    }
}
