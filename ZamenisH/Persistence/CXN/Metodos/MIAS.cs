using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Persistence.CXN.Metodos
{
    public class MIAS : IIAS
    {
        List<CXN_HISTORYIA> IIAS.Historial(string Usuario)
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

                    String Cargar_Hora = "SELECT TOP 50 * " +
                                         "FROM CXN_HISTORYIA " +
                                         "WHERE Usuario = @param1 " +
                                         "ORDER BY Id DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Usuario);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_HISTORYIA> L = new List<CXN_HISTORYIA>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new CXN_HISTORYIA
                                    {
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        IA = Lectura_Hora["IA"].ToString(),
                                        Consulta = Lectura_Hora["Consulta"].ToString(),
                                        FechaHora = Convert.ToDateTime(Lectura_Hora["FechaHora"]),
                                        Respuesta = Lectura_Hora["Respuesta"].ToString(),
                                        Usuario = Lectura_Hora["Usuario"].ToString()
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        bool IIAS.Insertar(CXN_HISTORYIA H)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_HISTORYIA (FechaHora, " +
                                                          "Consulta, " +
                                                          "Respuesta, " +
                                                          "Usuario, " +
                                                          "IA) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5)", con);

                    cmd.Parameters.AddWithValue("@param1", DateTime.Now);
                    cmd.Parameters.AddWithValue("@param2", H.Consulta);
                    cmd.Parameters.AddWithValue("@param3", H.Respuesta);
                    cmd.Parameters.AddWithValue("@param4", H.Usuario);
                    cmd.Parameters.AddWithValue("@param5", H.IA);
                    int save = cmd.ExecuteNonQuery();
                    return save > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        CXN_HISTORYIA IIAS.Historial(int id)
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

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_HISTORYIA " +
                                         "WHERE Id = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", id);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {

                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_HISTORYIA L = new CXN_HISTORYIA
                                {
                                    Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                    IA = Lectura_Hora["IA"].ToString(),
                                    Consulta = Lectura_Hora["Consulta"].ToString(),
                                    FechaHora = Convert.ToDateTime(Lectura_Hora["FechaHora"]),
                                    Respuesta = Lectura_Hora["Respuesta"].ToString(),
                                    Usuario = Lectura_Hora["Usuario"].ToString()
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
    }
}
