using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.Fibromialgia;
using Persistence.Fibromialgia.Interfaces;

namespace Persistence.Fibromialgia.Metodos
{
    public class MExport : IExport
    {
        string getResult(int Val)
        {
            string res = "";

            switch (Val)
            {
                case 0:
                    res = "Siempre";
                    break;

                case 1:
                    res = "Habitualmente";
                    break;

                case 2:
                    res = "Ocacionalmente";
                    break;

                case 3:
                    res = "Nunca";
                    break;
            }

            return res;
        }

        string getResult2(int Val)
        {
            string res = "";

            switch (Val)
            {
                case 1:
                    res = "Excelente";
                    break;

                case 2:
                    res = "Muy Buena";
                    break;

                case 3:
                    res = "Buena";
                    break;

                case 4:
                    res = "Regular";
                    break;

                case 5:
                    res = "Mala";
                    break;
            }

            return res;
        }

        string getResult3(int Val)
        {
            string res = "";

            switch (Val)
            {
                case 1:
                    res = "Si, me limita mucho";
                    break;

                case 2:
                    res = "Si, me limita un poco";
                    break;

                case 3:
                    res = "No, no me limita nada";
                    break;
            }

            return res;
        }

        string getResult4(int Val)
        {
            string res = "";

            switch (Val)
            {
                case 1:
                    res = "SI";
                    break;

                case 2:
                    res = "NO";
                    break;
            }

            return res;
        }

        string getResult5(int Val)
        {
            string res = "";

            switch (Val)
            {
                case 1:
                    res = "Nada";
                    break;

                case 2:
                    res = "Un poco";
                    break;

                case 3:
                    res = "Regular";
                    break;

                case 4:
                    res = "Bastante";
                    break;

                case 5:
                    res = "Mucho";
                    break;
            }

            return res;
        }

        string getResult6(int Val)
        {
            string res = "";

            switch (Val)
            {
                case 1:
                    res = "Siempre";
                    break;

                case 2:
                    res = "Casi siempre";
                    break;

                case 3:
                    res = "Muchas veces";
                    break;

                case 4:
                    res = "Algunas veces";
                    break;

                case 5:
                    res = "Solo una vez";
                    break;

                case 6:
                    res = "Nunca";
                    break;
            }

            return res;
        }

        string getResult7(int Val)
        {
            string res = "";

            switch (Val)
            {
                case 1:
                    res = "Siempre";
                    break;

                case 2:
                    res = "Casi siempre";
                    break;

                case 3:
                    res = "Algunas veces";
                    break;

                case 4:
                    res = "Solo alguna vez";
                    break;

                case 5:
                    res = "Nunca";
                    break;
            }

            return res;
        }

        string getResult8(int Val)
        {
            string res = "";

            switch (Val)
            {
                case 1:
                    res = "Totalmente cierta";
                    break;

                case 2:
                    res = "Bastante cierta";
                    break;

                case 3:
                    res = "No lo se";
                    break;

                case 4:
                    res = "Bastante falsa";
                    break;

                case 5:
                    res = "Totalmente falsa";
                    break;
            }

            return res;
        }

        string getResultC2(int Val)
        {
            string res = "";

            switch (Val)
            {
                case 0:
                    res = "No ha sido un problema";
                    break;

                case 1:
                    res = "Leve, ocasional";
                    break;

                case 2:
                    res = "Moderada, presente casi siempre";
                    break;

                case 3:
                    res = "Grave, persistente, he tenido grandes problemas";
                    break;
            }

            return res;
        }

        List<FIB_ENCUESTA3> IExport.ExportarEncuesta3(int Id)
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
                    String Cargar_Hora2 = "SELECT *  " +
                                          "FROM FIB_ENCUESTA3 " +
                                          "INNER JOIN CXN_PACIENTES ON FIB_ENCUESTA3.IdPaciente = CXN_PACIENTES.Pac_Id " +
                                          "WHERE FIB_ENCUESTA3.Id = '" + Id + "'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        List<FIB_ENCUESTA3> E3 = new List<FIB_ENCUESTA3>();

