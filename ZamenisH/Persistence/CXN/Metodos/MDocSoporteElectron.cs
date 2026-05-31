using Domain;
using Domain.Contabilidad;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;

namespace Persistence.CXN.Metodos
{
    public class MDocSoporteElectron : IDocSoporteElectron
    {
        private static readonly IFacturacion repofac = new MFacturacion();
        private static readonly IGenerales repoGen = new MGenerales();

        List<CXN_PERSONASCOMPROBANTES> IDocSoporteElectron.GetAllPeople()
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

                    String Cargar_Hora = "SELECT * FROM CXN_PERSONASCOMPROBANTES ORDER BY Persona ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_PERSONASCOMPROBANTES> C = new List<CXN_PERSONASCOMPROBANTES>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_PERSONASCOMPROBANTES
                                    {
                                        Ciudad = Lectura_Hora["Ciudad"].ToString(),
                                        Persona = Lectura_Hora["Persona"].ToString(),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        Direccion = Lectura_Hora["Direccion"].ToString(),
                                        Documento = Lectura_Hora["Documento"].ToString(),
                                        Telefono = Lectura_Hora["Telefono"].ToString(),
                                        TipoId = Lectura_Hora["TipoId"].ToString(),
                                        Email = Lectura_Hora["Email"].ToString()
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
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        CXN_PERSONASCOMPROBANTES IDocSoporteElectron.GetPersonDocOrId(string DocId, string TipoBusqueda)
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

                    if (TipoBusqueda == "Documento")
                    {
                        Cargar_Hora = "SELECT * FROM CXN_PERSONASCOMPROBANTES WHERE Documento = @param1";
                    }
                    if (TipoBusqueda == "Id")
                    {
                        Cargar_Hora = "SELECT * FROM CXN_PERSONASCOMPROBANTES WHERE Id = @param1";
                    }

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", DocId);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_PERSONASCOMPROBANTES C = new CXN_PERSONASCOMPROBANTES
                                {
                                    Ciudad = Lectura_Hora["Ciudad"].ToString(),
                                    Persona = Lectura_Hora["Persona"].ToString(),
                                    Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                    Direccion = Lectura_Hora["Direccion"].ToString(),
                                    Documento = Lectura_Hora["Documento"].ToString(),
                                    Telefono = Lectura_Hora["Telefono"].ToString(),
                                    TipoId = Lectura_Hora["TipoId"].ToString(),
                                    Email = Lectura_Hora["Email"].ToString()
                                };                              

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
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        bool IDocSoporteElectron.CrearPresona(CXN_PERSONASCOMPROBANTES P)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_PERSONASCOMPROBANTES (Persona, " +
                                                                  "TipoId, " +
                                                                  "Documento, " +
                                                                  "Direccion, " +
                                                                  "Ciudad, " +
                                                                  "Telefono, " +
                                                                  "Email) " +
                                         "values                  (@param1, " +
                                                                  "@param2, " +
                                                                  "@param3, " +
                                                                  "@param4, " +
                                                                  "@param5, " +
                                                                  "@param6, " +
                                                                  "@param7)", con);

                    cmd.Parameters.AddWithValue("@param1", P.Persona);
                    cmd.Parameters.AddWithValue("@param2", P.TipoId);
                    cmd.Parameters.AddWithValue("@param3", P.Documento);
                    cmd.Parameters.AddWithValue("@param4", P.Direccion);
                    cmd.Parameters.AddWithValue("@param5", P.Ciudad);
                    cmd.Parameters.AddWithValue("@param6", P.Telefono);
                    cmd.Parameters.AddWithValue("@param7", P.Email);
                    
                    int g = cmd.ExecuteNonQuery();
                    
                    if (g > 0)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool IDocSoporteElectron.ActualizarPersona(CXN_PERSONASCOMPROBANTES P)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_PERSONASCOMPROBANTES " +
                                                          "SET  " +
                                                          "Persona = @param1, " +
                                                          "TipoId = @param2, " +
                                                          "Documento = @param3, " +
                                                          "Direccion = @param4, " +
                                                          "Ciudad = @param5, " +
                                                          "Telefono = @param6, " +
                                                          "Email = @param7 " +
                                                          "WHERE Id = @param8", con);

                    Busqueda.Parameters.AddWithValue("@param1", P.Persona);
                    Busqueda.Parameters.AddWithValue("@param2", P.TipoId);
                    Busqueda.Parameters.AddWithValue("@param3", P.Documento);
                    Busqueda.Parameters.AddWithValue("@param4", P.Direccion);
                    Busqueda.Parameters.AddWithValue("@param5", P.Ciudad);
                    Busqueda.Parameters.AddWithValue("@param6", P.Telefono);
                    Busqueda.Parameters.AddWithValue("@param7", P.Email);
                    Busqueda.Parameters.AddWithValue("@param8", P.Id);                  

