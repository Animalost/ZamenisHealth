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
        Dictionary<string, int> ICIerresCaja.getIngresos(int Code, DateTime Desde, DateTime Hasta)
        {
            try
            {
                var DVentas = Ventas(Code, Desde, Hasta);
                var DCaja = Caja(Code, Desde, Hasta);
                var DParticulares = Particulares(Code, Desde, Hasta);

                return new[] { DVentas, DCaja, DParticulares }
    .Where(d => d != null) // Filtra los que no son null
    .SelectMany(d => d)    // Aplana todos los diccionarios válidos
    .GroupBy(kvp => kvp.Key)
    .ToDictionary(g => g.Key, g => g.Sum(kvp => kvp.Value));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
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

        void ICIerresCaja.GrabarReporte(CXN_REPORTECAJA C)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_REPORTECAJA (Consecutivo, " + //1
                                                                  "Cia, " +//2
                                                                  "Desde, " +//3
                                                                  "Hasta, " +//4
                                                                  "Observacion, " +//5
                                                                  "Usuario, " +//6
                                                                  "BCincuenta, " +//7
                                                                  "BCincuentaCantidad, " +//8
                                                                  "BCincuentaValor, " +//9
                                                                  "BVeinte, " +//10
                                                                  "BVeinteCantidad, " +//11
                                                                  "BVeinteValor, " +
                                                                  "BDiez, " +//12
                                                                  "BDiezCantidad, " +//13
                                                                  "BDiezValor, " +//14
                                                                  "BCinco, " +//15
                                                                  "BCincoCantidad, " +//16
                                                                  "BCincoValor, " +//17
                                                                  "BDosMil, " +//18
                                                                  "BDosMilCantidad, " +//19
                                                                  "BDosMilValor, " +//20
                                                                  "BMil, " +//21
                                                                  "BMilCantidad, " +//22
                                                                  "BMilValor, " +//23
                                                                  "MMil, " +//24
                                                                  "MMilCantidad, " +//25
                                                                  "MMilValor, " +//26
                                                                  "MQuinientos, " +//27
                                                                  "MQuinientosCantidad, " +//28
                                                                  "MQuinientosValor, " +//29
                                                                  "MDoscientos, " +//30
                                                                  "MDoscientosCantidad, " +//31
                                                                  "MDoscientosValor, " +//32
                                                                  "MCien, " +//33
                                                                  "MCienCantidad, " +//34
                                                                  "MCienValor, " +//35
                                                                  "MCincuenta, " +//36
                                                                  "MCincuentaCantidad, " +//37
                                                                  "MCincuentaValor, " +//38
                                                                  "EgresoRazon1, " +//39
                                                                  "EgresoValor1, " +//40
                                                                  "EgresoRazon2, " +//41
                                                                  "EgresoValor2, " +//42
                                                                  "EgresoRazon3, " +//43
                                                                  "EgresoValor3, " +//44
                                                                  "EgresoRazon4, " +//45
                                                                  "EgresoValor4, " +//46
                                                                  "EgresoRazon5, " +//47
                                                                  "EgresoValor5, " +//48
                                                                  "BCien, " +//49
                                                                  "BCienCantidad, " +//50
                                                                  "BCienValor, " +//51
                                                                  "TipoPagoEfectivo, " +//52
                                                                  "TipoPagoEfectivoValor, " +//53
                                                                  "TipoPagoTC, " +//54
                                                                  "TipoPagoTCValor, " +//55
                                                                  "TipoPagoDB, " +//56
                                                                  "TipoPagoDBValor, " +//57
                                                                  "TipoPagoNequi, " +//58
                                                                  "TipoPagoNequiValor, " +//59
                                                                  "TipoPagoDaviplata, " +//60
                                                                  "TipoPagoDaviplataValor, " +//61
                                                                  "TipoPagoOtraBilletera, " +//62
                                                                  "TipoPagoOtraBilleteraValor, " +
                                                                  "FechaGeneracion) " +//63
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
                                                                  "@param28, " +
                                                                  "@param29, " +
                                                                  "@param30, " +
                                                                  "@param31, " +
                                                                  "@param32, " +
                                                                  "@param33, " +
                                                                  "@param34, " +
                                                                  "@param35, " +
                                                                  "@param36, " +
                                                                  "@param37, " +
                                                                  "@param38, " +
                                                                  "@param39, " +
                                                                  "@param40, " +
                                                                  "@param41, " +
                                                                  "@param42, " +
                                                                  "@param43, " +
                                                                  "@param44, " +
                                                                  "@param45, " +
                                                                  "@param46, " +
                                                                  "@param47, " +
                                                                  "@param48, " +
                                                                  "@param49, " +
                                                                  "@param50, " +
                                                                  "@param51, " +
                                                                  "@param52, " +
                                                                  "@param53, " +
                                                                  "@param54, " +
                                                                  "@param55, " +
                                                                  "@param56, " +
                                                                  "@param57, " +
                                                                  "@param58, " +
                                                                  "@param59, " +
                                                                  "@param60, " +
                                                                  "@param61, " +
                                                                  "@param62, " +
                                                                  "@param63, " +
                                                                  "@param64, " +
                                                                  "@param65)", con);

                    cmd.Parameters.AddWithValue("@param1", C.Consecutivo);
                    cmd.Parameters.AddWithValue("@param2", C.Cia);
                    cmd.Parameters.AddWithValue("@param3", Convert.ToDateTime(C.Desde));
                    cmd.Parameters.AddWithValue("@param4", Convert.ToDateTime(C.Hasta));
                    cmd.Parameters.AddWithValue("@param5", C.Observacion);
                    cmd.Parameters.AddWithValue("@param6", C.Usuario);
                    cmd.Parameters.AddWithValue("@param7", C.BCincuenta);
                    cmd.Parameters.AddWithValue("@param8", C.BCincuentaCantidad);
                    cmd.Parameters.AddWithValue("@param9", C.BCincuentaValor);
                    cmd.Parameters.AddWithValue("@param10", C.BVeinte);
                    cmd.Parameters.AddWithValue("@param11", C.BVeinteCantidad);
                    cmd.Parameters.AddWithValue("@param12", C.BVeinteValor);
                    cmd.Parameters.AddWithValue("@param13", C.BDiez);
                    cmd.Parameters.AddWithValue("@param14", C.BDiezCantidad);
                    cmd.Parameters.AddWithValue("@param15", C.BDiezValor);
                    cmd.Parameters.AddWithValue("@param16", C.BCinco);
                    cmd.Parameters.AddWithValue("@param17", C.BCincoCantidad);
                    cmd.Parameters.AddWithValue("@param18", C.BCincoValor);
                    cmd.Parameters.AddWithValue("@param19", C.BDosMil);
                    cmd.Parameters.AddWithValue("@param20", C.BDosMilCantidad);
                    cmd.Parameters.AddWithValue("@param21", C.BDosMilValor);
                    cmd.Parameters.AddWithValue("@param22", C.BMil);
                    cmd.Parameters.AddWithValue("@param23", C.BMilCantidad);
                    cmd.Parameters.AddWithValue("@param24", C.BMilValor);
                    cmd.Parameters.AddWithValue("@param25", C.MMil);
                    cmd.Parameters.AddWithValue("@param26", C.MMilCantidad);
                    cmd.Parameters.AddWithValue("@param27", C.MMilValor);
                    cmd.Parameters.AddWithValue("@param28", C.MQuinientos);
                    cmd.Parameters.AddWithValue("@param29", C.MQuinientosCantidad);
                    cmd.Parameters.AddWithValue("@param30", C.MQuinientosValor);
                    cmd.Parameters.AddWithValue("@param31", C.MDoscientos);
                    cmd.Parameters.AddWithValue("@param32", C.MDoscientosCantidad);
                    cmd.Parameters.AddWithValue("@param33", C.MDoscientosValor);
                    cmd.Parameters.AddWithValue("@param34", C.MCien);
                    cmd.Parameters.AddWithValue("@param35", C.MCienCantidad);
                    cmd.Parameters.AddWithValue("@param36", C.MCienValor);
                    cmd.Parameters.AddWithValue("@param37", C.MCincuenta);
                    cmd.Parameters.AddWithValue("@param38", C.MCincuentaCantidad);
                    cmd.Parameters.AddWithValue("@param39", C.MCincuentaValor);
                    cmd.Parameters.AddWithValue("@param40", C.EgresoRazon1);
                    cmd.Parameters.AddWithValue("@param41", C.EgresoValor1);
                    cmd.Parameters.AddWithValue("@param42", C.EgresoRazon2);
                    cmd.Parameters.AddWithValue("@param43", C.EgresoValor2);
                    cmd.Parameters.AddWithValue("@param44", C.EgresoRazon3);
                    cmd.Parameters.AddWithValue("@param45", C.EgresoValor3);
                    cmd.Parameters.AddWithValue("@param46", C.EgresoRazon4);
                    cmd.Parameters.AddWithValue("@param47", C.EgresoValor4);
                    cmd.Parameters.AddWithValue("@param48", C.EgresoRazon5);
                    cmd.Parameters.AddWithValue("@param49", C.EgresoValor5);
                    cmd.Parameters.AddWithValue("@param50", C.BCien);
                    cmd.Parameters.AddWithValue("@param51", C.BCienCantidad);
                    cmd.Parameters.AddWithValue("@param52", C.BCienValor);
                    cmd.Parameters.AddWithValue("@param53", C.TipoPagoEfectivo);
                    cmd.Parameters.AddWithValue("@param54", C.TipoPagoEfectivoValor);
                    cmd.Parameters.AddWithValue("@param55", C.TipoPagoTC);
                    cmd.Parameters.AddWithValue("@param56", C.TipoPagoTCValor);
                    cmd.Parameters.AddWithValue("@param57", C.TipoPagoDB);
                    cmd.Parameters.AddWithValue("@param58", C.TipoPagoDBValor);
                    cmd.Parameters.AddWithValue("@param59", C.TipoPagoNequi);
                    cmd.Parameters.AddWithValue("@param60", C.TipoPagoNequiValor);
                    cmd.Parameters.AddWithValue("@param61", C.TipoPagoDaviplata);
                    cmd.Parameters.AddWithValue("@param62", C.TipoPagoDaviplataValor);
                    cmd.Parameters.AddWithValue("@param63", C.TipoPagoOtraBilletera);
                    cmd.Parameters.AddWithValue("@param64", C.TipoPagoOtraBilleteraValor);
                    cmd.Parameters.AddWithValue("@param65", DateTime.Now.Date);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        List<CXN_REPORTECAJA> ICIerresCaja.GetReport(string consecutivo)
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

                    String Cargar_Hora = "SELECT * FROM CXN_REPORTECAJA WHERE Consecutivo = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", consecutivo);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {                               
                                List<CXN_REPORTECAJA> C = new List<CXN_REPORTECAJA>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_REPORTECAJA
                                    {
                                        Consecutivo = Lectura_Hora["Consecutivo"].ToString(),
                                        Cia = Convert.ToInt32(Lectura_Hora["Cia"]),
                                        Desde = Convert.ToDateTime(Lectura_Hora["Desde"]),
                                        Hasta = Convert.ToDateTime(Lectura_Hora["Hasta"]),
                                        Observacion = Lectura_Hora["Observacion"].ToString(),
                                        Usuario = Lectura_Hora["Usuario"].ToString(),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        BCincuenta = Convert.ToInt32(Lectura_Hora["BCincuenta"]),
                                        BCincuentaCantidad = Convert.ToInt32(Lectura_Hora["BCincoCantidad"]),
                                        BCincuentaValor = Convert.ToInt32(Lectura_Hora["BCincuentaValor"]),
                                        BVeinte = Convert.ToInt32(Lectura_Hora["BVeinte"]),
                                        BVeinteCantidad = Convert.ToInt32(Lectura_Hora["BVeinteCantidad"]),
                                        BVeinteValor = Convert.ToInt32(Lectura_Hora["BVeinteValor"]),
                                        BDiez = Convert.ToInt32(Lectura_Hora["BDiez"]),
                                        BDiezCantidad = Convert.ToInt32(Lectura_Hora["BDiezCantidad"]),
                                        BDiezValor = Convert.ToInt32(Lectura_Hora["BDiezValor"]),
                                        BCinco = Convert.ToInt32(Lectura_Hora["BCinco"]),
                                        BCincoCantidad = Convert.ToInt32(Lectura_Hora["BCincoCantidad"]),
                                        BCincoValor = Convert.ToInt32(Lectura_Hora["BCincoValor"]),
                                        BDosMil = Convert.ToInt32(Lectura_Hora["BDosMil"]),
                                        BDosMilCantidad = Convert.ToInt32(Lectura_Hora["BDosMilCantidad"]),
                                        BDosMilValor = Convert.ToInt32(Lectura_Hora["BDosMilValor"]),
                                        BMil = Convert.ToInt32(Lectura_Hora["BMil"]),
                                        BMilCantidad = Convert.ToInt32(Lectura_Hora["BMilCantidad"]),
                                        BMilValor = Convert.ToInt32(Lectura_Hora["BMilValor"]),
                                        MMil = Convert.ToInt32(Lectura_Hora["MMil"]),
                                        MMilCantidad = Convert.ToInt32(Lectura_Hora["MMilCantidad"]),
                                        MMilValor = Convert.ToInt32(Lectura_Hora["MMilValor"]),
                                        MQuinientos = Convert.ToInt32(Lectura_Hora["MQuinientos"]),
                                        MQuinientosCantidad = Convert.ToInt32(Lectura_Hora["MQuinientosCantidad"]),
                                        MQuinientosValor = Convert.ToInt32(Lectura_Hora["MQuinientosValor"]),
                                        MDoscientos = Convert.ToInt32(Lectura_Hora["MDoscientos"]),
                                        MDoscientosCantidad = Convert.ToInt32(Lectura_Hora["MDoscientosCantidad"]),
                                        MDoscientosValor = Convert.ToInt32(Lectura_Hora["MDoscientosValor"]),
                                        MCien = Convert.ToInt32(Lectura_Hora["MCien"]),
                                        MCienCantidad = Convert.ToInt32(Lectura_Hora["MCienCantidad"]),
                                        MCienValor = Convert.ToInt32(Lectura_Hora["MCienValor"]),
                                        MCincuenta = Convert.ToInt32(Lectura_Hora["MCincuenta"]),
                                        MCincuentaCantidad = Convert.ToInt32(Lectura_Hora["MCincuentaCantidad"]),
                                        MCincuentaValor = Convert.ToInt32(Lectura_Hora["MCincuentaValor"]),
                                        EgresoRazon1 = Lectura_Hora["EgresoRazon1"].ToString(),
                                        EgresoValor1 = Convert.ToInt32(Lectura_Hora["EgresoValor1"]),
                                        EgresoRazon2 = Lectura_Hora["EgresoRazon2"].ToString(),
                                        EgresoValor2 = Convert.ToInt32(Lectura_Hora["EgresoValor2"]),
                                        EgresoRazon3 = Lectura_Hora["EgresoRazon3"].ToString(),
                                        EgresoValor3 = Convert.ToInt32(Lectura_Hora["EgresoValor3"]),
                                        EgresoRazon4 = Lectura_Hora["EgresoRazon4"].ToString(),
                                        EgresoValor4 = Convert.ToInt32(Lectura_Hora["EgresoValor4"]),
                                        EgresoRazon5 = Lectura_Hora["EgresoRazon5"].ToString(),
                                        EgresoValor5 = Convert.ToInt32(Lectura_Hora["EgresoValor5"]),
                                        BCien = Convert.ToInt32(Lectura_Hora["BCien"]),
                                        BCienCantidad = Convert.ToInt32(Lectura_Hora["BCienCantidad"]),
                                        BCienValor = Convert.ToInt32(Lectura_Hora["BCienValor"]),
                                        TipoPagoEfectivo = Lectura_Hora["TipoPagoEfectivo"].ToString(),
                                        TipoPagoEfectivoValor = Convert.ToInt32(Lectura_Hora["TipoPagoEfectivoValor"]),
                                        TipoPagoTC = Lectura_Hora["TipoPagoTC"].ToString(),
                                        TipoPagoTCValor = Convert.ToInt32(Lectura_Hora["TipoPagoTCValor"]),
                                        TipoPagoDB = Lectura_Hora["TipoPagoDB"].ToString(),
                                        TipoPagoDBValor = Convert.ToInt32(Lectura_Hora["TipoPagoDBValor"]),
                                        TipoPagoNequi = Lectura_Hora["TipoPagoNequi"].ToString(),
                                        TipoPagoNequiValor = Convert.ToInt32(Lectura_Hora["TipoPagoNequiValor"]),
                                        TipoPagoDaviplata = Lectura_Hora["TipoPagoDaviplata"].ToString(),
                                        TipoPagoDaviplataValor = Convert.ToInt32(Lectura_Hora["TipoPagoDaviplataValor"]),
                                        TipoPagoOtraBilletera = Lectura_Hora["TipoPagoOtraBilletera"].ToString(),
                                        TipoPagoOtraBilleteraValor = Convert.ToInt32(Lectura_Hora["TipoPagoOtraBilleteraValor"]),
                                        FechaGeneracion = Convert.ToDateTime(Lectura_Hora["FechaGeneracion"])
                                    });
                                }

                                C[0].TotalIngresos = C[0].TipoPagoEfectivoValor + C[0].TipoPagoTCValor + C[0].TipoPagoDBValor + C[0].TipoPagoNequiValor + C[0].TipoPagoDaviplataValor + C[0].TipoPagoOtraBilleteraValor;
                                C[0].TotalEgresos = C[0].EgresoValor1 + C[0].EgresoValor2 + C[0].EgresoValor3 + C[0].EgresoValor4 + C[0].EgresoValor5;
                                C[0].TotalBilletes = C[0].BCienValor + C[0].BCincuentaValor + C[0].BVeinteValor + C[0].BDiezValor + C[0].BCincoValor + C[0].BDosMilValor + C[0].BMilValor;
                                C[0].TotalMonedas = C[0].MMilValor + C[0].MQuinientosValor + C[0].MDoscientosValor + C[0].MCienValor + C[0].MCincuentaValor;
                                C[0].AEntregar = C[0].TipoPagoEfectivoValor - C[0].TotalEgresos;

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
        List<CXN_REPORTECAJA> ICIerresCaja.GetPrevios(int Cia, DateTime Desde, DateTime Hasta)
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

                    String Cargar_Hora = "SELECT * FROM CXN_REPORTECAJA WHERE Cia = @param1 AND FechaGeneracion BETWEEN @param2 AND @param3";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Cia);
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(Desde.Date) );
                        Carga_Command.Parameters.AddWithValue("@param3", Convert.ToDateTime(Hasta.Date));

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_REPORTECAJA> C = new List<CXN_REPORTECAJA>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_REPORTECAJA
                                    {
                                        Consecutivo = Lectura_Hora["Consecutivo"].ToString(),
                                        Usuario = Lectura_Hora["Usuario"].ToString(),
                                        AEntregar = Convert.ToInt32(Lectura_Hora["TipoPagoEfectivoValor"]),
                                        FechaGeneracion = Convert.ToDateTime(Lectura_Hora["FechaGeneracion"])
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