                        while (Lectura_Hora2.Read() == true)
                        {
                            E3.Add(new FIB_ENCUESTA3
                            {
                                P57 = Lectura_Hora2["Pac_PrimerA"].ToString() + " " +
                                                             Lectura_Hora2["Pac_SegundoA"].ToString() + " " +
                                                             Lectura_Hora2["Pac_PrimerN"].ToString() + " " +
                                                             Lectura_Hora2["Pac_SegundoN"].ToString(),
                                FechaEncuesta = Convert.ToDateTime(Lectura_Hora2["FechaEncuesta"]),
                                P1 = Lectura_Hora2["Pregunta1"].ToString(),
                                P2 = Lectura_Hora2["Pregunta2"].ToString(),
                                P3 = Lectura_Hora2["Pregunta3"].ToString(),
                                P4 = Lectura_Hora2["Pregunta4"].ToString(),
                                P5 = Lectura_Hora2["Pregunta5"].ToString(),
                                P6 = Lectura_Hora2["Pregunta6"].ToString(),
                                P7 = Lectura_Hora2["Pregunta7"].ToString(),
                                P8 = Lectura_Hora2["Pregunta8"].ToString(),
                                P9 = Lectura_Hora2["Pregunta9"].ToString(),
                                P10 = Lectura_Hora2["Pregunta10"].ToString()
                            });
                        }

                        return E3;
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

        List<FIB_ENCUESTA2> IExport.ExportarEncuesta2(int Id)
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
                    String Cargar_Hora2 = "SELECT *  " +
                                          "FROM FIB_ENCUESTA2 " +
                                          "INNER JOIN CXN_PACIENTES ON FIB_ENCUESTA2.IdPaciente = CXN_PACIENTES.Pac_Id " +
                                          "WHERE FIB_ENCUESTA2.Id = '" + Id + "'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        List<FIB_ENCUESTA2> E2 = new List<FIB_ENCUESTA2>();

