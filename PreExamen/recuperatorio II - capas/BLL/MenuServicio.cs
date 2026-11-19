using DomainModel;

namespace BLL
{
    // Saca del catalogo todo lo que el perfil no puede ver.
    public class MenuServicio
    {
        public Funcionalidad ArmarMenu(Funcionalidad catalogo, Perfil perfil)
        {
            FuncionalidadCompuesta raiz = new FuncionalidadCompuesta(catalogo.Nombre);

            foreach (Funcionalidad hijo in catalogo.ObtenerHijos())
            {
                Funcionalidad visible = Filtrar(hijo, perfil);

                if (visible != null)
                {
                    raiz.Agregar(visible);
                }
            }

            return raiz;
        }

        private Funcionalidad Filtrar(Funcionalidad funcionalidad, Perfil perfil)
        {
            // Si es compuesta, se filtra a sus hijos.
            if (funcionalidad.TieneHijos())
            {
                FuncionalidadCompuesta copia = new FuncionalidadCompuesta(funcionalidad.Nombre);

                foreach (Funcionalidad hijo in funcionalidad.ObtenerHijos())
                {
                    Funcionalidad visible = Filtrar(hijo, perfil);

                    if (visible != null)
                    {
                        copia.Agregar(visible);
                    }
                }

                // Si no quedo nada visible adentro, tampoco se muestra la compuesta.
                if (copia.TieneHijos())
                {
                    return copia;
                }

                return null;
            }

            // Si es simple, se muestra solo si el perfil la tiene permitida.
            if (perfil.Puede(funcionalidad.Nombre))
            {
                return new FuncionalidadSimple(funcionalidad.Nombre);
            }

            return null;
        }
    }
}
