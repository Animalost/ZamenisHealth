using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using System.Linq;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MMensajeria : IMensajeria
    {
        private static readonly IGenerales repositorioGenerales = new MGenerales();

        Dictionary<string, string> IMensajeria.getUsersforSendMessage()
        {
            try
            {
                var datConect = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(datConect["Conexion"])) 
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Log_PrimerA, Log_PrimerA + ' ' + Log_SegundoA + ' ' + Log_PrimerN + ' ' + Log_SegundoN AS Persona, Log_Usuario " +
                                   "FROM CXN_LOGIN " +
                                   "WHERE Log_Habilitado = @param1 " +
                                   "ORDER BY Log_PrimerA ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", "A");

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                Dictionary<string, string> Lista = new Dictionary<string, string>();

                                while (Reader.Read() == true)
                                {
                                    Lista.Add(Reader["Log_Usuario"].ToString(), Reader["Persona"].ToString());
                                }

                                return Lista;
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
        string IMensajeria.getUsertoSendMessage(string Name)
        {
            try
            {
                var datConect = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(datConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Log_PrimerA, Log_SegundoA, Log_PrimerN, Log_SegundoN, Log_Usuario " +
                                   "FROM CXN_LOGIN " +
                                   "ORDER BY Log_Id ASC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        string DatoAComprobar = "";

                        while (Reader.Read() == true)
                        {
                            DatoAComprobar = Reader["Log_PrimerA"].ToString() + " " +
                                             Reader["Log_SegundoA"].ToString() + " " +
                                             Reader["Log_PrimerN"].ToString() + " " +
                                             Reader["Log_SegundoN"].ToString();

                            if (DatoAComprobar == Name)
                            {
                                return Reader["Log_Usuario"].ToString();
                            }
                        }

                        return "";
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
                return "";
            }
        }
        bool IMensajeria.SendMessage(CXN_MESSENGER message)
        {
            try
            {
                var datConect = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(datConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now.Date;
                    DateTime Hora = DateTime.Now;

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_MESSENGER (Estado, " +
                                                          "Men_Mensaje, " +
                                                          "Men_Usuario_Para, " +
                                                          "Men_Usuario_De, " +
                                                          "Fecha, " +
                                                          "Hora) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5, " +
                                                          "@param6)", con);

                    cmd.Parameters.AddWithValue("@param1", message.Estado);
                    cmd.Parameters.AddWithValue("@param2", message.Men_Mensaje);
                    cmd.Parameters.AddWithValue("@param3", message.Men_Usuario_Para);
                    cmd.Parameters.AddWithValue("@param4", message.Men_Usuario_De);
                    cmd.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = Hoy;
                    cmd.Parameters.Add(new SqlParameter("@param6", SqlDbType.DateTime)).Value = Hora;
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }    
        List<CXN_MESSENGER> IMensajeria.getAllMessages(string _anotherUser, string meUser, DateTime Fecha)
        {
            try
            {
                Dictionary<string, string> getConect = Conexion.Conection();
                
                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    List<CXN_MESSENGER> historial = new List<CXN_MESSENGER>();

                    String Queryenviados = "SELECT * " +
                                           "FROM CXN_MESSENGER " +
                                           "WHERE Men_Usuario_De = '" + meUser + "' " +
                                           "AND Men_Usuario_Para = '" + _anotherUser + "' " +
                                           "AND Fecha = '" + Convert.ToDateTime(Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                           "ORDER BY Hora DESC";

                    String Queryenviados2 = "SELECT * " +
                                            "FROM CXN_MESSENGER " +
                                            "WHERE Men_Usuario_De = '" + _anotherUser + "' " +
                                            "AND Men_Usuario_Para = '" + meUser + "' " +
                                            "AND Fecha = '" + Convert.ToDateTime(Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                            "ORDER BY Hora DESC";

                    SqlCommand Commando = new SqlCommand(Queryenviados, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {                        
                        while (Reader.Read() == true)
                        {
                            historial.Add(new CXN_MESSENGER
                            {
                                Fecha = Convert.ToDateTime(Reader["Fecha"]),
                                Hora = Convert.ToDateTime(Reader["Hora"]),
                                Men_Mensaje = Reader["Men_Mensaje"].ToString(),
                                Estado = Reader["Estado"].ToString(),
                                Men_Id = Convert.ToInt32(Reader["Men_Id"]),
                                Men_Usuario_De = Reader["Men_Usuario_De"].ToString(),
                                Men_Usuario_Para = Reader["Men_Usuario_Para"].ToString()
                            });
                        }                     
                    }

                    SqlCommand Commando2 = new SqlCommand(Queryenviados2, con);
                    SqlDataReader Reader2 = (Commando2.ExecuteReader());
                    if (Reader2.HasRows)
                    {
                        while (Reader2.Read() == true)
                        {
                            historial.Add(new CXN_MESSENGER
                            {
                                Fecha = Convert.ToDateTime(Reader2["Fecha"]),
                                Hora = Convert.ToDateTime(Reader2["Hora"]),
                                Men_Mensaje = Reader2["Men_Mensaje"].ToString(),
                                Estado = Reader2["Estado"].ToString(),
                                Men_Id = Convert.ToInt32(Reader2["Men_Id"]),
                                Men_Usuario_De = _anotherUser,
                                Men_Usuario_Para = meUser
                            });
                        }
                    }

                    if (historial != null)
                    {
                        List<CXN_MESSENGER> listaOrdenada = historial.OrderByDescending(p => p.Hora).ToList();
                        return listaOrdenada;
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
        void IMensajeria.Leido(int Id)
        {
            var getConect = Conexion.Conection();
            using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string Busqueda = "UPDATE CXN_MESSENGER " +
                                  "SET Estado = 'L' " +
                                  "WHERE Men_Id = '" + Id + "'";
                SqlCommand Accion = new SqlCommand(Busqueda, con);
                int Guarda;
                Guarda = Accion.ExecuteNonQuery();
            }
        }
        CXN_MESSENGER IMensajeria.Mensajes(string User)
        {
            CXN_MESSENGER mensjaes_I = new CXN_MESSENGER();

            try
            {
                var getConect = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    con.Open();

                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Agenda = "SELECT * " +
                                           "FROM CXN_MESSENGER " +
                                           "WHERE Men_Usuario_Para = '" + User + "' " +
                                           "AND Estado = 'E' " +
                                           "ORDER BY Men_Id DESC";
                    SqlCommand Carga_Agenda = new SqlCommand(Cargar_Agenda, con);
                    SqlDataReader Lectura_Agenda = (Carga_Agenda.ExecuteReader());
                    if (Lectura_Agenda.Read() == true)
                    {
                        var Men = repositorioGenerales.Base64Decode(Lectura_Agenda["Men_Mensaje"].ToString());

                        mensjaes_I.Men_Id = Convert.ToInt32(Lectura_Agenda["Men_Id"]);
                        mensjaes_I.Men_Mensaje = Men;
                        mensjaes_I.Men_Usuario_De = Lectura_Agenda["Men_Usuario_De"].ToString();
                    }
                    else
                    {
                        mensjaes_I.Men_Id = 0;
                    }
                    return mensjaes_I;
                }
            }
            catch (Exception ex)
            {
                mensjaes_I.Men_Id = 1;
                mensjaes_I.Men_Mensaje = ex.ToString();
                return mensjaes_I;
            }
        }
    }
}
