using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MDisponibilidad : IDisponibilidad
    {
        List<CXN_DISPONIBILIDAD_2> IDisponibilidad.GetHorarioByMedAndDay(int Bodega, string Dia)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_DISPONIBILIDAD_2 " +
                                   "WHERE Med = @param1  " +
                                   "AND Dia = @param2 " +
                                   "ORDER BY Hora ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Bodega);
                        Commando.Parameters.AddWithValue("@param2", Dia);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_DISPONIBILIDAD_2> D = new List<CXN_DISPONIBILIDAD_2>();

                                while (Reader.Read() == true)
                                {
                                    D.Add(new CXN_DISPONIBILIDAD_2
                                    {
                                        Id = Convert.ToInt32(Reader["Id"]),
                                        Med = Convert.ToInt32(Reader["Med"]),
                                        Dia = Reader["Dia"].ToString(),
                                        Hora = Convert.ToDateTime(Reader["Hora"]),
                                        Habilita = Reader["Habilita"].ToString(),
                                        Ide = Reader["Ide"].ToString()
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        List<CXN_DISPONIBILIDAD_2> IDisponibilidad.getHorariosByCodeMed(int Bodega)
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

                    String Query = "SELECT * " +
                                   "FROM CXN_DISPONIBILIDAD_2 " +
                                   "WHERE Med = '" + Bodega + "' " +
                                   "ORDER BY Med, Dia, Hora ASC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_DISPONIBILIDAD_2> D = new List<CXN_DISPONIBILIDAD_2>();

                        while (Reader.Read() == true)
                        {
                            D.Add(new CXN_DISPONIBILIDAD_2
                            {
                                Id = Convert.ToInt32(Reader["Id"]),
                                Med = Convert.ToInt32(Reader["Med"]),
                                Dia = Reader["Dia"].ToString(),
                                Hora = Convert.ToDateTime(Reader["Hora"]),
                                Habilita = Reader["Habilita"].ToString(),
                                Ide = Reader["Ide"].ToString()
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        bool IDisponibilidad.ConsultarCodigo(CXN_DISPONIBILIDAD_2 D)
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

                    String Query = "SELECT * " +
                                   "FROM CXN_DISPONIBILIDAD_2 " +
                                   "WHERE Med = '" + D.Med + "' " +
                                   "AND Dia = '" + D.Dia + "' " +
                                   "AND Ide = '" + D.Ide + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return true;
            }
        }
        bool IDisponibilidad.CrearHora(CXN_DISPONIBILIDAD_2 D)
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

                    DateTime H = new DateTime(1900, 01, 01, D.Hora.Hour, D.Hora.Minute, D.Hora.Second);

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_DISPONIBILIDAD_2 (Med, " +
                                                          "Dia, " +
                                                          "Hora, " +
                                                          "Ide, " +
                                                          "Habilita) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5)", con);

                    cmd.Parameters.AddWithValue("@param1", D.Med);
                    cmd.Parameters.AddWithValue("@param2", D.Dia);
                    cmd.Parameters.Add(new SqlParameter("@param3", SqlDbType.DateTime)).Value = H;
                    cmd.Parameters.AddWithValue("@param4", D.Ide.ToString().ToUpper());
                    cmd.Parameters.AddWithValue("@param5", D.Habilita.ToString().ToUpper());
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (SqlException ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        (string Habilita, DateTime Hora) IDisponibilidad.ConsultarHora(int Posision)
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

                    String Query = "SELECT Hora, Habilita FROM CXN_DISPONIBILIDAD_2 WHERE Id = '" + Posision + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return (Reader["Habilita"].ToString(), Convert.ToDateTime(Reader["Hora"]));
                    }
                    else
                    {
                        DateTime d = new DateTime(00, 00, 00);
                        return ("", d);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                DateTime d = new DateTime(00, 00, 00);
                return ("", d);
            }
        }
        bool IDisponibilidad.UpdateHora(CXN_DISPONIBILIDAD_2 D)
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

                    string Busqueda = "UPDATE CXN_DISPONIBILIDAD_2 " +
                                                "SET Hora = '" + Convert.ToDateTime(D.Hora).ToString("HH:mm:ss") + "', " +
                                                "Habilita = '" + D.Habilita + "' " +
                                                "WHERE Id = '" + D.Id + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();

                    return true;
                }
            }
            catch (SqlException ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
    }
}
