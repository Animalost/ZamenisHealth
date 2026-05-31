using Domain;
using Domain.Fibromialgia;
using Persistence.Fibromialgia.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;

namespace Persistence.Fibromialgia.Metodos
{
    public class MInformeMensualFibromialgia : IInformeMensualFibromialgia
    {
        int IInformeMensualFibromialgia.getCantidad(string TipoBod, string Mes, int Año)
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

                    String Cargar_Hora = "SELECT COUNT(*) AS Valor " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Estado = @param1 " +
                                         "AND Hor_Pac_Tipo_Serv = @param2 " +
                                         "AND Hor_Pac_Fecha_Cita BETWEEN @param3 AND @param4";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime Desde = new DateTime(Año, getMonthNumbre(Mes), 01);
                        DateTime Hasta = new DateTime(Año, getMonthNumbre(Mes), getLastDayMonth(Mes));

                        Carga_Command.Parameters.AddWithValue("@param1", "H");
                        Carga_Command.Parameters.AddWithValue("@param2", TipoBod);
                        Carga_Command.Parameters.AddWithValue("@param3", Convert.ToDateTime(Desde).Date);
                        Carga_Command.Parameters.AddWithValue("@param4", Convert.ToDateTime(Hasta).Date);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Valor"]);
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
        (string Profesional, int Cantidad) IInformeMensualFibromialgia.getCantidadByProfesional(string TipoBod, string Mes, int Año, int Med)
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

                    String Cargar_Hora = "SELECT B.Bod_Responsable " +
                                         "FROM CXN_HORARIO H " +
                                         "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                         "WHERE H.Hor_Estado = @param1 " +
                                         "AND H.Hor_Pac_Tipo_Serv = @param2 " +
                                         "AND H.Hor_Pac_Fecha_Cita BETWEEN @param3 AND @param4 " +
                                         "AND H.Hor_Pac_Bod = @param5";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime Desde = new DateTime(Año, getMonthNumbre(Mes), 01);
                        DateTime Hasta = new DateTime(Año, getMonthNumbre(Mes), getLastDayMonth(Mes));

                        Carga_Command.Parameters.AddWithValue("@param1", "H");
                        Carga_Command.Parameters.AddWithValue("@param2", TipoBod);
                        Carga_Command.Parameters.AddWithValue("@param3", Convert.ToDateTime(Desde).Date);
                        Carga_Command.Parameters.AddWithValue("@param4", Convert.ToDateTime(Hasta).Date);
                        Carga_Command.Parameters.AddWithValue("@param5", Med);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                int canTemp = 1;
                                string R = "";

                                while (Lectura_Hora.Read() == true)
                                {
                                    R = Lectura_Hora["Bod_Responsable"].ToString();
                                    canTemp++;
                                }
                                
                                int C = canTemp;

