using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.Fibromialgia;
using Persistence.Fibromialgia.Interfaces;
using Domain;

namespace Persistence.Fibromialgia.Metodos
{
    public class MEncuestas : IEncuestas
    {
        List<FIB_ENCUESTA1> IEncuestas.getPreviosXPacEncuesta1(string tdoc, string doc)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT E.Id, E.FechaEncuesta, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + " +
                                   "P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS Paciente, E.UsuarioRegistra, E.Estado " +
                                   "FROM FIB_ENCUESTA1 E  " +
                                   "INNER JOIN CXN_PACIENTES P ON E.IdPAciente = P.Pac_Id " +
                                   "WHERE P.Pac_TipoId = '" + tdoc + "' " +
                                   "AND P.Pac_IdNum = '" + doc + "' " +
                                   "AND E.Estado = 'V' " +
                                   "ORDER BY E.Id DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<FIB_ENCUESTA1> E = new List<FIB_ENCUESTA1>();

                        while (Reader.Read() == true)
                        {
                            E.Add(new FIB_ENCUESTA1
                            {
                                Id = Convert.ToInt32(Reader["Id"]),
                                FechaEncuesta = Convert.ToDateTime(Reader["FechaEncuesta"]),
                                UsuarioCambia = Reader["Paciente"].ToString(), // Paciente en este caso
                                Estado = Reader["Estado"].ToString(),
                                UsuarioRegistra = Reader["UsuarioRegistra"].ToString()
                            });
                        }

                        return E;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        List<FIB_ENCUESTA1> IEncuestas.getPreviosGeneralEncuesta1()
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT E.Id, E.FechaEncuesta, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + " +
                                   "P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS Paciente, E.UsuarioRegistra, E.Estado " +
                                   "FROM FIB_ENCUESTA1 E  " +
                                   "INNER JOIN CXN_PACIENTES P ON E.IdPaciente = P.Pac_Id " +
                                   "WHERE E.Estado = 'V' " +
                                   "ORDER BY E.Id DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<FIB_ENCUESTA1> E = new List<FIB_ENCUESTA1>();

                        while (Reader.Read() == true)
                        {
                            E.Add(new FIB_ENCUESTA1
                            {
                                Id = Convert.ToInt32(Reader["Id"]),
                                FechaEncuesta = Convert.ToDateTime(Reader["FechaEncuesta"]),
                                UsuarioCambia = Reader["Paciente"].ToString(), // Paciente en este caso
                                UsuarioRegistra = Reader["UsuarioRegistra"].ToString(),
                                Estado = Reader["Estado"].ToString()
                            });
                        }

                        return E;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        List<FIB_ENCUESTA2> IEncuestas.getPreviosXPacEncuesta2(string tdoc, string doc)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT E.Id, E.FechaEncuesta, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + " +
                                   "P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS Paciente, E.UsuarioRegistra, E.Estado " +
                                   "FROM FIB_ENCUESTA2 E  " +
                                   "INNER JOIN CXN_PACIENTES P ON E.IdPAciente = P.Pac_Id " +
                                   "WHERE P.Pac_TipoId = '" + tdoc + "' " +
                                   "AND P.Pac_IdNum = '" + doc + "' " +
                                   "AND E.Estado = 'V' " +
                                   "ORDER BY E.Id DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<FIB_ENCUESTA2> E = new List<FIB_ENCUESTA2>();

                        while (Reader.Read() == true)
                        {
                            E.Add(new FIB_ENCUESTA2
                            {
                                Id = Convert.ToInt32(Reader["Id"]),
                                FechaEncuesta = Convert.ToDateTime(Reader["FechaEncuesta"]),
                                UsuarioCambia = Reader["Paciente"].ToString(), // Paciente en este caso
                                Estado = Reader["Estado"].ToString(),
                                UsuarioRegistra = Reader["UsuarioRegistra"].ToString()
                            });
                        }

                        return E;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        List<FIB_ENCUESTA2> IEncuestas.getPreviosGeneralEncuesta2()
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT E.Id, E.FechaEncuesta, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + " +
                                   "P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS Paciente, E.UsuarioRegistra, E.Estado " +
                                   "FROM FIB_ENCUESTA2 E  " +
                                   "INNER JOIN CXN_PACIENTES P ON E.IdPaciente = P.Pac_Id " +
                                   "WHERE E.Estado = 'V' " +
                                   "ORDER BY E.Id DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<FIB_ENCUESTA2> E = new List<FIB_ENCUESTA2>();

