using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Persistence.CXN.Metodos
{
    public class MRcCaja : IRcCaja
    {
        private static readonly IGenerales repositorioGenerales = new MGenerales();
        private static readonly IFacturacion repositorioFacturacion = new MFacturacion();

        List<RCCAJA> IRcCaja.ReciboRpt(string Admition)
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

                    String Cargar_Hora = "SELECT R.Rc_Id, R.Rc_Caja_Adm AS Hor_Id, C.Com_Logo, P.Pac_PrimerA, P.Pac_PrimerN, P.Pac_SegundoA, P.Pac_SegundoN, P.Pac_TipoId, P.Pac_IdNum, " +
                                         "P.Pac_Telefono, P.Pac_TelefonoAux, P.Pac_Direccion, A.Ase_Descripcion, C.Com_Nombre, C.Com_Identificacion, " +
                                         "C.Com_Direccion, C.Com_Telefono, C.Com_Resolucion_Electron, C.Com_Prefijo_Electron, C.Com_Doc_Electron, R.Rc_Caja_UsrGraba, " +
                                         "R.Rc_Caja_Fecha, R.Rc_Caja_Observacion, R.Rc_Caja_Valor, R.Hor_DocFEModerador, P.Pac_Email, " +
                                         "R.Hor_DocFEModeradorCUFE, R.Hor_DocFEModeradorRes, A.Ase_Identificador, R.Hor_DocFEModeradorFechaHora " +
                                         "FROM CXN_RC_CAJA R " +
                                         "INNER JOIN CXN_PACIENTES P ON R.Rc_Caja_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_ASEGURADORA A ON R.Rc_Caja_Ase = A.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA C ON R.Rc_Caja_Cia = C.Com_Identificador " +
                                         "WHERE R.Hor_DocFEModerador = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admition);

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
                                    string tempCad7 = "ValIva" + "0";
                                    string tempCad8 = "ValOtroIm:" + "0";
                                    string tempCad9 = "ValTolFac" + Convert.ToInt32(Lectura_Hora["Rc_Caja_Valor"]).ToString("N0");
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
                                        Recibo = Convert.ToInt32(Lectura_Hora["Rc_Id"]),
                                        Cantidad = 1,
                                        Observacion = Lectura_Hora["Rc_Caja_Observacion"].ToString() + " --> " + Lectura_Hora["Hor_Id"].ToString(),
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
                                        PrefijoElectron = Letras(Lectura_Hora["Hor_DocFEModerador"].ToString()),
                                        NumeroElectron = Convert.ToInt32(Numeros(Lectura_Hora["Hor_DocFEModerador"].ToString())),
                                        FechaRealElectron = Convert.ToDateTime(fooDate),
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        int IRcCaja.getLastAdmition(int Admision)
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

                    String Cargar_Hora2 = "SELECT TOP 1 Rc_Id " +
                                          "FROM CXN_RC_CAJA " +
                                          "WHERE Rc_Caja_Adm = @param1 " +
                                          "ORDER BY Rc_Id DESC";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        Carga_Command2.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora2["Rc_Id"]);
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
        List<RCCAJA> IRcCaja.ReciboRpt(int PosId)
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
                                         "C.Com_Direccion, C.Com_Telefono, C.Com_Resolucion_Electron, C.Com_Prefijo_Electron, C.Com_Doc_Electron, R.Rc_Caja_UsrGraba, " +
                                         "R.Rc_Caja_Fecha, R.Rc_Caja_Observacion, R.Rc_Caja_Valor, R.Hor_DocFEModerador, P.Pac_Email, " +
                                         "R.Hor_DocFEModeradorCUFE, R.Hor_DocFEModeradorRes, A.Ase_Identificador, R.Hor_DocFEModeradorFechaHora, R.Rc_Caja_Adm " +
                                         "FROM CXN_RC_CAJA R " +
                                         "INNER JOIN CXN_PACIENTES P ON R.Rc_Caja_Pac = P.Pac_Id " +
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

                                    string tempCad1 = "NumFac:" + Lectura_Hora["Hor_DocFEModerador"].ToString();
                                    string tempCad2 = "FecFac:" + Convert.ToDateTime(Lectura_Hora["Rc_Caja_Fecha"]).ToString("yyyy-MM-dd");
                                    string tempCad3 = "HorFac:" + Convert.ToDateTime(Lectura_Hora["Rc_Caja_Fecha"]).ToString("hh:mm:ss tt");
                                    string tempCad4 = "NitFac:" + Lectura_Hora["Com_Identificacion"].ToString();
                                    string tempCad5 = "DocAdq:" + Lectura_Hora["Pac_IdNum"].ToString();
                                    string tempCad6 = "ValFac:" + Convert.ToInt32(Lectura_Hora["Rc_Caja_Valor"]).ToString("N0");
                                    string tempCad7 = "ValIva" + "0";
                                    string tempCad8 = "ValOtroIm:" + "0";
                                    string tempCad9 = "ValTolFac" + Convert.ToInt32(Lectura_Hora["Rc_Caja_Valor"]).ToString("N0");
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
                                         Recibo = PosId,
                                        Cantidad = 1,
                                         Observacion = Lectura_Hora["Rc_Caja_Observacion"].ToString() + " --> " + Lectura_Hora["Rc_Caja_Adm"].ToString(),
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
                                         PrefijoElectron = !string.IsNullOrEmpty(Lectura_Hora["Hor_DocFEModerador"].ToString()) ? Letras(Lectura_Hora["Hor_DocFEModerador"].ToString()) : "",
                                         NumeroElectron = !string.IsNullOrEmpty(Lectura_Hora["Hor_DocFEModerador"].ToString()) ? Convert.ToInt32(Numeros(Lectura_Hora["Hor_DocFEModerador"].ToString())) : 0,
                                        FechaRealElectron = Convert.ToDateTime(fooDate)
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        string Numeros(string Cadena)
        {
            string numeros = new string(Cadena.Where(char.IsDigit).ToArray());
            return numeros;
        }
        string Letras(string Cadena)
        {
            string letras = new string(Cadena.Where(char.IsLetter).ToArray());
            return letras;
        }
        int IRcCaja.AgregarRecibo(CXN_RC_CAJA R)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_RC_CAJA (Rc_Caja_Pac, 
                                                                                   Rc_Caja_Ase, 
                                                                                   Rc_Caja_Cia, 
                                                                                   Rc_Caja_Fecha, 
                                                                                   Rc_Caja_UsrGraba, 
                                                                                   Rc_Caja_Valor, 
                                                                                   Rc_Caja_Adm, " +
                                                                              "Rc_Caja_Observacion, " +
                                                                              "FormaPago, " +
                                                                              "Hor_ConceptoRecaudo, " +
                                                                              "Num_Cruce) " +
                                                         "values (@param1 , " +
                                                                 "@param2 , " +
                                                                 "@param3 , " +
                                                                 "@param4 , " +
                                                                 "@param5 , " +
                                                                 "@param6 , " +
                                                                 "@param7 , " +
                                                                 "@param8, " +
                                                                 "@param9, " +
                                                                 "@param10, " +
                                                                 "@param11); SELECT SCOPE_IDENTITY()", con);

                    cmd.Parameters.AddWithValue("@param1", R.Rc_Caja_Pac);
                    cmd.Parameters.AddWithValue("@param2", R.Rc_Caja_Ase);
                    cmd.Parameters.AddWithValue("@param3", R.Rc_Caja_Cia);
                    cmd.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = Convert.ToDateTime(R.Rc_Caja_Fecha).Date;
                    cmd.Parameters.AddWithValue("@param5", R.Rc_Caja_UsrGraba);
                    cmd.Parameters.AddWithValue("@param6", R.Rc_Caja_Valor);
                    cmd.Parameters.AddWithValue("@param7", R.Rc_Caja_Adm);
                    cmd.Parameters.AddWithValue("@param8", R.Rc_Caja_Observacion);
                    cmd.Parameters.AddWithValue("@param9", R.FormaPago);
                    cmd.Parameters.AddWithValue("@param10", R.Hor_ConceptoRecaudo);
                    cmd.Parameters.AddWithValue("@param11", R.Num_Cruce);
                    var s = cmd.ExecuteScalar();
                    return Convert.ToInt32(s);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }
        List<CXN_RC_CAJA> IRcCaja.RCCAJAS(string TID, string ID)
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

                    String Cargar_Hora2 = "SELECT * " +
                                          "FROM CXN_PACIENTES " +
                                          "INNER JOIN CXN_RC_CAJA ON CXN_PACIENTES.Pac_Id = CXN_RC_CAJA.Rc_Caja_Pac " +
                                          "WHERE CXN_PACIENTES.Pac_TipoId = '" + TID + "' " +
                                          "AND CXN_PACIENTES.Pac_IdNum = '" + ID + "' " +
                                          "ORDER BY CXN_RC_CAJA.Rc_Caja_Fecha ASC";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        List<CXN_RC_CAJA> L = new List<CXN_RC_CAJA>();

                        while (Lectura_Hora2.Read() == true)
                        {
                            L.Add(new CXN_RC_CAJA
                            {
                                Rc_Caja_Adm = Convert.ToInt32(Lectura_Hora2["Rc_Caja_Adm"]),
                                Rc_Caja_Fecha = Convert.ToDateTime(Lectura_Hora2["Rc_Caja_Fecha"]),
                                Rc_Caja_UsrGraba = Lectura_Hora2["Rc_Caja_UsrGraba"].ToString(),
                                Rc_Caja_Observacion = Lectura_Hora2["Pac_PrimerA"].ToString() + " " + Lectura_Hora2["Pac_SegundoA"].ToString() + " " + Lectura_Hora2["Pac_PrimerN"].ToString() + " " + Lectura_Hora2["Pac_SegundoN"].ToString() // paciente en este caso
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        (int Valor, string Concepto) IRcCaja.getValRcCaja(int Admision)
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

                    String Cargar_Hora2 = "SELECT TOP 1 Hor_ConceptoRecaudo, Rc_Caja_Valor " +
                                          "FROM CXN_RC_CAJA " +
                                          "WHERE Rc_Caja_Adm = @param1 " +
                                          "ORDER BY Rc_Id DESC";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        Carga_Command2.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.Read() == true)
                            {
                                return ((Lectura_Hora2["Rc_Caja_Valor"] == DBNull.Value || Lectura_Hora2["Rc_Caja_Valor"].ToString() == "" ? 0 : Convert.ToInt32(Lectura_Hora2["Rc_Caja_Valor"])), Lectura_Hora2["Hor_ConceptoRecaudo"].ToString());
                            }
                            else
                            {
                                return (0, null);
                            }
                        }
                    }                                        
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return (0, null);
            }
        }
        bool IRcCaja.EsConsulta(int Admision, string TipoServicio)
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

                    String Cargar_Hora2 = "SELECT H.Hor_RcCaja " +
                                          "FROM CXN_HORARIO H " +
                                          "INNER JOIN CXN_CONVENIOS C ON H.Hor_Pac_Cup = C.Con_Id_Serv " +
                                          "WHERE H.Hor_Id = '" + Admision + "' " +                                          
                                          "AND C.Con_Aseguradora = H.Hor_Pac_Ase " +
                                          "AND C.Con_Clase = '" + TipoServicio + "'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.Read() == true)
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
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
