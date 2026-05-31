using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MHomologos : IHomologos
    {
        List<CXN_FACTURA> IHomologos.Filtra_Generales(string Tipo, int Compañia, int Factura)
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

                    String Cargar_Hora = "";

                    if (Tipo == "UNOGeneral")
                    {
                        Cargar_Hora = "SELECT F.Fac_Num_Fac as FACTURA, F.Homologo as HOMOLOGO, F.Fac_Fecha as FECHA, " +
                                         "P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS Nombre, " +
                                         "F.Fac_Id AS POS, F.Fac_Usr_Graba AS USERG, A.Ase_Descripcion AS ASEG, F.Fac_Tipo_Doc AS TDOC " +
                                         "FROM CXN_FACTURA F " +
                                         "INNER JOIN CXN_PACIENTES P ON F.Fac_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_ASEGURADORA A ON F.Fac_Ase = A.Ase_Identificador " +
                                         "WHERE F.Fac_Num_Fac = '" + Factura + "' " +
                                         "AND F.Fac_Estado = 'F' " +
                                         "AND F.Fac_Cia = '" + Compañia + "' " +
                                         "ORDER BY F.Fac_Num_Fac ASC";
                    }

                    if (Tipo == "UNOVentas")
                    {
                        Cargar_Hora = "SELECT V.Ven_Factura  as FACTURA, V.Ven_Homologo  as HOMOLOGO, V.Ven_Fecha  as FECHA, " +
                                         "P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS Nombre, " +
                                         "V.Ven_Id AS POS, V.Ven_Usr_Graba AS USERG, A.Ase_Descripcion AS ASEG, V.Ven_Tipo_Doc AS TDOC " +
                                         "FROM CXN_VENTAS V " +
                                         "INNER JOIN CXN_PACIENTES P ON V.Ven_Cod_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_ASEGURADORA A ON P.Pac_Aseguradora = A.Ase_Identificador " +
                                         "WHERE V.Ven_Factura = '" + Factura + "' " +
                                         "AND V.Ven_Estado = 'F' " +
                                         "AND V.Ven_Cod_Cia = '" + Compañia + "' " +
                                         "ORDER BY V.Ven_Factura ASC";
                    }

                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_FACTURA> F = new List<CXN_FACTURA>();

                        while (Lectura_Hora.Read() == true)
                        {
                            F.Add(new CXN_FACTURA
                            {
                                Fac_Id = Convert.ToInt32(Lectura_Hora["POS"]),
                                Fac_Num_Fac = Convert.ToInt32(Lectura_Hora["FACTURA"]),
                                Homologo = Lectura_Hora["HOMOLOGO"].ToString(),
                                Fac_Observa = Lectura_Hora["Nombre"].ToString(),
                                Fac_Fecha = Convert.ToDateTime(Lectura_Hora["FECHA"]),
                                Fac_Res = Lectura_Hora["ASEG"].ToString(),
                                Fac_Usr_Graba = Lectura_Hora["USERG"].ToString(),
                                Fac_Tipo_Doc = Lectura_Hora["TDOC"].ToString()
                            });
                        }

                        return F;
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

        bool IHomologos.ConsultarExistenciaGeneral(string Homologo, int Compañia)
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

                    String Cargar_Hora = "SELECT Homologo " +
                                         "FROM CXN_FACTURA " +
                                         "WHERE Homologo = '" + Homologo + "' " +
                                         "AND Fac_Estado = 'F' " +
                                         "AND Fac_Cia = '" + Compañia + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return true;
            }
        }

        bool IHomologos.ConsultarExistenciaVentas(string Homologo, int Compañia)
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

                    String Cargar_Hora = "SELECT Ven_Homologo " +
                                         "FROM CXN_VENTAS " +
                                         "WHERE Ven_Homologo = '" + Homologo + "' " +
                                         "AND Ven_Estado = 'F' " +
                                         "AND Ven_Cod_Cia = '" + Compañia + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return true;
            }
        }

        bool IHomologos.AddHomologoGeneral(string Homologo, int Pos, DateTime Fecha, DateTime Hora, string CUFE, string Resolucion)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_FACTURA " +
                                                          "SET  " +
                                                          "Homologo = @param1, " +
                                                          "Fac_Fecha = @param2, " +
                                                          "Cufe = @param3, " +
                                                          "Hora = @param4, " +
                                                          "Fac_Res = @param5 " +
                                                          "WHERE Fac_Id = @param6", con);

                    Busqueda.Parameters.AddWithValue("@param1", Homologo);
                    Busqueda.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = DateTime.Now.Date;
                    Busqueda.Parameters.AddWithValue("@param3", CUFE);
                    Busqueda.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = DateTime.Now;
                    Busqueda.Parameters.AddWithValue("@param5", Resolucion);
                    Busqueda.Parameters.AddWithValue("@param6", Pos);
                    int D = Busqueda.ExecuteNonQuery();

                    if (D >= 1)
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

        bool IHomologos.AddHomologoRecepcion(string Homologo, int Facs, DateTime Fecha, DateTime Hora, string CUFE, string Resolucion)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_VENTAS " +
                                                        "SET  " +
                                                        "Ven_Homologo = @param1, " +
                                                        "Ven_Fecha = @param2, " +
                                                        "Cufe = @param3, " +
                                                        "Hora = @param4, " +
                                                        "Ven_Res = @param5 " +
                                                        "WHERE Ven_Factura = @param6", con);

                    Busqueda.Parameters.AddWithValue("@param1", Homologo);
                    Busqueda.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = DateTime.Now.Date;
                    Busqueda.Parameters.AddWithValue("@param3", CUFE);
                    Busqueda.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = DateTime.Now;
                    Busqueda.Parameters.AddWithValue("@param5", Resolucion);
                    Busqueda.Parameters.AddWithValue("@param6", Facs);
                    int D = Busqueda.ExecuteNonQuery();

                    if (D >= 1)
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

        bool IHomologos.ConsultarRecibo(int FZ, string FC)
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

                    String Cargar_Hora = "SELECT TOP 1 Hor_DocFEModerador " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_DocFEModerador = '" + FC + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }

        bool IHomologos.ConsultarFactura(int FZ, string FC)
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

                    String Cargar_Hora = "SELECT Fac_Num_Fac " +
                                         "FROM Cxn_Factura " +
                                         "WHERE Fac_Num_Fac = '" + FZ + "' " +
                                         "AND Fac_Estado = 'F' " +
                                         "AND Fac_Cia = '" + FC + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }

     
        bool IHomologos.Verifica_Homologo(string FH, string FC)
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
                    String Cargar_Hora = "SELECT Homologo " +
                                         "FROM Cxn_Factura " +
                                         "WHERE Homologo = '" + FH + "' " +
                                         "AND Fac_Estado = 'F' " +
                                         "AND Fac_Cia = '" + FC + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return true;
            }
        }

        bool IHomologos.Verifica_HomologoRec(string FH, string FC)
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
                    String Cargar_Hora = "SELECT Ven_Homologo " +
                                         "FROM Cxn_Ventas " +
                                         "WHERE Ven_Homologo = '" + FH + "' " +
                                         "AND Ven_Estado = 'F' " +
                                         "AND Ven_Cod_Cia = '" + FC + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return true;
            }
        }

        bool IHomologos.ActualizarMasivo(string FH, int FZ, string FC, DateTime FF, string CUFE, DateTime HORA, string RESOLUCION)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE Cxn_Factura " +
                                                          "SET  " +
                                                          "Homologo = @param1, " +
                                                          "Fac_Fecha = @param2, " +
                                                          "Cufe = @param3, " +
                                                          "Hora = @param4, " +
                                                          "Fac_Res = @paramRes " +
                                                          "WHERE Fac_Num_Fac = @param5 " +
                                                          "AND Fac_Estado = @param6 " +
                                                          "AND Fac_Cia = @param7", con);

                    Busqueda.Parameters.AddWithValue("@param1", FH);
                    Busqueda.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(FF).ToString(getData["Format_Fecha"]);
                    Busqueda.Parameters.AddWithValue("@param3", CUFE);
                    Busqueda.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = Convert.ToDateTime(HORA).ToString("hh:mm:ss tt");
                    Busqueda.Parameters.AddWithValue("@param5", FZ);
                    Busqueda.Parameters.AddWithValue("@param6", "F");
                    Busqueda.Parameters.AddWithValue("@param7", FC);
                    Busqueda.Parameters.AddWithValue("@paramRes", RESOLUCION);
                    int d = Busqueda.ExecuteNonQuery();
                    if (d <= 0)
                    {
                        TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "ERROR ACTUALIZANDO", Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                        return false;
                    }                
                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }

        bool IHomologos.ActualizarMasivoRecibo(int Recibo, string Homologo, string CUFE, string Resolucion)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_HORARIO " +
                                                          "SET  " +
                                                          "Hor_DocFEModerador = @param2, " +
                                                          "Hor_DocFEModeradorCUFE = @param3, " +
                                                          "Hor_DocFEModeradorRes = @param4, " +
                                                          "Hor_DocFEModeradorFechaHora = @param5 " +
                                                          "WHERE Hor_Id = @param1", con);

                    Busqueda.Parameters.AddWithValue("@param1", Recibo);
                    Busqueda.Parameters.AddWithValue("@param2", Homologo);
                    Busqueda.Parameters.AddWithValue("@param3", CUFE);
                    Busqueda.Parameters.AddWithValue("@param4", Resolucion);
                    Busqueda.Parameters.AddWithValue("@param5", Convert.ToDateTime(DateTime.Now));

                    int d = Busqueda.ExecuteNonQuery();
                    if (d <= 0)
                    {
                        TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "ERROR ACTUALIZANDO", Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }

        bool IHomologos.ActualizarMasivoRec(string FH, int FZ, string FC, DateTime FF, string CUFE, DateTime HORA, string RESOLUCION)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE Cxn_Ventas " +
                                                          "SET  " +
                                                          "Ven_Homologo = @param1, " +
                                                          "Ven_Fecha = @param2, " +
                                                          "Cufe = @param3, " +
                                                          "Hora = @param4, " +
                                                          "Ven_Res = @paramRes " +
                                                          "WHERE Ven_Factura = @param5 " +
                                                          "AND Ven_Estado = @param6 " +
                                                          "AND Ven_Cod_Cia = @param7", con);

                    Busqueda.Parameters.AddWithValue("@param1", FH);
                    Busqueda.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(FF).ToString(getData["Format_Fecha"]);
                    Busqueda.Parameters.AddWithValue("@param3", CUFE);
                    Busqueda.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = Convert.ToDateTime(HORA).ToString("hh:mm:ss tt");
                    Busqueda.Parameters.AddWithValue("@param5", FZ);
                    Busqueda.Parameters.AddWithValue("@param6", "F");
                    Busqueda.Parameters.AddWithValue("@param7", FC);
                    Busqueda.Parameters.AddWithValue("@paramRes", RESOLUCION);
                    int d = Busqueda.ExecuteNonQuery();
                    if (d <= 0)
                    {
                        TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "ERROR ACTUALIZANDO", Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            return false;
            }
        }

        DataView IHomologos.ImportarDatos(string nombrearchivo)
        {
            try
            {
                string conexion = string.Format("Provider = Microsoft.ACE.OLEDB.12.0; Data Source = {0}; Extended Properties = 'Excel 12.0;'", nombrearchivo);
                OleDbConnection conector = new OleDbConnection(conexion);
                conector.Open();
                OleDbCommand consulta = new OleDbCommand("select * from [Hoja1$]", conector);
                OleDbDataAdapter adaptador = new OleDbDataAdapter
                {
                    SelectCommand = consulta
                };
                DataSet ds = new DataSet();
                adaptador.Fill(ds);
                conector.Close();
                return ds.Tables[0].DefaultView;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }          
        }       
    }
}
