using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MConfSystem : IConfSystem
    {
        bool IConfSystem.updateImagenSystem(CXN_IMAGEN_SYSTEM I)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_IMAGEN_SYSTEM " +
                                                          "SET Tab_Clave = @param1 " +
                                                          "WHERE Tab_Nombre = @param2", con);

                    Busqueda.Parameters.AddWithValue("@param1", I.Tab_Clave);
                    Busqueda.Parameters.AddWithValue("@param2", I.Tab_Nombre);
                    int c = Busqueda.ExecuteNonQuery();
                    if (c > 0) 
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        List<CXN_IMAGEN_SYSTEM> IConfSystem.getConfSystem()
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

                    String Cargar_Hora = "SELECT * FROM CXN_IMAGEN_SYSTEM";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_IMAGEN_SYSTEM> L = new List<CXN_IMAGEN_SYSTEM>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new CXN_IMAGEN_SYSTEM 
                                    {
                                        Tab_Clave = Lectura_Hora["Tab_Clave"] == DBNull.Value ? "" : Lectura_Hora["Tab_Clave"].ToString(),
                                        Tab_Config = Lectura_Hora["Tab_Config"] == DBNull.Value ? "" : Lectura_Hora["Tab_Config"].ToString(),
                                        Tab_Nombre = Lectura_Hora["Tab_Nombre"] == DBNull.Value ? "" : Lectura_Hora["Tab_Nombre"].ToString(),
                                        Tab_Id = Convert.ToInt32(Lectura_Hora["Tab_Id"])
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
    }
}