                                return (R, C);
                            }
                            else
                            {
                                return ("", 0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return ("", 0);
            }
        }
        Byte[] IInformeMensualFibromialgia.GraficoCantidadPorProfesional(List<ClaseReportsFibro> datos)
        {
            try
            {
                Chart chart = new Chart
                {
                    Width = 800,
                    Height = 600,
                    RenderType = RenderType.ImageTag,
                    AntiAliasing = AntiAliasingStyles.All,
                    TextAntiAliasingQuality = TextAntiAliasingQuality.High
                };

                chart.Titles.Add("NUMERO DE ATENCIONES POR PROFESIONAL");
                chart.Titles[0].Font = new System.Drawing.Font("Arial Narrow", 10f);

                chart.ChartAreas.Add("");
                chart.ChartAreas[0].AxisX.Title = "";
                chart.ChartAreas[0].AxisY.Title = "Nivel";
                chart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial Narrow", 10f);
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial Narrow", 10f);
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
                chart.ChartAreas[0].BackColor = Color.White;

                chart.Series.Add("");
                chart.Series[0].ChartType = SeriesChartType.Column;

                foreach (var item in datos)
                {
                    chart.Series[0].Points.AddXY(item.Dato1.ToString().ToUpper().Trim() + "   " + item.Dato2.ToString().Trim(), item.Dato2);
                }

                var ms = new System.IO.MemoryStream();
                chart.SaveImage(ms, ChartImageFormat.Jpeg);
                byte[] pdfBytes = ms.ToArray();

                return pdfBytes;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "BACKEND", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BACKEND" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        int IInformeMensualFibromialgia.getCantidadPacientesMES(string Mes, int Año, bool Total)
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

                    String Cargar_Hora = "SELECT DISTINCT Hor_Pac_Id AS Valor " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Estado = @param1 " +
                                         "AND Hor_Pac_Tipo_Serv IN ('TF','TO','PS') " +
                                         "AND Hor_Pac_Fecha_Cita BETWEEN @param2 AND @param3";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime? Desde = null;
                        DateTime? Hasta = null;

                        if (Total == false)
                        {
                            Desde = new DateTime(Año, getMonthNumbre(Mes), 01);
                            Hasta = new DateTime(Año, getMonthNumbre(Mes), getLastDayMonth(Mes));
                        }
                        else
                        {
                            Desde = new DateTime(Año, 01, 01);
                            Hasta = new DateTime(Año, 12, 31);
                        }                        

                        Carga_Command.Parameters.AddWithValue("@param1", "H");
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(Desde).Date);
                        Carga_Command.Parameters.AddWithValue("@param3", Convert.ToDateTime(Hasta).Date);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                int canTemp = 1;

                                while (Lectura_Hora.Read() == true)
                                {
                                    canTemp++;
                                }

                                return canTemp;
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
        List<(string Codigo, string Servicio)> IInformeMensualFibromialgia.getServices()
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

                    String Cargar_Hora = "SELECT Con_Id_Serv, Con_Nombre " +
                                         "FROM CXN_CONVENIOS " +
                                         "WHERE Con_Tipo_Serv IN ('TF','TO','PS')";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<(string Codigo, string Servicio)> L = new List<(string Codigo, string Servicio)>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add((Lectura_Hora["Con_Id_Serv"].ToString().Trim(), Lectura_Hora["Con_Nombre"].ToString().Trim()));
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        int IInformeMensualFibromialgia.getCantidadByServ(string Codigo, string Mes, int Año)
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

                    String Cargar_Hora = "SELECT COUNT(*) AS Valor " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Pac_Cup = @param1 " +
                                         "AND Hor_Estado = @param2 " +
                                         "AND Hor_Pac_Fecha_Cita BETWEEN @param3 AND @param4";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime Desde = new DateTime(Año, getMonthNumbre(Mes), 01);
                        DateTime Hasta = new DateTime(Año, getMonthNumbre(Mes), getLastDayMonth(Mes));

                        Carga_Command.Parameters.AddWithValue("@param1", Codigo);
                        Carga_Command.Parameters.AddWithValue("@param2", "H");
                        Carga_Command.Parameters.AddWithValue("@param3", Convert.ToDateTime(Desde).Date);
                        Carga_Command.Parameters.AddWithValue("@param4", Convert.ToDateTime(Hasta).Date);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Valor"]);
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

        int getMonthNumbre(string Month)
        {
            switch (Month)
            {
                case "ENERO":
                    return 1;
                case "FEBRERO":
                    return 2;
                case "MARZO":
                    return 3;
                case "ABRIL":
                    return 4;
                case "MAYO":
                    return 5;
                case "JUNIO":
                    return 6;
                case "JULIO":
                    return 7;
                case "AGOSTO":
                    return 8;
                case "SEPTIEMBRE":
                    return 9;
                case "OCTUBRE":
                    return 10;
                case "NOVIEMBRE":
                    return 11;
                case "DICIEMBRE":
                    return 12;

                case "Enero":
                    return 1;
                case "Febrero":
                    return 2;
                case "Marzo":
                    return 3;
                case "Abril":
                    return 4;
                case "Mayo":
                    return 5;
                case "Junio":
                    return 6;
                case "Julio":
                    return 7;
                case "Agosto":
                    return 8;
                case "Septiembre":
                    return 9;
                case "Octubre":
                    return 10;
                case "Noviembre":
                    return 11;
                case "Diciembre":
                    return 12;

                default:
                    return 0;
            }
        }
        int getLastDayMonth(string Month)
        {
            switch (Month)
            {
                case "ENERO":
                    return 31;
                case "FEBRERO":
                    return 28;
                case "MARZO":
                    return 31;
                case "ABRIL":
                    return 30;
                case "MAYO":
                    return 31;
                case "JUNIO":
                    return 30;
                case "JULIO":
                    return 31;
                case "AGOSTO":
                    return 31;
                case "SEPTIEMBRE":
                    return 30;
                case "OCTUBRE":
                    return 31;
                case "NOVIEMBRE":
                    return 30;
                case "DICIEMBRE":
                    return 31;

                case "Enero":
                    return 31;
                case "Febrero":
                    return 28;
                case "Marzo":
                    return 31;
                case "Abril":
                    return 30;
                case "Mayo":
                    return 31;
                case "Junio":
                    return 30;
                case "Julio":
                    return 31;
                case "Agosto":
                    return 31;
                case "Septiembre":
                    return 30;
                case "Octubre":
                    return 31;
                case "Noviembre":
                    return 30;
                case "Diciembre":
                    return 31;

                default:
                    return 0;
            }
        }
    }
}
