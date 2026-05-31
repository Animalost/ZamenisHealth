using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MAseguradoras : IAseguradoras
    {
        CXN_ASEGURADORA IAseguradoras.getInfoFromAsebyCode(int Code)
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
                                   "FROM CXN_ASEGURADORA " +
                                   "WHERE Ase_Identificador = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Code);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_ASEGURADORA DH = new CXN_ASEGURADORA
                                {
                                    Ase_Id = Convert.ToInt32(Reader["Ase_Id"]),
                                    Ase_NitCia = Reader["Ase_NitCia"].ToString(),
                                    Ase_Responable = Reader["Ase_Responable"].ToString(),
                                    Ase_Identificador = Convert.ToInt32(Reader["Ase_Identificador"]),
                                    Ase_Telefono = Reader["Ase_Telefono"].ToString(),
                                    Ase_UsuarioGraba = Reader["Ase_UsuarioGraba"].ToString(),
                                    Ase_Cod_Emp = Reader["Ase_Cod_Emp"].ToString(),
                                    Ase_Cod_Prest = Reader["Ase_Cod_Prest"].ToString(),
                                    Ase_Descripcion = Reader["Ase_Descripcion"].ToString(),
                                    Ase_Direccion = Reader["Ase_Direccion"].ToString(),
                                    Ase_DVNitCia = Reader["Ase_DVNitCia"].ToString(),
                                    Ase_Email = Reader["Ase_Email"].ToString()
                                };

                                return DH;
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

        CXN_ASEGURADORA IAseguradoras.getInfoFromAsebyName(string Name)
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
                                   "FROM CXN_ASEGURADORA " +
                                   "WHERE Ase_Descripcion = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Name);
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_ASEGURADORA DH = new CXN_ASEGURADORA
                                {
                                    Ase_Id = Convert.ToInt32(Reader["Ase_Id"]),
                                    Ase_NitCia = Reader["Ase_NitCia"].ToString(),
                                    Ase_Responable = Reader["Ase_Responable"].ToString(),
                                    Ase_Identificador = Convert.ToInt32(Reader["Ase_Identificador"]),
                                    Ase_Telefono = Reader["Ase_Telefono"].ToString(),
                                    Ase_UsuarioGraba = Reader["Ase_UsuarioGraba"].ToString(),
                                    Ase_Cod_Emp = Reader["Ase_Cod_Emp"].ToString(),
                                    Ase_Cod_Prest = Reader["Ase_Cod_Prest"].ToString(),
                                    Ase_Descripcion = Reader["Ase_Descripcion"].ToString(),
                                    Ase_Direccion = Reader["Ase_Direccion"].ToString(),
                                    Ase_DVNitCia = Reader["Ase_DVNitCia"].ToString(),
                                    Ase_Email = Reader["Ase_Email"].ToString()
                                };

                                return DH;
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

        List<CXN_ASEGURADORA> IAseguradoras.getAseguradoras()
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
                                   "FROM CXN_ASEGURADORA " +
                                   "ORDER BY Ase_Descripcion ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_ASEGURADORA> A = new List<CXN_ASEGURADORA>();

                                while (Reader.Read() == true)
                                {
                                    A.Add(new CXN_ASEGURADORA
                                    {
                                        Ase_Descripcion = Reader["Ase_Descripcion"].ToString(),
                                        Ase_Identificador = Convert.ToInt32(Reader["Ase_Identificador"])
                                    });
                                }

                                return A;
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

        List<string> IAseguradoras.CargarAseguradorasXServ(string TipoServ)
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

                    String Query = "SELECT Ase_Descripcion " +
                                   "FROM CXN_ASEGURADORA " +
                                   "INNER JOIN CXN_CONVENIOS ON CXN_ASEGURADORA.Ase_Identificador = CXN_CONVENIOS.Con_Aseguradora " +
                                   "WHERE Con_Tipo_Serv = '" + TipoServ + "' " +
                                   "ORDER BY Ase_Descripcion ASC";
                    SqlCommand Command = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Command.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<string> L = new List<string>();

                        while (Reader.Read() == true)
                        {
                            L.Add(Reader["Ase_Descripcion"].ToString());
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

        bool IAseguradoras.updateAse(CXN_ASEGURADORA A)
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

                    string Busqueda = "UPDATE CXN_ASEGURADORA " +
                                     "SET Ase_Descripcion = '" + A.Ase_Descripcion + "', " +
                                     "Ase_UsuarioGraba = '" + A.Ase_UsuarioGraba + "', " +
                                     "Ase_NitCia = '" + A.Ase_NitCia + "', " +
                                     "Ase_Cod_Prest = '" + A.Ase_Cod_Prest + "', " +
                                     "Ase_Cod_Emp = '" + A.Ase_Cod_Emp + "', " +
                                     "Ase_Direccion = '" + A.Ase_Direccion + "', " +
                                     "Ase_Responable = '" + A.Ase_Responable + "', " +
                                     "Ase_Telefono = '" + A.Ase_Telefono + "', " +
                                     "Ase_DVNitCia = '" + A.Ase_DVNitCia + "', " +
                                     "Ase_Email = '" + A.Ase_Email + "' " +
                                     "WHERE Ase_Identificador = '" + A.Ase_Identificador + "'";
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

        bool IAseguradoras.createAse(CXN_ASEGURADORA A)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_ASEGURADORA (Ase_Identificador, " + //param1
                                                          "Ase_Descripcion, " + //param2
                                                          "Ase_UsuarioGraba, " + //param3
                                                          "Ase_NitCia, " + //param4
                                                          "Ase_Cod_Prest, " + //param5
                                                          "Ase_Cod_Emp, " +
                                                          "Ase_Direccion, " +
                                                          "Ase_Responable, " +
                                                          "Ase_Telefono, " +
                                                          "Ase_DVNitCia, " +
                                                          "Ase_Email) " + //param16
                                 "values                  (@param1, " + // Hor_Estado
                                                          "@param2, " + // Hor_Pac_Id
                                                          "@param3, " + // Hor_Pac_Bod
                                                          "@param4, " + // Hor_Pac_Tipo_Serv
                                                          "@param5, " + // Hor_Pac_Cia
                                                          "@param6, " + // Hor_Pac_Cia
                                                          "@param7, " + // Hor_Pac_Cia
                                                          "@param8, " + // Hor_Pac_Cia
                                                          "@param9, " + // Hor_Pac_Cia
                                                          "@param10, " +
                                                          "@param11)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", A.Ase_Identificador);
                    cmd.Parameters.AddWithValue("@param2", A.Ase_Descripcion);
                    cmd.Parameters.AddWithValue("@param3", A.Ase_UsuarioGraba);
                    cmd.Parameters.AddWithValue("@param4", A.Ase_NitCia);
                    cmd.Parameters.AddWithValue("@param5", A.Ase_Cod_Prest);
                    cmd.Parameters.AddWithValue("@param6", A.Ase_Cod_Emp);
                    cmd.Parameters.AddWithValue("@param7", A.Ase_Direccion);
                    cmd.Parameters.AddWithValue("@param8", A.Ase_Responable);
                    cmd.Parameters.AddWithValue("@param9", A.Ase_Telefono);
                    cmd.Parameters.AddWithValue("@param10", A.Ase_DVNitCia);
                    cmd.Parameters.AddWithValue("@param11", A.Ase_Email);
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
    }
}
