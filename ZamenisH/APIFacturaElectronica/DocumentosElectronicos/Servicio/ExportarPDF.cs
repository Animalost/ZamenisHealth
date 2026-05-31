using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DocumentosElectronicos.Servicio
{
    public class ExportarPDF
    {
        private static readonly IGenerales repositorioGenerales = new MGenerales();
        private static readonly IFacturacion repositorioFacturacion = new MFacturacion();
        private static readonly IPacientes repositorioPacientes = new MPacientes();

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

        //Facturas
        public static List<FacturasR> ExportarFacturaAseguradoras(int Numero_Fac, int cia, string Tipo)
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
                    String Cargar_Hora = "SELECT Com_Nombre, Com_Identificador, Com_Direccion, Com_Telefono, Com_Logo, Com_Identificacion, Fac_Res, " +
                                         "Pac_PrimerN, Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, Pac_Direccion, Pac_Telefono, Pac_TelefonoAux, Pac_TipoId, " +
                                         "Pac_IdNum, Pac_Email, Ase_Descripcion, Ase_Identificador, Ase_NitCia, Ase_DVNitCia, Ase_Telefono, Ase_Direccion, Ase_Email, Fac_Num_Fac, Fac_Fecha, Fac_Fecha_Has, " +
                                         "Fac_Fecha_Des, Fac_Num_Aut, Fac_Descuento, Fac_Observa, Fac_Pac, Fac_Cia, Fac_Ase, Fac_Estado, Car_Factura, Car_Estado, Car_Cod, Fac_Usr_Graba, " +
                                         "Car_Item, Car_Val_Un, sum(cast(Car_Cant as int)) as Cantidad, sum(cast(Car_Val_Tot as int)) as Total, " +
                                         "Cufe, QRCufe, VrCompartido, Copago, Anticipo, CodPrestador, ContratoPoliza, Cobertura, ModPago, Homologo, Hora, Com_Resolucion_Electron, Com_Prefijo_Electron, Com_Doc_Electron, MedioPago, MetodoPago, DiasVencimiento, " +
                                         "PercentICA, PercentFUENTE, VrICA, VrFUENTE " +
                                         "FROM CXN_FACTURA " +
                                         "INNER JOIN CXN_CARGOS ON CXN_FACTURA.Fac_Num_Fac = CXN_CARGOS.Car_Factura " +
                                         "INNER JOIN CXN_ASEGURADORA ON CXN_FACTURA.Fac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA ON CXN_FACTURA.Fac_Cia = CXN_CIA.Com_Identificador " +
                                         "INNER JOIN CXN_PACIENTES ON CXN_FACTURA.Fac_Pac = CXN_PACIENTES.Pac_Id " +
                                         "WHERE CXN_FACTURA.Fac_Num_Fac = @param1 " +
                                         "AND CXN_FACTURA.Fac_Tipo_Doc = @param2 " +
                                         "AND CXN_FACTURA.Fac_Cia = @param3 " +
                                         "AND CXN_CARGOS.Car_Tipo_Doc = @param2 " +
                                         "AND CXN_CARGOS.Car_Cia = @param3 " +
                                         "GROUP BY Com_Nombre, Com_Identificador, Com_Direccion, Com_Telefono, Com_Logo, Com_Identificacion, Fac_Res, Pac_PrimerN, " +
                                         "Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, Pac_Direccion, Pac_Telefono, Pac_TelefonoAux, Pac_TipoId, Pac_IdNum, Pac_Email, Ase_Descripcion, Ase_Identificador, " +
                                         "Ase_NitCia, Ase_DVNitCia, Ase_Telefono, Ase_Direccion, Ase_Email, Fac_Num_Fac, Fac_Fecha, Fac_Fecha_Has, Fac_Fecha_Des, Fac_Num_Aut, Fac_Descuento, " +
                                         "Fac_Observa, Fac_Pac, Fac_Cia, Fac_Ase, Fac_Estado, Car_Factura, Car_Estado, Car_Cod, Fac_Usr_Graba, Car_Item, Car_Val_Un, " +
                                         "Cufe, QRCufe, VrCompartido, Copago, Anticipo, CodPrestador, ContratoPoliza, Cobertura, ModPago, Homologo, Hora, Com_Resolucion_Electron, Com_Prefijo_Electron, Com_Doc_Electron, MedioPago, MetodoPago, DiasVencimiento, " +
                                         "PercentICA, PercentFUENTE, VrICA, VrFUENTE ";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Numero_Fac);
                        Carga_Command.Parameters.AddWithValue("@param2", Tipo);
                        Carga_Command.Parameters.AddWithValue("@param3", cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<FacturasR> Class_FacServ1 = new List<FacturasR>();
                               
                                int Total = SubTotalAseguradoras(Numero_Fac, cia, Tipo);                             

                                string QrElectron = "Vacio";                               

                                while (Lectura_Hora.Read() == true)
                                {
                                    //qrcufe 
                                    if (!string.IsNullOrEmpty(Lectura_Hora["Cufe"].ToString()))
                                    {
                                        QrElectron = "NumFac:" + Lectura_Hora["Homologo"].ToString() + "\r\n" +
                                            "FecFac:" + Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]).ToString("yyyy-MM-dd") + "\r\n" +
                                            "HorFac:" + Convert.ToDateTime(Lectura_Hora["Hora"]).ToString("hh:mm:ss tt") + "\r\n" +
                                            "NitFac:" + Lectura_Hora["Com_Identificacion"].ToString() + "\r\n" +
                                            "DocAdq:" + Lectura_Hora["Ase_NitCia"].ToString() + "\r\n" +
                                            "ValFac:" + Convert.ToInt32(Total) + "\r\n" + //total antes de iva
                                            "ValIva:" + "0" + "\r\n" + //Total IVA 
                                            "ValOtroIm:" + "0" + "\r\n" +
                                            "ValTolFac:" + Convert.ToInt32(Total) + "\r\n" +
                                            "CUFE:" + Lectura_Hora["Cufe"].ToString() + "\r\n" +
                                            "https://catalogo-vpfe.dian.gov.co/document/searchqr?documentkey=" + Lectura_Hora["Cufe"].ToString();
                                    }

                                    Image Code_QR_Fac_CUFE = repositorioGenerales.CodifyQR(QrElectron);

                                    string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString(); //trae base64
                                    Byte[] bytes = Convert.FromBase64String(Bod_Firma1); //convierte a bytes
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = Image.FromStream(stmBLOBData);

                                    string Letra;
                                    int Neto;

                                    int DescuentosFactura = 0;
                                    DescuentosFactura = DescuentosFactura + (!string.IsNullOrEmpty(Lectura_Hora["Fac_Descuento"].ToString()) ? Convert.ToInt32(Lectura_Hora["Fac_Descuento"]) : 0);
                                    DescuentosFactura = DescuentosFactura + (!string.IsNullOrEmpty(Lectura_Hora["VrCompartido"].ToString()) ? Convert.ToInt32(Lectura_Hora["VrCompartido"]) : 0);
                                    DescuentosFactura = DescuentosFactura + (!string.IsNullOrEmpty(Lectura_Hora["Copago"].ToString()) ? Convert.ToInt32(Lectura_Hora["Copago"]) : 0);
                                    DescuentosFactura = DescuentosFactura + (!string.IsNullOrEmpty(Lectura_Hora["Anticipo"].ToString()) ? Convert.ToInt32(Lectura_Hora["Anticipo"]) : 0);

                                    Neto = Convert.ToInt32(Total) - Convert.ToInt32(DescuentosFactura);
                                    Letra = repositorioFacturacion.enletras(Convert.ToInt32(Neto).ToString()).ToUpper() + " PESOS";

                                    string percentfuente = string.IsNullOrEmpty(Lectura_Hora["PercentFUENTE"].ToString()) ? "0" : Lectura_Hora["PercentFUENTE"].ToString();
                                    string percentica = string.IsNullOrEmpty(Lectura_Hora["PercentICA"].ToString()) ? "0" : Lectura_Hora["PercentICA"].ToString();
                                    int vrfuente = string.IsNullOrEmpty(Lectura_Hora["VrFUENTE"].ToString()) ? 0 : Convert.ToInt32(Lectura_Hora["VrFUENTE"]);
                                    int vrica = string.IsNullOrEmpty(Lectura_Hora["VrICA"].ToString()) ? 0 : Convert.ToInt32(Lectura_Hora["VrICA"]);

                                    string TID = repositorioPacientes.getTipoDoc(Lectura_Hora["Pac_TipoId"].ToString());

                                    Class_FacServ1.Add(new FacturasR
                                    {
                                        Letras = Letra,
                                        Car_Cod = Lectura_Hora["Car_Cod"].ToString(),
                                        Car_Item = Lectura_Hora["Car_Item"].ToString(),
                                        Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        Car_Val_Un = Convert.ToInt32(Lectura_Hora["Car_Val_Un"]),
                                        Total = Convert.ToInt32(Lectura_Hora["Total"]),
                                        EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(),
                                        EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                        EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                        Fac_Num_Fac = Convert.ToInt32(Lectura_Hora["Fac_Num_Fac"]),
                                        PacienteNombre = Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString(),
                                        PacienteDireccion = Lectura_Hora["Pac_Direccion"].ToString(),
                                        PacienteIdentificacion = TID.ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                        PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                        PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                        Ase_NitCia = Lectura_Hora["Ase_NitCia"].ToString(),
                                        Ase_DVNitCia = Lectura_Hora["Ase_DVNitCia"].ToString(),
                                        FechaBase = Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]),
                                        Fac_Fecha_Des = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Des"]),
                                        Fac_Fecha_Has = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Has"]),
                                        Fac_Num_Aut = Lectura_Hora["Fac_Num_Aut"].ToString(),
                                        Ase_Telefono = Lectura_Hora["Ase_Telefono"].ToString(),
                                        Ase_Direccion = Lectura_Hora["Ase_Direccion"].ToString(),
                                        Fac_Res = Lectura_Hora["Fac_Res"].ToString(),
                                        Fac_Observa = Lectura_Hora["Fac_Observa"].ToString(),
                                        Fac_Descuento = DescuentosFactura,
                                        Fac_Total = Convert.ToInt32(Total),
                                        Fac_Neto = Convert.ToInt32(Neto),
                                        Usuario = Lectura_Hora["Fac_Usr_Graba"].ToString(),
                                        Code_QR = repositorioGenerales.GetBytes(Code_QR_Fac_CUFE),
                                        Com_Logo = repositorioGenerales.GetBytes(pic.Image),
                                        Cufe = Lectura_Hora["Cufe"].ToString(),
                                        VrCompartido = (Lectura_Hora["VrCompartido"] == DBNull.Value ? 0 : Convert.ToInt32(Lectura_Hora["VrCompartido"])),
                                        Copago = (Lectura_Hora["Copago"] == DBNull.Value ? 0 : Convert.ToInt32(Lectura_Hora["Copago"])),
                                        Anticipo = (Lectura_Hora["Anticipo"] == DBNull.Value ? 0 : Convert.ToInt32(Lectura_Hora["Anticipo"])),
                                        CodPrestador = Lectura_Hora["CodPrestador"].ToString(),
                                        ContratoPoliza = Lectura_Hora["ContratoPoliza"].ToString(),
                                        Cobertura = Lectura_Hora["Cobertura"].ToString(),
                                        ModPago = Lectura_Hora["ModPago"].ToString(),
                                        QRCufe = repositorioGenerales.GetBytes(Code_QR_Fac_CUFE),
                                        EmpresaNombre = Lectura_Hora["Homologo"].ToString(),

                                        Admision = Convert.ToInt32(Lectura_Hora["Ase_Identificador"]),
                                        Com_Direccion = TID.ToString(),
                                        DocE_1 = Lectura_Hora["Pac_IdNum"].ToString(),
                                        DocE_2 = Lectura_Hora["Pac_Email"].ToString(),
                                        DocE_3 = Lectura_Hora["Ase_Email"].ToString(),
                                        DocE_4 = Lectura_Hora["Fac_Res"].ToString(),
                                        ReteFuente = Lectura_Hora["Pac_TipoId"].ToString(),
                                        ProfesionalNombre = Letras(Lectura_Hora["Homologo"].ToString()),
                                        DocE_5 = Convert.ToInt32(Numeros(Lectura_Hora["Homologo"].ToString())),

                                        Dias = (Lectura_Hora["DiasVencimiento"] == DBNull.Value ? 30 : Convert.ToInt32(Lectura_Hora["DiasVencimiento"])),
                                        MedioP = Lectura_Hora["MedioPago"].ToString(),
                                        MetodoP = Lectura_Hora["MetodoPago"].ToString(),
                                        NombrePrestador = Lectura_Hora["Com_Nombre"].ToString(),

                                        PercentFUENTE = percentfuente,
                                        PercentICA = percentica,
                                        VrFUENTE = vrfuente,
                                        VrICA = vrica,

                                        LetrasImpuestos = repositorioFacturacion.enletras(Convert.ToInt32(Neto - vrfuente - vrica).ToString()).ToUpper() + " PESOS",
                                        VrNeto = Neto - vrfuente - vrica
                                    });
                                }
                              
                                return Class_FacServ1;
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "Exportar PDF", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        public static List<RCCAJA> ExportarReciboCaja(int IdPos)
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

                    String Cargar_Hora = "SELECT C.Com_Logo, P.Pac_PrimerA, P.Pac_PrimerN, P.Pac_SegundoA, P.Pac_SegundoN, P.Pac_TipoId, P.Pac_IdNum, " +
                                         "P.Pac_Telefono, P.Pac_TelefonoAux, P.Pac_Direccion, A.Ase_Descripcion, C.Com_Nombre, C.Com_Identificacion, " +
                                         "C.Com_Direccion, C.Com_Telefono, C.Com_Resolucion_Electron, C.Com_Identificador, C.Com_Prefijo_Electron, C.Com_Doc_Electron, R.Rc_Caja_UsrGraba, " +
                                         "R.Rc_Caja_Fecha, R.Rc_Caja_Observacion, R.Rc_Caja_Valor, R.Hor_DocFEModerador, P.Pac_Email, " +
                                         "R.Hor_DocFEModeradorCUFE, R.Hor_DocFEModeradorRes, A.Ase_Identificador, R.Hor_DocFEModeradorFechaHora, R.Rc_Id, R.Rc_Caja_Adm " +
                                         "FROM CXN_RC_CAJA R " +
                                         "INNER JOIN CXN_PACIENTES P ON R.Rc_Caja_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_ASEGURADORA A ON R.Rc_Caja_Ase = A.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA C ON R.Rc_Caja_Cia = C.Com_Identificador " +
                                         "WHERE R.Rc_Id = @param1 " +
                                         "ORDER BY Rc_Id DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", IdPos);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<RCCAJA> Class_RCCaja1 = new List<RCCAJA>();

                                if (Lectura_Hora.Read() == true)
                                {
                                    string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString();
                                    Byte[] bytes = Convert.FromBase64String(Bod_Firma1);
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = Image.FromStream(stmBLOBData);

                                    string tempCad1 = "NumFac:" + Lectura_Hora["Hor_DocFEModerador"].ToString();
                                    string tempCad2 = "FecFac:" + Convert.ToDateTime(Lectura_Hora["Rc_Caja_Fecha"]).ToString("yyyy-MM-dd");
                                    string tempCad3 = "HorFac:" + Convert.ToDateTime(Lectura_Hora["Rc_Caja_Fecha"]).ToString("hh:mm:ss tt");
                                    string tempCad4 = "NitFac:" + Lectura_Hora["Com_Identificacion"].ToString();
                                    string tempCad5 = "DocAdq:" + Lectura_Hora["Pac_IdNum"].ToString();
                                    string tempCad6 = "ValFac:" + Convert.ToInt32(Lectura_Hora["Rc_Caja_Valor"]).ToString("N0");
                                    string tempCad7 = "ValIva:" + "0";
                                    string tempCad8 = "ValOtroIm:" + "0";
                                    string tempCad9 = "ValTolFac:" + Convert.ToInt32(Lectura_Hora["Rc_Caja_Valor"]).ToString("N0");
                                    string tempCad10 = (Lectura_Hora["Hor_DocFEModeradorCUFE"].ToString() == "" ? "CUFE:" : "CUFE:" + Lectura_Hora["Hor_DocFEModeradorCUFE"].ToString());
                                    string tempCad11 = "https://catalogo-vpfe.dian.gov.co/document/searchqr?documentkey=" + Lectura_Hora["Hor_DocFEModeradorCUFE"].ToString();

                                    string QRElectron = tempCad1 + "\r\n" +
                                                        tempCad2 + "\r\n" +
                                                        tempCad3 + "\r\n" +
                                                        tempCad4 + "\r\n" +
                                                        tempCad5 + "\r\n" +
                                                        tempCad6 + "\r\n" + //total antes de iva
                                                        tempCad7 + "\r\n" + //Total IVA 
                                                        tempCad8 + "\r\n" +
                                                        tempCad9 + "\r\n" +
                                                        tempCad10 + "\r\n" +
                                                        tempCad11;

                                    Image Code_QR_Fac = repositorioGenerales.CodifyQR(QRElectron);

                                    DateTime fooDate = Convert.ToDateTime(Lectura_Hora["Rc_Caja_Fecha"]);
                                    if (!string.IsNullOrEmpty(Lectura_Hora["Hor_DocFEModeradorFechaHora"].ToString()))
                                    {
                                        fooDate = Convert.ToDateTime(Lectura_Hora["Hor_DocFEModeradorFechaHora"].ToString());
                                    }

                                    Class_RCCaja1.Add(new RCCAJA
                                    {
                                        PacienteNombre = Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString(),
                                        PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                        PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString() + " " + Lectura_Hora["Pac_TelefonoAux"].ToString(),
                                        PacienteDireccion = Lectura_Hora["Pac_Direccion"].ToString(),
                                        PacienteAseguradora = Lectura_Hora["Pac_IdNum"].ToString(),
                                        EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                        EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                        EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(),
                                        EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                        Vendedor = Lectura_Hora["Rc_Caja_UsrGraba"].ToString(),
                                        FechaBase = Convert.ToDateTime(Lectura_Hora["Rc_Caja_Fecha"]),
                                        Recibo = Convert.ToInt32(Lectura_Hora["Rc_Caja_Adm"]),
                                        Cantidad = 1,
                                        Observacion = Lectura_Hora["Rc_Caja_Observacion"].ToString(),
                                        Logo = repositorioGenerales.GetBytes(pic.Image),
                                        Valor = Convert.ToInt32(Lectura_Hora["Rc_Caja_Valor"]),
                                        ProfesionalNombre = Lectura_Hora["Hor_DocFEModerador"].ToString(),
                                        Letras = repositorioFacturacion.enletras(Convert.ToInt32(Lectura_Hora["Rc_Caja_Valor"]).ToString()).ToUpper() + " PESOS",
                                        QRImage = repositorioGenerales.GetBytes(Code_QR_Fac),
                                        Correo = Lectura_Hora["Pac_Email"].ToString(),
                                        Com_Direccion = Lectura_Hora["Hor_DocFEModeradorCUFE"].ToString() == "" ? " " : Lectura_Hora["Hor_DocFEModeradorCUFE"].ToString(), //cufe
                                        Resolucion = Lectura_Hora["Hor_DocFEModeradorRes"].ToString(),

                                        TDocReceptor = Lectura_Hora["Pac_TipoId"].ToString(),
                                        DocReceptor = Lectura_Hora["Pac_IdNum"].ToString(),
                                        AseIdentificator = Convert.ToInt32(Lectura_Hora["Ase_Identificador"]),
                                        ResElectron = Lectura_Hora["Hor_DocFEModeradorRes"].ToString(),
                                        PrefijoElectron = string.IsNullOrEmpty(Lectura_Hora["Hor_DocFEModerador"].ToString()) ? "" : Letras(Lectura_Hora["Hor_DocFEModerador"].ToString()),
                                        NumeroElectron = string.IsNullOrEmpty(Lectura_Hora["Hor_DocFEModerador"].ToString()) ? 0 : Convert.ToInt32(Numeros(Lectura_Hora["Hor_DocFEModerador"].ToString())),
                                        FechaRealElectron = Convert.ToDateTime(fooDate),
                                        Admision = Convert.ToInt32(Lectura_Hora["Com_Identificador"]),
                                        RC_ID = Convert.ToInt32(Lectura_Hora["Rc_Id"])
                                    });

                                    return Class_RCCaja1;
                                }
                                else
                                {
                                    return null;
                                }
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "ExportarPDF", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        public static List<FacturacionRpt> ExportarFacturaVentas(int Docu_Ven, int cia, string Tipo)
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
                                    QRReal = repositorioGenerales.CodifyQR(textoQR);
                                }

                                while (Lectura_Hora.Read() == true)
                                {
                                    string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString(); //trae base64
                                    Byte[] bytes = Convert.FromBase64String(Bod_Firma1); //convierte a bytes
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = Image.FromStream(stmBLOBData);


                                    string percentretefuente = string.IsNullOrEmpty(Lectura_Hora["PercentFUENTE"].ToString()) ? "0" : Lectura_Hora["PercentFUENTE"].ToString();
                                    string percentreteica = string.IsNullOrEmpty(Lectura_Hora["PercentICA"].ToString()) ? "0" : Lectura_Hora["PercentICA"].ToString();
                                    int vrfuente = string.IsNullOrEmpty(Lectura_Hora["VrFUENTE"].ToString()) ? 0 : Convert.ToInt32(Lectura_Hora["VrFUENTE"]);
                                    int vrica = string.IsNullOrEmpty(Lectura_Hora["VrICA"].ToString()) ? 0 : Convert.ToInt32(Lectura_Hora["VrICA"]);

                                    string Letra;
                                    
                                    Letra = repositorioFacturacion.enletras(Convert.ToInt32(SubT).ToString()).ToUpper() + " PESOS";

                                    int OP = Convert.ToInt32(Lectura_Hora["Ven_Factura"].ToString());
                                    string NumFacturaReal = Lectura_Hora["Ven_Homologo"].ToString();
                               

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
                                        Logo = repositorioGenerales.GetBytes(pic.Image),
                                        Resolucion = Lectura_Hora["Ven_Res"].ToString(),
                                        CUFE = Lectura_Hora["Cufe"].ToString(),
                                        Dcto = Convert.ToInt32(Lectura_Hora["Ven_Dcto"]),
                                        QRLogo = (QRReal != null ? repositorioGenerales.GetBytes(QRReal) : null),

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

                                        Admision = OP, // Num OP Real

                                        VrFUENTE = vrfuente,
                                        VrICA = vrica,
                                        PercentFUENTE = percentretefuente,
                                        PercentICA = percentreteica,

                                        LetraImpuestos = repositorioFacturacion.enletras(Convert.ToInt32(SubT - vrfuente - vrica).ToString()).ToUpper() + " PESOS",
                                        TotalImpuestos = SubT - vrfuente - vrica
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
        public static List<FacturasR> ExportarFacturaOtras(int Numero_Fac, int cia, string Tipo, bool MostrarImpuestos)
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

                    String Cargar_Hora = "SELECT Com_Nombre, Com_Identificador, Com_Email, Com_Direccion, Com_Telefono, Com_Logo, Com_Identificacion, Fac_Res, " +
                                         "Ase_Descripcion, Ase_Identificador, Ase_NitCia, Ase_DVNitCia, Ase_Telefono, Ase_Direccion, Ase_Email, Fac_Num_Fac, Fac_Fecha, Fac_Fecha_Has, " +
                                         "Fac_Fecha_Des, Fac_Num_Aut, Fac_Descuento, Fac_Observa, Car_ICA, Fac_Pac, Fac_Cia, Fac_Ase, Fac_Estado, Car_Factura, Car_RetFte, Car_Estado, Car_Cod, Fac_Usr_Graba, " +
                                         "Car_Item, Car_Val_Un, Car_IVA, Car_Cant, Car_Detalle, sum(cast(Car_Cant as int)) as Cantidad, sum(cast(Car_Val_Tot as int)) as Total, " +
                                         "Cufe, QRCufe, VrCompartido, Copago, Anticipo, CodPrestador, ContratoPoliza, Cobertura, ModPago, Homologo, Hora, Com_Resolucion_Electron, Com_Prefijo_Electron, Com_Doc_Electron, MedioPago, MetodoPago, DiasVencimiento " +
                                         "FROM CXN_FACTURA " +
                                         "INNER JOIN CXN_CARGOS ON CXN_FACTURA.Fac_Num_Fac = CXN_CARGOS.Car_Factura " +
                                         "INNER JOIN CXN_ASEGURADORA ON CXN_FACTURA.Fac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA ON CXN_FACTURA.Fac_Cia = CXN_CIA.Com_Identificador " +
                                         "WHERE CXN_FACTURA.Fac_Num_Fac = @param1 " +
                                         "AND CXN_FACTURA.Fac_Tipo_Doc = @param2 " +
                                         "AND CXN_FACTURA.Fac_Cia = @param3 " +
                                         "AND CXN_CARGOS.Car_Tipo_Doc = @param2 " +
                                         "AND CXN_CARGOS.Car_Cia = @param3 " +
                                         "GROUP BY Com_Nombre, Com_Identificador, Com_Email, Com_Direccion, Com_Telefono, Com_Logo, Com_Identificacion, Fac_Res, " +
                                         "Ase_Descripcion, Ase_Identificador, " +
                                         "Ase_NitCia, Ase_DVNitCia, Ase_Telefono, Ase_Direccion, Ase_Email, Fac_Num_Fac, Fac_Fecha, Fac_Fecha_Has, Fac_Fecha_Des, Fac_Num_Aut, Fac_Descuento, " +
                                         "Fac_Observa, Fac_Pac, Fac_Cia, Fac_Ase, Car_ICA, Fac_Estado, Car_Factura, Car_RetFte, Car_Detalle, Car_Estado, Car_Cod, Fac_Usr_Graba, Car_Item, Car_Val_Un, Car_Cant, Car_IVA, " +
                                         "Cufe, QRCufe, VrCompartido, Copago, Anticipo, CodPrestador, ContratoPoliza, Cobertura, ModPago, Homologo, Hora, Com_Resolucion_Electron, Com_Prefijo_Electron, Com_Doc_Electron, MedioPago, MetodoPago, DiasVencimiento";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Numero_Fac);
                        Carga_Command.Parameters.AddWithValue("@param2", Tipo);
                        Carga_Command.Parameters.AddWithValue("@param3", cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<FacturasR> Class_FacServ1 = new List<FacturasR>();

                                Dictionary<int, decimal> Subtotales = new Dictionary<int, decimal>();
                                int Contador = 1;
                                int VrBrutoAcumulado = 0;
                                decimal VrIva = 0;

                                while (Lectura_Hora.Read() == true)
                                {
                                    DateTime Hora = Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]);

                                    if (!string.IsNullOrEmpty(Lectura_Hora["Hora"].ToString()))
                                    {
                                        Hora = Convert.ToDateTime(Lectura_Hora["Hora"]);
                                    }

                                    //Valor Bruto Producto
                                    decimal VrBrutoProducto = Math.Round(Convert.ToDecimal(Lectura_Hora["Car_Val_Un"]) * Convert.ToInt32(Lectura_Hora["Car_Cant"]), 2);
                                    VrBrutoAcumulado = VrBrutoAcumulado + (int)VrBrutoProducto;
                                    //IVA Producto
                                    decimal IVAProducto = Convert.ToDecimal(Lectura_Hora["Car_IVA"]);
                                    //Valor IVA Producto
                                    decimal ValorIVAProducto = Math.Round((VrBrutoProducto * IVAProducto) / 100, 2);
                                    if (IVAProducto > 0)
                                    {
                                        VrIva = VrIva + ValorIVAProducto;
                                    }                                                                       
                                    //ReteFuente Producto
                                    decimal ReteFuenteProducto = Convert.ToDecimal(Lectura_Hora["Car_RetFte"]);
                                    //Valor ReteFuente Producto
                                    decimal ValorReteFuenteProducto = Math.Round((VrBrutoProducto * ReteFuenteProducto) / 100, 2);
                                    //Retencion del ICA 8.66/1000", "9.66/1000
                                    string valorICA = Lectura_Hora["Car_ICA"]?.ToString()?.Trim();
                                    decimal ReteICAProducto =
                                                            valorICA == "7.12/1000" ? 0.00712m :
                                                            valorICA == "8.66/1000" ? 0.00866m :
                                                            valorICA == "9.66/1000" ? 0.00966m :
                                                            0.00m;
                                    //Valor retencion del ICA
                                    decimal ValorRetencionICA = Math.Round(VrBrutoProducto * ReteICAProducto, 2);

                                    //Valor Neto Producto
                                    decimal ValorNetoProducto = 0;
                                    if (MostrarImpuestos == true)
                                    {
                                        ValorNetoProducto = Math.Round(VrBrutoProducto + ValorIVAProducto - ValorReteFuenteProducto - ValorRetencionICA, 2);
                                    }
                                    else
                                    {
                                        ValorNetoProducto = Math.Round(VrBrutoProducto + ValorIVAProducto, 2);
                                    }

                                    //Descuentos Factura
                                    decimal DescuentosFactura = Math.Round(string.IsNullOrEmpty(Lectura_Hora["Fac_Descuento"].ToString()) ? 0 : Convert.ToDecimal(Lectura_Hora["Fac_Descuento"]), 2);

                                    if (MostrarImpuestos == true)
                                    {
                                        Subtotales.Add(Contador, Convert.ToDecimal(VrBrutoProducto + (ValorIVAProducto / 100) - (ValorReteFuenteProducto / 100) - ValorRetencionICA));
                                    }
                                    else
                                    {
                                        Subtotales.Add(Contador, Convert.ToDecimal(VrBrutoProducto + (ValorIVAProducto / 100)));
                                    }

                                    Contador++;

                                    string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString(); //trae base64
                                    Byte[] bytes = Convert.FromBase64String(Bod_Firma1); //convierte a bytes
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = Image.FromStream(stmBLOBData);

                                    //string TID = repoPacs.getTipoDoc(Lectura_Hora["Pac_TipoId"].ToString());

                                    Class_FacServ1.Add(new FacturasR
                                    {
                                        Car_Cod = Lectura_Hora["Car_Cod"].ToString(),
                                        Car_Item = Lectura_Hora["Car_Item"].ToString(),
                                        Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        Car_Val_Un = Convert.ToInt32(Lectura_Hora["Car_Val_Un"]),
                                        Total = Convert.ToInt32(VrBrutoProducto + (ValorIVAProducto / 100) - (ValorReteFuenteProducto / 100) - ValorRetencionICA),
                                        IVA = Lectura_Hora["Car_IVA"].ToString(),
                                        ReteFuente = Lectura_Hora["Car_RetFte"].ToString(),
                                        ValorIVA = ValorIVAProducto / 100,
                                        ValorReteFuente = ValorReteFuenteProducto / 100,
                                        ValorReteICA = ValorRetencionICA,
                                        DocE_4 = Lectura_Hora["Car_Detalle"].ToString(),

                                        EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(),
                                        EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                        EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                        EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                        DocE_1 = Lectura_Hora["Com_Email"].ToString(),
                                        FechaBase = Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]),

                                        Fac_Num_Fac = Convert.ToInt32(Lectura_Hora["Fac_Num_Fac"]),

                                        PacienteNombre = Lectura_Hora["Ase_Descripcion"].ToString(),
                                        PacienteDireccion = Lectura_Hora["Ase_Direccion"].ToString(),
                                        PacienteIdentificacion = Lectura_Hora["Ase_NitCia"].ToString() + "-" + Lectura_Hora["Ase_DVNitCia"].ToString(),
                                        PacienteTelefono = Lectura_Hora["Ase_Telefono"].ToString(),
                                        Ase_Direccion = Lectura_Hora["Ase_Email"].ToString(),

                                        Fac_Observa = Lectura_Hora["Fac_Observa"].ToString(),
                                        Fac_Descuento = Convert.ToInt32(DescuentosFactura),

                                        Com_Logo = repositorioGenerales.GetBytes(pic.Image),

                                        Admision = Convert.ToInt32(Lectura_Hora["Ase_Identificador"]),
                                        //Com_Direccion = TID.ToString(),
                                        DocE_2 = Lectura_Hora["Ase_NitCia"].ToString(),
                                        DocE_3 = Lectura_Hora["Ase_DVNitCia"].ToString(),
                                        Fac_Res = Lectura_Hora["Fac_Res"].ToString(),
                                        ProfesionalNombre = Letras(Lectura_Hora["Homologo"].ToString()),
                                        DocE_5 = Convert.ToInt32(Numeros(Lectura_Hora["Homologo"].ToString())),
                                        Dias = (Lectura_Hora["DiasVencimiento"] == DBNull.Value ? 30 : Convert.ToInt32(Lectura_Hora["DiasVencimiento"])),
                                        MedioP = Lectura_Hora["MedioPago"].ToString(),
                                        MetodoP = Lectura_Hora["MetodoPago"].ToString(),
                                        Ase_Telefono = Lectura_Hora["Homologo"].ToString(),
                                        Cufe = Lectura_Hora["Cufe"].ToString(),

                                        Fac_Fecha_Des = Convert.ToDateTime(Hora),
                                        Cobertura = Lectura_Hora["Homologo"].ToString(), //numero real facelectron
                                        VrCompartido = Convert.ToInt32(VrBrutoProducto + (ValorIVAProducto / 100)), //vr total producto solo iva
                                    });
                                }

                                //sumar vr compartido es decir vr x prod con iva para obenter el total con iva
                                int sumTemp = 0;
                                foreach (FacturasR i in Class_FacServ1)
                                {
                                    sumTemp = sumTemp + i.VrCompartido;
                                }

                                sumTemp = sumTemp - Class_FacServ1[0].Fac_Descuento; //descuentos de la factura

                                if (VrIva > 0)
                                {
                                    VrIva = VrIva / 100;
                                }

                                string QrElectron = "NumFac:" + Class_FacServ1[0].Cobertura + "\r\n" +
                                                    "FecFac:" + Convert.ToDateTime(Class_FacServ1[0].FechaBase).ToString("yyyy-MM-dd") + "\r\n" +
                                                    "HorFac:" + Convert.ToDateTime(Class_FacServ1[0].Fac_Fecha_Des).ToString("hh:mm:ss tt") + "\r\n" +
                                                    "NitFac:" + Class_FacServ1[0].EmpresaIdentificacion.ToString() + "\r\n" +
                                                    "DocAdq:" + Class_FacServ1[0].DocE_2 + "\r\n" +
                                                    "ValFac:" + VrBrutoAcumulado.ToString("N2", CultureInfo.InvariantCulture) + "\r\n" + //total antes de iva
                                                    "ValIva:" + VrIva.ToString("N2", CultureInfo.InvariantCulture) + "\r\n" + //Total IVA 
                                                    "ValOtroIm:" + "0.00" + "\r\n" +
                                                    "ValTolFac:" + sumTemp.ToString("N2", CultureInfo.InvariantCulture) + "\r\n" +
                                                    "CUFE:" + Class_FacServ1[0].Cufe + "\r\n" +
                                                    "https://catalogo-vpfe.dian.gov.co/document/searchqr?documentkey=" + Class_FacServ1[0].Cufe;


                                Image Code_QR_Fac_CUFE = repositorioGenerales.CodifyQR(QrElectron);

                                foreach (FacturasR i in Class_FacServ1)
                                {
                                    i.Fac_Total = Convert.ToInt32(Subtotales.Values.Sum()); //subtotal
                                    i.Fac_Neto = Convert.ToInt32(Subtotales.Values.Sum()) - Class_FacServ1[0].Fac_Descuento;
                                    i.Letras = repositorioFacturacion.enletras(Convert.ToInt32(Subtotales.Values.Sum() - Class_FacServ1[0].Fac_Descuento).ToString().ToUpper()) + " PESOS M/C";
                                    i.Car_Val_Tot = VrBrutoAcumulado; //subtotal para la factura sin iva
                                    i.Anticipo = sumTemp; //total con iva
                                    i.Usuario = repositorioFacturacion.enletras(Convert.ToInt32(sumTemp).ToString());
                                    i.Code_QR = repositorioGenerales.GetBytes(Code_QR_Fac_CUFE);
                                }

                                return Class_FacServ1;
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "ExportarPDF", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }


        //Totales y QR
        static string getQRVentas(string Docu_Ven, int cia, string Tipo)
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
        static int SubTotalRec(int Numero_Fac, int cia, string Tipo)
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "Exportar PDF", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }
        static int SubTotalAseguradoras(int Numero_Fac, int cia, string Tipo)
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
                    String Cargar_Hora = "SELECT Car_Factura, sum(cast(Car_Val_Tot as int)) as Total " +
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Factura = @param1 " +
                                         "AND Car_Tipo_Doc = @param2 " +
                                         "AND Car_Cia = @param3 " +
                                         "GROUP BY Car_Factura";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Numero_Fac);
                        Carga_Command.Parameters.AddWithValue("@param2", Tipo);
                        Carga_Command.Parameters.AddWithValue("@param3", cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Total"]);
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "Exportar PDF", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }





        //Notas Credito
        //Facturas
        public static List<FacturasR> ExportarFacturaAseguradorasNC(string NumeroNC, int cia)
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
                    String Cargar_Hora = "SELECT Com_Nombre, Com_Identificador, Com_Direccion, Com_Telefono, Com_Logo, Com_Identificacion, Fac_Res, " +
                                         "Pac_PrimerN, Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, Pac_Direccion, Pac_Telefono, Pac_TelefonoAux, Pac_TipoId, " +
                                         "Pac_IdNum, Pac_Email, Ase_Descripcion, Ase_Identificador, Ase_NitCia, Ase_DVNitCia, Ase_Telefono, Ase_Direccion, Ase_Email, Fac_Num_Fac, Fac_Fecha, FechaNC, Fac_Fecha_Has, " +
                                         "Fac_Fecha_Des, Fac_Num_Aut, Fac_Descuento, Fac_Observa, Fac_Pac, Fac_Cia, Fac_Ase, Fac_Estado, Fac_Usr_Graba, " +
                                         "CXN_FACTURANC.Cufe, QRCufe, VrCompartido, Copago, Anticipo, CodPrestador, ContratoPoliza, Cobertura, ModPago, Homologo, Hora, Com_Resolucion_Electron, Com_Prefijo_Electron, Com_Doc_Electron, MedioPago, MetodoPago, DiasVencimiento, " +
                                         "Codigo, Item, Cantidad, VrUnitario, VrTotal, CXN_FACTURANC.NumeroNC " +
                                         "FROM CXN_FACTURA " +
                                         "INNER JOIN CXN_FACTURANC ON CXN_FACTURA.Fac_Num_Fac = CXN_FACTURANC.OrdenPedido " +
                                         "INNER JOIN CXN_CARGOSNC ON CXN_FACTURANC.NumeroNC = CXN_CARGOSNC.NumeroNC " +
                                         "INNER JOIN CXN_ASEGURADORA ON CXN_FACTURA.Fac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA ON CXN_FACTURA.Fac_Cia = CXN_CIA.Com_Identificador " +
                                         "INNER JOIN CXN_PACIENTES ON CXN_FACTURA.Fac_Pac = CXN_PACIENTES.Pac_Id " +
                                         "AND CXN_FACTURA.Homologo = CXN_FACTURANC.FacturaElectronica " +
                                         "WHERE CXN_FACTURANC.NumeroNC = @param1 " +
                                         "AND CXN_FACTURA.Fac_Tipo_Doc = @param2 " +
                                         "AND CXN_FACTURA.Fac_Cia = @param3 " +
                                         "AND CXN_CARGOSNC.NumeroNC = @param1 " +
                                         "GROUP BY Com_Nombre, Com_Identificador, Com_Direccion, Com_Telefono, Com_Logo, Com_Identificacion, Fac_Res, " +
                                         "Pac_PrimerN, Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, Pac_Direccion, Pac_Telefono, Pac_TelefonoAux, Pac_TipoId, " +
                                         "Pac_IdNum, Pac_Email, Ase_Descripcion, Ase_Identificador, Ase_NitCia, Ase_DVNitCia, Ase_Telefono, Ase_Direccion, Ase_Email, Fac_Num_Fac, Fac_Fecha, FechaNC, Fac_Fecha_Has, " +
                                         "Fac_Fecha_Des, Fac_Num_Aut, Fac_Descuento, Fac_Observa, Fac_Pac, Fac_Cia, Fac_Ase, Fac_Estado, Fac_Usr_Graba, " +
                                         "CXN_FACTURANC.Cufe, QRCufe, VrCompartido, Copago, Anticipo, CodPrestador, ContratoPoliza, Cobertura, ModPago, Homologo, Hora, Com_Resolucion_Electron, Com_Prefijo_Electron, Com_Doc_Electron, MedioPago, MetodoPago, DiasVencimiento, " +
                                         "Codigo, Item, Cantidad, VrUnitario, VrTotal, CXN_FACTURANC.NumeroNC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", NumeroNC);
                        Carga_Command.Parameters.AddWithValue("@param2", "OP");
                        Carga_Command.Parameters.AddWithValue("@param3", cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<FacturasR> Class_FacServ1 = new List<FacturasR>();

                                int Total = SubTotalAseguradorasNC(NumeroNC, "Salud");

                                string QrElectron = "Vacio";

                                while (Lectura_Hora.Read() == true)
                                {
                                    //qrcufe 
                                    if (!string.IsNullOrEmpty(Lectura_Hora["Cufe"].ToString()))
                                    {
                                        QrElectron = "https://catalogo-vpfe.dian.gov.co/document/searchqr?documentkey=" + Lectura_Hora["Cufe"].ToString();
                                    }

                                    Image Code_QR_Fac_CUFE = repositorioGenerales.CodifyQR(QrElectron);

                                    string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString(); //trae base64
                                    Byte[] bytes = Convert.FromBase64String(Bod_Firma1); //convierte a bytes
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = Image.FromStream(stmBLOBData);

                                    string Letra;
                                    
                                    Letra = repositorioFacturacion.enletras(Convert.ToInt32(Total).ToString()).ToUpper() + " PESOS";

                                    string TID = repositorioPacientes.getTipoDoc(Lectura_Hora["Pac_TipoId"].ToString());

                                    Class_FacServ1.Add(new FacturasR
                                    {
                                        Letras = Letra,
                                        Car_Cod = Lectura_Hora["Codigo"].ToString(),
                                        Car_Item = Lectura_Hora["Item"].ToString(),
                                        Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        Car_Val_Un = Convert.ToInt32(Lectura_Hora["VrUnitario"]),
                                        Total = Convert.ToInt32(Lectura_Hora["VrTotal"]),
                                        EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(),
                                        EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                        EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                        Fac_Num_Fac = Convert.ToInt32(Lectura_Hora["Fac_Num_Fac"]),
                                        PacienteNombre = Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString(),
                                        PacienteDireccion = Lectura_Hora["Pac_Direccion"].ToString(),
                                        PacienteIdentificacion = TID.ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                        PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                        PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                        Ase_NitCia = Lectura_Hora["Ase_NitCia"].ToString(),
                                        Ase_DVNitCia = Lectura_Hora["Ase_DVNitCia"].ToString(),
                                        FechaBase = Convert.ToDateTime(Lectura_Hora["FechaNC"]),
                                        Fac_Fecha_Des = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Des"]),
                                        Fac_Fecha_Has = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Has"]), 
                                        Fac_Num_Aut = Lectura_Hora["Fac_Num_Aut"].ToString(),
                                        Ase_Telefono = Lectura_Hora["Ase_Telefono"].ToString(),
                                        Ase_Direccion = Lectura_Hora["Ase_Direccion"].ToString(),
                                        Fac_Res = Lectura_Hora["Fac_Res"].ToString(),
                                        Fac_Observa = Lectura_Hora["Fac_Observa"].ToString() + "\n\n" + " Nota Credito Numero " + NumeroNC + " asociada a la Factura Electronica Numero " + Lectura_Hora["Homologo"].ToString(),
                                        Fac_Total = Convert.ToInt32(Total),
                                        Usuario = Lectura_Hora["Fac_Usr_Graba"].ToString(),
                                        Code_QR = repositorioGenerales.GetBytes(Code_QR_Fac_CUFE),
                                        Com_Logo = repositorioGenerales.GetBytes(pic.Image),
                                        Cufe = Lectura_Hora["Cufe"].ToString(),
                                        CodPrestador = Lectura_Hora["CodPrestador"].ToString(),
                                        ContratoPoliza = Lectura_Hora["ContratoPoliza"].ToString(),
                                        Cobertura = Lectura_Hora["Cobertura"].ToString(),
                                        ModPago = Lectura_Hora["ModPago"].ToString(),
                                        QRCufe = repositorioGenerales.GetBytes(Code_QR_Fac_CUFE),
                                        EmpresaNombre = Lectura_Hora["NumeroNC"].ToString(),
                                        Fac_Neto = Total,
                                        
                                        Admision = Convert.ToInt32(Lectura_Hora["Ase_Identificador"]),
                                        Com_Direccion = TID.ToString(),
                                        DocE_1 = Lectura_Hora["Pac_IdNum"].ToString(),
                                        DocE_2 = Lectura_Hora["Pac_Email"].ToString(),
                                        DocE_3 = Lectura_Hora["Ase_Email"].ToString(),
                                        DocE_4 = Lectura_Hora["Fac_Res"].ToString(),
                                        ReteFuente = Lectura_Hora["Pac_TipoId"].ToString(),
                                        ProfesionalNombre = Letras(Lectura_Hora["NumeroNC"].ToString()),
                                        DocE_5 = Convert.ToInt32(Numeros(Lectura_Hora["NumeroNC"].ToString())),

                                        Dias = (Lectura_Hora["DiasVencimiento"] == DBNull.Value ? 30 : Convert.ToInt32(Lectura_Hora["DiasVencimiento"])),
                                        MedioP = Lectura_Hora["MedioPago"].ToString(),
                                        MetodoP = Lectura_Hora["MetodoPago"].ToString(),
                                        NombrePrestador = Lectura_Hora["Com_Nombre"].ToString()
                                    });
                                }

                                return Class_FacServ1;
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "Exportar PDF", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        public static List<FacturacionRpt> ExportarFacturaVentasNC(int Docu_Ven, int cia, string Tipo, string Numeronc)
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
                                         "P.Pac_Direccion, P.Pac_Telefono, NC.FechaNC, C.Com_Logo, C.Com_Nombre, V.Ven_Res, V.Ven_Dcto, V.Ven_Cod, Ven_Item, V.Ven_Cantidad, V.Ven_Precio, V.Ven_Total, NC.Cufe, NC.NumeroNC, V.Ven_Homologo, V.Ven_Dcto, " +
                                         "V.DiasVencimiento, V.MedioPago, V.MetodoPago, V.Hora, " +
                                         "SUM(CAST(V.Ven_Total as int)) as ValTot " +
                                         "FROM CXN_VENTAS V " +
                                         "INNER JOIN CXN_PACIENTES P ON V.Ven_Cod_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_CIA C ON V.Ven_Cod_Cia = C.Com_Identificador " +
                                         "INNER JOIN CXN_FACTURANC NC ON V.Ven_Homologo = NC.Facturaelectronica " +
                                         "AND NC.OrdenPedido = V.Ven_Factura " +
                                         "WHERE V.Ven_Factura = @param1 " +
                                         "AND V.Ven_Cod_Cia = @param2 " +
                                         "AND V.Ven_Tipo_Doc = @param3 " +
                                         "AND NC.NumeroNC = @param4 " +
                                         "GROUP BY V.Ven_Usr_Graba, V.Ven_Factura, C.Com_Direccion, C.Com_Telefono, C.Com_Identificacion, C.Com_Prefijo_Electron, C.Com_Doc_Electron, C.Com_Resolucion_Electron, P.Pac_TipoId, P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_IdNum, P.Pac_Email, " +
                                         "P.Pac_Direccion, P.Pac_Telefono, NC.FechaNC, C.Com_Logo, C.Com_Nombre, V.Ven_Res, V.Ven_Dcto, V.Ven_Cod, Ven_Item, V.Ven_Cantidad, V.Ven_Precio, V.Ven_Total, NC.Cufe, NC.NumeroNC, V.Ven_Homologo, V.Ven_Dcto, " +
                                         "V.DiasVencimiento, V.MedioPago, V.MetodoPago, V.Hora";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Docu_Ven);
                        Carga_Command.Parameters.AddWithValue("@param2", cia);
                        Carga_Command.Parameters.AddWithValue("@param3", Tipo);
                        Carga_Command.Parameters.AddWithValue("@param4", Numeronc);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<FacturacionRpt> Class_FacVen1 = new List<FacturacionRpt>();
                                var SubT = SubTotalRec(Docu_Ven, cia, Tipo);

                                Image QRReal = null;                               
                                
                                while (Lectura_Hora.Read() == true)
                                {
                                    string QrElectron = "https://catalogo-vpfe.dian.gov.co/document/searchqr?documentkey=" + Lectura_Hora["Cufe"].ToString();
                                    QRReal = repositorioGenerales.CodifyQR(QrElectron);

                                    string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString(); //trae base64
                                    Byte[] bytes = Convert.FromBase64String(Bod_Firma1); //convierte a bytes
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = Image.FromStream(stmBLOBData);

                                    string Letra;
                                    Letra = repositorioFacturacion.enletras(Convert.ToInt32(SubT).ToString()).ToUpper() + " PESOS";

                                    string NumFacturaReal = Lectura_Hora["NumeroNC"].ToString();

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
                                        FechaBase = Convert.ToDateTime(Lectura_Hora["FechaNC"]),
                                        Logo = repositorioGenerales.GetBytes(pic.Image),
                                        Resolucion = Lectura_Hora["Ven_Res"].ToString(),
                                        CUFE = Lectura_Hora["Cufe"].ToString() + "\n\n" + "Nota Credito Numero " + NumFacturaReal + " asociada a la Factura Electronica Numero " + Lectura_Hora["Ven_Homologo"].ToString(),
                                        Dcto = Convert.ToInt32(Lectura_Hora["Ven_Dcto"]),
                                        QRLogo = (QRReal != null ? repositorioGenerales.GetBytes(QRReal) : null),
                                        
                                        Com_Direccion = Lectura_Hora["Pac_TipoId"].ToString(),
                                        PacienteAseguradora = Lectura_Hora["Pac_IdNum"].ToString(),
                                        ProfesionalNombre = Lectura_Hora["Pac_Email"].ToString(),
                                        Com_Resolucion_Electron = Lectura_Hora["Com_Resolucion_Electron"].ToString(),
                                        PrefijoElectron = Lectura_Hora["Com_Prefijo_Electron"].ToString(),
                                        NumElectron = Convert.ToInt32(Lectura_Hora["Com_Doc_Electron"]),
                                        
                                        Dias = (Lectura_Hora["DiasVencimiento"] == DBNull.Value ? 30 : Convert.ToInt32(Lectura_Hora["DiasVencimiento"])),
                                        MedioP = Lectura_Hora["MedioPago"].ToString(),
                                        MetodoP = Lectura_Hora["MetodoPago"].ToString(),
                                        Hora = Convert.ToDateTime(hTemp)
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
        public static List<RCCAJA> ExportarReciboCajaNC(int PosId)
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

                    String Cargar_Hora = "SELECT TOP 1 C.Com_Logo, P.Pac_PrimerA, P.Pac_PrimerN, P.Pac_SegundoA, P.Pac_SegundoN, P.Pac_TipoId, P.Pac_IdNum, " +
                                          "P.Pac_Telefono, P.Pac_TelefonoAux, P.Pac_Direccion, A.Ase_Descripcion, C.Com_Nombre, C.Com_Identificacion, " +
                                          "C.Com_Direccion, C.Com_Telefono, C.Com_Resolucion_Electron, C.Com_Prefijo_Electron, C.Com_Doc_Electron, R.Rc_Caja_UsrGraba, " +
                                          "R.Rc_Caja_Fecha, R.Rc_Caja_Observacion, R.Rc_Caja_Valor, R.Hor_DocFEModerador, P.Pac_Email, " +
                                          "R.Hor_DocFEModeradorCUFE, R.Hor_DocFEModeradorRes, A.Ase_Identificador, R.Hor_DocFEModeradorFechaHora, F.Cufe, F.FechaNC, F.NumeroNC " +
                                          "FROM CXN_RC_CAJA R " +
                                          "INNER JOIN CXN_PACIENTES P ON R.Rc_Caja_Pac = P.Pac_Id " +
                                          "INNER JOIN CXN_FACTURANC F ON R.Rc_Caja_Adm = F.OrdenPedido " +
                                          "INNER JOIN CXN_ASEGURADORA A ON R.Rc_Caja_Ase = A.Ase_Identificador " +
                                          "INNER JOIN CXN_CIA C ON R.Rc_Caja_Cia = C.Com_Identificador " +                                          
                                          "WHERE R.Rc_Id = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", PosId);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<RCCAJA> Class_RCCaja1 = new List<RCCAJA>();

                                if (Lectura_Hora.Read() == true)
                                {
                                    string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString();
                                    Byte[] bytes = Convert.FromBase64String(Bod_Firma1);
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = Image.FromStream(stmBLOBData);
                               
                                    string QRElectron = "https://catalogo-vpfe.dian.gov.co/document/searchqr?documentkey=" + Lectura_Hora["Cufe"].ToString();                               
                                    Image Code_QR_Fac = repositorioGenerales.CodifyQR(QRElectron);

                                    DateTime fooDate = Convert.ToDateTime(Lectura_Hora["FechaNC"]);

                                    Class_RCCaja1.Add(new RCCAJA
                                    {
                                        PacienteNombre = Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString(),
                                        PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                        PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString() + " " + Lectura_Hora["Pac_TelefonoAux"].ToString(),
                                        PacienteDireccion = Lectura_Hora["Pac_Direccion"].ToString(),
                                        PacienteAseguradora = Lectura_Hora["Pac_IdNum"].ToString(),
                                        EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                        EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                        EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(),
                                        EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                        Vendedor = Lectura_Hora["Rc_Caja_UsrGraba"].ToString(),
                                        FechaBase = Convert.ToDateTime(fooDate),
                                        Recibo = PosId,
                                        Cantidad = 1,
                                        Observacion = Lectura_Hora["Rc_Caja_Observacion"].ToString() + " " + Lectura_Hora["Rc_Caja_Adm"].ToString(),
                                        Logo = repositorioGenerales.GetBytes(pic.Image),
                                        Valor = Convert.ToInt32(Lectura_Hora["Rc_Caja_Valor"]),
                                        ProfesionalNombre = Lectura_Hora["NumeroNC"].ToString(),
                                        Letras = repositorioFacturacion.enletras(Convert.ToInt32(Lectura_Hora["Rc_Caja_Valor"]).ToString()).ToUpper() + " PESOS",
                                        QRImage = repositorioGenerales.GetBytes(Code_QR_Fac),
                                        Correo = Lectura_Hora["Pac_Email"].ToString(),
                                        Com_Direccion = Lectura_Hora["Cufe"].ToString(), //cufe
                                        Resolucion = Lectura_Hora["Hor_DocFEModeradorRes"].ToString(),

                                        TDocReceptor = Lectura_Hora["Pac_TipoId"].ToString(),
                                        DocReceptor = Lectura_Hora["Pac_IdNum"].ToString(),
                                        AseIdentificator = Convert.ToInt32(Lectura_Hora["Ase_Identificador"]),
                                        ResElectron = Lectura_Hora["Hor_DocFEModeradorRes"].ToString(),
                                        PrefijoElectron = Lectura_Hora["Hor_DocFEModerador"].ToString(),
                                        NumeroElectron = Convert.ToInt32(Numeros(Lectura_Hora["Hor_DocFEModerador"].ToString())),
                                        FechaRealElectron = Convert.ToDateTime(fooDate),
                                        
                                    });

                                    return Class_RCCaja1;
                                }
                                else
                                {
                                    return null;
                                }
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


        //TOTALES Y QR NC
        static int SubTotalAseguradorasNC(string NumeroNC, string Tipo)
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
                    String Cargar_Hora = "SELECT NumeroNC, sum(cast(VrTotal as int)) as Total " +
                                         "FROM CXN_CARGOSNC " +
                                         "WHERE NumeroNC = @param1 " +
                                         "AND Tipo = @param2 " +
                                         "GROUP BY NumeroNC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", NumeroNC);
                        Carga_Command.Parameters.AddWithValue("@param2", Tipo);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Total"]);
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "Exportar PDF", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }
    }
}
