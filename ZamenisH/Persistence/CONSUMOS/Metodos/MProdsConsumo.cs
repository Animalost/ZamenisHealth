using Domain;
using Domain.CONSUMOS;
using Persistence.CONSUMOS.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CONSUMOS.Metodos
{
    public class MProdsConsumo : IProdsConsumo
    {
        List<CON_PRODUCTOS> IProdsConsumo.GetProductos(string Filtro)
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

                    String Query = ""; 

                    if (string.IsNullOrEmpty(Filtro))
                    {
                        Query = "SELECT * " +
                                  "FROM CON_PRODUCTOS " +
                                  "ORDER BY Con_Prod_Name ASC";
                    }
                    else
                    {
                        Query = "SELECT * " +
                                "FROM CON_PRODUCTOS " +
                                "WHERE Con_Prod_Name LIKE '%" + Filtro + "%' " +
                                "ORDER BY Con_Prod_Name ASC";
                    }

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CON_PRODUCTOS> C = new List<CON_PRODUCTOS>();

                                while (Reader.Read() == true)
                                {
                                    C.Add(new CON_PRODUCTOS
                                    {
                                        Con_Prod_Cod_Externo = Reader["Con_Prod_Cod_Externo"].ToString(),
                                        Con_Prod_Id = Convert.ToInt32(Reader["Con_Prod_Id"]),
                                        Con_Prod_Cod_Interno = Reader["Con_Prod_Cod_Interno"].ToString(),
                                        Con_Prod_Status = (bool)Reader["Con_Prod_Status"],
                                        Con_Prod_Name = Reader["Con_Prod_Name"].ToString()
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
        List<CON_PRODUCTOS> IProdsConsumo.GetProductoByCodeInterno(string Filtro)
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

                    String Query = @"SELECT * 
                                    FROM CON_PRODUCTOS 
                                    WHERE Con_Prod_Cod_Interno = @param1 
                                    ORDER BY Con_Prod_Name ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Filtro);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CON_PRODUCTOS> C = new List<CON_PRODUCTOS>();

                                while (Reader.Read() == true)
                                {
                                    C.Add(new CON_PRODUCTOS
                                    {
                                        Con_Prod_Cod_Externo = Reader["Con_Prod_Cod_Externo"].ToString(),
                                        Con_Prod_Id = Convert.ToInt32(Reader["Con_Prod_Id"]),
                                        Con_Prod_Cod_Interno = Reader["Con_Prod_Cod_Interno"].ToString(),
                                        Con_Prod_Status = (bool)Reader["Con_Prod_Status"],
                                        Con_Prod_Name = Reader["Con_Prod_Name"].ToString()
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
        CON_PRODUCTOS IProdsConsumo.GetProductoById(int Id)
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
                                   "FROM CON_PRODUCTOS " +
                                   "WHERE Con_Prod_Id = @param1";
                    
                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Id);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CON_PRODUCTOS C = new CON_PRODUCTOS
                                {
                                    Con_Prod_Cod_Externo = Reader["Con_Prod_Cod_Externo"].ToString(),
                                    Con_Prod_Id = Convert.ToInt32(Reader["Con_Prod_Id"]),
                                    Con_Prod_Cod_Interno = Reader["Con_Prod_Cod_Interno"].ToString(),
                                    Con_Prod_Status = (bool)Reader["Con_Prod_Status"],
                                    Con_Prod_Name = Reader["Con_Prod_Name"].ToString()
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
        CON_PRODUCTOS IProdsConsumo.GetProductoByCodeExtern(string Code)
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
                                   "FROM CON_PRODUCTOS " +
                                   "WHERE Con_Prod_Cod_Externo = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Code);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CON_PRODUCTOS C = new CON_PRODUCTOS
                                {
                                    Con_Prod_Cod_Externo = Reader["Con_Prod_Cod_Externo"].ToString(),
                                    Con_Prod_Id = Convert.ToInt32(Reader["Con_Prod_Id"]),
                                    Con_Prod_Cod_Interno = Reader["Con_Prod_Cod_Interno"].ToString(),
                                    Con_Prod_Status = (bool)Reader["Con_Prod_Status"],
                                    Con_Prod_Name = Reader["Con_Prod_Name"].ToString()
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
        bool IProdsConsumo.CreaProducto(CON_PRODUCTOS C)
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

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO CON_PRODUCTOS (Con_Prod_Cod_Interno, " +
                                                                  "Con_Prod_Cod_Externo, " +
                                                                  "Con_Prod_Name, " +
                                                                  "Con_Prod_Status) " +
                                         "values                  (@param1, " +
                                                                  "@param2, " +
                                                                  "@param3, " +
                                                                  "@param4)", con);

                    cmd.Parameters.AddWithValue("@param1", C.Con_Prod_Cod_Interno);
                    cmd.Parameters.AddWithValue("@param2", C.Con_Prod_Cod_Externo);
                    cmd.Parameters.AddWithValue("@param3", C.Con_Prod_Name);
                    cmd.Parameters.AddWithValue("@param4", C.Con_Prod_Status);
                    return cmd.ExecuteNonQuery() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool IProdsConsumo.UpdateProducto(CON_PRODUCTOS C)
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

                    string Busqueda = "UPDATE CON_PRODUCTOS " +
                                      "SET Con_Prod_Cod_Interno = @param1, " +
                                      "Con_Prod_Cod_Externo = @param2, " +
                                      "Con_Prod_Name = @param3, " +
                                      "Con_Prod_Status = @param4 " +
                                      "WHERE Con_Prod_Id = @param5";

                    using (SqlCommand Accion = new SqlCommand(Busqueda, con))
                    {
                        Accion.Parameters.AddWithValue("@param1", C.Con_Prod_Cod_Interno);
                        Accion.Parameters.AddWithValue("@param2", C.Con_Prod_Cod_Externo);
                        Accion.Parameters.AddWithValue("@param3", C.Con_Prod_Name);
                        Accion.Parameters.AddWithValue("@param4", C.Con_Prod_Status);
                        Accion.Parameters.AddWithValue("@param5", C.Con_Prod_Id);

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