                        while (Lectura_Hora2.Read() == true)
                        {
                            string res = "";
                            string p1 = ""; string p2 = ""; string p3 = ""; string p4 = ""; string p5 = ""; string p6 = ""; string p7 = ""; string p8 = ""; string p9 = ""; string p10 = "";
                            string p11 = ""; string p12 = ""; string p13 = ""; string p14 = ""; string p15 = ""; string p16 = ""; string p17 = ""; string p18 = ""; string p19 = "";

                            if (Lectura_Hora2["Pregunta1"].ToString() != "")
                            {
                                p1 = Lectura_Hora2["Pregunta1"].ToString() + " - 1";
                                res = res + p1;
                            }

                            if (Lectura_Hora2["Pregunta2"].ToString() != "")
                            {
                                p2 = Lectura_Hora2["Pregunta2"].ToString() + " - 2";
                                res = res + "\n\r" + p2;
                            }

                            if (Lectura_Hora2["Pregunta3"].ToString() != "")
                            {
                                p3 = Lectura_Hora2["Pregunta3"].ToString() + " - 3";
                                res = res + "\n\r" + p3;
                            }

                            if (Lectura_Hora2["Pregunta4"].ToString() != "")
                            {
                                p4 = Lectura_Hora2["Pregunta4"].ToString() + " - 4";
                                res = res + "\n\r" + p4;
                            }

                            if (Lectura_Hora2["Pregunta5"].ToString() != "")
                            {
                                p5 = Lectura_Hora2["Pregunta5"].ToString() + " - 5";
                                res = res + "\n\r" + p5;
                            }

                            if (Lectura_Hora2["Pregunta6"].ToString() != "")
                            {
                                p6 = Lectura_Hora2["Pregunta6"].ToString() + " - 6";
                                res = res + "\n\r" + p6;
                            }

                            if (Lectura_Hora2["Pregunta7"].ToString() != "")
                            {
                                p7 = Lectura_Hora2["Pregunta7"].ToString() + " - 7";
                                res = res + "\n\r" + p7;
                            }

                            if (Lectura_Hora2["Pregunta8"].ToString() != "")
                            {
                                p8 = Lectura_Hora2["Pregunta8"].ToString() + " - 8";
                                res = res + "\n\r" + p8;
                            }

                            if (Lectura_Hora2["Pregunta9"].ToString() != "")
                            {
                                p9 = Lectura_Hora2["Pregunta9"].ToString() + " - 9";
                                res = res + "\n\r" + p9;
                            }

                            if (Lectura_Hora2["Pregunta10"].ToString() != "")
                            {
                                p10 = Lectura_Hora2["Pregunta10"].ToString() + " - 10";
                                res = res + "\n\r" + p10;
                            }

                            if (Lectura_Hora2["Pregunta11"].ToString() != "")
                            {
                                p11 = Lectura_Hora2["Pregunta11"].ToString() + " - 11";
                                res = res + "\n\r" + p11;
                            }

                            if (Lectura_Hora2["Pregunta12"].ToString() != "")
                            {
                                p12 = Lectura_Hora2["Pregunta12"].ToString() + " - 12";
                                res = res + "\n\r" + p12;
                            }

                            if (Lectura_Hora2["Pregunta13"].ToString() != "")
                            {
                                p13 = Lectura_Hora2["Pregunta13"].ToString() + " - 13";
                                res = res + "\n\r" + p13;
                            }

                            if (Lectura_Hora2["Pregunta14"].ToString() != "")
                            {
                                p14 = Lectura_Hora2["Pregunta14"].ToString() + " - 14";
                                res = res + "\n\r" + p14;
                            }

                            if (Lectura_Hora2["Pregunta15"].ToString() != "")
                            {
                                p15 = Lectura_Hora2["Pregunta15"].ToString() + " - 15";
                                res = res + "\n\r" + p15;
                            }

                            if (Lectura_Hora2["Pregunta16"].ToString() != "")
                            {
                                p16 = Lectura_Hora2["Pregunta16"].ToString() + " - 16";
                                res = res + "\n\r" + p16;
                            }

                            if (Lectura_Hora2["Pregunta17"].ToString() != "")
                            {
                                p17 = Lectura_Hora2["Pregunta17"].ToString() + " - 17";
                                res = res + "\n\r" + p17;
                            }

                            if (Lectura_Hora2["Pregunta18"].ToString() != "")
                            {
                                p18 = Lectura_Hora2["Pregunta18"].ToString() + " - 18";
                                res = res + "\n\r" + p18;
                            }

                            if (Lectura_Hora2["Pregunta19"].ToString() != "")
                            {
                                p19 = Lectura_Hora2["Pregunta19"].ToString() + " - 19";
                                res = res + "\n\r" + p19;
                            }

                            E2.Add(new FIB_ENCUESTA2
                            {
                                P57 = Lectura_Hora2["Pac_PrimerA"].ToString() + " " +
                                                             Lectura_Hora2["Pac_SegundoA"].ToString() + " " +
                                                             Lectura_Hora2["Pac_PrimerN"].ToString() + " " +
                                                             Lectura_Hora2["Pac_SegundoN"].ToString(),
                                FechaEncuesta = Convert.ToDateTime(Lectura_Hora2["FechaEncuesta"]),
                                Id = Convert.ToInt32(Lectura_Hora2["Id"]),
                                IdPaciente = Convert.ToInt32(Lectura_Hora2["IdPaciente"]),
                                P19 = Lectura_Hora2["ResultadoP19"].ToString(),
                                P20 = getResultC2(Convert.ToInt32(Lectura_Hora2["Fatiga"])) + " - " + Lectura_Hora2["Fatiga"].ToString(), //fatiga
                                P21 = getResultC2(Convert.ToInt32(Lectura_Hora2["Sueño"])) + " - " + Lectura_Hora2["Sueño"].ToString(), //sueño
                                P22 = getResultC2(Convert.ToInt32(Lectura_Hora2["Trastorno"])) + " - " + Lectura_Hora2["Trastorno"].ToString(), // trastorno
                                P23 = Lectura_Hora2["ResultadoP3"].ToString(),
                                P1 = res
                            });
                        }

                        return E2;
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

        List<FIB_ENCUESTA1> IExport.ExportarEncuesta1(int Id)
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
                    String Cargar_Hora2 = "SELECT *  " +
                                          "FROM FIB_ENCUESTA1 " +
                                          "INNER JOIN CXN_PACIENTES ON FIB_ENCUESTA1.IdPaciente = CXN_PACIENTES.Pac_Id " +
                                          "WHERE FIB_ENCUESTA1.Id = '" + Id + "'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        List<FIB_ENCUESTA1> E1 = new List<FIB_ENCUESTA1>();

