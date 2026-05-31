using Domain;
using Persistence.Informes.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

namespace Persistence.Informes.Methods
{
    public class MInformeEnfermeria : IInformeEnfermeria
    {
        private static readonly IBodegas repoBod = new MBodegas();

        int IInformeEnfermeria.getCantidadAtendidos(int Bodega, string Mes, string Año)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    int monthNumber = getMonthNumber(Mes);
                    string Prof = repoBod.getDatosCode(Bodega).Bod_Usuario;

                    String Cargar_Hora = "SELECT COUNT(*) AS Cantidad " +
                                         "FROM CXN_ESTADISTICAS " +
                                         "WHERE Est_Fecha BETWEEN @param1 AND @param2 " +
                                         "AND Est_Usuario = @param3";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {                                               
                        DateTime d = new DateTime(Convert.ToInt32(Año), monthNumber, 1, 00, 00, 000);
                        
                        int Days = 0;

                        if (d.Month == 1 || d.Month == 3 || d.Month == 5 || d.Month == 7 || d.Month == 8 || d.Month == 10 || d.Month == 12)
                        {
                            Days = 31;
                        }
                        if (d.Month == 2)
                        {
                            Days = 28;
                        }
                        if (d.Month == 4 || d.Month == 6 || d.Month == 9 || d.Month == 11)
                        {
                            Days = 30;
                        }

                        DateTime h = new DateTime(d.Year, monthNumber, Days, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        Carga_Command.Parameters.AddWithValue("@param3", Prof);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Cantidad"]);
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
        int IInformeEnfermeria.getCantidadAtendidosMG(int Bodega, string Mes, string Año)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    int monthNumber = getMonthNumber(Mes);

                    String Cargar_Hora = "SELECT COUNT(*) AS Cantidad " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Pac_Fecha_Cita BETWEEN @param1 AND @param2 " +
                                         "AND Hor_Estado = @param3 " +
                                         "AND Hor_Pac_Bod = @param4";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime d = new DateTime(Convert.ToInt32(Año), monthNumber, 1, 00, 00, 000);

                        int Days = 0;

                        if (d.Month == 1 || d.Month == 3 || d.Month == 5 || d.Month == 7 || d.Month == 8 || d.Month == 10 || d.Month == 12)
                        {
                            Days = 31;
                        }
                        if (d.Month == 2)
                        {
                            Days = 28;
                        }
                        if (d.Month == 4 || d.Month == 6 || d.Month == 9 || d.Month == 11)
                        {
                            Days = 30;
                        }

                        DateTime h = new DateTime(d.Year, d.Month, Days, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        Carga_Command.Parameters.AddWithValue("@param3", "H");
                        Carga_Command.Parameters.AddWithValue("@param4", Bodega);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Cantidad"]);
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
        int IInformeEnfermeria.getCantPacsForYear(string Month, string Year, string TSERV)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    int monthNumber = getMonthNumber(Month);

                    String Cargar_Hora = "SELECT COUNT(DISTINCT(Hor_Pac_Id)) AS Cantidad " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Pac_Fecha_Cita BETWEEN @param1 AND @param2 " +
                                         "AND Hor_Estado = @param3 " +
                                         "AND Hor_Pac_Tipo_Serv = @param4";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime d = new DateTime(Convert.ToInt32(Year), 1, 1, 00, 00, 000);

                        int Days = 0;

                        if (monthNumber == 1 || monthNumber == 3 || monthNumber == 5 || monthNumber == 7 || monthNumber == 8 || monthNumber == 10 || monthNumber == 12)
                        {
                            Days = 31;
                        }
                        if (monthNumber == 2)
                        {
                            Days = 28;
                        }
                        if (monthNumber == 4 || monthNumber == 6 || monthNumber == 9 || monthNumber == 11)
                        {
                            Days = 30;
                        }

                         DateTime h = new DateTime(d.Year, monthNumber, Days, 00, 00, 000);
                        //DateTime h = new DateTime(d.Year, 12, 31, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        Carga_Command.Parameters.AddWithValue("@param3", "H");
                        Carga_Command.Parameters.AddWithValue("@param4", TSERV);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Cantidad"]);
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
        int IInformeEnfermeria.getCantPacsForMonth(string Month, string Year, string TSERV)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    int monthNumber = getMonthNumber(Month);

                    String Cargar_Hora = "SELECT COUNT(DISTINCT(Hor_Pac_Id)) AS Cantidad " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Pac_Fecha_Cita BETWEEN @param1 AND @param2 " +
                                         "AND Hor_Estado = @param3 " +
                                         "AND Hor_Pac_Tipo_Serv = @param4";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime d = new DateTime(Convert.ToInt32(Year), monthNumber, 1, 00, 00, 000);

                        int Days = 0;

                        if (d.Month == 1 || d.Month == 3 || d.Month == 5 || d.Month == 7 || d.Month == 8 || d.Month == 10 || d.Month == 12)
                        {
                            Days = 31;
                        }
                        if (d.Month == 2)
                        {
                            Days = 28;
                        }
                        if (d.Month == 4 || d.Month == 6 || d.Month == 9 || d.Month == 11)
                        {
                            Days = 30;
                        }

                        DateTime h = new DateTime(d.Year, d.Month, Days, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        Carga_Command.Parameters.AddWithValue("@param3", "H");
                        Carga_Command.Parameters.AddWithValue("@param4", TSERV);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Cantidad"]);
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
        int IInformeEnfermeria.getCantidadAtendidos2(string Mes, string Año, string TSERV)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    int monthNumber = getMonthNumber(Mes);

                    String Cargar_Hora = "SELECT COUNT(*) AS Cantidad " +
                                         "FROM CXN_ESTADISTICAS " +
                                         "WHERE Est_Fecha BETWEEN @param1 AND @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime d = new DateTime(Convert.ToInt32(Año), monthNumber, 1, 00, 00, 000);

                        int Days = 0;

                        if (d.Month == 1 || d.Month == 3 || d.Month == 5 || d.Month == 7 || d.Month == 8 || d.Month == 10 || d.Month == 12)
                        {
                            Days = 31;
                        }
                        if (d.Month == 2)
                        {
                            Days = 28;
                        }
                        if (d.Month == 4 || d.Month == 6 || d.Month == 9 || d.Month == 11)
                        {
                            Days = 30;
                        }

                        DateTime h = new DateTime(d.Year, monthNumber, Days, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        //Carga_Command.Parameters.AddWithValue("@param3", "H");
                        //Carga_Command.Parameters.AddWithValue("@param4", TSERV);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Cantidad"]);
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
        int IInformeEnfermeria.getCantidadPatologia2(string Mes, string Año, string Patologia)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    int monthNumber = getMonthNumber(Mes);

                    String Cargar_Hora = "SELECT COUNT(*) AS CANTIDAD " +
                                         "FROM CXN_ESTADISTICAS " +
                                         "WHERE Est_Fecha BETWEEN @param1 AND @param2 " +
                                         "AND Est_Patologia LIKE '%" + Patologia + "%'";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime d = new DateTime(Convert.ToInt32(Año), monthNumber, 1, 00, 00, 000);

                        int Days = 0;

                        if (d.Month == 1 || d.Month == 3 || d.Month == 5 || d.Month == 7 || d.Month == 8 || d.Month == 10 || d.Month == 12)
                        {
                            Days = 31;
                        }
                        if (d.Month == 2)
                        {
                            Days = 28;
                        }
                        if (d.Month == 4 || d.Month == 6 || d.Month == 9 || d.Month == 11)
                        {
                            Days = 30;
                        }

                        DateTime h = new DateTime(d.Year, monthNumber, Days, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Cantidad"]);
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
        int IInformeEnfermeria.getCantidadPatologia(string Mes, string Año, string Patologia)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    int monthNumber = getMonthNumber(Mes);

                    String Cargar_Hora = "SELECT COUNT(*) AS CANTIDAD " +
                                         "FROM CXN_ESTADISTICAS " +
                                         "WHERE Est_Fecha BETWEEN @param1 AND @param2 " +
                                         "AND Est_Patologia = @param3";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime d = new DateTime(Convert.ToInt32(Año), monthNumber, 1, 00, 00, 000);

                        int Days = 0;

                        if (d.Month == 1 || d.Month == 3 || d.Month == 5 || d.Month == 7 || d.Month == 8 || d.Month == 10 || d.Month == 12)
                        {
                            Days = 31;
                        }
                        if (d.Month == 2)
                        {
                            Days = 28;
                        }
                        if (d.Month == 4 || d.Month == 6 || d.Month == 9 || d.Month == 11)
                        {
                            Days = 30;
                        }

                        DateTime h = new DateTime(d.Year, monthNumber, Days, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        Carga_Command.Parameters.AddWithValue("@param3", Patologia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Cantidad"]);
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
        int IInformeEnfermeria.getCantidadEdad(string Mes, string Año, string Edad)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    int monthNumber = getMonthNumber(Mes);

                    String Cargar_Hora = "SELECT COUNT(*) AS CANTIDAD " +
                                         "FROM CXN_ESTADISTICAS " +
                                         "WHERE Est_Fecha BETWEEN @param1 AND @param2 " +
                                         "AND Est_Edad = @param3";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime d = new DateTime(Convert.ToInt32(Año), monthNumber, 1, 00, 00, 000);

                        int Days = 0;

                        if (d.Month == 1 || d.Month == 3 || d.Month == 5 || d.Month == 7 || d.Month == 8 || d.Month == 10 || d.Month == 12)
                        {
                            Days = 31;
                        }
                        if (d.Month == 2)
                        {
                            Days = 28;
                        }
                        if (d.Month == 4 || d.Month == 6 || d.Month == 9 || d.Month == 11)
                        {
                            Days = 30;
                        }

                        DateTime h = new DateTime(d.Year, d.Month, Days, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        Carga_Command.Parameters.AddWithValue("@param3", Edad);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Cantidad"]);
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
        List<int> IInformeEnfermeria.getEnfermerosYear(string Año)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT DISTINCT H.Hor_Pac_Bod " +
                                         "FROM CXN_ESTADISTICAS E " +
                                         "INNER JOIN CXN_HORARIO H ON E.Est_Admision = H.Hor_Id " +
                                         "WHERE E.Est_Fecha BETWEEN @param1 AND @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime d = new DateTime(Convert.ToInt32(Año), 1, 1, 00, 00, 000);
                        DateTime h = new DateTime(Convert.ToInt32(Año), 12, 31, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<int> lT = new List<int>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    lT.Add(Convert.ToInt32(Lectura_Hora["Hor_Pac_Bod"]));
                                }

                                return lT;
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
        List<int> IInformeEnfermeria.getMedicosYear(string Año)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT DISTINCT Hor_Pac_Bod " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Pac_Fecha_Cita BETWEEN @param1 AND @param2 " +
                                         "AND Hor_Pac_Tipo_Serv = @param3";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime d = new DateTime(Convert.ToInt32(Año), 1, 1, 00, 00, 000);
                        DateTime h = new DateTime(Convert.ToInt32(Año), 12, 31, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        Carga_Command.Parameters.AddWithValue("@param3", "MG");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<int> lT = new List<int>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    lT.Add(Convert.ToInt32(Lectura_Hora["Hor_Pac_Bod"]));
                                }

                                return lT;
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

        int getMonthNumber(string Month)
        {
            int resut = 0;

            switch (Month)
            {
                case "Enero":
                    resut = 01;
                    break;
                case "Febrero":
                    resut = 02;
                    break;
                case "Marzo":
                    resut = 03;
                    break;
                case "Abril":
                    resut = 04;
                    break;
                case "Mayo":
                    resut = 05;
                    break;
                case "Junio":
                    resut = 06;
                    break;
                case "Julio":
                    resut = 07;
                    break;
                case "Agosto":
                    resut = 08;
                    break;
                case "Septiembre":
                    resut = 09;
                    break;
                case "Octubre":
                    resut = 10;
                    break;
                case "Noviembre":
                    resut = 11;
                    break;
                case "Diciembre":
                    resut = 12;
                    break;

                case "ENERO":
                    resut = 01;
                    break;
                case "FEBRERO":
                    resut = 02;
                    break;
                case "MARZO":
                    resut = 03;
                    break;
                case "ABRIL":
                    resut = 04;
                    break;
                case "MAYO":
                    resut = 05;
                    break;
                case "JUNIO":
                    resut = 06;
                    break;
                case "JULIO":
                    resut = 07;
                    break;
                case "AGOSTO":
                    resut = 08;
                    break;
                case "SEPTIEMBRE":
                    resut = 09;
                    break;
                case "OCTUBRE":
                    resut = 10;
                    break;
                case "NOVIEMBRE":
                    resut = 11;
                    break;
                case "DICIEMBRE":
                    resut = 12;
                    break;
            }

            return resut;
        }


    }
}
