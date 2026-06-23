using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Persistence.Fibromialgia.Interfaces;
using Domain.Fibromialgia;
using Domain;

namespace Persistence.Fibromialgia.Metodos
{
    public class MEncuestasXPaciente : IEncuestasXPaciente
    {
        double Consulta1(double Valor)
        {
            double res = 0;

            switch (Valor)
            {
                case 1:
                    res = 100;
                    break;
                case 2:
                    res = 75;
                    break;
                case 3:
                    res = 50;
                    break;
                case 4:
                    res = 25;
                    break;
                default:
                    res = 0;
                    break;
            }
            return res;
        }
        double Consulta2(double Valor)
        {
            double res = 0;

            switch (Valor)
            {
                case 1:
                    res = 0;
                    break;
                case 2:
                    res = 50;
                    break;
                default:
                    res = 100;
                    break;
            }
            return res;
        }
        double Consulta3(double Valor)
        {
            double res = 0;

            switch (Valor)
            {
                case 1:
                    res = 0;
                    break;
                default:
                    res = 100;
                    break;
            }
            return res;
        }
        double Consulta4(double Valor)
        {
            double res = 0;

            switch (Valor)
            {
                case 1:
                    res = 100;
                    break;
                case 2:
                    res = 80;
                    break;
                case 3:
                    res = 60;
                    break;
                case 4:
                    res = 40;
                    break;
                case 5:
                    res = 20;
                    break;
                default:
                    res = 0;
                    break;
            }
            return res;
        }
        double Consulta5(double Valor)
        {
            double res = 0;

            switch (Valor)
            {
                case 1:
                    res = 0;
                    break;
                case 2:
                    res = 20;
                    break;
                case 3:
                    res = 40;
                    break;
                case 4:
                    res = 60;
                    break;
                case 5:
                    res = 80;
                    break;
                default:
                    res = 100;
                    break;
            }
            return res;
        }
        double Consulta6(double Valor)
        {
            double res = 0;

            switch (Valor)
            {
                case 1:
                    res = 0;
                    break;
                case 2:
                    res = 25;
                    break;
                case 3:
                    res = 50;
                    break;
                case 4:
                    res = 75;
                    break;
                default:
                    res = 100;
                    break;
            }
            return res;
        }
        double ConsultaDias(double Valor)
        {
            return 7 - Valor;
        }
        Dictionary<string, double> getResultPacE1(DateTime desde,
                                                  DateTime hasta,
                                                  string TipoId,
                                                  string NumId)
        {
            Dictionary<string, double> D = new Dictionary<string, double>();

            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Cargar_Hora2 = "SELECT  *  " +
                                          "FROM FIB_ENCUESTA1 " +
                                          "INNER JOIN CXN_PACIENTES ON FIB_ENCUESTA1.IdPaciente = CXN_PACIENTES.Pac_Id " +
                                          "WHERE CXN_PACIENTES.Pac_IdNum = '" + NumId + "' " +
                                          "AND CXN_PACIENTES.Pac_TipoId = '" + TipoId + "' " +
                                          "AND FIB_ENCUESTA1.FechaEncuesta BETWEEN '" + Convert.ToDateTime(desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(hasta).ToString("yyyy-MM-dd") + "' " +
                                          "AND FIB_ENCUESTA1.Estado = 'V' " +
                                          "ORDER BY FIB_ENCUESTA1.FechaEncuesta ASC";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        int Contador = 1;

                        while (Lectura_Hora2.Read() == true)
                        {
                            Double Pregunta1 = Convert.ToDouble(Lectura_Hora2["Pregunta1"]);
                            Double Pregunta2 = Convert.ToDouble(Lectura_Hora2["Pregunta2"]);
                            Double Pregunta3 = Convert.ToDouble(Lectura_Hora2["Pregunta3"]);
                            Double Pregunta4 = Convert.ToDouble(Lectura_Hora2["Pregunta4"]);
                            Double Pregunta5 = Convert.ToDouble(Lectura_Hora2["Pregunta5"]);
                            Double Pregunta6 = Convert.ToDouble(Lectura_Hora2["Pregunta6"]);
                            Double Pregunta7 = Convert.ToDouble(Lectura_Hora2["Pregunta7"]);
                            Double Pregunta8 = Convert.ToDouble(Lectura_Hora2["Pregunta8"]);
                            Double Pregunta9 = Convert.ToDouble(Lectura_Hora2["Pregunta9"]);
                            Double Pregunta10 = Convert.ToDouble(Lectura_Hora2["Pregunta10"]);
                            Double Pregunta11 = Convert.ToDouble(Lectura_Hora2["Pregunta11"]);
                            Double Pregunta12 = ConsultaDias(Convert.ToDouble(Lectura_Hora2["Pregunta12"]));
                            Double Pregunta13 = Convert.ToDouble(Lectura_Hora2["Pregunta13"]);
                            Double Pregunta14 = Convert.ToDouble(Lectura_Hora2["Pregunta14"]);
                            Double Pregunta15 = Convert.ToDouble(Lectura_Hora2["Pregunta15"]);
                            Double Pregunta16 = Convert.ToDouble(Lectura_Hora2["Pregunta16"]);
                            Double Pregunta17 = Convert.ToDouble(Lectura_Hora2["Pregunta17"]);
                            Double Pregunta18 = Convert.ToDouble(Lectura_Hora2["Pregunta18"]);
                            Double Pregunta19 = Convert.ToDouble(Lectura_Hora2["Pregunta19"]);
                            Double Pregunta20 = Convert.ToDouble(Lectura_Hora2["Pregunta20"]);

                            Double Pregunta23 = Consulta2(Convert.ToDouble(Lectura_Hora2["Pregunta23"])); // 3 de las 36
                            Double Pregunta24 = Consulta2(Convert.ToDouble(Lectura_Hora2["Pregunta24"]));  // 4 de las 36
                            Double Pregunta25 = Consulta2(Convert.ToDouble(Lectura_Hora2["Pregunta25"]));  // 5 de las 36
                            Double Pregunta26 = Consulta2(Convert.ToDouble(Lectura_Hora2["Pregunta26"]));  // 6 de las 36 
                            Double Pregunta27 = Consulta2(Convert.ToDouble(Lectura_Hora2["Pregunta27"]));  // 7 de las 36
                            Double Pregunta28 = Consulta2(Convert.ToDouble(Lectura_Hora2["Pregunta28"]));  // 8  de las 36
                            Double Pregunta29 = Consulta2(Convert.ToDouble(Lectura_Hora2["Pregunta29"]));  // 9 de las 36
                            Double Pregunta30 = Consulta2(Convert.ToDouble(Lectura_Hora2["Pregunta30"]));  // 10 de las 36
                            Double Pregunta31 = Consulta2(Convert.ToDouble(Lectura_Hora2["Pregunta31"]));  // 11 de las 36
                            Double Pregunta32 = Consulta2(Convert.ToDouble(Lectura_Hora2["Pregunta32"]));  // 12 de las 36

                            Double Pregunta33 = Consulta3(Convert.ToDouble(Lectura_Hora2["Pregunta33"])); // 13 de las 36
                            Double Pregunta34 = Consulta3(Convert.ToDouble(Lectura_Hora2["Pregunta34"]));// 14 de las 36
                            Double Pregunta35 = Consulta3(Convert.ToDouble(Lectura_Hora2["Pregunta35"]));// 15 de las 36
                            Double Pregunta36 = Consulta3(Convert.ToDouble(Lectura_Hora2["Pregunta36"]));// 16 de las 36

                            Double Pregunta37 = Consulta3(Convert.ToDouble(Lectura_Hora2["Pregunta37"]));// 17 de las 36
                            Double Pregunta38 = Consulta3(Convert.ToDouble(Lectura_Hora2["Pregunta38"]));// 18 de las 36
                            Double Pregunta39 = Consulta3(Convert.ToDouble(Lectura_Hora2["Pregunta39"]));// 19 de las 36

                            Double Pregunta43 = Consulta4(Convert.ToDouble(Lectura_Hora2["Pregunta43"])); // 23 de las 36
                            Double Pregunta47 = Consulta4(Convert.ToDouble(Lectura_Hora2["Pregunta47"])); // 27 de las 36
                            Double Pregunta49 = Consulta5(Convert.ToDouble(Lectura_Hora2["Pregunta49"])); // 29 de las 36
                            Double Pregunta51 = Consulta5(Convert.ToDouble(Lectura_Hora2["Pregunta51"])); // 31 de las 36

                            Double Pregunta44 = Consulta5(Convert.ToDouble(Lectura_Hora2["Pregunta44"]));  // 24 de las 36
                            Double Pregunta45 = Consulta5(Convert.ToDouble(Lectura_Hora2["Pregunta45"])); // 25 de las 36
                            Double Pregunta46 = Consulta4(Convert.ToDouble(Lectura_Hora2["Pregunta46"])); // 26 de las 36
                            Double Pregunta48 = Consulta5(Convert.ToDouble(Lectura_Hora2["Pregunta48"])); // 28 de las 36
                            Double Pregunta50 = Consulta4(Convert.ToDouble(Lectura_Hora2["Pregunta50"])); // 30 de las 36

                            Double Pregunta40 = Consulta1(Convert.ToDouble(Lectura_Hora2["Pregunta40"])); // 20 de las 36
                            Double Pregunta52 = Consulta6(Convert.ToDouble(Lectura_Hora2["Pregunta52"])); // 32 de las 36

                            Double Pregunta41 = Consulta4(Convert.ToDouble(Lectura_Hora2["Pregunta41"])); // 21 de las 36
                            Double Pregunta42 = Consulta1(Convert.ToDouble(Lectura_Hora2["Pregunta42"])); // 22 de las 36

                            Double Pregunta21 = Consulta1(Convert.ToDouble(Lectura_Hora2["Pregunta21"])); // 1 de las 36                                                      
                            Double Pregunta22 = Consulta1(Convert.ToDouble(Lectura_Hora2["Pregunta22"])); // 2 de las 36

                            Double Pregunta53 = Consulta6(Convert.ToDouble(Lectura_Hora2["Pregunta53"])); // 33 de las 36
                            Double Pregunta54 = Consulta1(Convert.ToDouble(Lectura_Hora2["Pregunta54"])); // 34 de las 36
                            Double Pregunta55 = Consulta6(Convert.ToDouble(Lectura_Hora2["Pregunta55"])); // 35 de las 36
                            Double Pregunta56 = Consulta1(Convert.ToDouble(Lectura_Hora2["Pregunta56"])); // 36 de las 36

                            Double Suma11 = Pregunta1 + Pregunta2 + Pregunta3 + Pregunta4 + Pregunta5 + Pregunta6 +
                                Pregunta7 + Pregunta8 + Pregunta9 + Pregunta10 + Pregunta11;

                            Double operacion = Suma11 / 11;
                            double OP = operacion + Pregunta12 + Pregunta13 + Pregunta14 + Pregunta15 + Pregunta16 +
                                        Pregunta17 + Pregunta18 + Pregunta19 + Pregunta20; //DEVOLVER

                            Double Suma10 = Pregunta23 + Pregunta24 + Pregunta25 + Pregunta26 + Pregunta27 + Pregunta28 +
                                Pregunta29 + Pregunta30 + Pregunta31 + Pregunta32;
                            double OP2 = Suma10 / 10; //DEVOLVER Funcion Fisica

                            Double Suma4 = Pregunta33 + Pregunta34 + Pregunta35 + Pregunta36;
                            double OP3 = Suma4 / 4; //DEVOLVER

                            //Tercera --> Guia
                            Double Suma3 = Pregunta37 + Pregunta38 + Pregunta39;
                            double OP4 = Suma3 / 3; //DEVOLVER

                            Double Suma42 = Pregunta43 + Pregunta47 + Pregunta49 + Pregunta51;
                            double OP5 = Suma42 / 4; //DEVOLVER

                            Double Suma5 = Pregunta44 + Pregunta45 + Pregunta46 + Pregunta48 + Pregunta50;
                            double OP6 = Suma5 / 5; //DEVOLVER

                            Double Suma2 = Pregunta40 + Pregunta52;
                            double OP7 = Suma2 / 2; //DEVOLVER

                            Double Suma22 = Pregunta41 + Pregunta42;
                            double OP8 = Suma22 / 2; //DEVOLVER

                            Double Suma6 = Pregunta21 + Pregunta22 + Pregunta53 + Pregunta54 + Pregunta55 + Pregunta56;
                            double OP9 = Suma6 / 6; //DEVOLVER

                            double Promedio8 = (OP2 + OP3 + OP4 + OP5 + OP6 + OP7 + OP8 + OP9) / 8; //DEVOLVER

                            double promedioEmocional = (OP4 + OP6 + OP7 + OP9) / 4; //DEVOLVER
                            double promedioFisico = (OP2 + OP3 + OP5 + OP8) / 4; //DEVOLVER

                            D.Add("FIQ" + Contador, OP); //FIQ
                            D.Add("D111" + Contador, OP2); //Funcion Fisica
                            D.Add("D112" + Contador, OP3);
                            D.Add("D113" + Contador, OP4);
                            D.Add("D114" + Contador, OP5);
                            D.Add("D115" + Contador, OP6);
                            D.Add("D116" + Contador, OP7);
                            D.Add("D117" + Contador, OP8);
                            D.Add("D118" + Contador, OP9);
                            D.Add("Promedio8" + Contador, Promedio8);
                            D.Add("promedioEmocional" + Contador, promedioEmocional);
                            D.Add("promedioFisico" + Contador, promedioFisico);

                            Contador = Contador + 1;
                        }

                        if (Contador == 2)
                        {
                            D.Add("FIQ" + Contador, 0); //FIQ
                            D.Add("D111" + Contador, 0); //Funcion Fisica
                            D.Add("D112" + Contador, 0);
                            D.Add("D113" + Contador, 0);
                            D.Add("D114" + Contador, 0);
                            D.Add("D115" + Contador, 0);
                            D.Add("D116" + Contador, 0);
                            D.Add("D117" + Contador, 0);
                            D.Add("D118" + Contador, 0);
                            D.Add("Promedio8" + Contador, 0);
                            D.Add("promedioEmocional" + Contador, 0);
                            D.Add("promedioFisico" + Contador, 0);

                            Contador = Contador + 1;
                        }

                        if (Contador == 3)
                        {
                            D.Add("FIQ" + Contador, 0); //FIQ
                            D.Add("D111" + Contador, 0); //Funcion Fisica
                            D.Add("D112" + Contador, 0);
                            D.Add("D113" + Contador, 0);
                            D.Add("D114" + Contador, 0);
                            D.Add("D115" + Contador, 0);
                            D.Add("D116" + Contador, 0);
                            D.Add("D117" + Contador, 0);
                            D.Add("D118" + Contador, 0);
                            D.Add("Promedio8" + Contador, 0);
                            D.Add("promedioEmocional" + Contador, 0);
                            D.Add("promedioFisico" + Contador, 0);
                        }

                        return D;
                    }
                    else
                    {
                        D.Add("FIQ" + 1, 0); //FIQ
                        D.Add("D111" + 1, 0); //Funcion Fisica
                        D.Add("D112" + 1, 0);
                        D.Add("D113" + 1, 0);
                        D.Add("D114" + 1, 0);
                        D.Add("D115" + 1, 0);
                        D.Add("D116" + 1, 0);
                        D.Add("D117" + 1, 0);
                        D.Add("D118" + 1, 0);
                        D.Add("Promedio8" + 1, 0);
                        D.Add("promedioEmocional" + 1, 0);
                        D.Add("promedioFisico" + 1, 0);

                        D.Add("FIQ" + 2, 0); //FIQ
                        D.Add("D111" + 2, 0); //Funcion Fisica
                        D.Add("D112" + 2, 0);
                        D.Add("D113" + 2, 0);
                        D.Add("D114" + 2, 0);
                        D.Add("D115" + 2, 0);
                        D.Add("D116" + 2, 0);
                        D.Add("D117" + 2, 0);
                        D.Add("D118" + 2, 0);
                        D.Add("Promedio8" + 2, 0);
                        D.Add("promedioEmocional" + 2, 0);
                        D.Add("promedioFisico" + 2, 0);

                        D.Add("FIQ" + 3, 0); //FIQ
                        D.Add("D111" + 3, 0); //Funcion Fisica
                        D.Add("D112" + 3, 0);
                        D.Add("D113" + 3, 0);
                        D.Add("D114" + 3, 0);
                        D.Add("D115" + 3, 0);
                        D.Add("D116" + 3, 0);
                        D.Add("D117" + 3, 0);
                        D.Add("D118" + 3, 0);
                        D.Add("Promedio8" + 3, 0);
                        D.Add("promedioEmocional" + 3, 0);
                        D.Add("promedioFisico" + 3, 0);

                        return D;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        Dictionary<string, double> getResultPacE2(DateTime desde,
                                                  DateTime hasta,
                                                  string TipoId,
                                                  string NumId)
        {
            Dictionary<string, double> D = new Dictionary<string, double>();

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
                                          "WHERE CXN_PACIENTES.Pac_IdNum = '" + NumId + "' " +
                                          "AND CXN_PACIENTES.Pac_TipoId = '" + TipoId + "' " +
                                          "AND FIB_ENCUESTA2.FechaEncuesta BETWEEN '" + Convert.ToDateTime(desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(hasta).ToString("yyyy-MM-dd") + "' " +
                                          "AND FIB_ENCUESTA2.Estado = 'V' " +
                                          "ORDER BY FIB_ENCUESTA2.FechaEncuesta ASC";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        int Contador = 1;

                        while (Lectura_Hora2.Read() == true)
                        {
                            string Pregunta1 = Lectura_Hora2["Pregunta1"].ToString();
                            string Pregunta2 = Lectura_Hora2["Pregunta2"].ToString();
                            string Pregunta3 = Lectura_Hora2["Pregunta3"].ToString();
                            string Pregunta4 = Lectura_Hora2["Pregunta4"].ToString();
                            string Pregunta5 = Lectura_Hora2["Pregunta5"].ToString();
                            string Pregunta6 = Lectura_Hora2["Pregunta6"].ToString();
                            string Pregunta7 = Lectura_Hora2["Pregunta7"].ToString();
                            string Pregunta8 = Lectura_Hora2["Pregunta8"].ToString();
                            string Pregunta9 = Lectura_Hora2["Pregunta9"].ToString();
                            string Pregunta10 = Lectura_Hora2["Pregunta10"].ToString();
                            string Pregunta11 = Lectura_Hora2["Pregunta11"].ToString();
                            string Pregunta12 = Lectura_Hora2["Pregunta12"].ToString();
                            string Pregunta13 = Lectura_Hora2["Pregunta13"].ToString();
                            string Pregunta14 = Lectura_Hora2["Pregunta14"].ToString();
                            string Pregunta15 = Lectura_Hora2["Pregunta15"].ToString();
                            string Pregunta16 = Lectura_Hora2["Pregunta16"].ToString();
                            string Pregunta17 = Lectura_Hora2["Pregunta17"].ToString();
                            string Pregunta18 = Lectura_Hora2["Pregunta18"].ToString();
                            string Pregunta19 = Lectura_Hora2["Pregunta19"].ToString();

                            int P1 = 0; int P2 = 0; int P3 = 0; int P4 = 0; int P5 = 0; int P6 = 0; int P7 = 0; int P8 = 0; int P9 = 0; int P10 = 0;
                            int P11 = 0; int P12 = 0; int P13 = 0; int P14 = 0; int P15 = 0; int P16 = 0; int P17 = 0; int P18 = 0; int P19 = 0;

                            if (Pregunta1 != "") { P1 = 1; }
                            if (Pregunta2 != "") { P2 = 1; }
                            if (Pregunta3 != "") { P3 = 1; }
                            if (Pregunta4 != "") { P4 = 1; }
                            if (Pregunta5 != "") { P5 = 1; }
                            if (Pregunta6 != "") { P6 = 1; }
                            if (Pregunta7 != "") { P7 = 1; }
                            if (Pregunta8 != "") { P8 = 1; }
                            if (Pregunta9 != "") { P9 = 1; }
                            if (Pregunta10 != "") { P10 = 1; }
                            if (Pregunta11 != "") { P11 = 1; }
                            if (Pregunta12 != "") { P12 = 1; }
                            if (Pregunta13 != "") { P13 = 1; }
                            if (Pregunta14 != "") { P14 = 1; }
                            if (Pregunta15 != "") { P15 = 1; }
                            if (Pregunta16 != "") { P16 = 1; }
                            if (Pregunta17 != "") { P17 = 1; }
                            if (Pregunta18 != "") { P18 = 1; }
                            if (Pregunta19 != "") { P19 = 1; } //DEVOLVER

                            int ResultadoP19 = Convert.ToInt32(Lectura_Hora2["ResultadoP19"]); //DEVOLVER

                            int Fatiga = Convert.ToInt32(Lectura_Hora2["Fatiga"]); //DEVOLVER
                            int Sueño = Convert.ToInt32(Lectura_Hora2["Sueño"]); //DEVOLVER
                            int Trastorno = Convert.ToInt32(Lectura_Hora2["Trastorno"]); //DEVOLVER

                            int ResultadoP3 = Convert.ToInt32(Lectura_Hora2["ResultadoP3"]); //DEVOLVER

                            D.Add("Pregunta01" + Contador, P1);
                            D.Add("Pregunta02" + Contador, P2);
                            D.Add("Pregunta03" + Contador, P3);
                            D.Add("Pregunta04" + Contador, P4);
                            D.Add("Pregunta05" + Contador, P5);
                            D.Add("Pregunta06" + Contador, P6);
                            D.Add("Pregunta07" + Contador, P7);
                            D.Add("Pregunta08" + Contador, P8);
                            D.Add("Pregunta09" + Contador, P9);
                            D.Add("Pregunta10" + Contador, P10);
                            D.Add("Pregunta11" + Contador, P11);
                            D.Add("Pregunta12" + Contador, P12);
                            D.Add("Pregunta13" + Contador, P13);
                            D.Add("Pregunta14" + Contador, P14);
                            D.Add("Pregunta15" + Contador, P15);
                            D.Add("Pregunta16" + Contador, P16);
                            D.Add("Pregunta17" + Contador, P17);
                            D.Add("Pregunta18" + Contador, P18);
                            D.Add("Pregunta19" + Contador, P19);
                            D.Add("ResultadoP19" + Contador, ResultadoP19);
                            D.Add("Fatiga" + Contador, Fatiga);
                            D.Add("Sueño" + Contador, Sueño);
                            D.Add("Trastorno" + Contador, Trastorno);
                            D.Add("ResultadoP3" + Contador, ResultadoP3);

                            Contador = Contador + 1;
                        }

                        if (Contador == 2)
                        {
                            D.Add("Pregunta01" + Contador, 0);
                            D.Add("Pregunta02" + Contador, 0);
                            D.Add("Pregunta03" + Contador, 0);
                            D.Add("Pregunta04" + Contador, 0);
                            D.Add("Pregunta05" + Contador, 0);
                            D.Add("Pregunta06" + Contador, 0);
                            D.Add("Pregunta07" + Contador, 0);
                            D.Add("Pregunta08" + Contador, 0);
                            D.Add("Pregunta09" + Contador, 0);
                            D.Add("Pregunta10" + Contador, 0);
                            D.Add("Pregunta11" + Contador, 0);
                            D.Add("Pregunta12" + Contador, 0);
                            D.Add("Pregunta13" + Contador, 0);
                            D.Add("Pregunta14" + Contador, 0);
                            D.Add("Pregunta15" + Contador, 0);
                            D.Add("Pregunta16" + Contador, 0);
                            D.Add("Pregunta17" + Contador, 0);
                            D.Add("Pregunta18" + Contador, 0);
                            D.Add("Pregunta19" + Contador, 0);
                            D.Add("ResultadoP19" + Contador, 0);
                            D.Add("Fatiga" + Contador, 0);
                            D.Add("Sueño" + Contador, 0);
                            D.Add("Trastorno" + Contador, 0);
                            D.Add("ResultadoP3" + Contador, 0);

                            Contador = Contador + 1;
                        }

                        if (Contador == 3)
                        {
                            D.Add("Pregunta01" + Contador, 0);
                            D.Add("Pregunta02" + Contador, 0);
                            D.Add("Pregunta03" + Contador, 0);
                            D.Add("Pregunta04" + Contador, 0);
                            D.Add("Pregunta05" + Contador, 0);
                            D.Add("Pregunta06" + Contador, 0);
                            D.Add("Pregunta07" + Contador, 0);
                            D.Add("Pregunta08" + Contador, 0);
                            D.Add("Pregunta09" + Contador, 0);
                            D.Add("Pregunta10" + Contador, 0);
                            D.Add("Pregunta11" + Contador, 0);
                            D.Add("Pregunta12" + Contador, 0);
                            D.Add("Pregunta13" + Contador, 0);
                            D.Add("Pregunta14" + Contador, 0);
                            D.Add("Pregunta15" + Contador, 0);
                            D.Add("Pregunta16" + Contador, 0);
                            D.Add("Pregunta17" + Contador, 0);
                            D.Add("Pregunta18" + Contador, 0);
                            D.Add("Pregunta19" + Contador, 0);
                            D.Add("ResultadoP19" + Contador, 0);
                            D.Add("Fatiga" + Contador, 0);
                            D.Add("Sueño" + Contador, 0);
                            D.Add("Trastorno" + Contador, 0);
                            D.Add("ResultadoP3" + Contador, 0);
                        }

                        return D;
                    }
                    else
                    {
                        D.Add("Pregunta01" + 1, 0);
                        D.Add("Pregunta02" + 1, 0);
                        D.Add("Pregunta03" + 1, 0);
                        D.Add("Pregunta04" + 1, 0);
                        D.Add("Pregunta05" + 1, 0);
                        D.Add("Pregunta06" + 1, 0);
                        D.Add("Pregunta07" + 1, 0);
                        D.Add("Pregunta08" + 1, 0);
                        D.Add("Pregunta09" + 1, 0);
                        D.Add("Pregunta10" + 1, 0);
                        D.Add("Pregunta11" + 1, 0);
                        D.Add("Pregunta12" + 1, 0);
                        D.Add("Pregunta13" + 1, 0);
                        D.Add("Pregunta14" + 1, 0);
                        D.Add("Pregunta15" + 1, 0);
                        D.Add("Pregunta16" + 1, 0);
                        D.Add("Pregunta17" + 1, 0);
                        D.Add("Pregunta18" + 1, 0);
                        D.Add("Pregunta19" + 1, 0);
                        D.Add("ResultadoP19" + 1, 0);
                        D.Add("Fatiga" + 1, 0);
                        D.Add("Sueño" + 1, 0);
                        D.Add("Trastorno" + 1, 0);
                        D.Add("ResultadoP3" + 1, 0);

                        D.Add("Pregunta01" + 2, 0);
                        D.Add("Pregunta02" + 2, 0);
                        D.Add("Pregunta03" + 2, 0);
                        D.Add("Pregunta04" + 2, 0);
                        D.Add("Pregunta05" + 2, 0);
                        D.Add("Pregunta06" + 2, 0);
                        D.Add("Pregunta07" + 2, 0);
                        D.Add("Pregunta08" + 2, 0);
                        D.Add("Pregunta09" + 2, 0);
                        D.Add("Pregunta10" + 2, 0);
                        D.Add("Pregunta11" + 2, 0);
                        D.Add("Pregunta12" + 2, 0);
                        D.Add("Pregunta13" + 2, 0);
                        D.Add("Pregunta14" + 2, 0);
                        D.Add("Pregunta15" + 2, 0);
                        D.Add("Pregunta16" + 2, 0);
                        D.Add("Pregunta17" + 2, 0);
                        D.Add("Pregunta18" + 2, 0);
                        D.Add("Pregunta19" + 2, 0);
                        D.Add("ResultadoP19" + 2, 0);
                        D.Add("Fatiga" + 2, 0);
                        D.Add("Sueño" + 2, 0);
                        D.Add("Trastorno" + 2, 0);
                        D.Add("ResultadoP3" + 2, 0);

                        D.Add("Pregunta01" + 3, 0);
                        D.Add("Pregunta02" + 3, 0);
                        D.Add("Pregunta03" + 3, 0);
                        D.Add("Pregunta04" + 3, 0);
                        D.Add("Pregunta05" + 3, 0);
                        D.Add("Pregunta06" + 3, 0);
                        D.Add("Pregunta07" + 3, 0);
                        D.Add("Pregunta08" + 3, 0);
                        D.Add("Pregunta09" + 3, 0);
                        D.Add("Pregunta10" + 3, 0);
                        D.Add("Pregunta11" + 3, 0);
                        D.Add("Pregunta12" + 3, 0);
                        D.Add("Pregunta13" + 3, 0);
                        D.Add("Pregunta14" + 3, 0);
                        D.Add("Pregunta15" + 3, 0);
                        D.Add("Pregunta16" + 3, 0);
                        D.Add("Pregunta17" + 3, 0);
                        D.Add("Pregunta18" + 3, 0);
                        D.Add("Pregunta19" + 3, 0);
                        D.Add("ResultadoP19" + 3, 0);
                        D.Add("Fatiga" + 3, 0);
                        D.Add("Sueño" + 3, 0);
                        D.Add("Trastorno" + 3, 0);
                        D.Add("ResultadoP3" + 3, 0);

                        return D;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        Dictionary<int, string> FechasEncuesta1(DateTime desde,
                                                  DateTime hasta,
                                                  string TipoId,
                                                  string NumId)
        {
            Dictionary<int, string> D = new Dictionary<int, string>();

            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Cargar_Hora2 = "SELECT FechaEncuesta  " +
                                          "FROM FIB_ENCUESTA1 " +
                                          "INNER JOIN CXN_PACIENTES ON FIB_ENCUESTA1.IdPaciente = CXN_PACIENTES.Pac_Id " +
                                          "WHERE CXN_PACIENTES.Pac_IdNum = '" + NumId + "' " +
                                          "AND CXN_PACIENTES.Pac_TipoId = '" + TipoId + "' " +
                                          "AND FIB_ENCUESTA1.FechaEncuesta BETWEEN '" + Convert.ToDateTime(desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(hasta).ToString("yyyy-MM-dd") + "' " +
                                          "AND FIB_ENCUESTA1.Estado = 'V' " +
                                          "ORDER BY FIB_ENCUESTA1.FechaEncuesta ASC";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        int Contador = 1;

                        while (Lectura_Hora2.Read() == true)
                        {
                            D.Add(Contador, Convert.ToDateTime(Lectura_Hora2["FechaEncuesta"]).ToString("yyyy-MM-dd"));
                            Contador = Contador + 1;
                        }

                        if (Contador == 2)
                        {
                            D.Add(2, "N/A");
                            D.Add(3, "N/A");
                        }

                        if (Contador == 3)
                        {
                            D.Add(3, "N/A");
                        }

                        return D;
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

        List<ClaseReportsFibro> IEncuestasXPaciente.ExportarResGlobalPaciente(DateTime desde,
                                                                                     DateTime hasta,
                                                                                     string TipoId,
                                                                                     string NumId,
                                                                                     string usuario)
        {
            try
            {
                var getRes1 = getResultPacE1(desde,
                                             hasta,
                                             TipoId,
                                             NumId); //PRIMER RESULTADO EN DICCIONARIO DE 3 DE DOUBLES 
                if (getRes1 == null)
                {
                    return null;
                }

                var getRes2 = getResultPacE2(desde,
                                             hasta,
                                             TipoId,
                                             NumId); //PRIMER RESULTADO EN DICCIONARIO DE 3 DE DOUBLES
                if (getRes2 == null)
                {
                    return null;
                }

                var FechasEnc1 = FechasEncuesta1(desde,
                                                 hasta,
                                                 TipoId,
                                                 NumId);

                if (FechasEnc1 == null)
                {
                    return null;
                }

                int R1 = Convert.ToInt32(getRes2["Fatiga1"]) + Convert.ToInt32(getRes2["Sueño1"]) + Convert.ToInt32(getRes2["Trastorno1"]);
                int R2 = Convert.ToInt32(getRes2["Fatiga2"]) + Convert.ToInt32(getRes2["Sueño2"]) + Convert.ToInt32(getRes2["Trastorno2"]);
                int R3 = Convert.ToInt32(getRes2["Fatiga3"]) + Convert.ToInt32(getRes2["Sueño3"]) + Convert.ToInt32(getRes2["Trastorno3"]);

                List<ClaseReportsFibro> C = new List<ClaseReportsFibro>();

                DateTime except2 = new DateTime(2000, 01, 01);
                DateTime except3 = new DateTime(2000, 01, 01);

                if (FechasEnc1[2] != "N/A") { except2 = Convert.ToDateTime(FechasEnc1[2]); }
                if (FechasEnc1[3] != "N/A") { except3 = Convert.ToDateTime(FechasEnc1[3]); }

                C.Add(new ClaseReportsFibro
                {
                    Fecha1 = Convert.ToDateTime(desde),
                    Fecha2 = Convert.ToDateTime(hasta),
                    Fecha3 = Convert.ToDateTime(FechasEnc1[1]),
                    Fecha4 = Convert.ToDateTime(except2),
                    Fecha5 = Convert.ToDateTime(except3),
                    Dato1 = getRes1["FIQ1"].ToString("N2"),
                    Dato2 = getRes1["FIQ2"].ToString("N2"),
                    Dato3 = getRes1["FIQ3"].ToString("N2"),
                    Dato4 = getRes1["D1111"].ToString("N2"), //empieza dimension
                    Dato5 = getRes1["D1112"].ToString("N2"),
                    Dato6 = getRes1["D1113"].ToString("N2"),
                    Dato7 = getRes1["D1121"].ToString("N2"), //limitaciones
                    Dato8 = getRes1["D1122"].ToString("N2"),
                    Dato9 = getRes1["D1123"].ToString("N2"),
                    Dato10 = getRes1["D1131"].ToString("N2"),
                    Dato11 = getRes1["D1132"].ToString("N2"),
                    Dato12 = getRes1["D1133"].ToString("N2"),
                    Dato13 = getRes1["D1141"].ToString("N2"),
                    Dato14 = getRes1["D1142"].ToString("N2"),
                    Dato15 = getRes1["D1143"].ToString("N2"),
                    Dato16 = getRes1["D1151"].ToString("N2"),
                    Dato17 = getRes1["D1152"].ToString("N2"),
                    Dato18 = getRes1["D1153"].ToString("N2"),
                    Dato19 = getRes1["D1161"].ToString("N2"),
                    Dato20 = getRes1["D1162"].ToString("N2"),
                    Dato21 = getRes1["D1163"].ToString("N2"),
                    Dato22 = getRes1["D1171"].ToString("N2"),
                    Dato23 = getRes1["D1172"].ToString("N2"),
                    Dato24 = getRes1["D1173"].ToString("N2"),
                    Dato25 = getRes1["D1181"].ToString("N2"),
                    Dato26 = getRes1["D1182"].ToString("N2"),
                    Dato27 = getRes1["D1183"].ToString("N2"),
                    Dato28 = getRes1["Promedio81"].ToString("N2"), //Global , osea respuesta
                    Dato29 = getRes1["Promedio82"].ToString("N2"),
                    Dato30 = getRes1["Promedio83"].ToString("N2"),
                    Dato31 = getRes1["promedioEmocional1"].ToString("N2"),
                    Dato32 = getRes1["promedioEmocional2"].ToString("N2"),
                    Dato33 = getRes1["promedioEmocional3"].ToString("N2"),
                    Dato34 = getRes1["promedioFisico1"].ToString("N2"),
                    Dato35 = getRes1["promedioFisico2"].ToString("N2"),
                    Dato36 = getRes1["promedioFisico3"].ToString("N2"),
                    Dato37 = getRes2["Pregunta011"].ToString(), //ultima tabla
                    Dato38 = getRes2["Pregunta012"].ToString(),
                    Dato39 = getRes2["Pregunta013"].ToString(),
                    Dato40 = getRes2["Pregunta021"].ToString(),
                    Dato41 = getRes2["Pregunta022"].ToString(),
                    Dato42 = getRes2["Pregunta023"].ToString(),
                    Dato43 = getRes2["Pregunta031"].ToString(),
                    Dato44 = getRes2["Pregunta032"].ToString(),
                    Dato45 = getRes2["Pregunta033"].ToString(),
                    Dato46 = getRes2["Pregunta041"].ToString(),
                    Dato47 = getRes2["Pregunta042"].ToString(),
                    Dato48 = getRes2["Pregunta043"].ToString(),
                    Dato49 = getRes2["Pregunta051"].ToString(),
                    Dato50 = getRes2["Pregunta052"].ToString(),
                    Dato51 = getRes2["Pregunta053"].ToString(),
                    Dato52 = getRes2["Pregunta061"].ToString(),
                    Dato53 = getRes2["Pregunta062"].ToString(),
                    Dato54 = getRes2["Pregunta063"].ToString(),
                    Dato55 = getRes2["Pregunta071"].ToString(),
                    Dato56 = getRes2["Pregunta072"].ToString(),
                    Dato57 = getRes2["Pregunta073"].ToString(),
                    Dato58 = getRes2["Pregunta081"].ToString(),
                    Dato59 = getRes2["Pregunta082"].ToString(),
                    Dato60 = getRes2["Pregunta083"].ToString(),
                    Dato61 = getRes2["Pregunta091"].ToString(),
                    Dato62 = getRes2["Pregunta092"].ToString(),
                    Dato63 = getRes2["Pregunta093"].ToString(),
                    Dato64 = getRes2["Pregunta101"].ToString(),
                    Dato65 = getRes2["Pregunta102"].ToString(),
                    Dato66 = getRes2["Pregunta103"].ToString(),
                    Dato67 = getRes2["Pregunta111"].ToString(),
                    Dato68 = getRes2["Pregunta112"].ToString(),
                    Dato69 = getRes2["Pregunta113"].ToString(),
                    Dato70 = getRes2["Pregunta121"].ToString(),
                    Dato71 = getRes2["Pregunta122"].ToString(),
                    Dato72 = getRes2["Pregunta123"].ToString(),
                    Dato73 = getRes2["Pregunta131"].ToString(),
                    Dato74 = getRes2["Pregunta132"].ToString(),
                    Dato75 = getRes2["Pregunta133"].ToString(),
                    Dato76 = getRes2["Pregunta141"].ToString(),
                    Dato77 = getRes2["Pregunta142"].ToString(),
                    Dato78 = getRes2["Pregunta143"].ToString(),
                    Dato79 = getRes2["Pregunta151"].ToString(),
                    Dato80 = getRes2["Pregunta152"].ToString(),
                    Dato81 = getRes2["Pregunta153"].ToString(),
                    Dato82 = getRes2["Pregunta161"].ToString(),
                    Dato83 = getRes2["Pregunta162"].ToString(),
                    Dato84 = getRes2["Pregunta163"].ToString(),
                    Dato85 = getRes2["Pregunta171"].ToString(),
                    Dato86 = getRes2["Pregunta172"].ToString(),
                    Dato87 = getRes2["Pregunta173"].ToString(),
                    Dato88 = getRes2["Pregunta181"].ToString(),
                    Dato89 = getRes2["Pregunta182"].ToString(),
                    Dato90 = getRes2["Pregunta183"].ToString(),
                    Dato91 = getRes2["Pregunta191"].ToString(),
                    Dato92 = getRes2["Pregunta192"].ToString(),
                    Dato93 = getRes2["Pregunta193"].ToString(),
                    Dato94 = getRes2["ResultadoP191"].ToString(),
                    Dato95 = getRes2["ResultadoP192"].ToString(),
                    Dato96 = getRes2["ResultadoP193"].ToString(),
                    Dato97 = getRes2["Fatiga1"].ToString(),
                    Dato98 = getRes2["Fatiga2"].ToString(),
                    Dato99 = getRes2["Fatiga3"].ToString(),
                    Dato100 = getRes2["Sueño1"].ToString(),
                    Dato101 = getRes2["Sueño2"].ToString(),
                    Dato102 = getRes2["Sueño3"].ToString(),
                    Dato103 = getRes2["Trastorno1"].ToString(),
                    Dato104 = getRes2["Trastorno2"].ToString(),
                    Dato105 = getRes2["Trastorno3"].ToString(),
                    Dato106 = R1.ToString(),
                    Dato107 = R2.ToString(),
                    Dato108 = R3.ToString(),
                    Dato109 = usuario
                });

                return C;

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
    }
}
