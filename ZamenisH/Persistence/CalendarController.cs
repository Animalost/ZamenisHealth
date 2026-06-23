using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence
{
    public class CalendarController
    {
        public List<DateTime> getFestivos(DateTime Desde, DateTime Hasta)
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

                    String Cargar_Agenda = "SELECT * " +
                                           "FROM CXN_FECHAS " +
                                           "WHERE F_Fecha BETWEEN @param1 AND @param2 " +
                                           "ORDER BY F_Fecha ASC";

                    using (SqlCommand Carga_Agenda = new SqlCommand(Cargar_Agenda, con))
                    {
                        Carga_Agenda.Parameters.AddWithValue("@param1", Convert.ToDateTime(Desde));
                        Carga_Agenda.Parameters.AddWithValue("@param2", Convert.ToDateTime(Hasta));

                        using (SqlDataReader Lectura_Agenda = (Carga_Agenda.ExecuteReader()))
                        {
                            if (Lectura_Agenda.HasRows)
                            {
                                List<DateTime> L = new List<DateTime>();

                                while (Lectura_Agenda.Read() == true)
                                {
                                    L.Add(Convert.ToDateTime(Lectura_Agenda["F_Fecha"]));
                                }
                                return L;
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
                Console.WriteLine(ex.ToString());
                return null;
            }            
        }

        public List<DateTime> GetBloqueosProfesionales(DateTime Desde, DateTime Hasta, int Medico)
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

                    String Query = "SELECT * " +
                                   "FROM CXN_DIAS_WEB " +
                                   "WHERE F_Prof = @param3 " +
                                   "AND F_Estado = 'B' " +
                                   "AND F_Fecha BETWEEN @param1 AND @param2 " +
                                   "ORDER BY F_Fecha DESC";
                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Convert.ToDateTime(Desde));
                        Commando.Parameters.AddWithValue("@param2", Convert.ToDateTime(Hasta));
                        Commando.Parameters.AddWithValue("@param3", Medico);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<DateTime> L = new List<DateTime>();

                                while (Reader.Read() == true)
                                {
                                    L.Add(Convert.ToDateTime(Reader["F_Fecha"]));
                                }

                                return L;
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
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
