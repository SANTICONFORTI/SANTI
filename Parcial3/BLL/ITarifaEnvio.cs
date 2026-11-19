namespace BLL
{
    // PATRÓN STRATEGY - Estrategia
    public interface ITarifaEnvio
    {
        string Nombre { get; }
        decimal Calcular(decimal tarifaBase);
    }
}
