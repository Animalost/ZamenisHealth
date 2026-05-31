using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using Domain.Fibromialgia;
using Persistence.Fibromialgia.Interfaces;
using Domain;

namespace Persistence.Fibromialgia.Metodos
{
    public class MImpactoFibromialgia : IImpactoFibromialgia
    {
        List<E1Respuestas> R = new List<E1Respuestas>();

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

        List<E1Respuestas> getResultPacE1(DateTime desde,
                                                  DateTime hasta,
                                                  int Paciente)
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

                    //PRIMERA
                    String Primera = "SELECT TOP 1 * " +
                                          "FROM FIB_ENCUESTA1 " +
                                          "INNER JOIN CXN_PACIENTES ON FIB_ENCUESTA1.IdPaciente = CXN_PACIENTES.Pac_Id " +
                                          "WHERE FIB_ENCUESTA1.FechaEncuesta BETWEEN '" + Convert.ToDateTime(desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(hasta).ToString("yyyy-MM-dd") + "' " +
                                          "AND FIB_ENCUESTA1.Estado = 'V' " +
                                          "AND FIB_ENCUESTA1.IdPaciente = '" + Paciente + "' " +
                                          "ORDER BY FIB_ENCUESTA1.FechaEncuesta ASC";

                    //ULTIMA
                    String Ultima = "SELECT TOP 1 * " +
                                    "FROM FIB_ENCUESTA1 " +
                                    "INNER JOIN CXN_PACIENTES ON FIB_ENCUESTA1.IdPaciente = CXN_PACIENTES.Pac_Id " +
                                    "WHERE FIB_ENCUESTA1.FechaEncuesta BETWEEN '" + Convert.ToDateTime(desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(hasta).ToString("yyyy-MM-dd") + "' " +
                                    "AND FIB_ENCUESTA1.Estado = 'V' " +
                                    "AND FIB_ENCUESTA1.IdPaciente = '" + Paciente + "' " +
                                    "ORDER BY FIB_ENCUESTA1.FechaEncuesta DESC";

                    SqlCommand Carga_Command2 = new SqlCommand(Primera, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.Read() == true)
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

                        SqlCommand Carga_Command = new SqlCommand(Ultima, con);
                        SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                        if (Lectura_Hora.Read() == true)
                        {
                            Double Pregunta1E1 = Convert.ToDouble(Lectura_Hora["Pregunta1"]);
                            Double Pregunta2E1 = Convert.ToDouble(Lectura_Hora["Pregunta2"]);
                            Double Pregunta3E1 = Convert.ToDouble(Lectura_Hora["Pregunta3"]);
                            Double Pregunta4E1 = Convert.ToDouble(Lectura_Hora["Pregunta4"]);
                            Double Pregunta5E1 = Convert.ToDouble(Lectura_Hora["Pregunta5"]);
                            Double Pregunta6E1 = Convert.ToDouble(Lectura_Hora["Pregunta6"]);
                            Double Pregunta7E1 = Convert.ToDouble(Lectura_Hora["Pregunta7"]);
                            Double Pregunta8E1 = Convert.ToDouble(Lectura_Hora["Pregunta8"]);
                            Double Pregunta9E1 = Convert.ToDouble(Lectura_Hora["Pregunta9"]);
                            Double Pregunta10E1 = Convert.ToDouble(Lectura_Hora["Pregunta10"]);
                            Double Pregunta11E1 = Convert.ToDouble(Lectura_Hora["Pregunta11"]);
                            Double Pregunta12E1 = ConsultaDias(Convert.ToDouble(Lectura_Hora["Pregunta12"]));
                            Double Pregunta13E1 = Convert.ToDouble(Lectura_Hora["Pregunta13"]);
                            Double Pregunta14E1 = Convert.ToDouble(Lectura_Hora["Pregunta14"]);
                            Double Pregunta15E1 = Convert.ToDouble(Lectura_Hora["Pregunta15"]);
                            Double Pregunta16E1 = Convert.ToDouble(Lectura_Hora["Pregunta16"]);
                            Double Pregunta17E1 = Convert.ToDouble(Lectura_Hora["Pregunta17"]);
                            Double Pregunta18E1 = Convert.ToDouble(Lectura_Hora["Pregunta18"]);
                            Double Pregunta19E1 = Convert.ToDouble(Lectura_Hora["Pregunta19"]);
                            Double Pregunta20E1 = Convert.ToDouble(Lectura_Hora["Pregunta20"]);

                            Double Pregunta23E1 = Consulta2(Convert.ToDouble(Lectura_Hora["Pregunta23"])); // 3 de las 36
                            Double Pregunta24E1 = Consulta2(Convert.ToDouble(Lectura_Hora["Pregunta24"]));  // 4 de las 36
                            Double Pregunta25E1 = Consulta2(Convert.ToDouble(Lectura_Hora["Pregunta25"]));  // 5 de las 36
                            Double Pregunta26E1 = Consulta2(Convert.ToDouble(Lectura_Hora["Pregunta26"]));  // 6 de las 36 
                            Double Pregunta27E1 = Consulta2(Convert.ToDouble(Lectura_Hora["Pregunta27"]));  // 7 de las 36
                            Double Pregunta28E1 = Consulta2(Convert.ToDouble(Lectura_Hora["Pregunta28"]));  // 8  de las 36
                            Double Pregunta29E1 = Consulta2(Convert.ToDouble(Lectura_Hora["Pregunta29"]));  // 9 de las 36
                            Double Pregunta30E1 = Consulta2(Convert.ToDouble(Lectura_Hora["Pregunta30"]));  // 10 de las 36
                            Double Pregunta31E1 = Consulta2(Convert.ToDouble(Lectura_Hora["Pregunta31"]));  // 11 de las 36
                            Double Pregunta32E1 = Consulta2(Convert.ToDouble(Lectura_Hora["Pregunta32"]));  // 12 de las 36

                            Double Pregunta33E1 = Consulta3(Convert.ToDouble(Lectura_Hora["Pregunta33"])); // 13 de las 36
                            Double Pregunta34E1 = Consulta3(Convert.ToDouble(Lectura_Hora["Pregunta34"]));// 14 de las 36
                            Double Pregunta35E1 = Consulta3(Convert.ToDouble(Lectura_Hora["Pregunta35"]));// 15 de las 36
                            Double Pregunta36E1 = Consulta3(Convert.ToDouble(Lectura_Hora["Pregunta36"]));// 16 de las 36

                            Double Pregunta37E1 = Consulta3(Convert.ToDouble(Lectura_Hora["Pregunta37"]));// 17 de las 36
                            Double Pregunta38E1 = Consulta3(Convert.ToDouble(Lectura_Hora["Pregunta38"]));// 18 de las 36
                            Double Pregunta39E1 = Consulta3(Convert.ToDouble(Lectura_Hora["Pregunta39"]));// 19 de las 36

                            Double Pregunta43E1 = Consulta4(Convert.ToDouble(Lectura_Hora["Pregunta43"])); // 23 de las 36
                            Double Pregunta47E1 = Consulta4(Convert.ToDouble(Lectura_Hora["Pregunta47"])); // 27 de las 36
                            Double Pregunta49E1 = Consulta5(Convert.ToDouble(Lectura_Hora["Pregunta49"])); // 29 de las 36
                            Double Pregunta51E1 = Consulta5(Convert.ToDouble(Lectura_Hora["Pregunta51"])); // 31 de las 36

                            Double Pregunta44E1 = Consulta5(Convert.ToDouble(Lectura_Hora["Pregunta44"]));  // 24 de las 36
                            Double Pregunta45E1 = Consulta5(Convert.ToDouble(Lectura_Hora["Pregunta45"])); // 25 de las 36
                            Double Pregunta46E1 = Consulta4(Convert.ToDouble(Lectura_Hora["Pregunta46"])); // 26 de las 36
                            Double Pregunta48E1 = Consulta5(Convert.ToDouble(Lectura_Hora["Pregunta48"])); // 28 de las 36
                            Double Pregunta50E1 = Consulta4(Convert.ToDouble(Lectura_Hora["Pregunta50"])); // 30 de las 36

                            Double Pregunta40E1 = Consulta1(Convert.ToDouble(Lectura_Hora["Pregunta40"])); // 20 de las 36
                            Double Pregunta52E1 = Consulta6(Convert.ToDouble(Lectura_Hora["Pregunta52"])); // 32 de las 36

                            Double Pregunta41E1 = Consulta4(Convert.ToDouble(Lectura_Hora["Pregunta41"])); // 21 de las 36
                            Double Pregunta42E1 = Consulta1(Convert.ToDouble(Lectura_Hora["Pregunta42"])); // 22 de las 36

                            Double Pregunta21E1 = Consulta1(Convert.ToDouble(Lectura_Hora["Pregunta21"])); // 1 de las 36                                                      
                            Double Pregunta22E1 = Consulta1(Convert.ToDouble(Lectura_Hora["Pregunta22"])); // 2 de las 36

                            Double Pregunta53E1 = Consulta6(Convert.ToDouble(Lectura_Hora["Pregunta53"])); // 33 de las 36
                            Double Pregunta54E1 = Consulta1(Convert.ToDouble(Lectura_Hora["Pregunta54"])); // 34 de las 36
                            Double Pregunta55E1 = Consulta6(Convert.ToDouble(Lectura_Hora["Pregunta55"])); // 35 de las 36
                            Double Pregunta56E1 = Consulta1(Convert.ToDouble(Lectura_Hora["Pregunta56"])); // 36 de las 36

                            Double Suma11E1 = Pregunta1E1 + Pregunta2E1 + Pregunta3E1 + Pregunta4E1 + Pregunta5E1 + Pregunta6E1 +
                                Pregunta7E1 + Pregunta8E1 + Pregunta9E1 + Pregunta10E1 + Pregunta11E1;

                            Double operacionE1 = Suma11E1 / 11;
                            double OPE1 = operacionE1 + Pregunta12E1 + Pregunta13E1 + Pregunta14E1 + Pregunta15E1 + Pregunta16E1 +
                                        Pregunta17E1 + Pregunta18E1 + Pregunta19E1 + Pregunta20E1; //DEVOLVER

                            Double Suma10E1 = Pregunta23E1 + Pregunta24E1 + Pregunta25E1 + Pregunta26E1 + Pregunta27E1 + Pregunta28E1 +
                                Pregunta29E1 + Pregunta30E1 + Pregunta31E1 + Pregunta32E1;
                            double OP2E1 = Suma10E1 / 10; //DEVOLVER Funcion Fisica

                            Double Suma4E1 = Pregunta33E1 + Pregunta34E1 + Pregunta35E1 + Pregunta36E1;
                            double OP3E1 = Suma4E1 / 4; //DEVOLVER

                            //Tercera --> Guia
                            Double Suma3E1 = Pregunta37E1 + Pregunta38E1 + Pregunta39E1;
                            double OP4E1 = Suma3E1 / 3; //DEVOLVER

                            Double Suma42E1 = Pregunta43E1 + Pregunta47E1 + Pregunta49E1 + Pregunta51E1;
                            double OP5E1 = Suma42E1 / 4; //DEVOLVER

                            Double Suma5E1 = Pregunta44E1 + Pregunta45E1 + Pregunta46E1 + Pregunta48E1 + Pregunta50E1;
                            double OP6E1 = Suma5E1 / 5; //DEVOLVER

                            Double Suma2E1 = Pregunta40E1 + Pregunta52E1;
                            double OP7E1 = Suma2E1 / 2; //DEVOLVER

                            Double Suma22E1 = Pregunta41E1 + Pregunta42E1;
                            double OP8E1 = Suma22E1 / 2; //DEVOLVER

                            Double Suma6E1 = Pregunta21E1 + Pregunta22E1 + Pregunta53E1 + Pregunta54E1 + Pregunta55E1 + Pregunta56E1;
                            double OP9E1 = Suma6E1 / 6; //DEVOLVER

                            double Promedio8E1 = (OP2E1 + OP3E1 + OP4E1 + OP5E1 + OP6E1 + OP7E1 + OP8E1 + OP9E1) / 8; //DEVOLVER

                            double promedioEmocionalE1 = (OP4E1 + OP6E1 + OP7E1 + OP9E1) / 4; //DEVOLVER
                            double promedioFisicoE1 = (OP2E1 + OP3E1 + OP5E1 + OP8E1) / 4; //DEVOLVER

                            R.Add(new E1Respuestas
                            {
                                FIQ1 = OP,
                                D1111 = OP2,
                                D1121 = OP3,
                                D1131 = OP4,
                                D1141 = OP5,
                                D1151 = OP6,
                                D1161 = OP7,
                                D1171 = OP8,
                                D1181 = OP9,
                                Promedio81 = Promedio8,
                                promedioEmocional1 = promedioEmocional,
                                promedioFisico1 = promedioFisico,
                                paciente = Lectura_Hora2["Pac_PrimerA"].ToString() + " " + Lectura_Hora2["Pac_SegundoA"].ToString() + " " + Lectura_Hora2["Pac_PrimerN"].ToString() + " " + Lectura_Hora2["Pac_SegundoN"].ToString(),
                                FIQ2 = OPE1,
                                D1112 = OP2E1,
                                D1122 = OP3E1,
                                D1132 = OP4E1,
                                D1142 = OP5E1,
                                D1152 = OP6E1,
                                D1162 = OP7E1,
                                D1172 = OP8E1,
                                D1182 = OP9E1,
                                Promedio82 = Promedio8E1,
                                promedioEmocional2 = promedioEmocionalE1,
                                promedioFisico2 = promedioFisicoE1,
                                idPaciente = Convert.ToInt32(Lectura_Hora2["IdPaciente"])
                            });
                        }
                        else
                        {
                            return null;
                        }

                        return R;
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

        List<int> getIdPaciente(DateTime Desde, DateTime Hasta, string Tabla)
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

                    String Query = "";

                    if (Tabla == "E1")
                    {
                        Query = "SELECT DISTINCT IdPaciente  " +
                                "FROM FIB_ENCUESTA1 " +
                                "WHERE FIB_ENCUESTA1.FechaEncuesta BETWEEN '" + Convert.ToDateTime(Desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(Hasta).ToString("yyyy-MM-dd") + "' " +
                                "AND Estado = 'V'";
                    }

                    if (Tabla == "E2")
                    {
                        Query = "SELECT DISTINCT IdPaciente  " +
                                "FROM FIB_ENCUESTA2 " +
                                "WHERE FIB_ENCUESTA2.FechaEncuesta BETWEEN '" + Convert.ToDateTime(Desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(Hasta).ToString("yyyy-MM-dd") + "' " +
                                "AND Estado = 'V'";
                    }

                    SqlCommand Carga_Command2 = new SqlCommand(Query, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        List<int> Id = new List<int>();

                        while (Lectura_Hora2.Read() == true)
                        {
                            Id.Add(Convert.ToInt32(Lectura_Hora2["IdPaciente"]));
                        }

                        return Id;
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

        Byte[] ChartsgenerateGraphics(string Q1, string Q2, string Titulo,
                                      double Por1, double Por2)
        {
            var chart = new Chart
            {
                Width = 600,
                Height = 450,
                RenderType = RenderType.ImageTag,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };

            chart.Titles.Add(Titulo);
            chart.Titles[0].Font = new System.Drawing.Font("Arial", 12f);

            chart.ChartAreas.Add("");
            chart.ChartAreas[0].AxisX.Title = "";
            chart.ChartAreas[0].AxisY.Title = "Nivel";
            chart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 12f);
            chart.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 12f);
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 10f);
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
            chart.ChartAreas[0].BackColor = Color.White;

            chart.Series.Add("");
            chart.Series[0].ChartType = SeriesChartType.Column;

            chart.Series[0].Points.AddXY(Q1 + "   " + Por1.ToString("N2") + " %   .", Por1);
            chart.Series[0].Points.AddXY(Q2 + "   " + Por2.ToString("N2") + " %   .", Por2);

            var ms = new System.IO.MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Jpeg);
            byte[] pdfBytes = ms.ToArray();
            return pdfBytes;
        }

        List<int> TotalPacientes(DateTime desde, DateTime hasta)
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

                    String Cargar_Hora2 = "SELECT DISTINCT IdPaciente " +
                                          "FROM FIB_ENCUESTA1 " +
                                          "WHERE FechaEncuesta BETWEEN '" + Convert.ToDateTime(desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(hasta).ToString("yyyy-MM-dd") + "' " +
                                          "AND Estado = 'V'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        List<int> L = new List<int>();

                        while (Lectura_Hora2.Read() == true)
                        {
                            L.Add(Convert.ToInt32(Lectura_Hora2["IdPaciente"]));
                        }

                        return L;
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

        List<Preguntas3> IImpactoFibromialgia.ExportarImpacto(DateTime Desde,
                                                              DateTime Hasta)
        {
            try
            {
                Dictionary<int, Dictionary<string, double>> D = new Dictionary<int, Dictionary<string, double>>();

                var getIdsE1 = getIdPaciente(Desde, Hasta, "E1");
                if (getIdsE1 == null)
                {
                    return null;
                }

                var getIdsE2 = getIdPaciente(Desde, Hasta, "E2");
                if (getIdsE2 == null)
                {
                    return null;
                }

                R.Clear();
                foreach (var datosE1 in getIdsE1)
                {
                    var dicResE1 = getResultPacE1(Desde, Hasta, datosE1); //llenar lista global de rsultaos
                }

                double _totalPacientes = TotalPacientes(Desde, Hasta).Count();

                double AreaFisicaIngreso = 0;
                double AreaFisicaEgreso = 0;
                double AreaEmocionalIngreso = 0;
                double AreaEmocionalEgreso = 0;
                double AreaImpactoIngreso = 0;
                double AreaImpactoEgreso = 0;

                foreach (var i in R)
                {
                    AreaFisicaIngreso = AreaFisicaIngreso + i.promedioFisico1;
                    AreaFisicaEgreso = AreaFisicaEgreso + i.promedioFisico2;
                    AreaEmocionalIngreso = AreaEmocionalIngreso + i.promedioEmocional1;
                    AreaEmocionalEgreso = AreaEmocionalEgreso + i.promedioEmocional2;
                    AreaImpactoIngreso = AreaImpactoIngreso + i.FIQ1;
                    AreaImpactoEgreso = AreaImpactoEgreso + i.FIQ2;
                }

                string Tit = "";
                string Q1 = "";
                string Q2 = "";
                double Por1 = 0;
                double Por2 = 0;

                //FISICA
                Tit = "SF36 Area Fisica";
                Q1 = "Ingreso";
                Q2 = "Egreso";
                Por1 = AreaFisicaIngreso / _totalPacientes;
                Por2 = AreaFisicaEgreso / _totalPacientes;

                var _chartFisica = ChartsgenerateGraphics(Q1, Q2, Tit, Por1, Por2);

                //EMOCIONAL
                Tit = "SF36 Area Emocional";
                Q1 = "Ingreso";
                Q2 = "Egreso";
                Por1 = AreaEmocionalIngreso / _totalPacientes;
                Por2 = AreaEmocionalEgreso / _totalPacientes;

                var _chartEmocional = ChartsgenerateGraphics(Q1, Q2, Tit, Por1, Por2);

                //IMPACTO
                Por1 = AreaImpactoIngreso / _totalPacientes;
                Por2 = AreaImpactoEgreso / _totalPacientes;
                double oper = Por1 - Por2;
                Tit = "Impacto de Fibromialgia FIQ - EVOLUCION: " + oper.ToString("N2") + " %";
                Q1 = "Ingreso";
                Q2 = "Egreso";


                var _chartImpacto = ChartsgenerateGraphics(Q1, Q2, Tit, Por1, Por2);

                List<Preguntas3> L = new List<Preguntas3>();

                L.Add(new Preguntas3
                {
                    Graph1 = _chartFisica,
                    Graph2 = _chartEmocional,
                    Graph3 = _chartImpacto,
                    Desde = Convert.ToDateTime(Desde),
                    Hasta = Convert.ToDateTime(Hasta)
                });

                return L;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
    }
}
