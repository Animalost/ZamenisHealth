using Persistence.CXN.Interfaces;
using System;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;

namespace Persistence.CXN.Metodos
{
    public class MDatosEmail : IDatosEmail
    {
        CXN_EMAIL IDatosEmail.FirstEmail()
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT TOP 1 * " +
                                   "FROM CXN_EMAIL " +
                                   "ORDER BY Ema_Id ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_EMAIL E = new CXN_EMAIL
                                {
                                    Ema_Puerto = Convert.ToInt32(Reader["Ema_Puerto"]),
                                    Ema_Pass = Reader["Ema_Pass"].ToString(),
                                    Ema_Server = Reader["Ema_Server"].ToString(),
                                    Ema_Muestra = Reader["Ema_Muestra"].ToString(),
                                    Ema_Email = Reader["Ema_Email"].ToString(),
                                    Ema_Asunto = "Programacion de Citas",
                                    Ema_Baja_Email = Reader["Ema_Baja_Email"].ToString(),
                                    Ema_URL_CitasM = Reader["Ema_URL_CitasM"].ToString(),
                                    Ema_URL_CitasS = Reader["Ema_URL_CitasS"].ToString()
                                };

                                return E;
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }              
    }
}
