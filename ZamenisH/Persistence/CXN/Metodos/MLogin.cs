using Persistence.CXN.Interfaces;
using System;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using System.Collections.Generic;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MLogin : ILogin
    {
        private static readonly IGenerales repositorioGenerales = new MGenerales();

        CXN_LOGIN ILogin.Loguear(string User, string Pass)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    var ClaveEncode = repositorioGenerales.GetMD5(Pass);

                    String Query = "SELECT * " + 
                                   "FROM CXN_LOGIN " +
                                   "WHERE Log_Usuario = '" + User + "' " +
                                   "AND Log_ClaveC = '" + ClaveEncode.ToString() + "'";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_LOGIN P = new CXN_LOGIN();

                                if (Reader["Log_Habilitado"].ToString() != "A")
                                {
                                    P.Log_Habilitado = "N";
                                    return P;
                                }

                                P.Log_Usuario = Reader["Log_Usuario"].ToString();
                                P.Log_ClaveC = Reader["Log_ClaveC"].ToString();
                                P.Log_PrimerN = Reader["Log_PrimerN"].ToString();
                                P.Log_SegundoN = Reader["Log_SegundoN"].ToString();
                                P.Log_PrimerA = Reader["Log_PrimerA"].ToString();
                                P.Log_SegundoA = Reader["Log_SegundoA"].ToString();
                                P.Log_Identificacion = Reader["Log_Identificacion"].ToString();
                                P.Log_Celular = Reader["Log_Celular"].ToString();
                                P.Log_Rol_Admin = Reader["Log_Rol_Admin"].ToString();
                                P.Log_Rol_Recepcion = Reader["Log_Rol_Recepcion"].ToString();
                                P.Log_Rol_Enfermero = Reader["Log_Rol_Enfermero"].ToString();
                                P.Log_Rol_AdminI = Reader["Log_Rol_AdminI"].ToString();
                                P.Log_Rol_MedGen = Reader["Log_Rol_MedGen"].ToString();
                                P.Log_Rol_Gerencial = Reader["Log_Rol_Gerencial"].ToString();
                                P.Log_Rol_Psicologia = Reader["Log_Rol_Psicologia"].ToString();
                                P.Log_Rol_TO = Reader["Log_Rol_TO"].ToString();
                                P.Log_Rol_TF = Reader["Log_Rol_TF"].ToString();
                                P.Log_Rol_FI = Reader["Log_Rol_FI"].ToString();
                                P.Log_Email = Reader["Log_Email"].ToString();
                                P.Log_Habilitado = Reader["Log_Habilitado"].ToString();
                                P.Log_Varios = Reader["Log_Varios"].ToString();
                                P.Log_UpdatePass = Convert.ToDateTime(Reader["Log_UpdatePass"]);
                                P.TyC = Reader["TyC"].ToString();
                                P.Log_RolRadiologia = Reader["Log_RolRadiologia"].ToString();
                                P.Patron = Reader["Patron"].ToString();
                                return P;
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
                Console.WriteLine("Error en MLogin.Loguear: " + ex.Message);
                return null;
            }
        }
        CXN_LOGIN ILogin.getUser(string User)
        {
            try
            {
                //PRODUCCION SIN COMENTAR
                var getCon = Conexion.Conection();
                //MEDELLIN
                //getCon["Conexion"] = "Data Source=slsoft.net,14330;Initial Catalog=cxn_medellin;User ID=sa;Password=Sharon*55284;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True;";
                //WOUND CLINIC
                //getCon["Conexion"] = "Data Source=slsoft.net,14330;Initial Catalog=CXN_WOUND_CLINIC;User ID=sa;Password=Sharon*55284;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True;";
                //QA
               // getCon["Conexion"] = "Data Source=slsoft.net,14330;Initial Catalog=CXN_PRUEBAS;User ID=sa;Password=Sharon*55284;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True;";

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_LOGIN " +
                                   "WHERE Log_Usuario = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", User);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_LOGIN P = new CXN_LOGIN
                                {
                                    Log_Usuario = Reader["Log_Usuario"].ToString(),
                                    Log_ClaveC = Reader["Log_ClaveC"].ToString(),
                                    Log_PrimerN = Reader["Log_PrimerN"].ToString(),
                                    Log_SegundoN = Reader["Log_SegundoN"].ToString(),
                                    Log_PrimerA = Reader["Log_PrimerA"].ToString(),
                                    Log_SegundoA = Reader["Log_SegundoA"].ToString(),
                                    Log_Identificacion = Reader["Log_Identificacion"].ToString(),
                                    Log_Celular = Reader["Log_Celular"].ToString(),
                                    Log_Rol_Admin = Reader["Log_Rol_Admin"].ToString(),
                                    Log_Rol_Recepcion = Reader["Log_Rol_Recepcion"].ToString(),
                                    Log_Rol_Enfermero = Reader["Log_Rol_Enfermero"].ToString(),
                                    Log_Rol_AdminI = Reader["Log_Rol_AdminI"].ToString(),
                                    Log_Rol_MedGen = Reader["Log_Rol_MedGen"].ToString(),
                                    Log_Rol_Gerencial = Reader["Log_Rol_Gerencial"].ToString(),
                                    Log_Rol_Psicologia = Reader["Log_Rol_Psicologia"].ToString(),
                                    Log_Rol_TO = Reader["Log_Rol_TO"].ToString(),
                                    Log_Rol_TF = Reader["Log_Rol_TF"].ToString(),
                                    Log_Rol_FI = Reader["Log_Rol_FI"].ToString(),
                                    Log_Email = Reader["Log_Email"].ToString(),
                                    Log_Habilitado = Reader["Log_Habilitado"].ToString(),
                                    Log_Varios = Reader["Log_Varios"].ToString(), //Enfermero Jefe
                                    Log_Fotos = Reader["Log_Fotos"].ToString(),
                                    Log_NotasAcla = Reader["Log_NotasAcla"].ToString(),
                                    Log_Image = (Reader["Log_Image"] != DBNull.Value ? (byte[])Reader["Log_Image"] : null),
                                    Log_RolRadiologia = Reader["Log_Rol_Gerencial"].ToString(),
                                    TyC = Reader["TyC"].ToString(),
                                    Patron = Reader["Patron"].ToString(),
                                    Log_UpdatePass = Convert.ToDateTime(Reader["Log_UpdatePass"])
                                };

                                return P;
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
        string ILogin.getClave(string User)
        {
            try
            {
                var getCon = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT Log_ClaveC " +
                                   "FROM CXN_LOGIN " +
                                   "WHERE Log_Usuario = '" + User + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return Reader["Log_ClaveC"].ToString();
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
        bool ILogin.changeClave(string User, string Clave)
        {
            try
            {
                DateTime Hoy = DateTime.Now.Date;
                Hoy = Hoy.AddDays(90);

                var getCon = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_LOGIN " +
                                      "SET Log_ClaveC = @param1, " +
                                      "Log_UpdatePass = @param2 " +
                                      "WHERE Log_Usuario = @param3";
                    
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@param1", Clave);
                    Accion.Parameters.AddWithValue("@param2", Convert.ToDateTime(Hoy).ToString(Conexion.ConectionDictionary["Format_Fecha"]));
                    Accion.Parameters.AddWithValue("@param3", User);

                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();                   
                    return Guarda > 0  ? true : false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        bool ILogin.updatePatron(string User, string Patron)
        {
            try
            {
                var getCon = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_LOGIN " +
                                                         "SET  " +
                                                         "Patron = @param1 " +
                                                         "WHERE Log_Usuario = @param2", con);

                    Busqueda.Parameters.AddWithValue("@param1", Patron.Trim());
                    Busqueda.Parameters.AddWithValue("@param2", User.ToUpper().Trim());
                    int P = Busqueda.ExecuteNonQuery();
                    if (P > 0) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        bool ILogin.ValidarTipoUsuarioEspecialidad(string User, string Requiere)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_LOGIN " +
                                   "WHERE Log_Usuario = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", User);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                if (Requiere == "TO")
                                {
                                    if (Reader["Log_Rol_TO"].ToString() == "A")
                                    {
                                        return true;
                                    }
                                }
                                else if (Requiere == "PS")
                                {
                                    if (Reader["Log_Rol_Psicologia"].ToString() == "A")
                                    {
                                        return true;
                                    }
                                }
                                else if (Requiere == "FI")
                                {
                                    if (Reader["Log_Rol_FI"].ToString() == "A")
                                    {
                                        return true;
                                    }
                                }
                                else if (Requiere == "TF")
                                {
                                    if (Reader["Log_Rol_TF"].ToString() == "A")
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    return false;
                                }

                                return false;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool ILogin.ActualizarFuncionario(CXN_LOGIN L)
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
                    string Busqueda = "UPDATE CXN_LOGIN " +
                                      "SET Log_PrimerN = '" + L.Log_PrimerN + "', " +
                                      "Log_SegundoN = '" + L.Log_SegundoN + "', " +
                                      "Log_PrimerA = '" + L.Log_PrimerA + "', " +
                                      "Log_SegundoA = '" + L.Log_SegundoA + "', " +
                                      "Log_Email = '" + L.Log_Email + "', " +
                                      "Log_Habilitado = '" + L.Log_Habilitado + "', " +
                                      "Log_Rol_Admin = '" + L.Log_Rol_Admin + "', " +
                                      "Log_Rol_Recepcion = '" + L.Log_Rol_Recepcion + "', " +
                                      "Log_Rol_Enfermero = '" + L.Log_Rol_Enfermero + "', " +
                                      "Log_Rol_AdminI = '" + L.Log_Rol_AdminI + "', " +
                                      "Log_Rol_MedGen = '" + L.Log_Rol_MedGen + "', " +
                                      "Log_Rol_Gerencial = '" + L.Log_Rol_Gerencial + "', " +
                                      "Log_Rol_Psicologia = '" + L.Log_Rol_Psicologia + "', " +
                                      "Log_Varios = '" + L.Log_Varios + "', " +
                                      "Log_Rol_TO = '" + L.Log_Rol_TO + "', " +
                                      "Log_Rol_TF = '" + L.Log_Rol_TF + "', " +
                                      "Log_Rol_FI = '" + L.Log_Rol_FI + "', " +
                                      "Log_Fotos = '" + L.Log_Fotos + "', " +
                                      "Log_NotasAcla = '" + L.Log_NotasAcla + "', " +
                                      "Log_Celular = '" + L.Log_Celular + "', " +
                                      "Log_UsuarioGraba = '" + L.Log_UsuarioGraba + "', " +
                                      "Log_RolRadiologia = '" + L.Log_RolRadiologia + "' " +
                                      "WHERE Log_Usuario = '" + L.Log_Usuario + "'";
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
        bool ILogin.Graba_Funcionario(CXN_LOGIN L)
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

                    DateTime fechaFinal = DateTime.Now.Date.AddDays(90);

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_LOGIN (Log_PrimerN, " +
                                                          "Log_SegundoN, " +
                                                          "Log_PrimerA, " +
                                                          "Log_SegundoA, " +
                                                          "Log_Email, " +
                                                          "Log_Habilitado, " +
                                                          "Log_Rol_Admin, " +
                                                          "Log_Rol_Recepcion, " +
                                                          "Log_Rol_Enfermero, " +
                                                          "Log_Rol_AdminI, " +
                                                          "Log_Rol_MedGen, " +
                                                          "Log_Rol_Gerencial, " +
                                                          "Log_Rol_Psicologia, " +
                                                          "Log_Varios, " +
                                                          "Log_Rol_TO, " +
                                                          "Log_Rol_TF, " +
                                                          "Log_Rol_FI, " +
                                                          "Log_UsuarioGraba, " +
                                                          "Log_Identificacion, " +
                                                          "Log_Usuario, " +
                                                          "Log_Celular, " +
                                                          "Log_Clave, " +
                                                          "Log_ClaveC, " +
                                                          "Log_Fotos, " +
                                                          "Log_NotasAcla, " +
                                                          "TyC, " +
                                                          "Log_UpdatePass, " +
                                                          "Log_RolRadiologia) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5, " +
                                                          "@param6, " +
                                                          "@param7, " +
                                                          "@param8, " +
                                                          "@param9, " +
                                                          "@param10, " +
                                                          "@param11, " +
                                                          "@param12, " +
                                                          "@param13, " +
                                                          "@param14, " +
                                                          "@param15, " +
                                                          "@param16, " +
                                                          "@param17, " +
                                                          "@param18, " +
                                                          "@param19, " +
                                                          "@param20, " +
                                                          "@param21, " +
                                                          "@param22, " +
                                                          "@param23, " +
                                                          "@param24, " +
                                                          "@param25, " +
                                                          "@param26, " +
                                                          "@param27, " +
                                                          "@param28)", con);

                    cmd.Parameters.AddWithValue("@param1", L.Log_PrimerN);
                    cmd.Parameters.AddWithValue("@param2", L.Log_SegundoN);
                    cmd.Parameters.AddWithValue("@param3", L.Log_PrimerA);
                    cmd.Parameters.AddWithValue("@param4", L.Log_SegundoA);
                    cmd.Parameters.AddWithValue("@param5", L.Log_Email);
                    cmd.Parameters.AddWithValue("@param6", L.Log_Habilitado);
                    cmd.Parameters.AddWithValue("@param7", L.Log_Rol_Admin);
                    cmd.Parameters.AddWithValue("@param8", L.Log_Rol_Recepcion);
                    cmd.Parameters.AddWithValue("@param9", L.Log_Rol_Enfermero);
                    cmd.Parameters.AddWithValue("@param10", L.Log_Rol_AdminI);
                    cmd.Parameters.AddWithValue("@param11", L.Log_Rol_MedGen);
                    cmd.Parameters.AddWithValue("@param12", L.Log_Rol_Gerencial);
                    cmd.Parameters.AddWithValue("@param13", L.Log_Rol_Psicologia);
                    cmd.Parameters.AddWithValue("@param14", L.Log_Varios);
                    cmd.Parameters.AddWithValue("@param15", L.Log_Rol_TO);
                    cmd.Parameters.AddWithValue("@param16", L.Log_Rol_TF);
                    cmd.Parameters.AddWithValue("@param17", L.Log_Rol_FI);
                    cmd.Parameters.AddWithValue("@param18", L.Log_UsuarioGraba);
                    cmd.Parameters.AddWithValue("@param19", L.Log_Identificacion);
                    cmd.Parameters.AddWithValue("@param20", L.Log_Usuario);
                    cmd.Parameters.AddWithValue("@param21", L.Log_Celular);
                    cmd.Parameters.AddWithValue("@param22", L.Log_ClaveC);
                    cmd.Parameters.AddWithValue("@param23", L.Log_ClaveC);
                    cmd.Parameters.AddWithValue("@param24", L.Log_Fotos);
                    cmd.Parameters.AddWithValue("@param25", L.Log_NotasAcla);
                    cmd.Parameters.AddWithValue("@param26", L.TyC);
                    cmd.Parameters.Add(new SqlParameter("@param27", SqlDbType.DateTime)).Value = fechaFinal;
                    cmd.Parameters.AddWithValue("@param28", L.Log_RolRadiologia);
                    int c = cmd.ExecuteNonQuery();
                    if (c > 0) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool ILogin.resetClave(string User)
        {
            try
            {
                var getCon = Conexion.Conection();

                DateTime Hoy = DateTime.Now.Date;

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    string Busqueda = "UPDATE CXN_LOGIN " +
                                         "SET Log_ClaveC = '202cb962ac59075b964b07152d234b70', " +
                                         "Log_Clave = '202cb962ac59075b964b07152d234b70', " +
                                         "Log_UpdatePass = '" + Convert.ToDateTime(Hoy).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                         "WHERE Log_Usuario = '" + User + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        bool ILogin.AcceptTyC(string User)
        {
            try
            {
                var getCon = Conexion.Conection();

                DateTime Hoy = DateTime.Now.Date;

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    string Busqueda = "UPDATE CXN_LOGIN " +
                                      "SET TyC = 'A' " +
                                      "WHERE Log_Usuario = '" + User + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        List<CXN_LOGIN> ILogin.getUsersforSendMessage()
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

                    String Query = "SELECT Log_PrimerA, Log_SegundoA, Log_PrimerN, Log_SegundoN, Log_Usuario, Log_Habilitado " +
                                   "FROM CXN_LOGIN " +
                                   "ORDER BY Log_PrimerA ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_LOGIN> Lista = new List<CXN_LOGIN>();

                                while (Reader.Read() == true)
                                {
                                    Lista.Add(new CXN_LOGIN 
                                    {
                                        Log_PrimerA = Reader["Log_PrimerA"].ToString(),
                                        Log_SegundoA = Reader["Log_SegundoA"].ToString(),
                                        Log_PrimerN = Reader["Log_PrimerN"].ToString(),
                                        Log_SegundoN = Reader["Log_SegundoN"].ToString(),
                                        Log_Usuario = Reader["Log_Usuario"].ToString(),
                                        Log_Habilitado = Reader["Log_Habilitado"].ToString()
                                    });    
                                }

                                return Lista;
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
        void ILogin.saveAvatar(byte[] _avatar, string user)
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

                    string Busqueda = "UPDATE CXN_LOGIN SET Log_Image = @Avatar WHERE Log_Usuario = @User";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@Avatar", _avatar);
                    Accion.Parameters.AddWithValue("@User", user);
                    int Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        byte[] ILogin.getAvatar(string User)
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
                    String Query = "SELECT Log_Image " +
                                   "FROM CXN_LOGIN " +
                                   "WHERE Log_Usuario = '" + User + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        
                        return (Reader["Log_Image"] != DBNull.Value ? (byte[])Reader["Log_Image"] : null);
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
        int ILogin.ActualizarMisDatos(CXN_LOGIN L)
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
                    string Busqueda = "UPDATE CXN_LOGIN " +
                                      "SET Log_Identificacion = '" + L.Log_Identificacion + "', " +
                                      "Log_Email = '" + L.Log_Email + "', " +
                                      "Log_Celular = '" + L.Log_Celular + "' " +
                                      "WHERE Log_Usuario = '" + L.Log_Usuario + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return Guarda;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }
        CXN_LOGIN ILogin.getDatosCode(int Code)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_BODEGAS " +
                                   "INNER JOIN CXN_LOGIN ON CXN_BODEGAS.Bod_Usuario = CXN_LOGIN.Log_Usuario " +
                                   "WHERE Bod_Numero = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Code);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_LOGIN P = new CXN_LOGIN
                                {
                                    Log_Usuario = Reader["Log_Usuario"].ToString(),
                                    Log_ClaveC = Reader["Log_ClaveC"].ToString(),
                                    Log_PrimerN = Reader["Log_PrimerN"].ToString(),
                                    Log_SegundoN = Reader["Log_SegundoN"].ToString(),
                                    Log_PrimerA = Reader["Log_PrimerA"].ToString(),
                                    Log_SegundoA = Reader["Log_SegundoA"].ToString(),
                                    Log_Identificacion = Reader["Log_Identificacion"].ToString(),
                                    Log_Celular = Reader["Log_Celular"].ToString(),
                                    Log_Rol_Admin = Reader["Log_Rol_Admin"].ToString(),
                                    Log_Rol_Recepcion = Reader["Log_Rol_Recepcion"].ToString(),
                                    Log_Rol_Enfermero = Reader["Log_Rol_Enfermero"].ToString(),
                                    Log_Rol_AdminI = Reader["Log_Rol_AdminI"].ToString(),
                                    Log_Rol_MedGen = Reader["Log_Rol_MedGen"].ToString(),
                                    Log_Rol_Gerencial = Reader["Log_Rol_Gerencial"].ToString(),
                                    Log_Rol_Psicologia = Reader["Log_Rol_Psicologia"].ToString(),
                                    Log_Rol_TO = Reader["Log_Rol_TO"].ToString(),
                                    Log_Rol_TF = Reader["Log_Rol_TF"].ToString(),
                                    Log_Rol_FI = Reader["Log_Rol_FI"].ToString(),
                                    Log_Email = Reader["Log_Email"].ToString(),
                                    Log_Habilitado = Reader["Log_Habilitado"].ToString(),
                                    Log_Varios = Reader["Log_Varios"].ToString(), //Enfermero Jefe
                                    Log_Fotos = Reader["Log_Fotos"].ToString(),
                                    Log_NotasAcla = Reader["Log_NotasAcla"].ToString(),
                                    Log_Image = (Reader["Log_Image"] != DBNull.Value ? (byte[])Reader["Log_Image"] : null),
                                    Log_RolRadiologia = Reader["Log_Rol_Gerencial"].ToString(),
                                    TyC = Reader["TyC"].ToString(),
                                    Patron = Reader["Patron"].ToString()
                                };

                                return P;
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
    }
}
