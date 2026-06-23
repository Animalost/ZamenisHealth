using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MCruces : ICruces
    {
        CXN_VENTAS ICruces.GetVentaRecepcion(string FacElectron)
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

                    String Cargar_Hora = "SELECT V.Ven_Factura, V.Ven_Fecha, V.Ven_Id, V.Ven_Cruce, " +
                                         "P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_TipoId, P.Pac_IdNum " +
                                         "FROM CXN_VENTAS V " +
                                         "INNER JOIN CXN_PACIENTES P ON V.Ven_Cod_Pac = P.Pac_Id " +
                                         "WHERE V.Ven_Homologo = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", FacElectron);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                string hecho = "X";

                                if (Lectura_Hora["Ven_Cruce"] != DBNull.Value)
                                {
                                    if (Lectura_Hora["Ven_Cruce"].ToString() != "")
                                    {
                                        hecho = "V";
                                    }
                                }

                                CXN_VENTAS L = new CXN_VENTAS()
                                {
                                    Ven_Factura = Lectura_Hora["Ven_Factura"].ToString(),
                                    Ven_Fecha = Convert.ToDateTime(Lectura_Hora["Ven_Fecha"]),
                                    Ven_Usr_Graba = Lectura_Hora["Pac_PrimerA"].ToString() + " " +
                                                    Lectura_Hora["Pac_SegundoA"].ToString() + " " +
                                                    Lectura_Hora["Pac_PrimerN"].ToString() + " " +
                                                    Lectura_Hora["Pac_SegundoN"].ToString(),
                                    Ven_Item = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                    Ven_Id = Convert.ToInt32(Lectura_Hora["Ven_Id"]),
                                    Grafica = hecho
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
        bool ICruces.Cruzar(int Pos, string Tabla, string Usuario)
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

                    string Busqueda = "";
                    
                    switch (Tabla)
                    {
                        case "Ventas":
                            Busqueda = "UPDATE CXN_VENTAS " +
                                       "SET " +
                                       "Ven_UsrCruce = @param1, " +
                                       "Ven_Cruce = @param2, " +
                                       "Ven_FechaCruce = @param3 " +
                                       "WHERE Ven_Id = @param4";
                            break;

                        case "Bonos":
                            Busqueda = "UPDATE CXN_RC_CAJA " +
                                       "SET Hor_Cruce = @param2, " +
                                       "Hor_FechaCruce = @param3 " +
                                       "WHERE Rc_Id = @param4";
                            break;

                        case "Particulares":
                            Busqueda = "UPDATE CXN_FACTURA " +
                                       "SET " +
                                       "Fac_UsrCruce = @param1, " +
                                       "Fac_Cruce = @param2, " +
                                       "Fac_FechaCruce = @param3 " +
                                       "WHERE Fac_Id = @param4";
                            break;

                        default:
                            return false;
                    }
          
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@param1", Usuario);
                    Accion.Parameters.AddWithValue("@param2", "H");
                    Accion.Parameters.AddWithValue("@param3", Convert.ToDateTime(DateTime.Now.Date));
                    Accion.Parameters.AddWithValue("@param4", Pos);

                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    if (Guarda > 0)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        CXN_HORARIO ICruces.GetRecaudosBonos(string FacElectron)
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

                    String Cargar_Hora = "SELECT H.Rc_Id, H.Rc_Caja_Fecha, 'BONOS' AS Hor_Imp_Age, H.Hor_Cruce," +
                                         "P.Pac_TipoId, P.Pac_IdNum " +
                                         "FROM CXN_RC_CAJA H " +
                                         "INNER JOIN CXN_PACIENTES P ON H.Rc_Caja_Pac = P.Pac_Id " +
                                         "WHERE H.Hor_DocFEModerador = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", FacElectron);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                string hecho = "X";

                                if (Lectura_Hora["Hor_Cruce"] != DBNull.Value)
                                {
                                    if (Lectura_Hora["Hor_Cruce"].ToString() != "")
                                    {
                                        hecho = "V";
                                    }
                                }

                                CXN_HORARIO L = new CXN_HORARIO()
                                {
                                    Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Rc_Caja_Fecha"]),
                                    Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                    Hor_Pac_Tipo_Serv = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                    Hor_Id = Convert.ToInt32(Lectura_Hora["Rc_Id"]),
                                    Hor_ValDerechos = hecho
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
        CXN_FACTURA ICruces.Particulares(string FacElectron)
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

                    String Cargar_Hora = "SELECT P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_TipoId, P.Pac_IdNum, " +
                                         "F.Fac_Cruce, F.Fac_Fecha, F.Fac_Id " +
                                         "FROM CXN_FACTURA F " +
                                         "INNER JOIN CXN_PACIENTES P ON F.Fac_Pac = P.Pac_Id " +
                                         "WHERE F.Homologo = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", FacElectron);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                string hecho = "X";

                                if (Lectura_Hora["Fac_Cruce"] != DBNull.Value)
                                {
                                    if (Lectura_Hora["Fac_Cruce"].ToString() != "")
                                    {
                                        hecho = "V";
                                    }
                                }

                                CXN_FACTURA L = new CXN_FACTURA()
                                {
                                    Fac_Fecha = Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]),
                                    Fac_Usr_Graba = Lectura_Hora["Pac_PrimerA"].ToString() + " " +
                                                    Lectura_Hora["Pac_SegundoA"].ToString() + " " +
                                                    Lectura_Hora["Pac_PrimerN"].ToString() + " " +
                                                    Lectura_Hora["Pac_SegundoN"].ToString(),
                                    FacResNumeracion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                    Fac_Id = Convert.ToInt32(Lectura_Hora["Fac_Id"]),
                                    QRCufe = hecho
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
        void ICruces.EliminaCierre(int Num_Cierre)
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

                    string Busqueda = "UPDATE CXN_VENTAS " +
                                      "SET " +
                                      "Num_Cruce = @param1 " +
                                      "WHERE Num_Cruce = @param2";                    

                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@param1", 0);
                    Accion.Parameters.AddWithValue("@param2", Num_Cierre);
                    Accion.ExecuteNonQuery();

                    string Busqueda2 = "UPDATE CXN_FACTURA " +
                                       "SET " +
                                       "Num_Cruce = @param1 " +
                                       "WHERE Num_Cruce = @param2";

                    SqlCommand Accion2 = new SqlCommand(Busqueda2, con);
                    Accion2.Parameters.AddWithValue("@param1", 0);
                    Accion2.Parameters.AddWithValue("@param2", Num_Cierre);
                    Accion2.ExecuteNonQuery();

                    string Busqueda3 = "UPDATE CXN_RC_CAJA " +
                                       "SET " +
                                       "Num_Cruce = @param1 " +
                                       "WHERE Num_Cruce = @param2";

                    SqlCommand Accion3 = new SqlCommand(Busqueda3, con);
                    Accion3.Parameters.AddWithValue("@param1", 0);
                    Accion3.Parameters.AddWithValue("@param2", Num_Cierre);
                    Accion3.ExecuteNonQuery();

                    string Busqueda4 = "UPDATE CXN_REPORTECAJA2 " +
                                      "SET " +
                                      "Estado = @param1 " +
                                      "WHERE Consecutivo = @param2";

                    SqlCommand Accion4 = new SqlCommand(Busqueda4, con);
                    Accion4.Parameters.AddWithValue("@param1", "A");
                    Accion4.Parameters.AddWithValue("@param2", Num_Cierre);
                    Accion4.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