                        while (Reader.Read() == true)
                        {
                            E.Add(new FIB_ENCUESTA2
                            {
                                Id = Convert.ToInt32(Reader["Id"]),
                                FechaEncuesta = Convert.ToDateTime(Reader["FechaEncuesta"]),
                                UsuarioCambia = Reader["Paciente"].ToString(), // Paciente en este caso
                                Estado = Reader["Estado"].ToString(),
                                UsuarioRegistra = Reader["UsuarioRegistra"].ToString()
                            });
                        }

                        return E;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        List<FIB_ENCUESTA3> IEncuestas.getPreviosXPacEncuesta3(string tdoc, string doc)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT E.Id, E.FechaEncuesta, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + " +
                                   "P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS Paciente, E.UsuarioRegistra, E.Estado " +
                                   "FROM FIB_ENCUESTA3 E  " +
                                   "INNER JOIN CXN_PACIENTES P ON E.IdPAciente = P.Pac_Id " +
                                   "WHERE P.Pac_TipoId = '" + tdoc + "' " +
                                   "AND P.Pac_IdNum = '" + doc + "' " +
                                   "AND E.Estado = 'V' " +
                                   "ORDER BY E.Id DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<FIB_ENCUESTA3> E = new List<FIB_ENCUESTA3>();

                        while (Reader.Read() == true)
                        {
                            E.Add(new FIB_ENCUESTA3
                            {
                                Id = Convert.ToInt32(Reader["Id"]),
                                FechaEncuesta = Convert.ToDateTime(Reader["FechaEncuesta"]),
                                UsuarioCambia = Reader["Paciente"].ToString(), // Paciente en este caso
                                Estado = Reader["Estado"].ToString(),
                                UsuarioRegistra = Reader["UsuarioRegistra"].ToString()
                            });
                        }

                        return E;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        List<FIB_ENCUESTA3> IEncuestas.getPreviosGeneralEncuesta3()
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT E.Id, E.FechaEncuesta, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + " +
                                   "P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS Paciente, E.UsuarioRegistra, E.Estado " +
                                   "FROM FIB_ENCUESTA3 E  " +
                                   "INNER JOIN CXN_PACIENTES P ON E.IdPaciente = P.Pac_Id " +
                                   "WHERE E.Estado = 'V' " +
                                   "ORDER BY E.Id DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<FIB_ENCUESTA3> E = new List<FIB_ENCUESTA3>();

                        while (Reader.Read() == true)
                        {
                            E.Add(new FIB_ENCUESTA3
                            {
                                Id = Convert.ToInt32(Reader["Id"]),
                                FechaEncuesta = Convert.ToDateTime(Reader["FechaEncuesta"]),
                                UsuarioCambia = Reader["Paciente"].ToString(), // Paciente en este caso
                                Estado = Reader["Estado"].ToString(),
                                UsuarioRegistra = Reader["UsuarioRegistra"].ToString()
                            });
                        }

                        return E;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        bool IEncuestas.ExcluirEncuesta(int Posision, string TipoEncuesta, string User)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now;

                    string Busqueda = "";

                    if (TipoEncuesta == "Encuesta1")
                    {
                        Busqueda = "UPDATE FIB_ENCUESTA1 " +
                                          "SET Estado = 'A', " +
                                          "UsuarioCambia = '" + User + "', " +
                                          "FechaCambio = '" + Convert.ToDateTime(Hoy).ToString(getData["Format_Fecha"]) + "' " +
                                          "WHERE Id = '" + Posision + "'";
                    }
                    else if (TipoEncuesta == "Encuesta2")
                    {
                        Busqueda = "UPDATE FIB_ENCUESTA2 " +
                                          "SET Estado = 'A', " +
                                          "UsuarioCambia = '" + User + "', " +
                                          "FechaCambio = '" + Convert.ToDateTime(Hoy).ToString(getData["Format_Fecha"]) + "' " +
                                          "WHERE Id = '" + Posision + "'";
                    }
                    else if (TipoEncuesta == "Encuesta3")
                    {
                        Busqueda = "UPDATE FIB_ENCUESTA3 " +
                                          "SET Estado = 'A', " +
                                          "UsuarioCambia = '" + User + "', " +
                                          "FechaCambio = '" + Convert.ToDateTime(Hoy).ToString(getData["Format_Fecha"]) + "' " +
                                          "WHERE Id = '" + Posision + "'";
                    }
                    else
                    {
                        return false;
                    }

                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }

    }
}
