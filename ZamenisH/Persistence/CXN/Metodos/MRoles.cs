using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MRoles : IRoles
    { 
        bool updateRoles(CXN_DESKTOP_ROLES R)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_DESKTOP_ROLES " +
                                                          "SET  " +
                                                          "Recepcion = @param1, " +
                                                          "Administracion = @param2, " +
                                                          "Opciones = @param3, " +
                                                          "Enfermeria = @param4, " +
                                                          "MedicinaGeneral = @param5, " +
                                                          "Gerencial = @param6, " +
                                                          "Fisiatria = @param7 " +
                                                          "WHERE Usuario = @param8", con);

                    Busqueda.Parameters.AddWithValue("@param1", R.Recepcion);
                    Busqueda.Parameters.AddWithValue("@param2", R.Administracion);
                    Busqueda.Parameters.AddWithValue("@param3",R.Opciones);
                    Busqueda.Parameters.AddWithValue("@param4",R.Enfermeria);
                    Busqueda.Parameters.AddWithValue("@param5", R.MedicinaGeneral);
                    Busqueda.Parameters.AddWithValue("@param6", R.Gerencial);
                    Busqueda.Parameters.AddWithValue("@param7", R.Fisiatria);
                    Busqueda.Parameters.AddWithValue("@param8", R.Usuario);

                    int s = Busqueda.ExecuteNonQuery();
                    if (s > 0)
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
        bool insertRoles(CXN_DESKTOP_ROLES R)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_DESKTOP_ROLES ( " + //param1
                                                          "Recepcion, " + //param2
                                                          "Administracion, " + //param3
                                                          "Opciones, " +
                                                          "Enfermeria, " +
                                                          "MedicinaGeneral, " +
                                                          "Gerencial, " +
                                                          "Fisiatria, " +
                                                          "Usuario) " + 
                                 "values                  (@param1, " + // Hor_Estado
                                                          "@param2, " + // Hor_Pac_Id
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5, " +
                                                          "@param6, " +
                                                          "@param7, " +// Hor_Pac_Bod
                                                          "@param8)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", R.Recepcion);
                    cmd.Parameters.AddWithValue("@param2", R.Administracion);
                    cmd.Parameters.AddWithValue("@param3", R.Opciones);
                    cmd.Parameters.AddWithValue("@param4", R.Enfermeria);
                    cmd.Parameters.AddWithValue("@param5", R.MedicinaGeneral);
                    cmd.Parameters.AddWithValue("@param6", R.Gerencial);
                    cmd.Parameters.AddWithValue("@param7", R.Fisiatria);
                    cmd.Parameters.AddWithValue("@param8", R.Usuario);

                    int s = cmd.ExecuteNonQuery();
                    if (s > 0)
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
        bool IRoles.SaveRoles(CXN_DESKTOP_ROLES roles, string Usuario)
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

                    String Query = "SELECT * " +
                                   "FROM CXN_DESKTOP_ROLES " +
                                   "WHERE Usuario = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Usuario);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                //Editar
                                return updateRoles(roles);
                            }
                            else
                            {
                                //Crear
                                return insertRoles(roles);
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
        CXN_DESKTOP_ROLES IRoles.getDesktopRoles(string User)
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

                    String Query = "SELECT * " +
                                   "FROM CXN_DESKTOP_ROLES " +
                                   "WHERE Usuario = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", User);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_DESKTOP_ROLES DH = new CXN_DESKTOP_ROLES
                                {
                                    Recepcion = Reader["Recepcion"].ToString(),
                                    Administracion = Reader["Administracion"].ToString(),
                                    Opciones = Reader["Opciones"].ToString(),
                                    Usuario = Reader["Usuario"].ToString(),
                                    Id = Convert.ToInt32(Reader["Id"]),
                                    Enfermeria = Reader["Enfermeria"].ToString(),
                                    Fisiatria = Reader["Fisiatria"].ToString(),
                                    Gerencial = Reader["Gerencial"].ToString(),
                                    MedicinaGeneral = Reader["MedicinaGeneral"].ToString()
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
    }
}
