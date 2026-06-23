using Domain;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MRIPS_Res2275_2023 : IRIPS_Res2275_2023
    {               
        string IRIPS_Res2275_2023.getCodeGrupoServicios(string NombreServicios)
        {
            switch (NombreServicios)
            {
                case "Consulta externa":
                    return "01";
                case "Apoyo diagnóstico y complementación terapéutica":
                    return "02";
                case "Internación":
                    return "03"; 
                case "Quirúrgico":
                    return "04";
                case "Atención inmediata":
                    return "05";
                default:
                    return "01";
            }
        }
        List<string> IRIPS_Res2275_2023.getTecSalud()
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                           "FROM CXN_TECNOSALUD ORDER BY Tec_DX ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader leer = (Commando.ExecuteReader()))
                        {
                            if (leer.HasRows)
                            {
                                List<string> L = new List<string>();

                                while (leer.Read() == true)
                                {
                                    L.Add(leer["Tec_DX"].ToString());
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
        void IRIPS_Res2275_2023.updateTecnoSalud(int Admision, string Valor)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_HORARIO " +
                                       "SET  " +
                                       "HorTecnoSalud = @param1 " +
                                       "WHERE Hor_id = @param2", con);

                    Busqueda.Parameters.AddWithValue("@param1", Valor);
                    Busqueda.Parameters.AddWithValue("@param2", Admision);
                    Busqueda.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        string IRIPS_Res2275_2023.getCodeTecnoSalud(string NombreServicios)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                           "FROM CXN_TECNOSALUD WHERE Tec_DX = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", NombreServicios);

                        using (SqlDataReader leer = (Commando.ExecuteReader()))
                        {
                            if (leer.Read() == true)
                            {
                                return leer["Tec_Cod"].ToString();
                            }
                            else
                            {
                                return "15";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "15";
            }            
        }
        string IRIPS_Res2275_2023.getNameTecnoSalud(string Code)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Tec_DX " +
                           "FROM CXN_TECNOSALUD WHERE Tec_Cod = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Code);

                        using (SqlDataReader leer = (Commando.ExecuteReader()))
                        {
                            if (leer.Read() == true)
                            {
                                return leer["Tec_DX"].ToString();
                            }
                            else
                            {
                                return "DIAGNOSTICO";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "DIAGNOSTICO";
            }
        }
        List<string> IRIPS_Res2275_2023.getTecSaludCEXTERNA()
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                           "FROM CXN_TECNOSALUDMOTATENCION ORDER BY Motivo ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader leer = (Commando.ExecuteReader()))
                        {
                            if (leer.HasRows)
                            {
                                List<string> L = new List<string>();

                                while (leer.Read() == true)
                                {
                                    L.Add(leer["Motivo"].ToString());
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
        string IRIPS_Res2275_2023.getCodeTecnoSaludCExterna(string NombreServicios)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_TECNOSALUDMOTATENCION WHERE Motivo = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", NombreServicios);

                        using (SqlDataReader leer = (Commando.ExecuteReader()))
                        {
                            if (leer.Read() == true)
                            {
                                return leer["Codigo"].ToString();
                            }
                            else
                            {
                                return "38";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "38";
            }
        }
        string IRIPS_Res2275_2023.getNameTecnoSaludCExterna(int Code)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_TECNOSALUDMOTATENCION " +
                                   "WHERE Codigo = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Code);

                        using (SqlDataReader leer = (Commando.ExecuteReader()))
                        {
                            if (leer.Read() == true)
                            {
                                return leer["Motivo"].ToString();
                            }
                            else
                            {
                                return "ENFERMEDAD GENERAL";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "ENFERMEDAD GENERAL";
            }
        }
        bool IRIPS_Res2275_2023.insertToken(string Token, int Cia)
        {
            try
            {
                Dictionary<string, string> dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now;
                    DateTime Ven_Fecha = DateTime.Now.Date;

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_TOKENSMinSalud ( " + 
                                                          "Token, " + 
                                                          "Fecha, " + 
                                                          "Hora, " + 
                                                          "CodePrestador) " +
                                 "values                  (@param1, " + 
                                                          "@param2, " + 
                                                          "@param3, " + 
                                                          "@param4)", con); 

                    cmd.Parameters.AddWithValue("@param1", Token);
                    cmd.Parameters.AddWithValue("@param2", Convert.ToDateTime(DateTime.Now.Date));
                    cmd.Parameters.AddWithValue("@param3", Convert.ToDateTime(DateTime.Now));
                    cmd.Parameters.AddWithValue("@param4", Cia);
                    int g = cmd.ExecuteNonQuery();

                    if (g > 0)
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
        string IRIPS_Res2275_2023.getLastToken()
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Token " +
                                   "FROM CXN_TOKENSMinSalud " +
                                   "ORDER BY Id DESC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader leer = (Commando.ExecuteReader()))
                        {
                            if (leer.Read() == true)
                            {
                                return leer["Token"].ToString();
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
    }
}
