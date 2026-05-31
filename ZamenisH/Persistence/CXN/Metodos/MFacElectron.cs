using Domain;
using Domain.Contabilidad;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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

        List<ReporteContable> IFacElectron.getReportContableNC(int Cia, DateTime Desde, DateTime Hasta, string TipoLista)
        {
            try
            {                
                return getReportContable_NC(Cia, Desde, Hasta, TipoLista);                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        List<ReporteContable> IFacElectron.getReportContable(int Cia, DateTime Desde, DateTime Hasta, string TipoLista)
        {
            try
            {
                //Facturas Aseguradoras CURACIONES
                if (TipoLista == "CURACIONES")
                {
                    return getReportContable_Facturacion(Cia, Desde, Hasta, "CURACIONES");
                }
                //Facturas Recuado de BONOS
                else if (TipoLista == "BONOS")
                {
                    return getReportContable_Bonos(Cia, Desde, Hasta);
                }
                //Facturas Recuado de VENTAS
                else if (TipoLista == "VENTAS")
                {
                    return getReportContable_Ventas(Cia, Desde, Hasta);
                }
                else if (TipoLista == "FIBROMIALGIA")
                {
                    return getReportContable_Facturacion(Cia, Desde, Hasta, "FIBROMIALGIA");
                }
                else if (TipoLista == "OTRAS")
                {
                    return getReportContable_Facturacion(Cia, Desde, Hasta, "OTRAS");
                }               
                else
                {
                    return null;
                }            
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        CXN_CARGOS IFacElectron.getDetalleCargos(int Cia, int Factura, string TipoCargo, string ClaseCargo)
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

                    if (TipoCargo == "Cargos")
                    {
                        Cargar_Hora = "SELECT SUM(C.Car_Val_Tot) AS TOTAL " +
                                         "FROM CXN_CARGOS C " +
                                         "INNER JOIN CXN_INVENTARIO I ON C.Car_Ase = I.InvConvenio " +
                                         "AND I.InvCod = C.Car_Cod " +
                                         "WHERE C.Car_Factura = @param1 " +
                                         "AND C.Car_Cia = @param2 " +
                                         "AND C.Car_Estado = @param3 " +
                                         "AND I.InvTipo = @param4";
                    }

                    if (TipoCargo == "Servicios")
                    {
                        Cargar_Hora = "SELECT SUM(C.Car_Val_Tot) AS TOTAL " +
                                         "FROM CXN_CARGOS C " +
                                         "INNER JOIN CXN_CONVENIOS CO ON C.Car_Ase = CO.Con_Aseguradora " +
                                         "AND CO.Con_Id_Serv = C.Car_Cod " +
                                         "AND CO.Con_Tipo_Serv = C.Car_tipo_Serv " +
                                         "WHERE C.Car_Factura = @param1 " +
                                         "AND C.Car_Cia = @param2 " +
                                         "AND C.Car_Estado = @param3 " +
                                         "AND C.Car_Tipo_Serv = @param4";
                    }

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Factura);
                        Carga_Command.Parameters.AddWithValue("@param2", Cia);
                        Carga_Command.Parameters.AddWithValue("@param3", "F");
                        Carga_Command.Parameters.AddWithValue("@param4", ClaseCargo);
                    
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_CARGOS c = new CXN_CARGOS
                                {
                                    Car_Val_Tot = (Lectura_Hora["TOTAL"] != DBNull.Value ? Convert.ToInt32(Lectura_Hora["TOTAL"]) : 0)
                                };

                                return c;
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


        List<ReporteContable> getReportContable_NC(int Cia, DateTime Desde, DateTime Hasta, string Tipo)
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

                    String Cargar_Hora = "SELECT SUM(C.VrTotal) AS Total, F.FacturaElectronica, F.NumeroNC, F.FechaNC, F.OrdenPedido " +
                                         "FROM CXN_FACTURANC F " +
                                         "INNER JOIN CXN_CARGOSNC C ON F.NumeroNC = C.NumeroNC " +
                                         "WHERE F.Prestador = @param1 " +
                                         "AND C.Tipo = @param2 " +
                                         "AND F.FechaNC BETWEEN @param3 AND @param4 " +
                                         "GROUP BY F.FacturaElectronica, F.NumeroNC, F.FechaNC, F.OrdenPedido " +
                                         "ORDER BY F.NumeroNC ASC";                  

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Cia);
                        Carga_Command.Parameters.AddWithValue("@param2", Tipo);
                        Carga_Command.Parameters.AddWithValue("@param3", Convert.ToDateTime(Desde.Date));
                        Carga_Command.Parameters.AddWithValue("@param4", Convert.ToDateTime(Hasta.Date));

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<ReporteContable> L = new List<ReporteContable>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    DatosNC D = gtDatosNC(Cia, Convert.ToInt32(Lectura_Hora["OrdenPedido"]), Tipo);
                                    if (D == null)
                                    {
                                        continue;
                                    }

                                    L.Add(new ReporteContable
                                    {
                                        Fecha = Convert.ToDateTime(Lectura_Hora["FechaNC"]),
                                        TipoDocumento = Letras(Lectura_Hora["NumeroNC"].ToString()),
                                        NumeroDocumento = Convert.ToInt32(Numeros(Lectura_Hora["NumeroNC"].ToString())),
                                        Concepto = "NOTA CREDITO DE FACTURA ELECTRONICA " + Lectura_Hora["FacturaElectronica"].ToString(),
                                        Identidad = D.Identidad,
                                        Valor = Convert.ToInt32(Lectura_Hora["Total"]),
                                        Descuentos = 0,
                                        IVA = (Tipo == "OTRAS" ? D.Iva : "0" ),
                                        IdentificadorAse = Convert.ToInt32(D.Identificador),
                                        ClaseFactura = Tipo,
                                        FacturaZamenis = Convert.ToInt32(Lectura_Hora["OrdenPedido"]),
                                        ICA = (Tipo == "OTRAS" ? D.Ica : "0"),
                                        ReteFuente = (Tipo == "OTRAS" ? D.RFuente : "0")
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

        class DatosNC
        {
            public string Identidad { get; set; }
            public string IdentidadNombre { get; set; }
            public string Identificador { get; set; }
            public string Iva { get; set; }
            public string Ica { get; set; }
            public string RFuente { get; set; }
        }

        DatosNC gtDatosNC(int Cia, int Zamenis, string Tipo)
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
                    
                    if (Tipo == "Caja")
                    {
                        Cargar_Hora = "SELECT A.Pac_PrimerA + ' ' + A.Pac_SegundoA + ' ' + A.Pac_PrimerN + ' ' + A.Pac_SegundoN AS PACIENTE, A.Pac_IdNum " +
                                        "FROM CXN_HORARIO H " +
                                        "INNER JOIN CXN_PACIENTES A ON H.Hor_Pac_Id = A.Pac_Id " +
                                        "WHERE H.Hor_Pac_Cia = @param1 " +
                                        "AND H.Hor_Id = @param2";
                    }
                    else if (Tipo == "Salud")
                    {
                        Cargar_Hora = "SELECT A.Ase_Descripcion, A.Ase_Identificador, A.Ase_NitCia " +
                                      "FROM CXN_FACTURA F " +
                                      "INNER JOIN CXN_ASEGURADORA A ON F.Fac_Ase = A.Ase_Identificador " +
                                      "WHERE F.Fac_Cia = @param1 " +
                                      "AND F.Fac_Num_Fac = @param2";
                    }
                    else if (Tipo == "VENTAS")
                    {
                        Cargar_Hora = "SELECT A.Pac_PrimerA + ' ' + A.Pac_SegundoA + ' ' + A.Pac_PrimerN + ' ' + A.Pac_SegundoN AS PACIENTE, A.Pac_IdNum " +
                                        "FROM CXN_VENTAS H " +
                                        "INNER JOIN CXN_PACIENTES A ON H.Ven_Cod_Pac = A.Pac_Id " +
                                        "WHERE H.Ven_Cod_Cia = @param1 " +
                                        "AND H.Ven_Factura = @param2";
                    }
                    else
                    {
                        return null;
                    }

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Cia);
                        Carga_Command.Parameters.AddWithValue("@param2", Zamenis);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                DatosNC D = new DatosNC();

                                if (Tipo == "Salud")
                                {
                                    D = new DatosNC
                                    {
                                        Identidad = Lectura_Hora["Ase_NitCia"].ToString(),
                                        Identificador = Convert.ToInt32(Lectura_Hora["Ase_Identificador"]).ToString(),    
                                        IdentidadNombre = Lectura_Hora["Ase_Descripcion"].ToString()
                                    };
                                }
                                else if (Tipo == "Caja")
                                {
                                    D = new DatosNC
                                    {
                                        Identidad = Lectura_Hora["Pac_IdNum"].ToString(),
                                        IdentidadNombre = "RECAUDO DE BONOS - " + Lectura_Hora["PACIENTE"].ToString(),
                                        Identificador = "88"
                                    };
                                }
                                else if (Tipo == "VENTAS")
                                {
                                    D = new DatosNC
                                    {
                                        Identidad = Lectura_Hora["Pac_IdNum"].ToString(),
                                        IdentidadNombre = "VENTAS - " + Lectura_Hora["PACIENTE"].ToString(),
                                        Identificador = "88"
                                    };
                                }

                                return D;
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

        List<ReporteContable> getReportContable_Bonos(int Cia, DateTime Desde, DateTime Hasta)
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

                    /*String Cargar_Hora = "SELECT H.Hor_DocFEModeradorFechaHora, H.Hor_DocFEModerador, P.Pac_IdNum, H.Hor_Pac_Ase, H.Hor_Imp_Age AS Paciente, H.Hor_RcCaja AS Total " +
                                        "FROM CXN_HORARIO H " +
                                        "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                        "WHERE H.Hor_Pac_Cia = @param1 " +
                                        "AND H.Hor_DocFEModeradorCUFE IS NOT NULL " +
                                        "AND H.Hor_DocFEModeradorCUFE <> '' " +
                                        "AND H.Hor_DocFEModeradorFechaHora BETWEEN @param3 AND @param4 " +
                                        "GROUP BY H.Hor_DocFEModeradorFechaHora, H.Hor_DocFEModerador, P.Pac_IdNum, H.Hor_Imp_Age, H.Hor_Pac_Ase, Hor_RcCaja  " +
                                        "ORDER BY H.Hor_DocFEModerador ASC";*/

                    String Cargar_Hora = "SELECT H.Hor_DocFEModeradorFechaHora, H.Hor_DocFEModerador, P.Pac_IdNum, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS Paciente, H.Rc_Caja_Valor AS Total " +
                                        "FROM CXN_RC_CAJA H " +
                                        "INNER JOIN CXN_PACIENTES P ON H.Rc_Caja_Pac = P.Pac_Id " +
                                        "WHERE H.RC_Caja_Cia = @param1 " +
                                        "AND H.Hor_DocFEModeradorCUFE IS NOT NULL " +
                                        "AND H.Hor_DocFEModeradorCUFE <> '' " +
                                        "AND H.Hor_DocFEModeradorFechaHora BETWEEN @param3 AND @param4 " +
                                        "GROUP BY H.Hor_DocFEModeradorFechaHora, H.Hor_DocFEModerador, P.Pac_IdNum, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, H.Rc_Caja_Valor  " +
                                        "ORDER BY H.Hor_DocFEModerador ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Cia);
                        Carga_Command.Parameters.AddWithValue("@param3", Convert.ToDateTime(Desde.Date));
                        Carga_Command.Parameters.AddWithValue("@param4", Convert.ToDateTime(Hasta.Date));

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<ReporteContable> L = new List<ReporteContable>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new ReporteContable
                                    {
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Hor_DocFEModeradorFechaHora"]),
                                        TipoDocumento = Letras(Lectura_Hora["Hor_DocFEModerador"].ToString()),
                                        NumeroDocumento = Convert.ToInt32(Numeros(Lectura_Hora["Hor_DocFEModerador"].ToString())),
                                        //Cuenta = Lectura_Hora["Ase_NitCia"].ToString(),
                                        Concepto = "BONOS " + Lectura_Hora["Paciente"].ToString(),
                                        Identidad = Lectura_Hora["Pac_IdNum"].ToString(),
                                        //CentroCosto = Lectura_Hora["Ase_Identificador"].ToString(),
                                        Valor = Convert.ToInt32(Lectura_Hora["Total"]),
                                        Descuentos = 0,
                                        IVA = "0",
                                        Tabla = "CXN_HORARIO",
                                        IdentificadorAse = Convert.ToInt32(Lectura_Hora["Hor_Pac_Ase"]),
                                        ClaseFactura = "BONOS",
                                        Clase = "BONOS",
                                        //Naturaleza = "Credito",
                                        //Clase = "Facturacion",
                                        //ConsecutivoContable = "FAC-" + Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]).ToString("yyyyMMdd") + "-" + Convert.ToInt32(Lectura_Hora["Homologo"])
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
        List<ReporteContable> getReportContable_Ventas(int Cia, DateTime Desde, DateTime Hasta)
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

                    String Cargar_Hora = "SELECT V.Ven_Fecha, V.Ven_Homologo, P.Pac_IdNum, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS Paciente, SUM(V.Ven_Total) AS Total " +
                                        "FROM CXN_VENTAS V " +
                                        "INNER JOIN CXN_PACIENTES P ON V.Ven_Cod_Pac = P.Pac_Id " +
                                        "WHERE V.Ven_Cod_Cia = @param1 " +
                                        "AND V.Ven_Estado = @param2 " +
                                        "AND V.Cufe IS NOT NULL " +
                                        "AND V.Cufe <> '' " +
                                        "AND V.Ven_Fecha BETWEEN @param3 AND @param4 " +
                                        "GROUP BY V.Ven_Fecha, V.Ven_Homologo, P.Pac_IdNum, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN  " +
                                        "ORDER BY V.Ven_Homologo ASC";                   

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Cia);
                        Carga_Command.Parameters.AddWithValue("@param2", "F");
                        Carga_Command.Parameters.AddWithValue("@param3", Convert.ToDateTime(Desde.Date));
                        Carga_Command.Parameters.AddWithValue("@param4", Convert.ToDateTime(Hasta.Date));

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<ReporteContable> L = new List<ReporteContable>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new ReporteContable
                                    {
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Ven_Fecha"]),
                                        TipoDocumento = Letras(Lectura_Hora["Ven_Homologo"].ToString()),
                                        NumeroDocumento = Convert.ToInt32(Numeros(Lectura_Hora["Ven_Homologo"].ToString())),
                                        Concepto = "VENTAS RECEPCION " + Lectura_Hora["Paciente"].ToString(),
                                        Identidad = Lectura_Hora["Pac_IdNum"].ToString(),
                                        Valor = Convert.ToInt32(Lectura_Hora["Total"]),
                                        Descuentos = 0,
                                        IVA = "0",
                                        Tabla = "CXN_VENTAS",
                                        IdentificadorAse = 88,
                                        ClaseFactura = "VENTAS",
                                        Clase = "VENTAS"
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
        List<ReporteContable> getReportContable_Facturacion(int Cia, DateTime Desde, DateTime Hasta, string TipoServicio)
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

                    if (TipoServicio == "CURACIONES")
                    {
                        Cargar_Hora = "SELECT F.Fac_Fecha, F.Homologo, A.Ase_NitCia, P.Pac_IdNum, A.Ase_Identificador, A.Ase_Descripcion, F.VrCompartido, F.Copago, F.Anticipo, F.Fac_Num_Fac, F.Fac_Descuento, C.Car_RetFte, C.Car_ICA, C.Car_IVA, 'CURACIONES' AS Clase, SUM(C.Car_Val_Tot)  AS Total " +
                                        "FROM CXN_FACTURA F " +
                                        "INNER JOIN CXN_CARGOS C ON F.Fac_Num_Fac = C.Car_Factura " +
                                        "INNER JOIN CXN_PACIENTES P ON F.Fac_Pac = P.Pac_Id " +
                                        "INNER JOIN CXN_ASEGURADORA A ON F.Fac_Ase = A.Ase_Identificador " +
                                        "AND C.Car_Ase = A.Ase_Identificador " +
                                        "WHERE F.Fac_Cia = @param1 " +
                                        "AND C.Car_Cia = @param1 " +
                                        "AND F.Fac_Estado = @param2 " +
                                        "AND C.Car_Estado = @param2 " +
                                        "AND F.Cufe IS NOT NULL " +
                                        "AND F.Cufe <> '' " +
                                        "AND C.Car_Tipo_Serv IN ('MG','CU') " +
                                        "AND F.Fac_Fecha BETWEEN @param3 AND @param4 " +
                                        "GROUP BY F.Fac_Fecha, F.Homologo, A.Ase_NitCia, F.Fac_Cia, P.Pac_IdNum, A.Ase_Identificador, A.Ase_Descripcion, F.VrCompartido, F.Fac_Num_Fac, C.Car_ICA, F.Copago, F.Anticipo, F.Fac_Descuento, C.Car_RetFte, C.Car_IVA  " +
                                        "ORDER BY F.Fac_Cia, F.Fac_Fecha ASC";
                    }
                    if (TipoServicio == "FIBROMIALGIA")
                    {
                        Cargar_Hora = "SELECT F.Fac_Fecha, F.Homologo, A.Ase_NitCia, P.Pac_IdNum, A.Ase_Identificador, A.Ase_Descripcion, F.VrCompartido, F.Copago, F.Anticipo, F.Fac_Num_Fac, F.Fac_Descuento, C.Car_RetFte, C.Car_ICA, C.Car_IVA, 'TERAPIA' AS Clase, C.Car_IVA, SUM(C.Car_Val_Tot)  AS Total " +
                                        "FROM CXN_FACTURA F " +
                                        "INNER JOIN CXN_CARGOS C ON F.Fac_Num_Fac = C.Car_Factura " +
                                        "INNER JOIN CXN_PACIENTES P ON F.Fac_Pac = P.Pac_Id " +
                                        "INNER JOIN CXN_ASEGURADORA A ON F.Fac_Ase = A.Ase_Identificador " +
                                        "AND C.Car_Ase = A.Ase_Identificador " +
                                        "WHERE F.Fac_Cia = @param1 " +
                                        "AND C.Car_Cia = @param1 " +
                                        "AND F.Fac_Estado = @param2 " +
                                        "AND C.Car_Estado = @param2 " +
                                        "AND F.Cufe IS NOT NULL " +
                                        "AND F.Cufe <> '' " +
                                        "AND C.Car_Tipo_Serv IN ('TF','TO','PS','FI') " +
                                        "AND F.Fac_Fecha BETWEEN @param3 AND @param4 " +
                                        "GROUP BY F.Fac_Fecha, F.Homologo, A.Ase_NitCia, F.Fac_Cia, P.Pac_IdNum, A.Ase_Identificador, A.Ase_Descripcion, F.VrCompartido, F.Copago, F.Fac_Num_Fac, C.Car_RetFte, C.Car_ICA, F.Anticipo, F.Fac_Descuento, C.Car_IVA " +
                                        "ORDER BY F.Fac_Cia, F.Fac_Fecha ASC";
                    }
                    if (TipoServicio == "OTRAS")
                    {
                        Cargar_Hora = "SELECT F.Fac_Fecha, F.Homologo, A.Ase_NitCia, P.Pac_IdNum, A.Ase_Identificador, A.Ase_Descripcion, F.VrCompartido, F.Copago, F.Anticipo, F.Fac_Num_Fac, C.Car_RetFte, C.Car_ICA, F.Fac_Descuento, C.Car_IVA, 'OTRAS' AS Clase, SUM(C.Car_Val_Tot)  AS Total " +
                                        "FROM CXN_FACTURA F " +
                                        "INNER JOIN CXN_CARGOS C ON F.Fac_Num_Fac = C.Car_Factura " +
                                        "INNER JOIN CXN_PACIENTES P ON F.Fac_Pac = P.Pac_Id " +
                                        "INNER JOIN CXN_ASEGURADORA A ON F.Fac_Ase = A.Ase_Identificador " +
                                        "AND C.Car_Ase = A.Ase_Identificador " +
                                        "WHERE F.Fac_Cia = @param1 " +
                                        "AND C.Car_Cia = @param1 " +
                                        "AND F.Fac_Estado = @param2 " +
                                        "AND C.Car_Estado = @param2 " +
                                        "AND F.Cufe IS NOT NULL " +
                                        "AND F.Cufe <> '' " +
                                        "AND C.Car_Tipo_Serv = 'Otro' " +
                                        "AND F.Fac_Fecha BETWEEN @param3 AND @param4 " +
                                        "GROUP BY F.Fac_Fecha, F.Homologo, A.Ase_NitCia, F.Fac_Cia, P.Pac_IdNum, A.Ase_Identificador, A.Ase_Descripcion, F.VrCompartido, F.Copago, F.Fac_Num_Fac, C.Car_RetFte, C.Car_ICA, F.Anticipo, F.Fac_Descuento, C.Car_IVA " +
                                        "ORDER BY F.Fac_Cia, F.Fac_Fecha ASC";
                    }

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Cia);
                        Carga_Command.Parameters.AddWithValue("@param2", "F");
                        Carga_Command.Parameters.AddWithValue("@param3", Convert.ToDateTime(Desde.Date));
                        Carga_Command.Parameters.AddWithValue("@param4", Convert.ToDateTime(Hasta.Date));

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<ReporteContable> L = new List<ReporteContable>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    int Desc1 = !string.IsNullOrEmpty(Lectura_Hora["VrCompartido"].ToString()) ? Convert.ToInt32(Lectura_Hora["VrCompartido"]) : 0;
                                    int Desc2 = !string.IsNullOrEmpty(Lectura_Hora["Copago"].ToString()) ? Convert.ToInt32(Lectura_Hora["Copago"]) : 0;
                                    int Desc3 = !string.IsNullOrEmpty(Lectura_Hora["Anticipo"].ToString()) ? Convert.ToInt32(Lectura_Hora["Anticipo"]) : 0;
                                    int Desc4 = !string.IsNullOrEmpty(Lectura_Hora["Fac_Descuento"].ToString()) ? Convert.ToInt32(Lectura_Hora["Fac_Descuento"]) : 0;

                                    int Desc = Desc1 + Desc2 + Desc3 + Desc4;

                                    L.Add(new ReporteContable
                                    {
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]),
                                        TipoDocumento = Letras(Lectura_Hora["Homologo"].ToString()),
                                        NumeroDocumento = Convert.ToInt32(Numeros(Lectura_Hora["Homologo"].ToString())),
                                        Concepto = " " + Lectura_Hora["Ase_Descripcion"].ToString(),
                                        Identidad = Lectura_Hora["Ase_NitCia"].ToString(),
                                        Valor = Convert.ToInt32(Lectura_Hora["Total"]),
                                        Descuentos = Desc,
                                        IVA = Lectura_Hora["Car_IVA"].ToString(),
                                        Tabla = "CXN_FACTURA",
                                        IdentificadorAse = Convert.ToInt32(Lectura_Hora["Ase_Identificador"]),
                                        ClaseFactura = Lectura_Hora["Clase"].ToString(),
                                        FacturaZamenis = Convert.ToInt32(Lectura_Hora["Fac_Num_Fac"]),
                                        ICA = Lectura_Hora["Car_ICA"].ToString(),
                                        ReteFuente = Lectura_Hora["Car_RetFte"].ToString(),
                                        Clase = Lectura_Hora["Clase"].ToString()
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
    

        static string Numeros(string Cadena)
        {
            string numeros = new string(Cadena.Where(char.IsDigit).ToArray());
            return numeros;
        }
        static string Letras(string Cadena)
        {
            string letras = new string(Cadena.Where(char.IsLetter).ToArray());
            return letras;
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
