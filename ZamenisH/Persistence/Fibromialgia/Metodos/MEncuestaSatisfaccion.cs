using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using Domain.Fibromialgia;
using Persistence.Fibromialgia.Interfaces;

using Chart = System.Web.UI.DataVisualization.Charting.Chart;
using AntiAliasingStyles = System.Web.UI.DataVisualization.Charting.AntiAliasingStyles;
using TextAntiAliasingQuality = System.Web.UI.DataVisualization.Charting.TextAntiAliasingQuality;
using SeriesChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType;
using ChartImageFormat = System.Web.UI.DataVisualization.Charting.ChartImageFormat;
using System.Web.UI.DataVisualization.Charting;
using Domain;

namespace Persistence.Fibromialgia.Metodos
{
    public class MEncuestaSatisfaccion : IEncuestaSatisfaccion
    {
        Byte[] ChartsgenerateGraphics(int P1, int P2, int P3, int P4,
           string Q1, string Q2, string Q3, string Q4, string Titulo,
           double Por1, double Por2, double Por3, double Por4)
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
            chart.ChartAreas[0].AxisX.Title = "Pregunta";
            chart.ChartAreas[0].AxisY.Title = "Nivel";
            chart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 12f);
            chart.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 12f);
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 10f);
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
            chart.ChartAreas[0].BackColor = Color.White;

            chart.Series.Add("");
            chart.Series[0].ChartType = SeriesChartType.Column;

            chart.Series[0].Points.AddXY(Q1 + "   " + P1.ToString() + "   " + Por1.ToString("N2") + " %", P1);
            chart.Series[0].Points.AddXY(Q2 + "   " + P2.ToString() + "   " + Por2.ToString("N2") + " %", P2);
            chart.Series[0].Points.AddXY(Q3 + "   " + P3.ToString() + "   " + Por3.ToString("N2") + " %", P3);
            chart.Series[0].Points.AddXY(Q4 + "   " + P4.ToString() + "   " + Por4.ToString("N2") + " %", P4);

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
                                          "FROM FIB_ENCUESTA3 " +
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

        Preguntas3 ListaRespuestas(DateTime desde, DateTime hasta, int Paci)
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

                    String Cargar_Hora2 = "SELECT TOP 1 *  " +
                                          "FROM FIB_ENCUESTA3 " +
                                          "WHERE FechaEncuesta BETWEEN '" + Convert.ToDateTime(desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(hasta).ToString("yyyy-MM-dd") + "' " +
                                          "AND Estado = 'V' " +
                                          "AND IdPaciente = '" + Paci + "' " +
                                          "ORDER BY FechaEncuesta DESC";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.Read() == true)
                    {
                        Preguntas3 P = new Preguntas3();
                        P.Pregunta1 = Lectura_Hora2["Pregunta1"].ToString();
                        P.Pregunta2 = Lectura_Hora2["Pregunta2"].ToString();
                        P.Pregunta3 = Lectura_Hora2["Pregunta3"].ToString();
                        P.Pregunta4 = Lectura_Hora2["Pregunta4"].ToString();
                        P.Pregunta5 = Lectura_Hora2["Pregunta5"].ToString();
                        P.Pregunta6 = Lectura_Hora2["Pregunta6"].ToString();
                        P.Pregunta7 = Lectura_Hora2["Pregunta7"].ToString();
                        P.Pregunta8 = Lectura_Hora2["Pregunta8"].ToString();
                        P.Pregunta9 = Lectura_Hora2["Pregunta9"].ToString();
                        P.Pregunta10 = Lectura_Hora2["Pregunta10"].ToString();

                        return P;
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

        List<Preguntas3> IEncuestaSatisfaccion.ExportarResGlobalGeneral(DateTime desde,
                                                                       DateTime hasta)
        {
            //int numero = 0;

            try
            {
                //Obtener Total de Pacientes
                var _totalPaciente = TotalPacientes(desde, hasta);
                if (_totalPaciente == null)
                {
                    //tuca hacer return aqui
                }

                int totalCuenta = _totalPaciente.Count();

                List<Preguntas3> listaTotal = new List<Preguntas3>();

                foreach (var i in _totalPaciente)
                {
                    var listaTemp = ListaRespuestas(desde, hasta, i);
                    if (listaTemp != null)
                    {
                        listaTotal.Add(new Preguntas3
                        {
                            Pregunta1 = listaTemp.Pregunta1,
                            Pregunta2 = listaTemp.Pregunta2,
                            Pregunta3 = listaTemp.Pregunta3,
                            Pregunta4 = listaTemp.Pregunta4,
                            Pregunta5 = listaTemp.Pregunta5,
                            Pregunta6 = listaTemp.Pregunta6,
                            Pregunta7 = listaTemp.Pregunta7,
                            Pregunta8 = listaTemp.Pregunta8,
                            Pregunta9 = listaTemp.Pregunta9,
                            Pregunta10 = listaTemp.Pregunta10
                        });
                    }
                }

                //Datos para Graficos
                //Pregunta 1
                int ContadorP1_R1 = 0;
                int ContadorP1_R2 = 0;
                int ContadorP1_R3 = 0;
                int ContadorP1_R4 = 0;

                //Pregunta 2
                int ContadorP2_R1 = 0;
                int ContadorP2_R2 = 0;
                int ContadorP2_R3 = 0;
                int ContadorP2_R4 = 0;

                //Pregunta 3
                int ContadorP3_R1 = 0;
                int ContadorP3_R2 = 0;
                int ContadorP3_R3 = 0;
                int ContadorP3_R4 = 0;

                //Pregunta 4
                int ContadorP4_R1 = 0;
                int ContadorP4_R2 = 0;
                int ContadorP4_R3 = 0;
                int ContadorP4_R4 = 0;

                //Pregunta 5
                int ContadorP5_R1 = 0;
                int ContadorP5_R2 = 0;
                int ContadorP5_R3 = 0;
                int ContadorP5_R4 = 0;

                //Pregunta 6
                int ContadorP6_R1 = 0;
                int ContadorP6_R2 = 0;
                int ContadorP6_R3 = 0;
                int ContadorP6_R4 = 0;

                //Pregunta 7
                int ContadorP7_R1 = 0;
                int ContadorP7_R2 = 0;
                int ContadorP7_R3 = 0;
                int ContadorP7_R4 = 0;

                //Pregunta 8
                int ContadorP8_R1 = 0;
                int ContadorP8_R2 = 0;
                int ContadorP8_R3 = 0;
                int ContadorP8_R4 = 0;

                //Pregunta 9
                int ContadorP9_R1 = 0;
                int ContadorP9_R2 = 0;
                int ContadorP9_R3 = 0;
                int ContadorP9_R4 = 0;

                //Pregunta 10
                int ContadorP10_R1 = 0;
                int ContadorP10_R2 = 0;
                int ContadorP10_R3 = 0;
                int ContadorP10_R4 = 0;

                foreach (var i2 in listaTotal)
                {
                    //Pregunta 1
                    if (i2.Pregunta1 == "No")
                    {
                        ContadorP1_R1 = ContadorP1_R1 + 1;
                    }
                    else if (i2.Pregunta1 == "Si, parcialmente")
                    {
                        ContadorP1_R2 = ContadorP1_R2 + 1;
                    }
                    else if (i2.Pregunta1 == "Si, en general")
                    {
                        ContadorP1_R3 = ContadorP1_R3 + 1;
                    }
                    else
                    {
                        ContadorP1_R4 = ContadorP1_R4 + 1;
                    }

                    //Pregunta 2
                    if (i2.Pregunta2 == "No")
                    {
                        ContadorP2_R1 = ContadorP2_R1 + 1;
                    }
                    else if (i2.Pregunta2 == "Si, parcialmente")
                    {
                        ContadorP2_R2 = ContadorP2_R2 + 1;
                    }
                    else if (i2.Pregunta2 == "Si, en general")
                    {
                        ContadorP2_R3 = ContadorP2_R3 + 1;
                    }
                    else
                    {
                        ContadorP2_R4 = ContadorP2_R4 + 1;
                    }

                    //Pregunta 3
                    if (i2.Pregunta3 == "Excelente")
                    {
                        ContadorP3_R1 = ContadorP3_R1 + 1;
                    }
                    else if (i2.Pregunta3 == "Bueno")
                    {
                        ContadorP3_R2 = ContadorP3_R2 + 1;
                    }
                    else if (i2.Pregunta3 == "Regular")
                    {
                        ContadorP3_R3 = ContadorP3_R3 + 1;
                    }
                    else
                    {
                        ContadorP3_R4 = ContadorP3_R4 + 1;
                    }

                    //Pregunta 4
                    if (i2.Pregunta4 == "No")
                    {
                        ContadorP4_R1 = ContadorP4_R1 + 1;
                    }
                    else if (i2.Pregunta4 == "Si, parcialmente")
                    {
                        ContadorP4_R2 = ContadorP4_R2 + 1;
                    }
                    else if (i2.Pregunta4 == "Si, en general")
                    {
                        ContadorP4_R3 = ContadorP4_R3 + 1;
                    }
                    else
                    {
                        ContadorP4_R4 = ContadorP4_R4 + 1;
                    }

                    //Pregunta 5
                    if (i2.Pregunta5 == "Totalmente")
                    {
                        ContadorP5_R1 = ContadorP5_R1 + 1;
                    }
                    else if (i2.Pregunta5 == "En general")
                    {
                        ContadorP5_R2 = ContadorP5_R2 + 1;
                    }
                    else if (i2.Pregunta5 == "Parcialmente")
                    {
                        ContadorP5_R3 = ContadorP5_R3 + 1;
                    }
                    else
                    {
                        ContadorP5_R4 = ContadorP5_R4 + 1;
                    }

                    //Pregunta 6
                    if (i2.Pregunta6 == "No")
                    {
                        ContadorP6_R1 = ContadorP6_R1 + 1;
                    }
                    else if (i2.Pregunta6 == "Probablemente no")
                    {
                        ContadorP6_R2 = ContadorP6_R2 + 1;
                    }
                    else if (i2.Pregunta6 == "Probablemente si")
                    {
                        ContadorP6_R3 = ContadorP6_R3 + 1;
                    }
                    else
                    {
                        ContadorP6_R4 = ContadorP6_R4 + 1;
                    }

                    //Pregunta 7
                    if (i2.Pregunta7 == "Muchisimo")
                    {
                        ContadorP7_R1 = ContadorP7_R1 + 1;
                    }
                    else if (i2.Pregunta7 == ">Mucho")
                    {
                        ContadorP7_R2 = ContadorP7_R2 + 1;
                    }
                    else if (i2.Pregunta7 == "Bastante")
                    {
                        ContadorP7_R3 = ContadorP7_R3 + 1;
                    }
                    else
                    {
                        ContadorP7_R4 = ContadorP7_R4 + 1;
                    }

                    //Pregunta 8
                    if (i2.Pregunta8 == "Mucho")
                    {
                        ContadorP8_R1 = ContadorP8_R1 + 1;
                    }
                    else if (i2.Pregunta8 == ">Bastante")
                    {
                        ContadorP8_R2 = ContadorP8_R2 + 1;
                    }
                    else if (i2.Pregunta8 == "Poco")
                    {
                        ContadorP8_R3 = ContadorP8_R3 + 1;
                    }
                    else
                    {
                        ContadorP8_R4 = ContadorP8_R4 + 1;
                    }

                    //Pregunta 9
                    if (i2.Pregunta9 == "Mucho")
                    {
                        ContadorP9_R1 = ContadorP9_R1 + 1;
                    }
                    else if (i2.Pregunta9 == ">Bastante")
                    {
                        ContadorP9_R2 = ContadorP9_R2 + 1;
                    }
                    else if (i2.Pregunta9 == "Poco")
                    {
                        ContadorP9_R3 = ContadorP9_R3 + 1;
                    }
                    else
                    {
                        ContadorP9_R4 = ContadorP9_R4 + 1;
                    }

                    //Pregunta 10
                    if (i2.Pregunta10 == "No")
                    {
                        ContadorP10_R1 = ContadorP10_R1 + 1;
                    }
                    else if (i2.Pregunta10 == ">Probablemente no")
                    {
                        ContadorP10_R2 = ContadorP10_R2 + 1;
                    }
                    else if (i2.Pregunta10 == "Probablemente si")
                    {
                        ContadorP10_R3 = ContadorP10_R3 + 1;
                    }
                    else
                    {
                        ContadorP10_R4 = ContadorP10_R4 + 1;
                    }
                }

                //Generacion de Graficos desde los Datos para Graficos

                //Gracico 1
                string Tit = "AREA ADMINISTRATIVA: ¿Estoy satisfecho(a) con el servicio al cliente de la institucion?";
                string Q1 = "No";
                string Q2 = "Si, parcialmente";
                string Q3 = "Si, en general";
                string Q4 = "Si, totalmente";
                double Por1 = 0;
                double Por2 = 0;
                double Por3 = 0;
                double Por4 = 0;

                Por1 = (ContadorP1_R1 * 100) / totalCuenta;
                Por2 = (ContadorP1_R2 * 100) / totalCuenta;
                Por3 = (ContadorP1_R3 * 100) / totalCuenta;
                Por4 = (ContadorP1_R4 * 100) / totalCuenta;

                var _chartP1 = ChartsgenerateGraphics(ContadorP1_R1, ContadorP1_R2, ContadorP1_R3, ContadorP1_R4,
                                                      Q1, Q2, Q3, Q4, Tit,
                                                      Por1, Por2, Por3, Por4);

                //Grafico 2
                Tit = "AREA ADMINISTRATIVA: ¿El servicio al cliente facilita los procesos terapéuticos de la institución (asignación de citas, cambios, etc)?";
                Q1 = "No";
                Q2 = "Si, parcialmente";
                Q3 = "Si, en general";
                Q4 = "Si, totalmente";
                Por1 = (ContadorP2_R1 * 100) / totalCuenta;
                Por2 = (ContadorP2_R2 * 100) / totalCuenta;
                Por3 = (ContadorP2_R3 * 100) / totalCuenta;
                Por4 = (ContadorP2_R4 * 100) / totalCuenta;

                var _chartP2 = ChartsgenerateGraphics(ContadorP2_R1, ContadorP2_R2, ContadorP2_R3, ContadorP2_R4,
                                                      Q1, Q2, Q3, Q4, Tit,
                                                      Por1, Por2, Por3, Por4);

                //Grafico 3
                Tit = "AREA CLINICA: ¿Cómo calificaría la calidad del servicio que ha recibido?";
                Q1 = "Excelente";
                Q2 = "Bueno";
                Q3 = "Regular";
                Q4 = "Malo";
                Por1 = (ContadorP3_R1 * 100) / totalCuenta;
                Por2 = (ContadorP3_R2 * 100) / totalCuenta;
                Por3 = (ContadorP3_R3 * 100) / totalCuenta;
                Por4 = (ContadorP3_R4 * 100) / totalCuenta;

                var _chartP3 = ChartsgenerateGraphics(ContadorP3_R1, ContadorP3_R2, ContadorP3_R3, ContadorP3_R4,
                                                      Q1, Q2, Q3, Q4, Tit,
                                                      Por1, Por2, Por3, Por4);

                //Grafico 4
                Tit = "AREA CLINICA: ¿Encontró la clase de ayuda que buscaba?";
                Q1 = "No";
                Q2 = "Si, parcialmente";
                Q3 = "Si, en general";
                Q4 = "Si, totalmente";
                Por1 = (ContadorP4_R1 * 100) / totalCuenta;
                Por2 = (ContadorP4_R2 * 100) / totalCuenta;
                Por3 = (ContadorP4_R3 * 100) / totalCuenta;
                Por4 = (ContadorP4_R4 * 100) / totalCuenta;

                var _chartP4 = ChartsgenerateGraphics(ContadorP4_R1, ContadorP4_R2, ContadorP4_R3, ContadorP4_R4,
                                                      Q1, Q2, Q3, Q4, Tit,
                                                      Por1, Por2, Por3, Por4);

                //Grafico 5
                Tit = "AREA CLINICA: ¿En qué medida nuestro programa ha satisfecho sus necesidades?";
                Q1 = "Totalmente";
                Q2 = "En general";
                Q3 = "Parcialmente";
                Q4 = "Ninguna";
                Por1 = (ContadorP5_R1 * 100) / totalCuenta;
                Por2 = (ContadorP5_R2 * 100) / totalCuenta;
                Por3 = (ContadorP5_R3 * 100) / totalCuenta;
                Por4 = (ContadorP5_R4 * 100) / totalCuenta;

                var _chartP5 = ChartsgenerateGraphics(ContadorP5_R1, ContadorP5_R2, ContadorP5_R3, ContadorP5_R4,
                                                      Q1, Q2, Q3, Q4, Tit,
                                                      Por1, Por2, Por3, Por4);

                //Grafico 6
                Tit = "AREA CLINICA: ¿Si un amigo o familiar necesitara ayuda similar, le recomendaría nuestro programa?";
                Q1 = "No";
                Q2 = "Probablemente no";
                Q3 = "Probablemente si";
                Q4 = "Si";
                Por1 = (ContadorP6_R1 * 100) / totalCuenta;
                Por2 = (ContadorP6_R2 * 100) / totalCuenta;
                Por3 = (ContadorP6_R3 * 100) / totalCuenta;
                Por4 = (ContadorP6_R4 * 100) / totalCuenta;

                var _chartP6 = ChartsgenerateGraphics(ContadorP6_R1, ContadorP6_R2, ContadorP6_R3, ContadorP6_R4,
                                                      Q1, Q2, Q3, Q4, Tit,
                                                      Por1, Por2, Por3, Por4);

                //Grafico 7
                Tit = "AREA CLINICA: ¿En qué medida está satisfecho con el tipo de ayuda recibida?";
                Q1 = "Muchisimo";
                Q2 = ">Mucho";
                Q3 = "Bastante";
                Q4 = "Poco";
                Por1 = (ContadorP7_R1 * 100) / totalCuenta;
                Por2 = (ContadorP7_R2 * 100) / totalCuenta;
                Por3 = (ContadorP7_R3 * 100) / totalCuenta;
                Por4 = (ContadorP7_R4 * 100) / totalCuenta;

                var _chartP7 = ChartsgenerateGraphics(ContadorP7_R1, ContadorP7_R2, ContadorP7_R3, ContadorP7_R4,
                                                      Q1, Q2, Q3, Q4, Tit,
                                                      Por1, Por2, Por3, Por4);

                //Grafico 8
                Tit = "AREA CLINICA: ¿Le ha ayudado los servicios que ha recibido a hacer frente más eficazmente sus problemas?";
                Q1 = "Mucho";
                Q2 = ">Bastante";
                Q3 = "Poco";
                Q4 = "Nada";
                Por1 = (ContadorP8_R1 * 100) / totalCuenta;
                Por2 = (ContadorP8_R2 * 100) / totalCuenta;
                Por3 = (ContadorP8_R3 * 100) / totalCuenta;
                Por4 = (ContadorP8_R4 * 100) / totalCuenta;

                var _chartP8 = ChartsgenerateGraphics(ContadorP8_R1, ContadorP8_R2, ContadorP8_R3, ContadorP8_R4,
                                                      Q1, Q2, Q3, Q4, Tit,
                                                      Por1, Por2, Por3, Por4);

                //Grafico 9
                Tit = "AREA CLINICA: ¿En general, en qué medida está satisfecho con el servicio recibido?";
                Q1 = "Mucho";
                Q2 = ">Bastante";
                Q3 = "Poco";
                Q4 = "Nada";
                Por1 = (ContadorP9_R1 * 100) / totalCuenta;
                Por2 = (ContadorP9_R2 * 100) / totalCuenta;
                Por3 = (ContadorP9_R3 * 100) / totalCuenta;
                Por4 = (ContadorP9_R4 * 100) / totalCuenta;

                var _chartP9 = ChartsgenerateGraphics(ContadorP9_R1, ContadorP9_R2, ContadorP9_R3, ContadorP9_R4,
                                                      Q1, Q2, Q3, Q4, Tit,
                                                      Por1, Por2, Por3, Por4);

                //Grafico 10
                Tit = "AREA CLINICA: ¿Si tuviera que buscar ayuda otra vez volvería a acudir a nuestro programa?";
                Q1 = "No";
                Q2 = ">Probablemente no";
                Q3 = "Probablemente si";
                Q4 = "Si";
                Por1 = (ContadorP10_R1 * 100) / totalCuenta;
                Por2 = (ContadorP10_R2 * 100) / totalCuenta;
                Por3 = (ContadorP10_R3 * 100) / totalCuenta;
                Por4 = (ContadorP10_R4 * 100) / totalCuenta;

                var _chartP10 = ChartsgenerateGraphics(ContadorP10_R1, ContadorP10_R2, ContadorP10_R3, ContadorP10_R4,
                                                      Q1, Q2, Q3, Q4, Tit,
                                                      Por1, Por2, Por3, Por4);


                List<Preguntas3> L = new List<Preguntas3>();

                L.Add(new Preguntas3
                {
                    Graph1 = _chartP1,
                    Graph2 = _chartP2,
                    Graph3 = _chartP3,
                    Graph4 = _chartP4,
                    Graph5 = _chartP5,
                    Graph6 = _chartP6,
                    Graph7 = _chartP7,
                    Graph8 = _chartP8,
                    Graph9 = _chartP9,
                    Graph10 = _chartP10,
                    Desde = Convert.ToDateTime(desde),
                    Hasta = Convert.ToDateTime(hasta),
                    Pregunta1 = totalCuenta.ToString() + " Pacientes"
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
