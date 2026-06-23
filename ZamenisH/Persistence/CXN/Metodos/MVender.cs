using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Persistence.CXN.Metodos
{
    public class MVender : IVender
    {
        private IPacientes repoPacientes;
        private ICompañia repoCompañia;
        private IInventario repoInventario;
        private IFacElectron repoFacElectron;
        private IImpuestos repoImpuestos;
        private IGenerales repoGenerales;
        private IFacturacion repositorioFactura;
        private IConfSystem repoSystem;
        private IHelisa repoWSClaves;

        public MVender()
        {
            repoPacientes = new MPacientes();
            repoCompañia = new MCompañia();
            repoInventario = new MInventario();
            repoFacElectron = new MFacElectron();
            repoImpuestos = new MImpuestos();
            repoGenerales = new MGenerales();
            repositorioFactura = new MFacturacion();
            repoSystem = new MConfSystem();
            repoWSClaves = new MHelisa();   
        }

        CXN_PACIENTES IVender.LlamarPacienteNumDoc(string NId)
        {
            return repoPacientes.LlamarPacienteNumDoc(NId);
        }
        CXN_PACIENTES IVender.LlamarPacientebyId(int pacid)
        {
            return repoPacientes.LlamarPacientebyId(pacid);
        }
        List<CXN_CIA> IVender.getAllCompañias()
        {
            return repoCompañia.getAllCompañias();
        }
        CXN_CIA IVender.getPrestadorbyName(string NamePrestador)
        {
            return repoCompañia.getPrestadorbyName(NamePrestador);
        }
        CXN_CIA IVender.getPrestadorbyCode(int code)
        {
            return repoCompañia.getPrestadorbyCode(code);
        }
        List<CXN_MEDIOSPAGO> IVender.ListaMediosPago()
        {
            return repoFacElectron.ListaMediosPago();
        }
        CXN_INVENTARIO IVender.getProductbyCode(string Code)
        {
            return repoInventario.getProductbyCode(Code);
        }
        int IVender.Fuente(int SubTotal, string FuenteTarifa)
        {
            return repoImpuestos.Fuente(SubTotal, FuenteTarifa);
        }
        decimal IVender.ICA(string IcaTarifa)
        {
            return repoImpuestos.ICA(IcaTarifa);
        }
        void IVender.ActualizarCliente(CXN_PACIENTES P)
        {
            var getCon = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
             
                string Busqueda = "UPDATE CXN_PACIENTES " +
                                  "SET Pac_Email = @param1, " +
                                  "Pac_Telefono = @param2 " +
                                  "WHERE Pac_Id = @param3";

                SqlCommand Accion = new SqlCommand(Busqueda, con);
                Accion.Parameters.AddWithValue("@param1", P.Pac_Email);
                Accion.Parameters.AddWithValue("@param2", P.Pac_Telefono);
                Accion.Parameters.AddWithValue("@param3", P.Pac_Id);
                Accion.ExecuteNonQuery();
            }
        }
        bool IVender.insertarVenta(CXN_VENTAS ventas)
        {
            try
            {
                var getDataCon = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(getDataCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now;
                    DateTime Ven_Fecha = Convert.ToDateTime(Hoy.ToString(getDataCon["Format_Fecha"]));

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_VENTAS (Ven_Cod, " + //param1
                                                          "Ven_Cantidad, " + //param2
                                                          "Ven_Precio, " + //param3
                                                          "Ven_Total, " + //param4
                                                          "Ven_Cod_Pac, " + //param5
                                                          "Ven_Cod_Cia, " + //param6
                                                          "Ven_Estado, " + //param7
                                                          "Ven_Factura, " + //param8
                                                          "Ven_Usr_Graba, " + //param9
                                                          "Ven_Item, " + //param10
                                                          "Ven_Res, " + //param11
                                                          "Ven_Dcto, " + //param11
                                                          "Ven_Fecha, " +
                                                          "Ven_Tipo_Doc, " +
                                                          "MetodoPago, " +
                                                          "MedioPago, " +
                                                          "FormaPago, " +
                                                          "PercentICA, " +
                                                          "PercentFUENTE, " +
                                                          "Num_Cruce) " + //param16
                                 "values                  (@param1, " + // Hor_Estado
                                                          "@param2, " + // Hor_Pac_Id
                                                          "@param3, " + // Hor_Pac_Bod
                                                          "@param4, " + // Hor_Pac_Tipo_Serv
                                                          "@param5, " + // Hor_Pac_Cia
                                                          "@param6, " + // Hor_Pac_Ase
                                                          "@param7, " + // Hor_Pac_Cup
                                                          "@param8, " + // Hor_Pac_UsrGraba
                                                          "@param9, " + // Hor_Imp_Age
                                                          "@param10, " + // Hor_Pac_Fecha
                                                          "@param11, " + // Hor_Pac_Fecha_Cita
                                                          "@param12, " + // Hor_Pac_Fecha_Cita
                                                          "@param13, " +
                                                          "@param14, " +
                                                          "@param15, " +
                                                          "@param16, " +
                                                          "@param17," +
                                                          "@param18," +
                                                          "@param19, " +
                                                          "@param20)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", ventas.Ven_Cod);
                    cmd.Parameters.AddWithValue("@param2", ventas.Ven_Cantidad);
                    cmd.Parameters.AddWithValue("@param3", ventas.Ven_Precio);
                    cmd.Parameters.AddWithValue("@param4", ventas.Ven_Total);
                    cmd.Parameters.AddWithValue("@param5", ventas.Ven_Cod_Pac);
                    cmd.Parameters.AddWithValue("@param6", ventas.Ven_Cod_Cia);
                    cmd.Parameters.AddWithValue("@param7", ventas.Ven_Estado);
                    cmd.Parameters.AddWithValue("@param8", ventas.Ven_Factura); //usuario factura
                    cmd.Parameters.AddWithValue("@param9", ventas.Ven_Usr_Graba);
                    cmd.Parameters.AddWithValue("@param10", ventas.Ven_Item);
                    cmd.Parameters.AddWithValue("@param11", ventas.Ven_Res);
                    cmd.Parameters.AddWithValue("@param12", ventas.Ven_Dcto);
                    cmd.Parameters.Add(new SqlParameter("@param13", SqlDbType.DateTime)).Value = Ven_Fecha;
                    cmd.Parameters.AddWithValue("@param14", ventas.Ven_Tipo_Doc);
                    cmd.Parameters.AddWithValue("@param15", ventas.MetodoPago);
                    cmd.Parameters.AddWithValue("@param16", ventas.MedioPago);
                    cmd.Parameters.AddWithValue("@param17", ventas.FormaPago);
                    cmd.Parameters.AddWithValue("@param18", ventas.PercentICA);
                    cmd.Parameters.AddWithValue("@param19", ventas.PercentFUENTE);
                    cmd.Parameters.AddWithValue("@param20", ventas.Num_Cruce);

                    int s = cmd.ExecuteNonQuery();
                    return s > 0 ? true : false;
                }
            }
            catch (Exception eex)
            {
                Console.WriteLine(eex.ToString());
                return false;
            }
        }
        bool IVender.UpdateFuenteICA(int Fuente, int Ica, int Orden, int Cia)
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
                                      "SET VrICA = '" + Convert.ToInt32(Ica) + "', " +
                                      "VrFUENTE = '" + Convert.ToInt32(Fuente) + "' " +
                                      "WHERE Ven_Factura = '" + Orden + "' " +
                                      "AND Ven_Cod_Cia = '" + Cia + "'";

                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        List<FacturacionRpt> IVender.Exp_Fac_Ven(int Docu_Ven, int cia, string Tipo)
        {
            try
            {
                var getDataCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT V.Ven_Usr_Graba, V.Ven_Factura, C.Com_Direccion, C.Com_Telefono, C.Com_Identificacion, C.Com_Prefijo_Electron, C.Com_Doc_Electron, C.Com_Resolucion_Electron, P.Pac_TipoId, P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_IdNum, P.Pac_Email, " +
                                         "P.Pac_Direccion, P.Pac_Telefono, V.Ven_Fecha, C.Com_Logo, C.Com_Nombre, V.Ven_Res, V.Ven_Dcto, V.Ven_Cod, Ven_Item, V.Ven_Cantidad, V.Ven_Precio, V.Ven_Total, V.Cufe, V.Ven_Homologo, V.Ven_Dcto, " +
                                         "V.DiasVencimiento, V.MedioPago, V.MetodoPago, V.Hora, V.PercentICA, V.PercentFUENTE, V.VrICA, V.VrFUENTE, " +
                                         "SUM(CAST(V.Ven_Total as int)) as ValTot " +
                                         "FROM CXN_VENTAS V " +
                                         "INNER JOIN CXN_PACIENTES P ON V.Ven_Cod_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_CIA C ON V.Ven_Cod_Cia = C.Com_Identificador " +
                                         "WHERE V.Ven_Factura = @param1 " +
                                         "AND V.Ven_Cod_Cia = @param2 " +
                                         "AND V.Ven_Tipo_Doc = @param3 " +
                                         "GROUP BY V.Ven_Usr_Graba, V.Ven_Factura, C.Com_Direccion, C.Com_Telefono, C.Com_Identificacion, C.Com_Prefijo_Electron, C.Com_Doc_Electron, C.Com_Resolucion_Electron, P.Pac_TipoId, P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_IdNum, P.Pac_Email, " +
                                         "P.Pac_Direccion, P.Pac_Telefono, V.Ven_Fecha, C.Com_Logo, C.Com_Nombre, V.Ven_Res, V.Ven_Dcto, V.Ven_Cod, Ven_Item, V.Ven_Cantidad, V.Ven_Precio, V.Ven_Total, V.Cufe, V.Ven_Homologo, V.Ven_Dcto, " +
                                         "V.DiasVencimiento, V.MedioPago, V.MetodoPago, V.Hora, V.PercentICA, V.PercentFUENTE, V.VrICA, V.VrFUENTE";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Docu_Ven);
                        Carga_Command.Parameters.AddWithValue("@param2", cia);
                        Carga_Command.Parameters.AddWithValue("@param3", Tipo);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<FacturacionRpt> Class_FacVen1 = new List<FacturacionRpt>();
                                var SubT = SubTotalRec(Docu_Ven, cia, Tipo);

                                Image QRReal = null;
                                string textoQR = getQRVentas(Docu_Ven.ToString(), cia, Tipo);
                                if (textoQR != "")
                                {
                                    QRReal = repoGenerales.CodifyQR(textoQR);
                                }

                                while (Lectura_Hora.Read() == true)
                                {
                                    string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString(); //trae base64
                                    Byte[] bytes = Convert.FromBase64String(Bod_Firma1); //convierte a bytes
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = Image.FromStream(stmBLOBData);

                                    string Letra;
                                    Letra = repositorioFactura.enletras(Convert.ToInt32(SubT).ToString()).ToUpper() + " PESOS";

                                    string NumFacturaReal = Lectura_Hora["Ven_Factura"].ToString();

                                    if (!string.IsNullOrEmpty(Lectura_Hora["Ven_Homologo"].ToString()))
                                    {
                                        NumFacturaReal = Lectura_Hora["Ven_Homologo"].ToString();
                                    }

                                    DateTime hTemp = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day,
                                        00, 00, 00);

                                    if (!string.IsNullOrEmpty(Lectura_Hora["Hora"].ToString()))
                                    {
                                        hTemp = Convert.ToDateTime(Lectura_Hora["Hora"]);
                                    }

                                    string perFuente = "0";
                                    string perIca = "0";
                                    int vrFuente = 0;
                                    int vrIca = 0;

                                    if (!string.IsNullOrEmpty(Lectura_Hora["PercentICA"].ToString()))
                                    {
                                        perIca = Lectura_Hora["PercentICA"].ToString();
                                    }
                                    if (!string.IsNullOrEmpty(Lectura_Hora["PercentFUENTE"].ToString()))
                                    {
                                        perFuente = Lectura_Hora["PercentFUENTE"].ToString();
                                    }
                                    if (!string.IsNullOrEmpty(Lectura_Hora["VrICA"].ToString()))
                                    {
                                        vrIca = Convert.ToInt32(Lectura_Hora["VrICA"]);
                                    }
                                    if (!string.IsNullOrEmpty(Lectura_Hora["VrFUENTE"].ToString()))
                                    {
                                        vrFuente = Convert.ToInt32(Lectura_Hora["VrFUENTE"]);
                                    }

                                    Class_FacVen1.Add(new FacturacionRpt
                                    {
                                        Descuento = Convert.ToInt32(Lectura_Hora["Ven_Dcto"]),
                                        CodigoProd = Lectura_Hora["Ven_Cod"].ToString().Trim(),
                                        ItemProd = Lectura_Hora["Ven_Item"].ToString(),
                                        CantidadProd = Lectura_Hora["Ven_Cantidad"].ToString(),
                                        VrUnitarioProd = Convert.ToInt32(Lectura_Hora["Ven_Precio"]),
                                        VrTotalProd = Convert.ToInt32(Lectura_Hora["Ven_Total"]),
                                        ValorLetras = Letra,
                                        VrNetoaPagar = Convert.ToInt32(SubT),
                                        Deduccion = 0,
                                        VrTotalFac = Convert.ToInt32(SubT),
                                        UsuarioFactura = Lectura_Hora["Ven_Usr_Graba"].ToString(),
                                        NumFac = NumFacturaReal,
                                        EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(),
                                        EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                        EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                        EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                        PacienteNombre = Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString() + " " + Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString(),
                                        PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                        PacienteDireccion = Lectura_Hora["Pac_Direccion"].ToString(),
                                        PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                        FechaBase = Convert.ToDateTime(Lectura_Hora["Ven_Fecha"]),
                                        Logo = repoGenerales.GetBytes(pic.Image),
                                        Resolucion = Lectura_Hora["Ven_Res"].ToString(),
                                        CUFE = Lectura_Hora["Cufe"].ToString(),
                                        Dcto = Convert.ToInt32(Lectura_Hora["Ven_Dcto"]),
                                        QRLogo = (QRReal != null ? repoGenerales.GetBytes(QRReal) : null),

                                        Com_Direccion = Lectura_Hora["Pac_TipoId"].ToString(),
                                        PacienteAseguradora = Lectura_Hora["Pac_IdNum"].ToString(),
                                        ProfesionalNombre = Lectura_Hora["Pac_Email"].ToString(),
                                        Com_Resolucion_Electron = Lectura_Hora["Com_Resolucion_Electron"].ToString(),
                                        PrefijoElectron = Lectura_Hora["Com_Prefijo_Electron"].ToString(),
                                        NumElectron = Convert.ToInt32(Lectura_Hora["Com_Doc_Electron"]),

                                        Dias = (Lectura_Hora["DiasVencimiento"] == DBNull.Value ? 30 : Convert.ToInt32(Lectura_Hora["DiasVencimiento"])),
                                        MedioP = Lectura_Hora["MedioPago"].ToString(),
                                        MetodoP = Lectura_Hora["MetodoPago"].ToString(),
                                        Hora = Convert.ToDateTime(hTemp),

                                        PercentICA = perIca,
                                        PercentFUENTE = perFuente,
                                        VrICA = vrIca,
                                        VrFUENTE = vrFuente,

                                        TotalImpuestos = SubT - vrIca - vrFuente,
                                        LetraImpuestos = repositorioFactura.enletras(Convert.ToInt32(SubT - vrIca - vrFuente).ToString()).ToUpper() + " PESOS"
                                    });
                                }
                                return Class_FacVen1;
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
        int SubTotalRec(int Numero_Fac, int cia, string Tipo)
        {
            try
            {
                var getDataCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Cargar_Hora = "SELECT Ven_Factura, SUM(CAST(Ven_Total as int)) as ValTot " +
                                         "FROM CXN_VENTAS " +
                                         "WHERE Ven_Factura = @param1 " +
                                         "AND Ven_Cod_Cia = @param2 " +
                                         "AND Ven_Tipo_Doc = @param3 " +
                                         "AND Ven_Estado = @param4 " +
                                         "GROUP BY Ven_Factura";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Numero_Fac);
                        Carga_Command.Parameters.AddWithValue("@param2", cia);
                        Carga_Command.Parameters.AddWithValue("@param3", Tipo);
                        Carga_Command.Parameters.AddWithValue("@param4", "F");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["ValTot"]);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }                   
                }
            }
            catch
            {
                return 0;
            }
        }
        string getQRVentas(string Docu_Ven, int cia, string Tipo)
        {
            try
            {
                var getDataCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT C.Com_Nombre, C.Com_Identificacion, C.Com_Tipo_Doc, V.Ven_Factura, V.Ven_Fecha, V.Hora, P.Pac_IdNum, V.Cufe, V.Ven_Homologo, " +
                                         "SUM(CAST(V.Ven_Total as int)) as ValTot " +
                                         "FROM CXN_VENTAS V " +
                                         "INNER JOIN CXN_PACIENTES P ON V.Ven_Cod_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_CIA C ON V.Ven_Cod_Cia = C.Com_Identificador " +
                                         "WHERE V.Ven_Factura = @param1 " +
                                         "AND V.Ven_Cod_Cia = @param2 " +
                                         "AND V.Ven_Tipo_Doc = @param3 " +
                                         "GROUP BY C.Com_Nombre, C.Com_Identificacion, C.Com_Tipo_Doc, V.Ven_Factura, V.Ven_Fecha, V.Hora, P.Pac_IdNum, V.Cufe, V.Ven_Homologo";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Docu_Ven);
                        Carga_Command.Parameters.AddWithValue("@param2", cia);
                        Carga_Command.Parameters.AddWithValue("@param3", Tipo);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                string NumFacturaReal = Lectura_Hora["Ven_Factura"].ToString();


                                if (!string.IsNullOrEmpty(Lectura_Hora["Ven_Homologo"].ToString()) &&
                                    !string.IsNullOrEmpty(Lectura_Hora["Hora"].ToString()) &&
                                    !string.IsNullOrEmpty(Lectura_Hora["Cufe"].ToString()))
                                {
                                    NumFacturaReal = Lectura_Hora["Ven_Homologo"].ToString();

                                    string QrElectron = "NumFac:" + NumFacturaReal + "\r\n" +
                                        "FecFac:" + Convert.ToDateTime(Lectura_Hora["Ven_Fecha"]).ToString("yyyy-MM-dd") + "\r\n" +
                                        "HorFac:" + Convert.ToDateTime(Lectura_Hora["Hora"]).ToString("hh:mm:ss tt") + "\r\n" +
                                        "NitFac:" + Lectura_Hora["Com_Identificacion"].ToString() + "\r\n" +
                                        "DocAdq:" + Lectura_Hora["Pac_IdNum"].ToString() + "\r\n" +
                                        "ValFac:" + Convert.ToInt32(Lectura_Hora["ValTot"]) + "\r\n" + //total antes de iva
                                        "ValIva" + "0" + "\r\n" + //Total IVA 
                                        "ValOtroIm:" + "0" + "\r\n" +
                                        "CUFE:" + Lectura_Hora["Cufe"].ToString() + "\r\n" +
                                        "https://catalogo-vpfe.dian.gov.co/document/searchqr?documentkey=" + Lectura_Hora["Cufe"].ToString();


                                    return QrElectron;
                                }
                                else
                                {
                                    return "";
                                }

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
        List<CXN_VENTAS> IVender.getPrevios(int idPac)
        {
            try
            {
                var getConec = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(getConec["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT Ven_Estado, Ven_Fecha, Pac_PrimerN, Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, Ven_Factura, " +
                                         "Ven_Cod, Ven_Cantidad, Ven_Item " +
                                         "FROM CXN_VENTAS " +
                                         "INNER JOIN CXN_PACIENTES ON CXN_VENTAS.Ven_Cod_Pac = CXN_PACIENTES.Pac_Id " +
                                         "WHERE CXN_VENTAS.Ven_Cod_Pac = @param1 " +
                                         "ORDER BY Ven_Fecha DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", idPac);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_VENTAS> V = new List<CXN_VENTAS>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    string Esta = "";

                                    if (Lectura_Hora["Ven_Estado"].ToString() == "A")
                                    {
                                        Esta = "Anulado";
                                    }
                                    if (Lectura_Hora["Ven_Estado"].ToString() == "F")
                                    {
                                        Esta = "Vigente";
                                    }

                                    V.Add(new CXN_VENTAS
                                    {
                                        Ven_Fecha = Convert.ToDateTime(Lectura_Hora["Ven_Fecha"]),
                                        Grafica = Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString() + " " + Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString(),
                                        Ven_Estado = Esta,
                                        Ven_Factura = Lectura_Hora["Ven_Factura"].ToString(),
                                        Ven_Cod = Lectura_Hora["Ven_Cod"].ToString().TrimEnd() + " - " + Lectura_Hora["Ven_Item"].ToString(),
                                        Ven_Cantidad = Convert.ToInt32(Lectura_Hora["Ven_Cantidad"])
                                    });
                                }

                                return V;
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
        bool IVender.ConsecutivoActualiza(int Cia, string TipoDoc, int NuevoCons)
        {
            return repoCompañia.ConsecutivoActualiza(Cia, TipoDoc, NuevoCons);
        }
        List<CXN_INVENTARIO> IVender.getProductbyName(string Name)
        {
            return repoInventario.getProductbyName(Name);
        }
        Dictionary<string, string> IVender.getListado()
        {
            return repoSystem.getListado();
        }
        string IVender.getTipoDoc(string Tipo)
        {
            return repoPacientes.getTipoDoc(Tipo);
        }
        bool IVender.ValidaEmail(string Val_Email)
        {
            return repoPacientes.ValidaEmail(Val_Email);
        }
        Dictionary<string, string> IVender.Claves(string nameClient, int Prestador)
        {
            return repoWSClaves.Claves(nameClient, Prestador);
        }
        string IVender.GetTokenSaved(int Cia)
        {
            return repoFacElectron.GetTokenSaved(Cia);
        }
        Image IVender.CodifyQR(string T_Codifica)
        {
            return repoGenerales.CodifyQR(T_Codifica);
        }
        byte[] IVender.GetBytes(Image ImageIn)
        {
            return repoGenerales.GetBytes(ImageIn);
        }
    }
}
