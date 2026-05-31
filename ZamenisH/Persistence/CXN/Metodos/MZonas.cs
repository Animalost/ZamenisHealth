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
                                   "WHERE Zon_Dep_Cod = '" + CodDep + "'";
                    SqlCommand Command = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Command.ExecuteReader());
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
            catch
            {
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
                                   "WHERE Zon_Dep = '" + NomDep + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
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
            catch
            {
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
                                   "WHERE Zon_Mun = '" + NomMun + "' " +
                                   "AND Zon_Dep = '" + NomDep + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
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
            catch
            {
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
                                   "WHERE Zon_Mun_Cod = '" + CodMun + "' " +
                                   "AND Zon_Dep_Cod = '" + DepCod + "'";
                    SqlCommand Command = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Command.ExecuteReader());
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
            catch
            {
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

                    SqlCommand Carga_Command2 = new SqlCommand(Query, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
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
            catch
            {
                return null;
            }
        }

    }
}
