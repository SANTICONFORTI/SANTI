namespace DomainModel
{
    // Una funcionalidad del sistema puede ser simple o compuesta.
    // Las dos se tratan igual: eso es el patrón Composite.
    public abstract class Funcionalidad
    {
        public string Nombre { get; set; }

        public virtual void Agregar(Funcionalidad funcionalidad)
        {
            // Una simple no tiene hijos, asi que no se le puede agregar nada.
            throw new FuncionalidadException("Esta funcionalidad no admite hijos.");
        }

        public virtual void Quitar(Funcionalidad funcionalidad)
        {
            throw new FuncionalidadException("Esta funcionalidad no admite hijos.");
        }

        public virtual System.Collections.Generic.List<Funcionalidad> ObtenerHijos()
        {
            return new System.Collections.Generic.List<Funcionalidad>();
        }

        public virtual bool TieneHijos()
        {
            return false;
        }
    }
}
