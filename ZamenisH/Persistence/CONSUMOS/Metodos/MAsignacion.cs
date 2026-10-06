using Domain;
using Domain.CONSUMOS;
using Persistence.CONSUMOS.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CONSUMOS.Metodos
{
    public class MAsignacion : IAsignacion
    {
        List<CON_ASIGNACION> IAsignacion.GetBodegas()
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CON_ASIGNACION " +
                                   "ORDER BY Asi_Name ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CON_ASIGNACION> C = new List<CON_ASIGNACION>();

                                while (Reader.Read() == true)
                                {
                                    C.Add(new CON_ASIGNACION
                                    {
                                        Asi_Name = Reader["Asi_Name"].ToString(),
                                        Asi_Id = Convert.ToInt32(Reader["Asi_Id"]),
                                        Asi_Number = Convert.ToInt32(Reader["Asi_Number"]),
                                        Asi_Status = (bool)Reader["Asi_Status"]
                                    });
                                }                             

                                return C;
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
        CON_ASIGNACION IAsignacion.GetBodega(int NumerBod)
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CON_ASIGNACION " +
                                   "WHERE Asi_Number = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", NumerBod);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CON_ASIGNACION C = new CON_ASIGNACION
                                {
                                    Asi_Name = Reader["Asi_Name"].ToString(),
                                    Asi_Id = Convert.ToInt32(Reader["Asi_Id"]),
                                    Asi_Number = Convert.ToInt32(Reader["Asi_Number"]),
                                    Asi_Status = (bool)Reader["Asi_Status"]
                                };                              

                                return C;
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
        CON_ASIGNACION IAsignacion.GetBodega(string NameBod)
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CON_ASIGNACION " +
                                   "WHERE Asi_Name = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", NameBod);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CON_ASIGNACION C = new CON_ASIGNACION
                                {
                                    Asi_Name = Reader["Asi_Name"].ToString(),
                                    Asi_Id = Convert.ToInt32(Reader["Asi_Id"]),
                                    Asi_Number = Convert.ToInt32(Reader["Asi_Number"]),
                                    Asi_Status = (bool)Reader["Asi_Status"]
                                };

                                return C;
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
        bool IAsignacion.CreaBodega(CON_ASIGNACION C)
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

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO CON_ASIGNACION (Asi_Name, " +
                                                                  "Asi_Number, " +
                                                                  "Asi_Status) " +
                                         "values                  (@param1, " +
                                                                  "@param2, " +
                                                                  "@param3)", con);

                    cmd.Parameters.AddWithValue("@param1", C.Asi_Name);
                    cmd.Parameters.AddWithValue("@param2", C.Asi_Number);
                    cmd.Parameters.AddWithValue("@param3", C.Asi_Status);
                    return cmd.ExecuteNonQuery() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool IAsignacion.UpdateBodega(CON_ASIGNACION C)
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

                    string Busqueda = "UPDATE CON_ASIGNACION " +
                                      "SET Asi_Name = @param1, " +
                                      "Asi_Status = @param2 " +
                                      "WHERE Asi_Number = @param3";

                    using (SqlCommand Accion = new SqlCommand(Busqueda, con))
                    {
                        Accion.Parameters.AddWithValue("@param1", C.Asi_Name);
                        Accion.Parameters.AddWithValue("@param2", C.Asi_Status);
                        Accion.Parameters.AddWithValue("@param3", C.Asi_Number);

                        return Accion.ExecuteNonQuery() > 0 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
    }
}
