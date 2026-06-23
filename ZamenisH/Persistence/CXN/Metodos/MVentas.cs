using Domain.CXN;
using Domain;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace Persistence.CXN.Metodos
{
    public class MVentas : IVentas
    {
        private static readonly IFacturacion repositorioFactura = new MFacturacion();
        private static readonly IGenerales repoGenerales = new MGenerales();

        List<CXN_FACTURA> IVentas.ListaFacturasFacElectron(DateTime Desde, DateTime Hasta, int Compañia)
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

                    String Cargar_Hora = "SELECT DISTINCT V.Ven_Homologo, V.Cufe, V.Ven_Factura, V.Ven_Cod_Cia, V.Ven_Tipo_Doc, V.Ven_Cod_Pac, V.Ven_Fecha, V.Ven_Usr_Graba, " +
                                         "P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PAC " +
                                         "FROM CXN_VENTAS V " +
                                         "INNER JOIN CXN_PACIENTES P ON V.Ven_Cod_Pac = P.Pac_Id " +
                                         "WHERE V.Ven_Fecha BETWEEN @param1 AND @param2 " +
                                         "AND V.Ven_Cod_Cia = @param3 " +
                                         "AND V.Ven_Estado = @param4 " +
                                         "AND V.Ven_Tipo_Doc = @param5 " +
                                         "GROUP BY V.Ven_Homologo, V.Cufe, V.Ven_Factura, V.Ven_Cod_Cia, V.Ven_Tipo_Doc, V.Ven_Cod_Pac, V.Ven_Fecha, V.Ven_Usr_Graba, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Convert.ToDateTime(Desde).ToString("yyyy-MM-dd");
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(Hasta).ToString("yyyy-MM-dd");
                        Carga_Command.Parameters.AddWithValue("@param3", Compañia);
                        Carga_Command.Parameters.AddWithValue("@param4", "F");
                        Carga_Command.Parameters.AddWithValue("@param5", "OP");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_FACTURA> L = new List<CXN_FACTURA>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    if (Lectura_Hora["Ven_Homologo"] == DBNull.Value)
                                    {
                                        L.Add(new CXN_FACTURA
                                        {
                                            Fac_Num_Fac = Convert.ToInt32(Lectura_Hora["Ven_Factura"]),
                                            Fac_Fecha = Convert.ToDateTime(Lectura_Hora["Ven_Fecha"]),
                                            Fac_Fecha_Des = Convert.ToDateTime(Lectura_Hora["Ven_Fecha"]),
                                            Fac_Fecha_Has = Convert.ToDateTime(Lectura_Hora["Ven_Fecha"]),
                                            Fac_Usr_Graba = Lectura_Hora["Ven_Usr_Graba"].ToString(),
                                            Fac_Observa = Lectura_Hora["PAC"].ToString(),
                                            Cobertura = "VENTAS VENTANILLA",
                                            VrCompartido = SubTotalRec(Convert.ToInt32(Lectura_Hora["Ven_Factura"]),
                                                                       Convert.ToInt32(Lectura_Hora["Ven_Cod_Cia"]),
                                                                       "OP")
                                        });
                                    }
                                    else if (string.IsNullOrEmpty(Lectura_Hora["Ven_Homologo"].ToString()) || string.IsNullOrWhiteSpace(Lectura_Hora["Ven_Homologo"].ToString())) 
                                    {
                                        L.Add(new CXN_FACTURA
                                        {
                                            Fac_Num_Fac = Convert.ToInt32(Lectura_Hora["Ven_Factura"]),
                                            Fac_Fecha = Convert.ToDateTime(Lectura_Hora["Ven_Fecha"]),
                                            Fac_Fecha_Des = Convert.ToDateTime(Lectura_Hora["Ven_Fecha"]),
                                            Fac_Fecha_Has = Convert.ToDateTime(Lectura_Hora["Ven_Fecha"]),
                                            Fac_Usr_Graba = Lectura_Hora["Ven_Usr_Graba"].ToString(),
                                            Fac_Observa = Lectura_Hora["PAC"].ToString(),
                                            Cobertura = "VENTAS VENTANILLA",
                                            VrCompartido = SubTotalRec(Convert.ToInt32(Lectura_Hora["Ven_Factura"]),
                                                                       Convert.ToInt32(Lectura_Hora["Ven_Cod_Cia"]),
                                                                       "OP")
                                        });
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        CXN_FACTURA IVentas.getDAtosFactura(CXN_FACTURA F)
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

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_VENTAS " +
                                         "WHERE Ven_Factura = @param1 " +
                                         "AND Ven_Cod_Cia = @param2 " +
                                         "AND Ven_Tipo_Doc = @param3 ";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", F.Fac_Num_Fac);
                        Carga_Command.Parameters.AddWithValue("@param2", F.Fac_Cia);
                        Carga_Command.Parameters.AddWithValue("@param3", F.Fac_Tipo_Doc);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_FACTURA F2 = new CXN_FACTURA
                                {
                                    Homologo = Lectura_Hora["Ven_Homologo"].ToString(),
                                    Fac_Fecha = Convert.ToDateTime(Lectura_Hora["Ven_Fecha"]),
                                    Cufe = Lectura_Hora["Cufe"].ToString()
                                };

                                return F2;
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
        List<FacturacionRpt> IVentas.Exp_Fac_Ven(int Docu_Ven, int cia, string Tipo)
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
        List<FacturacionRpt> IVentas.Exp_Fac_Ven(string Docu_Ven, int cia)
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
                                         "V.DiasVencimiento, V.MedioPago, V.MetodoPago, V.Hora, " +
                                         "SUM(CAST(V.Ven_Total as int)) as ValTot " +
                                         "FROM CXN_VENTAS V " +
                                         "INNER JOIN CXN_PACIENTES P ON V.Ven_Cod_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_CIA C ON V.Ven_Cod_Cia = C.Com_Identificador " +
                                         "WHERE V.Ven_Homologo = @param1 " +
                                         "AND V.Ven_Cod_Cia = @param2 " +
                                         "AND V.Ven_Tipo_Doc = @param3 " +
                                         "GROUP BY V.Ven_Usr_Graba, V.Ven_Factura, C.Com_Direccion, C.Com_Telefono, C.Com_Identificacion, C.Com_Prefijo_Electron, C.Com_Doc_Electron, C.Com_Resolucion_Electron, P.Pac_TipoId, P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_IdNum, P.Pac_Email, " +
                                         "P.Pac_Direccion, P.Pac_Telefono, V.Ven_Fecha, C.Com_Logo, C.Com_Nombre, V.Ven_Res, V.Ven_Dcto, V.Ven_Cod, Ven_Item, V.Ven_Cantidad, V.Ven_Precio, V.Ven_Total, V.Cufe, V.Ven_Homologo, V.Ven_Dcto, " +
                                         "V.DiasVencimiento, V.MedioPago, V.MetodoPago, V.Hora";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Docu_Ven);
                        Carga_Command.Parameters.AddWithValue("@param2", cia);
                        Carga_Command.Parameters.AddWithValue("@param3", "OP");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<FacturacionRpt> Class_FacVen1 = new List<FacturacionRpt>();
                                var SubT = SubTotalRec(Docu_Ven, cia);                           

                                while (Lectura_Hora.Read() == true)
                                {                                
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
                                        //Logo = repoGenerales.GetBytes(pic.Image),
                                        Resolucion = Lectura_Hora["Ven_Res"].ToString(),
                                        CUFE = Lectura_Hora["Cufe"].ToString(),
                                        Dcto = Convert.ToInt32(Lectura_Hora["Ven_Dcto"]),
                                        //QRLogo = (QRReal != null ? repoGenerales.GetBytes(QRReal) : null),

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

                                        Admision = Convert.ToInt32(Lectura_Hora["Ven_Factura"])
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
                                         "WHERE Ven_Factura = '" + Numero_Fac + "' " +
                                         "AND Ven_Cod_Cia = '" + cia + "' " +
                                         "AND Ven_Tipo_Doc = '" + Tipo + "' " +
                                         "AND Ven_Estado = 'F' " +
                                         "GROUP BY Ven_Factura";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
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
            catch
            {
                return 0;
            }
        }
        //POR HOMOLOGO
        int SubTotalRec(string Numero_Fac, int cia)
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
                                         "WHERE Ven_Homologo = @param1 " +
                                         "AND Ven_Cod_Cia = @param2 " +
                                         "AND Ven_Tipo_Doc = @param3 " +
                                         "AND Ven_Estado = @param4 " +
                                         "GROUP BY Ven_Factura";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Numero_Fac);
                        Carga_Command.Parameters.AddWithValue("@param2", cia);
                        Carga_Command.Parameters.AddWithValue("@param3", "OP");
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
        List<ReportesRecepcion> IVentas.Rpt_FacturasVenta(DateTime Desde,
                                                  DateTime Hasta,
                                                  int Cia,
                                                  string Prestador,
                                                  string Tipo)
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
                    String Cargar_Hora = "SELECT V.Ven_Fecha, V.Ven_Factura, A.Ase_Descripcion, V.Ven_Homologo, V.Ven_Item, V.Ven_Deducciones, V.Ven_Tipo_Doc, V.Ven_Usr_Graba, V.FormaPago, V.Num_Cruce, " +
                                         "SUM(V.Ven_Total) AS Val " +
                                         "FROM CXN_VENTAS V " +
                                         "INNER JOIN CXN_PACIENTES P ON V.Ven_Cod_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_ASEGURADORA A ON P.Pac_Aseguradora = A.Ase_Identificador " +
                                         "WHERE V.Ven_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString(getDataCon["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getDataCon["Format_Fecha"]) + "' " +
                                         "AND V.Ven_Estado = 'F' " +
                                         "AND V.Ven_Cod_Cia = '" + Cia + "' " +
                                         "AND V.Ven_Tipo_Doc = '" + Tipo + "' " +
                                         "GROUP BY V.Ven_Fecha, V.Ven_Factura, A.Ase_Descripcion, V.Ven_Homologo, V.Ven_Item, V.Ven_Deducciones, V.Ven_Tipo_Doc, V.Ven_Usr_Graba, V.FormaPago, V.Num_Cruce";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                var Tota = CalculoFacturas(Desde, Hasta, Cia, Tipo);
                                List<ReportesRecepcion> class_RptRecepcion = new List<ReportesRecepcion>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    class_RptRecepcion.Add(new ReportesRecepcion
                                    {
                                        FechaBase = Convert.ToDateTime(Lectura_Hora["Ven_Fecha"]),
                                        ValorReciboFactura = Convert.ToInt32(Lectura_Hora["Val"]),
                                        Admision = Convert.ToInt32(Lectura_Hora["Ven_Factura"]),
                                        PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                        Homologo = Lectura_Hora["Ven_Homologo"].ToString(),
                                        PacienteNombre = Lectura_Hora["Ven_Item"].ToString(),
                                        Desde = Convert.ToDateTime(Desde),
                                        Hasta = Convert.ToDateTime(Hasta),
                                        TotalRpt = Convert.ToInt32(Tota),
                                        Tipo = Lectura_Hora["Ven_Tipo_Doc"].ToString(),
                                        EmpresaNombre = Prestador,
                                        ProfesionalNombre = Lectura_Hora["Ven_Usr_Graba"].ToString(),
                                        PacienteTelefono = string.IsNullOrEmpty(Lectura_Hora["FormaPago"].ToString()) ? "" : Lectura_Hora["FormaPago"].ToString(),
                                        Num_Cruce = Convert.ToInt32(Lectura_Hora["Num_Cruce"]),
                                        TReport = "Ventas Recepcion"
                                    });
                                }

                                int Efectivo = 0;
                                int TarjetaCredito = 0;
                                int TarjetaDebito = 0;
                                int Nequi = 0;
                                int Daviplata = 0;
                                int OtrasBilleteras = 0;
                                int SinClasificar = 0;

                                if (class_RptRecepcion != null && class_RptRecepcion.Count > 0)
                                {
                                    Efectivo = class_RptRecepcion.Where(x => x.PacienteTelefono == "Efectivo").Sum(x => x.ValorReciboFactura);
                                    TarjetaCredito = class_RptRecepcion.Where(x => x.PacienteTelefono == "Tarjeta Credito").Sum(x => x.ValorReciboFactura);
                                    TarjetaDebito = class_RptRecepcion.Where(x => x.PacienteTelefono == "Tarjeta Debito").Sum(x => x.ValorReciboFactura);
                                    Nequi = class_RptRecepcion.Where(x => x.PacienteTelefono == "Nequi").Sum(x => x.ValorReciboFactura);
                                    Daviplata = class_RptRecepcion.Where(x => x.PacienteTelefono == "Daviplata").Sum(x => x.ValorReciboFactura);
                                    OtrasBilleteras = class_RptRecepcion.Where(x => x.PacienteTelefono == "Otras Billeteras").Sum(x => x.ValorReciboFactura);
                                    SinClasificar = class_RptRecepcion.Where(x => x.PacienteTelefono == "" || x.PacienteTelefono is null).Sum(x => x.ValorReciboFactura);

                                    foreach (var i in class_RptRecepcion)
                                    {
                                        i.Efectivo = Efectivo;
                                        i.TarjetaCredito = TarjetaCredito;
                                        i.Nequi = Nequi;
                                        i.TarjetaDebito = TarjetaDebito;
                                        i.Daviplata = Daviplata;
                                        i.OtrasBilleteras = OtrasBilleteras;
                                        i.SinClasificar = SinClasificar;
                                    }
                                }

                                return class_RptRecepcion;
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
        List<FacturacionReports> IVentas.Rpt_FacturasNC(DateTime Desde,
                                                 DateTime Hasta,
                                                 int Cia,
                                                 string Prestador,
                                                 string Tipo)
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
                
                    String Cargar_Hora = "SELECT F.FechaNC, F.OrdenPedido, F.Usuario, F.NumeroNC, C.VrTotal, C.Tipo " +
                                         "FROM CXN_CARGOSNC C " +
                                         "INNER JOIN CXN_FACTURANC F ON C.NumeroNC = F.NumeroNC " +
                                         "WHERE F.FechaNC BETWEEN '" + Convert.ToDateTime(Desde).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getData["Format_Fecha"]) + "' " +
                                         "AND F.Prestador = '" + Cia + "' " +
                                         "GROUP BY F.FechaNC, F.OrdenPedido, F.Usuario, F.NumeroNC, C.VrTotal, C.Tipo  " +
                                         "ORDER BY F.NumeroNC ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<FacturacionReports> Class_Fac_Serv1 = new List<FacturacionReports>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    Class_Fac_Serv1.Add(new FacturacionReports
                                    {
                                        FechaBase = Convert.ToDateTime(Lectura_Hora["FechaNC"]),
                                        Admision = Convert.ToInt32(Lectura_Hora["OrdenPedido"]),
                                        PacienteDireccion = Lectura_Hora["Usuario"].ToString(), //USUARIO QUE GRABA
                                        PacienteNombre = "NO DISPONIBLE",
                                        Efectivo = Convert.ToInt32(Lectura_Hora["VrTotal"]),
                                        PacienteAseguradora = "NO DISPONIBLE",
                                        Homologo = Lectura_Hora["NumeroNC"].ToString(),
                                        Desde = Convert.ToDateTime(Desde),
                                        Hasta = Convert.ToDateTime(Hasta),
                                        EmpresaNombre = Prestador.ToString(),
                                        Tipo = Lectura_Hora["Tipo"].ToString(),
                                        ProfesionalNombre = Prestador.ToString()
                                    });
                                }

                                int totalrpt = Class_Fac_Serv1.Sum(x => x.Efectivo);

                                var resultado = Class_Fac_Serv1.GroupBy(f => f.Admision).Select(g => new
                                                    {
                                                        FacNumFac = g.Key,
                                                        SumaVrTotal = g.Sum(x => x.Efectivo)
                                                    });                               

                                foreach (var f in Class_Fac_Serv1)
                                {
                                    var tot = resultado.FirstOrDefault(r => r.FacNumFac == f.Admision);

                                    f.TotalRpt = totalrpt;
                                    f.ValorReciboFactura = tot.SumaVrTotal;
                                }

                                Class_Fac_Serv1 = Class_Fac_Serv1
                                                 .GroupBy(f => f.Homologo)
                                                 .Select(g => g.First())
                                                 .ToList();

                                return Class_Fac_Serv1;
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
        int CalculoFacturas(DateTime Desde,
                            DateTime Hasta, int Compañia, string Tipo)
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
                    String Cargar_Hora = "SELECT SUM(V.Ven_Total) AS Val " +
                                         "FROM CXN_VENTAS V " +
                                         "WHERE V.Ven_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString(getDataCon["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getDataCon["Format_Fecha"]) + "' " +
                                         "AND V.Ven_Estado = 'F' " +
                                         "AND V.Ven_Cod_Cia = '" + Compañia + "' " +
                                         "AND V.Ven_Tipo_Doc = '" + Tipo + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return Convert.ToInt32(Lectura_Hora["Val"]);
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
        List<ReportesRecepcion> IVentas.Rpt_RecibosdeCaja(DateTime Desde,
                                                          DateTime Hasta,
                                                          int Cia,
                                                          string Prestador, 
                                                          bool Valores0)
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

                    String Cargar_Hora = "SELECT R.Rc_Caja_Fecha, R.Rc_Caja_Valor, A.Ase_Descripcion, " +
                                         "P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN as Nombre, R.Rc_Caja_Valor, R.Hor_DocFEModerador, " +
                                         "R.Rc_Caja_UsrGraba, R.FormaPago, R.Num_Cruce, " +
                                         "R.Rc_Caja_Adm, R.Rc_Id " +
                                         "FROM CXN_RC_CAJA R " +
                                         "INNER JOIN CXN_PACIENTES P ON R.Rc_Caja_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_ASEGURADORA A ON P.Pac_ASeguradora = A.Ase_IDentificador " +
                                         "WHERE R.Rc_Caja_Fecha BETWEEN @param1 AND @param2 " +
                                         "AND R.Rc_Caja_Cia = @param3 " +
                                         "ORDER BY R.Rc_Caja_Fecha ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Convert.ToDateTime(Desde.Date).ToString(getDataCon["Format_Fecha"]));
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(Hasta.Date).ToString(getDataCon["Format_Fecha"]));
                        Carga_Command.Parameters.AddWithValue("@param3", Cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<ReportesRecepcion> class_RptRecepcion = new List<ReportesRecepcion>();
                                var Tot = CalculoRcCaja(Desde.Date, Hasta.Date, Cia);

                                while (Lectura_Hora.Read() == true)
                                {
                                    if (Valores0 == true)
                                    {
                                        class_RptRecepcion.Add(new ReportesRecepcion
                                        {
                                            FechaBase = Convert.ToDateTime(Lectura_Hora["Rc_Caja_Fecha"]),
                                            ValorReciboFactura = Convert.ToInt32(Lectura_Hora["Rc_Caja_Valor"]),
                                            Admision = Convert.ToInt32(Lectura_Hora["Rc_Id"]),
                                            PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString() + " --> " + Lectura_Hora["Rc_Caja_Adm"].ToString(),
                                            Homologo = Lectura_Hora["Hor_DocFEModerador"].ToString(),
                                            PacienteNombre = Lectura_Hora["Nombre"].ToString(),
                                            Desde = Convert.ToDateTime(Desde),
                                            Hasta = Convert.ToDateTime(Hasta),
                                            TotalRpt = Convert.ToInt32(Tot),
                                            Tipo = "Recibo de Caja",
                                            EmpresaNombre = Prestador,
                                            ProfesionalNombre = Lectura_Hora["Rc_Caja_UsrGraba"].ToString(),
                                            PacienteTelefono = string.IsNullOrEmpty(Lectura_Hora["FormaPago"].ToString()) ? "" : Lectura_Hora["FormaPago"].ToString(),
                                            Num_Cruce = Convert.ToInt32(Lectura_Hora["Num_Cruce"]),
                                            TReport = "Recibos de Caja - Bonos"
                                        });
                                    }
                                    if (Valores0 == false)
                                    {
                                        if (Convert.ToInt32(Lectura_Hora["Rc_Caja_Valor"]) > 0)
                                        {
                                            class_RptRecepcion.Add(new ReportesRecepcion
                                            {
                                                FechaBase = Convert.ToDateTime(Lectura_Hora["Rc_Caja_Fecha"]),
                                                ValorReciboFactura = Convert.ToInt32(Lectura_Hora["Rc_Caja_Valor"]),
                                                Admision = Convert.ToInt32(Lectura_Hora["Rc_Id"]),
                                                PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString() + " --> " + Lectura_Hora["Rc_Caja_Adm"].ToString(),
                                                Homologo = Lectura_Hora["Hor_DocFEModerador"].ToString(),
                                                PacienteNombre = Lectura_Hora["Nombre"].ToString(),
                                                Desde = Convert.ToDateTime(Desde),
                                                Hasta = Convert.ToDateTime(Hasta),
                                                TotalRpt = Convert.ToInt32(Tot),
                                                Tipo = "Recibo de Caja",
                                                EmpresaNombre = Prestador,
                                                ProfesionalNombre = Lectura_Hora["Rc_Caja_UsrGraba"].ToString(),
                                                PacienteTelefono = string.IsNullOrEmpty(Lectura_Hora["FormaPago"].ToString()) ? "" : Lectura_Hora["FormaPago"].ToString(),
                                                Num_Cruce = Convert.ToInt32(Lectura_Hora["Num_Cruce"]),
                                                TReport = "Recibos de Caja - Bonos"
                                            });
                                        }
                                    }
                                }

                                int Efectivo = 0;
                                int TarjetaCredito = 0;
                                int TarjetaDebito = 0;
                                int Nequi = 0;
                                int Daviplata = 0;
                                int OtrasBilleteras = 0;
                                int SinClasificar = 0;

                                if (class_RptRecepcion != null && class_RptRecepcion.Count > 0 && Valores0 == false)
                                {
                                    Efectivo = class_RptRecepcion.Where(x => x.PacienteTelefono == "Efectivo").Sum(x => x.ValorReciboFactura);
                                    TarjetaCredito = class_RptRecepcion.Where(x => x.PacienteTelefono == "Tarjeta Credito").Sum(x => x.ValorReciboFactura);
                                    TarjetaDebito = class_RptRecepcion.Where(x => x.PacienteTelefono == "Tarjeta Debito").Sum(x => x.ValorReciboFactura);
                                    Nequi = class_RptRecepcion.Where(x => x.PacienteTelefono == "Nequi").Sum(x => x.ValorReciboFactura);
                                    Daviplata = class_RptRecepcion.Where(x => x.PacienteTelefono == "Daviplata").Sum(x => x.ValorReciboFactura);
                                    OtrasBilleteras = class_RptRecepcion.Where(x => x.PacienteTelefono == "Otras Billeteras").Sum(x => x.ValorReciboFactura);
                                    SinClasificar = class_RptRecepcion.Where(x => x.PacienteTelefono == "" || x.PacienteTelefono is null).Sum(x => x.ValorReciboFactura);

                                    foreach (var i in class_RptRecepcion)
                                    {
                                        i.Efectivo = Efectivo;
                                        i.TarjetaCredito = TarjetaCredito;
                                        i.Nequi = Nequi;
                                        i.TarjetaDebito = TarjetaDebito;
                                        i.Daviplata = Daviplata;
                                        i.OtrasBilleteras = OtrasBilleteras;
                                        i.SinClasificar = SinClasificar;
                                    }
                                }

                                return class_RptRecepcion;
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
        int CalculoRcCaja(DateTime Desde,
                          DateTime Hasta,
                          int Compañia)
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
                    String Cargar_Hora = "SELECT SUM(Rc_Caja_Valor) as Val " +
                                         "FROM CXN_RC_CAJA " +
                                         "WHERE CXN_RC_CAJA.Rc_Caja_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString(getDataCon["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getDataCon["Format_Fecha"]) + "' " +
                                         "AND CXN_RC_CAJA.Rc_Caja_Cia = '" + Compañia + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return Convert.ToInt32(Lectura_Hora["Val"]);
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
        bool IVentas.Anula_Recepcion(CXN_VENTAS V)
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

                    DateTime Hoy = DateTime.Now.Date;

                    string Busqueda = "UPDATE CXN_VENTAS " +
                                      "SET Ven_Estado = 'A', " +
                                      "Ven_Usr_Anula = '" + V.Ven_Usr_Anula + "', " +
                                      "Ven_Mot_Anula = '" + V.Ven_Mot_Anula + "', " +
                                      "Ven_Fecha_Anula = '" + Convert.ToDateTime(Hoy).ToString(getData["Format_Fecha"]) + "' " +
                                      "WHERE Ven_Factura = '" + V.Ven_Factura + "' " +
                                      "AND Ven_Cod_Cia = '" + V.Ven_Cod_Cia + "' " +
                                      "AND Ven_Tipo_Doc = '" + V.Ven_Tipo_Doc + "'";
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
    }
}