                    int g = Busqueda.ExecuteNonQuery();

                    if (g > 0)
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
        bool IDocSoporteElectron.InsertarDocumentoSoporte(CXN_FACTURADOCSOPORTE P)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_FACTURADOCSOPORTE (UsuarioGenera, " +
                                                                  "Fecha, " +
                                                                  "DocumentoOrden, " +
                                                                  "CentroCosto, " +
                                                                  "Resolucion, " +
                                                                  "TextoResolucion, " +
                                                                  "ReteFuente, " +
                                                                  "ReteICA, " +
                                                                  "IdCliente, " +
                                                                  "Estado) " +
                                         "values                  (@param1, " +
                                                                  "@param2, " +
                                                                  "@param3, " +
                                                                  "@param4, " +
                                                                  "@param5, " +
                                                                  "@param6, " +
                                                                  "@param7, " +
                                                                  "@param8, " +
                                                                  "@param9, " +
                                                                  "@param10)", con);

                    cmd.Parameters.AddWithValue("@param1", P.UsuarioGenera);
                    cmd.Parameters.AddWithValue("@param2", Convert.ToDateTime(P.Fecha));
                    cmd.Parameters.AddWithValue("@param3", P.DocumentoOrden);
                    cmd.Parameters.AddWithValue("@param4", P.CentroCosto);
                    cmd.Parameters.AddWithValue("@param5", P.Resolucion);
                    cmd.Parameters.AddWithValue("@param6", P.TextoResolucion);
                    cmd.Parameters.AddWithValue("@param7", P.ReteFuente);
                    cmd.Parameters.AddWithValue("@param8", P.ReteICA);
                    cmd.Parameters.AddWithValue("@param9", P.IdCliente);
                    cmd.Parameters.AddWithValue("@param10", "F");

                    int g = cmd.ExecuteNonQuery();

                    if (g > 0)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool IDocSoporteElectron.InsertarCargo(CXN_CARGOSDOCSOPORTE P)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_CARGOSDOCSOPORTE (Concepto, " +
                                                                  "Cantidad, " +
                                                                  "VrUnitario, " +
                                                                  "VrTotal, " +
                                                                  "DocumentoOrden) " +
                                         "values                  (@param1, " +
                                                                  "@param2, " +
                                                                  "@param3, " +
                                                                  "@param4, " +
                                                                  "@param5)", con);

                    cmd.Parameters.AddWithValue("@param1", P.Concepto);
                    cmd.Parameters.AddWithValue("@param2", P.Cantidad);
                    cmd.Parameters.AddWithValue("@param3", P.VrUnitario);
                    cmd.Parameters.AddWithValue("@param4", P.VrTotal);
                    cmd.Parameters.AddWithValue("@param5", P.DocumentoOrden);

                    int g = cmd.ExecuteNonQuery();

                    if (g > 0)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        List<CXN_FACTURADOCSOPORTE> IDocSoporteElectron.GetDocumentsCliente(int IdCliente)
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

