using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using Domain;

namespace Persistence.CXN.Metodos
{

    public class MCManenejo : ICManejo
    {
        private static readonly IBodegas repoBod = new MBodegas();

        bool ICManejo.insertarCambioManejo(CXN_CMAN C)
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

                    DateTime Cam_Fecha = DateTime.Now;

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_CMAN (Cam_IdPac, " +
                                                              "Cam_Descripcion, " +
                                                              "Cam_Fecha, " +
                                                              "Cam_Estado, " +
                                                              "Cam_UsrGenera, " +
                                                              "Cam_Cia) " +
                                     "values                  (@param1, " +
                                                              "@param2, " +
                                                              "@param3, " +
                                                              "@param4, " +
                                                              "@param5, " +
                                                              "@param6)", con);

                    cmd.Parameters.AddWithValue("@param1", C.Cam_IdPac);
                    cmd.Parameters.AddWithValue("@param2", C.Cam_Descripcion);
                    cmd.Parameters.Add(new SqlParameter("@param3", SqlDbType.DateTime)).Value = Cam_Fecha;
                    cmd.Parameters.AddWithValue("@param4", C.Cam_Estado);
                    cmd.Parameters.AddWithValue("@param5", C.Cam_UsrGenera);
                    cmd.Parameters.AddWithValue("@param6", C.Cam_Cia);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        List<CXN_CMAN> ICManejo.cargarCambiosPendientes()
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

                    String Cargar_Hora = "SELECT C.Cam_Id, P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA as Nombre, C.Cam_Fecha, C.Cam_Descripcion " +
                                         "FROM CXN_CMAN C " +
                                         "INNER JOIN CXN_PACIENTES P ON C.Cam_IdPac = P.Pac_Id " +
                                         "INNER JOIN CXN_BODEGAS B ON C.Cam_UsrGenera = B.Bod_Usuario " +
                                         "WHERE C.Cam_Estado = 'G' " +
                                         "ORDER BY P.Pac_Id";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_CMAN> L = new List<CXN_CMAN>();

