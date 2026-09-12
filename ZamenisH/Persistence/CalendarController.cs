using Domain;
using Domain.CXN;
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
        public CXN_DIAS_WEB GetRazonDiaBloqueado(DateTime Fecha, int Profesional)
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
                                   "WHERE F_Prof = @param2 " +
                                   "AND F_Estado = 'B' " +
                                   "AND F_Fecha = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Convert.ToDateTime(Fecha));
                        Commando.Parameters.AddWithValue("@param2", Profesional);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_DIAS_WEB L = new CXN_DIAS_WEB
                                {
                                    F_Bloquea = Reader["F_Bloquea"].ToString(),
                                    R_Razon = Reader["R_Razon"].ToString()
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public List<CXN_DISPONIBILIDAD_2> GetLlenos(int Profesional)
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
                                   "FROM CXN_DISPONIBILIDAD_2 " +
                                   "WHERE Med = @param1 " +
                                   "AND Habilita = 'A'";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Profesional);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_DISPONIBILIDAD_2> D = new List<CXN_DISPONIBILIDAD_2>();

                                while (Reader.Read() == true)
                                {
                                    D.Add(new CXN_DISPONIBILIDAD_2
                                    {
                                        Dia = Reader["Dia"].ToString(),
                                        Habilita = Reader["Habilita"].ToString(),
                                        Hora = Convert.ToDateTime(Reader["Hora"]),
                                        Id = Convert.ToInt32(Reader["Id"]),
                                        Ide = Reader["Ide"].ToString(),
                                        Med = Convert.ToInt32(Reader["Med"]),
                                    });
                                }

                                return D;
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
        public int CantidadUsada(int Profesional, DateTime Dia)
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

                    String Query = "SELECT Hor_Id " +
                                   "FROM CXN_HORARIO " +
                                   "WHERE Hor_Pac_Bod = @param1 " +
                                   "AND Hor_Pac_Fecha_Cita = @param2";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Profesional);
                        Commando.Parameters.AddWithValue("@param2", Convert.ToDateTime(Dia));

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_HORARIO> D = new List<CXN_HORARIO>();

                                while (Reader.Read() == true)
                                {
                                    D.Add(new CXN_HORARIO
                                    {
                                        Hor_Id = Convert.ToInt32(Reader["Hor_Id"])
                                    });
                                }

                                return D.Count;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public List<DiaCalendarios> ObtenerDiasLlenos(int Profesional,int Año, int Mes)
        {
            List<DiaCalendarios> resultado = new List<DiaCalendarios>();

            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    con.Open();

                    string Query = @"
                                        WITH Disponibilidad AS
                                        (
                                            SELECT
                                                Ide,
                                                Dia,
                                                Habilita, 
                                                Med
                                            FROM CXN_DISPONIBILIDAD_2
                                            WHERE Med = @Profesional
                                        ),
                                        Usados AS
                                            (
                                                SELECT
                                                    H.Hor_Pac_Fecha_Cita,
                                                    COUNT(*) AS CantidadUsada
                                                FROM CXN_HORARIO H
                                                WHERE H.Hor_Pac_Bod = @Profesional
                                                AND H.Hor_Pac_Fecha_Cita BETWEEN @FechaInicio AND @FechaFin
                                                AND H.Hor_Estado NOT IN ('C')
                                                AND H.Hor_Pac_Id_Hora <> @IDEHORA

                                                AND EXISTS
                                                (
                                                    SELECT 1
                                                    FROM CXN_DISPONIBILIDAD_2 D
                                                    WHERE D.Ide = H.Hor_Pac_Id_Hora
                                                    AND D.Med = @Profesional
                                                    AND D.Habilita = 'A'
                                                )

                                                GROUP BY H.Hor_Pac_Fecha_Cita
                                            ),
                                        CantidadDisponibilidad AS
                                        (
                                            SELECT
                                                Dia,
                                                COUNT(*) AS CantidadHabilitada
                                            FROM CXN_DISPONIBILIDAD_2
                                            WHERE Med = @Profesional
                                            AND Habilita = 'A'
                                            GROUP BY Dia
                                        )
                                        SELECT
                                            DAY(U.Hor_Pac_Fecha_Cita) AS Dia,
                                            U.CantidadUsada,
                                            ISNULL(D.CantidadHabilitada, 0) AS CantidadHabilitada
                                        FROM Usados U
                                        LEFT JOIN CantidadDisponibilidad D
                                            ON D.Dia =
                                            CASE DATEPART(WEEKDAY, U.Hor_Pac_Fecha_Cita)
                                                WHEN 1 THEN 'Domingo'
                                                WHEN 2 THEN 'Lunes'
                                                WHEN 3 THEN 'Martes'
                                                WHEN 4 THEN 'Miércoles'
                                                WHEN 5 THEN 'Jueves'
                                                WHEN 6 THEN 'Viernes'
                                                WHEN 7 THEN 'Sábado'
                                            END
                                        ORDER BY U.Hor_Pac_Fecha_Cita;
                                        ";

                    DateTime FechaInicio = new DateTime(Año, Mes, 1);
                    DateTime FechaFin = new DateTime(Año, Mes, GetLastDay(FechaInicio.Month)); 

                    using (SqlCommand comando = new SqlCommand(Query, con))
                    {
                        comando.Parameters.AddWithValue("@Profesional", Profesional);
                        comando.Parameters.AddWithValue("@FechaInicio", Convert.ToDateTime(FechaInicio));
                        comando.Parameters.AddWithValue("@FechaFin", Convert.ToDateTime(FechaFin));
                        comando.Parameters.AddWithValue("@IDEHORA", "0000"); 

                        using (SqlDataReader reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultado.Add(new DiaCalendarios
                                {
                                    Dia = Convert.ToInt32(reader["Dia"]),
                                    CantidadUsada = Convert.ToInt32(reader["CantidadUsada"]),
                                    CantidadHabilitada = Convert.ToInt32(reader["CantidadHabilitada"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return resultado;
        }

        int GetLastDay(int Month)
        {
            switch(Month)
            {
                case 1:
                    return 31;
                case 2:
                    return 28;
                case 3:
                    return 31;
                case 4:
                    return 30;
                case 5:
                    return 31;
                case 6:
                    return 30;
                case 7:
                    return 31;
                case 8:
                    return 31;
                case 9:
                    return 30;
                case 10:
                    return 31;
                case 11:
                    return 30;
                case 12:
                    return 31;
                default:
                    return 0;
            }
        }
    }
}
