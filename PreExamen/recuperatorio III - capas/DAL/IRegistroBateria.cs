namespace DAL
{
    // Lo que tiene que saber hacer el que guarda las lineas del registro.
    public interface IRegistroBateria
    {
        void Anotar(string linea);
    }
}