                        while (Lectura_Hora2.Read() == true)
                        {
                            E1.Add(new FIB_ENCUESTA1
                            {
                                FechaEncuesta = Convert.ToDateTime(Lectura_Hora2["FechaEncuesta"]),
                                Id = Convert.ToInt32(Lectura_Hora2["Id"]),
                                IdPaciente = Convert.ToInt32(Lectura_Hora2["IdPaciente"]),
                                UsuarioRegistra = Lectura_Hora2["UsuarioRegistra"].ToString(),
                                P57 = Lectura_Hora2["Pac_PrimerA"].ToString() + " " +
                                                             Lectura_Hora2["Pac_SegundoA"].ToString() + " " +
                                                             Lectura_Hora2["Pac_PrimerN"].ToString() + " " +
                                                             Lectura_Hora2["Pac_SegundoN"].ToString(),
                                P1 = getResult(Convert.ToInt32(Lectura_Hora2["Pregunta1"])).ToString() + " - " + Lectura_Hora2["Pregunta1"].ToString(),
                                P2 = getResult(Convert.ToInt32(Lectura_Hora2["Pregunta2"])) + " - " + Lectura_Hora2["Pregunta2"].ToString(),
                                P3 = getResult(Convert.ToInt32(Lectura_Hora2["Pregunta3"])) + " - " + Lectura_Hora2["Pregunta3"].ToString(),
                                P4 = getResult(Convert.ToInt32(Lectura_Hora2["Pregunta4"])) + " - " + Lectura_Hora2["Pregunta4"].ToString(),
                                P5 = getResult(Convert.ToInt32(Lectura_Hora2["Pregunta5"])) + " - " + Lectura_Hora2["Pregunta5"].ToString(),
                                P6 = getResult(Convert.ToInt32(Lectura_Hora2["Pregunta6"])) + " - " + Lectura_Hora2["Pregunta6"].ToString(),
                                P7 = getResult(Convert.ToInt32(Lectura_Hora2["Pregunta7"])) + " - " + Lectura_Hora2["Pregunta7"].ToString(),
                                P8 = getResult(Convert.ToInt32(Lectura_Hora2["Pregunta8"])) + " - " + Lectura_Hora2["Pregunta8"].ToString(),
                                P9 = getResult(Convert.ToInt32(Lectura_Hora2["Pregunta9"])) + " - " + Lectura_Hora2["Pregunta9"].ToString(),
                                P10 = getResult(Convert.ToInt32(Lectura_Hora2["Pregunta10"])) + " - " + Lectura_Hora2["Pregunta10"].ToString(),
                                P11 = getResult(Convert.ToInt32(Lectura_Hora2["Pregunta11"])) + " - " + Lectura_Hora2["Pregunta11"].ToString(),
                                P12 = Lectura_Hora2["Pregunta12"].ToString(),
                                P13 = Lectura_Hora2["Pregunta13"].ToString(),
                                P14 = Lectura_Hora2["Pregunta14"].ToString(),
                                P15 = Lectura_Hora2["Pregunta15"].ToString(),
                                P16 = Lectura_Hora2["Pregunta16"].ToString(),
                                P17 = Lectura_Hora2["Pregunta17"].ToString(),
                                P18 = Lectura_Hora2["Pregunta18"].ToString(),
                                P19 = Lectura_Hora2["Pregunta19"].ToString(),
                                P20 = Lectura_Hora2["Pregunta20"].ToString(),
                                P21 = getResult2(Convert.ToInt32(Lectura_Hora2["Pregunta21"])) + " - " + Lectura_Hora2["Pregunta21"].ToString(),
                                P22 = getResult2(Convert.ToInt32(Lectura_Hora2["Pregunta22"])) + " - " + Lectura_Hora2["Pregunta22"].ToString(),
                                P23 = getResult3(Convert.ToInt32(Lectura_Hora2["Pregunta23"])) + " - " + Lectura_Hora2["Pregunta23"].ToString(),
                                P24 = getResult3(Convert.ToInt32(Lectura_Hora2["Pregunta24"])) + " - " + Lectura_Hora2["Pregunta24"].ToString(),
                                P25 = getResult3(Convert.ToInt32(Lectura_Hora2["Pregunta25"])) + " - " + Lectura_Hora2["Pregunta25"].ToString(),
                                P26 = getResult3(Convert.ToInt32(Lectura_Hora2["Pregunta26"])) + " - " + Lectura_Hora2["Pregunta26"].ToString(),
                                P27 = getResult3(Convert.ToInt32(Lectura_Hora2["Pregunta27"])) + " - " + Lectura_Hora2["Pregunta27"].ToString(),
                                P28 = getResult3(Convert.ToInt32(Lectura_Hora2["Pregunta28"])) + " - " + Lectura_Hora2["Pregunta28"].ToString(),
                                P29 = getResult3(Convert.ToInt32(Lectura_Hora2["Pregunta29"])) + " - " + Lectura_Hora2["Pregunta29"].ToString(),
                                P30 = getResult3(Convert.ToInt32(Lectura_Hora2["Pregunta30"])) + " - " + Lectura_Hora2["Pregunta30"].ToString(),
                                P31 = getResult3(Convert.ToInt32(Lectura_Hora2["Pregunta31"])) + " - " + Lectura_Hora2["Pregunta31"].ToString(),
                                P32 = getResult3(Convert.ToInt32(Lectura_Hora2["Pregunta32"])) + " - " + Lectura_Hora2["Pregunta32"].ToString(),
                                P33 = getResult4(Convert.ToInt32(Lectura_Hora2["Pregunta33"])) + " - " + Lectura_Hora2["Pregunta33"].ToString(),
                                P34 = getResult4(Convert.ToInt32(Lectura_Hora2["Pregunta34"])) + " - " + Lectura_Hora2["Pregunta34"].ToString(),
                                P35 = getResult4(Convert.ToInt32(Lectura_Hora2["Pregunta35"])) + " - " + Lectura_Hora2["Pregunta35"].ToString(),
                                P36 = getResult4(Convert.ToInt32(Lectura_Hora2["Pregunta36"])) + " - " + Lectura_Hora2["Pregunta36"].ToString(),
                                P37 = getResult4(Convert.ToInt32(Lectura_Hora2["Pregunta37"])) + " - " + Lectura_Hora2["Pregunta37"].ToString(),
                                P38 = getResult4(Convert.ToInt32(Lectura_Hora2["Pregunta38"])) + " - " + Lectura_Hora2["Pregunta38"].ToString(),
                                P39 = getResult4(Convert.ToInt32(Lectura_Hora2["Pregunta39"])) + " - " + Lectura_Hora2["Pregunta39"].ToString(),
                                P40 = getResult5(Convert.ToInt32(Lectura_Hora2["Pregunta40"])) + " - " + Lectura_Hora2["Pregunta40"].ToString(),
                                P41 = getResult5(Convert.ToInt32(Lectura_Hora2["Pregunta41"])) + " - " + Lectura_Hora2["Pregunta41"].ToString(),
                                P42 = getResult5(Convert.ToInt32(Lectura_Hora2["Pregunta42"])) + " - " + Lectura_Hora2["Pregunta42"].ToString(),
                                P43 = getResult6(Convert.ToInt32(Lectura_Hora2["Pregunta43"])) + " - " + Lectura_Hora2["Pregunta43"].ToString(),
                                P44 = getResult6(Convert.ToInt32(Lectura_Hora2["Pregunta44"])) + " - " + Lectura_Hora2["Pregunta44"].ToString(),
                                P45 = getResult6(Convert.ToInt32(Lectura_Hora2["Pregunta45"])) + " - " + Lectura_Hora2["Pregunta45"].ToString(),
                                P46 = getResult6(Convert.ToInt32(Lectura_Hora2["Pregunta46"])) + " - " + Lectura_Hora2["Pregunta46"].ToString(),
                                P47 = getResult6(Convert.ToInt32(Lectura_Hora2["Pregunta47"])) + " - " + Lectura_Hora2["Pregunta47"].ToString(),
                                P48 = getResult6(Convert.ToInt32(Lectura_Hora2["Pregunta48"])) + " - " + Lectura_Hora2["Pregunta48"].ToString(),
                                P49 = getResult6(Convert.ToInt32(Lectura_Hora2["Pregunta49"])) + " - " + Lectura_Hora2["Pregunta49"].ToString(),
                                P50 = getResult6(Convert.ToInt32(Lectura_Hora2["Pregunta50"])) + " - " + Lectura_Hora2["Pregunta50"].ToString(),
                                P51 = getResult6(Convert.ToInt32(Lectura_Hora2["Pregunta51"])) + " - " + Lectura_Hora2["Pregunta51"].ToString(),
                                P52 = getResult7(Convert.ToInt32(Lectura_Hora2["Pregunta52"])) + " - " + Lectura_Hora2["Pregunta52"].ToString(),
                                P53 = getResult8(Convert.ToInt32(Lectura_Hora2["Pregunta53"])) + " - " + Lectura_Hora2["Pregunta53"].ToString(),
                                P54 = getResult8(Convert.ToInt32(Lectura_Hora2["Pregunta54"])) + " - " + Lectura_Hora2["Pregunta54"].ToString(),
                                P55 = getResult8(Convert.ToInt32(Lectura_Hora2["Pregunta55"])) + " - " + Lectura_Hora2["Pregunta55"].ToString(),
                                P56 = getResult8(Convert.ToInt32(Lectura_Hora2["Pregunta56"])) + " - " + Lectura_Hora2["Pregunta56"].ToString()
                            });
                        };

                        return E1;
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
    }
}
