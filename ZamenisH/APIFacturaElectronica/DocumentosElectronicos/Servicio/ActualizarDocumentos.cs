using Domain;
using Persistence;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DocumentosElectronicos.Servicio
{
    public class ActualizarDocumentos
    {
        public static bool AddHomologoAseguradoras(UpdateFacturaElectronica F)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_FACTURA " +
                                                          "SET  " +
                                                          "Homologo = @param1, " +
                                                          "Fac_Fecha = @param2, " +
                                                          "Cufe = @param3, " +
                                                          "Hora = @param4, " +
                                                          "Fac_Res = @param5, " +
                                                          "FacResNumeracion = @param6 " +
                                                          "WHERE Fac_Num_Fac = @param7 " +
                                                          "AND Fac_Cia = @param8", con);

                    Busqueda.Parameters.AddWithValue("@param1", F.FacturaElectronica);
                    Busqueda.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = DateTime.Now.Date;
                    Busqueda.Parameters.AddWithValue("@param3", F.Cufe);
                    Busqueda.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = DateTime.Now;
                    Busqueda.Parameters.AddWithValue("@param5", F.Resolucion);
                    Busqueda.Parameters.AddWithValue("@param6", F.ResolucionNumeracion);
                    Busqueda.Parameters.AddWithValue("@param7", F.FacturaZamenis);
                    Busqueda.Parameters.AddWithValue("@param8", F.Prestador);

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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "AcualizarDocumentos", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        public static bool AddHomologoCaja(UpdateFacturaElectronica F)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_RC_CAJA " +
                                                          "SET  " +
                                                          "Hor_DocFEModerador = @param1, " +
                                                          "Hor_DocFEModeradorCUFE = @param2, " +
                                                          "Hor_DocFEModeradorRes = @param3, " +
                                                          "Hor_DocFEModeradorFechaHora = @param4, " +
                                                          "Hor_DocFEModeradorNumeracion = @param5 " +
                                                          "WHERE Rc_Id = @param6", con);

                    Busqueda.Parameters.AddWithValue("@param1", F.FacturaElectronica);
                    Busqueda.Parameters.AddWithValue("@param2", F.Cufe);
                    Busqueda.Parameters.AddWithValue("@param4", Convert.ToDateTime(DateTime.Now));
                    Busqueda.Parameters.AddWithValue("@param3", F.Resolucion);
                    Busqueda.Parameters.AddWithValue("@param5", F.ResolucionNumeracion);
                    Busqueda.Parameters.AddWithValue("@param6", F.FacturaZamenis);

                    int d = Busqueda.ExecuteNonQuery();
                    if (d <= 0)
                    {
                        TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "ERROR ACTUALIZANDO", Formulario = "AcualizarDocumentos", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "AcualizarDocumentos", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        public static bool AddHomologoVentas(UpdateFacturaElectronica F)
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
                                                          "Ven_Res = @param5 " +
                                                          "WHERE Ven_Factura = @param6 " +
                                                          "AND Ven_Estado = @param7 " +
                                                          "AND Ven_Cod_Cia = @param8", con);

                    Busqueda.Parameters.AddWithValue("@param1", F.FacturaElectronica);
                    Busqueda.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(F.Fecha).ToString(getData["Format_Fecha"]);
                    Busqueda.Parameters.AddWithValue("@param3", F.Cufe);
                    Busqueda.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = Convert.ToDateTime(F.Hora).ToString("hh:mm:ss tt");
                    Busqueda.Parameters.AddWithValue("@param5", F.Resolucion);
                    Busqueda.Parameters.AddWithValue("@param6", F.FacturaZamenis);
                    Busqueda.Parameters.AddWithValue("@param7", "F");
                    Busqueda.Parameters.AddWithValue("@param8", F.Prestador);
                    int d = Busqueda.ExecuteNonQuery();
                    if (d <= 0)
                    {
                        TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "ERROR ACTUALIZANDO", Formulario = "AcualizarDocumentos", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "AcualizarDocumentos", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }


        public static int GetFacZamenis(string DocumentoElectronico, int Compañia)
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

                    String Cargar_Hora = "SELECT Fac_Num_Fac " +
                                         "FROM CXN_FACTURA " +
                                         "WHERE Homologo = @param1 " +
                                         "AND Fac_Cia = @param2";                    

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", DocumentoElectronico);
                        Carga_Command.Parameters.AddWithValue("@param2", Compañia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {

                                return Convert.ToInt32(Lectura_Hora["Fac_Num_Fac"]);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        public static string GetCUFE(string DocumentoElectronico, int Compañia, string Tipodocumento)
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

                    String Cargar_Hora = "";
                   
                    if (Tipodocumento == "AseguraOtras")
                    {
                        Cargar_Hora = "SELECT Cufe " +
                                      "FROM CXN_FACTURA " +
                                      "WHERE Homologo = @param1 " +
                                      "AND Fac_Cia = @param2";
                    }
                    else if (Tipodocumento == "Caja")
                    {
                        Cargar_Hora = "SELECT TOP 1 Hor_DocFEModeradorCUFE AS Cufe " +
                                      "FROM CXN_RC_CAJA " +
                                      "WHERE Hor_DocFEModerador = @param1 " +
                                      "AND Rc_Caja_Cia = @param2 " +
                                      "ORDER BY Rc_Id DESC";
                    }
                    else if (Tipodocumento == "Ventas")
                    {
                        Cargar_Hora = "SELECT Cufe " +
                                      "FROM CXN_VENTAS " +
                                      "WHERE Ven_Homologo = @param1 " +
                                      "AND Ven_Cod_Cia = @param2";
                    }
                    else if (Tipodocumento == "NotasCredito")
                    {
                        Cargar_Hora = "SELECT Cufe " +
                                      "FROM CXN_FACTURANC " +
                                      "WHERE FacturaElectronica = @param1 " +
                                      "AND Prestador = @param2";
                    }
                    else
                    {
                        return "";
                    }

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                        {
                            Carga_Command.Parameters.AddWithValue("@param1", DocumentoElectronico);
                            Carga_Command.Parameters.AddWithValue("@param2", Compañia);

                            using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                            {
                                if (Lectura_Hora.Read() == true)
                                {

                                    return Lectura_Hora["Cufe"].ToString();
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
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public static bool CleanInvoice(string FacElectron, int Compañia, string TipoFactura, int? FacZamenis)
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

                    SqlCommand Busqueda = null;

                    if (TipoFactura == "Aseguradoras")
                    {
                        Busqueda = new SqlCommand(@"UPDATE CXN_FACTURA " +
                                                   "SET  " +
                                                   "Homologo = @FacZam, " +
                                                   "Cufe = @param1, " +
                                                   "FacResNumeracion = @param1 " +
                                                   "WHERE Homologo = @param3 " +
                                                   "AND Fac_Cia = @param4", con);

                        Busqueda.Parameters.AddWithValue("@FacZam", FacZamenis);
                    }
                    else if (TipoFactura == "Caja")
                    {
                        Busqueda = new SqlCommand(@"UPDATE CXN_HORARIO " +
                                                   "SET  " +
                                                   "Hor_DocFEModeradorRes = @param1, " +
                                                   "Hor_DocFEModeradorNumeracion = @param1, " +
                                                   "Hor_DocFEModeradorCUFE = @param1, " +
                                                   "Hor_DocFEModerador = @param1 " +
                                                   "WHERE Hor_DocFEModerador = @param3 " +
                                                   "AND Hor_Pac_Cia = @param4", con);
                    }
                    else if (TipoFactura == "Ventas")
                    {
                        Busqueda = new SqlCommand(@"UPDATE CXN_VENTAS " +
                                                   "SET  " +
                                                   "Cufe = @param1, " +
                                                   "Ven_Homologo = @param1, " +
                                                   "Ven_Res = @param1 " +
                                                   "WHERE Ven_Homologo = @param3 " +
                                                   "AND Ven_Cod_Cia = @param4", con);
                    }
                    else
                    {
                        return false;
                    }
                    
                    Busqueda.Parameters.AddWithValue("@param1", "");
                    Busqueda.Parameters.AddWithValue("@param3", FacElectron);
                    Busqueda.Parameters.AddWithValue("@param4", Compañia);

                    int d = Busqueda.ExecuteNonQuery();
                    if (d <= 0)
                    {
                        TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "ERROR LIMPIANDO FACTURAS", Formulario = "AcualizarDocumentos", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "AcualizarDocumentos", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
    }
}
