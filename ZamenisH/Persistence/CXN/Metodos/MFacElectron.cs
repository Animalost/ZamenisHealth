using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Persistence.CXN.Metodos
{
    public class MFacElectron : IFacElectron
    {
        List<CXN_MEDIOSPAGO> IFacElectron.ListaMediosPago()
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
                                         "FROM CXN_MEDIOSPAGO " +
                                         "ORDER BY Codigo ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_MEDIOSPAGO> L = new List<CXN_MEDIOSPAGO>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new CXN_MEDIOSPAGO
                                    {
                                        Codigo = Convert.ToInt32(Lectura_Hora["Codigo"]),
                                        Medio = Lectura_Hora["Medio"].ToString(),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"])
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
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }        
        string IFacElectron.insertToken(string Token, int Prestador)
        {
            try
            {
                var datCone = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now.Date;

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO CXN_TOKENS " +
                                                     "(Token, " +
                                                      "Fecha, " +
                                                      "Hora, " +
                                                      "CodePrestador, " +
                                                      "FechaHora) " +
                             "values                  (@param1, " +
                                                      "@param2, " +
                                                      "@param3, " +
                                                      "@param4, " +
                                                      "@param5)", con);

                    cmd.Parameters.AddWithValue("@param1", Token);
                    cmd.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = DateTime.Now.Date;
                    cmd.Parameters.Add(new SqlParameter("@param3", SqlDbType.DateTime)).Value = DateTime.Now;
                    cmd.Parameters.AddWithValue("@param4", Prestador);
                    cmd.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = DateTime.Now.AddDays(1);
                    int g = cmd.ExecuteNonQuery();
                    if (g == 0)
                    {
                        throw new Exception("No se pudo grabar el token en la base de datos.");
                    }

                    return "OK";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        string IFacElectron.GetTokenSaved(int Cia)
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

                    String Cargar_Hora = "SELECT TOP 1 * " +
                                         "FROM CXN_TOKENS " +
                                         "WHERE CodePrestador = @param1 " +
                                         "ORDER BY Id DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                if (Convert.ToDateTime(DateTime.Now) > Convert.ToDateTime(Lectura_Hora["FechaHora"]))
                                {
                                    return "";
                                }

                                return Lectura_Hora["Token"].ToString();
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
        string IFacElectron.insertNC(CXN_FACTURANC F)
        {
            try
            {
                var datCone = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now.Date;

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO CXN_FACTURANC " +
                                                     "(FacturaElectronica, " +
                                                      "OrdenPedido, " +
                                                      "FechaNC, " +
                                                      "HoraNC, " +
                                                      "Cufe, " +
                                                      "Resolucion, " +
                                                      "NumeroNC, " +
                                                      "Usuario, " +
                                                      "Prestador) " +
                             "values                  (@param1, " +
                                                      "@param2, " +
                                                      "@param3, " +
                                                      "@param4, " +
                                                      "@param5, " +
                                                      "@param6, " +
                                                      "@param7, " +
                                                      "@param8, " +
                                                      "@param9)", con);

                    cmd.Parameters.AddWithValue("@param1", F.FacturaElectronica);
                    cmd.Parameters.AddWithValue("@param2", F.OrdenPedido);
                    cmd.Parameters.Add(new SqlParameter("@param3", SqlDbType.DateTime)).Value = DateTime.Now.Date;
                    cmd.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = DateTime.Now;
                    cmd.Parameters.AddWithValue("@param5", F.Cufe);
                    cmd.Parameters.AddWithValue("@param6", F.Resolucion);
                    cmd.Parameters.AddWithValue("@param7", F.NumeroNC);
                    cmd.Parameters.AddWithValue("@param8", F.Usuario);
                    cmd.Parameters.AddWithValue("@param9", F.Prestador);

                    int g = cmd.ExecuteNonQuery();
                    if (g == 0)
                    {
                        throw new Exception("NO");
                    }

                    insertarCargoNC(F);
                    return "OK";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        void insertarCargoNC(CXN_FACTURANC F)
        {
            try
            {
                var datCone = Conexion.Conection();               

                using (SqlConnection con = new SqlConnection(datCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Nnc = F.NumeroNC;

                    foreach (CXN_CARGOSNC n in F.listaCargos)
                    {
                        SqlCommand cmd = new SqlCommand(@"INSERT INTO CXN_CARGOSNC " +
                                                     "(Codigo, " +
                                                      "Item, " +
                                                      "Cantidad, " +
                                                      "VrUnitario, " +
                                                      "VrTotal, " +
                                                      "NumeroNC, " +
                                                      "Tipo) " +
                             "values                  (@param1, " +
                                                      "@param2, " +
                                                      "@param3, " +
                                                      "@param4, " +
                                                      "@param5, " +
                                                      "@param6, " +
                                                      "@param7)", con);

                        cmd.Parameters.AddWithValue("@param1", n.Codigo);
                        cmd.Parameters.AddWithValue("@param2", n.Item);
                        cmd.Parameters.AddWithValue("@param3", n.Cantidad);
                        cmd.Parameters.AddWithValue("@param4", n.VrUnitario);
                        cmd.Parameters.AddWithValue("@param5", n.VrTotal);
                        cmd.Parameters.AddWithValue("@param6", Nnc);
                        cmd.Parameters.AddWithValue("@param7", n.Tipo);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        async Task<DataTable> IFacElectron.getReportContableCompleto(int compania,
                                                         string estado,
                                                         DateTime desde,
                                                         DateTime hasta)
        {
            try
            {
                var datCone = Conexion.Conection();

                DataTable dt = new DataTable();

                using (SqlConnection cn = new SqlConnection(datCone["Conexion"]))
                using (SqlCommand cmd = new SqlCommand("dbo.sp_ReporteContable", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Compañia", SqlDbType.Int).Value = compania;
                    cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 1).Value = estado;
                    cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
                    cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = hasta;

                    await cn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.GetType().Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = "BackEnd"
                };

                OverridesExtern.GenerarTXTException(T);

                return null;
            }
        }
        List<CXN_FACTURANC> IFacElectron.GetNotasCredito(string Tipo, DateTime Desde, DateTime Hasta, int Prestador)
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

                    String Cargar_Hora = "SELECT F.FacturaElectronica, F.NumeroNC, F.FechaNC, F.OrdenPedido " +
                                         "FROM CXN_FACTURANC F " +
                                         "INNER JOIN CXN_CARGOSNC C ON F.NumeroNC = C.NumeroNC " +
                                         "WHERE F.Prestador = @param1 " +
                                         "AND C.Tipo = @param2 " +
                                         "AND F.FechaNC BETWEEN @param3 AND @param4 " +
                                         "GROUP BY F.FacturaElectronica, F.NumeroNC, F.FechaNC, F.OrdenPedido " +
                                         "ORDER BY F.NumeroNC ASC";                  

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Prestador);
                        Carga_Command.Parameters.AddWithValue("@param2", Tipo);
                        Carga_Command.Parameters.AddWithValue("@param3", Convert.ToDateTime(Desde.Date));
                        Carga_Command.Parameters.AddWithValue("@param4", Convert.ToDateTime(Hasta.Date));

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_FACTURANC> L = new List<CXN_FACTURANC>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new CXN_FACTURANC
                                    {
                                        FacturaElectronica = Lectura_Hora["FacturaElectronica"].ToString(),
                                        NumeroNC = Lectura_Hora["NumeroNC"].ToString(),
                                        FechaNC = Convert.ToDateTime(Lectura_Hora["FechaNC"].ToString()),
                                        OrdenPedido = Convert.ToInt32(Lectura_Hora["OrdenPedido"])
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
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        int IFacElectron.GetFacZam(int Cia, string Homologo, string Tipo)
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

                    if (Tipo == "Salud")
                    {
                        Cargar_Hora = "SELECT Fac_Num_Fac AS DocZam  " +
                                      "FROM CXN_FACTURA " +
                                      "WHERE Fac_Cia = @param1 " +
                                      "AND Homologo = @param2";
                    }
                    else if (Tipo == "Caja")
                    {
                        Cargar_Hora = "SELECT TOP 1 Rc_Id AS DocZam " +
                                      "FROM CXN_RC_CAJA " +
                                      "WHERE Rc_Caja_Cia = @param1 " +
                                      "AND Hor_DocFEModerador = @param2 " +
                                      "ORDER BY Rc_Id DESC";
                    }
                    else if (Tipo == "Ventas")
                    {
                        Cargar_Hora = "SELECT Ven_Factura AS DocZam " +
                                      "FROM CXN_VENTAS " +
                                      "WHERE Ven_Cod_Cia = @param1 " +
                                      "AND Ven_Homologo = @param2";
                    }
                    else if (Tipo == "NC")
                    {
                        Cargar_Hora = "SELECT OrdenPedido AS DocZam " +
                                      "FROM CXN_FACTURANC " +
                                      "WHERE Prestador = @param1 " +
                                      "AND NumeroNC = @param2";
                    }
                    else
                    {
                        return 0;
                    }

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Cia);
                        Carga_Command.Parameters.AddWithValue("@param2", Homologo);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["DocZam"]);
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }
        CXN_FACTURA IFacElectron.GetFacZam(int Cia, int docZam)
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

                    String Cargar_Hora =  "SELECT * " +
                                          "FROM CXN_FACTURA " +
                                          "WHERE Fac_Cia = @param1 " +
                                          "AND Fac_Num_Fac = @param2";                 

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Cia);
                        Carga_Command.Parameters.AddWithValue("@param2", docZam);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_FACTURA F;

                                if (string.IsNullOrEmpty(Lectura_Hora["Hora"].ToString()))
                                {
                                    DateTime D = new DateTime(2000, 01, 01);

                                    F = new CXN_FACTURA
                                    {
                                        Fac_Num_Fac = Convert.ToInt32(Lectura_Hora["Fac_Num_Fac"]),
                                        Homologo = string.IsNullOrEmpty(Lectura_Hora["Homologo"].ToString()) ? "" : Lectura_Hora["Homologo"].ToString(),
                                        Cufe = string.IsNullOrEmpty(Lectura_Hora["Cufe"].ToString()) ? "" : Lectura_Hora["Cufe"].ToString(),
                                        Hora = Convert.ToDateTime(D),
                                        Fac_Res = string.IsNullOrEmpty(Lectura_Hora["Fac_Res"].ToString()) ? "" : Lectura_Hora["Fac_Res"].ToString(),
                                        Fac_Cia = Convert.ToInt32(Lectura_Hora["Fac_Cia"])
                                    };
                                }
                                else
                                {
                                    F = new CXN_FACTURA
                                    {
                                        Fac_Num_Fac = Convert.ToInt32(Lectura_Hora["Fac_Num_Fac"]),
                                        Homologo = string.IsNullOrEmpty(Lectura_Hora["Homologo"].ToString()) ? "" : Lectura_Hora["Homologo"].ToString(),
                                        Cufe = string.IsNullOrEmpty(Lectura_Hora["Cufe"].ToString()) ? "" : Lectura_Hora["Cufe"].ToString(),
                                        Hora = Convert.ToDateTime(Lectura_Hora["Hora"]),
                                        Fac_Res = string.IsNullOrEmpty(Lectura_Hora["Fac_Res"].ToString()) ? "" : Lectura_Hora["Fac_Res"].ToString(),
                                        Fac_Cia = Convert.ToInt32(Lectura_Hora["Fac_Cia"])
                                    };
                                }                             

                                return F;
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
        bool IFacElectron.UpdateDatosDIAN(CXN_FACTURA F)
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

                    string Busqueda = "UPDATE CXN_FACTURA " +
                                      "SET Homologo = '" + F.Homologo + "', " +
                                      "CUFE = '" + F.Cufe + "', " +
                                      "Hora = '" + Convert.ToDateTime(F.Hora) + "', " +
                                      "Fac_Res = '" + F.Fac_Res + "' " +
                                      "WHERE Fac_Num_Fac = '" + F.Fac_Num_Fac + "' " +
                                      "AND Fac_Cia = '" + F.Fac_Cia + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    if (Guarda > 0) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        string IFacElectron.GetHomologo(int Cia, int docZam)
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

                    String Cargar_Hora = "SELECT Homologo " +
                                          "FROM CXN_FACTURA " +
                                          "WHERE Fac_Cia = @param1 " +
                                          "AND Fac_Num_Fac = @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Cia);
                        Carga_Command.Parameters.AddWithValue("@param2", docZam);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Lectura_Hora["Homologo"].ToString();
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
        string IFacElectron.GetHomologoRcCaja(int Cia, int docZam)
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

                    String Cargar_Hora = "SELECT Hor_DocFEModerador " +
                                          "FROM CXN_HORARIO " +
                                          "WHERE Hor_Pac_Cia = @param1 " +
                                          "AND Hor_Id = @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Cia);
                        Carga_Command.Parameters.AddWithValue("@param2", docZam);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Lectura_Hora["Hor_DocFEModerador"].ToString();
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
        string IFacElectron.GetHomologoVentas(int Cia, int docZam)
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
                                          "FROM CXN_VENTASO " +
                                          "WHERE Ven_Cod_Cia = @param1 " +
                                          "AND Ven_Factura = @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Cia);
                        Carga_Command.Parameters.AddWithValue("@param2", docZam);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Lectura_Hora["Ven_Homologo"].ToString();
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
    public class UpdateFacturaElectronica
    {
        public string FacturaElectronica { get; set; }
        public int FacturaZamenis { get; set; }
        public int Prestador { get; set; }
        public DateTime Hora { get; set; }
        public DateTime Fecha { get; set; }
        public string Cufe { get; set; }
        public string Resolucion { get; set; }
        public string ResolucionNumeracion { get; set; }
    }
}
