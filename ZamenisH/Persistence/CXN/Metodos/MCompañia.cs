using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Persistence.CXN.Interfaces;
using Domain.CXN;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MCompañia : ICompañia
    {
        CXN_CIA ICompañia.getPrestadorbyCode(int Code)
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
                                   "FROM CXN_CIA " +
                                   "WHERE Com_Identificador = @Com_Identificador";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@Com_Identificador", Code);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_CIA L = new CXN_CIA
                                {
                                    Com_Identificacion = Reader["Com_Identificacion"].ToString(),
                                    Com_Cod_Prestador = Reader["Com_Cod_Prestador"].ToString(),
                                    Com_Cod_Prestador_2 = Reader["Com_Cod_Prestador_2"].ToString(),
                                    Com_Cotiza = Convert.ToInt32(Reader["Com_Cotiza"]),
                                    Com_DE = Convert.ToInt32(Reader["Com_DE"]),
                                    Com_Direccion = Reader["Com_Direccion"].ToString(),
                                    Com_Email = Reader["Com_Email"].ToString(),
                                    Com_Fac = Convert.ToInt32(Reader["Com_Fac"]),
                                    Com_Id = Reader["Com_Id"].ToString(),
                                    Com_Identificador = Convert.ToInt32(Reader["Com_Identificador"]),
                                    Com_Logo = Reader["Com_Logo"].ToString(),
                                    Com_Nombre = Reader["Com_Nombre"].ToString(),
                                    Com_Nombre_SMS = Reader["Com_Nombre_SMS"].ToString(),
                                    Com_OM = Convert.ToInt32(Reader["Com_OM"]),
                                    Com_OP = Convert.ToInt32(Reader["Com_OP"]),
                                    Com_PedPro = Convert.ToInt32(Reader["Com_PedPro"]),
                                    Com_Resolucion = Reader["Com_Resolucion"].ToString(),
                                    Com_RIP = Convert.ToInt32(Reader["Com_RIP"]),
                                    Com_Telefono = Reader["Com_Telefono"].ToString(),
                                    Com_Telefono_SMS = Reader["Com_Telefono_SMS"].ToString(),
                                    Com_Tipo_Doc = Reader["Com_Tipo_Doc"].ToString(),
                                    Com_UsuarioGraba = Reader["Com_UsuarioGraba"].ToString(),
                                    Com_SMS = Convert.ToInt32(Reader["Com_SMS"]),
                                    Com_Resolucion_Electron = Reader["Com_Resolucion_Electron"].ToString(),
                                    Com_Doc_Electron = Convert.ToInt32(Reader["Com_Doc_Electron"]),
                                    Com_Prefijo_Electron = Reader["Com_Prefijo_Electron"].ToString(),
                                    Com_Fecha_Electron = Convert.ToDateTime(Reader["Com_Fecha_Electron"]),
                                    Com_Numeracion_Electron = Reader["Com_Numeracion_Electron"].ToString(),
                                    Com_Doc_Electron_NC = Convert.ToInt32(Reader["Com_Doc_Electron_NC"]),
                                    Com_Prefijo_Electron_NC = Reader["Com_Prefijo_Electron_NC"].ToString(),
                                    Com_ConsContable = Convert.ToInt32(Reader["Com_ConsContable"]),
                                    Com_DVerifica = Reader["Com_DVerifica"].ToString(),

                                    Com_Doc_Soporte = Convert.ToInt32(Reader["Com_Doc_Soporte"]),
                                    Com_Doc_Soporte_NC = Convert.ToInt32(Reader["Com_Doc_Soporte_NC"]),
                                    Com_Fecha_Soporte = Convert.ToDateTime(Reader["Com_Fecha_Soporte"]),
                                    Com_Numeracion_Soporte = Reader["Com_Numeracion_Soporte"].ToString(),
                                    Com_Prefijo_Soporte = Reader["Com_Prefijo_Soporte"].ToString(),
                                    Com_Prefijo_Soporte_NC = Reader["Com_Prefijo_Soporte"].ToString(),
                                    Com_Resolucion_Soporte = Reader["Com_Resolucion_Soporte"].ToString(),
                                    Diferenciador = Reader["Diferenciador"].ToString(),
                                    Com_Cierres = Convert.ToInt32(Reader["Com_Cierres"]),
                                };

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
        CXN_CIA ICompañia.getPrestadorbyName(string Name)
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
                                   "FROM CXN_CIA " +
                                   "WHERE Com_Nombre = @Com_Nombre";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@Com_Nombre", Name);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_CIA L = new CXN_CIA
                                {
                                    Com_Identificacion = Reader["Com_Identificacion"].ToString(),
                                    Com_Cod_Prestador = Reader["Com_Cod_Prestador"].ToString(),
                                    Com_Cod_Prestador_2 = Reader["Com_Cod_Prestador_2"].ToString(),
                                    Com_Cotiza = Convert.ToInt32(Reader["Com_Cotiza"]),
                                    Com_DE = Convert.ToInt32(Reader["Com_DE"]),
                                    Com_Direccion = Reader["Com_Direccion"].ToString(),
                                    Com_Email = Reader["Com_Email"].ToString(),
                                    Com_Fac = Convert.ToInt32(Reader["Com_Fac"]),
                                    Com_Id = Reader["Com_Id"].ToString(),
                                    Com_Identificador = Convert.ToInt32(Reader["Com_Identificador"]),
                                    Com_Logo = Reader["Com_Logo"].ToString(),
                                    Com_Nombre = Reader["Com_Nombre"].ToString(),
                                    Com_Nombre_SMS = Reader["Com_Nombre_SMS"].ToString(),
                                    Com_OM = Convert.ToInt32(Reader["Com_OM"]),
                                    Com_OP = Convert.ToInt32(Reader["Com_OP"]),
                                    Com_PedPro = Convert.ToInt32(Reader["Com_PedPro"]),
                                    Com_Resolucion = Reader["Com_Resolucion"].ToString(),
                                    Com_RIP = Convert.ToInt32(Reader["Com_RIP"]),
                                    Com_Telefono = Reader["Com_Telefono"].ToString(),
                                    Com_Telefono_SMS = Reader["Com_Telefono_SMS"].ToString(),
                                    Com_Tipo_Doc = Reader["Com_Tipo_Doc"].ToString(),
                                    Com_UsuarioGraba = Reader["Com_UsuarioGraba"].ToString(),
                                    Com_SMS = Convert.ToInt32(Reader["Com_SMS"]),
                                    Com_Resolucion_Electron = Reader["Com_Resolucion_Electron"].ToString(),
                                    Com_Doc_Electron = Convert.ToInt32(Reader["Com_Doc_Electron"]),
                                    Com_Prefijo_Electron = Reader["Com_Prefijo_Electron"].ToString(),
                                    Com_Fecha_Electron = Convert.ToDateTime(Reader["Com_Fecha_Electron"]),
                                    Com_Numeracion_Electron = Reader["Com_Numeracion_Electron"].ToString(),
                                    Com_Doc_Electron_NC = Convert.ToInt32(Reader["Com_Doc_Electron_NC"]),
                                    Com_Prefijo_Electron_NC = Reader["Com_Prefijo_Electron_NC"].ToString(),
                                    Com_ConsContable = Convert.ToInt32(Reader["Com_ConsContable"]),
                                    Com_DVerifica = Reader["Com_DVerifica"].ToString(),

                                    Com_Doc_Soporte = Convert.ToInt32(Reader["Com_Doc_Soporte"]),
                                    Com_Doc_Soporte_NC = Convert.ToInt32(Reader["Com_Doc_Soporte_NC"]),
                                    Com_Fecha_Soporte = Convert.ToDateTime(Reader["Com_Fecha_Soporte"]),
                                    Com_Numeracion_Soporte = Reader["Com_Numeracion_Soporte"].ToString(),
                                    Com_Prefijo_Soporte = Reader["Com_Prefijo_Soporte"].ToString(),
                                    Com_Prefijo_Soporte_NC = Reader["Com_Prefijo_Soporte"].ToString(),
                                    Com_Resolucion_Soporte = Reader["Com_Resolucion_Soporte"].ToString(),
                                    Diferenciador = Reader["Diferenciador"].ToString(),
                                    Com_Cierres = Convert.ToInt32(Reader["Com_Cierres"]),
                                };

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

        List<CXN_CIA> ICompañia.getAllCompañias()
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

                    String Query = "SELECT Com_Nombre, Com_Identificador " +
                                   "FROM CXN_CIA ";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_CIA> L = new List<CXN_CIA>();

                        while (Reader.Read() == true)
                        {
                            L.Add(new CXN_CIA
                            {
                                Com_Nombre = Reader["Com_Nombre"].ToString(),
                                Com_Identificador = Convert.ToInt32(Reader["Com_Identificador"])
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

        bool ICompañia.ConsecutivoActualiza(int Cia, string TipoDoc, int NuevoCons)
        {
            var getCone = Conexion.Conection();
            using (SqlConnection con = new SqlConnection(getCone["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Query;

                switch (TipoDoc)
                {
                    case "OP":
                        Query = "UPDATE CXN_CIA " +
                                "SET Com_OP = '" + NuevoCons + "' " +
                                "WHERE Com_Identificador = '" + Cia + "'";
                        break;

                    case "FA":
                        Query = "UPDATE CXN_CIA " +
                                "SET Com_Fac = '" + NuevoCons + "' " +
                                "WHERE Com_Identificador = '" + Cia + "'";
                        break;

                    case "COTIZA":
                        Query = "UPDATE CXN_CIA " +
                                "SET Com_Cotiza = '" + NuevoCons + "' " +
                                "WHERE Com_Identificador = '" + Cia + "'";
                        break;

                    case "DE":
                        Query = "UPDATE CXN_CIA " +
                                "SET Com_DE = '" + NuevoCons + "' " +
                                "WHERE Com_Identificador = '" + Cia + "'";
                        break;

                    case "OM":
                        Query = "UPDATE CXN_CIA " +
                                "SET Com_OM = '" + NuevoCons + "' " +
                                "WHERE Com_Identificador = '" + Cia + "'";
                        break;

                    case "PEDPRO":
                        Query = "UPDATE CXN_CIA " +
                                "SET Com_PedPro = '" + NuevoCons + "' " +
                                "WHERE Com_Identificador = '" + Cia + "'";
                        break;

                    case "RIP":
                        Query = "UPDATE CXN_CIA " +
                                "SET Com_RIP = '" + NuevoCons + "' " +
                                "WHERE Com_Identificador = '" + Cia + "'";
                        break;

                    case "SMS":
                        Query = "UPDATE CXN_CIA " +
                                "SET Com_SMS = '" + NuevoCons + "' " +
                                "WHERE Com_Identificador = '" + Cia + "'";
                        break;

                    case "CONSELECTRON":
                        Query = "UPDATE CXN_CIA " +
                                "SET Com_Doc_Electron = '" + NuevoCons + "' " +
                                "WHERE Com_Identificador = '" + Cia + "'";
                        break;

                    case "CONSELECTRONNC":
                        Query = "UPDATE CXN_CIA " +
                                "SET Com_Doc_Electron_NC = '" + NuevoCons + "' " +
                                "WHERE Com_Identificador = '" + Cia + "'";
                        break;

                    case "SOPORTE":
                        Query = "UPDATE CXN_CIA " +
                                "SET Com_Doc_Soporte = '" + NuevoCons + "' " +
                                "WHERE Com_Identificador = '" + Cia + "'";
                        break;

                    case "SOPORTENC":
                        Query = "UPDATE CXN_CIA " +
                                "SET Com_Doc_Soporte_NC = '" + NuevoCons + "' " +
                                "WHERE Com_Identificador = '" + Cia + "'";
                        break;

                    case "CIERRES":
                        Query = "UPDATE CXN_CIA " +
                                "SET Com_Cierres = '" + NuevoCons + "' " +
                                "WHERE Com_Identificador = '" + Cia + "'";
                        break;

                    default:
                        Query = "";
                        return false;
                }

                SqlCommand Accion = new SqlCommand(Query, con);
                int Guarda;
                Guarda = Accion.ExecuteNonQuery();
                return true;
            }
        }
        bool ICompañia.updateCompañia(CXN_CIA C)
        {
            try
            {
                var getCone = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_CIA " +
                                      "SET Com_Nombre = '" + C.Com_Nombre + "', " +
                                      "Com_Identificacion = '" + C.Com_Identificacion + "', " +
                                      "Com_Direccion = '" + C.Com_Direccion + "', " +
                                      "Com_Telefono = '" + C.Com_Telefono + "', " +
                                      "Com_Cod_Prestador = '" + C.Com_Cod_Prestador + "', " +
                                      "Com_Cod_Prestador_2 = '" + C.Com_Cod_Prestador_2 + "', " +
                                      "Com_Email = '" + C.Com_Email + "', " +
                                      "Com_Resolucion = '" + C.Com_Resolucion + "', " +
                                      "Com_Tipo_Doc = '" + C.Com_Tipo_Doc + "', " +
                                      "Com_Nombre_SMS = '" + C.Com_Nombre_SMS + "', " +
                                      "Com_Telefono_SMS = '" + C.Com_Telefono_SMS + "', " +
                                      "Com_OP = '" + C.Com_OP + "', " +
                                      "Com_Fac = '" + C.Com_Fac + "', " +
                                      "Com_Cotiza = '" + C.Com_Cotiza + "', " +
                                      "Com_DE = '" + C.Com_DE + "', " +
                                      "Com_OM = '" + C.Com_OM + "', " +
                                      "Com_UsuarioGraba = '" + C.Com_UsuarioGraba + "' " +
                                      "WHERE Com_Identificador = '" + C.Com_Identificador + "'";
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
        bool ICompañia.createCompañia(CXN_CIA C)
        {
            try
            {
                var getCone = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_CIA (Com_Nombre, " + //param2
                                                          "Com_Identificacion, " + //param3
                                                          "Com_Direccion, " + //param4
                                                          "Com_Telefono, " + //param5
                                                          "Com_Cod_Prestador, " + //param7
                                                          "Com_Cod_Prestador_2, " + //param8
                                                          "Com_Email, " + //param9
                                                          "Com_Resolucion, " + //param10
                                                          "Com_Tipo_Doc, " + //param11
                                                          "Com_UsuarioGraba, " + //param12
                                                          "Com_OP, " +
                                                          "Com_Fac, " +
                                                          "Com_Cotiza, " +
                                                          "Com_DE, " +
                                                          "Com_OM, " +
                                                          "Com_Identificador, " +
                                                          "Com_Nombre_SMS, " +
                                                          "Com_Telefono_SMS ) " + //param16
                                 "values                  (@param1, " + // Hor_Estado
                                                          "@param2, " + // Hor_Pac_Id
                                                          "@param3, " + // Hor_Pac_Bod
                                                          "@param4, " + // Hor_Pac_Tipo_Serv
                                                          "@param6, " + // Hor_Pac_Ase
                                                          "@param7, " + // Hor_Pac_Cup
                                                          "@param8, " + // Hor_Pac_UsrGraba
                                                          "@param9, " + // Hor_Imp_Age
                                                          "@param10, " + // Hor_Pac_Fecha
                                                          "@param11, " + // Hor_Pac_Fecha_Cita
                                                          "@param12, " +
                                                          "@param13, " +
                                                          "@param14, " +
                                                          "@param15, " +
                                                          "@param16, " +
                                                          "@param17, " +
                                                          "@param18, " +
                                                          "@param19)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", C.Com_Nombre);
                    cmd.Parameters.AddWithValue("@param2", C.Com_Identificacion);
                    cmd.Parameters.AddWithValue("@param3", C.Com_Direccion);
                    cmd.Parameters.AddWithValue("@param4", C.Com_Telefono);
                    cmd.Parameters.AddWithValue("@param6", C.Com_Cod_Prestador);
                    cmd.Parameters.AddWithValue("@param7", C.Com_Cod_Prestador_2);
                    cmd.Parameters.AddWithValue("@param8", C.Com_Email);
                    cmd.Parameters.AddWithValue("@param9", C.Com_Resolucion);
                    cmd.Parameters.AddWithValue("@param10", C.Com_Tipo_Doc);
                    cmd.Parameters.AddWithValue("@param11", C.Com_UsuarioGraba);
                    cmd.Parameters.AddWithValue("@param12", C.Com_OP);
                    cmd.Parameters.AddWithValue("@param13", C.Com_Fac);
                    cmd.Parameters.AddWithValue("@param14", C.Com_Cotiza);
                    cmd.Parameters.AddWithValue("@param15", C.Com_DE);
                    cmd.Parameters.AddWithValue("@param16", C.Com_OM);
                    cmd.Parameters.AddWithValue("@param17", C.Com_Identificador);
                    cmd.Parameters.AddWithValue("@param18", C.Com_Nombre_SMS);
                    cmd.Parameters.AddWithValue("@param19", C.Com_Telefono_SMS);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        void ICompañia.grabaLogo(CXN_CIA C)
        {
            try
            {
                var getCone = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_CIA " +
                                      "SET Com_Logo = '" + C.Com_Logo + "' " +
                                      "WHERE Com_Identificador = '" + C.Com_Identificador + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        bool ICompañia.updateDataElectron(CXN_CIA C)
        {
            try
            {
                var getCone = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_CIA " +
                                      "SET Com_Resolucion_Electron = @param1, " +
                                      "Com_Doc_Electron = @param2, " +
                                      "Com_Prefijo_Electron = @param3, " +
                                      "Com_Fecha_Electron = @param4, " +
                                      "Com_Numeracion_Electron = @param5, " +
                                      "Com_Doc_Electron_NC = @param6, " +
                                      "Com_Prefijo_Electron_NC = @param7 " +
                                      "WHERE Com_Identificador = @param8", con);

                    Busqueda.Parameters.AddWithValue("@param1", C.Com_Resolucion_Electron);
                    Busqueda.Parameters.AddWithValue("@param2", C.Com_Doc_Electron);
                    Busqueda.Parameters.AddWithValue("@param3", C.Com_Prefijo_Electron);
                    Busqueda.Parameters.AddWithValue("@param4", Convert.ToDateTime(C.Com_Fecha_Electron));
                    Busqueda.Parameters.AddWithValue("@param5", C.Com_Numeracion_Electron);
                    Busqueda.Parameters.AddWithValue("@param6", C.Com_Doc_Electron_NC);
                    Busqueda.Parameters.AddWithValue("@param7", C.Com_Prefijo_Electron_NC);
                    Busqueda.Parameters.AddWithValue("@param8", C.Com_Identificador);

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
    }
}