                    String Cargar_Hora = "SELECT F.DocumentoOrden, F.DocumentoElectronico, F.Fecha, F.CUFE, CI.Com_Nombre, F.CentroCosto  " +
                                         "FROM CXN_PERSONASCOMPROBANTES P " +
                                         "INNER JOIN CXN_FACTURADOCSOPORTE F ON P.Id = F.IdCliente " +
                                         "INNER JOIN CXN_CIA CI ON F.CentroCosto = CI.Com_Identificador " +
                                         "WHERE P.Id = @param1 " +
                                         "AND F.Estado = @param2 " +
                                         "ORDER BY F.DocumentoOrden ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", IdCliente);
                        Carga_Command.Parameters.AddWithValue("@param2", "F");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_FACTURADOCSOPORTE> C = new List<CXN_FACTURADOCSOPORTE>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_FACTURADOCSOPORTE
                                    {
                                        DocumentoOrden = Convert.ToInt32(Lectura_Hora["DocumentoOrden"]),
                                        DocumentoElectronico = Lectura_Hora["DocumentoElectronico"].ToString(),
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                        CUFE = Lectura_Hora["CUFE"].ToString(),
                                        Concepto = Lectura_Hora["Com_Nombre"].ToString(),
                                        CentroCosto = Convert.ToInt32(Lectura_Hora["CentroCosto"])
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
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        List<CXN_FACTURA> IDocSoporteElectron.GetDocumentsToSign(int Cia, DateTime Desde, DateTime Hasta)
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

                    String Cargar_Hora = "SELECT DISTINCT F.DocumentoOrden, F.DocumentoElectronico, F.Fecha, F.CUFE, CI.Com_Nombre, F.CentroCosto, PA.Persona, F.UsuarioGenera " +
                                         "FROM CXN_PERSONASCOMPROBANTES P " +
                                         "INNER JOIN CXN_FACTURADOCSOPORTE F ON P.Id = F.IdCliente " +
                                         "INNER JOIN CXN_CIA CI ON F.CentroCosto = CI.Com_Identificador " +
                                         "INNER JOIN CXN_PERSONASCOMPROBANTES PA ON F.IdCliente = PA.Id " +
                                         "WHERE F.Fecha BETWEEN @param1 AND @param2 " +
                                         "AND F.CentroCosto = @param3 " +
                                         "GROUP BY F.DocumentoOrden, F.DocumentoElectronico, F.Fecha, F.CUFE, CI.Com_Nombre, F.CentroCosto, PA.Persona, F.UsuarioGenera " +
                                         "ORDER BY F.DocumentoOrden ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Convert.ToDateTime(Desde.Date));
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(Hasta.Date));
                        Carga_Command.Parameters.AddWithValue("@param3", Cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_FACTURA> C = new List<CXN_FACTURA>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    if (string.IsNullOrEmpty(Lectura_Hora["CUFE"].ToString()) || Lectura_Hora["CUFE"].ToString() == "")
                                    {
                                        C.Add(new CXN_FACTURA
                                        {
                                            Fac_Num_Fac = Convert.ToInt32(Lectura_Hora["DocumentoOrden"]),
                                            Fac_Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                            Fac_Fecha_Des = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                            Fac_Fecha_Has = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                            VrCompartido = 0,
                                            Fac_Observa = Lectura_Hora["Persona"].ToString(),
                                            Fac_Usr_Graba = Lectura_Hora["UsuarioGenera"].ToString(),
                                        });
                                    }                                   
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
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        List<DocumentoSoporte> IDocSoporteElectron.GetDocumentoSoportePDF(int Cia, int Orden)
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

                    String Cargar_Hora = "SELECT F.Documentoelectronico, F.CUFE, F.DocumentoOrden, F.ReteICA, F.ReteFuente, " +
                                         "C.Cantidad, C.VrUnitario, C.VrTotal, CO.Com_Nombre, CO.Com_Direccion, CO.Com_Telefono, CO.Com_Identificacion, CO.Com_Identificador, " +
                                         "P.Persona, P.TipoId, P.Documento, P.Direccion, P.Ciudad, P.Telefono, P.Email, F.UsuarioGenera, " +
                                         "F.Fecha, C.Concepto, F.Resolucion, F.TextoResolucion, CO.Com_Logo " +
                                         "FROM CXN_FACTURADOCSOPORTE F " +
                                         "INNER JOIN CXN_CARGOSDOCSOPORTE C ON F.DocumentoOrden = C.DocumentoOrden " +
                                         "INNER JOIN CXN_PERSONASCOMPROBANTES P ON F.IdCliente = P.Id " +
                                         "INNER JOIN CXN_CIA CO ON F.CentroCosto = CO.Com_Identificador " +
                                         "AND F.DocumentoOrden = C.DocumentoOrden " +
                                         "WHERE C.DocumentoOrden = @param1 " +
                                         "AND F.DocumentoOrden = @param1 " +
                                         "AND F.CentroCosto = @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Orden);
                        Carga_Command.Parameters.AddWithValue("@param2", Cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<DocumentoSoporte> lista = new List<DocumentoSoporte>();
                                decimal VrBruto = 0;
                                decimal PercentReteFuente = 0;
                                decimal PercentICA = 0;
                                decimal VrReteFuente = 0;
                                decimal VrReteIca = 0;
                                decimal Neto = 0;
                                decimal SubTotal = 0;
                                string Letra = "";
                                Image QRCUDS = null;
                                bool yaCalculado = false;

                                while (Lectura_Hora.Read() == true)
                                {
                                    // Sumar VrTotal en cada fila
                                    decimal VrTotalFila = Convert.ToDecimal(Lectura_Hora["VrTotal"]);
                                    VrBruto += VrTotalFila;

                                    // Solo ejecutar estos cálculos una vez (en la primera vuelta)
                                    if (!yaCalculado)
                                    {
                                        PercentReteFuente = Decimal.Parse(Lectura_Hora["ReteFuente"].ToString(), CultureInfo.InvariantCulture);
                                        PercentICA = Decimal.Parse(Lectura_Hora["ReteICA"].ToString(), CultureInfo.InvariantCulture);

                                        VrReteFuente = PercentReteFuente > 0 ? ((VrBruto * PercentReteFuente)) / 100 : 0;
                                        VrReteIca = PercentICA > 0 ? VrBruto * PercentICA : 0;
                                        Neto = VrBruto;
                                        SubTotal = Neto + VrReteFuente + VrReteIca;
                                        Letra = repofac.enletras(Neto.ToString());

                                        if (!string.IsNullOrEmpty(Lectura_Hora["CUFE"].ToString()))
                                        {
                                            string AConvertir = "NumFac: " + Lectura_Hora["DocumentoElectronico"].ToString() + "\n" +
                                                "FecFac: " + Convert.ToDateTime(Lectura_Hora["Fecha"]).ToString("yyyy-MM-dd") + "\n" +
                                                "NitFac: " + Lectura_Hora["Com_Identificacion"].ToString() + "\n" +
                                                "ValFac: " + SubTotal + "\n" +
                                                "ValIva: 0.00" + "\n" +
                                                "ValOtroIm: 0.00" + "\n" +
                                                "ValFacIm: " + SubTotal + "\n" +
                                                "CUFE: " + Lectura_Hora["Cufe"].ToString() + "\n" +
                                                "https://catalogo-vpfe.dian.gov.co/document/searchqr?documentkey=" + Lectura_Hora["Cufe"].ToString();

                                            QRCUDS = repoGen.CodifyQR(AConvertir);
                                        }
                                        else
                                        {
                                            string AConvertir = "Orden de pedido pendiente de firma electronica";
                                            QRCUDS = repoGen.CodifyQR(AConvertir);
                                        }

                                        yaCalculado = true; // marcamos que ya hicimos el cálculo inicial
                                    }

                                    string DocReal = Lectura_Hora["DocumentoOrden"].ToString();
                                    if (!string.IsNullOrEmpty(Lectura_Hora["CUFE"].ToString()))
                                    {
                                        DocReal = Lectura_Hora["Documentoelectronico"].ToString();
                                    }

                                    Byte[] bytes = Convert.FromBase64String(Lectura_Hora["Com_Logo"].ToString());

                                    lista.Add(new DocumentoSoporte
                                    {
                                        Documento = DocReal,
                                        ReteICA = Decimal.Parse(Lectura_Hora["ReteICA"].ToString()),
                                        ReteFuente = Decimal.Parse(Lectura_Hora["ReteFuente"].ToString()),
                                        Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        VrUnitario = Convert.ToDecimal(Lectura_Hora["VrUnitario"]),
                                        VrTotal = VrTotalFila,
                                        CentroCostoName = Lectura_Hora["Com_Nombre"].ToString(),
                                        CentroCostoDir = Lectura_Hora["Com_Direccion"].ToString(),
                                        CentroCostoNit = Lectura_Hora["Com_Identificacion"].ToString(),
                                        CentroCostoTel = Lectura_Hora["Com_Telefono"].ToString(),
                                        ClienteCiudad = Lectura_Hora["Ciudad"].ToString(),
                                        ClienteDireccion = Lectura_Hora["Direccion"].ToString(),
                                        ClienteDocumento = Lectura_Hora["Documento"].ToString(),
                                        ClienteEmail = Lectura_Hora["Email"].ToString(),
                                        ClienteName = Lectura_Hora["Persona"].ToString(),
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                        Concepto = Lectura_Hora["Concepto"].ToString(),
                                        Resolucion = Lectura_Hora["Resolucion"].ToString(),
                                        TextoResolucion = Lectura_Hora["TextoResolucion"].ToString(),
                                        UsuarioGenera = Lectura_Hora["UsuarioGenera"].ToString(),
                                        LogoCia = bytes,
                                        VrBruto = VrBruto,
                                        VrReteFuente = VrReteFuente,
                                        VrReteICA = VrReteIca,
                                        VrNeto = Neto,
                                        VrSubtotal = SubTotal,
                                        Letras = Letra,
                                        CodeQR = repoGen.GetBytes(QRCUDS),
                                        CUDS = Lectura_Hora["CUFE"].ToString(),
                                        tipoDoc = Lectura_Hora["TipoId"].ToString(),
                                        DocumentoOrden = Convert.ToInt32(Lectura_Hora["DocumentoOrden"]),
                                        CentroCosto = Convert.ToInt32(Lectura_Hora["Com_Identificador"]),
                                        CUFE = ""
                                    });
                                }

                                return lista;
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
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        bool IDocSoporteElectron.EliminarDocumento(int FacZam, int Cia)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_FACTURADOCSOPORTE " +
                                                          "SET  " +
                                                          "Estado = @param1 " +
                                                          "WHERE DocumentoOrden = @param2 " +
                                                          "AND CentroCosto = @param3 " +
                                                          "AND Estado = @param4", con);

                    Busqueda.Parameters.AddWithValue("@param1", "A");
                    Busqueda.Parameters.AddWithValue("@param2", FacZam);
                    Busqueda.Parameters.AddWithValue("@param3", Cia);
                    Busqueda.Parameters.AddWithValue("@param4", "F");

                    int g = Busqueda.ExecuteNonQuery();

                    if (g > 0)
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
