using Domain;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Persistence
{
    public static class OverridesExtern 
    {       
        public static string GetCurrentMethodName()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            return stackFrame.GetMethod().Name;
        }

        public static void GenerarTXTException(TXTException T)
        {
            FileStream Query = new FileStream("C:/Cxn/Reportes/LogErrores.txt", FileMode.Append, FileAccess.Write);
            StreamWriter Escriba = new StreamWriter(Query);

            try
            {
                Escriba.Write("Fecha: " + Convert.ToDateTime(T.FechaHora).ToString("yyyy/MM/dd HH:mm:ss tt") + "\r" +
                              "Usuario: " + T.Usuario.ToString() + "\r" +
                              "Formulario: " + T.Formulario.ToString() + "\r" +
                              "Metodo: " + T.Metodo.ToString() + "\r" +
                              "Error:" + T.Error.ToString() + "\n\r");
                Escriba.WriteLine();
                Escriba.Flush();
                Escriba.Close();
            }
            catch (Exception ex)
            {
                DateTime Hoy = DateTime.Now;
                Escriba.Write("Fecha: " + Convert.ToDateTime(Hoy).ToString("yyyy/MM/dd HH:mm:ss tt") + "\r" +
                              "Usuario: " + T.Usuario.ToString() + "\r" +
                              "Formulario: " + T.Formulario.ToString() + "\r" +
                              "Metodo: " + T.Metodo.ToString() + "\r" +
                              "Error:" + ex.ToString() + "\n\r");
                Escriba.WriteLine();
                Escriba.Flush();
                Escriba.Close();
            }
        }
        public static void GenerarTXT(TXTException T)
        {
            FileStream Query = new FileStream("C:/Cxn/Reportes/Log.txt", FileMode.Append, FileAccess.Write);
            StreamWriter Escriba = new StreamWriter(Query);

            try
            {
                Escriba.Write("Error: " + T.Error.ToString() + "\r\n");
                Escriba.WriteLine();
                Escriba.Flush();
                Escriba.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
