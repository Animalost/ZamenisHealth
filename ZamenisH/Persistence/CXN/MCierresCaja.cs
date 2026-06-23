using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Persistence.CXN
{
    public class MCierresCaja : ICIerresCaja
    {
        (Dictionary<string, int> Ventas, Dictionary<string, int> Caja, Dictionary<string, int> Particulares) ICIerresCaja.getIngresos(int Code, DateTime Desde, DateTime Hasta)
        {
            try
            {
                var DVentas = Ventas(Code, Desde, Hasta);
                var DCaja = Caja(Code, Desde, Hasta);
                var DParticulares = Particulares(Code, Desde, Hasta);

                return (DVentas, DCaja, DParticulares);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (null, null, null);
            }            
        }
        Dictionary<string, int> Particulares(int Cia, DateTime Desde, DateTime Hasta)
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

                    String Query = "SELECT SUM(C.Car_Val_Tot) AS Total, F.FormaPago " +
                                   "FROM CXN_FACTURA F " +
                                   "INNER JOIN CXN_CARGOS C ON F.Fac_Num_Fac = C.Car_Factura " +
                                   "WHERE F.Fac_Fecha BETWEEN @desde AND @hasta " +
                                   "AND F.Fac_Cia = @cia " +
                                   "AND F.Fac_Estado = 'F' " +
                                   "AND F.Fac_Ase = '99' " +
                                   "AND C.Car_Ase = '99' " +
                                   "AND F.CUFE IS NOT NULL " +
                                   "AND F.Num_Cruce = '0' " +
                                   "GROUP BY F.FormaPago";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@desde", Convert.ToDateTime(Desde));
                        Commando.Parameters.AddWithValue("@hasta", Convert.ToDateTime(Hasta));
                        Commando.Parameters.AddWithValue("@cia", Cia);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                Dictionary<string, int> L = new Dictionary<string, int>();

                                while (Reader.Read() == true)
                                {
                                    L.Add(Reader["FormaPago"].ToString(), Convert.ToInt32(Reader["Total"]));
                                }

                                L.Add("Total", L.Sum(x => x.Value));

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
        Dictionary<string, int> Caja(int Cia, DateTime Desde, DateTime Hasta)
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

                    String Query = "SELECT SUM(CAST(Rc_Caja_Valor AS INT)) AS Total, FormaPago " +
                                   "FROM CXN_RC_CAJA " +
                                   "WHERE Rc_Caja_Fecha BETWEEN @desde AND @hasta " +
                                   "AND Rc_Caja_Cia = @cia " +
                                   "AND Hor_DocFEModeradorCufe IS NOT NULL " +
                                   "AND Num_Cruce = '0' " +
                                   "GROUP BY FormaPago";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@desde", Convert.ToDateTime(Desde));
                        Commando.Parameters.AddWithValue("@hasta", Convert.ToDateTime(Hasta));
                        Commando.Parameters.AddWithValue("@cia", Cia);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                Dictionary<string, int> L = new Dictionary<string, int>();

                                while (Reader.Read() == true)
                                {
                                    L.Add(Reader["FormaPago"].ToString(), Convert.ToInt32(Reader["Total"]));
                                }

                                L.Add("Total", L.Sum(x => x.Value));

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
        Dictionary<string, int> Ventas(int Cia, DateTime Desde, DateTime Hasta)
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

                    String Query = "SELECT SUM(Ven_Total) AS Total, FormaPago " +
                                   "FROM CXN_VENTAS " +
                                   "WHERE Ven_Fecha BETWEEN @desde AND @hasta " +
                                   "AND Ven_Cod_Cia = @cia " +
                                   "AND Ven_Estado = 'F' " +
                                   "AND Cufe IS NOT NULL " +
                                   "AND Num_Cruce = '0' " +
                                   "GROUP BY FormaPago";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@desde", Convert.ToDateTime(Desde));
                        Commando.Parameters.AddWithValue("@hasta", Convert.ToDateTime(Hasta));
                        Commando.Parameters.AddWithValue("@cia", Cia);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                Dictionary<string, int> L = new Dictionary<string, int>();

                                while (Reader.Read() == true)
                                {
                                    L.Add(Reader["FormaPago"].ToString(), Convert.ToInt32(Reader["Total"]));
                                }

                                L.Add("Total", L.Sum(x => x.Value));

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
        void ICIerresCaja.ActualizarNumCruce(int NumCruce, int Cia, DateTime Desde, DateTime Hasta)
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

                    //PARTICULARES
                    string Busqueda = "UPDATE CXN_FACTURA " +
                                      "SET Num_Cruce = @param1 " +
                                      "WHERE Fac_Fecha BETWEEN @desde AND @hasta " +
                                      "AND Fac_Cia = @cia " +
                                      "AND Fac_Estado = 'F' " +
                                      "AND Fac_Ase = '99' " +
                                      "AND CUFE IS NOT NULL " +
                                      "AND Num_Cruce = '0'";

                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@param1", NumCruce);
                    Accion.Parameters.AddWithValue("@cia", Cia);
                    Accion.Parameters.AddWithValue("@desde", Convert.ToDateTime(Desde));
                    Accion.Parameters.AddWithValue("@hasta", Convert.ToDateTime(Hasta));
                    Accion.ExecuteNonQuery();

                    //CAJA
                    string Busqueda2 = "UPDATE CXN_RC_CAJA " +
                                       "SET Num_Cruce = @param1 " +
                                       "WHERE Rc_Caja_Fecha BETWEEN @desde AND @hasta " +
                                       "AND Rc_Caja_Cia = @cia " +
                                       "AND Hor_DocFEModeradorCufe IS NOT NULL " +
                                       "AND Num_Cruce = '0'";

                    SqlCommand Accion2 = new SqlCommand(Busqueda2, con);
                    Accion2.Parameters.AddWithValue("@param1", NumCruce);
                    Accion2.Parameters.AddWithValue("@cia", Cia);
                    Accion2.Parameters.AddWithValue("@desde", Convert.ToDateTime(Desde));
                    Accion2.Parameters.AddWithValue("@hasta", Convert.ToDateTime(Hasta));
                    Accion2.ExecuteNonQuery();

                    //VENTAS
                    string Busqueda3 = "UPDATE CXN_VENTAS " +
                                       "SET Num_Cruce = @param1 " +
                                       "WHERE Ven_Fecha BETWEEN @desde AND @hasta " +
                                       "AND Ven_Cod_Cia = @cia " +
                                       "AND Ven_Estado = 'F' " +
                                       "AND Cufe IS NOT NULL " +
                                       "AND Num_Cruce = '0'";

                    SqlCommand Accion3 = new SqlCommand(Busqueda3, con);
                    Accion3.Parameters.AddWithValue("@param1", NumCruce);
                    Accion3.Parameters.AddWithValue("@cia", Cia);
                    Accion3.Parameters.AddWithValue("@desde", Convert.ToDateTime(Desde));
                    Accion3.Parameters.AddWithValue("@hasta", Convert.ToDateTime(Hasta));
                    Accion3.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        int ICIerresCaja.GrabarReporte2(CXN_REPORTECAJA2 C)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_REPORTECAJA2 (Consecutivo, " + //1
                                                                  "Usuario, " +//2
                                                                  "Desde, " +//3
                                                                  "Hasta, " +//4
                                                                  "Generacion, " +//5
                                                                  "Tipo, " +//6
                                                                  "Clase, " +//7
                                                                  "Valor, " +//8
                                                                  "Estado, " +
                                                                  "Compañia, " +
                                                                  "Observacion) " +//63
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
                                                                  "@param11)", con);

                    cmd.Parameters.AddWithValue("@param1", C.Consecutivo);
                    cmd.Parameters.AddWithValue("@param2", C.Usuario);
                    cmd.Parameters.AddWithValue("@param3", Convert.ToDateTime(C.Desde));
                    cmd.Parameters.AddWithValue("@param4", Convert.ToDateTime(C.Hasta));
                    cmd.Parameters.AddWithValue("@param5", Convert.ToDateTime(C.Generacion));
                    cmd.Parameters.AddWithValue("@param6", C.Tipo);
                    cmd.Parameters.AddWithValue("@param7", C.Clase);
                    cmd.Parameters.AddWithValue("@param8", C.Valor);
                    cmd.Parameters.AddWithValue("@param9", C.Estado);
                    cmd.Parameters.AddWithValue("@param10", C.Compañia);
                    cmd.Parameters.AddWithValue("@param11", C.Observacion);

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }
        List<CXN_REPORTECAJA2> ICIerresCaja.GetReport2(string consecutivo, int Cia)
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

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_REPORTECAJA2 " +
                                         "WHERE Consecutivo = @param1 " +
                                         "AND Compañia = @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", consecutivo);
                        Carga_Command.Parameters.AddWithValue("@param2", Cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_REPORTECAJA2> C = new List<CXN_REPORTECAJA2>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_REPORTECAJA2
                                    {
                                        Consecutivo = Convert.ToInt32(Lectura_Hora["Consecutivo"]),
                                        IdRC = Convert.ToInt32(Lectura_Hora["IdRC"]),
                                        Desde = Convert.ToDateTime(Lectura_Hora["Desde"]),
                                        Hasta = Convert.ToDateTime(Lectura_Hora["Hasta"]),
                                        Observacion = Lectura_Hora["Observacion"].ToString(),
                                        Usuario = Lectura_Hora["Usuario"].ToString(),
                                        Clase = Lectura_Hora["Clase"].ToString(),
                                        Estado = Lectura_Hora["Estado"].ToString(),
                                        Generacion = Convert.ToDateTime(Lectura_Hora["Generacion"]),
                                        Tipo = Lectura_Hora["Tipo"].ToString(),
                                        Valor = Convert.ToInt32(Lectura_Hora["Valor"]),
                                        Compañia = Convert.ToInt32(Lectura_Hora["Compañia"])
                                    });
                                }

                                return C;
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
        List<CXN_REPORTECAJA2> ICIerresCaja.GetPrevios(int Cia, DateTime Desde, DateTime Hasta)
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

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_REPORTECAJA2 " +
                                         "WHERE Compañia = @param1 " +
                                         "AND Generacion BETWEEN @param2 AND @param3 " +
                                         "AND Tipo <> @param4";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Cia);
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(Desde.Date));
                        Carga_Command.Parameters.AddWithValue("@param3", Convert.ToDateTime(Hasta.Date));
                        Carga_Command.Parameters.AddWithValue("@param4", "EGRESOS");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_REPORTECAJA2> C = new List<CXN_REPORTECAJA2>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_REPORTECAJA2
                                    {
                                        Consecutivo = Convert.ToInt32(Lectura_Hora["Consecutivo"]),
                                        Usuario = Lectura_Hora["Usuario"].ToString(),
                                        Valor = Convert.ToInt32(Lectura_Hora["Valor"]),
                                        Generacion = Convert.ToDateTime(Lectura_Hora["Generacion"]),
                                        Compañia = Convert.ToInt32(Lectura_Hora["Compañia"]),
                                        Estado = Lectura_Hora["Estado"].ToString()
                                    });
                                }

                                return C;
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
    }
}
