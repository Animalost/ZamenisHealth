using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MCIE10 : ICIE10
    {
        List<CXN_CIE10> ICIE10.PorCodigo(string codigo)
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

                    String Cargar_Hora = "SELECT * FROM CXN_CIE10 WHERE Cie_Cod Like '%" + codigo + "%'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_CIE10> C = new List<CXN_CIE10>();

                        while (Lectura_Hora.Read() == true)
                        {
                            C.Add(new CXN_CIE10
                            {
                                Cie_Cod = Lectura_Hora["Cie_Cod"].ToString(),
                                Cie_Serv = Lectura_Hora["Cie_Serv"].ToString(),
                                Cie_Id = Convert.ToInt32(Lectura_Hora["Cie_Id"])
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
            catch
            {
                return null;
            }
        }
        List<CXN_CIE10> ICIE10.Ultimos(int Medico)
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
                    String Cargar_Hora = "SELECT TOP 10 Cie_Cod, Cie_Serv " +
                                         "FROM( " +
                                         "SELECT CI.Cie_Cod, CI.Cie_Serv, " +
                                         "ROW_NUMBER() OVER(PARTITION BY CI.Cie_Cod, CI.Cie_Serv ORDER BY C.Car_Id DESC) AS RowNum  " +
                                         "FROM CXN_CARGOS C  " +
                                         "INNER JOIN CXN_CIE10 CI ON C.Car_DX1 = CI.Cie_Cod  " +
                                         "WHERE C.Car_Prof = @param1  " +
                                         ") AS Subconsulta " +
                                         "WHERE RowNum = 1";
                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Medico);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {                            
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_CIE10> C = new List<CXN_CIE10>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_CIE10
                                    {
                                        Cie_Cod = Lectura_Hora["Cie_Cod"].ToString(),
                                        Cie_Serv = Lectura_Hora["Cie_Serv"].ToString()
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
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        List<CXN_CIE10> ICIE10.PorDesc(string servicio)
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

                    String Cargar_Hora = "SELECT * FROM CXN_CIE10 WHERE Cie_Serv Like '%" + servicio + "%'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_CIE10> C = new List<CXN_CIE10>();

                        while (Lectura_Hora.Read() == true)
                        {
                            C.Add(new CXN_CIE10
                            {
                                Cie_Cod = Lectura_Hora["Cie_Cod"].ToString(),
                                Cie_Serv = Lectura_Hora["Cie_Serv"].ToString(),
                                Cie_Id = Convert.ToInt32(Lectura_Hora["Cie_Id"])
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
            catch
            {
                return null;
            }
        }
        CXN_OM ICIE10.CargaDX(int Paciente)
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
                    String Cargar_Hora = "SELECT TOP 1 Car_DX1, Car_DX2, Car_DX3 " +
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Pac = @param1 " +
                                         "AND Car_DX1 <> '' " +
                                         "ORDER BY Car_Fecha DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Paciente);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_OM HC = new CXN_OM
                                {
                                    OM_DX1 = Lectura_Hora["Car_DX1"] == DBNull.Value ? "" : Lectura_Hora["Car_DX1"].ToString(),
                                    OM_DX2 = Lectura_Hora["Car_DX2"] == DBNull.Value ? "" : Lectura_Hora["Car_DX2"].ToString(),
                                    OM_DX3 = Lectura_Hora["Car_DX3"] == DBNull.Value ? "" : Lectura_Hora["Car_DX3"].ToString(),
                                };

                                return HC;
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
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        string ICIE10.BuscaDX(string CodDX)
        {
            Dictionary<string,string> getData = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getData["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Cargar_Hora2 = "SELECT Cie_Serv " +
                                      "FROM CXN_CIE10 " +
                                      "WHERE Cie_Cod = @param1";

                using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                {
                    Carga_Command2.Parameters.AddWithValue("@param1", CodDX);

                    using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                    {
                        if (Lectura_Hora2.Read() == true)
                        {
                            return Lectura_Hora2["Cie_Serv"].ToString();
                        }
                        else
                        {
                            return "";
                        }
                    }
                }                                  
            }
        }
        bool ICIE10.CreaCIE10(CXN_CIE10 C)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_CIE10 (Cie_Cod, " +
                                                                  "Cie_Serv) " +
                                         "values                  (@param1, " +
                                                                  "@param2)", con);

                    cmd.Parameters.AddWithValue("@param1", C.Cie_Cod);
                    cmd.Parameters.AddWithValue("@param2", C.Cie_Serv);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool ICIE10.UpdateCIE10(CXN_CIE10 C)
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

                    string Busqueda = "UPDATE CXN_CIE10 " +
                                     "SET Cie_Serv = '" + C.Cie_Serv + "' " +
                                     "WHERE Cie_Cod = '" + C.Cie_Cod + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        CXN_OM ICIE10.CargaDXByAdmitionHCMG(int Admision)
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

                    String Cargar_Hora = "SELECT Car_DX1, Car_DX2, Car_DX3 " +
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Adm_Id = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_OM HC = new CXN_OM
                                {
                                    OM_DX1 = Lectura_Hora["Car_DX1"] == DBNull.Value ? "" : Lectura_Hora["Car_DX1"].ToString(),
                                    OM_DX2 = Lectura_Hora["Car_DX2"] == DBNull.Value ? "" : Lectura_Hora["Car_DX2"].ToString(),
                                    OM_DX3 = Lectura_Hora["Car_DX3"] == DBNull.Value ? "" : Lectura_Hora["Car_DX3"].ToString(),
                                };

                                return HC;
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
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        List<CXN_CUP> ICIE10.GetLista(string Servicio)
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

                    String Cargar_Hora = "";

                    if (string.IsNullOrEmpty(Servicio))
                    {
                        Cargar_Hora = @"SELECT * 
                                        FROM CXN_CUP 
                                        ORDER BY Servicio ASC";
                    }
                    else
                    {
                        Cargar_Hora =  "SELECT * " +
                                       "FROM CXN_CUP " +
                                       "WHERE Servicio LIKE '%" + Servicio + "%'" +
                                       "ORDER BY Servicio ASC";
                    }

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_CUP> C = new List<CXN_CUP>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_CUP
                                    {
                                        CUP = Lectura_Hora["CUP"].ToString(),
                                        Servicio = Lectura_Hora["Servicio"].ToString(),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"])
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
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        string ICIE10.GetDXName(string Code)
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
                                         "FROM CXN_CUP " +
                                         "WHERE CUP = @param1";
                    

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Code);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {                               
                                return Lectura_Hora["Servicio"].ToString();
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

        //CIE 11 - OMS
        bool ICIE10.InsertaCIE11(CXN_CIE11 C)
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

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO CXN_CIE11 (Admision, " +
                                                                  "Paciente, " +
                                                                  "Codigo, " +
                                                                  "Descripcion, " +
                                                                  "Identificador, " +
                                                                  "Capitulo, " +
                                                                  "Terminal, " +
                                                                  "Puntacion) " +
                                         "values                  (@param1, " +
                                                                  "@param2, " +
                                                                  "@param3, " +
                                                                  "@param4, " +
                                                                  "@param5, " +
                                                                  "@param6, " +
                                                                  "@param7, " +
                                                                  "@param8)", con);

                    cmd.Parameters.AddWithValue("@param1", C.Admision);
                    cmd.Parameters.AddWithValue("@param2", C.Paciente);
                    cmd.Parameters.AddWithValue("@param3", C.Codigo);
                    cmd.Parameters.AddWithValue("@param4", C.Descripcion);
                    cmd.Parameters.AddWithValue("@param5", C.Identificador);
                    cmd.Parameters.AddWithValue("@param6", C.Capitulo);
                    cmd.Parameters.AddWithValue("@param7", C.Terminal);
                    cmd.Parameters.AddWithValue("@param8", C.Puntacion);
                    return cmd.ExecuteNonQuery() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool ICIE10.UpdateCIE11(CXN_CIE11 C)
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

                    string Busqueda = @"UPDATE CXN_CIE11 
                                        SET Codigo = @param1, 
                                          Descripcion = @param2, 
                                          Identificador = @param3, 
                                          Capitulo = @param4, 
                                          Terminal = @param5, 
                                          Puntacion = @param6 
                                        WHERE Admision = @param7";

                    using (SqlCommand Accion = new SqlCommand(Busqueda, con))
                    {
                        Accion.Parameters.AddWithValue("@param1", C.Codigo);
                        Accion.Parameters.AddWithValue("@param2", C.Descripcion);
                        Accion.Parameters.AddWithValue("@param3", C.Identificador);
                        Accion.Parameters.AddWithValue("@param4", C.Capitulo);
                        Accion.Parameters.AddWithValue("@param5", C.Terminal);
                        Accion.Parameters.AddWithValue("@param6", C.Puntacion);
                        Accion.Parameters.AddWithValue("@param7", C.Admision);

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
        CXN_CIE11 ICIE10.GetCIE11ByAdm(int Admision)
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
                    String Cargar_Hora = "SELECT TOP 1 * " +
                                         "FROM CXN_CIE11 " +
                                         "WHERE Admision = @param1 " +
                                         "ORDER BY IdCIE11 DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_CIE11 HC = new CXN_CIE11
                                {
                                    Admision = Convert.ToInt32(Lectura_Hora["Admision"]),
                                    Capitulo = Lectura_Hora["Capitulo"] == DBNull.Value ? "" : Lectura_Hora["Capitulo"].ToString(),
                                    Codigo = Lectura_Hora["Codigo"] == DBNull.Value ? "" : Lectura_Hora["Codigo"].ToString(),
                                    Descripcion = Lectura_Hora["Descripcion"] == DBNull.Value ? "" : Lectura_Hora["Descripcion"].ToString(),
                                    IdCIE11 = Convert.ToInt32(Lectura_Hora["IdCIE11"]),
                                    Identificador = Lectura_Hora["Identificador"] == DBNull.Value ? "" : Lectura_Hora["Identificador"].ToString(),
                                    Paciente = Convert.ToInt32(Lectura_Hora["Paciente"]),
                                    Puntacion = Lectura_Hora["Puntacion"] == DBNull.Value ? "" : Lectura_Hora["Puntacion"].ToString(),
                                    Terminal = Lectura_Hora["Terminal"] == DBNull.Value ? "" : Lectura_Hora["Terminal"].ToString()
                                };

                                return HC;
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
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
    }
}
