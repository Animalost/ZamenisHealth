using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MNuevaAgenda : INuevaAgenda
    {
        List<CXN_DISPONIBILIDAD_2> INuevaAgenda.GetDisponibilidad(string Dia, int Med)
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

                    String QueryCons =  "SELECT * " +
                                        "FROM CXN_DISPONIBILIDAD_2 " +
                                        "WHERE Med = @param1 " +
                                        "AND Dia = @param2 " +
                                        "ORDER BY Hora ASC";                    

                    using (SqlCommand Command = new SqlCommand(QueryCons, con))
                    {                        
                        Command.Parameters.AddWithValue("@param1", Med);
                        Command.Parameters.AddWithValue("@param2", Dia);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_DISPONIBILIDAD_2> L = new List<CXN_DISPONIBILIDAD_2>();

                                while (Reader.Read() == true)
                                {
                                    L.Add(new CXN_DISPONIBILIDAD_2
                                    {
                                        Dia = Reader["Dia"].ToString(),
                                        Id = Convert.ToInt32(Reader["Id"]),
                                        Habilita = Reader["Habilita"].ToString(),
                                        Hora = Convert.ToDateTime(Reader["Hora"]),
                                        Ide = Reader["Ide"].ToString(),
                                        Med = Convert.ToInt32(Reader["Med"])
                                    });
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
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                return null;
            }
        }
        List<CXN_HORARIO> INuevaAgenda.GetHorariosSemana(DateTime Desde, DateTime Hasta, int Med, int Cia)
        {
            var lista = new List<CXN_HORARIO>();

            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    con.Open();

                    string query = @"
                                    SELECT
                                        Hor_Id,
                                        Hor_Pac_Id_Hora,
                                        Hor_Pac_Fecha_Cita,
                                        Hor_Pac_Bod,
                                        Hor_Pac_Cia,
                                        Hor_Imp_Age,
                                        Hor_Estado, 
                                        Hor_Color
                                    FROM CXN_HORARIO
                                    WHERE Hor_Pac_Fecha_Cita >= @Desde
                                      AND Hor_Pac_Fecha_Cita < @Hasta
                                      AND Hor_Pac_Bod = @Med
                                      AND Hor_Pac_Cia = @Cia";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value =
                            Desde.Date;

                        cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value =
                            Hasta.Date.AddDays(1);

                        cmd.Parameters.Add("@Med", SqlDbType.Int).Value = Med;

                        cmd.Parameters.Add("@Cia", SqlDbType.Int).Value = Cia;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CXN_HORARIO horario = new CXN_HORARIO
                                {
                                    Hor_Id = Convert.ToInt32(reader["Hor_Id"]),

                                    Hor_Imp_Age =
                                        reader["Hor_Imp_Age"]?.ToString(),

                                    Hor_Estado =
                                        reader["Hor_Estado"]?.ToString(),

                                    Hor_Color =
                                        reader["Hor_Color"]?.ToString()
                                };

                                // Guardamos los campos necesarios para relacionar
                                // cada horario con su fecha y disponibilidad.
                                horario.Hor_Pac_Id_Hora =
                                    reader["Hor_Pac_Id_Hora"].ToString();

                                horario.Hor_Pac_Fecha_Cita =
                                    Convert.ToDateTime(
                                        reader["Hor_Pac_Fecha_Cita"]);

                                lista.Add(horario);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return lista;
        }
        CXN_HORARIO INuevaAgenda.GetAdmision(int Admision)
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

                    String QueryCons = "SELECT H.Hor_Estado, P.Pac_IdNum, H.Hor_Imp_Age, H.Hor_Pac_Fecha_Cita, H.Hor_Pac_Tipo_Serv " +
                                       "FROM CXN_HORARIO H " +
                                       "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                       "WHERE H.Hor_Id = @param1";

                    using (SqlCommand Command = new SqlCommand(QueryCons, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_HORARIO L = new CXN_HORARIO()
                                {
                                    Hor_Estado = Reader["Hor_Estado"].ToString(),
                                    PacienteIdentificacion = Reader["Pac_IdNum"].ToString(),
                                    Hor_Imp_Age = Reader["Hor_Imp_Age"].ToString(),
                                    Hor_Pac_Fecha_Cita = Convert.ToDateTime(Reader["Hor_Pac_Fecha_Cita"]),
                                    Hor_Pac_Id = Convert.ToInt32(Reader["Hor_Pac_Id"]),
                                    Hor_Pac_Tipo_Serv = Reader["Hor_Pac_Tipo_Serv"].ToString()
                                };
                               
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
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                return null;
            }
        }
    }
}
