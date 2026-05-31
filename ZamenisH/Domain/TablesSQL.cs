using iText.Layout.Element;

using System;

namespace Domain
{
    public static class TablesSQL
    {
        public static object CrearInstancia(string nombreClase)
        {
            try
            {
                string table = "Domain.CXN." + nombreClase;
                System.Reflection.Assembly ensamblado = System.Reflection.Assembly.GetExecutingAssembly();

                if (table == "Domain.CXN.FIB_ENCUESTA1" || table == "Domain.CXN.FIB_ENCUESTA2" || table == "Domain.CXN.FIB_ENCUESTA3")
                {
                    table = "Domain.Fibromialgia." + nombreClase;
                }
             
                Type tipo = ensamblado.GetType(table);

                // Verificar si el tipo es válido
                if (tipo != null)
                {
                    // Crear una instancia del tipo
                    return Activator.CreateInstance(tipo);
                }
                else
                {
                    throw new ArgumentException();
                    throw new ArgumentException($"No se pudo encontrar la clase con el nombre {nombreClase}");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"No se pudo encontrar la clase con el nombre {nombreClase}  - " + ex.Message);
            }
        }
        
    }
}
