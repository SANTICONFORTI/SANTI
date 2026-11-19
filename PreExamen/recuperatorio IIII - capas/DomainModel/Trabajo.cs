namespace DomainModel
{
    // Un trabajo de impresion.
    public class Trabajo
    {
        public int Numero { get; set; }
        public string Usuario { get; set; }
        public string Archivo { get; set; }

        public Trabajo(int numero, string usuario, string archivo)
        {
            Numero = numero;
            Usuario = usuario;
            Archivo = archivo;
        }

        public override string ToString()
        {
            return "#" + Numero + " " + Archivo + " (" + Usuario + ")";
        }
    }
}
