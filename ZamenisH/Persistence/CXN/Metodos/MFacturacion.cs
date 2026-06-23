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
    public class MFacturacion : IFacturacion
    {
        private static readonly IGenerales repositorioGenerales = new MGenerales();
        private static readonly IFacturacion repositorioFacturacion = new MFacturacion();
        private static readonly IPacientes repoPacs = new MPacientes();
        private static readonly IFacturacion thisRepo = new MFacturacion();

        string IFacturacion.enletras(string num)
        {
            try
            {
                string res, dec = "";
                Int64 entero;
                int decimales;
                double nro;

                try
                {
                    nro = Convert.ToDouble(num);
                }
                catch
                {
                    return "";
                }

                entero = Convert.ToInt64(Math.Truncate(nro));
                decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2));
                if (decimales > 0)
                {
                    dec = " CON " + decimales.ToString() + "/100";
                }

                res = toText(Convert.ToDouble(entero)) + dec;
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }            
        }
        string toText(double value)
        {
            try
            {
                string Num2Text = "";
                value = Math.Truncate(value);
                if (value == 0) Num2Text = "CERO";
                else if (value == 1) Num2Text = "UNO";
                else if (value == 2) Num2Text = "DOS";
                else if (value == 3) Num2Text = "TRES";
                else if (value == 4) Num2Text = "CUATRO";
                else if (value == 5) Num2Text = "CINCO";
                else if (value == 6) Num2Text = "SEIS";
                else if (value == 7) Num2Text = "SIETE";
                else if (value == 8) Num2Text = "OCHO";
                else if (value == 9) Num2Text = "NUEVE";
                else if (value == 10) Num2Text = "DIEZ";
                else if (value == 11) Num2Text = "ONCE";
                else if (value == 12) Num2Text = "DOCE";
                else if (value == 13) Num2Text = "TRECE";
                else if (value == 14) Num2Text = "CATORCE";
                else if (value == 15) Num2Text = "QUINCE";
                else if (value < 20) Num2Text = "DIECI" + toText(value - 10);
                else if (value == 20) Num2Text = "VEINTE";
                else if (value < 30) Num2Text = "VEINTI" + toText(value - 20);
                else if (value == 30) Num2Text = "TREINTA";
                else if (value == 40) Num2Text = "CUARENTA";
                else if (value == 50) Num2Text = "CINCUENTA";
                else if (value == 60) Num2Text = "SESENTA";
                else if (value == 70) Num2Text = "SETENTA";
                else if (value == 80) Num2Text = "OCHENTA";
                else if (value == 90) Num2Text = "NOVENTA";
                else if (value < 100) Num2Text = toText(Math.Truncate(value / 10) * 10) + " Y " + toText(value % 10);
                else if (value == 100) Num2Text = "CIEN";
                else if (value < 200) Num2Text = "CIENTO " + toText(value - 100);
                else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = toText(Math.Truncate(value / 100)) + "CIENTOS";
                else if (value == 500) Num2Text = "QUINIENTOS";
                else if (value == 700) Num2Text = "SETECIENTOS";
                else if (value == 900) Num2Text = "NOVECIENTOS";
                else if (value < 1000) Num2Text = toText(Math.Truncate(value / 100) * 100) + " " + toText(value % 100);
                else if (value == 1000) Num2Text = "MIL";
                else if (value < 2000) Num2Text = "MIL " + toText(value % 1000);
                else if (value < 1000000)
                {
                    Num2Text = toText(Math.Truncate(value / 1000)) + " MIL";
                    if ((value % 1000) > 0) Num2Text = Num2Text + " " + toText(value % 1000);
                }

                else if (value == 1000000) Num2Text = "UN MILLON";
                else if (value < 2000000) Num2Text = "UN MILLON " + toText(value % 1000000);
                else if (value < 1000000000000)
                {
                    Num2Text = toText(Math.Truncate(value / 1000000)) + " MILLONES ";
                    if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000) * 1000000);
                }

                else if (value == 1000000000000) Num2Text = "UN BILLON";
                else if (value < 2000000000000) Num2Text = "UN BILLON " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

                else
                {
                    Num2Text = toText(Math.Truncate(value / 1000000000000)) + " BILLONES";
                    if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
                }
                return Num2Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }           
        }
        string IFacturacion.insertarDocumento(CXN_FACTURA f)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_FACTURA " +
                                                     "(Fac_Num_Fac, " +
                                                      "Fac_Res, " +
                                                      "Homologo, " +
                                                      "Fac_Estado, " +
                                                      "Fac_Ase, " +
                                                      "Fac_Cia, " +
                                                      "Fac_Pac, " +
                                                      "Fac_Fecha, " +
                                                      "Fac_Fecha_Des, " +
                                                      "Fac_Fecha_Has, " +
                                                      "Fac_Observa, " +
                                                      "Fac_Tipo_Doc, " +
                                                      "Fac_Usr_Graba) " +
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
                                                      "@param13)", con);

                    cmd.Parameters.AddWithValue("@param1", f.Fac_Num_Fac);
                    cmd.Parameters.AddWithValue("@param2", f.Fac_Res);
                    cmd.Parameters.AddWithValue("@param3", f.Homologo);
                    cmd.Parameters.AddWithValue("@param4", f.Fac_Estado);
                    cmd.Parameters.AddWithValue("@param5", f.Fac_Ase);
                    cmd.Parameters.AddWithValue("@param6", f.Fac_Cia);
                    cmd.Parameters.AddWithValue("@param7", f.Fac_Pac);
                    cmd.Parameters.Add(new SqlParameter("@param8", SqlDbType.DateTime)).Value = Hoy;
                    cmd.Parameters.Add(new SqlParameter("@param9", SqlDbType.DateTime)).Value = Hoy;
                    cmd.Parameters.Add(new SqlParameter("@param10", SqlDbType.DateTime)).Value = Hoy;
                    cmd.Parameters.AddWithValue("@param11", f.Fac_Observa);
                    cmd.Parameters.AddWithValue("@param12", f.Fac_Tipo_Doc);
                    cmd.Parameters.AddWithValue("@param13", f.Fac_Usr_Graba);
                    cmd.ExecuteNonQuery();
                    return "OK";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        List<CXN_FACTURA> IFacturacion.Filter(string Tipo, string TID, string ID, int Cia)
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

                    String Cargar_Hora2 = "SELECT Fac_Estado, Fac_Num_Fac, Homologo, Fac_Fecha, Fac_Fecha_Des, Fac_Fecha_Has, Fac_Usr_Graba, " +
                                          "Pac_PrimerA, Pac_SegundoA, Pac_PrimerN, Pac_SegundoN, Fac_Tipo_Doc " +
                                          "FROM CXN_PACIENTES " +
                                          "INNER JOIN CXN_FACTURA ON CXN_PACIENTES.Pac_Id = CXN_FACTURA.Fac_Pac " +
                                          "WHERE CXN_PACIENTES.Pac_TipoId = '" + TID + "' " +
                                          "AND CXN_FACTURA.Fac_Cia = '" + Cia + "' " +
                                          "AND CXN_PACIENTES.Pac_IdNum = '" + ID + "' " +
                                          "AND CXN_FACTURA.Fac_Tipo_Doc = '" + Tipo + "' " +
                                          "ORDER BY CXN_FACTURA.Fac_Fecha ASC";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        List<CXN_FACTURA> F = new List<CXN_FACTURA>();

                        while (Lectura_Hora2.Read() == true)
                        {
                            F.Add(new CXN_FACTURA
                            {
                                Fac_Estado = Lectura_Hora2["Fac_Estado"].ToString(),
                                Fac_Num_Fac = Convert.ToInt32(Lectura_Hora2["Fac_Num_Fac"]),
                                Homologo = Lectura_Hora2["Homologo"].ToString(),
                                Fac_Fecha = Convert.ToDateTime(Lectura_Hora2["Fac_Fecha"]),
                                Fac_Fecha_Des = Convert.ToDateTime(Lectura_Hora2["Fac_Fecha_Des"]),
                                Fac_Fecha_Has = Convert.ToDateTime(Lectura_Hora2["Fac_Fecha_Has"]),
                                Fac_Usr_Graba = Lectura_Hora2["Fac_Usr_Graba"].ToString(),
                                Fac_Tipo_Doc = Lectura_Hora2["Fac_Tipo_Doc"].ToString(),
                                Fac_Observa = Lectura_Hora2["Pac_PrimerA"].ToString() + " " + Lectura_Hora2["Pac_SegundoA"].ToString() + " " + Lectura_Hora2["Pac_PrimerN"].ToString() + " " + Lectura_Hora2["Pac_SegundoN"].ToString() //paciente en este caso
                            });
                        }

                        return F;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        List<CXN_FACTURA> IFacturacion.GetFacturasForConvertElectron(int Cia, DateTime Desde, DateTime Hasta)
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

                    String Cargar_Hora2 = "SELECT F.Fac_Num_Fac, F.Homologo, F.Fac_Fecha, F.Fac_Fecha_Des, F.Fac_Fecha_Has, F.Fac_Usr_Graba, " +
                                          "P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PAC, " +
                                          "A.Ase_Descripcion " +
                                          "FROM CXN_PACIENTES P " +
                                          "INNER JOIN CXN_FACTURA F ON P.Pac_Id = F.Fac_Pac " +
                                          "INNER JOIN CXN_ASEGURADORA A ON F.Fac_Ase = A.Ase_Identificador " +
                                          "WHERE F.Fac_Cia = @param1 " +
                                          "AND F.Fac_Tipo_Doc = @param2 " +
                                          "AND F.Fac_Fecha BETWEEN @param3 AND @param4 " +
                                          "AND F.Fac_Estado = @param5 " +
                                          "ORDER BY F.Fac_Num_Fac ASC";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        Carga_Command2.Parameters.AddWithValue("@param1", Cia);
                        Carga_Command2.Parameters.AddWithValue("@param2", "OP");
                        Carga_Command2.Parameters.AddWithValue("@param3", Convert.ToDateTime(Desde).ToString(datCone["Format_Fecha"]));
                        Carga_Command2.Parameters.AddWithValue("@param4", Convert.ToDateTime(Hasta).ToString(datCone["Format_Fecha"]));
                        Carga_Command2.Parameters.AddWithValue("@param5", "F");

                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.HasRows)
                            {
                                List<CXN_FACTURA> F = new List<CXN_FACTURA>();

                                while (Lectura_Hora2.Read() == true)
                                {
                                    if (Lectura_Hora2["Homologo"].ToString() == Lectura_Hora2["Fac_Num_Fac"].ToString())
                                    {
                                        F.Add(new CXN_FACTURA
                                        {
                                            Fac_Num_Fac = Convert.ToInt32(Lectura_Hora2["Fac_Num_Fac"]),
                                            Fac_Fecha = Convert.ToDateTime(Lectura_Hora2["Fac_Fecha"]),
                                            Fac_Fecha_Des = Convert.ToDateTime(Lectura_Hora2["Fac_Fecha_Des"]),
                                            Fac_Fecha_Has = Convert.ToDateTime(Lectura_Hora2["Fac_Fecha_Has"]),
                                            Fac_Usr_Graba = Lectura_Hora2["Fac_Usr_Graba"].ToString(),
                                            Fac_Observa = Lectura_Hora2["PAC"].ToString(),
                                            Cobertura = Lectura_Hora2["Ase_Descripcion"].ToString(),
                                            VrCompartido = SubTotal(Convert.ToInt32(Lectura_Hora2["Fac_Num_Fac"]), Cia, "OP")
                                        });
                                    }                                   
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
        List<FacturasR> IFacturacion.Fac_Export(int Numero_Fac, int cia, string Tipo)
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
                                         "WHERE CXN_FACTURA.Fac_Num_Fac = '" + Numero_Fac + "' " +
                                         "AND CXN_FACTURA.Fac_Tipo_Doc = '" + Tipo + "' " +
                                         "AND CXN_FACTURA.Fac_Cia = '" + cia + "' " +
                                         "AND CXN_CARGOS.Car_Tipo_Doc = '" + Tipo + "' " +
                                         "AND CXN_CARGOS.Car_Cia = '" + cia + "' " +
                                         "GROUP BY Com_Nombre, Com_Identificador, Com_Direccion, Com_Telefono, Com_Logo, Com_Identificacion, Fac_Res, Pac_PrimerN, " +
                                         "Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, Pac_Direccion, Pac_Telefono, Pac_TelefonoAux, Pac_TipoId, Pac_IdNum, Pac_Email, Ase_Descripcion, Ase_Identificador, " +
                                         "Ase_NitCia, Ase_DVNitCia, Ase_Telefono, Ase_Direccion, Ase_Email, Fac_Num_Fac, Fac_Fecha, Fac_Fecha_Has, Fac_Fecha_Des, Fac_Num_Aut, Fac_Descuento, " +
                                         "Fac_Observa, Fac_Pac, Fac_Cia, Fac_Ase, Fac_Estado, Car_Factura, Car_Estado, Car_Cod, Fac_Usr_Graba, Car_Item, Car_Val_Un, " +
                                         "Cufe, QRCufe, VrCompartido, Copago, Anticipo, CodPrestador, ContratoPoliza, Cobertura, ModPago, Homologo, Hora, Com_Resolucion_Electron, Com_Prefijo_Electron, Com_Doc_Electron, MedioPago, MetodoPago, DiasVencimiento, " +
                                         "PercentICA, PercentFUENTE, VrICA, VrFUENTE";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<FacturasR> Class_FacServ1 = new List<FacturasR>(); 
                                var Total = SubTotal(Numero_Fac, cia, Tipo);
                                string strQR = "Numero de Documento: " + Numero_Fac + "\n\r" +
                                               "Compañia: " + cia + "\n\r" +
                                               "Tipo: " + Tipo;
                                var Code_QR_Fac = repositorioGenerales.CodifyQR(strQR);

                                string QrElectron = "Vacio";

                                while (Lectura_Hora.Read() == true)
                                {
                                    //qrcufe 
                                    if (Lectura_Hora["Cufe"] != DBNull.Value)
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
                                    
                                    string TID = repoPacs.getTipoDoc(Lectura_Hora["Pac_TipoId"].ToString());

                                    string percentfuente = "0";
                                    string percentica = "0";
                                    int vrfuente = string.IsNullOrEmpty(Lectura_Hora["VrFUENTE"].ToString()) ? 0 : Convert.ToInt32(Lectura_Hora["VrFUENTE"]);
                                    int vrica = string.IsNullOrEmpty(Lectura_Hora["VrICA"].ToString()) ? 0 : Convert.ToInt32(Lectura_Hora["VrICA"]);

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
                                        Code_QR = repositorioGenerales.GetBytes(Code_QR_Fac),
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
                                        ProfesionalNombre = !string.IsNullOrEmpty(Lectura_Hora["Homologo"].ToString()) ? Letras(Lectura_Hora["Homologo"].ToString()) : "",
                                        DocE_5 = !string.IsNullOrEmpty(Lectura_Hora["Homologo"].ToString()) ? Convert.ToInt32(Numeros(Lectura_Hora["Homologo"].ToString())) : 0,

                                        Dias = (Lectura_Hora["DiasVencimiento"] == DBNull.Value ? 30 : Convert.ToInt32(Lectura_Hora["DiasVencimiento"])),
                                        MedioP = Lectura_Hora["MedioPago"].ToString(),
                                        MetodoP = Lectura_Hora["MetodoPago"].ToString(),
                                        NombrePrestador = Lectura_Hora["Com_Nombre"].ToString(),
                                        
                                        PercentFUENTE = percentfuente,
                                        PercentICA = percentica,
                                        VrFUENTE = vrfuente,
                                        VrICA = vrica,
                                        VrNeto = Total - vrfuente - vrica,
                                        LetrasImpuestos = repositorioFacturacion.enletras(Convert.ToInt32(Total - vrfuente - vrica).ToString()).ToUpper() + " PESOS",
                                        
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }   
        List<FacturasR> IFacturacion.Fac_ExportOtrosServiciosTODOIMPUESTOS(int Numero_Fac, int cia, string Tipo, bool MostrarImpuestos)
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

                                //Valor Bruto
                                //var Total = SubTotal(Numero_Fac, cia, Tipo);
                                //decimal BrutoFactura = 0;
                                //decimal SubTotalFactura = 0;
                                //decimal NetoFactura = 0;

                                Dictionary<int, decimal> Subtotales = new Dictionary<int, decimal>();
                                int Contador = 1;
                                int VrBrutoAcumulado = 0;

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

                                    //BrutoFactura = Math.Round(BrutoFactura + (Convert.ToInt32(VrBrutoProducto + (ValorIVAProducto / 100) - (ValorReteFuenteProducto / 100) - ValorRetencionICA)), 2);
                                    //SubTotalFactura = Math.Round(SubTotalFactura + Convert.ToInt32(ValorNetoProducto), 2);
                                    //NetoFactura = Math.Round(SubTotalFactura - DescuentosFactura, 2);

                                    string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString(); //trae base64
                                    Byte[] bytes = Convert.FromBase64String(Bod_Firma1); //convierte a bytes
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = Image.FromStream(stmBLOBData);

                                    //string TID = repoPacs.getTipoDoc(Lectura_Hora["Pac_TipoId"].ToString());

                                    int TotLinea = 0;

                                    if (MostrarImpuestos == true)
                                    {
                                        TotLinea = Convert.ToInt32(VrBrutoProducto + (ValorIVAProducto / 100) - (ValorReteFuenteProducto / 100) - ValorRetencionICA);
                                    }
                                    else
                                    {
                                        TotLinea = Convert.ToInt32(VrBrutoProducto + (ValorIVAProducto / 100));
                                    }

                                    Class_FacServ1.Add(new FacturasR
                                    {
                                        Car_Cod = Lectura_Hora["Car_Cod"].ToString(),
                                        Car_Item = Lectura_Hora["Car_Item"].ToString(),
                                        Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        Car_Val_Un = Convert.ToInt32(Lectura_Hora["Car_Val_Un"]),
                                        Total = TotLinea,
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
                                        ProfesionalNombre = !string.IsNullOrEmpty(Lectura_Hora["Homologo"].ToString()) ? Lectura_Hora["Homologo"].ToString().Any(char.IsLetter) ? Letras(Lectura_Hora["Homologo"].ToString()) : "" : "",
                                        DocE_5 = !string.IsNullOrEmpty(Lectura_Hora["Homologo"].ToString()) ? Convert.ToInt32(Numeros(Lectura_Hora["Homologo"].ToString())) : 0,
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

                                string QrElectron = "NumFac:" + Class_FacServ1[0].Cobertura + "\r\n" +
                                                    "FecFac:" + Convert.ToDateTime(Class_FacServ1[0].FechaBase).ToString("yyyy-MM-dd") + "\r\n" +
                                                    "HorFac:" + Convert.ToDateTime(Class_FacServ1[0].Fac_Fecha_Des).ToString("hh:mm:ss tt") + "\r\n" +
                                                    "NitFac:" + Class_FacServ1[0].EmpresaIdentificacion.ToString() + "\r\n" +
                                                    "DocAdq:" + Class_FacServ1[0].DocE_2 + "\r\n" +
                                                    "ValFac:" + Convert.ToInt32(sumTemp) + "\r\n" + //total antes de iva
                                                    "ValIva:" + "0" + "\r\n" + //Total IVA 
                                                    "ValOtroIm:" + "0" + "\r\n" +
                                                    "ValTolFac:" + Convert.ToInt32(sumTemp) + "\r\n" +
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

        //Por homologo Orden de Pedido
        List<FacturasR> IFacturacion.Fac_Export(string Numero_Fac, int cia)
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
                                         "Pac_IdNum, Ase_Descripcion, Ase_NitCia, Ase_DVNitCia, Ase_Telefono, Ase_Direccion, Fac_Num_Fac, Fac_Fecha, Fac_Fecha_Has, " +
                                         "Fac_Fecha_Des, Fac_Num_Aut, Fac_Descuento, Fac_Observa, Fac_Pac, Fac_Cia, Fac_Ase, Fac_Estado, Car_Factura, Car_Estado, Car_Cod, Fac_Usr_Graba, " +
                                         "Car_Item, Car_Val_Un, sum(cast(Car_Cant as int)) as Cantidad, sum(cast(Car_Val_Tot as int)) as Total, " +
                                         "Cufe, QRCufe, VrCompartido, Copago, Anticipo, CodPrestador, ContratoPoliza, Cobertura, ModPago, Homologo, Hora, Fac_Id " +
                                         "FROM CXN_FACTURA " +
                                         "INNER JOIN CXN_CARGOS ON CXN_FACTURA.Fac_Num_Fac = CXN_CARGOS.Car_Factura " +
                                         "INNER JOIN CXN_ASEGURADORA ON CXN_FACTURA.Fac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA ON CXN_FACTURA.Fac_Cia = CXN_CIA.Com_Identificador " +
                                         "INNER JOIN CXN_PACIENTES ON CXN_FACTURA.Fac_Pac = CXN_PACIENTES.Pac_Id " +
                                         "WHERE CXN_FACTURA.Homologo = @param1 " +
                                         "AND CXN_FACTURA.Fac_Tipo_Doc = @param3 " +
                                         "AND CXN_FACTURA.Fac_Cia = @param2 " +
                                         "AND CXN_CARGOS.Car_Tipo_Doc = @param3 " +
                                         "AND CXN_CARGOS.Car_Cia = @param2 " +
                                         "GROUP BY Com_Nombre, Com_Identificador, Com_Direccion, Com_Telefono, Com_Logo, Com_Identificacion, Fac_Res, Pac_PrimerN, " +
                                         "Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, Pac_Direccion, Pac_Telefono, Pac_TelefonoAux, Pac_TipoId, Pac_IdNum, Ase_Descripcion, " +
                                         "Ase_NitCia, Ase_DVNitCia, Ase_Telefono, Ase_Direccion, Fac_Num_Fac, Fac_Fecha, Fac_Fecha_Has, Fac_Fecha_Des, Fac_Num_Aut, Fac_Descuento, " +
                                         "Fac_Observa, Fac_Pac, Fac_Cia, Fac_Ase, Fac_Estado, Car_Factura, Car_Estado, Car_Cod, Fac_Usr_Graba, Car_Item, Car_Val_Un, " +
                                         "Cufe, QRCufe, VrCompartido, Copago, Anticipo, CodPrestador, ContratoPoliza, Cobertura, ModPago, Homologo, Hora, Fac_Id";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Numero_Fac);
                        Carga_Command.Parameters.AddWithValue("@param2", cia);
                        Carga_Command.Parameters.AddWithValue("@param3", "OP");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<FacturasR> Class_FacServ1 = new List<FacturasR>();
                                CXN_FACTURA f = thisRepo.getFacElectronica(Numero_Fac);
                                int Total = SubTotal(f.Fac_Num_Fac, cia, "OP");

                                string QrElectron = "Vacio";

                                while (Lectura_Hora.Read() == true)
                                {
                                    //qrcufe 
                                    if (Lectura_Hora["Cufe"] != DBNull.Value)
                                    {
                                        QrElectron = "NumFac:" + Lectura_Hora["Homologo"].ToString() + "\r\n" +
                                            "FecFac:" + Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]).ToString("yyyy-MM-dd") + "\r\n" +
                                            "HorFac:" + Convert.ToDateTime(Lectura_Hora["Hora"]).ToString("hh:mm:ss tt") + "\r\n" +
                                            "NitFac:" + Lectura_Hora["Com_Identificacion"].ToString() + "\r\n" +
                                            "DocAdq:" + Lectura_Hora["Ase_NitCia"].ToString() + "\r\n" +
                                            "ValFac:" + Convert.ToInt32(Total) + "\r\n" + //total antes de iva
                                            "ValIva" + "0" + "\r\n" + //Total IVA 
                                            "ValOtroIm:" + "0" + "\r\n" +
                                            "ValTolFac" + Convert.ToInt32(Total) + "\r\n" +
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

                                    string TID = repoPacs.getTipoDoc(Lectura_Hora["Pac_TipoId"].ToString());

                                    Class_FacServ1.Add(new FacturasR
                                    {
                                        Admision = Convert.ToInt32(Lectura_Hora["Fac_Id"]),
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
                                        //Code_QR = repositorioGenerales.GetBytes(Code_QR_Fac),
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
                                        EmpresaNombre = Lectura_Hora["Homologo"].ToString()
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        int SubTotal(int Numero_Fac, int cia, string Tipo)
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }
        List<FacturasR> IFacturacion.Exporta_DOCE_IND_Orden(int Numero_DOCE_Ind, int cia, string Tipo)
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
                    String Cargar_Hora = "SELECT P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_TipoId, P.Pac_IdNum, " +
                                         "C.Com_Nombre, C.Com_Identificador, C.Com_Direccion, C.Com_Telefono, " +
                                         "C.Com_Identificacion, A.Ase_Descripcion, A.Ase_NitCia, CA.Car_Cod, CA.Car_Item, CA.Car_Val_Un, " +
                                         "A.Ase_Telefono, A.Ase_Direccion, F.Fac_Num_Fac, " +
                                         "F.Fac_Fecha, F.Fac_Res, F.Fac_Fecha_Has, F.Fac_Fecha_Des, F.Fac_Num_Aut, " +
                                         "F.Fac_Descuento, F.Fac_Observa, " +
                                         "F.DocE_1, F.DocE_2, F.DocE_3, F.DocE_4, F.DocE_5, " +
                                         "F.DocE_6, F.DocE_7, F.DocE_8, F.Fac_Observa, " +
                                         "SUM(CAST(CA.Car_Cant AS int)) AS Cantidad, " +
                                         "SUM(CAST(CA.Car_Val_Tot AS int)) AS Total " +
                                         "FROM CXN_FACTURA F " +
                                         "INNER JOIN CXN_CARGOS CA ON F.Fac_Num_Fac = CA.Car_Factura " +
                                         "INNER JOIN CXN_ASEGURADORA A ON F.Fac_Ase = A.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA C ON F.Fac_Cia = C.Com_Identificador " +
                                         "INNER JOIN CXN_PACIENTES P ON F.Fac_Pac = P.Pac_Id " +
                                         "WHERE F.Fac_Num_Fac = '" + Numero_DOCE_Ind + "' " +
                                         "AND F.Fac_Tipo_Doc = '" + Tipo + "' " +
                                         "AND F.Fac_Cia = '" + cia + "' " +
                                         "AND CA.Car_Tipo_Doc = '" + Tipo + "' " +
                                         "AND CA.Car_Cia = '" + cia + "' " +
                                         "GROUP BY P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_TipoId, P.Pac_IdNum, " +
                                         "C.Com_Nombre, C.Com_Identificador, C.Com_Direccion, C.Com_Telefono, " +
                                         "C.Com_Identificacion, A.Ase_Descripcion, A.Ase_NitCia, " +
                                         "A.Ase_Telefono, A.Ase_Direccion, F.Fac_Num_Fac, " +
                                         "F.Fac_Fecha, F.Fac_Res, F.Fac_Fecha_Has, F.Fac_Fecha_Des, F.Fac_Num_Aut, " +
                                         "F.Fac_Descuento, F.Fac_Observa, " +
                                         "F.DocE_1, F.DocE_2, F.DocE_3, F.DocE_4, F.DocE_5, " +
                                         "F.DocE_6, F.DocE_7, F.DocE_8, F.Fac_Observa, CA.Car_Cod, CA.Car_Item, CA.Car_Val_Un";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<FacturasR> Class_DOCE1 = new List<FacturasR>();
                        string strQR = "Documento Equivalente: " + Convert.ToInt32(Numero_DOCE_Ind) + "\n\r" +
                                       "Compañia: " + cia + "\n\r" +
                                       "Tipo: " + Tipo;
                        var QR = repositorioGenerales.CodifyQR(strQR);
                        var Total = SubTotal(Convert.ToInt32(Numero_DOCE_Ind), cia, Tipo);
                        int DOCE5, DOCE6;
                        DateTime DOCE7, DOCE8;

                        while (Lectura_Hora.Read() == true)
                        {
                            string Letra;
                            double Neto = Convert.ToInt32(Total) - Convert.ToInt32(Lectura_Hora["Fac_Descuento"]);
                            Letra = repositorioFacturacion.enletras(Convert.ToInt32(Neto).ToString()).ToUpper() + " PESOS";

                            if (Lectura_Hora["DocE_5"] == DBNull.Value)
                            {
                                DOCE5 = 0;
                            }
                            else
                            {
                                DOCE5 = Convert.ToInt32(Lectura_Hora["DocE_5"]);
                            }

                            if (Lectura_Hora["DocE_6"] == DBNull.Value)
                            {
                                DOCE6 = 0;
                            }
                            else
                            {
                                DOCE6 = Convert.ToInt32(Lectura_Hora["DocE_6"]);
                            }

                            DateTime H7 = Convert.ToDateTime("2000-01-01");
                            DateTime H8 = Convert.ToDateTime("2000-01-01");

                            if (Lectura_Hora["DocE_7"] == DBNull.Value)
                            {
                                DOCE7 = H7;
                            }
                            else
                            {
                                DOCE7 = Convert.ToDateTime(Lectura_Hora["DocE_7"]);
                            }

                            if (Lectura_Hora["DocE_8"] == DBNull.Value)
                            {
                                DOCE8 = H8;
                            }
                            else
                            {
                                DOCE8 = Convert.ToDateTime(Lectura_Hora["DocE_8"]);
                            }

                            Class_DOCE1.Add(new FacturasR
                            {
                                Letras = Letra,
                                PacienteNombre = Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString(),
                                PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                Fac_Total = Convert.ToInt32(Total), //total sin descuentos
                                Fac_Neto = Convert.ToInt32(Neto), //neto a pagar
                                Fac_Num_Fac = Convert.ToInt32(Numero_DOCE_Ind),
                                EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(),
                                EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                Fac_Fecha_Des = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Des"]),
                                Fac_Fecha_Has = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Has"]),
                                FechaBase = Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]),
                                PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                Ase_NitCia = Lectura_Hora["Ase_NitCia"].ToString(),
                                Ase_Direccion = Lectura_Hora["Ase_Direccion"].ToString(),
                                Ase_Telefono = Lectura_Hora["Ase_Telefono"].ToString(),
                                Fac_Descuento = Convert.ToInt32(Lectura_Hora["Fac_Descuento"]),
                                Fac_Res = Lectura_Hora["Fac_Res"].ToString(),
                                Fac_Num_Aut = Lectura_Hora["Fac_Num_Aut"].ToString(),
                                DocE_1 = Lectura_Hora["DocE_1"].ToString(),
                                DocE_2 = Lectura_Hora["DocE_2"].ToString(),
                                DocE_3 = Lectura_Hora["DocE_3"].ToString(),
                                DocE_4 = Lectura_Hora["DocE_4"].ToString(),
                                DocE_5 = Convert.ToInt32(DOCE5),
                                DocE_6 = Convert.ToInt32(DOCE6),
                                DocE_7 = Convert.ToDateTime(DOCE7),
                                DocE_8 = Convert.ToDateTime(DOCE8),
                                Fac_Observa = Lectura_Hora["Fac_Observa"].ToString(),
                                Code_QR = repositorioGenerales.GetBytes(QR),
                                Car_Cod = Lectura_Hora["Car_Cod"].ToString(),
                                Car_Item = Lectura_Hora["Car_Item"].ToString(),
                                Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                Car_Val_Un = Convert.ToInt32(Lectura_Hora["Car_Val_Un"]),
                                Total = Convert.ToInt32(Lectura_Hora["Total"])
                            });
                        }
                        return Class_DOCE1;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        List<CXN_CARGOS> IFacturacion.getCargosForInvoice(string Query)
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

                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_CARGOS> C = new List<CXN_CARGOS>();

                        while (Reader.Read() == true)
                        {
                            C.Add(new CXN_CARGOS
                            {
                                Car_Id = Convert.ToInt32(Reader["Car_Id"]),
                                Car_Tipo = Reader["Car_Tipo"].ToString(),
                                Car_Adm_Id = Convert.ToInt32(Reader["Car_Adm_Id"]),
                                Car_Fecha = Convert.ToDateTime(Reader["Car_Fecha"]),
                                Car_Cod = Reader["Car_Cod"].ToString(),
                                Car_Item = Reader["Car_Item"].ToString(),
                                Car_Cant = Convert.ToInt32(Reader["Car_Cant"]),
                                Car_Val_Un = Convert.ToInt32(Reader["Car_Val_Un"]),
                                Car_Val_Tot = Convert.ToInt32(Reader["Car_Val_Tot"]),
                                Car_Detalle = Reader["PAC"].ToString(),
                                Car_Estado = Reader["Pac_TipoId"].ToString() + " " + Reader["Pac_IdNum"].ToString(),
                                Car_Pac = Convert.ToInt32(Reader["Pac_Id"])

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
            catch
            {
                return null;
            }
        }
        void IFacturacion.changeTypeCargo(int posision, string Tipo)
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

                    string Busqueda = "UPDATE CXN_CARGOS " +
                                      "SET Car_Tipo = '" + Tipo + "' " +
                                      "WHERE Car_Id = '" + posision + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        bool IFacturacion.UpdateCUVManual(string FElectron, string CUV)
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
                                      "SET CUV = @param1 " +
                                      "WHERE Homologo = @param2";
                    
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@param1", CUV);
                    Accion.Parameters.AddWithValue("@param2", FElectron);

                    int Guarda = Accion.ExecuteNonQuery();
                    if (Guarda > 0)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        void IFacturacion.excluirCargoFactura(int Posision)
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
                    string Busqueda = "UPDATE CXN_CARGOS " +
                                      "SET Car_Estado = 'A' " +
                                      "WHERE Car_Id = '" + Posision + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        List<FacturasR> IFacturacion.Exporta_DOCE_Orden(int Numero_DOCE, int cia, string Tipo)
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
                    String Cargar_Hora = "SELECT C.Com_Nombre, C.Com_Identificador, C.Com_Direccion, C.Com_Telefono, " +
                                         "C.Com_Identificacion, A.Ase_Descripcion, A.Ase_NitCia, CA.Car_Cod, CA.Car_Item, CA.Car_Val_Un, " +
                                         "A.Ase_Telefono, A.Ase_Direccion, F.Fac_Num_Fac, " +
                                         "F.Fac_Fecha, F.Fac_Res, F.Fac_Fecha_Has, F.Fac_Fecha_Des, F.Fac_Num_Aut, " +
                                         "F.Fac_Descuento, F.Fac_Observa, " +
                                         "F.DocE_1, F.DocE_2, F.DocE_3, F.DocE_4, F.DocE_5, " +
                                         "F.DocE_6, F.DocE_7, F.DocE_8, F.Fac_Observa, " +
                                         "SUM(CAST(CA.Car_Cant AS int)) AS Cantidad, " +
                                         "SUM(CAST(CA.Car_Val_Tot AS int)) AS Total " +
                                         "FROM CXN_FACTURA F " +
                                         "INNER JOIN CXN_CARGOS CA ON F.Fac_Num_Fac = CA.Car_Factura " +
                                         "INNER JOIN CXN_ASEGURADORA A ON F.Fac_Ase = A.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA C ON F.Fac_Cia = C.Com_Identificador " +
                                         "WHERE F.Fac_Num_Fac = '" + Numero_DOCE + "' " +
                                         "AND F.Fac_Tipo_Doc = '" + Tipo + "' " +
                                         "AND F.Fac_Cia = '" + cia + "' " +
                                         "AND CA.Car_Tipo_Doc = '" + Tipo + "' " +
                                         "AND CA.Car_Cia = '" + cia + "' " +
                                         "GROUP BY C.Com_Nombre, C.Com_Identificador, C.Com_Direccion, C.Com_Telefono, " +
                                         "C.Com_Identificacion, A.Ase_Descripcion, A.Ase_NitCia, " +
                                         "A.Ase_Telefono, A.Ase_Direccion, F.Fac_Num_Fac, " +
                                         "F.Fac_Fecha, F.Fac_Res, F.Fac_Fecha_Has, F.Fac_Fecha_Des, F.Fac_Num_Aut, " +
                                         "F.Fac_Descuento, F.Fac_Observa, " +
                                         "F.DocE_1, F.DocE_2, F.DocE_3, F.DocE_4, F.DocE_5, " +
                                         "F.DocE_6, F.DocE_7, F.DocE_8, F.Fac_Observa, CA.Car_Cod, CA.Car_Item, CA.Car_Val_Un";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<FacturasR> Class_DOCE1 = new List<FacturasR>();
                        var Total = SubTotal(Convert.ToInt32(Numero_DOCE), cia, Tipo);
                        int DOCE5, DOCE6;
                        DateTime DOCE7, DOCE8;

                        while (Lectura_Hora.Read() == true)
                        {
                            string Letra;
                            double Neto = Convert.ToInt32(Total) - Convert.ToInt32(Lectura_Hora["Fac_Descuento"]);
                            Letra = repositorioFacturacion.enletras(Convert.ToInt32(Neto).ToString()).ToUpper() + " PESOS";

                            if (Lectura_Hora["DocE_5"] == DBNull.Value)
                            {
                                DOCE5 = 0;
                            }
                            else
                            {
                                DOCE5 = Convert.ToInt32(Lectura_Hora["DocE_5"]);
                            }

                            if (Lectura_Hora["DocE_6"] == DBNull.Value)
                            {
                                DOCE6 = 0;
                            }
                            else
                            {
                                DOCE6 = Convert.ToInt32(Lectura_Hora["DocE_6"]);
                            }

                            DateTime H7 = Convert.ToDateTime("2000-01-01");
                            DateTime H8 = Convert.ToDateTime("2000-01-01");

                            if (Lectura_Hora["DocE_7"] == DBNull.Value)
                            {
                                DOCE7 = H7;
                            }
                            else
                            {
                                DOCE7 = Convert.ToDateTime(Lectura_Hora["DocE_7"]);
                            }

                            if (Lectura_Hora["DocE_8"] == DBNull.Value)
                            {
                                DOCE8 = H8;
                            }
                            else
                            {
                                DOCE8 = Convert.ToDateTime(Lectura_Hora["DocE_8"]);
                            }

                            Class_DOCE1.Add(new FacturasR
                            {
                                Fac_Total = Convert.ToInt32(Total), //total sin descuentos
                                Fac_Neto = Convert.ToInt32(Neto), //neto a pagar
                                Fac_Num_Fac = Numero_DOCE,
                                EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(),
                                EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                Fac_Fecha_Des = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Des"]),
                                Fac_Fecha_Has = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Has"]),
                                FechaBase = Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]),
                                PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                Ase_NitCia = Lectura_Hora["Ase_NitCia"].ToString(),
                                Ase_Direccion = Lectura_Hora["Ase_Direccion"].ToString(),
                                Ase_Telefono = Lectura_Hora["Ase_Telefono"].ToString(),
                                Fac_Descuento = Convert.ToInt32(Lectura_Hora["Fac_Descuento"]),
                                Fac_Res = Lectura_Hora["Fac_Res"].ToString(),
                                Fac_Num_Aut = Lectura_Hora["Fac_Num_Aut"].ToString(),
                                DocE_1 = Lectura_Hora["DocE_1"].ToString(),
                                DocE_2 = Lectura_Hora["DocE_2"].ToString(),
                                DocE_3 = Lectura_Hora["DocE_3"].ToString(),
                                DocE_4 = Lectura_Hora["DocE_4"].ToString(),
                                DocE_5 = DOCE5,
                                DocE_6 = DOCE6,
                                DocE_7 = Convert.ToDateTime(DOCE7),
                                DocE_8 = Convert.ToDateTime(DOCE8),
                                Fac_Observa = Lectura_Hora["Fac_Observa"].ToString(),
                                Letras = Letra,
                                Car_Cod = Lectura_Hora["Car_Cod"].ToString(),
                                Car_Item = Lectura_Hora["Car_Item"].ToString(),
                                Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                Car_Val_Un = Convert.ToInt32(Lectura_Hora["Car_Val_Un"]),
                                Total = Convert.ToInt32(Lectura_Hora["Total"])
                            });
                        }
                        return Class_DOCE1;
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
        bool IFacturacion.Graba_Factura_Orden(CXN_FACTURA F)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                DateTime Hoy = DateTime.Now.Date;

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string FPago = string.IsNullOrEmpty(F.FormaPago) ?  "Efectivo" : F.FormaPago;

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_FACTURA " +
                                                     "(Fac_Num_Fac, " +
                                                      "Fac_Res, " +
                                                      "Homologo, " +
                                                      "Fac_Estado, " +
                                                      "Fac_Ase, " +
                                                      "Fac_Cia, " +
                                                      "Fac_Pac, " +
                                                      "Fac_Fecha, " +
                                                      "Fac_Fecha_Des, " +
                                                      "Fac_Fecha_Has, " +
                                                      "Fac_Num_Aut, " +
                                                      "Fac_Descuento, " +
                                                      "Fac_Observa, " +
                                                      "Fac_Tipo_Doc, " +
                                                      "Fac_ConSub, " +
                                                      "Fac_Usr_Graba," +
                                                      "VrCompartido," +
                                                      "Copago," +
                                                      "Anticipo," +
                                                      "CodPrestador," +
                                                      "ContratoPoliza," +
                                                      "Cobertura," +
                                                      "ModPago, " +
                                                      "DiasVencimiento, " +
                                                      "MetodoPago, " +
                                                      "MedioPago, " +
                                                      "FormaPago, " +
                                                      "PercentICA, " +
                                                      "PercentFUENTE, " +
                                                      "Num_Cruce) " +
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
                                                      "@param16," +
                                                      "@param17," +
                                                      "@param18," +
                                                      "@param19," +
                                                      "@param20," +
                                                      "@param21," +
                                                      "@param22," +
                                                      "@param23, " +
                                                      "@param24, " +
                                                      "@param25, " +
                                                      "@param26, " +
                                                      "@param27, " +
                                                      "@param28, " +
                                                      "@param29, " +
                                                      "@param30)", con);

                    cmd.Parameters.AddWithValue("@param1", F.Fac_Num_Fac);
                    cmd.Parameters.AddWithValue("@param2", F.Fac_Res);
                    cmd.Parameters.AddWithValue("@param3", F.Homologo);
                    cmd.Parameters.AddWithValue("@param4", F.Fac_Estado);
                    cmd.Parameters.AddWithValue("@param5", F.Fac_Ase);
                    cmd.Parameters.AddWithValue("@param6", F.Fac_Cia);
                    cmd.Parameters.AddWithValue("@param7", F.Fac_Pac);
                    cmd.Parameters.Add(new SqlParameter("@param8", SqlDbType.DateTime)).Value = Hoy;
                    cmd.Parameters.Add(new SqlParameter("@param9", SqlDbType.DateTime)).Value = F.Fac_Fecha_Des.Date;
                    cmd.Parameters.Add(new SqlParameter("@param10", SqlDbType.DateTime)).Value = F.Fac_Fecha_Has.Date;
                    cmd.Parameters.AddWithValue("@param11", F.Fac_Num_Aut);
                    cmd.Parameters.AddWithValue("@param12", F.Fac_Descuento);
                    cmd.Parameters.AddWithValue("@param13", F.Fac_Observa);
                    cmd.Parameters.AddWithValue("@param14", F.Fac_Tipo_Doc);
                    cmd.Parameters.AddWithValue("@param15", F.Fac_ConSub);
                    cmd.Parameters.AddWithValue("@param16", F.Fac_Usr_Graba);

                    cmd.Parameters.AddWithValue("@param17", F.VrCompartido);
                    cmd.Parameters.AddWithValue("@param18", F.Copago);
                    cmd.Parameters.AddWithValue("@param19", F.Anticipo);
                    cmd.Parameters.AddWithValue("@param20", F.CodPrestador);
                    cmd.Parameters.AddWithValue("@param21", F.ContratoPoliza);
                    cmd.Parameters.AddWithValue("@param22", F.Cobertura);
                    cmd.Parameters.AddWithValue("@param23", F.ModPago);

                    cmd.Parameters.AddWithValue("@param24", F.DiasVencimiento);
                    cmd.Parameters.AddWithValue("@param25", F.MetodoPago);
                    cmd.Parameters.AddWithValue("@param26", F.MedioPago);
                    cmd.Parameters.AddWithValue("@param27", FPago);
                    cmd.Parameters.AddWithValue("@param28", F.PercentICA);
                    cmd.Parameters.AddWithValue("@param29", F.PercentFUENTE);
                    cmd.Parameters.AddWithValue("@param30", F.Num_Cruce);

                    int s = cmd.ExecuteNonQuery();
                    return s > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        int IFacturacion.getTotalFac(int Cia, int Orden)
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

                    String Cargar_Hora2 = "SELECT SUM(Car_Val_Tot) AS Total " +
                                          "FROM CXN_CARGOS " +
                                          "WHERE Car_Cia = '" + Cia + "' " +
                                          "AND Car_Factura = '" + Orden + "'";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora2["Total"]);
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
        bool IFacturacion.UpdateFuenteICA(int fuente, int ica, int orden, int cia)
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
                                           "SET " +
                                           "VrICA = '" + Convert.ToInt32(ica) + "', " +
                                           "VrFUENTE = '" + Convert.ToInt32(fuente) + "' " +
                                           "WHERE Fac_Num_Fac = '" + orden + "' " +
                                           "AND Fac_Cia = '" + cia + "' ";

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
        bool IFacturacion.Actualiza_Cargo_Facturado(CXN_FACTURA F, string AFacturar, bool EsGlobal)
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

                    string Busqueda = "";

                    if (EsGlobal == false)
                    {
                        switch (AFacturar)
                        {
                            case "Todo":
                                Busqueda = "UPDATE CXN_CARGOS " +
                                           "SET Car_Estado = 'F', " +
                                           "Car_Factura = '" + F.Fac_Num_Fac + "', " +
                                           "Car_Tipo_Doc = '" + F.Fac_Tipo_Doc + "' " +
                                           "WHERE Car_Fecha BETWEEN '" + Convert.ToDateTime(F.Fac_Fecha_Des).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(F.Fac_Fecha_Has).ToString(getData["Format_Fecha"]) + "' " +
                                           "AND Car_Pac = '" + F.Fac_Pac + "' " +
                                           "AND Car_Estado = 'G' " +
                                           "AND Car_Cia = '" + F.Fac_Cia + "' " +
                                           "AND Car_Ase = '" + F.Fac_Ase + "'";
                                break;

                            case "Curaciones y Consultas":
                                Busqueda = "UPDATE CXN_CARGOS " +
                                           "SET Car_Estado = 'F', " +
                                           "Car_Factura = '" + F.Fac_Num_Fac + "', " +
                                           "Car_Tipo_Doc = '" + F.Fac_Tipo_Doc + "' " +
                                           "WHERE Car_Fecha BETWEEN '" + Convert.ToDateTime(F.Fac_Fecha_Des).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(F.Fac_Fecha_Has).ToString(getData["Format_Fecha"]) + "' " +
                                           "AND Car_Pac = '" + F.Fac_Pac + "' " +
                                           "AND Car_Estado = 'G' " +
                                           "AND Car_Cia = '" + F.Fac_Cia + "' " +
                                           "AND Car_Ase = '" + F.Fac_Ase + "' " +
                                           "AND Car_Tipo_Serv IN ('CU','MG')";
                                break;

                            case "Terapias":
                                Busqueda = "UPDATE CXN_CARGOS " +
                                           "SET Car_Estado = 'F', " +
                                           "Car_Factura = '" + F.Fac_Num_Fac + "', " +
                                           "Car_Tipo_Doc = '" + F.Fac_Tipo_Doc + "' " +
                                           "WHERE Car_Fecha BETWEEN '" + Convert.ToDateTime(F.Fac_Fecha_Des).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(F.Fac_Fecha_Has).ToString(getData["Format_Fecha"]) + "' " +
                                           "AND Car_Pac = '" + F.Fac_Pac + "' " +
                                           "AND Car_Estado = 'G' " +
                                           "AND Car_Cia = '" + F.Fac_Cia + "' " +
                                           "AND Car_Ase = '" + F.Fac_Ase + "' " +
                                           "AND Car_Tipo_Serv IN ('TF','TO','PS')";
                                break;

                            case "Fisiatria":
                                Busqueda = "UPDATE CXN_CARGOS " +
                                           "SET Car_Estado = 'F', " +
                                           "Car_Factura = '" + F.Fac_Num_Fac + "', " +
                                           "Car_Tipo_Doc = '" + F.Fac_Tipo_Doc + "' " +
                                           "WHERE Car_Fecha BETWEEN '" + Convert.ToDateTime(F.Fac_Fecha_Des).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(F.Fac_Fecha_Has).ToString(getData["Format_Fecha"]) + "' " +
                                           "AND Car_Pac = '" + F.Fac_Pac + "' " +
                                           "AND Car_Estado = 'G' " +
                                           "AND Car_Cia = '" + F.Fac_Cia + "' " +
                                           "AND Car_Ase = '" + F.Fac_Ase + "' " +
                                           "AND Car_Tipo_Serv = 'FI'";
                                break;

                            case "Terapias y Fisiatria":
                                Busqueda = "UPDATE CXN_CARGOS " +
                                           "SET Car_Estado = 'F', " +
                                           "Car_Factura = '" + F.Fac_Num_Fac + "', " +
                                           "Car_Tipo_Doc = '" + F.Fac_Tipo_Doc + "' " +
                                           "WHERE Car_Fecha BETWEEN '" + Convert.ToDateTime(F.Fac_Fecha_Des).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(F.Fac_Fecha_Has).ToString(getData["Format_Fecha"]) + "' " +
                                           "AND Car_Pac = '" + F.Fac_Pac + "' " +
                                           "AND Car_Estado = 'G' " +
                                           "AND Car_Cia = '" + F.Fac_Cia + "' " +
                                           "AND Car_Ase = '" + F.Fac_Ase + "' " +
                                           "AND Car_Tipo_Serv IN ('TF','TO','PS','FI')";
                                break;

                            case "OtrasFacturas":
                                Busqueda = "UPDATE CXN_CARGOS " +
                                           "SET Car_Estado = 'F', " +
                                           "Car_Factura = '" + F.Fac_Num_Fac + "', " +
                                           "Car_Tipo_Doc = 'OP' " +
                                           "WHERE Car_Fecha = '" + Convert.ToDateTime(DateTime.Now.Date).ToString(getData["Format_Fecha"]) + "' " +
                                           "AND Car_Pac = '0' " +
                                           "AND Car_Estado = 'G' " +
                                           "AND Car_Cia = '" + F.Fac_Cia + "' " +
                                           "AND Car_Ase = '" + F.Fac_Ase + "'";
                                break;

                            case "Radiologia":
                                Busqueda = "UPDATE CXN_CARGOS " +
                                           "SET Car_Estado = 'F', " +
                                           "Car_Factura = '" + F.Fac_Num_Fac + "', " +
                                           "Car_Tipo_Doc = '" + F.Fac_Tipo_Doc + "' " +
                                           "WHERE Car_Fecha BETWEEN '" + Convert.ToDateTime(F.Fac_Fecha_Des).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(F.Fac_Fecha_Has).ToString(getData["Format_Fecha"]) + "' " +
                                           "AND Car_Pac = '" + F.Fac_Pac + "' " +
                                           "AND Car_Estado = 'G' " +
                                           "AND Car_Cia = '" + F.Fac_Cia + "' " +
                                           "AND Car_Ase = '" + F.Fac_Ase + "' " +
                                           "AND Car_Tipo_Serv = 'RA'";
                                break;

                            default:
                                return false;
                        }
                        SqlCommand Accion = new SqlCommand(Busqueda, con);
                        int Guarda;
                        Guarda = Accion.ExecuteNonQuery();
                        return true;
                    }

                    if (EsGlobal == true)
                    {
                        switch (AFacturar)
                        {
                            case "Todo":
                                Busqueda = "UPDATE CXN_CARGOS " +
                                           "SET Car_Estado = 'F', " +
                                           "Car_Factura = '" + F.Fac_Num_Fac + "', " +
                                           "Car_Tipo_Doc = '" + F.Fac_Tipo_Doc + "' " +
                                           "WHERE Car_Fecha BETWEEN '" + Convert.ToDateTime(F.Fac_Fecha_Des).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(F.Fac_Fecha_Has).ToString(getData["Format_Fecha"]) + "' " +
                                           "AND Car_Estado = 'G' " +
                                           "AND Car_Cia = '" + F.Fac_Cia + "' " +
                                           "AND Car_Ase = '" + F.Fac_Ase + "'";
                                break;

                            case "Curaciones y Consultas":
                                Busqueda = "UPDATE CXN_CARGOS " +
                                           "SET Car_Estado = 'F', " +
                                           "Car_Factura = '" + F.Fac_Num_Fac + "', " +
                                           "Car_Tipo_Doc = '" + F.Fac_Tipo_Doc + "' " +
                                           "WHERE Car_Fecha BETWEEN '" + Convert.ToDateTime(F.Fac_Fecha_Des).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(F.Fac_Fecha_Has).ToString(getData["Format_Fecha"]) + "' " +
                                           "AND Car_Estado = 'G' " +
                                           "AND Car_Cia = '" + F.Fac_Cia + "' " +
                                           "AND Car_Ase = '" + F.Fac_Ase + "' " +
                                           "AND Car_Tipo_Serv IN ('CU','MG')";
                                break;

                            case "Terapias":
                                Busqueda = "UPDATE CXN_CARGOS " +
                                           "SET Car_Estado = 'F', " +
                                           "Car_Factura = '" + F.Fac_Num_Fac + "', " +
                                           "Car_Tipo_Doc = '" + F.Fac_Tipo_Doc + "' " +
                                           "WHERE Car_Fecha BETWEEN '" + Convert.ToDateTime(F.Fac_Fecha_Des).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(F.Fac_Fecha_Has).ToString(getData["Format_Fecha"]) + "' " +
                                           "AND Car_Estado = 'G' " +
                                           "AND Car_Cia = '" + F.Fac_Cia + "' " +
                                           "AND Car_Ase = '" + F.Fac_Ase + "' " +
                                           "AND Car_Tipo_Serv IN ('TF','TO','PS')";
                                break;

                            case "Fisiatria":
                                Busqueda = "UPDATE CXN_CARGOS " +
                                           "SET Car_Estado = 'F', " +
                                           "Car_Factura = '" + F.Fac_Num_Fac + "', " +
                                           "Car_Tipo_Doc = '" + F.Fac_Tipo_Doc + "' " +
                                           "WHERE Car_Fecha BETWEEN '" + Convert.ToDateTime(F.Fac_Fecha_Des).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(F.Fac_Fecha_Has).ToString(getData["Format_Fecha"]) + "' " +
                                           "AND Car_Estado = 'G' " +
                                           "AND Car_Cia = '" + F.Fac_Cia + "' " +
                                           "AND Car_Ase = '" + F.Fac_Ase + "' " +
                                           "AND Car_Tipo_Serv = 'FI'";
                                break;

                            case "Terapias y Fisiatria":
                                Busqueda = "UPDATE CXN_CARGOS " +
                                           "SET Car_Estado = 'F', " +
                                           "Car_Factura = '" + F.Fac_Num_Fac + "', " +
                                           "Car_Tipo_Doc = '" + F.Fac_Tipo_Doc + "' " +
                                           "WHERE Car_Fecha BETWEEN '" + Convert.ToDateTime(F.Fac_Fecha_Des).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(F.Fac_Fecha_Has).ToString(getData["Format_Fecha"]) + "' " +
                                           "AND Car_Estado = 'G' " +
                                           "AND Car_Cia = '" + F.Fac_Cia + "' " +
                                           "AND Car_Ase = '" + F.Fac_Ase + "' " +
                                           "AND Car_Tipo_Serv IN ('TF','TO','PS','FI')";
                                break;

                            default:
                                return false;
                        }
                        SqlCommand Accion = new SqlCommand(Busqueda, con);
                        int Guarda;
                        Guarda = Accion.ExecuteNonQuery();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        (List<CXN_CARGOS> lista, int ValorFac) IFacturacion.FiltroFacturas(string Query)
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

                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_CARGOS> L = new List<CXN_CARGOS>();
                        int Tot = 0;

                        while (Reader.Read() == true)
                        {
                            L.Add(new CXN_CARGOS
                            {
                                Car_Tipo = Reader["Car_Tipo"].ToString(),
                                Car_Cod = Reader["Car_Cod"].ToString(),
                                Car_Item = Reader["Car_Item"].ToString(),
                                Car_Cant = Convert.ToInt32(Reader["Cantidad"]),
                                Car_Val_Un = Convert.ToInt32(Reader["Car_Val_Un"]),
                                Car_Val_Tot = Convert.ToInt32(Reader["Total"])
                            });

                            Tot = Tot + Convert.ToInt32(Reader["Total"]);
                        }

                        return (L, Tot);
                    }
                    else
                    {
                        return (null, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return (null, 0);
            }
        }
        (int Cantidad, int Total) Sumatoria(int factura)
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
                    String Cargar_Hora = "SELECT C.Car_Factura, SUM(CAST(C.Car_Val_Tot as int)) AS Total, " +
                                         "SUM(CAST(C.Car_Cant as int)) AS Cantidad " +
                                         "FROM CXN_CARGOS C " +
                                         "INNER JOIN CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac " +
                                         "WHERE F.Fac_Estado = 'F' " +
                                         "AND C.Car_Estado = 'F' " +
                                         "AND C.Car_Tipo_Doc = 'DE' " +
                                         "AND F.Fac_Tipo_Doc = 'DE' " +
                                         "AND F.Fac_Num_Fac = '" + factura + "' " +
                                         "GROUP BY C.Car_Factura";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return (Convert.ToInt32(Lectura_Hora["Cantidad"]), Convert.ToInt32(Lectura_Hora["Total"]));
                    }
                    else
                    {
                        return (0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return (0, 0);
            }
        }
        List<Class_DetailDE> IFacturacion.getDetailDE(int Factura)
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
                    String Cargar_Hora = "SELECT CI.Com_Nombre, A.Ase_Descripcion, C.Car_Fecha, C.Car_Adm_Id, " +
                                         "P.Pac_PrimerA + '  ' + P.Pac_SegundoA + '  ' + P.Pac_PrimerN + '  ' + P.Pac_SegundoN AS Nombres, " +
                                         "C.Car_Item, P.Pac_TipoId, P.Pac_IdNum, H.Hor_ValDerechos, C.Car_Val_Un, H.Hor_Autoriza, C.Car_Cant, C.Car_Val_Tot, " +
                                         "SUM(CAST(C.Car_Val_Tot as int)) AS Total, " +
                                         "SUM(CAST(C.Car_Cant as int)) AS Cantidad " +
                                         "FROM CXN_CARGOS C " +
                                         "INNER JOIN CXN_HORARIO H ON C.Car_Adm_Id = H.Hor_Id " +
                                         "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac " +
                                         "INNER JOIN CXN_CIA CI ON C.Car_Cia = CI.Com_Identificador " +
                                         "INNER JOIN CXN_ASEGURADORA A ON C.Car_Ase = A.Ase_Identificador " +
                                         "WHERE F.Fac_Estado = 'F' " +
                                         "AND C.Car_Estado = 'F' " +
                                         "AND C.Car_Tipo_Doc = 'DE' " +
                                         "AND F.Fac_Tipo_Doc = 'DE' " +
                                         "AND F.Fac_Num_Fac = '" + Factura + "' " +
                                         "GROUP BY CI.Com_Nombre, A.Ase_Descripcion, C.Car_Fecha, C.Car_Adm_Id, " +
                                         "C.Car_Item, P.Pac_TipoId, P.Pac_IdNum, H.Hor_ValDerechos, C.Car_Val_Un, H.Hor_Autoriza, C.Car_Cant, C.Car_Val_Tot, " +
                                         "P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<Class_DetailDE> Class_DetailDE1 = new List<Class_DetailDE>();

                        var getSums = Sumatoria(Factura);

                        DateTime Hoy = DateTime.Now;

                        while (Lectura_Hora.Read() == true)
                        {
                            Class_DetailDE1.Add(new Class_DetailDE
                            {
                                EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                Hoy = Convert.ToDateTime(Hoy).ToString(getData["Format_Fecha"]),
                                Texto1 = "000000" + Factura.ToString(),
                                PacienteDireccion = Convert.ToInt32(getSums.Cantidad).ToString(), //cantidad cargos
                                Gran_Tot = "$ " + Convert.ToInt32(getSums.Total).ToString(),
                                FechaBase = Convert.ToDateTime(Lectura_Hora["Car_Fecha"]),
                                Car_Adm_Id = Lectura_Hora["Car_Adm_Id"].ToString(),
                                Nombres = Lectura_Hora["Nombres"].ToString(),
                                Car_Item = Lectura_Hora["Car_Item"].ToString(),
                                PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                Hor_ValDerechos = Lectura_Hora["Hor_ValDerechos"].ToString(),
                                Car_Val_Un = "$ " + Convert.ToInt32(Lectura_Hora["Car_Val_Un"]).ToString(),
                                Hor_Autoriza = Lectura_Hora["Hor_Autoriza"].ToString(),
                                Car_Cant = Lectura_Hora["Car_Cant"].ToString(),
                                Car_Val_Tot = "$ " + Convert.ToInt32(Lectura_Hora["Car_Val_Tot"]).ToString()
                            });
                        }

                        return Class_DetailDE1;

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
        bool IFacturacion.Anula_Factura(CXN_FACTURA F)
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

                    String Cargar_Cod_Prof = "Select Fac_Num_Fac " +
                                             "From CXN_FACTURA " +
                                             "WHERE Fac_Num_Fac = '" + F.Fac_Num_Fac + "' " +
                                             "AND Fac_Estado = 'F' " +
                                             "AND Fac_Cia = '" + F.Fac_Cia + "' " +
                                             "AND Fac_Tipo_Doc = '" + F.Fac_Tipo_Doc + "'";
                    SqlCommand Carga_Cod_Prof = new SqlCommand(Cargar_Cod_Prof, con);
                    SqlDataReader Lectura_Cod_Prof = (Carga_Cod_Prof.ExecuteReader());
                    if (Lectura_Cod_Prof.Read() == true)
                    {
                        string Busqueda = "UPDATE CXN_CARGOS " +
                                          "SET Car_Estado = 'G', " +
                                          "Car_Factura = '' " +
                                          "WHERE Car_Factura = '" + F.Fac_Num_Fac + "' " +
                                          "AND Car_Estado = 'F' " +
                                          "AND Car_Cia = '" + F.Fac_Cia + "' " +
                                          "AND Car_Tipo_Doc = '" + F.Fac_Tipo_Doc + "'";
                        SqlCommand Accion = new SqlCommand(Busqueda, con);
                        int Guarda;
                        Guarda = Accion.ExecuteNonQuery();

                        string Busqueda2 = "UPDATE CXN_FACTURA " +
                                           "SET Fac_Estado = 'A' " +
                                           "WHERE Fac_Num_Fac = '" + F.Fac_Num_Fac + "' " +
                                           "AND Fac_Estado = 'F' " +
                                           "AND Fac_Cia = '" + F.Fac_Cia + "' " +
                                           "AND Fac_Tipo_Doc = '" + F.Fac_Tipo_Doc + "'";
                        SqlCommand Accion2 = new SqlCommand(Busqueda2, con);
                        int Guarda2;
                        Guarda2 = Accion2.ExecuteNonQuery();

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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }             
        CXN_FACTURA IFacturacion.consAutorizacion(string Numeero)
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

                    String Query = "SELECT * FROM CXN_FACTURA WHERE Fac_Num_Aut = @param1";
                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Numeero);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_FACTURA f = new CXN_FACTURA 
                                { 
                                    Fac_Num_Fac = Convert.ToInt32(Reader["Fac_Num_Fac"]),
                                    Homologo = Reader["Homologo"].ToString(),
                                    Fac_Fecha = Convert.ToDateTime(Reader["Fac_Fecha"]),
                                    Fac_Fecha_Des = Convert.ToDateTime(Reader["Fac_Fecha_Des"]),
                                    Fac_Fecha_Has = Convert.ToDateTime(Reader["Fac_Fecha_Has"])
                                };

                                return f;
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
        CXN_FACTURA IFacturacion.getFacElectronica(string Numeero) 
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

                    String Query = "SELECT * FROM CXN_FACTURA WHERE Homologo = @param1";
                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Numeero);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_FACTURA f = new CXN_FACTURA
                                {
                                    Fac_Num_Fac = Convert.ToInt32(Reader["Fac_Num_Fac"]),
                                    Homologo = Reader["Homologo"].ToString(),
                                    Fac_Fecha = Convert.ToDateTime(Reader["Fac_fecha"]),
                                    Fac_Pac = Convert.ToInt32(Reader["Fac_Pac"]),
                                    Fac_Cia = Convert.ToInt32(Reader["Fac_Cia"]),
                                    Cufe = Reader["Cufe"].ToString(),
                                    CUV = Reader["CUV"].ToString()
                                };

                                return f;
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
        bool IFacturacion.existeHomologo(string Homologo, int Cia)
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
                    String Cargar_Hora = "SELECT TOP 1 Homologo " +
                                         "FROM CXN_FACTURA " +
                                         "WHERE Homologo = @param1 " +
                                         "AND Fac_Cia = @param2 " +
                                         "ORDER BY Fac_Id DESC";
                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Homologo);
                        Carga_Command.Parameters.AddWithValue("@param2", Cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {                                
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }                        
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        void IFacturacion.updateDatosGlosas(CXN_PAGOS F)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_PAGOS " +
                                                    "(FRadica, " +
                                                     "VrPagado, " +
                                                     "FPago, " +
                                                     "RT, " +
                                                     "Homologo, " +
                                                     "Cia) " +
                            "values                  (@param1, " +
                                                     "@param2, " +
                                                     "@param3, " +
                                                     "@param4, " +
                                                     "@param5, " +
                                                     "@param6)", con);

                    cmd.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = F.FRadica;
                    cmd.Parameters.AddWithValue("@param2", F.VrPagado);
                    cmd.Parameters.Add(new SqlParameter("@param3", SqlDbType.DateTime)).Value = F.FPago;
                    cmd.Parameters.AddWithValue("@param4", F.RT);
                    cmd.Parameters.AddWithValue("@param5", F.Homologo);
                    cmd.Parameters.AddWithValue("@param6", F.Fac_Cia);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void IFacturacion.UpdateCUV(string FE, string CUV)
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
                                      "SET CUV = '" + CUV + "' " +
                                      "WHERE Homologo = '" + FE + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        CXN_FACTURA IFacturacion.getFacturasTable(string Homologo, int Cia)
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

                    String Cargar_Hora = "SELECT *  " +
                                         "FROM CXN_FACTURA " +
                                         "WHERE Homologo = @param1 " +
                                         "AND Fac_Cia = @param2 " +
                                         "ORDER BY Fac_Id DESC";
                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Homologo);
                        Carga_Command.Parameters.AddWithValue("@param2", Cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_FACTURA f = new CXN_FACTURA
                                {
                                    Fac_Num_Fac = Convert.ToInt32(Lectura_Hora["Fac_Num_Fac"]),
                                    Fac_Estado = Lectura_Hora["Fac_Estado"].ToString(),
                                    Fac_Ase = Convert.ToInt32(Lectura_Hora["Fac_Ase"]),
                                    Fac_Cia = Convert.ToInt32(Lectura_Hora["Fac_Cia"]),
                                    Fac_Fecha = Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]),
                                    Fac_Fecha_Des = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Des"]),
                                    Fac_Fecha_Has = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Has"]),
                                    Fac_Num_Aut = Lectura_Hora["Fac_Num_Aut"].ToString(),
                                    Fac_Descuento = Lectura_Hora["Fac_Descuento"].ToString(),
                                    Fac_Observa = Lectura_Hora["Fac_Observa"].ToString(),
                                    Fac_Usr_Graba = Lectura_Hora["Fac_Usr_Graba"].ToString(),
                                    Fac_Pac = Convert.ToInt32(Lectura_Hora["Fac_Pac"]),
                                    Fac_Res = Lectura_Hora["Fac_Res"].ToString(),
                                    Fac_Tipo_Doc = Lectura_Hora["Fac_Tipo_Doc"].ToString(),
                                    Fac_ConSub = Lectura_Hora["Fac_ConSub"].ToString(),
                                    VrCompartido = Convert.ToInt32(Lectura_Hora["VrCompartido"]),
                                    Copago = Convert.ToInt32(Lectura_Hora["Copago"]),
                                    Anticipo = Convert.ToInt32(Lectura_Hora["Anticipo"]),
                                    CodPrestador = Lectura_Hora["CodPrestador"].ToString(),
                                    ContratoPoliza = Lectura_Hora["ContratoPoliza"].ToString(),
                                    Cobertura = Lectura_Hora["Cobertura"].ToString(),
                                    ModPago = Lectura_Hora["ModPago"].ToString(),
                                    DiasVencimiento = Convert.ToInt32(Lectura_Hora["DiasVencimiento"]),
                                    MetodoPago = Lectura_Hora["MetodoPago"].ToString(),
                                    MedioPago = Lectura_Hora["MedioPago"].ToString(),
                                    FormaPago = Lectura_Hora["FormaPago"].ToString()
                                };

                                return f;
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
        bool IFacturacion.InsertCopyTableFacturas(CXN_FACTURA F)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                DateTime Hoy = DateTime.Now.Date;

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_FACTURA " +
                                                     "(Fac_Num_Fac, " +
                                                      "Fac_Estado, " +
                                                      "Fac_Ase, " +
                                                      "Fac_Cia, " +
                                                      "Fac_Fecha, " +
                                                      "Fac_Fecha_Des, " +
                                                      "Fac_Fecha_Has, " +
                                                      "Fac_Num_Aut, " +
                                                      "Fac_Descuento, " +
                                                      "Fac_Observa, " +
                                                      "Fac_Usr_Graba, " +
                                                      "Fac_Pac, " +
                                                      "Homologo, " +
                                                      "Fac_Tipo_Doc, " +
                                                      "Fac_ConSub, " +
                                                      "VrCompartido, " +
                                                      "Copago, " +
                                                      "Anticipo, " +
                                                      "CodPrestador, " +
                                                      "ContratoPoliza, " +
                                                      "Cobertura, " +
                                                      "ModPago, " +
                                                      "DiasVencimiento, " +
                                                      "MetodoPago ," +
                                                      "MedioPago, " +
                                                      "FormaPago) " +
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
                                                      "@param16," +
                                                      "@param17," +
                                                      "@param18," +
                                                      "@param19," +
                                                      "@param20," +
                                                      "@param21," +
                                                      "@param22," +
                                                      "@param23, " +
                                                      "@param24, " +
                                                      "@param25, " +
                                                      "@param26)", con);

                    cmd.Parameters.AddWithValue("@param1", F.Fac_Num_Fac);
                    cmd.Parameters.AddWithValue("@param2", F.Fac_Estado);
                    cmd.Parameters.AddWithValue("@param3", F.Fac_Ase);
                    cmd.Parameters.AddWithValue("@param4", F.Fac_Cia);
                    cmd.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = F.Fac_Fecha;
                    cmd.Parameters.Add(new SqlParameter("@param6", SqlDbType.DateTime)).Value = F.Fac_Fecha_Des.Date;
                    cmd.Parameters.Add(new SqlParameter("@param7", SqlDbType.DateTime)).Value = F.Fac_Fecha_Has.Date;
                    cmd.Parameters.AddWithValue("@param8", F.Fac_Num_Aut);
                    cmd.Parameters.AddWithValue("@param9", F.Fac_Descuento);                   
                    cmd.Parameters.AddWithValue("@param10", F.Fac_Observa);
                    cmd.Parameters.AddWithValue("@param11", F.Fac_Usr_Graba);
                    cmd.Parameters.AddWithValue("@param12", F.Fac_Pac);
                    cmd.Parameters.AddWithValue("@param13", F.Fac_Num_Fac);
                    cmd.Parameters.AddWithValue("@param14", F.Fac_Tipo_Doc);
                    cmd.Parameters.AddWithValue("@param15", F.Fac_ConSub);
                    cmd.Parameters.AddWithValue("@param16", F.VrCompartido);
                    cmd.Parameters.AddWithValue("@param17", F.Copago);
                    cmd.Parameters.AddWithValue("@param18", F.Anticipo);
                    cmd.Parameters.AddWithValue("@param19", F.CodPrestador);
                    cmd.Parameters.AddWithValue("@param20", F.ContratoPoliza);
                    cmd.Parameters.AddWithValue("@param21", F.Cobertura);
                    cmd.Parameters.AddWithValue("@param22", F.ModPago);
                    cmd.Parameters.AddWithValue("@param23", F.DiasVencimiento);
                    cmd.Parameters.AddWithValue("@param24", F.MetodoPago);
                    cmd.Parameters.AddWithValue("@param25", F.MedioPago);
                    cmd.Parameters.AddWithValue("@param26", F.FormaPago);

                    int c = cmd.ExecuteNonQuery();
                    if (c > 0) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        List<CXN_CARGOS> IFacturacion.getCargosTable(int FacZamenis, int Cia)
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

                    String Cargar_Hora = "SELECT *  " +
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Factura = @param1 " +
                                         "AND Car_Cia = @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", FacZamenis);
                        Carga_Command.Parameters.AddWithValue("@param2", Cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_CARGOS> c = new List<CXN_CARGOS>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    c.Add(new CXN_CARGOS {
                                        Car_Adm_Id = string.IsNullOrEmpty(Lectura_Hora["Car_Adm_Id"].ToString()) ? 0 : Convert.ToInt32(Lectura_Hora["Car_Adm_Id"]),
                                        Car_Pac = Convert.ToInt32(Lectura_Hora["Car_Pac"]),
                                        Car_Cia = Convert.ToInt32(Lectura_Hora["Car_Cia"]),
                                        Car_Ase = Convert.ToInt32(Lectura_Hora["Car_Ase"]),
                                        Car_Prof = Convert.ToInt32(Lectura_Hora["Car_Prof"]),
                                        Car_Fecha = Convert.ToDateTime(Lectura_Hora["Car_Fecha"]),
                                        Car_Estado = Lectura_Hora["Car_Estado"].ToString(),
                                        Car_Tipo = Lectura_Hora["Car_Tipo"].ToString(),
                                        Car_Cod = Lectura_Hora["Car_Cod"].ToString(),
                                        Car_Cant = Convert.ToInt32(Lectura_Hora["Car_Cant"]),
                                        Car_Val_Un = Convert.ToInt32(Lectura_Hora["Car_Val_Un"]),
                                        Car_Val_Tot = Convert.ToInt32(Lectura_Hora["Car_Val_Tot"]),
                                        Car_Tipo_Serv = Lectura_Hora["Car_Tipo_Serv"].ToString(),
                                        Car_Item = Lectura_Hora["Car_Item"].ToString(),
                                        Car_Detalle = string.IsNullOrEmpty(Lectura_Hora["Car_Detalle"].ToString()) ? "" : Lectura_Hora["Car_Detalle"].ToString(),
                                        Car_Dx1 = string.IsNullOrEmpty(Lectura_Hora["Car_Dx1"].ToString()) ? "" : Lectura_Hora["Car_Dx1"].ToString(),
                                        Car_Dx2 = string.IsNullOrEmpty(Lectura_Hora["Car_Dx2"].ToString()) ? "" : Lectura_Hora["Car_Dx2"].ToString(),
                                        Car_Dx3 = string.IsNullOrEmpty(Lectura_Hora["Car_Dx3"].ToString()) ? "" : Lectura_Hora["Car_Dx3"].ToString(),
                                        Car_Imp_Dx = string.IsNullOrEmpty(Lectura_Hora["Car_Imp_Dx"].ToString()) ? 0 : Convert.ToInt32(Lectura_Hora["Car_Imp_Dx"]),
                                        Car_Finalidad = string.IsNullOrEmpty(Lectura_Hora["Car_Finalidad"].ToString()) ? 0 : Convert.ToInt32(Lectura_Hora["Car_Finalidad"]),
                                        Car_Ambito = string.IsNullOrEmpty(Lectura_Hora["Car_Ambito"].ToString()) ? 0 : Convert.ToInt32(Lectura_Hora["Car_Ambito"]),
                                        Car_Personal = string.IsNullOrEmpty(Lectura_Hora["Car_Personal"].ToString()) ? 0 : Convert.ToInt32(Lectura_Hora["Car_Personal"]),
                                        Car_CExterna = string.IsNullOrEmpty(Lectura_Hora["Car_CExterna"].ToString()) ? 0 : Convert.ToInt32(Lectura_Hora["Car_CExterna"]),
                                        Car_Finalidad_CO = string.IsNullOrEmpty(Lectura_Hora["Car_Finalidad_CO"].ToString()) ? 0 : Convert.ToInt32(Lectura_Hora["Car_Finalidad_CO"]),
                                        Car_Regimen = string.IsNullOrEmpty(Lectura_Hora["Car_Regimen"].ToString()) ? "1" : Lectura_Hora["Car_Regimen"].ToString(),
                                        Car_Usr_Graba = Lectura_Hora["Car_Usr_Graba"].ToString(),
                                        Car_Tipo_Doc = Lectura_Hora["Car_Tipo_Doc"].ToString(),
                                        Car_IVA = string.IsNullOrEmpty(Lectura_Hora["Car_IVA"].ToString()) ? "1" : Lectura_Hora["Car_IVA"].ToString(),
                                        Car_ICA = string.IsNullOrEmpty(Lectura_Hora["Car_ICA"].ToString()) ? "1" : Lectura_Hora["Car_ICA"].ToString(),
                                        Car_RetFte = string.IsNullOrEmpty(Lectura_Hora["Car_RetFte"].ToString()) ? "1" : Lectura_Hora["Car_RetFte"].ToString(),
                                    });
                                }                               

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
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        void IFacturacion.InsertCopyTableCargos(CXN_CARGOS F)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                DateTime Hoy = DateTime.Now.Date;

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_CARGOS " +
                                                     "(Car_Factura, " +
                                                      "Car_Adm_Id, " +
                                                      "Car_Pac, " +
                                                      "Car_Cia, " +
                                                      "Car_Ase, " +
                                                      "Car_Prof, " +
                                                      "Car_Fecha, " +
                                                      "Car_Estado, " +
                                                      "Car_Tipo, " +
                                                      "Car_Cod, " +
                                                      "Car_Cant, " +
                                                      "Car_Val_Un, " +
                                                      "Car_Val_Tot, " +
                                                      "Car_Tipo_Serv, " +
                                                      "Car_Item, " +
                                                      "Car_Detalle, " +
                                                      "Car_Dx1, " +
                                                      "Car_Dx2, " +
                                                      "Car_Dx3, " +
                                                      "Car_Imp_Dx, " +
                                                      "Car_Finalidad, " +
                                                      "Car_Ambito, " +
                                                      "Car_Personal, " +
                                                      "Car_CExterna ," +
                                                      "Car_Finalidad_CO, " +
                                                      "Car_Regimen, " +
                                                      "Car_Usr_Graba, " +
                                                      "Car_Tipo_Doc, " +
                                                      "Car_IVA, " +
                                                      "Car_ICA, " +
                                                      "Car_RetFte) " +
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
                                                      "@param16," +
                                                      "@param17," +
                                                      "@param18," +
                                                      "@param19," +
                                                      "@param20," +
                                                      "@param21," +
                                                      "@param22," +
                                                      "@param23, " +
                                                      "@param24, " +
                                                      "@param25, " +
                                                      "@param26, " +
                                                      "@param27, " +
                                                      "@param28, " +
                                                      "@param29, " +
                                                      "@param30, " +
                                                      "@param31)", con);

                    cmd.Parameters.AddWithValue("@param1", F.Car_Factura);
                    cmd.Parameters.AddWithValue("@param2", F.Car_Adm_Id);
                    cmd.Parameters.AddWithValue("@param3", F.Car_Pac);
                    cmd.Parameters.AddWithValue("@param4", F.Car_Cia);
                    cmd.Parameters.AddWithValue("@param5", F.Car_Ase);
                    cmd.Parameters.AddWithValue("@param6", F.Car_Prof);
                    cmd.Parameters.Add(new SqlParameter("@param7", SqlDbType.DateTime)).Value = F.Car_Fecha.Date;
                    cmd.Parameters.AddWithValue("@param8", F.Car_Estado);
                    cmd.Parameters.AddWithValue("@param9", F.Car_Tipo);
                    cmd.Parameters.AddWithValue("@param10", F.Car_Cod);
                    cmd.Parameters.AddWithValue("@param11", F.Car_Cant);
                    cmd.Parameters.AddWithValue("@param12", F.Car_Val_Un);
                    cmd.Parameters.AddWithValue("@param13", F.Car_Val_Tot);
                    cmd.Parameters.AddWithValue("@param14", F.Car_Tipo_Serv);
                    cmd.Parameters.AddWithValue("@param15", F.Car_Item);
                    cmd.Parameters.AddWithValue("@param16", F.Car_Detalle == null ? "" : F.Car_Detalle);
                    cmd.Parameters.AddWithValue("@param17", F.Car_Dx1 == null ? "" : F.Car_Dx1);
                    cmd.Parameters.AddWithValue("@param18", F.Car_Dx2 == null ? "" : F.Car_Dx2);
                    cmd.Parameters.AddWithValue("@param19", F.Car_Dx3 == null ? "" : F.Car_Dx3);
                    cmd.Parameters.AddWithValue("@param20", F.Car_Imp_Dx.ToString() == null ? 1 : F.Car_Imp_Dx);
                    cmd.Parameters.AddWithValue("@param21", F.Car_Finalidad.ToString() == null ? 1 : F.Car_Finalidad);
                    cmd.Parameters.AddWithValue("@param22", F.Car_Ambito.ToString() == null ? 1 : F.Car_Ambito);
                    cmd.Parameters.AddWithValue("@param23", F.Car_Personal.ToString() == null ? 1 : F.Car_Personal);
                    cmd.Parameters.AddWithValue("@param24", F.Car_CExterna.ToString() == null ? 1 : F.Car_CExterna);
                    cmd.Parameters.AddWithValue("@param25", F.Car_Finalidad_CO.ToString() == null ? 1 : F.Car_Finalidad_CO);
                    cmd.Parameters.AddWithValue("@param26", F.Car_Regimen == null ? "1" : F.Car_Regimen);
                    cmd.Parameters.AddWithValue("@param27", F.Car_Usr_Graba);
                    cmd.Parameters.AddWithValue("@param28", F.Car_Tipo_Doc);
                    cmd.Parameters.AddWithValue("@param29", F.Car_IVA.ToString() == null ? "0" : F.Car_IVA);
                    cmd.Parameters.AddWithValue("@param30", F.Car_ICA.ToString() == null ? "0" : F.Car_ICA);
                    cmd.Parameters.AddWithValue("@param31", F.Car_RetFte.ToString() == null ? "0" : F.Car_RetFte);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);                
            }
        }
        List<int> IFacturacion.GetAdmitionByType(string TipoCargo, int IdPaciente, DateTime Desde, DateTime Hasta, int Ase)
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

                    String Cargar_Hora = "SELECT DISTINCT Car_Adm_Id " +
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Fecha BETWEEN @param1 AND @param2 " +
                                         "AND Car_Tipo = @param3 " +
                                         "AND Car_Pac = @param4 " +
                                         "AND Car_Estado = 'G' " +
                                         "AND Car_Ase = @param5";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Convert.ToDateTime(Desde.Date));
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(Hasta.Date));
                        Carga_Command.Parameters.AddWithValue("@param3", TipoCargo);
                        Carga_Command.Parameters.AddWithValue("@param4", IdPaciente);
                        Carga_Command.Parameters.AddWithValue("@param5", Ase);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<int> L = new List<int>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(Convert.ToInt32(Lectura_Hora["Car_Adm_Id"]));
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
    }
}
