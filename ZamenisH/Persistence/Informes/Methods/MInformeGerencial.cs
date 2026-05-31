using Domain;
using Domain.CXN;
using Domain.Informes;
using Persistence.Informes.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Numerics;

namespace Persistence.Informes.Methods
{
    public class MInformeGerencial : IInformeGerencial
    {
        //OBTWNWE lista de facturas por paciente
        async Task<List<CXN_FACTURA>> IInformeGerencial.getDatosFacturaIndividual(InformeGerencia I)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        await con.OpenAsync();
                    }

                    int monthNumber = getMonthNumber(I.Mes);

                    String Cargar_Hora = "SELECT Fac_Pac, Fac_Fecha, Fac_Cia, Fac_Ase, Fac_Usr_Graba, Fac_Fecha_Des, Fac_Fecha_Has, Homologo, Fac_Num_Fac, Fac_Tipo_Doc " +
                                         "FROM CXN_FACTURA " +
                                         "WHERE Fac_Fecha <= @param1 " +
                                         "AND Fac_Cia = @param3 " +
                                         "AND Fac_Ase = @param4 " +
                                         "AND Fac_Estado = @param5 " +
                                         "AND Fac_Pac = @param6 " +
                                         "ORDER BY Fac_Fecha ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.CommandTimeout = int.MaxValue;

                        DateTime d = new DateTime(Convert.ToInt32(I.Año), monthNumber, 1, 00, 00, 000);

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

                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        Carga_Command.Parameters.AddWithValue("@param3", I.Compañia);
                        Carga_Command.Parameters.AddWithValue("@param4", I.Aseguradora);
                        Carga_Command.Parameters.AddWithValue("@param5", "F");
                        Carga_Command.Parameters.AddWithValue("@param6", I.IdPaciente);

