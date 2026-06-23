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
        Dictionary<string, string> IConfSystem.getListado()
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

                    String Cargar_Hora = "Select * from CXN_IMAGEN_SYSTEM";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                Dictionary<string, string> D = new Dictionary<string, string>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    if (Lectura_Hora["Tab_Nombre"].ToString() == "INFECCIONES")
                                    {
                                        D.Add(Lectura_Hora["Tab_Nombre"].ToString(), 
                                              Lectura_Hora["Tab_Config"].ToString() == "A" ? 
                                              Lectura_Hora["Tab_Clave"].ToString() : 
                                              "X");
                                        continue;
                                    }
                                    if (Lectura_Hora["Tab_Nombre"].ToString() == "EVENTOSADVERSOS")
                                    {
                                        D.Add(Lectura_Hora["Tab_Nombre"].ToString(), 
                                              Lectura_Hora["Tab_Config"].ToString() == "A" ? 
                                              Lectura_Hora["Tab_Clave"].ToString() : 
                                              "X");
                                        continue;
                                    }                                    

                                    //debe ser en este orden
                                    if (Lectura_Hora["Tab_Nombre"].ToString() == "RellenarVaciosMG" && Lectura_Hora["Tab_Clave"].ToString() == "A")
                                    {
                                        D.Add(Lectura_Hora["Tab_Nombre"].ToString() + "1", Lectura_Hora["Tab_Config"].ToString());
                                    }

                                    D.Add(Lectura_Hora["Tab_Nombre"].ToString(), Lectura_Hora["Tab_Clave"].ToString()); //aqui agrego los originales

                                    if (Lectura_Hora["Tab_Nombre"].ToString() == "Videoconferencia" && Lectura_Hora["Tab_Clave"].ToString() == "A")
                                    {
                                        D.Add(Lectura_Hora["Tab_Nombre"].ToString() + "1", Lectura_Hora["Tab_Config"].ToString());
                                    }

                                    if (Lectura_Hora["Tab_Nombre"].ToString() == "VideoconferenciaTwilio" && Lectura_Hora["Tab_Clave"].ToString() == "A")
                                    {
                                        D.Add(Lectura_Hora["Tab_Nombre"].ToString() + "1", Lectura_Hora["Tab_Config"].ToString());
                                    }

                                    if (Lectura_Hora["Tab_Nombre"].ToString() == "ConsentimientosWEB" && Lectura_Hora["Tab_Clave"].ToString() == "A")
                                    {
                                        D.Add(Lectura_Hora["Tab_Nombre"].ToString() + "1", Lectura_Hora["Tab_Config"].ToString());
                                    }

                                    if (Lectura_Hora["Tab_Nombre"].ToString() == "ConsentimientosWEBPSI" && Lectura_Hora["Tab_Clave"].ToString() == "A")
                                    {
                                        D.Add(Lectura_Hora["Tab_Nombre"].ToString() + "1", Lectura_Hora["Tab_Config"].ToString());
                                    }

                                    if (Lectura_Hora["Tab_Nombre"].ToString() == "URLIHCEVisor")
                                    {
                                        D.Add(Lectura_Hora["Tab_Nombre"].ToString() + "1", Lectura_Hora["Tab_Config"].ToString());
                                    }
                                }

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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        string IConfSystem.getURLConsentimientos(string TipoCon)
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

                    String Cargar_Hora  = "Select Tab_Config FROM CXN_IMAGEN_SYSTEM WHERE Tab_Nombre = @param1";                    

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", TipoCon);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Lectura_Hora["Tab_Config"].ToString();
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "";
            }
        }
        (bool Noticia, string Ruta) IConfSystem.getDatoNoticias()
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

                    String Cargar_Hora = "Select Tab_Clave, Tab_Config from CXN_IMAGEN_SYSTEM WHERE Tab_Nombre = 'Noticia'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        string s = Lectura_Hora["Tab_Config"].ToString();
                        return ((Lectura_Hora["Tab_Clave"].ToString() == "A" ? true : false), Lectura_Hora["Tab_Config"].ToString());
                    }
                    else
                    {
                        return (false, "");
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return (false, "");
            }
        }
    }
}
