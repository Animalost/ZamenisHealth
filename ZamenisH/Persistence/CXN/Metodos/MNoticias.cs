using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MNoticias : INoticias
    {
        List<CXN_NOTICIAS> INoticias.GetAllNoticias()
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
                                         "FROM CXN_NOTICIAS " +
                                         "ORDER BY Fecha DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    { 
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_NOTICIAS> L = new List<CXN_NOTICIAS>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new CXN_NOTICIAS
                                    {
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        Estado = (bool)Lectura_Hora["Estado"],
                                        Resumen = Lectura_Hora["Resumen"].ToString(),
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                        URL = Lectura_Hora["URL"].ToString(),
                                        Titulo = Lectura_Hora["Titulo"].ToString()
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
        bool INoticias.UpdateEstado(int C, bool Estado)
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

                    string Busqueda = "UPDATE CXN_NOTICIAS " +
                                     "SET Estado = @param2 " +
                                     "WHERE Id = @param1";

                    using (SqlCommand Accion = new SqlCommand(Busqueda, con))
                    {
                        Accion.Parameters.AddWithValue("@param1", C);
                        Accion.Parameters.AddWithValue("@param2", Estado);

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
        bool INoticias.CreaNoticia(CXN_NOTICIAS C)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_NOTICIAS (URL, " +
                                                                  "Fecha, " +
                                                                  "Estado, " +
                                                                  "Resumen, " +
                                                                  "Titulo) " +
                                         "values                  (@param1, " +
                                                                  "@param2, " +
                                                                  "@param3, " +
                                                                  "@param4, " +
                                                                  "@param5)", con);

                    cmd.Parameters.AddWithValue("@param1", C.URL);
                    cmd.Parameters.AddWithValue("@param2", C.Fecha);
                    cmd.Parameters.AddWithValue("@param3", C.Estado);
                    cmd.Parameters.AddWithValue("@param4", C.Resumen);
                    cmd.Parameters.AddWithValue("@param5", C.Titulo);
                    return cmd.ExecuteNonQuery() > 0 ? true : false;
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
