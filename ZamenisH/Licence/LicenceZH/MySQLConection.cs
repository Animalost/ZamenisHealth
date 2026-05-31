using MySql.Data.MySqlClient;
using System.Text.Json;
using System;
using System.Data;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

namespace LicenceZH
{
    public class MySQLConection
    {
        private static IGenerales repoGen = new MGenerales();
        static string CadenaHostinger = "Server=82.197.82.161;Database=u415794711_Licencias;User Id=u415794711_Default;Password=Fl7P7t0*bL4kC7T0;";
        static string CadenaAlwaysData = "Server=mysql-zamenis.alwaysdata.net;Database=zamenis_licencias;Uid=zamenis;Pwd=Fl7P7t0*bL4kC7T0;Port=3306;";
        
        public MySQLConection()
        {
            repoGen = new MGenerales();
        }

        public static Domain.Licence.ZhealthClass ObtenerLicencia(string Licence)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(CadenaHostinger))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT * " + 
                                         "FROM ZHealth " +
                                         "WHERE Serial = @param1";

                    using (MySqlCommand Carga_Command = new MySqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", repoGen.Base64Decode(Licence));

                        using (MySqlDataReader Lectura_Hora2 = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora2.Read() == true)
                            {
                                Domain.Licence.ZhealthClass obj = JsonSerializer.Deserialize<Domain.Licence.ZhealthClass>(Lectura_Hora2["Valores"].ToString());                                
                                return obj;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener la licencia: " + ex.Message);
                return null;
            }
        }
    }
}
