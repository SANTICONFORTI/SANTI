namespace DomainModel
{
    // El que escucha los cambios de la bateria.
    public interface IEscuchaBateria
    {
        void Actualizar(Bateria bateria);
    }
}
