using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;

namespace Persistence.CXN.Metodos
{
    public class MZonas : IZonas
    {
        string IZonas.DepartamentoNombre(string CodDep)
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

                    String Query = "SELECT Zon_Dep " +
                                   "FROM CXN_ZONAS " +
                                   "WHERE Zon_Dep_Cod = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", CodDep);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Zon_Dep"].ToString();
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
        string IZonas.DepartamentoCodigo(string NomDep)
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
                    String Query = "SELECT Zon_Dep_Cod " +
                                   "FROM CXN_ZONAS " +
                                   "WHERE Zon_Dep = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", NomDep);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Zon_Dep_Cod"].ToString();
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
        string IZonas.MunicipioCodigo(string NomMun, string NomDep)
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
                    String Query = "SELECT Zon_Mun_Cod " +
                                   "FROM CXN_ZONAS " +
                                   "WHERE Zon_Mun = @param1 " +
                                   "AND Zon_Dep = @param2";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", NomMun);
                        Commando.Parameters.AddWithValue("@param2", NomDep);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Zon_Mun_Cod"].ToString();
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
        string IZonas.MunicipioNombre(string CodMun, string DepCod)
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

                    String Query = "SELECT Zon_Mun " +
                                   "FROM CXN_ZONAS " +
                                   "WHERE Zon_Mun_Cod = @param1 " +
                                   "AND Zon_Dep_Cod = @param2";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", CodMun);
                        Command.Parameters.AddWithValue("@param2", DepCod);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Zon_Mun"].ToString();
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
        List<CXN_ZONAS> IZonas._listadoCodigos(string DatoDep, string DatoMun, string Filtro)
        {
            try
            {
                var datConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datConect["Conexion"]))
                {
                    con.Open();

                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Query = "";

                    if (Filtro == "Departamento")
                    {
                        Query = "SELECT DISTINCT Zon_Dep_Cod as CODIGO, Zon_Dep as NOMBRE FROM CXN_ZONAS WHERE Zon_Dep Like '%" + DatoDep + "%'";
                    }

                    if (Filtro == "Municipio")
                    {
                        Query = "SELECT Zon_Mun_Cod as CODIGO, Zon_Mun as NOMBRE FROM CXN_ZONAS WHERE Zon_Mun Like '%" + DatoMun + "%' AND Zon_Dep_Cod = '" + DatoDep + "'";
                    }

                    using (SqlCommand Carga_Command2 = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.HasRows)
                            {
                                List<CXN_ZONAS> L = new List<CXN_ZONAS>();

                                while (Lectura_Hora2.Read() == true)
                                {
                                    L.Add(new CXN_ZONAS
                                    {
                                        Zon_Dep_Cod = Lectura_Hora2["CODIGO"].ToString(), //GENERAL
                                        Zon_Mun = Lectura_Hora2["NOMBRE"].ToString() //GENERAL
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