                        while (Lectura_Hora.Read() == true)
                        {
                            L.Add(new CXN_CMAN
                            {
                                Cam_Id = Convert.ToInt32(Lectura_Hora["Cam_Id"]),
                                Cam_Adherencia = Lectura_Hora["Nombre"].ToString(), //pacinete
                                Cam_Fecha = Convert.ToDateTime(Lectura_Hora["Cam_Fecha"]),
                                Cam_Descripcion = Lectura_Hora["Cam_Descripcion"].ToString()
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

        bool ICManejo.updateCambioManejo(CXN_CMAN C)
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

                    DateTime Hoy = DateTime.Now;

                    string Busqueda = "UPDATE CXN_CMAN " +
                                      "SET Cam_Estado = 'R', " +
                                      "Cam_UsrActualiza = '" + C.Cam_UsrActualiza + "', " +
                                      "Cam_FechaActualiza = '" + Convert.ToDateTime(Hoy.Date).ToString(getCon["Format_Fecha"]) + "', " +
                                      "Cam_Adherencia = '" + C.Cam_Adherencia.ToUpper() + "', " +
                                      "Cam_ActualizaOb = '" + C.Cam_ActualizaOb + "' " +
                                      "WHERE Cam_Id = '" + C.Cam_Id + "' " +
                                      "AND Cam_Estado = 'G'";
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

        int ICManejo.getIdPacByIdCMan(int IdPac)
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
                    String Cargar_Hora = "SELECT Cam_IdPac " +
                                         "FROM CXN_CMAN " +
                                         "WHERE Cam_Id = '" + IdPac + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return Convert.ToInt32(Lectura_Hora["Cam_IdPac"]);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        List<CXN_CMAN> ICManejo.getManejosxPaciente(int IdPac)
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
                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_CMAN " +
                                         "WHERE Cam_IdPac = '" + IdPac + "' " +
                                         "ORDER BY Cam_Fecha DESC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_CMAN> L = new List<CXN_CMAN>();

                        while (Lectura_Hora.Read() == true)
                        {
                            L.Add(new CXN_CMAN
                            {
                                Cam_Id = Convert.ToInt32(Lectura_Hora["Cam_Id"]),
                                Cam_UsrGenera = Lectura_Hora["Cam_UsrGenera"].ToString(),
                                Cam_Fecha = Convert.ToDateTime(Lectura_Hora["Cam_Fecha"]),
                                Cam_Adherencia = Lectura_Hora["Cam_Adherencia"].ToString()
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

        List<int> ICManejo.getManejosxPaciente(int IdPac, DateTime Desde, DateTime Hasta)
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
                    
                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_CMAN " +
                                         "WHERE Cam_IdPac = @param1 " +
                                         "AND Cam_Fecha BETWEEN @param2 AND @param3 " +
                                         "ORDER BY Cam_Fecha ASC";
                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", IdPac);
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Desde;
                        Carga_Command.Parameters.Add(new SqlParameter("@param3", SqlDbType.DateTime)).Value = Hasta;

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<int> L = new List<int>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(Convert.ToInt32(Lectura_Hora["Cam_Id"]));
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

        (int IdHc, string NotaAclaratoria) ICManejo.getLastIdByIdPac(int IdPac)
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
                    String Cargar_Hora = "SELECT TOP 1 HC_Id, HC_Nota_Acl " +
                                         "FROM CXN_HCMG " +
                                         "WHERE HC_Pacid = '" + IdPac + "'" +
                                         "ORDER BY HC_Fecha DESC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return (1, Lectura_Hora["HC_Nota_Acl"].ToString());
                    }
                    else
                    {
                        return (0, "");
                    }
                }
            }
            catch (Exception ex)
            {
                return (0, ex.Message);
            }
        }

        bool ICManejo.insertNotaAclaratoria(int IdHC, string texto)
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

                    string Busqueda = "UPDATE CXN_HCMG " +
                                      "SET HC_Nota_Acl = '" + texto + "' " +
                                      "WHERE HC_Id = '" + IdHC + "'";
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

        bool ICManejo.GrabarManejo(string Tipo,
                                     int Admision,
                                     string NotaNueva,
                                     string User)
        {
            Dictionary<string,string> getData = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getData["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                DateTime Hoy = DateTime.Now.Date;
                var Prof = repoBod.getDatosUser(User);
                if (Prof == null)
                {
                    return false;
                }

                var NotaAnt = Buscar_Anterior(Admision, Tipo);

                string Dato_Grabar = NotaAnt.ToString() +
                                     NotaNueva.ToUpper() +
                                     " --> NOTA GRABADA EL DIA: " +
                                     Convert.ToDateTime(Hoy).ToString(getData["Format_Fecha"]) +
                                     " POR: " +
                                     Prof.Bod_Responsable.ToString() +
                                     " | ";

                switch (Tipo)
                {
                    case "Curaciones":
                        string Busqueda = "UPDATE CXN_NOTAS " +
                                          "SET Not_NotaAcla = '" + Dato_Grabar + "' " +
                                          "WHERE Not_Adm = '" + Admision + "'";
                        SqlCommand Accion = new SqlCommand(Busqueda, con);
                        int Guarda;
                        Guarda = Accion.ExecuteNonQuery();
                        return true;

                    case "MedGen":
                        string Busqueda2 = "UPDATE CXN_HCMG " +
                                           "SET HC_Nota_Acl = '" + Dato_Grabar + "' " +
                                           "WHERE HC_Adm = '" + Admision + "'";
                        SqlCommand Accion2 = new SqlCommand(Busqueda2, con);
                        int Guarda2;
                        Guarda2 = Accion2.ExecuteNonQuery();
                        return true;

                    case "Fisiatria":
                        string Busqueda3 = "UPDATE CXN_HCFI " +
                                          "SET HC_NotaA = '" + Dato_Grabar + "' " +
                                          "WHERE HC_Adm = '" + Admision + "'";
                        SqlCommand Accion3 = new SqlCommand(Busqueda3, con);
                        int Guarda3;
                        Guarda3 = Accion3.ExecuteNonQuery();
                        return true;

                    case "Radiologia":
                        string Busqueda4 = "UPDATE CXN_HCRADIOLOGIA " +
                                           "SET NotaAclaratoria = '" + Dato_Grabar + "' " +
                                           "WHERE HCAdm = '" + Admision + "'";
                        SqlCommand Accion4 = new SqlCommand(Busqueda4, con);
                        int Guarda4;
                        Guarda4 = Accion4.ExecuteNonQuery();
                        return true;

                    default:
                        return false;
                }
            }
        }

        string Buscar_Anterior(int Admision,
                               string Tipo)
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

                    String Cargar_Hora;

                    switch (Tipo)
                    {
                        case "Curaciones":
                            Cargar_Hora = "SELECT Not_NotaAcla AS NOTAANTERIOR " +
                                          "FROM CXN_NOTAS " +
                                          "WHERE Not_Adm = '" + Admision + "'";
                            break;

                        case "MedGen":
                            Cargar_Hora = "SELECT HC_Nota_Acl " +
                                         " FROM CXN_HCMG " +
                                         " WHERE HC_Adm = '" + Admision + "'";
                            break;

                        case "Fisiatria":
                            Cargar_Hora = "SELECT HC_NotaA " +
                                          " FROM CXN_HCFI " +
                                          " WHERE HC_Adm = '" + Admision + "'";
                            break;

                        default:
                            Cargar_Hora = "";
                            break;
                    }

                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return Lectura_Hora["NOTAANTERIOR"].ToString();
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

     
    }
}
