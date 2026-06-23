using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MAdherencia : IAdherencia
    {
        List<CXN_ADHERENCIA> IAdherencia.listaApositos()
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

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_ADHERENCIA " +
                                         "ORDER BY Adh_Aposito ASC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_ADHERENCIA> L = new List<CXN_ADHERENCIA>();

                        while (Lectura_Hora.Read() == true)
                        {
                            L.Add(new CXN_ADHERENCIA
                            {
                                Adh_Aposito = Lectura_Hora["Adh_Aposito"].ToString(),
                                Adh_Descripcion = Lectura_Hora["Adh_Descripcion"].ToString()
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        string IAdherencia.DescripcionAposito(string Aposito)
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

                    String Cargar_Hora = "SELECT Adh_Descripcion " +
                                         "FROM CXN_ADHERENCIA " +
                                         "WHERE Adh_Aposito = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Aposito);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Lectura_Hora["Adh_Descripcion"].ToString();
                            }
                            else
                            {
                                return "";
                            }
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        bool IAdherencia.Crea(CXN_ADHERENCIA A)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_ADHERENCIA (Adh_Aposito, " +
                                                          "Adh_Descripcion) " +
                                 "values                  (@param1, " +
                                                          "@param2)", con);

                    cmd.Parameters.AddWithValue("@param1", A.Adh_Aposito);
                    cmd.Parameters.AddWithValue("@param2", A.Adh_Descripcion);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        bool IAdherencia.Actualiza(CXN_ADHERENCIA A)
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

                    string Busqueda = "UPDATE CXN_ADHERENCIA " +
                                      "SET " +
                                      "Adh_Descripcion = '" + A.Adh_Descripcion + "' " +
                                      "WHERE Adh_Aposito = '" + A.Adh_Aposito + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
