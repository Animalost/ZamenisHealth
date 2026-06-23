using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MConvenios : IConvenios
    {
        CXN_CONVENIOS IConvenios.ServicioNombre(string CUP, int Ase, string Tipo)
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
                   
                    String Query = "SELECT Con_Nombre, Con_Valor, Con_Id_Serv, Con_Tipo_Serv " +
                                   "FROM CXN_CONVENIOS " +
                                   "WHERE Con_Aseguradora = @param1 " +
                                   "AND Con_Id_Serv = @param2 " +
                                   "AND Con_Tipo_Serv = @param3";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Ase);
                        Commando.Parameters.AddWithValue("@param2", CUP);
                        Commando.Parameters.AddWithValue("@param3", Tipo);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_CONVENIOS D = new CXN_CONVENIOS()
                                {
                                    Con_Nombre = Reader["Con_Nombre"].ToString(),
                                    Con_Valor = Convert.ToInt32(Reader["Con_Valor"]),
                                    Con_Id_Serv = Reader["Con_Id_Serv"].ToString(),
                                    Con_Tipo_Serv = Reader["Con_Tipo_Serv"].ToString()
                                };
                                
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
        CXN_CONVENIOS IConvenios.DatosServicioXNameAse(int Ase, string Name)
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
                    String Query = "SELECT Con_Nombre, Con_Valor, Con_Id_Serv, Con_Tipo_Serv " +
                                   "FROM CXN_CONVENIOS " +
                                   "WHERE Con_Aseguradora = '" + Ase + "' " +
                                   "AND Con_Nombre = '" + Name + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        CXN_CONVENIOS D = new CXN_CONVENIOS();
                        D.Con_Nombre = Reader["Con_Nombre"].ToString();
                        D.Con_Valor = Convert.ToInt32(Reader["Con_Valor"]);
                        D.Con_Id_Serv = Reader["Con_Id_Serv"].ToString();
                        D.Con_Tipo_Serv = Reader["Con_Tipo_Serv"].ToString();
                        return D;
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
        List<string> IConvenios.CargarServiciosxASE(int Ase)
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

                    String Query = "SELECT Con_Nombre " +
                                   "FROM CXN_CONVENIOS " +
                                   "WHERE Con_Aseguradora = @param1 " +
                                   "ORDER BY Con_Nombre ASC";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Ase);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<string> L = new List<string>();
                                while (Reader.Read() == true)
                                {
                                    L.Add(Reader["Con_Nombre"].ToString());
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
        string IConvenios.NameServiceCUP(string Name)
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

                    String Query = "SELECT TOP 1 Con_Nombre " +
                                   "FROM CXN_CONVENIOS " +
                                   "WHERE Con_Id_Serv = '" + Name + "'";
                    SqlCommand Command = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Command.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return Reader["Con_Nombre"].ToString();
                    }
                    else
                    {
                        return "NO REGISTRA SERVICIO";
                    }
                }
            }
            catch
            {
                return "NO REGISTRA SERVICIO";
            }
        }
        CXN_CONVENIOS IConvenios.ServicioCUP(string Nombre, int Ase)
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
                    String Query = "SELECT Con_Nombre, Con_Valor, Con_Id_Serv, Con_Tipo_Serv " +
                                   "FROM CXN_CONVENIOS " +
                                   "WHERE Con_Aseguradora = @param1 " +
                                   "AND Con_Nombre = @param2";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Ase);
                        Commando.Parameters.AddWithValue("@param2", Nombre);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_CONVENIOS D = new CXN_CONVENIOS()
                                {
                                    Con_Id_Serv = Reader["Con_Id_Serv"].ToString(),
                                    Con_Tipo_Serv = Reader["Con_Tipo_Serv"].ToString(),
                                    Con_Valor = Convert.ToInt32(Reader["Con_Valor"])
                                };
                                
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
        List<string> IConvenios.CargarServicios(string TipoMed, int Ase)
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

                    String Query = "SELECT Con_Nombre " +
                                   "FROM CXN_CONVENIOS " +
                                   "WHERE Con_Aseguradora = '" + Ase + "' " +
                                   "AND Con_Tipo_Serv = '" + TipoMed + "'";
                    SqlCommand Command = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Command.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<string> L = new List<string>();
                        while (Reader.Read() == true)
                        {
                            L.Add(Reader["Con_Nombre"].ToString());
                        }

                        return L;
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
        List<CXN_CONVENIOS> IConvenios.getConvenios()
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

                    String Query = "SELECT C.Con_Id, C.Con_Nombre, C.Con_Id_Serv, A.Ase_Descripcion, C.Con_Valor, A.Ase_Identificador " +
                                   "FROM CXN_CONVENIOS C " +
                                   "INNER JOIN CXN_ASEGURADORA A ON C.Con_Aseguradora = A.Ase_Identificador " +
                                   "ORDER BY A.Ase_Descripcion, C.Con_Nombre ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_CONVENIOS> lista = new List<CXN_CONVENIOS>();

                                while (Reader.Read() == true)
                                {
                                    lista.Add(new CXN_CONVENIOS
                                    {
                                        Con_Id_Serv = Reader["Con_Id_Serv"].ToString(),
                                        Con_Id = Convert.ToInt32(Reader["Con_Id"]),
                                        Con_Nombre = Reader["Con_Nombre"].ToString(),
                                        Con_Clase = Reader["Ase_Descripcion"].ToString(),
                                        Con_Valor = Convert.ToInt32(Reader["Con_Valor"]),
                                        Con_Aseguradora = Reader["Ase_Identificador"].ToString()
                                    });
                                }

                                return lista;
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
        List<CXN_CONVENIOS> IConvenios.getConvenios(int Ase)
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

                    String Query = "SELECT Con_Nombre, Con_Id_Serv, Con_Valor, Con_Tipo_Serv, Con_Clase " +
                                   "FROM CXN_CONVENIOS " +                                   
                                   "WHERE Con_ASeguradora = @param1 " +
                                   "ORDER BY Con_Nombre ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Ase);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_CONVENIOS> lista = new List<CXN_CONVENIOS>();

                                while (Reader.Read() == true)
                                {
                                    lista.Add(new CXN_CONVENIOS
                                    {
                                        Con_Id_Serv = Reader["Con_Id_Serv"].ToString(),
                                        Con_Nombre = Reader["Con_Nombre"].ToString(),
                                        Con_Valor = Convert.ToInt32(Reader["Con_Valor"]),
                                        Con_Tipo_Serv = Reader["Con_Tipo_Serv"].ToString(),
                                        Con_Clase = Reader["Con_Clase"].ToString()
                                    });
                                }

                                return lista;
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
        CXN_CONVENIOS IConvenios.getConvenio(int Posision)
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
                    String Query = "SELECT * FROM CXN_CONVENIOS " +
                                   "INNER JOIN CXN_ASEGURADORA ON CXN_CONVENIOS.Con_Aseguradora = CXN_ASEGURADORA.Ase_Identificador " +
                                   "WHERE Con_Id = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Posision);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_CONVENIOS D = new CXN_CONVENIOS
                                {
                                    Con_Id_Serv = Reader["Con_Id_Serv"].ToString(),
                                    Con_Tipo_Serv = Reader["Con_Tipo_Serv"].ToString(),
                                    Con_Valor = Convert.ToInt32(Reader["Con_Valor"]),
                                    Con_Clase = Reader["Ase_Descripcion"].ToString(),
                                    Con_Nombre = Reader["Con_Nombre"].ToString(),
                                    Con_CUP = Reader["Con_CUP"].ToString(),
                                    Con_CodServicio = Reader["Con_CodServicio"].ToString()
                                };

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
            catch
            {
                return null;
            }
        }
        bool IConvenios.updateConvenio(CXN_CONVENIOS C)
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

                    string Busqueda = "UPDATE CXN_CONVENIOS " +
                                      "SET Con_Nombre = '" + C.Con_Nombre + "', " +
                                      "Con_Valor = '" + C.Con_Valor + "', " +
                                      "Con_UsuarioGraba = '" + C.Con_UsuarioGraba + "', " +
                                      "Con_CUP = '" + C.Con_CUP + "', " +
                                      "Con_CodServicio = '" + C.Con_CodServicio + "' " +
                                      "WHERE Con_Id_Serv = '" + C.Con_Id_Serv + "' " +
                                      "AND Con_Aseguradora = '" + C.Con_Aseguradora + "'";
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
        bool IConvenios.createConvenio(CXN_CONVENIOS C)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_CONVENIOS (Con_Id_Serv, " + //param1
                                                           "Con_Nombre, " + //param2
                                                           "Con_Tipo_Serv, " + //param3
                                                           "Con_Valor, " + //param4
                                                           "Con_Aseguradora, " + //param5
                                                           "Con_UsuarioGraba, " +
                                                           "Con_CUP, " +
                                                           "Con_CodServicio) " + //param16
                                  "values                  (@param1, " + // Hor_Estado
                                                           "@param2, " + // Hor_Pac_Id
                                                           "@param3, " + // Hor_Pac_Bod
                                                           "@param4, " + // Hor_Pac_Tipo_Serv
                                                           "@param5, " + // Hor_Pac_Cia
                                                           "@param6, " +
                                                           "@param7, " +
                                                           "@param8)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", C.Con_Id_Serv);
                    cmd.Parameters.AddWithValue("@param2", C.Con_Nombre);
                    cmd.Parameters.AddWithValue("@param3", C.Con_Tipo_Serv);
                    cmd.Parameters.AddWithValue("@param4", C.Con_Valor);
                    cmd.Parameters.AddWithValue("@param5", C.Con_Aseguradora);
                    cmd.Parameters.AddWithValue("@param6", C.Con_UsuarioGraba);
                    cmd.Parameters.AddWithValue("@param7", C.Con_CUP);
                    cmd.Parameters.AddWithValue("@param8", C.Con_CodServicio);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        List<CXN_CONVENIOS> IConvenios.getServicesXAseServ(int Ase, string Serv)
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

                    String Query = "SELECT Con_Id_Serv, Con_Valor, Con_Nombre " +
                                    "FROM CXN_CONVENIOS " +
                                    "WHERE Con_Aseguradora = '" + Ase + "' " +
                                    "AND Con_Tipo_Serv = '" + Serv + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_CONVENIOS> L = new List<CXN_CONVENIOS>();

                        while (Reader.Read() == true)
                        {
                            L.Add(new CXN_CONVENIOS
                            {
                                Con_Id_Serv = Reader["Con_Id_Serv"].ToString(),
                                Con_Nombre = Reader["Con_Nombre"].ToString(),
                                Con_Valor = Convert.ToInt32(Reader["Con_Valor"])
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
        List<string> IConvenios.CargarCUPS(int Posision)
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

                    String Query = "SELECT C.Con_Id_Serv, C.Con_Nombre " +
                                   "FROM CXN_CONVENIOS C " +
                                   "INNER JOIN CXN_CARGOS CA ON C.Con_Aseguradora = CA.Car_Ase " +
                                   "AND C.Con_Tipo_Serv = CA.Car_Tipo_Serv " +
                                   "WHERE CA.Car_Id = '" + Posision + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<string> L = new List<string>();

                        while (Reader.Read() == true)
                        {
                            L.Add(Reader["Con_Nombre"].ToString());

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
    }
}
