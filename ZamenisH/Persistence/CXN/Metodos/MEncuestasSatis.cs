using Domain.CXN;
using Domain;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace Persistence.CXN.Metodos
{
    public class MEncuestasSatis : IEncuestasSatis
    {
        private static readonly IGenerales repoGen = new MGenerales();

        bool IEncuestasSatis.getCantEncuestaCU(string Service, string Mes, int Año)
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

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_CONFENCUESTA " +
                                         "WHERE Servicio = @param1 " +
                                         "AND Mes = @param2 " +
                                         "AND Año = @param3";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Service);
                        Carga_Command.Parameters.AddWithValue("@param2", Mes);
                        Carga_Command.Parameters.AddWithValue("@param3", Año);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {                               
                                return (Convert.ToInt32(Lectura_Hora["Actual"]) < Convert.ToInt32(Lectura_Hora["Total"]) ? true : false);
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

        bool IEncuestasSatis.getCantCitas(int Paciente, CXN_CONFENCUESTA C)
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

                    DateTime Hoy = DateTime.Now.Date;

                    String Cargar_Hora = "SELECT COUNT(*) AS Cantidad " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Pac_Id = @param1 " +
                                         "AND Hor_Estado = @param2 " +
                                         "AND Hor_Pac_Tipo_Serv = @param3 " +
                                         "AND Hor_Pac_Fecha_Cita <> @param6 " +
                                         "AND Hor_Pac_Fecha_Cita BETWEEN @param4 AND @param5";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Paciente);
                        Carga_Command.Parameters.AddWithValue("@param2", "H");
                        Carga_Command.Parameters.AddWithValue("@param3", C.Servicio);

                        DateTime d = new DateTime(C.Año, Capitalize(C.Mes), 1, 00, 00, 000);
                        DateTime h = new DateTime(C.Año, Capitalize(C.Mes), 28, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        Carga_Command.Parameters.Add(new SqlParameter("@param6", SqlDbType.DateTime)).Value = Convert.ToDateTime(Hoy);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                int res = Convert.ToInt32(Lectura_Hora["Cantidad"]);
                                return (res >= 1 ? true : false);
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

        bool IEncuestasSatis.getCantEncuestasPaciente(int Paciente, CXN_CONFENCUESTA C)
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

                    String Cargar_Hora = "SELECT COUNT(*) AS Cantidad " +
                                         "FROM CXN_ENCUESTASATIS E " +
                                         "INNER JOIN CXN_HORARIO H ON E.Admision = H.Hor_Id " +
                                         "WHERE H.Hor_Pac_Id = @param1 " +
                                         "AND H.Hor_Pac_Tipo_Serv = @param3 " +
                                         "AND H.Hor_Pac_Fecha_Cita BETWEEN @param4 AND @param5 " +
                                         "AND E.Estado = @param6";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Paciente);
                        Carga_Command.Parameters.AddWithValue("@param3", C.Servicio);

                        DateTime d = new DateTime(C.Año, Capitalize(C.Mes), 1, 00, 00, 000);
                        DateTime h = new DateTime(C.Año, Capitalize(C.Mes), 28, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        Carga_Command.Parameters.AddWithValue("@param6", "H");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return (Convert.ToInt32(Lectura_Hora["Cantidad"]) >= 1 ? false : true);
                            }
                            else
                            {
                                return true;
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

        int Capitalize(string Mes)
        {
            switch (Mes)
            {
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

        bool IEncuestasSatis.GrabarNoRealizacionEncuesta(CXN_ENCUESTASATIS E)
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

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO CXN_ENCUESTASATIS (Admision, " +
                                                          "Estado, " +
                                                          "Observacion) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3)", con);

                    cmd.Parameters.AddWithValue("@param1", E.Admision);
                    cmd.Parameters.AddWithValue("@param2", E.Estado);
                    cmd.Parameters.AddWithValue("@param3", E.Observacion);
                    if (cmd.ExecuteNonQuery() >= 1)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }

        bool IEncuestasSatis.GrabarEncuesta(CXN_ENCUESTASATIS E)
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

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO CXN_ENCUESTASATIS (Admision, " +
                                                          "Estado, " +
                                                          "Observacion, " +
                                                          "P1, " +
                                                          "P2, " +
                                                          "P3, " +
                                                          "P4, " +
                                                          "P5, " +
                                                          "P6, " +
                                                          "Lugar) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3," +
                                                          "@param4," +
                                                          "@param5," +
                                                          "@param6," +
                                                          "@param7," +
                                                          "@param8," +
                                                          "@param9, " +
                                                          "@param10)", con);

                    cmd.Parameters.AddWithValue("@param1", E.Admision);
                    cmd.Parameters.AddWithValue("@param2", E.Estado);
                    cmd.Parameters.AddWithValue("@param3", E.Observacion);
                    cmd.Parameters.AddWithValue("@param4", E.P1);
                    cmd.Parameters.AddWithValue("@param5", E.P2);
                    cmd.Parameters.AddWithValue("@param6", E.P3);
                    cmd.Parameters.AddWithValue("@param7", E.P4);
                    cmd.Parameters.AddWithValue("@param8", E.P5);
                    cmd.Parameters.AddWithValue("@param9", E.P6);
                    cmd.Parameters.AddWithValue("@param10", E.Lugar);

                    if (cmd.ExecuteNonQuery() >= 1)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }

        Dictionary<int, List<CXN_ENCUESTASATIS>> IEncuestasSatis.getEncuestas(DateTime Fecha, string TServ)
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

                    String Cargar_Hora = "SELECT C.Com_Logo, E.Admision, P.Pac_TipoId, P.Pac_IdNum, H.Hor_Pac_Fecha_Cita, H.Hor_Imp_Age, " +
                        "E.Estado, E.Observacion, E.P1, E.P2, E.P3, E.P4, E.P5, E.P6, E.Lugar, B.Bod_Responsable " +
                                         "FROM CXN_ENCUESTASATIS E " +
                                         "INNER JOIN CXN_HORARIO H ON E.Admision = H.Hor_Id " +
                                         "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                         "INNER JOIN CXN_CIA C ON H.Hor_Pac_Cia = C.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                         "WHERE E.Estado = @param1 " +
                                         "AND H.Hor_Pac_Tipo_Serv = @param2 " +
                                         "AND H.Hor_Pac_Fecha_Cita BETWEEN @param3 AND @param4";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", "H");
                        Carga_Command.Parameters.AddWithValue("@param2", TServ);

                        DateTime d = new DateTime(Fecha.Year, Fecha.Month, 1, 00, 00, 000);

                        int Days = 0;

                        if (Fecha.Month == 1 || Fecha.Month == 3 || Fecha.Month == 5 || Fecha.Month == 7 || Fecha.Month == 8 || Fecha.Month == 10 || Fecha.Month == 12)
                        {
                            Days = 31;
                        }
                        if (Fecha.Month == 2)
                        {
                            Days = 28;
                        }
                        if (Fecha.Month == 4 || Fecha.Month == 6 || Fecha.Month == 9 || Fecha.Month == 11)
                        {
                            Days = 30;
                        }

                        DateTime h = new DateTime(Fecha.Year, Fecha.Month, Days, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param3", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                Dictionary<int, List<CXN_ENCUESTASATIS>> D = new Dictionary<int, List<CXN_ENCUESTASATIS>>();                               
                                int Contador = 1;

                                while (Lectura_Hora.Read() == true)
                                {
                                    string lugar = (Lectura_Hora["Lugar"] == DBNull.Value ? "Consultorio" : Lectura_Hora["Lugar"].ToString());

                                    List<CXN_ENCUESTASATIS> Etemp = new List<CXN_ENCUESTASATIS>();

                                    string Logos = Lectura_Hora["Com_Logo"].ToString();
                                    Byte[] bytes = Convert.FromBase64String(Logos);
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = Image.FromStream(stmBLOBData);

                                    Etemp.Add(new CXN_ENCUESTASATIS { 
                                        Admision = Convert.ToInt32(Lectura_Hora["Admision"]),
                                        Documento = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        Paciente = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                        Logo = repoGen.GetBytes(pic.Image),
                                        Estado = Lectura_Hora["Estado"].ToString(),
                                        Observacion = Lectura_Hora["Observacion"].ToString(),
                                        P1t = ValoresRespuesta(Convert.ToInt32(Lectura_Hora["P1"])),
                                        P2t = ValoresRespuesta(Convert.ToInt32(Lectura_Hora["P2"])),
                                        P3t = ValoresRespuesta(Convert.ToInt32(Lectura_Hora["P3"])),
                                        P4t = ValoresRespuesta(Convert.ToInt32(Lectura_Hora["P4"])),
                                        P5t = ValoresRespuesta(Convert.ToInt32(Lectura_Hora["P5"])),
                                        P6t = ValoresRespuesta(Convert.ToInt32(Lectura_Hora["P6"])),
                                        Lugar = lugar,
                                        Bueno = Lectura_Hora["Bod_Responsable"].ToString() //profesional que aplica o atiende la atencion o encuensta
                                    });

                                    D.Add(Contador, Etemp);
                                    Contador++;
                                }

                                return D;
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

        Dictionary<int, List<CXN_ENCUESTASATIS>> IEncuestasSatis.getEncuestas(int Paciente, DateTime Fecha, string TServ)
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

                    String Cargar_Hora = "SELECT C.Com_Logo, E.Admision, P.Pac_TipoId, P.Pac_IdNum, H.Hor_Pac_Fecha_Cita, H.Hor_Imp_Age, " +
                                         "E.Estado, E.Observacion, E.P1, E.P2, E.P3, E.P4, E.P5, E.P6, E.Lugar, B.Bod_Responsable " +
                                         "FROM CXN_ENCUESTASATIS E " +
                                         "INNER JOIN CXN_HORARIO H ON E.Admision = H.Hor_Id " +
                                         "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                         "INNER JOIN CXN_CIA C ON H.Hor_Pac_Cia = C.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                         "WHERE E.Estado = @param1 " +
                                         "AND H.Hor_Pac_Tipo_Serv = @param2 " +
                                         "AND H.Hor_Pac_Fecha_Cita BETWEEN @param3 AND @param4 " +
                                         "AND H.Hor_Pac_Id = @param5";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", "H");
                        Carga_Command.Parameters.AddWithValue("@param2", TServ);

                        DateTime d = new DateTime(Fecha.Year, Fecha.Month, 1, 00, 00, 000);

                        int Days = 0;

                        if (Fecha.Month == 1 || Fecha.Month == 3 || Fecha.Month == 5 || Fecha.Month == 7 || Fecha.Month == 8 || Fecha.Month == 10 || Fecha.Month == 12)
                        {
                            Days = 31;
                        }
                        if (Fecha.Month == 2)
                        {
                            Days = 28;
                        }
                        if (Fecha.Month == 4 || Fecha.Month == 6 || Fecha.Month == 9 || Fecha.Month == 11)
                        {
                            Days = 30;
                        }

                        DateTime h = new DateTime(Fecha.Year, Fecha.Month, Days, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param3", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        Carga_Command.Parameters.AddWithValue("@param5", Paciente);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                Dictionary<int, List<CXN_ENCUESTASATIS>> D = new Dictionary<int, List<CXN_ENCUESTASATIS>>();                               
                                int Contador = 1;

                                while (Lectura_Hora.Read() == true)
                                {
                                    List<CXN_ENCUESTASATIS> Etemp = new List<CXN_ENCUESTASATIS>();

                                    string lugar = (Lectura_Hora["Lugar"] == DBNull.Value ? "Consultorio" : Lectura_Hora["Lugar"].ToString());

                                    string Logos = Lectura_Hora["Com_Logo"].ToString();
                                    Byte[] bytes = Convert.FromBase64String(Logos);
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = Image.FromStream(stmBLOBData);

                                    Etemp.Add(new CXN_ENCUESTASATIS
                                    {
                                        Admision = Convert.ToInt32(Lectura_Hora["Admision"]),
                                        Documento = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        Paciente = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                        Logo = repoGen.GetBytes(pic.Image),
                                        Estado = Lectura_Hora["Estado"].ToString(),
                                        Observacion = Lectura_Hora["Observacion"].ToString(),
                                        P1t = ValoresRespuesta(Convert.ToInt32(Lectura_Hora["P1"])),
                                        P2t = ValoresRespuesta(Convert.ToInt32(Lectura_Hora["P2"])),
                                        P3t = ValoresRespuesta(Convert.ToInt32(Lectura_Hora["P3"])),
                                        P4t = ValoresRespuesta(Convert.ToInt32(Lectura_Hora["P4"])),
                                        P5t = ValoresRespuesta(Convert.ToInt32(Lectura_Hora["P5"])),
                                        P6t = ValoresRespuesta(Convert.ToInt32(Lectura_Hora["P6"])),
                                        Lugar = lugar,
                                        Bueno = Lectura_Hora["Bod_Responsable"].ToString()
                                    });

                                    D.Add(Contador, Etemp);
                                    Contador++;
                                }

                                return D;
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

        string ValoresRespuesta(int Valor)
        {
            string Respuesta = "";

            switch (Valor)
            {
                case 1:
                    Respuesta = "MUY MALA";
                    break;

                case 2:
                    Respuesta = "MALA";
                    break;

                case 3:
                    Respuesta = "REGULAR";
                    break;

                case 4:
                    Respuesta = "BUENA";
                    break;

                case 5:
                    Respuesta = "MUY BUENA";
                    break;

                default:
                    Respuesta = "";
                    break;
            }

            return Respuesta;
        }

        int IEncuestasSatis.getActual(CXN_CONFENCUESTA C)
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

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_CONFENCUESTA " +
                                         "WHERE Mes = @param1 " +
                                         "AND Año = @param2 " +
                                         "AND Servicio = @param3";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", C.Mes);
                        Carga_Command.Parameters.AddWithValue("@param2", C.Año);
                        Carga_Command.Parameters.AddWithValue("@param3", C.Servicio);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Actual"]);
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

        void IEncuestasSatis.updateActual(CXN_CONFENCUESTA C)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_CONFENCUESTA " +
                                      "SET  " +
                                      "Actual = @param1 " +
                                      "WHERE Mes = @param2 " +
                                      "AND Año = @param3 " +
                                      "AND Servicio = @param4", con);

                    Busqueda.Parameters.AddWithValue("@param1", C.Actual);
                    Busqueda.Parameters.AddWithValue("@param2", C.Mes);
                    Busqueda.Parameters.AddWithValue("@param3", C.Año);
                    Busqueda.Parameters.AddWithValue("@param4", C.Servicio);
                    Busqueda.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }

        List<CXN_ENCUESTASATIS> IEncuestasSatis.InformeMensual(DateTime Fecha, string Servicio, int Cia)
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

                    String Cargar_Hora = "SELECT E.Admision, H.Hor_Pac_Fecha_Cita, E.Estado, E.Observacion, E.P1, E.P2, E.P3, E.P4, E.P5, E.P6 " +
                                         "FROM CXN_ENCUESTASATIS E " +
                                         "INNER JOIN CXN_HORARIO H ON E.Admision = H.Hor_Id " +
                                         "WHERE H.Hor_Pac_Fecha_Cita BETWEEN @param1 AND @param2 " +
                                         "AND H.Hor_Pac_Tipo_Serv = @param3 " +
                                         "AND E.Estado = @param4 " +
                                         "AND H.Hor_Pac_Cia = @param5";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        DateTime d = new DateTime(Fecha.Year, Fecha.Month, 1, 00, 00, 000);

                        int Days = 0;

                        if (Fecha.Month == 1 || Fecha.Month == 3 || Fecha.Month == 5 || Fecha.Month == 7 || Fecha.Month == 8 || Fecha.Month == 10 || Fecha.Month == 12)
                        {
                            Days = 31;
                        }
                        if (Fecha.Month == 2)
                        {
                            Days = 28;
                        }
                        if (Fecha.Month == 4 || Fecha.Month == 6 || Fecha.Month == 9 || Fecha.Month == 11)
                        {
                            Days = 30;
                        }

                        DateTime h = new DateTime(Fecha.Year, Fecha.Month, Days, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = d;
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = h;
                        Carga_Command.Parameters.AddWithValue("@param3", Servicio);
                        Carga_Command.Parameters.AddWithValue("@param4", "H");
                        Carga_Command.Parameters.AddWithValue("@param5", Cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_ENCUESTASATIS> E = new List<CXN_ENCUESTASATIS>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    E.Add(new CXN_ENCUESTASATIS {
                                        Admision = Convert.ToInt32(Lectura_Hora["Admision"]),
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        Estado = Lectura_Hora["Estado"].ToString(),
                                        Observacion = Lectura_Hora["Observacion"].ToString(),
                                        P1 = Convert.ToInt32(Lectura_Hora["P1"]),
                                        P2 = Convert.ToInt32(Lectura_Hora["P2"]),
                                        P3 = Convert.ToInt32(Lectura_Hora["P3"]),
                                        P4 = Convert.ToInt32(Lectura_Hora["P4"]),
                                        P5 = Convert.ToInt32(Lectura_Hora["P5"]),
                                        P6 = Convert.ToInt32(Lectura_Hora["P6"])
                                    });
                                }

                                return E;
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

        CXN_ENCUESTASATIS IEncuestasSatis.getTrimestre(DateTime desde, DateTime hasta, string TServ, int Cia)
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

                    String Cargar_Hora = "SELECT E.Admision, H.Hor_Pac_Fecha_Cita, E.Estado, E.Observacion, E.P1, E.P2, E.P3, E.P4, E.P5, E.P6 " +
                                         "FROM CXN_ENCUESTASATIS E " +
                                         "INNER JOIN CXN_HORARIO H ON E.Admision = H.Hor_Id " +
                                         "WHERE H.Hor_Pac_Fecha_Cita BETWEEN @param1 AND @param2 " +
                                         "AND H.Hor_Pac_Tipo_Serv = @param3 " +
                                         "AND E.Estado = @param4 " +
                                         "AND H.Hor_Pac_Cia = @param5";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = desde;
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = hasta;
                        Carga_Command.Parameters.AddWithValue("@param3", TServ);
                        Carga_Command.Parameters.AddWithValue("@param4", "H");
                        Carga_Command.Parameters.AddWithValue("@param5", Cia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_ENCUESTASATIS> listaRes = new List<CXN_ENCUESTASATIS>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    listaRes.Add(new CXN_ENCUESTASATIS { 
                                        P1 = Convert.ToInt32(Lectura_Hora["P1"]),
                                        P2 = Convert.ToInt32(Lectura_Hora["P2"]),
                                        P3 = Convert.ToInt32(Lectura_Hora["P3"]),
                                        P4 = Convert.ToInt32(Lectura_Hora["P4"]),
                                        P5 = Convert.ToInt32(Lectura_Hora["P5"]),
                                        P6 = Convert.ToInt32(Lectura_Hora["P6"]),
                                    });
                                }

                                CXN_ENCUESTASATIS E = new CXN_ENCUESTASATIS
                                {
                                    P1R1Cantidad = Convert.ToInt32(listaRes.Where(x => x.P1 == 1).Count()).ToString(),
                                    P1R2Cantidad = Convert.ToInt32(listaRes.Where(x => x.P2 == 2).Count()).ToString(),
                                    P1R3Cantidad = Convert.ToInt32(listaRes.Where(x => x.P3 == 3).Count()).ToString(),
                                    P1R4Cantidad = Convert.ToInt32(listaRes.Where(x => x.P4 == 4).Count()).ToString(),
                                    P1R5Cantidad = Convert.ToInt32(listaRes.Where(x => x.P5 == 5).Count()).ToString(),
                                    P1R6Cantidad = Convert.ToInt32(listaRes.Where(x => x.P6 == 6).Count()).ToString(),

                                    P6R1Cantidad = Convert.ToInt32(listaRes.Where(x => x.P6 == 1).Count()).ToString(),
                                    P6R2Cantidad = Convert.ToInt32(listaRes.Where(x => x.P6 == 2).Count()).ToString(),
                                    P6R3Cantidad = Convert.ToInt32(listaRes.Where(x => x.P6 == 3).Count()).ToString(),
                                    P6R4Cantidad = Convert.ToInt32(listaRes.Where(x => x.P6 == 4).Count()).ToString(),
                                    P6R5Cantidad = Convert.ToInt32(listaRes.Where(x => x.P6 == 5).Count()).ToString(),
                                    P6R6Cantidad = Convert.ToInt32(listaRes.Where(x => x.P6 == 6).Count()).ToString(),

                                    TotalEncuestas = listaRes.Count()
                                };

                                return E;
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

    }
}
