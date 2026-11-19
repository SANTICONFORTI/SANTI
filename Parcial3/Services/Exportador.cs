using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Services
{
    // Exportador GENÉRICO con REFLECTION:
    // funciona con cualquier clase (Producto, Cliente, Pedido o una futura)
    // porque lee sus propiedades en tiempo de ejecución.
    public static class Exportador
    {
        public static void Exportar<T>(IEnumerable<T> lista, string archivo)
        {
            PropertyInfo[] props = typeof(T).GetProperties();

            using (StreamWriter sw = new StreamWriter(archivo, false))
            {
                // Encabezados = nombres de las propiedades
                List<string> encabezados = new List<string>();
                foreach (PropertyInfo p in props) encabezados.Add(p.Name);
                sw.WriteLine(string.Join(";", encabezados));

                // Una fila por objeto
                foreach (T item in lista)
                {
                    List<string> valores = new List<string>();
                    foreach (PropertyInfo p in props)
                        valores.Add(Formatear(p.GetValue(item)));
                    sw.WriteLine(string.Join(";", valores));
                }
            }
        }

        private static string Formatear(object valor)
        {
            if (valor == null) return "";
            if (valor is string) return (string)valor;
            if (valor is ICollection) return ((ICollection)valor).Count + " items";
            // Objetos con propiedad Nombre (ej: el Cliente dentro de un Pedido)
            PropertyInfo nombre = valor.GetType().GetProperty("Nombre");
            if (nombre != null) return Convert.ToString(nombre.GetValue(valor));
            return valor.ToString();
        }
    }
}
