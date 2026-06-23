using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain;
using Persistence.CXN.Interfaces;
using System.IO;
using Domain.CXN;
using System.Windows.Forms;

namespace Persistence.CXN.Metodos
{
    public class MRIPS : IRIPS
    {
        private static readonly ICompañia repoCia = new MCompañia();
        private static readonly IPacientes repoPacientes = new MPacientes();

        private int Aseguradora;
        private int Compañia;
        private DateTime Desde;
        private DateTime Hasta;
        private string Tipo_Documento;
        private string RegimenFacturado;

        private Dictionary<string, int> dicValoresAC;
        private CXN_CIA dataCompany;
        string SANO, SAPE, Valor_Cuota, Fac_Temp, Fin, CExt, Tipo_Doc, Reg_Cod, FACELECTRONICA;
        int Rip;

        void IRIPS.setDatos(int _aseguradora, int _compañia, DateTime _desde, DateTime _hasta, string _tipo_documento, string _regimen, string FacElectron)
        {
            this.Aseguradora = _aseguradora;
            this.Compañia = _compañia;
            this.Desde = _desde;
            this.Hasta = _hasta;
            this.Tipo_Documento = _tipo_documento;
            this.RegimenFacturado = _regimen;
            this.FACELECTRONICA = FacElectron;
            REvento();
        }
        void REvento()
        {
            dataCompany = new CXN_CIA();

            Reg_Cod = repoPacientes.Regimen(this.RegimenFacturado);

            dataCompany = repoCia.getPrestadorbyCode(Compañia);
            Rip = Convert.ToInt32(dataCompany.Com_RIP);

            int RipNuevo = Rip + 1;
            repoCia.ConsecutivoActualiza(Compañia, "RIP", RipNuevo);

            Ap();
            US();
            AC();
            AF();
            AT();
            CT();
        }
        string tipoDocR2000(string Tipo)
        {
            switch (Tipo)
            {
                case "MS":
                    return "RC";

                case "DE":
                    return "CE";
            }

            return Tipo;
        }
        private void Ap()
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

                    String Query = "SELECT F.Homologo, CO.Com_Cod_Prestador, P.Pac_TipoId, P.Pac_IdNum, C.Car_Fecha, " +
                                   "F.Fac_Num_Aut, C.Car_Cod, C.Car_DX1, C.Car_DX2, " +
                                   "C.Car_DX3, '1' AS RipFormaRe, C.Car_Val_Un, C.Car_Ambito, C.Car_Finalidad, " +
                                   "C.Car_Personal, CON.Con_CUP, H.Hor_RcCaja, H.Hor_Id, Hor_ConceptoRecaudo " +
                                   "FROM CXN_CARGOS C " +
                                   "INNER JOIN CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac " +
                                   "INNER JOIN CXN_CIA CO ON C.Car_Cia = CO.Com_Identificador " +
                                   "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                   "INNER JOIN CXN_CONVENIOS CON ON C.Car_Cod = CON.Con_Id_Serv " +
                                   "INNER JOIN CXN_HORARIO H ON C.Car_Adm_Id = H.Hor_Id " +
                                   "AND H.Hor_Pac_Ase = CON.Con_Aseguradora " +
                                   "AND H.Hor_Pac_Cup = C.Car_Cod " +
                                   "AND C.Car_Ase = CON.Con_Aseguradora " +
                                   "WHERE F.Fac_Fecha BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(getData["Format_Fecha"]) + "' " +
                                   "AND F.Fac_Ase = '" + Aseguradora + "' " +
                                   "AND F.Fac_Cia = '" + Compañia + "' " +
                                   "AND F.Fac_Estado = 'F' " +
                                   "AND F.Fac_Tipo_Doc = '" + Tipo_Documento + "' " +
                                   "AND C.Car_Tipo_Doc = '" + Tipo_Documento + "' " +
                                   "AND C.Car_Tipo = 'Nota' " +
                                   "AND F.Homologo IN (" + FACELECTRONICA + ") " + 
                                   "AND F.Fac_ConSub = '" + Reg_Cod + "'";
                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.CommandTimeout = 180;