                        using (SqlDataReader Lectura_Hora = await (Carga_Command.ExecuteReaderAsync()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_FACTURA> F = new List<CXN_FACTURA>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    F.Add(new CXN_FACTURA
                                    {
                                        Fac_Ase = Convert.ToInt32(Lectura_Hora["Fac_Ase"]),
                                        Fac_Cia = Convert.ToInt32(Lectura_Hora["Fac_Cia"]),
                                        Fac_Pac = Convert.ToInt32(Lectura_Hora["Fac_Pac"]),
                                        Fac_Usr_Graba = Lectura_Hora["Fac_Usr_Graba"].ToString(),
                                        Fac_Fecha_Des = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Des"]),
                                        Fac_Fecha_Has = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Has"]),
                                        Fac_Fecha = Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]),
                                        Homologo = Lectura_Hora["Homologo"].ToString(),
                                        Fac_Num_Fac = Convert.ToInt32(Lectura_Hora["Fac_Num_Fac"]),
                                        Fac_Tipo_Doc = Lectura_Hora["Fac_Tipo_Doc"].ToString()
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
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        //OBTWNWE lista de pacientes atendidos en el mes
        async Task<List<int>> IInformeGerencial.getDatosAtendidosMES(InformeGerencia I)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        await con.OpenAsync();
                    }

                    int monthNumber = getMonthNumber(I.Mes);

                    String Cargar_Hora = "SELECT DISTINCT Hor_Pac_Id " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Pac_Fecha_Cita BETWEEN @param1 AND @param2 " +
                                         "AND Hor_Pac_Cia = @param3 " +
                                         "AND Hor_Pac_Ase = @param4 " +
                                         "AND Hor_Estado = @param5 " +
                                         "AND Hor_Pac_Tipo_Serv IN ('CU','MG')";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.CommandTimeout = int.MaxValue;

                        DateTime d = new DateTime(Convert.ToInt32(I.Año), monthNumber, 1, 00, 00, 000);

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
                        Carga_Command.Parameters.AddWithValue("@param3", I.Compañia);
                        Carga_Command.Parameters.AddWithValue("@param4", I.Aseguradora);
                        Carga_Command.Parameters.AddWithValue("@param5", "H");

                        using (SqlDataReader Lectura_Hora = await (Carga_Command.ExecuteReaderAsync()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<int> F = new List<int>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    F.Add(Convert.ToInt32(Lectura_Hora["Hor_Pac_Id"]));
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

        //Obtener lista de pacientes facturados en el mes
        async Task<List<CXN_FACTURA>> IInformeGerencial.getDatosFactura(InformeGerencia I)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        await con.OpenAsync();
                    }

                    int monthNumber = getMonthNumber(I.Mes);

                    String Cargar_Hora = "SELECT Fac_Pac, Fac_Fecha, Fac_Cia, Fac_Ase, Fac_Usr_Graba, Fac_Fecha_Des, Fac_Fecha_Has, Homologo, Fac_Num_Fac, Fac_Tipo_Doc " +
                                         "FROM CXN_FACTURA " +
                                         "WHERE Fac_Fecha BETWEEN @param1 AND @param2 " +
                                         "AND Fac_Cia = @param3 " +
                                         "AND Fac_Ase = @param4 " +
                                         "AND Fac_Estado = @param5";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.CommandTimeout = int.MaxValue;

                        DateTime d = new DateTime(Convert.ToInt32(I.Año), monthNumber, 1, 00, 00, 000);

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
                        Carga_Command.Parameters.AddWithValue("@param3", I.Compañia);
                        Carga_Command.Parameters.AddWithValue("@param4", I.Aseguradora);
                        Carga_Command.Parameters.AddWithValue("@param5", "F");

                        using (SqlDataReader Lectura_Hora = await (Carga_Command.ExecuteReaderAsync()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_FACTURA> F = new List<CXN_FACTURA>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    F.Add(new CXN_FACTURA { 
                                        Fac_Ase = Convert.ToInt32(Lectura_Hora["Fac_Ase"]),
                                        Fac_Cia = Convert.ToInt32(Lectura_Hora["Fac_Cia"]),
                                        Fac_Pac = Convert.ToInt32(Lectura_Hora["Fac_Pac"]),
                                        Fac_Usr_Graba = Lectura_Hora["Fac_Usr_Graba"].ToString(),
                                        Fac_Fecha_Des = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Des"]),
                                        Fac_Fecha_Has = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Has"]),
                                        Fac_Fecha = Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]),
                                        Homologo = Lectura_Hora["Homologo"].ToString(),
                                        Fac_Num_Fac = Convert.ToInt32(Lectura_Hora["Fac_Num_Fac"]),
                                        Fac_Tipo_Doc = Lectura_Hora["Fac_Tipo_Doc"].ToString()
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
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        //Obtener datos de factura por paciente hisotiroc
        async Task<List<CXN_FACTURA>> IInformeGerencial.getDatosFacturaXPaciente(InformeGerencia I)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        await con.OpenAsync();
                    }

                    int monthNumber = getMonthNumber(I.Mes);

                    String Cargar_Hora = "SELECT P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS NOMBRE, P.Pac_TipoId + ' ' + P.Pac_IdNum AS IDD, " +
                                                "F.Fac_Num_Fac, F.Homologo, F.Fac_Tipo_Doc, F.Fac_Fecha, F.Fac_Fecha_Des, F.Fac_Fecha_Has, F.Fac_Usr_Graba " + 
                                         "FROM CXN_FACTURA F " +
                                         "INNER JOIN CXN_PACIENTES P ON F.Fac_Pac = P.Pac_Id " +
                                         "WHERE F.Fac_Pac = @param1 " +
                                         "AND F.Fac_Cia = @param2 " +
                                         "AND F.Fac_Ase = @param3 " +
                                         "AND F.Fac_Estado = @param4 " +
                                         "AND F.Fac_Fecha <= @param5 " +
                                         "ORDER BY F.Fac_Fecha ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.CommandTimeout = int.MaxValue;

                        DateTime d = new DateTime(Convert.ToInt32(I.Año), monthNumber, 1, 00, 00, 000);

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

                        I.FechaCorte = new DateTime(d.Year, monthNumber, Days, 00, 00, 000);

                        Carga_Command.Parameters.AddWithValue("@param1", I.IdPaciente);
                        Carga_Command.Parameters.AddWithValue("@param2", I.Compañia);
                        Carga_Command.Parameters.AddWithValue("@param3", I.Aseguradora);
                        Carga_Command.Parameters.AddWithValue("@param4", "F");
                        Carga_Command.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = Convert.ToDateTime(I.FechaCorte);

                        using (SqlDataReader Lectura_Hora = await (Carga_Command.ExecuteReaderAsync()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_FACTURA> F = new List<CXN_FACTURA>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    I.FacturaZamenis = Convert.ToInt32(Lectura_Hora["Fac_Num_Fac"]);
                                    I.TipoDoc = Lectura_Hora["Fac_Tipo_Doc"].ToString();
                                    string pat = getPatologia(I);

                                    F.Add(new CXN_FACTURA
                                    {
                                        Homologo = Lectura_Hora["Homologo"].ToString(),
                                        Fac_Num_Fac = Convert.ToInt32(Lectura_Hora["Fac_Num_Fac"]),
                                        Fac_Tipo_Doc = Lectura_Hora["Fac_Tipo_Doc"].ToString(),
                                        Fac_Fecha = Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]),
                                        Fac_Fecha_Des = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Des"]),
                                        Fac_Fecha_Has = Convert.ToDateTime(Lectura_Hora["Fac_Fecha_Has"]),
                                        Fac_Observa = Lectura_Hora["NOMBRE"].ToString(),
                                        Fac_Res = Lectura_Hora["IDD"].ToString(),
                                        Fac_Usr_Graba = Lectura_Hora["Fac_Usr_Graba"].ToString(),
                                        QRCufe = pat
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
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        public static string getPatologia(InformeGerencia I)
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

                    String Cargar_Hora = "SELECT Car_Fecha, Car_Detalle " +
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Pac = @param1 " +
                                         "AND Car_Cia = @param2 " +
                                         "AND Car_Ase = @param3 " +
                                         "AND Car_Estado = @param4 " +
                                         "AND Car_Factura = @param5 " +
                                         "AND Car_Tipo_Doc = @param6 " +
                                         "AND Car_Tipo = @param7 " +
                                         "ORDER BY Car_Fecha ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.CommandTimeout = int.MaxValue;

                        Carga_Command.Parameters.AddWithValue("@param1", I.IdPaciente);
                        Carga_Command.Parameters.AddWithValue("@param2", I.Compañia);
                        Carga_Command.Parameters.AddWithValue("@param3", I.Aseguradora);
                        Carga_Command.Parameters.AddWithValue("@param4", "F");
                        Carga_Command.Parameters.AddWithValue("@param5", I.FacturaZamenis);
                        Carga_Command.Parameters.AddWithValue("@param6", I.TipoDoc);
                        Carga_Command.Parameters.AddWithValue("@param7", "Nota");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Lectura_Hora["Car_Detalle"].ToString();
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "getPatologia()", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "";
            }
        }

        async Task<int> IInformeGerencial.sumarValorXFactura(InformeGerencia I)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                       await con.OpenAsync();
                    }

                    String Cargar_Hora = "SELECT SUM(Car_Val_Tot) AS TOTAL " +
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Pac = @param1 " +
                                         "AND Car_Cia = @param2 " +
                                         "AND Car_Ase = @param3 " +
                                         "AND Car_Estado = @param4 " +
                                         "AND Car_Factura = @param5 " +
                                         "AND Car_Tipo_Doc = @param6";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.CommandTimeout = int.MaxValue;

                        Carga_Command.Parameters.AddWithValue("@param1", I.IdPaciente);
                        Carga_Command.Parameters.AddWithValue("@param2", I.Compañia);
                        Carga_Command.Parameters.AddWithValue("@param3", I.Aseguradora);
                        Carga_Command.Parameters.AddWithValue("@param4", "F");
                        Carga_Command.Parameters.AddWithValue("@param5", I.FacturaZamenis);
                        Carga_Command.Parameters.AddWithValue("@param6", I.TipoDoc);

                        using (SqlDataReader Lectura_Hora = await (Carga_Command.ExecuteReaderAsync()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                int val = Convert.ToInt32(Lectura_Hora["TOTAL"]);
                                return val;
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

        async Task<BigInteger> IInformeGerencial.sumarValorGlobal(InformeGerencia I)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        await con.OpenAsync();
                    }

                    String Cargar_Hora = "SELECT SUM(Car_Val_Tot) AS TOTAL " +
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Cia = @param1 " +
                                         "AND Car_Ase = @param2 " +
                                         "AND Car_Estado = @param3";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.CommandTimeout = int.MaxValue;

                        Carga_Command.Parameters.AddWithValue("@param1", I.Compañia);
                        Carga_Command.Parameters.AddWithValue("@param2", I.Aseguradora);
                        Carga_Command.Parameters.AddWithValue("@param3", "F");

                        using (SqlDataReader Lectura_Hora = await (Carga_Command.ExecuteReaderAsync()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                BigInteger val = Convert.ToUInt32(Lectura_Hora["TOTAL"]);
                                return val;
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

        async Task<int> IInformeGerencial.sumarValorTotalFacturasXPac(InformeGerencia I)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        await con.OpenAsync();
                    }

                    String Cargar_Hora = "SELECT SUM(C.Car_Val_Tot) AS TOTAL " +
                                         "FROM CXN_CARGOS C " +
                                         "INNER JOIN CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac " + 
                                         "AND C.Car_Tipo_Doc = F.Fac_Tipo_Doc " + 
                                         "WHERE C.Car_Pac = @param1 " +
                                         "AND C.Car_Cia = @param2 " +
                                         "AND C.Car_Ase = @param3 " +
                                         "AND C.Car_Estado = @param4 " +
                                         "AND F.Fac_Fecha <= @param5";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.CommandTimeout = int.MaxValue;

                        int monthNumber = getMonthNumber(I.Mes);

                        DateTime d = new DateTime(Convert.ToInt32(I.Año), monthNumber, 1, 00, 00, 000);

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

                        I.FechaCorte = new DateTime(d.Year, monthNumber, Days, 00, 00, 000);

                        Carga_Command.Parameters.AddWithValue("@param1", I.IdPaciente);
                        Carga_Command.Parameters.AddWithValue("@param2", I.Compañia);
                        Carga_Command.Parameters.AddWithValue("@param3", I.Aseguradora);
                        Carga_Command.Parameters.AddWithValue("@param4", "F");
                        Carga_Command.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = Convert.ToDateTime(I.FechaCorte);

                        using (SqlDataReader Lectura_Hora = await (Carga_Command.ExecuteReaderAsync()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["TOTAL"]);
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

        int getMonthNumber(string Month)
        {
            int resut = 0;

            switch (Month)
            {
                case "Enero":
                    resut = 1;
                    break;
                case "Febrero":
                    resut = 2;
                    break;
                case "Marzo":
                    resut = 3;
                    break;
                case "Abril":
                    resut = 4;
                    break;
                case "Mayo":
                    resut = 5;
                    break;
                case "Junio":
                    resut = 6;
                    break;
                case "Julio":
                    resut = 7;
                    break;
                case "Agosto":
                    resut = 8;
                    break;
                case "Septiembre":
                    resut = 9;
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
            }

            return resut;
        }
    }
}
