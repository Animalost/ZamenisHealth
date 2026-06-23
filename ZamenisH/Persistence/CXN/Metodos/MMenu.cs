using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MMenu : IMenu
    {
        List<string> IMenu.getCausaExterna()
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

                    String Query = "SELECT Causa_Externa FROM CXN_MENUHGMG";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<string> L = new List<string>();

                        while (Reader.Read() == true)
                        {
                            L.Add(Reader["Causa_Externa"].ToString());
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
        List<CXN_MENUHGMG> IMenu.getMenus()
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

                    String Query = "SELECT * FROM CXN_MENUHGMG";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_MENUHGMG> L = new List<CXN_MENUHGMG>();

                        while (Reader.Read() == true)
                        {
                            L.Add(new CXN_MENUHGMG
                            {
                                Grado = Reader["Grado"].ToString(),
                                TipoLesion = Reader["TipoLesion"].ToString(),
                                ActEjer = Reader["ActEjer"].ToString(),
                                Vez = Reader["Vez"].ToString(),
                                Apariencia = Reader["Apariencia"].ToString(),
                                Emocional = Reader["Emocional"].ToString(),
                                Nutricional = Reader["Nutricional"].ToString(),
                                Exudado = Reader["Exudado"].ToString(),
                                Tamaño = Reader["Tamaño"].ToString(),
                                ConsCant = Reader["ConsCant"].ToString(),
                                Estado = Reader["Estado"].ToString(),
                                Dolor = Reader["Dolor"].ToString(),
                                Patologia = Reader["Patologia"].ToString(),
                                R_Cancela = Reader["R_Cancela"].ToString(),
                                Tej_Com = Reader["Tej_Com"].ToString(),
                                Carac_Tej = Reader["Carac_Tej"].ToString(),
                                Sig_Inf = Reader["Sig_Inf"].ToString(),
                                Piel_Circ = Reader["Piel_Circ"].ToString(),
                                Pos_Patologia = Reader["Pos_Patologia"].ToString(),
                                Pos_Patologia_2 = Reader["Pos_Patologia_2"].ToString(),
                                Causa_Externa = Reader["Causa_Externa"].ToString(),
                                Men_Id = Convert.ToInt32(Reader["Men_Id"]),
                                Rango_Edad = Reader["Rango_Edad"].ToString(),
                                Evolucion = Reader["Evolucion"].ToString(),
                                Ingreso = Reader["Ingreso"].ToString()
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
        List<string> IMenu.getPosPatologia()
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

                    String Query = "SELECT Pos_Patologia FROM CXN_MENUHGMG";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<string> L = new List<string>();

                        while (Reader.Read() == true)
                        {
                            L.Add(Reader["Pos_Patologia"].ToString());
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
        List<string> IMenu.getPosPatologia2()
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

                    String Query = "SELECT Pos_Patologia_2 FROM CXN_MENUHGMG";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<string> L = new List<string>();

                        while (Reader.Read() == true)
                        {
                            L.Add(Reader["Pos_Patologia_2"].ToString());
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