                        using (SqlDataReader leer = (Commando.ExecuteReader()))
                        {
                            if (leer.HasRows)
                            {
                                string Texto = "";
                                FileStream QueryTxt = new FileStream("C:/Cxn/Rips/AP" + Rip + ".txt", FileMode.Append, FileAccess.Write);

                                while (leer.Read() == true)
                                {
                                    int ValPaga = Convert.ToInt32(leer["Car_Val_Un"]);

                                    /* if (leer["Hor_RcCaja"] != DBNull.Value && leer["Hor_RcCaja"].ToString() != "0" && leer["Hor_RcCaja"].ToString() != "" && leer["Hor_ConceptoRecaudo"].ToString() == "02") //02cuotas mod
                                     {
                                         ValPaga = Convert.ToInt32(leer["Car_Val_Un"]) - Convert.ToInt32(leer["Hor_RcCaja"]);
                                     }*/

                                    Tipo_Doc = repoPacientes.getTipoDoc(leer["Pac_TipoId"].ToString());
                                    Tipo_Doc = tipoDocR2000(Tipo_Doc);

                                    Texto = Texto + leer["Homologo"].ToString() + "," +
                                                    leer["Com_Cod_Prestador"].ToString() + "," +
                                                    Tipo_Doc + "," +
                                                    leer["Pac_IdNum"].ToString() + "," +
                                                    Convert.ToDateTime(leer["Car_Fecha"].ToString()).ToString("dd/MM/yyyy") + "," +
                                                    leer["Fac_Num_Aut"].ToString() + "," +
                                                    leer["Con_CUP"].ToString() + "," +
                                                    leer["Car_Ambito"].ToString() + "," +
                                                    leer["Car_Finalidad"].ToString() + "," +
                                                    leer["Car_Personal"].ToString() + "," +
                                                    leer["Car_DX1"].ToString() + "," +
                                                    leer["Car_DX2"].ToString() + "," +
                                                    leer["Car_DX3"].ToString() + "," +
                                                    leer["RipFormaRe"].ToString() + "," +
                                                    Convert.ToInt32(ValPaga).ToString() + "\n";
                                }

                                StreamWriter Escriba = new StreamWriter(QueryTxt);
                                Escriba.Write(Texto);
                                Escriba.Close();
                            }
                        }
                    }
                }
            }
             catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "AP: -->" + ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void US()
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

                    String Query = "SELECT DISTINCT P.Pac_IdNum, P.Pac_TipoId, A.Ase_Cod_Emp, P.Pac_PrimerA, P.Pac_PrimerN, P.Pac_Sexo, P.Pac_Dep_Cod, P.Pac_Mun_Cod, P.Pac_Zona, P.Pac_SegundoN, P.Pac_SegundoA, P.Pac_FechaNto, F.Fac_ConSub " +
                                   "FROM CXN_PACIENTES P " +
                                   "INNER JOIN CXN_CARGOS C ON P.Pac_Id = C.Car_Pac " +
                                   "INNER JOIN CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac " +
                                   "INNER JOIN CXN_ASEGURADORA A ON F.Fac_Ase = A.Ase_Identificador " +
                                   "WHERE F.Fac_Fecha BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(getData["Format_Fecha"]) + "' " +
                                   "AND F.Fac_Ase = '" + Aseguradora + "' " +
                                   "AND F.Fac_Cia = '" + Compañia + "' " +
                                   "AND F.Fac_Estado = 'F' " +
                                   "AND F.Fac_Tipo_Doc = '" + Tipo_Documento + "' " +
                                   "AND C.Car_Tipo_Doc = '" + Tipo_Documento + "' " +
                                   "AND C.Car_Tipo <> 'Cargo' " +
                                   "AND F.Homologo IN (" + FACELECTRONICA + ") " +
                                   "AND F.Fac_ConSub = '" + Reg_Cod + "'";
                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.CommandTimeout = 180;

                        using (SqlDataReader leer = (Commando.ExecuteReader()))
                        {
                            if (leer.HasRows)
                            {
                                string Texto = "";
                                FileStream QueryTxt = new FileStream("C:/Cxn/Rips/US" + Rip + ".txt", FileMode.Append, FileAccess.Write);

                                while (leer.Read() == true)
                                {
                                    Tipo_Doc = repoPacientes.getTipoDoc(leer["Pac_TipoId"].ToString());
                                    SANO = (string.IsNullOrWhiteSpace(leer["Pac_SegundoN"].ToString()) ? "" : leer["Pac_SegundoN"].ToString());
                                    SAPE = (string.IsNullOrWhiteSpace(leer["Pac_SegundoA"].ToString()) ? "" : leer["Pac_SegundoA"].ToString());
                                    Tipo_Doc = tipoDocR2000(Tipo_Doc);

                                    DateTime nacimiento = Convert.ToDateTime(leer["Pac_FechaNto"]);
                                    int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                                    string Edad = edad.ToString();

                                    int reImprime = 0;

                                    switch (Reg_Cod)
                                    {
                                        case "01":
                                            reImprime = 1;
                                            break;

                                        case "02":
                                            reImprime = 1;
                                            break;

                                        case "03":
                                            reImprime = 1;
                                            break;

                                        case "04":
                                            reImprime = 2;
                                            break;

                                        case "05":
                                            reImprime = 0;
                                            break;

                                        case "12":
                                            reImprime = 0;
                                            break;

                                        default:
                                            reImprime = 1;
                                            break;

                                    }

                                    Texto = Texto + Tipo_Doc + "," +
                                            leer["Pac_IdNum"].ToString() + "," +
                                            leer["Ase_Cod_Emp"].ToString() + "," +
                                            reImprime + "," +
                                            leer["Pac_PrimerA"].ToString() + "," +
                                            SAPE + "," +
                                            leer["Pac_PrimerN"].ToString() + "," +
                                            SANO + "," +
                                            Edad + "," +
                                            "1" + "," +
                                            leer["Pac_Sexo"].ToString() + "," +
                                            leer["Pac_Dep_Cod"].ToString() + "," +
                                            leer["Pac_Mun_Cod"].ToString() + "," +
                                            leer["Pac_Zona"].ToString() + "\n";
                                }

                                StreamWriter Escriba = new StreamWriter(QueryTxt);
                                Escriba.Write(Texto);
                                Escriba.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "US: -->" + ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void AC()
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

                    String Query = "SELECT DISTINCT F.Homologo, CO.Com_Cod_Prestador, P.Pac_TipoId, P.Pac_IdNum, C.Car_Fecha, " +
                                   "F.Fac_Num_Aut, C.Car_Cod, C.Car_CExterna, C.Car_DX1, C.Car_DX2, " +
                                   "C.Car_DX3, C.Car_Imp_Dx, C.Car_Val_Un, H.Hor_RcCaja, C.Car_Val_Un, C.Car_Finalidad_CO, CON.Con_CUP " +
                                   "FROM CXN_CARGOS C " +
                                   "INNER JOIN CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac " +
                                   "INNER JOIN CXN_CIA CO ON C.Car_Cia = CO.Com_Identificador " +
                                   "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                   "INNER JOIN CXN_HORARIO H ON C.Car_Adm_Id = H.Hor_Id " +
                                   "INNER JOIN CXN_CONVENIOS CON ON C.Car_Cod = CON.Con_Id_Serv " +
                                   "AND C.Car_Ase = CON.Con_Aseguradora " +
                                   "WHERE F.Fac_Fecha BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(getData["Format_Fecha"]) + "' " +
                                   "AND F.Fac_Ase = '" + Aseguradora + "' " +
                                   "AND F.Fac_Cia = '" + Compañia + "' " +
                                   "AND F.Fac_Estado = 'F' " +
                                   "AND F.Fac_Tipo_Doc = '" + Tipo_Documento + "' " +
                                   "AND C.Car_Tipo_Doc = '" + Tipo_Documento + "' " +
                                   "AND C.Car_Tipo = 'Historia' " +
                                   "AND F.Fac_ConSub = '" + Reg_Cod + "' " +
                                   "AND F.Homologo IN (" + FACELECTRONICA + ") " +
                                   "GROUP BY F.Homologo, CO.Com_Cod_Prestador, P.Pac_TipoId, P.Pac_IdNum, C.Car_Fecha, F.Fac_Num_Aut, C.Car_Cod, C.Car_CExterna, " +
                                   "C.Car_DX1, C.Car_DX2, C.Car_DX3, C.Car_Imp_Dx, C.Car_Val_Un, H.Hor_RcCaja, C.Car_Val_Un, C.Car_Finalidad_CO, CON.Con_CUP";
                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.CommandTimeout = 180;

                        using (SqlDataReader leer = (Commando.ExecuteReader()))
                        {
                            if (leer.HasRows)
                            {
                                string Texto = "";
                                FileStream QueryTxt = new FileStream("C:/Cxn/Rips/AC" + Rip + ".txt", FileMode.Append, FileAccess.Write);
                                dicValoresAC = new Dictionary<string, int>();

                                while (leer.Read() == true)
                                {
                                    Valor_Cuota = (string.IsNullOrWhiteSpace(leer["Hor_RcCaja"].ToString()) == true ? "0" : leer["Hor_RcCaja"].ToString());
                                    int V_Final = Convert.ToInt32(leer["Car_Val_Un"]) - Convert.ToInt32(Valor_Cuota);

                                    if (dicValoresAC.ContainsKey(leer["Homologo"].ToString()))
                                    {
                                        var getDatoDicAc = dicValoresAC[leer["Homologo"].ToString()];
                                        dicValoresAC[leer["Homologo"].ToString()] = getDatoDicAc + Convert.ToInt32(Valor_Cuota);
                                    }
                                    else
                                    {
                                        dicValoresAC.Add(leer["Homologo"].ToString(), Convert.ToInt32(Valor_Cuota));
                                    }

                                    Tipo_Doc = repoPacientes.getTipoDoc(leer["Pac_TipoId"].ToString());
                                    Tipo_Doc = tipoDocR2000(Tipo_Doc);

                                    switch (leer["Car_Finalidad_CO"].ToString())
                                    {
                                        case "1":
                                            Fin = "01";
                                            break;
                                        case "2":
                                            Fin = "02";
                                            break;
                                        case "3":
                                            Fin = "03";
                                            break;
                                        case "4":
                                            Fin = "04";
                                            break;
                                        case "5":
                                            Fin = "05";
                                            break;
                                        case "6":
                                            Fin = "06";
                                            break;
                                        case "7":
                                            Fin = "07";
                                            break;
                                        case "8":
                                            Fin = "08";
                                            break;
                                        case "9":
                                            Fin = "09";
                                            break;
                                        case "10":
                                            Fin = "10";
                                            break;
                                        default:
                                            Fin = "10";
                                            break;
                                    }

                                    switch (leer["Car_CExterna"].ToString())
                                    {
                                        case "1":
                                            CExt = "01";
                                            break;
                                        case "2":
                                            CExt = "02";
                                            break;
                                        case "3":
                                            CExt = "03";
                                            break;
                                        case "4":
                                            CExt = "04";
                                            break;
                                        case "5":
                                            CExt = "05";
                                            break;
                                        case "6":
                                            CExt = "06";
                                            break;
                                        case "7":
                                            CExt = "07";
                                            break;
                                        case "8":
                                            CExt = "08";
                                            break;
                                        case "9":
                                            CExt = "09";
                                            break;
                                        case "10":
                                            CExt = "10";
                                            break;
                                        case "11":
                                            CExt = "11";
                                            break;
                                        case "12":
                                            CExt = "12";
                                            break;
                                        case "13":
                                            CExt = "13";
                                            break;
                                        case "14":
                                            CExt = "14";
                                            break;
                                        case "15":
                                            CExt = "15";
                                            break;
                                        default:
                                            CExt = "13";
                                            break;
                                    }

                                    CExt = "13";

                                    Texto = Texto + leer["Homologo"].ToString() + "," +
                                                    leer["Com_Cod_Prestador"].ToString() + "," +
                                                    Tipo_Doc + "," +
                                                    leer["Pac_IdNum"].ToString() + "," +
                                                    Convert.ToDateTime(leer["Car_Fecha"].ToString()).ToString("dd/MM/yyyy") + "," +
                                                    leer["Fac_Num_Aut"].ToString() + "," +
                                                    leer["Con_CUP"].ToString() + "," +
                                                    Fin + "," +
                                                    CExt + "," +
                                                    leer["Car_Dx1"].ToString() + "," +
                                                    leer["Car_Dx2"].ToString() + "," +
                                                    leer["Car_Dx3"].ToString() + "," +
                                                    "" + "," +
                                                    leer["Car_Imp_Dx"].ToString() + "," +
                                                    Convert.ToInt32(leer["Car_Val_Un"]).ToString() + "," +
                                                    Valor_Cuota + "," +
                                                    Convert.ToInt32(V_Final) + "\n";
                                }

                                StreamWriter Escriba = new StreamWriter(QueryTxt);
                                Escriba.Write(Texto);
                                Escriba.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "AC: -->" + ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void AF()
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

                    int V_Fac_Final = 0;
                    int V_RC_Final = 0;

                    FileStream Query = new FileStream("C:/Cxn/Rips/AF" + Rip + ".txt", FileMode.Append, FileAccess.Write);
                    StreamWriter Escriba = new StreamWriter(Query);

                    String Query1 = "SELECT DISTINCT Com_Cod_Prestador, Com_Nombre, Com_Tipo_Doc, Com_Identificacion, Homologo, Fac_Fecha, Fac_Num_Fac, " +
                                    "Fac_Fecha_Des, Fac_Fecha_Has, Ase_Cod_Emp, '' as RipAseg, '' as RipVacio1, '' as RipVacio2, '' as RipVacio3, '' as RipVacio4, " +
                                    "'' as RipVacio5, '' as RipVacio6, Ase_Descripcion, Fac_Descuento, VrCompartido, Copago, Anticipo " +
                                    "FROM CXN_FACTURA " +
                                    "INNER JOIN CXN_CARGOS ON CXN_FACTURA.Fac_Num_Fac = CXN_CARGOS.Car_Factura " +
                                    "INNER JOIN CXN_CIA ON CXN_FACTURA.Fac_Cia = CXN_CIA.Com_Identificador " +
                                    "INNER JOIN CXN_ASEGURADORA ON CXN_FACTURA.Fac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                    "WHERE Fac_Fecha BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(getData["Format_Fecha"]) + "' " +
                                    "AND Fac_Ase = '" + Aseguradora + "' " +
                                    "AND Fac_Cia = '" + Compañia + "' " +
                                    "AND Car_Estado = 'F' " +
                                    "AND Fac_Tipo_Doc = '" + Tipo_Documento + "' " +
                                    "AND Car_Tipo_Doc = '" + Tipo_Documento + "' " +
                                    "AND Fac_Estado = 'F' " +
                                    "AND Homologo IN (" + FACELECTRONICA + ") " +
                                    "AND Fac_ConSub = '" + Reg_Cod + "'";
                    using (SqlCommand Commando1 = new SqlCommand(Query1, con))
                    {
                        using (SqlDataReader Reader1 = (Commando1.ExecuteReader()))
                        {
                            if (Reader1.HasRows)
                            {
                                while (Reader1.Read() == true)
                                {
                                    Fac_Temp = Reader1["Fac_Num_Fac"].ToString();

                                    String Query2 = "SELECT sum(car_val_tot) as ValorTot " +
                                                    "FROM CXN_CARGOS " +
                                                    "WHERE Car_Factura = '" + Fac_Temp + "'";
                                    using (SqlCommand Commando2 = new SqlCommand(Query2, con))
                                    {
                                        using (SqlDataReader Reader2 = (Commando2.ExecuteReader()))
                                        {
                                            if (Reader2.Read() == true)
                                            {
                                                V_Fac_Final = Convert.ToInt32(Reader2["ValorTot"]);
                                            }
                                        }
                                    }

                                    string Compartido = "";  // es el mismo copago, en este caso comision
                                    string Anticipo = ""; //en este caso descuentos, Fac_descuentos es cuotras moderadaoras y se registra en copagos
                                    string Copago = ""; //en este caso descuentos, Fac_descuentos es cuotras moderadaoras y se registra en copagos
                                    string Descuento = ""; //descuento cuotas moderadoras registradas en gfacturacion, Consultas

                                    if (Reader1["VrCompartido"] != DBNull.Value && Reader1["VrCompartido"].ToString() != "0" && Reader1["VrCompartido"].ToString() != "")
                                    {
                                        // V_Fac_Final = V_Fac_Final - Convert.ToInt32(Reader1["VrCompartido"]);
                                        Compartido = Convert.ToInt32(Reader1["VrCompartido"]).ToString();
                                    }

                                    if (Reader1["Copago"] != DBNull.Value && Reader1["Copago"].ToString() != "0" && Reader1["Copago"].ToString() != "")
                                    {
                                        // V_Fac_Final = V_Fac_Final - Convert.ToInt32(Reader1["Copago"]);
                                        Copago = Convert.ToInt32(Reader1["Copago"]).ToString();
                                    }

                                    if (Reader1["Anticipo"] != DBNull.Value && Reader1["Anticipo"].ToString() != "0" && Reader1["Anticipo"].ToString() != "")
                                    {
                                        // V_Fac_Final = V_Fac_Final - Convert.ToInt32(Reader1["Anticipo"]);
                                        Anticipo = Convert.ToInt32(Reader1["Anticipo"]).ToString();
                                    }

                                    if (Reader1["Fac_Descuento"] != DBNull.Value && Reader1["Fac_Descuento"].ToString() != "0" && Reader1["Fac_Descuento"].ToString() != "")
                                    {
                                        // V_Fac_Final = V_Fac_Final - Convert.ToInt32(Reader1["Fac_Descuento"]);
                                        Descuento = Convert.ToInt32(Reader1["Fac_Descuento"]).ToString();
                                    }

                                    if (this.dicValoresAC != null)
                                    {
                                        if (dicValoresAC.ContainsKey(Reader1["Homologo"].ToString()))
                                        {
                                            V_Fac_Final = V_Fac_Final - dicValoresAC[Reader1["Homologo"].ToString()];
                                        }
                                    }

                                    Escriba.Write(Reader1["Com_Cod_Prestador"].ToString() + ",");
                                    Escriba.Write(Reader1["Com_Nombre"].ToString() + ",");
                                    Escriba.Write(Reader1["Com_Tipo_Doc"].ToString().TrimEnd() + ",");
                                    Escriba.Write(Reader1["Com_Identificacion"].ToString() + ",");
                                    Escriba.Write(Reader1["Homologo"].ToString() + ",");
                                    Escriba.Write(Convert.ToDateTime(Reader1["Fac_Fecha"].ToString()).ToString("dd/MM/yyyy") + ",");
                                    Escriba.Write(Convert.ToDateTime(Reader1["Fac_Fecha_Des"].ToString()).ToString("dd/MM/yyyy") + ",");
                                    Escriba.Write(Convert.ToDateTime(Reader1["Fac_Fecha_Has"].ToString()).ToString("dd/MM/yyyy") + ",");
                                    Escriba.Write(Reader1["Ase_Cod_Emp"].ToString() + ",");
                                    Escriba.Write(Reader1["Ase_Descripcion"].ToString() + ",");
                                    Escriba.Write(Reader1["RipVacio1"].ToString() + ","); //Numero de Contrato vacio
                                    Escriba.Write(Reader1["RipVacio2"].ToString() + ","); //Plan de Beneficios
                                    Escriba.Write(Reader1["RipVacio3"].ToString() + ","); //Numero de Poliza SOAT
                                    //Escriba.Write(V_RC_Final <= 0 ? "" : Convert.ToInt32(V_RC_Final) + ",");
                                    Escriba.Write(Reader1["Fac_Descuento"].ToString() == "0" ? "," : Convert.ToInt32(Reader1["Fac_Descuento"]) + ","); //Vr Copagos
                                    Escriba.Write((Compartido == "0" ? "" : Compartido) + ","); //Vr comision
                                    Escriba.Write((Anticipo == "0" ? "" : Anticipo) + ","); //Vr Descuentos en este caso
                                    Escriba.Write(V_Fac_Final); //Neto factura
                                    Escriba.WriteLine();
                                    Escriba.Flush();
                                }
                            }

                            Escriba.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "AF: -->" + ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void AT()
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

                    String Query = "SELECT F.Homologo, C.Com_Cod_Prestador, P.Pac_TipoId, P.Pac_IdNum, '1' AS MATERIAL, " +
                                   " Ca.Car_Cod, Ca.Car_Item, Ca.Car_Cant, Ca.Car_Val_Un, Ca.Car_Val_Tot " +
                                   " FROM CXN_CARGOS Ca " +
                                   " INNER JOIN CXN_FACTURA F ON Ca.Car_Factura = F.Fac_Num_Fac " +
                                   " INNER JOIN CXN_CIA C ON Ca.Car_Cia = C.Com_Identificador " +
                                   " INNER JOIN CXN_PACIENTES P ON Ca.Car_Pac = P.Pac_Id " +
                                   " WHERE F.Fac_Fecha BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(getData["Format_Fecha"]) + "' " +
                                   " AND F.Fac_Ase = '" + Aseguradora + "' " +
                                   " AND F.Fac_Cia = '" + Compañia + "' " +
                                   " AND F.Fac_Estado = 'F' " +
                                   " AND F.Fac_Tipo_Doc = '" + Tipo_Documento + "' " +
                                   " AND Ca.Car_Tipo_Doc = '" + Tipo_Documento + "' " +
                                   " AND Ca.Car_Tipo = 'Cargo' " +
                                   " AND F.Homologo IN (" + FACELECTRONICA + ") " +
                                   " AND F.Fac_ConSub = '" + Reg_Cod + "'";
                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.CommandTimeout = 180;

                        using (SqlDataReader leer = (Commando.ExecuteReader()))
                        {
                            if (leer.HasRows)
                            {
                                string Texto = "";
                                FileStream QueryTxt = new FileStream("C:/Cxn/Rips/AT" + Rip + ".txt", FileMode.Append, FileAccess.Write);

                                while (leer.Read() == true)
                                {
                                    Tipo_Doc = repoPacientes.getTipoDoc(leer["Pac_TipoId"].ToString());
                                    Tipo_Doc = tipoDocR2000(Tipo_Doc);

                                    Texto = Texto + leer["Homologo"].ToString() + "," +
                                        leer["Com_Cod_Prestador"].ToString() + "," +
                                        Tipo_Doc + "," +
                                        leer["Pac_IdNum"].ToString() + "," +
                                        "" + "," +
                                        leer["MATERIAL"].ToString() + "," +
                                        leer["Car_Cod"].ToString() + "," +
                                        leer["Car_Item"].ToString() + "," +
                                        leer["Car_Cant"].ToString() + "," +
                                        Convert.ToInt32(leer["Car_Val_Un"]).ToString() + "," +
                                        Convert.ToInt32(leer["Car_Val_Tot"]).ToString() + "\n";
                                }

                                StreamWriter Escriba = new StreamWriter(QueryTxt);
                                Escriba.Write(Texto);
                                Escriba.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "AT: -->" + ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void CT()
        {
            try
            {
                string Texto = "";
                FileStream QueryTxt = new FileStream("C:/Cxn/Rips/CT" + Rip + ".txt", FileMode.Append, FileAccess.Write);
                DateTime Hoy = DateTime.Now.Date;

                string rutaArchivo = @"C:\CXN\RIPS\AP" + Rip + ".txt";

                if (File.Exists(rutaArchivo))
                {
                    //AP
                    string fileAP = new StreamReader("C:\\CXN\\Rips\\AP" + Rip + ".txt").ReadToEnd();
                    string[] linesAP = fileAP.Split('\n');
                    int ContAP = linesAP.GetLength(0) - 1;

                    Texto = Texto + this.dataCompany.Com_Cod_Prestador + "," +
                        Convert.ToDateTime(Hoy.Date).ToString("dd/MM/yyyy") + "," +
                        "AP" + Rip + "," +
                        ContAP + "\n";
                }

                rutaArchivo = @"C:\CXN\RIPS\US" + Rip + ".txt";

                if (File.Exists(rutaArchivo))
                {
                    //US
                    string fileUS = new StreamReader("C:\\CXN\\Rips\\US" + Rip + ".txt").ReadToEnd();
                    string[] linesUS = fileUS.Split('\n');
                    int ContUS = linesUS.GetLength(0) - 1;

                    Texto = Texto + this.dataCompany.Com_Cod_Prestador + "," +
                        Convert.ToDateTime(Hoy.Date).ToString("dd/MM/yyyy") + "," +
                        "US" + Rip + "," +
                        ContUS + "\n";
                }

                rutaArchivo = @"C:\CXN\RIPS\AC" + Rip + ".txt";

                if (File.Exists(rutaArchivo))
                {
                    //AC
                    string fileAC = new StreamReader("C:\\CXN\\Rips\\AC" + Rip + ".txt").ReadToEnd();
                    string[] linesAC = fileAC.Split('\n');
                    int ContAC = linesAC.GetLength(0) - 1;

                    Texto = Texto + this.dataCompany.Com_Cod_Prestador + "," +
                        Convert.ToDateTime(Hoy.Date).ToString("dd/MM/yyyy") + "," +
                        "AC" + Rip + "," +
                        ContAC + "\n";
                }

                rutaArchivo = @"C:\CXN\RIPS\AF" + Rip + ".txt";

                if (File.Exists(rutaArchivo))
                {
                    //AF
                    string fileAF = new StreamReader("C:\\CXN\\Rips\\AF" + Rip + ".txt").ReadToEnd();
                    string[] linesAF = fileAF.Split('\n');
                    int ContAF = linesAF.GetLength(0) - 1;

                    Texto = Texto + this.dataCompany.Com_Cod_Prestador + "," +
                        Convert.ToDateTime(Hoy.Date).ToString("dd/MM/yyyy") + "," +
                        "AF" + Rip + "," +
                        ContAF + "\n";
                }

                rutaArchivo = @"C:\CXN\RIPS\AT" + Rip + ".txt";

                if (File.Exists(rutaArchivo))
                {
                    //AT
                    string fileAT = new StreamReader("C:\\CXN\\Rips\\AT" + Rip + ".txt").ReadToEnd();
                    string[] linesAT = fileAT.Split('\n');
                    int ContAT = linesAT.GetLength(0) - 1;

                    Texto = Texto + this.dataCompany.Com_Cod_Prestador + "," +
                        Convert.ToDateTime(Hoy.Date).ToString("dd/MM/yyyy") + "," +
                        "AT" + Rip + "," +
                        ContAT + "\n";
                }

                StreamWriter Escriba = new StreamWriter(QueryTxt);
                Escriba.Write(Texto);
                Escriba.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "CT: -->" + ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void IRIPS.RepararRIPSCupCarVsHor(RIPS_Class R)
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

                    string selectQuery = "SELECT Fac_Num_Fac " +
                                         "FROM CXN_FACTURA " +
                                         "WHERE Fac_Fecha BETWEEN @Desde AND @Hasta " +
                                         "AND Fac_Ase = @Aseguradora " +
                                         "AND Fac_Cia = @Compañia " +
                                         "AND Fac_Estado = 'F'";

                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, con))
                    {
                        selectCommand.Parameters.AddWithValue("@Aseguradora", R.aseRIPS);
                        selectCommand.Parameters.AddWithValue("@Compañia", R.ciaRIPS);
                        selectCommand.Parameters.Add(new SqlParameter("@Desde", SqlDbType.DateTime)).Value = Convert.ToDateTime(R.desdeRIPS.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        selectCommand.Parameters.Add(new SqlParameter("@Hasta", SqlDbType.DateTime)).Value = Convert.ToDateTime(R.hastaRIPS.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]);

                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string facturaZamenis = reader["Fac_Num_Fac"].ToString();

                                SqlCommand Busqueda = new SqlCommand(@"UPDATE T2 " +
                                                                      "SET T2.Hor_Pac_Cup = T1.Car_Cod " +
                                                                      "FROM CXN_HORARIO T2 " +
                                                                      "JOIN CXN_CARGOS T1 ON T2.Hor_Id = T1.Car_Adm_Id " +
                                                                      "WHERE T1.Car_Factura = @facturaZamenis " +
                                                                      "AND T1.Car_Tipo = 'Nota'", con);

                                Busqueda.Parameters.AddWithValue("@facturaZamenis", facturaZamenis);
                                Busqueda.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Hecho", "Hecho", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        int IRIPS.TipoRipCargo(string Valor, string Tipo)
        {
            if (Tipo == "CODEEGRESO")
            {
                switch (Valor)
                {
                    case "PACIENTE CON DESTINO A SU DOMICILIO":
                        return 1;

                    case "PACIENTE MUERTO":
                        return 2;

                    case "PACIENTE DERIVADO A OTRO SERVICIO":
                        return 3;

                    case "REFERIDO A OTRA INSTITUCION":
                        return 4;

                    case "CONTRAREFERIDO A OTRA INSTITUCION":
                        return 5;

                    case "DERIVADO O REFERIDO A HOSPITALIZACION DOMICILIRIA":
                        return 6;

                    case "DERIVADO A SERVICIO SOCIAL":
                        return 7;

                    case "PACIENTE CONTINUA EN EL SERVICIO (CORTE FACTURACION)":
                        return 8;

                    default:
                        return 1;
                }
            }
            if (Tipo == "Ambito") 
            {
                switch (Valor)
                {
                    case "Ambulatorio":
                        return 1;

                    case "Hospitalario":
                        return 2;

                    case "Urgencias":
                        return 3;

                    default:
                        return 1;
                }
            }
            if (Tipo == "Personal")
            {
                switch (Valor)
                {
                    case "Medico General":
                        return 2;

                    case "Auxiliar de Enfermeria":
                        return 4;

                    case "Medico Especialista":
                        return 1;

                    case "Enfermero":
                        return 3;

                    default:
                        return 5;
                }
            }
            if (Tipo == "CExterna")
            {
                switch (Valor)
                {
                    case "Enfermedad general":
                        return 13;

                    case "Accidente de trabajo":
                        return 1;

                    case "Accidente de tránsito":
                        return 2;

                    case "Accidente rábico":
                        return 3;

                    case "Accidente ofídico":
                        return 4;

                    case "Otro tipo de accidente":
                        return 5;

                    case "Evento catastrófico":
                        return 6;

                    case "Lesión por agresión":
                        return 7;

                    case "Lesión auto infligida":
                        return 8;

                    case "Sospecha de maltrato físico":
                        return 9;

                    case "Sospecha de abuso sexual":
                        return 10;

                    case "Sospecha de violencia sexual":
                        return 11;

                    case "Sospecha de maltrato emocional":
                        return 12;

                    case "Enfermedad laboral":
                        return 14;

                    default:
                        return 15;
                }
            }
            if (Tipo == "Finalidad")
            {
                switch (Valor)
                {
                    case "Diagnostico":
                        return 1;

                    case "Terapeutico":
                        return 2;

                    case "Proteccion Esspecifica":
                        return 3;

                    case "Deteccion Temprana Enfermedad General":
                        return 4;

                    case "Deteccion Temprana Enfermedad Laboral":
                        return 5;

                    default:
                        return 1;
                }
            }
            if (Tipo == "Motivo")
            {
                switch (Valor)
                {
                    case "No aplica":
                        return 10;

                    case "Atención del parto (puerperio)":
                        return 1;

                    case "Atención del recién nacido":
                        return 2;

                    case "Atención en planificación familiar":
                        return 3;

                    case "Detección de alteraciones de crecimiento y desarrollo del menor de diez años":
                        return 4;

                    case "Detección de alteración del desarrollo joven":
                        return 5;

                    case "Detección de alteraciones del embarazo":
                        return 6;

                    case "Detección de alteraciones del adulto":
                        return 7;

                    case "Detección de alteraciones de agudeza visual":
                        return 8;

                    case "Detección de enfermedad profesional":
                        return 9;

                    default:
                        return 10;
                }
            }
            if (Tipo == "IMPDX")
            {
                switch (Valor)
                {
                    case "CONFIRMADO NUEVO":
                        return 2;

                    case "CONFIRMADO ANTIGUO":
                        return 3;

                    case "IMPRESION DIAGNOSTICA":
                        return 1;

                    default:
                        return 1;
                }
            }

            return 0;
        }
        string IRIPS.TipoRipCargo(int Valor, string Tipo)
        {
            if (Tipo == "Ambito")
            {
                switch (Valor)
                {
                    case 1:
                        return "Ambulatorio";

                    case 2:
                        return "Hospitalario";

                    case 3:
                        return "Urgencias";

                    default:
                        return "Ambulatorio";
                }
            }
            if (Tipo == "Personal")
            {
                switch (Valor)
                {
                    case 2:
                        return "Medico General";

                    case 4:
                        return "Auxiliar de Enfermeria";

                    case 1:
                        return "Medico Especialista";

                    case 3:
                        return "Enfermero";

                    default:
                        return "Medico General";
                }
            }
            if (Tipo == "CExterna")
            {
                switch (Valor)
                {
                    case 13:
                        return "Enfermedad general";

                    case 1:
                        return "Accidente de trabajo";

                    case 2:
                        return "Accidente de tránsito";

                    case 3:
                        return "Accidente rábico";

                    case 4:
                        return "Accidente ofídico";

                    case 5:
                        return "Otro tipo de accidente";

                    case 6:
                        return "Evento catastrófico";

                    case 7:
                        return "Lesión por agresión";

                    case 8:
                        return "Lesión auto infligida";

                    case 9:
                        return "Sospecha de maltrato físico";

                    case 10:
                        return "Sospecha de abuso sexual";

                    case 11:
                        return "Sospecha de violencia sexual";

                    case 12:
                        return "Sospecha de maltrato emocional";

                    case 14:
                        return "Enfermedad laboral";

                    default:
                        return "Enfermedad general";
                }
            }
            if (Tipo == "Finalidad")
            {
                switch (Valor)
                {
                    case 1:
                        return "Diagnostico";

                    case 2:
                        return "Terapeutico";

                    case 3:
                        return "Proteccion Esspecifica";

                    case 4:
                        return "Deteccion Temprana Enfermedad General";

                    case 5:
                        return "Deteccion Temprana Enfermedad Laboral";

                    default:
                        return "Diagnostico";
                }
            }
            if (Tipo == "Motivo")
            {
                switch (Valor)
                {
                    case 10:
                        return "No aplica";

                    case 1:
                        return "Atención del parto (puerperio)";

                    case 2:
                        return "Atención del recién nacido";

                    case 3:
                        return "Atención en planificación familiar";

                    case 4:
                        return "Detección de alteraciones de crecimiento y desarrollo del menor de diez años";

                    case 5:
                        return "Detección de alteración del desarrollo joven";

                    case 6:
                        return "Detección de alteraciones del embarazo";

                    case 7:
                        return "Detección de alteraciones del adulto";

                    case 8:
                        return "Detección de alteraciones de agudeza visual";

                    case 9:
                        return "Detección de enfermedad profesional";

                    default:
                        return "No aplica";
                }
            }
            if (Tipo == "IMPDX")
            {
                switch (Valor)
                {
                    case 2:
                        return "CONFIRMADO NUEVO";

                    case 3:
                        return "CONFIRMADO ANTIGUO";

                    case 1:
                        return "IMPRESION DIAGNOSTICA";

                    default:
                        return "IMPRESION DIAGNOSTICA";
                }
            }

            return "";
        }
        void IRIPS.updateCargo(int ambito, int personal, int cexterna, int finalidad, int motivo, int impdx, int adm, int CodeEgreso)
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

                    string Busqueda = "UPDATE CXN_CARGOS " +
                                          "SET Car_Ambito = '" + ambito + "', " +
                                          "Car_Finalidad = '" + finalidad + "', " +
                                          "Car_Personal = '" + personal + "', " +
                                          "Car_CExterna = '" + cexterna + "', " +
                                          "Car_Imp_Dx = '" + impdx + "', " +
                                          "Car_Finalidad_CO = '" + motivo + "', " +
                                          "Car_CodeEgreso = '" + CodeEgreso + "' " +
                                          "WHERE Car_Adm_Id = '" + adm + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void IRIPS.updateRegimenMP(DateTime Desde, DateTime Hasta, int Cia, int Ase)
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
                                      "SET Fac_ConSub = '05' " + // No  Aplica  Regimen segun SISPRO
                                      "WHERE Fac_Fecha BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                      "AND Fac_Ase = '" + Ase + "' " +
                                      "AND Fac_Cia = '" + Cia + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
